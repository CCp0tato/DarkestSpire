using DarkestSpire.Characters.HighwayMan.Cards;
using DarkestSpire.DarkestSpire.CardTags;
using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Models;

namespace DarkestSpire.DSSingleton;

public class ShotSingleton : HookedSingletonModel
{
    public ShotSingleton() : base(HookType.Combat)
    {
    }
    
    public int _cardsPlayedThisTurn;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        if (!card.Tags.Contains(DSCardTag.Shot))
            return;
        int maxDropValue = card.DynamicVars["Shot"].IntValue;
        
        if (maxDropValue == 0)
        {
            await ShotEvent(cardPlay, choiceContext, 0, 0);
            return;
        }
        if (maxDropValue != -1)
        {
            IEnumerable<CardModel> discardCards = await CardSelectCmd.FromHandForDiscard(choiceContext, card.Owner,
                new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 0, maxDropValue), null,
                card);
            await ShotEvent(cardPlay, choiceContext, discardCards.Count(), maxDropValue);
        }
        else
        {
            IEnumerable<CardModel> discardCards = await CardSelectCmd.FromHandForDiscard(choiceContext, card.Owner,
                new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 0, 9999999), null,
                card);
            await ShotEvent(cardPlay, choiceContext, 99999999, 0, discardCards.Count());
        }
    }

    private async Task ShotEvent(CardPlay shotCardPlay, PlayerChoiceContext choiceContext, int discardCount, int shotCount, int shotTimes = 1)
    {
        CardModel shotCard = shotCardPlay.Card;
        int gapCount = shotCount - discardCount;
        if (gapCount > 0)
        {
            if (!shotCard.Owner.Creature.HasPower<GhostBulletPower>())
                return;
            await PowerCmd.Apply<DrawCardsNextTurnPower>(new ThrowingPlayerChoiceContext(), shotCard.Owner.Creature,
                0 - gapCount, null, null);
        }
        
        foreach (CardModel card in PileType.Discard.GetPile(shotCard.Owner).Cards)
        {
            if (card is DoubleBarrel)
                await CardPileCmd.Add(card, PileType.Hand);
        }
        
        IEnumerable<DynamicVar> cardVars = shotCard.DynamicVars.Values.Where((c) => c.Name.Contains("Shot"));
        for (int i = 0; i < shotTimes; i++)
        {
            foreach (DynamicVar dynamicVar in cardVars)
            {
                if (dynamicVar is DamageVar)
                {
                    if (shotCard.TargetType == TargetType.AllEnemies)
                        await DamageCmd.Attack(dynamicVar.IntValue).FromCard(shotCard)
                            .TargetingAllOpponents(shotCard.Owner.Creature.CombatState!).Execute(choiceContext);
                    else
                        await DamageCmd.Attack(dynamicVar.IntValue).FromCard(shotCard)
                            .Targeting(shotCardPlay.Target!).Execute(choiceContext);
                }
                else if (dynamicVar is EnergyVar)
                {
                    await PlayerCmd.GainEnergy(dynamicVar.IntValue, shotCard.Owner);
                }
                else if (dynamicVar is BlockVar)
                {
                    await CreatureCmd.GainBlock(shotCard.Owner.Creature, (BlockVar)dynamicVar, (CardPlay)null!);
                }
                else if (dynamicVar is GoldVar)
                {
                    await PlayerCmd.GainGold(dynamicVar.IntValue, shotCard.Owner);
                }
                else if (dynamicVar is PowerVarFB<PowerModel> powerVar)
                {
                    Creature powerTargets = (powerVar.targetType == TargetType.Self)
                        ? shotCard.Owner.Creature
                        : shotCardPlay.Target!;
                    await PowerCmd.Apply(choiceContext, powerVar.powerType, powerTargets,
                        powerVar.IntValue, shotCard.Owner.Creature, (CardModel)null!);
                }
            }
        }
    }
}