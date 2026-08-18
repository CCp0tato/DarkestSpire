using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using DarkestSpire.Afflictions;

namespace DarkestSpire.GeneralPowers;

[RegisterPower]
public class ParanoiaPower : UniquePowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsUniquePower => true;

    private CardType _currentCardType = CardType.None;
    
    // public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    // 自定义图标路径。1:1即可。原版游戏大图256x256，小图64x64。
    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/{GetType().Name}.png"
    );

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {   
        if (cardPlay.Card.Owner.Creature != this.Owner)
            return;
        _currentCardType = cardPlay.Card.Type;
        this.Flash();
        foreach (CardModel allCard in this.Owner.Player.PlayerCombatState.AllCards)
        {
            if (allCard.Type == _currentCardType && allCard.Affliction == null)
            {
                ParanoriaAfflicted para = await CardCmd.Afflict<ParanoriaAfflicted>(allCard, 1M);
            }
            else
            {
                if (allCard.Affliction is ParanoriaAfflicted)
                    CardCmd.ClearAffliction(allCard);
            }
        }
    }
    
    public override bool ShouldPlay(CardModel card, AutoPlayType _)
    {
        return card.Owner != this.Owner.Player || !(card.Affliction is ParanoriaAfflicted);
    }
}