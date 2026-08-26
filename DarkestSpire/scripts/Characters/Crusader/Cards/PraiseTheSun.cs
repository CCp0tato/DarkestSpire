// | 赞美太阳！ | PraiseTheSun | 技能 | 0 | 多人模式，抽 1/2 张牌，获得 1/2 点能量，将这张牌放入随机一名其他玩家的抽牌堆 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class PraiseTheSun : ModCardTemplate
{
    public PraiseTheSun() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, true)
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new EnergyVar(1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);

        var otherPlayers = CombatState!.Players.Where(player => player != Owner).ToList();
        var recipient = Owner.RunState.Rng.CombatTargets.NextItem(otherPlayers)!;
        var transferredCard = CombatState.CreateCard(ModelDb.Card<PraiseTheSun>(), recipient);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(transferredCard);
        }

        await CardPileCmd.AddGeneratedCardToCombat(
            transferredCard,
            PileType.Draw,
            Owner,
            CardPilePosition.Random);
    }

    protected override PileType GetResultPileTypeForCardPlay() => PileType.None; // Exhaust? None?

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars.Energy.UpgradeValueBy(1);
    }
}
