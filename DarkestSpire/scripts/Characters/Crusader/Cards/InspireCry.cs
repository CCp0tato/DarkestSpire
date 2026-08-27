// | 激励呐喊 | InspireCry | 能力 | 2 | 所有玩家获得4/5层信仰，你的信仰不会在回合开始时下降 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DarkestSpire.GeneralPowers;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class InspireCry : ModCardTemplate
{
    public InspireCry() : base(2, CardType.Power, CardRarity.Ancient, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FaithPower>(4)];

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FaithPower>(choiceContext, Owner.Creature, DynamicVars["FaithPower"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<InspireCryPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FaithPower"].UpgradeValueBy(1);
    }
}