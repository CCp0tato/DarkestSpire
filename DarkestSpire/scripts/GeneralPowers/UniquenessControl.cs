using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

public class UniquenessControl
{
    public IEnumerable<PowerModel> DSUniqueDebuff
    {
        get
        {
            return new PowerModel[10]
            {
                ModelDb.Power<FearPower>(),
                ModelDb.Power<ParanoiaPower>(),
                ModelDb.Power<SelfishPower>(),
                ModelDb.Power<MasochismPower>(),
                ModelDb.Power<TyrannyPower>(),
                ModelDb.Power<DespairPower>(),
                ModelDb.Power<FrenzyPower>(),
                ModelDb.Power<RefractionPower>(),
                ModelDb.Power<RapturePower>(),
                ModelDb.Power<SpitePower>()
            };
        }
    }
    
    public IEnumerable<PowerModel> DSUniqueBuff
    {
        get
        {
            return new PowerModel[5]
            {
                ModelDb.Power<ResolvePower>(),
                ModelDb.Power<FearlessnessPower>(),
                ModelDb.Power<AttentivenessPower>(),
                ModelDb.Power<ValorPower>(),
                ModelDb.Power<RallyPower>()
            };
        }
    }
    
    public static void ClearDSDebuffs(Creature player)
    {
        if (player.HasPower<FearPower>()) PowerCmd.Remove<FearPower>(player);
        if (player.HasPower<ParanoiaPower>()) PowerCmd.Remove<ParanoiaPower>(player);
        if (player.HasPower<SelfishPower>()) PowerCmd.Remove<SelfishPower>(player);
        if (player.HasPower<MasochismPower>()) PowerCmd.Remove<MasochismPower>(player);
        if (player.HasPower<TyrannyPower>()) PowerCmd.Remove<TyrannyPower>(player);
        if (player.HasPower<DespairPower>()) PowerCmd.Remove<DespairPower>(player);
        if (player.HasPower<FrenzyPower>()) PowerCmd.Remove<FrenzyPower>(player);
        if (player.HasPower<RefractionPower>()) PowerCmd.Remove<RefractionPower>(player);
        if (player.HasPower<RapturePower>()) PowerCmd.Remove<RapturePower>(player);
        if (player.HasPower<SpitePower>()) PowerCmd.Remove<SpitePower>(player);
    }

    public static void ClearDSBuffs(Creature player)
    {
        if (player.HasPower<ResolvePower>()) PowerCmd.Remove<ResolvePower>(player);
        if (player.HasPower<FearlessnessPower>()) PowerCmd.Remove<FearlessnessPower>(player);
        if (player.HasPower<AttentivenessPower>()) PowerCmd.Remove<AttentivenessPower>(player);
        if (player.HasPower<ValorPower>()) PowerCmd.Remove<ValorPower>(player);
        if (player.HasPower<RallyPower>()) PowerCmd.Remove<RallyPower>(player);
    }
}

public abstract class UniquePowerModel : ModPowerTemplate
{
    public virtual bool IsUniquePower => false;

    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (!IsUniquePower) return Task.CompletedTask;
        if (Type == PowerType.Buff)
        { 
            UniquenessControl.ClearDSBuffs(target);
            UniquenessControl.ClearDSDebuffs(target);
            return Task.CompletedTask;
        }
        if (Type == PowerType.Debuff)
        {
            UniquenessControl.ClearDSBuffs(target);
            UniquenessControl.ClearDSDebuffs(target);
        }
        return Task.CompletedTask;
    }
}