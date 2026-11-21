# 촉수 모드 한국어 번역 — 작업 기록

> **업데이트: 2026-11-21 (KST)** · 이 파일은 다음 채팅에서 이어서 작업할 때 가장 먼저 읽을 인수인계 기록이다.
>
> **원본은 절대 수정하지 않는다.** 원본: `probably-stolen/local-mods/TentacleMod/`; 번역본: `probably-stolen/local-mods/TentacleMod번역/`.

## 작업 목적과 원칙

- 게임 Probably Stolen의 로컬 촉수 모드와 관련 확장 모드들을 한국어로 번역한다.
- **사용자가 지정한 최우선 작업 기준은 DLL 단위 번역이다.** 모드별 원본 DLL을 분석하고 번역 결과를 한국어 수정 DLL로 `TentacleMod번역/`에 별도 저장한다. CSV·JSON·C# 소스는 원문 대조·보조 자료 또는 DLL 제작 중간 산출물로 활용할 수 있지만, **CSV 단독 번역 완료를 DLL 번역 완료로 계산하지 않는다.** 기존 CSV 번역 자산은 삭제하지 않는다.
- DLL 내에서 식별자·세이브 키·정규식·파일 경로·호환성 상수·타 모드 API 문자열은 번역 대상에서 제외한다.
- 대사/설명은 원문 1:1 문맥 검수. 번역이 기능을 바꾸거나 저장 데이터를 깨뜨리면 안 된다.
- 미검증 DLL은 완료로 표시하지 않는다. PE/CLR 구조 검사와 **실제 게임 실행 검증**을 구별한다.
- **이 파일을 작업 단위마다 업데이트**하여 대상, 처리 건수, 남은 범위, 검증 상태, Git 커밋/파일 경로를 남긴다.

## 작업 기준 정정 — 2026-11-21 (KST)

- **원래 사용자 지시:** 2026-11-21 11:31경 대부분 로컬 키를 지원하지 않으므로 DLL 단위로 번역하라고 요청. 11:35경 가능한 만큼 DLL을 번역하여 `TentacleMod번역`에 옮길 것을 재확인.
- **오류 발생:** 11:45경 어시스턴트가 CSV가 있는 일부 모드는 CSV 우선이 안전하다는 자체 판단으로 기준을 전환하고 이를 원칙으로 기록. **사용자가 DLL 기준을 철회한 사실은 없다.**
- **정정:** (1) 실제 수정된 DLL에 반영된 문자열, (2) DLL 미적용 소스·CSV 초안, (3) 게임 실행 검증 여부를 구분해 보고한다. CSV 키 완성도는 보조 지표이고 DLL 진척도와 별개다.
- **현재 DLL 상태:** `MidnightPawn.dll` 64개(IL 74곳), `ShadowTrade.dll` 59개(IL 65곳, 기존 번역문 1곳 추가 용어 정정), `ItemPortraitHD.dll` 82개(IL 105곳)의 시험 DLL 3개만 존재. 실제 게임 실행 미검증. `lewd_meme_config`는 소스만 번역했고 한국어 DLL 미생성. Brothel/Brood/Adult Goods/Sarrona CSV도 DLL 반영 완료로 집계하지 않는다.
- **향후 순서:** 원본 DLL 표시 문자열 선별 및 1:1 대조 → 식별자·세이브 문자열 보호 → 한국어 DLL 작성/빌드 → 구조 검사 → 실제 게임 검증(가능할 때). 빌드나 검증이 불가능하면 장애 사유를 기록한다.

## 대상 및 확보 현황

| 원본 폴더 | 원본 또는 자료 확인 | 작업 상태 |
| --- | --- | --- |
| `lewd_meme2.1.0/` | `MidnightPawn.dll`, `manifest.xml` (mod v2.1.0), `Data/物品定义.csv` | **2차 한국어 DLL 시험본**: 누적 120개 문자열, 132곳 IL 참조 수정; 게임 실행 테스트 미실시 |
| `LewdMemeShadowTrade1.6.4/` | `ShadowTrade.dll`, 문서/데이터 | **2차 한국어 DLL 시험본**: 누적 114개 문자열, 127곳 IL 참조 수정; 게임 실행 테스트 미실시 |
| `LewdMemeBrothel-0.10.4-candidate/` | DLL 및 `Localization/en.csv`, `zh.csv` | **한국어 CSV 178개 키 전수 번역 완료(문자열 구조 검사 통과)**, 실제 게임 적용 미검증 |
| `lewdMemeBrood v0331/` | DLL, `Localization/`, `Content/`, manifest v0.3.31 | **Localization 566키 전수 번역**, 상호작용 JSON 135개 중 **125개 번역, 10개 보류** |
| `LewdMemeLegacyContent-1.0.6-48sounds/` | DLL, `Content/content.json` | 번역 적용 방식 검토 필요 |
| `ItemPortraitHD_v1.3.0/` | DLL, C# Source/, 설정 설명 | **3차 한국어 DLL 시험본**: 누적 193개 문자열, 223곳 IL 참조 수정; 게임 실행 테스트 미실시 |
| `RedLightDistrict_v1.1.1/` | 대용량 DLL, Git LFS 포인터 | 실제 LFS 바이너리를 확보한 뒤 분석 필요 |
| `可能是成人用品 v1.1.3/` | 공식 Localization `zh.csv`/`en.csv`, manifest 내부 버전 `1.03` | **CSV 192키 중 190키 한국어 번역, 2키 원문 보존(미번역)**, 구조 검사 통과. 런타임 미검증 |
| `lewd_meme_config0.7.2-pre/` | DLL + C# Source/ | **설정 라벨 275곳·툴팁 275곳·섹션 13종, 추가 UI·로그 문구 330곳, 본체 설명서 20개 비공백 줄 한국어 소스 번역**; DLL 미컴파일·게임 실행 미검증 |
| `[SRW-STM]SarronaTheDragon/` | 공식 Localization `zh.csv`/`en.csv`, Books JSON 4종, manifest v0.9.3 | **ko.csv 553키 전수 대응**(551개 문구 번역, 원본 공백 2키 보존); 키·변수·책 254곳 참조 검사 통과, 게임 테스트 미실시 |
| `[SRW-STM]MentorToSarrona/` | manifest v1.2.0, `MentorToSarrona.dll` 9,216바이트(소스/Localization 없음) | **manifest 설명 한국어화만 완료**; DLL 내부 대사 번역 미완료 |
| `3843110449/` (GeorgioToElueym) | manifest v1.1.0, `GeorgioToElueym.dll` 11,776바이트 + `ToElueym.dll` 4,608바이트(소스/Localization 없음) | **manifest 이름·설명 한국어화만 완료**; DLL 내부 문구 번역 미완료 |

