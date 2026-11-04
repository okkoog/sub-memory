# CURRENT BASELINE — 2026-11-04 Work-improved package

**Use this as the current project baseline before any older source/build.**

- Korean Pack version: **0.7.5.12-dev**
- Current baseline metadata: `probably-stolen/current-v07512/README.md`
- Current manifest: `probably-stolen/current-v07512/manifest.xml`
- Exact user-tested/improved package SHA-256: `7386fee70300ec0969d83c0154cbe47e2efa54c3d1a6253f8b7bba999bdf568b`
- Exact DLL size: **84,992 bytes**
- Exact DLL SHA-256: `834be6542920c335f812368e1d86318a06736a16b0a66f87907453c6defa8466`
- Previous 0.7.5.4 / 58,880-byte DLL is retained only as an older rollback/reference baseline.
- Work-improved notes are under `probably-stolen/current-v07512/README_*_KO.txt`.
- Third-party mod files remain unmodified.
- Do **not** treat `probably-stolen/source/current-v0754` or older `KoreanPack.v0.5.41.cs` as the current implementation.

Important: the exact 0.7.5.12 DLL was recovered/decompiled in the working session, but the repository currently records its authoritative package/DLL hashes and Work change notes; do not rebuild/replace the user-tested DLL from older source unless the 0.7.5.12 recovered source has first been synchronized and verified.

---

## 최우선 작업 경계 — 2026-11-04 재확인

**사용자가 허용한 수정 대상은 우리 한국어팩뿐이다.** CorpPresence·Proficiency·PCExpansion·기본 한국어 번역 모드·게임 DLL은 비교 및 역컴파일로 분석만 허용된다. 제3자 모드의 패치 DLL 생성, 별도 테스트 빌드, 재배포, 원본 변경 모두 금지. 충돌 해결책은 우리 한국어팩의 소스와 DLL에서만 구현할 것.

이전 응답에서 CorpPresence 수정 테스트 DLL을 제작한 것은 사용자 지시 위반이었다. `main`의 `probably-stolen/test-builds/CorpPresence-v0.9.4-delayed-locale/`, `probably-stolen/tools/corp-init-fix/`, `.github/workflows/corp-init-fix.yml`는 삭제했고, `mods/3828913695/CorpPresence.dll` 원본 blob `49ad5f6b8894d7525e6b5c81b0a4aa124a2f4732`는 그대로 확인했다. 과거 커밋 이력에는 테스트 빌드 흔적이 남지만 현재 브랜치에는 없다.

---

# Probably Stolen — 한국어팩 개발 인수인계
마지막 업데이트: 2026-11-03 (KST)

## 목표
하나의 KoreanPack DLL과 자체 `Localization/<WorkshopModID>/ko.csv` 모음으로 여러 Probably Stolen 워크샵 모드를 한국어화한다.

- 원본 모드의 DLL·자산 **수정 금지**
- 모드별 별도 DLL **생성 금지**
- 게임이 제공하는 `ModLoader.LoadLocalization`, `LoadedMod.Localization`, `ModHelper.GetLocalized` 우선
- 불특정 전역 UI·파일·리소스 읽기 후킹 금지
- 새 모드는 가능하면 CSV 추가만으로 지원
- 기존 번역 데이터 약 16개 모드/12,740행 보존

## 최근 버전
- v0.5.41: 소스 구조 개편, 광범위한 Harmony 후킹. `KoreanPack.cs` 소스 있음.
- v0.5.42: 스캐너 제한 회피 바이너리 패치, Unity Localization 초기화 및 IL 오류 발생.
- v0.5.43: `Assembly.GetManifestResourceStream` 후킹과 조기 locale 접근 일부 완화. 본체 번역 정상화, 워크샵 번역 거의 미적용.
- v0.5.44: 전역 `File.ReadAllText`/`File.ReadAllBytes` 후킹 제거. 미적용 문제 미해결.

## 사용자가 확정한 충돌
- CorpPresence + Proficiency System → 충돌
- CorpPresence + PCExpansion → 충돌
- CorpPresence + 한국어팩 → 충돌
이 실험 결과를 재검증 요구하거나 특정 모드 하나를 단독 원인으로 단정하지 말 것.

## 확인된 예외 경로
`Player(1).log`에서 PCExpansion의 Harmony.PatchAll 중 `ShowcaseHelper.CreateClientByStockType` 패치가 `WaterFeatureHelper..cctor()`를 트리거하고, `LocHelper.GetLocalizedMechanic` → Unity Localization/Addressables에서 `Attempting to use an invalid operation handle` 발생.
이 경로 자체는 확인됐으나 세 충돌 조합의 공통 원인은 **미확정**.

