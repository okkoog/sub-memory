# MentorToSarrona.dll 정적 전수 검사 — 2026-11-21

- 기존 시험 DLL의 한국어 변경: 2개 문자열 / 활성 IL 2곳. 이번에는 추가 문자열을 변경할 필요 없음.
- 기존 바이너리에서 활성 중국어 8개를 직접 확인. 전부 RenameText 또는 RewriteLocalizedText의 검색·치환 원문이며, 이 문자열을 한국어로 바꾸면 원문 매칭이 깨질 수 있어 그대로 유지한다.
- CLI #US/ldstr 정적 스캔 완료. 실제 게임/.NET CLR 로드 테스트는 미실시.
- 원본 TentacleMod/ 폴더 변경 없음.
