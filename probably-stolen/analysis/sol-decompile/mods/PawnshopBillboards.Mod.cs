using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyFileVersion("1.0.25.0")]
[assembly: AssemblyVersion("1.0.25.0")]
[module: RefSafetyRules(11)]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableAttribute : Attribute
	{
		public readonly byte[] NullableFlags;

		public NullableAttribute(byte P_0)
		{
			NullableFlags = new byte[1] { P_0 };
		}

		public NullableAttribute(byte[] P_0)
		{
			NullableFlags = P_0;
		}
	}
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableContextAttribute : Attribute
	{
		public readonly byte Flag;

		public NullableContextAttribute(byte P_0)
		{
			Flag = P_0;
		}
	}
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		public readonly int Version;

		public RefSafetyRulesAttribute(int P_0)
		{
			Version = P_0;
		}
	}
}
namespace PawnshopBillboards
{
	internal static class BillboardLightInput
	{
		private static GameItem? pending;

		private static Vector2 position;

		private static float deadline;

		internal static void Cancel()
		{
			pending = null;
		}

		internal static void Queue(GameItem item, Vector2 point, float delay)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			Complete();
			pending = item;
			position = point;
			deadline = Time.time + delay;
		}

		internal static void Tick()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (pending != null)
			{
				if (!BillboardManager.CanSwitchLighting(pending, position))
				{
					Cancel();
				}
				else if (Time.time >= deadline)
				{
					Complete();
				}
			}
		}

		private static void Complete()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			GameItem val = pending;
			Cancel();
			if (val != null && BillboardManager.CanSwitchLighting(val, position))
			{
				BillboardManager.SwitchLighting(val);
			}
		}
	}
	[HarmonyPatch(typeof(ItemMouseDoubleClickHandler), "OnEventRelease", new Type[] { typeof(ISet<KeyCode>) })]
	internal static class HangingLightReleaseHook
	{
		private static bool Prefix(ItemMouseDoubleClickHandler __instance, ISet<KeyCode> keysReleased, GameItem ___lastClickedItem, KeyCode ___lastKey, Vector2 ___pressPosition, int ___pressFrame, int ___consumedPressFrame, bool ___isDoubleClick, float ___timeDeltaSeconds)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			if (!Core.Enabled || ___lastClickedItem == null || !BillboardManager.IsStored(___lastClickedItem) || !keysReleased.Contains(__instance.key))
			{
				return true;
			}
			try
			{
				Vector2 val = Vector2.op_Implicit(Input.mousePosition);
				ItemMouseDragHandler current = ItemMouseDragHandler.current;
				if (___lastKey != __instance.key || ___pressFrame < 0 || ___pressFrame == ___consumedPressFrame || ___isDoubleClick || (Object)(object)current == (Object)null || current.IsDraggingItem)
				{
					goto IL_00b9;
				}
				Vector2 val2 = val - ___pressPosition;
				if (((Vector2)(ref val2)).magnitude >= current.dragThreshold)
				{
					goto IL_00b9;
				}
				ItemSelectHandler current2 = ItemSelectHandler.current;
				if ((current2 != null && current2.IsEquipped) || KeyBindHelper.GetKey("quick_move_inventory") || KeyBindHelper.GetKey("quick_move_showcase") || KeyBindHelper.GetKey("quick_move_storage") || !BillboardManager.CanSwitchLighting(___lastClickedItem, val))
				{
					goto IL_00b9;
				}
				BillboardLightInput.Queue(___lastClickedItem, val, Mathf.Max(0f, __instance.doubleclickTimeThresholdSeconds - ___timeDeltaSeconds));
				goto end_IL_0023;
				IL_00b9:
				return false;
				end_IL_0023:;
			}
			catch (Exception e)
			{
				BillboardManager.Fail(e);
			}
			return false;
		}
	}
	[HarmonyPatch(typeof(ItemMouseDoubleClickHandler), "OnEventPress", new Type[] { typeof(ISet<KeyCode>) })]
	internal static class HangingLightDoublePressHook
	{
		private static void Postfix(bool ___isDoubleClick)
		{
			if (___isDoubleClick)
			{
				BillboardLightInput.Cancel();
			}
		}
	}
	[HarmonyPatch(typeof(ItemMouseDoubleClickHandler), "OnEventReset", new Type[] { })]
	internal static class HangingLightResetHook
	{
		private static void Postfix()
		{
			BillboardLightInput.Cancel();
		}
	}
	internal static class BillboardManager
	{
		internal sealed class NativeTransfer
		{
			internal GameItemElement Item;

			internal GameInventory Source;

			internal GridShape Shape;
		}

		internal const string Key = "billboards.hanging.layout.v1";

		internal const string Header = "billboards:1\n";

		private const string SiaBlade = "ce_sia_blade";

		private static readonly HashSet<string> KnownSigns = new HashSet<string>(StringComparer.Ordinal)
		{
			"sign_water", "sign_food", "sign_material", "sign_ruined_machine", "sign_medical", "sign_household", "sign_firearm", "sign_firearm_heavy", "sign_berry", "sign_expedition",
			"sign_luxury", "ce_electronics_sign", "be_brewery_sign", "ce_sia_blade"
		};

		private static readonly Dictionary<int, Placement> records = new Dictionary<int, Placement>();

		private static readonly Dictionary<int, StoredBillboard?> unavailable = new Dictionary<int, StoredBillboard>();

		private static readonly Dictionary<int, BillboardMirror> mirrors = new Dictionary<int, BillboardMirror>();

		private static readonly List<SpriteRenderer> occluders = new List<SpriteRenderer>();

		private static readonly Dictionary<Sprite, (Vector2[] vertices, ushort[] triangles)> meshes = new Dictionary<Sprite, (Vector2[], ushort[])>();

		private static PlayerStore? owner;

		private static string run = "";

		private static SpriteRenderer? anchor;

		private static RectTransform? root;

		private static Camera? camera;

		private static bool expanded;

		private static bool fault;

		private static bool focused = true;

		private static float refreshAt;

		private static GameItemElement? dragged;

		private static bool initialized;

		private static bool busy;

		internal static bool Quitting { get; private set; }

		private static bool CurrentOwner
		{
			get
			{
				if (PlayerStore.HasInstance && owner == PlayerStore.Instance)
				{
					return owner.runID == run;
				}
				return false;
			}
		}

		internal static IEnumerable<GameItem> StoredItems
		{
			get
			{
				if (!CurrentOwner || !initialized || busy || HangingStorage.Inventory == null)
				{
					return Enumerable.Empty<GameItem>();
				}
				return ((GameInventory)HangingStorage.Inventory).childItems.Where((GameItem i) => i.unitCount > 0);
			}
		}

		internal static IEnumerable<GameItem> OwnedStoredItems
		{
			get
			{
				if (!CurrentOwner || HangingStorage.Inventory == null)
				{
					return Enumerable.Empty<GameItem>();
				}
				return ((GameInventory)HangingStorage.Inventory).childItems.Where((GameItem i) => i.unitCount > 0 && GeneralHelper.IsItemOwned(i));
			}
		}

		internal static IEnumerable<GameItem> ActiveSigns
		{
			get
			{
				if (!Core.Enabled)
				{
					return Enumerable.Empty<GameItem>();
				}
				return StoredItems.Where((GameItem i) => IsSign(i) && GeneralHelper.IsItemOwned(i) && IsLit(i));
			}
		}

		private static bool InStore
		{
			get
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				if (PlayerStore.HasInstance && (Object)(object)EmporiumEntry.Instance != (Object)null && (int)PlayerStore.Instance.playerLocation == 0 && (Object)(object)EmporiumEntry.Instance.storePhysical != (Object)null)
				{
					return EmporiumEntry.Instance.storePhysical.activeInHierarchy;
				}
				return false;
			}
		}

		private static bool Presentable
		{
			get
			{
				if (Core.Enabled && !fault && InStore && (Object)(object)StoreUIManager.Instance != (Object)null && !StoreUIManager.Instance.isShootingMode)
				{
					return !StoreUIManager.Instance.isMagnifyingMode;
				}
				return false;
			}
		}

		private static bool Interactive
		{
			get
			{
				if (!Quitting && Presentable && focused && Application.isFocused)
				{
					return !PlayerStore.Instance.cinematicMode;
				}
				return false;
			}
		}

		private static Camera? PresentationCamera
		{
			get
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				Canvas rootCanvas = ((Component)root).GetComponent<Canvas>().rootCanvas;
				if ((int)rootCanvas.renderMode != 0)
				{
					return rootCanvas.worldCamera;
				}
				return null;
			}
		}

		internal static void BeginQuit()
		{
			if (!Quitting)
			{
				if (PlayerStore.HasInstance)
				{
					BeforeSave(PlayerStore.Instance);
				}
				Quitting = true;
			}
		}

		internal static bool IsStored(GameItem item)
		{
			if (CurrentOwner && HangingStorage.Inventory != null && (object)item.parentInventory == HangingStorage.Inventory)
			{
				return ((GameInventory)HangingStorage.Inventory).childItems.Contains(item);
			}
			return false;
		}

		internal static bool IsLit(GameItem item)
		{
			if (records.TryGetValue(item.uniqueId, out Placement value) && value.id == item.identifier)
			{
				return value.lit;
			}
			return false;
		}

		internal static bool CanSwitchLighting(GameItem item, Vector2 point)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if (Interactive && !ItemPinHelper.BlocksClicks && !UIDragHandler.isDraggingAnyUI && !GeneralHelper.IsDisplayOnly(item) && GeneralHelper.IsItemOwned(item) && mirrors.TryGetValue(item.uniqueId, out BillboardMirror value) && (object)value.Item == item && CanHit(value, point))
			{
				return RenderHandler.RaycastElement<GameItem>(point, (TreeNodeRender)null) == item;
			}
			return false;
		}

		internal static void SwitchLighting(GameItem item)
		{
			Placement placement = records[item.uniqueId];
			bool lit = placement.lit;
			placement.lit = !lit;
			try
			{
				Commit();
			}
			catch
			{
				placement.lit = lit;
				throw;
			}
			mirrors[item.uniqueId].Refresh();
			ClienteleUIManager instance = ClienteleUIManager.Instance;
			if (instance != null)
			{
				instance.RefreshUI();
			}
			Core.Notice(placement.lit ? "广告牌已点亮，广告效果已启用。" : "广告牌已关闭，广告效果已停用。");
		}

		internal static bool IsSign(GameItem item)
		{
			if (item != null)
			{
				if (!item.IsTag("SHOWCASE_SPECIAL_ITEM_TAG") && !KnownSigns.Contains(item.identifier))
				{
					string name = item.name;
					if (name == null)
					{
						return false;
					}
					return name.IndexOf("灯牌", StringComparison.Ordinal) >= 0;
				}
				return true;
			}
			return false;
		}

		internal static void SetApplicationFocus(bool focus)
		{
			if (Quitting)
			{
				return;
			}
			focused = focus;
			refreshAt = 0f;
			if (!focus)
			{
				BillboardLightInput.Cancel();
				ItemMouseDragHandler current = ItemMouseDragHandler.current;
				if (dragged != null && !dragged.IsDestroyed() && (Object)(object)current != (Object)null && current.IsDraggingItem && (object)current.GetCurrentItem() == dragged)
				{
					current.Cancel();
				}
				CancelDrag();
			}
		}

		internal static void Reset()
		{
			if (!Quitting)
			{
				busy = true;
				try
				{
					ClearViews();
					HangingStorage.Reset();
					records.Clear();
					unavailable.Clear();
				}
				finally
				{
					busy = false;
				}
				owner = null;
				run = "";
				fault = (initialized = false);
				refreshAt = 0f;
				dragged = null;
			}
		}

		private static void ClearViews()
		{
			BillboardLightInput.Cancel();
			foreach (BillboardMirror value in mirrors.Values)
			{
				value.Dispose();
			}
			mirrors.Clear();
			occluders.Clear();
			meshes.Clear();
			BillboardNeon.Clear();
			if ((Object)(object)root != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)root).gameObject);
			}
			root = null;
			anchor = null;
			camera = null;
		}

		internal static void Hide()
		{
			BillboardLightInput.Cancel();
			foreach (BillboardMirror value in mirrors.Values)
			{
				value.SetVisible(visible: false);
			}
			if ((Object)(object)root != (Object)null)
			{
				((Component)root).gameObject.SetActive(false);
			}
		}

		internal static void Fail(Exception e)
		{
			if (!fault)
			{
				fault = true;
				ClearViews();
				Core.Warn("挂牌功能已暂停，原物品和记录保留，原版保存照常。重新读档后再初始化：" + e);
				Core.Notice("广告牌挂位暂不可用，原实例与记录已保留；游戏仍可保存，重新读档后再试。");
			}
		}

		internal static void Initialize()
		{
			if (!Core.Enabled || !InStore)
			{
				return;
			}
			try
			{
				EnsureOwner();
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}

		private static void EnsureOwner()
		{
			PlayerStore instance = PlayerStore.Instance;
			if (owner == instance && run == instance.runID)
			{
				return;
			}
			Reset();
			owner = instance;
			run = instance.runID;
			Dictionary<string, string> modData = instance.modData;
			if (modData != null && modData.ContainsKey("billboards.hanging.pending.v1"))
			{
				throw new InvalidOperationException("有未核对的广告牌搬运恢复记录，已保留且不会自动重发物品。");
			}
			HangingStorage.Create();
			if (instance.modData == null || !instance.modData.TryGetValue("billboards.hanging.layout.v1", out var value))
			{
				initialized = true;
				return;
			}
			if (!value.StartsWith("billboards:1\n", StringComparison.Ordinal))
			{
				throw new InvalidOperationException("未知挂位记录版本，未覆盖原记录。");
			}
			bool lostLegacy;
			LayoutRecord layoutRecord = LayoutCodec.Read(value.Substring("billboards:1\n".Length), out lostLegacy);
			if (lostLegacy)
			{
				initialized = true;
				instance.modData.Remove("billboards.hanging.layout.v1");
				Core.Warn("已清除旧版仅剩版本号的广告牌记录，可以继续挂牌；未补发已丢失的广告牌。");
				return;
			}
			foreach (Placement item in layoutRecord.items)
			{
				if (item == null || item.uid <= 0 || string.IsNullOrWhiteSpace(item.id) || records.ContainsKey(item.uid) || item.width <= 0 || item.height <= 0 || item.width > 18 || item.height > 5 || item.x < 0 || item.y < 0 || item.x + item.width > 18 || item.y + item.height > 5 || (item.region != 2 && item.region != 1))
				{
					throw new InvalidOperationException("挂位记录身份或占格无效，未覆盖原记录。");
				}
				records.Add(item.uid, item);
			}
			HangingStorage.ValidateSnapshots(layoutRecord.stored, records);
			foreach (Placement value2 in records.Values)
			{
				if (!DirectoryMaster.Has<GameItem>(value2.id))
				{
					unavailable.Add(value2.uid, null);
				}
			}
			foreach (StoredBillboard item2 in layoutRecord.stored)
			{
				if (unavailable.ContainsKey(item2.item.uniqueId))
				{
					unavailable[item2.item.uniqueId] = item2;
				}
			}
			Dictionary<int, Placement> placements = records.Where<KeyValuePair<int, Placement>>((KeyValuePair<int, Placement> keyValuePair) => !unavailable.ContainsKey(keyValuePair.Key)).ToDictionary((KeyValuePair<int, Placement> keyValuePair) => keyValuePair.Key, (KeyValuePair<int, Placement> keyValuePair) => keyValuePair.Value);
			busy = true;
			try
			{
				Dictionary<int, GameItemElement> originals = CounterStorage.FindOriginals(placements);
				if (layoutRecord.version == 2)
				{
					HangingStorage.Restore(layoutRecord.stored.Where((StoredBillboard s) => !unavailable.ContainsKey(s.item.uniqueId) && !originals.ContainsKey(s.item.uniqueId)).ToList(), placements);
				}
				CounterStorage.TakeOriginals(originals.Values);
			}
			finally
			{
				busy = false;
			}
			initialized = true;
			Placement[] array = records.Values.ToArray();
			foreach (Placement p in array)
			{
				if (!unavailable.ContainsKey(p.uid) && !((GameInventory)HangingStorage.Inventory).childItems.Any((GameItem i) => i.uniqueId == p.uid))
				{
					GameItemElement val = ((GameInventory)EmporiumEntry.Instance.showcaseElement).childItems.OfType<GameItemElement>().SingleOrDefault((Func<GameItemElement, bool>)((GameItemElement i) => ((GameItem)i).uniqueId == p.uid && ((GameItem)i).identifier == p.id));
					if (val != null && GeneralHelper.IsItemOwned((GameItem)(object)val) && IsSign((GameItem)(object)val) && ValidArt(val) && !MoveWhole(val, HangingStorage.Inventory, p))
					{
						throw new InvalidOperationException("旧挂牌未能移入独立挂位，已保留原位与记录。");
					}
				}
			}
			if (unavailable.Count > 0)
			{
				Core.Warn("已暂存缺少来源物品定义的灯牌记录：" + string.Join(", ", unavailable.Keys.Select((int uid) => records[uid].id)));
				Core.Notice($"{unavailable.Count}块灯牌缺少来源模组，记录已保留；其他灯牌可正常安装。装回来源模组后重新读档恢复。");
			}
		}

		private static void Commit()
		{
			if (fault || owner == null || owner != PlayerStore.Instance || owner.runID != run)
			{
				return;
			}
			List<StoredBillboard> list = new List<StoredBillboard>();
			foreach (GameItem childItem in ((GameInventory)HangingStorage.Inventory).childItems)
			{
				if (childItem.unitCount > 0)
				{
					if (unavailable.ContainsKey(childItem.uniqueId) || !records.TryGetValue(childItem.uniqueId, out Placement value) || value.id != childItem.identifier || !IsSign(childItem) || !GeneralHelper.IsItemOwned(childItem))
					{
						throw new InvalidOperationException("独立挂位与保存身份不一致，未覆盖原记录。");
					}
					list.Add(HangingStorage.Capture(childItem));
				}
			}
			list.AddRange(from s in unavailable.Values
				where s != null
				select (s));
			string value2 = "billboards:1\n" + LayoutCodec.Write(records.Values.OrderBy((Placement p) => p.uid), list);
			PlayerStore val = owner;
			if (val.modData == null)
			{
				val.modData = new Dictionary<string, string>();
			}
			owner.modData["billboards.hanging.layout.v1"] = value2;
		}

		internal static void BeforeSave(PlayerStore store)
		{
			try
			{
				if (!Quitting && CurrentOwner && owner == store && initialized && !fault && !busy)
				{
					Commit();
				}
			}
			catch (Exception ex)
			{
				fault = true;
				Core.Warn("挂牌快照未更新，保留上次记录；原版及其他模块继续保存：" + ex);
			}
		}

		internal static void Disable()
		{
			if (Quitting)
			{
				return;
			}
			if (CurrentOwner)
			{
				BeforeSave(owner);
			}
			if (CurrentOwner && initialized && !fault && InStore)
			{
				GameItemElement[] array = ((GameInventory)HangingStorage.Inventory).childItems.OfType<GameItemElement>().ToArray();
				foreach (GameItemElement item in array)
				{
					try
					{
						MoveWhole(item, EmporiumEntry.Instance.invElement, null);
					}
					catch (Exception e)
					{
						Fail(e);
						break;
					}
				}
				if (((GameInventory)HangingStorage.Inventory).childItems.Count > 0)
				{
					Core.Notice("仓库放不下的广告牌已保留在独立挂位记录中，重新启用模组后可取回。");
				}
			}
			ClearViews();
		}

		private static bool EnsureScene()
		{
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Expected O, but got Unknown
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0361: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			Transform obj = EmporiumEntry.Instance.storePhysical.transform.Find("front_counter");
			SpriteRenderer source = ((obj != null) ? ((Component)obj).GetComponent<SpriteRenderer>() : null);
			StoreUIManager instance = StoreUIManager.Instance;
			object obj2;
			if (instance == null)
			{
				obj2 = null;
			}
			else
			{
				GameObject mainCamera = instance.mainCamera;
				obj2 = ((mainCamera != null) ? mainCamera.GetComponent<Camera>() : null);
			}
			Camera main = (Camera)obj2;
			StoreShutterButton instance2 = StoreShutterButton.Instance;
			if ((Object)(object)source == (Object)null || (Object)(object)source.sprite == (Object)null || (Object)(object)main == (Object)null || (Object)(object)RenderHandler.current == (Object)null || (Object)(object)instance2 == (Object)null || (Object)(object)instance2.blockerMaskObject == (Object)null)
			{
				return false;
			}
			if ((Object)(object)anchor != (Object)(object)source || (Object)(object)root == (Object)null)
			{
				ClearViews();
				anchor = source;
				camera = main;
				GameObject val = new GameObject("PawnshopBillboards.World", new Type[3]
				{
					typeof(RectTransform),
					typeof(Canvas),
					typeof(GraphicRaycaster)
				})
				{
					layer = ((Component)source).gameObject.layer
				};
				val.GetComponent<Canvas>().renderMode = (RenderMode)2;
				root = (RectTransform)val.transform;
			}
			camera = main;
			Canvas component = ((Component)root).GetComponent<Canvas>();
			Sprite sprite = source.sprite;
			float num = 0f - sprite.pivot.x;
			Rect rect = sprite.rect;
			Vector3 val2 = new Vector3(num, ((Rect)(ref rect)).height - sprite.pivot.y, 0f) / sprite.pixelsPerUnit;
			RectTransform? obj3 = root;
			RectTransform? obj4 = root;
			RectTransform? obj5 = root;
			Vector2 val3 = default(Vector2);
			((Vector2)(ref val3))..ctor(0f, 1f);
			obj5.pivot = val3;
			Vector2 anchorMin = (obj4.anchorMax = val3);
			obj3.anchorMin = anchorMin;
			RectTransform? obj6 = root;
			rect = sprite.rect;
			float width = ((Rect)(ref rect)).width;
			rect = sprite.rect;
			obj6.sizeDelta = new Vector2(width, ((Rect)(ref rect)).height);
			RectTransform rectTransform = ((Graphic)instance2.blockerMaskObject).rectTransform;
			RectTransform parent2;
			Camera uiCamera;
			int layer;
			if (((Component)rectTransform).gameObject.activeInHierarchy)
			{
				Transform parent = ((Transform)rectTransform).parent;
				parent2 = (RectTransform)(object)((parent is RectTransform) ? parent : null);
				object obj7;
				if (!((Object)(object)parent2 != (Object)null))
				{
					obj7 = null;
				}
				else
				{
					Canvas componentInParent = ((Component)parent2).GetComponentInParent<Canvas>();
					obj7 = ((componentInParent != null) ? componentInParent.rootCanvas : null);
				}
				Canvas val5 = (Canvas)obj7;
				if ((Object)(object)parent2 == (Object)null || (Object)(object)val5 == (Object)null)
				{
					return false;
				}
				uiCamera = (((int)val5.renderMode == 0) ? null : val5.worldCamera);
				if ((int)val5.renderMode != 0 && (Object)(object)uiCamera == (Object)null)
				{
					return false;
				}
				Vector2 val6 = Project(val2);
				Vector2 val7 = Project(val2 + Vector3.right / sprite.pixelsPerUnit) - val6;
				Vector2 val8 = Project(val2 + Vector3.up / sprite.pixelsPerUnit) - val6;
				((Transform)root).SetParent((Transform)(object)parent2, false);
				((Transform)root).SetSiblingIndex(((Transform)rectTransform).GetSiblingIndex() + ((((Transform)root).GetSiblingIndex() > ((Transform)rectTransform).GetSiblingIndex()) ? 1 : 0));
				component.overrideSorting = false;
				component.renderMode = val5.renderMode;
				component.worldCamera = uiCamera;
				((Transform)root).localPosition = new Vector3(val6.x, val6.y, 0f);
				((Transform)root).localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(val7.y, val7.x) * 57.29578f);
				((Transform)root).localScale = new Vector3(((Vector2)(ref val7)).magnitude, ((Vector2)(ref val8)).magnitude, 1f);
				layer = ((Component)val5).gameObject.layer;
			}
			else
			{
				((Transform)root).SetParent(((Component)source).transform, false);
				component.renderMode = (RenderMode)2;
				component.worldCamera = main;
				component.overrideSorting = true;
				component.sortingLayerID = ((Renderer)source).sortingLayerID;
				component.sortingOrder = 1;
				((Transform)root).localPosition = val2;
				((Transform)root).localRotation = Quaternion.identity;
				((Transform)root).localScale = Vector3.one / sprite.pixelsPerUnit;
				layer = ((Component)source).gameObject.layer;
			}
			if (((Component)root).gameObject.layer != layer)
			{
				Transform[] componentsInChildren = ((Component)root).GetComponentsInChildren<Transform>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					((Component)componentsInChildren[i]).gameObject.layer = layer;
				}
			}
			Transform val9 = ((Component)source).transform.Find("ShopExpansion.ApprovedLayout");
			bool flag = (Object)(object)val9 != (Object)null && ((Component)val9).gameObject.activeInHierarchy;
			if (expanded != flag)
			{
				expanded = flag;
				refreshAt = 0f;
			}
			return true;
			Vector2 Project(Vector3 local)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				Vector2 val10 = RectTransformUtility.WorldToScreenPoint(main, ((Component)source).transform.TransformPoint(local));
				Vector2 result = default(Vector2);
				RectTransformUtility.ScreenPointToLocalPointInRectangle(parent2, val10, uiCamera, ref result);
				return result;
			}
		}

		internal static void Tick()
		{
			if (!PlayerStore.HasInstance || !InStore)
			{
				Hide();
				return;
			}
			EnsureOwner();
			if (fault || !EnsureScene())
			{
				Hide();
				return;
			}
			if (Time.unscaledTime >= refreshAt)
			{
				refreshAt = Time.unscaledTime + 0.25f;
				Reconcile();
			}
			bool presentable = Presentable;
			((Component)root).gameObject.SetActive(presentable);
			foreach (BillboardMirror value in mirrors.Values)
			{
				value.Refresh();
				value.SetVisible(presentable && value.Item != dragged);
			}
			if (!Interactive)
			{
				BillboardLightInput.Cancel();
			}
			else
			{
				BillboardLightInput.Tick();
			}
		}

		internal static bool ValidArt(GameItemElement item)
		{
			if (!item.IsDestroyed() && (Object)(object)item.background != (Object)null && (Object)(object)item.background.image.sprite != (Object)null)
			{
				return (Object)(object)item.background.image.sprite != (Object)(object)RenderHandler.unknownSprite;
			}
			return false;
		}

		internal static Vector2 ArtworkSize(GameItemElement item)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			Rect rect = item.background.image.sprite.rect;
			return ((Rect)(ref rect)).size / item.background.image.pixelsPerUnit;
		}

		private static (int width, int height) Size(GameItemElement item)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = ArtworkSize(item);
			return (width: Math.Max(((GameItem)item).shape.width, Mathf.CeilToInt(val.x / 16f)), height: Math.Max(((GameItem)item).shape.height, Mathf.CeilToInt(val.y / 16f)));
		}

		private static void Reconcile()
		{
			GameGridInventory inventory = HangingStorage.Inventory;
			KeyValuePair<int, BillboardMirror>[] array = mirrors.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<int, BillboardMirror> keyValuePair = array[i];
				GameItemElement item = keyValuePair.Value.Item;
				if ((Object)(object)item.handler == (Object)null || (object)((GameItem)item).parentInventory != inventory || !GeneralHelper.IsItemOwned((GameItem)(object)item) || !records.ContainsKey(keyValuePair.Key) || !ValidArt(item))
				{
					keyValuePair.Value.Dispose();
					mirrors.Remove(keyValuePair.Key);
				}
			}
			foreach (GameItemElement item2 in ((GameInventory)inventory).childItems.OfType<GameItemElement>())
			{
				if (((GameItem)item2).uniqueId <= 0 || !records.TryGetValue(((GameItem)item2).uniqueId, out Placement value) || value.id != ((GameItem)item2).identifier || !IsSign((GameItem)(object)item2) || !GeneralHelper.IsItemOwned((GameItem)(object)item2) || !ValidArt(item2))
				{
					continue;
				}
				(int, int) tuple = Size(item2);
				Placement placement = value;
				IEnumerable<Placement> occupied = from m in mirrors.Values
					where m.Item != item2
					select m.Placement;
				if (!SlotLayout.Fits(placement, tuple.Item1, tuple.Item2, expanded, occupied))
				{
					placement = SlotLayout.Find(((GameItem)item2).uniqueId, ((GameItem)item2).identifier, tuple.Item1, tuple.Item2, expanded, occupied, value.region);
				}
				if (placement == null)
				{
					if (mirrors.TryGetValue(((GameItem)item2).uniqueId, out BillboardMirror value2))
					{
						value2.Dispose();
						mirrors.Remove(((GameItem)item2).uniqueId);
					}
					continue;
				}
				placement = new Placement
				{
					uid = value.uid,
					id = value.id,
					region = placement.region,
					x = placement.x,
					y = placement.y,
					width = tuple.Item1,
					height = tuple.Item2,
					lit = value.lit
				};
				if (mirrors.TryGetValue(((GameItem)item2).uniqueId, out BillboardMirror value3))
				{
					value3.Placement = placement;
				}
				else
				{
					mirrors.Add(((GameItem)item2).uniqueId, new BillboardMirror(item2, placement, root));
				}
			}
			occluders.Clear();
			SpriteRenderer[] componentsInChildren = EmporiumEntry.Instance.storePhysical.GetComponentsInChildren<SpriteRenderer>(true);
			foreach (SpriteRenderer val in componentsInChildren)
			{
				if ((Object)(object)val != (Object)(object)anchor && (((Object)((Component)val).gameObject).name.StartsWith("ShopExpansion.ApprovedRoom.", StringComparison.Ordinal) || ((Object)((Component)val).gameObject).name == "ShopExpansion.SecondaryClient.Body"))
				{
					occluders.Add(val);
				}
			}
		}

		internal static bool CanHit(BillboardMirror mirror, Vector2 point)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			if (Interactive && mirror.Visible && !((Object)(object)camera == (Object)null) && !((Object)(object)root == (Object)null) && ((Component)root).gameObject.activeInHierarchy && mirror.Item != dragged && IsStored((GameItem)(object)mirror.Item))
			{
				Rect pixelRect = camera.pixelRect;
				if (((Rect)(ref pixelRect)).Contains(point))
				{
					if ((Object)(object)((Transform)root).parent != (Object)(object)((Component)anchor).transform)
					{
						return true;
					}
					if (Covered(StoreUIManager.Instance.clientGameObject?.image, point))
					{
						return false;
					}
					foreach (SpriteRenderer occluder in occluders)
					{
						if (Covered(occluder, point))
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		internal static bool PointerOnBillboard()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (!Interactive || (Object)(object)EventSystem.current == (Object)null)
			{
				return false;
			}
			GameItem val = RenderHandler.RaycastElement<GameItem>(Vector2.op_Implicit(Input.mousePosition), (TreeNodeRender)null);
			if (val != null)
			{
				return mirrors.ContainsKey(val.uniqueId);
			}
			return false;
		}

		private static bool Covered(SpriteRenderer? image, Vector2 point)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)image == (Object)null || !((Renderer)image).enabled || !((Component)image).gameObject.activeInHierarchy || (Object)(object)image.sprite == (Object)null || ((Renderer)image).sortingLayerID != ((Renderer)anchor).sortingLayerID || ((Renderer)image).sortingOrder <= 1)
			{
				return false;
			}
			Ray val = camera.ScreenPointToRay(Vector2.op_Implicit(point));
			Plane val2 = default(Plane);
			((Plane)(ref val2))..ctor(((Component)image).transform.forward, ((Component)image).transform.position);
			float num = default(float);
			if (!((Plane)(ref val2)).Raycast(val, ref num))
			{
				return false;
			}
			Vector3 val3 = ((Component)image).transform.InverseTransformPoint(((Ray)(ref val)).GetPoint(num));
			Vector2 val4 = default(Vector2);
			((Vector2)(ref val4))..ctor(image.flipX ? (0f - val3.x) : val3.x, image.flipY ? (0f - val3.y) : val3.y);
			Sprite sprite = image.sprite;
			Bounds bounds = sprite.bounds;
			float x = val4.x;
			float y = val4.y;
			Bounds bounds2 = sprite.bounds;
			if (!((Bounds)(ref bounds)).Contains(new Vector3(x, y, ((Bounds)(ref bounds2)).center.z)))
			{
				return false;
			}
			if (!meshes.TryGetValue(sprite, out (Vector2[], ushort[]) value))
			{
				value = (sprite.vertices, sprite.triangles);
				meshes.Add(sprite, value);
			}
			for (int i = 0; i + 2 < value.Item2.Length; i += 3)
			{
				Vector2 val5 = value.Item1[value.Item2[i]];
				Vector2 val6 = value.Item1[value.Item2[i + 1]];
				Vector2 val7 = value.Item1[value.Item2[i + 2]];
				float num2 = Cross(val6 - val5, val4 - val5);
				float num3 = Cross(val7 - val6, val4 - val6);
				float num4 = Cross(val5 - val7, val4 - val7);
				if ((num2 >= 0f && num3 >= 0f && num4 >= 0f) || (num2 <= 0f && num3 <= 0f && num4 <= 0f))
				{
					return true;
				}
			}
			return false;
		}

		private static float Cross(Vector2 a, Vector2 b)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return a.x * b.y - a.y * b.x;
		}

		internal static bool TryToggle(GameItem item, Vector2 pointer)
		{
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			if (Core.Enabled)
			{
				GameItemElement val = (GameItemElement)(object)((item is GameItemElement) ? item : null);
				if (val != null && IsSign(item) && GeneralHelper.IsItemOwned(item))
				{
					if (item.identifier == "ce_sia_blade")
					{
						return false;
					}
					if (!InStore)
					{
						return false;
					}
					EnsureOwner();
					if (fault)
					{
						Core.Notice("广告牌记录或恢复发生异常，挂牌已暂停；原记录保留，请重新读档并查看模组日志。");
						return true;
					}
					if (unavailable.ContainsKey(item.uniqueId))
					{
						Core.Notice("这块灯牌缺少来源模组，记录已保留；装回来源模组后重新读档。其他灯牌仍可安装。");
						return true;
					}
					if (!Interactive)
					{
						Core.Notice("当前不能挂广告牌，请回到店内正常视角再试。");
						return true;
					}
					if (!EnsureScene())
					{
						Core.Notice("店内柜台或挂位显示尚未就绪，暂时不能安装广告牌。");
						return true;
					}
					if (!item.MayRemove() || GeneralHelper.IsDisplayOnly(item) || !ValidArt(val))
					{
						return true;
					}
					if (IsStored(item))
					{
						if (!MoveWhole(val, EmporiumEntry.Instance.invElement, null))
						{
							Core.Notice("仓库放不下，广告牌仍留在原挂位。");
							return true;
						}
						Core.Notice("广告牌已取回仓库。");
						return true;
					}
					(int, int) tuple = Size(val);
					int uniqueID = item.GetUniqueID();
					Vector2 val2 = default(Vector2);
					RectTransformUtility.ScreenPointToLocalPointInRectangle(root, pointer, PresentationCamera, ref val2);
					int preferredRegion = SlotLayout.PreferredRegion(val2.x, expanded);
					Placement placement = SlotLayout.Find(uniqueID, item.identifier, tuple.Item1, tuple.Item2, expanded, mirrors.Values.Select((BillboardMirror m) => m.Placement), preferredRegion);
					if (placement == null)
					{
						Core.Notice("柜台上方放不下这块广告牌，原物品保持原位。");
						return true;
					}
					if (!MoveWhole(val, HangingStorage.Inventory, placement))
					{
						Core.Notice("独立挂位暂时放不下，广告牌仍留在原处。");
						return true;
					}
					BillboardMirror billboardMirror = new BillboardMirror(val, placement, root);
					mirrors.Add(uniqueID, billboardMirror);
					billboardMirror.SetVisible(visible: true);
					return true;
				}
			}
			return false;
		}

		private static bool MoveWhole(GameItemElement item, GameGridInventory destination, Placement? place)
		{
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Expected O, but got Unknown
			if (unavailable.ContainsKey(((GameItem)item).uniqueId))
			{
				return false;
			}
			GameInventory parentInventory = ((GameItem)item).parentInventory;
			if (parentInventory == null || !((GameItem)item).MayRemove() || ((GameItem)item).MaxNumRemove() < ((GameItem)item).unitCount || !GeneralHelper.MayPlayerInsertInto((GameInventory)(object)destination))
			{
				return false;
			}
			bool flag = (object)parentInventory != destination;
			SlotMarker val = (flag ? destination.TryFindOneValidInventorySlot((GameItem)(object)item, false) : null);
			if (flag && (val == null || !val.IsValid() || val.targetItem != null || val.numTransfer < ((GameItem)item).unitCount))
			{
				return false;
			}
			GridShape val2 = ((GameItem)item).modifiedShape.Clone();
			records.TryGetValue(((GameItem)item).uniqueId, out Placement value);
			if (place != null && value != null)
			{
				place.lit = value.lit;
			}
			Dictionary<string, string> modData = owner.modData;
			string value2;
			string text = ((modData != null && modData.TryGetValue("billboards.hanging.layout.v1", out value2)) ? value2 : null);
			TransferReceipt.Begin((GameItem)(object)item, parentInventory, (GameInventory)(object)destination);
			busy = true;
			try
			{
				if (flag)
				{
					val.TryAcceptOnce(((GameItem)item).unitCount);
				}
				if ((object)((GameItem)item).parentInventory != destination || !((GameInventory)destination).childItems.Contains((GameItem)(object)item))
				{
					throw new InvalidOperationException("广告牌未到达目标容器。");
				}
				if (place == null)
				{
					records.Remove(((GameItem)item).uniqueId);
				}
				else
				{
					records[((GameItem)item).uniqueId] = place;
				}
				Commit();
				TransferReceipt.Complete();
			}
			catch (Exception innerException)
			{
				if (value == null)
				{
					records.Remove(((GameItem)item).uniqueId);
				}
				else
				{
					records[((GameItem)item).uniqueId] = value;
				}
				if ((object)((GameItem)item).parentInventory == destination && flag)
				{
					((GameInventory)destination).Expel((GameItem)(object)item);
				}
				if (((GameItem)item).parentInventory == null)
				{
					((GameItem)item).modifiedShape = (GridShape)new GridShapeBuilder(val2);
					parentInventory.UncheckedAccept((GameItem)(object)item);
				}
				if (((GameItem)item).parentInventory == parentInventory && parentInventory.childItems.Contains((GameItem)(object)item))
				{
					if (text == null)
					{
						owner.modData.Remove("billboards.hanging.layout.v1");
					}
					else
					{
						owner.modData["billboards.hanging.layout.v1"] = text;
					}
					TransferReceipt.Complete();
				}
				throw new InvalidOperationException("广告牌搬运或记录提交失败，已尝试回退一次；未补发物品。", innerException);
			}
			finally
			{
				busy = false;
			}
			if (place == null && mirrors.TryGetValue(((GameItem)item).uniqueId, out BillboardMirror value3))
			{
				value3.Dispose();
				mirrors.Remove(((GameItem)item).uniqueId);
			}
			ClienteleUIManager instance = ClienteleUIManager.Instance;
			if (instance != null)
			{
				instance.RefreshUI();
			}
			return true;
		}

		internal static void Remove(GameItem item)
		{
			if (!Quitting && !busy && CurrentOwner && !unavailable.ContainsKey(item.uniqueId))
			{
				if (mirrors.TryGetValue(item.uniqueId, out BillboardMirror value))
				{
					value.Dispose();
					mirrors.Remove(item.uniqueId);
				}
				if (records.Remove(item.uniqueId))
				{
					Commit();
				}
			}
		}

		internal static NativeTransfer? BeginNativeTransfer(SlotMarker slot)
		{
			if (Core.Enabled && initialized && !fault && !busy)
			{
				GameItem item = slot.item;
				GameItemElement val = (GameItemElement)(object)((item is GameItemElement) ? item : null);
				if (val != null && IsStored((GameItem)(object)val))
				{
					NativeTransfer nativeTransfer = new NativeTransfer
					{
						Item = val,
						Source = ((GameItem)val).parentInventory,
						Shape = ((GameItem)val).modifiedShape.Clone()
					};
					TransferReceipt.Begin((GameItem)(object)val, nativeTransfer.Source, slot.inventory ?? nativeTransfer.Source);
					busy = true;
					return nativeTransfer;
				}
			}
			return null;
		}

		internal static void FinishNativeTransfer(NativeTransfer context)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			GameItemElement item = context.Item;
			try
			{
				if (((GameItem)item).parentInventory == null && ((GameItem)item).unitCount > 0)
				{
					((GameItem)item).modifiedShape = (GridShape)new GridShapeBuilder(context.Shape);
					context.Source.UncheckedAccept((GameItem)(object)item);
					if (((GameItem)item).parentInventory != context.Source || !context.Source.childItems.Contains((GameItem)(object)item))
					{
						throw new InvalidOperationException("拖出广告牌后原实例归属未恢复，搬运记录已保留。");
					}
				}
				if (!IsStored((GameItem)(object)item) || ((GameItem)item).unitCount <= 0)
				{
					records.Remove(((GameItem)item).uniqueId);
				}
				Commit();
				TransferReceipt.Complete();
			}
			finally
			{
				busy = false;
			}
			if (!IsStored((GameItem)(object)item) && mirrors.TryGetValue(((GameItem)item).uniqueId, out BillboardMirror value))
			{
				value.Dispose();
				mirrors.Remove(((GameItem)item).uniqueId);
			}
			refreshAt = 0f;
			ClienteleUIManager instance = ClienteleUIManager.Instance;
			if (instance != null)
			{
				instance.RefreshUI();
			}
		}

		internal static void BeginDrag(ItemMouseDragHandler handler)
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			BillboardLightInput.Cancel();
			if (!handler.IsDraggingItem)
			{
				return;
			}
			GameItem currentItem = handler.GetCurrentItem();
			GameItemElement val = (GameItemElement)(object)((currentItem is GameItemElement) ? currentItem : null);
			if (val != null && !unavailable.ContainsKey(((GameItem)val).uniqueId) && Interactive && IsSign((GameItem)(object)val) && GeneralHelper.IsItemOwned((GameItem)(object)val) && !GeneralHelper.IsDisplayOnly((GameItem)(object)val) && ValidArt(val) && EnsureScene())
			{
				dragged = val;
				if (mirrors.TryGetValue(((GameItem)val).uniqueId, out BillboardMirror value))
				{
					value.SetVisible(visible: false);
					Vector2 fakeRotatorCenteredPosition = val.GetFakeRotatorCenteredPosition();
					val.MovePosition(Input.mousePosition.x - fakeRotatorCenteredPosition.x, Input.mousePosition.y - fakeRotatorCenteredPosition.y);
				}
			}
		}

		internal static bool DropOnWall(ItemMouseDragHandler handler)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			if (dragged == null || (object)handler.GetCurrentItem() != dragged || !Interactive || !EnsureScene() || (Object)(object)root == (Object)null)
			{
				return false;
			}
			Vector2 val = default(Vector2);
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(root, Vector2.op_Implicit(Input.mousePosition), PresentationCamera, ref val))
			{
				return false;
			}
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))..ctor(val.x, 0f - val.y);
			int num = SlotLayout.RegionAt(val2.x, val2.y, expanded);
			if (num == 0 || !WallInFront(Vector2.op_Implicit(Input.mousePosition)))
			{
				return false;
			}
			GameItemElement val3 = dragged;
			int uniqueID = ((GameItem)val3).GetUniqueID();
			(int, int) tuple = Size(val3);
			Placement placement = new Placement
			{
				uid = uniqueID,
				id = ((GameItem)val3).identifier,
				region = num,
				x = Mathf.RoundToInt((val2.x - (float)SlotLayout.OriginX(num)) / 16f - (float)tuple.Item1 * 0.5f),
				y = Mathf.RoundToInt((val2.y - 52f) / 16f - (float)tuple.Item2 * 0.5f),
				width = tuple.Item1,
				height = tuple.Item2
			};
			if (!SlotLayout.Fits(placement, tuple.Item1, tuple.Item2, expanded, mirrors.Values.Select((BillboardMirror m) => m.Placement)))
			{
				Core.Notice("广告牌之间不能重叠或超出挂区，已保留原位置。");
				return true;
			}
			if (!MoveWhole(val3, HangingStorage.Inventory, placement))
			{
				Core.Notice("独立挂位暂时放不下，广告牌仍留在原处。");
				return true;
			}
			if (mirrors.TryGetValue(uniqueID, out BillboardMirror value))
			{
				value.Placement = placement;
			}
			else
			{
				mirrors.Add(uniqueID, new BillboardMirror(val3, placement, root));
			}
			return true;
		}

		private static bool WallInFront(Vector2 point)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current)
			{
				position = point
			}, list);
			foreach (RaycastResult item in list)
			{
				RaycastResult current = item;
				if (dragged == null || !((RaycastResult)(ref current)).gameObject.transform.IsChildOf(((Component)dragged.fakeMiddle).transform))
				{
					StoreShutterButton instance = StoreShutterButton.Instance;
					if ((Object)(object)instance != (Object)null && (Object)(object)((Transform)root).parent == (Object)(object)((Transform)((Graphic)instance.blockerMaskObject).rectTransform).parent && ((Object)(object)((RaycastResult)(ref current)).gameObject == (Object)(object)instance.blockerGameObject || (Object)(object)((RaycastResult)(ref current)).gameObject == (Object)(object)((Component)instance.blockerMaskObject).gameObject))
					{
						return true;
					}
					return (Object)(object)((RaycastResult)(ref current)).gameObject.GetComponentInParent<Canvas>() == (Object)(object)((Component)root).GetComponent<Canvas>();
				}
			}
			return true;
		}

		internal static void EndDrag()
		{
			if (dragged != null)
			{
				GameItemElement item = dragged;
				dragged = null;
				if (!IsStored((GameItem)(object)item))
				{
					Remove((GameItem)(object)item);
				}
				refreshAt = 0f;
			}
		}

		internal static void CancelDrag()
		{
			dragged = null;
			refreshAt = 0f;
		}
	}
	internal sealed class BillboardMirror : IDisposable
	{
		internal readonly GameItemElement Item;

		internal readonly TreeNodeRender Node;

		internal Placement Placement;

		private readonly RectTransform line1;

		private readonly RectTransform line2;

		private readonly Image glow;

		private readonly Image tube;

		private readonly CanvasGroup original;

		private readonly bool addedGroup;

		private readonly bool oldRaycasts;

		private readonly bool oldInteractable;

		private readonly float oldAlpha;

		private Sprite? sprite;

		internal bool Visible { get; private set; }

		private bool SingleWire
		{
			get
			{
				if (((GameItem)Item).shape.width == 1)
				{
					return ((GameItem)Item).shape.height == 1;
				}
				return false;
			}
		}

		internal BillboardMirror(GameItemElement item, Placement placement, RectTransform parent)
		{
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			Item = item;
			Placement = placement;
			Node = TreeNodeRender.Instantiate((PixelElement)(object)item, "Hanging billboard " + ((GameItem)item).uniqueId);
			((Transform)Node.rectTransform).SetParent((Transform)(object)parent, false);
			((Component)Node).gameObject.layer = ((Component)parent).gameObject.layer;
			Node.InitEventHandler();
			((Behaviour)Node.image).enabled = true;
			((Graphic)Node.image).color = Color.white;
			Node.image.type = (Type)0;
			((Graphic)Node.image).raycastTarget = true;
			Node.image.preserveAspect = true;
			((Component)Node).gameObject.AddComponent<BillboardHitFilter>().Mirror = this;
			glow = Decoration(parent, "Billboard neon glow");
			((Transform)((Graphic)glow).rectTransform).SetAsFirstSibling();
			tube = Decoration(Node.rectTransform, "Billboard neon tube");
			line1 = Line(parent);
			line2 = Line(parent);
			((Transform)line1).SetAsFirstSibling();
			((Transform)line2).SetAsFirstSibling();
			CanvasGroup component = ((Component)item.middle).gameObject.GetComponent<CanvasGroup>();
			addedGroup = (Object)(object)component == (Object)null;
			original = (((Object)(object)component != (Object)null) ? component : ((Component)item.middle).gameObject.AddComponent<CanvasGroup>());
			oldAlpha = original.alpha;
			oldRaycasts = original.blocksRaycasts;
			oldInteractable = original.interactable;
			Refresh();
		}

		private static Image Decoration(RectTransform parent, string name)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = new GameObject(name, new Type[3]
			{
				typeof(RectTransform),
				typeof(CanvasRenderer),
				typeof(Image)
			})
			{
				layer = ((Component)parent).gameObject.layer
			};
			val.transform.SetParent((Transform)(object)parent, false);
			Image component = val.GetComponent<Image>();
			((Graphic)component).raycastTarget = false;
			((Graphic)component).color = Color.white;
			((Behaviour)component).enabled = false;
			return component;
		}

		private static RectTransform Line(RectTransform parent)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = new GameObject("Billboard grey suspension wire", new Type[3]
			{
				typeof(RectTransform),
				typeof(CanvasRenderer),
				typeof(Image)
			})
			{
				layer = ((Component)parent).gameObject.layer
			};
			RectTransform val2 = (RectTransform)val.transform;
			((Transform)val2).SetParent((Transform)(object)parent, false);
			Image component = val.GetComponent<Image>();
			((Graphic)component).color = Color32.op_Implicit(new Color32((byte)132, (byte)132, (byte)132, byte.MaxValue));
			((Graphic)component).raycastTarget = false;
			return val2;
		}

		private static void Rect(RectTransform rect, float x, float top, float width, float height)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = default(Vector2);
			((Vector2)(ref val))..ctor(0f, 1f);
			rect.pivot = val;
			Vector2 anchorMin = (rect.anchorMax = val);
			rect.anchorMin = anchorMin;
			((Transform)rect).localRotation = Quaternion.identity;
			((Transform)rect).localScale = Vector3.one;
			rect.anchoredPosition = new Vector2(x, 0f - top);
			rect.sizeDelta = new Vector2(width, height);
		}

		internal void Refresh()
		{
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			Sprite val = Item.background.image.sprite;
			if ((Object)(object)val != (Object)(object)sprite)
			{
				sprite = val;
				Node.image.sprite = sprite;
				BillboardNeon billboardNeon = BillboardNeon.Get(sprite);
				glow.sprite = billboardNeon?.Glow;
				tube.sprite = billboardNeon?.Core;
			}
			bool flag = BillboardManager.IsLit((GameItem)(object)Item);
			((Behaviour)glow).enabled = flag && (Object)(object)glow.sprite != (Object)null;
			((Behaviour)tube).enabled = flag && (Object)(object)tube.sprite != (Object)null;
			((Graphic)Node.image).color = (Color)(flag ? Color.white : new Color(0.38f, 0.38f, 0.38f, 1f));
			Vector2 val2 = BillboardManager.ArtworkSize(Item);
			float x = val2.x;
			float y = val2.y;
			float num = 10f * x;
			Rect rect = sprite.rect;
			float num2 = num / ((Rect)(ref rect)).width;
			float num3 = 10f * y;
			rect = sprite.rect;
			float num4 = num3 / ((Rect)(ref rect)).height;
			float num5 = Mathf.Round((float)(SlotLayout.OriginX(Placement.region) + Placement.x * 16) + ((float)(Placement.width * 16) - x) * 0.5f);
			float num6 = 52 + Placement.y * 16;
			Rect(Node.rectTransform, num5, num6, x, y);
			Rect(((Graphic)glow).rectTransform, num5 - num2, num6 - num4, x + num2 * 2f, y + num4 * 2f);
			Rect(((Graphic)tube).rectTransform, 0f, 0f, x, y);
			Rect(line1, Mathf.Round(num5 + x * (SingleWire ? 0.5f : 0.25f)), 26f, 1f, num6 - 26f);
			Rect(line2, Mathf.Round(num5 + x * 0.75f), 26f, 1f, num6 - 26f);
		}

		internal void SetVisible(bool visible)
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Invalid comparison between Unknown and I4
			if ((Object)(object)Node != (Object)null)
			{
				((Component)Node).gameObject.SetActive(visible);
			}
			Image val = (((Object)(object)Node != (Object)null) ? Node.image : null);
			Canvas val2 = (((Object)(object)val != (Object)null && (Object)(object)((Graphic)val).canvas != (Object)null) ? ((Graphic)val).canvas.rootCanvas : null);
			Camera val3 = (((Object)(object)val2 != (Object)null) ? val2.worldCamera : null);
			bool flag = (Object)(object)val2 != (Object)null && (int)val2.renderMode == 0;
			Visible = visible && (Object)(object)val != (Object)null && ((Behaviour)val).isActiveAndEnabled && (Object)(object)val.sprite != (Object)null && (Object)(object)val2 != (Object)null && ((Behaviour)val2).isActiveAndEnabled && (flag || ((Object)(object)val3 != (Object)null && ((Behaviour)val3).isActiveAndEnabled && (val3.cullingMask & (1 << ((Component)Node).gameObject.layer)) != 0));
			if ((Object)(object)line1 != (Object)null)
			{
				((Component)line1).gameObject.SetActive(Visible);
			}
			if ((Object)(object)line2 != (Object)null)
			{
				((Component)line2).gameObject.SetActive(Visible && !SingleWire);
			}
			if ((Object)(object)glow != (Object)null)
			{
				((Component)glow).gameObject.SetActive(Visible);
			}
			if ((Object)(object)original != (Object)null)
			{
				original.alpha = (Visible ? 0f : oldAlpha);
				original.blocksRaycasts = !Visible && oldRaycasts;
				original.interactable = !Visible && oldInteractable;
			}
		}

		public void Dispose()
		{
			SetVisible(visible: false);
			if (addedGroup && (Object)(object)original != (Object)null)
			{
				Object.Destroy((Object)(object)original);
			}
			if ((Object)(object)Node != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)Node).gameObject);
			}
			if ((Object)(object)line1 != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)line1).gameObject);
			}
			if ((Object)(object)line2 != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)line2).gameObject);
			}
			if ((Object)(object)glow != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)glow).gameObject);
			}
		}
	}
	public sealed class BillboardHitFilter : MonoBehaviour, ICanvasRaycastFilter
	{
		internal BillboardMirror? Mirror;

		public bool IsRaycastLocationValid(Vector2 point, Camera eventCamera)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (Mirror != null)
			{
				return BillboardManager.CanHit(Mirror, point);
			}
			return false;
		}
	}
	internal sealed class BillboardNeon : IDisposable
	{
		internal const int Padding = 10;

		private static readonly Dictionary<Sprite, BillboardNeon?> cache = new Dictionary<Sprite, BillboardNeon>();

		internal Sprite? Glow;

		internal Sprite? Core;

		private Texture2D? glowTexture;

		private Texture2D? coreTexture;

		internal static BillboardNeon? Get(Sprite source)
		{
			if (cache.TryGetValue(source, out BillboardNeon value))
			{
				return value;
			}
			BillboardNeon billboardNeon = new BillboardNeon();
			try
			{
				billboardNeon.Build(source);
				value = billboardNeon;
			}
			catch (Exception e)
			{
				billboardNeon.Dispose();
				value = null;
				CoreWarning(source, e);
			}
			cache.Add(source, value);
			return value;
		}

		private static void CoreWarning(Sprite source, Exception e)
		{
			PawnshopBillboards.Core.Warn("霓虹灯效未生成，保留广告牌原图；重新读档后可重建：" + ((Object)source).name + " / " + e.Message);
		}

		internal static void Clear()
		{
			foreach (BillboardNeon value in cache.Values)
			{
				value?.Dispose();
			}
			cache.Clear();
		}

		private static Color32[] Read(Sprite source, int width, int height)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Invalid comparison between Unknown and I4
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			Rect textureRect = source.textureRect;
			Vector2 textureRectOffset = source.textureRectOffset;
			RenderTexture active = RenderTexture.active;
			bool sRGBWrite = GL.sRGBWrite;
			RenderTexture temporary = RenderTexture.GetTemporary(((Texture)source.texture).width, ((Texture)source.texture).height, 0, (RenderTextureFormat)0);
			Texture2D val = null;
			try
			{
				GL.sRGBWrite = (int)QualitySettings.activeColorSpace == 1 && temporary.sRGB;
				Graphics.Blit((Texture)(object)source.texture, temporary);
				RenderTexture.active = temporary;
				val = new Texture2D((int)((Rect)(ref textureRect)).width, (int)((Rect)(ref textureRect)).height, (TextureFormat)4, false);
				val.ReadPixels(textureRect, 0, 0, false);
				Color32[] pixels = val.GetPixels32();
				Color32[] array = (Color32[])(object)new Color32[width * height];
				for (int i = 0; i < ((Texture)val).height; i++)
				{
					for (int j = 0; j < ((Texture)val).width; j++)
					{
						array[(i + (int)textureRectOffset.y) * width + j + (int)textureRectOffset.x] = pixels[i * ((Texture)val).width + j];
					}
				}
				return array;
			}
			finally
			{
				RenderTexture.active = active;
				GL.sRGBWrite = sRGBWrite;
				RenderTexture.ReleaseTemporary(temporary);
				if ((Object)(object)val != (Object)null)
				{
					Object.Destroy((Object)(object)val);
				}
			}
		}

		private void Build(Sprite source)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			Rect rect = source.rect;
			int num = (int)((Rect)(ref rect)).width;
			rect = source.rect;
			int num2 = (int)((Rect)(ref rect)).height;
			Color32[] array = Read(source, num, num2);
			int num3 = num + 20;
			int num4 = num2 + 20;
			Color[] array2 = (Color[])(object)new Color[num3 * num4];
			Color[] array3 = (Color[])(object)new Color[num * num2];
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					Color val = Color32.op_Implicit(array[i * num + j]);
					float num5 = Mathf.Max(val.r, Mathf.Max(val.g, val.b));
					float num6 = val.a * Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.34f, 0.82f, num5));
					Color val2 = (Color)((num5 > 0f) ? new Color(val.r / num5, val.g / num5, val.b / num5) : Color.black);
					array2[(i + 10) * num3 + j + 10] = new Color(val2.r * num6, val2.g * num6, val2.b * num6, num6);
					Color val3 = Color.Lerp(val2, Color.white, 0.55f);
					array3[i * num + j] = new Color(val3.r, val3.g, val3.b, num6 * 0.78f);
				}
			}
			Color[] array4 = Blur(array2, num3, num4, 1.15f, 3);
			Color[] array5 = Blur(array2, num3, num4, 3f, 9);
			Color[] array6 = (Color[])(object)new Color[num3 * num4];
			for (int k = 0; k < array6.Length; k++)
			{
				Color val4 = array4[k] * 1.5f + array5[k] * 2.4f;
				if (val4.a > 0f)
				{
					array6[k] = new Color(val4.r / val4.a, val4.g / val4.a, val4.b / val4.a, Mathf.Min(0.72f, val4.a));
				}
			}
			glowTexture = Texture(num3, num4, array6, (FilterMode)1, "Billboard neon halo");
			coreTexture = Texture(num, num2, array3, (FilterMode)0, "Billboard neon tube");
			Glow = Sprite.Create(glowTexture, new Rect(0f, 0f, (float)num3, (float)num4), new Vector2(0.5f, 0.5f), source.pixelsPerUnit, 0u, (SpriteMeshType)0);
			Core = Sprite.Create(coreTexture, new Rect(0f, 0f, (float)num, (float)num2), new Vector2(0.5f, 0.5f), source.pixelsPerUnit, 0u, (SpriteMeshType)0);
		}

		private static Texture2D Texture(int width, int height, Color[] pixels, FilterMode filter, string name)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			Texture2D val = new Texture2D(width, height, (TextureFormat)4, false)
			{
				name = name,
				filterMode = filter,
				wrapMode = (TextureWrapMode)1
			};
			try
			{
				val.SetPixels(pixels);
				val.Apply(false, true);
				return val;
			}
			catch
			{
				Object.Destroy((Object)(object)val);
				throw;
			}
		}

		private static Color[] Blur(Color[] pixels, int width, int height, float sigma, int radius)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			float[] array = new float[radius * 2 + 1];
			float num = 0f;
			for (int i = -radius; i <= radius; i++)
			{
				num += (array[i + radius] = Mathf.Exp((float)(-i * i) / (2f * sigma * sigma)));
			}
			for (int j = 0; j < array.Length; j++)
			{
				array[j] /= num;
			}
			Color[] array2 = (Color[])(object)new Color[pixels.Length];
			Color[] array3 = (Color[])(object)new Color[pixels.Length];
			for (int k = 0; k < height; k++)
			{
				for (int l = 0; l < width; l++)
				{
					for (int m = -radius; m <= radius; m++)
					{
						if (l + m >= 0 && l + m < width)
						{
							ref Color reference = ref array2[k * width + l];
							reference += pixels[k * width + l + m] * array[m + radius];
						}
					}
				}
			}
			for (int n = 0; n < height; n++)
			{
				for (int num2 = 0; num2 < width; num2++)
				{
					for (int num3 = -radius; num3 <= radius; num3++)
					{
						if (n + num3 >= 0 && n + num3 < height)
						{
							ref Color reference2 = ref array3[n * width + num2];
							reference2 += array2[(n + num3) * width + num2] * array[num3 + radius];
						}
					}
				}
			}
			return array3;
		}

		public void Dispose()
		{
			if ((Object)(object)Glow != (Object)null)
			{
				Object.Destroy((Object)(object)Glow);
			}
			if ((Object)(object)Core != (Object)null)
			{
				Object.Destroy((Object)(object)Core);
			}
			if ((Object)(object)glowTexture != (Object)null)
			{
				Object.Destroy((Object)(object)glowTexture);
			}
			if ((Object)(object)coreTexture != (Object)null)
			{
				Object.Destroy((Object)(object)coreTexture);
			}
		}
	}
	public sealed class Core : IMod
	{
		private GameObject? driver;

		internal static bool Enabled;

		public void Init(ModManifest manifest)
		{
		}

		public void OnEnable()
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			Enabled = true;
			Application.quitting += BillboardManager.BeginQuit;
			ModHook.OnGameLoadedLate += Loaded;
			ModHook.OnNewGameStarted += NewGame;
			driver = new GameObject("PawnshopBillboards.Runtime");
			Object.DontDestroyOnLoad((Object)(object)driver);
			driver.AddComponent<BillboardLoop>();
			Loaded();
		}

		public void OnDisable()
		{
			BillboardManager.Disable();
			Enabled = false;
			Application.quitting -= BillboardManager.BeginQuit;
			ModHook.OnGameLoadedLate -= Loaded;
			ModHook.OnNewGameStarted -= NewGame;
			if ((Object)(object)driver != (Object)null)
			{
				Object.Destroy((Object)(object)driver);
			}
			driver = null;
		}

		private static void Loaded()
		{
			BillboardManager.Reset();
			BillboardManager.Initialize();
		}

		private static void NewGame(string scenario)
		{
			Loaded();
		}

		internal static void Warn(string text)
		{
			Debug.LogWarning((object)("[更合理的广告牌] " + text));
		}

		internal static void Notice(string text)
		{
			StoreUIManager instance = StoreUIManager.Instance;
			if (instance != null)
			{
				instance.Notify(text, "default");
			}
		}
	}
	[DefaultExecutionOrder(1500)]
	public sealed class BillboardLoop : MonoBehaviour
	{
		private void OnEnable()
		{
			SceneManager.sceneUnloaded += Unloaded;
		}

		private void OnDisable()
		{
			SceneManager.sceneUnloaded -= Unloaded;
		}

		private static void Unloaded(Scene scene)
		{
			if (!BillboardManager.Quitting)
			{
				if (PlayerStore.HasInstance)
				{
					BillboardManager.BeforeSave(PlayerStore.Instance);
				}
				BillboardManager.Reset();
			}
		}

		private void LateUpdate()
		{
			if (!Core.Enabled || BillboardManager.Quitting)
			{
				return;
			}
			try
			{
				BillboardManager.Tick();
			}
			catch (Exception e)
			{
				BillboardManager.Fail(e);
			}
		}

		private void OnApplicationFocus(bool focus)
		{
			try
			{
				BillboardManager.SetApplicationFocus(focus);
			}
			catch (Exception e)
			{
				BillboardManager.Fail(e);
			}
		}

		private void OnApplicationPause(bool paused)
		{
			OnApplicationFocus(!paused && Application.isFocused);
		}

		private void OnApplicationQuit()
		{
			BillboardManager.BeginQuit();
		}
	}
	internal static class CounterStorage
	{
		internal static void IncludeInCounter(GraphNodeStorage firstNode, List<SaveItemNode> nodes)
		{
			GameItem bag = (GameItem)(object)((firstNode is GameItem) ? firstNode : null);
			if (bag == null || bag.identifier != "back_inv_save_bag" || (Object)(object)EmporiumEntry.Instance == (Object)null || HangingStorage.Inventory == null)
			{
				return;
			}
			GameItem[] array = ((GameInventory)HangingStorage.Inventory).childItems.Where(BillboardManager.IsStored).ToArray();
			if (array.Length == 0)
			{
				return;
			}
			Queue<GraphNodeStorage> queue = new Queue<GraphNodeStorage>(bag.children);
			Dictionary<GraphNodeStorage, int> dictionary = new Dictionary<GraphNodeStorage, int>();
			while (queue.Count > 0)
			{
				GraphNodeStorage val = queue.Dequeue();
				if (val is GameItem)
				{
					continue;
				}
				if (!dictionary.ContainsKey(val))
				{
					dictionary.Add(val, dictionary.Count);
				}
				foreach (GraphNodeStorage child in val.children)
				{
					queue.Enqueue(child);
				}
			}
			if (!dictionary.TryGetValue((GraphNodeStorage)(object)EmporiumEntry.Instance.backInvinvElement, out var value))
			{
				return;
			}
			SaveItemNode val2 = nodes.Single((SaveItemNode n) => n.uniqueId == bag.uniqueId && n.identifier == bag.identifier);
			HashSet<int> hashSet = new HashSet<int>(nodes.Select((SaveItemNode n) => n.uniqueId));
			long num = nodes.Max((SaveItemNode n) => n.uuid) + 1;
			List<SaveItemNode> list = new List<SaveItemNode>();
			GameItem[] array2 = array;
			foreach (GameItem val3 in array2)
			{
				if (!hashSet.Add(val3.uniqueId))
				{
					throw new InvalidOperationException("柜台内广告牌编号冲突，保留当前挂位快照。");
				}
				SaveItemNode item = HangingStorage.Capture(val3).item;
				item.uuid = num++;
				list.Add(item);
			}
			foreach (SaveItemNode item2 in list)
			{
				val2.childItems.Add(item2.uuid);
				val2.childItemInventoryNode.Add(value);
			}
			nodes.AddRange(list);
			array2 = array;
			foreach (GameItem val4 in array2)
			{
				PlayerStore.Instance.savedItemFeatureList.AddRange(val4.itemFeatures);
			}
		}

		internal static Dictionary<int, GameItemElement> FindOriginals(Dictionary<int, Placement> placements)
		{
			Dictionary<int, GameItemElement> dictionary = new Dictionary<int, GameItemElement>();
			List<GameItem> allItems = EmporiumEntry.Instance.GetAllItems();
			foreach (Placement p in placements.Values)
			{
				GameItem[] array = allItems.Where((GameItem i) => i.uniqueId == p.uid).ToArray();
				if (array.Length == 0)
				{
					continue;
				}
				if (array.Length != 1)
				{
					throw new InvalidOperationException("当前存档广告牌编号重复，保留原实例和挂位记录。");
				}
				GameItem obj = array[0];
				GameItemElement val = (GameItemElement)(object)((obj is GameItemElement) ? obj : null);
				if ((object)((val != null) ? ((GameItem)val).parentInventory : null) == EmporiumEntry.Instance.backInvinvElement)
				{
					val.ValidatePartialSprite();
					if (((GameItem)val).identifier != p.id || ((GameItem)val).unitCount <= 0 || !GeneralHelper.IsItemOwned((GameItem)(object)val) || !BillboardManager.IsSign((GameItem)(object)val) || !BillboardManager.ValidArt(val))
					{
						throw new InvalidOperationException("柜台内广告牌身份不符，保留原实例和挂位记录。");
					}
					dictionary.Add(p.uid, val);
				}
			}
			return dictionary;
		}

		internal static void TakeOriginals(IEnumerable<GameItemElement> items)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			foreach (GameItemElement item in items)
			{
				GameInventory parentInventory = ((GameItem)item).parentInventory;
				GridShape val = ((GameItem)item).modifiedShape.Clone();
				try
				{
					parentInventory.Expel((GameItem)(object)item);
					if (!((GameInventory)HangingStorage.Inventory).UncheckedAccept((GameItem)(object)item) || (object)((GameItem)item).parentInventory != HangingStorage.Inventory || !((GameInventory)HangingStorage.Inventory).childItems.Contains((GameItem)(object)item))
					{
						throw new InvalidOperationException("柜台内广告牌未到达挂位。");
					}
				}
				catch
				{
					if ((object)((GameItem)item).parentInventory == HangingStorage.Inventory)
					{
						((GameInventory)HangingStorage.Inventory).Expel((GameItem)(object)item);
					}
					if (((GameItem)item).parentInventory == null)
					{
						((GameItem)item).modifiedShape = (GridShape)new GridShapeBuilder(val);
						parentInventory.UncheckedAccept((GameItem)(object)item);
					}
					throw;
				}
			}
		}
	}
	internal static class HangingAdvertising
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static bool Contains(string id)
		{
			if (!ShowcaseHelper.IsShowcaseContainItemById(id))
			{
				return BillboardManager.ActiveSigns.Any((GameItem i) => i.identifier == id);
			}
			return true;
		}

		internal static List<GameItem> DisplayItems(GameInventory inventory)
		{
			List<GameItem> childItems = inventory.childItems;
			if ((object)inventory != EmporiumEntry.Instance.showcaseElement)
			{
				return childItems;
			}
			GameItem[] array = BillboardManager.ActiveSigns.ToArray();
			if (array.Length == 0)
			{
				return childItems;
			}
			List<GameItem> list = new List<GameItem>(childItems);
			GameItem[] array2 = array;
			foreach (GameItem item in array2)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list;
		}
	}
	[HarmonyPatch(typeof(ShowcaseHelper), "GetDisplayCasePrediction", new Type[] { })]
	internal static class HangingPredictionHook
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo source = AccessTools.Method(typeof(ShowcaseHelper), "IsShowcaseContainItemById", (Type[])null, (Type[])null);
			MethodInfo target = AccessTools.Method(typeof(HangingAdvertising), "Contains", (Type[])null, (Type[])null);
			foreach (CodeInstruction instruction in instructions)
			{
				if (CodeInstructionExtensions.Calls(instruction, source))
				{
					instruction.operand = target;
				}
				yield return instruction;
			}
		}
	}
	[HarmonyPatch(typeof(ClienteleUIManager), "RefreshUI", new Type[] { })]
	internal static class HangingForecastHook
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo source = AccessTools.PropertyGetter(typeof(GameInventory), "childItems");
			foreach (CodeInstruction instruction in instructions)
			{
				if (CodeInstructionExtensions.Calls(instruction, source))
				{
					yield return CodeInstructionExtensions.MoveBlocksFrom(CodeInstructionExtensions.MoveLabelsFrom(CodeInstruction.Call(typeof(HangingAdvertising), "DisplayItems", (Type[])null, (Type[])null), instruction), instruction);
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
	internal static class HangingOwnership
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static List<GameItem> GetOwnedItems(EmporiumEntry entry, bool isBurglary, bool includeAfterhour)
		{
			List<GameItem> allOwnedItems = entry.GetAllOwnedItems(isBurglary, includeAfterhour);
			AddOwned(allOwnedItems);
			return allOwnedItems;
		}

		internal static void AddOwned(List<GameItem> items)
		{
			foreach (GameItem ownedStoredItem in BillboardManager.OwnedStoredItems)
			{
				if (!items.Contains(ownedStoredItem))
				{
					items.Add(ownedStoredItem);
				}
			}
		}
	}
	[Serializable]
	internal sealed class StoredBillboard
	{
		public SaveItemNode item = new SaveItemNode();

		public string features = "";
	}
	internal static class HangingStorage
	{
		internal static GameGridInventory? Inventory;

		private static ES3Settings Settings()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			return new ES3Settings(Array.Empty<Enum>())
			{
				encryptionType = (EncryptionType)0,
				compressionType = (CompressionType)0
			};
		}

		internal static StoredBillboard Capture(GameItem original)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Expected O, but got Unknown
			if (original.children.Count != 0)
			{
				throw new InvalidOperationException("广告牌含有未适配的子容器，已保留原实例。");
			}
			return new StoredBillboard
			{
				item = new SaveItemNode
				{
					identifier = original.identifier,
					unitCount = original.unitCount,
					uuid = 0L,
					itemState = original.state.Clone(),
					itemShape = new GridShapeBuilder(original.shape),
					itemModifiedShape = new GridShapeBuilder(original.modifiedShape),
					itemType = ((object)original).GetType().FullName,
					name = original.name,
					shortDescription = original.shortDescription,
					flavorText = original.flavorText,
					uniqueId = original.GetUniqueID(),
					unitBaseValue = original.unitBaseValue,
					unitValue = original.unitValue,
					lateUnitValue = original.lateUnitValue,
					backupUnitValue = original.backupUnitValue,
					spriteAtlasPath = original.spriteAtlasPath,
					spritePath = original.spritePath,
					itemTypes = original.itemTypes.ToList()
				},
				features = EncodeFeatures(original.itemFeatures.ToList())
			};
		}

		internal static string EncodeFeatures(List<ItemFeature> features)
		{
			return Convert.ToBase64String(ES3.Serialize<List<ItemFeature>>(features, Settings()));
		}

		internal static void Create()
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			Inventory = new GameGridInventory(36, 10)
			{
				identifier = "billboards.hanging.inventory"
			};
			((Component)Inventory.handler).gameObject.SetActive(false);
		}

		internal static void ValidateSnapshots(List<StoredBillboard> stored, Dictionary<int, Placement> placements)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (StoredBillboard item in stored)
			{
				SaveItemNode val = item?.item;
				if (val == null || val.uniqueId <= 0 || val.unitCount <= 0 || !hashSet.Add(val.uniqueId) || !placements.TryGetValue(val.uniqueId, out Placement value) || value.id != val.identifier || val.itemType != typeof(GameItemElement).FullName || val.itemState == null || val.itemShape == null || val.itemModifiedShape == null || val.childItems == null || val.childItems.Count != 0 || val.childItemInventoryNode == null || val.childItemInventoryNode.Count != 0 || item.features == null)
				{
					throw new InvalidOperationException("独立挂位物品身份无效，已保留记录且未补发。");
				}
			}
		}

		internal static void Restore(List<StoredBillboard> stored, Dictionary<int, Placement> placements)
		{
			ValidateSnapshots(stored, placements);
			HashSet<int> hashSet = new HashSet<int>(from i in EmporiumEntry.Instance.GetAllItems()
				select i.uniqueId);
			List<List<ItemFeature>> list = new List<List<ItemFeature>>();
			foreach (StoredBillboard item2 in stored)
			{
				SaveItemNode n = item2.item;
				if (hashSet.Contains(n.uniqueId) || !DirectoryMaster.Has<GameItem>(n.identifier))
				{
					throw new InvalidOperationException("独立挂位物品编号冲突或原工厂无效，已保留记录且未补发。");
				}
				List<ItemFeature> list2 = ES3.Deserialize<List<ItemFeature>>(Convert.FromBase64String(item2.features), Settings());
				if (list2 == null || list2.Any((ItemFeature f) => f == null || f.parentItemUniqueId != n.uniqueId))
				{
					throw new InvalidOperationException("广告牌特征身份无效，已保留原记录。");
				}
				list.Add(list2);
			}
			List<GameItemElement> list3 = new List<GameItemElement>();
			try
			{
				for (int num = 0; num < stored.Count; num++)
				{
					SaveItemNode item = stored[num].item;
					GameItem obj = SaveManager.DecodeNodes(new List<SaveItemNode> { item });
					GameItemElement val = (GameItemElement)(object)(((obj is GameItemElement) ? obj : null) ?? throw new InvalidOperationException("独立挂位原实例类型无效。"));
					list3.Add(val);
					val.ValidatePartialSprite();
					if (((GameItem)val).uniqueId != item.uniqueId || ((GameItem)val).identifier != item.identifier || !BillboardManager.ValidArt(val) || !GeneralHelper.IsItemOwned((GameItem)(object)val))
					{
						throw new InvalidOperationException("独立挂位原实例恢复不一致。");
					}
					((GameItem)val).itemFeatures.Clear();
					((GameItem)val).itemFeatures.AddRange(list[num]);
					GeneralHelper.LoadLockItem((GameItem)(object)val);
					((GameItemFunc)val).onLoaded?.Invoke((GameItem)(object)val);
					if (!BillboardManager.IsSign((GameItem)(object)val))
					{
						throw new InvalidOperationException("独立挂位物品已不具备广告牌标记，已保留原记录。");
					}
					if (!((GameInventory)Inventory).UncheckedAccept((GameItem)(object)val) || (object)((GameItem)val).parentInventory != Inventory)
					{
						throw new InvalidOperationException("独立挂位未接受恢复的原实例。");
					}
				}
			}
			catch
			{
				foreach (StoredBillboard item3 in stored)
				{
					GameItem tempLink = item3.item.tempLink;
					GameItemElement val2 = (GameItemElement)(object)((tempLink is GameItemElement) ? tempLink : null);
					if (val2 != null && !list3.Contains(val2))
					{
						list3.Add(val2);
					}
				}
				foreach (GameItemElement item4 in list3)
				{
					((GameItem)item4).Destroy();
				}
				throw;
			}
			finally
			{
				foreach (StoredBillboard item5 in stored)
				{
					item5.item.tempLink = null;
				}
			}
		}

		internal static void Reset()
		{
			GameGridInventory? inventory = Inventory;
			if ((Object)(object)((inventory != null) ? inventory.handler : null) != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)Inventory.handler).gameObject);
			}
			Inventory = null;
		}
	}
	internal static class LayoutCodec
	{
		internal static string Write(IEnumerable<Placement> placements, List<StoredBillboard> stored)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Expected O, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Expected O, but got Unknown
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Expected O, but got Unknown
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			JArray val = new JArray();
			foreach (Placement placement in placements)
			{
				val.Add((JToken)new JObject
				{
					["uid"] = JToken.op_Implicit(placement.uid),
					["id"] = JToken.op_Implicit(placement.id),
					["region"] = JToken.op_Implicit(placement.region),
					["x"] = JToken.op_Implicit(placement.x),
					["y"] = JToken.op_Implicit(placement.y),
					["width"] = JToken.op_Implicit(placement.width),
					["height"] = JToken.op_Implicit(placement.height),
					["lit"] = JToken.op_Implicit(placement.lit)
				});
			}
			JArray val2 = new JArray();
			foreach (StoredBillboard item in stored)
			{
				val2.Add((JToken)new JObject
				{
					["item"] = (JToken)(object)NativeNode(item.item),
					["features"] = JToken.op_Implicit(item.features)
				});
			}
			return ((JToken)new JObject
			{
				["version"] = JToken.op_Implicit(2),
				["items"] = (JToken)(object)val,
				["stored"] = (JToken)(object)val2
			}).ToString((Formatting)0, Array.Empty<JsonConverter>());
		}

		internal static LayoutRecord Read(string json, out bool lostLegacy)
		{
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			lostLegacy = false;
			JObject val = JObject.Parse(json);
			int num = Integer(val, "version");
			if (num != 1 && num != 2)
			{
				throw new InvalidOperationException("未知挂位记录版本，未覆盖原记录。");
			}
			if (num == 2 && ((JContainer)val).Count == 1)
			{
				lostLegacy = true;
				return new LayoutRecord();
			}
			JToken obj = val["items"];
			JToken obj2 = ((obj is JArray) ? obj : null);
			if (obj2 == null || (num == 2 && !(val["stored"] is JArray)))
			{
				throw new InvalidOperationException("旧广告牌记录缺少挂位或物品列表，无法据此恢复原牌；原记录已保留，未当作空挂位覆盖。");
			}
			LayoutRecord layoutRecord = new LayoutRecord
			{
				version = num
			};
			foreach (JToken item in (JArray)obj2)
			{
				JObject obj3 = (JObject)(object)(((item is JObject) ? item : null) ?? throw new InvalidOperationException("挂位记录损坏。"));
				layoutRecord.items.Add(new Placement
				{
					uid = Integer(obj3, "uid"),
					id = Text(obj3, "id"),
					region = Integer(obj3, "region"),
					x = Integer(obj3, "x"),
					y = Integer(obj3, "y"),
					width = Integer(obj3, "width"),
					height = Integer(obj3, "height"),
					lit = Lighting(obj3)
				});
			}
			if (num == 2)
			{
				foreach (JToken item2 in (JArray)val["stored"])
				{
					JObject val2 = (JObject)(object)(((item2 is JObject) ? item2 : null) ?? throw new InvalidOperationException("挂位物品记录损坏。"));
					layoutRecord.stored.Add(new StoredBillboard
					{
						item = ReadNative(val2["item"]),
						features = Text(val2, "features")
					});
				}
			}
			return layoutRecord;
		}

		internal static JObject NativeNode(SaveItemNode node)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			JToken obj = JObject.Parse(JsonUtility.ToJson((object)new SaveState
			{
				saveItems = new List<SaveItemNode> { node }
			}))["saveItems"];
			JArray val = (JArray)(object)((obj is JArray) ? obj : null);
			if (val != null && ((JContainer)val).Count == 1)
			{
				JToken obj2 = val[0];
				JObject val2 = (JObject)(object)((obj2 is JObject) ? obj2 : null);
				if (val2 != null)
				{
					SaveItemNode val3 = ReadNative((JToken?)(object)val2);
					if (val3.uniqueId != node.uniqueId || val3.identifier != node.identifier || val3.unitCount != node.unitCount)
					{
						throw new InvalidOperationException("广告牌原生快照身份不一致，未提交记录。");
					}
					return val2;
				}
			}
			throw new InvalidOperationException("广告牌原生快照缺少物品，未提交记录。");
		}

		private static SaveItemNode ReadNative(JToken? item)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			if (!(item is JObject))
			{
				throw new InvalidOperationException("广告牌原生物品节点缺失。");
			}
			SaveState val = JsonUtility.FromJson<SaveState>(((JToken)new JObject { ["saveItems"] = (JToken)new JArray((object)item.DeepClone()) }).ToString((Formatting)0, Array.Empty<JsonConverter>()));
			if (val?.saveItems == null || val.saveItems.Count != 1 || val.saveItems[0] == null)
			{
				throw new InvalidOperationException("广告牌原生物品节点未能解码。");
			}
			return val.saveItems[0];
		}

		private static bool Lighting(JObject obj)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Invalid comparison between Unknown and I4
			if (obj.ContainsKey("lit"))
			{
				JToken obj2 = obj["lit"];
				if (obj2 == null || (int)obj2.Type != 9)
				{
					throw new InvalidOperationException("广告牌开关字段损坏，原记录已保留。");
				}
				return Extensions.Value<bool>((IEnumerable<JToken>)obj["lit"]);
			}
			return true;
		}

		private static int Integer(JObject obj, string key)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			JToken obj2 = obj[key];
			if (obj2 == null || (int)obj2.Type != 6)
			{
				throw new InvalidOperationException("挂位记录缺少整数字段：" + key);
			}
			return Extensions.Value<int>((IEnumerable<JToken>)obj[key]);
		}

		private static string Text(JObject obj, string key)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Invalid comparison between Unknown and I4
			JToken obj2 = obj[key];
			if (obj2 == null || (int)obj2.Type != 8)
			{
				throw new InvalidOperationException("挂位记录缺少文本字段：" + key);
			}
			return Extensions.Value<string>((IEnumerable<JToken>)obj[key]);
		}
	}
	[HarmonyPatch(typeof(ItemMouseDoubleClickHandler), "DoubleClickAction", new Type[]
	{
		typeof(GameItem),
		typeof(Vector2)
	})]
	internal static class DoubleClickHook
	{
		private static bool Prefix(GameItem newItem, Vector2 mousePosition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			BillboardLightInput.Cancel();
			try
			{
				return !BillboardManager.TryToggle(newItem, mousePosition);
			}
			catch (Exception e)
			{
				BillboardManager.Fail(e);
				return false;
			}
		}
	}
	[HarmonyPatch(typeof(ItemMouseDragHandler), "StartDrag", new Type[] { })]
	internal static class DragStartHook
	{
		private static void Postfix(ItemMouseDragHandler __instance)
		{
			try
			{
				BillboardManager.BeginDrag(__instance);
			}
			catch (Exception e)
			{
				BillboardManager.Fail(e);
			}
		}
	}
	[HarmonyPatch(typeof(ItemMouseDragHandler), "EndDrag", new Type[] { })]
	internal static class DragEndHook
	{
		private static void Prefix(ItemMouseDragHandler __instance, ref GameInventory ___lastInventory, ref GameItem ___lastItem)
		{
			try
			{
				if (BillboardManager.DropOnWall(__instance))
				{
					___lastInventory = null;
					___lastItem = null;
				}
			}
			catch (Exception e)
			{
				BillboardManager.Fail(e);
			}
		}

		private static void Postfix()
		{
			try
			{
				BillboardManager.EndDrag();
			}
			catch (Exception e)
			{
				BillboardManager.Fail(e);
			}
		}
	}
	[HarmonyPatch(typeof(ItemMouseDragHandler), "OnEventReset", new Type[] { })]
	internal static class DragResetHook
	{
		private static void Postfix()
		{
			BillboardManager.CancelDrag();
		}
	}
	[HarmonyPatch(typeof(SlotMarker), "TryAcceptOnce", new Type[] { typeof(int) })]
	internal static class HangingTransferHook
	{
		private static bool Prefix(SlotMarker __instance, ref int __result, out BillboardManager.NativeTransfer? __state)
		{
			__state = null;
			try
			{
				__state = BillboardManager.BeginNativeTransfer(__instance);
				return true;
			}
			catch (Exception e)
			{
				BillboardManager.Fail(e);
				__result = 0;
				return false;
			}
		}

		private static Exception? Finalizer(Exception? __exception, BillboardManager.NativeTransfer? __state)
		{
			if (__state == null)
			{
				return __exception;
			}
			try
			{
				BillboardManager.FinishNativeTransfer(__state);
			}
			catch (Exception e)
			{
				BillboardManager.Fail(e);
			}
			if (__exception != null)
			{
				Core.Warn("广告牌原生搬运异常已隔离，原实例或恢复记录保留：" + __exception);
			}
			return null;
		}
	}
	[HarmonyPatch(typeof(GameItemElement), "Destroy", new Type[] { })]
	internal static class ItemDestructionHook
	{
		private static void Prefix(GameItemElement __instance, out bool __state)
		{
			__state = Core.Enabled && BillboardManager.IsStored((GameItem)(object)__instance);
		}

		private static void Postfix(GameItemElement __instance, bool __state)
		{
			if (!__state || !Core.Enabled)
			{
				return;
			}
			try
			{
				BillboardManager.Remove((GameItem)(object)__instance);
			}
			catch (Exception e)
			{
				BillboardManager.Fail(e);
			}
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "SaveGame", new Type[] { })]
	internal static class HangingSaveHook
	{
		private static void Prefix(PlayerStore __instance)
		{
			BillboardManager.BeforeSave(__instance);
		}
	}
	[HarmonyPatch(typeof(SaveManager), "EncodeNodes", new Type[] { typeof(GraphNodeStorage) })]
	internal static class InsideCounterSaveHook
	{
		private static void Postfix(GraphNodeStorage firstNode, List<SaveItemNode> __result)
		{
			try
			{
				CounterStorage.IncludeInCounter(firstNode, __result);
			}
			catch (Exception ex)
			{
				Core.Warn("广告牌柜台内保存接入失败，保留挂位快照，原版保存继续：" + ex);
			}
		}
	}
	[HarmonyPatch(typeof(ShowcaseHelper), "IsShowcaseContainItemById", new Type[] { typeof(string) })]
	internal static class HangingAdvertisingHook
	{
		private static void Postfix(string id, ref bool __result)
		{
			if (!__result)
			{
				__result = BillboardManager.ActiveSigns.Any((GameItem i) => i.identifier == id);
			}
		}
	}
	[HarmonyPatch(typeof(ShowcaseHelper), "HasShowcaseSpecialItem", new Type[] { })]
	internal static class HangingSpecialHook
	{
		private static void Postfix(ref bool __result)
		{
			if (!__result)
			{
				__result = BillboardManager.ActiveSigns.Any((GameItem i) => i.IsTag("SHOWCASE_SPECIAL_ITEM_TAG"));
			}
		}
	}
	[HarmonyPatch(typeof(ShowcaseHelper), "GetShowCaseContainItemCount", new Type[] { typeof(string) })]
	internal static class HangingSignCountHook
	{
		private static void Postfix(string id, ref int __result)
		{
			__result += BillboardManager.ActiveSigns.Count((GameItem i) => i.identifier == id);
		}
	}
	[HarmonyPatch(typeof(EmporiumEntry), "GetAllItems", new Type[] { })]
	internal static class HangingAllItemsHook
	{
		private static void Postfix(List<GameItem> __result)
		{
			Add(__result, ownedOnly: false);
		}

		internal static void Add(List<GameItem> items, bool ownedOnly)
		{
			if (ownedOnly)
			{
				HangingOwnership.AddOwned(items);
				return;
			}
			foreach (GameItem storedItem in BillboardManager.StoredItems)
			{
				if (!items.Contains(storedItem))
				{
					items.Add(storedItem);
				}
			}
		}
	}
	[HarmonyPatch]
	internal static class DoctorHangingOwnershipHook
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			yield return AccessTools.Method(typeof(StoreClientList), "PlaceInventorInventory", new Type[1] { typeof(bool) }, (Type[])null);
			yield return AccessTools.Method(typeof(StoreClientList), "PlaceInventorSpecialStock", Type.EmptyTypes, (Type[])null);
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo source = AccessTools.Method(typeof(EmporiumEntry), "GetAllOwnedItems", new Type[2]
			{
				typeof(bool),
				typeof(bool)
			}, (Type[])null);
			MethodInfo target = AccessTools.Method(typeof(HangingOwnership), "GetOwnedItems", (Type[])null, (Type[])null);
			foreach (CodeInstruction instruction in instructions)
			{
				if (CodeInstructionExtensions.Calls(instruction, source))
				{
					yield return CodeInstructionExtensions.MoveBlocksFrom(CodeInstructionExtensions.MoveLabelsFrom(new CodeInstruction(OpCodes.Call, (object)target), instruction), instruction);
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
	[HarmonyPatch(typeof(EmporiumEntry), "GetAllOwnedItems", new Type[]
	{
		typeof(bool),
		typeof(bool)
	})]
	internal static class HangingOwnedItemsHook
	{
		private static void Postfix(List<GameItem> __result)
		{
			HangingAllItemsHook.Add(__result, ownedOnly: true);
		}
	}
	[HarmonyPatch(typeof(EmporiumEntry), "GetAllOwnedItemsShallow", new Type[] { typeof(bool) })]
	internal static class HangingOwnedShallowHook
	{
		private static void Postfix(List<GameItem> __result)
		{
			HangingAllItemsHook.Add(__result, ownedOnly: true);
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "LoadGame")]
	internal static class LoadHook
	{
		private static void Prefix()
		{
			BillboardManager.Reset();
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "StartNewGame")]
	internal static class NewGameHook
	{
		private static void Prefix()
		{
			BillboardManager.Reset();
		}
	}
	[HarmonyPatch(typeof(StoreClientMono), "OnClick", new Type[] { })]
	internal static class CustomerClickHook
	{
		private static bool Prefix()
		{
			return !BillboardManager.PointerOnBillboard();
		}
	}
	internal static class SlotLayout
	{
		internal const int Cell = 16;

		internal const int Columns = 18;

		internal const int Rows = 5;

		internal const int Top = 52;

		internal const int Main = 2;

		internal const int Left = 1;

		internal static int OriginX(int region)
		{
			if (region != 2)
			{
				return 2;
			}
			return 336;
		}

		internal static IEnumerable<int> Regions(bool expanded)
		{
			yield return 2;
			if (expanded)
			{
				yield return 1;
			}
		}

		internal static bool Fits(Placement p, int width, int height, bool expanded, IEnumerable<Placement> occupied)
		{
			if (Regions(expanded).Contains(p.region) && p.x >= 0 && p.y >= 0 && width > 0 && height > 0 && p.x + width <= 18 && p.y + height <= 5)
			{
				return !occupied.Any((Placement o) => o.uid != p.uid && o.region == p.region && p.x < o.x + o.width && p.x + width > o.x && p.y < o.y + o.height && p.y + height > o.y);
			}
			return false;
		}

		internal static int PreferredRegion(float roomX, bool expanded)
		{
			if (!expanded || !(roomX < 313f))
			{
				return 2;
			}
			return 1;
		}

		internal static int RegionAt(float roomX, float roomY, bool expanded)
		{
			if (roomY < 52f || roomY >= 132f)
			{
				return 0;
			}
			return Regions(expanded).FirstOrDefault((int r) => roomX >= (float)OriginX(r) && roomX < (float)(OriginX(r) + 288));
		}

		internal static Placement? Find(int uid, string id, int width, int height, bool expanded, IEnumerable<Placement> occupied, int preferredRegion = 2)
		{
			foreach (int item in from r in Regions(expanded)
				orderby (r != preferredRegion) ? 1 : 0
				select r)
			{
				for (int num = 0; num + height <= 5; num++)
				{
					foreach (int item2 in from x in Enumerable.Range(0, Math.Max(0, 18 - width + 1))
						orderby Math.Abs(2 * x + width - 18), x
						select x)
					{
						Placement placement = new Placement
						{
							uid = uid,
							id = id,
							region = item,
							x = item2,
							y = num,
							width = width,
							height = height
						};
						if (Fits(placement, width, height, expanded, occupied))
						{
							return placement;
						}
					}
				}
			}
			return null;
		}
	}
	[Serializable]
	internal sealed class Placement
	{
		public int uid;

		public int region;

		public int x;

		public int y;

		public int width;

		public int height;

		public string id = "";

		public bool lit = true;
	}
	[Serializable]
	internal sealed class LayoutRecord
	{
		public int version = 2;

		public List<Placement> items = new List<Placement>();

		public List<StoredBillboard> stored = new List<StoredBillboard>();
	}
	internal static class TransferReceipt
	{
		internal const string Key = "billboards.hanging.pending.v1";

		internal static void Begin(GameItem original, GameInventory source, GameInventory destination)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			StoredBillboard storedBillboard = HangingStorage.Capture(original);
			string value = ((JToken)new JObject
			{
				["version"] = JToken.op_Implicit(2),
				["source"] = JToken.op_Implicit(source.identifier),
				["destination"] = JToken.op_Implicit(destination.identifier),
				["item"] = (JToken)(object)LayoutCodec.NativeNode(storedBillboard.item),
				["features"] = JToken.op_Implicit(storedBillboard.features)
			}).ToString((Formatting)0, Array.Empty<JsonConverter>());
			PlayerStore instance;
			PlayerStore obj = (instance = PlayerStore.Instance);
			if (instance.modData == null)
			{
				instance.modData = new Dictionary<string, string>();
			}
			if (obj.modData.ContainsKey("billboards.hanging.pending.v1"))
			{
				throw new InvalidOperationException("尚有未核对的广告牌搬运记录，未覆盖。");
			}
			obj.modData["billboards.hanging.pending.v1"] = value;
		}

		internal static void Complete()
		{
			PlayerStore.Instance.modData.Remove("billboards.hanging.pending.v1");
		}
	}
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '9.1.0.7988')
