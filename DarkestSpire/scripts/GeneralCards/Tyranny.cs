using DarkestSpire.DarkestSpire.CardTags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using TyrannyPower = DarkestSpire.GeneralPowers.TyrannyPower;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(GeneralCardPool))]
public class Tyranny : ModCardTemplate
{
    public Tyranny() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }
    
    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{GeneralCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );
    protected override HashSet<CardTag> CanonicalTags => 
    [
        DSCardTag.Torture,
        DSCardTag.Unique,
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TyrannyPower>(choiceContext, Owner.Creature,
            1, Owner.Creature, cardPlay.Card);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}