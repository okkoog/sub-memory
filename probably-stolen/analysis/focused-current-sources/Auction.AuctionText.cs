using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XIAOWONightAuction;

internal static class AuctionText
{
	private static readonly (string Chinese, string English)[] LogPhrases = new(string, string)[64]
	{
		("拍卖师：", "Auctioneer: "),
		("你已退出本箱竞拍，本箱计入今晚的竞拍场次。未扣除箱款。", "You withdrew. This lot counts toward tonight's auctions. No payment was taken."),
		("AI已完成剩余竞价，省略中间播报。", "AI bidding completed; intermediate announcements skipped."),
		("退出：资金不足", "Out: insufficient funds"),
		("退出：超过心理价", "Out: above my limit"),
		("退出：抬价后收手", "Out: bluff ended"),
		("退出：不再冒险", "Out: no more risk"),
		("无人达到起拍价，本箱流拍。", "No bid reached the opening price. Lot unsold."),
		("无人提交有效报价，本箱流拍。", "No valid bids were submitted. Lot unsold."),
		("未达到本轮领先条件，进入", "Lead threshold not met. Proceed to "),
		("竞拍，最多五轮；领先足够即可提前成交。", " auction, up to five rounds. A sufficient lead wins early."),
		("最多五轮，每轮揭价；领先足够可提前成交。", "Up to five rounds. Bids are revealed each round; a sufficient lead wins early."),
		("内部线索：确认存在高价值物品。", "Inside tip: high-value goods confirmed."),
		("内部线索：存在高价值物品的迹象。", "Inside tip: signs of high-value goods."),
		("内部线索：未发现明显高价值物品。", "Inside tip: no clear sign of high-value goods."),
		("抽检货物：已揭示 ", "Cargo sample: revealed "),
		(" 件真实物品，详见抽检窗口。", " actual items. See the sample window."),
		("复鉴失败：读数超出设备量程，可能存在异常高价值货物。", "Rescan failed: reading beyond range; unusually valuable goods possible."),
		("搅局者已经接近退出价格。", "The disruptor is close to dropping out."),
		("搅局者更像在抬价，赌徒意愿更强。", "The disruptor may be raising prices; the gambler is more committed."),
		("搅局者仍有明显抬价空间。", "The disruptor still has room to raise the price."),
		("这场竞拍已经结束。", "This auction has ended."),
		("报价低于最低接受价格。", "Bid is below the minimum accepted price."),
		("现金不足，无法提交报价。", "Not enough cash to submit this bid."),
		("本轮已经提交报价。", "You already submitted a bid this round."),
		("当前没有待播报的报价。", "No bids are pending announcement."),
		("请先等待三位竞拍者报价。", "Wait for all three rivals to bid."),
		("已提交", "Submitted"),
		("现金不足", "Insufficient cash"),
		("该类别没有可用物品池", "No items available for this category"),
		("本轮未报价", "No bid this round"),
		("暗标已提交", "Sealed bid submitted"),
		("守住报价", "Holding bid"),
		("小幅跟价", "Small raise"),
		("试探报价", "Opening probe"),
		("重新报价", "Revised bid"),
		("购买估值情报，花费 ", "Bought appraisal intel for "),
		("观望报价", "Waiting with bid"),
		("独立估价", "Own valuation"),
		("争抢报价", "Contesting bid"),
		("试探抬价", "Testing a bluff"),
		("购买复鉴道具，花费 ", "Bought a rescan tool for "),
		("未报价", "No bid"),
		("提前得标。", " won early."),
		("在最终轮得标。", " won in the final round."),
		("稳健客预算约 ", "Cautious budget approx. "),
		("稳健客心理价约 ", "Cautious spending limit approx. "),
		("情报估值 ", "Intel value "),
		("复鉴区间 ", "Rescan range "),
		("情报 · ", "Intel - "),
		("情报", "Intel"),
		("稳健客", "Cautious"),
		("赌徒", "Gambler"),
		("搅局者", "Disruptor"),
		("暗标", "Sealed"),
		("公开", "Open"),
		("流拍", "Unsold"),
		("你报价 ", "You bid "),
		("你未报价", "You did not bid"),
		("报价 ", "bid "),
		("以 ", " bid "),
		(" 至 ", " to "),
		("你", "You"),
		("：", ": ")
	};

