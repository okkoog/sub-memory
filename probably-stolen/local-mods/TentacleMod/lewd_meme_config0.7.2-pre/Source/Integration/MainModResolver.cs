using System;
using System.Collections.Generic;
using UnityEngine;

namespace LewdMemeConfig
{
    /// <summary>
    /// 主 mod 解析结果:MainModSupported = 主 mod 在场且版本受支持(0.6.0 版本
    /// 严格绑定;钩子装配的版本压制查询已改走 Family 家族快照——R-B 接线,本字段
    /// 余下职责 = DataWrite 装配期写入守卫与 Changed 转发短路),MainModVersion =
    /// 前置 manifest 的 ModVersion(可能为 null),Types = ModReflect.Types
    /// ("lewd_meme") 的全名索引(Absent 时空字典,面板动态层守卫用)。
    /// RemakeActive = lewd_meme_remake 启用在场(0.7.0 T4:让位裁决已归 Router
    /// [1.6.1 翻转后仅 on_loot_rolled/price_ovulation_shot 两单元],本字段
    /// 余下职责 = ModEntry 的让位总览日志)。
    /// </summary>
    public sealed class MainModResolveResult
    {
        internal bool MainModSupported;            // 在场且版本受支持(DataWrite 写入守卫与 Changed 转发短路;版本压制查询走 Family 快照)
        public string MainModVersion;             // 可能为 null
        public bool RemakeActive;                 // lewd_meme_remake 启用在场(探测通道见 ReadRemakeActive)
        public IReadOnlyDictionary<string, System.Type> Types;  // ModReflect.Types("lewd_meme") 结果；Absent 时空字典
    }

    /// <summary>
    /// 运行期探测主 mod lewd_meme 的状态与版本(不可单测:依赖 ModLoader /
    /// ModReflect,仅本 mod 被加载器登记后才允许调用——ModEntry 已推迟到
    /// 物品目录初始化之后)。
    /// 版本来源:ModHelper 未暴露前置版本读取,官方文档(ModdingGuide「Other
    /// textures」节)给出的是 ModLoader.Mods.Find(m => m.Manifest.ID == ...)
    /// 这条公开只读通道,manifest 的 ModVersion 由加载器解析时已 Trim;
    /// 该路径无 [ModRestricted],不涉及信任层,零能力即可使用。
    /// 版本拿不到(manifest ModVersion null/空)→ MainModSupported=false(F2,
    /// 0.6.0 严格绑定:版本无法验证 = 不受支持,与 FamilyMods.VersionMatches
    /// 的 null→不匹配同语义,DataWrite 与 Router 两层裁定一致;0.4 时代的
    /// 「保守 Full」已被 spec §4.6 严格绑定取代);ModReflect 抛
    /// ModReflectAccessException 则为主 mod 缺席(MainModSupported=false)。
    /// </summary>
    public static class MainModResolver
    {
        /// <summary>主 mod ID(manifest &lt;Prerequisites&gt; 同名声明)。</summary>
        public const string MainModId = "lewd_meme";

        /// <summary>remake mod ID(非前置,仅探测在场与否,不访问其类型)。</summary>
        public const string RemakeModId = "lewd_meme_remake";

        /// <summary>可选日志出口(ModEntry 接 ModLog);未接线时退回 UnityEngine.Debug.Log。</summary>
        internal static Action<string> Logger { get; set; }

        public static MainModResolveResult Resolve()
        {
            // remake 探测不走 ModReflect(见 ReadRemakeActive),与主 mod 探测互不依赖,先读一次三处共用。
            bool remakeActive = ReadRemakeActive();
            List<System.Type> exposed;
            try
            {
                // 前置已声明才可达;主 mod 未装/未启用/未在运行 → ModReflectAccessException。
                exposed = new List<System.Type>(ModReflect.Types(MainModId));
            }
            catch (ModReflectAccessException ex)
            {
                Log("主 mod " + MainModId + " 不可用,判定缺席(MainModSupported=false):" + ex.Message);
                return new MainModResolveResult
                {
                    MainModSupported = false,
                    MainModVersion = ReadMainModVersion(),
                    RemakeActive = remakeActive,
                    Types = new Dictionary<string, System.Type>(),
                };
            }

            var types = new Dictionary<string, System.Type>(exposed.Count, StringComparer.Ordinal);
            foreach (System.Type t in exposed)
            {
                types[t.FullName ?? t.Name] = t;
            }

            string version = ReadMainModVersion();
            if (string.IsNullOrEmpty(version))
            {
                // 裁定(F2,0.6.0 严格绑定):版本字符串拿不到 = 绑定无法验证 →
                // 不受支持(与 Family/FamilyMods 的 null→VersionOk=false 同语义,
                // DataWrite 与 Router 两层一致;0.4 时代「保守 Full」已废止)。
                Log("无法读取主 mod " + MainModId + " 的版本(manifest ModVersion 为空),严格判定不受支持(版本绑定无法验证)。");
                return new MainModResolveResult
                {
                    MainModSupported = false,
                    MainModVersion = null,
                    RemakeActive = remakeActive,
                    Types = types,
                };
            }

            bool supported = VersionGuard.IsSupported(version);
            Log("主 mod " + MainModId + " v" + version + (supported ? " 受支持。" : " 不在受支持版本(" + VersionGuard.SupportedMainModVersion + "),主 mod 家族钩子全部压制(VersionBlocked,不写不装)。")
                + (remakeActive ? " 检测到 " + RemakeModId + " 启用在场,夜间数值组将让位。" : string.Empty));
            return new MainModResolveResult
            {
                MainModSupported = supported,
                MainModVersion = version,
                RemakeActive = remakeActive,
                Types = types,
            };
        }

        private static string ReadMainModVersion()
        {
            List<LoadedMod> mods = ModLoader.Mods; // 每次返回列表副本,只读遍历安全
            LoadedMod main = mods.Find(m => m != null && m.Manifest != null && m.Manifest.ID == MainModId);
            return main?.Manifest.ModVersion;
        }

        /// <summary>
        /// 探测 remake(lewd_meme_remake)是否启用在场。通道与 ReadMainModVersion 相同:
        /// ModLoader.Mods / ModLoader.IsModEnabled 是加载器的公开只读通道(ModdingGuide
        /// 「Reflection · Hard refusals」节:读取 ModLoader.Mods、IsModEnabled 免费,Mods 返回副本),
        /// 不涉及前置关系——本 mod 未声明 remake 为前置,ModReflect.Types(remake) 会抛
        /// ModReflectAccessException,故不能用。启用状态以 ModLoader.IsModEnabled(加载器
        /// 内部 EnabledMods 记账)为准,不读 LoadedMod.IsEnabled 公开字段(可被他 mod 篡改;
        /// 反编译 ModLoader:未启用条目同样入列表("Registered (disabled)"),在场 ≠ 启用)。
        /// 判定 = 在场(Find 命中,manifest 非空)+ 启用(IsModEnabled);任一不满足返回 false。
        /// </summary>
        private static bool ReadRemakeActive()
        {
            List<LoadedMod> mods = ModLoader.Mods; // 副本,只读遍历安全
            LoadedMod remake = mods.Find(m => m != null && m.Manifest != null && m.Manifest.ID == RemakeModId);
            return remake != null && ModLoader.IsModEnabled(RemakeModId);
        }

        private static void Log(string message)
        {
            Action<string> logger = Logger;
            if (logger != null)
            {
                logger(message);
                return;
            }
            Debug.Log("[lewd_meme_config] " + message);
        }
    }
}
