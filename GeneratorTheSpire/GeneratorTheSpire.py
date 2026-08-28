import argparse
import os
from typing import overload


class CardGenerator:
    def __init__(self, character: str, card_pool: str, card_name: str, cost: int,
                 card_type: str, card_rarity: str, target_type: str):
        if ' ' in card_name:
            card_name = card_name.replace(' ', '')
        if cost is None:
            cost = -1
        if card_type not in {'Skill', 'Attack', 'Power', 'Status', 'Curse', 'Quest', }:
            card_type = 'None'
        if card_rarity not in {'Common', 'Uncommon', 'Rare', 'Token', 'Basic', 'Ancient',
                               'Event', 'Curse', 'Quest', 'Status'}:
            card_rarity = 'None'
        if target_type not in {'AllEnemies', 'AnyEnemy', 'AnyAlly', 'AllAllies', 'Self', 'RandomEnemy', 'AnyPlayer',
                               'TargetedNoCreature', 'Osty'}:
            target_type = 'None'

        self.card_name = card_name

        self.card_string = f'''using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;

namespace DarkestSpire.Characters.{character}.Cards;

[RegisterCard(typeof({card_pool}))]
public class {card_name} : ModCardTemplate
{{
    public {card_name}() : base({cost}, CardType.{card_type}, CardRarity.{card_rarity}, TargetType.{target_type}, true)
    {{
    }}


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{{{card_pool}.getImageRoot()}}/{{GetType().Name}}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
'''

    def addKeywords(self, *keywords):
        filtered_kw = []
        for keyword in keywords:
            if keyword not in {'Exhaust', 'Ethereal', 'Innate', 'Unplayable', 'Retain', 'Sly', 'Eternal', }:
                filtered_kw.append('None')
                continue
            filtered_kw.append(keyword)
        filtered_kw = list(set(filtered_kw))

        connected_keywords = ', '.join([f'CardKeyword.{keyword}' for keyword in filtered_kw])

        self.card_string += f'''
    public override IEnumerable<CardKeyword> CanonicalKeywords => [{connected_keywords}];
'''
        return self

    def addTags(self, *tags):
        filtered_tags = []
        for tag in tags:
            if tag not in {'Strike', 'Defend', 'Minion', 'OstyAttack', 'Shiv', }:
                filtered_tags.append('None')
                continue
            filtered_tags.append(tag)
        filtered_tags = list(set(filtered_tags))

        connected_tags = ', '.join([f'CardTag.{tag}' for tag in filtered_tags])
        self.card_string += f'''
    protected override HashSet<CardTag> CanonicalTags => [{connected_tags}];
'''
        return self

    def addVars(self, **dynamics):
        connected_dv = []
        connneced_onplay = []
        for dynamic_var in dynamics:
            if not (dynamic_var in {'DamageVar', 'BlockVar', 'CardsVar', 'HealVar',
                                    'EnergyVar'} or 'PowerVar' in dynamic_var):
                continue
            match dynamic_var:
                case 'DamageVar':
                    connected_dv.append(f'new {dynamic_var}({dynamics[dynamic_var]}, ValueProp.Move)')
                    connneced_onplay.append(
                        'await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);')
                case 'BlockVar':
                    connected_dv.append(f'new {dynamic_var}({dynamics[dynamic_var]}, BlockProps.card)')
                    connneced_onplay.append(
                        'await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, cardPlay);')
                case 'HealVar':
                    connected_dv.append(f'new {dynamic_var}({dynamics[dynamic_var]})')
                    connneced_onplay.append('await CreatureCmd.Heal(Owner.Creature, this.DynamicVars.Heal.IntValue);')
                case 'CardsVar':
                    connected_dv.append(f'new {dynamic_var}({dynamics[dynamic_var]})')
                    connneced_onplay.append('await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);')
                case 'EnergyVar':
                    connected_dv.append(f'new {dynamic_var}({dynamics[dynamic_var]})')
                    connneced_onplay.append('await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);')
                case _:
                    powername = dynamic_var.replace('PowerVar_', '').replace('_', '')
                    connected_dv.append(f'new PowerVar<{powername}>({dynamics[dynamic_var]})')
                    connneced_onplay.append(
                        f'await PowerCmd.Apply<{powername}>(choiceContext, Owner.Creature, DynamicVars["{powername}"].IntValue, Owner.Creature, cardPlay.Card);')
        self.card_string += f'''
    protected override IEnumerable<DynamicVar> CanonicalVars => [{', '.join(connected_dv)}];
'''
        self.card_string += f'''
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {{
        {'\n        '.join(connneced_onplay)}
    }}
'''
        return self

    def addUpgrade(self, **upgrades):
        connneced_onplay = []
        for upgrade in upgrades:
            if not (upgrade in {'Damage', 'Block', 'Cards', 'Heal', 'EnergyCost'} or 'PowerVar' in upgrade):
                continue
            match upgrade:
                case 'Damage':
                    connneced_onplay.append(f'DynamicVars.{upgrade}.UpgradeValueBy({upgrades[upgrade]});')
                case 'Block':
                    connneced_onplay.append(f'DynamicVars.{upgrade}.UpgradeValueBy({upgrades[upgrade]});')
                case 'Heal':
                    connneced_onplay.append(f'DynamicVars.{upgrade}.UpgradeValueBy({upgrades[upgrade]});')
                case 'Cards':
                    connneced_onplay.append(f'DynamicVars.{upgrade}.UpgradeValueBy({upgrades[upgrade]});')
                case 'EnergyCost':
                    connneced_onplay.append(f'{upgrade}.UpgradeBy({upgrades[upgrade]});')
                case _:
                    powername = upgrade.replace('PowerVar_', '').replace('_', '')
                    connneced_onplay.append(f'DynamicVars["{powername}"].UpgradeValueBy({upgrades[upgrade]});')
        self.card_string += f'''
    protected override void OnUpgrade()
    {{
        {'\n        '.join(connneced_onplay)}
    }}
'''
        return self

    def setMultiplayer(self, multi: bool = True):
        self.card_string += f'''    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.{'Multiplayer' if multi else 'Singleplayer'}Only;'''

    def output(self, output_dir: str = '.', stringForm=False):
        self.card_string += '''}'''

        if stringForm:
            return self.card_string

        os.makedirs(output_dir, exist_ok=True)
        output_path = os.path.join(output_dir, f'{self.card_name}.cs')
        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(self.card_string)
        return output_path


