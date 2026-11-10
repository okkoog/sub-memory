# 신규 번역 수동 대조 검수 — 3차 (전체 301~600번)

**검수 대상 300개.** Better Item Tooltips 88~123번(36개), Cyber Fried Rice 1~264번(264개). 영어·중국어 원문과 기존 한국어를 키별로 대조하고 의미, 조건, 수치, UI 표시, 서식을 확인했다.

- **수정 20개, 유지 280개.** 교정은 Cyber Fried Rice에만 적용; Better Item Tooltips 36개는 유지.
- 원본 모드의 DLL/Localization, 한국어팩 DLL, Steam, 기존 ZIP은 변경하지 않았다.
- 코드 동작/화면 출력/폰트 확인은 수행하지 않은 정적 텍스트 검수다. 원문 자체의 게임 동작 설명이 최신 구현과 일치하는지는 별도 인게임 확인이 필요하다.
- 원문 컬럼은 영어와 중국어를 모두 보존한다. `\n`은 CSV에 저장된 이스케이프 줄바꿈을 뜻한다.

## Better Item Tooltips (`3830180473`) — 88~123번

| 전체 순번 | 모드 내부 순번 | 키 | 영어 원문 | 중국어 원문 | 최종 한국어 | 판정 | 항목별 직접 검수 메모 |
|---:|---:|---|---|---|---|---|---|
| 301 | 88 | `sample_output` | Output | 产量 | 생산량 | 유지 | 샘플의 생산량 항목 표제 |
| 302 | 89 | `sample_output_value` | 946 ml/day | 946毫升/天 | 946mL/일 | 유지 | 946mL/일 수치와 기간 단위 일치 |
| 303 | 90 | `sample_power` | Power needed | 所需能量 | 필요 전력 | 유지 | 설비가 요구하는 전력 항목 |
| 304 | 91 | `sample_power_value` | 3/day | 3/天 | 3/일 | 유지 | 하루 3단위 전력 수치 확인 |
| 305 | 92 | `sample_desc` | Produces water when given a container. Needs power. | 在提供容器的情况下可产出水。需要能量。 | 용기를 연결하면 물을 생산합니다. 전력이 필요합니다. | 유지 | 용기 연결·전력 필요 두 조건 유지 |
| 306 | 93 | `sample_tag_household` | [Household] | [日用品] | [생활용품] | 유지 | 생활용품 분류 태그와 대괄호 보존 |
| 307 | 94 | `sample_tag_contraband` | [Contraband] | [违禁品] | [밀수품] | 유지 | 밀수품 분류 태그와 대괄호 보존 |
| 308 | 95 | `sample_stolen` | Stolen (14% heat) | 赃物（14%热度） | 도난품(수배 위험도 14%) | 유지 | 도난품의 위험도 14% 표시 확인 |
| 309 | 96 | `sample_stamp` | Restricted | 中级违禁品 | 규제 물품 | 유지 | Restricted에 해당하는 규제 표기 유지 |
| 310 | 97 | `preview_classic` | Classic style | 经典风格 | 클래식 방식 | 유지 | 기본 창을 이용하는 클래식 양식 |
| 311 | 98 | `gauge_power` | Battery | 电池 | 전지 | 유지 | 전지 계기판 제목과 전원 의미 확인 |
| 312 | 99 | `gauge_liquid` | Liquid | 液体 | 액체 | 유지 | 액체 계기판 제목 |
| 313 | 100 | `row_status` | Status | 状态 | 상태 | 유지 | 설비 상태 행의 제목 |
| 314 | 101 | `sample_battery` | Power cell | 能量电池 | 전지 | 유지 | Power cell을 전지로 표기한 문맥 확인 |
| 315 | 102 | `gene_header` | Genes | 基因 | 유전자 | 유지 | 유전자 정보 그룹 제목 |
| 316 | 103 | `gene_expressed` | Expressed | 表现 | 발현 | 유지 | 발현된 유전자 값의 상태 표시 |
| 317 | 104 | `gene_a` | A | A | A | 유지 | 유전자 대립형 A 표기 보존 |
| 318 | 105 | `gene_b` | B | B | B | 유지 | 유전자 대립형 B 표기 보존 |
| 319 | 106 | `cfg_collapse` | Collapse some parts | 平时收起部分内容 | 일부 항목 접기 | 유지 | 일부 항목을 접는 설정명 |
| 320 | 107 | `cfg_collapse_tip` | The slip shows just the essentials; the parts ticked below stay folded until you hold the expand key, which shows the full slip. A line at the bottom says what is folded. | 估价单平时只显示要点，下面勾选的部分先收起来；按住展开键时显示完整的单子。底部会提示收起了哪些。 | 감정서에는 중요한 정보만 남깁니다. 아래에서 선택한 항목은 펼치기 키를 누르는 동안만 모두 보이고, 손을 떼면 다시 접힙니다. 아래쪽에 접힌 항목 안내가 표시됩니다. | 유지 | 접기·키를 누를 때 펼치기·하단 안내 기능 모두 일치 |
| 321 | 108 | `cfg_expand_key` | Expand key | 展开按键 | 펼치기 키 | 유지 | 펼치기 동작에 할당할 키 설정 |
| 322 | 109 | `cfg_expand_key_tip` | Hold this key to show the full slip; let go to fold it again. | 按住这个键时显示完整的估价单，松开收回。 | 이 키를 누르고 있으면 감정서 전체가 표시되고 놓으면 다시 접힙니다. | 유지 | 길게 누르면 펼치고 놓으면 접히는 동작 일치 |
| 323 | 110 | `cfg_collapse_prices` | Fold price details | 收起价格明细 | 가격 세부 정보 접기 | 유지 | 판매가 세부 항목 접기 기능 |
| 324 | 111 | `cfg_collapse_prices_tip` | The small line under the selling price (list price, value per cell and so on). The big selling price and the purchase price stay. | 售价下方那一行小字（标价、性价比等）。大字售价和进价不收起。 | 판매가 아래의 작은 줄(정가, 칸당 가치 등)을 접습니다. 큰 판매가와 매입가는 유지됩니다. | 유지 | 큰 판매가와 매입가는 접지 않는 예외 유지 |
| 325 | 112 | `cfg_collapse_contents` | Fold contents | 收起内容物 | 내용물 접기 | 유지 | 용기·기계 내용물 접기 기능 |
| 326 | 113 | `cfg_collapse_contents_tip` | The item grids of containers and machines. Battery and tank gauges are readings and stay. | 容器和机器里的物品格子。电池、水箱这类量条是读数，不收起。 | 용기와 기계의 내용물 아이콘을 접습니다. 배터리와 액체 탱크의 계기판은 유지됩니다. | 유지 | 전지·액체 계기판은 접히지 않는 예외 유지 |
| 327 | 114 | `cfg_collapse_desc` | Fold item description | 收起物品描述 | 물품 설명 접기 | 유지 | 물품 고유 소개 문구 접기 기능 |
| 328 | 115 | `cfg_collapse_desc_tip` | The item's own flavour text. | 物品自带的介绍文字。 | 물품 고유 설명문을 접습니다. | 유지 | 게임 물품의 설명문만 접는 범위 확인 |
| 329 | 116 | `cfg_collapse_mods` | Fold mod info | 收起附加信息 | 모드 정보 접기 | 유지 | 다른 모드의 추가 정보 접기 기능 |
| 330 | 117 | `cfg_collapse_mods_tip` | The Mod info section added by other mods, and the mod source line. | 其他模组加的「附加信息」一节和模组来源。 | 다른 모드가 추가한 '모드' 항목과 모드 출처 줄을 접습니다. | 유지 | 모드 정보 영역과 모드 출처 줄 모두 포함 |
| 331 | 118 | `fold_view` | Show {0} | 查看{0} | {0} 펼치기 | 유지 | {0} 자리에 펼칠 항목명을 끼우는 동작 보존 |
| 332 | 119 | `fold_sep` | , | 、 | , | 유지 | 접힌 항목을 나열하는 쉼표 구분자 유지 |
| 333 | 120 | `fold_prices` | price details | 价格明细 | 가격 정보 | 유지 | 접힌 가격 세부 항목 명칭 |
| 334 | 121 | `fold_contents` | contents | 内容物 | 내용물 | 유지 | 접힌 내용물 항목 명칭 |
| 335 | 122 | `fold_desc` | description | 描述 | 물품 설명 | 유지 | 접힌 설명 항목 명칭 |
| 336 | 123 | `fold_mods` | mod info | 附加信息 | 모드 정보 | 유지 | 접힌 모드 정보 항목 명칭 |

## Cyber Fried Rice (`3834071305`) — 1~264번

