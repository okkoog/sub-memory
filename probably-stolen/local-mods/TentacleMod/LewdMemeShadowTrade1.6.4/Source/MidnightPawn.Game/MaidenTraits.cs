using Random = UnityEngine.Random;
using UnityEngine;

namespace MidnightPawn.Game;

public static class MaidenTraits
{
	public static readonly string[] JOBS = new string[20]
	{
		"拾荒者", "矿工", "雇佣兵", "工程师", "护士", "医生", "厨师", "农场主", "小偷", "商人",
		"清洁员", "技师", "文员", "走私者", "货运员", "酒保", "维修工", "研究员", "学徒", "艺人"
	};

	public static readonly string[] BREASTS = new string[3] { "贫乳", "普乳", "巨乳" };

	public static string Pick(string[] arr)
	{
		return arr[Random.Range(0, arr.Length)];
	}
}

