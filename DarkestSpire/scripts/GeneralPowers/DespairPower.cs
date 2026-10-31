using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class DespairPower : UniquePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsUniquePower => true;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    private int _cardPlayedTurn = 0;
    
    private int _energyLeftTurn = 0;
    
    private int _cardsLeftTurn = 0;

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 只统计持有者自己打出的牌，避免多人联机时全队共享计数。
        if (cardPlay.Card.Owner == Owner.Player)
            _cardPlayedTurn++;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        _cardPlayedTurn = 0;

        await CardPileCmd.Draw(choiceContext, (int) _cardsLeftTurn / 2, player);
        await PlayerCmd.GainEnergy(_energyLeftTurn, player);
        
        _energyLeftTurn = 0;
        _cardsLeftTurn = 0;
    }

    public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return Task.CompletedTask;
        // 必须在回合结束清空能量/手牌之前记录；AfterSideTurnEnd 时能量已经被重置。
        _energyLeftTurn = Owner.Player!.GetEnergy();
        IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(Owner.Player!).Cards;
        _cardsLeftTurn = cards.Count;
        return Task.CompletedTask;
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        return card.Owner != Owner.Player || _cardPlayedTurn <= 5;
    }
}