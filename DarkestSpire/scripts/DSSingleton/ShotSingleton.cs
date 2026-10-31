using DarkestSpire.Characters.HighwayMan.Cards;
using DarkestSpire.DarkestSpire.CardTags;
using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;

namespace DarkestSpire.DSSingleton;

[RegisterSingleton]
public class ShotSingleton : HookedSingletonModel
{
    public ShotSingleton() : base(HookType.Combat)
    {
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        if (!card.Tags.Contains(DSCardTag.Shot))
            return;
        if (!card.DynamicVars.TryGetValue("Shot", out DynamicVar? shotVar))
            return;
        int maxDropValue = shotVar.IntValue;
        
        if (maxDropValue == 0)
        {
            await ShotEvent(cardPlay, choiceContext, 0, 0);
            return;
        }
        if (maxDropValue > -1)
        {
            int discardCardCount = await ShotDiscard(choiceContext, card, maxDropValue);
                
            await ShotEvent(cardPlay, choiceContext, discardCardCount, maxDropValue);
        }
        else
        {
            int discardCardCount = await ShotDiscard(choiceContext, card, 999999999);
            int shotCountAddtives = (-1 - maxDropValue) + (2 * card.Owner.Relics.Count(c => c is ChemicalX));
            await ShotEvent(cardPlay, choiceContext, 0, 0, discardCardCount + shotCountAddtives);
        }
    }

