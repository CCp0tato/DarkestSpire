using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;

#nullable enable
namespace DarkestSpire.GeneralPowers;

public sealed class FearPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        this.GetDumbHoverTip()
    ];

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        if (card.Owner.Creature != this.Owner || card.Type != CardType.Skill)
        {
            modifiedCost = originalCost;
            return false;
        }
        modifiedCost = 0M;
        return true;
    }

    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        PileType pileType,
        CardPilePosition position)
    {
        if (card.Owner.Creature != this.Owner)
            return (pileType, position);
        return card.Type != CardType.Skill ? (pileType, position) : (PileType.Exhaust, position);
    }
}