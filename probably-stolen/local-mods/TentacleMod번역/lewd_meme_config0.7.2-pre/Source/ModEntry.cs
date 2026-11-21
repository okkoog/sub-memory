using System;

namespace LewdMemeConfig
{
    /// <summary>
    /// lewd_meme_config 入口(spec §7 生命周期)。
    /// 定位：把主模组 lewd_meme 写死的参数与方法转接为可配置项。
    /// 硬约束：保持零能力声明（manifest 的 &lt;Capabilities /&gt; 为空）。
    /// </summary>
    public sealed class ModEntry : IMod
    {
        internal const string Id = "lewd_meme_config";

        private ModLog log;
        internal bool Active;

        // Init 建立的配置宿主(纯内存,无 ModReflect,主菜单阶段即可安全使用)。
        private ConfigRegistry registry;

        // 装配产物(安装器/harmony/注册表缓存)静态保存在 HookRuntime——即使
        // 加载器重建 ModEntry 实例,退出时也按缓存实例撤补丁(0.5.0 先例)。

        public void Init(ModManifest manifest)
        {
            log = new ModLog(manifest);
            // 调试面板(Task 14):发布开关 Enabled=false 时完全惰性(零 GameObject/
            // 零 F9/零订阅),此调用是无条件 no-op
            DebugPanel.Activate();
            MainModResolver.Logger = s => log.Log(s);
            registry = ConfigRegistry.CreateDefault();
            // 互斥让位(spec §5.3;Init 期,面板 Define 前):读加载器的启用集合——
            // EnabledMods 在任何 mod Init 之前已从磁盘就绪,不受 mod 加载顺序影响
            // (IsModRunning 读的是「已完成 Init+OnEnable 并登记」的列表,Init 期
            // 另一 mod 可能尚未登记,不可用)。ModRestricted 只拦信任层写操作,
            // IsModEnabled 纯读,mod 调用合法。在场则不开面板;装配路径还有
            // IsModRunning 让位双保险(游戏内验收:单装面板可开/双装整体让位)。
            if (ModLoader.IsModEnabled("lewd_meme_api"))
            {
                log.Log("lewd_meme_api 활성화 감지. 추가 설정 기능을 양보하여 패널 및 패치를 설치하지 않음(둘 중 하나만 사용, 변경 후 게임 재시작 필요).");
                return;
            }
            // 玩家配置面板(更多可配置项 85 键):Define + 玩家层注入 + OnChanged 转发。
            // 必须在 Init(主菜单阶段)执行——Mods 菜单齿轮入口只在主菜单可用,而
            // OnModItemDirectoryInit 要进入存档后才触发(上游 lewd_meme_api 先例)。
            // 整体失败不影响配置读取(默认值照常)。
            try
            {
                PanelConfig.DefineAll(ModHelper.GetConfig(Id), registry, s => log.Log(s));
                // 子 mod 镜像分区(M4b/R-I):remake/Brood 在场才 Define 其分区
                // (「暗流交易联动」/「巢穴扩展联动」);Family.Probe 在 Init 期可靠
                // (前置先载,对方 Init 已跑过,其 ModConfig 已注册)。缺席 = 面板
                // 无此分区,零写零订阅。
                PanelConfig.DefineFamily(ModHelper.GetConfig(Id), registry, s => log.Log(s));
            }
            catch (Exception ex)
            {
                log.Log("설정 패널 초기화 실패(기본값 읽기에는 영향 없음):" + ex.Message);
            }
            // 0.6.0 面板标记(M3d/R-C):家族在场/版本快照 → 让位/版本压制标记表,
            // 面板行构建期(PanelRowPatch prefix)只读渲染。Init 期即可靠
            // (Family.Probe 只读 ModLoader.Mods + IsModEnabled,任何 mod Init 前
            // 已就绪);失败只记日志(标记缺席 = 行不带提示,零危害)。
            try
            {
                PanelMarks.Rebuild(Family.ProbeMap());
            }
            catch (Exception ex)
            {
                log.Log("패널 표시 상태 생성 실패(설정 항목에 기능 양보/버전 안내가 표시되지 않음):" + ex.Message);
            }
            // 0.5.1 面板三件套(滑条+实时数值+精调文本框):Init 期即装——
            // 玩家主要在主菜单改配置,装配期装太晚。失败只记日志(退回原版行)。
            // 独立 harmony id 恒装,不参与按需装卸(R11:装配计数不含它)。
            PanelRowPatch.Install(s => log.Log(s));
            // spec §7.1:Init 只建注册表;此时加载器尚未登记本 mod,
            // 任何 ModReflect 查找都会被拒,一概不做。
            log.Log("lewd_meme_config 초기화 완료: 설정 등록 준비됨.");
        }

        public void OnEnable()
        {
            Active = true;
            // 加载器要等 Init 与首次 OnEnable 返回之后才登记本模组,
            // 集成推迟到物品目录初始化之后(remake / ItemPortraitHD 先例)。
            ModHook.OnModItemDirectoryInit += OnDirectoriesReady;
            log.Log("lewd_meme_config 활성화됨. 아이템 목록 초기화를 기다리는 중.");
        }

