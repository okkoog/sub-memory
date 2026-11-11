# Eleanor Romance (`3834588749`) — 번역·검수 진행 현황

- 기준: 2026-11-11 (KST). 모드 버전 **0.90**.
- 소스: 영어 `Localization/en.csv` **2,236개 키**, 중국어 `Localization/zh.csv` **2,178개 키**. 영어 전용 58개.
- **누적 1,800 / 2,236개 (80.50%)** 1차 번역·원문 대조 완료. **남은 436개**.
- 누적 한국어 번역 **1,796개**, 원문 자체 공백 4개(126·128·144·146번) 유지.
- **이번 1,501~1,800번: 300개 한국어 초안 작성**, 신규 원문 공백 0개.
- 누적 초안: `번역 모드/translation-drafts/pskb__3834588749_DRAFT.csv`
- 구간별 영문·중문·한국어 검수 기록:
  - `translation-drafts/EleanorRomance-review-001-120.csv`
  - `translation-drafts/EleanorRomance-review-121-300.csv`
  - `translation-drafts/EleanorRomance-review-301-600.csv`
  - `translation-drafts/EleanorRomance-review-601-900.csv`
  - `translation-drafts/EleanorRomance-review-901-1200.csv`
  - `translation-drafts/EleanorRomance-review-1201-1500.csv`
  - **`translation-drafts/EleanorRomance-review-1501-1800.csv`**
- 다음 구간: **1,801~2,100번(300개)**. 시작 키 `er_v06_c07_b001_02`.
- 원문 Git blob SHA: 영어 `1e139cc778922f7adc4980cf72e47ef7b2e0db9f`, 중국어 `7c12e55b4b188a0944520915373a69812a2eb220`.
- 누적 1,800개 검증: 원문 순서 일치, 중복 0, CSV 열 이상 0, 변수 자리표시자 오류 0, 리터럴 `\\n` 줄바꿈 오류 0. 신규 구간 `<i>` 서식 태그 보존 확인, 300개 모두 중국어 원문 동시 대조.
- **2차 독립 대사·화자·문맥 검수는 아직 미완료.** 특히 1522·1557·1566·1587·1611번의 문맥 확인이 필요하며 검수 CSV에 기록함.
- 아직 실행용 `번역 모드/Localization/pskb__3834588749.csv` 미생성. 스팀 미배포.

## 엘리나 말투·번역 기준
- 사용자 제공 기본 게임 한국어 대사를 우선 참조. 원래 초반의 오만한 존댓말(1639~1640)과 궁핍해진 뒤의 사과(1754~1755), 자립 단계의 변화(1779 이후)를 연결하여 번역.
- 명칭은 엘리나, 리디아, 와일드, 상층 구역·하층 구역, 뉴트리프루트, 파스텔 사로 유지.
- 장면별 플레이어/엘리나/리디아/NPC 대사, 선택지·서술문·시스템 메시지를 분리하며 근거 없는 반말 전환 금지.
- 혁명군 루트는 미라 사망, 조 구출, 호송 작전과 외상 이후의 감정 표현을 절제. 원문에 없는 사건·감정 추가하지 않음.
- 사용자 제공 기본 게임 대사가 원문과 다소 다른 경우 원문의 의미·호칭을 유지하면서 어투를 맞춤.

## 별도 모드 업데이트
- Go Fishing +81, 세력 평판 확장 +27, CorpPresence +1, PC Expansion +1 = 총 110개 신규 키는 커밋 `0dd6a57f394e3f755f2e851ba770a2107b64028f` 반영 완료.
