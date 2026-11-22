namespace LewdMemeConfig
{
    /// <summary>
    /// patch 层与配置的接线点:ModEntry 在 HookRuntime 装配之前把全局
    /// ConfigRegistry 注入这里,各 patch 经它读取实时配置(含子 mod
    /// override,档位切换即时生效)。ConfigRegistry 尚未注入(或 mod 停用后
    /// 置空)时,读取方以 ?. 兜底为默认档 0 = 全部放行原版行为。
    /// </summary>
    public static class PatchCtx
    {
        public static ConfigRegistry Config;
    }
}
