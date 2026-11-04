using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace XIAOWONightAuction;

internal static class AuctionPhone
{
	internal const string CardId = "xiaowo_auction_phone_card";

	internal const string ContactId = "xiaowo_night_auction_phone";

	internal const string IntroSource = "xiaowo_night_auction_phone_intro";

	internal const long Number = 4367L;

	internal const string Atlas = "XIAOWO/NightAuctionPhone";

	private static int _tick;

	private static bool _storeWorkPending;

	private static int _storeWorkFailures;

	private static bool _warned;

	private static PaperUI _paper;

	internal static string ContactName => AuctionText.Select("夜间拍卖行", "Night Auction House");

	internal static bool CardGiven => AuctionState.Read("phone_card_given") == "1";

	internal static int ReadyDay
	{
		get
		{
			if (!int.TryParse(AuctionState.Read("phone_ready_day"), out var result))
			{
				return 0;
			}
			return result;
		}
	}

	internal static bool IsVisitDay(int day)
	{
		if (!(AuctionState.Read("phone_visit_day") == day.ToString()))
		{
			if (TryPending(out var dueDay, out var _))
			{
				return AuctionPhoneRules.VisitDue(day, dueDay);
			}
			return false;
		}
		return true;
	}

	private static string VisitSource(int dueDay)
	{
		return "xiaowo_night_auction_phone_visit_" + dueDay;
	}

