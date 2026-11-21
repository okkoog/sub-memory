using System;
using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 家族 mod 状态快照(Family.Probe 的返回元素;Router 与装配期查询消费):
    /// ModId = 家族 mod ID;Present = 在场且启用(判定通道见 Family.Probe);
    /// Version = manifest ModVersion(可能为 null);
    /// VersionOk = Present 且版本命中 FamilyMods 绑定表。
    /// </summary>
    public sealed class FamilyState
    {
        public string ModId;
        public bool Present;
        public string Version;
        public bool VersionOk;
    }

    /// <summary>
    /// 家族 mod 版本绑定表(纯逻辑,System.* only,随 Config/ 通配进测试工程):
    /// 本构建对 8 个家族 mod 各自验证过的精确版本(spec §4.6/§5.7 版本严格绑定)。
    /// 语义:精确字符串相等,不做语义化比较(补丁差异即视为不符,与 VersionGuard
    /// 同一裁定);actual 为 null/空 → 不匹配(manifest ModVersion 经加载器 Trim
    /// 后可能为空串,同样视为不命中)。
    /// lewd_meme 行与 VersionGuard.SupportedMainModVersion 刻意同值——前者服务
    /// 家族钩子按需装配(0.6.0),后者服务既有主 mod 判定,两处测试钉住防漂移。
    /// </summary>
    public static class FamilyMods
    {
        /// <summary>主 mod(前置,家族钩子的接管目标)。</summary>
        public const string MainMod = "lewd_meme";

        /// <summary>Brood(子 mod,SubModHook 载体)。</summary>
        public const string Brood = "claude.lewdMemeBrood";

        /// <summary>remake(子 mod,在场时 nightly_tick 让位)。</summary>
        public const string Remake = "lewd_meme_remake";

        /// <summary>brothel(子 mod)。</summary>
        public const string Brothel = "lewd_meme_brothel";

        /// <summary>tearl(子 mod)。</summary>
        public const string Tearl = "dui_mian_lan_qiu_ji_tearl";

        /// <summary>legacy content(子 mod)。</summary>
        public const string Legacy = "lewd_meme_legacy_content";

        /// <summary>crisp portraits(子 mod)。</summary>
        public const string Crisp = "lewd_meme_crisp_portraits";

        /// <summary>ItemPortraitHD(软依赖前置,IPH 兼容先例)。</summary>
        public const string Iph = "item_portrait_hd";

        /// <summary>mod ID → 绑定版本的只读表(恰好 8 项;Ordinal 保证精确匹配语义)。
        /// ShadowTrade 适配 Task 2(2026-11-17):remake 1.5.24 → 1.6.1(mod 更名
        /// ShadowTrade/暗流交易,基线 = Mods/LewdMemeShadowTrade 源码)——三处断链
        /// (ModSettings.get_ProductMultiplier 删除 / RecordNightProduct 改单参
        /// (string) / SemenProduction 类整体删除 → remake.semen_formula 单元
        /// 退役),RemakePatches 委托同步;CreateMilkVariant(:437-473)与
        /// ConfigureOvulationShot(:499-504,去重字典撤、identifier 守卫加)
        /// 数值面零漂移。
        /// 1.6.1 → 1.6.4(2026-11-18):装桶/管制品加载两修复,零接触面漂移(审计 2026-11-18)。
        /// 0.7.0 Task 1(2026-11-17):lewd_meme 1.7.1 → 1.9.2(基线 lewd_meme_192,
        /// 171 存档;版本表 bump 先行,1.9.2 载体迁移随后续任务)。
        /// Task 14b(2026-11-13):lewd_meme 1.6.1 → 1.7.1(基线 lewd_meme_171)、
        /// legacy 1.0.2 → 1.0.5(基线 legacy_0105;其 ModReflect 迁移 + SettleAll
        /// prefix 均与本 mod 无方法冲突,仅版本号绑定更新)。
        /// 2026-11-13:remake 1.5.16 → 1.5.19(基线 remake_1519)——写透 7 键
        /// (ModSettings.cs)与接管结构(AddBodyExp/GetBodyLevel/NightlyTick)零漂移;
        /// 1.5.17 loot NOP transpiler 适配主 mod 1.7.1(此前「1.7.1 上 remake loot
        /// 自撤」的 over-yield 边缘撤销),让位规则 presence-based 不变。
        /// 2026-11-15(R13a):remake 1.5.21 → 1.5.24(基线 remake_1524)——写透
        /// 7 键 Define 段逐字一致;差分接管点(CreateMilkVariant/TryProduceMilk/
        /// ConfigureOvulationShot/AddAlarm* 三方法)签名与常量零漂移;SemenProduction
        /// 未变;CompatOriginal 唯一实质变化 = 新增 OriginalBuyerGuestPatch(patch
        /// 主 mod GuestBox.Show 收购商人格屏蔽),与我方 pools.guest_display
        /// (DisplayEnabled 字段写,无补丁)方法无冲突语义正交,让位表零触碰。
        /// 2026-11-18(IPH bump):IPH 1.2.18 → 1.3.0(基线 iph_130)——写透
        /// 16 键键名/默认/Choices/CSV 形态零漂移(hotkey 迁官方 KeyBind 控件,
        /// 仍按 Brood 热键先例不镜像);工厂 transpiler 六替换/三池与 JOBS
        /// 让位依据原样(Install 删 PerformanceTrace 段、反射合规化,零接触面)。
        /// IPH 1.2.17 → 1.2.18(基线 iph_1218)——17 键(16 镜像 + hotkey)键名/
        /// 默认/Choices/CSV 形态逐字一致;OriginalCompat 工厂 transpiler 逻辑零
        /// 漂移(属性掷骰段仍零引用,R10 属性 postfix 共存前提成立);
        /// AdultBodySources.PickBody/PickBreastForBody 与 BodyCsvSettings 两 CSV
        /// 键机制不变,R9 池拆分依据成立。legacy 1.0.5 → 1.0.6(基线 legacy_106)
        /// ——patch 面(BaseContentPatches)零变化,仅内容文本层微调
        /// (MaidenContentOverlay 不再覆写 MAIDEN_FACTION/BODY/BREAST 与
        /// item.name,ContentFactoryPostfixes 不再覆写 name),与本 mod 无方法
        /// 交互,仅版本号绑定更新。
        /// 2026-11-13(R8):remake 1.5.19 → 1.5.21(基线 remake_1521)——写透 7 键
        /// Define 段逐字一致;差分接管点(CreateMilkVariant/TryProduceMilk/
        /// ConfigureOvulationShot/AddAlarm*)签名与常量零漂移,仅反编译行号偏移;
        /// 收购商重构(+10 Buyer* 文件)patch 目标全为游戏类型(商店/收购 UI),
        /// 让位表零触碰。IPH 1.2.15 → 1.2.17(基线 iph_1217)——OriginalCompat
        /// Install 清单逐项一致(工厂 transpiler 仍全程接管),AdultBodySources
        /// PickBody/PickBreastForBody 签名不变;新立绘包(AllGirls/BodyCsv/
        /// Rimworld)纯数据源零 Harmony。
        /// ⚠️ bump 版本时:本字典值与 FamilyModsTests ⑤ 的显式期望表<b>两处同改</b>
        /// (R5 教训:上一波只改了本注释的版本号、漏改字典值,remake 1.5.19 在场
        /// 被误判版本不匹配,R4 新增 4 个 SubModHook 单元遭 VersionBlocked 误压制)。
        /// ⚠️ bump 版本时 manifest.xml Description 的「绑定版本:…」串与
        /// <see cref="BoundVersionsSummary"/> 同步手抄(见其注释,测试 ⑥ 钉顺序)。</summary>
        public static readonly IReadOnlyDictionary<string, string> BoundVersions =
            new Dictionary<string, string>(8, StringComparer.Ordinal)
            {
                { MainMod, "1.9.2" },
                { Brood, "0.3.31" },
                { Remake, "1.6.4" },
                { Brothel, "0.10.4" },
                { Tearl, "7" },
                { Legacy, "1.0.6" },
                { Crisp, "1.0.0" },
                { Iph, "1.3.0" },
            };

        /// <summary>actual 是否命中 modId 的绑定版本:null/空 → false;其余精确字符串相等;未知 modId → false。</summary>
        public static bool VersionMatches(string modId, string actual)
        {
            if (string.IsNullOrEmpty(actual)) return false;
            return BoundVersions.TryGetValue(modId, out string bound) && actual == bound;
        }

        /// <summary>
        /// 绑定版本一行摘要(R13b):manifest Description 的「绑定版本:…」串的
        /// 单一事实源——值全部取自 <see cref="BoundVersions"/>(bump 版本时自动
        /// 跟随),显示名 = 玩家向短名(游戏内 mod 列表的 Name 简称,非 mod ID)。
        /// ⚠️ manifest.xml 的 Description 里手抄了本串(构建期注入做不到)——
        /// <b>bump BoundVersions 时 manifest 与本方法两处同改</b>(FamilyModsTests
        /// ⑥ 的镜像断言可拦显示名/顺序漂移,版本数值漂移由 ⑤ 显式期望表拦)。
        /// </summary>
        public static string BoundVersionsSummary()
        {
            return "绑定版本:lewd_meme " + BoundVersions[MainMod]
                + " / 暗流交易 " + BoundVersions[Remake]
                + " / 巢穴扩展 " + BoundVersions[Brood]
                + " / 掌上青楼 " + BoundVersions[Brothel]
                + " / tearlaments " + BoundVersions[Tearl]
                + " / 旧内容恢复 " + BoundVersions[Legacy]
                + " / 触手bug修复 " + BoundVersions[Crisp]
                + " / 物品高清立绘 " + BoundVersions[Iph];
        }
    }
}
