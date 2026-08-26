// | 悲伤 | Doloris | 状态 | 1 | 抽到这张牌时生成一个[黑暗]充能球；打出时[激发]最右侧的充能球 4 次；[消耗]。 |

using DarkestSpire.TokenCards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.GeneralCards;

[RegisterCard(typeof(DSTokenCardPool))]
public class Doloris : ModCardTemplate
{
    public override OrbEvokeType OrbEvokeType => OrbEvokeType.Front;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new RepeatVar(4)];

    public Doloris() : base(1, CardType.Status, CardRarity.Token, TargetType.None, true)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{DSTokenCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card != this)
            return;

        await Cmd.Wait(0.25f);
        await OrbCmd.Channel<DarkOrb>(choiceContext, Owner);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.PlayerCombatState?.OrbQueue.Orbs.Count is not > 0)
            return;

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        for (var i = 0; i < DynamicVars.Repeat.IntValue; i++)
        {
            var isFinalEvoke = i == DynamicVars.Repeat.IntValue - 1;
            await OrbCmd.EvokeLast(choiceContext, Owner, isFinalEvoke);
            if (!isFinalEvoke)
                await Cmd.CustomScaledWait(0.15f, 0.25f);
        }
    }
}
