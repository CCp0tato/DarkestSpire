using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Data.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Godot;

namespace DarkestSpire.Characters.Example;

[RegisterCharacter]
public class ExampleCharacter : ModCharacterTemplate<ExampleCardPool, ExampleRelicPool, ExamplePotionPool>
{
    // public static readonly Color ThemeColor = new(0.42f, 0.65f, 0.72f);

    private const string SceneRoot = $"{Entry.ResPath}/scenes/characters/Example";
    private const string ImageRoot = $"{Entry.ResPath}/images/characters/Example";

    // 角色名称颜色
    public override Color NameColor => new(0.5f, 0.5f, 1f);
    // 能量图标轮廓颜色
    public override Color EnergyLabelOutlineColor => new(0.5f, 0.5f, 1f);
    // 地图绘制颜色
    public override Color MapDrawingColor => new(0.5f, 0.5f, 1f);

    // 人物性别（男女中立）
    public override CharacterGender Gender => CharacterGender.Masculine;

    // 初始血量和金币
    public override int StartingHp => 80;
    public override int StartingGold => 99;

    public override CharacterAssetProfile AssetProfile => CharacterAssetProfiles.Merge(
        CharacterAssetProfiles.Defect(),
        new(
            Scenes: new(
                // 人物模型tscn路径。
                VisualsPath: $"{SceneRoot}/example_character.tscn",
                // 能量表盘tscn路径。
                EnergyCounterPath: $"{SceneRoot}/example_energy_counter.tscn",
                // 商店人物场景。
                MerchantAnimPath: $"{SceneRoot}/example_character_merchant.tscn",
                // 篝火休息场景。
                RestSiteAnimPath: $"{SceneRoot}/example_character_rest_site.tscn"
            ),
            Ui: new(
                // 对于图片，只要是godot支持的格式都可以，例如png,jpg,svg等等，之后不再说明
                // 人物头像路径。自适应大小。
                IconTexturePath: $"{ImageRoot}/character_icon_example.png",
                // 游戏左上角头像、角色统计页头像、每日挑战角色头像。这个是场景而不是图片。参考下方附赠资源搭建。
                IconPath: $"{SceneRoot}/example_icon.tscn",
                // 人物选择背景。
                CharacterSelectBgPath: $"{SceneRoot}/example_bg.tscn",
                // 人物选择图标。
                CharacterSelectIconPath: $"{ImageRoot}/char_select_example.png",
                // 人物选择图标-锁定状态。
                CharacterSelectLockedIconPath: $"{ImageRoot}/char_select_example_locked.png",
                // 人物选择过渡动画。
                CharacterSelectTransitionPath: "res://materials/transitions/ironclad_transition_mat.tres",
                // 地图上的角色标记图标、表情轮盘上的角色头像。
                MapMarkerPath: $"{ImageRoot}/map_marker_example.png"
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

    // 某个字段没写时，RitsuLib 会从占位角色配置里补齐。
    // public override string? PlaceholderCharacterId => "Silent";

    // 攻击和施法动画延迟，以对齐动画
    public override float AttackAnimDelay => 0f;
    public override float CastAnimDelay => 0f;

    // 如果你的人物不需要时间线小故事，加上这句。
    public override bool RequiresEpochAndTimeline => false;

    // 自动转换人物场景，让你不需要手动挂脚本。复制即可。
    protected override NCreatureVisuals? TryCreateCreatureVisuals() => RitsuGodotNodeFactories.CreateFromScenePath<NCreatureVisuals>(AssetProfile.Scenes!.VisualsPath!);

    // 初始卡组，或者在卡牌类上用RegisterCharacterStarterCard就不用写这个
    // protected override IEnumerable<StartingDeckEntry> StartingDeckEntries => [
    //     new(typeof(TestCard), 5)
    // ];

    // 初始遗物，或者在遗物类上用RegisterCharacterStarterRelic就不用写这个
    // protected override IEnumerable<Type> StartingRelicTypes => [
    //     typeof(Akabeko)
    // ];

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}