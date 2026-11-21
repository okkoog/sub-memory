음란한 심우주: 더 많은 상호작용 v1.0.6 — 외부 콘텐츠 한국어화

대상: lewd_meme_legacy_content (48개 음성 파일 버전)
필수 선행 모드: lewd_meme

1. 설치
- 원본 v1.0.6을 설치한 후 게임을 종료합니다.
- 번역 파일의 Content/content.json을 원본 모드의 Content/content.json 위치에 덮어씁니다.
- 원본 DLL, 리소스, 음성 파일(Sounds) 및 모드 ID는 변경하지 않습니다.
- 기존에 제공된 한국어 DLL도 동일한 v1.0.6용이라면 유지할 수 있습니다.

2. 현재 번역 상태 (원본 JSON 구조 기준)
- descriptions.entries 72개: 비어 있지 않은 71개 중 70개 번역 변경, 변수만 있는 1개 원문과 동일, 1개는 원본부터 빈 값.
- 캐릭터 표시 label 53개: 52개 한국어화, 1개 원문 유지.
- dialogue 198개: 170개 한국어화, 28개 원문 유지.
- descriptions.nightScenes 5개: 5개 모두 한국어화.
- manifest 표시명/설명: 2개 한국어화.
- game parser에 전달되는 id, bodyId, personalityId, factionId, tag/weight 구조는 변경하지 않았습니다.
- {name}, {personality}, {faction}, {zone}, {0} 등 원본 변수와 배열 순서는 그대로 유지했습니다.

3. 다른 파일
- README.md와 安装说明.txt는 별도로 한국어 번역돼 있습니다.
- 원본 DLL에 존재하는 C# 코드상 문자열은 이 JSON 작업으로 변경되지 않습니다.
- 원본 설치 파일의 Content/content.json만 교체하는 추가 번역 패치입니다.

4. 작업 기록
번역대조표/LegacyContent_v1.0.6_CONTENT_JSON_AUDIT_20261121.md
게임 구동 여부는 본 번역 파일의 텍스트 검수와 별개입니다.
