// | 荣誉共鸣 | HonorResonance | 技能 | 1 | 多人模式，复活一名死去的队友并使其回复 25% 的生命值，保留，消耗 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class HonorResonance : ModCardTemplate
{
    public HonorResonance() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None, true)
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // The base game's player-targeting code rejects dead creatures, so this card
    // resolves its dead teammate without using AnyPlayer/AnyAlly targeting.
    protected override bool IsPlayable => GetDeadTeammate() is not null;

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature? teammate = GetDeadTeammate();
        if (teammate is null)
        {
            return;
        }

        await CreatureCmd.Heal(teammate, teammate.MaxHp * 0.25m);
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    private Creature? GetDeadTeammate()
    {
        return CombatState?.PlayerCreatures.FirstOrDefault(creature =>
            creature.IsDead && creature.Player != Owner);
    }
}
