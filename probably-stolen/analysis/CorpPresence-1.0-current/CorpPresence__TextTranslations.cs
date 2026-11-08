using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace CorpPresence;

internal static class TextTranslations
{
	private static readonly Lazy<Dictionary<string, string[]>> Catalog = new Lazy<Dictionary<string, string[]>>(() => LoadCatalog("CorpPresence.Translations.json"));

	private static readonly Lazy<Dictionary<string, string[]>> KeyedCatalog = new Lazy<Dictionary<string, string[]>>(() => LoadCatalog("CorpPresence.Translations.Keys.json"));

	internal static bool HasLocale(string locale)
	{
		if (LanguageIndex(locale) < 0)
		{
			return ExternalTranslations.HasLocale(locale);
		}
		return true;
	}

	private static int LanguageIndex(string locale)
	{
		if (string.IsNullOrEmpty(locale))
		{
			return -1;
		}
		if (IsBuiltInLocale(locale, "ru"))
		{
			return 0;
		}
		if (IsBuiltInLocale(locale, "fr"))
		{
			return 1;
		}
		return -1;
	}

	internal static bool IsBuiltInLocale(string locale, string language)
	{
		if (locale != null)
		{
			if (!string.Equals(locale, language, StringComparison.OrdinalIgnoreCase) && !locale.StartsWith(language + "-", StringComparison.OrdinalIgnoreCase))
			{
				return locale.StartsWith(language + "_", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}
		return false;
	}

	internal static string FormatKey(string key, string english, string locale, params object[] arguments)
	{
		FormattableString formattableString = FormattableStringFactory.Create(english, arguments);
		if (ExternalTranslations.TryFormat(key, formattableString, locale, out var value))
		{
			return value;
		}
		string format = SelectBuiltInKey(key, english, locale);
		try
		{
			return string.Format(format, arguments);
		}
		catch (FormatException)
		{
			return formattableString.ToString();
		}
	}

	internal static string Select(string english, string locale)
	{
		if (english == null)
		{
			return null;
		}
		int num = LanguageIndex(locale);
		if (num < 0)
		{
			return english;
		}
		if (Catalog.Value.TryGetValue(english, out var value) && value != null && value.Length > num)
		{
			if (!string.IsNullOrEmpty(value[num]))
			{
				return value[num];
			}
			return english;
		}
		return english;
	}

	internal static string SelectKey(string key, string english, string locale)
	{
		if (!ExternalTranslations.TryGet(key, english, locale, out var value))
		{
			return SelectBuiltInKey(key, english, locale);
		}
		return value;
	}

	internal static string SelectBuiltInKey(string key, string english, string locale)
	{
		if (english == null)
		{
			return null;
		}
		int num = LanguageIndex(locale);
		if (num < 0)
		{
			return english;
		}
		if (key != null && KeyedCatalog.Value.TryGetValue(key, out var value) && value != null && value.Length > num && !string.IsNullOrEmpty(value[num]))
		{
			return value[num];
		}
		return Select(english, locale);
	}

	private static Dictionary<string, string[]> LoadCatalog(string resourceName)
	{
		try
		{
			using Stream stream = typeof(TextTranslations).Assembly.GetManifestResourceStream(resourceName);
			if (stream != null)
			{
				using StreamReader streamReader = new StreamReader(stream);
				Dictionary<string, string[]> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(streamReader.ReadToEnd());
				if (dictionary != null)
				{
					return dictionary;
				}
			}
		}
		catch (Exception ex)
		{
			ModLog.Warn("翻译资源读取失败：" + ex.Message);
		}
		return new Dictionary<string, string[]>(StringComparer.Ordinal);
	}
}