        public void OnDisable()
        {
            // 仅退出游戏时调用(Mods 菜单停用不触发)。
            Active = false;
            ModHook.OnModItemDirectoryInit -= OnDirectoriesReady;
            // 终局撤装集中于此(R10):安装器 UninstallAll(按记账逐方法 Unpatch
            // 自身 id)+ Changed 分发器退订(DataWrite 处理器随之退订)+ 面板三件套
            // 撤装 + PatchCtx/MainModResolver 清理。
            HookRuntime.Shutdown();
            log?.Log("lewd_meme_config 비활성화됨.");
        }

        private void OnDirectoriesReady(ModItemDirectory _)
        {
            if (!Active) return;
            // 幂等(R1):重复触发的物品目录事件(再次进档)不再重探测/重装配——
            // HookRuntime.composed 护栏(镜像旧 hub != null 判定)。
            if (HookRuntime.Composed) return;

            // 互斥让位(spec §5.3;装配期双保险):Init 期已按启用集合让位过面板,
            // 这里用 IsModRunning(官方通道,此时全部 mod 已登记,可靠)复核实际
            // 运行状态——在场则不装补丁,本 mod 成为空壳;让位后事件
            // 重复触发会再次进入本分支,幂等无害。
            if (ModHelper.IsModRunning("lewd_meme_api"))
            {
                log.Log("lewd_meme_api 감지. 추가 설정 기능을 양보하여 패치를 설치하지 않음(둘 중 하나만 사용, 변경 후 게임 재시작 필요).");
                return;
            }

            // 0.5.0:ItemPortraitHD(物品立绘管理器)调用链兼容解析——在任何可能
            // 消费它的装配之前(失败只记日志,Active=false 走原版,A 兜底)
            IphCompat.Resolve(s => log.Log(s));

            // ① 探测主 mod(此时本 mod 已被加载器登记,ModReflect 合法)
            MainModResolveResult resolve = MainModResolver.Resolve();
            // ② 先注入 PatchCtx —— 装配瞬间即读它(顺序硬约束)
            PatchCtx.Config = registry;
            // ③ 主 mod 面板动态层:拾荒少女概率转发 ModSettings.MaidenChance
            //    (独立 try/catch,失败只记日志,不参与装配;MainModAbsent 守卫 =
            //    缺席时 Types 为空字典,恒跳过)
            if (resolve.Types.Count > 0)
            {
                TryInjectMainModPanel(resolve);
            }
            // ④ remake 让位日志(ST 1.6.1:R1 自 9 缩至 2——on_loot_rolled/
            //    price_ovulation_shot 两单元在 HookRuntime ③ 登记期过滤,不产
            //    hook;夜间数值组恢复共存装配;此日志只作玩家可见的总览行)
            if (resolve.RemakeActive)
            {
                log.Log("remake 감지. 드롭 빈도 제한 및 배란 주사 단가 제어를 양보함(on_loot_rolled/price_ovulation_shot). 야간 수치 그룹은 1.6.1부터 함께 패치되며 각 스위치에 따라 적용됨.");
            }
            // ⑤ 按需装配:目录 + 映射 + 状态机在 HookRuntime 合流——只装
            //    「开关开 且 非版本压制」的单元(默认全关 = 0 项,零 hook),
            //    DataWrite 装配期一次写入,Changed 分发器在其中单一订阅
            HookRuntime.ComposeAndInstall(registry, resolve, s => log.Log(s));
        }

        /// <summary>
        /// 主 mod 面板动态层注入(Task 8):把 loot.scav_maiden_base 的面板层接到
        /// 主 mod <c>ModSettings.MaidenChance</c> getter(每次读取现求值,玩家在
        /// 主 mod 面板调整拾荒概率即时生效)。getter 内部已 clamp [0.05,0.25] 且
        /// 失败回 0.05(反编译 ModSettings.cs L16-32),此处不双重钳制;任何失败
        /// (类型缺失/签名漂移)只记日志跳过——配置链退默认 0.05f,与 getter 的
        /// catch 回退值相等,零行为差。ModSettings 在 resolve.Types(FullName→Type
        /// 字典,MainModAbsent 时为空字典,调用方已先行守卫)按 Name 查找
        /// (同 LootPatch.FindMainType 的同名命中语义)。
        /// </summary>
        private void TryInjectMainModPanel(MainModResolveResult resolve)
        {
            try
            {
                System.Type settings = null;
                foreach (var kv in resolve.Types)
                    if (kv.Value.Name == "ModSettings")
                    {
                        settings = kv.Value;
                        break;
                    }
                if (settings == null)
                    throw new MissingMemberException("본편 모드에 ModSettings 타입이 노출되지 않음");
                var getter = ModReflect.StaticMethod<Func<float>>(settings, "get_MaidenChance");
                registry.SetMainModPanelProvider(CfgKeys.ScavMaidenBase, getter);
                log.Log("본편 모드 패널의 동적 설정 연결 완료: loot.scav_maiden_base ← ModSettings.MaidenChance");
            }
            catch (Exception ex)
            {
                log.Log("본편 모드 패널의 동적 설정 연결 실패(폐품 수집 시 소녀 발견 확률은 기본값 0.05로 복귀):" + ex.Message);
            }
        }
    }
}
