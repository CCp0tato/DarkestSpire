using System.Reflection;
using DarkestSpire.Characters.Example;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.HighwayMan.Cards;

[RegisterCard(typeof(HighwayManCardPool))]
public class Provoke : ModCardTemplate
{
    public Provoke() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, true)
    {
    }


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{HighwayManCardPool.getImageRoot()}/{GetType().Name}.png"
        // FramePath: "", // 卡牌背景
        // PortraitBorderPath: "", // 边框（状态牌感染使用的）
        // BannerTexturePath: "" // 横幅（不同类型）
    );

    // 卡牌基础数值
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        this.EnergyHoverTip,
        HoverTipFactory.FromCard<Dazed>()
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        MonsterModel enemy = cardPlay.Target!.Monster!;
        if (enemy.MoveStateMachine is null)
            return;
        FieldInfo? field = enemy.MoveStateMachine.GetType().GetField(
            "_currentState",
            BindingFlags.NonPublic | 
            BindingFlags.Instance | 
            BindingFlags.FlattenHierarchy 
        );
        MoveState moveState = (MoveState)field.GetValue(enemy.MoveStateMachine);
        if  (moveState is null)
            return;
        await enemy.PerformMove();
        enemy.MoveStateMachine.ForceCurrentState(moveState);
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}