| 기타 이미지 교체 패키지 | manifest, 이미지 등 | 문구 여부 확인 후 필요한 경우만 번역 |

## 실제 작성 파일 (모두 `TentacleMod번역/` 기준)

| 모드 | 산출물 | 번역 실적 | 상태 |
| --- | --- | --- | --- |
| 촉수 본체 | `lewd_meme2.1.0/MidnightPawn.dll` | 문자열 120개/IL 참조 132곳 | 시험 DLL, 게임 테스트 미실시 |
| 암거래 확장 | `LewdMemeShadowTrade1.6.4/ShadowTrade.dll` | 문자열 114개/IL 참조 127곳 | 시험 DLL, 게임 테스트 미실시 |
| 고해상도 초상화 | `ItemPortraitHD_v1.3.0/ItemPortraitHD.dll` | 문자열 193개/IL 참조 223곳 | 시험 DLL, 게임 테스트 미실시 |
| 매음굴 확장 | `LewdMemeBrothel-0.10.4-candidate/Localization/ko.csv` | 원문 178키 모두 번역 | CSV 구조 검사 통과, 실제 적용 미검증 |
| 촉수 번식 확장 | `lewdMemeBrood v0331/Localization/ko.csv` | 원문 566키 모두 번역 | CSV 구조 검사 통과, 실제 적용 미검증 |
| 촉수 번식 상호작용 | `lewdMemeBrood v0331/Content/interactionLines.json` | 135줄 중 125개 대사 번역, 10개 보류 | JSON 구조·치환 변수 검사 통과, 실제 적용 미검증 |
| 세로나의 용 모드 | `[SRW-STM]SarronaTheDragon/Localization/ko.csv` | 553키(551개 한국어 문구 + 원본 빈 값 2개) | 원본 키·치환 변수·Books loc 참조 검사 통과, 실제 적용 미검증 |

- DLL 문자열 번역 합계 **427개**, 변경된 IL 문자열 참조 **482곳**. 공식 CSV 번역 **1,295개 비어 있지 않은 문구**(Brothel 178 + Brood 566 + Sarrona 551; Sarrona 원본 빈 값 2키 별도), 외부 JSON 번역 **125개 대사**는 별도 합산.
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
- **실제 번역 범위 (최신, 정정)**: CSV 총 192키 중 **190키 한국어**, **2키 미번역(중국어 원문 유지)**. `dildo_red_flavor_alt`와 `dildo_pink_slim_flavor_alt`의 이전 임의 개작을 철회했고, `anal_beads_tail_cat_flavor_alt`는 `小女仆`의 '작은 메이드'라는 뜻을 임의의 성인 설정 없이 원문에 맞춰 번역했다. 원본 의미를 바꿔 번역 완료로 계산하지 않음.
- **작업 기록**: `번역대조표/AdultGoods_001-096.tsv` (94개), `AdultGoods_097-192.tsv` (96개). 대조표는 번역된 **총 190개**만 기재. 나머지 2키는 미번역으로 명시.
- **검증 범위**: GitHub에서 결과 CSV 재조회, 원본의 192키 동일성, 필드 수/CSV 형식 검사. 원문 수치 단위 `毫米`는 한국어에서 `mm`로 표기한 부분이 있음. **게임 적용 미확인**, 실제 텍스트 표시/UI 잘림·용어·문맥 2차 검수 필요.
- **주의**: `cfg_specialty_desc_tip`에 따르면 모드의 '개선된 설명' 기능은 신규 생성 아이템에만 적용됨. 기존 아이템이 바뀌지 않을 수 있다.

### lewd_meme_config 설정 화면 — C# 소스 한국어 라벨 작업

- **모드 원본**: `TentacleMod/lewd_meme_config0.7.2-pre/` (manifest `0.7.2-pre`). DLL 원본과 Source 폴더는 **변경하지 않음**.
- **한국어 소스 작업본 1**: `lewd_meme_config0.7.2-pre/Source/Config/PanelKeys.cs`. GitHub 커밋 `7813cb1664c771616b7c60f9eb7205def80881b8`. UI `Label` 문자열 182개와 `Section` 표시문구 13종(코드 내 지정 182곳)을 한국어로 교체함. 원본 파일 blob SHA `9000c74d2935e59fc74f9cde0da76a573f75318b`.
- **한국어 소스 작업본 2**: `lewd_meme_config0.7.2-pre/Source/Config/FamilyPanelKeys.cs`. 커밋 `bff90b5061e056f06d2da3c6c4848629689e4dc0`. UI `Label` 문자열 93개를 한국어로 교체함. 원본 SHA `e68a68b8f97106acb27f5737224134af7388710d`.
- **번역 대조표**: `번역대조표/LewdMemeConfig_PanelKeys_labels.tsv` (182개 라벨 + 13개 섹션), `번역대조표/LewdMemeConfig_FamilyPanelKeys_labels.tsv` (93개 라벨).
- **검증**: 두 원본 소스와 한국어 작업본을 재조회 후 `Label`/`Section` 문자열 부분만 제거해 비교한 결과 나머지 소스 텍스트는 완전히 동일. 라벨 누락 0, 새 라벨 내 한자 잔존 0, C# 소스의 나머지 코드·영문 식별자·툴팁은 변경하지 않음.
- **툴팁 진행 상태 갱신**: 아래 'C# 설정 툴팁 전수 번역' 기록에서 182+93=275곳 번역·대조표 완료 확인. 게임 DLL은 미반영.
- **중요: 현재 파일은 소스 작업본이며 배포용 DLL이 아님.** 프로젝트의 `LewdMemeConfig.csproj`는 로컬 게임의 `Probably Stolen_Data/Managed`에 있는 Unity·게임 DLL 참조에 의존함. 이 참조가 확보되지 않아 빌드/실게임 동작은 미검증이며, 기존 `LewdMemeConfig.dll`에는 한국어가 적용되지 않음.


### lewd_meme_config — C# 설정 툴팁 전수 번역 (2026-11-21)

