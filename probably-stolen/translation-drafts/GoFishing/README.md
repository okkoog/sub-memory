# GoFishing 한국어 번역

- 원본: `en(1).csv`
- 전체: **3,215행**
- 초벌 번역: **3,215 / 3,215 완료**
- 품질 검수: **전체 3,215행 완료**
- 최종 검수본: `GoFishing_ko_reviewed_final_3215.csv`
- 초벌 보존본: `GoFishing_ko_firstpass_complete_3215.csv`
- 이전 중간본: `GoFishing_ko_draft_wip_0965_of_3215.csv`

## 품질 검수 결과

초벌 완료본 전체를 영문 원문과 다시 대조해 용어, 직역투, 고유명사, 어종명, UI 문구를 재검수했습니다.

초벌본 대비 **159개 행을 수정**했습니다.

주요 수정:
- `Large Yellow Croaker` → `부세`
- `Fishing Spot` → `낚시터`
- `Fish Fight` → `물고기 힘겨루기`
- `Fishing Net` → `어망`
- `Makeshift Fishing Rod` → `급조 낚싯대`
- `Mechanical Fish / Biological Fish` → `기계 어류 / 생물 어류`
- `Repair Powder / Excitement Powder` → `회복 분말 / 각성 분말`
- `Auxiliary` 계열 → `보조재`
- `Dream-cap Extract` → `꿈버섯 추출물`
- `Canopy` → `캐노피`
- `reef(s)` → `암초`
- Linguan 특수 지점의 king spot / haul 표현을 `왕어 낚시터 / 어획 횟수`로 통일
- 화물 상자의 `Unpack All` 계열 → `모두 꺼내기`
- 어색한 직역 및 일부 잘못된 기능 설명 교정

## 최종 구조 검증

- 전체 행 수: **3,215행 유지**
- 원본 대비 키 순서 및 키 값 변경: **0건**
- `{0}`, `{1}`, `{0:N0}` 등 플레이스홀더 불일치: **0건**
- 원본 `\\n` 이스케이프 개수 불일치: **0건**
- CSV 값 내부의 실제 개행: **0건**
- 번역 불필요 문자열을 제외한 영문 원문 잔존 없음
- 동일 영문이 여러 위치에 쓰인 항목도 문맥 차이가 없는 경우 용어를 통일함

## 상태

**초벌 + 전체 품질 검수 완료본입니다.**

이후 남는 작업은 실제 인게임에서 UI 폭, 줄바꿈, 캐릭터 말투와 특정 고유명사의 표시 문맥을 확인하는 실기 검수입니다.
