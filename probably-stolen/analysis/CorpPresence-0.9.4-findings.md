# CorpPresence v0.9.4 — DLL 분석 결론
분석일: 2026-11-03
상태: **정적 분석 완료 / 게임 런타임 충돌 원인 미확정**

대상: `probably-stolen/mods/3828913695/CorpPresence.dll` (6,961,664 bytes)
SHA256: `7e34d05ad2269a0a0403fd5dcd3cea78f8b629af47f956a8366a807eea4ff668`
분석 방식: GitHub Actions에서 ILSpyCmd 9.1.0.7988로 정적 역컴파일 (C# 104개 파일). DLL **실행하지 않음**.
근거 파일:
- [원시 정적 스캔](CorpPresence-0.9.4-static-audit.md)
- [초기화 및 Harmony 호출문맥](CorpPresence-0.9.4-callsite-context.md)

## 1. 확인된 초기화 순서

1. `Main.OnEnable` → `_runtime.Start(EnableCore)`; 정상 시작 시 `_bootstrap.Tick()` 호출.
2. `Main.EnableCore`는 `GameResources.Begin`, `ItemRegistry.Install`, `Hooks.InstallCore`를 등록/실행하는 코드가 있음.
3. `ResourceBootstrap`은 `Update()`와 `ModHook.OnModItemDirectoryInit`에서 `Tick()` 실행.
4. `Tick()`에서 `GameResources.Poll()`이 성공하면 completion 콜백을 실행함.
5. `GameResources.Poll`은 `LocalizationSettings.InitializationOperation`의 Valid/Done/Status 확인 뒤 `StringDatabase.GetTableAsync("Mechanic", null)`로 테이블을 요청하고 핸들/상태/키 존재를 검증함.
6. `Main.EnableResources`는 `Hooks.InstallResources`를 호출하며, 이 함수는 `GameResources.Ready`가 거짓이면 예외를 발생시킴.

**판정:** 0.9.4는 리소스 준비 확인 뒤 주요 게임플레이/물 시스템 패치를 설치하도록 설계됨. 그러나 기본 패치 `InstallCore`는 이보다 먼저 실행된다.

## 2. 오류 문자열의 실제 발생 위치

`GameResources.Poll()`의 검사 결과에 따라:

- Localization initialization 실패 → `本地化系统初始化失败`
- Mechanic 테이블 핸들 무효 → `Mechanic 表句柄失效，停止公司初始化`
- Mechanic 테이블 로딩 실패 → `Mechanic 表加载失败`
- 키 누락 → `Mechanic 表缺少 mech_condition_pure_water`

따라서 이전에 보고된 `Mechanic 表缺少 mech_condition_pure_water`는 CorpPresence **자체 검사에서 발생하는 메시지**라는 점이 확정됨. 왜 실제로 해당 키를 찾지 못하는지는 미확정.

## 3. Harmony 패치

초기 `Hooks.InstallCore`는 `SaveManager.DecodeNodes`, `SaveManager.EncodeNode`, `PlayerStore.SaveGame`, `PlayerStore.LoadGame`, `PlayerStore.StartNewGame` 등을 패치.

리소스 준비 뒤 `Hooks.InstallResources`가 `InstallGameplay`과 아래의 물 시스템 패치 실행:
- `WaterFeatureHelper.UpdateWaterFeature`
- `WaterFeatureHelper.OnLiquidChanged`
- `WaterFeatureHelper.OnLiquidContainerPourInto`

`InstallGameplay`에는 `ShowcaseHelper.HandleDisplayCase`, `AquacultureHelper`, `StoreClient`, `GameItem`, `PlayerStore` 등 다수 게임 메서드에 대한 개별 패치 선언이 있음.

**현재 분석에서 `ShowcaseHelper.CreateClientByStockType`를 CorpPresence가 직접 패치하는 선언은 확인되지 않음.** 구 로그의 `CreateClientByStockType` 패치 오류를 곧바로 CorpPresence 패치의 직접 결과라고 단정하면 안 됨.

## 4. CorpPresence의 자체 언어 선택 — 한국어팩 개발에 중요

`CorpPresence/L.cs`의 `L.T(string zh, string en)`은 현재 게임 Locale 코드가 `zh`로 시작하면 중국어, 그 외에는 영어를 반환하는 2언어 선택기다.
따라서 **ko 로케일에서는 기본적으로 영어가 선택됨.** 이는 `ModHelper.GetLocalized` 또는 `LoadedMod.Localization`을 통하지 않는 문자열 경로이며, 사전 병합만으로는 해당 텍스트가 한국어가 되지 않을 수 있다.

향후 대응이 필요하면 **CorpPresence 전용 L.T 반환 지점**을 제한적으로 연결하는 방식을 검토할 수 있음. 단 하나의 범용 한국어팩 DLL 내부에서 처리하며, 타 모드 DLL을 수정하지 않고 전역 UI·파일 읽기 후킹을 추가하지 않는다는 기존 정책 유지. 현재는 설계 후보일 뿐, 구현·검증되지 않음.

## 5. 결론과 미확정 항목

**확정:** CorpPresence 0.9.4는 Mechanic 테이블 준비/키 존재를 자체 검증한다. 초기 Core Harmony 패치와 준비 이후 Resource Harmony 패치를 분리한다. 텍스트 일부는 L.T의 중국어/영어 분기로 처리한다.

**미확정:** 실제 게임에서 `Mechanic`의 `mech_condition_pure_water`가 왜 누락되는가. 다른 모드 및 한국어팩과의 공통 Harmony 패치 충돌 메서드. 0.9.4가 사용자 확인 충돌을 해결했는가. 정적 분석만으로 결론 불가.

이 문서는 CorpPresence 단독 분석이며, 다른 모드의 DLL/코드는 대조하지 않았다.


## 6. 게임 본체 DLL 대조 후 확정한 조기 초기화 결함

2026-11-04: `Managed/Assembly-CSharp.dll`과 `Managed/Unity.Localization.dll`을 추가로 ILSpy 역컴파일하여 Corp 코드 경로를 확인함.

- `ModLoader.Awake()`: `LoadAllMods()` → `LoadWorkshopMods()` → `ApplySavedOrder()` → `ValidatePrerequisites()` → `LoadModAssets()`.
- `ModLoader.LoadModDll()`: 모드 `Init()`과 `OnEnable()`을 **LoadModAssets 이전**에 호출.
- `CorpPresence.Main.OnEnable()`: `_runtime.Start(EnableCore)`에 성공하면 **즉시** `_bootstrap.Tick()` 실행.
- `ResourceBootstrap.Tick()` → `GameResources.Poll()` → `LocalizationSettings.InitializationOperation`.
- 실제 게임의 `Unity.Localization.dll`에서 `InitializationOperation` getter는 `Instance.GetInitializationOperation()`을 호출하고, 기존 핸들이 없으면 **`ResourceManager.StartOperation(...)`으로 새 로컬라이제이션 초기화 작업을 시작**함. 따라서 단순한 준비 상태 조회가 아니라 초기화 자체를 앞당기는 호출임.
- `GameResources.Poll()`은 그 뒤 선택 언어의 `Mechanic` 테이블을 로드하고 `mech_condition_pure_water`의 존재를 검증.
- `ResourceBootstrap.Tick()`은 폴링 중 예외가 나면 `Detach` + abort 콜백을 실행하고 **예외를 다시 던짐**. 일시적인 테이블/키 문제에서도 자체적인 재시도가 없음.
- 게임 `WaterFeatureHelper`의 첫 정적 필드가 `LocHelper.GetLocalizedMechanic("mech_condition_pure_water")` 호출. 다른 모드의 Harmony 패치가 이 타입의 정적 초기화를 조기에 실행하면 게임 쪽은 동기 `GetLocalizedString`을 호출하여 아직 초기화되지 않은 로컬라이제이션 작업과 충돌할 수 있음.

**확정한 설계상 원인:** CorpPresence가 **모드 자산 로딩 전** Unity Localization 초기화 작업을 시작시키는 코드 경로가 존재하며, 테이블 키 검사가 일시적으로 실패해도 해당 모드 로딩을 예외로 종료하는 취약성이 있음. 앞서 추측한 수준의 단순 `SelectedLocale` 접근 문제가 아니라, 실제 Unity DLL에서 초기화 작업 생성까지 확인함.

**여전히 구별해야 할 항목:** 사용자 실험의 세 조합에서 충돌이 모두 해당 순서 문제로 *실제로 발생했는지*는 실행 중 CorpPresence가 로드된 로그 없이는 확정할 수 없음. 기존 `Player(1).log`의 CorpPresence 0.9.1은 로드 거부돼 해당 오류와 인과관계가 없음.

### 필요한 해결 변경

1. `OnEnable()`에서 `_bootstrap.Tick()` 즉시 실행 제거; `ResourceBootstrap`의 초기 준비 검사를 **모드 자산 로딩이 끝난 이후** 시작.
2. `OnModItemDirectoryInit` 콜백에서도 조기 `Poll()` 진입을 허용하지 않도록 이벤트 순서 및 별도의 시작 플래그 보장.
3. `GameResources.Poll()`의 `Mechanic` 테이블 키 검사를 선택 로케일의 단일 키를 기준으로 무조건 실패시키지 않도록 기본 로케일 검증/fallback 또는 복구 가능한 retry 처리.
4. `Poll()`의 초기화 실패 시 `ResourceBootstrap.Tick()`의 무조건 abort/rethrow 경로를 분리하여 영구적인 실패에서만 abort.
5. 원본 CorpPresence DLL은 변경하지 않은 상태임. 변경 시에는 별도의 명확한 테스트 후보로 만들고 게임 내 충돌 조합을 재현해 효과를 검증할 필요가 있음.


## 7. 실제 게임 모드 로더의 추가 확인 (2026-11-04)

`ModLoader.LoadModAssets()`(본체 `Assembly-CSharp.dll` 1218줄 이후)은 활성 모드를 순회하며 **`LoadTextures(mod)` → `LoadLocalization(mod)` → `ModLootFile.Load(mod)` → `PaperBookRegistry.LoadMod(mod)`** 순서로 호출한다.

`ModLoader.LoadModDll()`(1132줄 이후)의 `mod.OnEnable()` 호출(1165줄)은 이보다 앞에서 실행된다. 즉 CorpPresence의 `OnEnable` → `_bootstrap.Tick` → `GameResources.Poll`가 실행되면 **게임은 아직 모드 Localization CSV를 로드하지 않은 상태**다.

유니티 `LocalizationSettings.GetInitializationOperation()` 220–232줄은 기존 핸들이 유효하지 않으면 `AddressablesInterface.ResourceManager.StartOperation(...)`으로 초기화를 실제 시작한다. 단순히 초기화 완료 여부를 검사하는 무해한 접근이 아니다.

게임 `WaterFeatureHelper`는 타입 정적 필드에서 **즉시** `LocHelper.GetLocalizedMechanic("mech_condition_pure_water")`를 호출한다. 따라서 이 클래스에 Harmony detour를 설치하면서 타입 초기화가 발생하는 경우, 조기 Localization 초기화와 동기 조회가 겹칠 수 있다.

### 수정 우선순위

**1차 최소 변경:**
- `Main.OnEnable`에서 `_bootstrap.Tick()`의 즉시 호출을 제거.
- `ResourceBootstrap.Configure`의 `OnModItemDirectoryInit`에서 수행하는 즉시 `Tick()`도 제거/지연.
- `ResourceBootstrap.Update()`에서 **첫 Unity 프레임 이후** 단계적으로 `GameResources.Poll()` 실행해 Loader의 `Awake → LoadModAssets`가 끝난 뒤 Localization을 시작.
- `_pendingDirectory`는 기존 `ItemRegistry`가 이미 보존하므로 자산 준비 후 `ItemRegistry.Activate()` 처리 가능.

**2차 안전성 변경:**
- 선택 언어의 테이블 키가 아직 존재하지 않는다는 사유만으로 첫 실패 시 `Abort + rethrow`하지 않도록 실패 유형 분리.
- 키/테이블이 실제로 없는 경우에는 기본 언어 확인과 제한된 재시도 후 구체적인 로그 작성.

이 내용은 정적 코드 검사로 확인한 **실행 순서 문제에 대한 수정 설계**다. 수정 DLL을 빌드하거나 게임에서 시험하지 않았으며, 3개 조합의 실제 실패가 모두 이 경로에서 시작하는지까지는 정적 분석으로 확정할 수 없다.
