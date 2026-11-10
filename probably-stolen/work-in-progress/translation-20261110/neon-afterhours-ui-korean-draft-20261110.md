# Neon Undertow 'AFTER HOURS · THE STACK' — UI 한국어 사전 번역 초안

**상태: 번역문 준비만 완료. 실제 게임에 적용되지 않음.**

출처: 2026-11-10 사용자가 제공한 인게임 스크린샷 두 장 및 `Player(20261110-102807).log` (Neon Undertow v0.5.15.1).
문제: Neon Undertow `Localization/en.csv`, `Localization/zh.csv`에서 아래 UI 텍스트와 대응하는 원본 키가 발견되지 않음. 사용자가 약속한 원본 모드 비수정·DLL 후킹 금지·전역 런타임 문자열 치환 금지 원칙에 따라 한국어팩에 가짜 `K|` 키를 생성하거나 코드 후킹하지 않음.

## 실제 화면 문구 제안

| 스크린샷 표시 | 한국어 초안 | 주의 |
|---|---|---|
| AFTER HOURS | 영업 종료 후 | 야간 이동 시스템 표제 |
| THE STACK | 더 스택 | 지역 고유명, 음역 유지 |
| AFTER HOURS / 18:00 / 12 blocks | 영업 종료 후 / 18:00 / 남은 12구간 | 실제 blocks가 이용 가능 이동 구간인지, 시간 단위인지 모더 확인 필요; 현재는 잠정 |
| At Player Shop | 현재 위치: 플레이어 상점 | 위치 표기 |
| Service until 24:00 | 영업 종료: 24:00 | 시간 제한 의미 확인 |
| Travel bag | 외출 가방 | 기존 게임 용어와 대조 |
| Current floor | 현재 층 | 층 표시 |
| LIFT STOPS | 엘리베이터 정차층 | 이동 가능한 구역 목록 |
| Underworks | 하층 작업 구역 | 지명·공식 명칭 확인 필요 |
| Lower Exchange | 하층 거래 구역 | 층 명칭 |
| Midline Commons | 중층 공동 구역 | 지명 |
| Upper Crown | 상층 중심 구역 | 지명·공식 명칭 확인 필요 |
| Directory | 안내도 | 목록·지도 기능 |
| East market lane | 동쪽 시장 거리 | 지명 |
| Return home | 귀가 | 액션 버튼 |
| Market square | 시장 광장 | 하층 거래 구역 세부 위치 |
| Select a stop | 정차 지점 선택 | UI 지시 |
| COM(M)ISSARY | 보급소 | 이미지 간판일 수 있음; 코드 번역 대상 불명 |
| Dr Jackson's EQUIPMENT TRADER | 잭슨 박사의 장비점 | 이미지 간판일 수 있음 |
| THE CANNERY | 통조림 공장 | 이미지 간판일 수 있음 |
| Switchback NOODLE COUNTER | 스위치백 국수 가게 | 이미지 간판일 수 있음 |
| RELAY BAR | 릴레이 바 | 이미지 간판일 수 있음 |

## 실제 적용을 위해 필요한 것

1. 모더가 'The Stack' 야간 이동 UI의 동적 글자를 `ModHelper.GetLocalized("water_seller_english_collection", "...")` 등 공식 API와 연결하고, `Localization/en.csv` 및 `zh.csv`에 실제 사용되는 키를 등록해야 함. 임의의 키 이름을 만들어 한국어팩에 먼저 추가해도 실제 UI가 해당 키를 요청하지 않으면 아무 효과가 없음.
2. 모더가 등록한 실제 키를 확인하면 한국어팩 `Localization/pskb__3833095213.csv`에 1:1로 번역을 추가하면 됨. 새 DLL이나 외부 DLL 패치는 필요 없어야 함.
3. 지도 배경 그림에 박힌 영어 간판은 별도 이미지 리소스일 수 있으므로 텍스트 번역과 따로 판단.
4. `blocks`가 실제로 시간 단위인지, 이동 횟수인지 확인되기 전에는 잠정 번역을 배포에 사용하지 말 것.

**변경 범위:** 이 문서는 작업 초안이다. 실제 모드 파일, 한국어팩 CSV 및 DLL은 이 문서로 변경되지 않음.
