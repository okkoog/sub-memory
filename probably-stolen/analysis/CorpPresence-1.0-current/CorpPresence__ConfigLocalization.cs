using System;
using System.Collections.Generic;

namespace CorpPresence;

internal static class ConfigLocalization
{
	private const string VisitSectionMarker = " / Visits: ";

	internal static void AfterOpen(LoadedMod mod, CustomUIWindow __result)
	{
		if (__result == null || mod?.Manifest.ID != "CorpPresence" || !TextTranslations.HasLocale(L.Locale))
		{
			return;
		}
		ModConfig val = ModConfig.Find(mod.Manifest.ID);
		if (val == null)
		{
			return;
		}
		__result.SetTitle(ConfigDisplay.Text(mod.Manifest.Name));
		foreach (string section in val.Sections)
		{
			int num = section.IndexOf(" / Visits: ", StringComparison.Ordinal);
			string text = ((num < 0) ? ConfigDisplay.Text(section) : TextTranslations.FormatKey("presentation.configLocalization.afterOpen.visits", "Visits: {0}", L.Locale, ConfigDisplay.Text(section.Substring(num + " / Visits: ".Length))));
			CustomUIElement obj = __result.Get("section_" + section);
			if (obj != null)
			{
				obj.SetText(text);
			}
		}
		foreach (ConfigEntry entry in val.Entries)
		{
			if (entry.Choices == null || entry.Choices.Length == 0)
			{
				continue;
			}
			CustomUIElement val2 = __result.Get(entry.Key);
			int num2 = ((val2 != null) ? val2.GetIndex() : (-1));
			if (num2 >= 0)
			{
				val2.SetOptions((IList<string>)Array.ConvertAll(entry.Choices, (string choice) => ConfigDisplay.Text(choice)));
				val2.SetIndex(num2, false);
			}
		}
	}
}
