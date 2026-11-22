using System;
using System.Globalization;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace LewdMemeConfig
{
    /// <summary>
    /// 面板行接管(0.5.1 三件套 + 0.6.0 标记渲染):prefix patch 游戏
    /// ModConfigWindow.BuildEntry,只接管本 mod(config.ModId == lewd_meme_config)的行。
    ///  - 数值行(int/float 带范围):三件套——滑条(粗调)+ 实时数值 label +
    ///    文本框(精调),模式照抄巢穴扩展(claude.lewdMemeBrood ConfigRows)的
    ///    生产验证先例:滑条 widget 的 id = entry.Key → 原版 Apply 管道
    ///    (window.Get → ReadWidget → 写 entry)零改动复用;OnSlider/OnInput 是
    ///    纯视图层事件,不碰数据;文本框编辑结束(onEndEdit)→ NumericInput
    ///    解析+钳制 → SetFloat(notify=true) 写滑条 → 回显同步。
    ///  - 标记渲染(0.6.0 spec §4.6/§5.4):PanelMarks.For(key) 非空的行,label
    ///    文案后追加 TMP 富文本后缀段(0x07 轮 R5):「版本不匹配,该项不生效」
    ///    灰 #9e9e9e /「启用&lt;owner&gt;,该配置不生效」绿 #6fd073,均
    ///    &lt;size=85%&gt; 独立小字段——主 label 原样式不动(不设控件级
    ///    color/fontSize,R4 的整 label 弱化做法废弃);同键双标记(版本 × 让位)
    ///    版本优先只拼一段。带标记的开关/选项/CSV 行做轻接管——逐句复刻原版
    ///    BuildEntry 的控件构建(控件 id = entry.Key,Apply 管道零改动),仅
    ///    label 换文案;数值行在三件套 label 追加。窗口构建期求值一次
    ///    (Apply 模型无实时刷新,0.5.0 裁定边界);无标记的非数值行与其余
    ///    mod 放行原版。
    /// 装配时机:ModEntry.Init(主菜单)即装——玩家主要在主菜单改配置,装配期装太晚;
    /// 独立 Harmony id(ModEntry.Id + ".panel"),OnDisable 单独 UnpatchAll。
    /// BuildEntry 签名漂移(游戏更新)→ Install 抛 → ModEntry 记日志跳过,
    /// 面板退回原版行,零危害。
    /// </summary>
    internal static class PanelRowPatch
    {
        private const string PatchId = ModEntry.Id + ".panel";
        private static Harmony harmony;

        /// <summary>Init 期安装(主菜单);失败只记日志,面板退回原版行。</summary>
        internal static void Install(Action<string> log)
        {
            try
            {
                harmony = new Harmony(PatchId);
                MethodBase original = ModReflect.Target(typeof(ModConfigWindow), "BuildEntry");
                harmony.Patch(original, prefix: new HarmonyMethod(typeof(PanelRowPatch), nameof(BuildEntryPrefix)));
            }
            catch (Exception ex)
            {
                harmony = null;
                log?.Invoke("面板三件套安装失败,数值行退回原版样式: " + ex.Message);
            }
        }

        internal static void Uninstall()
        {
            harmony?.UnpatchAll(PatchId);
            harmony = null;
        }

        /// <summary>返回 false = 本 mod 行已自行构建(跳过原版);其余放行。
        /// 数值行恒走三件套;带标记的开关/选项/CSV 行走 MarkedRow 轻接管;
        /// 无标记的非数值行与其余 mod 放行原版。</summary>
        private static bool BuildEntryPrefix(CustomUIBuilder b, ModConfig config, ConfigEntry entry)
        {
            try
            {
                if (config == null || entry == null || config.ModId != ModEntry.Id) return true;
                // 标记来源(0.6.0):Init/装配期由 PanelMarks.Rebuild 灌好,窗口
                // 构建期只读(非标记键返回 null)
                string mark = PanelMarks.For(entry.Key);
                if (entry.ValueType == typeof(int) && entry.HasRange)
                {
                    TripleRow(b, config, entry, whole: true, mark);
                    return false;
                }
                if (entry.ValueType == typeof(float) && entry.HasRange)
                {
                    TripleRow(b, config, entry, whole: false, mark);
                    return false;
                }
                if (mark != null)
                {
                    MarkedRow(b, config, entry, mark);
                    return false;
                }
                return true; // 无标记的 Toggle/Choice/Input 行与其余 mod 走原版
            }
            catch
            {
                return true; // 任何异常退回原版行,绝不让面板打不开
            }
        }

        private static void TripleRow(CustomUIBuilder b, ModConfig config, ConfigEntry entry, bool whole, string mark)
        {
            // 行首:label + ⓘ(对齐原版 BuildEntry 的样式;标记插在 label 文案后)
            string text = LabelText(config, entry, mark);
            b.BeginRow(4f, TextAnchor.MiddleLeft);
            b.AddLabel(text, "label_" + entry.Key);
            if (mark != null) EnsureRichText(b.Last);
            b.AddFlexibleSpace();

            // 滑条(id = entry.Key:Apply 管道按此 id 读值;min/max 来自 Define 的范围)
            float initial = whole
                ? Convert.ToInt32(entry.BoxedValue, CultureInfo.InvariantCulture)
                : Convert.ToSingle(entry.BoxedValue, CultureInfo.InvariantCulture);
            float min = entry.Min, max = entry.Max;
            b.AddSlider(min, max, initial, null, whole, entry.Key);
            b.Last.SetFixedWidth(150f);
            CustomUIElement slider = b.Last;

            // 实时数值 label
            b.AddLabel(FormatValue(initial, whole), "value_" + entry.Key);
            CustomUIElement valueLabel = b.Last;
            valueLabel.SetFixedWidth(44f);

            // 精调文本框(编辑结束才处理;id 独立,不参与 Apply)
            b.AddInput("", FormatValue(initial, whole), null, "input_" + entry.Key);
            CustomUIElement input = b.Last;
            input.SetFixedWidth(76f);

            // 拖动滑条 → 刷数值 label + 同步文本框(不触发 OnInput,无回环)
            slider.OnSlider(v =>
            {
                string shown = FormatValue(Clamp(v, min, max), whole);
                valueLabel.SetText(shown);
                input.SetString(shown, notify: false);
            });

            // 文本框编辑结束 → 解析+钳制 → 写滑条(notify=true 触发 OnSlider 链同步);
            // 解析失败 → 回显滑条现值
            input.OnInput(s =>
            {
                float target;
                if (whole)
                {
                    if (NumericInput.TryParseInt(s, Mathf.RoundToInt(min), Mathf.RoundToInt(max), out int iv))
                        target = iv;
                    else
                        target = float.NaN;
                }
                else if (NumericInput.TryParseFloat(s, min, max, out float fv))
                    target = fv;
                else
                    target = float.NaN;
                if (!float.IsNaN(target))
                    slider.SetFloat(target, notify: true);
                else
                    input.SetString(FormatValue(slider.GetFloat(), whole), notify: false);
            });

            b.End();
        }

        /// <summary>带标记的非数值行(开关/选项/CSV 文本,0.6.0 R-D):逐句复刻原版
        /// ModConfigWindow.BuildEntry 的控件构建(控件 id = entry.Key → Apply 管道
        /// 零改动),仅 label 文案追加标记。</summary>
        private static void MarkedRow(CustomUIBuilder b, ModConfig config, ConfigEntry entry, string mark)
        {
            b.BeginRow(4f, TextAnchor.MiddleLeft);
            b.AddLabel(LabelText(config, entry, mark), "label_" + entry.Key);
            if (mark != null) EnsureRichText(b.Last);
            b.AddFlexibleSpace();
            if (entry.ValueType == typeof(bool))
            {
                b.AddToggle("", (bool)entry.BoxedValue, null, entry.Key);
            }
            else if (entry.Choices != null && entry.Choices.Length != 0)
            {
                b.AddDropdown(entry.Choices,
                    Mathf.Max(0, Array.IndexOf(entry.Choices, ConfigEntry.Format(entry.BoxedValue))),
                    null, entry.Key);
                b.Last.SetFixedWidth(245f);
            }
            else
            {
                b.AddInput("", ConfigEntry.Format(entry.BoxedValue), null, entry.Key);
                b.Last.SetFixedWidth(245f);
            }
            b.End();
        }

        /// <summary>行首 label 文案(对齐原版 BuildEntry 的样式):label(缺席回退
        /// entry.Key)+ 标记富文本后缀段(插在 ⓘ 图标前)+ ⓘ tooltip 链接。</summary>
        private static string LabelText(ModConfig config, ConfigEntry entry, string mark)
        {
            string text = entry.Label?.Invoke() ?? entry.Key;
            string suffix = MarkSuffix(mark);
            if (suffix != null) text = text + suffix;
            if (entry.Tooltip != null)
            {
                string link = "modcfg:" + config.ModId + ":" + entry.Key;
                TMPIconHover.CustomLinks[link] = entry.Tooltip;
                text = text + " <link=\"" + link + "\"><sprite name=\"info\" tint=1></link>";
            }
            return text;
        }

        /// <summary>标记 → TMP 富文本后缀段(0x07 轮 R5):主 label 原样式不动,
        /// 标记文案包独立小字色段追加在原文后(不连带整项);版本不匹配灰
        /// #9e9e9e、让位柔和绿 #6fd073,字号 85%,段首空一格随小字号
        /// (空格放标签内,原文与后缀段之间不留全尺寸空隙)。构建期一次。</summary>
        private static string MarkSuffix(string mark)
        {
            if (mark == null) return null;
            string color = PanelMarks.IsVersionMark(mark) ? "#9e9e9e" : "#6fd073";
            return "<color=" + color + "><size=85%> " + mark + "</size></color>";
        }

        /// <summary>行首 label 的 richText 显式兜底(TMP 默认开;标记后缀段的
        /// color/size 标签与 ⓘ 图标的 link/sprite 标签都依赖富文本解析)。
        /// 构建期一次;失败零危害(退回默认设置)。</summary>
        private static void EnsureRichText(CustomUIElement label)
        {
            try
            {
                if (label.text != null) label.text.richText = true;
            }
            catch
            {
                // 富文本开关失败零危害:TMP 默认即开
            }
        }

        private static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        private static string FormatValue(float v, bool whole) => whole
            ? Mathf.RoundToInt(v).ToString(CultureInfo.InvariantCulture)
            : v.ToString(CultureInfo.InvariantCulture);
    }
}
