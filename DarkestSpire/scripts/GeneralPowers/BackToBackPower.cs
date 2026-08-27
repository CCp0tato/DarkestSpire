using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class BackToBackPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
    
    private bool _hasAlreadyBeenGivenBlock;
    private bool HasAlreadyBeenGivenBlock
    {
        get => this._hasAlreadyBeenGivenBlock;
        set
        {
            this.AssertMutable();
            this._hasAlreadyBeenGivenBlock = value;
        }
    }

    public override async Task AfterBlockGained(
        Creature creature,
        Decimal amount,
        ValueProp props,
        CardModel? cardSource)
    {
        if (amount < 1M || creature != this.Owner || this.CombatState.CurrentSide != this.Owner.Side || this.HasAlreadyBeenGivenBlock || !creature.HasPower<BackToBackPower>())
            return;
        Decimal amountToGive = amount;
        IEnumerable<Creature> creatures = this.CombatState.GetTeammatesOf(this.Owner).Where<Creature>((c => c != null && c.IsAlive && c.IsPlayer && c.Player.Creature != this.Owner && c.HasPower<BackToBackPower>()));
        this.HasAlreadyBeenGivenBlock = true;
        foreach (Creature creature1 in creatures)
        {
            Decimal num = await CreatureCmd.GainBlock(creature1, amountToGive, ValueProp.Unpowered, (CardPlay) null);
        }
        this.HasAlreadyBeenGivenBlock = false;
    }
}