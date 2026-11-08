from pathlib import Path
import os, shutil

root = Path(os.environ["RUNNER_TEMP"])
src = root/"src"/"ProbablyStolenKoreanPack"
kp = src/"KoreanPack.cs"
wn = src/"WaterNameCompatibility.cs"

k = kp.read_text(encoding="utf-8")

old = """\t\t\tenabled = true;
\t\t\ttry
\t\t\t{
\t\t\t\twaterNames.Enable();"""
new = """\t\t\tenabled = true;
\t\t\tFishingStartPerkCompatibility.Enable();
\t\t\ttry
\t\t\t{
\t\t\t\twaterNames.Enable();"""
if old not in k: raise SystemExit("OnEnable anchor not found")
k = k.replace(old,new,1)

old = """\tpublic void OnDisable()
\t{
\t\tif (!enabled)"""
new = """\tpublic void OnDisable()
\t{
\t\tFishingStartPerkCompatibility.Disable();
\t\tif (!enabled)"""
if old not in k: raise SystemExit("OnDisable anchor not found")
k = k.replace(old,new,1)

old = """\tprivate void EnableLabelerConditionRepairLate()
\t{
\t\tif (!enabled || labelerHarmony != null)"""
new = """\tprivate void EnableLabelerConditionRepairLate()
\t{
\t\tRepairConditionDisplays(includeItems: true);
\t\tif (!enabled || labelerHarmony != null)"""
if old not in k: raise SystemExit("late labeler anchor not found")
k = k.replace(old,new,1)

old = """\t\tDebug.Log("[KoreanPack] Applied key translations: " + mod.Manifest.ID);
\t}
}"""
new = """\t\tDebug.Log("[KoreanPack] Applied key translations: " + mod.Manifest.ID);
\t\tFishingStartPerkCompatibility.Refresh();
\t}
}"""
if old not in k: raise SystemExit("Apply tail anchor not found")
k = k.replace(old,new,1)

