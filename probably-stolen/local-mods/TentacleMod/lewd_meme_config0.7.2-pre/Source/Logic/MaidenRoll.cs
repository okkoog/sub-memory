using System;
using System.Collections.Generic;
using System.Globalization;

namespace LewdMemeConfig
{
    /// <summary>
    /// 少女生成的纯分布逻辑(Task 9):属性三档滚动、属性价格曲线、阵营基价查表与
    /// 池写入守卫,零游戏类型依赖,供测试工程通配链接与 <see cref="MaidenPatch"/>
    /// 复用。语义对照主 mod v1.3.8 <c>MidnightPawn.Game.ItemFactory</c>:
    ///  RollAttr = 原版 <c>RollAttr()</c>(L466-478:Random.value &lt; 0.01f →
    ///  Range(70,81) 即 [70,80] 含端点;&lt; 0.06f → [60,70];否则 [10,60];
    ///  累计阈值 + 区间内均匀)。与原版的实现差异(sanctioned):原版每个属性消耗
    ///  两次随机数(value 选档 + Range 取值),纯函数用单个 roll 的档内小数位取值
    ///  (frac = 档内位置 / 档宽),默认阈值下三档概率与档内分布均与原版逐项一致;
    ///  AttrPrice = 原版 <c>AttrValue</c>(L480-491:≤60 原值;≤80: 60+(v-60)×5;
    ///  &gt;80: 160+(v-80)×10),断点经参数传入(调用点固定 60/80 = 原版常量,
    ///  斜率 5/10 按 brief 不做键);FactionBase = 原版 <c>FactionBase</c>
    ///  (L493-506:上层区 100 / 治安部·黑市·革命军 50 / default 10)的查表化,
    ///  默认 CSV 已逐项编码该表。全默认(无 override)时与原版逐项一致 = 验收红线。
    /// </summary>
    public static class MaidenRoll
    {
        /// <summary>
        /// 属性三档滚动:roll &lt; highChance → high 档;&lt; midChance(累计阈值)→
        /// mid 档;否则 low 档。档内取值 = roll 在该档的相对位置(小数位)乘档宽
        /// (含端点项数)再加 lo,单 roll 即得均匀分布(见类注释的 sanctioned 差异)。
        /// roll 约定 [0,1);可档宽 hi &lt; lo 时退化为恒 lo。
        /// </summary>
        public static int RollAttr(double roll, double highChance, double midChance,
            (int lo, int hi) high, (int lo, int hi) mid, (int lo, int hi) low)
        {
            if (roll < highChance)
            {
                return Pick(Share(roll, highChance), high);
            }
            if (roll < midChance)
            {
                return Pick(Share(roll - highChance, midChance - highChance), mid);
            }
            return Pick(Share(roll - midChance, 1 - midChance), low);
        }

        /// <summary>档内小数位:num/den,den ≤ 0(退化配置,该档本不可达)时取 0。</summary>
        private static double Share(double num, double den)
        {
            return den > 0 ? num / den : 0;
        }

        /// <summary>档内取值:lo + (int)(frac × 宽度),宽度 = 含端点项数(hi-lo+1,
        /// 至少 1);结果钳回 [lo, hi] 防浮点越界。</summary>
        private static int Pick(double frac, (int lo, int hi) range)
        {
            int width = range.hi - range.lo + 1;
            if (width < 1) width = 1;
            int v = range.lo + (int)(frac * width);
            if (v < range.lo) return range.lo;
            if (v > range.hi) return range.hi;
            return v;
        }

        /// <summary>
        /// 属性价格曲线(原版 AttrValue):attr ≤ knee1 原值;≤ knee2:
        /// knee1+(attr-knee1)×5;&gt; knee2:knee1+(knee2-knee1)×5+(attr-knee2)×10。
        /// 断点 60/80 由调用点固定传入(原版常量;斜率 5/10 无键)。
        /// </summary>
        public static int AttrPrice(int attr, int knee1, int knee2)
        {
            if (attr <= knee1)
            {
                return attr;
            }
            if (attr <= knee2)
            {
                return knee1 + (attr - knee1) * 5;
            }
            return knee1 + (knee2 - knee1) * 5 + (attr - knee2) * 10;
        }

        /// <summary>
        /// 三属性差量价(R10 属性重掷 postfix 差量修正的单一事实源,remake 缺席时
        /// 消费):Δ = ΣAttrPrice(new) − ΣAttrPrice(old)。恒等性(防掩蔽断言的
        /// 数学依据):AttrPrice 分段斜率 1/5/10 恒正 → 严格递增 → old≠new
        /// (任一属性)⇒ Δ≠0;old 全等 new ⇒ Δ=0。old 三值的读取点必须在写入点
        /// 之前(TagState 同存储,先写后读 = old 恒等 new = 差量恒 no-op,R2
        /// 复审 Critical 教训)。
        /// </summary>
        public static long AttrDelta(int oldForce, int oldIntel, int oldCharm,
            int newForce, int newIntel, int newCharm, int knee1, int knee2)
        {
            return (long)(AttrPrice(newForce, knee1, knee2) + AttrPrice(newIntel, knee1, knee2)
                    + AttrPrice(newCharm, knee1, knee2))
                - (long)(AttrPrice(oldForce, knee1, knee2) + AttrPrice(oldIntel, knee1, knee2)
                    + AttrPrice(oldCharm, knee1, knee2));
        }

