using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using TyrannyPower = DarkestSpire.GeneralPowers.TyrannyPower;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(GeneralCardPool))]
public class Despair : ModCardTemplate
{
    public Despair() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }
    
    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{GeneralCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("NextTurnEnergyAndDraw", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<IntangiblePower>(choiceContext, Owner.Creature,
            1, Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature,
            DynamicVars["NextTurnEnergyAndDraw"].IntValue, Owner.Creature,  cardPlay.Card);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner.Creature,
            DynamicVars["NextTurnEnergyAndDraw"].IntValue, Owner.Creature,  cardPlay.Card);
        await PowerCmd.Apply<DespairPower>(choiceContext, Owner.Creature,
            1, Owner.Creature, cardPlay.Card);
        
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        DynamicVars["NextTurnEnergyAndDraw"].UpgradeValueBy(1);
    }
}