using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace LewdMemeConfig
{
    /// <summary>
    /// 掌上青楼经济接管(M5a,目录单元 brothel.* ×9,Kind = SubModHook,TargetMod
    /// = lewd_meme_brothel,绑定版本 0.10.4;开关开且版本命中时装配,装配路径与
    /// MainHook 相同——HookRuntime R-N)。brothel 无任何配置系统,数值全部 const
    /// 编译期内联(改字段无效),故逐方法 prefix 复刻:方法体与反编译
    /// brothel_0104 逐字对齐(各复刻体注释标 file:line),仅把常量换成
    /// <c>PatchCtx.Config.Current&lt;int&gt;</c> 读数(默认值 = 原版字面量)。
    /// 单元 → 方法(decomp 相对路径行号):
    ///  <list type="bullet">
    ///   <item>brothel.budget — Rules/ServiceMath.cs:29 EffectiveBudget</item>
    ///   <item>brothel.debt — Rules/ServiceMath.cs:43 SellsGoods + :56 PriceHint
    ///     + PalmPage.cs:134 PriceHintLine(私有静态 UI 文本,「静态方法优先」)</item>
    ///   <item>brothel.guests — Rules/GuestCapacity.cs:29 GuestCount + :34
    ///     GuestsAfterUpgrade + BrothelState.cs:55 Normalize(postfix 再钳,取舍见
    ///     NormalizePostfix 注释)+ PalmWindow.cs:66 EnsurePages +
    ///     PalmBrothelService.cs:244 AutoLockAll——上限 20 消费于 5 处,逐处接管</item>
    ///   <item>brothel.traits — Rules/TraitCategories.cs:36 RequirementCount</item>
    ///   <item>brothel.fee — Rules/MaidenTraits.cs:56 ClampFee + :61 ResolveFee +
    ///     MaidenMenuUi.cs:62 ShowFeeDialog(静态 UI 文本 "0 - 9999" 随动)</item>
    ///   <item>brothel.mastery — MaidenCatalog.cs:224 TrainOneNight</item>
    ///   <item>brothel.schedule — Rules/MerchantSchedule.cs:12 NextVisitDay</item>
    ///   <item>brothel.prices — BrothelItemCatalog.cs:293 ItemValue(私有静态)</item>
    ///   <item>brothel.stock — Rules/GearCatalog.cs:44 RollStock</item>
    ///  </list>
    /// 零漂移红线:各 prefix 先比较「全部消费键 == 原版字面量」,相等即放行原方法
    /// (装上且默认值 = 原版行为);默认全关 = 单元不装(穿透原则)。取值兜底:
    /// PatchCtx 未注入 / mod 停用时 ?? 原版常量(等价放行)。
    /// 取舍记载:PalmPage.Refresh(PalmPage.cs:68)工具提示中的 9999 为实例 UI
    /// 方法(整页构建体,复刻不可行)——按控制器裁定只接管 Clamp 语义,该提示
    /// 文本不随动(见任务报告);BrothelState.Normalize 同为实例方法且体内含
    /// 存档迁移,采用 postfix 再钳(默认 20 时纯 no-op)。
    /// 类型分工(AlarmPatch 先例):brothel 类型一律运行时经
    /// <c>ModReflect.Types(FamilyMods.Brothel)</c> 按 Name 解析(前置已声明,
    /// manifest Prerequisites 合法化),不编译引用其 DLL;游戏类型(GameItem/
    /// MachineryTabs/CustomUI*/GameSlotInventory 等)直接编译引用。解析只发生在
    /// 装配委托内(两阶段提交:EnsureResolved 任一成员失败 → 原样上抛不提交,
    /// 9 个条目一致不装);安装失败(版本漂移致签名变)由 HookInstaller 逐单元
    /// 隔离记 Dead。
    /// </summary>
    public static class BrothelPatches
    {
        // -- brothel 类型/成员的运行时缓存(install 期经 ModReflect 填充,prefix 只做直接调用) --
        private static bool resolved;
        private static System.Type serviceMath;         // Rules.ServiceMath
        private static System.Type guestCapacity;       // Rules.GuestCapacity
        private static System.Type traitCategories;     // Rules.TraitCategories
        private static System.Type maidenTraits;        // Rules.MaidenTraits
        private static System.Type merchantSchedule;    // Rules.MerchantSchedule
        private static System.Type gearCatalog;         // Rules.GearCatalog
        private static System.Type brothelItemCatalog;  // BrothelItemCatalog
        private static System.Type maidenCatalog;       // MaidenCatalog
        private static System.Type brothelStateType;    // BrothelState(Normalize postfix 目标/EnsurePages 签名)
        private static System.Type palmWindow;          // PalmWindow
        private static System.Type palmService;         // PalmBrothelService
        private static System.Type palmPage;            // PalmPage(PriceHintLine 目标)
        private static System.Type maidenMenuUi;        // MaidenMenuUi(ShowFeeDialog 目标)
        private static IReadOnlyList<string> upgradeIds;      // GuestCapacity.UpgradeIds(3 项)
        private static int traitCategoryCount;                // TraitCategories.All.Length(8)
        private static string[] countTags;                    // MaidenTraits.CountTags(3 项)
        private static string[] masteryTags;                  // MaidenTraits.MasteryTags(3 项)
        private static IReadOnlyList<string> firstVisitItems; // GearCatalog.FirstVisitItemIds(首访固定 3 件)
        private static IReadOnlyList<string> allGearItems;    // GearCatalog.AllItemIds(6 件)
        private static IDictionary palmViews;                 // PalmWindow.Views(静态只读字典,身份稳定)

        // -- 委托缓存(静态方法,含私有/internal——ModReflect.StaticMethod 合法) --
        private static Func<object> pwFindOwned;                 // PalmWindow.FindOwned()(无参,返回 GameItem?)
        private static Func<GameItem, int, object> pwPageAt;      // PalmWindow.Page(GameItem, int)
        private static Func<GameItem, int, object> pwCreatePage;  // PalmWindow.CreatePage(GameItem, int)
        private static Func<object> repoLoad;                     // BrothelRepository.Load()
        private delegate bool TryLockDelegate(int page, out string error);
        private static TryLockDelegate pbsTryLock;                // PalmBrothelService.TryLock(int, out string)
        private static Func<GameItem, string, int> mcCount;       // MaidenCatalog.Count(GameItem?, string)
        private static Func<GameItem, bool> mcHasAllMasteries;    // MaidenCatalog.HasAllMasteries(GameItem)(私有)
        private static Func<GameItem, int> mcFee;                 // MaidenCatalog.Fee(GameItem?)
        private static Func<int, string> mtCountTagFor;           // MaidenTraits.CountTagFor(int)
        private static Func<string, string> mtMasteryTagFor;      // MaidenTraits.MasteryTagFor(string)
        private static Func<string, object[], string> rtText;     // BrothelRuntime.Text(string, params object[])
        private static Action<string> rtWarn;                     // BrothelRuntime.Warn(string)
        private static Action<string, Exception> rtError;         // BrothelRuntime.Error(string, Exception)
        private static Func<GameItem, string> mmuName;            // MaidenMenuUi.Name(GameItem)(私有)
        private static Action<GameItem> mmuCommitFromInput;       // MaidenMenuUi.CommitFromInput(GameItem)(私有)
        private static Action mmuClose;                           // MaidenMenuUi.Close()

        /// <summary>把 9 个条目的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("brothel.budget", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(serviceMath, "EffectiveBudget",
                    new[] { typeof(int), typeof(int) });
                h.Patch(m, prefix: new HarmonyMethod(EffectiveBudgetPrefix));
                return new[] { m };
            });
            installs.Add("brothel.debt", h =>
            {
                EnsureResolved();
                MethodBase sells = ModReflect.Target(serviceMath, "SellsGoods",
                    new[] { typeof(int), typeof(int), typeof(int), typeof(int) });
                MethodBase hint = ModReflect.Target(serviceMath, "PriceHint",
                    new[] { typeof(int), typeof(int), typeof(int) });
                MethodBase line = ModReflect.Target(palmPage, "PriceHintLine",
                    new[] { typeof(int), typeof(int), typeof(int) });
                h.Patch(sells, prefix: new HarmonyMethod(SellsGoodsPrefix));
                h.Patch(hint, prefix: new HarmonyMethod(PriceHintPrefix));
                h.Patch(line, prefix: new HarmonyMethod(PriceHintLinePrefix));
                return new[] { sells, hint, line };
            });
            installs.Add("brothel.guests", h =>
            {
                EnsureResolved();
                MethodBase guestCount = ModReflect.Target(guestCapacity, "GuestCount",
                    new[] { typeof(Func<string, bool>), typeof(int) });
                MethodBase afterUpgrade = ModReflect.Target(guestCapacity, "GuestsAfterUpgrade",
                    new[] { typeof(int) });
                MethodBase normalize = ModReflect.Target(brothelStateType, "Normalize", Type.EmptyTypes);
                MethodBase ensurePages = ModReflect.Target(palmWindow, "EnsurePages",
                    new[] { typeof(GameItem), brothelStateType, typeof(int) });
                MethodBase autoLock = ModReflect.Target(palmService, "AutoLockAll", Type.EmptyTypes);
                h.Patch(guestCount, prefix: new HarmonyMethod(GuestCountPrefix));
                h.Patch(afterUpgrade, prefix: new HarmonyMethod(GuestsAfterUpgradePrefix));
                h.Patch(normalize, postfix: new HarmonyMethod(NormalizePostfix));
                h.Patch(ensurePages, prefix: new HarmonyMethod(EnsurePagesPrefix));
                h.Patch(autoLock, prefix: new HarmonyMethod(AutoLockAllPrefix));
                return new[] { guestCount, afterUpgrade, normalize, ensurePages, autoLock };
            });
            installs.Add("brothel.traits", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(traitCategories, "RequirementCount",
                    new[] { typeof(int) });
                h.Patch(m, prefix: new HarmonyMethod(RequirementCountPrefix));
                return new[] { m };
            });
            installs.Add("brothel.fee", h =>
            {
                EnsureResolved();
                MethodBase clamp = ModReflect.Target(maidenTraits, "ClampFee", new[] { typeof(int) });
                MethodBase resolve = ModReflect.Target(maidenTraits, "ResolveFee",
                    new[] { typeof(bool), typeof(int) });
                MethodBase dialog = ModReflect.Target(maidenMenuUi, "ShowFeeDialog",
                    new[] { typeof(GameItem) });
                h.Patch(clamp, prefix: new HarmonyMethod(ClampFeePrefix));
                h.Patch(resolve, prefix: new HarmonyMethod(ResolveFeePrefix));
                h.Patch(dialog, prefix: new HarmonyMethod(ShowFeeDialogPrefix));
                return new[] { clamp, resolve, dialog };
            });
            installs.Add("brothel.mastery", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(maidenCatalog, "TrainOneNight",
                    new[] { typeof(GameItem), typeof(int) });
                h.Patch(m, prefix: new HarmonyMethod(TrainOneNightPrefix));
                return new[] { m };
            });
            installs.Add("brothel.schedule", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(merchantSchedule, "NextVisitDay",
                    new[] { typeof(int), typeof(Func<int, int, int>) });
                h.Patch(m, prefix: new HarmonyMethod(NextVisitDayPrefix));
                return new[] { m };
            });
            installs.Add("brothel.prices", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(brothelItemCatalog, "ItemValue",
                    new[] { typeof(string) });
                h.Patch(m, prefix: new HarmonyMethod(ItemValuePrefix));
                return new[] { m };
            });
            installs.Add("brothel.stock", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(gearCatalog, "RollStock",
                    new[] { typeof(int), typeof(Func<int, int, int>) });
                h.Patch(m, prefix: new HarmonyMethod(RollStockPrefix));
                return new[] { m };
            });
        }

        /// <summary>
        /// 幂等解析全部 brothel 成员(9 个条目共享;任一成员解析抛出 → 原样上抛、
        /// 不提交任何字段,后续条目重试同样失败 → 9 个补丁一致不装,杜绝半解析
        /// 状态下装出运行时 NRE 的 prefix——两阶段提交,同 AlarmPatch)。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type svc = FindBrothelType("ServiceMath");
            System.Type cap = FindBrothelType("GuestCapacity");
            System.Type tc = FindBrothelType("TraitCategories");
            System.Type mt = FindBrothelType("MaidenTraits");
            System.Type ms = FindBrothelType("MerchantSchedule");
            System.Type gc = FindBrothelType("GearCatalog");
            System.Type bic = FindBrothelType("BrothelItemCatalog");
            System.Type mc = FindBrothelType("MaidenCatalog");
            System.Type st = FindBrothelType("BrothelState");
            System.Type pw = FindBrothelType("PalmWindow");
            System.Type pbs = FindBrothelType("PalmBrothelService");
            System.Type pp = FindBrothelType("PalmPage");
            System.Type mmu = FindBrothelType("MaidenMenuUi");
            System.Type rt = FindBrothelType("BrothelRuntime");
            System.Type repo = FindBrothelType("BrothelRepository");
            // 静态数据(readonly,读一次缓存)
            var rUpgradeIds = ModReflect.GetStatic<IReadOnlyList<string>>(cap, "UpgradeIds");
            var rTraitCount = ((Array)ModReflect.GetStatic(tc, "All")).Length;
            var rCountTags = ModReflect.GetStatic<string[]>(mt, "CountTags");
            var rMasteryTags = ModReflect.GetStatic<string[]>(mt, "MasteryTags");
            var rFirstVisit = ModReflect.GetStatic<IReadOnlyList<string>>(gc, "FirstVisitItemIds");
            var rAllItems = ModReflect.GetStatic<IReadOnlyList<string>>(gc, "AllItemIds");
            var rViews = (IDictionary)ModReflect.GetStatic(pw, "Views");
            // 委托(含私有/internal 成员)
            var rFindOwned = ModReflect.StaticMethod<Func<object>>(pw, "FindOwned");
            var rPageAt = ModReflect.StaticMethod<Func<GameItem, int, object>>(pw, "Page");
            var rCreatePage = ModReflect.StaticMethod<Func<GameItem, int, object>>(pw, "CreatePage");
            var rLoad = ModReflect.StaticMethod<Func<object>>(repo, "Load");
            var rTryLock = ModReflect.StaticMethod<TryLockDelegate>(pbs, "TryLock");
            var rCount = ModReflect.StaticMethod<Func<GameItem, string, int>>(mc, "Count");
            var rHasAll = ModReflect.StaticMethod<Func<GameItem, bool>>(mc, "HasAllMasteries");
            var rFee = ModReflect.StaticMethod<Func<GameItem, int>>(mc, "Fee");
            var rCountTagFor = ModReflect.StaticMethod<Func<int, string>>(mt, "CountTagFor");
            var rMasteryTagFor = ModReflect.StaticMethod<Func<string, string>>(mt, "MasteryTagFor");
            var rText = ModReflect.StaticMethod<Func<string, object[], string>>(rt, "Text");
            var rWarn = ModReflect.StaticMethod<Action<string>>(rt, "Warn");
            var rError = ModReflect.StaticMethod<Action<string, Exception>>(rt, "Error");
            var rName = ModReflect.StaticMethod<Func<GameItem, string>>(mmu, "Name");
            var rCommit = ModReflect.StaticMethod<Action<GameItem>>(mmu, "CommitFromInput");
            var rClose = ModReflect.StaticMethod<Action>(mmu, "Close");
            // 全部成功,一次性提交
            serviceMath = svc; guestCapacity = cap; traitCategories = tc; maidenTraits = mt;
            merchantSchedule = ms; gearCatalog = gc; brothelItemCatalog = bic; maidenCatalog = mc;
            brothelStateType = st; palmWindow = pw; palmService = pbs; palmPage = pp;
            maidenMenuUi = mmu;
            upgradeIds = rUpgradeIds; traitCategoryCount = rTraitCount;
            countTags = rCountTags; masteryTags = rMasteryTags;
            firstVisitItems = rFirstVisit; allGearItems = rAllItems; palmViews = rViews;
            pwFindOwned = rFindOwned; pwPageAt = rPageAt; pwCreatePage = rCreatePage;
            repoLoad = rLoad; pbsTryLock = rTryLock;
            mcCount = rCount; mcHasAllMasteries = rHasAll; mcFee = rFee;
            mtCountTagFor = rCountTagFor; mtMasteryTagFor = rMasteryTagFor;
            rtText = rText; rtWarn = rWarn; rtError = rError;
            mmuName = rName; mmuCommitFromInput = rCommit; mmuClose = rClose;
            resolved = true;
        }

        /// <summary>在前置 lewd_meme_brothel 暴露的类型里按名查找(同名不同命名空间亦命中;
        /// 缺席/未运行时 ModReflect.Types 抛 → install 失败记 Dead)。</summary>
        private static System.Type FindBrothelType(string name)
        {
            foreach (System.Type t in ModReflect.Types(FamilyMods.Brothel))
                if (t.Name == name)
                    return t;
            throw new MissingMemberException("前置 lewd_meme_brothel 未暴露类型 " + name);
        }

        /// <summary>配置读取兜底:PatchCtx 未注入(mod 停用后)→ 原版常量(等价放行)。</summary>
        private static int Cfg(string key, int originalDefault)
        {
            return PatchCtx.Config?.Current<int>(key) ?? originalDefault;
        }

        // ------------------------------------------------------------------
        // brothel.budget — ServiceMath.EffectiveBudget
        // ------------------------------------------------------------------

        /// <summary>Rules/ServiceMath.cs:29-32:
        /// <code>return Math.Max(0, baseBudget) * (100 + 10 * Math.Max(0, matches)) / 100;</code>
        /// 10(=BudgetBonusPercent,const 内联)→ brothel.budget_bonus(默认 10)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool EffectiveBudgetPrefix(int baseBudget, int matches, ref int __result)
        {
            int bonus = Cfg(CfgKeys.BrothelBudgetBonus, 10);
            if (bonus == 10) return true; // 默认零漂移:原方法原样运行

            __result = Math.Max(0, baseBudget) * (100 + bonus * Math.Max(0, matches)) / 100;
            return false;
        }

        // ------------------------------------------------------------------
        // brothel.debt — SellsGoods / PriceHint / PriceHintLine
        // ------------------------------------------------------------------

        /// <summary>Rules/ServiceMath.cs:43-53(嵌套结构逐字对齐):
        /// 超预算时 matches ≥ 5 → rollPercent &lt; 50,否则 false。5/50(const 内联)
        /// → brothel.debt_tags / brothel.debt_chance(默认 5/50)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool SellsGoodsPrefix(int goodsTotal, int effectiveBudget, int matches,
            int rollPercent, ref bool __result)
        {
            int tags = Cfg(CfgKeys.BrothelDebtTags, 5);
            int chance = Cfg(CfgKeys.BrothelDebtChance, 50);
            if (tags == 5 && chance == 50) return true;

            if (goodsTotal > Math.Max(0, effectiveBudget))
            {
                if (matches >= tags)
                {
                    __result = rollPercent < chance;
                    return false;
                }
                __result = false;
                return false;
            }
            __result = true;
            return false;
        }

        /// <summary>Rules/ServiceMath.cs:56-67:超预算时 matches &lt; 5 → "unsellable",
        /// 否则 "debt";不超 → null。5 → brothel.debt_tags(0.10.4 现版无调用方,
        /// 属预留 API——按控制器裁定随单元接管,防将来接线后漂移)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool PriceHintPrefix(int goodsTotal, int effectiveBudget, int matches,
            ref string __result)
        {
            int tags = Cfg(CfgKeys.BrothelDebtTags, 5);
            if (tags == 5) return true;

            if (goodsTotal > Math.Max(0, effectiveBudget))
            {
                if (matches < tags)
                {
                    __result = "unsellable";
                    return false;
                }
                __result = "debt";
                return false;
            }
            __result = null;
            return false;
        }

        /// <summary>PalmPage.cs:134-149(私有静态,页面提示行——ServiceMath.PriceHint
        /// 的实际渲染路径):noGoods/sure/unsellable/debt 四分支逐字对齐,
        /// 5 → brothel.debt_tags。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool PriceHintLinePrefix(int goodsTotal, int effectiveBudget, int matches,
            ref string __result)
        {
            int tags = Cfg(CfgKeys.BrothelDebtTags, 5);
            if (tags == 5) return true;

            if (goodsTotal <= 0)
            {
                __result = rtText("palm.hint.noGoods", new object[0]);
                return false;
            }
            if (goodsTotal <= effectiveBudget)
            {
                __result = rtText("palm.hint.sure", new object[0]);
                return false;
            }
            if (matches < tags)
            {
                __result = rtText("palm.hint.unsellable", new object[0]);
                return false;
            }
            __result = rtText("palm.hint.debt", new object[0]);
            return false;
        }

        // ------------------------------------------------------------------
        // brothel.guests — 上限 20 的 5 处消费(GuestCount/GuestsAfterUpgrade/
        // Normalize/EnsurePages/AutoLockAll),逐处接管
        // ------------------------------------------------------------------

        /// <summary>Rules/GuestCapacity.cs:29-32:
        /// <code>Math.Clamp(2 + 2 * UpgradeCount(isUnlocked) + Math.Max(0, showcaseMaidens), 2, 20)</code>
        /// 三个常量(InitialGuests/GuestsPerUpgrade/MaxGuests)→ 三键;UpgradeCount
        /// (:16-27)就地内联复刻。下界 Math.Min(b, m) 为防呆(b&gt;m 时
        /// Math.Clamp 抛异常——原版常量恒 2&lt;20 无此路径,面板可配出逆序)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool GuestCountPrefix(Func<string, bool> isUnlocked, int showcaseMaidens,
            ref int __result)
        {
            int b = Cfg(CfgKeys.BrothelGuestsBase, 2);
            int p = Cfg(CfgKeys.BrothelGuestsPerUpgrade, 2);
            int m = Cfg(CfgKeys.BrothelGuestsMax, 20);
            if (b == 2 && p == 2 && m == 20) return true;

            int num = 0;
            foreach (string upgradeId in upgradeIds)
            {
                if (isUnlocked(upgradeId))
                {
                    num++;
                }
            }
            __result = Math.Clamp(b + p * num + Math.Max(0, showcaseMaidens), Math.Min(b, m), m);
            return false;
        }

        /// <summary>Rules/GuestCapacity.cs:34-37:
        /// <code>Math.Clamp(2 + 2 * Math.Clamp(index, 0, UpgradeIds.Count), 2, 20)</code>
        /// 同 GuestCount 的三键替换;index 内层 Clamp(0, UpgradeIds.Count) 保留。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool GuestsAfterUpgradePrefix(int index, ref int __result)
        {
            int b = Cfg(CfgKeys.BrothelGuestsBase, 2);
            int p = Cfg(CfgKeys.BrothelGuestsPerUpgrade, 2);
            int m = Cfg(CfgKeys.BrothelGuestsMax, 20);
            if (b == 2 && p == 2 && m == 20) return true;

            __result = Math.Clamp(b + p * Math.Clamp(index, 0, upgradeIds.Count), Math.Min(b, m), m);
            return false;
        }

        /// <summary>BrothelState.cs:105(Normalize 尾行 PalmPages = Math.Clamp(PalmPages, 0, 20))
        /// ——取舍:Normalize 是实例方法且体内含存档 schema 迁移(:55-107,涉
        /// BrothelGuest 十余字段),prefix 全量复刻经 ModReflect 逐字段读写风险
        /// 远大于收益,故用 postfix 再钳:原方法跑完后按 brothel.guests_max 重钳
        /// PalmPages。m ≤ 20 时与「直接 clamp(x,0,m)」数学等价;m &gt; 20 时存档
        /// 值仍被原方法压到 20——PalmPages 只是已建页签数缓存(真实页数由
        /// Refresh→EnsurePages 从 Guests.Count 重建,见 PalmWindow.cs:243-249),
        /// 自愈无数据损失;默认 20 时纯 no-op(零漂移)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void NormalizePostfix(object __instance)
        {
            int m = Cfg(CfgKeys.BrothelGuestsMax, 20);
            if (m >= 20) return; // 默认零漂移(≥20 时原版 clamp(0,20) 已覆盖)

            int pages = ModReflect.Get<int>(__instance, "PalmPages");
            int clamped = Math.Clamp(pages, 0, m);
            if (pages != clamped)
            {
                ModReflect.Set(__instance, "PalmPages", clamped);
            }
        }

        /// <summary>PalmWindow.cs:66-91:Views 查找失败/空 → 0;desired 钳
        /// (1, 20) → (1, m);AddTab 循环(AddTab/Validate/文本 "palm.tab" 逐字,
        /// CreatePage/BrothelRuntime.Text 经委托直调原实现;Row 字段经 ModReflect
        /// 读出后按游戏基类 PixelElement 使用)。Views 为
        /// Dictionary&lt;GameItem, MachineryTabs&gt;,经非泛型 IDictionary 用同一
        /// 比较器查找。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool EnsurePagesPrefix(GameItem item, object state, int desired, ref int __result)
        {
            int m = Cfg(CfgKeys.BrothelGuestsMax, 20);
            if (m == 20) return true;

            if (!palmViews.Contains(item))
            {
                __result = 0;
                return false;
            }
            MachineryTabs value = (MachineryTabs)palmViews[item];
            if (value == null)
            {
                __result = 0;
                return false;
            }
            desired = Math.Clamp(desired, 1, m);
            bool flag = false;
            while (value.count < desired)
            {
                int count = value.count;
                object palmPage = pwCreatePage(item, count);
                value.AddTab(rtText("palm.tab", new object[] { count + 1 }),
                    (PixelElement)ModReflect.Get(palmPage, "Row"));
                flag = true;
            }
            if (!flag)
            {
                __result = value.count;
                return false;
            }
            if (item.contentWindow != null)
            {
                item.contentWindow.Validate();
            }
            value.Validate();
            __result = value.count;
            return false;
        }

        /// <summary>PalmBrothelService.cs:244-277:FindOwned → Load →
        /// Math.Min(Guests.Count, 20) → 逐页 IsWaiting/Maidens.childItem 检查后
        /// TryLock(i, out error)(失败/异常日志文案逐字)。20 → brothel.guests_max;
        /// FindOwned/Page/TryLock/BrothelRepository.Load/BrothelRuntime.Warn/Error
        /// 经委托直调原实现,Maidens(游戏类型 GameSlotInventory)直接编译访问。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool AutoLockAllPrefix()
        {
            int m = Cfg(CfgKeys.BrothelGuestsMax, 20);
            if (m == 20) return true;

            GameItem val = (GameItem)pwFindOwned();
            if (val == null)
            {
                return false; // 原版同路径:空手返回,无任何状态写入
            }
            object brothelState = repoLoad();
            IList guests = (IList)ModReflect.Get(brothelState, "Guests");
            int num = Math.Min(guests.Count, m);
            for (int i = 0; i < num; i++)
            {
                object guest = guests[i];
                if (!(bool)ModReflect.Get(guest, "IsWaiting"))
                {
                    continue;
                }
                object palmPage = pwPageAt(val, i);
                object maidens = palmPage != null ? ModReflect.Get(palmPage, "Maidens") : null;
                if ((maidens != null ? ((GameSlotInventory)maidens).childItem : null) == null)
                {
                    continue;
                }
                try
                {
                    string error;
                    if (!pbsTryLock(i, out error) && !string.IsNullOrEmpty(error))
                    {
                        rtWarn("自动接客第 " + (i + 1) + " 页失败：" + error);
                    }
                }
                catch (Exception error2)
                {
                    rtError("自动接客第 " + (i + 1) + " 页异常", error2);
                }
            }
            return false;
        }

        // ------------------------------------------------------------------
        // brothel.traits — TraitCategories.RequirementCount
        // ------------------------------------------------------------------

        /// <summary>Rules/TraitCategories.cs:36-39:
        /// <code>Math.Clamp(3 + Math.Max(0, upgrades), 3, All.Length)</code>
        /// 3(BaseRequirementCount,const 内联)→ brothel.traits_base;扩建增量为
        /// 原版隐式 ×1 → brothel.traits_per_upgrade;上界 All.Length(8)保留
        /// (All 数组运行时读取)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool RequirementCountPrefix(int upgrades, ref int __result)
        {
            int b = Cfg(CfgKeys.BrothelTraitsBase, 3);
            int p = Cfg(CfgKeys.BrothelTraitsPerUpgrade, 1);
            if (b == 3 && p == 1) return true;

            __result = Math.Clamp(b + p * Math.Max(0, upgrades), Math.Min(b, traitCategoryCount), traitCategoryCount);
            return false;
        }

        // ------------------------------------------------------------------
        // brothel.fee — ClampFee / ResolveFee / ShowFeeDialog
        // ------------------------------------------------------------------

        /// <summary>Rules/MaidenTraits.cs:56-59:
        /// <code>Math.Clamp(fee, 0, 9999)</code>——9999(MaxFee)→ brothel.fee_max。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool ClampFeePrefix(int fee, ref int __result)
        {
            int max = Cfg(CfgKeys.BrothelFeeMax, 9999);
            if (max == 9999) return true;

            __result = Math.Clamp(fee, 0, max);
            return false;
        }

        /// <summary>Rules/MaidenTraits.cs:61-68:无标签 → 120(DefaultFee)→
        /// brothel.fee_default;有标签 → ClampFee(storedValue) 就地展开为
        /// Math.Clamp(storedValue, 0, fee_max)(与原版调用链逐字等价)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool ResolveFeePrefix(bool hasFeeTag, int storedValue, ref int __result)
        {
            int def = Cfg(CfgKeys.BrothelFeeDefault, 120);
            int max = Cfg(CfgKeys.BrothelFeeMax, 9999);
            if (def == 120 && max == 9999) return true;

            if (!hasFeeTag)
            {
                __result = def;
                return false;
            }
            __result = Math.Clamp(storedValue, 0, max);
            return false;
        }

        /// <summary>MaidenMenuUi.cs:62-84(静态 UI 文本随动):输入框占位
        /// "0 - " + 9999 → "0 - " + fee_max;其余(窗口 id/尺寸/拖拽/Esc/标签/
        /// 按钮回调 CommitFromInput、Close、本地化键)逐字——CustomUI* 为游戏
        /// 类型直接编译引用,MaidenCatalog.Fee / MaidenMenuUi.Name /
        /// CommitFromInput / Close / BrothelRuntime.Text 经委托直调原实现。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool ShowFeeDialogPrefix(GameItem maiden)
        {
            int max = Cfg(CfgKeys.BrothelFeeMax, 9999);
            if (max == 9999) return true; // 默认零漂移(占位 "0 - 9999" 原样)

            GameItem maiden2 = maiden;
            if (!((UnityEngine.Object)(object)CustomUIManager.Instance == (UnityEngine.Object)null) && maiden2 != null)
            {
                int num = mcFee(maiden2);
                CustomUIBuilder val = CustomUIManager.Window("lewd_meme_brothel.fee",
                    rtText("fee.windowTitle", new object[0]), "overlay").SetSize(440f, 260f).SetDraggable(true)
                    .SetCloseOnEscape(true);
                val.AddLabel(mmuName(maiden2), "fee-maiden");
                val.AddLabel(rtText("fee.current", new object[] { num }), "fee-current");
                val.AddInput("0 - " + max, num.ToString(), null, "fee-input", null);
                val.AddLabel(rtText("fee.hint", new object[0]), "fee-hint");
                val.BeginRow(4f, (TextAnchor)0);
                val.AddButton(rtText("fee.confirm", new object[0]), delegate
                {
                    mmuCommitFromInput(maiden2);
                }, "fee-confirm");
                val.AddFlexibleSpace();
                val.AddButton(rtText("ui.close", new object[0]), mmuClose, "fee-close");
                val.End();
                val.Show();
            }
            return false;
        }

        // ------------------------------------------------------------------
        // brothel.mastery — MaidenCatalog.TrainOneNight
        // ------------------------------------------------------------------

        /// <summary>MaidenCatalog.cs:224-255:按日+uid 播种的次数标签 → +1 写回 →
        /// 次数 ≥ 30 且未精通 → 打精通标签;三系全精通 → 抹除并打 SUCCUBUS。
        /// 30(MasteryThreshold,const 内联)→ brothel.mastery_threshold;Count/
        /// HasAllMasteries(私有)/CountTagFor/MasteryTagFor/CountTags/MasteryTags
        /// 经委托与缓存数组直调原实现;ModifyTag/EnableTag/DisableTag/IsTag/
        /// uniqueId 为游戏 API 直接编译访问。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool TrainOneNightPrefix(GameItem item, int day, ref string __result)
        {
            int threshold = Cfg(CfgKeys.BrothelMasteryThreshold, 30);
            if (threshold == 30) return true;

            if (item == null)
            {
                __result = string.Empty;
                return false;
            }
            string text = mtCountTagFor(new System.Random(day * 131 + item.uniqueId).Next(countTags.Length));
            int current = mcCount(item, text) + 1;
            item.ModifyTag(text, delegate (TagState state)
            {
                state.Enable();
                state.SetInt(current);
            }, false);
            string text2 = mtMasteryTagFor(text);
            string result = string.Empty;
            if (current >= threshold && !item.IsTag(text2))
            {
                item.EnableTag(text2, false);
                result = text2;
            }
            if (mcHasAllMasteries(item) && !item.IsTag("BROTHEL_SUCCUBUS"))
            {
                string[] array = masteryTags;
                foreach (string text3 in array)
                {
                    item.DisableTag(text3, false);
                }
                item.EnableTag("BROTHEL_SUCCUBUS", false);
                __result = "BROTHEL_SUCCUBUS";
                return false;
            }
            __result = result;
            return false;
        }

        // ------------------------------------------------------------------
        // brothel.schedule — MerchantSchedule.NextVisitDay
        // ------------------------------------------------------------------

        /// <summary>Rules/MerchantSchedule.cs:12-15:
        /// <code>checked(currentDay + randomRange(4, 10))</code>——掷骰实得 4..9
        /// (Random.Range 上界排除):4/10 → min/max 两键(max 存 9,调用时 +1
        /// 还原排除上界);checked 溢出语义保留。max &lt; min 时钳回 min(面板可
        /// 逆序,防随机区间倒挂)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool NextVisitDayPrefix(int currentDay, Func<int, int, int> randomRange,
            ref int __result)
        {
            int min = Cfg(CfgKeys.BrothelVisitMinDays, 4);
            int max = Cfg(CfgKeys.BrothelVisitMaxDays, 9);
            if (min == 4 && max == 9) return true;

            if (max < min) max = min;
            __result = checked(currentDay + randomRange(min, max + 1));
            return false;
        }

        // ------------------------------------------------------------------
        // brothel.prices — BrothelItemCatalog.ItemValue
        // ------------------------------------------------------------------

        /// <summary>BrothelItemCatalog.cs:293-302(私有静态 switch 表):六件服饰
        /// 单价 → 六键(默认 260/180/220/240/175/150 = 原版字面量);未知 id →
        /// 100(原版 default,保留)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool ItemValuePrefix(string itemId, ref long __result)
        {
            int maidDress = Cfg(CfgKeys.BrothelPriceMaidDress, 260);
            int catEars = Cfg(CfgKeys.BrothelPriceCatEars, 180);
            int catTailPlug = Cfg(CfgKeys.BrothelPriceCatTailPlug, 220);
            int laceDress = Cfg(CfgKeys.BrothelPriceLaceDress, 240);
            int bunnyEars = Cfg(CfgKeys.BrothelPriceBunnyEars, 175);
            int laceGarter = Cfg(CfgKeys.BrothelPriceLaceGarter, 150);
            if (maidDress == 260 && catEars == 180 && catTailPlug == 220
                && laceDress == 240 && bunnyEars == 175 && laceGarter == 150)
            {
                return true; // 默认零漂移
            }

            switch (itemId)
            {
                case "lewd_meme_brothel.gear_maid_dress":
                    __result = maidDress;
                    return false;
                case "lewd_meme_brothel.gear_cat_ears":
                    __result = catEars;
                    return false;
                case "lewd_meme_brothel.gear_cat_tail_plug":
                    __result = catTailPlug;
                    return false;
                case "lewd_meme_brothel.gear_lace_dress":
                    __result = laceDress;
                    return false;
                case "lewd_meme_brothel.gear_bunny_ears":
                    __result = bunnyEars;
                    return false;
                case "lewd_meme_brothel.gear_lace_garter":
                    __result = laceGarter;
                    return false;
                default:
                    __result = 100;
                    return false;
            }
        }

        // ------------------------------------------------------------------
        // brothel.stock — GearCatalog.RollStock
        // ------------------------------------------------------------------

        /// <summary>GearCatalog.cs:44-63:首访(visitNumber ≤ 1)返回
        /// FirstVisitItemIds 固定 3 件(恰为 AllItemIds 目录序前 3)→
        /// brothel.stock_first(=3 时原样返回原静态列表本体;≠3 时按目录序取前 N
        /// 件,确定性同原版);后续 randomRange(3, 6) 实得 3..5 →
        /// stock_min/stock_max(调用时 +1 还原排除上界);无放回抽取循环逐字
        /// (AllItemIds.ToList() 快照 → 随机抽取并 RemoveAt)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool RollStockPrefix(int visitNumber, Func<int, int, int> randomRange,
            ref IReadOnlyList<string> __result)
        {
            int first = Cfg(CfgKeys.BrothelStockFirst, 3);
            int min = Cfg(CfgKeys.BrothelStockMin, 3);
            int max = Cfg(CfgKeys.BrothelStockMax, 5);
            if (first == 3 && min == 3 && max == 5) return true;

            if (visitNumber <= 1)
            {
                if (first == 3)
                {
                    __result = firstVisitItems; // 原版静态列表本体(引用同一)
                    return false;
                }
                int take = Math.Clamp(first, 0, allGearItems.Count);
                List<string> head = new List<string>(take);
                for (int i = 0; i < take; i++)
                {
                    head.Add(allGearItems[i]);
                }
                __result = head;
                return false;
            }
            if (max < min) max = min; // 面板可逆序,钳回单调
            int num = randomRange(min, max + 1);
            List<string> list = new List<string>(allGearItems); // = AllItemIds.ToList() 快照
            List<string> list2 = new List<string>(num);
            while (list2.Count < num && list.Count > 0)
            {
                int index = randomRange(0, list.Count);
                list2.Add(list[index]);
                list.RemoveAt(index);
            }
            __result = list2;
            return false;
        }
    }
}
