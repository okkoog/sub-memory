# Post Item Translation Probe — 확장 테스트팩 (2026-11-15)

**후킹 없음 / 기존 한국어팩 무변경 / 원본 모드 DLL 무변경.**
실험용으로만 사용하세요. 게임 내 모든 번역이 보장되는 버전이 아닙니다.

## 설치용 ZIP 구성
- `PostItemTranslationProbe/` : 이전 v0.3.0의 공식 API 기반 CSV 번역 엔진 + 확장 CSV 세 개
- `ProbePaperFixture/` : **선택 설치**하는 별도 종이 문서 모드. 외부 UI 번역 가능성을 검증하기 위한 시험용 문서만 추가
- `README.ko.md` : 본 파일

게임 종료 후 설치 위치:
`G:\steam\steamapps\common\Probably Stolen\Mods\`

기존 `Mods/PostItemTranslationProbe` 폴더의 `Translations/items.csv`를 삭제하고, 최신 ZIP의 `PostItemTranslationProbe` 폴더로 교체합니다. UI 실험을 할 때만 `ProbePaperFixture`를 별도로 설치·활성화합니다.

## CSV는 모드별 파일로 관리

```text
PostItemTranslationProbe/
├── PostItemTranslationProbe.dll
├── manifest.xml
└── Translations/
    ├── lewd_meme.csv             # 원본 촉수 모드, 9개 아이템·19행
    ├── lewd_meme_remake.csv      # 리메이크, 8개 아이템·17행
    └── probe.paper_fixture.csv   # 선택형 종이 문서 시험, 4행
```

기능 구분은 CSV의 Type 열에서 합니다. 신규 모드를 추가할 때는 `<실제 ModID>.csv` 파일을 새로 만들기만 하면 됩니다. **기존 DLL을 다시 빌드할 필요가 없습니다.**

```csv
Type,Target,Field,Original,Korean
Item,tentacle_body,name,触手本体,촉수 본체
Item,tentacle_box,name,触手苗床箱,촉수 묘상 상자
```

공통 CSV 헤더: `Type,Target,Field,Original,Korean`. `Item`의 Field는 `name`, `shortDescription`, `longDescription`, `flavorText`. `Paper`는 Target=문서 ID, Field=문서 내부 요소 ID. 같은 모드의 아이템과 문서 문구는 **한 CSV에 함께** 넣습니다.

- 원문이 정확히 일치할 때만 번역합니다. 불일치·null·누락은 원문 유지.
- UTF-8/따옴표/쉼표/줄바꿈/변수 유효성 검사 포함.
- CSV에서 한 모드의 데이터가 깨지면 그 모드 파일만 건너뜁니다.
- CSV 편집 후 **게임 완전 재시작** 필요.

## 1. 아이템 확장 테스트

원본 `lewd_meme` v1.7.1과 리메이크 `lewd_meme_remake` v1.5.24를 활성화합니다. 두 파일의 36개 번역 행은 제공된 TEST.zip의 **DLL UTF-16 문자열 및 리메이크 소스에 모두 대조하여 일치**했습니다.

- 기존 시험: `tentacle_body` 이름 → `촉수 본체`
- 신규 시험: `alarm_clock` 이름/짧은 설명/부가 설명
- 신규 시험: `tentacle_box` 이름/짧은 설명
- 신규 시험: `wondrous_milk` 이름/짧은 설명/부가 설명
- 다른 모드 시험: `milk_rich`, `milk_diluted` 이름/설명
- 다른 모드 시험: `barrel_milk_rich` 등 이름/부가 설명

F8 또는 F9 개발자 메뉴에서 아이템을 새로 생성하여 확인합니다. F8의 `help`와 `man` 명령으로 해당 빌드의 실제 스폰 명령을 먼저 확인하세요. **저장된 기존 아이템**은 생성 함수가 다시 실행되지 않으면 미번역 상태일 수 있습니다.

리메이크의 `barrel_*` 아이템은 원본 모드가 추후 `shortDescription`을 재생성하므로, 일부러 이름과 `flavorText`만 대상으로 삼았습니다.

## 2. 종이 문서 UI 실험 (선택)

이전 업로드의 세 모드에는 공식 PaperUI를 쓰는 실제 문서가 확인되지 않았습니다. 그래서 별도 `ProbePaperFixture` 모드를 제공했습니다. 이 fixture는 번역 DLL을 참조하지 않으며 **원래 영어로 된 두 페이지의 문서**를 추가합니다.

`PostItemTranslationProbe`와 `ProbePaperFixture`를 둘 다 활성화한 뒤, 시험용 세이브에서 F8 콘솔을 엽니다.

```text
paper-list
paper-open probe.paper_fixture:translation_probe
```

기대되는 출력:
- 첫 페이지 제목: `생성 후 번역 테스트`
- 첫 페이지 본문: `이 문장은 다른 모드의 종이 문서에서 출력한 것입니다.`
- 닫기 버튼: `닫기`
- 두 번째 페이지 본문: `두 번째 페이지: 영어 원문이 한국어로 바뀌었습니다.`

첫 페이지·두 번째 페이지를 넘겨 반복 표시도 확인합니다. fixture를 비활성화하고 게임을 재시작하면 해당 CSV는 있으나 처리 대상은 없어야 합니다.

**중요:** 이 문서는 *실제로 게임에 존재하는 다른 모드*의 문구가 아니라, 후킹 없이 PaperUI 출력을 시험할 수 있도록 만들어 둔 **별도의 가짜 문서**입니다. 일반 게임 UI나 NPC 대사 번역 성공을 증명하지 않습니다. 게임 내에서 실제로 표시되는지는 아직 미검증입니다.

## 3. 남은 구조적 한계
- 공식 디렉터리를 우회한 생성·이미 보유한 저장 객체·생성 후 재대입은 아이템 번역을 우회할 수 있음.
- 다른 모드가 등록한 factory를 교체하므로 **아이템 출처 표시가 실험 모드로 보일 수 있음**.
- PaperUI의 동적 텍스트·자체 창·NPC 대사는 미지원.
- Harmony/IL 패치·전역 문자열 강제 치환·다른 모드 DLL 수정 없음.
- 테스트 모드가 실행되지 않거나 CSV 파일이 없는 경우 번역 기능은 원본 결과를 유지하도록 설계됨.

### 빌드 상태
GitHub Actions에서 게임 참조 DLL로 컴파일하고 스텁 단위 테스트를 시행합니다. 성공하더라도 실게임 검증과 동일하지 않습니다.
