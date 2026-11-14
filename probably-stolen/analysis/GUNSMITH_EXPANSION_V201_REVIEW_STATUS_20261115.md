# Gunsmith Expansion v2.0.1 — 468개 원문 한국어 검수 (2026-11-15)

## 기준
- GitHub `probably-stolen/mods/3834619049/Localization/zh.csv` / `probably-stolen/mods/3834619049/Localization/en.csv` / `probably-stolen/번역 모드/Localization/pskb__3834619049.csv`, 각 468개 키 (ID 3834619049, manifest 2.0.1).
- 한국어 468개 키를 중국어 원문 우선으로 확인하고 영어를 보조 참고함. Probably Translated 번역팩도 같은 키를 가진 참고자료이며 **문장을 가져오지 않고, 해석 차이만 참고**함.
- 총기/부품 수치와 제작 조건, NPC 벡스의 대사, 주문서·고객·UI, FR1-ES(프라이즈)의 상황별 대사를 확인.

## 결과
- **이번 검수 468개 중 19개 수정, 449개 유지.** 문맥 및 원문에 직접 근거가 없는 취향 차이는 수정하지 않음.
- 주요 교정: `fries_client_5` 안전장치를 못 푼다 → 거는 것도 잊는다; `gsx_press_45_desc` 탄약 → 탄피; `vex_phone_raid` 수색 → 무기고 포위; `gsx_crate_parts_desc` 희귀 부착물 1개 추가 확률 명시; `guide_5_buyers` 3일/5일 각각 1명 방문; `shop_vex_board_pages` 원문에 없는 3페이지 고정 번호 제거; 벡스 반말과 동일 모드 용어 정합성 수정.
- 상세 키·원문·전후·수정 이유: `probably-stolen/analysis/GUNSMITH_EXPANSION_V201_SOURCE_REVIEW_468_20261115.csv`.
- 검수 기록에서 449개 유지 키의 판단 근거를 개별로 꾸며 쓰지 않음. 수정 근거가 확정된 19개에만 구체적 기록을 남김.
- 변경 전후 변수·태그·줄바꿈 및 수치 확인. 숫자가 바뀐 두 항목은 원문에 있는 추가 부착물 1개를 복원하거나 원문에 없는 고정 페이지 번호를 제거한 의도된 변경.

## 미실행 검증
- 게임 실행을 통한 실제 UI 잘림, 벡스 방문·판매 조건, FR1-ES 동적 대사 타이밍은 확인하지 못함.
- 스팀과 원본 DLL 미수정.
