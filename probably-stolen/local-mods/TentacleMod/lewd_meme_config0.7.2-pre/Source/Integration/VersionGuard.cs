namespace LewdMemeConfig
{
    /// <summary>
    /// 主 mod(lewd_meme)版本守卫:纯逻辑、零游戏类型引用,
    /// 因而被显式链接进测试工程(Integration/ 默认不进测试通配)。
    /// 语义:null→false;精确等于受支持版本→true;其余(含更高版本)→false。
    /// </summary>
    public static class VersionGuard
    {
        /// <summary>本 API 适配并验证过的主 mod 版本(manifest 的 ModVersion)。
        /// 0.7.0 Task 1(2026-11-17):1.7.1 → 1.9.2(0.7.0 适配波次目标基线,
        /// 反编译基线 lewd_meme_192;lewd_meme_171 存档)。
        /// Task 14b(2026-11-13):1.6.1 → 1.7.1(1.6.1 支持随基线切换弃置,
        /// 反编译基线 lewd_meme_161 存档、现行 = lewd_meme_171)。</summary>
        public const string SupportedMainModVersion = "1.9.2";

        /// <summary>版本字符串是否受支持(精确匹配,不做语义化比较:补丁差异即视为不符)。</summary>
        public static bool IsSupported(string mainModVersion)
        {
            return mainModVersion == SupportedMainModVersion;
        }
    }
}
