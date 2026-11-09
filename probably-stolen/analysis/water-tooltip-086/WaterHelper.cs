using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterHelper
{
	public const string LIQUID_CONTAINER_CAPACITY = "LIQUID_CONTAINER_CAPACITY";

	public const string LIQUID_CONTAINER_BASE_PRICE_TAG = "LIQUID_CONTAINER_BASE_PRICE_TAG";

	public const string LIQUID_CONTAINER_TAG = "LIQUID_CONTAINER_TAG";

	public const string STRIP_TESTED_TAG = "STRIP_TESTED_TAG";

	public const string FILTER_CONTAINER_TAG = "FILTER_CONTAINER_TAG";

	public const string GRADUATED_CONTAINER_TAG = "GRADUATED_CONTAINER_TAG";

	public const string LAST_SCAN_RESULT_TAG = "LAST_SCAN_RESULT_TAG";

	public const string LIQUID_CONTAINER_DROPPER_TAG = "LIQUID_CONTAINER_DROPPER_TAG";

	private static readonly HashSet<string> WaterBottleIds = new HashSet<string> { "small_bottled_water", "bottled_water", "mini_bottle", "large_bottled_water", "water_jug" };

	private const double LOWER_EDGE_SCALE = 0.08;

	public const int NUDKA_VOLUME_ML = 750;

	public const int NUDKA_ALCOHOL_PERCENT = 45;

	public const int CONTAMINANT_PERCENTAGE_MAX = 1000000;

	public const int DRY_TOLERANCE_UL = 100;

	public static void UpdateWaterBottleSound(GameItem item)
	{
		if (WaterBottleIds.Contains(item.identifier))
		{
			if (GetCurrentCapacity(item) > 10)
			{
				AudioHelper.InitFullPlasticBottle(item);
			}
			else
			{
				AudioHelper.InitEmptyPlasticBottle(item);
			}
		}
	}

	public static void InitLiquidContainerItem(GameItem item, int capacityInMl, int basePrice, bool isFilterContainer = false, bool isGraduated = false, bool isDropper = false, bool isTransparentContainer = false, int sheetPage = 3)
	{
		item.EnableTag("LIQUID_CONTAINER_TAG");
		item.ModifyTag("LIQUID_CONTAINER_BASE_PRICE_TAG", (TagState state) =>
		{
			state.Enable().SetInt(basePrice);
		});
		if (isDropper)
		{
			item.EnableTag("LIQUID_CONTAINER_DROPPER_TAG");
		}
		ChemicalFeedbackHelper.InitChemicalIntentItem(item, ChemicalFeedbackHelper.ChemicalIntent.HYDRATION);
		if (isFilterContainer)
		{
			item.EnableTag("FILTER_CONTAINER_TAG");
		}
		ChemicalHelper.InitContainerTemp(item);
		if (capacityInMl > 2147483)
		{
			Debug.LogError("Liquid container capacity " + capacityInMl + " ml exceeds the storage ceiling, clamping to " + 2147483 + " ml: " + item.identifier);
			capacityInMl = 2147483;
		}
		int capacityInParts = capacityInMl * 1000;
		item.ModifyTag("LIQUID_CONTAINER_CAPACITY", (TagState state) =>
		{
			state.Enable().SetInt(capacityInParts);
		});
		if (isGraduated)
		{
			item.EnableTag("GRADUATED_CONTAINER_TAG");
		}
		else
		{
			item.DisableTag("GRADUATED_CONTAINER_TAG");
		}
		foreach (Liquid liquid in Liquid.Liquids)
		{
			item.ModifyTag(liquid.currentPart, (TagState state) =>
			{
				state.Enable().SetInt(0);
			});
		}
		UpdateContainerGoodType(item);
		item.maySelectSlotItemFunc = (GameItem gameItem, GameInventory _, SlotMarker _) => CanUseWaterContainer(gameItem);
		item.canSelectSlotItemFunc = (GameItem gameItem, GameInventory _, SlotMarker _) => CanUseWaterContainer(gameItem);
		item.mayThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.mayThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => (t.IsTag("LIQUID_CONTAINER_TAG") && CanUseWaterContainer(t) && CanUseWaterContainer(s)) || (IsDrain(t) && CanUseWaterContainer(s) && !IsEmpty(s)) || (IsFaucet(t) && CanUseWaterContainer(s))));
		item.canThisTargetItemFunc = (Func<GameItem, GameItem, bool>)Delegate.Combine(item.canThisTargetItemFunc, (Func<GameItem, GameItem, bool>)((GameItem s, GameItem t) => (t.IsTag("LIQUID_CONTAINER_TAG") && CanUseWaterContainer(t) && CanUseWaterContainer(s)) || (IsDrain(t) && CanUseWaterContainer(s) && !IsEmpty(s)) || (IsFaucet(t) && CanUseWaterContainer(s))));
		item.onThisTargetItemFunc = (Action<GameItem, GameItem>)Delegate.Combine(item.onThisTargetItemFunc, (Action<GameItem, GameItem>)((GameItem s, GameItem t) =>
		{
			if (t.IsTag("LIQUID_CONTAINER_TAG"))
			{
				if (!isDropper)
				{
					if (!IsEmpty(s))
					{
						if (s.identifier.Equals("water_pitcher"))
						{
							AudioHelper.UseWaterFilter();
						}
						else
						{
							AudioHelper.PourWater();
						}
					}
					TransferLiquid(s, t);
					UpdateWaterBottleSound(s);
					UpdateWaterBottleSound(t);
				}
				else
				{
					int totalVolume = GetTotalVolume(s);
					if (IsEmpty(s))
					{
						TransferLiquid(t, s);
						UpdateWaterBottleSound(t);
						UpdateWaterBottleSound(s);
						if (GetTotalVolume(s) > totalVolume)
						{
							AudioHelper.DropperDraw();
						}
					}
					else
					{
						TransferLiquid(s, t);
						UpdateWaterBottleSound(s);
						UpdateWaterBottleSound(t);
						if (GetTotalVolume(s) < totalVolume)
						{
							AudioHelper.DropperDrop();
						}
					}
				}
			}
			else if (IsDrain(t))
			{
				EmptyContainer(s);
				AudioHelper.DumpWater();
				UpdateWaterBottleSound(s);
			}
			else if (IsFaucet(t))
			{
				if (NetworkUpgrade.GetUpgradeById("RUSTWATER_TAP").IsUnlocked())
				{
					AudioHelper.UseFaucet();
					AddWater(s, 1, 9999999, useRange: true, StoreStation.Instance.todayRustwaterPurity, StoreStation.Instance.todayRustwaterPurity);
					UpdateWaterBottleSound(s);
				}
				else if (NetworkUpgrade.GetUpgradeById("GUTTERFLOW_TAP").IsUnlocked())
				{
					AudioHelper.UseFaucet();
					AddWater(s, 1, 9999999, useRange: true, StoreStation.Instance.todayGutterflowPurity, StoreStation.Instance.todayGutterflowPurity);
					UpdateWaterBottleSound(s);
				}
			}
		}));
		ChemicalHelper.InitContainerTempFunction(item);
		WaterFeatureHelper.InitWaterFeature(item);
		if (isDropper)
		{
			item.getItemSpriteModifierForItemFunc = (GameItem gameItem) =>
			{
				int currentCapacity = GetCurrentCapacity(gameItem);
				if (currentCapacity != 0)
				{
					SpriteHelper.MapCapacity(currentCapacity);
					string spritePath = gameItem.spritePath;
					List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
					string liquidContainerColor = GetLiquidContainerColor(gameItem);
					list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/water_container3", spritePath + "_" + Liquid.GetColorBase(liquidContainerColor)), new Vector2(0f, 0f), 0, isFlipped: false, 0, haveMask: false, MetaHelper.HexToColor(liquidContainerColor)));
					return list;
				}
				return (List<ItemSpriteModifier>)null;
			};
		}
		else if (isTransparentContainer)
		{
			item.getItemSpriteModifierForItemFunc = (GameItem gameItem) =>
			{
				int currentCapacity = GetCurrentCapacity(gameItem);
				if (currentCapacity == 0)
				{
					return (List<ItemSpriteModifier>)null;
				}
				string text = SpriteHelper.MapCapacity(currentCapacity);
				string spritePath = gameItem.spritePath;
				List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
				string liquidContainerColor = GetLiquidContainerColor(gameItem);
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/water_container" + sheetPage, spritePath + "_" + Liquid.GetColorBase(liquidContainerColor) + "_" + text), new Vector2(0f, 0f), 0, isFlipped: false, 0, haveMask: false, MetaHelper.HexToColor(liquidContainerColor)));
				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/water_container" + sheetPage, spritePath + "_hl_" + text), new Vector2(0f, 0f)));
				return list;
			};
		}
		UpdateWaterBottleSound(item);
	}

	public static int[] SplitProportionally(int[] parts, int amount)
	{
		int[] array = new int[parts.Length];
		long num = 0L;
		int[] array2 = parts;
		foreach (int num2 in array2)
		{
			if (num2 > 0)
			{
				num += num2;
			}
		}
		if (num <= 0 || amount <= 0)
		{
			return array;
		}
		if (amount >= num)
		{
			for (int j = 0; j < parts.Length; j++)
			{
				array[j] = Math.Max(0, parts[j]);
			}
			return array;
		}
		long[] remainders = new long[parts.Length];
		long num3 = 0L;
		for (int k = 0; k < parts.Length; k++)
		{
			if (parts[k] > 0)
			{
				long num4 = (long)parts[k] * (long)amount;
				array[k] = (int)(num4 / num);
				remainders[k] = num4 % num;
				num3 += array[k];
			}
		}
		long num5 = amount - num3;
		int[] array3 = (from num7 in Enumerable.Range(0, parts.Length)
			where remainders[num7] > 0
			orderby remainders[num7] descending, parts[num7] descending
			select num7).ToArray();
		int num6 = 0;
		while (num6 < array3.Length && num5 > 0)
		{
			array[array3[num6]]++;
			num6++;
			num5--;
		}
		return array;
	}

	public static int[] GetLiquidParts(GameItem container)
	{
		int[] array = new int[Liquid.Liquids.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = container.GetTagReadonly(Liquid.Liquids[i].currentPart).GetInt();
		}
		return array;
	}

	public static void TransferLiquid(GameItem sourceContainer, GameItem targetContainer)
	{
		bool wasEmpty = IsEmpty(targetContainer);
		if (!sourceContainer.IsTag("LIQUID_CONTAINER_TAG") || !targetContainer.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int num = targetContainer.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
		int num2 = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num2 += targetContainer.GetTagReadonly(liquid.currentPart).GetInt();
		}
		int num3 = num - num2;
		if (num3 <= 0)
		{
			return;
		}
		int num4 = 0;
		foreach (Liquid liquid2 in Liquid.Liquids)
		{
			num4 += sourceContainer.GetTagReadonly(liquid2.currentPart).GetInt();
		}
		if (num4 <= 0)
		{
			return;
		}
		int amount = Math.Min(num3, num4);
		GameItem filterItem = GetFilterItem(targetContainer);
		int[] array = SplitProportionally(GetLiquidParts(sourceContainer), amount);
		for (int i = 0; i < Liquid.Liquids.Count; i++)
		{
			int sourcePartAmount = sourceContainer.GetTagReadonly(Liquid.Liquids[i].currentPart).GetInt();
			int amountToTransfer = array[i];
			int amountToRemove = amountToTransfer;
			amountToTransfer = HandleFilter(Liquid.Liquids[i].identifier, amountToTransfer, sourceContainer, filterItem);
			int targetPartAmount = targetContainer.GetTagReadonly(Liquid.Liquids[i].currentPart).GetInt();
			ChemicalHelper.HandleReceiveLiquidTemp(targetContainer, amountToTransfer, sourceContainer);
			targetContainer.ModifyTag(Liquid.Liquids[i].currentPart, (TagState state) =>
			{
				state.Enable().SetInt(targetPartAmount + amountToTransfer);
			}, skipValidate: true);
			sourceContainer.ModifyTag(Liquid.Liquids[i].currentPart, (TagState state) =>
			{
				state.Enable().SetInt(sourcePartAmount - amountToRemove);
			}, skipValidate: true);
		}
		WaterFeatureHelper.OnLiquidContainerPourInto(targetContainer, sourceContainer, wasEmpty);
		WaterFeatureHelper.OnLiquidContainerPourOutOf(sourceContainer);
	}

	public static GameItem GetFilterItem(GameItem game)
	{
		if (game.IsTag("FILTER_CONTAINER_TAG"))
		{
			foreach (GameItem item in game.FindAllChildItems())
			{
				if (item.IsTag("WATER_FILTER"))
				{
					return item;
				}
			}
		}
		return null;
	}

	public static int HandleFilter(string identifier, int amount, GameItem sourceContainer, GameItem filter)
	{
		if (amount <= 0)
		{
			return 0;
		}
		if (filter == null)
		{
			return amount;
		}
		if (filter.identifier.Equals("uv_filter"))
		{
			if (identifier.Equals("microbe"))
			{
				return 0;
			}
			return amount;
		}
		if (filter.IsTag("WATER_FILTER"))
		{
			int num = amount;
			int realDurability = DurabilityHelper.GetRealDurability(filter);
			if (realDurability == 0)
			{
				return amount;
			}
			if (identifier.Equals("microbe"))
			{
				return amount;
			}
			foreach (Liquid liquid in Liquid.Liquids)
			{
				if (!identifier.Equals(liquid.identifier))
				{
					continue;
				}
				int num2 = filter.GetTagReadonly("FILTER_SIZE_TAG").GetInt();
				int particleSize = liquid.particleSize;
				if (num2 > particleSize)
				{
					break;
				}
				int num3 = (int)((double)num * liquid.filterMinimumPurity);
				int num4 = 0;
				while (realDurability > 0 && amount > num3 && num4 < 10000)
				{
					if (liquid.filterVolumeRemoved <= 0)
					{
						Debug.LogError("Invalid filterVolumeRemoved for " + liquid.identifier);
						break;
					}
					DurabilityHelper.ChangeDurability(filter, -liquid.filterDurabilityUsage);
					realDurability = DurabilityHelper.GetRealDurability(filter);
					amount -= liquid.filterVolumeRemoved;
					num4++;
				}
				amount = Mathf.Clamp(amount, num3, num);
				break;
			}
			return Mathf.Max(0, amount);
		}
		return amount;
	}

	public static int GetFreeCapacity(GameItem item)
	{
		if (!item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return 0;
		}
		int num = item.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
		int num2 = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num2 += item.GetTagReadonly(liquid.currentPart).GetInt();
		}
		return num - num2;
	}

	public static int GetTotalCapacity(GameItem item)
	{
		if (!item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return 0;
		}
		return item.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
	}

	public static int GetTotalCapacityML(GameItem item)
	{
		if (!item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return 0;
		}
		return item.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt() / 1000;
	}

	public static void RemoveExcessLiquid(GameItem item)
	{
		int currentCapacityUL = GetCurrentCapacityUL(item);
		int totalCapacity = GetTotalCapacity(item);
		if (currentCapacityUL > totalCapacity)
		{
			Remove(item, currentCapacityUL - totalCapacity);
		}
	}

	public static int GetCurrentCapacity(GameItem item)
	{
		if (!item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return 0;
		}
		int num = item.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
		int num2 = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num2 += item.GetTagReadonly(liquid.currentPart).GetInt();
		}
		return (int)((long)num2 * 100L / num);
	}

	public static int GetCurrentCapacityML(GameItem item)
	{
		if (!item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return 0;
		}
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num += item.GetTagReadonly(liquid.currentPart).GetInt();
		}
		return num / 1000;
	}

	public static int GetCurrentCapacityUL(GameItem item)
	{
		if (!item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return 0;
		}
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num += item.GetTagReadonly(liquid.currentPart).GetInt();
		}
		return num;
	}

	public static string GetEstimatedVolume(int currentVolume, int totalVolume)
	{
		if (totalVolume <= 0)
		{
			return "Empty";
		}
		float num = (float)currentVolume / (float)totalVolume * 100f;
		if (num >= 100f)
		{
			return "Full";
		}
		if (num > 90f)
		{
			return "Almost Full";
		}
		if (num > 70f)
		{
			return "More Than Half Full";
		}
		if (num > 50f)
		{
			return "Half Full";
		}
		if (num > 30f)
		{
			return "Less Than Half Full";
		}
		if (num > 0f)
		{
			return "Almost Empty";
		}
		return "Empty";
	}

	public static void CreateWaterContainerTooltip(RichTextBuilder builder, GameItem item)
	{
		if (item.IsTag("WINE_CONTAINER_TAG") || !item.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int num = item.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
		int num2 = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num2 += item.GetTagReadonly(liquid.currentPart).GetInt();
		}
		if (item.IsTag("GRADUATED_CONTAINER_TAG") || StartingPerk.IsExperiencedChemist())
		{
			float ratio = (float)num2 / (float)num;
			builder.AddProgressBar(ratio, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
			builder.AddLine(" " + ((float)num2 / 1000f).ToString("F0") + " ml / " + ((float)num / 1000f).ToString("F0") + " ml", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		else
		{
			string estimatedVolume = GetEstimatedVolume(num2, num);
			builder.AddLine(estimatedVolume + ", Capacity:" + ((float)num / 1000f).ToString("F0") + " ml", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
		}
		if (item.IsTag("LAST_SCAN_RESULT_TAG"))
		{
			string text = item.GetTagReadonly("LAST_SCAN_RESULT_TAG").GetString();
			if (!string.IsNullOrEmpty(text))
			{
				builder.AddLine(text, ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
			}
		}
		if (PlayerStore.Instance.devMode)
		{
			ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(item);
			builder.AddLine("[Debug]Real Condition: " + waterFeature.GetBaseRealDisplay());
			builder.AddLine("[Debug]Fake Condition: " + waterFeature.GetBaseFakeDisplay());
			builder.AddLine("Liquid Base: " + Liquid.GetColorBase(GetLiquidContainerColor(item)));
		}
	}

	public static int GetWaterPurity(GameItem gameItem)
	{
		if (!gameItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return 0;
		}
		int num = gameItem.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
		int num2 = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num2 += gameItem.GetTagReadonly(liquid.currentPart).GetInt();
		}
		if (num2 > 0)
		{
			return (int)((long)num * 10000L / num2);
		}
		return 0;
	}

	public static int GetTotalVolume(GameItem gameItem)
	{
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num += gameItem.GetTagReadonly(liquid.currentPart).GetInt();
		}
		return num;
	}

	public static void UpdateContainerGoodType(GameItem gameItem)
	{
		if (gameItem.IsTag("LIQUID_CONTAINER_BASE_PRICE_TAG"))
		{
			bool flag = gameItem.IsGameItemType("HOUSEHOLD_GOOD");
			if (IsEmpty(gameItem))
			{
				gameItem.RemoveGameItemType("WATER");
				gameItem.SetGameItemType("HOUSEHOLD_GOOD");
			}
			else
			{
				gameItem.SetGameItemType("WATER");
				gameItem.RemoveGameItemType("HOUSEHOLD_GOOD");
			}
			if (flag != gameItem.IsGameItemType("HOUSEHOLD_GOOD") && gameItem.IsRootedInBackInv() && (gameItem.parentInventory == null || gameItem.parentInventory.GetParentItem() == null))
			{
				gameItem.RemoveFeatureRelatedToEvent();
				gameItem.AddFeatureRelatedToEvent();
			}
		}
	}

	public static int GetContainerPrice(GameItem gameItem)
	{
		if (!gameItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return 0;
		}
		return gameItem.GetTagReadonly("LIQUID_CONTAINER_BASE_PRICE_TAG").GetInt();
	}

	public static void EmptyContainer(GameItem gameItem)
	{
		if (!gameItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		foreach (Liquid liquid in Liquid.Liquids)
		{
			gameItem.ModifyTag(liquid.currentPart, (TagState state) =>
			{
				state.Enable().SetInt(0);
			});
		}
		gameItem.unitValue = WaterFeatureHelper.GetWaterPrice(0);
		WaterFeatureHelper.UpdateWaterFeature(gameItem, isExpose: true);
		if (IsEmpty(gameItem))
		{
			WaterFeatureHelper.GetWaterFeature(gameItem).SetExposed(value: false);
			gameItem.DisableTag("STRIP_TESTED_TAG");
		}
		SpriteHelper.UpdateWaterContainerSprite(gameItem);
	}

	public static bool IsEmpty(GameItem gameItem)
	{
		if (!gameItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return true;
		}
		foreach (Liquid liquid in Liquid.Liquids)
		{
			if (gameItem.GetTagReadonly(liquid.currentPart).GetInt() > 0)
			{
				return false;
			}
		}
		return true;
	}

	public static bool CanUseWaterContainer(GameItem gameItem)
	{
		if (GeneralHelper.IsItemOwned(gameItem))
		{
			return true;
		}
		return false;
	}

	public static void OnWaterPurificationTabletAdded2(GameItem waterContainer)
	{
		if (waterContainer.IsTag("LIQUID_CONTAINER_TAG"))
		{
			int num = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("microbe")).GetInt();
			int newMicrobialAmount = Mathf.Max(0, num - 50000);
			waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("microbe"), (TagState state) =>
			{
				state.Enable().SetInt(newMicrobialAmount);
			});
			int num2 = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("chemical_contaminant")).GetInt();
			int newChemicalAmount = Mathf.Max(500, num2 - 5000);
			waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("chemical_contaminant"), (TagState state) =>
			{
				state.Enable().SetInt(newChemicalAmount);
			});
			WaterFeatureHelper.UpdateWaterFeature(waterContainer, isExpose: false);
		}
	}

	public static void OnWaterPurificationTabletAdded(GameItem waterContainer)
	{
		if (waterContainer.IsTag("LIQUID_CONTAINER_TAG"))
		{
			int amount1 = RemoveContaminantFromContainer(waterContainer, "microbe", 75000, 0);
			int amount2 = RemoveContaminantFromContainer(waterContainer, "chemical_contaminant", 10000, 5);
			int targetPartAmount = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
			waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("water"), (TagState state) =>
			{
				state.Enable().SetInt(targetPartAmount + amount1);
			});
			targetPartAmount = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
			waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("water"), (TagState state) =>
			{
				state.Enable().SetInt(targetPartAmount + amount2);
			});
			WaterFeatureHelper.UpdateWaterFeature(waterContainer, isExpose: false);
		}
	}

	private static int SumParts(GameItem container, List<Liquid> liquids)
	{
		int num = 0;
		foreach (Liquid liquid in liquids)
		{
			num += container.GetTagReadonly(liquid.currentPart).GetInt();
		}
		return num;
	}

	private static int ShrinkProportionally(GameItem container, List<Liquid> liquids, int currentSum, int targetSum)
	{
		if (currentSum <= 0)
		{
			return 0;
		}
		int num = 0;
		foreach (Liquid liquid in liquids)
		{
			int num2 = container.GetTagReadonly(liquid.currentPart).GetInt();
			if (num2 > 0)
			{
				int newAmt = (int)((long)num2 * (long)targetSum / currentSum);
				container.ModifyTag(liquid.currentPart, (TagState state) =>
				{
					state.Enable().SetInt(newAmt);
				});
				num += newAmt;
			}
		}
		return num;
	}

	public static void PurifyToBaseWater(GameItem container)
	{
		if (!container.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int total = GetTotalVolume(container);
		if (total <= 0)
		{
			return;
		}
		int num = container.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
		int num2 = (int)(((long)total * 9800L + 9999) / 10000);
		if (num >= num2)
		{
			return;
		}
		List<Liquid> list = new List<Liquid>();
		List<Liquid> list2 = new List<Liquid>();
		foreach (Liquid liquid in Liquid.Liquids)
		{
			if (!(liquid.identifier == "water"))
			{
				if (liquid.identifier == "chemical_contaminant" || liquid.identifier == "heavy_metal")
				{
					list.Add(liquid);
				}
				else
				{
					list2.Add(liquid);
				}
			}
		}
		int num3 = num2 - num;
		int num4 = SumParts(container, list2);
		int num5 = SumParts(container, list);
		int newContaminantSum;
		if (num4 >= num3)
		{
			newContaminantSum = ShrinkProportionally(container, list2, num4, num4 - num3) + num5;
		}
		else
		{
			ShrinkProportionally(container, list2, num4, 0);
			newContaminantSum = ShrinkProportionally(container, list, num5, num5 - (num3 - num4));
		}
		container.ModifyTag(Liquid.GetCurrentPartFromId("water"), (TagState state) =>
		{
			state.Enable().SetInt(total - newContaminantSum);
		});
		WaterFeatureHelper.SetPresumedPurity(container, GetWaterPurity(container));
	}

	public static void OnHydroponicNutrientTabletAdded(GameItem waterContainer)
	{
		if (!waterContainer.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int mineralAdd = 15000;
		int freeCapacity = GetFreeCapacity(waterContainer);
		if (freeCapacity <= 0)
		{
			int currentWater = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
			waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("water"), (TagState state) =>
			{
				state.Enable().SetInt(currentWater - mineralAdd);
			});
			int currentMineral = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("mineral")).GetInt();
			waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("mineral"), (TagState state) =>
			{
				state.Enable().SetInt(currentMineral + mineralAdd);
			});
		}
		else if (freeCapacity < mineralAdd)
		{
			int currentWater2 = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
			int watermod = mineralAdd - freeCapacity;
			waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("water"), (TagState state) =>
			{
				state.Enable().SetInt(currentWater2 - watermod);
			});
			int currentMineral2 = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("mineral")).GetInt();
			waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("mineral"), (TagState state) =>
			{
				state.Enable().SetInt(currentMineral2 + mineralAdd);
			});
		}
		else
		{
			int currentMineral3 = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("mineral")).GetInt();
			waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("mineral"), (TagState state) =>
			{
				state.Enable().SetInt(currentMineral3 + mineralAdd);
			});
		}
		WaterFeatureHelper.UpdateWaterFeature(waterContainer, isExpose: false);
	}

	public static void OnUVUsed(GameItem waterContainer)
	{
		int currentMicrobial = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("microbe")).GetInt();
		int currentWater = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
		waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("microbe"), (TagState state) =>
		{
			state.Enable().SetInt(0);
		});
		waterContainer.ModifyTag(Liquid.GetCurrentPartFromId("water"), (TagState state) =>
		{
			state.Enable().SetInt(currentWater + currentMicrobial);
		});
		WaterFeatureHelper.UpdateWaterFeature(waterContainer, isExpose: false);
	}

	public static int RemoveContaminantFromContainer(GameItem waterContainer, string contaminantId, int volumeToRemove, int minPercentage, int floorBaseVolume = -1)
	{
		if (Liquid.GetLiquidFromId(contaminantId) == null)
		{
			Debug.LogError("Invalid contaminant ID: " + contaminantId);
			return 0;
		}
		if (!Liquid.GetLiquidFromId(contaminantId).canBeRemovedByPurifier)
		{
			return 0;
		}
		int num = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
		if (num == 0)
		{
			return 0;
		}
		int num2 = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId(contaminantId)).GetInt();
		int num3 = (int)((long)((floorBaseVolume >= 0) ? floorBaseVolume : num) * (long)minPercentage / 10000);
		if (num2 <= num3)
		{
			num3 = num2;
		}
		int newContaminantValue = num2 - volumeToRemove;
		if (newContaminantValue < num3)
		{
			newContaminantValue = num3;
		}
		if (newContaminantValue < 0)
		{
			newContaminantValue = 0;
		}
		int result = num2 - newContaminantValue;
		waterContainer.ModifyTag(Liquid.GetCurrentPartFromId(contaminantId), (TagState state) =>
		{
			state.Enable().SetInt(newContaminantValue);
		});
		WaterFeatureHelper.UpdateWaterFeature(waterContainer, isExpose: false);
		return result;
	}

	public static void AddLiquid(GameItem waterContainer, string liquidId, int addAmount)
	{
		if (addAmount <= 0)
		{
			return;
		}
		if (Liquid.GetLiquidFromId(liquidId) == null)
		{
			Debug.LogError("Invalid liquid ID: " + liquidId);
			return;
		}
		int currentAmount = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId(liquidId)).GetInt();
		int num = waterContainer.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
		if ((long)currentAmount + (long)addAmount > num)
		{
			addAmount = num - currentAmount;
		}
		ChemicalHelper.HandleReceiveLiquidTemp(waterContainer, addAmount);
		waterContainer.ModifyTag(Liquid.GetCurrentPartFromId(liquidId), (TagState state) =>
		{
			state.Enable().SetInt(currentAmount + addAmount);
		});
		WaterFeatureHelper.UpdateWaterFeature(waterContainer, isExpose: false);
		SpriteHelper.UpdateWaterContainerSprite(waterContainer);
	}

	public static void AddMaxLiquid(GameItem waterContainer, string liquidId)
	{
		if (Liquid.GetLiquidFromId(liquidId) == null)
		{
			Debug.LogError("Invalid liquid ID: " + liquidId);
			return;
		}
		int currentAmount = waterContainer.GetTagReadonly(Liquid.GetCurrentPartFromId(liquidId)).GetInt();
		int num = waterContainer.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
		int addAmount = num - currentAmount;
		if (addAmount > 0)
		{
			ChemicalHelper.HandleReceiveLiquidTemp(waterContainer, addAmount);
			waterContainer.ModifyTag(Liquid.GetCurrentPartFromId(liquidId), (TagState state) =>
			{
				state.Enable().SetInt(currentAmount + addAmount);
			});
			WaterFeatureHelper.UpdateWaterFeature(waterContainer, isExpose: false);
			SpriteHelper.UpdateWaterContainerSprite(waterContainer);
		}
	}

	public static void AddWater(GameItem waterContainer, int grade, int addAmount = -1, bool useRange = false, int lowPara = 0, int highPara = 1, bool initWaterFeature = true)
	{
		Dictionary<string, float> dictionary;
		switch (grade)
		{
		case 0:
		case 1:
		case 3:
			dictionary = WaterPremadeHelper.waterContaminantDistribution;
			break;
		case 2:
			dictionary = WaterPremadeHelper.baseWaterContaminantDistribution;
			break;
		case 4:
			dictionary = WaterPremadeHelper.rustWaterContaminantDistribution;
			break;
		default:
			dictionary = WaterPremadeHelper.gutterflowContaminantDistribution;
			break;
		}
		Dictionary<string, float> dictionary2 = dictionary;
		if (!waterContainer.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int num = waterContainer.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
		Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
		int num2 = 0;
		foreach (Liquid liquid2 in Liquid.Liquids)
		{
			int num3 = waterContainer.GetTagReadonly(liquid2.currentPart).GetInt();
			dictionary3[liquid2.currentPart] = num3;
			num2 += num3;
		}
		int num4 = num - num2;
		int num5 = ((addAmount == -1) ? num4 : Math.Min(addAmount, num4));
		if (num5 <= 0)
		{
			return;
		}
		var (num6, num7) = grade switch
		{
			0 => (9990, 9999), 
			1 => (9900, 9989), 
			2 => (9800, 9899), 
			3 => (9600, 9799), 
			4 => (9000, 9599), 
			_ => (3500, 8999), 
		};
		if (useRange)
		{
			num6 = lowPara;
			num7 = highPara;
		}
		int num8 = ((!useRange && (grade == 1 || grade == 2 || grade == 3)) ? RollNearLowerEdge(num6, num7) : RNG.GetRandomInt(num6, num7));
		int num9 = (int)(((long)num5 * (long)num8 + 9999) / 10000);
		if (num9 > num5)
		{
			num9 = num5;
		}
		int num10 = num5 - num9;
		Dictionary<string, int> dictionary4 = new Dictionary<string, int>();
		int num11 = 0;
		foreach (KeyValuePair<string, float> item in dictionary2)
		{
			int num12 = (int)((float)num10 * item.Value);
			dictionary4[item.Key] = num12;
			num11 += num12;
		}
		int num13 = num10 - num11;
		if (num13 > 0 && dictionary2.Count > 0)
		{
			dictionary4[dictionary2.Keys.First()] += num13;
		}
		int num14 = num9 + dictionary4.Values.Sum();
		if (num14 > num5)
		{
			int num15 = num14 - num5;
			foreach (string item2 in dictionary4.Keys.ToList())
			{
				int val = dictionary4[item2] - (int)((float)dictionary4[item2] / (float)num10 * (float)num15);
				dictionary4[item2] = Math.Max(val, 0);
			}
		}
		GameItem filterItem = GetFilterItem(waterContainer);
		int num16 = 0;
		if (filterItem != null)
		{
			foreach (string key in dictionary4.Keys.ToList())
			{
				Liquid liquid = Liquid.Liquids.Find((Liquid l) => l.currentPart.Equals(key));
				if (liquid != null)
				{
					int num17 = dictionary4[key];
					dictionary4[key] = HandleFilter(liquid.identifier, num17, null, filterItem);
					num16 += num17 - dictionary4[key];
				}
			}
		}
		dictionary3[Liquid.GetCurrentPartFromId("water")] += num9;
		foreach (KeyValuePair<string, int> item3 in dictionary4)
		{
			if (dictionary3.ContainsKey(item3.Key))
			{
				dictionary3[item3.Key] += item3.Value;
			}
			else
			{
				dictionary3[item3.Key] = item3.Value;
			}
		}
		foreach (KeyValuePair<string, int> part in dictionary3)
		{
			waterContainer.ModifyTag(part.Key, (TagState state) =>
			{
				state.Enable().SetInt(part.Value);
			});
		}
		if (initWaterFeature)
		{
			WaterFeatureHelper.InitWaterFeature(waterContainer);
			if (num16 > 0)
			{
				long num18 = (long)num2 + (long)num5;
				int num19 = ((num2 > 0) ? WaterFeatureHelper.GetWaterFeature(waterContainer).customIntValue1 : 0);
				int presumedPurity = (int)(((long)num2 * (long)num19 + (long)num9 * 10000L) / num18);
				WaterFeatureHelper.SetPresumedPurity(waterContainer, presumedPurity);
			}
		}
	}

	private static int RollNearLowerEdge(int low, int high)
	{
		int num = high - low;
		double num2 = Math.Max(1.0, (double)num * 0.08);
		for (int i = 0; i < 10; i++)
		{
			double d = Math.Max(1E-09, UnityEngine.Random.value);
			double d2 = Math.Max(1E-09, UnityEngine.Random.value);
			int num3 = (int)((0.0 - num2) * (Math.Log(d) + Math.Log(d2)));
			if (num3 <= num)
			{
				return low + num3;
			}
		}
		return low;
	}

	public static void AddWaterOld(GameItem waterContainer, int grade, int addAmount = -1)
	{
		Dictionary<string, float> waterContaminantDistribution = WaterPremadeHelper.waterContaminantDistribution;
		if (!waterContainer.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int num = waterContainer.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
		int num2 = 0;
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (Liquid liquid in Liquid.Liquids)
		{
			int num3 = waterContainer.GetTagReadonly(liquid.currentPart).GetInt();
			dictionary[liquid.currentPart] = num3;
			num2 += num3;
		}
		int num4 = num - num2;
		int num5 = 0;
		num5 = ((addAmount != -1) ? Math.Min(addAmount, num4) : num4);
		int minInclusive = 0;
		int maxInclusive = 0;
		if (grade == 0)
		{
			minInclusive = 9990;
			maxInclusive = 9999;
		}
		if (grade == 1)
		{
			minInclusive = 9900;
			maxInclusive = 9989;
		}
		if (grade == 2)
		{
			minInclusive = 9800;
			maxInclusive = 9899;
		}
		if (grade == 3)
		{
			minInclusive = 9600;
			maxInclusive = 9799;
		}
		else if (grade == 4)
		{
			minInclusive = 9000;
			maxInclusive = 9599;
		}
		else if (grade > 4)
		{
			minInclusive = 3500;
			maxInclusive = 8999;
		}
		int randomInt = RNG.GetRandomInt(minInclusive, maxInclusive);
		int num6 = (int)(((long)num5 * (long)randomInt + 9999) / 10000);
		if (num6 > num5)
		{
			num6 = num5;
		}
		int num7 = num5 - num6;
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		int num8 = 0;
		foreach (KeyValuePair<string, float> item in waterContaminantDistribution)
		{
			string key = item.Key;
			float value = item.Value;
			int num9 = (dictionary2[key] = (int)((float)num7 * value));
			num8 += num9;
		}
		int num11 = num6 + num8;
		if (num11 > num5)
		{
			int num12 = num11 - num5;
			foreach (string item2 in dictionary2.Keys.ToList())
			{
				int num13 = dictionary2[item2];
				int val = num13 - (int)((float)num13 / (float)num8 * (float)num12);
				dictionary2[item2] = Math.Max(val, 0);
			}
		}
		dictionary[Liquid.GetCurrentPartFromId("water")] += num6;
		foreach (KeyValuePair<string, int> item3 in dictionary2)
		{
			if (dictionary.ContainsKey(item3.Key))
			{
				dictionary[item3.Key] += item3.Value;
			}
			else
			{
				dictionary[item3.Key] = item3.Value;
			}
		}
		foreach (KeyValuePair<string, int> part in dictionary)
		{
			waterContainer.ModifyTag(part.Key, (TagState state) =>
			{
				state.Enable().SetInt(part.Value);
			});
		}
		WaterFeatureHelper.InitWaterFeature(waterContainer);
	}

	public static void FillWithPureWater(GameItem waterContainer)
	{
		AddWater(waterContainer, 0);
	}

	public static void AddPureWater(GameItem waterContainer, int addAmount)
	{
		AddWater(waterContainer, 0, addAmount);
	}

	public static void FillWithHighQualityWater(GameItem waterContainer)
	{
		AddWater(waterContainer, 1);
	}

	public static void AddHighQualityWater(GameItem waterContainer, int addAmount)
	{
		AddWater(waterContainer, 1, addAmount);
	}

	public static void FillWithBaseWater(GameItem waterContainer)
	{
		AddWater(waterContainer, 2);
	}

	public static void AddBaseWater(GameItem waterContainer, int addAmount)
	{
		AddWater(waterContainer, 2, addAmount);
	}

	public static void FillWithGhostwater(GameItem waterContainer)
	{
		AddWater(waterContainer, 3);
	}

	public static void FillWithGhostwaterHighEnd(GameItem waterContainer)
	{
		AddWater(waterContainer, 3, -1, useRange: true, 9700, 9799);
	}

	public static void FillWithGhostwaterLowEnd(GameItem waterContainer)
	{
		AddWater(waterContainer, 3, -1, useRange: true, 9600, 9650);
	}

	public static void AddGhostWater(GameItem waterContainer, int addAmount)
	{
		AddWater(waterContainer, 3, addAmount);
	}

	public static void FillWithRustWater(GameItem waterContainer)
	{
		AddWater(waterContainer, 4);
	}

	public static void FillWithRustWaterHighEnd(GameItem waterContainer)
	{
		AddWater(waterContainer, 4, -1, useRange: true, 9550, 9599);
	}

	public static void FillWithRustWaterLowEnd(GameItem waterContainer)
	{
		AddWater(waterContainer, 4, -1, useRange: true, 9000, 9200);
	}

	public static void AddRustWater(GameItem waterContainer, int addAmount)
	{
		AddWater(waterContainer, 4, addAmount);
	}

	public static void FillWithGutterflow(GameItem waterContainer)
	{
		AddWater(waterContainer, 5);
	}

	public static void AddGutterflow(GameItem waterContainer, int addAmount)
	{
		AddWater(waterContainer, 5, addAmount);
	}

	public static void FillWithNutrientSolution(GameItem waterContainer, int fillAmount = -1)
	{
		if (!waterContainer.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int num = waterContainer.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		int num2 = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			int num3 = waterContainer.GetTagReadonly(liquid.currentPart).GetInt();
			dictionary[liquid.currentPart] = num3;
			num2 += num3;
		}
		int num4 = num - num2;
		int num5 = ((fillAmount == -1) ? num4 : Math.Min(fillAmount, num4));
		if (num5 <= 0)
		{
			return;
		}
		int num6 = num5 / 4;
		int num7 = num5 - num6;
		dictionary[Liquid.GetCurrentPartFromId("water")] += num6;
		dictionary[Liquid.GetCurrentPartFromId("mineral")] = (dictionary.ContainsKey(Liquid.GetCurrentPartFromId("mineral")) ? (dictionary[Liquid.GetCurrentPartFromId("mineral")] + num7) : num7);
		foreach (KeyValuePair<string, int> part in dictionary)
		{
			waterContainer.ModifyTag(part.Key, (TagState state) =>
			{
				state.Enable().SetInt(part.Value);
			});
		}
		WaterFeatureHelper.InitWaterFeature(waterContainer);
	}

	public static bool CanPourVolume(GameItem container, int volumeMl)
	{
		if (container.IsTag("LIQUID_CONTAINER_TAG") && CanUseWaterContainer(container))
		{
			return GetFreeCapacity(container) >= (long)volumeMl * 1000L;
		}
		return false;
	}

	public static void PourSpirit(GameItem container, int volumeMl, int alcoholPercent)
	{
		if (CanPourVolume(container, volumeMl))
		{
			int num = volumeMl * 1000;
			int num2 = (int)((long)num * (long)alcoholPercent / 100);
			AddLiquid(container, "alcohol", num2);
			AddWater(container, 2, num - num2);
		}
	}

	public static bool CanPourNudka(GameItem container)
	{
		return CanPourVolume(container, 750);
	}

	public static void PourNudka(GameItem container)
	{
		PourSpirit(container, 750, 45);
	}

	public static void SpawnWaterTestingKit()
	{
		GameGridInventory invElement = EmporiumEntry.Instance.invElement;
		GameItem gameItem = DirectoryMaster.Item("large_bottled_water");
		FillWithPureWater(gameItem);
		GraphUtils.TryAcceptAll(invElement, gameItem);
		GameItem gameItem2 = DirectoryMaster.Item("large_bottled_water");
		FillWithHighQualityWater(gameItem2);
		GraphUtils.TryAcceptAll(invElement, gameItem2);
		GameItem gameItem3 = DirectoryMaster.Item("large_bottled_water");
		FillWithBaseWater(gameItem3);
		GraphUtils.TryAcceptAll(invElement, gameItem3);
		GameItem gameItem4 = DirectoryMaster.Item("large_bottled_water");
		FillWithGhostwater(gameItem4);
		GraphUtils.TryAcceptAll(invElement, gameItem4);
		GameItem gameItem5 = DirectoryMaster.Item("large_bottled_water");
		FillWithGhostwater(gameItem5);
		GraphUtils.TryAcceptAll(invElement, gameItem5);
		GameItem gameItem6 = DirectoryMaster.Item("large_bottled_water");
		FillWithRustWaterHighEnd(gameItem6);
		GraphUtils.TryAcceptAll(invElement, gameItem6);
		GameItem gameItem7 = DirectoryMaster.Item("large_bottled_water");
		FillWithRustWaterLowEnd(gameItem7);
		GraphUtils.TryAcceptAll(invElement, gameItem7);
		GameItem gameItem8 = DirectoryMaster.Item("large_bottled_water");
		FillWithGutterflow(gameItem8);
		GraphUtils.TryAcceptAll(invElement, gameItem8);
		GraphUtils.TryAcceptAll(invElement, PreBuiltItemHelper.FullWaterPurificationTabletBox());
		GraphUtils.TryAcceptAll(invElement, PreBuiltItemHelper.FullWaterTestStripBox());
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("water_guide"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("small_bottled_water"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("large_bottled_water"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("water_jug"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("water_filter"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("water_pitcher"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("aquascan"));
		GraphUtils.TryAcceptAll(invElement, PreBuiltItemHelper.CreateWaterPurifier());
		GraphUtils.TryAcceptAll(invElement, PreBuiltItemHelper.CreateMoistureFarm());
		GraphUtils.TryAcceptAll(invElement, PreBuiltItemHelper.EnergyCreditExt());
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("bottle_printer"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("uv_filter"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("heating_plate"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("uv_filter"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("bottled_water"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("bottled_water"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("bottled_water"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("water_filter_adv"));
		GraphUtils.TryAcceptAll(invElement, DirectoryMaster.Item("portable_water_purifier"));
	}

	public static bool IsDrain(GameItem gameItem)
	{
		return gameItem.identifier.Equals("drain");
	}

	public static bool IsFaucet(GameItem gameItem)
	{
		return gameItem.identifier.Equals("faucet");
	}

	public static int GetContaminantCount()
	{
		return Liquid.Liquids.Count - 1;
	}

	public static int GetContaminantAmount(GameItem gameItem, string liquidID)
	{
		return gameItem.GetTagReadonly(Liquid.GetCurrentPartFromId(liquidID)).GetInt();
	}

	public static int GetContaminantPercentage(GameItem gameItem, string liquidID)
	{
		int num = gameItem.GetTagReadonly(Liquid.GetCurrentPartFromId("water")).GetInt();
		int num2 = gameItem.GetTagReadonly(Liquid.GetCurrentPartFromId(liquidID)).GetInt();
		if (num2 <= 0)
		{
			return 0;
		}
		if (num <= 0)
		{
			return 1000000;
		}
		return (int)Math.Min(1000000L, (long)num2 * 10000L / num);
	}

	public static string GetReactionString(string display)
	{
		if (display.Equals(WaterFeatureHelper.GUTTERFLOW_WATER_DISPLAY))
		{
			return LocHelper.GetLocalizedDialogue("dialog_water_refuse_gutterflow");
		}
		if (display.Equals(WaterFeatureHelper.RUST_WATER_DISPLAY))
		{
			return LocHelper.GetLocalizedDialogue("dialog_water_refuse_rust");
		}
		if (display.Equals(WaterFeatureHelper.GHOSTWATER_DISPLAY))
		{
			return LocHelper.GetLocalizedDialogue("dialog_water_refuse_ghostwater");
		}
		return LocHelper.GetLocalizedDialogue("dialog_water_refuse_default");
	}

	public static bool WillAcceptWater(GameItem targetItem, List<string> acceptedDisplays)
	{
		if (!targetItem.IsTag("WATER"))
		{
			return false;
		}
		string currentWaterDisplay = WaterFeatureHelper.GetCurrentWaterDisplay(targetItem);
		return acceptedDisplays.Contains(currentWaterDisplay);
	}

	public static void RemoveMl(GameItem container, int volumeInMl)
	{
		if (volumeInMl <= 0)
		{
			return;
		}
		int num = (int)Math.Min(2147483647L, (long)volumeInMl * 1000L);
		int num2 = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num2 += container.GetTagReadonly(liquid.currentPart).GetInt();
		}
		if (num2 <= num)
		{
			foreach (Liquid liquid2 in Liquid.Liquids)
			{
				container.ModifyTag(liquid2.currentPart, (TagState state) =>
				{
					state.Enable().SetInt(0);
				});
			}
			WaterFeatureHelper.OnLiquidContainerPourOutOf(container);
			return;
		}
		int[] liquidParts = GetLiquidParts(container);
		int[] array = SplitProportionally(liquidParts, num);
		for (int num3 = 0; num3 < Liquid.Liquids.Count; num3++)
		{
			int newAmount = liquidParts[num3] - array[num3];
			container.ModifyTag(Liquid.Liquids[num3].currentPart, (TagState state) =>
			{
				state.Enable().SetInt(newAmount);
			});
		}
		WaterFeatureHelper.OnLiquidContainerPourOutOf(container);
	}

	public static void Remove(GameItem container, int volumeInPart)
	{
		if (volumeInPart <= 0)
		{
			return;
		}
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num += container.GetTagReadonly(liquid.currentPart).GetInt();
		}
		if (num <= volumeInPart)
		{
			foreach (Liquid liquid2 in Liquid.Liquids)
			{
				container.ModifyTag(liquid2.currentPart, (TagState state) =>
				{
					state.Enable().SetInt(0);
				}, skipValidate: true);
			}
			WaterFeatureHelper.OnLiquidContainerPourOutOf(container);
			return;
		}
		int[] liquidParts = GetLiquidParts(container);
		int[] array = SplitProportionally(liquidParts, volumeInPart);
		for (int num2 = 0; num2 < Liquid.Liquids.Count; num2++)
		{
			int newAmount = liquidParts[num2] - array[num2];
			container.ModifyTag(Liquid.Liquids[num2].currentPart, (TagState state) =>
			{
				state.Enable().SetInt(newAmount);
			}, skipValidate: true);
		}
		WaterFeatureHelper.OnLiquidContainerPourOutOf(container);
	}

	public static List<KeyValuePair<string, int>> RemoveAndReturn(GameItem container, int volumeInPart)
	{
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
		if (volumeInPart <= 0)
		{
			return list;
		}
		int num = 0;
		foreach (Liquid liquid2 in Liquid.Liquids)
		{
			num += container.GetTagReadonly(liquid2.currentPart).GetInt();
		}
		if (num <= volumeInPart)
		{
			foreach (Liquid liquid3 in Liquid.Liquids)
			{
				int num2 = container.GetTagReadonly(liquid3.currentPart).GetInt();
				if (num2 > 0)
				{
					list.Add(new KeyValuePair<string, int>(liquid3.identifier, num2));
				}
				container.ModifyTag(liquid3.currentPart, (TagState state) =>
				{
					state.Enable().SetInt(0);
				});
			}
			WaterFeatureHelper.OnLiquidContainerPourOutOf(container);
			return list;
		}
		int[] liquidParts = GetLiquidParts(container);
		int[] array = SplitProportionally(liquidParts, volumeInPart);
		for (int num3 = 0; num3 < Liquid.Liquids.Count; num3++)
		{
			Liquid liquid = Liquid.Liquids[num3];
			int num4 = array[num3];
			if (num4 > 0)
			{
				list.Add(new KeyValuePair<string, int>(liquid.identifier, num4));
			}
			int newAmount = liquidParts[num3] - num4;
			container.ModifyTag(liquid.currentPart, (TagState state) =>
			{
				state.Enable().SetInt(newAmount);
			});
		}
		WaterFeatureHelper.OnLiquidContainerPourOutOf(container);
		return list;
	}

	public static bool IsFull(GameItem container)
	{
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			num += container.GetTagReadonly(liquid.currentPart).GetInt();
		}
		int num2 = container.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt();
		return num >= num2;
	}

	public static bool IsWet(GameItem container)
	{
		int num = 0;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			if (liquid.isEvaporative)
			{
				num += container.GetTagReadonly(liquid.currentPart).GetInt();
			}
		}
		return num > 100;
	}

	public static void SetLastScanResult(GameItem gameItem)
	{
		if (gameItem == null || !gameItem.IsTag("LIQUID_CONTAINER_TAG"))
		{
			return;
		}
		int waterPurity = GetWaterPurity(gameItem);
		string scanText = string.Format("[{0}]: {1:F2}% ", LocHelper.GetLocalizedMechanic("mech_last_scan"), (double)waterPurity / 100.0) + WaterFeatureHelper.GetWaterDisplayName(waterPurity);
		TagState tagReadonly = gameItem.GetTagReadonly("LAST_SCAN_RESULT_TAG");
		if (!tagReadonly.IsEnabled() || !scanText.Equals(tagReadonly.GetString()))
		{
			gameItem.ModifyTag("LAST_SCAN_RESULT_TAG", (TagState state) =>
			{
				state.Enable().SetString(scanText);
			}, skipValidate: true);
			gameItem.SyncModifiedState();
		}
	}

	public static void ResetLastScanResult(GameItem gameItem)
	{
		gameItem?.ModifyTag("LAST_SCAN_RESULT_TAG", (TagState state) =>
		{
			state.Disable().SetString("");
		});
	}

	public static string GetLiquidContainerColor(GameItem gameItem)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		foreach (Liquid liquid in Liquid.Liquids)
		{
			int num5 = gameItem.GetTagReadonly(liquid.currentPart).GetInt();
			if (num5 > 0 && ColorUtility.TryParseHtmlString("#" + liquid.color, out var color))
			{
				float num6 = (float)num5 * (float)Mathf.Max(1, liquid.colorMultiplier);
				num2 += color.r * num6;
				num3 += color.g * num6;
				num4 += color.b * num6;
				num += num6;
			}
		}
		if (num <= 0f)
		{
			return "000000";
		}
		num2 /= num;
		num3 /= num;
		num4 /= num;
		return MetaHelper.ColorToHex(new Color(num2, num3, num4));
	}
}
