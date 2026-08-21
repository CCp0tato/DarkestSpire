using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

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
        for (int i = 0; i<Amount; i++)
            room.AddExtraReward(this.Owner.Player, (Reward) new RelicReward(fakeRelics.TakeRandom(1, Owner.Player.RunState.Rng.TreasureRoomRelics).FirstOrDefault(), Owner.Player));
        return Task.CompletedTask;
    }
}