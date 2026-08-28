# CardGenerator - 杀戮尖塔2 卡牌代码生成器

## 📖 简介

`CardGenerator.py` 是一个用于快速生成《杀戮尖塔2》(Slay the Spire 2) 模组卡牌 C# 代码文件的 Python 工具。它通过链式调用的方式，让开发者能够以简洁的 Python 语法定义卡牌属性、效果、关键词和升级逻辑，自动生成符合游戏框架的 `.cs` 文件。

## 🚀 快速开始

### 基础用法

```python
from GeneratorTheSpire import GeneratorTheSpire

# 最简示例：生成一张基础攻击牌
card = GeneratorTheSpire(
    character='Ironclad',  # 角色名称
    card_pool='IroncladCardPool',  # 卡池类名
    card_name='StrikeIronclad',  # 卡牌类名
    cost=1,  # 费用
    card_type='Attack',  # 卡牌类型
    card_rarity='Basic',  # 稀有度
    target_type='AnyEnemy'  # 目标类型
)
card.output()  # 生成 StrikeIronclad.cs 文件
```

## 🔧 参数说明

### 构造函数参数

| 参数 | 类型 | 说明 | 可选值 |
|------|------|------|--------|
| `character` | str | 角色名称 | 任意字符串 |
| `card_pool` | str | 卡池类名 | 任意字符串 |
| `card_name` | str | 卡牌类名（会自动去除空格） | 任意字符串 |
| `cost` | int | 能量消耗 | 整数，`-1` 表示无费用 |
| `card_type` | str | 卡牌类型 | `Skill`, `Attack`, `Power`, `Status`, `Curse`, `Quest` |
| `card_rarity` | str | 稀有度 | `Common`, `Uncommon`, `Rare`, `Token`, `Basic`, `Ancient`, `Event`, `Curse`, `Quest`, `Status` |
| `target_type` | str | 目标类型 | `AllEnemies`, `AnyEnemy`, `AnyAlly`, `AllAllies`, `Self`, `RandomEnemy`, `AnyPlayer`, `TargetedNoCreature`, `Osty` |

_ ⚠️ 无效的参数值会被自动设置为 `'None'`

## 📦 链式方法

### `addKeywords(*keywords)`
添加卡牌关键词（自动去重）。

**可用关键词：**
- `Exhaust` - 消耗
- `Ethereal` - 虚无
- `Innate` - 固有
- `Unplayable` - 无法打出
- `Retain` - 保留
- `Sly` - 狡诈
- `Eternal` - 永恒

```python
.addKeywords('Exhaust', 'Innate')  # 生成: [CardKeyword.Exhaust, CardKeyword.Innate]
```

### `addTags(*tags)`
添加卡牌标签（自动去重）。

**可用标签：**
- `Strike` - 打击
- `Defend` - 防御
- `Minion` - 随从
- `OstyAttack` - Osty攻击
- `Shiv` - 小刀

```python
.addTags('Strike', 'Defend')  # 生成: [CardTag.Strike, CardTag.Defend]
```

### `addVars(**dynamics)`
添加动态变量并生成对应的 `OnPlay` 效果。

**支持的动态变量：**
- `DamageVar` - 伤害值
- `BlockVar` - 格挡值
- `HealVar` - 治疗值
- `CardsVar` - 抽牌数量
- `PowerVar_PowerName_` - 力量/增益层数

```python
.addVars(
    DamageVar=6,                          # 造成6点伤害
    BlockVar=5,                           # 获得5点格挡
    HealVar=3,                            # 治疗3点生命
    CardsVar=2,                           # 抽2张牌
    PowerVar_Strength_=2                  # 获得2层力量
)
```

### `addUpgrade(**upgrades)`
定义卡牌升级后的数值变化。

**支持的升级属性：**
- `Damage` - 伤害增量
- `Block` - 格挡增量
- `Heal` - 治疗增量
- `Cards` - 抽牌增量
- `EnergyCost` - 费用变化（负值表示减费）
- `PowerVar_PowerName_` - 力量/增益层数增量

