using System;
using LewdMemeConfig;

// Task 12:ManualTextRenderer 纯函数语义(manual.render postfix 的渲染核心)
// + 模板红线:默认配置(无玩家干预)渲染结果与主 mod v1.3.8 CreateNote 的
// shortDescription 逐字节一致(原文 = 反编译 ItemFactory.cs L253,含 U+00B7
// 间隔号、U+00D7 乘号、U+2265 ≥、两处 U+3000 全角空格、ASCII "~")。
// 数值位置换 {cfg:key} 占位的映射(key 有值的才换,其余字面量原样):
//   ①侵蚀区 3×16   → corrupt_cols × brood_size(Task 7 裁定:腐蚀/触手区高度取 brood_size)
//   ②苗床区 16×16  → brood_size × brood_size
//   ③产出区 16×16  → output_size × output_size
//   ④触手区 3×16   → tool_cols × brood_size
//   ⑤便签区 2×2    → 无键(RectShape(2,2) 字面量),原样
//   性爱 5~10 / 口交 0~2 / 肛交 0~2 → 六个 *_base 键
//   精液产出率 40%  → semen_chance_base(×100 四舍五入);
//     "每 10 点 +10%" 与 "（60 点必定产出）" 是 per-train/cap 的派生表述,无独立键 → 原样
//   价值 100 + 等级×40 → milk_value_base / milk_value_per_lv;"≥1" 无键原样
//   1/7             → ovulate_chance(默认值渲染回 "1/7",非默认渲染为整百分比)
//   怀孕 30 天      → gestation_days
//   回 10 点 / +10 / 满 100 → corrupt.daily_recharge / semen_recharge / max_charge
//   "受孕率 = 当晚性交次数%" 无数字位(conceive_per_sex 无处替换)→ 原样
public static class ManualTextRendererTests
{
    /// <summary>主 mod v1.9.2 CreateNote shortDescription 原文(逐字,反编译 L325;
    /// 0.7.0 T7 同步:侵蚀触手两行「充能」改「淫力」、精液行尾增「（可多瓶一起拖）」)。</summary>
    private const string Original =
        "【本体的五个区，从左到右】\n" +
        "① 侵蚀区 3×16：只收带【触手侵蚀】的箱子 / 机器\n" +
        "② 苗床区 16×16：把【女性角色】放进来，每晚自动结算调教\n" +
        "③ 产出区 16×16：产出的乳汁 / 精液落在这里\n" +
        "④ 触手区 3×16：只收带【触手】标签的工具（侵蚀性触手）\n" +
        "⑤ 便签区 2×2：就是这张纸\n" +
        "\n" +
        "【每晚结算（每个苗床）】\n" +
        "· 性爱 5~10 次、口交 0~2 次、肛交 0~2 次，本体等级越高越多\n" +
        "· 调教值 = 性交 + 口交 + 肛交次数之和\n" +
        "· 精液产出率 = 40% + 调教值每 10 点 +10%（60 点必定产出）\n" +
        "· 泌乳等级 ≥1 才产【神奇乳汁】，价值 = 100 + 等级×40\n" +
        "· 每天 1/7 概率进入排卵期，受孕率 = 当晚性交次数%\n" +
        "· 怀孕 30 天后产下一个继承立绘和势力的女儿\n" +
        "\n" +
        "【触手区】\n" +
        "· 侵蚀性触手每天自动回 10 点淫力；把【触手精液】拖到它身上 +10（可多瓶一起拖）\n" +
        "· 淫力满 100 时，把下面这些【拖到它身上】，可以侵蚀成【触手苗床箱】：\n" +
        "\u3000工程铸造厂物资箱 / 后勤服务部物资箱 / 医疗舱物资箱 /\n" +
        "\u3000治安部物证柜 / 治安部物资箱 / 储藏区 / 储藏区（扩建）\n" +
        "· 带【重要物品】标签的东西（剧情道具之类）一律侵蚀不了\n" +
        "\n" +
        "本体的侵蚀区是暗格，盘查时里面的东西不会被搜出来。";

