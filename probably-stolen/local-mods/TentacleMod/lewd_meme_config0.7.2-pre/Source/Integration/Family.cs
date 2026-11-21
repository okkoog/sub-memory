using System;
using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 家族 mod 在场/版本探测(Init 期可调;不可单测:依赖 ModLoader 游戏通道,
    /// 由游戏内验收覆盖)。通道与 MainModResolver 的 ReadMainModVersion /
    /// ReadRemakeActive 完全一致:ModLoader.Mods 是加载器公开只读通道,每次
    /// 返回列表副本,只读遍历安全;manifest 的 ModVersion 由加载器解析时已 Trim。
    /// 两通道时机规则:ModLoader.IsModEnabled 读加载器磁盘 EnabledMods 集合,
    /// 任何 mod Init 前已就绪、Init 期可靠;ModHelper.IsModRunning 读已登记列表,
    /// 装配期才可靠——本探测只用 Mods 列表 + IsModEnabled,Init 期调用合法
    /// (不触碰 ModReflect:多数家族 mod 非前置,Types 越界会抛异常)。
    /// 判定:Present = 列表命中(manifest 非空、ID 精确匹配;未启用条目同样入
    /// 列表「Registered (disabled)」,故必须叠加 IsModEnabled)+ IsModEnabled;
    /// Version = manifest ModVersion(可能为 null,缺席时为 null);
    /// VersionOk = Present 且命中 FamilyMods.BoundVersions。
    /// 纯读取:无缓存、无副作用、不装任何补丁;每次调用重读列表,返回新数组。
    /// </summary>
    public static class Family
    {
        /// <summary>逐一探测 8 个绑定家族 mod,按 ID 常量声明序返回快照数组。</summary>
        public static FamilyState[] Probe()
        {
            List<LoadedMod> mods = ModLoader.Mods; // 每次返回列表副本,只读遍历安全
            // 固定输出顺序(仅影响返回数组排列,不影响判定)。
            string[] order =
            {
                FamilyMods.MainMod, FamilyMods.Brood, FamilyMods.Remake, FamilyMods.Brothel,
                FamilyMods.Tearl, FamilyMods.Legacy, FamilyMods.Crisp, FamilyMods.Iph,
            };
            var states = new FamilyState[order.Length];
            for (int i = 0; i < order.Length; i++)
            {
                string id = order[i];
                LoadedMod entry = mods.Find(m => m != null && m.Manifest != null && m.Manifest.ID == id);
                bool present = entry != null && ModLoader.IsModEnabled(id);
                string version = entry?.Manifest.ModVersion; // 缺席/无 manifest 时为 null
                states[i] = new FamilyState
                {
                    ModId = id,
                    Present = present,
                    Version = version,
                    VersionOk = present && FamilyMods.VersionMatches(id, version),
                };
            }
            return states;
        }

        /// <summary>Probe + 转 modId → 状态字典(Router.DecideAll 与 PanelMarks.Rebuild
        /// 的输入形态;Task 8 接线点)。每次调用重读列表,返回新字典。</summary>
        public static IReadOnlyDictionary<string, FamilyState> ProbeMap()
        {
            FamilyState[] states = Probe();
            var map = new Dictionary<string, FamilyState>(states.Length, StringComparer.Ordinal);
            foreach (FamilyState st in states) map[st.ModId] = st;
            return map;
        }
    }
}