```python
.addUpgrade(
    Damage=3,          # 伤害+3
    Block=3,           # 格挡+3
    EnergyCost=-1      # 费用-1
)
```

### `output()`
生成 `.cs` 文件，文件名为 `{card_name}.cs`。

---

## 📝 完整示例

### 示例 1：基础打击牌
```python
(CardGenerator(
    character='HighwayMan',
    card_name='StrikeHighwayMan',
    card_type='Attack',
    card_rarity='Basic',
    target_type='AnyEnemy',
    card_pool='HighwayManCardPool',
    cost=1
)
.addTags('Strike')
.addVars(DamageVar=6)
.addUpgrade(Damage=3)
.output())
```
*生成一张造成6点伤害，升级后+3伤害的基础打击牌。*

---

### 示例 2：复合效果攻击牌
```python
(CardGenerator(
    character='HighwayMan',
    card_name='SwordCombo',
    card_type='Attack',
    card_rarity='Uncommon',
    target_type='AnyEnemy',
    card_pool='HighwayManCardPool',
    cost=1
)
.addTags('Strike')
.addKeywords('Exhaust')
.addVars(
    DamageVar=4,
    BlockVar=4
)
.addUpgrade(
    Damage=2,
    Block=2
)
.output())
```
*消耗，造成4点伤害并获得4点格挡，升级后伤害和格挡各+2。*

---

### 示例 3：力量增益牌（Power）
```python
(CardGenerator(
    character='Ironclad',
    card_name='DemonicStrength',
    card_type='Power',
    card_rarity='Rare',
    target_type='Self',
    card_pool='IroncladCardPool',
    cost=2
)
.addKeywords('Ethereal')
.addVars(
    PowerVar_Strength_=3,
    PowerVar_Weak_=1
)
.addUpgrade(
    PowerVar_Strength_=2,
    PowerVar_Weak_=0
)
.output())
```
*虚无。获得3层力量和1层虚弱（升级后力量+2，虚弱不再增加）。*

---

### 示例 4：抽牌技能
```python
(CardGenerator(
    character='Silent',
    card_name='QuickDraw',
    card_type='Skill',
    card_rarity='Common',
    target_type='Self',
    card_pool='SilentCardPool',
    cost=0
)
.addKeywords('Retain')
.addVars(
    CardsVar=2,
    HealVar=3
)
.addUpgrade(
    CardsVar=1,
    HealVar=2
)
.output())
```
*0费保留。抽2张牌并治疗3点生命，升级后抽牌+1，治疗+2。*

---

### 示例 5：多目标AOE攻击
```python
(CardGenerator(
    character='HighwayMan',
    card_name='WhirlwindSlash',
    card_type='Attack',
    card_rarity='Rare',
    target_type='AllEnemies',
    card_pool='HighwayManCardPool',
    cost=2
)
.addTags('Strike')
.addKeywords('Exhaust', 'Innate')
.addVars(DamageVar=12)
.addUpgrade(
    Damage=6,
    EnergyCost=-1
)
.output())
```
*固有、消耗。对所有敌人造成12点伤害，升级后伤害+6且费用-1。*

---

## 📂 生成的文件结构

生成的 C# 文件将包含：
- 完整的命名空间声明
- 自动注册属性 `[RegisterCard]`
- 卡图资源配置 `AssetProfile`
- 动态变量定义
- `OnPlay` 异步执行逻辑
- `OnUpgrade` 升级逻辑

## 📌 注意事项

1. **文件命名**：`card_name` 会自动移除空格，确保符合 C# 类命名规范。`PowerVar_PowerName_` 中的 `PowerName`  实际皆以 `Power` 结尾，如力量为 `StrengthPower` 而不是 `Strength`。
2. **参数校验**：无效的枚举值会被自动修正为 `None`，不会抛出异常。
3. **动态变量**：`PowerVar_PowerName_` 中的 `PowerName` 必须是游戏中已定义的Power类型。
4. **输出位置**：生成的 `.cs` 文件位于脚本运行目录。
