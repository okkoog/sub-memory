몰락한 영애 엘리너 + Probably Adult Goods 통합판 v1.1.3 — 한국어 번역 DLL

■ 원본
- TentacleMod/3843142171/fallen_heiress_eleanor.dll (16,721,920바이트)
- 원본 manifest.xml: ID=probably_adult_goods, ModVersion=1.1.3
- 이 통합판은 다른 Probably Adult Goods 버전(1.03/1.1.1/1.2.1), 엘리너 독립판과 동시에 설치하지 마세요. 중복 모드 ID/기능 충돌 가능성이 있습니다.
- 원본에 native capability 및 payload.txt가 포함돼 있습니다. 번역은 이를 변경하거나 실행하지 않았습니다.

■ 한국어 번역 구현
- 원본 EleanorEmbedded/Localization/zh.csv 및 en.csv는 엘리너 독립판 v1.0.3과 바이트까지 동일합니다. 기존에 원문을 대조해 번역한 유효 키 1,553개를 그대로 사용하고 개발용 비활성 placeholder 286개를 유지합니다.
- 별도 EleanorEmbedded/Localization/ko.csv를 내장하고 한국어 사전 ko/ko-KR를 등록하는 IL을 추가했습니다.
- 통합된 ProbablyAdultGoods.Localization.zh.csv 및 en.csv는 Adult Goods v1.1.1과 바이트까지 동일합니다. 기존 검수한 181개를 ko/ko-KR 독립 리소스로 추가했습니다.
- Adult Goods 진열장 표시 전용 10개 IL 문자열도 한국어로 수정합니다. 내부 ID, 저장·판정용 문자열은 바꾸지 않습니다. 2026-11-22에 Adult Goods 설명 2개를 중국어 원문 기준으로 복원했습니다.

■ 사용
1. 반드시 원본 3843142171 v1.1.3 모드를 먼저 설치하고 원본 파일을 보관하세요.
2. 게임을 종료한 상태에서 이 번역본의 fallen_heiress_eleanor.dll만 원본 폴더에 덮어씁니다.
3. 원본 payload.txt 등 다른 리소스는 삭제하거나 교체하지 마세요. ZIP은 원본 전체가 아닌 덮어쓰기 패치입니다.
4. 설치 후 게임에서 실제 로딩·대사·툴팁·기존 저장 데이터·모드 호환성을 검증해야 합니다.

■ 중요
- 원본 통합판은 native capability를 선언하고 별도 payload.txt를 보유합니다. 이 번역 패치에서 실행·분석 완료 또는 안전성을 보증하지 않습니다.
- GitHub Actions 및 정적 CLR/리소스 검사만 수행했습니다. Unity/CLR 인게임 테스트는 실시하지 않았습니다.
- 한국어 번역으로 인해 원본 앱 기능 및 기타 버전과 호환성이 검증된 것은 아닙니다.
