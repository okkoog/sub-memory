# CorpPresence 1.0 current DLL static decompilation

DLL sha256: ef3f3f093e5460c697811104c5a1c0f0b643b866283d411fbb0f2355cb87962b

Decompiled C# files: 123

Copied localization classes: 7


## ExternalTranslations

- `CorpPresence/ExternalTranslations.cs:11` — `internal static class ExternalTranslations`
- `CorpPresence/ExternalTranslations.cs:28` — `internal static string DefaultDirectory => Path.Combine(Path.GetDirectoryName(typeof(ExternalTranslations).Assembly.Location) ?? AppContext.BaseDirectory, "Translations");`
- `CorpPresence/TextTranslations.cs:19` — `return ExternalTranslations.HasLocale(locale);`
- `CorpPresence/TextTranslations.cs:57` — `if (ExternalTranslations.TryFormat(key, formattableString, locale, out var value))`
- `CorpPresence/TextTranslations.cs:96` — `if (!ExternalTranslations.TryGet(key, english, locale, out var value))`
- `CorpPresence/Brand.cs:105` — `int version = ExternalTranslations.Version;`
- `CorpPresence/L.cs:23` — `int version = ExternalTranslations.Version;`
- `CorpPresence/L.cs:38` — `_locale = ExternalTranslations.ResolveLocale(LocHelper.GetCurrentLocaleCode() ?? "");`
- `CorpPresence/L.cs:74` — `if (!ExternalTranslations.TryGet(key, en, _locale, out var value))`
- `CorpPresence/L.cs:89` — `if (!ExternalTranslations.TryFormat(key, en, locale, out var value))`
- `CorpPresence/L.cs:116` — `if (ExternalTranslations.TryFormat(key, en, _locale, out var value))`
- `CorpPresence/L.cs:130` — `if (ExternalTranslations.TryFormat(key, en, _locale, out var value))`
- `CorpPresence/L.cs:148` — `if (!ExternalTranslations.TryFormat(key, english, _locale, out var value))`
- `CorpPresence/L.cs:158` — `if (ExternalTranslations.TryFormat(key, en, _locale, out var value))`
- `CorpPresence/L.cs:207` — `if (!en2 && (!ExternalTranslations.HasLocale(_locale) || zh == null || en == null))`
- `CorpPresence/L.cs:219` — `array[i] = (en2 ? TextTranslations.SelectKey(key, en[i], _locale) : (ExternalTranslations.TryGet(key, en[i], _locale, out var value) ? value : zh[i]));`
- `CorpPresence/SpacerCo.cs:257` — `if (!L.En && !ExternalTranslations.HasLocale(L.Locale))`
- `CorpPresence/Main.cs:60` — `_runtime.Own(ExternalTranslations.Reload, ExternalTranslations.Clear);`
- `CorpPresence/UI/MenuLabels.cs:88` — `if (ExternalTranslations.HasLocale(locale))`
- `CorpPresence/UI/MenuLabels.cs:90` — `int version = ExternalTranslations.Version;`

Total matching lines: 20

## ModHelper.GetLocalized


Total matching lines: 0

## LoadLocalization


Total matching lines: 0

## LoadedMod.Localization


Total matching lines: 0

## File.ReadAllText

- `CorpPresence/ExternalTranslations.cs:254` — `return JObject.Parse(File.ReadAllText(file, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)));`

Total matching lines: 1

## File.ReadAllBytes


Total matching lines: 0

## Directory.Exists

- `CorpPresence/ExternalTranslations.cs:166` — `if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))`

Total matching lines: 1

## File.Exists

- `CorpPresence/ExternalTranslations.cs:223` — `if (File.Exists(text2))`

Total matching lines: 1

## ReadAllText

- `CorpPresence/ExternalTranslations.cs:254` — `return JObject.Parse(File.ReadAllText(file, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)));`

Total matching lines: 1

## language.json

