using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Test.Scripts;

[RegisterOwnedCardKeyword(nameof(Shot),  CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(FightBack),  CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(Virtue),  CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(Torture),  CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]

public class DSKeywords
{
    public static readonly CardKeyword Shot = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(Shot)).GetModCardKeyword();
    public static readonly CardKeyword FightBack = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(FightBack)).GetModCardKeyword();
    public static readonly CardKeyword Virtue = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(Virtue)).GetModCardKeyword();
    public static readonly CardKeyword Torture = ModContentRegistry.GetQualifiedKeywordId(Entry.ModId, nameof(Torture)).GetModCardKeyword();
}