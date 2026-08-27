// | 断罪 | Condemnation | 攻击 | 1 | 造成 7 点伤害，额外增加目标最大生命 10%/15% 的伤害，消耗 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class Condemnation : ModCardTemplate
{
    public Condemnation() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
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

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new IntVar("MaxHpPercent", 10)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal damage = DynamicVars.Damage.BaseValue + (cardPlay.Target!.MaxHp * DynamicVars["MaxHpPercent"].IntValue / 100);
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MaxHpPercent"].UpgradeValueBy(5);
    }
}