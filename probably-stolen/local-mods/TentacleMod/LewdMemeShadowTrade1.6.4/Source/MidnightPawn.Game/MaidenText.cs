extern alias OriginalMod;
using NativeApi = OriginalMod::MidnightPawn.Api.TentacleApi;
using System.Text;
using UnityEngine;

namespace MidnightPawn.Game;

public static class MaidenText
{
	private static readonly string[] TierAdult = new string[5]
	{
		"她还带着明显的抗拒，眼神不肯服软。",
		"她的身体已经开始不听使唤，嘴上却还在硬撑。",
		"她藏不住沉沦，呼吸和眼神都变得黏腻。",
		"她顺从得几乎不再反抗，会主动贴近触手。",
		"她彻底变成了离不开触手的苗床。"
	};

	private static readonly string[] TierYoung = new string[5]
	{
		"她还很警惕，缩在束缚囊的角落里。",
		"她不再剧烈挣扎，但仍然不肯开口。",
		"她沉默的时间变多，偶尔会避开你的视线。",
		"她开始习惯束缚囊里的生活，不再试着逃跑。",
		"她已经安静下来，至少不再反抗。"
	};

	private static readonly string[] ModeAdult = new string[5]
	{
		"{name}，{faction}的{job}。{jobFlavor}，{factionFlavor}已经被弄得凌乱，{bodyFlavor}陷在束缚囊里。{tierState}{breastFlavor}",
		"被触手抓回来的{name}是{faction}的{job}。{tierState}她努力维持着最后一点体面，可{bodyFlavor}已经先一步背叛了她。",
		"{name}在束缚囊里轻轻动了一下，{faction}的{job}制服摩擦出细小的声音。{bodyFlavor}随着呼吸起伏，{tierState}{breastFlavor}",
		"“别碰我……”{faction}的{job}{name}低声说。可她的{bodyFlavor}已经软在束缚囊里，{tierState}",
		"触手从束缚囊的缝隙里伸进去，缠住{name}的腰。{faction}的{job}制服被一点点扯开，{bodyFlavor}在触手下发抖。{tierState}{breastFlavor}"
	};

	private static readonly string[] ModeYoung = new string[5]
	{
		"{name}，{faction}的{job}。{jobFlavor}，{tierState}她缩在束缚囊里，{bodyFlavor}。",
		"被触手抓回来的{name}是{faction}的{job}。{tierState}她没有再大声挣扎。",
		"{name}在束缚囊里动了一下，{faction}的{job}衣服上还沾着灰尘。{tierState}{bodyFlavor}",
		"“放我出去……”{name}低声说。{tierState}",
		"触手从束缚囊外收紧，{name}缩起身体。{faction}的{job}制服被弄得凌乱，{tierState}"
	};

	public static string Pregnancy(GameItem item)
	{
		if (item == null)
		{
			return "有个苗床在夜里怀上了。";
		}
		string name = Name(item);
		if (MaidenAgeFilter.IsYoung(item))
		{
			return $"{name}的身体出现了明显变化，她不安地蜷在束缚囊里，手指紧紧抓着衣角。";
		}
		return $"{name}在束缚囊里蜷起身体，手护住小腹。她的呼吸又浅又急，{Faction(item)}的{Job(item)}制服下，身体已经开始为新生命让出位置。";
	}

	public static string Birth(GameItem mother, GameItem daughter)
	{
		string name = Name(mother);
		string faction = Faction(mother);
		string daughterName = ((daughter != null && !string.IsNullOrEmpty(daughter.name)) ? daughter.name : "女儿");
		if (MaidenAgeFilter.IsYoung(mother))
		{
			return $"女儿出生了。{name}虚弱地躺在束缚囊里，触手把新生儿带到安全的地方。她继承了母亲的{faction}身份与立绘，名字是{daughterName}。";
		}
		return $"{name}在束缚囊里生下了女儿。触手接住了那个小小的苗床，她被取名为{daughterName}，继承了母亲的{faction}身份与立绘。";
	}

