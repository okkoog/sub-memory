# 신규 번역 수동 원문 대조 — 4차 (전체 601~900번)

검수 300개: Cyber Fried Rice 265~473 (209개), NPC Girls 1~91 (91개). **수정 33개 / 유지 267개**.
인게임 동작과 폰트는 확인되지 않은 텍스트 기반 수동 검수다. 스팀과 외부 모드 DLL, 외부 모드 CSV, 기존 ZIP은 변경하지 않았다.
각 항목에서 영어 원문과 중국어 원문을 함께 대조했다. 원문에 있는 `{0}` 계열 변수를 보존하고, 기존 CSV와 수정 CSV의 변수·줄바꿈·서식 태그를 비교했다.

## Cyber Fried Rice (3834071305) — 265~473
| 전체 순번 | 모드 순번 | 키 | 영어 원문 | 중국어 원문 | 최종 한국어 | 판정 | 개별 검수 메모 |
|---:|---:|---|---|---|---|---|---|
| 601 | 265 | `investigator_2` | We've prepared a coupon plan. We fund the subsidy, we send the traffic. The cut just needs adjusting. | 我们准备了神券计划，补贴我们出，流量我们给。就是抽成要调一调。 | 쿠폰 프로그램을 마련했습니다. 할인액은 저희가 부담하고 주문도 더 보내 드립니다. 다만 수수료는 조금 조정해야 합니다. | 유지 | 보조금은 플랫폼 부담이고 수수료만 조정하는 제안 |
| 602 | 266 | `investigator_offer` | Everyone's handing out coupons, and diners go where it's cheaper. The platform just routes by algorithm. Join, and we fund the subsidy; the cut goes from {0}% to {1}%, with {2} extra couriers a day. Don't, and the coupon stores will probably take your orders: {3} fewer a day. {4} days. Your call. | 同行都在发券让利，食客自然往便宜的店走，平台只是按算法分流。加入的话，补贴我们出，抽成从 {0}% 调到 {1}%，每天多给您推 {2} 位骑手。不加入嘛，单子大概会被发券的店分走，每天少 {3} 位。为期 {4} 天，您定。 | 다른 가게들도 쿠폰을 뿌리고 있으니 손님들은 당연히 저렴한 곳으로 몰리죠. 플랫폼은 알고리즘에 따라 주문을 배분할 뿐입니다. 참여하시면 보조금은 저희가 부담하고 수수료는 {0}%에서 {1}%로 조정됩니다. 하루에 배달 기사도 {2}명 더 찾아오고요. 참여하지 않으시면 쿠폰을 주는 가게에 주문을 빼앗겨 하루에 {3}명이 줄어들 수 있습니다. 기간은 {4}일입니다. 선택은 사장님 몫입니다. | 유지 | 수수료 변화와 기사 증감 및 기간 네 변수 보존 |
| 603 | 267 | `investigator_choice_join` | Join the coupon plan | 加入神券计划 | 쿠폰 프로그램에 참여한다 | 유지 | 쿠폰 프로그램 참여 선택지 |
| 604 | 268 | `investigator_choice_decline` | Decline | 不加入 | 참여하지 않는다 | 유지 | 쿠폰 프로그램 거절 선택지 |
| 605 | 269 | `investigator_join_reply` | Great! The contract starts next Monday (day {0}), then orders won't stop for {1} days. Better stock up. | 爽快！合同签好了，下周一（第 {0} 天）生效，之后 {1} 天单子管够。这几天赶紧囤货吧。 | 좋습니다! 계약은 다음 주 월요일({0}일 차)부터 적용되며 {1}일 동안 주문이 밀려들 겁니다. 재고를 넉넉히 확보하세요. | 유지 | 계약 시작일과 주문 증가 기간의 수치 유지 |
| 606 | 270 | `investigator_decline_reply` | Understood, your call entirely. Just so you know, most of the street signed. If orders dip, don't blame the algorithm. | 理解理解，您考虑周全。不过隔壁几家都签了，到时候单子少了，别怪算法。 | 알겠습니다. 사장님 선택이죠. 다만 주변 가게는 대부분 계약했어요. 주문이 줄어도 알고리즘을 탓하진 마세요. | 유지 | 불참 시 알고리즘 경쟁 불이익 설명 |
| 607 | 271 | `event_scandal_news` | "Guochao takeout exposed" video goes viral | 国潮外卖打假视频走红 | '궈차오 배달 음식의 실체' 폭로 영상 확산 | 유지 | 궈차오 외식 폭로 영상 뉴스의 이름 |
| 608 | 272 | `event_scandal_display` | Takeout exposé | 外卖打假风波 | 배달 음식 폭로 사태 | 유지 | 배달 음식 조리 방식 폭로 사태 표시 |
| 609 | 273 | `event_scandal_desc` | A blogger unboxed ten "wok-fresh" takeouts. Nine were reheated meal packs. The video passed a hundred million views and the comments are naming stores one by one. | 一名博主把十家“现炒”外卖挨个拆箱，九家都是料理包加热。视频播放量破亿，评论区开始逐家点名。 | 한 영상 제작자가 '갓 볶은 음식'을 판다는 배달 가게 10곳의 음식을 뜯어봤습니다. 그중 9곳은 밀키트를 데우기만 하고 있었습니다. 영상 조회수가 1억을 넘기자 댓글에서 가게 이름을 하나씩 거론하기 시작했습니다. | **수정** | 10개 음식 아닌 가게 열 곳 중 아홉 곳이라는 단위 교정 |
| 610 | 274 | `nightlog_scandal` | A takeout exposé video went viral and your reviews got flooded too. Likes −{0}, now {1}. | 国潮外卖打假视频走红，你家的评论区也被刷屏了。点赞 −{0}，现在 {1} 个赞。 | 배달 음식 폭로 영상이 퍼져 가게의 댓글란도 난리가 났습니다. 좋아요 -{0}, 현재 {1}개. | 유지 | 폭로 이후 좋아요 감소·잔여량 변수 확인 |
| 611 | 275 | `event_drama_news` | AI micro-drama "Heartburn Takeout" blows up | AI 短剧《烧心外卖》爆火 | AI 숏드라마 '속쓰림 배달' 대유행 | 유지 | AI 숏드라마의 명칭 및 인기 상태 |
| 612 | 276 | `event_drama_display` | "Heartburn Takeout" trending | 《烧心外卖》热播 | '속쓰림 배달' 인기 급상승 | 유지 | 숏드라마 인기 상승 UI 제목 |
| 613 | 277 | `event_drama_desc` | The AI-generated micro-drama "Heartburn Takeout", about a delivery rider who becomes an uptown chef, is everywhere. Viewers are ordering "the same takeout". | AI 生成的短剧《烧心外卖》讲外卖小哥逆袭成为上城大厨，全站刷屏。观众纷纷下单“同款外卖”。 | 배달 기사가 상층구역 요리사로 성공하는 내용을 다룬 AI 숏드라마 '속쓰림 배달'이 온 플랫폼에서 화제입니다. 시청자들이 드라마에 나온 음식을 주문하고 있습니다. | 유지 | 배달 기사에서 상층구역 요리사로 역전한 내용 |
| 614 | 278 | `nightlog_drama` | The AI drama "Heartburn Takeout" blew up and viewers want the same meal. Likes +{0}, now {1}. | AI 短剧《烧心外卖》爆火，观众跑来点同款。点赞 +{0}，现在 {1} 个赞。 | AI 드라마 '속쓰림 배달'이 대박 나면서 시청자들이 드라마 속 음식을 주문했습니다. 좋아요 +{0}, 현재 {1}개. | 유지 | 드라마 인기에 따른 좋아요 증가 수치 |
| 615 | 279 | `nightlog_drama_upper` | The AI drama "Heartburn Takeout" blew up and viewers want the same meal. Likes +{0}, now {1}. Expect more uptown orders for a few days. | AI 短剧《烧心外卖》爆火，观众跑来点同款。点赞 +{0}，现在 {1} 个赞。这几天上城单也会多一些。 | AI 드라마 '속쓰림 배달'이 대박 나면서 시청자들이 드라마 속 음식을 주문했습니다. 좋아요 +{0}, 현재 {1}개. 며칠 동안 상층구역 주문이 늘어날 전망입니다. | 유지 | 상층구역 주문이 며칠 늘어난다는 부가 효과 |
| 616 | 280 | `nightlog_price_war_tomorrow` | The price war starts tomorrow. Stores outside the coupon plan will lose traffic to those inside. | 价格战明天开打。没加入神券计划的店，推流会被别家抢走。 | 가격 전쟁이 내일 시작됩니다. 쿠폰 프로그램에 참여하지 않은 가게는 참여한 가게에 주문을 빼앗길 겁니다. | 유지 | 쿠폰 프로그램 미참여 점포의 주문 감소 예고 |
| 617 | 281 | `nightlog_price_war_tomorrow_join` | The price war starts tomorrow and your coupon plan kicks in: more orders, higher cut. Have stock ready. | 价格战明天开打，神券计划生效：单子会多起来，抽成也跟着涨。备好货。 | 가격 전쟁이 내일 시작되고 쿠폰 프로그램도 적용됩니다. 주문은 늘지만 플랫폼 수수료도 올라갑니다. 재고를 준비해 두세요. | 유지 | 참여 점포 주문 증가·수수료 증가 조건 |
| 618 | 282 | `board_coupon_soon` | Coupon plan · starts next Monday ({0} day(s) left): {1}% cut, {2} extra couriers a day. Stock up. | 神券计划 · 下周一生效（还有 {0} 天）：抽成 {1}%，每天多 {2} 位骑手。先把货囤好。 | 쿠폰 프로그램 · 다음 주 월요일 시작 ({0}일 남음): 수수료 {1}%, 하루 배달 기사 +{2}명. 재고를 확보하세요. | 유지 | 시작일까지 남은 날 수와 증가 기사 수 |
| 619 | 283 | `board_coupon_on` | Coupon plan active: {0}% cut, {1} extra couriers a day, {2} day(s) left. | 神券计划生效中：抽成 {0}%，每天多 {1} 位骑手，还剩 {2} 天。 | 쿠폰 프로그램 적용 중: 수수료 {0}%, 하루 배달 기사 +{1}명, {2}일 남음. | 유지 | 진행 기간 잔여일·수수료·기사 수 |
| 620 | 284 | `board_coupon_tip` | You joined the platform's coupon plan for the price war: more orders, higher cut. It ends with the price war. | 价格战期间加入了平台的神券计划：单子多、抽成高。价格战结束后自动退出。 | 가격 전쟁 기간에 플랫폼 쿠폰 프로그램에 참여했습니다. 주문은 늘지만 수수료도 높아집니다. 가격 전쟁이 끝나면 자동으로 종료됩니다. | 유지 | 가격 전쟁 종료 시 쿠폰 계약 자동 종료 |
| 621 | 285 | `nightlog_scandal_none` | A takeout exposé video went viral and your reviews got flooded too. Luckily there were no likes left to lose. | 国潮外卖打假视频走红，你家的评论区也被刷屏了。好在本来就没什么赞可掉。 | 배달 음식 폭로 영상으로 가게의 댓글란도 난리가 났습니다. 다행히 잃을 좋아요가 이미 하나도 없었습니다. | 유지 | 이미 좋아요가 없어 추가 하락 없다는 예외 |
| 622 | 286 | `boss_name` | Boss on a Delivery Shift | 跑单的老板 | 직접 배달 나온 플랫폼 대표 | 유지 | 플랫폼 대표가 직접 배달한다는 인물명 |
| 623 | 287 | `boss_greet` | Pickup, brother. I'm riding with the crew today to see the front line. I started out delivering too, you know. | 兄弟，取餐。我今天跟大伙一起跑一天，体验体验一线。当年我也是从送货干起来的。 | 형씨, 음식 픽업하러 왔어. 오늘은 나도 직원들과 같이 뛰면서 현장을 좀 보려고. 나도 배달 일부터 시작했거든. | 유지 | 사장이 과거 배달 경험을 언급하는 친근한 인사 |
| 624 | 288 | `boss_greet_sign` | Pickup, brother. ...That "Wok-Fresh" sign out front, is it really wok-fresh? Never mind. Everyone's working hard. | 兄弟，取餐。……门口那块“现炒”的牌子，真是现炒？算了，兄弟们都不容易。 | 형씨, 음식 픽업이야. ……밖에 걸린 '갓 볶은 음식' 간판, 정말 갓 볶은 거 맞아? 뭐, 됐어. 다들 먹고살기 바쁘잖아. | 유지 | 현장 볶음 간판의 진위에 의문을 품는 대사 |
| 625 | 289 | `boss_accept` | Fast pickup, good! I'm waiving the platform cut on this one. Thanks for the hard work, brothers! | 出餐快，好！这单的抽成我做主免了。兄弟们辛苦！ | 음식이 빨리 나왔네! 좋아, 이 주문의 플랫폼 수수료는 내가 면제해 줄게. 다들 수고 많아! | 유지 | 빠른 출고에 대한 수수료 면제 보상 |
| 626 | 290 | `nightlog_boss_praise` | One of today's couriers was the platform boss himself, riding a shift. He praised your fast pickup and waived the cut on that order. {0} extra courier(s) tomorrow. | 今天来取餐的骑手里有一位是亲自跑单的平台老板。他点名表扬你家出餐快，那单抽成全免。明天多来 {0} 位骑手。 | 오늘 온 배달 기사 중 한 명은 직접 배달 체험에 나선 플랫폼 사장이었습니다. 빠른 음식 준비를 칭찬하며 해당 주문의 수수료를 면제했습니다. 내일 배달 기사 +{0}명. | 유지 | 칭찬·수수료 면제·내일 기사 증가 기록 |
| 627 | 291 | `nightlog_boss_timeout` | The platform boss rode a shift today and left your counter empty-handed. The platform issued a notice overnight. Likes −{0}, {1} fewer courier(s) tomorrow. | 今天亲自跑单的平台老板在你家空等了一场，平台连夜通报。点赞 −{0}，明天少来 {1} 位骑手。 | 오늘 직접 배달하러 나온 플랫폼 사장이 빈손으로 가게를 떠났습니다. 플랫폼이 밤사이 공지를 냈습니다. 좋아요 -{0}, 내일 배달 기사 -{1}명. | 유지 | 배송 지연으로 플랫폼 공지가 난 불이익 |
| 628 | 292 | `blogger_name` | Food Vlogger | 探店博主 | 맛집 탐방 크리에이터 | 유지 | 방문형 맛집 영상 제작자의 호칭 |
| 629 | 293 | `blogger_1` | Hey fam, today we're checking out this "wok-fresh" takeout spot! ...Camera off for a sec. Boss, I just got some footage by your back kitchen. | 家人们，今天探的是这家“现炒”外卖！……镜头先关一下。老板，我刚在你家后厨门口拍了点东西。 | 여러분, 오늘은 '갓 볶은 음식'을 판다는 이 배달 가게를 찾아왔습니다! ……잠깐 카메라 끄고요. 사장님, 방금 주방 쪽에서 재미있는 걸 좀 찍었거든요. | 유지 | 촬영 중단 후 주방 영상을 보유했다고 압박 |
| 630 | 294 | `blogger_2` | Microwaves. Meal packs, by the crate. If this video goes up, your shop is done. | 微波炉，料理包，一箱一箱的。这条视频发出去，你这店也就到头了。 | 전자레인지에 밀키트가 상자째 쌓여 있더군요. 이 영상이 올라가면 가게는 끝장날 겁니다. | 유지 | 전자레인지와 상자 밀키트로 협박하는 대사 |
| 631 | 295 | `blogger_offer` | I'm a reasonable guy. {0}¢ for my trouble and this footage never existed. Short on cash? I'll take goods. Or for {1}¢, I'll shoot you a "most honest wok-fresh kitchen on the station" feature instead. Pay nothing and it goes up tonight. Your call. | 我也不是不讲理的人。{0}¢ 辛苦费，视频我就当没拍过；手头紧的话，拿东西抵也行。要是肯出 {1}¢，我反过来给你拍一期“深空最良心现炒”。都不给的话，今晚就发。您定。 | 저도 말이 통하는 사람입니다. 수고비로 {0}크레딧만 주시면 촬영한 영상은 없던 일이 되죠. 현금이 부족하면 물건으로 받아도 되고요. 아니면 {1}크레딧을 주시면 오히려 '정거장에서 가장 정직한 직화 요리 가게'라는 홍보 영상을 찍어 드리겠습니다. 돈을 안 주면 오늘 밤 영상을 공개하죠. 결정하세요. | 유지 | 입막음·물품 대납·홍보 영상 각 선택지 가격 |
| 632 | 296 | `blogger_choice_pay` | Pay {0}¢ | 给 {0}¢ | {0}크레딧을 낸다 | 유지 | 입막음 비용 현금 지급 선택 |
| 633 | 297 | `blogger_choice_refuse` | Post it, then | 爱发就发 | 그럼 공개해 봐 | 유지 | 폭로를 감수하고 거절하는 선택 |
| 634 | 298 | `blogger_pay_reply` | Pleasure doing business! Fam, this place is the real deal, pure wok fire! ...Kidding. Footage deleted. | 爽快！家人们，这家真是现炒，锅气十足，冲！……开玩笑的，视频我删了。 | 거래가 시원해서 좋네요! 여러분, 여기야말로 진짜 직화 요리 맛집입니다! ……농담이에요. 영상은 삭제했어요. | 유지 | 촬영물 삭제와 위선적인 홍보 말투 |
| 635 | 299 | `blogger_refuse_reply` | Bold. Tonight, eight o'clock. Don't miss it. | 行，有骨气。今晚八点，记得看。 | 배짱 좋네요. 오늘 밤 8시에 올릴 테니 놓치지 마세요. | 유지 | 당일 밤 8시에 공개하겠다는 경고 |
| 636 | 300 | `nightlog_blogger_paid` | You slipped the food vlogger {0}¢ in hush money. The kitchen video never went up. | 你给探店博主塞了 {0}¢ 封口费，那条后厨视频没发出来。 | 먹방 크리에이터에게 입막음 비용으로 {0}크레딧을 건넸습니다. 주방 영상은 공개되지 않았습니다. | 유지 | 입막음 비용 지불로 영상 미공개 처리 |
| 637 | 301 | `nightlog_blogger_exposed` | The food vlogger posted the kitchen footage: "Wok-Fresh Exposed: The Microwave Is the Real Chef". Likes −{0}, now {1}. | 探店博主把后厨视频发了出来，标题《揭秘“现炒”外卖：微波炉才是主厨》。点赞 −{0}，现在 {1} 个赞。 | 먹방 크리에이터가 '갓 볶은 음식의 실체: 진짜 요리사는 전자레인지!'라는 제목으로 주방 영상을 공개했습니다. 좋아요 -{0}, 현재 {1}개. | 유지 | 공개된 영상 제목과 좋아요 감소·현재치 |
| 638 | 302 | `event_blogger_exposed_news` | Food vlogger exposes a "wok-fresh" kitchen | 探店博主曝光“现炒”外卖后厨 | 먹방 크리에이터가 '갓 볶은 음식' 주방의 실태를 폭로하다 | 유지 | 폭로 영상을 알리는 뉴스 제목 |
| 639 | 303 | `event_blogger_exposed_display` | Vlogger exposé | 探店曝光 | 먹방 크리에이터의 폭로 | 유지 | 폭로 사건의 UI 제목 |
| 640 | 304 | `event_blogger_exposed_desc` | A food vlogger with a million followers posted footage from a takeout stall's back kitchen: crates of meal packs and a row of microwaves. Top comment: "So that's why they all taste the same." | 一位百万粉探店博主发布视频，镜头扫过某家外卖档口的后厨：成箱的料理包，一排微波炉。评论区：“我就说怎么每家都一个味。” | 팔로워 100만 명의 먹방 크리에이터가 배달 가게의 주방을 촬영한 영상을 공개했습니다. 화면에는 상자째 쌓인 밀키트와 전자레인지가 줄지어 나옵니다. 인기 댓글은 '그래서 모든 가게 음식 맛이 똑같았구나.' | 유지 | 팔로워 100만·전자레인지 줄·댓글 원문 유지 |
| 641 | 305 | `blogger_choice_items` | Pay {0}¢ in goods | 拿东西抵 {0}¢ | 물건으로 {0}크레딧어치를 지불한다 | 유지 | 현금 아닌 물품으로 값을 치르는 선택 |
| 642 | 306 | `blogger_items_reply` | Fine. Put it on the counter and I'll value it at market. Cover {0}¢ and we're square. | 行，柜台上摆出来，我按市价估。抵够 {0}¢ 就算两清。 | 좋아요. 카운터에 올려놓으면 시세대로 값을 매기죠. {0}크레딧어치를 채우면 끝입니다. | **수정** | 원문의 counter를 카운터로 복구한 지불 위치 |
| 643 | 307 | `blogger_items_progress` | Still {0}¢ short. | 还差 {0}¢。 | 아직 {0}크레딧이 부족합니다. | 유지 | 물품 대금에서 남은 부족액 |
| 644 | 308 | `blogger_items_done` | That'll do. Fam, this place is truly wok-fresh... Kidding. Footage deleted. | 够了够了。家人们，这家真是现炒……开玩笑的，视频我删了。 | 이 정도면 됐어요. 여러분, 이 가게는 진짜 갓 볶은 음식만…… 농담이에요. 영상은 삭제했어요. | 유지 | 시세만큼의 물품을 건네 영상 삭제 확정 |
| 645 | 309 | `blogger_choice_promo` | Pay {0}¢ for the honest-kitchen feature | 出 {0}¢，拍一期良心现炒 | {0}크레딧을 내고 정직한 주방 홍보 영상을 의뢰한다 | 유지 | 홍보 영상 제작에 돈을 지불하는 선택지 |
| 646 | 310 | `blogger_promo_reply` | Now that's class! It goes up starting tomorrow. Title's ready: "The Last Real Wok Fire on the Station". Leave the script to me. | 大气！明天开始连发，标题我都想好了：《深空最后一口锅气》。台词你别管，我来编。 | 통이 크시네요! 내일부터 영상을 올릴 겁니다. 제목도 정해 뒀죠. '정거장에 마지막으로 남은 진짜 직화 요리'. 대본은 저한테 맡기세요. | 유지 | 다음 날 연속 홍보 시작과 영상 제목 |
| 647 | 311 | `nightlog_blogger_promo` | You paid the food vlogger {0}¢ for an "honest wok-fresh kitchen" feature, and it's doing great. Likes +{1}, now {2}. For the next {3} days, {4} extra couriers a day. | 你花 {0}¢ 请探店博主拍了一期“良心现炒”，视频反响很好。点赞 +{1}，现在 {2} 个赞。接下来 {3} 天，每天多来 {4} 位骑手。 | 먹방 크리에이터에게 {0}크레딧을 주고 '정직한 직화 요리 가게' 홍보 영상을 촬영했습니다. 반응이 좋습니다. 좋아요 +{1}, 현재 {2}개. 앞으로 {3}일 동안 하루 배달 기사 +{4}명. | 유지 | 비용·좋아요·기사 추가 기간의 다섯 변수 |
| 648 | 312 | `nightlog_blogger_exposed_partial` | You handed over {2}¢ worth of goods, but not enough. The food vlogger posted the kitchen footage anyway: "Wok-Fresh Exposed: The Microwave Is the Real Chef". Likes −{0}, now {1}. | 你拿东西抵了 {2}¢，没抵够，探店博主还是把后厨视频发了出来，标题《揭秘“现炒”外卖：微波炉才是主厨》。点赞 −{0}，现在 {1} 个赞。 | 물건 {2}크레딧어치를 넘겼지만 금액이 모자랐습니다. 크리에이터가 '갓 볶은 음식의 실체: 진짜 요리사는 전자레인지!'라는 주방 영상을 공개했습니다. 좋아요 -{0}, 현재 {1}개. | 유지 | 물품을 부족하게 납부했을 때 영상 폭로 |
| 649 | 313 | `event_blogger_promo_news` | Food vlogger raves about the station's most honest wok-fresh takeout | 探店博主力荐：深空最良心的现炒外卖 | 먹방 크리에이터가 정거장 최고의 직화 요리 배달 가게를 극찬하다 | 유지 | 음식점 칭찬 영상의 뉴스 제목 |
| 650 | 314 | `event_blogger_promo_display` | Vlogger feature | 探店博主力荐 | 먹방 크리에이터 추천 | 유지 | 크리에이터 홍보 보도 표시 |
| 651 | 315 | `event_blogger_promo_desc` | A food vlogger with a million followers has posted a run of videos praising one takeout stall as "made to order, real wok fire". The comments are a queue: "Ordered." "Link please." | 一位百万粉探店博主连发视频，力荐某家外卖档口“现点现炒、锅气十足”。评论区排起长队：“已下单”“求链接”。 | 팔로워 100만 명의 먹방 크리에이터가 한 배달 가게를 '주문 즉시 조리하는 진짜 직화 요리'라며 칭찬하는 영상을 연달아 올렸습니다. 댓글에는 '주문 완료', '링크 주세요'라는 반응이 줄을 이었습니다. | 유지 | 연속 영상과 주문·링크 요청 댓글의 반응 |
| 652 | 316 | `nightlog_hot` | Tomorrow's trending takeout: {0}. They'll sell more tomorrow, at +{1}% price. Remember to list them on the menu board. | 明天的外卖热搜：{0}。这几道明天出单更多，价格 +{1}%。记得在菜单板上把它们上架。 | 내일의 인기 배달 메뉴: {0}. 내일은 주문이 더 많이 들어오고 가격도 {1}% 오릅니다. 메뉴판에 등록해 두세요. | 유지 | 내일 인기 메뉴의 주문량·가격 상승 |
| 653 | 317 | `board_hot` | Trending today: {0} (more orders, +{1}% price) | 今日热搜：{0}（出单更多，价格 +{1}%） | 오늘의 인기 메뉴: {0} (주문 증가, 가격 +{1}%) | 유지 | 인기 메뉴의 배달 주문·가격 보너스 |
| 654 | 318 | `board_hot_name` | [Trending] {0} | 【热搜】{0} | [인기 급상승] {0} | 유지 | 인기 급상승 메뉴 표시 태그 |
| 655 | 319 | `chef_name` | Old Chef | 老厨师 | 노련한 요리사 | 유지 | Old Chef의 성별을 단정하지 않은 기존 통일명 |
| 656 | 320 | `chef_1` | You buy things, boss? My little place just closed. Forty years, every plate fresh from the wok. Customers said it took too long. They all order takeout now. | 老板，收东西吗？我那小馆子关门了。四十年，一锅一锅现炒的，客人说等太久，都去点外卖了。 | 사장님, 물건도 사세요? 제가 하던 작은 식당이 문을 닫았어요. 40년 동안 한 접시 한 접시 직접 볶아 냈는데, 손님들은 오래 걸린다며 전부 배달 음식만 시키더군요. | 유지 | 40년 영업한 식당이 배달 때문에 폐업한 사정 |
| 657 | 321 | `chef_2` | I hear some takeout places don't even use a wok anymore. ...Never mind. Not my business. | 听说现在的外卖，有的连锅都不用。……算了，不说这个。 | 요즘 배달 가게 중에는 아예 프라이팬도 쓰지 않는 곳이 있다면서요. ……됐어요. 제가 상관할 일은 아니죠. | 유지 | 웍을 쓰지 않는 요즘 배달 업계에 대한 탄식 |
| 658 | 322 | `chef_3` | This wok's been with me forty years. Better seasoned than my own son. Name your price. | 这口锅跟了我四十年，养得比我儿子还亮。您看着给个价吧。 | 이 무쇠 팬은 40년 동안 저와 함께했어요. 제 아들보다도 더 윤이 나게 길들였죠. 적당한 값을 쳐 주세요. | **수정** | 40년 사용한 웍과 아들 비교의 중국어 말장난 |
| 659 | 323 | `item_old_wok_name` | Old Chef's Heirloom Wok | 老厨师的祖传铁锅 | 노련한 요리사의 가보 웍 | **수정** | 성별을 잘못 붙인 노련한 여주방장 명칭 수정 |
| 660 | 324 | `item_old_wok_desc` | Forty years of real wok fire. It won't cook here anymore, but it deserves a place where people can see it. | 四十年的老铁锅，锅气是真的。炒不了菜了，可总该摆在个让人看得见的地方。 | 40년 동안 진짜 불맛을 내 온 철제 웍입니다. 이곳에서 더는 요리하지 못해도 사람들이 볼 수 있는 곳에 둘 가치는 있습니다. | 유지 | 40년 불맛을 냈던 웍을 전시할 가치 |
| 661 | 325 | `chef_choice_take` | (Take the wok) All right, I'll take it. Let's talk price. | （收下铁锅）行，锅我收了，咱们谈谈价。 | (웍을 받는다) 좋아요. 받겠습니다. 가격을 이야기해 보죠. | 유지 | 웍을 사고 가격을 흥정하는 선택 |
| 662 | 326 | `chef_take_reply` | ...Just give me a fair price. | ……您给个实在价就行。 | ……적당한 값만 쳐 주세요. | 유지 | 정당한 값을 바라는 요리사 대사 |
| 663 | 327 | `chef_choice_decline` | (Decline) Keep the wok. I can't take it. | （婉拒）这锅您留着吧，我收不起。 | (거절한다) 이 웍은 가지고 계세요. 전 받지 못하겠습니다. | 유지 | 웍을 받지 않고 돌려주는 선택 |
| 664 | 328 | `chef_decline_reply` | ...Fair enough. Where the wok goes, I go. Sorry to bother you. | ……也是。锅在人在。打扰了。 | ……그렇겠죠. 웍이 있는 곳에 저도 있으니까요. 실례했습니다. | 유지 | 웍을 떠나보낼 수 없다는 심정 표현 |
| 665 | 329 | `chef_choice_aid` | (Help him) Keep the wok. Tell me what you need, and I'll send customers your way. ({0} fewer courier a day) | （援助）锅您留着。以后缺什么跟我说，我这儿的客人也往您那儿引。（每天少 {0} 位骑手） | (돕는다) 웍은 가지고 계세요. 필요한 게 있으면 말씀하시고, 제 손님도 그쪽으로 보내 드리죠. (하루 배달 기사 -{0}명) | 유지 | 손님을 보내며 보태 주는 도움의 대가 |
| 666 | 330 | `chef_aid_reply1` | ...Why would you do that? I'm just a cook. I can't repay a favor like that. | ……您这是何必呢。我一个炒菜的，欠不起人情。 | ……아이고, 이렇게까지 해 주시다니요. 저는 평생 요리만 한 늙은이인데, 그 큰 은혜를 어떻게 갚아야 할지…… | 유지 | 남의 호의를 감당하기 어렵다는 겸손한 반응 |
| 667 | 331 | `chef_aid_reply2` | Here's my card, number {0}. Call me when it suits you and I'll come by the next day. All I need is clean water and real food. No meal packs. | 名片您收着，号码 {0}。哪天方便打个电话，我第二天过来。缺的也就是干净水和正经吃的，料理包就别给了。 | 제 명함입니다. 번호는 {0}이에요. 편하실 때 연락하시면 다음 날 찾아오겠습니다. 필요한 건 깨끗한 물과 제대로 된 음식뿐이에요. 밀키트는 사양하겠습니다. | 유지 | 전화번호·익일 방문·물과 식품 필요 조건 |
| 668 | 332 | `chef_aid_visit_first` | Here I am, boss. I won't take it for free. I'll cook you something in return. | 老板，我来了。说好了，我不白拿，回头给您炒一份。 | 왔습니다, 사장님. 공짜로 받지는 않겠습니다. 대신 맛있는 요리를 해 드릴게요. | 유지 | 물품 도움을 무상으로 받지 않겠다는 대사 |
| 669 | 333 | `chef_aid_visit` | Sorry to trouble you again, boss. | 老板，又来叨扰了。 | 사장님, 또 신세 좀 지겠습니다. | 유지 | 재방문 시 손님에게 폐를 끼친다는 인사 |
| 670 | 334 | `chef_aid_need` | Still need {0} kcal of food and {1} ml of clean water (99%+ purity). Just put it on the counter. No meal packs. | 还差 {0} 千卡吃的、{1} 毫升干净水（纯度 99% 以上）。东西摆柜台上就行，料理包就别拿了。 | 아직 음식 {0}kcal와 순도 99% 이상의 깨끗한 물 {1}ml가 필요합니다. 카운터에 올려놓아 주세요. 밀키트는 받지 않습니다. | **수정** | 카운터에 놓을 음식 열량과 순도 99% 물 조건 |
| 671 | 335 | `chef_aid_angry` | That's a meal pack! Forty years at the wok, and you hand me this? ...Fine, I'll toss it for you. Still need {0} kcal and {1} ml. | 这不就是料理包吗！我炒了四十年菜，你拿这个打发我？……算了，东西我替你扔了。还差 {0} 千卡、{1} 毫升。 | 이건 밀키트잖아요! 제가 40년이나 직접 볶아 왔는데 이런 걸 주시다니요? ……됐습니다. 제가 버릴게요. 음식 {0}kcal와 물 {1}ml가 더 필요합니다. | 유지 | 밀키트 거절과 남은 열량·물의 요구 |
| 672 | 336 | `chef_aid_progress` | Got it. Still need {0} kcal and {1} ml. | 好，记着呢。还差 {0} 千卡、{1} 毫升。 | 알겠습니다. 음식 {0}kcal와 물 {1}ml가 더 필요합니다. | 유지 | 받은 수량이 반영된 잔여 열량과 물 |
| 673 | 337 | `chef_aid_done1` | That's plenty. Thank you, boss. | 够了够了，谢谢老板。 | 충분합니다. 고맙습니다, 사장님. | 유지 | 물품을 충분히 받았다는 감사 인사 |
| 674 | 338 | `chef_aid_done2` | Try this egg fried rice: day-old rice, farm eggs, a spoon of lard, ninety seconds on full flame. The real thing, not the... you know. | 这份蛋炒饭您尝尝：隔夜饭、土鸡蛋、一勺猪油，猛火一分半。正宗的，不是那个……您懂的。 | 이 달걀볶음밥을 드셔 보세요. 하룻밤 묵힌 밥, 토종닭 달걀, 돼지기름 한 숟갈을 강불에서 90초 동안 볶았죠. 진짜 볶음밥입니다. 그…… 무슨 말인지 아시죠? | **수정** | 하룻밤 묵힌 밥·토종닭 달걀·돼지기름·90초 |
| 675 | 339 | `chef_phone_hello` | Hello? Oh, it's you, boss. | 喂，哪位？哦，是老板啊。 | 여보세요? 아, 사장님이시군요. | 유지 | 주방장과의 전화 응대 첫인사 |
| 676 | 340 | `chef_phone_ask_cook` | A customer wants egg fried rice cooked right in front of him. I can't do it. Could you come and help me out? | 老师傅，有位客人点名要当面现炒的蛋炒饭，我做不来，您能出山帮我一把吗？ | 선생님, 손님이 자기 눈앞에서 갓 볶은 달걀볶음밥을 먹고 싶다는데 제가 못 하겠어요. 와서 도와주실 수 있을까요? | 유지 | 현장 볶음 주문을 돕는 부탁의 화자 구분 |
| 677 | 341 | `chef_phone_cook_ok` | Cooked in front of him? Nobody's asked for that in years. Sure, I'll bring my wok that day. | 当面炒？好久没人这么点了。行，那天我带着锅过去。 | 눈앞에서 직접 볶으라고요? 그런 주문은 몇 년 만이네요. 좋습니다. 그날 웍을 들고 가죠. | 유지 | 수년 만의 직접 볶음 요청과 웍 준비 약속 |
| 678 | 342 | `chef_phone_cook_now` | Today? All right, I'm on my way with the wok. Keep him waiting. | 今天？行，我这就带锅过去，您先稳住客人。 | 오늘이라고요? 알겠습니다. 지금 웍을 들고 가겠습니다. 손님을 잠시 붙잡아 두세요. | 유지 | 오늘 바로 웍을 들고 와 준다는 답장 |
| 679 | 343 | `chef_phone_coming` | I'll be there tomorrow as promised. Don't worry. | 说好了明天过去，您放心。 | 약속대로 내일 찾아가겠습니다. 걱정하지 마세요. | 유지 | 익일 방문 일정을 확인하는 말 |
| 680 | 344 | `chef_phone_busy` | I'm tied up these days. Call me again in {0} days. | 这两天腾不开手，{0} 天后您再打给我。 | 요즘은 손이 좀 바빠서요. {0}일 뒤에 다시 전화해 주세요. | 유지 | 바빠서 {0}일 뒤에 연락하라는 안내 |
| 681 | 345 | `chef_phone_invite` | Come by the shop tomorrow? | 老师傅，明天来店里坐坐？ | 선생님, 내일 가게에 들러 주실 수 있을까요? | 유지 | 내일 가게에 방문할 수 있는지 묻는 문구 |
| 682 | 346 | `chef_phone_invite_ok` | All right, see you tomorrow. | 好，明天我过去。 | 좋습니다. 내일 뵙죠. | 유지 | 내일 방문하겠다는 간단한 회신 |
| 683 | 347 | `chef_phone_bye` | Nothing, just checking in. | 没事，就问候一声。 | 별일은 없고, 안부만 여쭤보려고요. | 유지 | 안부만 묻는 전화의 종료 선택 |
| 684 | 348 | `chef_phone_bye_reply` | Take care, boss. | 嗯，您也保重。 | 사장님도 건강 조심하세요. | 유지 | 상대의 건강을 염려하는 인사 |
| 685 | 349 | `item_chef_card_name` | Old Chef's Card | 老师傅的名片 | 노련한 요리사의 명함 | **수정** | 구판의 여성 표기가 잔존한 명함명 통일 |
| 686 | 350 | `item_chef_card_desc` | Cream paper, a wok and a number. Open it and click the number to dial. | 米黄卡纸，一口铁锅和一串号码。点开看看，点号码就拨过去。 | 크림색 카드에 철제 웍 그림과 전화번호가 있습니다. 명함을 열어 번호를 누르면 전화를 겁니다. | 유지 | 명함의 크림색·웍 그림·전화번호 정보 |
| 687 | 351 | `item_chef_rice_name` | Authentic Golden Egg Fried Rice | 正宗黄金蛋炒饭 | 정통 황금 달걀볶음밥 | 유지 | 정통 황금 달걀볶음밥 아이템 이름 |
| 688 | 352 | `item_chef_rice_desc` | Cooked by the old chef himself. Every grain separate, gold wrapped around silver. You can smell it from down the street. | 老师傅亲手炒的，粒粒分明，金裹银。香味飘得老远。 | 노련한 요리사가 직접 볶았습니다. 밥알이 하나하나 살아 있고 황금빛 달걀이 감싸고 있습니다. 골목 너머까지 향이 퍼집니다. | **수정** | 여주방장가 오탈자와 성별 오표기 동시 교정 |
| 689 | 353 | `diner_name` | Mysterious Diner | 神秘食客 | 수수께끼의 손님 | 유지 | 정체불명의 손님을 부르는 호칭 |
| 690 | 354 | `diner_first1` | I hear your "Deep Space Wok-Fired Golden Fried Rice" is quite famous. | 听说你家的“深空猛火锅气黄金炒饭”很有名。 | 가게의 '심우주 강불 직화 황금 볶음밥'이 꽤 유명하다더군요. | 유지 | 상점의 대표 달걀볶음밥 이름 재사용 |
| 691 | 355 | `diner_first2` | I'll leave my card. Call me when you're ready. | 名片留下了。想清楚了再打给我。 | 명함을 두고 가죠. 준비되면 전화하세요. | 유지 | 명함을 맡기고 재연락해 달라는 안내 |
| 692 | 356 | `diner_phone1` | I don't eat takeout. | 我不吃外卖。 | 전 배달 음식은 먹지 않습니다. | 유지 | 현장 조리가 아닌 배달은 먹지 않는다는 조건 |
| 693 | 357 | `diner_phone2` | Tomorrow I'll come to your shop, and you'll cook me egg fried rice right in front of me. Do it well and it's {0} credits. | 明天我到你店里，你当着我的面炒一份蛋炒饭。炒得好，{0} 信用点。 | 내일 가게에 가겠습니다. 제 앞에서 달걀볶음밥을 직접 볶아 주세요. 잘 만들면 {0}크레딧을 드리죠. | 유지 | 내일 눈앞에서 볶으면 {0}크레딧 지급 |
| 694 | 358 | `diner_phone3` | If you can't, that's fine too. I just want to see if anyone still knows how to fry rice. | 炒不出来也没关系。我只是想看看，这年头还有没有人会炒饭。 | 못 만들어도 괜찮습니다. 이 시대에 아직 볶음밥을 만들 줄 아는 사람이 있는지 보고 싶을 뿐입니다. | 유지 | 볶는 사람이 남아 있는지 보려는 시험 |
| 695 | 359 | `diner_phone_tomorrow` | See you tomorrow. | 明天见。 | 내일 뵙죠. | 유지 | 내일 방문 인사 |
| 696 | 360 | `diner_phone_today` | I'm on my way. | 我在路上了。 | 지금 가는 중입니다. | 유지 | 당일 방문 진행 상태 알림 |
| 697 | 361 | `diner_challenge1` | I'm here. Where's your stove? | 我来了。灶在哪儿？ | 왔습니다. 화구는 어디 있죠? | **수정** | 원문의 stove에 맞게 화구 위치를 묻도록 수정 |
| 698 | 362 | `diner_challenge2` | So, the egg fried rice? | 蛋炒饭呢？ | 그래서 달걀볶음밥은요? | 유지 | 달걀볶음밥 제공을 재촉하는 질문 |
| 699 | 363 | `diner_choice_chef` | (Call in the old chef) | （请老师傅出马） | (노련한 요리사를 부른다) | **수정** | 구판 여주방장 오역을 요리사로 통일 |
| 700 | 364 | `diner_chef1` | (The old chef ties on his apron. The wok smokes. Egg goes in, rice follows, the spatula flies.) | （老师傅系上围裙，铁锅烧得冒青烟。蛋液下锅，米饭跟上，锅铲翻飞。） | (노련한 요리사가 앞치마를 두르고 웍을 달굽니다. 연기가 피어오르자 달걀, 이어서 밥을 넣고 주걱을 빠르게 놀립니다.) | **수정** | 요리사가 앞치마·연기·계란·밥을 조리하는 장면 |
| 701 | 365 | `diner_chef2` | Here you are. Try it. | 客人，您尝尝。 | 자, 드셔 보세요. | 유지 | 갓 볶은 음식을 권하는 짧은 대사 |
| 702 | 366 | `diner_win1` | ...Gold wrapped around silver. Every grain separate. Real wok breath. | ……金裹银，粒粒分明，还有锅气。 | ……황금빛 달걀이 밥알을 감싸고, 밥알은 하나하나 살아 있군요. 진짜 불맛까지. | 유지 | 금포은·밥알·불맛에 감탄하는 평가 |
| 703 | 367 | `diner_win2` | I haven't had this in years. The {0} credits are yours. This bowl is worth it. | 好久没吃到了。这 {0} 信用点是你们的，这碗饭值。 | 이 맛을 본 지 몇 년이나 됐는지 모르겠군요. {0}크레딧은 받으세요. 이 볶음밥이라면 그만한 가치가 있습니다. | 유지 | 잘 만든 음식 대가 {0}크레딧 지급 |
| 704 | 368 | `diner_choice_give_up` | (Give up) ...I can't cook it. | （只能放弃）……我炒不出来。 | (포기한다) ……만들 수 없어요. | 유지 | 현장 볶음을 못 한다고 인정하는 포기 선택 |
| 705 | 369 | `diner_lose` | I figured. A microwave can't give you wok breath. | 我猜也是。微波炉是炒不出锅气的。 | 그럴 줄 알았습니다. 전자레인지로는 불맛을 낼 수 없죠. | 유지 | 전자레인지가 불맛을 못 낸다는 결론 |
| 706 | 370 | `nightlog_diner_win` | The mysterious diner got his egg fried rice, cooked in front of him by the old chef, and left {0} credits. Word got around: likes +{1} (now {2}). | 神秘食客吃到了老师傅当面现炒的蛋炒饭，留下 {0} 信用点。口碑传开了：点赞 +{1}（现在 {2}）。 | 수수께끼의 손님이 노련한 요리사가 눈앞에서 볶아 준 달걀볶음밥을 먹고 {0}크레딧을 남겼습니다. 소문이 퍼져 좋아요 +{1}개, 현재 {2}개. | **수정** | 노련한 요리사 명칭과 현장 조리 보상 통일 |
| 707 | 371 | `nightlog_diner_lose` | The mysterious diner never got his fresh egg fried rice and left shaking his head. | 神秘食客没吃到现炒的蛋炒饭，摇着头走了。 | 수수께끼의 손님은 갓 볶은 달걀볶음밥을 먹지 못하고 고개를 저으며 떠났습니다. | 유지 | 볶음밥을 못 받아 떠난 손님 |
| 708 | 372 | `item_diner_card_name` | Mysterious Diner's Card | 神秘食客的名片 | 수수께끼의 손님 명함 | 유지 | 수수께끼 손님 명함 아이템 이름 |
| 709 | 373 | `item_diner_card_desc` | Black and gold, a bowl, chopsticks and a number. Open it and click the number to dial. | 黑卡烫金，一副碗筷和一串号码。点开看看，点号码就拨过去。 | 금박 장식이 있는 검은 명함에 그릇과 젓가락, 전화번호가 적혀 있습니다. 열어서 번호를 누르면 전화를 겁니다. | 유지 | 검정·금박·그릇과 젓가락·전화번호 확인 |
| 710 | 374 | `item_chef_letter_name` | A Letter from the Old Chef | 老师傅的信 | 노련한 요리사의 편지 | **수정** | 노련한 요리사 명칭으로 편지 이름 통일 |
| 711 | 375 | `item_chef_letter_desc` | My wish is fulfilled. Time to go home. | 愿望已结，不如归去。 | 소원을 이뤘으니 이제 집으로 돌아가겠습니다. | 유지 | 소원을 이뤄 귀향한다는 짧은 편지 |
| 712 | 376 | `whale_name` | Big Blue Fish | 蓝色大肥鱼 | 커다란 파란 물고기 | 유지 | 큰 파란 물고기의 호칭 |
| 713 | 377 | `whale_knock1` | Something is knocking under the counter. | 柜台下面传来咚咚的敲打声。 | 카운터 아래에서 똑똑 두드리는 소리가 납니다. | **수정** | 카운터 아래에서 노크한다는 위치 교정 |
| 714 | 378 | `whale_knock2` | The knocking gets louder. | 敲打声更响了。 | 두드리는 소리가 점점 커집니다. | 유지 | 두드리는 소리가 커지는 단계 |
| 715 | 379 | `whale_line1` | Rice rice. | 饭饭。 | 밥, 밥. | 유지 | 밥을 요구하는 짧은 한마디 |
| 716 | 380 | `whale_line2` | Rice rice! | 饭饭！ | 밥, 밥! | 유지 | 강조된 밥 요구 말투와 느낌표 |
| 717 | 381 | `item_whale_letter_name` | Pale Blue Letter | 淡蓝色的信 | 연하늘색 편지 | 유지 | 옅은 푸른색 편지 아이템명 |
| 718 | 382 | `item_whale_letter_desc` | Pale blue paper with a red seal at the bottom. Open it to read. | 淡蓝信纸，末尾盖着一枚红印。点开看看。 | 연하늘색 편지지 아래쪽에 붉은 인장이 찍혀 있습니다. 열어서 읽어 보세요. | 유지 | 편지지 색·붉은 인장·개봉 지시 |
| 719 | 383 | `whale_letter_body` | Sorry the little one bothered your shop. Her meals, repaid double: {0} credits. | 孩子顽皮，叨扰贵店。饭钱加倍奉还，{0} 信用点。 | 아이가 가게에 폐를 끼쳐 죄송합니다. 식사비는 두 배로 갚겠습니다. {0}크레딧을 받아 주세요. | 유지 | 폐 끼친 아이의 식사비를 두 배 보상 |
| 720 | 384 | `item_chute_name` | Serving Chute | 传菜滑道 | 음식 전달 미끄럼틀 | 유지 | 음식을 옮기는 미끄럼틀 아이템명 |
| 721 | 385 | `item_chute_desc` | Put one end against a microwave or air fryer and the other against the packing station or light-meal station, all in the same grid. Packing station: heated food gets bagged. Light-meal station: every dish with an uptown version gets boxed. Short on supplies, it comes out as plain food; dishes that need torching stay put so you can torch and pack them by hand. Can be rotated. | 一头挨着微波炉或空气炸锅，另一头挨着打包台或轻食操作台，都摆在同一个格子里。接打包台：热好的菜直接打成外卖。接轻食操作台：有上城版的菜一律装木盒外卖。材料不够就照常出熟食；要燎的菜不会滑走，燎完再手动打包。可以旋转。 | 한쪽 끝은 전자레인지나 에어프라이어에, 다른 끝은 포장 작업대나 가벼운 식사 작업대에 붙이고 모두 같은 격자 안에 놓으세요. 포장 작업대와 연결하면 데운 음식이 자동으로 포장됩니다. 가벼운 식사 작업대와 연결하면 상층구역용 조리법이 있는 음식이 나무 상자에 포장됩니다. 재료가 부족하면 일반 조리 음식으로 나옵니다. 토치 작업이 필요한 음식은 그대로 남아 직접 그을리고 포장할 수 있습니다. 방향을 회전할 수도 있습니다. | **수정** | 가벼운 식사 작업대 명칭과 자동 포장 조건 |
| 722 | 386 | `tip_chute_linked` | Linked: heated food slides straight over | 已接通：热好直接滑过去 | 연결됨: 데운 음식이 바로 전달됩니다. | 유지 | 연결되면 데운 식품이 바로 이동함 |
| 723 | 387 | `tip_chute_unlinked` | Not linked: each end must touch a heater and a packing or light-meal station | 没接通：两头要分别挨着加热机器和打包台 / 轻食操作台 | 연결되지 않음: 양쪽 끝이 각각 가열 기기와 포장 작업대 또는 가벼운 식사 작업대에 닿아야 합니다. | **수정** | 가벼운 식사 작업대와 가열 기기의 연결 조건 |
| 724 | 388 | `shop_chute` | Serving Chute | 传菜滑道 | 음식 전달 미끄럼틀 | 유지 | 상점 목록에 표시되는 전달 미끄럼틀 |
| 725 | 389 | `item_pantry_name` | Raw Pantry | 生食柜 | 밀키트 보관함 | 유지 | 조리 전 밀키트 전용 보관함의 이름 |
| 726 | 390 | `item_pantry_desc` | Half the size of the kitchen cabinet, for raw meal packs only. Keep it in the under-counter storage; meal packs from the order terminal go here first, then to the kitchen cabinet when it's full. | 后厨柜的一半大，只放生料理包。放在柜台下面的物品栏里，下单机送来的料理包会先进这里，满了再进后厨柜。 | 조리실 보관함의 절반 크기로, 조리 전 밀키트만 보관합니다. 카운터 아래 수납공간에 두세요. 주문 단말기에서 받은 밀키트가 먼저 여기에 들어가고, 가득 차면 조리실 보관함으로 들어갑니다. | **수정** | 저장 공간이 카운터 아래라는 위치 교정 |
| 727 | 391 | `shop_pantry` | Raw Pantry | 生食柜 | 밀키트 보관함 | 유지 | 상점에서 발주하는 보관함 품목명 |
| 728 | 392 | `courier_dismissed` | Your guts are real chubby, huh. | 你的胆子真是肥嘟嘟的。 | 정말 간이 배 밖으로 나왔네. | 유지 | 무례한 퇴장 발언의 핀잔 어투 |
| 729 | 393 | `king_name` | Delivery King | 单王 | 배달왕 | 유지 | 모든 음식을 가져가는 배달왕 호칭 |
| 730 | 394 | `king_greet` | Boss, it's your lucky day. This is a big one. | 老板，你真走运，这可是一个大单。 | 사장님, 오늘 운이 좋으시네요. 큰 주문이 들어왔거든요. | 유지 | 대량 주문을 들고 온 기사의 인사 |
| 731 | 395 | `king_order` | Every takeout bag on your counter. All of it. | 柜台上的外卖，有多少我拿多少。 | 카운터 위의 배달 음식 전부요. 남김없이 가져갈게요. | **수정** | 카운터 위의 모든 배달 음식이 대상 |
| 732 | 396 | `king_accept` | All cleared. I'm off. | 全清，走了。 | 다 챙겼습니다. 갑니다! | 유지 | 모든 음식을 받았다는 배달왕 인사 |
| 733 | 397 | `order_king` | Every takeout bag on the back counter | 后柜台所有国潮外卖袋 | 뒤쪽 카운터에 있는 모든 배달 봉투 | **수정** | 뒤쪽 카운터에 있는 모든 가방을 주문 |
| 734 | 398 | `barter_body_king` | Pickup\nEvery takeout bag on the back counter\n\nPlatform pays per dish | 取餐\n后柜台所有国潮外卖袋\n\n平台按每份的价格结算 | 음식 픽업\n뒤쪽 카운터에 있는 모든 배달 봉투\n\n플랫폼에서 음식별로 정산 | **수정** | 뒤쪽 카운터·음식별 정산·개행을 일치 |
| 735 | 399 | `item_wok_spray_name` | Wok Hei Spray | 锅气喷雾 | 불맛 스프레이 | 유지 | 불맛을 내는 스프레이 이름 |
| 736 | 400 | `item_wok_spray_desc` | One spritz and it smells freshly stir-fried. Spray a courier at the counter: every dish in that order gets a good review. {0} sprays per can. | 一喷就有“现炒”的锅气。骑手在柜台前时对他喷一下，这一单每份必定好评。一罐能喷 {0} 次。 | 한 번 뿌리면 갓 볶은 음식 냄새가 납니다. 카운터 앞의 배달 기사에게 뿌리면 해당 주문의 모든 음식이 좋은 평가를 받습니다. 한 통당 {0}회 사용 가능합니다. | **수정** | 카운터 앞 기사에게 뿌리고 전 주문 호평 |
| 737 | 401 | `spray_used` | The wok hei hits hard. This order is in the bag. | 锅气扑面而来。这一单稳了。 | 불맛이 확 퍼집니다. 이번 주문은 성공이네요. | 유지 | 스프레이 사용 시 한 줄 안내 |
| 738 | 402 | `shop_spray` | Wok hei spray ({0} uses) | 锅气喷雾（{0} 次） | 불맛 스프레이 ({0}회) | 유지 | 스프레이 재고와 사용 가능 횟수 |
| 739 | 403 | `thursday_name` | Passerby | 路人 | 지나가는 사람 | 유지 | 광란의 목요일 유행어 대사를 하는 행인 |
| 740 | 404 | `thursday_1` | It's Crazy Thursday. Send me 50. | 疯狂星期四，v我50。 | 오늘은 광란의 목요일이잖아. 50크레딧만 보내 줘. | 유지 | 광란의 목요일 50크레딧 요구 밈 |
| 741 | 405 | `thursday_2` | ...What? | ……啥？ | ……뭐라고? | 유지 | 갑작스러운 요구에 당황한 한마디 |
| 742 | 406 | `thursday_3` | Send me 50. | v我50。 | 50크레딧만 보내 줘. | 유지 | 50크레딧 요구를 반복하는 말 |
| 743 | 407 | `thursday_choice_leave` | No. Bye. | 不给。慢走。 | 싫어. 잘 가. | 유지 | 돈을 주지 않고 보내는 선택 |
| 744 | 408 | `thursday_leave_reply` | ... | …… | …… | 유지 | 대답 없는 말줄임표 유지 |
| 745 | 409 | `thursday_choice_give` | (Give him {0} credits, for some reason) | （不明所以地给他 {0} 信用点） | (영문도 모른 채 {0}크레딧을 건넨다) | 유지 | 이유도 모른 채 {0}크레딧을 주는 선택 |
| 746 | 410 | `thursday_give_reply` | You get it! Here, take this. Happy Thursday. | 懂行！这个你拿着，星期四快乐。 | 아는 사람이네! 자, 이거 받아. 즐거운 목요일 보내! | 유지 | 목요일 밈을 아는 사람에게 선물을 주는 반응 |
| 747 | 411 | `thursday_chicken_given` | A bucket of finger-lickin' chicken appears on the back counter. | 后柜台多了一桶吮指原味鸡。 | 뒤쪽 작업대에 손가락까지 핥게 되는 치킨 한 통이 나타났습니다. | 유지 | 뒤쪽에 치킨 한 통이 나타나는 상황 |
| 748 | 412 | `item_crispy_chicken_name` | Finger-Lickin' Chicken | 吮指原味鸡 | 손가락까지 핥게 되는 치킨 | 유지 | 손가락까지 핥는 치킨의 명칭 |
| 749 | 413 | `item_crispy_chicken_desc` | Fried chicken in a red-and-white bucket, still warm. A gift from a stranger on a Thursday. | 红白纸桶装的炸鸡，还是热的。一位路人在星期四送的。 | 빨간색과 흰색 종이통에 담긴 따뜻한 치킨입니다. 목요일에 낯선 사람이 준 선물입니다. | 유지 | 빨강·흰색 통과 따뜻한 치킨 선물 설명 |
| 750 | 414 | `scam_name` | Review Extortionist | 差评师 | 악성 리뷰 협박꾼 | 유지 | 협박하는 악성 리뷰 작성자의 호칭 |
| 751 | 415 | `scam_1` | Boss, look what I found in your egg fried rice. See for yourself. | 老板，你家蛋炒饭里吃出了这个。你自己看看。 | 사장님, 달걀볶음밥에서 이런 게 나왔는데요? 직접 보시죠. | 유지 | 달걀볶음밥에서 이빨이 나왔다고 주장 |
| 752 | 416 | `scam_2` | ...Teeth? There are no teeth in our rice. The corner even says "AI generated". | ……牙？我们饭里哪来的牙。这图角上还写着“AI 生成”。 | ……이빨이라고? 우리 볶음밥에 이빨이 왜 있어? 사진 구석에 'AI 생성'이라고 적혀 있잖아. | 유지 | 사진 구석의 AI 생성 표시를 지적 |
| 753 | 417 | `scam_3` | Who reads watermarks? Who will people believe, you or me? {0} credits and this never happened. Otherwise it goes on the platform, and I tell Security your shop has a body count. | 谁看水印啊？网友信你还是信我？{0} 信用点，这事就当没发生过。不给，我就发到平台上，再报给治安部，说你们店出了人命。 | 누가 워터마크까지 확인해요? 사람들은 사장님 말을 믿겠어요, 제 말을 믿겠어요? {0}크레딧만 주시면 없던 일로 해 드리죠. 안 주면 플랫폼에 올리고 치안부에는 이 가게에서 사람이 죽었다고 신고할 겁니다. | 유지 | {0}크레딧 협박 및 치안부 허위 신고 |
| 754 | 418 | `scam_choice_pay` | (Pay him {0} credits to make it go away) | （破财消灾，给他 {0} 信用点） | (일을 무마하려고 {0}크레딧을 건넨다) | 유지 | 입막음 비용을 주고 해결하는 선택지 |
| 755 | 419 | `scam_pay_reply` | Smart. Photo's deleted. Pleasure doing business. | 爽快。照片我删了，合作愉快。 | 현명하시네요. 사진은 삭제했습니다. 좋은 거래였어요. | 유지 | 돈을 받은 뒤 AI 사진을 삭제하는 대사 |
| 756 | 420 | `scam_choice_refuse` | Post it then. My shop is clean. | 要发你就发，我们店清清白白。 | 올릴 테면 올려. 우리 가게는 떳떳해. | 유지 | 허위 신고를 무시하는 선택 |
| 757 | 421 | `scam_refuse_reply` | Fine. You'll see. | 行，你等着。 | 좋아요. 두고 보세요. | 유지 | 거절당한 협박범의 보복 경고 |
| 758 | 422 | `nightlog_scam` | The extortionist posted the "teeth in the fried rice" photo: likes −{0} (now {1}). He also tipped off Security: inspectors will drop by often for the next {2} days. | 差评师把“蛋炒饭吃出人牙”的照片发上了平台：点赞 −{0}（现 {1}）。他还报给了治安部，接下来 {2} 天治安部会常来查违禁品。 | 협박범이 '달걀볶음밥에서 사람 이빨 발견' 사진을 올렸습니다. 좋아요 -{0}개 (현재 {1}개). 치안부에도 신고해 앞으로 {2}일 동안 밀수품 단속이 잦아집니다. | 유지 | 이빨 사진·좋아요 감소·밀수품 단속 기간 |
| 759 | 423 | `orange_name` | Girl in Orange | 橙色的少女 | 주황색 옷을 입은 소녀 | 유지 | 주황색 차림의 소녀 이름 표기 |
| 760 | 424 | `orange_ask` | Excuse me. Have you... seen a big blue fish around here? | 打扰了。请问……你有没有见过一条蓝色的大肥鱼？ | 실례합니다. 혹시…… 이 근처에서 커다란 파란 물고기를 보셨나요? | 유지 | 큰 파란 물고기를 봤는지 묻는 공손한 말 |
| 761 | 425 | `orange_answer_seen` | I have. She popped up from under the counter and only said "rice rice". | 见过。从柜台底下冒出来，只会说“饭饭”。 | 봤어요. 카운터 아래에서 나타나더니 '밥, 밥'이라는 말만 하더군요. | **수정** | 카운터 아래에서 등장한 물고기 목격담 |
| 762 | 426 | `orange_answer_unseen` | A big... blue fish? No, never. | 蓝色的……大肥鱼？没见过。 | 커다란…… 파란 물고기요? 아니요, 못 봤어요. | 유지 | 물고기를 보지 못했다는 부정 대답 |
| 763 | 427 | `orange_reply_seen` | So she did sneak over here. She always runs off when she smells rice. | 果然溜到这儿来了。她总是这样，闻到饭香就跑。 | 역시 여기까지 몰래 왔군요. 밥 냄새만 맡으면 이렇게 뛰쳐나간다니까요. | 유지 | 밥 냄새에 이끌려 도망치는 물고기 설명 |
| 764 | 428 | `orange_reply_unseen` | I see. She's been wandering around here lately. She comes running when she smells rice. | 这样啊。她最近总在这一带转悠，闻到饭香就会过来。 | 그렇군요. 요즘 이 근처를 돌아다니거든요. 밥 냄새가 나면 곧장 달려와요. | 유지 | 최근 이곳에 나타난 물고기 습관 설명 |
| 765 | 429 | `orange_card` | If you see her again, please call this number. I'll come right away. | 下次再看到她，请打这个电话告诉我。我马上过来。 | 다시 보시면 이 번호로 전화해 주세요. 바로 오겠습니다. | 유지 | 재목격 시 전화를 달라는 요청 |
| 766 | 430 | `item_orange_card_name` | Orange Card | 橙色的名片 | 주황색 명함 | 유지 | 주황색 명함 아이템명 |
| 767 | 431 | `item_orange_card_desc` | An off-white card with an orange starburst flower in the middle. Open it to look. | 米白卡片，正中一朵橙色的放射花。点开看看。 | 미색 명함 한가운데 주황색 별 모양 꽃이 그려져 있습니다. 열어서 확인해 보세요. | 유지 | 명함의 미색과 주황 꽃 그림 특징 |
| 768 | 432 | `orange_phone_coming` | She's at your place? I'm on my way! Don't let her run off. | 她在你那儿？我马上到！别让她跑了。 | 그 애가 거기 있다고요? 바로 갈게요! 도망가지 못하게 해 주세요. | 유지 | 물고기를 발견했다는 전화의 긴급 반응 |
| 769 | 433 | `orange_phone_not_here` | She's not with you today, is she? Call me when you see her again. | 她今天不在你那儿吧？再看到她就打给我。 | 오늘은 그 애가 거기 없죠? 다시 보시면 연락해 주세요. | 유지 | 오늘은 없으면 다음에 다시 연락 요청 |
| 770 | 434 | `orange_phone_done` | She's behaving at home now. Thank you. | 她在家很乖，谢谢你。 | 집에 돌아와서 얌전히 지내고 있어요. 고마워요. | 유지 | 집으로 돌아온 뒤 얌전하다는 감사 |
| 771 | 435 | `orange_fetch1` | Found you, little fish. Let's go home. | 找到你了，小肥鱼。跟我回家。 | 찾았다, 작은 물고기야. 이제 집에 가자. | 유지 | 작은 물고기를 집으로 데려가는 말 |
| 772 | 436 | `orange_fetch2` | Sorry she caused you trouble. I'll keep a closer eye on her. | 她给你添麻烦了。以后我会看好她的。 | 폐를 끼쳐 죄송해요. 앞으로는 더 잘 지켜볼게요. | 유지 | 소동에 사과하며 잘 돌보겠다는 약속 |
| 773 | 437 | `orange_fetch3` | Please take this. A small thank-you. | 这个送给你，一点心意。 | 이걸 받아 주세요. 작은 감사의 표시예요. | 유지 | 감사 표시로 선물을 건네는 말 |
| 774 | 438 | `item_clawd_name` | Clawd Keepsake | Clawd 纪念品 | Clawd 기념품 | 유지 | Clawd 기념품의 고유 철자 보존 |
| 775 | 439 | `item_clawd_desc` | A little terracotta block critter on a wooden stand. A thank-you gift from the girl in orange. You can now order Clawd cookies from the order terminal. | 赤陶色的小方块怪，站在木底座上。橙色的少女送的谢礼。现在你可以在下单机订购 Clawd 饼干了。 | 나무 받침대에 올려놓은 테라코타색 네모난 작은 생물 모형입니다. 주황색 소녀가 준 감사 선물입니다. 이제 주문 단말기에서 Clawd 쿠키를 주문할 수 있습니다. | 유지 | 테라코타 모형·쿠키 추가 주문 해금 |
| 776 | 440 | `item_shop_panel_name` | Merchant Console | 商家后台 | 가맹점 관리 단말기 | 유지 | 플랫폼 가맹점용 태블릿 아이템명 |
| 777 | 441 | `item_shop_panel_desc` | The delivery platform's merchant tablet. Open it to check likes, couriers and the last 7 nights of likes. | 外卖平台发的商家平板。点开看点赞、骑手和近 7 晚的点赞走势。 | 배달 플랫폼에서 지급한 가맹점용 태블릿입니다. 열면 좋아요 수, 배달 기사 수, 최근 7일간의 좋아요 변화를 확인할 수 있습니다. | 유지 | 좋아요·기사·지난 7일 변화 화면 설명 |
| 778 | 442 | `panel_title` | Merchant Console | 商家后台 | 가맹점 관리 단말기 | 유지 | 관리 단말기의 화면 제목 |
| 779 | 443 | `panel_likes` | {0} likes | {0} 赞 | 좋아요 {0}개 | 유지 | 좋아요 현재 수 {0} |
| 780 | 444 | `panel_chart_title` | Likes, last {0} nights | 近 {0} 晚点赞 | 최근 {0}일간 좋아요 | 유지 | 최근 {0}일 좋아요 추이 제목 |
| 781 | 445 | `panel_chart_max` | Peak {0} | 最高 {0} | 최고 {0} | 유지 | 최고 좋아요 수치 표시 |
| 782 | 446 | `panel_chart_empty` | Recording starts tonight | 今晚结算后开始记录 | 오늘 밤 정산부터 기록을 시작합니다 | 유지 | 기록이 오늘 밤 정산 후 시작되는 조건 |
| 783 | 447 | `panel_k0` | Open for | 开业 | 영업 기간 | 유지 | 영업 일수를 표시하는 필드명 |
| 784 | 448 | `panel_k1` | Couriers/day | 每天骑手 | 하루 배달 기사 | 유지 | 하루 방문하는 배달 기사 수 |
| 785 | 449 | `panel_k2` | Max per order | 每单最多 | 주문당 최대 수량 | 유지 | 주문 한 건당 최대 수량 |
| 786 | 450 | `panel_k3` | Dishes listed | 上架菜品 | 등록 메뉴 | 유지 | 온라인 메뉴판에 올라간 음식 종류 |
| 787 | 451 | `panel_k4` | Regulars | 回头客 | 단골손님 | 유지 | 재주문을 하는 단골손님 수 |
| 788 | 452 | `panel_k5` | Next courier | 下一位骑手 | 다음 배달 기사 해금 | 유지 | 다음 배달 기사 해금까지 남은 수치 |
| 789 | 453 | `panel_k6` | Delivered | 累计送出 | 누적 배달 | 유지 | 누적 배달한 주문량 |
| 790 | 454 | `panel_k7` | Earned | 累计收入 | 누적 수입 | 유지 | 지금까지 벌어들인 누적 수입 |
| 791 | 455 | `panel_v_day` | {0} days | 第 {0} 天 | {0}일 차 | 유지 | 제 {0}일차의 운영 현황 |
| 792 | 456 | `panel_v_couriers` | ~{0} | 约 {0} 位 | 약 {0}명 | 유지 | 약 {0}명으로 추정되는 기사 수 |
| 793 | 457 | `panel_v_per_order` | {0} | {0} 份 | {0}인분 | 유지 | 주문당 {0}인분의 제한 |
| 794 | 458 | `panel_v_listed` | {0} | {0} 样 | {0}종 | 유지 | 판매 메뉴 종류 수 {0} |
| 795 | 459 | `panel_v_regulars` | {0} | {0} 位 | {0}명 | 유지 | 단골손님 {0}명 |
| 796 | 460 | `panel_v_next` | {0} likes to go | 还差 {0} 赞 | 좋아요 {0}개 더 필요 | 유지 | 배달 기사 해금까지 좋아요 {0}개 |
| 797 | 461 | `panel_v_full` | Maxed | 已满 | 최대치 도달 | 유지 | 해금 가능한 기사 수가 최대치에 도달 |
| 798 | 462 | `panel_v_delivered` | {0} orders | {0} 单 | 주문 {0}건 | 유지 | 누적 주문 {0}건 |
| 799 | 463 | `panel_v_earned` | {0} credits | {0} 信用点 | {0}크레딧 | 유지 | 누적 정산액 {0}크레딧 |
| 800 | 464 | `item_clawd_dough_name` | Clawd Cookie Dough | Clawd 饼干生坯 | Clawd 쿠키 반죽 | 유지 | Clawd 쿠키 반죽 아이템 이름 |
| 801 | 465 | `item_clawd_dough_desc` | Raw cookie dough in the shape of Clawd. Bake it in the air fryer to get a Clawd cookie. | Clawd 形状的生饼干坯。放进空气炸锅烤一下就是 Clawd 饼干。 | Clawd 모양의 굽지 않은 쿠키 반죽입니다. 에어프라이어로 구우면 Clawd 쿠키가 됩니다. | 유지 | 에어프라이어에서 굽는 Clawd 모양 반죽 |
| 802 | 466 | `item_clawd_cookie_name` | Clawd Cookie | Clawd 饼干 | Clawd 쿠키 | 유지 | 구운 Clawd 쿠키의 명칭 |
| 803 | 467 | `item_clawd_cookie_desc` | A butter cookie baked in the shape of Clawd. Drag it onto a wooden-box takeout to tuck it in: that box is guaranteed a like and a good review. | 烤成 Clawd 样子的黄油饼干。拖到木盒外卖上放进盒里，这一盒必定点赞好评。 | Clawd 모양으로 구운 버터 쿠키입니다. 나무 상자 도시락에 끌어다 놓으면 쿠키가 들어가며, 그 도시락은 반드시 좋아요와 좋은 평가를 받습니다. | 유지 | 나무 상자에 넣으면 반드시 좋아요·호평 |
| 804 | 468 | `cookie_used` | Clawd cookie tucked in. This box is in the bag. | Clawd 饼干放进去了。这一盒稳了。 | Clawd 쿠키를 넣었습니다. 이 도시락은 문제없겠네요. | 유지 | 쿠키를 넣어 성공했다는 안내 |
| 805 | 469 | `tip_cookie` | A Clawd cookie is tucked inside | 盒里有一块 Clawd 饼干 | 상자 안에 Clawd 쿠키가 들어 있습니다. | 유지 | 상자 속에 숨긴 쿠키 표시 |
| 806 | 470 | `shop_cookie` | Clawd cookie dough (once a day) | Clawd 饼干生坯（每天限订一次） | Clawd 쿠키 반죽 (하루 한 번 주문 가능) | 유지 | 하루에 한 번만 주문 가능한 반죽 |
| 807 | 471 | `terminal_today_done` | Ordered today | 今日已订 | 오늘 주문 완료 | 유지 | 오늘 이미 주문했다는 버튼 상태 |
| 808 | 472 | `rat_premade_tainted` | Too much premade food: its meat will be trash grade | 吃了太多预制菜：宰出来的肉是垃圾级 | 밀키트 과다 섭취: 이 동물에게서 얻는 고기는 쓰레기 등급이 됩니다. | 유지 | 밀키트를 과식한 동물의 고기 품질 하락 |
| 809 | 473 | `dispenser_premade` | Of which premade food: {0} kcal (counted toward whatever eats it) | 其中预制料理 {0} kcal（按比例记到吃它的鱼和老鼠身上） | 이 중 미리 조리된 음식: {0}kcal (섭취한 동물에게 반영됨) | 유지 | {0}kcal 미리 조리 식품이 섭취 동물에 귀속 |

