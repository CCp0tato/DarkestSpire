using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class BleedingPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (dealer != Owner)
            return;
        // 必须使用带 dealer/cardSource 的重载。
        // 5 参数重载（Damage(..., CardModel cardSource)）内部会直接读取 cardSource.Owner.Creature，
        // 这里没有来源卡（cardSource 为 null），会抛 NullReferenceException，
        // 使敌方回合的伤害结算中断并导致战斗卡死。
        // 以 dealer/cardSource 均显式传 null 的方式结算“失去生命”，
        // 同时避免 dealer 指向自己而再次触发流血形成递归。
        await CreatureCmd.Damage(choiceContext, Owner, (Decimal) Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        // 与原版中毒一致：若流血把自己打死了就不再扣层数。
        if (Owner.IsAlive)
            await PowerCmd.Decrement(this);
    }
}