// | 神的旨意 | DivineWill | 技能 | 1/0 | 对随机目标打出手中所有的攻击牌并将其消耗，消耗 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class DivineWill : ModCardTemplate
{
    public DivineWill() : base(1, CardType.Skill, CardRarity.Rare, TargetType.RandomEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attacks = PileType.Hand.GetPile(Owner).Cards
            .Where(card => card.Type == CardType.Attack)
            .ToList();

        foreach (var attack in attacks)
        {
            await CardCmd.AutoPlay(choiceContext, attack, null);
            await CardCmd.Exhaust(choiceContext, attack);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}