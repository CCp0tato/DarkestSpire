using DarkestSpire.Characters.HighwayMan.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DarkestSpire.GeneralPowers;

public interface IPowerVarFBBase
{
    public abstract TargetType TargetType { get; }
    public abstract PowerModel GetPowerInstance();

    public abstract int IntValue { get; }
}


public class PowerVarFB<T> : PowerVar<T>, IPowerVarFBBase where T : PowerModel
{
    public PowerVarFB(Decimal powerAmount, TargetType targetType)
        : base("FightBackPower", powerAmount)
    {
        this.TargetType = targetType;
    }

    public PowerVarFB(string name, Decimal powerAmount, TargetType targetType)
        : base(name, powerAmount)
    {
        this.TargetType = targetType;
    }
    
    public TargetType TargetType { get; }

    public PowerModel GetPowerInstance()
    {
        return ModelDb.Power<T>();
    }
}



[RegisterPower]
public class FightBackBasePower : ModPowerTemplate
{
    public FightBackBasePower()
    {
    }
    
    public FightBackBasePower(IEnumerable<DynamicVar> fightBackEffects, CardModel fightBackCardSource)
    {
        this._fightBackCardSource = fightBackCardSource;
        this._fightBackEffects = fightBackEffects.ToList();
        this._fightBackTargetType = fightBackCardSource.TargetType;
    }
    
    public FightBackBasePower(CardModel card)
    {
        this._fightBackCardSource = card;
        this._fightBackEffects = card.DynamicVars.Values.Where(c => c.Name.Contains("FightBack")).ToList();
        this._fightBackTargetType = card.TargetType;
    }

    public FightBackBasePower(CardPlay cardPlay) : this(cardPlay.Card)
    {
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private List<DynamicVar> _fightBackEffects { get; set; } = [];
    private CardModel _fightBackCardSource { get; set; } = null!;
    private TargetType? _fightBackTargetType { get; set; }

    public IEnumerable<DynamicVar> FightBackEffects
    {
        get => _fightBackEffects;
        set => _fightBackEffects = value.ToList();
    }

    public CardModel FightBackCardSource
    {
        get => _fightBackCardSource;
        set => _fightBackCardSource = value;
    }

    public TargetType FightBackTargetType
    {
        get => _fightBackTargetType ?? _fightBackCardSource.TargetType;
        set => _fightBackTargetType = value;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer is null || !props.IsPoweredAttack())
            return;
        foreach (DynamicVar dynamicVar in _fightBackEffects.ToList())
        {
            if (dynamicVar is DamageVar)
            {
                if (FightBackTargetType == TargetType.AllEnemies)
                    await DamageCmd.Attack(dynamicVar.IntValue).FromCard(_fightBackCardSource)
                        .TargetingAllOpponents(Owner.CombatState!).Execute(choiceContext);
                else if (FightBackTargetType == TargetType.AnyEnemy)
                    await DamageCmd.Attack(dynamicVar.IntValue).FromCard(_fightBackCardSource)
                        .Targeting(dealer).Execute(choiceContext);
            }
            else if (dynamicVar is EnergyVar)
            {
                await PlayerCmd.GainEnergy(dynamicVar.IntValue, Owner.Player!);
            }
            else if (dynamicVar is BlockVar)
            {
                await CreatureCmd.GainBlock(Owner, (BlockVar) dynamicVar, (CardPlay) null!);
            }
            else if (dynamicVar is GoldVar)
            {
                await PlayerCmd.GainGold(dynamicVar.IntValue, Owner.Player!);
            }
            else if (dynamicVar.Name == "FightBackPower" && dynamicVar is IPowerVarFBBase powerVar)
            {
                TargetType targetType = powerVar.TargetType;
                if (targetType == TargetType.AllEnemies)
                {
                    foreach (Creature creature in CombatState.Enemies.ToList())
                    {
                        await PowerCmd.Apply(choiceContext, powerVar.GetPowerInstance().ToMutable(), creature,
                            powerVar.IntValue, Owner, _fightBackCardSource);
                    }
                    continue;
                }
                Creature powerTarget = targetType == TargetType.Self ? Owner : dealer;
                await PowerCmd.Apply(choiceContext, powerVar.GetPowerInstance().ToMutable(), powerTarget,
                    powerVar.IntValue, Owner, _fightBackCardSource);
                
            }
        }
        if (Owner.HasPower<PocketGunPower>() && Owner.Player is not null)
            await QuickShot.CreateInHand(Owner.Player, 1, CombatState);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }

    public DynamicVar AddDynamicVar(DynamicVar dynamicVar)
    {
        _fightBackEffects.Add(dynamicVar);
        return dynamicVar;
    }
}
