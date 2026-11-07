# Go Fishing 2.4.2 Detailed Stats Guide

Applicable to: official source and config as of November 6, 2026; includes fixed-kcal meat cuts, trilingual text, always-visible boat key, and the 60 L standalone water tank rule. It is the numeric supplement to the [Mechanics Guide](去钓鱼2.4.2模组攻略_EN.md); the two can be read together.

All amounts are in credits, weights in grams, food calories in kcal, and power in device energy points. The defined base price, formula quote, and shop transaction price in the tables are explained separately. Market conditions, native quality traits, actual weight, and rounding may continue to affect interface prices.

**This guide verifies source, config, and related vanilla interfaces; no new game play-testing was added. Farming multipliers are attainable caps; reference quotes are not net profit after deducting feed, battery, equipment, and space costs.**

## Table of Contents

1. [Fish Species, Defined Weight & Base Price](#1-fish-species-defined-weight--base-price)
2. [Six-Stage Growth & Farming Physiology](#2-six-stage-growth--farming-physiology)
3. [Farming Markup & Profit Calculation](#3-farming-markup--profit-calculation)
4. [Fixed-Calorie Filleting & By-products](#4-fixed-calorie-filleting--by-products)
5. [Farming Equipment & Worms](#5-farming-equipment--worms)
6. [Fishing XP, Grade & Traits](#6-fishing-xp-grade--traits)
7. [Voyage, Gear, Bait & Ship](#7-voyage-gear-bait--ship)
8. [Unlocks, Research & Delivery](#8-unlocks-research--delivery)
9. [Canning, Drying, Printing & 58 Processing Recipes](#9-canning-drying-printing--processing-recipes)
10. [Fishmeal & Reagent Full Stats](#10-fishmeal--reagent-full-stats)
11. [Calculation Basis & Version Sources](#11-calculation-basis--version-sources)

## 1. Fish Species, Defined Weight & Base Price

The current catalog has 33 species: 25 biological fish (including two large hunt targets and black fish) and 8 mechanical fish. So 33 is the catalog count and cannot be called "33 ordinary fishing targets".

The "defined weight range" below is the base config range, not the farming weight cap, nor a promised final catch weight with traits. Biological fish enter native growth and first-trait processing during voyages; large, fat, giant, and specific nodes change results. "Base sprite footprint" uses the current sprite definition; growth stage, rotation, old-save compatibility, and temporary hold shape can change actual footprint.

For an ordinary whole fish with no other modifiers, the base value is approximately `floor(weight g × per-kg base price ÷ 1000)`. For example, a 1000 g black fish is 100, not that all black fish sell for a fixed 100.

| Species | Category | Defined weight range g | Whole-fish base price /kg | Base sprite footprint |
|---|---|---|---|---|
| Small yellow croaker | Biological | 50–200 | 30 | 2×1 |
| Large yellow croaker | Biological | 300–2000 | 45 | 3×2 |
| Sardine | Biological | 50–150 | 35 | 2×1 |
| Water-purifier fish | Biological | 400–1200 | 90 | 4×2 |
| Volcanic fish | Biological | 500–1500 | 70 | 3×2 |
| Glow fish | Biological | 1500–4000 | 55 | 3×3 |
| Glass fish | Biological | 100–400 | 480 | 2×1 |
| Great white shark | Hunt target | 50000–100000 | 30 | 6×2 |
| Emperor red-armor crab | Hunt target | 25000–50000 | 60 | 6×4 |
| Swordfish | Biological | 4000–15000 | 12 | 12×2 |
| Rainbow fish | Biological | 300–900 | 200 | 2×2 |
| Goldfish | Biological | 300–1000 | 450 | 4×2 |
| Blue crystal fish | Biological | 500–1500 | 300 | 3×3 |
| Red-tail fish | Biological | 1000–3000 | 75 | 5×2 |
| Mouse fish | Biological | 200–800 | 100 | 3×2 |
| Big-eye fish | Mechanical | 600–1000 | 188 | 2×2 |
| Electric clam | Mechanical | 800–1200 | 150 | 3×2 |
| Kelp cable | Mechanical | 400–800 | 233 | 2×2 |
| Energy fish | Mechanical | 1200–1800 | 107 | 2×3 |
| Copper-wire eel | Mechanical | 600–1000 | 175 | 2×2 |
| Chip puffer | Mechanical | 700–1100 | 172 | 2×2 |
| Chip crab | Mechanical | 500–900 | 214 | 2×2 |
| Pearl shell | Mechanical | 400–600 | 310 | 2×2 |
| Ferrite jellyfish | Biological | 600–1000 | 81 | 2×3 |
| Wu Zhi Qi | Biological | 1800–3200 | 80 | 3×2 |
| Star dolphin | Biological | 400–800 | 75 | 2×2 |
| Goldfish (金鱼) | Biological | 150–450 | 100 | 2×2 |
| Rahn's Nightmare | Biological | 1800–3000 | 75 | 3×2 |
| Luo Fu Ling | Biological | 800–1600 | 350 | 4×4 |
| Muspel's Son | Biological | 1200–2400 | 108 | 4×3 |
| Space rocket fish | Biological | 800–1600 | 67 | 3×2 |
| Neolon | Biological | 1200–2400 | 278 | 4×4 |
| Black fish | Farm supply | 800–4000 | 100 | 6×2 |

The swordfish adult base shape is 12×2 (2×12 when rotated); the voyage hold uses a temporary shrunken shape, restored on return; old swordfish compatibility shapes do not change original age, health, etc. Mechanical fish follow the mechanical dismantling route and do not apply the biological growth and filleting tables below.

## 2. Six-Stage Growth & Farming Physiology

The six columns are always in the order **roe / fry / juvenile / sub-adult / adult / elder**. "Stage start" is the native development-age threshold; actual calendar days elapsed depend on food and growth speed. "Stage weight" is the growth-curve node, not a daily automatically-granted weight gain.

Adult does not equal immediately earning full farming markup. Look at development age and weight first, then the actual feeding-weight ledger in Chapter 3.

### 2.1 Stage Start: Development Days

| Species | Roe | Fry | Juvenile | Sub-adult | Adult | Elder | Aging-risk start |
|---|---|---|---|---|---|---|---|
| Small yellow croaker | 0 | 1 | 2 | 3 | 4 | 18 | 30 |
| Large yellow croaker | 0 | 1 | 2 | 4 | 7 | 26 | 42 |
| Sardine | 0 | 1 | 2 | 3 | 4 | 16 | 26 |
| Water-purifier fish | 0 | 1 | 3 | 7 | 14 | 28 | 44 |
| Volcanic fish | 0 | 1 | 3 | 7 | 13 | 30 | 46 |
| Glow fish | 0 | 1 | 3 | 7 | 14 | 36 | 60 |
| Glass fish | 0 | 1 | 4 | 7 | 12 | 36 | 50 |
| Great white shark | 0 | 4 | 12 | 28 | 50 | 90 | 130 |
| Emperor red-armor crab | 0 | 3 | 10 | 24 | 45 | 85 | 125 |
| Swordfish | 0 | 1 | 4 | 8 | 15 | 40 | 64 |
| Rainbow fish | 0 | 1 | 4 | 7 | 11 | 30 | 46 |
| Goldfish | 0 | 2 | 4 | 8 | 18 | 48 | 68 |
| Blue crystal fish | 0 | 2 | 4 | 8 | 18 | 46 | 66 |
| Red-tail fish | 0 | 1 | 2 | 6 | 12 | 30 | 46 |
| Mouse fish | 0 | 1 | 2 | 5 | 7 | 22 | 36 |
| Ferrite jellyfish | 0 | 1 | 3 | 7 | 11 | 32 | 50 |
| Wu Zhi Qi | 0 | 1 | 3 | 7 | 11 | 32 | 50 |
| Star dolphin | 0 | 1 | 3 | 6 | 8 | 26 | 42 |
| Goldfish (金鱼) | 0 | 1 | 2 | 4 | 8 | 22 | 36 |
| Rahn's Nightmare | 0 | 1 | 4 | 7 | 13 | 34 | 54 |
| Luo Fu Ling | 0 | 2 | 4 | 8 | 15 | 42 | 61 |
| Muspel's Son | 0 | 1 | 3 | 7 | 13 | 32 | 50 |
| Space rocket fish | 0 | 1 | 3 | 6 | 10 | 28 | 44 |
| Neolon | 0 | 1 | 4 | 8 | 15 | 40 | 59 |
| Black fish | 0 | 1 | 3 | 7 | 13 | 28 | 42 |

After reaching the aging-risk start, the native daily age settlement has a 50% natural-death check; it is not "dies on that day", nor does it mean death cannot occur earlier from health, food, or environment problems. The freezer pauses related life activities and cannot revive dead fish.

### 2.2 Stage Weight: g

| Species | Roe | Fry | Juvenile | Sub-adult | Adult | Elder |
|---|---|---|---|---|---|---|
| Small yellow croaker | 1 | 3 | 12 | 30 | 125 | 200 |
| Large yellow croaker | 1 | 6 | 65 | 180 | 1150 | 2000 |
| Sardine | 1 | 2 | 10 | 28 | 125 | 150 |
| Water-purifier fish | 1 | 7 | 90 | 220 | 800 | 1200 |
| Volcanic fish | 1 | 8 | 110 | 300 | 1000 | 1500 |
| Glow fish | 1 | 12 | 320 | 900 | 2750 | 4000 |
| Glass fish | 1 | 3 | 20 | 60 | 250 | 400 |
| Great white shark | 1 | 100 | 3000 | 18000 | 75000 | 100000 |
| Emperor red-armor crab | 1 | 60 | 1250 | 8000 | 37500 | 50000 |
| Swordfish | 1 | 20 | 800 | 2300 | 9500 | 15000 |
| Rainbow fish | 1 | 4 | 55 | 170 | 600 | 900 |
| Goldfish | 1 | 5 | 60 | 180 | 650 | 1000 |
| Blue crystal fish | 1 | 8 | 100 | 300 | 1000 | 1500 |
| Red-tail fish | 1 | 10 | 220 | 600 | 2000 | 3000 |
| Mouse fish | 1 | 4 | 40 | 120 | 500 | 800 |
| Ferrite jellyfish | 1 | 10 | 150 | 350 | 800 | 1000 |
| Wu Zhi Qi | 1 | 14 | 400 | 1100 | 2500 | 3200 |
| Star dolphin | 1 | 6 | 85 | 240 | 600 | 800 |
| Goldfish (金鱼) | 1 | 3 | 30 | 90 | 300 | 450 |
| Rahn's Nightmare | 1 | 15 | 420 | 1000 | 2400 | 3000 |
| Luo Fu Ling | 1 | 7 | 150 | 460 | 1200 | 1600 |
| Muspel's Son | 1 | 12 | 260 | 720 | 1800 | 2400 |
| Space rocket fish | 1 | 8 | 180 | 480 | 1200 | 1600 |
| Neolon | 1 | 9 | 240 | 700 | 1800 | 2400 |
| Black fish | 1 | 8 | 120 | 650 | 2400 | 4000 |

### 2.3 Stage Water Need: ml

| Species | Roe | Fry | Juvenile | Sub-adult | Adult | Elder |
|---|---|---|---|---|---|---|
| Small yellow croaker | 20 | 30 | 70 | 150 | 250 | 450 |
| Large yellow croaker | 40 | 100 | 270 | 670 | 1200 | 2000 |
| Sardine | 20 | 30 | 50 | 100 | 180 | 300 |
| Water-purifier fish | 50 | 140 | 330 | 890 | 1500 | 2200 |
| Volcanic fish | 40 | 100 | 270 | 650 | 1300 | 2000 |
| Glow fish | 50 | 130 | 700 | 1750 | 3500 | 5000 |
| Glass fish | 30 | 70 | 180 | 470 | 600 | 900 |
| Great white shark | 100 | 500 | 900 | 4000 | 10000 | 15000 |
| Emperor red-armor crab | 90 | 320 | 580 | 2530 | 6500 | 10000 |
| Swordfish | 150 | 380 | 900 | 2250 | 4500 | 7000 |
| Rainbow fish | 40 | 110 | 300 | 750 | 1400 | 2100 |
| Goldfish | 60 | 160 | 420 | 1020 | 1800 | 2700 |
| Blue crystal fish | 70 | 170 | 520 | 1150 | 2300 | 3400 |
| Red-tail fish | 50 | 120 | 550 | 1290 | 2400 | 3600 |
| Mouse fish | 20 | 60 | 140 | 320 | 600 | 1000 |
| Ferrite jellyfish | 50 | 120 | 400 | 900 | 1000 | 1500 |
| Wu Zhi Qi | 50 | 120 | 710 | 1660 | 3000 | 4300 |
| Star dolphin | 40 | 90 | 240 | 550 | 1000 | 1500 |
| Goldfish (金鱼) | 20 | 40 | 110 | 250 | 450 | 750 |
| Rahn's Nightmare | 50 | 130 | 710 | 1640 | 3000 | 4500 |
| Luo Fu Ling | 60 | 150 | 570 | 1260 | 2400 | 3500 |
| Muspel's Son | 40 | 120 | 520 | 1190 | 2300 | 3400 |
| Space rocket fish | 30 | 80 | 350 | 830 | 1600 | 2400 |
| Neolon | 50 | 140 | 640 | 1550 | 3000 | 4400 |
| Black fish | 40 | 100 | 350 | 1000 | 2400 | 4000 |

### 2.4 Nocturnal Power Generation

The following are fish **power generation** parameters, not consumption. Unlisted species are 0 in all six stages; they take effect only under native farming settlement conditions and cannot be treated as carried power just from the fish name.

| Species | Roe | Fry | Juvenile | Sub-adult | Adult | Elder |
|---|---|---|---|---|---|---|
| Glow fish | 0 | 0 | 1 | 4 | 6 | 4 |
| Ferrite jellyfish | 0 | 0 | 0 | 0 | 4 | 6 |

### 2.5 Diet, Energy & Health

Maintenance coefficient is kcal per 100 g reference weight per night; growth coefficient is kcal per 1 g gained. Native maintenance uses the age-corresponding reference weight, with elder-stage maintenance multiplied by 0.5. Raw energy budget = maintenance budget + growth budget; feeding is further affected by conversion efficiency, diet, and available food, so do not treat this table as a fixed daily feed-deduction table.

| Species | Diet | Maint. coeff | Growth coeff | Conv. eff. | Hunger cap | Health cap | Meat-growth accel |
|---|---|---|---|---|---|---|---|
| Small yellow croaker | Omnivore | 1.2 | 1.05 | 93% | 250 | 90 | 10% |
| Large yellow croaker | Carnivore | 1.6 | 1.2 | 92% | 1200 | 110 | 20% |
| Sardine | Herbivore | 1 | 0.95 | 96% | 250 | 80 | 0% |
| Water-purifier fish | Omnivore | 1.2 | 1.2 | 84% | 900 | 115 | 10% |
| Volcanic fish | Carnivore | 2.5 | 1.3 | 78% | 1000 | 125 | 20% |
| Glow fish | Carnivore | 1.8 | 1.2 | 28% | 2200 | 120 | 20% |
| Glass fish | Omnivore | 1.2 | 1.4 | 65% | 250 | 60 | 5% |
| Great white shark | Carnivore | 0.55 | 1.2 | 82% | 8850 | 250 | 20% |
| Emperor red-armor crab | Omnivore | 0.6 | 1.5 | 72% | 5850 | 300 | 10% |
| Swordfish | Carnivore | 0.8 | 0.85 | 86% | 6500 | 145 | 20% |
| Rainbow fish | Omnivore | 1.4 | 1.25 | 74% | 350 | 85 | 10% |
| Goldfish | Omnivore | 1.8 | 1.5 | 62% | 900 | 105 | 10% |
| Blue crystal fish | Omnivore | 1.8 | 1.45 | 66% | 1100 | 110 | 10% |
| Red-tail fish | Carnivore | 2.6 | 1.2 | 87% | 1800 | 135 | 30% |
| Mouse fish | Omnivore | 1 | 1.1 | 90% | 400 | 105 | 10% |
| Ferrite jellyfish | Carnivore | 0.8 | 1 | 72% | 800 | 65 | 10% |
| Wu Zhi Qi | Omnivore | 1.6 | 1.25 | 85% | 2200 | 185 | 20% |
| Star dolphin | Carnivore | 2.2 | 1.1 | 85% | 700 | 130 | 15% |
| Goldfish (金鱼) | Omnivore | 1 | 1 | 91% | 400 | 90 | 5% |
| Rahn's Nightmare | Carnivore | 1.6 | 1.3 | 80% | 1800 | 145 | 30% |
| Luo Fu Ling | Herbivore | 2.4 | 1.4 | 67% | 550 | 80 | 0% |
| Muspel's Son | Carnivore | 1.8 | 1.3 | 77% | 1600 | 140 | 25% |
| Space rocket fish | Carnivore | 2.8 | 0.9 | 88% | 1200 | 110 | 30% |
| Neolon | Omnivore | 2.6 | 1.35 | 70% | 1400 | 95 | 10% |
| Black fish | Carnivore | 1.8 | 1.1 | 92% | 2200 | 140 | 20% |

### 2.6 Water Quality & Breeding

Pollution and purification coefficients are ml per 100 g per night parameters. Breeding rate is the configured percentage after entering qualified native breeding checks; it still requires suitable adult male/female, satiety and environment, and product space. "Satiety threshold" is the proportion of the capacity the current satiety must reach; "hatch group" is the fry-group parameter for native fertilized eggs, not a fixed nightly spawn count. A breeding rate of 0 cannot auto-obtain hatchable eggs via that group parameter.

| Species | Pollution coeff | Purification coeff | Sewage sensitivity | Breeding rate | Satiety threshold | Hatch group |
|---|---|---|---|---|---|---|
| Small yellow croaker | 0.35 | 0 | 0.8 | 40% | 50% | 4 |
| Large yellow croaker | 0.65 | 0 | 1 | 32% | 60% | 3 |
| Sardine | 0.3 | 0 | 1 | 44% | 50% | 6 |
| Water-purifier fish | 0.35 | 8 | Immune | 20% | 65% | 2 |
| Volcanic fish | 0.5 | 0 | 0.45 | 24% | 60% | 3 |
| Glow fish | 0.6 | 0 | 1 | 20% | 75% | 2 |
| Glass fish | 0.3 | 0 | 1.8 | 20% | 70% | 2 |
| Great white shark | 0.12 | 0 | 1 | 0% | 100% | 1 |
| Emperor red-armor crab | 0.18 | 0 | 0.8 | 0% | 100% | 1 |
| Swordfish | 0.16 | 0 | 1.05 | 20% | 70% | 2 |
| Rainbow fish | 0.4 | 0 | 1.4 | 20% | 65% | 2 |
| Goldfish | 0.7 | 0 | 1.3 | 0% | 100% | 1 |
| Blue crystal fish | 0.35 | 0 | 1.5 | 0% | 100% | 1 |
| Red-tail fish | 0.8 | 0 | 1 | 20% | 65% | 3 |
| Mouse fish | 0.45 | 0 | Immune | 32% | 55% | 4 |
| Ferrite jellyfish | 0.25 | 0 | 1.2 | 20% | 70% | 2 |
| Wu Zhi Qi | 0.6 | 0 | 0.8 | 20% | 60% | 3 |
| Star dolphin | 0.65 | 0 | 0.65 | 24% | 65% | 3 |
| Goldfish (金鱼) | 0.4 | 0 | 0.9 | 28% | 55% | 3 |
| Rahn's Nightmare | 0.9 | 0 | 1.1 | 20% | 70% | 2 |
| Luo Fu Ling | 0.6 | 0 | 1.8 | 0% | 75% | 1 |
| Muspel's Son | 0.85 | 0 | 0.35 | 20% | 65% | 3 |
| Space rocket fish | 0.8 | 0 | 1.1 | 28% | 60% | 4 |
| Neolon | 0.65 | 0 | 1.4 | 0% | 75% | 1 |
| Black fish | 0.65 | 0 | 1.8 | 24% | 65% | 2 |

Neolon also has an independent edible-egg mechanism: nocturnal egg quality parameter 120 g, egg-load ratio 15%, per-gram egg energy parameter 2.5 kcal; full/early egg-harvest health-cost parameters 20%/30%; re-grow egg health-ratio threshold 90%. This surgical egg-harvest chain and hatchable fertilized eggs are different mechanisms; do not treat harvested edible eggs as a fry source.

## 3. Farming Markup & Profit Calculation

### 3.1 Which Growth Earns Markup

New-high weight actually produced by eating feed in the farming device enters the markup ledger. Simply placing in a tank, editing items, changing containers, or recovering previously-lost weight does not earn growth rewards again.

Fish grown from eggs can show a "farmed" source label; fish obtained at sea show "wild". The source label and the internal weight ledger are separate; new weight a wild fish genuinely farms later is still handled by the current ledger rules. Old eggs missing the new source marker cannot be inferred to have shown the farmed label just from price.

Let:

- `Main window = max(1, adult-node g − juvenile-node g)`; tail window = elder-node g − adult-node g.
- `Main extra markup = (main cap multiplier − 1) × recorded main growth / main window`.
- `Tail extra markup = (main cap multiplier − 1) × 0.5 × recorded tail growth / tail window`; 0 if no tail window.
- `Current multiplier = 1 + main extra markup + tail extra markup`.

The code floors each segment in basis points (1/10000). When neither window has earned enough new weight, you cannot apply the full multiplier.

### 3.2 Markup & Meat Price for All 23 Ordinary Fish

Main-meat base price is per kg of real meat weight, different from whole-fish base price. Display keeps 6 decimals; calculation uses the full decimals in source. The two bosses go through separate large-game rules and are not in this 23-fish price table.

| Species | Main window g | Tail window g | Main cap multiplier | Theoretical cap incl. tail | Main-meat base price /kg |
|---|---|---|---|---|---|
| Small yellow croaker | 113 | 75 | 2 | 2.5 | 85.000000 |
| Large yellow croaker | 1085 | 850 | 3.5 | 4.75 | 54.460094 |
| Sardine | 115 | 25 | 1 | 1 | 85.000000 |
| Water-purifier fish | 710 | 400 | 1 | 1 | 76.305221 |
| Volcanic fish | 890 | 500 | 1.5 | 1.75 | 42.726348 |
| Glow fish | 2430 | 1250 | 1 | 1 | 52.671181 |
| Glass fish | 230 | 150 | 1 | 1 | 85.000000 |
| Swordfish | 8700 | 5500 | 1 | 1 | 7.135307 |
| Rainbow fish | 545 | 300 | 1 | 1 | 41.176471 |
| Goldfish | 590 | 350 | 1 | 1 | 68.062827 |
| Blue crystal fish | 900 | 500 | 1 | 1 | 65.430752 |
| Red-tail fish | 1780 | 1000 | 3.4 | 4.6 | 75.112669 |
| Mouse fish | 460 | 300 | 2.4 | 3.1 | 117.525773 |
| Ferrite jellyfish | 650 | 200 | 1 | 1 | 72.445019 |
| Wu Zhi Qi | 2100 | 700 | 3 | 4 | 88.706366 |
| Star dolphin | 515 | 200 | 3.2 | 4.3 | 93.511450 |
| Goldfish (金鱼) | 270 | 150 | 1.8 | 2.2 | 157.142857 |
| Rahn's Nightmare | 1980 | 600 | 1.4 | 1.6 | 68.750000 |
| Luo Fu Ling | 1050 | 400 | 1 | 1 | 371.644295 |
| Muspel's Son | 1540 | 600 | 1.6 | 1.9 | 95.439189 |
| Space rocket fish | 1020 | 400 | 2.8 | 3.7 | 74.912892 |
| Neolon | 1560 | 600 | 1 | 1 | 265.527066 |
| Black fish | 2280 | 1600 | 4 | 5.5 | 68.750000 |

### 3.3 Live / Dead / Main-Meat Price Differences

Live fish first computes the base value from real weight and whole-fish unit price, then multiplies the earned farming multiplier; dead whole fish and dried whole fish no longer use this live-fish farming markup. Filleted main meat inherits the corresponding farming progress, source, and grade — you cannot take the marked-up live fish's total price as the main-meat base and multiply again.

The current per-cut main-meat quote core is:

`floor(cut real grams × per-species meat price × grade coefficient × current farming multiplier ÷ 1000)`.

Crystallization adds its modifier separately. Each cut is rounded separately, then summed. Fish offal is not main meat and does not auto-apply the main-meat price table.

### 3.4 Adult Live Fish Uniform Reference

The following only computes the whole-fish base quote for "adult-node weight, grade 4, no trait, unaffected by market". The right column additionally assumes the main-stage markup is fully earned, excluding tail. They are two quote scenarios, not guaranteed income for every fish farmed from egg to adult, nor filleting income or daily net profit.

| Species | Adult dev days | Adult node g | Quote without farming markup | Main-stage fully earned quote |
|---|---|---|---|---|
| Small yellow croaker | 4 | 125 | 3 | 6 |
| Large yellow croaker | 7 | 1150 | 51 | 178 |
| Sardine | 4 | 125 | 4 | 4 |
| Water-purifier fish | 14 | 800 | 72 | 72 |
| Volcanic fish | 13 | 1000 | 70 | 105 |
| Glow fish | 14 | 2750 | 151 | 151 |
| Glass fish | 12 | 250 | 120 | 120 |
| Swordfish | 15 | 9500 | 114 | 114 |
| Rainbow fish | 11 | 600 | 120 | 120 |
| Goldfish | 18 | 650 | 292 | 292 |
| Blue crystal fish | 18 | 1000 | 300 | 300 |
| Red-tail fish | 12 | 2000 | 150 | 510 |
| Mouse fish | 7 | 500 | 50 | 120 |
| Ferrite jellyfish | 11 | 800 | 64 | 64 |
| Wu Zhi Qi | 11 | 2500 | 200 | 600 |
| Star dolphin | 8 | 600 | 45 | 144 |
| Goldfish (金鱼) | 8 | 300 | 30 | 54 |
| Rahn's Nightmare | 13 | 2400 | 180 | 252 |
| Luo Fu Ling | 15 | 1200 | 420 | 420 |
| Muspel's Son | 13 | 1800 | 194 | 310 |
| Space rocket fish | 10 | 1200 | 80 | 224 |
| Neolon | 15 | 1800 | 500 | 500 |
| Black fish | 13 | 2400 | 240 | 960 |

Using black fish as an example: juvenile 120 g, adult 2400 g, main window 2280 g, main cap 4×. Upon earning 1140 g of effective new weight, the main multiplier is about 2.5×; fully earning the main stage is 4×, and fully earning the tail stage reaches 5.5×. A grade-4, no-trait, 2400 g live fish has base value 240; with a complete main-stage ledger, the formula quote is 960. Farming cost is not yet deducted.

When choosing a farming route, compare separately: target fish's adult time, water use, footprint, maintenance and growth feed, live-fish price, actual main-meat yield, by-products, and breeding. Swordfish's main markup cap is 1×, its advantage lying in meat supply, offal, and feed use; high whole-fish-unit-price species like glass fish also cannot use whole-fish per-kg price as meat price.

## 4. Fixed-Calorie Filleting & By-products

### 4.1 23-Fish Four-Tier Calorie Budget

Each ordinary biological fish currently has large/normal/small/tiny four tiers, 92 tiers total, all multiples of 100. They are the **calorie budget deducted at decomposition**, not fixed grams. Total cuttable meat takes the fish's actual meat amount: allocate and deduct from large to small in order, with a remainder reaching 25 kcal kept as offal and below 25 kcal producing none.

The same product ID can hold different cuts' actual calories, weight, and sprite; this version does not restore the old meat-combining mechanism.

| Species | Large kcal | Normal kcal | Small kcal | Tiny kcal |
|---|---|---|---|---|
| Small yellow croaker | 400 | 300 | 200 | 100 |
| Large yellow croaker | 1800 | 700 | 300 | 200 |
| Sardine | 400 | 300 | 200 | 100 |
| Water-purifier fish | 1400 | 600 | 300 | 200 |
| Volcanic fish | 2100 | 900 | 500 | 300 |
| Glow fish | 1200 | 500 | 200 | 100 |
| Glass fish | 500 | 300 | 200 | 100 |
| Swordfish | 13400 | 5400 | 2700 | 1400 |
| Rainbow fish | 1000 | 400 | 200 | 100 |
| Goldfish | 1300 | 600 | 300 | 200 |
| Blue crystal fish | 2000 | 800 | 400 | 200 |
| Red-tail fish | 2900 | 1100 | 500 | 300 |
| Mouse fish | 600 | 300 | 200 | 100 |
| Ferrite jellyfish | 900 | 400 | 200 | 100 |
| Wu Zhi Qi | 2900 | 1100 | 500 | 300 |
| Star dolphin | 700 | 300 | 200 | 100 |
| Goldfish (金鱼) | 500 | 300 | 200 | 100 |
| Rahn's Nightmare | 2600 | 1000 | 400 | 200 |
| Luo Fu Ling | 1500 | 600 | 300 | 200 |
| Muspel's Son | 2100 | 800 | 400 | 200 |
| Space rocket fish | 1300 | 500 | 200 | 100 |
| Neolon | 2300 | 900 | 400 | 200 |
| Black fish | 3300 | 1300 | 600 | 300 |

Swordfish retains an extra food compensation: the filleted meat and offal final food calories are 2× the table allocation budget, with real grams not doubled; dried swordfish also has corresponding compensation. Do not deduct the original meat amount again with the doubled food calories, nor double the farming price directly.

Weight is allocated by the proportion of cumulative allocated calories to the fish's total meat amount, with cumulative rounding; total allocation does not exceed the fish's real grams. For example, with total meat 1050 kcal and tiers 400/300/200/100, you get two 400 and one 200, with 50 as offal — not a fixed "cut one large meat" then stop.

### 4.2 12 By-product Thresholds

Each rule computes independently by whole-fish weight: `count = floor(original grams ÷ threshold)`, not by cut count, and one by-product's deducted weight is not subtracted from another threshold. Below threshold is 0 units.

| Source fish | Product | Whole-fish g per unit |
|---|---|---|
| Water-purifier fish | Purification sac | 800 |
| Volcanic fish | Heat gland | 1000 |
| Glow fish | Conductive tendon | 2750 |
| Glass fish | Transparent fish membrane | 250 |
| Swordfish | Swordfish rostrum | 9500 |
| Goldfish | Gold | 650 |
| Blue crystal fish | Ice crystal | 1000 |
| Red-tail fish | Red-tail fin | 2000 |
| Muspel's Son | Heat gland | 1800 |
| Rahn's Nightmare | Nightmare sac | 2400 |
| Neolon | Spectral wing membrane | 1800 |
| Ferrite jellyfish | Ferrite granule | 800 |

For example, 2750 g glow fish produces 1 conductive tendon by threshold, 5500 g produces 2; a 9500 g swordfish produces 1 rostrum. The fish-skin slot is zeroed in this mod's ordinary-fish route. Great white shark and Emperor red-armor crab still follow their own proportional decomposition and placement transactions — do not force the 92-tier ordinary-fish table onto them; shark meat and correctly-sourced crab meat can enter the corresponding can and worm-box processing chains.

## 5. Farming Equipment & Worms

### 5.1 Large Circulating Farming Device

- Initial interior 24×4; width and height can each be upgraded 4 times, max 28×8.
- Let width upgrades = W, height = H, efficiency = E; W/H range 0–4, E range 0–12.
- Per-night power: `max(1, 10 + 2W + 3H − 2E)`.
- Initial 10 power; full-size with no efficiency 30; full-size with 12 efficiency 6; initial-size with 5 efficiency upgrades reaches the 1-power floor.
- Standalone water tank capacity 60 L (new tank empty; old tank on load only changes cap, keeps water, does not refill), 4 filter slots. Device power, fish generation, water, and water quality settle separately — checking one item does not guarantee stable operation.

### 5.2 Worms, Stored Feed & Industrial Feed

| Item | Current value / rule |
|---|---|
| Juvenile / adult worm defined price | 10 / 15 |
| Juvenile / adult food calories | 100 / 500 kcal |
| Adult condition | 2 growth nights with feeding and space met |
| Lifespan age threshold | 12 days; unmet growth still increments age |
| Live worm nightly maintenance | 10 kcal each; individuals reaching death processing no longer count |
| Breeding | At night-start, two adult, surviving, fed worms form a pair, producing 2–4 juveniles |
| Juvenile breeding cost | 125 kcal extra each; product space must satisfy whole-group placement |
| Adult worm cutting | One worm cut into 3 juveniles; container must fit all products |
| Normal / industrial meat feed craft | Each consumes 1 live adult worm + 5000 kcal box reserve, produces one 5000 kcal feed |
| Industrial meat feed | Meat food, can feed and merge with same type, cannot split |

The box accepts worms and recognizable fish meat, offal, and other materials — not live fish in the input area. Stored food converts to box reserve in the night processing chain only when live worms are present; "already placed a piece of meat" is not "reserve instantly increased". Reserve cap 10¹² kcal.

The 5000 kcal for feed comes from built-in reserve; the adult worm is a separate consumption condition — its own 500 kcal cannot substitute for the 5000. Generation and placement failures follow existing transactions; do not judge delivery just because the button was clicked.

## 6. Fishing XP, Grade & Traits

Each collected fish grants 10 XP; max level 9500 XP. If relying solely on this collection count, you need 950 fish. The grade column is the actual player display name: grade 4, 3, 2, 1, special correspond to 1.00/1.10/1.25/1.50/2.00× price coefficients. Bred fish default to grade 4, i.e., 1×.

| Fishing level | Cumulative XP | Grade 4 | Grade 3 | Grade 2 | Grade 1 | Special |
|---|---|---|---|---|---|---|
| 0 | 0 | 100% | 0% | 0% | 0% | 0% |
| 1 | 200 | 100% | 0% | 0% | 0% | 0% |
| 2 | 500 | 90% | 10% | 0% | 0% | 0% |
| 3 | 900 | 75% | 23% | 2% | 0% | 0% |
| 4 | 1400 | 58% | 30% | 10% | 2% | 0% |
| 5 | 2050 | 40% | 33% | 20% | 6% | 1% |
| 6 | 2900 | 25% | 30% | 28% | 14% | 3% |
| 7 | 4000 | 14% | 23% | 32% | 24% | 7% |
| 8 | 5400 | 7% | 15% | 28% | 35% | 15% |
| 9 | 7200 | 3% | 9% | 20% | 40% | 28% |
| 10 | 9500 | 1% | 4% | 15% | 40% | 40% |

First-catch traits apply only once, not repeatedly amplified by reloading: Fat multiplies the then weight target by 1.25, Giant by 1.60, both capped by the species' elder weight; Scarred deducts 35% of then health; Pregnant only works for species supporting the egg mechanism, adjusting female/adult and current egg load. Crystallization adds 50% to the price layer, then grade modifier applies.

Grade is not the can's 1–5 score, nor the powder's three effect tiers. A fish's age, grade, source label, and farming markup are saved separately — seeing "grade 4" does not mean the fish is adult or has 4× farming profit.

## 7. Voyage, Gear, Bait & Ship

The four formal-voyage fishing gears all have a base hook rate of 60%, then node, bait, and sea-state modifiers apply.

The starting trait "simple fishing" still has an out-of-shop scavenging entry: with the trait and usable gear, it prefers the highest-tier carried tool and judges a fishing attempt at 50%, with the fish pool further limited by date etc. Bait changes relevant species weights, not turning this 50% into voyage probability; failure consumes the corresponding bait and continues vanilla scavenging, success replaces this product with the catch. This entry and the voyage 60% are two separate judgments.

| Gear | Defined value | Footprint | Base catch | Use difference |
|---|---:|---:|---:|---|
| Simple fishing rod | 100 | 1×4 | 1 | Entry single hook |
| Double-hook rod | 500 | 1×5 | 2 | Success = base 2 fish per cast |
| Carbon-fiber rod | 1000 | 1×6 | 1 | Expands usable fish pool |
| Fishing net | 2000 | 2×2 | 3 | Uses carbon-fiber pool, each fish drawn independently |

Table values are item definition values, not all shop transaction prices. The net's three fish are not three independent 60% checks: it first generates multiple candidates for one cast, then judges the cast's hook, then possibly enters the fight. Do not calculate this as "one fish guaranteed per cast". Carbon-fiber and net listing are also gated by Willie's fish-research storyline.

### Bait Reference Table

Double-click bait to transfer into the voyage equipment library. Normal, durable, and strong versions target the same effect, differing in durability and bonus magnitude. Success-rate bonuses are percentage points: e.g., 60% + 25 points is 85%, not 75%.

| Bait | Condition | Normal: dur/bonus | Durable: dur/bonus | Strong: dur/bonus |
|---|---|---|---|---|
| Mouse droppings ball | Blood fish shadow | 15 / +25 pts | 30 / +25 pts | 10 / +50 pts |
| Fish meat mash | Normal spots, excl. blood & giant shadows | 15 / +25 pts | 30 / +25 pts | 10 / +50 pts |
| Glazed bait | Giant fish shadow | 15 / +25 pts | 30 / +25 pts | 10 / +50 pts |
| Flash bait | Chest check after successful catch | 15 / +15 pts | 30 / +15 pts | 10 / +30 pts |

Flash bait does not increase this cast's hook rate; it raises the conditional probability after a successful hook. Suppose a case has 60% hook rate and 20% chest rate after success; the whole-cast chest probability is 12%; this 20% is only an illustration, not a confirmed chest base value. Actual per-cast bait use is "1 + location extra wear + this-cast extra wear", so 15 durability cannot be written as "usable for 15 casts". Growth-sourced durability bonus also adds on top of item base durability.

### How This-Cast Hook Rate Works

Current cast order: 60% base, add location and consecutive-catch modifiers, add applicable bait, add node/sea-state modifiers; if this cast's candidates contain any biological giant fish, subtract 15 points; finally, if the force engine is on, subtract 20 points and clamp the result to 5%–100%. This is only hook probability; fish needing a fight still must pass tension operation — hook rate is not final hold rate.

| Condition | Hook-rate modifier | Other effect |
|---|---:|---|
| Point pressure 0–2 | 0 | Fish abundant |
| Point pressure 3–5 | −25 pts | Fish startled |
| Point pressure 6+ | −50 pts | Fish sparse |
| News fish school | +10 pts | Still stacks other rules |
| Dense fish school | 0 | Base +1 fish; +2 disturbance if pressure <3, else +4 |
| Deep-sea nest | 0 | Rare weight ×3; +5 disturbance |
| Blood fish shadow | −15 pts | Weight ×1.5; +5 disturbance |
| Giant fish shadow | −35 pts | Forced giant, weight ×1.5; +8 disturbance |
| Storm | −10 pts | Weight ×1.15 again |
| Red tide | 0 | Rare weight ×1.5 |
| Migration | +5 pts | No extra weight multiplier |

Storm, red tide, and migration take effect in order — only one at a time in this segment. Disturbance reaching 75 gives weight ×1.10; reaching 90 gives rare weight ×1.5 again. "Weight ×3" is not the final rare rate tripled directly; final probability also depends on the total weight of other candidates in the pool. Hot-spring areas add 2 extra disturbance, but blood and giant nodes subsequently override node disturbance — do not unconditionally add all text. Cave areas also add 10 to trait bonus; trait bonus by disturbance: 0–24 → 0, 25–49 → 5, 50–74 → 10, 75–89 → 15, 90–100 → 20.

At an ordinary spot, no pressure and other modifiers, no giant candidate, force engine off, ordinary fish-meat mash turns 60% into 85%, strong can reach the 100% cap. If the force engine is on, the two examples become 65% and 90%. The giant shadow's −35 and the −15 after generating a giant candidate are two different steps — do not subtract only the former. Consecutive catch and specific sea areas produce extra values; the examples hold only when those modifiers are zero.

### Ship Upgrade Per-Level Prices & Effects

Level 0 is the initial state. Prices below are the one-time cost to go from the previous level to that level; the cumulative column helps budget. Hull, hold, and engine are at the dock; chart at research-station base research; silence at anomaly research in applicable natural sea areas.

| Item | Level | This cost | Cumulative | Value after |
|---|---:|---:|---:|---|
| Hull | 0 | — | 0 | Durability cap 10 |
| Hull | 1 | 120 | 120 | Durability cap 14 |
| Hull | 2 | 300 | 420 | Durability cap 18 |
| Hull | 3 | 650 | 1070 | Durability cap 22 |
| Hold | 0 | — | 0 | 6×6, 36 tiles |
| Hold | 1 | 200 | 200 | 7×6, 42 tiles |
| Hold | 2 | 400 | 600 | 8×6, 48 tiles |
| Hold | 3 | 600 | 1200 | 9×6, 54 tiles |
| Hold | 4 | 1000 | 2200 | 10×6, 60 tiles |
| Engine | 0 | — | 0 | 15 min per tile |
| Engine | 1 | 200 | 200 | 13 min per tile |
| Engine | 2 | 400 | 600 | 10 min per tile |
| Engine | 3 | 700 | 1300 | 8 min per tile |
| Engine | 4 | 1100 | 2400 | 5 min per tile |
| Chart | 0 | — | 0 | Reveal radius 2 tiles |
| Chart | 1 | 500 | 500 | Reveal radius 3 tiles |
| Chart | 2 | 1000 | 1500 | Reveal radius 4 tiles |
| Silence | 0 | — | 0 | Anomaly growth multiplier 1.0 |
| Silence | 1 | 200 | 200 | Multiplier 0.9 when on |
| Silence | 2 | 450 | 650 | Multiplier 0.8 when on |

Engine time is 15 − level × 2.5, then rounded away from zero (half away), so level 1 is 13 minutes and level 3 is 8 minutes. Hold tile count is planar area; whether long items fit still depends on actual shape and placement. Chart distance is the sum of horizontal and vertical steps, revealing a diamond; the selectable investigate distance is reveal radius + 1, capped at 5 tiles, i.e., initial 3, level-1 4, level-2 5. Normal ship movement only allows one adjacent tile up/down/left/right.

The force engine is a separate device priced 1000, not a fifth level of the permanent engine upgrade. When on, it saves 1 more minute per tile, so levels 0–4 become 14, 12, 9, 7, 4 minutes; the cost is −20 points hook rate, +20 points hunt-appearance rate, and +10 extra disturbance per hour in natural sea areas. It suits travel; turn it off before fishing to avoid the hook penalty. Silence also requires confirming the switch after purchase; when off, the level is kept but disturbance reduction does not apply.

### Scheduling Search & Risk Within 600 Minutes

The voyage time cap is 600 game minutes. With 18:00 as the voyage start, 600 minutes corresponds to 04:00 the next day. Some interface drawing code retains an old 18:00–02:00 comment; follow the 600-minute actual timing rule. Ordinary salvage resource cycle is 2 days, computed by days-since-start divided by 2 — not every return refreshing all crates.

Search charges by whole seconds of real progress: each complete search second deducts 5 game minutes. Search quality tiers 0 and 1 need 1 second (5 min); tiers 2 and 3 need 2 seconds (10 min); higher tiers need 3 seconds (15 min). Mid-progress is charged by segment — do not record every complete search as 5 minutes. The ordinary fishing interface shows 30 minutes, using the searchlight shows 25 minutes; movement, search, and other services all count toward the return budget.

Natural disturbance base growth: voyage minutes 0–239 → 0; 240–359 → +5 per 30 min; 360–479 → +8 per 30 min; 480+ → +12 per 30 min. A calm night multiplies by 0.7 first, then by the silence multiplier, then adds force-engine extra growth and the prior sub-1 remainder, flooring down; remaining decimals are kept. Disturbance clamps to 0–100. Example: late-stage 30-min base 12, level-2 silence becomes 9.6 — do not round off 0.6 each segment; accumulated remainders eventually become integer disturbance. The force engine's extra is added after the silence multiplier; do not claim silence also cuts it by 20%.

| Disturbance | Anomaly-incident base probability | Hunt-appearance base probability |
|---|---:|---:|
| 0–24 | 0% | 10% |
| 25–49 | 12% | 20% |
| 50–74 | 30% | 35% |
| 75–89 | 50% | 50% |
| 90–100 | 75% | 70% |

The two columns are different judgments, not a guarantee of two things at once. Natural voyages schedule incident checks by 30 minutes, but there are also node and event entry points; do not write it simply as "one fixed draw per tile moved". Hunt base value adds 20 points after enabling the force engine, clamped to 0–100%. Underwater-impact branch damage: disturbance <50 deducts 1, 50–89 deducts 3, 90+ deducts 5; not all incidents necessarily deduct this value.

### Tension Operation & Hunt

Each round's new tension = current tension + fish's round force + player operation force, clamped to 0–100. The three operations' forces are −25, +15, +30. The safe target zone is 30–70; the system suggestion picks the option closest to 50 among those that can land in this zone. When you see the fish surge upward, still look at current tension and actual force — do not memorize a fixed rotation.

| Fish weight | Base success targets | Base reaction time |
|---|---:|---:|
| Under 500 g | 2 | 4.5 s |
| 500–1999 g | 3 | 4 s |
| 2000–4999 g | 4 | 3.5 s |
| 5000 g+ | 5 | 3 s |

Rare or giant fish reaction time is reduced by 0.5 s, minimum 2.5 s. This is the ordinary weight rule; hunt battles set their own targets — do not apply the table to assume all bosses need only 5. Small yellow croaker and sardine still have giant-trait fight exemption; Linguan fish also use a fight threshold of per-kg base price strictly greater than 100.

On a hunt encounter, having no usable harpoon triggers a predator attack. Formally starting battle first consumes 1 harpoon durability, so failure should not be treated as a free trial. Harpoon cap 3; obtained from the ghost ship and brought back in the formal hold to enter the equipment library; each repair costs 150 at the dock for 1 point. Safe harbor clears the current encounter. A successful predator still has collection and loading steps — winning does not mean it is in your backpack.

The distress beacon auto-consumes 1 in the implemented fatal-impact rescue entry. If the hull has dropped to 0, rescue first restores it to 1, then processes a successful return, settling the hold as a normal return. This is not the same trigger as returning to port yourself, and cannot be generalized to "all failure causes auto-save cargo". Life, blood, and other keys and hidden exits have their own entries and inventory conditions; this section has not completed per-item verification of the corresponding drop and consumption chains, so it does not fabricate key drop rates, pity rounds, or hidden reward counts.

## 8. Unlocks, Research & Delivery

### 8.1 Key Unlock Table

| Item | Cost / condition | What you get or effect |
|---|---|---|
| Ship repair debt | Total 1000; can pay 100 first or clear all | First successful payment unlocks sailing; no need to clear all first |
| Shop boat key | Shows when shop is ready; prompts repair before payment | Scene entrance, not a shop-sold key item |
| Willie first visit | Story arrangement after gaining sailing eligibility | Business card and fishing terminal; phone 2424 |
| Linguan intel | 2000; debt cleared and day 10 reached | Opens the planet's location |
| Caststar intel | 2000; debt cleared, day 10, and the electronics integration enabled | Opens the mechanical planet entrance; card not shown without the integration |
| Farming system | 650; first unlock vanilla Aquaculture II | 2 fertilized black-fish roe, 5 juvenile worms; new unlock adds 1 worm box |
| Container printing | 300 | Grants 1 container printer; terminal opens repurchase |
| Reagent equipment purchase | 2000 | First-time: reagent manual, grinder, reagent synthesizer ×1, empty syringes ×4; opens later purchases |
| Cuisine operation | 1000; needs the cuisine expansion installed | The cuisine expansion provides operation and equipment; see the cuisine guide |

The farming system no longer grants the fishing terminal or printer. Old saves that already unlocked or have pending-gift records do not re-grant the whole pack for the new worm box; the actual pending record governs. Not installing the cuisine expansion does not block the fishing mod.

### 8.2 Terminal Fish File Research

Research is separate from farming-system qualification — no need to unlock farming first. You need the terminal and a sample of the corresponding species; it consumes 1 valid sample to unlock that file, with no extra "research points" currency.

Biological samples need to reach adult or elder stage and cannot be dried; mechanical samples do not use biological stage checks. Linguan/Caststar samples also require the corresponding region intel. Sample search covers the current main item areas and warehouse etc., but only checks direct items in these root areas — it does not recursively search all nested containers. Take the sample out of the small box before researching to avoid eligible fish being blocked by container hierarchy.

### 8.3 Purchase vs. Gift Difference

Equipment purchase simultaneously attempts placement in the currently-connected warehouse and main inventory etc.; actual item placement is success — do not judge fit by whether some area has empty space. Purchase failure should not deduct money or weekly count; permanent unlock packs can keep pending-gift records, to be claimed after space is freed. The two flows are not the same thing.

Farming equipment's weekly limit is recorded by the seven-day interval containing the game day, each device counted separately; it does not mean you must wait a full 168 hours after purchase. Ordinary terminal goods are purchased via plus/minus quantity, still limited by unlocked goods, balance, and actual placement.

## 9. Canning, Drying, Printing & Processing Recipes

### Permanent Qualification & Equipment

| Item | Cost / qty | Note |
|---|---:|---|
| Container printing qualification | 300 credits | Permanent unlock, plus 1 printer; keep pending if no space |
| Farming qualification | 650 credits | Grants 2 roe, 5 worms; new unlock adds a worm box; old records not re-granted |
| Additional printer | 400 credits | After printing unlock, 1 per week |
| Large farming device | 1800 credits | After farming unlock, 1 per week |
| Water tank | 300 credits | After farming unlock, 1 per week |

Weekly limits are divided by game-day ÷ 7, each device counted separately. Printing produces 1 item per craft; small empty can, large empty can, worm box, and empty syringe each consume 4 power.

### Canning

Small empty can interior 4×3, large 6×4; both process 2 days and consume the empty can. An in-progress empty can placed directly in the can rack shortens 1 extra day at sleep settlement, finishing as early as the next day; at most one acceleration per day, not retroactive for time off-rack.

Score = clamp 1–5 of [base score + suitable-fish count + seasoning count − mixed-fish penalty]. Pure whole fish base 1; with usable meat, take the highest base 2; mixed fish −1. Suitability counts by piece, not weight. At most 2 seasoning portions, and only one method. No seasoning → suitability bonus 0.

Material total = sum of all materials' native value × count + small can 5 / large can 10; finished price = material total × (1 + score ÷ 10), rounded to integer (half away from zero). Thus scores 1–5 are 1.1/1.2/1.3/1.4/1.5×.

| Method | Seasoning | Suitable species |
|---|---|---|
| Salt-cured | Salt, super salt | Vanilla graft tilapia (graft_tilapia), water-purifier fish, glass fish, rainbow fish, blue crystal fish, Luo Fu Ling, Neolon, ferrite jellyfish |
| Spicy | Chili, blazing chili | Vanilla tilapia, vanilla gutter tilapia (gutter_tilapia), black fish, mouse fish, great white shark, swordfish, Emperor red-armor crab, volcanic fish, Muspel's Son, red-tail fish, Wu Zhi Qi, Rahn's Nightmare |
| Oil-marinated | Fish oil | Vanilla sturgeon, eel, electric eel, and other species not in the two groups above |
| No suitability | — | Mixed meat |

### Drying Full Table

Yield only computes sale-purpose output; actual body weight and calories do not shrink by yield. Sale yield grams = original weight × yield, rounded and at least 1 g; base value = yield grams × per-kg unit price ÷ 1000 (integer truncation). Crystallization adds 50% first, then grade multiplier: grade 4 = 1.00, 3 = 1.10, 2 = 1.25, 1 = 1.50, special = 2.00.

| Species | Days | Sale yield | Credits/kg |
|---|---:|---:|---:|
| Swordfish (unicorn) | 4 | 70% | 15 |
| Rainbow fish | 2 | 30% | 200 |
| Water-purifier fish | 3 | 30% | 200 |
| Volcanic fish | 1 | 80% | 125 |
| Glow fish | 3 | 60% | 92 |
| Glass fish | 1 | 10% | 2400 |
| Sardine | 1 | 80% | 55 |
| Large yellow croaker | 2 | 80% | 60 |
| Small yellow croaker | 1 | 80% | 45 |
| Goldfish | 4 | 50% | 225 |
| Blue crystal fish | 4 | 45% | 150 |
| Red-tail fish | 4 | 55% | 205 |
| Mouse fish | 2 | 80% | 130 |
| Goldfish (金鱼) | 1 | 80% | 110 |
| Star dolphin | 2 | 60% | 110 |
| Space rocket fish | 2 | 70% | 70 |
| Wu Zhi Qi | 3 | 80% | 63 |
| Muspel's Son | 1 | 80% | 60 |
| Rahn's Nightmare | 3 | 40% | 95 |
| Luo Fu Ling | 4 | 50% | 480 |
| Neolon | 4 | 40% | 550 |
| Ferrite jellyfish | 2 | 30% | 180 |

### Processing Station Instant Recipes: 58 in Current Config

Place only the listed materials; extra mixed materials will not match. Quantity must fall within the min–max range; each returns the corresponding configured product; if the product carries a native stack count, the actual product governs. Advanced-bait recipes require the corresponding research qualification first.

| Product | Input materials (count) | Qualification |
|---|---|---|
| Gun part (gun_part) | `nuts_metal` ×1 + `flux_agent` ×1 | Base |
| Common electronic (common_electronic) | `wire` ×1 + `scrap_metal` ×1 | Base |
| Smelling salt (smelling_salt) | `caffeine_pill` ×1 | Base |
| Taser (tazer) | Conductive tendon ×1 + `wire` ×2 | Base |
| "Pulse Guard" stun gun (stun_gun) | `tazer` ×1 + Conductive tendon ×1 + `wire` ×1 + `common_electronic` ×1 | Base |
| Flashbang (flashbang_grenade) | Heat gland ×1 + `flux_agent` ×2 | Base |
| Restricted chemical (restricted_chemical) | Heat gland ×1 + `wine_bottle` ×1 + `flux_agent` ×1 | Base |
| Immunity injector (large_purple_injector) | Purification sac ×1 + `blood_bag` ×1 + `rubbing_alcohol` ×1 | Base |
| Surgery tool kit (surgery_tool) | Swordfish rostrum ×1 + `common_medical` ×1 + `nuts_metal` ×1 + `gun_part` ×1 | Base |
| Neuroactive perfume (neuroactive_perfume) | Transparent fish membrane ×1 + `common_chemical` ×1 + `wine_berry` ×1 | Base |
| Synapse injector (black_injector) | `blue_blood_bag` ×1 + `rubbing_alcohol` ×1 | Base |
| Eternal Revitalizer Essence (skincare_cream) | `blue_blood_bag` ×1 + Purification sac ×1 + Transparent fish membrane ×1 | Base |
| Advanced water filter (water_filter_adv) | `water_filter` ×3 + Purification sac ×3 | Base |
| Water purifier (portable_water_purifier) | Purification sac ×2 + `metal_ingot` ×1 + `common_electronic` ×1 + `wire` ×2 + `nuts_metal` ×2 | Base |
| Water quality scanner (aquascan) | `water_filter` ×1 + `water_test_container` ×1 + `common_electronic` ×1 + `scrap_metal` ×2 | Base |
| Energy cell (energy_credit) | Conductive tendon ×1 + `common_electronic` ×1 | Base |
| Energy cell Type 2 (energy_credit_ext) | `energy_credit` ×1 + Conductive tendon ×2 | Base |
| Self-charging energy cell (energy_credit_breeder) | Conductive tendon ×3 + `energy_credit_ext` ×1 + `common_electronic` ×1 + `wire` ×1 | Base |
| Advanced turbo booster (turbo_booster_adv) | `turbo_booster` ×1 + Red-tail fin ×1 + `common_electronic` ×1 + `metal_ingot` ×1 | Base |
| Stun baton (stun_baton) | `energy_credit` ×1 + Conductive tendon ×1 + `scrap_metal` ×1 | Base |
| Flashlight (flashlight) | Conductive tendon ×1 + `scrap_metal` ×1 | Base |
| Blue blood (blue_blood_bag) | Transparent fish membrane ×2 + `blood_bag` ×3 | Base |
| UV lamp (uv_filter) | Transparent fish membrane ×1 + `glass_shard` ×1 + `common_electronic` ×1 | Base |
| Flux agent (advanced_flux_agent) | Heat gland ×5 + `flux_agent` ×15 | Base |
| Nightingale salve (salve) | `rubbing_alcohol` ×1 + Fish oil ×3 | Base |
| Performance module (system_module_performance) | `system_module_corrupt` ×1 + `system_module_ruined` ×1 | Base |
| Bottled water small (small_bottled_water) | `mini_bottle` ×2 | Base |
| Bottled water standard (bottled_water) | `small_bottled_water` ×2 | Base |
| Bottled water large (large_bottled_water) | `bottled_water` ×2 | Base |
| Decompose large water (bottled_water) | `large_bottled_water` ×1 | Base |
| Decompose standard water (small_bottled_water) | `bottled_water` ×1 | Base |
| Decompose small water (mini_bottle) | `small_bottled_water` ×1 | Base |
| Scratch card (scratcher_stack) | Transparent fish membrane ×1 | Base |
| Nightmare dust (nightmare_dust) | `dream_dust` ×1 + Super salt ×1 + `caffeine_pill` ×1 | Base |
| Mouse droppings ball | `feces_small` ×5 | Base |
| Fish meat mash (small yellow croaker) | Small yellow croaker ×3 | Base |
| Fish meat mash (sardine) | Sardine ×3 | Base |
| Glazed bait | `glass_shard` ×3 | Base |
| Flash bait | `wire` ×1 + `common_electronic` ×1 | Base |
| Random material (table:material) | `junk` ×5 | Base |
| Random tool (table:tool) | `junk` ×5 + `common_electronic` ×1 | Base |
| Random medical (table:medical) | `bandage_item` ×1 + `topical_bandage_item` ×1 + `hemostatic_bandage_item` ×1 | Base |
| Random makeshift item (table:makeshift_weapon) | `glass_shard` ×5 + `scrap_metal` ×1 | Base |
| Random tier-1 module (table:t1module) | `system_module_corrupt` ×1 + `scrap_metal` ×1 | Base |
| Random tier-2 module (table:t2module) | `system_module_corrupt` ×2 + `wire` ×1 | Base |
| Wine bottle remake (wine_bottle) | `empty_beer_bottle` ×2 | Base |
| Random packaged food (table:packed_food) | `feces_small` ×10 | Base |
| Durable mouse droppings ball | Mouse droppings ball ×1 + `feces_small` ×5 | Advanced bait research |
| Strong mouse droppings ball | Mouse droppings ball ×1 + Salt ×3 | Advanced bait research |
| Durable fish meat mash | Fish meat mash ×1 + Fish oil ×1 | Advanced bait research |
| Strong fish meat mash | Fish meat mash ×1 + Heat gland ×1 | Advanced bait research |
| Durable glazed bait | Glazed bait ×1 + `glass_shard` ×3 | Advanced bait research |
| Strong glazed bait | Glazed bait ×1 + Transparent fish membrane ×1 | Advanced bait research |
| Durable flash bait | Flash bait ×1 + `common_electronic` ×1 | Advanced bait research |
| Strong flash bait | Flash bait ×1 + Conductive tendon ×1 | Advanced bait research |
| Nightmare dust (Nightmare sac) | Nightmare sac ×1 | Base |
| Neuroactive perfume (Spectral wing membrane) | Spectral wing membrane ×1 + Purification sac ×1 + `rubbing_alcohol` ×1 | Base |
| Energy cell (Ferrite granule) | Ferrite granule ×1 + `common_electronic` ×1 | Base |

Supplemental example: 2 pieces of same-route suitable meat + 1 same-method seasoning = score 2 + 2 + 1 = 5; 2 whole fish with the same config = 1 + 2 + 1 = 4. A 5-score can with material base value plus empty can = 105 yields a product rounded away from zero from 157.5 → 158. Mixing different fish deducts 1, but each piece matching the chosen method still counts suitability separately; there is no "unsuitable deducts 1 more" rule.

Using no seasoning (plain/water route) still forms a valid base score, but gets no suitability bonus; same-species meat with no seasoning is base 2. Super salt and blazing chili belong to the corresponding method's seasoning; current ordinary processing is uniformly 2 days, not restoring the old aging-based appreciation. Finished value is fixed at this batch's processing; later storage does not continue to appreciate.

The processing table keeps backtick-wrapped vanilla material IDs for accurate cross-reference with the vanilla catalog, avoiding mistranslating unverified names into another item. Random-table products are not a guaranteed named item.

## 10. Fishmeal & Reagent Full Stats

This section corresponds to the five fish powders and reagent mixing already present in source; it does not treat the later chemistry-refactor plan as current play. Values come from static source verification, not game play-testing. Here µl is the in-game component unit, 1000 µl = 1 ml; dry powder uses mg, 1000 mg = 1 g. Solids in the liquid also occupy equivalent volume — do not treat only added water as total liquid.

### Species & Five-Powder Full Routes

Below lists all 25 source routes by current Chinese fish name. The English after the powder name is the component identifier for recognizing old recipes.

| Powder | All source species |
|---|---|
| Sedative nightmare | Rahn's Nightmare, Neolon, glass fish, blue crystal fish, goldfish |
| Repair blossom | Luo Fu Ling, large yellow croaker, mouse fish, small yellow croaker, sardine, black fish |
| Stimulant ember | Space rocket fish, glow fish, volcanic fish, red-tail fish, ferrite jellyfish |
| Purification spectral | Water-purifier fish, rainbow fish, star dolphin |
| Fortify tide | Muspel's Son, Wu Zhi Qi, swordfish, goldfish, great white shark, Emperor red-armor crab |

All 25 routes come from powder-items.json. Ordinary powder yield is 250 mg dry powder per 1 g input, i.e., 25%; great white shark alone yields 10 mg per 1 g, i.e., 1%. So for the same 1 kg material, ordinary fish produce 250 g, great white shark 10 g. Do not judge powder efficiency by body size.

Power is computed by the total input grams rounded up: power = ceil(total grams ÷ 1000). 1000 g uses 1, 1001 g uses 2; not per-fish rounding. Powder preserves source species and grade. The code's low-material base value is 40 credits per 1 kg of powder, then rounded by full value; this is not the post-reagent sale formula.

After placing usable material and batteries, it auto-starts and settles in the **next overnight new-day callback**. The start code's recorded current day is already the post-overnight new day, so it does not mean opening the panel produces powder instantly during the day, nor does it wait an extra full day. When output space is insufficient, the processed batch is kept; free space then continue releasing via window open, tile change, or reload — no need to wait another night for the same batch.

### 3 g Powder, 1 ml Equivalent & Purity Are Three Things

Dry powder enters the liquid and is extracted by one-third only at the dry→liquid boundary: 3000 mg dry powder forms 1000 µl of raw-powder-equivalent component. Already-made liquid poured again, transferred, or split does not lose another two-thirds. Ordinary container extraction uses integer rawMg ÷ 3; the final 1–2 mg below 3 mg is recorded as extraction tail loss when the whole pack is consumed. When space is insufficient, only the accommodable portion is extracted, the rest of the dry powder preserved. Machine prep also checks materials, capacity, and purity — do not read this formula as "any powder converts 100% to effective ingredient".

| Powder source grade | Effective ratio in raw equivalent | 3 g powder → effective | Corresponding residue |
|---|---:|---:|---:|
| Grade 4 | 55% | 550 µl | 450 µl |
| Grade 3 | 70% | 700 µl | 300 µl |
| Grade 2 | 82% | 820 µl | 180 µl |
| Grade 1 | 92% | 920 µl | 80 µl |
| Special | 99% | 990 µl | 10 µl |

Effective amount = floor(raw equivalent µl × grade percentage ÷ 100); the rest becomes gofishing_residue. Purity = sum of five-powder effective ÷ (sum of five-powder effective + impurities); water and legal adjuvants do not enter this denominator. So adding water lowers concentration but does not wash 55% purity into 99%. Native fish read as grade 4 through this path; owned fish read their actual grade.

Example: 3 g grade-4 repair powder plus 9 ml clean water yields a final 10 ml liquid with 550 µl repair effective, 450 µl residue, 9000 µl water. Effective concentration 5.5%, repair score 22, purity still 55%. Switching to special, same powder and water, repair score 39.6, displayed as 40; do not write "1 ml raw equivalent" directly as "1 ml effective repair".

### Full Concentration Curve of Effects

Each fish powder scores independently — no "main powder" gets an extra multiplier. Primary and secondary effects are the result of concentration sorting. Uniformly normalized to per 10 ml liquid: x = that effective component µl ÷ total liquid µl × 10000.

| Per-10ml effective x | Effect score |
|---|---|
| 0–1000 µl | 0.04x |
| 1000–1500 µl | 40 + 0.05×(x−1000) |
| 1500–2500 µl | 65 + 0.02×(x−1500) |
| Over 2500 µl | 85 cap |

Final score clamps to 0–100 and rounds. Tiers: below 20 no effect; 20–39 first tier; 40–64 second; 65+ third. The theoretical continuous curve's 20/40/65/85 points correspond to 5%/10%/15%/25% effective concentration; displayed rounding may shift boundary crossing slightly early. Concentration hint uses the highest effective concentration: below 10% weak, 10%–25% moderate, over 25% too strong. Other components can independently form second and third effects, but mixing more kinds does not raise a single item's cap.

Example: to reach 65 repair in 10 ml liquid, you need 1500 µl effective repair. Grade 4 needs at least ceil(1500 ÷ 0.55) = 2728 µl raw equivalent, i.e., 8.184 g dry powder, effective 1500 µl, residue 1228 µl, leaving 7272 µl for water and adjuvants. Special needs 1516 µl raw equivalent, i.e., 4.548 g powder, effective 1500 µl, residue 16 µl, leaving 8484 µl. This is integer-arithmetic result — do not simply multiply required effective by 3 as the actual weighing.

Adding water does not destroy existing residue; adding adjuvants also occupies volume and may dilute a liquid that is exactly 65 down to 64. When preparing a target recipe, include water, effective components, residue, and adjuvants in the final total.

### Inherent Side Effects, Conflicts & Stability

Initial stability 92. Single-powder side-effect anchors at effect 20/40/65 are below, with linear interpolation between anchors; after 65 this part does not grow.

| Powder effect | Side-effect values at 20 / 40 / 65 |
|---|---|
| Repair | Fatigue 10 / 25 / 40 |
| Fortify | Thirst 10 / 25 / 40; Fatigue 5 / 10 / 20 |
| Stimulant | Agitation 8 / 20 / 35; Insomnia 5 / 15 / 30 |
| Sedative | Drowsiness 10 / 25 / 40 |
| Purification | Nausea 10 / 25 / 40 |

When two or more effects form, in descending score order, every effect after the first adds extra nausea: at effect 20/40/65 → 8/12/16 points; stability drops by 30% of this nausea. When stimulant and sedative both reach the effective threshold, conflict is computed by the lower score: extra fatigue 10/25/40, agitation 20/40/55, stability drop 5/10/15.

Over 25% effective concentration has no higher positive effect. Define over-concentration O = sum over all powders of max(0, x−2500) ÷ 180, then extra fatigue min(40, O), nausea min(25, 0.5O), stability drop min(45, 1.2O). Residue also adds nausea by "residue volume fraction × 70"; unknown pollutants add their volume fraction × 250, with same-item pollution burden capped at 70. Higher grade saves dry powder and also reduces this pollution burden that adjuvants cannot directly erase.

### Adjuvant Micro-Dosage & Combinations

Adjuvants are computed by actual effective µl per 10 ml liquid; one group's dosage follows.

| Adjuvant | Per-group base µl | Unassigned-excess scale µl |
|---|---:|---:|
| Dream-cap extract dream_cap_extract | 20 | 40 |
| Protein protein | 150 | 300 |
| Mineral mineral | 60 | 120 |
| Stabilizer stabilizing_agent | 20 | 40 |
| Caffeine caffeine | 30 | 60 |
| Sugar sugar | 150 | 300 |

| Target side effect | Required group |
|---|---|
| Fatigue | Sugar 150 + Protein 150 |
| Thirst | Sugar 150 + Mineral 60 |
| Agitation | Dream-cap 20 + Mineral 60 |
| Insomnia | Dream-cap 20 + Protein 150 |
| Drowsiness | Caffeine 30 + Sugar 150 |
| Nausea | Protein 150 + Mineral 60 |

One group reduces treatable burden 50%, two groups 80%, three groups 90%; below one group follows continuous ratio. The same sugar cannot fully serve fatigue, thirst, and drowsiness simultaneously; the system allocates by specified priority then by side-effect magnitude, up to three rounds. Pollution nausea and some conflict base values are protected — do not promise three adjuvant groups eliminate all side effects.

The stabilizer specifically handles mixing and stimulant-sedative conflict: 20 µl per group; 1/2/3 groups reduce mixing burden 50%/80%/90%, conflict up to 80%, and restore corresponding stability loss; with a single powder and no mixing conflict, the stabilizer has no such use.

Unassigned adjuvants produce extra burden. Let e = unused amount ÷ table scale; penalty = min(80, 22e), stability drop min(35, 10e). Caffeine adds agitation and half of insomnia; dream-cap adds drowsiness; mineral adds thirst; other defined adjuvants add nausea. The scale is not a "below this is fully safe" threshold, nor a usable-dose cap. Auto-balancing searches for improvement within existing inventory and capacity, considering dilution — it does not fabricate water or powder for the player.

### Capacity, Splitting & Historical Recipes

Current mixing working capacity cap is 1000 ml; powder input 0–999 g at 0.001 g precision, water 0–1000 ml at 0.001 ml precision. Split into 1–50 syringes, each consuming 1 power and 1 empty syringe. The 10 ml here is the normalization base; actual syringe capacity is determined by total batch liquid ÷ syringe count — do not assume each syringe is fixed 10 ml.

Splitting evenly divides the whole batch, keeping no in-batch remainder. When integer µl cannot divide evenly, the first several syringes each get 1 µl more; each syringe extracts by original ratio, impurities included. Example: 100001 µl into 10 syringes — the first 10001 µl, the rest 10000 µl; total 10 power, 10 empty syringes. Changing count mainly changes per-syringe volume, not concentration. Insufficient output space, battery, or syringes blocks completion; transaction exceptions have a rollback path.

Finished packaging value takes the actual consumed empty syringes' unit value; you cannot derive a forever-fixed syringe credit cost from here. Reagent value is additionally computed by effect tier, legally sellable volume, side effects, and stability — not simply reimbursing all fish-powder and water costs. Purity goes through the native quality-price path separately. Poor side effects and stability take the weaker quality factor; cheap dilution cannot infinitely amplify profit.

Historical recipes can be named and saved, up to 24 characters; the name does not determine actual effect. Reuse scales by target total; target >0 and at most 1000 ml at 0.001 ml precision, with integer remainder allocated by largest-remainder method. The machine compares target components with the current batch's components, only topping up the shortfall; insufficient powder converts to actual-grade required powder and brings in residue. Old recipes' text may show "g" as effective component ×3 — understand it with grade and actual prep preview, not as a weighing promise valid for any purity. Discarding current liquid does not return materials, but keeps adjuvant inventory and recorded recipes.

### Reagent Base Valuation Supplement

The reagent's service-value core is:

`legally sellable volume ml ÷ 10 × 100 × combined effect weight × min(side-effect coefficient, stability coefficient)`, plus the actually-consumed syringes' packaging value. Material cost is not reimbursed as input; residue and pollutants are not counted in sellable volume. Native purity-trait price is handled once more.

Single-item scores below 20 / 20–39 / 40–64 / 65+ have tier weights 0/1/2/3. Effects in descending score order get sort weights 1, 0.35, 0.15, 0.05, 0.05; summed, they cannot exceed 1.6× the highest single-tier weight.

| Side-effect total burden | 0 | 20 | 40 | 60 | 80+ |
|---|---|---|---|---|---|
| Side-effect value coefficient | 1.00 | 0.85 | 0.65 | 0.40 | 0.15 |
| Stability | 0 | 20 | 40 | 60 | 80–100 |
|---|---|---|---|---|---|
| Stability value coefficient | 0.10 | 0.25 | 0.50 | 0.85 | 1.00 |

Linear interpolation between table nodes. Side effects take the post-synthesis burden, not an unconditional sum of all interface numbers; the product still passes native price traits and rounding. For example, 10 ml sellable liquid, single item at third tier, both quality coefficients 1, service base value 300, then syringes and later price modifiers — not that all "10 ml tier-3 reagents" are fixed at 300.

## 11. Calculation Basis & Version Sources

### Check Profit in This Order

1. First determine whether it is a whole live fish, corpse, main meat, offal, dried product, or can; these are not the same price base.
2. Read the current real grams and remaining calories; do not guess actual meat by old sprite size.
3. Read grade, source, earned farming progress, and crystallization separately; count each only once.
4. Fillet rounds per cut; by-products divide by whole weight separately; cans count suitability by input pieces.
5. Finally deduct feed, battery, consumables, equipment, time, and space costs for net profit. Different species do not need to pursue the same daily profit.

### Version Basis

This table is read from the current project; specific data sources follow. Links are for players and maintainers needing to verify numbers.

| Scope | Config / implementation entry |
|---|---|
| 33 fish, 92 meat tiers, 25 physiology files, 12 by-products | [Fish catalog](../../content/fish/fish.json), [physiology params](../../content/fish/profiles.json), [by-products](../../content/fish/products.json), [base sprite sizes](../../content/fish/sprites.json) |
| Farming markup & main-meat price | [Markup calc](../../src/Logic/Aquaculture/NativeFishFarmCore.cs), [meat price table](../../src/Logic/Aquaculture/NativeFishMeatPrices.cs) |
| Weight, cutting & swordfish compensation | [Cut budget](../../src/Logic/Items/Fish/FishHarvestRules.cs), [actual allocation](../../src/Mod/Items/Fish/FishHarvest.cs) |
| 58 instant recipes | [Processing config](../../content/gameplay-metadata.json) |
| Drying | [Drying config](../../assets/fish_processing.json) |
| XP & grade | [Grade rules](../../src/Mod/Fishing/FishingQualityRuntime.cs) |
| Fish powder sources | [Powder definitions](../../content/items/powder-items.json) |
| Effect, splitting & value | [Reagent rules dir](../../src/Logic/Processing/Reagents/) |

Stage naming, native weight pricing, age and maintenance settlement are also cross-checked against the current game's Assembly-CSharp. After vanilla, integration mods, or later config updates, re-verify tables against the new version.

This version does not itemize all hidden-key drop chains, all boss-battle-specific targets, native upgrade consumable prices, or reagent actual medical duration; these unverified contents are not filled in as guessed numbers. For ordinary operation and flow, pair with the mechanics guide.
