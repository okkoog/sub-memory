using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 落穴陷阱接管(0.7.0 Task 7:Create 系复刻体退役;目录单元
    /// main.create_manhole / main.create_trap / main.trap_nightly_once /
    /// main.trap_sell_scav / main.trap_sell_thief + pools.trap_tactic 数据
    /// 写入(DataWrite 载体,不挂补丁);基线主 mod 1.9.2):
    ///  <list type="bullet">
    ///   <item>main.create_manhole — CreateManhole postfix(0.7.0 T7):单价
    ///     trap.price(原版 100)经 <see cref="ValuePostfixPatches.TrapPricePostfix"/>
    ///     改 __result.unitValue;名称/描述/flavor/贴图/WEAPON+MELEE_WEAPON、
    ///     1.9.2 新增的 ApplyCsv 尾调全部归原方法照跑(零复刻)。</item>
    ///   <item>main.create_trap — CreateTrap postfix(单价 trap.trap_price,
    ///     原版 200,方法体 = ValuePostfixPatches)+ TentacleTrap.BuildWindow
    ///     transpiler(trap.brood_size/output_size,原版 12×12)双接管:
    ///     注入点 = TranspilePlan F 节四点(两组 GameGridInventory(12,12)
    ///     双参 ldc,After 尾 stloc 槽位 1/2 区分);1.9.2 的 tags/onCycleEnd/
    ///     onLoaded/TRAP_FACTION=0/TRAP_TACTIC=1/TRAP_CATCH=0/Register + Slots
    ///     登记、RefreshDescription 与 TRAP_ENABLED 写入、**部署面板三按钮**
    ///     (势力/策略/电源——1.9.2 新增 powerBtn,0.6.0 复刻体把它整个吃掉,
    ///     退役后原方法跑,按钮 UI 自动恢复在场)全部归原方法照跑。</item>
    ///   <item>main.trap_nightly_once — NightlyTickOnce transpiler
    ///     (TranspilePlan F 节两点:① trap.tactic_chances 的
    ///     [ldsfld TACTIC_CHANCES; ldloc 档位; ldelem.r4] 整序列替换 →
    ///     [原 ldloc; call GetTacticChance](CSV 现读,即时生效);
    ///     ② trap.semen_chance 的 ldc.r4 0.5)。捕捉掷骰/CreateMaidenOfFaction/
    ///     PlaceInto/TRAP_CATCH/TRAP_LAST_NAME 写入、AddOral/AddSex/AddAnal
    ///     (固定 1,原版硬编码无键)、TRAP_REPORT 文案与 RefreshDescription
    ///     全部归原方法照跑。让位:remake 在场 Yield(Router R1,同 alarm)。</item>
    ///   <item>main.trap_sell_scav / main.trap_sell_thief — 主 mod 自有 Harmony
    ///     补丁类 Postfix 方法的 transpiler(PatchScavGeneralAlarm/Patch-
    ///     ThiefHouseholdAlarm,合法目标;ldc.r4 0.1/0.05 → getter 现读
    ///     trap.npc_sell_chance_scav/thief)。0.6.0 的 prefix 复刻体(防
    ///     0.1f 写死值双跑)随退役删除——原 Postfix 体照跑两调用
    ///     (AlarmClock.TrySellToNpc + TentacleTrap.TrySellToNpc),闹钟侧经
    ///     alarm_try_sell_npc 的注入读配置,井盖侧读本键注入值,双源各自
    ///     生效无竞争。</item>
    ///   <item>pools.trap_tactic(DataWrite,不挂补丁,0.6.0 形态保留)——
    ///     装配时 + HookRuntime 分发器转发(key==trap.tactic_chances)时,把
    ///     ParseTacticChances 结果就地覆写进主 mod TentacleTrap.TACTIC_CHANCES
    ///     (public static readonly float[3]):T7 后掷骰点已换 CSV 现读
    ///     (transpiler),数组消费方 = BuildWindow 策略按钮 caption 与
    ///     TacticCaption 文案——写入使按钮显示与掷骰值同源一致(单一
    ///     CSV 事实源);null(无效 CSV)→ 跳过保持现值;新值==现值 → 跳过
    ///     (默认安装零写入,红线)。</item>
    ///  </list>
    /// 全默认(无 override)时与原版逐项一致 = 验收红线(getter 注册表缺席/
    /// 读失败回退 1.9.2 原常量);默认全关 = 五单元 Absent = 零 hook。
    /// 0.6.0 的 prefix 复刻体(CreateManholeBody/CreateTrapBody/BuildWindowBody/
    /// FactionCaption/TacticCaption/NightlyOnceBody/SellScav/SellThief)与
    /// 24 个主 mod 委托缓存随 T7 退役;主 mod 类型(TentacleTrap/
    /// PatchScavGeneralAlarm/PatchThiefHouseholdAlarm/ModCtx)运行时经
    /// ModReflect 解析,解析只发生在装配委托内。装配经
    /// TranspileCarrier.PatchTranspiler 幂等挂载;失败由 HookInstaller
    /// 逐单元隔离记 Dead,不阻断其余补丁。
    /// </summary>
    public static class TrapPatch
    {
        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充) --
        private static bool resolved;                                          // 全部成员就位后才置真
        private static System.Type tentacleTrap;                             // TentacleTrap(补丁目标 + Slots/静态数组)
        private static System.Type patchScavGeneral;                          // PatchScavGeneralAlarm(sell_scav 补丁目标)
        private static System.Type patchThiefHousehold;                       // PatchThiefHouseholdAlarm(sell_thief 补丁目标)
        private static Action<object> mainLog;                                // ModCtx.Log
        private static Action<object> mainLogWarning;                         // ModCtx.LogWarning

        /// <summary>把五条目的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账——create_trap
        /// 返回双 original,HookInstaller 方法级引用计数天然支持)。
        /// pools.trap_tactic 是 DataWrite 载体,不进映射也不进 HookInstaller
        /// (R7):装配期一次写入走 WriteTacticChancesAtCompose,Changed 重写经
        /// HookRuntime 分发器转发 OnConfigChanged。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            // 价格 postfix(方法体集中 ValuePostfixPatches;TRAP_ENABLED 写入归原方法)
            installs.Add("main.create_manhole", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(tentacleTrap, "CreateManhole", Type.EmptyTypes);
                h.Patch(original, postfix: new HarmonyMethod(ValuePostfixPatches.TrapPricePostfix));
                return new[] { original };
            });
            // 价格 postfix + BuildWindow transpiler 双接管(alarm 先例;电源按钮
            // 随原方法自动在场)
            installs.Add("main.create_trap", h =>
            {
                EnsureResolved();
                MethodBase create = ModReflect.Target(tentacleTrap, "CreateTrap", Type.EmptyTypes);
                MethodBase build = ModReflect.Target(tentacleTrap, "BuildWindow",
                    new[] { typeof(GameItem), typeof(List<GameGridInventory>).MakeByRefType() });
                h.Patch(create, postfix: new HarmonyMethod(ValuePostfixPatches.TrapTrapPricePostfix));
                TranspileCarrier.PatchTranspiler(h, build);
                return new[] { create, build };
            });
            installs.Add("main.trap_nightly_once", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(tentacleTrap, "NightlyTickOnce",
                    new[] { typeof(GameItem) });
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            installs.Add("main.trap_sell_scav", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(patchScavGeneral, "Postfix", Type.EmptyTypes);
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            installs.Add("main.trap_sell_thief", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(patchThiefHousehold, "Postfix", Type.EmptyTypes);
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
        }

        /// <summary>
        /// 幂等解析主 mod 类型/成员(五个 hook 条目共享;解析失败 → 原样上抛
        /// 不提交,条目一致不装——两阶段提交)。0.7.0 T7 起复刻体的 24 个方法
        /// 委托缓存下线,剩四个类型 + 日志双委托(DataWrite 的数组写入消费)。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type trap = FindMainType("TentacleTrap");
            System.Type scavPatch = FindMainType("PatchScavGeneralAlarm");
            System.Type thiefPatch = FindMainType("PatchThiefHouseholdAlarm");
            System.Type ctx = FindMainType("ModCtx");
            var resolvedLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            // 全部成功,一次性提交
            tentacleTrap = trap;
            patchScavGeneral = scavPatch;
            patchThiefHousehold = thiefPatch;
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
        // TACTIC_CHANCES 就地覆写(DataWrite 载体:装配期一次 + Changed 分发重写)
        // ------------------------------------------------------------------

        /// <summary>
        /// pools.trap_tactic 装配期一次写入(HookRuntime 主 mod 在场且版本受支持时
        /// 调用):默认值与原版一致 → 零写入;后续重写经 HookRuntime 分发器转发
        /// OnConfigChanged。
        /// </summary>
        internal static void WriteTacticChancesAtCompose()
        {
            EnsureResolved();
            WriteTacticChances();   // 装配时同步一次(默认值与原版一致 → 零写入)
        }

        /// <summary>
        /// 把 trap.tactic_chances(经 ParseTacticChances)就地覆写进主 mod
        /// TentacleTrap.TACTIC_CHANCES(public static readonly float[3]):引用只读但
        /// 数组内容可变,GetStatic 取活引用等长覆写。T7 后掷骰消费点已迁
        /// transpiler 的 CSV 现读,数组消费方 = BuildWindow 策略按钮 caption/
        /// TacticCaption 文案——写入使命令面板显示与掷骰值同源(单一 CSV)。
        /// 守卫链:解析 null → 跳过保持现值(无效 CSV 契约);现数组长度≠3 →
        /// 记日志跳过;新值==现值 → 跳过(默认安装零写入,红线)。
        /// </summary>
        private static void WriteTacticChances()
        {
            float[] target = TrapRules.ParseTacticChances(Cfg(CfgKeys.TrapTacticChances, "0.1,0.2,0.3"));
            if (target == null)
            {
                LogMainWarning("策略捕捉率 CSV 无效,跳过写入保持原版: " + CfgKeys.TrapTacticChances);
                return;
            }
            float[] current = ModReflect.GetStatic<float[]>(tentacleTrap, "TACTIC_CHANCES");
            if (current == null || current.Length != 3)
            {
                LogMainWarning("主 mod TACTIC_CHANCES 数组异常(现 "
                    + (current == null ? "null" : current.Length.ToString()) + " 项,应 3 项),跳过写入保持原版");
                return;
            }
            if (Same(current, target))
            {
                LogMain("策略捕捉率与现值一致,跳过写入: TACTIC_CHANCES");
                return;
            }
            Array.Copy(target, current, 3);
            LogMain("策略捕捉率已就地覆写: " + target[0].ToString("P0") + "/"
                + target[1].ToString("P0") + "/" + target[2].ToString("P0"));
        }

        // ------------------------------------------------------------------
        // Changed 重写处理器(HookRuntime 分发器转发;R10:订阅/退订归分发器,
        // 本类不再自持 registry 引用)
        // ------------------------------------------------------------------

        /// <summary>仅 trap.tactic_chances 变更 → 重写 TACTIC_CHANCES(异常吞掉记日志)。</summary>
        internal static void OnConfigChanged(string key)
        {
            if (key != CfgKeys.TrapTacticChances) return;
            try
            {
                WriteTacticChances();
            }
            catch (Exception ex)
            {
                LogMainWarning("策略捕捉率重写失败(" + key + "): " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // 共用小件
        // ------------------------------------------------------------------

        private static bool Same(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

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
