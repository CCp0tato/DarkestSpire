using DarkestSpire.Characters.Example;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(GeneralCardPool))]
public class SkillRefinement : ModCardTemplate
{
    public SkillRefinement() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }
    
    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{GeneralCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );

    // 卡牌基础数值
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9M, ValueProp.Move)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
        if (this.IsUpgraded)
        {
            foreach (CardModel card in PileType.Hand.GetPile(this.Owner).Cards.Where<CardModel>((Func<CardModel, bool>) (c => c.IsUpgradable)))
                CardCmd.Upgrade(card);
        }
        else
        {
            CardModel card = await CardSelectCmd.FromHandForUpgrade(choiceContext, this.Owner, (AbstractModel) this);
            if (card == null)
                return;
            CardCmd.Upgrade(card);
        }
    }
}