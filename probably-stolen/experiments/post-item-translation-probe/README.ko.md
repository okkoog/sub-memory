# Post Item Translation Probe v0.3.0 — 모드별 CSV + 종이 문서 UI

**이것은 실험용 독립 모드입니다.** 기존 통합 한국어팩 및 원본 촉수 모드와 무관합니다. Harmony·IL 후킹을 사용하지 않습니다.

## 모드별 파일 구조

\`\`\`text
PostItemTranslationProbe/
  PostItemTranslationProbe.dll
  manifest.xml
  Translations/
    lewd_meme.csv
    another_mod.csv         (추가할 때 DLL 수정 필요 없음)
\`\`\`

CSV 파일명은 **정확한 게임 Mod ID**여야 합니다. 엔진은 \`ModLoader.Mods\`에서 실제 로딩된 모드를 한 번 검사해 **해당 ID의 CSV가 존재하면 자동으로 로드**합니다. 별도의 \`index.csv\`도 필요 없습니다.

### CSV 형식 (모드별 한 파일)

\`\`\`csv
Type,Target,Field,Original,Korean
Item,tentacle_body,name,触手本体,촉수 본체
Paper,example_book,title,Original title,한국어 제목
\`\`\`

- \`Type=Item\`: \`Target\`은 아이템 ID, \`Field\`는 \`name\` / \`shortDescription\` / \`longDescription\` / \`flavorText\`.
- \`Type=Paper\`: \`Target\`은 **그 모드가 만든 종이 문서 ID**, \`Field\`는 문서의 **텍스트 요소 ID**. 공식 \`ModHook.OnPaperOpened\`에서 현재 표시 문구가 \`Original\`과 일치할 때 \`SetText\`를 적용합니다. 원본 데이터 파일은 수정하지 않습니다.
- 같은 모드의 아이템·문서 UI 번역을 하나의 CSV에 혼합할 수 있습니다.
- **대사는 아직 지원하지 않습니다.** 이를 위한 범용 대사 출력 후크가 공식 API에 확인되지 않았습니다. \`Dialogue\` 행을 넣으면 임의로 처리하지 않고 해당 CSV를 거부합니다.
- CSV는 UTF-8 BOM, 큰따옴표, 쉼표, 따옴표 이스케이프, 인용된 줄바꿈을 처리하며 변수 \`{0}\` 등을 검사합니다.
- 원문이 일치하지 않으면 아무 속성도 다시 대입하지 않습니다.
- CSV의 특정 모드 파일이 깨져도 나머지 모드 번역은 계속 로드합니다.
- \`Translations/<모드ID>.csv\`를 추가/수정하면 **게임을 완전히 재시작**해야 반영됩니다.

### 촉수 본체 테스트

제공 CSV \`Translations/lewd_meme.csv\`는 \`Item,tentacle_body,name,触手本体,촉수 본체\` 한 줄만 포함합니다. 기존 v0.2.0과 동일하게 신규 시작 특성을 선택한 뒤 이름을 확인하세요.

**종이 문서 기능을 실게임에서 시험하려면**, 공식 PaperUI로 제작된 문서의 정확한 book ID 및 요소 ID, 원문 문구를 조사한 뒤 같은 \`lewd_meme.csv\` 파일에 \`Paper\` 행을 추가해야 합니다. 이 모드에 그런 PaperUI 문서가 존재하는지는 아직 검증하지 않았습니다. 문서 이름을 임의로 가정하여 번역이 적용된 것처럼 주장하지 않습니다.

### 지원 범위 및 위험

- 아이템: 원본 factory를 감싼 새 디렉터리 생성에만 적용. 원본을 정확히 한 번 호출, \`GameItemElement\`의 4가지 문자열 속성만 검사.
- 문서 UI: \`OnPaperOpened\`와 페이지 표시 이벤트를 이용하여 **정적 텍스트/버튼 요소**만 변경. 요소 ID·기존 표시값이 정확히 일치해야 하며 기존 \`loc\`·\`translations\`가 설정된 요소는 건드리지 않습니다.
- 일반 UI, 독자적 모드 창, NPC 대사, 코드에서 직접 조립하는 툴팁은 현재 지원하지 않습니다.
- 문서 모드가 표시 직전에 또는 그 후 \`SetText\`로 값을 다시 설정하면 결과가 덮어써질 수 있습니다. 공식 이벤트 순서가 보장되지 않으면 100% 유지할 수 없습니다.
- factory 교체로 인해 아이템 출처 표시가 시험 모드로 바뀔 수 있으며 출처 태그 변경은 하지 않습니다.
- 모드가 없는 경우 처음 디렉터리 이벤트에서 파일 검색만 하고 모든 번역 이벤트에서 구독 해제합니다. 프레임마다 검색하지 않습니다.
- 공식 API만 사용하되 **실제 게임의 보안 스캐너 통과·새 UI 번역 실동작은 별도의 실게임 시험 대상**입니다.

### 설치
게임 종료 후 ZIP의 \`PostItemTranslationProbe\` 폴더를 다음 위치에 놓습니다.
\`G:\steam\steamapps\common\Probably Stolen\Mods\PostItemTranslationProbe\`

기존 \`Translations/items.csv\` 파일은 **제거**하고 \`Translations/lewd_meme.csv\`로 교체합니다. 기존 한국어팩의 CSV는 수정하지 마세요.

### 빌드
GitHub Actions에서 \`probably-stolen/Managed\`의 참조 DLL을 이용해 .NET Standard 2.1 DLL을 빌드합니다. 단위 테스트는 실제 게임이 아닌 API 스텁을 사용하므로, 통과하더라도 게임 내 실증을 의미하지 않습니다.
