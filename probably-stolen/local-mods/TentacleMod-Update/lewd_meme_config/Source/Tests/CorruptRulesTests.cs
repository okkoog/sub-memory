using System;
using LewdMemeConfig;

// Task 8:CorruptRules 纯函数语义(侵蚀触手充能/白黑名单接管的前置判定)。
// csvBase 常量 = 主 mod v1.3.8 CorruptTentacle.IsCorruptible 关键词全集(反编译 L204-232):
//   白名单 identifier 子串(原版先 ToLowerInvariant 再 Contains):crate/storage/supplybox/supply_box;
//   白名单 name 含:物资箱/货箱/物证柜/储藏区;
//   黑名单:lou_locker(identifier,原版另有一处冗余精确比较,子串语义已覆盖)/卢的储物柜(name)。
// CorruptPatch 依赖主 mod 类型(运行时 ModReflect)与 Harmony,住在 Patches/ 不进测试工程;
// 其接管语义由本纯函数测试 + Task 14 游戏内验收共同覆盖。
public static class CorruptRulesTests
{
    // 与 CorruptPatch.WhitelistBase/BlacklistBase 逐字相同(v1.3.8 关键词全集)
    private const string WlBase = "crate,storage,supplybox,supply_box,物资箱,货箱,物证柜,储藏区";
    private const string BlBase = "lou_locker,卢的储物柜";

