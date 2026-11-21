# SarronaTheDragon.dll — 정적 번역 검사 기록

- 원본 Git blob: `d0cfa70ac02ef32004da72e61c2473d1aca78cd6`, 크기 237056바이트
- 번역 DLL: `probably-stolen/local-mods/TentacleMod번역/[SRW-STM]SarronaTheDragon/SarronaTheDragon.dll`, 크기 413696바이트
- 원본 #US 전체 문자열 812개, 중국어 0개. 일반 문장 형태의 영어 오류·진단 문구 9개를 새 UTF-16LE 한국어 문자열로 직접 교체(활성 IL 9곳).
- `Metal Printing Module`은 아이템 생성용 검색 이름일 가능성이 있어 보존. 다른 영어 CamelCase/API/태그/아이템 키들도 번역 대상에 합산하지 않음.
- `Localization/ko.csv` 553개 키는 별도. CLR 로딩 및 게임 검증 미실시.

- `Unable to queue Sarrona's blackout visit: ` → `세로나의 정전 방문을 대기열에 추가하지 못했습니다: `
- `Could not retain Sarrona's milk jug while clearing the counter.` → `카운터를 정리하는 동안 세로나의 모유 통을 보관하지 못했습니다.`
- `The food relief visitor could not be queued for ` → `식량 지원 방문객을 대기열에 추가하지 못했습니다. 대상: `
- ` on day ` → ` / 날짜: `
- `Cannot seal an occupied can.` → `내용물이 들어 있는 캔은 밀봉할 수 없습니다.`
- `Unknown printed can: ` → `알 수 없는 프린팅 캔: `
- `[Sarrona Night] Nutrition=` → `[세로나 야간] 영양=`
- `.frames must contain 240 frame indices.` → `.frames 파일에는 240개의 프레임 인덱스가 있어야 합니다.`
- `Invalid Sarrona idle atlas: ` → `세로나의 대기 애니메이션 아틀라스가 올바르지 않습니다: `