`Player(3).log`에서는 CorpPresence v0.9.1이 `reflection-external` 권한 변경 때문에 로딩되지 않았으므로, 이것은 충돌 실행 로그가 아니라 권한 차단 로그임.

## v0.5.41 코드 분석에서 확인된 위험
1. `PatchModOwnedWriteSites`가 `GetConstructors`와 `TypeInitializer`를 포함해 타 모드의 생성자/.cctor를 Harmony transpiler 대상으로 삼을 수 있음.
2. `OnAssemblyLoad`에서 타 모드 Assembly 로딩 직후 `PatchModOwnedModelSetters` 및 `PatchModOwnedWriteSites`를 호출함. 공식 로딩 완료 이전 개입 위험.
3. `RefreshLoadedMods` 안에 Localization 사전 병합과 광범위한 모드 메서드 패치가 섞여 있음.
4. `IsKorean`과 `KoreanLocaleCodes`에서 `LocalizationSettings.SelectedLocale`에 접근함. Locale 초기화 전 조기 접근 방지 검토.
5. `MergeNativeEntries`는 `loaded.Localization == null`이면 즉시 반환함.
6. `ModHelper.GetLocalized`의 특정 시그니처만 후킹: 실제 게임 DLL에서 오버로드 재검증 필요.
7. `localized mod write methods: 0` 로그는 OnEnable 시점 스냅샷이므로, 그 로그만으로 이후 패치 개수를 확정할 수 없음.

## 다음 구현 방향
- `ModHook.OnModAssetsLoaded` 등 게임 공식 lifecycle 이벤트의 **실제 선언/호출 시점/시그니처**를 게임 DLL에서 검증.
- Localization 로드 완료 이후 `LoadedMod.Localization`에 ko 번역 병합, `ModHelper.GetLocalized`로 정상 조회되게 재구성.
- Harmony 패치는 안전성이 확인된 타깃에만 제한; 타 모드 .cctor/광범위한 IL 패칭 폐기.
- 기존 JSON 리소스는 전역 `File` 및 `Assembly.GetManifestResourceStream` 후킹 없이, 해당 모드의 데이터 생성·등록 위치에서만 처리.
- CSV에 원문이 없는 미번역과 CSV는 있으나 적용이 안 된 미번역을 구분.
- `Welcome to Valhalla`는 이전 검사에서 CSV에 없는 문자열의 예로 확인됨.

## 원본 자료 (ChatGPT 파일 라이브러리)
- **최신 모드집** `4348910(2).zip` (303,792,997 bytes)
- 이전 모드집 `4348910(1).zip` (269,373,244 bytes)
- `ProbablyStolen_KoreanPack_v0.5.44_SafeRead_Install.zip`
- `ProbablyStolen_KoreanPack_GenericBridge_v0.5.41_CompileFix_Source.zip`
- `ProbablyStolen_KoreanPack_v0.5.41_KoreanPack_CompileFix.cs`
- `Player(1).log`, `Player(2).log`, `Player(3).log`
- 게임 관리 DLL 묶음 `Managed(1).zip`

## 실행환경/배포 유의
ChatGPT 실행 컨테이너가 `ClientError`를 반환하여 압축 해제·DLL 비교·빌드가 불가했음. 파일 라이브러리 접근과 GitHub API 연결은 정상.
GitHub는 압축파일을 자동 해제하지 않으며 GitHub Actions에서 실행 스크립트로 해제 가능. 다만 303.8MB 원본 ZIP은 GitHub Contents API 100MB 제한에 걸리고, ChatGPT 라이브러리 파일은 GitHub Actions에 자동 전달되지 않음.
원본 워크샵 DLL·게임 자산을 공개 저장소에 무단 배포하는 것은 피할 것.

## 수행할 순서
1. 최신 ZIP의 CorpPresence/ProficiencyMod/PCExpansion 해시·크기·manifest 및 이전 버전과의 차이 확인.
2. 공통 Harmony 패치 대상, WaterFeatureHelper/LocHelper 등 초기화 연결 지점 확인.
3. 한국어팩의 조기/광범위 Harmony 패치 제거; Locale lifecycle 기반 설계 구현.
4. 기존 CSV 보존 및 번역 적용 연결.
5. DLL을 실제로 빌드하고 설치 구조를 검증한 후에만 배포. 게임 내 정상 동작은 게임 테스트 전까지 '미검증'으로 표시.

이 폴더는 본 프로젝트의 GitHub 서브메모리 기준점으로 사용.


## 2026-11-04 — CorpPresence v0.9.4 분석 결과 (우선 확인할 것)

