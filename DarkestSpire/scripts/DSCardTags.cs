using DarkestSpire;
using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.CardTags;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace DarkestSpire.DarkestSpire.CardTags;

[RegisterOwnedCardTag(nameof(Torture))]
[RegisterOwnedCardTag(nameof(Virtue))]
[RegisterOwnedCardTag(nameof(Unique))]
[RegisterOwnedCardTag(nameof(FightBack))]
[RegisterOwnedCardTag(nameof(Shot))]
public static class DSCardTag
{
    public static readonly CardTag Torture =
        ModContentRegistry
            .GetQualifiedCardTagId(Entry.ModId, nameof(Torture))
            .GetModCardTag();

    public static readonly CardTag Virtue =
        ModContentRegistry
            .GetQualifiedCardTagId(Entry.ModId, nameof(Virtue))
            .GetModCardTag();

    public static readonly CardTag Unique =
        ModContentRegistry
            .GetQualifiedCardTagId(Entry.ModId, nameof(Unique))
            .GetModCardTag();

    public static readonly CardTag FightBack =
        ModContentRegistry
            .GetQualifiedCardTagId(Entry.ModId, nameof(FightBack))
            .GetModCardTag();

    public static readonly CardTag Shot =
        ModContentRegistry
            .GetQualifiedCardTagId(Entry.ModId, nameof(Shot))
            .GetModCardTag();
}
