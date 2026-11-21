아마도 성인용품 (Probably Adult Goods) — 원본 manifest v1.03 한국어 패치

원본 폴더명은 '可能是成人用品 v1.1.3'이지만, 실제 manifest의 ModVersion은 **1.03**입니다.
같은 ID probably_adult_goods의 v1.1.1 / v1.2.1 / 통합 v1.1.3과는 다른 배포본입니다.
같은 ID의 두 버전을 동시에 설치하면 안 됩니다.

- 원본용 한국어 수정 probably_adult_goods.dll(앞선 정적 번역 작업)
- Localization/ko.csv: 중국어 원문 로컬라이제이션 192키 대응 자료.
  그중 190키를 한국어로 번역하고 2키는 원문을 유지했습니다. 따라서 완역 192/192라고 기록하지 않습니다.
- 번역 manifest.xml / 원본 README의 한국어 설명서 README.txt

설치 시 같은 버전 모드에 해당 파일만 덮어쓰고,
원본 Textures/Items의 PNG 파일과 Data의 상품 ID/카탈로그는 삭제·개명하지 마세요.
