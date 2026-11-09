using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ProbablyStolenKoreanPack;

internal static class FishingStartPerkCompatibility
{
	private static bool enabled;

	private static StartingPerk saved;

	private static string oldName;

	private static string oldDescription;

	private static string appliedName;

	private static string appliedDescription;

	private static AsyncOperationHandle<LocalizationSettings> initialization;

	internal static void Enable()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			return;
		}
		enabled = true;
		try
		{
			LocalizationSettings.SelectedLocaleChanged += LocaleChanged;
			ModHook.OnGameLoadedLate += Refresh;
			initialization = LocalizationSettings.InitializationOperation;
			if (initialization.IsDone)
			{
				Refresh();
			}
			else
			{
				initialization.Completed += Initialized;
			}
		}
		catch (Exception ex)
		{
			Disable();
			Debug.LogWarning((object)("[KoreanPack] Fishing starting perk compatibility unavailable: " + ex.Message));
		}
	}

	internal static void Disable()
	{
		enabled = false;
		LocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
		ModHook.OnGameLoadedLate -= Refresh;
		if (initialization.IsValid())
		{
			initialization.Completed -= Initialized;
		}
		Restore();
	}

	private static void Initialized(AsyncOperationHandle<LocalizationSettings> operation)
	{
		Refresh();
	}

	private static void LocaleChanged(Locale locale)
	{
		Refresh();
	}

	private static void Restore()
	{
		if (saved != null)
		{
			if (saved.rawName == appliedName)
			{
				saved.rawName = oldName;
			}
			if (saved.rawDescription == appliedDescription)
			{
				saved.rawDescription = oldDescription;
			}
		}
		saved = null;
		oldName = null;
		oldDescription = null;
		appliedName = null;
		appliedDescription = null;
	}

	internal static void Refresh()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!enabled || !initialization.IsValid() || !initialization.IsDone || (int)initialization.Status != 1)
			{
				return;
			}
			Locale selectedLocale = LocalizationSettings.SelectedLocale;
			if ((Object)(object)selectedLocale == (Object)null)
			{
				return;
			}
			LocaleIdentifier identifier = selectedLocale.Identifier;
			string code = identifier.Code;
			if (code != "ko" && code != "ko-KR")
			{
				Restore();
				return;
			}
			LoadedMod val = null;
			foreach (LoadedMod mod in ModLoader.Mods)
			{
				if (mod != null && mod.IsEnabled && mod.Manifest != null && mod.Manifest.ID == "ps-fishing-gameplay")
				{
					val = mod;
					break;
				}
			}
			Dictionary<string, string> value = null;
			if (val == null || val.Localization == null || (!val.Localization.TryGetValue(code, out value) && !val.Localization.TryGetValue("ko", out value)) || value == null)
			{
				Restore();
			}
			else
			{
				if (!value.TryGetValue("gameplay.67f15a2ad1a0f439", out var value2) || !value.TryGetValue("gameplay.6f7353ff4a5869a4", out var value3) || string.IsNullOrWhiteSpace(value2) || string.IsNullOrWhiteSpace(value3))
				{
					return;
				}
				StartingPerk modPerk = StartingPerkList.GetModPerk("gf_simple_fishing");
				if (modPerk != null && !(modPerk.source != "ps-fishing-gameplay"))
				{
					if (saved != modPerk)
					{
						Restore();
						saved = modPerk;
						oldName = modPerk.rawName;
						oldDescription = modPerk.rawDescription;
					}
					if (!(modPerk.rawName == value2) || !(modPerk.rawDescription == value3))
					{
						appliedName = value2;
						appliedDescription = value3;
						modPerk.rawName = value2;
						modPerk.rawDescription = value3;
						Debug.Log((object)"[KoreanPack] Fishing starting perk names refreshed from Korean keys (official callbacks only).");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)("[KoreanPack] Fishing starting perk refresh skipped: " + ex.Message));
		}
	}
}
