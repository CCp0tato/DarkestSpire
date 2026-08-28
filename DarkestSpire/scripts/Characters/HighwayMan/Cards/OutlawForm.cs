using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class OutlawForm : ModCardTemplate
{
    public OutlawForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<OutlawFormPower>(1)];


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
            await PowerCmd.Apply<OutlawFormPower>(choiceContext, Owner.Creature, DynamicVars["OutlawFormPower"].IntValue, Owner.Creature, cardPlay.Card);
        else
            await PowerCmd.Apply<OutlawFormLesserPower>(choiceContext, Owner.Creature, DynamicVars["OutlawFormPower"].IntValue, Owner.Creature, cardPlay.Card);
    }
}