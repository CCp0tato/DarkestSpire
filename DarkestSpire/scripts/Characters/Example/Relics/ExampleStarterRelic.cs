using DarkestSpire.Characters.Example.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Example.Relics;

// 注册遗物。如果要写自定义池看添加人物的开头
[RegisterRelic(typeof(ExampleRelicPool))]
[RegisterCharacterStarterRelic(typeof(ExampleCharacter))] 
public class ExampleStarterRelic : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public BlockVar blockValue => new(3M, BlockProps.nonCardUnpowered);
    
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
        options.Add((RestSiteOption) new ExampleRestSiteOption(player));
        return true;
    }

    public override async Task AfterObtained()
    {
        await CreatureCmd.GainMaxHp(this.Owner.Creature, 5M);
    }

    public async Task AddBlock()
    {
        await CreatureCmd.GainBlock(this.Owner.Creature, this.blockValue, null);
    }

    public async Task Heal(decimal amount)
    {
        await CreatureCmd.Heal(this.Owner.Creature, amount);
    }
}