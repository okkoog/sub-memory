using System;
using System.Collections.Generic;

namespace GameCardPK;

internal static class TagDisplay
{
	private const string P = "game_card_pk.";

	internal static Dictionary<string, CustomTagData> NewTagDatas = BuildTagTable();

	private static readonly HashSet<string> _registeredTags = new HashSet<string>(StringComparer.Ordinal);

	private static bool _initialized;

	private static Dictionary<string, CustomTagData> BuildTagTable()
	{
		Dictionary<string, CustomTagData> dictionary = new Dictionary<string, CustomTagData>();
		Add(dictionary, "CanBattle", DisplayType.None, "可对战", (ColorPalette)12472652);
		Add(dictionary, "OutOfPrint", DisplayType.None, "绝版", (ColorPalette)6511905);
		Add(dictionary, "FirstEdition", DisplayType.None, "初版", (ColorPalette)4365501);
		Add(dictionary, "Memorial", DisplayType.None, "纪念版", (ColorPalette)11567781);
		Add(dictionary, "Rare", DisplayType.None, "稀有版", (ColorPalette)9672266, string.Empty, (Func<GameItem, string>?)RareSeriesLabel.Format);
		Add(dictionary, "Damaged", DisplayType.None, "破损版", (ColorPalette)9454675);
		Add(dictionary, "Tournament", DisplayType.None, "赛事版", (ColorPalette)4367018);
		Add(dictionary, "VariantArt", DisplayType.None, "异画", (ColorPalette)9672266);
		Add(dictionary, "MutantForm", DisplayType.None, "变异", (ColorPalette)8407419);
		Add(dictionary, "HP", DisplayType.Int, "生命值：", (ColorPalette)(-1));
		Add(dictionary, "ATK", DisplayType.Int, "攻击力：", (ColorPalette)(-1));
		Add(dictionary, "SPD", DisplayType.Int, "速度：", (ColorPalette)(-1));
		Add(dictionary, "CoreForce", DisplayType.Int, "核心技能加成：", (ColorPalette)(-1));
		for (int i = 1; i <= 8; i++)
		{
			AddOtherForce(dictionary, i);
		}
		Add(dictionary, "CoreSkill", DisplayType.None, "核心技能", (ColorPalette)9672266);
		Add(dictionary, "Series", DisplayType.Hidden, string.Empty, (ColorPalette)(-1));
		Add(dictionary, "Sticker", DisplayType.Int, "已添加贴纸：", (ColorPalette)9672266);
		Add(dictionary, "Insurance", DisplayType.None, "保险", (ColorPalette)9672266);
		Add(dictionary, "HpScale", DisplayType.Int, "生命缩放（%）：", (ColorPalette)(-1));
		Add(dictionary, "AtkScale", DisplayType.Int, "攻击缩放（%）：", (ColorPalette)(-1));
		Add(dictionary, "SpdScale", DisplayType.Int, "速度缩放（%）：", (ColorPalette)(-1));
		Add(dictionary, "Energy", DisplayType.Int, "能量：", (ColorPalette)9672266);
		Add(dictionary, "SkillSlotMax", DisplayType.Int, "技能槽上限：", (ColorPalette)(-1));
		Add(dictionary, "Difficulty", DisplayType.Int, "难度：", (ColorPalette)9672266);
		Add(dictionary, "Waste", DisplayType.Int, "废纸数：", (ColorPalette)9672266);
		Add(dictionary, "Junk", DisplayType.Int, "垃圾数：", (ColorPalette)9672266);
		Add(dictionary, "ChipDurability", DisplayType.CurrentMax, "芯片耐久：", (ColorPalette)9672266, "game_card_pk.ChipDurabilityMax");
		Add(dictionary, "ChipDurabilityMax", DisplayType.Hidden, string.Empty, (ColorPalette)(-1));
		Add(dictionary, "CardId", DisplayType.Hidden, string.Empty, (ColorPalette)(-1));
		Add(dictionary, "WeaponDurability", DisplayType.Int, "武器耐久：", (ColorPalette)9672266);
		Add(dictionary, "AdvWeaponDurability", DisplayType.Int, "高级武器耐久：", (ColorPalette)9672266);
		Add(dictionary, "MoodValue", DisplayType.Int, "心情值：", (ColorPalette)9672266);
		Add(dictionary, "RewardTier", DisplayType.Hidden, string.Empty, (ColorPalette)(-1));
		Add(dictionary, "AuxEffect", DisplayType.String, "辅助效果：", (ColorPalette)9672266, string.Empty, DragTagLabel.FormatAux);
		return dictionary;
	}

