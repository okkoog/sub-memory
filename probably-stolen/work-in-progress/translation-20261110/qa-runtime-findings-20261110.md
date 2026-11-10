# 한국어팩 2026-11-10 인게임 검수 후속 — 배포 전 처리 상태

## 1. Fishing Cuisine Expansion (3830031787) — 수정 완료

- 사용자가 제공한 `Player(20261110-102807).log`에 `pskb__3830031787.csv line 663: no comma separator, line skipped`가 보고됨.
- 기존 `fce_0b0a364ec14394518dcfc639c8aa37558cd6baaeb0962c65a506b0dedb14f230`의 값에서 문장과 `평판: {0}` 사이에 **실제 줄바꿈**이 들어가, 줄 단위 로더가 뒷줄을 독립 CSV 레코드로 읽다가 건너뜀.
- 해결: 해당 필드 안의 실제 줄바꿈을 다른 항목과 같은 **문자 `\\n`** (백슬래시+알파벳 n)으로 정규화.
- 변경 후: `K|` 664개 그대로, 663번째 물리적 줄에 독립된 `평판: {0}` 줄 없음, 원본 키와 `{0}` 보존. GitHub 수정 커밋 `c34c65659333a1c04a76dcf1be774bafe7e79a2a`.
- 11개 한국어 CSV 배포 전 묶음 ZIP을 이 수정본 기준으로 **갱신**: `KoreanPack_20261110_QA_11CSVs.zip`, 커밋 `b5a58baf66f885ea3dde3e92bc6a3d406651cd12`.
- **인게임 재실행 검증은 아직 하지 않음**. 수정된 CSV로 기동한 다음 동일 경고가 사라졌는지 확인해야 함.

## 2. Neon Undertow: Vanilla Overhaul (3833095213) — 영어 UI 번역 원인 확인, 적용 대기

- 사용자 화면: `AFTER HOURS · THE STACK`, `Current floor`, `Lift stops`, `Underworks`, `Lower Exchange`, `Midline Commons`, `Upper Crown`, `Directory`, `East market lane`, `Return home` 등 새 야간 지역 화면 대부분 영어.
- 동일 실행 로그: `Neon Undertow: Vanilla Overhaul v0.5.15.1` 정상 초기화, `[KoreanPack] Applied key translations: water_seller_english_collection` 확인, `neon_undertow_afterhours_stack` 지명 등장.
- 모드 원본 `Localization/en.csv` 및 `zh.csv`에서 위 UI 문구 및 `THE STACK`에 대응하는 로컬라이제이션 레코드 발견되지 않음. 원본 한국어팩 CSV에도 위 UI 키 없음. 이 화면의 문자열이 **공식 모드 Localization 키로 노출되지 않은 상태**라는 판단.
- 사용자 고정 제약: 타 모드 DLL·파일 수정 금지, 타 모드 DLL 후킹 금지, 전역 UI 런타임 문자열 대체 금지.
- 따라서 임의 `K|` 키를 추가해 번역되었다고 보고하지 않음. 해당 화면에서 실제 사용하는 출력 경로를 모드 소스/모더가 확인하여 `ModHelper.GetLocalized(modId, key)` 등 정식 API에 연결한 후 한국어팩에 동일 키 번역을 추가해야 함. 원래 모드의 사용자가 제공하지 않은 DLL을 수정하지 않음.
- **미완료**: 새 야간 지역 UI 번역은 아직 영어. 모더 대응 또는 공식 게임 API 연동 경로 확인이 선행되어야 함.
- 로그의 `[CustomUI] No sprite found for key 'neon_undertow_afterhours_stack'` 등 리소스 경고는 별개이며 사용자가 실제 플레이 결과 UI 이동 기능은 작동한다고 확인함. UI 번역 불가의 직접 원인으로 단정하지 않음.

## 배포 상태

- CSV 잘못된 물리 줄바꿈: 수정·GitHub 반영 완료, **실기동 재확인 필요**.
- Neon After Hours 영어 UI: **미수정 / 원본 번역 키 등록 대기**.
- 게임 실행·Steam Workshop 업데이트는 이번 작업에서 진행하지 않음.
