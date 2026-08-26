// | 马拉松 | Marathon | 能力 | 2/1 | 回合结束时，若本回合未打出攻击牌，则下回合开始时抽 2 张牌并获得 2 点能量；你特别擅长逃跑（彩蛋：狂乱逃离可免费打出） |

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class MarathonPower : ModPowerTemplate
{
    private const int RewardPerStack = 2;

    private bool _rewardNextTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card is FranticEscape && card.Owner == Owner.Player)
        {
            modifiedCost = 0;
            return true;
        }

        return false;
    }

    public override Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;

        _rewardNextTurn = !CombatManager.Instance.History.CardPlaysFinished.Any(entry =>
            entry.HappenedThisTurn(CombatState) &&
            entry.CardPlay.Card.Owner == Owner.Player &&
            entry.CardPlay.Card.Type == CardType.Attack);

        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || !_rewardNextTurn)
            return;

        _rewardNextTurn = false;
        int reward = Amount * RewardPerStack;
        await CardPileCmd.Draw(choiceContext, reward, player);
        await PlayerCmd.GainEnergy(reward, player);
    }
}
