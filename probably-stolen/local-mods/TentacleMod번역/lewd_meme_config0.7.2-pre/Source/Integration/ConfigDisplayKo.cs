using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace LewdMemeConfig {
    internal static class ConfigDisplayKo {
        internal static string Translate(string s) {
            if (s == "顾客") return "고객";
            if (s == "价格") return "가격";
            if (s == "成长") return "성장";
            if (s == "掉落") return "드롭";
            if (s == "少女生成") return "소녀 생성";
            if (s == "闹钟") return "알람 시계";
            if (s == "陷阱") return "함정";
            if (s == "界面") return "인터페이스";
            if (s == "天赋") return "특성";
            if (s == "暗流交易联动") return "암류 거래 연동";
            if (s == "巢穴扩展联动") return "촉수 둥지 확장 연동";
            if (s == "掌上青楼联动") return "손안의 유곽 연동";
            if (s == "tearlaments 联动") return "tearlaments 연동";
            if (s == "物品高清立绘管理器联动") return "아이템 고해상도 초상화 관리자 연동";
            if (s == "开") return "켜기";
            if (s == "关" || s == "关闭") return "끄기";
            if (s == "不限制（原版）") return "제한 없음(원본)";
            if (s == "仅日本") return "일본만";
            if (s == "仅中国") return "중국만";
            if (s == "仅中日韩") return "한중일만";
            if (s == "仅女性（原版）") return "여성만(원본)";
            if (s == "不限性别") return "성별 제한 없음";
            if (s == "原版行为") return "원본 동작";
            if (s == "保留游戏原分布") return "원본 분포 유지";
            if (s == "仅普通顾客") return "일반 고객만";
            if (s == "顾客+特殊顾客") return "고객 + 특수 고객";
            if (s == "全体人类角色") return "모든 인간 캐릭터";
            return s;
        }

        internal static string Header(string s, LoadedMod loaded) {
            return loaded != null && loaded.Manifest != null &&
                loaded.Manifest.ID == "lewd_meme_config" ? Translate(s) : s;
        }
        internal static string[] Choices(string[] original, ModConfig config) {
            if (original == null || config == null || config.ModId != "lewd_meme_config") return original;
            string[] result = new string[original.Length];
            for (int i = 0; i < original.Length; i++) result[i] = Translate(original[i]);
            return result;
        }

        private static bool IsCall(CodeInstruction i, string type, string name) {
            MethodInfo m = i.operand as MethodInfo;
            return m != null && m.DeclaringType != null && m.DeclaringType.Name == type && m.Name == name &&
                (i.opcode == OpCodes.Call || i.opcode == OpCodes.Callvirt);
        }
        private static IEnumerable<CodeInstruction> HeaderTranspiler(IEnumerable<CodeInstruction> input) {
            var list = new List<CodeInstruction>(input);
            int anchor = -1;
            for (int i = 1; i + 3 < list.Count; i++) {
                if (list[i].opcode == OpCodes.Ldstr && (list[i].operand as string) == "section_" &&
                    IsCall(list[i+2], "String", "Concat") && IsCall(list[i+3], "CustomUIBuilder", "AddHeader")) {
                    if (anchor >= 0) throw new InvalidOperationException("Ambiguous config header");
                    anchor = i;
                }
            }
            if (anchor < 0 || list[anchor].labels.Count != 0 || list[anchor].blocks.Count != 0)
                throw new InvalidOperationException("Config header anchor changed");
            list.Insert(anchor, new CodeInstruction(OpCodes.Ldarg_0));
            list.Insert(anchor + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ConfigDisplayKo), "Header")));
            return list;
        }
        private static IEnumerable<CodeInstruction> ChoiceTranspiler(IEnumerable<CodeInstruction> input) {
            var list = new List<CodeInstruction>(input);
            int anchor = -1;
            for (int i = 0; i + 3 < list.Count; i++) {
                if (IsCall(list[i], "ConfigEntry", "get_Choices") && list[i+1].opcode == OpCodes.Ldc_I4_0 &&
                    IsCall(list[i+3], "ConfigEntry", "get_Choices")) {
                    if (anchor >= 0) throw new InvalidOperationException("Ambiguous choice array");
                    anchor = i+1;
                }
            }
            if (anchor < 0 || list[anchor].labels.Count != 0 || list[anchor].blocks.Count != 0)
                throw new InvalidOperationException("Choice array anchor changed");
            list.Insert(anchor, new CodeInstruction(OpCodes.Ldarg_1));
            list.Insert(anchor+1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ConfigDisplayKo), "Choices")));
            return list;
        }
        internal static void Install(Harmony harmony, Action<string> log) {
            try {
                harmony.Patch(ModReflect.Target(typeof(ModConfigWindow), "Open"),
                    transpiler: new HarmonyMethod(typeof(ConfigDisplayKo), "HeaderTranspiler"));
                harmony.Patch(ModReflect.Target(typeof(ModConfigWindow), "BuildEntry"),
                    transpiler: new HarmonyMethod(typeof(ConfigDisplayKo), "ChoiceTranspiler"));
            } catch (Exception ex) {
                if (log != null) log("설정 표시 전용 한국어 패치 설치 실패: " + ex.Message);
            }
        }
    }
}
