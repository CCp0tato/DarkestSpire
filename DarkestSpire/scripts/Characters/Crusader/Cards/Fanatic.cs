// | 狂信徒 | Fanatic | 能力 | 1 | 手牌中只有攻击牌时，你造成的伤害 +25%/50% |

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using DarkestSpire.GeneralPowers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class Fanatic : ModCardTemplate
{
    public Fanatic() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FanaticPower>(25)];

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FanaticPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["FanaticPower"].IntValue,
            Owner.Creature,
            cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FanaticPower"].UpgradeValueBy(25);
    }
}
