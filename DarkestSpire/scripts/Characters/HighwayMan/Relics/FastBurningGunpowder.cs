using DarkestSpire.DarkestSpire.CardTags;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Relics; 

// 注册遗物。如果要写自定义池看添加人物的开头
[RegisterRelic(typeof(HighwayManRelicPool))]
[RegisterCharacterStarterRelic(typeof(HighwayManCharacter))] 
public class FastBurningGunpowder : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override RelicAssetProfile AssetProfile => new(
        // 小图标（原版85x85）
        IconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        // 轮廓图标（原版85x85）
        IconOutlinePath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png",
        // 大图标（原版256x256）
        BigIconPath: $"{Entry.ResPath}/images/relics/{GetType().Name}.png"
    );

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != this.Owner)
            return false;
        return true;
    }

    private bool _hurtLastTurn = true;

    public override Task BeforeCombatStart()
    {
        foreach (CardModel card in Owner.Deck.Cards)
        {
            if (card.Tags.Contains(DSCardTag.Shot))
            {
                card.EnergyCost.SetUntilPlayed(0);
            }
        }
        return Task.CompletedTask;
    }
}