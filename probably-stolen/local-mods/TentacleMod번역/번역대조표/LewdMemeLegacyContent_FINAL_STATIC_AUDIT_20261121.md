# LewdMemeLegacyContent.dll 정적 번역 검수 (2026-11-21)

- 시험 DLL의 한자 포함 활성 문자열 35개 중 **19개(IL 22곳)** 사용자 표시 호칭·야간 보고·툴팁·문서 이름을 한국어로 반영.
- **16개 원문 유지:** `ResolveBodyId`, `ResolveBreastId`, `ResolveFactionId` 및 `Apply`의 체형·세력 키/입력값. `萝莉`·`少女` 등은 사용자가 설명한 성인 역할/체형 분류명으로 인식하며 원문 데이터 매칭을 위해 변경하지 않음.
- 포맷 변수 `{dominance}`, `{arousal}`, `{lactation}`, 숫자 자리표시자 보존. 기존 #US·기타 CLI 메타데이터 동일, 새 UTF-16LE 19건과 활성 IL 22곳 확인.
- 누적 LegacyContent **64개 문자열 / IL 68곳**. 전체 8개 시험 DLL 누적 **2739개 / IL 3162곳**.
- 원본 `TentacleMod/` 미수정, 게임/CLR 로딩/세이브·UI 검증은 미실시. 전수 목록 `LewdMemeLegacyContent_remaining_inventory_20261121.tsv`.
