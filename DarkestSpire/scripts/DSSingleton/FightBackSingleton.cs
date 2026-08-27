using DarkestSpire.DarkestSpire.CardTags;
using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;

namespace DarkestSpire.DSSingleton;

[RegisterSingleton]
public class FightBackSingleton : HookedSingletonModel
{
    public FightBackSingleton() : base(HookType.Combat)
    {
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;
        if (card is null)
            return;
        if (!card.Tags.Contains(DSCardTag.FightBack) || card.Tags.Contains(DSCardTag.FightBackSkip))
            return;
        IEnumerable<DynamicVar> cardVars = card.DynamicVars.Values.Where((c) => c.Name.Contains("FightBack"));
        FightBackBasePower fbp = ModelDb.Power<FightBackBasePower>();
        fbp = (FightBackBasePower) fbp.ToMutable();
        fbp.FightBackCardSource = card;
        fbp.FightBackEffects = cardVars;
        
        await PowerCmd.Apply(choiceContext, fbp, card.Owner.Creature, 1,
            card.Owner.Creature, card);
    }
}