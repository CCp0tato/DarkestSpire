using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class HolyFire : ModCardTemplate
{
    public HolyFire() : base(0, CardType.Skill, CardRarity.Basic, TargetType.AnyPlayer, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (Creature creature in (IEnumerable<Creature>) this.CombatState!.PlayerCreatures.Where<Creature>((Func<Creature, bool>) (c => c != null! && c.IsAlive)).ToList<Creature>())
        {
            await CreatureCmd.Heal(creature,
                creature.HasPower<FaithPower>() ? creature.GetPower<FaithPower>()!.Amount : 0M);
        }
    }
}