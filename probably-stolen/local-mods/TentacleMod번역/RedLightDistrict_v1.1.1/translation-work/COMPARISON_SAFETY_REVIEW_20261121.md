# Red Light District v1.1.1 — 한국어 Localization 문자열 비교 및 기존 세이브 안전성 재검수

## 검증 근거
- 원본 메서드별 IL: `translation-work/RLD_COMPARISON_IL_AUDIT_20261121.md`
- 키/호출 위치/기존 번역: `../KEY_COVERAGE_AUDIT_20261121.tsv`
- 원문: `TentacleMod/RedLightDistrict_v1.1.1/Localization/cn.csv` (변경 없음)
- 실제 병합 파일: `Localization/ko.csv`
- 실행 환경의 실제 게임 테스트는 **미실시**. 아래 판단은 .NET IL 호출 흐름에 근거한 정적 위험 제거다.

## 추가로 보호한 10개 키

| 모드 메서드 | 키 수 | 용도 | 조치 |
| --- | ---: | --- | --- |
| `TalentScenarioMaid.ContainerByName` | 4 | `前柜`, `主仓`, `展示柜`, `后舱`를 원문과 정확히 비교하여 보관함 선택 | 검색 기준 원문 유지 |
| `TalentFeeder.AppendFeederLine` | 3 | 급식기 미설치 상태 문구, 상태 출력 접두부, 기존 설명에서 `IndexOf`로 찾는 접두부 | 세 문자열 모두 중국어 원문 유지 |
| `TalentWaterTank.AppendTankLine` | 3 | 정수기 미설치 상태 문구, 상태 출력 접두부, 기존 설명에서 `IndexOf`로 찾는 접두부 | 세 문자열 모두 중국어 원문 유지 |

- 보관함 이름 키 4개를 한국어로 바꾸면 호출부가 넘기는 원래 이름과 일치하지 않아 검색 결과가 달라질 수 있다.
- 급식기/정수기의 `IndexOf` 검색어만 원문으로 유지하고 생성 헤더를 한국어로 두면, 이전 버전의 저장된 중국어 설명과 새 한국어 설명이 섞일 때 같은 상태 줄이 반복 추가될 수 있다. **미설치 상태 문구 + 생성 헤더 + 검색 문자열**을 각각 한 세트로 보호해야 한다.
- 해당 키는 이미 번역한 초안을 폐기한 것이 아니라 `translation-work/internal_string_reference.tsv`에 한국어 참고 번역과 보존 사유를 남겼다.
- 보호는 `merge_localization.py`의 `COMPARISON_PROTECTION`에서 누락 검사를 거쳐 적용되므로 다음 CSV 재생성에서도 덮어쓰지 않는다.

## 비교 연산을 사용하지만 번역을 유지한 6개 키

- `ShowerRoom.RefreshShowerDescription`: 설명 내용 `get_shortDescription/get_longDescription`을 같은 문구와 비교한 뒤 `set_shortDescription/set_longDescription`으로 다시 설정한다. 비교 키 2개는 같은 원문을 쓰는 설정 문자열과 함께 한국어가 일치해 내부 이름 검색과 성격이 다르다.
- `TalentWaterTank.FindTankInCell`: `IsTankItem`로 먼저 대상 아이템을 판정한 뒤, 설명을 비교해 같은 내용으로 다시 설정한다. 비교 키 4개는 해당 설명 설정 키와 같은 한국어 텍스트를 사용하므로 일단 유지했다.
- 두 유형 모두 기존 저장 데이터/상호작용에 대한 인게임 검증은 남아 있다. 이 검수만으로 게임 호환성이 확인됐다고 표기하지 않는다.

## 최종 정적 계수 (2026-11-21)

- 원본 호출 키 **3,619개**
- 한국어 Localization **3,512개**
- 원문 유지(내부 검색·저장·구조 보호) **107개** — 기존 97 + 신규 10
- 미분류 수동 번역 대기 **0개**
- 원본 `RedLightDistrict.dll`과 `TentacleMod/` 내 원문 불변
- 번역본 DLL 재빌드·게임 실행 검증 없음. **외부 `Localization/ko.csv`만 갱신.**

정확한 완료 판정: 한국어 번역 후보에 대한 정적 키 검수 및 기능 위험 보호 완료. 화면 전체 한국어화 또는 실제 구동 완료를 의미하지 않음.