	private static void Add(Dictionary<string, CustomTagData> table, string key, DisplayType type, string displayName, ColorPalette color = (ColorPalette)(-1), string secondId = "", Func<string, string>? formatValue = null)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		table[key] = new CustomTagData("game_card_pk." + key, type, displayName, color, secondId, formatValue);
	}

	private static void Add(Dictionary<string, CustomTagData> table, string key, DisplayType type, string displayName, ColorPalette color, string secondId, Func<GameItem, string>? composeValue)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		table[key] = new CustomTagData("game_card_pk." + key, type, displayName, color, secondId)
		{
			ComposeValue = composeValue
		};
	}

	private static void AddOtherForce(Dictionary<string, CustomTagData> table, int index)
	{
		Add(table, "OtherForce" + index, DisplayType.Int, "技能槽" + index + "加成：", (ColorPalette)(-1));
	}

	internal static void EnsureOtherForceTags(int count)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (count < 0)
		{
			count = 0;
		}
		if (count > 32)
		{
			count = 32;
		}
		for (int i = 1; i <= count; i++)
		{
			string key = "OtherForce" + i;
			if (!NewTagDatas.ContainsKey(key))
			{
				AddOtherForce(NewTagDatas, i);
				if (_initialized)
				{
					CustomTagData customTagData = NewTagDatas[key];
					TagRegister(customTagData.Id, customTagData.DisplayName, customTagData.DisplayType, customTagData.Color, customTagData.SecondId, customTagData.FormatValue, customTagData.ComposeValue);
				}
			}
		}
	}

	internal static void InitTagDisplay()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		EnsureOtherForceTags(GameCardData.MaxSkillSlots);
		if (_initialized)
		{
			return;
		}
		_initialized = true;
		foreach (KeyValuePair<string, CustomTagData> newTagData in NewTagDatas)
		{
			TagRegister(newTagData.Value.Id, newTagData.Value.DisplayName, newTagData.Value.DisplayType, newTagData.Value.Color, newTagData.Value.SecondId, newTagData.Value.FormatValue, newTagData.Value.ComposeValue);
		}
	}

	internal static bool TryGetTagId(string key, out string id)
	{
		id = string.Empty;
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		if (!NewTagDatas.TryGetValue(key, out CustomTagData value) || value == null)
		{
			return false;
		}
		id = value.Id;
		return !string.IsNullOrEmpty(id);
	}

	internal static string FullTag(string key)
	{
		string text = ItemConfigInjector.BareId(Core.Clean(key));
		if (text.Length == 0)
		{
			return string.Empty;
		}
		if (TryGetTagId(text, out string id) && !string.IsNullOrEmpty(id))
		{
			return id;
		}
		return ItemConfigInjector.Prefixed(text);
	}

	internal static void TagRegister(string tag, string displayName, DisplayType displayType, ColorPalette colorPalette, string secondId = "", Func<string, string>? formatValue = null, Func<GameItem, string>? composeValue = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(tag))
		{
			return;
		}
		if (!_registeredTags.Add(tag))
		{
			Core.Debug("[卡牌PK] 标签已注册，忽略重复登记：" + tag);
		}
		else
		{
			if (displayType == DisplayType.Hidden)
			{
				return;
			}
			Action<RichTextBuilder, GameItem> action = delegate(RichTextBuilder builder, GameItem item)
			{
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0156: Unknown result type (might be due to invalid IL or missing references)
				//IL_0139: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00da: Unknown result type (might be due to invalid IL or missing references)
				if (builder == null || item == null)
				{
					return;
				}
				try
				{
					if (item.IsTag(tag))
					{
						TagState tagReadonly = item.GetTagReadonly(tag);
						if (tagReadonly != null)
						{
							switch (displayType)
							{
							case DisplayType.Int:
								builder.AddLine(displayName + tagReadonly.GetInt(), true, colorPalette, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
								break;
							case DisplayType.String:
							{
								string text2 = Core.Clean(tagReadonly.GetString());
								if (text2.Length != 0)
								{
									string text3 = ((formatValue != null) ? Core.Clean(formatValue(text2)) : text2);
									if (text3.Length != 0)
									{
										builder.AddLine(displayName + text3, true, colorPalette, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
									}
								}
								break;
							}
							case DisplayType.None:
								if (composeValue != null)
								{
									string text = Core.Clean(composeValue(item));
									if (text.Length != 0)
									{
										builder.AddLine(text, true, colorPalette, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
									}
								}
								else
								{
									builder.AddLine(displayName, true, colorPalette, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
								}
								break;
							case DisplayType.CurrentMax:
								RenderCurrentMax(builder, item, displayName, colorPalette, secondId, tagReadonly.GetInt());
								break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Core.Debug("[卡牌PK] tooltip 渲染失败（标签 " + tag + "）：" + ex.Message);
				}
			};
			Action<RichTextBuilder, GameItem> action2 = action;
			if (action2 == null)
			{
				Core.Log?.Error("Tag '" + tag + "' 注册失败：无法转换为 Il2Cpp 委托");
				return;
			}
			ModHook.OnCreateTooltipLate += action2;
			Core._pinned.Add(action);
			Core._pinned.Add(action2);
		}
	}

	private static void RenderCurrentMax(RichTextBuilder builder, GameItem item, string displayName, ColorPalette colorPalette, string maxTag, int rawCurrent)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if (!string.IsNullOrEmpty(maxTag))
		{
			try
			{
				if (item.IsTag(maxTag))
				{
					TagState tagReadonly = item.GetTagReadonly(maxTag);
					if (tagReadonly != null)
					{
						num = tagReadonly.GetInt();
					}
				}
			}
			catch (Exception ex)
			{
				Core.Debug("[卡牌PK] 读取上限标签失败（按当前值处理）：" + maxTag + " — " + ex.Message);
			}
		}
		int num2 = rawCurrent;
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num <= 0)
		{
			num = ((num2 <= 0) ? 1 : num2);
		}
		if (num2 > num)
		{
			num2 = num;
		}
		ColorPalette val = (ColorPalette)(((float)num2 / (float)num <= 0.2f) ? 12822958 : ((int)colorPalette));
		builder.AddLine(displayName + num2 + "/" + num, true, val, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
	}
}
