using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace DarkestSpire.Scripts;

[ModInitializer(nameof(Init))]
public class Entry
{
    public const string ModId = "DarkestSpire";
    private const string BuildMarker = "DarkestSpire loaded vision 1.0.1";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);
    private static Harmony? _harmony;

    public static void Init()
    {
        LocalMultiControlLogger.Info("开始初始化 RitsuLib 补丁。");
        LocalMultiControlLogger.Info(BuildMarker);
        LocalWakuuRelicLocalization.Initialize();
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        RitsuLibFramework.RegisterArchaicToothTranscendenceMapping<TestCard, Shiv>();
        LocalMultiControlLogger.Info("Mod 初始化完成。");
    }
}
