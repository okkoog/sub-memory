# CorpPresence 1.0 Korean Translation — Independent QA Pass 2

Status: IN PROGRESS
Target: `probably-stolen/work-in-progress/corp-1.0-ko/ko.json`
Source: `probably-stolen/mods/3828913695/TranslationExamples/es.template.json`

## Rechecked so far
- 1621-key structure and metadata
- placeholder / brace / rich-text / newline structure
- fixed-number and comparison-condition semantics
- all character dialogue (Doreen, Isa, Mentor, Milo, Rosen, Seline)
- all 85 item description/effect/manual entries
- all story-guide requirement entries
- settings.init entries and 42 matching legacyDisplay pairs
- terminology variant scan
- selected ambiguous Malice wording cross-checked against official fr/ru examples

## New findings not yet applied
1. `ui.malice.envoy.payOffZacchaeus`: "합의금 지급" changes meaning. Should be repayment/payment wording.
2. Three Carmicheal/Solana lab contexts use "연구실" while the same system uses "연구소"; normalize.
3. User-facing item is "단조 소재", but two manuals still say "단조 빌렛"; normalize.
4. `companies.services.joinDealer.alreadyDealer` says "협력 판매점" while Pastel dealer terminology is consistently "제휴 판매점".
5. Generic `period` wording is mixed between 기간/주기 in recurring allocation/quota contexts; normalize where it is the same game period concept.
6. `items.definitions.oilflowerSeeds.description`: "약 6일 밤" is awkward Korean; rewrite without changing 6-night timing.
7. `items.definitions.packagingMachine.description`: "한밤에 두 갑" is awkward; rewrite more naturally while preserving default 2-pack/night and 8-charge behavior.
8. `ui.malice.fence.bookUndergroundBuyerTomorrowTakes`: early-booking surcharge should explicitly say support points for clarity.

No modifications have been applied during this second pass yet. Continue checking transaction directionality and remaining high-risk UI before patching.
