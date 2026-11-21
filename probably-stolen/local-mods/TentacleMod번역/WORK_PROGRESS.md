# 촉수 모드 한국어 번역 — 작업 기록

> **업데이트: 2026-11-21 (KST)** · 이 파일은 다음 채팅에서 이어서 작업할 때 가장 먼저 읽을 인수인계 기록이다.
>
> **원본은 절대 수정하지 않는다.** 원본: `probably-stolen/local-mods/TentacleMod/`; 번역본: `probably-stolen/local-mods/TentacleMod번역/`.

## 작업 목적과 원칙

- 게임 Probably Stolen의 로컬 촉수 모드와 관련 확장 모드들을 한국어로 번역한다.
- Localization 파일이 있는 모드는 CSV를 먼저 활용한다. 공식 Localization을 사용하지 않는 화면 문자열만 DLL에서 수정한다.
- DLL 내에서 식별자·세이브 키·정규식·파일 경로·호환성 상수·타 모드 API 문자열은 번역 대상에서 제외한다.
- 대사/설명은 원문 1:1 문맥 검수. 번역이 기능을 바꾸거나 저장 데이터를 깨뜨리면 안 된다.
- 미검증 DLL은 완료로 표시하지 않는다. PE/CLR 구조 검사와 **실제 게임 실행 검증**을 구별한다.
- **이 파일을 작업 단위마다 업데이트**하여 대상, 처리 건수, 남은 범위, 검증 상태, Git 커밋/파일 경로를 남긴다.

## 대상 및 확보 현황

| 원본 폴더 | 원본 또는 자료 확인 | 작업 상태 |
| --- | --- | --- |
| `lewd_meme2.1.0/` | `MidnightPawn.dll`, `manifest.xml` (mod v2.1.0), `Data/物品定义.csv` | **1차 한국어 DLL 시험본 업로드**: 64개 문자열, 74곳 참조 수정; 게임 실행 테스트 미실시 |
| `LewdMemeShadowTrade1.6.4/` | `ShadowTrade.dll`, 문서/데이터 | **1차 한국어 DLL 시험본 업로드**: 59개 문자열, 65곳 참조 수정; 게임 실행 테스트 미실시 |
| `LewdMemeBrothel-0.10.4-candidate/` | DLL 및 `Localization/en.csv`, `zh.csv` | **한국어 CSV 178개 키 전수 번역 완료(문자열 구조 검사 통과)**, 실제 게임 적용 미검증 |
| `lewdMemeBrood v0331/` | DLL, `Localization/`, `Content/`, manifest v0.3.31 | **Localization 566키 전수 번역**, 상호작용 JSON 135개 중 **125개 번역, 10개 보류** |
| `LewdMemeLegacyContent-1.0.6-48sounds/` | DLL, `Content/content.json` | 번역 적용 방식 검토 필요 |
| `ItemPortraitHD_v1.3.0/` | DLL, C# Source/, 설정 설명 | **1차 한국어 DLL 시험본 업로드**: 82개 문자열, 105곳 참조 수정; 실행 테스트 미실시 |
| `RedLightDistrict_v1.1.1/` | 대용량 DLL, Git LFS 포인터 | 실제 LFS 바이너리를 확보한 뒤 분석 필요 |
| `可能是成人用品 v1.1.3/` | 공식 Localization `zh.csv`/`en.csv`, manifest 내부 버전 `1.03` | **한국어 `ko.csv` 192개 키 유지; 189개 번역·3개 원문 유지**, 구조 검사 통과. 런타임 미검증 |
| `lewd_meme_config0.7.2-pre/` | DLL + C# Source/ | 설정 표시문구 조사 착수: `PanelKeys.cs` 라벨 182개·섹션 13개·툴팁 99개, `FamilyPanelKeys.cs` 라벨 93개·툴팁 93개. 아직 DLL 번역본 없음. |

| 기타 이미지 교체 패키지 | manifest, 이미지 등 | 문구 여부 확인 후 필요한 경우만 번역 |

## 실제 작성 파일 (모두 `TentacleMod번역/` 기준)

