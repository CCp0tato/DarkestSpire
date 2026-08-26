using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class Aim : ModCardTemplate
{
    public Aim() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<AimShotPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AimShotPower>(choiceContext, Owner.Creature, DynamicVars["AimShotPower"].IntValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AimShotPower"].UpgradeValueBy(1);
    }
}