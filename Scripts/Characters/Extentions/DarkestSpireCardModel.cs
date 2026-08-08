using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Extentions;

// 设置Inherit为true允许自动注册该类的所有子类
[RegisterCard(typeof(TestCardPool), Inherit = true)]
public abstract class DarkestSpireCardModel : ModCardTemplate
{
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"res://DarkestSpire/images/cards/{GetType().Name}.png",
        // 根据不同类型设置不同卡框
        FramePath: Type switch
        {
            CardType.Attack => "res://DarkestSpire/images/card_frame_attack.png",
            CardType.Skill => "res://DarkestSpire/images/card_frame_skill.png",
            CardType.Power => "res://DarkestSpire/images/card_frame_power.png",
            _ => ""
        }
        // PortraitBorderPath: "",
        // BannerTexturePath: ""
    );

    public DarkestSpireCardModel(int energyCost, CardType type, CardRarity rarity, TargetType targetType, bool shouldShowInCardLibrary)
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
}