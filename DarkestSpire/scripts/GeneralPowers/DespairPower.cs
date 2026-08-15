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
public class DespairPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    
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
        _cardPlayedTurn++;
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        _cardPlayedTurn = 0;

        CardPileCmd.Draw(choiceContext, (int) _cardsLeftTurn / 2, player);
        PlayerCmd.GainEnergy(_energyLeftTurn, player);
        
        _energyLeftTurn = 0;
        _cardsLeftTurn = 0;
        
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return Task.CompletedTask;
        _energyLeftTurn = Owner.Player.GetEnergy();
        IReadOnlyList<CardModel> cards = PileType.Hand.GetPile(Owner.Player).Cards;
        _cardsLeftTurn = cards.Count;
        return Task.CompletedTask;
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        return _cardPlayedTurn <= 5;
    }
}