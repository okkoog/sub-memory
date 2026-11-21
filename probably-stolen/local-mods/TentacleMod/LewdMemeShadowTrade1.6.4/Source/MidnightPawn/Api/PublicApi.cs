using System;
using System.Collections.Generic;
using MidnightPawn.Game;

namespace MidnightPawn.Api;

/// <summary>tooltip 对外接口，按段落触发。</summary>
public static class TooltipApi
{
	public static event Action<TooltipContext> BuildTooltip;

	internal static void Raise(RichTextBuilder builder, GameItem item, TooltipSection section)
	{
		Action<TooltipContext> handler = BuildTooltip;
		if (handler == null)
		{
			return;
		}
		TooltipContext context = new TooltipContext(builder, item, section);
		foreach (Action<TooltipContext> callback in handler.GetInvocationList())
		{
			try
			{
				callback(context);
			}
			catch (Exception ex)
			{
				ModCtx.LogWarning("tooltip 接口 " + section + " 回调失败: " + ex.Message);
			}
		}
	}
}

public enum TooltipSection
{
	Body,
	CorruptTentacle,
	TentacleBox,
	Barrel,
	Maiden,
	Other
}

public sealed class TooltipContext
{
	internal TooltipContext(RichTextBuilder builder, GameItem item, TooltipSection section)
	{
		Builder = builder;
		Item = item;
		Section = section;
	}

	public RichTextBuilder Builder { get; private set; }

	public GameItem Item { get; private set; }

	public TooltipSection Section { get; private set; }
}

