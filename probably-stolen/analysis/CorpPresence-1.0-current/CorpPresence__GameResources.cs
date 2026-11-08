using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CorpPresence;

internal static class GameResources
{
	private static AsyncOperationHandle<StringTable> _mechanic;

	private static string _requestedLocale;

	private static long _generation;

	public static bool Ready { get; private set; }

	public static long Generation => _generation;

	public static void Begin()
	{
		_generation++;
		Reset();
	}

	public static void Reset()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Ready = false;
		_requestedLocale = null;
		_mechanic = default(AsyncOperationHandle<StringTable>);
	}

	public static bool Poll()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Invalid comparison between Unknown and I4
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		if (Ready)
		{
			return true;
		}
		AsyncOperationHandle<LocalizationSettings> initializationOperation = LocalizationSettings.InitializationOperation;
		if (!initializationOperation.IsValid() || !initializationOperation.IsDone)
		{
			return false;
		}
		if ((int)initializationOperation.Status != 1)
		{
			throw new InvalidOperationException("本地化系统初始化失败", initializationOperation.OperationException);
		}
		Locale selectedLocale = LocalizationSettings.SelectedLocale;
		if ((Object)(object)selectedLocale == (Object)null)
		{
			return false;
		}
		string text = ResolveMenuStartupLocale();
		LocaleIdentifier val = selectedLocale.Identifier;
		if (((LocaleIdentifier)(ref val)).Code != text)
		{
			return false;
		}
		if (_requestedLocale != text)
		{
			_mechanic = ((LocalizedDatabase<StringTable, StringTableEntry>)(object)LocalizationSettings.StringDatabase).GetTableAsync(TableReference.op_Implicit("Mechanic"), selectedLocale);
			_requestedLocale = text;
		}
		if (!_mechanic.IsValid())
		{
			throw new InvalidOperationException("Mechanic 表句柄失效，停止公司初始化");
		}
		if (!_mechanic.IsDone)
		{
			return false;
		}
		if ((int)_mechanic.Status != 1 || (Object)(object)_mechanic.Result == (Object)null)
		{
			throw new InvalidOperationException("Mechanic 表加载失败", _mechanic.OperationException);
		}
		val = ((LocalizationTable)_mechanic.Result).LocaleIdentifier;
		if (((LocaleIdentifier)(ref val)).Code != text)
		{
			throw new InvalidOperationException("Mechanic 表语言与游戏启动语言不符");
		}
		if (((DetailedLocalizationTable<StringTableEntry>)(object)_mechanic.Result).GetEntry("mech_condition_pure_water") == null)
		{
			throw new InvalidOperationException("Mechanic 表缺少 mech_condition_pure_water");
		}
		Ready = true;
		return true;
	}

	private static string ResolveMenuStartupLocale()
	{
		return LocHelper.ResolveStartupLocale() switch
		{
			"zh" => "zh", 
			"ru" => "ru", 
			"de" => "de", 
			"ja" => "ja", 
			"fr" => "fr", 
			"es" => "es", 
			"pl" => "pl", 
			_ => "en", 
		};
	}
}
