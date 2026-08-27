using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;

namespace Test.Scripts;

[RegisterOwnedCardKeyword(nameof(Shot),  CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]
[RegisterOwnedCardKeyword(nameof(FightBack),  CardDescriptionPlacement = ModKeywordCardDescriptionPlacement.BeforeCardDescription)]

public class MyKeywords
{
    public static readonly CardKeyword Shot = ModContentRegistry.GetQualifiedKeywordId(Entry.DarkestSpire, nameof(Shot)).GetModCardKeyword();
    public static readonly CardKeyword FightBack = ModContentRegistry.GetQualifiedKeywordId(Entry.DarkestSpire, nameof(FightBack)).GetModCardKeyword();
}