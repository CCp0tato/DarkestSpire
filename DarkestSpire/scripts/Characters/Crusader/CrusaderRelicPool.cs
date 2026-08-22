using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Crusader;

public class CrusaderRelicPool : TypeListRelicPoolModel
{
    private const string ImageRoot = $"{Entry.ResPath}/images/characters/Example";
    
    // 描述中使用的能量图标。大小为24x24。
    public override string? TextEnergyIconPath => $"{ImageRoot}/energy_example.png";
    // tooltip和卡牌左上角的能量图标。大小为74x74。
    public override string? BigEnergyIconPath => $"{ImageRoot}/energy_example_big.png";

    public override string EnergyColorName => "ExampleEnergy";
}