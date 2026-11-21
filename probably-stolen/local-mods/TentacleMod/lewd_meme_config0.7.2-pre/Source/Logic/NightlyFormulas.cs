using System;

namespace LewdMemeConfig
{
    /// <summary>
    /// 夜间结算纯公式(Task 7)。语义逐条对应反编译 v1.3.8
    /// <c>MidnightPawn.Game.TentacleBody.NightlyTick</c> 的硬编码字面量(清单 A):
    /// 系数本身由 CfgKeys(nightly.*)注入,本类只负责形状;全默认时与原版逐项一致。
    /// 零游戏类型(Logic/ 通配进测试工程),概率与区间均为纯 System 数学。
    /// </summary>
    public static class NightlyFormulas
    {
        /// <summary>
        /// 每晚行为次数区间:min = minBase + (level-1)×perMin,max 同理;
        /// max&lt;min 时 max=min(防配置写反导致 Random.Range(min, max+1) 空区间)。
        /// 原版:性交 [5+(L-1), 10+(L-1)×3];口交/肛交 [L-1, 2+(L-1)×2]。
        /// </summary>
        public static (int min, int max) ActRange(int minBase, int perMin, int maxBase, int perMax, int level)
        {
            int min = minBase + (level - 1) * perMin;
            int max = maxBase + (level - 1) * perMax;
            return max < min ? (min, min) : (min, max);
        }

        /// <summary>
        /// 受孕概率:min(cap, sexCount×perSex),再 clamp 到 [0,1]。
        /// 原版:Mathf.Clamp01(当晚性交次数 / 100f),即 perSex=0.01、cap=1。
        /// </summary>
        public static float ConceiveChance(int sexCount, float perSex, float cap)
        {
            float v = Math.Min(cap, sexCount * perSex);
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }

        /// <summary>乳汁单价:valueBase + 泌乳等级×perLv。原版:100 + lv×40。</summary>
        public static int MilkValue(int valueBase, int perLv, int lactation)
        {
            return valueBase + perLv * lactation;
        }

        /// <summary>
        /// 触手精液产出概率:min(cap, chanceBase + trainValue×perTrain)。
        /// 原版:Mathf.Min(1f, 0.4f + 调教值/10f×0.1f),即 base=0.4、perTrain=0.01、cap=1。
        /// </summary>
        public static float SemenChance(float chanceBase, float perTrain, int trainValue, float cap)
        {
            return Math.Min(cap, chanceBase + trainValue * perTrain);
        }

        /// <summary>泌乳等级增量后取整:min(cap, current+gain)。原版:Mathf.Min(3, lv+1)。</summary>
        public static int LactationAfterGain(int current, int gain, int cap)
        {
            return Math.Min(cap, current + gain);
        }

        /// <summary>方形网格尺寸:(edge, edge)。供 layout.brood_size / layout.output_size 取边。</summary>
        public static (int w, int h) GridSize(int edge)
        {
            return (edge, edge);
        }
    }
}
