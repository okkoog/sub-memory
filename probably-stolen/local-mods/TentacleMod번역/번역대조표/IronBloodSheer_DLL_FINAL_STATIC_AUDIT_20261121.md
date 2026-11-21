# IronBloodSheer DLL 정적 현지화 확인

- 원본 blob `15910691fd1663190feb60d75430fb64a295e18e`, 9216바이트. 중국어 user-string 0개, 영어 진단 및 상태 메시지 10개 존재.
- 한국어 시험 DLL `probably-stolen/local-mods/TentacleMod번역/IronBloodSheer/ironblood_sheer.dll`, 15872바이트. 10개 메시지/IL 10곳 변경. 기능 파일명, 이벤트 키 및 경로는 보존.
- 실제 게임/CLR 로드 테스트 미실시. 외부 이미지 파일/원본 폴더 변경 없음.

- `[ironblood_sheer] Init v` → `[ironblood_sheer] 초기화 v`
- `[ironblood_sheer] Enabled` → `[ironblood_sheer] 활성화됨`
- `[ironblood_sheer] Disabled` → `[ironblood_sheer] 비활성화됨`
- `[ironblood_sheer] cannot decode ` → `[ironblood_sheer] 디코딩 실패: `
- `[ironblood_sheer] load failed for ` → `[ironblood_sheer] 파일 불러오기 실패: `
- `[ironblood_sheer] no Art folder at ` → `[ironblood_sheer] Art 폴더를 찾을 수 없음: `
- `, falling back to Textures/Items` → `, 대신 Textures/Items 경로 사용`
- `[ironblood_sheer] on-demand art: ` → `[ironblood_sheer] 요청 시 이미지 로드: `
- ` file(s)` → `개 파일`
- `[ironblood_sheer] art scan failed: ` → `[ironblood_sheer] 이미지 검색 실패: `
