// | 死亡 | Mortis | 状态 | 1 | 你受到致死伤害时免疫该次伤害并获得等量的[灾厄]；[消耗]。 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class MortisPower : ModPowerTemplate
{
    private decimal _preventedDamage;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override decimal ModifyHpLostAfterOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || amount < Owner.CurrentHp)
        {
            return amount;
        }

        _preventedDamage = amount;
        return 0m;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        decimal preventedDamage = _preventedDamage;
        _preventedDamage = 0m;

        await PowerCmd.Apply<DoomPower>(new ThrowingPlayerChoiceContext(), Owner, preventedDamage, null, null);
        await PowerCmd.Remove(this);
    }
}
