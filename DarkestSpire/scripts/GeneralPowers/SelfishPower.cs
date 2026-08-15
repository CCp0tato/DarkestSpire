using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DarkestSpire.Afflictions;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class SelfishPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        foreach (Creature creature in (IEnumerable<Creature>)CombatState.PlayerCreatures.Where(_ => true)
                     .ToList<Creature>())
        {
            await PlayerCmd.LoseGold(4, creature.Player);
            await PlayerCmd.GainGold(5, Owner.Player);
        }
        await PlayerCmd.LoseGold(1, Owner.Player);
    }
    
}