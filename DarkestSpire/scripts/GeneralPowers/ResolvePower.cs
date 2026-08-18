using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class ResolvePower : UniquePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsUniquePower => true;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    private bool gainedBlockTurn => CombatManager.Instance.History.Entries.OfType<BlockGainedEntry>()
        .Count<BlockGainedEntry>(
            (Func<BlockGainedEntry, bool>)(e => e.HappenedThisTurn(this.CombatState) && e.Actor == this.Owner)) > 1;

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        bool mayHeal = !(gainedBlockTurn) && !(cardSource is null);
        if (!mayHeal)
            return;
        await CreatureCmd.Heal(Owner, amount);
    }
}