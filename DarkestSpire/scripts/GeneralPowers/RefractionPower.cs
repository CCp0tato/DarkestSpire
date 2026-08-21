using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class RefractionPower : UniquePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsUniquePower => true;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
    
    private IEnumerable<CardModel> availableStatusCards
    {
        get
        {
            return new CardModel[11]
            {
                ModelDb.Card<Beckon>(),
                ModelDb.Card<Burn>(),
                ModelDb.Card<Dazed>(),
                ModelDb.Card<Debris>(),
                ModelDb.Card<Infection>(),
                ModelDb.Card<Wither>(),
                ModelDb.Card<Slimed>(),
                ModelDb.Card<Soot>(),
                ModelDb.Card<Toxic>(),
                ModelDb.Card<Void>(),
                ModelDb.Card<Wound>()
            };
        }
    } 

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        await CardPileCmd.AutoPlayFromDrawPile(choiceContext, Owner.Player!, 2, CardPilePosition.Top, false);
        for (int i = 0; i < 2; i++)
        {
            CardModel randomStatusCard = availableStatusCards
                .TakeRandom(1, Owner.Player!.RunState.Rng.CombatCardGeneration).FirstOrDefault()!;
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
                CombatState.CreateCard(randomStatusCard, this.Owner.Player), PileType.Discard, Owner.Player));
        }
    }
}