- `CorpPresence/ExternalTranslations.cs:174` — `if (string.Equals(Path.GetFileName(item), "language.json", StringComparison.OrdinalIgnoreCase))`
- `CorpPresence/ExternalTranslations.cs:222` — `string text2 = Path.Combine(directory, "language.json");`

Total matching lines: 2

## Translations/


Total matching lines: 0

## Translations\\


Total matching lines: 0

## Capability


Total matching lines: 0

## ResolveLocale

- `CorpPresence/ExternalTranslations.cs:78` — `internal static string ResolveLocale(string gameLocale)`
- `CorpPresence/L.cs:38` — `_locale = ExternalTranslations.ResolveLocale(LocHelper.GetCurrentLocaleCode() ?? "");`

Total matching lines: 2

## L.K(

- `CorpPresence/Company.cs:47` — `return L.K("companies.company.tierName.unsigned", "未签约", "Unsigned");`
- `CorpPresence/StoryGuide.cs:13` — `return WithPrerequisites(EnvoyPages(), L.K("presentation.storyGuide.build.allChaptersMaliceModeEnvoy", "共同前提：恶意模式开启，撒该未离场；开店时检查。", "All chapters: Malice mode on, envoy not gone; checked at opening."));`
- `CorpPresence/StoryGuide.cs:17` — `return WithPrerequisites(RebelPages(), L.K("presentation.storyGuide.build.allChaptersMaliceModeOdin", "共同前提：恶意模式开启，已认识奥丁，未被捕且未断线；开店时检查。助力点门槛按累计获得计算，不是当前余额。", "All chapters: Malice mode on; Odin met, not arrested or cut off; checked at opening. Support thresholds use lifetime earned points, not your current balance."));`
- `CorpPresence/StoryGuide.cs:22` — `return new string[1] { L.K("presentation.storyGuide.build.thisCompanyHasNoIndependent", "此公司没有独立章节。", "This company has no independent chapters.") };`
- `CorpPresence/StoryGuide.cs:31` — `stringBuilder.AppendLine(L.F("presentation.storyGuide.build.chapter", "第 {0} 章（{1}）", "Chapter {0} ({1})", i, flag ? L.K("presentation.storyGuide.build.sideStory", "支线", "side story") : L.K("presentation.storyGuide.build.mainStory", "主线", "main story")));`
- `CorpPresence/StoryGuide.cs:32` — `stringBuilder.AppendLine(L.K("presentation.storyGuide.build.bookingRequirementsAllRequired", "预约条件（须同时满足）：", "Booking requirements (all required):"));`
- `CorpPresence/StoryGuide.cs:33` — `stringBuilder.AppendLine(L.K("presentation.storyGuide.build.reputation", $"• {Faction(character)}声望 ≥ {thresholds[i - 1]}。", (FormattableString)$"• {Faction(character)} reputation ≥ {thresholds[i - 1]}."));`
- `CorpPresence/StoryGuide.cs:34` — `stringBuilder.AppendLine((num == 0) ? L.K("presentation.storyGuide.build.noPreviousChapterRequired", "• 无前置章节。", "• No previous chapter required.") : L.K("presentation.storyGuide.build.mainProgressLeastChapterThis", $"• 主线进度达到第 {num} 章；本章尚未完成。", (FormattableString)$"• Main progress at least chapter {num}; this chapter not completed."));`
- `CorpPresence/StoryGuide.cs:37` — `stringBuilder.AppendLine(L.K("presentation.storyGuide.build.holdLeastQualifyingItemS", $"• 店内持有符合收购要求的物品 ≥ {character.BuyGoodsNeeded} 件；本章不检查现金门槛。", (FormattableString)$"• Hold at least {character.BuyGoodsNeeded} qualifying item(s) in the shop; no cash gate for this chapter."));`
- `CorpPresence/StoryGuide.cs:41` — `stringBuilder.AppendLine(L.K("presentation.storyGuide.build.cashEqualityDoesNotQualify", $"• 现金 > {character.CashFor(i)}₵（恰好相等不满足）。", (FormattableString)$"• Cash > {character.CashFor(i)}₵ (equality does not qualify)."));`
- `CorpPresence/StoryGuide.cs:49` — `stringBuilder.AppendLine(L.K("presentation.storyGuide.build.checkedClosingNextDayPlanned", $"关店时检索，满足后预约次日。按预计来访日计算，距该人物上次剧情至少 {Settings.StoryGap(character)} 天（当前设置）。已有剧情预约先完成，主线优先于支线；不同人物独立判断。", (FormattableString)$"Checked at closing for the next day. The planned visit must be at least {Settings.StoryGap(character)} day(s) after this character's `
- `CorpPresence/StoryGuide.cs:50` — `stringBuilder.AppendLine(L.K("presentation.storyGuide.build.thisGuideDoesNotComplete", "这是触发条件说明，不会替你完成任务、补发物品或改变存档。", "This guide does not complete tasks, grant items, or change your save."));`
- `CorpPresence/StoryGuide.cs:70` — `return L.K("presentation.storyGuide.faction.highestBlackMarketRevolutionLower", "黑市/革命军/下层区三者最高值（恶意模式仅黑市）", "highest of Black Market/Revolution/Lower Levels (Black Market only in Malice mode)");`
- `CorpPresence/StoryGuide.cs:74` — `"FACTION_UPPER_LEVEL" => L.K("presentation.storyGuide.faction.upperLevels", "上层区", "Upper Levels"),`
- `CorpPresence/StoryGuide.cs:75` — `"FACTION_SECURITY" => L.K("presentation.storyGuide.faction.security", "治安部", "Security"),`
- `CorpPresence/StoryGuide.cs:76` — `"FACTION_LOWER_LEVEL" => L.K("presentation.storyGuide.faction.lowerLevels", "下层区", "Lower Levels"),`
- `CorpPresence/StoryGuide.cs:89` — `return L.K("presentation.storyGuide.extra.itemPortableWaterPurifier", "• 物品：便携净水器 portable_water_purifier。", "• Item: portable_water_purifier.");`
- `CorpPresence/StoryGuide.cs:91` — `return L.K("presentation.storyGuide.extra.waterMerchantStartOrVanilla", "• 净水商开局，或已解锁原版水商功能。无需先完成罐装支线。", "• Water Merchant start, or vanilla water trading unlocked. Canning is not a prerequisite.");`
- `CorpPresence/StoryGuide.cs:93` — `return L.K("presentation.storyGuide.extra.davreCooperationRancherStartOr", "• 达夫雷合作度 ≥ 60。\n• 养鼠开局，或解锁 RETIRED_RANCHER（养鼠相关升级）。\n• 不要求先完成第3章净水支线。交谈后拒绝或现金不足，可在电话“罐装设备”页补办。", "• Davre cooperation ≥ 60.\n• Rancher start, or RETIRED_RANCHER unlocked.\n• The chapter 3 water side story is not required. If declined or payment fails, sign up later on the C`
- `CorpPresence/StoryGuide.cs:100` — `return L.K("presentation.storyGuide.extra.itemBlackInjectorCountedBy", "• 物品：突触注射器 black_injector；按物品标识计数。", "• Item: black_injector; counted by item identifier.");`
- `CorpPresence/StoryGuide.cs:102` — `return L.K("presentation.storyGuide.extra.carmichealHasRecordedCreatureGrant", "• 卡迈克尔已记录发放三叶蜥（CreatureGiven）。\n• 当前持有活着的成年或更高阶段三叶蜥。主线需先达到第5章。", "• Carmicheal has recorded the creature grant (CreatureGiven).\n• Currently hold a living creature at Adult stage or above. Main progress must have reached chapter 5.");`
- `CorpPresence/StoryGuide.cs:104` — `return L.K("presentation.storyGuide.extra.carmichealBloodDrawingLicenseGranted", "• 已获卡迈克尔银血采血许可（Licensed）。\n• 累计采血记录 BloodDrawn > 0。", "• Carmicheal blood-drawing license granted (Licensed).\n• Recorded blood drawn (BloodDrawn) > 0.");`
- `CorpPresence/StoryGuide.cs:111` — `return L.K("presentation.storyGuide.extra.farmerStartOrRETIREDFARMER", "• 水培开局，或解锁 RETIRED_FARMER（水培相关升级）。\n• 第6～8章只要求主线达到第3章，不必等主线第5章。", "• Farmer start, or RETIRED_FARMER unlocked.\n• Chapters 6–8 require main chapter 3, not chapter 5.");`
- `CorpPresence/StoryGuide.cs:113` — `return L.K("presentation.storyGuide.extra.rollingCraftStageCompletedCraftDone", "• 已完成卷烟工艺第一阶段（CraftDone(1)）。\n• 已累计售出散烟 ≥ 10（LooseSold）。现金门槛使用当前打点费用。", "• Rolling craft stage 1 completed (CraftDone(1)).\n• Loose cigarettes sold ≥ 10 (LooseSold). The cash gate uses the current bribe fee.");`
- `CorpPresence/StoryGuide.cs:115` — `return L.K("presentation.storyGuide.extra.bribePaidBribedCigarettePacks", "• 已完成打点（Bribed）。\n• 已累计制作烟包 ≥ 6（PacksMade）。", "• Bribe paid (Bribed).\n• Cigarette packs made ≥ 6 (PacksMade).");`
- `CorpPresence/StoryGuide.cs:121` — `return L.K("presentation.storyGuide.extra.meridianCooperationCashGateUses", "• 子午线合作度 ≥ 60。现金门槛使用当前锻造设备手续费。", "• Meridian cooperation ≥ 60. The cash gate uses the current forge equipment fee.");`
- `CorpPresence/StoryGuide.cs:127` — `return L.K("presentation.storyGuide.extra.twoAcceptedLuxuryItemsExcluded", "• 需2件可收购奢侈品；排除索拉纳拒收名单：护肤霜、神经活性香水、费洛蒙香水及本模组面膜、蜡烛（含点燃）、精华。", "• Two accepted luxury items; excluded IDs: skincare_cream, neuroactive_perfume, pheromone_perfume, corp_presence_mask, corp_presence_candle, corp_presence_candle_lit, corp_presence_elixir.");`
- `CorpPresence/StoryGuide.cs:138` — `L.K("presentation.storyGuide.envoyPages.chapterAidMaliceModeEnvoy", "第1章：扶持金\n\n恶意模式开启；撒该未离场、扶持金尚未处理。\n现金首次 >3000 后记下日期；此后现金 <2500，或距该日期满4天，才会预约。\n同时必须拿到帕斯泰尔和卡迈克尔两家联系方式（公司启用且已发名片）。\n接受后需连续订阅两家公司21天；断订转为每周800、共5周的还款。开店时检查；待排队的旧预约也检查联系方式。", "Chapter 1: Aid\n\nMalice mode on; envoy not gone; aid not handled.\nFirst cash >3000 records the day. Booking `
- `CorpPresence/StoryGuide.cs:139` — `L.K("presentation.storyGuide.envoyPages.chapterServiceFeeGiftAid", "第2章：服务费与礼物\n\n第1章状态为扶持金完成或拒绝；本章未处理；现金 >6000。\n可支付服务费、接受礼物或拒绝。\n礼物期限14天，只能交给从反抗军电话“任务”页专门预约来的奥丁；普通奥丁、其他剧情奥丁及地下买家不能代收。预约次日到达，截止日当天不可再预约次日；被捕、断线或黑名单时不能交付。", "Chapter 2: Service fee and gift\n\nAid completed or declined; chapter 2 not handled; cash >6000.\nPay the service fee, accept t`
- `CorpPresence/StoryGuide.cs:140` — `L.K("presentation.storyGuide.envoyPages.chapterJoinCorporationsChapterPaid", "第3章：加入巨企\n\n第2章已付费、礼物已交付，或渠道费阶段已结束。\n已经买下店铺产权（IsPropertyPaid）；本章尚未处理。\n开店时预约。加入和拒绝会进入不同后续分支；加入会影响反抗军联系。", "Chapter 3: Join the corporations\n\nChapter 2 paid, gift delivered, or channel-fee period ended.\nShop property purchased (IsPropertyPaid); chapter 3 not handled.\nB`
- `CorpPresence/StoryGuide.cs:141` — `L.K("presentation.storyGuide.envoyPages.chapterMentorSMessageCoins", "第4章：导师来信与硬币\n\n已经加入巨企；本章未处理；现金 ≥20000。\n开店时登记导师来信，需回拨商人联合电话继续。\n进入沉默分支后，未成为代理人时，帕斯泰尔、索拉纳、卡迈克尔、子午线、达夫雷各自合作度 ≥100，会分别预约尚未领取的命运硬币。", "Chapter 4: Mentor's message and coins\n\nCorporations joined; chapter 4 not handled; cash ≥20000.\nOpening records the message; call the Merchant Uni`
- `CorpPresence/StoryGuide.cs:149` — `L.K("presentation.storyGuide.rebelPages.resistanceChapterMaliceModeOdin", "反抗军第1章\n\n恶意模式开启，已认识奥丁，未被捕且未断线。原版反抗军电话为正常或冷却状态后会登记认识并开放本模组反抗军电话。\n尚无已完成/进行中的反抗军章节，累计获得助力点 ≥50。\n开店时预约；任务交一瓶毒药，或累计价值100的梦尘/芬太片。", "Resistance chapter 1\n\nMalice mode on; Odin met, not arrested or cut off. A Regular/Cooldown vanilla Resistance phone records the introduction a`
- `CorpPresence/StoryGuide.cs:150` — `L.K("presentation.storyGuide.rebelPages.bombRoute", "反抗军第2章\n\n第1章完成，当前没有进行中章节，累计获得助力点 ≥300。\n可打点子午线取得炸弹再交给任务买家奥丁，或通过地下买家累计售出价值1000的武器。\n此章的子午线炸弹与撒该的礼物是两种不同任务物品，不能混交。", "Resistance chapter 2\n\nChapter 1 completed, no active chapter, total support points earned ≥300.\nBribe Meridian and sell its bomb to the chapter's Odin buyer, or sell 1000 worth `
- `CorpPresence/StoryGuide.cs:151` — `L.K("presentation.storyGuide.rebelPages.medicalAid", "反抗军第3章\n\n第2章完成，当前没有进行中章节，累计获得助力点 ≥500。\n开店时启动；奥丁按任务周期来收医疗品，累计售出金额达到1200完成。\n仍要求已认识奥丁、未被捕、未断线。", "Resistance chapter 3\n\nChapter 2 completed, no active chapter, total support points earned ≥500.\nStarts at opening. Sell 1200 worth of medical supplies to the scheduled Odin buyer.\nOdin must stil`
- `CorpPresence/StoryGuide.cs:152` — `L.K("presentation.storyGuide.rebelPages.joinResistance", "反抗军第4章\n\n第3章完成，当前没有进行中章节，尚未记录加入邀请；撒该第3章已选择拒绝巨企。\n邀请后可从反抗军任务页呼叫奥丁。加入后，向任务奥丁累计售出武器/医疗品达到3000，会安排突袭剧情，再按剧情日期推进。", "Resistance chapter 4\n\nChapter 3 completed, no active chapter, invitation not yet recorded; corporations refused in envoy chapter 3.\nAfter invitation, call Odin from the Tasks p`
- `CorpPresence/StoryGuide.cs:153` — `L.K("presentation.storyGuide.rebelPages.resistanceChapterChapterCompletedChapter", "反抗军第5章\n\n第4章完成，第5章尚未启动，现金 ≥16000；仍需奥丁已认识、未被捕、未断线。\n启动后，帕斯泰尔、索拉纳、卡迈克尔、子午线、达夫雷各自合作度 ≥100，分别安排未领取的命运硬币。\n五家公司硬币都登记领取后，才完成最终代表资格。", "Resistance chapter 5\n\nChapter 4 completed, chapter 5 not started, cash ≥16000; Odin still met, not arrested and not cut off.\nAfter ac`
- `CorpPresence/Services.cs:41` — `return L.K("companies.services.periodWord.thisPeriod", "本期", "this period");`
- `CorpPresence/Services.cs:43` — `return L.K("companies.services.periodWord.thisWeek", "本周", "this week");`
- `CorpPresence/Services.cs:101` — `return L.K("companies.services.bloodRecovery.requestBloodRecovery.carmichealBreedingLicenseRequired", "需要卡迈克尔的养殖许可。", "A Carmicheal breeding license is required.");`
- `CorpPresence/Services.cs:107` — `return L.K("companies.services.bloodRecovery.requestBloodRecovery.recoveryOpensAgainDay", $"第 {num} 天可再次回收。", (FormattableString)$"Recovery opens again on day {num}.");`
- `CorpPresence/Services.cs:112` — `return L.K("companies.services.bloodRecovery.requestBloodRecovery.thereAreNoEligibleFull", "店内没有符合条件的完整银血样品。", "There are no eligible full Silver Blood samples in the shop.");`
- `CorpPresence/Services.cs:116` — `return L.K("companies.services.bloodRecovery.requestBloodRecovery.sampleCountChangedCheckUpdated", "样品数量有变化，请核对后重试。", "The sample count changed. Check the updated quote and try again.");`
- `CorpPresence/Services.cs:120` — `return L.K("companies.services.bloodRecovery.requestBloodRecovery.cashNearItsLimitRecovery", "现金已接近上限，暂时无法结算。", "Cash is near its limit. Recovery cannot be settled.");`
- `CorpPresence/Services.cs:132` — `return L.K("companies.services.bloodRecovery.requestBloodRecovery.noSamplesWereCollectedNo", "未收走样品，未开始冷却。", "No samples were collected. No cooldown started.");`
- `CorpPresence/Services.cs:138` — `return L.K("companies.services.bloodRecovery.requestBloodRecovery.collectedSilverBloodSamplesReceived", $"已回收 {num2} 支银血，到账 {num3}₵。", (FormattableString)$"Collected {num2} Silver Blood samples. Received {num3}₵.");`
- `CorpPresence/Services.cs:434` — `return L.K("companies.services.pay.saveNotReady", "存档未就绪", "Save not ready");`
- `CorpPresence/Services.cs:441` — `return L.K("companies.services.pay.companyDataNotReady", "公司数据未就绪，尚未付款。", "Company data is not ready. No payment attempted.");`
- `CorpPresence/Services.cs:449` — `return L.K("companies.services.pay.saveLoadError", "公司数据未就绪，尚未付款。请先检查存档读取错误。", "Company data is not ready. No payment attempted. Check the save-load error first.");`
- `CorpPresence/Services.cs:473` — `return L.K("companies.services.pay.paymentStatusUncertainCheckBalance", "付款状态异常，请先核对余额，避免重复付款。", "Payment status is uncertain. Check your balance before trying again.");`
- `CorpPresence/Services.cs:486` — `return L.K("companies.services.pay.notEnoughCash", "现金不够", "Not enough cash");`
- `CorpPresence/Services.cs:497` — `return L.K("companies.services.joinDealer.alreadyDealer", "已经是合作商了", "Already a dealer");`
- `CorpPresence/Services.cs:730` — `NightLog(L.K("companies.services.nightly.rescueCrates.deliveryCrateSLeftCounter", $"柜台上的 {num} 只送货箱已收进背包。", (FormattableString)$"{num} delivery crate(s) left on the counter were moved into your bag."), null, isCurrent);`
- `CorpPresence/Services.cs:734` — `NightLog(L.K("companies.services.nightly.rescueCrates.bagFullDeliveryCrateS", $"背包放不下，柜台上还有 {num2} 只送货箱，过夜会被清掉。", (FormattableString)$"Your bag is full: {num2} delivery crate(s) remain on the counter and will be cleared overnight."), "9c3738", isCurrent);`
- `CorpPresence/Services.cs:750` — `string text = ((!(kind == "vip")) ? ((c is SpacerCo) ? L.K("companies.services.nightly.nightly.mutualAid", "街坊互助", "Mutual Aid") : L.K("companies.services.nightly.nightly.lowerFootTraffic", "下层客流", "Lower Foot Traffic")) : L.K("companies.services.nightly.nightly.vIPReferral", "贵宾引荐", "VIP Referral"));`
- `CorpPresence/Services.cs:771` — `NightLog(L.K("companies.services.nightly.nightly.servicePaused", c.Name + "的" + text + "未结算，已暂停：" + text2, (FormattableString)$"{c.Name} {text} paused: {text2}"), "9c3738", isCurrent);`
- `CorpPresence/Services.cs:783` — `string text3 = ((!(kind == "vip")) ? ((c is SpacerCo) ? L.K("companies.services.nightly.nightly.moreSpacersWillComeBy", "明天会多来几位星际客", "More Spacers will come by tomorrow") : L.K("companies.services.nightly.nightly.moreLowerLevelsCustomersWill", "明天会多来几位下层区顾客", "More Lower Levels customers will come by tomorrow")) : L.K("companies.services.nightly.n`
- `CorpPresence/Services.cs:784` — `NightLog(L.K("companies.services.nightly.nightly.paidCredits", $"{c.Name}{text}已付 {fee} 信用点，{text3}。", (FormattableString)$"{c.Name} {text}: paid {fee} credits. {text3}."), null, isCurrent);`
- `CorpPresence/Services.cs:815` — `NightLog(L.K("companies.services.nightly.rebate.rebateCreditsCourierBringsTomorrow", $"{c.Name}返点 {num2} 信用点，送货员明天带来。", (FormattableString)$"{c.Name} rebate: {num2} credits. The courier brings it tomorrow."), "5c6f46", isCurrent);`
- `CorpPresence/Services.cs:850` — `return L.K("companies.services.orders.placeOrder.saveNotReady", "存档未就绪", "Save not ready");`
- `CorpPresence/Services.cs:854` — `return L.K("companies.services.orders.placeOrder.alreadyOrderedToday", "今天已经下过单了", "Already ordered today");`
- `CorpPresence/Services.cs:863` — `return L.K("companies.services.orders.placeOrder.orderNotReadyHaveNot", "订单未就绪，尚未扣款。", "Order not ready. You have not been charged.");`
- `CorpPresence/Services.cs:868` — `return L.K("companies.services.orders.placeOrder.perfumerySuppliesRequireGoldMembership", "香水工艺供应仅向金卡及以上会员开放。", "Perfumery supplies require Gold membership or above.");`
- `CorpPresence/Services.cs:872` — `return L.K("companies.services.orders.placeOrder.someItemsAreUnavailableRight", "有货品暂不可用，请稍后再试。尚未扣款。", "Some items are unavailable right now. Try again later. You have not been charged.");`
- `CorpPresence/Services.cs:885` — `return L.K("companies.services.orders.placeOrder.overLimitCanOrderMore", $"{key.Name()}超出限购，还能订 {Left(s, key)} 份", (FormattableString)$"{key.Name()} is over the limit. You can order {Left(s, key)} more");`
- `CorpPresence/Services.cs:905` — `return L.K("companies.services.orders.placeOrder.itemIdentifierNotReadyHave", "货品标识未就绪，尚未扣款。", "Item identifier not ready. You have not been charged.");`

Total matching lines: 1062

## L.T(


Total matching lines: 0
