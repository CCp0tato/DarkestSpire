using DarkestSpire.Characters.HighwayMan.Cards;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

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



public class FightBackBasePower : ModPowerTemplate
{
    public FightBackBasePower()
    {
    }
    
    public FightBackBasePower(IEnumerable<DynamicVar> fightBackEffects, CardModel fightBackCardSource)
    {
        this._fightBackCardSource = fightBackCardSource;
        this._fightBackEffects = fightBackEffects;
    }
    
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private IEnumerable<DynamicVar> _fightBackEffects { get; set; }
    private CardModel _fightBackCardSource { get; set; }

    public IEnumerable<DynamicVar> FightBackEffects
    {
        get => _fightBackEffects;
        set => _fightBackEffects = value;
    }

    public CardModel FightBackCardSource
    {
        get => _fightBackCardSource;
        set => _fightBackCardSource = value;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
            return;
        foreach (DynamicVar dynamicVar in _fightBackEffects)
        {
            if (dynamicVar is DamageVar)
            {
                if (_fightBackCardSource.TargetType == TargetType.AllEnemies)
                    await DamageCmd.Attack(dynamicVar.IntValue).FromCard(_fightBackCardSource)
                        .TargetingAllOpponents(Owner.CombatState!).Execute(choiceContext);
                else if (_fightBackCardSource.TargetType == TargetType.AnyEnemy)
                    await DamageCmd.Attack(dynamicVar.IntValue).FromCard(_fightBackCardSource)
                        .Targeting(dealer!).Execute(choiceContext);
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
            else if (dynamicVar.Name == "FightBackPower")
            {
                IPowerVarFBBase powerVar = (IPowerVarFBBase)dynamicVar;
                TargetType targetType = powerVar.TargetType;
                PowerModel powerInstance = powerVar.GetPowerInstance().ToMutable();
                
                if (targetType == TargetType.AllEnemies)
                {
                    foreach (Creature creature in CombatState.Enemies)
                    {
                        await PowerCmd.Apply(choiceContext, powerInstance, creature, powerVar.IntValue, Owner, _fightBackCardSource);
                    }
                    return;
                }
                Creature powerTargets = (targetType == TargetType.Self) ? Owner : dealer! ; 
                await PowerCmd.Apply(choiceContext, powerInstance, powerTargets, powerVar.IntValue, Owner, _fightBackCardSource);
                
            }
            if (Owner.HasPower<PocketGunPower>())
                await QuickShot.CreateInHand(Owner.Player, 1, CombatState);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
        {
            await PowerCmd.Remove(this);
        }
    }

    public DynamicVar AddDynamicVar(DynamicVar dynamicVar)
    {
        _fightBackEffects.AddItem(dynamicVar);
        return dynamicVar;
    }
}