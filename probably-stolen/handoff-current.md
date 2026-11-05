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


---

# 최우선 최신 인수인계 — 2026-11-05: NoHooks 전환 확정 (이전 지침보다 우선)

> **이 섹션이 현재 작업 기준이다.** 위에 남겨둔 2026-11-03~04 기록의 '런타임 번역 후킹 유지/개선' 또는 '타 모드 로컬 DLL 번역 절대 금지' 방침은 이번 사용자 결정으로 변경되었다. 이전 내용은 역사적 참고용으로 보존한다.

## 사용자 확정 목표

1. **한국어팩의 런타임 번역 후킹을 전부 폐기한다.** 프레임마다 출력·UI·아이템·방문객 등을 가로채 번역하지 않는다. 성능 안정성을 번역 적용률보다 우선한다.
2. **NoHooks DLL을 새 기준으로 사용한다.** 기존 후킹 기반 DLL로 되돌리지 않는다.
3. **게임이 공식적으로 읽는 Localization 데이터 기반 번역**을 먼저 복구·확장한다. 게임 로더와 `LoadedMod.Localization` / `ModHelper.GetLocalized`의 정상 경로를 우선 사용한다. 가능한 경우 새 모드는 한국어팩 CSV 추가만으로 지원한다.
4. **하드코딩으로 인해 이 방식만으로 번역이 불가능한 필수 모드는 예외적으로 로컬 모드화**하여 해당 모드의 DLL 문자열을 직접 번역한다. 워크샵 모드 자동 업데이트를 그대로 적용하지 않고 로컬 번역본을 사용하며, 원작 업데이트는 **필요해졌을 때 대응**한다. 자동 패치 시스템을 우선 개발하지 않는다.
5. 제3자 모드 DLL 변경은 **해당 모드의 로컬 번역 작업을 사용자가 지정·승인했을 때만** 진행한다. 원래 워크샵 모드를 무단 수정하거나, 테스트 명목으로 제3자 DLL을 고치는 일은 금지한다. 원본 DLL/자산과 세이브를 보존하고, 내부 ID·메서드명·필드명·직렬화 키는 건드리지 않는다.
6. 기존 번역 CSV는 후킹 제거로 적용되지 않는 `S`/`S64` 항목도 **삭제하지 않고** 향후 로컬 DLL 번역의 대응표로 보존한다. `S64`는 객체 ID가 아니라 **원문 문자열의 Base64URL 인코딩**이며, 그 형식 자체는 출력 시 번역을 수행하지 않는다. `K`/`K64`는 Localization 키 기반이다.
7. '완료'는 필요한 번역과 검증을 마쳤을 때에만 사용한다. 후킹 제거로 발생한 미번역은 별도 후속 작업이다.

## 성능 문제 및 게임 내 검증 — 사용자 직접 확인

- **증상:** AI 상점 방문객 GPT(워크숍 `3832941698`) 및 세력 평판 확장 '상층 구역 특별 귀빈'(`3828971731`) 등 모드 방문객이 화면에 **머무르는 동안 지속적으로 렉** 발생.
- 인물 미화팩(`3828971372`)을 꺼도 렉 지속.
- 초상화 런타임(`3831627217`)을 꺼도 렉 지속. F11 진단은 아무것도 나타나지 않음.
- **기존 한국어팩을 끄자 렉 해소.**
- **후킹 제거 NoHooks DLL을 적용하자 렉이 확실히 사라졌다고 사용자가 확인함.** 번역 일부가 풀리는 것은 예상 결과이며, 이번 실험의 핵심 목적은 렉 제거였다.
- 결론: **기존 한국어팩의 런타임 번역 후킹이 해당 렉을 유발했다는 강력한 비교 검증 결과.** 개별 병목 메서드는 아직 프로파일러로 특정되지 않았고, 모든 환경에서의 성능 보장은 별도 검증 사항이다. 기존 후킹 복원·추가 금지.

## 실제 빌드·파일 (GitHub)

