using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Combat.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace DarkestSpire.Extensions
{
    public abstract class DarkestSpirePower : ModPowerTemplate
    {
        public override PowerAssetProfile AssetProfile => new(
            IconPath: $"res://DarkestSpire/DarkestSpire/images/powers/{GetType().Name}.png",
            BigIconPath: $"res://DarkestSpire/DarkestSpire/images/powers/{GetType().Name}.png"
        );
    }
}