class CharacterGenerator:
    def __init__(self, character_name: str, StartingHp: int, StartingGold: int):
        self.character_name = character_name

        self.basic_string = f'''using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Godot;

namespace DarkestSpire.Characters.{self.character_name};

[RegisterCharacter]
public class {self.character_name}Character : ModCharacterTemplate<{self.character_name}CardPool, {self.character_name}RelicPool, {self.character_name}PotionPool>
{{
    // public static readonly Color ThemeColor = new(0.42f, 0.65f, 0.72f);
    private const string CharacterName = "{self.character_name}";
    private const string SceneRoot = $"{{Entry.ResPath}}/scenes/characters/{{CharacterName}}";
    private const string ImageRoot = $"{{Entry.ResPath}}/images/characters/{{CharacterName}}";

    public override Color NameColor => new(0.5f, 0.5f, 1f);
    public override Color EnergyLabelOutlineColor => new(0.5f, 0.5f, 1f);
    public override Color MapDrawingColor => new(0.5f, 0.5f, 1f);

    public override CharacterGender Gender => CharacterGender.Masculine;

    public override int StartingHp => {StartingHp};
    public override int StartingGold => {StartingGold};

    public override CharacterAssetProfile AssetProfile => CharacterAssetProfiles.Merge(
        CharacterAssetProfiles.Defect(),
        new(
            Scenes: new(
                VisualsPath: $"{{SceneRoot}}/{self.character_name.lower()}_character.tscn",
                EnergyCounterPath: $"{{SceneRoot}}/{self.character_name.lower()}_energy_counter.tscn",
                MerchantAnimPath: $"{{SceneRoot}}/{self.character_name.lower()}_character_merchant.tscn",
                RestSiteAnimPath: $"{{SceneRoot}}/{self.character_name.lower()}_character_rest_site.tscn"
            ),
            Ui: new(
                // 对于图片，只要是godot支持的格式都可以，例如png,jpg,svg等等，之后不再说明
                // 人物头像路径。自适应大小。
                IconTexturePath: $"{{ImageRoot}}/character_icon_{self.character_name.lower()}.png",
                // 游戏左上角头像、角色统计页头像、每日挑战角色头像。这个是场景而不是图片。参考下方附赠资源搭建。
                IconPath: $"{{SceneRoot}}/{self.character_name.lower()}_icon.tscn",
                // 人物选择背景。
                CharacterSelectBgPath: $"{{SceneRoot}}/{self.character_name.lower()}_bg.tscn",
                // 人物选择图标。
                CharacterSelectIconPath: $"{{ImageRoot}}/char_select_{self.character_name.lower()}.png",
                // 人物选择图标-锁定状态。
                CharacterSelectLockedIconPath: $"{{ImageRoot}}/char_select_{self.character_name.lower()}_locked.png",
                // 人物选择过渡动画。
                CharacterSelectTransitionPath: "res://materials/transitions/ironclad_transition_mat.tres",
                // 地图上的角色标记图标、表情轮盘上的角色头像。
                MapMarkerPath: $"{{ImageRoot}}/map_marker_{self.character_name.lower()}.png"
            ),
            Vfx: new(
                // 卡牌拖尾场景。
                // TrailPath: "res://scenes/vfx/card_trail_ironclad.tscn"
            ),
            Audio: new(
                // 攻击音效
                // AttackSfx: null,
                // 施法音效
                // CastSfx: null,
                // 死亡音效
                // DeathSfx: null,
                // 角色选择音效
                // CharacterSelectSfx: null,
                // 过渡音效
                // CharacterTransitionSfx: "event:/sfx/ui/wipe_ironclad"
            ),
            Multiplayer: new(
                // 多人模式-手指。
                // ArmPointingTexturePath: null,
                // 多人模式剪刀石头布-石头。
                // ArmRockTexturePath: null,
                // 多人模式剪刀石头布-布。
                // ArmPaperTexturePath: null,
                // 多人模式剪刀石头布-剪刀。
                // ArmScissorsTexturePath: null
            )
            // 其余如果有需要自行取消注释使用
            // Spine: null,
            // VisualCues: null, // 帧动画静态图人物使用，查看角色动画一章
            // WorldProceduralVisuals: null,
            // 以下为让遗物根据你的人物展现不同的图像资源，在列表里添加即可
            // VanillaCardVisualOverrides: [],
            // VanillaRelicVisualOverrides: [
            //     new (CharacterOwnedVanillaRelicModelId.YummyCookie, new("res://icon.svg")) // 美味饼干覆盖
            // ],
            // VanillaPotionVisualOverrides: []
        ));

    public override string? PlaceholderCharacterId => "Defect";

    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    public override bool RequiresEpochAndTimeline => false;

    protected override NCreatureVisuals? TryCreateCreatureVisuals() => RitsuGodotNodeFactories.CreateFromScenePath<NCreatureVisuals>(AssetProfile.Scenes!.VisualsPath!);

    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}}'''
        self.card_pool = f'''using Godot;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace DarkestSpire.Characters.{self.character_name};

public class {self.character_name}CardPool : TypeListCardPoolModel
{{
    private const string CharacterName = "{self.character_name}";
    private const string ImageRoot = $"{{Entry.ResPath}}/images/characters/{{CharacterName}}";

    public override string Title => "{self.character_name}CardPool";
    public override string EnergyColorName => "{self.character_name}EnergyColor";

    public override string? TextEnergyIconPath => $"{{ImageRoot}}/energy_{self.character_name.lower()}.png";
    public override string? BigEnergyIconPath => $"{{ImageRoot}}/energy_{self.character_name.lower()}_big.png";

    public override Color DeckEntryCardColor => new(0.5f, 0.5f, 1f);
    public override Color EnergyOutlineColor => new(0.5f, 0.5f, 1f);

    private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateReplaceHueShaderMaterial(0.5f, 0.5f, 1f); // 如果你使用原版卡框，使用这个直接替换色调。
    // private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateRgbShaderMaterial(0.5f, 0.5f, 1f); // 使用原版卡框替换色调。除非你的版本没有CreateReplaceHueShaderMaterial函数，否则应使用上面那种
    // private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateUnmodulatedHsvShaderMaterial(); // 如果你是自定义卡框，使用这个
    public override Material? PoolFrameMaterial => _poolFrameMaterial;

    public override bool IsColorless => false;

    public static string getImageRoot()
    {{
        return ImageRoot;
    }}
}}'''
        self.potion_pool = f'''using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.{self.character_name};

public class {self.character_name}PotionPool : TypeListPotionPoolModel
{{
    private const string CharacterName = "{self.character_name}";
    private const string ImageRoot = $"{{Entry.ResPath}}/images/characters/{{CharacterName}}";

    public override string? TextEnergyIconPath => $"{{ImageRoot}}/energy_{self.character_name.lower()}.png";
    public override string? BigEnergyIconPath => $"{{ImageRoot}}/energy_{self.character_name.lower()}_big.png";

    public override string EnergyColorName => "{self.character_name.lower()}Energy";
}}'''
        self.relic_pool = f'''using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.{self.character_name};

public class {self.character_name}RelicPool : TypeListRelicPoolModel
{{
    private const string CharacterName = "{self.character_name}";
    private const string ImageRoot = $"{{Entry.ResPath}}/images/characters/{{CharacterName}}";
    
    public override string? TextEnergyIconPath => $"{{ImageRoot}}/energy_{self.character_name.lower()}.png";
    public override string? BigEnergyIconPath => $"{{ImageRoot}}/energy_{self.character_name.lower()}_big.png";

    public override string EnergyColorName => "{self.character_name.lower()}Energy";
}}'''

        self.struct = {"Cards": {}, "Events": {}, "Potions": {}, "Relics": {},
                       f"{self.character_name}": self.basic_string,
                       f"{self.character_name}CardPool": self.card_pool,
                       f"{self.character_name}PotionPool": self.potion_pool,
                       f"{self.character_name}RelicPool": self.relic_pool}

    @overload
    def AddCard(self, card_name: str, card: str):
        ...

    @overload
    def AddCard(self, cards: dict[str, str]):
        ...

    @overload
    def AddCard(self, card_names: list[str], cards: list[str]):
        ...

    def AddCard(self, *args):
        if len(args) == 1 and isinstance(args[0], dict):
            for cardname in args[0]:
                self.struct["Cards"][cardname] = args[0][cardname]
            return self

        if len(args) > 2:
            return self

        card_name, card = args

        if isinstance(card_name, str) and isinstance(card, str):
            self.struct["Cards"][card_name] = card
            return self

        if isinstance(card_name, list) and isinstance(card, list):
            for n, c in zip(card_name, card):
                self.struct["Cards"][n] = c
            return self

        return self

    @overload
    def AddRelic(self, relic_name: str, relic: str):
        ...

    @overload
    def AddRelic(self, relics: dict[str, str]):
        ...

    @overload
    def AddRelic(self, relic_names: list[str], relics: list[str]):
        ...

    def AddRelic(self, *args):
        if len(args) == 1 and isinstance(args[0], dict):
            for relicname in args[0]:
                self.struct["Relics"][relicname] = args[0][relicname]
            return self

        if len(args) > 2:
            return self

        relic_name, relic = args

        if isinstance(relic_name, str) and isinstance(relic, str):
            self.struct["Relics"][relic_name] = relic
            return self

        if isinstance(relic_name, list) and isinstance(relic, list):
            for n, c in zip(relic_name, relic):
                self.struct["Relics"][n] = c
            return self

        return self

    @overload
    def AddPotion(self, potion_name: str, potion: str):
        ...

    @overload
    def AddPotion(self, potions: dict[str, str]):
        ...

    @overload
    def AddPotion(self, potion_names: list[str], potions: list[str]):
        ...

    def AddPotion(self, *args):
        if len(args) == 1 and isinstance(args[0], dict):
            for potion_name in args[0]:
                self.struct["Potions"][potion_name] = args[0][potion_name]
            return self

        if len(args) > 2:
            return self

        potion_name, potion = args

        if isinstance(potion_name, str) and isinstance(potion, str):
            self.struct["Potions"][potion_name] = potion
            return self

        if isinstance(potion_name, list) and isinstance(potion, list):
            for n, c in zip(potion_name, potion):
                self.struct["Potions"][n] = c
            return self

        return self

    def WithBasicStrike(self):
        self.AddCard(f"Strike{self.character_name}", (
            CardGenerator(character=self.character_name, card_name=f"Strike{self.character_name}",
                          card_type='Attack', card_rarity='Basic', target_type='AnyEnemy',
                          card_pool=f"{self.character_name}CardPool", cost=1)
            .addVars(DamageVar=6)
            .addTags('Strike')
            .addUpgrade(Damage=3)
            .output(stringForm=True)))
        return self

    def WithBasicDefend(self):
        self.AddCard(f"Defend{self.character_name}", (
            CardGenerator(character=self.character_name, card_name=f"Defend{self.character_name}", card_type='Skill',
                          card_rarity='Basic', target_type='Self',
                          card_pool=f"{self.character_name}CardPool", cost=1)
            .addVars(BlockVar=5)
            .addUpgrade(Block=3)
            .output(stringForm=True)))
        return self

    def WithBasicStrikeAndDefend(self):
        return self.WithBasicStrike().WithBasicDefend()

    def WithBasicRelic(self) -> 'CharacterGenerator':
        return self.AddRelic(f"{self.character_name}StarterRelic", f'''using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.{self.character_name}.Relics;

[RegisterRelic(typeof({self.character_name}RelicPool))]
[RegisterCharacterStarterRelic(typeof({self.character_name}Character))] 
public class {self.character_name}StarterRelic : ModRelicTemplate
{{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: $"{{Entry.ResPath}}/images/relics/{{GetType().Name}}.png",
        IconOutlinePath: $"{{Entry.ResPath}}/images/relics/{{GetType().Name}}.png",
        BigIconPath: $"{{Entry.ResPath}}/images/relics/{{GetType().Name}}.png"
    );
}}''')

    @staticmethod
    def buildStructure(structure: dict, root_dir='.'):
        for i in structure:
            if isinstance(structure[i], str):
                with open(f"{root_dir}/{i}.cs", 'w', encoding='utf-8') as f:
                    f.write(structure[i])
            elif isinstance(structure[i], dict):
                os.makedirs(f"{root_dir}/{i}", exist_ok=True)
                CharacterGenerator.buildStructure(structure[i], f"{root_dir}/{i}")
        return None

    def __getitem__(self, item):
        return self.struct[item]

    def output(self, item=None, rootdir='.'):
        if item:
            return self[item]
        return self.buildStructure({self.character_name: self.struct}, root_dir=rootdir)