| 모드 | 산출물 | 번역 실적 | 상태 |
| --- | --- | --- | --- |
| 촉수 본체 | `lewd_meme2.1.0/MidnightPawn.dll` | 문자열 64개/IL 참조 74곳 | 시험 DLL, 게임 테스트 미실시 |
| 암거래 확장 | `LewdMemeShadowTrade1.6.4/ShadowTrade.dll` | 문자열 59개/IL 참조 65곳 | 시험 DLL, 게임 테스트 미실시 |
| 고해상도 초상화 | `ItemPortraitHD_v1.3.0/ItemPortraitHD.dll` | 문자열 82개/IL 참조 105곳 | 시험 DLL, 게임 테스트 미실시 |
| 매음굴 확장 | `LewdMemeBrothel-0.10.4-candidate/Localization/ko.csv` | 원문 178키 모두 번역 | CSV 구조 검사 통과, 실제 적용 미검증 |
| 촉수 번식 확장 | `lewdMemeBrood v0331/Localization/ko.csv` | 원문 566키 모두 번역 | CSV 구조 검사 통과, 실제 적용 미검증 |
| 촉수 번식 상호작용 | `lewdMemeBrood v0331/Content/interactionLines.json` | 135줄 중 125개 대사 번역, 10개 보류 | JSON 구조·치환 변수 검사 통과, 실제 적용 미검증 |

- DLL 문자열 번역 합계 **205개**, 변경된 IL 문자열 참조 **244곳**. 공식 CSV 번역 **744개 키**(Brothel 178 + Brood 566), 외부 JSON 번역 **125개 대사**는 별도 합산.
- 원문/번역 대조표: `번역대조표/MidnightPawn.json` (78쌍), `번역대조표/ShadowTrade.json` (75쌍), `번역대조표/ItemPortraitHD.json` (94쌍). 대조표의 전체 행이 DLL에 반영된 것은 아님.
- 각 JSON에는 기능에 영향을 줄 수 있어 보류한 원문도 포함되어 있음. 정확한 반영 수는 이 문서의 DLL별 기록을 따를 것.
- DLL과 CSV 모두 게임 실행/표시 테스트 전 결과물이다. 번역 품질에 대한 2차 문맥·용어 검수도 필요하다.

## DLL 시험본 결과 (2026-11-21)

- **GitHub 커밋**: `21fe012aa4ef9eb0ceaef15a8585818e8cca65a1`
- **수정본 경로**: `lewd_meme2.1.0/MidnightPawn.dll` (모두 이 `TentacleMod번역` 폴더 기준)
- **원본 DLL**: `TentacleMod/lewd_meme2.1.0/MidnightPawn.dll` (Git blob SHA `c27be30741564a51f57495a06681974bcc9421ca`)
- **초안 대조표**: 총 78쌍 중 64쌍을 DLL `ldstr` 참조에 반영했고, 동일 문자열을 참조한 74곳을 변경함
- **보류 14개**: `无`, `侵染`, `微荧`, `明耀`, `恒昼`, `触手`, `侵蚀`, `闹钟`, `客人`, `统计`, `查看`, `生长`, `已拥有`, `未获得`. 내부 로직의 식별자나 비교 문자열로 이용할 가능성이 있어 사용처 검증 전 치환 보류
- **수정 방식**: 원본 `#US` 힙을 보존하며 새 한국어 문자열을 별도 메타데이터 영역으로 재배치하고, `ldstr` IL 토큰만 변경. 원본 184,320바이트 → 수정본 313,344바이트
- **구조 검사**: 새 `#US` 문자열을 다시 읽고 변경된 IL 토큰 74개가 정확한 오프셋을 가리키는지 확인. **실행/로딩 검사는 하지 못함**. PE/CLR 로더와 게임 내 UI/기능 테스트가 반드시 필요
- **주의**: 후보 DLL이 정상 로딩되어도 모든 화면 문구가 번역된 것이 아님. 중국어 문자열 다수가 그대로 남아 있음. 원본은 변경하지 않았음

### ShadowTrade.dll — 1차 시험본

- **커밋**: `88b114cb9745e12ca0bd9067e471bdb9e6cfc421`
- **수정본**: `LewdMemeShadowTrade1.6.4/ShadowTrade.dll`
- **원본 blob SHA**: `c400f9dc58750d6b8494725e91e159626611c41c`
- **대조표**: `번역대조표/ShadowTrade.json` (75쌍 중 59쌍 DLL 반영, 16쌍 내부 비교/식별 가능성으로 보류)
- **검증**: 한국어 문자열 59개, IL 참조 65개, 메타데이터 크기 및 토큰 검사 통과. 원본 183,808 → 327,168바이트. **게임 내 로딩/동작 미검증**.


### ItemPortraitHD.dll — 1차 시험본

