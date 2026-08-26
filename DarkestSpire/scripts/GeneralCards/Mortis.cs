// | 死亡 | Mortis | 状态 | 1 | 你受到致死伤害时免疫该次伤害并获得等量的[灾厄]；[消耗]。 |

using DarkestSpire.TokenCards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DarkestSpire.GeneralPowers;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(DSTokenCardPool))]
public class Mortis : ModCardTemplate
{
    public Mortis() : base(1, CardType.Status, CardRarity.Token, TargetType.None, true)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{DSTokenCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<MortisPower>(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MortisPower>(choiceContext, Owner.Creature, DynamicVars["MortisPower"].IntValue, Owner.Creature, this);
    }
}