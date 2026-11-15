# Post Item Translation Probe 0.2.0 (CSV 기반 범용 실험판)

**독립 테스트 모드입니다. 기존 한국어팩, 원본 촉수 모드, main 브랜치는 수정하지 않습니다.**

## 실행 구조
1. 자신의 \`Translations/items.csv\`를 공식 \`ModHelper.ReadModFileText\`로 읽습니다.
2. CSV에 기재된 모드가 활성 설정 상태이고 활성화되어 있으면 해당 아이템 ID의 공용 factory delegate를 검사합니다.
3. 실제로 등록된 factory의 소유 모드 ID와 소유 객체의 IsEnabled까지 일치할 때 원본 함수를 한 번만 실행하는 wrapper로 교체합니다.
4. 그 경로로 아이템을 새로 생성할 때마다 **원문이 정확히 일치한 속성만** 한국어로 바꿉니다. 번역이 없거나 null이면 수정하지 않습니다.
5. 모드 비활성화 시 자신이 설치한 wrapper일 때만 원본 delegate로 되돌립니다. 다른 모드의 후속 교체를 덮어쓰지 않습니다.

## CSV (DLL 수정 불필요)
\`\`\`csv
ModID,ItemID,Field,Original,Korean
lewd_meme,tentacle_body,name,触手本体,촉수 본체
\`\`\`
- 추가 모드마다 위 구조로 **CSV 행만 추가**한 후 게임을 완전히 재시작합니다.
- 네 가지 번역 속성: \`name\`, \`shortDescription\`, \`longDescription\`, \`flavorText\`.
- 쉼표와 따옴표가 있는 원문은 일반 CSV 인용 규칙을 따릅니다. UTF-8 BOM, 인용 셀 줄바꿈 지원.
- \`{0}\`, \`{name}\` 같은 변수는 원문과 번역의 토큰이 같아야 합니다.
- CSV 전체를 읽거나 해석하지 못하면 안전하게 번역을 비활성화합니다.

## 촉수 본체 시험
원본 촉수 모드 v1.7.1의 실제 ID \`lewd_meme\`, item ID \`tentacle_body\`, 원문 이름 \`触手本体\`를 확인했습니다. 새 게임에서 시작 특성 \`触手起源\`을 선택하고 처음 지급되는 촉수 본체의 이름이 **촉수 본체**로 나오는지 확인하세요. 기존 소유 아이템은 소급 변경되지 않을 수 있습니다.

설치는 ZIP 안 \`mod/PostItemTranslationProbe/\` 폴더만 \`G:\steam\steamapps\common\Probably Stolen\Mods\PostItemTranslationProbe\`로 복사합니다. 게임에서 테스트 모드를 활성화한 뒤 재시작합니다.

## 제한과 위험
- 디렉터리 factory를 통하지 않는 자체 생성·PreBuiltItem·복제 후 문자열 재복사·사후 속성 재설정·독립 UI는 못 잡을 수 있습니다.
- 대상 모드가 비활성 또는 미설치이면 factory를 변경하지 않습니다. 활성 설정이 남아 있지만 실제 로딩에 실패한 경우에는 이벤트가 등록될 수 있으나, 소유 모드 확인 단계에서 생성 함수는 변경하지 않습니다.
- 아이템 하위 타입을 안전하게 다루기 위해 기본 \`GameItemElement\`와 정확히 같은 객체만 수정합니다.
- **아이템 출처 표시가 원본 모드 대신 시험 모드로 바뀔 수 있습니다.** 출처 태그는 수정하지 않았습니다.
- directory 초기화 이벤트 순서나 실제 인게임 반복 생성은 **빌드 성공만으로 검증되지 않습니다**. 별도 게임 실행 시험이 필요합니다.
- 원본 factory 예외는 전달하며 재시도하지 않습니다. 번역 실패는 가능한 범위에서 원래 문자열로 복구합니다.
- Unity 게임 스캐너 통과 여부는 실제 실행으로 최종 확인해야 합니다.

## 빌드
GitHub Actions가 \`probably-stolen/Managed\`의 게임 참조 DLL을 이용해 빌드합니다.
\`dotnet build source/PostItemTranslationProbe.csproj -c Release -p:GameManaged="<게임 Managed 경로>"\`
