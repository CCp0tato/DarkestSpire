using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class Ambush : ModCardTemplate
{
    public Ambush() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new PowerVar<DrawCardsNextTurnPower>(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = (await CardSelectCmd.FromHandForDiscard(choiceContext, this.Owner, new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1), (Func<CardModel, bool>) null, (AbstractModel) this)).FirstOrDefault<CardModel>();
        if (card != null)
            await CardCmd.Discard(choiceContext, card);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner.Creature, DynamicVars["DrawCardsNextTurnPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DrawCardsNextTurnPower"].UpgradeValueBy(1);
    }
}