def cli():
    parser = argparse.ArgumentParser(
        description='Generate Slay the Spire 2 mod card C# file.'
    )
    parser.add_argument('--character', required=True, help='Character name')
    parser.add_argument('--card-pool', required=True, help='Card pool class name')
    parser.add_argument('--card-name', required=True, help='Card class name, English only recommended')
    parser.add_argument('--cost', type=int, default=-1, help='Energy cost')
    parser.add_argument('--card-type', required=True, help='Skill/Attack/Power/Status/Curse/Quest')
    parser.add_argument('--card-rarity', required=True,
                        help='Common/Uncommon/Rare/Token/Basic/Ancient/Event/Curse/Quest/Status')
    parser.add_argument('--target-type', required=True,
                        help='AllEnemies/AnyEnemy/AnyAlly/AllAllies/Self/RandomEnemy/AnyPlayer/TargetedNoCreature/Osty')
    parser.add_argument('--keywords', nargs='*', default=[], help='Keywords list')
    parser.add_argument('--tags', nargs='*', default=[], help='Tags list')
    parser.add_argument('--var', action='append', default=[],
                        help='Dynamic variable in key=value form, e.g. --var DamageVar=6 --var BlockVar=5')
    parser.add_argument('--upgrade', action='append', default=[],
                        help='Upgrade in key=value form, e.g. --upgrade Damage=3 --upgrade EnergyCost=-1')
    parser.add_argument('--output-dir', default='.', help='Output directory for generated .cs file')
    args = parser.parse_args()

    def parse_key_value(items):
        result = {}
        for item in items:
            if '=' not in item:
                continue
            key, value = item.split('=', 1)
            key = key.strip()
            value = value.strip()
            try:
                value = int(value)
            except ValueError:
                continue
            result[key] = value
        return result

    vars_dict = parse_key_value(args.var)
    upgrades_dict = parse_key_value(args.upgrade)
    card = CardGenerator(
        character=args.character,
        card_pool=args.card_pool,
        card_name=args.card_name,
        cost=args.cost,
        card_type=args.card_type,
        card_rarity=args.card_rarity,
        target_type=args.target_type
    )
    if args.keywords:
        card.addKeywords(*args.keywords)
    if args.tags:
        card.addTags(*args.tags)
    if vars_dict:
        card.addVars(**vars_dict)
    if upgrades_dict:
        card.addUpgrade(**upgrades_dict)
    output_path = card.output(args.output_dir)
    print(output_path)


def generator_all_chars(index=-1):
    all_chars = ["Vestal", "PlagueDoctor", "Hellion", "BountyHunter", "GraveRobber", "Occultist", "Jester", "Leper",
                 "Arbalest", "ManAtArms", "HoundMaster", "Flagellant", "ShieldBreaker", "Antiquarian", "Abomination"]
    if index >= 0:
        CharacterGenerator(all_chars[index], 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
        return

    CharacterGenerator('Vestal', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('PlagueDoctor', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('Hellion', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('BountyHunter', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('GraveRobber', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('Occultist', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('Jester', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('Leper', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('Arbalest', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('ManAtArms', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('HoundMaster', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('Flagellant', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('ShieldBreaker', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('Antiquarian', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()
    CharacterGenerator('Abomination', 80, 99).WithBasicStrikeAndDefend().WithBasicRelic().output()


if __name__ == '__main__':
    generator_all_chars(0)
    cli()
