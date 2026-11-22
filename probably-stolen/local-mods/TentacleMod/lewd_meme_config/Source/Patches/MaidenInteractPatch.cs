using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 少女互动阈值 + 初始处女接管(0.7.0 Task 9,两个目录单元
    /// main.interact_threshold / main.initial_virgin,恒 Own 无让位——
    /// 家族 mod 零触碰这两个数值段):
    ///  <list type="bullet">
    ///   <item>main.interact_threshold — prefix 复刻主 mod
    ///     <c>ItemFactory.ComputeInteractMax(GameItem)</c>(public static,
    ///     1.9.2 :582-606):null → 1;否则从 1 起数,情感(MAIDEN_AFFECTION)/
    ///     信任(MAIDEN_TRUST)/催眠(MAIDEN_HYPNOSIS)/调教(TRAIN_VALUE)四项
    ///     各自 ≥60 各 +1。四阈值同源单键 maiden.interact_threshold(默认 60),
    ///     默认放行原方法零漂移;偏离时 GetInt 读值改委托直调(与 R10 postfix
    ///     同一 ModReflect 委托先例),逐行复刻其余流程。</item>
    ///   <item>main.initial_virgin — postfix on 主 mod
    ///     <c>ItemFactory.CreateMaidenOfFaction(string)</c>(1.9.2 :119-120
    ///     无条件 Enable VIRGIN/ANAL_VIRGIN 双 tag,0.6 掷骰段已删):面板
    ///     bool 键 maiden.initial_virgin(默认 true = 原版行为)为 false 时,
    ///     对产物 <c>GameItem.DisableTag</c>("MAIDEN_VIRGIN"/"MAIDEN_ANAL_
    ///     VIRGIN",游戏 public API 直接编译引用——BrothelPatches 先例,
    ///     skipValidate=false 与主 mod Enable 的 ModifyTag 包装逐字对齐)。
    ///     开关形态 = Toggle 自开关**反向分派**(SwitchDispatch T9 条:默认
    ///     true → IsOn false 不装配,「默认零 patch」红线成立;false → 装配)。
    ///     共存:与池段复刻体(main.create_maiden 两 prefix,R4 让位)同方法
    ///     ——prefix 返回 false 跳过原方法时 Harmony postfix 仍跑,Disable
    ///     对复刻体产物同样生效;与 R10 attrs postfix 三 postfix 叠加,先后
    ///     顺序无冲突语义(操作不相交的 tag);含 CreateMaiden() 无参版的
    ///     内部调用(IL call 指令指向同一方法,patched 版本对所有调用方生效,
    ///     R10 同理)。UpdateMaidenDescription(1.9.2)不消费处女 tag
    ///     (描述只读名字/阵营/调教值),Disable 后无需刷描述。</item>
    ///  </list>
    /// 主 mod 类型(ItemFactory/ModCtx)一律运行时经 ModReflect 解析成委托,
    /// 不编译引用 MidnightPawn.*;解析只发生在装配委托内(装配 = 物品目录
    /// 初始化后,本 mod 已被加载器登记,ModReflect 合法),两阶段提交(任一
    /// 成员解析失败 → 原样上抛不提交,单元记 Dead,开关 off→on 可重试)。
    /// postfix 修正体异常 → 吞掉记日志保留原产物(半修改优于毁产物,
    /// ValuePostfixPatches 先例)。
    /// </summary>
    public static class MaidenInteractPatch
    {
        // 主 mod 原文字面量(lewd_meme_192 ItemFactory.ComputeInteractMax :589-601):
        // 四阈值全部 ≥ 60
        internal const int ThresholdDefault = 60;

        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充,补丁只做直接调用)--
        private static bool resolved;                                          // 全部成员就位后才置真
        private static System.Type itemFactory;                              // ItemFactory(补丁目标)
        private static Func<GameItem, string, int> ifGetInt;                  // ItemFactory.GetInt(:387,四阈值读值)
        private static Action<object> mainLog;                                // ModCtx.Log
        private static Action<object> mainLogWarning;                         // ModCtx.LogWarning

        /// <summary>把两条单元的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。两单元独立登记、
        /// 独立装配(键互不相关,开关各自拨)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("main.interact_threshold", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(itemFactory, "ComputeInteractMax",
                    new[] { typeof(GameItem) });
                h.Patch(original, prefix: new HarmonyMethod(ComputeInteractMaxPrefix));
                return new[] { original };
            });
            installs.Add("main.initial_virgin", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(itemFactory, "CreateMaidenOfFaction",
                    new[] { typeof(string) });
                h.Patch(original, postfix: new HarmonyMethod(CreateMaidenOfFactionPostfix));
                return new[] { original };
            });
        }

        /// <summary>
        /// 幂等解析全部主 mod 成员(两单元共享;任一成员解析抛出 → 本方法原样上抛、
        /// 不提交任何字段,两阶段提交,同 MaidenAttrIphPatch)。失败由 HookInstaller
        /// 逐单元隔离记 Dead。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type factory = FindMainType("ItemFactory");
            System.Type ctx = FindMainType("ModCtx");
            var resolvedGetInt = ModReflect.StaticMethod<Func<GameItem, string, int>>(factory, "GetInt");
            var resolvedLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            // 全部成功,一次性提交
            itemFactory = factory;
            ifGetInt = resolvedGetInt;
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
        // ComputeInteractMax:互动次数上限
        // ------------------------------------------------------------------

        /// <summary>
        /// 逐行复刻 v1.9.2 ComputeInteractMax(decomp :582-606);差异仅四阈值
        /// 60 → maiden.interact_threshold(默认 60 放行原方法 = 零漂移)。
        /// 读配置失败 → 放行原方法(纯函数无状态,安全)。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool ComputeInteractMaxPrefix(GameItem maid, ref int __result)
        {
            int threshold;
            try
            {
                threshold = PatchCtx.Config?.Current<int>(CfgKeys.InteractThreshold) ?? ThresholdDefault;
            }
            catch (Exception)
            {
                return true; // 配置读取失败:放行原版阈值
            }
            if (threshold == ThresholdDefault) return true; // 默认零漂移:原方法原样运行
            if (maid == null)
            {
                __result = 1; // 原版 null 短路(:585-588)
                return false;
            }
            int num = 1;
            if (ifGetInt(maid, "MAIDEN_AFFECTION") >= threshold) num++;
            if (ifGetInt(maid, "MAIDEN_TRUST") >= threshold) num++;
            if (ifGetInt(maid, "MAIDEN_HYPNOSIS") >= threshold) num++;
            if (ifGetInt(maid, "TRAIN_VALUE") >= threshold) num++;
            __result = num;
            return false; // 复刻体已定结果,跳过原方法
        }

        // ------------------------------------------------------------------
        // CreateMaidenOfFaction:初始处女(1.9.2 无条件 Enable 的反向出口)
        // ------------------------------------------------------------------

        /// <summary>
        /// postfix:面板值 true(默认,= 1.9.2 原版无条件处女)→ 零参与;false →
        /// Disable 双 tag。原方法失败(产物 null)零参与;读配置失败保持原产物。
        /// DisableTag(skipValidate=false)与主 mod Enable 的 ModifyTag 包装逐字
        /// 对称(:33-38)。
        /// </summary>
        [HarmonyPostfix]
        private static void CreateMaidenOfFactionPostfix(GameItem __result)
        {
            if (__result == null) return; // 原方法失败(半途异常产物为 null),零参与
            bool initialVirgin;
            try
            {
                initialVirgin = PatchCtx.Config?.Current<bool>(CfgKeys.InitialVirgin) ?? true;
            }
            catch (Exception ex)
            {
                LogMainWarning("初始处女配置读取失败,保持原产物: " + ex.Message);
                return;
            }
            if (initialVirgin) return; // 默认 = 原版行为,零参与(精确零漂移)
            try
            {
                __result.DisableTag("MAIDEN_VIRGIN", false);
                __result.DisableTag("MAIDEN_ANAL_VIRGIN", false);
                LogMain("少女生成:初始处女已关闭(处女/肛处女 tag 移除)");
            }
            catch (Exception ex)
            {
                // 半修改优于毁产物:已写入的 tag 保留,绝不回滚原方法状态
                LogMainWarning("初始处女 tag 移除失败,保留原产物: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // 共用小件(同 MaidenAttrIphPatch;独立持有,不依赖其 install 期解析缓存)
        // ------------------------------------------------------------------

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
