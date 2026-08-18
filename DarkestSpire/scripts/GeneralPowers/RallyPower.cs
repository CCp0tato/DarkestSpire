using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class RallyPower : UniquePowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsUniquePower => true;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        int playerCounts = Owner.CombatState.Allies.Count;
        int blockPerPlayer = 24 / playerCounts;
        foreach (Creature ally in Owner.CombatState.Allies)
        {
            CreatureCmd.GainBlock(ally, new BlockVar(blockPerPlayer, BlockProps.nonCardUnpowered), (CardPlay) null);
        }
        return Task.CompletedTask;
    }
}