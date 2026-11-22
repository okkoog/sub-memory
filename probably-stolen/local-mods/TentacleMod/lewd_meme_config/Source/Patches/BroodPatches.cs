using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 巢穴扩展·信息素两键接管(0.6.0 R12,目录单元 brood.perfume_mod;
    /// Kind = SubModHook,TargetMod = <see cref="FamilyMods.Brood"/>,绑定版本
    /// 0.3.31)。等价恢复背景(依据 r12-reaudit-report.md 一、零风险清单):
    /// 主面板 <c>client.semen_price_mod / semen_bonus_mod</c>(main.semen_effects
    /// 单元,postfix on 主 mod TentacleEffects.Make*Feature)在 Brood 在场时让位
    /// ——顾客进场 Brood <c>Fragrance.OnClientArea</c> 开头无条件调
    /// <c>WeakenBasePerfume</c>,用<b>同一 API 同一槽位</b>
    /// (<c>ItemFeature.SetPreExposedValueMod</c>)把两特性强制 ±10
    /// (brood_0331 BroodMainCycle.TentacleChild/Fragrance.cs:30-47,买价 +10
    /// :39 / 影响 −10 :43),时序后置恒胜创建时的本 mod postfix。
    /// 恢复形态(R10 属性键同模式):postfix on Brood
    /// <c>WeakenBasePerfume</c>(internal static void,参数名 item,签名与
    /// brood_0331 Fragrance.cs:30 逐字符核对;运行时唯一入口 = OnClientArea
    /// :19,:103 的调用是 RegisterTests 测试代码)——Brood 写完 ±10 后同 API
    /// 同槽位重设面板值,每次进场重申 = 稳态恒胜;不跳过原方法(Brood 逻辑
    /// 原样跑),postfix 天然共存。
    /// 槽位配对以主 mod 反编译为准(lewd_meme_171 MidnightPawn.Game/
    /// TentacleEffects.cs:78-101/:53-76,与主面板 Make*/TryApply 三 postfix
    /// 同一配对):<c>MakeSemenFeature → tentacleSemenInfluence</c>(原字面量
    /// −15 = price 键)、<c>MakeBuyBonusFeature → tentacleSemenBuyBonus</c>
    /// (+15 = bonus 键)。
    /// 零漂移红线(R10 模式):全默认(两键读回均 = 主 mod 原字面量 -15/+15,
    /// 含开关开但值 = 默认)→ 零参与(Brood 的 ±10 原样保留);任一键 ≠ 默认 →
    /// 两特性按面板值重设。默认全关 = 单元不装 = 物理零 hook(穿透原则);
    /// Brood 缺席 → ③ 登记期拦截不注册(R11-F1);版本漂移 → VersionBlocked
    /// 自动压制(bound 0.3.31 钉住签名)。
    /// 类型分工(TearlPatches/BrothelPatches 先例):Brood 类型一律运行时经
    /// <c>ModReflect.Types(FamilyMods.Brood)</c> 按 Name 解析(manifest 已声明
    /// 前置,缺席仅 warning),不编译引用其 DLL;游戏类型(GameItem/
    /// ItemFeature)直接编译引用。解析只发生在装配委托内(两阶段提交:
    /// EnsureResolved 任一成员失败 → 原样上抛不提交,单元记 Dead,开关
    /// off→on 可重试)。postfix 修正体异常 → 吞掉记日志(Brood 已写的 ±10
    /// 保留,零参与语义)。
    /// 红线:主面板旧键 client.semen_* 的 Brood 让位行为零改动(Router R2
    /// 不动);client.semen_rep_mult 的让位维持(r12 复审低风险项
    /// AttachSemen 二次包装,本波不实现)。
    /// </summary>
    public static class BroodPatches
    {
        // 主 mod 原文槽位字面量(lewd_meme_171 TentacleEffects.cs:74 +15 / :99 −15):
        // 面板默认 = 主 mod 原值(R10 模式),≠ 默认才进场重设
        internal const float PriceModDefault = -15f;
        internal const float BonusModDefault = 15f;

        // 主 mod 特性标识符常量复刻(TentacleEffects.cs:13/15 DISCOUNT_ID/BONUS_ID;
        // Brood WeakenBasePerfume 同串匹配,brood_0331 Fragrance.cs:37/41)
        private const string InfluenceId = "tentacleSemenInfluence"; // = price 槽位(MakeSemenFeature)
        private const string BuyBonusId = "tentacleSemenBuyBonus";   // = bonus 槽位(MakeBuyBonusFeature)

        // -- Brood 类型的运行时缓存(install 期经 ModReflect 填充,postfix 只做直接调用)--
        private static bool resolved;
        private static System.Type fragrance;      // BroodMainCycle.TentacleChild.Fragrance(patch 目标宿主)
        private static System.Reflection.Assembly broodAssembly; // 防串锚(全部解析类型必须同属此程序集)

        /// <summary>把接管单元的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("brood.perfume_mod", h =>
            {
                EnsureResolved();
                // brood_0331 Fragrance.cs:30 internal static void WeakenBasePerfume(GameItem item)
                // ——参数名 item 与基线逐字符一致(Harmony 按名注入;R6 教训)
                MethodBase m = ModReflect.Target(fragrance, "WeakenBasePerfume",
                    new[] { typeof(GameItem) });
                h.Patch(m, postfix: new HarmonyMethod(WeakenBasePerfumePostfix));
                return new[] { m };
            });
        }

        /// <summary>
        /// 幂等解析 Brood 成员(两阶段提交,同 TearlPatches/BrothelPatches:
        /// 任一成员解析抛出 → 原样上抛不提交任何字段,单元记 Dead)。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type frag = FindBroodType("Fragrance");
            // 全部成功,一次性提交
            fragrance = frag;
            resolved = true;
        }

        /// <summary>在前置 claude.lewdMemeBrood 暴露的类型里按名查找。防串断言:
        /// 解析类型锚定单一程序集(RemakePatches.FindRemakeType 先例;Fragrance
        /// 为 Brood 独有类,主 mod 无同名)——加载器行为漂移时装配期即炸(记
        /// Dead),绝不装错族方法。</summary>
        private static System.Type FindBroodType(string name)
        {
            foreach (System.Type t in ModReflect.Types(FamilyMods.Brood))
                if (t.Name == name)
                {
                    if (broodAssembly == null) broodAssembly = t.Assembly;
                    else if (broodAssembly != t.Assembly)
                        throw new MissingMemberException(
                            "Brood 类型程序集不一致(疑似串到其他 mod 同名类型): " + name);
                    return t;
                }
            throw new MissingMemberException("前置 claude.lewdMemeBrood 未暴露类型 " + name);
        }

        /// <summary>
        /// postfix on Brood <c>Fragrance.WeakenBasePerfume(GameItem item)</c>
        /// (brood_0331 Fragrance.cs:30-47):Brood 遍历 itemFeatures 把
        /// BuyBonus 强制 +10、Influence 强制 −10 后,本 postfix 同 API 同槽位
        /// 重设面板值(PercentToInt 取整,主面板 Make* postfix 同换算)。
        /// 全默认(两键 = -15/+15)→ 零参与(精确零漂移,Brood ±10 原样);
        /// 特性不存在 → 不添加(与 Brood 的「只改已有特性」遍历同门控)。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void WeakenBasePerfumePostfix(GameItem item)
        {
            if (item == null || item.itemFeatures == null) return; // 与 OnClientArea :15 同守卫
            float price, bonus;
            try
            {
                price = CfgF(CfgKeys.BroodSemenPriceMod, PriceModDefault);
                bonus = CfgF(CfgKeys.BroodSemenBonusMod, BonusModDefault);
            }
            catch (Exception ex)
            {
                LogWarning("信息素两键配置读取失败,保持巢穴扩展原值: " + ex.Message);
                return;
            }
            if (price == PriceModDefault && bonus == BonusModDefault)
            {
                return; // 全默认 = 零参与(R10 模式;Brood 的 ±10 语义完整保留)
            }
            try
            {
                int priceMod = ValueMath.PercentToInt(price);
                int bonusMod = ValueMath.PercentToInt(bonus);
                foreach (ItemFeature f in item.itemFeatures)
                {
                    if (f == null) continue;
                    if (f.identifier == BuyBonusId)             // 主 mod MakeBuyBonusFeature 槽位(+15)
                    {
                        f.SetPreExposedValueMod(bonusMod, true);
                    }
                    else if (f.identifier == InfluenceId)       // 主 mod MakeSemenFeature 槽位(−15)
                    {
                        f.SetPreExposedValueMod(priceMod, true);
                    }
                }
                Log("巢穴扩展信息素接管: 买价 " + bonusMod + "% / 影响 " + priceMod
                    + "%(Brood 压制值已被面板值重设)");
            }
            catch (Exception ex)
            {
                // 吞掉记日志:Brood 已写的 ±10 保留(零参与语义),绝不回滚原方法状态
                LogWarning("巢穴扩展信息素接管失败,保留其 ±10: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // 共用小件(独立持有,不依赖其他 patch 类的 install 期解析缓存)
        // ------------------------------------------------------------------

        /// <summary>float 取值兜底:PatchCtx 未注入(mod 停用后)→ 原版常量(等价零参与)。</summary>
        private static float CfgF(string key, float originalDefault)
        {
            return PatchCtx.Config?.Current<float>(key) ?? originalDefault;
        }

        /// <summary>日志出口:Brood 无跨 mod 日志通道可复用(其 Log 为 internal
        /// 自家类),退回自有前缀(DebugPanel/SelfTestRunner 同例)。</summary>
        private static void Log(object msg)
        {
            UnityEngine.Debug.Log("[lewd_meme_config→巢穴扩展] " + msg);
        }

        private static void LogWarning(object msg)
        {
            UnityEngine.Debug.LogWarning("[lewd_meme_config→巢穴扩展] " + msg);
        }
    }
}
