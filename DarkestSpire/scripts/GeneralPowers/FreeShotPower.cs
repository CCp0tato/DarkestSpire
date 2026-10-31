using DarkestSpire.DarkestSpire.CardTags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class FreeShotPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    /// <summary>
    /// 战斗内把 [枪击] 牌的费用按 0 结算。
    /// 走游戏提供的费用钩子动态计算，不再改写卡牌自身费用，
    /// 因此既不需要遍历牌堆（战斗内用的是抽牌堆里的克隆卡），
    /// 也不需要在层数归零时记录/还原原价。
    /// </summary>
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (Amount <= 0 || card.Owner != Owner.Player || !card.Tags.Contains(DSCardTag.Shot))
            return false;

        modifiedCost = 0;
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        if (cardPlay.Card.Tags.Contains(DSCardTag.Shot))
        {
            await PowerCmd.Decrement(this);
        }
    }
}
