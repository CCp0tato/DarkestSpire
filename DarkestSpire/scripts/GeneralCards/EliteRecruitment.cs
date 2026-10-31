using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(GeneralCardPool))]
public class EliteRecruitment : ModCardTemplate
{
    public EliteRecruitment() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{GeneralCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );
    
    // 由 EliteRecruitmentPower.AfterCombatEnd 设置。
    // 卡牌奖励钩子是 run-state 钩子，只对卡组中的卡牌/遗物等 run-state 监听者派发，
    // 战斗中的 Power 不会收到该钩子，所以把逻辑放在卡牌本身，并用这个战斗结束标记来判定本场是否打出过。
    internal bool PendingRewardUpgrade;

    public override bool TryModifyCardRewardOptions(
        Player player,
        List<CardCreationResult> cardRewards,
        CardCreationOptions options)
    {
        if (player != Owner || !options.Flags.HasFlag(CardCreationFlags.IsCardReward))
            return false;
        if (!PendingRewardUpgrade && Owner.Creature?.HasPower<EliteRecruitmentPower>() != true)
            return false;
        PendingRewardUpgrade = false;

        foreach (CardCreationResult cardReward in cardRewards)
        {
            CardModel card = cardReward.Card;
            if (!card.IsUpgradable)
                continue;
            CardModel upgraded = Owner.RunState.CloneCard(card);
            CardCmd.Upgrade(upgraded);
            cardReward.ModifyCard(upgraded, null);
        }
        return true;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // 清理上一场战斗遗留的标记（例如本场没有卡牌奖励时）。
        PendingRewardUpgrade = false;
        return Task.CompletedTask;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EliteRecruitmentPower>(choiceContext, Owner.Creature,
            1, this.Owner.Creature, cardPlay.Card);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}