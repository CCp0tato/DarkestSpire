// | 热诚坚守 | ZealousStand | 技能 | 1 | 自身手中的所有攻击牌获得保留，每保留一张攻击牌获得 4/6 点格挡 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class ZealousStand : ModCardTemplate
{
    public ZealousStand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4, BlockProps.card)];

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var attacks = PileType.Hand.GetPile(Owner).Cards
            .Where(card => card.Type == CardType.Attack)
            .ToList();

        foreach (var attack in attacks)
        {
            CardCmd.ApplyKeyword(attack, CardKeyword.Retain);
        }

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block.BaseValue * attacks.Count,
            DynamicVars.Block.Props,
            cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
    }
}
