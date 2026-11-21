# DailyWearWorkshop.dll 8차 정적 검사

- 기준 DLL blob: `73cd7645b8afd8d48ab3a18f151062807336bdc8`
- 수정 DLL blob: `e07c7a6854213711dcc761f76f3684e56338fe8a` (540,160바이트)
- 이번 16개 문자열 / 수정 IL 참조 16곳, 기존 문자열 활성 참조 0곳
- 번역 누적: 완전 번역 598개/초기 중국어 활성 813개. 공유 원문 4개는 별도 부분 번역
- 잔여 활성 한자 포함 원문: 215개 (`DailyWearWorkshop_remaining_inventory_20261121.tsv`)
- PE #US UTF-16LE 역디코딩 및 각 IL 참조 검증. 기존 원문 #US 바이트 및 비대상 CLR 메타데이터 스트림 불변
- DLL 원본은 수정하지 않고 번역 폴더의 시험본만 수정
- 누적 ZIP: `ProbablyStolen_DailyWearWorkshop_DLL_KO_Partial_598_20261121.zip`, 671,249바이트, 11파일
- 아이템 등록/검색 키, 리소스 객체명, 캐릭터명 등은 의미가 명확할 때까지 원문 유지
- 실제 Unity/CLR 게임 로딩, UI, 저장/호환성 테스트 미실시; GitHub 바이너리 독립 재다운로드도 미검증
