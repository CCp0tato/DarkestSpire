// | 远征 | Expedition | 攻击 | 1 | 造成 6/10 点伤害；击败阶层头目时从卡组移除这张牌，并使你额外获得 100/150 金币和 2 个遗物 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class Expedition : ModCardTemplate
{
    public Expedition() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, ValueProp.Move), new GoldVar(100)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (room.RoomType != RoomType.Boss || Pile?.Type != PileType.Deck)
            return;

        room.AddExtraReward(Owner, new GoldReward(DynamicVars.Gold.IntValue, Owner));
        room.AddExtraReward(Owner, new RelicReward(Owner));
        room.AddExtraReward(Owner, new RelicReward(Owner));
        await CardPileCmd.RemoveFromDeck(this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
        DynamicVars.Gold.UpgradeValueBy(50);
    }
}
