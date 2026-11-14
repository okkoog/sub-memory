# Brewing Expansion v3.6.8.1 — 한국어 전수 재검수 (2026-11-15)

## 범위
- Steam 창작마당 ID 3829730904. GitHub zh.csv/en.csv와 통합 한국어팩 CSV 각각 551키.
- 551개 키를 한국어 CSV 순서대로 중국어 원문과 비교, 영어는 의미 확인용 보조자료로 사용. 수치·조건·UI·NPC 어체·제조법의 결과물명 점검.
- 이전 교정 25키 상태를 출발점으로 이번에 **추가 19키 수정 / 532키 유지**. 기타 기존 키 값은 보존.
- 다른 한국어팩 Probably Translated v1.0.19의 양조 CSV는 551키. 이견이 있는 표현을 참고했으나 **복사하지 않음**. 상대 번역의 551개 전체를 키별로 우열 평가한 것은 아님.
- 전체 551행의 중국어 원문, 영어 참고, 교정 전후 번역과 유지·수정 여부: `probably-stolen/analysis/BREWING_V3681_DEEP_REVIEW_551_20261115.csv`. 변경한 19개 키에는 개별 근거를, 유지 키에는 근거 없는 설명을 꾸며 쓰지 않음.

## 주요 교정
- `be_stardust_berry_fruit_description`: 떠난 뒤 남은 말들이 여전히 빛난다는 원문 후반부 누락 복원.
- `be_midautumn_bag_description`: 怀袖 한자음 '회수'를 뜻이 통하는 '소매에 품다'로 번역.
- `be_47_love_cup_description`: 47이 수분 섭취를 걱정하는 상대가 xuxu라는 점 명확화.
- `be_recipe_6_name/description`: 재스민 꽃과 청포도 혼합 제조법을 '열매 둘'로만 설명하던 문제 교정.
- `be_perk_aversion_description`: 47이 판매 목록에 진열하는 씨앗 수량 감소라는 효과 명확화.
- `be_forty_seven_greeting`: 47에게서 산 씨앗으로 빚은 술을 니니에게 마시게 하는 행위 및 니니가 이미 다녀간 조건 복원.
- `be_liunian_current_stats`, `be_machine_night_energy`, `be_machine_pulp_yield`, `be_incubator_fruit_quality`: 숙성/작동 회차와 품질 단위 교정.
- 외곽구역 평판 단계명 및 분위기 문장의 追杀, 挚友, 最足的诚意 등 의미 구분.
- `be_yujin_wine_description`: 열매 두 '종류'가 아니라 두 '개'로 교정.
- `be_nini_trade_accepted`: NPC 인수 완료형 시제 복원.

## 검사 및 한계
- 한국어/중국어/영어 각각 551개 키, 누락·중복 0, 중국어 대비 변수 불일치 0, 한국어 CSV의 모든 K|·M|·W| 행 2열 정상.
- 기존 주석 1행과 M|·W| 메타데이터 보존.
- 제조법 8페이지의 결과물 55개를 중국어 아이템명부터 역추적해 한국어 아이템명과 대조한 결과, 불일치 0개.
- 실게임 UI 줄바꿈, NPC 분기, DLL의 실제 동작은 실행 검증하지 않음. 스팀/DLL 미수정.
