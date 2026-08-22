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

public abstract class FightBackBasePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public virtual required IEnumerable<DynamicVar> FightBackEffects { get; set; }

    public virtual required CardModel fightBackCardSource { get; set; }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        foreach (DynamicVar dynamicVar in FightBackEffects)
        {
            if (dynamicVar is DamageVar)
            {
                if (fightBackCardSource.TargetType == TargetType.AllEnemies)
                    DamageCmd.Attack(dynamicVar.IntValue).FromCard(fightBackCardSource)
                        .TargetingAllOpponents(Owner.CombatState!);
                else
                    DamageCmd.Attack(dynamicVar.IntValue).FromCard(fightBackCardSource)
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
            else if (dynamicVar is PowerVar<PowerModel> powerVar)
            {
                
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
        FightBackEffects.AddItem(dynamicVar);
        return dynamicVar;
    }
}