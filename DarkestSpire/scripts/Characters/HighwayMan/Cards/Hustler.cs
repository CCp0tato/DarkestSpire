using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class Hustler : ModCardTemplate
{
    public Hustler() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(4)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        CardPile drawPile = PileType.Draw.GetPile(Owner);

        List<CardModel> topCards = drawPile.Cards
            .Take(DynamicVars.Cards.IntValue)
            .ToList();

        if (topCards.Count == 0)
            return;

        int freeHandSlots = Math.Max(
            0,
            CardPile.MaxCardsInHand -
            PileType.Hand.GetPile(Owner).Cards.Count);

        int selectCount = Math.Min(
            DynamicVars.Cards.IntValue / 2,
            Math.Min(topCards.Count, freeHandSlots));

        if (selectCount == 0)
            return;

        List<CardModel> selectedCards = (await CardSelectCmd.FromCombatPile( choiceContext, drawPile, Owner, new CardSelectorPrefs(SelectionScreenPrompt, selectCount), card => topCards.Contains(card))).ToList();

        if (selectedCards.Count > 0)
            await CardPileCmd.Add(selectedCards, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2);
    }
}