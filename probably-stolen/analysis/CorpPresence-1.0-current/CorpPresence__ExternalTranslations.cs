using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace CorpPresence;

internal static class ExternalTranslations
{
	private sealed class Snapshot
	{
		internal readonly Dictionary<string, Dictionary<string, string>> Locales = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);

		internal readonly HashSet<string> Warnings = new HashSet<string>(StringComparer.Ordinal);

		internal string Override;
	}

	private static readonly Regex LocalePattern = new Regex("^[a-z]{2,8}(?:-[a-z0-9]{1,8})*$", RegexOptions.CultureInvariant);

	private static Snapshot _snapshot;

	private static int _version;

	internal static string DefaultDirectory => Path.Combine(Path.GetDirectoryName(typeof(ExternalTranslations).Assembly.Location) ?? AppContext.BaseDirectory, "Translations");

	private static Snapshot Current
	{
		get
		{
			if (_snapshot == null)
			{
				Reload();
			}
			return _snapshot;
		}
	}

	internal static int Version
	{
		get
		{
			_ = Current;
			return _version;
		}
	}

	internal static void Reload()
	{
		Reload(DefaultDirectory);
	}

	internal static void Reload(string directory)
	{
		Snapshot snapshot;
		try
		{
			snapshot = Load(directory);
		}
		catch (Exception ex)
		{
			Warn("外部翻译目录读取失败，沿用内置文本：" + ex.Message);
			snapshot = new Snapshot();
		}
		_snapshot = snapshot;
		_version++;
	}

	internal static void Clear()
	{
		_snapshot = new Snapshot();
		_version++;
	}

	internal static string ResolveLocale(string gameLocale)
	{
		return Current.Override ?? gameLocale;
	}

	internal static bool HasLocale(string locale)
	{
		return Parents(locale).Any((string code) => Current.Locales.ContainsKey(code));
	}

	internal static bool TryGet(string key, string english, string locale, out string value)
	{
		value = null;
		using (IEnumerator<string> enumerator = Candidates(key, english, locale).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				value = current;
				return true;
			}
		}
		return false;
	}

	internal static bool TryFormat(string key, FormattableString english, string locale, out string value)
	{
		value = null;
		foreach (string item in Candidates(key, english.Format, locale))
		{
			try
			{
				value = string.Format(item, english.GetArguments());
				return true;
			}
			catch (FormatException)
			{
				WarnOnce(Current, locale, key, "外部翻译格式无效，已回退：");
			}
		}
		return false;
	}

	private static IEnumerable<string> Candidates(string key, string english, string locale)
	{
		if (key == null || english == null)
		{
			yield break;
		}
		Snapshot snapshot = Current;
		foreach (string item in Parents(locale))
		{
			if (snapshot.Locales.TryGetValue(item, out var value) && value.TryGetValue(key, out var value2))
			{
				if (TranslationFormat.Compatible(english, value2))
				{
					yield return value2;
				}
				else
				{
					WarnOnce(snapshot, item, key, "外部翻译占位符无效，已回退：");
				}
			}
		}
	}

	private static void WarnOnce(Snapshot snapshot, string locale, string key, string message)
	{
		lock (snapshot.Warnings)
		{
			if (snapshot.Warnings.Add(locale + "/" + key))
			{
				Warn(message + locale + "/" + key);
			}
		}
	}

	private static Snapshot Load(string directory)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Invalid comparison between Unknown and I4
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Invalid comparison between Unknown and I4
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Invalid comparison between Unknown and I4
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Invalid comparison between Unknown and I4
		Snapshot result = new Snapshot();
		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
		{
			return result;
		}
		foreach (string item in (from p in Directory.GetFiles(directory)
			where string.Equals(Path.GetExtension(p), ".json", StringComparison.OrdinalIgnoreCase)
			select p).OrderBy<string, string>((string p) => p, StringComparer.Ordinal))
		{
			if (string.Equals(Path.GetFileName(item), "language.json", StringComparison.OrdinalIgnoreCase))
			{
				continue;
			}
			try
			{
				JObject val = Read(item);
				JToken obj = val["schemaVersion"];
				if (obj == null || (int)obj.Type != 6 || (int)val["schemaVersion"] != 1)
				{
					throw new InvalidDataException("schemaVersion must be 1");
				}
				JToken obj2 = val["locale"];
				string text = Normalize((obj2 != null && (int)obj2.Type == 8) ? ((string)val["locale"]) : null);
				if (text == null)
				{
					throw new InvalidDataException("Invalid locale code");
				}
				JToken obj3 = val["translations"];
				JToken obj4 = ((obj3 is JObject) ? obj3 : null) ?? throw new InvalidDataException("Missing translations object");
				if (!result.Locales.TryGetValue(text, out var value))
				{
					value = (result.Locales[text] = new Dictionary<string, string>(StringComparer.Ordinal));
				}
				int num = 0;
				foreach (JProperty item2 in ((JObject)obj4).Properties())
				{
					if ((int)item2.Value.Type != 8)
					{
						num++;
						continue;
					}
					string value2 = (string)item2.Value;
					if (!string.IsNullOrEmpty(item2.Name) && !string.IsNullOrWhiteSpace(value2))
					{
						value[item2.Name] = value2;
					}
				}
				if (num > 0)
				{
					Warn("外部翻译已忽略非字符串条目：" + Path.GetFileName(item));
				}
			}
			catch (Exception ex)
			{
				Warn("外部翻译文件已忽略 " + Path.GetFileName(item) + "：" + ex.Message);
			}
		}
		string text2 = Path.Combine(directory, "language.json");
		if (File.Exists(text2))
		{
			try
			{
				JObject val2 = Read(text2);
				JToken obj5 = val2["locale"];
				string text3 = ((obj5 != null && (int)obj5.Type == 8) ? ((string)val2["locale"]) : null);
				if (!string.IsNullOrEmpty(text3) && !string.Equals(text3.Trim(), "auto", StringComparison.OrdinalIgnoreCase))
				{
					string text4 = Normalize(text3);
					if (text4 == null || !Parents(text4).Any((string code) => result.Locales.ContainsKey(code)))
					{
						throw new InvalidDataException("Selected language has no loaded pack");
					}
					result.Override = text4;
				}
			}
			catch (Exception ex2)
			{
				Warn("翻译语言选择已忽略，使用游戏语言：" + ex2.Message);
			}
		}
		return result;
	}

	private static JObject Read(string file)
	{
		if (new FileInfo(file).Length > 4194304)
		{
			throw new InvalidDataException("Translation file exceeds 4 MiB");
		}
		return JObject.Parse(File.ReadAllText(file, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)));
	}

	private static string Normalize(string locale)
	{
		if (string.IsNullOrWhiteSpace(locale))
		{
			return null;
		}
		string text = locale.Trim().Replace('_', '-').ToLowerInvariant();
		if (!LocalePattern.IsMatch(text))
		{
			return null;
		}
		return text;
	}

	private static IEnumerable<string> Parents(string locale)
	{
		string code = Normalize(locale);
		while (code != null)
		{
			yield return code;
			int num = code.LastIndexOf('-');
			code = ((num < 0) ? null : code.Substring(0, num));
		}
	}

	private static void Warn(string message)
	{
		try
		{
			ModLog.Warn(message);
		}
		catch
		{
		}
	}
}