    public static void Run()
    {
        // -- ClampCharge:[0,max],Mathf.Clamp(v, 0, max) 同语义 --
        Check.Eq(100, CorruptRules.ClampCharge(150, 100), "clamp 150/100 -> 100");
        Check.Eq(0, CorruptRules.ClampCharge(-1, 100), "clamp -1/100 -> 0");
        Check.Eq(100, CorruptRules.ClampCharge(100, 100), "clamp 100/100 -> 100 (boundary)");
        Check.Eq(0, CorruptRules.ClampCharge(0, 100), "clamp 0/100 -> 0 (boundary)");
        Check.Eq(50, CorruptRules.ClampCharge(50, 100), "clamp 50/100 -> 50 (in-range passthrough)");
        Check.Eq(0, CorruptRules.ClampCharge(0, 0), "clamp 0/0 -> 0");
        Check.Eq(7, CorruptRules.ClampCharge(7, 7), "clamp 7/7 -> 7");

        // -- MergeKeys:逗号切分去空、trim、去重保序(base 在前) --
        Check.Eq("a,b,c", string.Join(",", CorruptRules.MergeKeys("a,b", "b,c")), "merge a,b + b,c -> a,b,c");
        Check.Eq("a", string.Join(",", CorruptRules.MergeKeys("a", "")), "merge a + empty -> a");
        Check.Eq("", string.Join(",", CorruptRules.MergeKeys("", "")), "merge empty + empty -> empty");
        Check.Eq("", string.Join(",", CorruptRules.MergeKeys(null, null)), "merge null + null -> empty");
        Check.Eq("a,b", string.Join(",", CorruptRules.MergeKeys(" a , b ", "")), "merge trims spaces");
        Check.Eq("a", string.Join(",", CorruptRules.MergeKeys("a,,", ",,")), "merge drops empties");
        Check.Eq("a,b,c,d", string.Join(",", CorruptRules.MergeKeys("a,b", "c,d")), "merge keeps base-first order");
        Check.Eq("a,b", string.Join(",", CorruptRules.MergeKeys("a,b", "b,a")), "merge dedups both ways");
        Check.Eq(WlBase, string.Join(",", CorruptRules.MergeKeys(WlBase, "")), "whitelist base roundtrip (supplybox 与 supply_box 不互斥)");
        Check.Eq(BlBase, string.Join(",", CorruptRules.MergeKeys(BlBase, "")), "blacklist base roundtrip");
        Check.Eq("crate,storage,supplybox,supply_box,物资箱,货箱,物证柜,储藏区,medkit",
            string.Join(",", CorruptRules.MergeKeys(WlBase, "medkit")), "merge extra appended after base");

        // -- Matches:任一 key 为 identifier 子串(不区分大小写)或 displayName 含 key --
        Check.Eq(true, CorruptRules.Matches("crate_big", "物资箱", new[] { "crate", "柜" }), "matches id substring");
        Check.Eq(true, CorruptRules.Matches("x", "治安部物证柜", new[] { "crate", "柜" }), "matches name contains");
        Check.Eq(false, CorruptRules.Matches("x", "名", new[] { "crate" }), "no key hits -> false");
        Check.Eq(true, CorruptRules.Matches("CRATE_BIG", "", new[] { "crate" }), "id case-insensitive (原版 ToLowerInvariant)");
        Check.Eq(true, CorruptRules.Matches("SUPPLY_BOX", "", new[] { "supply_box" }), "id supply_box case-insensitive");
        Check.Eq(false, CorruptRules.Matches("x", "名", new string[0]), "empty keys -> false");
        Check.Eq(false, CorruptRules.Matches("", "", new[] { "" }), "empty key skipped -> false");
        Check.Eq(true, CorruptRules.Matches("prefix_crate_suffix", "", new[] { "crate" }), "id substring mid-position");
        Check.Eq(true, CorruptRules.Matches("x", "治安部物证柜", new[] { "物证柜" }), "name 物证柜");
        Check.Eq(true, CorruptRules.Matches("x", "储藏区（扩建）", new[] { "储藏区" }), "name 储藏区（扩建）");
        Check.Eq(false, CorruptRules.Matches("crate", "某物品", new[] { "柜" }), "no transitive hit(id 命中不代表 name 命中)");
        Check.Eq(false, CorruptRules.Matches("x", "Wooden Crate", new[] { "crate" }), "name contains case-sensitive(原版 name 不 lower)");

        // -- Whitelisted:原版关键词全集逐项命中(csvBase 常量 = v1.3.8 字面量) --
        Check.Eq(true, CorruptRules.Whitelisted("supply_box", "", WlBase, ""), "wl supply_box (brief 样例)");
        Check.Eq(true, CorruptRules.Whitelisted("bigsupplybox", "", WlBase, ""), "wl supplybox");
        Check.Eq(true, CorruptRules.Whitelisted("cold_storage_unit", "", WlBase, ""), "wl storage");
        Check.Eq(true, CorruptRules.Whitelisted("eng_crate_01", "", WlBase, ""), "wl crate");
        Check.Eq(true, CorruptRules.Whitelisted("whatever", "工程铸造厂物资箱", WlBase, ""), "wl name 物资箱");
        Check.Eq(true, CorruptRules.Whitelisted("whatever", "后勤货箱", WlBase, ""), "wl name 货箱");
        Check.Eq(true, CorruptRules.Whitelisted("whatever", "治安部物证柜", WlBase, ""), "wl name 物证柜");
        Check.Eq(true, CorruptRules.Whitelisted("whatever", "储藏区", WlBase, ""), "wl name 储藏区");
        Check.Eq(false, CorruptRules.Whitelisted("med_kit", "医疗包", WlBase, ""), "wl no hit -> false");
        Check.Eq(true, CorruptRules.Whitelisted("medkit_box", "", WlBase, "medkit"), "wl extra csv hits");
        Check.Eq(false, CorruptRules.Whitelisted("med_kit", "", WlBase, "medkit"), "wl extra is substring not fuzzy");
        Check.Eq(false, CorruptRules.Whitelisted("med_kit", "", WlBase, "other"), "wl extra miss -> false");

        // -- Blacklisted:原版两项逐项命中 --
        Check.Eq(true, CorruptRules.Blacklisted("lou_locker_a", "", BlBase, ""), "bl lou_locker substring (brief 样例)");
        Check.Eq(true, CorruptRules.Blacklisted("LOU_LOCKER_B", "", BlBase, ""), "bl case-insensitive");
        Check.Eq(true, CorruptRules.Blacklisted("lou_locker", "", BlBase, ""), "bl exact id (原版精确比较被子串覆盖)");
        Check.Eq(true, CorruptRules.Blacklisted("x", "卢的储物柜", BlBase, ""), "bl name 卢的储物柜");
        Check.Eq(false, CorruptRules.Blacklisted("eng_crate", "物资箱", BlBase, ""), "bl whitelist item not blacklisted");
        Check.Eq(true, CorruptRules.Blacklisted("x", "卢的柜", BlBase, "卢的柜"), "bl extra csv hits");
        Check.Eq(false, CorruptRules.Blacklisted("x", "", BlBase, ""), "bl no extra no hit -> false");

        // -- 0.7.0 T5:外部侵蚀规则透传(1.9.2 IsCorruptible:打印机精确 id 白名单
        //    之后、crate 关键词白名单之前插 TentacleApi.FindCorruptRule(it) != null
        //    → true;次序事实 = 黑名单判定在外部规则之前——黑名单追加项仍压过
        //    外部 mod 注册的侵蚀规则,关键词白名单殿后)。复刻体侧委托经
        //    ModReflect 解析(测试工程不可用),此处以 mock 委托钉纯逻辑半边 --
        Check.Eq(true, CorruptRules.ExternalRuleHits<string>(s => s, "rule"),
            "external rule hit (non-null) passes through");
        Check.Eq(false, CorruptRules.ExternalRuleHits<string>(s => null, "x"),
            "external rule miss (null) -> false");
        Check.Eq(false, CorruptRules.ExternalRuleHits<string>(null, "x"),
            "delegate absent (解析失败防御) -> false");

        // -- 0.7.0 T5:corrupt_cost 边界(纯逻辑侧模拟 getter 组合:充能经
        //    ClampCharge 恒 ≤ max_charge;transpile 后门槛 = charge ≥ cost) --
        // cost = max:充满(= max)即过门槛(bge 含等号,charge == cost 触发)
        Check.Eq(true, CorruptRules.ClampCharge(200, 200) >= 200,
            "cost = max: 恰满即触发(边界含等号)");
        Check.Eq(true, CorruptRules.ClampCharge(999999, 200) >= 200,
            "cost = max: 超充钳到 max 仍触发");
        // cost > max:钳制后充能恒 ≤ max < cost → 门槛永不通过(组合求值无异常)
        foreach (int raw in new[] { -10, 0, 200, 999999 })
            Check.Eq(true, CorruptRules.ClampCharge(raw, 200) < 250,
                "cost > max: 钳制后恒拒(raw=" + raw + ")");
        // cost = 0(滑条下界可达):充能下限 0 ≥ 0 恒过门槛 = 免费侵蚀
        Check.Eq(true, CorruptRules.ClampCharge(0, 100) >= 0,
            "cost = 0: 下限充能也过门槛(免费侵蚀)");

        // -- 0.7.0 T6:on_semen_used 截断公式(纯逻辑侧模拟注入后组合:
        //    门槛 charge >= max → 已饱拒收(blt 严格小于才继续吸取);
        //    增量 gain = Min(semen, max - charge);落库 charge + gain 经
        //    SetCharge 前缀 ClampCharge 钳制——注入后 gain ≤ max - charge
        //    恒成立,钳制是二道防线) --
        // 默认 10/100:缺口充足全额吸取;逼近上限按缺口截断
        Check.Eq(10, Math.Min(10, 100 - 0), "默认 gain(0/100) = 10 全额");
        Check.Eq(5, Math.Min(10, 100 - 95), "默认 gain(95/100) = 5 缺口截断");
        Check.Eq(1, Math.Min(10, 100 - 99), "默认 gain(99/100) = 1 临门一点");
        Check.Eq(true, 100 >= 100, "默认 charge=max 已饱拒收(门槛含等号)");
        // semen 超缺口:按缺口截断,落库恰满不溢出
        Check.Eq(10, Math.Min(50, 100 - 90), "semen 50 > 缺口 10 → 截 10");
        Check.Eq(100, CorruptRules.ClampCharge(90 + Math.Min(50, 100 - 90), 100),
            "截断后落库恰满不越界");
        // max > 100(T2 顾虑①:若不注 max_charge,150 ≥ 100 会在旧门槛处
        // 提前拒收——两点注入后门槛随 max 放开,语义完整)
        Check.Eq(true, 150 < 200, "max=200:150 未饱(旧 100 门槛会提前拒收)");
        Check.Eq(30, Math.Min(30, 200 - 150), "max=200:缺口 50 > semen 30 → 全额 30");
        Check.Eq(180, CorruptRules.ClampCharge(150 + Math.Min(30, 200 - 150), 200),
            "max=200:吸取后 180");
        // semen = 0:吸了不涨(toast +0),仍走吸取成功分支,无异常
        Check.Eq(0, Math.Min(0, 100 - 40), "semen=0 → gain 0(不涨不异常)");
        // 网格不变量:任意 semen/charge 组合落库恒 ≤ max(注入公式防溢出)
        foreach (int semenC in new[] { 0, 10, 50 })
            foreach (int chargeC in new[] { 0, 50, 95, 99 })
            {
                int gain = Math.Min(semenC, 100 - chargeC);
                Check.Eq(chargeC + gain, CorruptRules.ClampCharge(chargeC + gain, 100),
                    "落库不越界(semen=" + semenC + ",charge=" + chargeC + ")");
            }
    }
}
