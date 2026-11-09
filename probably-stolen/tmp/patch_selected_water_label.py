from pathlib import Path
import os

p = Path(os.environ["RUNNER_TEMP"]) / "src" / "ProbablyStolenKoreanPack" / "WaterNameCompatibility.cs"
s = p.read_text(encoding="utf-8")

def replace_one(old, new):
    global s
    if old not in s:
        raise SystemExit("Source anchor not found: " + old[:90])
    s = s.replace(old, new, 1)

replace_one(
    "using System.Collections.Generic;\n",
    "using System.Collections.Generic;\nusing HarmonyLib;\n"
)

replace_one(
    "\tprivate bool assetsReady;\n\n\tprivate int generation;",
    """\tprivate bool assetsReady;

\tprivate bool tooltipRepairLogged;

\tprivate bool nativeWaterRepairLogged;

\tprivate static WaterNameCompatibility activeInstance;

\tprivate int generation;"""
)

replace_one(
    """\tprivate static readonly string[] QualityKeys = new string[6] { "mech_condition_pure_water", "mech_condition_high_quality_water", "mech_condition_basewater", "mech_condition_ghostwater", "mech_condition_rustwater", "mech_condition_gutterflow" };""",
    """\tprivate static readonly string[] QualityKeys = new string[6] { "mech_condition_pure_water", "mech_condition_high_quality_water", "mech_condition_basewater", "mech_condition_ghostwater", "mech_condition_rustwater", "mech_condition_gutterflow" };

\tprivate static readonly string[] KoreanQualityNames = new string[6] { "순수한 물", "고품질 물", "기본수", "유령수", "녹물", "하수" };"""
)

replace_one(
    """\t\t\tenabled = true;
\t\t\tLocalizationSettings.SelectedLocaleChanged += LocaleChanged;""",
    """\t\t\tenabled = true;
\t\t\tactiveInstance = this;
\t\t\tLocalizationSettings.SelectedLocaleChanged += LocaleChanged;"""
)

replace_one(
    """\t\t\tModHook.OnGameLoadedLate += Loaded;
\t\t\tinitialization = LocalizationSettings.InitializationOperation;""",
    """\t\t\tModHook.OnGameLoadedLate += Loaded;
\t\t\tModHook.OnCreateTooltipEarly += BeforeWaterTooltip;
\t\t\tinitialization = LocalizationSettings.InitializationOperation;"""
)

replace_one(
    """\t\tenabled = false;
\t\tworldReady = false;""",
    """\t\tenabled = false;
\t\tif (activeInstance == this)
\t\t{
\t\t\tactiveInstance = null;
\t\t}
\t\tworldReady = false;"""
)

replace_one(
    """\t\tModHook.OnGameLoadedLate -= Loaded;
\t\tif (initialization.IsValid())""",
    """\t\tModHook.OnGameLoadedLate -= Loaded;
\t\tModHook.OnCreateTooltipEarly -= BeforeWaterTooltip;
\t\tif (initialization.IsValid())"""
)

method = r'''
    // Restore the six water labels before the game's native tooltip is created.
    private void BeforeWaterTooltip(RichTextBuilder builder, GameItem item)
    {
        if (!enabled || item == null || item.itemFeatures == null)
        {
            return;
        }
        Locale locale = LocalizationSettings.SelectedLocale;
        if (locale == null || (locale.Identifier.Code != "ko" && locale.Identifier.Code != "ko-KR"))
        {
            return;
        }
        try
        {
            if (!item.IsTag("LIQUID_CONTAINER_TAG") && item.FindItemFeatureByCategory("CATEGORY_WATER_PURITY") == null)
            {
                return;
            }
            int changed = 0;
            foreach (ItemFeature feature in item.itemFeatures)
            {
                if (feature == null)
                {
                    continue;
                }
                bool repaired = false;
                if (feature.fakeCondition != null)
                {
                    string before = feature.fakeCondition.display;
                    string after = Repair(before);
                    if (before != after)
                    {
                        feature.fakeCondition.display = after;
                        repaired = true;
                    }
                }
                if (feature.realCondition != null && feature.realCondition != feature.fakeCondition)
                {
                    string before = feature.realCondition.display;
                    string after = Repair(before);
                    if (before != after)
                    {
                        feature.realCondition.display = after;
                        repaired = true;
                    }
                }
                if (!feature.useCondition)
                {
                    string before = feature.GetBaseFakeDisplay();
                    string after = Repair(before);
                    if (before != after)
                    {
                        feature.SetPublicDisplay(after);
                        repaired = true;
                    }
                    before = feature.GetBaseRealDisplay();
                    after = Repair(before);
                    if (before != after)
                    {
                        feature.SetActualDisplay(after);
                        repaired = true;
                    }
                }
                if (repaired)
                {
                    changed++;
                    if (!tooltipRepairLogged)
                    {
                        Debug.Log("[KoreanPack] Water tooltip label repaired: " + feature.category);
                    }
                }
            }
            if (changed > 0)
            {
                tooltipRepairLogged = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[KoreanPack] Water tooltip label repair skipped: " + ex.Message);
        }
    }

    // Other mods can restore cached water errors after the early tooltip callback.
    // Repair only the core-game water feature, after their Harmony postfixes.
    internal static void RestoreWaterFeatureAfterMods(ItemFeature feature)
    {
        WaterNameCompatibility self = activeInstance;
        if (self == null || !self.enabled || feature == null || feature.category != "CATEGORY_WATER_PURITY")
        {
            return;
        }
        Locale locale = LocalizationSettings.SelectedLocale;
        if (locale == null || (locale.Identifier.Code != "ko" && locale.Identifier.Code != "ko-KR"))
        {
            return;
        }
        bool changed = self.RepairCondition(feature.fakeCondition);
        if (feature.realCondition != feature.fakeCondition)
        {
            changed |= self.RepairCondition(feature.realCondition);
        }
        if (changed && !self.nativeWaterRepairLogged)
        {
            self.nativeWaterRepairLogged = true;
            Debug.Log("[KoreanPack] Native water feature repaired after other mods; feature=" + feature.category);
        }
    }

'''
replace_one("\tprivate void Request()\n", method.replace("    ", "\t") + "\tprivate void Request()\n")

replace_one(
    """\t\tforeach (KeyValuePair<string, string> replacement in replacements)
\t\t{
\t\t\tvalue = value.Replace(replacement.Key, replacement.Value);
\t\t}
\t\treturn value;""",
    """\t\tforeach (KeyValuePair<string, string> replacement in replacements)
\t\t{
\t\t\tvalue = value.Replace(replacement.Key, replacement.Value);
\t\t}
\t\tfor (int i = 0; i < QualityKeys.Length; i++)
\t\t{
\t\t\tvalue = value.Replace(Error(QualityKeys[i]), KoreanQualityNames[i]);
\t\t}
\t\treturn value;"""
)

s += r'''

[HarmonyPatch(typeof(WaterFeatureHelper), nameof(WaterFeatureHelper.GetWaterFeature))]
internal static class CoreGameWaterFeaturePostfix
{
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(ItemFeature __result)
    {
        WaterNameCompatibility.RestoreWaterFeatureAfterMods(__result);
    }
}
'''
p.write_text(s, encoding="utf-8")
print("Applied label-only water tooltip and native water-feature repair.")
