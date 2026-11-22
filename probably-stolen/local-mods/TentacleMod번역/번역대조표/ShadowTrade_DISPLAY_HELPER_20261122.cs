using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MidnightPawn.Game {
    internal static class ShadowTradeDisplayKo {
        internal static string Display(string s) {
            if (s == "下层区") return "하층 구역";
            if (s == "上层区") return "상층 구역";
            if (s == "治安部") return "치안부";
            if (s == "黑市") return "암시장";
            if (s == "革命军") return "혁명군";
            if (s == "拾荒者") return "폐품 수집가";
            if (s == "雇佣兵") return "용병";
            if (s == "工程师") return "엔지니어";
            if (s == "农场主") return "농장주";
            if (s == "清洁员") return "청소부";
            if (s == "走私者") return "밀수업자";
            if (s == "货运员") return "화물 운송인";
            if (s == "维修工") return "정비공";
            if (s == "研究员") return "연구원";
            if (s == "矿工") return "광부";
            if (s == "护士") return "간호사";
            if (s == "医生") return "의사";
            if (s == "厨师") return "요리사";
            if (s == "小偷") return "도둑";
            if (s == "商人") return "상인";
            if (s == "技师") return "기술자";
            if (s == "文员") return "사무원";
            if (s == "酒保") return "바텐더";
            if (s == "学徒") return "견습생";
            if (s == "艺人") return "예술인";
            if (s == "萝莉") return "로리";
            if (s == "少女") return "소녀";
            if (s == "御姐") return "누님";
            if (s == "少妇") return "젊은 부인";
            if (s == "熟女") return "성숙한 여성";
            if (s == "贫乳") return "작은 가슴";
            if (s == "普乳") return "보통 가슴";
            if (s == "巨乳") return "큰 가슴";
            if (s == "处女") return "처녀";
            if (s == "双处女") return "두 처녀 상태";
            if (s == "肛门处女") return "항문 처녀";
            if (s == "非处女") return "비처녀";
            if (s == "收购商") return "매입상";
            if (s == "下层区收购商") return "하층 구역 매입상";
            if (s == "上层区收购商") return "상층 구역 매입상";
            if (s == "治安部收购商") return "치안부 매입상";
            if (s == "黑市收购商") return "암시장 매입상";
            if (s == "革命军收购商") return "혁명군 매입상";
            return s;
        }
        internal static string CustomerName(StoreClient client) {
            string name = client.displayName;
            return client.sourceModId == "lewd_meme_remake" ? Display(name) : name;
        }
        internal static string[] ChoiceLabels(string[] values, ModConfig config) {
            if (values == null || config == null || config.ModId != "lewd_meme_remake") return values;
            var shown = new string[values.Length];
            for (int i = 0; i < values.Length; i++) {
                shown[i] = values[i] == "开" ? "켜기" : values[i] == "关" ? "끄기" : values[i];
            }
            return shown;
        }
        private static bool IsCall(CodeInstruction i, string type, string name) {
            var m = i.operand as MethodInfo;
            return m != null && m.DeclaringType != null && m.DeclaringType.Name == type && m.Name == name &&
                (i.opcode == OpCodes.Call || i.opcode == OpCodes.Callvirt);
        }
        private static IEnumerable<CodeInstruction> ChoiceTranspiler(IEnumerable<CodeInstruction> input) {
            var list = new List<CodeInstruction>(input);
            var hits = new List<int>();
            for (int i = 0; i < list.Count; i++)
                if (IsCall(list[i], "ConfigEntry", "get_Choices")) hits.Add(i);
            if (hits.Count != 4) throw new InvalidOperationException("Config choices signature changed");
            int anchor = hits[2]+1;
            list.Insert(anchor, new CodeInstruction(OpCodes.Ldarg_1));
            list.Insert(anchor+1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ShadowTradeDisplayKo), "ChoiceLabels")));
            return list;
        }
        private static IEnumerable<CodeInstruction> CustomerTranspiler(IEnumerable<CodeInstruction> input, MethodBase original) {
            var list = new List<CodeInstruction>(input);
            int n = 0;
            foreach (var ins in list) {
                var f = ins.operand as FieldInfo;
                if (ins.opcode != OpCodes.Ldfld || f == null || f.DeclaringType != typeof(StoreClient) || f.Name != "displayName") continue;
                ins.opcode = OpCodes.Call;
                ins.operand = AccessTools.Method(typeof(ShadowTradeDisplayKo), "CustomerName");
                n++;
            }
            int want = original.Name == "DisplayClientText" ? 2 : 1;
            if (n != want) throw new InvalidOperationException("Customer name UI anchor changed: " + original.Name);
            return list;
        }
        internal static void Install() {
            var harmony = new Harmony("lewd_meme_remake.ko.display");
            try {
                harmony.Patch(AccessTools.Method(typeof(ModConfigWindow), "BuildEntry"), transpiler: new HarmonyMethod(typeof(ShadowTradeDisplayKo), "ChoiceTranspiler"));
                harmony.Patch(AccessTools.Method(AccessTools.TypeByName("DialogUIManager"), "DisplayClientText"), transpiler: new HarmonyMethod(typeof(ShadowTradeDisplayKo), "CustomerTranspiler"));
                harmony.Patch(AccessTools.Method(AccessTools.TypeByName("DialogUIManager"), "PlayClientSingleText"), transpiler: new HarmonyMethod(typeof(ShadowTradeDisplayKo), "CustomerTranspiler"));
                harmony.Patch(AccessTools.Method(AccessTools.TypeByName("DialoguePhoneUIManager"), "PlayClientSingleText"), transpiler: new HarmonyMethod(typeof(ShadowTradeDisplayKo), "CustomerTranspiler"));
                harmony.Patch(AccessTools.Method(AccessTools.TypeByName("NegociationUIManager"), "RefreshClientHeader"), transpiler: new HarmonyMethod(typeof(ShadowTradeDisplayKo), "CustomerTranspiler"));
            } catch (Exception ex) {
                harmony.UnpatchAll("lewd_meme_remake.ko.display");
                UnityEngine.Debug.LogWarning("[ShadowTrade KO] Display patch installation failed: " + ex.Message);
            }
        }
        internal static void Uninstall() {
            new Harmony("lewd_meme_remake.ko.display").UnpatchAll("lewd_meme_remake.ko.display");
        }
    }
}
