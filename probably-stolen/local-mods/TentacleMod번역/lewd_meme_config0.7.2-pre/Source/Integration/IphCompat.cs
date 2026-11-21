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
                    throw new MissingMemberException("item_portrait_hd에 AdultBodySources 타입이 노출되지 않음");
                PickBody = ModReflect.StaticMethod<Func<string[], string>>(adult, "PickBody");
                PickBreastForBody = ModReflect.StaticMethod<Func<string[], string, string>>(adult, "PickBreastForBody");
                Active = true;
                log?.Invoke("아이템 초상화 관리자 감지: 소녀 생성 처리를 해당 모드에 양보(공장 후킹 체인이 전 과정을 담당하며 기본/고해상도 초상화로 되돌리지 않음);"
                    + "세력/체형/가슴 크기 추출 풀 3종을 해당 모드에 양보(추출 교체 담당, 분포 조정은 연동 설정의 '아이템 고해상도 초상화 관리자 연동' 참조), 직업 풀 CSV는 정상적으로 기록합니다.");
            }
            catch (Exception ex)
            {
                Active = false;
                PickBody = null;
                PickBreastForBody = null;
                log?.Invoke("아이템 초상화 관리자 호환성 확인 실패, 초상화/체형은 기본 호출 사용:" + ex.Message);
            }
        }
    }
}
