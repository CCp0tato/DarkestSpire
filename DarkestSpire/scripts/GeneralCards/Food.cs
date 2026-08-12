using DarkestSpire.Characters.Example;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(GeneralCardPool))]
public class Food : ModCardTemplate
{
    public Food() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self, true)
    {
    }

    public override bool GainsBlock => true;

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{GeneralCardPool.getImageRoot()}/{GetType().Name}.png"
        // 卡框等，有需求自己添加。需要自行判断卡牌类型（攻击、技能、能力等）设置，建议写在基类里。
        // 如果使用自定义卡池，需要改下material，看添加人物章节的添加卡池部分
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );

    // 卡牌基础数值
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(7, BlockProps.card),
        new HealVar(3)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.IntValue);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars.Heal.UpgradeValueBy(1);
    }
}