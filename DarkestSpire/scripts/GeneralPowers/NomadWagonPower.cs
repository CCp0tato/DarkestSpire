using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class NomanWagonPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );
    
    private IEnumerable<RelicModel> fakeRelics
    {
        get
        {
            return new RelicModel[9]
            {
                (RelicModel)ModelDb.Relic<FakeAnchor>(),
                (RelicModel)ModelDb.Relic<FakeBloodVial>(),
                (RelicModel)ModelDb.Relic<FakeHappyFlower>(),
                (RelicModel)ModelDb.Relic<FakeLeesWaffle>(),
                (RelicModel)ModelDb.Relic<FakeMango>(),
                (RelicModel)ModelDb.Relic<FakeMerchantsRug>(),
                (RelicModel)ModelDb.Relic<FakeOrichalcum>(),
                (RelicModel)ModelDb.Relic<FakeStrikeDummy>(),
                (RelicModel)ModelDb.Relic<FakeVenerableTeaSet>()
            };
        }
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        for (int i = 0; i < Amount; i++)
        {
            // ModelDb.Relic<T>() 返回的是 canonical（全局共享、只读）模型实例。
            // RelicReward 构造函数会调用 AssertMutable()，直接传入 canonical 模型会抛
            // CanonicalModelException；该异常发生在战斗结束流程中，会导致战斗结束卡死。
            // 因此必须先克隆出可变实例，再作为奖励发放。
            RelicModel? relic = fakeRelics.TakeRandom(1, Owner.Player.RunState.Rng.TreasureRoomRelics).FirstOrDefault();
            if (relic == null)
                continue;
            room.AddExtraReward(this.Owner.Player, (Reward) new RelicReward(relic.ToMutable(), Owner.Player));
        }
        return Task.CompletedTask;
    }
}