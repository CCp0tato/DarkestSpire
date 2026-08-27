// | 神力恩赐 | DivineBlessing | 攻击 | 1 | 造成 15 点伤害；斩杀时此牌伤害永久增加 4/6 点，消耗 |

using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class DivineBlessing : ModCardTemplate
{
    private const int BaseDamage = 15;

    private int _currentDamage = BaseDamage;

    private int _increasedDamage;

    [SavedProperty]
    public int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            AssertMutable();
            _currentDamage = value;
            DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get => _increasedDamage;
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }

    public DivineBlessing() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
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

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.Static(StaticHoverTip.Fatal)];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(CurrentDamage, ValueProp.Move), new IntVar("DamageUpgrade", 4)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool shouldTriggerFatal = cardPlay.Target!.Powers.All(power => power.ShouldOwnerDeathTriggerFatal());
        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        if (shouldTriggerFatal && attackCommand.Results.SelectMany(results => results).Any(result => result.WasTargetKilled))
        {
            int damageIncrease = DynamicVars["DamageUpgrade"].IntValue;
            BuffFromFatal(damageIncrease);
            (DeckVersion as DivineBlessing)?.BuffFromFatal(damageIncrease);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DamageUpgrade"].UpgradeValueBy(2);
    }

    protected override void AfterDowngraded()
    {
        UpdateDamage();
    }

    private void BuffFromFatal(int damageIncrease)
    {
        IncreasedDamage += damageIncrease;
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        CurrentDamage = BaseDamage + IncreasedDamage;
    }
}