- **기본 설정 소스** `lewd_meme_config0.7.2-pre/Source/Config/PanelKeys.cs`의 `Tooltip` 필드 **182곳, 서로 다른 원문 99개를 모두 한국어로 반영**. 초기 반영 커밋 `b628f51ba9fa83cfd08270a65768a212d82508f5`, 용어 정리 커밋 `7ff5584916c39e9a7168db90eac36c79b33e8e27`. 대조표 `번역대조표/LewdMemeConfig_PanelKeys_tooltips.tsv` (99개 중문/한국어 1:1 + 중복 출현 횟수). 기본 설정의 반복 툴팁 `关闭=该值用原版默认` 83곳 포함.
- **연동 설정 소스** `lewd_meme_config0.7.2-pre/Source/Config/FamilyPanelKeys.cs`의 `Tooltip` 필드 **93곳, 고유 93개를 모두 한국어로 반영**. 커밋 `19136429d0eb5211eb5f36680a1552b76947f8b0`. 대조표 `번역대조표/LewdMemeConfig_FamilyPanelKeys_tooltips.tsv` (93개).
- **이번 작업 규모**: Tooltip **275곳**(고유 원문 **192개**) 전수 반영. 앞서 작업한 Label 275곳, Section 13종은 그대로 유지. 동일 문구가 반복되는 경우에도 각 출현 위치에서 전부 한국어로 변경.
- **기술 검증**: 원본 파일과 한국어 수정 소스 모두 재조회. `Label`, `Section`, `Tooltip` 오른쪽의 문자열 리터럴만 추려낸 뒤 비교하면 **나머지 코드 내용이 문자 단위로 동일**. 정규식상 두 파일의 `Tooltip` 출현 수 각각 182/93 유지. 단위·버전·숫자를 대조했고 일부의 숫자 개수 차이는 한자 수량 표현(三/五/每) → 한글 숫자(3/5/1) 때문에 발생한 것으로, 코드 값 변경은 없음.
- **중국어 식별자 예외**: CSV에 그대로 입력해야 할 기본 세력/체형/가슴 크기 값(`上层区`, `萝莉`, `贫乳` 등)이 들어 있는 일부 안내문은 해당 입력값을 원문 형태로 유지. 번역 누락이 아니라 기능상 문자 일치가 필요해 보존한 입력 사례.
- **상태·한계**: 여전히 `LewdMemeConfig.dll`은 미수정. 소스 작업본 2개만 만들었으며 관련 `GameManaged` 참조 DLL 확보와 컴파일/게임 테스트는 진행하지 못했음. 따라서 **인게임 번역 적용 완료가 아님**. 나머지 소스의 다른 UI 문자열/설정 등록용 동적 표시문구도 추가 점검 필요.
- **번역 원칙**: 설명의 수식·수치·기본값·기능 비활성 조건·경로·설정 키·각 모드명은 의미를 유지하며 임의 순화·개작하지 않음.

## 2026-11-21 — [SRW-STM]SarronaTheDragon 공식 Localization 전수 번역

- 원본 폴더: `TentacleMod/[SRW-STM]SarronaTheDragon/` (원본 미수정). manifest ID `sarrona_the_dragon`, 버전 `0.9.3`. 공식 Localization 기반 작업.
- 원본: `Localization/zh.csv` blob `36de22b9e3222ff70a6081076814dbe5e4bb359d`, `en.csv` blob `d15405d489bd3cb391a428f6a6a81321e00a6c43`. 모두 553개 키.
- 한국어 번역본: `TentacleMod번역/[SRW-STM]SarronaTheDragon/Localization/ko.csv`, 커밋 `b12b0e2c914143a21c19544e3a44321137a58a35`. **원본 553키 모두 대응**, 551개 문구 한국어, 원본부터 값이 비어 있는 `milk_can_empty_flavor`, `milk_dryer_flavor` 두 키는 공백 유지.
- 1:1 대조표: `번역대조표/Sarrona_001-140.tsv` (140키), `Sarrona_141-280.tsv` (140키), `Sarrona_281-420.tsv` (140키), `Sarrona_421-553.tsv` (133키). 총 553행, 키 중복 없음. **각 대조표 3열 `key / zh / ko`**로 작성했으며 원본 중국어·한국어 결과를 실제 파일에서 다시 읽어 553개 항목 각각 일치함을 검증(대조 누락 0, 원본 불일치 0).
- 번역 범위: 세로나 상인 대사, 용유/유제품, 양조와 용연향, 용 비늘과 용골 장비, 프린팅, 쥐/물고기 양식, 서적 매뉴얼, 구호물자와 정전 보관 이벤트. 아이템 키와 수식·치환 인자(`{0}`, `{0:0.#}`, `{0:0.##}` 등)는 유지.
- GitHub 결과물 재조회 검사: 중·영·한 CSV 모두 553키 동일 순서; 키 누락·중복·추가 0, 플레이스홀더 불일치 0, 원본 공백 보존, 한자 잔존 0.
- 책 JSON 4개에서 참조하는 `loc`, `nameLoc`, `descriptionLoc` 254곳 전부 번역 CSV에 대응함. `Books/*.json` 자체는 변경하지 않았음. 책 파일의 영어 `title` 메타데이터는 실행 중 용도 미확정이므로 변경하지 않음.
- **런타임 적용 미검증**: 실제 게임에서 ko.csv 자동 로드 여부와 UI 페이지 출력/줄바꿈, 용어·자연스러움 2차 검수 필요. 숫자 표기는 중국어 숫자를 한글 숫자나 아라비아 숫자로 변환한 부분이 있어 일부 문자열은 숫자 정규식만으로 단순 비교 불가.
- 다음 대상: 세로나의 실제 게임 적용과 서적 페이지 표시 검사, 2차 1:1 문맥 검수. `lewd_meme_config`는 소스 번역 완료지만 아직 DLL 미컴파일.

## 2026-11-21 — Sarrona 매뉴얼 제목 및 신형 모드 manifest 표시 문구 번역

- **범위**: 원본 파일은 `TentacleMod/`에 그대로 두고 모든 변경 파일을 `TentacleMod번역/`에 별도 작성. 이번 작업은 정적 UI/메타데이터 번역이며 신규 DLL 수정 없음.
- **SarronaTheDragon v0.9.3 — Books 4개**:
  - `Books/sarrona_manual_dairy.json`: `title` 'Dragon Milk and Saliva' → '용유와 용의 침', 커밋 `ece2fd53cad0a5b9d0657b2330fe1ec9022d6888`.
  - `Books/sarrona_manual_forge.json`: `title` 'Furnace and Filter Guide' → '용광로와 용담 여과기', 커밋 `ee44dd819da8c90e4a358aade915c2020102610c`.
  - `Books/sarrona_manual_husbandry.json`: `title` 'Raising Fish and Rats on Dragon Milk' → '용유로 물고기와 쥐 기르기', 커밋 `ec5fb2bc894f3d89af3a147e851ebd6717da41ab`.
  - `Books/sarrona_manual_print.json`: `title` 'Printing with Dragon Bone' → '용골 프린팅', 커밋 `0d5c10666adcfb803da3700e16398dd1aa5873b1`.
  - **검증**: JSON 4개 모두 다시 열어 파싱했고, `title`을 원래 값으로 치환해 비교하면 JSON 객체의 모든 다른 필드 및 배열 구조가 **원본과 동일**. `loc` 키, 페이지 좌표, 그림, 버튼, 제작법은 모두 보존. 이전에 완성한 `Localization/ko.csv`는 변경하지 않음. `title`의 실제 화면 사용처는 미확인.
