using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class FuckTheAssAssIn : ModCardTemplate
{
    public FuckTheAssAssIn() : base(2, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ThornsPower>(10)];

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Target!.IsMonster || cardPlay.Target is null)
        {
            return;
        }
        if (!cardPlay.Target.Monster!.IntendsToAttack)
            await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, DynamicVars["ThornsPower"].IntValue, Owner.Creature, this);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override void OnUpgrade()
    {
        DynamicVars["ThornsPower"].UpgradeValueBy(2);
    }
}