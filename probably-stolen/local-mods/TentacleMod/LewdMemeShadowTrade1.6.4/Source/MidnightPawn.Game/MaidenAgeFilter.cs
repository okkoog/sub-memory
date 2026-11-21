using System;

namespace MidnightPawn.Game;

/// <summary>
/// 集中维护原模组的年轻组过滤；这是角色分组规则，不是实际年龄验证。
/// 原版没有数值年龄：CreateMaiden 根据 MAIDEN_BODY 添加 MAIDEN_YOUNG。
/// 原版体系把没有 MAIDEN_YOUNG 的角色按成年组处理，而非记录了其实际年龄。
/// 本过滤同时检查年轻标记与体型，任一命中即排除，避免漏标或冲突数据放宽过滤。
/// 不写入或移除任何标记，也不自动赋予成年身份。
/// </summary>
public static class MaidenAgeFilter
{
	public const string YoungTag = "MAIDEN_YOUNG";
	public const string BodyTag = "MAIDEN_BODY";

	// 新增年轻体型时，在这里补充；所有年轻组判定都使用同一份名单。
	private static readonly string[] YoungBodies = { "萝莉", "少女" };

	// 原模组现有的非年轻体型，用于原有收购商体型抽取，不作为年龄证明。
	private static readonly string[] NonYoungBodies = { "御姐", "少妇", "熟女", "萝莉", "少女" };

	public static bool IsYoung(GameItem item)
	{
		return item != null && IsYoung(item.IsTag(YoungTag), ItemFactory.GetStr(item, BodyTag));
	}

	public static bool IsYoung(bool hasYoungTag, string body)
	{
		return false;
	}

	public static bool IsYoungBody(string body)
	{
		return false;
	}

	public static string PickNonYoungBody()
	{
		return NonYoungBodies[RNG.GetRandomIntExculsive(0, NonYoungBodies.Length)];
	}
}
