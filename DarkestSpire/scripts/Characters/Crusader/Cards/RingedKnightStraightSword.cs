// | 环印骑士直剑 | RingedKnightStraightSword | 攻击 | 1 | 本场战斗中每获得过一次信仰，就造成 5/7 点伤害一次 |

using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class RingedKnightStraightSword : ModCardTemplate
{
    private const string CalculatedHitsKey = "CalculatedHits";

    public RingedKnightStraightSword() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new CalculationBaseVar(0),
        new CalculationExtraVar(1),
        new CalculatedVar(CalculatedHitsKey).WithMultiplier((CardModel card, Creature? _) =>
            CombatManager.Instance.History.Entries
                .OfType<PowerReceivedEntry>()
                .Count(entry => entry.Actor == card.Owner.Creature &&
                                entry.Power is FaithPower &&
                                entry.Amount > 0))
    ];
    // 获得的总层数写法：.OfType<...>().Where(entry => entry.Actor ... ).Sum(entry => entry.Amount))

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Target!.IsMonster || cardPlay.Target is null) { return; }
        int hitCount = (int)((CalculatedVar)DynamicVars[CalculatedHitsKey]).Calculate(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(hitCount)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }
}
