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