## NPC Girls (3835292703) — 1~91
| 전체 순번 | 모드 순번 | 키 | 영어 원문 | 중국어 원문 | 최종 한국어 | 판정 | 개별 검수 메모 |
|---:|---:|---|---|---|---|---|---|
| 810 | 1 | `dialog_wild_network_1` | Listen up, darling. Let big sister walk you through how things work down here. | 听好了，亲爱的小邻居，姐姐来跟你聊聊这里的规矩。 | 잘 들어, 귀여운 이웃. 이 동네가 어떻게 돌아가는지 내가 알려 줄게. | 유지 | 와일드의 친근한 누님 말투와 하층구역 규칙 소개 |
| 811 | 2 | `dialog_wild_network_2` | This little sector of ours is a delicate ecosystem, you see. | 咱们这个小小的下层区，可是一套自成一体的生态系统呢。 | 이 작은 하층구역도 나름대로 균형이 잡힌 생태계거든. | 유지 | 하층구역 생태계의 미묘한 균형 설명 |
| 812 | 3 | `dialog_wild_network_3` | If someone takes too much without giving a single thing back... | 要是光想着从骨头里榨油，却连一滴水都舍不得回馈…… | 받아 가기만 하고 아무것도 돌려주지 않으려 든다면…… | 유지 | 기여하지 않고 이익만 취하면 생길 결과 |
| 813 | 4 | `dialog_wild_network_4` | The balance shatters, and the whole house of cards comes crashing down. | 平衡一旦碎掉，这儿的一切可就全砸了哦。 | 그 균형이 깨지는 순간, 이 동네가 송두리째 무너질 거야. | 유지 | 균형 붕괴 시 지역 전체가 무너지는 비유 |
| 814 | 5 | `dialog_wild_network_5` | Play your cards right, and I'd love to do you some favors. In return... | 只要你乖乖听话，我很乐意关照关照你，至于回报嘛…… | 네가 처신만 잘하면 여러모로 도와줄 수 있지. 대신…… | 유지 | 도움 제공과 대가 요구의 연결 |
| 815 | 6 | `dialog_wild_network_6` | You look out for my Lower Levels folks from time to time, deal? | 有空的时候，多替姐姐顾照着点下层区的街坊们，嗯？ | 가끔 하층구역 사람들을 좀 챙겨 줘. 약속할 수 있지? | 유지 | 하층구역 주민을 돌보라는 부탁 |
| 816 | 7 | `dialog_wild_network_7` | Here, take my personal card. Keep it close. | 这是我的私人名片，收好了。 | 자, 내 개인 명함이야. 잘 간직해. | 유지 | 개인 명함을 건네는 행동 |
| 817 | 8 | `dialog_wild_network_8` | Whenever you're in over your head, just give me a ring. | 遇到摆不平的麻烦，随时给姐姐打电话。 | 혼자 감당하기 힘든 일이 생기면 언제든 전화해. | 유지 | 어려울 때 전화하라는 조언 |
| 818 | 9 | `dialog_wild_upgrade_reminder_1` | Well hello there, shopkeeper. It's been a while since I heard your voice. | 嗨呀，我的大店长，有一阵子没听到你的声音了呢。 | 어머, 우리 사장님 아니신가! 한동안 목소리를 못 들었네. | 유지 | 연락이 뜸했던 가게 주인에게 안부 |
| 819 | 10 | `dialog_wild_upgrade_reminder_2` | Dr. Jackson was just pouting to me that you haven't dropped by his place either. | 杰克逊博士前两天还在跟我抱怨，说你最近都没去瞧过他。 | 잭슨 박사도 요즘 네가 통 들르지 않는다며 투덜대더라. | 유지 | 잭슨 박사가 방문하지 않아 서운해함 |
| 820 | 11 | `dialog_wild_upgrade_reminder_3` | Just a gentle reminder, darling: whenever you need a hand, I'm right here. | 只是个温柔的善意提醒——缺什么了尽管开口，有我在呢。 | 그냥 살짝 일러 주는 거야. 도움이 필요하면 언제든 날 찾아. | 유지 | 필요하면 자신에게 도움을 요청하라는 말 |
| 821 | 12 | `dialog_wild_upgrade_reminder_4` | Come closer. Take a look at the little treats I've put together for you. | 来，凑近点，看看姐姐为你备下了什么好东西。 | 자, 가까이 와 봐. 널 위해 준비한 좋은 물건들이 있거든. | 유지 | 미리 준비한 물건을 보여 주려는 유도 |
| 822 | 13 | `dialog_wild_bailout_1` | Oh my... word on the street is you've hit a bit of a rough patch. | 哎呀呀……听说你最近手头紧得快揭不开锅了？ | 어머나…… 요즘 자금 사정이 영 좋지 않다는 소문을 들었어. | 유지 | 자금 사정이 나쁘다는 소문 |
| 823 | 14 | `dialog_wild_bailout_2` | Charming local shops like yours are the beating heart of our community, you know. | 像你这么招人喜欢的本地小店，可是咱们社区的心头肉呢。 | 너처럼 정겨운 동네 가게가 우리 공동체의 버팀목인걸. | 유지 | 동네 가게를 공동체의 중심으로 칭찬 |
| 824 | 15 | `dialog_wild_bailout_3` | Chin up, sweetheart. I'm throwing a few favors your way—on the house. | 别哭丧着脸啦，这批周转姐姐无偿赞助你，不收利息。 | 기운 내, 귀염둥이. 이번엔 내가 무상으로 좀 도와줄게. 이자도 받지 않을 거야. | **수정** | 무상 지원과 무이자 조건을 함께 복원 |
| 825 | 16 | `dialog_wild_bailout_4` | Here. Don't let me down now, okay? | 拿去吧，别让我失望哦。 | 자, 받아. 날 실망시키진 말아 줘. | 유지 | 물건을 건네고 실망시키지 말라는 부탁 |
| 826 | 17 | `dialog_wild_network_reminder_1` | Hey there, neighbor! Did you forget how to dial, or are you just playing hard to get? | 你好呀，邻居！我看你的拨号盘都快生锈了吧，怎么一个电话都不打？ | 안녕, 이웃! 전화 거는 법을 잊은 거야, 아니면 일부러 밀당하는 거야? | 유지 | 오랫동안 전화하지 않은 점을 장난스럽게 책망 |
| 827 | 18 | `dialog_wild_network_reminder_2` | Ring me up when you get a second. I don't bite... much. | 抽个空打给我嘛，我又不会吃了你。 | 시간 날 때 한번 전화해. 잡아먹지는 않을 테니까…… 아마도? | 유지 | 전화를 권하며 물지 않는다는 농담 |
| 828 | 19 | `dialog_wild_network_reminder_3` | My rolodex is packed with all sorts of delightful, capable folks. | 姐姐手里的人脉，可比你想象的要神通广大得多。 | 내 인맥에는 온갖 재주를 가진 사람이 다 있어. 네가 생각하는 것보다 훨씬 넓지. | **수정** | 인맥이 다양하고 능력자들이 많다는 비유 개선 |
| 829 | 20 | `dialog_wild_network_reminder_4` | Farming, purifying water, distilling fine moonshine... just say the word, and I'll make the introductions. | 无论是种地净水，还是偷酿私酒……只要你想学，我都能替你牵线搭桥。 | 농사부터 정수, 밀주 증류까지…… 말만 하면 적당한 사람을 소개해 줄게. | 유지 | 농사·정수·밀주 증류 분야 인맥 소개 |
| 830 | 21 | `dialog_wild_network_reminder_5` | Just punch my number whenever you're ready, darling. | 想好了就按号码呼我，我随时恭候。 | 마음이 정해지면 언제든 내 번호로 연락해. | 유지 | 준비되면 전화를 걸라는 말 |
| 831 | 22 | `dialog_wild_cheng_1` | Ah... hello there. | 唉……你来了。 | 아…… 왔구나. | 유지 | 와일드의 근심 섞인 첫인사 |
| 832 | 23 | `dialog_wild_cheng_2` | I'm afraid I come with some rather heartbreaking news. | 姐姐这儿……有个挺让人难受的坏消息。 | 안타까운 소식을 전해야 할 것 같아. | 유지 | 안타까운 소식이 있다는 예고 |
| 833 | 24 | `dialog_wild_cheng_3` | You were quite fond of Grandpa Cheng, weren't you? | 你一直挺敬重程大爷的，对吧？ | 정 할아버지를 참 좋아했지? | 유지 | 程 할아버지를 정 할아버지로 통일 |
| 834 | 25 | `dialog_wild_cheng_4` | Last night... the old man slipped away peacefully in his sleep. | 昨晚……老人家在睡梦里安详地走了。 | 어젯밤…… 할아버지가 잠결에 조용히 세상을 떠나셨어. | 유지 | 수면 중 평온하게 별세했다는 소식 |
| 835 | 26 | `dialog_wild_cheng_5` | He had a golden heart. A true cornerstone of our Lower Levels family. | 他是个不可多得的好人，一直都是下层区的定海神针啊。 | 정말 마음씨 좋은 분이었어. 하층구역의 든든한 버팀목이셨지. | 유지 | 고인의 선함과 지역 사회 기여 |
| 836 | 27 | `dialog_wild_cheng_6` | He left something behind. I want you to have it, to remember him by. | 这是他留下的一点遗物，交给你保管，权当是个念想吧。 | 유품을 하나 남기셨는데, 네가 기념으로 간직해 줬으면 해. | 유지 | 고인이 남긴 유품을 기념으로 전달 |
| 837 | 28 | `dialog_wild_deed_1` | Well, well, look at my star player! Good news travels fast. | 哟，我的大功臣！消息传得可真快呢。 | 어머, 우리 일등 공신 아니야! 좋은 소문은 정말 빨리 퍼진다니까. | 유지 | 와일드가 상인을 일등 공신으로 부르는 인사 |
| 838 | 29 | `dialog_wild_deed_2` | This whole place is officially in your hands now. Congratulations! That's quite a feat down here. | 这间地盘现在彻底姓你的名了，恭喜呀！在下层区这可真了不起！ | 이제 이곳이 정식으로 네 소유가 됐어. 축하해! 하층구역에서 정말 대단한 일을 해냈네. | 유지 | 점포 소유권을 정식 취득한 축하 |
| 839 | 30 | `dialog_wild_deed_3` | Since you've proven yourself a truly capable merchant, I've got something exclusive for you. | 既然你已经证明了自己的本事，姐姐也该拿出点真正的大生意了。 | 유능한 상인이라는 걸 증명했으니, 특별한 거래를 하나 소개해 줄게. | 유지 | 유능한 상인에게 특별 거래를 제안 |
| 840 | 31 | `dialog_wild_deed_4` | Ever heard whispers about... "Hypertech"? | 听说过……“超科技”吗？ | 혹시…… '하이퍼테크'라고 들어 봤어? | 유지 | 하이퍼테크라는 특수 물품 용어 유지 |
| 841 | 32 | `dialog_wild_deed_5` | Miraculous stuff, darling. Far beyond anything the crude factories on this station could ever dream of making. | 那可是真正的好宝贝，根本不是这个破空间站的破工业能造出来的神物。 | 기적 같은 물건이지. 이 우주 정거장의 조잡한 공장에서는 꿈도 못 꿀 기술이야. | 유지 | 상층 제조업보다 뛰어난 기술의 가치 |
| 842 | 33 | `dialog_wild_deed_6` | Even the high-and-mighty up in the Upper Levels would claw each other's eyes out for it. | 就连上头那些高高在上的贵族老爷，想沾一口都得求爷爷告奶奶。 | 상층구역의 잘난 귀족들조차 서로 차지하려고 싸울걸? | 유지 | 상층구역 귀족도 탐내는 물건임을 강조 |
| 843 | 34 | `dialog_wild_deed_7` | I know a smuggler who dances in those shadows. I can make an introduction—for a rather handsome fee, of course. | 我认得一个专走这条暗线的走私贩。引荐费嘛……当然不便宜哦。 | 그런 물건을 몰래 거래하는 밀수업자를 알고 있어. 소개비는…… 물론 꽤 비싸고. | 유지 | 밀수업자 소개에 상당한 대가가 필요 |
| 844 | 35 | `dialog_wild_deed_8` | But a fair warning from me first: every single step down that road will cost a pretty penny. | 别怪姐姐没先给你打预防针：碰那种尖端货，每一步都是烧钱的无底洞。 | 하지만 미리 경고할게. 그쪽에 발을 들이는 순간 매 단계마다 큰돈이 들어. | 유지 | 한 단계씩 큰돈이 드는 사업 경고 |
| 845 | 36 | `dialog_wild_deed_9` | Once your nerves and your wallet are ready, give me a call. | 准备好钱包和胆量了，就给姐姐打电话吧。 | 배짱도 지갑도 준비되면 내게 전화해. | 유지 | 배짱과 자금 준비 뒤 연락하라는 말 |
| 846 | 37 | `PhoneDialogue/browse_wild_service` | [Browse Ms. Wild's Services] | [浏览王尔德女士的业务列表] | [와일드 여사의 서비스 보기] | 유지 | 와일드 여사의 서비스 둘러보기 선택지 |
| 847 | 38 | `PhoneDialogue/wild1` | Hello, this is Ms. Wild's private office... | 您好，这里是王尔德女士的专属事务所…… | 여보세요. 와일드 여사의 개인 사무실입니다…… | 유지 | 개인 사무실의 전화 응대 인사 |
| 848 | 39 | `PhoneDialogue/wild2` | How may we be of service to you today, dear? | 请问今天需要姐姐为您做些什么呢？ | 오늘은 어떤 도움을 드릴까요? | 유지 | 오늘 필요한 서비스를 묻는 말투 |
| 849 | 40 | `PhoneDialogue/wild3` | Here is what we can arrange for you at the moment. | 以下是目前能为您行个方便的特别服务清单。 | 현재 이용하실 수 있는 특별 서비스를 안내해 드리겠습니다. | 유지 | 현재 받을 수 있는 서비스 목록 |
| 850 | 41 | `dialog_lun_visit_1` | Ahem... Citizen, attention! This is Officer Lun from the Security Department! | 咳咳……公民，立、立正！我是治安部的伦警官！ | 크흠…… 시, 시민! 차, 차렷! 치안부 소속 룬 경관입니다! | **수정** | 룬 경관의 더듬는 차렷 명령과 신분 소개 |
| 851 | 42 | `dialog_lun_visit_2` | I am currently on a very serious, top-priority sector patrol...! | 我今天正在进行非常严肃、非常重要的街区巡逻……！ | 지금 아주 진지하고 중요한 관할 구역 순찰을 하는 중인데……! | 유지 | 중요한 관할 순찰이라는 허세 표현 |
| 852 | 43 | `dialog_lun_visit_3` | The thing is... um... I left my lunchbox at home in a rush... Do you have anything edible here? whimpers | 那个……就是说……我出门太急忘带便当盒了……你这里有多余能吃的东西吗？呜…… | 그게…… 어…… 급하게 나오느라 도시락을 깜빡해서요…… 혹시 여기 먹을 만한 게 있나요? 훌쩍…… | 유지 | 도시락을 잊고 먹을 것을 달라는 말 |
| 853 | 44 | `dialog_lun_visit_accept` | Wah, a lifesaver! I-I'll accept this under official requisition orders...! | 呜哇，救命口粮！那、那我就以治安征用的名义收下了……！ | 우와, 살았다! 이, 이건 정식 징발 명령에 따라 받는 거예요……! | 유지 | 식품을 정식 징발로 받는다는 변명 |
| 854 | 45 | `dialog_lun_visit_wrong_item` | Dummy! My stomach is growling here, that's not something you can eat...! | 笨蛋！肚子都在咕咕叫了，这根本不是能吃的东西嘛……！ | 바보야! 배고파 죽겠는데 이건 먹을 수 있는 게 아니잖아……! | 유지 | 먹을 수 없는 물건을 줬다는 불평 |
| 855 | 46 | `dialog_lun_visit_right_item` | nom nom... Hmph, good citizen. I'll let your minor infractions slide today! | 呼……嚼嚼……算你识相啦，本警官今天就先放过你的小违章！ | 냠냠…… 흥, 모범 시민이군요. 오늘은 사소한 위반쯤 눈감아 드리죠! | 유지 | 음식을 받은 뒤 경미한 위반 묵인 |
| 856 | 47 | `dialog_lun_shot_1` | Help, somebody help meee! Boss! Give me some neural painkillers or whatever you've got, right now! | 救命、救命啊呜呜呜！店长！快给我神经止痛针，随便什么药都行，现在就要！ | 살려 주세요! 누가 좀 도와줘요! 사장님! 신경 진통제든 뭐든 당장 주세요! | 유지 | 신경 진통제가 필요하다는 긴급 요청 |
| 857 | 48 | `dialog_lun_shot_2` | It hurts so bad...! Some jerk in a grey jumpsuit jumped me in the alley...! | 疼死了……一个穿灰连体服的混蛋居然在巷子转角偷袭我……！ | 아파 죽겠어……! 회색 작업복 입은 나쁜 놈이 골목 모퉁이에서 기습했어요……! | **수정** | 회색 작업복 습격자가 골목 모퉁이에서 기습 |
| 858 | 49 | `dialog_lun_shot_3` | I don't care if it's black-market contraband! Just shoot me with the strongest dose you have, please... it hurts so much...! | 我不管那是不是管制药剂了！快给我来一针药效最猛的，呜……眼泪都止不住了……！ | 암시장 밀수품이어도 상관없어요! 제일 센 걸로 한 대만 놔 주세요…… 너무 아파요……! | 유지 | 통제 약물이라도 가장 강한 주사 요구 |
| 859 | 50 | `dialog_lun_shot_4` | Shh... this is completely off the books! Internal Affairs can NEVER find out about this, got it...?! | 嘘……这笔交易不入档案，绝对绝对不能让内务督察部知道……！ | 쉿…… 이건 완전히 비공식이에요! 감찰부에는 절대 알려지면 안 돼요, 알겠죠……?! | 유지 | 감찰부에는 비공식 거래를 숨기는 조건 |
| 860 | 51 | `dialog_lun_shot_accept` | Yes... that's it... Th-Thank you so much, sniff... | 对……就是这个……太感谢你了呜…… | 맞아요…… 그거예요…… 저, 정말 고마워요. 훌쩍…… | 유지 | 진통 주사를 받고 고마워하는 반응 |
| 861 | 52 | `dialog_lun_shot_wrong_item` | Wah! Not that! I'm seeing stars here, give me something that stops the pain! | 呜！不是这个！我都快疼昏过去了，给我能止痛的神经舒缓剂啊！ | 으악! 그거 아니에요! 눈앞에 별이 보인다니까요! 통증을 없애 주는 걸 달라고요! | 유지 | 진통제 아닌 물건을 준 데 대한 항의 |
| 862 | 53 | `dialog_lun_shot_contraband` | Eep! Who cares if it's unregistered contraband, as long as it works! Give it here! | 呀啊！管他是几级黑市管制针，现在能保命就行，快拿来！ | 히익! 미등록 밀수품이면 어때요, 효과만 있으면 됐지! 빨리 주세요! | 유지 | 미등록 밀수품이라도 약이면 수락 |
| 863 | 54 | `dialog_lun_shot_right_item` | sniff... Ahhh... I think the feeling is coming back... I almost became a line-of-duty casualty, geez...! | 呜……哈啊……感觉稍微活过来了……差一点就因公殉职了啦…… | 훌쩍…… 아아…… 이제 좀 감각이 돌아오는 것 같아…… 하마터면 순직 처리될 뻔했네……! | 유지 | 약을 맞고 다시 감각이 돌아온 상태 |
| 864 | 55 | `dialog_lun_crim_1` | Um... Boss... I, uh, "accidentally" stumbled across some gear during patrol... | 那个……店长……我、我巡逻时“偶然”捡到了一些物资…… | 저기…… 사장님…… 순찰하다가 장비를 좀…… '우연히' 발견했거든요…… | 유지 | 순찰 중 물자를 우연히 주웠다고 둘러대기 |
| 865 | 56 | `dialog_lun_crim_2` | A-Are you buying? C'mon, give a girl a decent price, we're buddies, right...? | 你……你收不收二手货呀？看在熟人的份上给个好价钱嘛…… | 사, 사실래요? 에이, 우리 친한 사이잖아요. 괜찮은 가격 좀 쳐 줘요……? | 유지 | 친분을 내세워 비싼 매입 가격을 요구 |
| 866 | 57 | `dialog_lun_crim_accept` | Yay! D-Deal! Remember, you never got this from me, okay?! | 太好了！成、成交！千万别跟别人说是我卖的哦！ | 야호! 거, 거래 성립! 이 물건 어디서 났는지는 절대 말하면 안 돼요, 알았죠?! | 유지 | 비밀리에 판매가 성사된 감탄 |
| 867 | 58 | `dialog_lun_crim_arrest_client` | Eh? Wait... wait a sec! What are you guys doing here?! | 诶？等等……等一下！你们怎么会在这里？！ | 어? 잠깐만…… 잠깐! 당신들 여기서 뭐 하는 거예요?! | 유지 | 관련자의 갑작스러운 체포 상황 |
| 868 | 59 | `dialog_lun_crim_arrest_sec_1` | Hey! That's the scumbag who ambushed Officer Lun! | 喂！就是这家伙偷袭了伦警官！ | 저기 있다! 룬 경관을 습격한 놈이야! | 유지 | 룬 경관 습격범을 특정하는 경고 |
| 869 | 60 | `dialog_lun_crim_arrest_sec_2` | Grab him! Don't let him get away! | 抓住他！别让他跑了！ | 잡아! 도망치게 두지 마! | 유지 | 범인을 잡고 도주를 막으라는 지시 |
| 870 | 61 | `dialog_lun_crim_arrest_client_2` | Wahhh?! This is the absolute worst! | 呜哇哇？！糟透了——！ | 으아아?! 이건 최악이야! | 유지 | 돌발 체포에 놀란 룬 경관 |
| 871 | 62 | `dialog_lun_duty_1` | Heheh, salute! See that? Officer Lun is officially back on duty, fully recovered! | 哼哼，敬礼！看到没有，本警官已经满血复活归队啦！ | 헤헤, 경례! 보이시죠? 룬 경관, 완전히 회복해서 공식 복귀했습니다! | 유지 | 회복 후 순찰에 복귀했다는 자랑 |
| 872 | 63 | `dialog_lun_duty_2` | That was just a little paid vacation in the Medbay, that's all... I totally didn't cry for my mommy! | 之前不过是在医疗舱稍微享受了一场带薪度假而已……才没有哭着喊妈妈呢！ | 의무실에서 잠깐 유급 휴가를 즐긴 것뿐이라고요…… 엄마 찾으면서 운 적은 절대 없어요! | 유지 | 의무실의 유급 휴가라며 울지 않았다고 변명 |
| 873 | 64 | `dialog_lun_duty_3` | Don't panic, don't panic! This is just a completely normal routine check-in. | 别紧张，别紧张！今天只是非常正规的例行巡检啦。 | 놀라지 마세요, 놀라지 마세요! 그냥 지극히 평범한 정기 순찰이에요. | 유지 | 평범한 정기 점검이라는 주장 |
| 874 | 65 | `dialog_lun_duty_4` | Since I'm already here... could you slip me some of those heavy-duty painkillers? | 那个……既然我都顺路过来了，顺便给我拿两盒强效神经止痛片呗？ | 여기까지 온 김에…… 그 강력한 신경 진통제 두 상자만 더 받을 수 있을까요? | **수정** | 신경 진통제 두 상자를 요구한 수량 복구 |
| 875 | 66 | `dialog_lun_duty_5` | The extra-strength kind, please... The stitches still sting sometimes, you know... | 要止痛效果最猛的那种……伤口偶尔还是会隐隐作痛嘛…… | 제일 센 걸로요…… 아직도 가끔 봉합한 자리가 쑤시거든요…… | 유지 | 봉합한 상처의 통증으로 강한 약 필요 |
| 876 | 67 | `dialog_lun_duty_6` | Just for the record! It's purely for emergency reserves! I am definitely not getting dependent on them, okay?! | 声明一下！我只是放在口袋里备着应急防身！绝对没有依赖性哦！ | 확실히 해 두는데! 어디까지나 비상용이에요! 절대로 약에 의존하는 거 아니라고요, 알겠죠?! | 유지 | 의존성을 부정하면서 비상용이라고 강조 |
| 877 | 68 | `dialog_lun_duty_accept` | Hehe, deal! Tucked safely inside my sleeves where nobody can see~ | 嘿嘿，成交！袖子里一揣就没人发现了~ | 헤헤, 거래 성립! 아무도 못 보게 소매 안에 잘 숨겨야지~ | 유지 | 약을 소매 속에 숨기는 행동 |
| 878 | 69 | `dialog_lun_duty_black_injector_thanks` | Wah, thanks a million... High-grade neural injectors like this cost a fortune. You're actually really sweet...! | 呜哇，太感谢你了……这种高阶黑市舒缓剂在黑市可贵了，你人真好……！ | 와, 정말 감사합니다…… 이런 고급 암시장 신경 주사제는 엄청 비싼데. 사장님, 정말 좋은 분이네요……! | **수정** | 비싼 암시장 신경 주사제라는 성격 복원 |
| 879 | 70 | `dialog_lun_duty_wrong_item` | No, no, no! I asked for neuro-blocker painkillers! What did you just hand me?! | 不对不对！人家要的是神经阻断止痛药，你给我拿的是什么乱七八糟的啦！ | 아니, 아니, 아니! 신경 차단 진통제를 달랬잖아요! 대체 뭘 주신 거예요?! | 유지 | 신경 차단 진통제가 아닌 물건 거절 |
| 880 | 71 | `dialog_lun_duty_blackinjector_no` | S-Synaprest?! Do you... do you think I've got a substance problem?! I don't! | 赛、赛纳普斯神经针？！你……你以为我对管制舒缓剂成瘾了吗？！我才没有！ | 시, 시나프레스트요?! 제가…… 제가 규제 진정제에 중독됐다고 생각하세요?! 아니거든요! | **수정** | 규제 진정제 중독 의혹을 구체적으로 부정 |
| 881 | 72 | `dialog_lun_duty_blackinjector_no2` | And even if I did... with my pathetic patrol stipend, I could never afford this stuff anyway, sniff... | 就算真有……以我那点可怜的巡逻津贴，哪买得起这种天价违禁药剂啊呜呜…… | 설령 그렇다 해도…… 이 쥐꼬리만 한 순찰 수당으로는 이런 약을 살 돈도 없단 말이에요. 훌쩍…… | 유지 | 순찰 수당으로는 약을 살 수 없다고 하소연 |
| 882 | 73 | `dialog_lun_duty_contraband` | Works for me! Quick, shove it up my sleeve... I'll take it! | 没问题，快塞进我袖子里……我全收了！ | 그것도 좋아요! 빨리 소매 안으로 넣어 주세요…… 제가 받을게요! | 유지 | 밀수품도 소매 속에 넣어 받겠다는 말 |
| 883 | 74 | `dialog_lun_duty_right_item` | Mmhmm, looks legit! Now I can finally finish patrol without wincing! | 嗯嗯，成色不错！今天巡逻的底气总算有了！ | 으음, 진짜 약이 맞네요! 이제 아파서 찡그리지 않고 순찰할 수 있겠어요! | 유지 | 약을 받고 순찰할 자신이 생김 |
| 884 | 75 | `dialog_lun_surplus_1` | Shh—hey! Keep it down, little boss, it's your absolute lucky day! | 嘘——喂！别声张，小店长，你今天可是踩到狗屎运了！ | 쉿, 사장님! 목소리 낮춰요. 오늘 정말 운이 좋으시네요! | 유지 | 오늘 운이 좋다며 은밀한 거래를 예고 |
| 885 | 76 | `dialog_lun_surplus_2` | Requisition just authorized the release of some... cough, 'surplus tactical assets'! | 治安征用处刚刚特批了一小批……咳，‘冗余闲置’的战术物资处理配额！ | 치안 징발처에서 드디어…… 크흠, '잉여 전술 장비' 반출을 특별 승인했거든요! | **수정** | 치안 징발처에서 잉여 전술 물자 반출 |
| 886 | 77 | `dialog_lun_surplus_3` | Well? I smuggled them straight here just for you. Interested? | 怎么样？本警官可是第一时间偷偷给你送来的，感兴趣吗？ | 사장님 드리려고 여기까지 몰래 가져왔어요. 어때요, 관심 있죠? | 유지 | 사장에게 몰래 가져온 장비라고 소개 |
| 887 | 78 | `dialog_lun_surplus_4` | But pinky promise first! Strictly confidential between us, not a peep to anyone! | 但我们得先拉钩！这事属于治安部最高机密，绝对不许外传！ | 하지만 먼저 새끼손가락 걸고 약속해요! 우리 둘만 아는 비밀이에요. 절대 발설 금지! | 유지 | 새끼손가락 걸고 비밀을 지키라는 약속 |
| 888 | 79 | `dialog_lun_surplus_5` | If you're a pal and take the whole batch, I'm authorized to give you 33% off! Pretty sweet deal, huh! | 要是你讲义气全包圆了，我就行使特权给你打个六七折（33% off）！很划算吧！ | 한 번에 전부 사 주시면 특별히 33% 할인해 드릴게요! 굉장하죠?! | 유지 | 물건 전체 구매 시 33% 할인 |
| 889 | 80 | `dialog_lun_surplus_accept` | Splendid! You're a lifesaver... the ledger might actually balance now! | 太好啦！真是帮大忙了……账目终于能平上了！ | 좋아요! 정말 살았어요…… 이제 장부 숫자도 좀 맞겠네! | 유지 | 거래 후 장부 금액을 맞출 수 있다고 안도 |
| 890 | 81 | `dialog_lun_surplus_arrest_client_1` | Waaah?! W-Wait! Why are Internal Affairs here...?! | 哇啊啊？！等、等等！内务督察怎么会……？！ | 으아아?! 자, 잠깐! 감찰부가 왜 여기에 있는 거죠……?! | 유지 | 감찰부가 갑자기 나타나 놀라는 반응 |
| 891 | 82 | `dialog_lun_surplus_arrest_sec_1` | Officer Lun? What in the world are you doing skulking around that counter? | 伦警官？你鬼鬼祟祟地在柜台前倒腾什么呢？ | 룬 경관? 거기 계산대에서 대체 뭘 하고 있지? | 유지 | 감찰관이 카운터에서의 수상한 행동을 추궁 |
| 892 | 83 | `dialog_lun_surplus_arrest_sec_2` | Found them! This is the missing gear flagged from the Armory! | 找到了！这批正是军械库报失的防暴战术装备！ | 찾았습니다! 무기고에서 분실 신고한 진압용 전술 장비들입니다! | **수정** | 무기고에서 분실된 진압용 전술 장비 확인 |
| 893 | 84 | `dialog_lun_surplus_arrest_client_2` | W-Wait, I can explain! I just brought them out for anti-rust maintenance... it wasn't black-marketing, I swear! | 哇呜呜！等等！听我解释！我只是拿出来做防锈保养……真的不是倒卖啊啊！ | 자, 잠깐! 설명할 수 있어요! 녹슬지 말라고 관리하려던 것뿐이에요…… 암거래가 아니라니까요! | 유지 | 암거래가 아닌 방청 관리라고 둘러대기 |
| 894 | 85 | `dialog_lun_surplus_arrest_sec_3` | Come quietly, Officer Lun. You know what happens if you resist the inspectorate. | 乖乖跟我们回禁闭室，伦警官。你很清楚抗拒督察条例的下场。 | 얌전히 구금실로 따라와, 룬 경관. 감찰 명령에 저항하면 어떻게 되는지 알잖아. | **수정** | 구금실에 따라와야 한다는 감찰 경고 |
| 895 | 86 | `dialog_lun_inspect_1` | Ahem! Freeze! S-Surprise inspection! Routine security check! | 咳咳！肃静！突、突击盘查！治安部例行临检！ | 크흠! 꼼짝 마! 기, 기습 검사입니다! 통상 보안 점검이에요! | 유지 | 기습 검사와 정기 점검을 동시에 말하는 당황 |
| 896 | 87 | `dialog_lun_inspect_2` | Open up the safe, right now! No stalling, I can be very tough if I have to! | 快点把保险柜打开！不准磨蹭，本警官可是很严厉的！ | 금고 열어요, 당장! 시간 끌지 마세요. 저도 마음먹으면 아주 무섭다고요! | 유지 | 금고를 열라고 위협하는 과장된 명령 |
| 897 | 88 | `dialog_lun_inspect_drugs_1` | Hmph! Caught red-handed! Unregistered controlled neuro-stims detected. Confiscating them this instant! | 哼！当场人赃俱获！发现未申报的受管制神经制剂，现依法予以扣押没收！ | 흥! 현행범으로 딱 걸렸네요! 미등록 규제 신경 각성제를 발견했습니다. 즉시 압수하겠어요! | 유지 | 미등록 규제 신경 제제 압수 |
| 898 | 89 | `dialog_lun_inspect_drugs_2` | Don't you pout at me! You should be grateful I didn't slap cuffs on you and drag you to the station! | 扁什么嘴啦！本警官没有当场给你戴上手铐押进局子，你就该偷着乐了哼！ | 그런 표정 짓지 마세요! 수갑 채워서 연행하지 않은 걸 감사히 여기시라고요! | 유지 | 수갑을 채우지 않았음을 강조하는 훈계 |
| 899 | 90 | `dialog_lun_inspect_clear` | sighs in deep relief... Phew, all clear. You're clean... gosh, that was nerve-wracking. | 悄悄松了一大口气……呼，算你守法，一切正常……差点吓死我了。 | (안도의 한숨) 휴우, 전부 확인했는데 불법 물품은 없네요. 문제없어요…… 아, 진짜 떨렸다. | 유지 | 불법 물품이 없어 안도하는 룬 경관 |
| 900 | 91 | `dialog_lun_inspect_arrest_client_1` | Eh...? C-Chief? | 诶……？头、头儿？ | 어……? 대, 대장님? | **수정** | 원문의 Chief는 서장 단정 대신 대장으로 처리 |

## 검수 결과 및 다음 범위
- 이번 검수 33개 교정, 267개 유지.
- 전체 진행 **900 / 2,593**, 누적 교정 **69개**, 미검수 **1,693개**.
- 다음은 전체 901번 / NPC Girls 내부 92번부터.
- CSV 키 집합·변수·줄바꿈·서식·수정 범위 외 텍스트 불변을 점검했다.
- Steam 미반영, 기존 ZIP은 이번 변경을 포함하지 않으므로 최종 배포 금지.
