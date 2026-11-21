extern alias OriginalMod;
using NativeApi = OriginalMod::MidnightPawn.Api.TentacleApi;
using System;
using MidnightPawn.Game;

namespace MidnightPawn;

public class ModEntry : IMod
{
	private ModLog log;

	private static bool contrabandRepairDone;

	public void Init(ModManifest manifest)
	{
		log = new ModLog(manifest);
		ModCtx.Attach(log);
		ModCtx.Log("淫色深空：暗流交易 初始化 (v" + manifest.ModVersion + ")");
		LoadedMod prerequisite = CompatOriginal.FindOriginal();
		if (prerequisite == null || !Version.TryParse(prerequisite.Manifest.ModVersion, out Version version) || version < new Version(1, 9, 2))
			throw new InvalidOperationException("暗流交易需要先启用 lewd_meme 1.9.2 或更高版本。");
		ModCtx.Log("已接入原版 " + prerequisite.Manifest.ModVersion + "：孕育由原版处理，本模组追加疲劳、乳汁产物与交易。");
		ModSettings.Init();
		// The loader registers our assembly only after Init and OnEnable return.
		// Checked cross-mod access must wait until item-directory initialization.
		MaidenLedger.EnsureRegistered();
		// 保留原版 tooltip，本模组仅追加疲劳与偏好
		MaidenTooltip.Register();

		ModHook.OnModItemDirectoryInit += RegisterItems;
		ModHook.OnGenerateCustomerVeryLate += MaidenBuyer.OnGenerateCustomerVeryLate;
		ModHook.OnGenerateCustomerVeryLate += SecurityLiaison.OnGenerateCustomerVeryLate;
		ModHook.OnShutterOpenedLate += SecurityLiaison.OnShutterOpened;
		SecurityLiaison.EnsurePhoneClient();
		// 夜间状态结算由 NightlyPersistencePatch 在自动存档前执行。
		ModHook.OnShutterOpenedLate += MaidenLedger.OnShutterOpened;
		ModHook.OnShutterOpenedLate += OnFirstShutterContrabandRepair;
		// 读档后的首次开门补一次违禁词条；不批量重建物品或贴图。
		// 苗床与本体 tooltip 保留原版并追加独有信息
		Perks.Register();
		// 新开档：原版负责发本体/天赋；本模组只标记“首次开门补发名册”
		ModHook.OnNewGameStarted += OnNewGameStarted;
		ModHook.OnGameLoadedNormal += OnGameLoaded;
		// 当前游戏加载器已在 Init 前安装本程序集的声明式 Harmony 补丁。
		try
		{
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "生成少女", (Action)delegate
			{
				GameItem maiden = ModHelper.SpawnItem("maiden_lower");
				if (maiden != null)
				{
					ModHelper.SetItemOwnerShip(maiden, true);
					NativeApi.DeliverItem(maiden);
				}
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "生成收购商", (Action)MaidenBuyer.DebugSpawn, false);

			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "生成触手本体", (Action)delegate
			{
				GameItem val = ModHelper.SpawnItem("tentacle_body");
				if (val != null)
				{
					NativeApi.DeliverItem(val);
				}
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "生成排卵诱发剂", (Action)delegate
			{
				GameItem val2 = ModHelper.SpawnItem("ovulation_shot");
				if (val2 != null)
				{
					ModHelper.SetItemOwnerShip(val2, true);
					NativeApi.DeliverItem(val2);
				}
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "触手本体 +100 经验", (Action)delegate
			{
				TentacleBody.DebugAddExp(100);
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "全部少女疲劳 +30", (Action)delegate
			{
				MaidenFatigue.DebugAddAll(30);
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "全部少女疲劳清零", (Action)delegate
			{
				MaidenFatigue.DebugAddAll(-100);
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "清理多余触手本体", (Action)delegate
			{
				int num2 = TentacleBody.DeduplicateBodies();
				Toast.Show(num2 > 0 ? $"已清理 {num2} 个多余的触手本体。" : "没有找到可清理的多余本体（有内容的本体会被跳过）。", "触手本体");
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "生成苗床名册", (Action)delegate
			{
				MaidenLedger.SpawnLedger();
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "打开苗床名册", (Action)delegate
			{
				MaidenLedger.OpenBook();
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "修复苗床名册外观", (Action)delegate
			{
				MaidenLedger.RepairExisting();
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "治安部：解锁专线", (Action)delegate
			{
				SecurityLiaison.DebugUnlock();
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "治安部：今天上门（送货）", (Action)delegate
			{
				SecurityLiaison.DebugSchedule(SecurityLiaison.ServiceDeliver);
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "治安部：今天上门（归档）", (Action)delegate
			{
				SecurityLiaison.DebugSchedule(SecurityLiaison.ServiceArchive);
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "治安部：今天上门（结案）", (Action)delegate
			{
				SecurityLiaison.DebugSchedule(SecurityLiaison.ServiceStall);
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "治安部：生成名片", (Action)delegate
			{
				GameItem val5 = ModHelper.SpawnItem(SecurityLiaison.CardItemId);
				if (val5 != null)
				{
					ModHelper.SetItemOwnerShip(val5, true);
					NativeApi.DeliverItem(val5);
				}
			}, false);
			ModHelper.AddDebugFunction("lewd_meme_remake", "Spawn", "生成乳汁+精液", (Action)delegate
			{
				GameItem val3 = ModHelper.SpawnItem("milk_wondrous");
				if (val3 != null)
				{
					NativeApi.DeliverItem(val3);
				}
				GameItem val4 = ModHelper.SpawnItem("tentacle_semen");
				if (val4 != null)
				{
					NativeApi.DeliverItem(val4);
				}
			}, false);
		}
		catch (Exception ex2)
		{
			ModCtx.LogWarning("注册调试按钮失败: " + ex2.Message);
		}
	}

	private static void RegisterItems(ModItemDirectory directory)
	{
		ItemFactory.Register(directory);
		CompatOriginal.Install();
	}

	private void OnGameLoaded()
	{
		try
		{
			TentacleBody.ResetCache();
			ItemFactory.MigrateLegacyDaughters();
			MaidenLedger.MarkPendingCheck();
			contrabandRepairDone = false;
			SecurityLiaison.EnsurePhoneClient();
			CompatOriginal.Install();
			TentacleBody.UpgradeLegacyNoteZone();
			if (!CompatOriginal.Detected)
			{
				Toast.Show("缺少前置模组 lewd_meme：请在模组列表里启用原版「可能是触手」。", "缺少前置");
			}
			// 读档补齐旧女儿标识并初始化状态；不重建旧物品、本体或贴图。
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("读档修复启动失败: " + ex.Message);
		}
	}

	/// <summary>读档后第一次开门时补一次违禁品词条；此时物品已完整载入。</summary>
	private static void OnFirstShutterContrabandRepair()
	{
		if (contrabandRepairDone)
		{
			return;
		}
		contrabandRepairDone = true;
		ItemFactory.RepairContrabandOnLoad();
	}

	/// <summary>新开档时原版会发触手本体；本模组只安排第一次开门检查名册。</summary>
	private static void OnNewGameStarted(string scenarioId)
	{
		try
		{
			TentacleBody.ResetCache();
			MaidenLedger.MarkPendingCheck();
			ModCtx.Log("新开档：触手本体由原版发放，苗床名册将在首次开门时补发");
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("新开档初始化失败: " + ex.Message);
		}
	}

	public void OnEnable()
	{
		ModSettings.Init();

		ModCtx.Log("淫色深空：暗流交易 已启用");
	}

	public void OnDisable()
	{
		CompatOriginal.Uninstall();
		ModHelper.StopAllCoroutines("lewd_meme_remake");
		TentacleBody.ResetCache();
		ModHook.OnModItemDirectoryInit -= RegisterItems;
		ModCtx.Log("淫色深空：暗流交易 已停用");
		ModHook.OnGenerateCustomerVeryLate -= MaidenBuyer.OnGenerateCustomerVeryLate;
		ModHook.OnGenerateCustomerVeryLate -= SecurityLiaison.OnGenerateCustomerVeryLate;
		ModHook.OnShutterOpenedLate -= SecurityLiaison.OnShutterOpened;
		ModHook.OnShutterOpenedLate -= MaidenLedger.OnShutterOpened;
		ModHook.OnShutterOpenedLate -= OnFirstShutterContrabandRepair;
		ModHook.OnNewGameStarted -= OnNewGameStarted;
		ModHook.OnGameLoadedNormal -= OnGameLoaded;
		MaidenTooltip.Unregister();

		try
		{
			ModHelper.RemoveDebugFunctions("lewd_meme_remake");
		}
		catch
		{
		}
	}
}

