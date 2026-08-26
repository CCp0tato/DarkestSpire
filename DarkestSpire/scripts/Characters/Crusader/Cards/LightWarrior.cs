// | 光明战士 | LightWarrior | 能力 | 1 | 回合开始时，若自身生命值大于 60%/40%，抽两张牌 |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using DarkestSpire.GeneralPowers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class LightWarrior : ModCardTemplate
{
    public LightWarrior() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<LightWarriorPower>(60)];

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LightWarriorPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["LightWarriorPower"].IntValue,
            Owner.Creature,
            cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["LightWarriorPower"].UpgradeValueBy(-20);
    }
}