using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Random;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class FrenzyPower : UniquePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsUniquePower => true;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override Task BeforeAttack(AttackCommand command)
    {
        if (command.IsRandomlyTargeted || command.IsMultiTargeted || command.Attacker != Owner)
        {
            return Task.CompletedTask;
        }
        Type targetType = typeof(AttackCommand);
        FieldInfo? field = targetType.GetField(
            "_singleTarget",
            BindingFlags.NonPublic | 
            BindingFlags.Instance | 
            BindingFlags.FlattenHierarchy 
        );
        if (field == null)
        {
            return Task.CompletedTask;
        }
        field.SetValue(command, null);
        command.TargetingRandomOpponents(Owner.CombatState);
        return Task.CompletedTask;
    }
}