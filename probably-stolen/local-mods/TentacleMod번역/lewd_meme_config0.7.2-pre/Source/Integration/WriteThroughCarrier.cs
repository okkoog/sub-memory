using System;
using System.Globalization;

namespace LewdMemeConfig
{
    /// <summary>
    /// 写透载体(M4b 游戏侧,spec §4.5/控制器 R-H):把本面板镜像值写进对方 mod
    /// 的官方 ModConfig 条目(remake 7 键 / Brood 12 键,目标形态见
    /// FamilyPanelKeys)。值序列化一律 string 往返(WriteThroughEngine 契约):
    /// 我方强类型值经 <see cref="ConfigEntry.Format"/>(公开静态,invariant)转出;
    /// 写回经 <c>ConfigEntry&lt;T&gt;.Set(T)</c>(公开,类型模式匹配分派,零反射)
    /// ——Set 自带 Sanitize(Choices/Range 钳制)、触发对方自己的 OnChanged
    /// (等价玩家在对方面板改值,官方语义——本 mod 从不订阅它,被动
    /// last-writer-wins)且值变化时自动 Save;此处再显式 Save 兜底(R-H:写回 +
    /// Save;Save 幂等,值未变时多写一次无行为差)。
    /// 注:ConfigEntry.TryDeserialize / ApplyBoxed 是 internal(mod 程序集不可见,
    /// 游戏 decomp 无 InternalsVisibleTo),BoxedValue 只读无 setter——公开写通道
    /// 只有强类型 Set,这就是类型分派的由来(反编译 ConfigEntry.cs 逐行确认)。
    /// 存在性守卫(spec §4.6「写透键免绑」):对方 config 未注册(ModConfig.Find
    /// null——比 GetConfig=For 更干净:不为缺席目标创建空实例)或目标键缺席
    /// (Get null,版本漂移信号)→ 日志 + false,零写入、零快照。
    /// </summary>
    internal static class WriteThroughCarrier
    {
        private static readonly WriteThroughEngine Engine = new WriteThroughEngine();

        /// <summary>
        /// 启用写透:快照对方当前值(Serialize)+ 写入 ourValue。快照锚定开启
        /// 时刻:周期内重复 Enable(值变化重写路径)只写值、不覆盖恢复点
        /// (engine 0.6.0 修正语义);Disable 清除后再次 Enable 重新锚定。
        /// 目标缺席 → 日志 + false(键守卫)。
        /// </summary>
        internal static bool Enable(FamilyPanelEntry e, string ourValue, Action<string> log)
        {
            ConfigEntry entry = FindEntry(e, log);
            if (entry == null) return false;
            ModConfig config = entry.Config; // 所属 ModConfig(与 Find 同一实例)
            Engine.Enable(e.TargetMod, e.TargetKey, entry.Serialize(), ourValue,
                v => Write(config, entry, v));
            return true;
        }

        /// <summary>
        /// 停用写透:恢复开启前快照(engine.Disable;无快照纯 no-op,不调 write)。
        /// 目标缺席 → 日志 + false(此时必无快照,语义同为 no-op)。
        /// </summary>
        internal static bool Disable(FamilyPanelEntry e, Action<string> log)
        {
            ConfigEntry entry = FindEntry(e, log);
            if (entry == null) return false;
            ModConfig config = entry.Config;
            Engine.Disable(e.TargetMod, e.TargetKey, v => Write(config, entry, v));
            return true;
        }

        /// <summary>
        /// 终局恢复(HookRuntime.Shutdown / spec §5.2 OnDisable「写透键恢复快照」):
        /// 逐镜像键 Disable,有未恢复快照的键写回对方原值,无快照纯 no-op;
        /// 逐键隔离,失败只记日志。M5a:SubModHook 条目(brothel.*)无写透语义,
        /// 直接跳过(其撤装归安装器 UninstallAll)。
        /// </summary>
        internal static void RestoreAll(Action<string> log)
        {
            foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
            {
                if (e.Carrier != CarrierKind.WriteThrough) continue;
                try
                {
                    Disable(e, log);
                }
                catch (Exception ex)
                {
                    log?.Invoke("직접 기록 스냅샷 복원 실패(" + e.OurKey + "): " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 该镜像键是否持有未恢复快照。调用方 = DebugPanel 写透快照区(工程内唯一
        /// 消费点,经此转发 engine 侧查询——保留判定 2026-11-13 清理单)。
        /// </summary>
        internal static bool HasSnapshot(FamilyPanelEntry e)
        {
            return Engine.HasSnapshot(e.TargetMod, e.TargetKey);
        }

        /// <summary>存在性守卫:对方 config 已注册且目标键在场,否则日志 + null。</summary>
        private static ConfigEntry FindEntry(FamilyPanelEntry e, Action<string> log)
        {
            ModConfig config = ModConfig.Find(e.TargetMod);
            if (config == null)
            {
                log?.Invoke("직접 기록 대상 모드가 등록되지 않아 건너뜀: " + e.TargetMod + " / " + e.TargetKey);
                return null;
            }
            ConfigEntry entry = config.Get(e.TargetKey);
            if (entry == null)
            {
                log?.Invoke("직접 기록 대상 키가 없어 건너뜀(버전 차이 가능성): " + e.TargetMod + " / " + e.TargetKey);
                return null;
            }
            return entry;
        }

        /// <summary>
        /// string → 对方条目强类型值的公开通道分派:ConfigEntry&lt;T&gt;.Set(T)。
        /// int/float 用 invariant TryParse(与 ConfigEntry.Format 的序列化对称);
        /// string 直通(Sanitize 会按对方 Choices 钳制);解析失败抛(上游
        /// HookRuntime 逐键 catch 记日志,快照语义见 engine 注释)。
        /// </summary>
        private static void Write(ModConfig config, ConfigEntry entry, string text)
        {
            if (entry is ConfigEntry<string> es) es.Set(text);
            else if (entry is ConfigEntry<int> ei
                && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
                ei.Set(i);
            else if (entry is ConfigEntry<bool> eb
                && bool.TryParse(text, out bool b))
                eb.Set(b);
            else if (entry is ConfigEntry<float> ef
                && float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float f))
                ef.Set(f);
            else
                throw new InvalidOperationException(
                    "공개 인터페이스로 직접 기록 값을 되쓸 수 없음: " + config.ModId + "/" + entry.Key + " ← '" + text + "'");
            config.Save(); // Set 值未变化时不自动 Save,显式兜底(R-H)
        }
    }
}
