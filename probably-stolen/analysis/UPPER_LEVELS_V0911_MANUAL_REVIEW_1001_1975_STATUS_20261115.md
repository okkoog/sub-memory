# Upper Levels v0.9.11 — 1001~1975번 한국어 원문 대조 (2026-11-15)

## 범위 및 결과
- ID 3838944803, 전체 한국어·중국어·영어 각 1975개 키.
- 이번 차수: 1001~1975번 975개 키를 순서대로 원문 대조. **34개 수정 / 941개 유지**. 기존 1~1000번 CSV 값은 그대로 유지.
- 중문 원문을 주 기준으로 실제 문장 뜻과 기능/수치/대사·지문을 살피고 필요 시 영어를 참고. 결과표 `probably-stolen/analysis/UPPER_LEVELS_V0911_MANUAL_REVIEW_1001_1975_20261115.csv`에는 구간의 975개 키 전부와 수정 전후 값·판정을 보관. 실제 수정한 키에는 사유를 기재하고 유지 판정에는 근거를 꾸며 적지 않음.

## 대표 교정
- `navy.order.desc/desc_any/short/short_fleet`: 물품명 {1}, 수량 {0} 순서 복원.
- `navy.status.totals`: 음수도 가능한 경매 누적 손익을 순수익으로 잘못 표시한 문제 수정.
- `appraiser.guide.prov`: 소장 기록의 ×{0}%는 곱셈 배율로 표기.
- `perk.ul_security.desc`: 밤의 강도 방지를 모든 도난 방지로 설명했던 문제 수정.
- `item_ul_perfume_accord_flavor`: 원문에 없는 15분 주기·별명 삭제.
- `item_ul_perfume_master_flavor`: 다음 제조분에서도 동일 향을 낼 수 없다는 원문 복원.
- `item_ul_lightfield_bench_flavor`: 장비 숙련자의 희소성 때문에 복원·인증이 비싸다는 이유 복원.
- `scenario.perfume.desc`: 모두 합성품이라고 단정한 표현을 대부분 출력 향수로 교정.
- `clinic.intro.1`: 백탑 진료소/Clinic과 별도의 백탑 사립 병원/Private Hospital 구분.
- `crate.scenario.title`: 상자 수집가 대신 상자 개봉광으로 역할 반영.
- `apguide.p6.body`: 400년 산화 흔적 등 중국어 원문에 없는 설명 제거.

## 보류 사항
- `mach_cryst_status`: 중국어는 `{0} 块在凝`으로 결정 개수, 영어는 `{0} setting`으로 설정 단계를 지칭해 불일치. 실제 DLL 호출 인자를 알 수 없어 기존 한국어 '단계'를 유지하고 추후 확인 대상으로 기록.
- 실제 게임 UI 출력·물품 동작과 DLL의 런타임 변수 값은 이번 작업에서 확인하지 않음.

## 재확인
- 한국어 CSV 1975개 키, 누락/중복/변수 불일치 0, 표준 2열 CSV 형태 정상, 975키 기록과 실제 번역 불일치 0.
- 1~1000번 변경 0건. Steam 및 DLL 미변경.
