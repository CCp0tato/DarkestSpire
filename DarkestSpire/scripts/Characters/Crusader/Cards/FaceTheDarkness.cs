// | 直面黑暗 | FaceTheDarkness | 技能 | 1 | 多人模式，所有玩家生命值上限增加 2/3 点，消耗 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class FaceTheDarkness : ModCardTemplate
{
    public FaceTheDarkness() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies, true)
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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("MaxHpAddition", 2)];

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<Creature> players = CombatState!.PlayerCreatures;

        foreach (Creature player in players)
        {
            await CreatureCmd.GainMaxHp(player, DynamicVars["MaxHpAddition"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MaxHpAddition"].UpgradeValueBy(1);
    }
}
