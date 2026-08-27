// | 无畏 | Fearlessness | 能力 | 2/1 | [固有]；将 5 张特定状态牌加入你的[抽牌堆]；获得[无畏]。 |

using DarkestSpire.DarkestSpire.CardTags;
using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(GeneralCardPool))]
public class Fearlessness : ModCardTemplate
{
    public Fearlessness() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }
    
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{GeneralCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];
    
    protected override HashSet<CardTag> CanonicalTags => 
    [
        DSCardTag.Virtue,
        DSCardTag.Unique,
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FearlessnessPower>(choiceContext, Owner.Creature, 1, Owner.Creature, cardPlay.Card);
        CardPileAddResult[] addedCards = new CardPileAddResult[5];
        CardModel[] cards = new CardModel[]
        {
            CombatState!.CreateCard<Amoris>(Owner),
            CombatState!.CreateCard<Timoris>(Owner),
            CombatState!.CreateCard<Oblivionis>(Owner),
            CombatState!.CreateCard<Mortis>(Owner),
            CombatState!.CreateCard<Doloris>(Owner)
        };
        for (int i = 0; i < 5; i++)
        {
            addedCards[i] = await CardPileCmd.AddGeneratedCardToCombat(
                cards[i],
                PileType.Draw,
                Owner,
                CardPilePosition.Random);
        }
        CardCmd.PreviewCardPileAdd(addedCards);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}