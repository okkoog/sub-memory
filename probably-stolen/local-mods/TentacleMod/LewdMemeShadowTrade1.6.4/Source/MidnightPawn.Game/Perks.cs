using System;
using System.Collections.Generic;

namespace MidnightPawn.Game;

public static class Perks
{
	public const string PERK_TENTACLE = "lewd_meme_remake.tentacle_origin";

	public const string PERK_TENTACLE_LEGACY = "lewd_meme.tentacle_origin";

	/// <summary>老存档里可能还挂着旧 ID 的天赋（原 mod / 旧魔改版），一并识别。</summary>
	public static bool IsPerkActive()
	{
		try
		{
			return ModHelper.IsStartingPerkActive(PERK_TENTACLE) || ModHelper.IsStartingPerkActive(PERK_TENTACLE_LEGACY);
		}
		catch
		{
			return false;
		}
	}

	public static void Register()
	{
		// 原版是前置：起始天赋、开局发本体、每日顾客都交给原版，本模组不再注册/监听
		ModCtx.Log("起始天赋「触手起源」由前置原版提供，本模组不重复注册");
	}

}