- 원본 기준 입력 DLL: `probably-stolen/0.7.5.21-dev/ProbablyStolenKoreanPack.dll` (원본 보존).
- **NoHooks 산출물:** `probably-stolen/build/out/ProbablyStolenKoreanPack.NoHooks.0.7.5.21-dev.dll` (**97,280바이트**).
- 다운로드: https://github.com/okkoog/sub-memory/blob/main/probably-stolen/build/out/ProbablyStolenKoreanPack.NoHooks.0.7.5.21-dev.dll
- 검증 기록: `probably-stolen/build/out/ProbablyStolenKoreanPack.NoHooks.audit.txt`.
- 재현 가능한 패처: `probably-stolen/tools/nohooks/Program.cs`, `KoreanPack.NoHooks.csproj`.
- 빌드 워크플로: `.github/workflows/build-koreanpack-nohooks.yml`.
- Mono.Cecil로 DLL 내 후킹 등록 진입점 **6개를 안전한 no-op으로 교체**하고, Harmony `Patch` 호출 경로 **2개 → 0개**를 검사했다. IL에서 후킹 메서드 정의 자체를 제거한 것은 아니다. 원본 DLL 및 제3자 DLL은 변경하지 않았다.
- Game Localization 병합 메서드 존재 여부는 정적으로 확인했으며, 실제 모드별 번역 적용률은 계속 점검해야 한다.
- 설치 시 출력 DLL을 `ProbablyStolenKoreanPack.dll`로 이름을 바꿔 기존 DLL을 백업하고 교체한다.

## 2026-11-05 `mods/` 실사 결과 (29개 모드)

- 자체 `Localization/` 언어 CSV가 확인된 모드 10개:
  `3828925546`, `3829053332`, `3829429358`, `3829730904`, `3831429588`(원본 ko.csv 있음), `3831752512`, `3832253334`, `3832840208`, `3833572288`, `3833616855`.
- 별도의 번역 전용 모드 1개: `3828891515` (`Data/ko.tsv`, `KoreanTranslation.dll`).
- 자체 `Localization/` 폴더가 없는 모드 18개:
  `3828902092`, `3828910575`, `3828913695`, `3828971372`, `3828971731`, `3829085054`, `3829267478`, `3829413447`, `3829672185`, `3830874694`, `3831627217`, `3831771576`, `3831791299`, `3831791602`, `3831837821`, `3832048841`, `3832941698`, `3833095213`.
- 빈 `dkv` 디렉터리 제외.
- **이 수치는 폴더 구성 및 일부 DLL의 로컬라이저 API 확인에 따른 1차 분류다.** 언어별 CSV가 있더라도 모든 화면 출력이 그 경로를 이용하는지 확정할 수 없고, CSV가 없더라도 자체 번역 함수·JSON 등을 사용할 수 있다. 최종 판정은 실제 출력 경로 확인 및 게임 내 검증으로 한다.

## 후속 작업 순서

1. NoHooks DLL로 기존 게임 성능을 계속 유지하며 기준점 고정.
2. 대상 모드별 `K/K64` 및 정적 Localization 경로가 실제 적용되는지 확인, 누락분 번역.
3. `S/S64`에만 의존하던 출력은 자동 해결된다고 주장하지 말고 **미번역 현황**으로 분류.
4. 게임 공식 Localization으로 해결되지 않는 필수 하드코딩 모드만 **사용자의 개별 지시에 따라** 로컬 DLL 번역.
5. 번역 적용 여부, 게임 로딩/방문객 성능, 세이브 호환성 확인. 후킹 복원 금지.

**현재 완료 판정:** 사용자 테스트에서 **NoHooks 적용 후 방문객 지속 렉 해소 확인**. 전체 모드의 번역 복구 및 하드코딩 모드의 로컬 번역은 **아직 미완료**.


## 2026-11-05 — CorpPresence NoHooks 일회성 배열 번역 시험 실패 (스캐너 차단)