anchor = """
\tprivate static bool IsOwn(LoadedMod mod)
"""
if anchor not in k: raise SystemExit("IsOwn anchor not found")
helpers = r"""
\tinternal static void RepairConditionDisplays(bool includeItems)
\t{
\t\ttry
\t\t{
\t\t\tvar locale = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale;
\t\t\tif (locale == null || (locale.Identifier.Code != "ko" && locale.Identifier.Code != "ko-KR"))
\t\t\t{
\t\t\t\treturn;
\t\t\t}
\t\t\tconst string gateKey = "mech_condition_low_purity";
\t\t\tif (!ValidMechanic(LocHelper.GetLocalizedMechanic(gateKey), gateKey))
\t\t\t{
\t\t\t\treturn;
\t\t\t}

\t\t\tint cached = 0;
\t\t\tint loadedItems = 0;

\t\t\tif (ItemConditionList.conditionLookup != null)
\t\t\t{
\t\t\t\tforeach (string category in new List<string>(ItemConditionList.conditionLookup.Keys))
\t\t\t\t{
\t\t\t\t\tcached += RepairFeatureCategory(category);
\t\t\t\t\tcached += RepairConditionCategory(category);
\t\t\t\t}
\t\t\t}

\t\t\tcached += RepairFeatureCategory("CATEGORY_FRESHNESS");
\t\t\tcached += RepairConditionList(ItemConditionList.conditionFoodFreshness);
\t\t\tcached += RepairFeatureCategory("CATEGORY_MODULE_STUCK");
\t\t\tcached += RepairConditionList(ItemConditionList.conditionModuleStuck);

\t\t\tif (IngotPurityHelper.ingotPurities != null)
\t\t\t{
\t\t\t\tforeach (var purity in IngotPurityHelper.ingotPurities.Values)
\t\t\t\t{
\t\t\t\t\tif (purity != null && RepairGenericCondition(purity.itemCondition))
\t\t\t\t\t{
\t\t\t\t\t\tcached++;
\t\t\t\t\t}
\t\t\t\t}
\t\t\t}

\t\t\tcached += RepairModuleEffectDisplays();
\t\t\tcached += RepairChemFinisherRecipeNames();

\t\t\tif (includeItems && PlayerStore.HasInstance)
\t\t\t{
\t\t\t\tforeach (GameItem item in PlayerStore.Instance.FindAllItem(isOwn: false))
\t\t\t\t{
\t\t\t\t\tif (item == null || item.itemFeatures == null) continue;
\t\t\t\t\tforeach (ItemFeature feature in item.itemFeatures)
\t\t\t\t\t{
\t\t\t\t\t\tif (feature == null) continue;
\t\t\t\t\t\tbool changed = RepairGenericCondition(feature.fakeCondition);
\t\t\t\t\t\tif (feature.fakeCondition != feature.realCondition)
\t\t\t\t\t\t{
\t\t\t\t\t\t\tchanged |= RepairGenericCondition(feature.realCondition);
\t\t\t\t\t\t}
\t\t\t\t\t\tif (!feature.useCondition)
\t\t\t\t\t\t{
\t\t\t\t\t\t\tstring before = feature.GetBaseFakeDisplay();
\t\t\t\t\t\t\tstring after = RepairPlainMechanicDisplay(before);
\t\t\t\t\t\t\tif (before != after)
\t\t\t\t\t\t\t{
\t\t\t\t\t\t\t\tfeature.SetPublicDisplay(after);
\t\t\t\t\t\t\t\tchanged = true;
\t\t\t\t\t\t\t}
\t\t\t\t\t\t\tbefore = feature.GetBaseRealDisplay();
\t\t\t\t\t\t\tafter = RepairPlainMechanicDisplay(before);
\t\t\t\t\t\t\tif (before != after)
\t\t\t\t\t\t\t{
\t\t\t\t\t\t\t\tfeature.SetActualDisplay(after);
\t\t\t\t\t\t\t\tchanged = true;
\t\t\t\t\t\t\t}
\t\t\t\t\t\t}
\t\t\t\t\t\tif (changed) loadedItems++;
\t\t\t\t\t}
\t\t\t\t}
\t\t\t}

\t\t\tif (cached + loadedItems > 0)
\t\t\t{
\t\t\t\tDebug.Log("[KoreanPack] Condition display repair: cached names=" + cached + ", loaded item labels=" + loadedItems + "; official load callbacks only.");
\t\t\t}
\t\t}
\t\tcatch (Exception ex)
\t\t{
\t\t\tDebug.LogWarning("[KoreanPack] Condition display repair skipped: " + ex.Message);
\t\t}
\t}

\tprivate static int RepairConditionList(List<ItemCondition> list)
\t{
\t\tif (list == null) return 0;
\t\tint count = 0;
\t\tforeach (ItemCondition condition in list)
\t\t{
\t\t\tif (RepairGenericCondition(condition)) count++;
\t\t}
\t\treturn count;
\t}

\tprivate static string RepairPlainMechanicDisplay(string value)
\t{
\t\tif (!TryGetMechanicErrorKey(value, out var key)) return value;
\t\tstring localized = LocHelper.GetLocalizedMechanic(key);
\t\treturn ValidMechanic(localized, key) ? localized : value;
\t}

\tprivate static int RepairModuleEffectDisplays()
\t{
\t\tif (ModuleEffectHelper.moduleEffects == null) return 0;
\t\tint count = 0;
\t\tforeach (var pair in ModuleEffectHelper.moduleEffects)
\t\t{
\t\t\tvar effect = pair.Value;
\t\t\tswitch (pair.Key)
\t\t\t{
\t\t\t\tcase "MODULE_EFFECT_PREMIUM": count += RepairModuleEffect(effect, "mech_module_effect_premium_name", "mech_module_effect_premium_desc", 20); break;
\t\t\t\tcase "MODULE_EFFECT_INFERIOR": count += RepairModuleEffect(effect, "mech_module_effect_inferior_name", "mech_module_effect_inferior_desc", 19); break;
\t\t\t\tcase "MODULE_EFFECT_RECEIVER": count += RepairModuleEffect(effect, "mech_module_effect_receiver_name", "mech_module_effect_receiver_desc", 10); break;
\t\t\t\tcase "MODULE_EFFECT_TOP_MOUNT": count += RepairModuleEffect(effect, "mech_module_effect_top_mount_name", "mech_module_effect_top_mount_desc", 20); break;
\t\t\t\tcase "MODULE_EFFECT_SIDE_MOUNT": count += RepairModuleEffect(effect, "mech_module_effect_side_mount_name", "mech_module_effect_side_mount_desc", 20); break;
\t\t\t\tcase "MODULE_EFFECT_OVERVOLTED": count += RepairModuleEffect(effect, "mech_module_effect_overvolted_name", "mech_module_effect_overvolted_desc", 200, 10); break;
\t\t\t\tcase "MODULE_EFFECT_DEGRADING": count += RepairModuleEffect(effect, "mech_module_effect_degrading_name", "mech_module_effect_degrading_desc", 1); break;
\t\t\t\tcase "MODULE_EFFECT_LEARNING_ALGORITHM": count += RepairModuleEffect(effect, "mech_module_effect_learning_algorithm_name", "mech_module_effect_learning_algorithm_desc", 1); break;
\t\t\t\tcase "MODULE_EFFECT_NEGATIVE_FEEDBACK": count += RepairModuleEffect(effect, "mech_module_effect_negative_feedback_name", "mech_module_effect_negative_feedback_desc", 25, 25); break;
\t\t\t\tcase "NODE_EFFECT_SUPPORT": count += RepairModuleEffect(effect, "mech_module_effect_support_node_name", "mech_module_effect_support_node_desc", 20); break;
\t\t\t}
\t\t}
\t\treturn count;
\t}

\tprivate static int RepairModuleEffect(ModuleEffectHelper.ModuleEffect effect, string nameKey, string descriptionKey, params object[] descriptionArgs)
\t{
\t\tif (effect == null) return 0;
\t\tint count = 0;
\t\tif (TryGetMechanicErrorKey(effect.displayName, out var badNameKey) && badNameKey == nameKey)
\t\t{
\t\t\tstring localized = LocHelper.GetLocalizedMechanic(nameKey);
\t\t\tif (ValidMechanic(localized, nameKey))
\t\t\t{
\t\t\t\teffect.displayName = localized;
\t\t\t\tcount++;
\t\t\t}
\t\t}
\t\tif (TryGetMechanicErrorKey(effect.description, out var badDescKey) && badDescKey == descriptionKey)
\t\t{
\t\t\tstring localized = LocHelper.GetLocalizedMechanic(descriptionKey, descriptionArgs);
\t\t\tif (ValidMechanic(localized, descriptionKey))
\t\t\t{
\t\t\t\teffect.description = localized;
\t\t\t\tcount++;
\t\t\t}
\t\t}
\t\treturn count;
\t}

\tprivate static int RepairChemFinisherRecipeNames()
\t{
\t\tif (ChemFinisherRecipe.chemFinisherRecipes == null) return 0;
\t\tint count = 0;
\t\tforeach (ChemFinisherRecipe recipe in ChemFinisherRecipe.chemFinisherRecipes)
\t\t{
\t\t\tif (recipe == null) continue;
\t\t\tstring key = ChemRecipeNameKey(recipe.identifier);
\t\t\tif (key == null || !TryGetAnyTranslationErrorKey(recipe.displayName, out var badKey) || badKey != key) continue;
\t\t\tstring localized = LocHelper.GetLocalizedItem(key);
\t\t\tif (ValidLocalized(localized, key))
\t\t\t{
\t\t\t\trecipe.displayName = localized;
\t\t\t\tcount++;
\t\t\t}
\t\t}
\t\treturn count;
\t}

\tprivate static string ChemRecipeNameKey(string identifier)
\t{
\t\tswitch (identifier)
\t\t{
\t\t\tcase "dream_dust": return "item_dream_dust_name";
\t\t\tcase "nightmare_dust": return "item_nightmare_dust_name";
\t\t\tcase "caffeine_pill": return "item_caffeine_pill_name";
\t\t\tcase "zyanide_pill": return "item_zyanide_pill_name";
\t\t\tcase "birth_control_pill": return "item_birth_control_pill_name";
\t\t\tcase "fanta_pill": return "item_fanta_pill_name";
\t\t\tcase "pink_alt_injector": return "item_pink_alt_injector_name";
\t\t\tdefault: return null;
\t\t}
\t}

\tprivate static bool TryGetAnyTranslationErrorKey(string value, out string key)
\t{
\t\tkey = null;
\t\tconst string prefix = "Translation Error '";
\t\tif (string.IsNullOrEmpty(value) || !value.StartsWith(prefix, StringComparison.Ordinal)) return false;
\t\tint end = value.IndexOf("' in ", prefix.Length, StringComparison.Ordinal);
\t\tif (end <= prefix.Length) return false;
\t\tkey = value.Substring(prefix.Length, end - prefix.Length);
\t\treturn !string.IsNullOrEmpty(key);
\t}

\tprivate static bool ValidLocalized(string value, string key)
\t{
\t\treturn !string.IsNullOrEmpty(value) && value != key && value.IndexOf("Translation Error", StringComparison.OrdinalIgnoreCase) < 0;
\t}
"""
k = k.replace(anchor, "\n"+helpers+anchor, 1)
k = k.replace("0.8.2-labeler-latefix", "0.8.2-cachefix", 1)
kp.write_text(k, encoding="utf-8")

