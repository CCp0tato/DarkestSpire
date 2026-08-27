using DarkestSpire.Characters.Crusader.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class YourSunUpgradePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        Flash();
        CardPileAddResult[] addedCards = new CardPileAddResult[Amount];
        for (int i = 0; i < Amount; i++)
        {
            CardModel dampSickle = CombatState.CreateCard<DampSickle>(player);
            CardCmd.Upgrade(dampSickle);
            addedCards[i] = await CardPileCmd.AddGeneratedCardToCombat(
                dampSickle,
                PileType.Draw,
                player,
                CardPilePosition.Random);
        }

        CardCmd.PreviewCardPileAdd(addedCards);
    }

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{nameof(YourSunPower)}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{nameof(YourSunPower)}.png"
    );
}
