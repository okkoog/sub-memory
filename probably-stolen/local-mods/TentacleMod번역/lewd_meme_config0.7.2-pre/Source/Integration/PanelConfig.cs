using System;
using UnityEngine;

namespace LewdMemeConfig
{
    /// <summary>
    /// 配置面板游戏壳(spec 2026-11-11 §6.2):把 PanelKeys 定义表喂给官方 ModConfig,
    /// 并承担玩家层流转——Define 后立即注入 entry.Value(此时 json 已保存值已生效),
    /// 订阅 OnChanged 转发进 ConfigRegistry.SetPlayerValue;
    /// registry.Changed 的既有订阅(池重写/数值刷新)自动覆盖玩家改值路径。
    /// M4b 增 <see cref="DefineFamily"/>:子 mod 镜像分区(remake/Brood)按在场
    /// 条件 Define(R-I),玩家层流转与主面板同构。
    /// 单键失败只记日志跳过,不阻断其余(spec §6.1);无单测(依赖游戏类型),
    /// 行为由游戏内验收覆盖(同 MainModResolver 先例)。
    /// </summary>
    internal static class PanelConfig
    {
        internal static void DefineAll(ModConfig config, ConfigRegistry registry, Action<string> log)
        {
            foreach (var e in PanelKeys.Entries)
            {
                try
                {
                    DefineOne(config, registry, e, log);
                }
                catch (Exception ex)
                {
                    log?.Invoke("설정 패널 키 정의 실패: " + e.Key + ": " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 子 mod 镜像分区(M4b,R-I 条件 Define):按 <see cref="Family.ProbeMap"/>
        /// 在场快照过滤——目标缺席 → 该分区条目整体不 Define(面板不可见,缺席
        /// no-op 红线:零 Define、零写、零订阅);在场 → 值键 + 子项开关键全部
        /// Define 进本 mod 自己的 ModConfig(id lewd_meme_config),分区
        /// 「暗流交易联动」(remake 8 键)/「巢穴扩展联动」(Brood 12 键)。
        /// 玩家层流转与主面板同构:Define 注入 entry.Value(我们 json 已存的面板值)
        /// + OnChanged 转发 SetPlayerValue。镜像初值不强行同步对方已有非默认值
        /// (spec §4.5:启用期间以本面板为准,tooltip 已注明)。
        /// Init 期调用(Family.Probe 只读 ModLoader.Mods + IsModEnabled,任何 mod
        /// Init 前已就绪);单键失败只记日志跳过。
        /// </summary>
        internal static void DefineFamily(ModConfig config, ConfigRegistry registry, Action<string> log)
        {
            System.Collections.Generic.IReadOnlyDictionary<string, FamilyState> family = Family.ProbeMap();
            foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
            {
                try
                {
                    if (!family.TryGetValue(e.TargetMod, out FamilyState st) || !st.Present)
                        continue; // 目标缺席:不 Define(面板无此分区)
                    DefineMirrorOne(config, registry, e);
                }
                catch (Exception ex)
                {
                    log?.Invoke("연동 설정 패널 키 정의 실패: " + e.OurKey + ": " + ex.Message);
                }
            }
        }

        /// <summary>单个镜像条目 Define:先子项开关键、后值键(0x06 轮 F3:开关
        /// 行在前、值行在后,与主面板 0.5.x 的「ItemSwitchKey 条目紧邻且先于其
        /// 值条目」形态一致——游戏面板按 Define 顺序展示)。0.7.1:自开关 Toggle /
        /// EnumKeyCode 纯参数条目(0.7.1 拾取器,ConfigRegistry.SelfSwitchForm)
        /// 不派生 .enabled 行——面板恰一行;分区行序仍按条目表序(开关条目在前)。</summary>
        private static void DefineMirrorOne(ModConfig config, ConfigRegistry registry, FamilyPanelEntry e)
        {
            if (!ConfigRegistry.SelfSwitchForm(e))
            {
                // 子项开关键(OurKey + ".enabled",默认关 = 装上即纯原版;0.5.0 开关
                // 体系同形态,ConfigRegistry.PlayerLayerAllowed 的压层语义天然覆盖)。
                // M5a:开关语义按载体分流——WriteThrough = 写对方配置+快照;SubModHook =
                // 装卸接管补丁(关闭即恢复对方原版行为)
                string switchNote = e.Carrier == CarrierKind.SubModHook
                    ? "켜기 = 이 패널의 값으로 상대 모드의 동작을 제어, 끄기 = 상대 모드의 원래 동작으로 복귀"
                    : "켜기 = 이 패널의 값을 상대 모드 설정에 기록하고 기존 값을 별도 보관;" + FamilyPanelKeys.ThroughNote;
                string switchKey = PanelKeys.ItemSwitchKey(e.OurKey);
                var sw = config.Define(switchKey, registry.Current<bool>(switchKey), e.Section,
                    () => "사용:" + e.Label,
                    () => switchNote);
                registry.SetPlayerValue(switchKey, sw.Value);
                sw.OnChanged += v => registry.SetPlayerValue(switchKey, v);
            }

            switch (e.Kind)
            {
                case PanelKind.Choice:
                {
                    // 镜像 Choice = 对方档位串直通(无注册表 int 转换——键域独立,
                    // 不走主面板 gender_scope 的 ToRegistryValue 通道)
                    var entry = config.Define(e.OurKey, registry.Current<string>(e.OurKey), e.Section,
                        () => e.Label, e.Choices, () => e.Tooltip);
                    registry.SetPlayerValue(e.OurKey, entry.Value);
                    entry.OnChanged += v => registry.SetPlayerValue(e.OurKey, v);
                    break;
                }
                case PanelKind.SliderInt:
                {
                    // 数值镜像:范围 = 对方 Define 范围;PanelRowPatch 三件套(滑条 +
                    // 实时数值 + 精调文本框)对本 mod 面板的数值行自动生效
                    var entry = config.Define(e.OurKey, registry.Current<int>(e.OurKey), e.Section,
                        () => e.Label, e.MinInt, e.MaxInt,
                        () => e.Tooltip + " " + NumericInput.RangeNoteInt(e.MinInt, e.MaxInt));
                    registry.SetPlayerValue(e.OurKey, entry.Value);
                    entry.OnChanged += v => registry.SetPlayerValue(e.OurKey, v);
                    break;
                }
                case PanelKind.SliderFloat:
                {
                    // M5b(tearl.spawn_chance):float 数值镜像,范围 = 本 mod 自定
                    // 面板元数据(SubModHook 无对方 Define 可镜像),三件套同生效
                    var entry = config.Define(e.OurKey, registry.Current<float>(e.OurKey), e.Section,
                        () => e.Label, e.MinFloat, e.MaxFloat,
                        () => e.Tooltip + " " + NumericInput.RangeNoteFloat(e.MinFloat, e.MaxFloat));
                    registry.SetPlayerValue(e.OurKey, entry.Value);
                    entry.OnChanged += v => registry.SetPlayerValue(e.OurKey, v);
                    break;
                }
                case PanelKind.Input:
                {
                    // R9(IPH body_weights_csv / scavenger_double_virgin_csv):
                    // string CSV 文本框镜像(choices 省略),与主面板 maiden 四池
                    // CSV 同形态;对方 Set 的 Sanitize 按其 Parse 规则整串回退
                    var entry = config.Define(e.OurKey, registry.Current<string>(e.OurKey), e.Section,
                        () => e.Label, tooltip: () => e.Tooltip);
                    registry.SetPlayerValue(e.OurKey, entry.Value);
                    entry.OnChanged += v => registry.SetPlayerValue(e.OurKey, v);
                    break;
                }
                case PanelKind.Toggle:
                {
                    var entry = config.Define(e.OurKey, registry.Current<bool>(e.OurKey), e.Section,
                        () => e.Label, () => e.Tooltip);
                    registry.SetPlayerValue(e.OurKey, entry.Value);
                    entry.OnChanged += v => registry.SetPlayerValue(e.OurKey, v);
                    break;
                }
                case PanelKind.EnumKeyCode:
                {
                    // 0.7.1 拾取器:游戏原生 DefineEnum<KeyCode> 下拉(Brood 热键先例
                    // Settings.cs:130 testPanelKey;Choices = Enum.GetNames 全名——
                    // 游戏原生形态即如此)。注册表层存 KeyCode 名串(string 与
                    // Choice/Input 同路径):Define 时解析回枚举喂入,OnChanged 转
                    // 名串写回玩家层(值 = 默认名 → SetPlayerValue 自动视为未干预)
                    var entry = config.DefineEnum<KeyCode>(e.OurKey,
                        ParseKeyCodeName(registry.Current<string>(e.OurKey)), e.Section,
                        () => e.Label, () => e.Tooltip);
                    registry.SetPlayerValue(e.OurKey, entry.Value.ToString());
                    entry.OnChanged += v => registry.SetPlayerValue(e.OurKey, v.ToString());
                    break;
                }
            }
        }

        /// <summary>KeyCode 名串 → 枚举(json/注册表侧坏值回退默认键,与
        /// BroodTagPicker.CurrentKey 同兜底语义;默认值单源 = BroodTagPicker.
        /// DefaultKey;分开实现——此处无游戏日志出口)。</summary>
        private static KeyCode ParseKeyCodeName(string name)
        {
            return Enum.TryParse<KeyCode>(name, out KeyCode kc)
                && Enum.IsDefined(typeof(KeyCode), kc) ? kc : BroodTagPicker.DefaultKey;
        }

        private static void DefineOne(ModConfig config, ConfigRegistry registry, PanelEntry e, Action<string> log)
        {
            switch (e.Kind)
            {
                case PanelKind.Toggle:
                {
                    var entry = config.Define(e.Key, registry.Current<bool>(e.Key), e.Section,
                        Labeled(e, registry), () => e.Tooltip);
                    registry.SetPlayerValue(e.Key, entry.Value);
                    entry.OnChanged += v => registry.SetPlayerValue(e.Key, v);
                    break;
                }
                case PanelKind.SliderInt:
                {
                    // 0.5.1:数值键回归 int/float + 范围的 Define(原版 Apply 按滑条读值);
                    // 精确输入由 PanelRowPatch 三件套的行内文本框承担(滑条粗调+数值实时显示+
                    // 文本框精调)。三件套未安装时退回原版滑条行(可用,显示不精确)。
                    var entry = config.Define(e.Key, registry.Current<int>(e.Key), e.Section,
                        Labeled(e, registry), e.MinInt, e.MaxInt, () => e.Tooltip + " " + NumericInput.RangeNoteInt(e.MinInt, e.MaxInt));
                    registry.SetPlayerValue(e.Key, entry.Value);
                    entry.OnChanged += v => registry.SetPlayerValue(e.Key, v);
                    break;
                }
                case PanelKind.SliderFloat:
                {
                    var entry = config.Define(e.Key, registry.Current<float>(e.Key), e.Section,
                        Labeled(e, registry), e.MinFloat, e.MaxFloat, () => e.Tooltip + " " + NumericInput.RangeNoteFloat(e.MinFloat, e.MaxFloat));
                    registry.SetPlayerValue(e.Key, entry.Value);
                    entry.OnChanged += v => registry.SetPlayerValue(e.Key, v);
                    break;
                }
                case PanelKind.Choice:
                {
                    var entry = config.Define(e.Key, PlayerChoices.ToLabel(registry.Current<int>(e.Key)),
                        e.Section, Labeled(e, registry), e.Choices, () => e.Tooltip);
                    registry.SetPlayerValue(e.Key, ToRegistryValue(e, entry.Value));
                    entry.OnChanged += v => registry.SetPlayerValue(e.Key, ToRegistryValue(e, v));
                    break;
                }
                case PanelKind.Input:
                {
                    // string 文本框(choices 省略);玩家层流转与数值键一致(同源默认 + OnChanged 转发)
                    var entry = config.Define(e.Key, registry.Current<string>(e.Key), e.Section,
                        Labeled(e, registry), tooltip: () => e.Tooltip);
                    registry.SetPlayerValue(e.Key, entry.Value);
                    entry.OnChanged += v => registry.SetPlayerValue(e.Key, v);
                    break;
                }
            }
        }

        /// <summary>label 联动标记已按作者裁定移除:游戏面板 label 仅在窗口构建时
        /// 求值一次(开着不刷新),「[关]」前缀会在开关打开后误导;子项开关紧邻其值,
        /// 标记无必要。label 回归静态。</summary>
        private static System.Func<string> Labeled(PanelEntry e, ConfigRegistry registry)
        {
            return () => e.Label;
        }

        /// <summary>面板值(string)→ 注册值(int)转换;新增 Choice 键未写映射时抛,由 DefineAll 记日志暴露。</summary>
        private static int ToRegistryValue(PanelEntry e, string label)
        {
            if (e.Key == CfgKeys.GenderScope) return PlayerChoices.ToScope(label);
            throw new InvalidOperationException("지원하지 않는 Choice 패널 키: " + e.Key);
        }
    }
}