s = wn.read_text(encoding="utf-8")
old = """\t\t\treplacements.Clear();
\t\t\tfor (int j = 0; j < array.Length; j++)"""
new = """\t\t\treplacements.Clear();
\t\t\tKoreanPack.RepairConditionDisplays(includeItems: false);
\t\t\tfor (int j = 0; j < array.Length; j++)"""
if old not in s: raise SystemExit("Water Complete anchor not found")
s = s.replace(old,new,1)
wn.write_text(s, encoding="utf-8")

fish = r"""using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

namespace ProbablyStolenKoreanPack;

internal static class FishingStartPerkCompatibility
{
\tprivate static bool enabled;
\tprivate static StartingPerk saved;
\tprivate static string oldName;
\tprivate static string oldDescription;
\tprivate static string appliedName;
\tprivate static string appliedDescription;
\tprivate static AsyncOperationHandle<LocalizationSettings> initialization;

\tinternal static void Enable()
\t{
\t\tif (enabled) return;
\t\tenabled = true;
\t\ttry
\t\t{
\t\t\tLocalizationSettings.SelectedLocaleChanged += LocaleChanged;
\t\t\tModHook.OnGameLoadedLate += Refresh;
\t\t\tinitialization = LocalizationSettings.InitializationOperation;
\t\t\tif (initialization.IsDone) Refresh();
\t\t\telse initialization.Completed += Initialized;
\t\t}
\t\tcatch (Exception ex)
\t\t{
\t\t\tDisable();
\t\t\tDebug.LogWarning("[KoreanPack] Fishing starting perk compatibility unavailable: " + ex.Message);
\t\t}
\t}

\tinternal static void Disable()
\t{
\t\tenabled = false;
\t\tLocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
\t\tModHook.OnGameLoadedLate -= Refresh;
\t\tif (initialization.IsValid()) initialization.Completed -= Initialized;
\t\tRestore();
\t}

\tprivate static void Initialized(AsyncOperationHandle<LocalizationSettings> operation) { Refresh(); }
\tprivate static void LocaleChanged(Locale locale) { Refresh(); }

\tprivate static void Restore()
\t{
\t\tif (saved != null)
\t\t{
\t\t\tif (saved.rawName == appliedName) saved.rawName = oldName;
\t\t\tif (saved.rawDescription == appliedDescription) saved.rawDescription = oldDescription;
\t\t}
\t\tsaved = null;
\t\toldName = null;
\t\toldDescription = null;
\t\tappliedName = null;
\t\tappliedDescription = null;
\t}

\tinternal static void Refresh()
\t{
\t\ttry
\t\t{
\t\t\tif (!enabled || !initialization.IsValid() || !initialization.IsDone || initialization.Status != AsyncOperationStatus.Succeeded) return;
\t\t\tLocale locale = LocalizationSettings.SelectedLocale;
\t\t\tif (locale == null) return;
\t\t\tstring code = locale.Identifier.Code;
\t\t\tif (code != "ko" && code != "ko-KR")
\t\t\t{
\t\t\t\tRestore();
\t\t\t\treturn;
\t\t\t}
\t\t\tLoadedMod fishingMod = null;
\t\t\tforeach (LoadedMod mod in ModLoader.Mods)
\t\t\t{
\t\t\t\tif (mod != null && mod.IsEnabled && mod.Manifest != null && mod.Manifest.ID == "ps-fishing-gameplay")
\t\t\t\t{
\t\t\t\t\tfishingMod = mod;
\t\t\t\t\tbreak;
\t\t\t\t}
\t\t\t}
\t\t\tDictionary<string, string> loc = null;
\t\t\tif (fishingMod == null || fishingMod.Localization == null ||
\t\t\t\t(!fishingMod.Localization.TryGetValue(code, out loc) && !fishingMod.Localization.TryGetValue("ko", out loc)) || loc == null)
\t\t\t{
\t\t\t\tRestore();
\t\t\t\treturn;
\t\t\t}
\t\t\tif (!loc.TryGetValue("gameplay.67f15a2ad1a0f439", out var name) ||
\t\t\t\t!loc.TryGetValue("gameplay.6f7353ff4a5869a4", out var description) ||
\t\t\t\tstring.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description)) return;
\t\t\tStartingPerk perk = StartingPerkList.GetModPerk("gf_simple_fishing");
\t\t\tif (perk == null || perk.source != "ps-fishing-gameplay") return;
\t\t\tif (saved != perk)
\t\t\t{
\t\t\t\tRestore();
\t\t\t\tsaved = perk;
\t\t\t\toldName = perk.rawName;
\t\t\t\toldDescription = perk.rawDescription;
\t\t\t}
\t\t\tif (perk.rawName == name && perk.rawDescription == description) return;
\t\t\tappliedName = name;
\t\t\tappliedDescription = description;
\t\t\tperk.rawName = name;
\t\t\tperk.rawDescription = description;
\t\t\tDebug.Log("[KoreanPack] Fishing starting perk names refreshed from Korean keys (official callbacks only).");
\t\t}
\t\tcatch (Exception ex)
\t\t{
\t\t\tDebug.LogWarning("[KoreanPack] Fishing starting perk refresh skipped: " + ex.Message);
\t\t}
\t}
}
"""
fish = fish.replace("\\\\t", "\\t")\n(src/"FishingStartPerkCompatibility.cs").write_text(fish,encoding="utf-8")

