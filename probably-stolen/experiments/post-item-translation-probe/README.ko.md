# Post Item Translation Probe v0.3.1 — 원본 촉수 모드 단독 번역 시험판

**대상:** `lewd_meme` v1.7.1 — `TEST.zip/lewd_meme1.7.1/MidnightPawn.dll` (SHA-256: `97cb1c61ec1c8ee495c8ab634281eb0ca6fdcb96ce0bceb191d975eb43a055b2`).

## 범위
이 ZIP에는 **원본 촉수 모드용 `Translations/lewd_meme.csv`만** 있습니다. 이전의 `lewd_meme_remake.csv`, `probe.paper_fixture.csv` 및 시험 문서 모드는 배포하지 않습니다. 기존 한국어팩은 수정하지 않습니다.

원본 DLL에서 실제 `Directory.Set` 등록 확인: 아이템 ID 14개. 게임의 공식 생성 함수 경로에서 읽는 **정적 이름·설명·부가 설명 34행**을 CSV에 기록했습니다. 생성 후 텍스트가 바뀌는 문구에는 현재 방식이 항상 적용되지 않습니다.

| 항목 | 현황 |
| --- | --- |
| 모드별 CSV | `Translations/lewd_meme.csv` 한 개 |
| 등록 아이템 | 14종 |
| 정적 번역 행 | 34행 (이름 13, 짧은 설명 11, 부가 설명 10) |
| 신규 게임에서 생성된 정적 아이템 | 번역 적용 대상 |
| 아이템의 동적 설명·툴팁·보고서·설정 UI·대사 | **별도 지원 필요 / 미완료** |
| DLL 후킹·원본 파일 변경 | 없음 |

원본 `MidnightPawn.dll`에서 추출한 중국어 포함 문자열은 중복 제거 **523종**, 총 사용 위치 705개입니다. 여기에는 실제 화면 문구와 내부 분류명·로그가 혼재하므로 전부 CSV에 강제 주입하지 않았습니다. 분류용 검수 목록은 별도 `lewd_meme_1.7.1_full_audit.csv`에 있습니다.

## 설치
게임을 종료한 뒤, ZIP의 `PostItemTranslationProbe` 폴더를 다음 경로로 교체하세요.

`G:\steam\steamapps\common\Probably Stolen\Mods\PostItemTranslationProbe`

기존 파일 `Translations/items.csv` 및 기타 실험용 CSV가 남아 있다면 제거합니다. **원본 `lewd_meme` 모드를 함께 활성화**하고 신규 게임/신규 아이템으로 테스트하세요.

## 외부 CSV 관리 방식
```csv
Type,Target,Field,Original,Korean
Item,tentacle_body,name,触手本体,촉수 본체
Item,alarm_clock,name,闹钟,알람시계
```
- 모드 하나당 파일 하나. 새 모드를 지원하려면 `Translations/<ModID>.csv`를 추가하면 됩니다. 이 시험팩은 다른 모드를 포함하지 않습니다.
- 이름, `shortDescription`, `longDescription`, `flavorText` 네 필드만 지원합니다.
- 원문이 **완전히 일치**할 때만 번역하고, 그렇지 않으면 그대로 둡니다.
- CSV 인용부호/쉼표/개행/UTF-8 BOM/서식 변수 검사 포함.
- CSV 수정 후 게임을 완전히 재시작해야 합니다.

## 완전 번역까지 남은 작업
**현재 완료된 것은 원본 촉수 모드의 정적 아이템 문자열 정리 단계입니다. 완전 번역본이 아닙니다.**

1. `CorruptTentacle.RefreshTooltip`, `TentaclePrinter.RefreshDescription`, `TentacleTrap.RefreshDescription`처럼 **사용 도중 재생성되는 설명**은 생성 시 번역만으로 유지되지 않습니다.
2. `MaidenTooltip.OnTooltip`, `TentacleBody.ShowReport`, `AlarmClock.WriteMorningReport`, `TentaclePrinter.RecipeTip` 등의 화면 문구는 공식 API로 개입 가능한 범위를 추가 조사해야 합니다.
3. 일반 대사·자체 UI 전용 범용 출력 이벤트가 공식 API에 확인되지 않았습니다. **후킹 금지 조건을 유지하면 일부 문구의 인게임 적용은 불가능할 수 있습니다.**
4. 번역용 factory를 등록하면 아이템 출처 모드 표시가 바뀔 가능성이 있으며, 출처 태그를 임의 변경하지 않습니다.

기존 한국어팩과 원본 모드 DLL은 변경하지 않았습니다. GitHub의 별도 실험 브랜치에서만 빌드합니다.
