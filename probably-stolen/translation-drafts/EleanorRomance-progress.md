# Eleanor Romance (`3834588749`) — 번역·검수 진행 현황

- 기준: 2026-11-11 (KST). 모드 버전 **0.90**.
- 원문: 영어 `Localization/en.csv` 2,236개 키, 중국어 `Localization/zh.csv` 2,178개 키 (영어 전용 58개).
- **누적 2,100 / 2,236개 (93.92%)** 1차 번역·영문/중문 대조 완료. **남은 136개**.
- 누적 한국어 번역 **2,096개**. 원문 공백 4개(126·128·144·146번)는 공백 유지.
- **이번 1,801~2,100번: 300개 한국어 초안 작성**, 신규 원문 공백 0개.
- 누적 초안: `번역 모드/translation-drafts/pskb__3834588749_DRAFT.csv`
- 1:1 검수 기록:
  - `translation-drafts/EleanorRomance-review-001-120.csv`
  - `translation-drafts/EleanorRomance-review-121-300.csv`
  - `translation-drafts/EleanorRomance-review-301-600.csv`
  - `translation-drafts/EleanorRomance-review-601-900.csv`
  - `translation-drafts/EleanorRomance-review-901-1200.csv`
  - `translation-drafts/EleanorRomance-review-1201-1500.csv`
  - `translation-drafts/EleanorRomance-review-1501-1800.csv`
  - **`translation-drafts/EleanorRomance-review-1801-2100.csv`**
- **다음 마지막 초안 구간: 2,101~2,236번(136개).** 시작 키 `er_v06_c21_b006_03`.
- 원본 Git blob SHA: 영어 `1e139cc778922f7adc4980cf72e47ef7b2e0db9f`, 중국어 `7c12e55b4b188a0944520915373a69812a2eb220`.
- 누적 2,100개 형식 검사: 원문 키 순서·중복·CSV 열 수·변수·리터럴 `\\n` 및 `<i>` 태그 일치. 이번 300개 전부 중문 원문 키 대응.
- **2차 독립 대사·화자·감정선 검수는 아직 미완료.** 특히 `er_v06_c14_b001_04` (1939번)은 원문 내 300크레딧 수취 주체가 기존 줄거리와 상충해 별도 표시함.
- 실행용 `번역 모드/Localization/pskb__3834588749.csv`는 아직 만들지 않음. 스팀 배포 없음.

## 엘리나의 말투·검수 방향
- 사용자가 제공한 기본 게임 엘리나의 한국어 대사를 기준으로, 초반의 오만함 → 궁핍 → 자립 → 성숙한 친근함 변화를 구분.
- **엘리나**, **리디아**, **어머니**, **와일드**, **상층 구역**, **하층 구역**, **뉴트리프루트**, **파스텔 사** 표기 유지.
- 엘리나와 리디아의 대사, 플레이어 선택지, 협박자, 공식 통지문, 모친 대사를 구분. 친밀해졌다고 임의로 반말 전환하지 않음.
- 기본 게임 원문에 대응하는 `er_v06` 대사는 제공된 한국어 대사 표현을 최대한 보존하면서 추가된 분기 대사도 번역.
- 2차 검수 때 문장 흐름과 원문 의미뿐 아니라 인물별 존대·화자 변경, 300크레딧 수취 주체 및 석방 조건을 재확인.

## 별도 키 갱신 상태
Go Fishing +81, 세력 평판 확장 +27, CorpPresence +1, PC Expansion +1 = 총 110개는 커밋 `0dd6a57f394e3f755f2e851ba770a2107b64028f`에 반영 완료.
