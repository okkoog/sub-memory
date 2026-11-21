몰락한 영애 엘리너 v1.0.0 — 독립 한국어 번역 DLL

■ 원본 및 제약
- 원본: TentacleMod/3842247788/fallen_heiress_eleanor.dll (16,418,816바이트, 모드 ID probably_eleanor_followup).
- 동일 ID를 쓰는 엘리너 v1.0.3과 동시에 사용하지 마세요. 공식 원본은 Probably Adult Goods 선행 모드를 필요로 합니다.
- 기존 엘리너 v1.0.3과 내장 Localization 1,826개 키가 겹치고, 그중 중국어/영어 원문이 일치하는 1,824개를 비교했습니다.
- 원문이 바뀐 두 대사(purity_wilde_visit_1_6, purity_wilde_visit_1_15)는 본 v1.0.0 버전의 문장과 말투에 맞춰 별도 번역했습니다. v1.0.3에만 있는 12개 키는 추가하지 않았습니다.
- 유효 문구 1,541개 한국어화, 원본 비활성 개발용 placeholder 286개 유지.
- 별도 EleanorEmbedded/Localization/ko.csv를 DLL에 추가해 ko, ko-KR 두 가지 언어 사전 등록.
- 영어/중국어 원본을 수정하지 않고 원본 모드 DLL도 변경하지 않았습니다.

■ 설치
1. v1.0.0 원본 모드를 설치합니다.
2. 게임 종료 상태에서 번역본 DLL만 해당 폴더에 덮어씁니다.
3. 게임에서 대사·설정·세이브·다른 모드 호환성을 실제 확인합니다.

■ 정적 검사
- GitHub Actions에서 dnlib 재빌드 + 독립 dnfile CLR 구조, 원본 리소스, 한국어 키 수, ZIP CRC를 검증합니다.
- 실제 Unity/CLR 게임 실행 및 세이브 호환성 검증은 하지 않았습니다. 최신 v1.0.3과 구버전 v1.0.0을 임의로 혼합하지 마세요.
