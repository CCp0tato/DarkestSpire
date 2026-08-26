// | 审判 | Judgment | 技能 | 1 | 若目标生命值不高于 30/40%，则直接将其击杀 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class Judgment : ModCardTemplate
{
    public Judgment() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Judgement", 30)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is null) { return; }
        if (cardPlay.Target.CurrentHp * 100 <= cardPlay.Target.MaxHp * DynamicVars["Judgment"].BaseValue)
        {
            await CreatureCmd.Kill(cardPlay.Target, false);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Judgment"].UpgradeValueBy(10);
    }
}