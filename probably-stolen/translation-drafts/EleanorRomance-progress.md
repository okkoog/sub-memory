# Eleanor Romance (`3834588749`) — 한국어 번역·검수 진행 현황

- 기준: 2026-11-11 (KST). 원본 모드 버전 **0.90**.
- 원문 영어 `Localization/en.csv` **2,236개 키**; 중국어 `Localization/zh.csv` **2,178개 키**. 영문에만 존재하는 키는 **58개**(`er_v06_rc_b001_01`부터 `er_v06_rc_b015_03`; 2,163~2,220번).
- **1차 번역 2,236 / 2,236개 (100%) 완료**: 한국어 문구 **2,232개**, 영어 원문 자체가 비어 있어 유지한 **4개**(126·128·144·146번).
- **이번 마지막 구간 2,101~2,236번 136개 처리**. 2,101~2,162 및 2,221~2,236은 영문·중문 모두 대조, 2,163~2,220의 58개는 중국어 키가 없어 영문으로 번역.
- 누적 초안: `번역 모드/translation-drafts/pskb__3834588749_DRAFT.csv`.
- 구간별 1:1 검수 기록:
  - `translation-drafts/EleanorRomance-review-001-120.csv`
  - `translation-drafts/EleanorRomance-review-121-300.csv`
  - `translation-drafts/EleanorRomance-review-301-600.csv`
  - `translation-drafts/EleanorRomance-review-601-900.csv`
  - `translation-drafts/EleanorRomance-review-901-1200.csv`
  - `translation-drafts/EleanorRomance-review-1201-1500.csv`
  - `translation-drafts/EleanorRomance-review-1501-1800.csv`
  - `translation-drafts/EleanorRomance-review-1801-2100.csv`
  - **`translation-drafts/EleanorRomance-review-2101-2236.csv`**
- 원본 Git blob SHA: 영어 `1e139cc778922f7adc4980cf72e47ef7b2e0db9f`, 중국어 `7c12e55b4b188a0944520915373a69812a2eb220`.
- 누적 **2,236개 키** 전체 검증: 영어 원본 키와 1:1 순서 일치, 중복 0, CSV 칼럼 오류 0, `{0}` 등 변수·리터럴 `\\n`·기존 태그 차이 0, 빈 원문 보존. 중국어 키 없는 58개도 정확히 분리.
- **아직 최종 번역본이 아니라 1차 수동 번역 초안**. 독립적인 2차 문맥·화자·말투·용어 검수가 남아 있다. 300개 단위(마지막 136개)로 재검수 권장.
- 실행용 `번역 모드/Localization/pskb__3834588749.csv`는 **아직 생성하지 않음**. 스팀 미배포.

## 이번 구간에서 확인할 사항
- 2,116번 `er_v06_c22_b005_01`: 영어·중국어의 1·2인칭 및 방문 방향이 서로 다름. 발화자/선택지의 장면 문맥 2차 확인 필요.
- 2,222번 `er_v06_rd_b001_02`: 플레이어가 드라이브를 넘기고 300크레딧을 받았다는 실토. 앞선 1,939번의 수취 주체 해석도 함께 검증.
- 2,163~2,220번: 영문 전용 비극·혁명군 서사 58개. 중국어 대조 불가능하므로 2차에서 전후 대사·원작 캐릭터 화법을 추가 점검.
- 2,201번 리디아 발화, 2,231~2,233번 신문사 유출 분기 등 플레이어·엘리나 화자 구분.
- 과거부터 보류한 1,087·1,167·1,236·1,434·1,476·1,522·1,557·1,566·1,587·1,611번 및 다른 기록의 보류 키도 2차에서 순서대로 검토.
- 엘리나의 **초기 거만함 → 빈곤·불안 → 자립 → 성숙한 연인/동료** 단계별 존댓말을 유지. 임의 반말 전환 금지. 이름 표기는 엘리나, 리디아.

## 다른 모드 신규 번역 키
Go Fishing +81, 세력 평판 확장 +27, CorpPresence +1, PC Expansion +1 = 110개 키를 별도 커밋 `0dd6a57f394e3f755f2e851ba770a2107b64028f`에 반영 완료.
