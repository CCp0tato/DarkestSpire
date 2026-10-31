using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class TyrannyPower : UniquePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsUniquePower => true;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    private bool _energyDrawUsedThisTurn;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
            _energyDrawUsedThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task BeforeAttack(AttackCommand command)
    {
        if (command.IsMultiTargeted || command.Attacker != Owner)
        {
            return;
        }

        Type targetType = typeof(AttackCommand);
        FieldInfo? field = targetType.GetField(
            "_damagePerHit",
            BindingFlags.NonPublic | 
            BindingFlags.Instance | 
            BindingFlags.FlattenHierarchy 
        );
        if (field == null)
        {
            return;
        }
        
        // 只从“队友”中随机，排除自己（Allies 在部分实现里包含自身）。
        List<Creature> allies = Owner.CombatState!.Allies
            .Where(c => c != Owner && c.IsPlayer && c.IsAlive)
            .ToList();
        if (allies.Count == 0)
            return;
        Creature? creature = allies.TakeRandom(1, Owner.Player!.RunState.Rng.CombatTargets).FirstOrDefault();
        if (creature == null)
            return;
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), creature, new DamageVar((Decimal) field.GetValue(command)!, ValueProp.Move), Owner);

        // 获得能量 + 抽牌每回合仅触发一次；伤害仍按每次单体攻击触发。
        if (_energyDrawUsedThisTurn)
            return;
        _energyDrawUsedThisTurn = true;
        await PlayerCmd.GainEnergy(2, Owner.Player);
        await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), 2, Owner.Player);
    }
}