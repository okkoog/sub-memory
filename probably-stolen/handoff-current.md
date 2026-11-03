# Probably Stolen — 한국어팩 개발 인수인계
마지막 업데이트: 2026-11-04 (KST)
상태: **미완료 / v0.6.8 빌드·정적 검증 완료 / 게임 내 런타임 검증 대기**

## 목표
하나의 KoreanPack DLL과 자체 `Localization/<WorkshopModID>/ko.csv` 모음으로 여러 Probably Stolen 워크샵 모드를 한국어화한다.

- 원본 모드의 DLL·자산 **수정 금지**
- 모드별 별도 DLL **생성 금지**
- 게임이 제공하는 `ModLoader.LoadLocalization`, `LoadedMod.Localization`, `ModHelper.GetLocalized` 우선
- 불특정 전역 UI·파일·리소스 읽기 후킹 금지
- 새 모드는 가능하면 CSV 추가만으로 지원
- 기존 번역 데이터 약 16개 모드/12,740행 보존
- **모든 필수 요구 구현+빌드+검증이 끝나기 전에는 완료로 보고하지 않는다**

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


## 2026-11-04 — Sol 재구축 v0.6.8-dev

작업 브랜치: `probably-stolen-koreanpack-sol` — `main` 미수정.

### 확인 완료
- 게임 `ModLoader` 역컴파일로 실제 순서 확인: 각 모드 `Init/OnEnable` 완료 후 `LoadModAssets -> LoadLocalization`.
- `ModHelper.GetLocalized(string modId, string key, params object[] args)` 실제 시그니처 확인.
- 초기 `LocHelper.GetCurrentLocaleCode()`가 `LocalizationSettings.SelectedLocale`을 읽는 것을 확인.
- v0.6.8은 공식 `LoadLocalization` 이전에는 ES3 저장 locale만 읽고, 이후에만 SelectedLocale 접근.
- 타 모드 생성자/.cctor 패치 없음.
- Harmony transpiler/IL 문자열 치환 없음.
- `AssemblyLoad` 선제 타 모드 패치 없음.
- TMP/TextMeshPro 전역 후킹 없음.
- File/embedded-resource 전역 후킹 없음.
- 타 모드 메서드 패치는 `LoadLocalization` 이후, 번역 catalog가 연결된 해당 assembly에만 적용.
- 초기 local-mod manifest-ID fallback은 게임의 private `loadingMod`을 식별용으로만 읽으며, caller assembly가 실제 loadingMod 폴더 아래일 때만 허용.
- 기존 v0.5.44 번역 16 CSV / 12,740행 보존. CSV 파싱 오류 0. 16개 파일 SHA-256 모두 기존과 동일.
- 대표 DLL 정적 분석: CorpPresence / ProficiencyMod / ItemStorage / PCExpansion / GameCardPK.
- GitHub Actions v0.6.8 컴파일 성공.
- 개발 브랜치에 게시된 DLL과 사용자 테스트 ZIP의 DLL은 동일 파일:
  `SHA-256 c7be62021a8f1638f079f34e60180f3985d20da32824e63b932a17e3141fac86`.

### 현재 테스트 패키지
- `ProbablyStolen_KoreanPack_v0.6.8-dev_Install.zip`
- ZIP SHA-256: `a71d534cb9d6db1b45db05ed5065c096416bf79a1fe8b7bbae819339778ae733`
- ZIP 무결성 검사 통과.
- 실제 게임 실행 검증 전이므로 **완료/릴리즈 판정 금지**.

### 다음 단계
1. 기존 KoreanPack 폴더를 v0.6.8-dev 테스트본으로 교체.
2. 먼저 한국어팩 단독 로딩/게임 시작 확인.
3. 이후 기존 충돌 조합(CorpPresence+한국어팩, CorpPresence+Proficiency, CorpPresence+PCExpansion)을 포함해 로딩 안정성 확인.
4. native key 번역 모드와 text 기반 모드 양쪽에서 실제 한국어 적용 여부 확인.
5. 실패 시 Player.log 기준으로 범위를 좁혀 수정. 타 모드 파일 자체는 수정하지 않는다.
