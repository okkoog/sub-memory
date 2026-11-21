using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MidnightPawn.Game;

/// <summary>
/// 苗床疲劳系统：按当晚实际行为次数累积，只修正本模组售价；透支时暂停完整夜间结算。
/// 售价惩罚只作用于少女本身，上限 -20%；无早产、无流产。
/// </summary>
public static class MaidenFatigue
{
	public const string FATIGUE_TAG = "MAIDEN_FATIGUE";

	public const int MaxFatigue = 100;

	public const int MilkFloor = 40;

	public const int TiredAt = 60;

	public const int WearyAt = 80;

	public const int ExhaustedAt = 90;

	public const int RestPerDay = 25;

	public const int ExhaustRecover = 5;

	private const string LAST_DECAY_KEY = "last_fatigue_decay_day";

	private const string LAST_REPORT_KEY = "last_fatigue_report_day";

	private const int ReportLineCap = 8;

	public static bool Enabled
	{
		get
		{
			try
			{
				return ModSettings.FatigueEnabled;
			}
			catch
			{
				return true;
			}
		}
	}

	public static float Scale
	{
		get
		{
			try
			{
				return ModSettings.FatigueScale;
			}
			catch
			{
				return 1f;
			}
		}
	}

	public static int Get(GameItem item)
	{
		if (item == null)
		{
			return 0;
		}
		try
		{
			return Mathf.Clamp(ItemFactory.GetInt(item, FATIGUE_TAG), 0, MaxFatigue);
		}
		catch
		{
			return 0;
		}
	}

	public static void Set(GameItem item, int value)
	{
		if (item == null)
		{
			return;
		}
		try
		{
			ItemFactory.SetInt(item, FATIGUE_TAG, Mathf.Clamp(value, 0, MaxFatigue));
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("写疲劳失败: " + ex.Message);
		}
	}

	public static void Add(GameItem item, int delta)
	{
		if (item == null || delta == 0)
		{
			return;
		}
		Set(item, Get(item) + delta);
	}

	public static bool IsExhausted(int fatigue)
	{
		return fatigue >= ExhaustedAt;
	}

	private const string ExhaustedRestDayTag = "SHADOWTRADE_EXHAUSTED_REST_DAY";

	/// <summary>恢复 5 点后也保持本夜暂停，避免同日重入重新活动。</summary>
	internal static bool ShouldRest(GameItem item) => Enabled && item != null &&
		(IsExhausted(Get(item)) || (Today() >= 0 && item.IsTag(ExhaustedRestDayTag)
			&& ItemFactory.GetInt(item, ExhaustedRestDayTag) == Today()));

	internal static void RecoverExhausted(GameItem item)
	{
		int day = Today();
		if (day >= 0 && item.IsTag(ExhaustedRestDayTag) && ItemFactory.GetInt(item, ExhaustedRestDayTag) == day) return;
		if (day >= 0) ItemFactory.SetInt(item, ExhaustedRestDayTag, day);
		Set(item, Mathf.Max(0, Get(item) - ExhaustRecover));
		ItemFactory.UpdateMaidenValue(item);
		ItemFactory.UpdateMaidenDescription(item);
	}

	public static bool IsWeary(int fatigue)
	{
		return fatigue >= WearyAt;
	}

	public static int Today()
	{
		try
		{
			return StoreStation.Instance?.dayCounter ?? (-1);
		}
		catch
		{
			return -1;
		}
	}

	public static string StateName(int fatigue)
	{
		if (fatigue >= ExhaustedAt)
		{
			return "透支";
		}
		if (fatigue >= WearyAt)
		{
			return "疲惫";
		}
		if (fatigue >= TiredAt)
		{
			return "疲倦";
		}
		if (fatigue >= 20)
		{
			return "正常";
		}
		return "休息充足";
	}

	public static string StateColorHex(int fatigue)
	{
		if (fatigue >= ExhaustedAt)
		{
			return "d1483f";
		}
		if (fatigue >= WearyAt)
		{
			return "d8a13a";
		}
		if (fatigue >= TiredAt)
		{
			return "c9b03a";
		}
		if (fatigue >= 20)
		{
			return "b0b7bd";
		}
		return "63c464";
	}

	public static int PaletteColor(int fatigue)
	{
		if (fatigue >= ExhaustedAt)
		{
			return 13715519;
		}
		if (fatigue >= WearyAt)
		{
			return 14711354;
		}
		if (fatigue >= TiredAt)
		{
			return 14197050;
		}
		if (fatigue >= 20)
		{
			return 11581373;
		}
		return 6538340;
	}

	/// <summary>少女自身售价倍率，最高 +10% / 最低 -20%。</summary>
	public static float ValueMul(int fatigue)
	{
		if (!Enabled)
		{
			return 1f;
		}
		if (fatigue >= ExhaustedAt)
		{
			return 0.8f;
		}
		if (fatigue >= WearyAt)
		{
			return 0.88f;
		}
		if (fatigue >= TiredAt)
		{
			return 0.94f;
		}
		if (fatigue < 20)
		{
			return 1.1f;
		}
		return 1f;
	}

