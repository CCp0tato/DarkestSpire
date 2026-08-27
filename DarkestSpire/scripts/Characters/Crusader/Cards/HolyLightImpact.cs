// | 圣光冲击 | HolyLightImpact | 攻击 | 2 | 造成 16/19 点伤害；本回合自身每打出过一张攻击牌，则为所有玩家回复 2/3 点生命值 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class HolyLightImpact : ModCardTemplate
{
    public HolyLightImpact() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16, ValueProp.Move), new IntVar("HealAmount", 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);
        int healCount = CombatManager.Instance.History.CardPlaysStarted.Count(entry => entry.CardPlay.Card.Owner == Owner && entry.CardPlay.Card.Type == CardType.Attack);
        if (healCount == 0) { return; }
        var players = CombatState?.PlayerCreatures;
        foreach (var player in players!)
        {
            if (player is null) { continue; }
            await CreatureCmd.Heal(player, DynamicVars["HealAmount"].IntValue * healCount);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["HealAmount"].UpgradeValueBy(1);
    }
}