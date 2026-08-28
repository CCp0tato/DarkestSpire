using DarkestSpire.DarkestSpire.CardTags;
using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class PointBlankShot : ModCardTemplate
{
    public PointBlankShot() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TempDexterityPower>(5), new CardsVar("Shot", 2), new PowerVarFB<FightBackBasePower>("ShotPower", 1, TargetType.AnyEnemy), new DamageVar("FightBackDamage", 16, ValueProp.Move)];
    protected override HashSet<CardTag> CanonicalTags => [DSCardTag.FightBack, DSCardTag.FightBackSkip, DSCardTag.Shot];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TempDexterityPower>(choiceContext, Owner.Creature, DynamicVars["TempDexterityPower"].IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["TempDexterityPower"].UpgradeValueBy(2);
        DynamicVars["FightBackDamage"].UpgradeValueBy(4);
    }
}
