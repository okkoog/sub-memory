using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LewdMemeConfig
{
    /// <summary>
    /// 游戏内调试面板(Task 14,R-S/R-T;Brood Runner 宿主 + IPH OnGUI/GUI.Window
    /// 先例)。发布开关 <see cref="Enabled"/> = false:Activate 直接 return——
    /// 零 GameObject、零 F9 注册、零事件订阅,面板代码完全惰性(编译但不可达);
    /// 开发验收时改 true 重编译即可。启用时:
    ///  - 宿主:自建 GameObject "LewdMemeConfig_DebugPanel"(DontDestroyOnLoad,
    ///    OnDestroy 清理静态态;协程随宿主销毁自动停);
    ///  - 热键:Update 轮询 GetKeyDown(LeftBracket「[」)切换窗口;
    ///  - F9:ModHelper.AddDebugFunction(ModEntry.Id, "Special", "디버그 패널 ([ 키) / 빠른 테스트 (] 키)")。
    /// 三区(R-T):①状态总览(家族 8 行 + 单元表[计数随 HookCatalog 动态],期望≠实际行标红)
    /// ②自检(三级 + 全量按钮;协程逐批执行、进度行、完成摘要 + 失败明细滚动,
    /// 每条结果同步 Player.log 一行)③写透快照(每镜像键快照持有/对方当前值)。
    /// F4(验收 R1):三区各自独立 try/catch——单区渲染异常显示「本区渲染失败」
    /// 行,不打断 GUILayout 布局,其余区照常(0x01.log:期望态求值每帧抛
    /// InvalidCastException 曾令窗口只剩边框无按钮);期望态/拨开的开关语义一律
    /// 经 HookRuntime.SwitchOnFor(SwitchDispatch 同源),零平行复刻。
    /// R12:第二个窗口「快捷测试」(<see cref="QuickTestKey"/>「]」呼出,与「[」
    /// 面板独立开关、共存可同屏,共享同一 Host GameObject/协程宿主;参考 Brood
    /// Home 键调试菜单的「一键动作」形态)。三区(每区独立 try/catch 同 F4):
    /// ①常用物品生成(一键替代 F9 逐个找;物品 id 复用 SelfTestRunner.SpawnItem
    /// 清单 = 单一事实源,点击 = SpawnItem + MoveItemToInventory 到背包 + 日志);
    /// ②标准测试组(一键跑组,编排层零新逻辑——组员 = SelfTestPlan 目录行,
    /// 执行 = SelfTestRunner 既有执行器;进度行 + 结果行「N PASS / M FAIL /
    /// K SKIP + 失败键列表」+ 失败明细滚动,日志同步
    /// [lewd_meme_config][quicktest];R13b 五组 → 七组:+legacy 组[LegacyOverlay 七行,
    /// legacy 缺席整组灰显]+ brothel 组[掌上青楼分区 InvokeStatic 21 行,brothel 缺席
    /// 整组灰显],行级执行器 SKIP 双保险;0.7.0 T11 七组 → 八组:+0.7.0 组[新键
    /// 五行:exp 公式三键/interact_threshold[invoke 直测]+ initial_virgin[spawn
    /// 生成断言],组员 = SelfTestPlan 新升直测行]);
    /// ③常用键快捷开关(高频调试键 Toggle 直改,写注册表玩家层 + 拨对应
    /// .enabled——经 registry.Changed → HookRuntime.OnSwitchChanged 同管道,
    /// 不绕过分发器;复原 = 先值后开关,SelfTestRunner 同序;0.7.0 T11
    /// +经验公式 base/互动阈值/淫力上限三键,六键分两行)。
    /// 纯只读渲染 + 自检的「临时写并复原」(SelfTestRunner 契约)——不改变任何
    /// 装配/路由语义。IMGUI/GameObject/Input 均为游戏合法通道,零能力声明。
    /// </summary>
    internal static class DebugPanel
    {
        /// <summary>发布开关:false = 完全惰性(评审红线:零订阅、零 GameObject、零 F9)。</summary>
        internal const bool Enabled = true; // TODO(发布前): 验收结束后改回 false——发布版调试面板恒关

        /// <summary>呼出热键:「[」(R-S 裁定)。</summary>
        internal const KeyCode ToggleKey = KeyCode.LeftBracket;

        /// <summary>快捷测试窗口呼出热键:「]」(R12;与「[」面板独立开关、共存可同屏)。</summary>
        internal const UnityEngine.KeyCode QuickTestKey = UnityEngine.KeyCode.RightBracket;

        private const string HostName = "LewdMemeConfig_DebugPanel";
        /// <summary>GUI.Window 唯一 id(0x4C4D43 = "LMC";IPH 728194 同例自选)。</summary>
        private const int WindowId = 0x4C4D43;
        /// <summary>快捷测试窗口 id(主面板 +1,自定 id 常量与 WindowId 同例)。</summary>
        private const int QuickWindowId = 0x4C4D44;

        internal static bool Visible;

        /// <summary>快捷测试窗口可见态(与 <see cref="Visible"/> 独立开关)。</summary>
        internal static bool QuickVisible;

        private static GameObject host;
        private static Host component;
        private static Rect window = new Rect(48f, 36f, 660f, 720f);
        private static Vector2 overviewScroll;
        private static Vector2 reportScroll;
        private static Vector2 mirrorScroll;

        // 快捷测试窗口运行态(②区:协程写、OnGUI 读;③区:Toggle 记账)
        private static Rect quickWindow = new Rect(724f, 36f, 560f, 720f);
        private static Vector2 quickScroll;
        private static bool quickRunning;
        private static string quickProgress = "";
        private static string quickGroupLabel = "";
        private static TestReport quickReport;

        // 自检运行态(协程写、OnGUI 读)
        private static bool running;
        private static string progress = "";
        private static TestReport report;

        // IMGUI 样式(OnGUI 内惰性建;IPH 先例:系统中文字体,防默认字体缺字)
        private static Font font;
        private static GUIStyle label;
        private static GUIStyle button;
        private static GUIStyle header;

        /// <summary>
        /// 启用入口(ModEntry.Init 调一行;Enabled=false 直接 return,零副作用)。
        /// 幂等:宿主已在则不重建。CS0162 pragma:发布开关是编译期常量,
        /// false 时方法体不可达即「完全惰性」的编译器证明——这正是设计意图。
        /// </summary>
#pragma warning disable CS0162
        public static void Activate()
        {
            if (!Enabled) return;
            if (component != null) return;
            try
            {
                host = new GameObject(HostName);
                UnityEngine.Object.DontDestroyOnLoad(host);
                component = host.AddComponent<Host>();
                ModHelper.AddDebugFunction(ModEntry.Id, "Special", "디버그 패널 ([ 키) / 빠른 테스트 (] 키)",
                    (Action)Toggle, false);
                UnityEngine.Debug.Log("[lewd_meme_config] 디버그 패널 활성화:[" + ToggleKey
                    + "] 3개 구역 패널 / [" + QuickTestKey + "] 빠른 테스트. F9 Special로도 열 수 있습니다.");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("[lewd_meme_config] 디버그 패널 호스트 생성 실패: " + ex.Message);
            }
        }
#pragma warning restore CS0162

        /// <summary>F9 回调与热键共用的开合切换。</summary>
        internal static void Toggle()
        {
            Visible = !Visible;
        }

        // ------------------------------------------------------------------
        // 宿主(Brood RoutineHost 先例:自建 GameObject + MonoBehaviour)
        // ------------------------------------------------------------------

        private sealed class Host : MonoBehaviour
        {
            private void Update()
            {
                try
                {
                    if (Input.GetKeyDown(ToggleKey)) Visible = !Visible;
                    if (Input.GetKeyDown(QuickTestKey)) QuickVisible = !QuickVisible;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.Log("[lewd_meme_config] 디버그 패널 단축키 확인 실패: " + ex.Message);
                }
            }

            private void OnGUI()
            {
                if (Visible)
                {
                    try
                    {
                        EnsureStyles();
                        ClampWindow(ref window);
                        Font old = GUI.skin.font;
                        GUI.skin.font = font;
                        try
                        {
                            window = GUILayout.Window(WindowId, window, (GUI.WindowFunction)DrawWindow,
                                "lewd_meme_config 디버그 패널 ([ 키로 닫기)");
                        }
                        finally { GUI.skin.font = old; }
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.Log("[lewd_meme_config] 디버그 패널 표시 실패: " + ex.Message);
                    }
                }
                // R12:快捷测试窗口(独立开关、共存可同屏,共享本 Host)
                if (QuickVisible)
                {
                    try
                    {
                        EnsureStyles();
                        ClampWindow(ref quickWindow);
                        Font old = GUI.skin.font;
                        GUI.skin.font = font;
                        try
                        {
                            quickWindow = GUILayout.Window(QuickWindowId, quickWindow,
                                (GUI.WindowFunction)DrawQuickWindow,
                                "lewd_meme_config 빠른 테스트 (] 키로 닫기)");
                        }
                        finally { GUI.skin.font = old; }
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.Log("[lewd_meme_config] 빠른 테스트 창 표시 실패: " + ex.Message);
                    }
                }
            }

            /// <summary>窗口贴边钳制(两窗口共用;OnGUI 内逐帧)。</summary>
            private static void ClampWindow(ref Rect w)
            {
                w.width = Math.Min(w.width, Screen.width - 16f);
                w.height = Math.Min(w.height, Screen.height - 16f);
                w.x = Mathf.Clamp(w.x, 0f, Math.Max(0f, Screen.width - w.width));
                w.y = Mathf.Clamp(w.y, 0f, Math.Max(0f, Screen.height - w.height));
            }

            private void OnDestroy()
            {
                // 宿主销毁(退出游戏)时复位静态态;协程随组件销毁自动停
                component = null;
                host = null;
                Visible = false;
                QuickVisible = false;
                running = false;
                quickRunning = false;
                progress = "";
                quickProgress = "";
                quickGroupLabel = "";
                report = null;
                quickReport = null;
                foreach (QuickToggle t in QuickToggles) t.Active = false;
            }

            /// <summary>自检协程:逐批执行(每帧 12 条,进度可见),结果同步 Player.log。</summary>
            private IEnumerator RunTests(List<KeyTest> tests)
            {
                running = true;
                report = null;
                var results = new List<TestResult>(tests.Count);
                for (int i = 0; i < tests.Count; i++)
                {
                    KeyTest t = tests[i];
                    progress = "(" + (i + 1) + "/" + tests.Count + ") " + t.Key;
                    TestResult r = null;
                    try { r = SelfTestRunner.Run(t); }
                    catch (Exception ex) { UnityEngine.Debug.Log("[lewd_meme_config][selftest] " + t.Key + " ERROR " + ex.Message); }
                    if (r != null)
                    {
                        results.Add(r);
                        UnityEngine.Debug.Log("[lewd_meme_config][selftest] " + r.Id + " "
                            + (r.Pass ? "PASS" : "FAIL") + " " + r.Detail);
                    }
                    if ((i + 1) % 12 == 0) yield return null;
                }
                report = SelfTestPlan.Aggregate(results);
                progress = "";
                running = false;
            }

            private void StartRun(int tier)
            {
                if (running) return;
                List<KeyTest> tests = SelfTestRunner.Tier(tier);
                StartCoroutine(RunTests(tests));
            }

            /// <summary>
            /// R12 快捷测试组协程(镜像 RunTests;逐条执行既有执行器,进度行 +
            /// 聚合报告,日志同步 [lewd_meme_config][quicktest];每帧 12 条)。
            /// </summary>
            private IEnumerator RunQuickTests(string groupLabel, List<KeyTest> tests)
            {
                quickRunning = true;
                quickGroupLabel = groupLabel;
                quickReport = null;
                var results = new List<TestResult>(tests.Count);
                for (int i = 0; i < tests.Count; i++)
                {
                    KeyTest t = tests[i];
                    quickProgress = "(" + (i + 1) + "/" + tests.Count + ") " + t.Key;
                    TestResult r = null;
                    try { r = SelfTestRunner.Run(t); }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.Log("[lewd_meme_config][quicktest] " + t.Key + " ERROR " + ex.Message);
                    }
                    if (r != null)
                    {
                        results.Add(r);
                        UnityEngine.Debug.Log("[lewd_meme_config][quicktest] " + r.Id + " "
                            + (r.Pass ? "PASS" : "FAIL") + " " + r.Detail);
                    }
                    if ((i + 1) % 12 == 0) yield return null;
                }
                quickReport = SelfTestPlan.Aggregate(results);
                quickProgress = "";
                quickRunning = false;
                UnityEngine.Debug.Log("[lewd_meme_config][quicktest] " + groupLabel + " → "
                    + QuickResultLine(quickReport));
            }

            /// <summary>启动一组快捷测试(任一协程运行中拒绝,防并发写报告态)。</summary>
            private void StartQuickRun(string groupLabel, List<KeyTest> tests)
            {
                if (running || quickRunning) return;
                StartCoroutine(RunQuickTests(groupLabel, tests));
            }

            // ------------------------------------------------------------------
            // 窗口三区(R-T)
            // ------------------------------------------------------------------

            private void DrawWindow(int id)
            {
                GUILayout.Space(4f);

                // —— ① 状态总览(F4 逐区防炸:内容异常显示失败行,不打断 GUILayout
                //    布局,其余区照常渲染;期望态求值经 HookRuntime.SwitchOnFor 同源)——
                try
                {
                    overviewScroll = GUILayout.BeginScrollView(overviewScroll, GUILayout.Height(300f));
                    try
                    {
                        GUILayout.Label("① 연동 모드 설치 여부 / 버전", header);
                        foreach (FamilyState st in Family.Probe())
                        {
                            string text = st.Present
                                ? st.ModId + ":설치됨 v" + (st.Version ?? "?")
                                    + (st.VersionOk ? "(연동 정상)" : "(버전 불일치→적용 억제)")
                                : st.ModId + ":없음";
                            GUILayout.Label(text, label);
                        }

                        GUILayout.Space(6f);
                        GUILayout.Label("패치 구성 단위 " + HookCatalog.Units.Count + "(경로 / 예상 / 실제 / 스위치 수; 예상≠실제는 빨간색 표시)", header);
                        foreach (HookUnitDef def in HookCatalog.Units)
                        {
                            string expected = SelfTestRunner.ExpectedState(def);
                            string actual = SelfTestRunner.ActualState(def);
                            int switches = def.Keys == null ? 0 : def.Keys.Length;
                            bool mismatch = expected != "未装配" && expected != actual;
                            Color old = GUI.color;
                            if (mismatch) GUI.color = new Color(1f, 0.36f, 0.3f);
                            GUILayout.Label(def.Id + " | " + RouteText(def) + " | 예상 "
                                + expected + " | 실제 " + actual + " | " + switches, label);
                            GUI.color = old;
                        }
                    }
                    catch (Exception ex)
                    {
                        GUILayout.Label("①구역 표시 실패: " + ex.Message, label);
                    }
                    GUILayout.EndScrollView();
                }
                catch (Exception ex)
                {
                    GUILayout.Label("①구역 표시 실패: " + ex.Message, label);
                }

                // —— ② 自检 ——
                try
                {
                    GUILayout.Space(6f);
                    GUILayout.Label("② 자체 검사(3단계 + 전체; 임시로 값을 썼다가 복원하며 SpawnItem 테스트 항목은 즉시 폐기)", header);
                    GUI.enabled = !running;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Tier1 경로 검증(" + SelfTestRunner.Tier(1).Count + ")", button))
                        StartRun(1);
                    if (GUILayout.Button("Tier2 값 직접 검사(" + SelfTestRunner.Tier(2).Count + ")", button))
                        StartRun(2);
                    if (GUILayout.Button("Tier3 조회(" + SelfTestRunner.Tier(3).Count + ")", button))
                        StartRun(3);
                    if (GUILayout.Button("전체(" + SelfTestRunner.Tier(0).Count + ")", button))
                        StartRun(0);
                    GUILayout.EndHorizontal();
                    GUI.enabled = true;

                    if (running) GUILayout.Label("진행 중: " + progress, label);
                    if (report != null)
                    {
                        GUILayout.Label(report.Summary(), label);
                        reportScroll = GUILayout.BeginScrollView(reportScroll, GUILayout.Height(110f));
                        try
                        {
                            if (report.Failures.Count == 0)
                                GUILayout.Label("(실패 없음; SKIP은 대상이 없거나 패치 구성이 준비되지 않은 정상 생략 상태)", label);
                            else
                                foreach (TestResult f in report.Failures)
                                    GUILayout.Label("✗ " + f.Id + " [" + f.Section + "]: " + f.Detail, label);
                        }
                        catch (Exception ex)
                        {
                            GUILayout.Label("보고서 세부 내용 표시 실패: " + ex.Message, label);
                        }
                        GUILayout.EndScrollView();
                    }
                }
                catch (Exception ex)
                {
                    GUILayout.Label("②구역 표시 실패: " + ex.Message, label);
                }

                // —— ③ 写透快照 ——
                try
                {
                    GUILayout.Space(6f);
                    GUILayout.Label("③ 설정값 쓰기 동기화 기록(OurKey | 원본값 보존 | 상대 모드 현재값)", header);
                    mirrorScroll = GUILayout.BeginScrollView(mirrorScroll, GUILayout.Height(150f));
                    try
                    {
                        foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
                        {
                            if (e.Carrier != CarrierKind.WriteThrough) continue;
                            bool has = false;
                            string theirs = null;
                            try
                            {
                                has = WriteThroughCarrier.HasSnapshot(e);
                                ModConfig cfg = ModConfig.Find(e.TargetMod);
                                ConfigEntry entry = cfg == null ? null : cfg.Get(e.TargetKey);
                                theirs = entry == null ? null : entry.Serialize();
                            }
                            catch (Exception ex)
                            {
                                theirs = "(조회 실패: " + ex.Message + ")";
                            }
                            GUILayout.Label(e.OurKey + " | " + (has ? "보존 중" : "없음") + " | "
                                + (theirs ?? "(대상 없음)"), label);
                        }
                    }
                    catch (Exception ex)
                    {
                        GUILayout.Label("③구역 표시 실패: " + ex.Message, label);
                    }
                    GUILayout.EndScrollView();
                }
                catch (Exception ex)
                {
                    GUILayout.Label("③구역 표시 실패: " + ex.Message, label);
                }
                GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
            }

            // ------------------------------------------------------------------
            // 快捷测试窗口(R12,三区独立 try/catch 同 F4)
            // ------------------------------------------------------------------

            private void DrawQuickWindow(int id)
            {
                GUILayout.Space(4f);

                // —— ① 常用物品生成(一键替代 F9 逐个找;点击 = SpawnItem 到背包 + 日志)——
                try
                {
                    GUILayout.Label("① 자주 쓰는 아이템 생성(누르면 인벤토리에 생성; F9에서 하나씩 찾는 작업을 대신함)", header);
                    for (int i = 0; i < SpawnItems.Length; i += 4)
                    {
                        GUILayout.BeginHorizontal();
                        for (int j = i; j < Math.Min(i + 4, SpawnItems.Length); j++)
                        {
                            var entry = SpawnItems[j];
                            if (GUILayout.Button(entry.Label, button))
                                SpawnQuickItem(entry.Label, entry.ItemId);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                catch (Exception ex)
                {
                    GUILayout.Label("①구역 표시 실패: " + ex.Message, label);
                }

                // —— ② 标准测试组(一键跑组;编排层零新逻辑 = SelfTestPlan 目录行
                //    × SelfTestRunner 既有执行器;R13b +legacy/brothel 两组:目标
                //    缺席时整组灰显不可点,行级执行器另 SKIP 双保险)——
                try
                {
                    GUILayout.Space(6f);
                    GUILayout.Label("② 표준 테스트 그룹 | 권장 순서: ①에서 각 아이템 생성 → ②의 8개 그룹 모두 통과(그룹별 테스트 값 설정 → 생성/호출 → 검증 → 복원; 대상이 없는 그룹은 회색/SKIP 처리하며 실패로 집계하지 않음)", header);
                    GUI.enabled = !quickRunning && !running;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("가격 그룹(" + QuickGroupTests("price").Count + ")", button))
                        StartQuickRun("가격 그룹", QuickGroupTests("price"));
                    if (GUILayout.Button("후보군 그룹(" + QuickGroupTests("pools").Count + ")", button))
                        StartQuickRun("후보군 그룹", QuickGroupTests("pools"));
                    if (GUILayout.Button("능력치 그룹(" + QuickGroupTests("attrs").Count + ")", button))
                        StartQuickRun("능력치 그룹", QuickGroupTests("attrs"));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("쓰기 동기화 그룹(" + QuickGroupTests("wt").Count + ")", button))
                        StartQuickRun("쓰기 동기화 그룹", QuickGroupTests("wt"));
                    if (GUILayout.Button("경로 검증(" + QuickGroupTests("route").Count + ")", button))
                        StartQuickRun("경로 검증", QuickGroupTests("route"));
                    // 0.7.0 T11:新键一组(exp 公式三键 + 互动阈值[invoke 直测]
                    // + 初始处女[spawn 生成断言],5 行与 SelfTestPlan 新行同步)
                    if (GUILayout.Button("0.7.0 그룹(" + QuickGroupTests("v070").Count + ")", button))
                        StartQuickRun("0.7.0 그룹", QuickGroupTests("v070"));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    bool legacyPresent = GroupPresent(FamilyMods.Legacy);
                    bool brothelPresent = GroupPresent(FamilyMods.Brothel);
                    GUI.enabled = !quickRunning && !running && legacyPresent;
                    if (GUILayout.Button("legacy 그룹(" + QuickGroupTests("legacy").Count
                            + (legacyPresent ? "" : "/없음") + ")", button))
                        StartQuickRun("legacy 그룹", QuickGroupTests("legacy"));
                    GUI.enabled = !quickRunning && !running && brothelPresent;
                    if (GUILayout.Button("brothel 그룹(" + QuickGroupTests("brothel").Count
                            + (brothelPresent ? "" : "/없음") + ")", button))
                        StartQuickRun("brothel 그룹", QuickGroupTests("brothel"));
                    GUILayout.EndHorizontal();
                    GUI.enabled = true;

                    if (quickRunning) GUILayout.Label("진행 중: " + quickProgress, label);
                    if (quickReport != null)
                    {
                        GUILayout.Label(quickGroupLabel + " → " + QuickResultLine(quickReport), label);
                        quickScroll = GUILayout.BeginScrollView(quickScroll, GUILayout.Height(120f));
                        try
                        {
                            if (quickReport.Failures.Count == 0)
                                GUILayout.Label("(실패 없음; SKIP은 대상이 없거나 패치 구성이 준비되지 않은 정상 생략 상태)", label);
                            else
                                foreach (TestResult f in quickReport.Failures)
                                    GUILayout.Label("✗ " + f.Id + " [" + f.Section + "]: " + f.Detail, label);
                        }
                        catch (Exception ex)
                        {
                            GUILayout.Label("그룹별 결과 세부 표시 실패: " + ex.Message, label);
                        }
                        GUILayout.EndScrollView();
                    }
                }
                catch (Exception ex)
                {
                    GUILayout.Label("②구역 표시 실패: " + ex.Message, label);
                }

                // —— ③ 常用键快捷开关(Toggle 直改,经 OnSwitchChanged 同管道)——
                try
                {
                    GUILayout.Space(6f);
                    GUILayout.Label("③ 주요 설정 빠른 전환(자주 쓰는 디버그 설정에 시험값 적용/원래 값 복원, 즉시 적용)", header);
                    // T11:6 键分两行(每行 3 个,窗口宽 560 放不下 6 个横排)
                    for (int i = 0; i < QuickToggles.Length; i += 3)
                    {
                        GUILayout.BeginHorizontal();
                        for (int j = i; j < Math.Min(i + 3, QuickToggles.Length); j++)
                        {
                            QuickToggle t = QuickToggles[j];
                            bool now = GUILayout.Toggle(t.Active, t.Label, button);
                            if (now != t.Active) FlipQuickToggle(t);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                catch (Exception ex)
                {
                    GUILayout.Label("③구역 표시 실패: " + ex.Message, label);
                }
                GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
            }

            private void EnsureStyles()
            {
                if (label != null) return;
                font = Font.CreateDynamicFontFromOSFont(
                    new[] { "Microsoft YaHei", "SimHei", "Arial" }, 15);
                label = new GUIStyle(GUI.skin.label) { font = font, fontSize = 14, wordWrap = false };
                button = new GUIStyle(GUI.skin.button) { font = font, fontSize = 14, wordWrap = false };
                header = new GUIStyle(GUI.skin.label)
                {
                    font = font, fontSize = 15, fontStyle = FontStyle.Bold, wordWrap = false,
                };
            }
        }

        // ------------------------------------------------------------------
        // 快捷测试窗口的数据与动作(R12;①③ 在 OnGUI 事件里同步执行——
        // SpawnItem/注册表写均为轻量同步操作,无需协程;② 走 Host 协程)
        // ------------------------------------------------------------------

        /// <summary>①区物品行(物品 id 复用 SelfTestRunner SpawnItem 清单 = 单一事实源)。</summary>
        private sealed class SpawnItemEntry
        {
            public string Label;
            public string ItemId;
        }

        private static readonly SpawnItemEntry[] SpawnItems =
        {
            new SpawnItemEntry { Label = "유즙", ItemId = "wondrous_milk" },
            new SpawnItemEntry { Label = "정액", ItemId = "tentacle_semen" },
            new SpawnItemEntry { Label = "배란 주사", ItemId = "ovulation_shot" },
            new SpawnItemEntry { Label = "촉수 상자", ItemId = "tentacle_box" },
            new SpawnItemEntry { Label = "알람 시계", ItemId = "alarm_clock" },
            new SpawnItemEntry { Label = "촉수 알람 시계", ItemId = "tentacle_alarm" },
            new SpawnItemEntry { Label = "맨홀 뚜껑", ItemId = "manhole_cover" },
            new SpawnItemEntry { Label = "함정", ItemId = "tentacle_trap" },
            new SpawnItemEntry { Label = "프린터", ItemId = "tentacle_printer" },
            new SpawnItemEntry { Label = "빈 주사기", ItemId = "empty_syringe" },
            new SpawnItemEntry { Label = "소녀", ItemId = "maiden_lower" },
        };

        /// <summary>生成物品到背包并记日志(与自检的 MoveItemToTrash 清理不同——
        /// 本面板的目的就是给玩家物品,垃圾桶留给自检契约)。</summary>
        private static void SpawnQuickItem(string label, string itemId)
        {
            try
            {
                GameItem item = ModHelper.SpawnItem(itemId);
                if (item == null)
                {
                    UnityEngine.Debug.Log("[lewd_meme_config][quicktest] 아이템 목록에 등록되지 않음: " + itemId + "(저장 파일에서 실행해야 함)");
                    return;
                }
                ModHelper.MoveItemToInventory(item);
                UnityEngine.Debug.Log("[lewd_meme_config][quicktest] 생성 완료: " + label + "(" + itemId + ")→ 인벤토리");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("[lewd_meme_config][quicktest] 생성 실패: " + itemId + " 실패(저장 파일에서 실행해야 함): " + ex.Message);
            }
        }

        /// <summary>③区高频调试键的快捷开关(点选 = 设测试值 + 拨 .enabled;
        /// 再点 = 复原;全部经 registry.Changed → HookRuntime.OnSwitchChanged
        /// 同管道,不绕过分发器)。</summary>
        private sealed class QuickToggle
        {
            public string Label;
            public string Key;          // 配置键
            public object TestValue;    // 装箱测试值(int/float)
            public bool Active;         // 当前是否处于测试态
            public object SavedValue;   // 拨开前的原值(装箱)
            public bool SavedSwitch;    // 拨开前 .enabled 原态
        }

        private static readonly QuickToggle[] QuickToggles =
        {
            // T8:virgin_chance 键废弃(1.9.2 无条件处女),原「少女概率=1」快捷键移除
            new QuickToggle { Label = "레벨 상한=6", Key = CfgKeys.LevelCap, TestValue = 6 },
            // ST4:remake.semen_base 键全链废弃(1.6.1 SemenProduction 类删除),
            // 原「精液公式 base=1.0」快捷键移除;cap 键随跨族消费终结回归
            // main.nightly_transpile 唯一注入(ST2 前它兼作 remake.semen_formula
            // 的等价消费),Label 改主面板语义
            new QuickToggle { Label = "정액 생산 확률 상한=0.5", Key = CfgKeys.SemenChanceCap, TestValue = 0.5f },
            // 0.7.0 T11 新增三项(新键高频验证:拨开即装配 + 设测试值,经
            // OnSwitchChanged 同管道即时生效;复原 = 先值后开关)
            new QuickToggle { Label = "경험치 기본값=200", Key = CfgKeys.ExpBase, TestValue = 200 },
            new QuickToggle { Label = "상호작용 임계값=30", Key = CfgKeys.InteractThreshold, TestValue = 30 },
            new QuickToggle { Label = "음란력 상한=200", Key = CfgKeys.CorruptMaxCharge, TestValue = 200 },
        };

        /// <summary>拨动一个快捷开关:开 = 记原值/原开关 → 设测试值 → 拨 .enabled
        /// (触发 Changed → OnSwitchChanged 热装卸);关 = 先复原值、后复原开关
        /// (SelfTestRunner.WithSetup 同序)。</summary>
        private static void FlipQuickToggle(QuickToggle t)
        {
            ConfigRegistry reg = HookRuntime.Registry;
            if (reg == null)
            {
                UnityEngine.Debug.Log("[lewd_meme_config][quicktest] 설정 등록이 준비되지 않음(" + t.Label + "; 먼저 저장 파일을 불러오세요)");
                return;
            }
            try
            {
                string switchKey = PanelKeys.ItemSwitchKey(t.Key);
                if (!t.Active)
                {
                    t.SavedValue = SelfTestRunner.CurrentBoxed(t.Key);
                    t.SavedSwitch = reg.Current<bool>(switchKey);
                    SelfTestRunner.SetValueBoxed(t.Key, t.TestValue);
                    reg.SetPlayerValue(switchKey, true);
                    t.Active = true;
                }
                else
                {
                    SelfTestRunner.SetValueBoxed(t.Key, t.SavedValue);
                    reg.SetPlayerValue(switchKey, t.SavedSwitch);
                    t.Active = false;
                }
                UnityEngine.Debug.Log("[lewd_meme_config][quicktest] " + t.Label
                    + (t.Active ? " → 테스트 값 적용" : " → 원래 값 복원"));
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("[lewd_meme_config][quicktest] " + t.Label + " 전환 실패: " + ex.Message);
            }
        }

        /// <summary>
        /// ②区组员目录(编排层零新逻辑:全部取自 SelfTestPlan.KeyTests 既有行,
        /// 执行走 SelfTestRunner 既有执行器)。price/pools/attrs/v070 按键集过滤
        /// 值键行,wt/route/legacy 按机制整类收编,brothel = 掌上青楼分区的
        /// InvokeStatic 行(26 键中可直断的 21[brothel 系]——mastery_threshold
        /// 写 tag 有状态留 ReadPath),v070 = 0.7.0 新键 5 行(exp 三键 +
        /// interact_threshold[invoke]+ initial_virgin[spawn];T11)。
        /// </summary>
        private static List<KeyTest> QuickGroupTests(string group)
        {
            HashSet<string> keys = null;
            if (group == "price")
            {
                keys = new HashSet<string>(StringComparer.Ordinal)
                {
                    CfgKeys.PriceMilk, CfgKeys.PriceSemen, CfgKeys.PriceOvulationShot,
                    CfgKeys.AlarmPrice, CfgKeys.AlarmTentaclePrice,
                    CfgKeys.TrapPrice, CfgKeys.TrapTrapPrice,
                    CfgKeys.PrinterPrice, CfgKeys.PrinterNeedlePrice,
                };
            }
            else if (group == "pools")
            {
                keys = new HashSet<string>(StringComparer.Ordinal)
                {
                    CfgKeys.Factions, CfgKeys.Jobs, CfgKeys.Bodies, CfgKeys.Breasts,
                };
            }
            else if (group == "attrs")
            {
                keys = new HashSet<string>(StringComparer.Ordinal)
                {
                    CfgKeys.IphAttrHighChance, CfgKeys.IphAttrMidChance,
                    CfgKeys.IphAttrHighRange, CfgKeys.IphAttrMidRange, CfgKeys.IphAttrLowRange,
                };
            }
            else if (group == "v070")
            {
                // 0.7.0 T11:新键一组(与 SelfTestPlan 新升直测行同步——
                // exp 三键/interact_threshold 走 invoke,initial_virgin 走 spawn)
                keys = new HashSet<string>(StringComparer.Ordinal)
                {
                    CfgKeys.ExpBase, CfgKeys.ExpDivisor, CfgKeys.ExpStep,
                    CfgKeys.InteractThreshold, CfgKeys.InitialVirgin,
                };
            }
            else if (group == "brothel")
            {
                keys = new HashSet<string>(StringComparer.Ordinal);
                foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
                    if (e.Section == FamilyPanelKeys.BrothelSection)
                        keys.Add(e.OurKey);
            }
            var list = new List<KeyTest>();
            foreach (KeyTest t in SelfTestPlan.KeyTests)
            {
                if (keys != null)
                {
                    if (group == "brothel")
                    {
                        // brothel 组 = 分区键 ∩ InvokeStatic(mastery 的 ReadPath 行不入组)
                        if (t.Mechanism == TestMechanism.InvokeStatic && keys.Contains(t.Key)) list.Add(t);
                    }
                    else if (t.Mechanism != TestMechanism.RouteAudit && keys.Contains(t.Key)) list.Add(t);
                }
                else if (group == "wt" && t.Mechanism == TestMechanism.WriteThrough) list.Add(t);
                else if (group == "route" && t.Mechanism == TestMechanism.RouteAudit) list.Add(t);
                else if (group == "legacy" && t.Mechanism == TestMechanism.LegacyOverlay) list.Add(t);
            }
            return list;
        }

        /// <summary>②区组目标是否在场(R13b:legacy/brothel 组灰显判定;行级
        /// 执行器的 ModUsable SKIP 是第二道保险)。只读装配期快照——快照未建
        /// (主菜单/未进存档)时返回 false:测试本就要求装配后就绪,面板显示
        /// 「/缺席」语义正确,兼防每帧 ProbeMap 磁盘读。</summary>
        private static bool GroupPresent(string modId)
        {
            try
            {
                IReadOnlyDictionary<string, FamilyState> family = HookRuntime.FamilySnapshot;
                return family != null && family.TryGetValue(modId, out FamilyState st) && st.Present;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>组结果一行:「N PASS / M FAIL / K SKIP [失败键列表]」(R13b
        /// 显式 SKIP 计数:SKIP ⊆ PASS,目标缺席/让位/内容未加载的设计态;
        /// 失败键截断至前 8 个防长行)。</summary>
        private static string QuickResultLine(TestReport r)
        {
            if (r == null) return "";
            string line = r.Passed + " PASS / " + r.Failed + " FAIL / " + r.Skipped + " SKIP";
            if (r.Failures.Count > 0)
            {
                var ids = new List<string>(Math.Min(r.Failures.Count, 8));
                for (int i = 0; i < Math.Min(r.Failures.Count, 8); i++) ids.Add(r.Failures[i].Id);
                line += " [" + string.Join(", ", ids.ToArray())
                    + (r.Failures.Count > 8 ? " …×" + r.Failures.Count : "") + "]";
            }
            return line;
        }

        /// <summary>单元的路由裁决文案(装配期快照优先;未装配用现调值对照)。</summary>
        private static string RouteText(HookUnitDef def)
        {
            try
            {
                var snapshot = HookRuntime.RouteSnapshot;
                if (snapshot != null && snapshot.ContainsKey(def.Id))
                    return snapshot[def.Id].ToString();
                IReadOnlyDictionary<string, FamilyState> family = HookRuntime.FamilySnapshot;
                if (family == null) family = Family.ProbeMap();
                return Router.Decide(def.Id, family).ToString() + "(현재 판정)";
            }
            catch (Exception)
            {
                return "route?";
            }
        }
    }
}
