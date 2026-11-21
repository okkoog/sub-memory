using System;
using LewdMemeConfig;

namespace LewdMemeConfig
{
    /// <summary>
    /// ItemPortraitHD(物品立绘管理器)调用链兼容(0.5.0 Task 6,spec §4.1 B 方案)。
    /// 它对 ItemFactory.CreateMaiden* 的 transpiler 只存在于原方法体——本 mod 的
    /// Takeover prefix(return false)短路原方法即短路其替换逻辑,立绘回退像素。
    /// 兼容方式:它在场时,复刻体的体型/乳量抽取改调其 AdultBodySources 替换点
    /// (签名 (string[] original) → string / (string[], string body) → string;
    /// original = 主 mod 池数组,本 mod 池写经参数继续生效);立绘注入改走
    /// SpriteInjector.InjectPortraitTrimmed(其方法级 InjectPrefix 对任何调用方
    /// 生效——高清发布)。
    /// 前置声明(item_portrait_hd)使 ModReflect.Types 访问其 internal 类型合法;
    /// 前置缺席仅 warning(官方语义),本 mod 照常运行(Active=false 走原版路径)。
    /// 解析失败(签名漂移)→ Active=false + 日志一次,绝不抛、绝不阻断装配(A 兜底)。
    /// </summary>
    internal static class IphCompat
    {
        internal static bool Active;
        internal static Func<string[], string> PickBody;               // AdultBodySources.PickBody
        internal static Func<string[], string, string> PickBreastForBody; // AdultBodySources.PickBreastForBody

        /// <summary>装配期一次(ModEntry.OnDirectoriesReady,让位检查后、BuildHub 前)。</summary>
        internal static void Resolve(Action<string> log)
        {
            Active = false;
            PickBody = null;
            PickBreastForBody = null;
            try
            {
                if (!ModHelper.IsModRunning("item_portrait_hd")) return;
                System.Type adult = null;
                foreach (var t in ModReflect.Types("item_portrait_hd"))
                    if (t.Name == "AdultBodySources") { adult = t; break; }
                if (adult == null)
                    throw new MissingMemberException("item_portrait_hd 未暴露类型 AdultBodySources");
                PickBody = ModReflect.StaticMethod<Func<string[], string>>(adult, "PickBody");
                PickBreastForBody = ModReflect.StaticMethod<Func<string[], string, string>>(adult, "PickBreastForBody");
                Active = true;
                log?.Invoke("物品立绘管理器在场:少女生成接管让位(其工厂挂钩链全程接管,原版+高清立绘不回退);"
                    + "阵营/体型/胸部三池让位(其抽取替换接管,分布调参见联动分区「物品高清立绘管理器联动」),职业池 CSV 照常写入。");
            }
            catch (Exception ex)
            {
                Active = false;
                PickBody = null;
                PickBreastForBody = null;
                log?.Invoke("物品立绘管理器兼容解析失败,立绘/体型走原版调用:" + ex.Message);
            }
        }
    }
}
