# Snake Ranch 0.7.10 — 搜查「主动告知」收口成一笔会话、调试项中英切换即时跟随 / The pre-search choice is now one validated session; debug labels follow the language live

## 0.7.10 本次更新 / This release

- **把「待搜查」收口成一笔有身份的会话。** 0.7.9 让弹窗终于能弹出来，但等待期间只记了几个散装变量，没有「这次搜查是谁的」这个概念。现在每次待搜查都绑定**发起时的那家店 + 那位搜查官 + 一个流水号**，弹窗、玩家确认、12 秒兜底三条路径**共用同一套身份校验**，任何一条路径在动手前都会先问一句「这笔搜查还算数吗」。
- **换客人不再吃掉宽限期。** 计时改成「按区间开始时的状态记账」：别的客人在场的那段时间**一秒都不计**，哪怕他站了 19 秒再走，原搜查的 12 秒宽限也**原封不动**；搜查官归位后的第一帧也不会被追溯扣费。
- **旧弹窗和迟到回调一律作废。** 原搜查官已经离场、店里换了客人、这次搜查已被新的一笔取代、或者换店／读档／回主菜单／模组被关掉——只要会话失效，**旧弹窗立即撤下，迟到的确认与兜底一律不重放原生搜查、不发告知奖励、不伤任何一条蛇**。0.7.9 里这些迟到路径仍可能把奖励和伤害兑现到一笔早已结束的搜查上。
- **枪战／逮捕期间不再事后补扣。** 之前 UI 被原生占用时整条计时支线根本不采样，等占用解除的那一瞬间会把整段被占用的时间**一次性补记**到宽限头上——和换客人是同一类缺陷。现在计时在每一次心跳里都先走一遍，再进各种显示门；玩家正看着我们自己的问题时也不计费。
- **预检口径与原生对齐。** 假检查员（wanted9）**不再被本模组接管**：不弹告知窗、不动声望，原生搜查照它自己的口径跑完（原生只把数量归零、等级下调，搜查本身并不取消）。懒散检查员（isLazyInspector）按原生把性格覆写成 2，**性格 2 不扫垃圾箱**，因此「只有垃圾箱里有蛇」时不再白白挂起一笔注定搜不到的等待。预检和真正开扫**共用同一个读取谓词**，不会再出现「预检说有、开扫说无」的分叉。
- **换客途中点旧弹窗，不再把整场搜查吞掉。** 弹窗开着时店里换了客人，这笔搜查只是**暂停**、不是结束。此时点旧弹窗不会结算（否则会读到错客人的蓝图），但**也不会把搜查丢弃**——搜查官重新成为当前客人后，弹窗会再出现，原生搜查照常做完，罚款、扣押与后续对白都还在。修前这一笔会被直接丢掉，原生搜查再也不跑。
- **被拒绝的选择，永远不会在事后偷偷结算。** 弹窗开着时换了客人，你点「主动告知」或「什么都不说」会看到「情况已变」的提示——这次选择**没有生效**。但如果搜查官在结果页翻完之前又回来了，旧代码会让那个**已经被拒绝**的选择照样结算：告知偷偷加声望，沉默偷偷把眼镜蛇打死。现在末页结算必须**同时**匹配「这笔搜查还算数」和「这个选项当时真的通过了校验」，两者缺一就不结算、不发奖、不伤蛇；会话仍有效时弹窗会重新出现，让你重新选一次，新选择只结算一次。
- **两个选项按钮不再是同一句话。** 原生气泡的按钮文字取自「选项目标页」的正文，而不是选项名或说明。此前四个目标页写的都是「正在结算……」，于是告知弹窗的两个按钮都显示「正在结算……」，蛇胆援助弹窗也一样——玩家根本分不出哪个是告知、哪个是沉默。现在四个按钮分别显示**告诉他店里有蛇／什么都不说／提供蛇胆（眼镜王蛇）／当场剖开取胆（眼镜蛇）／拒绝**，中英文都已核对。选择之后的结算页文案不变。
- **换客那一帧跨过 12 秒，不再把整场搜查吞掉。** 计时按区间开始时的状态记账，因此「跨过 12.0 秒的那一次心跳」可能已经在暂停中了。旧顺序先判兜底再判暂停，于是这一帧会去重放一笔非活跃会话，结果整笔搜查被丢弃——罚款、扣押、后续对白全没了，搜查官回来也不会再触发。现在**先判暂停再判兜底**：暂停期间保留剩余预算，搜查官归位后兜底照常只触发一次。普通对话占用时的 12 秒兜底原样保留。
- **不再误清原版的 SEC 独白标志。** 告知弹窗此前会借用 `isSecMonologuePlaying`，退出时按进入时采样的旧值还原。若弹窗挂着时原版因枪战／义体举报把该标志置真，我们的清理会把它改回假，可能打断原版独白。现在**告知弹窗完全不碰这个标志**（搜查官仍是当前客人，本就不需要它）；蛇胆援助弹窗保留借用，因为那时客人已经离开。
- **12 秒兜底与 30 秒冻结上限都保留。** 普通对话占用照常计时，12 秒兜底原样保留；本次独白最多冻结 30 秒，超出照常计费。**告知、沉默、蛇胆援助的既定玩法一个字没改**：主动告知＝这条蛇不被搜到、检查照常做完、安全检查声望 +1；什么都不说＝与原来完全一致（眼镜蛇死亡、眼镜王蛇重伤存活），随后再问是否用蛇胆援助。
- **修复调试项中英切换不跟随、注册失败后越retry越多。** 原生调试面板的 Add **不去重**、Remove 按来源整批删除。0.7.9 的延迟注册只注册一次，之后切语言标签不动；一旦注册到一半抛异常，重试会把已写进去的那半截**再追加一遍**（实测 3 + 18 = 21 项）。现在注册改为**先按来源清空再整批写入**，并用一个本地化指纹做节流探针：同一进程内中英来回切换，**18 项标签会跟着更新**；注册一半失败再重试，**仍然恰好 18 项**。18 个动作、13 个试剂场景、测试开关与三组配置分组**全部保留**。
- 已知取舍：主菜单调试面板在首次进店／读档前仍可能看不到本模组条目；语言切换后标签更新有 **≤0.5 秒**的探针延迟；配置分组标题仍是中英并列串；枪战期间本模组的援助弹窗会一并暂停。
- **独白归属的证据与取舍（重要，实机待核）**：反编译确认 `DialogUIManager.isSecMonologuePlaying` 是个裸 public bool，该类型内**只声明和读取、从不赋值**；全项目唯一写入点在 `StoreUIManager.OnGenericArrived(SEC)` 的**枪战**与**义体举报**两个分支，清除点在 `StartLeaveSec()` 与 `ClearArrestState()`。也就是说**这个标志属于 SEC 到场独白，不是常规违禁品搜查的独白，本身也不带任何搜查身份标记**。这修正了 0.7.9 「独白长期成立吃掉宽限」的旧叙述——真正恒成立的拦路门只有客人门。本版据此采取最小取舍：**只在同一搜查官仍在场且 UI 可用时**才把该标志计入独立冻结预算（硬上限 30 秒），枪战／逮捕改判为暂停（不计费、无上限）。**本版并未声称已能区分「本次搜查的独白」**，该点需要实机核对。
- 门禁：规则套件 **1085 断言**、搜查定向套件 **249 断言**（含 21 个会话端到端场景，新增按钮文案探针与两条 r4 会话场景）、调试注册套件 **34 断言**、补丁生命周期 **52 条 IL 契约**（新增 10 条 r4 契约：两个弹窗写入各自的选项文案、暂停早退在 12 秒兜底之前、告知弹窗不写独白标志而援助弹窗仍写）、交配反馈回归 **183 断言**。其余原有门禁全部复跑通过。三条新回归均已在 r3 上复现为红。
- 无新增物品 ID、无新增物品标签、不改原生存档格式；击杀条件、声望数值、蛇胆援助规则全部未动。本包已包含 0.7.8 与 0.7.9 的全部改动，可以直接上传本版本。
- **离线全绿 ≠ 实机通过**：以上均为纯规则／生产替身／原生 IL 契约层面的结论，Unity 运行时、真实 UI 与真实存档仍需实机验证。

