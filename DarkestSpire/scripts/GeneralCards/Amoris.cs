// | 爱 | Amoris | 状态 | 2 | 打出时[消耗]所有[手牌]；每[消耗] 1 张造成 10 点伤害；[消耗]。 |

using DarkestSpire.TokenCards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(DSTokenCardPool))]
public class Amoris : ModCardTemplate
{
    public Amoris() : base(2, CardType.Status, CardRarity.Token, TargetType.AnyEnemy, true)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{DSTokenCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10, ValueProp.Move)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> cards = PileType.Hand.GetPile(Owner).Cards.ToList();
        foreach (CardModel card in cards)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(cards.Count)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }
}
