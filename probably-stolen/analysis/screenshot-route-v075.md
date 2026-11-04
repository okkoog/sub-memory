# Screenshot route trace v0.7.5

## storage

### ItemStorage/AutoStow.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Diagnostics;
00004: using System.Text;
00005: using UnityEngine;
00006: 
00007: namespace ItemStorage;
00008: 
00009: internal static class AutoStow
00010: {
00011: 	internal static void Run(GameItem pack, string context)
00012: 	{
00013: 		if (pack == null || !BoxItem.IsBox(pack))
00014: 		{
00015: 			return;
00016: 		}
00017: 		try
00018: 		{
00019: 			Core.Debug("收纳箱[" + context + "] 目标箱 uid=" + pack.uniqueId + "，父链=" + ((pack.parentInventory != null) ? "在容器内" : "【孤儿-幽灵箱】") + "。");
00020: 		}
00021: 		catch
00022: 		{
00023: 		}
00024: 		if (!BoxTransfer.IsInStore())
00025: 		{
00026: 			Core.Debug("收纳箱[" + context + "] 一键收纳：玩家不在店内，忽略。");
00027: 			BoxItem.NotifyUser("请回到店内再收纳");
00028: 			return;
00029: 		}
00030: 		Stopwatch stopwatch = Stopwatch.StartNew();
00031: 		Dictionary<string, int> dictionary = ReadSpace(pack, context);
00032: 		if (dictionary == null)
00033: 		{
00034: 			return;
00035: 		}
00036: 		if (dictionary.Count == 0)
00037: 		{
00038: 			Core.Debug("收纳箱[" + context + "] 一键收纳：盒子里没有物品，不工作。");
00039: 			BoxItem.NotifyUser("没有可收纳的物品");
00040: 			return;
00041: 		}
00042: 		List<GameItem> list = CollectCandidates(new HashSet<string>(dictionary.Keys, StringComparer.Ordinal), context);
00043: 		int num = 0;
00044: 		int num2 = 0;
00045: 		int num3 = 0;
00046: 		int num4 = 0;
00047: 		foreach (GameItem item in list)
00048: 		{
00049: 			if (MoveOneInto(pack, item, dictionary, context, out var movedUnits, out var nakedKilled))
00050: 			{
00051: 				num++;
00052: 				num2 += movedUnits;
00053: 				if (nakedKilled)
00054: 				{
00055: 					num4++;
00056: 				}
00057: 			}
00058: 			else
00059: 			{
00060: 				num3++;
00061: 			}
00062: 		}
00063: 		int num5 = 0;
00064: 		try
00065: 		{
00066: 			if (StorageGrid.GetBoxOption(pack, "PULL") == 1)
00067: 			{
00068: 				num5 = GatherFromBoxes(pack, context);
00069: 			}
00070: 		}
00071: 		catch (Exception ex)
00072: 		{
00073: 			Core.Log?.Warning("收纳箱[" + context + "] 箱间联动异常：" + ex.Message);
00074: 		}
00075: 		Core.Debug("收纳箱[" + context + "] 一键收纳耗时 " + stopwatch.ElapsedMilliseconds + " ms（收 " + num + " 叠 / 候选 " + list.Count + " 件）。");
00076: 		if (num > 0 || num5 > 0)
00077: 		{
00078: 			try
00079: 			{
00080: 				BoxWindow.RefreshButtons(pack);
00081: 			}
00082: 			catch
00083: 			{
00084: 			}
00085: 			try
00086: 			{
00087: 				EmporiumEntry instance = EmporiumEntry.Instance;
00088: 				if (instance != null)
00089: 				{
00090: 					instance.Validate(true);
00091: 				}
00092: 			}
00093: 			catch
00094: 			{
00095: 			}
00096: 			Core.Log?.Msg("收纳箱[" + context + "] 一键收纳完成：收进 " + num + " 叠共 " + num2 + " 件" + ((num5 > 0) ? ("，箱间归拢 " + num5 + " 格") : "") + ((num3 > 0) ? ("，" + num3 + " 叠放不下或不可收，跳过") : "") + ((num4 > 0) ? ("，" + num4 + " 件销毁走裸销毁兜底") : "") + "。");
00097: 		}
00098: 		else
00099: 		{
00100: 			Core.Log?.Msg("收纳箱[" + context + "] 一键收纳：店里没有能收进来的物品" + ((num3 > 0) ? ("（" + num3 + " 叠空间不足或不可收，跳过）") : "") + ((num5 > 0) ? ("；从其他收纳箱归拢 " + num5 + " 格") : "") + "。");
00101: 		}
00102: 		if (num2 > 0)
00103: 		{
00104: 			BoxItem.NotifyUser("已收进 " + num2 + " 件");
00105: 		}
00106: 		else if (num5 <= 0)
00107: 		{
00108: 			BoxItem.NotifyUser("没有可收纳的物品");
00109: 		}
00110: 	}
00111: 
00112: 	internal static Dictionary<string, int>? ReadSpace(GameItem pack, string context)
00113: 	{
00114: 		Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
00115: 		try
00116: 		{
00117: 			for (int i = 0; i < 16; i++)
00118: 			{
00119: 				string itemId = StorageGrid.GetItemId(pack, i);
00120: 				if (NullGuard.HasText(itemId))
00121: 				{
00122: 					int num = 99 - StorageGrid.GetCount(pack, i);
00123: 					if (num > 0)
00124: 					{
00125: 						dictionary.TryGetValue(itemId, out var value);
00126: 						dictionary[itemId] = value + num;
00127: 					}
00128: 				}
00129: 			}
00130: 			return dictionary;
00131: 		}
00132: 		catch (Exception ex)
00133: 		{
00134: 			Core.Log?.Warning("收纳箱[" + context + "] 一键收纳读槽失败：" + ex.Message);
00135: 			return null;
00136: 		}
00137: 	}
00138: 
00139: 	internal static List<GameItem> CollectCandidates(HashSet<string> interest, string context)
00140: 	{
00141: 		List<GameItem> list = new List<GameItem>();
00142: 		try
00143: 		{
00144: 			EmporiumEntry instance = EmporiumEntry.Instance;
00145: 			if ((Object)(object)instance != (Object)null)
00146: 			{
00147: 				Collect(instance.invElement, "桌面", list);
00148: 				Collect(instance.frontInvinvElement, "柜台正面", list);
00149: 				Collect(instance.backInvinvElement, "柜台里侧", list);
00150: 				Collect(instance.backInvinvElementCounter, "柜台里侧B", list);
00151: 			}
00152: 		}
00153: 		catch (Exception ex)
00154: 		{
00155: 			Core.Log?.Warning("收纳箱[" + context + "] 一键收纳扫描店铺失败：" + ex.Message);
00156: 		}
00157: 		HashSet<GameItem> hashSet = new HashSet<GameItem>();
00158: 		List<GameItem> list2 = new List<GameItem>();
00159: 		foreach (GameItem item in list)
00160: 		{
00161: 			try
00162: 			{
00163: 				if (!hashSet.Add(item) || BoxItem.IsBox(item))
00164: 				{
00165: 					continue;
00166: 				}
00167: 				string text = Core.Clean(item.identifier) ?? "?";
00168: 				try
00169: 				{
00170: 					if (!GeneralHelper.IsItemOwned(item))
00171: 					{
00172: 						Core.Debug("收纳箱[" + context + "] 跳过（非玩家所有，疑似上柜/商人货）：" + text);
00173: 						continue;
00174: 					}
00175: 				}
00176: 				catch
00177: 				{
00178: 					Core.Debug("收纳箱[" + context + "] 跳过（归属读取失败）：" + text);
00179: 					goto end_IL_00a8;
00180: 				}
00181: 				if (string.IsNullOrEmpty(text) || !interest.Contains(text))
00182: 				{
00183: 					Core.Debug("收纳箱[" + context + "] 跳过（盒内无此种类，非收纳目标）：" + text);
00184: 					continue;
00185: 				}
00186: 				if (IsFullWaterContainer(item))
00187: 				{
00188: 					Core.Debug("收纳箱[" + context + "] 跳过（含水水容器不一键收纳，防混水污染）：" + text);
00189: 					continue;
00190: 				}
00191: 				try
00192: 				{
00193: 					if (item.IsTag("ANIMAL_SPECIES_TAG") || item.IsTag("ANIMAL_AGE_TAG"))
00194: 					{
00195: 						Core.Debug("收纳箱[" + context + "] 跳过（活物不一键收纳，玩家可手动拖入）：" + text);
00196: 						continue;
00197: 					}
00198: 				}
00199: 				catch
00200: 				{
00201: 				}
00202: 				if (BoxTransfer.HasContentInside(item))
00203: 				{
00204: 					Core.Debug("收纳箱[" + context + "] 跳过（容器内有物品，拒收防内容丢失）：" + text);
00205: 					continue;
00206: 				}
00207: 				try
00208: 				{
00209: 					if (BoxItem.IsModuleItem(item) == true)
00210: 					{
00211: 						Core.Debug("收纳箱[" + context + "] 跳过（【模组】标签物品，不一键收纳）：" + text);
00212: 						continue;
00213: 					}
00214: 					if (BoxItem.IsSupplyCrateItem(item) != true)
00215: 					{
00216: 						goto IL_0268;
00217: 					}
00218: 					bool flag = true;
00219: 					try
00220: 					{
00221: 						flag = item.IsGameItemType("SUPPLY_CRATE");
00222: 					}
00223: 					catch
00224: 					{
00225: 						flag = true;
00226: 					}
00227: 					if (flag)
00228: 					{
00229: 						Core.Debug("收纳箱[" + context + "] 跳过（锁定物资箱，不一键收纳）：" + text);
00230: 						continue;
00231: 					}
00232: 					Core.Debug("收纳箱[" + context + "] 刷开物资箱参与收纳：" + text);
00233: 					goto IL_0268;
00234: 					IL_0268:
00235: 					if (BoxItem.IsNodeItem(item) == true)
00236: 					{
00237: 						Core.Debug("收纳箱[" + context + "] 跳过（【节点】标签物品，不一键收纳）：" + text);
00238: 						continue;
00239: 					}
00240: 				}
00241: 				catch
00242: 				{
00243: 				}
00244: 				try
00245: 				{
00246: 					if (StolenHelper.IsStolenItem(item))
00247: 					{
00248: 						Core.Debug("收纳箱[" + context + "] 跳过（赃物，不一键收纳）：" + text);
00249: 						continue;
00250: 					}
00251: 				}
00252: 				catch
00253: 				{
00254: 				}
00255: 				try
00256: 				{
00257: 					if (WineHelper.IsFermentingOrFinished(item))
00258: 					{
00259: 						Core.Debug("收纳箱[" + context + "] 跳过（酿造中/陈酿的酒瓶，不一键收纳）：" + text);
00260: 						continue;
00261: 					}
00262: 				}
00263: 				catch
00264: 				{
00265: 				}
00266: 				list2.Add(item);
00267: 				end_IL_00a8:;
00268: 			}
00269: 			catch
00270: 			{
00271: 			}
00272: 		}
00273: 		return list2;
00274: 	}
00275: 
00276: 	internal static bool MoveOneInto(GameItem pack, GameItem it, Dictionary<string, int> space, string context, out int movedUnits, out bool nakedKilled)
00277: 	{
00278: 		movedUnits = 0;
00279: 		nakedKilled = false;
00280: 		try
00281: 		{
00282: 			string text = Core.Clean(it.identifier) ?? string.Empty;
00283: 			int num;
00284: 			try
00285: 			{
00286: 				num = Math.Max(1, it.unitCount);
00287: 			}
00288: 			catch
00289: 			{
00290: 				num = 1;
00291: 			}
00292: 			space.TryGetValue(text, out var value);
00293: 			if (num > value)
00294: 			{
00295: 				Core.Debug("收纳箱[" + context + "] 跳过（一叠 " + num + " 件超过剩余空间 " + value + "）：" + text);
00296: 				return false;
00297: 			}
00298: 			List<GraphNodeStorage> list = null;
00299: 			try
00300: 			{
00301: 				list = it.parents;
00302: 			}
00303: 			catch
00304: 			{
00305: 				list = null;
00306: 			}
00307: 			if (list == null || list.Count == 0)
00308: 			{
00309: 				Core.Debug("收纳箱[" + context + "] 跳过（无网格父节点，物品悬空/拖拽中）：" + text);
00310: 				return false;
00311: 			}
00312: 			GameGridInventory val = null;
00313: 			try
00314: 			{
00315: 				GameInventory parentInventory = it.parentInventory;
00316: 				val = (GameGridInventory)(object)((parentInventory is GameGridInventory) ? parentInventory : null);
00317: 			}
00318: 			catch
00319: 			{
00320: 				val = null;
00321: 			}
00322: 			bool flag = false;
00323: 			if (val != null)
00324: 			{
00325: 				try
00326: 				{
00327: 					flag = ((GameInventory)val).Expel(it);
00328: 				}
00329: 				catch (Exception ex)
00330: 				{
00331: 					Core.Log?.Warning("收纳箱[" + context + "] Expel 失败（回退解绑姿势）：" + ex.Message);
00332: 				}
00333: 			}
00334: 			if (!flag && !DetachFromGrid(it, list, context))
00335: 			{
00336: 				Core.Debug("收纳箱[" + context + "] 跳过（网格父节点移除不完整，物品原样保留）：" + text);
00337: 				return false;
00338: 			}
00339: 			if (!BoxTransfer.TryDeposit(pack, it, 0, -1, consume: false, refresh: false, notify: false))
00340: 			{
00341: 				if (val != null)
00342: 				{
00343: 					try
00344: 					{
00345: 						GraphUtils.TryAcceptAllMid(val, it, -1);
00346: 					}
00347: 					catch
00348: 					{
00349: 					}
00350: 				}
00351: 				bool flag2 = false;
00352: 				try
00353: 				{
00354: 					flag2 = it.parentInventory != null;
00355: 				}
00356: 				catch
00357: 				{
00358: 					flag2 = false;
00359: 				}
00360: 				if (!flag2)
00361: 				{
00362: 					bool flag3 = false;
00363: 					try
00364: 					{
00365: 						flag3 = GraphUtils.DestroyIfUnplaced(it, "ItemStorage_AutoStow");
00366: 					}
00367: 					catch
00368: 					{
00369: 						flag3 = false;
00370: 					}
00371: 					if (!flag3)
00372: 					{
00373: 						try
00374: 						{
00375: 							it.Destroy();
00376: 						}
00377: 						catch
00378: 						{
00379: 							try
00380: 							{
00381: 								it.unitCount = 0;
00382: 							}
00383: 							catch
00384: 							{
00385: 							}
00386: 						}
00387: 					}
00388: 				}
00389: 				Core.Log?.Warning("收纳箱[" + context + "] 一键收纳：'" + text + "' 存入失败，物品已" + (flag2 ? "放回原网格" : "销毁防悬空"));
00390: 				return false;
00391: 			}
00392: 			bool flag4 = false;
00393: 			try
00394: 			{
00395: 				flag4 = GraphUtils.DestroyIfUnplaced(it, "ItemStorage_AutoStow");
00396: 			}
00397: 			catch
00398: 			{
00399: 				flag4 = false;
00400: 			}
00401: 			if (!flag4)
00402: 			{
00403: 				try
00404: 				{
00405: 					List<GraphNodeStorage> parents = it.parents;
00406: 					if (parents != null)
00407: 					{
00408: 						foreach (GraphNodeStorage item in parents)
00409: 						{
00410: 							if (item != null)
00411: 							{
00412: 								try
00413: 								{
00414: 									it.RemoveParent(item);
00415: 								}
00416: 								catch
00417: 								{
00418: 								}
00419: 							}
00420: 						}
00421: 					}
00422: 				}
00423: 				catch
00424: 				{
00425: 				}
00426: 				try
00427: 				{
00428: 					it.Destroy();
00429: 					nakedKilled = true;
00430: 				}
00431: 				catch
00432: 				{
00433: 					try
00434: 					{
00435: 						it.unitCount = 0;
00436: 					}
00437: 					catch
00438: 					{
00439: 					}
00440: 				}
00441: 				Core.Debug("收纳箱[" + context + "] 一键收纳：'" + text + "' 官方销毁未生效，已裸销毁（占位已由 Expel 释放）。");
00442: 			}
00443: 			space[text] = value - num;
00444: 			movedUnits = num;
00445: 			return true;
00446: 		}
00447: 		catch (Exception ex2)
00448: 		{
00449: 			Core.Log?.Warning("收纳箱[" + context + "] 一键收纳单件失败：" + ex2.Message);
00450: 			return false;
00451: 		}
00452: 	}
00453: 
00454: 	private static int GatherFromBoxes(GameItem pack, string context)
00455: 	{
00456: 		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
00457: 		for (int i = 0; i < 16; i++)
00458: 		{
00459: 			string text = VariantKeyOf(pack, i);
00460: 			if (text != null)
00461: 			{
00462: 				hashSet.Add(text);
00463: 			}
00464: 		}
00465: 		if (hashSet.Count == 0)
00466: 		{
00467: 			return 0;
00468: 		}
00469: 		List<int> list = StorageGrid.EmptySlots(pack);
00470: 		int num = 0;
00471: 		foreach (GameItem item in BoxItem.FindAllBoxes())
00472: 		{
00473: 			if (item == null || item == pack)
00474: 			{
00475: 				continue;
00476: 			}
00477: 			try
00478: 			{
00479: 				if (!BoxItem.IsUsableByPlayer(item) || StorageGrid.GetBoxOption(item, "GUARD") == 1)
00480: 				{
00481: 					continue;
00482: 				}
00483: 				for (int j = 0; j < 16; j++)
00484: 				{
00485: 					if (SlotLock.IsLocked(item, j))
00486: 					{
00487: 						continue;
00488: 					}
00489: 					string text2 = VariantKeyOf(item, j);
00490: 					if (text2 == null || !hashSet.Contains(text2))
00491: 					{
00492: 						continue;
00493: 					}
00494: 					if (StorageGrid.IsWaterSlot(item, j))
00495: 					{
00496: 						bool flag = true;
00497: 						try
00498: 						{
00499: 							flag = StorageGrid.GetStoredWaterML(item, j) > 0;
00500: 						}
00501: 						catch
00502: 						{
00503: 						}
00504: 						if (flag)
00505: 						{
00506: 							continue;
00507: 						}
00508: 					}
00509: 					if (TagSnapshot.PeekLastAnimal(item, j).HasValue)
00510: 					{
00511: 						continue;
00512: 					}
00513: 					int num2 = StorageGrid.FindMergeTarget(pack, item, j);
00514: 					if (num2 >= 0 && StorageGrid.MergeSlots(item, j, pack, num2))
00515: 					{
00516: 						num++;
00517: 						try
00518: 						{
00519: 							BoxWindow.RefreshButtons(item);
00520: 						}
00521: 						catch
00522: 						{
00523: 						}
00524: 						Core.Debug("收纳箱[" + context + "] 箱间联动：'" + text2 + "' 并入本箱已有叠（源格 " + j + " → 本箱格 " + num2 + "）。");
00525: 					}
00526: 					else
00527: 					{
00528: 						if (list.Count == 0)
00529: 						{
00530: 							continue;
00531: 						}
00532: 						int num3 = list[list.Count - 1];
00533: 						list.RemoveAt(list.Count - 1);
00534: 						if (!StorageGrid.MoveSlotTo(item, j, pack, num3))
00535: 						{
00536: 							list.Add(num3);
00537: 							continue;
00538: 						}
00539: 						num++;
00540: 						try
00541: 						{
00542: 							BoxWindow.RefreshButtons(item);
00543: 						}
00544: 						catch
00545: 						{
00546: 						}
00547: 						Core.Debug("收纳箱[" + context + "] 箱间联动：'" + text2 + "' 整格自其他箱归拢（源格 " + j + " → 本箱格 " + num3 + "）。");
00548: 					}
00549: 				}
00550: 			}
00551: 			catch
00552: 			{
00553: 			}
00554: 		}
00555: 		return num;
00556: 	}
00557: 
00558: 	private static string? VariantKeyOf(GameItem pack, int slot)
00559: 	{
00560: 		string itemId = StorageGrid.GetItemId(pack, slot);
00561: 		if (string.IsNullOrEmpty(itemId))
00562: 		{
00563: 			return null;
00564: 		}
00565: 		bool? slotDeadFlag = StorageGrid.GetSlotDeadFlag(pack, slot);
00566: 		return itemId + "\u0002" + StorageGrid.GetSlotSignature(pack, slot) + "\u0002" + (slotDeadFlag?.ToString() ?? "-");
00567: 	}
00568: 
00569: 	private static void Collect(GameGridInventory? grid, string label, List<GameItem> into)
00570: 	{
00571: 		if (grid == null)
00572: 		{
00573: 			Core.Debug("收纳箱一键收纳扫描：" + label + " 网格为空（未初始化）。");
00574: 			return;
00575: 		}
00576: 		try
00577: 		{
00578: 			int num = 0;
00579: 			StringBuilder stringBuilder = new StringBuilder(128);
00580: 			foreach (GameItem childItem in ((GameInventory)grid).childItems)
00581: 			{
00582: 				if (childItem != null)
00583: 				{
00584: 					into.Add(childItem);
00585: 					num++;
00586: 					stringBuilder.Append(' ').Append(Core.Clean(childItem.identifier));
00587: 				}
00588: 			}
00589: 			Core.Debug("收纳箱一键收纳扫描：" + label + " 共 " + num + " 件：" + ((num > 0) ? stringBuilder.ToString() : "无"));
00590: 		}
00591: 		catch (Exception ex)
00592: 		{
00593: 			Core.Log?.Warning("收纳箱：枚举" + label + "物品失败：" + ex.Message);
00594: 		}
00595: 	}
00596: 
00597: 	private static bool DetachFromGrid(GameItem it, List<GraphNodeStorage> parents, string context)
00598: 	{
00599: 		try
00600: 		{
00601: 			int num = 0;
00602: 			foreach (GraphNodeStorage parent in parents)
00603: 			{
00604: 				if (parent == null)
00605: 				{
00606: 					num++;
00607: 					continue;
00608: 				}
00609: 				try
00610: 				{
00611: 					num += (it.RemoveParent(parent) ? 1 : 0);
00612: 				}
00613: 				catch
00614: 				{
00615: 					num = num;
00616: 				}
00617: 			}
00618: 			if (num < parents.Count)
00619: 			{
00620: 				Core.Log?.Warning("收纳箱[" + context + "] 一键收纳：物品父节点移除不完整（" + num + "/" + parents.Count + "）。");
00621: 				return false;
00622: 			}
00623: 			return true;
00624: 		}
00625: 		catch (Exception ex)
00626: 		{
00627: 			Core.Log?.Warning("收纳箱[" + context + "] 一键收纳脱离网格失败：" + ex.Message);
00628: 			return false;
00629: 		}
00630: 	}
00631: 
00632: 	private static bool IsFullWaterContainer(GameItem it)
00633: 	{
00634: 		try
00635: 		{
00636: 			bool flag = false;
00637: 			try
00638: 			{
00639: 				flag = WaterFeatureHelper.GetWaterFeature(it) != null;
00640: 			}
00641: 			catch
00642: 			{
00643: 			}
00644: 			if (!flag)
00645: 			{
00646: 				return false;
00647: 			}
00648: 			bool flag2 = true;
00649: 			try
00650: 			{
00651: 				flag2 = WaterHelper.IsEmpty(it);
00652: 			}
00653: 			catch
00654: 			{
00655: 			}
00656: 			if (!flag2)
00657: 			{
00658: 				return true;
00659: 			}
00660: 			try
00661: 			{
00662: 				if (WaterHelper.GetCurrentCapacityML(it) > 0)
00663: 				{
00664: 					return true;
00665: 				}
00666: 			}
00667: 			catch
00668: 			{
00669: 			}
00670: 			return false;
00671: 		}
00672: 		catch
00673: 		{
00674: 			return false;
00675: 		}
00676: 	}
00677: }
~~~

### ItemStorage/BoxContainerDirectoryPatch.cs

~~~csharp
00001: using System;
00002: using HarmonyLib;
00003: 
00004: namespace ItemStorage;
00005: 
00006: [HarmonyPatch(typeof(ContainerItemDirectory), "InitDirectory")]
00007: internal static class BoxContainerDirectoryPatch
00008: {
00009: 	[HarmonyPriority(0)]
00010: 	private static void Postfix(ContainerItemDirectory __instance)
00011: 	{
00012: 		try
00013: 		{
00014: 			BoxItem.Register(__instance);
00015: 		}
00016: 		catch (Exception ex)
00017: 		{
00018: 			Core.Log?.Warning("收纳箱注册异常（ContainerItemDirectory）：" + ex.Message);
00019: 		}
00020: 	}
00021: }
~~~

### ItemStorage/BoxHotkey.cs

~~~csharp
00022: 
00023: 	private static GameItem? _lastHitBox;
00024: 
00025: 	private static long _lastHitUid;
00026: 
00027: 	private static float _nextRestore;
00028: 
00029: 	private static bool _noBinding;
00030: 
00031: 	private static KeyCode _suppressKey = (KeyCode)0;
00032: 
00033: 	private static bool IsLiveBox(GameItem box)
00034: 	{
00035: 		try
00036: 		{
00037: 			if (box.parentInventory == null)
00038: 			{
00039: 				return false;
00040: 			}
00041: 			return !BoxItem.IsReallyDestroyed(box);
00042: 		}
00043: 		catch
00044: 		{
00045: 			return false;
00046: 		}
00047: 	}
00048: 
00049: 	private static void RestoreFromBoxes()
00050: 	{
00051: 		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
00052: 		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
00053: 		int num = 0;
00054: 		try
00055: 		{
00056: 			foreach (GameItem item in BoxItem.FindAllBoxes())
00057: 			{
00058: 				num++;
00059: 				try
00060: 				{
00061: 					long num2 = item.uniqueId;
00062: 					int boxOption = StorageGrid.GetBoxOption(item, "HOTKEY");
00063: 					int boxOption2 = StorageGrid.GetBoxOption(item, "HOTRESTOCK");
00064: 					if (boxOption != 0 && (!BoundKeys.TryGetValue(boxOption, out var value) || value.Restock || ResolveBox(value.Uid) == null))
00065: 					{
00066: 						BoundKeys[boxOption] = new HotkeyEntry(num2, restock: false);
00067: 						Core.Debug("快捷键自愈：恢复 " + ((object)(KeyCode)boxOption/*cast due to .constrained prefix*/).ToString() + " → 箱 uid " + num2 + "（一键收纳）" + ((value.Uid != 0L && value.Uid != num2) ? ("（旧 uid " + value.Uid + " 已失效，覆盖）") : "") + "。");
00068: 					}
00069: 					if (boxOption2 != 0 && (!BoundKeys.TryGetValue(boxOption2, out var value2) || !value2.Restock || ResolveBox(value2.Uid) == null))
00070: 					{
00071: 						BoundKeys[boxOption2] = new HotkeyEntry(num2, restock: true);
00072: 						Core.Debug("快捷键自愈：恢复 " + ((object)(KeyCode)boxOption2/*cast due to .constrained prefix*/).ToString() + " → 箱 uid " + num2 + "（一键上货）" + ((value2.Uid != 0L && value2.Uid != num2) ? ("（旧 uid " + value2.Uid + " 已失效，覆盖）") : "") + "。");
00073: 					}
00074: 				}
00075: 				catch
00076: 				{
00077: 				}
00078: 			}
00079: 		}
00080: 		catch
00081: 		{
00082: 		}
00083: 		if (num > 0)
00084: 		{
00085: 			_noBinding = true;
00086: 			Core.Debug("快捷键自愈：本轮扫到 " + num + " 箱，恢复 " + BoundKeys.Count + " 键，停扫。");
00087: 		}
00088: 	}
00089: 
00090: 	internal static void ResetRecovery()
00091: 	{
00092: 		BoundKeys.Clear();
00093: 		_noBinding = false;
00094: 		_nextRestore = 0f;
00095: 		_lastHitBox = null;
00096: 		_lastHitUid = 0L;
00097: 		BoxItem.ResetRegistry();
00098: 		BoxStowAll.ResetRecovery();
00099: 		BoxRestock.ResetRecovery();
00100: 	}
00101: 
00102: 	internal static void SuppressUntilRelease(KeyCode key)
00103: 	{
00104: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00105: 		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
00106: 		_suppressKey = key;
00107: 	}
00108: 
00109: 	internal static bool IsSuppressed(KeyCode key)
00110: 	{
00111: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00112: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
00113: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
00114: 		if ((int)_suppressKey != 0)
00115: 		{
00116: 			return _suppressKey == key;
00117: 		}
00118: 		return false;
00119: 	}
00120: 
00121: 	internal static void NotifyBound(long uid, KeyCode key, bool restock = false)
00122: 	{
00123: 		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
00124: 		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
00125: 		//IL_00ac: Expected I4, but got Unknown
00126: 		try
00127: 		{
00128: 			List<int> list = new List<int>();
00129: 			foreach (KeyValuePair<int, HotkeyEntry> boundKey in BoundKeys)
00130: 			{
00131: 				if (boundKey.Value.Uid == uid && boundKey.Value.Restock == restock)
00132: 				{
00133: 					list.Add(boundKey.Key);
00134: 				}
00135: 			}
00136: 			foreach (int item in list)
00137: 			{
00138: 				BoundKeys.Remove(item);
00139: 			}
00140: 			if ((int)key != 0)
00141: 			{
00142: 				BoundKeys[(int)key] = new HotkeyEntry(uid, restock);
00143: 			}
00144: 		}
00145: 		catch
00146: 		{
00147: 		}
00148: 	}
00149: 
00150: 	internal static void Tick()
00151: 	{
00152: 		TickSingleBoxKeys();
00153: 		BoxStowAll.Tick();
00154: 		BoxRestock.Tick();
00155: 		BoxStowAll.Watch(BoundKeys.Count);
00156: 	}
00157: 
00158: 	internal static bool UnbindUidFor(long uid, bool restock)
00159: 	{
00160: 		try
00161: 		{
00162: 			List<int> list = new List<int>();
00163: 			foreach (KeyValuePair<int, HotkeyEntry> boundKey in BoundKeys)
00164: 			{
00165: 				if (boundKey.Value.Uid == uid && boundKey.Value.Restock == restock)
00166: 				{
00167: 					list.Add(boundKey.Key);
00168: 				}
00169: 			}
00170: 			foreach (int item in list)
00171: 			{
00172: 				BoundKeys.Remove(item);
00173: 			}
00174: 			if (list.Count > 0)
00175: 			{
00176: 				Core.Log?.Msg("快捷键映射：箱 uid " + uid + " 的" + (restock ? "一键上货键" : "一键收纳键") + "已由玩家主动解绑（Esc），释放 " + list.Count + " 个键。");
00177: 			}
00178: 			return list.Count > 0;
00179: 		}
00180: 		catch
00181: 		{
00182: 			return false;
00183: 		}
00184: 	}
00185: 
00186: 	internal static bool ForgetUid(long uid)
00187: 	{
00188: 		try
00189: 		{
00190: 			List<int> list = new List<int>();
00191: 			foreach (KeyValuePair<int, HotkeyEntry> boundKey in BoundKeys)
00192: 			{
00193: 				if (boundKey.Value.Uid == uid)
00194: 				{
00195: 					list.Add(boundKey.Key);
00196: 				}
00197: 			}
00198: 			foreach (int item in list)
00199: 			{
00200: 				BoundKeys.Remove(item);
00201: 			}
00202: 			if (list.Count > 0)
00203: 			{
00204: 				Core.Debug("快捷键映射：箱 uid " + uid + " 已离场，释放 " + list.Count + " 个单箱键。");
00205: 			}
00206: 			return list.Count > 0;
00207: 		}
00208: 		catch
00209: 		{
00210: 			return false;
00211: 		}
00212: 	}
00213: 
00214: 	private static void TickSingleBoxKeys()
00215: 	{
00216: 		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
00217: 		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
00218: 		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
00219: 		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
00220: 		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
00221: 		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
00222: 		if (BoundKeys.Count == 0)
00223: 		{
00224: 			if (_noBinding || Time.unscaledTime < _nextRestore)
00225: 			{
00226: 				return;
00227: 			}
00228: 			_nextRestore = Time.unscaledTime + 2f;
00229: 			RestoreFromBoxes();
00230: 			if (BoundKeys.Count == 0)
00231: 			{
00232: 				return;
00233: 			}
00234: 		}
00235: 		if (GearPanel.IsCapturing)
00236: 		{
00237: 			return;
00238: 		}
00239: 		if ((int)_suppressKey != 0)
00240: 		{
00241: 			try
00242: 			{
00243: 				if (Input.GetKey(_suppressKey))
00244: 				{
00245: 					return;
00246: 				}
00247: 			}
00248: 			catch
00249: 			{
00250: 			}
00251: 			_suppressKey = (KeyCode)0;
00252: 		}
00253: 		foreach (KeyValuePair<int, HotkeyEntry> boundKey in BoundKeys)
00254: 		{
00255: 			if (!Input.GetKeyDown((KeyCode)boundKey.Key))
00256: 			{
00257: 				continue;
00258: 			}
00259: 			string text = ((object)(KeyCode)boundKey.Key/*cast due to .constrained prefix*/).ToString();
00260: 			try
00261: 			{
00262: 				if (KeyBindHelper.SuppressHotkeys)
00263: 				{
00264: 					Core.Log?.Msg("快捷键 " + text + " 按下但被官方静默闸拦下（SuppressHotkeys=true，对话框/捕获期标记——若已无对话框仍被拦即官方标记卡住）。");
00265: 					break;
00266: 				}
00267: 			}
00268: 			catch
00269: 			{
00270: 			}
00271: 			try
00272: 			{
00273: 				if (StoreUIManager.IsTextFieldFocused())
00274: 				{
00275: 					Core.Log?.Msg("快捷键 " + text + " 按下但被输入框焦点闸拦下。");
00276: 					break;
00277: 				}
00278: 			}
00279: 			catch
00280: 			{
00281: 			}
00282: 			if (!BoxTransfer.IsInStore())
00283: 			{
00284: 				Core.Log?.Msg("快捷键 " + text + " 按下但判定为店外，忽略。");
00285: 				break;
00286: 			}
00287: 			GameItem val = ResolveBox(boundKey.Value.Uid);
00288: 			if (val == null || !BoxItem.IsBox(val))
00289: 			{
00290: 				Core.Log?.Msg("快捷键 " + text + " 命中但绑定箱失效（uid " + boundKey.Value.Uid + " 找不到活的收纳箱），本帧忽略。");
00291: 			}
00292: 			else if (boundKey.Value.Restock)
00293: 			{
00294: 				Core.Debug("收纳箱快捷键 " + ((object)(KeyCode)boundKey.Key/*cast due to .constrained prefix*/).ToString() + " 触发一键上货。");
00295: 				BoxItem.ClickSound();
00296: 				BoxRestock.RunOne(val, "快捷键");
00297: 			}
00298: 			else
00299: 			{
00300: 				Core.Debug("收纳箱快捷键 " + ((object)(KeyCode)boundKey.Key/*cast due to .constrained prefix*/).ToString() + " 触发一键收纳。");
00301: 				BoxItem.ClickSound();
00302: 				AutoStow.Run(val, "快捷键");
00303: 			}
00304: 			break;
00305: 		}
00306: 	}
00307: 
00308: 	private static GameItem? ResolveBox(long uid)
00309: 	{
00310: 		if (_lastHitUid == uid && _lastHitBox != null)
00311: 		{
00312: 			try
00313: 			{
00314: 				if (BoxItem.IsBox(_lastHitBox) && IsLiveBox(_lastHitBox))
00315: 				{
00316: 					return _lastHitBox;
00317: 				}
00318: 			}
00319: 			catch
00320: 			{
00321: 			}
00322: 			_lastHitBox = null;
00323: 		}
00324: 		foreach (GameItem item in BoxItem.FindAllBoxes())
00325: 		{
00326: 			try
00327: 			{
00328: 				if (item.uniqueId != uid || !IsLiveBox(item))
00329: 				{
00330: 					continue;
00331: 				}
00332: 				_lastHitBox = item;
00333: 				_lastHitUid = uid;
00334: 				return item;
00335: 			}
00336: 			catch
00337: 			{
00338: 			}
00339: 		}
00340: 		return null;
00341: 	}
00342: }
~~~

### ItemStorage/BoxItem.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Reflection;
00004: using HarmonyLib;
00005: using UnityEngine;
00006: 
00007: namespace ItemStorage;
00008: 
00009: internal static class BoxItem
00010: {
00011: 	internal const string StorageId = "Dream_Item_Storage";
00012: 
00013: 	internal const string SpriteKey = "Dream_Item_Storage";
00014: 
00015: 	internal const string LegacySpriteKey = "item_storage";
00016: 
00017: 	internal const string UiTag = "ITEM_STORAGE_BOX_UI_V30";
00018: 
00019: 	internal const string DisplayNameText = "收纳箱";
00020: 
00021: 	internal const string GameItemTypeStorage = "STORAGE";
00022: 
00023: 	private static readonly Dictionary<long, GameItem> _known = new Dictionary<long, GameItem>();
00024: 
00025: 	private static readonly FieldInfo? _destroyedFi = AccessTools.Field(typeof(GameItemElement), "destroyed");
00026: 
00027: 	internal const string NumberTag = "DREAM_ITEM_STORAGE_NUM";
00028: 
00029: 	internal const int NoNumber = -1;
00030: 
00031: 	internal static void ResetRegistry()
00032: 	{
00033: 		_known.Clear();
00034: 	}
00035: 
00036: 	internal static bool IsReallyDestroyed(GameItem? item)
00037: 	{
00038: 		try
00039: 		{
00040: 			GameItemElement val = (GameItemElement)(object)((item is GameItemElement) ? item : null);
00041: 			if (val == null)
00042: 			{
00043: 				return true;
00044: 			}
00045: 			object obj = _destroyedFi?.GetValue(val);
00046: 			bool flag = default(bool);
00047: 			int num;
00048: 			if (obj is bool)
00049: 			{
00050: 				flag = (bool)obj;
00051: 				num = 1;
00052: 			}
00053: 			else
00054: 			{
00055: 				num = 0;
00056: 			}
00057: 			return (byte)((uint)num & (flag ? 1u : 0u)) != 0;
00058: 		}
00059: 		catch
00060: 		{
00061: 			return true;
00062: 		}
00063: 	}
00064: 
00065: 	internal static bool IsBeingDragged(GameItem? item)
00066: 	{
00067: 		if (item == null)
00068: 		{
00069: 			return false;
00070: 		}
00071: 		try
00072: 		{
00073: 			if ((Object)(object)ItemMouseDragHandler.current != (Object)null && ItemMouseDragHandler.current.GetCurrentItem() == item)
00074: 			{
00075: 				return true;
00076: 			}
00077: 		}
00078: 		catch
00079: 		{
00080: 		}
00081: 		try
00082: 		{
00083: 			if ((Object)(object)ItemMultiSelectHandler.current != (Object)null && ItemMultiSelectHandler.current.IsGroupDraggingItem(item))
00084: 			{
00085: 				return true;
00086: 			}
00087: 		}
00088: 		catch
00089: 		{
00090: 		}
00091: 		return false;
00092: 	}
00093: 
00094: 	internal static int ReadNumber(GameItem box)
00095: 	{
00096: 		try
00097: 		{
00098: 			TagSystem state = box.state;
00099: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
00100: 			if (dictionary == null || !dictionary.ContainsKey("DREAM_ITEM_STORAGE_NUM"))
00101: 			{
00102: 				return -1;
00103: 			}
00104: 			return dictionary["DREAM_ITEM_STORAGE_NUM"]?.valueInt ?? (-1);
00105: 		}
00106: 		catch
00107: 		{
00108: 			return -1;
~~~

~~~csharp
00423: 	{
00424: 		if (item == null)
00425: 		{
00426: 			return false;
00427: 		}
00428: 		try
00429: 		{
00430: 			if (item.IsGameItemType("NODE"))
00431: 			{
00432: 				return true;
00433: 			}
00434: 		}
00435: 		catch
00436: 		{
00437: 			return null;
00438: 		}
00439: 		return false;
00440: 	}
00441: 
00442: 	internal static bool IsUsableByPlayer(GameItem? box)
00443: 	{
00444: 		if (!IsBox(box))
00445: 		{
00446: 			return false;
00447: 		}
00448: 		try
00449: 		{
00450: 			return GeneralHelper.IsItemOwned(box);
00451: 		}
00452: 		catch
00453: 		{
00454: 			return true;
00455: 		}
00456: 	}
00457: 
00458: 	internal static GameItem? Create()
00459: 	{
00460: 		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
00461: 		try
00462: 		{
00463: 			Core.Debug("BoxItem: Create() start");
00464: 			GameItem val = ItemDirectory.CreateEmptyItem("Dream_Item_Storage") ?? throw new InvalidOperationException("CreateEmptyItem('Dream_Item_Storage') null");
00465: 			val.identifier = "Dream_Item_Storage";
00466: 			try
00467: 			{
00468: 				val.SetName("收纳箱");
00469: 			}
00470: 			catch
00471: 			{
00472: 			}
00473: 			val.name = "收纳箱";
00474: 			try
00475: 			{
00476: 				val.SetSprite(string.Empty, "Dream_Item_Storage");
00477: 			}
00478: 			catch (Exception ex)
00479: 			{
00480: 				Core.Log?.Warning("收纳箱贴图字段设置失败：" + ex.Message);
00481: 			}
00482: 			try
00483: 			{
00484: 				val.SetShape(new GridShapeBuilder(2, 3).SetDataFill(2, 3, (byte)1).Build());
00485: 			}
00486: 			catch (Exception ex2)
00487: 			{
00488: 				Core.Log?.Warning("收纳箱 SetShape 2x3 失败：" + ex2.Message);
00489: 			}
00490: 			val.SetValue(600L);
00491: 			try
00492: 			{
00493: 				val.SetGameItemType("STORAGE");
00494: 			}
00495: 			catch
00496: 			{
00497: 			}
00498: 			try
00499: 			{
00500: 				val.EnableTag("NESTABLE_DEVICE_TAG", false);
00501: 			}
00502: 			catch
00503: 			{
00504: 			}
00505: 			try
00506: 			{
00507: 				val.EnableTag("ITEM_STORAGE_BOX_UI_V30", false);
00508: 			}
00509: 			catch
00510: 			{
00511: 			}
00512: 			try
00513: 			{
00514: 				ContainerHelper.AddContainerTooltipTag(val);
00515: 			}
00516: 			catch (Exception ex3)
00517: 			{
00518: 				Core.Log?.Warning("收纳箱 AddContainerTooltipTag 失败：" + ex3.Message);
00519: 			}
00520: 			try
00521: 			{
00522: 				AudioHelper.InitPouch(val);
00523: 			}
00524: 			catch
00525: 			{
00526: 				try
00527: 				{
00528: 					AudioHelper.InitPlasticCard(val);
00529: 				}
00530: 				catch
00531: 				{
00532: 				}
00533: 			}
00534: 			BoxWindow.UpdateTooltip(val);
00535: 			BoxWindow.EnsureWindow(val, "Create");
00536: 			Core.Debug("BoxItem: Create() done");
00537: 			return val;
00538: 		}
00539: 		catch (Exception ex4)
00540: 		{
00541: 			Core.Debug("BoxItem: Create() 早期调用失败（渲染树未就绪？返回 null 等真实调用）：" + ex4.Message);
00542: 			return null;
00543: 		}
00544: 	}
00545: 
00546: 	private static void RegisterCore(string label, Func<bool> has, Func<Func<GameItem>, bool> add)
00547: 	{
00548: 		if (has == null || add == null)
00549: 		{
00550: 			return;
00551: 		}
00552: 		try
00553: 		{
00554: 			if (has())
00555: 			{
00556: 				return;
00557: 			}
00558: 		}
00559: 		catch
00560: 		{
00561: 			return;
00562: 		}
00563: 		try
00564: 		{
00565: 			if (DirectoryMaster.Has<GameItem>("Dream_Item_Storage"))
00566: 			{
00567: 				Core.Debug("已注册物品工厂：Dream_Item_Storage 全局已有，跳过 " + label);
00568: 				return;
00569: 			}
00570: 		}
00571: 		catch
00572: 		{
00573: 		}
00574: 		try
00575: 		{
00576: 			Func<GameItem> arg = () => Create() ?? throw new InvalidOperationException("BoxItem.Create 返回 null（目录/模板未就绪）");
00577: 			if (!add(arg))
00578: 			{
00579: 				Core.Log?.Warning("收纳箱注册：" + label + ".Add 返回 false");
00580: 			}
00581: 			else
00582: 			{
00583: 				Core.Debug("已注册物品工厂：Dream_Item_Storage @ " + label);
00584: 			}
00585: 		}
00586: 		catch (Exception ex)
00587: 		{
00588: 			Core.Log?.Error("注册收纳箱失败（" + label + "）：" + ex.Message);
00589: 		}
00590: 	}
00591: 
00592: 	internal static void Register(MiscItemDirectory dir)
00593: 	{
00594: 		RegisterCore("MiscItemDirectory", () => ((Directory<GameItem>)(object)dir).Has("Dream_Item_Storage"), (Func<GameItem> f) => ((Directory<GameItem>)(object)dir).Add("Dream_Item_Storage", f));
00595: 	}
00596: 
00597: 	internal static void Register(ModItemDirectory dir)
00598: 	{
00599: 		RegisterCore("ModItemDirectory", () => ((Directory<GameItem>)(object)dir).Has("Dream_Item_Storage"), (Func<GameItem> f) => ((Directory<GameItem>)(object)dir).Add("Dream_Item_Storage", f));
00600: 	}
00601: 
00602: 	internal static void Register(ContainerItemDirectory dir)
00603: 	{
00604: 		RegisterCore("ContainerItemDirectory", () => ((Directory<GameItem>)(object)dir).Has("Dream_Item_Storage"), (Func<GameItem> f) => ((Directory<GameItem>)(object)dir).Add("Dream_Item_Storage", f));
00605: 	}
00606: }
~~~

### ItemStorage/BoxLabel.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Text;
00004: using TMPro;
00005: using UnityEngine;
00006: using UnityEngine.UI;
00007: 
00008: namespace ItemStorage;
00009: 
00010: internal static class BoxLabel
00011: {
00012: 	private sealed class Entry
00013: 	{
00014: 		internal int Uid;
00015: 
00016: 		internal string BaseName = "";
00017: 
00018: 		internal float LastApplyAt = -99f;
00019: 
00020: 		internal bool PendingItem;
00021: 
00022: 		internal TMP_Text? Label;
00023: 
00024: 		internal GameItemElement? El;
00025: 
00026: 		internal RectTransform? RootRt;
00027: 
00028: 		internal RectTransform? IconRt;
00029: 	}
00030: 
00031: 	private sealed class Anchor
00032: 	{
00033: 		internal readonly RectTransform Rt;
00034: 
00035: 		internal readonly string Kind;
00036: 
00037: 		internal Anchor(RectTransform rt, string kind)
00038: 		{
00039: 			Rt = rt;
00040: 			Kind = kind;
00041: 		}
00042: 	}
00043: 
00044: 	private const int MaxChars = 3;
00045: 
00046: 	private const string DefaultText = "收纳箱";
00047: 
00048: 	private const float FontRatioVertical = 0.23f;
00049: 
00050: 	private const float FontRatioHorizontal = 0.35f;
00051: 
00052: 	private const float Throttle = 0.1f;
00053: 
00054: 	private const float FindInterval = 2f;
00055: 
00056: 	private const float MissTtl = 10f;
00057: 
00058: 	private const string HideNamesKey = "BoxLabelHideNames";
00059: 
00060: 	private static readonly Vector3[] Corners = (Vector3[])(object)new Vector3[4];
00061: 
00062: 	private static readonly Dictionary<GameItemElement, Entry> Entries = new Dictionary<GameItemElement, Entry>();
00063: 
00064: 	private static readonly Dictionary<int, (GameItem? Box, float At)> ItemCache = new Dictionary<int, (GameItem, float)>();
00065: 
00066: 	private static readonly HashSet<int> DragUids = new HashSet<int>();
00067: 
00068: 	private static readonly HashSet<int> LoggedOnce = new HashSet<int>();
00069: 
00070: 	private static float _nextFindAt;
00071: 
00072: 	private static float _nextTickAt;
00073: 
00074: 	private static float _nextAttachAt;
00075: 
00076: 	private static TMP_FontAsset? _tmpFont;
00077: 
00078: 	private static string HideKeyFor(int uid)
00079: 	{
00080: 		return "BoxLabelHideNames." + uid;
00081: 	}
00082: 
00083: 	internal static bool IsShownFor(int uid)
00084: 	{
00085: 		PlayerStore instance = PlayerStore.Instance;
00086: 		if (instance == null)
00087: 		{
00088: 			return true;
00089: 		}
00090: 		try
00091: 		{
00092: 			return PerSaveState.GetInt(instance, HideKeyFor(uid)) != 1;
00093: 		}
00094: 		catch
00095: 		{
00096: 			return true;
00097: 		}
00098: 	}
00099: 
00100: 	internal static void ToggleFor(int uid)
00101: 	{
00102: 		int num = (IsShownFor(uid) ? 1 : 0);
00103: 		PlayerStore instance = PlayerStore.Instance;
00104: 		try
00105: 		{
00106: 			if (instance != null)
00107: 			{
00108: 				PerSaveState.SetInt(instance, HideKeyFor(uid), num);
00109: 			}
00110: 		}
00111: 		catch
00112: 		{
00113: 		}
00114: 		foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00115: 		{
00116: 			Entry value = entry.Value;
00117: 			if (value != null && value.Uid == uid)
00118: 			{
00119: 				ApplyVisibility(entry.Value);
00120: 			}
00121: 		}
00122: 		Core.Debug("收纳箱 BoxLabel[uid=" + uid + "]：名字显隐切换 → " + ((num == 1) ? "隐藏" : "显示"));
00123: 	}
00124: 
00125: 	internal static void OnDragStart(IEnumerable<GameItem?>? items)
00126: 	{
00127: 		try
00128: 		{
00129: 			DragUids.Clear();
00130: 			if (items != null)
00131: 			{
00132: 				foreach (GameItem item in items)
00133: 				{
00134: 					if (item == null)
00135: 					{
00136: 						continue;
00137: 					}
00138: 					try
00139: 					{
00140: 						if (BoxItem.IsBox(item))
00141: 						{
00142: 							int uniqueId;
00143: 							try
00144: 							{
00145: 								uniqueId = item.uniqueId;
00146: 							}
00147: 							catch
00148: 							{
00149: 								goto end_IL_0021;
00150: 							}
00151: 							if (uniqueId != 0)
00152: 							{
00153: 								DragUids.Add(uniqueId);
00154: 							}
00155: 						}
00156: 						end_IL_0021:;
00157: 					}
00158: 					catch
00159: 					{
00160: 					}
00161: 				}
00162: 			}
00163: 			if (DragUids.Count <= 0)
00164: 			{
00165: 				return;
00166: 			}
00167: 			foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00168: 			{
00169: 				ApplyVisibility(entry.Value);
00170: 			}
00171: 		}
00172: 		catch
00173: 		{
00174: 		}
00175: 	}
00176: 
00177: 	internal static void OnDragEnd()
00178: 	{
00179: 		try
00180: 		{
00181: 			DragUids.Clear();
00182: 			foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00183: 			{
00184: 				ApplyVisibility(entry.Value);
00185: 				RealignEntry(entry.Value);
00186: 			}
00187: 		}
00188: 		catch
00189: 		{
00190: 		}
00191: 	}
00192: 
00193: 	internal static void Apply(GameItemElement el)
00194: 	{
00195: 		try
00196: 		{
00197: 			if (el == null)
00198: 			{
00199: 				return;
00200: 			}
00201: 			int uniqueId;
00202: 			try
00203: 			{
00204: 				uniqueId = ((GameItem)el).uniqueId;
00205: 			}
00206: 			catch (Exception ex)
00207: 			{
00208: 				LogOnce(-1, "uniqueId 读取失败：" + ex.GetType().Name + " " + ex.Message);
00209: 				return;
00210: 			}
00211: 			if (uniqueId == 0)
~~~

~~~csharp
00213: 				LogOnce(0, "uniqueId=0（元素晚绑定），先挂默认字待补绑");
00214: 			}
00215: 			if (Entries.TryGetValue(el, out Entry value) && value != null && Time.time - value.LastApplyAt < 0.1f)
00216: 			{
00217: 				return;
00218: 			}
00219: 			RectTransform val = PickRootRt(el);
00220: 			RectTransform val2 = PickIconRt(el, val);
00221: 			if ((Object)(object)val == (Object)null || (Object)(object)val2 == (Object)null)
00222: 			{
00223: 				LogOnce(uniqueId, "无挂点：根/图标节点均不可得");
00224: 				return;
00225: 			}
00226: 			if (value == null)
00227: 			{
00228: 				value = new Entry();
00229: 				Entries[el] = value;
00230: 			}
00231: 			value.LastApplyAt = Time.time;
00232: 			value.Uid = uniqueId;
00233: 			value.El = el;
00234: 			value.RootRt = val;
00235: 			value.IconRt = val2;
00236: 			GameItem val3 = ((uniqueId != 0) ? FindBox(uniqueId, "apply") : null);
00237: 			value.PendingItem = uniqueId != 0 && val3 == null;
00238: 			value.BaseName = BaseNameOf(val3);
00239: 			EnsureLabel(val, value);
00240: 			RealignEntry(value);
00241: 			ApplyVisibility(value);
00242: 		}
00243: 		catch (Exception ex2)
00244: 		{
00245: 			LogOnce(-2, "Apply 异常：" + ex2.GetType().Name + " " + ex2.Message);
00246: 		}
00247: 	}
00248: 
00249: 	internal static void Strip(GameItemElement el)
00250: 	{
00251: 		try
00252: 		{
00253: 			if (el != null && Entries.Count != 0 && Entries.TryGetValue(el, out Entry value) && value != null)
00254: 			{
00255: 				DestroyLabel(value.Label);
00256: 				value.Label = null;
00257: 				Entries.Remove(el);
00258: 				Core.Debug("收纳箱 BoxLabel：元素复用给非箱物品，已摘字（uid=" + value.Uid + "）。");
00259: 			}
00260: 		}
00261: 		catch
00262: 		{
00263: 		}
00264: 	}
00265: 
00266: 	internal static void Sweep()
00267: 	{
00268: 		try
00269: 		{
00270: 			if (Entries.Count > 0)
00271: 			{
00272: 				List<GameItemElement> list = null;
00273: 				foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00274: 				{
00275: 					Entry value = entry.Value;
00276: 					if ((Object)(object)value.Label == (Object)null || !Alive((Object?)(object)value.Label) || (Object)(object)value.RootRt == (Object)null || !Alive((Object?)(object)value.RootRt) || (Object)(object)value.IconRt == (Object)null || !Alive((Object?)(object)value.IconRt))
00277: 					{
00278: 						(list ?? (list = new List<GameItemElement>())).Add(entry.Key);
00279: 					}
00280: 				}
00281: 				if (list != null)
00282: 				{
00283: 					foreach (GameItemElement item2 in list)
00284: 					{
00285: 						Entries.Remove(item2);
00286: 					}
00287: 				}
00288: 			}
00289: 			if (ItemCache.Count > 0)
00290: 			{
00291: 				List<int> list2 = null;
00292: 				foreach (KeyValuePair<int, (GameItem, float)> item3 in ItemCache)
00293: 				{
00294: 					GameItem item = item3.Value.Item1;
00295: 					if (item == null || !Alive((object?)item))
00296: 					{
00297: 						(list2 ?? (list2 = new List<int>())).Add(item3.Key);
00298: 					}
00299: 				}
00300: 				if (list2 != null)
00301: 				{
00302: 					foreach (int item4 in list2)
00303: 					{
00304: 						ItemCache.Remove(item4);
00305: 					}
00306: 				}
00307: 			}
00308: 			if (LoggedOnce.Count > 512)
00309: 			{
00310: 				LoggedOnce.Clear();
00311: 			}
00312: 		}
00313: 		catch
00314: 		{
00315: 		}
00316: 	}
00317: 
00318: 	internal static void Tick()
00319: 	{
00320: 		try
00321: 		{
00322: 			if (Time.time >= _nextAttachAt)
00323: 			{
00324: 				_nextAttachAt = Time.time + 10f;
00325: 				SweepAttach();
00326: 			}
00327: 			if (Entries.Count == 0 || Time.time < _nextTickAt)
00328: 			{
00329: 				return;
00330: 			}
00331: 			_nextTickAt = Time.time + 2f;
00332: 			bool flag = false;
00333: 			foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00334: 			{
00335: 				if (entry.Value.PendingItem)
00336: 				{
00337: 					flag = true;
00338: 					break;
00339: 				}
00340: 			}
00341: 			if (flag)
00342: 			{
00343: 				_nextFindAt = 0f;
00344: 				foreach (KeyValuePair<GameItemElement, Entry> entry2 in Entries)
00345: 				{
00346: 					Entry value = entry2.Value;
00347: 					if (value.PendingItem)
00348: 					{
00349: 						GameItem val = FindBox(value.Uid, "tick");
00350: 						if (val != null)
00351: 						{
00352: 							value.PendingItem = false;
00353: 							value.BaseName = BaseNameOf(val);
00354: 						}
00355: 					}
00356: 				}
00357: 			}
00358: 			foreach (KeyValuePair<GameItemElement, Entry> entry3 in Entries)
00359: 			{
00360: 				Entry value2 = entry3.Value;
00361: 				if (value2.Uid == 0 && value2.El != null && Alive((object?)value2.El))
00362: 				{
00363: 					int uniqueId;
00364: 					try
00365: 					{
00366: 						uniqueId = ((GameItem)value2.El).uniqueId;
00367: 					}
00368: 					catch
00369: 					{
00370: 						continue;
00371: 					}
00372: 					if (uniqueId != 0)
00373: 					{
00374: 						value2.Uid = uniqueId;
00375: 						value2.PendingItem = true;
00376: 						Core.Debug("收纳箱 BoxLabel：uid 补绑 0→" + uniqueId + "（元素晚绑定）");
00377: 					}
00378: 				}
00379: 			}
00380: 			foreach (KeyValuePair<GameItemElement, Entry> entry4 in Entries)
00381: 			{
00382: 				Entry value3 = entry4.Value;
00383: 				if ((Object)(object)value3.Label == (Object)null)
00384: 				{
00385: 					continue;
00386: 				}
00387: 				if (!value3.PendingItem)
00388: 				{
00389: 					GameItem val2 = CachedBox(value3.Uid);
00390: 					if (val2 == null)
00391: 					{
00392: 						value3.PendingItem = true;
00393: 					}
00394: 					else
00395: 					{
00396: 						value3.BaseName = BaseNameOf(val2);
00397: 					}
00398: 				}
00399: 				RealignEntry(value3);
00400: 				ApplyVisibility(value3);
00401: 			}
00402: 		}
00403: 		catch
00404: 		{
00405: 		}
00406: 	}
00407: 
00408: 	internal static void OnItemRenamed(GameItem? item)
00409: 	{
00410: 		try
00411: 		{
00412: 			if (item == null || Entries.Count == 0)
00413: 			{
00414: 				if (item != null)
00415: 				{
00416: 					SweepAttach();
00417: 				}
00418: 				return;
00419: 			}
00420: 			int uniqueId;
00421: 			try
00422: 			{
00423: 				uniqueId = item.uniqueId;
00424: 			}
00425: 			catch
00426: 			{
00427: 				return;
00428: 			}
00429: 			bool flag = false;
00430: 			foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00431: 			{
00432: 				Entry value = entry.Value;
00433: 				if (value.Uid == uniqueId)
00434: 				{
00435: 					flag = true;
00436: 					value.PendingItem = false;
00437: 					value.BaseName = BaseNameOf(item);
00438: 					RealignEntry(value);
00439: 				}
00440: 			}
00441: 			if (!flag)
00442: 			{
00443: 				ItemCache[uniqueId] = (item, Time.time);
00444: 				SweepAttach();
00445: 			}
00446: 		}
00447: 		catch
00448: 		{
00449: 		}
00450: 	}
00451: 
00452: 	internal static void SweepAttach()
00453: 	{
00454: 	}
00455: 
00456: 	private static bool HasRegisteredSprite(GameItemElement el)
00457: 	{
00458: 		try
00459: 		{
00460: 			Transform val = null;
00461: 			try
00462: 			{
00463: 				TreeNodeRender handler = el.handler;
00464: 				val = ((handler != null) ? ((Component)handler).transform : null);
00465: 			}
~~~

~~~csharp
00789: 				{
00790: 				}
00791: 				val2.transform.SetAsLastSibling();
00792: 				entry.Label = (TMP_Text?)(object)val3;
00793: 				LogOnce(entry.Uid, "文字层已挂（TMP+元素根+世界对齐，font=" + (((Object)(object)((TMP_Text)val3).font != (Object)null) ? "OK" : "NULL") + "）");
00794: 			}
00795: 		}
00796: 		catch (Exception ex)
00797: 		{
00798: 			LogOnce(entry.Uid, "EnsureLabel 异常：" + ex.GetType().Name + " " + ex.Message);
00799: 		}
00800: 	}
00801: 
00802: 	internal static TMP_FontAsset? GetTmpFont()
00803: 	{
00804: 		if ((Object)(object)_tmpFont != (Object)null)
00805: 		{
00806: 			try
00807: 			{
00808: 				if ((Object)(object)_tmpFont != (Object)null)
00809: 				{
00810: 					return _tmpFont;
00811: 				}
00812: 			}
00813: 			catch
00814: 			{
00815: 			}
00816: 			_tmpFont = null;
00817: 		}
00818: 		try
00819: 		{
00820: 			TMP_FontAsset[] array = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
00821: 			if (array != null)
00822: 			{
00823: 				TMP_FontAsset[] array2 = array;
00824: 				foreach (TMP_FontAsset val in array2)
00825: 				{
00826: 					try
00827: 					{
00828: 						if (!((Object)(object)val == (Object)null))
00829: 						{
00830: 							string name = ((Object)val).name;
00831: 							if (!string.IsNullOrEmpty(name) && (name.IndexOf("SC", StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("CN", StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("Chinese", StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("Han", StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("NotoSansSC", StringComparison.OrdinalIgnoreCase) >= 0))
00832: 							{
00833: 								_tmpFont = val;
00834: 								Core.Debug("收纳箱 BoxLabel：TMP 字体命中（CJK 扫描）：" + name);
00835: 								return _tmpFont;
00836: 							}
00837: 						}
00838: 					}
00839: 					catch
00840: 					{
00841: 					}
00842: 				}
00843: 			}
00844: 		}
00845: 		catch
00846: 		{
00847: 		}
00848: 		try
00849: 		{
00850: 			TextMeshProUGUI[] array3 = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
00851: 			if (array3 != null)
00852: 			{
00853: 				TextMeshProUGUI[] array4 = array3;
00854: 				foreach (TextMeshProUGUI val2 in array4)
00855: 				{
00856: 					try
00857: 					{
00858: 						if ((Object)(object)val2 == (Object)null || (Object)(object)((TMP_Text)val2).font == (Object)null)
00859: 						{
00860: 							continue;
00861: 						}
00862: 						_tmpFont = ((TMP_Text)val2).font;
00863: 						Core.Debug("收纳箱 BoxLabel：TMP 字体兜底（在用组件）：" + ((Object)((TMP_Text)val2).font).name);
00864: 						return _tmpFont;
00865: 					}
00866: 					catch
00867: 					{
00868: 					}
00869: 				}
00870: 			}
00871: 		}
00872: 		catch
00873: 		{
00874: 		}
00875: 		try
00876: 		{
00877: 			TMP_FontAsset[] array5 = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
00878: 			if (array5 != null && array5.Length != 0)
00879: 			{
00880: 				_tmpFont = array5[0];
00881: 				Core.Debug("收纳箱 BoxLabel：TMP 字体最后兜底（第一个资产）");
00882: 				return _tmpFont;
00883: 			}
00884: 		}
00885: 		catch
00886: 		{
00887: 		}
00888: 		return null;
00889: 	}
00890: 
00891: 	private static void RealignEntry(Entry e)
00892: 	{
00893: 		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
00894: 		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
00895: 		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
00896: 		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
00897: 		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
00898: 		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
00899: 		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
00900: 		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
00901: 		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
00902: 		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
00903: 		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
00904: 		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
00905: 		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
00906: 		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
00907: 		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
00908: 		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
00909: 		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
00910: 		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
00911: 		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
00912: 		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
00913: 		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
00914: 		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
00915: 		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
00916: 		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
00917: 		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
00918: 		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
00919: 		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
00920: 		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
00921: 		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
00922: 		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
00923: 		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
00924: 		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
00925: 		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
00926: 		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
00927: 		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
00928: 		try
00929: 		{
00930: 			TMP_Text label = e.Label;
00931: 			RectTransform iconRt = e.IconRt;
00932: 			RectTransform rootRt = e.RootRt;
00933: 			if ((Object)(object)label == (Object)null || (Object)(object)iconRt == (Object)null || (Object)(object)rootRt == (Object)null || !Alive((Object?)(object)label) || !Alive((Object?)(object)iconRt) || !Alive((Object?)(object)rootRt))
00934: 			{
00935: 				return;
00936: 			}
00937: 			try
00938: 			{
00939: 				iconRt.GetWorldCorners(Corners);
00940: 			}
00941: 			catch
00942: 			{
00943: 				return;
00944: 			}
00945: 			Vector3 val = Corners[0];
00946: 			Vector3 val2 = Corners[2];
00947: 			float num = Mathf.Abs(val2.x - val.x);
00948: 			float num2 = Mathf.Abs(val2.y - val.y);
00949: 			if (num < 1f || num2 < 1f)
00950: 			{
00951: 				return;
00952: 			}
00953: 			Vector3 val3 = default(Vector3);
00954: 			((Vector3)(ref val3))._002Ector((val.x + val2.x) * 0.5f, (val.y + val2.y) * 0.5f, (val.z + val2.z) * 0.5f);
00955: 			bool flag = num > num2;
00956: 			string text = FormatText(e.BaseName, flag);
00957: 			Vector3 lossyScale = ((Transform)rootRt).lossyScale;
00958: 			float num3 = Mathf.Max(Mathf.Abs(lossyScale.x), 0.01f);
00959: 			float num4 = Mathf.Max(Mathf.Abs(lossyScale.y), 0.01f);
00960: 			float num5 = num / num3;
00961: 			float num6 = num2 / num4;
00962: 			int num7 = Mathf.Clamp(Mathf.RoundToInt((flag ? (num2 * 0.35f) : (num2 * 0.23f)) / num4), 6, 40);
00963: 			RectTransform rectTransform = label.rectTransform;
00964: 			if (!((Object)(object)rectTransform == (Object)null))
00965: 			{
00966: 				if (label.text != text)
00967: 				{
00968: 					label.text = text;
00969: 				}
00970: 				float num8 = num7;
~~~

~~~csharp
00983: 					((Transform)rectTransform).localPosition = val4;
00984: 				}
00985: 				Quaternion val6;
00986: 				try
00987: 				{
00988: 					val6 = Quaternion.Inverse(((Transform)rootRt).rotation);
00989: 				}
00990: 				catch
00991: 				{
00992: 					val6 = Quaternion.identity;
00993: 				}
00994: 				if (Mathf.Abs(Quaternion.Dot(((Transform)rectTransform).localRotation, val6)) < 0.999f)
00995: 				{
00996: 					((Transform)rectTransform).localRotation = val6;
00997: 				}
00998: 			}
00999: 		}
01000: 		catch
01001: 		{
01002: 		}
01003: 	}
01004: 
01005: 	private static void ApplyVisibility(Entry? e)
01006: 	{
01007: 		if ((Object)(object)e?.Label == (Object)null)
01008: 		{
01009: 			return;
01010: 		}
01011: 		try
01012: 		{
01013: 			bool flag = e.Uid != 0 && IsShownFor(e.Uid) && !DragUids.Contains(e.Uid);
01014: 			if (((Component)e.Label).gameObject.activeSelf != flag)
01015: 			{
01016: 				((Component)e.Label).gameObject.SetActive(flag);
01017: 			}
01018: 		}
01019: 		catch
01020: 		{
01021: 		}
01022: 	}
01023: 
01024: 	internal static string BaseNameOf(GameItem? box)
01025: 	{
01026: 		if (box == null)
01027: 		{
01028: 			return "收纳箱";
01029: 		}
01030: 		try
01031: 		{
01032: 			if (GeneralHelper.HasCustomName(box))
01033: 			{
01034: 				string text = Core.Clean(GeneralHelper.GetCustomName(box));
01035: 				if (!string.IsNullOrEmpty(text))
01036: 				{
01037: 					string text2 = text.Trim();
01038: 					if (text2.Length > 0)
01039: 					{
01040: 						return text2;
01041: 					}
01042: 				}
01043: 			}
01044: 		}
01045: 		catch
01046: 		{
01047: 		}
01048: 		return "收纳箱";
01049: 	}
01050: 
01051: 	private static string FormatText(string? baseName, bool horizontal)
01052: 	{
01053: 		if (!NullGuard.HasText(baseName))
01054: 		{
01055: 			baseName = "收纳箱";
01056: 		}
01057: 		int num = Math.Min(baseName.Length, 3);
01058: 		if (num <= 0)
01059: 		{
01060: 			return "收纳箱";
01061: 		}
01062: 		if (horizontal)
01063: 		{
01064: 			return baseName.Substring(0, num);
01065: 		}
01066: 		StringBuilder stringBuilder = new StringBuilder();
01067: 		for (int i = 0; i < num; i++)
01068: 		{
01069: 			if (i > 0)
01070: 			{
01071: 				stringBuilder.Append('\n');
01072: 			}
01073: 			stringBuilder.Append(baseName[i]);
01074: 		}
01075: 		return stringBuilder.ToString();
01076: 	}
01077: 
01078: 	private static GameItem? FindBox(int uid, string source)
01079: 	{
01080: 		if (ItemCache.TryGetValue(uid, out (GameItem, float) value))
01081: 		{
01082: 			if (value.Item1 != null && Alive((object?)value.Item1) && SameUid(value.Item1, uid))
01083: 			{
01084: 				return value.Item1;
01085: 			}
01086: 			bool num = Time.time - value.Item2 >= 10f;
01087: 			ItemCache.Remove(uid);
01088: 			if (!num)
01089: 			{
01090: 				return null;
01091: 			}
01092: 		}
01093: 		if (Time.time < _nextFindAt)
01094: 		{
01095: 			return null;
01096: 		}
01097: 		_nextFindAt = Time.time + 2f;
01098: 		try
01099: 		{
01100: 			foreach (GameItem item in BoxItem.FindAllBoxes())
01101: 			{
01102: 				int uniqueId;
01103: 				try
01104: 				{
01105: 					uniqueId = item.uniqueId;
01106: 				}
01107: 				catch
01108: 				{
01109: 					continue;
01110: 				}
01111: 				ItemCache[uniqueId] = (item, Time.time);
01112: 			}
01113: 			Core.Debug("收纳箱 BoxLabel：uid 反查扫描完成（触发 uid=" + uid + "，来源=" + source + "，缓存 " + ItemCache.Count + " 条）。");
01114: 		}
01115: 		catch
01116: 		{
01117: 		}
01118: 		ItemCache.TryGetValue(uid, out (GameItem, float) value2);
01119: 		return value2.Item1;
01120: 	}
01121: 
01122: 	private static bool SameUid(GameItem item, int uid)
01123: 	{
01124: 		try
01125: 		{
01126: 			return item.uniqueId == uid;
01127: 		}
01128: 		catch
01129: 		{
01130: 			return false;
01131: 		}
01132: 	}
01133: 
01134: 	private static GameItem? CachedBox(int uid)
01135: 	{
01136: 		if (ItemCache.TryGetValue(uid, out (GameItem, float) value) && value.Item1 != null && Alive((object?)value.Item1) && SameUid(value.Item1, uid))
01137: 		{
01138: 			return value.Item1;
01139: 		}
01140: 		return null;
01141: 	}
01142: 
01143: 	private static void LogOnce(int uid, string msg)
01144: 	{
01145: 		if (LoggedOnce.Add(uid))
01146: 		{
01147: 			if (LoggedOnce.Count > 512)
01148: 			{
01149: 				LoggedOnce.Clear();
01150: 			}
01151: 			Core.Debug("收纳箱 BoxLabel[uid=" + uid + "]：" + msg);
01152: 		}
01153: 	}
01154: 
01155: 	private static bool Alive(Object? o)
01156: 	{
01157: 		return o != (Object)null;
01158: 	}
01159: 
01160: 	private static bool Alive(object? o)
01161: 	{
01162: 		return o != null;
01163: 	}
01164: 
01165: 	private static bool SameParent(TMP_Text txt, RectTransform parent)
01166: 	{
01167: 		try
01168: 		{
01169: 			Transform transform = txt.transform;
01170: 			Transform val = ((transform != null) ? transform.parent : null);
01171: 			return (Object)(object)val != (Object)null && (Object)(object)parent != (Object)null && (Object)(object)val == (Object)(object)parent;
01172: 		}
01173: 		catch
01174: 		{
01175: 			return false;
01176: 		}
01177: 	}
01178: 
01179: 	private static void DestroyLabel(TMP_Text? txt)
01180: 	{
01181: 		try
01182: 		{
01183: 			if (!((Object)(object)txt == (Object)null))
01184: 			{
01185: 				Object.Destroy((Object)(object)((Component)txt).gameObject);
01186: 			}
01187: 		}
01188: 		catch
01189: 		{
01190: 		}
01191: 	}
01192: }
~~~

### ItemStorage/BoxMiscDirectoryPatch.cs

~~~csharp
00001: using System;
00002: using HarmonyLib;
00003: 
00004: namespace ItemStorage;
00005: 
00006: [HarmonyPatch(typeof(MiscItemDirectory), "InitDirectory")]
00007: internal static class BoxMiscDirectoryPatch
00008: {
00009: 	[HarmonyPriority(0)]
00010: 	private static void Postfix(MiscItemDirectory __instance)
00011: 	{
00012: 		try
00013: 		{
00014: 			BoxItem.Register(__instance);
00015: 		}
00016: 		catch (Exception ex)
00017: 		{
00018: 			Core.Log?.Warning("收纳箱注册异常（MiscItemDirectory）：" + ex.Message);
00019: 		}
00020: 	}
00021: }
~~~

### ItemStorage/BoxModDirectoryPatch.cs

~~~csharp
00001: using System;
00002: using HarmonyLib;
00003: 
00004: namespace ItemStorage;
00005: 
00006: [HarmonyPatch(typeof(ModItemDirectory), "InitDirectory")]
00007: internal static class BoxModDirectoryPatch
00008: {
00009: 	[HarmonyPriority(0)]
00010: 	private static void Postfix(ModItemDirectory __instance)
00011: 	{
00012: 		try
00013: 		{
00014: 			BoxItem.Register(__instance);
00015: 		}
00016: 		catch (Exception ex)
00017: 		{
00018: 			Core.Log?.Warning("收纳箱注册异常（ModItemDirectory）：" + ex.Message);
00019: 		}
00020: 	}
00021: }
~~~

### ItemStorage/BoxRestock.cs

~~~csharp
00074: 			}
00075: 			return _key;
00076: 		}
00077: 	}
00078: 
00079: 	private static void LoadKey()
00080: 	{
00081: 		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
00082: 		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
00083: 		try
00084: 		{
00085: 			PlayerStore instance = PlayerStore.Instance;
00086: 			if (instance != null)
00087: 			{
00088: 				_loaded = true;
00089: 				int num = PerSaveState.GetInt(instance, "RestockAllHotkey");
00090: 				_key = (KeyCode)((num >= 8 && num <= 350) ? num : 0);
00091: 			}
00092: 		}
00093: 		catch
00094: 		{
00095: 			_loaded = true;
00096: 			_key = (KeyCode)0;
00097: 		}
00098: 	}
00099: 
00100: 	internal static void SetKey(KeyCode key)
00101: 	{
00102: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00103: 		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
00104: 		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
00105: 		//IL_0021: Expected I4, but got Unknown
00106: 		_key = key;
00107: 		_loaded = true;
00108: 		try
00109: 		{
00110: 			PlayerStore instance = PlayerStore.Instance;
00111: 			if (instance != null)
00112: 			{
00113: 				PerSaveState.SetInt(instance, "RestockAllHotkey", (int)key);
00114: 				PerSaveState.Save(instance);
00115: 			}
00116: 		}
00117: 		catch (Exception ex)
00118: 		{
00119: 			Core.Log?.Warning("全箱上货键位写入存档失败：" + ex.Message);
00120: 		}
00121: 	}
00122: 
00123: 	private static void ClearKey(string reason)
00124: 	{
00125: 		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
00126: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
00127: 		if (_loaded && (int)_key == 0)
00128: 		{
00129: 			return;
00130: 		}
00131: 		_key = (KeyCode)0;
00132: 		_loaded = true;
00133: 		try
00134: 		{
00135: 			PlayerStore instance = PlayerStore.Instance;
00136: 			if (instance != null)
00137: 			{
00138: 				PerSaveState.SetInt(instance, "RestockAllHotkey", 0);
00139: 			}
00140: 		}
00141: 		catch
00142: 		{
00143: 		}
00144: 		Core.Log?.Msg("全箱上货键位已解绑：" + reason);
00145: 	}
00146: 
00147: 	internal static void UnbindManually()
00148: 	{
00149: 		ClearKey("玩家在齿轮面板捕获态按 Esc 主动解除绑定");
00150: 	}
00151: 
00152: 	internal static void ResetRecovery()
00153: 	{
00154: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
00155: 		_loaded = false;
00156: 		_key = (KeyCode)0;
00157: 	}
00158: 
00159: 	internal static void OnAllBoxesDestroyed()
00160: 	{
00161: 		ClearKey("玩家拥有的收纳箱已全部被系统销毁，释放全箱上货键");
00162: 	}
00163: 
00164: 	internal unsafe static void Tick()
00165: 	{
00166: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00167: 		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
00168: 		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
00169: 		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
00170: 		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
00171: 		PumpRun();
00172: 		KeyCode key = Key;
00173: 		if ((int)key == 0)
00174: 		{
00175: 			return;
00176: 		}
00177: 		try
00178: 		{
00179: 			if (!Input.GetKeyDown(key))
00180: 			{
00181: 				return;
00182: 			}
00183: 		}
00184: 		catch
00185: 		{
00186: 			return;
00187: 		}
00188: 		if (GearPanel.IsCapturing || BoxHotkey.IsSuppressed(key))
00189: 		{
00190: 			return;
00191: 		}
00192: 		try
00193: 		{
00194: 			if (KeyBindHelper.SuppressHotkeys)
00195: 			{
00196: 				return;
00197: 			}
00198: 		}
00199: 		catch
00200: 		{
00201: 		}
00202: 		try
00203: 		{
00204: 			if (StoreUIManager.IsTextFieldFocused())
00205: 			{
00206: 				return;
00207: 			}
00208: 		}
00209: 		catch
00210: 		{
00211: 		}
00212: 		if (!BoxTransfer.IsInStore())
00213: 		{
00214: 			return;
00215: 		}
00216: 		Core.Debug("全箱上货快捷键 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + " 触发。");
00217: 		BoxItem.ClickSound();
00218: 		try
00219: 		{
00220: 			RunAllBoxes("全箱快捷键");
00221: 		}
00222: 		catch (Exception ex)
00223: 		{
00224: 			Core.Log?.Warning("全箱上货执行异常：" + ex.Message);
00225: 		}
00226: 	}
00227: 
00228: 	internal static void RunOne(GameItem box, string context)
00229: 	{
00230: 		if (box != null && BoxItem.IsBox(box))
00231: 		{
00232: 			RunCore(new List<GameItem> { box }, context, counterScope: false);
00233: 		}
00234: 	}
00235: 
00236: 	internal static void RunAllBoxes(string context)
00237: 	{
00238: 		List<(GameItem, int)> list = new List<(GameItem, int)>();
00239: 		try
00240: 		{
00241: 			foreach (GameItem item2 in BoxItem.FindAllBoxes())
00242: 			{
00243: 				if (item2 != null && BoxItem.IsUsableByPlayer(item2))
00244: 				{
00245: 					list.Add((item2, BoxItem.ReadNumber(item2)));
00246: 				}
00247: 			}
00248: 		}
00249: 		catch (Exception ex)
00250: 		{
00251: 			Core.Log?.Warning("全箱上货[" + context + "] 扫描箱子失败：" + ex.Message);
00252: 			return;
00253: 		}
00254: 		if (list.Count == 0)
00255: 		{
00256: 			BoxItem.NotifyUser("没有可用的收纳箱");
00257: 			return;
00258: 		}
00259: 		list.Sort(delegate((GameItem Box, int Num) a, (GameItem Box, int Num) b)
00260: 		{
00261: 			if (a.Num == b.Num)
00262: 			{
00263: 				return 0;
00264: 			}
00265: 			if (a.Num == -1)
00266: 			{
00267: 				return 1;
00268: 			}
00269: 			return (b.Num == -1) ? (-1) : a.Num.CompareTo(b.Num);
00270: 		});
00271: 		List<GameItem> list2 = new List<GameItem>();
00272: 		int num = 0;
00273: 		foreach (var item3 in list)
00274: 		{
00275: 			GameItem item = item3.Item1;
00276: 			try
00277: 			{
00278: 				if (StorageGrid.GetBoxOption(item, "RESTOCKGUARD") == 1)
00279: 				{
00280: 					num++;
00281: 					continue;
00282: 				}
00283: 			}
00284: 			catch
00285: 			{
00286: 				num++;
00287: 			}
00288: 			list2.Add(item);
00289: 		}
00290: 		if (list2.Count == 0)
00291: 		{
00292: 			BoxItem.NotifyUser("所有收纳箱均设为不参与全箱上架");
00293: 			return;
00294: 		}
00295: 		RunCore(list2, context, counterScope: true);
00296: 		if (num > 0)
00297: 		{
00298: 			Core.Debug("全箱上货[" + context + "]：跳过 " + num + " 个设为不参与的箱。");
00299: 		}
00300: 	}
00301: 
00302: 	private static void RunCore(List<GameItem> boxes, string context, bool counterScope)
00303: 	{
00304: 		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
00305: 		//IL_0082: Invalid comparison between Unknown and I4
00306: 		if (_run != null)
00307: 		{
00308: 			BoxItem.NotifyUser("上货进行中，请稍候");
00309: 			return;
00310: 		}
00311: 		if (!BoxTransfer.IsInStore())
00312: 		{
00313: 			Core.Debug("一键上货[" + context + "]：玩家不在店内，忽略。");
00314: 			BoxItem.NotifyUser("请回到店内再上货");
00315: 			return;
00316: 		}
00317: 		StoreClient val = null;
00318: 		StoreClientInstance val2 = null;
00319: 		try
00320: 		{
00321: 			val2 = PlayerStore.Instance?.currentClientInstance;
00322: 			if (val2 == null)
00323: 			{
00324: 				BoxItem.NotifyUser("必须有顾客在场时才能一键上货");
00325: 				return;
00326: 			}
00327: 			val = val2.GetClientBlueprint();
00328: 			if (val == null)
00329: 			{
00330: 				BoxItem.NotifyUser("必须有顾客在场时才能一键上货");
00331: 				return;
00332: 			}
00333: 			try
00334: 			{
00335: 				if ((int)val.clientIntent != 1)
00336: 				{
00337: 					BoxItem.NotifyUser("该顾客不是来购物的，不支持一键上货");
00338: 					return;
00339: 				}
00340: 			}
00341: 			catch
00342: 			{
00343: 			}
00344: 			if (!val.useClientBudget || val.GetBudget() <= 0)
00345: 			{
00346: 				BoxItem.NotifyUser("该顾客没有预算，不支持一键上货");
00347: 				return;
00348: 			}
00349: 		}
00350: 		catch (Exception ex)
00351: 		{
00352: 			Core.Log?.Warning("一键上货[" + context + "] 顾客判定异常：" + ex.Message);
00353: 			BoxItem.NotifyUser("必须有顾客在场时才能一键上货");
00354: 			return;
00355: 		}
00356: 		GameGridInventory val3 = null;
00357: 		try
00358: 		{
00359: 			EmporiumEntry instance = EmporiumEntry.Instance;
00360: 			val3 = ((instance != null) ? instance.backInvinvElement : null);
00361: 		}
00362: 		catch
00363: 		{
00364: 		}
00365: 		if (val3 == null)
00366: 		{
00367: 			BoxItem.NotifyUser("柜台未就绪，无法上货");
00368: 			return;
00369: 		}
00370: 		int budget = 0;
00371: 		try
00372: 		{
00373: 			budget = val.GetBudget();
00374: 		}
00375: 		catch
00376: 		{
00377: 		}
00378: 		long total = SumCounterValue(val, val3);
00379: 		bool flag = false;
00380: 		try
00381: 		{
00382: 			flag = val.isRequireVariety;
00383: 		}
00384: 		catch
00385: 		{
00386: 		}
00387: 		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
00388: 		HashSet<string> hashSet2 = new HashSet<string>(StringComparer.Ordinal);
00389: 		if (flag)
00390: 		{
00391: 			try
00392: 			{
00393: 				List<GameItem> childItems = ((GameInventory)val3).childItems;
00394: 				if (childItems != null)
00395: 				{
00396: 					foreach (GameItem item in childItems)
00397: 					{
00398: 						if (item == null)
00399: 						{
00400: 							continue;
00401: 						}
00402: 						try
00403: 						{
00404: 							string identifier = item.identifier;
00405: 							if (NullGuard.HasText(identifier))
00406: 							{
00407: 								hashSet2.Add(identifier);
00408: 							}
00409: 						}
00410: 						catch
00411: 						{
00412: 						}
00413: 					}
00414: 				}
00415: 			}
00416: 			catch
00417: 			{
00418: 			}
00419: 		}
00420: 		List<Candidate> list = new List<Candidate>();
00421: 		HashSet<GameItem> hashSet3 = new HashSet<GameItem>();
00422: 		try
00423: 		{
00424: 			foreach (GameItem box in boxes)
00425: 			{
00426: 				for (int i = 0; i < 16; i++)
00427: 				{
00428: 					if (SlotLock.IsLocked(box, i))
00429: 					{
00430: 						continue;
00431: 					}
00432: 					string itemId = StorageGrid.GetItemId(box, i);
00433: 					if (!NullGuard.HasText(itemId) || StorageGrid.GetCount(box, i) <= 0)
00434: 					{
00435: 						continue;
00436: 					}
00437: 					try
00438: 					{
00439: 						if ((StorageGrid.GetItemCapacityUl(box, i) > 0 && !BoxTransfer.TryGetPoolFill(box, i, out var _)) || (StorageGrid.GetEnergyMax(box, i) > 0 && StorageGrid.GetEnergyPool(box, i) <= 0))
00440: 						{
00441: 							continue;
00442: 						}
~~~

~~~csharp
00466: 								flag2 = val.IsClientBuyingThisItem(val4);
00467: 							}
00468: 							catch
00469: 							{
00470: 								flag2 = false;
00471: 							}
00472: 							if (flag2 && flag)
00473: 							{
00474: 								try
00475: 								{
00476: 									flag3 = val.IsItemInVarietyList(val4);
00477: 								}
00478: 								catch
00479: 								{
00480: 									flag3 = false;
00481: 								}
00482: 							}
00483: 							try
00484: 							{
00485: 								val4.Destroy();
00486: 							}
00487: 							catch
00488: 							{
00489: 							}
00490: 						}
00491: 					}
00492: 					catch
00493: 					{
00494: 						flag2 = false;
00495: 					}
00496: 					if (flag2 && (!flag || (!flag3 && !hashSet.Contains(itemId) && !hashSet2.Contains(itemId))))
00497: 					{
00498: 						list.Add(new Candidate
00499: 						{
00500: 							Box = box,
00501: 							Slot = i,
00502: 							ItemId = itemId
00503: 						});
00504: 						hashSet3.Add(box);
00505: 					}
00506: 				}
00507: 			}
00508: 		}
00509: 		catch (Exception ex2)
00510: 		{
00511: 			Core.Log?.Warning("一键上货[" + context + "] 候选收集异常：" + ex2.Message);
00512: 		}
00513: 		if (list.Count == 0)
00514: 		{
00515: 			BoxItem.NotifyUser("没有顾客感兴趣的物品可上货");
00516: 			return;
00517: 		}
00518: 		_run = new RestockRun
00519: 		{
00520: 			ClientInst = val2,
00521: 			BP = val,
00522: 			Counter = val3,
00523: 			Context = context,
00524: 			Scope = (counterScope ? "全箱上货" : "一键上货"),
00525: 			Budget = budget,
00526: 			Total = total,
00527: 			Variety = flag,
00528: 			Queue = list,
00529: 			RestockedIds = hashSet,
00530: 			BoxesTouched = hashSet3
00531: 		};
00532: 	}
00533: 
00534: 	private static void PumpRun()
00535: 	{
00536: 		RestockRun run = _run;
00537: 		if (run == null)
00538: 		{
00539: 			return;
00540: 		}
00541: 		Stopwatch stopwatch = Stopwatch.StartNew();
00542: 		double num;
00543: 		try
00544: 		{
00545: 			num = (double)Time.unscaledDeltaTime * 1000.0 * 0.3;
00546: 		}
00547: 		catch
00548: 		{
00549: 			num = 5.0;
00550: 		}
00551: 		if (num > 8.0)
00552: 		{
00553: 			num = 8.0;
00554: 		}
00555: 		int num2 = 0;
00556: 		try
00557: 		{
00558: 			do
00559: 			{
00560: 				if (num2 > 0 && (stopwatch.Elapsed.TotalMilliseconds >= num || num2 >= 4))
00561: 				{
00562: 					return;
00563: 				}
00564: 				if (!BoxTransfer.IsInStore() || BoxTransfer.PeekStore()?.currentClientInstance != run.ClientInst)
00565: 				{
00566: 					Finish(run, aborted: true);
00567: 					return;
00568: 				}
00569: 				Candidate candidate = null;
00570: 				while (run.Queue.Count > 0)
00571: 				{
00572: 					Candidate candidate2 = run.Queue[0];
00573: 					run.Queue.RemoveAt(0);
00574: 					try
00575: 					{
00576: 						if (StorageGrid.GetCount(candidate2.Box, candidate2.Slot) <= 0 || !NullGuard.HasText(StorageGrid.GetItemId(candidate2.Box, candidate2.Slot)))
00577: 						{
00578: 							continue;
00579: 						}
00580: 						goto IL_00f5;
00581: 					}
00582: 					catch
00583: 					{
00584: 					}
00585: 					continue;
00586: 					IL_00f5:
00587: 					candidate = candidate2;
00588: 					break;
00589: 				}
00590: 				if (candidate == null)
00591: 				{
00592: 					Finish(run);
00593: 					return;
00594: 				}
00595: 				PlaceOne(run, candidate);
00596: 				num2++;
00597: 			}
00598: 			while (!run.CounterFull && !run.BudgetReached);
00599: 			Finish(run);
00600: 		}
00601: 		catch (Exception ex)
00602: 		{
00603: 			Core.Log?.Warning("自动上货[" + run.Context + "] 泵异常中止：" + ex.Message);
00604: 			try
00605: 			{
00606: 				Finish(run, aborted: true);
00607: 			}
00608: 			catch
00609: 			{
00610: 			}
00611: 		}
00612: 	}
00613: 
~~~

~~~csharp
00832: 			else
00833: 			{
00834: 				BoxItem.NotifyUser("自动上货 " + run.Placed + " 件");
00835: 			}
00836: 		}
00837: 		else if (run.CounterFull)
00838: 		{
00839: 			BoxItem.NotifyUser("柜台已满，无法上货");
00840: 		}
00841: 		else
00842: 		{
00843: 			BoxItem.NotifyUser("顾客预算不足，买不起箱中任何物品");
00844: 		}
00845: 	}
00846: 
00847: 	private static long SumCounterValue(StoreClient bp, GameGridInventory counter)
00848: 	{
00849: 		long num = 0L;
00850: 		try
00851: 		{
00852: 			List<GameItem> childItems = ((GameInventory)counter).childItems;
00853: 			if (childItems == null)
00854: 			{
00855: 				return 0L;
00856: 			}
00857: 			foreach (GameItem item in childItems)
00858: 			{
00859: 				if (item == null)
00860: 				{
00861: 					continue;
00862: 				}
00863: 				try
00864: 				{
00865: 					if (bp.IsClientBuyingThisItem(item))
00866: 					{
00867: 						num += item.GetNegociatedValue();
00868: 					}
00869: 				}
00870: 				catch
00871: 				{
00872: 				}
00873: 			}
00874: 		}
00875: 		catch (Exception ex)
00876: 		{
00877: 			Core.Debug("一键上货 柜台总值重算异常：" + ex.Message);
00878: 		}
00879: 		return num;
00880: 	}
00881: }
~~~

### ItemStorage/BoxStowAll.cs

~~~csharp
00042: 			return _key;
00043: 		}
00044: 	}
00045: 
00046: 	private static void LoadKey()
00047: 	{
00048: 		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
00049: 		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
00050: 		try
00051: 		{
00052: 			PlayerStore instance = PlayerStore.Instance;
00053: 			if (instance != null)
00054: 			{
00055: 				_loaded = true;
00056: 				int num = PerSaveState.GetInt(instance, "StowAllHotkey");
00057: 				_key = (KeyCode)((num >= 8 && num <= 350) ? num : 0);
00058: 			}
00059: 		}
00060: 		catch
00061: 		{
00062: 			_loaded = true;
00063: 			_key = (KeyCode)0;
00064: 		}
00065: 	}
00066: 
00067: 	internal static void SetKey(KeyCode key)
00068: 	{
00069: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00070: 		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
00071: 		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
00072: 		//IL_0026: Expected I4, but got Unknown
00073: 		_key = key;
00074: 		_loaded = true;
00075: 		ResetWatch();
00076: 		try
00077: 		{
00078: 			PlayerStore instance = PlayerStore.Instance;
00079: 			if (instance != null)
00080: 			{
00081: 				PerSaveState.SetInt(instance, "StowAllHotkey", (int)key);
00082: 				PerSaveState.Save(instance);
00083: 			}
00084: 		}
00085: 		catch (Exception ex)
00086: 		{
00087: 			Core.Log?.Warning("全箱收纳键位写入存档失败：" + ex.Message);
00088: 		}
00089: 	}
00090: 
00091: 	private static void ClearKey(string reason)
00092: 	{
00093: 		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
00094: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
00095: 		if (_loaded && (int)_key == 0)
00096: 		{
00097: 			return;
00098: 		}
00099: 		_key = (KeyCode)0;
00100: 		_loaded = true;
00101: 		try
00102: 		{
00103: 			PlayerStore instance = PlayerStore.Instance;
00104: 			if (instance != null)
00105: 			{
00106: 				PerSaveState.SetInt(instance, "StowAllHotkey", 0);
00107: 			}
00108: 		}
00109: 		catch
00110: 		{
00111: 		}
00112: 		Core.Log?.Msg("全箱收纳键位已自动解绑：" + reason);
00113: 	}
00114: 
00115: 	internal static void UnbindManually()
00116: 	{
00117: 		ClearKey("玩家在齿轮面板捕获态按 Esc 主动解除绑定");
00118: 	}
00119: 
00120: 	internal static void ResetRecovery()
00121: 	{
00122: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
00123: 		_loaded = false;
00124: 		_key = (KeyCode)0;
00125: 		ResetWatch();
00126: 	}
00127: 
00128: 	internal unsafe static void Tick()
00129: 	{
00130: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00131: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00132: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
00133: 		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
00134: 		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
00135: 		KeyCode key = Key;
00136: 		if ((int)key == 0)
00137: 		{
00138: 			return;
00139: 		}
00140: 		try
00141: 		{
00142: 			if (!Input.GetKeyDown(key))
00143: 			{
00144: 				return;
00145: 			}
00146: 		}
00147: 		catch
00148: 		{
00149: 			return;
00150: 		}
00151: 		if (GearPanel.IsCapturing || BoxHotkey.IsSuppressed(key))
00152: 		{
00153: 			return;
00154: 		}
00155: 		try
00156: 		{
00157: 			if (KeyBindHelper.SuppressHotkeys)
00158: 			{
00159: 				return;
00160: 			}
00161: 		}
00162: 		catch
00163: 		{
00164: 		}
00165: 		try
00166: 		{
00167: 			if (StoreUIManager.IsTextFieldFocused())
00168: 			{
00169: 				return;
00170: 			}
00171: 		}
00172: 		catch
00173: 		{
00174: 		}
00175: 		if (!BoxTransfer.IsInStore())
00176: 		{
00177: 			return;
00178: 		}
00179: 		Core.Debug("全箱收纳快捷键 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + " 触发。");
00180: 		BoxItem.ClickSound();
00181: 		try
00182: 		{
00183: 			RunAll("全箱快捷键");
00184: 		}
00185: 		catch (Exception ex)
00186: 		{
00187: 			Core.Log?.Warning("全箱收纳执行异常：" + ex.Message);
00188: 		}
00189: 	}
00190: 
00191: 	internal static void Watch(int singleBoundCount)
00192: 	{
00193: 		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
00194: 		if ((singleBoundCount == 0 && (int)Key == 0) || Time.unscaledTime < _nextWatch)
00195: 		{
00196: 			return;
00197: 		}
00198: 		_nextWatch = Time.unscaledTime + 2f;
00199: 		try
00200: 		{
00201: 			if (!BoxTransfer.IsInStore())
00202: 			{
00203: 				ResetWatch();
00204: 				return;
00205: 			}
00206: 		}
00207: 		catch
00208: 		{
00209: 			return;
00210: 		}
00211: 		Dictionary<long, GameItem> dictionary = new Dictionary<long, GameItem>();
00212: 		try
00213: 		{
00214: 			if (PlayerStore.Instance == null)
00215: 			{
00216: 				return;
00217: 			}
00218: 			foreach (GameItem item in BoxItem.FindAllBoxes())
00219: 			{
00220: 				if (item != null && BoxItem.IsBox(item) && BoxItem.IsUsableByPlayer(item))
00221: 				{
00222: 					long num;
00223: 					try
00224: 					{
00225: 						num = item.uniqueId;
00226: 					}
00227: 					catch
00228: 					{
00229: 						continue;
00230: 					}
00231: 					if (num != 0L)
00232: 					{
00233: 						dictionary[num] = item;
00234: 					}
00235: 				}
00236: 			}
00237: 		}
00238: 		catch
00239: 		{
00240: 			return;
00241: 		}
00242: 		foreach (KeyValuePair<long, GameItem> item2 in dictionary)
00243: 		{
00244: 			_tracked[item2.Key] = item2.Value;
00245: 		}
00246: 		if (dictionary.Count > 0)
00247: 		{
00248: 			_hadOwner = true;
00249: 		}
00250: 		List<long> list = new List<long>();
00251: 		foreach (KeyValuePair<long, GameItem> item3 in _tracked)
00252: 		{
00253: 			if (!dictionary.ContainsKey(item3.Key) && BoxItem.IsReallyDestroyed(item3.Value))
00254: 			{
00255: 				list.Add(item3.Key);
00256: 			}
00257: 		}
00258: 		foreach (long item4 in list)
00259: 		{
00260: 			_tracked.Remove(item4);
00261: 			BoxHotkey.ForgetUid(item4);
00262: 			Core.Log?.Msg("收纳箱（uid " + item4 + "）已被系统销毁（卖出/垃圾桶清空等），其单箱快捷键已释放。");
00263: 		}
00264: 		if (dictionary.Count > 0 || _tracked.Count > 0)
00265: 		{
00266: 			_zeroStreak = 0;
00267: 		}
00268: 		else if (_hadOwner && ++_zeroStreak >= 5)
00269: 		{
00270: 			ClearKey("玩家拥有的收纳箱已全部被系统销毁（卖出/垃圾桶清空等），释放全箱键供其他功能绑定");
00271: 			BoxRestock.OnAllBoxesDestroyed();
00272: 			_hadOwner = false;
00273: 			_zeroStreak = 0;
00274: 		}
00275: 	}
00276: 
00277: 	private static void ResetWatch()
00278: 	{
00279: 		_tracked.Clear();
00280: 		_hadOwner = false;
00281: 		_zeroStreak = 0;
00282: 	}
00283: 
00284: 	internal static void RunAll(string context)
00285: 	{
00286: 		if (!BoxTransfer.IsInStore())
00287: 		{
00288: 			Core.Debug("全箱收纳[" + context + "]：玩家不在店内，忽略。");
00289: 			BoxItem.NotifyUser("请回到店内再收纳");
00290: 			return;
00291: 		}
00292: 		Stopwatch stopwatch = Stopwatch.StartNew();
00293: 		List<Participant> list = new List<Participant>();
00294: 		try
00295: 		{
00296: 			foreach (GameItem item in BoxItem.FindAllBoxes())
00297: 			{
00298: 				if (item != null && BoxItem.IsUsableByPlayer(item))
00299: 				{
00300: 					list.Add(new Participant
00301: 					{
00302: 						Box = item,
00303: 						Num = BoxItem.ReadNumber(item)
00304: 					});
00305: 				}
00306: 			}
00307: 		}
00308: 		catch (Exception ex)
00309: 		{
00310: 			Core.Log?.Warning("全箱收纳[" + context + "] 扫描箱子失败：" + ex.Message);
00311: 			return;
00312: 		}
00313: 		if (list.Count == 0)
00314: 		{
00315: 			if (_hadOwner && _tracked.Count == 0)
00316: 			{
00317: 				ClearKey("玩家已没有任何拥有的收纳箱（Watch 已确认全销毁，按键兜底清除）");
00318: 			}
00319: 			else
00320: 			{
00321: 				Core.Log?.Msg("全箱收纳[" + context + "]：场景里没有可用的收纳箱（可能拿在手上或不在店内），键位保留。");
00322: 			}
00323: 			return;
00324: 		}
00325: 		list.Sort(delegate(Participant a, Participant b)
00326: 		{
00327: 			if (a.Num == b.Num)
00328: 			{
00329: 				return 0;
00330: 			}
00331: 			if (a.Num == -1)
00332: 			{
00333: 				return 1;
00334: 			}
00335: 			return (b.Num == -1) ? (-1) : a.Num.CompareTo(b.Num);
00336: 		});
00337: 		int num = 0;
00338: 		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
00339: 		foreach (Participant item2 in list)
00340: 		{
00341: 			Dictionary<string, int> dictionary = AutoStow.ReadSpace(item2.Box, context);
00342: 			if (dictionary == null || dictionary.Count == 0)
00343: 			{
00344: 				continue;
00345: 			}
00346: 			item2.Space = dictionary;
00347: 			foreach (string key in dictionary.Keys)
00348: 			{
00349: 				hashSet.Add(key);
00350: 			}
00351: 			num++;
00352: 		}
00353: 		if (num == 0)
00354: 		{
00355: 			Core.Log?.Msg("全箱收纳[" + context + "]：所有收纳箱都是空的，没有可收的目标种类。");
00356: 			BoxItem.NotifyUser("没有可收纳的物品");
00357: 			return;
00358: 		}
00359: 		List<GameItem> list2 = AutoStow.CollectCandidates(hashSet, context);
00360: 		int num2 = 0;
00361: 		int num3 = 0;
00362: 		int num4 = 0;
00363: 		HashSet<GameItem> hashSet2 = new HashSet<GameItem>();
00364: 		foreach (GameItem item3 in list2)
00365: 		{
00366: 			string text;
00367: 			try
00368: 			{
00369: 				text = Core.Clean(item3.identifier) ?? string.Empty;
00370: 			}
00371: 			catch
00372: 			{
00373: 				text = string.Empty;
00374: 			}
00375: 			if (string.IsNullOrEmpty(text) || !hashSet.Contains(text))
00376: 			{
00377: 				continue;
00378: 			}
00379: 			int num5;
00380: 			try
00381: 			{
00382: 				num5 = Math.Max(1, item3.unitCount);
00383: 			}
00384: 			catch
00385: 			{
00386: 				num5 = 1;
00387: 			}
00388: 			bool flag = false;
00389: 			foreach (Participant item4 in list)
00390: 			{
00391: 				Dictionary<string, int> space = item4.Space;
00392: 				if (space != null && space.TryGetValue(text, out var value) && num5 <= value && AutoStow.MoveOneInto(item4.Box, item3, space, context, out var movedUnits, out var _))
00393: 				{
00394: 					num2++;
00395: 					num3 += movedUnits;
00396: 					hashSet2.Add(item4.Box);
00397: 					flag = true;
00398: 					break;
00399: 				}
00400: 			}
00401: 			if (!flag)
00402: 			{
00403: 				num4++;
00404: 			}
00405: 		}
00406: 		Core.Debug("全箱收纳[" + context + "] 耗时 " + stopwatch.ElapsedMilliseconds + " ms（收 " + num2 + " 叠 / 候选 " + list2.Count + " 件 / 参与箱 " + num + "）。");
00407: 		if (num2 > 0)
00408: 		{
00409: 			foreach (GameItem item5 in hashSet2)
00410: 			{
00411: 				try
00412: 				{
00413: 					BoxWindow.RefreshButtons(item5);
00414: 				}
00415: 				catch
00416: 				{
00417: 				}
00418: 			}
00419: 			try
00420: 			{
00421: 				EmporiumEntry instance = EmporiumEntry.Instance;
00422: 				if (instance != null)
00423: 				{
00424: 					instance.Validate(true);
00425: 				}
00426: 			}
00427: 			catch
00428: 			{
00429: 			}
00430: 			Core.Log?.Msg("全箱收纳[" + context + "] 完成：收进 " + num2 + " 叠共 " + num3 + " 件，分入 " + hashSet2.Count + " / " + num + " 个收纳箱" + ((num4 > 0) ? ("，" + num4 + " 叠装不下，留原地") : "") + "。");
00431: 		}
00432: 		else
00433: 		{
00434: 			Core.Log?.Msg("全箱收纳[" + context + "]：店里没有能收进收纳箱的物品" + ((num4 > 0) ? ("（" + num4 + " 叠装不下，留原地）") : "") + "。");
00435: 		}
00436: 		if (num3 > 0)
00437: 		{
00438: 			BoxItem.NotifyUser("已收进 " + num3 + " 件，分派到各收纳箱");
00439: 		}
00440: 		else
00441: 		{
00442: 			BoxItem.NotifyUser("没有可收纳的物品");
00443: 		}
00444: 	}
00445: }
~~~

### ItemStorage/BoxTidy.cs

~~~csharp
00001: using System;
00002: 
00003: namespace ItemStorage;
00004: 
00005: internal static class BoxTidy
00006: {
00007: 	internal static int Tidy(GameItem pack)
00008: 	{
00009: 		int num = 0;
00010: 		try
00011: 		{
00012: 			int num2 = -1;
00013: 			for (int i = 0; i < 16; i++)
00014: 			{
00015: 				if (SlotLock.IsLocked(pack, i))
00016: 				{
00017: 					continue;
00018: 				}
00019: 				if (StorageGrid.GetCount(pack, i) <= 0 || !NullGuard.HasText(StorageGrid.GetItemId(pack, i)))
00020: 				{
00021: 					if (num2 < 0)
00022: 					{
00023: 						num2 = i;
00024: 					}
00025: 				}
00026: 				else if (num2 >= 0 && num2 < i)
00027: 				{
00028: 					if (!StorageGrid.MoveSlot(pack, i, num2))
00029: 					{
00030: 						break;
00031: 					}
00032: 					num++;
00033: 					num2 = FindHole(pack, num2 + 1, i);
00034: 				}
00035: 			}
00036: 		}
00037: 		catch (Exception ex)
00038: 		{
00039: 			Core.Log?.Warning("收纳箱 一键整理：" + ex.Message);
00040: 		}
00041: 		return num;
00042: 	}
00043: 
00044: 	private static int FindHole(GameItem pack, int start, int endInclusive)
00045: 	{
00046: 		for (int i = start; i <= endInclusive; i++)
00047: 		{
00048: 			if (!SlotLock.IsLocked(pack, i) && StorageGrid.GetCount(pack, i) <= 0)
00049: 			{
00050: 				return i;
00051: 			}
00052: 		}
00053: 		return -1;
00054: 	}
00055: }
~~~

### ItemStorage/BoxTooltip.cs

~~~csharp
00060: 			(string, string, int, float)? tuple = TagSnapshot.PeekLastAnimal(pack, slot);
00061: 			if (tuple.HasValue)
00062: 			{
00063: 				try
00064: 				{
00065: 					(string, string, int, float) value = tuple.Value;
00066: 					if (AnimalSpecies.All.TryGetValue(value.Item1, out var value2) && value2 != null)
00067: 					{
00068: 						bool flag2 = string.Equals(value.Item2, "MALE", StringComparison.Ordinal);
00069: 						string localizedMechanic = LocHelper.GetLocalizedMechanic("husbandry_tooltip_header", new object[3]
00070: 						{
00071: 							HusbandryHelper.GetSexeDisplay(flag2 ? "MALE" : "FEMALE"),
00072: 							StageDisplay(value.Item3, value2.growthStages),
00073: 							value2.GetDisplayName()
00074: 						});
00075: 						localizedMechanic = LocGenderHelper.Resolve(localizedMechanic, (LocGender)(flag2 ? 1 : 2));
00076: 						val.AddLine(localizedMechanic, true, (ColorPalette)35216, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00077: 						val.AddLine(LocHelper.GetLocalizedMechanic("husbandry_tooltip_age", new object[2]
00078: 						{
00079: 							value.Item3,
00080: 							Mathf.RoundToInt(value.Item4)
00081: 						}), true, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00082: 					}
00083: 				}
00084: 				catch
00085: 				{
00086: 				}
00087: 				return val.Build(-1).richTextForeground;
00088: 			}
00089: 			if (StorageGrid.IsWaterSlot(pack, slot))
00090: 			{
00091: 				int storedWaterPurity = StorageGrid.GetStoredWaterPurity(pack, slot);
00092: 				string text3 = null;
00093: 				string text4 = ((float)storedWaterPurity / 100f).ToString("F2", CultureInfo.InvariantCulture) + "%";
00094: 				bool flag3 = HasWaterInstrument();
00095: 				try
00096: 				{
00097: 					text3 = (Core.Clean(WaterFeatureHelper.GetWaterDisplayName(storedWaterPurity)) ?? "").TrimEnd('\u3000', ' ');
00098: 					if (string.IsNullOrEmpty(text3))
00099: 					{
00100: 						text3 = null;
00101: 					}
00102: 				}
00103: 				catch (Exception ex)
00104: 				{
00105: 					Core.Debug("收纳箱水格档位名提取失败：" + ex.Message);
00106: 				}
00107: 				if (!string.IsNullOrEmpty(text3))
00108: 				{
00109: 					val.AddLine(text3 + (flag3 ? ("（" + text4 + "）") : ""), true, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00110: 				}
00111: 				if (!NullGuard.HasText(types))
00112: 				{
00113: 					int storedWaterML = StorageGrid.GetStoredWaterML(pack, slot);
00114: 					if (storedWaterML > 0)
00115: 					{
00116: 						val.AddLine(FormatWaterML(storedWaterML), true, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00117: 					}
00118: 				}
00119: 				if (NullGuard.HasText(StorageGrid.GetLiquidSnapshot(pack, slot)) && HasWaterInstrument())
00120: 				{
00121: 					try
00122: 					{
00123: 						string liquidSnapshot = StorageGrid.GetLiquidSnapshot(pack, slot);
00124: 						long num2 = 0L;
00125: 						try
00126: 						{
00127: 							num2 = (long)StorageGrid.GetStoredWaterML(pack, slot) * 1000L;
00128: 						}
00129: 						catch
00130: 						{
00131: 						}
00132: 						string[] array = (liquidSnapshot ?? "").Split(';');
00133: 						foreach (string text5 in array)
00134: 						{
00135: 							int num3 = text5.IndexOf('=');
00136: 							if (num3 <= 0)
00137: 							{
00138: 								continue;
00139: 							}
00140: 							string text6 = text5.Substring(0, num3);
00141: 							if (text6.StartsWith("CURRENT_PART_", StringComparison.Ordinal))
00142: 							{
00143: 								string text7 = text6.Substring("CURRENT_PART_".Length).ToLowerInvariant();
00144: 								string text8;
00145: 								try
00146: 								{
00147: 									text8 = Liquid.GetLiquidFromId(text7)?.displayName ?? text7;
00148: 								}
00149: 								catch
00150: 								{
00151: 									text8 = text7;
00152: 								}
00153: 								int num4 = 0;
00154: 								try
00155: 								{
00156: 									num4 = int.Parse(text5.Substring(num3 + 1), CultureInfo.InvariantCulture);
00157: 								}
00158: 								catch
00159: 								{
00160: 								}
00161: 								string text9;
00162: 								try
00163: 								{
00164: 									text9 = text8 + ": " + WaterScannerUIManager.FormatAmount(num4);
00165: 								}
00166: 								catch
00167: 								{
00168: 									text9 = text8 + ": " + ((double)num4 / 1000.0).ToString("0.##") + " mL";
00169: 								}
00170: 								if (num2 > 0 && num4 > 0)
00171: 								{
00172: 									text9 += string.Format(CultureInfo.InvariantCulture, " ({0:F2}%)", (double)num4 * 100.0 / (double)num2);
00173: 								}
00174: 								val.AddLine("<size=65%>" + text9 + "</size>", true, (ColorPalette)8950922, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00175: 							}
00176: 						}
00177: 					}
00178: 					catch
00179: 					{
00180: 					}
00181: 				}
00182: 			}
00183: 			else
00184: 			{
00185: 				string purity = StorageGrid.GetPurity(pack, slot);
00186: 				if (NullGuard.HasText(purity))
00187: 				{
00188: 					string text10 = (BoxTransfer.PurityShortName(purity, 99) ?? "").TrimEnd('\u3000', ' ');
00189: 					if (!string.IsNullOrEmpty(text10))
00190: 					{
00191: 						val.AddLine(text10, true, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00192: 					}
00193: 				}
00194: 				int energyPool = StorageGrid.GetEnergyPool(pack, slot);
~~~

### ItemStorage/BoxTransfer.cs

~~~csharp
00052: 			TryRegister(dictionary, () => ItemConditionList.CreateJunkGrade());
00053: 			TryRegister(dictionary, () => ItemConditionList.CreateStarchGrade());
00054: 			TryRegister(dictionary, () => ItemConditionList.CreateGenuineInjector(), delegate(GameItem it)
00055: 			{
00056: 				InsInjectorHelper.SetGenuine(it);
00057: 			}, isOfficialHelper: true, () => PreBuiltItemHelper.CreateRealGenuineInjector());
00058: 			TryRegister(dictionary, () => ItemConditionList.CreateCounterfeitInjector(), delegate(GameItem it)
00059: 			{
00060: 				InsInjectorHelper.SetCounterfeit(it);
00061: 			}, isOfficialHelper: true, () => PreBuiltItemHelper.CreateRealCounterfeitInjector());
00062: 			TryRegister(dictionary, () => ItemConditionList.CreateExpiredInjector(), delegate(GameItem it)
00063: 			{
00064: 				InsInjectorHelper.SetExpired(it);
00065: 			}, isOfficialHelper: true, () => PreBuiltItemHelper.CreateRealExpiredInjector());
00066: 			TryRegister(dictionary, () => ItemConditionList.CreateUnusableInjector(), delegate(GameItem it)
00067: 			{
00068: 				InsInjectorHelper.SetUnusable(it);
00069: 			}, isOfficialHelper: true);
00070: 			TryRegister(dictionary, () => ItemConditionList.CreateGenuineCigarette(), delegate(GameItem it)
00071: 			{
00072: 				InsCigaretteHelper.SetGenuine(it);
00073: 			}, isOfficialHelper: true, LaunderGenuineCigarette);
00074: 			TryRegister(dictionary, () => ItemConditionList.CreateCounterfeitCigarette(), delegate(GameItem it)
00075: 			{
00076: 				ItemCondition val = ItemConditionList.CreateCounterfeitCigarette();
00077: 				ItemFeature val2 = StorageGrid.FindConditionFeature(it);
00078: 				if (val2 != null)
00079: 				{
00080: 					val2.SetConditions(val, val);
00081: 				}
00082: 			}, isOfficialHelper: false, () => PreBuiltItemHelper.CounterfeitCigarette());
00083: 			_condMap = dictionary;
00084: 			return dictionary;
00085: 		}
00086: 	}
00087: 
00088: 	internal static void PollAlt()
00089: 	{
00090: 		try
00091: 		{
00092: 			_altState = ((Input.GetKey((KeyCode)308) || Input.GetKey((KeyCode)307)) ? 1 : 0);
00093: 		}
00094: 		catch (Exception ex)
00095: 		{
00096: 			_altState = 0;
00097: 			Core.Log?.Warning("收纳箱 Alt 轮询失败：" + ex.Message);
00098: 		}
00099: 	}
00100: 
00101: 	internal static bool AltHeld()
00102: 	{
00103: 		if (_altState >= 0)
00104: 		{
00105: 			return _altState == 1;
00106: 		}
00107: 		try
00108: 		{
00109: 			return Input.GetKey((KeyCode)308) || Input.GetKey((KeyCode)307);
00110: 		}
00111: 		catch (Exception ex)
00112: 		{
00113: 			Core.Log?.Warning("收纳箱 Alt 读取失败：" + ex.Message);
00114: 			return false;
00115: 		}
00116: 	}
00117: 
00118: 	internal static PlayerStore? PeekStore()
00119: 	{
00120: 		try
00121: 		{
00122: 			object? obj = AccessTools.Field(typeof(PlayerStore), "instance")?.GetValue(null);
00123: 			return (PlayerStore?)((obj is PlayerStore) ? obj : null);
00124: 		}
00125: 		catch
00126: 		{
00127: 			return null;
00128: 		}
00129: 	}
00130: 
00131: 	internal static bool IsInStore()
00132: 	{
00133: 		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
00134: 		try
00135: 		{
00136: 			PlayerStore val = PeekStore();
00137: 			if (val == null)
00138: 			{
00139: 				return false;
00140: 			}
00141: 			if ((int)val.playerLocation != 0)
00142: 			{
00143: 				return false;
00144: 			}
00145: 		}
00146: 		catch
00147: 		{
00148: 		}
00149: 		try
00150: 		{
00151: 			EmporiumEntry instance = EmporiumEntry.Instance;
00152: 			GameGridInventory val2 = ((instance != null) ? instance.backInvinvElement : null);
00153: 			if (val2 == null)
00154: 			{
00155: 				return false;
00156: 			}
00157: 			TreeNodeRender val3 = ((val2 != null) ? ((PixelElement)val2).handler : null);
00158: 			GameObject val4 = (((Object)(object)val3 != (Object)null) ? ((Component)val3).gameObject : null);
00159: 			if ((Object)(object)val4 == (Object)null || !val4.activeInHierarchy)
00160: 			{
00161: 				return false;
00162: 			}
00163: 		}
00164: 		catch
00165: 		{
00166: 		}
00167: 		return true;
00168: 	}
00169: 
00170: 	internal unsafe static GameGridInventory? ResolveCounterDrop()
00171: 	{
00172: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00173: 		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
00174: 		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
00175: 		try
00176: 		{
00177: 			try
00178: 			{
00179: 				PlayerLocation playerLocation = PlayerStore.Instance.playerLocation;
00180: 				if ((int)playerLocation != 0)
00181: 				{
00182: 					Core.Log?.Warning("收纳箱：玩家不在店内（" + ((object)(*(PlayerLocation*)(&playerLocation))/*cast due to .constrained prefix*/).ToString() + "），Alt 直上柜台不可用，本次走原落点");
00183: 					return null;
00184: 				}
00185: 			}
00186: 			catch
00187: 			{
00188: 			}
00189: 			EmporiumEntry instance = EmporiumEntry.Instance;
00190: 			if ((Object)(object)instance == (Object)null)
00191: 			{
00192: 				Core.Log?.Warning("收纳箱：Alt 已按下但店铺场景未就绪，本次走原落点");
00193: 				return null;
00194: 			}
00195: 			GameGridInventory backInvinvElement = instance.backInvinvElement;
00196: 			if (backInvinvElement == null)
00197: 			{
00198: 				Core.Log?.Warning("收纳箱：柜台面 backInvinvElement 不可用（场景未就绪？），本次走原落点");
00199: 				return null;
00200: 			}
00201: 			try
00202: 			{
00203: 				TreeNodeRender val = ((backInvinvElement != null) ? ((PixelElement)backInvinvElement).handler : null);
00204: 				GameObject val2 = (((Object)(object)val != (Object)null) ? ((Component)val).gameObject : null);
00205: 				if ((Object)(object)val2 == (Object)null || !val2.activeInHierarchy)
00206: 				{
00207: 					Core.Log?.Warning("收纳箱：柜台窗口未激活（玩家不在店内），Alt 直上柜台不可用，本次走原落点");
00208: 					return null;
00209: 				}
00210: 			}
00211: 			catch
00212: 			{
00213: 			}
00214: 			return backInvinvElement;
00215: 		}
00216: 		catch (Exception ex)
00217: 		{
00218: 			Core.Log?.Warning("收纳箱：柜台网格读取失败（" + ex.Message + "），本次走原落点");
00219: 			return null;
00220: 		}
00221: 	}
00222: 
00223: 	internal static bool TryDeposit(GameItem pack, GameItem dragged, int context, int preferredSlot = -1, bool consume = true, bool refresh = true, bool notify = true)
00224: 	{
00225: 		if (pack == null || dragged == null)
00226: 		{
00227: 			return false;
00228: 		}
00229: 		if (!BoxItem.IsBox(pack))
00230: 		{
00231: 			return false;
00232: 		}
00233: 		if (!BoxItem.IsUsableByPlayer(pack))
00234: 		{
00235: 			Core.Debug("收纳箱 deposit：目标收纳箱非玩家所有，拒绝存入");
00236: 			if (notify)
00237: 			{
00238: 				BoxItem.NotifyUnowned();
00239: 			}
00240: 			return false;
00241: 		}
00242: 		try
00243: 		{
00244: 			if (!GeneralHelper.IsItemOwned(dragged))
00245: 			{
00246: 				Core.Debug("收纳箱 deposit：物品非玩家所有（未购买的商品），拒绝存入");
00247: 				if (notify)
00248: 				{
00249: 					BoxItem.NotifyUser("请先购买后再存");
00250: 				}
00251: 				return false;
00252: 			}
00253: 		}
00254: 		catch
00255: 		{
00256: 		}
00257: 		string text = Core.Clean(dragged.identifier) ?? string.Empty;
00258: 		if (string.IsNullOrEmpty(text))
00259: 		{
00260: 			return false;
00261: 		}
00262: 		bool flag = true;
00263: 		try
00264: 		{
00265: 			flag = DirectoryMaster.Has<GameItem>(text);
00266: 		}
00267: 		catch
00268: 		{
00269: 			flag = true;
00270: 		}
00271: 		if (!flag)
00272: 		{
00273: 			Core.Debug("收纳箱 deposit：'" + text + "' 不在物品目录中（对应 MOD 可能未加载），拒绝存入");
00274: 			if (notify)
00275: 			{
00276: 				BoxItem.NotifyUser("该物品所属模组未加载，无法存入");
00277: 			}
00278: 			return false;
00279: 		}
00280: 		try
00281: 		{
00282: 			bool? flag2 = BoxItem.IsModuleItem(dragged);
00283: 			if (flag2 == true)
00284: 			{
00285: 				Core.Debug("收纳箱 deposit：'" + text + "' 带【模组】标签（MODULE 类型），拒绝存入");
00286: 				if (notify)
00287: 				{
00288: 					BoxItem.NotifyUser("模组类物品请勿存进收纳箱");
00289: 				}
00290: 				return false;
00291: 			}
00292: 			if (flag2 == false && text.IndexOf("module", StringComparison.OrdinalIgnoreCase) >= 0)
00293: 			{
00294: 				try
00295: 				{
00296: 					List<string> itemTypes = dragged.itemTypes;
00297: 					Core.Debug("收纳箱 deposit 侦查：'" + text + "' 未判中 MODULE，itemTypes=" + ((itemTypes != null) ? itemTypes.ToString() : "null"));
00298: 				}
00299: 				catch
00300: 				{
00301: 				}
00302: 			}
00303: 		}
00304: 		catch
00305: 		{
00306: 		}
00307: 		try
00308: 		{
00309: 			bool? flag3 = BoxItem.IsSupplyCrateItem(dragged);
00310: 			if (flag3 == true)
00311: 			{
00312: 				bool flag4 = true;
00313: 				try
00314: 				{
00315: 					flag4 = dragged.IsGameItemType("SUPPLY_CRATE");
00316: 				}
00317: 				catch
00318: 				{
00319: 					flag4 = true;
00320: 				}
00321: 				if (flag4)
00322: 				{
00323: 					Core.Debug("收纳箱 deposit：'" + text + "' 是锁定物资箱（SUPPLY_CRATE 类型在），拒绝存入");
00324: 					if (notify)
00325: 					{
00326: 						BoxItem.NotifyUser("物资箱请先刷开清空再存");
00327: 					}
00328: 					return false;
00329: 				}
00330: 				Core.Debug("收纳箱 deposit：'" + text + "' 是刷开物资箱（仅 tag 残留），准入交含物闸");
00331: 			}
00332: 			if (flag3 == false && (text.IndexOf("crate", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("_box", StringComparison.OrdinalIgnoreCase) >= 0))
00333: 			{
00334: 				try
00335: 				{
00336: 					List<string> itemTypes2 = dragged.itemTypes;
00337: 					Core.Debug("收纳箱 deposit 侦查：'" + text + "' 未判中 SUPPLY_CRATE，itemTypes=" + ((itemTypes2 != null) ? itemTypes2.ToString() : "null"));
00338: 				}
00339: 				catch
00340: 				{
00341: 				}
00342: 			}
00343: 		}
00344: 		catch
00345: 		{
00346: 		}
00347: 		try
00348: 		{
00349: 			bool? flag5 = BoxItem.IsNodeItem(dragged);
00350: 			if (flag5 == true)
00351: 			{
00352: 				Core.Debug("收纳箱 deposit：'" + text + "' 带【节点】标签（NODE 类型），拒绝存入");
00353: 				if (notify)
00354: 				{
00355: 					BoxItem.NotifyUser("节点类物品请勿存进收纳箱");
00356: 				}
00357: 				return false;
00358: 			}
00359: 			if (flag5 == false && text.IndexOf("node", StringComparison.OrdinalIgnoreCase) >= 0)
00360: 			{
00361: 				try
00362: 				{
00363: 					List<string> itemTypes3 = dragged.itemTypes;
00364: 					Core.Debug("收纳箱 deposit 侦查：'" + text + "' 未判中 NODE，itemTypes=" + ((itemTypes3 != null) ? itemTypes3.ToString() : "null"));
00365: 				}
00366: 				catch
00367: 				{
00368: 				}
00369: 			}
00370: 		}
00371: 		catch
00372: 		{
00373: 		}
00374: 		try
00375: 		{
00376: 			if (dragged.IsGameItemType("STORAGE"))
00377: 			{
00378: 				Core.Debug("收纳箱 deposit：储藏装置（STORAGE）不能存进收纳箱");
00379: 				if (notify)
00380: 				{
00381: 					BoxItem.NotifyUser("储藏装置请勿放进收纳箱");
00382: 				}
00383: 				return false;
00384: 			}
00385: 		}
00386: 		catch
00387: 		{
00388: 		}
00389: 		if (string.Equals(text, "item_storage", StringComparison.Ordinal))
00390: 		{
00391: 			Core.Debug("收纳箱 deposit：容器不能存进收纳箱");
00392: 			if (notify)
00393: 			{
00394: 				BoxItem.NotifyUser("储藏装置请勿放进收纳箱");
00395: 			}
00396: 			return false;
00397: 		}
00398: 		try
00399: 		{
00400: 			if (dragged.IsGameItemType("MACHINE"))
00401: 			{
00402: 				Core.Debug("收纳箱 deposit：机器不能存进收纳箱");
00403: 				if (notify)
00404: 				{
00405: 					BoxItem.NotifyUser("机器请勿存进收纳箱");
00406: 				}
00407: 				return false;
00408: 			}
00409: 		}
00410: 		catch
00411: 		{
00412: 		}
00413: 		try
00414: 		{
00415: 			if (dragged.IsTag("TOOLBOX_TAG"))
00416: 			{
00417: 				Core.Debug("收纳箱 deposit：工具箱不能存进收纳箱");
00418: 				if (notify)
00419: 				{
00420: 					BoxItem.NotifyUser("工具箱请勿收进收纳箱");
00421: 				}
00422: 				return false;
00423: 			}
00424: 		}
00425: 		catch
00426: 		{
00427: 		}
00428: 		try
00429: 		{
00430: 			if (StolenHelper.IsStolenItem(dragged))
00431: 			{
00432: 				Core.Debug("收纳箱 deposit：赃物（带热度标记）不能存进收纳箱");
00433: 				if (notify)
00434: 				{
00435: 					BoxItem.NotifyUser("赃物请勿存进收纳箱");
00436: 				}
00437: 				return false;
00438: 			}
00439: 		}
00440: 		catch
00441: 		{
00442: 		}
00443: 		try
00444: 		{
00445: 			if (WineHelper.IsFermentingOrFinished(dragged))
00446: 			{
00447: 				Core.Log?.Msg("收纳箱 deposit：酿造中/陈酿的酒不能存进收纳箱 id=" + text);
00448: 				if (notify)
00449: 				{
00450: 					BoxItem.NotifyUser("酿造中或陈酿的酒不能存");
00451: 				}
00452: 				return false;
00453: 			}
00454: 		}
00455: 		catch
00456: 		{
00457: 		}
00458: 		try
00459: 		{
00460: 			bool flag6 = false;
00461: 			try
00462: 			{
00463: 				flag6 = WaterFeatureHelper.GetWaterFeature(dragged) != null;
00464: 			}
00465: 			catch
00466: 			{
00467: 			}
00468: 			bool flag7 = true;
00469: 			try
00470: 			{
00471: 				flag7 = WaterHelper.IsEmpty(dragged);
00472: 			}
00473: 			catch
00474: 			{
00475: 			}
00476: 			if (flag6 && !flag7 && preferredSlot < 0)
00477: 			{
00478: 				Core.Log?.Msg("收纳箱 deposit：含水水容器拒绝一键收纳（拖到面板格子松手可存）id=" + text);
00479: 				if (notify)
00480: 				{
00481: 					BoxItem.NotifyUser("含水容器请拖到面板格子存放");
00482: 				}
00483: 				return false;
00484: 			}
00485: 		}
00486: 		catch
00487: 		{
00488: 		}
00489: 		try
00490: 		{
00491: 			if (HasContentInside(dragged))
00492: 			{
00493: 				Core.Log?.Msg("收纳箱 deposit：容器内有物品，拒收（内容物保护）id=" + text);
00494: 				if (notify)
00495: 				{
00496: 					BoxItem.NotifyUser("物品内部还放着东西，请先清空再存");
00497: 				}
00498: 				return false;
00499: 			}
00500: 		}
00501: 		catch
00502: 		{
00503: 		}
00504: 		try
00505: 		{
00506: 			RealizeTruthBeforeDeposit(dragged);
00507: 		}
00508: 		catch (Exception ex)
00509: 		{
00510: 			Core.Debug("收纳箱 deposit 实化外层异常：" + ex.Message);
00511: 		}
00512: 		int num = 1;
00513: 		try
00514: 		{
00515: 			num = Math.Max(1, dragged.unitCount);
00516: 		}
00517: 		catch
00518: 		{
00519: 		}
00520: 		string text2;
00521: 		try
00522: 		{
00523: 			text2 = ReadItemSignature(dragged);
00524: 		}
00525: 		catch
00526: 		{
00527: 			text2 = "";
00528: 		}
00529: 		try
00530: 		{
00531: 			if (StorageGrid.ReadItemEnergy(dragged).HasValue)
00532: 			{
00533: 				text2 = null;
00534: 			}
00535: 		}
00536: 		catch
00537: 		{
00538: 		}
00539: 		try
00540: 		{
00541: 			if (StorageGrid.ReadItemUseCount(dragged).isUse)
00542: 			{
00543: 				text2 = null;
00544: 			}
00545: 		}
00546: 		catch
00547: 		{
00548: 		}
00549: 		bool flag8 = false;
00550: 		try
00551: 		{
00552: 			if (TagSnapshot.HasIndividualState(dragged))
00553: 			{
00554: 				text2 = null;
00555: 				flag8 = true;
00556: 			}
00557: 		}
00558: 		catch
00559: 		{
00560: 		}
00561: 		bool flag9 = false;
00562: 		try
00563: 		{
00564: 			flag9 = TagSnapshot.IsDead(dragged);
00565: 		}
00566: 		catch
00567: 		{
00568: 		}
00569: 		string text3 = null;
00570: 		try
00571: 		{
00572: 			text3 = Core.Clean(dragged.spritePath);
00573: 		}
00574: 		catch
00575: 		{
00576: 		}
00577: 		string text4 = null;
00578: 		try
00579: 		{
00580: 			text4 = TryReadDepositName(dragged, text);
00581: 		}
00582: 		catch
00583: 		{
00584: 		}
00585: 		bool flag10 = false;
00586: 		try
00587: 		{
00588: 			flag10 = StorageGrid.ReadItemEnergy(dragged).HasValue;
00589: 		}
00590: 		catch
00591: 		{
00592: 		}
00593: 		string text5 = null;
00594: 		try
00595: 		{
00596: 			text5 = TryReadCompSignature(dragged);
00597: 		}
00598: 		catch
00599: 		{
00600: 		}
00601: 		if (context > 0)
00602: 		{
00603: 			try
00604: 			{
00605: 				BoxWindow.EnsureWindow(pack, "Deposit");
00606: 			}
00607: 			catch (Exception ex2)
00608: 			{
00609: 				Core.Log?.Warning("收纳箱 EnsureWindow deposit：" + ex2.Message);
00610: 			}
00611: 		}
00612: 		int num2 = num;
00613: 		int num3 = 0;
00614: 		try
00615: 		{
00616: 			string itemId;
00617: 			int count;
00618: 			if (StorageGrid.IsValidSlot(preferredSlot))
00619: 			{
00620: 				itemId = StorageGrid.GetItemId(pack, preferredSlot);
00621: 				count = StorageGrid.GetCount(pack, preferredSlot);
00622: 				if (itemId == null)
00623: 				{
00624: 					goto IL_0694;
00625: 				}
00626: 				if (string.Equals(itemId, text, StringComparison.Ordinal))
00627: 				{
00628: 					bool num4;
00629: 					if (text2 != null)
00630: 					{
00631: 						num4 = string.Equals(StorageGrid.GetSlotSignature(pack, preferredSlot), text2, StringComparison.Ordinal);
00632: 					}
00633: 					else
00634: 					{
00635: 						if (!flag8 || StorageGrid.GetSlotDeadFlag(pack, preferredSlot) != flag9 || (!flag10 && !string.Equals(StorageGrid.GetSpritePath(pack, preferredSlot), text3, StringComparison.Ordinal)) || !string.Equals(StorageGrid.GetDisplayName(pack, preferredSlot), text4, StringComparison.Ordinal))
00636: 						{
00637: 							goto IL_0a0f;
00638: 						}
00639: 						num4 = string.Equals(StorageGrid.GetCompSig(pack, preferredSlot) ?? "", text5 ?? "", StringComparison.Ordinal);
00640: 					}
00641: 					if (num4)
00642: 					{
00643: 						goto IL_0694;
00644: 					}
00645: 				}
00646: 			}
00647: 			goto IL_0a0f;
00648: 			IL_0694:
00649: 			if (count < 99)
00650: 			{
00651: 				int num5 = Math.Min(99 - count, num2);
00652: 				if (num5 > 0)
00653: 				{
00654: 					StorageGrid.SetSlot(pack, preferredSlot, text, count + num5);
00655: 					if (itemId == null)
00656: 					{
00657: 						StorageGrid.SetDisplayName(pack, preferredSlot, TryReadDepositName(dragged, text));
00658: 						StorageGrid.SetCompSig(pack, preferredSlot, text5);
00659: 						long? unitValue = TryReadUnitValue(dragged);
00660: 						StorageGrid.SetUnitValue(pack, preferredSlot, unitValue);
00661: 						StorageGrid.SetCalorie(pack, preferredSlot, StorageGrid.ReadItemCalorie(dragged));
00662: 						StorageGrid.SetShape(pack, preferredSlot, StorageGrid.ReadItemShape(dragged));
00663: 						StorageGrid.SetSpriteRef(pack, preferredSlot, TryReadSpriteAtlas(dragged), TryReadSpritePath(dragged));
00664: 						StorageGrid.SetTypes(pack, preferredSlot, StorageGrid.ReadItemTypes(dragged));
00665: 						Core.Debug("收纳箱 deposit 贴图记录：en=" + (StorageGrid.ReadItemEnergy(dragged)?.ToString() ?? "无") + " atlas='" + (TryReadSpriteAtlas(dragged) ?? "-") + "' path='" + (TryReadSpritePath(dragged) ?? "-") + "'");
00666: 						StorageGrid.SetPurity(pack, preferredSlot, IsEmptyWaterItem(dragged) ? null : StorageGrid.ReadItemPurity(dragged));
00667: 						StorageGrid.SetSlotCustomName(pack, preferredSlot, (WaterFeatureHelper.GetWaterFeature(dragged) != null) ? Core.Clean(GeneralHelper.GetCustomName(dragged)) : null);
00668: 						InitEnergySlotDisplay(pack, preferredSlot, dragged, text);
00669: 						StorageGrid.SetSnapshotQueue(pack, preferredSlot, null);
00670: 						StorageGrid.SetSlotDeadFlag(pack, preferredSlot, flag9);
00671: 						StorageGrid.SetCrateOpenFlag(pack, preferredSlot, BoxItem.IsSupplyCrateItem(dragged) == true);
00672: 					}
00673: 					DepositPool(pack, preferredSlot, dragged, num5);
00674: 					num2 -= num5;
00675: 					num3 += num5;
00676: 				}
00677: 			}
00678: 			goto IL_0a0f;
00679: 			IL_0a0f:
00680: 			while (num2 > 0)
00681: 			{
00682: 				int num6 = (flag8 ? StorageGrid.FindIndividualSlot(pack, text, flag9, text4, text3, !flag10, text5) : StorageGrid.FindSlotWithSpace(pack, text, text2));
00683: 				if (num6 < 0)
00684: 				{
00685: 					num6 = StorageGrid.FindEmptySlot(pack);
00686: 				}
00687: 				if (num6 >= 0)
00688: 				{
00689: 					int count2 = StorageGrid.GetCount(pack, num6);
00690: 					int num7 = Math.Min(99 - count2, num2);
00691: 					if (num7 > 0)
00692: 					{
00693: 						StorageGrid.SetSlot(pack, num6, text, count2 + num7);
00694: 						if (count2 <= 0)
00695: 						{
00696: 							StorageGrid.SetSlotDeadFlag(pack, num6, flag9);
00697: 							StorageGrid.SetCompSig(pack, num6, text5);
00698: 							StorageGrid.SetDisplayName(pack, num6, TryReadDepositName(dragged, text));
00699: 							long? unitValue2 = TryReadUnitValue(dragged);
00700: 							StorageGrid.SetUnitValue(pack, num6, unitValue2);
00701: 							StorageGrid.SetCalorie(pack, num6, StorageGrid.ReadItemCalorie(dragged));
00702: 							StorageGrid.SetShape(pack, num6, StorageGrid.ReadItemShape(dragged));
00703: 							StorageGrid.SetSpriteRef(pack, num6, TryReadSpriteAtlas(dragged), TryReadSpritePath(dragged));
00704: 							StorageGrid.SetTypes(pack, num6, StorageGrid.ReadItemTypes(dragged));
00705: 							Core.Debug("收纳箱 deposit 贴图记录：en=" + (StorageGrid.ReadItemEnergy(dragged)?.ToString() ?? "无") + " atlas='" + (TryReadSpriteAtlas(dragged) ?? "-") + "' path='" + (TryReadSpritePath(dragged) ?? "-") + "'");
00706: 							StorageGrid.SetPurity(pack, num6, IsEmptyWaterItem(dragged) ? null : StorageGrid.ReadItemPurity(dragged));
00707: 							StorageGrid.SetSlotCustomName(pack, num6, (WaterFeatureHelper.GetWaterFeature(dragged) != null) ? Core.Clean(GeneralHelper.GetCustomName(dragged)) : null);
00708: 							InitEnergySlotDisplay(pack, num6, dragged, text);
00709: 							StorageGrid.SetSnapshotQueue(pack, num6, null);
00710: 							StorageGrid.SetCrateOpenFlag(pack, num6, BoxItem.IsSupplyCrateItem(dragged) == true);
00711: 						}
00712: 						DepositPool(pack, num6, dragged, num7);
00713: 						num2 -= num7;
00714: 						num3 += num7;
00715: 						continue;
00716: 					}
00717: 					break;
00718: 				}
00719: 				break;
00720: 			}
00721: 		}
00722: 		catch (Exception ex3)
00723: 		{
00724: 			Core.Log?.Warning("收纳箱 deposit 分配异常：" + ex3.Message);
00725: 		}
00726: 		if (num3 <= 0)
00727: 		{
00728: 			Core.Debug("收纳箱 deposit '" + text + "'：格子已满或无法分配，未存入");
00729: 			if (notify)
00730: 			{
00731: 				BoxItem.NotifyUser("收纳箱满了");
00732: 			}
00733: 			return false;
00734: 		}
00735: 		int num8 = StorageGrid.TotalCount(pack, text);
00736: 		if (consume)
00737: 		{
00738: 			ConsumeDragged(dragged, num3, num);
00739: 		}
00740: 		if (refresh)
00741: 		{
00742: 			BoxWindow.RefreshButtons(pack);
00743: 			BoxWindow.UpdateTooltip(pack);
00744: 		}
00745: 		Core.Debug("收纳箱 deposit '" + text + "' +" + num3 + " → 全盒 " + num8 + "（手上剩 " + num2 + "）");
00746: 		return true;
00747: 	}
00748: 
00749: 	internal static string SpaceShort(bool counter)
00750: 	{
00751: 		if (!counter)
00752: 		{
00753: 			return "箱子所在空间不足";
00754: 		}
00755: 		return "柜台空间不足";
00756: 	}
00757: 
00758: 	internal static WithdrawResult TryWithdraw(GameItem pack, int slot, GameGridInventory? dropGrid = null, bool refresh = true)
00759: 	{
00760: 		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
00761: 		if (pack == null || !StorageGrid.IsValidSlot(slot))
00762: 		{
00763: 			return WithdrawResult.Empty;
00764: 		}
00765: 		if (!BoxItem.IsBox(pack))
00766: 		{
00767: 			return WithdrawResult.Empty;
00768: 		}
00769: 		if (dropGrid != null)
00770: 		{
00771: 			bool flag = false;
00772: 			try
00773: 			{
00774: 				flag = !IsInStore();
00775: 			}
00776: 			catch
00777: 			{
00778: 			}
00779: 			if (flag)
00780: 			{
00781: 				BoxItem.NotifyUser("店外无法取出到柜台，请回到店内再取");
00782: 				return WithdrawResult.SilentFail;
00783: 			}
00784: 		}
00785: 		if (!BoxItem.IsUsableByPlayer(pack))
00786: 		{
00787: 			Core.Debug("收纳箱 withdraw：收纳箱非玩家所有，拒绝取出");
00788: 			return WithdrawResult.Empty;
00789: 		}
00790: 		string itemId = StorageGrid.GetItemId(pack, slot);
00791: 		if (!NullGuard.HasText(itemId))
00792: 		{
00793: 			try
00794: 			{
00795: 				StorageGrid.ClearSlot(pack, slot);
00796: 			}
00797: 			catch
00798: 			{
00799: 			}
00800: 			Core.Debug("收纳箱 withdraw 槽 " + slot + "：空槽（无货可取），忽略。");
00801: 			return WithdrawResult.Empty;
00802: 		}
00803: 		int count = StorageGrid.GetCount(pack, slot);
00804: 		if (count <= 0)
00805: 		{
00806: 			Core.Debug("收纳箱 withdraw 槽 " + slot + " '" + itemId + "'：数量为 0");
00807: 			return WithdrawResult.Empty;
00808: 		}
00809: 		GameGridInventory val = null;
00810: 		try
00811: 		{
00812: 			GameInventory parentInventory = pack.parentInventory;
00813: 			val = (GameGridInventory)(object)((parentInventory is GameGridInventory) ? parentInventory : null);
00814: 		}
00815: 		catch
00816: 		{
00817: 			val = null;
00818: 		}
00819: 		if (val == null && dropGrid == null)
00820: 		{
00821: 			Core.Debug("收纳箱 withdraw：盒子不在网格上，无法同网格发放");
00822: 			return WithdrawResult.NotOnGrid;
00823: 		}
00824: 		GameItem item = null;
00825: 		string text = null;
00826: 		bool flag2 = false;
00827: 		int v = 0;
00828: 		int v2 = 0;
00829: 		try
00830: 		{
00831: 			item = DirectoryMaster.Item(itemId, true);
00832: 			if (item == null)
00833: 			{
00834: 				bool flag3 = true;
00835: 				try
00836: 				{
00837: 					flag3 = DirectoryMaster.Has<GameItem>(itemId);
00838: 				}
00839: 				catch
00840: 				{
00841: 					flag3 = true;
00842: 				}
00843: 				Core.Log?.Warning("收纳箱 withdraw：'" + itemId + "' 重建失败（目录登记=" + (flag3 ? "有" : "无，所属 MOD 可能未加载") + "，槽位保留不扣账）");
00844: 				if (!flag3)
00845: 				{
00846: 					BoxItem.NotifyUser("该物品所属模组未加载，暂时无法取出");
00847: 				}
00848: 				return WithdrawResult.SilentFail;
00849: 			}
00850: 			try
00851: 			{
00852: 				GeneralHelper.SetItemOwned(item, true);
00853: 			}
00854: 			catch
00855: 			{
00856: 			}
00857: 			try
00858: 			{
00859: 				item.unitCount = 1;
00860: 			}
00861: 			catch
00862: 			{
00863: 			}
00864: 			bool flag4 = false;
00865: 			try
00866: 			{
00867: 				flag4 = WaterFeatureHelper.GetWaterFeature(item) != null;
00868: 			}
00869: 			catch
00870: 			{
00871: 			}
00872: 			bool flag5 = false;
00873: 			try
00874: 			{
00875: 				string purity = StorageGrid.GetPurity(pack, slot);
00876: 				GameItem val2 = TryBuildFactoryItem(itemId, purity);
00877: 				if (val2 != null)
00878: 				{
00879: 					try
00880: 					{
00881: 						item.Destroy();
00882: 					}
00883: 					catch
00884: 					{
00885: 					}
00886: 					item = val2;
00887: 					flag5 = true;
00888: 					try
00889: 					{
00890: 						GeneralHelper.SetItemOwned(item, true);
00891: 					}
00892: 					catch
00893: 					{
00894: 					}
00895: 					try
00896: 					{
00897: 						item.unitCount = 1;
00898: 					}
00899: 					catch
00900: 					{
00901: 					}
00902: 					try
00903: 					{
00904: 						flag4 = WaterFeatureHelper.GetWaterFeature(item) != null;
00905: 					}
00906: 					catch
00907: 					{
00908: 					}
00909: 				}
00910: 			}
00911: 			catch (Exception ex)
00912: 			{
00913: 				Core.Debug("收纳箱 withdraw 工厂直发异常：" + ex.Message + "（退重建路径）");
00914: 			}
00915: 			string displayName = StorageGrid.GetDisplayName(pack, slot);
00916: 			if (!string.IsNullOrEmpty(displayName) && !flag4 && !flag5)
00917: 			{
00918: 				try
00919: 				{
00920: 					item.name = displayName;
00921: 				}
00922: 				catch
00923: 				{
00924: 				}
00925: 			}
00926: 			string spriteAtlas = StorageGrid.GetSpriteAtlas(pack, slot);
00927: 			bool flag6 = false;
00928: 			try
00929: 			{
00930: 				flag6 = StorageGrid.ReadItemEnergy(item).HasValue;
00931: 			}
00932: 			catch
00933: 			{
00934: 			}
00935: 			if (!string.IsNullOrEmpty(spriteAtlas) && !flag6 && !flag4)
00936: 			{
00937: 				try
00938: 				{
00939: 					item.spriteAtlasPath = spriteAtlas;
00940: 				}
00941: 				catch
00942: 				{
00943: 				}
00944: 			}
00945: 			string text2 = StorageGrid.GetSpritePath(pack, slot);
00946: 			try
00947: 			{
00948: 				string text3 = TagSnapshot.PeekLastSprite(pack, slot);
00949: 				if (!string.IsNullOrEmpty(text3))
00950: 				{
00951: 					text2 = text3;
00952: 				}
00953: 			}
00954: 			catch
00955: 			{
00956: 			}
00957: 			bool flag7 = true;
00958: 			if (!string.IsNullOrEmpty(text2) && !flag6 && !flag4)
00959: 			{
00960: 				flag7 = BoxWindow.IsSpriteResolvable(string.IsNullOrEmpty(spriteAtlas) ? null : spriteAtlas, text2);
00961: 				if (!flag7)
00962: 				{
00963: 					Core.Debug("收纳箱 withdraw 贴图：键解析失败跳过写回（防覆盖原生贴图）atlas='" + (Core.Clean(spriteAtlas) ?? "-") + "' spr='" + (text2 ?? "-") + "'");
00964: 				}
00965: 			}
00966: 			if (!string.IsNullOrEmpty(text2) && !flag6 && !flag4 && flag7)
00967: 			{
00968: 				bool flag8 = false;
00969: 				try
00970: 				{
00971: 					item.SetSprite(Core.Clean(spriteAtlas), Core.Clean(text2));
00972: 					flag8 = true;
00973: 				}
00974: 				catch
00975: 				{
00976: 				}
00977: 				if (!flag8)
00978: 				{
00979: 					try
00980: 					{
00981: 						item.spriteAtlasPath = spriteAtlas;
00982: 					}
00983: 					catch
00984: 					{
00985: 					}
00986: 					try
00987: 					{
00988: 						item.spritePath = text2;
00989: 					}
00990: 					catch
00991: 					{
00992: 					}
00993: 				}
00994: 				try
00995: 				{
00996: 					int uniqueId = item.uniqueId;
00997: 					Core.Debug("收纳箱 withdraw 贴图登记：uid=" + uniqueId + " atlas='" + (Core.Clean(spriteAtlas) ?? "-") + "' spr='" + (text2 ?? "-") + "' setSprite=" + flag8);
00998: 					if (uniqueId != 0)
00999: 					{
01000: 						StoragePatch.RegisterWithdrawSprite(uniqueId, Core.Clean(spriteAtlas) ?? "", Core.Clean(text2) ?? "");
01001: 					}
01002: 				}
01003: 				catch
01004: 				{
01005: 				}
01006: 			}
01007: 			long? unitValue = StorageGrid.GetUnitValue(pack, slot);
01008: 			bool flag9 = false;
01009: 			try
01010: 			{
01011: 				flag9 = StorageGrid.IsWaterSlot(pack, slot) && StorageGrid.GetStoredWaterML(pack, slot) > 0;
01012: 			}
01013: 			catch
01014: 			{
01015: 			}
01016: 			if (!flag9)
01017: 			{
01018: 				try
01019: 				{
01020: 					ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(item);
01021: 					if (waterFeature != null)
01022: 					{
01023: 						bool flag10 = false;
01024: 						GameItem val3 = null;
01025: 						try
01026: 						{
01027: 							val3 = DirectoryMaster.Item(itemId, true);
01028: 						}
01029: 						catch
01030: 						{
01031: 						}
01032: 						try
01033: 						{
01034: 							flag10 = val3 != null && WaterFeatureHelper.GetWaterFeature(val3) != null;
01035: 						}
01036: 						catch
01037: 						{
01038: 						}
01039: 						try
01040: 						{
01041: 							if (val3 != null)
01042: 							{
01043: 								val3.Destroy();
01044: 							}
01045: 						}
01046: 						catch
01047: 						{
01048: 						}
01049: 						if (!flag10)
01050: 						{
01051: 							string text4 = "-";
01052: 							try
01053: 							{
01054: 								text4 = Core.Clean(waterFeature.identifier) ?? "-";
01055: 							}
01056: 							catch
01057: 							{
01058: 							}
01059: 							try
01060: 							{
01061: 								item.RemoveItemFeatureByID(text4);
01062: 							}
01063: 							catch
01064: 							{
01065: 							}
01066: 							try
01067: 							{
01068: 								WaterFeatureHelper.InitWaterFeature(item, false);
01069: 							}
01070: 							catch
01071: 							{
01072: 							}
01073: 							Core.Debug("收纳箱 feat 残留重置：'" + itemId + "' featId='" + text4 + "'（出厂无 feat 而现物有，按水容器正规化）");
01074: 						}
01075: 					}
01076: 				}
01077: 				catch
01078: 				{
01079: 				}
01080: 			}
01081: 			if (!flag9 && !flag5)
01082: 			{
01083: 				if (unitValue.HasValue && unitValue.Value > 0)
01084: 				{
01085: 					try
01086: 					{
01087: 						item.unitValue = unitValue.Value;
01088: 					}
01089: 					catch
01090: 					{
01091: 					}
01092: 				}
01093: 				else
01094: 				{
01095: 					long num = -1L;
01096: 					try
01097: 					{
01098: 						GameItem val4 = DirectoryMaster.Item(itemId, true);
01099: 						if (val4 != null)
01100: 						{
01101: 							try
01102: 							{
01103: 								num = val4.unitValue;
01104: 							}
01105: 							catch
01106: 							{
01107: 							}
01108: 							try
01109: 							{
01110: 								val4.Destroy();
01111: 							}
01112: 							catch
01113: 							{
01114: 							}
01115: 						}
01116: 					}
01117: 					catch
01118: 					{
01119: 					}
01120: 					if (num > 0)
01121: 					{
01122: 						try
01123: 						{
01124: 							item.unitValue = num;
01125: 						}
01126: 						catch
01127: 						{
01128: 						}
01129: 						Core.Debug("收纳箱 withdraw 价值兜底：'" + itemId + "' 存入记录 " + unitValue.GetValueOrDefault() + " → 出厂模板价 " + num);
01130: 					}
01131: 				}
01132: 			}
01133: 			int? calorie = StorageGrid.GetCalorie(pack, slot);
01134: 			if (calorie.HasValue && calorie.Value > 0)
01135: 			{
01136: 				StorageGrid.WriteItemCalorie(item, calorie.Value);
01137: 			}
01138: 			string shape = StorageGrid.GetShape(pack, slot);
01139: 			if (NullGuard.HasText(shape))
01140: 			{
01141: 				string[] array = shape.Split(',');
01142: 				if (array.Length == 2 && int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2) && result > 0 && result2 > 0)
01143: 				{
01144: 					try
01145: 					{
01146: 						item.SetShape(new GridShapeBuilder(result, result2).SetDataFill(result, result2, (byte)1).Build());
01147: 					}
01148: 					catch (Exception ex2)
01149: 					{
01150: 						Core.Debug("收纳箱 withdraw：占格还原失败 " + shape + "：" + ex2.Message);
01151: 					}
01152: 				}
01153: 			}
01154: 			string purity2 = StorageGrid.GetPurity(pack, slot);
01155: 			if (NullGuard.HasText(purity2) && !flag4 && !flag5)
01156: 			{
01157: 				RestoreCondition(item, purity2);
01158: 			}
01159: 			if (StorageGrid.GetCrateOpenFlag(pack, slot))
01160: 			{
01161: 				try
01162: 				{
01163: 					try
01164: 					{
01165: 						item.ModifyTag("LOCK_STATUS", (Action<TagState>)delegate(TagState t)
01166: 						{
01167: 							t.Enable().SetBool(false);
01168: 						}, false);
01169: 					}
01170: 					catch (Exception ex3)
01171: 					{
01172: 						Core.Debug("收纳箱 crate 还原 LOCK_STATUS：" + ex3.Message);
01173: 					}
01174: 					try
01175: 					{
01176: 						if (item.contentWindow != null)
01177: 						{
01178: 							item.contentWindow.isLocked = false;
01179: 						}
01180: 					}
01181: 					catch
01182: 					{
01183: 					}
01184: 					try
01185: 					{
01186: 						string text5 = item.spritePath ?? "";
01187: 						if (!string.IsNullOrEmpty(text5) && !text5.EndsWith("_u", StringComparison.Ordinal))
01188: 						{
01189: 							item.SetSpriteAndShape(item.spriteAtlasPath, text5 + "_u");
01190: 						}
01191: 					}
01192: 					catch (Exception ex4)
01193: 					{
01194: 						Core.Debug("收纳箱 crate 还原贴图：" + ex4.Message);
01195: 					}
01196: 					try
01197: 					{
01198: 						item.RemoveGameItemType("SUPPLY_CRATE");
01199: 					}
01200: 					catch (Exception ex5)
01201: 					{
01202: 						Core.Debug("收纳箱 crate 还原类型：" + ex5.Message);
01203: 					}
01204: 					try
01205: 					{
01206: 						item.EnableTag("IMPORTANT_TAG", false);
01207: 					}
01208: 					catch (Exception ex6)
01209: 					{
01210: 						Core.Debug("收纳箱 crate 还原 IMPORTANT：" + ex6.Message);
01211: 					}
01212: 					try
01213: 					{
01214: 						item.unitValue = 15L;
01215: 					}
01216: 					catch
01217: 					{
01218: 					}
01219: 					try
01220: 					{
01221: 						MetaHelper.RemoveIgnoreChildValue(item);
01222: 					}
01223: 					catch
01224: 					{
01225: 					}
01226: 					try
01227: 					{
01228: 						if (NullGuard.HasText(item.name))
01229: 						{
01230: 							item.name = item.name.Replace("(" + LocHelper.GetLocalizedMechanic("mech_lock_sealed", Array.Empty<object>()) + ")", "").Trim();
01231: 						}
01232: 					}
01233: 					catch
01234: 					{
01235: 					}
01236: 					try
01237: 					{
01238: 						List<GameItem> list = item.FindAllChildItems();
01239: 						if (list != null && list.Count > 0)
01240: 						{
01241: 							List<GameItem> list2 = new List<GameItem>(list);
01242: 							foreach (GameItem item2 in list2)
01243: 							{
01244: 								try
01245: 								{
01246: 									if (item2 != null)
01247: 									{
01248: 										item2.Destroy();
01249: 									}
01250: 								}
01251: 								catch
01252: 								{
01253: 								}
01254: 							}
01255: 							Core.Log?.Msg("收纳箱 crate 还原：清除重建预填内部货 x" + list2.Count);
01256: 						}
01257: 					}
01258: 					catch
01259: 					{
01260: 					}
01261: 					Core.Debug("收纳箱 crate 还原完成：'" + itemId + "' → 刷开空箱");
01262: 				}
01263: 				catch (Exception ex7)
01264: 				{
01265: 					Core.Debug("收纳箱 crate 还原异常：" + ex7.Message);
01266: 				}
01267: 			}
01268: 			TryRestoreBoughtPrice(pack, slot, item);
01269: 			if (flag4)
01270: 			{
01271: 				try
01272: 				{
01273: 					int itemCapacityUl = StorageGrid.GetItemCapacityUl(pack, slot);
01274: 					if (itemCapacityUl > 0)
01275: 					{
01276: 						TagSystem state = item.state;
01277: 						TagState val5 = ((state != null) ? state.GetTag("LIQUID_CONTAINER_CAPACITY") : null);
01278: 						if (val5 != null)
01279: 						{
01280: 							val5.SetEnabled(true);
01281: 							val5.valueInt = itemCapacityUl;
01282: 							try
01283: 							{
01284: 								item.ValidateShapeState();
01285: 							}
01286: 							catch
01287: 							{
01288: 							}
01289: 							Core.Debug("收纳箱 withdraw 容量还原：" + itemCapacityUl / 1000 + "ml（槽 " + slot + "）");
01290: 						}
01291: 					}
01292: 				}
01293: 				catch
01294: 				{
01295: 				}
01296: 				try
01297: 				{
01298: 					string slotCustomName = StorageGrid.GetSlotCustomName(pack, slot);
01299: 					if (!string.IsNullOrEmpty(slotCustomName))
01300: 					{
01301: 						GeneralHelper.SetCustomName(item, slotCustomName);
01302: 						Core.Debug("收纳箱 withdraw 改名水还原：'" + slotCustomName + "'（槽 " + slot + "）");
01303: 					}
01304: 					else if (GeneralHelper.HasCustomName(item))
01305: 					{
01306: 						GeneralHelper.RemoveCustomName(item);
01307: 						Core.Debug("收纳箱 withdraw 改名水清残留（槽 " + slot + "）");
01308: 					}
01309: 				}
01310: 				catch
01311: 				{
01312: 				}
01313: 				int storedWaterML = StorageGrid.GetStoredWaterML(pack, slot);
01314: 				int storedWaterPurity = StorageGrid.GetStoredWaterPurity(pack, slot);
01315: 				if (storedWaterML > 0)
01316: 				{
01317: 					bool flag11 = false;
01318: 					try
01319: 					{
01320: 						string liquidSnapshot = StorageGrid.GetLiquidSnapshot(pack, slot);
01321: 						if (!string.IsNullOrEmpty(liquidSnapshot))
01322: 						{
01323: 							Dictionary<string, long> dictionary = ParsePool(liquidSnapshot);
01324: 							long num2 = 0L;
01325: 							foreach (long value6 in dictionary.Values)
01326: 							{
01327: 								num2 += value6;
01328: 							}
01329: 							long num3 = -1L;
01330: 							try
01331: 							{
01332: 								TagSystem state2 = item.state;
01333: 								TagState val6 = ((state2 == null) ? null : state2.dict?["LIQUID_CONTAINER_CAPACITY"]);
01334: 								if (val6 != null && val6.valueInt > 0)
01335: 								{
01336: 									num3 = val6.valueInt;
01337: 								}
01338: 							}
01339: 							catch
01340: 							{
01341: 							}
01342: 							if (num3 <= 0)
01343: 							{
01344: 								num3 = num2;
01345: 							}
01346: 							long num4 = Math.Min(num2, num3);
01347: 							int num5 = (int)(num4 / 1000);
01348: 							Dictionary<string, long> dictionary2 = new Dictionary<string, long>();
01349: 							long num6 = 0L;
01350: 							foreach (KeyValuePair<string, long> item3 in dictionary)
01351: 							{
01352: 								long num7 = ((num2 > 0) ? (item3.Value * num4 / num2) : 0);
01353: 								if (num7 > 0)
01354: 								{
01355: 									dictionary2[item3.Key] = num7;
01356: 								}
01357: 								num6 += num7;
01358: 							}
01359: 							long num8 = num4 - num6;
01360: 							if (num8 != 0L)
01361: 							{
01362: 								dictionary2["CURRENT_PART_WATER"] = (dictionary2.TryGetValue("CURRENT_PART_WATER", out var value) ? value : 0) + num8;
01363: 							}
01364: 							long value2;
01365: 							long num9 = (dictionary2.TryGetValue("CURRENT_PART_WATER", out value2) ? value2 : 0);
01366: 							int num10 = (int)((num4 > 0) ? (num9 * 10000 / num4) : 0);
01367: 							TagSystem state3 = item.state;
01368: 							Dictionary<string, TagState> dictionary3 = ((state3 != null) ? state3.dict : null);
01369: 							int num11 = 0;
01370: 							if (dictionary3 != null)
01371: 							{
01372: 								foreach (KeyValuePair<string, TagState> item4 in dictionary3)
01373: 								{
01374: 									if (item4.Key == null || !item4.Key.StartsWith("CURRENT_PART_"))
01375: 									{
01376: 										continue;
01377: 									}
01378: 									try
01379: 									{
01380: 										TagState value3 = item4.Value;
01381: 										if (value3 != null)
01382: 										{
01383: 											value3.SetInt(0);
01384: 										}
01385: 									}
01386: 									catch
01387: 									{
01388: 									}
01389: 								}
01390: 								foreach (KeyValuePair<string, long> item5 in dictionary2)
01391: 								{
01392: 									if (item5.Value <= 0)
01393: 									{
01394: 										continue;
01395: 									}
01396: 									try
~~~

~~~csharp
01424: 							catch
01425: 							{
01426: 							}
01427: 							try
01428: 							{
01429: 								WaterFeatureHelper.UpdateWaterFeatureFake(item, num10);
01430: 							}
01431: 							catch
01432: 							{
01433: 							}
01434: 							try
01435: 							{
01436: 								WaterHelper.UpdateContainerGoodType(item);
01437: 							}
01438: 							catch
01439: 							{
01440: 							}
01441: 							try
01442: 							{
01443: 								WaterFeatureHelper.GetWaterFeature(item).customIntValue1 = num10;
01444: 							}
01445: 							catch
01446: 							{
01447: 							}
01448: 							int num12 = Rd();
01449: 							int num13 = Rp();
01450: 							flag11 = num12 == num5 && Math.Abs(num13 - num10) <= 5;
01451: 							if (flag11)
01452: 							{
01453: 								foreach (KeyValuePair<string, long> item6 in dictionary2)
01454: 								{
01455: 									if (dictionary.TryGetValue(item6.Key, out var value4))
01456: 									{
01457: 										dictionary[item6.Key] = value4 - item6.Value;
01458: 									}
01459: 								}
01460: 								long num14 = 0L;
01461: 								foreach (long value7 in dictionary.Values)
01462: 								{
01463: 									num14 += value7;
01464: 								}
01465: 								text = SerializePool(dictionary);
01466: 								flag2 = num14 <= 0;
01467: 								v = (int)(num14 / 1000);
01468: 								v2 = (int)((!flag2) ? ((dictionary.TryGetValue("CURRENT_PART_WATER", out var value5) ? value5 : 0) * 10000 / ((num14 > 0) ? num14 : 1)) : 0);
01469: 								Core.Debug("收纳箱水还原成功：切 " + num5 + "ml（池余 " + num14 / 1000 + "ml）→ ml=" + num12 + " pur=" + num13 + "/" + num10 + " 键 " + num11);
01470: 							}
01471: 							else
01472: 							{
01473: 								Core.Log?.Msg("收纳箱水还原失败：切 " + num5 + "ml 写瓶读回 ml=" + num12 + "/" + num5 + " pur=" + num13 + "/" + num10 + "（拒绝取出，池保留）");
01474: 							}
01475: 						}
01476: 						else
01477: 						{
01478: 							try
01479: 							{
01480: 								WaterHelper.AddWater(item, 0, storedWaterML * 1000, false, 0, 0, false);
01481: 							}
01482: 							catch
01483: 							{
01484: 							}
01485: 							try
01486: 							{
01487: 								WaterFeatureHelper.OnLiquidChanged(item, false);
01488: 							}
01489: 							catch
01490: 							{
01491: 							}
01492: 							try
01493: 							{
01494: 								WaterFeatureHelper.UpdateWaterFeature(item, false);
01495: 							}
01496: 							catch
01497: 							{
01498: 							}
01499: 							try
01500: 							{
01501: 								WaterFeatureHelper.UpdateWaterFeatureFake(item, storedWaterPurity);
01502: 							}
01503: 							catch
01504: 							{
01505: 							}
01506: 							try
01507: 							{
01508: 								WaterHelper.UpdateContainerGoodType(item);
01509: 							}
01510: 							catch
01511: 							{
01512: 							}
01513: 							try
01514: 							{
01515: 								WaterFeatureHelper.GetWaterFeature(item).customIntValue1 = storedWaterPurity;
01516: 							}
01517: 							catch
01518: 							{
01519: 							}
01520: 							int num15 = Rd();
01521: 							flag11 = num15 == storedWaterML;
01522: 							Core.Log?.Msg(flag11 ? ("收纳箱水还原(旧格兼容)：ml=" + num15 + "/" + storedWaterML + " pur=" + Rp() + "/" + storedWaterPurity + "（纯度官方随机，近似）") : ("收纳箱水还原失败：AddWater→ml=" + num15 + "/" + storedWaterML + "（拒绝取出，槽位保留）"));
01523: 							if (flag11)
01524: 							{
01525: 								flag2 = true;
01526: 							}
01527: 						}
01528: 					}
01529: 					catch (Exception ex8)
01530: 					{
01531: 						Core.Log?.Msg("收纳箱水还原异常：" + ex8.Message + "（拒绝取出，池保留）");
01532: 					}
01533: 					if (!flag11)
01534: 					{
01535: 						try
01536: 						{
01537: 							item.Destroy();
01538: 						}
01539: 						catch
01540: 						{
01541: 						}
01542: 						return WithdrawResult.SilentFail;
01543: 					}
01544: 					Core.Debug("收纳箱 withdraw：水还原 " + storedWaterML + "ml 纯度 " + storedWaterPurity);
01545: 				}
01546: 				else
01547: 				{
01548: 					int num16 = -1;
01549: 					try
01550: 					{
01551: 						num16 = WaterHelper.GetCurrentCapacityML(item);
01552: 					}
01553: 					catch
01554: 					{
01555: 					}
01556: 					if (num16 > 0)
01557: 					{
01558: 						try
01559: 						{
01560: 							WaterHelper.EmptyContainer(item);
01561: 						}
01562: 						catch
01563: 						{
01564: 						}
01565: 						Core.Debug("收纳箱 withdraw：空瓶格重建瓶异常带水 " + num16 + "ml，已清空");
01566: 					}
01567: 				}
01568: 			}
01569: 			try
01570: 			{
01571: 				string text6 = Core.Clean(item.spritePath) ?? "-";
01572: 				string text7 = "-";
01573: 				try
01574: 				{
01575: 					text7 = item.unitValue.ToString();
01576: 				}
01577: 				catch
01578: 				{
01579: 				}
01580: 				string text8 = StorageGrid.ReadItemCalorie(item)?.ToString() ?? "-";
01581: 				string text9 = "-";
01582: 				try
01583: 				{
01584: 					text9 = ChemicalProductHelper.GetCurrentChemicalConditionId(item) ?? "-";
01585: 				}
01586: 				catch
01587: 				{
01588: 				}
01589: 				Core.Debug("收纳箱 withdraw 外观写回：name='" + (displayName ?? "-") + "' spr='" + (text2 ?? "-") + "→" + text6 + "' baseVal=" + (unitValue?.ToString() ?? "-") + "→" + text7 + " cal=" + (calorie?.ToString() ?? "-") + "→" + text8 + " cond='" + (purity2 ?? "-") + "→" + text9 + "'");
01590: 			}
01591: 			catch
01592: 			{
01593: 			}
01594: 			GameGridInventory val7 = dropGrid ?? val;
01595: 			int num17 = 0;
01596: 			try
01597: 			{
01598: 				GraphNodeStorage val8 = (GraphNodeStorage)(object)val7;
01599: 				num17 = ((val8 == null) ? GraphUtils.TryAcceptAllMid(val7, item, -1) : GraphUtils.TryAcceptAll(val8, item, -1));
01600: 			}
01601: 			catch (Exception ex9)
01602: 			{
01603: 				Core.Debug("收纳箱 TryAcceptAll 不可用（" + ex9.Message + "），退回 Mid");
01604: 				try
01605: 				{
01606: 					num17 = GraphUtils.TryAcceptAllMid(val7, item, -1);
01607: 				}
01608: 				catch (Exception ex10)
01609: 				{
01610: 					Core.Log?.Warning("收纳箱 TryAcceptAllMid：" + ex10.Message);
01611: 				}
01612: 			}
01613: 			bool flag12 = false;
01614: 			try
01615: 			{
01616: 				GameInventory parentInventory2 = item.parentInventory;
01617: 				GameGridInventory val9 = (GameGridInventory)(object)((parentInventory2 is GameGridInventory) ? parentInventory2 : null);
01618: 				flag12 = val9 != null && val9 == val7;
01619: 			}
01620: 			catch
01621: 			{
01622: 				flag12 = false;
01623: 			}
01624: 			if (!flag12)
01625: 			{
01626: 				Core.Debug("收纳箱 withdraw '" + itemId + "'：同网格无空位（TryAcceptAllMid=" + num17 + "），不扣库存");
01627: 				try
01628: 				{
01629: 					item.Destroy();
01630: 				}
01631: 				catch
01632: 				{
01633: 				}
01634: 				return WithdrawResult.NoSpace;
01635: 			}
01636: 			string text10 = null;
01637: 			try
01638: 			{
01639: 				text10 = TagSnapshot.PopAndRestore(pack, slot, item);
01640: 			}
01641: 			catch (Exception ex11)
01642: 			{
01643: 				Core.Debug("收纳箱 withdraw 快照还原异常：" + ex11.Message);
01644: 			}
01645: 			ReplayLoad("机器升级", delegate
01646: 			{
01647: 				MachineHelper.LoadUpgrade(item);
01648: 			});
01649: 			ReplayLoad("水容器贴图", delegate
01650: 			{
01651: 				SpriteHelper.UpdateWaterContainerSprite(item);
01652: 			});
01653: 			ReplayLoad("锁定物品", delegate
01654: 			{
01655: 				GeneralHelper.LoadLockItem(item);
01656: 			});
01657: 			ReplayLoad("可食产出", delegate
01658: 			{
01659: 				PlantTraitList.LoadEdibleProduce(item);
01660: 			});
01661: 			ReplayLoad("水培口味", delegate
01662: 			{
01663: 				PlantTraitList.LoadTasteFeature(item);
01664: 			});
01665: 			ReplayLoad("非自有工具", delegate
01666: 			{
01667: 				GeneralHelper.LoadNonOwnedToolTag(item);
01668: 			});
01669: 			ReplayLoad("危废标签", delegate
01670: 			{
01671: 				GeneralHelper.LoadHazardousWasteTag(item);
01672: 			});
01673: 			ReplayLoad("锁定容器", delegate
01674: 			{
01675: 				LockHelper.LoadLockedContainer(item);
01676: 			});
01677: 			ReplayLoad("损坏机器", delegate
01678: 			{
01679: 				MachineBrokenHelper.LoadBrokenMachine(item);
01680: 			});
01681: 			ReplayLoad("机器OnLoad", delegate
01682: 			{
01683: 				MachineHelper.OnLoad(item);
01684: 			});
01685: 			ReplayLoad("涡轮资格", delegate
01686: 			{
01687: 				MachineTurboBoosterAdv.LoadTurboBoostEligibleTag(item);
01688: 			});
01689: 			ReplayLoad("转换器", delegate
01690: 			{
01691: 				CurrentConverterHelper.OnLoad(item);
01692: 			});
01693: 			try
01694: 			{
01695: 				((GameItemFunc)item).onLoaded?.Invoke(item);
01696: 			}
01697: 			catch (Exception ex12)
01698: 			{
01699: 				Core.Debug("收纳箱 withdraw onLoaded 回调失败：" + ex12.Message);
01700: 			}
01701: 			try
01702: 			{
01703: 				string text11 = Core.Clean(item.spritePath);
01704: 				string text12 = null;
01705: 				try
01706: 				{
01707: 					text12 = HusbandryHelper.GetAnimalSprite(item);
01708: 				}
01709: 				catch
01710: 				{
01711: 				}
01712: 				bool flag13 = HusbandryHelper.TryChangeSprite(item);
01713: 				GameItem obj66 = item;
01714: 				GameItemElement val10 = (GameItemElement)(object)((obj66 is GameItemElement) ? obj66 : null);
01715: 				if (val10 != null)
01716: 				{
01717: 					try
01718: 					{
01719: 						val10.ValidateSelect(true);
01720: 					}
01721: 					catch
01722: 					{
01723: 					}
01724: 				}
01725: 				Core.Debug("收纳箱贴图档：'" + (Core.Clean(item.identifier) ?? "?") + "' 前=" + (text11 ?? "null") + " 官方算=" + (text12 ?? "null") + " changed=" + flag13 + " 后=" + (Core.Clean(item.spritePath) ?? "null") + " 是元素=" + (item is GameItemElement));
01726: 			}
01727: 			catch (Exception ex13)
01728: 			{
01729: 				Core.Debug("收纳箱 withdraw 贴图档重解析异常：" + ex13.Message);
01730: 			}
01731: 			try
01732: 			{
01733: 				if (NullGuard.HasText(text10) && count - 1 > 0)
01734: 				{
01735: 					string text13 = null;
01736: 					string[] array2 = text10.Split('\u0001');
01737: 					foreach (string text14 in array2)
01738: 					{
01739: 						if (!string.IsNullOrEmpty(text14))
01740: 						{
01741: 							text13 = text14;
01742: 						}
01743: 					}
01744: 					if (NullGuard.HasText(text13))
01745: 					{
01746: 						array2 = text13.Split('\u0003');
01747: 						for (int num18 = 0; num18 < array2.Length; num18++)
01748: 						{
01749: 							string[] array3 = array2[num18].Split('\u0002');
01750: 							if (array3.Length >= 10 && !string.IsNullOrEmpty(array3[^1]))
01751: 							{
01752: 								if (!string.Equals(StorageGrid.GetSpritePath(pack, slot), array3[^1], StringComparison.Ordinal))
01753: 								{
01754: 									StorageGrid.SetSpriteRef(pack, slot, StorageGrid.GetSpriteAtlas(pack, slot), array3[^1]);
01755: 								}
01756: 								break;
01757: 							}
01758: 						}
01759: 					}
01760: 				}
01761: 			}
01762: 			catch (Exception ex14)
01763: 			{
01764: 				Core.Debug("收纳箱 withdraw 显示顺延异常：" + ex14.Message);
01765: 			}
01766: 			AllocatePools(pack, slot, item);
01767: 			if (flag2)
01768: 			{
01769: 				StorageGrid.SetLiquidSnapshot(pack, slot, null);
01770: 				StorageGrid.SetStoredWaterML(pack, slot, 0);
01771: 				StorageGrid.SetStoredWaterPurity(pack, slot, 0);
01772: 			}
01773: 			else if (text != null)
01774: 			{
01775: 				StorageGrid.SetLiquidSnapshot(pack, slot, text);
01776: 				StorageGrid.SetStoredWaterML(pack, slot, v);
01777: 				StorageGrid.SetStoredWaterPurity(pack, slot, v2);
01778: 			}
01779: 			int num19 = count - 1;
01780: 			if (num19 > 0)
01781: 			{
01782: 				StorageGrid.SetSlot(pack, slot, itemId, num19);
01783: 			}
01784: 			else
01785: 			{
01786: 				StorageGrid.ClearSlot(pack, slot);
01787: 			}
01788: 			if (text10 != null)
01789: 			{
01790: 				TagSnapshot.SetQueue(pack, slot, text10);
01791: 			}
01792: 			if (refresh)
01793: 			{
01794: 				BoxWindow.RefreshButtons(pack);
01795: 				BoxWindow.UpdateTooltip(pack);
01796: 			}
01797: 			Core.Debug("收纳箱 withdraw 槽 " + slot + " '" + itemId + "' OK → 余 " + num19 + "（同网格发放）");
01798: 			return WithdrawResult.Ok;
01799: 		}
01800: 		catch (Exception ex15)
01801: 		{
01802: 			Core.Log?.Warning("收纳箱 TryWithdraw：" + ex15.Message);
01803: 			try
01804: 			{
01805: 				if (item != null && item.parentInventory == null)
01806: 				{
01807: 					item.Destroy();
01808: 				}
01809: 			}
01810: 			catch
01811: 			{
01812: 			}
01813: 			return WithdrawResult.SilentFail;
01814: 		}
01815: 		int Rd()
01816: 		{
01817: 			try
01818: 			{
01819: 				return WaterHelper.GetCurrentCapacityML(item);
01820: 			}
01821: 			catch
01822: 			{
01823: 				return -1;
01824: 			}
01825: 		}
01826: 		static void ReplayLoad(string name, Action call)
01827: 		{
01828: 			try
01829: 			{
01830: 				call();
01831: 			}
01832: 			catch (Exception ex16)
01833: 			{
01834: 				Core.Debug("收纳箱 withdraw " + name + "重建失败：" + ex16.Message);
01835: 			}
01836: 		}
01837: 		int Rp()
01838: 		{
01839: 			try
01840: 			{
01841: 				return WaterHelper.GetWaterPurity(item);
01842: 			}
01843: 			catch
01844: 			{
01845: 				return -1;
01846: 			}
01847: 		}
01848: 	}
01849: 
01850: 	private static void DepositPool(GameItem pack, int slot, GameItem dragged, int put)
01851: 	{
01852: 		try
01853: 		{
01854: 			string text = Core.Clean(dragged.spritePath);
01855: 			if (!string.IsNullOrEmpty(text) && !string.Equals(StorageGrid.GetSpritePath(pack, slot), text, StringComparison.Ordinal))
01856: 			{
01857: 				StorageGrid.SetSpriteRef(pack, slot, Core.Clean(dragged.spriteAtlasPath), text);
01858: 			}
01859: 		}
01860: 		catch
01861: 		{
01862: 		}
01863: 		try
01864: 		{
01865: 			if (put > 1)
01866: 			{
01867: 				string text2 = TagSnapshot.Capture(dragged);
01868: 				if (text2 != null)
01869: 				{
01870: 					for (int i = 0; i < put; i++)
01871: 					{
01872: 						TagSnapshot.Enqueue(pack, slot, text2);
01873: 					}
01874: 					Core.Debug("收纳箱 deposit 堆叠快照入队：槽 " + slot + " '" + (Core.Clean(dragged.identifier) ?? "?") + "' put=" + put + " 同条 ×" + put + "（条长 " + text2.Length + "）");
01875: 				}
01876: 				else
01877: 				{
01878: 					Core.Debug("收纳箱快照：put=" + put + "（同质无状态堆叠，不拍）'" + (Core.Clean(dragged.identifier) ?? "?") + "'");
01879: 				}
01880: 			}
01881: 			else if (put == 1)
01882: 			{
01883: 				string text3 = TagSnapshot.Capture(dragged);
01884: 				if (text3 != null)
01885: 				{
01886: 					TagSnapshot.Enqueue(pack, slot, text3);
01887: 					Core.Debug("收纳箱 deposit 快照入队：槽 " + slot + " '" + (Core.Clean(dragged.identifier) ?? "?") + "' 长度 " + text3.Length);
01888: 				}
01889: 			}
01890: 		}
01891: 		catch (Exception ex)
01892: 		{
01893: 			Core.Debug("收纳箱 deposit 快照异常：" + ex.Message);
01894: 		}
01895: 		try
01896: 		{
01897: 			TagSnapshot.EnqueueChemParts(pack, slot, dragged, put);
01898: 		}
01899: 		catch (Exception ex2)
01900: 		{
01901: 			Core.Debug("收纳箱 deposit 组成入队异常：" + ex2.Message);
01902: 		}
01903: 		try
01904: 		{
01905: 			int? num = StorageGrid.ReadItemEnergy(dragged);
01906: 			if (num.HasValue)
01907: 			{
01908: 				int num2 = num.Value * put;
01909: 				int v = StorageGrid.GetEnergyPool(pack, slot) + num2;
01910: 				StorageGrid.SetEnergyPool(pack, slot, v);
01911: 				Core.Debug("收纳箱 deposit 池：电量 +" + num2 + "（单件 " + num.Value + " × " + put + "）→ 格池 " + v);
01912: 				RefreshEnergySlotSprite(pack, slot);
01913: 			}
01914: 		}
01915: 		catch
01916: 		{
01917: 		}
01918: 		try
01919: 		{
01920: 			int? num3 = StorageGrid.ReadItemDurability(dragged);
01921: 			if (num3.HasValue)
01922: 			{
01923: 				int num4 = num3.Value * put;
01924: 				int v2 = StorageGrid.GetDurabilityPool(pack, slot) + num4;
01925: 				StorageGrid.SetDurabilityPool(pack, slot, v2);
01926: 				Core.Debug("收纳箱 deposit 池：耐久 +" + num4 + " → 格池 " + v2);
01927: 			}
01928: 		}
01929: 		catch
01930: 		{
01931: 		}
01932: 		try
01933: 		{
01934: 			var (flag, num5, num6) = StorageGrid.ReadItemUseCount(dragged);
01935: 			if (flag)
01936: 			{
01937: 				int num7 = num5 * put;
01938: 				int v3 = StorageGrid.GetUseCountPool(pack, slot) + num7;
01939: 				StorageGrid.SetUseCountPool(pack, slot, v3);
01940: 				if (num6 > 0)
01941: 				{
01942: 					StorageGrid.SetUseCountMax(pack, slot, num6);
01943: 				}
01944: 				Core.Debug("收纳箱 deposit 池：次数 +" + num7 + "（单件 " + num5 + "/" + num6 + " × " + put + "）→ 格池 " + v3);
01945: 			}
01946: 		}
01947: 		catch
01948: 		{
01949: 		}
01950: 		try
01951: 		{
01952: 			int num8 = 0;
01953: 			try
01954: 			{
01955: 				TagSystem state = dragged.state;
01956: 				num8 = ((state == null) ? null : state.dict?["LIQUID_CONTAINER_CAPACITY"])?.valueInt ?? 0;
01957: 			}
01958: 			catch
01959: 			{
01960: 			}
01961: 			if (num8 > 0)
01962: 			{
01963: 				StorageGrid.SetItemCapacityUl(pack, slot, num8);
01964: 			}
01965: 			else
01966: 			{
01967: 				StorageGrid.ClearItemCapacityUl(pack, slot);
01968: 			}
01969: 			bool flag2 = false;
01970: 			try
01971: 			{
01972: 				flag2 = WaterFeatureHelper.GetWaterFeature(dragged) != null;
01973: 			}
01974: 			catch
01975: 			{
01976: 			}
01977: 			if (flag2)
01978: 			{
01979: 				StorageGrid.SetWaterSlot(pack, slot);
01980: 				bool flag3 = true;
01981: 				try
01982: 				{
01983: 					flag3 = WaterHelper.IsEmpty(dragged);
01984: 				}
01985: 				catch
01986: 				{
01987: 				}
01988: 				if (flag3)
01989: 				{
01990: 					if (StorageGrid.GetLiquidSnapshot(pack, slot) == null)
01991: 					{
01992: 						StorageGrid.SetStoredWaterML(pack, slot, 0);
01993: 						StorageGrid.SetStoredWaterPurity(pack, slot, 0);
01994: 					}
01995: 					return;
01996: 				}
01997: 				TagSystem state2 = dragged.state;
01998: 				Dictionary<string, TagState> dictionary = ((state2 != null) ? state2.dict : null);
01999: 				if (dictionary == null)
02000: 				{
02001: 					return;
02002: 				}
02003: 				Dictionary<string, long> dictionary2 = ParsePool(StorageGrid.GetLiquidSnapshot(pack, slot));
02004: 				int num9 = 0;
02005: 				foreach (KeyValuePair<string, TagState> item in dictionary)
02006: 				{
02007: 					if (item.Key != null && item.Key.StartsWith("CURRENT_PART_"))
02008: 					{
02009: 						int num10 = 0;
02010: 						try
02011: 						{
02012: 							num10 = item.Value?.valueInt ?? 0;
02013: 						}
02014: 						catch
02015: 						{
02016: 						}
02017: 						if (num10 > 0)
02018: 						{
02019: 							dictionary2[item.Key] = (dictionary2.TryGetValue(item.Key, out var value) ? value : 0) + num10;
02020: 							num9 += num10;
02021: 						}
02022: 					}
02023: 				}
02024: 				if (num9 <= 0)
02025: 				{
02026: 					return;
02027: 				}
02028: 				long num11 = 0L;
02029: 				foreach (long value3 in dictionary2.Values)
02030: 				{
02031: 					num11 += value3;
02032: 				}
02033: 				long value2;
02034: 				long num12 = (dictionary2.TryGetValue("CURRENT_PART_WATER", out value2) ? value2 : 0);
02035: 				StorageGrid.SetLiquidSnapshot(pack, slot, SerializePool(dictionary2));
02036: 				StorageGrid.SetStoredWaterML(pack, slot, (int)(num11 / 1000));
02037: 				StorageGrid.SetStoredWaterPurity(pack, slot, (int)((num11 > 0) ? (num12 * 10000 / num11) : 0));
02038: 				Core.Debug("收纳箱 deposit 池累加：+" + num9 + "µL → 池总 " + num11 / 1000 + "ml（槽 " + slot + "）");
02039: 			}
02040: 			else
02041: 			{
02042: 				StorageGrid.ClearWaterTags(pack, slot);
02043: 			}
02044: 		}
02045: 		catch
02046: 		{
02047: 		}
02048: 	}
02049: 
02050: 	private static Dictionary<string, long> ParsePool(string? snap)
02051: 	{
02052: 		Dictionary<string, long> dictionary = new Dictionary<string, long>();
02053: 		if (!NullGuard.HasText(snap))
02054: 		{
02055: 			return dictionary;
02056: 		}
02057: 		string[] array = snap.Split(';');
02058: 		foreach (string text in array)
02059: 		{
02060: 			int num = text.IndexOf('=');
02061: 			if (num > 0 && long.TryParse(text.Substring(num + 1), out var result))
02062: 			{
02063: 				string key = text.Substring(0, num);
02064: 				dictionary[key] = (dictionary.TryGetValue(key, out var value) ? value : 0) + result;
02065: 			}
02066: 		}
02067: 		return dictionary;
02068: 	}
02069: 
02070: 	private static string[] GetWaterCondIds()
02071: 	{
02072: 		if (_waterCondIds != null)
02073: 		{
02074: 			return _waterCondIds;
02075: 		}
02076: 		List<string> list = new List<string>();
02077: 		try
02078: 		{
02079: 			list.Add(Core.Clean(ItemConditionList.CreatePureWater()?.identifier) ?? "");
02080: 			list.Add(Core.Clean(ItemConditionList.CreateHighQualityWater()?.identifier) ?? "");
02081: 			list.Add(Core.Clean(ItemConditionList.CreateBaseWater()?.identifier) ?? "");
02082: 			list.Add(Core.Clean(ItemConditionList.CreateGhostWater()?.identifier) ?? "");
02083: 			list.Add(Core.Clean(ItemConditionList.CreateRustWater()?.identifier) ?? "");
02084: 			list.Add(Core.Clean(ItemConditionList.CreateGutterflow()?.identifier) ?? "");
02085: 		}
02086: 		catch
02087: 		{
02088: 		}
02089: 		_waterCondIds = list.ToArray();
02090: 		return _waterCondIds;
02091: 	}
02092: 
02093: 	internal static bool TryGetPoolFill(GameItem pack, int slot, out float fill)
02094: 	{
02095: 		fill = 0f;
02096: 		try
02097: 		{
02098: 			Dictionary<string, long> dictionary = ParsePool(StorageGrid.GetLiquidSnapshot(pack, slot));
02099: 			long num = 0L;
02100: 			foreach (long value in dictionary.Values)
02101: 			{
02102: 				num += value;
02103: 			}
02104: 			if (num <= 0)
02105: 			{
02106: 				return false;
02107: 			}
02108: 			int itemCapacityUl = StorageGrid.GetItemCapacityUl(pack, slot);
02109: 			if (itemCapacityUl <= 0)
02110: 			{
02111: 				return false;
02112: 			}
02113: 			fill = (float)((double)num / (double)itemCapacityUl);
02114: 			if (fill > 1f)
02115: 			{
02116: 				fill = 1f;
02117: 			}
02118: 			return true;
02119: 		}
02120: 		catch
02121: 		{
02122: 			return false;
02123: 		}
02124: 	}
02125: 
02126: 	internal static bool TryGetPoolColor(GameItem pack, int slot, out Color col)
02127: 	{
~~~

~~~csharp
02280: 				return null;
02281: 			}
02282: 			if (text.Length > maxChars)
02283: 			{
02284: 				text = text.Substring(0, maxChars);
02285: 			}
02286: 			while (text.Length < maxChars)
02287: 			{
02288: 				text += "\u3000";
02289: 			}
02290: 			return text;
02291: 		}
02292: 		catch
02293: 		{
02294: 			return null;
02295: 		}
02296: 	}
02297: 
02298: 	private static string? TryReadDepositName(GameItem dragged, string itemId)
02299: 	{
02300: 		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
02301: 		try
02302: 		{
02303: 			if (IsEmptyWaterItem(dragged))
02304: 			{
02305: 				GameItem val = null;
02306: 				try
02307: 				{
02308: 					val = DirectoryMaster.Item(itemId, true);
02309: 				}
02310: 				catch
02311: 				{
02312: 				}
02313: 				if (val != null)
02314: 				{
02315: 					string text = TryReadName(val);
02316: 					try
02317: 					{
02318: 						val.Destroy();
02319: 					}
02320: 					catch
02321: 					{
02322: 					}
02323: 					if (!string.IsNullOrEmpty(text))
02324: 					{
02325: 						Core.Debug("收纳箱开格记名：空瓶出厂名 '" + text + "'");
02326: 						return text;
02327: 					}
02328: 				}
02329: 			}
02330: 		}
02331: 		catch
02332: 		{
02333: 		}
02334: 		string text2 = TryReadName(dragged);
02335: 		if (string.IsNullOrEmpty(text2))
02336: 		{
02337: 			return text2;
02338: 		}
02339: 		try
02340: 		{
02341: 			string localizedMechanic = LocHelper.GetLocalizedMechanic("husbandry_stage_unknown", Array.Empty<object>());
02342: 			if (dragged.IsTag("ANIMAL_AGE_TAG"))
02343: 			{
02344: 				string animalGrowthStageDisplay = HusbandryHelper.GetAnimalGrowthStageDisplay(dragged);
02345: 				string localizedMechanic2 = LocHelper.GetLocalizedMechanic("husbandry_stage_infant", Array.Empty<object>());
02346: 				if (!string.IsNullOrEmpty(animalGrowthStageDisplay) && animalGrowthStageDisplay != localizedMechanic && animalGrowthStageDisplay == localizedMechanic2)
02347: 				{
02348: 					return animalGrowthStageDisplay;
02349: 				}
02350: 			}
02351: 			else if (dragged.IsTag("FISH_AGE_TAG"))
02352: 			{
02353: 				string stageDisplay = FishSpecies.GetStageDisplay(AquacultureHelper.GetStage(dragged));
02354: 				string localizedMechanic3 = LocHelper.GetLocalizedMechanic("husbandry_stage_juvenile", Array.Empty<object>());
02355: 				if (!string.IsNullOrEmpty(stageDisplay) && stageDisplay != localizedMechanic && stageDisplay == localizedMechanic3)
02356: 				{
02357: 					return stageDisplay;
02358: 				}
02359: 			}
02360: 		}
02361: 		catch
02362: 		{
02363: 		}
02364: 		return text2;
02365: 	}
02366: 
02367: 	private static string SerializePool(Dictionary<string, long> pool)
02368: 	{
02369: 		StringBuilder stringBuilder = new StringBuilder();
02370: 		foreach (KeyValuePair<string, long> item in pool)
02371: 		{
02372: 			if (item.Value > 0)
02373: 			{
02374: 				if (stringBuilder.Length > 0)
02375: 				{
02376: 					stringBuilder.Append(';');
02377: 				}
02378: 				stringBuilder.Append(item.Key).Append('=').Append(item.Value);
02379: 			}
02380: 		}
02381: 		return stringBuilder.ToString();
02382: 	}
02383: 
02384: 	private static void AllocatePools(GameItem pack, int slot, GameItem item)
02385: 	{
02386: 		try
02387: 		{
02388: 			if (PowerHelper.IsAvailableEnergyFromItem(item))
02389: 			{
02390: 				int num = Rd();
02391: 				PowerHelper.ChargeBattery(item);
02392: 				int num2 = Rd();
02393: 				int energyPool = StorageGrid.GetEnergyPool(pack, slot);
02394: 				int num3 = Math.Min(Math.Max(0, energyPool), num2);
02395: 				int num4 = num2 - num3;
02396: 				if (num4 > 0)
02397: 				{
02398: 					try
02399: 					{
02400: 						PowerHelper.TryRemoveEnergy(item, num4, false);
02401: 					}
02402: 					catch
02403: 					{
02404: 					}
02405: 				}
02406: 				int num5 = Rd();
02407: 				if (num5 == num3)
02408: 				{
02409: 					StorageGrid.SetEnergyPool(pack, slot, energyPool - num3);
02410: 					try
02411: 					{
02412: 						PowerHelper.UpdateEnergyCreditSprite(item);
02413: 					}
02414: 					catch
02415: 					{
02416: 					}
02417: 					try
02418: 					{
02419: 						if (num3 == num2 && num3 > 0 && string.IsNullOrEmpty(StorageGrid.GetFullSprite(pack, slot)))
02420: 						{
02421: 							string text = TrySpriteKey(item);
02422: 							if (text != "-")
02423: 							{
02424: 								StorageGrid.SetFullSprite(pack, slot, text);
02425: 								StorageGrid.SetEnergyMax(pack, slot, num2);
02426: 								Core.Debug("收纳箱池回收：满电 key=" + text + " max=" + num2);
02427: 							}
02428: 						}
02429: 					}
02430: 					catch
02431: 					{
02432: 					}
02433: 					RefreshEnergySlotSprite(pack, slot);
02434: 					Core.Debug("收纳箱 withdraw 池：电量 give=" + num3 + "/" + num2 + " → 格池 " + (energyPool - num3));
02435: 				}
02436: 				else
02437: 				{
02438: 					int num6 = Rd();
02439: 					try
02440: 					{
02441: 						if (num6 > num)
02442: 						{
02443: 							PowerHelper.TryRemoveEnergy(item, num6 - num, false);
02444: 						}
02445: 					}
02446: 					catch
02447: 					{
02448: 					}
02449: 					try
02450: 					{
02451: 						PowerHelper.UpdateEnergyCreditSprite(item);
02452: 					}
02453: 					catch
02454: 					{
02455: 					}
02456: 					Core.Log?.Msg("收纳箱 withdraw 池：电量设值失败 give=" + num3 + " back=" + num5 + " cur0=" + num + "（回退出厂电量，池未扣）");
02457: 				}
02458: 			}
02459: 		}
02460: 		catch (Exception ex)
02461: 		{
02462: 			Core.Log?.Msg("收纳箱 withdraw 池：电量分配失败 " + ex.Message);
02463: 		}
02464: 		try
02465: 		{
02466: 			int maxDurability = DurabilityHelper.GetMaxDurability(item);
02467: 			if (maxDurability > 0)
02468: 			{
02469: 				int durabilityPool = StorageGrid.GetDurabilityPool(pack, slot);
02470: 				int num7 = Math.Min(Math.Max(0, durabilityPool), maxDurability);
02471: 				DurabilityHelper.SetCurrentDurability(item, num7);
02472: 				int num8 = -1;
02473: 				int num9 = -1;
02474: 				try
02475: 				{
02476: 					num8 = DurabilityHelper.GetRealDurability(item);
02477: 				}
02478: 				catch
02479: 				{
02480: 				}
02481: 				try
02482: 				{
02483: 					num9 = DurabilityHelper.GetFakeDurability(item);
02484: 				}
02485: 				catch
02486: 				{
02487: 				}
02488: 				if (num8 == num7 || num9 == num7)
02489: 				{
02490: 					StorageGrid.SetDurabilityPool(pack, slot, durabilityPool - num7);
02491: 					Core.Debug("收纳箱 withdraw 池：耐久 give=" + num7 + "/" + maxDurability + "（real=" + num8 + " fake=" + num9 + "）→ 格池 " + (durabilityPool - num7));
02492: 				}
02493: 				else
02494: 				{
02495: 					Core.Debug("收纳箱 withdraw 池：耐久读回不符 give=" + num7 + " real=" + num8 + " fake=" + num9 + "（池未扣）");
02496: 				}
02497: 			}
02498: 		}
02499: 		catch (Exception ex2)
02500: 		{
02501: 			Core.Debug("收纳箱 withdraw 池：耐久分配失败 " + ex2.Message);
02502: 		}
02503: 		try
02504: 		{
02505: 			var (flag, num10, num11) = StorageGrid.ReadItemUseCount(item);
02506: 			if (!flag || num11 <= 0)
02507: 			{
02508: 				return;
02509: 			}
02510: 			int useCountPool = StorageGrid.GetUseCountPool(pack, slot);
02511: 			int num12 = Math.Min(Math.Max(0, useCountPool), num11);
02512: 			UseCountHelper.SetCurrentUseCount(item, num12);
02513: 			int num13 = -1;
02514: 			try
02515: 			{
02516: 				TagSystem state = item.state;
02517: 				num13 = ((state == null) ? ((int?)null) : state.dict?["CURRENT_USE_COUNT_INT"]?.valueInt) ?? (-1);
02518: 			}
02519: 			catch
02520: 			{
02521: 			}
02522: 			if (num13 == num12 || (num12 == 0 && num13 < 0))
02523: 			{
02524: 				StorageGrid.SetUseCountPool(pack, slot, useCountPool - num12);
02525: 				Core.Debug("收纳箱 withdraw 池：次数 give=" + num12 + "/" + num11 + " → 格池 " + (useCountPool - num12));
02526: 				return;
02527: 			}
02528: 			try
02529: 			{
02530: 				UseCountHelper.SetCurrentUseCount(item, num10);
02531: 			}
02532: 			catch
02533: 			{
02534: 			}
02535: 			Core.Log?.Msg("收纳箱 withdraw 池：次数设值失败 give=" + num12 + " back=" + num13 + " cur0=" + num10 + "（回退出厂次数，池未扣）");
02536: 		}
02537: 		catch (Exception ex3)
02538: 		{
02539: 			Core.Log?.Msg("收纳箱 withdraw 池：次数分配失败 " + ex3.Message);
02540: 		}
02541: 		int Rd()
02542: 		{
02543: 			try
02544: 			{
02545: 				return Math.Max(0, PowerHelper.GetAvailableEnergyFromItem(item, false));
02546: 			}
02547: 			catch
02548: 			{
02549: 				return -1;
02550: 			}
02551: 		}
02552: 	}
02553: 
02554: 	private static string TrySpriteKey(GameItem item)
02555: 	{
02556: 		try
02557: 		{
02558: 			return Core.Clean(item.spritePath) ?? "-";
02559: 		}
02560: 		catch
02561: 		{
02562: 			return "-";
02563: 		}
02564: 	}
02565: 
02566: 	private static void InitEnergySlotDisplay(GameItem pack, int slot, GameItem dragged, string itemId)
02567: 	{
02568: 		try
02569: 		{
02570: 			bool flag = false;
02571: 			try
02572: 			{
02573: 				flag = StorageGrid.ReadItemEnergy(dragged).HasValue;
02574: 			}
02575: 			catch
02576: 			{
02577: 			}
02578: 			if (flag)
02579: 			{
02580: 				StorageGrid.SetOrigSprite(pack, slot, StorageGrid.GetSpritePath(pack, slot));
02581: 				ProbeFullSprite(pack, slot, itemId);
02582: 			}
02583: 		}
02584: 		catch
02585: 		{
02586: 		}
02587: 	}
02588: 
02589: 	private static void ProbeFullSprite(GameItem pack, int slot, string itemId)
02590: 	{
02591: 		try
02592: 		{
02593: 			if (!string.IsNullOrEmpty(StorageGrid.GetFullSprite(pack, slot)))
02594: 			{
02595: 				return;
02596: 			}
02597: 			GameItem val = null;
02598: 			try
02599: 			{
02600: 				val = DirectoryMaster.Item(itemId, true);
02601: 			}
02602: 			catch
02603: 			{
02604: 			}
02605: 			if (val == null)
02606: 			{
02607: 				Core.Debug("收纳箱探针：slot=" + slot + " '" + itemId + "' 重建失败，满电 key 待首次取出回收");
02608: 				return;
02609: 			}
02610: 			string text = TrySpriteKey(val);
02611: 			try
02612: 			{
02613: 				PowerHelper.ChargeBattery(val);
02614: 			}
02615: 			catch
02616: 			{
02617: 			}
02618: 			try
02619: 			{
02620: 				PowerHelper.UpdateEnergyCreditSprite(val);
02621: 			}
02622: 			catch
02623: 			{
02624: 			}
02625: 			string text2 = TrySpriteKey(val);
02626: 			int num = 0;
02627: 			try
02628: 			{
02629: 				num = Math.Max(0, PowerHelper.GetAvailableEnergyFromItem(val, false));
02630: 			}
02631: 			catch
02632: 			{
02633: 			}
02634: 			try
02635: 			{
02636: 				val.Destroy();
02637: 			}
02638: 			catch
02639: 			{
02640: 			}
02641: 			Core.Debug("收纳箱探针：slot=" + slot + " before=" + text + " after=" + text2 + " max=" + num);
02642: 			if (num > 0 && text2 != "-" && !string.IsNullOrEmpty(text2))
02643: 			{
02644: 				StorageGrid.SetFullSprite(pack, slot, text2);
02645: 				StorageGrid.SetEnergyMax(pack, slot, num);
02646: 			}
02647: 		}
02648: 		catch (Exception ex)
02649: 		{
02650: 			Core.Debug("收纳箱探针异常：" + ex.Message);
02651: 		}
02652: 	}
02653: 
02654: 	private static void RefreshEnergySlotSprite(GameItem pack, int slot)
02655: 	{
02656: 		try
02657: 		{
02658: 			string fullSprite = StorageGrid.GetFullSprite(pack, slot);
02659: 			int energyMax = StorageGrid.GetEnergyMax(pack, slot);
02660: 			if (string.IsNullOrEmpty(fullSprite) || energyMax <= 0)
02661: 			{
02662: 				return;
02663: 			}
02664: 			int energyPool = StorageGrid.GetEnergyPool(pack, slot);
02665: 			string text = null;
02666: 			int num = Math.Min(Math.Max(0, energyPool), energyMax);
02667: 			if (num >= energyMax)
02668: 			{
02669: 				text = fullSprite;
02670: 			}
02671: 			else
02672: 			{
02673: 				int num2 = num * 100 / energyMax;
02674: 				int num3 = ((num2 >= 90) ? 100 : ((num2 >= 70) ? 80 : ((num2 >= 50) ? 60 : ((num2 >= 30) ? 40 : ((num2 >= 10) ? 20 : ((num2 >= 1) ? 10 : 0))))));
02675: 				Match match = Regex.Match(fullSprite, "^(.*?)(\\d+)$");
02676: 				if (match.Success)
02677: 				{
02678: 					string text2 = match.Groups[1].Value + num3;
02679: 					if (BoxWindow.IsSpriteResolvable(StorageGrid.GetSpriteAtlas(pack, slot), text2))
02680: 					{
02681: 						text = text2;
02682: 					}
02683: 				}
02684: 			}
02685: 			if (!string.IsNullOrEmpty(text) && !string.Equals(StorageGrid.GetSpritePath(pack, slot), text, StringComparison.Ordinal))
02686: 			{
02687: 				StorageGrid.SetSpriteRef(pack, slot, StorageGrid.GetSpriteAtlas(pack, slot), text);
02688: 				Core.Debug("收纳箱格池贴图：slot=" + slot + " pool=" + energyPool + " next=" + num + "/" + energyMax + " → " + text);
02689: 			}
02690: 		}
02691: 		catch
02692: 		{
02693: 		}
02694: 	}
02695: 
02696: 	private static void RealizeTruthBeforeDeposit(GameItem item)
02697: 	{
02698: 		ItemFeature val = StorageGrid.FindConditionFeature(item);
02699: 		string text = "";
02700: 		try
02701: 		{
02702: 			text = Core.Clean(val?.realCondition?.identifier) ?? "";
02703: 		}
02704: 		catch
02705: 		{
02706: 		}
02707: 		if (string.IsNullOrEmpty(text) || !CondMap.TryGetValue(text, out CondEntry value))
02708: 		{
02709: 			return;
02710: 		}
02711: 		Action<GameItem> apply = value.Apply;
02712: 		bool flag = apply == null;
02713: 		try
02714: 		{
02715: 			ItemFeature val2;
02716: 			if (flag)
02717: 			{
02718: 				try
02719: 				{
02720: 					val2 = ChemicalProductHelper.GetChemicalFeature(item);
02721: 				}
02722: 				catch
02723: 				{
02724: 					val2 = null;
02725: 				}
02726: 			}
02727: 			else
02728: 			{
02729: 				val2 = StorageGrid.FindConditionFeature(item);
02730: 			}
02731: 			string b = "";
02732: 			try
02733: 			{
02734: 				b = Core.Clean(val2?.fakeCondition?.identifier) ?? "";
02735: 			}
02736: 			catch
02737: 			{
02738: 			}
02739: 			long num = -1L;
02740: 			try
02741: 			{
02742: 				num = item.unitValue;
02743: 			}
02744: 			catch
02745: 			{
02746: 			}
02747: 			if (flag)
02748: 			{
02749: 				ItemCondition val3 = value.Factory();
02750: 				if (val3 == null)
02751: 				{
02752: 					return;
02753: 				}
02754: 				bool flag2 = false;
02755: 				try
02756: 				{
02757: 					ChemicalProductHelper.SetPurityFeature(item, val3, val3);
02758: 					flag2 = true;
02759: 				}
02760: 				catch
02761: 				{
02762: 				}
02763: 				if (!flag2)
02764: 				{
02765: 					try
02766: 					{
02767: 						ChemicalProductHelper.SetPurityFeature(item, val3);
02768: 					}
02769: 					catch
02770: 					{
02771: 						return;
02772: 					}
02773: 				}
02774: 			}
02775: 			else
02776: 			{
02777: 				apply?.Invoke(item);
02778: 			}
02779: 			ItemFeature val4;
02780: 			if (flag)
02781: 			{
02782: 				try
02783: 				{
02784: 					val4 = ChemicalProductHelper.GetChemicalFeature(item);
02785: 				}
02786: 				catch
02787: 				{
02788: 					val4 = null;
02789: 				}
02790: 			}
02791: 			else
02792: 			{
02793: 				val4 = StorageGrid.FindConditionFeature(item);
02794: 			}
02795: 			string text2 = "";
02796: 			try
02797: 			{
02798: 				text2 = Core.Clean(val4?.realCondition?.identifier) ?? "";
02799: 			}
02800: 			catch
02801: 			{
02802: 			}
02803: 			if (!string.Equals(text2, text, StringComparison.Ordinal))
02804: 			{
02805: 				Core.Debug("收纳箱 deposit 实化：'" + text + "' 真值读回不符 back='" + text2 + "'（放弃实化，按现状存入）");
02806: 				return;
02807: 			}
02808: 			string a = "";
02809: 			try
02810: 			{
02811: 				a = Core.Clean(val4?.fakeCondition?.identifier) ?? "";
02812: 			}
02813: 			catch
02814: 			{
02815: 			}
02816: 			if (!string.Equals(a, text, StringComparison.Ordinal))
02817: 			{
02818: 				try
02819: 				{
02820: 					ItemCondition val5 = value.Factory();
02821: 					if (val4 != null)
02822: 					{
02823: 						val4.SetConditions(val5, val5);
02824: 					}
02825: 					a = text;
02826: 					Core.Debug("收纳箱 deposit 实化：'" + text + "' 标签归真值（Helper 未动 fake，手动双写）");
02827: 				}
02828: 				catch
02829: 				{
02830: 				}
02831: 			}
02832: 			long num2 = -1L;
02833: 			try
02834: 			{
02835: 				num2 = item.unitValue;
02836: 			}
02837: 			catch
02838: 			{
02839: 			}
02840: 			long num3 = -1L;
02841: 			try
02842: 			{
02843: 				num3 = item.unitBaseValue;
02844: 			}
02845: 			catch
02846: 			{
02847: 			}
02848: 			bool flag3 = !string.Equals(a, b, StringComparison.Ordinal);
02849: 			if (flag)
02850: 			{
02851: 				Core.Debug("收纳箱 deposit 实化：'" + text + "' 化学品缓存 " + num2 + "/基础字段 " + num3 + "（纯度修正走官方动态管线，不写缓存）");
02852: 			}
02853: 			else
02854: 			{
02855: 				if (flag3 && num >= 0 && num2 == num)
02856: 				{
02857: 					string text3 = "";
02858: 					try
02859: 					{
02860: 						text3 = Core.Clean(item.identifier) ?? "";
02861: 					}
02862: 					catch
02863: 					{
02864: 					}
02865: 					int num4 = 0;
02866: 					try
02867: 					{
02868: 						num4 = value.Factory()?.modValueDisplay ?? 0;
02869: 					}
02870: 					catch
02871: 					{
02872: 					}
02873: 					if (num4 == 0)
02874: 					{
02875: 						if (string.Equals(text, "genuineCigarette", StringComparison.Ordinal))
02876: 						{
02877: 							bool flag4 = false;
02878: 							try
02879: 							{
02880: 								TagSystem state = item.state;
02881: 								flag4 = ((state == null) ? ((bool?)null) : state.dict?.ContainsKey("LUXURY_ITEM")) == true;
02882: 							}
02883: 							catch
02884: 							{
02885: 							}
02886: 							long num5 = (flag4 ? 100 : 80);
02887: 							if (num2 > 0 && num2 != num5)
02888: 							{
02889: 								try
02890: 								{
02891: 									item.SetValue(num5);
02892: 								}
02893: 								catch
02894: 								{
02895: 								}
02896: 								Core.Debug("收纳箱 deposit 实化形态归一：'genuineCigarette' 价 " + num2 + " → " + num5 + (flag4 ? "（奢形态）" : "（素形态）") + "（贴标贬值价复位）");
02897: 							}
02898: 							try
02899: 							{
02900: 								string text4 = item.name ?? "";
02901: 								if (!text4.Contains("（赝品）"))
02902: 								{
02903: 									return;
02904: 								}
02905: 								string text5 = "";
02906: 								GameItem val6 = null;
02907: 								try
02908: 								{
02909: 									val6 = DirectoryMaster.Item("packet_red_cigarette", true);
02910: 								}
02911: 								catch
02912: 								{
02913: 								}
02914: 								try
02915: 								{
02916: 									text5 = ((val6 != null) ? val6.name : null) ?? "";
02917: 								}
02918: 								catch
02919: 								{
02920: 								}
02921: 								try
02922: 								{
02923: 									if (val6 != null)
02924: 									{
02925: 										val6.Destroy();
02926: 									}
02927: 								}
02928: 								catch
02929: 								{
02930: 								}
02931: 								if (!string.IsNullOrEmpty(text5))
02932: 								{
02933: 									try
02934: 									{
02935: 										item.name = text5;
02936: 									}
02937: 									catch
02938: 									{
02939: 									}
02940: 									Core.Debug("收纳箱 deposit 实化名字归一：'" + text4 + "' → '" + text5 + "'");
02941: 								}
02942: 								return;
02943: 							}
02944: 							catch
02945: 							{
02946: 								return;
02947: 							}
02948: 						}
02949: 						long num6 = -1L;
02950: 						GameItem val7 = null;
02951: 						try
02952: 						{
02953: 							val7 = DirectoryMaster.Item(text3, true);
02954: 						}
02955: 						catch
02956: 						{
02957: 						}
02958: 						try
02959: 						{
02960: 							num6 = ((val7 != null) ? val7.unitValue : (-1));
02961: 						}
02962: 						catch
02963: 						{
02964: 						}
02965: 						try
02966: 						{
02967: 							if (val7 != null)
02968: 							{
02969: 								val7.Destroy();
02970: 							}
02971: 						}
02972: 						catch
02973: 						{
02974: 						}
02975: 						if (num6 > 0 && num2 != num6)
02976: 						{
02977: 							try
02978: 							{
02979: 								item.SetValue(num6);
02980: 							}
02981: 							catch
02982: 							{
02983: 							}
02984: 							Core.Debug("收纳箱 deposit 实化形态归一：'" + text + "' 价 " + num2 + " → 模板价 " + num6 + "（零差价真值形态，贴标伪装价复位）");
02985: 						}
02986: 						else
02987: 						{
02988: 							Core.Debug("收纳箱 deposit 实化定价：'" + text + "' modDisplay=0 跳过（价已是模板价 " + num2 + "）");
02989: 						}
02990: 						return;
02991: 					}
02992: 					long num7 = -1L;
02993: 					Func<GameItem> priceProbe = value.PriceProbe;
02994: 					if (priceProbe != null)
02995: 					{
02996: 						GameItem val8 = null;
02997: 						try
02998: 						{
02999: 							val8 = priceProbe();
03000: 							if (val8 != null)
03001: 							{
03002: 								try
03003: 								{
03004: 									num7 = val8.unitValue;
03005: 								}
03006: 								catch
03007: 								{
03008: 								}
03009: 							}
03010: 						}
03011: 						catch
03012: 						{
03013: 						}
03014: 						finally
03015: 						{
03016: 							try
03017: 							{
03018: 								if (val8 != null)
03019: 								{
03020: 									val8.Destroy();
03021: 								}
03022: 							}
03023: 							catch
03024: 							{
03025: 							}
03026: 						}
03027: 					}
03028: 					long num8 = -1L;
03029: 					GameItem val9 = null;
03030: 					try
03031: 					{
03032: 						val9 = DirectoryMaster.Item(text3, true);
03033: 					}
03034: 					catch
03035: 					{
03036: 					}
03037: 					if (val9 != null)
03038: 					{
03039: 						try
03040: 						{
03041: 							num8 = val9.unitValue;
03042: 						}
03043: 						catch
03044: 						{
03045: 						}
03046: 						try
03047: 						{
03048: 							val9.Destroy();
03049: 						}
03050: 						catch
03051: 						{
03052: 						}
03053: 					}
03054: 					long num9 = -1L;
03055: 					try
03056: 					{
03057: 						num9 = item.GetBaseValue();
03058: 					}
03059: 					catch
03060: 					{
03061: 					}
03062: 					long num10 = ((num8 > 0) ? ((long)Math.Round((double)num8 * (1.0 + (double)num4 / 100.0))) : (-1));
03063: 					long num11 = ((num7 > 0 && num10 > 0 && num7 != num8) ? num7 : num10);
03064: 					Core.Debug("收纳箱 deposit 实化定价侦查：'" + text + "' 模板价=" + num8 + " GetBaseValue=" + num9 + " modDisplay=" + num4 + " probe价=" + num7 + " 公式价=" + num10 + " 当前=" + num2 + "/基础字段=" + num3);
03065: 					if (num11 <= 0 || num11 == num2)
03066: 					{
03067: 						return;
03068: 					}
03069: 					try
03070: 					{
03071: 						item.SetValue(num11);
03072: 						long num12 = -1L;
03073: 						try
03074: 						{
03075: 							num12 = item.unitValue;
03076: 						}
03077: 						catch
03078: 						{
03079: 						}
03080: 						Core.Debug("收纳箱 deposit 实化定价：'" + text + "' 价值 " + num2 + " → " + num11 + "（写回读回 " + num12 + "）");
03081: 						return;
03082: 					}
03083: 					catch (Exception ex)
03084: 					{
03085: 						Core.Debug("收纳箱 deposit 实化定价失败：'" + text + "' " + ex.Message + "（维持快照价值）");
03086: 						return;
03087: 					}
03088: 				}
03089: 				if (num >= 0 && num2 != num)
03090: 				{
03091: 					Core.Debug("收纳箱 deposit 实化：'" + text + "' 价值 " + num + " → " + num2 + "（官方重算）");
03092: 				}
03093: 			}
03094: 		}
03095: 		catch (Exception ex2)
03096: 		{
03097: 			Core.Debug("收纳箱 deposit 实化异常：" + ex2.Message + "（不影响存入）");
03098: 		}
03099: 	}
03100: 
03101: 	private static GameItem? TryBuildFactoryItem(string itemId, string? storedPur)
03102: 	{
03103: 		if (!NullGuard.HasText(itemId) || !NullGuard.HasText(storedPur))
03104: 		{
03105: 			return null;
03106: 		}
03107: 		int num = storedPur.IndexOf('\u0001');
03108: 		string text = ((num < 0) ? storedPur : storedPur.Substring(0, num));
03109: 		if (string.IsNullOrEmpty(text))
03110: 		{
03111: 			return null;
03112: 		}
03113: 		if (!CondMap.TryGetValue(text, out CondEntry value) || value.PriceProbe == null)
03114: 		{
03115: 			return null;
03116: 		}
03117: 		GameItem val = null;
03118: 		try
03119: 		{
03120: 			val = value.PriceProbe();
03121: 		}
03122: 		catch
03123: 		{
03124: 			return null;
03125: 		}
03126: 		if (val == null)
03127: 		{
03128: 			return null;
03129: 		}
03130: 		string text2 = "";
03131: 		string text3 = "";
03132: 		long num2 = -1L;
03133: 		try
03134: 		{
03135: 			text2 = Core.Clean(val.identifier) ?? "";
03136: 		}
03137: 		catch
03138: 		{
03139: 		}
03140: 		try
03141: 		{
03142: 			text3 = Core.Clean(StorageGrid.FindConditionFeature(val)?.realCondition?.identifier) ?? "";
03143: 		}
03144: 		catch
03145: 		{
03146: 		}
03147: 		try
03148: 		{
03149: 			num2 = val.unitValue;
03150: 		}
03151: 		catch
03152: 		{
03153: 		}
03154: 		bool flag = string.Equals(text2, itemId, StringComparison.Ordinal) && string.Equals(text3, text, StringComparison.Ordinal);
03155: 		Core.Debug("收纳箱 withdraw 工厂直发侦查：'" + text + "' 工厂产物 id='" + text2 + "'(期望'" + itemId + "') real='" + text3 + "' value=" + num2 + " → " + (flag ? "采用（官方产线针）" : "不采用（退重建路径）"));
03156: 		if (!flag)
03157: 		{
03158: 			try
03159: 			{
03160: 				val.Destroy();
03161: 			}
03162: 			catch
03163: 			{
03164: 			}
03165: 			return null;
03166: 		}
03167: 		return val;
03168: 	}
03169: 
03170: 	private static GameItem? LaunderGenuineCigarette()
03171: 	{
03172: 		GameItem val = null;
03173: 		try
03174: 		{
03175: 			val = PreBuiltItemHelper.FakeCigarette();
03176: 		}
03177: 		catch
03178: 		{
03179: 			return null;
03180: 		}
03181: 		if (val == null)
03182: 		{
03183: 			return null;
03184: 		}
03185: 		try
03186: 		{
03187: 			ItemFeature val2 = StorageGrid.FindConditionFeature(val);
03188: 			ItemCondition val3 = ItemConditionList.CreateGenuineCigarette();
03189: 			if (val2 != null && val3 != null)
03190: 			{
03191: 				val2.SetConditions(val3, val3);
03192: 			}
03193: 		}
03194: 		catch
03195: 		{
03196: 		}
03197: 		try
03198: 		{
03199: 			string[] array = new string[5] { "CIGARETTE_SEAL_ISSUE", "CIGARETTE_LOGO_ISSUE", "CIGARETTE_LETTERING_ISSUE", "CIGARETTE_BOX_ISSUE", "CIGARETTE_COLOR_ISSUE" };
03200: 			foreach (string key in array)
03201: 			{
03202: 				TagSystem state = val.state;
03203: 				if (state != null)
03204: 				{
03205: 					state.dict?.Remove(key);
03206: 				}
03207: 			}
03208: 		}
03209: 		catch
03210: 		{
03211: 		}
03212: 		return val;
03213: 	}
03214: 
03215: 	private static void TryRegister(Dictionary<string, CondEntry> map, Func<ItemCondition> factory, Action<GameItem>? apply = null, bool isOfficialHelper = false, Func<GameItem?>? priceProbe = null)
03216: 	{
03217: 		try
03218: 		{
03219: 			string text = Core.Clean(factory()?.identifier);
03220: 			if (!string.IsNullOrEmpty(text))
03221: 			{
03222: 				map[text] = new CondEntry
03223: 				{
03224: 					Factory = factory,
03225: 					Apply = apply,
03226: 					IsOfficialHelper = isOfficialHelper,
03227: 					PriceProbe = priceProbe
03228: 				};
03229: 			}
03230: 		}
03231: 		catch
03232: 		{
03233: 		}
03234: 	}
03235: 
03236: 	private static void RestoreCondition(GameItem item, string stored)
03237: 	{
03238: 		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
03239: 		//IL_0327: Expected O, but got Unknown
03240: 		try
03241: 		{
03242: 			if (string.IsNullOrEmpty(stored))
03243: 			{
03244: 				return;
03245: 			}
03246: 			int num = stored.IndexOf('\u0001');
03247: 			string text = ((num < 0) ? stored : stored.Substring(0, num));
03248: 			bool flag = num >= 0;
03249: 			CondEntry condEntry = null;
03250: 			if (CondMap.TryGetValue(text, out CondEntry value))
03251: 			{
03252: 				condEntry = value;
03253: 			}
03254: 			if (condEntry != null)
03255: 			{
03256: 				if (condEntry.Apply != null)
03257: 				{
03258: 					try
03259: 					{
03260: 						condEntry.Apply(item);
03261: 						string text2 = "";
03262: 						ItemFeature val = StorageGrid.FindConditionFeature(item);
03263: 						try
03264: 						{
03265: 							text2 = Core.Clean(val?.realCondition?.identifier) ?? "";
03266: 						}
03267: 						catch
03268: 						{
03269: 						}
03270: 						if (string.Equals(text2, text, StringComparison.Ordinal))
03271: 						{
03272: 							Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（体系专属 Helper 写回）");
03273: 							return;
03274: 						}
03275: 						Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（Helper 写回读回不一致 back=" + text2 + "，落通用克隆）");
03276: 					}
03277: 					catch (Exception ex)
03278: 					{
03279: 						Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（体系 Helper 异常：" + ex.Message + "，落通用克隆）");
03280: 					}
03281: 				}
03282: 				else
03283: 				{
03284: 					ItemCondition val2 = condEntry.Factory();
03285: 					if (val2 != null)
03286: 					{
03287: 						try
03288: 						{
03289: 							ChemicalProductHelper.SetPurityFeature(item, val2, val2);
03290: 							string a = "";
03291: 							try
03292: 							{
03293: 								a = Core.Clean(ChemicalProductHelper.GetChemicalFeature(item)?.realCondition?.identifier) ?? "";
03294: 							}
03295: 							catch
03296: 							{
03297: 							}
03298: 							if (string.Equals(a, text, StringComparison.Ordinal))
03299: 							{
03300: 								Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（SetPurityFeature 三参双写 real+fake）");
03301: 								return;
03302: 							}
03303: 						}
03304: 						catch
03305: 						{
03306: 						}
03307: 						try
03308: 						{
03309: 							ChemicalProductHelper.SetPurityFeature(item, val2);
03310: 							string a2 = "";
03311: 							try
03312: 							{
03313: 								a2 = Core.Clean(ChemicalProductHelper.GetCurrentChemicalConditionId(item)) ?? "";
03314: 							}
03315: 							catch
03316: 							{
03317: 							}
03318: 							if (string.Equals(a2, text, StringComparison.Ordinal))
03319: 							{
03320: 								Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（SetPurityFeature）");
03321: 								return;
03322: 							}
03323: 						}
03324: 						catch
03325: 						{
03326: 						}
03327: 					}
03328: 				}
03329: 			}
03330: 			if (flag)
03331: 			{
03332: 				try
03333: 				{
03334: 					ItemCondition val3 = StorageGrid.DeserializeCondition(stored);
03335: 					if (val3 != null)
03336: 					{
03337: 						ItemCondition val4 = null;
03338: 						try
03339: 						{
03340: 							val4 = condEntry?.Factory?.Invoke();
03341: 						}
03342: 						catch
03343: 						{
03344: 						}
03345: 						if (val4 == null)
03346: 						{
03347: 							val4 = val3;
03348: 						}
03349: 						string text3 = "";
03350: 						try
03351: 						{
03352: 							text3 = Core.Clean(val3.category) ?? "";
03353: 						}
03354: 						catch
03355: 						{
03356: 						}
03357: 						ItemFeature val5 = null;
03358: 						List<ItemFeature> itemFeatures = item.itemFeatures;
03359: 						if (itemFeatures != null)
03360: 						{
03361: 							int num2 = 0;
03362: 							try
03363: 							{
03364: 								num2 = itemFeatures.Count;
03365: 							}
03366: 							catch
03367: 							{
03368: 							}
03369: 							for (int i = 0; i < num2; i++)
03370: 							{
03371: 								ItemFeature val6 = null;
03372: 								try
03373: 								{
03374: 									val6 = itemFeatures[i];
03375: 								}
03376: 								catch
03377: 								{
03378: 									continue;
03379: 								}
03380: 								if (val6 == null)
03381: 								{
03382: 									continue;
03383: 								}
03384: 								string a3 = "";
03385: 								try
03386: 								{
03387: 									a3 = Core.Clean(val6.category) ?? "";
03388: 								}
03389: 								catch
03390: 								{
03391: 								}
03392: 								bool num3;
03393: 								if (string.IsNullOrEmpty(text3))
03394: 								{
03395: 									if (val6.realCondition != null)
03396: 									{
03397: 										goto IL_02fc;
03398: 									}
03399: 									num3 = val6.fakeCondition != null;
03400: 								}
03401: 								else
03402: 								{
03403: 									num3 = string.Equals(a3, text3, StringComparison.Ordinal);
03404: 								}
03405: 								if (!num3)
03406: 								{
03407: 									continue;
03408: 								}
03409: 								goto IL_02fc;
03410: 								IL_02fc:
03411: 								val5 = val6;
03412: 								break;
03413: 							}
03414: 						}
03415: 						bool flag2 = false;
03416: 						if (val5 == null && !string.IsNullOrEmpty(text3))
03417: 						{
03418: 							ItemFeature val7 = new ItemFeature(text3);
03419: 							val7.identifier = val4.identifier;
03420: 							val7.useCondition = true;
03421: 							val7.realCondition = val4;
03422: 							val7.fakeCondition = val4;
03423: 							if (itemFeatures != null)
03424: 							{
03425: 								itemFeatures.Add(val7);
03426: 								val5 = val7;
03427: 								flag2 = true;
03428: 							}
03429: 						}
03430: 						if (val5 != null)
03431: 						{
03432: 							val5.SetConditions(val4, val4);
03433: 							string text4 = "";
03434: 							try
03435: 							{
03436: 								text4 = Core.Clean(val5.realCondition?.identifier) ?? "";
03437: 							}
03438: 							catch
03439: 							{
03440: 							}
03441: 							if (string.Equals(text4, text, StringComparison.Ordinal))
03442: 							{
03443: 								Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（通用克隆 SetConditions" + (flag2 ? ("，新建 feature cat=" + text3) : "") + "）");
03444: 								return;
03445: 							}
03446: 							Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（通用克隆读回不一致 back=" + text4 + "）");
03447: 						}
03448: 						else
03449: 						{
03450: 							Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（无 category 且无现有 feature，无法挂载）");
03451: 						}
03452: 					}
03453: 				}
03454: 				catch
03455: 				{
03456: 				}
03457: 			}
03458: 			try
03459: 			{
03460: 				bool flag3 = text.IndexOf("cigarette", StringComparison.OrdinalIgnoreCase) >= 0;
03461: 				if (flag3 && text.IndexOf("genuine", StringComparison.OrdinalIgnoreCase) >= 0)
03462: 				{
03463: 					InsCigaretteHelper.SetGenuine(item);
03464: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（香烟 SetGenuine）");
03465: 				}
03466: 				else if (flag3 && text.IndexOf("counterfeit", StringComparison.OrdinalIgnoreCase) >= 0)
03467: 				{
03468: 					ItemCondition val8 = ItemConditionList.CreateCounterfeitCigarette();
03469: 					ItemFeature val9 = StorageGrid.FindConditionFeature(item);
03470: 					if (val9 != null)
03471: 					{
03472: 						val9.SetConditions(val8, val8);
03473: 					}
03474: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（香烟赝品双写）");
03475: 				}
03476: 				else if (!flag3 && text.IndexOf("genuine", StringComparison.OrdinalIgnoreCase) >= 0)
03477: 				{
03478: 					InsInjectorHelper.SetGenuine(item);
03479: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（注射器 SetGenuine）");
03480: 				}
03481: 				else if (!flag3 && text.IndexOf("counterfeit", StringComparison.OrdinalIgnoreCase) >= 0)
03482: 				{
03483: 					InsInjectorHelper.SetCounterfeit(item);
03484: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（注射器 SetCounterfeit）");
03485: 				}
03486: 				else if (!flag3 && text.IndexOf("expired", StringComparison.OrdinalIgnoreCase) >= 0)
03487: 				{
03488: 					InsInjectorHelper.SetExpired(item);
03489: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（注射器 SetExpired）");
03490: 				}
03491: 				else if (!flag3 && text.IndexOf("unusable", StringComparison.OrdinalIgnoreCase) >= 0)
03492: 				{
03493: 					InsInjectorHelper.SetUnusable(item);
03494: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（注射器 SetUnusable）");
03495: 				}
03496: 			}
03497: 			catch
03498: 			{
03499: 			}
03500: 		}
03501: 		catch (Exception ex2)
03502: 		{
03503: 			Core.Debug("收纳箱 withdraw：条件还原失败 '" + stored + "'：" + ex2.Message);
03504: 		}
03505: 	}
03506: 
03507: 	internal static string? PurityShortName(string? stored, int maxChars = 3)
03508: 	{
03509: 		try
03510: 		{
03511: 			if (!NullGuard.HasText(stored))
03512: 			{
03513: 				return null;
03514: 			}
03515: 			int num = stored.IndexOf('\u0001');
03516: 			if (num >= 0)
03517: 			{
03518: 				string text = stored.Substring(num + 1);
03519: 				int num2 = text.IndexOf('\u0001');
03520: 				if (num2 >= 0)
03521: 				{
03522: 					text = text.Substring(0, num2);
03523: 				}
03524: 				text = Core.Clean(text) ?? "";
03525: 				if (!string.IsNullOrEmpty(text))
03526: 				{
03527: 					if (text.Length > maxChars)
03528: 					{
03529: 						text = text.Substring(0, maxChars);
03530: 					}
03531: 					while (text.Length < maxChars)
03532: 					{
03533: 						text += "\u3000";
03534: 					}
03535: 					return text;
03536: 				}
03537: 			}
03538: 			if (CondMap.TryGetValue(stored, out CondEntry value))
03539: 			{
03540: 				string text2 = Core.Clean(value.Factory()?.display);
03541: 				if (string.IsNullOrEmpty(text2))
03542: 				{
03543: 					return null;
03544: 				}
03545: 				if (text2.Length > maxChars)
03546: 				{
03547: 					text2 = text2.Substring(0, maxChars);
03548: 				}
03549: 				while (text2.Length < maxChars)
03550: 				{
03551: 					text2 += "\u3000";
03552: 				}
03553: 				return text2;
03554: 			}
03555: 		}
03556: 		catch
03557: 		{
03558: 		}
03559: 		return null;
03560: 	}
03561: 
03562: 	private static string? TryReadSpriteAtlas(GameItem item)
03563: 	{
03564: 		try
03565: 		{
03566: 			return Core.Clean(item.spriteAtlasPath);
03567: 		}
03568: 		catch
03569: 		{
03570: 			return null;
03571: 		}
03572: 	}
03573: 
03574: 	private static string? TryReadSpritePath(GameItem item)
03575: 	{
03576: 		try
03577: 		{
03578: 			return Core.Clean(item.spritePath);
03579: 		}
03580: 		catch
03581: 		{
03582: 			return null;
03583: 		}
03584: 	}
03585: 
03586: 	private static string? TryReadCompSignature(GameItem item)
03587: 	{
03588: 		try
03589: 		{
03590: 			TagSystem state = item.state;
03591: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
03592: 			if (dictionary == null)
~~~

~~~csharp
03721: 		{
03722: 			text = "|W";
03723: 		}
03724: 		string text3;
03725: 		if (flag)
03726: 		{
03727: 			string text2 = "";
03728: 			try
03729: 			{
03730: 				text2 = Core.Clean(GeneralHelper.GetCustomName(item)) ?? "";
03731: 			}
03732: 			catch
03733: 			{
03734: 			}
03735: 			text3 = text2;
03736: 		}
03737: 		else
03738: 		{
03739: 			text3 = TryReadName(item) ?? "";
03740: 		}
03741: 		return text3 + "|" + (hasValue ? "" : (TryReadSpriteAtlas(item) ?? "")) + "|" + (hasValue ? "" : (TryReadSpritePath(item) ?? "")) + "|" + ((hasValue || flag) ? "" : (TryReadUnitValue(item)?.ToString() ?? "")) + "|" + StorageGrid.ReadItemCalorie(item) + "|" + StorageGrid.ReadItemShape(item) + "|" + (flag ? "" : (StorageGrid.ReadItemPurity(item) ?? "")) + text;
03742: 	}
03743: 
03744: 	private static void ConsumeDragged(GameItem dragged, int take, int units)
03745: 	{
03746: 		int num = Math.Max(0, units - take);
03747: 		try
03748: 		{
03749: 			dragged.unitCount = num;
03750: 			if (num <= 0)
03751: 			{
03752: 				try
03753: 				{
03754: 					dragged.Destroy();
03755: 				}
03756: 				catch
03757: 				{
03758: 					try
03759: 					{
03760: 						dragged.unitCount = 0;
03761: 					}
03762: 					catch
03763: 					{
03764: 					}
03765: 				}
03766: 				Core.Debug("收纳箱：消耗拖入物 unitCount→0 + Destroy");
03767: 			}
03768: 			else
03769: 			{
03770: 				Core.Debug("收纳箱：消耗拖入物 unitCount " + units + "→" + num);
03771: 			}
03772: 		}
03773: 		catch (Exception ex)
03774: 		{
03775: 			Core.Log?.Warning("收纳箱 ConsumeDragged：" + ex.Message);
03776: 		}
03777: 	}
03778: 
03779: 	internal static bool HasContentInside(GameItem it)
03780: 	{
03781: 		try
03782: 		{
03783: 			bool flag = false;
03784: 			bool flag2 = false;
03785: 			try
03786: 			{
03787: 				flag = it.GetCurrentChildValue(false, false, false, true) > 0;
03788: 			}
03789: 			catch
03790: 			{
03791: 			}
03792: 			try
03793: 			{
03794: 				List<GameItem> list = it.FindAllChildItems();
03795: 				flag2 = list != null && list.Count > 0;
03796: 			}
03797: 			catch
03798: 			{
03799: 			}
03800: 			if (flag != flag2)
03801: 			{
03802: 				try
03803: 				{
03804: 					Core.Debug("收纳箱 容器内物品双信号不一致：id=" + (Core.Clean(it.identifier) ?? "?") + " 按价值=" + flag + " 按列表=" + flag2);
03805: 				}
03806: 				catch
03807: 				{
03808: 				}
03809: 			}
03810: 			return flag || flag2;
03811: 		}
03812: 		catch
03813: 		{
03814: 			return false;
03815: 		}
03816: 	}
03817: 
03818: 	private static void TryRestoreBoughtPrice(GameItem pack, int slot, GameItem item)
03819: 	{
03820: 		try
03821: 		{
03822: 			try
03823: 			{
03824: 				if (StorageGrid.FindConditionFeature(item) == null)
03825: 				{
03826: 					return;
03827: 				}
03828: 			}
03829: 			catch
03830: 			{
03831: 			}
03832: 			long valueOrDefault = StorageGrid.GetUnitValue(pack, slot).GetValueOrDefault();
03833: 			if (valueOrDefault <= 0)
03834: 			{
03835: 				return;
03836: 			}
03837: 			try
03838: 			{
03839: 				TagSystem state = item.state;
03840: 				if (((state == null) ? ((bool?)null) : state.dict?.ContainsKey("BOUGHT_PRICE_TAG")) == true)
03841: 				{
03842: 					return;
03843: 				}
03844: 			}
03845: 			catch
03846: 			{
03847: 			}
03848: 			string text = "-";
03849: 			try
03850: 			{
03851: 				text = Core.Clean(item.identifier) ?? "-";
03852: 			}
03853: 			catch
03854: 			{
03855: 			}
03856: 			if (TryWriteBoughtTag(item, (int)Math.Min(valueOrDefault, 2147483647L), out string via))
03857: 			{
03858: 				Core.Debug("收纳箱 withdraw 买入价补写（基础价值 " + valueOrDefault + "）：'" + text + "'（" + via + "）——官方标签机激活链依赖该标签");
03859: 			}
03860: 			else
03861: 			{
03862: 				Core.Debug("收纳箱 withdraw 买入价补写失败：'" + text + "' 期望值=" + valueOrDefault + "（标签机改标可能仍报错，日志见写入路径异常）");
03863: 			}
03864: 		}
03865: 		catch
03866: 		{
03867: 		}
03868: 	}
03869: 
03870: 	private static bool TryWriteBoughtTag(GameItem item, int value, out string via)
03871: 	{
03872: 		try
03873: 		{
03874: 			TagSystem state = item.state;
03875: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
03876: 			if (dictionary != null)
03877: 			{
03878: 				if (!dictionary.ContainsKey("BOUGHT_PRICE_TAG"))
03879: 				{
03880: 					try
03881: 					{
03882: 						item.EnableTag("BOUGHT_PRICE_TAG", false);
03883: 					}
03884: 					catch
03885: 					{
03886: 					}
03887: 				}
03888: 				TagState val = (dictionary.ContainsKey("BOUGHT_PRICE_TAG") ? dictionary["BOUGHT_PRICE_TAG"] : null);
03889: 				if (val != null)
03890: 				{
03891: 					try
03892: 					{
03893: 						val.Enable();
03894: 					}
03895: 					catch
03896: 					{
03897: 					}
03898: 					try
03899: 					{
03900: 						val.SetInt(value);
03901: 					}
03902: 					catch
03903: 					{
03904: 					}
03905: 					if (VerifyBoughtTag(item, value))
03906: 					{
03907: 						via = "dict直写";
03908: 						return true;
03909: 					}
03910: 				}
03911: 			}
03912: 		}
03913: 		catch
03914: 		{
03915: 		}
03916: 		try
03917: 		{
03918: 			item.ModifyTag("BOUGHT_PRICE_TAG", (Action<TagState>)delegate(TagState t)
03919: 			{
03920: 				try
03921: 				{
03922: 					if (t != null)
03923: 					{
03924: 						t.Enable();
03925: 					}
03926: 				}
03927: 				catch
03928: 				{
03929: 				}
03930: 				try
03931: 				{
03932: 					if (t != null)
03933: 					{
03934: 						t.SetInt(value);
03935: 					}
03936: 				}
03937: 				catch
03938: 				{
03939: 				}
03940: 			}, true);
03941: 			if (VerifyBoughtTag(item, value))
03942: 			{
03943: 				via = "ModifyTag";
03944: 				return true;
03945: 			}
03946: 		}
03947: 		catch
03948: 		{
03949: 		}
03950: 		via = "";
03951: 		return false;
~~~

### ItemStorage/BoxWindow.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Reflection;
00004: using System.Text;
00005: using HarmonyLib;
00006: using TMPro;
00007: using UnityEngine;
00008: using UnityEngine.EventSystems;
00009: using UnityEngine.UI;
00010: 
00011: namespace ItemStorage;
00012: 
00013: internal static class BoxWindow
00014: {
00015: 	private sealed class SlotClickCatcher : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
00016: 	{
00017: 		internal GameItem Pack;
00018: 
00019: 		internal int Slot;
00020: 
00021: 		internal Action? Refresh;
00022: 
00023: 		public void OnPointerClick(PointerEventData eventData)
00024: 		{
00025: 			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
00026: 			if ((int)eventData.button != 0)
00027: 			{
00028: 				return;
00029: 			}
00030: 			try
00031: 			{
00032: 				if (SlotLock.Toggle(Pack, Slot).HasValue)
00033: 				{
00034: 					Refresh?.Invoke();
00035: 				}
00036: 			}
00037: 			catch (Exception ex)
00038: 			{
00039: 				Core.Log?.Warning("收纳箱 槽 " + Slot + " 锁定点击： " + ex.Message);
00040: 			}
00041: 		}
00042: 	}
00043: 
00044: 	private sealed class RetryState
00045: 	{
00046: 		internal float Time;
00047: 
00048: 		internal int Fails;
00049: 	}
00050: 
00051: 	private static readonly Dictionary<GameItem, Dictionary<string, AdvButtonElement>> ButtonsByRef = new Dictionary<GameItem, Dictionary<string, AdvButtonElement>>();
00052: 
00053: 	private static readonly Dictionary<string, Dictionary<string, AdvButtonElement>> ButtonsByUid = new Dictionary<string, Dictionary<string, AdvButtonElement>>();
00054: 
00055: 	private static readonly Dictionary<GameItem, Dictionary<string, MachineryLabel>> LabelsByRef = new Dictionary<GameItem, Dictionary<string, MachineryLabel>>();
00056: 
00057: 	private static readonly Dictionary<string, Dictionary<string, MachineryLabel>> LabelsByUid = new Dictionary<string, Dictionary<string, MachineryLabel>>();
00058: 
00059: 	private static readonly Dictionary<GameItem, Dictionary<string, Image>> IconsByRef = new Dictionary<GameItem, Dictionary<string, Image>>();
00060: 
00061: 	private static readonly Dictionary<string, Dictionary<string, Image>> IconsByUid = new Dictionary<string, Dictionary<string, Image>>();
00062: 
00063: 	private static readonly Dictionary<GameItem, Dictionary<string, GameObject>> ClicksByRef = new Dictionary<GameItem, Dictionary<string, GameObject>>();
00064: 
00065: 	private static readonly Dictionary<string, Dictionary<string, GameObject>> ClicksByUid = new Dictionary<string, Dictionary<string, GameObject>>();
00066: 
00067: 	private static readonly List<GameItem> TrackedBoxes = new List<GameItem>();
00068: 
00069: 	private static readonly Dictionary<string, Sprite?> SpriteCache = new Dictionary<string, Sprite>();
00070: 
00071: 	private static readonly List<GameItem> IconRefreshItems = new List<GameItem>();
00072: 
00073: 	private static readonly List<float> IconRefreshAt = new List<float>();
00074: 
00075: 	private static readonly List<int> IconRetryTried = new List<int>();
00076: 
00077: 	private const float IconRetryFirst = 0.6f;
00078: 
00079: 	private const float IconRetryInterval = 1.5f;
00080: 
00081: 	private const int IconRetryMax = 40;
00082: 
00083: 	private static bool _rebuilding;
00084: 
00085: 	private static Font? _gameFont;
00086: 
00087: 	private static readonly Color TitleColor = new Color(0.69803923f, 0.41960785f, 26f / 85f, 1f);
00088: 
00089: 	private static readonly FieldInfo? BtnTextColorField = AccessTools.Field(typeof(AdvButtonElement), "buttonTextColorNormal");
00090: 
00091: 	private static readonly Dictionary<string, Func<GameItem, List<ItemSpriteModifier>>?> SlotModDelegates = new Dictionary<string, Func<GameItem, List<ItemSpriteModifier>>>(StringComparer.Ordinal);
00092: 
00093: 	private static Sprite? _placeholder;
00094: 
00095: 	private const int ResolveMaxFails = 2;
00096: 
00097: 	private const float ResolveRetryGap = 60f;
00098: 
00099: 	private static readonly Dictionary<string, RetryState> WarmTried = new Dictionary<string, RetryState>(StringComparer.Ordinal);
00100: 
00101: 	private static readonly Dictionary<string, RetryState> ProbeTried = new Dictionary<string, RetryState>(StringComparer.Ordinal);
00102: 
00103: 	internal static bool EnsureWindow(GameItem pack, string context)
00104: 	{
00105: 		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
00106: 		//IL_017e: Expected O, but got Unknown
00107: 		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
00108: 		//IL_029d: Expected O, but got Unknown
00109: 		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
00110: 		//IL_01b0: Expected O, but got Unknown
00111: 		//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
00112: 		//IL_08f7: Expected O, but got Unknown
00113: 		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
00114: 		//IL_023b: Expected O, but got Unknown
00115: 		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
00116: 		//IL_0772: Expected O, but got Unknown
00117: 		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
00118: 		//IL_083e: Expected O, but got Unknown
00119: 		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
00120: 		//IL_0352: Expected O, but got Unknown
00121: 		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
00122: 		//IL_06a6: Expected O, but got Unknown
00123: 		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
00124: 		//IL_0424: Expected O, but got Unknown
00125: 		if (pack == null)
00126: 		{
00127: 			return false;
00128: 		}
00129: 		if (!BoxItem.IsBox(pack))
00130: 		{
00131: 			return false;
00132: 		}
00133: 		try
00134: 		{
00135: 			ContainerHelper.AddContainerTooltipTag(pack);
00136: 		}
00137: 		catch
00138: 		{
00139: 		}
00140: 		GhostSweep.Run(context);
00141: 		try
00142: 		{
00143: 			bool flag = false;
00144: 			try
00145: 			{
00146: 				flag = pack.IsTag("ITEM_STORAGE_BOX_UI_V30");
00147: 			}
00148: 			catch
00149: 			{
00150: 			}
00151: 			Dictionary<string, AdvButtonElement> dictionary = FindIn(ButtonsByRef, ButtonsByUid, pack);
00152: 			if (pack.contentWindow != null && flag && dictionary != null)
00153: 			{
00154: 				TrackBox(pack);
00155: 				SyncHotkeyMap(pack);
00156: 				RefreshButtons(pack);
00157: 				return true;
00158: 			}
00159: 		}
00160: 		catch
00161: 		{
00162: 		}
00163: 		try
00164: 		{
00165: 			pack.SetContentWindow((PixelWindow)null);
00166: 		}
00167: 		catch
00168: 		{
00169: 		}
00170: 		string text = "收纳箱";
00171: 		if (string.IsNullOrEmpty(pack.name) || LooksLikeLocError(pack.name) || string.Equals(pack.name, "收纳盒", StringComparison.Ordinal))
00172: 		{
00173: 			try
00174: 			{
00175: 				pack.SetName(text);
00176: 			}
00177: 			catch
00178: 			{
00179: 			}
00180: 			pack.name = text;
00181: 		}
00182: 		try
00183: 		{
00184: 			if (string.Equals(Core.Clean(pack.spritePath), "item_storage", StringComparison.Ordinal))
00185: 			{
00186: 				pack.SetSprite(string.Empty, "Dream_Item_Storage");
00187: 				Core.Debug("收纳箱：旧贴图键 item_storage → Dream_Item_Storage");
00188: 			}
00189: 		}
00190: 		catch
00191: 		{
00192: 		}
00193: 		try
00194: 		{
00195: 			PixelWindow val = new PixelWindow(true, text, (WindowStyle)1);
00196: 			try
00197: 			{
00198: 				Sprite val2 = SpriteAssets.Get("Dream_Item_Storage_Gear");
00199: 				if ((Object)(object)val2 != (Object)null)
00200: 				{
00201: 					MachineryButton val3 = new MachineryButton(val2, MakeAction(delegate
00202: 					{
00203: 						try
00204: 						{
00205: 							GearPanel.ToggleFor(pack);
00206: 						}
00207: 						catch
00208: 						{
00209: 						}
00210: 					}), 16);
00211: 					val.AddHeaderButton(val3);
00212: 				}
00213: 				else
00214: 				{
00215: 					Core.Debug("收纳箱[" + context + "] 齿轮图标未加载，标题栏按钮跳过。");
00216: 				}
00217: 			}
00218: 			catch (Exception ex)
00219: 			{
00220: 				Core.Log?.Warning("收纳箱[" + context + "] 标题栏齿轮挂载失败：" + ex.Message);
00221: 			}
00222: 			try
00223: 			{
00224: 				Sprite val4 = SpriteAssets.Get("Dream_Item_Storage_Tidy");
00225: 				if ((Object)(object)val4 != (Object)null)
00226: 				{
00227: 					MachineryButton val5 = new MachineryButton(val4, MakeAction(delegate
00228: 					{
00229: 						try
00230: 						{
00231: 							int num4 = BoxTidy.Tidy(pack);
00232: 							if (num4 > 0)
00233: 							{
00234: 								RefreshButtons(pack);
00235: 								BoxItem.NotifyUser("已整理：挪动 " + num4 + " 件");
00236: 							}
00237: 							else
00238: 							{
00239: 								BoxItem.NotifyUser("目前顺序已经无法整理了");
00240: 							}
00241: 						}
00242: 						catch (Exception ex9)
00243: 						{
00244: 							Core.Log?.Warning("收纳箱[" + context + "] 一键整理失败：" + ex9.Message);
00245: 						}
00246: 					}), 16);
00247: 					val.AddHeaderButton(val5);
00248: 				}
00249: 				else
00250: 				{
00251: 					Core.Debug("收纳箱[" + context + "] 整理图标未加载，标题栏按钮跳过。");
00252: 				}
00253: 			}
00254: 			catch (Exception ex2)
00255: 			{
00256: 				Core.Log?.Warning("收纳箱[" + context + "] 标题栏整理按钮挂载失败：" + ex2.Message);
00257: 			}
00258: 			GridPixelElement val6 = new GridPixelElement(4, 5, false);
00259: 			val6.SetSpacing(8, 3);
00260: 			Dictionary<string, AdvButtonElement> dictionary2 = new Dictionary<string, AdvButtonElement>(StringComparer.Ordinal);
00261: 			Dictionary<string, MachineryLabel> dictionary3 = new Dictionary<string, MachineryLabel>(StringComparer.Ordinal);
00262: 			Dictionary<string, Image> dictionary4 = new Dictionary<string, Image>(StringComparer.Ordinal);
00263: 			Dictionary<string, GameObject> dictionary5 = new Dictionary<string, GameObject>(StringComparer.Ordinal);
00264: 			for (int num = 0; num < 16; num++)
00265: 			{
00266: 				int num2 = num % 4;
00267: 				int num3 = num / 4;
00268: 				int slotIndex = num;
00269: 				try
00270: 				{
00271: 					Action click = MakeAction(delegate
00272: 					{
00273: 						try
00274: 						{
00275: 							if (StoragePatch.TryRegisterClickDeposit(pack, slotIndex) == false)
00276: 							{
00277: 								GameGridInventory val19 = (BoxTransfer.AltHeld() ? BoxTransfer.ResolveCounterDrop() : null);
00278: 								BoxTransfer.WithdrawResult withdrawResult = BoxTransfer.TryWithdraw(pack, slotIndex, val19);
00279: 								if (withdrawResult == BoxTransfer.WithdrawResult.NoSpace || withdrawResult == BoxTransfer.WithdrawResult.NotOnGrid)
00280: 								{
00281: 									BoxItem.NotifyUser(BoxTransfer.SpaceShort(val19 != null) + "，无法取出物品");
00282: 								}
00283: 							}
00284: 						}
00285: 						catch
00286: 						{
00287: 						}
00288: 					});
00289: 					TooltipSlotButton tooltipSlotButton = new TooltipSlotButton(pack, slotIndex, click);
00290: 					dictionary2[slotIndex.ToString()] = (AdvButtonElement)(object)tooltipSlotButton;
00291: 					GridPixelElement val7 = new GridPixelElement(1, 2, false);
00292: 					val7.SetSpacing(2, 2);
00293: 					PixelElement val8 = (PixelElement)(object)tooltipSlotButton;
00294: 					if (val8 == null)
00295: 					{
00296: 						Core.Log?.Warning("收纳箱[" + context + "] 槽 " + slotIndex + " 按钮非 PixelElement");
00297: 					}
00298: 					else if (!val7.Attach(val8, 0, 0, 48, 48))
00299: 					{
00300: 						Core.Log?.Warning("收纳箱[" + context + "] attach 按钮 " + slotIndex + " 失败");
00301: 					}
00302: 					MachineryLabel val9 = new MachineryLabel((string)null);
00303: 					val9.SetText(SlotText(pack, slotIndex));
00304: 					val9.SetWidth(48);
00305: 					dictionary3[slotIndex.ToString()] = val9;
00306: 					PixelElement val10 = (PixelElement)(object)val9;
00307: 					if (val10 != null && !val7.Attach(val10, 0, 1, 48, 8))
00308: 					{
00309: 						Core.Log?.Warning("收纳箱[" + context + "] attach 标签 " + slotIndex + " 失败");
00310: 					}
00311: 					GameObject val11 = CreateSlotClickLayer(val9, pack, slotIndex, context);
00312: 					if ((Object)(object)val11 != (Object)null)
00313: 					{
00314: 						dictionary5[slotIndex.ToString()] = val11;
00315: 					}
00316: 					Image val12 = CreateSlotIcon((AdvButtonElement)(object)tooltipSlotButton, context, slotIndex);
00317: 					if ((Object)(object)val12 != (Object)null)
00318: 					{
00319: 						dictionary4[slotIndex.ToString()] = val12;
00320: 					}
00321: 					PixelElement val13 = (PixelElement)(object)val7;
00322: 					if (val13 == null)
00323: 					{
00324: 						Core.Log?.Warning("收纳箱[" + context + "] 槽 " + slotIndex + " 非 PixelElement");
00325: 					}
00326: 					else if (!val6.Attach(val13, num2, num3, 48, 58))
00327: 					{
00328: 						Core.Log?.Warning("收纳箱[" + context + "] attach 槽位 " + slotIndex + " 失败");
00329: 					}
00330: 				}
00331: 				catch (Exception ex3)
00332: 				{
00333: 					Core.Log?.Warning("收纳箱[" + context + "] 槽 " + slotIndex + " 构建失败：" + ex3.Message);
00334: 				}
00335: 			}
00336: 			try
00337: 			{
00338: 				Action action = MakeAction(delegate
00339: 				{
00340: 					try
00341: 					{
00342: 						AutoStow.Run(pack, "按钮");
00343: 					}
00344: 					catch
00345: 					{
00346: 					}
00347: 				});
00348: 				PixelElement val14 = (PixelElement)new MachineryButton("一键收纳", action, 48, 24);
00349: 				if (val14 == null)
00350: 				{
00351: 					Core.Log?.Warning("收纳箱[" + context + "] 一键收纳按钮非 PixelElement");
00352: 				}
00353: 				else if (!val6.Attach(val14, 3, 4, 48, 24))
00354: 				{
00355: 					Core.Log?.Warning("收纳箱[" + context + "] attach 一键收纳按钮失败");
00356: 				}
00357: 			}
00358: 			catch (Exception ex4)
00359: 			{
00360: 				Core.Log?.Warning("收纳箱[" + context + "] 一键收纳按钮构建失败：" + ex4.Message);
00361: 			}
00362: 			try
00363: 			{
00364: 				Action action2 = MakeAction(delegate
00365: 				{
00366: 					try
00367: 					{
00368: 						BoxStowAll.RunAll("按钮");
00369: 					}
00370: 					catch
00371: 					{
00372: 					}
00373: 				});
00374: 				PixelElement val15 = (PixelElement)new MachineryButton("全箱收纳", action2, 48, 24);
00375: 				if (val15 == null)
00376: 				{
00377: 					Core.Log?.Warning("收纳箱[" + context + "] 全箱收纳按钮非 PixelElement");
00378: 				}
00379: 				else if (!val6.Attach(val15, 0, 4, 48, 24))
00380: 				{
00381: 					Core.Log?.Warning("收纳箱[" + context + "] attach 全箱收纳按钮失败");
00382: 				}
00383: 			}
00384: 			catch (Exception ex5)
00385: 			{
00386: 				Core.Log?.Warning("收纳箱[" + context + "] 全箱收纳按钮构建失败：" + ex5.Message);
00387: 			}
00388: 			try
00389: 			{
00390: 				Action action3 = MakeAction(delegate
00391: 				{
00392: 					try
00393: 					{
00394: 						BoxRestock.RunAllBoxes("按钮");
00395: 					}
00396: 					catch
00397: 					{
00398: 					}
00399: 				});
00400: 				PixelElement val16 = (PixelElement)new MachineryButton("全箱上货", action3, 48, 24);
00401: 				if (val16 == null)
00402: 				{
00403: 					Core.Log?.Warning("收纳箱[" + context + "] 全箱上货按钮非 PixelElement");
00404: 				}
00405: 				else if (!val6.Attach(val16, 1, 4, 48, 24))
00406: 				{
00407: 					Core.Log?.Warning("收纳箱[" + context + "] attach 全箱上货按钮失败");
00408: 				}
00409: 			}
00410: 			catch (Exception ex6)
00411: 			{
00412: 				Core.Log?.Warning("收纳箱[" + context + "] 全箱上货按钮构建失败：" + ex6.Message);
00413: 			}
00414: 			try
00415: 			{
00416: 				Action action4 = MakeAction(delegate
00417: 				{
00418: 					try
00419: 					{
00420: 						BoxRestock.RunOne(pack, "按钮");
00421: 					}
00422: 					catch
00423: 					{
00424: 					}
00425: 				});
00426: 				PixelElement val17 = (PixelElement)new MachineryButton("一键上货", action4, 48, 24);
00427: 				if (val17 == null)
00428: 				{
00429: 					Core.Log?.Warning("收纳箱[" + context + "] 一键上货按钮非 PixelElement");
00430: 				}
00431: 				else if (!val6.Attach(val17, 2, 4, 48, 24))
00432: 				{
00433: 					Core.Log?.Warning("收纳箱[" + context + "] attach 一键上货按钮失败");
00434: 				}
00435: 			}
00436: 			catch (Exception ex7)
00437: 			{
00438: 				Core.Log?.Warning("收纳箱[" + context + "] 一键上货按钮构建失败：" + ex7.Message);
00439: 			}
00440: 			SyncHotkeyMap(pack);
00441: 			StoreIn(ButtonsByRef, ButtonsByUid, pack, dictionary2);
00442: 			StoreIn(LabelsByRef, LabelsByUid, pack, dictionary3);
00443: 			StoreIn(IconsByRef, IconsByUid, pack, dictionary4);
00444: 			StoreIn(ClicksByRef, ClicksByUid, pack, dictionary5);
00445: 			TrackBox(pack);
00446: 			PixelElement val18 = (PixelElement)(object)val6;
00447: 			if (val18 == null)
00448: 			{
00449: 				Core.Log?.Warning("收纳箱[" + context + "]：carrier 不是 PixelElement");
00450: 				return false;
00451: 			}
00452: 			val.Attach(val18);
00453: 			pack.SetContentWindow(val);
00454: 			try
00455: 			{
00456: 				pack.EnableTag("ITEM_STORAGE_BOX_UI_V30", false);
00457: 			}
00458: 			catch
00459: 			{
00460: 			}
00461: 			try
00462: 			{
00463: 				val.Validate();
00464: 			}
00465: 			catch
00466: 			{
00467: 			}
00468: 			RefreshButtons(pack);
00469: 			QueueIconRefresh(pack);
00470: 			Core.Debug("收纳箱[" + context + "]：窗口构建完成（4×4 十六槽全满 + 网格下方横排一键收纳按钮 48×24）");
00471: 			return true;
00472: 		}
00473: 		catch (Exception ex8)
00474: 		{
00475: 			Core.Log?.Warning("收纳箱[" + context + "] EnsureWindow：" + ex8.GetType().Name + ": " + ex8.Message);
00476: 			return false;
00477: 		}
00478: 	}
00479: 
00480: 	internal static void RefreshButtons(GameItem pack)
00481: 	{
00482: 		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
00483: 		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
00484: 		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
00485: 		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
00486: 		if (pack == null)
00487: 		{
00488: 			return;
00489: 		}
00490: 		try
00491: 		{
00492: 			Dictionary<string, AdvButtonElement> dictionary = FindIn(ButtonsByRef, ButtonsByUid, pack);
00493: 			if (dictionary == null)
00494: 			{
00495: 				if (!_rebuilding)
00496: 				{
00497: 					Core.Debug("收纳箱 RefreshButtons：无按钮引用，重建窗口");
00498: 					ForceRebuildWindow(pack, "RefreshMiss");
00499: 				}
00500: 				return;
00501: 			}
00502: 			StoreIn(ButtonsByRef, ButtonsByUid, pack, dictionary);
00503: 			Dictionary<string, MachineryLabel> dictionary2 = FindIn(LabelsByRef, LabelsByUid, pack);
00504: 			Dictionary<string, Image> dictionary3 = FindIn(IconsByRef, IconsByUid, pack);
00505: 			for (int i = 0; i < 16; i++)
00506: 			{
00507: 				if (dictionary2 != null && dictionary2.TryGetValue(i.ToString(), out var value) && value != null)
00508: 				{
00509: 					try
00510: 					{
00511: 						value.SetText(SlotText(pack, i));
00512: 						try
00513: 						{
00514: 							bool flag = SlotLock.IsLocked(pack, i);
00515: 							value.SetTextColor(flag ? SlotLock.LockTextColor() : SlotLock.LabelColor());
00516: 							try
00517: 							{
00518: 								value.SetPlateColor(flag ? SlotLock.LockPlateColor() : SlotLock.PlateColor());
00519: 							}
00520: 							catch
00521: 							{
00522: 							}
00523: 						}
00524: 						catch
00525: 						{
00526: 						}
00527: 					}
00528: 					catch
00529: 					{
00530: 					}
00531: 				}
00532: 				if (dictionary3 != null && dictionary3.TryGetValue(i.ToString(), out var value2) && (Object)(object)value2 != (Object)null)
00533: 				{
00534: 					UpdateSlotIcon(pack, i, value2);
00535: 				}
00536: 			}
00537: 			UpdateTooltip(pack);
00538: 		}
00539: 		catch (Exception ex)
00540: 		{
00541: 			Core.Log?.Warning("收纳箱 RefreshButtons：" + ex.Message);
00542: 		}
00543: 	}
00544: 
00545: 	internal static void ForceRebuildWindow(GameItem pack, string reason)
00546: 	{
00547: 		if (pack == null || _rebuilding)
00548: 		{
00549: 			return;
00550: 		}
00551: 		_rebuilding = true;
00552: 		try
00553: 		{
00554: 			ForgetIn(ButtonsByRef, ButtonsByUid, pack);
00555: 			ForgetIn(LabelsByRef, LabelsByUid, pack);
00556: 			ForgetIn(IconsByRef, IconsByUid, pack);
00557: 			ForgetIn(ClicksByRef, ClicksByUid, pack);
00558: 			UntrackBox(pack);
00559: 			GearPanel.CloseFor(pack);
00560: 			try
00561: 			{
00562: 				pack.SetContentWindow((PixelWindow)null);
00563: 			}
00564: 			catch
00565: 			{
00566: 			}
00567: 			try
00568: 			{
00569: 				pack.DisableTag("ITEM_STORAGE_BOX_UI_V30", true);
00570: 			}
00571: 			catch
00572: 			{
00573: 				try
00574: 				{
00575: 					pack.DisableTag("ITEM_STORAGE_BOX_UI_V30", false);
00576: 				}
00577: 				catch
00578: 				{
00579: 				}
00580: 			}
00581: 			EnsureWindow(pack, reason);
00582: 		}
00583: 		finally
00584: 		{
00585: 			_rebuilding = false;
00586: 		}
00587: 	}
00588: 
00589: 	internal static string SlotText(GameItem pack, int slot)
00590: 	{
00591: 		string itemId = StorageGrid.GetItemId(pack, slot);
00592: 		if (itemId == null)
00593: 		{
00594: 			return "空";
00595: 		}
00596: 		if (StorageGrid.IsWaterSlot(pack, slot) && StorageGrid.GetStoredWaterML(pack, slot) > 0)
00597: 		{
00598: 			string text = BoxTransfer.WaterConditionName(StorageGrid.GetStoredWaterPurity(pack, slot));
00599: 			if (!string.IsNullOrEmpty(text))
00600: 			{
00601: 				return text + "×" + StorageGrid.GetCount(pack, slot);
00602: 			}
00603: 		}
00604: 		string purity = StorageGrid.GetPurity(pack, slot);
00605: 		if (NullGuard.HasText(purity))
00606: 		{
00607: 			string text2 = BoxTransfer.PurityShortName(purity, 4);
00608: 			if (!string.IsNullOrEmpty(text2))
00609: 			{
00610: 				return text2 + "×" + StorageGrid.GetCount(pack, slot);
00611: 			}
00612: 		}
00613: 		string text3 = StorageGrid.GetDisplayName(pack, slot);
00614: 		if (!NullGuard.HasText(text3))
00615: 		{
00616: 			text3 = DisplayNameForItem(itemId, 4);
00617: 		}
00618: 		else if (text3.Length > 4)
00619: 		{
00620: 			text3 = text3.Substring(0, 4);
00621: 		}
00622: 		return text3 + "×" + StorageGrid.GetCount(pack, slot);
00623: 	}
00624: 
00625: 	internal static string DisplayNameForItem(string itemId, int maxChars = 3)
00626: 	{
00627: 		string text = itemId;
00628: 		try
00629: 		{
00630: 			string localizedItem = LocHelper.GetLocalizedItem("item_" + itemId + "_name", Array.Empty<object>());
00631: 			if (!string.IsNullOrEmpty(localizedItem) && !localizedItem.StartsWith("Translation Error", StringComparison.Ordinal) && !localizedItem.Contains("item_" + itemId + "_name"))
00632: 			{
00633: 				text = localizedItem;
00634: 			}
00635: 		}
00636: 		catch
00637: 		{
00638: 		}
00639: 		if ((object)text == itemId)
00640: 		{
00641: 			try
00642: 			{
00643: 				GameItem val = DirectoryMaster.Item(itemId, false);
00644: 				string text2 = ((val != null) ? val.GetDisplayName(true) : null);
00645: 				if (string.IsNullOrWhiteSpace(text2))
00646: 				{
00647: 					text2 = ((val != null) ? val.name : null);
00648: 				}
00649: 				if (!string.IsNullOrWhiteSpace(text2))
00650: 				{
00651: 					text = text2;
00652: 				}
00653: 			}
00654: 			catch
00655: 			{
00656: 			}
00657: 		}
00658: 		text = Core.Clean(text);
00659: 		if (text.Length > maxChars)
00660: 		{
00661: 			text = text.Substring(0, maxChars);
00662: 		}
00663: 		return text;
00664: 	}
00665: 
00666: 	internal static void UpdateTooltip(GameItem pack)
00667: 	{
00668: 		if (pack == null)
00669: 		{
00670: 			return;
00671: 		}
00672: 		try
00673: 		{
00674: 			try
00675: 			{
00676: 				pack.shortDescription = "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。";
00677: 			}
00678: 			catch (Exception ex)
00679: 			{
00680: 				Core.Log?.Warning("收纳箱 shortDescription 写入：" + ex.Message);
00681: 			}
00682: 			try
00683: 			{
00684: 				string shortDescription = pack.shortDescription;
00685: 				if (!string.Equals(shortDescription, "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。", StringComparison.Ordinal))
00686: 				{
00687: 					Core.Debug("收纳箱 shortDescription 读回不一致：'" + Core.Clean(shortDescription) + "'");
00688: 				}
00689: 			}
00690: 			catch (Exception ex2)
00691: 			{
00692: 				Core.Log?.Warning("收纳箱 shortDescription 读回失败（getter 缺失？）：" + ex2.Message);
00693: 			}
00694: 			try
00695: 			{
00696: 				pack.longDescription = "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。";
00697: 			}
00698: 			catch
00699: 			{
00700: 			}
00701: 			pack.flavorText = string.Empty;
00702: 		}
00703: 		catch (Exception ex3)
00704: 		{
00705: 			Core.Log?.Warning("收纳箱 UpdateTooltip：" + ex3.Message);
00706: 		}
00707: 	}
00708: 
00709: 	private static Action MakeAction(Action action)
00710: 	{
00711: 		return action;
00712: 	}
00713: 
00714: 	internal static Font? GameFont()
00715: 	{
00716: 		if ((Object)(object)_gameFont != (Object)null)
00717: 		{
00718: 			try
00719: 			{
00720: 				if ((Object)(object)_gameFont != (Object)null)
00721: 				{
00722: 					return _gameFont;
00723: 				}
00724: 			}
00725: 			catch
00726: 			{
00727: 			}
00728: 			_gameFont = null;
00729: 		}
00730: 		try
00731: 		{
00732: 			Text[] array = Object.FindObjectsOfType<Text>();
00733: 			Text[] array2 = array;
00734: 			foreach (Text val in array2)
00735: 			{
00736: 				try
00737: 				{
00738: 					if ((Object)(object)val != (Object)null && (Object)(object)val.font != (Object)null && (Object)(object)val.font != (Object)null)
00739: 					{
00740: 						_gameFont = val.font;
00741: 						Core.Debug("收纳箱：按钮字体已抓取（来源=场景 Text，扫到 " + array.Length + " 个）。");
00742: 						return _gameFont;
00743: 					}
00744: 				}
00745: 				catch
00746: 				{
00747: 				}
00748: 			}
00749: 		}
00750: 		catch
00751: 		{
00752: 		}
00753: 		try
00754: 		{
00755: 			Font[] array3 = Resources.FindObjectsOfTypeAll<Font>();
00756: 			Font[] array4 = array3;
00757: 			foreach (Font val2 in array4)
00758: 			{
00759: 				try
00760: 				{
00761: 					if (!((Object)(object)val2 == (Object)null) && !((Object)(object)val2 == (Object)null))
00762: 					{
00763: 						string text = ((Object)val2).name ?? string.Empty;
00764: 						if (text.Length != 0 && text.IndexOf("Arial", StringComparison.OrdinalIgnoreCase) < 0 && text.IndexOf("LegacyRuntime", StringComparison.OrdinalIgnoreCase) < 0)
00765: 						{
00766: 							_gameFont = val2;
00767: 							Core.Debug("收纳箱：按钮字体已抓取（来源=内存 Font：" + text + "，共 " + array3.Length + " 个）。");
00768: 							return _gameFont;
00769: 						}
00770: 					}
00771: 				}
00772: 				catch
00773: 				{
00774: 				}
00775: 			}
00776: 		}
00777: 		catch
00778: 		{
00779: 		}
00780: 		Core.Debug("收纳箱：游戏字体抓取失败（场景 Text 与内存 Font 均未拿到）。");
00781: 		return null;
00782: 	}
00783: 
00784: 	private static Color ButtonTextColor(AdvButtonElement? button)
00785: 	{
00786: 		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
00787: 		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
00788: 		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
00789: 		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
00790: 		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
00791: 		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
00792: 		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
00793: 		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
00794: 		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
00795: 		try
00796: 		{
00797: 			if (button != null && BtnTextColorField != null && BtnTextColorField.GetValue(button) is Color32 val)
00798: 			{
00799: 				return new Color((float)(int)val.r / 255f, (float)(int)val.g / 255f, (float)(int)val.b / 255f, 1f);
00800: 			}
00801: 		}
00802: 		catch
00803: 		{
00804: 		}
00805: 		return TitleColor;
00806: 	}
00807: 
00808: 	private unsafe static Text? AttachButtonLabel(AdvButtonElement button, string text, string context)
00809: 	{
00810: 		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
00811: 		//IL_004a: Expected O, but got Unknown
00812: 		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
00813: 		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
00814: 		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
00815: 		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
00816: 		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
00817: 		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
00818: 		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
00819: 		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
00820: 		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
00821: 		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
00822: 		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
00823: 		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
00824: 		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
00825: 		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
00826: 		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
00827: 		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
00828: 		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
00829: 		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
00830: 		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
00831: 		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
00832: 		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
00833: 		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
00834: 		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
00835: 		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
00836: 		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
00837: 		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
00838: 		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
00839: 		try
00840: 		{
00841: 			object obj;
00842: 			if (button == null)
00843: 			{
00844: 				obj = null;
00845: 			}
00846: 			else
00847: 			{
00848: 				TreeNodeRender handler = ((SelectableElement)button).handler;
00849: 				obj = ((handler != null) ? handler.rectTransform : null);
00850: 			}
00851: 			RectTransform val = (RectTransform)obj;
00852: 			if ((Object)(object)val == (Object)null)
00853: 			{
00854: 				Core.Debug("收纳箱[" + context + "] 按钮文字：rectTransform=null");
00855: 				return null;
00856: 			}
00857: 			GameObject val2 = new GameObject("item_storage_stow_label");
00858: 			val2.transform.SetParent((Transform)(object)val, false);
00859: 			Transform transform = val2.transform;
00860: 			RectTransform val3 = (RectTransform)(((object)((transform is RectTransform) ? transform : null)) ?? ((object)val2.AddComponent<RectTransform>()));
00861: 			Rect rect = val.rect;
00862: 			float num = ((Rect)(ref rect)).width;
00863: 			if (num < 1f)
00864: 			{
00865: 				num = 48f;
00866: 			}
00867: 			rect = val.rect;
00868: 			float num2 = ((Rect)(ref rect)).height;
00869: 			if (num2 < 1f)
00870: 			{
00871: 				num2 = 24f;
00872: 			}
00873: 			val3.anchorMin = new Vector2(0f, 1f);
00874: 			val3.anchorMax = new Vector2(0f, 1f);
00875: 			val3.pivot = new Vector2(0.5f, 0.5f);
00876: 			val3.anchoredPosition = new Vector2(num / 2f, (0f - num2) / 2f);
00877: 			val3.sizeDelta = new Vector2(num, num2);
00878: 			((Transform)val3).localScale = Vector3.one;
00879: 			((Transform)val3).SetAsLastSibling();
00880: 			Text val4 = val2.AddComponent<Text>();
00881: 			val4.font = _gameFont;
00882: 			val4.fontSize = 10;
00883: 			val4.text = text;
00884: 			val4.alignment = (TextAnchor)4;
00885: 			val4.horizontalOverflow = (HorizontalWrapMode)1;
00886: 			val4.verticalOverflow = (VerticalWrapMode)1;
00887: 			((Graphic)val4).color = ButtonTextColor(button);
00888: 			((Graphic)val4).raycastTarget = false;
00889: 			val2.SetActive(true);
00890: 			try
00891: 			{
00892: 				string[] obj2 = new string[18]
00893: 				{
00894: 					"收纳箱[", context, "] 按钮文字诊断：rt.rect=", null, null, null, null, null, null, null,
00895: 					null, null, null, null, null, null, null, null
00896: 				};
00897: 				rect = val.rect;
00898: 				obj2[3] = ((object)(*(Rect*)(&rect))/*cast due to .constrained prefix*/).ToString();
00899: 				obj2[4] = " sizeDelta=";
00900: 				obj2[5] = ((object)val.sizeDelta/*cast due to .constrained prefix*/).ToString();
00901: 				obj2[6] = " pos=";
00902: 				obj2[7] = ((object)val.anchoredPosition/*cast due to .constrained prefix*/).ToString();
00903: 				obj2[8] = " pivot=";
00904: 				obj2[9] = ((object)val.pivot/*cast due to .constrained prefix*/).ToString();
00905: 				obj2[10] = " anchor=";
00906: 				obj2[11] = ((object)val.anchorMin/*cast due to .constrained prefix*/).ToString();
00907: 				obj2[12] = "~";
00908: 				obj2[13] = ((object)val.anchorMax/*cast due to .constrained prefix*/).ToString();
00909: 				obj2[14] = " → 文字 pos=";
00910: 				obj2[15] = ((object)val3.anchoredPosition/*cast due to .constrained prefix*/).ToString();
00911: 				obj2[16] = " size=";
00912: 				obj2[17] = ((object)val3.sizeDelta/*cast due to .constrained prefix*/).ToString();
00913: 				Core.Debug(string.Concat(obj2));
00914: 			}
00915: 			catch
00916: 			{
00917: 			}
00918: 			return val4;
00919: 		}
00920: 		catch (Exception ex)
00921: 		{
00922: 			Core.Log?.Warning("收纳箱[" + context + "] 按钮文字叠层：" + ex.Message);
00923: 			return null;
00924: 		}
00925: 	}
00926: 
00927: 	private static GameObject? CreateSlotClickLayer(MachineryLabel label, GameItem pack, int slot, string context)
00928: 	{
00929: 		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
00930: 		//IL_008d: Expected O, but got Unknown
00931: 		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
00932: 		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
00933: 		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
00934: 		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
00935: 		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
00936: 		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
00937: 		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
00938: 		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
00939: 		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
00940: 		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
00941: 		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
00942: 		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
00943: 		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
00944: 		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
00945: 		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
00946: 		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
00947: 		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
00948: 		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
00949: 		try
00950: 		{
00951: 			TreeNodeRender handler = label.handler;
00952: 			GameObject val = ((handler != null) ? ((Component)handler).gameObject : null);
00953: 			if ((Object)(object)val == (Object)null)
00954: 			{
00955: 				Core.Debug("收纳箱[" + context + "] 槽 " + slot + " 点击层：标签 handler 为空");
00956: 				return null;
00957: 			}
00958: 			GameObject val2 = new GameObject("item_storage_slot_click_" + slot);
00959: 			val2.transform.SetParent(val.transform, false);
00960: 			Transform transform = val2.transform;
00961: 			RectTransform val3 = (RectTransform)(((object)((transform is RectTransform) ? transform : null)) ?? ((object)val2.AddComponent<RectTransform>()));
00962: 			((Transform)val3).localScale = Vector3.one;
00963: 			string text = "text";
00964: 			TMP_Text componentInChildren = val.GetComponentInChildren<TMP_Text>(true);
00965: 			RectTransform val4 = ((componentInChildren != null) ? componentInChildren.rectTransform : null);
00966: 			if ((Object)(object)val4 == (Object)null && val.transform.childCount > 0)
00967: 			{
00968: 				Transform child = val.transform.GetChild(0);
00969: 				val4 = (RectTransform)(object)((child is RectTransform) ? child : null);
00970: 				text = "plate";
00971: 			}
00972: 			if ((Object)(object)val4 != (Object)null)
00973: 			{
00974: 				val3.anchorMin = val4.anchorMin;
00975: 				val3.anchorMax = val4.anchorMax;
00976: 				val3.pivot = val4.pivot;
00977: 				val3.anchoredPosition = val4.anchoredPosition;
00978: 				val3.sizeDelta = val4.sizeDelta + new Vector2(4f, 2f);
00979: 			}
00980: 			else
00981: 			{
00982: 				text = "fallback";
00983: 				val3.anchorMin = new Vector2(0f, 1f);
00984: 				val3.anchorMax = new Vector2(0f, 1f);
00985: 				val3.pivot = new Vector2(0f, 1f);
00986: 				val3.anchoredPosition = Vector2.zero;
00987: 				val3.sizeDelta = new Vector2(48f, 8f);
00988: 			}
00989: 			Image obj = val2.AddComponent<Image>();
00990: 			((Graphic)obj).color = new Color(1f, 1f, 1f, 0f);
00991: 			((Graphic)obj).raycastTarget = true;
00992: 			SlotClickCatcher slotClickCatcher = val2.AddComponent<SlotClickCatcher>();
00993: 			slotClickCatcher.Pack = pack;
00994: 			slotClickCatcher.Slot = slot;
00995: 			slotClickCatcher.Refresh = delegate
00996: 			{
00997: 				RefreshButtons(pack);
00998: 				Core.Log?.Msg("收纳箱 锁定 UI 已刷新：槽 " + slot + " locked=" + SlotLock.IsLocked(pack, slot));
00999: 			};
01000: 			val2.SetActive(true);
01001: 			Core.LogSink log = Core.Log;
01002: 			if (log != null)
01003: 			{
01004: 				string[] obj2 = new string[11]
01005: 				{
01006: 					"收纳箱[",
01007: 					context,
01008: 					"] 槽 ",
01009: 					slot.ToString(),
01010: 					" 点击层 v3 已构建（rect=",
01011: 					null,
01012: 					null,
01013: 					null,
01014: 					null,
01015: 					null,
01016: 					null
01017: 				};
01018: 				Rect rect = val3.rect;
01019: 				obj2[5] = ((Rect)(ref rect)).width.ToString("0.#");
01020: 				obj2[6] = "x";
01021: 				rect = val3.rect;
01022: 				obj2[7] = ((Rect)(ref rect)).height.ToString("0.#");
01023: 				obj2[8] = "，来源=";
01024: 				obj2[9] = text;
01025: 				obj2[10] = "）。";
01026: 				log.Msg(string.Concat(obj2));
01027: 			}
01028: 			return val2;
01029: 		}
01030: 		catch (Exception ex)
01031: 		{
01032: 			Core.Log?.Warning("收纳箱[" + context + "] 槽 " + slot + " 点击层构建：" + ex.Message);
01033: 			return null;
01034: 		}
01035: 	}
01036: 
01037: 	private static Image? CreateSlotIcon(AdvButtonElement button, string context, int slot)
01038: 	{
01039: 		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
01040: 		//IL_0077: Expected O, but got Unknown
01041: 		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
01042: 		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
01043: 		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
01044: 		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
01045: 		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
01046: 		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
01047: 		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
01048: 		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
01049: 		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
01050: 		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
01051: 		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
01052: 		//IL_0186: Expected O, but got Unknown
01053: 		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
01054: 		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
01055: 		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
01056: 		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
01057: 		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
01058: 		try
01059: 		{
01060: 			object obj;
01061: 			if (button == null)
01062: 			{
01063: 				obj = null;
01064: 			}
01065: 			else
01066: 			{
01067: 				TreeNodeRender handler = ((SelectableElement)button).handler;
01068: 				obj = ((handler != null) ? handler.rectTransform : null);
01069: 			}
01070: 			RectTransform val = (RectTransform)obj;
01071: 			if ((Object)(object)val == (Object)null)
01072: 			{
01073: 				Core.Debug("收纳箱[" + context + "] 槽 " + slot + " 图标层：rectTransform=null");
01074: 				return null;
01075: 			}
01076: 			GameObject val2 = new GameObject("item_storage_slot_icon_" + slot);
01077: 			val2.transform.SetParent((Transform)(object)val, false);
01078: 			Transform transform = val2.transform;
01079: 			object obj2 = ((object)((transform is RectTransform) ? transform : null)) ?? ((object)val2.AddComponent<RectTransform>());
01080: 			Rect rect = val.rect;
01081: 			float num = ((Rect)(ref rect)).width;
01082: 			if (num < 1f)
01083: 			{
01084: 				num = 48f;
01085: 			}
01086: 			rect = val.rect;
01087: 			float num2 = ((Rect)(ref rect)).height;
01088: 			if (num2 < 1f)
01089: 			{
01090: 				num2 = 48f;
01091: 			}
01092: 			((RectTransform)obj2).anchorMin = new Vector2(0f, 1f);
01093: 			((RectTransform)obj2).anchorMax = new Vector2(0f, 1f);
01094: 			((RectTransform)obj2).pivot = new Vector2(0.5f, 0.5f);
01095: 			((RectTransform)obj2).anchoredPosition = new Vector2(num / 2f, (0f - num2) / 2f);
01096: 			((RectTransform)obj2).sizeDelta = new Vector2(40f, 40f);
01097: 			((Transform)obj2).localScale = Vector3.one;
01098: 			((Transform)obj2).SetAsLastSibling();
01099: 			Image obj3 = val2.AddComponent<Image>();
01100: 			obj3.type = (Type)0;
01101: 			obj3.preserveAspect = true;
01102: 			((Graphic)obj3).raycastTarget = false;
01103: 			GameObject val3 = new GameObject("item_storage_slot_liquid_" + slot);
01104: 			val3.transform.SetParent(val2.transform, false);
01105: 			Transform transform2 = val3.transform;
01106: 			object obj4 = ((object)((transform2 is RectTransform) ? transform2 : null)) ?? ((object)val3.AddComponent<RectTransform>());
01107: 			((RectTransform)obj4).anchorMin = Vector2.zero;
01108: 			((RectTransform)obj4).anchorMax = Vector2.one;
01109: 			((RectTransform)obj4).offsetMin = Vector2.zero;
01110: 			((RectTransform)obj4).offsetMax = Vector2.zero;
01111: 			Image obj5 = val3.AddComponent<Image>();
01112: 			obj5.type = (Type)3;
01113: 			obj5.fillMethod = (FillMethod)1;
01114: 			obj5.fillOrigin = 0;
01115: 			obj5.fillAmount = 0f;
01116: 			obj5.preserveAspect = true;
01117: 			((Graphic)obj5).color = new Color(1f, 1f, 1f, 0.7f);
01118: 			((Graphic)obj5).raycastTarget = false;
01119: 			val3.SetActive(false);
01120: 			val2.SetActive(false);
01121: 			return obj3;
01122: 		}
01123: 		catch (Exception ex)
01124: 		{
01125: 			Core.Log?.Warning("收纳箱[" + context + "] 槽 " + slot + " 图标层构建：" + ex.Message);
01126: 			return null;
01127: 		}
01128: 	}
01129: 
01130: 	private static void UpdateSlotIcon(GameItem pack, int slot, Image icon)
01131: 	{
01132: 		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
01133: 		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
01134: 		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
01135: 		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
01136: 		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
01137: 		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
01138: 		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
01139: 		try
01140: 		{
01141: 			string itemId = StorageGrid.GetItemId(pack, slot);
01142: 			if (itemId == null)
01143: 			{
01144: 				if (((Component)icon).gameObject.activeSelf)
01145: 				{
01146: 					((Component)icon).gameObject.SetActive(false);
01147: 				}
01148: 				return;
01149: 			}
01150: 			string spriteAtlas = StorageGrid.GetSpriteAtlas(pack, slot);
01151: 			string spritePath = StorageGrid.GetSpritePath(pack, slot);
01152: 			Sprite val = ((spriteAtlas != null || spritePath != null) ? LoadSpriteCached(spriteAtlas, spritePath) : null);
01153: 			if ((Object)(object)val == (Object)null && NullGuard.HasText(itemId) && CanWarmNow(itemId))
01154: 			{
01155: 				RetryState retry = GetRetry(WarmTried, itemId);
01156: 				bool flag = true;
01157: 				try
01158: 				{
01159: 					GameItem val2 = null;
01160: 					try
01161: 					{
01162: 						val2 = DirectoryMaster.Item(itemId, true);
01163: 					}
01164: 					catch
01165: 					{
01166: 						flag = false;
01167: 					}
01168: 					try
01169: 					{
01170: 						if (val2 != null)
01171: 						{
01172: 							val2.Destroy();
01173: 						}
01174: 					}
01175: 					catch
01176: 					{
01177: 						flag = false;
01178: 					}
01179: 				}
01180: 				catch
01181: 				{
01182: 					flag = false;
01183: 				}
01184: 				retry.Time = Time.unscaledTime;
01185: 				val = ((spriteAtlas != null || spritePath != null) ? LoadSpriteCached(spriteAtlas, spritePath) : null);
01186: 				if ((Object)(object)val != (Object)null)
01187: 				{
01188: 					WarmTried.Remove(itemId);
01189: 				}
01190: 				else if (flag && ++retry.Fails >= 2)
01191: 				{
01192: 					Core.Debug("收纳箱预热：'" + itemId + "' 两轮预热后仍无贴图，本会话停止重试（对应 MOD 可能已卸载或贴图键失配）。");
01193: 				}
01194: 			}
01195: 			if ((Object)(object)val == (Object)null)
01196: 			{
01197: 				Sprite val3 = PlaceholderSprite();
01198: 				if ((Object)(object)val3 != (Object)null)
01199: 				{
01200: 					if ((Object)(object)icon.sprite != (Object)(object)val3)
01201: 					{
01202: 						icon.sprite = val3;
01203: 					}
01204: 					((Graphic)icon).color = Color.white;
01205: 					if (!((Component)icon).gameObject.activeSelf)
01206: 					{
01207: 						((Component)icon).gameObject.SetActive(true);
01208: 					}
01209: 				}
01210: 				return;
01211: 			}
01212: 			if ((Object)(object)icon.sprite != (Object)(object)val)
01213: 			{
01214: 				icon.sprite = val;
01215: 			}
01216: 			((Graphic)icon).color = Color.white;
01217: 			UpdateSlotModLayers(icon, slot, itemId);
01218: 			Transform val4 = ((Component)icon).transform.Find("item_storage_slot_liquid_" + slot);
01219: 			if ((Object)(object)val4 != (Object)null)
01220: 			{
01221: 				Image component = ((Component)val4).GetComponent<Image>();
01222: 				if ((Object)(object)component != (Object)null)
01223: 				{
01224: 					if ((Object)(object)component.sprite != (Object)(object)val)
01225: 					{
01226: 						component.sprite = val;
01227: 					}
01228: 					if (BoxTransfer.TryGetPoolFill(pack, slot, out var fill) && fill > 0.001f)
01229: 					{
01230: 						component.fillAmount = fill;
01231: 						((Graphic)component).color = (BoxTransfer.TryGetPoolColor(pack, slot, out var col) ? new Color(col.r, col.g, col.b, 0.7f) : new Color(1f, 1f, 1f, 0.7f));
01232: 						if (!((Component)val4).gameObject.activeSelf)
01233: 						{
01234: 							((Component)val4).gameObject.SetActive(true);
01235: 						}
01236: 					}
01237: 					else if (((Component)val4).gameObject.activeSelf)
01238: 					{
01239: 						((Component)val4).gameObject.SetActive(false);
01240: 					}
01241: 				}
01242: 			}
01243: 			SlotLock.UpdateBadge(icon, pack, slot);
01244: 			if (!((Component)icon).gameObject.activeSelf)
01245: 			{
01246: 				((Component)icon).gameObject.SetActive(true);
01247: 			}
01248: 		}
01249: 		catch (Exception ex)
01250: 		{
01251: 			Core.Log?.Warning("收纳箱 UpdateSlotIcon(" + slot + ")：" + ex.Message);
01252: 		}
01253: 	}
01254: 
01255: 	internal static bool IsSpriteResolvable(string? atlas, string? spr)
01256: 	{
01257: 		try
01258: 		{
01259: 			return (Object)(object)LoadSpriteCached(atlas, spr) != (Object)null;
01260: 		}
01261: 		catch
01262: 		{
01263: 			return false;
01264: 		}
01265: 	}
01266: 
01267: 	private static Func<GameItem, List<ItemSpriteModifier>>? GetTemplateModDelegate(string itemId)
01268: 	{
01269: 		if (SlotModDelegates.TryGetValue(itemId, out Func<GameItem, List<ItemSpriteModifier>> value))
01270: 		{
01271: 			return value;
01272: 		}
01273: 		Func<GameItem, List<ItemSpriteModifier>> func = null;
01274: 		try
01275: 		{
01276: 			GameItem val = null;
01277: 			try
01278: 			{
01279: 				val = DirectoryMaster.Item(itemId, true);
01280: 			}
01281: 			catch
01282: 			{
01283: 			}
01284: 			if (val != null)
01285: 			{
01286: 				try
01287: 				{
01288: 					func = ((GameItemFunc)val).getItemSpriteModifierForItemFunc;
01289: 				}
01290: 				catch
01291: 				{
01292: 				}
01293: 				try
01294: 				{
01295: 					val.Destroy();
01296: 				}
01297: 				catch
01298: 				{
01299: 				}
01300: 			}
01301: 		}
01302: 		catch
01303: 		{
01304: 		}
01305: 		SlotModDelegates[itemId] = func;
01306: 		return func;
01307: 	}
01308: 
01309: 	private static void UpdateSlotModLayers(Image icon, int slot, string itemId)
01310: 	{
01311: 		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
01312: 		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
01313: 		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
01314: 		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
01315: 		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
01316: 		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
01317: 		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
01318: 		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
01319: 		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
01320: 		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
01321: 		//IL_013f: Expected O, but got Unknown
01322: 		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
01323: 		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
01324: 		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
01325: 		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
01326: 		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
01327: 		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
01328: 		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
01329: 		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
01330: 		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
01331: 		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
01332: 		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
01333: 		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
01334: 		try
01335: 		{
01336: 			Transform transform = ((Component)icon).transform;
01337: 			for (int num = transform.childCount - 1; num >= 0; num--)
01338: 			{
01339: 				Transform child = transform.GetChild(num);
01340: 				if (((Object)child).name.StartsWith("item_storage_slot_mod_", StringComparison.Ordinal))
~~~

~~~csharp
01364: 			for (int i = 1; i < list.Count; i++)
01365: 			{
01366: 				ItemSpriteModifier val = list[i];
01367: 				int num2 = i - 1;
01368: 				while (num2 >= 0 && list[num2].priority > val.priority)
01369: 				{
01370: 					list[num2 + 1] = list[num2];
01371: 					num2--;
01372: 				}
01373: 				list[num2 + 1] = val;
01374: 			}
01375: 			int num3 = 0;
01376: 			foreach (ItemSpriteModifier item in list)
01377: 			{
01378: 				if ((Object)(object)item.sprite == (Object)null)
01379: 				{
01380: 					num3++;
01381: 					continue;
01382: 				}
01383: 				GameObject val2 = new GameObject("item_storage_slot_mod_" + slot + "_" + num3);
01384: 				val2.transform.SetParent(transform, false);
01385: 				Transform transform2 = val2.transform;
01386: 				RectTransform val3 = (RectTransform)(((object)((transform2 is RectTransform) ? transform2 : null)) ?? ((object)val2.AddComponent<RectTransform>()));
01387: 				val3.anchorMin = Vector2.zero;
01388: 				val3.anchorMax = Vector2.one;
01389: 				val3.offsetMin = Vector2.zero;
01390: 				val3.offsetMax = Vector2.zero;
01391: 				((Transform)val3).localScale = (Vector3)(item.isFlipped ? new Vector3(-1f, 1f, 1f) : Vector3.one);
01392: 				int num4 = (item.orientation % 4 + 4) % 4;
01393: 				if (num4 != 0)
01394: 				{
01395: 					((Transform)val3).localEulerAngles = new Vector3(0f, 0f, 90f * (float)num4);
01396: 				}
01397: 				Image obj2 = val2.AddComponent<Image>();
01398: 				obj2.sprite = item.sprite;
01399: 				obj2.type = (Type)0;
01400: 				obj2.preserveAspect = true;
01401: 				((Graphic)obj2).raycastTarget = false;
01402: 				((Graphic)obj2).color = item.color;
01403: 				val2.SetActive(true);
01404: 				num3++;
01405: 			}
01406: 		}
01407: 		catch (Exception ex)
01408: 		{
01409: 			Core.Debug("收纳箱 叠层渲染槽 " + slot + " '" + itemId + "'：" + ex.Message);
01410: 		}
01411: 	}
01412: 
01413: 	private static Sprite? PlaceholderSprite()
01414: 	{
01415: 		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
01416: 		//IL_002e: Expected O, but got Unknown
01417: 		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
01418: 		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
01419: 		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
01420: 		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
01421: 		try
01422: 		{
01423: 			if ((Object)(object)_placeholder != (Object)null && IsAliveSprite(_placeholder))
01424: 			{
01425: 				return _placeholder;
01426: 			}
01427: 			Texture2D val = new Texture2D(4, 4, (TextureFormat)4, false);
01428: 			Color32[] array = (Color32[])(object)new Color32[16];
01429: 			for (int i = 0; i < array.Length; i++)
01430: 			{
01431: 				array[i] = new Color32((byte)72, (byte)72, (byte)78, byte.MaxValue);
01432: 			}
01433: 			val.SetPixels32(array);
01434: 			val.Apply(false, true);
01435: 			_placeholder = Sprite.Create(val, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 100f);
01436: 			return _placeholder;
01437: 		}
01438: 		catch
01439: 		{
01440: 			return null;
01441: 		}
01442: 	}
01443: 
01444: 	private static Sprite? LoadSpriteCached(string? atlas, string? spr)
01445: 	{
01446: 		string key = atlas + "|" + spr;
01447: 		if (SpriteCache.TryGetValue(key, out Sprite value))
01448: 		{
01449: 			if (IsAliveSprite(value))
01450: 			{
01451: 				return value;
01452: 			}
01453: 			SpriteCache.Remove(key);
01454: 		}
01455: 		Sprite val = null;
01456: 		if (NullGuard.HasText(spr))
01457: 		{
01458: 			try
01459: 			{
01460: 				if (StoragePatch.ResolvedSpriteByName.TryGetValue(spr, out Sprite value2) && IsAliveSprite(value2))
01461: 				{
01462: 					val = value2;
01463: 				}
01464: 			}
01465: 			catch
01466: 			{
01467: 			}
01468: 		}
01469: 		if ((Object)(object)val == (Object)null && NullGuard.HasText(atlas) && NullGuard.HasText(spr))
01470: 		{
01471: 			try
01472: 			{
01473: 				val = RenderHandler.TryLoadFromAtlas(atlas, spr);
01474: 			}
01475: 			catch (Exception ex)
01476: 			{
01477: 				Core.Debug("收纳箱 TryLoadFromAtlas '" + spr + "'：" + ex.Message);
01478: 			}
01479: 		}
01480: 		if ((Object)(object)val == (Object)null && NullGuard.HasText(atlas) && NullGuard.HasText(spr))
01481: 		{
01482: 			try
01483: 			{
01484: 				LoadedMod val2 = null;
01485: 				List<LoadedMod> mods = ModLoader.Mods;
01486: 				if (mods != null)
01487: 				{
01488: 					for (int i = 0; i < mods.Count; i++)
01489: 					{
01490: 						LoadedMod val3 = mods[i];
01491: 						if (val3?.Manifest != null && string.Equals(val3.Manifest.ID, atlas, StringComparison.Ordinal))
01492: 						{
01493: 							val2 = val3;
01494: 							break;
01495: 						}
01496: 					}
01497: 				}
01498: 				if (val2 != null && val2.Sprites.TryGetValue(spr, out var value3) && IsAliveSprite(value3))
01499: 				{
01500: 					val = value3;
01501: 				}
01502: 			}
01503: 			catch
01504: 			{
01505: 			}
01506: 		}
01507: 		if ((Object)(object)val == (Object)null && NullGuard.HasText(atlas))
01508: 		{
01509: 			int num = atlas.LastIndexOf('/');
01510: 			if (num >= 0 && num < atlas.Length - 1)
01511: 			{
01512: 				string text = atlas.Substring(num + 1);
01513: 				string text2 = atlas.Substring(0, num);
01514: 				try
01515: 				{
01516: 					if (StoragePatch.ResolvedSpriteByName.TryGetValue(text, out Sprite value4) && IsAliveSprite(value4))
01517: 					{
01518: 						val = value4;
01519: 					}
01520: 				}
01521: 				catch
01522: 				{
01523: 				}
01524: 				if ((Object)(object)val == (Object)null)
01525: 				{
01526: 					try
01527: 					{
01528: 						val = RenderHandler.TryLoadFromAtlas(text2, text);
01529: 					}
01530: 					catch
01531: 					{
01532: 					}
01533: 				}
01534: 				if ((Object)(object)val == (Object)null)
01535: 				{
01536: 					try
01537: 					{
01538: 						val = RenderHandler.TryLoadFromAtlas(atlas, text);
01539: 					}
01540: 					catch
01541: 					{
01542: 					}
01543: 				}
01544: 			}
01545: 		}
01546: 		if ((Object)(object)val == (Object)null && NullGuard.HasText(spr) && CanProbeNow(spr))
01547: 		{
01548: 			Sprite val4 = ProbeResolve(spr);
01549: 			if ((Object)(object)val4 != (Object)null)
01550: 			{
01551: 				val = val4;
01552: 				ProbeTried.Remove(spr);
01553: 			}
01554: 			else
01555: 			{
01556: 				RetryState retry = GetRetry(ProbeTried, spr);
01557: 				retry.Time = Time.unscaledTime;
01558: 				if (++retry.Fails >= 2)
01559: 				{
01560: 					Core.Debug("收纳箱探测：'" + spr + "' 两轮探测后仍无贴图，本会话停止重试。");
01561: 				}
01562: 			}
01563: 		}
01564: 		if ((Object)(object)val != (Object)null && StoragePatch.IsUnknownSprite(val))
01565: 		{
01566: 			return null;
01567: 		}
01568: 		if (IsAliveSprite(val))
01569: 		{
01570: 			SpriteCache[key] = val;
01571: 		}
01572: 		return val;
01573: 	}
01574: 
01575: 	private static bool IsAliveSprite(Sprite? sprite)
01576: 	{
01577: 		if ((Object)(object)sprite == (Object)null)
01578: 		{
01579: 			return false;
01580: 		}
01581: 		try
01582: 		{
01583: 			return (Object)(object)sprite != (Object)null;
01584: 		}
01585: 		catch
01586: 		{
01587: 			return false;
01588: 		}
01589: 	}
01590: 
01591: 	private static RetryState GetRetry(Dictionary<string, RetryState> table, string key)
01592: 	{
01593: 		if (!table.TryGetValue(key, out RetryState value))
01594: 		{
01595: 			value = (table[key] = new RetryState());
01596: 		}
01597: 		return value;
01598: 	}
01599: 
01600: 	private static bool CanWarmNow(string itemId)
01601: 	{
01602: 		return CanRetry(WarmTried, itemId);
01603: 	}
01604: 
01605: 	private static bool CanProbeNow(string spr)
01606: 	{
01607: 		return CanRetry(ProbeTried, spr);
01608: 	}
01609: 
01610: 	private static bool CanRetry(Dictionary<string, RetryState> table, string key)
01611: 	{
01612: 		if (table.TryGetValue(key, out RetryState value))
01613: 		{
01614: 			if (value.Fails < 2)
01615: 			{
01616: 				return Time.unscaledTime - value.Time > 60f;
01617: 			}
01618: 			return false;
01619: 		}
01620: 		return true;
01621: 	}
01622: 
01623: 	private static Sprite? ProbeResolve(string spr)
01624: 	{
01625: 		GameItemElement val = FindAnyLiveElement();
01626: 		if (val != null)
01627: 		{
01628: 			try
01629: 			{
01630: 				Sprite val2 = val.ResolveSpriteByName(spr);
01631: 				if ((Object)(object)val2 != (Object)null && !StoragePatch.IsUnknownSprite(val2))
01632: 				{
01633: 					return val2;
01634: 				}
01635: 			}
01636: 			catch
01637: 			{
01638: 			}
01639: 		}
01640: 		GameItemElement val3 = null;
01641: 		try
01642: 		{
01643: 			val3 = (GameItemElement)(object)CharacterItemDirectory.CreateEmptyCharacterItem("__item_storage_probe");
01644: 			Sprite val4 = val3.ResolveSpriteByName(spr);
01645: 			if ((Object)(object)val4 != (Object)null && !StoragePatch.IsUnknownSprite(val4))
01646: 			{
01647: 				return val4;
01648: 			}
01649: 		}
~~~

~~~csharp
01752: 	{
01753: 		if (IconRefreshItems.Count == 0)
01754: 		{
01755: 			return;
01756: 		}
01757: 		try
01758: 		{
01759: 			float time = Time.time;
01760: 			for (int num = IconRefreshItems.Count - 1; num >= 0; num--)
01761: 			{
01762: 				if (!(time < IconRefreshAt[num]))
01763: 				{
01764: 					GameItem val = IconRefreshItems[num];
01765: 					bool flag = false;
01766: 					try
01767: 					{
01768: 						flag = val != null && val != null;
01769: 					}
01770: 					catch
01771: 					{
01772: 						flag = false;
01773: 					}
01774: 					if (!flag)
01775: 					{
01776: 						IconRefreshItems.RemoveAt(num);
01777: 						IconRefreshAt.RemoveAt(num);
01778: 						IconRetryTried.RemoveAt(num);
01779: 					}
01780: 					else
01781: 					{
01782: 						try
01783: 						{
01784: 							RefreshButtons(val);
01785: 						}
01786: 						catch
01787: 						{
01788: 						}
01789: 						if (!HasUnresolvedIcon(val))
01790: 						{
01791: 							IconRefreshItems.RemoveAt(num);
01792: 							IconRefreshAt.RemoveAt(num);
01793: 							IconRetryTried.RemoveAt(num);
01794: 						}
01795: 						else if (++IconRetryTried[num] >= 40)
01796: 						{
01797: 							Core.Debug("收纳箱 图标重试放弃：'" + (Core.Clean(val.identifier) ?? "?") + "' 仍有未解析贴图（MOD 卸载或贴图键失效）");
01798: 							IconRefreshItems.RemoveAt(num);
01799: 							IconRefreshAt.RemoveAt(num);
01800: 							IconRetryTried.RemoveAt(num);
01801: 						}
01802: 						else
01803: 						{
01804: 							IconRefreshAt[num] = time + 1.5f;
01805: 						}
01806: 					}
01807: 				}
01808: 			}
01809: 		}
01810: 		catch
01811: 		{
01812: 		}
01813: 	}
01814: 
01815: 	private static bool IsOwnTopmost(Vector2 screenPos, GameObject? btnGo, GameObject? iconGo, GameObject? clickGo = null)
01816: 	{
01817: 		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
01818: 		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
01819: 		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
01820: 		//IL_0048: Expected O, but got Unknown
01821: 		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
01822: 		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
01823: 		try
01824: 		{
01825: 			if ((Object)(object)btnGo == (Object)null && (Object)(object)iconGo == (Object)null && (Object)(object)clickGo == (Object)null)
01826: 			{
01827: 				return true;
01828: 			}
01829: 			EventSystem current = EventSystem.current;
01830: 			if ((Object)(object)current == (Object)null)
01831: 			{
01832: 				return true;
01833: 			}
01834: 			PointerEventData val = new PointerEventData(current)
01835: 			{
01836: 				position = screenPos
01837: 			};
01838: 			List<RaycastResult> list = new List<RaycastResult>();
01839: 			current.RaycastAll(val, list);
01840: 			if (list.Count == 0)
01841: 			{
01842: 				return true;
01843: 			}
01844: 			RaycastResult val2 = list[0];
01845: 			GameObject gameObject = ((RaycastResult)(ref val2)).gameObject;
01846: 			if ((Object)(object)gameObject == (Object)null)
01847: 			{
01848: 				return true;
01849: 			}
01850: 			if ((Object)(object)btnGo != (Object)null && ((Object)(object)gameObject == (Object)(object)btnGo || gameObject.transform.IsChildOf(btnGo.transform)))
01851: 			{
01852: 				return true;
01853: 			}
01854: 			if ((Object)(object)iconGo != (Object)null && ((Object)(object)gameObject == (Object)(object)iconGo || gameObject.transform.IsChildOf(iconGo.transform)))
01855: 			{
01856: 				return true;
01857: 			}
01858: 			if ((Object)(object)clickGo != (Object)null && ((Object)(object)gameObject == (Object)(object)clickGo || gameObject.transform.IsChildOf(clickGo.transform)))
01859: 			{
01860: 				return true;
01861: 			}
01862: 			return false;
01863: 		}
01864: 		catch
01865: 		{
01866: 			return true;
01867: 		}
01868: 	}
01869: 
01870: 	internal static bool TryHitSlot(Vector2 screenPos, out GameItem? box, out int slot, bool verbose = false)
01871: 	{
01872: 		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
01873: 		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
01874: 		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
01875: 		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
01876: 		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
01877: 		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
01878: 		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
01879: 		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
01880: 		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
01881: 		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
01882: 		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
01883: 		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
01884: 		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
01885: 		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
01886: 		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
~~~

~~~csharp
02005: 										Canvas val7 = (((Object)(object)val6 != (Object)null) ? ((Component)val6).GetComponentInParent<Canvas>() : null);
02006: 										if ((Object)(object)val7 != (Object)null && (int)val7.renderMode != 0)
02007: 										{
02008: 											val5 = val7.worldCamera;
02009: 										}
02010: 									}
02011: 									catch
02012: 									{
02013: 										val5 = null;
02014: 									}
02015: 									Vector2 zero = Vector2.zero;
02016: 									bool value5 = (Object)(object)val6 != (Object)null && RectTransformUtility.ScreenPointToLocalPointInRectangle(val6, screenPos, val5, ref zero);
02017: 									StringBuilder stringBuilder2 = stringBuilder.Append(" 槽0换算=").Append(value5).Append(" local=(")
02018: 										.Append(zero.x.ToString("0.0"))
02019: 										.Append(',')
02020: 										.Append(zero.y.ToString("0.0"))
02021: 										.Append(')')
02022: 										.Append(" rect=");
02023: 									Rect val8;
02024: 									if (!((Object)(object)val6 != (Object)null))
02025: 									{
02026: 										val3 = default(Rect);
02027: 										val8 = val3;
02028: 									}
02029: 									else
02030: 									{
02031: 										val8 = val6.rect;
02032: 									}
02033: 									stringBuilder2.Append(val8);
02034: 								}
02035: 								catch
02036: 								{
02037: 									stringBuilder.Append(" 槽0换算异常");
02038: 								}
02039: 							}
02040: 						}
02041: 					}
02042: 				}
02043: 				catch
02044: 				{
02045: 				}
02046: 			}
02047: 		}
02048: 		if (stringBuilder != null)
02049: 		{
02050: 			Core.Debug("收纳箱 drop 未命中诊断：鼠标=(" + screenPos.x.ToString("0.0") + "," + screenPos.y.ToString("0.0") + ") 候选盒=" + num + stringBuilder.ToString());
02051: 		}
02052: 		return false;
02053: 	}
02054: 
02055: 	private static bool HitTest(RectTransform rt, Vector2 screenPos)
02056: 	{
02057: 		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
02058: 		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
02059: 		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
02060: 		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
02061: 		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
02062: 		try
02063: 		{
02064: 			Camera val = null;
02065: 			try
02066: 			{
02067: 				Canvas componentInParent = ((Component)rt).GetComponentInParent<Canvas>();
02068: 				if ((Object)(object)componentInParent != (Object)null && (int)componentInParent.renderMode != 0)
02069: 				{
02070: 					val = (((Object)(object)componentInParent.worldCamera != (Object)null) ? componentInParent.worldCamera : Camera.main);
02071: 				}
02072: 			}
02073: 			catch
02074: 			{
02075: 				val = Camera.main;
02076: 			}
02077: 			Vector2 val2 = default(Vector2);
02078: 			int result;
02079: 			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPos, val, ref val2))
02080: 			{
02081: 				Rect rect = rt.rect;
02082: 				result = (((Rect)(ref rect)).Contains(val2) ? 1 : 0);
02083: 			}
02084: 			else
02085: 			{
02086: 				result = 0;
02087: 			}
02088: 			return (byte)result != 0;
02089: 		}
02090: 		catch
02091: 		{
02092: 			return false;
02093: 		}
02094: 	}
02095: 
02096: 	private static void TrackBox(GameItem pack)
02097: 	{
02098: 		try
02099: 		{
02100: 			for (int num = TrackedBoxes.Count - 1; num >= 0; num--)
02101: 			{
02102: 				try
02103: 				{
02104: 					if (TrackedBoxes[num] == pack)
02105: 					{
02106: 						TrackedBoxes.RemoveAt(num);
02107: 					}
02108: 				}
02109: 				catch
02110: 				{
02111: 					TrackedBoxes.RemoveAt(num);
02112: 				}
02113: 			}
02114: 			TrackedBoxes.Add(pack);
02115: 		}
02116: 		catch
02117: 		{
02118: 		}
02119: 	}
02120: 
02121: 	private static void SyncHotkeyMap(GameItem pack)
02122: 	{
02123: 		try
02124: 		{
02125: 			int boxOption = StorageGrid.GetBoxOption(pack, "HOTKEY");
02126: 			if (boxOption != 0)
02127: 			{
02128: 				int num = 0;
02129: 				try
02130: 				{
02131: 					num = pack.uniqueId;
02132: 				}
02133: 				catch
02134: 				{
02135: 				}
02136: 				if (num != 0)
02137: 				{
02138: 					BoxHotkey.NotifyBound(num, (KeyCode)boxOption);
02139: 				}
~~~

### ItemStorage/GearPanel.cs

~~~csharp
00088: 		}
00089: 		catch
00090: 		{
00091: 			return false;
00092: 		}
00093: 	}
00094: 
00095: 	private static void SafeSetText(MachineryButton? b, string text)
00096: 	{
00097: 		try
00098: 		{
00099: 			if (b != null)
00100: 			{
00101: 				b.SetText(text);
00102: 			}
00103: 		}
00104: 		catch
00105: 		{
00106: 		}
00107: 	}
00108: 
00109: 	internal static void ToggleFor(GameItem pack)
00110: 	{
00111: 		if (pack == null || !BoxItem.IsBox(pack))
00112: 		{
00113: 			return;
00114: 		}
00115: 		try
00116: 		{
00117: 			PanelEntry panelEntry = FindIn(pack);
00118: 			if (panelEntry?.Window != null && panelEntry.Window.IsVisible())
00119: 			{
00120: 				panelEntry.Window.Hide();
00121: 				return;
00122: 			}
00123: 			if (panelEntry == null)
00124: 			{
00125: 				panelEntry = BuildPanel(pack);
00126: 			}
00127: 			panelEntry.Window.Show();
00128: 			PlaceWindow(pack, panelEntry.Window);
00129: 			panelEntry.Window.ToFront(false);
00130: 		}
00131: 		catch (Exception ex)
00132: 		{
00133: 			Core.Log?.Warning("收纳箱齿轮面板开关异常：" + ex.Message);
00134: 		}
00135: 	}
00136: 
00137: 	internal static void CloseFor(GameItem pack)
00138: 	{
00139: 		try
00140: 		{
00141: 			PanelEntry panelEntry = FindIn(pack);
00142: 			if (panelEntry?.Window != null)
00143: 			{
00144: 				if (panelEntry.Window.IsVisible())
00145: 				{
00146: 					panelEntry.Window.Hide();
00147: 				}
00148: 				try
00149: 				{
00150: 					Object.Destroy((Object)(object)((Component)panelEntry.Window.handler).gameObject);
00151: 				}
00152: 				catch
00153: 				{
00154: 				}
00155: 				Forget(pack, panelEntry);
00156: 			}
00157: 		}
00158: 		catch
00159: 		{
00160: 		}
00161: 	}
00162: 
00163: 	private static void PlaceWindow(GameItem pack, PixelWindow win)
00164: 	{
00165: 		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
00166: 		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
00167: 		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
00168: 		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
00169: 		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
00170: 		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
00171: 		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
00172: 		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
00173: 		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
00174: 		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
00175: 		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
00176: 		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
00177: 		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
00178: 		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
00179: 		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
00180: 		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
00181: 		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
00182: 		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
00183: 		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
00184: 		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
00185: 		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
00186: 		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
00187: 		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
00188: 		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
00189: 		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
00190: 		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
00191: 		try
00192: 		{
00193: 			PixelWindow contentWindow = pack.contentWindow;
00194: 			if (contentWindow != null)
00195: 			{
00196: 				RectTransform rectTransform = contentWindow.rectTransform;
00197: 				RectTransform rectTransform2 = win.rectTransform;
00198: 				Rect rect = rectTransform.rect;
00199: 				float num = ((Rect)(ref rect)).width / 2f;
00200: 				rect = rectTransform2.rect;
00201: 				float num2 = num + ((Rect)(ref rect)).width / 2f + 6f;
00202: 				rect = rectTransform.rect;
00203: 				float height = ((Rect)(ref rect)).height;
00204: 				rect = rectTransform2.rect;
00205: 				float num3 = (height - ((Rect)(ref rect)).height) / 2f;
00206: 				Vector2 val = Vector2.op_Implicit(((Transform)rectTransform2).parent.InverseTransformPoint(((Transform)rectTransform).position));
00207: 				Vector2 anchoredPosition = val + new Vector2(num2, num3);
00208: 				float x = ((Transform)rectTransform).position.x;
00209: 				rect = rectTransform.rect;
00210: 				float num4 = x + ((Rect)(ref rect)).width / 2f + 6f;
00211: 				rect = rectTransform2.rect;
00212: 				if (num4 + ((Rect)(ref rect)).width > (float)Screen.width - 8f)
00213: 				{
00214: 					anchoredPosition = val + new Vector2(0f - num2, num3);
00215: 				}
00216: 				rectTransform2.anchoredPosition = anchoredPosition;
00217: 			}
00218: 			win.FixBounds();
00219: 		}
00220: 		catch
00221: 		{
00222: 		}
~~~

~~~csharp
00304: 		MachineryRadioButton val9 = MakeBoolRadio(pack, "RESTOCKGUARD", RestockGuardText, delegate(bool v)
00305: 		{
00306: 			PanelEntry panelEntry2 = FindIn(pack);
00307: 			try
00308: 			{
00309: 				if (panelEntry2 != null)
00310: 				{
00311: 					panelEntry2.RestockGuardRadio.SetOn(v, false);
00312: 				}
00313: 			}
00314: 			catch
00315: 			{
00316: 			}
00317: 		});
00318: 		Attach(val2, val9, 1, 3);
00319: 		MachineryRadioButton nameRef = null;
00320: 		MachineryRadioButton val10 = (nameRef = new MachineryRadioButton("隐藏名字", num2 != 0 && !BoxLabel.IsShownFor(num2), MakeBoolAction(delegate(bool on)
00321: 		{
00322: 			try
00323: 			{
00324: 				if (!BoxItem.IsUsableByPlayer(pack))
00325: 				{
00326: 					BoxItem.NotifyUnowned();
00327: 					SyncNameRadio(pack, nameRef);
00328: 				}
00329: 				else
00330: 				{
00331: 					int num3 = 0;
00332: 					try
00333: 					{
00334: 						num3 = pack.uniqueId;
00335: 					}
00336: 					catch
00337: 					{
00338: 					}
00339: 					bool flag = num3 != 0 && BoxLabel.IsShownFor(num3);
00340: 					if (num3 != 0 && flag == on)
00341: 					{
00342: 						BoxLabel.ToggleFor(num3);
00343: 					}
00344: 					SyncNameRadio(pack, nameRef);
00345: 				}
00346: 			}
00347: 			catch (Exception ex2)
00348: 			{
00349: 				Core.Log?.Warning("收纳箱齿轮面板名字开关异常：" + ex2.Message);
00350: 			}
00351: 		})));
00352: 		Attach(val2, val10, 1, 4);
00353: 		PanelEntry panelEntry = new PanelEntry
00354: 		{
00355: 			Window = val,
00356: 			HotkeyButton = val3,
00357: 			AllKeyButton = val4,
00358: 			RestockKeyButton = val5,
00359: 			AllRestockKeyButton = val6,
00360: 			PullRadio = val7,
00361: 			GuardRadio = val8,
00362: 			RestockGuardRadio = val9,
00363: 			NameRadio = val10
00364: 		};
00365: 		try
00366: 		{
00367: 			PixelElement val11 = (PixelElement)(object)val2;
00368: 			PixelElement val12 = val11 ?? throw new InvalidOperationException("grid 非 PixelElement");
00369: 			val.Attach(val12);
00370: 			val.Validate();
00371: 		}
00372: 		catch (Exception ex)
00373: 		{
00374: 			Core.Log?.Warning("收纳箱齿轮面板 Attach 失败：" + ex.Message);
00375: 		}
00376: 		if (num != 0L)
00377: 		{
00378: 			PanelsByUid[num] = panelEntry;
00379: 		}
00380: 		PanelsByRef[pack] = panelEntry;
00381: 		return panelEntry;
00382: 	}
00383: 
00384: 	private static MachineryRadioButton MakeBoolRadio(GameItem pack, string key, Func<GameItem, string> textOf, Action<bool> syncVisual)
00385: 	{
00386: 		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
00387: 		//IL_0055: Expected O, but got Unknown
00388: 		bool flag = StorageGrid.GetBoxOption(pack, key) == 1;
00389: 		return new MachineryRadioButton(textOf(pack), flag, MakeBoolAction(delegate(bool v)
00390: 		{
00391: 			try
00392: 			{
00393: 				if (!BoxItem.IsUsableByPlayer(pack))
00394: 				{
00395: 					BoxItem.NotifyUnowned();
00396: 					syncVisual(!v);
00397: 				}
00398: 				else
00399: 				{
00400: 					StorageGrid.SetBoxOption(pack, key, v ? 1 : 0);
00401: 					syncVisual(v);
00402: 				}
00403: 			}
00404: 			catch (Exception ex)
00405: 			{
00406: 				Core.Log?.Warning("收纳箱齿轮面板开关异常（" + key + "）：" + ex.Message);
00407: 			}
00408: 		}));
00409: 	}
00410: 
00411: 	private static void SyncNameRadio(GameItem pack, MachineryRadioButton? radio)
00412: 	{
00413: 		if (radio == null)
00414: 		{
00415: 			return;
00416: 		}
00417: 		int num = 0;
00418: 		try
00419: 		{
00420: 			num = pack.uniqueId;
00421: 		}
00422: 		catch
00423: 		{
00424: 		}
00425: 		bool flag = num == 0 || !BoxLabel.IsShownFor(num);
00426: 		try
00427: 		{
00428: 			radio.SetOn(flag, false);
00429: 		}
00430: 		catch
00431: 		{
00432: 		}
00433: 	}
00434: 
00435: 	private static void Attach(GridPixelElement grid, MachineryButton button, int col, int row)
00436: 	{
00437: 		if (button == null || !grid.Attach((PixelElement)(object)button, col, row, 116, 24))
00438: 		{
00439: 			Core.Log?.Warning("收纳箱齿轮面板按钮行 (" + col + "," + row + ") attach 失败");
00440: 		}
00441: 	}
00442: 
00443: 	private static void Attach(GridPixelElement grid, MachineryRadioButton radio, int col, int row)
00444: 	{
00445: 		if (radio == null || !grid.Attach((PixelElement)(object)radio, col, row, 116, 24))
00446: 		{
00447: 			Core.Log?.Warning("收纳箱齿轮面板开关行 (" + col + "," + row + ") attach 失败");
00448: 		}
00449: 	}
00450: 
00451: 	private static void AttachText(GridPixelElement grid, MachineryText text, int col, int row)
00452: 	{
00453: 		if (text == null || !grid.Attach((PixelElement)(object)text, col, row, 116, 14))
00454: 		{
00455: 			Core.Log?.Warning("收纳箱齿轮面板标题行 (" + col + "," + row + ") attach 失败");
00456: 		}
00457: 	}
00458: 
00459: 	private unsafe static string HotkeyText(GameItem pack)
00460: 	{
00461: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
00462: 		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
00463: 		KeyCode val = (KeyCode)StorageGrid.GetBoxOption(pack, "HOTKEY");
00464: 		if ((int)val != 0)
00465: 		{
00466: 			return "一键收纳:" + ((object)(*(KeyCode*)(&val))/*cast due to .constrained prefix*/).ToString();
00467: 		}
00468: 		return "一键收纳:点击绑定";
00469: 	}
00470: 
00471: 	private unsafe static string RestockKeyText(GameItem pack)
00472: 	{
00473: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
00474: 		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
00475: 		KeyCode val = (KeyCode)StorageGrid.GetBoxOption(pack, "HOTRESTOCK");
00476: 		if ((int)val != 0)
00477: 		{
00478: 			return "一键上货:" + ((object)(*(KeyCode*)(&val))/*cast due to .constrained prefix*/).ToString();
00479: 		}
00480: 		return "一键上货:点击绑定";
00481: 	}
00482: 
00483: 	private unsafe static string AllKeyText()
00484: 	{
00485: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00486: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00487: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
00488: 		KeyCode key = BoxStowAll.Key;
00489: 		if ((int)key != 0)
00490: 		{
00491: 			return "全箱收纳:" + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString();
00492: 		}
00493: 		return "全箱收纳:点击绑定";
00494: 	}
00495: 
00496: 	private unsafe static string AllRestockKeyText()
00497: 	{
00498: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00499: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00500: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
00501: 		KeyCode key = BoxRestock.Key;
00502: 		if ((int)key != 0)
00503: 		{
00504: 			return "全箱上货:" + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString();
00505: 		}
00506: 		return "全箱上货:点击绑定";
00507: 	}
00508: 
00509: 	private static string TitleOf(GameItem pack)
00510: 	{
00511: 		string text = BoxLabel.BaseNameOf(pack);
00512: 		if (string.IsNullOrEmpty(text))
00513: 		{
00514: 			return "设置";
00515: 		}
00516: 		if (text.Length > 3)
00517: 		{
00518: 			text = text.Substring(0, 3);
00519: 		}
00520: 		return text + "·设置";
00521: 	}
00522: 
00523: 	private static string PullText(GameItem pack)
00524: 	{
00525: 		return "吸纳同类";
00526: 	}
00527: 
00528: 	private static string GuardText(GameItem pack)
00529: 	{
00530: 		return "禁止被吸纳";
00531: 	}
00532: 
00533: 	private static string RestockGuardText(GameItem pack)
00534: 	{
00535: 		return "不参与全箱上架";
00536: 	}
00537: 
00538: 	private static Action MakeAction(Action body)
00539: 	{
00540: 		return delegate
00541: 		{
00542: 			try
00543: 			{
00544: 				body();
00545: 			}
00546: 			catch (Exception ex)
00547: 			{
00548: 				Core.Log?.Warning("收纳箱齿轮面板按钮异常：" + ex.Message);
00549: 			}
00550: 		};
00551: 	}
00552: 
00553: 	private static Action<bool> MakeBoolAction(Action<bool> body)
00554: 	{
00555: 		return delegate(bool v)
00556: 		{
00557: 			try
00558: 			{
00559: 				body(v);
00560: 			}
00561: 			catch (Exception ex)
00562: 			{
00563: 				Core.Log?.Warning("收纳箱齿轮面板开关异常：" + ex.Message);
00564: 			}
00565: 		};
00566: 	}
00567: 
00568: 	private static Action MakeCaptureAction(GameItem pack, CaptureMode mode)
00569: 	{
00570: 		return MakeAction(delegate
00571: 		{
00572: 			if (!BoxItem.IsUsableByPlayer(pack))
00573: 			{
00574: 				BoxItem.NotifyUnowned();
00575: 			}
00576: 			else
00577: 			{
00578: 				if (mode == CaptureMode.Single || mode == CaptureMode.RestockSingle)
00579: 				{
00580: 					long num = 0L;
00581: 					try
00582: 					{
00583: 						num = pack.uniqueId;
00584: 					}
00585: 					catch
00586: 					{
00587: 					}
00588: 					if (num == 0L)
00589: 					{
00590: 						return;
00591: 					}
00592: 					_captureUid = num;
00593: 				}
00594: 				else
00595: 				{
00596: 					_captureUid = 0L;
00597: 				}
00598: 				_captureMode = mode;
00599: 				_captureUntil = Time.realtimeSinceStartup + 6f;
00600: 				PanelEntry panelEntry = FindIn(pack);
00601: 				_captureButton = (MachineryButton?)(mode switch
00602: 				{
00603: 					CaptureMode.Single => panelEntry?.HotkeyButton, 
00604: 					CaptureMode.All => panelEntry?.AllKeyButton, 
00605: 					CaptureMode.RestockSingle => panelEntry?.RestockKeyButton, 
00606: 					CaptureMode.RestockAll => panelEntry?.AllRestockKeyButton, 
00607: 					_ => null, 
00608: 				});
00609: 				SafeSetText(_captureButton, "按键绑定中，Esc取消");
00610: 			}
00611: 		});
00612: 	}
00613: 
00614: 	internal static void TickCapture()
00615: 	{
00616: 		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
00617: 		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
00618: 		//IL_00d0: Invalid comparison between Unknown and I4
00619: 		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
00620: 		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
00621: 		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
00622: 		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
00623: 		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
00624: 		if ((Object)(object)_bannerTmp != (Object)null && _bannerShownUntil > 0f && Time.realtimeSinceStartup > _bannerShownUntil)
00625: 		{
00626: 			_bannerShownUntil = 0f;
00627: 			try
00628: 			{
00629: 				((Component)((TMP_Text)_bannerTmp).transform.parent).gameObject.SetActive(false);
00630: 			}
00631: 			catch
00632: 			{
00633: 			}
00634: 		}
00635: 		if (_captureMode == CaptureMode.None)
00636: 		{
00637: 			return;
00638: 		}
00639: 		if (!ButtonAlive(_captureButton))
00640: 		{
00641: 			_captureMode = CaptureMode.None;
00642: 			_captureUid = 0L;
00643: 			EndCaptureFrame();
00644: 			_captureButton = null;
00645: 			Core.Debug("收纳箱快捷键：捕获面板已随场景销毁，捕获态自动复位。");
00646: 			return;
00647: 		}
00648: 		if (Time.realtimeSinceStartup > _captureUntil)
00649: 		{
00650: 			CancelCapture("超时未按键，绑定取消");
00651: 			return;
00652: 		}
00653: 		if (Input.GetKeyDown((KeyCode)27))
00654: 		{
00655: 			if (!TryUnbindByEscape())
00656: 			{
00657: 				CancelCapture("绑定取消");
00658: 			}
00659: 			return;
00660: 		}
00661: 		try
00662: 		{
00663: 			if (KeyBindHelper.IsCapturing)
00664: 			{
00665: 				return;
00666: 			}
00667: 		}
00668: 		catch
00669: 		{
00670: 		}
00671: 		for (int i = 8; i <= 350; i++)
00672: 		{
00673: 			KeyCode val = (KeyCode)i;
00674: 			if ((int)val != 27 && Input.GetKeyDown(val))
00675: 			{
00676: 				switch (_captureMode)
00677: 				{
00678: 				case CaptureMode.Single:
00679: 					TryBindSingle(_captureUid, val);
00680: 					break;
00681: 				case CaptureMode.All:
00682: 					TryBindAll(val);
00683: 					break;
00684: 				case CaptureMode.RestockSingle:
00685: 					TryBindRestockSingle(_captureUid, val);
00686: 					break;
00687: 				case CaptureMode.RestockAll:
00688: 					TryBindRestockAll(val);
00689: 					break;
00690: 				}
00691: 				break;
00692: 			}
00693: 		}
00694: 	}
00695: 
00696: 	private unsafe static string? OfficialConflictText(KeyCode key)
00697: 	{
00698: 		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
00699: 		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
00700: 		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
00701: 		try
00702: 		{
00703: 			foreach (Binding binding in KeyBindHelper.Bindings)
00704: 			{
00705: 				if (binding != null && KeyBindHelper.Get(binding.id) == key)
00706: 				{
00707: 					string text;
00708: 					try
00709: 					{
00710: 						text = binding.label?.Invoke();
00711: 					}
00712: 					catch
00713: 					{
00714: 						text = null;
00715: 					}
00716: 					if (!NullGuard.HasText(text))
00717: 					{
00718: 						text = binding.id;
00719: 					}
00720: 					string text2;
00721: 					try
00722: 					{
00723: 						text2 = LocHelper.GetLocalizedMainMenu("option_keybind_status_conflict", new object[2]
00724: 						{
00725: 							KeyBindHelper.KeyName(key),
00726: 							text
00727: 						});
00728: 					}
00729: 					catch
00730: 					{
00731: 						text2 = "与官方按键冲突";
00732: 					}
00733: 					Core.Debug("收纳箱快捷键 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + " 与官方键位 " + binding.id + " 冲突：" + text2);
00734: 					return text2;
00735: 				}
00736: 			}
00737: 		}
00738: 		catch
00739: 		{
00740: 		}
00741: 		return null;
00742: 	}
00743: 
00744: 	private static void RejectBind(GameItem? pack, CaptureMode mode, string msg, float seconds)
00745: 	{
00746: 		PanelEntry? entry = ((pack != null) ? FindIn(pack) : FindEntryByButton(_captureButton));
00747: 		SafeSetText(_captureButton, RestoreTextOf(mode, pack));
00748: 		_captureButton = null;
00749: 		ShowBanner(entry, msg, seconds);
00750: 		BoxItem.NotifyUser("快捷键绑定失败：" + msg);
00751: 		Core.Log?.Msg("收纳箱：快捷键绑定被拒——" + msg + "。");
00752: 	}
00753: 
00754: 	private static string RestoreTextOf(CaptureMode mode, GameItem? pack)
00755: 	{
00756: 		return mode switch
00757: 		{
00758: 			CaptureMode.Single => (pack != null) ? HotkeyText(pack) : "一键收纳:点击绑定", 
00759: 			CaptureMode.All => AllKeyText(), 
00760: 			CaptureMode.RestockSingle => (pack != null) ? RestockKeyText(pack) : "一键上货:点击绑定", 
00761: 			CaptureMode.RestockAll => AllRestockKeyText(), 
00762: 			_ => "点击绑定", 
00763: 		};
00764: 	}
00765: 
00766: 	private static string? ModConflictText(KeyCode key, GameItem? self, bool restockSelf)
00767: 	{
00768: 		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
00769: 		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
00770: 		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
00771: 		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
00772: 		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
00773: 		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
00774: 		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
00775: 		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
00776: 		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
00777: 		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
00778: 		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
00779: 		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
00780: 		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
00781: 		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
00782: 		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
00783: 		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
00784: 		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
00785: 		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
00786: 		try
00787: 		{
00788: 			foreach (GameItem item in BoxItem.FindAllBoxes())
00789: 			{
00790: 				if (item == null)
00791: 				{
00792: 					continue;
00793: 				}
00794: 				bool flag = self != null && item == self;
00795: 				KeyCode val = (KeyCode)StorageGrid.GetBoxOption(item, "HOTKEY");
00796: 				KeyCode val2 = (KeyCode)StorageGrid.GetBoxOption(item, "HOTRESTOCK");
00797: 				if (flag)
00798: 				{
00799: 					if ((restockSelf || val != key) && (!restockSelf || val2 != key))
00800: 					{
00801: 						if (val == key)
00802: 						{
00803: 							return "该键已被本箱一键收纳占用";
00804: 						}
00805: 						if (val2 == key)
00806: 						{
00807: 							return "该键已被本箱一键上货占用";
00808: 						}
00809: 					}
00810: 				}
00811: 				else
00812: 				{
00813: 					if (val == key)
00814: 					{
00815: 						return "该键已被一键收纳占用";
00816: 					}
00817: 					if (val2 == key)
00818: 					{
00819: 						return "该键已被一键上货占用";
00820: 					}
00821: 				}
00822: 			}
00823: 		}
00824: 		catch
00825: 		{
00826: 		}
00827: 		if (BoxStowAll.Key == key)
00828: 		{
00829: 			return "该键已被全箱收纳占用";
00830: 		}
00831: 		if (BoxRestock.Key == key)
00832: 		{
00833: 			return "该键已被全箱上货占用";
00834: 		}
00835: 		return null;
00836: 	}
00837: 
00838: 	private unsafe static void TryBindSingle(long uid, KeyCode key)
00839: 	{
00840: 		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
00841: 		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
00842: 		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
00843: 		//IL_0069: Expected I4, but got Unknown
00844: 		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
00845: 		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
00846: 		GameItem val = FindByUid(uid);
00847: 		_captureMode = CaptureMode.None;
00848: 		EndCaptureFrame();
00849: 		if (val == null)
00850: 		{
00851: 			SafeSetText(_captureButton, "一键收纳:点击绑定");
00852: 			_captureButton = null;
00853: 			return;
00854: 		}
00855: 		string text = OfficialConflictText(key);
00856: 		if (text != null)
00857: 		{
00858: 			RejectBind(val, CaptureMode.Single, text, 3f);
00859: 			return;
00860: 		}
00861: 		string text2 = ModConflictText(key, val, restockSelf: false);
00862: 		if (text2 != null)
00863: 		{
00864: 			RejectBind(val, CaptureMode.Single, text2, 2.5f);
00865: 			return;
00866: 		}
00867: 		StorageGrid.SetBoxOption(val, "HOTKEY", (int)key);
00868: 		SafeSetText(_captureButton, HotkeyText(val));
00869: 		_captureButton = null;
00870: 		BoxHotkey.NotifyBound(uid, key);
00871: 		BoxHotkey.SuppressUntilRelease(key);
00872: 		Core.Log?.Msg("收纳箱：本箱快捷键绑定为 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + "。");
00873: 	}
00874: 
00875: 	private unsafe static void TryBindAll(KeyCode key)
00876: 	{
00877: 		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
00878: 		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
00879: 		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
00880: 		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
00881: 		GameItem pack = FindByUid(_captureUid);
00882: 		_captureMode = CaptureMode.None;
00883: 		EndCaptureFrame();
00884: 		string text = OfficialConflictText(key);
00885: 		if (text != null)
00886: 		{
00887: 			RejectBind(pack, CaptureMode.All, text, 3f);
00888: 			return;
00889: 		}
00890: 		string text2 = ModConflictText(key, null, restockSelf: false);
00891: 		if (text2 != null)
00892: 		{
00893: 			RejectBind(pack, CaptureMode.All, text2, 2.5f);
00894: 			return;
00895: 		}
00896: 		BoxStowAll.SetKey(key);
00897: 		SafeSetText(_captureButton, AllKeyText());
00898: 		_captureButton = null;
00899: 		BoxHotkey.SuppressUntilRelease(key);
00900: 		SyncGlobalKeyButtons();
00901: 		Core.Log?.Msg("收纳箱：全箱收纳键绑定为 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + "。");
00902: 	}
00903: 
00904: 	private unsafe static void TryBindRestockSingle(long uid, KeyCode key)
00905: 	{
00906: 		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
00907: 		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
00908: 		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
00909: 		//IL_0069: Expected I4, but got Unknown
00910: 		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
00911: 		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
00912: 		GameItem val = FindByUid(uid);
00913: 		_captureMode = CaptureMode.None;
00914: 		EndCaptureFrame();
00915: 		if (val == null)
00916: 		{
00917: 			SafeSetText(_captureButton, "一键上货:点击绑定");
00918: 			_captureButton = null;
00919: 			return;
00920: 		}
00921: 		string text = OfficialConflictText(key);
00922: 		if (text != null)
00923: 		{
00924: 			RejectBind(val, CaptureMode.RestockSingle, text, 3f);
00925: 			return;
00926: 		}
00927: 		string text2 = ModConflictText(key, val, restockSelf: true);
00928: 		if (text2 != null)
00929: 		{
00930: 			RejectBind(val, CaptureMode.RestockSingle, text2, 2.5f);
00931: 			return;
00932: 		}
00933: 		StorageGrid.SetBoxOption(val, "HOTRESTOCK", (int)key);
00934: 		SafeSetText(_captureButton, RestockKeyText(val));
00935: 		_captureButton = null;
00936: 		BoxHotkey.NotifyBound(uid, key, restock: true);
00937: 		BoxHotkey.SuppressUntilRelease(key);
00938: 		Core.Log?.Msg("收纳箱：本箱上货快捷键绑定为 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + "。");
00939: 	}
00940: 
00941: 	private unsafe static void TryBindRestockAll(KeyCode key)
00942: 	{
00943: 		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
00944: 		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
00945: 		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
00946: 		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
00947: 		GameItem pack = FindByUid(_captureUid);
00948: 		_captureMode = CaptureMode.None;
00949: 		EndCaptureFrame();
00950: 		string text = OfficialConflictText(key);
00951: 		if (text != null)
00952: 		{
00953: 			RejectBind(pack, CaptureMode.RestockAll, text, 3f);
00954: 			return;
00955: 		}
00956: 		string text2 = ModConflictText(key, null, restockSelf: true);
00957: 		if (text2 != null)
00958: 		{
00959: 			RejectBind(pack, CaptureMode.RestockAll, text2, 2.5f);
00960: 			return;
00961: 		}
00962: 		BoxRestock.SetKey(key);
00963: 		SafeSetText(_captureButton, AllRestockKeyText());
00964: 		_captureButton = null;
00965: 		BoxHotkey.SuppressUntilRelease(key);
00966: 		SyncGlobalKeyButtons();
00967: 		Core.Log?.Msg("收纳箱：全箱上货键绑定为 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + "。");
00968: 	}
00969: 
00970: 	private static void SyncGlobalKeyButtons()
00971: 	{
00972: 		string text = AllKeyText();
00973: 		string text2 = AllRestockKeyText();
00974: 		foreach (PanelEntry value in PanelsByUid.Values)
00975: 		{
00976: 			if (ButtonAlive(value.AllKeyButton))
00977: 			{
00978: 				SafeSetText(value.AllKeyButton, text);
00979: 			}
00980: 			if (ButtonAlive(value.AllRestockKeyButton))
00981: 			{
00982: 				SafeSetText(value.AllRestockKeyButton, text2);
00983: 			}
00984: 		}
00985: 		foreach (PanelEntry value2 in PanelsByRef.Values)
00986: 		{
00987: 			if (ButtonAlive(value2.AllKeyButton))
00988: 			{
00989: 				SafeSetText(value2.AllKeyButton, text);
00990: 			}
00991: 			if (ButtonAlive(value2.AllRestockKeyButton))
00992: 			{
00993: 				SafeSetText(value2.AllRestockKeyButton, text2);
00994: 			}
00995: 		}
00996: 	}
00997: 
00998: 	private static PanelEntry? FindEntryByButton(MachineryButton? btn)
00999: 	{
01000: 		if (btn == null)
01001: 		{
01002: 			return null;
01003: 		}
01004: 		foreach (PanelEntry value in PanelsByUid.Values)
01005: 		{
01006: 			if (value.AllKeyButton == btn || value.HotkeyButton == btn || value.AllRestockKeyButton == btn || value.RestockKeyButton == btn)
01007: 			{
01008: 				return value;
01009: 			}
01010: 		}
01011: 		foreach (PanelEntry value2 in PanelsByRef.Values)
01012: 		{
01013: 			if (value2.AllKeyButton == btn || value2.HotkeyButton == btn || value2.AllRestockKeyButton == btn || value2.RestockKeyButton == btn)
01014: 			{
01015: 				return value2;
01016: 			}
01017: 		}
01018: 		return null;
01019: 	}
01020: 
01021: 	private static bool TryUnbindByEscape()
01022: 	{
01023: 		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
01024: 		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
01025: 		CaptureMode captureMode = _captureMode;
01026: 		PanelEntry entry = FindEntryByButton(_captureButton);
01027: 		switch (captureMode)
01028: 		{
01029: 		case CaptureMode.All:
01030: 		case CaptureMode.RestockAll:
01031: 			if (captureMode == CaptureMode.All)
01032: 			{
01033: 				if ((int)BoxStowAll.Key == 0)
01034: 				{
01035: 					return false;
01036: 				}
01037: 				BoxStowAll.UnbindManually();
01038: 			}
01039: 			else
01040: 			{
01041: 				if ((int)BoxRestock.Key == 0)
01042: 				{
01043: 					return false;
01044: 				}
01045: 				BoxRestock.UnbindManually();
01046: 			}
01047: 			EndCaptureFrame();
01048: 			_captureMode = CaptureMode.None;
01049: 			_captureUid = 0L;
01050: 			SafeSetText(_captureButton, RestoreTextOf(captureMode, null));
01051: 			_captureButton = null;
01052: 			SyncGlobalKeyButtons();
01053: 			ShowBanner(entry, (captureMode == CaptureMode.All) ? "全箱收纳快捷键已解除绑定" : "全箱上货快捷键已解除绑定", 2.5f);
01054: 			Core.Log?.Msg("收纳箱：" + ((captureMode == CaptureMode.All) ? "全箱收纳键" : "全箱上货键") + "已由玩家在捕获态按 Esc 主动解绑。");
01055: 			return true;
01056: 		case CaptureMode.Single:
01057: 		case CaptureMode.RestockSingle:
01058: 		{
01059: 			GameItem val = FindByUid(_captureUid);
01060: 			string key = ((captureMode == CaptureMode.Single) ? "HOTKEY" : "HOTRESTOCK");
01061: 			if (val == null || StorageGrid.GetBoxOption(val, key) == 0)
01062: 			{
01063: 				return false;
01064: 			}
01065: 			StorageGrid.SetBoxOption(val, key, 0);
01066: 			BoxHotkey.UnbindUidFor(_captureUid, captureMode == CaptureMode.RestockSingle);
01067: 			EndCaptureFrame();
01068: 			_captureMode = CaptureMode.None;
01069: 			_captureUid = 0L;
01070: 			SafeSetText(_captureButton, RestoreTextOf(captureMode, val));
01071: 			_captureButton = null;
01072: 			ShowBanner(entry, (captureMode == CaptureMode.Single) ? "一键收纳快捷键已解除绑定" : "一键上货快捷键已解除绑定", 2.5f);
01073: 			return true;
01074: 		}
01075: 		default:
01076: 			return false;
01077: 		}
01078: 	}
01079: 
01080: 	private static void CancelCapture(string reason)
01081: 	{
01082: 		CaptureMode captureMode = _captureMode;
01083: 		GameItem pack = ((captureMode == CaptureMode.Single || captureMode == CaptureMode.RestockSingle) ? FindByUid(_captureUid) : null);
01084: 		_captureMode = CaptureMode.None;
01085: 		_captureUid = 0L;
01086: 		EndCaptureFrame();
01087: 		SafeSetText(_captureButton, RestoreTextOf(captureMode, pack));
01088: 		_captureButton = null;
01089: 		Core.Debug("收纳箱快捷键：" + reason);
01090: 	}
01091: 
01092: 	private static void ShowBanner(PanelEntry? entry, string text, float seconds)
01093: 	{
01094: 		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
01095: 		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
01096: 		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
01097: 		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
01098: 		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
01099: 		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
01100: 		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
01101: 		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
01102: 		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
01103: 		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
01104: 		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
01105: 		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
01106: 		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
01107: 		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
01108: 		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
01109: 		//IL_0098: Expected O, but got Unknown
01110: 		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
01111: 		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
01112: 		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
01113: 		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
01114: 		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
01115: 		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
01116: 		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
01117: 		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
01118: 		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
01119: 		try
01120: 		{
01121: 			if (entry?.Window == null || string.IsNullOrEmpty(text))
01122: 			{
01123: 				return;
01124: 			}
01125: 			if ((Object)(object)_bannerTmp != (Object)null && _bannerOwner != entry.Window)
01126: 			{
01127: 				try
01128: 				{
01129: 					Object.Destroy((Object)(object)((Component)((TMP_Text)_bannerTmp).transform.parent).gameObject);
01130: 				}
01131: 				catch
01132: 				{
01133: 				}
01134: 				_bannerTmp = null;
01135: 			}
01136: 			if ((Object)(object)_bannerTmp == (Object)null)
01137: 			{
01138: 				GameObject gameObject = ((Component)entry.Window.handler).gameObject;
01139: 				GameObject val = new GameObject("Dream_GearBanner", new Type[1] { typeof(Image) });
01140: 				RectTransform component = val.GetComponent<RectTransform>();
01141: 				((Transform)component).SetParent(gameObject.transform, false);
01142: 				Vector2 val2 = (component.pivot = new Vector2(0.5f, 0.5f));
01143: 				Vector2 anchorMin = (component.anchorMax = val2);
01144: 				component.anchorMin = anchorMin;
01145: 				float num = 0f;
01146: 				try
01147: 				{
01148: 					Rect rect = entry.Window.rectTransform.rect;
01149: 					num = ((Rect)(ref rect)).height;
01150: 				}
01151: 				catch
01152: 				{
01153: 				}
01154: 				component.anchoredPosition = new Vector2(0f, num / 2f + 21f);
01155: 				component.sizeDelta = new Vector2(256f, 34f);
01156: 				((Transform)component).localScale = Vector3.one;
01157: 				Image component2 = val.GetComponent<Image>();
01158: 				((Graphic)component2).color = Color32.op_Implicit(new Color32((byte)18, (byte)14, (byte)16, (byte)236));
01159: 				((Graphic)component2).raycastTarget = false;
01160: 				GameObject val5 = new GameObject("Text", new Type[1] { typeof(RectTransform) });
01161: 				val5.transform.SetParent((Transform)(object)component, false);
01162: 				RectTransform component3 = val5.GetComponent<RectTransform>();
01163: 				component3.anchorMin = Vector2.zero;
01164: 				component3.anchorMax = Vector2.one;
01165: 				component3.offsetMin = new Vector2(7f, 3f);
01166: 				component3.offsetMax = new Vector2(-7f, -3f);
01167: 				TextMeshProUGUI obj3 = val5.AddComponent<TextMeshProUGUI>();
01168: 				((TMP_Text)obj3).font = BoxLabel.GetTmpFont();
01169: 				((TMP_Text)obj3).fontSize = 13f;
01170: 				((Graphic)obj3).color = Color32.op_Implicit(new Color32((byte)204, (byte)72, (byte)73, byte.MaxValue));
01171: 				((TMP_Text)obj3).alignment = (TextAlignmentOptions)514;
01172: 				((TMP_Text)obj3).enableWordWrapping = true;
01173: 				((Graphic)obj3).raycastTarget = false;
01174: 				_bannerTmp = obj3;
01175: 				_bannerOwner = entry.Window;
01176: 			}
01177: 			((Component)((TMP_Text)_bannerTmp).transform.parent).gameObject.SetActive(true);
01178: 			((TMP_Text)_bannerTmp).text = text;
01179: 			_bannerShownUntil = Time.realtimeSinceStartup + seconds;
01180: 		}
01181: 		catch (Exception ex)
01182: 		{
01183: 			Core.Debug("收纳箱齿轮横幅显示异常：" + ex.Message);
01184: 		}
01185: 	}
01186: 
01187: 	private static PanelEntry? FindIn(GameItem pack)
01188: 	{
01189: 		if (pack == null)
01190: 		{
01191: 			return null;
01192: 		}
01193: 		if (PanelsByRef.TryGetValue(pack, out PanelEntry value))
01194: 		{
01195: 			try
01196: 			{
01197: 				if (PanelsByUid.TryGetValue(pack.uniqueId, out PanelEntry value2) && value2.Window == value.Window)
01198: 				{
01199: 					return AliveOrForget(pack, value);
01200: 				}
01201: 			}
01202: 			catch
01203: 			{
01204: 			}
01205: 		}
01206: 		long num = 0L;
01207: 		try
01208: 		{
01209: 			num = pack.uniqueId;
01210: 		}
01211: 		catch
01212: 		{
01213: 		}
01214: 		if (num != 0L && PanelsByUid.TryGetValue(num, out PanelEntry value3) && value3.Window != null)
01215: 		{
01216: 			if (value3.Window == null || !WindowAlive(value3.Window))
01217: 			{
01218: 				ForgetByEntry(value3);
01219: 				return null;
01220: 			}
01221: 			return value3;
01222: 		}
01223: 		return null;
01224: 	}
01225: 
01226: 	private static bool WindowAlive(PixelWindow win)
01227: 	{
01228: 		try
01229: 		{
01230: 			TreeNodeRender handler = win.handler;
01231: 			return (Object)(object)((handler != null) ? ((Component)handler).gameObject : null) != (Object)null;
01232: 		}
01233: 		catch
01234: 		{
01235: 			return false;
01236: 		}
01237: 	}
01238: 
01239: 	private static PanelEntry? AliveOrForget(GameItem pack, PanelEntry entry)
01240: 	{
01241: 		if (!WindowAlive(entry.Window))
01242: 		{
01243: 			ForgetByEntry(entry);
01244: 			return null;
01245: 		}
01246: 		return entry;
01247: 	}
01248: 
01249: 	private static void ForgetByEntry(PanelEntry entry)
01250: 	{
01251: 		try
01252: 		{
01253: 			GameItem val = null;
01254: 			foreach (KeyValuePair<GameItem, PanelEntry> item in PanelsByRef)
01255: 			{
01256: 				if (item.Value == entry)
01257: 				{
01258: 					val = item.Key;
01259: 					break;
01260: 				}
01261: 			}
01262: 			if (val != null)
01263: 			{
01264: 				PanelsByRef.Remove(val);
01265: 			}
01266: 		}
01267: 		catch
01268: 		{
01269: 		}
01270: 		try
01271: 		{
01272: 			foreach (KeyValuePair<long, PanelEntry> item2 in PanelsByUid)
~~~

### ItemStorage/GhostSweep.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using UnityEngine;
00004: 
00005: namespace ItemStorage;
00006: 
00007: internal static class GhostSweep
00008: {
00009: 	private static float _nextRunAt;
00010: 
00011: 	internal static void Run(string context)
00012: 	{
00013: 		if (Time.time < _nextRunAt)
00014: 		{
00015: 			return;
00016: 		}
00017: 		_nextRunAt = Time.time + 5f;
00018: 		try
00019: 		{
00020: 			EmporiumEntry instance = EmporiumEntry.Instance;
00021: 			if ((Object)(object)instance == (Object)null)
00022: 			{
00023: 				return;
00024: 			}
00025: 			int num = 0;
00026: 			num += Sweep(instance.invElement, "桌面", context);
00027: 			num += Sweep(instance.frontInvinvElement, "柜台正面", context);
00028: 			num += Sweep(instance.backInvinvElement, "柜台里侧", context);
00029: 			num += Sweep(instance.backInvinvElementCounter, "柜台里侧B", context);
00030: 			if (num > 0)
00031: 			{
00032: 				Core.Debug("收纳箱[" + context + "] 空壳清理：移除 " + num + " 个隐形占格残留（数量为 0 / 无 ID）。");
00033: 				try
00034: 				{
00035: 					instance.Validate(true);
00036: 				}
00037: 				catch
00038: 				{
00039: 				}
00040: 			}
00041: 			else
00042: 			{
00043: 				Core.Debug("收纳箱[" + context + "] 空壳清理：网格表面无残留。");
00044: 			}
00045: 			BoxLabel.Sweep();
00046: 			BoxLabel.SweepAttach();
00047: 		}
00048: 		catch (Exception ex)
00049: 		{
00050: 			Core.Log?.Warning("收纳箱[" + context + "] 空壳清理：" + ex.Message);
00051: 		}
00052: 	}
00053: 
00054: 	private static int Sweep(GameGridInventory? grid, string label, string context)
00055: 	{
00056: 		if (grid == null)
00057: 		{
00058: 			return 0;
00059: 		}
00060: 		int num = 0;
00061: 		try
00062: 		{
00063: 			foreach (GameItem childItem in ((GameInventory)grid).childItems)
00064: 			{
00065: 				if (childItem == null)
00066: 				{
00067: 					continue;
00068: 				}
00069: 				try
00070: 				{
00071: 					if (childItem == null)
00072: 					{
00073: 						continue;
00074: 					}
00075: 					int unitCount;
00076: 					try
00077: 					{
00078: 						unitCount = childItem.unitCount;
00079: 					}
00080: 					catch
00081: 					{
00082: 						goto end_IL_0027;
00083: 					}
00084: 					string text;
00085: 					try
00086: 					{
00087: 						text = Core.Clean(childItem.identifier);
00088: 					}
00089: 					catch
00090: 					{
00091: 						goto end_IL_0027;
00092: 					}
00093: 					if (unitCount > 0 && !string.IsNullOrEmpty(text))
00094: 					{
00095: 						continue;
00096: 					}
00097: 					List<GraphNodeStorage> list = null;
00098: 					try
00099: 					{
00100: 						list = childItem.parents;
00101: 					}
00102: 					catch
00103: 					{
00104: 						list = null;
00105: 					}
00106: 					if (list != null)
00107: 					{
00108: 						foreach (GraphNodeStorage item in list)
00109: 						{
00110: 							if (item != null)
00111: 							{
00112: 								try
00113: 								{
00114: 									childItem.RemoveParent(item);
00115: 								}
00116: 								catch
00117: 								{
00118: 								}
00119: 							}
00120: 						}
00121: 					}
00122: 					bool flag = false;
00123: 					try
00124: 					{
00125: 						flag = GraphUtils.DestroyIfUnplaced(childItem, "ItemStorage_Sweep");
00126: 					}
00127: 					catch
00128: 					{
00129: 						flag = false;
00130: 					}
00131: 					if (!flag)
00132: 					{
00133: 						try
00134: 						{
00135: 							childItem.Destroy();
00136: 						}
00137: 						catch
00138: 						{
00139: 							goto end_IL_0027;
00140: 						}
00141: 					}
00142: 					num++;
00143: 					Core.Debug("收纳箱[" + context + "] 空壳清理[" + label + "]：删除 '" + (string.IsNullOrEmpty(text) ? "(无ID)" : text) + "'（数量 " + unitCount + "）。");
00144: 					end_IL_0027:;
00145: 				}
00146: 				catch (Exception ex)
00147: 				{
00148: 					Core.Debug("收纳箱[" + context + "] 空壳清理[" + label + "] 单件失败：" + ex.Message);
00149: 				}
00150: 			}
00151: 		}
00152: 		catch (Exception ex2)
00153: 		{
00154: 			Core.Debug("收纳箱[" + context + "] 空壳扫描[" + label + "]：" + ex2.Message);
00155: 		}
00156: 		return num;
00157: 	}
00158: }
~~~

### ItemStorage/JacksonStock.cs

~~~csharp
00079: 			{
00080: 				flag = false;
00081: 			}
00082: 			bool flag2 = false;
00083: 			if (!flag)
00084: 			{
00085: 				try
00086: 				{
00087: 					flag = PlayerStore.Instance != null && (int)PlayerStore.Instance.playerLocation == 1;
00088: 				}
00089: 				catch
00090: 				{
00091: 					flag = false;
00092: 				}
00093: 				if (flag)
00094: 				{
00095: 					try
00096: 					{
00097: 						MapUIManager instance2 = MapUIManager.Instance;
00098: 						flag2 = (Object)(object)instance2 == (Object)null || instance2.Current == null || string.Equals(Core.Clean(instance2.Current.Id), "dr_jackson", StringComparison.OrdinalIgnoreCase);
00099: 					}
00100: 					catch
00101: 					{
00102: 						flag2 = true;
00103: 					}
00104: 					flag = flag2 && !FrontHasUnownedBox();
00105: 				}
00106: 			}
00107: 			if (!flag || _stockedThisArrival)
00108: 			{
00109: 				return;
00110: 			}
00111: 			_stockedThisArrival = true;
00112: 			GameItem val2 = DirectoryMaster.Item("Dream_Item_Storage", true);
00113: 			PlayerStore instance3 = PlayerStore.Instance;
00114: 			if (val2 == null || instance3 == null || !TryPlace(instance3, val2))
00115: 			{
00116: 				if (flag)
00117: 				{
00118: 					_stockedThisArrival = false;
00119: 				}
00120: 				Core.Debug("博士钩子补箱未落位，交由 Postfix/兜底接管。");
00121: 			}
00122: 			else
00123: 			{
00124: 				Core.Log?.Msg("博士货源已补入：收纳箱 ×1（官方铺货钩子，" + (flag2 ? "拜访博士" : "列博士货物首位") + "）。");
00125: 			}
00126: 		}
00127: 		catch (Exception ex)
00128: 		{
00129: 			_stockedThisArrival = false;
00130: 			Core.Debug("博士钩子补货异常：" + ex.Message);
00131: 		}
00132: 	}
00133: 
00134: 	internal static void OnClientArrived()
00135: 	{
00136: 		try
00137: 		{
00138: 			PlayerStore instance = PlayerStore.Instance;
00139: 			object obj;
00140: 			if (instance == null)
00141: 			{
00142: 				obj = null;
00143: 			}
00144: 			else
00145: 			{
00146: 				StoreClientInstance currentClientInstance = instance.currentClientInstance;
00147: 				obj = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
00148: 			}
00149: 			StoreClient val = (StoreClient)obj;
00150: 			if (val == null || val == _lastClient)
00151: 			{
00152: 				return;
00153: 			}
00154: 			_lastClient = val;
00155: 			if (IsJackson(val))
00156: 			{
00157: 				string text = "";
00158: 				string text2 = "";
00159: 				try
00160: 				{
00161: 					text = Core.Clean(val.identifier) ?? "";
00162: 				}
00163: 				catch
00164: 				{
00165: 				}
00166: 				try
00167: 				{
00168: 					text2 = Core.Clean(val.displayName) ?? "";
00169: 				}
00170: 				catch
00171: 				{
00172: 				}
00173: 				Core.Debug("博士到场：id=" + text + " 名=" + text2 + "。");
00174: 			}
00175: 			_stockedThisArrival = IsScriptedVisit(val) && FrontHasUnownedBox();
00176: 			_fallbackDeferrals = 0;
00177: 			_fallbackCheckAt = Time.time + 3f;
00178: 		}
00179: 		catch (Exception ex)
00180: 		{
00181: 			Core.Debug("博士货源处理失败：" + ex.Message);
00182: 		}
00183: 	}
00184: 
00185: 	internal static void OnMerchantItemAddedToTable()
00186: 	{
00187: 		try
00188: 		{
00189: 			if (_stockedThisArrival)
00190: 			{
00191: 				return;
00192: 			}
00193: 			PlayerStore instance = PlayerStore.Instance;
00194: 			object obj;
00195: 			if (instance == null)
00196: 			{
00197: 				obj = null;
00198: 			}
00199: 			else
00200: 			{
00201: 				StoreClientInstance currentClientInstance = instance.currentClientInstance;
00202: 				obj = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
00203: 			}
00204: 			StoreClient val = (StoreClient)obj;
00205: 			if (val != null && IsScriptedVisit(val))
00206: 			{
00207: 				_stockedThisArrival = true;
00208: 				if (!EnsureStock())
00209: 				{
00210: 					_stockedThisArrival = false;
00211: 				}
00212: 			}
00213: 		}
~~~

~~~csharp
00308: 			{
00309: 				if (string.Equals(a, b, StringComparison.Ordinal))
00310: 				{
00311: 					return true;
00312: 				}
00313: 			}
00314: 		}
00315: 		catch
00316: 		{
00317: 		}
00318: 		return false;
00319: 	}
00320: 
00321: 	private static bool IsScriptedVisit(StoreClient client)
00322: 	{
00323: 		try
00324: 		{
00325: 			string a = (Core.Clean(client.identifier) ?? "").ToLowerInvariant();
00326: 			string[] scriptedVisits = ScriptedVisits;
00327: 			foreach (string b in scriptedVisits)
00328: 			{
00329: 				if (string.Equals(a, b, StringComparison.Ordinal))
00330: 				{
00331: 					return true;
00332: 				}
00333: 			}
00334: 		}
00335: 		catch
00336: 		{
00337: 		}
00338: 		return false;
00339: 	}
00340: 
00341: 	private static bool EnsureStock()
00342: 	{
00343: 		EmporiumEntry instance = EmporiumEntry.Instance;
00344: 		GameGridInventory val = ((instance != null) ? instance.frontInvinvElement : null);
00345: 		if (val == null)
00346: 		{
00347: 			Core.Debug("博士柜台不可读，下次到场重试。");
00348: 			return false;
00349: 		}
00350: 		GameItem val2 = DirectoryMaster.Item("Dream_Item_Storage", true);
00351: 		if (val2 == null)
00352: 		{
00353: 			Core.Debug("收纳箱工厂未就绪，下次到场重试，不留标记。");
00354: 			return false;
00355: 		}
00356: 		PlayerStore instance2 = PlayerStore.Instance;
00357: 		if (instance2 == null)
00358: 		{
00359: 			Core.Debug("PlayerStore 未就绪，下次到场重试。");
00360: 			return false;
00361: 		}
00362: 		int num = 0;
00363: 		while (val.TryFindOneValidInventorySlot(val2, false) == null)
00364: 		{
00365: 			if (!TryMakeRoom(val) || ++num > 8)
00366: 			{
00367: 				Core.Debug("博士柜台放不下收纳箱且无可挤重复货物（已挤 " + num + " 件），下次到场重试。");
00368: 				return false;
00369: 			}
00370: 		}
00371: 		if (TryPlace(instance2, val2))
00372: 		{
00373: 			Core.Log?.Msg("博士货源已补入：收纳箱 ×1（柜台，走购买流程）。");
00374: 			return true;
00375: 		}
00376: 		while (num <= 8 && TryMakeRoom(val))
00377: 		{
00378: 			num++;
00379: 			GameItem val3 = DirectoryMaster.Item("Dream_Item_Storage", true);
00380: 			if (val3 != null && TryPlace(instance2, val3))
00381: 			{
00382: 				Core.Log?.Msg("博士货源已补入：收纳箱 ×1（柜台，走购买流程，挤占 " + num + " 件后自愈）。");
00383: 				return true;
00384: 			}
00385: 		}
00386: 		Core.Debug("博士货源上柜未落位（循环挤占后仍失败），回退标记待重试。");
00387: 		return false;
00388: 	}
00389: 
00390: 	private static bool FrontHasUnownedBox()
00391: 	{
00392: 		EmporiumEntry instance = EmporiumEntry.Instance;
00393: 		GameGridInventory val = ((instance != null) ? instance.frontInvinvElement : null);
00394: 		if (val == null)
00395: 		{
00396: 			return false;
00397: 		}
00398: 		try
00399: 		{
00400: 			foreach (GameItem childItem in ((GameInventory)val).childItems)
00401: 			{
00402: 				if (childItem == null)
00403: 				{
00404: 					continue;
00405: 				}
00406: 				string a = null;
00407: 				try
00408: 				{
00409: 					a = Core.Clean(childItem.identifier);
00410: 				}
00411: 				catch
00412: 				{
00413: 				}
00414: 				if (!string.Equals(a, "Dream_Item_Storage", StringComparison.Ordinal))
00415: 				{
00416: 					continue;
00417: 				}
00418: 				try
00419: 				{
00420: 					if (!GeneralHelper.IsItemOwned(childItem))
00421: 					{
00422: 						return true;
00423: 					}
00424: 				}
00425: 				catch
00426: 				{
00427: 				}
00428: 			}
00429: 		}
00430: 		catch (Exception ex)
00431: 		{
00432: 			Core.Debug("拜访补箱去重检查失败（按未补处理）：" + ex.Message);
00433: 		}
00434: 		return false;
00435: 	}
00436: 
00437: 	private static bool TryPlace(PlayerStore store, GameItem box)
00438: 	{
00439: 		if (box == null)
00440: 		{
00441: 			return false;
00442: 		}
00443: 		try
00444: 		{
00445: 			return store.TryAddDirectSellingItemToTable(box, false, false, false, 0);
00446: 		}
00447: 		catch (Exception ex)
00448: 		{
00449: 			Core.Debug("博士上柜调用异常：" + ex.Message);
00450: 			return false;
00451: 		}
00452: 	}
00453: 
00454: 	private static bool TryMakeRoom(GameGridInventory front)
00455: 	{
00456: 		GameItem val = FindVictim(front);
00457: 		if (val == null)
00458: 		{
00459: 			return false;
00460: 		}
00461: 		string text = "";
00462: 		long num = 0L;
00463: 		try
00464: 		{
00465: 			text = Core.Clean(val.identifier) ?? "";
00466: 		}
00467: 		catch
00468: 		{
00469: 		}
00470: 		try
00471: 		{
~~~

### ItemStorage/SlotLock.cs

~~~csharp
00001: using System;
00002: using UnityEngine;
00003: using UnityEngine.UI;
00004: 
00005: namespace ItemStorage;
00006: 
00007: internal static class SlotLock
00008: {
00009: 	internal static bool IsLocked(GameItem pack, int slot)
00010: 	{
00011: 		string slotLock = StorageGrid.GetSlotLock(pack, slot);
00012: 		if (!NullGuard.HasText(slotLock))
00013: 		{
00014: 			return false;
00015: 		}
00016: 		string itemId = StorageGrid.GetItemId(pack, slot);
00017: 		if (NullGuard.HasText(itemId))
00018: 		{
00019: 			return string.Equals(itemId, slotLock, StringComparison.Ordinal);
00020: 		}
00021: 		return false;
00022: 	}
00023: 
00024: 	internal static bool? Toggle(GameItem pack, int slot)
00025: 	{
00026: 		if (pack == null || !StorageGrid.IsValidSlot(slot))
00027: 		{
00028: 			return null;
00029: 		}
00030: 		try
00031: 		{
00032: 			if (IsLocked(pack, slot))
00033: 			{
00034: 				StorageGrid.SetSlotLock(pack, slot, null);
00035: 				Core.Log?.Msg("收纳箱：格 " + slot + " 已解锁（重新参与自动上货）。");
00036: 				NotifyToggle(pack, slot, locked: false);
00037: 				return false;
00038: 			}
00039: 			string itemId = StorageGrid.GetItemId(pack, slot);
00040: 			if (!NullGuard.HasText(itemId))
00041: 			{
00042: 				BoxItem.NotifyUser("空格子无法锁定，请先放入物品");
00043: 				return null;
00044: 			}
00045: 			if (!BoxItem.IsUsableByPlayer(pack))
00046: 			{
00047: 				BoxItem.NotifyUnowned();
00048: 				return null;
00049: 			}
00050: 			StorageGrid.SetSlotLock(pack, slot, itemId);
00051: 			Core.Log?.Msg("收纳箱：格 " + slot + " '" + itemId + "' 已锁定（不参与自动上货）。");
00052: 			NotifyToggle(pack, slot, locked: true);
00053: 			return true;
00054: 		}
00055: 		catch (Exception ex)
00056: 		{
00057: 			Core.Log?.Warning("收纳箱 SlotLock.Toggle(" + slot + ")：" + ex.Message);
00058: 			return null;
00059: 		}
00060: 	}
00061: 
00062: 	private static void NotifyToggle(GameItem pack, int slot, bool locked)
00063: 	{
00064: 		try
00065: 		{
00066: 			string text = StorageGrid.GetDisplayName(pack, slot);
00067: 			if (!NullGuard.HasText(text))
00068: 			{
00069: 				string itemId = StorageGrid.GetItemId(pack, slot);
00070: 				if (NullGuard.HasText(itemId))
00071: 				{
00072: 					text = BoxWindow.DisplayNameForItem(itemId, int.MaxValue);
00073: 				}
00074: 			}
00075: 			if (NullGuard.HasText(text))
00076: 			{
00077: 				BoxItem.NotifyUser(locked ? (text + " 将不参与自动上货") : (text + " 重新参与自动上货"));
00078: 			}
00079: 		}
00080: 		catch
00081: 		{
00082: 		}
00083: 	}
00084: 
00085: 	internal static Color BadgeColor()
00086: 	{
00087: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00088: 		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
00089: 		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
00090: 		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
00091: 		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
00092: 		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
00093: 		try
00094: 		{
00095: 			return Color32.op_Implicit(RenderHandler.GetColorFromPalette((ColorPalette)12615037));
00096: 		}
00097: 		catch
00098: 		{
00099: 			return new Color(0.85f, 0.15f, 0.15f, 1f);
00100: 		}
00101: 	}
00102: 
00103: 	internal static Color LockTextColor()
00104: 	{
00105: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00106: 		return Color.white;
00107: 	}
00108: 
00109: 	internal static Color LockPlateColor()
00110: 	{
00111: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00112: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00113: 		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
00114: 		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
00115: 		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
00116: 		try
00117: 		{
00118: 			return MachineryStyle.LABEL_TEXT;
00119: 		}
00120: 		catch
00121: 		{
00122: 			return new Color(0.13f, 0.12f, 0.14f, 1f);
00123: 		}
00124: 	}
00125: 
00126: 	internal static Color LabelColor()
00127: 	{
00128: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00129: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00130: 		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
00131: 		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
00132: 		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
00133: 		try
00134: 		{
00135: 			return MachineryStyle.LABEL_TEXT;
00136: 		}
00137: 		catch
00138: 		{
00139: 			return new Color(0.18f, 0.2f, 0.18f, 1f);
00140: 		}
00141: 	}
00142: 
00143: 	internal static Color PlateColor()
00144: 	{
00145: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00146: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
~~~

~~~csharp
00176: 		try
00177: 		{
00178: 			Transform val = ((Component)icon).transform.Find("item_storage_slot_lock");
00179: 			if (!IsLocked(pack, slot))
00180: 			{
00181: 				if ((Object)(object)val != (Object)null && ((Component)val).gameObject.activeSelf)
00182: 				{
00183: 					((Component)val).gameObject.SetActive(false);
00184: 				}
00185: 				return;
00186: 			}
00187: 			GameObject val2;
00188: 			if ((Object)(object)val == (Object)null)
00189: 			{
00190: 				val2 = new GameObject("item_storage_slot_lock");
00191: 				val2.transform.SetParent(((Component)icon).transform, false);
00192: 				Transform transform = val2.transform;
00193: 				object obj = ((object)((transform is RectTransform) ? transform : null)) ?? ((object)val2.AddComponent<RectTransform>());
00194: 				((RectTransform)obj).anchorMin = new Vector2(0f, 1f);
00195: 				((RectTransform)obj).anchorMax = new Vector2(0f, 1f);
00196: 				((RectTransform)obj).pivot = new Vector2(0f, 1f);
00197: 				((RectTransform)obj).anchoredPosition = new Vector2(2f, -2f);
00198: 				((RectTransform)obj).sizeDelta = new Vector2(6f, 6f);
00199: 				((Transform)obj).localScale = Vector3.one;
00200: 				Image obj2 = val2.AddComponent<Image>();
00201: 				obj2.sprite = null;
00202: 				((Graphic)obj2).color = BadgeColor();
00203: 				((Graphic)obj2).raycastTarget = false;
00204: 			}
00205: 			else
00206: 			{
00207: 				val2 = ((Component)val).gameObject;
00208: 				Image component = val2.GetComponent<Image>();
00209: 				if ((Object)(object)component != (Object)null)
00210: 				{
00211: 					((Graphic)component).color = BadgeColor();
00212: 				}
00213: 			}
00214: 			if (!val2.activeSelf)
00215: 			{
00216: 				val2.SetActive(true);
00217: 			}
00218: 		}
00219: 		catch (Exception ex)
00220: 		{
00221: 			Core.Debug("收纳箱 锁定红点刷新(" + slot + ")：" + ex.Message);
00222: 		}
00223: 	}
00224: }
~~~

### ItemStorage/StorageGrid.cs

~~~csharp
01750: 		{
01751: 			return (isUse: false, cur: 0, max: 0);
01752: 		}
01753: 	}
01754: 
01755: 	private static string OptTag(string key)
01756: 	{
01757: 		return "ITEM_STORAGE_SLOT_OPT_" + key;
01758: 	}
01759: 
01760: 	internal static int GetBoxOption(GameItem pack, string key, int def = 0)
01761: 	{
01762: 		try
01763: 		{
01764: 			TagSystem state = pack.state;
01765: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
01766: 			string key2 = OptTag(key);
01767: 			if (dictionary == null || !dictionary.ContainsKey(key2))
01768: 			{
01769: 				return def;
01770: 			}
01771: 			return dictionary[key2]?.valueInt ?? def;
01772: 		}
01773: 		catch
01774: 		{
01775: 			return def;
01776: 		}
01777: 	}
01778: 
01779: 	internal static void SetBoxOption(GameItem pack, string key, int value)
01780: 	{
01781: 		try
01782: 		{
01783: 			string tag = OptTag(key);
01784: 			if (value == 0)
01785: 			{
01786: 				DestroyTag(pack, tag);
01787: 			}
01788: 			else
01789: 			{
01790: 				WriteCountTag(pack, tag, value);
01791: 			}
01792: 		}
01793: 		catch (Exception ex)
01794: 		{
01795: 			Core.Log?.Warning("收纳箱 SetBoxOption(" + key + "=" + value + ")：" + ex.Message);
01796: 		}
01797: 	}
01798: 
01799: 	internal static void SetBoxOptionRaw(GameItem pack, string key, int value)
01800: 	{
01801: 		try
01802: 		{
01803: 			WriteCountTag(pack, OptTag(key), value);
01804: 		}
01805: 		catch (Exception ex)
01806: 		{
01807: 			Core.Log?.Warning("收纳箱 SetBoxOptionRaw(" + key + "=" + value + ")：" + ex.Message);
01808: 		}
01809: 	}
01810: 
01811: 	internal static string? GetSlotLock(GameItem pack, int slot)
01812: 	{
01813: 		if (!IsValidSlot(slot))
01814: 		{
01815: 			return null;
01816: 		}
01817: 		try
01818: 		{
01819: 			TagSystem state = pack.state;
01820: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
01821: 			if (dictionary == null || !dictionary.ContainsKey(LockTag(slot)))
01822: 			{
01823: 				return null;
01824: 			}
01825: 			TagState val = dictionary[LockTag(slot)];
01826: 			if (val == null)
01827: 			{
01828: 				return null;
01829: 			}
01830: 			string text = Core.Clean(val.valueString);
01831: 			return string.IsNullOrEmpty(text) ? null : text;
01832: 		}
01833: 		catch
01834: 		{
01835: 			return null;
01836: 		}
01837: 	}
01838: 
01839: 	internal static void SetSlotLock(GameItem pack, int slot, string? itemId)
01840: 	{
01841: 		if (IsValidSlot(slot))
01842: 		{
01843: 			string value = Core.Clean(itemId);
01844: 			if (string.IsNullOrEmpty(value))
01845: 			{
01846: 				DestroyTag(pack, LockTag(slot));
01847: 			}
01848: 			else
01849: 			{
01850: 				WriteTag(pack, LockTag(slot), value);
01851: 			}
01852: 		}
01853: 	}
01854: 
01855: 	internal static bool MoveSlotTo(GameItem src, int s, GameItem dst, int d)
01856: 	{
01857: 		if (!IsValidSlot(s) || !IsValidSlot(d))
01858: 		{
01859: 			return false;
01860: 		}
01861: 		if (string.IsNullOrEmpty(GetItemId(src, s)))
01862: 		{
01863: 			return false;
01864: 		}
01865: 		if (!string.IsNullOrEmpty(GetItemId(dst, d)))
01866: 		{
01867: 			return false;
01868: 		}
01869: 		TagSystem state = src.state;
01870: 		Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
01871: 		TagSystem state2 = dst.state;
01872: 		Dictionary<string, TagState> dictionary2 = ((state2 != null) ? state2.dict : null);
01873: 		if (dictionary == null || dictionary2 == null)
01874: 		{
01875: 			return false;
01876: 		}
01877: 		try
01878: 		{
01879: 			string[] slotTagSuffixes = SlotTagSuffixes;
01880: 			foreach (string text in slotTagSuffixes)
01881: 			{
01882: 				string key = "ITEM_STORAGE_SLOT_" + s + text;
01883: 				string tag = "ITEM_STORAGE_SLOT_" + d + text;
01884: 				if (!dictionary.ContainsKey(key))
01885: 				{
01886: 					continue;
01887: 				}
01888: 				TagState val = dictionary[key];
01889: 				if (val != null)
01890: 				{
01891: 					if (!string.IsNullOrEmpty(val.valueString))
01892: 					{
01893: 						WriteTag(dst, tag, val.valueString);
01894: 					}
01895: 					else
01896: 					{
~~~

~~~csharp
01983: 						if (!string.IsNullOrEmpty(value.valueString))
01984: 						{
01985: 							WriteTag(dst, text2, value.valueString);
01986: 						}
01987: 						else
01988: 						{
01989: 							WriteCountTag(dst, text2, value.valueInt);
01990: 						}
01991: 					}
01992: 				}
01993: 			}
01994: 			string snapshotQueue = GetSnapshotQueue(src, s);
01995: 			if (!string.IsNullOrEmpty(snapshotQueue))
01996: 			{
01997: 				var (text3, text4) = TagSnapshot.SplitTail(snapshotQueue, num2);
01998: 				if (!string.IsNullOrEmpty(text4))
01999: 				{
02000: 					string snapshotQueue2 = GetSnapshotQueue(dst, d);
02001: 					SetSnapshotQueue(dst, d, string.IsNullOrEmpty(snapshotQueue2) ? text4 : (snapshotQueue2 + "\u0001" + text4));
02002: 				}
02003: 				SetSnapshotQueue(src, s, string.IsNullOrEmpty(text3) ? null : text3);
02004: 			}
02005: 			int num3 = GetEnergyPool(dst, d) + GetEnergyPool(src, s);
02006: 			if (num3 > 0)
02007: 			{
02008: 				WriteCountTag(dst, EnTag(d), num3);
02009: 			}
02010: 			int num4 = GetDurabilityPool(dst, d) + GetDurabilityPool(src, s);
02011: 			if (num4 > 0)
02012: 			{
02013: 				WriteCountTag(dst, DuTag(d), num4);
02014: 			}
02015: 			int num5 = GetUseCountPool(dst, d) + GetUseCountPool(src, s);
02016: 			if (num5 > 0)
02017: 			{
02018: 				WriteCountTag(dst, UseTag(d), num5);
02019: 			}
02020: 			int useCountMax = GetUseCountMax(src, s);
02021: 			int useCountMax2 = GetUseCountMax(dst, d);
02022: 			if (useCountMax2 == 0 && useCountMax > 0)
02023: 			{
02024: 				WriteCountTag(dst, UseMaxTag(d), useCountMax);
02025: 			}
02026: 			else if (useCountMax > 0 && useCountMax2 > 0 && useCountMax != useCountMax2)
02027: 			{
02028: 				Core.Debug("收纳箱合并：次数上限不一致（源 " + useCountMax + " / 目标 " + useCountMax2 + "），保留目标格值。");
02029: 			}
02030: 			int energyMax = GetEnergyMax(src, s);
02031: 			int energyMax2 = GetEnergyMax(dst, d);
02032: 			if (energyMax2 == 0 && energyMax > 0)
02033: 			{
02034: 				WriteCountTag(dst, EnMaxTag(d), energyMax);
02035: 			}
02036: 			else if (energyMax > 0 && energyMax2 > 0 && energyMax != energyMax2)
02037: 			{
02038: 				Core.Debug("收纳箱合并：电量上限不一致（源 " + energyMax + " / 目标 " + energyMax2 + "），保留目标格值。");
02039: 			}
02040: 			WriteCountTag(dst, CntTag(d), GetCount(dst, d) + num2);
02041: 			int num6 = count - num2;
02042: 			if (num6 <= 0)
02043: 			{
02044: 				ClearSlot(src, s);
02045: 			}
02046: 			else
02047: 			{
02048: 				WriteCountTag(src, CntTag(s), num6);
02049: 			}
02050: 			return true;
02051: 		}
02052: 		catch (Exception ex)
02053: 		{
02054: 			Core.Log?.Warning("StorageGrid.MergeSlots(" + s + "→" + d + ")：" + ex.Message);
02055: 			return false;
02056: 		}
02057: 	}
02058: 
02059: 	internal static List<int> EmptySlots(GameItem pack)
02060: 	{
02061: 		List<int> list = new List<int>();
02062: 		for (int i = 0; i < 16; i++)
02063: 		{
02064: 			if (string.IsNullOrEmpty(GetItemId(pack, i)))
02065: 			{
02066: 				list.Add(i);
02067: 			}
02068: 		}
02069: 		return list;
02070: 	}
02071: }
~~~

### ItemStorage/StoragePatch.cs

~~~csharp
00010: {
00011: 	private struct PendingDeposit
00012: 	{
00013: 		public GameItem Box;
00014: 
00015: 		public GameItem Item;
00016: 
00017: 		public int Slot;
00018: 
00019: 		public int Batch;
00020: 	}
00021: 
00022: 	private struct PendingWithdraw
00023: 	{
00024: 		public GameItem Box;
00025: 
00026: 		public int Slot;
00027: 
00028: 		public GameGridInventory? Drop;
00029: 	}
00030: 
00031: 	[HarmonyPatch]
00032: 	internal static class StorageEndDragPatch
00033: 	{
00034: 		private static MethodBase TargetMethod()
00035: 		{
00036: 			MethodInfo methodInfo = AccessTools.Method(typeof(ItemMouseDragHandler), "EndDrag", (Type[])null, (Type[])null);
00037: 			if (methodInfo == null)
00038: 			{
00039: 				throw new InvalidOperationException("ItemMouseDragHandler.EndDrag 未找到");
00040: 			}
00041: 			return methodInfo;
00042: 		}
00043: 
00044: 		private static void Prefix(ItemMouseDragHandler __instance)
00045: 		{
00046: 			try
00047: 			{
00048: 				GameItem draggedItem = GetDraggedItem(__instance);
00049: 				GameItem lastItem = GetLastItem(__instance);
00050: 				Core.Debug("拖拽诊断：EndDrag Prefix 触发， currentItem='" + SafeId(draggedItem) + "' lastItem='" + SafeId(lastItem) + "'");
00051: 				if (draggedItem != null && lastItem != null && BoxItem.IsBox(lastItem) && !BoxItem.IsBox(draggedItem))
00052: 				{
00053: 					if (IsLabeler(draggedItem))
00054: 					{
00055: 						Core.Debug("收纳箱 EndDrag Prefix：'" + SafeId(draggedItem) + "' 是标签机，让路改名流程不存入");
00056: 						return;
00057: 					}
00058: 					_pendingDeposits.Enqueue(new PendingDeposit
00059: 					{
00060: 						Box = lastItem,
00061: 						Item = draggedItem,
00062: 						Slot = -1
00063: 					});
00064: 					Core.Debug("收纳箱 EndDrag Prefix：登记待存入 '" + SafeId(draggedItem) + "' → 盒（队列 " + _pendingDeposits.Count + "）");
00065: 				}
00066: 			}
00067: 			catch (Exception ex)
00068: 			{
00069: 				Core.Log?.Warning("ItemStorage PrefixEndDragNotePending：" + ex.Message);
00070: 			}
00071: 		}
00072: 	}
00073: 
00074: 	[HarmonyPatch]
00075: 	internal static class ItemSpriteCapturePatch
00076: 	{
00077: 		private static MethodBase TargetMethod()
00078: 		{
00079: 			MethodInfo methodInfo = AccessTools.Method(typeof(GameItemElement), "ResolveSpriteByName", (Type[])null, (Type[])null);
00080: 			if (methodInfo == null)
00081: 			{
00082: 				throw new InvalidOperationException("GameItemElement.ResolveSpriteByName 未找到");
00083: 			}
00084: 			return methodInfo;
00085: 		}
00086: 
00087: 		private static void Postfix(GameItemElement __instance, string name, ref Sprite __result)
00088: 		{
00089: 			try
00090: 			{
00091: 				if (__instance != null)
00092: 				{
00093: 					if (SpriteAssets.IsRegisteredClean((name != null && name.IndexOf('\0') < 0) ? name.Trim() : Core.Clean(name)))
00094: 					{
00095: 						BoxLabel.Apply(__instance);
00096: 					}
00097: 					else
00098: 					{
00099: 						BoxLabel.Strip(__instance);
00100: 					}
00101: 				}
00102: 				string text = ((name != null && name.IndexOf('\0') < 0) ? name.Trim() : Core.Clean(name));
00103: 				if (!string.IsNullOrEmpty(text) && (Object)(object)__result != (Object)null && !IsUnknownSprite(__result))
00104: 				{
00105: 					ResolvedSpriteByName[text] = __result;
00106: 				}
00107: 				if (__instance == null || (Object)(object)__result == (Object)null)
00108: 				{
00109: 					return;
00110: 				}
00111: 				int uniqueId;
00112: 				try
00113: 				{
00114: 					uniqueId = ((GameItem)__instance).uniqueId;
00115: 				}
00116: 				catch
00117: 				{
00118: 					return;
00119: 				}
00120: 				if (uniqueId == 0 || !WithdrawSpriteOverride.TryGetValue(uniqueId, out (string, string, float) value))
00121: 				{
00122: 					return;
00123: 				}
00124: 				string text2 = Core.Clean(value.Item2);
00125: 				if (string.IsNullOrEmpty(text2) || string.Equals(text2, text, StringComparison.Ordinal))
00126: 				{
00127: 					return;
00128: 				}
00129: 				Sprite val = null;
00130: 				if (ResolvedSpriteByName.TryGetValue(text2, out Sprite value2) && IsAlive(value2))
00131: 				{
00132: 					val = value2;
00133: 				}
00134: 				if ((Object)(object)val == (Object)null && !string.IsNullOrEmpty(value.Item1))
00135: 				{
00136: 					try
00137: 					{
00138: 						val = RenderHandler.TryLoadFromAtlas(value.Item1, text2);
00139: 					}
00140: 					catch
00141: 					{
00142: 					}
00143: 				}
00144: 				if (IsAlive(val))
00145: 				{
00146: 					ResolvedSpriteByName[text2] = val;
00147: 					__result = val;
00148: 				}
00149: 			}
00150: 			catch
00151: 			{
00152: 			}
00153: 		}
~~~

~~~csharp
00185: 		private static void Postfix(GameItemElement __instance, ref Sprite __result)
00186: 		{
00187: 			try
00188: 			{
00189: 				if (__instance == null || (Object)(object)__result == (Object)null)
00190: 				{
00191: 					return;
00192: 				}
00193: 				int uniqueId;
00194: 				try
00195: 				{
00196: 					uniqueId = ((GameItem)__instance).uniqueId;
00197: 				}
00198: 				catch
00199: 				{
00200: 					return;
00201: 				}
00202: 				if (uniqueId == 0 || !WithdrawSpriteOverride.TryGetValue(uniqueId, out (string, string, float) value))
00203: 				{
00204: 					return;
00205: 				}
00206: 				string text = Core.Clean(value.Item2);
00207: 				if (string.IsNullOrEmpty(text))
00208: 				{
00209: 					return;
00210: 				}
00211: 				Sprite val = null;
00212: 				if (ResolvedSpriteByName.TryGetValue(text, out Sprite value2) && IsAlive(value2))
00213: 				{
00214: 					val = value2;
00215: 				}
00216: 				if ((Object)(object)val == (Object)null && !string.IsNullOrEmpty(value.Item1))
00217: 				{
00218: 					try
00219: 					{
00220: 						val = RenderHandler.TryLoadFromAtlas(value.Item1, text);
00221: 					}
00222: 					catch
00223: 					{
00224: 					}
00225: 				}
00226: 				if (IsAlive(val))
00227: 				{
00228: 					ResolvedSpriteByName[text] = val;
00229: 					__result = val;
00230: 					Core.Debug("收纳箱 贴图纠偏生效：uid=" + uniqueId + " → '" + text + "'");
00231: 				}
00232: 			}
00233: 			catch
00234: 			{
00235: 			}
00236: 		}
00237: 
00238: 		private static bool IsAlive(Sprite? s)
00239: 		{
00240: 			if ((Object)(object)s == (Object)null)
00241: 			{
00242: 				return false;
00243: 			}
00244: 			try
00245: 			{
00246: 				return (Object)(object)s != (Object)null;
00247: 			}
00248: 			catch
00249: 			{
00250: 				return false;
00251: 			}
00252: 		}
00253: 	}
00254: 
00255: 	private static readonly Queue<PendingDeposit> _pendingDeposits = new Queue<PendingDeposit>();
00256: 
00257: 	private static int _batchSeq;
00258: 
00259: 	private static readonly Queue<PendingWithdraw> _pendingWithdraws = new Queue<PendingWithdraw>();
00260: 
00261: 	private static bool _rightWasDown;
00262: 
00263: 	private static int _wOk;
00264: 
00265: 	private static ItemMouseDragHandler? _dragHandler;
00266: 
00267: 	private static bool _wasDragging;
00268: 
00269: 	private static GameItem? _dragItem;
00270: 
00271: 	private static float _nextDragFindAt;
00272: 
00273: 	private static float _nextMultiFindAt;
00274: 
00275: 	private static ItemMultiSelectHandler? _multiHandler;
00276: 
00277: 	private static bool _groupWasActive;
00278: 
00279: 	private static List<GameItem>? _groupSnapshot;
00280: 
00281: 	private static GameItem? _groupHoverItem;
00282: 
00283: 	private static FieldInfo? _lastItemField;
00284: 
00285: 	private static FieldInfo? _currentRemoveField;
00286: 
00287: 	private static FieldInfo? _maxRemoveField;
00288: 
00289: 	private static FieldInfo? _selectedItemsField;
00290: 
00291: 	private static FieldInfo? _hoverItemField;
00292: 
00293: 	internal static readonly Dictionary<string, Sprite> ResolvedSpriteByName = new Dictionary<string, Sprite>(StringComparer.Ordinal);
00294: 
00295: 	internal static readonly Dictionary<int, (string Atlas, string Spr, float RegTime)> WithdrawSpriteOverride = new Dictionary<int, (string, string, float)>();
00296: 
00297: 	internal static void TickDeferred()
00298: 	{
00299: 		TrackDragSnapshot();
00300: 		TrackGroupDrag();
00301: 		try
00302: 		{
00303: 			List<GameItem> list = null;
00304: 			Dictionary<int, int> dictionary = new Dictionary<int, int>();
00305: 			Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
00306: 			try
00307: 			{
00308: 				while (_pendingDeposits.Count > 0)
00309: 				{
00310: 					PendingDeposit pendingDeposit = _pendingDeposits.Dequeue();
00311: 					if (pendingDeposit.Box == null || pendingDeposit.Item == null)
00312: 					{
00313: 						continue;
00314: 					}
00315: 					bool notify = pendingDeposit.Batch == 0;
00316: 					try
00317: 					{
00318: 						if (BoxTransfer.TryDeposit(pendingDeposit.Box, pendingDeposit.Item, 1, pendingDeposit.Slot, consume: true, refresh: false, notify))
00319: 						{
00320: 							Core.Debug("收纳箱 deferred：存入成功（槽 " + pendingDeposit.Slot + "，'" + SafeId(pendingDeposit.Item) + "'）");
00321: 							if (list == null)
00322: 							{
00323: 								list = new List<GameItem>();
00324: 							}
00325: 							bool flag = false;
00326: 							foreach (GameItem item in list)
00327: 							{
00328: 								if (item == pendingDeposit.Box)
00329: 								{
00330: 									flag = true;
00331: 									break;
00332: 								}
00333: 							}
00334: 							if (!flag)
00335: 							{
00336: 								list.Add(pendingDeposit.Box);
00337: 							}
00338: 							if (pendingDeposit.Batch != 0)
00339: 							{
00340: 								dictionary[pendingDeposit.Batch] = (dictionary.TryGetValue(pendingDeposit.Batch, out var value) ? value : 0) + 1;
00341: 							}
00342: 						}
00343: 						else
00344: 						{
00345: 							Core.Debug("收纳箱 deferred：存入未执行（'" + SafeId(pendingDeposit.Item) + "' 走原生退回，物品应在鼠标原 hover 处）");
00346: 							if (pendingDeposit.Batch != 0)
00347: 							{
00348: 								dictionary2[pendingDeposit.Batch] = (dictionary2.TryGetValue(pendingDeposit.Batch, out var value2) ? value2 : 0) + 1;
00349: 							}
00350: 						}
00351: 					}
00352: 					catch (Exception ex)
00353: 					{
00354: 						Core.Log?.Warning("收纳箱 deferred deposit：" + ex.Message);
00355: 						if (pendingDeposit.Batch != 0)
00356: 						{
00357: 							dictionary2[pendingDeposit.Batch] = (dictionary2.TryGetValue(pendingDeposit.Batch, out var value3) ? value3 : 0) + 1;
00358: 						}
00359: 					}
00360: 				}
00361: 				foreach (KeyValuePair<int, int> item2 in dictionary)
00362: 				{
00363: 					if (!dictionary2.TryGetValue(item2.Key, out var value4))
00364: 					{
00365: 						value4 = 0;
00366: 					}
00367: 					if (item2.Value + value4 >= 2)
00368: 					{
00369: 						if (value4 > 0)
00370: 						{
00371: 							BoxItem.NotifyUser("已存入 " + item2.Value + " 件，" + value4 + " 件未能存入");
00372: 						}
00373: 						else
00374: 						{
00375: 							BoxItem.NotifyUser("已存入 " + item2.Value + " 件");
00376: 						}
00377: 					}
00378: 				}
00379: 				foreach (KeyValuePair<int, int> item3 in dictionary2)
00380: 				{
00381: 					if (!dictionary.ContainsKey(item3.Key) && item3.Value >= 2)
00382: 					{
00383: 						BoxItem.NotifyUser("未能存入 " + item3.Value + " 件");
00384: 					}
00385: 				}
00386: 			}
00387: 			finally
00388: 			{
00389: 				if (list != null)
00390: 				{
00391: 					foreach (GameItem item4 in list)
00392: 					{
00393: 						try
00394: 						{
00395: 							BoxWindow.RefreshButtons(item4);
00396: 						}
00397: 						catch
00398: 						{
00399: 						}
00400: 					}
00401: 				}
00402: 			}
00403: 		}
00404: 		catch (Exception ex2)
00405: 		{
00406: 			Core.Log?.Warning("ItemStorage TickDeferred：" + ex2.Message);
00407: 		}
00408: 		try
00409: 		{
00410: 			if (_pendingWithdraws.Count > 0)
00411: 			{
00412: 				PendingWithdraw pendingWithdraw = _pendingWithdraws.Dequeue();
00413: 				BoxTransfer.WithdrawResult withdrawResult = BoxTransfer.WithdrawResult.SilentFail;
00414: 				try
00415: 				{
00416: 					withdrawResult = BoxTransfer.TryWithdraw(pendingWithdraw.Box, pendingWithdraw.Slot, pendingWithdraw.Drop);
00417: 				}
00418: 				catch (Exception ex3)
00419: 				{
00420: 					Core.Log?.Warning("收纳箱 right-click withdraw：" + ex3.Message);
00421: 				}
00422: 				if (withdrawResult == BoxTransfer.WithdrawResult.Ok)
00423: 				{
00424: 					_wOk++;
00425: 					if (_pendingWithdraws.Count == 0)
00426: 					{
00427: 						_wOk = 0;
00428: 					}
00429: 				}
00430: 				else
00431: 				{
00432: 					int count = _pendingWithdraws.Count;
00433: 					_pendingWithdraws.Clear();
00434: 					if (withdrawResult == BoxTransfer.WithdrawResult.NoSpace || withdrawResult == BoxTransfer.WithdrawResult.NotOnGrid)
00435: 					{
00436: 						BoxItem.NotifyUser(BoxTransfer.SpaceShort(pendingWithdraw.Drop != null) + "，未能完全取出物品");
00437: 					}
00438: 					_wOk = 0;
00439: 					Core.Debug("收纳箱 right-click：取出中断（" + withdrawResult.ToString() + "），余 " + count + " 条作废");
00440: 				}
00441: 			}
00442: 			TrackRightClick();
00443: 		}
00444: 		catch (Exception ex4)
00445: 		{
00446: 			Core.Log?.Warning("ItemStorage 右键取出轮询：" + ex4.Message);
00447: 			_rightWasDown = false;
00448: 			_pendingWithdraws.Clear();
00449: 		}
00450: 	}
00451: 
00452: 	private static void TrackRightClick()
00453: 	{
00454: 		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
00455: 		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
00456: 		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
00457: 		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
00458: 		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
00459: 		bool flag = false;
00460: 		try
00461: 		{
00462: 			flag = Input.GetMouseButtonDown(1);
00463: 		}
00464: 		catch
00465: 		{
00466: 			return;
00467: 		}
00468: 		bool rightWasDown = _rightWasDown;
00469: 		_rightWasDown = flag;
00470: 		if (!flag || rightWasDown)
00471: 		{
00472: 			return;
00473: 		}
00474: 		try
00475: 		{
00476: 			Vector3 mousePosition = Input.mousePosition;
00477: 			if (!BoxWindow.TryHitSlot(new Vector2(mousePosition.x, mousePosition.y), out GameItem box, out int slot) || box == null)
00478: 			{
00479: 				return;
00480: 			}
00481: 			BoxItem.ClickSound();
00482: 			GameGridInventory val = (BoxTransfer.AltHeld() ? BoxTransfer.ResolveCounterDrop() : null);
00483: 			BoxTransfer.WithdrawResult withdrawResult = BoxTransfer.WithdrawResult.SilentFail;
00484: 			try
00485: 			{
00486: 				withdrawResult = BoxTransfer.TryWithdraw(box, slot, val);
00487: 			}
00488: 			catch (Exception ex)
00489: 			{
00490: 				Core.Log?.Warning("收纳箱 right-click 首件：" + ex.Message);
00491: 			}
00492: 			if (_pendingWithdraws.Count == 0)
00493: 			{
00494: 				_wOk = 0;
00495: 			}
00496: 			switch (withdrawResult)
00497: 			{
00498: 			case BoxTransfer.WithdrawResult.NoSpace:
00499: 			case BoxTransfer.WithdrawResult.NotOnGrid:
00500: 				BoxItem.NotifyUser(BoxTransfer.SpaceShort(val != null) + "，无法取出物品");
00501: 				break;
00502: 			case BoxTransfer.WithdrawResult.Ok:
00503: 			{
00504: 				_wOk++;
00505: 				for (int i = 1; i < 5; i++)
00506: 				{
00507: 					_pendingWithdraws.Enqueue(new PendingWithdraw
00508: 					{
00509: 						Box = box,
00510: 						Slot = slot,
00511: 						Drop = val
00512: 					});
00513: 				}
00514: 				Core.Debug("收纳箱 right-click：首件已取，槽 " + slot + " 余 4 件排队（本批已成功 " + _wOk + "）");
00515: 				break;
00516: 			}
00517: 			}
00518: 		}
00519: 		catch (Exception ex2)
00520: 		{
00521: 			Core.Log?.Warning("收纳箱 right-click 判定：" + ex2.Message);
00522: 		}
00523: 	}
00524: 
00525: 	internal static bool? TryRegisterClickDeposit(GameItem pack, int slot)
00526: 	{
00527: 		try
00528: 		{
00529: 			EnsureHandler();
00530: 			if ((Object)(object)_dragHandler == (Object)null)
00531: 			{
00532: 				return false;
00533: 			}
00534: 			if (_dragHandler.IsDraggingItem)
00535: 			{
00536: 				GameItem draggedItem = GetDraggedItem(_dragHandler);
00537: 				if (draggedItem == null)
00538: 				{
00539: 					Core.Debug("收纳箱 click deposit：拖拽中但取不到物品引用，忽略本次点击");
00540: 					return null;
00541: 				}
00542: 				if (BoxItem.IsBox(draggedItem))
00543: 				{
00544: 					Core.Debug("收纳箱 click deposit：手持容器，忽略本次点击（容器不进容器）");
00545: 					return null;
00546: 				}
00547: 				_pendingDeposits.Enqueue(new PendingDeposit
00548: 				{
00549: 					Box = pack,
00550: 					Item = draggedItem,
00551: 					Slot = slot
00552: 				});
00553: 				Core.Debug("收纳箱 click deposit：登记 '" + SafeId(draggedItem) + "' → 槽 " + slot + "（队列 " + _pendingDeposits.Count + "）");
00554: 				return true;
00555: 			}
00556: 			if (_wasDragging)
00557: 			{
00558: 				Core.Debug("收纳箱 click deposit：松手后立即点击，跳过防误取");
00559: 				return null;
00560: 			}
00561: 			return false;
00562: 		}
00563: 		catch (Exception ex)
00564: 		{
00565: 			Core.Log?.Warning("ItemStorage click deposit：" + ex.Message);
00566: 			return null;
00567: 		}
00568: 	}
00569: 
00570: 	private static void TrackDragSnapshot()
00571: 	{
00572: 		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
00573: 		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
00574: 		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
00575: 		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
00576: 		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
00577: 		try
00578: 		{
00579: 			EnsureHandler();
00580: 			if ((Object)(object)_dragHandler == (Object)null)
00581: 			{
00582: 				_wasDragging = false;
00583: 				return;
00584: 			}
00585: 			bool isDraggingItem = _dragHandler.IsDraggingItem;
00586: 			if (isDraggingItem && !_wasDragging)
00587: 			{
00588: 				_dragItem = GetDraggedItem(_dragHandler);
00589: 				BoxLabel.OnDragStart((IEnumerable<GameItem?>?)(object)new GameItem[1] { _dragItem });
00590: 				int num = -1;
00591: 				try
00592: 				{
00593: 					GameItem? dragItem = _dragItem;
00594: 					num = ((dragItem != null) ? dragItem.unitCount : (-1));
00595: 				}
00596: 				catch
00597: 				{
00598: 				}
00599: 				int? removeCount = GetRemoveCount(_dragHandler, "currentRemove");
00600: 				int? removeCount2 = GetRemoveCount(_dragHandler, "maxRemove");
00601: 				Core.Debug("拖拽诊断：拖起 '" + SafeId(_dragItem) + "' unitCount=" + num + " currentRemove=" + (removeCount?.ToString() ?? "?") + " maxRemove=" + (removeCount2?.ToString() ?? "?"));
00602: 			}
00603: 			else if (!isDraggingItem && _wasDragging)
00604: 			{
00605: 				GameItem val = GetDraggedItem(_dragHandler) ?? _dragItem;
00606: 				GameItem lastItem = GetLastItem(_dragHandler);
00607: 				Core.Debug("拖拽诊断：松手 currentItem='" + SafeId(val) + "' lastItem='" + SafeId(lastItem) + "'");
00608: 				try
00609: 				{
00610: 					if (val != null && !BoxItem.IsBox(val))
00611: 					{
00612: 						Vector3 mousePosition = Input.mousePosition;
00613: 						if (BoxWindow.TryHitSlot(new Vector2(mousePosition.x, mousePosition.y), out GameItem box, out int slot, verbose: true) && box != null)
00614: 						{
00615: 							_pendingDeposits.Enqueue(new PendingDeposit
00616: 							{
00617: 								Box = box,
00618: 								Item = val,
00619: 								Slot = slot
00620: 							});
00621: 							Core.Debug("收纳箱 drop：登记 '" + SafeId(val) + "' → 槽 " + slot + "（队列 " + _pendingDeposits.Count + "）");
00622: 						}
00623: 						else
00624: 						{
00625: 							Core.Debug("收纳箱 drop：未命中格子（'" + SafeId(val) + "' 走官方 EndDrag，lastItem 路径）");
00626: 						}
00627: 					}
00628: 				}
00629: 				catch (Exception ex)
00630: 				{
00631: 					Core.Log?.Warning("收纳箱 drop 判定：" + ex.Message);
00632: 				}
00633: 				BoxLabel.OnDragEnd();
00634: 				_dragItem = null;
00635: 			}
00636: 			_wasDragging = isDraggingItem;
00637: 		}
00638: 		catch (Exception ex2)
00639: 		{
00640: 			Core.Log?.Warning("ItemStorage 拖拽轮询：" + ex2.Message);
00641: 			_wasDragging = false;
00642: 			_dragItem = null;
00643: 		}
00644: 	}
00645: 
00646: 	private static void TrackGroupDrag()
00647: 	{
00648: 		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
00649: 		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
00650: 		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
00651: 		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
00652: 		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
00653: 		try
00654: 		{
00655: 			EnsureMultiHandler();
00656: 			if ((Object)(object)_multiHandler == (Object)null)
00657: 			{
00658: 				_groupWasActive = false;
00659: 				_groupSnapshot = null;
00660: 				return;
00661: 			}
00662: 			int num = 0;
00663: 			try
00664: 			{
00665: 				num = GetSelectedItems()?.Count ?? 0;
00666: 			}
00667: 			catch
00668: 			{
00669: 				num = 0;
00670: 			}
00671: 			if (num > 0 && !_groupWasActive)
00672: 			{
00673: 				_groupSnapshot = new List<GameItem>();
00674: 				try
00675: 				{
00676: 					List<GameItem> selectedItems = GetSelectedItems();
00677: 					if (selectedItems != null)
00678: 					{
00679: 						foreach (GameItem item in selectedItems)
00680: 						{
00681: 							try
00682: 							{
00683: 								if (item != null)
00684: 								{
00685: 									_groupSnapshot.Add(item);
00686: 								}
00687: 							}
00688: 							catch
00689: 							{
00690: 							}
00691: 						}
00692: 					}
00693: 				}
00694: 				catch
00695: 				{
00696: 				}
00697: 				_groupWasActive = true;
00698: 				_groupHoverItem = null;
00699: 				BoxLabel.OnDragStart(_groupSnapshot);
00700: 				Core.Debug("拖拽诊断：框选组成立，集合 " + num + " 件（快照 " + _groupSnapshot.Count + "）");
00701: 			}
00702: 			if (_groupWasActive && num > 0)
00703: 			{
00704: 				try
00705: 				{
00706: 					_groupHoverItem = GetGroupHoverItem();
00707: 				}
00708: 				catch
00709: 				{
00710: 				}
00711: 				if (num > (_groupSnapshot?.Count ?? 0))
00712: 				{
00713: 					List<GameItem> list = new List<GameItem>();
00714: 					try
00715: 					{
00716: 						List<GameItem> selectedItems2 = GetSelectedItems();
00717: 						if (selectedItems2 != null)
00718: 						{
00719: 							foreach (GameItem item2 in selectedItems2)
00720: 							{
~~~

~~~csharp
00737: 					if (list.Count > (_groupSnapshot?.Count ?? 0))
00738: 					{
00739: 						_groupSnapshot = list;
00740: 						Core.Debug("拖拽诊断：框选集合增长 → " + num + " 件（重拍快照 " + list.Count + "）");
00741: 					}
00742: 				}
00743: 			}
00744: 			if (num != 0 || !_groupWasActive)
00745: 			{
00746: 				return;
00747: 			}
00748: 			_groupWasActive = false;
00749: 			BoxLabel.OnDragEnd();
00750: 			List<GameItem> groupSnapshot = _groupSnapshot;
00751: 			_groupSnapshot = null;
00752: 			if (groupSnapshot == null || groupSnapshot.Count == 0)
00753: 			{
00754: 				return;
00755: 			}
00756: 			try
00757: 			{
00758: 				GameItem groupHoverItem = _groupHoverItem;
00759: 				if (groupHoverItem != null && BoxItem.IsBox(groupHoverItem))
00760: 				{
00761: 					List<GameItem> list2 = new List<GameItem>();
00762: 					foreach (GameItem item3 in groupSnapshot)
00763: 					{
00764: 						if (item3 != null && !BoxItem.IsBox(item3) && !IsLabeler(item3))
00765: 						{
00766: 							list2.Add(item3);
00767: 						}
00768: 					}
00769: 					if (list2.Count > 0)
00770: 					{
00771: 						int batch = ((list2.Count > 1) ? (++_batchSeq) : 0);
00772: 						foreach (GameItem item4 in list2)
00773: 						{
00774: 							_pendingDeposits.Enqueue(new PendingDeposit
00775: 							{
00776: 								Box = groupHoverItem,
00777: 								Item = item4,
00778: 								Slot = -1,
00779: 								Batch = batch
00780: 							});
00781: 						}
00782: 						Core.Debug("收纳箱 group drop：登记 " + list2.Count + " 件 → 盒（批次 " + batch + "，队列 " + _pendingDeposits.Count + "）");
00783: 					}
00784: 					_groupHoverItem = null;
00785: 					return;
00786: 				}
00787: 				Core.Debug("拖拽诊断：组下降沿 hover 缓存='" + SafeId(groupHoverItem) + "'，走鼠标格子判定");
00788: 			}
00789: 			catch
00790: 			{
00791: 			}
00792: 			try
00793: 			{
00794: 				Vector3 mousePosition = Input.mousePosition;
00795: 				if (BoxWindow.TryHitSlot(new Vector2(mousePosition.x, mousePosition.y), out GameItem box, out int slot) && box != null)
00796: 				{
00797: 					List<GameItem> list3 = new List<GameItem>();
00798: 					foreach (GameItem item5 in groupSnapshot)
00799: 					{
00800: 						if (item5 != null && !BoxItem.IsBox(item5))
00801: 						{
00802: 							list3.Add(item5);
00803: 						}
00804: 					}
00805: 					if (list3.Count > 0)
00806: 					{
00807: 						int batch2 = ((list3.Count > 1) ? (++_batchSeq) : 0);
00808: 						foreach (GameItem item6 in list3)
00809: 						{
00810: 							_pendingDeposits.Enqueue(new PendingDeposit
00811: 							{
00812: 								Box = box,
00813: 								Item = item6,
00814: 								Slot = slot,
00815: 								Batch = batch2
00816: 							});
00817: 						}
00818: 						Core.Debug("收纳箱 group drop：登记 " + list3.Count + " 件 → 槽 " + slot + "（批次 " + batch2 + "，队列 " + _pendingDeposits.Count + "）");
00819: 					}
00820: 				}
00821: 			}
00822: 			catch (Exception ex)
00823: 			{
00824: 				Core.Log?.Warning("收纳箱 group drop 判定：" + ex.Message);
00825: 			}
00826: 			_groupHoverItem = null;
00827: 		}
00828: 		catch (Exception ex2)
00829: 		{
00830: 			Core.Log?.Warning("ItemStorage 组拖拽轮询：" + ex2.Message);
00831: 			_groupWasActive = false;
00832: 			_groupSnapshot = null;
00833: 			_groupHoverItem = null;
00834: 		}
00835: 	}
00836: 
00837: 	private static void EnsureMultiHandler()
00838: 	{
00839: 		if (((Object)(object)_multiHandler != (Object)null && IsAliveMulti(_multiHandler)) || Time.time < _nextMultiFindAt)
00840: 		{
00841: 			return;
00842: 		}
00843: 		_nextMultiFindAt = Time.time + 2f;
00844: 		try
00845: 		{
00846: 			_multiHandler = Object.FindObjectOfType<ItemMultiSelectHandler>();
00847: 		}
00848: 		catch
00849: 		{
00850: 			_multiHandler = null;
00851: 		}
00852: 	}
00853: 
00854: 	private static bool IsAliveMulti(ItemMultiSelectHandler handler)
00855: 	{
00856: 		try
00857: 		{
00858: 			return (Object)(object)handler != (Object)null;
00859: 		}
00860: 		catch
00861: 		{
00862: 			return false;
00863: 		}
00864: 	}
00865: 
00866: 	private static void EnsureHandler()
00867: 	{
00868: 		if (((Object)(object)_dragHandler != (Object)null && IsAlive(_dragHandler)) || Time.time < _nextDragFindAt)
00869: 		{
00870: 			return;
00871: 		}
00872: 		_nextDragFindAt = Time.time + 2f;
00873: 		try
00874: 		{
00875: 			_dragHandler = Object.FindObjectOfType<ItemMouseDragHandler>();
00876: 		}
00877: 		catch
00878: 		{
00879: 			_dragHandler = null;
00880: 		}
00881: 	}
00882: 
00883: 	private static bool IsAlive(ItemMouseDragHandler handler)
00884: 	{
00885: 		try
00886: 		{
00887: 			return (Object)(object)handler != (Object)null;
00888: 		}
00889: 		catch
00890: 		{
00891: 			return false;
00892: 		}
00893: 	}
00894: 
00895: 	private static GameItem? GetDraggedItem(ItemMouseDragHandler? handler)
00896: 	{
00897: 		try
00898: 		{
00899: 			return (handler != null) ? handler.GetCurrentItem() : null;
00900: 		}
00901: 		catch (Exception ex)
00902: 		{
00903: 			Core.Debug("收纳箱 GetCurrentItem 读取失败：" + ex.Message);
00904: 			return null;
00905: 		}
00906: 	}
00907: 
00908: 	private static GameItem? GetLastItem(ItemMouseDragHandler? handler)
00909: 	{
00910: 		try
00911: 		{
00912: 			if ((Object)(object)handler == (Object)null)
00913: 			{
00914: 				return null;
00915: 			}
00916: 			if ((object)_lastItemField == null)
00917: 			{
00918: 				_lastItemField = AccessTools.Field(typeof(ItemMouseDragHandler), "lastItem");
00919: 			}
00920: 			object? obj = _lastItemField?.GetValue(handler);
00921: 			return (GameItem?)((obj is GameItem) ? obj : null);
00922: 		}
00923: 		catch (Exception ex)
00924: 		{
00925: 			Core.Debug("收纳箱 lastItem 读取失败：" + ex.Message);
00926: 			return null;
00927: 		}
00928: 	}
00929: 
00930: 	private static int? GetRemoveCount(ItemMouseDragHandler? handler, string name)
00931: 	{
00932: 		try
00933: 		{
00934: 			if ((Object)(object)handler == (Object)null)
00935: 			{
00936: 				return null;
00937: 			}
00938: 			FieldInfo fieldInfo = ((name == "currentRemove") ? (_currentRemoveField ?? (_currentRemoveField = AccessTools.Field(typeof(ItemMouseDragHandler), "currentRemove"))) : (_maxRemoveField ?? (_maxRemoveField = AccessTools.Field(typeof(ItemMouseDragHandler), "maxRemove"))));
00939: 			if (fieldInfo == null)
00940: 			{
00941: 				return null;
00942: 			}
00943: 			return Convert.ToInt32(fieldInfo.GetValue(handler));
00944: 		}
00945: 		catch
00946: 		{
00947: 			return null;
00948: 		}
00949: 	}
00950: 
00951: 	private static List<GameItem>? GetSelectedItems()
00952: 	{
00953: 		try
00954: 		{
00955: 			if ((Object)(object)_multiHandler == (Object)null)
00956: 			{
00957: 				return null;
00958: 			}
00959: 			if ((object)_selectedItemsField == null)
00960: 			{
00961: 				_selectedItemsField = AccessTools.Field(typeof(ItemMultiSelectHandler), "selectedItems");
00962: 			}
00963: 			if (!(_selectedItemsField?.GetValue(_multiHandler) is List<GameItemElement> list))
00964: 			{
00965: 				return null;
00966: 			}
00967: 			List<GameItem> list2 = new List<GameItem>(list.Count);
00968: 			foreach (GameItemElement item in list)
00969: 			{
00970: 				try
00971: 				{
00972: 					if (item != null)
00973: 					{
00974: 						list2.Add((GameItem)(object)item);
00975: 					}
00976: 				}
00977: 				catch
00978: 				{
00979: 				}
00980: 			}
00981: 			return list2;
00982: 		}
00983: 		catch
00984: 		{
00985: 			return null;
00986: 		}
00987: 	}
00988: 
00989: 	private static GameItem? GetGroupHoverItem()
00990: 	{
00991: 		try
00992: 		{
00993: 			if ((Object)(object)_multiHandler == (Object)null)
00994: 			{
00995: 				return null;
00996: 			}
00997: 			if ((object)_hoverItemField == null)
00998: 			{
00999: 				_hoverItemField = AccessTools.Field(typeof(ItemMultiSelectHandler), "hoverItem");
01000: 			}
01001: 			object? obj = _hoverItemField?.GetValue(_multiHandler);
01002: 			return (GameItem?)((obj is GameItem) ? obj : null);
01003: 		}
01004: 		catch
01005: 		{
01006: 			return null;
01007: 		}
01008: 	}
01009: 
01010: 	private static string SafeId(GameItem? item)
01011: 	{
01012: 		try
01013: 		{
01014: 			return Core.Clean((item != null) ? item.identifier : null);
~~~

### ItemStorage/TagSnapshot.cs

~~~csharp
00380: 				if (WaterFeatureHelper.GetWaterFeature(item) != null)
00381: 				{
00382: 					return null;
00383: 				}
00384: 			}
00385: 			catch
00386: 			{
00387: 			}
00388: 			try
00389: 			{
00390: 				if (StorageGrid.FindConditionFeature(item) != null)
00391: 				{
00392: 					return null;
00393: 				}
00394: 			}
00395: 			catch
00396: 			{
00397: 			}
00398: 			TagSystem state = item.state;
00399: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
00400: 			if (dictionary == null)
00401: 			{
00402: 				return null;
00403: 			}
00404: 			string itemId = Core.Clean(item.identifier);
00405: 			long num = 0L;
00406: 			try
00407: 			{
00408: 				num = item.unitValue;
00409: 			}
00410: 			catch
00411: 			{
00412: 			}
00413: 			string text = "";
00414: 			try
00415: 			{
00416: 				text = Core.Clean(item.spritePath) ?? "";
00417: 			}
00418: 			catch
00419: 			{
00420: 			}
00421: 			if (!HasIndividualState(item))
00422: 			{
00423: 				if (num == 0L)
00424: 				{
00425: 					Core.Debug("收纳箱快照：普通物品无个体状态，不拍 '" + (Core.Clean(item.identifier) ?? "?") + "'");
00426: 					return null;
00427: 				}
00428: 				Core.Debug("收纳箱快照：普通物品拍纯价快照 uv=" + num);
00429: 				return string.Empty + "\u00020\u00020\u00020\u00020\u00020\u00020\u00020\u0002\u0002" + num + "\u0002" + text;
00430: 			}
00431: 			StringBuilder stringBuilder = new StringBuilder();
00432: 			foreach (KeyValuePair<string, TagState> item2 in dictionary)
00433: 			{
00434: 				if (Excluded(item2.Key))
00435: 				{
00436: 					continue;
00437: 				}
00438: 				TagState value = item2.Value;
00439: 				if (value != null)
00440: 				{
00441: 					if (stringBuilder.Length > 0)
00442: 					{
00443: 						stringBuilder.Append('\u0003');
00444: 					}
00445: 					stringBuilder.Append(item2.Key).Append('\u0002').Append(value.IsEnabled() ? '1' : '0')
00446: 						.Append('\u0002')
00447: 						.Append(value.valueInt)
00448: 						.Append('\u0002')
00449: 						.Append(value.valueFloat.ToString("R", CultureInfo.InvariantCulture))
00450: 						.Append('\u0002')
00451: 						.Append(value.valueLong)
00452: 						.Append('\u0002')
00453: 						.Append(value.valueDouble.ToString("R", CultureInfo.InvariantCulture))
00454: 						.Append('\u0002')
00455: 						.Append(value.valueBool ? '1' : '0')
00456: 						.Append('\u0002')
00457: 						.Append(value.valueString ?? "")
00458: 						.Append('\u0002')
00459: 						.Append(num)
00460: 						.Append('\u0002')
00461: 						.Append(text);
00462: 				}
00463: 			}
00464: 			try
00465: 			{
00466: 				List<string> gameItemType = item.GetGameItemType();
00467: 				if (!SameTypes(gameItemType, GetTemplateItemTypes(itemId)))
00468: 				{
00469: 					if (stringBuilder.Length > 0)
00470: 					{
00471: 						stringBuilder.Append('\u0003');
00472: 					}
00473: 					stringBuilder.Append("__ITEM_TYPES__").Append('\u0002').Append(string.Join(",", gameItemType ?? new List<string>()));
00474: 				}
00475: 			}
00476: 			catch
00477: 			{
00478: 			}
00479: 			if (stringBuilder.Length == 0)
00480: 			{
00481: 				if (num == 0L)
00482: 				{
00483: 					return null;
00484: 				}
00485: 				stringBuilder.Append('\u0002').Append('0').Append('\u0002')
00486: 					.Append('0')
00487: 					.Append('\u0002')
00488: 					.Append('0')
00489: 					.Append('\u0002')
00490: 					.Append('0')
00491: 					.Append('\u0002')
00492: 					.Append('0')
00493: 					.Append('\u0002')
00494: 					.Append('0')
00495: 					.Append('\u0002')
00496: 					.Append('0')
00497: 					.Append('\u0002')
00498: 					.Append("")
00499: 					.Append('\u0002')
00500: 					.Append(num)
00501: 					.Append('\u0002')
00502: 					.Append(text);
00503: 			}
00504: 			int num2 = 0;
00505: 			for (int i = 0; i < stringBuilder.Length; i++)
00506: 			{
00507: 				if (stringBuilder[i] == '\u0003')
00508: 				{
00509: 					num2++;
00510: 				}
00511: 			}
00512: 			Core.Debug("收纳箱快照：Capture 产出 1 只（tag " + (num2 + 1) + " 个）uv=" + num);
00513: 			return stringBuilder.ToString();
00514: 		}
00515: 		catch (Exception ex)
00516: 		{
00517: 			Core.Debug("收纳箱 快照拍取失败：" + ex.Message);
00518: 			return null;
00519: 		}
00520: 	}
00521: 
00522: 	private static bool RestoreOne(string itemSnap, GameItem item)
00523: 	{
00524: 		bool flag = false;
00525: 		long num = -1L;
00526: 		try
00527: 		{
00528: 			string[] array = itemSnap.Split('\u0003');
00529: 			foreach (string text in array)
00530: 			{
00531: 				try
00532: 				{
00533: 					string[] array2 = text.Split('\u0002');
00534: 					if (array2.Length >= 2 && array2[0] == "__ITEM_TYPES__")
00535: 					{
00536: 						try
00537: 						{
00538: 							item.RemoveAllGameItemType();
00539: 							string text2 = array2[1];
00540: 							if (!string.IsNullOrEmpty(text2))
00541: 							{
00542: 								string[] array3 = text2.Split(',');
00543: 								foreach (string text3 in array3)
00544: 								{
00545: 									if (NullGuard.HasText(text3))
00546: 									{
00547: 										item.SetGameItemType(text3);
00548: 									}
00549: 								}
00550: 							}
00551: 							flag = true;
00552: 						}
00553: 						catch
00554: 						{
00555: 						}
00556: 					}
00557: 					else
00558: 					{
00559: 						if (array2.Length < 9)
00560: 						{
00561: 							continue;
00562: 						}
00563: 						string text4 = array2[0];
00564: 						if (Excluded(text4))
00565: 						{
00566: 							continue;
00567: 						}
00568: 						if (string.IsNullOrEmpty(text4))
00569: 						{
00570: 							goto IL_0199;
00571: 						}
00572: 						bool enabled = array2[1] == "1";
00573: 						TagSystem state = item.state;
00574: 						TagState val = ((state != null) ? state.GetTag(text4) : null);
00575: 						if (val != null)
00576: 						{
00577: 							try
00578: 							{
00579: 								val.SetEnabled(enabled);
00580: 							}
00581: 							catch
00582: 							{
00583: 							}
00584: 							try
00585: 							{
00586: 								val.valueInt = int.Parse(array2[2], CultureInfo.InvariantCulture);
00587: 							}
00588: 							catch
00589: 							{
00590: 							}
00591: 							try
00592: 							{
00593: 								val.valueFloat = float.Parse(array2[3], CultureInfo.InvariantCulture);
00594: 							}
00595: 							catch
00596: 							{
00597: 							}
00598: 							try
00599: 							{
00600: 								val.valueLong = long.Parse(array2[4], CultureInfo.InvariantCulture);
00601: 							}
00602: 							catch
00603: 							{
00604: 							}
00605: 							try
00606: 							{
~~~

~~~csharp
00645: 			}
00646: 			if (flag && num >= 0)
00647: 			{
00648: 				try
00649: 				{
00650: 					item.unitValue = num;
00651: 				}
00652: 				catch
00653: 				{
00654: 				}
00655: 			}
00656: 			return flag;
00657: 		}
00658: 		catch
00659: 		{
00660: 			return false;
00661: 		}
00662: 	}
00663: 
00664: 	internal static string? PopAndRestore(GameItem pack, int slot, GameItem item)
00665: 	{
00666: 		string snapshotQueue = StorageGrid.GetSnapshotQueue(pack, slot);
00667: 		if (!NullGuard.HasText(snapshotQueue))
00668: 		{
00669: 			return null;
00670: 		}
00671: 		string text = snapshotQueue;
00672: 		bool flag = false;
00673: 		while (!string.IsNullOrEmpty(text))
00674: 		{
00675: 			int num = text.LastIndexOf('\u0001');
00676: 			string text2;
00677: 			if (num >= 0)
00678: 			{
00679: 				text2 = text.Substring(num + 1);
00680: 				text = text.Substring(0, num);
00681: 			}
00682: 			else
00683: 			{
00684: 				text2 = text;
00685: 				text = "";
00686: 			}
00687: 			if (RestoreOne(text2, item))
00688: 			{
00689: 				flag = true;
00690: 				Core.Debug("收纳箱快照：还原 1 只（tag " + text2.Split('\u0003').Length + " 个）");
00691: 				break;
00692: 			}
00693: 			Core.Log?.Warning("收纳箱 快照坏条目已跳过（key='" + ((text2.Length > 24) ? text2.Substring(0, 24) : text2) + "…'）");
00694: 		}
00695: 		if (flag)
00696: 		{
00697: 			try
00698: 			{
00699: 				item.ValidateShapeState();
00700: 			}
00701: 			catch
00702: 			{
00703: 			}
00704: 		}
00705: 		return text;
00706: 	}
00707: 
00708: 	internal static void Enqueue(GameItem pack, int slot, string snap)
00709: 	{
00710: 		try
00711: 		{
00712: 			if (string.IsNullOrEmpty(snap))
00713: 			{
00714: 				return;
00715: 			}
00716: 			string snapshotQueue = StorageGrid.GetSnapshotQueue(pack, slot);
00717: 			string text = (string.IsNullOrEmpty(snapshotQueue) ? snap : (snapshotQueue + "\u0001" + snap));
00718: 			int num = 1;
00719: 			for (int i = 0; i < text.Length; i++)
00720: 			{
00721: 				if (text[i] == '\u0001')
00722: 				{
00723: 					num++;
00724: 				}
00725: 			}
00726: 			if (num > 2000)
00727: 			{
00728: 				Core.Log?.Warning("收纳箱 快照队列过大：槽 " + slot + " 已 " + num + " 条（软上限 " + 2000 + "，观察哨）");
00729: 			}
00730: 			StorageGrid.SetSnapshotQueue(pack, slot, text);
00731: 		}
00732: 		catch (Exception ex)
00733: 		{
00734: 			Core.Debug("收纳箱 快照入队失败：" + ex.Message);
00735: 		}
00736: 	}
00737: 
00738: 	internal static void SetQueue(GameItem pack, int slot, string? queue)
00739: 	{
00740: 		try
00741: 		{
00742: 			StorageGrid.SetSnapshotQueue(pack, slot, queue);
00743: 		}
00744: 		catch (Exception ex)
00745: 		{
00746: 			Core.Debug("收纳箱 快照写回失败：" + ex.Message);
00747: 		}
00748: 	}
00749: 
00750: 	internal static string? CaptureChemParts(GameItem item)
00751: 	{
00752: 		try
00753: 		{
00754: 			if (StorageGrid.FindConditionFeature(item) == null)
00755: 			{
00756: 				return null;
00757: 			}
00758: 			TagSystem state = item.state;
00759: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
00760: 			if (dictionary == null)
00761: 			{
00762: 				return null;
00763: 			}
00764: 			long value = 0L;
00765: 			try
00766: 			{
00767: 				value = item.unitValue;
00768: 			}
00769: 			catch
00770: 			{
00771: 			}
00772: 			StringBuilder stringBuilder = new StringBuilder();
00773: 			foreach (KeyValuePair<string, TagState> item2 in dictionary)
00774: 			{
00775: 				if (item2.Key == null || !item2.Key.StartsWith("CURRENT_PART_", StringComparison.Ordinal))
00776: 				{
00777: 					continue;
00778: 				}
00779: 				TagState value2 = item2.Value;
00780: 				if (value2 != null)
00781: 				{
00782: 					if (stringBuilder.Length > 0)
00783: 					{
00784: 						stringBuilder.Append('\u0003');
00785: 					}
00786: 					stringBuilder.Append(item2.Key).Append('\u0002').Append(value2.IsEnabled() ? '1' : '0')
00787: 						.Append('\u0002')
00788: 						.Append(value2.valueInt)
00789: 						.Append('\u0002')
00790: 						.Append(value2.valueFloat.ToString("R", CultureInfo.InvariantCulture))
00791: 						.Append('\u0002')
00792: 						.Append(value2.valueLong)
00793: 						.Append('\u0002')
00794: 						.Append(value2.valueDouble.ToString("R", CultureInfo.InvariantCulture))
00795: 						.Append('\u0002')
00796: 						.Append(value2.valueBool ? '1' : '0')
00797: 						.Append('\u0002')
00798: 						.Append(value2.valueString ?? "")
00799: 						.Append('\u0002')
00800: 						.Append(value)
00801: 						.Append('\u0002')
00802: 						.Append("");
00803: 				}
00804: 			}
00805: 			return (stringBuilder.Length == 0) ? null : stringBuilder.ToString();
00806: 		}
00807: 		catch
00808: 		{
00809: 			return null;
00810: 		}
00811: 	}
00812: 
00813: 	internal static bool EnqueueChemParts(GameItem pack, int slot, GameItem item, int put)
00814: 	{
00815: 		try
00816: 		{
00817: 			string text = CaptureChemParts(item);
00818: 			if (text == null)
00819: 			{
00820: 				return false;
00821: 			}
00822: 			string snapshotQueue = StorageGrid.GetSnapshotQueue(pack, slot);
00823: 			StringBuilder stringBuilder = new StringBuilder(string.IsNullOrEmpty(snapshotQueue) ? "" : snapshotQueue);
00824: 			for (int i = 0; i < put; i++)
00825: 			{
00826: 				if (stringBuilder.Length > 0)
00827: 				{
00828: 					stringBuilder.Append('\u0001');
00829: 				}
00830: 				stringBuilder.Append(text);
00831: 			}
00832: 			StorageGrid.SetSnapshotQueue(pack, slot, stringBuilder.ToString());
00833: 			Core.Debug("收纳箱 deposit 组成入队：槽 " + slot + " '" + (Core.Clean(item.identifier) ?? "?") + "' x" + put + "（组成键 " + text.Split('\u0003').Length + " 项/条）");
00834: 			return true;
00835: 		}
00836: 		catch (Exception ex)
00837: 		{
00838: 			Core.Debug("收纳箱 组成入队失败：" + ex.Message);
00839: 			return false;
00840: 		}
00841: 	}
00842: 
00843: 	internal static string? PeekLastSprite(GameItem pack, int slot)
00844: 	{
00845: 		try
00846: 		{
00847: 			string snapshotQueue = StorageGrid.GetSnapshotQueue(pack, slot);
00848: 			if (!NullGuard.HasText(snapshotQueue))
00849: 			{
00850: 				return null;
00851: 			}
00852: 			string text = snapshotQueue;
00853: 			int num = snapshotQueue.LastIndexOf('\u0001');
00854: 			if (num >= 0)
00855: 			{
00856: 				text = snapshotQueue.Substring(num + 1);
00857: 			}
00858: 			string[] array = text.Split('\u0003');
00859: 			for (int i = 0; i < array.Length; i++)
00860: 			{
00861: 				string[] array2 = array[i].Split('\u0002');
00862: 				if (array2.Length >= 10 && !string.IsNullOrEmpty(array2[^1]))
00863: 				{
00864: 					return array2[^1];
00865: 				}
00866: 			}
00867: 		}
00868: 		catch
00869: 		{
00870: 		}
00871: 		return null;
00872: 	}
00873: 
00874: 	internal static (string speciesId, string sexe, int age, float weight)? PeekLastAnimal(GameItem pack, int slot)
00875: 	{
00876: 		try
00877: 		{
00878: 			string snapshotQueue = StorageGrid.GetSnapshotQueue(pack, slot);
00879: 			if (!NullGuard.HasText(snapshotQueue))
00880: 			{
00881: 				return null;
00882: 			}
00883: 			string text = snapshotQueue;
00884: 			int num = snapshotQueue.LastIndexOf('\u0001');
00885: 			if (num >= 0)
00886: 			{
00887: 				text = snapshotQueue.Substring(num + 1);
00888: 			}
00889: 			string text2 = null;
00890: 			string text3 = null;
00891: 			int result = -1;
00892: 			float result2 = -1f;
00893: 			string[] array = text.Split('\u0003');
00894: 			for (int i = 0; i < array.Length; i++)
00895: 			{
00896: 				string[] array2 = array[i].Split('\u0002');
00897: 				if (array2.Length >= 8)
00898: 				{
00899: 					switch (array2[0])
00900: 					{
00901: 					case "ANIMAL_SPECIES_TAG":
00902: 						text2 = array2[7];
00903: 						break;
00904: 					case "ANIMAL_SEXE_TAG":
00905: 						text3 = array2[7];
00906: 						break;
00907: 					case "ANIMAL_AGE_TAG":
00908: 						int.TryParse(array2[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
00909: 						break;
00910: 					case "ANIMAL_WEIGHT_TAG":
00911: 						float.TryParse(array2[3], NumberStyles.Float, CultureInfo.InvariantCulture, out result2);
00912: 						break;
00913: 					}
00914: 				}
00915: 			}
00916: 			if (!NullGuard.HasText(text2))
00917: 			{
00918: 				return null;
00919: 			}
00920: 			return (text2, text3 ?? "", (result >= 0) ? result : 0, (result2 < 0f) ? 0f : result2);
00921: 		}
00922: 		catch
00923: 		{
00924: 			return null;
00925: 		}
00926: 	}
00927: 
~~~

### ItemStorage/WelcomeGift.cs

~~~csharp
00082: 			}
00083: 			BoxItem.WriteNumber(val2, 0);
00084: 			if (BoxItem.ReadNumber(val2) != 0)
00085: 			{
00086: 				try
00087: 				{
00088: 					val2.Destroy();
00089: 					return;
00090: 				}
00091: 				catch
00092: 				{
00093: 					return;
00094: 				}
00095: 			}
00096: 			try
00097: 			{
00098: 				GeneralHelper.SetItemOwned(val2, true);
00099: 			}
00100: 			catch
00101: 			{
00102: 			}
00103: 			bool flag = TryPlaceInto(val, val2);
00104: 			bool flag2 = false;
00105: 			if (!flag)
00106: 			{
00107: 				EmporiumEntry instance3 = EmporiumEntry.Instance;
00108: 				flag = TryPlaceInto((instance3 != null) ? instance3.backInvinvElement : null, val2);
00109: 				flag2 = flag;
00110: 			}
00111: 			if (!flag)
00112: 			{
00113: 				try
00114: 				{
00115: 					val2.Destroy();
00116: 				}
00117: 				catch
00118: 				{
00119: 				}
00120: 				_pending = false;
00121: 				Core.Log?.Msg("开局礼：桌面与柜台都放不下，本次未发放（水位未推进，下次进店铺重试）。");
00122: 			}
00123: 			else
00124: 			{
00125: 				PerSaveState.SetInt(instance, "BoxNumberWatermark", 1);
00126: 				_issued = true;
00127: 				Core.Log?.Msg(flag2 ? "开局礼：已发放 0 号收纳箱 ×1（桌面放不下，已放柜台 backInvinvElement，玩家所有）。本存档不再发放。" : "开局礼：已发放 0 号收纳箱 ×1（玩家桌面 24×10，玩家所有）。本存档不再发放。");
00128: 				EnsureNumbersForOwned(boxes);
00129: 			}
00130: 		}
00131: 		catch (Exception ex2)
00132: 		{
00133: 			Core.Debug("开局礼发放失败：" + ex2.Message);
00134: 		}
00135: 	}
00136: 
00137: 	private static bool TryPlaceInto(GameGridInventory? grid, GameItem box)
00138: 	{
00139: 		if (grid == null || box == null)
00140: 		{
00141: 			return false;
00142: 		}
00143: 		try
00144: 		{
00145: 			if (grid != null)
00146: 			{
00147: 				GraphUtils.TryAcceptAll((GraphNodeStorage)(object)grid, box, -1);
00148: 			}
00149: 			else
00150: 			{
00151: 				GraphUtils.TryAcceptAllMid(grid, box, -1);
00152: 			}
00153: 		}
00154: 		catch (Exception ex)
00155: 		{
00156: 			Core.Debug("开局礼放置异常：" + ex.Message);
00157: 		}
00158: 		try
00159: 		{
00160: 			GameInventory parentInventory = box.parentInventory;
00161: 			return (object)((parentInventory is GameGridInventory) ? parentInventory : null) == grid;
00162: 		}
00163: 		catch
00164: 		{
00165: 			return false;
00166: 		}
00167: 	}
00168: 
00169: 	private static void EnsureNumbersForOwned(List<GameItem>? boxes = null)
00170: 	{
00171: 		try
00172: 		{
00173: 			if (boxes == null)
00174: 			{
00175: 				boxes = BoxItem.FindAllBoxes();
00176: 			}
00177: 			foreach (GameItem box in boxes)
00178: 			{
00179: 				if (BoxItem.ReadNumber(box) == -1)
00180: 				{
00181: 					bool flag = true;
00182: 					try
00183: 					{
00184: 						flag = GeneralHelper.IsItemOwned(box);
00185: 					}
00186: 					catch
00187: 					{
00188: 					}
00189: 					if (flag)
00190: 					{
00191: 						BoxItem.WriteNumber(box, BoxItem.NextNumber());
00192: 						Core.Log?.Msg("编号：玩家拥有的无编号箱已补号 → " + BoxItem.ReadNumber(box) + " 号。");
00193: 					}
00194: 				}
00195: 			}
00196: 		}
00197: 		catch (Exception ex)
00198: 		{
00199: 			Core.Debug("补号巡检失败：" + ex.Message);
00200: 		}
00201: 	}
00202: }
~~~

## tentacle

### MidnightPawn/Game/ItemFactory.cs

~~~csharp
00068: 			Enable(val, "MAIDEN_VIRGIN");
00069: 			Enable(val, "MAIDEN_ANAL_VIRGIN");
00070: 		}
00071: 		if (text5 == "少女" || text5 == "萝莉")
00072: 		{
00073: 			Enable(val, "MAIDEN_YOUNG");
00074: 		}
00075: 		if (text4 == "巨乳")
00076: 		{
00077: 			Enable(val, "MAIDEN_BIG_BREAST");
00078: 		}
00079: 		Enable(val, "FACTION_LOWER_LEVEL");
00080: 		Enable(val, "JOB_SCAVENGER");
00081: 		SetInt(val, "TRAIN_VALUE", 0);
00082: 		SetInt(val, "ORAL_COUNT", 0);
00083: 		SetInt(val, "SEX_COUNT", 0);
00084: 		SetInt(val, "ANAL_COUNT", 0);
00085: 		SetInt(val, "LACTATION_LV", 0);
00086: 		string text6 = SpriteInjector.PickAnyPortraitSource();
00087: 		string text7 = SpriteInjector.InjectPortraitTrimmed(text6, 64, 64);
00088: 		if (text7 != null)
00089: 		{
00090: 			val.SetSpriteAndShapeFromMod("lewd_meme", text7);
00091: 			SetStr(val, "MAIDEN_SPRITE_SRC", text6);
00092: 			ModCtx.Log("少女立绘注入: " + text6 + " -> " + text7);
00093: 			SpriteInjector.ExportInjected(text7, "portrait_" + text6 + ".png");
00094: 		}
00095: 		else
00096: 		{
00097: 			ModCtx.LogWarning("少女立绘注入失败");
00098: 			val.SetShape(RectShape(2, 5));
00099: 		}
00100: 		((GameItemFunc)val).onLoaded = delegate(GameItem it)
00101: 		{
00102: 			SpriteInjector.EnsureMaidenSprite(it);
00103: 		};
00104: 		UpdateMaidenDescription(val);
00105: 		LogShape(val, "少女");
00106: 		return val;
00107: 	}
00108: 
00109: 	private static GameItem CreateMilk()
00110: 	{
00111: 		GameItem val = ItemDirectory.CreateEmptyItem("wondrous_milk");
00112: 		val.name = "神奇乳汁";
00113: 		val.shortDescription = "从苗床身上挤出来的乳白液体。入口甘甜，后劲是热的。";
00114: 		val.flavorText = "上层区的太太们为了它愿意多付三成。";
00115: 		val.unitValue = 180L;
00116: 		val.SetGameItemType("LUXURY_ITEM");
00117: 		val.SetGameItemType("BREVAGE");
00118: 		val.SetGameItemType("TREAT");
00119: 		val.SetGameItemType("FOOD");
00120: 		GameItem val2 = SpriteInjector.FindVanillaItem("juice");
00121: 		if (val2 != null)
00122: 		{
00123: 			val.SetSprite(val2.spriteAtlasPath, val2.spritePath);
00124: 			if (val2.shape != null)
00125: 			{
00126: 				val.SetShape(val2.shape.Clone());
00127: 			}
00128: 			ModCtx.Log("乳汁外观克隆自 " + val2.identifier);
00129: 		}
00130: 		else
00131: 		{
00132: 			ModCtx.LogWarning("没找到果汁物品，乳汁使用占位");
00133: 			val.SetShape(RectShape(2, 3));
00134: 		}
00135: 		((GameItemFunc)val).canActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
00136: 		((GameItemFunc)val).mayActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
00137: 		((GameItemFunc)val).onActivateSlotItemFunc = delegate(GameItem it, GameInventory inv, SlotMarker slot)
00138: 		{
00139: 			ModHelper.AddCredit(30, true);
00140: 			ModCtx.Log("饮下一瓶神奇乳汁");
00141: 			it.Destroy();
00142: 		};
00143: 		LogShape(val, "乳汁");
00144: 		return val;
00145: 	}
00146: 
00147: 	private static GameItem CreateSemen()
00148: 	{
00149: 		GameItem val = ItemDirectory.CreateEmptyItem("tentacle_semen");
00150: 		val.name = "触手精液";
00151: 		val.shortDescription = "黏糊糊的浊白精液混杂了些许的女性爱液，充满了触手的信息素，对女性充满了吸引力，但是男性闻起来却是作呕的腥臭味。";
00152: 		val.flavorText = "别问里面有什么，问就是模因。";
00153: 		val.unitValue = 90L;
00154: 		val.SetGameItemType("CONTRABAND");
00155: 		val.SetGameItemType("LUXURY_ITEM");
00156: 		if (SpriteInjector.TryFindVanillaSprite(out var atlasPath, out var spriteName, "pheromone", "perfume", "scent"))
00157: 		{
00158: 			string text = SpriteInjector.InjectResizedSprite(SpriteInjector.FindExact(atlasPath, spriteName), 32, 32, "semen_icon");
00159: 			if (text != null)
00160: 			{
00161: 				val.SetSpriteAndShapeFromMod("lewd_meme", text);
00162: 				ModCtx.Log("精液图标: " + atlasPath + "/" + spriteName + " -> " + text);
00163: 			}
00164: 			else
00165: 			{
00166: 				val.SetSprite(atlasPath, spriteName);
00167: 				val.SetShape(RectShape(2, 2));
00168: 			}
00169: 		}
00170: 		else
00171: 		{
00172: 			ModCtx.LogWarning("没找到香水图标，精液使用占位");
00173: 			val.SetShape(RectShape(2, 2));
00174: 		}
00175: 		((GameItemFunc)val).canActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
00176: 		((GameItemFunc)val).mayActivateSlotItemFunc = delegate
00177: 		{
00178: 			ModCtx.Log("精液: mayActivate 被调用 -> true");
00179: 			return true;
00180: 		};
00181: 		((GameItemFunc)val).onActivateSlotItemFunc = delegate(GameItem it, GameInventory inv, SlotMarker slot)
00182: 		{
00183: 			ModCtx.Log("精液: 激活生效");
00184: 			if (TentacleEffects.TryApplySemen(it))
00185: 			{
00186: 				it.Destroy();
00187: 			}
00188: 		};
00189: 		LogShape(val, "精液");
00190: 		return val;
00191: 	}
00192: 
00193: 	private static GameItem CreateOvulationShot()
00194: 	{
00195: 		GameItem val = ItemDirectory.CreateEmptyItem("ovulation_shot");
00196: 		val.name = "排卵诱发剂";
00197: 		val.shortDescription = "一小管从触手腺体里抽出来的透明液体。注入后，她的身体会立刻开始排卵。";
00198: 		val.flavorText = "医学上它不存在。";
00199: 		val.unitValue = 320L;
00200: 		val.SetGameItemType("CONTRABAND");
00201: 		val.SetGameItemType("MEDICAL");
00202: 		val.SetGameItemType("NARCOTIC");
~~~

### MidnightPawn/Game/MaidenTooltip.cs

~~~csharp
00001: using System;
00002: using System.Text;
00003: 
00004: namespace MidnightPawn.Game;
00005: 
00006: public static class MaidenTooltip
00007: {
00008: 	public static void Register()
00009: 	{
00010: 		ModHook.OnCreateTooltipLate += OnTooltip;
00011: 	}
00012: 
00013: 	public static void Unregister()
00014: 	{
00015: 		ModHook.OnCreateTooltipLate -= OnTooltip;
00016: 	}
00017: 
00018: 	private static void OnTooltip(RichTextBuilder builder, GameItem item)
00019: 	{
00020: 		if (builder == null || item == null)
00021: 		{
00022: 			return;
00023: 		}
00024: 		if (item.IsTag("TENTACLE_BODY_TAG"))
00025: 		{
00026: 			try
00027: 			{
00028: 				int bodyLevel = TentacleBody.GetBodyLevel();
00029: 				int bodyExp = TentacleBody.GetBodyExp();
00030: 				int expNeed = TentacleBody.GetExpNeed();
00031: 				builder.AddEmptyLine(2);
00032: 				builder.AddLine($"触手本体 Lv.{bodyLevel}   经验 {bodyExp} / {expNeed}", false, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00033: 				builder.AddLine("携带它去垃圾场拾荒，有概率捕获少女。", false, (ColorPalette)11567781, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00034: 				string modData = ModHelper.GetModData("lewd_meme", "last_report", "");
00035: 				if (!string.IsNullOrEmpty(modData))
00036: 				{
00037: 					builder.AddLine("昨晚：" + modData, false, (ColorPalette)8035658, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00038: 				}
00039: 				return;
00040: 			}
00041: 			catch (Exception ex)
00042: 			{
00043: 				ModCtx.LogWarning("触手本体 tooltip 失败: " + ex.Message);
00044: 				return;
00045: 			}
00046: 		}
00047: 		if (!item.IsTag("MAIDEN_FEMALE"))
00048: 		{
00049: 			return;
00050: 		}
00051: 		try
00052: 		{
00053: 			string str = ItemFactory.GetStr(item, "MAIDEN_NAME");
00054: 			string str2 = ItemFactory.GetStr(item, "MAIDEN_FACTION");
00055: 			string str3 = ItemFactory.GetStr(item, "MAIDEN_JOB");
00056: 			string str4 = ItemFactory.GetStr(item, "MAIDEN_BREAST");
00057: 			string str5 = ItemFactory.GetStr(item, "MAIDEN_BODY");
00058: 			builder.AddEmptyLine(2);
00059: 			if (!string.IsNullOrEmpty(str))
00060: 			{
00061: 				builder.AddLine("姓名：" + str, false, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00062: 			}
00063: 			StringBuilder stringBuilder = new StringBuilder();
00064: 			stringBuilder.Append("【高级违禁品】【女性角色】");
00065: 			if (!string.IsNullOrEmpty(str2))
00066: 			{
00067: 				stringBuilder.Append("【").Append(str2).Append("】");
00068: 			}
00069: 			if (!string.IsNullOrEmpty(str3))
00070: 			{
00071: 				stringBuilder.Append("【").Append(str3).Append("】");
00072: 			}
00073: 			if (!string.IsNullOrEmpty(str5))
00074: 			{
00075: 				stringBuilder.Append("【").Append(str5).Append("】");
00076: 			}
00077: 			if (!string.IsNullOrEmpty(str4))
00078: 			{
00079: 				stringBuilder.Append("【").Append(str4).Append("】");
00080: 			}
00081: 			if (item.IsTag("MAIDEN_VIRGIN"))
00082: 			{
00083: 				stringBuilder.Append("【处女】");
00084: 			}
00085: 			if (item.IsTag("MAIDEN_VIRGIN_LOST"))
00086: 			{
00087: 				stringBuilder.Append("【处女丧失】");
00088: 			}
00089: 			if (item.IsTag("MAIDEN_ANAL_VIRGIN"))
00090: 			{
00091: 				stringBuilder.Append("【肛门处女】");
00092: 			}
00093: 			if (item.IsTag("MAIDEN_ANAL_VIRGIN_LOST"))
00094: 			{
00095: 				stringBuilder.Append("【肛门处女丧失】");
00096: 			}
00097: 			builder.AddLine(stringBuilder.ToString(), false, (ColorPalette)11567781, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00098: 			int num = ItemFactory.GetInt(item, "TRAIN_VALUE");
00099: 			int num2 = ItemFactory.GetInt(item, "LACTATION_LV");
00100: 			int num3 = ItemFactory.GetInt(item, "ORAL_COUNT");
00101: 			int num4 = ItemFactory.GetInt(item, "SEX_COUNT");
00102: 			int num5 = ItemFactory.GetInt(item, "ANAL_COUNT");
00103: 			builder.AddLine($"调教值 {num}  泌乳等级 {num2}", false, (ColorPalette)12615037, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00104: 			builder.AddLine($"口交 {num3} 次 · 性交 {num4} 次 · 肛交 {num5} 次", false, (ColorPalette)12092762, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00105: 			if (item.IsTag("MAIDEN_OVULATING"))
00106: 			{
00107: 				builder.AddLine("【排卵期】", false, (ColorPalette)8035658, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00108: 			}
00109: 			if (item.IsTag("MAIDEN_PREGNANT"))
00110: 			{
00111: 				int num6 = ItemFactory.GetInt(item, "MAIDEN_PREGNANT_DAYS");
00112: 				builder.AddLine($"【已怀孕】第 {num6}/30 天", false, (ColorPalette)12615037, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00113: 			}
00114: 		}
00115: 		catch (Exception ex2)
00116: 		{
00117: 			ModCtx.LogWarning("少女 tooltip 追加失败: " + ex2.Message);
00118: 		}
00119: 	}
00120: }
~~~

### MidnightPawn/Game/TentacleBody.cs

~~~csharp
00095: 					num += ((GameInventory)list[j]).childItems.Count;
00096: 				}
00097: 			}
00098: 		}
00099: 		if (num > 0)
00100: 		{
00101: 			AddBodyExp(num);
00102: 		}
00103: 		ShowReport();
00104: 	}
00105: 
00106: 	private static void CollectOutputs()
00107: 	{
00108: 		int num = 0;
00109: 		GameItem[] array = Instances.ToArray();
00110: 		foreach (GameItem val in array)
00111: 		{
00112: 			if (val == null)
00113: 			{
00114: 				continue;
00115: 			}
00116: 			try
00117: 			{
00118: 				List<GameGridInventory> list = ResolveGrids(val);
00119: 				if (list == null || list.Count < 2)
00120: 				{
00121: 					continue;
00122: 				}
00123: 				GameGridInventory val2 = list[list.Count - 1];
00124: 				if (val2 == null || ((GameInventory)val2).childItems == null || ((GameInventory)val2).childItems.Count == 0)
00125: 				{
00126: 					continue;
00127: 				}
00128: 				GameItem[] array2 = ((GameInventory)val2).childItems.ToArray();
00129: 				foreach (GameItem val3 in array2)
00130: 				{
00131: 					if (val3 != null)
00132: 					{
00133: 						ModHelper.MoveItemToInventory(val3);
00134: 						num++;
00135: 					}
00136: 				}
00137: 			}
00138: 			catch (Exception ex)
00139: 			{
00140: 				ModCtx.LogWarning("收取产出失败: " + ex.Message);
00141: 			}
00142: 		}
00143: 		if (num > 0)
00144: 		{
00145: 			ModCtx.Log("产出已收入背包: " + num + " 件");
00146: 		}
00147: 	}
00148: 
00149: 	private static void ShowReport()
00150: 	{
00151: 		if (ReportTypes.Count == 0 && ReportMilk == 0 && ReportSemen == 0)
00152: 		{
00153: 			return;
00154: 		}
00155: 		StringBuilder stringBuilder = new StringBuilder();
00156: 		if (ReportTypes.Count > 0)
00157: 		{
00158: 			int num = 0;
00159: 			foreach (KeyValuePair<string, int> reportType in ReportTypes)
00160: 			{
00161: 				if (num > 0)
00162: 				{
00163: 					stringBuilder.Append("和");
00164: 				}
00165: 				stringBuilder.Append(reportType.Value).Append("个【").Append(reportType.Key)
00166: 					.Append("】");
00167: 				num++;
00168: 			}
00169: 			stringBuilder.Append("在【触手本体】内被你的触手调教了一夜");
00170: 		}
00171: 		else
00172: 		{
00173: 			stringBuilder.Append("触手本体蠕动了整夜");
00174: 		}
00175: 		List<string> list = new List<string>();
00176: 		if (ReportMilk > 0)
00177: 		{
00178: 			list.Add("【神奇乳汁】*" + ReportMilk);
00179: 		}
00180: 		if (ReportSemen > 0)
00181: 		{
00182: 			list.Add("【触手精液】*" + ReportSemen);
00183: 		}
00184: 		if (list.Count > 0)
00185: 		{
00186: 			stringBuilder.Append("，产出了 ").Append(string.Join("，", list.ToArray()));
00187: 		}
00188: 		else
00189: 		{
00190: 			stringBuilder.Append("，什么也没产出");
00191: 		}
00192: 		string text = stringBuilder.ToString();
00193: 		ModCtx.Log("夜间报告: " + text);
00194: 		try
00195: 		{
00196: 			string text2 = ((list.Count <= 0) ? ("第 " + GetToday() + " 天：调教了 " + ReportMaids + " 个苗床，无产出") : ("第 " + GetToday() + " 天：调教了 " + ReportMaids + " 个苗床，产出 " + string.Join("、", list.ToArray())));
00197: 			ModHelper.SetModData("lewd_meme", "last_report", text2);
00198: 		}
00199: 		catch
00200: 		{
00201: 		}
00202: 		try
00203: 		{
00204: 			if ((Object)(object)ModHook.Instance != (Object)null)
00205: 			{
00206: 				((MonoBehaviour)ModHook.Instance).StartCoroutine(DelayReport(text));
00207: 				ModCtx.Log("夜间报告: 已排队弹窗");
00208: 			}
00209: 			else
00210: 			{
00211: 				ModCtx.LogWarning("夜间报告: ModHook.Instance 为 null，无法弹窗");
00212: 			}
00213: 		}
00214: 		catch (Exception ex)
00215: 		{
00216: 			ModCtx.LogWarning("夜间报告弹窗失败: " + ex.Message);
00217: 		}
00218: 	}
00219: 
00220: 	private static IEnumerator DelayReport(string text)
00221: 	{
00222: 		yield return (object)new WaitForSeconds(1.2f);
00223: 		try
00224: 		{
00225: 			if ((Object)(object)CustomUIManager.Instance == (Object)null)
00226: 			{
00227: 				ModCtx.LogWarning("夜间报告: CustomUIManager.Instance 为 null");
00228: 				yield break;
00229: 			}
00230: 			CustomUIManager.Window("lewd_meme.nightreport", "夜间报告", "overlay").SetSize(480f, 240f).Center()
00231: 				.SetDraggable(true)
00232: 				.SetCloseOnEscape(true)
00233: 				.AddLabel(text, (string)null)
00234: 				.AddSpace(8f)
00235: 				.AddButton("知道了", (Action)delegate
00236: 				{
00237: 					CustomUIManager.Instance.CloseWindow("lewd_meme.nightreport");
00238: 				}, (string)null)
00239: 				.Show();
00240: 			ModCtx.Log("夜间报告: 窗口已弹出");
00241: 		}
00242: 		catch (Exception ex)
00243: 		{
00244: 			ModCtx.LogWarning("夜间报告弹窗异常: " + ex.Message);
00245: 		}
00246: 	}
00247: 
00248: 	public static GameItem Create()
00249: 	{
00250: 		List<GameGridInventory> grids;
00251: 		PixelWindow contentWindow = BuildWindow(GetBodyLevel(), out grids);
00252: 		GameItem val = ItemDirectory.CreateEmptyItem("tentacle_body");
00253: 		val.SetContentWindow(contentWindow);
00254: 		val.name = "触手本体";
00255: 		val.shortDescription = "这是什么？这是你的本体！可爱又能干的触手怪！让可爱的少女进入其中让她幸福。将这个太空站改造成为你的个人苗床巢吧。";
00256: 		val.flavorText = "";
00257: 		val.unitValue = 0L;
00258: 		val.SetGameItemType("TOOL");
00259: 		val.EnableTag("STANDARD_MACHINE_TAG", false);
00260: 		val.EnableTag("TENTACLE_BODY_TAG", false);
00261: 		val.EnableTag("IMPORTANT_TAG", false);
00262: 		val.EnableTag("NOT_FOR_RESALE", false);
00263: 		val.EnableTag("ITEM_HIDDEN_TAG", false);
00264: 		string text = SpriteInjector.InjectModTexture("tentacle_body", 48, 48);
00265: 		if (text != null)
00266: 		{
00267: 			val.SetSpriteAndShapeFromMod("lewd_meme", text);
00268: 		}
00269: 		else
00270: 		{
00271: 			val.SetSpriteAndShapeFromMod("lewd_meme", "tentacle_body");
00272: 		}
00273: 		val.SetShape(ItemFactory.RectShape(3, 3));
00274: 		((GameItemFunc)val).onCycleEndSlotItemFunc = delegate
00275: 		{
00276: 			SettleIfNeeded();
00277: 		};
00278: 		((GameItemFunc)val).onLoaded = delegate(GameItem it)
00279: 		{
00280: 			if (!Instances.Contains(it))
00281: 			{
00282: 				Instances.Add(it);
00283: 			}
00284: 		};
00285: 		if (!Instances.Contains(val))
00286: 		{
00287: 			Instances.Add(val);
00288: 		}
00289: 		Slots[val] = grids;
00290: 		try
00291: 		{
00292: 			ModCtx.Log($"触手本体 shape = {((val.shape != null) ? val.shape.width : (-1))}x{((val.shape != null) ? val.shape.height : (-1))} (global {((val.shape != null) ? val.shape.globalWidth : (-1))}x{((val.shape != null) ? val.shape.globalHeight : (-1))})");
00293: 		}
00294: 		catch
00295: 		{
00296: 		}
00297: 		return val;
00298: 	}
00299: 
00300: 	private static PixelWindow BuildWindow(int level, out List<GameGridInventory> grids)
00301: 	{
00302: 		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
00303: 		//IL_001b: Expected O, but got Unknown
00304: 		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
00305: 		//IL_008e: Expected O, but got Unknown
00306: 		grids = new List<GameGridInventory>();
00307: 		List<MachineryPanel> list = new List<MachineryPanel>();
00308: 		for (int i = 0; i < level; i++)
00309: 		{
00310: 			GameGridInventory val = new GameGridInventory(12, 12);
00311: 			((GameInventoryFunc)val).mayInventoryAddItemFunc = delegate(GameItem it, GameInventory inv)
00312: 			{
00313: 				if (it == null)
00314: 				{
00315: 					return false;
00316: 				}
00317: 				if (GeneralHelper.IsItemOwned(it))
00318: 				{
00319: 					int num;
00320: 					if (!(it.identifier == "maiden_lower"))
00321: 					{
00322: 						num = (it.IsTag("MAIDEN_FEMALE") ? 1 : 0);
00323: 						if (num == 0)
00324: 						{
00325: 							ModCtx.Log("苗床区拒绝: " + it.identifier);
00326: 						}
00327: 					}
00328: 					else
00329: 					{
00330: 						num = 1;
00331: 					}
00332: 					return (byte)num != 0;
00333: 				}
00334: 				return false;
00335: 			};
00336: 			grids.Add(val);
00337: 			list.Add(MachineHelper.Panel("苗床区 " + (i + 1), (PixelElement)(object)val, "input" + i, true));
00338: 		}
00339: 		GameGridInventory val2 = new GameGridInventory(12, 12);
00340: 		((GameInventoryFunc)val2).mayInventoryAddItemFunc = (GameItem it, GameInventory inv) => it != null && (it.identifier == "wondrous_milk" || it.identifier == "tentacle_semen");
00341: 		grids.Add(val2);
00342: 		list.Add(MachineHelper.Panel("产出区", (PixelElement)(object)val2, "output", true));
00343: 		return MachineHelper.CreateMachineryWindow(true, list.ToArray());
00344: 	}
00345: 
00346: 	public static List<GameGridInventory> ResolveGrids(GameItem machine)
00347: 	{
00348: 		if (machine == null)
00349: 		{
00350: 			return null;
00351: 		}
00352: 		if (Slots.TryGetValue(machine, out var value) && value != null && value.Count >= 2)
00353: 		{
00354: 			return value;
00355: 		}
00356: 		List<GameGridInventory> list = new List<GameGridInventory>();
00357: 		try
00358: 		{
00359: 			for (int i = 0; i < 32; i++)
00360: 			{
00361: 				GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(machine, i);
00362: 				if (bodyStorage != null)
00363: 				{
00364: 					GameGridInventory val = (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
00365: 					if (val != null)
00366: 					{
00367: 						list.Add(val);
00368: 					}
00369: 					continue;
00370: 				}
00371: 				break;
00372: 			}
00373: 		}
00374: 		catch (Exception ex)
00375: 		{
00376: 			ModCtx.LogWarning("读取本体网格失败: " + ex.Message);
00377: 		}
00378: 		if (list.Count >= 2)
00379: 		{
00380: 			Slots[machine] = list;
00381: 			int num = 0;
00382: 			for (int j = 0; j < list.Count - 1; j++)
00383: 			{
00384: 				if (((GameInventory)list[j]).childItems != null)
00385: 				{
00386: 					num += ((GameInventory)list[j]).childItems.Count;
00387: 				}
00388: 			}
00389: 			int num2 = ((((GameInventory)list[list.Count - 1]).childItems != null) ? ((GameInventory)list[list.Count - 1]).childItems.Count : 0);
00390: 			ModCtx.Log("重建网格引用: " + (list.Count - 1) + " 页苗床(" + num + " 个少女) + 产出(" + num2 + " 件)");
00391: 			return list;
00392: 		}
00393: 		return null;
00394: 	}
00395: 
00396: 	public static void RebuildAllWindows()
00397: 	{
00398: 		if (Instances.Count == 0)
00399: 		{
00400: 			return;
00401: 		}
00402: 		GameItem[] array = Instances.ToArray();
00403: 		foreach (GameItem val in array)
00404: 		{
00405: 			if (val == null)
00406: 			{
00407: 				Instances.Remove(val);
00408: 				continue;
00409: 			}
00410: 			try
00411: 			{
00412: 				RebuildOne(val);
00413: 			}
00414: 			catch (Exception ex)
00415: 			{
00416: 				ModCtx.LogWarning("重建窗口失败: " + ex.Message);
00417: 			}
00418: 		}
00419: 	}
00420: 
00421: 	private static void RebuildOne(GameItem machine)
00422: 	{
00423: 		List<GameGridInventory> list = ResolveGrids(machine);
00424: 		List<GameItem> list2 = new List<GameItem>();
00425: 		if (list != null)
00426: 		{
00427: 			for (int i = 0; i < list.Count - 1; i++)
00428: 			{
00429: 				if (list[i] != null && ((GameInventory)list[i]).childItems != null)
00430: 				{
00431: 					list2.AddRange(((GameInventory)list[i]).childItems);
00432: 				}
00433: 			}
00434: 			for (int j = 0; j < list.Count - 1; j++)
00435: 			{
00436: 				if (list[j] != null && ((GameInventory)list[j]).childItems != null)
00437: 				{
00438: 					((GameInventory)list[j]).childItems.Clear();
00439: 				}
00440: 			}
00441: 		}
00442: 		int bodyLevel = GetBodyLevel();
00443: 		List<GameGridInventory> grids;
00444: 		PixelWindow contentWindow = BuildWindow(bodyLevel, out grids);
00445: 		machine.SetContentWindow(contentWindow);
00446: 		Slots[machine] = grids;
00447: 		foreach (GameItem item in list2)
00448: 		{
00449: 			if (item == null)
00450: 			{
00451: 				continue;
00452: 			}
00453: 			bool flag = false;
00454: 			for (int k = 0; k < grids.Count - 1; k++)
00455: 			{
00456: 				if (flag)
00457: 				{
00458: 					break;
00459: 				}
00460: 				if (GraphUtils.CanAccept((GraphNodeStorage)(object)grids[k], item))
00461: 				{
00462: 					GraphUtils.TryAcceptAll((GraphNodeStorage)(object)grids[k], item, -1);
00463: 					flag = true;
00464: 				}
00465: 			}
00466: 			if (!flag)
00467: 			{
00468: 				ModHelper.MoveItemToInventory(item);
00469: 			}
00470: 		}
00471: 		ModCtx.Log("本体窗口已重建为 Lv." + bodyLevel + "（" + (grids.Count - 1) + " 页苗床）");
00472: 	}
00473: 
00474: 	public static void NightlyTick(GameItem machine, GameGridInventory input, GameGridInventory output)
00475: 	{
00476: 		if (machine == null || input == null)
00477: 		{
00478: 			return;
00479: 		}
00480: 		List<GameItem> childItems = ((GameInventory)input).childItems;
00481: 		if (childItems == null || childItems.Count == 0)
00482: 		{
00483: 			return;
00484: 		}
00485: 		GameItem[] array = childItems.ToArray();
00486: 		foreach (GameItem val in array)
00487: 		{
00488: 			if (val == null || !val.IsTag("MAIDEN_FEMALE"))
00489: 			{
00490: 				continue;
00491: 			}
00492: 			int num = ItemFactory.GetInt(val, "TRAIN_VALUE");
00493: 			int num2 = ItemFactory.GetInt(val, "SEX_COUNT");
00494: 			int num3 = ItemFactory.GetInt(val, "LACTATION_LV");
00495: 			int bodyLevel = GetBodyLevel();
00496: 			int num4 = 5 + (bodyLevel - 1);
00497: 			int num5 = 10 + (bodyLevel - 1) * 3;
00498: 			int num6 = bodyLevel - 1;
00499: 			int num7 = 2 + (bodyLevel - 1) * 2;
00500: 			int num8 = bodyLevel - 1;
00501: 			int num9 = 2 + (bodyLevel - 1) * 2;
00502: 			int num10 = Random.Range(num4, num5 + 1);
00503: 			int num11 = Random.Range(num6, num7 + 1);
00504: 			int num12 = Random.Range(num8, num9 + 1);
00505: 			num += num10 + num11 + num12;
00506: 			num2 += num10;
00507: 			int value = ItemFactory.GetInt(val, "ORAL_COUNT") + num11;
00508: 			int value2 = ItemFactory.GetInt(val, "ANAL_COUNT") + num12;
00509: 			ItemFactory.SetInt(val, "TRAIN_VALUE", num);
00510: 			ItemFactory.SetInt(val, "SEX_COUNT", num2);
00511: 			ItemFactory.SetInt(val, "ORAL_COUNT", value);
00512: 			ItemFactory.SetInt(val, "ANAL_COUNT", value2);
00513: 			string key = (string.IsNullOrEmpty(val.name) ? val.identifier : val.name);
00514: 			ReportTypes.TryGetValue(key, out var value3);
00515: 			ReportTypes[key] = value3 + 1;
00516: 			ReportMaids++;
00517: 			if (val.IsTag("MAIDEN_VIRGIN"))
00518: 			{
00519: 				val.DisableTag("MAIDEN_VIRGIN", false);
00520: 				val.ModifyTag("MAIDEN_VIRGIN_LOST", (Action<TagState>)delegate(TagState s)
00521: 				{
00522: 					s.Enable();
00523: 				}, false);
00524: 			}
00525: 			if (ItemFactory.GetInt(val, "ANAL_COUNT") > 0 && val.IsTag("MAIDEN_ANAL_VIRGIN"))
00526: 			{
00527: 				val.DisableTag("MAIDEN_ANAL_VIRGIN", false);
00528: 				val.ModifyTag("MAIDEN_ANAL_VIRGIN_LOST", (Action<TagState>)delegate(TagState s)
00529: 				{
00530: 					s.Enable();
00531: 				}, false);
00532: 			}
00533: 			bool flag = val.IsTag("MAIDEN_PREGNANT");
00534: 			if (!flag)
00535: 			{
00536: 				bool flag2 = val.IsTag("MAIDEN_OVULATING");
00537: 				if (!flag2 && Random.value < 1f / 7f)
00538: 				{
00539: 					val.ModifyTag("MAIDEN_OVULATING", (Action<TagState>)delegate(TagState s)
00540: 					{
00541: 						s.Enable();
00542: 					}, false);
00543: 					flag2 = true;
00544: 					ModCtx.Log(val.name + " 进入排卵期");
00545: 				}
00546: 				if (flag2)
00547: 				{
00548: 					float num13 = Mathf.Clamp01((float)num10 / 100f);
00549: 					if (Random.value < num13)
00550: 					{
00551: 						val.ModifyTag("MAIDEN_PREGNANT", (Action<TagState>)delegate(TagState s)
00552: 						{
00553: 							s.Enable();
00554: 						}, false);
00555: 						val.DisableTag("MAIDEN_OVULATING", false);
00556: 						ItemFactory.SetInt(val, "MAIDEN_PREGNANT_DAYS", 0);
00557: 						num3 = Mathf.Min(3, num3 + 1);
00558: 						ItemFactory.SetInt(val, "LACTATION_LV", num3);
00559: 						flag = true;
00560: 						ModCtx.Log(val.name + " 怀孕了（泌乳等级 +1）");
00561: 					}
00562: 					else
00563: 					{
00564: 						val.DisableTag("MAIDEN_OVULATING", false);
00565: 					}
00566: 				}
00567: 			}
00568: 			if (flag)
00569: 			{
00570: 				int num14 = ItemFactory.GetInt(val, "MAIDEN_PREGNANT_DAYS") + 1;
00571: 				ItemFactory.SetInt(val, "MAIDEN_PREGNANT_DAYS", num14);
00572: 				if (num14 >= 30)
00573: 				{
00574: 					ProduceDaughter(val);
00575: 				}
00576: 			}
00577: 			if (num3 >= 1 || flag)
00578: 			{
00579: 				int value4 = 100 + num3 * 40;
00580: 				if (TryProduceMilk(output, value4))
00581: 				{
00582: 					ReportMilk++;
00583: 				}
00584: 			}
00585: 			float num15 = Mathf.Min(1f, 0.4f + (float)num / 10f * 0.1f);
00586: 			if (Random.value < num15 && TryProduce(output, "tentacle_semen"))
00587: 			{
00588: 				ReportSemen++;
00589: 			}
00590: 			ItemFactory.UpdateMaidenDescription(val);
00591: 			ModCtx.Log($"苗床结算: 调教 {num} / 性交 {num2} / 泌乳 {num3} / 精液概率 {num15:P0}");
00592: 		}
00593: 	}
00594: 
00595: 	private static void ProduceDaughter(GameItem mother)
00596: 	{
00597: 		try
00598: 		{
00599: 			GameItem val = DirectoryMaster.Item("maiden_lower", true);
00600: 			if (val == null)
00601: 			{
00602: 				return;
00603: 			}
00604: 			if (!string.IsNullOrEmpty(mother.spritePath))
00605: 			{
00606: 				try
00607: 				{
00608: 					val.SetSpriteAndShapeFromMod(mother.spriteAtlasPath, mother.spritePath);
00609: 				}
00610: 				catch
00611: 				{
00612: 				}
00613: 			}
00614: 			string str = ItemFactory.GetStr(mother, "MAIDEN_FACTION");
00615: 			if (!string.IsNullOrEmpty(str))
00616: 			{
00617: 				ItemFactory.SetStr(val, "MAIDEN_FACTION", str);
00618: 			}
00619: 			val.name = (string.IsNullOrEmpty(str) ? "" : str) + "的女儿";
00620: 			val.flavorText = "她从母亲的肚子里爬出来，眼睛还没睁开就学会了服从。";
00621: 			val.unitValue = 900L;
00622: 			ItemFactory.UpdateMaidenDescription(val);
00623: 			ModHelper.SetItemOwnerShip(val, true);
00624: 			ModHelper.MoveItemToInventory(val);
00625: 			mother.DisableTag("MAIDEN_PREGNANT", false);
00626: 			ItemFactory.SetInt(mother, "MAIDEN_PREGNANT_DAYS", 0);
00627: 			ModCtx.Log("产女成功: " + val.name);
00628: 		}
00629: 		catch (Exception ex)
00630: 		{
00631: 			ModCtx.LogWarning("产女失败: " + ex.Message);
00632: 		}
00633: 	}
00634: 
00635: 	private static bool TryProduceMilk(GameGridInventory output, int value)
00636: 	{
00637: 		if (output == null)
00638: 		{
00639: 			return false;
00640: 		}
00641: 		try
00642: 		{
00643: 			GameItem val = DirectoryMaster.Item("wondrous_milk", true);
00644: 			if (val == null)
00645: 			{
00646: 				return false;
00647: 			}
00648: 			bool num = GraphUtils.CanAccept((GraphNodeStorage)(object)output, val);
00649: 			val.Destroy();
00650: 			if (!num)
00651: 			{
00652: 				return false;
00653: 			}
00654: 			GameItem val2 = DirectoryMaster.Item("wondrous_milk", true);
00655: 			val2.unitValue = value;
00656: 			GraphUtils.TryAcceptAll((GraphNodeStorage)(object)output, val2, -1);
00657: 			return true;
00658: 		}
00659: 		catch (Exception ex)
00660: 		{
00661: 			ModCtx.LogWarning("产出乳汁失败: " + ex.Message);
00662: 			return false;
00663: 		}
00664: 	}
00665: 
00666: 	public static bool HasBody()
00667: 	{
00668: 		return Instances.Count > 0;
00669: 	}
00670: 
00671: 	public static GameItem FindFirstMaiden()
00672: 	{
00673: 		foreach (GameItem instance in Instances)
00674: 		{
00675: 			if (instance == null)
00676: 			{
00677: 				continue;
00678: 			}
00679: 			List<GameGridInventory> list = ResolveGrids(instance);
00680: 			if (list == null || list.Count < 2)
00681: 			{
00682: 				continue;
00683: 			}
00684: 			for (int i = 0; i < list.Count - 1; i++)
00685: 			{
00686: 				List<GameItem> childItems = ((GameInventory)list[i]).childItems;
00687: 				if (childItems == null)
00688: 				{
00689: 					continue;
00690: 				}
00691: 				foreach (GameItem item in childItems)
00692: 				{
00693: 					if (item != null && item.IsTag("MAIDEN_FEMALE"))
00694: 					{
00695: 						return item;
00696: 					}
00697: 				}
00698: 			}
00699: 		}
00700: 		return null;
00701: 	}
00702: 
00703: 	public static int GetExpNeed()
00704: 	{
00705: 		int bodyLevel = GetBodyLevel();
00706: 		if (bodyLevel - 1 >= EXP_NEED.Length)
00707: 		{
00708: 			return 0;
00709: 		}
00710: 		return EXP_NEED[bodyLevel - 1];
00711: 	}
00712: 
00713: 	public static int GetBodyLevel()
00714: 	{
00715: 		return Mathf.Clamp(ModHelper.GetModDataInt("lewd_meme", "body_lv", 1), 1, 99);
00716: 	}
00717: 
00718: 	public static int GetBodyExp()
00719: 	{
00720: 		return ModHelper.GetModDataInt("lewd_meme", "body_exp", 0);
00721: 	}
00722: 
00723: 	private static void AddBodyExp(int amount)
00724: 	{
00725: 		int num = GetBodyLevel();
00726: 		int num2 = GetBodyExp() + amount;
00727: 		int num3 = num;
00728: 		while (num - 1 < EXP_NEED.Length && num2 >= EXP_NEED[num - 1])
00729: 		{
00730: 			num2 -= EXP_NEED[num - 1];
00731: 			num++;
00732: 			ModCtx.Log("触手本体升级到 Lv." + num + "（苗床容量与性爱强度提升）");
00733: 		}
00734: 		if (num > num3)
00735: 		{
00736: 			RebuildAllWindows();
00737: 		}
00738: 		ModHelper.SetModDataInt("lewd_meme", "body_lv", num);
00739: 		ModHelper.SetModDataInt("lewd_meme", "body_exp", num2);
00740: 	}
00741: 
00742: 	private static bool TryProduce(GameGridInventory output, string itemId)
00743: 	{
00744: 		if (output == null)
00745: 		{
00746: 			return false;
00747: 		}
00748: 		try
00749: 		{
00750: 			GameItem val = DirectoryMaster.Item(itemId, true);
00751: 			if (val == null)
00752: 			{
00753: 				return false;
00754: 			}
00755: 			bool num = GraphUtils.CanAccept((GraphNodeStorage)(object)output, val);
00756: 			val.Destroy();
00757: 			if (!num)
00758: 			{
00759: 				return false;
00760: 			}
00761: 			GraphUtils.TryAcceptAll((GraphNodeStorage)(object)output, DirectoryMaster.Item(itemId, true), -1);
00762: 			return true;
00763: 		}
00764: 		catch (Exception ex)
00765: 		{
00766: 			ModCtx.LogWarning("产出 " + itemId + " 失败: " + ex.Message);
00767: 			return false;
00768: 		}
00769: 	}
00770: }
~~~

## corp

### CorpPresence/UI/CompanyWindow.cs

~~~csharp
02367: 			}
02368: 			GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + 36f, ((Rect)(ref val)).width, 22f), ((catalogEntry2 == catalogEntry) ? L.T("特价 ", "Deal ") : "") + $"{num6}₵", theme.Price);
02369: 			int num7 = Math.Min(10, Services.Left(state, catalogEntry2));
02370: 			if (catalogEntry2.Limit() > 0)
02371: 			{
02372: 				GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + 54f, ((Rect)(ref val)).width, 16f), L.En ? $"{num7} left {Services.PeriodWord}" : $"{Services.PeriodWord}还剩 {num7}", theme.Small);
02373: 			}
02374: 			Cart.TryGetValue(catalogEntry2, out var value);
02375: 			value = Ui.Qty(new Rect(((Rect)(ref val)).x + 6f, ((Rect)(ref val)).yMax - 28f, ((Rect)(ref val)).width - 12f, 24f), value, Math.Min(10, num7), theme.BtnStyle, theme.Center);
02376: 			if (value > 0)
02377: 			{
02378: 				Cart[catalogEntry2] = value;
02379: 			}
02380: 			else
02381: 			{
02382: 				Cart.Remove(catalogEntry2);
02383: 			}
02384: 		}
02385: 	}
02386: 
02387: 	private static void PastelBribeCell(Rect cell)
02388: 	{
02389: 		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
02390: 		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
02391: 		Theme theme = _theme;
02392: 		GUI.Label(new Rect(((Rect)(ref cell)).x + 6f, ((Rect)(ref cell)).y + 34f, ((Rect)(ref cell)).width - 12f, 22f), L.T("打点主管", "Pay off the supervisor"), theme.Center);
02393: 		bool enabled = GUI.enabled;
02394: 		GUI.enabled = Cash >= PastelCo.Bribe;
02395: 		if (Ui.Button(new Rect(((Rect)(ref cell)).x + 8f, ((Rect)(ref cell)).yMax - 30f, ((Rect)(ref cell)).width - 16f, 26f), L.En ? $"Pay {PastelCo.Bribe}₵" : $"补交 {PastelCo.Bribe}₵", theme.BtnHotStyle))
02396: 		{
02397: 			Toast(PastelCo.PayBribe(deliver: true) ? L.T("分装机明天送到。", "The packaging machine arrives tomorrow.") : L.T("现金不够", "Not enough cash"));
02398: 		}
02399: 		GUI.enabled = enabled;
02400: 	}
02401: 
02402: 	private static void DrawPastelDealer(Rect r)
02403: 	{
02404: 		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
02405: 		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
02406: 		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
02407: 		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
02408: 		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
02409: 		Theme theme = _theme;
02410: 		CompanyState state = State;
02411: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 20f, 30f), L.T("下层合作商", "Lower Dealer"), theme.Title);
02412: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 40f, ((Rect)(ref r)).width - 20f, 120f), L.T("成为帕斯泰尔的特约经销点。\n\n", "Become a pastel partner outlet.\n\n") + (L.En ? $"· Regular rebate: every {PastelCo.RebatePeriod} days, {0.1:P0} of what you sold in pastel products, paid back by the courier.\n" : $"· 定期返点：每 {PastelCo.RebatePeriod} 天，把这段时间你卖出的帕斯泰尔产品按售价的 {0.1:P0} 返现，由送货员带来。\n") + L.T("· 解锁“下层客流”：帕斯泰尔会把下层区的熟客介绍到你店里。", "· Unlocks “Lower Foot Traffic”: pastel sends Lower Levels regulars to your shop."), theme.Wrap);
02413: 		if (!state.DealerJoined)
02414: 		{
02415: 			if (Ui.Button(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 180f, 220f, 34f), L.En ? $"Join (one-time {PastelCo.DealerFee}₵)" : $"加盟（一次性 {PastelCo.DealerFee}₵）", theme.BtnHotStyle))
02416: 			{
02417: 				Toast(Services.JoinDealer(_company) ?? L.T("欢迎加入帕斯泰尔！", "Welcome to pastel!"));
02418: 			}
02419: 		}
02420: 		else
02421: 		{
02422: 			int num = Day - state.DealerSinceDay;
02423: 			int num2 = PastelCo.RebatePeriod - num % PastelCo.RebatePeriod;
02424: 			GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 180f, ((Rect)(ref r)).width - 20f, 24f), L.En ? $"Dealer for {num} days. Sold {state.WeekSales:0}₵ of pastel products this period." : $"已加盟 {num} 天。本期已卖出 {state.WeekSales:0}₵ 的帕斯泰尔产品。", theme.Body);
02425: 			GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 208f, ((Rect)(ref r)).width - 20f, 24f), L.En ? $"Expected rebate {state.WeekSales * 0.1:0}₵, settles in {num2} days." : $"预计返点 {state.WeekSales * 0.1:0}₵，{num2} 天后结算。", theme.Body);
02426: 		}
02427: 	}
02428: 
02429: 	private static void DrawPastelTraffic(Rect r)
02430: 	{
02431: 		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
02432: 		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
02433: 		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
02434: 		Theme theme = _theme;
02435: 		CompanyState state = State;
02436: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 20f, 30f), L.T("下层客流", "Lower Foot Traffic"), theme.Title);
02437: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 40f, ((Rect)(ref r)).width - 20f, 100f), (L.En ? $"{PastelCo.TrafficFee}₵ per night. pastel sends Lower Levels regulars over: the next day, 1–2 extra Lower Levels customers come looking for food and daily goods.\n\nPauses automatically when you are short on cash." : $"每晚 {PastelCo.TrafficFee}₵。帕斯泰尔把下层区的熟客介绍过来：第二天会额外来 1–2 位想买食物、日用品的下层区顾客。\n\n现金不够时自动暂停。") + ((Cash >= PastelCo.TrafficFee) ? "" : (L.En ? $"\n(You have {Cash}₵, not enough for tonight's fee.)" : $"\n（你现在的现金 {Cash}₵，不够今晚的费用。）")), theme.Wrap);
02438: 		if (Ui.Button(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 160f, 180f, 34f), state.TrafficOn ? L.T("已开启（点击关闭）", "On (click to turn off)") : L.T("开启", "Turn on"), state.TrafficOn ? theme.BtnStyle : theme.BtnHotStyle))
02439: 		{
02440: 			state.TrafficOn = !state.TrafficOn;
02441: 			ModLog.Info("下层客流：" + (state.TrafficOn ? L.T("开启", "Turn on") : "关闭"));
02442: 			Toast(state.TrafficOn ? L.T("从今晚开始介绍熟客", "Regulars start coming from tonight") : L.T("已关闭", "Turned off"));
02443: 		}
02444: 	}
02445: 
02446: 	private static void DrawPastelFooter(Rect r)
02447: 	{
02448: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
02449: 		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
02450: 		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
02451: 		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
02452: 		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
02453: 		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
02454: 		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
02455: 		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
02456: 		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
02457: 		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
02458: 		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
02459: 		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
02460: 		Theme theme = _theme;
02461: 		CompanyState state = State;
02462: 		GUI.Box(r, GUIContent.none, theme.CellBox);
02463: 		Texture2D val = Portraits.Texture(PastelCo.HemingwayOpen() ? "pastel_milo_hemingway" : "pastel_milo", "maleLower_0");
02464: 		Rect r2 = default(Rect);
02465: 		((Rect)(ref r2))._002Ector(((Rect)(ref r)).x + 6f, ((Rect)(ref r)).y + 6f, 130f, 95f);
02466: 		Ui.Fill(r2, Tex.Hex(13223594));
02467: 		if ((Object)(object)val != (Object)null)
02468: 		{
02469: 			Ui.Image(r2, val);
02470: 		}
02471: 		GUI.Label(new Rect(((Rect)(ref r)).x + 6f, ((Rect)(ref r)).y + 104f, 130f, 22f), L.T("米洛", "Milo"), theme.Center);
02472: 		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 10f, 300f, 22f), "“" + MiloLines[Day % MiloLines.Length] + "”", theme.Body);
02473: 		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 40f, 300f, 22f), L.En ? $"Cooperation {state.Coop}  ·  {TierLabel(state)}" : $"合作度 {state.Coop}  ·  {TierLabel(state)}", theme.Body);
02474: 		CoopBar(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 66f, 300f, 12f));
02475: 		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 82f, 300f, 20f), NextTierHint(), theme.Small);
02476: 		if (_module != 0 || !PastelUnlocked(0, out var _))
02477: 		{
02478: 			return;
02479: 		}
02480: 		int num = Cart.Sum((KeyValuePair<CatalogEntry, int> kv) => PastelUnitPrice(kv.Key) * kv.Value);
02481: 		GUI.Label(new Rect(((Rect)(ref r)).xMax - 180f, ((Rect)(ref r)).y + 12f, 170f, 22f), L.En ? $"Total {num}₵" : $"合计 {num}₵", theme.Price);
02482: 		GUI.Label(new Rect(((Rect)(ref r)).xMax - 180f, ((Rect)(ref r)).y + 36f, 170f, 22f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
02483: 		bool enabled = num > 0 && num <= Cash && Services.CanOrderToday(_company);
02484: 		bool enabled2 = GUI.enabled;
02485: 		GUI.enabled = enabled;
02486: 		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, ((Rect)(ref r)).y + 64f, 150f, 36f), L.T("下单", "Order"), theme.BtnHotStyle))
02487: 		{
02488: 			string text = Services.PlaceOrder(_company, Cart, PastelUnitPrice);
02489: 			if (text == null)
02490: 			{
02491: 				Cart.Clear();
02492: 				Toast(L.T("已下单！", "Ordered! ") + _company.FrontDeskDone);
02493: 			}
02494: 			else
02495: 			{
02496: 				Toast(text);
02497: 			}
02498: 		}
02499: 		GUI.enabled = enabled2;
02500: 	}
02501: 
02502: 	private static int SolanaNeed(int i)
02503: 	{
02504: 		if (i >= SolanaModuleTier.Length)
02505: 		{
02506: 			return 0;
02507: 		}
02508: 		return SolanaModuleTier[i];
02509: 	}
02510: 
02511: 	private static void DrawSolana()
02512: 	{
02513: 		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
02514: 		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
~~~

~~~csharp
02904: 			if (text2 == null)
02905: 			{
02906: 				Cart.Clear();
02907: 				Toast(_company.FrontDeskDone);
02908: 			}
02909: 			else
02910: 			{
02911: 				Toast(text2);
02912: 			}
02913: 		}
02914: 		GUI.enabled = enabled2;
02915: 	}
02916: 
02917: 	private static void DrawSpacerCredits(Rect r)
02918: 	{
02919: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
02920: 		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
02921: 		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
02922: 		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
02923: 		Theme theme = _theme;
02924: 		GUI.Box(r, GUIContent.none, theme.CellBox);
02925: 		GUI.Label(new Rect(((Rect)(ref r)).x + 12f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 24f, 22f), L.T("技术来源", "Tech credits"), theme.Small);
02926: 		float num = ((Rect)(ref r)).y + 28f;
02927: 		(string, string)[] techCredits = SpacerCo.TechCredits;
02928: 		for (int i = 0; i < techCredits.Length; i++)
02929: 		{
02930: 			var (text, text2) = techCredits[i];
02931: 			GUI.Label(new Rect(((Rect)(ref r)).x + 16f, num, 180f, 24f), text, theme.Body);
02932: 			GUI.Label(new Rect(((Rect)(ref r)).x + 200f, num, ((Rect)(ref r)).width - 216f, 24f), text2, theme.Body);
02933: 			num += 26f;
02934: 		}
02935: 	}
02936: 
02937: 	private static void DrawSpacerMutual(Rect r)
02938: 	{
02939: 		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
02940: 		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
02941: 		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
02942: 		Theme theme = _theme;
02943: 		CompanyState state = State;
02944: 		if (_company.Tier(state) < 2)
02945: 		{
02946: 			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 40f), L.En ? ("Only at " + _company.TierNames[1] + ".") : (_company.TierNames[1] + "才有。"), theme.Wrap);
02947: 			return;
02948: 		}
02949: 		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 90f), L.En ? $"Spacers at the docks are short on food, water and medicine. With this on, the Alliance points people your way every night: the next day, 1–2 extra Spacers come by to buy whatever they need.\n\n{SpacerCo.MutualFee}₵ per night. Stops automatically when you are short on cash." : $"码头上的星际客缺吃、缺水、缺药。开着这个，联盟每晚把人往你这儿指：第二天多来 1–2 位星际客，缺什么买什么。\n\n每晚 {SpacerCo.MutualFee}₵。现金不够时自动停。", theme.Wrap);
02950: 		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 110f, 200f, 34f), state.TrafficOn ? L.T("开着（点击关掉）", "On (click to turn off)") : L.T("开", "Turn on"), state.TrafficOn ? theme.BtnStyle : theme.BtnHotStyle))
02951: 		{
02952: 			state.TrafficOn = !state.TrafficOn;
02953: 			ModLog.Info("街坊互助：" + (state.TrafficOn ? "开启" : "关闭"));
02954: 			Toast(state.TrafficOn ? L.T("今晚开始。", "Starting tonight.") : L.T("停了。", "Stopped."));
02955: 		}
02956: 	}
02957: }
~~~

### CorpPresence/UI/MenuLabels.cs

~~~csharp
00001: using System;
00002: 
00003: namespace CorpPresence.UI;
00004: 
00005: internal static class MenuLabels
00006: {
00007: 	private static readonly string[][] Chinese = new string[12][]
00008: 	{
00009: 		new string[5] { "正品订货", "体力增强剂申领", "赝品换正品", "标本", "品牌目录" },
00010: 		new string[4] { "订货", "利维坦净水器", "罐装设备", "品牌目录" },
00011: 		new string[5] { "制式订货", "高端配件", "返厂翻新", "锻造", "品牌目录" },
00012: 		new string[4] { "邮购订货", "下层合作商", "下层客流", "品牌目录" },
00013: 		new string[3] { "今天进货吗？", "又是我！", "货多，随便挑。" },
00014: 		new string[3] { "军械订货", "地下销赃网络", "任务" },
00015: 		new string[2] { "军械订货", "任务" },
00016: 		new string[1] { "和解" },
00017: 		new string[1] { "营救奥丁" },
00018: 		new string[4] { "贵宾引荐", "实验室采购单", "会员礼遇", "品牌目录" },
00019: 		new string[3] { "码头补给", "街坊互助", "品牌目录" },
00020: 		new string[6] { "开源军械", "地下药房", "码头补给", "街坊互助", "合作阵线", "品牌目录" }
00021: 	};
00022: 
00023: 	private static readonly string[][] English = new string[12][]
00024: 	{
00025: 		new string[5] { "Genuine Orders", "Stamina Booster Request", "Fake-for-Genuine Swap", "Specimens", "Brand Catalog" },
00026: 		new string[4] { "Orders", "Leviathan Purifier", "Canning Equipment", "Brand Catalog" },
00027: 		new string[5] { "Standard Issue Orders", "High-End Attachments", "Factory Refurbishment", "Forging", "Brand Catalog" },
00028: 		new string[4] { "Mail Order", "Lower Dealer", "Lower Foot Traffic", "Brand Catalog" },
00029: 		new string[3] { "Stocking up today?", "Me again!", "Plenty of goods. Take your pick." },
00030: 		new string[3] { "Arms Orders", "Fence Network", "Tasks" },
00031: 		new string[2] { "Arms Orders", "Tasks" },
00032: 		new string[1] { "Make Peace" },
00033: 		new string[1] { "Rescue Odin" },
00034: 		new string[4] { "VIP Referral", "Lab Purchase Order", "Member Perks", "Brand Catalog" },
00035: 		new string[3] { "Dock Supplies", "Mutual Aid", "Brand Catalog" },
00036: 		new string[6] { "Open-Source Arms", "Back-Alley Pharmacy", "Dock Supplies", "Mutual Aid", "Joint Front", "Brand Catalog" }
00037: 	};
00038: 
00039: 	private static string[] _lastBase;
00040: 
00041: 	private static string _lastExtra;
00042: 
00043: 	private static string[] _lastCombined;
00044: 
00045: 	public static string[] Get(int index, bool english)
00046: 	{
00047: 		return (english ? English : Chinese)[index];
00048: 	}
00049: 
00050: 	public static string[] Append(string[] basis, string extra)
00051: 	{
00052: 		if (extra == null)
00053: 		{
00054: 			return basis;
00055: 		}
00056: 		if (basis == _lastBase && extra == _lastExtra)
00057: 		{
00058: 			return _lastCombined;
00059: 		}
00060: 		string[] array = new string[basis.Length + 1];
00061: 		Array.Copy(basis, array, basis.Length);
00062: 		array[basis.Length] = extra;
00063: 		_lastBase = basis;
00064: 		_lastExtra = extra;
00065: 		return _lastCombined = array;
00066: 	}
00067: }
~~~

## brewing

### BrewingExpansion/Formal/FormalNiniBatchLimit.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Reflection;
00004: using HarmonyLib;
00005: 
00006: namespace BrewingExpansion.Formal;
00007: 
00008: [HarmonyPatch]
00009: internal static class FormalNiniBatchLimit
00010: {
00011: 	private static MethodBase[] TargetMethods()
00012: 	{
00013: 		return new MethodInfo[2]
00014: 		{
00015: 			AccessTools.Method(typeof(NegociationUIManager), "SellMultiple", new Type[2]
00016: 			{
00017: 				typeof(List<GameItem>),
00018: 				typeof(bool)
00019: 			}, (Type[])null),
00020: 			AccessTools.Method(typeof(NegociationUIManager), "BuyMultipleItem", new Type[1] { typeof(List<GameItem>) }, (Type[])null)
00021: 		};
00022: 	}
00023: 
00024: 	private static void Prefix(out FormalReputation.GainScope __state)
00025: 	{
00026: 		__state = (ModEntry.Enabled ? FormalReputation.Begin() : null);
00027: 	}
00028: 
00029: 	private static void Finalizer(FormalReputation.GainScope __state)
00030: 	{
00031: 		__state?.Dispose();
00032: 	}
00033: }
~~~

### BrewingExpansion/Formal/FormalNiniCategories.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Reflection;
00004: using HarmonyLib;
00005: 
00006: namespace BrewingExpansion.Formal;
00007: 
00008: [HarmonyPatch]
00009: internal static class FormalNiniCategories
00010: {
00011: 	private static IEnumerable<MethodBase> TargetMethods()
00012: 	{
00013: 		MethodInfo methodInfo = AccessTools.DeclaredMethod(typeof(StoreClient), "IsClientBuyingThisItem", new Type[1] { typeof(GameItem) }, (Type[])null);
00014: 		MethodInfo detailed = AccessTools.DeclaredMethod(typeof(StoreClient), "IsClientBuyingThisItem", new Type[2]
00015: 		{
00016: 			typeof(GameItem),
00017: 			typeof(bool)
00018: 		}, (Type[])null);
00019: 		if (methodInfo == null || methodInfo.ReturnType != typeof(bool) || methodInfo.IsStatic)
00020: 		{
00021: 			throw new MissingMethodException("Unsupported StoreClient.IsClientBuyingThisItem(GameItem) interface.");
00022: 		}
00023: 		yield return methodInfo;
00024: 		if (detailed != null)
00025: 		{
00026: 			if (detailed.ReturnType != typeof(bool) || detailed.IsStatic)
00027: 			{
00028: 				throw new MissingMethodException("Unsupported StoreClient.IsClientBuyingThisItem(GameItem,bool) interface.");
00029: 			}
00030: 			yield return detailed;
00031: 		}
00032: 	}
00033: 
00034: 	private static void Postfix(StoreClient __instance, GameItem __0, ref bool __result)
00035: 	{
00036: 		if ((ModEntry.Enabled & __result) && __instance.identifier == "be_weekly_trader")
00037: 		{
00038: 			__result = __0 != null && (__0.IsGameItemType("ALCOHOL") || __0.IsTag("CIGARETTE_TAG") || __0.IsGameItemType("CIGARETTE") || __0.IsGameItemType("TOBACCO"));
00039: 		}
00040: 	}
00041: }
~~~

### BrewingExpansion/Formal/FormalNiniContactName.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace BrewingExpansion.Formal;
00004: 
00005: [HarmonyPatch(typeof(StorePhoneClient), "GetLocalizedDisplayName")]
00006: internal static class FormalNiniContactName
00007: {
00008: 	private static bool Prefix(StorePhoneClient __instance, ref string __result)
00009: 	{
00010: 		if (!ModEntry.Enabled || __instance.dialogFuncId != "brewing_expansion.nini")
00011: 		{
00012: 			return true;
00013: 		}
00014: 		__result = ModText.Get("be_nini_name");
00015: 		return false;
00016: 	}
00017: }
~~~

### BrewingExpansion/Formal/FormalNiniFinalGainCap.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace BrewingExpansion.Formal;
00004: 
00005: [HarmonyPatch(typeof(StoreReputation), "CheckReputation")]
00006: internal static class FormalNiniFinalGainCap
00007: {
00008: 	private static void Prefix(StoreReputation __instance, ref double ___amount)
00009: 	{
00010: 		FormalReputation.ChangeScope change = FormalReputation.Change;
00011: 		if (ModEntry.Enabled && change != null && !change.Applied && change.Faction == __instance)
00012: 		{
00013: 			change.Applied = true;
00014: 			double num = ___amount - change.Before;
00015: 			if (num > 0.0)
00016: 			{
00017: 				___amount = change.Before + change.Gain.Take(num);
00018: 			}
00019: 		}
00020: 	}
00021: }
~~~

### BrewingExpansion/Formal/FormalNiniInspectNative.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace BrewingExpansion.Formal;
00004: 
00005: [HarmonyPatch(typeof(StoreClient), "ClientExposeFeature")]
00006: internal static class FormalNiniInspectNative
00007: {
00008: 	private static void Prefix(StoreClient __instance, GameItem __0)
00009: 	{
00010: 		if (ModEntry.Enabled)
00011: 		{
00012: 			FormalTobaccoInspection.Reveal(__instance, __0);
00013: 		}
00014: 	}
00015: }
~~~

### BrewingExpansion/Formal/FormalNiniInspectSale.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace BrewingExpansion.Formal;
00004: 
00005: [HarmonyPatch(typeof(PlayerStore), "SellItem")]
00006: internal static class FormalNiniInspectSale
00007: {
00008: 	private static bool Prefix(GameItem item)
00009: 	{
00010: 		return FormalTobaccoInspection.BeforeSale(item);
00011: 	}
00012: }
~~~

### BrewingExpansion/Formal/FormalNiniInspectTable.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace BrewingExpansion.Formal;
00004: 
00005: [HarmonyPatch(typeof(PlayerStore), "SellItemFromTable")]
00006: internal static class FormalNiniInspectTable
00007: {
00008: 	private static bool Prefix(PlayerStore __instance)
00009: 	{
00010: 		return FormalTobaccoInspection.BeforeSale(__instance.currentNegociatedItem);
00011: 	}
00012: }
~~~

### BrewingExpansion/Formal/FormalNiniRepCap.cs

~~~csharp
00001: using System;
00002: using System.Reflection;
00003: using HarmonyLib;
00004: 
00005: namespace BrewingExpansion.Formal;
00006: 
00007: [HarmonyPatch]
00008: internal static class FormalNiniRepCap
00009: {
00010: 	private static MethodBase[] TargetMethods()
00011: 	{
00012: 		return new MethodInfo[2]
00013: 		{
00014: 			AccessTools.Method(typeof(StoreReputation), "ModReputation", new Type[1] { typeof(double) }, (Type[])null),
00015: 			AccessTools.Method(typeof(StoreReputation), "ModReputationRaw", new Type[1] { typeof(double) }, (Type[])null)
00016: 		};
00017: 	}
00018: 
00019: 	private static void Prefix(StoreReputation __instance, out FormalReputation.ChangeScope __state)
00020: 	{
00021: 		__state = ((ModEntry.Enabled && __instance.factionId == "BE_FACTION_OUTER_RING" && FormalReputation.Current != null) ? new FormalReputation.ChangeScope(__instance) : null);
00022: 	}
00023: 
00024: 	private static void Finalizer(FormalReputation.ChangeScope __state)
00025: 	{
00026: 		__state?.Dispose();
00027: 	}
00028: }
~~~

### BrewingExpansion/Formal/FormalNiniTradeFaction.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace BrewingExpansion.Formal;
00004: 
00005: [HarmonyPatch(typeof(StoreReputation), "OnItemTraded")]
00006: internal static class FormalNiniTradeFaction
00007: {
00008: 	private static void Prefix(ref string factionId, StoreClient storeClient, bool isPlayerSold, out FormalReputation.GainScope __state)
00009: 	{
00010: 		__state = null;
00011: 		if (ModEntry.Enabled && FormalReputation.IsTradeClient(storeClient))
00012: 		{
00013: 			FormalReputation.Ensure();
00014: 			factionId = "BE_FACTION_OUTER_RING";
00015: 			storeClient.clientFaction = factionId;
00016: 			__state = new FormalReputation.GainScope();
00017: 		}
00018: 	}
00019: 
00020: 	private static void Finalizer(FormalReputation.GainScope __state)
00021: 	{
00022: 		__state?.Dispose();
00023: 	}
00024: }
~~~

### BrewingExpansion/Formal/FormalReputation.cs

~~~csharp
00084: 	internal static ChangeScope Change;
00085: 
00086: 	private static readonly int[] Thresholds = new int[15]
00087: 	{
00088: 		-100, -80, -60, -40, -20, 20, 40, 60, 80, 100,
00089: 		120, 140, 160, 180, 200
00090: 	};
00091: 
00092: 	internal static int Reputation
00093: 	{
00094: 		get
00095: 		{
00096: 			StoreReputation storeReputation = StoreReputation.GetStoreReputation("BE_FACTION_OUTER_RING");
00097: 			if (storeReputation == null)
00098: 			{
00099: 				return 0;
00100: 			}
00101: 			return storeReputation.GetReputation();
00102: 		}
00103: 	}
00104: 
00105: 	internal static int Tier
00106: 	{
00107: 		get
00108: 		{
00109: 			if (Reputation >= 0)
00110: 			{
00111: 				return Math.Min(10, Reputation / 20);
00112: 			}
00113: 			return -Math.Min(5, -Reputation / 20);
00114: 		}
00115: 	}
00116: 
00117: 	internal static int SeedCountModifier
00118: 	{
00119: 		get
00120: 		{
00121: 			if (Tier > -3)
00122: 			{
00123: 				return Tier * 2;
00124: 			}
00125: 			return 0;
00126: 		}
00127: 	}
00128: 
00129: 	internal static int NiniBudget => (int)Math.Round(12000.0 * (1.0 + (double)Math.Max(-4, Math.Min(0, Tier)) * 0.2));
00130: 
00131: 	internal static void Ensure()
00132: 	{
00133: 		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
00134: 		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
00135: 		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
00136: 		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
00137: 		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
00138: 		//IL_0055: Expected O, but got Unknown
00139: 		PlayerStore instance = PlayerStore.Instance;
00140: 		if (instance?.storeReputations == null)
00141: 		{
00142: 			return;
00143: 		}
00144: 		StoreReputation val = StoreReputation.GetStoreReputation("BE_FACTION_OUTER_RING");
00145: 		if (val == null)
00146: 		{
00147: 			val = new StoreReputation
00148: 			{
00149: 				factionId = "BE_FACTION_OUTER_RING",
00150: 				factionColor1 = "#98454D",
00151: 				factionColor2 = "#98454D",
00152: 				factionColor3 = "#98454D"
00153: 			};
00154: 			instance.storeReputations.Add(val);
00155: 		}
00156: 		val.factionDisplay = ModText.Get("be_outer_ring_name");
00157: 		int[] array = new int[15]
00158: 		{
00159: 			-100, -80, -60, -40, -20, 20, 40, 60, 80, 100,
00160: 			120, 140, 160, 180, 200
00161: 		};
00162: 		for (int i = 0; i < array.Length; i++)
00163: 		{
00164: 			int num = array[i];
00165: 			string key = "BE_OUTER_" + ((num < 0) ? ("N" + -num) : ("P" + num));
00166: 			if (!val.perks.Any((StoreReputationPerk p) => p.perkId == key))
00167: 			{
00168: 				StoreReputationPerk val2 = ((num < 0) ? StoreReputationPerk.SecWatchlist() : StoreReputationPerk.SecProtection());
00169: 				val2.perkId = key;
00170: 				val2.perkThreshold = num;
00171: 				val2.perkType = ((num >= 0) ? 1 : 0);
00172: 				val2.firstTimeUnlockActionDone = true;
00173: 				val2.isUnlocked = ((num < 0) ? (val.GetReputation() <= num) : (val.GetReputation() >= num));
00174: 				val.perks.Add(val2);
00175: 			}
00176: 		}
00177: 	}
00178: 
00179: 	internal static string PerkText(StoreReputationPerk perk, bool description)
00180: 	{
00181: 		if (perk == null || perk.perkId?.StartsWith("BE_OUTER_", StringComparison.Ordinal) != true)
00182: 		{
00183: 			return null;
00184: 		}
00185: 		int perkThreshold = perk.perkThreshold;
00186: 		if (Array.IndexOf(Thresholds, perkThreshold) >= 0)
00187: 		{
00188: 			return ModText.Get("be_outer_" + ((perkThreshold < 0) ? ("n" + -perkThreshold) : ("p" + perkThreshold)) + (description ? "_description" : "_name"));
00189: 		}
00190: 		return null;
00191: 	}
00192: 
00193: 	internal static bool IsTradeClient(StoreClient client)
00194: 	{
00195: 		if (!(client?.identifier == "be_weekly_trader") && !(client?.identifier == "be_seed_trader_47"))
00196: 		{
00197: 			return client?.identifier == "be_brew_shop_47";
00198: 		}
00199: 		return true;
00200: 	}
00201: 
00202: 	internal static GainScope Begin()
00203: 	{
00204: 		PlayerStore instance = PlayerStore.Instance;
00205: 		object client;
00206: 		if (instance == null)
00207: 		{
00208: 			client = null;
00209: 		}
00210: 		else
00211: 		{
00212: 			StoreClientInstance currentClientInstance = instance.currentClientInstance;
00213: 			client = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
00214: 		}
00215: 		if (!IsTradeClient((StoreClient)client))
00216: 		{
00217: 			return null;
00218: 		}
~~~

### BrewingExpansion/Formal/FormalTobaccoInspection.cs

~~~csharp
00016: 		string identifier = ItemConditionList.CreateCounterfeitCigarette().identifier;
00017: 		ItemFeature val = item.FindItemFeatureByCategory("CATEGORY_GENUINE_CIGARETTE");
00018: 		ItemFeature val2 = item.FindItemFeatureByID("TE_TOBACCO_CONDITION");
00019: 		bool flag = false;
00020: 		bool flag2 = false;
00021: 		ItemFeature[] array = (ItemFeature[])(object)new ItemFeature[2] { val, val2 };
00022: 		foreach (ItemFeature val3 in array)
00023: 		{
00024: 			if (val3 == null || val3.isDisabled || val3.IsExposed())
00025: 			{
00026: 				continue;
00027: 			}
00028: 			bool flag3 = val3 == val && val3.realCondition?.identifier == identifier;
00029: 			bool flag4 = val3 == val2 && val3.realCondition?.identifier == "te_condition_defective";
00030: 			if ((flag3 || flag4) && !(val3.fakeCondition?.identifier == val3.realCondition?.identifier))
00031: 			{
00032: 				bool flag5 = flag3 || val3.fakeCondition?.identifier == "te_condition_good";
00033: 				val3.ExposeFeature(true, true);
00034: 				if (instance != PlayerStore.Instance || !MachineConnections.Live(item))
00035: 				{
00036: 					return false;
00037: 				}
00038: 				if (val3.IsExposed())
00039: 				{
00040: 					flag = true;
00041: 					flag2 = flag2 || flag5;
00042: 				}
00043: 			}
00044: 		}
00045: 		if (!flag)
00046: 		{
00047: 			return false;
00048: 		}
00049: 		if (flag2 && !item.IsTag("BE_NINI_TOBACCO_PENALIZED"))
00050: 		{
00051: 			FormalReputation.Ensure();
00052: 			StoreReputation.ModReputationRaw("BE_FACTION_OUTER_RING", -5, true);
00053: 			item.EnableTag("BE_NINI_TOBACCO_PENALIZED", false);
00054: 			item.EnableTag("TE_FRAUD_PENALIZED", false);
00055: 		}
00056: 		if (FormalVisitors.Open)
00057: 		{
00058: 			DialogUIManager instance2 = DialogUIManager.Instance;
00059: 			if (instance2 != null)
00060: 			{
00061: 				instance2.StartDialogue(new Dialogue().SetText(ModText.Get("be_nini_name"), ModText.Get("be_nini_inspection_warning")), false);
00062: 			}
00063: 		}
00064: 		return true;
00065: 	}
00066: 
00067: 	internal static bool BeforeSale(GameItem item)
00068: 	{
00069: 		if (!ModEntry.Enabled || RemainingConsumables.Client?.identifier != "be_weekly_trader")
00070: 		{
00071: 			return true;
00072: 		}
00073: 		try
00074: 		{
00075: 			if (!Reveal(RemainingConsumables.Client, item))
00076: 			{
00077: 				return true;
00078: 			}
00079: 			NegociationUIManager instance = NegociationUIManager.Instance;
00080: 			if (instance != null)
00081: 			{
00082: 				instance.UpdateUI(item);
00083: 			}
00084: 			NegociationUIManager instance2 = NegociationUIManager.Instance;
00085: 			if (instance2 != null)
00086: 			{
00087: 				instance2.OpenUI();
00088: 			}
00089: 			return false;
00090: 		}
00091: 		catch (Exception ex)
00092: 		{
00093: 			Debug.LogWarning((object)("[BrewingExpansion.Formal] Nini inspection interrupted; item retained: " + ex));
00094: 			return false;
00095: 		}
00096: 	}
00097: }
~~~

### BrewingExpansion/Formal/FormalVisitorDialogueHours.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace BrewingExpansion.Formal;
00004: 
00005: [HarmonyPatch(typeof(StoreClient), "StartMainDialogue")]
00006: internal static class FormalVisitorDialogueHours
00007: {
00008: 	private static bool Prefix(StoreClient __instance)
00009: 	{
00010: 		if (ModEntry.Enabled && !FormalVisitors.Open)
00011: 		{
00012: 			return !FormalVisitors.IsDayClient(__instance.identifier);
00013: 		}
00014: 		return true;
00015: 	}
00016: }
~~~

### BrewingExpansion/Formal/FormalVisitorPersistence.cs

~~~csharp
00048: 			if (!string.IsNullOrEmpty(modData))
00049: 			{
00050: 				Receipt receipt2 = JsonUtility.FromJson<Receipt>(modData);
00051: 				if (receipt2?.day == receipt.day)
00052: 				{
00053: 					receipt.stock = receipt.stock.Concat(receipt2.stock ?? Array.Empty<string>()).ToArray();
00054: 				}
00055: 			}
00056: 		}
00057: 		ModHelper.SetModData("brewing_expansion", "visitor_queue_v1", JsonUtility.ToJson((object)receipt));
00058: 	}
00059: 
00060: 	internal static void Restore()
00061: 	{
00062: 		string modData = ModHelper.GetModData("brewing_expansion", "visitor_queue_v1", "");
00063: 		if (string.IsNullOrEmpty(modData))
00064: 		{
00065: 			return;
00066: 		}
00067: 		Receipt receipt = JsonUtility.FromJson<Receipt>(modData);
00068: 		PlayerStore instance = PlayerStore.Instance;
00069: 		if (receipt == null || instance?.storeClientManager == null)
00070: 		{
00071: 			return;
00072: 		}
00073: 		ModHelper.SetModData("brewing_expansion", "visitor_queue_v1", "");
00074: 		if (receipt.day != StoreStation.Instance.dayCounter)
00075: 		{
00076: 			return;
00077: 		}
00078: 		receipt.stock = (receipt.stock ?? Array.Empty<string>()).Select(ItemIdentity.Canonical).ToArray();
00079: 		StoreClientInstance currentClientInstance = instance.currentClientInstance;
00080: 		string text = ((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.identifier);
00081: 		bool called = ModHelper.GetModDataInt("brewing_expansion", "nini_last_call", -999) == receipt.day;
00082: 		string[] array = (from text2 in ((!FormalVisitors.IsDayClient(receipt.active)) ? ((IEnumerable<string>)Array.Empty<string>()) : ((IEnumerable<string>)new string[1] { receipt.active })).Concat(receipt.queue ?? Array.Empty<string>())
00083: 			where FormalVisitors.IsDayClient(text2) && (text2 != "be_weekly_trader" || called)
00084: 			select text2).Distinct().ToArray();
00085: 		if (receipt.active == "be_seed_trader_47")
00086: 		{
00087: 			ModHelper.SetModData("brewing_expansion", "visitor_stock_resume_v1", JsonUtility.ToJson((object)receipt));
00088: 		}
00089: 		foreach (string id in array)
00090: 		{
00091: 			if (!(id == text) && !instance.storeClientManager.clientStack.Any((StoreClient c) => c.identifier == id))
00092: 			{
00093: 				StoreClient item = ((id == "be_weekly_trader") ? FormalVisitors.Nini() : ((id == "be_seed_trader_47") ? FormalVisitors.FortySeven() : FormalVisitors.Assassin()));
00094: 				if (id == receipt.active || id == "be_outer_ring_assassin")
00095: 				{
00096: 					instance.storeClientManager.clientStack.Insert(0, item);
00097: 				}
00098: 				else
00099: 				{
00100: 					instance.storeClientManager.clientStack.Add(item);
00101: 				}
00102: 			}
00103: 		}
00104: 		FormalVisitors.EnsureFortySeven(instance, allowNew: false);
00105: 	}
00106: 
00107: 	internal static bool ResumeStock()
00108: 	{
00109: 		string modData = ModHelper.GetModData("brewing_expansion", "visitor_stock_resume_v1", "");
00110: 		if (string.IsNullOrEmpty(modData))
00111: 		{
00112: 			return false;
00113: 		}
00114: 		Receipt receipt = JsonUtility.FromJson<Receipt>(modData);
00115: 		if (receipt == null || receipt.day != StoreStation.Instance.dayCounter)
00116: 		{
00117: 			ModHelper.SetModData("brewing_expansion", "visitor_stock_resume_v1", "");
00118: 			return false;
00119: 		}
00120: 		List<string> list = (receipt.stock ?? Array.Empty<string>()).Select(ItemIdentity.Canonical).ToList();
00121: 		string[] array = list.ToArray();
00122: 		foreach (string text in array)
00123: 		{
00124: 			string text2 = RemainingItems.Find(text)?.Kind;
00125: 			if (BrewingSeeds.Find(text) == null && text2 != "Recipe" && text2 != "Manual")
00126: 			{
00127: 				list.Remove(text);
00128: 				continue;
00129: 			}
00130: 			GameItem val = DirectoryMaster.Item(text, false);
00131: 			val.EnableTag("BE_47_STOCK", false);
00132: 			PlayerStore.Instance.AddDirectSellingItemToTable(val, false, false, false, 0);
00133: 			if (val.parentInventory == null)
00134: 			{
00135: 				if (MachineConnections.Live(val))
00136: 				{
00137: 					val.Destroy();
00138: 				}
00139: 			}
00140: 			else
00141: 			{
00142: 				list.Remove(text);
00143: 				receipt.stock = list.ToArray();
00144: 				ModHelper.SetModData("brewing_expansion", "visitor_stock_resume_v1", JsonUtility.ToJson((object)receipt));
00145: 			}
00146: 		}
00147: 		return true;
00148: 	}
00149: }
~~~

### BrewingExpansion/Formal/FormalVisitors.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Linq;
00004: using UnityEngine;
00005: 
00006: namespace BrewingExpansion.Formal;
00007: 
00008: internal static class FormalVisitors
00009: {
00010: 	internal const string NiniId = "be_weekly_trader";
00011: 
00012: 	internal const string FortySevenId = "be_seed_trader_47";
00013: 
00014: 	internal const string AssassinId = "be_outer_ring_assassin";
00015: 
00016: 	internal const string PhoneId = "brewing_expansion.nini";
00017: 
00018: 	private const string LegacyPhoneId = "BrewingExpansion.Nini";
00019: 
00020: 	private static readonly Dictionary<string, Sprite> Portraits = new Dictionary<string, Sprite>();
00021: 
00022: 	internal static bool Open
00023: 	{
00024: 		get
00025: 		{
00026: 			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
00027: 			//IL_0011: Invalid comparison between Unknown and I4
00028: 			PlayerStore instance = PlayerStore.Instance;
00029: 			if (instance == null)
00030: 			{
00031: 				return false;
00032: 			}
00033: 			return (int)instance.storeState == 1;
00034: 		}
00035: 	}
00036: 
00037: 	internal static bool IsDayClient(string id)
00038: 	{
00039: 		if (!(id == "be_weekly_trader") && !(id == "be_seed_trader_47"))
00040: 		{
00041: 			return id == "be_outer_ring_assassin";
00042: 		}
00043: 		return true;
00044: 	}
00045: 
00046: 	internal static string CallCooldown(int days)
00047: 	{
00048: 		return ModText.Get("be_nini_phone_cooldown", days);
00049: 	}
00050: 
00051: 	internal static void RegisterFactories()
00052: 	{
00053: 		if (!StoreClientListDict.storeClientDict.ContainsKey("be_weekly_trader"))
00054: 		{
00055: 			StoreClientListDict.storeClientDict.Add("be_weekly_trader", Nini);
00056: 		}
00057: 		if (!StoreClientListDict.storeClientDict.ContainsKey("be_seed_trader_47"))
00058: 		{
00059: 			StoreClientListDict.storeClientDict.Add("be_seed_trader_47", FortySeven);
00060: 		}
00061: 		if (!StoreClientListDict.storeClientDict.ContainsKey("be_outer_ring_assassin"))
00062: 		{
00063: 			StoreClientListDict.storeClientDict.Add("be_outer_ring_assassin", Assassin);
00064: 		}
00065: 	}
00066: 
00067: 	internal static void RegisterSprites()
00068: 	{
00069: 		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
00070: 		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
00071: 		if ((Object)(object)SpriteDict.Instance == (Object)null)
00072: 		{
00073: 			return;
00074: 		}
00075: 		string[] array = new string[3] { "be_nini", "be_47", "be_shop_47" };
00076: 		foreach (string text in array)
00077: 		{
00078: 			if (!Portraits.TryGetValue(text, out var value) || (Object)(object)value == (Object)null)
00079: 			{
00080: 				Sprite modSprite = CustomUIManager.Instance.GetModSprite("brewing_expansion", text);
00081: 				if ((Object)(object)modSprite == (Object)null)
00082: 				{
00083: 					continue;
00084: 				}
00085: 				value = Sprite.Create(modSprite.texture, modSprite.rect, new Vector2(0.5f, 0.5f), 100f);
00086: 				((Object)value).name = text;
00087: 				Portraits[text] = value;
00088: 			}
00089: 			if (!SpriteDict.Instance.spriteDictionary.ContainsKey(text))
00090: 			{
00091: 				SpriteDict.Instance.spriteDictionary.Add(text, value);
00092: 			}
00093: 		}
00094: 		RegisterFactories();
00095: 	}
00096: 
00097: 	internal static void Loaded()
00098: 	{
00099: 		FormalReputation.Ensure();
00100: 		RegisterSprites();
00101: 		RegisterPhone();
00102: 		NiniCertificates.Retry();
00103: 	}
00104: 
00105: 	internal static void Remove()
00106: 	{
00107: 		PhoneClientList.DialogFuncs.Remove("brewing_expansion.nini");
00108: 		foreach (KeyValuePair<string, Sprite> portrait in Portraits)
00109: 		{
00110: 			if ((Object)(object)SpriteDict.Instance != (Object)null && SpriteDict.Instance.spriteDictionary.TryGetValue(portrait.Key, out var value) && (Object)(object)value == (Object)(object)portrait.Value)
00111: 			{
00112: 				SpriteDict.Instance.spriteDictionary.Remove(portrait.Key);
00113: 			}
00114: 		}
00115: 		foreach (Sprite value5 in Portraits.Values)
00116: 		{
00117: 			if ((Object)(object)value5 != (Object)null)
00118: 			{
00119: 				Object.Destroy((Object)(object)value5);
00120: 			}
00121: 		}
00122: 		Portraits.Clear();
00123: 		if (StoreClientListDict.storeClientDict.TryGetValue("be_weekly_trader", out var value2) && value2 == new Func<StoreClient>(Nini))
00124: 		{
00125: 			StoreClientListDict.storeClientDict.Remove("be_weekly_trader");
00126: 		}
00127: 		if (StoreClientListDict.storeClientDict.TryGetValue("be_seed_trader_47", out var value3) && value3 == new Func<StoreClient>(FortySeven))
00128: 		{
00129: 			StoreClientListDict.storeClientDict.Remove("be_seed_trader_47");
00130: 		}
00131: 		if (StoreClientListDict.storeClientDict.TryGetValue("be_outer_ring_assassin", out var value4) && value4 == new Func<StoreClient>(Assassin))
00132: 		{
00133: 			StoreClientListDict.storeClientDict.Remove("be_outer_ring_assassin");
00134: 		}
00135: 	}
00136: 
00137: 	internal static void RegisterPhone()
00138: 	{
00139: 		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
00140: 		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
00141: 		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
00142: 		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
00143: 		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
00144: 		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
00145: 		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
00146: 		//IL_00f4: Expected O, but got Unknown
00147: 		//IL_00f9: Expected O, but got Unknown
00148: 		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
00149: 		PlayerStore instance = PlayerStore.Instance;
00150: 		if (instance?.PhoneClientDict == null)
00151: 		{
00152: 			return;
00153: 		}
00154: 		PhoneClientList.DialogFuncs["brewing_expansion.nini"] = (StorePhoneClient _) => PhoneDialogue();
00155: 		if (instance.PhoneClientDict.TryGetValue(4747L, out var value))
00156: 		{
00157: 			if (value == null || (value.dialogFuncId != "brewing_expansion.nini" && (!(value.dialogFuncId == "BrewingExpansion.Nini") || !(value.locID == "BrewingExpansion.Nini") || (int)value.phoneClientType != 0)))
00158: 			{
00159: 				return;
00160: 			}
00161: 		}
00162: 		else
00163: 		{
00164: 			if (!instance.FindAllItem(true).Any((GameItem i) => i.identifier == "be_nini_card" && GeneralHelper.IsItemOwned(i)))
00165: 			{
00166: 				return;
00167: 			}
00168: 			Dictionary<long, StorePhoneClient> phoneClientDict = instance.PhoneClientDict;
00169: 			long key = 4747L;
00170: 			StorePhoneClient val = new StorePhoneClient
00171: 			{
00172: 				phoneClientType = (PhoneClientType)0,
00173: 				phoneState = (PhoneState)4,
00174: 				dialedBefore = true
00175: 			};
00176: 			value = val;
00177: 			phoneClientDict.Add(key, val);
00178: 		}
00179: 		value.dialogFuncId = "brewing_expansion.nini";
00180: 		value.locID = "brewing_expansion.nini";
00181: 		value.displayName = ModText.Get("be_nini_name");
00182: 		StorePhoneClient val2 = value;
00183: 		if (val2.alias == null)
00184: 		{
00185: 			val2.alias = new List<string>();
00186: 		}
00187: 		if (!value.alias.Contains("be_weekly_trader"))
00188: 		{
00189: 			value.alias.Add("be_weekly_trader");
00190: 		}
00191: 	}
00192: 
00193: 	internal static Dialogue PhoneDialogue()
00194: 	{
00195: 		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
00196: 		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
00197: 		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
00198: 		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
00199: 		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
00200: 		PlayerStore store = PlayerStore.Instance;
00201: 		int day = StoreStation.Instance.dayCounter;
00202: 		string name = ModText.Get("be_nini_name");
00203: 		Dialogue obj = new Dialogue().SetText(name, ModText.Get("be_nini_phone_greeting"));
00204: 		Dialogue reply = new Dialogue().SetText(name, "");
00205: 		Dialogue nextDialogue = new Dialogue().SetText(name, ModText.Get("be_nini_phone_goodbye"));
00206: 		string text = ModText.Get("be_nini_phone_request");
00207: 		string text2 = ModText.Get("be_nini_phone_end");
00208: 		bool requested = false;
00209: 		obj.AddChoice(DirectoryMaster.Item("$", true), text, "", new Dialogue().SetText("PLAYER", text).SetNextDialogue(reply), (Action)delegate
00210: 		{
00211: 			if (store == PlayerStore.Instance)
00212: 			{
00213: 				int num = day - ModHelper.GetModDataInt("brewing_expansion", "nini_last_call", -999);
00214: 				if (!Open)
00215: 				{
00216: 					reply.SetText(name, ModText.Get("be_nini_phone_closed"));
00217: 				}
00218: 				else if (num < 5)
00219: 				{
00220: 					reply.SetText(name, CallCooldown(5 - num));
00221: 				}
00222: 				else if (!store.FindAllItem(true).Any((GameItem i) => i.identifier == "be_nini_card" && GeneralHelper.IsItemOwned(i)))
00223: 				{
00224: 					reply.SetText(name, ModText.Get("be_nini_phone_missing_card"));
00225: 				}
00226: 				else
00227: 				{
00228: 					requested = true;
00229: 					reply.SetText(name, ModText.Get("be_nini_phone_accepted"));
00230: 				}
00231: 			}
00232: 		}, (Func<bool>)null);
00233: 		obj.AddChoice(DirectoryMaster.Item("$", true), text2, "", new Dialogue().SetText("PLAYER", text2).SetNextDialogue(nextDialogue), (Action)null, (Func<bool>)null);
00234: 		reply.SetEndAction((Action)delegate
00235: 		{
00236: 			if (requested && store == PlayerStore.Instance && StoreStation.Instance.dayCounter == day && Open && day - ModHelper.GetModDataInt("brewing_expansion", "nini_last_call", -999) >= 5 && store.FindAllItem(true).Any((GameItem i) => i.identifier == "be_nini_card" && GeneralHelper.IsItemOwned(i)) && QueueNini(store.storeClientManager))
00237: 			{
00238: 				ModHelper.SetModDataInt("brewing_expansion", "nini_last_call", day);
00239: 			}
00240: 		});
00241: 		return obj;
00242: 	}
00243: 
00244: 	internal static StoreClient Nini()
00245: 	{
00246: 		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
00247: 		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
00248: 		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
00249: 		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
00250: 		StoreClient client = StoreClientList.CreateUpperLowerVisitorLuxury();
00251: 		client.identifier = "be_weekly_trader";
00252: 		client.displayName = ModText.Get("be_nini_name");
00253: 		client.spriteName = "be_nini";
00254: 		client.possibleSprites = new List<string> { "be_nini" };
00255: 		client.bubbleColor = "blue";
00256: 		client.clientIntent = (ClientIntent)1;
00257: 		client.clientBuyingTagList.Clear();
00258: 		client.clientBuyingTagList.Add("SUBSTANCE");
00259: 		client.clientItemFeatureBuying.Clear();
00260: 		client.clientNegociationCheck.Clear();
00261: 		client.clientBuyingIdList.Clear();
00262: 		client.clientBlackTagList.Clear();
00263: 		client.clientBlackIdList.Clear();
00264: 		client.mainDialogue = new Dialogue().SetText(client.displayName, ModText.Get("be_nini_trade_greeting"));
00265: 		client.acceptDealDialogue = new Dialogue().SetText(client.displayName, ModText.Get("be_nini_trade_accepted"));
00266: 		client.allDoneDialogue = new Dialogue().SetText(client.displayName, ModText.Get("be_nini_trade_done"));
00267: 		client.clientFaction = "BE_FACTION_OUTER_RING";
00268: 		client.SetBudget(FormalReputation.NiniBudget);
00269: 		client.clientCash = FormalReputation.NiniBudget;
00270: 		client.useClientBudget = true;
00271: 		client.isIntroduced = true;
00272: 		client.onItemBought = delegate(GameItem item, int _)
00273: 		{
00274: 			NiniCertificates.Sold(item, client);
00275: 		};
00276: 		client.OnArrival = delegate
00277: 		{
00278: 			RegisterSprites();
00279: 			NiniCertificates.Retry(client);
00280: 		};
00281: 		ModHelper.SetClientSource(client, "brewing_expansion");
00282: 		return client;
00283: 	}
00284: 
00285: 	internal static StoreClient FortySeven()
00286: 	{
00287: 		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
00288: 		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
00289: 		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
00290: 		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
00291: 		StoreClient val = StoreClientList.CreateUpperLowerVisitorLuxury();
00292: 		val.identifier = "be_seed_trader_47";
00293: 		val.displayName = ModText.Get("be_forty_seven_name");
00294: 		val.spriteName = "be_47";
00295: 		val.possibleSprites = new List<string> { "be_47" };
00296: 		val.clientIntent = (ClientIntent)2;
00297: 		val.isIntroduced = true;
00298: 		val.clientFaction = "BE_FACTION_OUTER_RING";
00299: 		val.bubbleColor = "blue";
00300: 		val.mainDialogue = new Dialogue().SetText(val.displayName, ModText.Get("be_forty_seven_greeting"));
00301: 		val.acceptDealDialogue = new Dialogue().SetText(val.displayName, ModText.Get("be_forty_seven_trade_accepted"));
00302: 		val.allDoneDialogue = new Dialogue().SetText(val.displayName, ModText.Get("be_forty_seven_trade_done"));
00303: 		val.OnArrival = StockSeeds;
00304: 		val.OnDismiss = DismissSeedStock;
00305: 		ModHelper.SetClientSource(val, "brewing_expansion");
00306: 		return val;
00307: 	}
00308: 
00309: 	internal static StoreClient Assassin()
00310: 	{
00311: 		StoreClient val = StoreClientListMorning.CreateULHitman();
00312: 		val.identifier = "be_outer_ring_assassin";
00313: 		val.clientFaction = "BE_FACTION_OUTER_RING";
00314: 		val.displayName = ModText.Get("be_assassin_name");
00315: 		val.mainDialogue.SetText(val.displayName, ModText.Get("be_assassin_greeting"));
00316: 		ModHelper.SetClientSource(val, "brewing_expansion");
00317: 		return val;
00318: 	}
00319: 
00320: 	internal static int CertificateLevel()
00321: 	{
00322: 		PlayerStore instance = PlayerStore.Instance;
00323: 		return (from i in ((instance != null) ? instance.FindAllItem(true) : null)?.Where((Func<GameItem, bool>)GeneralHelper.IsItemOwned)
00324: 			select Array.IndexOf(BrewingCertificates.Ids, i.identifier) + 1).DefaultIfEmpty().Max() ?? 0;
00325: 	}
00326: 
00327: 	internal static int SeedCount()
00328: 	{
00329: 		int num = (new int[4] { 3, 7, 9, 13 })[CertificateLevel()] + FormalReputation.SeedCountModifier;
00330: 		if (FormalTraits.Has("be_seed_aversion"))
00331: 		{
00332: 			num -= 2;
00333: 		}
00334: 		if (PlayerStore.Instance.FindAllItem(true).Any((GameItem i) => i.identifier == "be_xuxu_badge" && GeneralHelper.IsItemOwned(i)))
00335: 		{
00336: 			num++;
00337: 		}
00338: 		return Math.Max(0, num);
00339: 	}
00340: 
00341: 	internal static void StockSeeds()
00342: 	{
00343: 		RegisterSprites();
00344: 		if (!ModEntry.Enabled || !Open || FormalVisitorPersistence.ResumeStock())
00345: 		{
00346: 			return;
00347: 		}
00348: 		int dayCounter = StoreStation.Instance.dayCounter;
00349: 		if (ModHelper.GetModDataInt("brewing_expansion", "47_stock_day", -1) == dayCounter)
00350: 		{
00351: 			return;
00352: 		}
00353: 		PlayerStore instance = PlayerStore.Instance;
00354: 		Random random = new Random();
00355: 		int num = SeedCount();
00356: 		int num2 = ((ModHelper.GetModDataInt("brewing_expansion", "47_stock_count_day", -1) == dayCounter) ? ModHelper.GetModDataInt("brewing_expansion", "47_stock_count", 0) : 0);
00357: 		for (int i = num2; i < num; i++)
00358: 		{
00359: 			if (instance != PlayerStore.Instance)
00360: 			{
00361: 				return;
00362: 			}
00363: 			int modDataInt = ModHelper.GetModDataInt("brewing_expansion", "longing_misses", 0);
00364: 			int modDataInt2 = ModHelper.GetModDataInt("brewing_expansion", "afterglow_misses", 0);
00365: 			int level = CertificateLevel();
00366: 			(int, int, int) tuple = SeedOdds(modDataInt, modDataInt2, level);
00367: 			int num3 = random.Next(10000);
00368: 			int num4 = ((num3 >= tuple.Item1) ? ((num3 < tuple.Item1 + 1500) ? 1 : ((num3 < tuple.Item1 + 1500 + tuple.Item2) ? 2 : 3)) : 0);
00369: 			int num5 = num4 switch
00370: 			{
00371: 				2 => 12 + random.Next(2), 
00372: 				1 => 9 + random.Next(3), 
00373: 				0 => random.Next(9), 
00374: 				_ => 14 + random.Next(2), 
00375: 			};
00376: 			GameItem val = DirectoryMaster.Item(BrewingSeeds.All[num5].Id, false);
00377: 			val.EnableTag("BE_47_STOCK", false);
00378: 			instance.AddDirectSellingItemToTable(val, false, false, false, 0);
00379: 			if (val.parentInventory == null)
00380: 			{
00381: 				if (MachineConnections.Live(val))
00382: 				{
00383: 					val.Destroy();
00384: 				}
00385: 			}
00386: 			else
00387: 			{
00388: 				num2++;
00389: 				ModHelper.SetModDataInt("brewing_expansion", "47_stock_count_day", dayCounter);
00390: 				ModHelper.SetModDataInt("brewing_expansion", "47_stock_count", num2);
00391: 				ModHelper.SetModDataInt("brewing_expansion", "longing_misses", (num4 < 2) ? Math.Min(1000, modDataInt + 1) : 0);
00392: 				ModHelper.SetModDataInt("brewing_expansion", "afterglow_misses", (num4 != 3) ? Math.Min(1000, modDataInt2 + 1) : 0);
00393: 			}
00394: 		}
00395: 		HashSet<string> owned = new HashSet<string>(from val2 in ((IEnumerable<GameItem>)instance.FindAllItem(true)).Where((Func<GameItem, bool>)GeneralHelper.IsItemOwned)
00396: 			select val2.identifier);
00397: 		RemainingItems.Definition[] array = RemainingItems.All.Where((RemainingItems.Definition d) => d.Kind == "Recipe" && !owned.Contains(d.Id)).ToArray();
00398: 		if (array.Length != 0 && ModHelper.GetModDataInt("brewing_expansion", "47_recipe_day", -1) != dayCounter)
00399: 		{
00400: 			StockDocument(array[random.Next(array.Length)].Id, "47_recipe_day", instance, dayCounter);
00401: 		}
00402: 		if (!owned.Contains("be_brewing_manual") && ModHelper.GetModDataInt("brewing_expansion", "47_manual_day", -1) != dayCounter)
00403: 		{
00404: 			StockDocument("be_brewing_manual", "47_manual_day", instance, dayCounter);
~~~

~~~csharp
00420: 		}
00421: 		else if (MachineConnections.Live(val))
00422: 		{
00423: 			val.Destroy();
00424: 		}
00425: 	}
00426: 
00427: 	private static void DismissSeedStock()
00428: 	{
00429: 		if (PlayerStore.Instance == null)
00430: 		{
00431: 			return;
00432: 		}
00433: 		EmporiumEntry instance = EmporiumEntry.Instance;
00434: 		GameGridInventory val = ((instance != null) ? instance.frontInvinvElement : null);
00435: 		if (val == null)
00436: 		{
00437: 			return;
00438: 		}
00439: 		GameItem[] array = ((GameInventory)val).childItems.Where((GameItem i) => i.IsTag("BE_47_STOCK") && !GeneralHelper.IsItemOwned(i)).ToArray();
00440: 		foreach (GameItem val2 in array)
00441: 		{
00442: 			if (MachineConnections.Live(val2))
00443: 			{
00444: 				val2.Destroy();
00445: 			}
00446: 		}
00447: 		ModHelper.SetModDataInt("brewing_expansion", "47_stock_day", StoreStation.Instance.dayCounter);
00448: 	}
00449: 
00450: 	internal static (int, int, int) SeedOdds(int longingMisses, int afterglowMisses, int level)
00451: 	{
00452: 		int num = ((level >= 3) ? 16 : ((level >= 2) ? 13 : 10));
00453: 		int num2 = (300 + Math.Min(Math.Max(0, longingMisses) / 10, 5) * 100) * num / 10;
00454: 		int num3 = (100 + Math.Min(Math.Max(0, afterglowMisses) / 20, 8) * 50) * num / 10;
00455: 		int num4 = 8500 - num2 - num3;
00456: 		if (num4 < 5000)
00457: 		{
00458: 			num2 = num2 * 3500 / (num2 + num3);
00459: 			num3 = 3500 - num2;
00460: 			num4 = 5000;
00461: 		}
00462: 		return (num4, num2, num3);
00463: 	}
00464: 
00465: 	internal static bool QueueNini(StoreClientManager manager)
00466: 	{
00467: 		if (ModEntry.Enabled && Open && manager != null && !manager.clientStack.Any((StoreClient c) => c.identifier == "be_weekly_trader"))
00468: 		{
00469: 			StoreClientInstance currentClientInstance = PlayerStore.Instance.currentClientInstance;
00470: 			if (!(((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.identifier) == "be_weekly_trader"))
00471: 			{
00472: 				manager.AddClient(Nini());
00473: 				return true;
00474: 			}
00475: 		}
00476: 		return false;
00477: 	}
00478: 
00479: 	internal static void Generated(StoreClientManager manager)
00480: 	{
00481: 		Loaded();
00482: 		if (FormalReputation.Tier <= -5)
00483: 		{
00484: 			StoreClientInstance currentClientInstance = PlayerStore.Instance.currentClientInstance;
00485: 			if (((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.identifier) != "be_outer_ring_assassin" && !manager.clientStack.Any((StoreClient c) => c.identifier == "be_outer_ring_assassin"))
00486: 			{
00487: 				StoreClient val = Assassin();
00488: 				manager.AddClient(val);
00489: 				manager.clientStack.Remove(val);
00490: 				manager.clientStack.Insert(0, val);
00491: 			}
00492: 		}
00493: 	}
00494: 
00495: 	internal static void EnsureFortySeven(PlayerStore store, bool allowNew = true)
00496: 	{
00497: 		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
00498: 		//IL_00c7: Invalid comparison between Unknown and I4
00499: 		if (!ModEntry.Enabled || store != PlayerStore.Instance || store?.storeClientManager == null)
00500: 		{
00501: 			return;
00502: 		}
00503: 		List<StoreClient> clientStack = store.storeClientManager.clientStack;
00504: 		StoreClientInstance currentClientInstance = store.currentClientInstance;
00505: 		bool flag = ((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.identifier) == "be_seed_trader_47";
00506: 		int num = 0;
00507: 		while (num < clientStack.Count)
00508: 		{
00509: 			if (clientStack[num]?.identifier != "be_seed_trader_47")
00510: 			{
00511: 				num++;
00512: 				continue;
00513: 			}
00514: 			if (flag)
00515: 			{
00516: 				clientStack.RemoveAt(num);
00517: 				continue;
00518: 			}
00519: 			flag = true;
00520: 			num++;
00521: 		}
00522: 		int dayCounter = StoreStation.Instance.dayCounter;
00523: 		if (flag)
00524: 		{
00525: 			ModHelper.SetModDataInt("brewing_expansion", "47_queue_day", dayCounter);
00526: 		}
00527: 		else if (allowNew && (Open || (int)store.storeState == 2) && ModHelper.GetModDataInt("brewing_expansion", "47_queue_day", -1) != dayCounter && ModHelper.GetModDataInt("brewing_expansion", "47_stock_day", -1) != dayCounter && FormalReputation.Tier > -3 && ShowcaseHelper.IsShowcaseContainItemById("be_brewery_sign"))
00528: 		{
00529: 			store.storeClientManager.AddClient(FortySeven());
00530: 			if (clientStack.Any((StoreClient c) => c?.identifier == "be_seed_trader_47"))
00531: 			{
00532: 				ModHelper.SetModDataInt("brewing_expansion", "47_queue_day", dayCounter);
00533: 			}
00534: 		}
00535: 	}
00536: }
~~~

### BrewingExpansion/Formal/NiniCertificates.cs

~~~csharp
00001: using System;
00002: using System.Globalization;
00003: using UnityEngine;
00004: 
00005: namespace BrewingExpansion.Formal;
00006: 
00007: internal static class NiniCertificates
00008: {
00009: 	private const string CreditKey = "nini_certificate_credit";
00010: 
00011: 	private static readonly long[] Costs = new long[3] { 1500L, 3000L, 8000L };
00012: 
00013: 	private static bool issuing;
00014: 
00015: 	internal static void Sold(GameItem item, StoreClient client)
00016: 	{
00017: 		if (!ModEntry.Enabled || item == null || client?.identifier != "be_weekly_trader")
00018: 		{
00019: 			return;
00020: 		}
00021: 		try
00022: 		{
00023: 			long num = Math.Max(0L, item.GetBaseValue());
00024: 			long num2 = ReadCredit();
00025: 			WriteCredit((num2 > long.MaxValue - num) ? long.MaxValue : (num2 + num));
00026: 			Issue(client, reconcileOwned: false);
00027: 		}
00028: 		catch (Exception error)
00029: 		{
00030: 			Warn(error);
00031: 		}
00032: 	}
00033: 
00034: 	internal static void Retry(StoreClient client = null)
00035: 	{
00036: 		Issue(client, reconcileOwned: true);
00037: 	}
00038: 
00039: 	private static void Issue(StoreClient client, bool reconcileOwned)
00040: 	{
00041: 		if (!ModEntry.Enabled || issuing || PlayerStore.Instance == null || (Object)(object)EmporiumEntry.Instance == (Object)null)
00042: 		{
00043: 			return;
00044: 		}
00045: 		issuing = true;
00046: 		GameItem val = null;
00047: 		try
00048: 		{
00049: 			int i;
00050: 			for (i = 0; i < Costs.Length && ModHelper.GetModDataBool("brewing_expansion", IssuedKey(i), false); i++)
00051: 			{
00052: 			}
00053: 			if (i == Costs.Length)
00054: 			{
00055: 				return;
00056: 			}
00057: 			long num = ReadCredit();
00058: 			if (!reconcileOwned && num < Costs[i])
00059: 			{
00060: 				return;
00061: 			}
00062: 			int num2 = FormalVisitors.CertificateLevel();
00063: 			for (int j = 0; j < num2; j++)
00064: 			{
00065: 				ModHelper.SetModDataBool("brewing_expansion", IssuedKey(j), true);
00066: 			}
00067: 			for (int k = 0; k < Costs.Length; k++)
00068: 			{
00069: 				if (!ModHelper.GetModDataBool("brewing_expansion", IssuedKey(k), false))
00070: 				{
00071: 					if (num < Costs[k])
00072: 					{
00073: 						break;
00074: 					}
00075: 					val = ModHelper.SpawnItem(BrewingCertificates.Ids[k]);
00076: 					if (val == null || val.identifier != BrewingCertificates.Ids[k])
00077: 					{
00078: 						throw new InvalidOperationException("Certificate factory did not create " + BrewingCertificates.Ids[k]);
00079: 					}
00080: 					GameGridInventory frontInvinvElement = EmporiumEntry.Instance.frontInvinvElement;
00081: 					SlotMarker val2 = ((frontInvinvElement != null) ? frontInvinvElement.TryFindOneValidInventorySlot(val, false) : null);
00082: 					if (val2 != null && val2.IsValid())
00083: 					{
00084: 						val2.TryAcceptOnce(-1);
00085: 					}
00086: 					if (val.parentInventory == null)
00087: 					{
00088: 						Tell(client, pending: true);
00089: 						break;
00090: 					}
00091: 					val = null;
00092: 					num = ReadCredit() - Costs[k];
00093: 					ModHelper.SetModDataBool("brewing_expansion", IssuedKey(k), true);
00094: 					WriteCredit(num);
00095: 					Tell(client, pending: false);
00096: 				}
~~~

~~~csharp
00099: 		catch (Exception error)
00100: 		{
00101: 			Warn(error);
00102: 		}
00103: 		finally
00104: 		{
00105: 			try
00106: 			{
00107: 				if (val != null && val.parentInventory == null)
00108: 				{
00109: 					val.Destroy();
00110: 				}
00111: 			}
00112: 			catch (Exception error2)
00113: 			{
00114: 				Warn(error2);
00115: 			}
00116: 			finally
00117: 			{
00118: 				issuing = false;
00119: 			}
00120: 		}
00121: 	}
00122: 
00123: 	private static string IssuedKey(int n)
00124: 	{
00125: 		return "certificate_issued_" + n;
00126: 	}
00127: 
00128: 	private static long ReadCredit()
00129: 	{
00130: 		long.TryParse(ModHelper.GetModData("brewing_expansion", "nini_certificate_credit", "0"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result);
00131: 		return Math.Max(0L, result);
00132: 	}
00133: 
00134: 	private static void WriteCredit(long credit)
00135: 	{
00136: 		ModHelper.SetModData("brewing_expansion", "nini_certificate_credit", credit.ToString(CultureInfo.InvariantCulture));
00137: 	}
00138: 
00139: 	private static void Tell(StoreClient client, bool pending)
00140: 	{
00141: 		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
00142: 		if (!(client?.identifier != "be_weekly_trader"))
00143: 		{
00144: 			client.acceptDealDialogue = new Dialogue().SetText(client.displayName, pending ? ModText.Get("be_certificate_delivery_pending") : ModText.Get("be_certificate_awarded"));
00145: 			client.acceptDealDialoguePlayed = false;
00146: 		}
00147: 	}
00148: 
00149: 	private static void Warn(Exception error)
00150: 	{
00151: 		Debug.LogWarning((object)("[BrewingExpansion] Certificate processing deferred; saved progress is preserved: " + error));
00152: 	}
00153: }
~~~

### BrewingExpansion/Formal/RemainingConsumables.cs

~~~csharp
00040: 	}
00041: 
00042: 	internal static bool CanUse(GameItem item)
00043: 	{
00044: 		RemainingItems.Definition definition = RemainingItems.Find((item != null) ? item.identifier : null);
00045: 		if (!ModEntry.Enabled || definition == null || Busy.Contains(item) || !MachineConnections.Owned(item))
00046: 		{
00047: 			return false;
00048: 		}
00049: 		if (!(definition.Kind != "Perfume"))
00050: 		{
00051: 			if (UseCountHelper.CanUse(item, 1))
00052: 			{
00053: 				if (!(definition.Id != "be_trade_perfume"))
00054: 				{
00055: 					if (Client != null)
00056: 					{
00057: 						return Client.perfumeInfluence == 0;
00058: 					}
00059: 					return false;
00060: 				}
00061: 				return true;
00062: 			}
00063: 			return false;
00064: 		}
00065: 		return true;
00066: 	}
00067: 
00068: 	internal static bool Use(GameItem item)
00069: 	{
00070: 		if (!CanUse(item) || !Busy.Add(item))
00071: 		{
00072: 			return false;
00073: 		}
00074: 		PlayerStore instance = PlayerStore.Instance;
00075: 		try
00076: 		{
00077: 			RemainingItems.Definition definition = RemainingItems.Find(item.identifier);
00078: 			if (definition.Kind == "Box")
00079: 			{
00080: 				return Open(item, instance);
00081: 			}
00082: 			if (definition.Kind == "Card")
00083: 			{
00084: 				ModHelper.SetModDataBool("brewing_expansion", "nini_contact", true);
00085: 				FormalVisitors.RegisterPhone();
00086: 				return true;
00087: 			}
00088: 			if (definition.Kind != "Perfume")
00089: 			{
00090: 				return false;
00091: 			}
00092: 			StoreClient client = Client;
00093: 			int num = item.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt();
00094: 			UseCountHelper.Use(item, 1);
00095: 			if (instance != PlayerStore.Instance || item.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt() != num - 1)
00096: 			{
00097: 				return false;
00098: 			}
00099: 			if (item.identifier == "be_trade_perfume")
00100: 			{
00101: 				if (Client != client || client == null)
00102: 				{
00103: 					return false;
00104: 				}
00105: 				client.perfumeInfluence = 3;
00106: 				instance.RefreshCounterItem();
00107: 			}
00108: 			else
00109: 			{
00110: 				ReduceCrime(instance, (item.identifier == "be_freedom_perfume") ? 2000 : 20000);
00111: 			}
00112: 			AudioManager instance2 = AudioManager.Instance;
00113: 			if (instance2 != null)
00114: 			{
00115: 				instance2.Play2("perfume_spray");
00116: 			}
00117: 			return true;
00118: 		}
00119: 		catch (Exception ex)
00120: 		{
00121: 			Debug.LogWarning((object)("[BrewingExpansion.Formal] Item use interrupted: " + ex));
00122: 			return false;
00123: 		}
00124: 		finally
00125: 		{
00126: 			Busy.Remove(item);
00127: 		}
00128: 	}
00129: 
00130: 	private static void ReduceCrime(PlayerStore store, int amount)
00131: 	{
00132: 		SecData secData = store.secData;
00133: 		if (secData == null)
00134: 		{
00135: 			return;
00136: 		}
00137: 		int num = amount;
00138: 		for (int i = 0; i < secData.crimeAmount.Count; i++)
00139: 		{
00140: 			if (num <= 0)
00141: 			{
00142: 				break;
00143: 			}
00144: 			int num2 = Math.Min(num, Math.Max(0, secData.crimeAmount[i]));
00145: 			secData.crimeAmount[i] -= num2;
00146: 			num -= num2;
00147: 		}
00148: 		num = amount;
00149: 		for (int j = 0; j < secData.crimeAmountTotal.Count; j++)
00150: 		{
00151: 			if (num <= 0)
00152: 			{
00153: 				break;
00154: 			}
00155: 			int num3 = Math.Min(num, Math.Max(0, secData.crimeAmountTotal[j]));
00156: 			secData.crimeAmountTotal[j] -= num3;
00157: 			num -= num3;
00158: 		}
00159: 	}
00160: 
00161: 	internal static string[] Rewards(string id)
00162: 	{
00163: 		return id switch
00164: 		{
00165: 			"be_low_blindbox" => Seed(0, 9), 
00166: 			"be_mid_blindbox" => Seed(9, 3), 
00167: 			"be_premium_blindbox" => Seed(12, 2), 
00168: 			"be_afterglow_blindbox" => Seed(14, 2), 
00169: 			"be_wine_blindbox" => new string[1] { RemainingItems.All.Where((RemainingItems.Definition x) => x.Kind == "Collector").ElementAt(Random.Next(10)).Id }, 
00170: 			"be_figurine_blindbox" => new string[1] { Figurines.All[Random.Next(Figurines.All.Length)].Id }, 
00171: 			"be_container_blindbox" => new string[1] { Pick(new string[7] { "be_laolishi_bag", "be_cyber_bag", "be_sakura_bag", "be_cat_yoyou_satchel", "be_xuxu_warm_cup", "be_xuxu_sweet_cup", "be_47_love_cup" }, new int[7] { 25, 15, 5, 5, 30, 15, 5 }) }, 
00172: 			"be_midautumn_blindbox" => new string[1] { Pick(new string[7] { "be_midautumn_moon_fan", "be_midautumn_moon_lamp", "be_midautumn_jade_rabbit", "be_midautumn_lantern", "be_midautumn_moon_osmanthus", "be_midautumn_mooncake_box", "be_midautumn_bag" }, new int[7] { 10, 10, 10, 10, 25, 30, 5 }) }, 
00173: 			"be_midautumn_mooncake_box" => new string[1] { "be_wuren_mooncake" }, 
00174: 			"be_xuxu_cassette_bundle" => FormalMusic.Ids, 
~~~

## card

### GameCardPK/AuxEffectSystem.cs

~~~csharp
00028: 	internal static string ReadPayload(GameItem? card)
00029: 	{
00030: 		if (card == null)
00031: 		{
00032: 			return string.Empty;
00033: 		}
00034: 		try
00035: 		{
00036: 			if (!card.IsTag(AuxEffectTag))
00037: 			{
00038: 				return string.Empty;
00039: 			}
00040: 			TagState tagReadonly = card.GetTagReadonly(AuxEffectTag);
00041: 			return Core.Clean((tagReadonly != null) ? tagReadonly.GetString() : null);
00042: 		}
00043: 		catch (Exception ex)
00044: 		{
00045: 			Core.Debug("[辅助效果] 读卡上辅助效果标签失败（按没有处理）：" + ex.Message);
00046: 			return string.Empty;
00047: 		}
00048: 	}
00049: 
00050: 	internal static int CountOf(GameItem? card)
00051: 	{
00052: 		return DragTagCodec.ParseIds(ReadPayload(card)).Count;
00053: 	}
00054: 
00055: 	internal static int CountOfItem(GameItem? card, string itemId)
00056: 	{
00057: 		return DragTagCodec.CountOf(ReadPayload(card), itemId);
00058: 	}
00059: 
00060: 	internal static Applied? TryAttach(GameItem? item, GameItem? card)
00061: 	{
00062: 		if (item == null || card == null)
00063: 		{
00064: 			return null;
00065: 		}
00066: 		AuxEffectData byItemId = AuxEffectTable.GetByItemId(item.identifier);
00067: 		if (byItemId == null)
00068: 		{
00069: 			return null;
00070: 		}
00071: 		if (!GameCardFactory.IsCard(card))
00072: 		{
00073: 			Core.Log?.Msg("[辅助效果] 落点上没有 game_card 标签，不是游戏卡，不处理：" + CardIdentity.Of(card));
00074: 			return null;
00075: 		}
00076: 		if (byItemId.Target != DragReceiver.GameCard)
00077: 		{
00078: 			Core.Log?.Error("[辅助效果] " + byItemId.Id + " 的 target 配成了 " + byItemId.Target.ToString() + "，但辅助效果只能落在游戏卡上，已忽略。");
00079: 			return null;
00080: 		}
00081: 		if (!AuxEffectTable.IsUsable(byItemId))
00082: 		{
00083: 			Core.Debug("[辅助效果] " + byItemId.Id + " 没配齐（effects 为空或 consume 不可用），拖上去也不会有任何效果，不消耗。");
00084: 			return null;
00085: 		}
00086: 		string text = ReadPayload(card);
00087: 		List<string> list = DragTagCodec.ParseIds(text);
00088: 		if (!ItemConsumption.CanConsume(item, byItemId.Consume))
00089: 		{
00090: 			Core.Log?.Msg("[辅助效果]「" + byItemId.DisplayText + "」施加者不满足消耗条件（" + byItemId.Consume.Describe() + "），这次拖拽不生效。");
00091: 			return null;
00092: 		}
00093: 		int maxAuxEffectsPerCard = CardDatabase.MaxAuxEffectsPerCard;
00094: 		if (list.Count >= maxAuxEffectsPerCard)
00095: 		{
00096: 			Core.Log?.Msg("[辅助效果]「" + byItemId.DisplayText + "」拖上去无效：这张卡已经有 " + list.Count + " 条辅助效果，上限 " + maxAuxEffectsPerCard + " 条。");
00097: 			return null;
00098: 		}
00099: 		list.Add(byItemId.ItemId);
00100: 		string text2 = DragTagCodec.FormatIds(list);
00101: 		GameCardFactory.ChangeTagString("AuxEffect", card, text2);
00102: 		string text3 = ReadPayload(card);
00103: 		int count = DragTagCodec.ParseIds(text3).Count;
00104: 		if (!string.Equals(text3, text2, StringComparison.Ordinal) || count != list.Count)
00105: 		{
00106: 			Core.Log?.Error("[辅助效果] 卡上标签没写进去（期望「" + text2 + "」实际「" + text3 + "」，条目 " + list.Count + "→" + count + "），这次附着已撤回，物品留着。");
00107: 			Restore(card, text);
00108: 			return null;
00109: 		}
00110: 		Core.Debug("[辅助效果]「" + byItemId.DisplayText + "」已记在卡上（" + list.Count + "/" + maxAuxEffectsPerCard + "）：" + text2);
00111: 		return new Applied
00112: 		{
00113: 			Card = card,
00114: 			ItemId = byItemId.ItemId,
00115: 			BeforePayload = text,
00116: 			Spec = byItemId.Consume
00117: 		};
00118: 	}
00119: 
00120: 	internal static void Consume(GameItem? item, Applied? applied, ConsumeData? spec = null)
00121: 	{
00122: 		if (item != null && !ItemConsumption.Consume(item, spec))
00123: 		{
00124: 			Core.Debug("[辅助效果] 施加者没能消耗，卡上标记已撤回。");
00125: 			Rollback(applied);
00126: 		}
00127: 	}
00128: 
00129: 	private static void Restore(GameItem? card, string payload)
00130: 	{
00131: 		if (card == null)
00132: 		{
00133: 			return;
00134: 		}
00135: 		try
00136: 		{
00137: 			if (string.IsNullOrEmpty(payload))
00138: 			{
00139: 				if (card.IsTag(AuxEffectTag))
00140: 				{
00141: 					card.DisableTag(AuxEffectTag, false);
00142: 				}
00143: 			}
00144: 			else
00145: 			{
00146: 				GameCardFactory.ChangeTagString("AuxEffect", card, payload);
00147: 			}
00148: 		}
00149: 		catch (Exception ex)
00150: 		{
00151: 			Core.Log?.Error("[辅助效果] 回滚卡上标签失败：" + ex.Message);
00152: 		}
00153: 	}
00154: 
00155: 	private static void Rollback(Applied? applied)
00156: 	{
00157: 		if (applied?.Card != null)
00158: 		{
00159: 			Restore(applied.Card, applied.BeforePayload);
00160: 		}
00161: 	}
00162: 
00163: 	internal static void ApplyToBattle(BattleContext? ctx, GameItem? card)
00164: 	{
00165: 		if (ctx == null || card == null)
00166: 		{
00167: 			return;
~~~

### GameCardPK/CardDatabase.cs

~~~csharp
00334: 			Core.Log?.Warning("[卡牌PK] cards.json 的 skillSlots = " + raw + " 小于默认 " + 8 + "：老存档里多出来的技能条会失去槽位（本项目不做迁移）。");
00335: 		}
00336: 		return raw;
00337: 	}
00338: 
00339: 	private static int ClampAuxEffects(int raw)
00340: 	{
00341: 		if (raw <= 0)
00342: 		{
00343: 			Core.Debug("[卡牌PK] cards.json 的 maxAuxEffectsPerCard 没写或非正数，按默认 " + 3 + " 处理。");
00344: 			return 3;
00345: 		}
00346: 		if (raw > 16)
00347: 		{
00348: 			Core.Log?.Error("[卡牌PK] cards.json 的 maxAuxEffectsPerCard = " + raw + " 超过上限 " + 16 + "，已夹到上限。");
00349: 			return 16;
00350: 		}
00351: 		return raw;
00352: 	}
00353: 
00354: 	internal static GameCardData? GetCard(string? id)
00355: 	{
00356: 		if (string.IsNullOrEmpty(id))
00357: 		{
00358: 			return null;
00359: 		}
00360: 		EnsureLoaded();
00361: 		string key = ItemConfigInjector.PrefixedItem(id);
00362: 		if (!_cards.TryGetValue(key, out GameCardData value))
00363: 		{
00364: 			return null;
00365: 		}
00366: 		return value;
00367: 	}
00368: 
00369: 	internal static GameCardData GetFirstOrDefault()
00370: 	{
00371: 		EnsureLoaded();
00372: 		if (_order.Count > 0)
00373: 		{
00374: 			return _order[0];
00375: 		}
00376: 		return new GameCardData
00377: 		{
00378: 			Id = ItemConfigInjector.PrefixedItem("game_card"),
00379: 			Name = "游戏卡"
00380: 		};
00381: 	}
00382: 
00383: 	internal static void Register(GameCardData data)
00384: 	{
00385: 		if (data == null || string.IsNullOrEmpty(data.Id))
00386: 		{
00387: 			return;
00388: 		}
00389: 		EnsureLoaded();
00390: 		data.Id = ItemConfigInjector.PrefixedItem(data.Id);
00391: 		if (data.Skills != null)
00392: 		{
00393: 			for (int i = 0; i < data.Skills.Count; i++)
00394: 			{
00395: 				data.Skills[i] = ItemConfigInjector.PrefixedItem(data.Skills[i]);
00396: 			}
00397: 		}
00398: 		if (!_cards.ContainsKey(data.Id))
00399: 		{
00400: 			_order.Add(data);
00401: 		}
00402: 		_cards[data.Id] = data;
00403: 	}
00404: 
00405: 	internal static string? GetDefaultSkill(string? cardId, int slotOneBased)
00406: 	{
00407: 		return GetCard(cardId)?.GetSkillAt(slotOneBased);
00408: 	}
00409: 
00410: 	private static void LoadCardKinds(List<CardKindData>? kinds)
00411: 	{
00412: 		_cardKinds.Clear();
00413: 		if (kinds == null || kinds.Count == 0)
00414: 		{
00415: 			Core.Debug("[卡牌PK] cards.json 没写 cardKinds —— 错版卡 / 破损卡这类造物入口不存在。");
00416: 			return;
00417: 		}
00418: 		foreach (CardKindData kind in kinds)
00419: 		{
00420: 			if (kind == null)
00421: 			{
00422: 				continue;
00423: 			}
00424: 			string text = ItemConfigInjector.BareId(Core.Clean(kind.Id));
00425: 			if (text.Length == 0)
00426: 			{
00427: 				Core.Log?.Error("[卡牌PK] cardKinds 里有一条没写 id，已跳过。");
00428: 				continue;
00429: 			}
00430: 			kind.Id = text;
00431: 			kind.Name = Core.Clean(kind.Name);
00432: 			if (kind.Name.Length == 0)
00433: 			{
00434: 				kind.Name = text;
00435: 			}
00436: 			kind.Description = Core.Clean(kind.Description);
00437: 			kind.Flavor = Core.Clean(kind.Flavor);
00438: 			kind.Sprite = Core.Clean(kind.Sprite);
00439: 			if (kind.Sprite.Length == 0)
00440: 			{
00441: 				kind.Sprite = text;
00442: 			}
00443: 			kind.ItemType = ItemConfigInjector.BareId(Core.Clean(kind.ItemType));
00444: 			kind.ReferenceLootTable = Core.Clean(kind.ReferenceLootTable);
00445: 			if (kind.Weight <= 0)
00446: 			{
00447: 				kind.Weight = 100;
00448: 			}
00449: 			NormalizeKindVersions(kind);
00450: 			_cardKinds[text] = kind;
00451: 		}
00452: 		Core.Log?.Msg("[卡牌PK] 已加载印刷性质卡 " + _cardKinds.Count + " 类（" + string.Join("/", _cardKinds.Keys) + "）。");
00453: 	}
00454: 
00455: 	private static void NormalizeKindVersions(CardKindData kind)
00456: 	{
00457: 		if (kind.Versions == null || kind.Versions.Count == 0)
00458: 		{
00459: 			if (kind.ItemType.Length == 0 && Core.Clean(kind.ReferenceLootTable).Length == 0)
00460: 			{
00461: 				Core.Log?.Error("[卡牌PK] 性质卡「" + kind.Id + "」没写 itemType、没写 versions，也没配 referenceLootTable ⇒ 造出来就是普通卡。");
00462: 			}
00463: 			kind.Versions = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
00464: 			return;
00465: 		}
00466: 		Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
00467: 		foreach (KeyValuePair<string, int> version in kind.Versions)
00468: 		{
~~~

### GameCardPK/CardEvents.cs

~~~csharp
00166: 				StoreEvent val2 = activeEvents[i];
00167: 				if (val2 != null && IsCardEvent(val2.identifier))
00168: 				{
00169: 					list.Add(val2.displayName + "（剩 " + val2.duration + " 天）");
00170: 				}
00171: 			}
00172: 			return (list.Count == 0) ? "事件状态：三条卡牌事件都没在生效。" : ("事件状态：" + string.Join("；", list));
00173: 		}
00174: 		catch (Exception ex)
00175: 		{
00176: 			return "读事件状态失败：" + ex.Message;
00177: 		}
00178: 	}
00179: 
00180: 	private static bool IsCardEvent(string? identifier)
00181: 	{
00182: 		string text = Core.Clean(identifier);
00183: 		if (!(text == "cardpkg_memorial_release") && !(text == "cardpkg_misprint_flood"))
00184: 		{
00185: 			return text == "cardpkg_misprint_shortage";
00186: 		}
00187: 		return true;
00188: 	}
00189: 
00190: 	private static StoreEvent BuildMemorialRelease()
00191: 	{
00192: 		StoreEvent obj = NewEvent("cardpkg_memorial_release", 1, 6, "纪念版卡包发行", "纪念版卡包发行", "发行方在站内放出一批纪念版卡包，包装和普通包一样，只多一道烫金封条。消息传开以后，每天都有人揣着几包来碰运气。");
00193: 		obj.addClientFromEventActionId = "cardpkg_memorial_releaseaddClientAction";
00194: 		return obj;
00195: 	}
00196: 
00197: 	private static StoreEvent BuildMisprintFlood()
00198: 	{
00199: 		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
00200: 		//IL_0040: Expected O, but got Unknown
00201: 		StoreEvent val = NewEvent("cardpkg_misprint_flood", 3, 12, "第五印刷厂套色事故", "错版卡流入市场", "第五印刷厂一批卡在套色时整体错位，本该销毁的成品被整箱卖到了下层。现在市面上到处是错版，收卡的把价压得很低。");
00202: 		val.negociationDatas.Add(new NegociationData("misprint_card", -30, val.newsName, val.displayName));
00203: 		val.addClientFromEventActionId = "cardpkg_misprint_floodaddClientAction";
00204: 		return val;
00205: 	}
00206: 
00207: 	private static StoreEvent BuildMisprintShortage()
00208: 	{
00209: 		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
00210: 		//IL_0040: Expected O, but got Unknown
00211: 		StoreEvent val = NewEvent("cardpkg_misprint_shortage", 2, 12, "错版卡被炒起来了", "错版卡稀缺", "收藏圈有人放话，说错版是印厂的签名。上层买家开始四处收，出价高得离谱——问题是没人肯卖。");
00212: 		val.negociationDatas.Add(new NegociationData("misprint_card", 60, val.newsName, val.displayName));
00213: 		val.addClientFromEventActionId = "cardpkg_misprint_shortageaddClientAction";
00214: 		return val;
00215: 	}
00216: 
00217: 	private static StoreEvent NewEvent(string identifier, int duration, int cooldown, string newsName, string displayName, string description)
00218: 	{
00219: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00220: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00221: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
00222: 		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
00223: 		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
00224: 		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
00225: 		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
00226: 		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
00227: 		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
00228: 		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
00229: 		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
00230: 		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
00231: 		//IL_0047: Expected O, but got Unknown
00232: 		return new StoreEvent
00233: 		{
00234: 			identifier = identifier,
00235: 			newsName = newsName,
00236: 			displayName = displayName,
00237: 			newsDescription = description,
00238: 			duration = duration,
00239: 			cooldown = cooldown,
00240: 			importance = 5,
00241: 			eventType = (EventType)0,
00242: 			eventArea = (EventArea)2
00243: 		};
00244: 	}
00245: }
~~~

### GameCardPK/ChipSystem.cs

~~~csharp
00047: 			string text = Core.Clean(ItemConfigInjector.BareId(source.identifier));
00048: 			string text2 = Core.Clean(source.name);
00049: 			if (text.IndexOf("chip", StringComparison.OrdinalIgnoreCase) >= 0 || text2.Contains("芯片"))
00050: 			{
00051: 				Trace("unknown|" + Describe(source) + "|" + Describe(card), "拖拽落空：" + Describe(source) + " 不在芯片表里（identifier=" + Core.Clean(source.identifier) + "）⇒ 走不到芯片分支，落点 " + Describe(card));
00052: 			}
00053: 		}
00054: 	}
00055: 
00056: 	internal static void TraceShell(GameItem? card)
00057: 	{
00058: 		if (TraceEnabled && card != null)
00059: 		{
00060: 			bool flag = false;
00061: 			bool flag2 = false;
00062: 			try
00063: 			{
00064: 				flag = ((GameItemFunc)card).canItemTargetThisFunc != null;
00065: 				flag2 = ((GameItemFunc)card).onItemTargetThisFunc != null;
00066: 			}
00067: 			catch (Exception ex)
00068: 			{
00069: 				Trace("shell-ex", "读卡的接收器委托失败：" + ex.Message);
00070: 				return;
00071: 			}
00072: 			Trace("shell|" + flag + "|" + flag2, "造壳完成：canItemTargetThisFunc=" + (flag ? "已挂" : "空！") + " / onItemTargetThisFunc=" + (flag2 ? "已挂" : "空！"));
00073: 		}
00074: 	}
00075: 
00076: 	internal static void ExplainReject(GameItem? chipItem, GameItem? card)
00077: 	{
00078: 		if (chipItem == null || card == null)
00079: 		{
00080: 			return;
00081: 		}
00082: 		try
00083: 		{
00084: 			ChipData chipData = SpecialItemDatabase.ChipOf(chipItem.identifier);
00085: 			string text;
00086: 			if (chipData == null)
00087: 			{
00088: 				text = "被拖的不是芯片（芯片表里没有 " + ItemConfigInjector.BareId(chipItem.identifier) + "）";
00089: 			}
00090: 			else if (!GameCardFactory.IsCard(card))
00091: 			{
00092: 				text = "落点不是游戏卡（没有 game_card tag）";
00093: 			}
00094: 			else if (!ItemOwnership.IsPlayerOwned(card, "卡牌", log: false))
00095: 			{
00096: 				text = "这张卡不归玩家（柜台待售 / 待对战状态）";
00097: 			}
00098: 			else if (HasChip(card))
00099: 			{
00100: 				text = "这张卡已经有芯片了（一张卡只能装一次）";
00101: 			}
00102: 			else
00103: 			{
00104: 				if (Match(card, chipData.Mutant, out string reason))
00105: 				{
00106: 					return;
00107: 				}
00108: 				text = reason;
00109: 			}
00110: 			Trace("reject|" + ItemConfigInjector.BareId(chipItem.identifier) + "|" + CardIdentity.Of(card) + "|" + text, "拖不上：" + Describe(chipItem) + " → " + Describe(card) + "：" + text);
00111: 		}
00112: 		catch (Exception ex)
00113: 		{
00114: 			Trace("reject-ex|" + Describe(chipItem), "说明拒装原因时出错（已忽略）：" + ex.Message);
00115: 		}
00116: 	}
00117: 
00118: 	internal static bool CanInstall(GameItem? chipItem, GameItem? card)
00119: 	{
00120: 		if (chipItem == null || card == null)
00121: 		{
00122: 			return false;
00123: 		}
00124: 		try
00125: 		{
00126: 			ChipData chipData = SpecialItemDatabase.ChipOf(chipItem.identifier);
00127: 			if (chipData == null)
00128: 			{
00129: 				return false;
00130: 			}
00131: 			if (!GameCardFactory.IsCard(card))
00132: 			{
00133: 				return false;
00134: 			}
00135: 			if (!ItemOwnership.IsPlayerOwned(card, "卡牌", log: false))
00136: 			{
00137: 				return false;
00138: 			}
00139: 			if (HasChip(card))
00140: 			{
00141: 				return false;
00142: 			}
00143: 			string reason;
00144: 			return Match(card, chipData.Mutant, out reason);
00145: 		}
00146: 		catch (Exception ex)
00147: 		{
00148: 			Core.Debug("[芯片] 安装判定失败（按不能装处理）：" + ex.Message);
00149: 			return false;
00150: 		}
00151: 	}
00152: 
00153: 	private static bool Match(GameItem card, bool chipMutant, out string reason)
00154: 	{
00155: 		reason = string.Empty;
00156: 		if (!chipMutant)
00157: 		{
00158: 			return true;
00159: 		}
00160: 		if (HasAltArt(card))
00161: 		{
00162: 			return true;
00163: 		}
00164: 		reason = "只有带「异画」标记的卡才能装变异芯片";
00165: 		return false;
00166: 	}
00167: 
00168: 	internal static bool HasAltArt(GameItem? card)
00169: 	{
00170: 		if (card == null)
00171: 		{
00172: 			return false;
00173: 		}
00174: 		try
00175: 		{
00176: 			if (!TagDisplay.NewTagDatas.TryGetValue("VariantArt", out CustomTagData value) || value == null)
00177: 			{
00178: 				return false;
00179: 			}
00180: 			return !string.IsNullOrEmpty(value.Id) && card.IsTag(value.Id);
00181: 		}
00182: 		catch (Exception ex)
00183: 		{
00184: 			Core.Debug("[芯片] 读异画标签失败（按不是处理）：" + ex.Message);
00185: 			return false;
00186: 		}
00187: 	}
00188: 
00189: 	internal static bool HasChip(GameItem? card)
00190: 	{
00191: 		if (card == null)
00192: 		{
00193: 			return false;
00194: 		}
00195: 		try
00196: 		{
00197: 			return card.IsTag(CardDurabilitySystem.FullCurTag);
00198: 		}
00199: 		catch (Exception ex)
00200: 		{
00201: 			Core.Debug("[芯片] 读取芯片标签失败（按没有处理）：" + ex.Message);
00202: 			return false;
00203: 		}
00204: 	}
00205: 
00206: 	internal static bool TryInstall(GameItem? chipItem, GameItem? card)
00207: 	{
00208: 		if (chipItem == null || card == null)
00209: 		{
00210: 			return false;
00211: 		}
00212: 		ChipData chipData = SpecialItemDatabase.ChipOf(chipItem.identifier);
00213: 		if (chipData == null)
00214: 		{
00215: 			Trace("install-notchip|" + Describe(chipItem), "TryInstall：被拖的不是芯片（identifier=" + Core.Clean(chipItem.identifier) + "）");
00216: 			return false;
00217: 		}
00218: 		string text = Core.Clean(card.name);
00219: 		if (!GameCardFactory.IsCard(card))
00220: 		{
00221: 			Trace("install-notcard|" + Describe(card), "TryInstall：落点不是游戏卡：" + Core.Clean(card.identifier));
00222: 			return false;
00223: 		}
00224: 		if (!ItemOwnership.IsPlayerOwned(card, "卡牌"))
00225: 		{
00226: 			Trace("install-notown|" + Describe(card), "TryInstall：这张卡还不归你：" + text);
00227: 			return false;
00228: 		}
00229: 		if (HasChip(card))
00230: 		{
00231: 			Trace("install-haschip|" + Describe(card), "TryInstall：「" + text + "」已经有芯片（" + CardDurabilitySystem.FullCurTag + " 存在）。");
00232: 			return false;
00233: 		}
00234: 		if (!Match(card, chipData.Mutant, out string reason))
00235: 		{
00236: 			Trace("install-mismatch|" + Describe(card), "TryInstall：「" + text + "」装不了「" + chipData.DisplayName + "」：" + reason);
00237: 			return false;
00238: 		}
00239: 		int num = ResolveMax(card);
00240: 		if (num <= 0)
00241: 		{
00242: 			Core.Log?.Error("[芯片] 读不到「" + text + "」的耐久上限，这枚芯片不装、不消耗。");
00243: 			return false;
00244: 		}
00245: 		GameCardFactory.ChangeTag("ChipDurabilityMax", "耐久上限", card, num);
00246: 		GameCardFactory.ChangeTag("ChipDurability", "芯片耐久", card, num);
00247: 		int num2 = GameCardFactory.ReadInt(card, CardDurabilitySystem.FullCurTag);
00248: 		if (num2 != num)
00249: 		{
00250: 			Core.Log?.Error("[芯片]「" + text + "」的芯片耐久没写进去（期望 " + num + " 实际 " + num2 + "），已撤回标签，芯片不消耗。");
00251: 			Rollback(card);
00252: 			return false;
00253: 		}
00254: 		if (!ItemConsumption.Consume(chipItem))
00255: 		{
00256: 			Core.Log?.Error("[芯片] 芯片没能消耗，装着的结果已撤回：" + chipData.DisplayName);
00257: 			Rollback(card);
00258: 			return false;
00259: 		}
00260: 		Core.Debug("[芯片]「" + text + "」装上「" + chipData.DisplayName + "」，芯片耐久 " + num + "/" + num + "。");
00261: 		Trace("install-ok|" + Describe(chipItem) + "|" + Describe(card), "TryInstall 成功：" + Describe(chipItem) + " → " + Describe(card) + "，耐久 " + num + "/" + num);
00262: 		if (chipData.Mutant)
00263: 		{
00264: 			MutationSystem.ApplyOnChipInstall(card);
00265: 		}
00266: 		return true;
00267: 	}
00268: 
00269: 	private static int ResolveMax(GameItem card)
00270: 	{
00271: 		try
00272: 		{
00273: 			return CardDatabase.ResolveDurabilityMax(CardDatabase.GetCard(CardIdentity.Of(card)));
00274: 		}
00275: 		catch (Exception ex)
00276: 		{
00277: 			Core.Debug("[芯片] 回查耐久上限失败（用全局默认）：" + ex.Message);
00278: 			return CardDatabase.DefaultDurabilityMax;
00279: 		}
00280: 	}
00281: 
00282: 	private static void Rollback(GameItem card)
00283: 	{
00284: 		try
00285: 		{
00286: 			card.DisableTag(CardDurabilitySystem.FullCurTag, false);
00287: 		}
00288: 		catch (Exception ex)
00289: 		{
00290: 			Core.Debug("[芯片] 撤回耐久标签失败：" + ex.Message);
00291: 		}
00292: 		try
00293: 		{
00294: 			card.DisableTag(CardDurabilitySystem.FullMaxTag, false);
00295: 		}
00296: 		catch (Exception ex2)
00297: 		{
00298: 			Core.Debug("[芯片] 撤回耐久上限标签失败：" + ex2.Message);
00299: 		}
00300: 	}
00301: 
00302: 	internal static string Describe()
00303: 	{
00304: 		return "芯片 " + SpecialItemDatabase.Chips.Count + " 枚；卡的耐久上限取 cards.json 的 durability（默认 " + CardDatabase.DefaultDurabilityMax + "）；装过芯片的卡才能上阵，每战 -" + 10 + "，低于 " + 10 + " 不能参战。";
00305: 	}
00306: }
~~~

### GameCardPK/Core.cs

~~~csharp
00027: 			debugEntry = config.Define("debug_logging", false, "调试", (Func<string>)(() => "输出调试日志"), (Func<string>)(() => "命中判定、降级兜底等过程日志。排查问题时打开。"), false);
00028: 			slowLogEntry = config.Define("slow_battle_log", true, "战斗", (Func<string>)(() => "战斗日志逐条播放"), (Func<string>)(() => "关闭后战斗日志一次性全部显示（等价于战斗中右上角那个开关）。"), false);
00029: 			slowLogEntry.OnChanged += delegate(bool v)
00030: 			{
00031: 				BattleLogPlayback.StepMode = v;
00032: 			};
00033: 			BattleLogPlayback.StepMode = slowLogEntry.Value;
00034: 		}
00035: 		catch (Exception ex)
00036: 		{
00037: 			Log?.Warning("读取 mod 配置失败（使用默认值）：" + ex.Message);
00038: 		}
00039: 		Log.Msg("v" + manifest.ModVersion + " 初始化（Mono / 官方 ModLoader）" + (DebugLogging ? "，调试日志已开启" : ""));
00040: 	}
00041: 
00042: 	public void OnEnable()
00043: 	{
00044: 		BattleLogPlayback.StepMode = SlowBattleLog;
00045: 		ModRunner.Boot();
00046: 		try
00047: 		{
00048: 			try
00049: 			{
00050: 				BattleDatabase.EnsureLoaded();
00051: 			}
00052: 			catch (Exception ex)
00053: 			{
00054: 				Log?.Error("预加载技能配置失败：" + ex);
00055: 			}
00056: 			ItemConfigInjector.EnsureHook();
00057: 			JsonAssets.LoadAllConfigs();
00058: 			CardKindFactory.RegisterPreBuilt();
00059: 			CardEvents.EnsureRegistered();
00060: 			CardClubConfig.EnsureLoaded();
00061: 			CardNPC.RegisterToVanillaDict(verbose: false);
00062: 			SpriteAssets.InitSpecs();
00063: 			TagDisplay.InitTagDisplay();
00064: 			EventCheckHub.Init();
00065: 			MutationSystem.EnsureRegistered();
00066: 			JunkPressCycle.EnsureRegistered();
00067: 			StickerPrintCycle.EnsureRegistered();
00068: 			CrowPerchCycle.EnsureRegistered();
00069: 			CardDurabilitySystem.EnsureRegistered();
00070: 			BladeDurabilityDropChain.EnsureRegistered();
00071: 			WasteKingDropChain.EnsureRegistered();
00072: 			ShowcaseBuyerSystem.EnsureLoaded();
00073: 			CardNPC.EnsureHook();
00074: 			CardClubUnlock.Subscribe();
00075: 			CardFactoryUnlock.Subscribe();
00076: 			CardFactoryPhone.Subscribe();
00077: 			BattleInterrupt.EnsureHook();
00078: 			ClubOuting.EnsureHook();
00079: 		}
00080: 		catch (Exception ex2)
00081: 		{
00082: 			Log?.Error("配置加载失败：" + ex2);
00083: 		}
00084: 		Log?.Msg("已启用。");
00085: 	}
00086: 
00087: 	public void OnDisable()
00088: 	{
00089: 		ModRunner.Shutdown();
00090: 		try
00091: 		{
00092: 			ModHelper.RemoveNetworkUpgrades("game_card_pk");
00093: 		}
00094: 		catch (Exception ex)
00095: 		{
00096: 			Log?.Warning("移除网络升级项失败（可忽略）：" + ex.Message);
00097: 		}
00098: 		Log?.Msg("已停用。");
00099: 	}
00100: 
00101: 	internal static void OnSceneChanged()
00102: 	{
00103: 		CardClubPanel.Reset();
00104: 		PanelHost.Reset();
00105: 		BattlePanel.Reset();
00106: 		BattleRewardPanel.Reset();
00107: 		CardFaceAssets.Reset();
00108: 		ItemTargetRouter.ResetReceiverRepair();
00109: 	}
00110: 
00111: 	internal static void Debug(string message)
00112: 	{
00113: 		if (DebugLogging)
00114: 		{
00115: 			Log?.Msg(message);
00116: 		}
00117: 	}
00118: 
00119: 	internal static void SetSlowBattleLog(bool value)
00120: 	{
00121: 		BattleLogPlayback.StepMode = value;
00122: 		try
00123: 		{
00124: 			slowLogEntry?.Set(value);
00125: 		}
00126: 		catch (Exception ex)
00127: 		{
00128: 			Log?.Warning("保存「逐条播放」设置失败：" + ex.Message);
00129: 		}
00130: 	}
00131: 
00132: 	internal static string Clean(string? value)
00133: 	{
00134: 		if (!string.IsNullOrEmpty(value))
00135: 		{
00136: 			return value.Replace("\0", string.Empty).Trim();
00137: 		}
00138: 		return string.Empty;
00139: 	}
00140: }
~~~

### GameCardPK/JunkPressCycle.cs

~~~csharp
00114: 		{
00115: 			return StoreStation.GetDayCounter();
00116: 		}
00117: 		catch (Exception ex)
00118: 		{
00119: 			Core.Debug("[垃圾] 读天数失败（本夜不去重）：" + ex.Message);
00120: 			return int.MinValue;
00121: 		}
00122: 	}
00123: 
00124: 	private static string Settle(GameItem press)
00125: 	{
00126: 		SpecialItemDatabase.PressSpec pressSpec = SpecialItemDatabase.PressFor(Core.Clean(press.identifier));
00127: 		if (pressSpec == null)
00128: 		{
00129: 			return "没有 press 参数";
00130: 		}
00131: 		GameSlotInventory? obj = JunkPressWindow.BatterySlot(press);
00132: 		GameItem val = ((obj != null) ? obj.childItem : null);
00133: 		int num = 0;
00134: 		if (val != null)
00135: 		{
00136: 			try
00137: 			{
00138: 				num = PowerHelper.GetAvailableEnergyFromItem(val, false);
00139: 			}
00140: 			catch (Exception ex)
00141: 			{
00142: 				Core.Debug("[垃圾] 读电量失败（按 0 处理）：" + ex.Message);
00143: 			}
00144: 		}
00145: 		if (num < pressSpec.PowerCost)
00146: 		{
00147: 			Core.Debug("[垃圾] 电量 " + num + " < " + pressSpec.PowerCost + "，本夜不结算（不扣料）。");
00148: 			return "电量 " + num + " < " + pressSpec.PowerCost;
00149: 		}
00150: 		List<GameItem> list = Collect(JunkPressWindow.Grid(press, output: false), JunkTags.IsMaterial);
00151: 		if (list.Count < pressSpec.JunkCost)
00152: 		{
00153: 			Core.Debug("[垃圾] 投入格垃圾 " + list.Count + " < " + pressSpec.JunkCost + "，本夜不结算（不扣电）。");
00154: 			return "垃圾 " + list.Count + " < " + pressSpec.JunkCost;
00155: 		}
00156: 		List<GameItem> list2 = Collect(JunkPressWindow.Grid(press, output: true), IsPlayerGameCard);
00157: 		if (list2.Count == 0)
00158: 		{
00159: 			Core.Debug("[垃圾] 产出格没有游戏卡，本夜不结算（不扣电不扣料）。");
00160: 			return "产出格没有游戏卡";
00161: 		}
00162: 		int num2 = 0;
00163: 		if (pressSpec.PowerCost > 0)
00164: 		{
00165: 			if (val == null)
00166: 			{
00167: 				Core.Debug("[垃圾] 电池槽是空的，本夜不结算。");
00168: 				return "没有电池";
00169: 			}
00170: 			try
00171: 			{
00172: 				num2 = PowerHelper.TryRemoveEnergy(val, pressSpec.PowerCost, false);
00173: 			}
00174: 			catch (Exception ex2)
00175: 			{
00176: 				Core.Log?.Error("[垃圾] 扣电失败（本夜放弃）：" + ex2.Message);
00177: 				return "扣电异常";
00178: 			}
00179: 			if (num2 < pressSpec.PowerCost)
00180: 			{
00181: 				Core.Log?.Error("[垃圾] 只扣到 " + num2 + "/" + pressSpec.PowerCost + " 电（预检通过却扣不动？），本夜放弃且不扣垃圾。");
00182: 				return "扣电不足（" + num2 + "/" + pressSpec.PowerCost + "）";
00183: 			}
00184: 		}
00185: 		int num3 = 0;
00186: 		if (pressSpec.JunkCost > 0)
00187: 		{
00188: 			for (int i = 0; i < list.Count; i++)
00189: 			{
00190: 				if (num3 >= pressSpec.JunkCost)
00191: 				{
00192: 					break;
00193: 				}
00194: 				try
00195: 				{
00196: 					ClubInventory.Discard(list[i]);
00197: 					num3++;
00198: 				}
00199: 				catch (Exception ex3)
00200: 				{
00201: 					Core.Log?.Error("[垃圾] 销毁垃圾失败（已忽略这一件）：" + ex3.Message);
00202: 				}
00203: 			}
00204: 		}
00205: 		int num4 = 0;
00206: 		foreach (GameItem item in list2)
00207: 		{
00208: 			try
00209: 			{
00210: 				if (CardDurabilityChecks.ChargeCard(item, pressSpec.Repair))
00211: 				{
00212: 					num4++;
00213: 				}
00214: 			}
00215: 			catch (Exception ex4)
00216: 			{
00217: 				Core.Log?.Error("[垃圾] 修复耐久失败（已跳过这张卡）：" + ex4.Message);
00218: 			}
00219: 		}
00220: 		Core.Debug("[垃圾] 压榨一炉：扣电 " + num2 + " / 扣垃圾 " + num3 + " / 修复 " + num4 + " 张卡（+" + pressSpec.Repair + " 耐久，产出格共 " + list2.Count + " 张）。");
00221: 		return "扣电 " + num2 + "，扣垃圾 " + num3 + "，修复 " + num4 + "/" + list2.Count + " 张";
00222: 	}
00223: 
00224: 	private static List<GameItem> Collect(GameGridInventory? grid, Func<GameItem, bool> match)
00225: 	{
00226: 		List<GameItem> list = new List<GameItem>();
00227: 		if (grid == null)
00228: 		{
00229: 			return list;
00230: 		}
00231: 		try
00232: 		{
00233: 			List<GameItem> childItems = ((GameInventory)grid).childItems;
00234: 			if (childItems == null)
00235: 			{
00236: 				return list;
00237: 			}
00238: 			HashSet<IntPtr> hashSet = new HashSet<IntPtr>();
00239: 			for (int i = 0; i < childItems.Count; i++)
00240: 			{
00241: 				GameItem val = childItems[i];
00242: 				if (val != null && hashSet.Add(val.Pointer()) && match(val))
00243: 				{
00244: 					list.Add(val);
00245: 				}
00246: 			}
00247: 		}
00248: 		catch (Exception ex)
00249: 		{
~~~

### GameCardPK/ShowcaseBuyerSystem.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using UnityEngine;
00004: 
00005: namespace GameCardPK;
00006: 
00007: internal static class ShowcaseBuyerSystem
00008: {
00009: 	internal sealed class ClientRule
00010: 	{
00011: 		public string Id { get; set; } = "";
00012: 
00013: 		public int MinValue { get; set; }
00014: 	}
00015: 
00016: 	internal sealed class BuyerRule
00017: 	{
00018: 		public string Type { get; set; } = "";
00019: 
00020: 		public List<ClientRule> Clients { get; set; } = new List<ClientRule>();
00021: 	}
00022: 
00023: 	internal sealed class SellerRule
00024: 	{
00025: 		public string ItemId { get; set; } = "";
00026: 
00027: 		public string ClientId { get; set; } = "";
00028: 
00029: 		public int Chance { get; set; } = 100;
00030: 	}
00031: 
00032: 	private sealed class FileDto
00033: 	{
00034: 		public List<BuyerRule>? Buyers { get; set; }
00035: 
00036: 		public List<SellerRule>? Sellers { get; set; }
00037: 	}
00038: 
00039: 	private const string FileName = "showcase_buyers.json";
00040: 
00041: 	private static bool _loaded;
00042: 
00043: 	private static readonly List<BuyerRule> Rules = new List<BuyerRule>();
00044: 
00045: 	private static readonly List<SellerRule> SellerRules = new List<SellerRule>();
00046: 
00047: 	internal static void EnsureLoaded()
00048: 	{
00049: 		if (_loaded)
00050: 		{
00051: 			return;
00052: 		}
00053: 		_loaded = true;
00054: 		try
00055: 		{
00056: 			FileDto fileDto = JsonAssets.Load<FileDto>("showcase_buyers.json");
00057: 			if (fileDto == null)
00058: 			{
00059: 				Core.Log?.Error("[展示柜] 读不到 showcase_buyers.json，展示柜客人不会出现。");
00060: 				return;
00061: 			}
00062: 			if (fileDto.Buyers != null)
00063: 			{
00064: 				foreach (BuyerRule buyer in fileDto.Buyers)
00065: 				{
00066: 					if (buyer == null)
00067: 					{
00068: 						continue;
00069: 					}
00070: 					buyer.Type = Core.Clean(buyer.Type);
00071: 					if (buyer.Type.Length == 0)
00072: 					{
00073: 						continue;
00074: 					}
00075: 					if (buyer.Clients == null)
00076: 					{
00077: 						buyer.Clients = new List<ClientRule>();
00078: 					}
00079: 					Rules.Add(buyer);
00080: 					try
00081: 					{
00082: 						if (!TypeHelper.MAJOR_TYPES_LIST.Contains(buyer.Type))
00083: 						{
00084: 							TypeHelper.MAJOR_TYPES_LIST.Add(buyer.Type);
00085: 						}
00086: 					}
00087: 					catch (Exception ex)
00088: 					{
00089: 						Core.Log?.Error("[展示柜] 把类型「" + buyer.Type + "」并入官方品类表失败：" + ex.Message);
00090: 					}
00091: 				}
00092: 			}
00093: 			if (fileDto.Sellers != null)
00094: 			{
00095: 				foreach (SellerRule seller in fileDto.Sellers)
00096: 				{
00097: 					if (seller == null)
00098: 					{
00099: 						continue;
00100: 					}
00101: 					seller.ItemId = Core.Clean(seller.ItemId);
00102: 					seller.ClientId = Core.Clean(seller.ClientId);
00103: 					if (seller.ItemId.Length != 0 && seller.ClientId.Length != 0)
00104: 					{
00105: 						if (seller.Chance <= 0)
00106: 						{
00107: 							seller.Chance = 100;
00108: 						}
00109: 						SellerRules.Add(seller);
00110: 					}
00111: 				}
00112: 			}
00113: 			Core.Log?.Msg("[展示柜] 配置已加载：买家档位 " + Rules.Count + " 条 / 卖家 " + SellerRules.Count + " 条。");
00114: 		}
00115: 		catch (Exception ex2)
00116: 		{
00117: 			Core.Log?.Error("[展示柜] 读取 showcase_buyers.json 失败：" + ex2.Message);
00118: 		}
00119: 	}
00120: 
00121: 	private static BuyerRule? FindRule(string? type)
00122: 	{
00123: 		string text = Core.Clean(type);
00124: 		if (text.Length == 0)
00125: 		{
00126: 			return null;
00127: 		}
00128: 		for (int i = 0; i < Rules.Count; i++)
00129: 		{
00130: 			if (string.Equals(Rules[i].Type, text, StringComparison.OrdinalIgnoreCase))
00131: 			{
00132: 				return Rules[i];
00133: 			}
00134: 		}
00135: 		return null;
00136: 	}
00137: 
00138: 	internal static bool IsOurType(string? type)
00139: 	{
00140: 		return FindRule(type) != null;
00141: 	}
00142: 
00143: 	internal static string? PickClientId(string? type)
00144: 	{
00145: 		try
00146: 		{
00147: 			BuyerRule buyerRule = FindRule(type);
00148: 			if (buyerRule == null || buyerRule.Clients.Count == 0)
00149: 			{
00150: 				return null;
00151: 			}
00152: 			int typeTotalValue = ShowcaseHelper.GetTypeTotalValue(buyerRule.Type);
00153: 			if (typeTotalValue <= 0)
00154: 			{
00155: 				return null;
00156: 			}
00157: 			ClientRule clientRule = null;
00158: 			foreach (ClientRule client in buyerRule.Clients)
00159: 			{
00160: 				if (client != null && Core.Clean(client.Id).Length != 0 && typeTotalValue >= client.MinValue && (clientRule == null || client.MinValue > clientRule.MinValue))
00161: 				{
00162: 					clientRule = client;
00163: 				}
00164: 			}
00165: 			return (clientRule == null) ? null : Core.Clean(clientRule.Id);
00166: 		}
00167: 		catch (Exception ex)
00168: 		{
00169: 			Core.Debug("[展示柜] 选取买家失败（已忽略）：" + ex.Message);
00170: 			return null;
00171: 		}
00172: 	}
00173: 
00174: 	internal static void AppendBuyerCustomer(List<(string clientId, string type)> list)
00175: 	{
00176: 		if (list == null || Rules.Count == 0)
00177: 		{
00178: 			return;
00179: 		}
00180: 		try
00181: 		{
00182: 			List<string> displayCaseType = ShowcaseHelper.GetDisplayCaseType();
00183: 			for (int i = 0; i < displayCaseType.Count; i++)
00184: 			{
00185: 				string text = PickClientId(displayCaseType[i]);
00186: 				if (text == null)
00187: 				{
00188: 					continue;
00189: 				}
00190: 				bool flag = false;
00191: 				for (int j = 0; j < list.Count; j++)
00192: 				{
00193: 					if (string.Equals(list[j].clientId, text, StringComparison.OrdinalIgnoreCase))
00194: 					{
00195: 						flag = true;
00196: 						break;
00197: 					}
00198: 				}
00199: 				if (!flag)
00200: 				{
00201: 					list.Add((text, displayCaseType[i]));
00202: 				}
00203: 			}
00204: 		}
00205: 		catch (Exception ex)
00206: 		{
00207: 			Core.Debug("[展示柜] 补商品客户失败（已忽略）：" + ex.Message);
00208: 		}
00209: 	}
00210: 
00211: 	internal static void AppendSellerPrediction(List<(string, int, string)> list)
00212: 	{
00213: 		if (list == null || SellerRules.Count == 0)
00214: 		{
00215: 			return;
00216: 		}
00217: 		try
00218: 		{
00219: 			List<GameItem> list2 = ShowcaseItems();
00220: 			if (list2 == null)
00221: 			{
00222: 				return;
00223: 			}
00224: 			for (int i = 0; i < SellerRules.Count; i++)
00225: 			{
00226: 				SellerRule sellerRule = SellerRules[i];
00227: 				string b = ItemConfigInjector.Prefixed(sellerRule.ItemId);
00228: 				string text = null;
00229: 				for (int j = 0; j < list2.Count; j++)
00230: 				{
00231: 					GameItem val = list2[j];
00232: 					if (val != null && string.Equals(Core.Clean(val.identifier), b, StringComparison.OrdinalIgnoreCase))
00233: 					{
00234: 						text = Core.Clean(val.identifier);
00235: 						break;
00236: 					}
00237: 				}
00238: 				if (text == null)
00239: 				{
00240: 					continue;
00241: 				}
00242: 				bool flag = false;
00243: 				for (int k = 0; k < list.Count; k++)
00244: 				{
00245: 					if (string.Equals(list[k].Item1, sellerRule.ClientId, StringComparison.OrdinalIgnoreCase))
00246: 					{
00247: 						flag = true;
00248: 						break;
00249: 					}
00250: 				}
00251: 				if (!flag)
00252: 				{
00253: 					list.Add((sellerRule.ClientId, sellerRule.Chance, text));
00254: 				}
00255: 				break;
00256: 			}
00257: 		}
00258: 		catch (Exception ex)
00259: 		{
00260: 			Core.Debug("[展示柜] 追加卖家失败（已忽略）：" + ex.Message);
00261: 		}
00262: 	}
00263: 
~~~

### GameCardPK/ShowcaseCustomerPatch.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using HarmonyLib;
00004: 
00005: namespace GameCardPK;
00006: 
00007: [HarmonyPatch(typeof(ShowcaseHelper), "GetDisplayCaseCustomer")]
00008: internal static class ShowcaseCustomerPatch
00009: {
00010: 	private static void Postfix(ref List<(string clientId, string type)> __result)
00011: 	{
00012: 		try
00013: 		{
00014: 			ShowcaseBuyerSystem.AppendBuyerCustomer(__result);
00015: 		}
00016: 		catch (Exception ex)
00017: 		{
00018: 			Core.Debug("[展示柜] 补商品客户失败（已忽略）：" + ex.Message);
00019: 		}
00020: 	}
00021: }
~~~

### GameCardPK/ShowcasePredictionPatch.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using HarmonyLib;
00004: 
00005: namespace GameCardPK;
00006: 
00007: [HarmonyPatch(typeof(ShowcaseHelper), "GetDisplayCasePrediction")]
00008: internal static class ShowcasePredictionPatch
00009: {
00010: 	private static void Postfix(ref List<(string, int, string)> __result)
00011: 	{
00012: 		try
00013: 		{
00014: 			ShowcaseBuyerSystem.AppendSellerPrediction(__result);
00015: 		}
00016: 		catch (Exception ex)
00017: 		{
00018: 			Core.Debug("[展示柜] 追加卖家失败（已忽略）：" + ex.Message);
00019: 		}
00020: 	}
00021: }
~~~

### GameCardPK/SlotAdmission.cs

~~~csharp
00150: 	{
00151: 		try
00152: 		{
00153: 			return item.parentInventory != null && item.parentInventory.Pointer() == inventory.Pointer();
00154: 		}
00155: 		catch
00156: 		{
00157: 			return false;
00158: 		}
00159: 	}
00160: 
00161: 	private static bool AcceptStickerBlueprint(GameItem item)
00162: 	{
00163: 		try
00164: 		{
00165: 			if (!item.IsTag("STICKER_BLUEPRINT_TAG"))
00166: 			{
00167: 				return false;
00168: 			}
00169: 		}
00170: 		catch
00171: 		{
00172: 			return false;
00173: 		}
00174: 		return ItemOwnership.IsPlayerOwned(item, "贴纸蓝图", log: false);
00175: 	}
00176: 
00177: 	private static bool AcceptStickerMaterial(GameItem item)
00178: 	{
00179: 		try
00180: 		{
00181: 			if (!WasteTags.IsMaterial(item))
00182: 			{
00183: 				return false;
00184: 			}
00185: 		}
00186: 		catch
00187: 		{
00188: 			return false;
00189: 		}
00190: 		return ItemOwnership.IsPlayerOwned(item, "废纸", log: false);
00191: 	}
00192: 
00193: 	internal static string Describe()
00194: 	{
00195: 		return "槽位准入（官方钩子 mayInventoryAddItemFunc）：技能槽只收技能条（核心槽不许放）/ battle_card_slot 只收自己的、装过芯片且耐久够的卡 / repair_slot 只收自己的游戏卡 / 贴纸打印机的蓝图槽只收贴纸蓝图、投入格只收废纸";
00196: 	}
00197: }
~~~
