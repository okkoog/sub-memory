namespace MidnightPawn.Game;

/// <summary>收购商相关的全部可见文案；MaidenBuyer 只负责调用。</summary>
public static class MaidenBuyerText
{
	public const string ToastTitle = "苗床收购商";
	public const string SettlementToastTitle = "苗床收购";
	public static string[] FirstPurchaseFailedLines() => new[]
	{
		"这身子没达到我开的条件，玩起来不够爽。",
		"这一笔按验过的成色结。今天到此为止，下次把货调教好再带来。"
	};

	public static string[] FirstPurchasePassedLines(int extra) => new[]
	{
		"这货对味，脱光了验也挑不出毛病，第一笔就按验过的成色结。",
		$"尝过一口就上瘾了，我还想再收 {extra} 个回去慢慢玩。",
		"后面的不用再对条件，每个都按基础价算。洗干净，一个一个送到我床上。"
	};

	public static string[] ExtraPurchaseLines(int remaining) => remaining > 0 ? new[]
	{
		"这一个也验过了，按基础价结，奶子和穴都还合用。",
		$"还能再玩 {remaining} 个，继续脱，别让我等。"
	} : new[]
	{
		"最后一个也结清了，今天的份额被你掏空了。",
		"合作愉快，我回去慢慢享用，先走了。"
	};

	public static string[] PurchaseErrorLines() => new[]
	{
		"账目出了岔子，今天先不玩了，省得扫兴。",
		"把款项和回执核对清楚，下次再把货带来。"
	};

	public static string ExtraSettlementToast(long payout) => $"追加收购完成：按基础估值获得 {payout} 信用点（不核对偏好，不加倍率，纯当买个泄火的）。";

	public static string ExtraReceipt(string buyer, string summary, long baseValue, long payout)
	{
		return "收购商：" + buyer + "\n收购对象：" + summary
			+ "\n\n追加收购：不再核对偏好，按基础估值结算，买来只供泄火。\n"
			+ ReceiptPayout(baseValue, 1.0, payout);
	}
	public const string BuyerSuffix = "收购商";
	public const string ReceiptTitle = "苗床收购回执";
	public const string ReceiptShortDescription = "一张手写的收购回执。写着买家、验货评语与最终成交价。";
	public const string ReceiptFlavorText = "纸角沾着可疑的湿痕，闻起来又腥又甜。";
	public const string LabelFaction = "阵营";
	public const string LabelJob = "职业";
	public const string LabelBody = "体型";
	public const string LabelBreast = "乳量";
	public const string LabelVirgin = "处女";
	public const string Unknown = "未知";

	public static string FactionName(string faction)
	{
		return faction switch
		{
			"FACTION_BLACK_MARKET" => "黑市",
			"FACTION_UPPER_LEVEL" => "上层区",
			"FACTION_SECURITY" => "治安部",
			"FACTION_REVOLUTION" => "革命军",
			_ => "下层区",
		};
	}

	public static string BuyerDisplayName(string faction)
	{
		return FactionName(faction) + BuyerSuffix;
	}

		/// <summary>从一组候选台词里随机抽一句，保持单句简短。</summary>
	private static string PickLine(string[] lines)
	{
		if (lines == null || lines.Length == 0)
		{
			return "";
		}
		return lines[RNG.GetRandomIntExculsive(0, lines.Length)];
	}

