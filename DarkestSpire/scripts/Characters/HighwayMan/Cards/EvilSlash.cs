using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class EvilSlash : ModCardTemplate
{
    public EvilSlash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    private int _cardPlayedTurn;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new DamageVar("ExtraDamage", 5, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + (ShouldGlowGoldInternal ? DynamicVars["ExtraDamage"].IntValue : 0M)).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner)
            ++_cardPlayedTurn;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        _cardPlayedTurn = 0;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["ExtraDamage"].UpgradeValueBy(3);
    }

    protected override bool ShouldGlowGoldInternal => _cardPlayedTurn < 4;
}