	public static string Describe(GameItem item)
	{
		if (item == null)
		{
			return "";
		}
		bool young = MaidenAgeFilter.IsYoung(item);
		int tier = TierIndex(item);
		string[] pool = young ? ModeYoung : ModeAdult;
		string text = Fill(pool[StableIndex(Key(item) + "|" + tier, pool.Length)], item, young);
		if (!young)
		{
			text += MaidenPrefs.Flavor(item);
		}
		if (item.IsTag("MAIDEN_PREGNANT"))
		{
			text += (young ? "她的身体出现了明显变化，行动也变得迟缓。" : "小腹已经有了弧度，呼吸里带着怀孕后特有的热气。");
		}
		else if (ItemFactory.GetInt(item, "LACTATION_LV") >= 1)
		{
			text += (young ? "她的身体偶尔会出现一些说不清的变化。" : "胸口偶尔渗出的乳汁把衣料洇湿了一小片。");
		}
		if (NativeApi.IsDaughter(item))
		{
			text += (young ? "她的身体恢复得比普通苗床快。" : "她的身体比普通苗床更快进入状态。");
		}
		if (item.IsTag("MAIDEN_VIRGIN") && item.IsTag("MAIDEN_ANAL_VIRGIN"))
		{
			text += (young ? "她还保留着两层处女。" : "她的两层处女都还在，触手还没真正破开她。");
		}
		return text;
	}

	public static string VirginBreak(GameItem item, bool anal)
	{
		if (item == null)
		{
			return "";
		}
		bool young = MaidenAgeFilter.IsYoung(item);
		string text;
		if (young)
		{
			text = "触手突破了{name}最后的防线。她疼得缩起身体，咬着嘴唇没有出声。";
		}
		else if (anal)
		{
			string[] pool = new string[3]
			{
				"触手挤进{name}的后穴。她猛地弓起背，{faction}的{job}制服被扯得凌乱，带着哭腔的骂声断在半空。束缚囊里留下一点暗红。{prefFlavor}",
				"你从后面压住{name}，把她的腰抬高，触手破开后穴。她还没准备好，尖叫就被束缚囊闷住了。{prefFlavor}",
				"肛门处女被撕开的那一刻，{name}整个人都软了下去。她咬着唇，眼泪和骂声一起漏出来。{prefFlavor}"
			};
			text = pool[StableIndex(Key(item) + "|anal", pool.Length)];
		}
		else
		{
			string[] pool = new string[3]
			{
				"触手终于顶开了{name}最后的防线。她整个人绷紧，{faction}的{job}制服被汗水浸湿，压抑的叫声从喉咙里漏出来。束缚囊里落下一点醒目的红。{prefFlavor}",
				"你按住{name}的腰，把她钉在束缚囊里。她的身体猛地一颤，指甲抓进触手表面，{faction}的{job}身份和那层处女一起被撕开。{prefFlavor}",
				"触手一点点挤进{name}的身体。她哭着摇头，双腿却被强行分开，最后那点阻碍终于被顶开。{prefFlavor}"
			};
			text = pool[StableIndex(Key(item) + "|vaginal", pool.Length)];
		}
		return Fill(text, item, young);
	}

	private static string Fill(string template, GameItem item, bool young)
	{
		return template.Replace("{name}", Name(item)).Replace("{faction}", Faction(item)).Replace("{job}", Job(item))
			.Replace("{jobFlavor}", JobFlavor(Job(item)))
			.Replace("{factionFlavor}", FactionFlavor(Faction(item)))
			.Replace("{bodyFlavor}", young ? BodyFlavorYoung(Body(item)) : BodyFlavorAdult(Body(item)))
			.Replace("{tierState}", TierState(TierIndex(item), young))
			.Replace("{breastFlavor}", young ? "" : BreastFlavor(ItemFactory.GetStr(item, "MAIDEN_BREAST")))
			.Replace("{prefFlavor}", young ? "" : MaidenPrefs.Flavor(item));
	}

