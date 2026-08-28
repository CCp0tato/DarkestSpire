using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.Vestal;

public class VestalPotionPool : TypeListPotionPoolModel
{
    private const string CharacterName = "Vestal";
    private const string ImageRoot = $"{Entry.ResPath}/images/characters/{CharacterName}";

    public override string? TextEnergyIconPath => $"{ImageRoot}/energy_vestal.png";
    public override string? BigEnergyIconPath => $"{ImageRoot}/energy_vestal_big.png";

    public override string EnergyColorName => "vestalEnergy";
}