	internal static void Tick()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!_storeWorkPending || _tick-- > 0)
		{
			return;
		}
		_tick = 60;
		Scene activeScene = SceneManager.GetActiveScene();
		if (((Scene)(ref activeScene)).name != "InventoryScene" || (Object)(object)EmporiumEntry.Instance == (Object)null)
		{
			return;
		}
		try
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance?.storeClientManager == null)
			{
				return;
			}
			if (!AuctionPhoneRules.Eligible(AuctionCatalog.Day))
			{
				_storeWorkPending = false;
				return;
			}
			if (!CardGiven && (AuctionCatalog.Day > 8 || AuctionState.Read("phone_card_pending") == "1"))
			{
				DeliverCard();
			}
			if (CardGiven)
			{
				EnsureContact();
			}
			HandleVisitors(instance.storeClientManager);
			_warned = false;
			_storeWorkPending = false;
		}
		catch (Exception ex)
		{
			_storeWorkFailures++;
			if (!_warned)
			{
				Core.Log?.Warning("[拍卖电话] 待店铺初始化后重试: " + ex.Message);
			}
			_warned = true;
			if (_storeWorkFailures >= 3)
			{
				_storeWorkPending = false;
				Core.Log?.Error("[拍卖电话] 本次店铺事件重试已达上限；下次读档、醒来或回店时重新检查。" + ex);
			}
		}
	}

	internal static void OnStoreReady()
	{
		_storeWorkPending = true;
		_storeWorkFailures = 0;
		_warned = false;
		_tick = 0;
		Tick();
	}

	internal static GameItem CreateCard(bool owned)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		GameItem obj = DirectoryMaster.Item("card_pharma", owned) ?? throw new InvalidOperationException("Native business card is unavailable.");
		obj.identifier = "xiaowo_auction_phone_card";
		OfficialAssets.SetSprite(obj, "auction_card_closed");
		obj.shape = new GridShapeBuilder(2, 1).SetDataFill(2, 1, (byte)1).Build();
		obj.modifiedShape = obj.shape.Clone();
		LocalizeCard(obj);
		((GameItemFunc)obj).onLoaded = MonoCallbacks.Bind<Action<GameItem>>(new Action<GameItem>(LocalizeCard));
		obj.ValidateShapeState();
		return obj;
	}

	private static void LocalizeCard(GameItem item)
	{
		string identifierName = (item.name = AuctionText.Select("夜间拍卖行名片", "Night Auction Business Card"));
		item.identifierName = identifierName;
		identifierName = (item.longDescription = AuctionText.Select("电话：4367。四类收货商仅接受电话预约，次日到店。通常第3天可再预约；预约核心伙伴时缩短为第2天。赠送对应类别的礼物可提升合作关系。", "Phone: 4367. Four specialist buyers visit only by appointment, arriving tomorrow. Book again after 3 days, or 2 days when booking a Trusted Partner. Give matching gifts to improve relations."));
		item.shortDescription = identifierName;
		item.flavorText = AuctionText.Select("有货要出，拨这个号码。", "Have goods to sell? Call this number.");
	}

	internal static void RefreshOwnedText()
	{
		PlayerStore instance = PlayerStore.Instance;
		List<GameItem> list = ((instance != null) ? instance.FindAllItem(true) : null);
		if (list == null)
		{
			return;
		}
		foreach (GameItem item in list)
		{
			if (((item != null) ? item.identifier : null) == "xiaowo_auction_phone_card")
			{
				LocalizeCard(item);
			}
		}
	}

	internal static bool DeliverCard()
	{
		if (CardGiven)
		{
			return true;
		}
		PlayerStore instance = PlayerStore.Instance;
		List<GameItem> list = ((instance != null) ? instance.FindAllItem(true) : null);
		if (list == null || (Object)(object)EmporiumEntry.Instance == (Object)null)
		{
			return false;
		}
		foreach (GameItem item in list)
		{
			if (((item != null) ? item.identifier : null) == "xiaowo_auction_phone_card" && item.parentInventory != null)
			{
				FinishDelivery();
				return true;
			}
		}
		AuctionState.Write("phone_card_pending", "1");
		GameItem val = CreateCard(owned: true);
		try
		{
			EmporiumEntry.Instance.TryAddToPlayerInv(val);
			if (val.parentInventory == null && (val.parents == null || val.parents.Count == 0))
			{
				GeneralHelper.SetItemOwned(val, false);
				return false;
			}
			FinishDelivery();
			Core.Log?.Msg("[拍卖电话] 名片已发放，号码4367，占格2x1。");
			return true;
		}
		catch
		{
			if (val.parentInventory == null)
			{
				GeneralHelper.SetItemOwned(val, false);
			}
			throw;
		}
	}

	private static void FinishDelivery()
	{
		AuctionState.Write("phone_card_given", "1");
		AuctionState.Delete("phone_card_pending");
		EnsureContact();
	}

	internal static StorePhoneClient EnsureContact()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		PlayerStore instance = PlayerStore.Instance;
		if (instance == null || !CardGiven)
		{
			return null;
		}
		PlayerStore val = instance;
		if (val.PhoneClientDict == null)
		{
			val.PhoneClientDict = new Dictionary<long, StorePhoneClient>();
		}
		if (!instance.PhoneClientDict.TryGetValue(4367L, out var value))
		{
			value = new StorePhoneClient
			{
				dialogFuncId = "xiaowo_night_auction_phone",
				locID = "xiaowo_night_auction_phone"
			};
			instance.PhoneClientDict.Add(4367L, value);
		}
		if (value.dialogFuncId != "xiaowo_night_auction_phone")
		{
			throw new InvalidOperationException("电话号码4367已被其他联系人占用。");
		}
		value.displayName = ContactName;
		value.locID = "xiaowo_night_auction_phone";
		value.phoneClientType = (PhoneClientType)0;
		value.cooldownDuration = ((int.TryParse(AuctionState.Read("phone_call_interval"), out var result) && result == 2) ? 2 : 3);
		value.currentCooldown = Math.Max(0, ReadyDay - AuctionCatalog.Day);
		value.phoneState = (PhoneState)((value.currentCooldown > 0) ? 5 : 4);
		return value;
	}

	internal static bool TryPending(out int dueDay, out ContainerCategory category)
	{
		dueDay = 0;
		category = ContainerCategory.Medical;
		string[] array = AuctionState.Read("phone_appointment").Split(',');
		if (array.Length != 2 || !int.TryParse(array[0], out dueDay) || !int.TryParse(array[1], out var result) || result < 0 || result > 3)
		{
			return false;
		}
		category = (ContainerCategory)result;
		return dueDay > 0;
	}

	internal static bool Reserve(ContainerCategory category, out string message)
	{
		int day = AuctionCatalog.Day;
		if (!CardGiven || category < ContainerCategory.Medical || category > ContainerCategory.Engineering || !AuctionPhoneRules.CanReserve(day, ReadyDay, TryPending(out var dueDay, out var _)))
		{
			message = UnavailableText();
			return false;
		}
		string text = (day + 1).ToString();
		dueDay = (int)category;
		AuctionState.Write("phone_appointment", text + "," + dueDay);
		AuctionState.Write("phone_call_interval", AuctionMerchantRules.CallInterval(AuctionMerchantRelations.Gifts(category)).ToString());
		AuctionState.Write("phone_ready_day", AuctionPhoneRules.ReadyDay(day, AuctionMerchantRelations.Gifts(category)).ToString());
		EnsureContact();
		message = AuctionMerchants.DisplayName(category) + AuctionText.Select("明天到店，预算 ", " will visit tomorrow. Budget: ") + AuctionMerchantRelations.Budget(category).ToString("N0") + AuctionText.Select("¢。第", "¢. Call again on day ") + ReadyDay + AuctionText.Select("天恢复预约。", ".");
		Core.Log?.Msg("[拍卖电话] 预约 " + category.ToString() + "，到访日=" + (day + 1) + "，下次预约日=" + ReadyDay + ".");
		return true;
	}

	private static string UnavailableText()
	{
		if (TryPending(out var dueDay, out var category))
		{
			return AuctionMerchants.DisplayName(category) + AuctionText.Select("已预约，第", " is booked for day ") + dueDay + AuctionText.Select("天到店。", ".");
		}
		return AuctionText.Select("采购团队还在周转，第", "Our buyers are busy. Call again on day ") + ReadyDay + AuctionText.Select("天恢复预约。", ".");
	}

	internal static Dialogue BuildDialogue()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		EnsureContact();
		Dialogue val = new Dialogue().SetText(ContactName, AuctionText.Select("夜间拍卖行服务。请选择：", "Night Auction House services. Please choose:"));
		AddServiceChoice(val, AuctionText.Select("预约收货商人", "Book a Buyer"), BuildBookingDialogue(), null);
		AddServiceChoice(val, AuctionText.Select("采购委托", "Procurement Orders"), AuctionCommissions.DialoguePage(), null);
		Dialogue val2 = new Dialogue().SetText(ContactName, AuctionText.Select("顾问与档案服务。", "Advisors and records."));
		Dialogue val3 = new Dialogue().SetText(ContactName, AuctionText.Select("档案服务。", "Records and collections."));
		AddServiceChoice(val3, AuctionText.Select("藏品收集册", "Collection Book"), Ending(string.Empty), AuctionController.RequestPhoneCollection);
		AddServiceChoice(val3, AuctionText.Select("合作商人档案", "Buyer Relationships"), Ending(string.Empty), AuctionController.RequestPhoneRelationships);
		AddServiceChoice(val3, AuctionText.Select("返回", "Back"), val2, null);
		AddServiceChoice(val2, AuctionText.Select("拍卖行顾问", "Auction Advisors"), AuctionNightMarket.BuildAdvisorDialogue(val2), null);
		AddServiceChoice(val2, AuctionText.Select("藏品 / 商人档案", "Collection / Buyer Records"), val3, null);
		AddServiceChoice(val2, AuctionText.Select("返回", "Back"), val, null);
		AddServiceChoice(val, AuctionText.Select("顾问 / 档案服务", "Advisors / Records"), val2, null);
		return val;
	}

	internal static Dialogue BuildBookingDialogue()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		EnsureContact();
		if (!AuctionPhoneRules.CanReserve(AuctionCatalog.Day, ReadyDay, TryPending(out var _, out var _)))
		{
			return Ending(UnavailableText());
		}
		Dialogue val = new Dialogue().SetText(ContactName, AuctionText.Select("夜间拍卖行收货专线。需要哪一类采购员？确认后明天到店。", "Night Auction buying desk. Which specialist do you need? Confirm a booking for tomorrow."));
		Dialogue val2 = new Dialogue().SetText(ContactName, AuctionText.Select("其他采购类别。", "Other buying categories."));
		AddBooking(val, ContainerCategory.Medical);
		AddBooking(val, ContainerCategory.Luxury);
		AddPhoneChoice(val, AuctionText.Select("违禁品类 / 工程类", "Contraband / Engineering"), val2);
		AddBooking(val2, ContainerCategory.Contraband);
		AddBooking(val2, ContainerCategory.Engineering);
		AddPhoneChoice(val2, AuctionText.Select("返回", "Back"), val);
		return val;
	}

	private static void AddBooking(Dialogue dialogue, ContainerCategory category)
	{
		Dialogue confirmation = Ending(string.Empty);
		Action action = MonoCallbacks.Bind<Action>((Action)delegate
		{
			Reserve(category, out var message);
			confirmation.SetText(ContactName, message);
		});
		AddPhoneChoice(dialogue, CategoryCaption(category) + " · " + AuctionMerchantRelations.TierName(category), confirmation, action);
	}

	internal static string CategoryCaption(ContainerCategory category)
	{
		return category switch
		{
			ContainerCategory.Medical => AuctionText.Select("医疗类", "Medical goods"), 
			ContainerCategory.Luxury => AuctionText.Select("奢侈品类", "Luxury goods"), 
			ContainerCategory.Contraband => AuctionText.Select("违禁品类", "Contraband"), 
			_ => AuctionText.Select("工程类", "Engineering goods"), 
		};
	}

	private static void AddPhoneChoice(Dialogue dialogue, string caption, Dialogue next, Action action = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Dialogue val = new Dialogue().SetText("PLAYER", caption).SetNextDialogue(next);
		dialogue.AddChoice(ItemDirectory.CreateEmptyItem((string)null), caption, "", val, action, (Func<bool>)null);
	}

	internal static void AddServiceChoice(Dialogue dialogue, string caption, Dialogue next, Action action)
	{
		AddPhoneChoice(dialogue, caption, next, (action == null) ? null : MonoCallbacks.Bind<Action>(action));
	}

	internal static Dialogue Ending(string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Dialogue obj = new Dialogue().SetText(ContactName, text);
		obj.SetEndAction(MonoCallbacks.Bind<Action>((Action)delegate
		{
			PhoneUIManager instance = PhoneUIManager.Instance;
			if (instance != null)
			{
				instance.OnPhoneDialogEnd(true);
			}
		}));
		return obj;
	}

	internal static void HandleVisitors(StoreClientManager manager)
	{
		int day = AuctionCatalog.Day;
		if (manager?.clientStack == null || !AuctionPhoneRules.Eligible(day))
		{
			return;
		}
		AuctionMerchants.HandleDailyVisitors(manager);
		if (!CardGiven && day == 8)
		{
			if (!ContainsSource(manager, "xiaowo_night_auction_phone_intro"))
			{
				manager.AddClient(AuctionMerchants.CreateClient(manager, AuctionPhoneRules.Presenter(PlayerStore.Instance.runID), "xiaowo_night_auction_phone_intro"));
			}
			if (ContainsSource(manager, "xiaowo_night_auction_phone_intro"))
			{
				AuctionState.Write("phone_intro_queued", "1");
			}
		}
		if (TryPending(out var dueDay, out var category) && AuctionPhoneRules.VisitDue(day, dueDay))
		{
			string text = VisitSource(dueDay);
			if (AuctionState.Read("phone_visit_delivered") == text)
			{
				AuctionState.Write("phone_visit_day", day.ToString());
				AuctionState.Delete("phone_appointment");
			}
			else
			{
				AuctionState.Write("phone_visit_day", day.ToString());
				PlayerStore instance = PlayerStore.Instance;
				object obj;
				if (instance == null)
				{
					obj = null;
				}
				else
				{
					StoreClientInstance currentClientInstance = instance.currentClientInstance;
					obj = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
				}
				StoreClient val = (StoreClient)obj;
				bool flag = val?.eventSourceId == text;
				if (flag)
				{
					EnsureBookedCategory(val, manager, category, text);
				}
				for (int i = 0; i < manager.clientStack.Count; i++)
				{
					StoreClient val2 = manager.clientStack[i];
					if (!(val2?.eventSourceId != text))
					{
						if (flag)
						{
							manager.clientStack.RemoveAt(i--);
							continue;
						}
						EnsureBookedCategory(val2, manager, category, text);
						flag = true;
					}
				}
				if (!flag)
				{
					manager.AddClient(AuctionMerchants.CreateClient(manager, category, text));
				}
				if (ContainsSource(manager, text))
				{
					AuctionState.Write("phone_visit_queued", text);
				}
			}
		}
		AuctionMerchants.HandleDailyVisitors(manager);
	}

	internal static void OnMerchantArrived(StoreClient client)
	{
		if (client == null || !TryPending(out var dueDay, out var category) || !AuctionPhoneRules.VisitDue(AuctionCatalog.Day, dueDay) || client.eventSourceId != VisitSource(dueDay))
		{
			return;
		}
		EnsureBookedCategory(client, PlayerStore.Instance?.storeClientManager, category, client.eventSourceId);
		if (client.identifier != AuctionMerchants.ClientId(category))
		{
			return;
		}
		AuctionState.Write("phone_visit_delivered", client.eventSourceId);
		AuctionState.Write("phone_visit_day", AuctionCatalog.Day.ToString());
		List<StoreClient> list = PlayerStore.Instance?.storeClientManager?.clientStack;
		if (list != null)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list[num]?.eventSourceId == client.eventSourceId)
				{
					list.RemoveAt(num);
				}
			}
		}
		AuctionState.Delete("phone_appointment");
		Core.Log?.Msg("[拍卖电话] 预约商人已进店: " + client.displayName + ".");
	}

	private static void EnsureBookedCategory(StoreClient client, StoreClientManager manager, ContainerCategory category, string source)
	{
		if (!(client.identifier == AuctionMerchants.ClientId(category)))
		{
			AuctionMerchants.ConfigureClient(client, manager, category, source);
			Core.Log?.Warning("[拍卖电话] 已纠正预约商人的类别: " + category.ToString() + ".");
		}
	}

	private static bool ContainsSource(StoreClientManager manager, string source)
	{
		PlayerStore instance = PlayerStore.Instance;
		object obj;
		if (instance == null)
		{
			obj = null;
		}
		else
		{
			StoreClientInstance currentClientInstance = instance.currentClientInstance;
			obj = ((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.eventSourceId);
		}
		if ((string?)obj == source)
		{
			return true;
		}
		foreach (StoreClient item in manager.clientStack)
		{
			if (item?.eventSourceId == source)
			{
				return true;
			}
		}
		return false;
	}

	internal static Dialogue WrapIntroduction(StoreClient client, Dialogue regular)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (client.eventSourceId != "xiaowo_night_auction_phone_intro" || CardGiven)
		{
			return regular;
		}
		return new Dialogue().SetText(client.displayName, AuctionText.Select("这是夜间拍卖行的名片，号码4367。打电话告诉我们你要出哪一类货，采购员第二天来；每次预约后要间隔两天。", "Here's our Night Auction card: 4367. Call to choose a buying specialist for tomorrow. Leave two days between bookings.")).SetStartAction(MonoCallbacks.Bind<Action>((Action)delegate
		{
			DeliverCard();
		})).SetNextDialogue(regular);
	}

	internal static Sprite Sprite(string name)
	{
		return OfficialAssets.Get(name);
	}

	internal static void OpenCard()
	{
		if (!Core.StoreReady)
		{
			return;
		}
		PaperUI paper = _paper;
		if (paper != null)
		{
			paper.Close();
		}
		_paper = ModHelper.CreatePaper("xiaowo_night_auction", "phone_card", "", 680f, 448f);
		_paper.Data.designWidth = 340f;
		_paper.Data.designHeight = 224f;
		_paper.SetChrome(true, false, false, false).AddPage((string)null, (string)null).AddImage(0f, 0f, 340f, 191f, "xiaowo_night_auction:auction_card_open", "fit", (string)null)
			.AddButton(100f, 196f, 140f, 24f, AuctionText.Select("拨打 4367", "Call 4367"), (Action)delegate
			{
				PaperUI paper2 = _paper;
				if (paper2 != null)
				{
					paper2.Hide();
				}
				EnsureContact();
				PhoneUIManager.Instance.OpenUI();
				PhoneUIManager.Instance.AutoCall(4367L);
			}, (string)null, (string)null);
		_paper.Show();
	}

	internal static void Dispose()
	{
		PaperUI paper = _paper;
		if (paper != null)
		{
			paper.Close();
		}
		_paper = null;
		_storeWorkPending = false;
		_storeWorkFailures = 0;
	}
}
