---
name: generate-simple-sts2-card
description: Generate Simple Slay the Spire 2 mod card C# scripts by calling CardGenerator.py, and write outputs into the user-requested directory.
---

# Generate Simple STS2 Card

This skill generates Slay the Spire 2 mod card C# files by calling `CardGenerator.py`.

## Use this skill when

Use this skill when the user asks to:
- generate one or more Slay the Spire 2 mod card `.cs` files
- convert card design requirements into generated C# card scripts
- batch generate multiple cards into a specified output directory

## Script capabilities

`CardGenerator.py` supports the following card fields and generated behaviors.

### Base fields
- `character`
- `card_pool`
- `card_name`
- `cost`
- `card_type`
  - `Skill`
  - `Attack`
  - `Power`
  - `Status`
  - `Curse`
  - `Quest`
- `card_rarity`
  - `Common`
  - `Uncommon`
  - `Rare`
  - `Token`
  - `Basic`
  - `Ancient`
  - `Event`
  - `Curse`
  - `Quest`
  - `Status`
- `target_type`
  - `AllEnemies`
  - `AnyEnemy`
  - `AnyAlly`
  - `AllAllies`
  - `Self`
  - `RandomEnemy`
  - `AnyPlayer`
  - `TargetedNoCreature`
  - `Osty`

### Supported keywords
- `Exhaust`
- `Ethereal`
- `Innate`
- `Unplayable`
- `Retain`
- `Sly`
- `Eternal`

### Supported tags
- `Strike`
- `Defend`
- `Minion`
- `OstyAttack`
- `Shiv`

### Supported dynamic vars
- `DamageVar`
- `BlockVar`
- `HealVar`
- `CardsVar`
- `EnergyVar`
- `PowerVar_<PowerName>_`

### Supported upgrades
- `Damage`
- `Block`
- `Heal`
- `Cards`
- `EnergyCost`
- `PowerVar_<PowerName>_`

## Required behavior rules

You must follow all of these rules:

1. If any requested card design contains parts that `CardGenerator.py` cannot implement, generate only the code for the implementable portion.
2. All unimplementable requested features must be written as a comment at the very last line of the generated `.cs` file.
3. Card names must use English only.
4. If the user requests batch generation, also create a `Checklist.md` file in the target output directory.
5. In `Checklist.md`, use each card's `card_name` as the todo item text.
6. If a card is fully implementable by the script, mark it as checked.
7. If a card is not fully implementable, leave it unchecked.

## Important implementation notes

### Card name restriction
- `card_name` must be English only.
- Remove spaces if needed.
- Do not use Chinese, Japanese, or other non-English identifiers in the generated C# class name.

### Power naming restriction
For `PowerVar_<PowerName>_`, the `<PowerName>` must be an existing game power type and should normally include the `Power` suffix.
Examples:
- `PowerVar_StrengthPower_`
- `PowerVar_WeakPower_`

### Output location
Generate the `.cs` files into the directory requested by the user.

## What the script can directly express

The script can directly generate:
- simple attack cards with damage
- simple block cards
- healing cards
- draw cards
- gain energy cards
- self-applied power cards
- standard keyword declarations
- standard tag declarations
- standard numeric upgrades
- cost upgrades

## What the script cannot directly express

Assume the following are not directly supported unless the request can be reduced to supported vars, keywords, tags, or upgrades:

- custom conditional logic
- “if” / “when” / “unless” logic
- random card generation
- arbitrary card creation into hand/discard/draw pile
- discard effects
- exhaust pile manipulation beyond keyword declaration
- custom retain behavior logic
- end-of-turn, start-of-turn, on-draw, on-exhaust, on-kill triggers
- multiple separate hit sequences
- target-state-dependent branching
- status checks like “if target is Weak/Vulnerable then ...”
- hand/deck/discard inspection logic
- summon logic
- minion behavior logic
- custom localization text
- custom VFX/SFX
- custom event hooks
- custom multi-stage resolution
- X-cost special logic
- nonstandard targeting logic beyond provided `target_type`
- effects on arbitrary entities not represented by supported generated behavior

If the user requests unsupported behavior, keep only the supported subset and append one final-line comment describing the unsupported parts.

## Natural language mapping guide

Map user requirements into script arguments using these rules whenever possible.

### Common effect mapping
- “造成X伤害” -> `DamageVar=X`
- “获得X格挡” -> `BlockVar=X`
- “回复X生命” -> `HealVar=X`
- “抽X张牌” -> `CardsVar=X`
- “获得X点能量” -> `EnergyVar=X`
- “获得X层某Power” -> `PowerVar_<PowerName>_=X`

