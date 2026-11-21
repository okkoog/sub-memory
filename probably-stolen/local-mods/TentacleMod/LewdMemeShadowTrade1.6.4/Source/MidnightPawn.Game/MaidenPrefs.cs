using System;
using System.Collections.Generic;
using System.Text;
using Random = UnityEngine.Random;

namespace MidnightPawn.Game;

public static class MaidenPrefs
{
	public const string OralLike = "MAIDEN_PREF_ORAL_LIKE";

	public const string OralDislike = "MAIDEN_PREF_ORAL_DISLIKE";

	public const string VaginalLike = "MAIDEN_PREF_VAGINAL_LIKE";

	public const string AnalLike = "MAIDEN_PREF_ANAL_LIKE";

	public const string AnalDislike = "MAIDEN_PREF_ANAL_DISLIKE";

	public const string VaginaSensitiveTag = "MAIDEN_SENSITIVE_VAGINA";

	public const string AnalSensitiveTag = "MAIDEN_SENSITIVE_ANAL";

	public const string RolledTag = "MAIDEN_PREFS_ROLLED";

	public static void Assign(GameItem item)
	{
		if (item == null || MaidenAgeFilter.IsYoung(item) || item.IsTag(RolledTag))
		{
			return;
		}
		Enable(item, RolledTag);
		bool alreadyHasPreference = item.IsTag(OralLike) || item.IsTag(OralDislike) || item.IsTag(VaginalLike) || item.IsTag(AnalLike) || item.IsTag(AnalDislike) || item.IsTag(VaginaSensitiveTag) || item.IsTag(AnalSensitiveTag);
		if (alreadyHasPreference)
		{
			return;
		}
		float num = Random.value;
		if (num < 0.15f)
		{
			Enable(item, OralLike);
		}
		else if (num < 0.25f)
		{
			Enable(item, OralDislike);
		}
		if (Random.value < 0.2f)
		{
			Enable(item, VaginalLike);
		}
		num = Random.value;
		if (num < 0.1f)
		{
			Enable(item, AnalLike);
		}
		else if (num < 0.25f)
		{
			Enable(item, AnalDislike);
		}
		num = Random.value;
		if (num < 0.15f)
		{
			Enable(item, VaginaSensitiveTag);
		}
		else if (num < 0.25f)
		{
			Enable(item, AnalSensitiveTag);
		}
	}

	public static string Tags(GameItem item)
	{
		if (item == null || MaidenAgeFilter.IsYoung(item))
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (item.IsTag(OralLike))
		{
			stringBuilder.Append("【口交喜好】");
		}
		if (item.IsTag(OralDislike))
		{
			stringBuilder.Append("【口交厌恶】");
		}
		if (item.IsTag(VaginalLike))
		{
			stringBuilder.Append("【性交喜好】");
		}
		if (item.IsTag(AnalLike))
		{
			stringBuilder.Append("【肛交喜好】");
		}
		if (item.IsTag(AnalDislike))
		{
			stringBuilder.Append("【肛交厌恶】");
		}
		if (item.IsTag(VaginaSensitiveTag))
		{
			stringBuilder.Append("【小穴敏感】");
		}
		if (item.IsTag(AnalSensitiveTag))
		{
			stringBuilder.Append("【屁股敏感】");
		}
		return stringBuilder.ToString();
	}

	public static string Flavor(GameItem item)
	{
		if (item == null || MaidenAgeFilter.IsYoung(item))
		{
			return "";
		}
		List<string> list = new List<string>();
		if (item.IsTag(VaginaSensitiveTag))
		{
			list.Add("小穴敏感得厉害");
		}
		if (item.IsTag(AnalSensitiveTag))
		{
			list.Add("屁股轻轻一碰就会发抖");
		}
		if (item.IsTag(OralLike))
		{
			list.Add("并不排斥口交");
		}
		if (item.IsTag(OralDislike))
		{
			list.Add("提到口交时会别开脸");
		}
		if (item.IsTag(VaginalLike))
		{
			list.Add("被进入时反应格外明显");
		}
		if (item.IsTag(AnalLike))
		{
			list.Add("提到肛交时呼吸会变快");
		}
		if (item.IsTag(AnalDislike))
		{
			list.Add("对肛交仍然很抗拒");
		}
		if (list.Count == 0)
		{
			return "";
		}
		return "她" + string.Join("，", list) + "。";
	}

	private static void Enable(GameItem item, string tag)
	{
		item.ModifyTag(tag, (Action<TagState>)delegate(TagState s)
		{
			s.Enable();
		}, false);
	}
}
