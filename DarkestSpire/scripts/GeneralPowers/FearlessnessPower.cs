// 无畏：抽到状态牌时，抽 3 张牌。检测消耗堆，出现5张Mujica状态牌后向手牌中加入AveMujica，一场战斗一次

using DarkestSpire.Characters.Crusader.Cards;
using DarkestSpire.GeneralCards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Models;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class FearlessnessPower : UniquePowerModel
{
    private bool _hasCreatedAveMujica;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsUniquePower => true;
    
    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
    
    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner || card.Type != CardType.Status)
            return;
        Flash();
        await CardPileCmd.Draw(choiceContext, (Decimal) 3, Owner.Player!);
    }

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal)
    {
        if (_hasCreatedAveMujica || card.Owner.Creature != Owner)
            return;

        IReadOnlyList<CardModel> exhaustPile = PileType.Exhaust.GetPile(Owner.Player!).Cards;
        if (!exhaustPile.Any(exhaustedCard => exhaustedCard is Amoris) ||
            !exhaustPile.Any(exhaustedCard => exhaustedCard is Mortis) ||
            !exhaustPile.Any(exhaustedCard => exhaustedCard is Timoris) ||
            !exhaustPile.Any(exhaustedCard => exhaustedCard is Doloris) ||
            !exhaustPile.Any(exhaustedCard => exhaustedCard is Oblivionis))
            return;

        _hasCreatedAveMujica = true;
        Flash();
        CardModel aveMujica = CombatState.CreateCard<AveMujica>(Owner.Player!);
        await CardPileCmd.AddGeneratedCardToCombat(aveMujica, PileType.Hand, Owner.Player);
    }
}
