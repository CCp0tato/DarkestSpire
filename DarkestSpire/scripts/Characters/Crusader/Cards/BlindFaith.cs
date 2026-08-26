// | 盲信 | BlindFaith | 攻击 | 0 | 造成 7/10 点伤害，获得 2/3 层信仰，将一张晕眩加入你的抽牌堆 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DarkestSpire.GeneralPowers;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class BlindFaith : ModCardTemplate
{
    public BlindFaith() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new PowerVar<FaithPower>(2)];

    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!cardPlay.Target!.IsMonster || cardPlay.Target is null) { return; }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);
        await PowerCmd.Apply<FaithPower>(choiceContext, Owner.Creature, DynamicVars["FaithPower"].IntValue, Owner.Creature, this);
        await CardPileCmd.AddToCombatAndPreview<Dazed>(Owner.Creature, PileType.Draw, 1, Owner, CardPilePosition.Random);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["FaithPower"].UpgradeValueBy(1);
    }
}