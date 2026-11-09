using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ProbablyStolenKoreanPack;

internal sealed class WaterNameCompatibility
{
	private bool enabled;

	private bool worldReady;

	private bool assetsReady;

	private bool tooltipRepairLogged;

	private int generation;

	private AsyncOperationHandle<LocalizationSettings> initialization;

	private readonly Dictionary<string, string> replacements = new Dictionary<string, string>(StringComparer.Ordinal);

	private static readonly string[] QualityKeys = new string[6] { "mech_condition_pure_water", "mech_condition_high_quality_water", "mech_condition_basewater", "mech_condition_ghostwater", "mech_condition_rustwater", "mech_condition_gutterflow" };

	// Original Korean Translation 2.1 (Mechanic table IDs 951840645365857..862).
	// This is a narrow data repair, not a global UI text hook.
	private static readonly string[] KoreanQualityNames = new string[6] { "순수한 물", "고품질 물", "기본수", "유령수", "녹물", "하수" };

	private static readonly Dictionary<string, string> LiquidKeys = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		{ "water", "liquid_water" },
		{ "heavy_metal", "liquid_heavy_metal" },
		{ "organic_waste", "liquid_organic_waste" },
		{ "microplastic", "liquid_microplastic" },
		{ "microbe", "liquid_microbes" },
		{ "chemical_contaminant", "liquid_chemical_contaminant" },
		{ "physical_contaminant", "liquid_physical_contaminant" },
		{ "mineral", "liquid_mineral" },
		{ "dream_cap_mush", "liquid_dream_cap_mush" },
		{ "dream_cap_extract", "liquid_dream_cap_extract" },
		{ "plant_matter", "liquid_plant_matter" },
		{ "alcohol", "liquid_alcohol" },
		{ "caffeine", "liquid_caffeine" },
		{ "protein", "liquid_protein" },
		{ "fat", "liquid_fat_oil" },
		{ "starch", "liquid_starch" },
		{ "sugar", "liquid_sugar" },
		{ "coffee_mush", "liquid_coffee_bean_mush" },
		{ "contraceptive", "liquid_contraceptive_hormones" },
		{ "zyanide", "liquid_zyanide" },
		{ "nightmare_extract", "liquid_nightmare_extract" },
		{ "nightmare_cap_mush", "liquid_nightmare_cap_mush" },
		{ "chlorine", "liquid_chlorine" },
		{ "synaprest", "liquid_synaprest" },
		{ "fanta", "liquid_fanta" },
		{ "methylol", "liquid_n_methylol_base" },
		{ "reducing_agent", "liquid_reducing_agent" },
		{ "acid", "liquid_acid" },
		{ "base", "liquid_base" },
		{ "stabilizing_agent", "liquid_stabilizing_agent" },
		{ "debris", "liquid_insoluble_debris" },
		{ "lipobond_agent", "liquid_lipobond_agent" },
		{ "livestock_steroid", "liquid_livestock_steroid" },
		{ "livestock_immunity", "liquid_livestock_immunity" },
		{ "wine_extract", "liquid_wine_extract" },
		{ "nomore", "liquid_nomore" }
	};

	internal void Enable()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!enabled)
		{
			enabled = true;
			LocalizationSettings.SelectedLocaleChanged += LocaleChanged;
			ModHook.OnGameLoadedInit += Loading;
			ModHook.OnGameLoadedLate += Loaded;
			ModHook.OnCreateTooltipEarly += BeforeWaterTooltip;
			initialization = LocalizationSettings.InitializationOperation;
			if (initialization.IsDone)
			{
				Request();
			}
			else
			{
				initialization.Completed += Initialized;
			}
		}
	}

	internal void Disable()
	{
		enabled = false;
		worldReady = false;
		assetsReady = false;
		generation++;
		LocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
		ModHook.OnGameLoadedInit -= Loading;
		ModHook.OnGameLoadedLate -= Loaded;
		ModHook.OnCreateTooltipEarly -= BeforeWaterTooltip;
		if (initialization.IsValid())
		{
			initialization.Completed -= Initialized;
		}
		replacements.Clear();
	}

	private void Initialized(AsyncOperationHandle<LocalizationSettings> operation)
	{
		Request();
	}

	internal void AssetsReady()
	{
		assetsReady = true;
		Request();
	}

	private void LocaleChanged(Locale locale)
	{
		generation++;
		Request();
	}

	private void Loading()
	{
		worldReady = false;
	}

	private void Loaded()
	{
		worldReady = true;
		Request();
	}

	// Repair the source ItemFeature data at the official early tooltip callback.
	// This is limited to water-purity features; it never intercepts or replaces UI text.
	private void BeforeWaterTooltip(RichTextBuilder builder, GameItem item)
	{
		if (!enabled || item == null)
		{
			return;
		}
		Locale locale = LocalizationSettings.SelectedLocale;
		if ((Object)(object)locale == (Object)null)
		{
			return;
		}
		string code = locale.Identifier.Code;
		if (code != "ko" && code != "ko-KR")
		{
			return;
		}
		try
		{
			ItemFeature feature = item.FindItemFeatureByCategory("CATEGORY_WATER_PURITY");
			if (feature == null)
			{
				return;
			}
			bool repaired = RepairCondition(feature.fakeCondition);
			if (feature.realCondition != feature.fakeCondition)
			{
				repaired |= RepairCondition(feature.realCondition);
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
			if (repaired && !tooltipRepairLogged)
			{
				tooltipRepairLogged = true;
				Debug.Log((object)"[KoreanPack] Water label data repaired before native tooltip (official item event).");
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)("[KoreanPack] Water tooltip data repair skipped: " + ex.Message));
		}
	}

	private void Request()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (!enabled || !assetsReady || !initialization.IsValid() || !initialization.IsDone || (int)initialization.Status != 1)
		{
			return;
		}
		Locale locale = LocalizationSettings.SelectedLocale;
		if ((Object)(object)locale == (Object)null)
		{
			return;
		}
		LocaleIdentifier identifier = locale.Identifier;
		if (identifier.Code != "ko")
		{
			identifier = locale.Identifier;
			if (identifier.Code != "ko-KR")
			{
				return;
			}
		}
		int token = ++generation;
		AsyncOperationHandle<StringTable> tableAsync = ((LocalizedDatabase<StringTable, StringTableEntry>)(object)LocalizationSettings.StringDatabase).GetTableAsync((TableReference)"Mechanic", locale);
		if (tableAsync.IsDone)
		{
			Complete(tableAsync, locale, token);
			return;
		}
		tableAsync.Completed += (AsyncOperationHandle<StringTable> operation) =>
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Complete(operation, locale, token);
		};
	}

	private void Complete(AsyncOperationHandle<StringTable> operation, Locale locale, int token)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		if (!enabled || token != generation || (Object)(object)LocalizationSettings.SelectedLocale != (Object)(object)locale || !operation.IsValid() || (int)operation.Status != 1 || (Object)(object)operation.Result == (Object)null)
		{
			return;
		}
		try
		{
			// Recover only missing Korean Mechanic entries at the authoritative
			// StringTable. Patching cached item labels alone misses later tooltip
			// calculations that call LocHelper.GetLocalizedMechanic again.
			StringTable mechanic = operation.Result;
			string[] array = new string[QualityKeys.Length];
			int tableRepairs = 0;
			for (int i = 0; i < array.Length; i++)
			{
				string key = QualityKeys[i];
				string translated = LocHelper.GetLocalizedMechanic(key, Array.Empty<object>());
				if (!Valid(translated, key))
				{
					// AddEntry also updates an existing value in this Unity Localization version.
					mechanic.AddEntry(key, KoreanQualityNames[i]);
					tableRepairs++;
					translated = LocHelper.GetLocalizedMechanic(key, Array.Empty<object>());
					if (!Valid(translated, key))
					{
						// Keep existing saved-item cache repair functional even when
						// the game's localized-string cache still holds an old error.
						translated = KoreanQualityNames[i];
					}
				}
				array[i] = translated;
			}
			if (tableRepairs != 0)
			{
				Debug.Log((object)("[KoreanPack] Korean Mechanic water entries repaired: " + tableRepairs));
			}
			replacements.Clear();
			KoreanPack.RepairConditionDisplays(includeItems: false);
			for (int j = 0; j < array.Length; j++)
			{
				replacements[Error(QualityKeys[j])] = array[j];
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (Liquid liquid in Liquid.Liquids)
			{
				if (liquid != null && liquid.identifier != null && LiquidKeys.TryGetValue(liquid.identifier, out var value) && !(liquid.displayName != Error(value)))
				{
					string localizedMechanic = LocHelper.GetLocalizedMechanic(value, Array.Empty<object>());
					if (Valid(localizedMechanic, value))
					{
						liquid.displayName = localizedMechanic;
						num++;
					}
				}
			}
			WaterFeatureHelper.PURE_WATER_DISPLAY = Repair(WaterFeatureHelper.PURE_WATER_DISPLAY);
			WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY = Repair(WaterFeatureHelper.HIGH_QUALITY_WATER_DISPLAY);
			WaterFeatureHelper.BASE_WATER_DISPLAY = Repair(WaterFeatureHelper.BASE_WATER_DISPLAY);
			WaterFeatureHelper.GHOSTWATER_DISPLAY = Repair(WaterFeatureHelper.GHOSTWATER_DISPLAY);
			WaterFeatureHelper.RUST_WATER_DISPLAY = Repair(WaterFeatureHelper.RUST_WATER_DISPLAY);
			WaterFeatureHelper.GUTTERFLOW_WATER_DISPLAY = Repair(WaterFeatureHelper.GUTTERFLOW_WATER_DISPLAY);
			if (WaterFeatureHelper.WATER_DISPLAY != null)
			{
				for (int k = 0; k < WaterFeatureHelper.WATER_DISPLAY.Length; k++)
				{
					WaterFeatureHelper.WATER_DISPLAY[k] = Repair(WaterFeatureHelper.WATER_DISPLAY[k]);
				}
			}
			foreach (ItemCondition item in ItemConditionList.conditionWaterPurity)
			{
				if (RepairCondition(item))
				{
					num2++;
				}
			}
			if (worldReady && PlayerStore.HasInstance)
			{
				foreach (GameItem item2 in PlayerStore.Instance.FindAllItem(false))
				{
					if (item2 == null)
					{
						continue;
					}
					ItemFeature val = item2.FindItemFeatureByCategory("CATEGORY_WATER_PURITY");
					if (val == null)
					{
						continue;
					}
					bool flag = RepairCondition(val.fakeCondition);
					flag |= RepairCondition(val.realCondition);
					if (!val.useCondition)
					{
						string baseFakeDisplay = val.GetBaseFakeDisplay();
						string text = Repair(baseFakeDisplay);
						if (baseFakeDisplay != text)
						{
							val.SetPublicDisplay(text);
							flag = true;
						}
						baseFakeDisplay = val.GetBaseRealDisplay();
						text = Repair(baseFakeDisplay);
						if (baseFakeDisplay != text)
						{
							val.SetActualDisplay(text);
							flag = true;
						}
					}
					if (flag)
					{
						num3++;
					}
				}
			}
			Debug.Log((object)("[KoreanPack] Water name compatibility: liquids=" + num + ", conditions=" + num2 + ", saved labels=" + num3 + "; official callbacks only."));
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)("[KoreanPack] Water name compatibility skipped: " + ex.Message));
		}
	}

	private bool RepairCondition(ItemCondition condition)
	{
		if (condition == null || condition.category != "CATEGORY_WATER_PURITY")
		{
			return false;
		}
		string text = Repair(condition.display);
		string text2 = Repair(condition.identifier);
		bool result = text != condition.display || text2 != condition.identifier;
		condition.display = text;
		condition.identifier = text2;
		return result;
	}

	private string Repair(string value)
	{
		if (value == null || value.IndexOf("Translation Error '", StringComparison.Ordinal) < 0)
		{
			return value;
		}
		foreach (KeyValuePair<string, string> replacement in replacements)
		{
			value = value.Replace(replacement.Key, replacement.Value);
		}
		// The locale callback can precede inventory deserialization or a new
		// container can be created later. Never depend solely on a cached map.
		for (int i = 0; i < QualityKeys.Length; i++)
		{
			value = value.Replace(Error(QualityKeys[i]), KoreanQualityNames[i]);
		}
		return value;
	}

	private static string Error(string key)
	{
		return "Translation Error '" + key + "' in Mechanic";
	}

	private static bool Valid(string value, string key)
	{
		if (!string.IsNullOrEmpty(value) && value != key)
		{
			return value.IndexOf("Translation Error", StringComparison.OrdinalIgnoreCase) < 0;
		}
		return false;
	}
}