- **커밋**: `8a1e73e8fc0f15c56028c4b25ae73116fc47d85a`
- **수정본**: `ItemPortraitHD_v1.3.0/ItemPortraitHD.dll`
- **원본 blob SHA**: `57ecff396d96c3f959b08ff775c1e373d3796159`
- **대조표**: `번역대조표/ItemPortraitHD.json` (94쌍 중 82쌍 DLL 반영, 12쌍 내부 식별자/비교 가능성으로 보류)
- **검증**: 한국어 문자열 82개, IL 참조 105개, 메타데이터 크기 및 토큰 검사 통과. 원본 169,984 → 275,968바이트. **게임 내 로딩/동작 미검증**.
- **주의**: 원본 C# 소스가 존재하므로 차후에는 소스 기반으로 더 정확하게 UI 문자열과 내부 판정 문자열을 분리하고 재컴파일하는 방식을 우선 검토한다.

### LewdMemeBrothel — 공식 Localization 번역

- **커밋**: `8b82cb8e6f1679dd4458a833fdf0b6d8d45d0bb7`
- **번역본**: `LewdMemeBrothel-0.10.4-candidate/Localization/ko.csv`
- **검수 범위**: `Localization/zh.csv` 178개 전체 키를 한국어로 번역. 178개 전체 키 수/키 일치, 플레이스홀더 `{0}` 형식, `\\n` 표기 수 일치 확인(차이 0).
- **미검증**: 모드가 자동으로 `ko.csv`를 읽는지, 한글이 게임 화면에 적용되는지 실제 구동 검사 필요. 공식 Localization만 수정했으며 DLL은 수정하지 않음.

### lewdMemeBrood — 공식 Localization 번역 (2026-11-21)

- **한국어 CSV 커밋**: `31bb64610daf1d979734259f88ecaeff4ee27070` (이후 인명 용어 통일 커밋 `b31e6b1dcc6b4b79e73a0f7aa644d2926ea880b0`)
- **저장 경로**: `lewdMemeBrood v0331/Localization/ko.csv`
- **원본 파일**: `TentacleMod/lewdMemeBrood v0331/Localization/zh.csv` (Git blob SHA `e06627af1d7d6ecd3ae0682efc40f9dad33aee26`), 영문 `en.csv` SHA `c6ef1a95666cb9b512de7fa9521bacfd10037888`
- **모드 버전**: manifest `0.3.31`
- **범위**: 중국어 원본 CSV의 실제 키 **566개 전부 번역**. 아이템·둥지·알·부화·유전자/적응·야간 보고·연출·이북문(Li Beiwen) 스토리·설정·디버그·촉수 말풍선 포함.
- **검사**: 중문·영문 키 수 각각 566, 한국어 566; 누락 0, 추가 0, 중복 0, `{0}` 등 서식 인자 불일치 0, 이스케이프 줄바꿈 `\\n` 수량 불일치 0, 빈 번역 0. 결과 CSV를 GitHub에서 다시 읽어 확인함.
- **검수/재개 자료**: `번역대조표/Brood_001-150.tsv` (144키), `Brood_146-306.tsv` (161키), `Brood_307-423.tsv` (117키), `Brood_424-567.tsv` (144키). **계 566키**. 각 파일 키는 원본과 1:1 대응하며, 다음 채팅에서 번역 내용과 문맥을 재검수할 수 있음.
- **용어**: `李北文`은 한국어 파일과 대조표 모두 **리베이원**으로 통일. `触巢`는 '촉수 둥지', `孵卵巢`는 '부화 둥지', `苗床`은 우선 '숙주'로 번역. 다른 촉수 확장과 맞춰 최종 용어 통일 검수가 필요함.
- **추가 작업**: `Content/interactionLines.json`에서 대사 135개 중 125개 번역, 10개 보류. 아래 별도 기록을 참조. 게임 로딩 방식은 여전히 확인 필요.
- **주의**: CSV의 완전한 키 대조를 통과했지만 실제 게임이 `ko.csv`를 불러오는지, 화면 UI 길이/대사 자연스러움은 **미검증**. DLL은 수정하지 않았음.

### lewdMemeBrood — 상호작용 대사 JSON

