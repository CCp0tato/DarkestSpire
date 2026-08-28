#!/usr/bin/env python3
"""检查模组内容（卡牌 / 能力 / 遗物 / 药水）的本地化完成情况，输出 md checklist。

扫描规则：
- Card / Potion / Relic：General 位于 DarkestSpire/scripts/General{Type}s，
  角色专属位于 DarkestSpire/scripts/Characters/{角色}/{Type}s。
- Power：只位于 DarkestSpire/scripts/GeneralPowers。
- 本地化文本在 DarkestSpire/localization/{语言}/{type}.json，
  键形如 DARKEST_SPIRE_{TYPE}_{ID}.{field}，ID 由文件名按驼峰转大写蛇形得到。

每个语言生成一份 `Localization - {语言}.md` 到本目录。
条目状态：全部字段已写 -> [x]；部分已写 -> [-]；全未写 -> [ ]。
只要键存在即视为已写（值是否为空字符串不影响）。
"""

import json
import re
from pathlib import Path

CHECKLIST_DIR = Path(__file__).resolve().parent
ROOT = CHECKLIST_DIR.parent
SCRIPTS_DIR = ROOT / "DarkestSpire" / "scripts"
LOCALIZATION_DIR = ROOT / "DarkestSpire" / "localization"

# 每种内容的检查配置（顺序即 md 中的输出顺序）：
#   json          -> localization/{语言}/{json} 文件名
#   fields        -> 一个完整条目所需的全部字段（键后缀）
#   in_characters -> 是否同时检查 Characters/{角色}/{Type}s 目录
TYPES = {
    "Card":   dict(json="cards.json",   fields=["title", "description"],             in_characters=True),
    "Power":  dict(json="powers.json",  fields=["title", "description",
                                                "smartDescription"],                 in_characters=False),
    "Potion": dict(json="potions.json", fields=["title", "description",
                                                "selectionScreenPrompt"],            in_characters=True),
    "Relic":  dict(json="relics.json",  fields=["title", "description", "flavor"],   in_characters=True),
}


def to_upper_snake(name: str) -> str:
    """按驼峰切分并转大写：AaaaBbb -> AAAA_BBB、PppWwPower -> PPP_WW_POWER。"""
    return re.sub(r"(?<=[a-z0-9])(?=[A-Z])", "_", name).upper()


def cs_stems(directory: Path) -> list[str]:
    """目录下所有 .cs 文件的主文件名（排序）；目录不存在则返回空列表。"""
    if not directory.is_dir():
        return []
    return sorted(p.stem for p in directory.glob("*.cs"))


def load_json(path: Path) -> dict:
    """读取本地化 json；文件不存在返回空 dict，损坏则告警并按空处理。"""
    if not path.is_file():
        return {}
    try:
        with path.open(encoding="utf-8-sig") as f:
            return json.load(f)
    except json.JSONDecodeError:
        print(f"[警告] {path} 不是合法的 JSON，已按空文件处理。")
        return {}


def entry_status(stem: str, type_name: str, fields: list[str], data: dict) -> str:
    """单个内容条目的完成状态：[x] / [-] / [ ]。"""
    prefix = f"DARKEST_SPIRE_{type_name.upper()}_{to_upper_snake(stem)}"
    written = sum(1 for field in fields if f"{prefix}.{field}" in data)
    if written == len(fields):
        return "[x]"
    if written == 0:
        return "[ ]"
    return "[-]"


def check_type(type_name: str, config: dict, characters: list[str], data: dict) -> str:
    """生成一个类型的 md 小节文本；没有任何内容时返回空字符串。"""
    fields = config["fields"]

    if not config["in_characters"]:
        # Power：只有 General 一种，直接平铺，不加二级标题
        stems = cs_stems(SCRIPTS_DIR / f"General{type_name}s")
        if not stems:
            return ""
        return "\n".join(f"- {entry_status(s, type_name, fields, data)} {s}" for s in stems)

    # Card / Potion / Relic：General（若存在）+ 各角色专属
    groups = []
    general_stems = cs_stems(SCRIPTS_DIR / f"General{type_name}s")
    if general_stems:
        groups.append(("General", general_stems))
    for char in characters:
        char_stems = cs_stems(SCRIPTS_DIR / "Characters" / char / f"{type_name}s")
        if char_stems:
            groups.append((char, char_stems))

    if not groups:
        return ""
    return "\n\n".join(
        "\n".join([f"## {heading}"]
                  + [f"- {entry_status(s, type_name, fields, data)} {s}" for s in stems])
        for heading, stems in groups
    )


def main() -> None:
    languages = sorted(p.name for p in LOCALIZATION_DIR.iterdir() if p.is_dir())
    if not languages:
        print(f"[警告] {LOCALIZATION_DIR} 下未找到任何语言目录。")
        return

    characters = sorted(p.name for p in (SCRIPTS_DIR / "Characters").iterdir() if p.is_dir())

    for language in languages:
        sections = []
        for type_name, config in TYPES.items():
            data = load_json(LOCALIZATION_DIR / language / config["json"])
            body = check_type(type_name, config, characters, data)
            if body:
                sections.append(f"# {type_name}s\n\n{body}")

        text = "\n\n".join(sections).rstrip() + "\n"
        output = CHECKLIST_DIR / f"Localization - {language}.md"
        output.write_text(text, encoding="utf-8")

        done = text.count("[x]")
        partial = text.count("[-]")
        missing = text.count("[ ]")
        print(f"已生成 {output.relative_to(ROOT)}：完成 {done} / 部分 {partial} / 未写 {missing}")


if __name__ == "__main__":
    main()
