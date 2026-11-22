using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace RedLightDistrict.Korean {
    internal static class DisplayBoundaryKo {
        // Build-time stubs: rebinding is verified by the patcher.
        internal static string LookupStub(string key, string original) { return original; }
        internal static string PatronIdStub() { return null; }
        internal static string DisplayTrait(string value) {
            if (value == "万人迷") return LookupStub("rld_display_102f641e1e10fea431ff", value);
            if (value == "差评服务") return LookupStub("rld_display_bc03e08b46482b7a2966", value);
            if (value == "裁缝") return LookupStub("rld_display_d8eb5bc17c64c004f95c", value);
            if (value == "顺手牵羊") return LookupStub("rld_display_148447ccc5bc854a4aaf", value);
            if (value == "精力充沛") return LookupStub("rld_display_9db7c70220dc08c81c16", value);
            if (value == "无师自通") return LookupStub("rld_display_dfcee520a3b8f07a039a", value);
            if (value == "爱干净") return LookupStub("rld_display_8f69da9b8bf553c30e52", value);
            if (value == "节约") return LookupStub("rld_display_5b532e0d9e8ac20a7efa", value);
            if (value == "榨取") return LookupStub("rld_display_3c265c6df72141d7167d", value);
            if (value == "销冠") return LookupStub("rld_display_1b0ec971311878278680", value);
            if (value == "绝色") return LookupStub("rld_display_b7847986338caf835fd9", value);
            if (value == "曼妙") return LookupStub("rld_display_2cb9d5a1b289d47e2baa", value);
            if (value == "优雅") return LookupStub("rld_display_82783ec552db3983a8e7", value);
            if (value == "听话") return LookupStub("rld_display_e98b511ac9df76a6d02c", value);
            if (value == "傻白甜") return LookupStub("rld_display_0f7aa46fdaeb993ddbea", value);
            if (value == "抖M") return LookupStub("rld_display_ba93355a20373b99953e", value);
            if (value == "玉玉症") return LookupStub("rld_display_b1bbb45b21f7784f770a", value);
            if (value == "洁癖") return LookupStub("rld_display_a3a3764ff0eac74a7009", value);
            if (value == "暴食") return LookupStub("rld_display_62de8b20264b1b63741e", value);
            if (value == "懒惰") return LookupStub("rld_display_373d080758162dc6c79b", value);
            if (value == "自残") return LookupStub("rld_display_d91049754e992c578424", value);
            if (value == "笨拙") return LookupStub("rld_display_9b49366c057e1fd0769b", value);
            return value;
        }
        internal static string DisplayMaid(string value) {
            return value == "女仆" ? LookupStub("rld_display_maid_name", value) : value;
        }
        internal static string DisplayPatronName(string value) {
            const string raw = "寻欢客（";
            if (value == null || !value.StartsWith(raw, StringComparison.Ordinal)) return value;
            string prefix = LookupStub("rld_display_patron_prefix", raw);
            if (prefix == raw) return value;
            string rest = value.Substring(raw.Length);
            if (rest.EndsWith("）", StringComparison.Ordinal)) rest = rest.Substring(0, rest.Length - 1) + ")";
            return prefix + rest;
        }
        internal static bool ContainsEither(string value, string raw, string shown) {
            return value.Contains(raw) || value.Contains(shown);
        }
        internal static bool StartsWithEither(string value, string raw, string shown) {
            return value.StartsWith(raw) || value.StartsWith(shown);
        }
        internal static int IndexEither(string value, string raw, string shown) {
            int i = value.IndexOf(raw), j = value.IndexOf(shown);
            return i < 0 ? j : j < 0 ? i : Math.Min(i, j);
        }
        internal static int IndexEitherOrdinal(string value, string raw, string shown, StringComparison comparison) {
            int i = value.IndexOf(raw, comparison), j = value.IndexOf(shown, comparison);
            return i < 0 ? j : j < 0 ? i : Math.Min(i, j);
        }
        internal static string CustomerName(StoreClient client, string value) {
            if (client == null || string.IsNullOrEmpty(client.identifier) || !string.Equals(client.identifier, PatronIdStub(), StringComparison.Ordinal)) return value;
            return DisplayPatronName(value);
        }
        private static IEnumerable<CodeInstruction> CustomerTranspiler(IEnumerable<CodeInstruction> input, MethodBase original) {
            var list = new List<CodeInstruction>(input);
            int count = 0;
            for (int i = list.Count - 1; i >= 0; i--) {
                CodeInstruction ins = list[i];
                var field = ins.operand as FieldInfo;
                if (ins.opcode != OpCodes.Ldfld || field == null || field.DeclaringType != typeof(StoreClient) || field.Name != "displayName") continue;
                if (ins.blocks.Count != 0) throw new InvalidOperationException("RLD display anchor has exception boundary");
                var dup = new CodeInstruction(OpCodes.Dup);
                dup.labels.AddRange(ins.labels);
                ins.labels.Clear();
                list.Insert(i, dup);
                list.Insert(i+2, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DisplayBoundaryKo), "CustomerName")));
                count++;
            }
            int expected = original.Name == "DisplayClientText" ? 2 : 1;
            if (count != expected) throw new InvalidOperationException("RLD name display anchor changed: " + original.Name);
            return list;
        }
        internal static void Install() {
            var h = new Harmony("red_light_district.ko.display");
            try {
                h.UnpatchAll("red_light_district.ko.display");
                var transpiler = new HarmonyMethod(typeof(DisplayBoundaryKo), "CustomerTranspiler") { priority = Priority.High };
                h.Patch(AccessTools.Method(AccessTools.TypeByName("DialogUIManager"), "DisplayClientText"), transpiler: transpiler);
                h.Patch(AccessTools.Method(AccessTools.TypeByName("DialogUIManager"), "PlayClientSingleText"), transpiler: transpiler);
                h.Patch(AccessTools.Method(AccessTools.TypeByName("DialoguePhoneUIManager"), "PlayClientSingleText"), transpiler: transpiler);
                h.Patch(AccessTools.Method(AccessTools.TypeByName("NegociationUIManager"), "RefreshClientHeader"), transpiler: transpiler);
            } catch (Exception ex) {
                h.UnpatchAll("red_light_district.ko.display");
                UnityEngine.Debug.LogWarning("[RLD KO] Display patch installation failed: " + ex.Message);
            }
        }
        internal static void Uninstall() {
            try { new Harmony("red_light_district.ko.display").UnpatchAll("red_light_district.ko.display"); }
            catch (Exception ex) { UnityEngine.Debug.LogWarning("[RLD KO] Removal failed: " + ex.Message); }
        }
    }
}
