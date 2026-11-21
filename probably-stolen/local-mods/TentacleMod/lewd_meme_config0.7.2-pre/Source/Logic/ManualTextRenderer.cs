using System;
using System.Globalization;
using System.Text;

namespace LewdMemeConfig
{
    /// <summary>
    /// 说明书文案渲染的纯逻辑(Task 12):<c>{cfg:key}</c> 占位符替换器 +
    /// 触手说明书模板 + 键感知数值格式化。模板 = 主 mod v1.9.2
    /// <c>ItemFactory.CreateNote</c>(反编译 L325;0.7.0 T7 自 1.3.8/L253
    /// 同步三处文案漂移)的 shortDescription 原文,
    /// 仅把有 CfgKeys 键的数字位换成占位符(映射见测试
    /// ManualTextRendererTests 头注;无键数字——便签区 2×2、"每 10 点 +10%"、
    /// "（60 点必定产出）"、"≥1"、"受孕率 = 当晚性交次数%" 等——原样保留,
    /// 不发明键)。默认配置渲染结果与原文逐字一致 = 验收红线(测试断言)。
    /// 零游戏类型;FormatValue 引用 CfgKeys(同 NightlyFormulas 先例)。
    /// </summary>
    public static class ManualTextRenderer
    {
        /// <summary>
        /// 触手说明书模板:CreateNote 原文 + {cfg:key} 占位(键映射:
        /// 侵蚀区 corrupt_cols×brood_size / 苗床区 brood_size² / 产出区
        /// output_size² / 触手区 tool_cols×brood_size——腐蚀/触手区行高取
        /// brood_size 是 Task 7 BuildWindow 拆键裁定;性爱/口交/肛交六键;
        /// 精液 40%=semen_chance_base;乳汁 100+40=milk_value_base/per_lv;
        /// 1/7=ovulate_chance;30 天=gestation_days;充能 10/+10/100=
        /// corrupt.daily_recharge/semen_recharge/max_charge)。
        /// 0.7.0 T7 同步 1.9.2 文案(反编译 ItemFactory.cs L325):侵蚀触手
        /// 两行「充能」改「淫力」(×2)、精液行尾增「（可多瓶一起拖）」
        /// (1.9.2 OnSemenUsed 按瓶去重的玩家侧说明)。
        /// </summary>
        public const string Template =
            "【本体的五个区，从左到右】\n" +
            "① 侵蚀区 {cfg:layout.corrupt_cols}×{cfg:layout.brood_size}：只收带【触手侵蚀】的箱子 / 机器\n" +
            "② 苗床区 {cfg:layout.brood_size}×{cfg:layout.brood_size}：把【女性角色】放进来，每晚自动结算调教\n" +
            "③ 产出区 {cfg:layout.output_size}×{cfg:layout.output_size}：产出的乳汁 / 精液落在这里\n" +
            "④ 触手区 {cfg:layout.tool_cols}×{cfg:layout.brood_size}：只收带【触手】标签的工具（侵蚀性触手）\n" +
            "⑤ 便签区 2×2：就是这张纸\n" +
            "\n" +
            "【每晚结算（每个苗床）】\n" +
            "· 性爱 {cfg:nightly.sex_min_base}~{cfg:nightly.sex_max_base} 次、口交 {cfg:nightly.oral_min_base}~{cfg:nightly.oral_max_base} 次、肛交 {cfg:nightly.anal_min_base}~{cfg:nightly.anal_max_base} 次，本体等级越高越多\n" +
            "· 调教值 = 性交 + 口交 + 肛交次数之和\n" +
            "· 精液产出率 = {cfg:nightly.semen_chance_base}% + 调教值每 10 点 +10%（60 点必定产出）\n" +
            "· 泌乳等级 ≥1 才产【神奇乳汁】，价值 = {cfg:nightly.milk_value_base} + 等级×{cfg:nightly.milk_value_per_lv}\n" +
            "· 每天 {cfg:nightly.ovulate_chance} 概率进入排卵期，受孕率 = 当晚性交次数%\n" +
            "· 怀孕 {cfg:nightly.gestation_days} 天后产下一个继承立绘和势力的女儿\n" +
            "\n" +
            "【触手区】\n" +
            "· 侵蚀性触手每天自动回 {cfg:corrupt.daily_recharge} 点淫力；把【触手精液】拖到它身上 +{cfg:corrupt.semen_recharge}（可多瓶一起拖）\n" +
            "· 淫力满 {cfg:corrupt.max_charge} 时，把下面这些【拖到它身上】，可以侵蚀成【触手苗床箱】：\n" +
            "\u3000工程铸造厂物资箱 / 后勤服务部物资箱 / 医疗舱物资箱 /\n" +
            "\u3000治安部物证柜 / 治安部物资箱 / 储藏区 / 储藏区（扩建）\n" +
            "· 带【重要物品】标签的东西（剧情道具之类）一律侵蚀不了\n" +
            "\n" +
            "本体的侵蚀区是暗格，盘查时里面的东西不会被搜出来。";

