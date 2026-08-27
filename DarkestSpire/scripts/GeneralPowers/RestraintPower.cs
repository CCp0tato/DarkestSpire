// | 战术预测-制约 | Restraint | 能力 | 0 | 打出攻击牌时，若自身造成的伤害与目标意图的伤害相差在 3/4 以内，则将目标击晕（按单段伤害计算） |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Combat.AttackHits;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class RestraintPower : ModPowerTemplate, IAttackHitHookListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public async Task AfterAttackHit(AttackHitContext context)
    {
        if (context.Dealer != Owner || context.CardSource?.Type != CardType.Attack)
            return;

        foreach (var result in context.Results)
        {
            var target = result.Receiver;
            if (target.IsDead || target.Monster is not { } monster)
                continue;

            var attackIntent = monster.NextMove.Intents.OfType<AttackIntent>().FirstOrDefault();
            if (attackIntent == null)
                continue;

            var intentDamage = attackIntent.GetSingleDamage(context.CombatState.PlayerCreatures, monster.Creature);
            if (Math.Abs(result.UnblockedDamage - intentDamage) <= Amount)
                await CreatureCmd.Stun(target);
        }
    }
}
