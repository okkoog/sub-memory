using System;
using System.Text;
using MidnightPawn.Api;

namespace MidnightPawn.Game;

public static class MaidenTooltip
{
	private static bool registered;

	public static void Register()
	{
		if (registered)
		{
			return;
		}
		registered = true;
		ModHook.OnCreateTooltipLate += OnTooltip;
	}

	public static void Unregister()
	{
		if (!registered)
		{
			return;
		}
		registered = false;
		ModHook.OnCreateTooltipLate -= OnTooltip;
	}

	private static void OnTooltip(RichTextBuilder builder, GameItem item)
	{
		if (builder == null || item == null)
		{
			return;
		}
		if (item.IsTag("TENTACLE_BODY_TAG"))
		{
			try
			{
				// 原版已显示接入实际统计的昨晚摘要；详细魔改产出保留在报告便签。
				TooltipApi.Raise(builder, item, TooltipSection.Body);
			}
			catch (Exception ex)
			{
				ModCtx.LogWarning("触手本体 tooltip 失败: " + ex.Message);
			}
			return;
		}
		if (item.identifier == "corrupt_tentacle")
		{
			TooltipApi.Raise(builder, item, TooltipSection.CorruptTentacle);
			return;
		}
		if (item.identifier == "tentacle_box" || item.IsTag("TENTACLE_CORRUPTION"))
		{
			TooltipApi.Raise(builder, item, TooltipSection.TentacleBox);
			return;
		}
		if (ItemFactory.IsBarrelIdentifier(item.identifier))
		{
			TooltipApi.Raise(builder, item, TooltipSection.Barrel);
			return;
		}
		if (!item.IsTag("MAIDEN_FEMALE"))
		{
			TooltipApi.Raise(builder, item, TooltipSection.Other);
			return;
		}
		try
		{
			// The description is rendered after this hook; remove saved legacy fatigue text too.
			string description = item.shortDescription;
			string cleanedDescription = MaidenFatigue.RemoveDescription(description);
			if (cleanedDescription != description)
			{
				item.shortDescription = cleanedDescription;
			}
			// 原版捕获的苗床没有本模组偏好：第一次查看时补一次（幂等，不会覆盖已有偏好）
			MaidenPrefs.Assign(item);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(MaidenPrefs.Tags(item));
			if (stringBuilder.Length > 0)
			{
				builder.AddEmptyLine(2);
				AddLine(builder, stringBuilder.ToString(), 11567781);
			}
			if (MaidenFatigue.Enabled)
			{
				AddLine(builder, MaidenFatigue.TooltipLine(item), MaidenFatigue.PaletteColor(MaidenFatigue.Get(item)));
			}
			TooltipApi.Raise(builder, item, TooltipSection.Maiden);
		}
		catch (Exception ex2)
		{
			ModCtx.LogWarning("少女 tooltip 追加失败: " + ex2.Message);
		}
	}

	private static void AddLine(RichTextBuilder builder, string text, int color)
	{
		builder.AddLine(text, false, (RenderHandler.ColorPalette)color, false, false, false, false, (RenderHandler.ColorPalette)(-1), (RenderHandler.ColorPalette)(-1), (RenderHandler.ColorPalette)(-1));
	}
}

