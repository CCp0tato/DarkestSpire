// | 军令状 | MilitaryOath | 能力 | 1 | 获得 5/8 点力量，3 回合后失去 9/11 点力量 |
// 这个能力实现：三回合后失去力量

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class MilitaryOathPower : ModPowerTemplate
{
    private const int DelayTurns = 3;

    private int _turnsRemaining = DelayTurns;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _turnsRemaining = DelayTurns;
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        _turnsRemaining--;
        if (_turnsRemaining > 0)
            return;

        int strengthLoss = Amount;
        Flash();
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -strengthLoss, Owner, null);
    }

    public int TurnsRemaining => _turnsRemaining;
}
