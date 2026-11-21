# 미완료 촉수 모드 원본 6개 버전·ID·접근 감사 (2026-11-21)

기준: `probably-stolen/local-mods/TentacleMod/`의 실제 GitHub `manifest.xml`. 폴더명보다 manifest `ID`와 `ModVersion`을 우선.

| 원본 폴더 | 실제 모드 ID | 버전 | DLL 크기 | 검수 판정 |
| --- | --- | --- | ---: | --- |
| `3842247788` | `probably_eleanor_followup` | 1.0.0 | 16,418,816 B | v1.0.3과 동일 ID의 구버전, DLL 번역 미착수 |
| `落魄千金-埃莉诺 v1.0.3` | `probably_eleanor_followup` | 1.0.3 | 16,439,808 B | v1.0.0보다 신버전, DLL 번역 미착수. 중국어/영어·텍스처 DLL 내장 |
| `3842259965` | `probably_adult_goods` | 1.1.1 | 8,166,912 B | 같은 ID의 다른 성인용품 버전과 충돌, 직접 DLL 번역 미착수 |
| `3843142171` | **`probably_adult_goods`** | 1.1.3 | 16,721,920 B | DLL은 `fallen_heiress_eleanor.dll`이지만 ID는 adult_goods. native 기능/BridgeInit 포함, DLL 미검증 |
| `Bai She's R18 Texture Pack 1.4.0` | `baishe_r18_pack` | 1.4.0 | 55,459,840 B | 제작자 설명상 이미지 전용; 별도 한국어 metadata/안내서 제작 |
| `RedLightDistrict_v1.1.1` | `red_light_district` | 1.1.1 | Git LFS 포인터 134 B, 실제 152,970,752 B | 실제 DLL 본문 미확보, 직접 DLL 번역 불가 |

## 기존 번역과 중복 ID

- 이미 별도 한국어 시험 DLL이 있는 `3843125702`도 ID가 `probably_adult_goods`이고 manifest 버전은 **1.2.1**. `DsBhyuStart.dll`과 native 선언이 있음. 실게임/보안 실행 검증은 별도 미실시.
- `可能是成人用品 v1.1.3`은 폴더 이름과 달리 manifest `ModVersion`은 **1.03**, ID `probably_adult_goods`. 같은 ID의 패키지끼리 동시 활성화하지 말 것.
- `3842247788`(v1.0.0)과 `落魄千金-埃莉诺 v1.0.3`은 같은 `probably_eleanor_followup` ID. 동시 사용 버전이 아닌 구·신 버전 관계.
- `3843142171`의 native 로더/페이로드를 직접 실행하거나 기능이 검증됐다고 가정하지 않음.

## 한국어 번역 산출물과 제약

- `Bai She's R18 Texture Pack 1.4.0`: 원본 `R18TexturePack.dll`과 이미지 무수정. `TentacleMod번역/Bai She's R18 Texture Pack 1.4.0/manifest.xml`의 Name/Description만 한국어화하고 `README_ko.txt`를 제작함. `ProbablyStolen_BaiShe_R18_Pack_Korean_Metadata_20261121.zip` 3,588바이트(2개 텍스트 파일). 원본 모드에 덮어쓸 오버레이이지 단독 실행 모드나 DLL 수정본 아님.
- `3842259965/probably_adult_goods.dll`은 8,166,912바이트인데 GitHub 연결 `fetch_file(encoding=base64)`에서 content가 **빈 문자열**로 반환됨. GitHub raw blob 조회는 UTF-8 디코딩 제한으로 거부. 실제 바이너리 내용이 없는 상태에서 추측으로 DLL 패치 금지.
- `RedLightDistrict.dll`은 LFS 포인터 `oid sha256:4f4f20a00e4754302c53879bfaeb7a7503df10e2f05ff6ad3744be31088159f8`, `size 152970752`로, GitHub에서 실제 원본 바이트를 확보하지 못함.
- 전체 Unity/CLR 게임 실행·세이브·호환성 검증은 실시하지 않았음. 15번째 DailyWearWorkshop의 정적 검수 완료와 전체 프로젝트 완료는 별개.
