# DailyWearWorkshop — 11차 실제 Name 필드 번역 감사

- `Definition.Apply`는 `Definition.Name`을 `GameItem.set_name`(0x0a000074)으로, `Definition.Intro`를 `set_shortDescription` 및 `set_description`으로 전달함. 기존 9차 '이름 번역' 표현 오류를 정정.
- 베이스 DLL SHA `d4c9178627c00f4d60f0ba2f3cb92185bc74e417`.
- 결과 DLL SHA `e90d6bf367f07da408efc3560fdadecd6416a972` (541,184바이트).
- 실제 Name 변경 107개, 활성 IL 110곳: 이전 Intro의 한국어 토큰 92개 재사용 + 신규 한국어 Name 15개.
- `Definition.Id` 문자열 참조 92개 원래 그대로 유지; 이름 비교가 필요한 안내서 2종 및 도마 UI 문구도 함께 수정.
- 원래 중국어 Name 완전 전환 15개 추가; 누적 중국어 활성 원문 완전 전환 709/813, 중국어 활성 잔여 104개.
- 기존 US/타 메타데이터 내용 불변, UTF-16LE 역해독 및 신규 IL 토큰 연결 확인.
- 패치 대조표 `DailyWearWorkshop_DLL_batch11_NAME_20261121.tsv` 107행, 최신 잔여 원문 목록 104행.
- 결과 ZIP `ProbablyStolen_DailyWearWorkshop_DLL_KO_Partial_709_20261121.zip`, 671,318바이트, 14파일.
- **실제 CLR/Unity 로딩·인게임 UI·세이브·타 모드 호환성 미검증**. 이름 비교 호환성 위험을 해결했다고 단정하지 않음.
