using DarkestSpire.Characters.Crusader;
using DarkestSpire.GeneralPowers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Script.Characters.Crusader.Potions;

[RegisterPotion(typeof(CrusaderPotionPool))]
public class LiquidHolyFire : ModPotionTemplate
{
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override PotionRarity Rarity => PotionRarity.Common;
    public override TargetType TargetType => TargetType.Self;

    public override PotionAssetProfile AssetProfile => new(
        ImagePath: $"{Entry.ResPath}/images/potions/{GetType().Name}.png",
        OutlinePath: $"{Entry.ResPath}/images/potions/{GetType().Name}.png"
    );

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PowerCmd.Apply<FaithPower>(choiceContext, Owner.Creature, 6, Owner.Creature, null);
    }
}