        /// <summary>占位符前缀(扫描锚点)。</summary>
        private const string Prefix = "{cfg:";

        /// <summary>
        /// 渲染:把 template 中每个 <c>{cfg:key}</c> 换成 resolveKey(key)。
        /// 契约:resolveKey 返回 null = 未知键,占位符原样保留(含大括号);
        /// 返回空串 = 已知且渲染为空(照常替换)。无占位符 / template 为 null /
        /// resolveKey 为 null → 原样返回。未闭合占位符("{cfg:" 后无 "}")原样。
        /// </summary>
        public static string Render(string template, Func<string, string> resolveKey)
        {
            if (template == null || resolveKey == null)
            {
                return template;
            }
            int start = template.IndexOf(Prefix, StringComparison.Ordinal);
            if (start < 0)
            {
                return template;
            }
            StringBuilder sb = new StringBuilder(template.Length + 64);
            int pos = 0;
            while (start >= 0)
            {
                sb.Append(template, pos, start - pos);
                int end = template.IndexOf('}', start + Prefix.Length);
                if (end < 0)
                {
                    // 其余部分无闭合大括号:整段原样收尾
                    sb.Append(template, start, template.Length - start);
                    return sb.ToString();
                }
                string key = template.Substring(start + Prefix.Length, end - start - Prefix.Length);
                string resolved = resolveKey(key);
                if (resolved == null)
                {
                    sb.Append(template, start, end - start + 1); // 未知键:占位符逐字保留
                }
                else
                {
                    sb.Append(resolved);
                }
                pos = end + 1;
                start = template.IndexOf(Prefix, pos, StringComparison.Ordinal);
            }
            sb.Append(template, pos, template.Length - pos);
            return sb.ToString();
        }

        /// <summary>
        /// 键感知格式化(InvariantCulture,与游戏/主 mod 字面量一致):
        /// semen_chance_base(float)→ 整数百分比(×100 四舍五入,半点远离零,
        /// 0.4 → "40");ovulate_chance(float)→ 与默认 1/7 距离 &lt; 0.0005 渲染回
        /// 原文写法 "1/7",否则整百分比 + "%";其余键(int / 未特化 float)→
        /// InvariantCulture ToString。value 为 null → null(调用方按未知键处理)。
        /// </summary>
        public static string FormatValue(string key, object value)
        {
            if (value == null)
            {
                return null;
            }
            switch (key)
            {
                case CfgKeys.SemenChanceBase:
                    return Percent((float)Convert.ToDouble(value, CultureInfo.InvariantCulture));
                case CfgKeys.OvulateChance:
                    float chance = (float)Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    if (Math.Abs(chance - 1f / 7f) < 0.0005f)
                    {
                        return "1/7";
                    }
                    return Percent(chance) + "%";
                default:
                    return Convert.ToString(value, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>概率 float → 整数百分比文本(0.4→"40";半点远离零,同 ValueMath)。</summary>
        private static string Percent(float fraction)
        {
            return Math.Round((double)fraction * 100.0, MidpointRounding.AwayFromZero)
                .ToString(CultureInfo.InvariantCulture);
        }
    }
}
