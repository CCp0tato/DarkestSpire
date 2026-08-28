using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class Flashbang : ModCardTemplate
{
    public Flashbang() : base(3, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Sly];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TempStrengthPower>(-7)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TempStrengthPower>(choiceContext, CombatState.Enemies, DynamicVars["TempStrengthPower"].IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["TempStrengthPower"].UpgradeValueBy(-2);
    }
}