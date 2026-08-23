class CardGenerator:
    def __init__(self, character: str,  card_pool: str, card_name: str, cost: int,
                 card_type: str, card_rarity: str, target_type: str):
        if ' ' in card_name:
            card_name = card_name.replace(' ', '')
        if cost is None:
            cost = -1
        if card_type not in {'Skill', 'Attack', 'Power', 'Status', 'Curse', 'Quest',}:
            card_type = 'None'
        if card_rarity not in {'Common','Uncommon', 'Rare', 'Token', 'Basic', 'Ancient',
                               'Event', 'Curse', 'Quest', 'Status'}:
            card_rarity = 'None'
        if target_type not in {'AllEnemies', 'AnyEnemy', 'AnyAlly', 'AllAllies', 'Self', 'RandomEnemy', 'AnyPlayer',
                               'TargetedNoCreature', 'Osty' }:
            target_type = 'None'

        self.card_name = card_name

        self.card_string = f'''using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace DarkestSpire.Characters.{character}.Cards;

[RegisterCard(typeof({card_pool}))]
public class {card_name} : ModCardTemplate
{{
    public {card_name}() : base({cost}, CardType.{card_type}, CardRarity.{card_rarity}, TargetType.{target_type}, true)
    {{
    }}


    // 卡图资源
    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{{{card_pool}.getImageRoot()}}/{{GetType().Name}}.png"
        // FramePath: "",
        // PortraitBorderPath: "",
        // BannerTexturePath: "" 
    );
'''

    def addKeywords(self, *keywords):
        filtered_kw = []
        for keyword in keywords:
            if keyword not in {'Exhaust', 'Ethereal', 'Innate', 'Unplayable', 'Retain', 'Sly', 'Eternal', }:
                filtered_kw.append('None')
                continue
            filtered_kw.append(keyword)
        filtered_kw = list(set(filtered_kw))

        connected_keywords = ', '.join([f'CardKeyword.{keyword}' for keyword in filtered_kw])

        self.card_string += f'''
    public override IEnumerable<CardKeyword> CanonicalKeywords => [{connected_keywords}];
'''
        return self

    def addTags(self, *tags):
        filtered_tags = []
        for tag in tags:
            if tag not in {'Strike', 'Defend', 'Minion', 'OstyAttack', 'Shiv',}:
                filtered_tags.append('None')
                continue
            filtered_tags.append(tag)
        filtered_tags = list(set(filtered_tags))

        connected_tags = ', '.join([f'CardTag.{tag}' for tag in filtered_tags])
        self.card_string += f'''
    protected override HashSet<CardTag> CanonicalTags => [{connected_tags}];
'''
        return self

    def addVars(self, **dynamics):
        connected_dv = []
        connneced_onplay = []
        for dynamic_var in dynamics:
            if not (dynamic_var in {'DamageVar', 'BlockVar', 'CardsVar', 'HealVar', 'EnergyVar'} or 'PowerVar' in dynamic_var):
                continue
            match dynamic_var:
                case 'DamageVar':
                    connected_dv.append(f'new {dynamic_var}({dynamics[dynamic_var]}, ValueProp.Move)')
                    connneced_onplay.append('await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target!).Execute(choiceContext);')
                case 'BlockVar':
                    connected_dv.append(f'new {dynamic_var}({dynamics[dynamic_var]}, BlockProps.card)')
                    connneced_onplay.append('await CreatureCmd.GainBlock(this.Owner.Creature, this.DynamicVars.Block, cardPlay);')
                case 'HealVar':
                    connected_dv.append(f'new {dynamic_var}({dynamics[dynamic_var]})')
                    connneced_onplay.append('await CreatureCmd.Heal(Owner.Creature, this.DynamicVars.Heal.IntValue);')
                case 'CardsVar':
                    connected_dv.append(f'new {dynamic_var}({dynamics[dynamic_var]})')
                    connneced_onplay.append('await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);')
                case 'EnergyVar':
                    connected_dv.append(f'new {dynamic_var}({dynamics[dynamic_var]})')
                    connneced_onplay.append('await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);')
                case _:
                    powername = dynamic_var.replace('PowerVar_', '').replace('_', '')
                    connected_dv.append(f'new PowerVar<{powername}>({dynamics[dynamic_var]})')
                    connneced_onplay.append(f'await PowerCmd.Apply<{powername}>(choiceContext, Owner.Creature, DynamicVars["{powername}"].IntValue, Owner.Creature, cardPlay.Card);')
        self.card_string += f'''
    protected override IEnumerable<DynamicVar> CanonicalVars => [{', '.join(connected_dv)}];
'''
        self.card_string += f'''
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {{
        {'\n        '.join(connneced_onplay)}
    }}
'''
        return self

    def addUpgrade(self, **upgrades):
        connneced_onplay = []
        for upgrade in upgrades:
            if not (upgrade in {'Damage', 'Block', 'Cards', 'Heal', 'EnergyCost'} or 'PowerVar' in upgrade):
                continue
            match upgrade:
                case 'Damage':
                    connneced_onplay.append(f'DynamicVars.{upgrade}.UpgradeValueBy({upgrades[upgrade]});')
                case 'Block':
                    connneced_onplay.append(f'DynamicVars.{upgrade}.UpgradeValueBy({upgrades[upgrade]});')
                case 'Heal':
                    connneced_onplay.append(f'DynamicVars.{upgrade}.UpgradeValueBy({upgrades[upgrade]});')
                case 'Cards':
                    connneced_onplay.append(f'DynamicVars.{upgrade}.UpgradeValueBy({upgrades[upgrade]});')
                case 'EnergyCost':
                    connneced_onplay.append(f'{upgrade}.UpgradeBy({upgrades[upgrade]});')
                case _:
                    powername = upgrade.replace('PowerVar_', '').replace('_', '')
                    connneced_onplay.append(f'DynamicVars["{powername}"].UpgradeValueBy({upgrades[upgrade]});')
        self.card_string += f'''
    protected override void OnUpgrade()
    {{
        {'\n        '.join(connneced_onplay)}
    }}
'''
        return self

    def output(self):
        self.card_string += '''}'''
        with open(f'{self.card_name}.cs', 'w', encoding='utf-8') as f:
            f.write(self.card_string)


if __name__ == '__main__':
    (CardGenerator(character='HighwayMan', card_name='StrikeHighwayMan', card_type='Attack', card_rarity='Basic',
                   target_type='AnyEnemy', card_pool='HighwayManCardPool', cost=1)
     .addVars(DamageVar=6, BlockVar=4)
     .addTags('Strike')
     .addUpgrade(Damage=3, Block=2)
     .output())








