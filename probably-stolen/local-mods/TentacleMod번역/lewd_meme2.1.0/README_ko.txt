음란한 심우주: 아마도 촉수 v2.1.0 — 한국어 DLL 및 기본 분류명

원본 모드 ID: lewd_meme
원본 ModVersion: 2.1.0

이 폴더에서 수정된 MidnightPawn.dll은 앞선 작업에서 한국어를 직접 반영한 번역 시험 DLL입니다.
이번 차수에서는 원본 Localization/zh.csv의 2개 분류 키를 정확히 대조해
Localization/ko.csv를 추가했습니다.

type_tentacle_corrupt = 촉수 침식
type_tentacle_tool = 촉수

적용:
1. 원본 lewd_meme v2.1.0 모드를 설치합니다.
2. 설치된 원본 MidnightPawn.dll은 별도로 보관한 뒤 번역본 DLL로 덮어씁니다.
3. 함께 제공된 Localization/ko.csv를 원본 모드의 Localization 폴더에 추가합니다.
4. manifest.xml, Data/물품 정의 파일, PNG 및 기존 다른 언어 CSV는 교체하거나 삭제하지 마세요.

주의:
- Data/物品定义.csv의 id·이름·분류와 Content/names.txt는
  다른 모드의 이름 비교 및 생성 규칙에 관계될 수 있어 원본 그대로 유지했습니다.
- 본 README의 '분류 2키'는 DLL 코드 문자열 625개 번역 기록과 별도입니다.
- 앞선 번역의 실제 작업량과 추가 키 수는 WORK_PROGRESS.md를 참고하세요.
