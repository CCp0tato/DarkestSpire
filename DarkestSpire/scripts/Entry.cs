using System.Reflection;
using DarkestSpire.Characters.Crusader.Cards;
using DarkestSpire.Characters.HighwayMan.Cards;
using DarkestSpire.GeneralCards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop;

namespace DarkestSpire;

[ModInitializer(nameof(Init))]
public class Entry
{
    // 你的modid
    public const string ModId = "DarkestSpire";
    public const string ResPath = $"res://{ModId}";
    private const string BuildMarker = "DarkestSpire loaded vision 0.0.1";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);
    

    public static void Init()
    {
        // harmony可用，但是最好用ritsu的封装patch，见补丁系统一章
        // var harmony = new Harmony("com.example.testmod");
        // harmony.PatchAll();
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        // 自动注册内容
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        
        RitsuLibFramework.RegisterArchaicToothTranscendenceMapping<Radiance, HolyFire>();
        RitsuLibFramework.RegisterArchaicToothTranscendenceMapping<PistolShot, FireSuppression>();
        ModContentRegistry.For(ModId)
            .RegisterCardLibraryCompendiumSharedPoolFilter<GeneralCardPool>(
                "reme_multiclass_shared_pool", // ID
                "res://DarkestSpire/images/characters/Example/character_icon_example.png" // 图标位置
                // null // 放置顺序（可选）
            );
        Logger.Info(BuildMarker);
        
        var cardTags = RitsuLibFramework.GetCardTagRegistry(Entry.ModId);
        cardTags.RegisterOwned("heavy");
    }
}
