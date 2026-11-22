using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 侵蚀触手接管(Task 8/9,目录单元 main.get_charge / main.set_charge /
    /// main.refresh_tooltip / main.daily_recharge / main.on_semen_used / 
    /// main.is_corruptible / main.corrupt_cost / main.initial_charge,开关开且版本
    /// 受支持时装配):prefix/postfix 接管主 mod <c>MidnightPawn.Game.
    /// CorruptTentacle</c>(静态类)方法 + 0.7.0 起 TryCorrupt(Task 5)/
    /// OnSemenUsed(Task 6)两复刻体退役改 transpiler 注入,接管体按反编译
    /// 逐行复刻(Task 14b 基线 v1.7.1;GetCharge/SetCharge/RefreshTooltip/
    /// DailyRecharge/拒绝日志/苗床箱主分支 1.6.1→1.7.1 零漂移,复刻体不动;
    /// IsCorruptible 复刻体随 1.9.2 增外部侵蚀规则透传行;TryCorrupt 复刻体
    /// 0.7.0 Task 5 退役——1.9.2 其方法体已加外部规则分支/扣能次序翻转/
    /// 交付改 ItemDelivery,逐行复刻的漂移面失控,corrupt_cost 六常量改经
    /// TranspileCarrier 注入,其余流程留在原方法体照跑;OnSemenUsed 复刻体
    /// 0.7.0 Task 6 同因退役——1.9.2 签名加精液瓶第二参 + usedSemen 按瓶
    /// 去重,semen_recharge 一点 + max_charge 两点改经 TranspileCarrier 注入),
    /// 充能常量与白/黑名单关键词换 CfgKeys 读数
    /// ——全默认(无 override)时与原版逐项一致 = 验收红线:
    ///  <list type="bullet">
    ///   <item>corrupt.get_charge — GetCharge(corrupt.max_charge;原版 Mathf.Clamp(…,0,100))</item>
    ///   <item>corrupt.set_charge — SetCharge(corrupt.max_charge;写 TENTACLE_CHARGE)</item>
    ///   <item>corrupt.refresh_tooltip — RefreshTooltip(max/daily/semen 三键;描述文案
    ///     的 100/10/10 拼接值,默认串与原版逐字相同;R2-F2 起单元并含
    ///     MaidenTooltip.OnTooltip 的 corrupt_tentacle 分支级接管——tooltip
    ///     标题区「充能 X / 100」行分母与(已充满)阈值换 max 键,其余物品
    ///     放行原方法零漂移)</item>
    ///   <item>corrupt.daily_recharge — DailyRecharge(corrupt.daily_recharge,满额
    ///     corrupt.max_charge 不充;逐实例 try/catch 复刻)</item>
    ///   <item>corrupt.on_semen_used — OnSemenUsed transpiler(0.7.0 Task 6,
    ///     即 brief 的 SemenRecharge;TranspilePlan C 节三点:semen_recharge
    ///     1 点[Min 首参]+ max_charge 2 点[门槛 blt/截断 sub],max>100 时
    ///     只注 semen 会在 100 处提前拒收——T2 顾虑①裁决两点补齐;toast 文本
    ///     「/100」拼接的 1 处不收,文本滞后是既定取舍)。1.9.2 双参签名
    ///     (GameItem tentacle, GameItem semen)+ usedSemen 按瓶去重(同帧
    ///     跨瓶可多次吸取)归原方法体照跑 = 零复刻;SetCharge/RefreshTooltip
    ///     调用由各自单元 prefix 接管</item>
    ///   <item>corrupt.is_corruptible — IsCorruptible(白/黑名单判定换
    ///     CorruptRules.Whitelisted/Blacklisted,csvBase 常量 = 原版关键词全集,
    ///     corrupt.whitelist_extra/blacklist_extra CSV 追加;精确 id 委托判定
    ///     不进关键词 CSV——闹钟(AlarmClock.IsPlainAlarm)、井盖
    ///     (TentacleTrap.IsPlainManhole)、crate 白名单之前增打印机
    ///     (TentaclePrinter.IsPlainPrinter,1.7.1 基线 L282-285)三白名单;
    ///     0.7.0 T5 按 1.9.2 增外部侵蚀规则透传行:打印机之后、关键词白名单
    ///     之前 TentacleApi.FindCorruptRule(it) != null → true(委托直调,
    ///     装配期一次解析;黑名单在外部规则之前 = 黑名单追加项压过外部 mod
    ///     注册的侵蚀规则);四个 id 排除、三个 tag 排除与 RejectLogged 拒绝
    ///     日志逐行复刻)</item>
    ///   <item>corrupt.corrupt_cost — TryCorrupt transpiler(0.7.0 Task 5,
    ///     TranspilePlan C 节六点:门槛 bge 1 处 + 扣减 sub/SetCharge 统一模式
    ///     5 处,ldc 100 → getter 现读 corrupt.corrupt_cost;仅入 Toast 文本的
    ///     1 处与 TryConvert 第 4 参 3 处不收——日志文本与配置值不一致是
    ///     spec §6 联动边界注记的既定取舍)。原方法体其余流程照跑:1.9.2 的
    ///     外部侵蚀规则分支(TentacleApi.FindCorruptRule → Do → ItemDelivery
    ///     交付,失败不扣能)、alarm/printer/manhole 三分流「先 TryConvert
    ///     成功才扣能」的次序翻转、苗床箱主分支,全部零复刻 = 零漂移;
    ///     名单判定经原方法内联的 IsCorruptible 调用走 is_corruptible 单元
    ///     前缀(该单元开 = 名单生效,与 0.6.0 复刻体内联形态的差别仅在
    ///     单元粒度,见任务报告)</item>
    ///  </list>
    /// 与原版的行为差异(sanctioned):
    ///  1) 配置读取集中在任何状态写入之前(Phase A);读失败(键缺失/类型不符)→
    ///     记日志放行原方法;纯查询(GetCharge/IsCorruptible)复刻体(Phase B)异常
    ///     亦放行原版(零状态写入,原版重跑安全),写状态方法(SetCharge/DailyRecharge/
    ///     RefreshTooltip)Phase B 异常吞掉记日志并跳过原版,
    ///     防半写状态被原版重放;corrupt_cost/on_semen_used 两单元为 transpiler,
    ///     无 Phase A/B 之分(getter 读失败回退默认常量,语义 = 原版值);
    ///  2) 无键 → 常量复刻(按指令不发明键):TENTACLE_CHARGE mod-data 键、四 id
    ///     排除(tentacle_box/corrupt_tentacle/tentacle_body/tentacle_manual)、
    ///     TENTACLE_CORRUPTION/MAIDEN_FEMALE/TENTACLE 三 tag 排除、拒绝日志的
    ///     IMPORTANT_TAG/CONTAINER_TAG/STANDARD_MACHINE_TAG 标注(IMPORTANT_TAG
    ///     在 1.6.1 仍只进拒绝日志,不作排除判定——as-is 复刻)、toast 标题"触手/
    ///     侵蚀"、semen_apply 音效与 tentacle_box/corrupt_tentacle 目录取自原版
    ///     字面量;
    ///  3) OnSemenUsed 前缀复刻体期(≤0.6.0)的 api 侧帧防重镜像
    ///     (semenFrame/semenTentacle)与 toast 字面量拼接已随 Task 6 复刻体
    ///     退役删除——1.9.2 原方法自带 usedSemen 按瓶去重(原版字段恢复
    ///     更新),两条 toast 全走原版字面量(吸取文案数值 X/100 中 100 为
    ///     原版常量:仅入文本不注入,max≠100 时 toast 显示滞后于实际充能,
    ///     行为正确的既定取舍,同 corrupt_cost)。
    /// 主 mod 类型(CorruptTentacle/ItemFactory/ModCtx/AlarmClock/TentacleTrap/
    /// TentaclePrinter[Task 14b,1.7.1]/TentacleApi[T5])一律运行时经 ModReflect 解析成委托/字段访问,
    /// 不编译引用 MidnightPawn.*;解析只发生在装配委托内(装配 = 物品目录初始化后,
    /// 本 mod 已被加载器登记,ModReflect 合法),绝不进静态构造。安装失败(主 mod
    /// 更新致签名漂移)由
    /// HookInstaller 逐单元隔离记 Dead,不阻断其余补丁;AlarmClock/
    /// TentacleTrap 的 RescanAll 归 ui.reload_fix 补丁类(SpecialAndManualPatches)
    /// 自持,本类只解析侵蚀转换链路成员。无协程/Invoke 调用面,不涉及 12/10 并发通道替换。
    /// </summary>
    public static class CorruptPatch
    {
        /// <summary>原版白名单关键词全集(1.3.8 与 1.6.1 一致,零漂移;4 英文
        /// identifier 子串 + 4 中文 name 关键词;brief 计 4+5,实数按反编译
        /// L282-301 为 8 项)。1.6.1 新增的闹钟/井盖为精确 id 委托判定,不在此列。</summary>
        internal const string WhitelistBase =
            "crate,storage,supplybox,supply_box,物资箱,货箱,物证柜,储藏区";

        /// <summary>原版黑名单 2 项(identifier 子串 lou_locker + name 含
        /// 卢的储物柜;原版对 lou_locker 的精确比较被大小写不敏感子串覆盖;
        /// 1.6.1 一致零漂移)。</summary>
        internal const string BlacklistBase = "lou_locker,卢的储物柜";

        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充,prefix 只做直接调用) --
        private static bool resolved;                                          // 全部成员就位后才置真
        private static System.Type corrupt;                                 // CorruptTentacle(字段访问)
        private static Func<GameItem, string, int> ifGetInt;                // 主 mod ItemFactory.GetInt
        private static Action<GameItem, string, int> ifSetInt;              // 主 mod ItemFactory.SetInt
        // 0.7.0 T6:Toast.Show 委托(toastShow)随 OnSemenUsed 复刻体退役删除
        //    ——最后消费者是吸取/已饱两条 toast,TryCorrupt 复刻体的 toast 随
        //    T5 先走,此后本类零 toast 调用面
        private static Action<object> mainLog;                              // ModCtx.Log
        private static Action<object> mainLogWarning;                       // ModCtx.LogWarning
        // -- 1.6.1 闹钟/井盖 + 1.7.1 打印机精确 id 白名单(委托直判;两 RescanAll
        //    归 ui.reload_fix 补丁类 SpecialAndManualPatches 自持。0.7.0 T5 起
        //    仅 IsCorruptible 复刻体消费——TryCorrupt 复刻体退役,三 TryConvert/
        //    SpillContents/IsAlive 委托随之拆除,侵蚀转换链路全留原方法体照跑)--
        private static Func<GameItem, bool> isPlainAlarm;                        // AlarmClock.IsPlainAlarm
        private static Func<GameItem, bool> isPlainManhole;                      // TentacleTrap.IsPlainManhole
        private static Func<GameItem, bool> isPlainPrinter;                      // TentaclePrinter.IsPlainPrinter(171 基线 L282-285)
        // 0.7.0 T5(1.9.2 基线):IsCorruptible 外部侵蚀规则透传——
        // MidnightPawn.Api.TentacleApi.FindCorruptRule(GameItem) → CorruptRule
        //(非 null 即可侵蚀,192 dump IsCorruptible IL_0105);返回类型为主 mod
        // 内嵌类,经 Func<in T, out TResult> 协变绑定为 object,复刻体只判非 null
        private static Func<GameItem, object> findCorruptRule;                   // TentacleApi.FindCorruptRule
        // R2-F2:主 mod MaidenTooltip.OnTooltip(经游戏 ModHook.OnCreateTooltipLate
        // 挂进物品 tooltip 渲染链,位于 shortDescription 行之前 = 标题区)对
        // corrupt_tentacle 追加「充能 X / 100」行,分母与(已充满)阈值是字面量
        // 100(decomp MaidenTooltip.cs L53-61)——与 main.refresh_tooltip 同读
        // corrupt.max_charge,并入该单元装配(分支级接管,其余物品放行原方法)
        private static System.Type maidenTooltip;                             // MaidenTooltip(标题行补丁目标)

        // 0.7.0 T6:OnSemenUsed 帧防重镜像(semenFrame/semenTentacle)随 prefix
        //    复刻体退役删除——1.9.2 原方法自带 usedSemen HashSet 按瓶去重
        //    (同帧跨瓶可多次吸取),原版字段恢复更新,自持镜像不再需要

        /// <summary>把八条目的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("main.get_charge", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(corrupt, "GetCharge", new[] { typeof(GameItem) });
                h.Patch(original, prefix: new HarmonyMethod(GetChargePrefix));
                return new[] { original };
            });
            installs.Add("main.set_charge", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(corrupt, "SetCharge", new[] { typeof(GameItem), typeof(int) });
                h.Patch(original, prefix: new HarmonyMethod(SetChargePrefix));
                return new[] { original };
            });
            installs.Add("main.refresh_tooltip", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(corrupt, "RefreshTooltip", new[] { typeof(GameItem) });
                h.Patch(original, prefix: new HarmonyMethod(RefreshTooltipPrefix));
                // R2-F2:标题区同步刷新——主 mod MaidenTooltip.OnTooltip 经游戏
                // ModHook.OnCreateTooltipLate 挂在 tooltip 渲染链 shortDescription
                // 行之前,其 corrupt_tentacle 分支的「充能 X / 100」分母是字面量
                // 100(0x03:长描述已随配置刷新而标题区仍 150/100);与 RefreshTooltip
                // 同单元装配(同读 corrupt.max_charge),双 original 记账
                // (HookInstaller 方法级引用计数天然支持)
                MethodBase tooltipHook = ModReflect.Target(maidenTooltip, "OnTooltip",
                    new[] { typeof(RichTextBuilder), typeof(GameItem) });
                h.Patch(tooltipHook, prefix: new HarmonyMethod(MaidenTooltipPrefix));
                return new[] { original, tooltipHook };
            });
            installs.Add("main.daily_recharge", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(corrupt, "DailyRecharge", Type.EmptyTypes);
                h.Patch(original, prefix: new HarmonyMethod(DailyRechargePrefix));
                return new[] { original };
            });
            // 0.7.0 Task 6:OnSemenUsed 复刻体(prefix)退役 → transpiler 单元。
            // 1.9.2 签名漂移 (GameItem)→(GameItem, GameItem)(第二参 = 精液瓶,
            // CorruptTentacle.cs:164)+ usedSemen HashSet 按瓶去重(:176)——
            // 双参重解析(ModReflect.Target 传 Type[] 双参);去重/帧清空语义
            // 归原方法体照跑 = 零复刻。装配形态 = TranspileCarrier.PatchTranspiler
            // 公共幂等挂载(corrupt_cost 先例同源);注入点 = TranspilePlan
            // on_semen_used 单元三点(semen_recharge 1 + max_charge 2),
            // 任一点未命中 → TranspileMatchException → HookInstaller 记 Dead
            // (宁拒不静默半改)。OnSemenUsed original 独占(无兄弟单元),
            // Carrier.Run 的 route 过滤对本单元恒 Own 放行(Router 无让位
            // 条目;remake SetupLiquidTargeting 经 InvokeOriginalStaticResult
            // 调本方法 = 调注入后方法体,共存语义不变)
            installs.Add("main.on_semen_used", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(corrupt, "OnSemenUsed",
                    new[] { typeof(GameItem), typeof(GameItem) });
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            installs.Add("main.is_corruptible", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(corrupt, "IsCorruptible", new[] { typeof(GameItem) });
                h.Patch(original, prefix: new HarmonyMethod(IsCorruptiblePrefix));
                return new[] { original };
            });
            // 0.7.0 Task 5:TryCorrupt 复刻体(prefix)退役 → transpiler 单元。
            // 装配形态 = TranspileCarrier.PatchTranspiler 公共幂等挂载(先 Unpatch
            // 自 id 的 transpiler 再 Patch——开关 off→on 重装/共享 original
            // 记账都安全,0.7.0 T7 起与各迁移单元同源);注入点 = TranspilePlan C 节
            // 六点(门槛 bge 1 + 扣减 sub/SetCharge 5),任一点未命中 →
            // TranspileMatchException → HookInstaller 记 Dead(宁拒不静默半改)。
            // TryCorrupt original 独占(无兄弟单元),Carrier.Run 的 route 过滤对本单元恒 Own 放行
            installs.Add("main.corrupt_cost", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(corrupt, "TryCorrupt", new[] { typeof(GameItem), typeof(GameItem) });
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            // 初始充能(更多可配置项 Task 9):Create postfix,主 mod 在场即有意义的
            // 兼容补丁(旧 Tolerant 档;按需装配下同样由开关驱动)
            installs.Add("main.initial_charge", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(corrupt, "Create", Type.EmptyTypes);
                h.Patch(original, postfix: new HarmonyMethod(InitialChargePostfix));
                return new[] { original };
            });
        }

        /// <summary>
        /// 幂等解析全部主 mod 成员(八条目共享;任一成员解析抛出 → 本方法原样上抛、
        /// 不提交任何字段,后续条目重试同样失败 → 八个单元一致不装,杜绝半解析状态
        /// 下装出运行时 NRE 的 prefix/transpiler——两阶段提交,同 Task 7)。失败由
        /// HookInstaller 逐单元隔离记 Dead,该组不装配。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type body = FindMainType("CorruptTentacle");
            System.Type itemFactory = FindMainType("ItemFactory");
            System.Type ctx = FindMainType("ModCtx");
            System.Type alarmClock = FindMainType("AlarmClock");
            System.Type tentacleTrap = FindMainType("TentacleTrap");
            System.Type tentaclePrinter = FindMainType("TentaclePrinter");
            System.Type tentacleApi = FindMainType("TentacleApi");
            System.Type tooltips = FindMainType("MaidenTooltip");
            var resolvedGet = ModReflect.StaticMethod<Func<GameItem, string, int>>(itemFactory, "GetInt");
            var resolvedSet = ModReflect.StaticMethod<Action<GameItem, string, int>>(itemFactory, "SetInt");
            var resolvedLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            var resolvedAlarmPlain = ModReflect.StaticMethod<Func<GameItem, bool>>(alarmClock, "IsPlainAlarm");
            var resolvedManholePlain = ModReflect.StaticMethod<Func<GameItem, bool>>(tentacleTrap, "IsPlainManhole");
            var resolvedPrinterPlain = ModReflect.StaticMethod<Func<GameItem, bool>>(tentaclePrinter, "IsPlainPrinter");
            // 0.7.0 T5:外部侵蚀规则查询(签名 = 1.9.2 反编译
            // static CorruptRule FindCorruptRule(GameItem)——返回内嵌类 CorruptRule,
            // 协变绑定为 Func<GameItem, object>,复刻体只判非 null)
            var resolvedFindRule = ModReflect.StaticMethod<Func<GameItem, object>>(tentacleApi, "FindCorruptRule");
            // 全部成功,一次性提交
            corrupt = body;
            ifGetInt = resolvedGet;
            ifSetInt = resolvedSet;
            mainLog = resolvedLog;
            mainLogWarning = resolvedLogWarning;
            isPlainAlarm = resolvedAlarmPlain;
            isPlainManhole = resolvedManholePlain;
            isPlainPrinter = resolvedPrinterPlain;
            findCorruptRule = resolvedFindRule;
            maidenTooltip = tooltips;
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
        // GetCharge / SetCharge:充能读写
        // ------------------------------------------------------------------

        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool GetChargePrefix(GameItem it, ref int __result)
        {
            int max;
            try
            {
                max = Cfg(CfgKeys.CorruptMaxCharge, 100);
            }
            catch (Exception ex)
            {
                LogMainWarning("侵蚀触手充能配置读取失败,放行原版: " + ex.Message);
                return true; // 纯查询零状态写入,原版重跑安全
            }
            try
            {
                __result = GetChargeBody(it, max);
            }
            catch (Exception ex)
            {
                LogMainWarning("侵蚀触手充能读取失败,放行原版: " + ex.Message);
                return true; // 纯查询:放行原版(原版无 catch,异常语义也随原版)
            }
            return false;
        }

        /// <summary>逐行复刻 v1.3.8 GetCharge;差异仅上限 corrupt.max_charge(默认 100 = 原版)。</summary>
        private static int GetChargeBody(GameItem it, int max)
        {
            if (it == null)
            {
                return 0;
            }
            return CorruptRules.ClampCharge(ifGetInt(it, "TENTACLE_CHARGE"), max);
        }

        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool SetChargePrefix(GameItem it, int v)
        {
            int max;
            try
            {
                max = Cfg(CfgKeys.CorruptMaxCharge, 100);
            }
            catch (Exception ex)
            {
                LogMainWarning("侵蚀触手充能配置读取失败,放行原版: " + ex.Message);
                return true; // 尚未写入任何状态,原版重跑安全
            }
            try
            {
                SetChargeBody(it, v, max);
            }
            catch (Exception ex)
            {
                // 写状态:SetInt 半写风险 > 重放风险,跳过原版只记日志。
                LogMainWarning("侵蚀触手充能写入失败: " + ex.Message);
            }
            return false;
        }

        /// <summary>逐行复刻 v1.3.8 SetCharge;差异仅钳制上限 corrupt.max_charge。</summary>
        private static void SetChargeBody(GameItem it, int v, int max)
        {
            if (it != null)
            {
                ifSetInt(it, "TENTACLE_CHARGE", CorruptRules.ClampCharge(v, max));
            }
        }

        /// <summary>
        /// 初始充能 postfix(更多可配置项 Task 9):Create() 原版尾部
        /// SetCharge(val, 100)(1.6.1 L35)——initial 与 max 均默认(100/100)时直接
        /// 返回(零漂移);否则对 __result 重设 Min(initial, max)(超上限由
        /// SetChargeBody → ClampCharge 天然钳制,spec §5.5 定案)。__result 为 null
        /// (理论不可达)或读取失败时跳过,不影响其余创建流程。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void InitialChargePostfix(ref GameItem __result)
        {
            GameItem it = __result;
            if (it == null) return;
            try
            {
                int initial = Cfg(CfgKeys.CorruptInitialCharge, 100);
                int max = Cfg(CfgKeys.CorruptMaxCharge, 100);
                if (initial == 100 && max == 100) return; // 双默认零漂移:原版 SetCharge(100) 已写
                SetChargeBody(it, Math.Min(initial, max), max);
            }
            catch (Exception ex)
            {
                LogMainWarning("侵蚀触手初始充能写入失败(保持原版 100): " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // RefreshTooltip:充能描述文案
        // ------------------------------------------------------------------

        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool RefreshTooltipPrefix(GameItem it)
        {
            int max, daily, semen;
            try
            {
                max = Cfg(CfgKeys.CorruptMaxCharge, 100);
                daily = Cfg(CfgKeys.CorruptDaily, 10);
                semen = Cfg(CfgKeys.CorruptSemen, 10);
            }
            catch (Exception ex)
            {
                LogMainWarning("侵蚀触手描述配置读取失败,放行原版: " + ex.Message);
                return true;
            }
            try
            {
                RefreshTooltipBody(it, max, daily, semen);
            }
            catch (Exception ex)
            {
                LogMainWarning("刷新触手描述失败(接管复刻): " + ex.Message);
            }
            return false;
        }

        /// <summary>逐行复刻 v1.3.8 RefreshTooltip;差异仅文案中 100/10/10 三个
        /// 拼接值改配置读取(默认值下串逐字相同)。R2-F2 核对:此即物品
        /// shortDescription(tooltip 尾部长描述区,GameItemElement L1144 渲染),
        /// 原版 Create() 不写 shortDescription——首次写入即 Create 尾部的
        /// RefreshTooltip 调用(decomp CorruptTentacle.cs L79→L119),本复刻体
        /// 已逐字对齐 L119 模板并按键读值,无需再改。</summary>
        private static void RefreshTooltipBody(GameItem it, int max, int daily, int semen)
        {
            if (it == null)
            {
                return;
            }
            try
            {
                int charge = GetChargeBody(it, max);
                string text = ((charge >= max) ? "\u3000（已充满）" : "");
                it.shortDescription = "具有侵蚀能力的触手，能够将物品侵蚀成为你身体的一部分。\n\n充能 " + charge + " / " + max + text + "\n· 每天自动恢复 " + daily + " 点\n· 把【触手精液】拖到它身上，恢复 " + semen + " 点\n· 充能满 " + max + " 时，把下面这些【拖到它身上】，\n\u3000可以将其侵蚀为一个【触手苗床箱】：\n\u3000工程铸造厂物资箱 / 后勤服务部物资箱 / 医疗舱物资箱 /\n\u3000治安部物证柜 / 治安部物资箱 / 储藏区 / 储藏区（扩建）\n· 带【重要物品】标签的东西（剧情道具之类）一律侵蚀不了";
            }
            catch (Exception ex)
            {
                LogMainWarning("刷新触手描述失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 分支级接管(R2-F2):主 mod MaidenTooltip.OnTooltip(经游戏
        /// ModHook.OnCreateTooltipLate 挂进 GameItemElement.GetTooltipBasic →
        /// ModInternal.CreateTooltipLate,渲染位置在 shortDescription 行之前 =
        /// tooltip 标题区)对 corrupt_tentacle 追加「充能 X / 100」行——分母与
        /// (已充满)阈值均为字面量 100(decomp MaidenTooltip.cs L53-61),不随
        /// corrupt.max_charge 刷新(0x03 实测:长描述已 100/200 而标题区仍
        /// 150/100)。prefix 仅对 corrupt_tentacle 物品接管该分支并逐行复刻两行
        /// (charge 经 GetChargeBody 读同一 max 键;分母与阈值换 max;【触手】
        /// 行 as-is,颜色 LightGreen/LightCyan/LightPurple 对齐原版调色值
        /// 8035658/4367018/11567781),其余物品(本体/苗床箱/少女)一律放行
        /// 原方法零漂移——分支次序对齐原版:本体 tag 优先、corrupt_tentacle
        /// 精确 id 其次。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool MaidenTooltipPrefix(RichTextBuilder builder, GameItem item)
        {
            if (builder == null || item == null) return true;        // 原方法自会直接返回
            if (item.IsTag("TENTACLE_BODY_TAG")) return true;        // 本体分支优先,放行原方法
            if (item.identifier != "corrupt_tentacle") return true;  // 其余物品放行(零漂移)
            int max;
            try
            {
                max = Cfg(CfgKeys.CorruptMaxCharge, 100);
            }
            catch (Exception ex)
            {
                LogMainWarning("侵蚀触手标题行配置读取失败,放行原版: " + ex.Message);
                return true; // 尚未写入任何 tooltip 状态,原版重跑安全
            }
            try
            {
                int charge = GetChargeBody(item, max);
                builder.AddEmptyLine(2);
                builder.AddLine("充能 " + charge + " / " + max + ((charge >= max) ? "\u3000（已充满）" : ""), false,
                    (charge >= max) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightCyan,
                    false, false, false, false,
                    RenderHandler.ColorPalette.None, RenderHandler.ColorPalette.None, RenderHandler.ColorPalette.None);
                builder.AddLine("【触手】", false, RenderHandler.ColorPalette.LightPurple,
                    false, false, false, false,
                    RenderHandler.ColorPalette.None, RenderHandler.ColorPalette.None, RenderHandler.ColorPalette.None);
            }
            catch (Exception ex)
            {
                // 原版分支同文案(decomp MaidenTooltip.cs L63-66)
                LogMainWarning("触手 tooltip 失败: " + ex.Message);
            }
            return false;
        }

        // ------------------------------------------------------------------
        // DailyRecharge:每日自动充能
        // ------------------------------------------------------------------

        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool DailyRechargePrefix()
        {
            int max, daily, semen;
            try
            {
                max = Cfg(CfgKeys.CorruptMaxCharge, 100);
                daily = Cfg(CfgKeys.CorruptDaily, 10);
                semen = Cfg(CfgKeys.CorruptSemen, 10); // RefreshTooltip 文案需要
            }
            catch (Exception ex)
            {
                LogMainWarning("每日充能配置读取失败,放行原版: " + ex.Message);
                return true;
            }
            try
            {
                DailyRechargeBody(max, daily, semen);
            }
            catch (Exception ex)
            {
                // 循环体自带逐实例 try/catch(原版同);此处兜底防 Instances 半扫重放。
                LogMainWarning("侵蚀性触手每日充能失败: " + ex.Message);
            }
            return false;
        }

        /// <summary>逐行复刻 v1.3.8 DailyRecharge;差异仅上限/每日量两配置
        /// (默认 100/10 = 原版)。Instances 为原版静态字段(经 ModReflect 活引用)。</summary>
        private static void DailyRechargeBody(int max, int daily, int semen)
        {
            List<GameItem> instances = ModReflect.GetStatic<List<GameItem>>(corrupt, "Instances");
            instances.RemoveAll((GameItem i) => i == null);
            if (instances.Count == 0)
            {
                return;
            }
            int num = 0;
            GameItem[] array = instances.ToArray();
            foreach (GameItem val in array)
            {
                if (val == null)
                {
                    continue;
                }
                try
                {
                    int charge = GetChargeBody(val, max);
                    if (charge < max)
                    {
                        SetChargeBody(val, Math.Min(max, charge + daily), max);
                        RefreshTooltipBody(val, max, daily, semen);
                        num++;
                    }
                }
                catch (Exception ex)
                {
                    LogMainWarning("触手充能失败: " + ex.Message);
                }
            }
            if (num > 0)
            {
                LogMain("侵蚀性触手每日充能: " + num + " 根 (+" + daily + ")");
            }
        }

        // ------------------------------------------------------------------
        // OnSemenUsed:精液充能(brief 的 SemenRecharge)
        // 0.7.0 Task 6:prefix 复刻体(OnSemenUsedPrefix/OnSemenUsedBody)与
        // api 侧帧防重镜像(semenFrame/semenTentacle)退役。1.9.2 相对 1.6.1
        // 两处漂移——①签名 (GameItem)→(GameItem, GameItem)(第二参 = 精液瓶,
        // ItemFactory.cs:273 拖放链传入);②防重改 usedSemen HashSet 按瓶去重
        // (同帧跨瓶可多次吸取,同瓶整帧一次,CorruptTentacle.cs:176)——
        // 逐行复刻的漂移面失控,semen_recharge 的 Min(10,…) 首参与 max_charge
        // 两消费点改经 TranspileCarrier 注入(见 Register 的 main.on_semen_used
        // 委托),其余流程(帧清空/按瓶去重/已饱 toast/吸取 toast/日志)留在
        // 原方法体照跑 = 零漂移。SetCharge/RefreshTooltip 调用由各自单元
        // (set_charge/refresh_tooltip prefix)接管,充能落库钳制不缺位。
        // ------------------------------------------------------------------

        // ------------------------------------------------------------------
        // IsCorruptible:白/黑名单判定
        // ------------------------------------------------------------------

        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool IsCorruptiblePrefix(GameItem it, ref bool __result)
        {
            string whitelistExtra, blacklistExtra;
            try
            {
                whitelistExtra = Cfg(CfgKeys.CorruptWhitelist, "");
                blacklistExtra = Cfg(CfgKeys.CorruptBlacklist, "");
            }
            catch (Exception ex)
            {
                LogMainWarning("侵蚀名单配置读取失败,放行原版: " + ex.Message);
                return true; // 纯查询,原版重跑安全
            }
            try
            {
                __result = IsCorruptibleBody(it, whitelistExtra, blacklistExtra);
            }
            catch (Exception ex)
            {
                LogMainWarning("侵蚀判定失败,放行原版: " + ex.Message);
                return true; // 纯查询(仅写拒绝日志缓存):放行原版
            }
            return false;
        }

        /// <summary>逐行复刻 1.6.1 IsCorruptible;差异仅黑/白名单关键词换
        /// CorruptRules.Blacklisted/Whitelisted(原版关键词全集为 csvBase 常量,
        /// *_extra CSV 追加;原版先查黑名单后查白名单的次序保留)。四个 id 排除、
        /// 三个 tag 排除、RejectLogged 拒绝日志(含 IMPORTANT_TAG/CONTAINER_TAG/
        /// STANDARD_MACHINE_TAG 标注——1.6.1 中 IMPORTANT_TAG 仍只进日志、不作
        /// 排除判定,as-is 复刻)逐行保留。1.6.1 在 lou_locker 黑名单之后、关键词
        /// 白名单之前增闹钟/井盖(L274-281)、1.7.1 增打印机(L282-285)、
        /// 1.9.2 增外部侵蚀规则透传(打印机之后,本任务 T5 补齐)四道精确
        /// 判定——均委托直判返回可侵蚀,不进关键词 CSV。</summary>
        private static bool IsCorruptibleBody(GameItem it, string whitelistExtra, string blacklistExtra)
        {
            if (it == null)
            {
                return false;
            }
            try
            {
                if (!GeneralHelper.IsItemOwned(it))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            string text = ((it.identifier == null) ? "" : it.identifier);
            switch (text)
            {
            case "tentacle_box":
                return false;
            case "corrupt_tentacle":
                return false;
            case "tentacle_body":
                return false;
            case "tentacle_manual":
                return false;
            default:
            {
                if (it.IsTag("TENTACLE_CORRUPTION"))
                {
                    return false;
                }
                if (it.IsTag("MAIDEN_FEMALE"))
                {
                    return false;
                }
                if (it.IsTag("TENTACLE"))
                {
                    return false;
                }
                string text3 = ((it.name == null) ? "" : it.name);
                if (CorruptRules.Blacklisted(text, text3, BlacklistBase, blacklistExtra))
                {
                    return false;
                }
                if (isPlainAlarm(it))
                {
                    return true;
                }
                if (isPlainManhole(it))
                {
                    return true;
                }
                // 1.7.1 L282-285:井盖之后、crate 关键词白名单之前增打印机精确
                // id 白名单(TentaclePrinter.IsPlainPrinter 委托直判——
                // "tentacle_printer" 自身排除、PRINTER_TAG 命中可侵蚀)
                if (isPlainPrinter(it))
                {
                    return true;
                }
                // 0.7.0 T5(1.9.2 基线,192 dump IL_0105):打印机之后、关键词白名单
                // 之前增外部侵蚀规则透传(TentacleApi.FindCorruptRule(it) != null
                // → true)——复刻体若照抄 1.7.1 体,接管期外部 mod 经
                // RegisterCorruptible 注册的侵蚀规则会整体失效;次序事实:黑名单
                // 判定在本行之前(黑名单追加项压过外部规则)。委托缺席(协变绑定
                // 失败的防御)时 ExternalRuleHits 恒 false,退化为 1.7.1 行为
                if (CorruptRules.ExternalRuleHits(findCorruptRule, it))
                {
                    return true;
                }
                if (CorruptRules.Whitelisted(text, text3, WhitelistBase, whitelistExtra))
                {
                    return true;
                }
                HashSet<string> rejectLogged = ModReflect.GetStatic<HashSet<string>>(corrupt, "RejectLogged");
                if (!rejectLogged.Contains(text))
                {
                    rejectLogged.Add(text);
                    string text4 = "";
                    try
                    {
                        if (it.IsTag("IMPORTANT_TAG"))
                        {
                            text4 += "[重要物品]";
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (it.IsTag("CONTAINER_TAG"))
                        {
                            text4 += "[容器]";
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        if (it.IsTag("STANDARD_MACHINE_TAG"))
                        {
                            text4 += "[机器]";
                        }
                    }
                    catch
                    {
                    }
                    LogMain("侵蚀拒绝: " + text + " / " + text3 + " " + text4);
                }
                return false;
            }
            }
        }

        // ------------------------------------------------------------------
        // TryCorrupt:满额侵蚀(brief 的 CorruptCost)
        // 0.7.0 Task 5:prefix 复刻体(TryCorruptPrefix/TryCorruptBody)退役。
        // 1.9.2 的 TryCorrupt 相对 1.7.1 已三处漂移——①头插 TentacleApi.
        // FindCorruptRule 外部规则分支(CorruptTentacle.cs:373-:414,产物经
        // ItemDelivery.TryDeliver 交付,失败不扣能);②alarm/printer/manhole
        // 三分流扣能次序翻转(先 TryConvert 成功才扣);③苗床箱主分支日志
        // 文案改「剩余淫力」——逐行复刻的漂移面失控,corrupt_cost 六常量改经
        // TranspileCarrier 注入(见 Register 的 main.corrupt_cost 委托),
        // 其余流程留在原方法体照跑 = 零漂移。名单判定经原方法内联的
        // IsCorruptible 调用走本类 is_corruptible 单元前缀。
        // ------------------------------------------------------------------

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
            else UnityEngine.Debug.Log("[lewd_meme_config→主mod] " + msg);
        }

        private static void LogMainWarning(object msg)
        {
            if (mainLogWarning != null) mainLogWarning(msg);
            else UnityEngine.Debug.LogWarning("[lewd_meme_config→主mod] " + msg);
        }
    }
}