	public static string IntroLine(string buyerFaction)
	{
		string[] lines = buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[3]
			{
				"今晚黑市开张，我要个能接客的苗床，先让我上手验验。",
				"货带来了？脱了让我看看，奶子和穴都得能卖钱。",
				"黑市不养废货，我要个张开腿就能接客的。"
			},
			"FACTION_UPPER_LEVEL" => new string[3]
			{
				"我今晚要一个人暖床。身子得干净，叫起来得好听。",
				"我的床上不养木头，先让我看看她怎么伺候人。",
				"别拿廉价货糊弄我，我要能折腾一整夜的。"
			},
			"FACTION_SECURITY" => new string[3]
			{
				"这单不上账。我要她嘴紧、身子干净，今晚就要用。",
				"上头有人等不及了，货得能扛，别一碰就哭。",
				"别问用途。钱我照付，人我带走过夜。"
			},
			"FACTION_REVOLUTION" => new string[3]
			{
				"前线的兄弟憋太久了，挑个耐操的，今晚就送走。",
				"我要能让兄弟们轮着用还不散架的身子。",
				"仗打完了总得泄火。货好，钱不少你的。"
			},
			_ => new string[3]
			{
				"我攒了点钱，想买个能过夜的苗床，别拿次货糊弄我。",
				"不用多漂亮，脱了能让我舒坦一晚上就行。",
				"一个人睡太冷了，买个会叫的回去暖被窝。"
			},
		};
		return PickLine(lines);
	}

	public static string DealLine(string buyerFaction)
	{
		string[] lines = buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2]
			{
				"成交。回执拿好，别让治安部看见。今晚她别想睡。",
				"钱货两清。明早她还能不能自己走，看造化。"
			},
			"FACTION_UPPER_LEVEL" => new string[2]
			{
				"成交。回执放柜台。今晚我会好好用她。",
				"钱付了，人我带走。明早派人送回来。"
			},
			"FACTION_SECURITY" => new string[2]
			{
				"货我带走，账不会留。回执给你，就当没这回事。",
				"钱拿好，嘴闭紧。回执是给你封口的。"
			},
			"FACTION_REVOLUTION" => new string[2]
			{
				"这批货送前线。回执收好，兄弟们会记得你的。",
				"钱点清楚。她今晚就得开始劳军。"
			},
			_ => new string[2]
			{
				"成交。回执给你，下次有好货还找我。",
				"钱在这儿。她跟我回去，今晚别想闲着。"
			},
		};
		return PickLine(lines);
	}

	public static string YoungIntroLine(string buyerFaction)
	{
		string[] lines = buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2]
			{
				"黑市缺个跑腿的，我看看这身子骨结不结实。",
				"别多问，我是来挑个能干活的。"
			},
			"FACTION_UPPER_LEVEL" => new string[2]
			{
				"我听说你这有年轻苗床，先让我验验货。",
				"家里缺个伺候起居的，带回去学规矩。"
			},
			"FACTION_SECURITY" => new string[2]
			{
				"这单不记在册子上。人我带回去问话。",
				"货对就行，别问带去做什么。"
			},
			"FACTION_REVOLUTION" => new string[2]
			{
				"前线缺人手，给我挑个能撑住的。",
				"队伍里缺个跑腿的，能吃苦就成。"
			},
			_ => new string[2]
			{
				"我想买个年轻点的，能干活、听话就行。",
				"家里缺个帮手，别多问。"
			},
		};
		return PickLine(lines);
	}

	public static string YoungDealLine(string buyerFaction)
	{
		string[] lines = buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2] { "人我带走了，回执拿好。", "成交。别多问，也别乱说。" },
			"FACTION_UPPER_LEVEL" => new string[2] { "钱货两清，回执在柜台上。", "人我带走，明早送回来。" },
			"FACTION_SECURITY" => new string[2] { "货我带走，账不会留。", "回执给你，这事到此为止。" },
			"FACTION_REVOLUTION" => new string[2] { "这批货送前线，回执收好。", "人我带走，兄弟们等着用。" },
			_ => new string[2] { "成交，回执在这儿。", "人我领走了，下次还找你。" },
		};
		return PickLine(lines);
	}

	private static string[] ArrivalLines(string buyerFaction)
	{
		return buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2] { "今晚要找个能接客的。", "好货摆上来，我赶时间。" },
			"FACTION_UPPER_LEVEL" => new string[2] { "我今晚就要用，别让我等。", "把最好的货拿出来。" },
			"FACTION_SECURITY" => new string[2] { "这单不记账，快点。", "人我看看，别多问。" },
			"FACTION_REVOLUTION" => new string[2] { "前线缺人，挑个耐用的。", "兄弟们等着用，动作快。" },
			_ => new string[2] { "我想买个能过夜的。", "把货拿出来我瞧瞧。" },
		};
	}

	/// <summary>要货的开场白，按买家阵营换口气。</summary>
	private static string[] DemandHeaderLines(string buyerFaction)
	{
		return buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2] { "今晚要开张，货得这样：", "黑市不养闲人，我要的货：" },
			"FACTION_UPPER_LEVEL" => new string[2] { "我的床不好上，先听清楚：", "今晚要尽兴，货得符合这些：" },
			"FACTION_SECURITY" => new string[2] { "这单要交差，货得对得上：", "上头催得紧，按这个配：" },
			"FACTION_REVOLUTION" => new string[2] { "兄弟们等着用，条件如下：", "前线的货，得这样：" },
			_ => new string[2] { "我钱不多，就这点要求：", "买个过夜的，得这样：" },
		};
	}

	/// <summary>要货的收尾话，按买家阵营换口气。</summary>
	private static string[] DemandTailLines(string buyerFaction)
	{
		return buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2] { "脱光了让我验，别磨蹭。", "把人洗干净，我要当场试。" },
			"FACTION_UPPER_LEVEL" => new string[2] { "我今晚就要用，别让我等太久。", "验货时手别抖，我看得很细。" },
			"FACTION_SECURITY" => new string[2] { "验货快点，我不想留记录。", "按单子来，出了岔子你担着。" },
			"FACTION_REVOLUTION" => new string[2] { "货得撑得住，兄弟们不会客气。", "我先试，撑不住就退货。" },
			_ => new string[2] { "让我摸摸看，值不值这个价。", "别拿调教过头的糊弄我。" },
		};
	}

	/// <summary>要求本身不带句号，拆成单独一句时补上。</summary>
	public static string AsSentence(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		char last = text[text.Length - 1];
		if (last == '。' || last == '！' || last == '？' || last == '…')
		{
			return text;
		}
		return text + "。";
	}

	/// <summary>年轻组买家只报需求，不写露骨句子；年轻判定统一走 MaidenAgeFilter。</summary>
	private static string DescribeNeutral(string[] array)
	{
		if (array == null || array.Length < 2)
		{
			return "";
		}
		return array[0] switch
		{
			"F" => "出身：" + array[1],
			"J" => "职业：" + array[1],
			"B" => "体型：" + array[1],
			"R" => "乳量：" + array[1],
			"V" => "处女要求：" + array[1],
			"A" => DescribeActPrefNeutral(array),
			_ => "",
		};
	}

	private static string DescribeActPrefNeutral(string[] parts)
	{
		if (parts.Length < 4)
		{
			return "";
		}
		string text = parts[1] switch
		{
			"ORAL" => "口交次数",
			"SEX" => "性交次数",
			_ => "肛交次数",
		};
		return text + " " + parts[2] + " " + parts[3];
	}

	private static string DescribeLewd(string[] array, string buyerFaction)
	{
		if (array == null || array.Length < 2)
		{
			return "";
		}
		return array[0] switch
		{
			"F" => DescribeFactionPref(array[1], buyerFaction),
			"J" => DescribeJobPref(array[1], buyerFaction),
			"B" => DescribeBodyPref(array[1], buyerFaction),
			"R" => DescribeBreastPref(array[1], buyerFaction),
			"V" => DescribeVirginPref(array[1], buyerFaction),
			"A" => DescribeActPref(array, buyerFaction),
			_ => "",
		};
	}

	private static string DescribeActPref(string[] parts, string buyerFaction)
	{
		if (parts.Length < 4)
		{
			return "";
		}
		string act = LewdActName(parts[1]);
		bool less = parts[2] == "<";
		string line = less ? (act + "被用过的次数要少于" + parts[3] + "次") : (act + "得被操过至少" + parts[3] + "次");
		return buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => less ? (line + "，越生越好卖") : (line + "，会伺候人才值钱"),
			"FACTION_UPPER_LEVEL" => less ? (line + "，别太杂") : (line + "，别像块木头"),
			"FACTION_SECURITY" => less ? (line + "，干净点") : (line + "，见过场面"),
			"FACTION_REVOLUTION" => less ? (line + "，别一上来就软") : (line + "，兄弟们不客气"),
			_ => less ? (line + "，够紧") : (line + "，会叫床"),
		};
	}

	/// <summary>出身要求：按买家阵营换说法。</summary>
	private static string DescribeFactionPref(string value, string buyerFaction)
	{
		string[] lines = buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2]
			{
				"出身要" + value + "的，见过世面才好玩",
				value + "来的最好，调教起来省事"
			},
			"FACTION_UPPER_LEVEL" => new string[2]
			{
				"身子要" + value + "的，干净又懂规矩",
				value + "出身，带出去不丢人"
			},
			"FACTION_SECURITY" => new string[2]
			{
				"底子要" + value + "的，好交代",
				value + "来的，背景越简单越好"
			},
			"FACTION_REVOLUTION" => new string[2]
			{
				value + "出身的，骨头硬，耐折腾",
				"要" + value + "的，扛得住兄弟们轮着来"
			},
			_ => new string[2]
			{
				value + "来的，肯张嘴就行",
				"最好是" + value + "的，懂事又便宜"
			},
		};
		return PickLine(lines);
	}

	/// <summary>职业要求：按买家阵营换说法。</summary>
	private static string DescribeJobPref(string value, string buyerFaction)
	{
		string[] lines = buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2]
			{
				"干过" + value + "的，手上嘴上都得巧",
				value + "出身，接客上手快"
			},
			"FACTION_UPPER_LEVEL" => new string[2]
			{
				"做过" + value + "，伺候人的活该熟",
				"当过" + value + "的，身段应该不错"
			},
			"FACTION_SECURITY" => new string[2]
			{
				"做过" + value + "的？见过血，胆子大",
				value + "干过就成，别太娇气"
			},
			"FACTION_REVOLUTION" => new string[2]
			{
				"当过" + value + "的，体力不能差",
				value + "出身的，能跑能扛"
			},
			_ => new string[2]
			{
				"做过" + value + "就行，能熬",
				value + "出身，别嫌脏"
			},
		};
		return PickLine(lines);
	}

	/// <summary>体型要求：露骨档只会在成年体型下走到这里。</summary>
	private static string DescribeBodyPref(string value, string buyerFaction)
	{
		string[] lines = buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2]
			{
				"身子要" + value + "那种，摆出来有人抢",
				value + "的体型，卖得上价"
			},
			"FACTION_UPPER_LEVEL" => new string[2]
			{
				"要" + value + "的身段，压上去才有手感",
				value + "的身子，才配得上我的床"
			},
			"FACTION_SECURITY" => new string[2]
			{
				value + "的体型，带出去不显眼",
				"要" + value + "那种，别太招摇"
			},
			"FACTION_REVOLUTION" => new string[2]
			{
				value + "的身子，能跟着队伍走",
				value + "的，耐操"
			},
			_ => new string[2]
			{
				"我偏爱" + value + "，抱着实在",
				"要" + value + "的，胸大腰细"
			},
		};
		return PickLine(lines);
	}

	/// <summary>乳量要求：按买家阵营换说法。</summary>
	private static string DescribeBreastPref(string value, string buyerFaction)
	{
		string[] lines = buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2]
			{
				"奶子得是" + value + "，挤得出货才值钱",
				value + "的胸，客人才肯掏钱"
			},
			"FACTION_UPPER_LEVEL" => new string[2]
			{
				"乳量要" + value + "，手感不能差",
				value + "才够看，也够摸"
			},
			"FACTION_SECURITY" => new string[2]
			{
				value + "就行，别太招眼",
				"乳量" + value + "，低调点"
			},
			"FACTION_REVOLUTION" => new string[2]
			{
				value + "的，够兄弟们分",
				value + "的奶子，捏着解乏"
			},
			_ => new string[2]
			{
				"胸要" + value + "，揉着才过瘾",
				value + "最好，晚上抱着睡"
			},
		};
		return PickLine(lines);
	}

	/// <summary>处女要求：按买家阵营换说法。</summary>
	private static string DescribeVirginPref(string value, string buyerFaction)
	{
		string[] lines = buyerFaction switch
		{
			"FACTION_BLACK_MARKET" => new string[2]
			{
				"要" + value + "，头一回能卖贵价",
				value + "的身子，开苞最值钱"
			},
			"FACTION_UPPER_LEVEL" => new string[2]
			{
				"我要" + value + "，别人的印子我不碰",
				value + "，第一次得归我"
			},
			"FACTION_SECURITY" => new string[2]
			{
				value + "的，干净，好交差",
				"要" + value + "，省得验出病来"
			},
			"FACTION_REVOLUTION" => new string[2]
			{
				value + "的身子，给兄弟们开荤",
				value + "最好，前线就缺这个"
			},
			_ => new string[2]
			{
				"最好还是" + value + "，玩起来够紧",
				value + "的，第一次归我"
			},
		};
		return PickLine(lines);
	}

	/// <summary>性经历要求里用的露骨部位名。</summary>
	private static string LewdActName(string act)
	{
		return act switch
		{
			"ORAL" => "嘴",
			"SEX" => "穴",
			_ => "后穴",
		};
	}

	/// <summary>年轻组：只按买家阵营换口气，不写露骨内容。</summary>
	private static string[] YoungPairLines(string buyerFactionName, string maidenFaction)
	{
		return buyerFactionName switch
		{
			"黑市" => new string[2]
			{
				maidenFaction + "来的？黑市缺个跑腿的，跟我走。",
				maidenFaction + "的？能干活就行。"
			},
			"上层区" => new string[2]
			{
				maidenFaction + "的？看着还算干净，带走。",
				maidenFaction + "出身，学规矩应该快。"
			},
			"治安部" => new string[2]
			{
				maidenFaction + "的，登记一下，别出岔子。",
				maidenFaction + "来的？带回去问话。"
			},
			"革命军" => new string[2]
			{
				maidenFaction + "的？前线缺人手，能扛就成。",
				maidenFaction + "来的，跟着队伍走。"
			},
			_ => new string[2]
			{
				maidenFaction + "的？老实听话就行。",
				maidenFaction + "来的，便宜点吧。"
			},
		};
	}

	/// <summary>成年苗床：买家阵营 × 苗床阵营共 25 组，每组两句。</summary>
	private static string[] AdultPairLines(string buyerFactionName, string maidenFaction)
	{
		return (buyerFactionName + "|" + maidenFaction) switch
		{
			"黑市|下层区" => new string[2] { "下层区的小骚货？正好，我那儿缺个会叫的。", "穷地方养出来的，插进去就老实。" },
			"黑市|上层区" => new string[2] { "上层区的大小姐？我出得起价，今晚让她腿软。", "调教过的身子就是水灵，黑市最爱这种。" },
			"黑市|治安部" => new string[2] { "治安部的？胆子不小，放心，没人会去找你。", "穿制服那股傲劲，我早就想撕开。" },
			"黑市|革命军" => new string[2] { "革命军的？先操服了再谈理想。", "嘴硬是吧？床上见真章。" },
			"黑市|黑市" => new string[2] { "自家货？知根知底，用起来最放心。", "黑市的姑娘，床上都懂事。" },
			"上层区|下层区" => new string[2] { "下层区的野味？洗干净，也能上我的床。", "身上脏点没关系，扒光了倒是够味。" },
			"上层区|上层区" => new string[2] { "上层区的名媛？门当户对，玩起来体面。", "上流的身子，得用最贵的方式疼。" },
			"上层区|治安部" => new string[2] { "治安部的？查到我可不好看。不过……刺激。", "穿制服的跪下来，才叫征服。" },
			"上层区|革命军" => new string[2] { "革命军的？床上磨一磨，什么主义都得软。", "反骨最难驯，我倒想试试。" },
			"上层区|黑市" => new string[2] { "黑市来的？没教养，可身子够浪。", "便宜货也行，反正只用来泄火。" },
			"治安部|下层区" => new string[2] { "下层区的？好办，带走也没人追问。", "没背景的最好，玩坏了也没人闹。" },
			"治安部|上层区" => new string[2] { "上层区的？小心点用，别留痕。", "体面人玩起来才够味，可惜不能留名。" },
			"治安部|黑市" => new string[2] { "黑市的脏货？先给我舔干净。", "黑市出来的，规矩不用我教。" },
			"治安部|革命军" => new string[2] { "革命军的？拷问和享用一起办。", "问完就上，省得浪费。" },
			"治安部|治安部" => new string[2] { "……这单我不接。", "人你自己留着吧。" },
			"革命军|下层区" => new string[2] { "下层区的姐妹，先给兄弟们暖被窝。", "穷苦出身，更能吃苦，也更耐操。" },
			"革命军|上层区" => new string[2] { "上层区的？扒了那身丝绸，也就一副身子。", "娇生惯养的，叫起来肯定好听。" },
			"革命军|黑市" => new string[2] { "黑市的骚货，正好犒劳前线。", "会伺候人的最好，兄弟们憋太久了。" },
			"革命军|治安部" => new string[2] { "治安部的？她叫得越大声，我越痛快。", "制服撕开那一下，值这个价。" },
			"革命军|革命军" => new string[2] { "自己人？放心，我会温柔点。", "革命军的姑娘，也是同志，也是女人。" },
			"下层区|下层区" => new string[2] { "街坊家的？知根知底，便宜点吧。", "穷人家的姑娘，胸大屁股翘就行。" },
			"下层区|上层区" => new string[2] { "上层区的？落到我手里，算她倒霉。", "高高在上的样子，操起来最解气。" },
			"下层区|黑市" => new string[2] { "黑市的？别耍花样，脱了让我看看。", "黑市货色，价高但会玩。" },
			"下层区|治安部" => new string[2] { "治安部的？平时抓我，现在轮到你张腿。", "制服一脱，还不都一样。" },
			"下层区|革命军" => new string[2] { "革命军的？嘴上喊得响，床上别怂。", "打完仗还有力气？正好试试。" },
			_ => new string[2] { "货我验过了，钱货两清。", "成交，人我带走。" },
		};
	}

	public static string ArrivalToast(string displayName, string buyerFaction)
	{
		return displayName + "：" + PickLine(ArrivalLines(buyerFaction));
	}

	public static string DemandHeader(bool youngPreferred, string buyerFaction)
	{
		return youngPreferred ? "我要的货：" : PickLine(DemandHeaderLines(buyerFaction));
	}

	public static string DemandTail(bool youngPreferred, string buyerFaction)
	{
		return youngPreferred ? "别多问，按单子上的来。" : PickLine(DemandTailLines(buyerFaction));
	}

	public static string DescribePreference(string[] parts, string buyerFaction, bool neutral)
	{
		return neutral ? DescribeNeutral(parts) : DescribeLewd(parts, buyerFaction);
	}

	public static string FactionPairLine(string buyerFactionName, string maidenFaction, bool young)
	{
		return PickLine(young ? YoungPairLines(buyerFactionName, maidenFaction) : AdultPairLines(buyerFactionName, maidenFaction));
	}

	public static string VirginState(bool virgin, bool analVirgin)
	{
		if (virgin && analVirgin)
		{
			return "双处女";
		}
		if (virgin)
		{
			return "处女";
		}
		if (analVirgin)
		{
			return "肛门处女";
		}
		return "非处女";
	}

	public static string ActLabel(string act)
	{
		return act switch
		{
			"ORAL" => "口交次数",
			"SEX" => "性交次数",
			_ => "肛交次数",
		};
	}

	public static string ActExpected(string sign, string value)
	{
		return sign + " " + value;
	}

	public static string MaidenSummary(string name, string faction, string job, string body, string breast, string virgin)
	{
		return name + " / " + faction + " / " + job + " / " + body + " / " + breast + " / " + virgin;
	}

	public static string SettlementToast(int matchedCount, int reqCount, string mulText, long payout)
	{
		return $"苗床收购完成：满足 {matchedCount}/{reqCount} 条（×{mulText}），获得 {payout} 信用点。";
	}

	public static string[] SecurityRefusalLines()
	{
		return new string[3]
		{
			"……你把她卖给我？",
			"你是在试探我，还是在侮辱我？",
			"这单不做了。人，你自己留着。\n（治安部声望 -10）"
		};
	}

	public static string ReceiptHeader(string buyerName, string maidenSummary)
	{
		return "收购商：" + buyerName + "\n收购对象：" + maidenSummary + "\n\n验收要求：\n";
	}

	public static string ReceiptRequirement(int number, string label, string expected, string actual, bool pass)
	{
		return number + ". " + label + "要求：" + expected + "；实际：" + actual + (pass ? "；√ 符合\n" : "；❌ 不符\n");
	}

	public static string ReceiptSummary(int matchedCount, int reqCount, bool allMatch)
	{
		return "\n验收：满足 " + matchedCount + "/" + reqCount + " 条" + (allMatch ? "（全部满足）" : "") + "\n";
	}

	public static string ReceiptPayout(long baseValue, double matchMul, long payout)
	{
		return "结算：基础价值 " + baseValue + " × " + matchMul.ToString("0.##") + " = " + payout + " 信用点";
	}

	public static string ReceiptBuyerComment(string comment)
	{
		return "\n\n买家评语：" + comment;
	}
}
