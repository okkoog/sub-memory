# CorpPresence 1.0 Korean Translation — Independent QA Pass 2

Status: FINAL AFTER INDEPENDENT RE-AUDIT

Source template: `probably-stolen/mods/3828913695/TranslationExamples/es.template.json`
Final Korean pack: `probably-stolen/work-in-progress/corp-1.0-ko/ko.json`
Synced candidate: `probably-stolen/work-in-progress/corp-1.0-ko/ko.candidate.json`

## Why this pass exists
The first completion judgment was too early. A second audit was performed from the finished JSON rather than trusting the first QA result. This pass found real semantic and terminology issues and changed 27 translation keys.

## Coverage rechecked
- All 1,621 source keys vs Korean keys.
- All character dialogue for Doreen, Isa, Mentor, Milo, Rosen and Seline.
- All 85 item description/effect/manual entries.
- All 68 item flavor entries.
- All story-guide requirement entries.
- All settings.init entries and matching legacyDisplay duplicates.
- Company/service, Corporate Malice/Resistance, production and UI transaction semantics.
- Fixed numbers, written-number quantities, comparison operators, deadlines, limits and prices.
- Proper nouns and recurring terminology.
- Ambiguous strings cross-checked against the official French and Russian 1.0 packs.

## Important corrections found only in pass 2
- `ui.malice.envoy.payOffZacchaeus`: “합의금 지급” → “전액 상환”.
- `items.definitions.lowerLevelsRepresentative.name`: “하층구역 대표 증서” → “하층구역 대표”; the source names the representative status, not a certificate.
- `items.definitions.isomerCarapaceFull.name`: “완전체” → “전체”; the source means a whole carapace.
- `companies.companyVisitors.labBuyer.purchaseOrderLabPutItems`: corrected to “연구소 매입 주문서”.
- `companies.carmichael.frontDeskDone.loggedOurCourierDeliversTomorrow`: removed the unsupported “전용” meaning from “our courier”.
- Special request/application strings were corrected from “특별 주문” to “특별 신청/요청”.
- Order-limit wording was corrected from “구매 한도” to “주문 한도”.
- Carmicheal batch/lot terms were normalized to “제조분”.
- Lab terminology was normalized to “연구소”.
- Forged Billet terminology in manuals was normalized to the visible item name “단조 소재”.
- Pastel dealer terminology was normalized to “제휴 판매점”.
- Recurring period/quota terminology was normalized to “주기” where it is the same game cycle.
- Underground-buyer early-booking surcharge now explicitly states that the surcharge is support points.
- Packaging-machine default throughput/power wording was clarified to “하룻밤 최대 2갑 / 작업당 전력 8”.
- Firearm refurbishment price wording was clarified to “1정에 …₵”.
- Several awkward but semantically correct lines were polished without changing mechanics.

## Final validation
- schemaVersion: 1
- locale: ko
- name: 한국어
- Source keys: 1,621
- Korean keys: 1,621
- Missing keys: 0
- Extra keys: 0
- Blank translations: 0
- Placeholder mismatches: 0
- Brace/format-token mismatches: 0
- Rich-text tag mismatches: 0
- Newline-structure mismatches: 0
- Fixed numeric-value omissions: 0
- Proper-name mismatch flags: 0
- Chinese remnants: 0
- Lowercase `ml` unit remnants: 0
- Known obsolete terminology remnants: 0
- Exact English values left intentionally: `A.L.I.S.O.N.` and the parameter-only format string `{0} {1}: {2}/{3}`.
- Duplicate English-source translations that differ: only three intentional context/character-voice cases (“I'm in.”, “Take care.”, “What do you say?”).

Final Git blob SHA: `641dbe79ad7f694247976bb1edf13869c9e40775`

This report supersedes the first completion judgment.
