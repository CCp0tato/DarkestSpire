// | Ave Mujica | 能力 | 0 | 回合结束时对所有敌人造成 60/70 点伤害。（通过无畏buff检测消耗堆，出现上面5张状态牌后塞入，一场战斗一次） |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DarkestSpire.GeneralPowers;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class AveMujica : ModCardTemplate
{
    public AveMujica() : base(0, CardType.Power, CardRarity.Common, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<AveMujicaPower>(60)];

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AveMujicaPower>(choiceContext, Owner.Creature, DynamicVars["AveMujicaPower"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AveMujicaPower"].UpgradeValueBy(10);
    }
}