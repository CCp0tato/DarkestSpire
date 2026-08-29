using DarkestSpire.Characters.HighwayMan;
using DarkestSpire.Characters.HighwayMan.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Script.Characters.HighwayMan.Potions;

[RegisterPotion(typeof(HighwayManPotionPool))]
public class DuelPotion : ModPotionTemplate
{
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override PotionRarity Rarity => PotionRarity.Common;
    public override TargetType TargetType => TargetType.AnyEnemy;

    public override PotionAssetProfile AssetProfile => new(
        ImagePath: $"{Entry.ResPath}/images/potions/{GetType().Name}.png",
        OutlinePath: $"{Entry.ResPath}/images/potions/{GetType().Name}.png"
    );

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        IEnumerable<CardModel> quickShots = [];
        for (int i = 0; i < 3; i++)
            quickShots.AddItem(Owner.Creature.CombatState.CreateCard<QuickShot>(Owner));
        await CardPileCmd.Add(quickShots, PileType.Hand);
    }
}