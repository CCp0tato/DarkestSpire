// | 恐惧 | Timoris | 状态 | 0 | 丢弃你的所有[手牌]然后抽等量的牌；丢弃状态牌时将其打出并[消耗]；[消耗]。 |

using DarkestSpire.TokenCards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(DSTokenCardPool))]
public class Timoris : ModCardTemplate
{
    public Timoris() : base(0, CardType.Status, CardRarity.Token, TargetType.None, true)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{DSTokenCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards = PileType.Hand.GetPile(Owner).Cards.ToList();
        foreach (CardModel card in cards)
        {
            if (card.Type != CardType.Status)
                await CardCmd.Discard(choiceContext, card);
            else
            {
                await CardCmd.AutoPlay(choiceContext, card, null);
                await CardCmd.Exhaust(choiceContext, card);
            }
        }
        await CardPileCmd.Draw(choiceContext, cards.Count, Owner);
    }
}