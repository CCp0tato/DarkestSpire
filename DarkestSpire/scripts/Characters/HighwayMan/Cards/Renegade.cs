using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class Renegade : ModCardTemplate
{
    public Renegade() : base(-1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RetainHandPower>(choiceContext, Owner.Creature, 1, Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, ResolveEnergyXValue() + (IsUpgraded ? 1 : 0), Owner.Creature, cardPlay.Card);
        PlayerCmd.EndTurn(Owner, false);
    }

    protected override void OnUpgrade()
    {
        
    }
}