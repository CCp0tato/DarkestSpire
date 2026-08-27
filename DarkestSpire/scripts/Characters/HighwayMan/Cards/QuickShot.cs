using DarkestSpire.DarkestSpire.CardTags;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class QuickShot : ModCardTemplate
{
    public QuickShot() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar("ShotDamage", 4, ValueProp.Move), new CardsVar("Shot", 0)];

    protected override HashSet<CardTag> CanonicalTags => [DSCardTag.Shot];

    protected override void OnUpgrade()
    {
        DynamicVars["ShotDamage"].UpgradeValueBy(2);
    }
    
    public static async Task<IEnumerable<CardModel>> CreateInHand(
        Player owner,
        int count,
        ICombatState combatState)
    {
        if (count == 0)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        if (CombatManager.Instance.IsOverOrEnding)
            return (IEnumerable<CardModel>) Array.Empty<CardModel>();
        List<CardModel> qshots = new List<CardModel>();
        for (int index = 0; index < count; ++index)
            qshots.Add((CardModel) combatState.CreateCard<QuickShot>(owner));
        IReadOnlyList<CardPileAddResult> combat = await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) qshots, PileType.Hand, owner);
        return (IEnumerable<CardModel>) qshots;
    }
}