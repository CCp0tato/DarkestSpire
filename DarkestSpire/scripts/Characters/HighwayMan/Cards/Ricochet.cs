using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class Ricochet : ModCardTemplate
{
    public Ricochet() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Sly, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4, ValueProp.Move), new CardsVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);
    }

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card is Ricochet)
        {
            IEnumerable<CardModel> newCards = [];
            for (int i =0; i<DynamicVars.Cards.IntValue; i++)
                newCards = newCards.Append(card.CreateClone());
            await CardPileCmd.Add(newCards, PileType.Draw);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}