	private static readonly Dictionary<string, string> English = new Dictionary<string, string>(StringComparer.Ordinal)
	{
		["cash"] = "CASH  {0}",
		["map_entry"] = "CONTAINER AUCTION",
		["title"] = "NIGHT CONTAINER AUCTION",
		["return_store"] = "Return to Shop",
		["join_bid"] = "Enter Auction",
		["cannot_bid"] = "Unavailable",
		["category_medical"] = "Medical Container",
		["category_luxury"] = "Luxury Container",
		["category_contraband"] = "Contraband Container",
		["category_engineering"] = "Engineering Container",
		["container_description"] = "A sealed auction container. Fill its extraction tray; items cannot be deposited.",
		["container_flavor"] = "The seal answers only to the auction house.",
		["container_empty"] = "This auction container is empty.",
		["container_remaining"] = "Sealed container: {0} items left. Take only; no deposits.",
		["remaining_count"] = "Items remaining: {0} / {1}",
		["take_only"] = "9x9 extraction tray - take only, no deposits",
		["fill_output_hint"] = "Extract until the 9x9 tray is full",
		["container_empty_short"] = "Container emptied",
		["fill_output"] = "Fill Extraction Tray",
		["empty_button"] = "Empty"
	};

	internal static string CurrentLocaleCode
	{
		get
		{
			try
			{
				return LocHelper.GetCurrentLocaleCode() ?? string.Empty;
			}
			catch
			{
				return string.Empty;
			}
		}
	}

	internal static bool IsChinese(string localeCode)
	{
		if (!string.IsNullOrWhiteSpace(localeCode))
		{
			if (!localeCode.Equals("zh", StringComparison.OrdinalIgnoreCase) && !localeCode.StartsWith("zh-", StringComparison.OrdinalIgnoreCase))
			{
				return localeCode.StartsWith("zh_", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}
		return false;
	}

	internal static string Select(string chinese, string english)
	{
		return Select(CurrentLocaleCode, chinese, english);
	}

	internal static string Select(string localeCode, string chinese, string english)
	{
		if (!IsChinese(localeCode))
		{
			return english;
		}
		return chinese;
	}

	internal static string BidderName(int index)
	{
		return index switch
		{
			0 => Select("稳健客", "Cautious"), 
			1 => Select("赌徒", "Gambler"), 
			2 => Select("搅局者", "Disruptor"), 
			_ => Select("你", "You"), 
		};
	}

	internal static string DisplayLog(string value)
	{
		if (IsChinese(CurrentLocaleCode) || string.IsNullOrEmpty(value))
		{
			return value;
		}
		value = Regex.Replace(value, "第(\\d+)轮揭价", "Round $1 bids revealed");
		value = Regex.Replace(value, "第(\\d+)轮", "Round $1");
		value = value.Replace(" · ", " - ", StringComparison.Ordinal);
		value = value.Replace("轮。", ".", StringComparison.Ordinal);
		(string, string)[] logPhrases = LogPhrases;
		for (int i = 0; i < logPhrases.Length; i++)
		{
			(string, string) tuple = logPhrases[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			value = value.Replace(item, item2, StringComparison.Ordinal);
		}
		value = value.Replace("。", ".", StringComparison.Ordinal);
		return value;
	}

	internal static string Get(string key, string fallback, params object[] args)
	{
		string text = ModHelper.GetLocalized("xiaowo_night_auction", key, Array.Empty<object>());
		if (text == key)
		{
			text = ((IsChinese(CurrentLocaleCode) || !English.TryGetValue(key, out var value)) ? fallback : value);
		}
		if (args.Length != 0)
		{
			return string.Format(text, args);
		}
		return text;
	}
}