- 시험용 별도 DLL `probably-stolen/build/out/ProbablyStolenKoreanPack.NoHooks.CorpMenuTrial.dll` (**97,792바이트**)은 빌드되었지만 **게임 ModScanner가 사용을 거부함**. 사용 금지. 이 시험판을 '적용 성공'으로 보고하지 말 것.
- 사용자가 제공한 스캐너 화면에는 `unlisted-reference: ProbablyStolenKoreanPack.dll -> references System.Private.CoreLib not listed in <Assemblies>` 및 `reflection-external` 위반(계산된 타입/어셈블리에 대한 `GetField`/`GetType`)이 표시됨. 실행 횟수가 1회인지 여부는 해당 정적 검사에 영향을 주지 않음.
- Corp DLL 0.9.6 메타데이터에서 `CorpPresence.UI.MenuLabels.Chinese` / `English` 정적 문자열 배열과 이를 직접 반환하는 `MenuLabels.Get`을 확인했으나, 해당 정적 데이터에 대한 **현재 방식의 리플렉션 접근은 스캐너 제한에 부적합**.
- 재검토 우선순위: 게임에서 허용하는 공식 ModHook/Localization 경로만 사용하는 접근. 다른 모드 DLL 수정은 사용자 별도 지시 전 금지.
- **유효한 안정 기준은 이전 `ProbablyStolenKoreanPack.NoHooks.0.7.5.21-dev.dll`**. CorpMenuTrial 시험판으로 교체하지 말 것.


---

## 2026-11-05 — 번역 우선순위 변경 및 Localization 키 보강

**사용자 최신 확정 지시:** CorpPresence처럼 직접 표시 문자열을 내보내 공식 Localization만으로 대응하기 어려운 모드는 **후순위**로 미루고, **원본 모드가 정식 Localization 키를 사용하는 모드 10개**부터 완성할 것. NoHooks DLL 기준과 런타임 출력 후킹 금지 원칙은 유지.

원본 `probably-stolen/mods/<ID>/Localization/*.csv`와 현행 `0.7.5.21-dev/Localization/pskb__<ID>.csv`의 키를 직접 대조해 다음을 **GitHub에 실제 반영**:
- `3829730904` 양조 확장: 기존 420개에서 누락된 `K` 번역 **97개 추가**, 현재 517/517 원본 키 확보. NPC 니니/포티세븐, 외곽 지역 평판·특성, 시장 이벤트, 케이블 표시 포함. 커밋 `3b5525ee2f908ae42552dd2092a87b0f4369518c`.
- `3832253334` 신문 읽고 버리기: 신규 `pskb__3832253334.csv` 생성, `K|full` 번역 1개 추가. 커밋 `b9d40ff86c77cdb227646256fe709df686db21d3`.
- `3831752512` 동방 카세트팩: 원본의 `cassette.desc`, `pack.desc` 값이 **빈 문자열**이라 번역 텍스트를 임의 생성하지 않음.
- 나머지 Localization 모드 7개는 기존 한국어팩에 원본 키가 이미 모두 존재함.

**10개 모드 합계: 원본 키 1,248개 중 한국어팩 `K` 키 1,246개. 미포함 2개는 원본 내용이 빈 키.** 현재 번역 데이터 상에서 비어 있지 않은 키의 대조 완료. 이는 CSV 키 대조 결과이며 실제 게임 출력 경로/자연스러운 번역·성능 테스트 완료를 뜻하지 않는다.

현재 Corp 전용 NoHooks 시험판은 **스캐너 차단 실패작**으로 사용하지 않으며, 안정 NoHooks DLL은 보존. 이후에는 공식 Localization 키 변경/신규 내용 추적 및 실제 게임 내 적용 확인을 우선한다. 현행 KoreanPack DLL 재빌드 불필요: 위 변경은 번역 CSV만 갱신.


---

## 2026-11-05 — CorpPresence v0.9.6 로컬 DLL 번역 시험본 제작

**사용자 최신 지시**: 모더가 공식 Localization 지원을 안정판 이후로 미뤘으므로, 서브메모리에 올라온 **CorpPresence 0.9.6**을 이번 로컬 번역 기준으로 확정. 최초 워크숍 모드 파일은 수정하지 않고, 로컬 복사본 DLL 자체의 표시 문자열을 번역한다.

