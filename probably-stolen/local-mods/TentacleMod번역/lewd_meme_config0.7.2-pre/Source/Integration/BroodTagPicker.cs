using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace LewdMemeConfig
{
    /// <summary>
    /// 巢穴扩展·苗床标签拾取器(0.7.1,2026-11-18;目录单元
    /// <c>brood.tag_picker</c>,Kind = SubModHook 但无 Harmony patch——宿主
    /// 生命周期单元,装配/卸载全部复用安装器状态机,见 HookCatalog 该行注释)。
    /// 背景:Brood 0.3.31 自带「编辑选中苗床的标签」调试功能
    /// (<c>MaidenTagEditor</c>,Home TestPanel / F9 Modding 分类),但「选中」
    /// 依赖红灯区的立绘放大镜检视链,而该链断裂:红灯区非家族前置(大量玩家
    /// 未装,无放大镜可用)、主 mod 苗床区拒绝 magnifier 交互。本单元只替换
    /// 「选中」一步——悬停苗床 + 快捷键 → 直调 Brood
    /// <c>MaidenTagEditor.Remember(item)</c>(internal static,brood_0331
    /// MaidenTagEditor.cs:26-34,经 ModReflect.CallStatic 带 Type[] 重载防
    /// 歧义;Brood 前置已声明,访问合法);其余全走 Brood(玩家照旧开 Home
    /// 面板点「编辑苗床标签」,编辑逻辑/标签语义/审计零改动)。
    /// 生命周期:开关(brood.tag_picker_enabled,Toggle 自开关)开 → install
    /// 委托 <see cref="EnsureResolved"/>(Brood 类型装配期一次解析)+ 
    /// <see cref="HostOn"/>(建宿主 GameObject);关/压制/失败 →
    /// <see cref="OnUnitGone"/>(HookRuntime.Report 迁移回调)+ Shutdown
    /// 显式销毁(UninstallAll 静默清场不走 Report)。默认关 = 零 GameObject
    /// 零轮询(按需装配红线);不依赖 DebugPanel.Enabled(发布版可用)。
    /// 悬停拾取:Update 轮询 Input.GetKeyUp(配置键,热读)→
    /// <c>RenderHandler.RaycastElement&lt;GameItem&gt;(Input.mousePosition)</c>
    /// (public static,game/RenderHandler.cs:491;零 Harmony,与 tooltip 同源
    /// 覆盖面——苗床都在格子)→ 四守卫(打字屏蔽 + 游戏三守卫,先例逐处:
    /// StoreUIManager.IsAnyTyping() 打字跳[public static,game/
    /// StoreUIManager.cs:1476;InputActionManager.cs:108 统一屏蔽层同款,
    /// 防文本框焦点与光标分离时误拾取]/ StoreUIManager.hoveringStoreUI
    /// 悬 UI 跳[game/HopperHelper.cs:991] / ItemMouseDragHandler.current.
    /// IsDraggingItem 拖物品跳[game/ItemMouseDragHandler.cs:187] /
    /// UIDragHandler.isDraggingAnyUI 拖窗跳[game/StoreUIManager.cs:289])→
    /// <c>item.IsTag("MAIDEN_FEMALE")</c>
    /// (BaseLink.IsMaiden 同语义,brood_0331 BaseLink.cs:311-317)过滤 →
    /// Remember + Note.Show 提示(Brood DebugHelp 弹 Note 先例,DebugHelp.cs:17;
    /// Note.Show(string,string,float,float) 四参全填——ModReflect.ParametersMatch
    /// 按参数计数精确匹配,默认参不折算)。
    /// 游戏类型(StoreUIManager/ItemMouseDragHandler/UIDragHandler/
    /// RenderHandler/GameItem)直接编译引用;Brood 类型(MaidenTagEditor/Note)
    /// 一律运行时经 ModReflect.Types(FamilyMods.Brood) 按名解析(BroodPatches
    /// 先例;两阶段提交,任一失败原样上抛 → 单元 Dead,开关 off→on 可重试)。
    /// </summary>
    internal static class BroodTagPicker
    {
        /// <summary>目录单元 id(与 HookCatalog 的 Def 行同名;HookRuntime.Report
        /// 的宿主销毁特判按此匹配)。</summary>
        internal const string UnitId = "brood.tag_picker";

        private const string HostName = "LewdMemeConfig_BroodTagPicker";
        internal const KeyCode DefaultKey = KeyCode.U; // 全家族+游戏绑定零冲突(勘察键位表);PanelConfig.ParseKeyCodeName 回退引用(默认值单源)
        private const string MaidenTag = "MAIDEN_FEMALE"; // BaseLink.IsMaiden 同语义

        // -- Brood 类型的运行时缓存(install 期经 ModReflect 填充,Update 只做直接调用)--
        private static bool resolved;
        private static System.Type maidenTagEditor;   // BroodMainCycle.MaidenTagEditor(Remember 宿主)
        private static System.Type note;              // BroodMainCycle.Note(Show 提示窗)
        private static Assembly broodAssembly;        // 防串锚(全部解析类型必须同属此程序集)

        private static GameObject host;

        /// <summary>把拾取器单元的装配委托登记进映射(catalog-id → install)。
        /// 无补丁:委托体 = 解析 + 建宿主,返回空 MethodBase[](空记账合法——
        /// HookInstaller.Uninstall 对空数组遍历 no-op,物理卸载动作不存在;
        /// 宿主销毁经 HookRuntime.Report 的状态迁移回调与 Shutdown)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add(UnitId, h =>
            {
                EnsureResolved();
                HostOn();
                return new MethodBase[0];
            });
        }

        /// <summary>宿主随单元离场销毁(HookRuntime.Report 的 Absent/VersionBlocked/
        /// Dead 迁移回调;非本单元 id 恒 no-op)。幂等:宿主不在时静默。</summary>
        internal static void OnUnitGone(string unitId)
        {
            if (unitId != UnitId) return;
            HostOff();
        }

        /// <summary>终局撤装入口(HookRuntime.Shutdown 显式调用——UninstallAll
        /// 静默清场不走 Report 回调,宿主须在此兜底销毁)。幂等。</summary>
        internal static void HostOff()
        {
            if (host == null) return;
            try
            {
                UnityEngine.Object.Destroy(host);
            }
            catch (Exception ex)
            {
                LogWarning("호스트 객체 제거 실패(동작하지 않는 객체가 하나 남으며 재시작하면 자동 정리됨): " + ex.Message);
            }
            finally
            {
                host = null; // Destroy 帧末生效前也不再轮询:宿主 Update 首行判 host
            }
        }

        /// <summary>建宿主(幂等:已在则不重建;DebugPanel.Activate 同形态)。
        /// DontDestroyOnLoad:进存档后建、场景切换存活(游戏单场景经营,防御性)。</summary>
        private static void HostOn()
        {
            if (host != null) return;
            host = new GameObject(HostName);
            UnityEngine.Object.DontDestroyOnLoad(host);
            host.AddComponent<PickerHost>();
            Log("선택 도구 설치 완료: 숙주에 커서를 올리고 [" + CurrentKey() + "] 키로 선택(둥지 확장 Home 패널의 『숙주 태그 편집』에서 사용)");
        }

        /// <summary>
        /// 幂等解析 Brood 成员(两阶段提交,同 BroodPatches/TearlPatches:任一
        /// 成员解析抛出 → 原样上抛不提交任何字段,单元记 Dead)。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type editor = FindBroodType("MaidenTagEditor");
            System.Type noteType = FindBroodType("Note");
            maidenTagEditor = editor;
            note = noteType;
            resolved = true;
        }

        /// <summary>在前置 claude.lewdMemeBrood 暴露的类型里按名查找。防串断言:
        /// 解析类型锚定单一程序集(BroodPatches.FindBroodType 先例;自持一份,
        /// 不依赖其 install 期缓存)——加载器行为漂移时装配期即炸(记 Dead),
        /// 绝不调错族方法。</summary>
        private static System.Type FindBroodType(string name)
        {
            foreach (System.Type t in ModReflect.Types(FamilyMods.Brood))
                if (t.Name == name)
                {
                    if (broodAssembly == null) broodAssembly = t.Assembly;
                    else if (broodAssembly != t.Assembly)
                        throw new MissingMemberException(
                            "Brood 타입이 다른 어셈블리에 속함(다른 모드의 동명 타입과 혼동했을 가능성): " + name);
                    return t;
                }
            throw new MissingMemberException("선행 모드 claude.lewdMemeBrood가 다음 타입을 제공하지 않음: " + name);
        }

        // ------------------------------------------------------------------
        // 配置读取(热读——改键即时生效)
        // ------------------------------------------------------------------

        /// <summary>拾取快捷键( KeyCode 名串 → 枚举;解析失败回退默认 U——
        /// json 手改坏值/旧档残留不致命)。</summary>
        private static KeyCode CurrentKey()
        {
            try
            {
                string name = PatchCtx.Config?.Current<string>(CfgKeys.BroodTagPickerKey);
                if (!string.IsNullOrEmpty(name)
                    && Enum.TryParse(name, out KeyCode kc)
                    && Enum.IsDefined(typeof(KeyCode), kc))
                    return kc;
            }
            catch (Exception ex)
            {
                LogWarning("선택 단축키 설정 읽기 실패. 이번 프레임은 기본값 U로 복귀: " + ex.Message);
            }
            return DefaultKey;
        }

        // ------------------------------------------------------------------
        // 宿主(Brood RoutineHost / DebugPanel.Host 先例:自建 GameObject + MonoBehaviour)
        // ------------------------------------------------------------------

        private sealed class PickerHost : MonoBehaviour
        {
            private void Update()
            {
                try
                {
                    if (host == null) return; // HostOff 已排队 Destroy:停止轮询
                    if (!Input.GetKeyUp(CurrentKey())) return;
                    Pick();
                }
                catch (Exception ex)
                {
                    // 吞掉记日志(DebugPanel.Update 先例):单帧异常不废宿主
                    LogWarning("선택 입력 확인 중 예외 발생(이번 프레임 무시): " + ex.Message);
                }
            }
        }

        /// <summary>单次拾取:守卫序首行 = 打字屏蔽(游戏快捷键统一屏蔽层
        /// InputActionManager.cs:108 同款:StoreUIManager.IsAnyTyping()),
        /// 其后三守卫——UI 悬停 / 拖物品 / 拖窗任一命中即放弃本帧。三守卫
        /// 先例逐处(非同函数):hoveringStoreUI = game/HopperHelper.cs:991、
        /// IsDraggingItem = game/ItemMouseDragHandler.cs:187、isDraggingAnyUI
        /// = game/StoreUIManager.cs:289。</summary>
        private static void Pick()
        {
            // 打字屏蔽守卫(首行;public static,game/StoreUIManager.cs:1476):
            // 文本框焦点与光标分离时按 U 不误拾取——与游戏快捷键体系一致
            if (StoreUIManager.IsAnyTyping()) return;
            // 三守卫(先例同款;可访问性以 game 基线实测:StoreUIManager.Instance
            // public static、hoveringStoreUI public 字段、ItemMouseDragHandler.current
            // public static、IsDraggingItem public 属性、UIDragHandler.isDraggingAnyUI
            // public static bool)
            if (StoreUIManager.Instance != null && StoreUIManager.Instance.hoveringStoreUI) return;
            if (ItemMouseDragHandler.current != null && ItemMouseDragHandler.current.IsDraggingItem) return;
            if (UIDragHandler.isDraggingAnyUI) return;

            // 悬停拾取(public static,零 Harmony;覆盖面 = UI 层物品,与 tooltip 同源)
            GameItem item = RenderHandler.RaycastElement<GameItem>(Input.mousePosition);
            if (item == null) return; // 未悬停任何物品:静默(避免空白处按键被弹窗骚扰)

            if (!item.IsTag(MaidenTag))
            {
                NoteShow("이 대상은 숙주가 아닙니다");
                return;
            }

            // 直调 Brood Remember(internal static void Remember(GameItem item),
            // 唯一签名;带 Type[] 重载防歧义)。编辑窗开着时 Brood 自身会 Close
            // 切换(原生行为,零干预)
            ModReflect.CallStatic(maidenTagEditor, "Remember",
                new[] { typeof(GameItem) }, item);
            NoteShow("선택됨: " + item.identifierName + " — Brood 패널(Home)에서 『숙주 태그 편집』 선택");
        }

        /// <summary>经 ModReflect 调 Brood Note.Show(brood_0331 Note.cs:10
        /// internal static void Show(string text, string title = null, float
        /// width = 0f, float height = 0f)——ParametersMatch 按计数精确匹配,
        /// 默认参必须显式补全)。</summary>
        private static void NoteShow(string text)
        {
            ModReflect.CallStatic(note, "Show",
                new[] { typeof(string), typeof(string), typeof(float), typeof(float) },
                text, null, 0f, 0f);
        }

        // ------------------------------------------------------------------
        // 共用小件(独立持有,不依赖其他 patch 类;BroodPatches 同例)
        // ------------------------------------------------------------------

        private static void Log(object msg)
        {
            UnityEngine.Debug.Log("[lewd_meme_config→둥지 확장] " + msg);
        }

        private static void LogWarning(object msg)
        {
            UnityEngine.Debug.LogWarning("[lewd_meme_config→둥지 확장] " + msg);
        }
    }
}
