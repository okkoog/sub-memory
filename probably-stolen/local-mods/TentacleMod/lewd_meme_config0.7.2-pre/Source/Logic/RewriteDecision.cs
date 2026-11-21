namespace LewdMemeConfig
{
    /// <summary>
    /// 性别改写 4 档的纯判定逻辑(Task 6):零游戏类型依赖,供测试工程通配链接与
    /// GenderRatioPatch 复用。档位语义(spec §6,client.gender_scope):
    /// 0 = 原版(chance 概率改写普通顾客,默认 0.75);
    /// 1 = 普通顾客 100% 改写(保留 SpecialClients.IsAny 排除);
    /// 2 = 连特殊顾客一起改写;
    /// 3 = 全女性(本判定恒 false,由 Task 5 游戏本体两点接管)。
    /// </summary>
    public static class RewriteDecision
    {
        /// <summary>gender_scope 越界收敛到 [0,3](Review Focus 3:&lt;0 → 0,&gt;3 → 3,界内原样)。</summary>
        public static int ClampScope(int scope)
        {
            if (scope < 0) return 0;
            if (scope > 3) return 3;
            return scope;
        }

        /// <summary>
        /// 是否执行性别改写(男主 → 女主立绘)。真值表(brief 逐字):
        /// scope 0: !isSpecial &amp;&amp; roll &lt; chance;
        /// scope 1: !isSpecial;
        /// scope 2: true;
        /// scope 3: false。
        /// roll 为 [0,1) 随机数(调用方传 UnityEngine.Random.value);scope 1/2 恒改写,
        /// roll 与 chance 不参与判定。
        /// </summary>
        public static bool ShouldRewrite(int scope, bool isSpecial, float chance, double roll /*[0,1)*/)
        {
            switch (scope)
            {
                case 0: return !isSpecial && roll < chance;
                case 1: return !isSpecial;
                case 2: return true;
                default: return false; // scope 3:不干预
            }
        }
    }
}