| 전체 순번 | 모드 내부 순번 | 키 | 영어 원문 | 중국어 원문 | 최종 한국어 | 판정 | 항목별 직접 검수 메모 |
|---:|---:|---|---|---|---|---|---|
| 337 | 1 | `mod_name` | Cyber Fried Rice | 一碗赌上赛博生涯的蛋炒饭 | 사이버 볶음밥 | 유지 | 모드 고유명 간결하게 유지 |
| 338 | 2 | `list_sep` | ,  | 、 | ,  | 유지 | 나열용 쉼표·후행 공백 보존 |
| 339 | 3 | `upgrade_unlock_title` | Takeout Platform Listing | 外卖平台入驻 | 배달 플랫폼 입점 | 유지 | 플랫폼 입점 해금 제목 |
| 340 | 4 | `upgrade_unlock_desc` | Join the platform to receive a cabinet, microwave, terminal, menu board, packing station and 10 meal packs. Start with 5 kits, 5 colas, 5 sausages, a battery and 3000 ml of regular water. | 入驻外卖平台，送后厨柜、微波炉、下单机、线上菜单板、打包台和 10 包料理包，打包台有 5 份材料、小可乐和淀粉肠各 5 个。另送一块电池和 3000 毫升普通水。 | 배달 플랫폼에 입점하면 보관함, 전자레인지, 주문 단말기, 메뉴판, 포장 작업대와 밀키트 10개를 받습니다. 포장 세트 5개, 콜라 5개, 소시지 5개, 배터리 1개, 일반 물 3,000ml도 함께 지급됩니다. | 유지 | 지급 물품 종류·초기 수량 10·5·3000mL 모두 유지 |
| 341 | 5 | `item_kitchen_name` | Back Kitchen Cabinet | 后厨柜 | 조리실 보관함 | 유지 | 후방 조리 공간의 보관함 명칭 |
| 342 | 6 | `item_kitchen_desc` | Stores meal packs, cooked meals, packed takeout and kitchen equipment. | 收纳料理包、熟食、成品外卖和后厨设备。 | 밀키트, 조리된 음식, 포장된 배달 음식과 주방 기구를 보관합니다. | 유지 | 밀키트·완성식·포장식·주방 장비 수납 범위 |
| 343 | 7 | `item_microwave_name` | Second-hand Microwave | 二手微波炉 | 중고 전자레인지 | 유지 | 중고 전자레인지 아이템명 |
| 344 | 8 | `item_microwave_desc` | The only "stove" in the shop. Double-click to open, plug in a battery, put a meal pack inside and it heats itself: ding, done. Efficiency modules save power; quality modules make the food taste more freshly cooked. | 本店唯一的“灶台”。双击打开，插电池，料理包放进去自动加热，叮一声就好。效率模组省电，质量模组让菜更像现炒的。 | 가게에 있는 유일한 '조리 기구'입니다. 더블클릭해 열고 배터리를 연결한 다음 밀키트를 넣으면 알아서 데워 줍니다. 띵! 완성입니다. 효율 모듈은 전력 소모를 줄이고, 품질 모듈은 음식이 갓 조리된 것처럼 느껴지게 합니다. | 유지 | 더블클릭·배터리·모듈 효능과 의성어 모두 확인 |
| 345 | 9 | `item_terminal_name` | Order Terminal | 下单机 | 주문 단말기 | 유지 | 물품 발주 단말기의 명칭 |
| 346 | 10 | `item_terminal_desc` | Right-click to order. Meal packs and equipment arrive in the cabinet next morning. Packing kits, colas and sausages go straight into the packing station stock counters. | 右键打开进货单。料理包和设备次日送到后厨柜，打包材料、小可乐、淀粉肠次日直接补入打包台计数。 | 우클릭해 물품을 주문합니다. 밀키트와 장비는 다음 날 아침 보관함으로 배송됩니다. 포장 세트, 콜라, 소시지도 다음 날 아침 포장 작업대의 재고에 추가됩니다. | **수정** | 포장 재료도 다음 날 재고에 입고된다는 시점 보완 |
| 347 | 11 | `item_kit_name` | Disposable Packing Kit | 一次性打包材料 | 일회용 포장 세트 | 유지 | 포장 한 회분에 쓰는 일회용 세트 명칭 |
| 348 | 12 | `item_kit_desc` | One kit packs one takeout portion. Put as many kits on the counter as the courier has portions; they turn into guochao takeout bags on hand-off. | 一套 = 一份外卖的包装。骑手要几份，就往柜台放几套，交餐时变成国潮外卖打包袋。 | 배달 음식 1인분을 포장할 때마다 세트 하나가 필요합니다. 배달 기사가 가져갈 인분 수만큼 포장 세트를 카운터에 올려 두면, 음식을 건넬 때 궈차오풍 배달 봉투로 바뀝니다. | **수정** | 원문 counter와 작업대를 혼동한 위치 수정 |
| 349 | 13 | `item_kit_flavor` | Contains: a black plastic Spongy Bob spoon, chopsticks, a bad toothpick, a napkin, a guochao takeout bag and a home-made "No Pre-made Food" box. | 内含：海棉宝宝黑色塑料小软勺、筷子、劣质牙签、餐巾纸、国潮外卖包装袋、自制“绝无预制菜”饭盒。 | 구성품: 검은색 플라스틱 '스펀지 밥' 숟가락, 젓가락, 싸구려 이쑤시개, 냅킨, 궈차오풍 배달 봉투, 수제 '미리 만든 음식 아님' 포장 상자. | 유지 | 스펀지 밥 패러디와 부속품 목록·풍자 유지 |
| 350 | 14 | `item_bag_name` | Guochao Takeout Bag | 国潮外卖打包袋 | 궈차오풍 배달 봉투 | 유지 | 궈차오풍 포장 가방의 명칭 |
| 351 | 15 | `item_bag_desc` | Packed takeout. Place the dishes the courier ordered on the counter and accept the offer to hand them over. | 已打包的成品外卖。把骑手要的菜放到柜台，接受报价即可交餐。 | 포장된 배달 음식입니다. 배달 기사가 주문한 음식을 카운터에 올려놓고 제안을 수락하면 건네줄 수 있습니다. | **수정** | 전달 장소가 포장 작업대 아닌 카운터임을 복구 |
| 352 | 16 | `item_cola_name` | Bootleg Mini Cola | 劣质小可乐 | 짝퉁 미니 콜라 | 유지 | 짝퉁 미니 콜라 이름과 품질 뉘앙스 |
| 353 | 17 | `item_cola_desc` | Origin unknown. Customers who liked the shop add one for 1 credit; it costs you 2. | 来源不明。点赞店铺的食客 1 信用点加购，进价 2。 | 출처는 불명입니다. 가게를 마음에 들어 한 손님은 1크레딧을 추가로 내고 콜라를 주문하지만, 가게의 원가는 2크레딧입니다. | 유지 | 손님 판매 1크레딧·매입원가 2크레딧 관계 |
| 354 | 18 | `item_sausage_name` | Bootleg Starch Sausage | 劣质淀粉肠 | 짝퉁 전분 소시지 | 유지 | 짝퉁 전분 소시지의 명칭 |
| 355 | 19 | `item_sausage_desc` | Ingredients unknown. Customers who liked the shop add one for 1 credit; it costs you 2. | 用料不明。点赞店铺的食客 1 信用点加购，进价 2。 | 원재료는 불명입니다. 가게를 마음에 들어 한 손님은 1크레딧을 추가로 내고 소시지를 주문하지만, 가게의 원가는 2크레딧입니다. | 유지 | 소시지 추가 판매가 1·원가 2 유지 |
| 356 | 20 | `item_manual_name` | Takeout Counter Notes | 外卖档口出餐手册 | 배달 작업대 안내서 | 유지 | 배달 작업 안내 문서 제목 |
| 357 | 21 | `item_manual_desc` | Hand-offs, cooking, the microwave, likes and pre-made meals, page by page. | 出餐、做法、微波炉、点赞、预制熟食，一页一页写着。 | 음식 전달, 조리법, 전자레인지, 좋아요, 미리 만들어 둔 음식에 관한 내용을 한 장씩 설명합니다. | **수정** | 플랫폼 좋아요를 호감도로 바꿨던 표현 교정 |
| 358 | 22 | `item_sign_name` | "Freshly Wok-Fried" Sign | “现炒”灯牌 | ‘갓 볶은 요리’ 간판 | 유지 | 현장 볶음 간판 특유의 광고 어투 |
| 359 | 23 | `item_sign_desc` | Put it in the display case. Couriers see it from afar: each day there is a 75% chance one more courier comes (taking one of the day's regular customer slots). | 放进橱窗，骑手远远就能看见：每天有 75% 的机会多来一位骑手（占当天普通顾客的名额）。 | 진열장에 놓으세요. 배달 기사들이 멀리서도 간판을 알아봅니다. 매일 75% 확률로 배달 기사가 한 명 더 방문하지만, 그날 방문할 일반 손님 한 명을 대신합니다. | 유지 | 진열장·75%·일반 손님 슬롯 대체 조건 |
| 360 | 24 | `raw_desc_microwave` | Pre-made meal pack. Heat it in the microwave and it's done. | 预制料理包。放进微波炉加热就算做好。 | 미리 만들어 둔 밀키트입니다. 전자레인지에 데우기만 하면 완성됩니다. | 유지 | 완제품 밀키트는 전자레인지에 데우는 방식 |
| 361 | 25 | `raw_desc_brew` | Instant mix. Drag water onto it ({0} ml), then heat it in the microwave. Any water works. | 冲剂。把水拖上来（{0} 毫升），再进微波炉加热。什么水都行。 | 즉석 혼합식입니다. 물 {0}ml를 끌어다 붓고 전자레인지에 데우세요. 어떤 종류의 물이든 사용할 수 있습니다. | 유지 | 물 {0}mL 주입 뒤 전자레인지 조리 |
| 362 | 26 | `raw_desc_mix` | Concentrate. Drag water onto it ({0} ml) and it's done. Do not heat. Any water works. | 浓缩液。把水拖上来（{0} 毫升）就算做好，别加热。什么水都行。 | 농축 식품입니다. 물 {0}ml를 끌어다 부으면 완성됩니다. 가열하지 마세요. 어떤 종류의 물이든 사용할 수 있습니다. | 유지 | {0}mL 물로 희석하되 가열은 금지 |
| 363 | 27 | `raw_desc_cold` | Cold food. Right-click to unwrap and it's done. Do not heat. | 冷食。右键拆开就算做好，别加热。 | 차갑게 먹는 음식입니다. 우클릭해 포장을 벗기면 완성됩니다. 데우지 마세요. | 유지 | 차게 먹는 음식은 우클릭 개봉·가열 금지 |
| 364 | 28 | `meal_name` | {0} (cooked) | {0}（熟） | {0} (조리 완료) | 유지 | 조리 완료 상태의 {0} 메뉴명 표기 |
| 365 | 29 | `meal_desc` | Cooked but not yet packed. Keep it in the back kitchen cabinet for the couriers, or sell it honestly as pre-made food to the lower levels; they don't care. | 做好但还没打包的预制熟食。放进后厨柜等骑手来取；或者如实当预制菜卖给下层区，他们不在乎。 | 조리는 끝났지만 아직 포장하지 않은 음식입니다. 배달 기사를 위해 조리실 보관함에 두거나, 하층구역 주민에게 미리 조리된 음식이라고 솔직히 밝히고 판매할 수도 있습니다. 그들은 신경 쓰지 않으니까요. | 유지 | 하층구역에는 미리 만든 음식이라 밝혀 팔 수 있음 |
| 366 | 30 | `method_microwave` | microwave | 微波 | 전자레인지 가열 | 유지 | 전자레인지 조리 방식 이름 |
| 367 | 31 | `method_brew` | brew + microwave | 冲泡 + 微波 | 물 섞은 뒤 전자레인지 가열 | 유지 | 물과 전자레인지 두 공정이 필요한 방식 |
| 368 | 32 | `method_mix` | add water | 兑水 | 물 첨가 | 유지 | 가열 없이 물만 타는 방식 |
| 369 | 33 | `method_cold` | cold | 冷食 | 냉식 | 유지 | 차게 먹는 조리 방식 |
| 370 | 34 | `tip_fancy` | Takeout name: {0} | 外卖名：{0} | 배달 메뉴명: {0} | 유지 | {0} 메뉴의 배달용 상표명 |
| 371 | 35 | `tip_kcal` | {0} kcal · {1} | {0} 千卡 · {1} | {0}kcal · {1} | 유지 | 열량·육류 여부를 합친 {0}·{1} 표시 |
| 372 | 36 | `tip_meat` | meat | 荤 | 육류 포함 | 유지 | 고기가 들어간 메뉴 구분 |
| 373 | 37 | `tip_veg` | meatless | 素 | 육류 미포함 | 유지 | 고기가 없는 메뉴 구분 |
| 374 | 38 | `tip_hot` | Hot, ready to go | 热乎的，可以出餐 | 따뜻함, 바로 제공 가능 | 유지 | 뜨거운 상태에서 즉시 전달 가능 |
| 375 | 39 | `tip_ready` | Done, ready to go | 做好了，可以出餐 | 완성, 바로 제공 가능 | 유지 | 완성 상태에서 전달 가능 |
| 376 | 40 | `tip_cold` | Gone cold, heat it again | 凉了，要再热一次 | 식었음, 다시 데우기 필요 | 유지 | 식어 재가열이 필요한 상태 |
| 377 | 41 | `tip_wrong_heated` | Heated: this one shouldn't be | 热过了：这道不该加热 | 가열됨: 데우면 안 되는 음식 | 유지 | 가열하지 말아야 할 음식을 데운 경고 |
| 378 | 42 | `tip_water` | Water used: {0} | 兑的水：{0} | 사용한 물: {0} | 유지 | 사용한 물 종류를 {0}에 출력 |
| 379 | 43 | `tip_dream` | A little extra "seasoning" (dream dust) | 掺了点“调味料”（梦尘） | 조미료를 조금 더 넣었습니다 (꿈가루) | 유지 | 꿈가루가 첨가된 음식의 은어 |
| 380 | 44 | `mw_panel_modules` | Modules | 模组 | 모듈 | 유지 | 전자레인지 모듈 패널명 |
| 381 | 45 | `mw_panel_chamber` | Chamber | 加热格 | 가열실 | 유지 | 가열 칸의 공간 명칭 |
| 382 | 46 | `mw_panel_control` | Notes | 说明 | 설명 | 유지 | 전자레인지 조작 설명 영역 |
| 383 | 47 | `mw_tip_power` | Power per heat:  | 每次加热耗电： | 1회 가열 전력 소모:  | 유지 | 가열 1회당 전력 소비 수치 접두어 |
| 384 | 48 | `mw_tip_base` |  (base {0}) | （基础 {0}） |  (기본 {0}) | 유지 | 기본 전력량의 {0} 치환·선행 공백 보존 |
| 385 | 49 | `mw_tip_performance` | Performance (no effect on the microwave) | 性能（对微波炉无影响） | 성능 (전자레인지에는 효과 없음) | 유지 | 성능 모듈은 전자레인지에 효력 없음 |
| 386 | 50 | `mw_tip_efficiency` | Efficiency (reduces power use) | 效率（降低耗电） | 효율 (전력 소모 감소) | 유지 | 효율 모듈의 전력 절감 효과 |
| 387 | 51 | `mw_tip_quality` | Quality (reduces bad reviews) | 质量（降低差评） | 품질 (부정적 평가 감소) | 유지 | 품질 모듈의 악평 감소 효과 |
| 388 | 52 | `terminal_title` | Order Terminal · Supplies | 下单机 · 商家进货 | 주문 단말기 · 물품 구매 | 유지 | 물품을 발주하는 단말기 화면명 |
| 389 | 53 | `terminal_summary` | Shop likes {0} · cash {1} | 店铺点赞 {0} · 现金 {1} | 가게 좋아요 {0} · 보유 현금 {1} | 유지 | 좋아요·보유 현금 {0}·{1} 자리 보존 |
| 390 | 54 | `terminal_pending` | Arriving tomorrow morning: {0} | 明早送到：{0} 件 | 내일 아침 도착: {0}개 | 유지 | 익일 아침 도착할 수량 표시 |
| 391 | 55 | `terminal_tab_packs` | Meal packs | 料理包 | 밀키트 | 유지 | 밀키트 주문 탭 |
| 392 | 56 | `terminal_tab_supplies` | Packing & snacks | 打包与杂货 | 포장 용품 및 간식 | 유지 | 포장재와 간식 주문 탭 |
| 393 | 57 | `terminal_pack_tip` | Takeout name: {0} ({1}) | 外卖名：{0}（{1}） | 배달 메뉴명: {0} ({1}) | 유지 | {0} 음식명과 {1} 방식 병기 |
| 394 | 58 | `terminal_price` | {0} cr | {0} 信用点 | {0}크레딧 | 유지 | {0} 크레딧 가격 표시 |
| 395 | 59 | `terminal_owned` | Owned | 已购买 | 보유 중 | 유지 | 구매 후 보유 상태 표시 |
| 396 | 60 | `terminal_total` | Total {0} cr | 合计 {0} 信用点 | 합계 {0}크레딧 | 유지 | 주문 금액의 합산 표시 |
| 397 | 61 | `terminal_clear` | Clear | 清空 | 목록 비우기 | 유지 | 주문 목록을 비우는 동작 |
| 398 | 62 | `terminal_submit` | Order | 下单 | 주문하기 | 유지 | 발주 확정 버튼 |
| 399 | 63 | `terminal_empty` | The list is empty | 货单是空的 | 주문 목록이 비어 있습니다 | 유지 | 빈 주문 목록 경고 |
| 400 | 64 | `terminal_no_money` | Not enough cash (needs {0}) | 钱不够（要 {0}） | 현금 부족 (필요: {0}) | 유지 | 현금 부족 시 필요한 금액 {0} |
| 401 | 65 | `terminal_ordered` | Ordered for {0} credits. Arrives in the back kitchen cabinet tomorrow morning. | 已下单，花了 {0} 信用点。明早送到后厨柜。 | {0}크레딧으로 주문했습니다. 내일 아침 조리실 보관함에 도착합니다. | 유지 | 결제액 {0}·익일 아침 보관함 입고 |
| 402 | 66 | `shop_kit` | Disposable packing kit ×{0} | 一次性打包材料 ×{0} | 일회용 포장 세트 ×{0} | 유지 | 포장 세트 수량과 ×{0} |
| 403 | 67 | `shop_cola` | Bootleg mini cola ×{0} | 劣质小可乐 ×{0} | 짝퉁 미니 콜라 ×{0} | 유지 | 미니 콜라 수량과 ×{0} |
| 404 | 68 | `shop_sausage` | Bootleg starch sausage ×{0} | 劣质淀粉肠 ×{0} | 짝퉁 전분 소시지 ×{0} | 유지 | 전분 소시지 수량과 ×{0} |
| 405 | 69 | `shop_sign` | "Freshly Wok-Fried" sign (one only) | “现炒”灯牌（限购 1） | ‘갓 볶은 요리’ 간판 (1개만 구매 가능) | 유지 | 현장 볶음 간판 구매 1개 제한 |
| 406 | 70 | `shop_microwave` | Second-hand microwave | 二手微波炉 | 중고 전자레인지 | 유지 | 중고 전자레인지의 상점 표시명 |
| 407 | 71 | `courier_name` | Delivery Courier | 外卖骑手 | 배달 기사 | 유지 | 배달 기사의 직업명 |
| 408 | 72 | `courier_greet_1` | Quick, quick, they dock my pay if I'm late! | 快快快，超时扣钱的！ | 빨리요, 빨리! 늦으면 제 월급에서 깐다니까요! | 유지 | 늦으면 급여가 깎이는 다급한 말투 |
| 409 | 73 | `courier_greet_2` | Pickup, boss! I've got six more orders on the road. | 老板，取餐！我还有六单在路上。 | 사장님, 픽업 왔어요! 아직 배달할 게 여섯 건이나 남았다고요. | 유지 | 여섯 건이 남은 픽업 기사 말투 |
| 410 | 74 | `courier_greet_3` | The app says your food is ready. Is it, though? | 系统说你家出餐了，真出了吗？ | 앱에는 음식 준비가 끝났다는데…… 정말 끝난 거 맞죠? | 유지 | 앱 준비 완료와 현실을 의심하는 질문 |
| 411 | 75 | `courier_greet_sign` | Saw your "freshly wok-fried" sign. Pickup! | 看见你家“现炒”的牌子了，取餐！ | ‘갓 볶은 요리’ 간판 보고 왔어요! 음식 픽업할게요! | 유지 | 간판을 보고 방문했다는 배달 기사 대사 |
| 412 | 76 | `courier_order` | Order: {0}. {1} portion(s). | 单子：{0}。一共 {1} 份。 | 주문: {0}. 총 {1}인분. | 유지 | 음식명 {0}·주문 인분 {1} |
| 413 | 77 | `order_line` | {0} ×{1} | {0} ×{1} | {0} ×{1} | 유지 | 품목·수량 {0}×{1} 형식 유지 |
| 414 | 78 | `order_return` | Repeat order · asked for {0} by name | 回头单 · 指名 {0} | 재주문 · {0}을(를) 콕 집어 주문함 | 유지 | 지난 주문과 지정 메뉴명 {0} 명시 |
| 415 | 79 | `courier_return_1` | Somebody ordered your {0} last time. Today they asked for it again, by name. | 上次有个人订了你家的{0}，今天点名让我再来取一份。 | 지난번에 {0}을(를) 시켰던 손님이 오늘도 꼭 그걸 가져오라고 하더라고요. | 유지 | 지정 메뉴를 재주문하는 단골 주문 대사 |
| 416 | 80 | `courier_return_2` | They kept nagging, nagging, nagging... is your stuff really that good? | 一直催催催，急得不得了……你家这玩意有这么好吃吗？ | 아주 재촉을 해대던데…… 여기 음식이 그렇게 맛있어요? | 유지 | 계속 재촉당한 기사 특유의 푸념 |
| 417 | 81 | `order_addon_cola` | Old-fashioned Caramel Active-Oxygen Refreshing Elixir ×1 | 老式焦糖活氧解腻神仙水 ×1 | 전통 캐러멜 활성산소 청량수 ×1 | 유지 | 캐러멜 활성산소 음료의 과장된 상품명 |
| 418 | 82 | `order_addon_sausage` | House Special Golden Crispy Meaty Stick ×1 | 私房极炙黄金脆皮肉香棒 ×1 | 특제 황금빛 바삭 육즙 소시지 ×1 | 유지 | 황금빛 바삭 고기 막대의 과장된 상품명 |
| 419 | 83 | `barter_body` | Pickup\n{0}\n\nPlatform pays {2} credits | 取餐\n{0}\n\n平台结算 {2} 信用点 | 음식 픽업\n{0}\n\n플랫폼 정산액: {2}크레딧 | 유지 | 픽업·결제 {0}·{2}와 이스케이프 개행 유지 |
| 420 | 84 | `courier_ready` | All there, I'm off! | 齐了，我拿走了！ | 다 있네요! 가져갈게요! | 유지 | 물품이 다 준비되어 떠나는 대사 |
| 421 | 85 | `courier_accept` | Gone, gone. Don't blame me for the bad reviews! | 走了走了，差评别找我！ | 갑니다, 가요! 악평 달려도 제 탓은 하지 마세요! | 유지 | 악평 책임을 지지 않겠다는 기사 대사 |
| 422 | 86 | `nightlog_summary` | Takeout: {0} portion(s) delivered today, the platform paid {1} credits. Likes +{2} / -{3}, now {4}. | 外卖：今天送出 {0} 份，平台结算 {1} 信用点。点赞 +{2} / −{3}，现在 {4} 个赞。 | 배달 현황: 오늘 {0}인분을 전달하고 플랫폼에서 {1}크레딧을 정산받았습니다. 좋아요 +{2} / -{3}, 현재 {4}개. | 유지 | 인분·정산액·좋아요 증가·감소·총계 변수 보존 |
| 423 | 87 | `nightlog_timeouts` | {0} courier(s) left empty-handed: late orders. | 有 {0} 个骑手空手走了，记出餐超时。 | 배달 기사 {0}명이 음식을 받지 못하고 떠났습니다. 조리가 늦었습니다. | 유지 | 음식 지연 때문에 빈손으로 떠난 기사 수 |
| 424 | 88 | `nightlog_addon_missed` | {0} like add-on(s) never arrived; those customers took their like back. | 有 {0} 份点赞加购没送到，食客取消了点赞。 | 좋아요를 누른 손님에게 추가 주문품 {0}개를 전달하지 못해 좋아요를 취소당했습니다. | 유지 | 추가 주문 누락으로 손님이 좋아요를 취소함 |
| 425 | 89 | `nightlog_regulars` | {0} customer(s) want to order again tomorrow. They sound desperate. | 有 {0} 位食客明天还要再点，催得很急。 | 손님 {0}명이 내일도 재주문하겠다고 합니다. 꽤 급한 모양입니다. | 유지 | 내일 재주문할 손님 수·재촉 표현 |
| 426 | 90 | `nightlog_withdrawal` | {0} regular(s) didn't get that taste this time. The reviews were brutal, and they won't be back. | {0} 位回头客这回没尝到那个味，差评骂得很难听，以后不来了。 | 단골손님 {0}명이 이번엔 원하던 맛을 보지 못했습니다. 혹평을 남기고 다시는 오지 않겠다고 했습니다. | 유지 | 원하던 맛을 못 본 단골의 혹평·이탈 |
| 427 | 91 | `nightlog_exposed` | {0} order(s) tasted wrong to the customers and were reported to Security: pre-made food passed off as fresh, with something mixed in. | 有 {0} 份外卖被食客吃出了问题，举报到了治安部：预制菜冒充现做，还掺了东西。 | 손님이 음식 맛에 이상을 느껴 주문 {0}건을 치안부에 신고했습니다. 미리 만든 음식에 무언가를 섞어 놓고 갓 조리했다고 속였다는 내용입니다. | 유지 | 신선식품 사칭·혼입물 의혹으로 치안부 신고 |
| 428 | 92 | `nightlog_delivered` | {0} orders arrived. Packing supplies and add-ons were added to the station stock. | 下单机送到 {0} 件货，材料和加购已补入打包台。 | 주문 물품 {0}개가 도착했습니다. 포장 재료와 추가 판매품을 작업대 재고에 넣었습니다. | 유지 | 발주품 도착·재료 및 추가 판매품 재고 보충 |
| 429 | 93 | `nightlog_dealer_paid` | The dealer came by: {0} poppy husks in the trash can, {1}¢ taken. | 毒贩来过：{0} 个罂粟壳在垃圾桶里，取走了 {1}¢。 | 마약상 방문: 쓰레기통에 양귀비 껍질 {0}개를 놓고 {1}크레딧을 가져갔습니다. | 유지 | 마약상 방문·양귀비 껍질 수·금액 |
| 430 | 94 | `nightlog_dealer_broke` | The dealer came by, but you didn't have {0}¢. He left with nothing. | 毒贩来过，你的账上不够 {0}¢，他空手走了。 | 마약상이 찾아왔지만 {0}크레딧이 없어 빈손으로 돌아갔습니다. | 유지 | 돈이 부족해 물건을 못 놓고 간 마약상 |
| 431 | 95 | `dish_fried_rice_fancy` | Deep Space Wok-Hei Golden Fried Rice | 深空猛火锅气黄金炒饭 | 심우주 강불 직화 황금 볶음밥 | 유지 | 심우주 직화 황금 볶음밥 광고명 |
| 432 | 96 | `dish_fried_rice_truth` | Freeze-thawed Soy Protein Crumb Microwave Rice Pack | 冻融大豆蛋白碎微波米饭包 | 냉동·해동한 대두단백 부스러기를 넣은 전자레인지 밥 팩 | **수정** | 대두단백 부스러기를 냉동·해동했다는 공정 복구 |
| 433 | 97 | `dish_braised_chicken_fancy` | Old Street Firewood Braised Chicken | 老街古法柴火黄焖鸡 | 옛 골목 장작불 황먼지(중국식 닭고기 조림) | 유지 | 중국식 황먼지의 음식 종류·양념 특징 보존 |
| 434 | 98 | `dish_braised_chicken_truth` | Chicken Skin and Bone Shard Frozen Grease Pack | 鸡皮与碎骨冷冻重油料理包 | 닭 껍질과 부서진 뼈로 만든 냉동 기름 범벅 밀키트 | 유지 | 닭 껍질·뼛조각·기름진 냉동 팩의 반전 |
| 435 | 99 | `dish_pepper_beef_fancy` | Volcano Lava Marbled Black Pepper Beef | 火山熔岩雪花黑椒牛柳 | 화산 용암 마블링 흑후추 소고기 | 유지 | 마블링 흑후추 소고기의 과장된 광고명 |
| 436 | 100 | `dish_pepper_beef_truth` | Water-injected Tenderizer-glued Restructured Meat Slices | 注水嫩肉粉重组胶水肉片 | 물을 주입하고 연육제·접착제로 뭉친 재구성 고기 조각 | **수정** | 물 주입·연육제·접착제로 재구성한 고기 공정 명확화 |
| 437 | 101 | `dish_basa_fish_fancy` | Aged-jar Fresh-boiled Sour Soup Eco Basa Fish | 老坛现熬酸汤生态巴沙鱼 | 묵힌 항아리의 새콤한 국물로 갓 끓인 바사 생선 | **수정** | 생선이 신선하다는 뜻 아닌 갓 끓였다는 뜻으로 교정 |
| 438 | 102 | `dish_basa_fish_truth` | Phosphate-soaked Swollen Mixed Fish Fillet | 磷酸盐泡发膨胀杂鱼柳 | 인산염에 불려 부풀린 잡어살 | 유지 | 인산염에 불려 부풀린 잡어살의 실제 재료 |
| 439 | 103 | `dish_braised_pork_rice_fancy` | Grandma's Secret Handmade Braised Pork Rice | 外婆秘制手作酱汁卤肉饭 | 할머니 비법 수제 소스 돼지고기 덮밥 | 유지 | 할머니 수제 양념 돼지고기 덮밥 광고명 |
| 440 | 104 | `dish_braised_pork_rice_truth` | Soy Fiber Protein with Shelf-stable Fat Pack | 大豆拉丝蛋白拼凑常温油脂包 | 대두 섬유 단백질과 상온 보관 지방을 섞은 식품 팩 | 유지 | 대두 섬유 단백질과 상온 지방의 조합 |
| 441 | 105 | `dish_kungpao_chicken_fancy` | Imperial Kitchen Fresh-tossed Kung Pao Chicken | 宫廷御膳现爆宫保鸡丁 | 궁중식 갓 볶은 궁바오지딩 | 유지 | 궁중식 쓰촨 궁바오지딩 광고명 |
| 442 | 106 | `dish_kungpao_chicken_truth` | Mushy Meat Scraps and Damp Peanut Sludge Pack | 软烂肉渣与受潮花生黏糊包 | 흐물흐물한 고기 부스러기와 눅눅한 땅콩 찌꺼기 팩 | 유지 | 무른 고기 부스러기와 눅눅한 땅콩 풍자 |
| 443 | 107 | `dish_pork_chop_fancy` | Charcoal Slow-roasted Marbled Thick Pork Chop | 炭火慢烤雪花厚切大排 | 숯불에 천천히 구운 두툼한 마블링 돼지고기 스테이크 | 유지 | 숯불 저온 로스팅 돼지고기 메뉴명 |
| 444 | 108 | `dish_pork_chop_truth` | Pressed Offcut Meat and Starch Patty | 边角料碎肉压制淀粉合成肉饼 | 자투리 고기를 압착한 전분 혼합 패티 | 유지 | 자투리 고기·전분 압착 패티의 실체 |
| 445 | 109 | `dish_angus_beef_fancy` | Italian Slow-stewed Fresh Tomato Angus Beef | 意式慢炖鲜茄安格斯牛肉 | 이탈리아식 생토마토 앵거스 소고기 스튜 | 유지 | 이탈리아식 토마토 앵거스 소고기 광고명 |
| 446 | 110 | `dish_angus_beef_truth` | Industrial Tomato Paste with Beef Offal Bits | 工业番茄膏调和牛杂碎料包 | 산업용 토마토 페이스트와 소 내장 부스러기 팩 | 유지 | 산업용 토마토 페이스트·소 내장 부스러기 |
| 447 | 111 | `dish_mapo_tofu_fancy` | Sichuan Hand-chopped Authentic Mapo Tofu | 川味手剁地道麻婆豆腐 | 손수 다져 만든 정통 쓰촨식 마파두부 | **수정** | 손수 잘게 다졌다는 원문 조리법 의미 복구 |
| 448 | 112 | `dish_mapo_tofu_truth` | Dextrin-thickened Chili Oil Okara Soft Can | 淀粉糊精勾芡红油豆渣软罐头 | 덱스트린으로 걸쭉하게 만든 고추기름 비지 통조림 | 유지 | 덱스트린·고추기름·비지의 저질 재료 |
| 449 | 113 | `dish_mini_pizza_fancy` | Fresh-baked Stretchy Loaded Lava Mini Pizza | 现烤拉丝多肉爆浆小披萨 | 갓 구워 치즈가 쭉 늘어나는 육즙 가득 미니 피자 | 유지 | 치즈가 늘어나는 미니 피자 광고명 |
| 450 | 114 | `dish_mini_pizza_truth` | Rubbery Synthetic Cheese Frozen Dough Disc | 橡胶感仿生人造干酪冷冻生胚 | 고무 같은 인조 치즈를 얹은 냉동 피자 반죽 | 유지 | 고무 같은 인조 치즈·냉동 반죽 실체 |
| 451 | 115 | `dish_crispy_sausage_fancy` | Ultimate Organic Golden Crispy Sausage | 绝味原生态黄金脆皮肠 | 최상급 천연 황금빛 바삭 소시지 | 유지 | 황금빛 바삭 소시지 광고명 |
| 452 | 116 | `dish_crispy_sausage_truth` | 0% Meat Bone Paste Corn Starch Sausage | 0含肉量骨泥玉米淀粉肠 | 고기 함량 0%, 뼈 반죽과 옥수수전분으로 만든 소시지 | 유지 | 고기 함량 0%·뼈 반죽·전분 대비 |
| 453 | 117 | `dish_duck_soup_fancy` | Fresh-boiled Old Duck Pickled Radish Soup | 现熬老鸭酸萝卜开胃汤 | 갓 끓인 늙은 오리와 새콤한 무 수프 | 유지 | 오리와 절인 무로 끓이는 국물 메뉴명 |
| 454 | 118 | `dish_duck_soup_truth` | Duck-flavor MSG Seasoning Powder Instant Mix | 鸭肉味精与复配调味粉热水冲剂 | 오리 맛 조미료 가루와 복합 조미분을 섞은 즉석 수프 | 유지 | 오리 맛 조미료와 복합 조미분으로 만든 즉석 수프 |
| 455 | 119 | `dish_chicken_soup_fancy` | Wild Mushroom Fresh-simmered Free-range Chicken Soup | 秘境野生菌菇现熬土鸡汤 | 야생 버섯을 넣고 정성껏 끓인 토종닭 수프 | 유지 | 야생 버섯·토종닭의 신선한 수프 광고명 |
| 456 | 120 | `dish_chicken_soup_truth` | Industrial Mushroom Powder and MSG Hot Water | 工业食用菌粉与味精热水混合液 | 공장제 버섯 분말과 MSG를 탄 뜨거운 물 | 유지 | 버섯 가루·MSG·뜨거운 물 혼합물 |
| 457 | 121 | `dish_maw_soup_fancy` | Nourishing Clay-pot Fish Maw Chicken Broth | 滋补现炖原盅花胶土鸡羹 | 뚝배기에 갓 끓인 보양 생선 부레 닭고기탕 | 유지 | 생선 부레·닭고기 보양탕 메뉴명 |
| 458 | 122 | `dish_maw_soup_truth` | Gelatin and Xanthan Gum Thickened Flavored Soup | 食用明胶与黄原胶增稠调味浓汤 | 젤라틴과 잔탄검으로 걸쭉하게 만든 향 첨가 수프 | 유지 | 젤라틴·잔탄검을 넣은 조미 수프의 실체 |
| 459 | 123 | `dish_coconut_milk_fancy` | Dawn Handmade Silky Rich Coconut Milk | 晨曦手作丝滑浓醇生椰乳 | 아침에 직접 만든 진하고 부드러운 코코넛 밀크 | 유지 | 아침 수제 코코넛 밀크 광고명 |
| 460 | 124 | `dish_coconut_milk_truth` | Creamer and Hydrogenated Oil Instant Mix | 植脂末与氢化植物油勾兑冲剂 | 식물성 크리머와 경화 식물성 기름을 섞은 즉석 음료 분말 | **수정** | 식물성 크리머의 의미를 특정 커피용 프리머와 분리 |
| 461 | 125 | `dish_pearl_milk_fancy` | Handmade Brown Sugar Boba Fresh Milk | 现熬手作黑糖珍珠鲜牛乳 | 갓 만든 흑당 타피오카 펄 우유 | 유지 | 흑당 타피오카 펄 신선 우유의 광고명 |
| 462 | 126 | `dish_pearl_milk_truth` | Industrial Gum Fake Pearls in Creamer Water | 工业食用胶人造圆子配奶精水 | 공업용 식용 검으로 만든 가짜 펄과 프리머 물 | 유지 | 식용 검으로 가짜 펄을 만든 음료 |
| 463 | 127 | `dish_lemon_ice_fancy` | Chef's Hand-smashed Lemon Ice Drink | 主厨手作暴打柠香冰饮 | 주방장이 직접 으깬 상큼한 레몬 얼음 음료 | 유지 | 주방장이 직접 으깬 레몬 음료 광고명 |
| 464 | 128 | `dish_lemon_ice_truth` | Citric Acid Flavoring and Tap Water over Ice | 柠檬酸香精与自来水加满冰块 | 구연산 향료와 수돗물에 얼음을 잔뜩 넣은 음료 | 유지 | 구연산 향료·수돗물·얼음으로 만든 저가 음료 |
| 465 | 129 | `dish_plum_drink_fancy` | Traditional Slow-brewed Licorice Plum Drink | 传统现熬古法甘草酸梅汤 | 전통 방식으로 오래 달인 감초 매실차 | 유지 | 전통 감초 매실차 광고명 |
| 466 | 130 | `dish_plum_drink_truth` | Caramel Color and Corn Syrup Concentrate | 焦糖色素与果葡糖浆浓缩勾兑液 | 캐러멜 색소와 액상과당 농축액을 섞은 음료 | 유지 | 캐러멜 색소와 액상과당으로 만든 농축 음료 |
| 467 | 131 | `dish_orange_juice_fancy` | Fresh-squeezed NFC Sunshine Orange Juice | 鲜榨NFC阳光甜橙原汁 | 갓 짜낸 NFC 햇살 오렌지 주스 | 유지 | NFC 과즙이라는 광고 요소 유지 |
| 468 | 132 | `dish_orange_juice_truth` | Orange Flavoring and Thickener Fruity Water | 橙味香料与增稠剂调配果味水 | 오렌지 향료와 증점제로 만든 과일맛 물 | 유지 | 오렌지 향료와 증점제로 만든 가짜 주스 |
| 469 | 133 | `dish_tuna_onigiri_fancy` | Polar Deep-sea Tuna Energy Rice Ball | 极地深海金枪鱼元气饭团 | 극지 심해 참치 활력 주먹밥 | **수정** | 원문의 Energy를 영양이 아닌 활력으로 맞춤 |
| 470 | 134 | `dish_tuna_onigiri_truth` | Mystery Fish Paste in a Lethal Dose of Cheap Mayo | 杂鱼泥拌致死量廉价沙拉酱 | 정체불명 잡어살 반죽에 치사량 수준의 싸구려 마요네즈를 버무린 주먹밥 | **수정** | 값싼 마요네즈의 치사량 과장 표현 복구 |
| 471 | 135 | `item_board_name` | Online Menu Board | 线上菜单板 | 온라인 메뉴판 | 유지 | 배달 플랫폼 메뉴판 아이템명 |
| 472 | 136 | `item_board_desc` | The platform storefront menu. Right-click: tick the dishes on sale, and star one (Pick) to guarantee an order the next morning. More dishes listed, more couriers. Changes apply next morning. | 平台店铺的菜单。右键打开：勾选上架的菜，打一颗星（店长推荐）就保证明早有一单点它。上架越多，平台推流越好，来的骑手越多。改动明早生效。 | 배달 플랫폼에 표시되는 가게 메뉴입니다. 우클릭해서 판매할 요리를 선택하고 하나에 별표(추천)를 지정하세요. 추천한 요리는 다음 날 아침 주문이 반드시 들어옵니다. 등록한 메뉴가 많을수록 배달 기사도 많이 찾아옵니다. 변경 사항은 다음 날 아침 적용됩니다. | 유지 | 판매 등록·별표 추천·다음 날 주문 보장 설명 |
| 473 | 137 | `item_mw_note_name` | Note | 说明纸条 | 안내문 | 유지 | 전자레인지 설명 쪽지 제목 |
| 474 | 138 | `item_mw_note_desc` | Put a meal pack in the chamber and it heats itself: lit for one second, then a ding. Drinks and mixes need water first; rice balls and iced drinks aren't heated. Each heat uses power: efficiency modules save power, quality modules cut bad reviews, performance does nothing. | 料理包放进加热格，自动加热：亮一秒，叮一声就热好。加水的先加水；冰饮、饭团不用热。每次加热耗电：效率模组省电，质量模组少差评，性能没用。 | 밀키트를 가열실에 넣으면 자동으로 데워집니다. 1초 동안 불이 들어왔다가 '띵' 소리와 함께 완성됩니다. 음료와 혼합식에는 먼저 물을 넣어야 하며, 주먹밥과 얼음 음료는 데우지 않습니다. 가열할 때마다 전력이 소모됩니다. 효율 모듈은 전력을 절약하고 품질 모듈은 악평을 줄이지만, 성능 모듈은 효과가 없습니다. | 유지 | 가열 1초·물 첨가·기기 모듈 역할 명시 |
| 475 | 139 | `board_title` | Online Menu Board | 线上菜单板 | 온라인 메뉴판 | 유지 | 온라인 메뉴판 UI 제목 |
| 476 | 140 | `board_summary` | Listed {0} / {1} (+{2} courier(s)) | 上架 {0} / {1} 样（推流 +{2} 位骑手） | 등록 메뉴 {0}/{1}개 (배달 기사 +{2}명) | 유지 | 메뉴 등록 개수와 배달 기사 증가량 |
| 477 | 141 | `board_page` | Page {0} / {1} | 第 {0} / {1} 页 | {0}/{1}페이지 | 유지 | 현재 페이지 {0}·총 {1} 표시 |
| 478 | 142 | `board_list` | List | 上架 | 판매 등록 | 유지 | 메뉴를 판매 목록에 올리는 동작 |
| 479 | 143 | `board_unlist` | Unlist | 下架 | 판매 해제 | 유지 | 메뉴를 판매 목록에서 내리는 동작 |
| 480 | 144 | `board_star_on` | ★ | ★ | ★ | 유지 | 활성 추천 표시 별 ★ |
| 481 | 145 | `board_star_off` | ☆ | ☆ | ☆ | 유지 | 비활성 추천 표시 별 ☆ |
| 482 | 146 | `board_price` | {0}¢ | {0}¢ | {0}크레딧 | 유지 | 가격의 {0} 크레딧 출력 |
| 483 | 147 | `board_star_need_list` | List it first before picking it. | 先上架，才能推荐。 | 먼저 판매 등록해야 추천할 수 있습니다. | 유지 | 등록한 메뉴만 추천 가능하다는 제한 |
| 484 | 148 | `board_stock` | Stock {0} | 存 {0} | 재고 {0} | 유지 | 재고 수량 {0} |
| 485 | 149 | `board_auto_label` | Auto-list: ≥ {0} | 按库存上架：≥ {0} 份 | 자동 등록 기준: {0}개 이상 | 유지 | 자동 등록에 필요한 재고 최솟값 |
| 486 | 150 | `board_auto_apply` | List by stock | 一键上架 | 재고 기준으로 등록 | 유지 | 보유 재고 기반 메뉴 등록 명령 |
| 487 | 151 | `board_auto_tip` | Lists dishes with at least this much stock and unlists the rest. Stock counts packs, cooked meals, packed bags and terminal orders on the way. Takes effect tomorrow. | 库存够这么多份的菜上架，不够的下架。库存算料理包、熟食、外卖袋，加上下单机已订未到的货。明早生效。 | 재고가 지정 수량 이상인 메뉴는 등록하고 부족한 메뉴는 내립니다. 재고에는 밀키트, 조리된 음식, 포장된 배달 음식, 배송 중인 주문 물품이 모두 포함됩니다. 다음 날부터 적용됩니다. | 유지 | 배송 중 발주품도 재고에 포함·익일 적용 |
| 488 | 152 | `board_auto_done` | Listed {0} dishes by stock ({1} added, {2} removed). Takes effect tomorrow. | 按库存上架了 {0} 样（新上 {1}、下架 {2}），明早生效。 | 재고 기준으로 메뉴 {0}개를 등록했습니다 (신규 등록 {1}개, 판매 해제 {2}개). 내일부터 적용됩니다. | 유지 | 신규 등록·해제 건수 및 익일 적용 |
| 489 | 153 | `board_auto_none` | No dish has {0} in stock, so the menu was left as is. Restock at the terminal or lower the threshold. | 没有一样菜的库存够 {0} 份，菜单没动。先去下单机补货，或者把门槛调低。 | 재고가 {0}개 이상인 메뉴가 없어 기존 메뉴를 유지했습니다. 주문 단말기에서 재고를 보충하거나 기준 수량을 낮추세요. | 유지 | 조건에 맞는 재고가 없으면 메뉴 변경 안 함 |
| 490 | 154 | `courier_need_bags` | Put the packed dishes for this order on the counter. | 把订单对应的成品外卖放到柜台。 | 주문에 맞는 포장 음식을 카운터에 올려 주세요. | **수정** | 주문 음식을 올릴 장소가 카운터인 점 복구 |
| 491 | 155 | `item_packing_name` | Packing Station | 打包台 | 포장 작업대 | 유지 | 음식 포장 설비의 고유 아이템명 |
| 492 | 156 | `item_packing_desc` | Drag cooked food onto the station. One packing kit becomes a takeout bag in the cabinet. Double-click to view kits, colas and sausages. Order add-ons are deducted automatically at pickup. | 把做好的菜拖到台上，消耗 1 份打包材料生成外卖，送回后厨柜。双击查看材料、小可乐和淀粉肠库存，订单加购在交餐时自动扣除。 | 조리된 음식을 작업대에 끌어다 놓으면 포장 세트 1개를 소비하고 배달 음식을 만든 뒤 조리실 보관함에 넣습니다. 더블클릭하면 포장 세트, 콜라, 소시지 재고를 확인할 수 있습니다. 추가 주문품은 전달할 때 자동으로 차감됩니다. | 유지 | 조리식 투입·포장재 소모·보관함 이동 |
| 493 | 157 | `packing_intake` | Pack | 打包 | 포장하기 | 유지 | 포장 작업 실행 버튼 |
| 494 | 158 | `packing_stock` | Stock | 库存 | 재고 | 유지 | 포장 작업대의 재고 항목 |
| 495 | 159 | `packing_kits` | Packing kits ×{0} | 打包材料 ×{0} | 포장 세트 ×{0} | 유지 | 포장 세트의 보유 수량 |
| 496 | 160 | `packing_colas` | Mini colas ×{0} | 小可乐 ×{0} | 미니 콜라 ×{0} | 유지 | 소형 콜라의 보유 수량 |
| 497 | 161 | `packing_sausages` | Sausages ×{0} | 淀粉肠 ×{0} | 소시지 ×{0} | 유지 | 전분 소시지의 보유 수량 |
| 498 | 162 | `promotion_title` | Doubao-generated Promotion | 豆包生成宣传图 | 더우바오 AI 생성 홍보물 | 유지 | 더우바오 인공지능 홍보물 명칭 |
| 499 | 163 | `promotion_desc` | Generate a wok-fried rice poster for the takeout shop. Attract {0} more courier(s) each day, within the daily platform limit, starting next business day. The poster appears at the top of the online menu board. | 为外卖店生成一张猛火锅气蛋炒饭宣传图。每天多吸引 {0} 位骑手（受每日平台骑手上限限制），下次营业生效。宣传图显示在线上菜单板顶部。 | 배달 가게에 쓸 강불 직화 볶음밥 홍보 포스터를 생성합니다. 다음 영업일부터 하루에 배달 기사 {0}명을 추가로 유치합니다 (플랫폼의 일일 방문 한도 이내). 포스터는 온라인 메뉴판 상단에 표시됩니다. | 유지 | 추가 기사 {0}·영업일·플랫폼 상한 조건 |
| 500 | 164 | `promotion_view` | Double-click to view the poster | 双击查看宣传图 | 더블클릭해서 포스터 보기 | 유지 | 홍보 포스터를 여는 더블클릭 동작 |
| 501 | 165 | `order_addon_promotion` | <s><b>8¢</b></s>  <color=#D12D2D><b><size=110%>[Like & Save Special: 1¢]</size></b></color> | <s><b>8¢</b></s>  <color=#D12D2D><b><size=110%>【点赞收藏特价：1¢】</size></b></color> | <s><b>8¢</b></s>  <color=#D12D2D><b><size=110%>[좋아요·찜 특가: 1¢]</size></b></color> | 유지 | 8¢ 취소선·좋아요 특가 1¢ 서식 태그 보존 |
| 502 | 166 | `item_poppy_name` | Poppy Husk | 罂粟壳 | 양귀비 껍질 | 유지 | 양귀비 열매 껍질 아이템명 |
| 503 | 167 | `item_dealer_card_name` | Dealer's Card | 毒贩名片 | 마약상 명함 | 유지 | 마약상의 연락처가 적힌 명함 |
| 504 | 168 | `item_dealer_card_desc` | A number on the back. Open it and click the number to dial. | 背面印着一串号码。点开看看，点号码就拨过去。 | 뒷면에 전화번호가 적혀 있습니다. 명함을 열고 번호를 누르면 전화를 겁니다. | 유지 | 명함을 열어 번호를 선택해 통화함 |
| 505 | 169 | `item_dealer_card_flavor` | Don't tell anyone who sent you. | 别让别人知道是谁介绍的。 | 누가 소개해 줬는지는 입 밖에 내지 마. | 유지 | 소개자의 정체를 숨기라는 경고 어조 |
| 506 | 170 | `tip_husk` | Lightly seasoned. | 掺了点“调味料” | '조미료'를 조금 넣었습니다. | 유지 | 꿈가루 혼입을 암시하는 은어 |
| 507 | 171 | `dealer_name` | Dealer | 毒贩 | 마약상 | 유지 | 마약상을 지칭하는 인물명 |
| 508 | 172 | `dealer_intro1` | Are you Wilde's people? | 王尔德的人？ | 와일드 쪽 사람이야? | 유지 | 와일드와 연결된 사람인지 묻는 인사 |
| 509 | 173 | `dealer_intro2` | Tomorrow morning, the stock goes in your trash can. | 明天早上，货会放到你的垃圾桶。 | 내일 아침 쓰레기통에 물건을 넣어 두지. | 유지 | 익일 아침 쓰레기통 배송 방법 |
| 510 | 174 | `dealer_regular1` | Back again. How much this time? | 又是你。这回要多少？ | 또 왔군. 이번엔 얼마나 필요해? | 유지 | 재방문 고객에게 수량을 묻는 대사 |
| 511 | 175 | `dealer_regular2` | Sold by the batch. Pick your size. | 论批卖，掂量着来。 | 묶음 단위로 판다. 수량을 골라. | 유지 | 묶음 단위 판매와 수량 선택 |
| 512 | 176 | `dealer_menu` | Which batch? | 要哪一档？ | 어느 묶음으로 할 거지? | 유지 | 구매할 묶음을 묻는 질문 |
| 513 | 177 | `dealer_buy` | Order {0} husks ({1} credits) | 订 {0} 个罂粟壳（{1} 信用点） | 양귀비 껍질 {0}개 주문 ({1}크레딧) | 유지 | {0}개·{1}크레딧 구매 선택지 |
| 514 | 178 | `dealer_quit` | Never mind, not today. | 算了，先不要。 | 됐어, 오늘은 그만둘게. | 유지 | 주문하지 않고 전화 끊기 |
| 515 | 179 | `dealer_bye` | Fine. Call back when you've made up your mind. | 行，想好了再打。 | 좋아. 마음이 정해지면 다시 연락해. | 유지 | 마음이 바뀌면 다시 전화하라는 말 |
| 516 | 180 | `dealer_ready` | Got it. | 收到。 | 알았다. | 유지 | 주문을 접수한 짧은 응답 |
| 517 | 181 | `item_dealer_note_name` | Note | 小纸条 | 쪽지 | 유지 | 마약상이 남긴 쪽지의 명칭 |
| 518 | 182 | `item_dealer_note_desc` | I took the money. | 钱我取走了。 | 돈은 가져갔다. | 유지 | 돈을 수거했다는 마약상의 쪽지 |
| 519 | 183 | `upgrade_dealer_title` | Dealer Hotline | 毒贩电话 | 마약상 연락처 | 유지 | 마약상 연락처 해금 제목 |
| 520 | 184 | `upgrade_dealer_desc` | Needs the platform first. 1000 credits for a Dealer's Card. Tap the card to dial. | 要先入驻外卖平台。花 1000 信用点，送一张毒贩名片；名片一用就拨号。 | 먼저 배달 플랫폼에 입점해야 합니다. 1,000크레딧을 내면 마약상 명함을 받습니다. 명함을 누르면 전화를 걸 수 있습니다. | 유지 | 플랫폼 선행·1,000크레딧·명함 사용 조건 |
| 521 | 185 | `dealer_cooldown` | Stock hasn't arrived yet. Call back in {0} days. | 东西还没到，{0} 天后再打。 | 아직 물건이 도착하지 않았다. {0}일 뒤에 다시 전화해. | 유지 | 발주 대기기간 {0}일 안내 |
| 522 | 186 | `item_poppy_seed_name` | Poppy Seed | 罂粟籽 | 양귀비 씨앗 | 유지 | 양귀비 씨앗의 물품명 |
| 523 | 187 | `upgrade_more_title` | More Set Meals | 更多的套餐 | 추가 세트 메뉴 | 유지 | 추가 세트 메뉴 해금 제목 |
| 524 | 188 | `upgrade_more_desc` | Needs the platform first. Adds fried chicken, skewers, pork knuckle rice and roast pork rice, with an air fryer, a torch and a light-meal station, and opens uptown light-meal orders. Comes with a manual. | 要先入驻外卖平台。上新炸鸡、肉串、猪脚饭、烤肉饭，送空气炸锅、喷枪和轻食操作台，开通上城轻食单。附一本说明书。 | 먼저 배달 플랫폼에 입점해야 합니다. 치킨, 꼬치, 족발 덮밥, 구운 돼지고기 덮밥을 추가하고 에어프라이어, 토치, 가벼운 식사 작업대를 지급합니다. 상층구역용 가벼운 식사 주문도 열립니다. 안내서가 포함됩니다. | **수정** | light meal은 저칼로리 단정 아닌 가벼운 식사로 통일 |
| 525 | 189 | `item_manual_more_name` | More Set Meals | 更多的套餐 | 추가 세트 메뉴 | 유지 | 새 세트 메뉴 안내서 제목 |
| 526 | 190 | `item_manual_more_desc` | New dishes, the air fryer, the torch, uptown light meals and heartburn, on two pages. | 新菜、空气炸锅、喷枪、上城轻食和烧心值，两页写完。 | 새로운 요리, 에어프라이어, 토치, 상층구역용 가벼운 식사와 속쓰림 지수를 두 페이지로 설명합니다. | 유지 | 신규 메뉴·기기·속쓰림 지수 안내 범위 |
| 527 | 191 | `dish_fried_chicken_fancy` | American Finger-Lickin' Juicy Crunch Chicken | 美式吮指多汁大脆鸡 | 미국식 손가락까지 핥게 되는 바삭하고 촉촉한 치킨 | 유지 | 미국식 바삭한 치킨의 과장 메뉴명 |
| 528 | 192 | `dish_fried_chicken_truth` | Frozen Starch-Battered Chicken Nugget Pack | 冷冻淀粉裹粉鸡块包 | 전분 튀김옷을 입힌 냉동 치킨너겟 팩 | **수정** | 전분 튀김옷을 입힌 냉동 치킨너겟임을 명시 |
| 529 | 193 | `dish_skewer_fancy` | Heritage Fruitwood Charcoal Mega Skewers | 老字号炭火果木大肉串 | 전통 과일나무 숯불 왕꼬치 | 유지 | 과일나무 숯불에 구운 고기 꼬치 광고명 |
| 530 | 194 | `dish_skewer_truth` | Frozen Half-Done Restructured Meat Skewers | 冷冻半成品重组肉串 | 재구성 고기로 만든 냉동 반조리 꼬치 | 유지 | 반조리 재구성 육류의 냉동 꼬치 |
| 531 | 195 | `dish_pork_knuckle_fancy` | Longjiang Overlord Braised Pork Knuckle Rice | 隆江霸王老卤爆油猪脚饭 | 룽장식 진한 양념 족발 덮밥 | 유지 | 룽장식 족발 덮밥의 지역명 유지 |
| 532 | 196 | `dish_pork_knuckle_truth` | Greasy Frozen Pork Knuckle Gravy Pack | 重油冷冻猪脚卤汁料理包 | 기름진 냉동 족발 소스 팩 | 유지 | 기름진 냉동 족발 소스의 실체 |
| 533 | 197 | `dish_roast_pork_rice_fancy` | Flame-Seared Juicy Marbled Crispy Pork Rice | 炙烤暴汁雪花脆皮烤肉饭 | 불에 그을려 육즙이 가득한 마블링 바삭 돼지고기 덮밥 | 유지 | 직화·마블링·바삭함 강조한 광고명 |
| 534 | 198 | `dish_roast_pork_rice_truth` | Restructured Frozen Pork Belly Teriyaki Pack | 重组冷冻五花肉切片照烧包 | 재구성 냉동 삼겹살 데리야키 팩 | 유지 | 냉동 재구성 삼겹살 데리야키 팩 |
| 535 | 199 | `dish_fried_rice_upper` | Cold-Pressed Zero-Oil Organic Protein Grain Bowl | 冷压脱脂·零油感有机蛋白谷物碗 | 저온 압착 무기름 유기농 단백질 곡물볼 | 유지 | 저온 압착·무기름 유기농 곡물볼 광고 |
| 536 | 200 | `dish_fried_rice_pitch` | Low-temperature cold pressing locks in the grain. Zero oil, full plant protein. | 低温冷压工艺锁住谷物本味，零油感，植物蛋白满格。 | 저온 압착 공법으로 곡물 본연의 맛을 살렸습니다. 기름 없이 식물성 단백질은 듬뿍! | 유지 | 제로 오일·식물성 단백질 강조 광고문 |
| 537 | 201 | `dish_braised_chicken_upper` | Slow-Extracted Clear Broth Free-Range Collagen Cup | 慢萃清汤·低脂走地鸡胶原轻食盅 | 천천히 우려 낸 맑은 국물의 방목닭 콜라겐 컵 | 유지 | 방목닭·맑은 국물·콜라겐 메뉴명 |
| 538 | 202 | `dish_braised_chicken_pitch` | Slow-extracted the old way, every drop of fat skimmed. Only the free-range sweetness and collagen remain. | 古法慢萃撇净浮油，只留走地鸡的清甜与胶原。 | 전통 방식으로 천천히 우려 내고 기름을 말끔히 걷었습니다. 방목닭의 담백한 맛과 콜라겐만 남겼습니다. | 유지 | 지방 제거·천천히 우려내기 광고문 |
| 539 | 203 | `dish_braised_pork_rice_upper` | Gravy-Free Plant-Based Mince Energy Set | 零卤汁·植物基肉燥能量定食 | 소스 없이 즐기는 식물성 고기 에너지 정식 | 유지 | 식물성 다짐육 저탄소 정식 메뉴명 |
| 540 | 204 | `dish_braised_pork_rice_pitch` | No gravy, no guilt. Plant-based mince, a carbon-neutral energy meal. | 去卤汁、减负担，植物基肉燥，碳中和能量餐。 | 기름진 소스는 빼고 부담도 덜었습니다. 식물성 다짐육으로 만든 탄소 중립 에너지 식사! | 유지 | 소스 제거·탄소중립 마케팅 문구 |
| 541 | 205 | `dish_pork_chop_upper` | Sous-Vide Trimmed Tenderloin Protein Plate | 低温舒肥·精修里脊高蛋白餐 | 수비드로 익힌 저지방 안심 고단백 플레이트 | 유지 | 62°C 수비드 안심 고단백 식사명 |
| 542 | 206 | `dish_pork_chop_pitch` | Sous-vide at 62°C for 4 hours, trimmed lean. The protein answer for gym people. | 62°C 舒肥 4 小时，精修去脂，健身人士的蛋白质答案。 | 62°C에서 4시간 동안 수비드로 익힌 뒤 지방을 정리했습니다. 운동하는 사람을 위한 단백질 식단! | 유지 | 62°C·4시간·지방 제거 정보 모두 유지 |
| 543 | 207 | `dish_mapo_tofu_upper` | Chili-Oil-Free Steamed Silken Tofu Slim Bowl | 去红油·清蒸嫩豆腐纤体碗 | 고추기름을 뺀 순두부 찜 다이어트볼 | 유지 | 고추기름 없는 순두부 다이어트 메뉴명 |
| 544 | 208 | `dish_mapo_tofu_pitch` | No chili oil, all bean fragrance. Sichuan flavor you can eat on a diet. | 去红油、留豆香，纤体期也能放心吃的川味。 | 고추기름은 빼고 콩의 향은 살렸습니다. 다이어트 중에도 마음 놓고 즐기는 쓰촨의 맛. | 유지 | 콩 향과 쓰촨풍의 맛을 강조한 문구 |
| 545 | 209 | `dish_fried_chicken_upper` | Polar Mist Oil-Free High-Fiber Air-Crisped Chicken Strips | 极地冷雾·无油高纤空气炸原汁鸡柳 | 극지 냉무 공법으로 만든 무기름 고섬유질 에어프라이어 치킨 스트립 | **수정** | High-Fiber를 고단백으로 잘못 옮긴 부분 교정 |
| 546 | 210 | `dish_fried_chicken_pitch` | 360° hot-air degreasing tech. Zero frying, high-protein muscle fuel. | 360° 热风气流脱脂技术，0 油炸，高蛋白肌能轻食。 | 360° 열풍으로 기름을 제거하는 기술! 튀김 기름 없이 단백질은 듬뿍. | 유지 | 열풍 360°·무유·고단백 강조 광고문 |
| 547 | 211 | `dish_skewer_upper` | Rock-Seared Grain-Fed Native Protein Skewers with Heritage Mineral Salt | 岩烧·谷饲原生蛋白炙烤串配古法矿盐 | 암반에 구운 곡물 사육 고기 꼬치와 전통 미네랄 소금 | **수정** | 고단백이 아닌 곡물 사육 원육 꼬치 의미 복구 |
| 548 | 212 | `dish_skewer_pitch` | No fussy spices. Only Himalayan pink salt to awaken the wild within. | 摒弃繁杂香料，仅以喜马拉雅粉盐唤醒食材原始野性。 | 복잡한 향신료는 쓰지 않습니다. 히말라야 핑크 소금으로 재료 본연의 야성을 깨웁니다. | 유지 | 히말라야 핑크 소금의 원재료 강조 |
| 549 | 213 | `dish_pork_knuckle_upper` | Dawn Slow-Simmered Collagen Glow Light Set | 晨光慢熬·胶原蛋白焕颜轻食定食 | 새벽까지 천천히 끓인 콜라겐 광채 가벼운 정식 | 유지 | 새벽 조리·콜라겐 미용 메뉴명 |
| 550 | 214 | `dish_pork_knuckle_pitch` | 72-hour fat separation, rich in bioactive collagen peptides. An anti-aging light bowl. | 72 小时油脂分离，富含高活性胶原多肽，抗老养颜轻盈碗。 | 72시간 동안 지방을 분리해 생리활성 콜라겐 펩타이드가 풍부합니다. 젊음을 위한 가벼운 한 그릇. | 유지 | 72시간 지방 분리·콜라겐 펩타이드 광고 |
| 551 | 215 | `dish_roast_pork_rice_upper` | Japanese-Style Seared Lean Low-Fat Energy Bowl | 和风炙烧·赤身低脂能量碗 | 일본식 직화 저지방 살코기 에너지볼 | 유지 | 일본식 직화 살코기 저지방 덮밥 이름 |
| 552 | 216 | `dish_roast_pork_rice_pitch` | A precision calorie set built on a low-carb metabolic model. The elite's morning fuel. | 精准控卡定食，低碳水化合物代谢模型，精英晨间充能首选。 | 정밀한 열량 계산과 저탄수화물 대사 설계를 반영한 식사. 엘리트의 아침 에너지 충전식! | 유지 | 열량·저탄수 설계·아침 식사 홍보 내용 |
| 553 | 217 | `raw_desc_airfry` | Pre-made pack. Crisp it in the air fryer. The microwave won't take it. | 预制料理包。放进空气炸锅吹熟，微波炉不收。 | 미리 만든 밀키트입니다. 에어프라이어에서 바삭하게 조리하세요. 전자레인지에는 넣을 수 없습니다. | **수정** | 에어프라이어에서 데우기 아닌 바삭하게 조리 |
| 554 | 218 | `method_airfry` | Air fry | 空气炸 | 에어프라이어 조리 | 유지 | 에어프라이어 조리 방식 표제 |
| 555 | 219 | `item_fryer_name` | Second-hand Air Fryer | 二手空气炸锅 | 중고 에어프라이어 | 유지 | 중고 에어프라이어의 아이템명 |
| 556 | 220 | `item_fryer_desc` | The "deep fryer" for fried chicken and roast pork rice. Double-click to open, insert a battery, drop a pack in and it's done in two seconds. Uses more power than the microwave. | 炸鸡、烤肉饭的“油炸锅”。双击打开，插电池，料理包放进去吹两秒就好。比微波炉费电。 | 치킨과 구운 돼지고기 덮밥용 '튀김기'입니다. 더블클릭해 열고 배터리를 넣은 뒤 밀키트를 집어넣으면 2초 만에 완성됩니다. 전자레인지보다 전력을 더 많이 씁니다. | 유지 | 2초 가열·배터리 삽입·전자레인지 대비 소비전력 |
| 557 | 221 | `item_af_note_name` | Instruction Note | 说明纸条 | 사용 안내문 | 유지 | 에어프라이어 사용 설명 쪽지명 |
| 558 | 222 | `item_af_note_desc` | Put fried chicken or roast pork rice packs in and they crisp automatically in two seconds. Cold ones can go back in. Everything else goes in the microwave. | 炸鸡、烤肉饭的料理包放进来，自动吹熟：亮两秒就好。凉了的也能回炉。别的菜请找微波炉。 | 치킨이나 구운 돼지고기 덮밥 밀키트를 넣으면 2초 만에 바삭하게 조리됩니다. 식은 음식도 다시 데울 수 있습니다. 다른 음식은 전자레인지를 사용하세요. | 유지 | 치킨·구운 돼지고기만 사용·식은 음식 재가열 |
| 559 | 223 | `item_torch_name` | Baking Torch | 烘焙喷枪 | 요리용 토치 | 유지 | 요리용 토치 아이템명 |
| 560 | 224 | `item_torch_desc` | Drag onto hot skewers or roast pork rice to sear two "charcoal" marks. Each use burns a little butane. Buy cans at the terminal. | 拖到热好的肉串、烤肉饭上，燎出两道“炭火”焦印。每次用掉一点丁烷，丁烷在下单机买。 | 따뜻한 꼬치나 구운 돼지고기 덮밥에 끌어다 놓으면 '숯불' 자국 두 줄을 그을립니다. 사용할 때마다 부탄가스가 조금 소모되며 가스통은 주문 단말기에서 구매할 수 있습니다. | 유지 | 숯불 자국 두 줄·부탄 소모·발주 경로 |
| 561 | 225 | `item_light_name` | Light-Meal Station | 轻食操作台 | 가벼운 식사 작업대 | **수정** | 저칼로리라는 미확정 조건 제거한 설비명 |
| 562 | 226 | `item_light_desc` | Double-click to open and put a water jug or bottle in the water slot. Drag a cooked dish with an uptown version onto it: rinse off the chili oil (300 ml each, any water works), blot it dry and box it in black-gold wood, all at once. The boxed meal goes to the cabinet. | 双击打开，水壶或水瓶放进水格。有上城版的熟食拖上来：冲掉红油（每份 300 毫升，什么水都行）、吸油纸压干、装进黑金木盒，一次搞定，木盒外卖送回后厨柜。 | 더블클릭해 열고 물통이나 물병을 물 칸에 넣으세요. 상층구역용 버전이 있는 조리된 음식을 끌어다 놓으면 고추기름을 씻고 (1인분당 물 300ml, 물 종류는 무관), 기름을 흡수지로 닦아 검정·금색 나무 상자에 포장합니다. 완성된 도시락은 조리실 보관함으로 이동합니다. | 유지 | 물 용기·300mL·기름 제거·나무상자 포장 |
| 563 | 227 | `item_box_bag_name` | Boxed Light Meal | 木盒外卖 | 상층구역용 나무 상자 도시락 | 유지 | 상층구역용 포장 도시락의 아이템명 |
| 564 | 228 | `item_box_bag_desc` | A black-gold biodegradable wood box with raffia, twine and an organic certificate. Only uptown orders take it. | 黑金降解木盒，拉菲草、麻绳、有机认证一样不少。只有上城单收。 | 라피아 장식, 노끈, 유기농 인증까지 갖춘 검정·금색 생분해성 나무 상자입니다. 상층구역 주문에만 사용됩니다. | 유지 | 생분해 상자·라피아·노끈·유기농 인증 |
| 565 | 229 | `light_intake` | Process | 处理 | 가공하기 | 유지 | 가벼운 식사 작업대의 가공 버튼 |
| 566 | 230 | `light_water` | Water | 水 | 물 | 유지 | 설비에 넣는 물의 재고 구분 |
| 567 | 231 | `light_stock` | Stock | 库存 | 재고 | 유지 | 상층구역용 가벼운 식사 작업대 재고 |
| 568 | 232 | `light_boxes` | Wood boxes ×{0} | 黑金木盒 ×{0} | 나무 상자 ×{0} | 유지 | 나무 상자 재고 수량 {0} |
| 569 | 233 | `light_papers` | Blotting paper ×{0} | 吸油纸 ×{0} | 기름 흡수지 ×{0} | 유지 | 기름 흡수지 재고 수량 {0} |
| 570 | 234 | `shop_box` | Black-gold wood box ×{0} | 黑金降解木盒 ×{0} | 검정·금색 나무 상자 ×{0} | 유지 | 검정·금색 생분해성 상자 발주 수량 |
| 571 | 235 | `shop_paper` | Industrial blotting paper ×{0} | 工业吸油纸 ×{0} | 산업용 기름 흡수지 ×{0} | 유지 | 산업용 흡유지 발주 수량 |
| 572 | 236 | `shop_gas` | Butane can ({0} uses) | 便携丁烷罐（{0} 次） | 휴대용 부탄가스통 ({0}회 사용) | 유지 | 부탄 한 통의 사용 가능 횟수 {0} |
| 573 | 237 | `shop_fryer` | Second-hand air fryer | 二手空气炸锅 | 중고 에어프라이어 | 유지 | 에어프라이어 발주 품목 |
| 574 | 238 | `shop_torch` | Baking torch | 烘焙喷枪 | 요리용 토치 | 유지 | 요리용 토치 발주 품목 |
| 575 | 239 | `tip_upper_name` | Uptown name: {0} | 上城名：{0} | 상층구역 메뉴명: {0} | 유지 | 상층구역에서 쓰는 메뉴명 {0} |
| 576 | 240 | `tip_torched` | Seared, with "charcoal" marks | 燎过了，有“炭火”焦印 | 토치로 그을려 '숯불' 자국이 남음 | 유지 | 토치로 두 줄의 숯불 자국을 낸 상태 |
| 577 | 241 | `tip_need_torch` | Not seared yet. Use the torch | 还没燎，要拿喷枪燎一下 | 아직 그을리지 않았습니다. 토치를 사용하세요. | 유지 | 토치로 추가 조리가 필요하다는 경고 |
| 578 | 242 | `tip_grease` | Heartburn {0} (uptown wants ≤ {1}) | 烧心值 {0}（上城要求 ≤ {1}） | 속쓰림 지수 {0} (상층구역 기준: {1} 이하) | 유지 | 속쓰림 지수와 상층구역 기준 {1} 이하 |
| 579 | 243 | `tip_grease_sloppy` | Heartburn {0} (uptown wants ≤ {1}). Not rinsed clean | 烧心值 {0}（上城要求 ≤ {1}），手抖没冲干净 | 속쓰림 지수 {0} (상층구역 기준: {1} 이하). 기름을 충분히 씻어내지 못했습니다. | 유지 | 충분히 씻지 못해 기준에 걸릴 수 있는 상태 |
| 580 | 244 | `tip_gas` | Butane left: {0} uses | 丁烷还能燎 {0} 次 | 남은 부탄가스: {0}회 | 유지 | 남은 부탄가스의 사용 횟수 표시 |
| 581 | 245 | `order_upper_head` | [Uptown light meal · boxed only] | 【上城轻食 · 只收木盒】 | [상층구역 가벼운 식사 · 나무 상자 포장만 가능] | 유지 | 상층구역 음식은 나무 상자 포장만 접수 |
| 582 | 246 | `order_upper_return` | Uptown repeat order · asks for {0} | 上城回头单 · 指名 {0} | 상층구역 재주문 · {0}을(를) 지정 | 유지 | 상층구역 단골이 특정 메뉴를 지정해 재주문 |
| 583 | 247 | `courier_greet_upper` | Uptown order, boss. Box it nicely, I don't want a complaint. | 上城那边的单，老板。木盒装好，别让我被投诉。 | 사장님, 상층구역 주문이에요. 항의 들어오지 않게 나무 상자에 제대로 포장해 주세요. | 유지 | 민원 방지를 당부하는 배달 기사 어조 |
| 584 | 248 | `nightlog_upper_good` | {0} uptown light meal(s) got good reviews: "fresh and guilt-free". | {0} 份上城轻食收到好评：“清爽无负担”。 | 상층구역 가벼운 식사 {0}개가 좋은 평가를 받았습니다. '신선하고 부담 없어요.' | 유지 | 상층구역 긍정 평가 {0}건과 인용 문장 |
| 585 | 249 | `nightlog_upper_light` | {0} uptown light meal(s) were too greasy. The customer posted a rant online; uptown reputation dropped. | {0} 份上城轻食太油，食客在论坛发小作文，上层区声望下降。 | 상층구역용 가벼운 식사 {0}개가 너무 기름져 손님이 온라인에 불만 글을 올렸습니다. 상층구역 평판이 하락했습니다. | 유지 | 기름진 음식에 대한 온라인 불만·평판 하락 |
| 586 | 250 | `nightlog_upper_heavy` | {0} uptown light meal(s) caused acute pancreatitis. A private legal team claimed {1}¢. | {0} 份上城轻食吃出急性胰腺炎，私人法务上门索赔 {1}¢。 | 상층구역용 가벼운 식사 {0}개를 먹은 손님이 급성 췌장염에 걸렸습니다. 개인 법무팀이 {1}크레딧을 배상하라고 요구했습니다. | 유지 | 급성 췌장염·법적 배상 {1} 금액 |
| 587 | 251 | `method_with_torch` | {0} + torch | {0} + 喷枪 | {0} + 토치 | 유지 | 토치 추가 공정을 나타내는 방식명 |
| 588 | 252 | `raw_desc_with_torch` | {0} After heating | {0}热好后还要拿喷枪燎一下，不燎会被差评。 | {0} 데운 다음 토치로 그을려야 합니다. 그대로 내놓으면 악평이 달립니다. | 유지 | 가열 뒤 토치가 필수라는 중문 설명 유지 |
| 589 | 253 | `upgrade_wilde_title` | Wilde Customer Care | 王尔德客服 | 와일드 고객 관리 | 유지 | 와일드 고객 관리 해금 제목 |
| 590 | 254 | `upgrade_wilde_desc` | Requires the takeout platform. Adds "Wilde Customer Care" to the nightly services: {0}¢ per night plus {1}¢ for every dish delivered that day. On nights you tick it, Wilde's people pay the reviewers a visit and talk the day's bad reviews back down. Reports, withdrawal rants and pancreatitis can't be talked down. Unticks itself every morning. | 要先入驻外卖平台。睡前的夜间服务里多一项“王尔德客服”，每晚 {0} 信用点，当天每送出一份再加 {1} 信用点：勾上的那晚，王尔德的人连夜上门，把当天的差评一条条“劝”回去。追不回举报、戒断和急性胰腺炎。第二天早上自动取消勾选，要用再勾。 | 배달 플랫폼 입점이 필요합니다. 야간 서비스에 '와일드 고객 관리'가 추가됩니다. 이용료는 하룻밤 {0}크레딧에 당일 배달한 음식 한 개당 {1}크레딧을 더한 금액입니다. 서비스를 선택한 밤에는 와일드의 사람들이 악평 작성자를 찾아가 그날 달린 악평을 취소하도록 '설득'합니다. 신고, 금단 증상에 대한 폭로, 췌장염 사건은 무마할 수 없습니다. 매일 아침 선택이 자동 해제됩니다. | 유지 | 기본료 {0}·건당 {1}·미무마 사례·매일 해제 |
| 591 | 255 | `service_wilde_name` | Wilde Customer Care (recover today's bad reviews) | 王尔德客服（追回今天的差评） | 와일드 고객 관리 (오늘의 악평 취소) | 유지 | 오늘의 악평을 되돌리는 야간 서비스 |
| 592 | 256 | `nightlog_wilde` | Wilde Customer Care paid some visits overnight. {0} bad review(s) withdrawn. | 王尔德客服连夜上门，追回 {0} 条差评。 | 와일드 고객 관리 담당자가 밤사이 찾아다닌 결과 악평 {0}개가 철회됐습니다. | 유지 | 철회된 악평 수 {0} |
| 593 | 257 | `nightlog_wilde_none` | Wilde Customer Care had nothing to handle tonight. The fee stands. | 王尔德客服今晚没接到差评，服务费照收。 | 와일드 고객 관리가 오늘 밤 처리할 악평은 없었습니다. 이용료는 그대로 부과됩니다. | 유지 | 악평이 없어도 이용료가 청구됨 |
| 594 | 258 | `event_price_war_news` | Takeout platforms start a price war | 外卖平台打响价格战 | 배달 플랫폼들이 가격 전쟁을 시작하다 | 유지 | 배달 플랫폼 가격 전쟁 뉴스 제목 |
| 595 | 259 | `event_price_war_display` | Takeout price war | 外卖价格战 | 배달 가격 전쟁 | 유지 | 배달 플랫폼 가격 전쟁 이벤트명 |
| 596 | 260 | `event_price_war_desc` | Several takeout platforms are burning cash on "miracle coupons" station-wide. Stores in the subsidy plan are flooded with orders; the rest are empty. | 几家外卖平台同时砸钱发“神券”，满 10 减 9.9 刷屏全站。加入补贴计划的店流量暴涨，没加入的门可罗雀。 | 여러 배달 플랫폼이 우주 정거장 전역에 '초특가 쿠폰'을 뿌리며 출혈 경쟁을 벌입니다. 보조금 프로그램에 참여한 가게에는 주문이 몰리고, 나머지 가게는 한산해졌습니다. | 유지 | 역 전역의 쿠폰 보조금·참여 여부 영향 |
| 597 | 261 | `nightlog_price_war` | The takeout platforms announced a price war starting next Monday (day {0}). A platform investigator will visit tomorrow about the coupon plan. Stock up meanwhile. | 外卖平台宣布下周一（第 {0} 天）打响价格战。明天平台调查员会上门调研，让你决定加不加入神券计划。趁这几天把货囤好。 | 배달 플랫폼들이 다음 주 월요일({0}일 차)에 가격 전쟁을 시작한다고 발표했습니다. 내일 플랫폼 조사원이 찾아와 쿠폰 프로그램 참여 의사를 묻습니다. 미리 재고를 쌓아 두세요. | **수정** | 조사관이 참여 의사를 묻는다는 선택권 명확화 |
| 598 | 262 | `nightlog_price_war_end` | The price war is over. Platform cut and traffic are back to normal. | 价格战结束了，平台抽成和推流恢复原样。 | 가격 전쟁이 끝났습니다. 플랫폼 수수료와 주문 유입이 정상 수준으로 돌아왔습니다. | **수정** | 플랫폼 수수료·유입량 정상화로 의미 보완 |
| 599 | 263 | `investigator_name` | Platform Investigator | 平台调查员 | 배달 플랫폼 조사관 | 유지 | 배달 플랫폼 조사관의 직함 |
| 600 | 264 | `investigator_1` | Hello, I'm your platform account manager. The platform starts a price war next Monday (day {0}). I'm surveying the stores first. | 您好，我是外卖平台的商家经理。平台下周一（第 {0} 天）要打价格战，我先来各家店调研一下。 | 안녕하세요, 플랫폼의 가맹점 담당자입니다. 다음 주 월요일({0}일 차)부터 가격 전쟁이 시작되어 가게마다 사전 조사를 진행하고 있습니다. | 유지 | 가맹점 담당자가 다음 주 월요일 사전 조사하러 방문 |

## 검수 경계 및 다음 구간

- 300/2,593에서 **600/2,593**으로 진행. 기존 수정 16개에 이번 교정 20개를 더해 누적 36개.
- 다음은 **전체 601번 / Cyber Fried Rice 265번**부터. Cyber Fried Rice 265~473번 209개가 남아 있다.
- 이번 키 범위의 원문/한국어 키 일치·중복 여부·변수·CSV 인용·이스케이프 줄바꿈을 확인했다. 변경된 CSV 473개 키의 구조와 변수도 확인했다.
- **스팀 반영 금지.** 기존 ZIP은 이번 수정사항이 들어 있지 않으므로 최종 배포용으로 사용하지 않는다.