    private async Task<int> ShotDiscard(PlayerChoiceContext choiceContext, CardModel cardSource, int maxDropValue)
    {
        List<CardModel> discardCards = (await CardSelectCmd.FromHandForDiscard(choiceContext, cardSource.Owner,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 0, maxDropValue), null,
            cardSource)).ToList();
        foreach (CardModel discardCard in discardCards)
        {
            if (cardSource is DoubleTap)
            {
                CardModel? selectedCard = discardCards.FirstOrDefault();
                if (selectedCard is not null)
                    await CardCmd.AutoPlay(choiceContext, selectedCard, null);
                continue;
            }
            
            await CardCmd.Discard(choiceContext, discardCard);
            
            if (discardCard is LetBulletFly)
            {
                await ((LetBulletFly) discardCard).AfterDiscardedByShot(discardCard);
            }
        }
        return discardCards.Count;
    }

    private async Task ShotEvent(CardPlay shotCardPlay, PlayerChoiceContext choiceContext, int discardCount, int shotCount, int shotTimes = 1)
    {
        CardModel shotCard = shotCardPlay.Card;

        PreAimPower? preAim = shotCard.Owner.Creature.GetPower<PreAimPower>();
        bool doubleShot = preAim?.ShouldDouble == true;
        bool appliedPreAim = false;

        if (shotCard.Owner.Creature.GetPower<SpareMagazinePower>() is { } spareMagazine)
            await CardPileCmd.Draw(choiceContext, spareMagazine.Amount,
                shotCard.Owner);
        
        int gapCount = shotCount - discardCount;
        if (gapCount > 0)
        {
            if (!shotCard.Owner.Creature.HasPower<GhostBulletPower>())
                return;
            await PowerCmd.Apply<DrawCardsNextTurnPower>(new ThrowingPlayerChoiceContext(), shotCard.Owner.Creature,
                0 - gapCount, null, null);
        }
        
        foreach (CardModel card in PileType.Discard.GetPile(shotCard.Owner).Cards.ToList())
        {
            if (card is DoubleBarrel)
                await CardPileCmd.Add(card, PileType.Hand);
        }
        
        List<DynamicVar> cardVars = shotCard.DynamicVars.Values
            .Where(c => c.Name.Contains("Shot"))
            .ToList();

        // 火力压制的枪击伤害应为“随机 1 名敌人”，且整张牌的多段伤害合并为一个 AttackCommand，
        // 这样 Vigor 等“下一次攻击”类效果会覆盖整张牌的所有段数，而不是第一段就消耗掉。
        bool isFireSuppression = shotCard is FireSuppression;
        int hitCount = Math.Max(shotTimes, 0);
        int iterations = isFireSuppression ? (hitCount > 0 ? 1 : 0) : shotTimes;

        for (int i = 0; i < iterations; i++)
        {
            foreach (DynamicVar dynamicVar in cardVars)
            {
                if (dynamicVar is DamageVar)
                {
                    int damageValue = dynamicVar.IntValue;
                    if (shotCard.Owner.Creature.GetPower<CaliberUpgradePower>() is { } caliberUpgrade)
                        damageValue += caliberUpgrade.Amount;
                    
                    bool hasAimShotPower = shotCard.Owner.Creature.HasPower<AimShotPower>();
                    if (hasAimShotPower)
                        damageValue *= 2;

                    if (doubleShot)
                    {
                        damageValue *= 2;
                        appliedPreAim = true;
                    }
                    
                    if (isFireSuppression)
                    {
                        await DamageCmd.Attack(damageValue).FromCard(shotCard)
                            .WithHitCount(hitCount)
                            .TargetingRandomOpponents(shotCard.Owner.Creature.CombatState!).Execute(choiceContext);
                        if (hasAimShotPower)
                            await PowerCmd.Decrement(shotCard.Owner.Creature.GetPower<AimShotPower>()!);
                    }
                    else if (shotCard.TargetType == TargetType.AllEnemies)
                    {
                        await DamageCmd.Attack(damageValue).FromCard(shotCard)
                            .TargetingAllOpponents(shotCard.Owner.Creature.CombatState!).Execute(choiceContext);
                        if (hasAimShotPower)
                            await PowerCmd.Decrement(shotCard.Owner.Creature.GetPower<AimShotPower>()!);
                    }
                    
                    else
                    {
                        // 单目标枪击牌正常会有选择目标；随机目标/自我目标类枪击牌的 Target 为 null，
                        // 这里兜底改为随机敌人，避免空引用中断结算。
                        if (shotCardPlay.Target is { } shotTarget)
                        {
                            await DamageCmd.Attack(damageValue).FromCard(shotCard)
                                .Targeting(shotTarget).Execute(choiceContext);
                        }
                        else
                        {
                            await DamageCmd.Attack(damageValue).FromCard(shotCard)
                                .TargetingRandomOpponents(shotCard.Owner.Creature.CombatState!).Execute(choiceContext);
                        }
                        if (hasAimShotPower)
                            await PowerCmd.Decrement(shotCard.Owner.Creature.GetPower<AimShotPower>()!);
                    }
                }
                else if (dynamicVar is EnergyVar)
                {
                    await PlayerCmd.GainEnergy(dynamicVar.IntValue, shotCard.Owner);
                }
                else if (dynamicVar is BlockVar)
                {
                    await CreatureCmd.GainBlock(shotCard.Owner.Creature, (BlockVar)dynamicVar, (CardPlay)null!);
                }
                else if (dynamicVar is GoldVar)
                {
                    await PlayerCmd.GainGold(dynamicVar.IntValue, shotCard.Owner);
                }
                else if (dynamicVar.Name == "ShotPower" && dynamicVar is IPowerVarFBBase powerVar)
                {
                    TargetType targetType = powerVar.TargetType;
                    if (powerVar.GetPowerInstance() is FightBackBasePower)
                    {
                        await ApplyShotPower(choiceContext, shotCard, shotCard.Owner.Creature, powerVar);
                        continue;
                    }
                    if (targetType == TargetType.AllEnemies)
                    {
                        foreach (Creature creature in shotCard.Owner.Creature.CombatState!.Enemies.ToList())
                        {
                            await ApplyShotPower(choiceContext, shotCard, creature, powerVar);
                        }
                        continue;
                    }
                    Creature? powerTarget = targetType == TargetType.Self
                        ? shotCard.Owner.Creature
                        : shotCardPlay.Target;
                    if (powerTarget is not null)
                        await ApplyShotPower(choiceContext, shotCard, powerTarget, powerVar);
                }
            }
            await AfterTriggerShot(shotCard);
        }

        if (appliedPreAim)
            preAim!._shotTurn++;
    }

    private static async Task ApplyShotPower(PlayerChoiceContext choiceContext, CardModel shotCard, Creature target, IPowerVarFBBase powerVar)
    {
        PowerModel powerInstance = powerVar.GetPowerInstance().ToMutable();
        if (powerInstance is FightBackBasePower)
        {
            FightBackBasePower fightBackPower = (FightBackBasePower) ModelDb.Power<FightBackBasePower>().ToMutable();
            fightBackPower.FightBackEffects = shotCard.DynamicVars.Values
                .Where(c => c.Name.Contains("FightBack"))
                .ToList();
            fightBackPower.FightBackCardSource = shotCard;
            fightBackPower.FightBackTargetType = powerVar.TargetType;
            target = shotCard.Owner.Creature;
            await PowerCmd.Apply(choiceContext, fightBackPower, target, powerVar.IntValue,
                shotCard.Owner.Creature, shotCard);
            return;
        }
        await PowerCmd.Apply(choiceContext, powerInstance, target, powerVar.IntValue,
            shotCard.Owner.Creature, shotCard);
    }

    private async Task AfterTriggerShot(CardModel shotCard)
    {
        if (shotCard is JackpotForHighwayMan)
        {
            await ((JackpotForHighwayMan)shotCard).OnShot();
        }
        if (shotCard is DeathBladeDance)
        {
            await ((DeathBladeDance)shotCard).OnShot();
        }
    }
}
