using DarkestSpire.DarkestSpire.CardTags;
using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class SerratedBlade : ModCardTemplate
{
    public SerratedBlade() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVarFB<BleedingPower>(7, TargetType.AnyEnemy)];
    protected override HashSet<CardTag> CanonicalTags => [DSCardTag.FightBack];

    // 流血由 FightBackSingleton 在卡牌打出后创建的 FightBackBasePower 施加。
    // 旧代码在 OnPlay 中把流血施加给了自己，并且引用了不存在的 "BleedingPower" 动态变量，
    // 打出或升级这张牌时会抛 KeyNotFoundException。

    protected override void OnUpgrade()
    {
        DynamicVars["FightBackPower"].UpgradeValueBy(3);
    }
}