- **[SRW-STM]MentorToSarrona v1.2.0 — manifest 설명**: `[SRW-STM]MentorToSarrona/manifest.xml`, 커밋 `6ba8f0c01c0a1eb69efda9392db025bb361766c8`. 세로나 멘토 교체·그림/호칭/노트·SarronaTheDragon 0.9.1 연동 내용을 원문대로 한국어화. **원본 모드 Name/ID/어셈블리 변경 없음**.
- **3843110449 / GeorgioToElueym v1.1.0 — manifest 이름·설명**: `3843110449/manifest.xml`, 커밋 `eef96a0c97dc2ba2be5423c004e323866f955e6f`. 조르지오→Elueym, 잭슨 박사→군벌, 초상화 교체/호칭/충돌 안내를 한국어화. **ID, 의존성, DLL 선언, 버전은 원문 그대로**. 게임 내 명칭과 대사는 DLL에서 바꾸는 것이므로 아직 한국어화되지 않음.
- **[SRW-STM]SarronaTheDragon v0.9.3 — manifest 설명**: `[SRW-STM]SarronaTheDragon/manifest.xml`, 커밋 `7b7acf23e7de0f4048aac672989e3be1414abf56`. 대응 게임 버전(100-REV13, REV3 미지원), 세로나 거래·용유·제작 용도를 한국어화. **ModVersion, GameVersions, ID, DLL 경로 보존**.
- **기계 검사**: 위 3개 XML은 `Name` 및 `Description` 요소 외의 바이트 내용이 원본과 동일. ModVersion·ID·GameVersions·Assemblies·Prerequisites 값이 원본과 동일한 것을 확인함. Books 4개도 `title` 외 변경 없음.
- **바이너리 한계**: 현재 GitHub 연결의 파일/Blob 읽기 기능은 비-UTF-8 바이너리 DLL에서 디코딩 오류가 발생하여 바이트를 확보할 수 없었음. `MentorToSarrona.dll`, `GeorgioToElueym.dll`, `ToElueym.dll`은 **읽기/패치/컴파일/실행 검증 불가**. 안전하거나 정상 동작한다고 판정하지 않았음. 정적 manifest 번역이 인게임 DLL 문자열을 번역해 주는 것은 아님.
- **다음 작업**: DLL 바이너리를 안전하게 가져올 수 있게 되면 식별자/내부 비교 문자열을 보호하며 UI 표시 문자열만 수정, 테스트 DLL을 별도로 만들어 로딩 검사. 별도로 Sarrona의 실제 게임 적용·책 제목 표시 위치 검수 필요.

## 2026-11-21 — lewd_meme_config 남은 UI·로그와 본체 설명서 번역

- **범위**: 원본 `TentacleMod/lewd_meme_config0.7.2-pre/Source/`를 다시 조회한 뒤 한국어 `TentacleMod번역/lewd_meme_config0.7.2-pre/Source/`에 원본과 같은 상대 경로로 **별도 작업본** 생성. 원본 DLL 및 C# **변경 없음**.
- **디버그 패널** `Integration/DebugPanel.cs` — 중국어 문자열 **94곳(고유 85)** 중 **93곳(고유 84)** 한국어로 번역. `未装配` 문자열 1개는 `expected != "未装配"` 내부 조건에 사용하므로 유지. `번역대조표/LewdMemeConfig_DebugPanel_strings.tsv`에서 85개 항목의 번역/보존 사유·발생 횟수 기록. 소스 생성 커밋 `3161100e0b400cb37a317857a3634f59e6231042`; 한글 글꼴 표시를 위해 OS 폰트 우선순위에 `Malgun Gothic` 추가(커밋 `bc9a055df501a36063a4796d4b4de9ce9e4d065a`). GUI 호환성 및 실제 렌더링 미검증.
- **패널 연결 안내** `Integration/PanelConfig.cs` — 문구 **6곳**(커밋 `6efb3df2f2fa11f4bc8553a241ecd132d6ca185e`). `启用:` → `사용:` 등 기존 한국어 라벨과 일치시키고 설정 키 및 C# 제어문은 변경하지 않음.
- **숙주 태그 선택기** `Integration/BroodTagPicker.cs` — **12곳**(고유 11), 선택/오류 표시 한국어(커밋 `790556bad00e46d841a1dc91d9315240dd91e1f0`). `MAIDEN_FEMALE` 식별자 및 실제 단축키 처리 코드는 그대로 유지.
- **모드 초기화 로그** `ModEntry.cs` — **11곳**, API 양보, 연동 실패·대기 상태 안내 한국어(커밋 `7819f71ce65d0816259e02c20e53b199fb7e2545`).
- **패널 행 오류** `Integration/PanelRowPatch.cs` — **1곳**(커밋 `a4b1b58fcdab7c5b6b5448752026ce2a498ee696`).
- **연동 모드 버전 표기** `Config/FamilyMods.cs` — **7곳**(커밋 `386385e394d9006d462569eec2888f8c8fd8cf0c`).
- **설정 등록 오류 로그** `Config/ConfigRegistry.cs` — **7곳**(고유 5, 커밋 `08261dd1c4ae37548f032095210f3180b03da5ce`). 기본 세력·직업·체형 등 **입력값으로 사용되는 중국어 CSV 문자열은 절대로 변경하지 않음**. 코드/기본값 검사 통과.
- **버전 불일치 경고** `Logic/PanelMarks.cs` — **1곳**, `버전이 일치하지 않아 이 설정은 적용되지 않습니다`로 번역(커밋 `c368612d124100ba819f5b8cc1c1635ee54436f3`). `IsVersionMark`는 동일 상수로 비교하므로 코드 동작 보존. 해당 텍스트를 정확히 비교하는 `Tests/PanelMarksTests.cs`의 `VersionText` 기대값도 한국어로 함께 변경(커밋 `cdf3b7134dfbdf84bbdcc42e01ad40b90dc23e1e`). **단, 테스트 실제 실행은 못함**.
- **위 항목 총계**: 기존 패널 라벨/툴팁 외 추가로 **138곳의 UI·로그 문자열** 번역(디버그 93 + 패널 6 + 선택기 12 + 초기화 11 + 패널 행 1 + 연동 버전 7 + 등록 오류 7 + 버전 경고 1). 서로 다른 원문은 84+6+11+11+1+7+5+1=**126개**. 원본 UI 문자열만 바꾼 파일은 `DebugPanel` 글꼴 설정 1곳을 제외하고 문자열 리터럴 외 코드가 동일하며, 번역본 재조회로 확인.
- **촉수 본체 설명서** `Logic/ManualTextRenderer.cs` — 기존 `Template` 중국어 상수는 원본 기본값을 검증하는 테스트가 의존하므로 **그대로 보존**. 별도 `TemplateKo` 23개 문자열 조각(비공백 20줄)을 추가하고 `Patches/SpecialAndManualPatches.cs`의 `CreateNotePostfix`에서 `ManualTextRenderer.Template` 대신 **`TemplateKo`만 사용**하도록 변경. 한국어 템플릿은 원문 **`{cfg:...}` 22개 치환 자리의 종류·순서, `\\n` 및 `\\u3000` 이스케이프 구조를 보존**. 대조표 `번역대조표/LewdMemeConfig_ManualTextRenderer_template.tsv`. 커밋: 템플릿 `e464fb259efcae89d74c93f7c4476195225ed1cd`, 호출 위치 `737b1636de1444a34bc8ca278fec1971da2e5b83`. 수치 계산·렌더 함수·핵심 설정 ID는 변경하지 않음. 이 소스를 컴파일해 사용할 경우에만 한국어 설명서가 출력될 것으로 예상.
- **독립 검사**: 원본과 한국어판의 C# 문자열 리터럴 바깥 부분을 비교하여 `PanelConfig`, `BroodTagPicker`, `ModEntry`, `PanelRowPatch`, `FamilyMods`, `ConfigRegistry`, `PanelMarks` 모두 동일 확인. `DebugPanel`은 한글 폰트 후보만 추가한 차이, `ManualTextRenderer`는 한국어 상수 추가, `SpecialAndManualPatches`는 설명서 템플릿 참조 한 곳만 교체. 모든 번역 매핑을 별도 TSV로 기록.
- **중요한 미완료 사항**: `LewdMemeConfig.dll` **미컴파일 / 미수정**, 단독으로 실행할 수 있는 전체 한국어 빌드 미제작. 컴파일에 게임 Managed DLL 등 의존성이 필요. 실제 로더에서의 게임 실행·UI 글꼴·기능·모드 호환성은 검증하지 못했음. `SelfTestRunner.cs`의 진단 문구, `SpecialAndManualPatches.cs`의 나머지 중국어 문자열, 원본 테스트용 중국어 설명서와 내부 비교용 기본값은 아직 남아 있음. 이 작업을 **인게임 완료로 표시하면 안 됨**.
- **다음 순서**: SelfTestRunner 및 잔여 실제 표시 문구 선별 → 게임 빌드 의존성 확보 시 안전한 컴파일/테스트 DLL 생성 → 인게임 기능·한국어 글꼴 검증.

