using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Scripts.GeneralPowers;

[RegisterPower]
public class DarkestNarratorPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff; // 可改为 Buff 或 Debuff，按设计意图
    public override PowerStackType StackType => PowerStackType.Single;

    // 自定义图标路径（可选）
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    // 记录数据
    private int _damageTakenThisTurn = 0;
    private int _currentCardDamage = 0;
    private bool _cardTriggered = false;
    private bool _takenTriggered = false;
    private CardModel? _currentCard = null;

    // 回合开始时重置所有记录
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner?.Player) return;
        _damageTakenThisTurn = 0;
        _currentCardDamage = 0;
        _cardTriggered = false;
        _takenTriggered = false;
        _currentCard = null;
        await Task.CompletedTask;
    }

    // 当能力拥有者受到伤害时触发
    public override async Task OnDamageTaken(PlayerChoiceContext choiceContext, DamageTaken damageTaken)
    {
        if (damageTaken.Target != Owner?.Creature) return;
        _damageTakenThisTurn += (int)damageTaken.Amount;
        if (_damageTakenThisTurn > 20 && !_takenTriggered)
        {
            _takenTriggered = true;
            Speak("你受到的伤害已超过20点！"); // 请替换为实际台词
        }
        await Task.CompletedTask;
    }

    // 当能力拥有者造成伤害时触发
    public override async Task OnDamageDealt(PlayerChoiceContext choiceContext, DamageDealt damageDealt)
    {
        if (damageDealt.Source != Owner?.Creature) return;
        if (damageDealt.CardSource != null)
        {
            // 如果卡牌变化，重置当前卡牌累计伤害
            if (_currentCard != damageDealt.CardSource)
            {
                _currentCard = damageDealt.CardSource;
                _currentCardDamage = 0;
                _cardTriggered = false;
            }
            _currentCardDamage += (int)damageDealt.Amount;
            if (_currentCardDamage > 45 && !_cardTriggered)
            {
                _cardTriggered = true;
                Speak("你的一张卡牌造成了超过45点伤害！"); // 请替换为实际台词
            }
        }
        await Task.CompletedTask;
    }
}