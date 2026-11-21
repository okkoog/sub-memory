using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 闹钟接管(0.7.0 Task 7:Create 系复刻体退役;目录单元 main.create_alarm /
    /// main.create_tentacle_alarm / main.alarm_nightly_once / main.alarm_try_sell_npc,
    /// 开关开且版本受支持时装配;基线主 mod 1.9.2):
    ///  <list type="bullet">
    ///   <item>main.create_alarm — CreateAlarm postfix(0.7.0 T7):单价
    ///     alarm.price(原版 75)经 <see cref="ValuePostfixPatches.AlarmPricePostfix"/>
    ///     改 __result.unitValue;名称/描述/flavor/贴图/形状/HOUSEHOLD_GOOD、
    ///     1.9.2 新增的 ApplyCsv 尾调全部归原方法照跑(零复刻)。</item>
    ///   <item>main.create_tentacle_alarm — CreateTentacleAlarm postfix(单价
    ///     alarm.tentacle_price,原版 150,方法体 = ValuePostfixPatches)+
    ///     AlarmClock.BuildWindow transpiler(alarm.brood_size/output_size,
    ///     原版 4×4)双接管:BuildWindow 注入点 = TranspilePlan F 节四点
    ///     (两组 GameGridInventory(4,4) 双参 ldc,After 尾 stloc 槽位 0/1
    ///     区分);1.9.2 的 tags/onCycleEnd/onLoaded/Alarms/Slots 登记与
    ///     双区过滤规则、面板标题(「苗床区 4×4」ldstr 字面量——仅入文本
    ///     不收,尺寸改值时标题滞后是既定取舍)全部原样。</item>
    ///   <item>main.alarm_nightly_once — NightlyTickOnce transpiler
    ///     (TranspilePlan F 节三点:DeflowerTracker.AddOral/AddSex/AddAnal
    ///     第二参 ldc.i4.1 → getter 现读 oral/sex/anal_per_night);苗床产精液
    ///     段(1.9.2 产出无条件,每苗床一瓶)与「触手闹钟：唤醒了 X」日志、
    ///     UpdateMaidenDescription 全部归原方法照跑。alarm.semen_output 是
    ///     T2 F 节放弃键(无数值 ldc)→ HookCatalog.DataOnlyKeys,面板行保留。
    ///     让位:remake 在场 Yield(Router R1——其 NightlyTickOnce transpiler
    ///     已删消费点区段,本 mod 注入点必不命中,不挂 = 不炸)。</item>
    ///   <item>main.alarm_try_sell_npc — TrySellToNpc transpiler(掷骰阈值
    ///     ldc.r4 0.05 → getter 现读 alarm.npc_sell_chance);三条调用路径
    ///     (NIGHTLY/Scav/Household posthooks)共用此方法,一处注入全路径生效;
    ///     命中日志「NPC 货架多了一个闹钟（5% 命中）」的 "5%" 是 ldstr 字面量
    ///     ——0.6.0 复刻体的动态百分比插入随退役回归原版文案(仅入文本不收)。</item>
    ///  </list>
    /// 全默认(无 override)时与原版逐项一致 = 验收红线(getter 注册表缺席/读
    /// 失败回退 1.9.2 原常量);默认全关 = 四单元 Absent = 零 hook。
    /// 0.6.0 的 prefix 复刻体(CreateAlarmBody/CreateTentacleAlarmBody/
    /// BuildWindowBody/NightlyOnceBody/TrySellToNpcBody)与 16 个主 mod 委托
    /// 缓存随 T7 退役——数值经 TranspileCarrier 注入点在原方法体内求值,
    /// 签名漂移面从「逐行复刻」缩到「锚点窗口」,任一注入点未命中 →
    /// TranspileMatchException → HookInstaller 记 Dead(宁拒不静默半改)。
    /// 主 mod 类型(AlarmClock)运行时经 ModReflect 解析;解析只发生在装配
    /// 委托内(装配 = 物品目录初始化后,本 mod 已被加载器登记,ModReflect
    /// 合法),绝不进静态构造。装配经 TranspileCarrier.PatchTranspiler 幂等
    /// 挂载(先 Unpatch 自 id 再 Patch);失败由 HookInstaller 逐单元隔离记
    /// Dead,不阻断其余补丁。
    /// </summary>
    public static class AlarmPatch
    {
        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充) --
        private static bool resolved;                                          // 全部成员就位后才置真
        private static System.Type alarmClock;                               // AlarmClock(四个补丁目标的 declaring type)

        /// <summary>把四条目的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账——
        /// create_tentacle_alarm 返回双 original,HookInstaller 方法级引用
        /// 计数天然支持)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            // 价格 postfix(Create* 尾 ApplyCsv 之后恒胜);方法体集中 ValuePostfixPatches
            installs.Add("main.create_alarm", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(alarmClock, "CreateAlarm", Type.EmptyTypes);
                h.Patch(original, postfix: new HarmonyMethod(ValuePostfixPatches.AlarmPricePostfix));
                return new[] { original };
            });
            // 价格 postfix + BuildWindow transpiler 双接管(brood/output 两布局
            // 键不上面板,搭 tentacle_price 开关的车——0.6.0 语义)
            installs.Add("main.create_tentacle_alarm", h =>
            {
                EnsureResolved();
                MethodBase create = ModReflect.Target(alarmClock, "CreateTentacleAlarm", Type.EmptyTypes);
                MethodBase build = ModReflect.Target(alarmClock, "BuildWindow",
                    new[] { typeof(List<GameGridInventory>).MakeByRefType() });
                h.Patch(create, postfix: new HarmonyMethod(ValuePostfixPatches.AlarmTentaclePricePostfix));
                TranspileCarrier.PatchTranspiler(h, build);
                return new[] { create, build };
            });
            installs.Add("main.alarm_nightly_once", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(alarmClock, "NightlyTickOnce",
                    new[] { typeof(GameItem) });
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            installs.Add("main.alarm_try_sell_npc", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(alarmClock, "TrySellToNpc", Type.EmptyTypes);
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
        }

        /// <summary>
        /// 幂等解析主 mod 类型(四个条目共享;解析失败 → 原样上抛不提交,
        /// 条目不装——两阶段提交)。0.7.0 T7 起只剩 AlarmClock 一个类型
        /// (复刻体退役,16 个方法委托缓存随之下线)。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            alarmClock = FindMainType("AlarmClock");
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
    }
}