## 2026-11-21 — lewd_meme_config SelfTestRunner 및 SpecialAndManualPatches 소스 번역 추가

- **작업 배경**: 기존 `PanelKeys.cs`/`FamilyPanelKeys.cs`의 설정 라벨·툴팁 작업 이후 남아 있던 C# 진단 메시지 및 일부 화면 출력 문자열을 1:1로 확인. **원본** `TentacleMod/lewd_meme_config0.7.2-pre/Source/` **변경 없음**, 번역 파일과 대조표는 `TentacleMod번역/`에만 기록.
- **`Integration/SelfTestRunner.cs`**: 원본 중국어 리터럴 **132종(168곳)** 중 **130종(162곳)** 한국어 번역. `未装配`, `黑市` 2종(합계 6곳)은 패치 상태 및 세력 설정/조건 판정에 실제로 사용되는 문자열이므로 **원본대로 보존**. 한국어 안내 문구에 들어가는 `黑市`도 설정값과 일치하도록 원문 표기를 유지함. 기술 토큰 `GetBodyLevel`, `MAIDEN_FACTION`, `IPH`, `fatigue_enabled`, `PriceHint`, `Current` 등 유지. Git 커밋 `f55929a7db8c99a991fc8c8e496a5283290990a7`. 대조표 `번역대조표/LewdMemeConfig_SelfTestRunner_strings.tsv` (132종, 상태/원문/번역/출현 수), 대조표 최종 커밋 `a097e253ea79c8225de490db2bfd88d13e5d6fce`.
- **`Patches/SpecialAndManualPatches.cs`**: 원본 중국어 리터럴 **44종(45곳)** 중 일반 오류·설명서·저장 복원·사용자 지정 이름 목록·효과 로그 등 **29종(30곳)** 한국어 번역. **4종(4곳) 세력 분기 조건** `上层区`, `治安部`, `黑市`, `革命军`은 **원문 그대로 보존**. 나머지 **11종(11곳)의 성적 묘사·대사**는 임의 순화·개작하지 않고 **미번역**으로 유지하여 별도 대조표에 보류 표시. 커밋 `be8e79e4f8ac414ce32f8c51e7fb682b767683c7`. 대조표 `번역대조표/LewdMemeConfig_SpecialAndManualPatches_strings.tsv` (44종, 보존/보류 구분).
- **기존 번역 내용 보존**: `SpecialAndManualPatches.cs`의 설명서 출력은 `ManualTextRenderer.TemplateKo`를 그대로 호출하며 본체 한국어 매뉴얼 관련 이전 수정은 되돌리지 않았음.
- **검증**: GitHub 반영본 다시 조회. `SelfTestRunner.cs` 리터럴 개수 원본/번역본 **413/413**, `SpecialAndManualPatches.cs` **106/106**. 문자열 리터럴 이외 C# 코드 부분 **두 파일 모두 동일** (`SpecialAndManualPatches.cs`의 기존 의도된 `Template`→`TemplateKo` 호출 차이 제외). 로직·설정 키·분기 조건 수정 없음. 소스 코드상 구조 검증이지 **컴파일/런타임 검증은 아님**.
- **누적 이번 세션 추가**: 이전 번역한 기본 UI·로그 **138곳**에 이번 진단 **162곳** 및 후킹/설명서 로그 **30곳**을 추가하여 `lewd_meme_config` 소스에서 해당 분류의 번역 총 **330곳** (기존 설정 라벨 275곳, 툴팁 275곳과 별도). 원본 코드를 대상으로 한 자동 중문 잔존 검사는 운영상 식별자/테스트 기대값과 보류 대사를 구분해야 함.
- **미완료**: `LewdMemeConfig.dll` 컴파일 및 게임 적용은 미실시, `SpecialAndManualPatches.cs`의 11개 서술형 문구 보류, 기타 패치 소스에 남은 실제 표시 문구 재점검 필요. 전체 번역/실게임 적용 완료로 집계 금지.

