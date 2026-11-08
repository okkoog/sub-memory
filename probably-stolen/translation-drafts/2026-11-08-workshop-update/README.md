# Probably Stolen 워크샵 업데이트 번역 초안 — 2026-11-08

기준:
- 워크샵 갱신 커밋: `7684f72179fb9eaf37baf79c3086faedca9e010e`
- 비교 기준 직전 커밋: `104f11de5a81ef9e8db0398b7a8962648994d5ca`
- 실제 배포용 `probably-stolen/0.8.3/Localization`은 아직 수정하지 않음.
- 이 폴더는 번역 검수 및 병합 전 초안 보관용.

## 번역 완료

| 워크샵 ID | 모드 | 버전 변화 | 이번 업데이트 신규 번역키 | 초안 파일 |
|---|---|---:|---:|---|
| 3828910575 | 숙련도 시스템 | 3.0.2 → 3.0.5 | 5 | `3828910575_proficiency.csv` |
| 3828971731 | Faction Reputation Expansion | 1.4.6 → 1.5.0 | 12 | `3828971731_faction_reputation.csv` |
| 3829730904 | Brewing Expansion | 3.6.7.8 → 3.6.8 | 32 | `3829730904_brewing.csv` |
| 3832840208 | Tobacco Expansion | 0.5.3.7 → 0.5.3.8 | 4 | `3832840208_tobacco.csv` |
| 3833095213 | Neon Undertow: Vanilla Overhaul | 0.5.3 → 0.5.9 | 210 | UI + Velvet 대사 2개 파일 |

업데이트로 새로 생긴 번역키 합계: **263개**

Neon Undertow는 검수 편의를 위해 다음처럼 분리함.
- `3833095213_neon_undertow_ui.csv` — UI, 작업실, 스테이션, 세력 손님 문구
- `3833095213_neon_undertow_velvet_01_25.csv` — Velvet 방문객 01~25 신규 대사
- `3833095213_neon_undertow_velvet_26_50.csv` — Velvet 방문객 26~50 신규 대사

Neon Undertow 신규 210키는 원본 `en.csv`와 자동 대조하여 **누락 0 / 추가 오염 0** 확인.

## 기존 누락 보충

`3829085054_itemstorage_existing_missing.csv`

스택 수납 1.4.0 자체의 신규 번역키는 없지만, 기존 한국어 `ko.csv`에 빠져 있던 **13개 키**를 별도 보충 초안으로 작성함.
- 소진된 물품 우선 설정 9키
- 다음 물품 상태 툴팁 4키

## 새 번역이 필요하지 않은 업데이트

- 3828925546 SelfLearning: 1.1.0 → 1.1.1, Localization 키 변화 없음.
- 3831429588 Hydroponic Auto Replant: 0.1.0 → 0.2.0, 신규 키 없음. 오히려 아래 3키가 원본에서 삭제됨.
  - `hydro_auto_replant.settings`
  - `hydro_auto_replant.toggle`
  - `hydro_auto_replant.toggle_hint`
- 3831752512 Dongfang Music: 2.0.0 → 2.1.0, Localization 파일 기준 신규 키 없음.
- 3832941698 AiEmporium: 1.3.25 → 1.3.26, Localization 파일 기준 신규 키 없음.

## 별도 확인 대상

Localization 원본 CSV가 없어 단순 키 대조만으로 판정할 수 없는 업데이트:
- 3828913695 CorpPresence: 0.9.8fix3 → 0.9.9
- 3829963738 Lower Dialogue: 1.0.2 → 1.0.3
- 3830874694 Pawnshop Billboards: 1.0.34 → 1.0.38

이들은 DLL/런타임 문자열 경로를 별도로 감사해야 함.

## 번역 원칙

- 기존 한국어팩 용어를 우선 유지.
- 자리표시자(`{0}`, `{1}` 등)는 원문 그대로 보존.
- Neon Undertow Velvet 인물 대사는 기존 번역과 동일하게 자연스러운 존댓말 기반으로 정리.
- 농담, 말장난, 회상 장면은 문장 구조를 억지로 직역하지 않고 의미와 화자의 분위기를 살리는 방향으로 번역.
- 실제 배포 CSV에 병합하기 전 사용자 검수 대상으로 취급.