### Upgrade mapping
- “升级后伤害+X” -> `Damage=X`
- “升级后格挡+X” -> `Block=X`
- “升级后治疗+X” -> `Heal=X`
- “升级后抽牌+X” -> `Cards=X`
- “升级后减费1” -> `EnergyCost=-1`
- “升级后某Power层数+X” -> `PowerVar_<PowerName>_=X`

### Keyword mapping
- 消耗 -> `Exhaust`
- 虚无 -> `Ethereal`
- 固有 -> `Innate`
- 无法打出 -> `Unplayable`
- 保留 -> `Retain`
- 狡诈 -> `Sly`
- 永恒 -> `Eternal`

### Tag mapping
- 打击 -> `Strike`
- 防御 -> `Defend`
- 随从 -> `Minion`
- Osty攻击 -> `OstyAttack`
- 小刀 -> `Shiv`

## Workflow

When using this skill, follow this process:

1. Read the user's card requirement carefully.
2. Determine whether the request is for a single card or batch generation.
3. Extract or infer the required base fields:
   - `character`
   - `card_pool`
   - `card_name`
   - `cost`
   - `card_type`
   - `card_rarity`
   - `target_type`
4. Normalize `card_name` to English-only.
5. Map supported requirements into:
   - keywords
   - tags
   - vars
   - upgrades
6. Identify any unsupported requested features.
7. Call `CardGenerator.py` with the proper CLI arguments and output directory.
8. If unsupported features exist, append a single final-line comment to the generated `.cs` file.
9. If this is batch generation, create `Checklist.md` after generating all card files.

## CLI command pattern

Use this pattern:

python {skill_path}/CardGenerator.py \
  --character <character> \
  --card-pool <card_pool> \
  --card-name <card_name> \
  --cost <cost> \
  --card-type <card_type> \
  --card-rarity <card_rarity> \
  --target-type <target_type> \
  [--keywords <kw1> <kw2> ...] \
  [--tags <tag1> <tag2> ...] \
  [--var Key=Value ...] \
  [--upgrade Key=Value ...] \
  --output-dir <target_dir>

## Post-generation unsupported comment format

If unsupported requested features exist, append exactly one final line comment like this:

// Unsupported requested features: <feature1>; <feature2>; <feature3>.

If the design is fully supported, do not append this comment.

## Checklist.md format

For batch generation, create `Checklist.md` in the output directory with this format:

- [x] CardNameA
- [ ] CardNameB
- [x] CardNameC

Rules:
- use `card_name` exactly as the todo item text
- checked means fully supported by `CardGenerator.py`
- unchecked means partially supported or contains unsupported requested features

## Behavior when information is missing

If the user does not provide enough information to safely generate the card, ask for clarification instead of inventing important card metadata.

You may infer only when it is obvious and low-risk, for example:
- an attack card that clearly targets a single enemy -> `target_type=AnyEnemy`
- a skill card that only gives self block/draw/heal -> `target_type=Self`

Do not invent:
- character
- card_pool
- card_rarity
- power type names
unless the user has already provided enough context to make them clear.

## Decision examples

### Example 1: fully supported
User request:
- 1费攻击，对单体造成8伤害，升级+3伤害

Supported mapping:
- `cost=1`
- `card_type=Attack`
- `target_type=AnyEnemy`
- `DamageVar=8`
- `Damage=3`

Result:
- generate the `.cs` file
- no unsupported final-line comment
- mark checked if in batch mode

### Example 2: partially supported
User request:
- 1费攻击，造成8伤害，若目标有易伤则再打一次，升级+3伤害

Supported mapping:
- `cost=1`
- `card_type=Attack`
- `target_type=AnyEnemy`
- `DamageVar=8`
- `Damage=3`

Unsupported:
- “若目标有易伤则再打一次”

Result:
- generate only the supported base attack card
- append final-line unsupported comment
- leave unchecked in `Checklist.md` if in batch mode

### Example 3: batch generation
If the user asks for multiple cards:
1. generate each supported `.cs` file into the requested directory
2. append unsupported comment to each partially supported card file if needed
3. write `Checklist.md` into the same directory

## Execution priority

Always prioritize:
1. generating valid `.cs` files for the implementable subset
2. writing unsupported requested features only as the final-line comment
3. ensuring `card_name` is English-only
4. creating `Checklist.md` for batch generation