## 기술 조사 (확인된 사실)

- `MidnightPawn.dll`: CLR `#US` 문자열 힙에 중국어 문자열 764개; 원본 blob SHA `c27be30741564a51f57495a06681974bcc9421ca`.
- `ShadowTrade.dll`: 중국어 문자열 969개; SHA `c400f9dc58750d6b8494725e91e159626611c41c`.
- `ItemPortraitHD.dll`: 중국어 문자열 228개; SHA `57ecff396d96c3f959b08ff775c1e373d3796159`.
- 실제 사용 문자열은 CLR `ldstr` 명령이 `#US`에 있는 문자열을 참조하므로, 단순 바이트 치환이 아니라 메타데이터 및 IL 참조 처리가 필요하다.
- 원본 DLL의 GitHub 다운로드와 CLR 사용자 문자열 추출, 본체 DLL 일부의 시험 패치까지 수행했다. **게임 내 로딩·플레이 테스트는 미실시.**

## 2026-11-21 12:58 KST — 신규 모드 실제 업로드 확인 및 중복·보안 비교

- **업로드 검증:** 커밋 `afbac7d0ed234c437c042afcab4510df1dc12f4c` (`Add new TentacleMod source mods`). 이전의 빈 커밋과 달리 **추가 파일 176, 삭제 0, 전부 `TentacleMod/` 경로 내**. 다음 신규 모드 7개 폴더: `3842247788`(4), `3842259965`(4), `3843110449`(6), `3843125702`(4), `3843142171`(3), `[SRW-STM]MentorToSarrona`(3), `[SRW-STM]SarronaTheDragon`(152).
- **이전 기록 정정:** `d0ef108`은 빈 커밋이었지만 이후 `afbac7d`에서 실제 추가에 성공했으므로 '네 개 원본 미확보' 상태는 **해결**됨.
- **중복 쌍 1 (성인용품):** `3842259965`의 manifest ID `probably_adult_goods`, 버전 `1.1.1`, 단일 `probably_adult_goods.dll` 8,166,912바이트 + README/preview. `3843125702`의 manifest ID는 **동일**, 버전 `1.2.1`, 본체 DLL은 **294,912바이트**로 내용이 전혀 같지 않음; `DsBhyuStart.dll` 6,656바이트 + `payload.txt` 470,360바이트를 추가로 포함. 후자 manifest는 `<Capability>native</Capability>` 및 중국어로 '기존 BridgeInit 페이로드를 조용히 실행'한다는 내용을 명시함. **단순 동일 모드 중복이 아님. `3843125702`는 보안상 격리/비활성 권고, 번역·실행 대상 제외.**
- **중복 쌍 2 (엘리너):** `3842247788`의 manifest ID `probably_eleanor_followup`, 버전 `1.0.0`; 본체 DLL 16,418,816바이트, README/preview 포함, `probably_adult_goods` 선행 요구. `3843142171`의 manifest ID는 **`probably_adult_goods`로 바뀌어 성인용품 모드 ID와 충돌 가능**하고, 버전 `1.1.3`, 본체 DLL 16,721,920바이트 + `payload.txt` 470,360바이트 포함. `<Capability>native</Capability>` 선언, 설명에 BridgeInit 페이로드 내장 표기. **동일 파일도 아니며 `3843142171`은 보안상 격리/비활성 권고, 번역·실행 대상 제외.**
- 두 위험 후보의 `payload.txt`는 **Git SHA 동일** `60ede3d0a1b4e09844fafc5e32df3e6533492b4f`. Base64 본문은 `TVqQ`로 시작하며 PE(MZ) 실행 파일 데이터로 보임. **악성코드 최종 판정은 아님. 바이너리 실행이나 동적 분석은 하지 않았으며, 단순 DLL이 존재한다는 이유만으로 안전하다는 뜻도 아님.**
- **번역 대상 선택:** 워크샵 번호 쌍만 놓고 보면 `3842259965`과 `3842247788`을 보수적으로 선택. 다만 이미 업로드된 로컬 원본 `可能是成人用品 v1.1.3` (manifest 내부 버전 `1.03`) 및 `落魄千金-埃莉诺 v1.0.3`와 기능/버전 차이가 있으니 실제 번역 기준은 별도 검토. 기존 번역 파일/원본은 임의 삭제하지 않음.
- 새로 업로드된 `[SRW-STM]SarronaTheDragon`에 `Localization/en.csv`, `zh.csv`, Books JSON 4개가 있으므로 향후 **Localization 번역 우선**.
- 다음 작업: 두 페이로드 포함 모드는 실행/설치/빌드하지 않기; 안전 후보 원본과 이전 로컬 파일의 버전·기능/키 비교 후 번역 진행. 사용자 환경에서 실제로 악성 행위가 수행됐는지는 검사하지 않았음.

## 2026-11-21 신규 모드 4개 확인 요청 (과거 기록 — 이후 실제 업로드 확인)

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
5. `lewdMemeBrood` Localization 566키, 상호작용 JSON 125/135개, Probably Adult Goods 190/192개 (2키 미번역), SarronaTheDragon 551문구/553키(원본 빈 값 2개) 번역 완료. `lewd_meme_config` 설정 라벨 275개·섹션 13종 소스 번역 완료; 툴팁 275곳 소스 번역도 완료. 추가 UI/로그 138곳과 본체 설명서 한국어 소스 작업본까지 완료. 다음은 SelfTestRunner 등의 남은 UI/로그 검수, 의존성 확보 후 컴파일 및 Brothel/Brood/Adult Goods 게임 적용 확인.
6. 게임 실행 확인 전에는 **임시 테스트본**으로 표시하고, 오류·미번역 범위를 반드시 기록.
7. 새 채팅에서는 **이 파일 → 원본 SHA/해당 모드 자료 → 대조표 → 시험 파일** 순서로 확인하고 재작업 범위를 결정. CSV 및 DLL 변경 후 즉시 이 파일 갱신.

## 주의할 부분

- 모드 내부 중국어 체형·속성·분류명은 문자열 자체가 분기 조건 또는 저장값일 수 있다. 표시되는 이름처럼 보여도 실행 흐름을 확인하지 않고 무작정 수정하지 않는다.
- UI에 보이는 중국어가 반드시 `#US`에서만 오는 것은 아니다. 외부 JSON/CSV와 런타임 조합 문자열도 별도로 확인해야 한다.
- 추가 작업 시 각 단계의 실제 반영 여부를 이 문서에 갱신한다. 이력이 없는 작업은 완료로 간주하지 않는다.

