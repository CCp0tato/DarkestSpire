// | 濡湿小镰刀 | DampSickle | 攻击 | 0 | 造成 8 点伤害，本回合获得 2/3 临时力量，将抽牌堆中的所有同名牌置入手牌 |

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DarkestSpire.GeneralPowers;

namespace DarkestSpire.Characters.Crusader.Cards;

[RegisterCard(typeof(CrusaderCardPool))]
public class DampSickle : ModCardTemplate
{
    public DampSickle() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{CrusaderCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move), new PowerVar<TempStrengthPower>(2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);
        await PowerCmd.Apply<TempStrengthPower>(choiceContext, Owner.Creature, DynamicVars["TempStrengthPower"].IntValue, Owner.Creature, this);

        foreach (CardModel card in PileType.Discard.GetPile(Owner).Cards.ToList().Where(c => c is DampSickle))
        {
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["TempStrengthPower"].UpgradeValueBy(1);
    }
}