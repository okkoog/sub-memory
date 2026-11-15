# Post Item Translation Probe v0.3.2 — 촉수 시작 특성 번역 추가

**독립 시험 모드. 기존 통합 한국어팩 및 촉수 원본 DLL 변경 없음. Harmony/IL 패치/메서드 후킹 없음.**

이번 v0.3.2는 원본 촉수 모드 `lewd_meme`의 **시작 특성**을 아이템과 같은 외부 CSV에서 번역하도록 확장했습니다. 시작 특성 화면은 게임에 진입하기 전이므로, CSV를 먼저 읽고 공식 공개 특성 객체의 표시용 `nameProvider`, `descriptionProvider`만 래핑합니다. 비용·아이콘·효과·식별자는 손대지 않습니다.

### CSV 구성

`PostItemTranslationProbe/Translations/lewd_meme.csv` **한 개만** 사용합니다.

| 종류 | 수량 | 적용 |
| --- | ---: | --- |
| `Item` | 정적 문구 34행 / 아이템 14종 | 게임 디렉터리 생성 시 |
| `Perk` | 2행 / 시작 특성 1종 | **게임 시작 특성 선택 화면이 열리기 전** |
| `Paper` | 0행 | 이번 촉수 모드에는 실증 대상 없음 |

CSV 예시:

```csv
Type,Target,Field,Original,Korean
Item,tentacle_body,name,触手本体,촉수 본체
Perk,lewd_meme.tentacle_origin,name,触手起源,촉수 기원
Perk,lewd_meme.tentacle_origin,description,원본 전체 문장,한국어 전체 문장
```

마지막 설명 행은 구조 예시입니다. **실제 CSV에는 원본 중국어 문장과 전체 한국어 번역을 그대로 입력**했습니다. 대상 추가는 DLL 수정 없이 CSV에 행을 더하는 방식이며, 모드는 `ModLoader.Mods`에 활성 상태로 로드되어야 합니다.

### 설치

게임 종료 후 ZIP의 `PostItemTranslationProbe` 폴더를 다음 위치에 덮어씁니다.

`G:\steam\steamapps\common\Probably Stolen\Mods\PostItemTranslationProbe`

기존 `Translations/items.csv`나 리메이크·가상 문서 시험용 CSV는 제거합니다. 원본 촉수 모드 `lewd_meme`를 켜고 게임을 완전히 재시작하세요.

### 테스트

1. 새 게임 만들기 → **시작 특성** 화면으로 들어갑니다. 촉수 특성 이름이 `촉수 기원`으로 표시되는지 확인합니다.
2. 마우스를 올려 특성 설명이 한국어로 나오는지 확인합니다. 시작 아이템·특성 슬롯 +3·일일 수집 +3·여성 손님 유입 설명이 그대로 보존돼야 합니다.
3. 실제로 해당 특성을 선택하고 게임에 진입합니다. 기존에 성공한 `촉수 본체` 아이템 이름도 한국어인지 확인합니다.
4. 시험 번역 모드를 비활성화한 뒤 재시작하면 특성 이름·설명은 원문으로 돌아와야 합니다.
5. 원본 촉수 모드 없이 번역 모드만 켰을 때 시작 특성 UI나 게임 초기화 오류가 없어야 합니다.

### 주의

- 게임 DLL 공개 API `StartingPerkList.GetModPerk`와 `StartingPerk.nameProvider/descriptionProvider`를 사용합니다. 다른 모드에서 특성 정의를 삭제·변경했거나 원문이 정확히 일치하지 않으면 건드리지 않습니다.
- 표시 함수의 원본값을 매번 확인하고 한국어로 변환합니다. 로딩 중 등록이 늦어도 공식 `OnModAssetsLoaded` 이벤트에서 재검사합니다. 매 프레임 검색하지 않습니다.
- 번역 모드를 비활성화할 때 **자신이 등록한 표시 함수인 경우에만** 원본으로 되돌립니다. 다른 모드가 나중에 교체했다면 건드리지 않습니다.
- **실제 게임에서 초기 특성 UI가 번역되는지는 아직 미검증**입니다. 컴파일 및 테스트 스텁 통과 여부와 구분해야 합니다.
- 시작 특성 문제와 별개로 동적 툴팁·야간 보고서·일반 NPC 대사는 현재 엔진에서 모두 지원하지 않습니다.
- 이 시험판을 완전 번역본으로 배포하지 마세요. 기존 한국어팩과도 별개입니다.

### 공식 API 근거

- 게임 DLL: `StartingPerkList.GetModPerk(string)`, `StartingPerk.nameProvider`, `StartingPerk.descriptionProvider`, `ModHook.OnModAssetsLoaded`.
- 공개 문서: https://github.com/dragonpan2/probably-stolen-modding/blob/3541332ad755a07e423077659863268256ccd233/ModdingGuide.md#starting-perks
