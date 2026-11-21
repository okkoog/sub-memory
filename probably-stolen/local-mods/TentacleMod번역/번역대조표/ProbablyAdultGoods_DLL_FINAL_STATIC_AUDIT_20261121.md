# Probably Adult Goods DLL — 정적 전수 분류

- 원본: `probably-stolen/local-mods/TentacleMod/可能是成人用品 v1.1.3/probably_adult_goods.dll` / Git blob `320419c86a0e136e0bc9467e94303da81734657e` (58880 bytes)
- 수정: `probably-stolen/local-mods/TentacleMod번역/可能是成人用品 v1.1.3/probably_adult_goods.dll` (103424 bytes)
- 원본 활성 중국어 포함 문자열: 18개. 그중 `物品标签价格.csv missing.` → `물품 태그·가격 CSV 파일(物品标签价格.csv)을 찾을 수 없습니다.` 진단 로그 **1개/활성 IL 참조 1곳** 직접 한국어 DLL에 반영. CSV 파일명 `物品标签价格.csv`는 기능상 그대로 유지.
- 그 외 17개는 CSV 경로, 상품 분류 판정 문자열, 모드 이름/등록명 및 동적 그룹명으로 사용 가능해 **원문 유지**. 단, 실제 호출부와 로딩 확인 없이는 17개 전부를 최종 기능값으로 확정할 수 없으므로 화면 표기 검증이 필요.
- 기존 Localization/ko.csv 192키 중 190키 번역 상태(나머지 2키 원문 보존)는 별도 기록이며 DLL 번역 실적으로 계산하지 않음.
- 정적 #US UTF-16LE 역디코딩 및 IL 참조, 다른 PE/CLI 스트림 보존 확인. **게임 실행/CLR 로드 미검증**.

원문 보존 주요 예시: `Data/物品标签价格.csv`, `日用品`, `成人用品`, `奢侈品`, `垃圾`, `能量电池`, `工具`, `避孕用品`, `假阳具`, `幻龙`, `肛塞`, `奇珍`, `飞机杯`, `SM教具`, `电动玩具`, `额外内容`, `可能是成人用品`.
