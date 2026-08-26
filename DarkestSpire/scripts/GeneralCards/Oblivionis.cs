// | 遗忘 | Oblivionis | 状态 | 0 | 抽 1 张牌；下回合抽 1 张牌；获得 1 点[能量]；下回合获得 1 点[能量]；[消耗]。 |

using DarkestSpire.TokenCards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(DSTokenCardPool))]
public class Oblivionis : ModCardTemplate
{
    public Oblivionis() : base(0, CardType.Status, CardRarity.Token, TargetType.None, true)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{DSTokenCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new EnergyVar(1)];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner.Creature,
            DynamicVars.Cards.IntValue, Owner.Creature, cardPlay.Card);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature,
            DynamicVars.Energy.IntValue, Owner.Creature, cardPlay.Card);
    }
}
