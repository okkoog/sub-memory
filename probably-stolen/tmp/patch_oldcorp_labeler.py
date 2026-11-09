from pathlib import Path
import os

root = Path(os.environ["RUNNER_TEMP"])
src = root / "src"
kp = src / "ProbablyStolenKoreanPack" / "KoreanPack.cs"
s = kp.read_text(encoding="utf-8")

# Guard: this must be the old Corp-capable build.
for token in (
    "CorpStartupLocalePostfix",
    "ClearCorpPendingStartupLocale",
    "corp-korean-startup-locale",
    "CorpPresence.UI.MenuLabels",
):
    if token not in s:
        raise SystemExit(f"old Corp compatibility token missing: {token}")

# Add a completely separate Harmony owner for Labeler only.
anchor = 'private const string DynamicHarmonyId = "probably_stolen_korean_pack.target_text";'
if anchor not in s:
    raise SystemExit("DynamicHarmonyId anchor missing")
s = s.replace(
    anchor,
    anchor + '\n\n\tprivate const string LabelerHarmonyId = "probably_stolen_korean_pack.labeler_condition_repair";',
    1,
)

anchor = "\tprivate static Harmony _dynamicHarmony;"
if anchor not in s:
    raise SystemExit("_dynamicHarmony anchor missing")
s = s.replace(anchor, anchor + "\n\n\tprivate static Harmony _labelerHarmony;", 1)

# Arm the Labeler patch only after the game load late callback.
old = """\t\t\t\t_enabled = true;
\t\t\t\tModHook.OnModAssetsLoaded += OnModAssetsLoaded;
\t\t\t\tPatchFinalTooltipBridge();"""
new = """\t\t\t\t_enabled = true;
\t\t\t\tModHook.OnModAssetsLoaded += OnModAssetsLoaded;
\t\t\t\tModHook.OnGameLoadedLate += EnableLabelerConditionRepairLate;
\t\t\t\tPatchFinalTooltipBridge();"""
if old not in s:
    raise SystemExit("OnEnable anchor missing")
s = s.replace(old, new, 1)

old = """\t\t\t\t_enabled = false;
\t\t\t\tModHook.OnModAssetsLoaded -= OnModAssetsLoaded;
\t\t\t\tRestoreNativeEntries();
\t\t\t\tUnpatchDynamic();"""
new = """\t\t\t\t_enabled = false;
\t\t\t\tModHook.OnModAssetsLoaded -= OnModAssetsLoaded;
\t\t\t\tModHook.OnGameLoadedLate -= EnableLabelerConditionRepairLate;
\t\t\t\tDisableLabelerConditionRepair();
\t\t\t\tRestoreNativeEntries();
\t\t\t\tUnpatchDynamic();"""
if old not in s:
    raise SystemExit("OnDisable anchor missing")
s = s.replace(old, new, 1)

# Insert the exact late-scoped Labeler repair logic used by the recent build.
anchor = "\n\tprivate static void OnModAssetsLoaded(LoadedMod loaded)"
if anchor not in s:
    raise SystemExit("OnModAssetsLoaded anchor missing")
methods = r'''
	private static void EnableLabelerConditionRepairLate()
	{
		if (!_enabled || _labelerHarmony != null)
		{
			return;
		}
		try
		{
			EnableLabelerConditionRepair();
			Debug.Log("[KoreanPack] Labeler condition repair armed after game load.");
		}
		catch (Exception ex)
		{
			DisableLabelerConditionRepair();
			Debug.LogWarning("[KoreanPack] Labeler condition repair unavailable: " + ex.Message);
		}
	}

	private static void EnableLabelerConditionRepair()
	{
		if (_labelerHarmony != null)
		{
			return;
		}
		MethodInfo original = AccessTools.Method(typeof(LabelerElement), "InitElement", new Type[1] { typeof(ItemFeature) });
		MethodInfo prefix = AccessTools.Method(typeof(KoreanPack), "BeforeLabelerInit");
		if (original == null || prefix == null)
		{
			throw new MissingMethodException("LabelerElement.InitElement repair target not found.");
		}
		_labelerHarmony = new Harmony(LabelerHarmonyId);
		_labelerHarmony.Patch(original, new HarmonyMethod(prefix));
	}

	private static void DisableLabelerConditionRepair()
	{
		if (_labelerHarmony == null)
		{
			return;
		}
		try
		{
			_labelerHarmony.UnpatchAll(LabelerHarmonyId);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Labeler condition repair cleanup failed: " + ex.Message);
		}
		_labelerHarmony = null;
	}

	private static void BeforeLabelerInit(ItemFeature itemFeature)
	{
		try
		{
			int num = RepairForLabeler(itemFeature);
			if (num > 0)
			{
				Debug.Log("[KoreanPack] Labeler condition cache repaired on open: " + num);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Labeler condition repair skipped: " + ex.Message);
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
		string localizedMechanic = LocHelper.GetLocalizedMechanic(key);
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
		string localizedMechanic = LocHelper.GetLocalizedMechanic(key);
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
		const string prefix = "Translation Error '";
		const string suffix = "' in Mechanic";
		if (string.IsNullOrEmpty(value) || !value.StartsWith(prefix, StringComparison.Ordinal) || !value.EndsWith(suffix, StringComparison.Ordinal))
		{
			return false;
		}
		int length = value.Length - prefix.Length - suffix.Length;
		if (length <= 0)
		{
			return false;
		}
		key = value.Substring(prefix.Length, length);
		return !string.IsNullOrEmpty(key);
	}

	private static bool ValidMechanic(string value, string key)
	{
		return !string.IsNullOrEmpty(value) && value != key && value.IndexOf("Translation Error", StringComparison.OrdinalIgnoreCase) < 0;
	}
'''
s = s.replace(anchor, "\n" + methods + anchor, 1)

# Final guard: Corp logic must still be present and untouched at the source level.
for token in (
    "CorpStartupLocalePostfix",
    "ClearCorpPendingStartupLocale",
    "corp-korean-startup-locale",
    "CorpPresence.UI.MenuLabels",
):
    if token not in s:
        raise SystemExit(f"Corp compatibility lost after patch: {token}")

kp.write_text(s, encoding="utf-8")
print("patched old Corp-working DLL source with late Labeler repair only")
