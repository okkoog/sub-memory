# CorpPresence 0.9.4 localization paths

## CorpPresence/SpacerCo.cs

~~~csharp
0210: 			if (!Settings.MaliceMode)
0211: 			{
0212: 				return L.T("星际客联盟", "Spacer Alliance");
0213: 			}
0214: 			return L.T("商人联合", "Merchant Union");
~~~

~~~csharp
0212: 				return L.T("星际客联盟", "Spacer Alliance");
0213: 			}
0214: 			return L.T("商人联合", "Merchant Union");
0215: 		}
0216: 	}
~~~

~~~csharp
0220: 	public override long PhoneNumber => 77223L;
0221: 
0222: 	public override string[] TierNames => L.T(new string[3] { "联络点", "中转站", "枢纽" }, new string[3] { "Contact Point", "Relay Station", "Hub" });
0223: 
0224: 	public override string CardFlavor => "";
~~~

~~~csharp
0224: 	public override string CardFlavor => "";
0225: 
0226: 	public override string DeskName => L.T("导师", "The Mentor");
0227: 
0228: 	public override string DeskAsk => "";
~~~

~~~csharp
0228: 	public override string DeskAsk => "";
0229: 
0230: 	public override string[] FrontDesk => L.T(new string[1] { "说。" }, new string[1] { "Talk." });
0231: 
0232: 	public override string FrontDeskDone => L.T("明天到。", "Tomorrow.");
~~~

~~~csharp
0230: 	public override string[] FrontDesk => L.T(new string[1] { "说。" }, new string[1] { "Talk." });
0231: 
0232: 	public override string FrontDeskDone => L.T("明天到。", "Tomorrow.");
0233: 
0234: 	public override CatalogEntry[] Catalog => Entries;
~~~

~~~csharp
0333: 		try
0334: 		{
0335: 			IntelHelper.InitTutorialPage(val, "", "77223", L.T("——导师", "— The Mentor"));
0336: 		}
0337: 		catch (Exception ex)
~~~

## CorpPresence/Mentor.cs

~~~csharp
0009: 	public override string Id => "mentor";
0010: 
0011: 	public override string Name => L.T("导师", "The Mentor");
0012: 
0013: 	public override string Faction => "FACTION_LOWER_LEVEL";
~~~

~~~csharp
0035: 	public override int BuyGoodsNeeded => 1;
0036: 
0037: 	private static string Nod => Character.Act(L.T("点点头", "nods"), "");
0038: 
0039: 	public override int Rep()
~~~

~~~csharp
0087: 			visit3.Lines = new string[3]
0088: 			{
0089: 				L.T("码头的货，大半都得过我们的手。", "Most of the cargo at the docks passes through our hands."),
0090: 				L.T("这是巢火和解放者，我们手搓的。", "This is the Nestfire and the Liberator. We made them by hand."),
0091: 				Character.Act(L.T("便签放在柜台上", "leaves a note on the counter"), L.T("有事，打这个。", "Need something, call this."))
~~~

~~~csharp
0088: 			{
0089: 				L.T("码头的货，大半都得过我们的手。", "Most of the cargo at the docks passes through our hands."),
0090: 				L.T("这是巢火和解放者，我们手搓的。", "This is the Nestfire and the Liberator. We made them by hand."),
0091: 				Character.Act(L.T("便签放在柜台上", "leaves a note on the counter"), L.T("有事，打这个。", "Need something, call this."))
0092: 			};
~~~

~~~csharp
0089: 				L.T("码头的货，大半都得过我们的手。", "Most of the cargo at the docks passes through our hands."),
0090: 				L.T("这是巢火和解放者，我们手搓的。", "This is the Nestfire and the Liberator. We made them by hand."),
0091: 				Character.Act(L.T("便签放在柜台上", "leaves a note on the counter"), L.T("有事，打这个。", "Need something, call this."))
0092: 			};
0093: 			visit3.Exit = Nod;
~~~

~~~csharp
0101: 			visit2.Lines = new string[2]
0102: 			{
0103: 				L.T("要加入星际客联盟吗。", "Want to join the Spacer Alliance?"),
0104: 				L.T("黑市，革命军，下层区，记住我们是一个整体。", "The Black Market, the Revolution, the Lower Levels. Remember, we are one whole.")
0105: 			};
~~~

~~~csharp
0102: 			{
0103: 				L.T("要加入星际客联盟吗。", "Want to join the Spacer Alliance?"),
0104: 				L.T("黑市，革命军，下层区，记住我们是一个整体。", "The Black Market, the Revolution, the Lower Levels. Remember, we are one whole.")
0105: 			};
0106: 			visit2.Exit = Nod;
~~~

~~~csharp
0114: 			visit.Lines = new string[2]
0115: 			{
0116: 				L.T("星际客也有自己的港口。", "Spacers have their own port too."),
0117: 				Character.Act(L.T("敲了敲柜台", "knocks on the counter"), L.T("你这儿也算一个。", "Yours counts as one too."))
0118: 			};
~~~

~~~csharp
0115: 			{
0116: 				L.T("星际客也有自己的港口。", "Spacers have their own port too."),
0117: 				Character.Act(L.T("敲了敲柜台", "knocks on the counter"), L.T("你这儿也算一个。", "Yours counts as one too."))
0118: 			};
0119: 			visit.Exit = Nod;
~~~

## CorpPresence/CompanyVisitors.cs

~~~csharp
0240: 		try
0241: 		{
0242: 			string text = L.T("索拉纳贵宾", "Solana VIP");
0243: 			StoreClient val = new StoreClient
0244: 			{
~~~

~~~csharp
0250: 			};
0251: 			val.SetBudget(budget);
0252: 			val.mainDialogue.SetText(text, L.T("索拉纳的顾问推荐了你的店。", "A Solana consultant recommended your shop.")).NextDialogue().SetText(text, L.T("听说你这儿有好东西。奢侈品、好烟、好酒，拿出来看看。", "I hear you carry fine things. Luxury goods, good smokes, good liquor. Let's see them."));
0253: 			string[] array = new string[3] { "LUXURY_ITEM", "CIGARETTE_TAG", "ALCOHOL" };
0254: 			foreach (string item in array)
~~~

~~~csharp
0325: 		if (orders.Any((Order o) => o.Kind == "swap"))
0326: 		{
0327: 			list.Add(L.T("问题批次已替换为正品。请核对。", "The problem batch has been replaced with genuine product. Please check."));
0328: 		}
0329: 		if (orders.Any((Order o) => o.Kind == "refurbish"))
~~~

~~~csharp
0329: 		if (orders.Any((Order o) => o.Kind == "refurbish"))
0330: 		{
0331: 			list.Add(L.T("返厂的枪。翻新好了。", "Your guns from the factory. Refurbished."));
0332: 		}
0333: 		if (orders.Any((Order o) => o.Kind == "stamina"))
~~~

~~~csharp
0333: 		if (orders.Any((Order o) => o.Kind == "stamina"))
0334: 		{
0335: 			list.Add(L.T("伊莎·兰登经办的特别申请。请签收。", "A special request handled by Isa Langdon. Please sign."));
0336: 		}
0337: 		if (num > 0)
~~~

~~~csharp
0337: 		if (num > 0)
0338: 		{
0339: 			list.Add(L.T($"这是这一期的返点，{num}₵。", $"Here's your rebate: {num}₵."));
0340: 		}
0341: 		if (card)
~~~

~~~csharp
0341: 		if (card)
0342: 		{
0343: 			list.Add(flag3 ? L.T("导师让我带张便签。", "The Mentor asked me to bring you a note.") : L.T((Engine.Find(c.CharacterId)?.Name ?? c.Name) + "让我把名片带给你。", (Engine.Find(c.CharacterId)?.Name ?? c.Name) + " asked me to bring you a card."));
0344: 		}
0345: 		string text2 = (L.En ? (flag5 ? "<i>(wheels the empty cart away)</i>" : (flag4 ? "<i>(salutes)</i>" : (flag3 ? "<i>(nods)</i>" : ((!flag2) ? (flag ? "<i>(bows slightly)</i>Have a pleasant day." : "<i>(nods)</i>See you next time.") : "<i>(takes back the receipt)</i>Thank you for your cooperation.")))) : (flag5 ? "<i>（推着空推车走了）</i>" : (flag4 ? "<i>（敬了个礼）</i>" : (flag3 ? "<i>（点点头）</i>" : ((!flag2) ? (flag ? "<i>（微微欠身）</i>祝您愉快。" : "<i>（点点头）</i>下回见。") : "<i>（收回签收单）</i>谢谢配合。")))));
~~~

~~~csharp
0357: 		if (list.Count == 0)
0358: 		{
0359: 			list.Add(L.T("送货。请签收。", "Delivery. Please sign."));
0360: 		}
0361: 		Dialogue val2 = new Dialogue().SetText(text, list[0]);
~~~

~~~csharp
0359: 			list.Add(L.T("送货。请签收。", "Delivery. Please sign."));
0360: 		}
0361: 		Dialogue val2 = new Dialogue().SetText(text, list[0]);
0362: 		Dialogue val3 = val2;
0363: 		for (int num3 = 1; num3 < list.Count; num3++)
~~~

~~~csharp
0363: 		for (int num3 = 1; num3 < list.Count; num3++)
0364: 		{
0365: 			val3 = val3.NextDialogue().SetText(text, list[num3]);
0366: 		}
0367: 		Dialogue val4 = val3.NextDialogue().SetText(text, text2);
~~~

~~~csharp
0365: 			val3 = val3.NextDialogue().SetText(text, list[num3]);
0366: 		}
0367: 		Dialogue val4 = val3.NextDialogue().SetText(text, text2);
0368: 		val3.SetEndAction(CreateDeliveryReceipt(c, s, orders, card));
0369: 		val4.SetEndAction(CreateCourierLeave(val));
~~~

~~~csharp
0385: 		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
0386: 		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
0387: 		string text = L.T("帕斯泰尔货商", "pastel Trader");
0388: 		StoreClient client = new StoreClient
0389: 		{
~~~

~~~csharp
0399: 		client.SetWholesale(20);
0400: 		string[] stock = Engine.Pick(PastelGoods, 3, 5);
0401: 		Dialogue val = new Dialogue().SetText(text, L.T("看到你的灯牌了。", "Saw your sign."));
0402: 		Dialogue obj = val.NextDialogue().SetText(text, L.T("帕斯泰尔的货，要吗？", "pastel goods. Interested?"));
0403: 		bool placed = false;
~~~

~~~csharp
0400: 		string[] stock = Engine.Pick(PastelGoods, 3, 5);
0401: 		Dialogue val = new Dialogue().SetText(text, L.T("看到你的灯牌了。", "Saw your sign."));
0402: 		Dialogue obj = val.NextDialogue().SetText(text, L.T("帕斯泰尔的货，要吗？", "pastel goods. Interested?"));
0403: 		bool placed = false;
0404: 		obj.SetEndAction((Action)delegate
~~~

~~~csharp
0427: 		});
0428: 		client.mainDialogue = val;
0429: 		Dialogue val2 = new Dialogue().SetText(text, L.T("<i>（点点头）</i>下回再来。", "<i>(nods)</i>I'll be back.")).SetEndAction((Action)delegate
0430: 		{
0431: 			try
~~~

~~~csharp
0468: 		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
0469: 		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
0470: 		string text = L.T("索拉纳采购员", "Solana Buyer");
0471: 		HashSet<string> list = new HashSet<string>(s.LabList);
0472: 		StoreClient client = new StoreClient
~~~

~~~csharp
0493: 			}
0494: 		};
0495: 		client.placedWrongItemWhenSellingToDialogue = new Dialogue().SetText(text, L.T("单子上没有这个。", "That's not on the list."));
0496: 		Dialogue val = new Dialogue().SetText(text, L.T("实验室的采购单。东西放台上。", "Purchase order from the lab. Put the items on the counter."));
0497: 		val.SetEndAction((Action)delegate
~~~

~~~csharp
0494: 		};
0495: 		client.placedWrongItemWhenSellingToDialogue = new Dialogue().SetText(text, L.T("单子上没有这个。", "That's not on the list."));
0496: 		Dialogue val = new Dialogue().SetText(text, L.T("实验室的采购单。东西放台上。", "Purchase order from the lab. Put the items on the counter."));
0497: 		val.SetEndAction((Action)delegate
0498: 		{
~~~

~~~csharp
0501: 		});
0502: 		client.mainDialogue = val;
0503: 		Dialogue allDoneDialogue = new Dialogue().SetText(text, L.T("<i>（在单子上打了个勾）</i>收齐了。", "<i>(ticks the list)</i>That's everything.")).SetEndAction((Action)delegate
0504: 		{
0505: 			Engine.BuyActive = false;
~~~

~~~csharp
0516: 			}
0517: 		});
0518: 		client.acceptDealDialogue = new Dialogue().SetText(text, L.T("收下了。单子上还有的，接着放。", "Got it. If there's more from the list, keep it coming."));
0519: 		client.allDoneDialogue = allDoneDialogue;
0520: 		return client;
~~~

~~~csharp
0741: 				if (instance != null)
0742: 				{
0743: 					instance.Notify(L.T($"{c.Name}还有 {num} 件货这次没送成（柜台放满或箱子装满），明天再送来。", $"{c.Name} couldn't deliver {num} items this time (counter or crate full). They'll come tomorrow."), "default");
0744: 				}
0745: 			}
~~~

## CorpPresence/MeridianCo.cs

~~~csharp
0158: 	public override string Id => "meridian";
0159: 
0160: 	public override string Name => L.T("子午线兵工", "Meridian Ballistics");
0161: 
0162: 	public override string CharacterId => "rosen";
~~~

~~~csharp
0164: 	public override long PhoneNumber => 63743L;
0165: 
0166: 	public override string[] TierNames => L.T(new string[3] { "一级许可", "二级许可", "三级许可" }, new string[3] { "Class I License", "Class II License", "Class III License" });
0167: 
0168: 	public override string CardFlavor => L.T("黑底红字，一道经线标记，63743。背面印着：返厂服务，终身有效。", "Red on black, a single meridian line, 63743. On the back: Factory service, for life.");
~~~

~~~csharp
0166: 	public override string[] TierNames => L.T(new string[3] { "一级许可", "二级许可", "三级许可" }, new string[3] { "Class I License", "Class II License", "Class III License" });
0167: 
0168: 	public override string CardFlavor => L.T("黑底红字，一道经线标记，63743。背面印着：返厂服务，终身有效。", "Red on black, a single meridian line, 63743. On the back: Factory service, for life.");
0169: 
0170: 	public override string DeskName => L.T("子午线订货处", "Meridian Order Desk");
~~~

~~~csharp
0168: 	public override string CardFlavor => L.T("黑底红字，一道经线标记，63743。背面印着：返厂服务，终身有效。", "Red on black, a single meridian line, 63743. On the back: Factory service, for life.");
0169: 
0170: 	public override string DeskName => L.T("子午线订货处", "Meridian Order Desk");
0171: 
0172: 	public override string DeskAsk => "";
~~~

~~~csharp
0172: 	public override string DeskAsk => "";
0173: 
0174: 	public override string[] FrontDesk => L.T(new string[1] { "子午线兵工，订货处。报许可编号。" }, new string[1] { "Meridian Ballistics, order desk. License number." });
0175: 
0176: 	public override string FrontDeskDone => L.T("登记了。明天到。", "Logged. Tomorrow.");
~~~

~~~csharp
0174: 	public override string[] FrontDesk => L.T(new string[1] { "子午线兵工，订货处。报许可编号。" }, new string[1] { "Meridian Ballistics, order desk. License number." });
0175: 
0176: 	public override string FrontDeskDone => L.T("登记了。明天到。", "Logged. Tomorrow.");
0177: 
0178: 	public static int ForgeFee => Services.Fee("meridian", "forge", 600);
~~~

~~~csharp
0215: 		if (companyState == null)
0216: 		{
0217: 			return L.T("存档未就绪", "Save not ready");
0218: 		}
0219: 		if (companyState.ForgeOwned)
~~~

~~~csharp
0219: 		if (companyState.ForgeOwned)
0220: 		{
0221: 			return L.T("已经办过了", "Already done");
0222: 		}
0223: 		if (!ForgeOffered)
~~~

~~~csharp
0223: 		if (!ForgeOffered)
0224: 		{
0225: 			return L.T("还没有开通", "Not available yet");
0226: 		}
0227: 		if (!PayForgeFee())
~~~

~~~csharp
0227: 		if (!PayForgeFee())
0228: 		{
0229: 			return L.T("现金不够", "Not enough cash");
0230: 		}
0231: 		companyState.Orders.Add(new Order
~~~

~~~csharp
0242: 		if (companyState == null)
0243: 		{
0244: 			return L.T("存档未就绪", "Save not ready");
0245: 		}
0246: 		if (!companyState.ForgeOwned)
~~~

~~~csharp
0246: 		if (!companyState.ForgeOwned)
0247: 		{
0248: 			return L.T("还没有开通", "Not available yet");
0249: 		}
0250: 		string text = Services.OrderBlocked(c);
~~~

~~~csharp
0309: 			return "?";
0310: 		}
0311: 		return L.T(LadderName[level], LadderNameEn[level]);
0312: 	}
0313: 
~~~

## CorpPresence/Visit.cs

~~~csharp
0021: 	public string[] Extra = Array.Empty<string>();
0022: 
0023: 	public string Wrong = L.T("别糊弄我。", "Don't try to fool me.");
0024: 
0025: 	public bool TalkOnly;
~~~

## CorpPresence/MachineUpgrade.cs

~~~csharp
0198: 				Rig value;
0199: 				int num2 = ((!Rigs.TryGetValue(machine, out value)) ? 1 : value.RowsPerLevel);
0200: 				Notify(flag ? L.T($"{name}容量升到 {level} 级：原料格和成品格各多 {num2} 行", $"{name} capacity upgraded to level {level}: {num2} more row(s) of input and output") : L.T(name + "装好省电件：每次加工少耗 1 电", name + " power saver installed: 1 less charge per run"));
0201: 				ModLog.Info(string.Format("机器升级：{0} {1} {2} 级", machine.identifier, flag ? "容量" : "省电", level));
0202: 			}
~~~

~~~csharp
0203: 			else
0204: 			{
0205: 				Notify(L.T(string.Format("{0}{1}：{2}/{3}", name, flag ? $"容量 {level + 1} 级" : "省电", paid, num), string.Format("{0} {1}: {2}/{3}", name, flag ? $"capacity level {level + 1}" : "power saver", paid, num)));
0206: 			}
0207: 			machine.ModifyTag(text2, (Action<TagState>)delegate(TagState s)
~~~

~~~csharp
0230: 				int num = Level(m, "CORP_UPG_CAP_INT");
0231: 				int num2 = Level(m, "CORP_UPG_CAP_PAID_INT");
0232: 				list.Add((num >= CapCost.Length) ? L.T($"容量：{num}/{CapCost.Length} 级（已满）", $"Capacity: level {num}/{CapCost.Length} (max)") : L.T($"容量：{num}/{CapCost.Length} 级，拖金属锭上来升级（这级 {num2}/{CapCost[num]} 块，纯度不限）", $"Capacity: level {num}/{CapCost.Length}. Drag metal ingots onto it to upgrade ({num2}/{CapCost[num]} for this level, any purity)"));
0233: 			}
0234: 			if (Is(m, PowerMachines))
~~~

~~~csharp
0234: 			if (Is(m, PowerMachines))
0235: 			{
0236: 				list.Add((Level(m, "CORP_UPG_PWR_INT") > 0) ? L.T("省电：已装，每次加工少耗 1 电", "Power saver: installed, 1 less charge per run") : L.T(string.Format("省电：拖电子元件上来安装（{0}/{1} 个），每次加工少耗 1 电", Level(m, "CORP_UPG_PWR_PAID_INT"), 3), string.Format("Power saver: drag electronic components onto it ({0}/{1}) for 1 less charge per run", Level(m, "CORP_UPG_PWR_PAID_INT"), 3)));
0237: 			}
0238: 			return (list.Count == 0) ? "" : ("\n\n" + string.Join("\n", list));
~~~

## CorpPresence/Rosen.cs

~~~csharp
0025: 	public override string Id => "rosen";
0026: 
0027: 	public override string Name => L.T("罗森", "Rosen");
0028: 
0029: 	public override string Faction => "FACTION_SECURITY";
~~~

~~~csharp
0041: 	public override int BuyGoodsNeeded => 1;
0042: 
0043: 	private static string Bye => L.T("保重", "Take care.");
0044: 
0045: 	private static int Disc
~~~

~~~csharp
0057: 	private static string[] Talk => new string[6]
0058: 	{
0059: 		L.T("哨兵3型不会卡壳，至少宣传上是这样。", "The Sentinel doesn't jam. At least, that's what the ads say."),
0060: 		L.T("非致命弹，就数治安部用得最多。报告上写着好看。", "Non-lethal rounds are what Security uses most. They look good in reports."),
0061: 		L.T("穿甲弹贵一点，毕竟明面上不让卖。", "AP rounds cost more. They're not supposed to be sold openly, after all."),
~~~

~~~csharp
0058: 	{
0059: 		L.T("哨兵3型不会卡壳，至少宣传上是这样。", "The Sentinel doesn't jam. At least, that's what the ads say."),
0060: 		L.T("非致命弹，就数治安部用得最多。报告上写着好看。", "Non-lethal rounds are what Security uses most. They look good in reports."),
0061: 		L.T("穿甲弹贵一点，毕竟明面上不让卖。", "AP rounds cost more. They're not supposed to be sold openly, after all."),
0062: 		Character.Act(L.T("敲了敲右臂", "taps his right arm"), L.T("每年都得回厂保养一次。", "Back to the factory once a year for maintenance.")),
~~~

~~~csharp
0059: 		L.T("哨兵3型不会卡壳，至少宣传上是这样。", "The Sentinel doesn't jam. At least, that's what the ads say."),
0060: 		L.T("非致命弹，就数治安部用得最多。报告上写着好看。", "Non-lethal rounds are what Security uses most. They look good in reports."),
0061: 		L.T("穿甲弹贵一点，毕竟明面上不让卖。", "AP rounds cost more. They're not supposed to be sold openly, after all."),
0062: 		Character.Act(L.T("敲了敲右臂", "taps his right arm"), L.T("每年都得回厂保养一次。", "Back to the factory once a year for maintenance.")),
0063: 		L.T("这道疤？演习。谁想到演习也用实弹。", "This scar? A drill. Who knew drills used live rounds."),
~~~

~~~csharp
0060: 		L.T("非致命弹，就数治安部用得最多。报告上写着好看。", "Non-lethal rounds are what Security uses most. They look good in reports."),
0061: 		L.T("穿甲弹贵一点，毕竟明面上不让卖。", "AP rounds cost more. They're not supposed to be sold openly, after all."),
0062: 		Character.Act(L.T("敲了敲右臂", "taps his right arm"), L.T("每年都得回厂保养一次。", "Back to the factory once a year for maintenance.")),
0063: 		L.T("这道疤？演习。谁想到演习也用实弹。", "This scar? A drill. Who knew drills used live rounds."),
0064: 		L.T("子午线可从来不卖给黑市，我们的活计只是损耗大一点。", "Meridian never sells to the Black Market. Our stock just has a higher loss rate.")
~~~

~~~csharp
0061: 		L.T("穿甲弹贵一点，毕竟明面上不让卖。", "AP rounds cost more. They're not supposed to be sold openly, after all."),
0062: 		Character.Act(L.T("敲了敲右臂", "taps his right arm"), L.T("每年都得回厂保养一次。", "Back to the factory once a year for maintenance.")),
0063: 		L.T("这道疤？演习。谁想到演习也用实弹。", "This scar? A drill. Who knew drills used live rounds."),
0064: 		L.T("子午线可从来不卖给黑市，我们的活计只是损耗大一点。", "Meridian never sells to the Black Market. Our stock just has a higher loss rate.")
0065: 	};
~~~

~~~csharp
0062: 		Character.Act(L.T("敲了敲右臂", "taps his right arm"), L.T("每年都得回厂保养一次。", "Back to the factory once a year for maintenance.")),
0063: 		L.T("这道疤？演习。谁想到演习也用实弹。", "This scar? A drill. Who knew drills used live rounds."),
0064: 		L.T("子午线可从来不卖给黑市，我们的活计只是损耗大一点。", "Meridian never sells to the Black Market. Our stock just has a higher loss rate.")
0065: 	};
0066: 
~~~

~~~csharp
0166: 			visit4.Lines = new string[3]
0167: 			{
0168: 				L.T("子午线兵工，罗森。", "Meridian Ballistics. Rosen."),
0169: 				L.T("子午线致力于生产标配军工（官方腔调）。", "Meridian is dedicated to standard-issue arms. (in an official tone)"),
0170: 				Character.Act(L.T("把一把哨兵3型放在柜台上", "sets a Sentinel Mk. III on the counter"), "")
~~~

~~~csharp
0167: 			{
0168: 				L.T("子午线兵工，罗森。", "Meridian Ballistics. Rosen."),
0169: 				L.T("子午线致力于生产标配军工（官方腔调）。", "Meridian is dedicated to standard-issue arms. (in an official tone)"),
0170: 				Character.Act(L.T("把一把哨兵3型放在柜台上", "sets a Sentinel Mk. III on the counter"), "")
0171: 			};
~~~

~~~csharp
0168: 				L.T("子午线兵工，罗森。", "Meridian Ballistics. Rosen."),
0169: 				L.T("子午线致力于生产标配军工（官方腔调）。", "Meridian is dedicated to standard-issue arms. (in an official tone)"),
0170: 				Character.Act(L.T("把一把哨兵3型放在柜台上", "sets a Sentinel Mk. III on the counter"), "")
0171: 			};
0172: 			visit4.Exit = Bye;
~~~

~~~csharp
0180: 			visit3.Lines = new string[3]
0181: 			{
0182: 				L.T("和平使者，标准军备。", "The Peacemaker. Standard armament."),
0183: 				Character.Act(L.T("指了指左脸那道疤", "points at the scar on his left cheek"), L.T("我身上绝大部分疤痕都是它留下的。", "Most of the scars on me came from one of these.")),
0184: 				L.T("所以我知道它好用。", "So I know it works.")
~~~

~~~csharp
0181: 			{
0182: 				L.T("和平使者，标准军备。", "The Peacemaker. Standard armament."),
0183: 				Character.Act(L.T("指了指左脸那道疤", "points at the scar on his left cheek"), L.T("我身上绝大部分疤痕都是它留下的。", "Most of the scars on me came from one of these.")),
0184: 				L.T("所以我知道它好用。", "So I know it works.")
0185: 			};
~~~

~~~csharp
0182: 				L.T("和平使者，标准军备。", "The Peacemaker. Standard armament."),
0183: 				Character.Act(L.T("指了指左脸那道疤", "points at the scar on his left cheek"), L.T("我身上绝大部分疤痕都是它留下的。", "Most of the scars on me came from one of these.")),
0184: 				L.T("所以我知道它好用。", "So I know it works.")
0185: 			};
0186: 			visit3.Exit = Bye;
~~~

~~~csharp
0194: 			visit2.Lines = new string[3]
0195: 			{
0196: 				L.T("“壁垒”，双动式霰弹枪，嘭！就结束了。", "The Rampart. Dual-action shotgun. Boom! And it's over."),
0197: 				L.T("产线一天出几百把，每一把都一样（撇嘴）。", "The line turns out hundreds a day, every one the same. (curls his lip)"),
0198: 				Character.Act(L.T("拍了拍枪托", "pats the stock"), L.T("当然这使维修很方便我承认。", "It does make repairs easy, I'll admit."))
~~~

~~~csharp
0195: 			{
0196: 				L.T("“壁垒”，双动式霰弹枪，嘭！就结束了。", "The Rampart. Dual-action shotgun. Boom! And it's over."),
0197: 				L.T("产线一天出几百把，每一把都一样（撇嘴）。", "The line turns out hundreds a day, every one the same. (curls his lip)"),
0198: 				Character.Act(L.T("拍了拍枪托", "pats the stock"), L.T("当然这使维修很方便我承认。", "It does make repairs easy, I'll admit."))
0199: 			};
~~~

~~~csharp
0196: 				L.T("“壁垒”，双动式霰弹枪，嘭！就结束了。", "The Rampart. Dual-action shotgun. Boom! And it's over."),
0197: 				L.T("产线一天出几百把，每一把都一样（撇嘴）。", "The line turns out hundreds a day, every one the same. (curls his lip)"),
0198: 				Character.Act(L.T("拍了拍枪托", "pats the stock"), L.T("当然这使维修很方便我承认。", "It does make repairs easy, I'll admit."))
0199: 			};
0200: 			visit2.Exit = Bye;
~~~

~~~csharp
0210: 			visit.Lines = new string[3]
0211: 			{
0212: 				L.T("公司的枪当然都有编号，这两把显然没有。", "Company guns all have serial numbers, of course. These two clearly don't."),
0213: 				Character.Act(L.T("打开一个旧木盒", "opens an old wooden case"), L.T("左轮，步枪。我自己做的。", "A revolver. A rifle. I made them myself.")),
0214: 				L.T("治安部用不上它们。相信你会找到用得上的人。", "Security has no use for them. I trust you'll find someone who does.")
~~~

~~~csharp
0211: 			{
0212: 				L.T("公司的枪当然都有编号，这两把显然没有。", "Company guns all have serial numbers, of course. These two clearly don't."),
0213: 				Character.Act(L.T("打开一个旧木盒", "opens an old wooden case"), L.T("左轮，步枪。我自己做的。", "A revolver. A rifle. I made them myself.")),
0214: 				L.T("治安部用不上它们。相信你会找到用得上的人。", "Security has no use for them. I trust you'll find someone who does.")
0215: 			};
~~~

~~~csharp
0212: 				L.T("公司的枪当然都有编号，这两把显然没有。", "Company guns all have serial numbers, of course. These two clearly don't."),
0213: 				Character.Act(L.T("打开一个旧木盒", "opens an old wooden case"), L.T("左轮，步枪。我自己做的。", "A revolver. A rifle. I made them myself.")),
0214: 				L.T("治安部用不上它们。相信你会找到用得上的人。", "Security has no use for them. I trust you'll find someone who does.")
0215: 			};
0216: 			visit.Exit = Character.Act(L.T("合上木盒", "closes the case"), L.T("保养好它们。", "Take care of them."));
~~~

~~~csharp
0214: 				L.T("治安部用不上它们。相信你会找到用得上的人。", "Security has no use for them. I trust you'll find someone who does.")
0215: 			};
0216: 			visit.Exit = Character.Act(L.T("合上木盒", "closes the case"), L.T("保养好它们。", "Take care of them."));
0217: 			visit.Discount = Disc;
0218: 			visit.Stock = new string[2] { "@rosen_revolver", "@rosen_rifle" };
~~~

~~~csharp
0228: 			Lines = new string[4]
0229: 			{
0230: 				L.T("店长，对锻造有兴趣吗？", "Interested in forging, boss?"),
0231: 				Character.Act(L.T("把一块钢胚放在柜台上", "sets a billet on the counter"), L.T("高纯度的金属锭，在炉子里烧一夜，就是这个。", "A high-purity metal ingot, a night in the forge, and you get this.")),
0232: 				L.T("我最近买了一套。。。呃不是，我最近希望传播这门手艺。", "I recently bought a set... er, no. I've been wanting to pass on the craft."),
~~~

~~~csharp
0229: 			{
0230: 				L.T("店长，对锻造有兴趣吗？", "Interested in forging, boss?"),
0231: 				Character.Act(L.T("把一块钢胚放在柜台上", "sets a billet on the counter"), L.T("高纯度的金属锭，在炉子里烧一夜，就是这个。", "A high-purity metal ingot, a night in the forge, and you get this.")),
0232: 				L.T("我最近买了一套。。。呃不是，我最近希望传播这门手艺。", "I recently bought a set... er, no. I've been wanting to pass on the craft."),
0233: 				L.T($"我手里的整套家伙 {MeridianCo.ForgeFee}。钢胚还送你三块，练手用足够了。", $"The whole kit I've got is {MeridianCo.ForgeFee}. I'll throw in three billets, plenty for practice.")
~~~

~~~csharp
0230: 				L.T("店长，对锻造有兴趣吗？", "Interested in forging, boss?"),
0231: 				Character.Act(L.T("把一块钢胚放在柜台上", "sets a billet on the counter"), L.T("高纯度的金属锭，在炉子里烧一夜，就是这个。", "A high-purity metal ingot, a night in the forge, and you get this.")),
0232: 				L.T("我最近买了一套。。。呃不是，我最近希望传播这门手艺。", "I recently bought a set... er, no. I've been wanting to pass on the craft."),
0233: 				L.T($"我手里的整套家伙 {MeridianCo.ForgeFee}。钢胚还送你三块，练手用足够了。", $"The whole kit I've got is {MeridianCo.ForgeFee}. I'll throw in three billets, plenty for practice.")
0234: 			},
~~~

~~~csharp
0231: 				Character.Act(L.T("把一块钢胚放在柜台上", "sets a billet on the counter"), L.T("高纯度的金属锭，在炉子里烧一夜，就是这个。", "A high-purity metal ingot, a night in the forge, and you get this.")),
0232: 				L.T("我最近买了一套。。。呃不是，我最近希望传播这门手艺。", "I recently bought a set... er, no. I've been wanting to pass on the craft."),
0233: 				L.T($"我手里的整套家伙 {MeridianCo.ForgeFee}。钢胚还送你三块，练手用足够了。", $"The whole kit I've got is {MeridianCo.ForgeFee}. I'll throw in three billets, plenty for practice.")
0234: 			},
0235: 			Exit = Bye,
~~~

~~~csharp
0235: 			Exit = Bye,
0236: 			TalkOnly = true,
0237: 			Ask = L.T("学不学？", "Well?")
0238: 		};
0239: 		v.Choices = new Choice[2]
~~~

~~~csharp
0239: 		v.Choices = new Choice[2]
0240: 		{
0241: 			new Choice(L.T("我学。", "Teach me."), L.T("炉子坏了，找我订。", "If the forge breaks, order from me."), delegate
0242: 			{
0243: 				if (MeridianCo.PayForgeFee())
~~~

~~~csharp
0250: 				}
0251: 			}),
0252: 			new Choice(L.T("改天。", "Another time."), L.T("想好了打电话。", "Call when you're ready."), null)
0253: 		};
0254: 		return v;
~~~

~~~csharp
0259: 		string[] talk = Talk;
0260: 		List<string> list = new List<string>();
0261: 		list.Add(Engine.Any(L.T("子午线。例行补货。", "Meridian. Routine restock."), L.T("罗森。照旧。", "Rosen. As usual."), L.T("补货。几分钟。", "Restock. A few minutes.")));
0262: 		list.Add(Engine.Any(talk));
0263: 		List<string> list2 = list;
~~~

~~~csharp
0269: 			Discount = Disc,
0270: 			Both = true,
0271: 			Wrong = L.T("只收材料。", "Materials only.")
0272: 		};
0273: 		if (s.Order != null)
~~~

~~~csharp
0274: 		{
0275: 			list3.Add(s.Order);
0276: 			list2.Add(Character.Act(L.T("放下一个木盒", "sets down a wooden case"), L.T("这是你订的“手工品”。", "Your \"handmade\" order.")));
0277: 		}
0278: 		if (s.Chapter >= 4)
~~~

~~~csharp
0278: 		if (s.Chapter >= 4)
0279: 		{
0280: 			visit.Ask = L.T("要订一把吗？", "Want to order one?");
0281: 			visit.Choices = new Choice[3]
0282: 			{
~~~

~~~csharp
0281: 			visit.Choices = new Choice[3]
0282: 			{
0283: 				new Choice(L.T("左轮", "Revolver"), L.T("七天，手工制作，快不了。", "Seven days. Handmade. Can't rush it."), delegate
0284: 				{
0285: 					s.Order = "@rosen_revolver";
~~~

~~~csharp
0285: 					s.Order = "@rosen_revolver";
0286: 				}),
0287: 				new Choice(L.T("步枪", "Rifle"), L.T("七天，手工制作，快不了。", "Seven days. Handmade. Can't rush it."), delegate
0288: 				{
0289: 					s.Order = "@rosen_rifle";
~~~

~~~csharp
0289: 					s.Order = "@rosen_rifle";
0290: 				}),
0291: 				new Choice(L.T("不用", "No thanks"), L.T("好。", "Fine."), delegate
0292: 				{
0293: 					s.Order = null;
~~~

## CorpPresence/SolanaCo.cs

~~~csharp
0015: 	public override string Id => "solana";
0016: 
0017: 	public override string Name => L.T("索拉纳生物", "Solana Biogenics");
0018: 
0019: 	public override string CharacterId => "seline";
~~~

~~~csharp
0021: 	public override long PhoneNumber => 76526L;
0022: 
0023: 	public override string[] TierNames => L.T(new string[3] { "银卡会员", "金卡会员", "黑金会员" }, new string[3] { "Silver Member", "Gold Member", "Black Gold Member" });
0024: 
0025: 	public override string CardFlavor => L.T("烫金的字，只有一个号码：76526。", "Gold-foil lettering. Just one number: 76526.");
~~~

~~~csharp
0023: 	public override string[] TierNames => L.T(new string[3] { "银卡会员", "金卡会员", "黑金会员" }, new string[3] { "Silver Member", "Gold Member", "Black Gold Member" });
0024: 
0025: 	public override string CardFlavor => L.T("烫金的字，只有一个号码：76526。", "Gold-foil lettering. Just one number: 76526.");
0026: 
0027: 	public override string DeskName => L.T("索拉纳前台", "Solana Concierge");
~~~

~~~csharp
0025: 	public override string CardFlavor => L.T("烫金的字，只有一个号码：76526。", "Gold-foil lettering. Just one number: 76526.");
0026: 
0027: 	public override string DeskName => L.T("索拉纳前台", "Solana Concierge");
0028: 
0029: 	public override string DeskAsk => L.T("请问需要什么服务？", "Which service may I assist you with?");
~~~

~~~csharp
0027: 	public override string DeskName => L.T("索拉纳前台", "Solana Concierge");
0028: 
0029: 	public override string DeskAsk => L.T("请问需要什么服务？", "Which service may I assist you with?");
0030: 
0031: 	public override string[] FrontDesk => new string[2]
~~~

~~~csharp
0031: 	public override string[] FrontDesk => new string[2]
0032: 	{
0033: 		L.T("索拉纳生物。正在为您调取会员档案。", "Solana Biogenics. Retrieving your membership file."),
0034: 		L.T("索拉纳。请说。", "Solana. Please, go ahead.")
0035: 	};
~~~

~~~csharp
0032: 	{
0033: 		L.T("索拉纳生物。正在为您调取会员档案。", "Solana Biogenics. Retrieving your membership file."),
0034: 		L.T("索拉纳。请说。", "Solana. Please, go ahead.")
0035: 	};
0036: 
~~~

~~~csharp
0035: 	};
0036: 
0037: 	public override string FrontDeskDone => L.T("已为您安排，明天专人上门。", "All arranged. Someone will call on you tomorrow.");
0038: 
0039: 	public static int VipFee => Services.Fee("solana", "vip", 300);
~~~

## CorpPresence/Settings.cs

~~~csharp
0253: 	{
0254: 		ModConfig config = ModConfig.For(modId);
0255: 		_cap = config.Define("DailyCap", "不限", "来访 / Visits", (Func<string>)(() => L.T("每天最多几位公司人物", "Max company characters per day")), new string[1] { "不限" }.Concat(Enumerable.Range(1, 6).Select(delegate(int n)
0256: 		{
0257: 			int num2 = n;
~~~

~~~csharp
0257: 			int num2 = n;
0258: 			return num2.ToString();
0259: 		})).ToArray(), (Func<string>)(() => L.T("超出的人顺延到第二天：剧情优先，其次是最久没来的。花钱请来的客流、贵宾、实验室访客不算在内。", "Extra visitors are pushed to the next day: story first, then whoever has waited longest. Paid foot traffic, VIPs and lab buyers don't count.")), false);
0260: 		_limits = config.Define("OrderLimits", false, "订货 / Orders", (Func<string>)(() => L.T("订货每周限量", "Weekly order limits")), (Func<string>)(() => L.T("打开后，各公司电话订货的每样货每周限量，按一份的基础价值分阶：\n", "When on, each item ordered by phone has a weekly limit, tiered by its base value:\n") + L.T("50₵ 以下 20 份，200₵ 以下 10 份，300₵ 以下 5 份，300₵ 及以上 3 份。\n", "under 50₵: 20, under 200₵: 10, under 300₵: 5, 300₵ and up: 3.\n") + L.T("海明威限量包装、免疫宁整箱这些独家货不管开不开都按原来的限量。", "Exclusives like Hemingway Limited Edition and the Immunivax case keep their own limits either way.")), false);
0261: 		_orderEvents = config.Define("OrderEvents", true, "订货 / Orders", (Func<string>)(() => L.T("事件影响订货", "Events affect orders")), (Func<string>)(() => L.T("打开后，原版市场事件（某类物品涨价、降价）也会影响电话订货的单价：事件让这类物品涨多少、跌多少，订货价就跟着涨跌多少，和店里同类物品的估价一致。\n", "When on, vanilla market events (a type of item going up or down in price) also change phone order prices: the order price rises or falls by as much as the event moves that type of item, matching how the shop values it.\n") + L.T("下单时按当时的价格结算，之后事件结束也不退差价。星际客改装盲盒、各公司的固定价设备不受影响。\n关闭：订货价不看事件（以前的做法）。", "You pay the price at the time of ordering; nothing is refunded when the event ends. The Spacer grab box and fixed-price machines are not affected.\nOff: order prices ignore events (the old behaviour).")), false);
~~~

~~~csharp
0258: 			return num2.ToString();
0259: 		})).ToArray(), (Func<string>)(() => L.T("超出的人顺延到第二天：剧情优先，其次是最久没来的。花钱请来的客流、贵宾、实验室访客不算在内。", "Extra visitors are pushed to the next day: story first, then whoever has waited longest. Paid foot traffic, VIPs and lab buyers don't count.")), false);
0260: 		_limits = config.Define("OrderLimits", false, "订货 / Orders", (Func<string>)(() => L.T("订货每周限量", "Weekly order limits")), (Func<string>)(() => L.T("打开后，各公司电话订货的每样货每周限量，按一份的基础价值分阶：\n", "When on, each item ordered by phone has a weekly limit, tiered by its base value:\n") + L.T("50₵ 以下 20 份，200₵ 以下 10 份，300₵ 以下 5 份，300₵ 及以上 3 份。\n", "under 50₵: 20, under 200₵: 10, under 300₵: 5, 300₵ and up: 3.\n") + L.T("海明威限量包装、免疫宁整箱这些独家货不管开不开都按原来的限量。", "Exclusives like Hemingway Limited Edition and the Immunivax case keep their own limits either way.")), false);
0261: 		_orderEvents = config.Define("OrderEvents", true, "订货 / Orders", (Func<string>)(() => L.T("事件影响订货", "Events affect orders")), (Func<string>)(() => L.T("打开后，原版市场事件（某类物品涨价、降价）也会影响电话订货的单价：事件让这类物品涨多少、跌多少，订货价就跟着涨跌多少，和店里同类物品的估价一致。\n", "When on, vanilla market events (a type of item going up or down in price) also change phone order prices: the order price rises or falls by as much as the event moves that type of item, matching how the shop values it.\n") + L.T("下单时按当时的价格结算，之后事件结束也不退差价。星际客改装盲盒、各公司的固定价设备不受影响。\n关闭：订货价不看事件（以前的做法）。", "You pay the price at the time of ordering; nothing is refunded when the event ends. The Spacer grab box and fixed-price machines are not affected.\nOff: order prices ignore events (the old behaviour).")), false);
0262: 		_orderFactor = config.Define("OrderMarkup", "1.1", "订货 / Orders", (Func<string>)(() => L.T("订购溢价（倍）", "Order markup (x)")), Factors, (Func<string>)(() => L.T("电话订货的价格 = 物品价值 × 这个倍数（默认 1.1）。合作档位越高每档便宜 0.05：签约 ×1.1、长期客户 ×1.05、合作伙伴 ×1.0（按默认值）。\n子午线高端配件、利维坦也按这个倍数（不看档位）。星际客改装盲盒固定 500₵。", "Phone order price = item value × this (default 1.1). Each higher cooperation tier is 0.05 cheaper: Signed ×1.1, Regular ×1.05, Partner ×1.0 (at the default).\nMeridian's high-end attachments and the Leviathan use this multiplier too (regardless of tier). The Spacer grab box is a fixed 500₵.")), false);
~~~

~~~csharp
0259: 		})).ToArray(), (Func<string>)(() => L.T("超出的人顺延到第二天：剧情优先，其次是最久没来的。花钱请来的客流、贵宾、实验室访客不算在内。", "Extra visitors are pushed to the next day: story first, then whoever has waited longest. Paid foot traffic, VIPs and lab buyers don't count.")), false);
0260: 		_limits = config.Define("OrderLimits", false, "订货 / Orders", (Func<string>)(() => L.T("订货每周限量", "Weekly order limits")), (Func<string>)(() => L.T("打开后，各公司电话订货的每样货每周限量，按一份的基础价值分阶：\n", "When on, each item ordered by phone has a weekly limit, tiered by its base value:\n") + L.T("50₵ 以下 20 份，200₵ 以下 10 份，300₵ 以下 5 份，300₵ 及以上 3 份。\n", "under 50₵: 20, under 200₵: 10, under 300₵: 5, 300₵ and up: 3.\n") + L.T("海明威限量包装、免疫宁整箱这些独家货不管开不开都按原来的限量。", "Exclusives like Hemingway Limited Edition and the Immunivax case keep their own limits either way.")), false);
0261: 		_orderEvents = config.Define("OrderEvents", true, "订货 / Orders", (Func<string>)(() => L.T("事件影响订货", "Events affect orders")), (Func<string>)(() => L.T("打开后，原版市场事件（某类物品涨价、降价）也会影响电话订货的单价：事件让这类物品涨多少、跌多少，订货价就跟着涨跌多少，和店里同类物品的估价一致。\n", "When on, vanilla market events (a type of item going up or down in price) also change phone order prices: the order price rises or falls by as much as the event moves that type of item, matching how the shop values it.\n") + L.T("下单时按当时的价格结算，之后事件结束也不退差价。星际客改装盲盒、各公司的固定价设备不受影响。\n关闭：订货价不看事件（以前的做法）。", "You pay the price at the time of ordering; nothing is refunded when the event ends. The Spacer grab box and fixed-price machines are not affected.\nOff: order prices ignore events (the old behaviour).")), false);
0262: 		_orderFactor = config.Define("OrderMarkup", "1.1", "订货 / Orders", (Func<string>)(() => L.T("订购溢价（倍）", "Order markup (x)")), Factors, (Func<string>)(() => L.T("电话订货的价格 = 物品价值 × 这个倍数（默认 1.1）。合作档位越高每档便宜 0.05：签约 ×1.1、长期客户 ×1.05、合作伙伴 ×1.0（按默认值）。\n子午线高端配件、利维坦也按这个倍数（不看档位）。星际客改装盲盒固定 500₵。", "Phone order price = item value × this (default 1.1). Each higher cooperation tier is 0.05 cheaper: Signed ×1.1, Regular ×1.05, Partner ×1.0 (at the default).\nMeridian's high-end attachments and the Leviathan use this multiplier too (regardless of tier). The Spacer grab box is a fixed 500₵.")), false);
0263: 		_budget = config.Define("AgentBudget", "10000", "来访 / Visits", (Func<string>)(() => L.T("公司人物携带信用点", "Company character credits")), Budgets, (Func<string>)(() => L.T("公司人物来收货时最多花多少信用点（六位人物、索拉纳采购员）。\n动态：800 + 随机 0～250 + 当前天数，参照原版最有钱的顾客（750～1000 + 天数）。\n索拉纳贵宾带多少钱看“服务强度”。", "The most a company character will spend when buying from you (the six characters and the Solana lab buyer).\nDynamic: 800 + random 0–250 + the current day, based on the richest vanilla customers (750–1000 + day).\nHow much Solana's VIPs bring depends on “Service strength”.")), false);
~~~

~~~csharp
0260: 		_limits = config.Define("OrderLimits", false, "订货 / Orders", (Func<string>)(() => L.T("订货每周限量", "Weekly order limits")), (Func<string>)(() => L.T("打开后，各公司电话订货的每样货每周限量，按一份的基础价值分阶：\n", "When on, each item ordered by phone has a weekly limit, tiered by its base value:\n") + L.T("50₵ 以下 20 份，200₵ 以下 10 份，300₵ 以下 5 份，300₵ 及以上 3 份。\n", "under 50₵: 20, under 200₵: 10, under 300₵: 5, 300₵ and up: 3.\n") + L.T("海明威限量包装、免疫宁整箱这些独家货不管开不开都按原来的限量。", "Exclusives like Hemingway Limited Edition and the Immunivax case keep their own limits either way.")), false);
0261: 		_orderEvents = config.Define("OrderEvents", true, "订货 / Orders", (Func<string>)(() => L.T("事件影响订货", "Events affect orders")), (Func<string>)(() => L.T("打开后，原版市场事件（某类物品涨价、降价）也会影响电话订货的单价：事件让这类物品涨多少、跌多少，订货价就跟着涨跌多少，和店里同类物品的估价一致。\n", "When on, vanilla market events (a type of item going up or down in price) also change phone order prices: the order price rises or falls by as much as the event moves that type of item, matching how the shop values it.\n") + L.T("下单时按当时的价格结算，之后事件结束也不退差价。星际客改装盲盒、各公司的固定价设备不受影响。\n关闭：订货价不看事件（以前的做法）。", "You pay the price at the time of ordering; nothing is refunded when the event ends. The Spacer grab box and fixed-price machines are not affected.\nOff: order prices ignore events (the old behaviour).")), false);
0262: 		_orderFactor = config.Define("OrderMarkup", "1.1", "订货 / Orders", (Func<string>)(() => L.T("订购溢价（倍）", "Order markup (x)")), Factors, (Func<string>)(() => L.T("电话订货的价格 = 物品价值 × 这个倍数（默认 1.1）。合作档位越高每档便宜 0.05：签约 ×1.1、长期客户 ×1.05、合作伙伴 ×1.0（按默认值）。\n子午线高端配件、利维坦也按这个倍数（不看档位）。星际客改装盲盒固定 500₵。", "Phone order price = item value × this (default 1.1). Each higher cooperation tier is 0.05 cheaper: Signed ×1.1, Regular ×1.05, Partner ×1.0 (at the default).\nMeridian's high-end attachments and the Leviathan use this multiplier too (regardless of tier). The Spacer grab box is a fixed 500₵.")), false);
0263: 		_budget = config.Define("AgentBudget", "10000", "来访 / Visits", (Func<string>)(() => L.T("公司人物携带信用点", "Company character credits")), Budgets, (Func<string>)(() => L.T("公司人物来收货时最多花多少信用点（六位人物、索拉纳采购员）。\n动态：800 + 随机 0～250 + 当前天数，参照原版最有钱的顾客（750～1000 + 天数）。\n索拉纳贵宾带多少钱看“服务强度”。", "The most a company character will spend when buying from you (the six characters and the Solana lab buyer).\nDynamic: 800 + random 0–250 + the current day, based on the richest vanilla customers (750–1000 + day).\nHow much Solana's VIPs bring depends on “Service strength”.")), false);
0264: 		_services = config.Define("ServiceStrength", "标准 / Standard", "来访 / Visits", (Func<string>)(() => L.T("服务强度", "Service strength")), Strengths, (Func<string>)(() => L.T("标准：现在的数值。\n原版：按原版数值校准——\n· 索拉纳贵宾带 800 + 随机 0～250 + 天数（标准是 3000～5000）；原版最有钱的顾客带 750～1000 + 天数；\n· 实验室采购员收单上的货一律 1.5 倍（标准：2 倍，蓝血 3 倍）；原版涨价事件最常见 +50%；\n· 体力增强剂累计最多 +2 次拾荒（标准：每针 +1，不封顶）；原版开局特长“熟练拾荒者”是 +2；\n· 罗森批发八折（标准：七折）；原版供货商八折。", "Standard: the current values.\nVanilla: tuned to the base game—\n· Solana VIPs bring 800 + random 0–250 + the day (Standard: 3000–5000); the richest vanilla customers bring 750–1000 + day;\n· the lab buyer pays 1.5x for everything on the list (Standard: 2x, Blue Blood 3x); vanilla price events are most often +50%;\n· Stamina Boosters add at most +2 scavenging attempts in total (Standard: +1 per shot, no cap); the vanilla Proficient Scavenger perk gives +2;\n· Rosen sells wholesale at 20% off (Standard: 30%); vanilla suppliers give 20% off.")), false);
~~~

~~~csharp
0261: 		_orderEvents = config.Define("OrderEvents", true, "订货 / Orders", (Func<string>)(() => L.T("事件影响订货", "Events affect orders")), (Func<string>)(() => L.T("打开后，原版市场事件（某类物品涨价、降价）也会影响电话订货的单价：事件让这类物品涨多少、跌多少，订货价就跟着涨跌多少，和店里同类物品的估价一致。\n", "When on, vanilla market events (a type of item going up or down in price) also change phone order prices: the order price rises or falls by as much as the event moves that type of item, matching how the shop values it.\n") + L.T("下单时按当时的价格结算，之后事件结束也不退差价。星际客改装盲盒、各公司的固定价设备不受影响。\n关闭：订货价不看事件（以前的做法）。", "You pay the price at the time of ordering; nothing is refunded when the event ends. The Spacer grab box and fixed-price machines are not affected.\nOff: order prices ignore events (the old behaviour).")), false);
0262: 		_orderFactor = config.Define("OrderMarkup", "1.1", "订货 / Orders", (Func<string>)(() => L.T("订购溢价（倍）", "Order markup (x)")), Factors, (Func<string>)(() => L.T("电话订货的价格 = 物品价值 × 这个倍数（默认 1.1）。合作档位越高每档便宜 0.05：签约 ×1.1、长期客户 ×1.05、合作伙伴 ×1.0（按默认值）。\n子午线高端配件、利维坦也按这个倍数（不看档位）。星际客改装盲盒固定 500₵。", "Phone order price = item value × this (default 1.1). Each higher cooperation tier is 0.05 cheaper: Signed ×1.1, Regular ×1.05, Partner ×1.0 (at the default).\nMeridian's high-end attachments and the Leviathan use this multiplier too (regardless of tier). The Spacer grab box is a fixed 500₵.")), false);
0263: 		_budget = config.Define("AgentBudget", "10000", "来访 / Visits", (Func<string>)(() => L.T("公司人物携带信用点", "Company character credits")), Budgets, (Func<string>)(() => L.T("公司人物来收货时最多花多少信用点（六位人物、索拉纳采购员）。\n动态：800 + 随机 0～250 + 当前天数，参照原版最有钱的顾客（750～1000 + 天数）。\n索拉纳贵宾带多少钱看“服务强度”。", "The most a company character will spend when buying from you (the six characters and the Solana lab buyer).\nDynamic: 800 + random 0–250 + the current day, based on the richest vanilla customers (750–1000 + day).\nHow much Solana's VIPs bring depends on “Service strength”.")), false);
0264: 		_services = config.Define("ServiceStrength", "标准 / Standard", "来访 / Visits", (Func<string>)(() => L.T("服务强度", "Service strength")), Strengths, (Func<string>)(() => L.T("标准：现在的数值。\n原版：按原版数值校准——\n· 索拉纳贵宾带 800 + 随机 0～250 + 天数（标准是 3000～5000）；原版最有钱的顾客带 750～1000 + 天数；\n· 实验室采购员收单上的货一律 1.5 倍（标准：2 倍，蓝血 3 倍）；原版涨价事件最常见 +50%；\n· 体力增强剂累计最多 +2 次拾荒（标准：每针 +1，不封顶）；原版开局特长“熟练拾荒者”是 +2；\n· 罗森批发八折（标准：七折）；原版供货商八折。", "Standard: the current values.\nVanilla: tuned to the base game—\n· Solana VIPs bring 800 + random 0–250 + the day (Standard: 3000–5000); the richest vanilla customers bring 750–1000 + day;\n· the lab buyer pays 1.5x for everything on the list (Standard: 2x, Blue Blood 3x); vanilla price events are most often +50%;\n· Stamina Boosters add at most +2 scavenging attempts in total (Standard: +1 per shot, no cap); the vanilla Proficient Scavenger perk gives +2;\n· Rosen sells wholesale at 20% off (Standard: 30%); vanilla suppliers give 20% off.")), false);
0265: 		_buyPrices = config.Define("BuyPriceStrength", "标准 / Standard", "来访 / Visits", (Func<string>)(() => L.T("人物收货价", "Character buying prices")), Strengths, (Func<string>)(() => L.T("标准：现在的数值。\n原版：日常收货不加价，特殊需求最多 1.5 倍（原版顾客按成交价付钱，涨价事件最常见 +50%）——\n· 米洛收海明威假烟 2 倍 → 1.5 倍；\n· 瑟琳日常收货 1.2 倍 → 1 倍，收蓝血 3 倍 → 1.5 倍；\n· 伊莎收赛纳普斯 1.5 倍、瑟琳章节里收奢侈品 1.3 倍、多琳收海德拉 400₵ 不变；伊莎、罗森日常收货本来就不加价。", "Standard: the current values.\nVanilla: no premium on everyday purchases, at most 1.5x for special demand (vanilla customers pay the negotiated price; price events are most often +50%)—\n· Milo's fake Hemingways: 2x → 1.5x;\n· Seline's everyday purchases: 1.2x → 1x, Blue Blood: 3x → 1.5x;\n· unchanged: Isa's Synapse 1.5x, Seline's story luxury purchases 1.3x, Doreen's Hydra 400₵; Isa's and Rosen's everyday purchases never had a premium.")), false);
~~~

~~~csharp
0262: 		_orderFactor = config.Define("OrderMarkup", "1.1", "订货 / Orders", (Func<string>)(() => L.T("订购溢价（倍）", "Order markup (x)")), Factors, (Func<string>)(() => L.T("电话订货的价格 = 物品价值 × 这个倍数（默认 1.1）。合作档位越高每档便宜 0.05：签约 ×1.1、长期客户 ×1.05、合作伙伴 ×1.0（按默认值）。\n子午线高端配件、利维坦也按这个倍数（不看档位）。星际客改装盲盒固定 500₵。", "Phone order price = item value × this (default 1.1). Each higher cooperation tier is 0.05 cheaper: Signed ×1.1, Regular ×1.05, Partner ×1.0 (at the default).\nMeridian's high-end attachments and the Leviathan use this multiplier too (regardless of tier). The Spacer grab box is a fixed 500₵.")), false);
0263: 		_budget = config.Define("AgentBudget", "10000", "来访 / Visits", (Func<string>)(() => L.T("公司人物携带信用点", "Company character credits")), Budgets, (Func<string>)(() => L.T("公司人物来收货时最多花多少信用点（六位人物、索拉纳采购员）。\n动态：800 + 随机 0～250 + 当前天数，参照原版最有钱的顾客（750～1000 + 天数）。\n索拉纳贵宾带多少钱看“服务强度”。", "The most a company character will spend when buying from you (the six characters and the Solana lab buyer).\nDynamic: 800 + random 0–250 + the current day, based on the richest vanilla customers (750–1000 + day).\nHow much Solana's VIPs bring depends on “Service strength”.")), false);
0264: 		_services = config.Define("ServiceStrength", "标准 / Standard", "来访 / Visits", (Func<string>)(() => L.T("服务强度", "Service strength")), Strengths, (Func<string>)(() => L.T("标准：现在的数值。\n原版：按原版数值校准——\n· 索拉纳贵宾带 800 + 随机 0～250 + 天数（标准是 3000～5000）；原版最有钱的顾客带 750～1000 + 天数；\n· 实验室采购员收单上的货一律 1.5 倍（标准：2 倍，蓝血 3 倍）；原版涨价事件最常见 +50%；\n· 体力增强剂累计最多 +2 次拾荒（标准：每针 +1，不封顶）；原版开局特长“熟练拾荒者”是 +2；\n· 罗森批发八折（标准：七折）；原版供货商八折。", "Standard: the current values.\nVanilla: tuned to the base game—\n· Solana VIPs bring 800 + random 0–250 + the day (Standard: 3000–5000); the richest vanilla customers bring 750–1000 + day;\n· the lab buyer pays 1.5x for everything on the list (Standard: 2x, Blue Blood 3x); vanilla price events are most often +50%;\n· Stamina Boosters add at most +2 scavenging attempts in total (Standard: +1 per shot, no cap); the vanilla Proficient Scavenger perk gives +2;\n· Rosen sells wholesale at 20% off (Standard: 30%); vanilla suppliers give 20% off.")), false);
0265: 		_buyPrices = config.Define("BuyPriceStrength", "标准 / Standard", "来访 / Visits", (Func<string>)(() => L.T("人物收货价", "Character buying prices")), Strengths, (Func<string>)(() => L.T("标准：现在的数值。\n原版：日常收货不加价，特殊需求最多 1.5 倍（原版顾客按成交价付钱，涨价事件最常见 +50%）——\n· 米洛收海明威假烟 2 倍 → 1.5 倍；\n· 瑟琳日常收货 1.2 倍 → 1 倍，收蓝血 3 倍 → 1.5 倍；\n· 伊莎收赛纳普斯 1.5 倍、瑟琳章节里收奢侈品 1.3 倍、多琳收海德拉 400₵ 不变；伊莎、罗森日常收货本来就不加价。", "Standard: the current values.\nVanilla: no premium on everyday purchases, at most 1.5x for special demand (vanilla customers pay the negotiated price; price events are most often +50%)—\n· Milo's fake Hemingways: 2x → 1.5x;\n· Seline's everyday purchases: 1.2x → 1x, Blue Blood: 3x → 1.5x;\n· unchanged: Isa's Synapse 1.5x, Seline's story luxury purchases 1.3x, Doreen's Hydra 400₵; Isa's and Rosen's everyday purchases never had a premium.")), false);
0266: 		_malice = config.Define("MaliceMode", false, "难度 / Difficulty", (Func<string>)(() => L.T("恶意巨企模式", "Corporate Malice mode")), (Func<string>)(() => L.T("打开后：巨企的订货要按周订阅，服务全面涨价，合作档位的货要花钱申请解锁；星际客联盟拆成商人联合和反抗军；巨企使者撒该会找上门。\n建议从新档开始。中途打开：合作档位保留，已解锁的货要重新申请。中途关闭（第二天开店生效）：两边并回星际客联盟，订阅、代金券、助力点、撒该的账全部清空。", "When on: corporate phone orders need a weekly subscription, services cost more, and tier goods must be unlocked by paid application; the Spacer Alliance splits into the Merchant Union and the Resistance; the corporate envoy Zacchaeus comes calling.\nStarting a new save is recommended. Turned on mid-game: cooperation tiers are kept, but unlocked goods must be applied for again. Turned off mid-game (from the next day's opening): both sides merge back into the Spacer Alliance, and subscriptions, vouchers, support points and Zacchaeus's accounts are cleared.")), false);
~~~

~~~csharp
0263: 		_budget = config.Define("AgentBudget", "10000", "来访 / Visits", (Func<string>)(() => L.T("公司人物携带信用点", "Company character credits")), Budgets, (Func<string>)(() => L.T("公司人物来收货时最多花多少信用点（六位人物、索拉纳采购员）。\n动态：800 + 随机 0～250 + 当前天数，参照原版最有钱的顾客（750～1000 + 天数）。\n索拉纳贵宾带多少钱看“服务强度”。", "The most a company character will spend when buying from you (the six characters and the Solana lab buyer).\nDynamic: 800 + random 0–250 + the current day, based on the richest vanilla customers (750–1000 + day).\nHow much Solana's VIPs bring depends on “Service strength”.")), false);
0264: 		_services = config.Define("ServiceStrength", "标准 / Standard", "来访 / Visits", (Func<string>)(() => L.T("服务强度", "Service strength")), Strengths, (Func<string>)(() => L.T("标准：现在的数值。\n原版：按原版数值校准——\n· 索拉纳贵宾带 800 + 随机 0～250 + 天数（标准是 3000～5000）；原版最有钱的顾客带 750～1000 + 天数；\n· 实验室采购员收单上的货一律 1.5 倍（标准：2 倍，蓝血 3 倍）；原版涨价事件最常见 +50%；\n· 体力增强剂累计最多 +2 次拾荒（标准：每针 +1，不封顶）；原版开局特长“熟练拾荒者”是 +2；\n· 罗森批发八折（标准：七折）；原版供货商八折。", "Standard: the current values.\nVanilla: tuned to the base game—\n· Solana VIPs bring 800 + random 0–250 + the day (Standard: 3000–5000); the richest vanilla customers bring 750–1000 + day;\n· the lab buyer pays 1.5x for everything on the list (Standard: 2x, Blue Blood 3x); vanilla price events are most often +50%;\n· Stamina Boosters add at most +2 scavenging attempts in total (Standard: +1 per shot, no cap); the vanilla Proficient Scavenger perk gives +2;\n· Rosen sells wholesale at 20% off (Standard: 30%); vanilla suppliers give 20% off.")), false);
0265: 		_buyPrices = config.Define("BuyPriceStrength", "标准 / Standard", "来访 / Visits", (Func<string>)(() => L.T("人物收货价", "Character buying prices")), Strengths, (Func<string>)(() => L.T("标准：现在的数值。\n原版：日常收货不加价，特殊需求最多 1.5 倍（原版顾客按成交价付钱，涨价事件最常见 +50%）——\n· 米洛收海明威假烟 2 倍 → 1.5 倍；\n· 瑟琳日常收货 1.2 倍 → 1 倍，收蓝血 3 倍 → 1.5 倍；\n· 伊莎收赛纳普斯 1.5 倍、瑟琳章节里收奢侈品 1.3 倍、多琳收海德拉 400₵ 不变；伊莎、罗森日常收货本来就不加价。", "Standard: the current values.\nVanilla: no premium on everyday purchases, at most 1.5x for special demand (vanilla customers pay the negotiated price; price events are most often +50%)—\n· Milo's fake Hemingways: 2x → 1.5x;\n· Seline's everyday purchases: 1.2x → 1x, Blue Blood: 3x → 1.5x;\n· unchanged: Isa's Synapse 1.5x, Seline's story luxury purchases 1.3x, Doreen's Hydra 400₵; Isa's and Rosen's everyday purchases never had a premium.")), false);
0266: 		_malice = config.Define("MaliceMode", false, "难度 / Difficulty", (Func<string>)(() => L.T("恶意巨企模式", "Corporate Malice mode")), (Func<string>)(() => L.T("打开后：巨企的订货要按周订阅，服务全面涨价，合作档位的货要花钱申请解锁；星际客联盟拆成商人联合和反抗军；巨企使者撒该会找上门。\n建议从新档开始。中途打开：合作档位保留，已解锁的货要重新申请。中途关闭（第二天开店生效）：两边并回星际客联盟，订阅、代金券、助力点、撒该的账全部清空。", "When on: corporate phone orders need a weekly subscription, services cost more, and tier goods must be unlocked by paid application; the Spacer Alliance splits into the Merchant Union and the Resistance; the corporate envoy Zacchaeus comes calling.\nStarting a new save is recommended. Turned on mid-game: cooperation tiers are kept, but unlocked goods must be applied for again. Turned off mid-game (from the next day's opening): both sides merge back into the Spacer Alliance, and subscriptions, vouchers, support points and Zacchaeus's accounts are cleared.")), false);
0267: 		_coopCap = config.Define("CoopDailyCap", true, "合作度 / Cooperation", (Func<string>)(() => L.T("合作度每天上限", "Daily cooperation cap")), (Func<string>)(() => L.T("打开：每家公司的合作度（星际客联盟叫信任度）每天最多涨 ", "On: each company's cooperation (Trust for the Spacer Alliance) rises by at most ") + 10 + L.T(" 点，超出的不累计。\n", " per day; the excess is lost.\n") + L.T("买卖、订货、服务、声望上涨都算在内；剧情推进的奖励和剧情保底不受限制。\n", "Trades, orders, services and reputation gains all count; story rewards and story minimums are not capped.\n") + L.T("关闭：不设上限。", "Off: no cap.")), false);
~~~

~~~csharp
0264: 		_services = config.Define("ServiceStrength", "标准 / Standard", "来访 / Visits", (Func<string>)(() => L.T("服务强度", "Service strength")), Strengths, (Func<string>)(() => L.T("标准：现在的数值。\n原版：按原版数值校准——\n· 索拉纳贵宾带 800 + 随机 0～250 + 天数（标准是 3000～5000）；原版最有钱的顾客带 750～1000 + 天数；\n· 实验室采购员收单上的货一律 1.5 倍（标准：2 倍，蓝血 3 倍）；原版涨价事件最常见 +50%；\n· 体力增强剂累计最多 +2 次拾荒（标准：每针 +1，不封顶）；原版开局特长“熟练拾荒者”是 +2；\n· 罗森批发八折（标准：七折）；原版供货商八折。", "Standard: the current values.\nVanilla: tuned to the base game—\n· Solana VIPs bring 800 + random 0–250 + the day (Standard: 3000–5000); the richest vanilla customers bring 750–1000 + day;\n· the lab buyer pays 1.5x for everything on the list (Standard: 2x, Blue Blood 3x); vanilla price events are most often +50%;\n· Stamina Boosters add at most +2 scavenging attempts in total (Standard: +1 per shot, no cap); the vanilla Proficient Scavenger perk gives +2;\n· Rosen sells wholesale at 20% off (Standard: 30%); vanilla suppliers give 20% off.")), false);
0265: 		_buyPrices = config.Define("BuyPriceStrength", "标准 / Standard", "来访 / Visits", (Func<string>)(() => L.T("人物收货价", "Character buying prices")), Strengths, (Func<string>)(() => L.T("标准：现在的数值。\n原版：日常收货不加价，特殊需求最多 1.5 倍（原版顾客按成交价付钱，涨价事件最常见 +50%）——\n· 米洛收海明威假烟 2 倍 → 1.5 倍；\n· 瑟琳日常收货 1.2 倍 → 1 倍，收蓝血 3 倍 → 1.5 倍；\n· 伊莎收赛纳普斯 1.5 倍、瑟琳章节里收奢侈品 1.3 倍、多琳收海德拉 400₵ 不变；伊莎、罗森日常收货本来就不加价。", "Standard: the current values.\nVanilla: no premium on everyday purchases, at most 1.5x for special demand (vanilla customers pay the negotiated price; price events are most often +50%)—\n· Milo's fake Hemingways: 2x → 1.5x;\n· Seline's everyday purchases: 1.2x → 1x, Blue Blood: 3x → 1.5x;\n· unchanged: Isa's Synapse 1.5x, Seline's story luxury purchases 1.3x, Doreen's Hydra 400₵; Isa's and Rosen's everyday purchases never had a premium.")), false);
0266: 		_malice = config.Define("MaliceMode", false, "难度 / Difficulty", (Func<string>)(() => L.T("恶意巨企模式", "Corporate Malice mode")), (Func<string>)(() => L.T("打开后：巨企的订货要按周订阅，服务全面涨价，合作档位的货要花钱申请解锁；星际客联盟拆成商人联合和反抗军；巨企使者撒该会找上门。\n建议从新档开始。中途打开：合作档位保留，已解锁的货要重新申请。中途关闭（第二天开店生效）：两边并回星际客联盟，订阅、代金券、助力点、撒该的账全部清空。", "When on: corporate phone orders need a weekly subscription, services cost more, and tier goods must be unlocked by paid application; the Spacer Alliance splits into the Merchant Union and the Resistance; the corporate envoy Zacchaeus comes calling.\nStarting a new save is recommended. Turned on mid-game: cooperation tiers are kept, but unlocked goods must be applied for again. Turned off mid-game (from the next day's opening): both sides merge back into the Spacer Alliance, and subscriptions, vouchers, support points and Zacchaeus's accounts are cleared.")), false);
0267: 		_coopCap = config.Define("CoopDailyCap", true, "合作度 / Cooperation", (Func<string>)(() => L.T("合作度每天上限", "Daily cooperation cap")), (Func<string>)(() => L.T("打开：每家公司的合作度（星际客联盟叫信任度）每天最多涨 ", "On: each company's cooperation (Trust for the Spacer Alliance) rises by at most ") + 10 + L.T(" 点，超出的不累计。\n", " per day; the excess is lost.\n") + L.T("买卖、订货、服务、声望上涨都算在内；剧情推进的奖励和剧情保底不受限制。\n", "Trades, orders, services and reputation gains all count; story rewards and story minimums are not capped.\n") + L.T("关闭：不设上限。", "Off: no cap.")), false);
0268: 		_coopCredits = config.Define("CoopCredits", "100", "合作度 / Cooperation", (Func<string>)(() => L.T("合作度：每多少信用点涨 1 点", "Cooperation: credits per point")), CoopRates, (Func<string>)(() => L.T("和公司的人买卖、电话订货（含利维坦）、帕斯泰尔加盟费、子午线翻新费，每这么多信用点涨 1 点合作度（默认 100）；卖出本公司产品按 3 倍算（默认每 300₵ 涨 1 点）。\n原版势力声望是每 400₵ 涨 1 点。剧情奖励和星际客的声望折算不受影响。", "Trading with company people, phone orders (including the Leviathan), the pastel dealer fee and Meridian refurbishment fees give 1 cooperation point per this many credits (default 100); selling the company's products counts at 3x that (default 1 point per 300₵).\nVanilla faction reputation rises 1 point per 400₵. Story rewards and the Spacer reputation conversion are not affected.")), false);
~~~

~~~csharp
0265: 		_buyPrices = config.Define("BuyPriceStrength", "标准 / Standard", "来访 / Visits", (Func<string>)(() => L.T("人物收货价", "Character buying prices")), Strengths, (Func<string>)(() => L.T("标准：现在的数值。\n原版：日常收货不加价，特殊需求最多 1.5 倍（原版顾客按成交价付钱，涨价事件最常见 +50%）——\n· 米洛收海明威假烟 2 倍 → 1.5 倍；\n· 瑟琳日常收货 1.2 倍 → 1 倍，收蓝血 3 倍 → 1.5 倍；\n· 伊莎收赛纳普斯 1.5 倍、瑟琳章节里收奢侈品 1.3 倍、多琳收海德拉 400₵ 不变；伊莎、罗森日常收货本来就不加价。", "Standard: the current values.\nVanilla: no premium on everyday purchases, at most 1.5x for special demand (vanilla customers pay the negotiated price; price events are most often +50%)—\n· Milo's fake Hemingways: 2x → 1.5x;\n· Seline's everyday purchases: 1.2x → 1x, Blue Blood: 3x → 1.5x;\n· unchanged: Isa's Synapse 1.5x, Seline's story luxury purchases 1.3x, Doreen's Hydra 400₵; Isa's and Rosen's everyday purchases never had a premium.")), false);
0266: 		_malice = config.Define("MaliceMode", false, "难度 / Difficulty", (Func<string>)(() => L.T("恶意巨企模式", "Corporate Malice mode")), (Func<string>)(() => L.T("打开后：巨企的订货要按周订阅，服务全面涨价，合作档位的货要花钱申请解锁；星际客联盟拆成商人联合和反抗军；巨企使者撒该会找上门。\n建议从新档开始。中途打开：合作档位保留，已解锁的货要重新申请。中途关闭（第二天开店生效）：两边并回星际客联盟，订阅、代金券、助力点、撒该的账全部清空。", "When on: corporate phone orders need a weekly subscription, services cost more, and tier goods must be unlocked by paid application; the Spacer Alliance splits into the Merchant Union and the Resistance; the corporate envoy Zacchaeus comes calling.\nStarting a new save is recommended. Turned on mid-game: cooperation tiers are kept, but unlocked goods must be applied for again. Turned off mid-game (from the next day's opening): both sides merge back into the Spacer Alliance, and subscriptions, vouchers, support points and Zacchaeus's accounts are cleared.")), false);
0267: 		_coopCap = config.Define("CoopDailyCap", true, "合作度 / Cooperation", (Func<string>)(() => L.T("合作度每天上限", "Daily cooperation cap")), (Func<string>)(() => L.T("打开：每家公司的合作度（星际客联盟叫信任度）每天最多涨 ", "On: each company's cooperation (Trust for the Spacer Alliance) rises by at most ") + 10 + L.T(" 点，超出的不累计。\n", " per day; the excess is lost.\n") + L.T("买卖、订货、服务、声望上涨都算在内；剧情推进的奖励和剧情保底不受限制。\n", "Trades, orders, services and reputation gains all count; story rewards and story minimums are not capped.\n") + L.T("关闭：不设上限。", "Off: no cap.")), false);
0268: 		_coopCredits = config.Define("CoopCredits", "100", "合作度 / Cooperation", (Func<string>)(() => L.T("合作度：每多少信用点涨 1 点", "Cooperation: credits per point")), CoopRates, (Func<string>)(() => L.T("和公司的人买卖、电话订货（含利维坦）、帕斯泰尔加盟费、子午线翻新费，每这么多信用点涨 1 点合作度（默认 100）；卖出本公司产品按 3 倍算（默认每 300₵ 涨 1 点）。\n原版势力声望是每 400₵ 涨 1 点。剧情奖励和星际客的声望折算不受影响。", "Trading with company people, phone orders (including the Leviathan), the pastel dealer fee and Meridian refurbishment fees give 1 cooperation point per this many credits (default 100); selling the company's products counts at 3x that (default 1 point per 300₵).\nVanilla faction reputation rises 1 point per 400₵. Story rewards and the Spacer reputation conversion are not affected.")), false);
0269: 		_panelScale = config.Define("PanelScale", "1.5", "画面 / Display", (Func<string>)(() => L.T("电话服务界面大小（倍）", "Phone service window size (x)")), Scales, (Func<string>)(() => L.T("公司电话打开的服务界面放大几倍。1 = 以前的大小。屏幕放不下时会自动缩小到刚好放得下。", "How much to enlarge the service window opened from company phone calls. 1 = the old size. Shrinks automatically if it doesn't fit the screen.")), false);
~~~

~~~csharp
0266: 		_malice = config.Define("MaliceMode", false, "难度 / Difficulty", (Func<string>)(() => L.T("恶意巨企模式", "Corporate Malice mode")), (Func<string>)(() => L.T("打开后：巨企的订货要按周订阅，服务全面涨价，合作档位的货要花钱申请解锁；星际客联盟拆成商人联合和反抗军；巨企使者撒该会找上门。\n建议从新档开始。中途打开：合作档位保留，已解锁的货要重新申请。中途关闭（第二天开店生效）：两边并回星际客联盟，订阅、代金券、助力点、撒该的账全部清空。", "When on: corporate phone orders need a weekly subscription, services cost more, and tier goods must be unlocked by paid application; the Spacer Alliance splits into the Merchant Union and the Resistance; the corporate envoy Zacchaeus comes calling.\nStarting a new save is recommended. Turned on mid-game: cooperation tiers are kept, but unlocked goods must be applied for again. Turned off mid-game (from the next day's opening): both sides merge back into the Spacer Alliance, and subscriptions, vouchers, support points and Zacchaeus's accounts are cleared.")), false);
0267: 		_coopCap = config.Define("CoopDailyCap", true, "合作度 / Cooperation", (Func<string>)(() => L.T("合作度每天上限", "Daily cooperation cap")), (Func<string>)(() => L.T("打开：每家公司的合作度（星际客联盟叫信任度）每天最多涨 ", "On: each company's cooperation (Trust for the Spacer Alliance) rises by at most ") + 10 + L.T(" 点，超出的不累计。\n", " per day; the excess is lost.\n") + L.T("买卖、订货、服务、声望上涨都算在内；剧情推进的奖励和剧情保底不受限制。\n", "Trades, orders, services and reputation gains all count; story rewards and story minimums are not capped.\n") + L.T("关闭：不设上限。", "Off: no cap.")), false);
0268: 		_coopCredits = config.Define("CoopCredits", "100", "合作度 / Cooperation", (Func<string>)(() => L.T("合作度：每多少信用点涨 1 点", "Cooperation: credits per point")), CoopRates, (Func<string>)(() => L.T("和公司的人买卖、电话订货（含利维坦）、帕斯泰尔加盟费、子午线翻新费，每这么多信用点涨 1 点合作度（默认 100）；卖出本公司产品按 3 倍算（默认每 300₵ 涨 1 点）。\n原版势力声望是每 400₵ 涨 1 点。剧情奖励和星际客的声望折算不受影响。", "Trading with company people, phone orders (including the Leviathan), the pastel dealer fee and Meridian refurbishment fees give 1 cooperation point per this many credits (default 100); selling the company's products counts at 3x that (default 1 point per 300₵).\nVanilla faction reputation rises 1 point per 400₵. Story rewards and the Spacer reputation conversion are not affected.")), false);
0269: 		_panelScale = config.Define("PanelScale", "1.5", "画面 / Display", (Func<string>)(() => L.T("电话服务界面大小（倍）", "Phone service window size (x)")), Scales, (Func<string>)(() => L.T("公司电话打开的服务界面放大几倍。1 = 以前的大小。屏幕放不下时会自动缩小到刚好放得下。", "How much to enlarge the service window opened from company phone calls. 1 = the old size. Shrinks automatically if it doesn't fit the screen.")), false);
0270: 		_cleanup = config.Define("CleanupForUninstall", false, "卸载 / Uninstall", (Func<string>)(() => L.T("卸载前清理", "Clean up before uninstalling")), (Func<string>)(() => L.T("要卸载本模组时打开，并同时勾选下面的“确认清理”。两项都打开后存一次档（睡觉或手动存档）：\n· 公司电话从电话簿和存档里移除；\n· 本模组的物品全部删除（名片、新物品、罗森的手作枪、利维坦等）。\n存完档就可以卸载。不卸载的话请保持关闭。", "Turn on when you want to uninstall, together with “Confirm cleanup” below. With both on, save once (sleep or save manually):\n· company phone numbers are removed from the phone book and the save;\n· every item from this mod is deleted (cards, new items, Rosen's guns, the Leviathan, etc.).\nThen you can uninstall. Keep it off if you're not uninstalling.")), false);
~~~

~~~csharp
0267: 		_coopCap = config.Define("CoopDailyCap", true, "合作度 / Cooperation", (Func<string>)(() => L.T("合作度每天上限", "Daily cooperation cap")), (Func<string>)(() => L.T("打开：每家公司的合作度（星际客联盟叫信任度）每天最多涨 ", "On: each company's cooperation (Trust for the Spacer Alliance) rises by at most ") + 10 + L.T(" 点，超出的不累计。\n", " per day; the excess is lost.\n") + L.T("买卖、订货、服务、声望上涨都算在内；剧情推进的奖励和剧情保底不受限制。\n", "Trades, orders, services and reputation gains all count; story rewards and story minimums are not capped.\n") + L.T("关闭：不设上限。", "Off: no cap.")), false);
0268: 		_coopCredits = config.Define("CoopCredits", "100", "合作度 / Cooperation", (Func<string>)(() => L.T("合作度：每多少信用点涨 1 点", "Cooperation: credits per point")), CoopRates, (Func<string>)(() => L.T("和公司的人买卖、电话订货（含利维坦）、帕斯泰尔加盟费、子午线翻新费，每这么多信用点涨 1 点合作度（默认 100）；卖出本公司产品按 3 倍算（默认每 300₵ 涨 1 点）。\n原版势力声望是每 400₵ 涨 1 点。剧情奖励和星际客的声望折算不受影响。", "Trading with company people, phone orders (including the Leviathan), the pastel dealer fee and Meridian refurbishment fees give 1 cooperation point per this many credits (default 100); selling the company's products counts at 3x that (default 1 point per 300₵).\nVanilla faction reputation rises 1 point per 400₵. Story rewards and the Spacer reputation conversion are not affected.")), false);
0269: 		_panelScale = config.Define("PanelScale", "1.5", "画面 / Display", (Func<string>)(() => L.T("电话服务界面大小（倍）", "Phone service window size (x)")), Scales, (Func<string>)(() => L.T("公司电话打开的服务界面放大几倍。1 = 以前的大小。屏幕放不下时会自动缩小到刚好放得下。", "How much to enlarge the service window opened from company phone calls. 1 = the old size. Shrinks automatically if it doesn't fit the screen.")), false);
0270: 		_cleanup = config.Define("CleanupForUninstall", false, "卸载 / Uninstall", (Func<string>)(() => L.T("卸载前清理", "Clean up before uninstalling")), (Func<string>)(() => L.T("要卸载本模组时打开，并同时勾选下面的“确认清理”。两项都打开后存一次档（睡觉或手动存档）：\n· 公司电话从电话簿和存档里移除；\n· 本模组的物品全部删除（名片、新物品、罗森的手作枪、利维坦等）。\n存完档就可以卸载。不卸载的话请保持关闭。", "Turn on when you want to uninstall, together with “Confirm cleanup” below. With both on, save once (sleep or save manually):\n· company phone numbers are removed from the phone book and the save;\n· every item from this mod is deleted (cards, new items, Rosen's guns, the Leviathan, etc.).\nThen you can uninstall. Keep it off if you're not uninstalling.")), false);
0271: 		_cleanupConfirm = config.Define("CleanupConfirm", false, "卸载 / Uninstall", (Func<string>)(() => L.T("确认清理（会删除本模组的物品）", "Confirm cleanup (deletes this mod's items)")), (Func<string>)(() => L.T("和“卸载前清理”一起打开才会生效。删除的物品无法恢复。", "Only takes effect together with “Clean up before uninstalling”. Deleted items cannot be restored.")), false);
~~~

~~~csharp
0268: 		_coopCredits = config.Define("CoopCredits", "100", "合作度 / Cooperation", (Func<string>)(() => L.T("合作度：每多少信用点涨 1 点", "Cooperation: credits per point")), CoopRates, (Func<string>)(() => L.T("和公司的人买卖、电话订货（含利维坦）、帕斯泰尔加盟费、子午线翻新费，每这么多信用点涨 1 点合作度（默认 100）；卖出本公司产品按 3 倍算（默认每 300₵ 涨 1 点）。\n原版势力声望是每 400₵ 涨 1 点。剧情奖励和星际客的声望折算不受影响。", "Trading with company people, phone orders (including the Leviathan), the pastel dealer fee and Meridian refurbishment fees give 1 cooperation point per this many credits (default 100); selling the company's products counts at 3x that (default 1 point per 300₵).\nVanilla faction reputation rises 1 point per 400₵. Story rewards and the Spacer reputation conversion are not affected.")), false);
0269: 		_panelScale = config.Define("PanelScale", "1.5", "画面 / Display", (Func<string>)(() => L.T("电话服务界面大小（倍）", "Phone service window size (x)")), Scales, (Func<string>)(() => L.T("公司电话打开的服务界面放大几倍。1 = 以前的大小。屏幕放不下时会自动缩小到刚好放得下。", "How much to enlarge the service window opened from company phone calls. 1 = the old size. Shrinks automatically if it doesn't fit the screen.")), false);
0270: 		_cleanup = config.Define("CleanupForUninstall", false, "卸载 / Uninstall", (Func<string>)(() => L.T("卸载前清理", "Clean up before uninstalling")), (Func<string>)(() => L.T("要卸载本模组时打开，并同时勾选下面的“确认清理”。两项都打开后存一次档（睡觉或手动存档）：\n· 公司电话从电话簿和存档里移除；\n· 本模组的物品全部删除（名片、新物品、罗森的手作枪、利维坦等）。\n存完档就可以卸载。不卸载的话请保持关闭。", "Turn on when you want to uninstall, together with “Confirm cleanup” below. With both on, save once (sleep or save manually):\n· company phone numbers are removed from the phone book and the save;\n· every item from this mod is deleted (cards, new items, Rosen's guns, the Leviathan, etc.).\nThen you can uninstall. Keep it off if you're not uninstalling.")), false);
0271: 		_cleanupConfirm = config.Define("CleanupConfirm", false, "卸载 / Uninstall", (Func<string>)(() => L.T("确认清理（会删除本模组的物品）", "Confirm cleanup (deletes this mod's items)")), (Func<string>)(() => L.T("和“卸载前清理”一起打开才会生效。删除的物品无法恢复。", "Only takes effect together with “Clean up before uninstalling”. Deleted items cannot be restored.")), false);
0272: 		_restock = config.Define("RestockWeeks", "1", "周期 / Periods", (Func<string>)(() => L.T("订货补货周期（周）", "Order restock period (weeks)")), new string[3] { "1", "2", "3" }, (Func<string>)(() => L.T("每周限量的货、达夫雷利维坦的配额，多少周补一次货。1 = 每周（原来的做法）。", "How many weeks until weekly order limits and Davre's Leviathan quota refill. 1 = every week (the original behavior).")), false);
~~~

~~~csharp
0269: 		_panelScale = config.Define("PanelScale", "1.5", "画面 / Display", (Func<string>)(() => L.T("电话服务界面大小（倍）", "Phone service window size (x)")), Scales, (Func<string>)(() => L.T("公司电话打开的服务界面放大几倍。1 = 以前的大小。屏幕放不下时会自动缩小到刚好放得下。", "How much to enlarge the service window opened from company phone calls. 1 = the old size. Shrinks automatically if it doesn't fit the screen.")), false);
0270: 		_cleanup = config.Define("CleanupForUninstall", false, "卸载 / Uninstall", (Func<string>)(() => L.T("卸载前清理", "Clean up before uninstalling")), (Func<string>)(() => L.T("要卸载本模组时打开，并同时勾选下面的“确认清理”。两项都打开后存一次档（睡觉或手动存档）：\n· 公司电话从电话簿和存档里移除；\n· 本模组的物品全部删除（名片、新物品、罗森的手作枪、利维坦等）。\n存完档就可以卸载。不卸载的话请保持关闭。", "Turn on when you want to uninstall, together with “Confirm cleanup” below. With both on, save once (sleep or save manually):\n· company phone numbers are removed from the phone book and the save;\n· every item from this mod is deleted (cards, new items, Rosen's guns, the Leviathan, etc.).\nThen you can uninstall. Keep it off if you're not uninstalling.")), false);
0271: 		_cleanupConfirm = config.Define("CleanupConfirm", false, "卸载 / Uninstall", (Func<string>)(() => L.T("确认清理（会删除本模组的物品）", "Confirm cleanup (deletes this mod's items)")), (Func<string>)(() => L.T("和“卸载前清理”一起打开才会生效。删除的物品无法恢复。", "Only takes effect together with “Clean up before uninstalling”. Deleted items cannot be restored.")), false);
0272: 		_restock = config.Define("RestockWeeks", "1", "周期 / Periods", (Func<string>)(() => L.T("订货补货周期（周）", "Order restock period (weeks)")), new string[3] { "1", "2", "3" }, (Func<string>)(() => L.T("每周限量的货、达夫雷利维坦的配额，多少周补一次货。1 = 每周（原来的做法）。", "How many weeks until weekly order limits and Davre's Leviathan quota refill. 1 = every week (the original behavior).")), false);
0273: 		P("vip", 3, "索拉纳贵宾：预约间隔（天）", "Solana VIP: booking interval (days)", "预约一次贵宾后，隔几天才能再约。", "Days before you can book another VIP.");
~~~

~~~csharp
0270: 		_cleanup = config.Define("CleanupForUninstall", false, "卸载 / Uninstall", (Func<string>)(() => L.T("卸载前清理", "Clean up before uninstalling")), (Func<string>)(() => L.T("要卸载本模组时打开，并同时勾选下面的“确认清理”。两项都打开后存一次档（睡觉或手动存档）：\n· 公司电话从电话簿和存档里移除；\n· 本模组的物品全部删除（名片、新物品、罗森的手作枪、利维坦等）。\n存完档就可以卸载。不卸载的话请保持关闭。", "Turn on when you want to uninstall, together with “Confirm cleanup” below. With both on, save once (sleep or save manually):\n· company phone numbers are removed from the phone book and the save;\n· every item from this mod is deleted (cards, new items, Rosen's guns, the Leviathan, etc.).\nThen you can uninstall. Keep it off if you're not uninstalling.")), false);
0271: 		_cleanupConfirm = config.Define("CleanupConfirm", false, "卸载 / Uninstall", (Func<string>)(() => L.T("确认清理（会删除本模组的物品）", "Confirm cleanup (deletes this mod's items)")), (Func<string>)(() => L.T("和“卸载前清理”一起打开才会生效。删除的物品无法恢复。", "Only takes effect together with “Clean up before uninstalling”. Deleted items cannot be restored.")), false);
0272: 		_restock = config.Define("RestockWeeks", "1", "周期 / Periods", (Func<string>)(() => L.T("订货补货周期（周）", "Order restock period (weeks)")), new string[3] { "1", "2", "3" }, (Func<string>)(() => L.T("每周限量的货、达夫雷利维坦的配额，多少周补一次货。1 = 每周（原来的做法）。", "How many weeks until weekly order limits and Davre's Leviathan quota refill. 1 = every week (the original behavior).")), false);
0273: 		P("vip", 3, "索拉纳贵宾：预约间隔（天）", "Solana VIP: booking interval (days)", "预约一次贵宾后，隔几天才能再约。", "Days before you can book another VIP.");
0274: 		P("lab", 7, "索拉纳实验室采购单：周期（天）", "Solana lab order: period (days)", "采购单多少天换一张，每张可以预约一次采购员。", "How often the lab's purchase list changes; one buyer booking per list.");
~~~

~~~csharp
0278: 		P("refurbish", 3, "子午线返厂翻新：间隔（天）", "Meridian refurbishment: interval (days)", "送一次枪翻新后，隔几天才能再送。以前固定 2 天。", "Days between refurbishment drop-offs. Used to be a fixed 2 days.");
0279: 		P("rebate", 7, "帕斯泰尔合作商返点：周期（天）", "pastel dealer rebate: period (days)", "多少天结一次返点，由送货员带来。", "How often the dealer rebate is settled and brought by the courier.");
0280: 		_portraits = config.Define("PortraitSet", PortraitSets[0], "画面 / Display", (Func<string>)(() => L.T("立绘", "Portraits")), PortraitSets, (Func<string>)(() => L.T("选公司人物用哪一套立绘。\n平滑新版立绘：分三档，1080p 起是平滑的高清图。\n像素化新版立绘：像素画，4K 用 4K 图，其余都用 2K 图。\n高清立绘：原来的高清立绘，4K 用 4K 图，其余都用 2K 图（撒该没有这一套，用平滑新版）。\n都按屏幕分辨率用不同清晰度的图；1080p 及以上会关掉游戏的低分辨率中转（会改游戏的像素完美相机）。", "Which set of company portraits to use.\nNew, smooth: three tiers, smooth HD images from 1080p up.\nNew, pixel: pixel art; 4K image at 4K, 2K image otherwise.\nHD: the original HD portraits; 4K image at 4K, 2K image otherwise (Zacchaeus uses New, smooth).\nAll use sharper images at higher resolutions; at 1080p and above the game's low-resolution pass is skipped (changes the game's pixel-perfect camera).")), false);
0281: 		try
0282: 		{
~~~

~~~csharp
0312: 				int item3 = defaultDailyGap.Min;
0313: 				int item4 = defaultDailyGap.Max;
0314: 				item = config.Define(character.Id + ".DailyGapMin", item3.ToString(), text3, (Func<string>)(() => L.T("日常来访间隔·最少（天）", "Daily visit gap, min (days)")), Days, (Func<string>)(() => L.T("这次日常来访之后，最少隔几天再来。改了从下一次来访起生效。", "Minimum days before the next daily visit. Takes effect from the next visit.")), false);
0315: 				item2 = config.Define(character.Id + ".DailyGapMax", item4.ToString(), text3, (Func<string>)(() => L.T("日常来访间隔·最多（天）", "Daily visit gap, max (days)")), Days, (Func<string>)(() => L.T("最多隔几天再来，在最少和最多之间随机。比最少还小时按最少算。", "Maximum days before the next visit; picked at random between min and max. If lower than min, min is used.")), false);
0316: 			}
~~~

~~~csharp
0313: 				int item4 = defaultDailyGap.Max;
0314: 				item = config.Define(character.Id + ".DailyGapMin", item3.ToString(), text3, (Func<string>)(() => L.T("日常来访间隔·最少（天）", "Daily visit gap, min (days)")), Days, (Func<string>)(() => L.T("这次日常来访之后，最少隔几天再来。改了从下一次来访起生效。", "Minimum days before the next daily visit. Takes effect from the next visit.")), false);
0315: 				item2 = config.Define(character.Id + ".DailyGapMax", item4.ToString(), text3, (Func<string>)(() => L.T("日常来访间隔·最多（天）", "Daily visit gap, max (days)")), Days, (Func<string>)(() => L.T("最多隔几天再来，在最少和最多之间随机。比最少还小时按最少算。", "Maximum days before the next visit; picked at random between min and max. If lower than min, min is used.")), false);
0316: 			}
0317: 			ConfigEntry<string> item5 = config.Define(character.Id + ".StoryGap", "1", text3, (Func<string>)(() => L.T("剧情章节间隔（天）", "Gap between story chapters (days)")), Days, (Func<string>)(() => L.T("上一章之后，至少隔几天下一章才来（1 = 条件够了第二天就来，原来的做法）。", "Minimum days between story chapters (1 = the next day once conditions are met, the original behavior).")), false);
~~~

~~~csharp
0315: 				item2 = config.Define(character.Id + ".DailyGapMax", item4.ToString(), text3, (Func<string>)(() => L.T("日常来访间隔·最多（天）", "Daily visit gap, max (days)")), Days, (Func<string>)(() => L.T("最多隔几天再来，在最少和最多之间随机。比最少还小时按最少算。", "Maximum days before the next visit; picked at random between min and max. If lower than min, min is used.")), false);
0316: 			}
0317: 			ConfigEntry<string> item5 = config.Define(character.Id + ".StoryGap", "1", text3, (Func<string>)(() => L.T("剧情章节间隔（天）", "Gap between story chapters (days)")), Days, (Func<string>)(() => L.T("上一章之后，至少隔几天下一章才来（1 = 条件够了第二天就来，原来的做法）。", "Minimum days between story chapters (1 = the next day once conditions are met, the original behavior).")), false);
0318: 			Visits[character.Id] = (item, item2, item5);
0319: 		}
~~~

~~~csharp
0320: 		void P(string key, int def, string zh, string en, string zhTip, string enTip)
0321: 		{
0322: 			PeriodEntries[key] = config.Define("Period." + key, def.ToString(), "周期 / Periods", (Func<string>)(() => L.T(zh, en)), PeriodDays, (Func<string>)(() => L.T(zhTip, enTip)), false);
0323: 		}
0324: 	}
~~~

## CorpPresence/Doreen.cs

~~~csharp
0014: 	public override string Id => "doreen";
0015: 
0016: 	public override string Name => L.T("多琳", "Doreen");
0017: 
0018: 	public override string Faction => "FACTION_LOWER_LEVEL";
~~~

~~~csharp
0026: 	public override int BuyGoodsNeeded => 1;
0027: 
0028: 	private static string Bye => L.T("按时付款，按时到货。", "Pay on time, get it on time.");
0029: 
0030: 	private static string[] Talk => new string[3]
~~~

~~~csharp
0030: 	private static string[] Talk => new string[3]
0031: 	{
0032: 		L.T("“泥炭”是什么肉？配方保密。", "What meat is in Peat? The recipe is confidential."),
0033: 		L.T("饿了有我们达夫雷拉面，当然要加水才能吃。水？也找我们买。", "Hungry? There's Davre ramen. It needs water, of course. Water? Buy that from us too."),
0034: 		L.T("治安部发口粮的时候，我们的单子最多，工厂都要炸了。", "When Security hands out rations, our orders are the biggest. The factory's about to burst.")
~~~

~~~csharp
0031: 	{
0032: 		L.T("“泥炭”是什么肉？配方保密。", "What meat is in Peat? The recipe is confidential."),
0033: 		L.T("饿了有我们达夫雷拉面，当然要加水才能吃。水？也找我们买。", "Hungry? There's Davre ramen. It needs water, of course. Water? Buy that from us too."),
0034: 		L.T("治安部发口粮的时候，我们的单子最多，工厂都要炸了。", "When Security hands out rations, our orders are the biggest. The factory's about to burst.")
0035: 	};
~~~

~~~csharp
0032: 		L.T("“泥炭”是什么肉？配方保密。", "What meat is in Peat? The recipe is confidential."),
0033: 		L.T("饿了有我们达夫雷拉面，当然要加水才能吃。水？也找我们买。", "Hungry? There's Davre ramen. It needs water, of course. Water? Buy that from us too."),
0034: 		L.T("治安部发口粮的时候，我们的单子最多，工厂都要炸了。", "When Security hands out rations, our orders are the biggest. The factory's about to burst.")
0035: 	};
0036: 
~~~

~~~csharp
0132: 			visit3.Lines = new string[3]
0133: 			{
0134: 				L.T("多琳，达夫雷食品，下层区供应专员。", "Doreen. Davre Food, Lower Levels supply rep."),
0135: 				Character.Act(L.T("放下一罐“泥炭”", "sets down a can of Peat"), L.T("便宜，管饱。", "Cheap and filling.")),
0136: 				Character.Act(L.T("看了眼你的水桶", "eyes your water jug"), L.T("海德拉净水器，用旧了别扔，我们回收。", "Don't throw out worn HydraTech purifiers. We buy them back."))
~~~

~~~csharp
0133: 			{
0134: 				L.T("多琳，达夫雷食品，下层区供应专员。", "Doreen. Davre Food, Lower Levels supply rep."),
0135: 				Character.Act(L.T("放下一罐“泥炭”", "sets down a can of Peat"), L.T("便宜，管饱。", "Cheap and filling.")),
0136: 				Character.Act(L.T("看了眼你的水桶", "eyes your water jug"), L.T("海德拉净水器，用旧了别扔，我们回收。", "Don't throw out worn HydraTech purifiers. We buy them back."))
0137: 			};
~~~

~~~csharp
0134: 				L.T("多琳，达夫雷食品，下层区供应专员。", "Doreen. Davre Food, Lower Levels supply rep."),
0135: 				Character.Act(L.T("放下一罐“泥炭”", "sets down a can of Peat"), L.T("便宜，管饱。", "Cheap and filling.")),
0136: 				Character.Act(L.T("看了眼你的水桶", "eyes your water jug"), L.T("海德拉净水器，用旧了别扔，我们回收。", "Don't throw out worn HydraTech purifiers. We buy them back."))
0137: 			};
0138: 			visit3.Exit = Character.Act(L.T("把名片压在柜台上", "pins a card under something on the counter"), Bye);
~~~

~~~csharp
0136: 				Character.Act(L.T("看了眼你的水桶", "eyes your water jug"), L.T("海德拉净水器，用旧了别扔，我们回收。", "Don't throw out worn HydraTech purifiers. We buy them back."))
0137: 			};
0138: 			visit3.Exit = Character.Act(L.T("把名片压在柜台上", "pins a card under something on the counter"), Bye);
0139: 			visit3.Discount = 30;
0140: 			visit3.Stock = new string[6] { "processed_meat", "processed_meat", "processed_meat", "cup_noodle", "cup_noodle", "cup_noodle" };
~~~

~~~csharp
0146: 			visit2.Lines = new string[3]
0147: 			{
0148: 				L.T("达夫雷在收海德拉的净水器。", "Davre is buying HydraTech purifiers."),
0149: 				L.T("新的旧的都要。", "New or used."),
0150: 				Character.Act(L.T("停顿", "pauses"), L.T("研发部要拆开看看。", "R&D wants to take them apart."))
~~~

~~~csharp
0147: 			{
0148: 				L.T("达夫雷在收海德拉的净水器。", "Davre is buying HydraTech purifiers."),
0149: 				L.T("新的旧的都要。", "New or used."),
0150: 				Character.Act(L.T("停顿", "pauses"), L.T("研发部要拆开看看。", "R&D wants to take them apart."))
0151: 			};
~~~

~~~csharp
0148: 				L.T("达夫雷在收海德拉的净水器。", "Davre is buying HydraTech purifiers."),
0149: 				L.T("新的旧的都要。", "New or used."),
0150: 				Character.Act(L.T("停顿", "pauses"), L.T("研发部要拆开看看。", "R&D wants to take them apart."))
0151: 			};
0152: 			visit2.Exit = Bye;
~~~

~~~csharp
0152: 			visit2.Exit = Bye;
0153: 			visit2.Buy = true;
0154: 			visit2.Wrong = L.T("只收海德拉的净水器。", "HydraTech purifiers only.");
0155: 			return visit2;
0156: 		}
~~~

~~~csharp
0162: 			visit.Lines = new string[5]
0163: 			{
0164: 				L.T("听说你开始卖水了。", "Heard you started selling water."),
0165: 				Character.Act(L.T("放下一台净水器", "sets down a purifier"), L.T("利维坦。达夫雷自己做的。", "The Leviathan. Davre makes it in-house.")),
0166: 				L.T("渗透度比海德拉差点，但是铁锈水、幽灵水都能过滤。", "It filters a bit worse than a HydraTech, but it handles Rustwater and Ghostwater."),
~~~

~~~csharp
0163: 			{
0164: 				L.T("听说你开始卖水了。", "Heard you started selling water."),
0165: 				Character.Act(L.T("放下一台净水器", "sets down a purifier"), L.T("利维坦。达夫雷自己做的。", "The Leviathan. Davre makes it in-house.")),
0166: 				L.T("渗透度比海德拉差点，但是铁锈水、幽灵水都能过滤。", "It filters a bit worse than a HydraTech, but it handles Rustwater and Ghostwater."),
0167: 				L.T("而且比海德拉耐用。", "And it lasts longer than a HydraTech."),
~~~

~~~csharp
0164: 				L.T("听说你开始卖水了。", "Heard you started selling water."),
0165: 				Character.Act(L.T("放下一台净水器", "sets down a purifier"), L.T("利维坦。达夫雷自己做的。", "The Leviathan. Davre makes it in-house.")),
0166: 				L.T("渗透度比海德拉差点，但是铁锈水、幽灵水都能过滤。", "It filters a bit worse than a HydraTech, but it handles Rustwater and Ghostwater."),
0167: 				L.T("而且比海德拉耐用。", "And it lasts longer than a HydraTech."),
0168: 				L.T("每周可以订一台。打电话就行。", "You can order one a week. Just call.")
~~~

~~~csharp
0165: 				Character.Act(L.T("放下一台净水器", "sets down a purifier"), L.T("利维坦。达夫雷自己做的。", "The Leviathan. Davre makes it in-house.")),
0166: 				L.T("渗透度比海德拉差点，但是铁锈水、幽灵水都能过滤。", "It filters a bit worse than a HydraTech, but it handles Rustwater and Ghostwater."),
0167: 				L.T("而且比海德拉耐用。", "And it lasts longer than a HydraTech."),
0168: 				L.T("每周可以订一台。打电话就行。", "You can order one a week. Just call.")
0169: 			};
~~~

~~~csharp
0166: 				L.T("渗透度比海德拉差点，但是铁锈水、幽灵水都能过滤。", "It filters a bit worse than a HydraTech, but it handles Rustwater and Ghostwater."),
0167: 				L.T("而且比海德拉耐用。", "And it lasts longer than a HydraTech."),
0168: 				L.T("每周可以订一台。打电话就行。", "You can order one a week. Just call.")
0169: 			};
0170: 			visit.Exit = L.T("水多少其实不重要。净化工艺谁手里，才重要。", "How much water there is doesn't really matter. Who holds the purification process does.");
~~~

~~~csharp
0168: 				L.T("每周可以订一台。打电话就行。", "You can order one a week. Just call.")
0169: 			};
0170: 			visit.Exit = L.T("水多少其实不重要。净化工艺谁手里，才重要。", "How much water there is doesn't really matter. Who holds the purification process does.");
0171: 			visit.TalkOnly = true;
0172: 			visit.Gift = new string[1] { "@leviathan" };
~~~

~~~csharp
0182: 			Lines = new string[4]
0183: 			{
0184: 				L.T("达夫雷要扩产。", "Davre is expanding production."),
0185: 				Character.Act(L.T("压低声音", "lowers her voice"), L.T("帕斯泰尔在往食品这块伸手。达夫雷决定下放一些设备。", "Pastel is reaching into food. Davre has decided to hand out some equipment.")),
0186: 				L.T("总部批了一批商用机器，下发给你这种小店。绞肉机一台，罐装机一台。", "HQ approved a batch of commercial machines for small shops like yours. One grinder, one canner."),
~~~

~~~csharp
0183: 			{
0184: 				L.T("达夫雷要扩产。", "Davre is expanding production."),
0185: 				Character.Act(L.T("压低声音", "lowers her voice"), L.T("帕斯泰尔在往食品这块伸手。达夫雷决定下放一些设备。", "Pastel is reaching into food. Davre has decided to hand out some equipment.")),
0186: 				L.T("总部批了一批商用机器，下发给你这种小店。绞肉机一台，罐装机一台。", "HQ approved a batch of commercial machines for small shops like yours. One grinder, one canner."),
0187: 				L.T($"手续费 {DavreCo.CanningFee}₵。机器归你，原料自己找。", $"{DavreCo.CanningFee}₵ handling fee. The machines are yours; source your own meat.")
~~~

~~~csharp
0184: 				L.T("达夫雷要扩产。", "Davre is expanding production."),
0185: 				Character.Act(L.T("压低声音", "lowers her voice"), L.T("帕斯泰尔在往食品这块伸手。达夫雷决定下放一些设备。", "Pastel is reaching into food. Davre has decided to hand out some equipment.")),
0186: 				L.T("总部批了一批商用机器，下发给你这种小店。绞肉机一台，罐装机一台。", "HQ approved a batch of commercial machines for small shops like yours. One grinder, one canner."),
0187: 				L.T($"手续费 {DavreCo.CanningFee}₵。机器归你，原料自己找。", $"{DavreCo.CanningFee}₵ handling fee. The machines are yours; source your own meat.")
0188: 			},
~~~

~~~csharp
0185: 				Character.Act(L.T("压低声音", "lowers her voice"), L.T("帕斯泰尔在往食品这块伸手。达夫雷决定下放一些设备。", "Pastel is reaching into food. Davre has decided to hand out some equipment.")),
0186: 				L.T("总部批了一批商用机器，下发给你这种小店。绞肉机一台，罐装机一台。", "HQ approved a batch of commercial machines for small shops like yours. One grinder, one canner."),
0187: 				L.T($"手续费 {DavreCo.CanningFee}₵。机器归你，原料自己找。", $"{DavreCo.CanningFee}₵ handling fee. The machines are yours; source your own meat.")
0188: 			},
0189: 			Exit = Bye,
~~~

~~~csharp
0189: 			Exit = Bye,
0190: 			TalkOnly = true,
0191: 			Ask = L.T("做不做？", "In or out?")
0192: 		};
0193: 		v.Choices = new Choice[2]
~~~

~~~csharp
0193: 		v.Choices = new Choice[2]
0194: 		{
0195: 			new Choice(L.T("我做。", "I'm in."), L.T("以后坏了、要添，打电话。", "If one breaks or you want more, call."), delegate
0196: 			{
0197: 				if (DavreCo.PayCanningFee())
~~~

~~~csharp
0204: 				}
0205: 			}),
0206: 			new Choice(L.T("再想想。", "Let me think."), L.T("名额不等人。想好了打电话，手续费照旧。", "The slots won't wait. Call when you've decided. Same fee."), null)
0207: 		};
0208: 		return v;
~~~

~~~csharp
0211: 	public override Visit Daily(CharacterState s)
0212: 	{
0213: 		string[] array = (s.SideDone.Contains(3) ? Talk.Append(L.T("利维坦用完了？别担心这是量产品。", "Leviathan worn out? Don't worry, it's mass-produced.")).ToArray() : Talk);
0214: 		if (DavreCo.CanningOpen)
0215: 		{
~~~

~~~csharp
0214: 		if (DavreCo.CanningOpen)
0215: 		{
0216: 			array = array.Append(L.T("罐头卖得怎么样？", "How are the cans selling?")).ToArray();
0217: 		}
0218: 		Visit visit = new Visit();
~~~

~~~csharp
0219: 		visit.Lines = new string[2]
0220: 		{
0221: 			Engine.Any(L.T("达夫雷，例行供货。", "Davre. Routine supply."), L.T("多琳。照单送货。", "Doreen. Delivering per the order."), L.T("（瞧了一眼水箱）需要净水服务吗？", "(glances at the water tank) Need purification service?")),
0222: 			Engine.Any(array)
0223: 		};
~~~

~~~csharp
0222: 			Engine.Any(array)
0223: 		};
0224: 		visit.Exit = Engine.Any(Bye, Character.Act(L.T("看了看清单", "checks the list"), L.T("下一家。", "Next stop.")));
0225: 		visit.Discount = 20;
0226: 		visit.Stock = Engine.Pick(new string[2] { "processed_meat", "cup_noodle" }, 4, 6);
~~~

## CorpPresence/Cards.cs

~~~csharp
0041: 		try
0042: 		{
0043: 			card.SetName(L.T("名片（" + c.Name + "）", "Business Card (" + c.Name + ")"));
0044: 		}
0045: 		catch
~~~

~~~csharp
0048: 		try
0049: 		{
0050: 			card.flavorText = c.CardFlavor;
0051: 		}
0052: 		catch
~~~

## CorpPresence/DavreCo.cs

~~~csharp
0063: 	public override string Id => "davre";
0064: 
0065: 	public override string Name => L.T("达夫雷食品", "Davre Food");
0066: 
0067: 	public override string CharacterId => "doreen";
~~~

~~~csharp
0069: 	public override long PhoneNumber => 32873L;
0070: 
0071: 	public override string[] TierNames => L.T(new string[3] { "普通供货", "区域供货", "长期合约" }, new string[3] { "Standard Supplier", "Regional Supplier", "Long-Term Contract" });
0072: 
0073: 	public override string CardFlavor => L.T("红底米色的达夫雷标志，32873。", "The Davre logo, beige on red. 32873.");
~~~

~~~csharp
0071: 	public override string[] TierNames => L.T(new string[3] { "普通供货", "区域供货", "长期合约" }, new string[3] { "Standard Supplier", "Regional Supplier", "Long-Term Contract" });
0072: 
0073: 	public override string CardFlavor => L.T("红底米色的达夫雷标志，32873。", "The Davre logo, beige on red. 32873.");
0074: 
0075: 	public override string DeskName => L.T("达夫雷订货中心", "Davre Order Center");
~~~

~~~csharp
0073: 	public override string CardFlavor => L.T("红底米色的达夫雷标志，32873。", "The Davre logo, beige on red. 32873.");
0074: 
0075: 	public override string DeskName => L.T("达夫雷订货中心", "Davre Order Center");
0076: 
0077: 	public override string DeskAsk => "";
~~~

~~~csharp
0077: 	public override string DeskAsk => "";
0078: 
0079: 	public override string[] FrontDesk => L.T(new string[2] { "达夫雷食品，订货中心。请报供货编号。", "达夫雷。罐头和药片，今天都有。" }, new string[2] { "Davre Food, order center. Supplier number, please.", "Davre. Cans and tablets, all in stock today." });
0080: 
0081: 	public override string FrontDeskDone => L.T("已排单，明天到货。", "Scheduled. Arriving tomorrow.");
~~~

~~~csharp
0079: 	public override string[] FrontDesk => L.T(new string[2] { "达夫雷食品，订货中心。请报供货编号。", "达夫雷。罐头和药片，今天都有。" }, new string[2] { "Davre Food, order center. Supplier number, please.", "Davre. Cans and tablets, all in stock today." });
0080: 
0081: 	public override string FrontDeskDone => L.T("已排单，明天到货。", "Scheduled. Arriving tomorrow.");
0082: 
0083: 	public override CatalogEntry[] Catalog => Entries;
~~~

## CorpPresence/Isa.cs

~~~csharp
0021: 	public override string Id => "isa";
0022: 
0023: 	public override string Name => L.T("伊莎", "Isa");
0024: 
0025: 	public override string Faction => "FACTION_UPPER_LEVEL";
~~~

~~~csharp
0035: 	public override int BuyGoodsNeeded => 1;
0036: 
0037: 	private static string Bye => L.T("本次拜访不构成任何医疗建议（官方腔调）。", "This visit does not constitute medical advice. (in an official tone)");
0038: 
0039: 	private static string[] Talk => new string[6]
~~~

~~~csharp
0039: 	private static string[] Talk => new string[6]
0040: 	{
0041: 		L.T("零嚼只是在上层区改了名，叫“纤体-X”。药是同一种，可是价格在下城区能买一箱零嚼。", "In the Upper Levels, ZeroChew just got renamed \"Slim-X\". Same drug, but what it costs up there buys a whole box of ZeroChew down here."),
0042: 		L.T("免疫宁每周都需要一针。回头客真是稳定（讽刺意味）。", "Immunivax, one shot every week. Such reliable repeat customers. (sarcastically)"),
0043: 		L.T("序列号前三位是批次。需要召回的批次，手册上有，记得比对一下。", "The first three digits of the serial are the batch. Batches to be recalled are in the manual. Remember to check against it."),
~~~

~~~csharp
0040: 	{
0041: 		L.T("零嚼只是在上层区改了名，叫“纤体-X”。药是同一种，可是价格在下城区能买一箱零嚼。", "In the Upper Levels, ZeroChew just got renamed \"Slim-X\". Same drug, but what it costs up there buys a whole box of ZeroChew down here."),
0042: 		L.T("免疫宁每周都需要一针。回头客真是稳定（讽刺意味）。", "Immunivax, one shot every week. Such reliable repeat customers. (sarcastically)"),
0043: 		L.T("序列号前三位是批次。需要召回的批次，手册上有，记得比对一下。", "The first three digits of the serial are the batch. Batches to be recalled are in the manual. Remember to check against it."),
0044: 		L.T("奥克西莫是合法药品，手续齐全。", "Oxymore is a legal drug. All the paperwork is in order."),
~~~

~~~csharp
0041: 		L.T("零嚼只是在上层区改了名，叫“纤体-X”。药是同一种，可是价格在下城区能买一箱零嚼。", "In the Upper Levels, ZeroChew just got renamed \"Slim-X\". Same drug, but what it costs up there buys a whole box of ZeroChew down here."),
0042: 		L.T("免疫宁每周都需要一针。回头客真是稳定（讽刺意味）。", "Immunivax, one shot every week. Such reliable repeat customers. (sarcastically)"),
0043: 		L.T("序列号前三位是批次。需要召回的批次，手册上有，记得比对一下。", "The first three digits of the serial are the batch. Batches to be recalled are in the manual. Remember to check against it."),
0044: 		L.T("奥克西莫是合法药品，手续齐全。", "Oxymore is a legal drug. All the paperwork is in order."),
0045: 		L.T("有人说哨兵崩溃是人为设计的。公司对此不予置评（官方腔调）。", "Some say Sentinel Collapse was engineered. The company has no comment. (in an official tone)"),
~~~

~~~csharp
0042: 		L.T("免疫宁每周都需要一针。回头客真是稳定（讽刺意味）。", "Immunivax, one shot every week. Such reliable repeat customers. (sarcastically)"),
0043: 		L.T("序列号前三位是批次。需要召回的批次，手册上有，记得比对一下。", "The first three digits of the serial are the batch. Batches to be recalled are in the manual. Remember to check against it."),
0044: 		L.T("奥克西莫是合法药品，手续齐全。", "Oxymore is a legal drug. All the paperwork is in order."),
0045: 		L.T("有人说哨兵崩溃是人为设计的。公司对此不予置评（官方腔调）。", "Some say Sentinel Collapse was engineered. The company has no comment. (in an official tone)"),
0046: 		L.T("噬菌霉素，专利在卡迈克尔手里，可是大部分人居然都不知道。", "Carmicheal holds the Phagimycin patent. Most people don't even know that.")
~~~

~~~csharp
0043: 		L.T("序列号前三位是批次。需要召回的批次，手册上有，记得比对一下。", "The first three digits of the serial are the batch. Batches to be recalled are in the manual. Remember to check against it."),
0044: 		L.T("奥克西莫是合法药品，手续齐全。", "Oxymore is a legal drug. All the paperwork is in order."),
0045: 		L.T("有人说哨兵崩溃是人为设计的。公司对此不予置评（官方腔调）。", "Some say Sentinel Collapse was engineered. The company has no comment. (in an official tone)"),
0046: 		L.T("噬菌霉素，专利在卡迈克尔手里，可是大部分人居然都不知道。", "Carmicheal holds the Phagimycin patent. Most people don't even know that.")
0047: 	};
~~~

~~~csharp
0044: 		L.T("奥克西莫是合法药品，手续齐全。", "Oxymore is a legal drug. All the paperwork is in order."),
0045: 		L.T("有人说哨兵崩溃是人为设计的。公司对此不予置评（官方腔调）。", "Some say Sentinel Collapse was engineered. The company has no comment. (in an official tone)"),
0046: 		L.T("噬菌霉素，专利在卡迈克尔手里，可是大部分人居然都不知道。", "Carmicheal holds the Phagimycin patent. Most people don't even know that.")
0047: 	};
0048: 
~~~

~~~csharp
0185: 			visit7.Lines = new string[5]
0186: 			{
0187: 				L.T("卡迈克尔制药，市场规范部专员。伊莎·兰登。", "Carmicheal Pharmaceutical, Market Compliance. Isa Langdon."),
0188: 				L.T("你能见到的药一大半都是我们在生产，只是目前公司最在意的还是专利期内的免疫宁", "Most of the medicine you'll ever see is ours. Right now, though, the company cares most about Immunivax, still under patent."),
0189: 				L.T("专利期内，居然会有大量仿制品，真是荒唐（不快）。", "Still under patent, and yet copies are everywhere. Absurd. (displeased)"),
~~~

~~~csharp
0186: 			{
0187: 				L.T("卡迈克尔制药，市场规范部专员。伊莎·兰登。", "Carmicheal Pharmaceutical, Market Compliance. Isa Langdon."),
0188: 				L.T("你能见到的药一大半都是我们在生产，只是目前公司最在意的还是专利期内的免疫宁", "Most of the medicine you'll ever see is ours. Right now, though, the company cares most about Immunivax, still under patent."),
0189: 				L.T("专利期内，居然会有大量仿制品，真是荒唐（不快）。", "Still under patent, and yet copies are everywhere. Absurd. (displeased)"),
0190: 				L.T("最近大量仿制品已经冲击了正品售卖", "Lately the copies have been cutting into genuine sales."),
~~~

~~~csharp
0187: 				L.T("卡迈克尔制药，市场规范部专员。伊莎·兰登。", "Carmicheal Pharmaceutical, Market Compliance. Isa Langdon."),
0188: 				L.T("你能见到的药一大半都是我们在生产，只是目前公司最在意的还是专利期内的免疫宁", "Most of the medicine you'll ever see is ours. Right now, though, the company cares most about Immunivax, still under patent."),
0189: 				L.T("专利期内，居然会有大量仿制品，真是荒唐（不快）。", "Still under patent, and yet copies are everywhere. Absurd. (displeased)"),
0190: 				L.T("最近大量仿制品已经冲击了正品售卖", "Lately the copies have been cutting into genuine sales."),
0191: 				Character.Act(L.T("递过一本手册", "hands over a manual"), L.T("我希望你能帮我们回收一部分仿制品。", "I'd like you to help us take some of the copies off the market."))
~~~

~~~csharp
0188: 				L.T("你能见到的药一大半都是我们在生产，只是目前公司最在意的还是专利期内的免疫宁", "Most of the medicine you'll ever see is ours. Right now, though, the company cares most about Immunivax, still under patent."),
0189: 				L.T("专利期内，居然会有大量仿制品，真是荒唐（不快）。", "Still under patent, and yet copies are everywhere. Absurd. (displeased)"),
0190: 				L.T("最近大量仿制品已经冲击了正品售卖", "Lately the copies have been cutting into genuine sales."),
0191: 				Character.Act(L.T("递过一本手册", "hands over a manual"), L.T("我希望你能帮我们回收一部分仿制品。", "I'd like you to help us take some of the copies off the market."))
0192: 			};
~~~

~~~csharp
0189: 				L.T("专利期内，居然会有大量仿制品，真是荒唐（不快）。", "Still under patent, and yet copies are everywhere. Absurd. (displeased)"),
0190: 				L.T("最近大量仿制品已经冲击了正品售卖", "Lately the copies have been cutting into genuine sales."),
0191: 				Character.Act(L.T("递过一本手册", "hands over a manual"), L.T("我希望你能帮我们回收一部分仿制品。", "I'd like you to help us take some of the copies off the market."))
0192: 			};
0193: 			visit7.Exit = Character.Act(L.T("合上文件夹", "closes the folder"), Bye);
~~~

~~~csharp
0191: 				Character.Act(L.T("递过一本手册", "hands over a manual"), L.T("我希望你能帮我们回收一部分仿制品。", "I'd like you to help us take some of the copies off the market."))
0192: 			};
0193: 			visit7.Exit = Character.Act(L.T("合上文件夹", "closes the folder"), Bye);
0194: 			visit7.Discount = 30;
0195: 			visit7.Stock = new string[2] { "large_purple_injector", "large_purple_injector" };
~~~

~~~csharp
0202: 			visit6.Lines = new string[3]
0203: 			{
0204: 				L.T("召回通知。", "Recall notice. ") + Character.Act(L.T("递过一张单子", "hands over a list"), L.T("单子上的批次我们都会回收。", "We're recalling every batch on this list.")),
0205: 				L.T("（官方腔调）你有外售权限", "(in an official tone) You are authorized to resell."),
0206: 				Character.Act(L.T("停顿", "pauses"), L.T("这批次的药其实没问题，就是外贸部经理那个糊涂蛋把价格定低了。", "Nothing was actually wrong with these batches. That idiot of a trade manager just priced them too low."))
~~~

~~~csharp
0203: 			{
0204: 				L.T("召回通知。", "Recall notice. ") + Character.Act(L.T("递过一张单子", "hands over a list"), L.T("单子上的批次我们都会回收。", "We're recalling every batch on this list.")),
0205: 				L.T("（官方腔调）你有外售权限", "(in an official tone) You are authorized to resell."),
0206: 				Character.Act(L.T("停顿", "pauses"), L.T("这批次的药其实没问题，就是外贸部经理那个糊涂蛋把价格定低了。", "Nothing was actually wrong with these batches. That idiot of a trade manager just priced them too low."))
0207: 			};
~~~

~~~csharp
0204: 				L.T("召回通知。", "Recall notice. ") + Character.Act(L.T("递过一张单子", "hands over a list"), L.T("单子上的批次我们都会回收。", "We're recalling every batch on this list.")),
0205: 				L.T("（官方腔调）你有外售权限", "(in an official tone) You are authorized to resell."),
0206: 				Character.Act(L.T("停顿", "pauses"), L.T("这批次的药其实没问题，就是外贸部经理那个糊涂蛋把价格定低了。", "Nothing was actually wrong with these batches. That idiot of a trade manager just priced them too low."))
0207: 			};
0208: 			visit6.Exit = Character.Act(L.T("在单子上签字", "signs the list"), L.T("已送达。", "Delivered."));
~~~

~~~csharp
0206: 				Character.Act(L.T("停顿", "pauses"), L.T("这批次的药其实没问题，就是外贸部经理那个糊涂蛋把价格定低了。", "Nothing was actually wrong with these batches. That idiot of a trade manager just priced them too low."))
0207: 			};
0208: 			visit6.Exit = Character.Act(L.T("在单子上签字", "signs the list"), L.T("已送达。", "Delivered."));
0209: 			visit6.Discount = 30;
0210: 			visit6.Stock = new string[3] { "large_purple_injector", "large_purple_injector", "large_purple_injector" };
~~~

~~~csharp
0216: 			visit5.Lines = new string[3]
0217: 			{
0218: 				L.T("（自言自语）奥克西莫，这个季度下层区销量涨了四成。", "(to herself) Oxymore sales in the Lower Levels are up forty percent this quarter."),
0219: 				Character.Act(L.T("翻了一页", "turns a page"), L.T("纳洛通，也涨了四成。", "Naloxgone, also up forty percent.")),
0220: 				L.T("哦对不起我们继续谈生意。", "Oh, sorry. Back to business.")
~~~

~~~csharp
0217: 			{
0218: 				L.T("（自言自语）奥克西莫，这个季度下层区销量涨了四成。", "(to herself) Oxymore sales in the Lower Levels are up forty percent this quarter."),
0219: 				Character.Act(L.T("翻了一页", "turns a page"), L.T("纳洛通，也涨了四成。", "Naloxgone, also up forty percent.")),
0220: 				L.T("哦对不起我们继续谈生意。", "Oh, sorry. Back to business.")
0221: 			};
~~~

~~~csharp
0218: 				L.T("（自言自语）奥克西莫，这个季度下层区销量涨了四成。", "(to herself) Oxymore sales in the Lower Levels are up forty percent this quarter."),
0219: 				Character.Act(L.T("翻了一页", "turns a page"), L.T("纳洛通，也涨了四成。", "Naloxgone, also up forty percent.")),
0220: 				L.T("哦对不起我们继续谈生意。", "Oh, sorry. Back to business.")
0221: 			};
0222: 			visit5.Exit = Character.Act(L.T("合上文件夹，犹豫", "closes the folder, hesitates"), L.T("最好别用奥克西莫。", "Better not to use Oxymore."));
~~~

~~~csharp
0220: 				L.T("哦对不起我们继续谈生意。", "Oh, sorry. Back to business.")
0221: 			};
0222: 			visit5.Exit = Character.Act(L.T("合上文件夹，犹豫", "closes the folder, hesitates"), L.T("最好别用奥克西莫。", "Better not to use Oxymore."));
0223: 			visit5.Discount = 30;
0224: 			visit5.Stock = new string[6] { "pink_injector", "pink_injector", "pink_injector", "pure_white_injector", "pure_white_injector", "pure_white_injector" };
~~~

~~~csharp
0230: 			visit4.Lines = new string[3]
0231: 			{
0232: 				L.T("我听说你这里有赛纳普斯？就是帕斯泰尔的戒瘾针。", "I hear you have some Synaprest? pastel's addiction shot."),
0233: 				L.T("研发部那帮人非要要拆开看看。", "The R&D people insist on taking it apart."),
0234: 				Character.Act(L.T("停顿", "pauses"), L.T("戒瘾的生意，我们也想做。", "We'd like a piece of the addiction business too."))
~~~

~~~csharp
0231: 			{
0232: 				L.T("我听说你这里有赛纳普斯？就是帕斯泰尔的戒瘾针。", "I hear you have some Synaprest? pastel's addiction shot."),
0233: 				L.T("研发部那帮人非要要拆开看看。", "The R&D people insist on taking it apart."),
0234: 				Character.Act(L.T("停顿", "pauses"), L.T("戒瘾的生意，我们也想做。", "We'd like a piece of the addiction business too."))
0235: 			};
~~~

~~~csharp
0232: 				L.T("我听说你这里有赛纳普斯？就是帕斯泰尔的戒瘾针。", "I hear you have some Synaprest? pastel's addiction shot."),
0233: 				L.T("研发部那帮人非要要拆开看看。", "The R&D people insist on taking it apart."),
0234: 				Character.Act(L.T("停顿", "pauses"), L.T("戒瘾的生意，我们也想做。", "We'd like a piece of the addiction business too."))
0235: 			};
0236: 			visit4.Exit = Character.Act(L.T("放进冷藏箱", "puts it in a cooler"), L.T("别跟帕斯泰尔提。", "Don't mention this to pastel."));
~~~

~~~csharp
0234: 				Character.Act(L.T("停顿", "pauses"), L.T("戒瘾的生意，我们也想做。", "We'd like a piece of the addiction business too."))
0235: 			};
0236: 			visit4.Exit = Character.Act(L.T("放进冷藏箱", "puts it in a cooler"), L.T("别跟帕斯泰尔提。", "Don't mention this to pastel."));
0237: 			visit4.Buy = true;
0238: 			visit4.Wrong = L.T("呃，店长，我是要赛纳普斯。", "Uh, I need Synaprest, boss.");
~~~

~~~csharp
0236: 			visit4.Exit = Character.Act(L.T("放进冷藏箱", "puts it in a cooler"), L.T("别跟帕斯泰尔提。", "Don't mention this to pastel."));
0237: 			visit4.Buy = true;
0238: 			visit4.Wrong = L.T("呃，店长，我是要赛纳普斯。", "Uh, I need Synaprest, boss.");
0239: 			return visit4;
0240: 		}
~~~

~~~csharp
0244: 			visit3.Lines = new string[3]
0245: 			{
0246: 				L.T("收容物在你这里吧。真让我头疼。", "The specimen is with you, isn't it. What a headache."),
0247: 				L.T("不过公司的繁殖计划才初步完成，你怎么能养得这么好。", "The company's breeding program has barely gotten started, and you've raised it this well?"),
0248: 				Character.Act(L.T("放下一份文件和一支采血器", "sets down a document and a blood drawer"), L.T("我帮你申请了养殖许可。以后要养这种未知生物，先跟我确定一下。", "I got you a breeding license. If you want to keep more of these unknown creatures, check with me first."))
~~~

~~~csharp
0245: 			{
0246: 				L.T("收容物在你这里吧。真让我头疼。", "The specimen is with you, isn't it. What a headache."),
0247: 				L.T("不过公司的繁殖计划才初步完成，你怎么能养得这么好。", "The company's breeding program has barely gotten started, and you've raised it this well?"),
0248: 				Character.Act(L.T("放下一份文件和一支采血器", "sets down a document and a blood drawer"), L.T("我帮你申请了养殖许可。以后要养这种未知生物，先跟我确定一下。", "I got you a breeding license. If you want to keep more of these unknown creatures, check with me first."))
0249: 			};
~~~

~~~csharp
0246: 				L.T("收容物在你这里吧。真让我头疼。", "The specimen is with you, isn't it. What a headache."),
0247: 				L.T("不过公司的繁殖计划才初步完成，你怎么能养得这么好。", "The company's breeding program has barely gotten started, and you've raised it this well?"),
0248: 				Character.Act(L.T("放下一份文件和一支采血器", "sets down a document and a blood drawer"), L.T("我帮你申请了养殖许可。以后要养这种未知生物，先跟我确定一下。", "I got you a breeding license. If you want to keep more of these unknown creatures, check with me first."))
0249: 			};
0250: 			visit3.Exit = Character.Act(L.T("合上文件夹", "closes the folder"), L.T("当然三叶蜥的产物我们也会回收，虽然我现在连产什么都不知道。", "We'll buy back whatever the trilobites produce, of course. Though right now I don't even know what that is."));
~~~

~~~csharp
0248: 				Character.Act(L.T("放下一份文件和一支采血器", "sets down a document and a blood drawer"), L.T("我帮你申请了养殖许可。以后要养这种未知生物，先跟我确定一下。", "I got you a breeding license. If you want to keep more of these unknown creatures, check with me first."))
0249: 			};
0250: 			visit3.Exit = Character.Act(L.T("合上文件夹", "closes the folder"), L.T("当然三叶蜥的产物我们也会回收，虽然我现在连产什么都不知道。", "We'll buy back whatever the trilobites produce, of course. Though right now I don't even know what that is."));
0251: 			visit3.TalkOnly = true;
0252: 			visit3.Gift = new string[1] { "@blood_drawer" };
~~~

~~~csharp
0258: 			visit2.Lines = new string[3]
0259: 			{
0260: 				L.T("你采过血了吗。研发部想谈原料供应。", "Have you drawn its blood yet? R&D wants to talk about supply."),
0261: 				Character.Act(L.T("让人搬进来一台机器", "has a machine carried in"), L.T("反应釜。纯度够，能做三种药。", "A reactor. With enough purity, it makes three kinds of medicine.")),
0262: 				L.T("这台机器不收钱。以后坏了，要按原价订。", "This machine is free. If it breaks, you'll have to order another at list price.")
~~~

~~~csharp
0259: 			{
0260: 				L.T("你采过血了吗。研发部想谈原料供应。", "Have you drawn its blood yet? R&D wants to talk about supply."),
0261: 				Character.Act(L.T("让人搬进来一台机器", "has a machine carried in"), L.T("反应釜。纯度够，能做三种药。", "A reactor. With enough purity, it makes three kinds of medicine.")),
0262: 				L.T("这台机器不收钱。以后坏了，要按原价订。", "This machine is free. If it breaks, you'll have to order another at list price.")
0263: 			};
~~~

~~~csharp
0260: 				L.T("你采过血了吗。研发部想谈原料供应。", "Have you drawn its blood yet? R&D wants to talk about supply."),
0261: 				Character.Act(L.T("让人搬进来一台机器", "has a machine carried in"), L.T("反应釜。纯度够，能做三种药。", "A reactor. With enough purity, it makes three kinds of medicine.")),
0262: 				L.T("这台机器不收钱。以后坏了，要按原价订。", "This machine is free. If it breaks, you'll have to order another at list price.")
0263: 			};
0264: 			visit2.Exit = Character.Act(L.T("合上文件夹", "closes the folder"), L.T("记住，纯度，决定一切。", "Remember: purity is everything."));
~~~

~~~csharp
0262: 				L.T("这台机器不收钱。以后坏了，要按原价订。", "This machine is free. If it breaks, you'll have to order another at list price.")
0263: 			};
0264: 			visit2.Exit = Character.Act(L.T("合上文件夹", "closes the folder"), L.T("记住，纯度，决定一切。", "Remember: purity is everything."));
0265: 			visit2.TalkOnly = true;
0266: 			visit2.Gift = new string[2] { "@isomer_reactor", "@sb_handbook" };
~~~

~~~csharp
0272: 			visit.Lines = new string[3]
0273: 			{
0274: 				L.T("新品体力增强剂，目前还没上市，老板让我来找几个实验人员。", "A new product, the Stamina Booster. Not on the market yet. My boss wants me to find a few test subjects."),
0275: 				L.T("能改善体质，每一支可是都要走实验流程才能申请，请你珍惜，实验员。", "It improves your constitution. Every dose has to go through the trial process, so treat it well, test subject."),
0276: 				Character.Act(L.T("放下一支", "sets one down"), L.T("这是本期的，以后要用，直接向我申请就行。", "This one is for this period. If you need more later, just apply to me directly."))
~~~

~~~csharp
0273: 			{
0274: 				L.T("新品体力增强剂，目前还没上市，老板让我来找几个实验人员。", "A new product, the Stamina Booster. Not on the market yet. My boss wants me to find a few test subjects."),
0275: 				L.T("能改善体质，每一支可是都要走实验流程才能申请，请你珍惜，实验员。", "It improves your constitution. Every dose has to go through the trial process, so treat it well, test subject."),
0276: 				Character.Act(L.T("放下一支", "sets one down"), L.T("这是本期的，以后要用，直接向我申请就行。", "This one is for this period. If you need more later, just apply to me directly."))
0277: 			};
~~~

~~~csharp
0274: 				L.T("新品体力增强剂，目前还没上市，老板让我来找几个实验人员。", "A new product, the Stamina Booster. Not on the market yet. My boss wants me to find a few test subjects."),
0275: 				L.T("能改善体质，每一支可是都要走实验流程才能申请，请你珍惜，实验员。", "It improves your constitution. Every dose has to go through the trial process, so treat it well, test subject."),
0276: 				Character.Act(L.T("放下一支", "sets one down"), L.T("这是本期的，以后要用，直接向我申请就行。", "This one is for this period. If you need more later, just apply to me directly."))
0277: 			};
0278: 			visit.Exit = Character.Act(L.T("合上文件夹", "closes the folder"), L.T("下个季度，下层区划拨到我的管辖范围，一定记得定期申请体力针剂。（小声嘟囔）这可是好东西。", "Next quarter the Lower Levels fall under my jurisdiction. Be sure to apply for your booster shots regularly. (mutters) It's good stuff."));
~~~

~~~csharp
0276: 				Character.Act(L.T("放下一支", "sets one down"), L.T("这是本期的，以后要用，直接向我申请就行。", "This one is for this period. If you need more later, just apply to me directly."))
0277: 			};
0278: 			visit.Exit = Character.Act(L.T("合上文件夹", "closes the folder"), L.T("下个季度，下层区划拨到我的管辖范围，一定记得定期申请体力针剂。（小声嘟囔）这可是好东西。", "Next quarter the Lower Levels fall under my jurisdiction. Be sure to apply for your booster shots regularly. (mutters) It's good stuff."));
0279: 			visit.TalkOnly = true;
0280: 			visit.Gift = new string[1] { "@stamina" };
~~~

~~~csharp
0289: 		if (s.SideDone.Contains(4))
0290: 		{
0291: 			source = source.Append(L.T("研发部拆了赛纳普斯。本想做戒瘾的，但是未知成分太多了，只做出来了镇痛针。", "R&D took the Synaprest apart. They wanted an anti-addiction drug, but there were too many unknown compounds. All they got was a painkiller."));
0292: 		}
0293: 		if (s.Chapter >= 5)
~~~

~~~csharp
0293: 		if (s.Chapter >= 5)
0294: 		{
0295: 			source = source.Append(L.T("体力增强剂，记得按期申请。", "Stamina Booster: remember to apply on schedule."));
0296: 		}
0297: 		IEnumerable<string> enumerable = new string[4] { "large_purple_injector", "phagimycin_pill", "smelling_salt", "zerochew" }.AsEnumerable();
~~~

~~~csharp
0308: 		visit.Lines = new string[2]
0309: 		{
0310: 			Engine.Any(L.T("市场规范部，例行走访。", "Market Compliance. Routine visit."), L.T("渠道检查，几分钟就好。", "Channel inspection. Just a few minutes."), L.T("伊莎·兰登。照例检查。", "Isa Langdon. The usual check.")),
0311: 			Engine.Any(source.ToArray())
0312: 		};
~~~

~~~csharp
0311: 			Engine.Any(source.ToArray())
0312: 		};
0313: 		visit.Exit = Character.Act(L.T("合上文件夹", "closes the folder"), Bye);
0314: 		visit.Discount = ((s.Chapter >= 3) ? 30 : 20);
0315: 		visit.Stock = enumerable2.ToArray();
~~~

~~~csharp
0316: 		visit.Extra = enumerable.ToArray();
0317: 		visit.Both = true;
0318: 		visit.Wrong = L.T("这次只收化学品。", "Only chemicals this time.");
0319: 		return visit;
0320: 	}
~~~

## CorpPresence/Engine.cs

~~~csharp
0129: 			client.SetBudget(Settings.AgentBudget);
0130: 			client.willAcceptItem = (flag ? new Func<GameItem, bool>(c.DailyAccepts) : new Func<GameItem, bool>(c.Accepts));
0131: 			client.placedWrongItemWhenSellingToDialogue = new Dialogue().SetText(c.Name, v.Wrong);
0132: 		}
0133: 		if (!v.TalkOnly && !v.Buy)
~~~

~~~csharp
0135: 			client.SetWholesale(v.Discount);
0136: 		}
0137: 		Dialogue val2 = new Dialogue().SetText(c.Name, v.Lines[0]);
0138: 		Dialogue val3 = val2;
0139: 		for (int i = 1; i < v.Lines.Length; i++)
~~~

~~~csharp
0139: 		for (int i = 1; i < v.Lines.Length; i++)
0140: 		{
0141: 			val3 = val3.NextDialogue().SetText(c.Name, v.Lines[i]);
0142: 		}
0143: 		CharacterVisitSession visitSession = new CharacterVisitSession(c.Id, characterState, client);
~~~

~~~csharp
0251: 		if (flag2)
0252: 		{
0253: 			Dialogue val4 = val3.NextDialogue().SetText(c.Name, v.Ask);
0254: 			bool choiceMade = false;
0255: 			Choice[] choices = v.Choices;
~~~

~~~csharp
0257: 			{
0258: 				Action then = choice.Then;
0259: 				Dialogue val5 = new Dialogue().SetText("PLAYER", choice.Label);
0260: 				Dialogue val6 = val5.NextDialogue().SetText(c.Name, choice.Reply);
0261: 				if (v.TalkOnly)
~~~

~~~csharp
0258: 				Action then = choice.Then;
0259: 				Dialogue val5 = new Dialogue().SetText("PLAYER", choice.Label);
0260: 				Dialogue val6 = val5.NextDialogue().SetText(c.Name, choice.Reply);
0261: 				if (v.TalkOnly)
0262: 				{
~~~

~~~csharp
0261: 				if (v.TalkOnly)
0262: 				{
0263: 					val6.NextDialogue().SetText(c.Name, v.Exit).SetEndAction(endAction);
0264: 				}
0265: 				else
~~~

~~~csharp
0295: 		if (v.TalkOnly && !flag2)
0296: 		{
0297: 			val3.NextDialogue().SetText(c.Name, v.Exit).SetEndAction(endAction);
0298: 		}
0299: 		client.mainDialogue = val2;
~~~

~~~csharp
0309: 	{
0310: 		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
0311: 		return new Dialogue().SetText(c.Name, line).SetEndAction((Action)delegate
0312: 		{
0313: 			if (!visitSession.TryFinish())
~~~

## CorpPresence/Milo.cs

~~~csharp
0014: 	public override string Id => "milo";
0015: 
0016: 	public override string Name => L.T("米洛", "Milo");
0017: 
0018: 	public override string Faction => "FACTION_LOWER_LEVEL";
~~~

~~~csharp
0026: 	public override int BuyGoodsNeeded => 1;
0027: 
0028: 	private static string WrongItem => L.T("我只收假海明威。", "I only take fake Hemingways.");
0029: 
0030: 	private static string[] Talk => new string[5]
~~~

~~~csharp
0030: 	private static string[] Talk => new string[5]
0031: 	{
0032: 		L.T("营养果没籽。吃完了，还得找我们买。", "Nutrifruit has no seeds. Finish one, and you buy another from us."),
0033: 		L.T("繁星果糊，最健康的。就是没人爱吃。", "Galaxy Blend is the healthiest. Nobody likes it, though."),
0034: 		L.T("四合一，洗头刷牙都用它。", "4 In 1 Wash. Hair, teeth, all of it."),
~~~

~~~csharp
0031: 	{
0032: 		L.T("营养果没籽。吃完了，还得找我们买。", "Nutrifruit has no seeds. Finish one, and you buy another from us."),
0033: 		L.T("繁星果糊，最健康的。就是没人爱吃。", "Galaxy Blend is the healthiest. Nobody likes it, though."),
0034: 		L.T("四合一，洗头刷牙都用它。", "4 In 1 Wash. Hair, teeth, all of it."),
0035: 		L.T("帕克斯在下层卖不动。太贵了。", "PAX doesn't sell in the Lower Levels. Too expensive."),
~~~

~~~csharp
0032: 		L.T("营养果没籽。吃完了，还得找我们买。", "Nutrifruit has no seeds. Finish one, and you buy another from us."),
0033: 		L.T("繁星果糊，最健康的。就是没人爱吃。", "Galaxy Blend is the healthiest. Nobody likes it, though."),
0034: 		L.T("四合一，洗头刷牙都用它。", "4 In 1 Wash. Hair, teeth, all of it."),
0035: 		L.T("帕克斯在下层卖不动。太贵了。", "PAX doesn't sell in the Lower Levels. Too expensive."),
0036: 		L.T("我们公司生产的韧棉纤维，小口径手枪完全挡得住。", "The Kotton fiber our company makes stops small-caliber pistol rounds cold.")
~~~

~~~csharp
0033: 		L.T("繁星果糊，最健康的。就是没人爱吃。", "Galaxy Blend is the healthiest. Nobody likes it, though."),
0034: 		L.T("四合一，洗头刷牙都用它。", "4 In 1 Wash. Hair, teeth, all of it."),
0035: 		L.T("帕克斯在下层卖不动。太贵了。", "PAX doesn't sell in the Lower Levels. Too expensive."),
0036: 		L.T("我们公司生产的韧棉纤维，小口径手枪完全挡得住。", "The Kotton fiber our company makes stops small-caliber pistol rounds cold.")
0037: 	};
~~~

~~~csharp
0034: 		L.T("四合一，洗头刷牙都用它。", "4 In 1 Wash. Hair, teeth, all of it."),
0035: 		L.T("帕克斯在下层卖不动。太贵了。", "PAX doesn't sell in the Lower Levels. Too expensive."),
0036: 		L.T("我们公司生产的韧棉纤维，小口径手枪完全挡得住。", "The Kotton fiber our company makes stops small-caliber pistol rounds cold.")
0037: 	};
0038: 
~~~

~~~csharp
0175: 			visit6.Lines = new string[2]
0176: 			{
0177: 				L.T("你好！我是米洛，帕斯泰尔公司跑下层区的推销员。", "Hi! I'm Milo, pastel Inc.'s sales rep for the Lower Levels."),
0178: 				L.T("帕斯泰尔几乎包揽了整个下层区的民生用品，哈哈，呃当然是除了达夫雷占据之外的部分。", "pastel supplies nearly all the everyday goods in the Lower Levels. Ha. Well, except Davre's share, of course.")
0179: 			};
~~~

~~~csharp
0176: 			{
0177: 				L.T("你好！我是米洛，帕斯泰尔公司跑下层区的推销员。", "Hi! I'm Milo, pastel Inc.'s sales rep for the Lower Levels."),
0178: 				L.T("帕斯泰尔几乎包揽了整个下层区的民生用品，哈哈，呃当然是除了达夫雷占据之外的部分。", "pastel supplies nearly all the everyday goods in the Lower Levels. Ha. Well, except Davre's share, of course.")
0179: 			};
0180: 			visit6.Exit = Character.Act(L.T("挥了挥手", "waves"), L.T("下周我再来！", "See you next week!"));
~~~

~~~csharp
0178: 				L.T("帕斯泰尔几乎包揽了整个下层区的民生用品，哈哈，呃当然是除了达夫雷占据之外的部分。", "pastel supplies nearly all the everyday goods in the Lower Levels. Ha. Well, except Davre's share, of course.")
0179: 			};
0180: 			visit6.Exit = Character.Act(L.T("挥了挥手", "waves"), L.T("下周我再来！", "See you next week!"));
0181: 			visit6.Discount = 50;
0182: 			visit6.Stock = new string[10] { "nutrifruit", "nutrifruit", "nutrifruit", "galaxy_blend", "galaxy_blend", "galaxy_blend", "shampoo", "shampoo", "toothpaste", "toothpaste" };
~~~

~~~csharp
0188: 			visit5.Lines = new string[2]
0189: 			{
0190: 				L.T("补贴到了，这箱不要钱。", "The subsidy came through. This box is free."),
0191: 				Character.Act(L.T("拍了拍箱子", "pats the box"), L.T("肚子饱了，我们这些“刁民”才不会闹事，上面比谁都清楚。", "Full bellies keep us \"troublemakers\" quiet. Upstairs knows that better than anyone."))
0192: 			};
~~~

~~~csharp
0189: 			{
0190: 				L.T("补贴到了，这箱不要钱。", "The subsidy came through. This box is free."),
0191: 				Character.Act(L.T("拍了拍箱子", "pats the box"), L.T("肚子饱了，我们这些“刁民”才不会闹事，上面比谁都清楚。", "Full bellies keep us \"troublemakers\" quiet. Upstairs knows that better than anyone."))
0192: 			};
0193: 			visit5.Exit = Character.Act(L.T("压低声音", "lowers his voice"), L.T("别说是我说的。", "You didn't hear it from me."));
~~~

~~~csharp
0191: 				Character.Act(L.T("拍了拍箱子", "pats the box"), L.T("肚子饱了，我们这些“刁民”才不会闹事，上面比谁都清楚。", "Full bellies keep us \"troublemakers\" quiet. Upstairs knows that better than anyone."))
0192: 			};
0193: 			visit5.Exit = Character.Act(L.T("压低声音", "lowers his voice"), L.T("别说是我说的。", "You didn't hear it from me."));
0194: 			visit5.Discount = 30;
0195: 			visit5.Gift = new string[4] { "pack_condom", "pack_condom", "pack_condom", "@gloves" };
~~~

~~~csharp
0202: 			visit4.Lines = new string[3]
0203: 			{
0204: 				L.T("嘿！我被调去海明威分部了，现在市场安全由我负责，想来点好烟吗？", "Hey! I got transferred to the Hemingway division. Market security is my job now. Want some good smokes?"),
0205: 				Character.Act(L.T("掏出一张色卡", "pulls out a color swatch"), Settings.VanillaBuyPrices ? L.T("有假烟就拿出来，我一点五倍收。", "Got fake cigarettes? Bring them out. I pay one and a half times.") : L.T("有假烟就拿出来，我两倍收。", "Got fake cigarettes? Bring them out. I pay double.")),
0206: 				L.T("正品我也带了，给你打八折。", "I brought the real stuff too. Twenty percent off for you.")
~~~

~~~csharp
0203: 			{
0204: 				L.T("嘿！我被调去海明威分部了，现在市场安全由我负责，想来点好烟吗？", "Hey! I got transferred to the Hemingway division. Market security is my job now. Want some good smokes?"),
0205: 				Character.Act(L.T("掏出一张色卡", "pulls out a color swatch"), Settings.VanillaBuyPrices ? L.T("有假烟就拿出来，我一点五倍收。", "Got fake cigarettes? Bring them out. I pay one and a half times.") : L.T("有假烟就拿出来，我两倍收。", "Got fake cigarettes? Bring them out. I pay double.")),
0206: 				L.T("正品我也带了，给你打八折。", "I brought the real stuff too. Twenty percent off for you.")
0207: 			};
~~~

~~~csharp
0204: 				L.T("嘿！我被调去海明威分部了，现在市场安全由我负责，想来点好烟吗？", "Hey! I got transferred to the Hemingway division. Market security is my job now. Want some good smokes?"),
0205: 				Character.Act(L.T("掏出一张色卡", "pulls out a color swatch"), Settings.VanillaBuyPrices ? L.T("有假烟就拿出来，我一点五倍收。", "Got fake cigarettes? Bring them out. I pay one and a half times.") : L.T("有假烟就拿出来，我两倍收。", "Got fake cigarettes? Bring them out. I pay double.")),
0206: 				L.T("正品我也带了，给你打八折。", "I brought the real stuff too. Twenty percent off for you.")
0207: 			};
0208: 			visit4.Exit = Character.Act(L.T("把色卡收回口袋", "pockets the swatch"), L.T("现在市场上的烟假的比真的还多。", "There are more fake cigarettes on the market than real ones these days."));
~~~

~~~csharp
0206: 				L.T("正品我也带了，给你打八折。", "I brought the real stuff too. Twenty percent off for you.")
0207: 			};
0208: 			visit4.Exit = Character.Act(L.T("把色卡收回口袋", "pockets the swatch"), L.T("现在市场上的烟假的比真的还多。", "There are more fake cigarettes on the market than real ones these days."));
0209: 			visit4.Both = true;
0210: 			visit4.Discount = 20;
~~~

~~~csharp
0218: 			visit3.Lines = new string[2]
0219: 			{
0220: 				L.T("托你的福，收上去的假烟够多，我升职了！", "Thanks to you, I turned in enough fakes to get promoted!"),
0221: 				Character.Act(L.T("放下两包烟", "sets down two packs"), L.T("正品，不要钱。", "Genuine, on the house."))
0222: 			};
~~~

~~~csharp
0219: 			{
0220: 				L.T("托你的福，收上去的假烟够多，我升职了！", "Thanks to you, I turned in enough fakes to get promoted!"),
0221: 				Character.Act(L.T("放下两包烟", "sets down two packs"), L.T("正品，不要钱。", "Genuine, on the house."))
0222: 			};
0223: 			visit3.Exit = Character.Act(L.T("看了看自己的新工牌", "glances at his new badge"), L.T("下次见，店长。", "See you, boss."));
~~~

~~~csharp
0221: 				Character.Act(L.T("放下两包烟", "sets down two packs"), L.T("正品，不要钱。", "Genuine, on the house."))
0222: 			};
0223: 			visit3.Exit = Character.Act(L.T("看了看自己的新工牌", "glances at his new badge"), L.T("下次见，店长。", "See you, boss."));
0224: 			visit3.TalkOnly = true;
0225: 			visit3.Gift = new string[2] { "@hemingway_genuine", "@hemingway_genuine" };
~~~

~~~csharp
0235: 			visit2.Lines = new string[3]
0236: 			{
0237: 				Character.Act(L.T("回头看了看门口", "glances back at the door"), L.T("分部印限量包装的标签戳，我顺了一个出来。", "I lifted one of the division's Limited Edition label stamps.")),
0238: 				L.T("一个标签戳能印八包。外面买不到的，一口价四百。", "One label stamp does eight packs. You can't get these outside. Four hundred, flat."),
0239: 				L.T("以后要，打电话找我，内部渠道，一期两个。", "Want more later? Call me. Inside channel, two per period.")
~~~

~~~csharp
0236: 			{
0237: 				Character.Act(L.T("回头看了看门口", "glances back at the door"), L.T("分部印限量包装的标签戳，我顺了一个出来。", "I lifted one of the division's Limited Edition label stamps.")),
0238: 				L.T("一个标签戳能印八包。外面买不到的，一口价四百。", "One label stamp does eight packs. You can't get these outside. Four hundred, flat."),
0239: 				L.T("以后要，打电话找我，内部渠道，一期两个。", "Want more later? Call me. Inside channel, two per period.")
0240: 			};
~~~

~~~csharp
0237: 				Character.Act(L.T("回头看了看门口", "glances back at the door"), L.T("分部印限量包装的标签戳，我顺了一个出来。", "I lifted one of the division's Limited Edition label stamps.")),
0238: 				L.T("一个标签戳能印八包。外面买不到的，一口价四百。", "One label stamp does eight packs. You can't get these outside. Four hundred, flat."),
0239: 				L.T("以后要，打电话找我，内部渠道，一期两个。", "Want more later? Call me. Inside channel, two per period.")
0240: 			};
0241: 			visit2.Exit = Character.Act(L.T("拉了拉衣领", "tugs at his collar"), L.T("我什么都没卖给你。", "I never sold you anything."));
~~~

~~~csharp
0239: 				L.T("以后要，打电话找我，内部渠道，一期两个。", "Want more later? Call me. Inside channel, two per period.")
0240: 			};
0241: 			visit2.Exit = Character.Act(L.T("拉了拉衣领", "tugs at his collar"), L.T("我什么都没卖给你。", "I never sold you anything."));
0242: 			visit2.Stock = new string[1] { "@hemingway_stamp" };
0243: 			return visit2;
~~~

~~~csharp
0248: 			visit.Lines = new string[2]
0249: 			{
0250: 				L.T("我们的主营业务当然是烟草", "Our main business is tobacco, of course."),
0251: 				Character.Act(L.T("放下一个盒子", "sets down a box"), L.T("但其实，我们也在卖戒瘾针（耸肩）。", "But honestly, we also sell the shot that helps you quit. (shrugs)"))
0252: 			};
~~~

~~~csharp
0249: 			{
0250: 				L.T("我们的主营业务当然是烟草", "Our main business is tobacco, of course."),
0251: 				Character.Act(L.T("放下一个盒子", "sets down a box"), L.T("但其实，我们也在卖戒瘾针（耸肩）。", "But honestly, we also sell the shot that helps you quit. (shrugs)"))
0252: 			};
0253: 			visit.Exit = Character.Act(L.T("笑了笑", "smiles"), L.T("帕斯泰尔应有尽有。", "pastel has everything."));
~~~

~~~csharp
0251: 				Character.Act(L.T("放下一个盒子", "sets down a box"), L.T("但其实，我们也在卖戒瘾针（耸肩）。", "But honestly, we also sell the shot that helps you quit. (shrugs)"))
0252: 			};
0253: 			visit.Exit = Character.Act(L.T("笑了笑", "smiles"), L.T("帕斯泰尔应有尽有。", "pastel has everything."));
0254: 			visit.Discount = 30;
0255: 			visit.Stock = new string[4] { "black_injector", "black_injector", "@hemingway_genuine", "@hemingway_genuine" };
~~~

~~~csharp
0266: 			Lines = new string[3]
0267: 			{
0268: 				L.T("分部要处理一批旧机器，说是型号淘汰了。", "The division is clearing out some old machines. Says the model's been retired."),
0269: 				Character.Act(L.T("压低声音", "lowers his voice"), L.T("其实还能用。拉去回收站之前，我想先问问你。", "They still work, honestly. Before they go to the scrapyard, I wanted to ask you.")),
0270: 				L.T("你要是有一台卷烟机，自己就能做散烟（小声）我会给你提供种子。", "With a cigarette roller, you could make loose cigarettes yourself. (quietly) I'll get you the seeds.")
~~~

~~~csharp
0267: 			{
0268: 				L.T("分部要处理一批旧机器，说是型号淘汰了。", "The division is clearing out some old machines. Says the model's been retired."),
0269: 				Character.Act(L.T("压低声音", "lowers his voice"), L.T("其实还能用。拉去回收站之前，我想先问问你。", "They still work, honestly. Before they go to the scrapyard, I wanted to ask you.")),
0270: 				L.T("你要是有一台卷烟机，自己就能做散烟（小声）我会给你提供种子。", "With a cigarette roller, you could make loose cigarettes yourself. (quietly) I'll get you the seeds.")
0271: 			},
~~~

~~~csharp
0268: 				L.T("分部要处理一批旧机器，说是型号淘汰了。", "The division is clearing out some old machines. Says the model's been retired."),
0269: 				Character.Act(L.T("压低声音", "lowers his voice"), L.T("其实还能用。拉去回收站之前，我想先问问你。", "They still work, honestly. Before they go to the scrapyard, I wanted to ask you.")),
0270: 				L.T("你要是有一台卷烟机，自己就能做散烟（小声）我会给你提供种子。", "With a cigarette roller, you could make loose cigarettes yourself. (quietly) I'll get you the seeds.")
0271: 			},
0272: 			Exit = Character.Act(L.T("挥了挥手", "waves"), L.T("走了！", "Off I go!")),
~~~

~~~csharp
0270: 				L.T("你要是有一台卷烟机，自己就能做散烟（小声）我会给你提供种子。", "With a cigarette roller, you could make loose cigarettes yourself. (quietly) I'll get you the seeds.")
0271: 			},
0272: 			Exit = Character.Act(L.T("挥了挥手", "waves"), L.T("走了！", "Off I go!")),
0273: 			TalkOnly = true,
0274: 			Ask = L.T("要不要？", "Want it?")
~~~

~~~csharp
0272: 			Exit = Character.Act(L.T("挥了挥手", "waves"), L.T("走了！", "Off I go!")),
0273: 			TalkOnly = true,
0274: 			Ask = L.T("要不要？", "Want it?")
0275: 		};
0276: 		v.Choices = new Choice[2]
~~~

~~~csharp
0276: 		v.Choices = new Choice[2]
0277: 		{
0278: 			new Choice(L.T("我要了。", "I'll take it."), L.T("给你搬进来了。种子我也塞了一包，别说是公司的。", "Brought it in for you. Slipped in a pack of seeds too. Don't say it came from the company."), delegate
0279: 			{
0280: 				v.Gift = new string[1] { "@craft_roller_set" };
~~~

~~~csharp
0280: 				v.Gift = new string[1] { "@craft_roller_set" };
0281: 			}),
0282: 			new Choice(L.T("不用了。", "No thanks."), L.T("行，那我拉走了。哪天想要，打电话订。", "Fine, I'll haul it off. If you change your mind, call and order one."), null)
0283: 		};
0284: 		return v;
~~~

~~~csharp
0291: 			Lines = new string[3]
0292: 			{
0293: 				L.T("店长我们的香烟生意怕是要黄了，上面追查追到我这儿了。", "Boss, I think our cigarette business is about to fall apart. Upstairs traced it all the way to me."),
0294: 				Character.Act(L.T("搓了搓手", "rubs his hands"), L.T($"不过我认识管这片的主管。{PastelCo.Bribe} 块，这事就当没发生。", $"But I know the supervisor for this sector. {PastelCo.Bribe}, and it never happened.")),
0295: 				L.T("条件是——以后你的烟，只能装进海明威的盒子里卖。", "The condition: from now on, your cigarettes only sell in Hemingway boxes.")
~~~

~~~csharp
0292: 			{
0293: 				L.T("店长我们的香烟生意怕是要黄了，上面追查追到我这儿了。", "Boss, I think our cigarette business is about to fall apart. Upstairs traced it all the way to me."),
0294: 				Character.Act(L.T("搓了搓手", "rubs his hands"), L.T($"不过我认识管这片的主管。{PastelCo.Bribe} 块，这事就当没发生。", $"But I know the supervisor for this sector. {PastelCo.Bribe}, and it never happened.")),
0295: 				L.T("条件是——以后你的烟，只能装进海明威的盒子里卖。", "The condition: from now on, your cigarettes only sell in Hemingway boxes.")
0296: 			},
~~~

~~~csharp
0293: 				L.T("店长我们的香烟生意怕是要黄了，上面追查追到我这儿了。", "Boss, I think our cigarette business is about to fall apart. Upstairs traced it all the way to me."),
0294: 				Character.Act(L.T("搓了搓手", "rubs his hands"), L.T($"不过我认识管这片的主管。{PastelCo.Bribe} 块，这事就当没发生。", $"But I know the supervisor for this sector. {PastelCo.Bribe}, and it never happened.")),
0295: 				L.T("条件是——以后你的烟，只能装进海明威的盒子里卖。", "The condition: from now on, your cigarettes only sell in Hemingway boxes.")
0296: 			},
0297: 			Exit = Character.Act(L.T("看了看表", "checks his watch"), L.T("还有三家要跑。", "Three more shops to hit.")),
~~~

~~~csharp
0295: 				L.T("条件是——以后你的烟，只能装进海明威的盒子里卖。", "The condition: from now on, your cigarettes only sell in Hemingway boxes.")
0296: 			},
0297: 			Exit = Character.Act(L.T("看了看表", "checks his watch"), L.T("还有三家要跑。", "Three more shops to hit.")),
0298: 			TalkOnly = true,
0299: 			Ask = L.T("给不给？", "Well?")
~~~

~~~csharp
0297: 			Exit = Character.Act(L.T("看了看表", "checks his watch"), L.T("还有三家要跑。", "Three more shops to hit.")),
0298: 			TalkOnly = true,
0299: 			Ask = L.T("给不给？", "Well?")
0300: 		};
0301: 		v.Choices = new Choice[2]
~~~

~~~csharp
0301: 		v.Choices = new Choice[2]
0302: 		{
0303: 			new Choice(L.T($"给你{PastelCo.Bribe}。", $"Here's {PastelCo.Bribe}."), L.T("分装机给你搬进来了。从今往后，你做的就是正品。", "Brought you a packaging machine. From now on, what you make is genuine."), delegate
0304: 			{
0305: 				if (PastelCo.PayBribe(deliver: false))
~~~

~~~csharp
0312: 				}
0313: 			}),
0314: 			new Choice(L.T("我再想想。", "Let me think."), L.T("别拖太久。想好了打电话。", "Don't take too long. Call when you've decided."), null)
0315: 		};
0316: 		return v;
~~~

~~~csharp
0319: 	public override Visit Daily(CharacterState s)
0320: 	{
0321: 		string[] options = ((s.Chapter >= 3) ? Append(Talk, L.T("海明威的假烟，比真的还多。", "There are more fake Hemingways than real ones.")) : Talk);
0322: 		string[] array = new string[6] { "nutrifruit", "galaxy_blend", "shampoo", "toothpaste", "box_tampon", "kotton_fabric" };
0323: 		if (s.Chapter >= 4)
~~~

~~~csharp
0331: 			visit.Lines = new string[2]
0332: 			{
0333: 				Engine.Any(L.T("又是我！", "Me again!"), L.T("米洛来了。", "Milo's here."), L.T("假烟留着没？", "Saved any fakes for me?"), L.T("海明威分部，例行回收。", "Hemingway division, routine collection.")),
0334: 				Engine.Any(options)
0335: 			};
~~~

~~~csharp
0334: 				Engine.Any(options)
0335: 			};
0336: 			visit.Exit = Engine.Any(Character.Act(L.T("挥了挥手", "waves"), L.T("走了！", "Off I go!")), Character.Act(L.T("看了看表", "checks his watch"), L.T("还有三家要跑。", "Three more shops to hit.")));
0337: 			visit.Both = true;
0338: 			visit.Discount = 20;
~~~

~~~csharp
0345: 		visit.Lines = new string[2]
0346: 		{
0347: 			Engine.Any(L.T("又是我！", "Me again!"), L.T("米洛来了。", "Milo's here."), L.T("今天进货吗？", "Stocking up today?")),
0348: 			Engine.Any(options)
0349: 		};
~~~

~~~csharp
0348: 			Engine.Any(options)
0349: 		};
0350: 		visit.Exit = Engine.Any(Character.Act(L.T("挥了挥手", "waves"), L.T("走了！", "Off I go!")), Character.Act(L.T("看了看表", "checks his watch"), L.T("还有三家要跑。", "Three more shops to hit.")));
0351: 		visit.Discount = 20;
0352: 		visit.Stock = Engine.Pick(array, 4, 6);
~~~

## CorpPresence/Seline.cs

~~~csharp
0007: 	public override string Id => "seline";
0008: 
0009: 	public override string Name => L.T("瑟琳", "Seline");
0010: 
0011: 	public override string Faction => "FACTION_UPPER_LEVEL";
~~~

~~~csharp
0023: 	private static string[] Talk => new string[6]
0024: 	{
0025: 		L.T("“同步”喷一点，它会帮你解开顾客钱包的扣子。", "A spritz of SYNC, and it'll unclasp your customers' wallets for you."),
0026: 		L.T("在上层区，只有一家顶尖奢侈品公司。", "In the Upper Levels, there is only one top luxury house."),
0027: 		L.T("没人会拒绝索拉纳。", "Nobody turns down Solana."),
~~~

~~~csharp
0024: 	{
0025: 		L.T("“同步”喷一点，它会帮你解开顾客钱包的扣子。", "A spritz of SYNC, and it'll unclasp your customers' wallets for you."),
0026: 		L.T("在上层区，只有一家顶尖奢侈品公司。", "In the Upper Levels, there is only one top luxury house."),
0027: 		L.T("没人会拒绝索拉纳。", "Nobody turns down Solana."),
0028: 		L.T("下层的人用不起这些。也用不着。", "People down here can't afford these. Or need them."),
~~~

~~~csharp
0025: 		L.T("“同步”喷一点，它会帮你解开顾客钱包的扣子。", "A spritz of SYNC, and it'll unclasp your customers' wallets for you."),
0026: 		L.T("在上层区，只有一家顶尖奢侈品公司。", "In the Upper Levels, there is only one top luxury house."),
0027: 		L.T("没人会拒绝索拉纳。", "Nobody turns down Solana."),
0028: 		L.T("下层的人用不起这些。也用不着。", "People down here can't afford these. Or need them."),
0029: 		L.T("科技展上那些新品化妆品，大半是索拉纳的下游实验室产出的。", "Most of the new cosmetics at the tech expo come out of Solana's downstream labs."),
~~~

~~~csharp
0026: 		L.T("在上层区，只有一家顶尖奢侈品公司。", "In the Upper Levels, there is only one top luxury house."),
0027: 		L.T("没人会拒绝索拉纳。", "Nobody turns down Solana."),
0028: 		L.T("下层的人用不起这些。也用不着。", "People down here can't afford these. Or need them."),
0029: 		L.T("科技展上那些新品化妆品，大半是索拉纳的下游实验室产出的。", "Most of the new cosmetics at the tech expo come out of Solana's downstream labs."),
0030: 		L.T("实验室的事，我不过问。", "I don't ask about the lab.")
~~~

~~~csharp
0027: 		L.T("没人会拒绝索拉纳。", "Nobody turns down Solana."),
0028: 		L.T("下层的人用不起这些。也用不着。", "People down here can't afford these. Or need them."),
0029: 		L.T("科技展上那些新品化妆品，大半是索拉纳的下游实验室产出的。", "Most of the new cosmetics at the tech expo come out of Solana's downstream labs."),
0030: 		L.T("实验室的事，我不过问。", "I don't ask about the lab.")
0031: 	};
~~~

~~~csharp
0028: 		L.T("下层的人用不起这些。也用不着。", "People down here can't afford these. Or need them."),
0029: 		L.T("科技展上那些新品化妆品，大半是索拉纳的下游实验室产出的。", "Most of the new cosmetics at the tech expo come out of Solana's downstream labs."),
0030: 		L.T("实验室的事，我不过问。", "I don't ask about the lab.")
0031: 	};
0032: 
~~~

~~~csharp
0095: 			visit3.Lines = new string[3]
0096: 			{
0097: 				L.T("瑟琳，索拉纳生物，品牌顾问。", "Seline. Solana Biogenics, brand consultant."),
0098: 				L.T("“永恒”“同步”“本源”，还用我继续介绍吗？（轻哼）。", "ETERNAL, SYNC, L'ESSENCE. Do I really need to go on? (scoffs)"),
0099: 				L.T("我们一般不跟下层做生意，不过听说你还算有能力。", "We don't usually do business down here. But I hear you're fairly capable.")
~~~

~~~csharp
0096: 			{
0097: 				L.T("瑟琳，索拉纳生物，品牌顾问。", "Seline. Solana Biogenics, brand consultant."),
0098: 				L.T("“永恒”“同步”“本源”，还用我继续介绍吗？（轻哼）。", "ETERNAL, SYNC, L'ESSENCE. Do I really need to go on? (scoffs)"),
0099: 				L.T("我们一般不跟下层做生意，不过听说你还算有能力。", "We don't usually do business down here. But I hear you're fairly capable.")
0100: 			};
~~~

~~~csharp
0097: 				L.T("瑟琳，索拉纳生物，品牌顾问。", "Seline. Solana Biogenics, brand consultant."),
0098: 				L.T("“永恒”“同步”“本源”，还用我继续介绍吗？（轻哼）。", "ETERNAL, SYNC, L'ESSENCE. Do I really need to go on? (scoffs)"),
0099: 				L.T("我们一般不跟下层做生意，不过听说你还算有能力。", "We don't usually do business down here. But I hear you're fairly capable.")
0100: 			};
0101: 			visit3.Exit = Character.Act(L.T("上下打量", "looks you up and down"), L.T("还不错。", "Not bad."));
~~~

~~~csharp
0099: 				L.T("我们一般不跟下层做生意，不过听说你还算有能力。", "We don't usually do business down here. But I hear you're fairly capable.")
0100: 			};
0101: 			visit3.Exit = Character.Act(L.T("上下打量", "looks you up and down"), L.T("还不错。", "Not bad."));
0102: 			visit3.Discount = 30;
0103: 			visit3.Stock = new string[2] { "skincare_cream", "skincare_cream" };
~~~

~~~csharp
0109: 			visit2.Lines = new string[2]
0110: 			{
0111: 				L.T("索拉纳在做市场调研，要收购一批奢侈品，看看别家在卖什么。", "Solana is doing market research. We're buying luxury goods to see what the competition sells."),
0112: 				Character.Act(L.T("看了看柜台", "glances at the counter"), L.T("按采购标准付钱，不用谈。次品就不必拿出来了。", "We pay by our purchasing standards, no haggling. Don't bother bringing out anything second-rate."))
0113: 			};
~~~

~~~csharp
0110: 			{
0111: 				L.T("索拉纳在做市场调研，要收购一批奢侈品，看看别家在卖什么。", "Solana is doing market research. We're buying luxury goods to see what the competition sells."),
0112: 				Character.Act(L.T("看了看柜台", "glances at the counter"), L.T("按采购标准付钱，不用谈。次品就不必拿出来了。", "We pay by our purchasing standards, no haggling. Don't bother bringing out anything second-rate."))
0113: 			};
0114: 			visit2.Exit = Character.Act(L.T("点点头", "nods"), L.T("好东西，记得先留给我。", "Remember to save the good stuff for me."));
~~~

~~~csharp
0112: 				Character.Act(L.T("看了看柜台", "glances at the counter"), L.T("按采购标准付钱，不用谈。次品就不必拿出来了。", "We pay by our purchasing standards, no haggling. Don't bother bringing out anything second-rate."))
0113: 			};
0114: 			visit2.Exit = Character.Act(L.T("点点头", "nods"), L.T("好东西，记得先留给我。", "Remember to save the good stuff for me."));
0115: 			visit2.Buy = true;
0116: 			visit2.Wrong = L.T("别糊弄我。", "Don't try to fool me.");
~~~

~~~csharp
0114: 			visit2.Exit = Character.Act(L.T("点点头", "nods"), L.T("好东西，记得先留给我。", "Remember to save the good stuff for me."));
0115: 			visit2.Buy = true;
0116: 			visit2.Wrong = L.T("别糊弄我。", "Don't try to fool me.");
0117: 			return visit2;
0118: 		}
~~~

~~~csharp
0122: 			visit.Lines = new string[2]
0123: 			{
0124: 				L.T("上层区最近都在找这个。", "Everyone in the Upper Levels is looking for this lately."),
0125: 				Character.Act(L.T("打开盒子", "opens a box"), L.T("我猜你用得上。", "I suspect you'll find a use for it."))
0126: 			};
~~~

~~~csharp
0123: 			{
0124: 				L.T("上层区最近都在找这个。", "Everyone in the Upper Levels is looking for this lately."),
0125: 				Character.Act(L.T("打开盒子", "opens a box"), L.T("我猜你用得上。", "I suspect you'll find a use for it."))
0126: 			};
0127: 			visit.Exit = Character.Act(L.T("看了眼门外", "glances at the door"), L.T("收好。", "Keep it safe."));
~~~

~~~csharp
0125: 				Character.Act(L.T("打开盒子", "opens a box"), L.T("我猜你用得上。", "I suspect you'll find a use for it."))
0126: 			};
0127: 			visit.Exit = Character.Act(L.T("看了眼门外", "glances at the door"), L.T("收好。", "Keep it safe."));
0128: 			visit.Discount = 50;
0129: 			visit.Stock = new string[2] { "pheromone_perfume", "pheromone_perfume" };
~~~

~~~csharp
0140: 		visit.Lines = new string[2]
0141: 		{
0142: 			Engine.Any(L.T("又见面了。", "We meet again."), L.T("路过。", "Just passing by."), L.T("生意还行？", "Business going well?")),
0143: 			Engine.Any(Talk)
0144: 		};
~~~

~~~csharp
0143: 			Engine.Any(Talk)
0144: 		};
0145: 		visit.Exit = Engine.Any(Character.Act(L.T("上下打量", "looks you up and down"), L.T("。", "")), Character.Act(L.T("看了看表", "checks her watch"), L.T("我该走了。", "I should go.")));
0146: 		visit.Both = true;
0147: 		visit.Discount = 20;
~~~

~~~csharp
0148: 		visit.Stock = Engine.Pick(array, 1, 2);
0149: 		visit.Extra = array;
0150: 		visit.Wrong = L.T("索拉纳的东西，我不收回。", "I don't buy back Solana products.");
0151: 		return visit;
0152: 	}
~~~

## CorpPresence/L.cs

~~~csharp
0028: 			try
0029: 			{
0030: 				string currentLocaleCode = LocHelper.GetCurrentLocaleCode();
0031: 				_en = !string.IsNullOrEmpty(currentLocaleCode) && !currentLocaleCode.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
0032: 			}
~~~

## CorpPresence/CarmichaelCo.cs

~~~csharp
0095: 	public override string Id => "carmicheal";
0096: 
0097: 	public override string Name => L.T("卡迈克尔制药", "Carmicheal Pharmaceutical");
0098: 
0099: 	public override string CharacterId => "isa";
~~~

~~~csharp
0101: 	public override long PhoneNumber => 22764L;
0102: 
0103: 	public override string[] TierNames => L.T(new string[3] { "登记药房", "指定药房", "合规伙伴" }, new string[3] { "Registered Pharmacy", "Designated Pharmacy", "Compliance Partner" });
0104: 
0105: 	public override string CardFlavor => L.T("白底青字，印着卡迈克尔的十字标和 22764。", "Teal on white, with the Carmicheal cross and 22764.");
~~~

~~~csharp
0103: 	public override string[] TierNames => L.T(new string[3] { "登记药房", "指定药房", "合规伙伴" }, new string[3] { "Registered Pharmacy", "Designated Pharmacy", "Compliance Partner" });
0104: 
0105: 	public override string CardFlavor => L.T("白底青字，印着卡迈克尔的十字标和 22764。", "Teal on white, with the Carmicheal cross and 22764.");
0106: 
0107: 	public override string DeskName => L.T("卡迈克尔客服", "Carmicheal Customer Service");
~~~

~~~csharp
0105: 	public override string CardFlavor => L.T("白底青字，印着卡迈克尔的十字标和 22764。", "Teal on white, with the Carmicheal cross and 22764.");
0106: 
0107: 	public override string DeskName => L.T("卡迈克尔客服", "Carmicheal Customer Service");
0108: 
0109: 	public override string DeskAsk => L.T("请问办理什么业务？", "How can I help you today?");
~~~

~~~csharp
0107: 	public override string DeskName => L.T("卡迈克尔客服", "Carmicheal Customer Service");
0108: 
0109: 	public override string DeskAsk => L.T("请问办理什么业务？", "How can I help you today?");
0110: 
0111: 	public override string[] FrontDesk => new string[2]
~~~

~~~csharp
0111: 	public override string[] FrontDesk => new string[2]
0112: 	{
0113: 		L.T("卡迈克尔制药客户服务部。请报您的药房登记号。", "Carmicheal Pharmaceutical customer service. Your pharmacy registration number, please."),
0114: 		L.T("卡迈克尔制药。本通话可能被录音。", "Carmicheal Pharmaceutical. This call may be recorded.")
0115: 	};
~~~

~~~csharp
0112: 	{
0113: 		L.T("卡迈克尔制药客户服务部。请报您的药房登记号。", "Carmicheal Pharmaceutical customer service. Your pharmacy registration number, please."),
0114: 		L.T("卡迈克尔制药。本通话可能被录音。", "Carmicheal Pharmaceutical. This call may be recorded.")
0115: 	};
0116: 
~~~

~~~csharp
0115: 	};
0116: 
0117: 	public override string FrontDeskDone => L.T("已登记。明早由专人配送。", "Logged. Our courier delivers tomorrow morning.");
0118: 
0119: 	public static int StaminaGap => Settings.Period("stamina", 10);
~~~

~~~csharp
0187: 			{
0188: 				identifier = "corp_specimen_escape",
0189: 				newsName = L.T("卡迈克尔制药收容物逃逸", "Carmicheal Specimen Breach"),
0190: 				displayName = L.T("收容物逃逸", "Specimen Breach"),
0191: 				newsDescription = L.T("上层区一片慌乱：卡迈克尔制药的实验室有危险生物逃逸。治安部提醒所有居民，见到没见过的生物立即举报。武器价格上涨。", "Panic in the Upper Levels: a dangerous organism has escaped a Carmicheal lab. Security urges all residents to report any unfamiliar creature at once. Weapon prices are up."),
~~~

~~~csharp
0188: 				identifier = "corp_specimen_escape",
0189: 				newsName = L.T("卡迈克尔制药收容物逃逸", "Carmicheal Specimen Breach"),
0190: 				displayName = L.T("收容物逃逸", "Specimen Breach"),
0191: 				newsDescription = L.T("上层区一片慌乱：卡迈克尔制药的实验室有危险生物逃逸。治安部提醒所有居民，见到没见过的生物立即举报。武器价格上涨。", "Panic in the Upper Levels: a dangerous organism has escaped a Carmicheal lab. Security urges all residents to report any unfamiliar creature at once. Weapon prices are up."),
0192: 				duration = 3,
~~~

~~~csharp
0189: 				newsName = L.T("卡迈克尔制药收容物逃逸", "Carmicheal Specimen Breach"),
0190: 				displayName = L.T("收容物逃逸", "Specimen Breach"),
0191: 				newsDescription = L.T("上层区一片慌乱：卡迈克尔制药的实验室有危险生物逃逸。治安部提醒所有居民，见到没见过的生物立即举报。武器价格上涨。", "Panic in the Upper Levels: a dangerous organism has escaped a Carmicheal lab. Security urges all residents to report any unfamiliar creature at once. Weapon prices are up."),
0192: 				duration = 3,
0193: 				importance = 8,
~~~

~~~csharp
0237: 				TraceEscape("尚未到首次出现日：新闻排期日的次日开店才发放");
0238: 			}
0239: 			else if (SpawnEscapee(L.T("柜台底下有什么东西在动。", "Something is moving under the counter.")))
0240: 			{
0241: 				cState.CreatureGiven = true;
~~~

~~~csharp
0268: 		else if ((long)dayCounter - (long)cState.CreatureLostDay >= 5)
0269: 		{
0270: 			if (SpawnEscapee(L.T("柜台底下又有东西在动。看来逃出来的不止一只。", "Something is moving under the counter again. More than one got out, it seems.")))
0271: 			{
0272: 				cState.CreatureLostDay = -1;
~~~

~~~csharp
0481: 		if (cState == null)
0482: 		{
0483: 			return L.T("存档未就绪", "Save not ready");
0484: 		}
0485: 		if (!cState.Licensed || ((id == "@isomer_reactor" || id == "@sb_handbook") && !ReactorOpen))
~~~

~~~csharp
0485: 		if (!cState.Licensed || ((id == "@isomer_reactor" || id == "@sb_handbook") && !ReactorOpen))
0486: 		{
0487: 			return L.T("还没有开通", "Not available yet");
0488: 		}
0489: 		string text = Services.OrderBlocked(c);
~~~

~~~csharp
0531: 			if (identifier == "unlicensed_phagimycin_pill")
0532: 			{
0533: 				return L.T("无证仿制", "Unlicensed copy");
0534: 			}
0535: 			if (identifier != "large_purple_injector")
~~~

~~~csharp
0539: 			if (item.IsTag("ISSUE_LABEL") || item.IsTag("ISSUE_COLOR") || item.IsTag("ISSUE_SERIAL") || item.IsTag("ISSUE_BODY"))
0540: 			{
0541: 				return L.T("假冒", "Counterfeit");
0542: 			}
0543: 			if (InsInjectorHelper.IsOnlyIssueRecalled(item))
~~~

~~~csharp
0543: 			if (InsInjectorHelper.IsOnlyIssueRecalled(item))
0544: 			{
0545: 				return L.T("召回批次", "Recalled batch");
0546: 			}
0547: 		}
~~~

## CorpPresence/Hooks.cs

~~~csharp
0593: 			if (instance != null)
0594: 			{
0595: 				instance.OpenPanelSingleButton(L.T("利维坦对付不了沟渠水。\n只能净化铁锈水和幽灵水。", "The Leviathan can't handle Gutterflow.\nIt only purifies Rustwater and Ghostwater."), L.T("确定", "OK"), (Action)null);
0596: 			}
0597: 			return false;
~~~

## CorpPresence/Cleanup.cs

~~~csharp
0087: 			if (instance2 != null)
0088: 			{
0089: 				instance2.Notify((num3 > 0) ? L.T("公司增强：有未清空的容器，请腾出库存空间后重试，暂不要卸载。", "CorpPresence: containers still hold items. Free inventory space and retry before uninstalling.") : L.T($"公司增强：已清理（电话 {num} 个，物品 {num2} 件），存档后即可卸载。", $"CorpPresence: cleaned up ({num} phone numbers, {num2} items). You can uninstall after this save."), "default");
0090: 			}
0091: 		}
~~~

## CorpPresence/Phone.cs

~~~csharp
0144: 			if (instance != null)
0145: 			{
0146: 				instance.Notify(L.T(company.Name + "拒接了你的电话：你在下层区“狗嫌人厌”，供货商都不愿和你来往。", company.Name + " won't take your call: you're a pariah in the Lower Levels, and no supplier wants to deal with you."), "default");
0147: 			}
0148: 		}
~~~

~~~csharp
0199: 		string text = frontDesk[new Random().Next(frontDesk.Length)];
0200: 		string text2 = (string.IsNullOrEmpty(c.DeskAsk) ? text : (text + "\n" + c.DeskAsk));
0201: 		Dialogue obj = new Dialogue().SetText(deskName, text2);
0202: 		Dialogue val2 = new Dialogue().SetText("PLAYER", L.T("浏览服务", "Browse services"));
0203: 		Dialogue val3 = new Dialogue().SetText("PLAYER", L.T("挂断", "Hang up"));
~~~

~~~csharp
0200: 		string text2 = (string.IsNullOrEmpty(c.DeskAsk) ? text : (text + "\n" + c.DeskAsk));
0201: 		Dialogue obj = new Dialogue().SetText(deskName, text2);
0202: 		Dialogue val2 = new Dialogue().SetText("PLAYER", L.T("浏览服务", "Browse services"));
0203: 		Dialogue val3 = new Dialogue().SetText("PLAYER", L.T("挂断", "Hang up"));
0204: 		val2.NextDialogue().SetText(deskName, L.T("好的，这就为您打开服务目录。", "Of course. Opening our service catalog now.")).SetStartAction((Action)delegate
~~~

~~~csharp
0201: 		Dialogue obj = new Dialogue().SetText(deskName, text2);
0202: 		Dialogue val2 = new Dialogue().SetText("PLAYER", L.T("浏览服务", "Browse services"));
0203: 		Dialogue val3 = new Dialogue().SetText("PLAYER", L.T("挂断", "Hang up"));
0204: 		val2.NextDialogue().SetText(deskName, L.T("好的，这就为您打开服务目录。", "Of course. Opening our service catalog now.")).SetStartAction((Action)delegate
0205: 		{
~~~

~~~csharp
0202: 		Dialogue val2 = new Dialogue().SetText("PLAYER", L.T("浏览服务", "Browse services"));
0203: 		Dialogue val3 = new Dialogue().SetText("PLAYER", L.T("挂断", "Hang up"));
0204: 		val2.NextDialogue().SetText(deskName, L.T("好的，这就为您打开服务目录。", "Of course. Opening our service catalog now.")).SetStartAction((Action)delegate
0205: 		{
0206: 			if (!opened && CallCurrent())
~~~

~~~csharp
0251: 			}
0252: 		});
0253: 		val3.NextDialogue().SetText(deskName, L.T("感谢来电，再见。", "Thank you for calling. Goodbye.")).SetEndAction((Action)delegate
0254: 		{
0255: 			if (hungUp || !CallCurrent())
~~~

~~~csharp
0270: 			}
0271: 		});
0272: 		obj.AddChoice(Compat.ChoiceIcon(), L.T("浏览服务", "Browse services"), "", val2, (Action)null, (Func<bool>)null);
0273: 		obj.AddChoice(Compat.ChoiceIcon(), L.T("挂断", "Hang up"), "", val3, (Action)null, (Func<bool>)null);
0274: 		return obj;
~~~

~~~csharp
0271: 		});
0272: 		obj.AddChoice(Compat.ChoiceIcon(), L.T("浏览服务", "Browse services"), "", val2, (Action)null, (Func<bool>)null);
0273: 		obj.AddChoice(Compat.ChoiceIcon(), L.T("挂断", "Hang up"), "", val3, (Action)null, (Func<bool>)null);
0274: 		return obj;
0275: 		bool CallCurrent()
~~~

## CorpPresence/Services.cs

~~~csharp
0032: 			if (Settings.RestockWeeks != 1)
0033: 			{
0034: 				return L.T("本期", "this period");
0035: 			}
0036: 			return L.T("本周", "this week");
~~~

~~~csharp
0034: 				return L.T("本期", "this period");
0035: 			}
0036: 			return L.T("本周", "this week");
0037: 		}
0038: 	}
~~~

~~~csharp
0259: 		if (currentStore == null)
0260: 		{
0261: 			return L.T("存档未就绪", "Save not ready");
0262: 		}
0263: 		if (c != null)
~~~

~~~csharp
0266: 			if (companyState == null || companyState.Orders == null || companyState.Visitors == null || companyState.LimitedBought == null)
0267: 			{
0268: 				return L.T("公司数据未就绪，尚未付款。", "Company data is not ready. No payment attempted.");
0269: 			}
0270: 			if (Settings.MaliceMode)
~~~

~~~csharp
0274: 			if (!SaveSlot.CanWrite("CorpPresence") || (Settings.MaliceMode && !SaveSlot.CanWrite("CorpPresence.Malice")))
0275: 			{
0276: 				return L.T("公司数据未就绪，尚未付款。请先检查存档读取错误。", "Company data is not ready. No payment attempted. Check the save-load error first.");
0277: 			}
0278: 		}
~~~

~~~csharp
0298: 				{
0299: 				}
0300: 				return L.T("付款状态异常，请先核对余额，避免重复付款。", "Payment status is uncertain. Check your balance before trying again.");
0301: 			}
0302: 			if (text != null)
~~~

~~~csharp
0311: 		if (!currentStore.CanPay(amount))
0312: 		{
0313: 			return L.T("现金不够", "Not enough cash");
0314: 		}
0315: 		CompanyMoney.Pay(currentStore, amount);
~~~

~~~csharp
0322: 		if (companyState.DealerJoined)
0323: 		{
0324: 			return L.T("已经是合作商了", "Already a dealer");
0325: 		}
0326: 		string text = Pay(c, PastelCo.DealerFee, "dealer");
~~~

~~~csharp
0517: 			if (num > 0)
0518: 			{
0519: 				NightLog(L.T($"柜台上的 {num} 只送货箱已收进背包。", $"{num} delivery crate(s) left on the counter were moved into your bag."));
0520: 			}
0521: 			if (num2 > 0)
~~~

~~~csharp
0521: 			if (num2 > 0)
0522: 			{
0523: 				NightLog(L.T($"背包放不下，柜台上还有 {num2} 只送货箱，过夜会被清掉。", $"Your bag is full: {num2} delivery crate(s) remain on the counter and will be cleared overnight."), "9c3738");
0524: 			}
0525: 		}
~~~

~~~csharp
0532: 	private static void Nightly(Company c, CompanyState s, int fee, string kind, int day)
0533: 	{
0534: 		string text = ((!(kind == "vip")) ? ((c is SpacerCo) ? L.T("街坊互助", "Mutual Aid") : L.T("下层客流", "Lower Foot Traffic")) : L.T("贵宾引荐", "VIP Referral"));
0535: 		Visitor item = new Visitor
0536: 		{
~~~

~~~csharp
0549: 				s.VipOn = false;
0550: 			}
0551: 			NightLog(L.T(c.Name + "的" + text + "未结算，已暂停：" + text2, c.Name + " " + text + " paused: " + text2), "9c3738");
0552: 			try
0553: 			{
~~~

~~~csharp
0561: 		}
0562: 		s.Visitors.Add(item);
0563: 		string text3 = ((!(kind == "vip")) ? ((c is SpacerCo) ? L.T("明天会多来几位星际客", "More Spacers will come by tomorrow") : L.T("明天会多来几位下层区顾客", "More Lower Levels customers will come by tomorrow")) : L.T("明天会有一位上层区贵宾光顾店铺", "An Upper Levels VIP will visit tomorrow"));
0564: 		NightLog(L.T($"{c.Name}{text}已付 {fee} 信用点，{text3}。", $"{c.Name} {text}: paid {fee} credits. {text3}."));
0565: 		try
~~~

~~~csharp
0562: 		s.Visitors.Add(item);
0563: 		string text3 = ((!(kind == "vip")) ? ((c is SpacerCo) ? L.T("明天会多来几位星际客", "More Spacers will come by tomorrow") : L.T("明天会多来几位下层区顾客", "More Lower Levels customers will come by tomorrow")) : L.T("明天会有一位上层区贵宾光顾店铺", "An Upper Levels VIP will visit tomorrow"));
0564: 		NightLog(L.T($"{c.Name}{text}已付 {fee} 信用点，{text3}。", $"{c.Name} {text}: paid {fee} credits. {text3}."));
0565: 		try
0566: 		{
~~~

~~~csharp
0591: 				});
0592: 				s.WeekSales = 0.0;
0593: 				NightLog(L.T($"{c.Name}返点 {num2} 信用点，送货员明天带来。", $"{c.Name} rebate: {num2} credits. The courier brings it tomorrow."), "5c6f46");
0594: 			}
0595: 			else
~~~

~~~csharp
0627: 		if (s == null || s.Orders == null || s.LimitedBought == null)
0628: 		{
0629: 			return L.T("存档未就绪", "Save not ready");
0630: 		}
0631: 		if (s.LastOrderDay == Day)
~~~

~~~csharp
0631: 		if (s.LastOrderDay == Day)
0632: 		{
0633: 			return L.T("今天已经下过单了", "Already ordered today");
0634: 		}
0635: 		string text = OrderBlocked(c);
~~~

~~~csharp
0640: 		if (cart == null || unitPrice == null)
0641: 		{
0642: 			return L.T("订单未就绪，尚未扣款。", "Order not ready. You have not been charged.");
0643: 		}
0644: 		KeyValuePair<CatalogEntry, int>[] array = cart.ToArray();
~~~

~~~csharp
0645: 		if (array.Any((KeyValuePair<CatalogEntry, int> kv) => kv.Key == null || kv.Value <= 0 || kv.Key.BaseValue() < 0))
0646: 		{
0647: 			return L.T("有货品暂不可用，请稍后再试。尚未扣款。", "Some items are unavailable right now. Try again later. You have not been charged.");
0648: 		}
0649: 		CatalogEntry key = array.FirstOrDefault((KeyValuePair<CatalogEntry, int> kv) => kv.Value > Left(s, kv.Key)).Key;
~~~

~~~csharp
0650: 		if (key != null)
0651: 		{
0652: 			return L.T($"{key.Name()}超出限购，还能订 {Left(s, key)} 份", $"{key.Name()} is over the limit. You can order {Left(s, key)} more");
0653: 		}
0654: 		int total = 0;
~~~

~~~csharp
0670: 			if (string.IsNullOrEmpty(id))
0671: 			{
0672: 				return L.T("货品标识未就绪，尚未扣款。", "Item identifier not ready. You have not been charged.");
0673: 			}
0674: 			string text2 = QuotaCounts.Key(id, windowDays, day, periodKey);
~~~

~~~csharp
0675: 			if (!QuotaCounts.TryAdd(s.LimitedBought, text2, keyValuePair.Value, out var _))
0676: 			{
0677: 				return L.T("历史订购计数异常或超出范围，尚未扣款。", "Purchase history is invalid or out of range. You have not been charged.");
0678: 			}
0679: 			if (!OrderAmounts.TryAdd(ref total, unitPrice(keyValuePair.Key), keyValuePair.Value, bundle, out var itemCount))
~~~

~~~csharp
0679: 			if (!OrderAmounts.TryAdd(ref total, unitPrice(keyValuePair.Key), keyValuePair.Value, bundle, out var itemCount))
0680: 			{
0681: 				return L.T("订单数量或金额无效，尚未扣款。", "Invalid order quantity or amount. You have not been charged.");
0682: 			}
0683: 			for (int num3 = 0; num3 < itemCount; num3++)
~~~

~~~csharp
0689: 		if (total <= 0)
0690: 		{
0691: 			return L.T("还没有选货", "Nothing selected yet");
0692: 		}
0693: 		string text3 = Pay(c, total, "order");
~~~

~~~csharp
0750: 			if (e.WindowDays <= 0)
0751: 			{
0752: 				return L.T($"\u3000{PeriodWord}剩 {num}", $"  {num} left {PeriodWord}");
0753: 			}
0754: 			return L.T($"\u3000剩余 {num}", $"  {num} left");
~~~

~~~csharp
0752: 				return L.T($"\u3000{PeriodWord}剩 {num}", $"  {num} left {PeriodWord}");
0753: 			}
0754: 			return L.T($"\u3000剩余 {num}", $"  {num} left");
0755: 		}
0756: 		return "";
~~~

~~~csharp
0845: 		if (companyState?.Orders == null)
0846: 		{
0847: 			return L.T("存档未就绪", "Save not ready");
0848: 		}
0849: 		if (Day < SwapNextDay(companyState))
~~~

~~~csharp
0849: 		if (Day < SwapNextDay(companyState))
0850: 		{
0851: 			return L.T($"第 {SwapNextDay(companyState)} 天才能再申请", $"You can apply again on day {SwapNextDay(companyState)}");
0852: 		}
0853: 		List<GameItem> list = FakesOwned().Take(6).ToList();
~~~

~~~csharp
0854: 		if (list.Count == 0)
0855: 		{
0856: 			return L.T("没有找到卡迈克尔产品的赝品", "No fake Carmicheal products found");
0857: 		}
0858: 		Order order = new Order
~~~

~~~csharp
0868: 			if (swapFee < 0 || num > int.MaxValue)
0869: 			{
0870: 				return L.T("换货费用无效，尚未扣款。", "Invalid exchange fee. You have not been charged.");
0871: 			}
0872: 			num2 = (int)num;
~~~

~~~csharp
0876: 				if (item == null || string.IsNullOrEmpty(text))
0877: 				{
0878: 					return L.T("换货明细未就绪，尚未扣款。", "Exchange details are not ready. You have not been charged.");
0879: 				}
0880: 				order.Items.Add(text);
~~~

~~~csharp
0883: 		catch
0884: 		{
0885: 			return L.T("换货明细未就绪，尚未扣款。", "Exchange details are not ready. You have not been charged.");
0886: 		}
0887: 		string text2 = Pay(c, num2, "swap");
~~~

~~~csharp
0947: 		if (companyState?.Orders == null)
0948: 		{
0949: 			return L.T("存档未就绪", "Save not ready");
0950: 		}
0951: 		if (c.Tier(companyState) < 2)
~~~

~~~csharp
0951: 		if (c.Tier(companyState) < 2)
0952: 		{
0953: 			return L.T(c.TierNames[1] + "才开放", "Opens at " + c.TierNames[1]);
0954: 		}
0955: 		if (Day < RefurbishNextDay(companyState))
~~~

~~~csharp
0955: 		if (Day < RefurbishNextDay(companyState))
0956: 		{
0957: 			return L.T($"第 {RefurbishNextDay(companyState)} 天才能再送", $"You can send again on day {RefurbishNextDay(companyState)}");
0958: 		}
0959: 		List<GameItem> list = RefurbishCandidates().Take(3).ToList();
~~~

~~~csharp
0960: 		if (list.Count == 0)
0961: 		{
0962: 			return L.T("没有能翻新的枪", "No guns to refurbish");
0963: 		}
0964: 		Order order = new Order
~~~

~~~csharp
0976: 				if (item == null || string.IsNullOrEmpty(item.identifier) || num2 < 0 || num2 >= 3)
0977: 				{
0978: 					return L.T("翻新明细未就绪，尚未扣款。", "Refurbishment details are not ready. You have not been charged.");
0979: 				}
0980: 				int num3 = MeridianCo.RefurbishFee(item);
~~~

~~~csharp
0982: 				if (num3 < 0 || num > int.MaxValue)
0983: 				{
0984: 					return L.T("翻新费用无效，尚未扣款。", "Invalid refurbishment fee. You have not been charged.");
0985: 				}
0986: 				order.Items.Add($"{item.identifier}|{num2 + 1}");
~~~

~~~csharp
0990: 		catch
0991: 		{
0992: 			return L.T("翻新明细未就绪，尚未扣款。", "Refurbishment details are not ready. You have not been charged.");
0993: 		}
0994: 		string text = Pay(c, num4, "refurbish");
~~~

~~~csharp
1053: 		if (companyState?.Visitors == null)
1054: 		{
1055: 			return L.T("存档未就绪", "Save not ready");
1056: 		}
1057: 		if (Day < VipNextDay(companyState))
~~~

~~~csharp
1057: 		if (Day < VipNextDay(companyState))
1058: 		{
1059: 			return L.T($"第 {VipNextDay(companyState)} 天才能再预约", $"You can book again on day {VipNextDay(companyState)}");
1060: 		}
1061: 		int day = Day;
~~~

~~~csharp
1088: 		if (companyState?.Visitors == null || companyState.LabList == null)
1089: 		{
1090: 			return L.T("存档未就绪", "Save not ready");
1091: 		}
1092: 		string text = OrderBlocked(c);
~~~

~~~csharp
1106: 		catch
1107: 		{
1108: 			return L.T("采购清单暂未就绪", "Procurement list not ready");
1109: 		}
1110: 		if (companyState.LabBooked)
~~~

~~~csharp
1110: 		if (companyState.LabBooked)
1111: 		{
1112: 			return L.T("这一期已经预约过了", "Already booked for this list");
1113: 		}
1114: 		companyState.Visitors.Add(new Visitor
~~~

~~~csharp
1131: 		if (companyState?.Orders == null)
1132: 		{
1133: 			return L.T("存档未就绪", "Save not ready");
1134: 		}
1135: 		string text = OrderBlocked(c);
~~~

~~~csharp
1146: 		if (companyState.GiftWeek == num)
1147: 		{
1148: 			return L.T("这一期已经领过了", "Already claimed for this period");
1149: 		}
1150: 		Order item;
~~~

~~~csharp
1154: 			if (string.IsNullOrEmpty(text3))
1155: 			{
1156: 				return L.T("赠礼暂未就绪", "Gift not ready");
1157: 			}
1158: 			item = new Order
~~~

~~~csharp
1165: 		catch
1166: 		{
1167: 			return L.T("赠礼暂未就绪", "Gift not ready");
1168: 		}
1169: 		companyState.Orders.Add(item);
~~~

~~~csharp
1186: 		if (companyState?.Orders == null)
1187: 		{
1188: 			return L.T("存档未就绪", "Save not ready");
1189: 		}
1190: 		if (!CarmichaelCo.StaminaOpen())
~~~

~~~csharp
1190: 		if (!CarmichaelCo.StaminaOpen())
1191: 		{
1192: 			return L.T("尚未向贵店配发", "Not yet allocated to your shop");
1193: 		}
1194: 		if (Day < StaminaNextDay(companyState))
~~~

~~~csharp
1194: 		if (Day < StaminaNextDay(companyState))
1195: 		{
1196: 			return L.T($"批次未到，第 {StaminaNextDay(companyState)} 天可申领", $"Batch not ready. Available on day {StaminaNextDay(companyState)}");
1197: 		}
1198: 		int day = Day;
~~~

~~~csharp
1226: 		if (companyState?.Orders == null || companyState.LimitedBought == null)
1227: 		{
1228: 			return L.T("存档未就绪", "Save not ready");
1229: 		}
1230: 		if (!DavreCo.LeviathanOpen)
~~~

~~~csharp
1230: 		if (!DavreCo.LeviathanOpen)
1231: 		{
1232: 			return L.T("还没有开通", "Not available yet");
1233: 		}
1234: 		string key = "leviathan@" + PeriodKey;
~~~

~~~csharp
1235: 		if ((companyState.LimitedBought.TryGetValue(key, out var value) ? value : 0) >= c.LeviathanLimit(companyState))
1236: 		{
1237: 			return L.T("这周的配额已经订完了", "This week's quota is used up");
1238: 		}
1239: 		if (price <= 0)
~~~

~~~csharp
1239: 		if (price <= 0)
1240: 		{
1241: 			return L.T("利维坦暂不可用，请稍后再试。尚未扣款。", "The Leviathan is unavailable right now. Try again later. You have not been charged.");
1242: 		}
1243: 		string text = OrderBlocked(c);
~~~

~~~csharp
1274: 		if (companyState?.Orders == null)
1275: 		{
1276: 			return L.T("存档未就绪", "Save not ready");
1277: 		}
1278: 		if (companyState.CanningOwned)
~~~

~~~csharp
1278: 		if (companyState.CanningOwned)
1279: 		{
1280: 			return L.T("已经办过了", "Already done");
1281: 		}
1282: 		if (!DavreCo.CanningOffered)
~~~

~~~csharp
1282: 		if (!DavreCo.CanningOffered)
1283: 		{
1284: 			return L.T("还没有开通", "Not available yet");
1285: 		}
1286: 		Order order;
~~~

~~~csharp
1290: 			if (list.Count == 0 || list.Any(string.IsNullOrEmpty))
1291: 			{
1292: 				return L.T("设备明细未就绪，尚未扣款。", "Equipment details not ready. You have not been charged.");
1293: 			}
1294: 			order = new Order
~~~

~~~csharp
1300: 		catch
1301: 		{
1302: 			return L.T("设备明细未就绪，尚未扣款。", "Equipment details not ready. You have not been charged.");
1303: 		}
1304: 		int canningFee = DavreCo.CanningFee;
~~~

~~~csharp
1325: 		if (companyState?.Orders == null)
1326: 		{
1327: 			return L.T("存档未就绪", "Save not ready");
1328: 		}
1329: 		if (!companyState.CanningOwned)
~~~

~~~csharp
1329: 		if (!companyState.CanningOwned)
1330: 		{
1331: 			return L.T("还没有开通", "Not available yet");
1332: 		}
1333: 		if (price <= 0 || string.IsNullOrEmpty(id) || DavreCo.CanningMachines == null || !DavreCo.CanningMachines.Any(((string Id, string Goods) m) => m.Id == id))
~~~

~~~csharp
1333: 		if (price <= 0 || string.IsNullOrEmpty(id) || DavreCo.CanningMachines == null || !DavreCo.CanningMachines.Any(((string Id, string Goods) m) => m.Id == id))
1334: 		{
1335: 			return L.T("设备暂不可用，尚未扣款。", "Equipment unavailable. You have not been charged.");
1336: 		}
1337: 		string text = OrderBlocked(c);
~~~

## CorpPresence/Forge.cs

~~~csharp
0298: 		{
0299: 			MachineHelper.PowerPanel(battery),
0300: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
0301: 			MachineHelper.Panel(L.T("金属锭", "Ingots"), (PixelElement)(object)input, "input", true),
0302: 			MachineHelper.Panel(L.T("钢胚", "Billets"), (PixelElement)(object)output, "output", true),
~~~

~~~csharp
0299: 			MachineHelper.PowerPanel(battery),
0300: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
0301: 			MachineHelper.Panel(L.T("金属锭", "Ingots"), (PixelElement)(object)input, "input", true),
0302: 			MachineHelper.Panel(L.T("钢胚", "Billets"), (PixelElement)(object)output, "output", true),
0303: 			MachineHelper.NotePanel(val)
~~~

~~~csharp
0300: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
0301: 			MachineHelper.Panel(L.T("金属锭", "Ingots"), (PixelElement)(object)input, "input", true),
0302: 			MachineHelper.Panel(L.T("钢胚", "Billets"), (PixelElement)(object)output, "output", true),
0303: 			MachineHelper.NotePanel(val)
0304: 		});
~~~

~~~csharp
0404: 		GameItem machine = null;
0405: 		MachineryRadioButton[] radios = (MachineryRadioButton[])(object)new MachineryRadioButton[Blades.Length];
0406: 		string[] array = L.T(new string[3] { "短刀", "军刀", "砍刀" }, new string[3] { "Dagger", "Saber", "Machete" });
0407: 		MachineryRadioGroup val2 = new MachineryRadioGroup();
0408: 		for (int i = 0; i < Blades.Length; i++)
~~~

~~~csharp
0422: 					}
0423: 				}
0424: 			}).SetDescription(L.T($"{Blades[i].Billets} 块钢胚，{Blades[i].Billets * 100} 毫升水", $"{Blades[i].Billets} billet(s), {Blades[i].Billets * 100} ml of water"));
0425: 			val2.Add(radios[i]);
0426: 		}
~~~

~~~csharp
0425: 			val2.Add(radios[i]);
0426: 		}
0427: 		MachineryButton val3 = new MachineryButton(L.T("锻造", "Forge"), (Action)null, -1, -1);
0428: 		PixelWindow val4 = MachineHelper.CreateMachineryWindow(true, (MachineryPanel[])(object)new MachineryPanel[6]
0429: 		{
~~~

~~~csharp
0429: 		{
0430: 			MachineHelper.PowerPanel(battery),
0431: 			MachineHelper.Panel(L.T("钢胚", "Billets"), (PixelElement)(object)input, "input", true),
0432: 			MachineHelper.Panel(L.T("水箱", "Quench tank"), (PixelElement)(object)water, "input", true),
0433: 			MachineHelper.Panel(L.T("款式", "Style"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[4]
~~~

~~~csharp
0430: 			MachineHelper.PowerPanel(battery),
0431: 			MachineHelper.Panel(L.T("钢胚", "Billets"), (PixelElement)(object)input, "input", true),
0432: 			MachineHelper.Panel(L.T("水箱", "Quench tank"), (PixelElement)(object)water, "input", true),
0433: 			MachineHelper.Panel(L.T("款式", "Style"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[4]
0434: 			{
~~~

~~~csharp
0431: 			MachineHelper.Panel(L.T("钢胚", "Billets"), (PixelElement)(object)input, "input", true),
0432: 			MachineHelper.Panel(L.T("水箱", "Quench tank"), (PixelElement)(object)water, "input", true),
0433: 			MachineHelper.Panel(L.T("款式", "Style"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[4]
0434: 			{
0435: 				(PixelElement)radios[0],
~~~

~~~csharp
0438: 				(PixelElement)val3
0439: 			}), "control", true),
0440: 			MachineHelper.Panel(L.T("成品", "Output"), (PixelElement)(object)output, "output", true),
0441: 			MachineHelper.NotePanel(val)
0442: 		});
~~~

~~~csharp
0496: 			if (list.Count < blade.Billets)
0497: 			{
0498: 				Tell(L.T($"钢胚不够，要 {blade.Billets} 块", $"Not enough billets: need {blade.Billets}"));
0499: 				return;
0500: 			}
~~~

~~~csharp
0503: 			if (childItem == null || WaterHelper.GetCurrentCapacityML(childItem) < num)
0504: 			{
0505: 				Tell(L.T($"水箱里的水不够，要 {num} 毫升", $"Not enough water in the tank: need {num} ml"));
0506: 				return;
0507: 			}
~~~

~~~csharp
0510: 			if (childItem2 == null || !PowerHelper.CanDrawPowerSource(childItem2, num2))
0511: 			{
0512: 				Tell(L.T($"电量不够，要 {num2} 电", $"Not enough charge: need {num2}"));
0513: 				return;
0514: 			}
~~~

~~~csharp
0526: 			{
0527: 				val2.Destroy();
0528: 				Tell(L.T("成品槽放不下了", "The output tray is full"));
0529: 				return;
0530: 			}
~~~

## CorpPresence/Tobacco.cs

~~~csharp
0291: 		GameGridInventory output = new GameGridInventory(6, 4);
0292: 		GameSlotInventory val = new GameSlotInventory();
0293: 		MachineryButton val2 = new MachineryButton(L.T("卷烟", "Roll"), (Action)null, -1, -1);
0294: 		PixelWindow contentWindow = MachineHelper.CreateMachineryWindow(true, (MachineryPanel[])(object)new MachineryPanel[5]
0295: 		{
~~~

~~~csharp
0295: 		{
0296: 			MachineHelper.PowerPanel(battery),
0297: 			MachineHelper.Panel(L.T("烟叶", "Leaves"), (PixelElement)(object)input, "input", true),
0298: 			MachineHelper.Panel(L.T("散烟", "Cigarettes"), (PixelElement)(object)output, "output", true),
0299: 			MachineHelper.Panel(L.T("操作", "Control"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[1] { (PixelElement)val2 }), "control", true),
~~~

~~~csharp
0296: 			MachineHelper.PowerPanel(battery),
0297: 			MachineHelper.Panel(L.T("烟叶", "Leaves"), (PixelElement)(object)input, "input", true),
0298: 			MachineHelper.Panel(L.T("散烟", "Cigarettes"), (PixelElement)(object)output, "output", true),
0299: 			MachineHelper.Panel(L.T("操作", "Control"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[1] { (PixelElement)val2 }), "control", true),
0300: 			MachineHelper.NotePanel(val)
~~~

~~~csharp
0297: 			MachineHelper.Panel(L.T("烟叶", "Leaves"), (PixelElement)(object)input, "input", true),
0298: 			MachineHelper.Panel(L.T("散烟", "Cigarettes"), (PixelElement)(object)output, "output", true),
0299: 			MachineHelper.Panel(L.T("操作", "Control"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[1] { (PixelElement)val2 }), "control", true),
0300: 			MachineHelper.NotePanel(val)
0301: 		});
~~~

~~~csharp
0389: 		{
0390: 			MachineHelper.PowerPanel(battery),
0391: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
0392: 			MachineHelper.Panel(L.T("散烟", "Cigarettes"), (PixelElement)(object)input, "input", true),
0393: 			MachineHelper.Panel(L.T("标签戳", "Label stamp"), (PixelElement)(object)label, "input", true),
~~~

~~~csharp
0390: 			MachineHelper.PowerPanel(battery),
0391: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
0392: 			MachineHelper.Panel(L.T("散烟", "Cigarettes"), (PixelElement)(object)input, "input", true),
0393: 			MachineHelper.Panel(L.T("标签戳", "Label stamp"), (PixelElement)(object)label, "input", true),
0394: 			MachineHelper.Panel(L.T("成品", "Output"), (PixelElement)(object)output, "output", true),
~~~

~~~csharp
0391: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
0392: 			MachineHelper.Panel(L.T("散烟", "Cigarettes"), (PixelElement)(object)input, "input", true),
0393: 			MachineHelper.Panel(L.T("标签戳", "Label stamp"), (PixelElement)(object)label, "input", true),
0394: 			MachineHelper.Panel(L.T("成品", "Output"), (PixelElement)(object)output, "output", true),
0395: 			MachineHelper.NotePanel(val)
~~~

~~~csharp
0392: 			MachineHelper.Panel(L.T("散烟", "Cigarettes"), (PixelElement)(object)input, "input", true),
0393: 			MachineHelper.Panel(L.T("标签戳", "Label stamp"), (PixelElement)(object)label, "input", true),
0394: 			MachineHelper.Panel(L.T("成品", "Output"), (PixelElement)(object)output, "output", true),
0395: 			MachineHelper.NotePanel(val)
0396: 		});
~~~

~~~csharp
0490: 		{
0491: 			string displayName = machine.GetDisplayName(false);
0492: 			val.name = L.T(displayName + "说明", displayName + " Instructions");
0493: 			val.shortDescription = L.T(displayName + "使用方法。", "How to use the " + displayName + ".");
0494: 			val.flavorText = L.T(text.Zh, text.En);
~~~

~~~csharp
0491: 			string displayName = machine.GetDisplayName(false);
0492: 			val.name = L.T(displayName + "说明", displayName + " Instructions");
0493: 			val.shortDescription = L.T(displayName + "使用方法。", "How to use the " + displayName + ".");
0494: 			val.flavorText = L.T(text.Zh, text.En);
0495: 			val.SetGameItemType("DOCUMENT");
~~~

~~~csharp
0492: 			val.name = L.T(displayName + "说明", displayName + " Instructions");
0493: 			val.shortDescription = L.T(displayName + "使用方法。", "How to use the " + displayName + ".");
0494: 			val.flavorText = L.T(text.Zh, text.En);
0495: 			val.SetGameItemType("DOCUMENT");
0496: 			val.EnableTag("IMPORTANT_TAG", false);
~~~

## CorpPresence/Brand.cs

~~~csharp
0112: 			{
0113: 				name = Clean(val.GetDisplayName(false))?.TrimEnd('。') ?? key;
0114: 				desc = Clean(string.IsNullOrEmpty(val.flavorText) ? val.shortDescription : val.flavorText) ?? "";
0115: 				icon = Tex.FromSprite(Goods.FindSprite(val.spritePath) ?? AtlasSprite(val) ?? Goods.VanillaByName(val.spritePath));
0116: 				flag = true;
~~~

## CorpPresence/Canning.cs

~~~csharp
0249: 		GameGridInventory output = new GameGridInventory(6, 4);
0250: 		GameSlotInventory val = new GameSlotInventory();
0251: 		MachineryButton val2 = new MachineryButton(L.T("绞肉", "Grind"), (Action)null, -1, -1);
0252: 		PixelWindow contentWindow = MachineHelper.CreateMachineryWindow(true, (MachineryPanel[])(object)new MachineryPanel[5]
0253: 		{
~~~

~~~csharp
0253: 		{
0254: 			MachineHelper.PowerPanel(battery),
0255: 			MachineHelper.Panel(L.T("生肉", "Raw meat"), (PixelElement)(object)input, "input", true),
0256: 			MachineHelper.Panel(L.T("肉糜", "Mince"), (PixelElement)(object)output, "output", true),
0257: 			MachineHelper.Panel(L.T("操作", "Control"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[1] { (PixelElement)val2 }), "control", true),
~~~

~~~csharp
0254: 			MachineHelper.PowerPanel(battery),
0255: 			MachineHelper.Panel(L.T("生肉", "Raw meat"), (PixelElement)(object)input, "input", true),
0256: 			MachineHelper.Panel(L.T("肉糜", "Mince"), (PixelElement)(object)output, "output", true),
0257: 			MachineHelper.Panel(L.T("操作", "Control"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[1] { (PixelElement)val2 }), "control", true),
0258: 			MachineHelper.NotePanel(val)
~~~

~~~csharp
0255: 			MachineHelper.Panel(L.T("生肉", "Raw meat"), (PixelElement)(object)input, "input", true),
0256: 			MachineHelper.Panel(L.T("肉糜", "Mince"), (PixelElement)(object)output, "output", true),
0257: 			MachineHelper.Panel(L.T("操作", "Control"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[1] { (PixelElement)val2 }), "control", true),
0258: 			MachineHelper.NotePanel(val)
0259: 		});
~~~

~~~csharp
0394: 		GameSlotInventory val = new GameSlotInventory();
0395: 		GameItem machine = null;
0396: 		MachineryRadioButton synthetic = new MachineryRadioButton(L.T("合成档", "Synthetic"), true, (Action<bool>)delegate(bool on)
0397: 		{
0398: 			if (on)
~~~

~~~csharp
0400: 				SetFresh(machine, on: false);
0401: 			}
0402: 		}).SetDescription(L.T("3 营养果 + 1 杂肉糜 → 4 罐“泥炭”", "3 nutrifruit + 1 scrap mince → 4 cans of Peat"));
0403: 		MachineryRadioButton fresh = new MachineryRadioButton(L.T("鲜肉档", "Fresh"), false, (Action<bool>)delegate(bool on)
0404: 		{
~~~

~~~csharp
0401: 			}
0402: 		}).SetDescription(L.T("3 营养果 + 1 杂肉糜 → 4 罐“泥炭”", "3 nutrifruit + 1 scrap mince → 4 cans of Peat"));
0403: 		MachineryRadioButton fresh = new MachineryRadioButton(L.T("鲜肉档", "Fresh"), false, (Action<bool>)delegate(bool on)
0404: 		{
0405: 			if (on)
~~~

~~~csharp
0407: 				SetFresh(machine, on: true);
0408: 			}
0409: 		}).SetDescription(L.T("2 精肉糜 + 1 营养果 → 1 罐鲜肉罐头", "2 prime mince + 1 nutrifruit → 1 can of fresh meat"));
0410: 		new MachineryRadioGroup().Add(synthetic).Add(fresh);
0411: 		PixelWindow val2 = MachineHelper.CreateMachineryWindow(true, (MachineryPanel[])(object)new MachineryPanel[7]
~~~

~~~csharp
0412: 		{
0413: 			MachineHelper.PowerPanel(battery),
0414: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
0415: 			MachineHelper.Panel(L.T("肉糜", "Mince"), (PixelElement)(object)mince, "input", true),
0416: 			MachineHelper.Panel(L.T("营养果", "Nutrifruit"), (PixelElement)(object)fruit, "input", true),
~~~

~~~csharp
0413: 			MachineHelper.PowerPanel(battery),
0414: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
0415: 			MachineHelper.Panel(L.T("肉糜", "Mince"), (PixelElement)(object)mince, "input", true),
0416: 			MachineHelper.Panel(L.T("营养果", "Nutrifruit"), (PixelElement)(object)fruit, "input", true),
0417: 			MachineHelper.Panel(L.T("档位", "Mode"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[2]
~~~

~~~csharp
0414: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
0415: 			MachineHelper.Panel(L.T("肉糜", "Mince"), (PixelElement)(object)mince, "input", true),
0416: 			MachineHelper.Panel(L.T("营养果", "Nutrifruit"), (PixelElement)(object)fruit, "input", true),
0417: 			MachineHelper.Panel(L.T("档位", "Mode"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[2]
0418: 			{
~~~

~~~csharp
0415: 			MachineHelper.Panel(L.T("肉糜", "Mince"), (PixelElement)(object)mince, "input", true),
0416: 			MachineHelper.Panel(L.T("营养果", "Nutrifruit"), (PixelElement)(object)fruit, "input", true),
0417: 			MachineHelper.Panel(L.T("档位", "Mode"), (PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[2]
0418: 			{
0419: 				(PixelElement)synthetic,
~~~

~~~csharp
0420: 				(PixelElement)fresh
0421: 			}), "control", true),
0422: 			MachineHelper.Panel(L.T("成品", "Output"), (PixelElement)(object)output, "output", true),
0423: 			MachineHelper.NotePanel(val)
0424: 		});
~~~

## CorpPresence/Company.cs

~~~csharp
0045: 			return TierNames[Tier(s) - 1];
0046: 		}
0047: 		return L.T("未签约", "Unsigned");
0048: 	}
0049: 
~~~

## CorpPresence/SilverBlood.cs

~~~csharp
0309: 			FishSpecies val = new FishSpecies();
0310: 			val.speciesID = "FISH_CORP_SILVERBLOOD";
0311: 			val.displayName = L.T("银血三叶蜥", "Silverblood Trilobite");
0312: 			val.flavorText = L.T("卡迈克尔实验室的编号标本。背甲分成三节，甲缝里透着银光。", "A numbered specimen from a Carmicheal lab. Its carapace splits into three lobes, silver light leaking from the seams.");
0313: 			val.fishIdentifier = "corp_presence_silverblood";
~~~

~~~csharp
0310: 			val.speciesID = "FISH_CORP_SILVERBLOOD";
0311: 			val.displayName = L.T("银血三叶蜥", "Silverblood Trilobite");
0312: 			val.flavorText = L.T("卡迈克尔实验室的编号标本。背甲分成三节，甲缝里透着银光。", "A numbered specimen from a Carmicheal lab. Its carapace splits into three lobes, silver light leaking from the seams.");
0313: 			val.fishIdentifier = "corp_presence_silverblood";
0314: 			val.groupCount = 0;
~~~

~~~csharp
0404: 	{
0405: 		RegisterSpecies();
0406: 		GameItem obj = Empty().SetName(Species.GetDisplayName()).SetSpriteAndShape(Species.spriteAtlas, Species.stageSprites[2]);
0407: 		obj.unitValue = 0L;
0408: 		AquacultureHelper.InitFishItem(obj, Species, (string)null);
~~~

~~~csharp
0672: 			if (tubes == 0)
0673: 			{
0674: 				Tell(L.T("店里放不下了，先腾点地方再采血。", "No room in the shop. Make some space before drawing blood."));
0675: 				return;
0676: 			}
~~~

~~~csharp
0782: 			throw new InvalidOperationException("公司液体资源尚未就绪");
0783: 		}
0784: 		AddLiquid("corp_isomer_raw", L.T("粗异构蛋白", "Raw Isomer Protein"), "9aa3b4", ref _ownedRaw);
0785: 		AddLiquid("corp_isomer", L.T("异构蛋白", "Isomer Protein"), "c8d0dc", ref _ownedIsomer);
0786: 	}
~~~

~~~csharp
0783: 		}
0784: 		AddLiquid("corp_isomer_raw", L.T("粗异构蛋白", "Raw Isomer Protein"), "9aa3b4", ref _ownedRaw);
0785: 		AddLiquid("corp_isomer", L.T("异构蛋白", "Isomer Protein"), "c8d0dc", ref _ownedIsomer);
0786: 	}
0787: 
~~~

~~~csharp
1008: 			meds.SentinelCured = true;
1009: 			RemoveSentinelPerk();
1010: 			return L.T("你注射了回春药剂。（3/3）\n哨兵崩溃综合征的症状消失了。你再也不需要免疫宁了。", "You inject the Rejuvenation Serum. (3/3)\nThe symptoms of Sentinel Collapse Syndrome are gone. You'll never need Immunivax again.");
1011: 		}
1012: 		if (meds.RejuvShots != 1)
~~~

~~~csharp
1012: 		if (meds.RejuvShots != 1)
1013: 		{
1014: 			return L.T("你注射了回春药剂。（2/3）\n症状几乎感觉不到了。这一针也当作打过免疫宁。", "You inject the Rejuvenation Serum. (2/3)\nYou can barely feel the symptoms. This shot also counts as Immunivax.");
1015: 		}
1016: 		return L.T("你注射了回春药剂。（1/3）\n症状减轻了。这一针也当作打过免疫宁。", "You inject the Rejuvenation Serum. (1/3)\nThe symptoms ease. This shot also counts as Immunivax.");
~~~

~~~csharp
1014: 			return L.T("你注射了回春药剂。（2/3）\n症状几乎感觉不到了。这一针也当作打过免疫宁。", "You inject the Rejuvenation Serum. (2/3)\nYou can barely feel the symptoms. This shot also counts as Immunivax.");
1015: 		}
1016: 		return L.T("你注射了回春药剂。（1/3）\n症状减轻了。这一针也当作打过免疫宁。", "You inject the Rejuvenation Serum. (1/3)\nThe symptoms ease. This shot also counts as Immunivax.");
1017: 	}
1018: 
~~~

~~~csharp
1144: 		{
1145: 			MachineHelper.PowerPanel(battery),
1146: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
1147: 			MachineHelper.Panel(L.T("溶液", "Solution"), (PixelElement)(object)tank, "input", true),
1148: 			MachineHelper.Panel(L.T("成品", "Output"), (PixelElement)(object)output, "output", true),
~~~

~~~csharp
1145: 			MachineHelper.PowerPanel(battery),
1146: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
1147: 			MachineHelper.Panel(L.T("溶液", "Solution"), (PixelElement)(object)tank, "input", true),
1148: 			MachineHelper.Panel(L.T("成品", "Output"), (PixelElement)(object)output, "output", true),
1149: 			MachineHelper.NotePanel(val)
~~~

~~~csharp
1146: 			MachineHelper.Panel(L.T("模组", "Modules"), (PixelElement)(object)modules, "module", true),
1147: 			MachineHelper.Panel(L.T("溶液", "Solution"), (PixelElement)(object)tank, "input", true),
1148: 			MachineHelper.Panel(L.T("成品", "Output"), (PixelElement)(object)output, "output", true),
1149: 			MachineHelper.NotePanel(val)
1150: 		});
~~~

~~~csharp
1299: 			if (delivered > 0)
1300: 			{
1301: 				Tell(L.T("门口放着一只没有寄件人的包裹。", "There's a parcel at the door with no sender."));
1302: 			}
1303: 		}
~~~

~~~csharp
1342: 			if (val3 != null)
1343: 			{
1344: 				val3.name = L.T("没署名的纸条", "Unsigned Note");
1345: 				val3.flavorText = L.T("感谢指令，通过，向你致谢。", "Gratitude directive: approved. Thank you.");
1346: 				val3.SetGameItemType("DOCUMENT");
~~~

~~~csharp
1343: 			{
1344: 				val3.name = L.T("没署名的纸条", "Unsigned Note");
1345: 				val3.flavorText = L.T("感谢指令，通过，向你致谢。", "Gratitude directive: approved. Thank you.");
1346: 				val3.SetGameItemType("DOCUMENT");
1347: 				val3.unitValue = 0L;
~~~

## CorpPresence/Goods.cs

~~~csharp
0060: 		public Action<GameItem> OnNew;
0061: 
0062: 		public string LName => L.T(Name, NameEn ?? Name);
0063: 
0064: 		public string LFlavor => L.T(Flavor, FlavorEn ?? Flavor);
~~~

~~~csharp
0062: 		public string LName => L.T(Name, NameEn ?? Name);
0063: 
0064: 		public string LFlavor => L.T(Flavor, FlavorEn ?? Flavor);
0065: 
0066: 		public string LDesc
~~~

~~~csharp
0070: 				if (Desc != null)
0071: 				{
0072: 					return L.T(Desc, DescEn ?? Desc);
0073: 				}
0074: 				return LFlavor;
~~~

~~~csharp
0594: 		try
0595: 		{
0596: 			item.SetName(d.LName);
0597: 		}
0598: 		catch
~~~

~~~csharp
0601: 		try
0602: 		{
0603: 			item.flavorText = ((d.Desc == null) ? "" : d.LFlavor);
0604: 		}
0605: 		catch
~~~

~~~csharp
0608: 		try
0609: 		{
0610: 			item.shortDescription = d.LDesc + MachineUpgrade.Status(item);
0611: 		}
0612: 		catch
~~~

~~~csharp
1296: 				}
1297: 				meds.ElixirUntil = Math.Max(meds.ElixirUntil, Day + 2);
1298: 				text = L.T("你抹上了名流精华。\n3 天内交易得到的声望 +50%。", "You put on the Socialite Essence.\nReputation gained from trades +50% for 3 days.");
1299: 				break;
1300: 			case 'c':
~~~

~~~csharp
1321: 				}
1322: 				ApplyCandle(PlayerStore.Instance?.storeClientManager);
1323: 				text = L.T("你点燃了索拉纳香薰蜡烛。\n3 天内上层区顾客的预算 +15%。", "You light the Solana Scented Candle.\nUpper Levels customers' budget +15% for 3 days.");
1324: 				break;
1325: 			}
~~~

~~~csharp
1332: 			}
1333: 			meds.StaminaShots++;
1334: 			text = ((Settings.VanillaServices && meds.StaminaShots > 2) ? L.T($"你注射了体力增强剂，但身体已经适应了。\n拾荒次数上限最多 +{2}，这一针没有再增加（设置“服务强度”为原版）。", $"You inject the Stamina Booster, but your body has adapted.\nScavenging attempts are capped at +{2}; this shot adds nothing more (Service strength is set to Vanilla).") : L.T($"你注射了体力增强剂。\n每天在垃圾场的拾荒次数上限 +1（已注射 {meds.StaminaShots} 针）。", $"You inject the Stamina Booster.\nDaily scavenging attempts in the Dumping Grounds +1 ({meds.StaminaShots} injected so far)."));
1335: 			break;
1336: 		case 9:
~~~

~~~csharp
1340: 			}
1341: 			meds.PainBlockDay = Day;
1342: 			text = L.T("你注射了痛觉屏蔽剂。\n今天拾荒受伤的几率减半。", "You inject the Pain Blocker.\nYour chance of getting hurt while scavenging is halved today.");
1343: 			break;
1344: 		case 5:
~~~

~~~csharp
1348: 			}
1349: 			meds.PatchUntil = Math.Max(meds.PatchUntil, Day + 2);
1350: 			text = L.T("你贴上了海明威戒烟贴。\n3 天内不受尼古丁戒断的交易惩罚。", "You apply the Hemingway Quit Patch.\nNo nicotine withdrawal trading penalty for 3 days.");
1351: 			break;
1352: 		case 4:
~~~

~~~csharp
1356: 			}
1357: 			meds.MaskReady = true;
1358: 			text = L.T("你敷上了夜间修护面膜。\n下一次夜间外出，第二天不会开店迟到。", "You put on the Night Repair Mask.\nAfter your next night out, you won't open late the next day.");
1359: 			break;
1360: 		case 12:
~~~

~~~csharp
1377: 					if (instance != null)
1378: 					{
1379: 						instance.Notify(L.T("店里放不下了，先腾点地方再拆。", "No room in the shop. Make some space first."), "default");
1380: 					}
1381: 					return;
~~~

~~~csharp
1386: 				}
1387: 			}
1388: 			text = L.T("你拆开了异构修复凝胶，得到 3 小管异构蛋白溶液。纯度各不相同，可以用原版的化学检测仪看。", "You split the isomer repair gel into 3 small tubes of isomer solution. Their purity varies; check them with a chemical scanner.");
1389: 			break;
1390: 		case 8:
~~~

~~~csharp
1423: 			if (instance3 != null)
1424: 			{
1425: 				instance3.OpenPanelSingleButton(text, L.T("确定", "OK"), (Action)null);
1426: 			}
1427: 		}
~~~

## CorpPresence/PastelCo.cs

~~~csharp
0146: 	public override string Id => "pastel";
0147: 
0148: 	public override string Name => L.T("帕斯泰尔公司", "pastel Inc.");
0149: 
0150: 	public override string CharacterId => "milo";
~~~

~~~csharp
0152: 	public override long PhoneNumber => 72783L;
0153: 
0154: 	public override string[] TierNames => L.T(new string[3] { "签约", "长期客户", "合作伙伴" }, new string[3] { "Signed", "Regular Client", "Partner" });
0155: 
0156: 	public override string CardFlavor => L.T("订货热线 72783。背面印着营养果的广告。", "Order hotline 72783. A Nutrifruit ad on the back.");
~~~

~~~csharp
0154: 	public override string[] TierNames => L.T(new string[3] { "签约", "长期客户", "合作伙伴" }, new string[3] { "Signed", "Regular Client", "Partner" });
0155: 
0156: 	public override string CardFlavor => L.T("订货热线 72783。背面印着营养果的广告。", "Order hotline 72783. A Nutrifruit ad on the back.");
0157: 
0158: 	public override string DeskName => L.T("帕斯泰尔前台", "pastel Front Desk");
~~~

~~~csharp
0156: 	public override string CardFlavor => L.T("订货热线 72783。背面印着营养果的广告。", "Order hotline 72783. A Nutrifruit ad on the back.");
0157: 
0158: 	public override string DeskName => L.T("帕斯泰尔前台", "pastel Front Desk");
0159: 
0160: 	public override string DeskAsk => L.T("看看今天的单子？", "Want to see today's list?");
~~~

~~~csharp
0158: 	public override string DeskName => L.T("帕斯泰尔前台", "pastel Front Desk");
0159: 
0160: 	public override string DeskAsk => L.T("看看今天的单子？", "Want to see today's list?");
0161: 
0162: 	public override CatalogEntry[] Catalog => Entries;
~~~

~~~csharp
0166: 	public override string[] FrontDesk => new string[3]
0167: 	{
0168: 		L.T("帕斯泰尔订货热线！今天要点什么？", "pastel order hotline! What'll it be today?"),
0169: 		L.T("您好，帕斯泰尔。营养果今天有货。", "Hello, pastel. Nutrifruit's in stock today."),
0170: 		L.T("帕斯泰尔，什么都有。您说。", "pastel. We've got everything. Go ahead.")
~~~

~~~csharp
0167: 	{
0168: 		L.T("帕斯泰尔订货热线！今天要点什么？", "pastel order hotline! What'll it be today?"),
0169: 		L.T("您好，帕斯泰尔。营养果今天有货。", "Hello, pastel. Nutrifruit's in stock today."),
0170: 		L.T("帕斯泰尔，什么都有。您说。", "pastel. We've got everything. Go ahead.")
0171: 	};
~~~

~~~csharp
0168: 		L.T("帕斯泰尔订货热线！今天要点什么？", "pastel order hotline! What'll it be today?"),
0169: 		L.T("您好，帕斯泰尔。营养果今天有货。", "Hello, pastel. Nutrifruit's in stock today."),
0170: 		L.T("帕斯泰尔，什么都有。您说。", "pastel. We've got everything. Go ahead.")
0171: 	};
0172: 
~~~

~~~csharp
0171: 	};
0172: 
0173: 	public override string FrontDeskDone => L.T("好嘞，明天就到！", "You got it. Arriving tomorrow!");
0174: 
0175: 	public static int DealerFee => Services.Fee("pastel", "dealer", 300);
~~~

~~~csharp
0237: 		try
0238: 		{
0239: 			item.SetName(L.T("海明威限量包装", "Hemingway Limited Edition"));
0240: 		}
0241: 		catch
~~~

~~~csharp
0251: 		try
0252: 		{
0253: 			item.flavorText = L.T(LimitedFlavor.Zh, LimitedFlavor.En);
0254: 		}
0255: 		catch
~~~

## CorpPresence/Malice/RebelStory.cs

~~~csharp
0054: 	private static RebelState R => MaliceState.Get()?.Rebel;
0055: 
0056: 	private static string OdinName => L.T("奥丁", "Odin");
0057: 
0058: 	public static bool Appeased
~~~

~~~csharp
0187: 			r.Ch5 = true;
0188: 			r.Active = 5;
0189: 			Notify(L.T("导师来了消息：他会替你去和那几家巨企谈。", "Word from the Mentor: he'll speak to the corporations on your behalf."));
0190: 		}
0191: 		if (r.Active != 5 || r.RepCert)
~~~

~~~csharp
0217: 			r.Active = 0;
0218: 			Envoy.GiveItem("lower_rep_cert");
0219: 			Notify(L.T("下层区的人联名推你做了代表：所有顾客带的钱 +50%。", "The Lower Levels have named you their representative: all customers carry 50% more money."));
0220: 		}
0221: 	}
~~~

~~~csharp
0229: 			return Envoy.Talk(kind, OdinName, "wanted4", new string[3]
0230: 			{
0231: 				L.T("你帮了我们不少。我想请你再帮一个忙。", "You've helped us a lot. I need one more favor."),
0232: 				L.T($"弄一瓶毒药给我们。弄不到的话，价值 {100}₵ 的梦尘和芬太片也行。", $"Get us a bottle of poison. If you can't, Dream Dust and Phanta Pills worth {100}₵ will do."),
0233: 				L.T("准备好了就打我们的电话，我会叫人来取。", "When it's ready, call us and I'll send someone.")
~~~

~~~csharp
0230: 			{
0231: 				L.T("你帮了我们不少。我想请你再帮一个忙。", "You've helped us a lot. I need one more favor."),
0232: 				L.T($"弄一瓶毒药给我们。弄不到的话，价值 {100}₵ 的梦尘和芬太片也行。", $"Get us a bottle of poison. If you can't, Dream Dust and Phanta Pills worth {100}₵ will do."),
0233: 				L.T("准备好了就打我们的电话，我会叫人来取。", "When it's ready, call us and I'll send someone.")
0234: 			}, null, null, delegate
~~~

~~~csharp
0231: 				L.T("你帮了我们不少。我想请你再帮一个忙。", "You've helped us a lot. I need one more favor."),
0232: 				L.T($"弄一瓶毒药给我们。弄不到的话，价值 {100}₵ 的梦尘和芬太片也行。", $"Get us a bottle of poison. If you can't, Dream Dust and Phanta Pills worth {100}₵ will do."),
0233: 				L.T("准备好了就打我们的电话，我会叫人来取。", "When it's ready, call us and I'll send someone.")
0234: 			}, null, null, delegate
0235: 			{
~~~

~~~csharp
0239: 			return Envoy.Talk(kind, OdinName, "wanted4", new string[3]
0240: 			{
0241: 				L.T("我们需要一批炸药，子午线的重火力也勉强可以。", "We need explosives. Meridian's heavy firepower would do in a pinch."),
0242: 				L.T($"子午线那边我知道你有关系可以打点：合作关系够的话 {200}₵，不然 {500}₵，他们会送一颗炸弹过来，我来买。", $"I know you have connections at Meridian, so they can be bought: {200}₵ if you're on good terms, {500}₵ otherwise. They'll send a bomb, and I'll buy it."),
0243: 				L.T($"不然的话，通过地下买家卖出价值 {1000}₵ 的武器。", $"Otherwise, sell {1000}₵ worth of weapons through the underground buyer.")
~~~

~~~csharp
0240: 			{
0241: 				L.T("我们需要一批炸药，子午线的重火力也勉强可以。", "We need explosives. Meridian's heavy firepower would do in a pinch."),
0242: 				L.T($"子午线那边我知道你有关系可以打点：合作关系够的话 {200}₵，不然 {500}₵，他们会送一颗炸弹过来，我来买。", $"I know you have connections at Meridian, so they can be bought: {200}₵ if you're on good terms, {500}₵ otherwise. They'll send a bomb, and I'll buy it."),
0243: 				L.T($"不然的话，通过地下买家卖出价值 {1000}₵ 的武器。", $"Otherwise, sell {1000}₵ worth of weapons through the underground buyer.")
0244: 			}, null, null, delegate
~~~

~~~csharp
0241: 				L.T("我们需要一批炸药，子午线的重火力也勉强可以。", "We need explosives. Meridian's heavy firepower would do in a pinch."),
0242: 				L.T($"子午线那边我知道你有关系可以打点：合作关系够的话 {200}₵，不然 {500}₵，他们会送一颗炸弹过来，我来买。", $"I know you have connections at Meridian, so they can be bought: {200}₵ if you're on good terms, {500}₵ otherwise. They'll send a bomb, and I'll buy it."),
0243: 				L.T($"不然的话，通过地下买家卖出价值 {1000}₵ 的武器。", $"Otherwise, sell {1000}₵ worth of weapons through the underground buyer.")
0244: 			}, null, null, delegate
0245: 			{
~~~

~~~csharp
0247: 			}, "FACTION_REVOLUTION");
0248: 		case "rebel:odinbomb":
0249: 			return Buyer(kind, "rebel_odin_buyer_bomb", new string[1] { L.T("子午线的货到了？给我。", "Meridian's package arrived? Hand it over.") }, Array.Empty<string>(), 5000, talkFirst: true);
0250: 		case "rebel:seller":
0251: 			return Seller(kind);
~~~

~~~csharp
0253: 		{
0254: 			bool flag = r.Active == 4;
0255: 			return Buyer(kind, "rebel_odin_buyer", new string[1] { flag ? L.T("老规矩：武器和医疗品，有多少收多少。", "Same as always: weapons and medical supplies, as much as you've got.") : L.T("大清缴以后伤员太多了。医疗品，有多少收多少。", "Too many wounded since the great crackdown. Medical supplies, as much as you've got.") }, flag ? new string[2] { "MEDICAL", "WEAPON" } : new string[1] { "MEDICAL" }, Settings.VanillaBudget(), talkFirst: false);
0256: 		}
0257: 		case "rebel:invite":
~~~

~~~csharp
0258: 			return Envoy.Talk(kind, OdinName, "wanted4", new string[2]
0259: 			{
0260: 				L.T("你拒绝了他们。我听说了。", "You turned them down. I heard."),
0261: 				L.T("加入我们吧。", "Join us.")
0262: 			}, L.T("怎么样？", "Well?"), new(string, string, Action)[2]
~~~

~~~csharp
0259: 			{
0260: 				L.T("你拒绝了他们。我听说了。", "You turned them down. I heard."),
0261: 				L.T("加入我们吧。", "Join us.")
0262: 			}, L.T("怎么样？", "Well?"), new(string, string, Action)[2]
0263: 			{
~~~

~~~csharp
0260: 				L.T("你拒绝了他们。我听说了。", "You turned them down. I heard."),
0261: 				L.T("加入我们吧。", "Join us.")
0262: 			}, L.T("怎么样？", "Well?"), new(string, string, Action)[2]
0263: 			{
0264: 				Envoy.Opt(L.T("我加入。", "I'm in."), L.T("拿着这个。", "Take this."), JoinRebels),
~~~

~~~csharp
0262: 			}, L.T("怎么样？", "Well?"), new(string, string, Action)[2]
0263: 			{
0264: 				Envoy.Opt(L.T("我加入。", "I'm in."), L.T("拿着这个。", "Take this."), JoinRebels),
0265: 				Envoy.Opt(L.T("让我再想想。", "Let me think about it."), L.T("想好了打电话叫我。", "Call me when you've decided."), delegate
0266: 				{
~~~

~~~csharp
0263: 			{
0264: 				Envoy.Opt(L.T("我加入。", "I'm in."), L.T("拿着这个。", "Take this."), JoinRebels),
0265: 				Envoy.Opt(L.T("让我再想想。", "Let me think about it."), L.T("想好了打电话叫我。", "Call me when you've decided."), delegate
0266: 				{
0267: 					r.Invited = true;
~~~

~~~csharp
0271: 			return Envoy.Talk(kind, OdinName, "wanted4", new string[2]
0272: 			{
0273: 				L.T("够了。我们准备好了。", "That's enough. We're ready."),
0274: 				L.T("三天后，我们去拔掉巨企在下层区的代理人站点。", "In three days, we take out the corporations' agent station in the Lower Levels.")
0275: 			}, null, null, delegate
~~~

~~~csharp
0272: 			{
0273: 				L.T("够了。我们准备好了。", "That's enough. We're ready."),
0274: 				L.T("三天后，我们去拔掉巨企在下层区的代理人站点。", "In three days, we take out the corporations' agent station in the Lower Levels.")
0275: 			}, null, null, delegate
0276: 			{
~~~

~~~csharp
0280: 			return Buyer(kind, "rebel_pickup", new string[2]
0281: 			{
0282: 				L.T("奥丁让我来取东西。", "Odin sent me to pick something up."),
0283: 				L.T("放这儿就行。", "Just put it here.")
0284: 			}, PickupTags(), 99999, talkFirst: false);
~~~

~~~csharp
0281: 			{
0282: 				L.T("奥丁让我来取东西。", "Odin sent me to pick something up."),
0283: 				L.T("放这儿就行。", "Just put it here.")
0284: 			}, PickupTags(), 99999, talkFirst: false);
0285: 		default:
~~~

~~~csharp
0292: 		if (!PickupOpen)
0293: 		{
0294: 			return L.T("现在没有要送的东西。", "Nothing needs picking up right now.");
0295: 		}
0296: 		if (Envoy.Booked("rebel:pickup"))
~~~

~~~csharp
0296: 		if (Envoy.Booked("rebel:pickup"))
0297: 		{
0298: 			return L.T("已经约好了，明天来人。", "Already booked: someone comes tomorrow.");
0299: 		}
0300: 		Envoy.Book("rebel:pickup", Day + 1);
~~~

~~~csharp
0455: 			case 1:
0456: 				Rebels.AddPoints(100, "反抗军第 1 章");
0457: 				Notify(L.T($"奥丁收到了东西。反抗军助力点 +{100}。", $"Odin got what he needed. +{100} support points."));
0458: 				break;
0459: 			case 2:
~~~

~~~csharp
0458: 				break;
0459: 			case 2:
0460: 				Notify(L.T("奥丁：“谢了。之后武器可能会涨价，囤点货吧。”", "Odin: “Thanks. Weapon prices might go up soon. Stock up.”"));
0461: 				QueueEvent("malice_raid_armory", L.T("革命军突袭治安部武器库", "Revolutionaries Raid Security Armory"), L.T("武器库遇袭", "Armory Raided"), L.T("革命军袭击了治安部的一座武器库。站内武器紧缺，价格上涨。", "Revolutionaries struck a Security armory. Weapons are scarce station-wide and prices are up."), 25, 3, 4);
0462: 				break;
~~~

~~~csharp
0459: 			case 2:
0460: 				Notify(L.T("奥丁：“谢了。之后武器可能会涨价，囤点货吧。”", "Odin: “Thanks. Weapon prices might go up soon. Stock up.”"));
0461: 				QueueEvent("malice_raid_armory", L.T("革命军突袭治安部武器库", "Revolutionaries Raid Security Armory"), L.T("武器库遇袭", "Armory Raided"), L.T("革命军袭击了治安部的一座武器库。站内武器紧缺，价格上涨。", "Revolutionaries struck a Security armory. Weapons are scarce station-wide and prices are up."), 25, 3, 4);
0462: 				break;
0463: 			case 3:
~~~

~~~csharp
0463: 			case 3:
0464: 				GiveMeridianCrate();
0465: 				Notify(L.T("奥丁送来一箱子午线的枪：清缴的时候那些治安部身上搜出来的。", "Odin sent a crate of Meridian guns, taken off Security officers during the crackdown."));
0466: 				break;
0467: 			}
~~~

~~~csharp
0484: 		if (!Ch2BribeOpen)
0485: 		{
0486: 			return L.T("现在不需要。", "Not needed right now.");
0487: 		}
0488: 		int num = Ch2BribePrice();
~~~

~~~csharp
0490: 		if (store.GetCash() < num)
0491: 		{
0492: 			return L.T("现金不够", "Not enough cash");
0493: 		}
0494: 		store.PayMoney(num, true);
~~~

~~~csharp
0516: 			GameItem val = DirectoryMaster.Item("c4", true);
0517: 			val.EnableTag("CORP_REBEL_BOMB", false);
0518: 			val.SetName(L.T("子午线的“样品”", "Meridian “Sample”"));
0519: 			val.shortDescription = L.T("子午线私下送来的炸药。奥丁会来买。", "Explosives Meridian sent on the quiet. Odin will come for it.");
0520: 			if (!StorePlacement.Place(val))
~~~

~~~csharp
0517: 			val.EnableTag("CORP_REBEL_BOMB", false);
0518: 			val.SetName(L.T("子午线的“样品”", "Meridian “Sample”"));
0519: 			val.shortDescription = L.T("子午线私下送来的炸药。奥丁会来买。", "Explosives Meridian sent on the quiet. Odin will come for it.");
0520: 			if (!StorePlacement.Place(val))
0521: 			{
~~~

~~~csharp
0522: 				ModLog.Warn("恶意巨企：店里所有库存都放满了，" + val.identifier + " 没送到");
0523: 			}
0524: 			Notify(L.T("子午线的人送来了一颗炸弹。明天奥丁来买。", "A Meridian courier dropped off a bomb. Odin comes for it tomorrow."));
0525: 		}
0526: 		catch (Exception ex)
~~~

~~~csharp
0536: 		r.Ch3Start = Day + 1;
0537: 		r.OdinBuyDue = Day + 4;
0538: 		QueueEvent("malice_crackdown", L.T("巨企协助治安部清缴反抗军", "Corporations Back Security Crackdown"), L.T("清缴", "Crackdown"), L.T("巨企协助治安部清缴反抗军：反抗军损失惨重，同时治安部得到大量补给。贪腐之下，大量武器流入市场。", "The corporations are backing a Security crackdown on the Resistance: heavy losses for the rebels, while Security gets a flood of supplies. Thanks to graft, plenty of weapons are leaking onto the market."), -25, 3, 1);
0539: 	}
0540: 
~~~

~~~csharp
0554: 		try
0555: 		{
0556: 			string text = L.T("子午线的人", "Meridian Man");
0557: 			StoreClient client = new StoreClient
0558: 			{
~~~

~~~csharp
0568: 			client.SetWholesale(20);
0569: 			bool placed = false;
0570: 			Dialogue val = new Dialogue().SetText(text, L.T("清缴“多出来”的货。你要的话便宜给你。", "Surplus from the crackdown. Cheap, if you want it."));
0571: 			val.SetEndAction((Action)delegate
0572: 			{
~~~

~~~csharp
0596: 			});
0597: 			client.mainDialogue = val;
0598: 			client.allDoneDialogue = new Dialogue().SetText(text, L.T("回见。", "Later.")).SetEndAction((Action)delegate
0599: 			{
0600: 				try
~~~

~~~csharp
0672: 		if (!CanCallOdin)
0673: 		{
0674: 			return L.T("现在不需要。", "Not needed right now.");
0675: 		}
0676: 		Envoy.Book("rebel:invite", Day + 1);
~~~

~~~csharp
0702: 		{
0703: 		}
0704: 		Notify(L.T("巨企意识到下层脱离掌控开始安抚下层区：所有订货九折，不再收订阅费。", "Realizing the Lower Levels are slipping out of their grip, the corporations are placating them: all orders 10% off, no more subscription fees."));
0705: 		QueueEvent("malice_devil_dies", L.T("恶魔之死", "Death of a Devil"), L.T("恶魔之死", "Death of a Devil"), L.T("联合站区管理处专员撒该·索恩在下层区遇袭身亡。治安部封锁了现场，武器价格上涨。", "Joint District Administration officer Zacchaeus Thorne was killed in an attack in the Lower Levels. Security has sealed the area; weapon prices are up."), 30, 3, 0);
0706: 	}
~~~

~~~csharp
0703: 		}
0704: 		Notify(L.T("巨企意识到下层脱离掌控开始安抚下层区：所有订货九折，不再收订阅费。", "Realizing the Lower Levels are slipping out of their grip, the corporations are placating them: all orders 10% off, no more subscription fees."));
0705: 		QueueEvent("malice_devil_dies", L.T("恶魔之死", "Death of a Devil"), L.T("恶魔之死", "Death of a Devil"), L.T("联合站区管理处专员撒该·索恩在下层区遇袭身亡。治安部封锁了现场，武器价格上涨。", "Joint District Administration officer Zacchaeus Thorne was killed in an attack in the Lower Levels. Security has sealed the area; weapon prices are up."), 30, 3, 0);
0706: 	}
0707: 
~~~

~~~csharp
0742: 		r.OdinRepLost = 0;
0743: 		r.RescueValue = 0;
0744: 		Notify(L.T("奥丁被救出来了。反抗军恢复了联系。", "Odin has been rescued. The Resistance is back in touch."));
0745: 	}
0746: 
~~~

~~~csharp
0786: 			{
0787: 				identifier = id,
0788: 				displayName = ((id == "rebel_pickup") ? L.T("反抗军联络员", "Resistance Runner") : OdinName),
0789: 				spriteName = ((id == "rebel_pickup") ? SpriteDict.GetRandomRevSpriteName() : "wanted4"),
0790: 				clientFaction = "FACTION_REVOLUTION",
~~~

~~~csharp
0800: 			client.CompleteClientCreation(false);
0801: 			client.SetBudget(budget);
0802: 			Dialogue val = new Dialogue().SetText(client.displayName, lines[0]);
0803: 			Dialogue val2 = val;
0804: 			for (int j = 1; j < lines.Length; j++)
~~~

~~~csharp
0804: 			for (int j = 1; j < lines.Length; j++)
0805: 			{
0806: 				val2 = val2.NextDialogue().SetText(client.displayName, lines[j]);
0807: 			}
0808: 			val2.SetEndAction((Action)delegate
~~~

~~~csharp
0826: 		{
0827: 		case 0:
0828: 			QueueEvent("malice_raid_armory", L.T("革命军突袭治安部武器库", "Revolutionaries Raid Security Armory"), L.T("武器库遇袭", "Armory Raided"), L.T("（测试）武器价格上涨。", "(Test) Weapon prices are up."), 25, 3, 0);
0829: 			return "武器库遇袭 +25%";
0830: 		case 1:
~~~

~~~csharp
0829: 			return "武器库遇袭 +25%";
0830: 		case 1:
0831: 			QueueEvent("malice_crackdown", L.T("巨企协助治安部清缴反抗军", "Corporations Back Security Crackdown"), L.T("清缴", "Crackdown"), L.T("（测试）武器流入市场。", "(Test) Weapons flood the market."), -25, 3, 0);
0832: 			return "清缴 -25%";
0833: 		default:
~~~

~~~csharp
0832: 			return "清缴 -25%";
0833: 		default:
0834: 			QueueEvent("malice_devil_dies", L.T("恶魔之死", "Death of a Devil"), L.T("恶魔之死", "Death of a Devil"), L.T("（测试）武器价格上涨。", "(Test) Weapon prices are up."), 30, 3, 0);
0835: 			return "恶魔之死 +30%";
0836: 		}
~~~

## CorpPresence/Malice/MalicePanel.cs

~~~csharp
0036: 		if (corpTerms == null || companyState == null)
0037: 		{
0038: 			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 24f), L.T("存档未就绪", "Save not ready"), t.Small);
0039: 			return;
0040: 		}
~~~

~~~csharp
0041: 		float y = ((Rect)(ref r)).y;
0042: 		bool flag = Malice.IsMembership(c);
0043: 		GUI.Label(new Rect(((Rect)(ref r)).x, y, ((Rect)(ref r)).width, 24f), flag ? L.T("基础会员", "Basic membership") : L.T("订购服务", "Ordering service"), t.Body);
0044: 		y += 26f;
0045: 		string text = (Malice.Subscribed(c) ? (L.En ? $"Active through day {corpTerms.SubUntil - 1}." : $"已开通，有效到第 {corpTerms.SubUntil - 1} 天（含）。") : ((!corpTerms.Subscribed) ? (flag ? L.T("未开通。会员才能使用索拉纳的服务。", "Not a member. Only members can use Solana's services.") : L.T("未开通。开通后才能电话订货。", "Not subscribed. Subscribe to order by phone.")) : ((L.En ? $"Expired (paused since day {corpTerms.SubUntil})." : $"已到期（第 {corpTerms.SubUntil} 天起暂停）。") + (Malice.Lapsed(corpTerms) ? L.T("断供超过一周，再续要加复通费。", " Lapsed over a week: reactivation fee applies.") : ""))));
~~~

~~~csharp
0043: 		GUI.Label(new Rect(((Rect)(ref r)).x, y, ((Rect)(ref r)).width, 24f), flag ? L.T("基础会员", "Basic membership") : L.T("订购服务", "Ordering service"), t.Body);
0044: 		y += 26f;
0045: 		string text = (Malice.Subscribed(c) ? (L.En ? $"Active through day {corpTerms.SubUntil - 1}." : $"已开通，有效到第 {corpTerms.SubUntil - 1} 天（含）。") : ((!corpTerms.Subscribed) ? (flag ? L.T("未开通。会员才能使用索拉纳的服务。", "Not a member. Only members can use Solana's services.") : L.T("未开通。开通后才能电话订货。", "Not subscribed. Subscribe to order by phone.")) : ((L.En ? $"Expired (paused since day {corpTerms.SubUntil})." : $"已到期（第 {corpTerms.SubUntil} 天起暂停）。") + (Malice.Lapsed(corpTerms) ? L.T("断供超过一周，再续要加复通费。", " Lapsed over a week: reactivation fee applies.") : ""))));
0046: 		GUI.Label(new Rect(((Rect)(ref r)).x, y, ((Rect)(ref r)).width, 40f), text, t.Wrap);
0047: 		y += 42f;
~~~

~~~csharp
0049: 		if (Ui.Button(text: (!corpTerms.Subscribed) ? ((!flag) ? (L.En ? $"Subscribe ({num}₵, first week included)" : $"开通（{num}₵，含第一周）") : (L.En ? $"Become a basic member ({num}₵ a week)" : $"成为基础会员（{num}₵ 一周）")) : (L.En ? $"Renew one week ({num}₵)" : $"续费一周（{num}₵）"), r: new Rect(((Rect)(ref r)).x, y, 260f, 32f), style: t.BtnHotStyle))
0050: 		{
0051: 			Say(Malice.Subscribe(c) ?? L.T("已续上。", "Done."));
0052: 		}
0053: 		if (Ui.Button(new Rect(((Rect)(ref r)).x + 270f, y, 200f, 32f), (corpTerms.AutoRenew ? "☑ " : "☐ ") + L.T("自动续费", "Auto-renew"), t.BtnStyle))
~~~

~~~csharp
0051: 			Say(Malice.Subscribe(c) ?? L.T("已续上。", "Done."));
0052: 		}
0053: 		if (Ui.Button(new Rect(((Rect)(ref r)).x + 270f, y, 200f, 32f), (corpTerms.AutoRenew ? "☑ " : "☐ ") + L.T("自动续费", "Auto-renew"), t.BtnStyle))
0054: 		{
0055: 			corpTerms.AutoRenew = !corpTerms.AutoRenew;
~~~

~~~csharp
0056: 		}
0057: 		y += 38f;
0058: 		GUI.Label(new Rect(((Rect)(ref r)).x, y, ((Rect)(ref r)).width, 40f), L.T("续费每次管 7 天，最多提前续一周，到期前一天提醒。自动续费只在扣完后还够交房租时才扣。", "Each renewal covers 7 days, at most one week ahead; you get a reminder the day before. Auto-renew only charges if you can still pay rent afterwards."), t.Small);
0059: 		y += 44f;
0060: 		if (!flag)
~~~

~~~csharp
0062: 			Ui.Fill(new Rect(((Rect)(ref r)).x, y, ((Rect)(ref r)).width, 1f), t.Line);
0063: 			y += 8f;
0064: 			GUI.Label(new Rect(((Rect)(ref r)).x, y, ((Rect)(ref r)).width, 24f), L.T("目录解锁", "Catalog unlocks"), t.Body);
0065: 			y += 26f;
0066: 			int num2 = c.Tier(companyState);
~~~

~~~csharp
0068: 			{
0069: 				string text2 = c.TierNames[i - 1];
0070: 				string text3 = ((corpTerms.Unlocked < i) ? ((num2 < i) ? L.T("合作度未到", "Cooperation too low") : (L.En ? $"Can apply ({Malice.ApplyFee[i]}₵)" : $"可申请（{Malice.ApplyFee[i]}₵）")) : L.T("已解锁", "Unlocked"));
0071: 				GUI.Label(new Rect(((Rect)(ref r)).x, y, 260f, 28f), text2 + "：" + text3, t.Small);
0072: 				if (corpTerms.Unlocked == i - 1 && num2 >= i && Ui.Button(new Rect(((Rect)(ref r)).x + 270f, y, 200f, 28f), L.En ? $"Apply ({Malice.ApplyFee[i]}₵)" : $"申请（{Malice.ApplyFee[i]}₵）", t.BtnStyle))
~~~

~~~csharp
0072: 				if (corpTerms.Unlocked == i - 1 && num2 >= i && Ui.Button(new Rect(((Rect)(ref r)).x + 270f, y, 200f, 28f), L.En ? $"Apply ({Malice.ApplyFee[i]}₵)" : $"申请（{Malice.ApplyFee[i]}₵）", t.BtnStyle))
0073: 				{
0074: 					Say(Malice.ApplyUnlock(c) ?? L.T("已解锁。", "Unlocked."));
0075: 				}
0076: 				y += 32f;
~~~

~~~csharp
0084: 			if (Ui.Button(new Rect(((Rect)(ref r)).x, y, 300f, 30f), L.En ? $"Quiet payment ({num3}₵, a “sample” arrives tomorrow)" : $"私下打点（{num3}₵，明天送“样品”）", t.BtnHotStyle))
0085: 			{
0086: 				Say(RebelStory.BribeMeridian() ?? L.T("明天送到。", "Arrives tomorrow."));
0087: 			}
0088: 			y += 36f;
~~~

## CorpPresence/Malice/Rebels.cs

~~~csharp
0123: 			{
0124: 				maliceState.OdinArrested = true;
0125: 				Notify(L.T("奥丁被捕了。反抗军断了线：订货、地下销赃网络都停了。", "Odin has been arrested. The Resistance has gone dark: orders and the fence network are suspended."));
0126: 			}
0127: 		}
~~~

~~~csharp
0141: 			{
0142: 			}
0143: 			Notify(L.T($"反抗军的电话（{company.PhoneNumber}）记在了电话旁的记事簿上。", $"The Resistance's number ({company.PhoneNumber}) is now in the notebook by the phone."));
0144: 		}
0145: 		int num = 0;
~~~

~~~csharp
0168: 						return null;
0169: 					}
0170: 					return L.T("反抗军把你拉黑了。", "The Resistance has blacklisted you.");
0171: 				}
0172: 				return L.T("你加入了巨企，反抗军不再和你来往。", "You joined the corporations; the Resistance wants nothing to do with you.");
~~~

~~~csharp
0170: 					return L.T("反抗军把你拉黑了。", "The Resistance has blacklisted you.");
0171: 				}
0172: 				return L.T("你加入了巨企，反抗军不再和你来往。", "You joined the corporations; the Resistance wants nothing to do with you.");
0173: 			}
0174: 			return L.T("反抗军断线了：奥丁被捕。", "The Resistance has gone dark: Odin was arrested.");
~~~

~~~csharp
0172: 				return L.T("你加入了巨企，反抗军不再和你来往。", "You joined the corporations; the Resistance wants nothing to do with you.");
0173: 			}
0174: 			return L.T("反抗军断线了：奥丁被捕。", "The Resistance has gone dark: Odin was arrested.");
0175: 		}
0176: 		return null;
~~~

~~~csharp
0199: 		if (maliceState == null || companyState == null)
0200: 		{
0201: 			return L.T("存档未就绪", "Save not ready");
0202: 		}
0203: 		if (Arrested || Cut || Blacklisted)
~~~

~~~csharp
0203: 		if (Arrested || Cut || Blacklisted)
0204: 		{
0205: 			return L.T("反抗军断线了。", "The Resistance has gone dark.");
0206: 		}
0207: 		if (FenceClosed)
~~~

~~~csharp
0207: 		if (FenceClosed)
0208: 		{
0209: 			return L.T("反抗军不再给你安排地下买家。", "The Resistance no longer sends you underground buyers.");
0210: 		}
0211: 		if (companyState.Visitors.Exists((Visitor v) => v.Kind == "fence"))
~~~

~~~csharp
0211: 		if (companyState.Visitors.Exists((Visitor v) => v.Kind == "fence"))
0212: 		{
0213: 			return L.T("已经约好了，明天来。", "Already booked for tomorrow.");
0214: 		}
0215: 		int num = FenceCost();
~~~

~~~csharp
0216: 		if (maliceState.RebelPoints < num)
0217: 		{
0218: 			return L.T($"助力点不够（要 {num} 点）。", $"Not enough support points (need {num}).");
0219: 		}
0220: 		maliceState.RebelPoints -= num;
~~~

~~~csharp
0248: 		try
0249: 		{
0250: 			string text = L.T("地下买家", "Underground Buyer");
0251: 			StoreClient val = new StoreClient
0252: 			{
~~~

~~~csharp
0265: 			}
0266: 			val.SetBudget(num);
0267: 			val.mainDialogue.SetText(text, L.T("奥丁让我来的。", "Odin sent me.")).NextDialogue().SetText(text, L.T("什么都收，现钱。价钱别指望太高。", "I take anything, cash. Don't expect top price."));
0268: 			val.clientBuyingTagList.Add("ALL_TYPE");
0269: 			val.AddBasicDialog();
~~~

~~~csharp
0324: 		if (instance == null)
0325: 		{
0326: 			return L.T("存档未就绪", "Save not ready");
0327: 		}
0328: 		if (!instance.CanPay(amount))
~~~

~~~csharp
0328: 		if (!instance.CanPay(amount))
0329: 		{
0330: 			return L.T("现金不够", "Not enough cash");
0331: 		}
0332: 		CompanyMoney.Pay(instance, amount);
~~~

## CorpPresence/Malice/RebelCo.cs

~~~csharp
0013: 	public override string Id => "rebel";
0014: 
0015: 	public override string Name => L.T("反抗军", "The Resistance");
0016: 
0017: 	public override string CharacterId => "odin";
~~~

~~~csharp
0019: 	public override long PhoneNumber => 77224L;
0020: 
0021: 	public override string[] TierNames => L.T(new string[3] { "外围", "同志", "骨干" }, new string[3] { "Sympathizer", "Comrade", "Cadre" });
0022: 
0023: 	public override string CardFlavor => "";
~~~

~~~csharp
0023: 	public override string CardFlavor => "";
0024: 
0025: 	public override string DeskName => L.T("奥丁", "Odin");
0026: 
0027: 	public override string DeskAsk => "";
~~~

~~~csharp
0027: 	public override string DeskAsk => "";
0028: 
0029: 	public override string[] FrontDesk => L.T(new string[2] { "是我。", "长话短说。" }, new string[2] { "It's me.", "Make it quick." });
0030: 
0031: 	public override string FrontDeskDone => L.T("会有人送过去。", "Someone will bring it.");
~~~

~~~csharp
0029: 	public override string[] FrontDesk => L.T(new string[2] { "是我。", "长话短说。" }, new string[2] { "It's me.", "Make it quick." });
0030: 
0031: 	public override string FrontDeskDone => L.T("会有人送过去。", "Someone will bring it.");
0032: 
0033: 	public override bool Active => Settings.MaliceMode;
~~~

## CorpPresence/Malice/Malice.cs

~~~csharp
0122: 		{
0123: 			Ext.ExtraModule = value;
0124: 		}, (Company c) => Applies(c) ? L.T("巨企合约", "Corporate Terms") : null);
0125: 		_callbacks.Replace(() => Ext.DrawExtraModule, delegate(Action<Company, Rect, Theme> value)
0126: 		{
~~~

~~~csharp
0272: 		if (corpTerms == null)
0273: 		{
0274: 			return L.T("存档未就绪", "Save not ready");
0275: 		}
0276: 		if (corpTerms.SubUntil - Day >= 7)
~~~

~~~csharp
0276: 		if (corpTerms.SubUntil - Day >= 7)
0277: 		{
0278: 			return L.T("最多提前续一周。", "You can only renew one week ahead.");
0279: 		}
0280: 		int num = SubscriptionPrice(c);
~~~

~~~csharp
0299: 		if (On && Envoy.Punished)
0300: 		{
0301: 			return L.T("撒该的惩罚：所有公司服务暂停。打他的电话答应还款计划。", "Zacchaeus's penalty: all company services are suspended. Call him to agree to a repayment plan.");
0302: 		}
0303: 		if (Applies(c) && Envoy.OrdersRefused)
~~~

~~~csharp
0303: 		if (Applies(c) && Envoy.OrdersRefused)
0304: 		{
0305: 			return L.T("你拒绝了撒该，巨企的订货暂不开放。打他的电话回心转意。", "You turned Zacchaeus down: corporate ordering is closed. Call him if you change your mind.");
0306: 		}
0307: 		if (!Applies(c) || Subscribed(c))
~~~

~~~csharp
0311: 		if (IsMembership(c))
0312: 		{
0313: 			return L.T($"需要{c.Name}的基础会员（{SubscriptionPrice(c)}₵ 一周），在“巨企合约”页开通。", $"Requires a {c.Name} basic membership ({SubscriptionPrice(c)}₵ a week). Sign up on the Corporate Terms page.");
0314: 		}
0315: 		return L.T($"{c.Name}的订购服务未开通或已到期，请在“巨企合约”页开通（{SubscriptionPrice(c)}₵）。", $"{c.Name}'s ordering service isn't active. Subscribe on the Corporate Terms page ({SubscriptionPrice(c)}₵).");
~~~

~~~csharp
0313: 			return L.T($"需要{c.Name}的基础会员（{SubscriptionPrice(c)}₵ 一周），在“巨企合约”页开通。", $"Requires a {c.Name} basic membership ({SubscriptionPrice(c)}₵ a week). Sign up on the Corporate Terms page.");
0314: 		}
0315: 		return L.T($"{c.Name}的订购服务未开通或已到期，请在“巨企合约”页开通（{SubscriptionPrice(c)}₵）。", $"{c.Name}'s ordering service isn't active. Subscribe on the Corporate Terms page ({SubscriptionPrice(c)}₵).");
0316: 	}
0317: 
~~~

~~~csharp
0335: 		if (corpTerms == null || companyState == null)
0336: 		{
0337: 			return L.T("存档未就绪", "Save not ready");
0338: 		}
0339: 		int num = corpTerms.Unlocked + 1;
~~~

~~~csharp
0340: 		if (num >= ApplyFee.Length)
0341: 		{
0342: 			return L.T("已经全部解锁", "Everything is unlocked");
0343: 		}
0344: 		if (c.Tier(companyState) < num)
~~~

~~~csharp
0344: 		if (c.Tier(companyState) < num)
0345: 		{
0346: 			return L.T("合作度要先到“" + c.TierNames[num - 1] + "”", "Reach “" + c.TierNames[num - 1] + "” first");
0347: 		}
0348: 		string text = Services.Pay(c, ApplyFee[num], "unlock");
~~~

~~~csharp
0423: 		if (On && Envoy.Punished && !(c is EnvoyCo))
0424: 		{
0425: 			return L.T("撒该的惩罚：公司服务暂停。", "Zacchaeus's penalty: company services are suspended.");
0426: 		}
0427: 		if (!Applies(c))
~~~

~~~csharp
0443: 		if (num2 > 0 && !instance.CanPay(num2))
0444: 		{
0445: 			return L.T("现金不够", "Not enough cash");
0446: 		}
0447: 		if (num2 > 0)
~~~

~~~csharp
0497: 				{
0498: 					value.RemindedDay = Day;
0499: 					Notify(L.T($"{company.Name}的订购服务明天到期，续费 {200}₵。", $"{company.Name}'s ordering service expires tomorrow. Renewal is {200}₵."));
0500: 				}
0501: 				else if (value.SubUntil == Day)
~~~

~~~csharp
0501: 				else if (value.SubUntil == Day)
0502: 				{
0503: 					Notify(L.T(company.Name + "的订购服务今天起暂停。", company.Name + "'s ordering service is paused from today."));
0504: 				}
0505: 			}
~~~

~~~csharp
0532: 			WasOn = false
0533: 		};
0534: 		Notify(L.T("恶意巨企模式已关闭：商人联合和反抗军并回星际客联盟，订阅、代金券、助力点、撒该的账都清掉了。", "Corporate Malice is off: the Merchant Union and the Resistance are the Spacer Alliance again, and subscriptions, vouchers, support points and Zacchaeus's accounts are cleared."));
0535: 	}
0536: 
~~~

~~~csharp
0542: 		if (instance.GetCash() - Math.Max(0, num - t.Vouchers) < num2)
0543: 		{
0544: 			Notify(L.T(c.Name + "的订购服务到期：现金扣完不够交房租，没有自动续费。", c.Name + "'s ordering service expired: not renewed automatically, it would leave you short for rent."));
0545: 			return;
0546: 		}
~~~

~~~csharp
0548: 		if (text != null)
0549: 		{
0550: 			Notify(L.T(c.Name + "自动续费失败：", c.Name + " auto-renewal failed: ") + text);
0551: 		}
0552: 		else
~~~

~~~csharp
0552: 		else
0553: 		{
0554: 			Notify(L.T($"{c.Name}的订购服务已自动续费 {num}₵。", $"{c.Name}'s ordering service was renewed automatically for {num}₵."));
0555: 		}
0556: 	}
~~~

## CorpPresence/Malice/Envoy.cs

~~~csharp
0185: 				s.RepayLeft = 5;
0186: 				s.RepayDue = Day + 7;
0187: 				Notify(L.T($"帕斯泰尔或卡迈克尔的订阅断了。撒该的援助成为了你脖子上的套索现在你每周需要还 {800}₵，共 {5} 周。", $"Your Pastel or Carmicheal subscription lapsed. Zacchaeus's aid is now a noose around your neck: you owe {800}₵ a week for {5} weeks."));
0188: 			}
0189: 			else if (Day >= s.AidStart + 21)
~~~

~~~csharp
0207: 			s.ChannelDue = Day + 7;
0208: 			s.ChannelFee = 0;
0209: 			Notify(L.T("礼物没送到该送到的人手上。巨企开始对你取收渠道费：两周内，每卖一件巨企目录里的货收取 5%的渠道费。", "The gift never reached the right hands. The corporations now charge you a channel fee: 5% on every corporate catalog item you sell for two weeks."));
0210: 		}
0211: 		if (s.Ch2 == "malice" && s.ChannelDue >= 0 && s.ChannelDue <= Day && !Booked("channel"))
~~~

~~~csharp
0225: 			s.RetaliateFrom = Day;
0226: 			s.RetaliateUntil = Day + 2;
0227: 			Notify(L.T("昨天那颗礼物在反抗军的据点爆炸。反抗军开始对你进行报复虽然被治安部拦下，但还是造成了影响：这几天来的顾客少了。", "Yesterday's gift went off in a Resistance safehouse. The Resistance struck back at you; Security stopped them, but the damage is done: fewer customers will come for a few days."));
0228: 		}
0229: 		if (s.RetaliateUntil >= 0 && Day > s.RetaliateUntil && !s.OdinUnderstood && !Booked("odin"))
~~~

~~~csharp
0271: 				}
0272: 			}
0273: 			Notify(L.T("导师留了话，让你回个电话（商人联合）。", "The Mentor left word: call him back (Merchant Union)."));
0274: 		}
0275: 		if (!(s.Ch4 == "silent") || s.Agent)
~~~

~~~csharp
0456: 	{
0457: 		EnvoyState s = S;
0458: 		string name = L.T("撒该", "Zacchaeus");
0459: 		switch (kind)
0460: 		{
~~~

~~~csharp
0462: 			return Talk(kind, name, Sprite, new string[3]
0463: 			{
0464: 				L.T("您好。我是撒该·索恩，联合站区管理处站长。", "Good day. I'm Zacchaeus Thorne, director of the Joint District Administration."),
0465: 				L.T("我们注意到您的生意最近不错，想必很需要一笔扶持金来扩大规模。", "We've noticed business has been good lately. Surely you could use a grant to expand."),
0466: 				L.T($"管理处可以提供一笔 {2000}₵ 扶持金，甚至不用您偿还。条件很简单：连续三周订阅帕斯泰尔和卡迈克尔的订购服务，我的朋友们最近需要扩大市场。", $"The Administration can offer a {2000}₵ grant, and you needn't even repay it. One simple condition: keep Pastel and Carmicheal's ordering services subscribed for three weeks straight. My friends are looking to expand their market.")
~~~

~~~csharp
0463: 			{
0464: 				L.T("您好。我是撒该·索恩，联合站区管理处站长。", "Good day. I'm Zacchaeus Thorne, director of the Joint District Administration."),
0465: 				L.T("我们注意到您的生意最近不错，想必很需要一笔扶持金来扩大规模。", "We've noticed business has been good lately. Surely you could use a grant to expand."),
0466: 				L.T($"管理处可以提供一笔 {2000}₵ 扶持金，甚至不用您偿还。条件很简单：连续三周订阅帕斯泰尔和卡迈克尔的订购服务，我的朋友们最近需要扩大市场。", $"The Administration can offer a {2000}₵ grant, and you needn't even repay it. One simple condition: keep Pastel and Carmicheal's ordering services subscribed for three weeks straight. My friends are looking to expand their market.")
0467: 			}, L.T("您意下如何？", "What do you say?"), new(string, string, Action)[2]
~~~

~~~csharp
0464: 				L.T("您好。我是撒该·索恩，联合站区管理处站长。", "Good day. I'm Zacchaeus Thorne, director of the Joint District Administration."),
0465: 				L.T("我们注意到您的生意最近不错，想必很需要一笔扶持金来扩大规模。", "We've noticed business has been good lately. Surely you could use a grant to expand."),
0466: 				L.T($"管理处可以提供一笔 {2000}₵ 扶持金，甚至不用您偿还。条件很简单：连续三周订阅帕斯泰尔和卡迈克尔的订购服务，我的朋友们最近需要扩大市场。", $"The Administration can offer a {2000}₵ grant, and you needn't even repay it. One simple condition: keep Pastel and Carmicheal's ordering services subscribed for three weeks straight. My friends are looking to expand their market.")
0467: 			}, L.T("您意下如何？", "What do you say?"), new(string, string, Action)[2]
0468: 			{
~~~

~~~csharp
0465: 				L.T("我们注意到您的生意最近不错，想必很需要一笔扶持金来扩大规模。", "We've noticed business has been good lately. Surely you could use a grant to expand."),
0466: 				L.T($"管理处可以提供一笔 {2000}₵ 扶持金，甚至不用您偿还。条件很简单：连续三周订阅帕斯泰尔和卡迈克尔的订购服务，我的朋友们最近需要扩大市场。", $"The Administration can offer a {2000}₵ grant, and you needn't even repay it. One simple condition: keep Pastel and Carmicheal's ordering services subscribed for three weeks straight. My friends are looking to expand their market.")
0467: 			}, L.T("您意下如何？", "What do you say?"), new(string, string, Action)[2]
0468: 			{
0469: 				Opt(L.T("我接受。", "I accept."), L.T("明智。钱已经放在柜台上了。", "Wise. The money is on your counter."), AcceptAid),
~~~

~~~csharp
0467: 			}, L.T("您意下如何？", "What do you say?"), new(string, string, Action)[2]
0468: 			{
0469: 				Opt(L.T("我接受。", "I accept."), L.T("明智。钱已经放在柜台上了。", "Wise. The money is on your counter."), AcceptAid),
0470: 				Opt(L.T("不用了。", "No thanks."), L.T("可惜。站区的物价最近恐怕会有些波动。", "A pity. Prices in the district may become somewhat volatile."), DeclineAid)
0471: 			}, delegate
~~~

~~~csharp
0468: 			{
0469: 				Opt(L.T("我接受。", "I accept."), L.T("明智。钱已经放在柜台上了。", "Wise. The money is on your counter."), AcceptAid),
0470: 				Opt(L.T("不用了。", "No thanks."), L.T("可惜。站区的物价最近恐怕会有些波动。", "A pity. Prices in the district may become somewhat volatile."), DeclineAid)
0471: 			}, delegate
0472: 			{
~~~

~~~csharp
0477: 			return Talk(kind, name, Sprite, new string[2]
0478: 			{
0479: 				L.T("三周，一天不落。联合管理处很满意您信守承诺。", "Three weeks, not a day missed. The Administration appreciates someone who keeps their word."),
0480: 				L.T("这枚奖牌请收下。摆在显眼的地方，上层区的客人会注意到。", "Please accept this medal. Display it somewhere visible; Upper Levels clients will notice.")
0481: 			}, null, null, delegate
~~~

~~~csharp
0478: 			{
0479: 				L.T("三周，一天不落。联合管理处很满意您信守承诺。", "Three weeks, not a day missed. The Administration appreciates someone who keeps their word."),
0480: 				L.T("这枚奖牌请收下。摆在显眼的地方，上层区的客人会注意到。", "Please accept this medal. Display it somewhere visible; Upper Levels clients will notice.")
0481: 			}, null, null, delegate
0482: 			{
~~~

~~~csharp
0485: 			});
0486: 		case "repay":
0487: 			return Talk(kind, name, Sprite, new string[1] { L.T($"这周的 {800}₵。", $"This week's {800}₵.") }, L.T($"还剩 {s.RepayLeft} 期。", $"{s.RepayLeft} installment(s) left."), new(string, string, Action)[2]
0488: 			{
0489: 				Opt(L.T($"给你 {800}₵。", $"Here's {800}₵."), L.T("合作愉快。", "A pleasure."), PayInstallment),
~~~

~~~csharp
0487: 			return Talk(kind, name, Sprite, new string[1] { L.T($"这周的 {800}₵。", $"This week's {800}₵.") }, L.T($"还剩 {s.RepayLeft} 期。", $"{s.RepayLeft} installment(s) left."), new(string, string, Action)[2]
0488: 			{
0489: 				Opt(L.T($"给你 {800}₵。", $"Here's {800}₵."), L.T("合作愉快。", "A pleasure."), PayInstallment),
0490: 				Opt(L.T("这周拿不出来。", "I can't pay this week."), L.T("我会如实记下的。", "I'll note that accurately."), MissInstallment)
0491: 			}, null);
~~~

~~~csharp
0488: 			{
0489: 				Opt(L.T($"给你 {800}₵。", $"Here's {800}₵."), L.T("合作愉快。", "A pleasure."), PayInstallment),
0490: 				Opt(L.T("这周拿不出来。", "I can't pay this week."), L.T("我会如实记下的。", "I'll note that accurately."), MissInstallment)
0491: 			}, null);
0492: 		case "ch2":
~~~

~~~csharp
0493: 			return Talk(kind, name, Sprite, new string[3]
0494: 			{
0495: 				L.T("生意兴隆。管理处其实一直在为您的店铺提供看不见的服务，我觉得您需要做出一些表示。", "Business is thriving. The Administration has quietly been providing your shop with services you never see. I think a gesture of thanks is in order."),
0496: 				L.T($"这些服务的费用是 {1500}₵。", $"Those services come to {1500}₵."),
0497: 				L.T("或者……您可以帮我们一个小忙。有一件东西，需要送到一位叫奥丁的人手里，我当然知道您和他很熟。", "Or... you could do us a small favor. There's a package that needs to reach a man named Odin. I know you two are well acquainted.")
~~~

~~~csharp
0494: 			{
0495: 				L.T("生意兴隆。管理处其实一直在为您的店铺提供看不见的服务，我觉得您需要做出一些表示。", "Business is thriving. The Administration has quietly been providing your shop with services you never see. I think a gesture of thanks is in order."),
0496: 				L.T($"这些服务的费用是 {1500}₵。", $"Those services come to {1500}₵."),
0497: 				L.T("或者……您可以帮我们一个小忙。有一件东西，需要送到一位叫奥丁的人手里，我当然知道您和他很熟。", "Or... you could do us a small favor. There's a package that needs to reach a man named Odin. I know you two are well acquainted.")
0498: 			}, L.T("您选哪一样？", "Which will it be?"), new(string, string, Action)[3]
~~~

~~~csharp
0495: 				L.T("生意兴隆。管理处其实一直在为您的店铺提供看不见的服务，我觉得您需要做出一些表示。", "Business is thriving. The Administration has quietly been providing your shop with services you never see. I think a gesture of thanks is in order."),
0496: 				L.T($"这些服务的费用是 {1500}₵。", $"Those services come to {1500}₵."),
0497: 				L.T("或者……您可以帮我们一个小忙。有一件东西，需要送到一位叫奥丁的人手里，我当然知道您和他很熟。", "Or... you could do us a small favor. There's a package that needs to reach a man named Odin. I know you two are well acquainted.")
0498: 			}, L.T("您选哪一样？", "Which will it be?"), new(string, string, Action)[3]
0499: 			{
~~~

~~~csharp
0496: 				L.T($"这些服务的费用是 {1500}₵。", $"Those services come to {1500}₵."),
0497: 				L.T("或者……您可以帮我们一个小忙。有一件东西，需要送到一位叫奥丁的人手里，我当然知道您和他很熟。", "Or... you could do us a small favor. There's a package that needs to reach a man named Odin. I know you two are well acquainted.")
0498: 			}, L.T("您选哪一样？", "Which will it be?"), new(string, string, Action)[3]
0499: 			{
0500: 				Opt(L.T($"我付 {1500}₵。", $"I'll pay {1500}₵."), L.T("感谢您的配合。", "Thank you for your cooperation."), delegate
~~~

~~~csharp
0498: 			}, L.T("您选哪一样？", "Which will it be?"), new(string, string, Action)[3]
0499: 			{
0500: 				Opt(L.T($"我付 {1500}₵。", $"I'll pay {1500}₵."), L.T("感谢您的配合。", "Thank you for your cooperation."), delegate
0501: 				{
0502: 					Say(PayServiceFee());
~~~

~~~csharp
0502: 					Say(PayServiceFee());
0503: 				}),
0504: 				Opt(L.T("东西给我吧。", "Give me the gift."), L.T("两周之内。打电话叫他来，卖给他就行。办成了，订购服务的订阅费我想我可以帮您免去。", "Within two weeks. Call him over and sell it to him. Do this, and I believe I can waive your subscription fees."), AcceptBomb),
0505: 				Opt(L.T("都不干。", "Neither."), L.T("那么巨企的订货通道，暂时可能就不对您开放了。当然想通了打我电话。", "Then the corporate ordering channel may be closed to you for a while. Do call me when you've reconsidered."), RefuseCh2)
0506: 			}, null);
~~~

~~~csharp
0503: 				}),
0504: 				Opt(L.T("东西给我吧。", "Give me the gift."), L.T("两周之内。打电话叫他来，卖给他就行。办成了，订购服务的订阅费我想我可以帮您免去。", "Within two weeks. Call him over and sell it to him. Do this, and I believe I can waive your subscription fees."), AcceptBomb),
0505: 				Opt(L.T("都不干。", "Neither."), L.T("那么巨企的订货通道，暂时可能就不对您开放了。当然想通了打我电话。", "Then the corporate ordering channel may be closed to you for a while. Do call me when you've reconsidered."), RefuseCh2)
0506: 			}, null);
0507: 		case "channel":
~~~

~~~csharp
0506: 			}, null);
0507: 		case "channel":
0508: 			return Talk(kind, name, Sprite, new string[1] { L.T($"这一周的渠道费：{s.ChannelFee}₵。", $"This week's channel fee: {s.ChannelFee}₵.") }, null, null, CollectChannel);
0509: 		case "odin":
0510: 			return Talk(kind, L.T("奥丁", "Odin"), "wanted4", new string[3]
~~~

~~~csharp
0508: 			return Talk(kind, name, Sprite, new string[1] { L.T($"这一周的渠道费：{s.ChannelFee}₵。", $"This week's channel fee: {s.ChannelFee}₵.") }, null, null, CollectChannel);
0509: 		case "odin":
0510: 			return Talk(kind, L.T("奥丁", "Odin"), "wanted4", new string[3]
0511: 			{
0512: 				L.T("别紧张。我不是来算账的。", "Relax. I'm not here to settle scores."),
~~~

~~~csharp
0510: 			return Talk(kind, L.T("奥丁", "Odin"), "wanted4", new string[3]
0511: 			{
0512: 				L.T("别紧张。我不是来算账的。", "Relax. I'm not here to settle scores."),
0513: 				L.T("我知道那颗炸弹是谁给你的。我也知道你是被逼的。", "I know who gave you that bomb. I also know you were forced."),
0514: 				L.T("下层区的人都一样，脖子上都套着他们的绳子。", "Everyone down here is the same. Their noose around every neck.")
~~~

~~~csharp
0511: 			{
0512: 				L.T("别紧张。我不是来算账的。", "Relax. I'm not here to settle scores."),
0513: 				L.T("我知道那颗炸弹是谁给你的。我也知道你是被逼的。", "I know who gave you that bomb. I also know you were forced."),
0514: 				L.T("下层区的人都一样，脖子上都套着他们的绳子。", "Everyone down here is the same. Their noose around every neck.")
0515: 			}, null, null, delegate
~~~

~~~csharp
0512: 				L.T("别紧张。我不是来算账的。", "Relax. I'm not here to settle scores."),
0513: 				L.T("我知道那颗炸弹是谁给你的。我也知道你是被逼的。", "I know who gave you that bomb. I also know you were forced."),
0514: 				L.T("下层区的人都一样，脖子上都套着他们的绳子。", "Everyone down here is the same. Their noose around every neck.")
0515: 			}, null, null, delegate
0516: 			{
~~~

~~~csharp
0520: 			return Talk(kind, name, Sprite, new string[3]
0521: 			{
0522: 				L.T("恭喜您买下了这间店。从今天起，您是这家店真正的店主了。", "Congratulations on buying the shop. As of today, it's truly yours."),
0523: 				L.T("不过店主理所应当的应该和管理处站在一方我们会为您争取新的利益：订货九折，每周一份物资补贴，甚至治安部那边我们也替您打招呼，不会过分追究您做了什么。", "But an owner naturally belongs on the Administration's side, and we'll secure new benefits for you: 10% off orders, a weekly supply subsidy, and we'll even have a word with Security so they won't look too closely at what you've done."),
0524: 				L.T("当然，您得和那些老鼠断干净。", "Naturally, you'd cut ties with those rats.")
~~~

~~~csharp
0521: 			{
0522: 				L.T("恭喜您买下了这间店。从今天起，您是这家店真正的店主了。", "Congratulations on buying the shop. As of today, it's truly yours."),
0523: 				L.T("不过店主理所应当的应该和管理处站在一方我们会为您争取新的利益：订货九折，每周一份物资补贴，甚至治安部那边我们也替您打招呼，不会过分追究您做了什么。", "But an owner naturally belongs on the Administration's side, and we'll secure new benefits for you: 10% off orders, a weekly supply subsidy, and we'll even have a word with Security so they won't look too closely at what you've done."),
0524: 				L.T("当然，您得和那些老鼠断干净。", "Naturally, you'd cut ties with those rats.")
0525: 			}, L.T("加入我们吗？", "Will you join us?"), new(string, string, Action)[2]
~~~

~~~csharp
0522: 				L.T("恭喜您买下了这间店。从今天起，您是这家店真正的店主了。", "Congratulations on buying the shop. As of today, it's truly yours."),
0523: 				L.T("不过店主理所应当的应该和管理处站在一方我们会为您争取新的利益：订货九折，每周一份物资补贴，甚至治安部那边我们也替您打招呼，不会过分追究您做了什么。", "But an owner naturally belongs on the Administration's side, and we'll secure new benefits for you: 10% off orders, a weekly supply subsidy, and we'll even have a word with Security so they won't look too closely at what you've done."),
0524: 				L.T("当然，您得和那些老鼠断干净。", "Naturally, you'd cut ties with those rats.")
0525: 			}, L.T("加入我们吗？", "Will you join us?"), new(string, string, Action)[2]
0526: 			{
~~~

~~~csharp
0523: 				L.T("不过店主理所应当的应该和管理处站在一方我们会为您争取新的利益：订货九折，每周一份物资补贴，甚至治安部那边我们也替您打招呼，不会过分追究您做了什么。", "But an owner naturally belongs on the Administration's side, and we'll secure new benefits for you: 10% off orders, a weekly supply subsidy, and we'll even have a word with Security so they won't look too closely at what you've done."),
0524: 				L.T("当然，您得和那些老鼠断干净。", "Naturally, you'd cut ties with those rats.")
0525: 			}, L.T("加入我们吗？", "Will you join us?"), new(string, string, Action)[2]
0526: 			{
0527: 				Opt(L.T("加入。", "I'm in."), L.T("欢迎。", "Welcome."), JoinCorps),
~~~

~~~csharp
0525: 			}, L.T("加入我们吗？", "Will you join us?"), new(string, string, Action)[2]
0526: 			{
0527: 				Opt(L.T("加入。", "I'm in."), L.T("欢迎。", "Welcome."), JoinCorps),
0528: 				Opt(L.T("不了。", "No."), L.T("您会后悔的。", "You'll regret this."), RefuseCorps)
0529: 			}, null);
~~~

~~~csharp
0526: 			{
0527: 				Opt(L.T("加入。", "I'm in."), L.T("欢迎。", "Welcome."), JoinCorps),
0528: 				Opt(L.T("不了。", "No."), L.T("您会后悔的。", "You'll regret this."), RefuseCorps)
0529: 			}, null);
0530: 		case "finale":
~~~

~~~csharp
0532: 			{
0533: 				"……",
0534: 				L.T("……", "...")
0535: 			}, null, null, Finale);
0536: 		default:
~~~

~~~csharp
0566: 		return Talk("coin:" + companyId, character.Name, sprite, new string[2]
0567: 		{
0568: 			L.T(company.Name + "让我把这个交给你。", company.Name + " asked me to give you this."),
0569: 			L.T("他们说，你是被选中的人。", "They say you're one of the chosen.")
0570: 		}, null, null, delegate
~~~

~~~csharp
0567: 		{
0568: 			L.T(company.Name + "让我把这个交给你。", company.Name + " asked me to give you this."),
0569: 			L.T("他们说，你是被选中的人。", "They say you're one of the chosen.")
0570: 		}, null, null, delegate
0571: 		{
~~~

~~~csharp
0590: 			if (!Malice.Subscribed(Companies.Find(id)))
0591: 			{
0592: 				Notify(L.T("记得开通" + Companies.Find(id).Name + "的订购服务（援助条件）。", "Remember to subscribe to " + Companies.Find(id).Name + "'s ordering service (aid condition)."));
0593: 			}
0594: 		}
~~~

~~~csharp
0617: 		{
0618: 			MissInstallment();
0619: 			Notify(L.T("现金不够，这期没交上。", "Not enough cash: this installment is missed."));
0620: 			return;
0621: 		}
~~~

~~~csharp
0640: 				s.Aid = "punished";
0641: 				s.RepayDue = -1;
0642: 				Notify(L.T("你没交上撒该的钱。所有公司服务暂停（反抗军除外），撒该开始“问候”你的客人。打他的电话可以答应还款计划。", "You missed Zacchaeus's payment. All company services are suspended (except the Resistance), and he's started “visiting” your customers. Call him to agree to a repayment plan."));
0643: 			}
0644: 			else if (s.Aid == "plan6")
~~~

~~~csharp
0646: 				s.LateWeeks++;
0647: 				s.RepayDue = Day + 7;
0648: 				Notify(L.T($"这期没交，顺延一周。巨企订货价格累计 +{s.LateWeeks * 10}%，还完恢复。", $"Missed: pushed back a week. Corporate order prices are now +{s.LateWeeks * 10}% until you finish paying."));
0649: 			}
0650: 		}
~~~

~~~csharp
0661: 			Book("medal", Day + 1);
0662: 		}
0663: 		Notify(L.T("撒该的钱还清了。", "Zacchaeus has been paid in full."));
0664: 	}
0665: 
~~~

~~~csharp
0669: 		if (s == null || s.Aid != "punished")
0670: 		{
0671: 			return L.T("现在不需要。", "Not needed right now.");
0672: 		}
0673: 		s.Aid = "plan6";
~~~

~~~csharp
0684: 		if (s.Ch2 != "" && s.Ch2 != "refused")
0685: 		{
0686: 			return L.T("现在不需要。", "Not needed right now.");
0687: 		}
0688: 		if (Store.GetCash() < 1500)
~~~

~~~csharp
0688: 		if (Store.GetCash() < 1500)
0689: 		{
0690: 			return L.T("现金不够", "Not enough cash");
0691: 		}
0692: 		Store.PayMoney(1500, true);
~~~

~~~csharp
0710: 			GameItem val = DirectoryMaster.Item("c4", true);
0711: 			val.EnableTag("CORP_ENVOY_BOMB", false);
0712: 			val.SetName(L.T("撒该的礼物", "Zacchaeus's Gift"));
0713: 			val.shortDescription = L.T("把他卖给该买的人。", "Sell it to the one who should buy it.");
0714: 			if (!StorePlacement.Place(val))
~~~

~~~csharp
0711: 			val.EnableTag("CORP_ENVOY_BOMB", false);
0712: 			val.SetName(L.T("撒该的礼物", "Zacchaeus's Gift"));
0713: 			val.shortDescription = L.T("把他卖给该买的人。", "Sell it to the one who should buy it.");
0714: 			if (!StorePlacement.Place(val))
0715: 			{
~~~

~~~csharp
0744: 				s.SubFree = true;
0745: 				s.BombSoldDay = Day;
0746: 				Notify(L.T("礼物送到了。撒该说，巨企的订购服务以后不收订阅费了。", "The gift was delivered. Zacchaeus says corporate ordering services are now subscription-free."));
0747: 			}
0748: 		}
~~~

~~~csharp
0813: 		s.SubsidyDue = Day + 7;
0814: 		Done("ch3");
0815: 		Notify(L.T("你加入了巨企。反抗军很失望断了和你的联系。", "You've joined the corporations. The Resistance, disappointed, has cut ties with you."));
0816: 	}
0817: 
~~~

~~~csharp
0828: 		if (s.Ch3 != "joined" || s.BribedEnvoy)
0829: 		{
0830: 			return L.T("现在不需要。", "Not needed right now.");
0831: 		}
0832: 		if (Store.GetCash() < 3000)
~~~

~~~csharp
0832: 		if (Store.GetCash() < 3000)
0833: 		{
0834: 			return L.T("现金不够", "Not enough cash");
0835: 		}
0836: 		Store.PayMoney(3000, true);
~~~

~~~csharp
0845: 		if (s.Ch3 != "joined" || s.ReconciledRebels)
0846: 		{
0847: 			return L.T("现在不需要。", "Not needed right now.");
0848: 		}
0849: 		if (Store.GetCash() < 1000)
~~~

~~~csharp
0849: 		if (Store.GetCash() < 1000)
0850: 		{
0851: 			return L.T("现金不够", "Not enough cash");
0852: 		}
0853: 		Store.PayMoney(1000, true);
~~~

~~~csharp
0863: 		{
0864: 			s.RebelCut = false;
0865: 			Notify(L.T("各办各事。反抗军恢复了联系，但不再给你安排地下买家。", "Each to their own. The Resistance is back in touch, but no longer sends you underground buyers."));
0866: 		}
0867: 	}
~~~

~~~csharp
0920: 			return null;
0921: 		}
0922: 		string text = L.T("导师", "The Mentor");
0923: 		Dialogue obj = new Dialogue().SetText(text, L.T("你甘心做巨企的看门人吗？", "Are you content to be the corporations' gatekeeper?"));
0924: 		Dialogue val = new Dialogue().SetText("PLAYER", L.T("当然，我很愿意。", "Of course. Gladly."));
~~~

~~~csharp
0921: 		}
0922: 		string text = L.T("导师", "The Mentor");
0923: 		Dialogue obj = new Dialogue().SetText(text, L.T("你甘心做巨企的看门人吗？", "Are you content to be the corporations' gatekeeper?"));
0924: 		Dialogue val = new Dialogue().SetText("PLAYER", L.T("当然，我很愿意。", "Of course. Gladly."));
0925: 		Dialogue val2 = new Dialogue().SetText("PLAYER", L.T("（沉默）", "(silence)"));
~~~

~~~csharp
0922: 		string text = L.T("导师", "The Mentor");
0923: 		Dialogue obj = new Dialogue().SetText(text, L.T("你甘心做巨企的看门人吗？", "Are you content to be the corporations' gatekeeper?"));
0924: 		Dialogue val = new Dialogue().SetText("PLAYER", L.T("当然，我很愿意。", "Of course. Gladly."));
0925: 		Dialogue val2 = new Dialogue().SetText("PLAYER", L.T("（沉默）", "(silence)"));
0926: 		val.NextDialogue().SetText(text, L.T("……那就这样吧。", "...So be it.")).SetEndAction(Phone.GuardCall(c, delegate
~~~

~~~csharp
0923: 		Dialogue obj = new Dialogue().SetText(text, L.T("你甘心做巨企的看门人吗？", "Are you content to be the corporations' gatekeeper?"));
0924: 		Dialogue val = new Dialogue().SetText("PLAYER", L.T("当然，我很愿意。", "Of course. Gladly."));
0925: 		Dialogue val2 = new Dialogue().SetText("PLAYER", L.T("（沉默）", "(silence)"));
0926: 		val.NextDialogue().SetText(text, L.T("……那就这样吧。", "...So be it.")).SetEndAction(Phone.GuardCall(c, delegate
0927: 		{
~~~

~~~csharp
0924: 		Dialogue val = new Dialogue().SetText("PLAYER", L.T("当然，我很愿意。", "Of course. Gladly."));
0925: 		Dialogue val2 = new Dialogue().SetText("PLAYER", L.T("（沉默）", "(silence)"));
0926: 		val.NextDialogue().SetText(text, L.T("……那就这样吧。", "...So be it.")).SetEndAction(Phone.GuardCall(c, delegate
0927: 		{
0928: 			s.Ch4 = "willing";
~~~

~~~csharp
0939: 			}
0940: 		}));
0941: 		val2.NextDialogue().SetText(text, L.T("我明白了。我去替你联络。", "I understand. I'll make contact for you.")).SetEndAction(Phone.GuardCall(c, delegate
0942: 		{
0943: 			s.Ch4 = "silent";
~~~

~~~csharp
0954: 			}
0955: 		}));
0956: 		obj.AddChoice(Compat.ChoiceIcon(), L.T("我愿意。", "I am."), "", val, (Action)null, (Func<bool>)null);
0957: 		obj.AddChoice(Compat.ChoiceIcon(), L.T("（沉默）", "(silence)"), "", val2, (Action)null, (Func<bool>)null);
0958: 		return obj;
~~~

~~~csharp
0955: 		}));
0956: 		obj.AddChoice(Compat.ChoiceIcon(), L.T("我愿意。", "I am."), "", val, (Action)null, (Func<bool>)null);
0957: 		obj.AddChoice(Compat.ChoiceIcon(), L.T("（沉默）", "(silence)"), "", val2, (Action)null, (Func<bool>)null);
0958: 		return obj;
0959: 	}
~~~

~~~csharp
0972: 		{
0973: 		}
0974: 		After(2f, L.T("你听到一声枪响。", "You hear a gunshot."));
0975: 		Later(L.T("你成了下层区的巨企代理人：所有公司服务免费，订货八折。", "You are now the corporations' agent for the Lower Levels: all company services are free, and orders are 20% off."));
0976: 	}
~~~

~~~csharp
0973: 		}
0974: 		After(2f, L.T("你听到一声枪响。", "You hear a gunshot."));
0975: 		Later(L.T("你成了下层区的巨企代理人：所有公司服务免费，订货八折。", "You are now the corporations' agent for the Lower Levels: all company services are free, and orders are 20% off."));
0976: 	}
0977: 
~~~

~~~csharp
1026: 	public static bool DebugGreet()
1027: 	{
1028: 		StoreClient val = Talk("greet", L.T("撒该", "Zacchaeus"), Sprite, new string[2]
1029: 		{
1030: 			L.T("（测试）只是路过，看看您的店。", "(Test) Just passing by to see your shop."),
~~~

~~~csharp
1028: 		StoreClient val = Talk("greet", L.T("撒该", "Zacchaeus"), Sprite, new string[2]
1029: 		{
1030: 			L.T("（测试）只是路过，看看您的店。", "(Test) Just passing by to see your shop."),
1031: 			L.T("（测试）生意兴隆。", "(Test) Business is booming.")
1032: 		}, null, null, null);
~~~

~~~csharp
1029: 		{
1030: 			L.T("（测试）只是路过，看看您的店。", "(Test) Just passing by to see your shop."),
1031: 			L.T("（测试）生意兴隆。", "(Test) Business is booming.")
1032: 		}, null, null, null);
1033: 		if (val == null)
~~~

~~~csharp
1104: 			if (flag)
1105: 			{
1106: 				Notify(L.T($"撒该的电话（{company.PhoneNumber}）记在了电话旁的记事簿上。", $"Zacchaeus's number ({company.PhoneNumber}) is now in the notebook by the phone."));
1107: 			}
1108: 		}
~~~

~~~csharp
1193: 				}
1194: 			};
1195: 			Dialogue val2 = new Dialogue().SetText(name, lines[0]);
1196: 			Dialogue val3 = val2;
1197: 			for (int num = 1; num < lines.Length; num++)
~~~

~~~csharp
1197: 			for (int num = 1; num < lines.Length; num++)
1198: 			{
1199: 				val3 = val3.NextDialogue().SetText(name, lines[num]);
1200: 			}
1201: 			if (!string.IsNullOrEmpty(ask) && choices != null && choices.Length != 0)
~~~

~~~csharp
1201: 			if (!string.IsNullOrEmpty(ask) && choices != null && choices.Length != 0)
1202: 			{
1203: 				Dialogue val4 = val3.NextDialogue().SetText(name, ask);
1204: 				for (int num2 = 0; num2 < choices.Length; num2++)
1205: 				{
~~~

~~~csharp
1208: 					string item = tuple.Item2;
1209: 					Action item2 = tuple.Item3;
1210: 					Dialogue val5 = new Dialogue().SetText("PLAYER", label);
1211: 					val5.NextDialogue().SetText(name, item).SetEndAction(endAction);
1212: 					Action act = item2;
~~~

~~~csharp
1209: 					Action item2 = tuple.Item3;
1210: 					Dialogue val5 = new Dialogue().SetText("PLAYER", label);
1211: 					val5.NextDialogue().SetText(name, item).SetEndAction(endAction);
1212: 					Action act = item2;
1213: 					val4.AddChoice(Compat.ChoiceIcon(), label, "", val5, (Action)delegate
~~~

## CorpPresence/Malice/EnvoyCo.cs

~~~csharp
0005: 	public override string Id => "envoy";
0006: 
0007: 	public override string Name => L.T("联合站区管理处", "Joint District Administration");
0008: 
0009: 	public override string CharacterId => "envoy";
~~~

~~~csharp
0011: 	public override long PhoneNumber => 77225L;
0012: 
0013: 	public override string[] TierNames => L.T(new string[3] { "登记", "合作", "伙伴" }, new string[3] { "Registered", "Associate", "Partner" });
0014: 
0015: 	public override string CardFlavor => "";
~~~

~~~csharp
0015: 	public override string CardFlavor => "";
0016: 
0017: 	public override string DeskName => L.T("撒该", "Zacchaeus");
0018: 
0019: 	public override string DeskAsk => "";
~~~

~~~csharp
0019: 	public override string DeskAsk => "";
0020: 
0021: 	public override string[] FrontDesk => L.T(new string[2] { "撒该·索恩。请讲。", "您好，这里是联合站区管理处。" }, new string[2] { "Zacchaeus Thorne. Go ahead.", "Joint District Administration. How can I help?" });
0022: 
0023: 	public override string FrontDeskDone => L.T("已经记下了。", "Noted.");
~~~

~~~csharp
0021: 	public override string[] FrontDesk => L.T(new string[2] { "撒该·索恩。请讲。", "您好，这里是联合站区管理处。" }, new string[2] { "Zacchaeus Thorne. Go ahead.", "Joint District Administration. How can I help?" });
0022: 
0023: 	public override string FrontDeskDone => L.T("已经记下了。", "Noted.");
0024: 
0025: 	public override bool Active
~~~

## CorpPresence/UI/Notebook.cs

~~~csharp
0284: 			((Graphic)_paper).color = Dim;
0285: 			((Graphic)_paper).raycastTarget = true;
0286: 			Label(_root.transform, L.T("记事簿", "Notebook"), new Vector2(22f, 16f), 216f, Ink, (TextAlignmentOptions)513, bold: true);
0287: 			_pageLabel = Label(_root.transform, "", new Vector2(86f, 192f), 80f, InkSoft, (TextAlignmentOptions)514);
0288: 			GameObject val3 = new GameObject("Rows");
~~~

## CorpPresence/UI/CompanyWindow.cs

~~~csharp
0243: 		{
0244: 			Ui.Image(r, theme.Banner);
0245: 			GUI.Label(new Rect(((Rect)(ref r)).x + 120f, ((Rect)(ref r)).y + 16f, 420f, 58f), _company.Name + L.T(" · 药品订货单", " · Pharmaceutical Order Form"), theme.Title);
0246: 		}
0247: 		else
~~~

~~~csharp
0249: 			Ui.Fill(new Rect(36f, 34f, 36f, 12f), theme.Accent);
0250: 			Ui.Fill(new Rect(48f, 22f, 12f, 36f), theme.Accent);
0251: 			GUI.Label(new Rect(88f, 18f, 520f, 44f), _company.Name + L.T(" · 药品订货单", " · Pharmaceutical Order Form"), theme.Title);
0252: 		}
0253: 		GUI.Label(new Rect(width - 260f, 30f, 220f, 22f), L.En ? $"Form CP-{_company.PhoneNumber}   Day {Day}" : $"表单 CP-{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
~~~

~~~csharp
0254: 		Ui.Fill(new Rect(20f, 96f, width - 40f, 2f), theme.Accent);
0255: 		Ui.Fill(new Rect(20f, 100f, width - 40f, 1f), theme.Line);
0256: 		GUI.Label(new Rect(24f, 104f, width - 48f, 22f), L.T("客服：", "Service: ") + _desk, theme.Body);
0257: 		Rect val = default(Rect);
0258: 		((Rect)(ref val))._002Ector(20f, 134f, 220f, height - 154f);
~~~

~~~csharp
0266: 			Ui.Image(new Rect(((Rect)(ref r2)).x + 2f, ((Rect)(ref r2)).y + 2f, ((Rect)(ref r2)).width - 4f, ((Rect)(ref r2)).height - 4f), val2);
0267: 		}
0268: 		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 4f, 200f, 20f), L.T("经办：伊莎·兰登（市场规范部）", "Handled by: Isa Langdon (Market Compliance)"), theme.Small);
0269: 		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 28f, 200f, 22f), L.En ? ("Your shop: " + TierLabel(state)) : ("贵店：" + TierLabel(state)), theme.Body);
0270: 		CoopBar(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 54f, 200f, 8f));
~~~

~~~csharp
0291: 		if (_company.Tier(state) < 1 && _module < 3)
0292: 		{
0293: 			GUI.Label(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).y, ((Rect)(ref val4)).width, 60f), L.T("贵店尚未登记。和伊莎多打几次交道吧。", "Your shop is not registered yet. Deal with Isa a few more times."), theme.Wrap);
0294: 			return;
0295: 		}
~~~

~~~csharp
0332: 		if (e.Id == "@painblock" && !CarmichaelCo.PainBlockOpen())
0333: 		{
0334: 			return L.T("伊莎第 4 章后", "After Isa chapter 4");
0335: 		}
0336: 		return null;
~~~

~~~csharp
0360: 		CompanyState state = State;
0361: 		bool flag = Services.CanOrderToday(_company);
0362: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 20f), flag ? L.T("今天下单，明天送到。每天一单。", "Order today, delivered tomorrow. One order per day. ") : L.T("今天已经下过单了。", "Already ordered today. "), theme.Small);
0363: 		float x = ((Rect)(ref r)).x;
0364: 		float num = ((Rect)(ref r)).x + 290f;
~~~

~~~csharp
0367: 		float num4 = ((Rect)(ref r)).y + 28f;
0368: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num4, ((Rect)(ref r)).width, 26f), Tex.Hex(13886431));
0369: 		GUI.Label(new Rect(x + 8f, num4, 280f, 26f), L.T("品名", "Item"), theme.Body);
0370: 		GUI.Label(new Rect(num, num4, 90f, 26f), L.T("单价", "Unit price"), theme.Center);
0371: 		GUI.Label(new Rect(num2, num4, 120f, 26f), L.T("数量", "Qty"), theme.Center);
~~~

~~~csharp
0368: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num4, ((Rect)(ref r)).width, 26f), Tex.Hex(13886431));
0369: 		GUI.Label(new Rect(x + 8f, num4, 280f, 26f), L.T("品名", "Item"), theme.Body);
0370: 		GUI.Label(new Rect(num, num4, 90f, 26f), L.T("单价", "Unit price"), theme.Center);
0371: 		GUI.Label(new Rect(num2, num4, 120f, 26f), L.T("数量", "Qty"), theme.Center);
0372: 		GUI.Label(new Rect(num3, num4, ((Rect)(ref r)).xMax - num3, 26f), L.T("小计", "Subtotal"), theme.Center);
~~~

~~~csharp
0369: 		GUI.Label(new Rect(x + 8f, num4, 280f, 26f), L.T("品名", "Item"), theme.Body);
0370: 		GUI.Label(new Rect(num, num4, 90f, 26f), L.T("单价", "Unit price"), theme.Center);
0371: 		GUI.Label(new Rect(num2, num4, 120f, 26f), L.T("数量", "Qty"), theme.Center);
0372: 		GUI.Label(new Rect(num3, num4, ((Rect)(ref r)).xMax - num3, 26f), L.T("小计", "Subtotal"), theme.Center);
0373: 		num4 += 30f;
~~~

~~~csharp
0370: 		GUI.Label(new Rect(num, num4, 90f, 26f), L.T("单价", "Unit price"), theme.Center);
0371: 		GUI.Label(new Rect(num2, num4, 120f, 26f), L.T("数量", "Qty"), theme.Center);
0372: 		GUI.Label(new Rect(num3, num4, ((Rect)(ref r)).xMax - num3, 26f), L.T("小计", "Subtotal"), theme.Center);
0373: 		num4 += 30f;
0374: 		CatalogEntry[] entries = CarmichaelCo.Entries;
~~~

~~~csharp
0393: 			if (num5 <= 0)
0394: 			{
0395: 				GUI.Label(new Rect(num, num4, ((Rect)(ref r)).xMax - num, 34f), L.T("暂不可用", "Unavailable"), theme.Small);
0396: 				Cart.Remove(catalogEntry);
0397: 				num4 += 36f;
~~~

~~~csharp
0419: 		bool enabled2 = GUI.enabled;
0420: 		GUI.enabled = enabled;
0421: 		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num7 + 2f, 160f, 36f), L.T("提交订货单", "Submit order"), theme.BtnHotStyle))
0422: 		{
0423: 			string text3 = Services.PlaceOrder(_company, Cart, CarmUnitPrice);
~~~

~~~csharp
0471: 		float num3 = ((Rect)(ref val)).width - 32f;
0472: 		float num4 = ((Rect)(ref val)).y + 12f;
0473: 		GUI.Label(new Rect(num2, num4, num3, 26f), L.T("申请单 · 体力增强剂（未上市）", "Request Form · Stamina Booster (unreleased)"), theme.Body);
0474: 		num4 += 32f;
0475: 		Ui.Fill(new Rect(num2, num4, num3, 1f), theme.Line);
~~~

~~~csharp
0475: 		Ui.Fill(new Rect(num2, num4, num3, 1f), theme.Line);
0476: 		num4 += 8f;
0477: 		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("申请药房：贵店", "Requesting pharmacy: your shop"), theme.Body);
0478: 		num4 += 26f;
0479: 		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("经办人：伊莎·兰登（市场规范部）", "Handler: Isa Langdon (Market Compliance)"), theme.Body);
~~~

~~~csharp
0477: 		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("申请药房：贵店", "Requesting pharmacy: your shop"), theme.Body);
0478: 		num4 += 26f;
0479: 		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("经办人：伊莎·兰登（市场规范部）", "Handler: Isa Langdon (Market Compliance)"), theme.Body);
0480: 		num4 += 26f;
0481: 		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("药品费用：0₵", "Drug cost: 0₵"), theme.Body);
~~~

~~~csharp
0479: 		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("经办人：伊莎·兰登（市场规范部）", "Handler: Isa Langdon (Market Compliance)"), theme.Body);
0480: 		num4 += 26f;
0481: 		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("药品费用：0₵", "Drug cost: 0₵"), theme.Body);
0482: 		num4 += 26f;
0483: 		GUI.Label(new Rect(num2, num4, 90f, 24f), L.T("手续费：", "Fee:"), theme.Body);
~~~

~~~csharp
0481: 		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("药品费用：0₵", "Drug cost: 0₵"), theme.Body);
0482: 		num4 += 26f;
0483: 		GUI.Label(new Rect(num2, num4, 90f, 24f), L.T("手续费：", "Fee:"), theme.Body);
0484: 		GUI.Label(new Rect(num2 + 70f, num4, 120f, 24f), $"{CarmichaelCo.StaminaFee}₵", theme.Price);
0485: 		num4 += 30f;
~~~

~~~csharp
0492: 		Ui.Fill(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).y, 2f, ((Rect)(ref val2)).height), hot);
0493: 		Ui.Fill(new Rect(((Rect)(ref val2)).xMax - 2f, ((Rect)(ref val2)).y, 2f, ((Rect)(ref val2)).height), hot);
0494: 		GUI.Label(val2, L.T("特别申请", "SPECIAL"), theme.Price);
0495: 		float num5 = ((Rect)(ref val)).yMax + 16f;
0496: 		if (!flag)
~~~

~~~csharp
0496: 		if (!flag)
0497: 		{
0498: 			GUI.Label(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 44f), L.T("新品尚未向贵店配发。伊莎·兰登为贵店争取到配额后开放。", "This new product hasn't been allocated to your shop yet. It opens once Isa Langdon secures you a quota."), theme.Wrap);
0499: 			return;
0500: 		}
~~~

~~~csharp
0500: 		}
0501: 		bool flag2 = Day >= num;
0502: 		GUI.Label(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 22f), flag2 ? L.T("本期配额：可申领。", "Current quota: available.") : (L.En ? $"Next available: day {num} ({num - Day} days left)" : $"下次可申领：第 {num} 天（还有 {num - Day} 天）"), theme.Body);
0503: 		bool enabled = GUI.enabled;
0504: 		GUI.enabled = flag2 && Cash >= CarmichaelCo.StaminaFee;
~~~

~~~csharp
0503: 		bool enabled = GUI.enabled;
0504: 		GUI.enabled = flag2 && Cash >= CarmichaelCo.StaminaFee;
0505: 		if (Ui.Button(new Rect(((Rect)(ref r)).x, num5 + 32f, 260f, 36f), (!flag2) ? L.T("批次未到", "Batch not ready") : (L.En ? $"Submit request (fee {CarmichaelCo.StaminaFee}₵)" : $"提交申请（手续费 {CarmichaelCo.StaminaFee}₵）"), theme.BtnHotStyle))
0506: 		{
0507: 			Toast(Services.RequestStamina(_company) ?? L.T("申请已受理。明早由专人配送。", "Request accepted. Hand-delivered tomorrow morning."));
~~~

~~~csharp
0505: 		if (Ui.Button(new Rect(((Rect)(ref r)).x, num5 + 32f, 260f, 36f), (!flag2) ? L.T("批次未到", "Batch not ready") : (L.En ? $"Submit request (fee {CarmichaelCo.StaminaFee}₵)" : $"提交申请（手续费 {CarmichaelCo.StaminaFee}₵）"), theme.BtnHotStyle))
0506: 		{
0507: 			Toast(Services.RequestStamina(_company) ?? L.T("申请已受理。明早由专人配送。", "Request accepted. Hand-delivered tomorrow morning."));
0508: 		}
0509: 		GUI.enabled = enabled;
~~~

~~~csharp
0551: 			return;
0552: 		}
0553: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 44f), L.T("养殖许可在册。银血三叶蜥养在循环水养殖系统里，成年后能采血；离开水也不会死，只是不积血。原价，明天送到，不限量。", "Breeding license on file. Keep Silverblood Trilobites in an aquaculture system; adults can be bled. Out of water they survive but build no blood. List price, delivered tomorrow, no limit."), theme.Wrap);
0554: 		float num = ((Rect)(ref r)).y + 54f;
0555: 		List<(string, int)> list = new List<(string, int)>
~~~

~~~csharp
0579: 			bool enabled = GUI.enabled;
0580: 			GUI.enabled = Cash >= item2;
0581: 			if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num + 22f, 160f, 36f), L.T("订一份", "Order one"), theme.BtnHotStyle))
0582: 			{
0583: 				Toast(CarmichaelCo.OrderSpecimen(_company, item, item2) ?? _company.FrontDeskDone);
~~~

~~~csharp
0613: 		float num = ((Rect)(ref r)).y + 74f;
0614: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 26f), Tex.Hex(13886431));
0615: 		GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 240f, 26f), L.T("贵店持有的问题药品", "Problem drugs in your shop"), theme.Body);
0616: 		GUI.Label(new Rect(((Rect)(ref r)).x + 250f, num, 110f, 26f), L.T("问题", "Issue"), theme.Center);
0617: 		GUI.Label(new Rect(((Rect)(ref r)).x + 360f, num, ((Rect)(ref r)).width - 360f, 26f), L.T("替换为", "Replace with"), theme.Center);
~~~

~~~csharp
0614: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 26f), Tex.Hex(13886431));
0615: 		GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 240f, 26f), L.T("贵店持有的问题药品", "Problem drugs in your shop"), theme.Body);
0616: 		GUI.Label(new Rect(((Rect)(ref r)).x + 250f, num, 110f, 26f), L.T("问题", "Issue"), theme.Center);
0617: 		GUI.Label(new Rect(((Rect)(ref r)).x + 360f, num, ((Rect)(ref r)).width - 360f, 26f), L.T("替换为", "Replace with"), theme.Center);
0618: 		num += 30f;
~~~

~~~csharp
0615: 		GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 240f, 26f), L.T("贵店持有的问题药品", "Problem drugs in your shop"), theme.Body);
0616: 		GUI.Label(new Rect(((Rect)(ref r)).x + 250f, num, 110f, 26f), L.T("问题", "Issue"), theme.Center);
0617: 		GUI.Label(new Rect(((Rect)(ref r)).x + 360f, num, ((Rect)(ref r)).width - 360f, 26f), L.T("替换为", "Replace with"), theme.Center);
0618: 		num += 30f;
0619: 		if (SwapRows.Count == 0)
~~~

~~~csharp
0619: 		if (SwapRows.Count == 0)
0620: 		{
0621: 			GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, ((Rect)(ref r)).width, 30f), L.T("没有找到。", "None found."), theme.Small);
0622: 			num += 34f;
0623: 		}
~~~

~~~csharp
0626: 			GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 240f, 30f), swapRow.Name, theme.Body);
0627: 			GUI.Label(new Rect(((Rect)(ref r)).x + 250f, num, 110f, 30f), swapRow.Reason, theme.Price);
0628: 			GUI.Label(new Rect(((Rect)(ref r)).x + 360f, num, ((Rect)(ref r)).width - 360f, 30f), L.T("正品", "Genuine ") + swapRow.Target, theme.Center);
0629: 			Ui.Fill(new Rect(((Rect)(ref r)).x, num + 30f, ((Rect)(ref r)).width, 1f), theme.Line);
0630: 			num += 32f;
~~~

~~~csharp
0642: 		bool enabled = GUI.enabled;
0643: 		GUI.enabled = flag && SwapRows.Count > 0 && Cash >= num3;
0644: 		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num4 + 2f, 160f, 36f), L.T("申请换货", "Request swap"), theme.BtnHotStyle))
0645: 		{
0646: 			string text = Services.RequestSwap(_company);
~~~

~~~csharp
0648: 			if (text == null)
0649: 			{
0650: 				text = L.T("问题药品已收回。明早送来正品。", "Problem drugs collected. Genuine replacements arrive tomorrow morning.");
0651: 			}
0652: 			Toast(text);
~~~

~~~csharp
0715: 		if (_brandSel == null || !state.Brand.Contains(_brandSel) || Array.IndexOf(array, _brandSel) < 0)
0716: 		{
0717: 			GUI.Label(new Rect(((Rect)(ref val3)).x + 10f, ((Rect)(ref val3)).y + 10f, ((Rect)(ref val3)).width - 20f, 24f), (num == 0) ? L.T("还没有经手过这家公司的产品。", "You haven't handled any of this company's products yet.") : L.T("点一件已收录的物品，看它的介绍。", "Click a recorded item to read about it."), theme.Small);
0718: 			return;
0719: 		}
~~~

~~~csharp
1158: 			return $"{num - state.Coop} more to “{_company.TierNames[Array.IndexOf(Company.TierAt, num)]}”";
1159: 		}
1160: 		return L.T("已是最高档", "Top tier reached");
1161: 	}
1162: 
~~~

~~~csharp
1204: 		{
1205: 			Ui.Image(r, theme.Banner);
1206: 			GUI.Label(new Rect(((Rect)(ref r)).x + 124f, ((Rect)(ref r)).y + 16f, 416f, 58f), _company.Name + L.T(" · 供货单", " · Supply Order"), theme.Title);
1207: 		}
1208: 		else
~~~

~~~csharp
1209: 		{
1210: 			Ui.Fill(new Rect(20f, 16f, 560f, 64f), theme.Accent);
1211: 			GUI.Label(new Rect(36f, 16f, 520f, 64f), _company.Name + L.T(" · 供货单", " · Supply Order"), theme.InvTitle);
1212: 		}
1213: 		GUI.Label(new Rect(600f, 40f, width - 640f, 22f), L.En ? $"Order DAV-{_company.PhoneNumber}   Day {Day}" : $"单号 DAV-{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
~~~

~~~csharp
1213: 		GUI.Label(new Rect(600f, 40f, width - 640f, 22f), L.En ? $"Order DAV-{_company.PhoneNumber}   Day {Day}" : $"单号 DAV-{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
1214: 		Ui.Fill(new Rect(20f, 100f, width - 40f, 2f), theme.Accent);
1215: 		GUI.Label(new Rect(24f, 106f, width - 48f, 22f), L.T("订货中心：", "Order center: ") + _desk, theme.Body);
1216: 		Rect val = default(Rect);
1217: 		((Rect)(ref val))._002Ector(20f, 136f, 220f, height - 156f);
~~~

~~~csharp
1225: 			Ui.Image(new Rect(((Rect)(ref r2)).x + 2f, ((Rect)(ref r2)).y + 2f, ((Rect)(ref r2)).width - 4f, ((Rect)(ref r2)).height - 4f), val2);
1226: 		}
1227: 		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 4f, 200f, 20f), L.T("供货专员：多琳", "Supply rep: Doreen"), theme.Small);
1228: 		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 28f, 200f, 22f), L.En ? ("Tier: " + TierLabel(state)) : ("档位：" + TierLabel(state)), theme.Body);
1229: 		CoopBar(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 54f, 200f, 8f));
~~~

~~~csharp
1250: 		if (_company.Tier(state) < 1 && _module < 3)
1251: 		{
1252: 			GUI.Label(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).y, ((Rect)(ref val4)).width, 40f), L.T("还没有供货编号。", "No supplier number yet."), theme.Wrap);
1253: 			return;
1254: 		}
~~~

~~~csharp
1298: 		int num = _company.CatalogTier(State);
1299: 		bool flag = Services.CanOrderToday(_company);
1300: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 20f), flag ? L.T("今天下单，明天到货。", "Order today, delivered tomorrow.") : L.T("今天已经下过单了。", "Already ordered today."), theme.Small);
1301: 		float x = ((Rect)(ref r)).x;
1302: 		float num2 = ((Rect)(ref r)).x + 290f;
~~~

~~~csharp
1305: 		float num5 = ((Rect)(ref r)).y + 26f;
1306: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 26f), theme.Accent);
1307: 		GUI.Label(new Rect(x + 8f, num5, 280f, 26f), L.T("品名", "Item"), theme.InvBody);
1308: 		GUI.Label(new Rect(num2, num5, 90f, 26f), L.T("单价", "Unit price"), theme.InvCenter);
1309: 		GUI.Label(new Rect(num3, num5, 120f, 26f), L.T("数量", "Qty"), theme.InvCenter);
~~~

~~~csharp
1306: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 26f), theme.Accent);
1307: 		GUI.Label(new Rect(x + 8f, num5, 280f, 26f), L.T("品名", "Item"), theme.InvBody);
1308: 		GUI.Label(new Rect(num2, num5, 90f, 26f), L.T("单价", "Unit price"), theme.InvCenter);
1309: 		GUI.Label(new Rect(num3, num5, 120f, 26f), L.T("数量", "Qty"), theme.InvCenter);
1310: 		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 26f), L.T("小计", "Subtotal"), theme.InvCenter);
~~~

~~~csharp
1307: 		GUI.Label(new Rect(x + 8f, num5, 280f, 26f), L.T("品名", "Item"), theme.InvBody);
1308: 		GUI.Label(new Rect(num2, num5, 90f, 26f), L.T("单价", "Unit price"), theme.InvCenter);
1309: 		GUI.Label(new Rect(num3, num5, 120f, 26f), L.T("数量", "Qty"), theme.InvCenter);
1310: 		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 26f), L.T("小计", "Subtotal"), theme.InvCenter);
1311: 		num5 += 30f;
~~~

~~~csharp
1308: 		GUI.Label(new Rect(num2, num5, 90f, 26f), L.T("单价", "Unit price"), theme.InvCenter);
1309: 		GUI.Label(new Rect(num3, num5, 120f, 26f), L.T("数量", "Qty"), theme.InvCenter);
1310: 		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 26f), L.T("小计", "Subtotal"), theme.InvCenter);
1311: 		num5 += 30f;
1312: 		CatalogEntry[] entries = DavreCo.Entries;
~~~

~~~csharp
1325: 			if (num6 <= 0)
1326: 			{
1327: 				GUI.Label(new Rect(num2, num5, ((Rect)(ref r)).xMax - num2, 34f), L.T("暂不可用", "Unavailable"), theme.Small);
1328: 				Cart.Remove(catalogEntry);
1329: 				num5 += 36f;
~~~

~~~csharp
1350: 		bool enabled = GUI.enabled;
1351: 		GUI.enabled = num7 > 0 && num7 <= Cash && flag;
1352: 		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num8 + 2f, 160f, 36f), L.T("提交供货单", "Submit order"), theme.BtnHotStyle))
1353: 		{
1354: 			string text = Services.PlaceOrder(_company, Cart, DavreUnitPrice);
~~~

~~~csharp
1382: 		Theme theme = _theme;
1383: 		DavreCo c = (DavreCo)_company;
1384: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 66f), L.T("绞肉机把生肉绞成肉糜，罐装机每晚把肉糜和营养果封成罐头：合成档出“泥炭”，鲜肉档出鲜肉罐头。", "The grinder turns raw meat into mince; each night the canner seals mince and nutrifruit into cans: Peat on Synthetic, fresh meat on Fresh."), theme.Wrap);
1385: 		float num = ((Rect)(ref r)).y + 76f;
1386: 		if (!DavreCo.CanningOpen && !DavreCo.CanningOffered)
~~~

~~~csharp
1403: 			return;
1404: 		}
1405: 		GUI.Label(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 20f), L.T("原价，明天到货，不限量。", "List price, delivered tomorrow, no limit."), theme.Small);
1406: 		num += 28f;
1407: 		(string, string)[] canningMachines = DavreCo.CanningMachines;
~~~

~~~csharp
1424: 			bool enabled2 = GUI.enabled;
1425: 			GUI.enabled = Cash >= num3;
1426: 			if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num + 30f, 160f, 36f), L.T("订一台", "Order one"), theme.BtnHotStyle))
1427: 			{
1428: 				Toast(Services.OrderCanningMachine(c, item, num3) ?? _company.FrontDeskDone);
~~~

~~~csharp
1463: 		if (!DavreCo.LeviathanOpen)
1464: 		{
1465: 			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 60f), L.T("达夫雷自己的净水器，只供给卖水的店。", "Davre's own water purifier, supplied only to shops that sell water."), theme.Wrap);
1466: 			return;
1467: 		}
~~~

~~~csharp
1469: 		int num2 = DavreCo.LeviathanUsed(state);
1470: 		int num3 = LeviathanPrice();
1471: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 66f), L.T("照着海德拉做的。铁锈水、幽灵水都能净化成基础水，沟渠水不行。耐久 600，能净化 60 升。\n", "Modeled on the HydraTech. Purifies Rustwater and Ghostwater to Basewater, but not Gutterflow. 600 durability, good for 60 liters.\n") + (L.En ? $"{num} per restock period (2 from {_company.TierNames[2]}), {num3}₵ each, delivered tomorrow." : $"每个补货周期可订 {num} 台（{_company.TierNames[2]}起 2 台），每台 {num3}₵，明天到货。"), theme.Wrap);
1472: 		Texture2D icon = Brand.Info("@leviathan").Icon;
1473: 		Rect val = default(Rect);
~~~

~~~csharp
1485: 		bool enabled = GUI.enabled;
1486: 		GUI.enabled = num3 > 0 && num2 < num && Cash >= num3;
1487: 		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num4 + 2f, 160f, 36f), L.T("订一台", "Order one"), theme.BtnHotStyle))
1488: 		{
1489: 			Toast(Services.OrderLeviathan(davreCo, num3) ?? _company.FrontDeskDone);
~~~

~~~csharp
1520: 		Ui.Fill(new Rect(20f, 20f, width - 40f, 56f), theme.Ink);
1521: 		GUI.Label(new Rect(36f, 20f, width - 80f, 56f), _company.Name, theme.InvTitle);
1522: 		GUI.Label(new Rect(24f, 90f, width - 48f, 22f), L.T("撒该：", "Zacchaeus: ") + _desk, theme.Body);
1523: 		Ui.Fill(new Rect(20f, 116f, width - 40f, 1f), theme.Line);
1524: 		if (envoyState == null)
~~~

~~~csharp
1529: 		((Rect)(ref val))._002Ector(30f, 130f, width - 60f, ((Rect)(ref _rect)).height - 150f);
1530: 		float y = ((Rect)(ref val)).y;
1531: 		GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 24f), L.T("援助", "Aid"), theme.Title);
1532: 		y += 30f;
1533: 		string text = envoyState.Aid switch
~~~

~~~csharp
1536: 			"repay5" => L.En ? $"Repaying: {envoyState.RepayLeft} installments of {800}₵ left; he collects on day {envoyState.RepayDue}. Missing one brings a penalty." : $"还款中：还剩 {envoyState.RepayLeft} 期，每期 {800}₵，第 {envoyState.RepayDue} 天他来收。交不上会受罚。", 
1537: 			"plan6" => (L.En ? $"Repayment plan: {envoyState.RepayLeft} installments of {800}₵ left; he collects on day {envoyState.RepayDue}." : $"还款计划：还剩 {envoyState.RepayLeft} 期，每期 {800}₵，第 {envoyState.RepayDue} 天他来收。") + ((envoyState.LateWeeks <= 0) ? "" : (L.En ? $" {envoyState.LateWeeks} week(s) late: corporate orders +{envoyState.LateWeeks * 10}%." : $"已拖 {envoyState.LateWeeks} 周，巨企订货 +{envoyState.LateWeeks * 10}%。")), 
1538: 			"punished" => L.T("惩罚中：所有公司服务暂停（反抗军除外），每天少一位顾客。", "Penalty: all company services are suspended (except the Resistance), and one fewer customer comes each day."), 
1539: 			"done" => L.T("已结清。", "Settled."), 
1540: 			"declined" => L.T("你拒绝了援助。", "You declined the aid."), 
~~~

~~~csharp
1537: 			"plan6" => (L.En ? $"Repayment plan: {envoyState.RepayLeft} installments of {800}₵ left; he collects on day {envoyState.RepayDue}." : $"还款计划：还剩 {envoyState.RepayLeft} 期，每期 {800}₵，第 {envoyState.RepayDue} 天他来收。") + ((envoyState.LateWeeks <= 0) ? "" : (L.En ? $" {envoyState.LateWeeks} week(s) late: corporate orders +{envoyState.LateWeeks * 10}%." : $"已拖 {envoyState.LateWeeks} 周，巨企订货 +{envoyState.LateWeeks * 10}%。")), 
1538: 			"punished" => L.T("惩罚中：所有公司服务暂停（反抗军除外），每天少一位顾客。", "Penalty: all company services are suspended (except the Resistance), and one fewer customer comes each day."), 
1539: 			"done" => L.T("已结清。", "Settled."), 
1540: 			"declined" => L.T("你拒绝了援助。", "You declined the aid."), 
1541: 			_ => L.T("无。", "None."), 
~~~

~~~csharp
1538: 			"punished" => L.T("惩罚中：所有公司服务暂停（反抗军除外），每天少一位顾客。", "Penalty: all company services are suspended (except the Resistance), and one fewer customer comes each day."), 
1539: 			"done" => L.T("已结清。", "Settled."), 
1540: 			"declined" => L.T("你拒绝了援助。", "You declined the aid."), 
1541: 			_ => L.T("无。", "None."), 
1542: 		};
~~~

~~~csharp
1539: 			"done" => L.T("已结清。", "Settled."), 
1540: 			"declined" => L.T("你拒绝了援助。", "You declined the aid."), 
1541: 			_ => L.T("无。", "None."), 
1542: 		};
1543: 		GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 44f), text, theme.Wrap);
~~~

~~~csharp
1555: 			Ui.Fill(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 1f), theme.Line);
1556: 			y += 8f;
1557: 			GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 24f), L.T("服务费", "Service fee"), theme.Title);
1558: 			y += 30f;
1559: 			string text2 = envoyState.Ch2 switch
~~~

~~~csharp
1559: 			string text2 = envoyState.Ch2 switch
1560: 			{
1561: 				"refused" => L.T("你拒绝了他。巨企的订货暂不开放，直到你交钱或收下礼物并办成。", "You turned him down. Corporate ordering stays closed until you pay or take the gift and deliver it."), 
1562: 				"bomb" => L.En ? $"Sell the gift to Odin before day {envoyState.BombDeadline} (call him over; if he's been arrested, sell it through the underground buyer)." : $"礼物要在第 {envoyState.BombDeadline} 天之前卖给奥丁（打他的电话叫他来；他被捕了就通过地下买家卖出）。", 
1563: 				"bombdone" => L.T("礼物送到了。巨企订购服务不收订阅费。", "Gift delivered. Corporate ordering services are subscription-free."), 
~~~

~~~csharp
1561: 				"refused" => L.T("你拒绝了他。巨企的订货暂不开放，直到你交钱或收下礼物并办成。", "You turned him down. Corporate ordering stays closed until you pay or take the gift and deliver it."), 
1562: 				"bomb" => L.En ? $"Sell the gift to Odin before day {envoyState.BombDeadline} (call him over; if he's been arrested, sell it through the underground buyer)." : $"礼物要在第 {envoyState.BombDeadline} 天之前卖给奥丁（打他的电话叫他来；他被捕了就通过地下买家卖出）。", 
1563: 				"bombdone" => L.T("礼物送到了。巨企订购服务不收订阅费。", "Gift delivered. Corporate ordering services are subscription-free."), 
1564: 				"malice" => L.En ? $"Corporate malice: channel fees until day {envoyState.ChannelUntil}. {envoyState.ChannelFee}₵ so far this week; he collects on day {envoyState.ChannelDue}." : $"巨企恶意：渠道费收到第 {envoyState.ChannelUntil} 天，这一期已记 {envoyState.ChannelFee}₵，第 {envoyState.ChannelDue} 天他来收。", 
1565: 				_ => L.T("已结清。", "Settled."), 
~~~

~~~csharp
1563: 				"bombdone" => L.T("礼物送到了。巨企订购服务不收订阅费。", "Gift delivered. Corporate ordering services are subscription-free."), 
1564: 				"malice" => L.En ? $"Corporate malice: channel fees until day {envoyState.ChannelUntil}. {envoyState.ChannelFee}₵ so far this week; he collects on day {envoyState.ChannelDue}." : $"巨企恶意：渠道费收到第 {envoyState.ChannelUntil} 天，这一期已记 {envoyState.ChannelFee}₵，第 {envoyState.ChannelDue} 天他来收。", 
1565: 				_ => L.T("已结清。", "Settled."), 
1566: 			};
1567: 			GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 44f), text2, theme.Wrap);
~~~

~~~csharp
1573: 					Toast(Envoy.PayServiceFee() ?? _company.FrontDeskDone);
1574: 				}
1575: 				if (Ui.Button(new Rect(((Rect)(ref val)).x + 230f, y, 220f, 34f), L.T("收下礼物", "Take the gift"), theme.BtnStyle))
1576: 				{
1577: 					Envoy.AcceptBomb();
~~~

~~~csharp
1576: 				{
1577: 					Envoy.AcceptBomb();
1578: 					Toast(L.T("礼物放在柜台上了。", "The gift is on your counter."));
1579: 				}
1580: 				y += 42f;
~~~

~~~csharp
1585: 			Ui.Fill(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 1f), theme.Line);
1586: 			y += 8f;
1587: 			GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 24f), L.T("各办各事", "Each to their own"), theme.Title);
1588: 			y += 30f;
1589: 			GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 44f), L.En ? $"Pay Zacchaeus {3000}₵, then 1000₵ to make peace on the Resistance's line: the Resistance is back in touch (no more underground buyers) and your corporate perks stay." : $"给撒该 {3000}₵，再在反抗军的电话里花 1000₵ 和解：反抗军恢复联系（不再有地下买家），巨企的待遇照旧。", theme.Wrap);
~~~

~~~csharp
1595: 			if (envoyState.BribedEnvoy)
1596: 			{
1597: 				GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 24f), L.T("已打点。", "Paid."), theme.Small);
1598: 			}
1599: 		}
~~~

~~~csharp
1639: 		GUI.Label(new Rect(600f, 40f, width - 640f, 22f), L.En ? $"{_company.PhoneNumber}   Day {Day}" : $"{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
1640: 		Ui.Fill(new Rect(20f, 100f, width - 40f, 2f), theme.Ink);
1641: 		GUI.Label(new Rect(24f, 106f, width - 48f, 22f), L.T("奥丁：", "Odin: ") + _desk, theme.Body);
1642: 		Rect val = default(Rect);
1643: 		((Rect)(ref val))._002Ector(20f, 136f, 190f, height - 156f);
~~~

~~~csharp
1677: 		{
1678: 			EnvoyState envoyState = maliceState?.Envoy;
1679: 			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 80f), L.T("你加入了巨企，反抗军和你断了联系。\n花 1000₵ 和解（还要在撒该那边打点 3000₵）：反抗军恢复联系，但不再给你安排地下买家。", "You joined the corporations and the Resistance cut you off.\nPay 1000₵ to make peace (and pay off Zacchaeus 3000₵ on his line): the Resistance is back in touch, but no more underground buyers."), theme.Wrap);
1680: 			if (envoyState != null && envoyState.ReconciledRebels)
1681: 			{
~~~

~~~csharp
1680: 			if (envoyState != null && envoyState.ReconciledRebels)
1681: 			{
1682: 				GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 90f, ((Rect)(ref r)).width, 24f), envoyState.Gone ? L.T("已付。明天开店时恢复联系。", "Paid. Back in touch from tomorrow's opening.") : L.T("已付。等撒该那边也打点好。", "Paid. Waiting on Zacchaeus's side."), theme.Small);
1683: 			}
1684: 			else if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 90f, 240f, 34f), L.T("和解（1000₵）", "Make peace (1000₵)"), theme.BtnHotStyle))
~~~

~~~csharp
1682: 				GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 90f, ((Rect)(ref r)).width, 24f), envoyState.Gone ? L.T("已付。明天开店时恢复联系。", "Paid. Back in touch from tomorrow's opening.") : L.T("已付。等撒该那边也打点好。", "Paid. Waiting on Zacchaeus's side."), theme.Small);
1683: 			}
1684: 			else if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 90f, 240f, 34f), L.T("和解（1000₵）", "Make peace (1000₵)"), theme.BtnHotStyle))
1685: 			{
1686: 				Toast(Envoy.ReconcileWithRebels() ?? _company.FrontDeskDone);
~~~

~~~csharp
1689: 		else if (_company.Tier(state) < 1)
1690: 		{
1691: 			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 40f), L.T("没人认识你。", "Nobody knows you."), theme.Wrap);
1692: 		}
1693: 		else if (_module == 0)
~~~

~~~csharp
1695: 			DrawSpacerOrder(r, "军械");
1696: 		}
1697: 		else if (rebelModules[_module] == L.T("任务", "Tasks"))
1698: 		{
1699: 			DrawRebelTasks(r);
~~~

~~~csharp
1736: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 64f), L.En ? $"Odin has been arrested; orders and the fence network are suspended.\nTo get him out, the Resistance needs {350}₵ worth of medical supplies and weapons. Call for a pickup when ready; someone comes the next day. You can hand them over in several trips (no payment)." : $"奥丁被捕了，订货和地下销赃网络都停了。\n要救他出来，反抗军需要价值 {350}₵ 的医药和武器。准备好了就呼叫提货，第二天有人来取，可以分几次交（交东西不给钱）。", theme.Wrap);
1737: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 72f, ((Rect)(ref r)).width, 22f), L.En ? $"Delivered: {rebelState?.RescueValue ?? 0} / {350}₵" : $"已交：{rebelState?.RescueValue ?? 0} / {350}₵", theme.Price);
1738: 		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 104f, 240f, 34f), L.T("呼叫提货", "Call for pickup"), theme.BtnHotStyle))
1739: 		{
1740: 			Toast(RebelStory.CallPickup() ?? L.T("明天有人来取。", "Someone will come tomorrow."));
~~~

~~~csharp
1738: 		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 104f, 240f, 34f), L.T("呼叫提货", "Call for pickup"), theme.BtnHotStyle))
1739: 		{
1740: 			Toast(RebelStory.CallPickup() ?? L.T("明天有人来取。", "Someone will come tomorrow."));
1741: 		}
1742: 	}
~~~

~~~csharp
1755: 			{
1756: 				1 => L.En ? $"Odin wants a bottle of poison, or {100}₵ worth of Dream Dust and Phanta Pills. Delivered so far: {rebelState.Ch1Value}₵." : $"奥丁要一瓶毒药，或者价值 {100}₵ 的梦尘和芬太片。已交梦尘、芬太片 {rebelState.Ch1Value}₵。", 
1757: 				2 => (rebelState.Ch2Route == "bribe") ? L.T("已经打点了子午线：炸弹送到后，奥丁会来买。", "Meridian has been paid off: once the bomb arrives, Odin will come for it.") : (L.En ? $"Odin wants explosives and Meridian heavy firepower: call Meridian and pay them off on the Corporate Terms page ({RebelStory.Ch2BribePrice()}₵), or sell {1000}₵ of weapons through the underground buyer ({rebelState.Ch2FenceWeapons}₵ so far)." : $"奥丁要炸药和子午线的重火力：打子午线的电话，在“巨企合约”页打点（{RebelStory.Ch2BribePrice()}₵）；或者通过地下买家卖出 {1000}₵ 的武器（已卖 {rebelState.Ch2FenceWeapons}₵）。"), 
1758: 				3 => L.En ? $"The crackdown left many wounded. Odin comes weekly for medical supplies: {1200}₵ in total ({rebelState.Ch3Medical}₵ so far)." : $"清缴之后伤员很多。奥丁每周来收医疗品，累计 {1200}₵（已卖 {rebelState.Ch3Medical}₵）。", 
1759: 				4 => (!rebelState.Joined) ? L.T("奥丁邀请你加入反抗军。想好了就叫他来。", "Odin has invited you to join the Resistance. Call him over when you've decided.") : (L.En ? $"Odin comes weekly for weapons and medical supplies: {3000}₵ in total ({rebelState.Ch4Sales}₵ so far)." : $"奥丁每周来收武器和医疗品，累计 {3000}₵（已卖 {rebelState.Ch4Sales}₵）。"), 
~~~

~~~csharp
1757: 				2 => (rebelState.Ch2Route == "bribe") ? L.T("已经打点了子午线：炸弹送到后，奥丁会来买。", "Meridian has been paid off: once the bomb arrives, Odin will come for it.") : (L.En ? $"Odin wants explosives and Meridian heavy firepower: call Meridian and pay them off on the Corporate Terms page ({RebelStory.Ch2BribePrice()}₵), or sell {1000}₵ of weapons through the underground buyer ({rebelState.Ch2FenceWeapons}₵ so far)." : $"奥丁要炸药和子午线的重火力：打子午线的电话，在“巨企合约”页打点（{RebelStory.Ch2BribePrice()}₵）；或者通过地下买家卖出 {1000}₵ 的武器（已卖 {rebelState.Ch2FenceWeapons}₵）。"), 
1758: 				3 => L.En ? $"The crackdown left many wounded. Odin comes weekly for medical supplies: {1200}₵ in total ({rebelState.Ch3Medical}₵ so far)." : $"清缴之后伤员很多。奥丁每周来收医疗品，累计 {1200}₵（已卖 {rebelState.Ch3Medical}₵）。", 
1759: 				4 => (!rebelState.Joined) ? L.T("奥丁邀请你加入反抗军。想好了就叫他来。", "Odin has invited you to join the Resistance. Call him over when you've decided.") : (L.En ? $"Odin comes weekly for weapons and medical supplies: {3000}₵ in total ({rebelState.Ch4Sales}₵ so far)." : $"奥丁每周来收武器和医疗品，累计 {3000}₵（已卖 {rebelState.Ch4Sales}₵）。"), 
1760: 				5 => L.En ? $"The Mentor is negotiating with the corporations. Each corporation at 100 cooperation sends a Coin of Fate ({maliceState.Envoy.Coins.Count} / 5)." : $"导师在替你和巨企谈。合作度满 100 的巨企会派人送命运硬币（已收 {maliceState.Envoy.Coins.Count} / 5）。", 
1761: 				_ => (rebelState.Done >= 5) ? L.T("你是下层区的代表。", "You are the Lower Levels' representative.") : (L.En ? $"No task right now. Keep dealing with the Resistance ({maliceState.RebelEarned} support points earned)." : $"暂时没有任务。多和反抗军打交道（助力点累计 {maliceState.RebelEarned}）。"), 
~~~

~~~csharp
1759: 				4 => (!rebelState.Joined) ? L.T("奥丁邀请你加入反抗军。想好了就叫他来。", "Odin has invited you to join the Resistance. Call him over when you've decided.") : (L.En ? $"Odin comes weekly for weapons and medical supplies: {3000}₵ in total ({rebelState.Ch4Sales}₵ so far)." : $"奥丁每周来收武器和医疗品，累计 {3000}₵（已卖 {rebelState.Ch4Sales}₵）。"), 
1760: 				5 => L.En ? $"The Mentor is negotiating with the corporations. Each corporation at 100 cooperation sends a Coin of Fate ({maliceState.Envoy.Coins.Count} / 5)." : $"导师在替你和巨企谈。合作度满 100 的巨企会派人送命运硬币（已收 {maliceState.Envoy.Coins.Count} / 5）。", 
1761: 				_ => (rebelState.Done >= 5) ? L.T("你是下层区的代表。", "You are the Lower Levels' representative.") : (L.En ? $"No task right now. Keep dealing with the Resistance ({maliceState.RebelEarned} support points earned)." : $"暂时没有任务。多和反抗军打交道（助力点累计 {maliceState.RebelEarned}）。"), 
1762: 			};
1763: 			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 90f), text, theme.Wrap);
~~~

~~~csharp
1763: 			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 90f), text, theme.Wrap);
1764: 			float num = ((Rect)(ref r)).y + 100f;
1765: 			if (RebelStory.PickupOpen && Ui.Button(new Rect(((Rect)(ref r)).x, num, 240f, 34f), L.T("呼叫提货", "Call for pickup"), theme.BtnHotStyle))
1766: 			{
1767: 				Toast(RebelStory.CallPickup() ?? L.T("明天有人来取。", "Someone will come tomorrow."));
~~~

~~~csharp
1765: 			if (RebelStory.PickupOpen && Ui.Button(new Rect(((Rect)(ref r)).x, num, 240f, 34f), L.T("呼叫提货", "Call for pickup"), theme.BtnHotStyle))
1766: 			{
1767: 				Toast(RebelStory.CallPickup() ?? L.T("明天有人来取。", "Someone will come tomorrow."));
1768: 			}
1769: 			if (RebelStory.CanCallOdin && Ui.Button(new Rect(((Rect)(ref r)).x, num, 240f, 34f), L.T("呼叫奥丁", "Call Odin"), theme.BtnHotStyle))
~~~

~~~csharp
1767: 				Toast(RebelStory.CallPickup() ?? L.T("明天有人来取。", "Someone will come tomorrow."));
1768: 			}
1769: 			if (RebelStory.CanCallOdin && Ui.Button(new Rect(((Rect)(ref r)).x, num, 240f, 34f), L.T("呼叫奥丁", "Call Odin"), theme.BtnHotStyle))
1770: 			{
1771: 				Toast(RebelStory.CallOdin() ?? L.T("他明天来。", "He'll come tomorrow."));
~~~

~~~csharp
1769: 			if (RebelStory.CanCallOdin && Ui.Button(new Rect(((Rect)(ref r)).x, num, 240f, 34f), L.T("呼叫奥丁", "Call Odin"), theme.BtnHotStyle))
1770: 			{
1771: 				Toast(RebelStory.CallOdin() ?? L.T("他明天来。", "He'll come tomorrow."));
1772: 			}
1773: 		}
~~~

~~~csharp
1818: 		{
1819: 			Ui.Image(r, theme.Banner);
1820: 			GUI.Label(new Rect(((Rect)(ref r)).x + 120f, ((Rect)(ref r)).y + 16f, 420f, 58f), _company.Name + L.T(" · 军械申领表", " · Ordnance Requisition Form"), theme.InvTitle);
1821: 		}
1822: 		else
~~~

~~~csharp
1824: 			Ui.Fill(new Rect(20f, 16f, 560f, 64f), theme.Ink);
1825: 			Ui.Fill(new Rect(20f, 80f, 560f, 4f), theme.Hot);
1826: 			GUI.Label(new Rect(36f, 16f, 520f, 64f), _company.Name + L.T(" · 军械申领表", " · Ordnance Requisition Form"), theme.InvTitle);
1827: 		}
1828: 		GUI.Label(new Rect(600f, 40f, width - 640f, 22f), L.En ? $"Form MRD-{_company.PhoneNumber}   Day {Day}" : $"表 MRD-{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
~~~

~~~csharp
1828: 		GUI.Label(new Rect(600f, 40f, width - 640f, 22f), L.En ? $"Form MRD-{_company.PhoneNumber}   Day {Day}" : $"表 MRD-{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
1829: 		Ui.Fill(new Rect(20f, 100f, width - 40f, 2f), theme.Ink);
1830: 		GUI.Label(new Rect(24f, 106f, width - 48f, 22f), L.T("订货处：", "Order desk: ") + _desk, theme.Body);
1831: 		Rect val = default(Rect);
1832: 		((Rect)(ref val))._002Ector(20f, 136f, 220f, height - 156f);
~~~

~~~csharp
1840: 			Ui.Image(new Rect(((Rect)(ref r2)).x + 2f, ((Rect)(ref r2)).y + 2f, ((Rect)(ref r2)).width - 4f, ((Rect)(ref r2)).height - 4f), val2);
1841: 		}
1842: 		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 4f, 200f, 20f), L.T("经办：罗森（驻站负责人）", "Handled by: Rosen (Station Manager)"), theme.Small);
1843: 		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 28f, 200f, 22f), L.En ? ("License: " + TierLabel(state)) : ("许可：" + TierLabel(state)), theme.Body);
1844: 		CoopBar(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 54f, 200f, 8f));
~~~

~~~csharp
1865: 		if (_company.Tier(state) < 1 && _module < 3)
1866: 		{
1867: 			GUI.Label(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).y, ((Rect)(ref val4)).width, 40f), L.T("没有许可。", "No license."), theme.Wrap);
1868: 			return;
1869: 		}
~~~

~~~csharp
1871: 		{
1872: 		case 0:
1873: 			DrawMeridianOrder(val4, MeridianCo.Entries, L.T("今天下单，明天到。全新出厂。", "Order today, here tomorrow. Factory new."));
1874: 			break;
1875: 		case 1:
~~~

~~~csharp
1929: 		int num = _company.CatalogTier(State);
1930: 		bool flag = Services.CanOrderToday(_company);
1931: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 20f), flag ? note : L.T("今天已经下过单了。", "Already ordered today."), theme.Small);
1932: 		float x = ((Rect)(ref r)).x;
1933: 		float num2 = ((Rect)(ref r)).x + 290f;
~~~

~~~csharp
1936: 		float num5 = ((Rect)(ref r)).y + 26f;
1937: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 26f), theme.Ink);
1938: 		GUI.Label(new Rect(x + 8f, num5, 280f, 26f), L.T("品名", "Item"), theme.InvBody);
1939: 		GUI.Label(new Rect(num2, num5, 90f, 26f), L.T("单价", "Unit price"), theme.InvCenter);
1940: 		GUI.Label(new Rect(num3, num5, 120f, 26f), L.T("数量", "Qty"), theme.InvCenter);
~~~

~~~csharp
1937: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 26f), theme.Ink);
1938: 		GUI.Label(new Rect(x + 8f, num5, 280f, 26f), L.T("品名", "Item"), theme.InvBody);
1939: 		GUI.Label(new Rect(num2, num5, 90f, 26f), L.T("单价", "Unit price"), theme.InvCenter);
1940: 		GUI.Label(new Rect(num3, num5, 120f, 26f), L.T("数量", "Qty"), theme.InvCenter);
1941: 		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 26f), L.T("小计", "Subtotal"), theme.InvCenter);
~~~

~~~csharp
1938: 		GUI.Label(new Rect(x + 8f, num5, 280f, 26f), L.T("品名", "Item"), theme.InvBody);
1939: 		GUI.Label(new Rect(num2, num5, 90f, 26f), L.T("单价", "Unit price"), theme.InvCenter);
1940: 		GUI.Label(new Rect(num3, num5, 120f, 26f), L.T("数量", "Qty"), theme.InvCenter);
1941: 		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 26f), L.T("小计", "Subtotal"), theme.InvCenter);
1942: 		num5 += 30f;
~~~

~~~csharp
1939: 		GUI.Label(new Rect(num2, num5, 90f, 26f), L.T("单价", "Unit price"), theme.InvCenter);
1940: 		GUI.Label(new Rect(num3, num5, 120f, 26f), L.T("数量", "Qty"), theme.InvCenter);
1941: 		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 26f), L.T("小计", "Subtotal"), theme.InvCenter);
1942: 		num5 += 30f;
1943: 		foreach (CatalogEntry catalogEntry in entries)
~~~

~~~csharp
1955: 			if (num6 <= 0)
1956: 			{
1957: 				GUI.Label(new Rect(num2, num5, ((Rect)(ref r)).xMax - num2, 34f), L.T("暂不可用", "Unavailable"), theme.Small);
1958: 				Cart.Remove(catalogEntry);
1959: 				num5 += 36f;
~~~

~~~csharp
1980: 		bool enabled = GUI.enabled;
1981: 		GUI.enabled = num7 > 0 && num7 <= Cash && flag;
1982: 		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num8 + 2f, 160f, 36f), L.T("提交申领", "Submit requisition"), theme.BtnHotStyle))
1983: 		{
1984: 			string text = Services.PlaceOrder(_company, Cart, MeridianUnitPrice);
~~~

~~~csharp
2037: 		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
2038: 		Theme theme = _theme;
2039: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 66f), L.T("精锻熔炉每晚把高纯度的金属锭炼成钢胚；锻台用钢胚锻刀，淬火的水越干净，刀的成色越好。", "The precision forge turns high-purity ingots into billets overnight; the anvil forges blades from billets, and the cleaner the quench water, the better the finish."), theme.Wrap);
2040: 		float num = ((Rect)(ref r)).y + 76f;
2041: 		if (!MeridianCo.ForgeOpen && !MeridianCo.ForgeOffered)
~~~

~~~csharp
2058: 			return;
2059: 		}
2060: 		GUI.Label(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 20f), L.T("找罗森订。原价，明天到，不限量。", "Order through Rosen. List price, here tomorrow, no limit."), theme.Small);
2061: 		num += 28f;
2062: 		(string, string)[] forgeMachines = MeridianCo.ForgeMachines;
~~~

~~~csharp
2079: 			bool enabled2 = GUI.enabled;
2080: 			GUI.enabled = Cash >= num3;
2081: 			if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num + 30f, 160f, 36f), L.T("订一台", "Order one"), theme.BtnHotStyle))
2082: 			{
2083: 				Toast(MeridianCo.OrderForgeMachine(_company, item, num3) ?? _company.FrontDeskDone);
~~~

~~~csharp
2119: 		float num = ((Rect)(ref r)).y + 54f;
2120: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 26f), theme.Ink);
2121: 		GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 260f, 26f), L.T("枪", "Gun"), theme.InvBody);
2122: 		GUI.Label(new Rect(((Rect)(ref r)).x + 270f, num, 160f, 26f), L.T("成色", "Condition"), theme.InvCenter);
2123: 		GUI.Label(new Rect(((Rect)(ref r)).x + 430f, num, ((Rect)(ref r)).width - 430f, 26f), L.T("费用", "Fee"), theme.InvCenter);
~~~

~~~csharp
2120: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 26f), theme.Ink);
2121: 		GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 260f, 26f), L.T("枪", "Gun"), theme.InvBody);
2122: 		GUI.Label(new Rect(((Rect)(ref r)).x + 270f, num, 160f, 26f), L.T("成色", "Condition"), theme.InvCenter);
2123: 		GUI.Label(new Rect(((Rect)(ref r)).x + 430f, num, ((Rect)(ref r)).width - 430f, 26f), L.T("费用", "Fee"), theme.InvCenter);
2124: 		num += 30f;
~~~

~~~csharp
2121: 		GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 260f, 26f), L.T("枪", "Gun"), theme.InvBody);
2122: 		GUI.Label(new Rect(((Rect)(ref r)).x + 270f, num, 160f, 26f), L.T("成色", "Condition"), theme.InvCenter);
2123: 		GUI.Label(new Rect(((Rect)(ref r)).x + 430f, num, ((Rect)(ref r)).width - 430f, 26f), L.T("费用", "Fee"), theme.InvCenter);
2124: 		num += 30f;
2125: 		if (RefurbRows.Count == 0)
~~~

~~~csharp
2125: 		if (RefurbRows.Count == 0)
2126: 		{
2127: 			GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, ((Rect)(ref r)).width, 30f), L.T("没有能翻新的枪。", "No guns to refurbish."), theme.Small);
2128: 			num += 34f;
2129: 		}
~~~

~~~csharp
2148: 		bool enabled = GUI.enabled;
2149: 		GUI.enabled = flag && RefurbRows.Count > 0 && Cash >= num3;
2150: 		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num4 + 2f, 160f, 36f), L.T("送厂翻新", "Send for refurbishment"), theme.BtnHotStyle))
2151: 		{
2152: 			string text = Services.RequestRefurbish(_company);
~~~

~~~csharp
2154: 			if (text == null)
2155: 			{
2156: 				text = L.T("收到。明天送回。", "Received. Back tomorrow.");
2157: 			}
2158: 			Toast(text);
~~~

~~~csharp
2184: 		Banner(r);
2185: 		GUI.Label(new Rect(((Rect)(ref r)).x + 140f, ((Rect)(ref r)).y + 20f, 324f, 50f), _company.Name, theme.Title);
2186: 		GUI.Label(new Rect(24f, 106f, width - 80f, 22f), L.T("前台：", "Front desk: ") + _desk, theme.Body);
2187: 		Rect val = default(Rect);
2188: 		((Rect)(ref val))._002Ector(10f, 136f, 168f, 470f);
~~~

~~~csharp
2188: 		((Rect)(ref val))._002Ector(10f, 136f, 168f, 470f);
2189: 		GUI.Box(val, GUIContent.none, theme.CellBox);
2190: 		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 6f, 150f, 22f), L.T("服务", "Services"), theme.Small);
2191: 		for (int i = 0; i < PastelModules.Length; i++)
2192: 		{
~~~

~~~csharp
2194: 			bool flag = PastelUnlocked(i, out why);
2195: 			Rect r2 = new Rect(((Rect)(ref val)).x + 6f, ((Rect)(ref val)).y + 30f + (float)(i * 32), ((Rect)(ref val)).width - 12f, 28f);
2196: 			string text = ((i == _module) ? "› " : "  ") + PastelModules[i] + (flag ? "" : L.T("（未解锁）", " (locked)"));
2197: 			if (Ui.Button(r2, text, (i == _module) ? theme.NavOnStyle : theme.NavStyle))
2198: 			{
~~~

~~~csharp
2233: 		if (module == 0 && num < 1)
2234: 		{
2235: 			why = L.T("签约后可以电话订货。和米洛多做几笔买卖吧。", "Sign with pastel to order by phone. Do a few more deals with Milo.");
2236: 		}
2237: 		if (module == 1 && num < 1)
~~~

~~~csharp
2237: 		if (module == 1 && num < 1)
2238: 		{
2239: 			why = L.T("签约后可以加盟成为帕斯泰尔的下层合作商。", "Sign with pastel to join as a Lower Levels dealer.");
2240: 		}
2241: 		if (module == 2 && !state.DealerJoined)
~~~

~~~csharp
2241: 		if (module == 2 && !state.DealerJoined)
2242: 		{
2243: 			why = L.T("先成为下层合作商，帕斯泰尔才会把熟客介绍给你。", "Become a Lower Levels dealer first, and pastel will send its regulars your way.");
2244: 		}
2245: 		return why == null;
~~~

~~~csharp
2292: 		int num = _company.CatalogTier(state);
2293: 		bool flag = PastelCo.HemingwayOpen();
2294: 		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, 90f, 26f), L.T("日用", "Daily"), (_page == "日用") ? theme.BtnHotStyle : theme.BtnStyle))
2295: 		{
2296: 			_page = "日用";
~~~

~~~csharp
2296: 			_page = "日用";
2297: 		}
2298: 		if (Ui.Button(new Rect(((Rect)(ref r)).x + 96f, ((Rect)(ref r)).y, 140f, 26f), flag ? L.T("海明威", "Hemingway") : L.T("海明威（未解锁）", "Hemingway (locked)"), (_page == "海明威") ? theme.BtnHotStyle : theme.BtnStyle))
2299: 		{
2300: 			if (flag)
~~~

~~~csharp
2304: 			else
2305: 			{
2306: 				Toast(L.T("米洛调去海明威之后才有这一页", "This page opens once Milo transfers to Hemingway"));
2307: 			}
2308: 		}
~~~

~~~csharp
2308: 		}
2309: 		bool flag2 = PastelCo.CraftDone(1);
2310: 		if (Ui.Button(new Rect(((Rect)(ref r)).x + 242f, ((Rect)(ref r)).y, 90f, 26f), flag2 ? L.T("工艺", "Craft") : "？？？", (_page == "工艺") ? theme.BtnHotStyle : theme.BtnStyle))
2311: 		{
2312: 			if (flag)
~~~

~~~csharp
2316: 			else
2317: 			{
2318: 				Toast(L.T("米洛调去海明威之后才有这一页", "This page opens once Milo transfers to Hemingway"));
2319: 			}
2320: 		}
~~~

~~~csharp
2326: 		}
2327: 		bool flag3 = Services.CanOrderToday(_company);
2328: 		GUI.Label(new Rect(((Rect)(ref r)).x + 340f, ((Rect)(ref r)).y, ((Rect)(ref r)).width - 340f, 26f), flag3 ? L.T("今天下单，明天送到", "Order today, delivered tomorrow") : L.T("今天已经下过单了", "Already ordered today"), theme.Small);
2329: 		CatalogEntry[] array = PastelCo.Entries.Where((CatalogEntry e) => e.Page == _page).ToArray();
2330: 		CatalogEntry catalogEntry = PastelSpecial();
~~~

~~~csharp
2362: 			if (num6 <= 0)
2363: 			{
2364: 				GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + 44f, ((Rect)(ref val)).width, 22f), L.T("暂不可用", "Unavailable"), theme.Center);
2365: 				Cart.Remove(catalogEntry2);
2366: 				continue;
~~~

~~~csharp
2366: 				continue;
2367: 			}
2368: 			GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + 36f, ((Rect)(ref val)).width, 22f), ((catalogEntry2 == catalogEntry) ? L.T("特价 ", "Deal ") : "") + $"{num6}₵", theme.Price);
2369: 			int num7 = Math.Min(10, Services.Left(state, catalogEntry2));
2370: 			if (catalogEntry2.Limit() > 0)
~~~

~~~csharp
2390: 		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
2391: 		Theme theme = _theme;
2392: 		GUI.Label(new Rect(((Rect)(ref cell)).x + 6f, ((Rect)(ref cell)).y + 34f, ((Rect)(ref cell)).width - 12f, 22f), L.T("打点主管", "Pay off the supervisor"), theme.Center);
2393: 		bool enabled = GUI.enabled;
2394: 		GUI.enabled = Cash >= PastelCo.Bribe;
~~~

~~~csharp
2395: 		if (Ui.Button(new Rect(((Rect)(ref cell)).x + 8f, ((Rect)(ref cell)).yMax - 30f, ((Rect)(ref cell)).width - 16f, 26f), L.En ? $"Pay {PastelCo.Bribe}₵" : $"补交 {PastelCo.Bribe}₵", theme.BtnHotStyle))
2396: 		{
2397: 			Toast(PastelCo.PayBribe(deliver: true) ? L.T("分装机明天送到。", "The packaging machine arrives tomorrow.") : L.T("现金不够", "Not enough cash"));
2398: 		}
2399: 		GUI.enabled = enabled;
~~~

~~~csharp
2409: 		Theme theme = _theme;
2410: 		CompanyState state = State;
2411: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 20f, 30f), L.T("下层合作商", "Lower Dealer"), theme.Title);
2412: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 40f, ((Rect)(ref r)).width - 20f, 120f), L.T("成为帕斯泰尔的特约经销点。\n\n", "Become a pastel partner outlet.\n\n") + (L.En ? $"· Regular rebate: every {PastelCo.RebatePeriod} days, {0.1:P0} of what you sold in pastel products, paid back by the courier.\n" : $"· 定期返点：每 {PastelCo.RebatePeriod} 天，把这段时间你卖出的帕斯泰尔产品按售价的 {0.1:P0} 返现，由送货员带来。\n") + L.T("· 解锁“下层客流”：帕斯泰尔会把下层区的熟客介绍到你店里。", "· Unlocks “Lower Foot Traffic”: pastel sends Lower Levels regulars to your shop."), theme.Wrap);
2413: 		if (!state.DealerJoined)
~~~

~~~csharp
2410: 		CompanyState state = State;
2411: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 20f, 30f), L.T("下层合作商", "Lower Dealer"), theme.Title);
2412: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 40f, ((Rect)(ref r)).width - 20f, 120f), L.T("成为帕斯泰尔的特约经销点。\n\n", "Become a pastel partner outlet.\n\n") + (L.En ? $"· Regular rebate: every {PastelCo.RebatePeriod} days, {0.1:P0} of what you sold in pastel products, paid back by the courier.\n" : $"· 定期返点：每 {PastelCo.RebatePeriod} 天，把这段时间你卖出的帕斯泰尔产品按售价的 {0.1:P0} 返现，由送货员带来。\n") + L.T("· 解锁“下层客流”：帕斯泰尔会把下层区的熟客介绍到你店里。", "· Unlocks “Lower Foot Traffic”: pastel sends Lower Levels regulars to your shop."), theme.Wrap);
2413: 		if (!state.DealerJoined)
2414: 		{
~~~

~~~csharp
2415: 			if (Ui.Button(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 180f, 220f, 34f), L.En ? $"Join (one-time {PastelCo.DealerFee}₵)" : $"加盟（一次性 {PastelCo.DealerFee}₵）", theme.BtnHotStyle))
2416: 			{
2417: 				Toast(Services.JoinDealer(_company) ?? L.T("欢迎加入帕斯泰尔！", "Welcome to pastel!"));
2418: 			}
2419: 		}
~~~

~~~csharp
2434: 		Theme theme = _theme;
2435: 		CompanyState state = State;
2436: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 20f, 30f), L.T("下层客流", "Lower Foot Traffic"), theme.Title);
2437: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 40f, ((Rect)(ref r)).width - 20f, 100f), (L.En ? $"{PastelCo.TrafficFee}₵ per night. pastel sends Lower Levels regulars over: the next day, 1–2 extra Lower Levels customers come looking for food and daily goods.\n\nPauses automatically when you are short on cash." : $"每晚 {PastelCo.TrafficFee}₵。帕斯泰尔把下层区的熟客介绍过来：第二天会额外来 1–2 位想买食物、日用品的下层区顾客。\n\n现金不够时自动暂停。") + ((Cash >= PastelCo.TrafficFee) ? "" : (L.En ? $"\n(You have {Cash}₵, not enough for tonight's fee.)" : $"\n（你现在的现金 {Cash}₵，不够今晚的费用。）")), theme.Wrap);
2438: 		if (Ui.Button(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 160f, 180f, 34f), state.TrafficOn ? L.T("已开启（点击关闭）", "On (click to turn off)") : L.T("开启", "Turn on"), state.TrafficOn ? theme.BtnStyle : theme.BtnHotStyle))
~~~

~~~csharp
2436: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 20f, 30f), L.T("下层客流", "Lower Foot Traffic"), theme.Title);
2437: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 40f, ((Rect)(ref r)).width - 20f, 100f), (L.En ? $"{PastelCo.TrafficFee}₵ per night. pastel sends Lower Levels regulars over: the next day, 1–2 extra Lower Levels customers come looking for food and daily goods.\n\nPauses automatically when you are short on cash." : $"每晚 {PastelCo.TrafficFee}₵。帕斯泰尔把下层区的熟客介绍过来：第二天会额外来 1–2 位想买食物、日用品的下层区顾客。\n\n现金不够时自动暂停。") + ((Cash >= PastelCo.TrafficFee) ? "" : (L.En ? $"\n(You have {Cash}₵, not enough for tonight's fee.)" : $"\n（你现在的现金 {Cash}₵，不够今晚的费用。）")), theme.Wrap);
2438: 		if (Ui.Button(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 160f, 180f, 34f), state.TrafficOn ? L.T("已开启（点击关闭）", "On (click to turn off)") : L.T("开启", "Turn on"), state.TrafficOn ? theme.BtnStyle : theme.BtnHotStyle))
2439: 		{
2440: 			state.TrafficOn = !state.TrafficOn;
~~~

~~~csharp
2439: 		{
2440: 			state.TrafficOn = !state.TrafficOn;
2441: 			ModLog.Info("下层客流：" + (state.TrafficOn ? L.T("开启", "Turn on") : "关闭"));
2442: 			Toast(state.TrafficOn ? L.T("从今晚开始介绍熟客", "Regulars start coming from tonight") : L.T("已关闭", "Turned off"));
2443: 		}
~~~

~~~csharp
2440: 			state.TrafficOn = !state.TrafficOn;
2441: 			ModLog.Info("下层客流：" + (state.TrafficOn ? L.T("开启", "Turn on") : "关闭"));
2442: 			Toast(state.TrafficOn ? L.T("从今晚开始介绍熟客", "Regulars start coming from tonight") : L.T("已关闭", "Turned off"));
2443: 		}
2444: 	}
~~~

~~~csharp
2469: 			Ui.Image(r2, val);
2470: 		}
2471: 		GUI.Label(new Rect(((Rect)(ref r)).x + 6f, ((Rect)(ref r)).y + 104f, 130f, 22f), L.T("米洛", "Milo"), theme.Center);
2472: 		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 10f, 300f, 22f), "“" + MiloLines[Day % MiloLines.Length] + "”", theme.Body);
2473: 		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 40f, 300f, 22f), L.En ? $"Cooperation {state.Coop}  ·  {TierLabel(state)}" : $"合作度 {state.Coop}  ·  {TierLabel(state)}", theme.Body);
~~~

~~~csharp
2484: 		bool enabled2 = GUI.enabled;
2485: 		GUI.enabled = enabled;
2486: 		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, ((Rect)(ref r)).y + 64f, 150f, 36f), L.T("下单", "Order"), theme.BtnHotStyle))
2487: 		{
2488: 			string text = Services.PlaceOrder(_company, Cart, PastelUnitPrice);
~~~

~~~csharp
2490: 			{
2491: 				Cart.Clear();
2492: 				Toast(L.T("已下单！", "Ordered! ") + _company.FrontDeskDone);
2493: 			}
2494: 			else
~~~

~~~csharp
2566: 			Ui.Image(new Rect(((Rect)(ref val2)).x + 4f, ((Rect)(ref val2)).y + 4f, ((Rect)(ref val2)).width - 8f, ((Rect)(ref val2)).height - 8f), val3);
2567: 		}
2568: 		GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val2)).yMax + 4f, ((Rect)(ref val)).width, 20f), L.T("专属顾问 · 瑟琳", "Personal Advisor · Seline"), desk);
2569: 		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val2)).yMax + 30f, ((Rect)(ref val)).width - 20f, 22f), L.En ? ("Membership: " + TierLabel(state)) : ("会员：" + TierLabel(state)), theme.Body);
2570: 		CoopBar(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val2)).yMax + 56f, ((Rect)(ref val)).width - 20f, 8f));
~~~

~~~csharp
2624: 		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 120f, 240f, 34f), (!flag) ? (L.En ? $"Available again on day {num}" : $"第 {num} 天可再预约") : (L.En ? $"Book a VIP ({SolanaCo.VipFee}₵)" : $"预约贵宾（{SolanaCo.VipFee}₵）"), flag ? theme.BtnHotStyle : theme.BtnStyle) && flag)
2625: 		{
2626: 			Toast(Services.BookVip(_company) ?? L.T("已为您安排，明天专人引荐。", "Arranged. Your VIP arrives tomorrow."));
2627: 		}
2628: 	}
~~~

~~~csharp
2652: 		if (state.LabBooked)
2653: 		{
2654: 			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 200f, ((Rect)(ref r)).width, 24f), L.T("这一期已预约。", "Booked for this list."), theme.Body);
2655: 		}
2656: 		else if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 200f, 200f, 34f), L.T("预约采购员", "Book a buyer"), theme.BtnHotStyle))
~~~

~~~csharp
2654: 			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 200f, ((Rect)(ref r)).width, 24f), L.T("这一期已预约。", "Booked for this list."), theme.Body);
2655: 		}
2656: 		else if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 200f, 200f, 34f), L.T("预约采购员", "Book a buyer"), theme.BtnHotStyle))
2657: 		{
2658: 			Toast(Services.BookLab(_company) ?? _company.FrontDeskDone);
~~~

~~~csharp
2670: 		bool enabled = GUI.enabled;
2671: 		GUI.enabled = flag;
2672: 		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 80f, 200f, 34f), flag ? L.T("领取礼遇", "Claim gift") : L.T("这一期已领取", "Claimed for this period"), theme.BtnHotStyle))
2673: 		{
2674: 			Toast(Services.ClaimGift(_company) ?? _company.FrontDeskDone);
~~~

~~~csharp
2726: 		GUI.Label(new Rect(600f, 40f, width - 640f, 22f), L.En ? $"{_company.PhoneNumber}   Day {Day}" : $"{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
2727: 		Ui.Fill(new Rect(20f, 100f, width - 40f, 2f), theme.Ink);
2728: 		GUI.Label(new Rect(24f, 106f, width - 48f, 22f), L.T("导师：", "The Mentor: ") + _desk, theme.Body);
2729: 		Rect val = default(Rect);
2730: 		((Rect)(ref val))._002Ector(20f, 136f, 190f, height - 156f);
~~~

~~~csharp
2754: 			if (_company.Tier(state) < 1)
2755: 			{
2756: 				GUI.Label(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y, ((Rect)(ref val3)).width, 40f), L.T("没人认识你。", "Nobody knows you."), theme.Wrap);
2757: 			}
2758: 			else if (SpacerPages[_module] == "阵线" && _company.Tier(state) < 2)
~~~

~~~csharp
2809: 		string text = page switch
2810: 		{
2811: 			"阵线" => L.T("几家一起出的。", "Made by several outfits together."), 
2812: 			"药房" => L.T("没有许可证。", "No license."), 
2813: 			"军械" => L.T("成色不保证。", "Condition not guaranteed."), 
~~~

~~~csharp
2810: 		{
2811: 			"阵线" => L.T("几家一起出的。", "Made by several outfits together."), 
2812: 			"药房" => L.T("没有许可证。", "No license."), 
2813: 			"军械" => L.T("成色不保证。", "Condition not guaranteed."), 
2814: 			_ => L.T("货船余货，量大。", "Freighter surplus. Plenty of it."), 
~~~

~~~csharp
2811: 			"阵线" => L.T("几家一起出的。", "Made by several outfits together."), 
2812: 			"药房" => L.T("没有许可证。", "No license."), 
2813: 			"军械" => L.T("成色不保证。", "Condition not guaranteed."), 
2814: 			_ => L.T("货船余货，量大。", "Freighter surplus. Plenty of it."), 
2815: 		};
~~~

~~~csharp
2812: 			"药房" => L.T("没有许可证。", "No license."), 
2813: 			"军械" => L.T("成色不保证。", "Condition not guaranteed."), 
2814: 			_ => L.T("货船余货，量大。", "Freighter surplus. Plenty of it."), 
2815: 		};
2816: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 20f), (flag ? L.T("今天下单，明天到。", "Order today, here tomorrow. ") : L.T("今天已经下过单了。", "Already ordered today. ")) + text, theme.Small);
~~~

~~~csharp
2814: 			_ => L.T("货船余货，量大。", "Freighter surplus. Plenty of it."), 
2815: 		};
2816: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 20f), (flag ? L.T("今天下单，明天到。", "Order today, here tomorrow. ") : L.T("今天已经下过单了。", "Already ordered today. ")) + text, theme.Small);
2817: 		float x = ((Rect)(ref r)).x;
2818: 		float num2 = ((Rect)(ref r)).x + 250f;
~~~

~~~csharp
2821: 		float num5 = ((Rect)(ref r)).y + 26f;
2822: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 24f), theme.Accent2);
2823: 		GUI.Label(new Rect(x + 8f, num5, 240f, 24f), L.T("品名", "Item"), theme.InvBody);
2824: 		GUI.Label(new Rect(num2, num5, 90f, 24f), L.T("单价", "Unit price"), theme.InvCenter);
2825: 		GUI.Label(new Rect(num3, num5, 120f, 24f), L.T("数量", "Qty"), theme.InvCenter);
~~~

~~~csharp
2822: 		Ui.Fill(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 24f), theme.Accent2);
2823: 		GUI.Label(new Rect(x + 8f, num5, 240f, 24f), L.T("品名", "Item"), theme.InvBody);
2824: 		GUI.Label(new Rect(num2, num5, 90f, 24f), L.T("单价", "Unit price"), theme.InvCenter);
2825: 		GUI.Label(new Rect(num3, num5, 120f, 24f), L.T("数量", "Qty"), theme.InvCenter);
2826: 		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 24f), L.T("小计", "Subtotal"), theme.InvCenter);
~~~

~~~csharp
2823: 		GUI.Label(new Rect(x + 8f, num5, 240f, 24f), L.T("品名", "Item"), theme.InvBody);
2824: 		GUI.Label(new Rect(num2, num5, 90f, 24f), L.T("单价", "Unit price"), theme.InvCenter);
2825: 		GUI.Label(new Rect(num3, num5, 120f, 24f), L.T("数量", "Qty"), theme.InvCenter);
2826: 		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 24f), L.T("小计", "Subtotal"), theme.InvCenter);
2827: 		num5 += 28f;
~~~

~~~csharp
2824: 		GUI.Label(new Rect(num2, num5, 90f, 24f), L.T("单价", "Unit price"), theme.InvCenter);
2825: 		GUI.Label(new Rect(num3, num5, 120f, 24f), L.T("数量", "Qty"), theme.InvCenter);
2826: 		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 24f), L.T("小计", "Subtotal"), theme.InvCenter);
2827: 		num5 += 28f;
2828: 		IEnumerable<CatalogEntry> source = ((_company is RebelCo) ? RebelCo.Entries : SpacerCo.PageEntries(page));
~~~

~~~csharp
2869: 			if (num11 <= 0)
2870: 			{
2871: 				GUI.Label(new Rect(num2, num5, ((Rect)(ref r)).xMax - num2, 32f), L.T("暂不可用", "Unavailable"), theme.Small);
2872: 				Cart.Remove(item);
2873: 				num5 += 33f;
~~~

~~~csharp
2899: 		bool enabled2 = GUI.enabled;
2900: 		GUI.enabled = enabled;
2901: 		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 150f, num13 + 2f, 140f, 36f), L.T("下单", "Order"), theme.BtnHotStyle))
2902: 		{
2903: 			string text2 = Services.PlaceOrder(_company, Cart, SpacerUnitPrice);
~~~

~~~csharp
2923: 		Theme theme = _theme;
2924: 		GUI.Box(r, GUIContent.none, theme.CellBox);
2925: 		GUI.Label(new Rect(((Rect)(ref r)).x + 12f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 24f, 22f), L.T("技术来源", "Tech credits"), theme.Small);
2926: 		float num = ((Rect)(ref r)).y + 28f;
2927: 		(string, string)[] techCredits = SpacerCo.TechCredits;
~~~

~~~csharp
2948: 		}
2949: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 90f), L.En ? $"Spacers at the docks are short on food, water and medicine. With this on, the Alliance points people your way every night: the next day, 1–2 extra Spacers come by to buy whatever they need.\n\n{SpacerCo.MutualFee}₵ per night. Stops automatically when you are short on cash." : $"码头上的星际客缺吃、缺水、缺药。开着这个，联盟每晚把人往你这儿指：第二天多来 1–2 位星际客，缺什么买什么。\n\n每晚 {SpacerCo.MutualFee}₵。现金不够时自动停。", theme.Wrap);
2950: 		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 110f, 200f, 34f), state.TrafficOn ? L.T("开着（点击关掉）", "On (click to turn off)") : L.T("开", "Turn on"), state.TrafficOn ? theme.BtnStyle : theme.BtnHotStyle))
2951: 		{
2952: 			state.TrafficOn = !state.TrafficOn;
~~~

~~~csharp
2952: 			state.TrafficOn = !state.TrafficOn;
2953: 			ModLog.Info("街坊互助：" + (state.TrafficOn ? "开启" : "关闭"));
2954: 			Toast(state.TrafficOn ? L.T("今晚开始。", "Starting tonight.") : L.T("停了。", "Stopped."));
2955: 		}
2956: 	}
~~~