	private static int TierIndex(GameItem item)
	{
		int @int = ItemFactory.GetInt(item, "TRAIN_VALUE");
		if (@int < 60)
		{
			return 0;
		}
		if (@int < 130)
		{
			return 1;
		}
		if (@int < 210)
		{
			return 2;
		}
		if (@int < 300)
		{
			return 3;
		}
		return 4;
	}

	private static string TierState(int tier, bool young)
	{
		string[] array = young ? TierYoung : TierAdult;
		return array[Mathf.Clamp(tier, 0, array.Length - 1)];
	}

	private static string FactionFlavor(string faction)
	{
		return faction switch
		{
			"上层区" => "熨得笔挺的上层区制服",
			"治安部" => "治安部的制式制服",
			"黑市" => "黑市的兜帽和皮衣",
			"革命军" => "革命军的旧军装",
			_ => "沾着灰的下层区工装"
		};
	}

	private static string Body(GameItem item)
	{
		return ItemFactory.GetStr(item, "MAIDEN_BODY");
	}

	private static string BodyFlavorAdult(string body)
	{
		return body switch
		{
			"御姐" => "高挑成熟的身体",
			"少妇" => "柔软丰润的身体",
			"熟女" => "丰腴沉静的身体",
			_ => "身体"
		};
	}

	private static string BodyFlavorYoung(string body)
	{
		return body switch
		{
			"少女" => "纤细的肩膀",
			"萝莉" => "单薄的身形",
			_ => "身体"
		};
	}

	private static string Name(GameItem item)
	{
		string text = ItemFactory.GetStr(item, "MAIDEN_NAME");
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return string.IsNullOrEmpty(item.name) ? "苗床" : item.name;
	}

	private static string Faction(GameItem item)
	{
		string text = ItemFactory.GetStr(item, "MAIDEN_FACTION");
		return string.IsNullOrEmpty(text) ? "下层区" : text;
	}

	private static string Job(GameItem item)
	{
		string text = ItemFactory.GetStr(item, "MAIDEN_JOB");
		return string.IsNullOrEmpty(text) ? "居民" : text;
	}

	private static string JobFlavor(string job)
	{
		return job switch
		{
			"拾荒者" => "身上还带着垃圾场的灰尘",
			"矿工" => "指节上还沾着矿尘",
			"雇佣兵" => "腰间还挂着空枪套",
			"工程师" => "袖口有机油味",
			"技师" => "指尖还有没洗掉的机油",
			"维修工" => "衣服上沾着金属碎屑",
			"护士" => "身上有消毒水味",
			"医生" => "白大褂还没来得及脱",
			"厨师" => "衣领上残留着香料味",
			"酒保" => "身上有淡淡的酒气",
			"农场主" => "带着泥土和青草味",
			"研究员" => "白大褂上还别着编号牌",
			"商人" => "手指习惯性地摸着藏东西的口袋",
			"走私者" => "外衣下藏着不少暗袋",
			"小偷" => "手指还在下意识地摸索退路",
			"文员" => "袖口熨得笔直",
			"清洁员" => "衣服洗得发白",
			"货运员" => "肩上还有搬运留下的红痕",
			"学徒" => "动作还带着生涩",
			"艺人" => "身上有淡淡的舞台香水味",
			_ => "呼吸乱得厉害"
		};
	}

	private static string BreastFlavor(string breast)
	{
		return breast switch
		{
			"贫乳" => "胸前起伏得很急",
			"巨乳" => "胸口的布料被撑得发紧",
			_ => "胸口随着呼吸起伏"
		};
	}

	private static string Key(GameItem item)
	{
		return Name(item) + "|" + Faction(item) + "|" + Job(item) + "|" + ItemFactory.GetStr(item, "MAIDEN_BODY") + "|" + ItemFactory.GetStr(item, "MAIDEN_BREAST");
	}

	private static int StableIndex(string key, int count)
	{
		if (count <= 1)
		{
			return 0;
		}
		int num = 17;
		foreach (char c in key)
		{
			num = num * 31 + c;
		}
		return (num & 0x7FFFFFFF) % count;
	}
}

