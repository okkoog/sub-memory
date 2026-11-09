using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace ProbablyStolenKoreanPack;

public sealed class KoreanPack : IMod
{
	private sealed class Catalog
	{
		internal readonly HashSet<string> Aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		internal readonly Dictionary<string, string> Entries = new Dictionary<string, string>(StringComparer.Ordinal);
	}

	private sealed class Original
	{
		internal bool Existed;

		internal string Value;

		internal string Applied;
	}

	private const string Id = "probably_stolen_korean_pack";

	private readonly List<Catalog> catalogs = new List<Catalog>();

	private readonly Dictionary<Dictionary<string, string>, Dictionary<string, Original>> originals = new Dictionary<Dictionary<string, string>, Dictionary<string, Original>>();

	private readonly WaterNameCompatibility waterNames = new WaterNameCompatibility();

	private const string LabelerHarmonyId = "probably_stolen_korean_pack.labeler_condition_repair";

	private Harmony labelerHarmony;

	private bool enabled;

	private bool ready;

	public void Init(ModManifest manifest)
	{
	}

	public void OnEnable()
	{
		if (!enabled)
		{
			enabled = true;
			FishingStartPerkCompatibility.Enable();
			try
			{
				waterNames.Enable();
			}
			catch (Exception ex)
			{
				waterNames.Disable();
				Debug.LogWarning((object)("[KoreanPack] Water name compatibility unavailable: " + ex.Message));
			}
			ModHook.OnGameLoadedLate += EnableLabelerConditionRepairLate;
			ModHook.OnModAssetsLoaded += AssetsLoaded;
			LoadedMod val = ((IEnumerable<LoadedMod>)ModLoader.Mods).FirstOrDefault((Func<LoadedMod, bool>)IsOwn);
			if (val != null && val.Localization != null && val.Localization.Count > 0)
			{
				Build(val);
				ApplyAll();
				waterNames.AssetsReady();
			}
			Debug.Log((object)"[KoreanPack] Key localization + event-based localization cache repair enabled (0.8.2-cachefix3-safe).");
		}
	}

	public void OnDisable()
	{
		FishingStartPerkCompatibility.Disable();
		if (!enabled)
		{
			return;
		}
		enabled = false;
		ModHook.OnGameLoadedLate -= EnableLabelerConditionRepairLate;
		DisableLabelerConditionRepair();
		waterNames.Disable();
		ModHook.OnModAssetsLoaded -= AssetsLoaded;
		foreach (KeyValuePair<Dictionary<string, string>, Dictionary<string, Original>> original in originals)
		{
			foreach (KeyValuePair<string, Original> item in original.Value)
			{
				if (original.Key.TryGetValue(item.Key, out var value) && !(value != item.Value.Applied))
				{
					if (item.Value.Existed)
					{
						original.Key[item.Key] = item.Value.Value;
					}
					else
					{
						original.Key.Remove(item.Key);
					}
				}
			}
		}
		originals.Clear();
		catalogs.Clear();
		ready = false;
	}

