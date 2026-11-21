using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace LewdMemeConfig
{
    /// <summary>
    /// 夜间结算接管(0.6.0 目录单元 main.add_body_exp / main.build_window /
    /// main.create / main.produce_daughter / main.night_report 五个前缀载体 +
    /// 0.7.0 Task 4 新增的 main.nightly_transpile / main.nightly_milk_value 两个
    /// transpiler 单元,开关开且版本受支持时装配):
    ///  <list type="bullet">
    ///   <item>tentacle.nightly_* 两单元 — NightlyTick 不再 prefix 复刻(0.6.0 的
    ///     NightlyTickBody 已退役):数值面 14 键与乳汁面 2 键经 TranspileCarrier 把
    ///     原方法体内 16 处常量换成 getter 现读(TranspilePlan A/B 节注入点;两单元
    ///     共享 original,替换点不相交,让位语义不同故分单元——数值面只让 remake,
    ///     乳汁面另让 Brood[夜间重算 unitValue 同源覆盖]);RNG 消耗次序、Tag 写入、
    ///     报告计数、TryProduce 产出全部留在原方法体照跑,签名漂移面从「逐行复刻」
    ///     缩到「锚点窗口」,任一注入点未命中 → Dead(宁拒不静默半改)。min 系 5 键
    ///     (sex_per_level_min / oral_min_base / oral_per_level_min / anal_min_base /
    ///     anal_per_level_min)1.9.2 编译期折叠无消费点 → 不入单元(DataOnlyKeys,
    ///     面板行保留)</item>
    ///   <item>tentacle.add_body_exp — AddBodyExp(0.7.0 T9 复刻体退役 → 改参
    ///     prefix:nightly.exp_multiplier 默认 1 = 原版直加零漂移放行;≠1 时把
    ///     amount 换成 RoundToInt(amount×multiplier) 后放行原方法体。升级
    ///     循环/ModData 写入/升级日志全部归原方法照跑——1.9.2 循环无 cap 条件
    ///     (仅 num3&lt;10000 防死循环,经验公式经 main.exp_formula 单元接管,
    ///     body_lv 读 clamp 封顶归 main.level_cap),零复刻自动在场)</item>
    ///   <item>tentacle.build_window — BuildWindow transpiler(0.7.0 T7 复刻体退役;
    ///     layout 四键 = corrupt/tool cols ldc + brood/output ldloc 特形,注入点
    ///     TranspilePlan D 节;五区网格/过滤规则/便签区归原方法照跑)</item>
    ///   <item>tentacle.create — Create transpiler(0.7.0 T7 复刻体退役;仅
    ///     layout.tabs_per_level 单键,原版储藏页签 = bodyLevel×2 的 mul 乘子;
    ///     布局四键归 build_window 单元,原版 Create 调原版 BuildWindow 过
    ///     Harmony 链,布局注入对 Create 路径照常生效)</item>
    ///   <item>tentacle.produce_daughter — ProduceDaughter transpiler(0.7.0 T8
    ///     复刻体退役;nightly.daughter_value 默认 900 = 原版 ldc;1.9.2 新行为
    ///     [MAIDEN_DAUGHTER tag / NameGen 姓氏继承]在原方法体照跑 = 零复刻
    ///     自动在场,remake 让位[R1]随单元保持)</item>
    ///   <item>tentacle.night_report — ShowReport(ui.report_delay/width/height;原版
    ///     ModHook.Instance.StartCoroutine(DelayReport) 协程弹窗改经 ModHelper.Delay 排队——
    ///     12/10 安全更新的合法延迟通道,不再复刻主 mod 的旧协程调用)</item>
    ///  </list>
    /// 与原版的行为差异(sanctioned,详见 task-7-report.md;NightlyTick 线 0.7.0 起
    /// 无行为差异——原方法体照跑,仅常量换 getter,默认值 = 1.9.2 原常量零漂移;
    /// 0.7.0 T9:AddBodyExp 改参 prefix,默认 multiplier=1 时同样零漂移):
    ///  1) 配置读取集中在任何状态写入之前;读失败(键缺失/类型不符)→ 记日志放行原方法;
    ///  2) 复刻体(Phase B)异常 → 吞掉记日志并跳过原方法,防止半写状态被原版重放
    ///     (ShowReport 线;AddBodyExp 复刻体已随 T9 退役,升级写入归原方法体);
    ///     BuildWindow 为纯构造无副作用,其 Phase B 异常改为放行原方法;Create 复刻
    ///     失败置 null 返回(原版此处异常会直接传播崩调用方);
    ///  3) 布局映射:原版腐蚀/触手区高度与苗床/产出区共用 GRID_SIZE=16,拆键后腐蚀/触手区
    ///     高度取 brood_size(默认四区完全同原版);便签区 2×2、贴图 48×48、本体形状 3×3、
    ///     受孕上限 1f(Clamp01)、等级上限 5(GetBodyLevel 接管归 LevelCapPatch
    ///     的 main.level_cap 单元;T9 后升级循环无封顶,读 clamp 单元承担显示封顶)
    ///     均无本单元专属键 → 按原版字面量常量复刻;
    ///  4) ModHelper.Delay 用 WaitForSecondsRealtime(原版 WaitForSeconds 受 timescale 影响),
    ///     夜报在日结时弹出、正常倍速下等时;本 mod 停用时延迟回调由加载器自动取消。
    /// 主 mod 类型(TentacleBody/ModCtx/ItemFactory/SpriteInjector)一律运行时经 ModReflect
    /// 解析成委托/字段缓存,不编译引用 MidnightPawn.*;解析只发生在装配委托内
    /// (装配 = 物品目录初始化后,本 mod 已被加载器登记,ModReflect 合法),绝不进静态构造。
    /// 安装失败(主 mod 更新致签名漂移)由 HookInstaller 逐单元隔离记 Dead,不阻断其余补丁。
    /// </summary>
    public static class NightlyPatch
    {
        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充,prefix 只做直接调用;
        //    0.7.0 T4:NightlyTick 复刻体退役,NightlyTick 专属的 TryProduce*/DeflowerTracker*
        //    /GetInt 缓存随之拆除;T8:ProduceDaughter 复刻体退役,ItemFactory 的
        //    SetInt/GetStr/SetStr/UpdateMaidenDescription 缓存随之拆除;T9:
        //    AddBodyExp 复刻体退役,getBodyLevel/getBodyExp 缓存随之拆除——
        //    改参 prefix 不再触达等级/经验读取,全部归原方法体)--
        private static bool resolved;                                          // 全部成员就位后才置真
        private static System.Type tentacle;                                // TentacleBody(字段访问)
        private static Func<int> getToday;                                  // GetToday(private,ShowReport 文案)
        private static Action<object> mainLog;                              // ModCtx.Log
        private static Action<object> mainLogWarning;                       // ModCtx.LogWarning

        /// <summary>
        /// 把七条目的装配委托登记进映射(catalog-id → install;Harmony 实例由安装器
        /// 转交,返回本次补丁的 original 清单供记账)。
        /// 0.7.0 Task 4:main.nightly_tick prefix 复刻体退役 → 两个 transpiler
        /// 单元(数值面/乳汁面),remake 让位不再走本类的让位分支(委托恒产出,
        /// Router ③ Yield 过滤拦在登记期——remake 在场两单元均让位,其 NightlyTick
        /// transpiler 已删消费点区段,本 mod 注入点必不命中);让位日志由 ModEntry
        /// 统一打出。两单元共享 NightlyTick original:替换点不相交(TranspilePlan
        /// A/B 节),叠加安全;装配经 <see cref="TranspileCarrier.PatchTranspiler"/>
        /// 幂等挂载(T7 起公共形态),卸载侧由 HookInstaller 方法级引用计数裁决
        /// (tearl/CustomCustomer.Create 先例)。
        /// 0.7.0 Task 7:main.build_window / main.create 两布局复刻体退役 →
        /// transpiler 单元(id/Keys 见目录;两单元键全为不上面板的布局键,
        /// 开关恒 false 恒不装——SwitchDispatch 四条分派,与 0.6.0「键不上面板
        /// = 复刻体读默认恒 no-op」语义等价;原版 Create 调原版 BuildWindow 过
        /// Harmony 链,布局注入对 Create 路径照常生效)。
        /// 0.7.0 Task 8:main.produce_daughter prefix 复刻体退役 → transpiler
        /// 单元(id 不变,methodId 保持 main.produce_daughter_value)。
        /// </summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("main.nightly_transpile", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(tentacle, "NightlyTick",
                    new[] { typeof(GameItem), typeof(GameGridInventory), typeof(GameGridInventory) });
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            installs.Add("main.nightly_milk_value", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(tentacle, "NightlyTick",
                    new[] { typeof(GameItem), typeof(GameGridInventory), typeof(GameGridInventory) });
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            installs.Add("main.add_body_exp", h =>
            {
                EnsureResolved();
                // 0.7.0 T9:AddBodyExpPrefix/Body 复刻体退役 → 改参 prefix
                //(amount×multiplier 后放行;升级循环归原方法体,经验公式由
                // main.exp_formula 单元接管[level_cap 键归读 clamp 单元])
                MethodBase original = ModReflect.Target(tentacle, "AddBodyExp", new[] { typeof(int) });
                h.Patch(original, prefix: new HarmonyMethod(AddBodyExpPrefix));
                return new[] { original };
            });
            installs.Add("main.build_window", h =>
            {
                EnsureResolved();
                // out 参数在反射里的 ParameterType 是 ByRefType,签名须传 MakeByRefType()。
                // 0.7.0 T7:BuildWindowPrefix/Body 复刻体退役 → transpiler
                //(四键注入点含 brood/output ldloc 特形);1.9.2 布局重构为
                // 固定 5 面板,level 参数零消费(签名保留)。
                MethodBase original = ModReflect.Target(tentacle, "BuildWindow",
                    new[] { typeof(int), typeof(List<GameGridInventory>).MakeByRefType() });
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            installs.Add("main.create", h =>
            {
                EnsureResolved();
                // 0.7.0 T7:CreatePrefix/Body 复刻体退役 → transpiler(tabs_per_level
                // 单键);布局四键归 main.build_window 单元,原版 Create 调原版
                // BuildWindow 过 Harmony 链,布局注入对 Create 路径照常生效。
                MethodBase original = ModReflect.Target(tentacle, "Create", Type.EmptyTypes);
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            installs.Add("main.produce_daughter", h =>
            {
                EnsureResolved();
                // 0.7.0 T8:ProduceDaughterPrefix/Body 复刻体退役 → transpiler
                //(methodId 保持 main.produce_daughter_value,与单元 id 不同名——
                // printer 先例;让位过滤经登记期裁决兜底,ProduceDaughter 无兄弟
                // 单元共享 original)。1.9.2 行为零复刻自动在场:MAIDEN_DAUGHTER
                // tag(:899-903)、NameGen.FromSurname(SurnameOf) 姓氏继承
                // (:907-914)、unitValue 900(:896)全在原方法体照跑。
                MethodBase original = ModReflect.Target(tentacle, "ProduceDaughter", new[] { typeof(GameItem) });
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            installs.Add("main.night_report", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(tentacle, "ShowReport", Type.EmptyTypes);
                h.Patch(original, prefix: new HarmonyMethod(ShowReportPrefix));
                return new[] { original };
            });
        }

        /// <summary>
        /// 幂等解析全部主 mod 成员(七个条目共享;任一成员解析抛出 → 本方法原样上抛、
        /// 不提交任何字段,后续条目重试同样失败 → 全部补丁一致不装,杜绝半解析状态下
        /// 装出运行时 NRE 的 prefix)。失败由 HookInstaller 逐单元隔离记 Dead,该组不装配。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type ctx = FindMainType("ModCtx");
            System.Type body = FindMainType("TentacleBody");
            var resolvedGetToday = ModReflect.StaticMethod<Func<int>>(body, "GetToday");
            var resolvedLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            // 全部成功,一次性提交
            tentacle = body;
            getToday = resolvedGetToday;
            mainLog = resolvedLog;
            mainLogWarning = resolvedLogWarning;
            resolved = true;
        }


        /// <summary>在前置 lewd_meme 暴露的类型里按名查找(同名不同命名空间亦命中)。</summary>
        private static System.Type FindMainType(string name)
        {
            foreach (System.Type t in ModReflect.Types(MainModResolver.MainModId))
                if (t.Name == name)
                    return t;
            throw new MissingMemberException("主 mod lewd_meme 未暴露类型 " + name);
        }

        // ------------------------------------------------------------------
        // NightlyTick(0.7.0 Task 4:prefix 复刻体退役)
        // ------------------------------------------------------------------
        // 数值面 14 键(main.nightly_transpile)与乳汁面 2 键(main.nightly_milk_value)
        // 的载体已迁 TranspileCarrier:IL 注入点把原版常量换成 TranspileValues getter
        // 现读(注册表缺席/读失败回退 1.9.2 原常量 = 原版行为),RNG 消耗次序、
        // DeflowerTracker/Tag 写入、报告计数、TryProduce 产出全部留在主 mod 原方法体
        // 内照跑——本类不再持有结算流程,签名漂移面从「逐行复刻」缩到「锚点窗口」
        // (任一注入点未命中 → TranspileMatchException → Dead,不静默半改)。

        // ------------------------------------------------------------------
        // BuildWindow / Create(0.7.0 Task 7:prefix 复刻体退役)
        // ------------------------------------------------------------------
        // 布局四键(corrupt/tool cols ldc + brood/output ldloc 特形)与页签乘子
        // tabs_per_level 的载体已迁 TranspileCarrier(BuildWindow/Create 注入点,
        // TranspilePlan D 节):五区网格构造、双区过滤规则、便签区 2×2、页签 tag
        // 写入、贴图/形状、Instances/Slots 登记全部留在主 mod 原方法体内照跑
        // ——1.9.2 布局重构(固定 5 面板、level 零消费)零复刻地自动在场。
        // 两单元键全为不上面板的布局键 → 开关恒 false 恒不装(0.6.0
        // 「键不上面板 = 恒 no-op」语义等价;SwitchDispatch 四条分派)。

        // ------------------------------------------------------------------
        // AddBodyExp:经验倍率(0.7.0 Task 9:prefix 复刻体退役 → 改参 prefix)
        // ------------------------------------------------------------------

        /// <summary>
        /// 改参前缀:读 nightly.exp_multiplier(默认 1)→ amount ←
        /// RoundToInt(amount×multiplier) 后放行原方法体(1.9.2 :1112-1125 的
        /// 升级循环/ModData 写入/升级日志零复刻照跑)。默认 1 直接放行不改参
        /// = 原版逐字零漂移;读配置失败同样放行(尚未写任何状态,安全)。
        /// 复刻体时代(R3)的 EXP_NEED 注册表直读与 level_cap 升级封顶已随
        /// 1.9.2 公式化分别移交 main.exp_formula / main.level_cap 两单元。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool AddBodyExpPrefix(ref int amount)
        {
            float expMultiplier;
            try
            {
                expMultiplier = Cfg(CfgKeys.ExpMultiplier, 1f);
            }
            catch (Exception ex)
            {
                LogMainWarning("经验倍率读取失败,放行原版: " + ex.Message);
                return true;
            }
            if (expMultiplier == 1f) return true; // 默认零漂移:amount 原样进原方法
            amount = Mathf.RoundToInt(amount * expMultiplier);
            return true; // 放行原方法体(参数已被改写)
        }

        // ------------------------------------------------------------------
        // ProduceDaughter:孕期产女(0.7.0 Task 8:prefix 复刻体退役)
        // ------------------------------------------------------------------
        // 载体已迁 TranspileCarrier(TranspilePlan D 节 produce_daughter_value,
        // ldc.i4 900 单点注入):女儿生成/立绘继承/阵营继承/MAIDEN_DAUGHTER tag/
        // 姓氏继承(NameGen.SurnameOf :68 / FromSurname :87,委托零解析——原方法
        // 自家 call 照跑)/UpdateMaidenDescription/移库/孕态清零全部留在主 mod
        // 原方法体内照跑——1.9.2 新行为零复刻自动在场。remake 让位(R1)随单元
        // 保持:其 NightlyTick transpiler 绕过产女调用点,注入点无消费方。

        // ------------------------------------------------------------------
        // ShowReport:夜报文案与弹窗(协程 → ModHelper.Delay)
        // ------------------------------------------------------------------

        // HarmonyPriority(300):恒排 legacy 夜报类 prefix(默认 400)之后——
        // legacy 有内容时其 prefix 先短路 → 旧版夜报优先;无内容时本 mod 窗口照常
        //(spec §5.4 顺序裁决,R8)。
        [HarmonyPriority(300)]
        [HarmonyPrefix]
        private static bool ShowReportPrefix()
        {
            float reportDelay;
            try
            {
                reportDelay = Cfg(CfgKeys.UiReportDelay, 1.2f);
            }
            catch (Exception ex)
            {
                LogMainWarning("夜报配置读取失败,放行原版: " + ex.Message);
                return true;
            }
            try
            {
                ShowReportBody(reportDelay);
            }
            catch (Exception ex)
            {
                LogMainWarning("夜间报告失败: " + ex.Message);
            }
            return false;
        }

        /// <summary>逐行复刻 v1.3.8 ShowReport(报告统计仍写读主 mod 静态字段 ReportTypes/
        /// ReportMilk/ReportSemen/ReportMaids——停用本 mod 后原版自然接管同一份状态);
        /// 差异仅弹窗排队:ModHook.Instance.StartCoroutine(DelayReport) → ModHelper.Delay。</summary>
        private static void ShowReportBody(float reportDelay)
        {
            Dictionary<string, int> reportTypes = ModReflect.GetStatic<Dictionary<string, int>>(tentacle, "ReportTypes");
            int reportMilk = ModReflect.GetStatic<int>(tentacle, "ReportMilk");
            int reportSemen = ModReflect.GetStatic<int>(tentacle, "ReportSemen");
            if (reportTypes.Count == 0 && reportMilk == 0 && reportSemen == 0)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            if (reportTypes.Count > 0)
            {
                int n = 0;
                foreach (KeyValuePair<string, int> reportType in reportTypes)
                {
                    if (n > 0)
                    {
                        sb.Append("和");
                    }
                    sb.Append(reportType.Value).Append("个【").Append(reportType.Key).Append("】");
                    n++;
                }
                sb.Append("在【触手本体】内被你的触手调教了一夜");
            }
            else
            {
                sb.Append("触手本体蠕动了整夜");
            }
            List<string> list = new List<string>();
            if (reportMilk > 0)
            {
                list.Add("【神奇乳汁】*" + reportMilk);
            }
            if (reportSemen > 0)
            {
                list.Add("【触手精液】*" + reportSemen);
            }
            if (list.Count > 0)
            {
                sb.Append("，产出了 ").Append(string.Join("，", list.ToArray()));
            }
            else
            {
                sb.Append("，什么也没产出");
            }
            string text = sb.ToString();
            LogMain("夜间报告: " + text);
            try
            {
                int reportMaids = ModReflect.GetStatic<int>(tentacle, "ReportMaids");
                string text2 = ((list.Count <= 0)
                    ? ("第 " + getToday() + " 天：调教了 " + reportMaids + " 个苗床，无产出")
                    : ("第 " + getToday() + " 天：调教了 " + reportMaids + " 个苗床，产出 " + string.Join("、", list.ToArray())));
                ModHelper.SetModData(MainModResolver.MainModId, "last_report", text2);
            }
            catch
            {
            }
            try
            {
                // 12/10 安全更新替代通道:modId 前缀延迟,本 mod 停用时自动取消;
                // 原版对 ModHook.Instance 的 null 检查在此通道下不再需要。
                ModHelper.Delay(ModEntry.Id, reportDelay, () => ShowReportWindow(text));
                LogMain("夜间报告: 已排队弹窗");
            }
            catch (Exception ex)
            {
                LogMainWarning("夜间报告弹窗失败: " + ex.Message);
            }
        }

        /// <summary>原 DelayReport 协程体(yield 1.2s 之后的部分);窗口尺寸 ui.report_width /
        /// ui.report_height 在此读取(默认 480×240 = 原版 SetSize 字面量)。</summary>
        private static void ShowReportWindow(string text)
        {
            try
            {
                if (CustomUIManager.Instance == null)
                {
                    LogMainWarning("夜间报告: CustomUIManager.Instance 为 null");
                    return;
                }
                int w = Cfg(CfgKeys.UiReportWidth, 480);
                int h = Cfg(CfgKeys.UiReportHeight, 240);
                CustomUIManager.Window("lewd_meme.nightreport", "夜间报告", "overlay").SetSize(w, h).Center()
                    .SetDraggable(true)
                    .SetCloseOnEscape(true)
                    .AddLabel(text, null)
                    .AddSpace(8f)
                    .AddButton("知道了", (Action)delegate
                    {
                        CustomUIManager.Instance.CloseWindow("lewd_meme.nightreport");
                    }, null)
                    .Show();
                LogMain("夜间报告: 窗口已弹出");
            }
            catch (Exception ex)
            {
                LogMainWarning("夜间报告弹窗异常: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // 共用小件
        // ------------------------------------------------------------------

        /// <summary>配置读取:注册表缺席(未注入/停用)时回退原版字面量默认值。</summary>
        private static T Cfg<T>(string key, T def)
        {
            return PatchCtx.Config != null ? PatchCtx.Config.Current<T>(key) : def;
        }

        /// <summary>日志走主 mod ModCtx(保持原版前缀与格式);委托缺席时退回 Debug.Log。</summary>
        private static void LogMain(object msg)
        {
            if (mainLog != null) mainLog(msg);
            else Debug.Log("[lewd_meme_config→主mod] " + msg);
        }

        private static void LogMainWarning(object msg)
        {
            if (mainLogWarning != null) mainLogWarning(msg);
            else Debug.LogWarning("[lewd_meme_config→主mod] " + msg);
        }
    }
}
