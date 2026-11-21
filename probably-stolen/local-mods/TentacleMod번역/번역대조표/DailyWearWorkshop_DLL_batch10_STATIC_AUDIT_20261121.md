# DailyWearWorkshop.dll — 10차 정적 검수

- 기준 DLL Git blob `2f3710f91d2d56719386a73b7be212d38ec4ebbc`.
- 10차 DLL Git blob `d4c9178627c00f4d60f0ba2f3cb92185bc74e417`, 541,184바이트.
- 11개 중국어 소스 → 한국어 사용자 문자열 11개, 활성 IL 수정 12곳. 완전 전환 10개/부분 전환 1개.
- `折叠衣柜`: `Register`의 `Definition.Intro` 1곳만 변경, `Build`의 객체명은 중국어 원문 참조 보존.
- `泡茶杯`: 등록측 `Intro` 및 동적 `GameItem.set_name` 양쪽을 전환해 원문 참조 제거.
- 동적 차 상태명 `回甘养生茶`, `浸泡中的回甘养生茶`, `浸泡异常的泡茶杯`은 `GameItem.set_name`에 연결되는 표시명에 한국어 적용.
- 기존 `Definition.Id`/`Name`은 변경하지 않았으나 동적 `GameItem.name` 자체가 외부에서 비교될 수 있으므로 실게임·호환성 위험은 남음.
- 누적 완전 번역 694/813, 원문 잔여 활성 119; 표시 호출부만 번역한 부분 전환 원문 5개는 별도.
- #US 한국어 문자열 재해석, 이전 원문 토큰 미참조, 보존 토큰 확인, 기존 #US 및 비대상 메타데이터 스트림 보존.
- `ProbablyStolen_DailyWearWorkshop_DLL_KO_Partial_694_20261121.zip` 661,546바이트/13파일.
- Unity/CLR 실행 검증 미실시, 원본 모드 무변경, 현재 시험본 미완료.
