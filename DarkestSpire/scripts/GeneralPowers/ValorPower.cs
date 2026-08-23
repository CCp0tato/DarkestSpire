using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class ValorPower : UniquePowerModel
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
        foreach (Creature enemy in CombatState.Enemies)
        {
            if (enemy.Monster!.IntendsToAttack)
            {
                PowerCmd.Apply<DexterityPower>(choiceContext, Owner, 1, Owner, (CardModel) null!);
                PowerCmd.Apply<StrengthPower>(choiceContext, Owner, 1, Owner, (CardModel) null!);
            }
        }
        return Task.CompletedTask;
    }
}