	/// <summary>所有设备共用本体等级系数；11 级起不再累积疲劳。</summary>
	public static float LevelMul()
	{
		try
		{
			return TentacleBody.GetBodyLevel() switch
			{
				1 => 1f, 2 => 0.8f, 3 => 0.65f, 4 => 0.5f, 5 => 0.4f,
				6 => 0.3f, 7 => 0.25f, 8 => 0.2f, 9 => 0.15f, 10 => 0.1f,
				_ => 0f
			};
		}
		catch
		{
			return 1f;
		}
	}

	/// <summary>按当晚最终记入的行为次数计算疲劳增长。</summary>
	public static int NightlyGain(int vaginal, int oral, int anal, bool pregnant)
	{
		if (!Enabled)
		{
			return 0;
		}
		int weighted = Mathf.Max(0, vaginal) + Mathf.Max(0, anal) + Mathf.RoundToInt(Mathf.Max(0, oral) * 0.5f);
		if (weighted <= 0)
		{
			return 0;
		}
		float gain = (float)weighted * 0.5f * LevelMul() * (pregnant ? 1.5f : 1f) * Scale;
		if (gain <= 0f) return 0;
		return Mathf.Max(1, Mathf.RoundToInt(gain));
	}

	public static string EffectSummary(int fatigue)
	{
		if (!Enabled)
		{
			return "疲劳系统已关闭";
		}
		if (fatigue >= ExhaustedAt)
		{
			return "本夜全部活动、产出和孕期暂停，售价 -20%";
		}
		if (fatigue >= WearyAt)
		{
			return "售价 -12%";
		}
		if (fatigue >= TiredAt)
		{
			return "售价 -6%";
		}
		if (fatigue < 20)
		{
			return "售价 +10%";
		}
		return "无影响";
	}

	public static string Name(GameItem item)
	{
		if (item == null)
		{
			return "苗床";
		}
		string text = "";
		try
		{
			text = ItemFactory.GetStr(item, "MAIDEN_NAME");
		}
		catch
		{
		}
		if (string.IsNullOrEmpty(text))
		{
			text = item.name;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = "苗床";
		}
		return text;
	}

	public static string Bar(int fatigue)
	{
		int num = Mathf.Clamp(Mathf.RoundToInt((float)fatigue / 10f), 0, 10);
		StringBuilder stringBuilder = new StringBuilder(10);
		for (int i = 0; i < 10; i++)
		{
			stringBuilder.Append((i < num) ? '●' : '○');
		}
		return stringBuilder.ToString();
	}

	public static string TooltipLine(GameItem item)
	{
		int fatigue = Get(item);
		return $"疲劳 {fatigue}/{MaxFatigue} {StateName(fatigue)}  {Bar(fatigue)}";
	}

	public static string RemoveDescription(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		int lineStart = text.LastIndexOf('\n') + 1;
		string lastLine = text.Substring(lineStart);
		if (lastLine.StartsWith("状态：", StringComparison.Ordinal)
			&& lastLine.IndexOf("（疲劳 ", StringComparison.Ordinal) >= 0
			&& lastLine.IndexOf("）｜", StringComparison.Ordinal) >= 0)
		{
			return lineStart == 0 ? "" : text.Substring(0, lineStart - 1).TrimEnd('\r');
		}
		return text;
	}

	/// <summary>喂乳汁：只能把疲劳降到 40，低于 40 时不消耗也不生效。</summary>
	public static bool Feed(GameItem maiden, int relief, string sourceName)
	{
		if (maiden == null || relief <= 0 || !Enabled)
		{
			return false;
		}
		try
		{
			int fatigue = Get(maiden);
			if (fatigue <= MilkFloor)
			{
				Toast.Show($"{Name(maiden)}的状态还算过得去，这个还是留着吧。", sourceName);
				return false;
			}
			int num = Mathf.Max(MilkFloor, fatigue - relief);
			Set(maiden, num);
			ItemFactory.UpdateMaidenValue(maiden);
			ItemFactory.UpdateMaidenDescription(maiden);
			Toast.Show($"{Name(maiden)}把{sourceName}喝了下去，紧绷的身体慢慢软下来。（疲劳 {fatigue} → {num}）", sourceName);
			ModCtx.Log($"喂食: {Name(maiden)} 疲劳 {fatigue} -> {num}（{sourceName}，下限 {MilkFloor}）");
			return true;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("喂食失败: " + ex.Message);
			return false;
		}
	}

	/// <summary>夜间结算完成后、日期递增与自动存档前播报并恢复疲劳。</summary>
	public static void OnNightSettled()
	{
		if (!Enabled)
		{
			return;
		}
		try
		{
			ReportNightly();
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("疲劳播报失败: " + ex.Message);
		}
		try
		{
			DailyDecay();
		}
		catch (Exception ex2)
		{
			ModCtx.LogWarning("疲劳恢复失败: " + ex2.Message);
		}
	}

