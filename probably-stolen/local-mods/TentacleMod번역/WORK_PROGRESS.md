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
| `lewd_meme2.1.0/` | `MidnightPawn.dll`, `manifest.xml` (mod v2.1.0), `Data/物品定义.csv` | 중국어 DLL 사용자 문자열 764개 발견, 한국어 대조표 78쌍 작성 |
| `LewdMemeShadowTrade1.6.4/` | `ShadowTrade.dll`, 문서/데이터 | 중국어 DLL 사용자 문자열 969개 발견, 아직 번역본 미반영 |
| `LewdMemeBrothel-0.10.4-candidate/` | DLL 및 `Localization/en.csv`, `zh.csv` | CSV 우선 검토 필요 |
| `lewdMemeBrood v0331/` | DLL, `Localization/`, `Content/` | CSV/콘텐츠 우선 검토 필요 |
| `LewdMemeLegacyContent-1.0.6-48sounds/` | DLL, `Content/content.json` | 번역 적용 방식 검토 필요 |
| `ItemPortraitHD_v1.3.0/` | DLL, C# Source/, 설정 설명 | DLL 중국어 사용자 문자열 228개 발견, 소스 참조 가능 |
| `RedLightDistrict_v1.1.1/` | 대용량 DLL, Git LFS 포인터 | 실제 LFS 바이너리를 확보한 뒤 분석 필요 |
| 기타 이미지 교체 패키지 | manifest, 이미지 등 | 문구 여부 확인 후 필요한 경우만 번역 |

## 실제 작성 파일

- `번역대조표/MidnightPawn.json`: `MidnightPawn.dll` 중국어 원문과 한국어의 대조표 **78쌍**. ※ 이 파일은 번역 초안이며, DLL 반영 완료를 의미하지 않는다.

## 기술 조사 (확인된 사실)

- `MidnightPawn.dll`: CLR `#US` 문자열 힙에 중국어 문자열 764개; 원본 blob SHA `c27be30741564a51f57495a06681974bcc9421ca`.
- `ShadowTrade.dll`: 중국어 문자열 969개; SHA `c400f9dc58750d6b8494725e91e159626611c41c`.
- `ItemPortraitHD.dll`: 중국어 문자열 228개; SHA `57ecff396d96c3f959b08ff775c1e373d3796159`.
- 실제 사용 문자열은 CLR `ldstr` 명령이 `#US`에 있는 문자열을 참조하므로, 단순 바이트 치환이 아니라 메타데이터 및 IL 참조 처리가 필요하다.
- 현재까지 원본 DLL의 GitHub 다운로드 및 CLR 사용자 문자열 추출까지만 확인했다. **게임 내 로딩·플레이 테스트는 미실시.**

## 다음 작업 절차

1. 위 JSON 대조표 중 내부 식별자로 재사용될 가능성이 있는 단어와 불필요한 디버그 문구를 분리하여 안전한 UI 표기 우선으로 선별.
2. 대상 DLL을 원본에서 읽어 별도 한국어 수정 DLL로 생성. 원본 파일에는 쓰기 금지.
3. 수정한 DLL의 PE/CLR 메타데이터, 모든 새 문자열 참조, 원본과 번역본 차이를 기계적으로 검증.
4. 성공한 DLL만 `TentacleMod번역/<원본 하위 폴더>/<DLL 파일명>`에 업로드. 각 DLL별 변경 건수와 검사 결과를 이 문서에 반영.
5. 남은 문자열은 본체부터 순차적으로 1:1 검수. CSV 지원 모드는 DLL을 수정하기 전에 Localization 정상 적용 여부를 확인.
6. 게임 실행 확인 전에는 **임시 테스트본**으로 표시하고, 오류·미번역 범위를 반드시 기록.

## 주의할 부분

- 모드 내부 중국어 체형·속성·분류명은 문자열 자체가 분기 조건 또는 저장값일 수 있다. 표시되는 이름처럼 보여도 실행 흐름을 확인하지 않고 무작정 수정하지 않는다.
- UI에 보이는 중국어가 반드시 `#US`에서만 오는 것은 아니다. 외부 JSON/CSV와 런타임 조합 문자열도 별도로 확인해야 한다.
- 추가 작업 시 각 단계의 실제 반영 여부를 이 문서에 갱신한다. 이력이 없는 작업은 완료로 간주하지 않는다.