- 게임 본체 `Assembly-CSharp.dll`, `Unity.Localization.dll`, Corp v0.9.4와 기본 한글 번역 DLL까지 GitHub Actions ILSpy로 실제 분석 성공.
- **코드상 명확한 초기화 결함**: `ModLoader.LoadModDll()`이 모든 모드의 `OnEnable`을 부른 뒤 `ModLoader.LoadModAssets()`에서 CSV 등을 로드함. 그러나 `CorpPresence.Main.OnEnable`은 `_bootstrap.Tick`을 즉시 실행해 `GameResources.Poll`의 `LocalizationSettings.InitializationOperation` 호출로 Unity Localization 초기화 작업을 미리 시작한다. 게임의 Unity.Localization DLL에서 Getter가 `ResourceManager.StartOperation`을 실제 호출함을 확인했음.
- `GameResources.Poll`의 선택 언어 Mechanic 키 강제 확인과 `ResourceBootstrap.Tick`의 예외 시 즉시 `Abort+rethrow`가 실패를 복구 불가능하게 만듦.
- 게임 `WaterFeatureHelper` 타입 정적 필드에 `LocHelper.GetLocalizedMechanic("mech_condition_pure_water")` 동기 호출이 있어, 다른 Harmony 모드가 정적 초기화를 앞당기면 초기화 타이밍 충돌 경로가 존재함.
- **수정안**: Corp `OnEnable` 직후 Tick 제거, `OnModItemDirectoryInit` 즉시 Tick도 지연, Loader.Awake 자산 로딩이 끝난 첫 프레임 이후 Poll; 선택 로케일의 키 누락은 fallback/retry 및 오류 유형 분리.
- 이것이 사용자 확인한 세 충돌 조합의 유일한 원인인지와 v0.9.4 런타임 복구 성공 여부는 **미검증**. 이전 Player(1) 로그의 Corp v0.9.1은 실제로 로딩되지 않았으므로 그 로그를 Corp 충돌 근거로 사용하지 말 것.
- 자세한 근거/수정 지점: [CorpPresence-0.9.4-findings.md](analysis/CorpPresence-0.9.4-findings.md) 및 [callsite-context](analysis/CorpPresence-0.9.4-callsite-context.md).


## 2026-11-04 — 인게임 미번역 재점검

사용자 최신 인게임 확인에서 모드 자체는 정상 적용되고 게임 진입도 성공했다. 다만 다음 대상이 화면상 미번역 또는 부분 번역으로 남아 있음.

- 화면상 사실상 미번역: game_card_pk(3832048841), lewd_meme/촉수(3831837821), xiaowo_night_auction(3829429358), Dream_Item_Storage(3829085054)
- 부분 번역: brewing_expansion(3829730904), CorpPresence(3828913695)
- 나머지는 아직 인게임 식별 미완료

중요: 최신 설치본 ProbablyStolen_KoreanPack_v0.7.0-dev_Install.zip을 확인한 결과 위 4개 "미번역" 대상에도 이미 상당량의 번역 카탈로그가 들어 있다.
- game_card_pk: pskb__3832048841.csv 약 410KB
- lewd_meme: pskb__3831837821.csv 약 72KB
- xiaowo_night_auction: pskb__3829429358.csv 약 67KB
- Dream_Item_Storage: pskb__3829085054.csv 약 62KB

따라서 이 4개는 단순 번역 데이터 부족이 주원인이 아니라, 해당 모드의 동적 아이템/툴팁 생성 경로에서 한국어팩 카탈로그가 적용되지 않는 문제로 우선 분류한다. 특히 game_card_pk의 인게임 미번역 예시 설명 "跟在队伍后面的黑鸟。它不挑食，只挑还剩气的。"은 과거 번역 데이터에 이미 "일행의 뒤를 따르는 검은 새. 먹이를 가리지는 않지만 아직 숨이 붙어 있는지만 확인합니다."로 존재한다. 즉 번역문 자체가 없어서 원문이 나온 사례가 아니다.

최신 모드 ID/버전 식별:
- game_card_pk / workshop 3832048841 / v0.9.9
- lewd_meme / workshop 3831837821 / v1.0.3
- xiaowo_night_auction / workshop 3829429358 / v2.1.7
- Dream_Item_Storage / workshop 3829085054 / v1.1.7
- brewing_expansion / workshop 3829730904 / v3.6.7.6

다음 우선순위는 CSV 증량보다 먼저 위 4개 모드에서 실제 표시 문자열이 생성·대입되는 경로를 추적하고, 우리 한국어팩 내부에서 그 경로만 안전하게 수용하는 것. 타 모드 원본/DLL 수정 금지 원칙은 그대로 유지한다.
