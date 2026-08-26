using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class DeadeyePower : UniquePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner != Owner.Player)
            return;
        this.Flash();
        await DamageCmd.Attack(Amount).Unpowered().TargetingRandomOpponents(CombatState).Execute(choiceContext);
    }
}