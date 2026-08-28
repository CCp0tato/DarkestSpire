using Godot;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace DarkestSpire.Characters.Vestal;

public class VestalCardPool : TypeListCardPoolModel
{
    private const string CharacterName = "Vestal";
    private const string ImageRoot = $"{Entry.ResPath}/images/characters/{CharacterName}";

    public override string Title => "VestalCardPool";
    public override string EnergyColorName => "VestalEnergyColor";

    public override string? TextEnergyIconPath => $"{ImageRoot}/energy_vestal.png";
    public override string? BigEnergyIconPath => $"{ImageRoot}/energy_vestal_big.png";

    public override Color DeckEntryCardColor => new(0.5f, 0.5f, 1f);
    public override Color EnergyOutlineColor => new(0.5f, 0.5f, 1f);

    private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateReplaceHueShaderMaterial(0.5f, 0.5f, 1f); // 如果你使用原版卡框，使用这个直接替换色调。
    // private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateRgbShaderMaterial(0.5f, 0.5f, 1f); // 使用原版卡框替换色调。除非你的版本没有CreateReplaceHueShaderMaterial函数，否则应使用上面那种
    // private static readonly Material? _poolFrameMaterial = MaterialUtils.CreateUnmodulatedHsvShaderMaterial(); // 如果你是自定义卡框，使用这个
    public override Material? PoolFrameMaterial => _poolFrameMaterial;

    public override bool IsColorless => false;

    public static string getImageRoot()
    {
        return ImageRoot;
    }
}