- **커밋**: `a37914e12b6b5820210463d3c5047014b52dda0b`
- **번역본**: `lewdMemeBrood v0331/Content/interactionLines.json`
- **원본**: `TentacleMod/lewdMemeBrood v0331/Content/interactionLines.json` (Git blob SHA `7855f69a67291a9b8324cfe7acce56ee3582b5d3`)
- **대사 검수 현황**: 전체 135개 중 125개 한국어로 번역. 10개는 연령이 불명확한 캐릭터 체형 분류에 연결된 대사라 **원문 유지/보류**. 이들을 완료로 집계하면 안 됨. 별도 설명·예시 메타데이터는 게임 표시 문구가 아닌 원문 그대로 유지.
- **대조표**: `번역대조표/Brood_interactions_000-074.tsv` (65개), `Brood_interactions_075-134.tsv` (60개). 대조표 첫 번째 열은 원본 `lines` 배열 인덱스(0부터 시작).
- **기술적 무결성 확인**: JSON 로드 가능, `lines` 135개 유지, 모든 `kind` 및 `tags` 원문과 동일, `{name}`·`{tentacle}` 등의 치환 변수 불일치 0, 번역 대상에 미번역 한자 0.
- **실게임 검증 미실시**. 실제 화면에서 문장 길이, 조건별 무작위 호출, JSON 우선 적용 여부 확인 필요.
- **다음 과제**: 보류 10개는 작업 정책과 안전 기준 확인 전에는 수정하지 않음. 이미 번역한 125개도 2차 어체 검수 대상.

## 2026-11-21 Probably Adult Goods — Localization 번역 추가

- **원본 폴더**: `TentacleMod/可能是成人用品 v1.1.3/` (원본 **미수정**).
- **원본 중국어 CSV**: `Localization/zh.csv`, SHA `8f2b0a437a49da5a603c96c5e904c96c33331c97` (192키). 영문 `en.csv`, SHA `dabc580903f01d8f46bcdd848b5173d0f47e45ef` (192키).
- **버전 주의**: 원본 폴더명은 `v1.1.3`이나 `manifest.xml`의 `ModVersion`은 `1.03`. 동일 여부·모더 표기 기준은 추가 확인 필요.
- **한국어 CSV**: `TentacleMod번역/可能是成人用品 v1.1.3/Localization/ko.csv`. 작성 커밋 `e6a8e819d45f8c4fb896484108dbd65b3854ff16`. CSV 구조 192행·각 2열·키/순서 동일·빈 값 0·변수 누락 없음.
- **실제 번역 범위**: 한국어 189개 키. 세 키(`dildo_red_flavor_alt`, `dildo_pink_slim_flavor_alt`, `anal_beads_tail_cat_flavor_alt`)는 연령 관련 표현이 걸려 원문 유지. **완역 아님**.
- **작업 기록**: `번역대조표/AdultGoods_001-096.tsv` (94개), `AdultGoods_097-192.tsv` (95개). 대조표는 총 189개.
- **검증 범위**: GitHub에서 결과 CSV 재조회, 원본의 192키 동일성, 필드 수/CSV 형식 검사. 원문 수치 단위 `毫米`는 한국어에서 `mm`로 표기한 부분이 있음. **게임 적용 미확인**, 실제 텍스트 표시/UI 잘림·용어·문맥 2차 검수 필요.
- **주의**: `cfg_specialty_desc_tip`에 따르면 모드의 '개선된 설명' 기능은 신규 생성 아이템에만 적용됨. 기존 아이템이 바뀌지 않을 수 있다.

### lewd_meme_config 설정 화면 — 차기 작업

- `TentacleMod/lewd_meme_config0.7.2-pre/Source/Config/PanelKeys.cs`에서 `Section` 표시 섹션 13개, `Label` 182개, `Tooltip` 고유 99개 확인.
- `FamilyPanelKeys.cs`에서 `Label` 93개, `Tooltip` 93개 확인. 소스가 있어 단순 DLL 블라인드 치환보다 UI 문자열 용도 확인이 쉬움.
- **아직 컴파일·번역 DLL 생성 없음.** 원본 DLL을 임의 수정하지 말고, UI 표기만 매핑해 안전성 검사 후 수정·컴파일 경로를 결정할 것.

## 기술 조사 (확인된 사실)

- `MidnightPawn.dll`: CLR `#US` 문자열 힙에 중국어 문자열 764개; 원본 blob SHA `c27be30741564a51f57495a06681974bcc9421ca`.
- `ShadowTrade.dll`: 중국어 문자열 969개; SHA `c400f9dc58750d6b8494725e91e159626611c41c`.
- `ItemPortraitHD.dll`: 중국어 문자열 228개; SHA `57ecff396d96c3f959b08ff775c1e373d3796159`.
- 실제 사용 문자열은 CLR `ldstr` 명령이 `#US`에 있는 문자열을 참조하므로, 단순 바이트 치환이 아니라 메타데이터 및 IL 참조 처리가 필요하다.
- 원본 DLL의 GitHub 다운로드와 CLR 사용자 문자열 추출, 본체 DLL 일부의 시험 패치까지 수행했다. **게임 내 로딩·플레이 테스트는 미실시.**