	/// <summary>不在本体、已启用箱内结算、闹钟或开启的陷阱苗床区的角色，每天恢复 25 点。</summary>
	public static void DailyDecay()
	{
		if (!Enabled)
		{
			return;
		}
		int today = Today();
		if (today < 0)
		{
			return;
		}
		if (ModHelper.GetModDataInt("lewd_meme_remake", LAST_DECAY_KEY, -1) == today)
		{
			return;
		}
		HashSet<GameItem> hashSet = CollectMachineMaidens();
		List<GameItem> list = CollectAllMaidens();
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			GameItem val = list[i];
			if (val == null || hashSet.Contains(val))
			{
				continue;
			}
			int fatigue = Get(val);
			if (fatigue <= 0)
			{
				continue;
			}
			Set(val, Mathf.Max(0, fatigue - RestPerDay));
			ItemFactory.UpdateMaidenValue(val);
			ItemFactory.UpdateMaidenDescription(val);
			num++;
		}
		ModHelper.SetModDataInt("lewd_meme_remake", LAST_DECAY_KEY, today);
		if (num > 0)
		{
			ModCtx.Log($"疲劳恢复: 第 {today} 天，{num} 名场外苗床各恢复 {RestPerDay} 点");
		}
	}

	/// <summary>每晚播报所有处于疲惫 / 透支状态的苗床。</summary>
	public static void ReportNightly()
	{
		if (!Enabled)
		{
			return;
		}
		int today = Today();
		if (today >= 0 && ModHelper.GetModDataInt("lewd_meme_remake", LAST_REPORT_KEY, -1) == today)
		{
			return;
		}
		List<GameItem> list = CollectAllMaidens();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < list.Count; i++)
		{
			GameItem val = list[i];
			if (val == null)
			{
				continue;
			}
			int fatigue = Get(val);
			if (fatigue < WearyAt)
			{
				continue;
			}
			num++;
			if (num2 >= ReportLineCap)
			{
				continue;
			}
			string text = $"【疲劳】{Name(val)} {fatigue}/{MaxFatigue} {StateName(fatigue)}：{EffectSummary(fatigue)}";
			PlayerStore instance = PlayerStore.Instance;
			if (instance != null)
			{
				instance.AddNightLog(text, StateColorHex(fatigue));
			}
			num2++;
		}
		if (num > num2)
		{
			PlayerStore instance2 = PlayerStore.Instance;
			if (instance2 != null)
			{
				instance2.AddNightLog($"【疲劳】另有 {num - num2} 名苗床处于疲惫 / 透支状态。", "b0b7bd");
			}
		}
		if (today >= 0)
		{
			ModHelper.SetModDataInt("lewd_meme_remake", LAST_REPORT_KEY, today);
		}
		if (num > 0)
		{
			ModCtx.Log($"疲劳播报: {num} 名（已播报 {num2} 行）");
		}
	}

	public static void DebugAddAll(int delta)
	{
		try
		{
			List<GameItem> list = CollectAllMaidens();
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == null)
				{
					continue;
				}
				Set(list[i], Get(list[i]) + delta);
				ItemFactory.UpdateMaidenValue(list[i]);
				ItemFactory.UpdateMaidenDescription(list[i]);
				num++;
			}
			Toast.Show($"已对 {num} 名少女调整疲劳 {delta:+#;-#;0}", "疲劳调试");
			ModCtx.Log($"疲劳调试: {num} 名，调整 {delta}");
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("疲劳调试失败: " + ex.Message);
		}
	}

	private static List<GameItem> CollectAllMaidens()
	{
		List<GameItem> list = new List<GameItem>();
		HashSet<GameItem> hashSet = new HashSet<GameItem>();
		try
		{
			EmporiumEntry instance = EmporiumEntry.Instance;
			if (instance == null)
			{
				return list;
			}
			List<GameItem> allItems = instance.GetAllItems();
			if (allItems == null)
			{
				return list;
			}
			for (int i = 0; i < allItems.Count; i++)
			{
				GameItem val = allItems[i];
				if (val != null && val.IsTag("MAIDEN_FEMALE") && hashSet.Add(val))
				{
					list.Add(val);
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("扫描苗床失败: " + ex.Message);
		}
		return list;
	}

	private static HashSet<GameItem> CollectMachineMaidens()
	{
		HashSet<GameItem> result = new HashSet<GameItem>();
		try
		{
			result.UnionWith(TentacleBody.CollectInputMaidens());
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("扫描苗床内少女失败: " + ex.Message);
		}
		try
		{
			result.UnionWith(OriginalDeviceActivityPatch.CollectMaidens());
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("扫描设备苗床失败: " + ex.Message);
		}
		return result;
	}
}