build = root/"build"
(build/"ProbablyStolenKoreanPack").mkdir(parents=True, exist_ok=True)
for f in [kp,wn,src/"FishingStartPerkCompatibility.cs"]:
    shutil.copy2(f,build/"ProbablyStolenKoreanPack"/f.name)

workspace = Path(os.environ["GITHUB_WORKSPACE"])
csproj = f"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <AssemblyName>ProbablyStolenKoreanPack</AssemblyName>
    <RootNamespace>ProbablyStolenKoreanPack</RootNamespace>
    <LangVersion>latest</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <Deterministic>true</Deterministic>
    <AssemblyVersion>0.8.2.0</AssemblyVersion>
    <FileVersion>0.8.2.3</FileVersion>
    <InformationalVersion>0.8.2-cachefix</InformationalVersion>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="ProbablyStolenKoreanPack/KoreanPack.cs" />
    <Compile Include="ProbablyStolenKoreanPack/WaterNameCompatibility.cs" />
    <Compile Include="ProbablyStolenKoreanPack/FishingStartPerkCompatibility.cs" />
    <Reference Include="Assembly-CSharp"><HintPath>{workspace}/probably-stolen/Managed/Assembly-CSharp.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="0Harmony"><HintPath>{workspace}/probably-stolen/Managed/0Harmony.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.Localization"><HintPath>{workspace}/probably-stolen/Managed/Unity.Localization.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.ResourceManager"><HintPath>{workspace}/probably-stolen/Managed/Unity.ResourceManager.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="UnityEngine.CoreModule"><HintPath>{workspace}/probably-stolen/Managed/UnityEngine.CoreModule.dll</HintPath><Private>false</Private></Reference>
  </ItemGroup>
</Project>"""
(build/"ProbablyStolenKoreanPack.csproj").write_text(csproj,encoding="utf-8")
print("patched cache repair sources")
