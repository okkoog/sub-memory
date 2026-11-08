using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CorpPresence;

internal static class L
{
	private static int _frame = -1;

	private static bool _en;

	private static bool _ready;

	private static string _locale = "";

	private static int _translationVersion = -1;

	public static bool En
	{
		get
		{
			int version = ExternalTranslations.Version;
			if (_frame == Time.frameCount && _translationVersion == version)
			{
				return _en;
			}
			_frame = Time.frameCount;
			_translationVersion = version;
			if (!_ready && (Object)(object)FontLoader.Instance == (Object)null)
			{
				_locale = "";
				return _en = false;
			}
			_ready = true;
			try
			{
				_locale = ExternalTranslations.ResolveLocale(LocHelper.GetCurrentLocaleCode() ?? "");
				_en = _locale.Length != 0 && !TextTranslations.IsBuiltInLocale(_locale, "zh");
			}
			catch
			{
				_locale = "";
				_en = false;
			}
			return _en;
		}
	}

	internal static string Locale
	{
		get
		{
			_ = En;
			return _locale;
		}
	}

	public static string T(string zh, string en)
	{
		if (!En)
		{
			return zh;
		}
		return TextTranslations.Select(en, _locale);
	}

	public static string K(string key, string zh, string en)
	{
		if (En)
		{
			return TextTranslations.SelectKey(key, en, _locale);
		}
		if (!ExternalTranslations.TryGet(key, en, _locale, out var value))
		{
			return zh;
		}
		return value;
	}

	internal static string K(string key, string en)
	{
		return TextTranslations.SelectKey(key, en, Locale);
	}

	internal static string K(string key, FormattableString en)
	{
		string locale = Locale;
		if (!ExternalTranslations.TryFormat(key, en, locale, out var value))
		{
			return FormatBuiltIn(key, en, locale);
		}
		return value;
	}

	private static string FormatBuiltIn(string key, FormattableString en, string locale)
	{
		string text = TextTranslations.SelectBuiltInKey(key, en.Format, locale);
		if (string.Equals(text, en.Format, StringComparison.Ordinal))
		{
			return en.ToString();
		}
		try
		{
			return string.Format(text, en.GetArguments());
		}
		catch (FormatException)
		{
			return en.ToString();
		}
	}

	public static string K(string key, string zh, FormattableString en)
	{
		bool en2 = En;
		if (ExternalTranslations.TryFormat(key, en, _locale, out var value))
		{
			return value;
		}
		if (!en2)
		{
			return zh;
		}
		return FormatBuiltIn(key, en, _locale);
	}

	public static string K(string key, FormattableString zh, FormattableString en)
	{
		bool en2 = En;
		if (ExternalTranslations.TryFormat(key, en, _locale, out var value))
		{
			return value;
		}
		if (!en2)
		{
			return zh.ToString();
		}
		return FormatBuiltIn(key, en, _locale);
	}

	internal static string F(string key, string zh, string en, params object[] arguments)
	{
		if (En)
		{
			return TextTranslations.FormatKey(key, en, _locale, arguments);
		}
		FormattableString english = FormattableStringFactory.Create(en, arguments);
		if (!ExternalTranslations.TryFormat(key, english, _locale, out var value))
		{
			return string.Format(zh, arguments);
		}
		return value;
	}

	internal static string KRendered(string key, string zh, FormattableString en)
	{
		bool en2 = En;
		if (ExternalTranslations.TryFormat(key, en, _locale, out var value))
		{
			return value;
		}
		if (!en2)
		{
			return zh;
		}
		return TextTranslations.Select(en.ToString(), _locale);
	}

	internal static string Translate(string en)
	{
		return TextTranslations.Select(en, Locale);
	}

	internal static string Translate(FormattableString en)
	{
		string text = Translate(en.Format);
		if (string.Equals(text, en.Format, StringComparison.Ordinal))
		{
			return en.ToString();
		}
		try
		{
			return string.Format(text, en.GetArguments());
		}
		catch (FormatException)
		{
			return en.ToString();
		}
	}

	public static string[] T(string[] zh, string[] en)
	{
		if (!En)
		{
			return zh;
		}
		if (!TextTranslations.HasLocale(_locale) || en == null)
		{
			return en;
		}
		return Array.ConvertAll(en, (string value) => TextTranslations.Select(value, _locale));
	}

	internal static string[] K(string[] keys, string[] zh, string[] en)
	{
		bool en2 = En;
		if (!en2 && (!ExternalTranslations.HasLocale(_locale) || zh == null || en == null))
		{
			return zh;
		}
		if (!TextTranslations.HasLocale(_locale) || en == null)
		{
			return en;
		}
		string[] array = new string[en.Length];
		for (int i = 0; i < en.Length; i++)
		{
			string key = ((keys != null && i < keys.Length) ? keys[i] : null);
			array[i] = (en2 ? TextTranslations.SelectKey(key, en[i], _locale) : (ExternalTranslations.TryGet(key, en[i], _locale, out var value) ? value : zh[i]));
		}
		return array;
	}
}
