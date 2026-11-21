using System;
using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 面板行标记表(M3d,纯逻辑):Router.DecideAll 的裁决 → 面板键 → 标记文案。
    /// 版本不匹配 → 恒 <c>"版本不匹配,该项不生效"</c>(spec §4.6 固定文案;
    /// R5 去「***」前缀——渲染层灰色独立小字段本身即标识);
    /// 让位 → Router.YieldNote(单元)(spec §5.4 R6)。
    /// 登记粒度 = 键级聚合(F5 精化):一键多消费单元时,该键的<b>全部</b>消费
    /// 单元均被压制/让位才标记,任一 Own 即该键仍有效不标(如 gender_scope 的
    /// main.on_generated 被压制时 game.* 两单元 TargetMod=null 照常工作;nightly
    /// 组的 sex_min_base 在 brood 让位 nightly_tick 时 manual_render 反界单元仍
    /// Own)。文案聚合:任一消费单元 VersionBlocked → 版本文案优先(R5:压过
    /// Yield);否则取目录序首个 Yield 单元的 YieldNote(多 owner 固定序)。
    /// 标记连同各键的派生子项开关键(PanelKeys.ItemSwitchKey)。
    /// 非破坏性红线(spec §4.6 设计裁定):只写本类内存表,绝不写 ConfigRegistry /
    /// ModConfig json——压制是运行层语义,玩家偏好保留,绑定新版本后自动恢复。
    /// 调用点:ModEntry.Init(面板 Define 后,主菜单窗口构建即可见)与 HookRuntime
    /// ComposeAndInstall(装配期复核)各一次;窗口行构建期(PanelRowPatch prefix)
    /// 只读 <see cref="For"/>。公开面仅 Rebuild/For 两个方法——除 Rebuild(只接受
    /// 家族快照)外无任何写入入口。
    /// </summary>
    public static class PanelMarks
    {
        /// <summary>版本不匹配固定文案(R5 修订:去「***」前缀,富文本后缀段
        /// 的灰色即标识);internal:PanelRowPatch 的标记渲染用它区分版本文案
        /// (灰 #9e9e9e)与让位文案(绿 #6fd073)。</summary>
        internal const string VersionMismatchText = "버전이 일치하지 않아 이 설정은 적용되지 않습니다";

        /// <summary>标记是否为版本不匹配文案(PanelRowPatch 渲染分流:版本文案
        /// 灰色、让位文案绿色,均 85% 独立小字富文本后缀段——R5 方案)。</summary>
        internal static bool IsVersionMark(string mark)
        {
            return mark == VersionMismatchText;
        }

        private static readonly Dictionary<string, string> marks =
            new Dictionary<string, string>(StringComparer.Ordinal);

        /// <summary>
        /// 以家族在场快照重建标记表(先清空):Router.DecideAll 单元级裁决 →
        /// 键 → 消费单元反查(HookCatalog Units 的 Keys)→ 键级聚合(F5:全部
        /// 消费单元均非 Own 才标)。输入 = Family.Probe 结果的字典形态。
        /// </summary>
        public static void Rebuild(IReadOnlyDictionary<string, FamilyState> family)
        {
            marks.Clear();
            IReadOnlyDictionary<string, RouteDecision> route = Router.DecideAll(family);

            // 键 → 消费单元反查(目录序;u.Keys 为空的单元无面板面,跳过)
            var keyUnits = new Dictionary<string, List<HookUnitDef>>(StringComparer.Ordinal);
            foreach (HookUnitDef u in HookCatalog.Units)
            {
                if (u.Keys == null) continue;
                foreach (string key in u.Keys)
                {
                    if (!keyUnits.TryGetValue(key, out List<HookUnitDef> list))
                    {
                        list = new List<HookUnitDef>();
                        keyUnits[key] = list;
                    }
                    list.Add(u);
                }
            }

            foreach (KeyValuePair<string, List<HookUnitDef>> kv in keyUnits)
            {
                bool anyBlocked = false, firstYield = true, allSuppressed = true;
                string yieldNote = null;
                foreach (HookUnitDef u in kv.Value)
                {
                    RouteDecision d = route[u.Id];
                    if (d == RouteDecision.Own) { allSuppressed = false; break; }
                    if (d == RouteDecision.VersionBlocked) anyBlocked = true;
                    else if (firstYield)
                    {
                        yieldNote = Router.YieldNote(u.Id);
                        firstYield = false;
                    }
                }
                if (!allSuppressed) continue;
                // 同键版本 × 让位双候选:版本文案优先(不覆盖语义的聚合形态);
                // 纯让位键取目录序首个让位单元的 YieldNote(多 owner 固定序)。
                string text = anyBlocked ? VersionMismatchText : yieldNote;
                if (text == null) continue; // 防御:无 Own 必有 Blocked/Yield,理论不可达
                marks[kv.Key] = text;
                marks[PanelKeys.ItemSwitchKey(kv.Key)] = text;
            }
        }

        /// <summary>面板键 → 标记文案;非标记键(含未知键)返回 null。</summary>
        public static string For(string key)
        {
            return marks.TryGetValue(key, out string mark) ? mark : null;
        }
    }
}
