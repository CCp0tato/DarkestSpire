using System.Reflection;
using DarkestSpire.Characters.Crusader;
using DarkestSpire.Characters.Crusader.Cards;
using DarkestSpire.Characters.HighwayMan;
using DarkestSpire.Characters.HighwayMan.Cards;
using DarkestSpire.GeneralCards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
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

        // 通用卡牌默认只挂在 GeneralCardPool（共享池）上，游戏生成房间卡牌奖励时
        // 只读 player.Character.CardPool（CardCreationOptions.ForRoom），不会去取共享池，
        // 所以这里把每张通用卡显式注入强盗/十字军的角色卡池。
        InjectGeneralCardsIntoCharacterPools(assembly);
        
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

    /// <summary>
    /// 把 <c>DarkestSpire.GeneralCards</c> 下的每张通用卡注入强盗与十字军的卡池。
    /// 使用游戏的 <c>ModHelper.AddModelToPool(poolType, modelType)</c>，
    /// 这也是 RitsuLib 的 <c>[RegisterCard(typeof(Pool))]</c> 最终走的注入通道。
    /// </summary>
    private static void InjectGeneralCardsIntoCharacterPools(Assembly assembly)
    {
        Type[] generalCardTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract
                        && t.Namespace == "DarkestSpire.GeneralCards"
                        && typeof(CardModel).IsAssignableFrom(t))
            .OrderBy(t => t.Name, StringComparer.Ordinal)
            .ToArray();

        if (generalCardTypes.Length == 0)
        {
            Logger.Warn("[Content] No general card found in DarkestSpire.GeneralCards; character pools left untouched.");
            return;
        }

        Type[] targetPools = [typeof(HighwayManCardPool), typeof(CrusaderCardPool)];

        foreach (Type poolType in targetPools)
        {
            foreach (Type cardType in generalCardTypes)
            {
                ModHelper.AddModelToPool(poolType, cardType);
            }
        }

        Logger.Info($"[Content] Injected {generalCardTypes.Length} general card(s) into pools: {string.Join(", ", targetPools.Select(p => p.Name))}.");
    }
}
