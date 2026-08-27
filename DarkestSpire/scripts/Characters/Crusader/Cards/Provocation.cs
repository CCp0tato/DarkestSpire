// | 嘲讽 | Provocation | 技能 | 1 | 将 2/3 张小零食加入你的手牌，消耗 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class Provocation : ModCardTemplate
{
    public Provocation() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self, true)
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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("GainCardNum", 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.AddToCombatAndPreview<Snack>(Owner.Creature, PileType.Draw, DynamicVars["GainCardNum"].IntValue, Owner, CardPilePosition.Random);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["GainCardNum"].UpgradeValueBy(1);
    }
}