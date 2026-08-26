using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class Hustler : ModCardTemplate
{
    public Hustler() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(4)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> selection = (await CardSelectCmd.FromSimpleGrid(choiceContext, PileType.Draw.GetPile(this.Owner).Cards.Take(DynamicVars.Cards.IntValue).ToList(), this.Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, this.DynamicVars.Cards.IntValue))).ToList<CardModel>();

    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2);
    }
}