- **The held-back search is now one session with an identity.** 0.7.9 made the prompt reachable, but the wait only kept a few loose variables and had no notion of *whose* search this was. Every held-back search now binds **the store it started in + that specific inspector + a serial token**, and the prompt, the player's confirmation and the 12-second fallback **share one identity check**: each path asks "is this search still valid?" before it does anything.
- **A different customer no longer eats the grace period.** Timing is now credited to the state observed **at the start of each interval**: time with another customer on the floor is **never billed**, even if he stands there for 19 seconds, and the first frame after the inspector returns is not charged retroactively either.
- **Stale prompts and late callbacks are void.** If the original inspector has left, the store has a different client, this search has been superseded by a newer one, or the store was swapped / a save loaded / the main menu returned / the mod disabled — the session is dead, so **the old prompt is withdrawn and any late confirmation or fallback never replays the native search, never grants the speak-up reward and never harms a snake**. Under 0.7.9 those late paths could still cash out rewards and damage on a search that had already ended.
- **Shootouts and arrests no longer bill retroactively.** While the native UI was busy the whole timing branch was not sampled at all, so the instant the block cleared, the entire blocked span was **charged in one lump** — the same class of defect as the customer swap. Timing now runs on every heartbeat before any display gate, and the player reading our own question is not billed either.
- **The pre-check now matches native semantics.** A fake inspector (wanted9) is **no longer taken over by this mod**: no speak-up prompt, no reputation change, and the native search completes on its own terms (native only zeroes the count and lowers the tier; it does not cancel the search). A lazy inspector (`isLazyInspector`) overrides personality to 2 exactly as native does, and **personality 2 does not scan the trash**, so a snake hidden only in the trash no longer parks a wait that could never find it. The pre-check and the real scan **share one read predicate**, so "the pre-check says yes, the scan says no" can no longer diverge.
- **Clicking the old prompt during a customer swap no longer swallows the whole search.** If the floor swaps customer while the prompt is open, the search is only **paused**, not over. Clicking the old prompt then does not settle it (that would read the wrong client's blueprint), but it **no longer drops the search either**: once the inspector is the current client again the prompt returns, the native search completes, and the fine, confiscation and follow-up dialogue all still happen. Before this fix that search was discarded outright and the native search never ran.
- **A rejected choice can never settle behind your back.** If the floor swaps customer while the prompt is open, clicking "speak up" or "stay silent" shows the "situation changed" page — that choice **did not take effect**. But if the inspector returned before the result page finished, the old code let that **already-rejected** choice settle anyway: speak-up quietly granted reputation, silence quietly killed the cobra. Settlement now has to match **both** "this search is still valid" **and** "this option actually passed validation at the time"; if either fails there is no replay, no reward and no harm. While the session is still valid the prompt reopens so you can choose again, and the new choice settles exactly once.
- **The two option buttons no longer read the same line.** The native bubble takes its caption from the choice TARGET page's body text, not from the option name or description. All four targets used to say "Settling...", so both buttons on the speak-up prompt read "Settling...", and the gallbladder aid prompt had the same problem — the player could not tell speak-up from silence. The buttons now read **Warn him about the snake / Say nothing / Offer a gallbladder (king cobra) / Cut the gall out on the spot (common cobra) / Refuse**, verified in both languages. The post-choice settling page is unchanged.
- **Crossing 12 seconds on the swap frame no longer swallows the whole search.** Timing is billed by the state at the start of an interval, so the pump that crosses 12.0 may already be paused. The old order tested the fuse before the pause, so that frame replayed a non-active session and the entire search was discarded — no fine, no confiscation, no follow-up dialogue, and nothing retriggered when the inspector returned. The pause is now checked **before** the fuse: a paused session keeps its remaining budget, and once the inspector is back the fuse still fires exactly once. The ordinary-dialogue 12-second fallback is untouched.
- **The native SEC monologue flag is no longer cleared by mistake.** The speak-up prompt used to borrow `isSecMonologuePlaying` and restore the value sampled on entry. If the native side set it true while our prompt was open (shootout / augment report), our cleanup wrote it back to false and could cut the native monologue short. The speak-up prompt now **never touches that flag** (the inspector is still the current client, so it was never needed); the gallbladder aid prompt keeps the borrow because the customer has already left there.
- **Both the 12-second fallback and the 30-second freeze cap are kept.** Ordinary dialogue occupancy still counts, and the 12-second fallback is untouched; this search's monologue freezes for at most 30 seconds and is billed normally beyond that. **The established outcomes are unchanged word for word**: speaking up means the snake is not found, the search still completes and security reputation gains +1; staying silent behaves exactly as before (common cobra dies, king cobra survives badly injured), followed by the gallbladder aid question.
- **Fixed debug labels not following the language, and retries piling up entries.** The native debug panel's Add **does not deduplicate** and Remove deletes by source. 0.7.9 registered once and never updated labels on a language switch; if registration threw halfway, a retry **appended the already-written half again** (measured: 3 + 18 = 21 entries). Registration now **clears by source first, then writes the whole batch**, with a localization fingerprint as a throttled probe: switching Chinese/English within one process **updates all 18 labels**, and a half-failed registration retried still leaves **exactly 18**. All 18 actions, 13 kit scenarios, the test gate and the three config groups are **preserved**.
- Known trade-offs: the main-menu debug panel may still not list this mod until you first enter a store or load a save; label updates lag the language switch by **up to 0.5 s**; config section headers remain bilingual strings; this mod's aid prompts pause during a shootout.
- **Monologue attribution — evidence and trade-off (important, needs in-game verification)**: decompilation confirms `DialogUIManager.isSecMonologuePlaying` is a bare public bool that the type **only declares and reads, never assigns**; the only writers in the whole project are the **shootout** and **augment-report** branches of `StoreUIManager.OnGenericArrived(SEC)`, cleared in `StartLeaveSec()` and `ClearArrestState()`. So **this flag belongs to the SEC-arrival monologue, not to a routine contraband search, and carries no search identity at all**. This corrects 0.7.9's older claim that a long-standing monologue ate the grace period — the only permanently-true gate was the customer gate. This release therefore takes the minimal trade-off: the flag counts against a separate freeze budget (hard cap 30 s) **only while the same inspector is still present and the UI is usable**, while shootout/arrest is reclassified as paused (unbilled, uncapped). **This release does not claim to distinguish "this search's monologue"**; that point still needs in-game verification.
- Gates: rules **1085 assertions**, inspection **249 assertions** (21 end-to-end session scenarios, plus a button-caption probe and two new r4 session scenarios), debug registration **34 assertions**, patch lifecycle **52 IL contracts** (10 new r4 contracts: each prompt writes its own option captions, the paused early-return precedes the 12-second fuse, the speak-up prompt never writes the monologue flag while the aid prompt still does), mating feedback regression **183 assertions**. Every pre-existing gate was re-run and passes. All three new regressions were reproduced as failures on r3 first.
- No new item IDs, no new item tags, no change to the native save format; kill conditions, reputation values and gallbladder aid rules are all untouched. This package already contains every 0.7.8 and 0.7.9 change and can be uploaded directly.
- **Offline green is not in-game verified**: everything above is at the pure-rules / production-stub / native-IL-contract level. The Unity runtime, the real UI and real saves still need in-game testing.# Snake Ranch 0.7.9 — 搜查前「主动告知有蛇」终于能弹出来、启动阶段不再把英文烤进界面 / The pre-search "speak up" prompt actually appears; startup no longer bakes English into the UI

## 0.7.9 本次更新 / This release

- **修复「主动告知有蛇」弹窗从来弹不出来的问题。** 这个选项 0.6.x 就写进了代码，但玩家实际永远看不到：原生搜查开始时，**治安官本人就是「当前客人」**，而旧判定写的是「店里还有客人就先不弹窗」——这条在搜查全程恒成立，于是弹窗被无限期挡住；宽限期 12 秒一到，程序自动按「什么都不说」跑原流程，**眼镜蛇在玩家做出任何选择之前就被打死**。这正是「描述里写了能保蛇，实际只能选拿不拿蛇胆」的原因。
- 现在只等**别的客人**：治安官自己在场不再阻挡自己的弹窗；治安官独白占用的时间**不再计入宽限期**（最多冻结 30 秒，防止异常长的独白把整场检查吞掉），弹窗会在能显示的第一时间出现。被**其他对话窗口**长时间占用的情况仍照常计时，12 秒兜底原样保留——这条 0.6.x 就有的保险丝没动，检查永远不会被无限期挂起。
- **后果口径一个字没改**：主动告知＝这条蛇不被搜到、检查照常做完、安全检查声望 +1；什么都不说＝与原来完全一致（眼镜蛇死亡、眼镜王蛇重伤存活），随后再问是否用蛇胆援助。**这次修的是「玩家终于能选」，不是「蛇不会死」。**
- **修复启动阶段把英文烤进界面的残留。** 调试面板条目名和配置分组名都是「立即求值」的字符串，而游戏本地化尚未就绪时，`ModHelper` 取不到语言就**回退英文**——模组加载早于主菜单本地化完成时，这些文字就被永久写成英文，之后切语言也救不回来。现在调试条目改到**新游戏／读档／进店之后**才注册，配置分组名改用固定双语标题、不再查本地化。启动阶段不再读取游戏语言。
- 已知取舍：主菜单的调试面板在**首次进店或读档之前**可能看不到本模组条目（纯测试功能，不影响正常游玩）。
- 门禁：规则套件 **1070 断言**（新增 24 条弹窗时序断言：治安官本人不再挡自己、别的客人仍然要等、宽限期只武装一次、独白时间被冻结且有上限、其他对话占用仍会超时、兜底仍按沉默走）；搜查定向套件 **126 断言**（新增 9 条端到端：治安官在场时弹窗真的出现且发生在任何扫描/踢击之前、长独白后仍能弹、别的客人离开后才弹）；补丁生命周期 **29 条 IL 契约**（新增 5 条：启动闭包不得注册调试条目、不得立即求值本地化文本、全程序集只有延迟构造器注册调试条目、注册不得由 OnEnable/Init 触发、标签确实在构造时才解析）。
- 无新增物品 ID、无新增物品标签、不改原生存档格式；击杀条件、声望数值、蛇胆援助规则全部未动。

- **Fixed the pre-search "speak up" prompt never appearing.** The choice has been in the code since 0.6.x, but players could never reach it: when the native search starts, **the inspector himself is the "current client"**, and the old gate said "hold the prompt while any client is present" — which is true for the entire search. The prompt was blocked indefinitely, and once the 12-second grace period expired the mod silently ran the original flow, **killing the cobra before the player made any choice**. That is exactly why the description promised a way to save the snake while the game only ever offered the gallbladder choice.
- The prompt now only waits for a **different** customer: the inspector being present no longer blocks his own prompt, and time spent on his monologue **no longer counts against the grace period** (frozen for at most 30 seconds, so an abnormally long speech still cannot swallow the search). A UI held by **another dialogue** still counts as before, and the 12-second fallback is kept untouched — that 0.6.x safety fuse still guarantees a search is never suspended indefinitely.
- **The outcomes are unchanged, word for word**: speaking up means the snake is not found, the search still completes, and security reputation gains +1; staying silent behaves exactly as before (common cobra dies, king cobra survives badly injured), followed by the gallbladder aid question. **This fix makes the choice reachable — it does not make snakes immortal.**
- **Fixed the leftover startup path that baked English into the UI.** Debug-panel entry names and config section names are eager strings, and `ModHelper` falls back to **English** whenever the game locale is not resolved yet — so a mod that loads before the main menu finishes localization wrote those strings in English permanently, unrecoverable by switching language later. Debug entries are now registered **after a new game, a save load, or entering the store**, and config sections use fixed bilingual headers that never query localization. Mod startup no longer reads the game language at all.
- Known trade-off: the main-menu debug panel may not list this mod's entries **until you first enter a store or load a save** (test-only feature, no impact on normal play).
- Gates: the rules suite passes **1070 assertions** (24 new prompt-timing assertions: the inspector no longer blocks himself, a different customer still waits, the grace period arms only once, monologue time is frozen out under a cap, an open dialogue still times out, the fallback still runs the silent search); the inspection suite passes **126 assertions** (9 new end-to-end ones: the prompt really appears with the inspector present and before any scan or kick, it still appears after a long monologue, and it waits for a different customer to leave); patch lifecycle passes **29 IL contracts** (5 new: the startup closure may not register debug entries or resolve eager localized text, only the deferred builder registers entries assembly-wide, registration is never triggered from OnEnable/Init, and labels really are resolved at build time).
- No new item IDs, no new item tags, no change to the native save format; kill conditions, reputation values and gallbladder aid rules are all untouched.# Snake Ranch 0.7.8 — 蛇夫人整对随机货单、8848 电话冷却缩短到 2 天 / Whole-pair random walk-in stock, 8848 phone cooldown cut to 2 days

## 0.7.8 本次更新 / This release

- **周三日常上门的蛇改为整对随机**：每次上门掷一次，50% 概率带**眼镜蛇对**、50% 概率带**玉米蛇对**，一公一母、整对同种，一次上门就能凑齐一对可繁育的组合。此前日常上门固定只带眼镜蛇，玉米蛇只能等「落魄上流人士」每天一条、性别当天定死、还有四分之一是染色假货。
- 掷骰按**上门当天的天数确定性生成**：同一次上门内的货单重试、柜台腾挪、读档重建都不会重掷，不会出现「看一眼柜台就换了个物种」。长期分布仍是各占一半。
- **玉米蛇保留自己的花色与纯度估值**：上架时不再用通用体重曲线覆写价格，配色溢价不会被抹掉；眼镜蛇与王蛇定价不变。
- **上周没买走的蛇不再挡住本周的整对**：库存判重改为按物种隔离，旧货原样保留、本周这一对照常补齐，不会再出现「柜台上是眼镜蛇公配玉米蛇母」这种配不上对的组合。
- **8848 电话冷却从 5 天缩短到 2 天**；旧存档里更长的剩余冷却在读档时自动收敛到 2 天，不需要重开档。
- **修复日常上门被误判成电话预约单、白白烧掉电话冷却的问题**：来源判定的兜底分支原本在「已交过号、队列里又有这位客人」时一律算作电话单，只要到期日标记与实际到达日差一天（早晨队列补偿、读档重建客户端、错过一天）就会误判。现在兜底改为**证明不了是预约单就当日常上门**，日常上门永不占用电话冷却。
- 中英文货单说明与电话提示同步更新，明确写出「整对同种、每次随机」与「2 天冷却」。
- 门禁：规则套件 **1046 断言**（新增 7 条整对随机断言：同日不重掷、边界天数稳定、两物种都出现、长期与周三网格分布各半）；商人集成套件 **256 断言**（新增整对同种、一公一母、跨物种存货不挡对、兜底不烧冷却等回归）。
- 无新增物品 ID、无新增物品标签、不改原生存档格式；本次不涉及偷蛋鼠获取渠道。

- **Wednesday walk-ins now bring a whole pair at random**: each visit rolls once — a 50% chance of a **cobra pair** and a 50% chance of a **corn-snake pair**, one male and one female of the same species, so a single walk-in gives you a breedable pair. Previously walk-ins always carried cobras, and corn snakes only came from the Broke Upper Level seller: one per day, sex fixed for that day, and a quarter of them dyed fakes.
- The roll is **deterministic on the visit day**: stock retries, counter reshuffles and save reloads inside the same visit never reroll it, so the species cannot change just because you looked at the counter again. The long-run split is still half and half.
- **Corn snakes keep their own coat and purity valuation**: listing them no longer overwrites the price with the generic weight curve, so colour premiums survive; cobra and king cobra pricing is unchanged.
- **An unsold snake from a previous visit no longer blocks this week's pair**: stock de-duplication is now scoped per species, so old stock is preserved and the new pair is still completed — you can no longer end up with a cobra male next to a corn female, which is not a breedable combination.
- **The 8848 phone cooldown drops from 5 days to 2 days**; a longer remainder left in an older save is clamped down to 2 days on load, no new game required.
- **Fixed walk-ins being misread as booked phone runs and burning the phone cooldown for nothing**: the fallback branch of the visit-source classifier used to treat any queued visit as a phone run once the number had been handed out, so a due-day marker off by a single day (morning queue compensation, a reload that recreates the client, a missed day) was enough to misfire. The fallback now **fails safe to a walk-in whenever a booking cannot be proven**, and walk-ins never touch the phone cooldown.
- English and Chinese stock lines and phone hints updated to state the same-species random pair and the 2-day cooldown explicitly.
- Gates: the rules suite passes **1046 assertions** (7 new whole-pair roll assertions: no reroll within a day, stable boundary days, both species appear, half-and-half over time and on the Wednesday grid); the merchant integration suite passes **256 assertions** (new regressions for same-species pairs, one male plus one female, cross-species leftovers not blocking a pair, and the fallback never arming the cooldown).
- No new item IDs, no new item tags, no change to the native save format; the egg-thief rat acquisition route is untouched in this release.# Snake Ranch 0.7.7 — 补丁守卫不再整体初始化失败、蛇笼跨档交易等级备份自愈 / Patch guard keeps the mod alive, cage level backup & self-heal across save trades

## 0.7.7 本次更新 / This release

- 修复部分模组共存环境下**整个模组初始化失败**的问题：旧逻辑里任一 IL 补丁发现游戏代码结构与预期不符就直接抛异常，Harmony 的整批补丁会被中止，连带 `Init failed`，表现为蛇夫人不出现、模组全部功能失效（日志里的 `egg thief ValueModifier base contract mismatch`）。现在所有 IL 补丁统一走**失效降级守卫**：契约不符只跳过该条补丁并输出带上下文的诊断，其余补丁与模组本体照常运行。
- 蛋贼价格补丁兼容两种 IL 形态：`unitValue` 读取无论是 `callvirt` 属性 getter 还是 `ldfld` 后备字段都能识别；找不到锚点时只降级，不再抛异常。
- 五处 IL 契约补丁（蛋贼价格、蛇笼三处上限、胎数提示）改为**逐条独立注册**，任何一条失败都不会影响其它补丁；模组停用时逐条摘除自己注册的补丁，不动模组加载器的整体生命周期。
- 新增**蛇笼跨档交易等级备份与读档自愈**：蛇笼用金属锭扩建成功后，把已达到的最高等级记在物品标签上；读档解码后若发现等级或格子低于备份值，自动补回等级标签并把格子撑到对应尺寸。只认本模组蛇笼 `snake_ranch.snake_cage`，不修改其它模组 DLL、不改原生存档格式、不新增外部账本；容器尚未挂载网格时静默跳过，8 级笼不产生任何变化与日志。
- 门禁补强：新增 **IL 套用审计**（对真实游戏 IL 跑全部 transpiler 并做故障注入，57 断言），补上过去"只审补丁绑定、不审 IL 能否套用"的缺口；蛇笼兼容规则新增 15 断言，规则套件 1039 断言全通过。
- 无新增物品 ID；仅新增一个物品标签 `SR_CAGE_LEVEL_BACKUP`，旧档缺失视为未备份，直接兼容。

- Fixed the mod failing to initialise entirely in some mod-loadout combinations: previously any IL patch that found an unexpected game code shape threw immediately, which aborted Harmony's whole patch pass and took `Init` down with it — the Snake Lady never appeared and every feature went dark (the `egg thief ValueModifier base contract mismatch` line in the log). All IL patches now run through a **fail-soft guard**: a broken contract skips only that one patch and prints a bounded diagnostic, while every other patch and the mod itself keep running.
- The egg-thief price patch now accepts both IL shapes of the `unitValue` read — a `callvirt` property getter or an `ldfld` backing field — and degrades instead of throwing when no anchor exists.
- The five IL contract patches (egg-thief price, three cage limits, litter tooltip) are now **registered one by one**, so a single failure cannot affect the others; on disable the mod removes exactly its own transpilers and never touches the loader's assembly-wide lifecycle.
- Added **cage level backup and post-load self-heal across save trades**: after a metal-ingot expansion succeeds, the highest level reached is stored on the item itself; after save decoding, if the level or the grid is below that backup, the level tag is restored and the grid is grown to the matching size. Only this mod's own cage `snake_ranch.snake_cage` is touched — no other mod's DLL, no change to the native save format, no external ledger. It stays silent when the container has no grid attached yet, and an 8-level cage is left completely unchanged.
- Gate hardening: a new **IL application audit** runs every transpiler against the real game IL with fault injection (57 assertions), closing the old gap where only patch bindings were audited and IL applicability was not; cage compatibility rules add 15 assertions, and the rules suite passes 1039.
- No new item IDs; only one new item tag `SR_CAGE_LEVEL_BACKUP`, absent in old saves and treated as "no backup", fully compatible.# Snake Ranch 0.7.6 — 开局保底蛇笼、周访不再烧电话冷却、货单明细文案 / Start-guarantee cage, walk-ins no longer burn phone cooldown, explicit stock lists

## 0.7.6 本次更新 / This release

- 养蛇开局保底蛇笼：玩家一个蛇笼都没有时，每周三日常上门（含首访）会**优先**摆一个蛇笼上架；一旦拥有任何蛇笼，日常货单恢复原样，8848 仍是加急设备通道。开局不再"17 天见不到笼"。
- 修复"NPC 一直来、一直占用打电话冷却"：旧逻辑在**每次**上门（含周三日常访）都调用 UseService() 把 8848 打进 5 天冷却，于是每周三她一来电话就又被锁 5 天，永远约不上。现在只有**预约的设备单送达**才启动冷却；日常上门与首访交号都不烧冷却。
- 货单优先级：设备单里蛇笼排在捕蛇夹/喷涂枪之前；日常访里保底笼排在蛇/蛋/王蛇货/清洁补给之前。柜台格子不够时后面的货**顺延到下次访**重试，不再硬塞。原生柜台没有翻页功能、商人带太多会卡到页外是游戏本体限制，模组侧只能靠优先级+顺延缓解，无法替原生加翻页。
- 文案写清两条货单（中英同步）：预约 8848＝明天带捕蛇夹、蛇笼、玉米喷涂枪各一件，不带蛇蛋，送达后才进 5 天冷却；每周三日常＝公母眼镜蛇、强化蛋、王蛇货、清洁补给，没笼时额外带一个蛇笼，不占电话冷却。对话尾提示与电话簿口径一致。
- 商人集成套件 250 断言（+8）：保底笼上架/有笼抑制/单格柜台笼优先/工具顺延/全 blocked 笼不记收据/日常访不烧冷却/仅设备单送达烧冷却。无新增物品 ID；仅新增一个周访保底笼标记键（snake_ranch.merchant.stock_cage_weekly），旧档缺失视为未供，直接兼容。

- Rancher-start cage guarantee: while the player owns no snake cage at all, the Wednesday walk-in (including the first visit) places one cage FIRST; once any cage is owned the natural stock returns to its usual line and 8848 stays the express equipment channel. No more "day 17 and still no cage for sale".
- Fixed "the NPC keeps coming and eats the phone cooldown": the old code called UseService() on EVERY arrival (including the Wednesday walk-in), re-arming the 5-day 8848 cooldown each week so booking was impossible forever. Now only a delivered booked equipment run starts the cooldown; walk-ins and the first number handout never burn it.
- Stock priority: on equipment runs the cage precedes the catching tool/spray gun; on walk-ins the guarantee cage precedes snakes/eggs/king goods/cleaners. When the counter lacks cells, later goods defer to the next visit instead of being forced in. The native counter has no pagination and an over-loaded merchant spills off-page - a base-game limit the mod can only mitigate with priority + deferral, not fix by adding pages.
- Both stock lists are now spelled out (zh/en together): booking 8848 = tomorrow's one catching tool, one cage, one corn spray gun, no snakes or eggs, cooldown only after delivery; Wednesday walk-in = male/female cobras, gene eggs, king cobra goods, cleaning supplies, plus one cage if you own none, never burning the phone cooldown. Dialogue tails and the phone book agree.
- Merchant integration suite at 250 assertions (+8): guarantee placement / owned-cage suppression / cage-first on a one-slot counter / tool deferral / fully-blocked cage records no receipt / walk-in never arms cooldown / only the equipment run arms it. No new item IDs; one new weekly-guarantee marker key (snake_ranch.merchant.stock_cage_weekly), absent in old saves and treated as unsupplied, so they load compatibly.

## 0.7.5 修复蛇蜕拖拽合并无效与 0 耐久空壳残余 / Fix shed drag-merge and 0-durability ghost packs

- 修复蛇蜕滤材拖拽合并始终无效：旧门禁要求合并双方都挂在容器里，而真实拖拽时源物品被拿在光标/手上（parentInventory 为空），于是每次拖拽都被判"不可合并"。0.6.9 的跨容器修复只补了回调存在性，没碰这条门禁，所以实机依旧合不了。现在源允许手持，目标仍需在容器内；锁定/装机的源依旧拒绝。
- 修复"没碰却自动合并、留下 0/200 空壳"：合并改为可验证事务——先把源从原容器 Expel 再 Destroy，销毁后校验源确实不再被任何容器跟踪；若原生 Destroy 只打标记仍把源留在格子里，就清掉残余槽位而保留合并结果；若销毁失败则全量回滚双方耐久与收据，并把源放回原容器，绝不落半状态。
- 新增空壳清理：读档与每次合并后扫一遍，剩余 0g 且挂在储藏格/笼格/手持的蛇蜕直接移除（0g 无材料无收据义务）；装在机器滤窗里的空袋交给原生滤材逻辑，不动。旧档里遗留的 0/200 残余读档即清。
- 蛇蜕集成套件补 9 条定向断言（手持源合并、Destroy 残留清槽、销毁失败回原位、读档扫空壳、滤窗空袋保留、清理幂等），套件重新纳入门禁；无新增本地化键、无新增物品 ID、无存档键结构改动。

- Fixed shed-filter drag-merge never firing: the old gate required BOTH packs to sit in a container, but a real drag holds the source in the cursor (parentInventory null), so every drag was rejected as "cannot merge". The 0.6.9 cross-container fix only restored callback presence and never touched this gate, hence no effect in game. The source may now be held; the target must still be stored; locked/installed sources are still refused.
- Fixed "untouched packs auto-merge and leave a 0/200 ghost": merging is now a verifiable transaction - the source is Expelled from its container before Destroy, then verified to be untracked everywhere; if native Destroy only flags the item and leaves it listed, the residual slot is cleared while keeping the merge; if Destroy fails, both durabilities and receipts roll back fully and the source is put back into its original container. No half-states.
- Added an empty-pack sweep: on load and after every merge, shed packs with 0g remaining that sit in storage/cage grids or in hand are removed (0g carries no material and no receipt obligation); empty bags installed in a machine filter window are left to native filter handling. Legacy 0/200 ghosts in old saves are cleared on load.
- The shed integration suite gains 9 targeted assertions (held-source merge, residual-slot clearing, destroy-failure put-back, load sweep, machine-window bag preserved, idempotent sweep) and is back in the gate; no new localization keys, no new item IDs, no save-key structure changes.

## 0.7.4 修复武器静养恢复被误判中断 / Fix weapon isolation-rest false interruption

- 修复武器形态静养恢复"单独笼养、饱腹充足却永远 0/2、挂着被取出笼中断"的问题：游戏内部同笼重插、读档重建、形状校验都会走 RemoveParent，旧逻辑一触发就立即当作"被取出笼"把夜数归零并禁止当日计数，若该内部重插每夜发生就永远无法累积。
- 取出改为对账判定：RemoveParent 只记一笔 pending（原笼+当日）；回到原笼撤销 pending 不算中断；进别的笼按"已移动到另一笼"中断；夜结算/读档对账时真不在笼内才写"被取出笼，静养中断"。玩家真取出依旧中断，误报消失。
- 新增纯规则层 SnakeRestRules 与 9 条定向断言；无新增本地化键、无新增物品 ID；仅新增两个静养对账标记键（SR_REST_REMOVED_CYCLE / SR_REST_REMOVED_FROM），旧档缺失视为无 pending，直接兼容。

- Fixed weapon-form isolation rest showing "taken out of the cage, rest interrupted" and staying at 0/2 forever even when the snake sits alone in a cage with enough fullness: native internal same-cage re-inserts, load rebuilds and shape validation all call RemoveParent, and the old logic treated every such call as a real removal, zeroing the night counter and blocking that cycle; a nightly internal re-insert therefore never let progress accumulate.
- Removal is now reconciled instead of instant: RemoveParent only records a pending removal (source cage + day); returning to the same cage clears it without a break; entering a different cage breaks as "moved to another cage"; only when the snake is genuinely out of any cage at the night/load reconciliation point is "taken out of the cage" written. Real player removals still interrupt; false positives are gone.
- Added the pure rule layer SnakeRestRules with 9 targeted assertions; no new localization keys and no new item IDs; only two new rest reconciliation marker keys (SR_REST_REMOVED_CYCLE / SR_REST_REMOVED_FROM), which old saves lack and are treated as no pending removal, so they load compatibly.

## 0.7.3 鼠蛇四阶像素图标与三吱儿奢侈品蛋 / Rat-snake stage icons and the Three Squeaks luxury egg

- 挑战者鼠四阶段换上专属像素图标，尺寸逐阶放大：32×32 → 64×48 → 80×64 → 96×64，终阶与王蛇最大档同尺寸；第四阶改用新的盘蛇图。硬边平涂工艺，与原生蛇贴图同质感，不再发糊。
- 鼠蛇（四阶）每 2 天产一枚「三吱儿」：占 2×2 共四格、售价 666、奢侈品食物（可食用、营养 120）、不能孵化；玻璃壳内蜷着一只熟睡小鼠。
- 三吱儿文案取自同名传闻菜：剥壳吱一声、蘸料吱一声、入口吱一声，然后就被吃掉了；小鼠随外壳破开而死去，卵碰到什么隐约还有点感觉。
- 产蛋走夜结算：成为鼠蛇的首夜只起钟不产蛋；笼内有空位放笼内，否则放玩家柜台/仓库；满位不丢蛋、下一夜重试；离线不补产。
- 中英文案同步；新增物品 ID `snake_ranch.rat_snake_egg` 与一个产蛋日标记键；未改动原有存档键结构或原生容器容量。

- The four challenger-rat stages now use dedicated pixel icons that grow step by step: 32×32 → 64×48 → 80×64 → 96×64, with the final stage matching the king cobra's largest tier; stage four uses the new coiled-serpent art. Hard-edged flat coloring matches the native snake textures instead of looking blurry.
- A rat-snake (stage four) lays one "Three Squeaks" egg every 2 days: it occupies 2×2 (four cells), sells for 666, is a luxury food (edible, nutrition 120) and cannot hatch; a sleeping mouse is curled inside the glass shell.
- The Three Squeaks text follows the rumored dish of the same name: one squeak as the shell is peeled, one as it is dipped, one as it is placed in the mouth - then it is eaten; the mouse dies as the shell breaks, and the egg still faintly feels whatever touches it.
- Laying runs in the night settlement: the first night as a rat-snake only starts the clock; the egg goes into the cage if there is space, otherwise onto the player's counter/warehouse; a full inventory never loses the egg and retries next night; offline time is not caught up.
- Chinese and English text updated together; one new item ID `snake_ranch.rat_snake_egg` and one lay-day marker key were added; no existing save-key structure or native container capacity was changed.

## 0.7.2 蛇夫人每周三固定上门、电话冷却 5 天 / Snake lady fixed Wednesday visits, 5-day phone cooldown

- 蛇夫人现在每周三固定上门（游戏内星期以 dayCounter%7 计，周日=0），不打电话也会来，不受电话冷却限制；上门走自然来访全货单：公母眼镜蛇、强化蛋、王蛇货与清洁补给照常补货。
- 修复强化蛋断供：0.6.3 起首访后自动到访被永久关闭，强化蛋的每 7 天补批逻辑再也走不到；周三固定访恢复自然货单后，强化蛋按原节奏续供。
- 电话 8848 保留为加急预约通道：冷却从 7 天缩短到 5 天，旧存档里剩余的 7 天冷却读档时自动钳到 5 天；预约明天上门的设备单（捕蛇夹、蛇笼、玉米喷涂枪）不变。
- 电话簿里冷却中的号码直接显示剩余天数「蛇夫人 8848（冷却 X 天）」，冷却结束恢复原名；预约日恰好撞上每周三时只来一次，按全货单算。
- 蛇夫人对话尾部新增每周三上门提示；禁用模组会清掉周访排队与标记，重新启用后自动回到周三节奏。
- 中英文案同步；未改动物品 ID、存档键结构或原生容器容量（仅新增一个周访日期标记键）。

- The snake lady now keeps a fixed Wednesday counter visit (the game's week is dayCounter%7 with Sunday=0). She comes without a phone call and ignores the phone cooldown, walking the full natural stock: cobra pair, gene eggs, king cobra goods and cleaning supplies restock as usual.
- Fixed the gene-egg drought: since 0.6.3 the first visit permanently disabled automatic returns, so the seven-day egg restock branch was unreachable. The Wednesday visit restores the natural stock line and eggs resume their original cadence.
- 8848 stays an express booking channel: cooldown shortened from 7 to 5 days, and a legacy seven-day remainder clamps to 5 on load. Booking tomorrow's equipment run (catching tool, cage, corn spray gun) is unchanged.
- The phone book now shows the remaining cooldown right in the contact name, "Snake Lady 8848 (cooldown Xd)", restoring the plain name when ready. If a booking lands on the same Wednesday she still only turns up once, with the full stock.
- Her visit dialogue now mentions the Wednesday schedule. Disabling the mod clears the weekly queue and marker; re-enabling it resumes the Wednesday grid automatically.
- Chinese and English text updated together. No item IDs, save-key structure or native container capacities were changed (one new weekly due-date marker key only).

## 0.7.1 挑战者鼠登阶与鼠蛇、蛇吞鼠反咬 / Challenger rat ascension, rat-snake and counter-bite

- 挑战者鼠每次从护蛋蛇口中活下来并吞下蛋，算一次登阶成功：升一阶，寿命翻倍。四阶寿命依次为 2×、4×、8×、16×，年龄地板同步下移，读档后不会因原寿命判定老死。
- 每阶有独立名称与描述。第三阶描述为“半鼠半蛇，盘踞笼中。它抬起头看着你：你才是挑战者！”；第四阶为“它是三次挑战登阶之鼠，得到了祂的注视。”
- 第四阶成为鼠蛇：不死（免疫老死、饿死与伤害），不可繁殖、不产毒，不再作为猎物被蛇捕食；只作为收藏品，按阶加价 +0% / +25% / +50% / +150%，武器形态同样生效。
- 蛇吞鼠时有 10% 概率被反咬，损失 25% 最大生命，生命钳在最低 1 点，永不致死；夜报与蛇的提示会记录累计被咬次数。鼠蛇不参与该判定。
- 旧存档中已获得护蛋强档的挑战者鼠，读档时按第一阶补齐寿命与阶段标签，不迁移、不删除、不重置原有体重和价格加成。
- 本版中英文案同步；暂用原生鼠图标占位，四阶段专属像素图标后续补齐。未改动物品 ID、存档键或原生容器容量。

- Each successful egg-guard survival (the rat survives the guard snake and swallows the egg) advances one stage and doubles lifespan: 2×, 4×, 8× and 16× at the four stages, with the age floor lowered in step so a loaded save cannot age the rat out.
- Every stage has its own name and description. Stage three reads "Half rat, half snake, coiled in the cage. It lifts its head and looks at you: you are the challenger!"; stage four reads "It is the rat that climbed three challenges, and it has received His gaze."
- Stage four becomes a rat-snake: immortal (immune to old age, starvation and damage), unable to breed or produce venom, and no longer valid prey for snakes. It is a collectible only, priced at +0% / +25% / +50% / +150% by stage, weapon forms included.
- When a snake swallows a rat there is a 10% chance of a counter-bite: 25% of max health is lost, health is clamped to at least 1 and the snake can never die from it. The night report and the snake's tooltip track the cumulative bite count. Rat-snakes are excluded.
- Existing saves with a strong-tier challenger rat are upgraded to stage one on load, with lifespan and stage tags filled in; weight and price bonuses are not migrated, deleted or reset.
- Chinese and English text updated together. The native rat icon is used as a placeholder and the four stage pixel icons will follow. No item IDs, save keys or native container capacities were changed.

## 0.7.0 眼镜蛇/王蛇纯色变异、日常蜷缩与概率设置 / Cobra & king pure-color morphs, idle coil and probability settings

- 眼镜蛇/王蛇新增纯色变异：参照真实蛇色的独立七色——赤红、白、墨黑、橄榄青绿、金黄、灰银、橄榄褐（与玉米品系完全解耦，玉米不变）。新刷始祖默认 1/10 概率携带，颜色均匀 1/7；蛇蛋与子代按单基因遗传继承显色。
- 带色个体只按稀有度加售价（白化最贵，橄榄褐/灰银平价），不含任何鉴定/伪造/线索机制；武器形态同样享溢价。旧存档无颜色标签视为原色，不迁移、不删档。
- 日常蜷缩：眼镜蛇/王蛇在普通日常状态约 50/50 蜷缩打盹（复用静养球贴图，纯外观），与饱腹/消化、功能静养、蛇杖、交配蛇球互不重叠；提示文案明确区分“蜷着打盹”与隔离静养。
- 游戏选项新增三个概率滑条，软下限即基线、只能上调：拾荒捡蛋 1–20%（默认 1%）、变异倍率 ×1–×50（默认 ×1）、纯色变异 1–100%（默认 10%），防止调太低玩不到内容。
- 中英文案同步；本版未改动物品 ID、存档键或原生容器容量。

- Cobras and king cobras gain solid pure-color morphs in a realistic, cobra-specific seven-color palette — Crimson, White, Black, Olive Teal, Gold, Silver and Olive Brown — fully decoupled from the corn morph system (corn is unchanged). Newly spawned founders carry a morph at a default 1/10 chance with a uniform 1/7 color pick; eggs and offspring inherit the color through single-locus inheritance.
- Colored snakes only receive a rarity price premium (albino dearest; olive brown / silver common), with no inspection, forgery or clue mechanics; weapon forms share the premium. Old saves without a color tag stay wild-type — no migration, no save deletion.
- Idle coil: in the plain daily state cobras/king cobras curl up dozing about 50/50 (reusing the rest-ball art, cosmetic only). It never overlaps fed/digestion, functional isolation rest, snake staffs or mating balls; the tooltip distinguishes "curled up dozing" from isolation rest.
- Three new probability sliders in game options, each with a soft floor at the baseline and upward-only adjustment: scavenger egg 1–20% (default 1%), mutation multiplier ×1–×50 (default ×1), pure-color morph 1–100% (default 10%), so the content cannot be tuned out of reach.
- Chinese and English text updated together. No item IDs, save keys or native container capacities were changed.

本版完成离线构建与包门禁；助手未启动游戏、未读写真实存档、未上传工坊，实机验证由作者本地进行。

This release completed offline build and package gates; no game launch, real-save access or workshop upload was performed by the assistant, and live verification is left to the author's local test.

## 0.6.9-r1 护蛋挑战者、蛇蜕跨容器合并与蛇毒收购 / Egg-guard challenger, cross-container shed merge and venom purchase

- 护蛋挑战同一蛇笼每夜最多触发一次；第一只老鼠触发后，当夜其余老鼠不再挑战。普通未受保护受精蛋被偷只触发弱档“偷蛋鼠”，护蛋成功才给强档“挑战者·吞蛋鼠”。
- 弱档“偷蛋鼠”为 +10% 体重（最多 +50 克）、+10% 基础售价（最多 +50）；强档“挑战者·吞蛋鼠”为 +50% 体重（最多 +300 克）、+50% 基础售价。弱档可升强档，强档不被弱档覆盖。
- 护蛋失败夜报会写明“护蛋蛇咬死了一只偷蛋鼠；这枚蛋没有丢”，被咬死老鼠的尸检说明写“死于护蛋蛇”；护蛋成功夜报写明老鼠撑过护蛋蛇并吞下蛇蛋。
- 两包蛇蜕不再要求放在同一个容器，直接拖到另一包蛇蜕上即可合并剩余量；跨容器合并失败会回滚双方耐久和收据，不重复加量。
- 蛇夫人不再被低层市民的随机禁运标志挡住蛇毒；她的收购预算下限提高到 900，旧存档里的蛇夫人蓝图读档后会修正。
- 启动路径加固：`OnEnable`、物品注册以及商人和升级注册都会先检查 `PlayerStore.HasInstance`，无存档或主菜单阶段不会提前创建 `PlayerStore`。
- 本地化初始化修复：玉米品类注册延迟到游戏内首次创建玉米特征时进行，`OnEnable` 不再触发 `ItemConditionList` 静态构造，避免本地化未就绪时打断主菜单语言下拉与读档面板接线。
- 以上中英文文案同步；本版未改动物品 ID、存档键或原生容器容量。

- Egg-guard challenges trigger at most once per cage per night: after the first rat, the rest do not challenge that night. An ordinary unprotected fertile egg only produces the weak “Egg Thief” rat; the strong “Challenger” rat is earned only by surviving the guard snake.
- Weak “Egg Thief”: +10% weight (max +50 g), +10% base price (max +50). Strong “Challenger”: +50% weight (max +300 g), +50% base price. Weak can upgrade to strong; strong is never overwritten by weak.
- A failed guard writes that the guard snake killed one egg-thief rat and the egg was not lost; the dead rat's tooltip says it died to the guard snake. A successful guard writes that the rat survived the guard snake and swallowed the egg.
- Two shed packs no longer need to be in the same container; drag one onto the other to combine remaining material. A failed cross-container merge rolls back both durability and receipts without adding material twice.
- The Snake Lady is no longer blocked from buying venom by the lower-level citizen's random contraband refusal. Her purchase budget floor is now 900, and old saved Snake Lady blueprints are repaired on load.
- Startup hardening: `OnEnable`, item registration, merchant registration and upgrade registration all check `PlayerStore.HasInstance` first, so no `PlayerStore` is created while no save or main menu is active.
- Localization-init fix: corn category registration is deferred to the first in-game corn feature creation; `OnEnable` no longer triggers the `ItemConditionList` static initializer, so localization startup is not interrupted before the main-menu locale dropdown and load panel wire up.
- Chinese and English text updated together. No item IDs, save keys or native container capacities were changed.

本版完成离线构建、包门禁和上传前检查；没有启动游戏、安装、读写真实存档或上传，不声称已实机验证。

This release completed offline build, package gates and pre-upload checks; no game launch, install, real-save access or upload was performed, and no live-game verification is claimed.

## 0.6.8-r3 加载保护与测试箱 / Load guard and test kit

- 0.6.8 在 `OnEnable` 里提前访问 `PlayerStore.Instance`，无存档时会新建 `PlayerStore` 并碰到尚未生成的场景单例，异常会让整个 mod 加载失败。本版只在 store 未就绪时跳过电话安装，读档后再补；不改物品 ID、笼具标签、存档键，也不删除或重建任何蛇/笼/物品。
- 测试箱改用蛇笼贴图和蛇笼同尺寸的外部占地（原生 4×3），不再固定 2×2；已有测试箱读档时同步修复图标与外部尺寸，容器内部网格、容量和内容不变。
- 0.6.8 evaluated `PlayerStore.Instance` too early in `OnEnable`; with no store it constructed a new `PlayerStore` and touched scene singletons that were not ready, disabling the whole mod. This release defers phone installation until the store exists; item IDs, cage tags, save keys and existing snakes/cages/items are untouched.
- The test kit now uses the snake-cage icon and the native cage's 4×3 footprint instead of a fixed 2×2. Existing saved kits are repaired on load without changing the internal grid, capacity or contents.

## 蛇夫人电话 / Snake Lady phone


- 旧存档里已经见过蛇夫人、但没有拿到号码的，读档时会自动补上 8848，不再出现“她不再来、电话也打不通”的死锁。
- 预约明天到店不再同时压 7 天冷却；重复拨号只确认已有预约，不会重复排队。
- 预约没能排上时会明确说“今天没记上”，不再把系统失败显示成玩家拒绝。
- 她已经在店里时拨 8848，会提示当面说，不会进入无人接听的电话对话。

- Saves that met the Snake Lady before the number key existed now receive 8848 on load, fixing the old deadlock where she never returned and the line never answered.
- Booking tomorrow no longer arms the 7-day cooldown; a repeat call confirms the existing booking without adding another visit.
- If the booking cannot be queued, the call says so instead of showing the player's declined option.
- Calling 8848 while she is in the store tells you to speak in person instead of entering the no-answer dialogue.

## 柜台与鉴定 / Counter and inspection

- 原版只收购的同行（BUY）不再被误接成玉米卖家，也不会在柜台摆出无法拿取、购买的蛇；玉米只由破产上层人士一名卖家供货，拾荒者不再携带。
- 玉米在创建和读档时挂载可鉴定标记；原生放大镜可对合法卖家柜台上的玉米打开四类线索与单选质询。自有玉米仍只允许观察，未拥有柜台玉米只在当前 SELL/SELLNBUY 卖家下可检查。

- The native BUY-only competitor is no longer treated as a corn seller and no longer displays snakes that cannot be taken or bought; corn is supplied only by the broke upper-level citizen, never by the inspectable scavenger.
- Corn is marked inspectable on creation and load. The native magnifier can open the four-clue, single-choice inspection on corn at an eligible seller's counter. Owned corn remains observation-only; unowned counter corn opens only while a real SELL/SELLNBUY seller is current.

## 鉴定图例 / Appraisal plate

- 放大镜窗口新增“查看图例”，打开静态图例：100%/95%/80%/50% 纯度对照，以及假货的色块、接缝、边缘斑点、发灰四类缺陷；不读取当前这条蛇的线索，不会提前泄题。
- 《养蛇手册》新增第 10 页《玉米蛇鉴定图例》，同一张图例随手册携带，随时可查。图例只说明表面证据，不改变线索概率、质询判定、价格或供货。
- The inspection window now has **View plate**, opening a static reference: 100%/95%/80%/50% purity scales plus the four fake defects (colour patch, spray seam, edge specks, dull grey patch). It never reads the current snake's clue mask, so it cannot leak the answer.
- The handbook now carries the same plate as chapter 10. The plate teaches surface evidence only; it does not change clue odds, challenge resolution, prices, or supply.

## 蛇夫人货单与喷枪 / Snake Lady stock and spray gun

- 电话邀约与自然来访按具体到访实例分流：电话货单提供工具、笼与喷涂枪，自然货单提供蛇、蛋与清洁剂；同一次到访不串货，离场或换客后的旧回调不落位。
- 喷枪未售出时保留到下一次电话邀约，不叠加到下一周；真实购买后同周不补，下一次电话再按周供货。价格、尺寸和解锁条件不变。

- Phone-booked and natural visits use separate stock lists per visit instance: phone visits bring tools, cages and the spray gun; natural visits bring snakes, eggs and cleaning supplies. Stock does not cross between them, and stale callbacks after leaving or switching customers do not place items.
- An unsold spray gun is retained for the next phone visit and never stacks into a later week. A real purchase is not replaced in the same week; the next phone visit supplies the next weekly gun. Price, size and unlock conditions are unchanged.

## 成年玉米蛇分类 / Adult corn classification

- 成年、存活的玉米蛇补 `LUXURY_ITEM` + `FOOD` 两个物品类型标签；幼体不加，死体不加。`FOOD` 只作为分类标签，不接 `FoodItemHelper`、不变成可食用或热量逻辑。
- 旧存档中的成年玉米蛇在读档时补标签；卖家假库存继续只生成成年蛇。

- Adult, living corn snakes receive the `LUXURY_ITEM` and `FOOD` item-type tags; juveniles and dead snakes do not. `FOOD` is a classification tag only: it is not passed to `FoodItemHelper` and does not become edible or provide calories.
- Adult corn snakes in old saves receive the tags on load. Seller forgery stock remains adult-only.

本版完成离线构建、包门禁和上传前检查；没有启动游戏、安装、读写真实存档或上传，不声称已实机验证。历史版本说明保留在下方。

This release completed offline build, package gates and pre-upload checks; no game launch, install, real-save access or upload was performed, and no live-game verification is claimed. Historical release notes remain below.

---

# Snake Ranch 0.6.4 — 夜结/配对修复与玉米喷涂伪造 / Night, mating and corn spray-forgery

## 多蛇夜结与配对 / Multi-snake night and mating

- 修复多条蛇同处容器时，不同个体因物品ID相同而被误判为已结算的问题；交配组预标、晚到回调与实际个体结算分开处理，保留同夜重复回调防重。
- 修复真实补食后仍残留饥饿标记，导致同夜饭后拒绝配对的问题；真实饥饿、能量与健康资格继续生效。零/部分鼠源储备仍不拒配。
- 拒配提示现在区分缺同种合格雄蛇、未成年、健康/伤势、饱食、生育力、恢复期和实际能量预算不足；雄蛇不会被提示缺雄蛇。
- 保留0.6.3的护蛋、搜查、电话、幼鼠显色及交配优先规则；不重置旧蛇、不改原生物品ID、不追补冻结期间的历史收益。

Fixes reproduced missed night updates when different snakes share an item ID. Mating preparation and late callbacks no longer substitute for another individual's update; repeated callbacks remain guarded against duplicate effects.
Clears a stale starvation flag after actual feeding, allowing eligible snakes to mate that night. Genuine starvation, energy and health checks still apply; zero or partial mouse-source reserves do not veto mating.
Mating messages now explain missing eligible same-species males, immaturity, health/injury, hunger, fertility, recovery and insufficient energy. Males are not incorrectly told they lack a male partner. Existing 0.6.3 features, saved progress and offspring plans are retained; no historical income is backfilled.

## 玉米喷涂枪与伪造 / Corn Spray Gun and forgery

- 蛇夫人每次电话邀约补一把「玉米喷涂枪」，450信用点；未售出的保留到下一次电话邀约，不叠加到下週，自然来访不摆枪。枪外占2×2、内舱4×3，持有或装备后使用。
- 只把一件色素材料放进枪内；枪不能放进自身；整堆材料必须先拆分，每次喷涂只耗一件。七种植物色素分别对应七种玉米主色；黑色色素须使用一份满耐久200/200的新试剂。
- 目标限自有、成年、存活且未曝光的玉米；只拒绝“本来就是目标色的完美纯色”，真混色喷自己的主色允许。失败不耗材并完整回滚；仅在实际耗材后提交成功，带重入保护。
- 涂层只改公开外观，不改真实遗传。未曝光玉米公开身份统一宣称100%纯色；曝光后名称、身份与价格回真实颜色纯度，表面涂层外观保留。标签机不能清除曝光，持久曝光标签为权威。
- 原生放大镜检查摊位玉米：四类线索（色系、花纹、缺色/漏喷、品质表现）单选质询；正确按原生15%折让，错误按当前客户阵营扣2。玩家把假货卖给买家被识破仍走原生-4，不额外追责或二次扣声望；自有玉米只能观察。放大镜以 `INSPECTABLE_TAG` 为目标条件，玉米在创建和读档时自动挂载；未拥有的柜台玉米只允许真实 SELL/SELLNBUY 当前卖家打开检查。
- 只有破产上层人士 `brokeUpperLevel` 每日补一只玉米，假货占比 25%；拾荒者 `scavInspectable` 与原版只收购的 `merchant` 同行都不是卖家，不会获得玉米库存。

- The Snake Lady brings one Corn Spray Gun on each phone-booked visit for 450 credits; unsold guns are kept for the next phone visit, never stacked into later weeks, and natural visits never display a gun. It occupies 2×2 outside and holds a 4×3 inner bay; hold or equip it to use.
- Put one pigment material inside the gun; the gun cannot be placed inside itself. Split a whole stack first; each spray consumes one item. Seven plant pigments map to the seven corn base colours; black pigment requires one fresh reagent at full 200/200 durability.
- Targets are limited to owned, adult, living and unexposed corn snakes. Only a genuine perfect pure coat already in the target colour is refused; a true mixed coat may be painted its own dominant colour. Failure consumes nothing and rolls back; success commits only after the material is actually consumed, with re-entry guards.
- The coat changes public appearance only, never true genetics. Unexposed corn publicly claims 100% pure colour; after exposure its name, identity and price return to the true colour purity while the surface coat stays. The labeler cannot clear exposure; the persistent exposure tag is authoritative.
- Inspect booth corn with the native magnifier: four clues (colour family, pattern, missing colour/overspray, quality) and one single-choice confrontation. A correct challenge applies the native 15% discount; a wrong one costs 2 with the current customer faction. Selling a fake to a buyer who sees through it still uses the native -4; there is no extra after-sale penalty or second reputation loss. Owned corn is observation-only. The magnifier targets `INSPECTABLE_TAG`; corn is tagged at creation and on load. Unowned counter corn can open inspection only while a real SELL/SELLNBUY seller is current.
- Only the broke upper-level citizen `brokeUpperLevel` restocks one corn per day with a 25% fake share; the inspectable scavenger `scavInspectable` and the native BUY-only `merchant` competitor are not sellers and never receive corn stock.

已知边界：发生额外扣耗或原生处理异常时，失败个体当夜可能不补算；两次夜结回执都写失败后读档，仍可能重复推进。本版未重写这些异常事务。离线回归与最终包检查通过不代表玩家现场已恢复；Unity/UI、真实载体、喷枪图标/容器、放大镜检查与ES3存读档仍待实机验证。测试礼包默认关闭，更新不重置用户设置。
Known limits: a failed extra charge or native update may leave that individual's night incomplete; reloading after both receipt writes fail may replay the native update. These exceptional paths remain unresolved. Offline regression and package checks do not establish live recovery; Unity/UI, real carriers, the spray-gun icon/container, magnifier inspection and ES3 save round-trips remain unverified. Test gifts stay off by default and saved settings are preserved.

---

# Snake Ranch 0.6.3 — 搜查、电话与养殖更新 / Inspection, phone and husbandry update

- 护蛋喂食补丁匹配失败时记录警告并放行原始流程，避免因此停用整个模组；匹配不上时护蛋功能可能不生效。
- 店内有可触发的活成年蛇时，接受搜查先选择告知或沉默。告知保住蛇、检查继续，安保声望另加1，原生清白奖励按原条件结算；沉默保留咬人、中止搜查及蛇伤亡。眼镜蛇救治可当场取尸体胆，王蛇救治消耗库存胆；救治不加声望，拒绝仍扣3。拿不到表态窗口12秒后按沉默继续。
- 蛇夫人首访交号码8848，之后通过原生电话约次日到店；通话和到访均重置7天冷却，当天在店不接。首访后停自动周访，补给随预约到访带来。
- 玉米蛇显色餐只吃占格不超过2格的幼鼠，不再与蛇体重比较或要求鼠重至少5g；仍每夜至多1只、养护+5、上限100。肉类/饲料正常成长和繁殖。
- 鼠源储备不再阻止配对，开窝仅按实际额度部分预留；孕期真正饥饿时每夜扣最大生命10%，每窝最多3夜。其他状态与饱食资格保留。
- 在动物日结边界隔离并记录异常；若外层夜结仍抛异常，再尝试补算当夜未收到回执的自有蛇。新增animal.failed与snake.recovered诊断。已在单只回调内部捕获的异常不因此自动重跑。

Offline checks are recorded separately from live-game testing. Egg-guard matching failures now log and preserve the original feeding flow so they do not disable the entire mod; egg guarding may be unavailable on an unmatched feeding implementation.

With an eligible live adult snake in the shop, accepting an inspection offers declare/silence. Declaring protects snakes and lets the search continue: +1 security reputation, with the native clean-search reward still subject to native conditions. Silence keeps the bite, aborted search and species-specific injury/death. Cobra aid can use its corpse gallbladder; King aid consumes a stocked one. Aid gives no reputation; refusing still costs 3. A blocked prompt falls back to silence after 12 seconds.

The Lady gives you 8848 on her first visit. Call through the native phone to book tomorrow; calls and arrivals reset a seven-day cooldown. She does not answer on a day she is at the shop. Automatic weekly visits stop after her first arrival; supplies come with booked visits.

Corn snakes only take juvenile rats occupying at most two cells for colour care, regardless of snake weight or a 5g rat minimum. Still at most one per night, +5 care, cap 100; meat/feed supports normal growth and breeding. Rat-source shortages no longer veto mating: reserve only the available portion. True starvation during pregnancy costs 10% maximum health per night, capped at three nights per litter; other eligibility checks remain.

Animal day-end failures are contained and logged. If the outer night still throws, recovery attempts owned snakes without that night's receipt. A failure already caught inside one callback does not trigger this sweep. New animal.failed/snake.recovered counters help diagnose it. This does not establish that every growth stall or native MoveAround exception is fixed.

验证边界：本次集中验收为离线构建、测试、原生契约/扫描及包核验；没有启动游戏、安装、修改真实存档或上传。护蛋旧版本本机加载证据不代表本合并包实机通过。搜查UI时序、电话簿/冷却/存读档、幼鼠真实占格和夜结补算仍待集中实机回归。
Live Unity/UI, save round-trips, rat cell sizes and real night recovery remain unverified for this integrated package. No game launch, install, save change or upload was performed.

---

# Snake Ranch 0.6.2 — 花色与测试补发修复 / Colour and test-key fixes

修复玉米蛇读档及模组禁用再启用后退回红色母图的显示路径，按已保存的配色、纯度与种子恢复贴图，不重新随机。玉米蛇肉类/饲料可正常成长和繁殖；鼠餐仅改善花色鲜艳度与收藏估值。
模组设置新增测试礼包补发键，默认F9，可改键；也可在游戏选项中录键。输入文字、设置、读档和睡眠期间屏蔽补发；原生调试菜单入口保留，测试开关仍默认关闭。背包放不下时清理本次未落地临时实例，腾位后可以重试，不会自动无限补发。
包含读档夜结接线与回执加固、有界夜结诊断。部分蛇停长、蛇笼拖拽停住及英文无效句柄反馈仍未确认根因，不能宣称本版已修复。未启动游戏/Unity或验证真实UI与ES3；当前实际包权限扫描以本次验收为准。

Restores saved corn-snake colours after loading or disabling/re-enabling the mod, without rerolling genes, purity or pattern seeds. Meat/feed supports normal corn growth and breeding; optional mice improve vibrancy and collectible value.
Adds a configurable test-kit reissue key (default F9) and native key capture. Text entry, settings, loading and sleep block the shortcut. Native debug-menu actions remain, and testing stays off by default. Unplaced temporary kit instances are cleaned up; free space and explicitly retry. No automatic endless reissue.
Includes load/night callback and receipt hardening plus bounded diagnostics. Growth-stop, cage-drag and invalid-handle reports remain unresolved; no live Unity/UI/ES3 test has been performed.

---

# Snake Ranch 0.6.1：王蛇精华与反馈修复

成年王蛇实际消化玉米蛇每累计500g产一份王蛇精华，余量跨餐；仅真实成功捕食的玉米来源计量，花色不增加营养或产量。默认宠物保护保留，须主动解除才能被正常合格捕食。满库存延期投递，确认后才可使用。

拖给自有、符合原生配种资格的成年鼠，随机永久获得一项：免疫相对当前+50–100%（上限100）；每夜饥饿消耗降低25–40%；仅母鼠窝产+25–33%；仅公鼠每夜最大配对次数+1（同库存至少4只成年母鼠才入池）。合格候选等概率；成功使用保存结果，终身一次、不遗传、不延寿。失败或重复不耗材，旧生育力/生命上限/免疫记录及加成保留。代谢并非消化加速，种公不保证每晚多一胎，窝产仍有随机波动并需要足够空间。

同时包含：毒液无变化时不反复执行库存UI校验、玉米蛇物种描述纠正、读档夜结接线与回执防御加固。不宣称MoveAround或玩家停长问题已全面解决。蛇笼当前扩建上限16×30；工坊正文同步更新对应宣传说明。

Adult King snakes produce one King essence per 500g of corn snake actually digested, retaining leftovers across meals. Coat colour adds no yield. Pets remain protected until explicitly unprotected. Full inventories defer delivery; unconfirmed essence cannot be used.

Use on an owned adult rat eligible for native mating: one permanent random effect—immunity +50–100% of its current value (cap 100), nightly hunger consumption -25–40%, female litter size +25–33%, or male nightly mating limit +1 (enters the pool only with at least four adult females in the same inventory). Eligible choices are equally likely. Successful use saves the result; once per life, not inherited, no extra lifespan. Failed or repeated use consumes nothing. Legacy fertility/health/immunity enhancements remain. Lower hunger consumption is not faster digestion; stud capacity guarantees no extra pregnancy, and litter output varies and requires space.

Also includes venom refresh UI-work reduction when unchanged, the corn-species description fix, and defensive load/night callback and receipt handling. MoveAround and player growth-stop reports are not claimed fully resolved. The cage expansion limit is now 16×30; its Workshop description is updated accordingly.

完整中英离线验证；F9测试礼包默认关闭，不重置用户已保存设置。未启动游戏/Unity或验证真实UI、ES3与玩法；本地包零额外权限扫描通过不代表绝对安全或全模组兼容。以下0.6.0既有玩法继续保留。

Offline checks only; F9 gifts remain off by default without resetting saved settings. Live Unity/UI/ES3 and gameplay remain untested. A zero-capability scan is not an absolute safety or compatibility guarantee. Existing 0.6.0 features follow unchanged.

---

# Snake Ranch 0.6.0：配色繁育与养蛇开局

正常启用模组并解锁蛇夫人后，她每自然周供应公母玉米蛇各一条；无需开启测试或开发预览。未售出的个体会保留，不叠加囤货。玉米蛇吃肉类/饲料正常成长，活鼠只提升显色，不加快成长；默认开启宠物保护，主动解除后才可能被合格王蛇捕食。

- 玉米蛇有七种基础色与21种双色组合，公母和各成长阶段共用体型美术。主色占比按50—69%、70—89%、90—99%、100%分为低、中、高、完美四档；繁育提纯，稀有颜色另有估值加成。
- 花纹由配色、纯度与固定种子生成；蛋壳也显示胚胎配色，读档和孵化不重新抽取。群体交配增加纯色机会，只从父母已有颜色继承。
- 新增「养蛇人」开局：600信用点、原版养鼠人的基础物资，并解锁蛇夫人服务；不是自动赠送高级蛇笼的测试开局。
- 蛇夫人每七天补货六瓶原生四合一洗涤剂，未售库存不积压；每瓶为自有蛇笼补250清洁点，储备上限仍为2000。
- 蛇夫人对话提供「先看看商品」，可结束对话后购买。F9测试礼包默认关闭，只有主动开启测试选项才可领取。

新增玉米蛇、蛋、双蛇与群体交配美术已获本轮整合接受；真实Unity显示、UI布局及ES3存读档仍需实机验证。模组清单不申请额外能力权限；最终发布扫描以候选验收记录为准。

Normal enabled play unlocks corn snakes through the Snake Lady: one male and one female per calendar week, with unsold stock retained. No test or preview switch is needed. Meat/feed supports normal growth; optional mice improve coloration only. Pet protection starts on; explicitly unprotected corn snakes may become eligible King Cobra prey.

Seven base colours form 21 two-colour combinations. Main-colour purity has four tiers (50–69%, 70–89%, 90–99%, 100%); breeding purifies colours, while rare colours have a separate value premium. A saved seed fixes the pattern, and eggs display their inherited coat without rerolling on loading or hatching. Group mating improves pure-colour chances using the parents' existing colours.

The Snake Rancher start has 600 credits, the native rat rancher's basic supplies and unlocked Snake Lady services. She also stocks six native 4 In 1 Wash bottles every seven days; each adds 250 cleaning points to our cage, capped at 2000. Choose “Browse the goods” to end her dialogue. F9 test gifts remain off by default.

New corn-snake, egg and mating artwork is accepted for this integration. Live Unity/UI/ES3 behaviour remains untested; the final candidate's scan is the authority for permission validation.


---

以下为历史说明，当前玩法及获取方式以上述0.6.0说明为准。

历史版本说明 / Historical release notes (0.5.9 and earlier)

【0.5.9 反馈修复质量包】
读档保护：解码后逐条校验蛇对应的存档实例、物品ID、动物标记与库存可达性，异常时记录原因并拒绝该次重建，不再静默生成损坏蛇。白块：读档镜像朝向重建失败时，仅在本体资源可用且降级成功时显示本体图，状态、形状与位置不变。夜间状态写入一次批量提交58项并只做一次形状校验；异常时对已写入状态做一次恢复校验，并保留原异常，玩法行为不变。扩建材料名称读取游戏本地化。交单改为选择页与结果页分离，旧选项按引用识别并在同页复用，避免重复选项。本轮仅离线回归，未启动游戏实机验证。

【0.5.8 养蛇手册与产毒交付】
蛇夫人首次赠送雌雄蛇时一并赠送养蛇手册；旧档在下次到访／交谈免费补领一次。满位保留领取资格，腾位后再交谈，不重送蛇。使用原生多页阅读器和手册本背景，中英随游戏语言，分章说明喂食、健康、繁殖、蛇笼及产物用法。
产毒达到4/4后先保存进度；交付失败保持4/4并显示等待交付，成功才清零。空间预检使用蛇毒成品形状，面板暂停提示复用真实产毒条件。此前玩家“三天停滞”个案尚未确认根因，不宣称覆盖全部症状。
蛇笼四类合法肉的新建／加载、外壳拖放／打开主格离线回归通过，本轮未复现玩家拒收食物，不宣称已找到其实机根因。摄食改用原生肉类判断，兼容带肉类标记或肉类型的其他模组食材，不再硬编码四个ID。肉放打开后的笼内主格，食槽孔装喂食器设备。补水采用原生“优质水／纯水”名称；“基础水”不满足当前接口要求。清洁用品为“一瓶四合一洗涤剂”。

白块回滚：读档后若蛇的朝向只以运行时镜像保存，首次绘制前会按本体图重建该镜像，不再显示原生白色占位图；重建失败时仅在本体资源可用且降级成功时显示本体图，并在日志标注，蛇的状态、形状和位置不变。育成来源说明：育成加成来自用游戏原生金属材料扩建蛇笼，共8次，每次+5%，满级+40%，材料名读取游戏本地化；没有单独的育成用品。反馈的“只能扩建两次”尚未复现，本包不改上限，第三方笼具扩容模组仍可能影响实际次数。

【0.5.7 蛇蜕与过滤代工】
两种蛇毒原瓶是试剂瓶，不能直接饮用或自动啜饮，也不再被普通买家按补水自用结算。倒入普通容器后依原版用途处理，毒性仍在。
蛇在幼年转亚成年、亚成年转成年时各产一次蛇蜕，按蜕皮时实际体重的1%计量，每克提供10点普通过滤耐久。可直接装入普通水滤槽；拖到同一空间的另一包蛇蜕上合并剩余有效量，用过不会回满，不能直接出售。旧档不补历史蛇蜕。【找蛇夫人加工】将60g剩余有效蛇蜕、1只「滤水器」（耐久耗空也能作框架）、1份「化学用品（管制）」放在店内可取用的自有库存。蛇夫人来访时与她交谈，选择「委托加工『高级滤水器』」，确认交单；不是普通出售，也不是把材料放在一起就自动合成。只扣所需数量，多余留原处。交单满7天后她到访时直接交付1只「高级滤水器」，无需再买或另付钱；仓库满位会保留订单，腾位后再与她交谈领取。
捕蛇夹支持原生模组区扩容工具的两级升级，保留原生耗材与技工效果、背包嵌套和模块收益上限。幼蛇饱腹素材仍待审核，本包不接入未批准餐态图。

【0.5.6 蛇夫人收毒售后修复】
蛇夫人收购蛇毒按专业收购处理，不再被当作个人补水消费而引发毒害投诉。蛇毒成分、毒性、估值及普通顾客售后规则保留。已发生的投诉和声望扣减不自动撤销。

【0.5.4 捕蛇夹背包收纳修复】
捕蛇夹补齐原生可嵌套设备标记，覆盖新工具与旧档加载后的工具。原版普通、军用、腰包、挎包和儿童背包在拥有权、空间等原生条件满足时可收纳；模块舱、模块属性、价格和捕蛇概率不变。

【0.5.3 蛇笼、维护与育成】
蛇夫人专售蛇笼：新笼基价600，初始内部10×8，再扩建8次到16×30（对齐目标储物仓单页尺寸）；原生外部占格、材料、喂食器、混合、屠宰、维修及耐久功能保留，普通鼠笼不变。每次扩建提高育成速度5%，满级+40%，与个体成长基因相乘；扩建材料是游戏原生金属材料（显示名随语言），没有单独的育成用品；真实增重仍完整支付营养。
拖入电子元件可升级温控，每件+10%孵化速度，最多30级，最终4倍。普通蛋基础8夜、王蛇16夜，满温控最快2/4夜。紫外线灯作为恢复升级耗材，最多5级，每级+10%自然正向回血；不直接清除专属重伤、静养或产后冷却。
水箱10000mL，清洁储备2000点。把装有「优质水」或「纯水」的单个液体容器或「一瓶四合一洗涤剂」拖到蛇笼补充：只转移水箱缺少的液量，瓶子和余液留原处；每件「一瓶四合一洗涤剂」补50清洁，可整叠补入，只消费所需。每2000g活蛇每夜40mL水和8清洁，随实际体重成比例；加速孵蛋每笼额外20mL水，不按蛋数倍增，蛇球内部真实蛇只算一次。水足让温控生效，清洁足让UV恢复生效，两者足才启用育成加成；缺资源只暂停相应额外收益，不新增扣血/致病。没有对应升级收益时不消耗那项维护资源，空笼不消耗。
普通孕期3夜/产后恢复2夜，王蛇7夜/4夜；旧孕窝保留剩余天数，旧蛋按完成比例迁移，暂停/搬动不重复推进。提前成年不再沿用幼年体重陡坡，真实日龄不变。
初赠为亚成年雌雄各一条。首次王蛇孵化后，蛇夫人最近周访告知，下周起出售一条异性幼王蛇5000信用点；未买继续带，购一次后停止，不增加保底。幼蛇4格放大，小亚成6格；鼓腹、伸展、蛇丸/蛇杖按体型复用，长图不超过10格宽，放不下保留原蛇与营养。

【0.5.2 养殖空间与伸展】
允许在同一自有储藏空间养蛇：活鼠正常收益，仅非笼散放肉按40%热量入账；按原生肉类标记／肉类型识别可食肉，不限四个原版ID；需有效热量与数量，不跨窗口觅食。配对、蛇群载体和孵化支持同一可信自有空间，静养仍需独笼。满餐长条无连续空位时先规划并尝试有限整理，只移动同一窗口内合法可移动物品，失败保留原布局与真实营养；不保证所有碎片布局均能整理成功。

【0.5.1 紧急修复】
修复重复注册游戏补丁导致的毒液来源账本错序、刷新滞留及成长回调重复执行；嵌套和异常结束会安全收尾。减少重复的外观像素计算和库存形状校验，补充慢夜结阶段诊断。保留0.5满餐、消化、成长及素材，不改配方、售价或产毒周期。其他模组或原生夜结仍可能卡顿，本次不承诺消除所有长卡。

把普通眼镜蛇和稀有王蛇养在店里：喂鼠、成长、产毒、配对、孵蛋、交易，也能驱盗和应对搜查。
经过大量干员反馈...总之我们没有暗改。

【开始与保命】
在王尔德的人脉网络开通养蛇服务后，蛇夫人首次次日来访，送一公一母两条普通亚成年蛇；领取满位时腾位重试。之后按每7天安排回访（原生访客队列拥挤可能顺延），出售普通公母蛇、捕蛇夹、专用蛇笼及每周限量王八蛋，并收购蛇毒、蛇蛋、蛇胆和蛇肉。
蛇的免疫遗传基准从鼠的30独立提高到100，旧蛇、旧蛋和待交付快照只迁移一次，保留后天修正、真实疾病及伤势。蛇更耐病，但拥挤、负面修正和缺食仍需处理。生病、低健康和重伤不阻止合法维生进食；疾病和专用重伤仍暂停成长、产毒、繁殖。
王八蛋用于自有活蛇：同一条蛇首次随机强化成长、窝产或生育力一项基础基因10%，并全面治疗；之后使用只治疗。每次恢复生命、当前饥饿上限和生育力，治愈疾病、朊病、蛇伤和饥饿。不会复活、延寿、重置年龄/孕期或赠送繁殖储备；武器化仍需主动隔离静养两个合格夜晚。

【怎么喂、为什么会停】
蛇和未标记活鼠必须位于同一可信自有库存窗口；动物笼、储藏柜或背包可养殖，但不会跨窗口捕食。鼠不设体重限制，按从小到大连续捕食，不吃保护标记、死鼠或非法堆叠；最后整只超出的营养保留。
储备低于两夜工作需求才开餐。先按当前饥饿缺口、八夜维生储备、未来两夜增重及已锁定一窝费用算鼠重需求D，抵扣已有鼠/肉储备；D=0不开餐，否则本餐目标G=max(D,向上取整的餐前体重30%)。例如2kg蛇的D若为427g，本餐实际目标是600g；鼠不足则只吃现有合法鼠，最后整鼠超额保留。吃足有限目标即停，不无限杀鼠。鼠餐合并消化2–4夜，期间不续餐；每夜应吸收部分先供能，再结算原生夜耗。当前饥饿、未消化餐和已吸收储备是不同账，不能重复花。
普通肉和装肉的饲料机也能维生、支持较慢成长。全鼠实际消化1克记3游戏能量，肉和饲料机按实际热量入账；每正增重1克付4能量。饥饿上限为个体常规夜耗8倍，普通夜耗160×(体重/2000)^0.75，王蛇300×(体重/5000)^0.75，再乘实际基因/状态倍率。左侧饥饿速度是遗传参考，右侧只补充实际夜耗，不重复画饥饿条。

【鼓腹与面板】
幼蛇不加鼓腹图；亚成年和所有成年档按物种/性别使用各自已制作的餐态图，更大体型复用既有成年姿态等比例最近邻缩放，保持体型差异。达到餐前体重30%的真实鼠餐，首个白天显示横向长条；之后按当夜开始的腹部负载收成盘绕，再回日常。两夜消化的满餐依次为长条、盘绕、日常；大餐可持续更久，最后一夜也会盘绕。分档仍为30%长条、5%盘绕、低于2%日常，2%–5%保留过渡。显示阶段跨存档保存，读档/拖动/刷新不推进。营养仍在当夜正常吸收，不为显示而饿蛇。
饥饿条满不等于肚内还有大餐。新姿态放不下时保留原实例、真实进食和安全旧图，腾位后重试。附加面板只保留原生没有的成长进度、实际夜耗、一行腹内鼠餐、储备和当前繁殖/产毒状态；删去重复饱食条、成熟进度条和常驻规则墙。

【成长、繁殖与产物】
年龄不等于生理成长；食物与状态合格时普通基准16、王蛇24生理夜成年，纯肉成长速度为全鼠50%。实际正增重=min(当夜年龄/基因潜力,维生及锁定繁殖资金之外可付能量/4)。持续充分供鼠可稳定兑现已有潜力，鼠不足则少长；外观不免费发体重；蛇笼环境加速的增重同样付营养。普通70/90、王蛇120/150为老年/寿终基准，实际受原生基因影响。
同种成年异性备足本窝鼠源储备，且双方当前饱食扣除各自双倍夜耗后仍至少保留上限50%，即可开始交配，无需吃满，持续一天；两条缠绕，三条及以上形成蛇球并提高普通受精蛋出现王蛇的概率。交配只禁食一晚，完成夜总2倍夜耗，完成同夜恢复补食。蛇球保留原蛇，空间不足延后，分开满位则整组等待腾位，不充当免费仓库。
繁殖锁定实际蛋数/蛋种与体重，母体鼠当量费用=0.10W+0.90W×本窝幼蛇参考出生重总和/(标准窝数×本物种出生重)，公体每窝=0.10W。普通2kg公母4普通蛋共2200g，王母4kg/公6kg2王蛋共4600g鼠当量；完成整窝支付一次，不逐蛋重复收费。肉源不能支付繁殖，旧已付孕窝不追收。
普通受精蛋逐枚独立1%基础变异，培养与多蛇群体交配提高至最高2%；王蛇配对出王蛇蛋，两种蛇不互配，吃鼠不会使普通蛇自身变王蛇。普通孕期/基础孵化3/8夜，王7/16夜，产后恢复2/4夜。受精蛋放入可信自有空间后夜间自动孵化，无需点击；离开空间暂停，放回继续；普通库存可拖动。
合格成年蛇每4个产毒夜交付一次，普通标准来源估值360、王720；实际液体成分、价格和交易受原生规则影响。可出售或参与沉淀、过滤、蒸发、化学加工，无新增血清配方，不保证加工利润。满位尝试外部库存；全部满保留4/4并显示等待交付，成功才清零；已有实际pending瓶重试同瓶，极窄会话pending尚不持久化。
正常成年蛇屠宰出1胆，未成年不出；普通/王蛇胆每成功自用3枚分别永久+1拾荒/夜晚外出上限，治疗动物不计玩家培养，不即时补满当晚次数。

【捕蛇夹与拾荒】
捕蛇夹600信用点、2×3格，带原生通用机器模块舱。必须真正随身携带自有工具，才开启本模组活蛇、蛋、尸体、咬伤事件；留在后仓无效。不带不会触发新增蛇事件，原版拾荒不变；旧待交付收获不受工具门槛影响。
基础每次有效拾荒4%蛇事件、1%安全蛋互斥。蛇事件50%活捕、30%破损尸体、20%逃走；基础咬伤22%（含活捕与逃跑联合结果），安全蛋/尸体不额外咬人。活蛇数量越多，活捕递减至基础的5/7；35次/周参考活捕0.7条，8条及以上降至0.5条，不保证实际每周到手数。
效率提高遭遇率至最多6%；性能活捕至最多65%、咬伤降至15%；质量偏向较大个体。模块+300%按原生4倍总倍率折算，但捕蛇收益封顶；保留模块退化/学习等原生使用效果，多工具不叠加。
基础体型幼/成长/亚成/成年40/30/20/10%；蛇自身受伤率随体型10/20/40/60%。幼/成长蛇咬为轻伤，亚成为原生重伤，成年才额外附加两完整夜恢复，可用蛇胆解除。破损尸体只能屠宰，幼至成年肉基础价值约20/50/100/150，不出胆，且不超过同重完整组织（180g幼尸实际19）。这些为基础估值，不是保证售价。
捕获满位写入本档待交付，腾位后重试同一确定收获，不重复咬人/扣次数。有未交活捕时暂停新增活捕和咬伤。入包与队列写回间的极窄崩溃窗口仍非事务，不声称绝对防重复。

【事件与测试】
每条合格成年蛇10个百分点真实驱盗，十条100%，成功写夜报。搜查翻出活成年蛇会咬检查员并中止后续搜查：普通蛇被杀，王蛇重伤；已经查到的违禁品仍处理。
测试开局默认关闭。F9快速箱发放满级蛇笼、普通与王蛇成年公母各一对、12只活鼠与食物。测试请将整个动物笼移出箱子，蛇与鼠放回笼内；不要把测试箱当鼠笼。餐态演示是固定外观，不代替真实摄食测试。正常赠礼每档一次，不受测试开关影响。
完整中英随游戏语言。无需额外加载器；订阅后启用并重启。其他动物/屠宰/机器模组可能影响实际规则。本候选通过离线构建、生产适配回归、原生扫描和包检查；未启动Unity、未实测真实夜结/点击/显示/ES3，不等同实机通过。

---

Snake Ranch 0.5.9

0.5.9 FEEDBACK-QUALITY PACK
Save protection checks each reconstructed snake's saved instance, item ID, animal tag and inventory reachability after decoding; a broken reconstruction is logged with its reason and rejected instead of appearing silently. On a mirror-facing rebuild failure, body art is shown only when the body resource is available and the fallback succeeds. Night-end state writes commit once and validate shape once; on failure, performs one bounded recovery validation of the partially written state and preserves the original exception. Expansion material names follow game localization. Delivery uses separate choice and result pages, recognises its old option by reference and reuses it in place instead of adding duplicates. Offline regression only; not tested in a running game.

0.5.8 HANDBOOK AND VENOM DELIVERY
The Snake Lady includes a Snake Handbook with the starter pair. Existing saves receive it once on her next visit/conversation; full storage keeps the entitlement pending without granting extra snakes. The native paged reader and manual background present feeding, health, breeding, cage upkeep and products in Chinese or English.
Venom progress is saved at 4/4 before delivery. Failed placement keeps 4/4 and shows waiting for delivery; only success resets it. Placement checks use the actual finished vial shape, and panel pauses use real production eligibility. The reported three-day stall remains unconfirmed; these fixes do not establish its full cause.
Offline cage tests accept the four supported meats through new/load and shell/open-grid paths. The reported live rejection was not reproduced, so no live food-input fix is claimed. Place meat in the opened main storage grid; the feeder socket holds a feeder device. Refill with High-Quality Water or Pure Water, not Basewater. Cleaning uses Bottle of 4 In 1 Wash.

White-sprite rollback: if a loaded snake's facing is stored only as a runtime mirror, the mirror is rebuilt from its body art before the first draw instead of falling back to the native white placeholder. If rebuilding fails, body art is shown and logged only when the body resource is available and the fallback succeeds; snake state, shape and position are unchanged. Rearing comes from expanding the cage with the game's native metal material: eight expansions, +5% each, +40% at maximum, with the material name read from the game localization; there is no separate rearing item. The reported "only two expansions" issue was not reproduced; this build keeps the limit unchanged, and third-party cage-expansion mods may still affect the actual count.

0.5.7 SHED MEMBRANE AND FILTER COMMISSIONS
The two original venom vials cannot be drunk or auto-sipped, and are not sold as personal hydration. Decanted liquid retains native container use and real toxicity.
Snakes shed at the juvenile-to-subadult and adult milestones. Membrane weighs 1% of their actual weight and provides 10 ordinary-filter durability per gram. Use it in ordinary water-filter slots, or drag onto another pack in the same inventory to combine remaining material. Used material never refills; direct sale is blocked. Historical sheds are not granted to old saves. To commission an Advanced Water Filter, keep 60g of remaining usable membrane, one Water Filter (depleted durability is fine) and one Chemical Supplies (Restricted) accessible in your owned shop inventory. Talk to the Snake Lady and select the Advanced Water Filter commission; do not use ordinary selling. Only required amounts are consumed, and excess stays. She delivers directly on a visit at least seven days after submission, with no extra charge or purchase. Full storage keeps the order pending; make room and talk again.
The Catching Kit accepts two native module-bay expansion upgrades, preserving native material/Tinkerer rules, nesting and capped module benefits. Juvenile meal artwork is pending review and is not included.

0.5.6 VENOM TRADE HOTFIX
The Snake Lady buys venom professionally rather than for personal hydration. This prevents inappropriate poisoning complaints without changing venom chemistry, toxicity, value or ordinary customers' feedback rules. Previously applied complaints and reputation losses are not reversed.

0.5.4 BACKPACK STORAGE HOTFIX
Adds the native nestable-device tag to newly created and loaded Snake Catching Kits. Native ordinary, military, waist, satchel and children backpacks can accept the tool when normal ownership and space checks pass. Module contents, effects, price and catching probabilities are unchanged.

0.5.2 HABITAT AND STRETCH
Snakes can live in the same owned storage window: live rats retain normal nutrition; only loose meat outside an animal cage yields 40% energy. Meat uses the native meat tag/type classification, including correctly tagged mod-added meats with valid calories and quantity; feeding never reaches another window. Breeding, carrier groups and incubation use the same trusted owned habitat; rest still requires an isolated animal cage. When a Full pose lacks a contiguous slot, a bounded layout planner tries to rearrange movable owned items in that window. Failure keeps the original layout and real nutrition. It cannot guarantee solving every fragmented layout.

0.5.1 HOTFIX
Fixes duplicate game-patch registration that caused out-of-order venom provenance tracking, pending refreshes and repeated growth callbacks. Nested and exceptional operations now close safely. Reduces repeated sprite-mask work and inventory shape validation, and adds slow-night phase diagnostics. Keeps the 0.5 meals, digestion, growth rules and artwork; recipes, prices and venom cycles are unchanged. Other mods or native night processing may still stall; this update does not promise to eliminate every long pause.

Raise common cobras and rare King Cobras: feed, grow, breed, hatch eggs, harvest venom, trade, guard the shop and interrupt inspections.
After extensive operator feedback... anyway, we made no stealth changes.

STARTING AND SURVIVAL
Unlock Snake Ranch through Wilde. The Snake Lady retains her next-day first visit and a male/female subadult pair as the starter gift, then schedules a return every seven days; native visitor congestion can delay arrival. She sells male/female cobras, a Snake Catching Kit, the exclusive Snake Cage and a weekly limited supply of Bastard eggs, and buys snake products.
Snake inherited immunity now has its own base of 100 instead of the rat base of 30. Existing snakes, eggs and pending snapshots are rebased once, preserving acquired modifiers, illness and injuries. Disease, low health and severe injury do not block maintenance food; disease and special injury still pause growth, venom and breeding. Crowding and severe negative modifiers still matter.
First use of a Bastard egg on an owned live snake improves one growth, litter-size or fertility gene by 10% and fully treats it. Later eggs only treat it. Every use restores health/current hunger capacity/current fertility and cures disease, prions, wounds and starvation. It does not revive, extend lifespan, reset age/pregnancy or grant breeding food. Weaponization still requires two qualifying isolation nights.

FEEDING AND RESERVES
Put the snake and unmarked live rats in the same trusted owned inventory window. Animal cages, cabinets and backpacks can serve as habitats; snakes do not hunt across windows. Rats have no weight cutoff and are hunted from smallest upward. Marked, dead and illegally stacked rats are spared, and surplus from the final whole rat is retained.
A meal starts only when the two-night working reserve is insufficient. Let D be required rat grams after deducting existing reserves from the current hunger gap, eight-night maintenance reserve, two-night growth budget and one locked clutch. D=0 means no meal; otherwise G=max(D,ceil(0.30*pre-meal body weight)). For a 2kg snake whose D is 427g, the actual meal target is 600g. Short supply yields only the eligible rats available; surplus from the final whole rat is retained. Hunting stops at this finite target. Digestion takes 2–4 nights, with no second meal during digestion. Each due share becomes available before native hunger/starvation handling; hunger, undigested food and absorbed reserves cannot be spent twice.
Meat and meat-filled feeders sustain life and slower growth. Digested whole rats provide 3 game-energy units/g; meat uses actual native energy; every gram of positive growth costs 4 units. Hunger capacity is eight regular nightly costs. Common cost is 160*(weight/2000)^0.75; king cost is 300*(weight/5000)^0.75, adjusted by actual genetic/state multipliers. The native hunger-rate stat is a reference; the added panel gives actual cost without duplicating its hunger bar.

BELLY APPEARANCE AND PANEL
Juveniles keep normal art. Subadults and adults use species/sex-specific meal poses; larger adults reuse existing adult poses with uniform nearest-neighbor scaling for their body tier. A real meal of at least 30% of pre-meal weight shows the extended Full pose on the first day, then coils as digestion proceeds before returning to Normal. A two-night full meal shows Full, then Fed, then Normal; larger meals can last longer, with a coiled final night. Thresholds remain 30% Full, 5% Fed and below 2% Normal, with hysteresis between 2% and 5%, using the load at the start of the night. Pose progress persists through saves; loading, dragging and refreshing never advance it. Nutrition is still absorbed on schedule that night.
A full hunger bar does not mean a large meal remains in the belly. If a new pose cannot fit, the original item, real meal and safe old appearance are kept for retry. The added panel now gives only missing information: physiological growth, actual night cost, one meal line, reserves and relevant production/breeding status. Duplicate fullness/maturity bars and permanent rules walls are removed.

GROWTH, BREEDING AND PRODUCTS
Age differs from physiological growth. Adequate whole-rat feeding supports baseline maturity at 16/24 physiological nights for common/king snakes; meat growth is 50% as fast. Positive growth is the smaller of age/genetic potential and available tissue energy divided by four, after maintenance and protected clutch funds. Sustained adequate food realizes existing potential; short supply reduces growth. Poses grant no free tissue; cage environment bonuses also pay the full tissue cost. Common old-age/end-of-life bases are 70/90 days; kings 120/150, with native genetics affecting the result.
Same-species adult males/females need funded rat-source clutch reserves and enough actual hunger to retain at least 50% of capacity after each parent's double nightly cost. Full hunger and forced pre-mating refills are not required. Mating lasts one day. Two intertwine; three or more form a snake ball and improve rare offspring odds. Only the mating night is fasted, its ending night costs twice normal, and food is available again that same completion night. Original snakes remain inside the carrier; insufficient space delays entry/release rather than granting storage.
Maternal rat-equivalent cost is 0.10W+0.90W*(sum of planned offspring reference birth weights)/(standard clutch size*species birth weight). Paternal cost is 0.10W per clutch. A 2kg common pair/four common eggs costs 2200g combined; a 4kg king female/6kg male/two king eggs costs 4600g. Pay the whole locked clutch once at completion, never again per egg. Meat cannot pay breeding costs; old paid pregnancies are grandfathered.
Common fertile eggs independently have a base 1% King Cobra chance, up to 2% through cultivation and group mating. King pairs produce king eggs, species do not cross, and eating rats never transforms a common snake. Gestation/base incubation is 3/8 nights common and 7/16 king; recovery is 2/4 nights. Fertile eggs in a trusted owned habitat incubate automatically overnight without clicking; leaving pauses and returning resumes; ordinary stored eggs remain draggable.
Eligible adults produce venom every four qualifying nights. Reference source values are 360 common/720 king; actual liquid composition and trade modifiers vary. Sell or use native settling/filtering/evaporation/chemistry; no new serum recipe or guaranteed processing profit. Full cages try outside storage; fully blocked delivery keeps progress. An actual session-pending venom bottle is retried without rerolling, but that narrow bottle path is not serialized.
Normal adult butchering yields one gallbladder; younger stages yield none. Every three player-self-consumed common/king gallbladders permanently add one scavenging/outing limit respectively; treating animals does not count or refill current attempts.

CATCHING KIT AND SCAVENGING
The 600-credit 2x3 kit has a native universal machine-module bay. Carry an owned kit outside to enable all mod snake/egg/carcass/bite events. Leaving it in shop storage has no effect. No kit means no added snake events; native scavenging is unchanged and old pending rewards can still be delivered.
Base rolls are mutually exclusive 4% snake events and 1% safe eggs. Snake events yield 50% live catches, 30% damaged carcasses, 20% escapes, with 22% total bite risk across the joint outcomes. Eggs/carcasses add no bite. More owned live snakes reduce live capture to at least 5/7 of base: 35 attempts model 0.7 live snakes/week when empty and 0.5 with eight or more, not a guaranteed payout.
Efficiency caps encounters at 6%; performance caps live capture at 65% and lowers bites to 15%; quality favours larger animals. Native +300% is normalized to 4x total, with bounded catching benefits and native module wear/learning effects. Multiple kits do not stack.
Base juvenile/growing/subadult/adult weights are 40/30/20/10%; the snake's injury chance rises 10/20/40/60%. Young bites cause minor wounds, subadults cause native major wounds, and only adult bites add two full recovery nights, removable with gallbladders. Damaged carcasses are butcher-only, with roughly 20/50/100/150 base meat value by size, no gallbladder, and no more than intact tissue of that weight (180g juvenile yields 19). Trade prices may differ.
Full inventories save determined rewards in a per-save pending queue for retry, without repeated bites/attempt costs. An outstanding live catch pauses new live captures/bites. The very narrow crash window between placement and queue writeback remains nontransactional.

EVENTS AND TESTING
Each eligible caged adult adds 10 percentage points of real theft interception, reaching 100% with ten. Inspectors finding an adult get bitten, stopping later search; common cobras die and kings are badly injured. Previously discovered contraband is still processed.
Test starts are off by default. The F9 quick kit grants a max-level snake cage, one adult male and female each of common and King snakes, 12 live rats and food. Move the entire animal cage out of the case and keep snakes/rats inside it. Fixed meal demos do not replace real feeding tests. The normal one-time gift is independent of test settings.
Chinese/English follow game language. No extra loader is required. Other animal/butchering/machine mods can affect actual behaviour. This candidate passes offline builds, production-adapter regressions, native scans and package checks. Unity, real nightly dispatch, clicking/rendering and ES3 remain untested.

【武器化与静养外观】
武器化变成蛇本体构成的蛇杖；静养变成单蛇蜷成的熟睡蛇丸。普通蛇和王蛇雌雄各有配色。死亡和交配画面优先，特态结束后恢复餐态或日常。开始、取消和完成静养当天刷新外观。新形状放不下保留原图，不影响真实状态。
武器化/静养蛇保留单件拖动修复：移入没有其他活体动物或活鼠的动物笼后，按游戏激活设置单击或双击开始/取消静养；两个完整合格夜后解除。静养期间仍处于武器化，喂饱不直接解除。

WEAPON AND REST APPEARANCE
Weaponized snakes take a staff form made from the living snake itself. Resting snakes curl into a sleeping ball. Common and King Cobras each retain male/female palettes. Death and mating displays take priority; finishing a special state returns to the appropriate meal or normal pose. Starting, cancelling and completing rest refresh the pose that day. If the new shape cannot fit, the old safe art stays and actual state is preserved.
Individual dragging remains fixed. Place a weaponized snake alone in an animal cage with no other live animals or rats, then single- or double-click according to the activation setting to start/cancel rest. Two complete qualifying nights clear weaponization; feeding alone does not.


[0.5.3 Snake Cage and body-size art]
The Snake Lady exclusively sells a Snake Cage. It starts at 10x8 internal cells and accepts eight paid expansions, ending at 16x30 (the target storage bay's single-page size). New cages have a base value of 600; it retains the native animal cage factory, outer footprint, upgrade materials, feeder, automatic butcher, repairs, durability and save/load callbacks. Ordinary cages are unchanged.
The starter gift is now a male/female subadult pair. After the first King Cobra egg hatches, the Snake Lady announces one opposite-sex juvenile on her next visit and offers it from the following week for 5000 credits. Unsold stock returns weekly; this special purchase is available once. No mutation guarantee was added.
Common hatchlings retain four cells with a larger visible silhouette; small subadults use six. Existing meal, resting-ball and staff art is uniformly scaled by body tier, with long poses capped at ten cells wide. Placement failures retain the real snake, nutrition and safe appearance.

[0.5.3 Maintenance, incubation and growth]
The Snake Cage starts at 600 credits and 10x8 internal cells, expanding eight times to 16x30. Each expansion adds 5% growth speed, up to +40%, multiplied by individual growth genes; expansion uses the game's native metal material (shown in the game language), with no separate rearing item; tissue growth still spends real nutrition. Electronics add 10% incubation speed each, capped at 30 levels and 4x total speed. Common/King eggs take 8/16 base nights, or 2/4 at full speed. UV Lamps fund five recovery upgrades, +10% natural positive healing each; they do not clear special injuries, rest or reproduction cooldowns.
Drag one liquid container containing at least 99% water, or Bottle of 4 In 1 Wash, onto the cage. Only the missing liquid volume is transferred; the container and excess remain. Capacity: 10L water and 2000 cleaning points; each Bottle of 4 In 1 Wash gives 50 points, consuming only the needed stack count. Upkeep scales with live snake weight: 40mL water and 8 cleaning per 2kg nightly. Accelerated incubation adds 20mL per cage, not per egg. Carrier children count once, carcasses do not. Water enables temperature bonuses, cleaning enables UV recovery, both enable growth. Shortages pause the corresponding extra benefit, without added damage/disease. Empty cages and resources with no applicable upgrade benefit do not pay upkeep.
Common gestation/recovery takes 3/2 nights, King 7/4. Existing pregnancies retain remaining time; existing eggs retain completion proportion. Fractional incubation and per-night guards persist. Earlier maturity no longer applies the juvenile weight slope to adult growth, without changing actual age. Tests use production adapters with explicit engine substitutes; Unity, real UI/night callbacks and ES3 remain untested.

[0.5.5 Cage material drag/drop hotfix]
Cage expansion, repair and care supplies act directly on the Snake Cage instead of being stored first. Uses only the selected quantity actually needed; unselected and excess materials stay at their source. Water refills transfer only the required liquid, leaving the bottle and remaining water in place. Full upgrades, full reserves and invalid inputs are rejected rather than stored. Native rat cages and other containers retain their normal behavior.


[0.5.10 护蛋与拾荒品质 / Egg guarding and scavenged egg quality]
成年活蛇公母均可在同一直接库存内保护全部未孵化受精蛇蛋，并尝试围住真实蛋的位置；空间不足仍有保护。活鼠实际偷吃时，95%被杀死、蛋保留；5%吃蛋成功并获得一次重量/价值/名称强化。没有每夜随机杀鼠，不额外提高孵化速度。四张护蛋圈图公用；蛇仍正常维持生理活动。
新拾荒蛋按脆弱25%、普通54%、优化20%、王蛇1%分类（占掉落蛋；原每次有效拾荒1%掉蛋率不变）。四项真实基因为生命、免疫、繁殖力、寿命；脆弱50–80%、普通90–110%、优化120–150%，王蛇按其物种基准90–110%。创建时确定并保存，读档/孵化不重掷，旧蛋和繁殖蛋不变。蛇笼适配鼠笼来源换肤，蛇夫人仅在可交单时保留选项。测试开局默认关闭。
Living adults of either sex protect all unhatched fertile eggs in the same direct inventory and attempt to surround the real eggs; protection remains when the layout cannot fit. When a rat actually steals an egg, it has a 95% chance to be killed with the egg retained, or a 5% chance to eat it and gain a one-time weight/value/name boost. No random nightly killing or extra incubation speed. Four shared guard-ring sprites; snakes retain normal physiology.
New scavenged egg tiers are 25% fragile, 54% ordinary, 20% optimized and 1% King, conditional on the unchanged 1% egg drop per effective search. Health, immunity, fertility and longevity genes vary at 50–80%, 90–110%, 120–150%, or 90–110% of the King species baseline respectively. Rolls persist with no load/hatch reroll; old and bred eggs stay unchanged. Snake cages support rat-cage skin sources; Snake Lady commission choices appear only when available. Test starts remain off by default.

[0.5.11 紧急权限声明修复 / Permission declaration hotfix]
补齐护蛋补丁使用的 reflection-external 权限声明，修复0.5.10因所需权限未写全而无法启用。新增扫描能力与manifest声明一致性检查；玩法、美术及测试默认关闭不变。
Declares reflection-external used by the egg-guard patch, fixing the missing-capability enable refusal in 0.5.10. Release checks now compare detected and declared capabilities. Gameplay and art are unchanged; test starts remain off by default.

[0.5.12 无额外权限 / No additional capabilities]
护蛋补丁改用固定目标的公共Harmony接口，不再使用触发reflection-external的动态反射；权限声明恢复为空。保留护蛋、偷蛋和品质规则。测试开局仍默认关闭。
Egg guarding now uses fixed public Harmony APIs instead of dynamic reflection requiring reflection-external. The manifest requests no additional capabilities; guarding, theft and egg-quality rules are preserved. Test starts remain off by default.
