extern alias OriginalMod;
using NativeApi = OriginalMod::MidnightPawn.Api.TentacleApi;
using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace MidnightPawn.Game;

/// <summary>
/// 治安部线人：第一次卖出苗床后上门送名片，之后通过原版电话 26535 约见，
/// 在柜台上交货推进五段剧情，并在终章后提供「归档」「结案」两项服务。
/// </summary>
public static class SecurityLiaison
{
	public const string ModId = "lewd_meme_remake";

	/// <summary>治安部线人专线号码。</summary>
	public const long Number = 26535L;

	public const string PhoneDialogId = "SecLiaison";

	public const string CardItemId = "lewd_meme_phone_card";

	public const string ClientTag = "lewd_liaison";

	public const string CardWindowId = "lewd_meme_remake.phone_card";

	private const string KeyStage = "liaison_stage";

	private const string KeyIntro = "liaison_intro_done";

	// 0=尚未触发，1=已安排，2=已加入来访队列；与对话完成状态分开保存。
	private const string KeyIntroVisit = "liaison_intro_visit_state";

	private const string KeyVisitDay = "liaison_visit_day";

	private const string KeyVisitService = "liaison_visit_service";

	private const string KeyCooldown = "liaison_cooldown";

	private const string KeyCredit = "liaison_crime_credit";

	private const string KeyRegSemen = "liaison_reg_semen";

	private const string KeyRegMaiden = "liaison_reg_maiden";

	private const string KeyRegOvulation = "liaison_reg_ovulation";

	private const string KeyLastTick = "liaison_last_tick";

	public const string ServiceDeliver = "deliver";

	public const string ServiceIntro = "intro";

	public const string ServiceArchive = "archive";

	public const string ServiceStall = "stall";

	/// <summary>阶段 1~4 的声望门槛。</summary>
	public static int RequiredReputation(int stage)
	{
		return stage switch
		{
			2 => 40, 
			3 => 60, 
			4 => 80, 
			_ => 0, 
		};
	}

	public static int Stage => ModHelper.GetModDataInt(ModId, KeyStage, 0);

	public static bool IntroDone => ModHelper.GetModDataInt(ModId, KeyIntro, 0) == 1;

