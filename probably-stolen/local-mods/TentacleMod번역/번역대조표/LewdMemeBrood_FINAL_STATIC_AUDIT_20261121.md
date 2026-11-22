# LewdMemeBrood.dll — DLL 정적 검수 기록

- 원본 DLL blob: `ba545e9e7bfe1b77e4387fb68fcc17f588d3ffab` (303104바이트)
- 번역 DLL: `probably-stolen/local-mods/TentacleMod번역/lewdMemeBrood v0331/LewdMemeBrood.dll`, 516096바이트
- 번역: 1차 85개, 2차 105개, 3차 100개, 합계 **290개** / 원본 활성 중국어 306개
- 2026-11-22 재검수: 기존 원문 유지 16개 중 자체 검사 결과 표제 **5개는 실제 `Check.That`의 출력 인자**임이 IL 호출부로 확인되어 한국어 번역 반영. **현재 기능 보호 원문 11개**, `use.红` 내부 분기명 1개는 유지.
- CSV 566키 및 JSON 대사는 별도 작업 결과물. DLL 자체 한국어화 수치에 포함하지 않음.
- 정적 구조 검사는 완료했지만 실제 CLR/Unity 로드, 화면, 저장 기능 검사는 **미실시**.

## 기능상 원문 보존 항목 (2026-11-22 현재)

- `萝莉`
- `少女`
- `御姐`
- `少妇`
- `熟女`
- `贫乳`
- `普乳`
- `巨乳`
- `白`
- `紫`
- `侵蚀`


## 2026-11-22 — 5개 표제 IL 검증 및 번역

- `萝莉 → 少女` → `로리 → 소녀`
- `少女 → 御姐` → `소녀 → 누님`
- `御姐 → 少妇` → `누님 → 새댁`
- `少妇 → 熟女` → `새댁 → 숙녀`
- `熟女为止` → `숙녀까지`
- 5개 모두 `BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_0()`에서 `Check.That(Boolean, String)`의 두 번째 인자로 전달됨을 확인. 체형 일치 비교에는 `萝莉`, `少女`, `御姐`, `少妇`, `熟女` 등 별도 내부 문자열이 사용되므로 그 값은 보존.
- 수정 DLL은 dnlib로 재작성; 표시 문자열 5개 각각 1개로 교체, 옛 표시문구 0개, 핵심 내부 비교 문자열 잔존 확인. 기존에 직접 번역한 290개 및 외부 CSV/JSON은 별도 변동 대상으로 분류하지 않음.
- 전용 정적 검수 문서: `Brood_STAGE_5_IL_CALLSITE_AUDIT_20261122.md`, `Brood_STAGE_5_KO_PATCH_AUDIT_20261122.md`.
- 신규 최종 ZIP: `ProbablyStolen_Brood_v0.3.31_KO_Final_DLL_JSON_20261122.zip` (번역 DLL·135개 대사 JSON·ko.csv·설치 안내·검수 보고). Github Actions `44969985828` 성공. 파일 재작성 시 크기 516,096→304,640바이트; 실제 게임·CLR 실행 검증 미실시.