- 원본: `probably-stolen/mods/3828913695/CorpPresence.dll` 및 원본 `manifest.xml` (ID `CorpPresence`, 버전 `0.9.6`).
- 입력 DLL SHA-256: `044627f6a9232691db6dbac38e509b676682cebed043b6cbd0ecc9b442935b81` (6,971,392바이트).
- 기존 번역 데이터: `0.7.5.21-dev/Localization/pskb__3828913695.csv` + `pskb__supp_3828913695.csv`. 유효 원문→한국어 매핑 2,728개; 충돌 21건은 보충본 우선.
- Mono.Cecil 패처 소스: `probably-stolen/tools/corp-local-translation/Program.cs`, 빌드: `.github/workflows/build-corp-local-ko.yml`.
- **번역된 로컬 DLL:** `probably-stolen/localized/CorpPresence-0.9.6-KO/CorpPresence/CorpPresence.dll` (6,902,272바이트). SHA-256 `890f7ec4830c75c9fdf7addd8b43a43540162f702718d4412eddfa1a4118a31a`.
- **설치 ZIP:** `probably-stolen/localized/CorpPresence-0.9.6-KO-Local.zip`. 구성: `CorpPresence/CorpPresence.dll`, `CorpPresence/manifest.xml`, `README_KO.txt`, `audit.txt`.
- 실행 파일 문자열 리터럴 **2,837곳**을 치환(중국어 1,611, 영어 1,224); 바이너리 리드백 성공. 번역 적용 메서드 386개. 원본 게임·제3자 모드 파일 불변.
- 리소스명/아이템 등록/세이브/초상화 경로를 포함할 수 있는 메서드를 번역 범위에서 제외해 식별자 훼손을 피했음. 남은 CJK 리터럴 273회(고유 248개)에는 디버깅·로그 및 일부 표시 문구가 섞여 있음. **모든 화면 번역 완료 아님.**
- **게임 내 로드/스캐너/세이브/번역 적용 시험은 아직 사용자 쪽에서 하지 않았음.** 정적 빌드·IL 읽기 검증까지만 성공. 게임 로더의 로컬 모드/워크숍 중복 로딩 우선순위도 실제 목록에서 반드시 확인해야 함.
- 기존 NoHooks 한국어팩 DLL은 변경하지 않았다. Corp 로컬화가 공식 Localization 지원으로 대체되면 정식 구조로 회귀.
- 새 ZIP/시험 DLL은 사용자 테스트용으로 제공 가능하되, 완전 검증됐다는 표현 금지.


---

# 2026-11-06 — 번역 품질 검수 중단 및 작업 관리 인수인계 (사용자 지시)

> **최신 작업 지시: 번역 검수·추가 수정은 즉시 중단하고, 작업 상태를 sub-memory에 먼저 기록할 것.** 사용자가 재개를 명시하기 전에는 CSV 교정/무작위 오역 탐색/모드 분석을 계속하지 않는다. 이 기록은 인수인계용이며 번역 검수 완료 선언이 아니다.

## 작업 목표와 완료 판정

- 대상: `probably-stolen/0.8.0/Localization/`의 **현재 17개 CSV 전체**. 오래된 `0.7.5.21-dev`나 역사적 기준 패키지와 혼동하지 말 것.
- 완료 기준: (1) 각 CSV의 번역 대상 키 전체 목록 고정, (2) 항목별 원문·한국어 문장을 실제로 대조하여 오역/의미·수치 누락/부자연스러운 표현/명칭·말투 불일치 검사, (3) 발견된 오류 수정, (4) CSV 구조·중복 키·변수 검사, (5) GitHub 반영 및 변경 내용 검증. **검수했지만 변경하지 않은 키도 반드시 검수 완료로 기록한다.**
- 원문이 제공되지 않는 `S/S64` 해시형 문자열·DLL 전용 문구 등은 `원문 확보 불가`로 분류하고, 원문 대조 가능 항목과 분리한다. 이 때문에 가능한 항목의 검수를 무기한 미완료로 두지 않는다.
- **게임 실행·인게임 출력 검증은 이 번역 CSV 품질 검수의 목표·완료 조건에서 제외하며 보고에 반복해서 언급하지 말 것.** 사용자가 명시적으로 요청하지 않는 한 작업 범위를 확대하지 않는다.
- 제3자 원본 모드·DLL은 수정하지 않는다. 이 업무는 한국어팩 CSV 검수다.

