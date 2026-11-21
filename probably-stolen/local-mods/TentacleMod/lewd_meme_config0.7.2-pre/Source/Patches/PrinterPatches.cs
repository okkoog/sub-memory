using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 触手3D打印机接管(目录单元 main.printer——五键共用一单元,开关任一开
    /// 且版本受支持时装配;基线主 mod 1.9.2,1.7.1 新增线):0.7.0 Task 7 起
    /// 七接管点两段拆分——
    ///  <list type="bullet">
    ///   <item>价格两键 postfix(Create* 尾 ApplyCsv 之后恒胜,方法体集中
    ///     ValuePostfixPatches):printer.price=500(CreatePrinter unitValue)、
    ///     printer.needle_price=15(CreateNeedle unitValue);1.9.2 的 tags/
    ///     PRINT_PROGRESS_X10=0/Register/Slots 登记全部归原方法照跑(零复刻)。</item>
    ///   <item>数值三键 transpiler(TranspilePlan F 节,恒 Own 无让位):
    ///     printer.grid=12(BuildWindow 三组 (12,12) 六处 ldc.i4.s 12 全集
    ///     替换,Count=6 锁量);printer.recipe_cost=15(NightlyTickOnce 完工
    ///     阈值 ldc.i4 150 = 15×10 折叠,getter 返回 cfg×10,进度存储 ×10);
    ///     printer.base_progress=3f(DailyProgress 每苗床基础进度 ldc.r4 3)。
    ///     出针循环/耗材消耗/「一夜造出 N 支空针管」日志、IntelBonus 智力
    ///     加成、ResolveGrids 全部归原方法照跑。</item>
    ///   <item>文案两 prefix 保留(0.6.0 增强行为,1.9.2 与 1.7.1 逐字一致
    ///     已核——RecipeTip 的「所需进度/当前进度」两拼接值与 RefreshDescription
    ///     的进度分母/天数估算/每苗床基础 N 点读配置,使面板文案与 transpile
    ///     后的实际数值同源;DailyProgress/CountMaterials 经委托调用,前者过
    ///     Harmony 链 = 读 base_progress 注入值)。</item>
    ///  </list>
    /// 全默认(无 override)时与 1.9.2 原版逐项一致 = 验收红线;默认全关 =
    /// 单元不装配,打印机线纯原版(零 hook)。
    /// 0.6.0 的 Create 系复刻体(CreatePrinterBody/BuildWindowBody/
    /// CreateNeedleBody/NightlyOnceBody/DailyProgressBody)与 13 个主 mod 委托
    /// 缓存随 T7 退役;主 mod 类型(TentaclePrinter/SpriteInjector 拆除后的
    /// ItemFactory[RectShape 拆除]/Toast 拆除——现仅 TentaclePrinter/ModCtx)
    /// 运行时经 ModReflect 解析。transpiler 装配经 TranspileCarrier.
    /// PatchTranspiler 幂等挂载;失败由 HookInstaller 逐单元隔离记 Dead。
    /// </summary>
    public static class PrinterPatches
    {
        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充;
        //    0.7.0 T7:Create 系复刻体退役,仅剩文案两 prefix 的消费面)--
        private static bool resolved;                                          // 全部成员就位后才置真
        private static System.Type printer;                                  // TentaclePrinter(补丁目标 + RECIPES 静态)
        private static string[] recipes;                                      // TentaclePrinter.RECIPES(static readonly,引用缓存;内容恒 "空针管" 1 项)
        private static Func<GameItem, float> dailyProgress;                   // TentaclePrinter.DailyProgress(经 Harmony 链 → transpiler 读 base_progress)
        private static Func<GameItem, string, int> tpGetInt;                  // TentaclePrinter.GetInt(private 包装,直调原实现)
        private static Func<GameItem, int> countMaterials;                    // TentaclePrinter.CountMaterials(private,直调原实现)
        private static Action<object> mainLogWarning;                         // ModCtx.LogWarning

        /// <summary>把 main.printer 单元的装配委托登记进映射(catalog-id →
        /// install;Harmony 实例由安装器转交,返回七个接管点的 original 清单
        /// 供记账——五键共用一单元,价格 postfix ×2 + 数值 transpiler ×3 +
        /// 文案 prefix ×2 在同一委托内一次装配)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("main.printer", h =>
            {
                EnsureResolved();
                MethodBase createPrinter = ModReflect.Target(printer, "CreatePrinter", Type.EmptyTypes);
                MethodBase createNeedle = ModReflect.Target(printer, "CreateNeedle", Type.EmptyTypes);
                MethodBase buildWindow = ModReflect.Target(printer, "BuildWindow",
                    new[] { typeof(GameItem), typeof(List<GameGridInventory>).MakeByRefType() });
                MethodBase nightlyOnce = ModReflect.Target(printer, "NightlyTickOnce", new[] { typeof(GameItem) });
                MethodBase dailyProgressM = ModReflect.Target(printer, "DailyProgress", new[] { typeof(GameItem) });
                MethodBase recipeTipM = ModReflect.Target(printer, "RecipeTip", new[] { typeof(GameItem) });
                MethodBase refreshDesc = ModReflect.Target(printer, "RefreshDescription", new[] { typeof(GameItem) });
                // 价格 postfix(方法体集中 ValuePostfixPatches;ApplyCsv 尾调之后恒胜)
                h.Patch(createPrinter, postfix: new HarmonyMethod(ValuePostfixPatches.PrinterPricePostfix));
                h.Patch(createNeedle, postfix: new HarmonyMethod(ValuePostfixPatches.PrinterNeedlePricePostfix));
                // 数值三键 transpiler(TranspilePlan printer_grids/
                // printer_nightly_transpile/printer_progress 三 methodId)
                TranspileCarrier.PatchTranspiler(h, buildWindow);
                TranspileCarrier.PatchTranspiler(h, nightlyOnce);
                TranspileCarrier.PatchTranspiler(h, dailyProgressM);
                // 文案两 prefix(1.9.2 与 1.7.1 逐字一致,增强行为保留)
                h.Patch(recipeTipM, prefix: new HarmonyMethod(RecipeTipPrefix));
                h.Patch(refreshDesc, prefix: new HarmonyMethod(RefreshDescriptionPrefix));
                return new[] { createPrinter, createNeedle, buildWindow, nightlyOnce,
                               dailyProgressM, recipeTipM, refreshDesc };
            });
        }

        /// <summary>
        /// 幂等解析主 mod 成员(单元内共享;任一成员解析抛出 → 原样上抛、
        /// 不提交任何字段,条目不装——两阶段提交)。失败由 HookInstaller
        /// 逐单元隔离记 Dead,该组不装配。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type tentaclePrinter = FindMainType("TentaclePrinter");
            System.Type ctx = FindMainType("ModCtx");
            var resolvedRecipes = ModReflect.GetStatic<string[]>(tentaclePrinter, "RECIPES");
            var resolvedDaily = ModReflect.StaticMethod<Func<GameItem, float>>(tentaclePrinter, "DailyProgress");
            var resolvedGet = ModReflect.StaticMethod<Func<GameItem, string, int>>(tentaclePrinter, "GetInt");
            var resolvedCount = ModReflect.StaticMethod<Func<GameItem, int>>(tentaclePrinter, "CountMaterials");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            // 全部成功,一次性提交
            printer = tentaclePrinter;
            recipes = resolvedRecipes;
            dailyProgress = resolvedDaily;
            tpGetInt = resolvedGet;
            countMaterials = resolvedCount;
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
        // RecipeTip:生产方案提示文案(prefix 复刻保留,1.9.2 逐字一致)
        // ------------------------------------------------------------------

        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool RecipeTipPrefix(GameItem machine, ref string __result)
        {
            int recipeCost;
            try
            {
                recipeCost = Cfg(CfgKeys.PrinterRecipeCost, 15);
            }
            catch (Exception ex)
            {
                LogMainWarning("打印方案文案配置读取失败,放行原版: " + ex.Message);
                return true; // 纯查询,原版重跑安全
            }
            try
            {
                __result = RecipeTipBody(machine, recipeCost);
            }
            catch (Exception ex)
            {
                LogMainWarning("打印方案文案生成失败,放行原版: " + ex.Message);
                return true; // 纯查询:放行原版
            }
            return false;
        }

        /// <summary>逐行复刻 1.7.1/1.9.2 RecipeTip(两版逐字一致);差异仅「所需
        /// 进度」两处拼接值 printer.recipe_cost(默认 15 = 原版字面量)。</summary>
        private static string RecipeTipBody(GameItem machine, int recipeCost)
        {
            int num = UnityEngine.Mathf.Clamp(tpGetInt(machine, "PRINT_RECIPE"), 0, recipes.Length - 1);
            int progress = tpGetInt(machine, "PRINT_PROGRESS_X10");
            return "所选方案：" + recipes[num] + "\n所需材料：3D打印耗材 ×1\n所需进度：" + recipeCost + "\n当前进度：" + ((float)progress / 10f).ToString("0.0") + " / " + recipeCost + "\n\n点一下切换方案。生产要等夜间按苗床进度结算。";
        }

        // ------------------------------------------------------------------
        // RefreshDescription:打印机描述文案(prefix 复刻保留,1.9.2 逐字一致)
        // ------------------------------------------------------------------

        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool RefreshDescriptionPrefix(GameItem machine)
        {
            int recipeCost;
            float baseProgress;
            try
            {
                recipeCost = Cfg(CfgKeys.PrinterRecipeCost, 15);
                baseProgress = Cfg(CfgKeys.PrinterBaseProgress, 3f);
            }
            catch (Exception ex)
            {
                LogMainWarning("打印机描述配置读取失败,放行原版: " + ex.Message);
                return true;
            }
            try
            {
                RefreshDescriptionBody(machine, recipeCost, baseProgress);
            }
            catch (Exception ex)
            {
                LogMainWarning("刷新打印机介绍失败(接管复刻): " + ex.Message);
            }
            return false;
        }

        /// <summary>逐行复刻 1.7.1/1.9.2 RefreshDescription(两版逐字一致);差异仅
        /// 文案中每苗床基础进度(base_progress,默认 3 → 串逐字相同)、未完成进度
        /// 分母与产完天数估算的 recipe_cost(默认 15 = 原版)。内层 try/catch 与
        /// 「刷新打印机介绍失败: 」警告为原版语义(LogMainWarning 即 ModCtx.
        /// LogWarning 委托)。DailyProgress 经委托调用(过 Harmony 链读 transpile
        /// 后的 base_progress 注入值,文案与实际进度同源)。</summary>
        private static void RefreshDescriptionBody(GameItem machine, int recipeCost, float baseProgress)
        {
            if (machine == null)
            {
                return;
            }
            try
            {
                float num = dailyProgress(machine);
                int progress = tpGetInt(machine, "PRINT_PROGRESS_X10");
                int num2 = countMaterials(machine);
                string text = ((num <= 0f) ? "—（苗床区还没有少女）" : ((num2 > 0) ? (UnityEngine.Mathf.CeilToInt((float)(num2 * recipeCost) / num) + " 天") : "—（材料区没有耗材）"));
                machine.shortDescription = "蠕动，吞吐，以及一如既往的内部触手异次元，在被你的触手侵蚀成你身体的一部分后，现在工作不需要电了，而是需要美少女，里面其实是让你的苗床操控精细触手进行工作。\n\n【材料区】放 3D 打印耗材\n【苗床区】放少女：每夜按智力提供打印进度\n【产出区】成品放这里\n\n当前每日进度：" + num.ToString("0.0") + "（每苗床基础 " + baseProgress.ToString("0.###") + " 点 + 智力加成）\n未完成进度：" + ((float)progress / 10f).ToString("0.0") + " / " + recipeCost + "\n材料区剩 " + num2 + " 份，全部生产完约需 " + text;
            }
            catch (Exception ex)
            {
                LogMainWarning("刷新打印机介绍失败: " + ex.Message);
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
        private static void LogMainWarning(object msg)
        {
            if (mainLogWarning != null) mainLogWarning(msg);
            else UnityEngine.Debug.LogWarning("[lewd_meme_config→主mod] " + msg);
        }
    }
}