        /// <summary>
        /// 阵营基价查表(原版 switch 的字典化):bases 命中返回表值,未命中(或
        /// faction/bases 为 null)返回 fallback——原版 default 分支的等价物,
        /// fallback 由调用方从 CSV 的"其他"项(缺省 10)解析传入。
        /// </summary>
        public static int FactionBase(string faction, IReadOnlyDictionary<string, int> bases, int fallback)
        {
            if (bases != null && faction != null && bases.TryGetValue(faction, out int v))
            {
                return v;
            }
            return fallback;
        }

        /// <summary>
        /// 池写入守卫(Review Focus 4):CSV 非空且切分后至少 1 个非空项才 true;
        /// null/空串/全空白/仅空项 → false(调用方跳过写入,保持原版池)。
        /// </summary>
        public static bool PoolValid(string csv)
        {
            return SplitCsv(csv).Count > 0;
        }

        /// <summary>
        /// 逗号 CSV 切分:逐项 Trim、去空项、保序不去重(池内重复项 = 原版 Pick 的
        /// 按下标均匀语义下即加倍权重,视为子 mod 有意为之);null 视为空串。
        /// </summary>
        internal static IReadOnlyList<string> SplitCsv(string csv)
        {
            var items = new List<string>();
            if (string.IsNullOrEmpty(csv)) return items;
            foreach (string raw in csv.Split(','))
            {
                string item = raw.Trim();
                if (item.Length > 0) items.Add(item);
            }
            return items;
        }

        /// <summary>
        /// "lo,hi" → (lo, hi) 区间解析:恰好两个整数,两侧 Trim,InvariantCulture;
        /// 格式不符(缺项/多项/非整数/空)抛 FormatException——Phase A 读取即失败,
        /// prefix 放行原版,不产出半配置少女。
        /// </summary>
        internal static (int lo, int hi) ParseIntRange(string csv)
        {
            IReadOnlyList<string> parts = SplitCsv(csv);
            if (parts.Count != 2)
            {
                throw new FormatException("属性区间 CSV 应为两项 \"lo,hi\",实得: \"" + csv + "\"");
            }
            return (ParseInt(parts[0]), ParseInt(parts[1]));
        }

        /// <summary>
        /// 面板容错版区间解析(Task 6,更多可配置项):csv 解析失败 → 回退解析
        /// defaultCsv(注册表默认串,与 Cfg() 第二参同源);defaultCsv 也非法
        /// (理论不可达:编译期常量)→ 终极回退 (0,0)。绝不抛——单键降级,
        /// 不连坐整条接管的其他键。usedDefault = 是否发生了回退(调用方日志去重用)。
        /// </summary>
        internal static (int lo, int hi) ParseIntRangeSafe(string csv, string defaultCsv, out bool usedDefault)
        {
            try
            {
                (int lo, int hi) r = ParseIntRange(csv);
                usedDefault = false;
                return r;
            }
            catch (FormatException)
            {
                usedDefault = true;
            }
            try
            {
                return ParseIntRange(defaultCsv);
            }
            catch (FormatException)
            {
                return (0, 0);
            }
        }

        /// <summary>
        /// "名:值,..." 阵营基价解析:"其他"项抽出作 fallback(不占表项),其余进表;
        /// 无"其他"项时 fallback 取参数。项缺":值"、值非整数 → FormatException
        /// (Phase A 放行原版)。名与值两侧 Trim。
        /// </summary>
        internal static (IReadOnlyDictionary<string, int> bases, int fallback) ParseFactionBases(
            string csv, int fallback)
        {
            var bases = new Dictionary<string, int>();
            foreach (string item in SplitCsv(csv))
            {
                int colon = item.IndexOf(':');
                if (colon <= 0)
                {
                    throw new FormatException("阵营基价项应为 \"名:值\",实得: \"" + item + "\"");
                }
                string name = item.Substring(0, colon).Trim();
                int value = ParseInt(item.Substring(colon + 1).Trim());
                if (name == "其他")
                {
                    fallback = value;
                }
                else
                {
                    bases[name] = value;
                }
            }
            return (bases, fallback);
        }

        /// <summary>
        /// 面板容错版阵营基价解析(Task 6,更多可配置项):csv 解析失败 → 回退解析
        /// defaultCsv;defaultCsv 也非法(理论不可达)→ 空表 + fallback 取参数。
        /// 绝不抛——语义同 <see cref="ParseIntRangeSafe"/>。
        /// </summary>
        internal static (IReadOnlyDictionary<string, int> bases, int fallback) ParseFactionBasesSafe(
            string csv, string defaultCsv, int fallback)
        {
            try
            {
                return ParseFactionBases(csv, fallback);
            }
            catch (FormatException)
            {
            }
            try
            {
                return ParseFactionBases(defaultCsv, fallback);
            }
            catch (FormatException)
            {
                return (new Dictionary<string, int>(), fallback);
            }
        }

        /// <summary>
        /// 整数 CSV → int[](progress.exp_per_level 等):逐项 Trim、InvariantCulture,
        /// 非整数项抛 FormatException(调用方 WriteExpNeed catch → 回退保持原版数组,
        /// 整串回退而非跳项——定长语义敏感,丢项会造成升级断裂)。空串 → 空数组
        /// (调用方 PoolValid 先行守卫跳过写入)。
        /// </summary>
        internal static int[] ParseIntList(string csv)
        {
            IReadOnlyList<string> parts = SplitCsv(csv);
            var values = new int[parts.Count];
            for (int i = 0; i < parts.Count; i++)
                values[i] = ParseInt(parts[i]);
            return values;
        }

        private static int ParseInt(string s)
        {
            return int.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
