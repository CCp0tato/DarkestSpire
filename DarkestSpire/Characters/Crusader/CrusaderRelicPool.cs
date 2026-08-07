using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader;

public class CrusaderRelicPool : TypeListRelicPoolModel
{
    // 描述中使用的能量图标。大小为24x24。
    public override string? TextEnergyIconPath => "res://Test/images/energy_test.png";
    // tooltip和卡牌左上角的能量图标。大小为74x74。
    public override string? BigEnergyIconPath => "res://Test/images/energy_test_big.png";

    public override string EnergyColorName => "Crusader";
}