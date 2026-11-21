# GitHub ZIP 재조회 및 CRC32 검증 — 2026-11-21

- `ProbablyStolen_DailyWearWorkshop_DLL_KO_Partial_709_20261121.zip`
  - Git blob: `987c84af9da6a2691bbfe446324cc03e6e28c511`
  - 크기 671,318바이트, ZIP 엔트리 14개
  - GitHub에서 `fetch_file(encoding=base64)`로 별도 재조회해 ZIP 바이트를 다시 디코딩함
  - EOCD, 중앙 디렉터리, 로컬 파일 헤더, 엔트리 길이, 저장 방식(method 0) 검사 통과
  - **14개 엔트리 전체 CRC32 비교 일치 / 불일치 0개**

- `ProbablyStolen_BaiShe_R18_Pack_Korean_Metadata_20261121.zip`
  - Git blob: `fa68498dad7c852b7eb8a30ea691bdee3628a927`
  - 크기 3,588바이트, 엔트리 2개
  - 독립적으로 재조회/디코딩한 ZIP의 중앙 디렉터리·로컬 헤더·크기·저장 방식 검증 통과
  - **2개 엔트리 모두 CRC32 비교 일치 / 불일치 0개**

**제한:** ZIP 손상 검사 통과는 PE/CLR 또는 Unity 게임에서 DLL의 기능/저장·모드 호환성이 정상임을 보증하지 않습니다. 실제 게임 실행 검증은 여전히 0회입니다.
