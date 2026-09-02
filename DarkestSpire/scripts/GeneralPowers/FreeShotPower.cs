using DarkestSpire.DarkestSpire.CardTags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class FreeShotPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
    
    private Dictionary<CardModel, int> _cardCosts = new();
    
    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        return ClearShotCardCost();
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        if (cardPlay.Card.Tags.Contains(DSCardTag.Shot))
        {
            await PowerCmd.Decrement(this);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner || power is not FreeShotPower)
            return;
        if (power.Amount > 0)
            await ClearShotCardCost();
        foreach (CardModel card in Owner.Player.Deck.Cards)
        {
            if (card.Tags.Contains(DSCardTag.Shot))
            {
                card.EnergyCost.SetThisCombat(_cardCosts.Keys.Contains(card) ? _cardCosts[card] : card.EnergyCost.GetWithModifiers(CostModifiers.Global));
            }
        }
    }

    private Task ClearShotCardCost()
    {
        foreach (CardModel card in Owner.Player.Deck.Cards)
        {
            if (card.EnergyCost.GetWithModifiers(CostModifiers.All) == 0)
                continue;
            if (card.Tags.Contains(DSCardTag.Shot))
            {
                _cardCosts[card] = card.EnergyCost.GetWithModifiers(CostModifiers.All);
                card.EnergyCost.SetThisCombat(0);
            }
        }
        return Task.CompletedTask;
    }
}

