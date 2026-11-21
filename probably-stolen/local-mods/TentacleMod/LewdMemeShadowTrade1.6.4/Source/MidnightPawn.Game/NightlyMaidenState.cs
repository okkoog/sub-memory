extern alias OriginalMod;
using NativeApi = OriginalMod::MidnightPawn.Api.TentacleApi;

namespace MidnightPawn.Game;

/// <summary>原版计数只读快照，疲劳根据实际增量追加。</summary>
internal sealed class NightlyMaidenState
{
	private readonly int sex, oral, anal;
	private readonly bool pregnant;
	internal NightlyMaidenState(GameItem item)
	{
		sex = NativeApi.GetSexCount(item);
		oral = NativeApi.GetOralCount(item);
		anal = NativeApi.GetAnalCount(item);
		pregnant = NativeApi.IsPregnant(item);
	}
	internal void Complete(GameItem item)
	{
		if (!MaidenFatigue.Enabled) return;
		int gain = MaidenFatigue.NightlyGain(NativeApi.GetSexCount(item) - sex,
			NativeApi.GetOralCount(item) - oral, NativeApi.GetAnalCount(item) - anal, pregnant);
		MaidenFatigue.Add(item, gain);
	}
}

