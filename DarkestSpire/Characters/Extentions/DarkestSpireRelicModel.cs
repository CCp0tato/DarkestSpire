using Godot;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Extentions
{
    /// 黑暗地牢（DarkestSpire）模组的遗物基类。
    /// 自动根据子类名称生成三个图标路径，约定为：
    /// res://DarkestSpire/DarkestSpire/images/relics/{类名}.png
    public abstract class DarkestSpireRelicModel : ModRelicTemplate
    {
        // 重写 AssetProfile，返回基于当前类名的路径（三个图标使用同一张图）
        public override RelicAssetProfile AssetProfile => new(
            IconPath: $"res://DarkestSpire/DarkestSpire/images/relics/{GetType().Name}.png",
            IconOutlinePath: $"res://DarkestSpire/DarkestSpire/images/relics/{GetType().Name}.png",
            BigIconPath: $"res://DarkestSpire/DarkestSpire/images/relics/{GetType().Name}.png"
        );
    }
}