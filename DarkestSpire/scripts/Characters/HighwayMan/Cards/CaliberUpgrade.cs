using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class CaliberUpgrade : ModCardTemplate
{
    public CaliberUpgrade() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }
    
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CaliberUpgradePower>(4)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CaliberUpgradePower>(choiceContext, Owner.Creature,
            DynamicVars["CaliberUpgradePower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CaliberUpgradePower"].UpgradeValueBy(2);
    }
}