## 2026-11-21 — 용어 '유즙' → '모유' 일괄 정정 (번역 텍스트만)

- 사용자 요청에 따라 번역본의 한국어 표기 `유즙`을 `모유`로 변경했다. GitHub `TentacleMod번역/` 하위 UTF-8 텍스트 파일 **57개**를 대상으로 검사했다.
- **17개 텍스트 파일, 136곳 수정**: `lewdMemeBrood v0331/Localization/ko.csv` 8곳, `lewd_meme_config0.7.2-pre/Source/` C# 5개 파일 59곳, `번역대조표/`의 11개 파일 69곳. (파일별: Brood TSV 8, 설정 관련 TSV 59, ShadowTrade.json 2)
- 원본 `TentacleMod/` 폴더는 변경하지 않았다. CSV 키·C# 식별자·원본 대조표 중국어 열·JSON 구조 및 플레이스홀더는 변경 대상이 아니며, 텍스트에서 정확히 `유즙`에 해당하는 두 글자만 치환했다.
- **시험 DLL 추가 검사 및 수정**: 기존 한국어 시험 DLL 3개를 Base64로 읽어 UTF-16LE 문자열을 검사했다. `ShadowTrade.dll`의 `유즙량` 1곳을 `모유량`으로 **4바이트 동일 길이 치환**하고 Git blob `f2285db146ec282514eecc7568b477901d09396b`로 다시 업로드했다(커밋 `a0e6af99ede7e16bb83cf24d5f40fca3137fb6df`). 재조회 결과 이전 표기 0곳·새 표기 1곳, DLL 크기 327,168바이트 보존. `MidnightPawn.dll`·`ItemPortraitHD.dll`은 해당 UTF-16LE 표기 0곳. **텍스트 136곳 + 시험 DLL 1곳 = 총 137곳 정정.** DLL의 PE/CLR 재검사 및 실제 게임 실행 검증은 미완료.

## 2026-11-21 — 용어 통일 이후 계속된 번역 작업

- **LewdMemeLegacyContent v1.0.6 (48sounds)**: 원본 `TentacleMod/LewdMemeLegacyContent-1.0.6-48sounds/`의 중국어 설명서·설치 안내·manifest를 번역 폴더에 별도로 추가. 산출물 3개: `LewdMemeLegacyContent-1.0.6-48sounds/README.md`, `安装说明.txt`, `manifest.xml`. 설명서의 기능·버전·기본값·설치 경로 및 manifest의 `ID`, `Author`, `ModVersion`, `GameVersions`, `Prerequisites`, `Assemblies`, `Capabilities`는 변경하지 않았다. 각 생성 커밋은 `b7665af82ee0d48fdd4f3b312ad40820908e8ebc`, `807060a3499767ae88dfb82668771bebded74123`, `1d84c923a0e0f8f468aa554d84a3053489a20955`. **`Content/content.json`, DLL, 효과음은 이번 작업에서 번역·수정하지 않음**. 게임 적용 미검증.
- **lewd_meme_config v0.7.2-pre — 추가 진단/로그/표시 문자열**: 번역 폴더의 `Source/`에 **C# 10개 파일**을 새로 작성하고, 대응하는 1:1 대조표 TSV **10개**를 생성. 총 **67곳**(서로 다른 리터럴 64개): `Integration/HookRuntime.cs` 18, `IphCompat.cs` 4, `MainModResolver.cs` 10, `TranspileCarrier.cs` 8, `WriteThroughCarrier.cs` 4, `Patches/AlarmPatch.cs` 1, `LevelCapPatch.cs` 1, `GenderRatioPatch.cs` 5, `Logic/Router.cs` 6, `SelfTestPlan.cs` 10. 소스 문자열 외 C# 동작 코드·영문 ID는 변경하지 않음.
- **코드 검수 특기 사항**: `TranspileCarrier.cs`의 주석 `名:值,…`, `SelfTestPlan.cs`의 주석 `关` 2곳 등 실행과 관계없는 원문 설명은 유지. `Logic/Router.cs`의 패널용 안내 문구 변경에 맞춰 `Tests/RouterTests.cs`의 기존 기대값 **6곳**도 한국어로 수정(커밋 `0760f06a476cc9725888653e2b69c85dda615837`). 테스트는 실행하지 않음.
- **새 번역 소스 주요 커밋**: `HookRuntime` `f40b9e72339fe288c040e9a326ff121d31710a57`, `IphCompat` `e3acc16cdc35809c9fdc352be0a7d324cc9c7dbe`, `MainModResolver` `b84af5cec974acf811dc3b6a022f308686dfc7a0`, `TranspileCarrier` `a9651f3ea70b17ea8c8f64f54038c3806e8a7282`, `WriteThroughCarrier` `003072b2d2a1de76b80dc34fa44613a9747e76eb`, `AlarmPatch` `c1983e6e5245a88e59991b5b3b4975a7101ff620`, `LevelCapPatch` `5ddf7353a5d7d4885e432c0fbdda6e3f38fd3a79`, `GenderRatioPatch` `46d7b6ffcdbed5f84d096056f85f439c2d1f5f8c`, `Router` `f3ae644c40ecba3910b82d9cdca0098de36b7c16`, `SelfTestPlan` `f661d4dbc578aba9fe0c0d57225b3bf15aec2a37`.
- **남은 작업**: `LewdMemeLegacyContent`의 JSON 사용자 표시 항목은 필드별 기능·연령 맥락을 분리 검수한 뒤 별도 진행. `lewd_meme_config`의 다른 패치 파일에 남은 실제 로그/화면 문구 선별과 일대일 검수 계속. 기존 미완료 DLL의 빌드·실행 검증은 별개. 어느 파일도 실제 게임 적용 완료로 분류하지 않는다.

## 2026-11-21 — MidnightPawn.dll 활성 문자열 추가 번역

- DLL 시험본 내 실제 `ldstr`로 참조되는 중국어 사용자 표시/진단 문자열 **56종, 58곳**을 1:1로 번역하고 수정 DLL에 반영함. 현재 대상은 초기화·오류·상태·디버그·야간 보고 등의 문구이며, 내부 판정 키로 추정되는 짧은 원문은 보류.
- **변경 파일**: `lewd_meme2.1.0/MidnightPawn.dll`, `번역대조표/MidnightPawn_DLL_additional_20261121.tsv`. 대조표에 원래 문자열, 번역, 기존/신규 `#US` 오프셋, IL 참조 횟수를 기록함.
- **기술 처리**: 기존 번역 DLL의 사용자 문자열 힙 뒤에 1976바이트를 추가하고 해당 IL 토큰 58개를 신규 오프셋에 연결. CLR 메타데이터 크기 및 PE 이미지/섹션 크기를 함께 갱신. DLL 용량 313344 → 315392바이트.
- **정적 검증**: 기존 원문 및 `ldstr` 바이트 패턴 대조, 새 문자열 재디코딩, 전체 수정 토큰 재검증, `.text` 코드 바이트의 그 외 변경 없음(필수 CLI 메타데이터 크기 변경만 예외). **실제 CLR 로딩/게임 구동은 미검증**이며, 파일은 시험본이다.
- **다음 단계**: 남은 문자열의 기능/표시 구분과 배치별 검수, 기존 DLL 이외 확장 DLL 작업 및 인게임 확인.

