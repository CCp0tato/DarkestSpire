using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(GeneralCardPool))]
public class Infighting : ModCardTemplate
{
    public Infighting() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly, true)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{GeneralCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature target = cardPlay.Target!;
        await CardPileCmd.Draw(choiceContext, 1, target.Player!);
        await CardPileCmd.Draw(choiceContext, 1, Owner);

        PowerModel torture = UniquenessControl.DSUniqueDebuff.TakeRandom<PowerModel>(1, Owner.RunState.Rng.CombatPotionGeneration).FirstOrDefault()!;
        
        await PowerCmd.Apply(choiceContext, torture, Owner.Creature, 1, Owner.Creature, cardPlay.Card);
        await PowerCmd.Apply(choiceContext, torture, target, 1, Owner.Creature, cardPlay.Card);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}