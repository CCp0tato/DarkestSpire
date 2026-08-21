using DarkestSpire.DarkestSpire.CardTags;
using DarkestSpire.GeneralPowers;
using DarkestSpire.TokenCards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(GeneralCardPool))]
public class Fearlessness : ModCardTemplate
{
    public Fearlessness() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self, true)
    {
    }
    
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{GeneralCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];
    
    protected override HashSet<CardTag> CanonicalTags => 
    [
        DSCardTag.Virtue,
        DSCardTag.Unique,
    ];

    public IEnumerable<CardModel> I_DO_NOT_FEAR => new CardModel[]
    {
        ModelDb.Card<Amoris>(),
        ModelDb.Card<Timoris>(),
        ModelDb.Card<Oblivionis>(),
        ModelDb.Card<Mortis>(),
        ModelDb.Card<Doloris>()
    };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FearlessnessPower>(choiceContext, Owner.Creature, 1, Owner.Creature, cardPlay.Card);
        await CardPileCmd.AddGeneratedCardsToCombat(I_DO_NOT_FEAR, PileType.Draw, Owner, CardPilePosition.Random);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}