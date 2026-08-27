// | 退化光环 | DegenerationAura | 能力 | 2 | 回合结束时，使所有意图为攻击的敌人失去 1/2 点力量 |

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class DegenerationAuraPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;

        List<Creature> attackingEnemies = Owner.CombatState!.Enemies
            .Where(enemy => enemy.IsAlive && enemy.Monster!.IntendsToAttack)
            .ToList();

        if (attackingEnemies.Count == 0)
            return;

        Flash();
        foreach (Creature enemy in attackingEnemies)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, enemy, -Amount, Owner, null);
        }
    }

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
}
