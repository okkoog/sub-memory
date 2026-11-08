# Purifier Forecast & Water Testing (물 관리 도우미)

## 한국어

정수기 툴팁에 현재 순도, 하룻밤 후 예상 순도, 고품질수까지 남은 밤과 완료 예정일을 표시합니다. 현재 물·모듈을 유지하고 매일 전원을 공급할 때의 예상이며 고장·전원 부족·도달 불가능 상태도 알려줍니다.

물통을 우클릭하면 내 보유품에서 사용 가능한 무한사용 수질검사기(AquaScan)를 먼저 찾아 게임의 원래 검사 동작을 실행합니다. 검사기를 사용할 때는 검사지를 쓰지 않습니다. 검사기가 없거나 사용할 수 없으면 미사용 검사지를 실제로 한 장 사용하며, 사용하던 키트부터 마저 씁니다. 검사기·검사지가 모두 없으면 검사할 수 없습니다.

내 물과 거래 상대의 물을 검사할 수 있습니다. 잠긴 보관함과 다른 사람 소유의 도구는 사용하지 않습니다. 메뉴에는 선택된 도구가 표시되고, 결과는 해당 도구의 기존 게임 창에 나타납니다. 메뉴 미리보기만으로 수질을 읽거나 검사지를 사용하지 않습니다.

여러 키트가 있으면 미사용 검사지를 적게 보유한 키트부터 사용합니다. 같은 수량이면 사용한 검사지가 남아 있는 키트를 우선하며, 키트 밖의 미사용 검사지도 사용할 수 있습니다. 사용한 검사지는 재사용하지 않습니다. 검사기 자체의 수량·상태는 바꾸지 않으며 게임 기본 검사 기능을 호출합니다.

게임에서 한국어를 선택하면 한국어, 그 외 언어에서는 영어로 표시합니다. 한글패치는 필수가 아닙니다. 구독 → Mods 메뉴에서 활성화 → 게임 완전 종료 후 재실행하세요. 수동 설치본과 창작마당 구독본을 중복 설치하지 마세요.

정수 예상은 현재 물의 실제 성분·양과 기계 성능을 기준으로 하며, 물·모듈 변경 시 다시 계산합니다. 미래의 고장·정전은 예측하지 않습니다. 최대 4,096밤을 계산하며 도달 불가능한 경우도 표시합니다. 모드는 게임 원본 파일과 세이브 형식을 수정하지 않습니다. 검사지를 사용한 결과와 검사기 기록은 기본 게임 동작대로 유지됩니다.

## English

Adds current purity, predicted purity after one night, nights until high-quality water and the expected completion day to purifier tooltips. Forecasts assume unchanged water/modules and power every night. Also identifies faults, insufficient power and unreachable purity targets.

Right-click a water container to test it. A usable reusable AquaScan you own takes priority and uses the game's normal scanner action without consuming test strips. If no scanner is usable, the mod actually uses one unused test strip, preferring partly used kits. Without a usable scanner or unused strip, testing is unavailable.

Supports your own water and merchant water. Tools in locked storage or owned by someone else are excluded. The menu shows the selected tool, and results open in that tool's existing game window. Menu previews never scan water or consume strips.

Kits with fewer unused strips are preferred; equal counts favor kits containing spent strips. Loose unused strips are supported, and spent strips cannot be reused. The scanner's quantity and reusable state are preserved through its native action.

UI follows the game language: Korean for Korean, English otherwise. The Korean language patch is optional. Subscribe, enable the mod in Mods, fully exit and restart the game. Avoid duplicate manual and Workshop copies.

Forecasts use the current water composition, volume and machine performance. Changing water or modules recalculates the estimate. Future faults or outages are not predicted. Estimates simulate up to 4,096 nights and report unreachable targets. Game files and save formats are unchanged. Strip usage and scanner records follow the normal game actions.

## Validation

The scanner priority, native-action routing, consumable preservation, ownership, locks, unavailable resources, bilingual menu and click-time rechecks are covered by 89 automated water-menu checks. Purifier calculations are compared with the installed game's isolated native calculation. This is not a claim of in-game visual verification.
