using DarkestSpire.DarkestSpire.CardTags;
using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class DirtyDiseaseThatAreAllCheap : ModCardTemplate
{
    public DirtyDiseaseThatAreAllCheap() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies, true)
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVarFB<EnergyNextTurnPower>(1, TargetType.Self)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override HashSet<CardTag> CanonicalTags => [DSCardTag.FightBack, DSCardTag.FightBackSkip];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        FightBackBasePower fbp = ModelDb.Power<FightBackBasePower>();
        fbp = (FightBackBasePower)fbp.ToMutable();
        fbp.FightBackCardSource = cardPlay.Card;
        fbp.FightBackEffects = CanonicalVars;

        foreach (Creature playerCreature in CombatState.PlayerCreatures)
        {
            await PowerCmd.Apply(choiceContext, fbp, playerCreature, 1, Owner.Creature, cardPlay.Card);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}