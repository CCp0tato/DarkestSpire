using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class Robbery : ModCardTemplate
{
    public Robbery() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5, ValueProp.Move), new IntVar("StealBuff", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);
        IEnumerable<PowerModel> buffs = cardPlay.Target!.Powers.ToList().Where(p => p.Type == PowerType.Buff);
        IEnumerable<PowerModel> stolenBuffs = buffs.TakeRandom(DynamicVars["StealBuff"].IntValue, Owner.RunState.Rng.CombatPotionGeneration);
        foreach (PowerModel stolenBuff in stolenBuffs)
        {
            PowerModel givenBuff = ModelDb.GetById<PowerModel>(stolenBuff.Id).ToMutable();
            await PowerCmd.Apply(choiceContext, givenBuff, Owner.Creature, stolenBuff.Amount, Owner.Creature, null);
            await PowerCmd.Remove(stolenBuff);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StealBuff"].UpgradeValueBy(1);
    }
}