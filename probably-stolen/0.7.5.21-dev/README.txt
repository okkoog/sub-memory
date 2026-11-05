Probably Stolen Korean Pack v0.7.0-dev

핵심 구조
- 게임 공식 ModHook.OnModAssetsLoaded 이벤트만 사용합니다.
- ModLoader를 Harmony/Reflection 대상으로 삼지 않습니다.
- 런타임 파일/폴더 탐색을 하지 않습니다.
- 번역 파일은 게임 기본 Localization 로더가 먼저 읽습니다.
- 대상 모드는 게임이 넘겨준 LoadedMod.Manifest.ID / WorkshopId로만 식별합니다.
- native localization key는 대상 LoadedMod.Localization의 ko/ko-KR에 병합합니다.
- hard-coded 텍스트는 대상 LoadedMod.Assembly 안의 표시/로컬라이저 후보 메서드만, 정확한 원문 일치 시에만 후킹합니다.

Localization 파일 형식
- 파일명: pskb__<WorkshopID 또는 식별자>.csv
- M|<ManifestID>,"1" : Manifest ID 별칭
- W|<WorkshopID>,"1" : Workshop ID 별칭
- K|<native_key>,"한국어" : 원래 Localization key 번역
- K64|<base64url_key>,"한국어" : 쉼표/개행 등이 있는 key
- S|<source>,"한국어" : 쉼표 없는 hard-coded 원문
- S64|<base64url_source>,"한국어" : hard-coded 원문(권장)

v0.7.0-dev는 실게임 런타임 검증 전 테스트 빌드입니다.

스캐너 검증
- 게임 ModScanner.Scan(): HARD_COUNT=0
- 요구 capability: reflection-external 만
- files / loader-target 없음