    public static void Run()
    {
        // -- Render:brief Step 1 样例 --
        Check.Eq("售价 180",
            ManualTextRenderer.Render("售价 {cfg:price.milk}", k => k == "price.milk" ? "180" : null),
            "render known key (brief)");
        Check.Eq("{cfg:nope}",
            ManualTextRenderer.Render("{cfg:nope}", k => null),
            "render unknown key kept verbatim (brief)");
        Check.Eq("无占位文本",
            ManualTextRenderer.Render("无占位文本", k => "x"),
            "no placeholder unchanged (brief)");

        // -- Render:多占位 / 重复键 / 空串解 / 边界形态 --
        Check.Eq("1 和 2",
            ManualTextRenderer.Render("{cfg:a} 和 {cfg:b}", k => k == "a" ? "1" : "2"),
            "two placeholders");
        Check.Eq("x-x",
            ManualTextRenderer.Render("{cfg:k}-{cfg:k}", k => "x"),
            "same key twice");
        Check.Eq("ab",
            ManualTextRenderer.Render("a{cfg:x}b", k => ""),
            "empty-string resolution replaces (null 才是未知哨兵)");
        Check.Eq("x{cfg:",
            ManualTextRenderer.Render("x{cfg:", k => "y"),
            "unclosed placeholder verbatim");
        Check.Eq("{cfg:}",
            ManualTextRenderer.Render("{cfg:}", k => null),
            "empty key -> unknown -> verbatim");
        Check.Eq("a{cfg:x}b}c",
            ManualTextRenderer.Render("a{cfg:x}b}c", k => null),
            "stray closing brace untouched");
        Check.Eq("前{cfg:x",
            ManualTextRenderer.Render("前{cfg:x", k => "y"),
            "unclosed placeholder keeps preceding text");
        Check.Eq(null,
            ManualTextRenderer.Render(null, k => "x"),
            "null template -> null");
        Check.Eq("模板",
            ManualTextRenderer.Render("模板", null),
            "null resolver -> template unchanged");
        Check.Eq("{cfg:x}",
            ManualTextRenderer.Render("{cfg:x}", null),
            "null resolver keeps placeholder");

        // -- FormatValue:键感知格式化(InvariantCulture) --
        Check.Eq("30", ManualTextRenderer.FormatValue(CfgKeys.GestationDays, 30), "int 30");
        Check.Eq("5", ManualTextRenderer.FormatValue(CfgKeys.SexMinBase, 5), "int 5");
        Check.Eq("40", ManualTextRenderer.FormatValue(CfgKeys.SemenChanceBase, 0.4f), "semen base 0.4 -> 40");
        Check.Eq("56", ManualTextRenderer.FormatValue(CfgKeys.SemenChanceBase, 0.555f), "semen base 0.555 -> 56 (半点远离零)");
        Check.Eq("100", ManualTextRenderer.FormatValue(CfgKeys.SemenChanceBase, 1f), "semen base 1 -> 100");
        Check.Eq("1/7", ManualTextRenderer.FormatValue(CfgKeys.OvulateChance, 1f / 7f), "ovulate default -> 1/7");
        Check.Eq("1/7", ManualTextRenderer.FormatValue(CfgKeys.OvulateChance, 0.1429f), "ovulate ~1/7 tolerance -> 1/7");
        Check.Eq("50%", ManualTextRenderer.FormatValue(CfgKeys.OvulateChance, 0.5f), "ovulate 0.5 -> 50%");
        Check.Eq("12%", ManualTextRenderer.FormatValue(CfgKeys.OvulateChance, 0.123f), "ovulate 0.123 -> 12%");
        Check.Eq("1", ManualTextRenderer.FormatValue(CfgKeys.ExpMultiplier, 1f), "fallback float invariant");

        // -- 红线:默认配置渲染 == 主 mod 原文逐字一致 --
        ConfigRegistry defaults = ConfigRegistry.CreateDefault();
        string renderedDefault = ManualTextRenderer.Render(ManualTextRenderer.Template,
            k => ManualTextRenderer.FormatValue(k, defaults.Current<object>(k)));
        Check.Eq(Original, renderedDefault, "default config renders verbatim original manual");

        // -- 玩家层改写生效:单键改值,其余默认 --
        ConfigRegistry tweaked = ConfigRegistry.CreateDefault();
        tweaked.SetPlayerValue(CfgKeys.GestationDays + ".enabled", true);   // 0.5.0:开关默认关,先开子项开关
        tweaked.SetPlayerValue(CfgKeys.GestationDays, 45);
        string rendered45 = ManualTextRenderer.Render(ManualTextRenderer.Template,
            k => ManualTextRenderer.FormatValue(k, tweaked.Current<object>(k)));
        Check.Eq(Original.Replace("怀孕 30 天", "怀孕 45 天"), rendered45,
            "gestation player value 30 -> 45");
        ConfigRegistry layout = ConfigRegistry.CreateDefault();
        layout.SetPlayerValue(CfgKeys.CorruptCols, 4);                     // 布局键不上面板,无子项开关
        layout.SetPlayerValue(CfgKeys.BroodSize, 12);
        string renderedLayout = ManualTextRenderer.Render(ManualTextRenderer.Template,
            k => ManualTextRenderer.FormatValue(k, layout.Current<object>(k)));
        Check.Eq(Original
                .Replace("① 侵蚀区 3×16", "① 侵蚀区 4×12")
                .Replace("② 苗床区 16×16", "② 苗床区 12×12")
                // ③ 产出区(output_size)与 ④ 的列宽(tool_cols)未改 → 原样;
                // ④ 行高与 ① 一样随 brood_size 12。
                .Replace("④ 触手区 3×16", "④ 触手区 3×12"),
            renderedLayout,
            "layout player values corrupt_cols=4 brood_size=12");
        Check.Eq(false, rendered45.Contains("{cfg:"), "rendered text has no leftover placeholder");
    }
}
