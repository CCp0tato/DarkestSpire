using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class OutlawLogic : ModCardTemplate
{
    public OutlawLogic() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(this.SelectionScreenPrompt, 0, 999999999);
        foreach (CardModel original in (await CardSelectCmd.FromHand(choiceContext, this.Owner, prefs, (Func<CardModel, bool>) null, (AbstractModel) this)).ToList<CardModel>())
        {
            CardModel card = (CardModel) this.CombatState.CreateCard<LogicalBullet>(this.Owner);
            if (this.IsUpgraded)
                CardCmd.Upgrade(card);
            CardPileAddResult? nullable = await CardCmd.Transform(original, card);
        }
    }
}