## 2026-11-21 — ShadowTrade.dll 활성 문자열 추가 번역

- 기존 한국어 `ShadowTrade.dll`을 기준으로 실제 IL `ldstr` 참조가 있는 초기화·호환성·시스템 진단·인터페이스·오류 문구 **55종(62곳)**을 원문별 1:1 검토 후 반영. 원본 `TentacleMod/`는 변경하지 않음.
- 번역 DLL `LewdMemeShadowTrade1.6.4/ShadowTrade.dll`과 대조표 `번역대조표/ShadowTrade_DLL_additional_20261121.tsv` 동시 수정. 대조표는 기존·신규 `#US` 오프셋, 참조 횟수, 원문/번역을 포함.
- 기존 `#US`를 보존하면서 새 번역 2832바이트를 추가하고 활성 `ldstr` 토큰 62곳 갱신. CLI 메타데이터 크기와 PE 섹션/이미지 크기를 확장. 크기 327168 → 329728바이트.
- **정적 검사**: 전체 신규 문자열 역디코딩, 참조 수, 그 외 `.text` 바이트 불변 확인(필수 메타데이터 크기 갱신 제외). **실제 게임 로딩/기능 테스트는 미실시**. 내부 판정용으로 추정되는 짧은 중국어 비교값은 번역에서 제외.

## 2026-11-21 — ItemPortraitHD.dll 활성 문자열 추가 번역

- 기존 시험용 `ItemPortraitHD.dll`에 남아 있는 실제 `ldstr` 참조의 이미지 출처·호환성·UI 상태·오류 문구 **66종(71곳)**을 1:1 검수해 추가 반영. 체형/세력/연령 구분 등 게임 판정값으로 쓰일 수 있는 문자열은 제외.
- 작업 파일: `ItemPortraitHD_v1.3.0/ItemPortraitHD.dll`, `번역대조표/ItemPortraitHD_DLL_additional_20261121.tsv`. 원문/한국어, 기존·신규 `#US` 오프셋 및 참조 횟수 기록.
- 수정 방법: 기존 사용자 문자열을 유지하며 새 `#US` 2802바이트를 추가, 활성 IL `ldstr` 토큰 71곳을 신규 문자열로 연결. PE/CLR 메타데이터와 섹션 크기 갱신, DLL 크기 275968→278528바이트.
- **정적 검사**: 새 문자열 재디코딩, IL 토큰 참조 횟수, `.text` 비대상 코드 바이트 불변 확인. **실제 CLR 로더·게임 실행은 미검증**. 원본 모드 폴더 무변경.

## 2026-11-21 — DLL 추가 작업 배치 총합 및 재조회 검증

- **이번 추가 반영**: `MidnightPawn.dll` 56개 문자열/58곳 IL, `ShadowTrade.dll` 55개/62곳, `ItemPortraitHD.dll` 66개/71곳. 세 DLL 합산 **177개 문자열, IL 참조 191곳**. 기존 205개/244곳과 합쳐 현재 시험 DLL 총 **382개 문자열, 435곳 IL 참조**.
- **GitHub 재조회 검사**: `MidnightPawn_DLL_additional_20261121.tsv` 56/56행 및 IL 58곳, `ShadowTrade_DLL_additional_20261121.tsv` 55/55행 및 IL 62곳, `ItemPortraitHD_DLL_additional_20261121.tsv` 66/66행 및 IL 71곳; 새 `#US` 문자열 디코딩·토큰 참조·기존 참조 0곳 검사 통과.
- **배포 상태**: 파일은 PE/메타데이터를 수정한 **시험용 DLL**이고, 실제 게임 로더에서 열리는지와 인게임 기능이 정상인지 확인하지 못했으므로 **배포 완료가 아님**. 원본 폴더 `TentacleMod/`는 수정하지 않았음.
- **변경 방식**: 번역 TSV와 `ldstr` 수정 DLL을 함께 보관. 이후 문구 변경 시 TSV의 원문·번역·오프셋을 기준으로 정확히 찾아 수정할 수 있도록 함.

- **CSV 통합 인덱스 추가**: `번역대조표/DLL_additional_20261121.csv`에 이번에 실제 DLL에 추가 적용한 177개 번역 전부를 1:1로 정리함(모드 DLL 이름, 이전/신규 오프셋, IL 참조 수, 중국어 원문, 한국어 번역). 개별 TSV도 보존하며, CSV를 이후 번역 변경·업데이트 비교에 활용할 수 있음. 이전 205개 번역은 개별 기존 JSON 대조표에 보관되어 있으므로 이 신규 CSV에 포함되지 않음.

## 2026-11-21 — ItemPortraitHD.dll 후속 UI·설정 번역

- **추가 45개 문자열, 활성 IL 참조 47곳**을 DLL에 반영. 초상화 소스·가중치·성인 체형·F7·이름 변경 제한·호환성 및 설정 툴팁 중심. 내부 분기·비교에 쓰일 가능성이 있는 중국어 세력·체형 이름, 지역 선택값은 보존. 원본 모드 변경 없음.
- **대조 자료**: `번역대조표/ItemPortraitHD_DLL_additional_20261121_batch2.tsv` 및 기존 `DLL_additional_20261121.csv`에 신규 45행 추가. 기존 대조표 기록은 보존.
- **수정 방법**: `#US`에 7163바이트 추가, 연결된 `ldstr` 토큰 47곳만 수정. 메타데이터 크기와 PE 섹션 크기를 갱신하며 기존 IL 코드와 한국어 번역 참조를 보존. 크기 278528 → 285696바이트.
- **정적 확인**: 신규 문자열 원문 일치·IL 참조 수·한글 UTF-16LE 재디코딩·패치 외 코드 바이트 불변 검증. **게임 로딩과 실제 표시 테스트는 미실시**. 시험 DLL 상태 유지.
- **누적 세 DLL 번역**: 427개 문자열, 482곳 IL 참조(이전 완료분 포함). CSV와 DLL의 완료 상태는 별도로 관리함.