	private void EnableLabelerConditionRepairLate()
	{
		RepairConditionDisplays(includeItems: true);
		try
		{
			int num = RepairChemFinisherRecipeNames();
			if (num > 0)
			{
				Debug.Log((object)("[KoreanPack] Late Item cache repair: chem finisher recipe names=" + num + "."));
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)("[KoreanPack] Late Chem Finisher cache repair skipped: " + ex.Message));
		}
		if (!enabled || labelerHarmony != null)
		{
			return;
		}
		try
		{
			EnableLabelerConditionRepair();
			Debug.Log((object)"[KoreanPack] Labeler condition repair armed after game load.");
		}
		catch (Exception ex2)
		{
			DisableLabelerConditionRepair();
			Debug.LogWarning((object)("[KoreanPack] Labeler condition repair unavailable: " + ex2.Message));
		}
	}

	private void EnableLabelerConditionRepair()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected Obj, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected Obj, but got Unknown
		if (labelerHarmony == null)
		{
			MethodInfo methodInfo = AccessTools.Method(typeof(LabelerElement), "InitElement", new Type[1] { typeof(ItemFeature) }, (Type[])null);
			MethodInfo methodInfo2 = AccessTools.Method(typeof(KoreanPack), "BeforeLabelerInit", (Type[])null, (Type[])null);
			if (methodInfo == null || methodInfo2 == null)
			{
				throw new MissingMethodException("LabelerElement.InitElement repair target not found.");
			}
			labelerHarmony = new Harmony("probably_stolen_korean_pack.labeler_condition_repair");
			labelerHarmony.Patch((MethodBase)methodInfo, new HarmonyMethod(methodInfo2), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
		}
	}

	private void DisableLabelerConditionRepair()
	{
		if (labelerHarmony != null)
		{
			try
			{
				labelerHarmony.UnpatchAll("probably_stolen_korean_pack.labeler_condition_repair");
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[KoreanPack] Labeler condition repair cleanup failed: " + ex.Message));
			}
			labelerHarmony = null;
		}
	}

	private static void BeforeLabelerInit(ItemFeature itemFeature)
	{
		try
		{
			int num = RepairForLabeler(itemFeature);
			if (num > 0)
			{
				Debug.Log((object)("[KoreanPack] Labeler condition cache repaired on open: " + num));
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)("[KoreanPack] Labeler condition repair skipped: " + ex.Message));
		}
	}

	private static int RepairForLabeler(ItemFeature itemFeature)
	{
		if (itemFeature == null)
		{
			return 0;
		}
		int num = RepairFeatureCategory(itemFeature.category);
		num += RepairConditionCategory(itemFeature.category);
		if (RepairGenericCondition(itemFeature.fakeCondition))
		{
			num++;
		}
		if (itemFeature.realCondition != itemFeature.fakeCondition && RepairGenericCondition(itemFeature.realCondition))
		{
			num++;
		}
		return num;
	}

	private static int RepairFeatureCategory(string category)
	{
		if (string.IsNullOrEmpty(category) || ItemFeatureList.CategoryNames == null || !ItemFeatureList.CategoryNames.TryGetValue(category, out var value) || !TryGetMechanicErrorKey(value, out var key))
		{
			return 0;
		}
		string localizedMechanic = LocHelper.GetLocalizedMechanic(key, Array.Empty<object>());
		if (!ValidMechanic(localizedMechanic, key))
		{
			return 0;
		}
		ItemFeatureList.CategoryNames[category] = localizedMechanic;
		return 1;
	}

	private static int RepairConditionCategory(string category)
	{
		if (string.IsNullOrEmpty(category) || ItemConditionList.conditionLookup == null || !ItemConditionList.conditionLookup.TryGetValue(category, out var value) || value == null)
		{
			return 0;
		}
		int num = 0;
		foreach (ItemCondition item in value)
		{
			if (RepairGenericCondition(item))
			{
				num++;
			}
		}
		return num;
	}

	private static bool RepairGenericCondition(ItemCondition condition)
	{
		if (condition == null || !TryGetMechanicErrorKey(condition.display, out var key))
		{
			return false;
		}
		string localizedMechanic = LocHelper.GetLocalizedMechanic(key, Array.Empty<object>());
		if (!ValidMechanic(localizedMechanic, key))
		{
			return false;
		}
		condition.display = localizedMechanic;
		return true;
	}

	private static bool TryGetMechanicErrorKey(string value, out string key)
	{
		key = null;
		if (string.IsNullOrEmpty(value) || !value.StartsWith("Translation Error '", StringComparison.Ordinal) || !value.EndsWith("' in Mechanic", StringComparison.Ordinal))
		{
			return false;
		}
		int num = value.Length - "Translation Error '".Length - "' in Mechanic".Length;
		if (num <= 0)
		{
			return false;
		}
		key = value.Substring("Translation Error '".Length, num);
		return !string.IsNullOrEmpty(key);
	}

	private static bool ValidMechanic(string value, string key)
	{
		if (!string.IsNullOrEmpty(value) && value != key)
		{
			return value.IndexOf("Translation Error", StringComparison.OrdinalIgnoreCase) < 0;
		}
		return false;
	}

	internal static void RepairConditionDisplays(bool includeItems)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Locale selectedLocale = LocalizationSettings.SelectedLocale;
			if ((Object)(object)selectedLocale == (Object)null)
			{
				return;
			}
			LocaleIdentifier identifier = selectedLocale.Identifier;
			if (identifier.Code != "ko")
			{
				identifier = selectedLocale.Identifier;
				if (identifier.Code != "ko-KR")
				{
					return;
				}
			}
			if (!ValidMechanic(LocHelper.GetLocalizedMechanic("mech_condition_low_purity", Array.Empty<object>()), "mech_condition_low_purity"))
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			if (ItemConditionList.conditionLookup != null)
			{
				foreach (string item in new List<string>(ItemConditionList.conditionLookup.Keys))
				{
					num += RepairFeatureCategory(item);
					num += RepairConditionCategory(item);
				}
			}
			num += RepairFeatureCategory("CATEGORY_FRESHNESS");
			num += RepairConditionList(ItemConditionList.conditionFoodFreshness);
			num += RepairFeatureCategory("CATEGORY_MODULE_STUCK");
			num += RepairConditionList(ItemConditionList.conditionModuleStuck);
			if (IngotPurityHelper.ingotPurities != null)
			{
				foreach (IngotPurity value in IngotPurityHelper.ingotPurities.Values)
				{
					if (value != null && RepairGenericCondition(value.itemCondition))
					{
						num++;
					}
				}
			}
			num += RepairModuleEffectDisplays();
			num += RepairChemFinisherRecipeNames();
			if (includeItems && PlayerStore.HasInstance)
			{
				foreach (GameItem item2 in PlayerStore.Instance.FindAllItem(false))
				{
					if (item2 == null || item2.itemFeatures == null)
					{
						continue;
					}
					foreach (ItemFeature itemFeature in item2.itemFeatures)
					{
						if (itemFeature == null)
						{
							continue;
						}
						bool flag = RepairGenericCondition(itemFeature.fakeCondition);
						if (itemFeature.fakeCondition != itemFeature.realCondition)
						{
							flag |= RepairGenericCondition(itemFeature.realCondition);
						}
						if (!itemFeature.useCondition)
						{
							string baseFakeDisplay = itemFeature.GetBaseFakeDisplay();
							string text = RepairPlainMechanicDisplay(baseFakeDisplay);
							if (baseFakeDisplay != text)
							{
								itemFeature.SetPublicDisplay(text);
								flag = true;
							}
							string baseRealDisplay = itemFeature.GetBaseRealDisplay();
							text = RepairPlainMechanicDisplay(baseRealDisplay);
							if (baseRealDisplay != text)
							{
								itemFeature.SetActualDisplay(text);
								flag = true;
							}
						}
						if (flag)
						{
							num2++;
						}
					}
				}
			}
			if (num + num2 > 0)
			{
				Debug.Log((object)("[KoreanPack] Condition display repair: cached names=" + num + ", loaded item labels=" + num2 + "; official load callbacks only."));
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)("[KoreanPack] Condition display repair skipped: " + ex.Message));
		}
	}

	private static int RepairConditionList(List<ItemCondition> list)
	{
		if (list == null)
		{
			return 0;
		}
		int num = 0;
		foreach (ItemCondition item in list)
		{
			if (RepairGenericCondition(item))
			{
				num++;
			}
		}
		return num;
	}

	private static string RepairPlainMechanicDisplay(string value)
	{
		if (!TryGetMechanicErrorKey(value, out var key))
		{
			return value;
		}
		string localizedMechanic = LocHelper.GetLocalizedMechanic(key, Array.Empty<object>());
		if (!ValidMechanic(localizedMechanic, key))
		{
			return value;
		}
		return localizedMechanic;
	}

	private static int RepairModuleEffectDisplays()
	{
		if (ModuleEffectHelper.moduleEffects == null)
		{
			return 0;
		}
		int num = 0;
		foreach (KeyValuePair<string, ModuleEffect> moduleEffect in ModuleEffectHelper.moduleEffects)
		{
			ModuleEffectHelper.ModuleEffect value = moduleEffect.Value;
			switch (moduleEffect.Key)
			{
			case "MODULE_EFFECT_PREMIUM":
				num += RepairModuleEffect(value, "mech_module_effect_premium_name", "mech_module_effect_premium_desc", 20);
				break;
			case "MODULE_EFFECT_INFERIOR":
				num += RepairModuleEffect(value, "mech_module_effect_inferior_name", "mech_module_effect_inferior_desc", 19);
				break;
			case "MODULE_EFFECT_RECEIVER":
				num += RepairModuleEffect(value, "mech_module_effect_receiver_name", "mech_module_effect_receiver_desc", 10);
				break;
			case "MODULE_EFFECT_TOP_MOUNT":
				num += RepairModuleEffect(value, "mech_module_effect_top_mount_name", "mech_module_effect_top_mount_desc", 20);
				break;
			case "MODULE_EFFECT_SIDE_MOUNT":
				num += RepairModuleEffect(value, "mech_module_effect_side_mount_name", "mech_module_effect_side_mount_desc", 20);
				break;
			case "MODULE_EFFECT_OVERVOLTED":
				num += RepairModuleEffect(value, "mech_module_effect_overvolted_name", "mech_module_effect_overvolted_desc", 200, 10);
				break;
			case "MODULE_EFFECT_DEGRADING":
				num += RepairModuleEffect(value, "mech_module_effect_degrading_name", "mech_module_effect_degrading_desc", 1);
				break;
			case "MODULE_EFFECT_LEARNING_ALGORITHM":
				num += RepairModuleEffect(value, "mech_module_effect_learning_algorithm_name", "mech_module_effect_learning_algorithm_desc", 1);
				break;
			case "MODULE_EFFECT_NEGATIVE_FEEDBACK":
				num += RepairModuleEffect(value, "mech_module_effect_negative_feedback_name", "mech_module_effect_negative_feedback_desc", 25, 25);
				break;
			case "NODE_EFFECT_SUPPORT":
				num += RepairModuleEffect(value, "mech_module_effect_support_node_name", "mech_module_effect_support_node_desc", 20);
				break;
			}
		}
		return num;
	}

	private static int RepairModuleEffect(ModuleEffectHelper.ModuleEffect effect, string nameKey, string descriptionKey, params object[] descriptionArgs)
	{
		if (effect == null)
		{
			return 0;
		}
		int num = 0;
		if (TryGetMechanicErrorKey(effect.displayName, out var key) && key == nameKey)
		{
			string localizedMechanic = LocHelper.GetLocalizedMechanic(nameKey, Array.Empty<object>());
			if (ValidMechanic(localizedMechanic, nameKey))
			{
				effect.displayName = localizedMechanic;
				num++;
			}
		}
		if (TryGetMechanicErrorKey(effect.description, out var key2) && key2 == descriptionKey)
		{
			string localizedMechanic2 = LocHelper.GetLocalizedMechanic(descriptionKey, descriptionArgs);
			if (ValidMechanic(localizedMechanic2, descriptionKey))
			{
				effect.description = localizedMechanic2;
				num++;
			}
		}
		return num;
	}

	private static int RepairChemFinisherRecipeNames()
	{
		if (ChemFinisherRecipe.chemFinisherRecipes == null)
		{
			return 0;
		}
		int num = 0;
		string a = default;
		string key = default;
		foreach (ChemFinisherRecipe chemFinisherRecipe in ChemFinisherRecipe.chemFinisherRecipes)
		{
			if (chemFinisherRecipe != null && LocHelper.TryParseMissingTranslation(chemFinisherRecipe.displayName, ref a, ref key) && string.Equals(a, "Item", StringComparison.Ordinal))
			{
				string text = LocHelper.RelocalizeIfMissing(chemFinisherRecipe.displayName);
				if (ValidLocalized(text, key))
				{
					chemFinisherRecipe.displayName = text;
					num++;
				}
			}
		}
		return num;
	}

	private static bool TryGetAnyTranslationErrorKey(string value, out string key)
	{
		key = null;
		if (string.IsNullOrEmpty(value) || !value.StartsWith("Translation Error '", StringComparison.Ordinal))
		{
			return false;
		}
		int num = value.IndexOf("' in ", "Translation Error '".Length, StringComparison.Ordinal);
		if (num <= "Translation Error '".Length)
		{
			return false;
		}
		key = value.Substring("Translation Error '".Length, num - "Translation Error '".Length);
		return !string.IsNullOrEmpty(key);
	}

	private static bool ValidLocalized(string value, string key)
	{
		if (!string.IsNullOrEmpty(value) && value != key)
		{
			return value.IndexOf("Translation Error", StringComparison.OrdinalIgnoreCase) < 0;
		}
		return false;
	}

	private static bool IsOwn(LoadedMod mod)
	{
		if (mod != null && mod.Manifest != null)
		{
			return string.Equals(mod.Manifest.ID, "probably_stolen_korean_pack", StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	private void AssetsLoaded(LoadedMod mod)
	{
		if (enabled && mod != null && mod.Manifest != null)
		{
			if (IsOwn(mod))
			{
				Build(mod);
				ApplyAll();
				waterNames.AssetsReady();
			}
			else if (ready)
			{
				Apply(mod);
			}
		}
	}

	private void Build(LoadedMod own)
	{
		catalogs.Clear();
		if (own.Localization == null)
		{
			return;
		}
		foreach (KeyValuePair<string, Dictionary<string, string>> item in own.Localization.Where((KeyValuePair<string, Dictionary<string, string>> x) =>
		{
			KeyValuePair<string, Dictionary<string, string>> keyValuePair = x;
			return keyValuePair.Key.StartsWith("pskb__", StringComparison.OrdinalIgnoreCase);
		}).OrderBy((KeyValuePair<string, Dictionary<string, string>> x) =>
		{
			KeyValuePair<string, Dictionary<string, string>> keyValuePair = x;
			return keyValuePair.Key.StartsWith("pskb__supp_", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
		}).ThenBy((KeyValuePair<string, Dictionary<string, string>> x) => x.Key, StringComparer.Ordinal))
		{
			if (item.Value == null)
			{
				continue;
			}
			Catalog catalog = new Catalog();
			string text = item.Key.Substring(6);
			if (text.StartsWith("supp_", StringComparison.OrdinalIgnoreCase))
			{
				text = text.Substring(5);
			}
			AddAlias(catalog, text);
			foreach (KeyValuePair<string, string> item2 in item.Value)
			{
				if (item2.Key.StartsWith("M|", StringComparison.Ordinal) || item2.Key.StartsWith("W|", StringComparison.Ordinal))
				{
					AddAlias(catalog, item2.Key.Substring(2));
				}
				else if (item2.Key.StartsWith("K|", StringComparison.Ordinal))
				{
					AddEntry(catalog, item2.Key.Substring(2), item2.Value);
				}
				else if (item2.Key.StartsWith("K64|", StringComparison.Ordinal))
				{
					try
					{
						string text2 = item2.Key.Substring(4).Replace('-', '+').Replace('_', '/');
						text2 = text2.PadRight((text2.Length + 3) / 4 * 4, '=');
						AddEntry(catalog, Encoding.UTF8.GetString(Convert.FromBase64String(text2)), item2.Value);
					}
					catch (FormatException)
					{
						Debug.LogWarning((object)("[KoreanPack] Invalid encoded localization key in " + item.Key));
					}
				}
			}
			if (catalog.Entries.Count > 0)
			{
				catalogs.Add(catalog);
			}
		}
		ready = true;
		Debug.Log((object)("[KoreanPack] Key catalogs: " + catalogs.Count + ", entries: " + catalogs.Sum((Catalog c) => c.Entries.Count)));
	}

	private static void AddAlias(Catalog catalog, string alias)
	{
		if (!string.IsNullOrWhiteSpace(alias))
		{
			catalog.Aliases.Add(alias.Trim());
		}
	}

	private static void AddEntry(Catalog catalog, string key, string value)
	{
		if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
		{
			catalog.Entries[key] = value;
		}
	}

	private void ApplyAll()
	{
		foreach (LoadedMod mod in ModLoader.Mods)
		{
			if (!IsOwn(mod))
			{
				Apply(mod);
			}
		}
	}

	private void Apply(LoadedMod mod)
	{
		if (mod == null || mod.Manifest == null)
		{
			return;
		}
		Catalog[] array = catalogs.Where((Catalog c) => c.Aliases.Contains(mod.Manifest.ID) || (mod.WorkshopId != 0L && c.Aliases.Contains(mod.WorkshopId.ToString())) || (mod.ShadowedWorkshopId != 0L && c.Aliases.Contains(mod.ShadowedWorkshopId.ToString()))).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		if (mod.Localization == null)
		{
			mod.Localization = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
		}
		string[] array2 = new string[2] { "ko", "ko-KR" };
		foreach (string key in array2)
		{
			if (!mod.Localization.TryGetValue(key, out var value) || value == null)
			{
				Dictionary<string, string> dictionary = (mod.Localization[key] = new Dictionary<string, string>(StringComparer.Ordinal));
				value = dictionary;
			}
			if (!originals.TryGetValue(value, out var value2))
			{
				Dictionary<string, Original> dictionary3 = (originals[value] = new Dictionary<string, Original>(StringComparer.Ordinal));
				value2 = dictionary3;
			}
			Catalog[] array3 = array;
			for (int num2 = 0; num2 < array3.Length; num2++)
			{
				foreach (KeyValuePair<string, string> entry in array3[num2].Entries)
				{
					if (!value2.TryGetValue(entry.Key, out var value3))
					{
						bool existed = value.TryGetValue(entry.Key, out var value4);
						Dictionary<string, Original> dictionary5 = value2;
						string key2 = entry.Key;
						Original original = new Original
						{
							Existed = existed,
							Value = value4
						};
						dictionary5[key2] = original;
						value3 = original;
					}
					value[entry.Key] = entry.Value;
					value3.Applied = entry.Value;
				}
			}
		}
		Debug.Log((object)("[KoreanPack] Applied key translations: " + mod.Manifest.ID));
		FishingStartPerkCompatibility.Refresh();
	}
}
