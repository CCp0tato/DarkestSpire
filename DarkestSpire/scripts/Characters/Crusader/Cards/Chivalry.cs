// | 骑士精神！ | Chivalry | 能力 | 2/1 | 获得一个随机的美德 |

using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class Chivalry : ModCardTemplate
{
    public Chivalry() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        PowerModel virtue = UniquenessControl.DSUniqueBuff
            .TakeRandom<PowerModel>(1, Owner.RunState.Rng.CombatPotionGeneration)
            .First();

        await PowerCmd.Apply(choiceContext, virtue, Owner.Creature, 1, Owner.Creature, cardPlay.Card);
    }

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
