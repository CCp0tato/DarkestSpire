using DarkestSpire.Characters.Crusader;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Script.Characters.Crusader.Potions;

[RegisterPotion(typeof(CrusaderPotionPool))]
public class BottledDarkSouls : ModPotionTemplate
{
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override TargetType TargetType => TargetType.Self;

    public override PotionAssetProfile AssetProfile => new(
        ImagePath: $"{Entry.ResPath}/images/potions/{GetType().Name}.png",
        OutlinePath: $"{Entry.ResPath}/images/potions/{GetType().Name}.png"
    );

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (Owner == null) return;
        CardModel particleWall = Owner.RunState.CreateCard<ParticleWall>(Owner);
        CardCmd.ApplyKeyword(particleWall, CardKeyword.Ethereal);
        //没找到把辉星费用设置为0的方法，或者你们去看一下干手之类的“免费”是怎么生效的
        await CardPileCmd.AddGeneratedCardToCombat(particleWall, PileType.Hand, Owner);
    }
}