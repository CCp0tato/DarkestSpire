using DarkestSpire.GeneralCards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class EliteRecruitmentPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override Task AfterCombatEnd(CombatRoom room)
    {
        // 卡牌奖励钩子只会派发给 run-state 监听者（卡组里的卡牌等），Power 收不到。
        // 因此在战斗结束时给卡组中的 EliteRecruitment 卡牌打上标记，由卡牌自己的钩子完成升级。
        if (Owner.Player is { } player)
        {
            foreach (EliteRecruitment card in player.Deck.Cards.OfType<EliteRecruitment>())
                card.PendingRewardUpgrade = true;
        }
        return Task.CompletedTask;
    }
}