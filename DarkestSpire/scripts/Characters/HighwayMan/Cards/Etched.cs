using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class Etched : ModCardTemplate
{
    public Etched() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.None, true)
    {
    }

    public override int MaxUpgradeLevel => 0;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains<Creature>(this.Owner.Creature) || this.Owner.PlayerCombatState.TurnNumber > 1)
            return;
        foreach (CardModel card in Owner.Deck.Cards.ToList().Where(c => c is Etched))
        {
            PowerModel buff = UniquenessControl.DSUniqueBuff.TakeRandom(1, Owner.RunState.Rng.CombatPotionGeneration)
                .FirstOrDefault().ToMutable();
            await PowerCmd.Apply(choiceContext, buff, Owner.Creature, 1, Owner.Creature, this);
        }
    }
}