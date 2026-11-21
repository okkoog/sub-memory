using System;
using System.Globalization;

namespace MidnightPawn.Game;

/// <summary>四种桶装共用的数量输入与逐瓶取出流程。</summary>
internal static class BarrelDispense
{
	private const string WindowId = "lewd_meme_remake.barrel_dispense";

	internal static void Open(GameItem barrel, string baseItemId)
	{
		if (!CanDispense(barrel) || CustomUIManager.Instance == null) return;
		CustomUIManager.Instance.CloseWindow(WindowId);
		bool finished = false;
		CustomUIBuilder builder = CustomUIManager.Window(WindowId, "取出瓶装物品", "overlay")
			.SetSize(500f, 300f).Center().SetDraggable(true).SetCloseOnEscape(true)
			.OnClose(() => finished = true);
		builder.AddLabel(barrel.name + "：当前剩余 " + ItemFactory.GetBarrelUses(barrel) + " 瓶。\n输入取出数量；放不下时立即停止，未取出的部分留在桶中。", null);
		builder.Last.SetFixedWidth(450f);
		if (builder.Last.text != null) builder.Last.text.enableWordWrapping = true;
		builder.AddInput("请输入正整数", "1", null, "quantity", null);
		CustomUIElement input = builder.Last;
		if (input.input != null) input.input.characterLimit = 32;
		builder.AddLabel("优先放在桶所在区域，其次为己方柜台、仓库。", "status");
		CustomUIElement status = builder.Last;
		status.SetFixedWidth(450f);
		if (status.text != null) status.text.enableWordWrapping = true;
		builder.AddButton("取出", () =>
		{
			if (finished) return;
			if (!CanDispense(barrel))
			{
				status.SetText("桶已不可用，请关闭窗口后重新选择。");
				return;
			}
			if (!TryParseQuantity(input.GetString(), ItemFactory.GetBarrelUses(barrel), out int requested, out string error))
			{
				status.SetText(error);
				return;
			}
			finished = true;
			CustomUIManager.Instance.CloseWindow(WindowId);
			int taken = Dispense(barrel, baseItemId, requested, out string reason);
			Toast.Show("已取出 " + taken + " / " + requested + " 瓶。" + (string.IsNullOrEmpty(reason) ? "" : "\n" + reason), "桶装取出");
		}, null);
		builder.AddButton("取消", () => CustomUIManager.Instance.CloseWindow(WindowId), null);
		builder.Show();
	}

	internal static bool TryParseQuantity(string text, int available, out int quantity, out string error)
	{
		quantity = 0;
		error = null;
		if (!int.TryParse(text?.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int parsed) || parsed <= 0)
		{
			error = "请输入大于 0 的整数，不支持小数、符号或其它文字。";
			return false;
		}
		if (parsed > ItemFactory.BarrelMaxUses || parsed > available)
		{
			error = "取出数量不能超过当前剩余的 " + Math.Max(0, Math.Min(available, ItemFactory.BarrelMaxUses)) + " 瓶。";
			return false;
		}
		quantity = parsed;
		return true;
	}

	private static bool CanDispense(GameItem barrel)
	{
		return barrel != null && ItemFactory.IsBarrelIdentifier(barrel.identifier)
			&& GeneralHelper.IsItemOwned(barrel) && UseCountHelper.CanUse(barrel)
			&& barrel.parentInventory != null && barrel.parentInventory.childItems != null
			&& barrel.parentInventory.childItems.Contains(barrel);
	}

	internal static int Dispense(GameItem barrel, string baseItemId, int requested, out string reason)
	{
		reason = null;
		if (!CanDispense(barrel) || !MatchesLiquid(barrel.identifier, baseItemId))
		{
			reason = "桶已不可用，请重新选择。";
			return 0;
		}
		if (requested <= 0 || requested > ItemFactory.BarrelMaxUses || requested > ItemFactory.GetBarrelUses(barrel))
		{
			reason = "数量无效或桶内余量已变化，请重新输入。";
			return 0;
		}
		int taken = 0;
		for (int i = 0; i < requested; i++)
		{
			GameItem bottle = null;
			bool placed = false;
			try
			{
				if (!CanDispense(barrel))
				{
					reason = "桶已不可用，已停止取出。";
					break;
				}
				bottle = DirectoryMaster.Item(baseItemId, true);
				if (bottle == null)
				{
					reason = "瓶装物品创建失败，未取出的部分仍在桶中。";
					break;
				}
				ModHelper.SetItemOwnerShip(bottle, true);
				bottle.SetUnitCount(1);
				GameInventory target = barrel.parentInventory;
				if (GraphUtils.CanAccept(target, bottle))
				{
					// 明确指定一瓶；返回剩余 0 也涵盖合入已有堆叠的成功情况。
					placed = GraphUtils.TryAcceptAll(target, bottle, 1) == 0;
				}
				if (!placed) placed = ItemFactory.TryPlaceSafely(bottle, out _);
				if (!placed)
				{
					reason = "没有足够空间，已停止；未取出的部分仍在桶中。";
					break;
				}
				UseCountHelper.Use(barrel);
				taken++;
			}
			catch (Exception ex)
			{
				ModCtx.LogWarning("桶装批量取出失败: " + ex.Message);
				reason = "取出时发生异常，已停止。";
				break;
			}
			finally
			{
				if (!placed && bottle != null && bottle.parentInventory == null)
				{
					try { bottle.Destroy(); }
					catch (Exception ex) { ModCtx.LogWarning("清理未放置瓶装物品失败: " + ex.Message); }
				}
			}
		}
		return taken;
	}

	private static bool MatchesLiquid(string barrelId, string baseItemId)
	{
		return (barrelId == ItemFactory.BarrelSemenId && baseItemId == "tentacle_semen")
			|| (barrelId == ItemFactory.BarrelMilkWondrousId && baseItemId == "milk_wondrous")
			|| (barrelId == ItemFactory.BarrelMilkRichId && baseItemId == "milk_rich")
			|| (barrelId == ItemFactory.BarrelMilkDilutedId && baseItemId == "milk_diluted");
	}
}