## 중단 시점 상태 (정확히 구분)

- `0.8.0/Localization/`에 **17개 CSV**, 이전 검수에서 **총 2,049개 레코드**로 계수. 전체 CSV 구문·중복 키·열 개수 검사는 0건 오류로 보고됨. 이 수치는 **수작업 의미 검수가 끝난 항목 수가 아니다.**
- 원본 CSV가 있는 **10개 모드/1,873개 키**는 키·변수 대조를 수행했다고 보고됨. 번역 파일 중 두 원본 키는 값이 비어 있어 대응 번역을 생성하지 않은 것으로 확인됨. 이 또한 의미 검수 완료 수치가 아니다.
- 양조 확장(`pskb__3829730904.csv`)과 담배 확장(`pskb__3832840208.csv`)의 상당수 번역을 여러 차례 교정함. 양조 제조법 55개 조합의 재료·결과물 대응에 대해 별도 검사를 진행했고 당시 불일치 0건. 촉수(`pskb__3831837821.csv`) 이름 데이터는 13개 지역/5,160개 항목의 배열 개수·순서 동일 여부 확인 후 일부 음역 교정.
- AI 상점(`pskb__3832941698.csv`), 마도카(`pskb__3833572288.csv`), 결속 밴드(`pskb__3833616855.csv`), 일부 소규모 모드도 수정함.
- 마지막까지 제시된 번역 변경 커밋 중 하나는 `015f250339e8e0850351da20b6b817dd3628e96b`이며, **그 이후 실제 최신 커밋 SHA는 재개 시 저장소에서 새로 읽어야 한다**. 현재 파일 내용 자체를 작업 기준으로 삼을 것.
- **검수 완료한 키별 체크리스트가 전혀 작성되지 않았다.** 검수 중 문제 없었던 키를 추적하지 않아, 전체 2,049개 중 *의미 검수 완료/미완료 개수와 진행률을 근거 있게 산출할 수 없음*. 이미 수정한 키를 전체 검수 완료로 간주하지 말 것. 작업 상태는 **중단/품질 전수 검수 미완료**.

## 기존 작업 방식의 실패 원인과 재발 방지

이전 작업은 눈에 띄는 오역을 반복 검색·수정하면서 키별 검사 이력을 남기지 않았고, 완료 판정이나 남은 물량을 제시하지 못했다. 사용자가 이 문제를 명시적으로 지적했다. 이후 **무작위 교정부터 재개하면 안 된다.**

재개 시 다음 관리 원칙을 먼저 구현/확정할 것.

1. 17개 CSV의 항목을 파일명+원문 키로 고유 식별하여 전수 목록화. `원문 비교 가능`/`원문 확보 불가`/`원본 빈 문자열` 구분.
2. 각 키마다 `미검수`, `검수 완료·수정 불필요`, `수정 완료·재검증 완료`, `원문 확보 불가` 상태를 영속적으로 기록. 문장 검토 전에는 검수 완료로 표시하지 말 것.
3. 파일별 총 대상 수/검수 완료 수/남은 수/수정 수 및 근거(원문 경로·GitHub 커밋)를 기록. **키·변수·CSV 자동 검사가 통과했다고 문장 품질 검수가 완료됐다고 주장하지 않는다.**
4. 장시간 또는 대화 단절 후에도 바로 이어갈 수 있도록, 수행 중 세션이 끝나기 전에 sub-memory에 현재 체크포인트를 갱신한다. 진행률을 기록하지 못한 과거 교정 구간은 검수 완료 처리하지 않는다.
5. 사용자가 **재개를 지시하기 전에는 여기서 중단**. 저장소의 기존 번역 파일에 이번 인수인계 작업을 핑계로 추가 변경을 가하지 않는다.

## 이번 중단 요청 처리 범위

- 이번 턴에서 기록해야 하는 것은 **본 문서 업데이트만**이다. CSV 번역 작업을 다시 시작하지 않는다.
- 기존 인수인계의 오래된 버전·적용률·DLL 개발 내용보다 **현재 요청의 중단 및 품질 검수 관리 규칙이 우선**한다.
