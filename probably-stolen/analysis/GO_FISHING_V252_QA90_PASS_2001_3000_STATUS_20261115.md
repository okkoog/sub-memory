# Go Fishing v2.5.2 — 90% 신뢰도 목표 추가 검수: 2001~3000번 (2026-11-15)

## 검수 범위 및 판단 기준
- 모드 ID: 3829110153. GitHub의 zh.csv/en.csv/통합 한국어팩 각 3,448개 고유 키를 사용함.
- 검수 범위: **2,001~3,000번, 정확히 1,000개**. 키 배열은 한국어 CSV의 K| 레코드 순서를 따름.
- 중국어 원문을 우선해 의미, 수치, 조건, 인칭/대사, UI 동작과 명칭을 문장 단위로 대조하고, 영문을 보조 기준으로 사용. 분말·물약·제조법 용어는 같은 모드 안의 관련 키와 비교.
- 이번 검수 판정: **수정 23개, 보류 2개, 유지 975개**.
- 상세 기록: `analysis/GO_FISHING_V252_QA90_PASS_2001_3000_20261115.csv` (원문/영문/수정 전후/판정/근거).

## 확정 교정 23건
- `item.gf_strong_fish_mince_bait.short`: 血色鱼影를 어군이 아닌 핏빛 그림자로 교정
- `item.gf_swordfish_rostrum.short`: 吻突를 동일 아이템 명칭 '주둥이돌기'로 통일
- `linguan.echo.done`: 疏通은 수문을 제거하는 것이 아니라 막힌 통로를 뚫는 동작
- `linguan.region.central`: 环林河道는 숲 주변을 도는 수로이며 단순 '숲 강'이 아님
- `mechanical.electricity.insufficient`: 电力与能源不足의 병렬 조건을 원문대로 표기
- `mechanical.power.accident`: 电站漏电는 일반 방전이 아니라 발전소 누전
- `mechanical.tuning.active`: 동일 장애물 电流场의 명칭을 다른 UI의 '전기장'과 일치
- `potion.story.dialogue.martin_retry.0`: 后门(뒷문) 위치 정보 누락 복원
- `reagent.confirmDiscard`: 材料不会返还는 구매 금액 환불이 아닌 재료 미반환
- `reagent.confirmLoad`: 중문 원문의 원액 저장조 이동과 고체·액체별 처리 동작 복원
- `reagent.knownFormulas`: 扩量은 비율만 조정하는 것이 아니라 배합량 확대
- `reagent.loadFormula`: 自动备料는 곧바로 조제가 아닌 재료 준비
- `reagent.mix`: 补料는 보정이 아닌 추가 재료 보충; 안내문 용어와 통일
- `reagent.naming.minor`: 初级 등급을 부작용 강도 '경미'와 구분
- `reagent.naming.standard`: 中级 등급을 일반적 '표준'이 아닌 중급으로 표기
- `reagent.order.drag.reagentOnly`: 可交付는 꺼낼 수 있음이 아닌 납품 가능 조건
- `reagent.order.drag.risk`: 反馈는 공식 신고가 아니라 손님에게서 받는 평가·피드백
- `reagent.order.visitor.riskConfirm`: 反馈를 '신고'가 아닌 반응·불만으로 교정
- `reagent.targetVolume`: 总量(총량) 안내에서 누락된 분량 의미 복원
- `reagent.ten.guide.rocket`: 火箭鱼는 모드에 존재하는 太空火箭鱼의 약칭; 다른 분말·어종 명칭과 통일
- `reagent.ten.rocket`: 火箭鱼를 太空火箭鱼(우주로켓어)와 통일
- `reagent.tier.description.minor`: 初级를 등급 '초급'으로 통일
- `reagent.tier.description.standard`: 中级를 등급 '중급'으로 통일

## 원문 충돌 및 검증 보류
- `potion.story.dialogue.edwin_retry.0`: 중문은 '喝了药剂后还是能睡着'(복용 후에도 잠들 수 있음), 영문은 'still cannot sleep'; 재방문 맥락은 영문과 일치. 중문 원문 오기 가능성, 번역 유지·보류
- `predator.encounter`: 중문 '每场'과 영문 'Each attempt'의 소모 단위가 다름. 게임 코드 검증 전 '시도할 때마다' 번역 유지·보류
- 기계 낚시 `mechanical.fishing.blood.description`의 중문은 血色鱼群이므로 '핏빛 어군' 유지. 미끼 `item.gf_strong_fish_mince_bait.short`의 血色鱼影와 구분.
- `powder.liquid.rocket`의 중국어 火箭鱼는 이 모드의 太空火箭鱼를 가리키는 문맥이므로 기존 '우주로켓어'를 유지하고 관련 10효과 안내 두 곳을 통일.

## 범위 및 형식 검증
- 수정 전후 한국어 고유 키 3,448개, 중국어·영어 원문 각각 3,448개. 키 추가/삭제 없음.
- 원문·영문 키 누락 0개, 자리표시자 변경 0개, 수정 범위 밖의 기존 번역 변경 0개.
- 기존 1~2,000번 및 3,001~3,448번 번역 값과 메타데이터 미수정. DLL, 원본 모드 파일, Steam 배포 미수정.
- 전체 추가 검수: **3,000/3,448개 (87.0%)**. 다음 구간 3,001~3,448번(448개).
- 이 수치는 검수 진행률이지 객관적 정답률/90% 품질 달성 보증이 아님. 실제 게임 내 효과 및 UI 작동은 별도 테스트 필요.
