using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DarkestSpire.GeneralPowers;

public class PowerVarFB<T> : PowerVar<T> where T : PowerModel
{
    public PowerVarFB(Decimal powerAmount, TargetType targetType)
        : base(typeof (T).Name, powerAmount)
    {
        this.targetType = targetType;
    }

    public PowerVarFB(string name, Decimal powerAmount)
        : base(name, powerAmount)
    {
    }

    public T powerType => Activator.CreateInstance<T>();

    public TargetType targetType { get; }
}



public class FightBackBasePower : ModPowerTemplate
{
    public FightBackBasePower(IEnumerable<DynamicVar> fightBackEffects, CardModel fightBackCardSource)
    {
        this._fightBackCardSource = fightBackCardSource;
        this._fightBackEffects = fightBackEffects;
    }
    
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private IEnumerable<DynamicVar> _fightBackEffects { get; }
    private CardModel _fightBackCardSource { get; }
    
    public IEnumerable<DynamicVar> FightBackEffects => _fightBackEffects;
    
    public CardModel FightBackCardSource => _fightBackCardSource;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        foreach (DynamicVar dynamicVar in _fightBackEffects)
        {
            if (dynamicVar is DamageVar)
            {
                if (_fightBackCardSource.TargetType == TargetType.AllEnemies)
                    DamageCmd.Attack(dynamicVar.IntValue).FromCard(_fightBackCardSource)
                        .TargetingAllOpponents(Owner.CombatState!);
                else
                    DamageCmd.Attack(dynamicVar.IntValue).FromCard(_fightBackCardSource)
                        .Targeting(dealer!);
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
            else if (dynamicVar is PowerVarFB<PowerModel> powerVar)
            {
                Creature powerTargets = (powerVar.targetType == TargetType.Self) ? Owner : dealer! ; 
                await PowerCmd.Apply(choiceContext, powerVar.powerType, powerTargets, powerVar.IntValue, Owner, (CardModel) null!);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
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