## 2026-11-21 신규 모드 4개 확인 요청 (미완료)

- 사용자 비교 요청: **3842259965 vs 3843125702**, **3842247788 vs 3843142171**. 각 쌍은 중복 가능성이 있으므로 실제 `manifest.xml`, DLL/콘텐츠 파일, 버전·의존성·파일 해시를 비교한 후 하나만 번역 대상으로 선택해야 한다. **현재 네 ID의 대응 관계와 중복 여부는 확인되지 않았다.**
- GitHub `main`의 최근 업로드 커밋 `d0ef108c66a44e7ffc6fca5dcd8743deffe7e6b4` (`Add new TentacleMod source mods`)은 부모 커밋 `b3745d302fe7ce51775c10b5d18d04b512a05feb`과 **트리 SHA가 동일**(`e5002a9a7843439a14e77bd5675912fe47ef0fb5`)하다. **변경 파일 0개인 빈 커밋**이며 새 모드 추가가 이루어졌다고 볼 수 없다.
- 현재 GitHub `TentacleMod` 폴더에는 네 숫자 ID를 파일/폴더명으로 사용하는 자료가 없다. 기존에 존재하던 설명형 폴더를 이 ID들에 임의로 대응시키지 말 것.
- 외부 Steam Workshop 네 개 직접 링크는 웹 열람에 실패했으며 검색으로도 각 번호와 내용을 확증하지 못했다. 실질적인 버전/기능 차이 판단을 위해 **해당 네 개 원본 모드 파일을 실제로 GitHub에 업로드**하거나 읽을 수 있는 출처가 필요하다.
- **원본 모드 삭제·통합·중복 선택은 아직 실행하지 않음.** 원본 `TentacleMod`의 기존 파일은 보존.
- 작업 재개 시: GitHub `main` 최신 트리의 신규 파일 존재 확인 → 네 원본의 `manifest.xml` ID·Version·dependencies 비교 → DLL/JSON/CSV 구성과 파일 SHA 비교 → 어느 버전을 남길지 확정 → 번역 폴더에만 선택된 모드의 번역본 생성.

## 다음 작업 절차

1. 위 JSON 대조표는 1차 선별 완료(64개 반영, 14개 보류). 추가 번역은 화면 표시 문구를 우선하여 문맥 검수를 계속할 것.
2. 대상 DLL을 원본에서 읽어 별도 한국어 수정 DLL로 생성. 원본 파일에는 쓰기 금지.
3. 수정한 DLL의 PE/CLR 메타데이터, 모든 새 문자열 참조, 원본과 번역본 차이를 기계적으로 검증.
4. 성공한 DLL만 `TentacleMod번역/<원본 하위 폴더>/<DLL 파일명>`에 업로드. 각 DLL별 변경 건수와 검사 결과를 이 문서에 반영.
5. `lewdMemeBrood` 공식 Localization 566키, 상호작용 JSON 125/135개, Probably Adult Goods 189/192개 번역 완료. 다음은 `lewd_meme_config` UI 라벨/툴팁 또는 타 DLL을 진행하고, Brothel/Brood/Adult Goods 실제 게임 적용을 확인.
6. 게임 실행 확인 전에는 **임시 테스트본**으로 표시하고, 오류·미번역 범위를 반드시 기록.
7. 새 채팅에서는 **이 파일 → 원본 SHA/해당 모드 자료 → 대조표 → 시험 파일** 순서로 확인하고 재작업 범위를 결정. CSV 및 DLL 변경 후 즉시 이 파일 갱신.

## 주의할 부분

- 모드 내부 중국어 체형·속성·분류명은 문자열 자체가 분기 조건 또는 저장값일 수 있다. 표시되는 이름처럼 보여도 실행 흐름을 확인하지 않고 무작정 수정하지 않는다.
- UI에 보이는 중국어가 반드시 `#US`에서만 오는 것은 아니다. 외부 JSON/CSV와 런타임 조합 문자열도 별도로 확인해야 한다.
- 추가 작업 시 각 단계의 실제 반영 여부를 이 문서에 갱신한다. 이력이 없는 작업은 완료로 간주하지 않는다.
