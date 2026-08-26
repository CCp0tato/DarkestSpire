// | 仁慈 | Mercy | 技能 | 1 | 仅可对生命值低于 40% 的敌人使用，使目标获得 1 层无实体，自身获得 6/8 层信仰，保留，消耗 |

using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class Mercy : ModCardTemplate
{
    public Mercy() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FaithPower>(6)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Target!.IsMonster || cardPlay.Target is null) { return; }
        int TargetHealthPercentage = 100 * cardPlay.Target.CurrentHp / cardPlay.Target.MaxHp;
        if (TargetHealthPercentage >= 40) { return; }
        await PowerCmd.Apply<IntangiblePower>(choiceContext, cardPlay.Target!, 1, Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply<FaithPower>(choiceContext, Owner.Creature, DynamicVars["FaithPower"].IntValue, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FaithPower"].UpgradeValueBy(2);
    }
}