	public static StorePhoneClient PhoneClient()
	{
		try
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance != null && instance.PhoneClientDict != null && instance.PhoneClientDict.TryGetValue(Number, out StorePhoneClient value))
			{
				return value;
			}
		}
		catch
		{
		}
		return null;
	}

	/// <summary>冷却天数：直接读原版电话联系人的冷却状态。</summary>
	public static int Cooldown
	{
		get
		{
			try
			{
				StorePhoneClient client = PhoneClient();
				if (client != null && client.phoneState == StorePhoneClient.PhoneState.Cooldown)
				{
					return Mathf.Max(0, client.currentCooldown);
				}
			}
			catch
			{
			}
			return 0;
		}
	}

	public static int CrimeCredit => Math.Max(0, ModHelper.GetModDataInt(ModId, KeyCredit, 0));

	// Stage 保存的是下一个交货节点；完成的剧情本身也能恢复旧档遗漏的登记奖励。
	public static bool SemenRegistered => Stage >= 4 || ModHelper.GetModDataInt(ModId, KeyRegSemen, 0) == 1;

	public static bool MaidenRegistered => Stage >= 5 || ModHelper.GetModDataInt(ModId, KeyRegMaiden, 0) == 1;

	public static bool OvulationRegistered => Stage >= 3 || ModHelper.GetModDataInt(ModId, KeyRegOvulation, 0) == 1;

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

	public static int SecurityReputation()
	{
		try
		{
			return StoreReputation.GetStoreReputation("FACTION_SECURITY")?.GetReputation() ?? 0;
		}
		catch
		{
			return 0;
		}
	}

	public static void AddReputation(int value)
	{
		try
		{
			StoreReputation.GetSecFaction().ModReputation(value);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("治安部声望变更失败: " + ex.Message);
		}
	}

	// ───────────────────────── 电话 ─────────────────────────

	/// <summary>把专线写进电话本（已存在则只同步状态）。</summary>
	public static void EnsurePhoneClient()
	{
		try
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance == null || instance.PhoneClientDict == null)
			{
				return;
			}
			StorePhoneClient client;
			if (!instance.PhoneClientDict.TryGetValue(Number, out client) || client == null)
			{
				client = new StorePhoneClient
				{
					displayName = "治安部线人",
					dialogFuncId = PhoneDialogId,
					phoneState = (IntroDone ? StorePhoneClient.PhoneState.Regular : StorePhoneClient.PhoneState.NotResponding),
					dialedBefore = IntroDone,
					cooldownDuration = 3
				};
				instance.PhoneClientDict[Number] = client;
				ModCtx.Log("治安部专线已写入电话本: " + Number);
			}
			else
			{
				client.displayName = "治安部线人";
				client.dialogFuncId = PhoneDialogId;
				client.cooldownDuration = 3;
				if (IntroDone && client.phoneState == StorePhoneClient.PhoneState.NotResponding)
				{
					client.phoneState = StorePhoneClient.PhoneState.Regular;
					client.dialedBefore = true;
				}
				else if (!IntroDone)
				{
					client.phoneState = StorePhoneClient.PhoneState.NotResponding;
					client.dialedBefore = false;
				}
			}
			if (PhoneClientList.DialogFuncs != null && !PhoneClientList.DialogFuncs.ContainsKey(PhoneDialogId))
			{
				PhoneClientList.DialogFuncs[PhoneDialogId] = BuildPhoneDialog;
			}
		}
		catch (Exception ex2)
		{
			ModCtx.LogWarning("治安部专线写入失败: " + ex2.Message);
		}
	}

	private static void EndCall(Dialogue dialogue)
	{
		dialogue.SetEndAction(delegate
		{
			PhoneUIManager.Instance.OnPhoneDialogEnd();
		});
	}

	private static Dialogue PlayerLine(Dialogue dialogue, string text)
	{
		return dialogue.SetText("PLAYER", text);
	}

	public static Dialogue BuildPhoneDialog(StorePhoneClient client)
	{
		EnsurePhoneClient();
		Dialogue dialogue = new Dialogue();
		string line = "治安部线人";
		try
		{
			if (!IntroDone)
			{
				PlayerLine(dialogue, "……（无人接听）");
				EndCall(dialogue);
				return dialogue;
			}
			string pendingService = ModHelper.GetModData(ModId, KeyVisitService, "");
			int pendingDay = ModHelper.GetModDataInt(ModId, KeyVisitDay, -1);
			if (!string.IsNullOrEmpty(pendingService) && pendingDay >= 0 && pendingDay >= Today())
			{
				Dialogue pending = dialogue.SetText(line, "我已经排好行程了。");
				pending = pending.NextDialogue().SetText(line, "别催。催了我反而慢点走。");
				pending = pending.NextDialogue().SetText("PLAYER", "……好。");
				EndCall(pending);
				return dialogue;
			}
			if (Cooldown > 0)
			{
				Dialogue cool = dialogue.SetText(line, "这几天别找我。我也要避嫌。");
				cool = cool.NextDialogue().SetText(line, $"上面有人盯着，我还得再等 {Cooldown} 天。");
				cool = cool.NextDialogue().SetText("PLAYER", "……行。");
				EndCall(cool);
				return dialogue;
			}
			if (Stage >= 5)
			{
				return BuildServiceMenu(dialogue, line);
			}
			int required = RequiredReputation(Stage);
			if (required > 0 && SecurityReputation() < required)
			{
				BuildRefuse(dialogue, line, Stage, required);
				return dialogue;
			}
			BuildRequirementCall(dialogue, line, Stage);
			return dialogue;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("治安部电话对话构建失败: " + ex.Message);
			dialogue.SetText(line, "……（杂音）");
			EndCall(dialogue);
			return dialogue;
		}
	}

	/// <summary>当前阶段的货物需求（给电话和日志用）。</summary>
	public static string NeedSummary(int stage)
	{
		return stage switch
		{
			1 => "5 份触手精液", 
			2 => "1 名双处女的下层区苗床", 
			3 => "30 份触手精液", 
			4 => "1 名苗床出生的女儿（调教值 > 200）", 
			_ => "无", 
		};
	}

	private static void BuildRequirementCall(Dialogue dialogue, string line, int stage)
	{
		string readyText;
		string noText;
		Dialogue node;
		switch (stage)
		{
		case 2:
			node = dialogue.SetText(line, "……是我。");
			node = node.NextDialogue().SetText(line, "这次不是土产。我要一件'干净的货'。");
			node = node.NextDialogue().SetText(line, "双处女，下层区出来的。别拿调教过的糊弄我，我一眼就能看出来。");
			readyText = "明天过午。东西摆柜台上，人别绑太紧，我要验。";
			noText = "行。那你先把她养干净。记住，我要的是没动过的。";
			break;
		case 3:
			node = dialogue.SetText(line, "……喂。听我说，别插嘴。");
			node = node.NextDialogue().SetText(line, "三十份触手精液。一滴都不能少。");
			node = node.NextDialogue().SetText(line, "部里最近压着一批货，上面催得紧。");
			readyText = "明天过午。别再让我空手回去。";
			noText = "三十份，一份都不能少。数不对，我可不认。";
			break;
		case 4:
			node = dialogue.SetText(line, "最后一个。");
			node = node.NextDialogue().SetText(line, "我要一个'女儿'——你自己养出来的那种，在你店里出生的。");
			node = node.NextDialogue().SetText(line, "还得是调教过的，调教值两百以上。别拿刚出生的糊弄我。");
			readyText = "明天过午。这是最后一趟，别搞砸。";
			noText = "那就慢慢养。养到她能上台面为止。我等得起。";
			break;
		default:
			node = dialogue.SetText(line, "……喂。别出声，听着就行。");
			node = node.NextDialogue().SetText(line, "五份。触手精液。你自己知道是什么。");
			node = node.NextDialogue().SetText(line, "明天过午，我到你店里。东西摆柜台上，别用袋子装，我看着不舒服。");
			readyText = "明天见。别让我等——我这人，等久了会想公事公办。";
			noText = "那就等你准备好。别太久。我记性不好，记性不好的人，容易把卷宗写歪。";
			break;
		}
		node = node.NextDialogue().SetText(line, "我要的是：" + NeedSummary(stage) + "。");
		node = node.NextDialogue().SetText(line, "准备好了吗？");
		Dialogue ready = new Dialogue();
		PlayerLine(ready, "准备好了。");
		Dialogue notReady = new Dialogue();
		PlayerLine(notReady, "还没。");
		node.AddChoice(DirectoryMaster.Item("$"), "准备好了", "", ready, delegate
		{
			ScheduleVisit(ServiceDeliver, stage);
		}).AddChoice(DirectoryMaster.Item("$"), "还没准备好", "", notReady, null);
		ready.NextDialogue().SetText(line, readyText);
		EndCall(ready);
		notReady.NextDialogue().SetText(line, noText);
		EndCall(notReady);
	}

	private static void BuildRefuse(Dialogue dialogue, string line, int stage, int required)
	{
		int rep = SecurityReputation();
		Dialogue node;
		switch (stage)
		{
		case 2:
			node = dialogue.SetText(line, "四十。你连四十都不到，就想让我替你在上面说话？");
			node = node.NextDialogue().SetText(line, "先把上一单做漂亮。你这种名声，我连你的名字都不敢往上报。");
			break;
		case 3:
			node = dialogue.SetText(line, "六十。你现在这个数，还进不了这间屋子。");
			node = node.NextDialogue().SetText(line, "我要往上递你的名字，别人会问我：这人是谁？我答不上来。");
			break;
		default:
			node = dialogue.SetText(line, "八十。你差得还远。");
			node = node.NextDialogue().SetText(line, "这一单不是钱的问题。这是要我把整个部门的信誉压上去。你还不值这个价。");
			break;
		}
		node = node.NextDialogue().SetText("PLAYER", "……");
		node = node.NextDialogue().SetText(line, $"治安部声望 {rep} / {required}。凑够了再打。");
		EndCall(node);
		Toast.Show($"治安部声望不足：{rep} / {required}", "治安部线人");
	}

	private static Dialogue BuildServiceMenu(Dialogue dialogue, string line)
	{
		dialogue.SetText(line, "……说。哪件事？");
		Dialogue archiveNode = new Dialogue();
		Dialogue stallNode = new Dialogue();
		Dialogue hangNode = new Dialogue();
		PlayerLine(archiveNode, "帮我抹掉案底。");
		PlayerLine(stallNode, "压一压调查。");
		PlayerLine(hangNode, "没事，挂了。");
		dialogue.AddChoice(DirectoryMaster.Item("$"), "帮我抹掉案底", "", archiveNode, delegate
		{
			BuildArchiveCall(archiveNode, line);
		}).AddChoice(DirectoryMaster.Item("$"), "压一压调查", "", stallNode, delegate
		{
			BuildStallCall(stallNode, line);
		})
			.AddChoice(DirectoryMaster.Item("$"), "没事，挂了", "", hangNode, null);
		hangNode.NextDialogue().SetText(line, "别浪费我的号线。");
		EndCall(hangNode);
		return dialogue;
	}

	/// <summary>按总案值分档：0=不做，1~4=四档。</summary>
	public static int ArchiveTier(int totalCrime)
	{
		if (totalCrime < 200)
		{
			return 0;
		}
		if (totalCrime < 800)
		{
			return 1;
		}
		if (totalCrime < 1600)
		{
			return 2;
		}
		if (totalCrime < 2500)
		{
			return 3;
		}
		return 4;
	}

	public static void ArchiveCost(int tier, out int richMilk, out int revolutionaryMaidens, out int blackMarketMaidens)
	{
		switch (tier)
		{
		case 1:
			richMilk = 6;
			revolutionaryMaidens = 0;
			blackMarketMaidens = 0;
			break;
		case 2:
			richMilk = 6;
			revolutionaryMaidens = 1;
			blackMarketMaidens = 0;
			break;
		case 3:
			richMilk = 6;
			revolutionaryMaidens = 1;
			blackMarketMaidens = 1;
			break;
		case 4:
			richMilk = 12;
			revolutionaryMaidens = 0;
			blackMarketMaidens = 0;
			break;
		default:
			richMilk = 0;
			revolutionaryMaidens = 0;
			blackMarketMaidens = 0;
			break;
		}
	}

	public static string ArchiveCostText(int richMilk, int revolutionaryMaidens, int blackMarketMaidens)
	{
		string text = richMilk + " 瓶浓缩乳汁";
		if (revolutionaryMaidens > 0)
		{
			text = text + " + " + revolutionaryMaidens + " 名革命军苗床";
		}
		if (blackMarketMaidens > 0)
		{
			text = text + " + " + blackMarketMaidens + " 名黑市苗床";
		}
		return text;
	}

	private static void BuildArchiveCall(Dialogue node, string line)
	{
		int tier = ArchiveTier(TotalCrime());
		Dialogue say;
		if (tier == 0)
		{
			say = node.NextDialogue().SetText(line, "这点小事也值得我跑一趟？");
			say = say.NextDialogue().SetText(line, "留着自己消化。等数字难看了再来找我。");
			EndCall(say);
			return;
		}
		ArchiveCost(tier, out int richMilk, out int revMaidens, out int bmMaidens);
		if (tier == 4)
		{
			say = node.NextDialogue().SetText(line, "这个数……你已经在名单的第三页了。");
			say = say.NextDialogue().SetText(line, ArchiveCostText(richMilk, revMaidens, bmMaidens) + "。我要拿它们去换一份'上面签字'。");
		}
		else
		{
			say = node.NextDialogue().SetText(line, "抹多少，得看你欠了多少。东西按数备好，我明天来取。");
			say = say.NextDialogue().SetText(line, ArchiveCostText(richMilk, revMaidens, bmMaidens) + "。别嫌多，流程就是这么长。");
		}
		say = say.NextDialogue().SetText(line, "准备好了吗？");
		Dialogue ready = new Dialogue();
		PlayerLine(ready, "准备好了。");
		Dialogue notReady = new Dialogue();
		PlayerLine(notReady, "还没。");
		say.AddChoice(DirectoryMaster.Item("$"), "准备好了", "", ready, delegate
		{
			ScheduleVisit(ServiceArchive, 0);
		}).AddChoice(DirectoryMaster.Item("$"), "还没准备好", "", notReady, null);
		ready.NextDialogue().SetText(line, "明天过午。这次数要是不对，我可不会再给你机会。");
		EndCall(ready);
		notReady.NextDialogue().SetText(line, "那就备齐了再打。别让我白跑。");
		EndCall(notReady);
	}

	private static void BuildStallCall(Dialogue node, string line)
	{
		if (PlayerStore.Instance == null || PlayerStore.Instance.secData == null)
		{
			EndCall(node.NextDialogue().SetText(line, "……（信号断了）"));
			return;
		}
		SecData sec = PlayerStore.Instance.secData;
		if (sec.IsOfficerPending())
		{
			Dialogue say2 = node.NextDialogue().SetText(line, "人已经在路上了，我拦不住。");
			say2 = say2.NextDialogue().SetText(line, "你自己想办法。别把我也写进去。");
			EndCall(say2);
			return;
		}
		if (sec.evidenceLevel < 20)
		{
			Dialogue say3 = node.NextDialogue().SetText(line, "还没到需要我动手的时候。");
			say3 = say3.NextDialogue().SetText(line, "别浪费我的时间，也别浪费你自己的钱。");
			EndCall(say3);
			return;
		}
		Dialogue say = node.NextDialogue().SetText(line, "卷宗总得有个结尾。你给我两件'证物'，我给它一个结尾。");
		say = say.NextDialogue().SetText(line, "一个革命军的，一个黑市的，调教值都得至少两百。要活的，死的写不进结案报告。");
		say = say.NextDialogue().SetText(line, "准备好了吗？");
		Dialogue ready = new Dialogue();
		PlayerLine(ready, "准备好了。");
		Dialogue notReady = new Dialogue();
		PlayerLine(notReady, "还没。");
		say.AddChoice(DirectoryMaster.Item("$"), "准备好了", "", ready, delegate
		{
			ScheduleVisit(ServiceStall, 0);
		}).AddChoice(DirectoryMaster.Item("$"), "还没准备好", "", notReady, null);
		ready.NextDialogue().SetText(line, "明天过午。这次别再让我空手回去。");
		EndCall(ready);
		notReady.NextDialogue().SetText(line, "慢慢凑。卷宗不会自己合上。");
		EndCall(notReady);
	}

	public static int TotalCrime()
	{
		try
		{
			return PlayerStore.Instance?.secData?.GetTotalCrimeValue() ?? 0;
		}
		catch
		{
			return 0;
		}
	}

	public static string ServiceName(string service)
	{
		return service switch
		{
			ServiceArchive => "归档", 
			ServiceStall => "结案", 
			ServiceIntro => "初次接触", 
			_ => "送货", 
		};
	}

	public static void ScheduleVisit(string service, int stage)
	{
		if (service == ServiceIntro && IntroVisitAlreadyTriggered()) return;
		int today = Today();
		int existingDay = ModHelper.GetModDataInt(ModId, KeyVisitDay, -1);
		string existingService = ModHelper.GetModData(ModId, KeyVisitService, "");
		if (existingDay >= today && !string.IsNullOrEmpty(existingService))
		{
			ModCtx.Log("治安部线人已有预约，忽略重复预约: " + existingService);
			return;
		}
		ModHelper.SetModDataInt(ModId, KeyVisitDay, today + 1);
		ModHelper.SetModData(ModId, KeyVisitService, service);
		if (service == ServiceIntro) ModHelper.SetModDataInt(ModId, KeyIntroVisit, 1);
		StorePhoneClient phone = PhoneClient();
		if (phone != null)
		{
			phone.cooldownDuration = 3;
			phone.UseService();
		}
		ModCtx.Log($"治安部线人预约: {service} / 第 {today + 1} 天");
		Toast.Show($"已约好：治安部线人明天过午上门（{ServiceName(service)}）。", "治安部线人");
	}

	/// <summary>每天醒来：同步电话本（冷却递减由原版 StorePhoneClient.OnEndDay 处理）。</summary>
	public static void OnWakeUp()
	{
		try
		{
			EnsurePhoneClient();
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("治安部线人每日刷新失败: " + ex.Message);
		}
	}

	public static void OnShutterOpened()
	{
		try
		{
			EnsurePhoneClient();
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("治安部线人开门刷新失败: " + ex.Message);
		}
	}

	private static bool IntroVisitAlreadyTriggered()
	{
		if (IntroDone || ModHelper.GetModDataInt(ModId, KeyIntroVisit, 0) > 0) return true;
		// 接续旧档：已有初次预约或已经排队的线人，也视为触发过。
		if (HasLiaisonClient())
		{
			ModHelper.SetModDataInt(ModId, KeyIntroVisit, 2);
			return true;
		}
		if (ModHelper.GetModData(ModId, KeyVisitService, "") == ServiceIntro)
		{
			ModHelper.SetModDataInt(ModId, KeyIntroVisit, 1);
			return true;
		}
		return false;
	}

	private static void ClearVisit()
	{
		ModHelper.SetModDataInt(ModId, KeyVisitDay, -1);
		ModHelper.SetModData(ModId, KeyVisitService, "");
	}

	/// <summary>第一次卖出苗床：安排一次来访，立即持久标记，避免等待或排队期间重复触发。</summary>
	public static void OnMaidenSold(GameItem item)
	{
		try
		{
			if (IntroVisitAlreadyTriggered())
			{
				return;
			}
			int today = Today();
			ModHelper.SetModDataInt(ModId, KeyVisitDay, today + 1);
			ModHelper.SetModData(ModId, KeyVisitService, ServiceIntro);
			ModHelper.SetModDataInt(ModId, KeyIntroVisit, 1);
			Toast.Show("街角有人看了你很久。他记住你的店了。", "治安部");
			ModCtx.Log("治安部线人：首次卖出苗床，明天上门送名片");
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("治安部线人首次接触触发失败: " + ex.Message);
		}
	}

	/// <summary>开门生成顾客后：如果今天有预约，把线人插进队里。</summary>
	public static void OnGenerateCustomerVeryLate(StoreClientManager manager)
	{
		try
		{
			EnsurePhoneClient();
			int visitDay = ModHelper.GetModDataInt(ModId, KeyVisitDay, -1);
			if (visitDay < 0 || visitDay != Today())
			{
				return;
			}
			string service = ModHelper.GetModData(ModId, KeyVisitService, "");
			if (string.IsNullOrEmpty(service))
			{
				return;
			}
			bool introVisit = service == ServiceIntro;
			if (introVisit && (IntroDone || ModHelper.GetModDataInt(ModId, KeyIntroVisit, 0) >= 2))
			{
				ClearVisit();
				return;
			}
			if (HasLiaisonClient())
			{
				if (introVisit)
				{
					ModHelper.SetModDataInt(ModId, KeyIntroVisit, 2);
					ClearVisit();
					ModCtx.Log("治安部线人初次来访已在队列中，取消多余初次预约");
					return;
				}
				ModHelper.SetModDataInt(ModId, KeyVisitDay, Today() + 1);
				ModCtx.Log("治安部线人已在店里，预约顺延到明天，避免重复上门");
				return;
			}
			StoreClient client = CreateVisitClient(service);
			if (client == null)
			{
				return;
			}
			int previousIntroState = ModHelper.GetModDataInt(ModId, KeyIntroVisit, 0);
			if (introVisit) ModHelper.SetModDataInt(ModId, KeyIntroVisit, 2);
			try
			{
				PlayerStore.Instance.storeClientManager.AddClient(client);
			}
			catch
			{
				if (introVisit) ModHelper.SetModDataInt(ModId, KeyIntroVisit, previousIntroState);
				throw;
			}
			ClearVisit();
			ModCtx.Log("治安部线人已到店: " + service);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("治安部线人到店失败: " + ex.Message);
		}
	}

	/// <summary>场上或队列里是否已经有一个线人（防止重复预约导致多个线人上门）。</summary>
	private static bool HasLiaisonClient()
	{
		try
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance == null)
			{
				return false;
			}
			if (instance.currentClientInstance != null && IsLiaisonId(instance.currentClientInstance.GetClientBlueprint().identifier))
			{
				return true;
			}
			if (instance.storeClientManager != null && instance.storeClientManager.clientStack != null)
			{
				foreach (StoreClient client in instance.storeClientManager.clientStack)
				{
					if (client != null && IsLiaisonId(client.identifier))
					{
						return true;
					}
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool IsLiaisonId(string identifier)
	{
		return !string.IsNullOrEmpty(identifier) && identifier.StartsWith("lewd_liaison_", StringComparison.Ordinal);
	}

	private static void BuildVisitDialog(StoreClient client, string service)
	{
		string name = client.displayName;
		Dialogue main = client.mainDialogue;
		if (service == ServiceIntro)
		{
			Dialogue intro = main.SetText(name, "老板，打扰。片区协管，例行检查……呵，别紧张。不是例行。");
			intro = intro.NextDialogue().SetText(name, "昨天你送走的那位，我的人看见了。装得挺仔细，就是袋子太新。");
			intro = intro.NextDialogue().SetText(name, "我不是来抓你的。抓你，我一分钱拿不到；跟你做，我能拿到不少。");
			intro = intro.NextDialogue().SetText(name, "这张名片，收好。上面那个号码，只给我一个人用。");
			intro.SetEndAction(delegate
			{
				CompleteIntro();
				PlayerStore.Instance.DismissCurrentClient(false);
			});
			return;
		}
		Dialogue node;
		if (service == ServiceArchive)
		{
			ArchiveCost(ArchiveTier(TotalCrime()), out int richMilk, out int revMaidens, out int bmMaidens);
			node = main.SetText(name, "东西呢？摆开。");
			node = node.NextDialogue().SetText(name, "……嗯。你这案子，卷宗薄得很，一撕就碎。");
			node = node.NextDialogue().SetText(name, "我要的是：" + ArchiveCostText(richMilk, revMaidens, bmMaidens) + "。");
		}
		else if (service == ServiceStall)
		{
			node = main.SetText(name, "两件。一件都不能少。");
			node = node.NextDialogue().SetText(name, "这个瘦的，写'已遣返'；那个，写'移交上级'。");
			node = node.NextDialogue().SetText(name, "一个革命军的，一个黑市的，调教值都得至少两百。要活的，死的写不进结案报告。");
		}
		else
		{
			node = BuildDeliveryLines(main, name, Stage);
			node = node.NextDialogue().SetText(name, "我要的是：" + NeedSummary(Stage) + "。");
		}
		node = node.NextDialogue().SetText(name, "都摆好了？");
		Dialogue result = new Dialogue();
		PlayerLine(result, "都摆好了。");
		Dialogue later = new Dialogue();
		PlayerLine(later, "再等等。");
		node.AddChoice(DirectoryMaster.Item("$"), "都摆好了", "", result, delegate
		{
			string reply = Deliver(service);
			Dialogue say = result.NextDialogue().SetText(name, reply);
			say.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(false);
			});
		}).AddChoice(DirectoryMaster.Item("$"), "再等等", "", later, delegate
		{
			AddReputation(-5);
			ClearCooldown();
			ModCtx.Log("治安部线人：玩家未备货，声望 -5，冷却清空，说完就走");
			Dialogue say2 = later.NextDialogue().SetText(name, "我等不了。下次别约我。\n（治安部声望 -5）");
			say2.SetEndAction(delegate
			{
				PlayerStore.Instance.DismissCurrentClient(false);
			});
		});
	}

	private static Dialogue BuildDeliveryLines(Dialogue main, string name, int stage)
	{
		Dialogue node;
		switch (stage)
		{
		case 2:
			node = main.SetText(name, "让我看看她。");
			node = node.NextDialogue().SetText(name, "……嗯，确实是没动过的。下层区出来的，身上还带着那股铁锈味。");
			node = node.NextDialogue().SetText(name, "这种货最好用。干干净净，没人会追问她去了哪儿。");
			return node;
		case 3:
			node = main.SetText(name, "三十份触手精液。摆开，我一样一样数。");
			node = node.NextDialogue().SetText(name, "……上面那帮人就好这口。别问，问就是'调理身体'。");
			return node;
		case 4:
			node = main.SetText(name, "就是她？");
			node = node.NextDialogue().SetText(name, "……啧啧。你看她的眼睛，跟她妈一模一样。你们这行，真能传代啊。");
			return node;
		default:
			node = main.SetText(name, "东西呢？摆出来，我看看。");
			node = node.NextDialogue().SetText(name, "五份触手精液，一滴都不能少。你知道这东西对女人意味着什么吧？");
			node = node.NextDialogue().SetText(name, "上面那几位太太，喝这个比喝什么都来劲。");
			return node;
		}
	}

	private static void CompleteIntro()
	{
		ModHelper.SetModDataInt(ModId, KeyIntro, 1);
		if (Stage < 1)
		{
			ModHelper.SetModDataInt(ModId, KeyStage, 1);
		}
		EnsurePhoneClient();
		try
		{
			GameItem card = ModHelper.SpawnItem(CardItemId);
			if (card != null)
			{
				ModHelper.SetItemOwnerShip(card, true);
				if (NativeApi.DeliverItem(card, "治安部线人名片"))
					ModCtx.Log("治安部线人名片已下发");
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("名片下发失败: " + ex.Message);
		}
		Toast.Show("电话本里多了一个号码，名字是「治安部线人」。", "治安部线人");
	}

	private static List<GameItem> CounterItems()
	{
		try
		{
			return EmporiumEntry.Instance?.GetCountersItems() ?? new List<GameItem>();
		}
		catch
		{
			return new List<GameItem>();
		}
	}

	private static bool IsSemen(GameItem item)
	{
		return item != null && item.IsID("tentacle_semen");
	}

	private static bool IsOvulation(GameItem item)
	{
		return item != null && item.IsID("ovulation_shot");
	}

	private static bool IsMaiden(GameItem item)
	{
		return item != null && item.IsTag("MAIDEN_FEMALE");
	}

	private static string FactionOf(GameItem item)
	{
		return ItemFactory.GetStr(item, "MAIDEN_FACTION");
	}
	private static void ClearCooldown()
	{
		ModHelper.SetModDataInt(ModId, KeyCooldown, 0);
		try
		{
			StorePhoneClient client = PhoneClient();
			if (client != null)
			{
				client.currentCooldown = 0;
				if (client.phoneState == StorePhoneClient.PhoneState.Cooldown)
				{
					client.phoneState = StorePhoneClient.PhoneState.Regular;
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("清除治安部冷却失败: " + ex.Message);
		}
	}

	private static string Fail(string message, int penalty)
	{
		AddReputation(-penalty);
		ClearCooldown();
		return message + $"\n（治安部声望 -{penalty}）";
	}

	/// <summary>验收柜台，成功则收走并结算，返回他说的话。</summary>
	private static string Deliver(string service)
	{
		try
		{
			List<GameItem> counters = CounterItems();
			if (counters.Count == 0)
			{
				return Fail("……你连摆都懒得摆。你是觉得我很好说话？", 5);
			}
			foreach (GameItem item in counters)
			{
				if (IsSecurityMaiden(item))
				{
					return Fail("……你把她摆到我面前？你是在羞辱我，还是在羞辱你自己？", 15);
				}
			}
			switch (service)
			{
			case ServiceArchive:
				return DeliverArchive(counters);
			case ServiceStall:
				return DeliverStall(counters);
			default:
				return DeliverStage(counters);
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("治安部验收异常: " + ex.Message);
			return "……（他皱了皱眉，什么也没说就走了）";
		}
	}

	private static string DeliverStage(List<GameItem> counters)
	{
		int stage = Stage;
		List<GameItem> take = new List<GameItem>();
		if (stage == 1)
		{
			foreach (GameItem item2 in counters)
			{
				if (IsSemen(item2))
				{
					take.Add(item2);
					if (take.Count >= 5)
					{
						break;
					}
				}
			}
			if (take.Count < 5)
			{
				return Fail($"五份都凑不齐？你是在打发我，还是在试探我？（{take.Count}/5）", 5);
			}
		}
		else if (stage == 2)
		{
			foreach (GameItem item3 in counters)
			{
				if (IsMaiden(item3) && item3.IsTag("MAIDEN_LOWER") && item3.IsTag("MAIDEN_VIRGIN") && item3.IsTag("MAIDEN_ANAL_VIRGIN"))
				{
					take.Add(item3);
					break;
				}
			}
			if (take.Count == 0)
			{
				return Fail("我说的是双处女。你耳朵有问题，还是脑子有问题？", 5);
			}
		}
		else if (stage == 3)
		{
			int semen = 0;
			foreach (GameItem item4 in counters)
			{
				if (IsSemen(item4) && semen < 30)
				{
					semen++;
					take.Add(item4);
				}
			}
			if (semen < 30)
			{
				return Fail($"三十份触手精液，你给了我 {semen} 份。数不对，我不认。", 5);
			}
		}
		else if (stage == 4)
		{
			foreach (GameItem item5 in counters)
			{
				if (IsMaiden(item5) && NativeApi.IsDaughter(item5) && ItemFactory.GetInt(item5, "TRAIN_VALUE") > 200)
				{
					take.Add(item5);
					break;
				}
			}
			if (take.Count == 0)
			{
				return Fail("我要的是调教过两百的女儿。你拿这么个东西来，是想羞辱我？", 5);
			}
		}
		else
		{
			return "……你还有什么事？";
		}
		TakeItems(take);
		// 常规剧情线交货成功：清掉冷却，方便继续推进下一段
		ClearCooldown();
		return CompleteStage(stage);
	}

	private static string DeliverArchive(List<GameItem> counters)
	{
		int tier = ArchiveTier(TotalCrime());
		ArchiveCost(tier, out int needRichMilk, out int needRevMaidens, out int needBmMaidens);
		List<GameItem> take = new List<GameItem>();
		int richMilk = 0;
		int revMaidens = 0;
		int bmMaidens = 0;
		foreach (GameItem item in counters)
		{
			if (IsMaiden(item) && revMaidens < needRevMaidens && FactionOf(item) == "革命军")
			{
				revMaidens++;
				take.Add(item);
			}
			else if (IsMaiden(item) && bmMaidens < needBmMaidens && FactionOf(item) == "黑市")
			{
				bmMaidens++;
				take.Add(item);
			}
			else if (item.identifier == "milk_rich" && richMilk < needRichMilk)
			{
				richMilk++;
				take.Add(item);
			}
		}
		if (richMilk < needRichMilk || revMaidens < needRevMaidens || bmMaidens < needBmMaidens)
		{
			return Fail($"数不对。你当我这儿是做慈善的？（浓缩乳汁 {richMilk}/{needRichMilk}，革命军苗床 {revMaidens}/{needRevMaidens}，黑市苗床 {bmMaidens}/{needBmMaidens}）", 5);
		}
		TakeItems(take);
		int cut = ApplyArchive();
		return $"一半。这次抹掉了 {cut} 点。\n剩下的，你自己收敛点。我不是你的保姆。";
	}

	private static string DeliverStall(List<GameItem> counters)
	{
		List<GameItem> take = new List<GameItem>();
		int rev = 0;
		int bm = 0;
		foreach (GameItem item in counters)
		{
			if (!IsMaiden(item) || ItemFactory.GetInt(item, "TRAIN_VALUE") < 200)
			{
				continue;
			}
			string faction = FactionOf(item);
			if (faction == "革命军" && rev < 1)
			{
				rev++;
				take.Add(item);
			}
			else if (faction == "黑市" && bm < 1)
			{
				bm++;
				take.Add(item);
			}
		}
		if (rev < 1 || bm < 1)
		{
			return Fail($"我要革命军和黑市各一件，调教值都得至少两百。合格的革命军 {rev}/1，黑市 {bm}/1。你拿这些东西敷衍我？", 5);
		}
		TakeItems(take);
		int cut = ApplyStall();
		return $"调查进度我给你摁回去 {cut} 点。\n剩下的，看你自己的运气。";
	}

	private static string CompleteStage(int stage)
	{
		ModHelper.SetModDataInt(ModId, KeyStage, Math.Min(5, stage + 1));
		switch (stage)
		{
		case 1:
		{
			int cut = ApplyCrimeReduction(500, half: false);
			return $"不错，是懂规矩的。\n这五百的案值，我替你抹了 {cut} 点。别问怎么抹的，问就是归档错误。";
		}
		case 2:
			RegisterOvulation();
			return "很好。你比我想的识相。\n那管东西我替你登记了——从今天起它算管制物品，进冊子，不吃案值。";
		case 3:
			RegisterSemen();
			return "从今天起，这东西上冊了。\n它会按管制物资算——不过放心，我罩着，不进你的案值。";
		case 4:
			RegisterMaiden();
			return "成了。从今天起，你所有的苗床都上冊。\n价格翻上去，案值不记你的。没人会来查你——除了我。";
		default:
			return "……行。";
		}
	}

	/// <summary>在所有罪名上按额度扣减案值；half=true 时每个罪名最多只扣一半。</summary>
	public static int ApplyCrimeReduction(int budget, bool half)
	{
		try
		{
			SecData sec = PlayerStore.Instance?.secData;
			if (sec == null || sec.crimeAmount == null || budget <= 0)
			{
				return 0;
			}
			int before = sec.GetTotalCrimeValue();
			int remaining = budget;
			for (int i = 0; i < sec.crimeAmount.Count && remaining > 0; i++)
			{
				if (sec.crimeAmount[i] > 0)
				{
					int cap = half ? (sec.crimeAmount[i] / 2) : sec.crimeAmount[i];
					int cut = Math.Min(remaining, cap);
					sec.crimeAmount[i] -= cut;
					remaining -= cut;
				}
			}
			int done = budget - remaining;
			ModCtx.Log($"治安部扣减案值: {before} -> {sec.GetTotalCrimeValue()}（本次 -{done}）");
			return done;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("扣减案值失败: " + ex.Message);
			return 0;
		}
	}

	/// <summary>所有犯罪案值减半，单次最多扣 1500。返回实际扣减量。</summary>
	public static int ApplyArchive()
	{
		try
		{
			SecData sec = PlayerStore.Instance?.secData;
			if (sec == null)
			{
				return 0;
			}
			int budget = Math.Min(1500, sec.GetTotalCrimeValue() / 2);
			int done = ApplyCrimeReduction(budget, half: true);
			ModCtx.Log($"治安部归档: 本次扣 {done}");
			return done;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("归档失败: " + ex.Message);
			return 0;
		}
	}

	/// <summary>调查进度 -30（最低 0）。返回实际扣减量。</summary>
	public static int ApplyStall()
	{
		try
		{
			SecData sec = PlayerStore.Instance?.secData;
			if (sec == null)
			{
				return 0;
			}
			int before = sec.evidenceLevel;
			sec.evidenceLevel = Math.Max(0, before - 30);
			int done = before - sec.evidenceLevel;
			ModCtx.Log($"治安部结案: 调查进度 {before} -> {sec.evidenceLevel}（压 {done}）");
			return done;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("结案失败: " + ex.Message);
			return 0;
		}
	}

	private static bool IsSecurityMaiden(GameItem item)
	{
		return IsMaiden(item) && FactionOf(item) == "治安部";
	}

	/// <summary>收走柜台上已经验过的货物。</summary>
	private static void TakeItems(List<GameItem> items)
	{
		foreach (GameItem item in items)
		{
			try
			{
				GeneralHelper.SetItemOwned(item, false);
				item.Destroy();
			}
			catch (Exception ex)
			{
				ModCtx.LogWarning("治安部收货失败: " + ex.Message);
			}
		}
	}

	private static StoreClient CreateVisitClient(string service)
	{
		StoreClient client = new StoreClient();
		client.identifier = "lewd_liaison_" + service + "_" + Today();
		client.displayName = "治安部线人";
		client.sourceModId = ModId;
		client.clientFaction = "FACTION_SECURITY";
		client.spriteName = SpriteDict.GetRandomSecSpriteName();
		client.clientIntent = StoreClient.ClientIntent.DIALOGUE;
		// 原生 CanEndDay 同时检查排队和当前客人的 dismissable。
		// 送名片与主线交货属于重要剧情来访；终章后的重复服务仍可提前关店。
		client.dismissable = service != ServiceIntro && service != ServiceDeliver;
		client.AddTag(ClientTag);
		BuildVisitDialog(client, service);
		client.CompleteClientCreation();
		return client;
	}
	// ───────────────────────── 管制品登记 ─────────────────────────

	/// <summary>统一登记等级；不依赖新物品是否已经补过违禁标签，保留原生管制加价。</summary>
	public static void ApplyRegistered(GameItem item)
	{
		try
		{
			if (item == null)
			{
				return;
			}
			int level = IsOvulation(item) ? 1
				: IsSemen(item) || item.identifier == ItemFactory.BarrelSemenId ? 2
				: IsMaiden(item) || item.identifier == "maiden_lower" ? 3
				: ContrabandHelper.GetContrabandLevel(item);
			if (level < 1 || level > 3) return;
			if (!item.IsGameItemType("RESTRICTED_" + level))
			{
				// SetRestrictedClass 按违禁等级转换；先补齐等级再调用游戏实现。
				for (int i = 1; i <= 3; i++) item.RemoveGameItemType("RESTRICTED_" + i);
				ContrabandHelper.InitContrabandItem(item, level);
				PermitHelper.SetRestrictedClass(item);
			}
			if (item.IsTag("CONTRABAND_ITEM_TAG") || item.IsGameItemType("CONTRABAND"))
				ContrabandHelper.RemoveContrabandStatus(item);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("管制登记失败: " + ex.Message);
		}
	}

	/// <summary>排卵针、瓶/桶装精液和苗床按当前剧情登记状态处理。</summary>
	public static void ApplyRegisteredIfNeeded(GameItem item)
	{
		if (item == null)
		{
			return;
		}
		try
		{
			if (IsRegisteredCategory(item))
			{
				ApplyRegistered(item);
			}
		}
		catch
		{
		}
	}

	private static bool IsRegisteredCategory(GameItem item) => item != null &&
		((SemenRegistered && (IsSemen(item) || item.identifier == ItemFactory.BarrelSemenId))
		|| (MaidenRegistered && (IsMaiden(item) || item.identifier == "maiden_lower"))
		|| (OvulationRegistered && IsOvulation(item)));

	/// <summary>把场上已有的对应物品按已解锁奖励批量登记。</summary>
	public static void SweepRegistered()
	{
		try
		{
			EmporiumEntry entry = EmporiumEntry.Instance;
			if (entry == null)
			{
				return;
			}
			int num = 0;
			foreach (GameItem allOwnedItem in entry.GetAllItems())
			{
				if (IsRegisteredCategory(allOwnedItem))
				{
					ApplyRegistered(allOwnedItem);
					num++;
				}
			}
			ModCtx.Log("管制登记刷新: " + num + " 件");
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("管制登记刷新失败: " + ex.Message);
		}
	}

	public static void RegisterSemen()
	{
		ModHelper.SetModDataInt(ModId, KeyRegSemen, 1);
		SweepRegistered();
	}

	public static void RegisterMaiden()
	{
		ModHelper.SetModDataInt(ModId, KeyRegMaiden, 1);
		SweepRegistered();
	}

	public static void RegisterOvulation()
	{
		ModHelper.SetModDataInt(ModId, KeyRegOvulation, 1);
		SweepRegistered();
	}

	/// <summary>用「归档」额度抵扣一次贩卖人口案值，返回剩余需要记入的案值。</summary>
	public static int ConsumeCrimeCredit(int amount)
	{
		try
		{
			int credit = CrimeCredit;
			if (credit <= 0 || amount <= 0)
			{
				return amount;
			}
			int used = Math.Min(credit, amount);
			ModHelper.SetModDataInt(ModId, KeyCredit, credit - used);
			int rest = amount - used;
			Toast.Show($"治安部替你抹掉 {used} 案值（剩余额度 {credit - used}）。", "归档");
			ModCtx.Log($"案值抵扣: -{used}（剩余额度 {credit - used}）");
			return rest;
		}
		catch
		{
			return amount;
		}
	}

	// ───────────────────────── 名片 ─────────────────────────

	public static void OpenCardWindow()
	{
		try
		{
			if ((UnityEngine.Object)(object)CustomUIManager.Instance == (UnityEngine.Object)null)
			{
				return;
			}
			Sprite sprite = ModTextureSprite("phone_number");
			CustomUIManager.Instance.CloseWindow(CardWindowId);
			float num = 440f;
			float num2 = 260f;
			if ((UnityEngine.Object)(object)sprite != (UnityEngine.Object)null)
			{
				Vector2 size = sprite.rect.size;
				float num3 = Mathf.Min(560f / Mathf.Max(1f, size.x), 640f / Mathf.Max(1f, size.y));
				num = Mathf.Round(size.x * num3);
				num2 = Mathf.Round(size.y * num3);
			}
			// 保留原生标题栏及其关闭按钮，并为标题栏预留高度，避免压缩名片图像。
			CustomUIBuilder builder = CustomUIManager.Window(CardWindowId, "治安部线人名片", "overlay").SetSize(num, num2 + 40f).Center()
				.SetDraggable(true)
				.SetCloseOnEscape(true);
			if ((UnityEngine.Object)(object)sprite != (UnityEngine.Object)null)
			{
				builder.AddImage(sprite, num - 16f, num2 - 16f, (string)null);
			}
			else
			{
				ModCtx.LogWarning("名片图像缺失：phone_number.png 未能在 mod 资源里找到");
			}
			builder.Show();
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("名片打开失败: " + ex.Message);
		}
	}

	private static Sprite ModTextureSprite(string key)
	{
		try
		{
			LoadedMod mod = SpriteInjector.MyMod();
			if (mod == null)
			{
				return null;
			}
			if (mod.Sprites != null && mod.Sprites.TryGetValue(key, out Sprite value) && (UnityEngine.Object)(object)value != (UnityEngine.Object)null)
			{
				return value;
			}
			if (mod.PaperSprites != null && mod.PaperSprites.TryGetValue(key, out Sprite value2) && (UnityEngine.Object)(object)value2 != (UnityEngine.Object)null)
			{
				return value2;
			}
			if (mod.Textures != null)
			{
				string[] array = new string[4] { "Items/" + key, key, "Paper/" + key, key + ".png" };
				foreach (string text in array)
				{
					if (mod.Textures.TryGetValue(text, out Texture2D tex) && (UnityEngine.Object)(object)tex != (UnityEngine.Object)null)
					{
						return SpriteInjector.MakeSprite(tex, SpriteInjector.BasePpu());
					}
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("读取名片贴图失败: " + ex.Message);
		}
		return null;
	}

	// ───────────────────────── 调试与补丁 ─────────────────────────

	public static void DebugSchedule(string service)
	{
		ModHelper.SetModDataInt(ModId, KeyVisitDay, Today());
		ModHelper.SetModData(ModId, KeyVisitService, service);
		Toast.Show("今天就会有人上门：" + ServiceName(service), "治安部线人");
	}

	public static void DebugUnlock()
	{
		ModHelper.SetModDataInt(ModId, KeyIntro, 1);
		ModHelper.SetModDataInt(ModId, KeyStage, Math.Max(1, Stage));
		EnsurePhoneClient();
		Toast.Show("治安部专线已解锁：26535", "治安部线人");
	}

	/// <summary>仅为本模组线人的无 locID 联系人使用 displayName。</summary>
	[HarmonyPatch(typeof(StorePhoneClient), "GetLocalizedDisplayName")]
	private static class PatchPhoneName
	{
		private static void Postfix(StorePhoneClient __instance, ref string __result)
		{
			try
			{
				if (__instance != null && __instance.dialogFuncId == PhoneDialogId
					&& string.IsNullOrEmpty(__instance.locID) && !string.IsNullOrEmpty(__instance.displayName))
				{
					__result = __instance.displayName;
				}
			}
			catch
			{
			}
		}
	}

	[HarmonyPatch(typeof(StorePhoneClient), "InitPhoneClientDict")]
	private static class PatchPhoneDict
	{
		private static void Postfix(ref Dictionary<long, StorePhoneClient> __result)
		{
			try
			{
				if (__result == null || __result.ContainsKey(Number))
				{
					return;
				}
				__result[Number] = new StorePhoneClient
				{
					displayName = "治安部线人",
					dialogFuncId = PhoneDialogId,
					phoneState = StorePhoneClient.PhoneState.NotResponding,
					dialedBefore = false,
					cooldownDuration = 0
				};
			}
			catch (Exception ex)
			{
				ModCtx.LogWarning("电话本注入失败: " + ex.Message);
			}
		}
	}

}


