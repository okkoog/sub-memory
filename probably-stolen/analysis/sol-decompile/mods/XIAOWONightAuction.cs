using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETStandard,Version=v2.1", FrameworkDisplayName = ".NET Standard 2.1")]
[assembly: AssemblyCompany("XIAOWONightAuction")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("2.1.7.0")]
[assembly: AssemblyInformationalVersion("2.1.7+f3b262f32ec241772b5b9c2015298555dc1b17fc")]
[assembly: AssemblyProduct("XIAOWONightAuction")]
[assembly: AssemblyTitle("XIAOWONightAuction")]
[assembly: AssemblyVersion("2.1.7.0")]
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
namespace XIAOWONightAuction
{
	internal static class AuctionLoadLifecycle
	{
		private sealed class Work
		{
			internal GameItem Item;

			internal string Id;

			internal int UniqueId;

			internal int Attempts;
		}

		private static readonly Queue<Work> Pending = new Queue<Work>();

		private static PlayerStore _store;

		private static string _run;

		private static int _slot;

		private static int _retryAt;

		private static int _scanAttempts;

		private static bool _scan;

		private static bool _newGame;

		private static bool _warm;

		internal static bool IsLoading { get; private set; }

		internal static long Generation { get; private set; }

		internal static void BeginLoad()
		{
			Dispose();
			IsLoading = true;
		}

		internal static void RequestNewGame()
		{
			BeginLoad();
			_newGame = true;
		}

		internal static void Loaded(PlayerStore store)
		{
			Dispose();
			_store = store;
			_run = store.runID;
			_slot = store.saveSlotId;
			Generation++;
			_scan = (_warm = true);
			_retryAt = Time.frameCount + 1;
			AuctionState.MigrateKeys();
			Core.Log?.Msg($"[Load] generation={Generation}; slot={_slot}; run={_run}; official late callback.");
		}

		internal static void Tick()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (Core.IsShuttingDown || !PlayerStore.HasInstance)
			{
				return;
			}
			Scene activeScene = SceneManager.GetActiveScene();
			if (((Scene)(ref activeScene)).name != "InventoryScene")
			{
				return;
			}
			if (_newGame && (Object)(object)EmporiumEntry.Instance != (Object)null)
			{
				Loaded(PlayerStore.Instance);
				AuctionPhone.OnStoreReady();
			}
			if (IsLoading || _store == null)
			{
				return;
			}
			PlayerStore instance = PlayerStore.Instance;
			if (instance != _store || instance.runID != _run || instance.saveSlotId != _slot)
			{
				Dispose();
			}
			else
			{
				if (Time.frameCount < _retryAt)
				{
					return;
				}
				if (_scan)
				{
					try
					{
						_scanAttempts++;
						foreach (GameItem item in instance.FindAllItem(true))
						{
							if (item != null && (AuctionExclusiveCatalog.Items.ContainsKey(item.identifier) || item.identifier == "xiaowo_auction_container"))
							{
								Pending.Enqueue(new Work
								{
									Item = item,
									Id = item.identifier,
									UniqueId = item.uniqueId
								});
							}
						}
						_scan = false;
					}
					catch (Exception arg)
					{
						Pending.Clear();
						_scan = _scanAttempts < 3;
						_retryAt = Time.frameCount + 30;
						Core.Log?.Warning($"[Load] generation={Generation}; stage=snapshot; attempt={_scanAttempts}/3; {arg}");
						return;
					}
				}
				int num = Math.Min(8, Pending.Count);
				for (int i = 0; i < num; i++)
				{
					Work work = Pending.Dequeue();
					try
					{
						work.Attempts++;
						if (work.Item.identifier != work.Id || work.Item.uniqueId != work.UniqueId)
						{
							throw new InvalidOperationException("Item identity changed");
						}
						if (work.Id == "xiaowo_auction_container")
						{
							AuctionContainer.Repair(work.Item);
						}
						else
						{
							AuctionExclusiveFactory.Repair(work.Item);
						}
						if (GeneralHelper.IsItemEffectivelyOwned(work.Item))
						{
							AuctionCollection.Discover(work.Id);
						}
						if (AuctionExclusiveCatalog.Items.TryGetValue(work.Id, out var value) && AuctionExclusiveFactory.NeedsRepair(work.Item, value))
						{
							throw new InvalidOperationException("Incomplete item repair");
						}
					}
					catch (Exception ex)
					{
						Core.Log?.Warning($"[Load] generation={Generation}; item={work.Id}#{work.UniqueId}; stage=repair; attempt={work.Attempts}/3; {ex}");
						if (work.Attempts < 3)
						{
							Pending.Enqueue(work);
						}
						_retryAt = Time.frameCount + 30;
					}
				}
				if (!_scan && Pending.Count == 0 && _warm)
				{
					_warm = false;
					Stopwatch stopwatch = Stopwatch.StartNew();
					AuctionCatalog.WarmPools();
					Core.Log?.Msg($"[Load] generation={Generation}; item pool warmup={stopwatch.Elapsed.TotalMilliseconds:F2} ms; completed before opening the auction UI.");
				}
			}
		}

		internal static void Dispose()
		{
			Pending.Clear();
			_store = null;
			_run = null;
			_scan = (_newGame = (_warm = false));
			_scanAttempts = 0;
			IsLoading = false;
			AuctionItemLinks.ClearTemplates();
			AuctionForgedPass.ClearTemplate();
			AuctionContainer.ClearViews();
			AuctionCatalog.InvalidatePools();
		}
	}
	internal static class AuctionPointerHitbox
	{
		internal static int Apply(GameItemElement element)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			if (element == null || (((GameItem)element).identifier != "xiaowo_auction_container" && ((GameItem)element).identifier != "xiaowo_auction_phone_card" && !AuctionExclusiveCatalog.IsExclusive(((GameItem)element).identifier)))
			{
				return 0;
			}
			Vector2 size = default(Vector2);
			((Vector2)(ref size))..ctor((float)element.widthPixels, (float)element.heightPixels);
			int num = 0;
			TreeNodeRender val = element.background ?? element.handler;
			num += Configure(val, size);
			if ((Object)(object)element.handler != (Object)(object)val)
			{
				num += Disable(element.handler);
			}
			num += Configure(element.fakeBackground, size);
			num += Disable(element.inner);
			num += Disable(element.middle);
			num += Disable(element.fakeInner);
			return num + Disable(element.fakeMiddle);
		}

		internal static bool IsConfigured(TreeNodeRender node, Vector2 size)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			Image val = ((node != null) ? node.image : null);
			RectTransform val2 = ((node != null) ? node.rectTransform : null);
			if ((Object)(object)val != (Object)null && (Object)(object)val2 != (Object)null && ((Graphic)val).raycastTarget && val.alphaHitTestMinimumThreshold <= 0f)
			{
				return Vector2.Distance(val2.sizeDelta, size) < 0.01f;
			}
			return false;
		}

		internal static bool IsDisabled(TreeNodeRender node)
		{
			Image val = ((node != null) ? node.image : null);
			if (!((Object)(object)val == (Object)null))
			{
				return !((Graphic)val).raycastTarget;
			}
			return true;
		}

		private static int Configure(TreeNodeRender node, Vector2 size)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			Image val = ((node != null) ? node.image : null);
			if ((Object)(object)val == (Object)null)
			{
				return 0;
			}
			if ((Object)(object)node.rectTransform != (Object)null)
			{
				node.rectTransform.sizeDelta = size;
			}
			((Graphic)val).raycastTarget = true;
			val.alphaHitTestMinimumThreshold = 0f;
			return 1;
		}

		private static int Disable(TreeNodeRender node)
		{
			Image val = ((node != null) ? node.image : null);
			if ((Object)(object)val == (Object)null)
			{
				return 0;
			}
			((Graphic)val).raycastTarget = false;
			return 1;
		}
	}
	[HarmonyPatch(typeof(GameItemElement), "ValidateSelect")]
	internal static class AuctionPointerHitboxPatch
	{
		private static void Postfix(GameItemElement __instance)
		{
			AuctionPointerHitbox.Apply(__instance);
		}
	}
	[HarmonyPatch(typeof(GameItemElement), "Validate")]
	internal static class AuctionPointerValidatePatch
	{
		private static void Postfix(GameItemElement __instance)
		{
			AuctionPointerHitbox.Apply(__instance);
		}
	}
	public sealed class Core : IMod
	{
		internal const string Id = "xiaowo_night_auction";

		internal const string LocationId = "xiaowo_night_auction";

		private GameObject _driver;

		private bool _enabled;

		internal static DelegateAfterhourLocation Location;

		internal static AuctionLogger Log { get; private set; }

		internal static bool IsShuttingDown { get; private set; } = true;

		internal static bool StoreReady
		{
			get
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				if (!IsShuttingDown && PlayerStore.HasInstance)
				{
					Scene activeScene = SceneManager.GetActiveScene();
					if (((Scene)(ref activeScene)).name == "InventoryScene")
					{
						return !AuctionLoadLifecycle.IsLoading;
					}
				}
				return false;
			}
		}

		public void Init(ModManifest manifest)
		{
			Log = new AuctionLogger(manifest);
		}

		public void OnEnable()
		{
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Expected O, but got Unknown
			if (!_enabled)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				IsShuttingDown = false;
				_enabled = true;
				ModHook.OnModItemDirectoryInit += AuctionItemRegistration.Register;
				ModHook.OnModAssetsLoaded += OfficialAssets.Loaded;
				ModHook.OnLootTablesLoaded += AuctionCatalog.InvalidatePools;
				ModHook.OnGameLoadedInit += BeforeLoad;
				ModHook.OnGameLoadedLate += AfterLoad;
				ModHook.OnNewGameStarted += NewGame;
				ModHook.OnGenerateCustomerLate += GeneratedCustomers;
				ModHook.OnWakingUpLate += AfterWake;
				ModHook.OnReturningStoreLate += Returned;
				SceneManager.sceneUnloaded += SceneUnloaded;
				Location = ModHelper.AddAfterhourLocation("xiaowo_night_auction", "xiaowo_night_auction", (Func<string>)(() => AuctionText.Get("map_entry", "集装箱拍卖")), (Action)delegate
				{
					AuctionController.Open();
					Location.panelObject = AuctionController.Root;
				}, (Action)delegate
				{
					AuctionController.Close(returnToStore: false);
				}, (Action<CustomUIBuilder>)null, "xiaowo_night_auction:auction_destination", "xiaowo_night_auction:auction_background", (string)null, (Func<bool>)null, (Func<bool>)null);
				AuctionNightMarket.Register();
				_driver = new GameObject("XIAOWONightAuction.OfficialDriver");
				Object.DontDestroyOnLoad((Object)(object)_driver);
				_driver.AddComponent<AuctionUpdateDriver>();
				Log.Msg(string.Format("Official Mono enabled in {0:F2} ms; Harmony owner=mod.{1} (loader-managed, no manual pass). Resources/items await official callbacks.", stopwatch.Elapsed.TotalMilliseconds, "xiaowo_night_auction"));
			}
		}

		private static void BeforeLoad()
		{
			AuctionController.Dispose(returnToStore: false);
			AuctionPhone.Dispose();
			AuctionMerchantRelations.Dispose();
			WildeExpressEvidence.Dispose();
			AuctionLoadLifecycle.BeginLoad();
		}

		private static void AfterLoad()
		{
			if (!IsShuttingDown && PlayerStore.HasInstance)
			{
				AuctionLoadLifecycle.Loaded(PlayerStore.Instance);
				AuctionController.EnsureRuntime();
				AfterWake();
			}
		}

		private static void NewGame(string _)
		{
			AuctionLoadLifecycle.RequestNewGame();
		}

		private static void GeneratedCustomers(StoreClientManager manager)
		{
			AuctionMerchants.HandleDailyVisitors(manager);
			AuctionPhone.HandleVisitors(manager);
		}

		private static void AfterWake()
		{
			if (StoreReady)
			{
				WildeExpressEvidence.RegisterForLoadedStore();
				AuctionNightMarket.EnsureToday();
				AuctionPhone.OnStoreReady();
				AuctionMerchants.RepairSavedBuyerFactions(PlayerStore.Instance.storeClientManager);
			}
		}

		private static void Returned()
		{
			if (StoreReady)
			{
				AuctionController.TryDeliverPendingFromHook();
				AuctionPhone.OnStoreReady();
			}
		}

		private static void SceneUnloaded(Scene scene)
		{
			if (!(((Scene)(ref scene)).name != "InventoryScene"))
			{
				RunCleanupStep("auction UI", delegate
				{
					AuctionController.Dispose(returnToStore: false);
				});
				RunCleanupStep("identity dialog", AuctionGhostIdentity.Dispose);
				RunCleanupStep("phone", AuctionPhone.Dispose);
				RunCleanupStep("relations", AuctionMerchantRelations.Dispose);
				RunCleanupStep("evidence button", WildeExpressEvidence.Dispose);
				RunCleanupStep("load generation", AuctionLoadLifecycle.Dispose);
			}
		}

		public void OnDisable()
		{
			if (_enabled)
			{
				IsShuttingDown = true;
				_enabled = false;
				ModHook.OnModItemDirectoryInit -= AuctionItemRegistration.Register;
				ModHook.OnModAssetsLoaded -= OfficialAssets.Loaded;
				ModHook.OnLootTablesLoaded -= AuctionCatalog.InvalidatePools;
				ModHook.OnGameLoadedInit -= BeforeLoad;
				ModHook.OnGameLoadedLate -= AfterLoad;
				ModHook.OnNewGameStarted -= NewGame;
				ModHook.OnGenerateCustomerLate -= GeneratedCustomers;
				ModHook.OnWakingUpLate -= AfterWake;
				ModHook.OnReturningStoreLate -= Returned;
				SceneManager.sceneUnloaded -= SceneUnloaded;
				RunCleanupStep("auction UI", delegate
				{
					AuctionController.Dispose(returnToStore: false);
				});
				RunCleanupStep("identity dialog", AuctionGhostIdentity.Dispose);
				RunCleanupStep("phone", AuctionPhone.Dispose);
				RunCleanupStep("relations", AuctionMerchantRelations.Dispose);
				RunCleanupStep("load generation", AuctionLoadLifecycle.Dispose);
				RunCleanupStep("evidence service", WildeExpressEvidence.Unregister);
				RunCleanupStep("UI style", AuctionUiStyle.Dispose);
				RunCleanupStep("assets", OfficialAssets.Clear);
				RunCleanupStep("night market events", AuctionNightMarket.Unregister);
				RunCleanupStep("location", delegate
				{
					ModHelper.RemoveAfterhourLocations("xiaowo_night_auction");
				});
				Location = null;
				if (Object.op_Implicit((Object)(object)_driver))
				{
					Object.Destroy((Object)(object)_driver);
				}
				_driver = null;
				Log?.Msg("Official Mono disabled; callbacks detached; no inventory created during teardown.");
			}
		}

		internal static bool RunCleanupStep(string name, Action action)
		{
			try
			{
				action();
				return true;
			}
			catch (Exception ex)
			{
				Log?.Warning("Cleanup [" + name + "]: " + ex);
				return false;
			}
		}
	}
	internal sealed class AuctionUpdateDriver : MonoBehaviour
	{
		private string _locale;

		private int _nextLocaleCheck;

		private void Update()
		{
			if (Core.IsShuttingDown)
			{
				return;
			}
			try
			{
				AuctionLoadLifecycle.Tick();
				if (!Core.StoreReady)
				{
					return;
				}
				AuctionController.Tick();
				WildeExpressEvidence.Tick();
				AuctionPhone.Tick();
				AuctionMerchantRelations.Tick();
				if (Time.frameCount >= _nextLocaleCheck)
				{
					_nextLocaleCheck = Time.frameCount + 30;
					string currentLocaleCode = AuctionText.CurrentLocaleCode;
					if (!(_locale == currentLocaleCode))
					{
						AuctionController.RefreshLocalizedText();
						AuctionExclusiveFactory.RefreshOwnedText();
						AuctionContainer.RefreshOwnedText();
						AuctionPhone.RefreshOwnedText();
						AuctionMerchants.RefreshLocalizedDialogue(PlayerStore.Instance.storeClientManager);
						_locale = currentLocaleCode;
					}
				}
			}
			catch (Exception ex)
			{
				Core.Log?.Error("Official update failed: " + ex);
				((Behaviour)this).enabled = false;
			}
		}
	}
	[HarmonyPatch(typeof(ItemMouseDoubleClickHandler), "DoubleClickAction")]
	internal static class AuctionContainerDoubleClickPatch
	{
		private static bool Prefix(GameItem newItem, Vector2 mousePosition)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (((newItem != null) ? newItem.identifier : null) != "xiaowo_auction_container")
			{
				return true;
			}
			if (!Core.StoreReady)
			{
				return false;
			}
			AuctionContainer.RepairForOpenIfNeeded(newItem);
			AuctionContainer.OpenTransientWindow(newItem, mousePosition);
			return false;
		}
	}
	[HarmonyPatch(typeof(ItemMouseDoubleClickHandler), "OpenContentAction")]
	internal static class AuctionContainerOpenPatch
	{
		private static bool Prefix(GameItem newItem, Vector2 mousePosition)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return AuctionContainerDoubleClickPatchProxy.Open(newItem, mousePosition);
		}
	}
	internal static class AuctionContainerDoubleClickPatchProxy
	{
		internal static bool Open(GameItem item, Vector2 position)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (((item != null) ? item.identifier : null) != "xiaowo_auction_container")
			{
				return true;
			}
			if (Core.StoreReady)
			{
				AuctionContainer.RepairForOpenIfNeeded(item);
				AuctionContainer.OpenTransientWindow(item, position);
			}
			return false;
		}
	}
	[HarmonyPatch(typeof(GameGridInventory), "MayHaveValidInventorySlot")]
	internal static class AuctionContainerIntakePatch
	{
		private static bool Prefix(GameGridInventory __instance, ref bool __result)
		{
			if (((GameInventory)(__instance?)).identifier != "xiaowo_auction_locked_intake" || AuctionContainer.InternalFillActive)
			{
				return true;
			}
			__result = false;
			return false;
		}
	}
	[HarmonyPatch(typeof(GameGridInventory), "Expel")]
	internal static class AuctionContainerOutputPatch
	{
		private static void Postfix(GameGridInventory __instance, GameItem item, bool __result)
		{
			if (__result && !(((GameInventory)(__instance?)).identifier != "xiaowo_auction_locked_intake") && !AuctionContainer.InternalFillActive)
			{
				AuctionContainer.ClearOutputSeal(item);
				AuctionContainer.CommitClaim(__instance, item);
			}
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "SaveGame")]
	internal static class AuctionContainerSavePatch
	{
		private static void Prefix(PlayerStore __instance)
		{
			AuctionContainer.CaptureLoadedOutputs(__instance);
		}
	}
	[HarmonyPatch(typeof(WildNetworkUI), "ShowInfo")]
	internal static class AuctionWildeSelectionPatch
	{
		private static void Postfix(string id)
		{
			WildeExpressEvidence.Selected(id);
		}
	}
	[HarmonyPatch(typeof(WildNetworkUI), "SelectTab")]
	internal static class AuctionWildeTabPatch
	{
		private static void Prefix()
		{
			WildeExpressEvidence.PageChanged();
		}
	}
	[HarmonyPatch(typeof(WildNetworkUI), "OnWindowClosed")]
	internal static class AuctionWildeClosePatch
	{
		private static void Prefix()
		{
			WildeExpressEvidence.Dispose();
		}
	}
	[HarmonyPatch(typeof(WildNetworkUI), "OnUseClicked")]
	internal static class AuctionWildeExpressPurchasePatch
	{
		private static bool Prefix()
		{
			return WildeExpressEvidence.BeforeNativePurchase();
		}
	}
	[HarmonyPatch(typeof(NetworkUpgrade), "GetCost")]
	internal static class AuctionWildeExpressCostPatch
	{
		private static void Postfix(NetworkUpgrade __instance, ref int __result)
		{
			if (__instance?.id == "xiaowo_auction_express_evidence")
			{
				__result = WildeExpressEvidence.CurrentPrice();
			}
		}
	}
	[HarmonyPatch(typeof(NetworkUpgrade), "IsReady")]
	internal static class AuctionWildeExpressReadyPatch
	{
		private static void Postfix(NetworkUpgrade __instance, ref bool __result)
		{
			if (__instance?.id == "xiaowo_auction_express_evidence")
			{
				__result = WildeExpressEvidence.CanPurchase();
			}
		}
	}
	internal static class OfficialAssets
	{
		private static LoadedMod _mod;

		private static readonly Dictionary<string, Sprite> Originals = new Dictionary<string, Sprite>();

		private static readonly List<Sprite> Owned = new List<Sprite>();

		internal static void Loaded(LoadedMod mod)
		{
			if (mod.Manifest.ID != "xiaowo_night_auction")
			{
				return;
			}
			Clear();
			_mod = mod;
			foreach (ContainerCategory value in Enum.GetValues(typeof(ContainerCategory)))
			{
				Fit(AuctionCatalog.SpriteName(value), 2);
			}
			foreach (AuctionExclusiveSpec value2 in AuctionExclusiveCatalog.Items.Values)
			{
				Fit(value2.Id, value2.Width);
			}
			Fit("auction_card_closed", 2);
			Core.Log?.Msg($"[Assets] Native sprites fitted: {Owned.Count}; expected 73. Texture ownership remains with the loader.");
		}

		private static void Fit(string key, int width)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (!_mod.Sprites.TryGetValue(key, out var value) || (Object)(object)value == (Object)null)
			{
				throw new InvalidOperationException("Missing official item texture: " + key);
			}
			Originals[key] = value;
			Texture2D texture = value.texture;
			Rect rect = value.rect;
			Vector2 val = new Vector2(0.5f, 0.5f);
			Rect rect2 = value.rect;
			Sprite val2 = Sprite.Create(texture, rect, val, ((Rect)(ref rect2)).width / ((float)width * 0.16f), 0u, (SpriteMeshType)0);
			((Object)val2).name = key;
			Owned.Add(val2);
			_mod.Sprites[key] = val2;
		}

		internal static Sprite Get(string key)
		{
			CustomUIManager instance = CustomUIManager.Instance;
			if (instance == null)
			{
				return null;
			}
			return instance.GetModSprite("xiaowo_night_auction", key, true);
		}

		internal static void SetSprite(GameItem item, string key)
		{
			item.SetSprite("xiaowo_night_auction", key);
			item.useSpriteFromMod = true;
			ModHelper.SetSource(item, "xiaowo_night_auction", "小窝竞拍之王");
		}

		internal static void Clear()
		{
			if (_mod != null)
			{
				foreach (KeyValuePair<string, Sprite> original in Originals)
				{
					_mod.Sprites[original.Key] = original.Value;
				}
			}
			foreach (Sprite item in Owned)
			{
				if (Object.op_Implicit((Object)(object)item))
				{
					Object.Destroy((Object)(object)item);
				}
			}
			Owned.Clear();
			Originals.Clear();
			_mod = null;
		}
	}
	internal static class AuctionContainerSprites
	{
		internal static Sprite Get(ContainerCategory category)
		{
			return OfficialAssets.Get(AuctionCatalog.SpriteName(category));
		}
	}
	internal static class ObjectIdentity
	{
		private sealed class Identity
		{
			internal readonly IntPtr Key = new IntPtr(Interlocked.Increment(ref _next));
		}

		private static int _next;

		private static readonly ConditionalWeakTable<object, Identity> Keys = new ConditionalWeakTable<object, Identity>();

		internal static IntPtr Key(object value)
		{
			if (value != null)
			{
				Object val = (Object)((value is Object) ? value : null);
				if (val == null || Object.op_Implicit(val))
				{
					return Keys.GetValue(value, (object _) => new Identity()).Key;
				}
			}
			return IntPtr.Zero;
		}
	}
	internal static class MonoCallbacks
	{
		internal static T Bind<T>(Delegate action) where T : Delegate
		{
			return (T)action;
		}
	}
	internal static class NativeStore
	{
		internal static PlayerStore Existing
		{
			get
			{
				if (!PlayerStore.HasInstance)
				{
					return null;
				}
				return PlayerStore.Instance;
			}
		}
	}
	internal static class NativePlacement
	{
		internal static bool Restore(GameItem item, GameInventory inventory, GridShape shape)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (item != null && inventory != null && shape != null)
			{
				return new SlotMarker
				{
					item = item,
					inventory = inventory,
					itemGridShape = shape,
					numTransfer = item.unitCount
				}.TryAcceptOnce(-1) >= item.unitCount;
			}
			return false;
		}
	}
	internal sealed class AuctionLogger
	{
		private readonly ModLog _log;

		internal AuctionLogger(ModManifest manifest)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected O, but got Unknown
			_log = new ModLog(manifest);
		}

		internal void Msg(string message)
		{
			_log.Log((object)message);
		}

		internal void Warning(string message)
		{
			_log.Warning((object)message);
		}

		internal void Error(string message)
		{
			_log.Error((object)message);
		}
	}
	internal static class WildeExpressEvidence
	{
		internal const string UpgradeId = "EVIDENCE_REMOVAL";

		internal const string ServiceId = "xiaowo_auction_express_evidence";

		internal const string IconKey = "auction_express_cleanup";

		private static readonly FieldInfo FrameField = AccessTools.Field(typeof(WildNetworkUI), "frame");

		private static readonly FieldInfo SelectionField = AccessTools.Field(typeof(WildNetworkUI), "selectedId");

		private static readonly FieldInfo TabsField = AccessTools.Field(typeof(WildNetworkUI), "tabs");

		private static readonly FieldInfo CurrentTabField = AccessTools.Field(typeof(WildNetworkUI), "currentTab");

		private static bool _registered;

		private static WildNetworkFrame _frame;

		private static string _selection;

		private static int _nextRefresh;

		internal static void RegisterForLoadedStore()
		{
			if (!Core.StoreReady)
			{
				return;
			}
			if (!_registered)
			{
				ModHelper.AddNetworkUpgrade("xiaowo_night_auction", "xiaowo_auction_express_evidence", 0, "Informant", (Func<string>)(() => AuctionText.Select("加急清证", "Express evidence cleanup")), (Func<string>)Description, (Action)null, (string)null, "xiaowo_night_auction:auction_express_cleanup", (string[])null, 0, (Func<string>)null, (Func<string>)null, false);
				_registered = true;
			}
			Dictionary<string, NetworkUpgrade> networkUpgrade = PlayerStore.Instance.networkUpgrade;
			if (networkUpgrade != null && !networkUpgrade.ContainsKey("xiaowo_auction_express_evidence"))
			{
				networkUpgrade.Add("xiaowo_auction_express_evidence", NetworkUpgrade.GetDefinition("xiaowo_auction_express_evidence").CopyForRuntime());
			}
		}

		internal static void Unregister()
		{
			Dispose();
			if (_registered)
			{
				ModHelper.RemoveNetworkUpgrades("xiaowo_night_auction");
			}
			_registered = false;
		}

		private static string Description()
		{
			return AuctionText.Select("在原版清除证据服务的冷却期间，用现金加急销毁已收集的证据并终止当前调查。原版服务可以使用时，本服务不可购买。\n\n费用为原版当前价格，加上按剩余冷却天数递减的加急费，最高加收3000信用点。只扣现金，不消耗王尔德的恩惠。\n\n每天最多加急一次；使用后同样重置原版14天冷却，并保留原版费用增长规则。", "Pay cash to destroy collected evidence and stop the current investigation while standard cleanup is cooling down. Unavailable when standard cleanup is ready.\n\nPay the current standard cost plus a surcharge of up to 3000 credits, falling with the remaining cooldown. Cash only; no Wilde favor is spent.\n\nOnce per day. Resets the standard 14-day cooldown and preserves its normal cost increase.");
		}

		internal static int Quote(int nativeCost, int remaining, int duration)
		{
			return ExpressEvidencePricing.Quote(nativeCost, remaining, duration);
		}

		internal static int CurrentPrice()
		{
			if (!Core.StoreReady)
			{
				return 0;
			}
			NetworkUpgrade upgrade = GetUpgrade(PlayerStore.Instance);
			if (upgrade != null)
			{
				return Quote(upgrade.GetCost(), upgrade.cooldownCurrent, upgrade.cooldownDuration);
			}
			return 0;
		}

		internal static bool CanPurchase()
		{
			if (!Core.StoreReady)
			{
				return false;
			}
			PlayerStore instance = PlayerStore.Instance;
			NetworkUpgrade upgrade = GetUpgrade(instance);
			if (upgrade != null)
			{
				return ExpressEvidencePricing.Available(upgrade.state, upgrade.cooldownCurrent, upgrade.IsHavePrerequisite(), demoLocked: false, UsedToday(), instance.GetCash(), CurrentPrice());
			}
			return false;
		}

		internal static void Tick()
		{
			if (!Core.StoreReady || !WildNetworkUI.IsOpen)
			{
				Dispose();
			}
			else if (!(_selection != "xiaowo_auction_express_evidence") && Time.frameCount >= _nextRefresh)
			{
				_nextRefresh = Time.frameCount + 10;
				RefreshButton();
			}
		}

		internal static void Selected(string id)
		{
			_selection = id;
			_frame = (WildNetworkFrame)((id == "xiaowo_auction_express_evidence" && Core.StoreReady && WildNetworkUI.IsOpen) ? /*isinst with value type is only supported in some contexts*/: null);
			RefreshButton();
		}

		internal static void PageChanged()
		{
			Dispose();
		}

		private static bool IsServiceTab()
		{
			List<Tab> list = TabsField?.GetValue(null) as List<Tab>;
			int num = (CurrentTabField?.GetValue(null) as int?) ?? (-1);
			if (list == null || num < 0 || num >= list.Count)
			{
				return false;
			}
			foreach (Card card in list[num].cards)
			{
				foreach (Cell cell in card.cells)
				{
					if (cell.id == "xiaowo_auction_express_evidence")
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void RefreshButton()
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			if (Core.StoreReady && WildNetworkUI.IsOpen && !(_selection != "xiaowo_auction_express_evidence") && Object.op_Implicit((Object)(object)_frame))
			{
				bool flag = CanPurchase();
				((Selectable)_frame.useButton).interactable = flag;
				((TMP_Text)_frame.useLabel).text = AuctionText.Select("加急清证", "Express cleanup");
				((TMP_Text)_frame.usePrice).text = CurrentPrice() + "¢";
				((Graphic)_frame.useLabel).color = (flag ? WildNetworkStyle.BUTTON_TEXT : WildNetworkStyle.WithAlpha(WildNetworkStyle.BUTTON_TEXT, 0.3f));
				((Graphic)_frame.usePrice).color = (flag ? WildNetworkStyle.BUTTON_TEXT : WildNetworkStyle.BUTTON_TEXT_UNAVAILABLE);
				if (Object.op_Implicit((Object)(object)_frame.useInnerBorder))
				{
					((Graphic)_frame.useInnerBorder).color = (flag ? WildNetworkStyle.BUTTON_INNER : WildNetworkStyle.BUTTON_INNER_UNAVAILABLE);
				}
				NetworkUpgrade upgrade = GetUpgrade(PlayerStore.Instance);
				string text = (UsedToday() ? AuctionText.Select("今日已加急，请明天再来。", "Already used today. Come back tomorrow.") : ((upgrade == null || upgrade.cooldownCurrent <= 0) ? AuctionText.Select("请先使用原版清除证据服务。", "Use standard evidence cleanup first.") : ((!flag) ? AuctionText.Select("现金不足或原版服务尚未解锁。", "Not enough cash, or standard cleanup is unavailable.") : "")));
				((TMP_Text)_frame.infoMessage).text = text;
				((Graphic)_frame.infoMessage).color = WildNetworkStyle.INFO_WARNING;
				((Component)_frame.infoMessage).gameObject.SetActive(text.Length > 0);
				((TMP_Text)_frame.infoCooldown).text = ((upgrade != null && upgrade.cooldownCurrent > 0) ? (AuctionText.Select("原版冷却剩余：", "Standard cooldown remaining: ") + upgrade.cooldownCurrent + AuctionText.Select("天", " days")) : "");
				((Component)_frame.infoCooldown).gameObject.SetActive(((TMP_Text)_frame.infoCooldown).text.Length > 0);
			}
		}

		internal static bool BeforeNativePurchase()
		{
			if (SelectionField?.GetValue(null) as string != "xiaowo_auction_express_evidence")
			{
				return true;
			}
			try
			{
				Purchase();
			}
			catch (Exception ex)
			{
				Core.Log?.Error("Express evidence removal failed: " + ex);
			}
			return false;
		}

		internal static bool Purchase()
		{
			if (!Core.StoreReady || !WildNetworkUI.IsOpen || _selection != "xiaowo_auction_express_evidence" || !IsServiceTab() || SelectionField?.GetValue(null) as string != "xiaowo_auction_express_evidence" || !Object.op_Implicit((Object)(object)_frame) || !((Component)_frame.useButton).gameObject.activeInHierarchy || !CanPurchase())
			{
				return false;
			}
			PlayerStore instance = PlayerStore.Instance;
			if (instance.secData == null)
			{
				return false;
			}
			NetworkUpgrade upgrade = GetUpgrade(instance);
			int num = CurrentPrice();
			upgrade.Unlock();
			instance.ModCash(-num, true);
			AuctionState.Write("express_evidence_day", AuctionCatalog.Day.ToString());
			RunStats.OnWildUpgradeBought(num);
			if (Object.op_Implicit((Object)(object)EmporiumAudioPlayer.Instance))
			{
				EmporiumAudioPlayer.Instance.PlayRegister();
			}
			WildNetworkUI.RefreshAll();
			Core.Log?.Msg("Express evidence removal purchased for " + num + " credits.");
			return true;
		}

		private static NetworkUpgrade GetUpgrade(PlayerStore store)
		{
			if (store.networkUpgrade == null || !store.networkUpgrade.TryGetValue("EVIDENCE_REMOVAL", out var value))
			{
				return null;
			}
			return value;
		}

		private static bool UsedToday()
		{
			if (int.TryParse(AuctionState.Read("express_evidence_day"), out var result))
			{
				return result == AuctionCatalog.Day;
			}
			return false;
		}

		internal static void Dispose()
		{
			_frame = null;
			_selection = null;
			_nextRefresh = 0;
		}
	}
	internal enum ContainerCategory
	{
		Medical,
		Luxury,
		Contraband,
		Engineering
	}
	internal sealed class AuctionLot
	{
		internal int Slot;

		internal ContainerCategory Category;

		internal AuctionVenue Venue;

		internal AuctionQuality Quality;

		internal int Seed;

		internal int ItemCount;

		internal int ReferenceValue;

		internal int TargetValue;

		internal int EstimatedValue;

		internal int CurrentBid;

		internal int OpeningBid;

		internal int PlayerOffer;

		internal int PendingOffer = -1;

		internal int BiddersShown;

		internal int Round;

		internal bool PlayerLeading;

		internal bool Finished;

		internal bool Lost;

		internal bool Withdrawn;

		internal bool Started;

		internal bool Sealed;

		internal int HighestBidder = -1;

		internal int ActiveMask = 7;

		internal int IntelMask;

		internal int IntelSpent;

		internal int[] Budgets = new int[3];

		internal int[] LastOffers = new int[3];

		internal int[] MemoryModifiers = new int[3];

		internal int[] RivalExitReasons = new int[3];

		internal int[] RivalIntelTiers = new int[3];

		internal int[] RivalIntelValues = new int[3];

		internal int[] RivalIntelSpent = new int[3];

		internal int[] RivalIntelRounds = new int[3];

		internal bool RivalMemoryApplied;

		internal bool RivalMemoryRecorded;

		internal List<string> BidLog = new List<string>();

		internal List<string> Contents = new List<string>();

		internal List<string> InspectionItems = new List<string>();
	}
	internal static class AuctionCatalog
	{
		internal const string ContainerId = "xiaowo_auction_container";

		internal const string RestrictedNeuralId = "system_capped_neural_core";

		internal const string ContainerAtlas = "XIAOWO/NightAuctionContainers";

		internal const string InternalOnlyTag = "XIAOWO_AUCTION_INTERNAL_ONLY";

		internal const string DataTag = "XIAOWO_AUCTION_CONTAINER_DATA";

		private static readonly HashSet<string> ExcludedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"save_bag", "back_inv_save_bag", "hidden_save_bag", "showcase_save_bag", "sold_save_bag", "sec_box", "evidence_box", "med_box", "eng_box", "service_box",
			"system_capped_neural_core"
		};

		private static readonly Dictionary<ContainerCategory, List<string>> Pools = new Dictionary<ContainerCategory, List<string>>();

		private static readonly Dictionary<string, long> ValueCache = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

		private static readonly Dictionary<(ContainerCategory, AuctionVenue), List<string>> BandCache = new Dictionary<(ContainerCategory, AuctionVenue), List<string>>();

		private static readonly HashSet<string> ReleasedNativeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<string> NativeModuleIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		internal static int Day => Math.Max(0, StoreStation.GetDayCounter());

		internal static bool AvailabilityReady
		{
			get
			{
				EnsureReleasedNativeIds();
				return ReleasedNativeIds.Count > 0;
			}
		}

		internal static string CategoryName(ContainerCategory category)
		{
			return category switch
			{
				ContainerCategory.Medical => AuctionText.Get("category_medical", "医疗集装箱"), 
				ContainerCategory.Luxury => AuctionText.Get("category_luxury", "奢侈品集装箱"), 
				ContainerCategory.Contraband => AuctionText.Get("category_contraband", "违禁品集装箱"), 
				_ => AuctionText.Get("category_engineering", "工程集装箱"), 
			};
		}

		internal static Color CategoryColor(ContainerCategory category)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			return (Color)(category switch
			{
				ContainerCategory.Medical => new Color(0.4f, 0.72f, 0.64f, 1f), 
				ContainerCategory.Luxury => new Color(0.78f, 0.62f, 0.25f, 1f), 
				ContainerCategory.Contraband => new Color(0.68f, 0.26f, 0.28f, 1f), 
				_ => new Color(0.3f, 0.55f, 0.72f, 1f), 
			});
		}

		internal static string SpriteName(ContainerCategory category)
		{
			return category switch
			{
				ContainerCategory.Medical => "xiaowo_auction_container_medical", 
				ContainerCategory.Luxury => "xiaowo_auction_container_luxury", 
				ContainerCategory.Contraband => "xiaowo_auction_container_contraband", 
				_ => "xiaowo_auction_container_engineering", 
			};
		}

		internal static string NativeSpriteName(ContainerCategory category)
		{
			return category switch
			{
				ContainerCategory.Medical => "med_box", 
				ContainerCategory.Luxury => "service_box", 
				ContainerCategory.Contraband => "sec_box", 
				_ => "eng_box", 
			};
		}

		internal static bool TryParseContainerSprite(string spriteName, out ContainerCategory category)
		{
			category = ContainerCategory.Engineering;
			switch (spriteName)
			{
			case "xiaowo_auction_container_medical":
				category = ContainerCategory.Medical;
				break;
			case "xiaowo_auction_container_luxury":
				category = ContainerCategory.Luxury;
				break;
			case "xiaowo_auction_container_contraband":
				category = ContainerCategory.Contraband;
				break;
			default:
				if (spriteName != "xiaowo_auction_container_engineering")
				{
					return false;
				}
				break;
			}
			return true;
		}

		internal static List<string> BuildPool(ContainerCategory category)
		{
			EnsurePools();
			if (!Pools.TryGetValue(category, out var value))
			{
				return new List<string>();
			}
			return new List<string>(value);
		}

		internal static void InvalidatePools()
		{
			Pools.Clear();
			ValueCache.Clear();
			BandCache.Clear();
			ReleasedNativeIds.Clear();
			NativeModuleIds.Clear();
		}

		internal static void WarmPools()
		{
			EnsurePools();
		}

		private static void EnsurePools()
		{
			if (Pools.Count == 4)
			{
				return;
			}
			Pools.Clear();
			ValueCache.Clear();
			BandCache.Clear();
			NativeModuleIds.Clear();
			foreach (ContainerCategory value in Enum.GetValues(typeof(ContainerCategory)))
			{
				Pools[value] = new List<string>();
			}
			EnsureReleasedNativeIds();
			if (ReleasedNativeIds.Count == 0)
			{
				Pools.Clear();
				Core.Log?.Warning("Native loot registry is not ready; auction item pools will be retried later.");
				return;
			}
			List<string> identifierList = DirectoryMaster.GetIdentifierList<GameItem>((string)null);
			if (identifierList == null)
			{
				return;
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			try
			{
				List<string> identifierList2 = DirectoryMaster.GetIdentifierList<GameItem>("ModItemDirectory");
				if (identifierList2 != null)
				{
					foreach (string item in identifierList2)
					{
						hashSet.Add(item);
					}
				}
			}
			catch
			{
			}
			foreach (string item2 in identifierList)
			{
				if (string.IsNullOrWhiteSpace(item2) || item2 == "xiaowo_auction_container" || ExcludedIds.Contains(item2) || AuctionExclusiveCatalog.IsExclusive(item2) || hashSet.Contains(item2) || !ReleasedNativeIds.Contains(item2))
				{
					continue;
				}
				try
				{
					GameItem val = DirectoryMaster.Item(item2, false);
					if (!IsEligible(val))
					{
						continue;
					}
					if (val.IsTag("MODULE"))
					{
						NativeModuleIds.Add(item2);
					}
					foreach (ContainerCategory value2 in Enum.GetValues(typeof(ContainerCategory)))
					{
						if (Matches(value2, val))
						{
							Pools[value2].Add(item2);
						}
					}
				}
				catch
				{
				}
			}
			Core.Log?.Msg("Auction pools cached: " + string.Join(", ", new ContainerCategory[4]
			{
				ContainerCategory.Medical,
				ContainerCategory.Luxury,
				ContainerCategory.Contraband,
				ContainerCategory.Engineering
			}.Select((ContainerCategory category) => category.ToString() + "=" + Pools[category].Count)));
		}

		private static void EnsureReleasedNativeIds()
		{
			if (ReleasedNativeIds.Count > 0)
			{
				return;
			}
			foreach (string tableId in LootRegistry.TableIds)
			{
				List<LootEntry> tableEntries = LootRegistry.GetTableEntries(tableId);
				if (tableEntries == null)
				{
					continue;
				}
				for (int i = 0; i < tableEntries.Count; i++)
				{
					string text = tableEntries[i]?.id;
					if (!string.IsNullOrWhiteSpace(text))
					{
						ReleasedNativeIds.Add(text);
					}
				}
			}
			Core.Log?.Msg("Retail loot whitelist cached: " + ReleasedNativeIds.Count + " item(s); other MOD directory IDs excluded.");
		}

		internal static List<string> GenerateContents(AuctionLot lot)
		{
			if (lot == null)
			{
				return new List<string>();
			}
			if (lot.Contents.Count > 0)
			{
				return new List<string>(lot.Contents);
			}
			List<string> list = null;
			long num = long.MaxValue;
			(int, int) tuple = AuctionRules.ItemCountRange(lot.Venue);
			int num2 = Math.Clamp(lot.ItemCount, tuple.Item1, tuple.Item2);
			int num3 = AuctionNightMarket.MainPercent(lot.Category);
			bool flag = AuctionRules.ShouldIncludeRestrictedNeural(lot.Venue, lot.Category, lot.Seed) && IsAvailableItem("system_capped_neural_core");
			int num4 = AuctionRules.RollExclusiveTier(lot.Venue, lot.Quality, lot.Seed);
			int num5 = 0;
			if (lot.Quality == AuctionQuality.Jackpot && num4 >= 2)
			{
				int num6 = AuctionExclusiveCatalog.Items.Values.Where((AuctionExclusiveSpec spec) => spec.Tier == 3).Min((AuctionExclusiveSpec spec) => spec.Value);
				int num7 = AuctionExclusiveCatalog.Items.Values.Where((AuctionExclusiveSpec spec) => spec.Tier == 2).Min((AuctionExclusiveSpec spec) => spec.Value);
				if (num4 == 3 && lot.TargetValue >= (long)num6 * 13L / 10)
				{
					num5 = 3;
				}
				else if (lot.TargetValue >= (long)num7 * 13L / 10)
				{
					num5 = 2;
				}
			}
			for (int num8 = 0; num8 < 128; num8++)
			{
				Random random = new Random(lot.Seed ^ ((num8 + 1) * 104729));
				int num9 = ((num8 == 0) ? num2 : random.Next(tuple.Item1, tuple.Item2 + 1));
				int num10 = (flag ? random.Next(num9) : (-1));
				int num11 = AuctionRules.ExclusiveSlots(lot.Quality, num9, random);
				HashSet<int> hashSet = new HashSet<int>();
				while (hashSet.Count < num11)
				{
					hashSet.Add(random.Next(num9));
				}
				int num12 = ((num5 > 0 && hashSet.Count > 0) ? hashSet.Min() : (-1));
				if (num12 >= 0 && num10 == num12)
				{
					num10 = (num12 + 1) % num9;
				}
				List<string> list2 = new List<string>();
				Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
				long num13 = 0L;
				for (int num14 = 0; num14 < 32 && (num14 < num9 || num13 < (long)lot.TargetValue * 94L / 100); num14++)
				{
					ContainerCategory category = ((random.Next(100) < num3) ? lot.Category : OtherCategory(lot.Category, random));
					int val = Math.Max(1, num9 - num14);
					long desired = Math.Max(1L, (lot.TargetValue - num13) / Math.Max(1, val));
					string text = ((num14 == num10) ? "system_capped_neural_core" : (hashSet.Contains(num14) ? ChooseExclusive(category, num4, (num14 == num12) ? num5 : 0, desired, dictionary, random) : ChooseRegular(category, lot.Venue, lot.Quality, num4, desired, dictionary, random)));
					if (text == null)
					{
						text = ChooseRegular(lot.Category, lot.Venue, lot.Quality, num4, desired, dictionary, random);
					}
					if (text == null)
					{
						break;
					}
					list2.Add(text);
					dictionary[text] = ((!dictionary.TryGetValue(text, out var value)) ? 1 : (value + 1));
					num13 += ItemValue(text);
				}
				if (list2.Count < tuple.Item1)
				{
					continue;
				}
				long num15 = Math.Abs(num13 - lot.TargetValue);
				if (num15 < num)
				{
					list = list2;
					num = num15;
					if (num15 <= Math.Max(50L, (long)lot.TargetValue / 25L))
					{
						break;
					}
				}
			}
			return list ?? new List<string>();
		}

		internal static int ContentsValue(IReadOnlyList<string> contents)
		{
			long num = 0L;
			if (contents != null)
			{
				foreach (string content in contents)
				{
					num += ItemValue(content);
				}
			}
			return (int)Math.Min(2147483647L, Math.Max(0L, num));
		}

		internal static long ItemValue(string id)
		{
			if (AuctionExclusiveCatalog.Items.TryGetValue(id ?? string.Empty, out var value))
			{
				return value.Value;
			}
			if (id != null && ValueCache.TryGetValue(id, out var value2))
			{
				return value2;
			}
			long num;
			try
			{
				long val = 1L;
				GameItem obj = DirectoryMaster.Item(id, false);
				num = Math.Max(val, (obj != null) ? obj.GetCurrentValue(false, false, false, false, 0L) : 0);
			}
			catch
			{
				num = 1L;
			}
			if (id != null)
			{
				ValueCache[id] = num;
			}
			return num;
		}

		private static string ChooseExclusive(ContainerCategory category, int maxTier, int requiredTier, long desired, Dictionary<string, int> usage, Random random)
		{
			List<AuctionExclusiveSpec> source = AuctionExclusiveCatalog.Candidates(category, maxTier);
			if (requiredTier > 0)
			{
				source = source.Where((AuctionExclusiveSpec spec) => spec.Tier == requiredTier).ToList();
			}
			return ChooseClosest(source.Select((AuctionExclusiveSpec spec) => spec.Id), desired, usage, random, 4);
		}

		private static string ChooseRegular(ContainerCategory category, AuctionVenue venue, AuctionQuality quality, int exclusiveTier, long desired, Dictionary<string, int> usage, Random random)
		{
			List<string> list = new List<string>(ValueBand(category, venue));
			list.AddRange(from spec in AuctionExclusiveCatalog.SupplementalCandidates(category)
				select spec.Id);
			if (quality != AuctionQuality.Loss && quality != AuctionQuality.Hidden && (venue != AuctionVenue.Beginner || quality >= AuctionQuality.Profit))
			{
				int maxTier = ((quality == AuctionQuality.Random) ? 1 : exclusiveTier);
				list.AddRange(from spec in AuctionExclusiveCatalog.Candidates(category, maxTier)
					select spec.Id);
			}
			return ChooseClosest(list, desired, usage, random, 3);
		}

		private static string ChooseClosest(IEnumerable<string> source, long desired, Dictionary<string, int> usage, Random random, int duplicateLimit)
		{
			List<string> list = source.Distinct<string>(StringComparer.OrdinalIgnoreCase).ToList();
			string best = null;
			long num = long.MaxValue;
			foreach (string item in list)
			{
				if (!usage.TryGetValue(item, out var value) || value < duplicateLimit)
				{
					long num2 = ItemValue(item);
					int num3 = random.Next(0, (int)Math.Min(2147483647L, Math.Max(2L, desired / 5)));
					long num4 = Math.Abs(num2 - desired) + num3;
					if (num4 < num)
					{
						best = item;
						num = num4;
					}
				}
			}
			if (best != null && NativeModuleIds.Contains(best))
			{
				int value2;
				List<string> list2 = list.Where((string id) => NativeModuleIds.Contains(id) && ItemValue(id) == ItemValue(best) && (!usage.TryGetValue(id, out value2) || value2 < duplicateLimit)).ToList();
				int leastUsed = list2.Min((string id) => usage.TryGetValue(id, out value2) ? value2 : 0);
				list2.RemoveAll((string id) => (usage.TryGetValue(id, out value2) ? value2 : 0) != leastUsed);
				return list2[random.Next(list2.Count)];
			}
			return best;
		}

		private static List<string> ValueBand(ContainerCategory category, AuctionVenue venue)
		{
			if (BandCache.TryGetValue((category, venue), out var value))
			{
				return value;
			}
			List<string> list = BuildPool(category).OrderBy(ItemValue).ToList();
			if (list.Count <= 2)
			{
				BandCache[(category, venue)] = list;
				return list;
			}
			double num = venue switch
			{
				AuctionVenue.Intermediate => 0.25, 
				AuctionVenue.Beginner => 0.0, 
				_ => 0.55, 
			};
			double num2 = venue switch
			{
				AuctionVenue.Intermediate => 0.85, 
				AuctionVenue.Beginner => 0.55, 
				_ => 1.0, 
			};
			int num3 = Math.Clamp((int)Math.Floor((double)list.Count * num), 0, list.Count - 1);
			int num4 = Math.Clamp((int)Math.Ceiling((double)list.Count * num2), num3 + 1, list.Count);
			List<string> range = list.GetRange(num3, num4 - num3);
			long lowValue = ItemValue(range[0]);
			List<string> list2 = range;
			long highValue = ItemValue(list2[list2.Count - 1]);
			range = list.Where((string id) => ItemValue(id) >= lowValue && ItemValue(id) <= highValue).ToList();
			BandCache[(category, venue)] = range;
			return range;
		}

		private static ContainerCategory OtherCategory(ContainerCategory primary, Random random)
		{
			List<ContainerCategory> list = new List<ContainerCategory>();
			foreach (ContainerCategory value in Enum.GetValues(typeof(ContainerCategory)))
			{
				if (value != primary)
				{
					list.Add(value);
				}
			}
			return list[random.Next(list.Count)];
		}

		internal static string BuildPoolStatus()
		{
			List<string> list = new List<string>();
			foreach (ContainerCategory value in Enum.GetValues(typeof(ContainerCategory)))
			{
				List<string> list2 = BuildPool(value);
				list.Add(value.ToString() + "=" + list2.Count + ((list2.Count == 0) ? string.Empty : (" sample=" + string.Join("|", list2.GetRange(0, Math.Min(8, list2.Count))))));
			}
			return string.Join("\n", list);
		}

		internal static bool IsAvailableItem(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return false;
			}
			if (AuctionExclusiveCatalog.IsExclusive(id))
			{
				return true;
			}
			if (string.Equals(id, "system_capped_neural_core", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					return IsEligible(DirectoryMaster.Item(id, false));
				}
				catch
				{
					return false;
				}
			}
			EnsureReleasedNativeIds();
			if (!ReleasedNativeIds.Contains(id))
			{
				return false;
			}
			try
			{
				return IsEligible(DirectoryMaster.Item(id, false));
			}
			catch
			{
				return false;
			}
		}

		internal static bool AreContentsAvailable(IEnumerable<string> contents)
		{
			if (AvailabilityReady)
			{
				return contents?.All(IsAvailableItem) ?? false;
			}
			return true;
		}

		internal static bool RepairRemainingContents(ContainerCategory category, int seed, int claimed, List<string> contents)
		{
			if (!AvailabilityReady || contents == null || contents.Count == 0)
			{
				return false;
			}
			List<string> list = BuildPool(category);
			list.AddRange(from spec in AuctionExclusiveCatalog.Candidates(category, 1)
				select spec.Id);
			list = list.Where(IsAvailableItem).Distinct<string>(StringComparer.OrdinalIgnoreCase).ToList();
			if (list.Count == 0)
			{
				return false;
			}
			bool result = false;
			for (int num = Math.Clamp(claimed, 0, contents.Count); num < contents.Count; num++)
			{
				if (!IsAvailableItem(contents[num]))
				{
					Random random = new Random(seed ^ ((num + 1) * 104729));
					contents[num] = list[random.Next(list.Count)];
					result = true;
				}
			}
			return result;
		}

		private static bool IsEligible(GameItem item)
		{
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			if (item == null || item.unitValue <= 0 || item.shape == null)
			{
				return false;
			}
			string text = item.identifier ?? string.Empty;
			if (text.StartsWith("character_", StringComparison.OrdinalIgnoreCase) || text.Contains("quest", StringComparison.OrdinalIgnoreCase) || text.Contains("story", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			string[] array = new string[5] { "KEY_ITEM", "QUEST_ITEM", "CHARACTER_ITEM", "UNIQUE_ITEM_TAG", "NOT_FOR_SALE_TAG" };
			foreach (string text2 in array)
			{
				if (item.IsTag(text2))
				{
					return false;
				}
			}
			if (string.IsNullOrWhiteSpace(item.spriteAtlasPath) || string.IsNullOrWhiteSpace(item.spritePath))
			{
				return false;
			}
			try
			{
				Sprite val = RenderHandler.LoadFromAtlas(item.spriteAtlasPath, item.spritePath);
				if ((Object)(object)val == (Object)null || (Object)(object)val.texture == (Object)null)
				{
					goto IL_0116;
				}
				Rect rect = val.rect;
				if (((Rect)(ref rect)).width < 2f)
				{
					goto IL_0116;
				}
				rect = val.rect;
				if (((Rect)(ref rect)).height < 2f)
				{
					goto IL_0116;
				}
				Sprite unknownSprite = RenderHandler.unknownSprite;
				if ((Object)(object)unknownSprite != (Object)null && ObjectIdentity.Key(val) == ObjectIdentity.Key(unknownSprite))
				{
					return false;
				}
				goto end_IL_00bc;
				IL_0116:
				return false;
				end_IL_00bc:;
			}
			catch
			{
				return false;
			}
			return true;
		}

		private static bool Matches(ContainerCategory category, GameItem item)
		{
			return category switch
			{
				ContainerCategory.Medical => HasAny(item, "MEDICAL"), 
				ContainerCategory.Luxury => HasAny(item, "LUXURY_ITEM", "ALCOHOL", "WINE_CONTAINER_TAG"), 
				ContainerCategory.Contraband => HasAny(item, "CONTRABAND", "CONTRABAND_ITEM_TAG", "WEAPON", "FIREARM"), 
				ContainerCategory.Engineering => HasAny(item, "MACHINE", "STANDARD_MACHINE_TAG", "MODULE", "MATERIAL", "TOOL"), 
				_ => false, 
			};
		}

		private static bool HasAny(GameItem item, params string[] tags)
		{
			foreach (string text in tags)
			{
				if (item.IsTag(text))
				{
					return true;
				}
			}
			return false;
		}
	}
	internal static class AuctionItemRegistration
	{
		internal static void Register(ModItemDirectory directory)
		{
			if ((Object)(object)directory == (Object)null)
			{
				return;
			}
			int num = 0;
			if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey("xiaowo_auction_container") && ((Directory<GameItem>)(object)directory).Add("xiaowo_auction_container", (Func<GameItem>)(() => AuctionContainer.Create(owned: false, ContainerCategory.Engineering, 1, Array.Empty<string>()))))
			{
				num++;
			}
			foreach (string key in AuctionExclusiveCatalog.Items.Keys)
			{
				if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey(key))
				{
					string captured = key;
					if (((Directory<GameItem>)(object)directory).Add(key, (Func<GameItem>)(() => AuctionExclusiveFactory.Create(captured, owned: false))))
					{
						num++;
					}
				}
			}
			if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey("xiaowo_auction_phone_card"))
			{
				((Directory<GameItem>)(object)directory).Add("xiaowo_auction_phone_card", (Func<GameItem>)(() => AuctionPhone.CreateCard(owned: false)));
			}
			Core.Log?.Msg("Auction item directory registered: added=" + num + " total=" + (AuctionExclusiveCatalog.Items.Count + 1) + ".");
		}
	}
	internal static class AuctionContainer
	{
		private sealed class ContainerData
		{
			internal ContainerCategory Category;

			internal int Seed;

			internal int Index;

			internal readonly List<string> Contents = new List<string>();
		}

		private sealed class WindowView
		{
			internal bool OutputRestored;

			internal bool RecoverLegacyOutput;

			internal readonly GameItem Container;

			internal readonly PixelWindow Window;

			internal readonly GameGridInventory Output;

			internal readonly TagElement RemainingLabel;

			internal readonly TagElement HintLabel;

			internal readonly TagElement TakeOnlyLabel;

			internal readonly ButtonElement Button;

			internal WindowView(GameItem container, PixelWindow window, GameGridInventory output, TagElement remainingLabel, TagElement hintLabel, TagElement takeOnlyLabel, ButtonElement button)
			{
				Container = container;
				Window = window;
				Output = output;
				RemainingLabel = remainingLabel;
				HintLabel = hintLabel;
				TakeOnlyLabel = takeOnlyLabel;
				Button = button;
			}
		}

		private sealed class OutputSnapshot
		{
			public string Manifest { get; set; }

			public List<string> Items { get; set; } = new List<string>();
		}

		internal const string InventoryId = "xiaowo_auction_locked_intake";

		private const string OutputTag = "XIAOWO_AUCTION_OUTPUT_SAVE";

		private const int OutputGridSize = 9;

		private const int OutputGridPixelSize = 144;

		private static readonly Dictionary<IntPtr, WindowView> Views = new Dictionary<IntPtr, WindowView>();

		private static readonly string[] NativeLockTags = new string[5] { "LOCKED_CONTAINER_TAG", "LOCK_STATUS", "LOCK_ID", "LOCK_ID_SER", "SUPPLY_CRATE" };

		internal static bool InternalFillActive { get; private set; }

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
				if (item != null && !(item.identifier != "xiaowo_auction_container"))
				{
					ContainerData data = ReadData(item);
					ApplyLocalizedText(item, data);
					if (Views.TryGetValue(ObjectIdentity.Key(item), out var value) && value.Window != null)
					{
						UpdateWindow(item, value);
					}
				}
			}
		}

		internal static GameItem Create(bool owned, ContainerCategory category, int seed, IReadOnlyList<string> contents)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			GameItem obj = DirectoryMaster.Item("service_box", owned) ?? throw new InvalidOperationException("Native container template service_box is unavailable.");
			GeneralHelper.SetItemOwned(obj, owned);
			obj.identifier = "xiaowo_auction_container";
			string name = (obj.identifierName = AuctionCatalog.CategoryName(category));
			obj.name = name;
			obj.shortDescription = AuctionText.Get("container_description", "拍卖场购得的密封集装箱。可连续填充提取区，无法向内存放物品。");
			obj.flavorText = AuctionText.Get("container_flavor", "箱门只认拍卖行内部封签。");
			obj.longDescription = obj.shortDescription;
			obj.SetValue(100L);
			obj.unitCount = 1;
			ApplySprite(obj, category);
			obj.shape = new GridShapeBuilder(2, 2).SetDataFill(2, 2, (byte)1).Build();
			obj.modifiedShape = obj.shape.Clone();
			ClearNativeLockState(obj);
			obj.EnableTag("CONTAINER_ITEM_TAG", false);
			SetData(obj, category, seed, 0, contents);
			ApplyLocalizedText(obj, ReadData(obj));
			obj.SetContentWindow((PixelWindow)null);
			((GameItemFunc)obj).onLoaded = delegate(GameItem loaded)
			{
				Repair(loaded);
			};
			obj.ValidateShapeState();
			return obj;
		}

		internal static void Repair(GameItem item)
		{
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			if (item == null || item.identifier != "xiaowo_auction_container")
			{
				return;
			}
			ContainerData containerData = ReadData(item);
			if (!AuctionLoadLifecycle.IsLoading && AuctionCatalog.RepairRemainingContents(containerData.Category, containerData.Seed, containerData.Index, containerData.Contents))
			{
				SetData(item, containerData.Category, containerData.Seed, containerData.Index, containerData.Contents);
				Core.Log?.Warning("Replaced unavailable item identifiers in a saved auction container.");
			}
			if (!AuctionLoadLifecycle.IsLoading && containerData.Contents.Count > 0)
			{
				AuctionState.AcknowledgeDelivery(containerData.Seed);
			}
			ApplyLocalizedText(item, containerData);
			ApplySprite(item, containerData.Category);
			GridShape shape = item.shape;
			int num;
			if (shape != null && shape.width == 2 && item.shape.height == 2)
			{
				GridShape modifiedShape = item.modifiedShape;
				if (modifiedShape != null && modifiedShape.width == 2)
				{
					num = ((item.modifiedShape.height != 2) ? 1 : 0);
					if (num != 0)
					{
						goto IL_0102;
					}
					goto IL_012d;
				}
			}
			num = 1;
			goto IL_0102;
			IL_012d:
			ClearNativeLockState(item);
			item.SetContentWindow((PixelWindow)null);
			if (num != 0)
			{
				item.ValidateShapeState();
			}
			return;
			IL_0102:
			item.shape = new GridShapeBuilder(2, 2).SetDataFill(2, 2, (byte)1).Build();
			item.modifiedShape = item.shape.Clone();
			goto IL_012d;
		}

		private static void ApplyLocalizedText(GameItem item, ContainerData data)
		{
			string name = (item.identifierName = AuctionCatalog.CategoryName(data.Category));
			item.name = name;
			item.shortDescription = ((data.Index >= data.Contents.Count) ? AuctionText.Get("container_empty", "这个拍卖集装箱已经取空。") : AuctionText.Get("container_remaining", "密封集装箱，剩余 {0} 件。只能取出，不能存入。", data.Contents.Count - data.Index));
			item.longDescription = item.shortDescription;
			item.flavorText = AuctionText.Get("container_flavor", "箱门只认拍卖行内部封签。");
		}

		internal static void RepairForOpenIfNeeded(GameItem item)
		{
			if (item != null)
			{
				GridShape shape = item.shape;
				if (((shape != null) ? new int?(shape.width) : ((int?)null)) == 2 && item.shape.height == 2)
				{
					GridShape modifiedShape = item.modifiedShape;
					if (modifiedShape != null && modifiedShape.width == 2 && item.modifiedShape.height == 2 && !item.IsTag("SUPPLY_CRATE") && !item.IsTag("LOCKED_CONTAINER_TAG") && item.contentWindow == null && !(item.spriteAtlasPath != "xiaowo_night_auction"))
					{
						return;
					}
				}
			}
			Repair(item);
		}

		private static void ClearNativeLockState(GameItem item)
		{
			string[] nativeLockTags = NativeLockTags;
			foreach (string text in nativeLockTags)
			{
				item.DisableTag(text, true);
				TagSystem state = item.state;
				if (state != null)
				{
					state.DisableTag(text);
				}
				TagSystem modifiedState = item.modifiedState;
				if (modifiedState != null)
				{
					modifiedState.DisableTag(text);
				}
			}
		}

		private static void ApplySprite(GameItem item, ContainerCategory category)
		{
			OfficialAssets.SetSprite(item, AuctionCatalog.SpriteName(category));
		}

		internal static string Describe(GameItem item)
		{
			ContainerData containerData = ReadData(item);
			return AuctionCatalog.CategoryName(containerData.Category) + " remaining=" + Math.Max(0, containerData.Contents.Count - containerData.Index) + "/" + containerData.Contents.Count + " index=" + containerData.Index + " seed=" + containerData.Seed;
		}

		internal static string NextItemId(GameItem item)
		{
			ContainerData containerData = ReadData(item);
			if (containerData.Index >= containerData.Contents.Count)
			{
				return string.Empty;
			}
			return containerData.Contents[containerData.Index];
		}

		private static WindowView EnsureWindow(GameItem item)
		{
			if (Views.TryGetValue(ObjectIdentity.Key(item), out var value) && value.Window != null && ObjectIdentity.Key(value.Window) != IntPtr.Zero)
			{
				UpdateWindow(item, value);
				return value;
			}
			return BuildWindow(item);
		}

		private static WindowView BuildWindow(GameItem item)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Expected O, but got Unknown
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected O, but got Unknown
			ContainerData containerData = ReadData(item);
			GridPixelElement val = new GridPixelElement(1, 5, true);
			TagElement val2 = new TagElement(-1, true);
			TagElement val3 = new TagElement(-1, true);
			Attach(val, val2, 0, 0);
			Attach(val, val3, 0, 1);
			GameGridInventory val4 = new GameGridInventory(9, 9)
			{
				identifier = "xiaowo_auction_locked_intake",
				renderDynamicBackground = true
			};
			val4.ForceRecomputeDynamicBackground();
			ContainerHelper.AllowOnlyTaggedItemsOr((GameInventory)(object)val4, new string[1] { "XIAOWO_AUCTION_INTERNAL_ONLY" }, true);
			Attach(val, val4, 0, 2);
			TagElement val5 = new TagElement(-1, true).SetText(AuctionText.Get("take_only", "9×9 提取窗口 · 只能取出，禁止存入"), 10000, (ColorPalette)12822958);
			Attach(val, val5, 0, 3);
			ButtonElement val6 = new ButtonElement(AuctionText.Get("fill_output", "填充提取区"), 144, -1, (Action)delegate
			{
				Dispense(item);
			}, (Color)0, "interface_click");
			Attach(val, val6, 0, 4);
			PixelWindow val7 = new PixelWindow(true, (string)null, (WindowStyle)0);
			val7.SetTitle(AuctionCatalog.CategoryName(containerData.Category));
			val7.Attach((PixelElement)(object)val);
			WindowView windowView = new WindowView(item, val7, val4, val2, val3, val5, val6);
			Views[ObjectIdentity.Key(item)] = windowView;
			UpdateWindow(item, windowView);
			return windowView;
		}

		internal static void OpenTransientWindow(GameItem item, Vector2 mousePosition)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (!Core.IsShuttingDown && !AuctionLoadLifecycle.IsLoading)
			{
				WindowView windowView = EnsureWindow(item);
				RestoreOutput(windowView);
				windowView.Window.Show(mousePosition, Vector2.zero, true);
			}
		}

		internal static void CaptureLoadedOutputs(PlayerStore store)
		{
			if (Core.IsShuttingDown || AuctionLoadLifecycle.IsLoading || Views.Count == 0 || store == null || ObjectIdentity.Key(store) == IntPtr.Zero)
			{
				return;
			}
			List<GameItem> list = store.FindAllItem(true);
			if (list == null)
			{
				return;
			}
			foreach (GameItem item in list)
			{
				if (!(((item != null) ? item.identifier : null) != "xiaowo_auction_container") && Views.TryGetValue(ObjectIdentity.Key(item), out var value) && value.OutputRestored)
				{
					CaptureOutput(value, store);
				}
			}
		}

		internal static void ClearViews()
		{
			Views.Clear();
		}

		private static void CaptureOutput(WindowView view, PlayerStore store = null)
		{
			if (Core.IsShuttingDown)
			{
				return;
			}
			if (store == null)
			{
				store = (PlayerStore.HasInstance ? PlayerStore.Instance : null);
			}
			if (store == null || ObjectIdentity.Key(store) == IntPtr.Zero)
			{
				throw new InvalidOperationException("Cannot snapshot auction cargo without an existing player store.");
			}
			OutputSnapshot outputSnapshot = new OutputSnapshot();
			TagState tagReadonly = view.Container.GetTagReadonly("XIAOWO_AUCTION_CONTAINER_DATA");
			outputSnapshot.Manifest = ((tagReadonly != null) ? tagReadonly.GetString() : null) ?? string.Empty;
			OutputSnapshot outputSnapshot2 = outputSnapshot;
			foreach (GameItem childItem in ((GameInventory)view.Output).childItems)
			{
				if (childItem != null)
				{
					outputSnapshot2.Items.Add(store.EncodeItem(childItem));
				}
			}
			string text = "1:" + Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object)outputSnapshot2)));
			view.Container.state.EnableTag("XIAOWO_AUCTION_OUTPUT_SAVE");
			view.Container.state.GetTag("XIAOWO_AUCTION_OUTPUT_SAVE").SetString(text);
			if (view.Container.modifiedState != null)
			{
				view.Container.modifiedState.EnableTag("XIAOWO_AUCTION_OUTPUT_SAVE");
				view.Container.modifiedState.GetTag("XIAOWO_AUCTION_OUTPUT_SAVE").SetString(text);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void RestoreOutput(WindowView view)
		{
			if (Core.IsShuttingDown || AuctionLoadLifecycle.IsLoading || view.OutputRestored)
			{
				return;
			}
			TagState tagReadonly = view.Container.GetTagReadonly("XIAOWO_AUCTION_OUTPUT_SAVE");
			string text = ((tagReadonly != null) ? tagReadonly.GetString() : null);
			ContainerData containerData = ReadData(view.Container);
			if (string.IsNullOrEmpty(text))
			{
				view.OutputRestored = true;
				if (view.RecoverLegacyOutput)
				{
					Dispense(view.Container);
				}
				return;
			}
			if (!text.StartsWith("1:", StringComparison.Ordinal))
			{
				throw new InvalidOperationException("Unsupported auction extraction save format.");
			}
			OutputSnapshot outputSnapshot = JsonConvert.DeserializeObject<OutputSnapshot>(Encoding.UTF8.GetString(Convert.FromBase64String(text.Substring(2))));
			if (outputSnapshot != null)
			{
				string manifest = outputSnapshot.Manifest;
				TagState tagReadonly2 = view.Container.GetTagReadonly("XIAOWO_AUCTION_CONTAINER_DATA");
				if (!(manifest != ((tagReadonly2 != null) ? tagReadonly2.GetString() : null)))
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
					for (int i = containerData.Index; i < containerData.Contents.Count; i++)
					{
						string key = containerData.Contents[i];
						dictionary[key] = ((!dictionary.TryGetValue(key, out var value)) ? 1 : (value + 1));
					}
					foreach (string item in outputSnapshot.Items)
					{
						SaveState val = JsonUtility.FromJson<SaveState>(item);
						if (val?.saveItems == null || val.saveItems.Count == 0)
						{
							throw new InvalidOperationException("Empty auction extraction item save.");
						}
						SaveItemNode val2 = val.saveItems[0];
						if (!dictionary.TryGetValue(val2.identifier, out var value2) || val2.unitCount < 1 || val2.unitCount > value2)
						{
							throw new InvalidOperationException("Auction extraction item does not belong to its remaining cargo.");
						}
						dictionary[val2.identifier] = value2 - val2.unitCount;
					}
					List<GameItem> list = new List<GameItem>();
					try
					{
						InternalFillActive = true;
						foreach (string item2 in outputSnapshot.Items)
						{
							GameItem val3 = SaveManager.DecodeNodes(JsonUtility.FromJson<SaveState>(item2).saveItems);
							if (val3 == null || !((GameInventory)view.Output).UncheckedAccept(val3))
							{
								throw new InvalidOperationException("Could not restore auction extraction item placement.");
							}
							val3.EnableTag("XIAOWO_AUCTION_INTERNAL_ONLY", false);
							list.Add(val3);
						}
						view.OutputRestored = true;
						view.RecoverLegacyOutput = false;
						UpdateWindow(view.Container, view);
						return;
					}
					catch
					{
						foreach (GameItem item3 in list)
						{
							((GameInventory)view.Output).Expel(item3);
						}
						throw;
					}
					finally
					{
						InternalFillActive = false;
					}
				}
			}
			Core.Log?.Warning("Auction extraction snapshot no longer matches its repaired manifest; restoring remaining cargo.");
			view.OutputRestored = true;
			Dispense(view.Container);
		}

		private static void Attach(GridPixelElement root, TagElement element, int x, int y)
		{
			root.Attach((PixelElement)(object)element, x, y);
		}

		private static void Attach(GridPixelElement root, GameSlotInventory element, int x, int y)
		{
			root.Attach((PixelElement)(object)element, x, y);
		}

		private static void Attach(GridPixelElement root, GameGridInventory element, int x, int y)
		{
			root.Attach((PixelElement)(object)element, x, y);
		}

		private static void Attach(GridPixelElement root, ButtonElement element, int x, int y)
		{
			root.Attach((PixelElement)(object)element, x, y);
		}

		private static void UpdateWindow(GameItem item, WindowView view)
		{
			ContainerData containerData = ReadData(item);
			int num = Math.Max(0, containerData.Contents.Count - containerData.Index);
			view.RemainingLabel.SetText(AuctionText.Get("remaining_count", "剩余物品：{0} / {1}", num, containerData.Contents.Count), 10000, (ColorPalette)12822958);
			view.HintLabel.SetText((num > 0) ? AuctionText.Get("fill_output_hint", "将持续提取，直到 9×9 窗口放不下") : AuctionText.Get("container_empty_short", "集装箱已经取空"), 10000, (ColorPalette)12822958);
			view.TakeOnlyLabel.SetText(AuctionText.Get("take_only", "9×9 提取窗口 · 只能取出，禁止存入"), 10000, (ColorPalette)12822958);
			view.Button.Set((num > 0) ? AuctionText.Get("fill_output", "填充提取区") : AuctionText.Get("empty_button", "已取空"), 144, -1);
			view.Window.SetTitle(AuctionCatalog.CategoryName(containerData.Category));
			view.Output.ForceRecomputeDynamicBackground();
			view.Window.Validate();
			CenterButtonText(view.Button);
		}

		private static void CenterButtonText(ButtonElement button)
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			object? obj = AccessTools.Field(typeof(ButtonElement), "buttonNode")?.GetValue(button);
			object? obj2 = ((obj is TreeNodeRender) ? obj : null);
			RectTransform val = ((obj2 != null) ? ((TreeNodeRender)obj2).rectTransform : null);
			object? obj3 = AccessTools.Field(typeof(ButtonElement), "textNode")?.GetValue(button);
			TreeNodeRender val2 = (TreeNodeRender)((obj3 is TreeNodeRender) ? obj3 : null);
			RectTransform val3 = ((val2 != null) ? val2.rectTransform : null);
			if (!((Object)(object)val == (Object)null) && !((Object)(object)val3 == (Object)null))
			{
				((Transform)val3).SetParent((Transform)(object)val, false);
				val3.anchorMin = Vector2.zero;
				val3.anchorMax = Vector2.one;
				val3.pivot = new Vector2(0.5f, 0.5f);
				val3.anchoredPosition = Vector2.zero;
				val3.sizeDelta = Vector2.zero;
				((Transform)val3).localScale = Vector3.one;
				if ((Object)(object)val2.text != (Object)null)
				{
					((TMP_Text)val2.text).alignment = (TextAlignmentOptions)514;
				}
			}
		}

		internal static void Dispense(GameItem container)
		{
			if (Core.IsShuttingDown || AuctionLoadLifecycle.IsLoading)
			{
				return;
			}
			try
			{
				WindowView windowView = EnsureWindow(container);
				RestoreOutput(windowView);
				ContainerData containerData = ReadData(container);
				if (containerData.Index >= containerData.Contents.Count)
				{
					UpdateWindow(container, windowView);
					return;
				}
				int num = 0;
				int num2 = OutputUnitCount(windowView.Output);
				int num3 = containerData.Index + num2;
				while (num3 < containerData.Contents.Count)
				{
					string text = containerData.Contents[num3];
					GameItem val = DirectoryMaster.Item(text, true);
					if (val == null)
					{
						throw new InvalidOperationException("物品目录中不存在 " + text);
					}
					val.unitCount = 1;
					GameItem val2 = val;
					bool flag = false;
					try
					{
						GeneralHelper.SetItemOwned(val, true);
						val.EnableTag("XIAOWO_AUCTION_INTERNAL_ONLY", false);
						try
						{
							InternalFillActive = true;
							SlotMarker val3 = windowView.Output.TryFindOneValidInventorySlot(val, false);
							if (val3 != null && val3.TryAcceptOnce(-1) > 0)
							{
								flag = true;
								if (val3.targetItem != null && ObjectIdentity.Key(val3.targetItem) != ObjectIdentity.Key(val))
								{
									val2 = val3.targetItem;
									val2.EnableTag("XIAOWO_AUCTION_INTERNAL_ONLY", false);
								}
							}
						}
						finally
						{
							InternalFillActive = false;
						}
					}
					finally
					{
						if (!flag)
						{
							ClearOutputSeal(val);
							GeneralHelper.SetItemOwned(val, false);
						}
					}
					if (!flag)
					{
						break;
					}
					if (val2.parentInventory == null && (val2.parents == null || val2.parents.Count == 0))
					{
						throw new InvalidOperationException("物品未正确写入 9×9 提取窗口：" + text);
					}
					AuctionCollection.Discover(text);
					num3++;
					num++;
				}
				container.shortDescription = ((containerData.Index >= containerData.Contents.Count) ? AuctionText.Get("container_empty", "这个拍卖集装箱已经取空。") : AuctionText.Get("container_remaining", "密封集装箱，剩余 {0} 件。只能取出，不能存入。", containerData.Contents.Count - containerData.Index));
				container.longDescription = container.shortDescription;
				CaptureOutput(windowView);
				UpdateWindow(container, windowView);
				Core.Log?.Msg("Auction container staged " + num + " item(s) in the 9x9 extraction window (" + OutputUnitCount(windowView.Output) + " staged, " + containerData.Index + "/" + containerData.Contents.Count + " claimed).");
			}
			catch (Exception ex)
			{
				Core.Log?.Warning("Could not dispense auction container item: " + ex.Message);
			}
		}

		internal static int OutputCount(GameItem container)
		{
			if (container == null || !Views.TryGetValue(ObjectIdentity.Key(container), out var value))
			{
				return -1;
			}
			return OutputCount(value.Output);
		}

		internal static string OutputItemId(GameItem container)
		{
			if (container != null && Views.TryGetValue(ObjectIdentity.Key(container), out var value))
			{
				GameGridInventory output = value.Output;
				if (((output != null) ? ((GameInventory)output).childItems : null) != null && ((GameInventory)value.Output).childItems.Count != 0)
				{
					GameItem obj = ((GameInventory)value.Output).childItems[0];
					return ((obj != null) ? obj.identifier : null) ?? string.Empty;
				}
			}
			return string.Empty;
		}

		internal static List<GameItem> OutputItemsForTest(GameItem container)
		{
			List<GameItem> list = new List<GameItem>();
			if (container != null && Views.TryGetValue(ObjectIdentity.Key(container), out var value))
			{
				GameGridInventory output = value.Output;
				if (((output != null) ? ((GameInventory)output).childItems : null) != null)
				{
					{
						foreach (GameItem childItem in ((GameInventory)value.Output).childItems)
						{
							if (childItem != null)
							{
								list.Add(childItem);
							}
						}
						return list;
					}
				}
			}
			return list;
		}

		internal static string OutputDiagnostics(GameItem container)
		{
			List<string> list = new List<string>();
			foreach (GameItem item in OutputItemsForTest(container))
			{
				list.Add((item.identifier ?? "<null-id>") + ":uid=" + item.uniqueId + ",parent=" + ((item.parentInventory == null) ? "null" : (item.parentInventory.identifier ?? "<null-inventory-id>")));
			}
			return string.Join("\n", list);
		}

		internal static void RemoveViewForTest(GameItem container)
		{
			if (container != null)
			{
				Views.Remove(ObjectIdentity.Key(container));
			}
		}

		internal static IntPtr WindowPointer(GameItem container)
		{
			if (container == null || !Views.TryGetValue(ObjectIdentity.Key(container), out var value))
			{
				return IntPtr.Zero;
			}
			return ObjectIdentity.Key(value.Window);
		}

		internal static string OutputShape(GameItem container)
		{
			if (container == null || !Views.TryGetValue(ObjectIdentity.Key(container), out var value))
			{
				return "missing";
			}
			return value.Output.inventoryShape.width + "x" + value.Output.inventoryShape.height;
		}

		internal static bool RejectsManualInput(GameItem container, GameItem candidate)
		{
			if (container != null && candidate != null && Views.TryGetValue(ObjectIdentity.Key(container), out var value))
			{
				return !((GameInventory)value.Output).MayHaveValidInventorySlot(candidate);
			}
			return false;
		}

		internal static GameItem TakeOutputForTest(GameItem container)
		{
			if (container != null && Views.TryGetValue(ObjectIdentity.Key(container), out var value))
			{
				GameGridInventory output = value.Output;
				if (((output != null) ? ((GameInventory)output).childItems : null) != null && ((GameInventory)value.Output).childItems.Count != 0)
				{
					GameItem val = ((GameInventory)value.Output).childItems[0];
					if (val == null || !((GameInventory)value.Output).Expel(val))
					{
						return null;
					}
					EmporiumEntry.Instance.TryAddToPlayerInv(val);
					if (val.parentInventory == null && (val.parents == null || val.parents.Count == 0))
					{
						return null;
					}
					UpdateWindow(container, value);
					return val;
				}
			}
			return null;
		}

		internal static void ClearOutputSeal(GameItem item)
		{
			if (item != null)
			{
				item.DisableTag("XIAOWO_AUCTION_INTERNAL_ONLY", true);
				TagSystem state = item.state;
				if (state != null)
				{
					state.DisableTag("XIAOWO_AUCTION_INTERNAL_ONLY");
				}
				TagSystem modifiedState = item.modifiedState;
				if (modifiedState != null)
				{
					modifiedState.DisableTag("XIAOWO_AUCTION_INTERNAL_ONLY");
				}
			}
		}

		internal static void NotifyOutputChanged(GameGridInventory output)
		{
			if (output == null)
			{
				return;
			}
			foreach (WindowView value in Views.Values)
			{
				if (!(ObjectIdentity.Key(value.Output) != ObjectIdentity.Key(output)))
				{
					UpdateWindow(value.Container, value);
					break;
				}
			}
		}

		internal static void CommitClaim(GameGridInventory output, GameItem item)
		{
			if (output == null || item == null || InternalFillActive)
			{
				return;
			}
			foreach (WindowView value3 in Views.Values)
			{
				if (ObjectIdentity.Key(value3.Output) != ObjectIdentity.Key(output))
				{
					continue;
				}
				ContainerData containerData = ReadData(value3.Container);
				int num = Math.Max(1, item.unitCount);
				for (int i = 0; i < num; i++)
				{
					if (containerData.Index >= containerData.Contents.Count)
					{
						break;
					}
					int num2 = -1;
					for (int j = containerData.Index; j < containerData.Contents.Count; j++)
					{
						if (!(containerData.Contents[j] != item.identifier))
						{
							num2 = j;
							break;
						}
					}
					if (num2 < 0)
					{
						break;
					}
					List<string> contents = containerData.Contents;
					int index = containerData.Index;
					List<string> contents2 = containerData.Contents;
					int index2 = num2;
					string value = containerData.Contents[num2];
					string value2 = containerData.Contents[containerData.Index];
					contents[index] = value;
					contents2[index2] = value2;
					containerData.Index++;
				}
				SetData(value3.Container, containerData.Category, containerData.Seed, containerData.Index, containerData.Contents);
				if (containerData.Index >= containerData.Contents.Count)
				{
					AuctionState.RevealHistory(containerData.Seed);
				}
				value3.Container.shortDescription = ((containerData.Index >= containerData.Contents.Count) ? AuctionText.Get("container_empty", "这个拍卖集装箱已经取空。") : AuctionText.Get("container_remaining", "密封集装箱，剩余 {0} 件。只能取出，不能存入。", containerData.Contents.Count - containerData.Index));
				value3.Container.longDescription = value3.Container.shortDescription;
				CaptureOutput(value3);
				UpdateWindow(value3.Container, value3);
				break;
			}
		}

		private static int OutputCount(GameGridInventory output)
		{
			return ((output == null) ? ((int?)null) : ((GameInventory)output).childItems?.Count).GetValueOrDefault();
		}

		private static int OutputUnitCount(GameGridInventory output)
		{
			if (((output != null) ? ((GameInventory)output).childItems : null) == null)
			{
				return 0;
			}
			int num = 0;
			foreach (GameItem childItem in ((GameInventory)output).childItems)
			{
				if (childItem != null)
				{
					num += Math.Max(1, childItem.unitCount);
				}
			}
			return num;
		}

		private static ContainerData ReadData(GameItem item)
		{
			object obj;
			if (item == null)
			{
				obj = null;
			}
			else
			{
				TagState tagReadonly = item.GetTagReadonly("XIAOWO_AUCTION_CONTAINER_DATA");
				obj = ((tagReadonly != null) ? tagReadonly.GetString() : null);
			}
			string text = (string)obj;
			if (string.IsNullOrWhiteSpace(text))
			{
				return new ContainerData();
			}
			string[] array = text.Split('~');
			if (array.Length != 4)
			{
				return new ContainerData();
			}
			ContainerData containerData = new ContainerData();
			int.TryParse(array[0], out var result);
			int.TryParse(array[1], out containerData.Seed);
			int.TryParse(array[2], out containerData.Index);
			containerData.Category = (ContainerCategory)Math.Clamp(result, 0, 3);
			string[] array2 = array[3].Split('|');
			foreach (string text2 in array2)
			{
				if (!string.IsNullOrWhiteSpace(text2))
				{
					containerData.Contents.Add(text2);
				}
			}
			containerData.Index = Math.Clamp(containerData.Index, 0, containerData.Contents.Count);
			return containerData;
		}

		private static void SetData(GameItem item, ContainerCategory category, int seed, int index, IReadOnlyList<string> contents)
		{
			string[] array = new string[7];
			int num = (int)category;
			array[0] = num.ToString();
			array[1] = "~";
			array[2] = seed.ToString();
			array[3] = "~";
			array[4] = index.ToString();
			array[5] = "~";
			array[6] = string.Join("|", contents);
			string text = string.Concat(array);
			item.state.EnableTag("XIAOWO_AUCTION_CONTAINER_DATA");
			item.state.GetTag("XIAOWO_AUCTION_CONTAINER_DATA").SetString(text);
			if (item.modifiedState != null)
			{
				item.modifiedState.EnableTag("XIAOWO_AUCTION_CONTAINER_DATA");
				item.modifiedState.GetTag("XIAOWO_AUCTION_CONTAINER_DATA").SetString(text);
			}
		}
	}
	internal static class AuctionController
	{
		private enum Page
		{
			None,
			Venues,
			VenueConfirm,
			Lots,
			Stage,
			Intel,
			Inspection,
			Manifest,
			Withdraw,
			Settlement,
			History,
			Orders,
			Collection,
			Relationships
		}

		private static CustomUIWindow _officialWindow;

		internal const string RootName = "XIAOWONightAuctionRoot";

		private static GameObject _root;

		private static RectTransform _layout;

		private static TextMeshProUGUI _cash;

		private static TextMeshProUGUI _title;

		private static TextMeshProUGUI _status;

		private static TextMeshProUGUI _eventSummary;

		private static TextMeshProUGUI _sessionSummary;

		private static AuctionLot _manifestLot;

		private static ScrollRect _broadcastScroll;

		private static float _broadcastPosition;

		private static float _nextAnnouncementAt;

		private static bool _fastAnnouncements;

		private static bool _stageVisible;

		private static ContainerCategory _bookCategory;

		private static bool _cancelOrderPending;

		private static Page _page;

		private static AuctionVenue _confirmVenue;

		private static bool _settlementWon;

		private static AuctionLot _settlementLot;

		private static readonly List<GameObject> Dynamic = new List<GameObject>();

		private static List<AuctionLot> _lots = new List<AuctionLot>();

		private static AuctionLot _selected;

		private static AuctionVenue _venue;

		private static string _bidInput = string.Empty;

		private static bool _outsideStarted;

		private static bool _phoneCollection;

		private static bool _phoneRelationships;

		private static int _collectionRequestedAt = -1;

		private static PlayerStore _collectionStore;

		private static int _collectionSlot;

		private static string _collectionRun;

		private static string _collectionRevision;

		private static int _bookPage;

		private static int _tick;

		private static readonly Color Surface = new Color(0.141f, 0.153f, 0.2f, 0.99f);

		private static readonly Color SurfaceRaised = new Color(0.18f, 0.195f, 0.245f, 1f);

		private static readonly Color Frame = new Color(0.075f, 0.085f, 0.11f, 1f);

		private static readonly Color Paper = new Color(0.46f, 0.37f, 0.29f, 1f);

		internal static GameObject Root => _root;

		internal static float AnnouncementInterval
		{
			get
			{
				if (!_fastAnnouncements)
				{
					return 1.8f;
				}
				return 0.45f;
			}
		}

		private static int ResolvedAuctions => _lots.Count((AuctionLot lot) => lot.Finished || lot.Lost);

		private static bool SessionComplete => ResolvedAuctions >= 3;

		internal static bool IsOpen
		{
			get
			{
				if (_outsideStarted)
				{
					return (Object)(object)_root != (Object)null;
				}
				return false;
			}
		}

		internal static bool IsPhoneCollectionOpen
		{
			get
			{
				if (_phoneCollection)
				{
					return (Object)(object)_root != (Object)null;
				}
				return false;
			}
		}

		internal static IReadOnlyList<AuctionLot> TestLots => _lots;

		internal static AuctionVenue TestVenue => _venue;

		private static string T(string chinese, string english)
		{
			return AuctionText.Select(chinese, english);
		}

		internal static void RequestPhoneCollection()
		{
			RequestPhonePanel(relationships: false);
		}

		internal static void RequestPhoneRelationships()
		{
			RequestPhonePanel(relationships: true);
		}

		private static void RequestPhonePanel(bool relationships)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			PlayerStore instance = PlayerStore.Instance;
			if (!((Object)(object)_root != (Object)null) && _collectionRequestedAt < 0 && !_outsideStarted && instance != null && !((Object)(object)EmporiumEntry.Instance == (Object)null))
			{
				Scene activeScene = SceneManager.GetActiveScene();
				if (!(((Scene)(ref activeScene)).name != "InventoryScene"))
				{
					_collectionStore = instance;
					_collectionSlot = instance.saveSlotId;
					_collectionRun = instance.runID;
					_phoneRelationships = relationships;
					_collectionRequestedAt = Time.frameCount;
				}
			}
		}

		private static bool CollectionContextMatches()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			PlayerStore instance = PlayerStore.Instance;
			Scene activeScene = SceneManager.GetActiveScene();
			if (((Scene)(ref activeScene)).name == "InventoryScene" && instance != null && _collectionStore != null && (Object)(object)EmporiumEntry.Instance != (Object)null && instance == _collectionStore && instance.saveSlotId == _collectionSlot)
			{
				return instance.runID == _collectionRun;
			}
			return false;
		}

		private static string CollectionRevision()
		{
			return AuctionState.Read("collection_found") + "|" + AuctionState.Read("collection_claimed");
		}

		private static void OpenPhoneCollection()
		{
			_collectionRequestedAt = -1;
			if (!CollectionContextMatches() || (Object)(object)_root != (Object)null || _outsideStarted)
			{
				return;
			}
			try
			{
				PhoneUIManager instance = PhoneUIManager.Instance;
				if (instance != null)
				{
					instance.CloseUI();
				}
				OverlayHandler current = OverlayHandler.current;
				if (current != null)
				{
					current.HideTooltip();
				}
				_phoneCollection = true;
				BuildRoot();
				if (_phoneRelationships)
				{
					RenderRelationships();
				}
				else
				{
					RenderCollection(_bookCategory);
				}
			}
			catch (Exception ex)
			{
				Core.Log?.Error("Could not open the phone collection book: " + ex);
				Close(returnToStore: false);
			}
		}

		internal static void RefreshLocalizedText()
		{
			if ((Object)(object)_root == (Object)null)
			{
				return;
			}
			if ((Object)(object)_title != (Object)null)
			{
				((TMP_Text)_title).text = (_phoneCollection ? PhonePanelTitle() : T("小窝夜间集装箱拍卖", "Night Container Auction"));
			}
			if ((Object)(object)_cash != (Object)null)
			{
				((TMP_Text)_cash).text = CashCaption();
			}
			switch (_page)
			{
			case Page.Venues:
				RenderVenues();
				break;
			case Page.VenueConfirm:
				RenderVenueConfirm(_confirmVenue);
				break;
			case Page.Lots:
				RenderLots(T("选择一个集装箱。", "Choose a container."));
				break;
			case Page.Stage:
				RenderStage();
				break;
			case Page.Intel:
				RenderIntelMenu();
				break;
			case Page.Inspection:
				RenderInspection();
				break;
			case Page.Manifest:
				if (_manifestLot != null)
				{
					RenderManifest(_manifestLot);
				}
				break;
			case Page.Withdraw:
				RenderWithdrawConfirm();
				break;
			case Page.Settlement:
				if (_settlementLot != null)
				{
					RenderSettlement(_settlementLot, _settlementWon);
				}
				break;
			case Page.History:
				RenderHistory();
				break;
			case Page.Orders:
				RenderOrders();
				break;
			case Page.Collection:
				RenderCollection(_bookCategory);
				break;
			case Page.Relationships:
				RenderRelationships();
				break;
			}
		}

		internal static void Tick()
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (_collectionRequestedAt >= 0 && Time.frameCount > _collectionRequestedAt)
			{
				OpenPhoneCollection();
			}
			if (_phoneCollection && (!CollectionContextMatches() || (Object)(object)_root == (Object)null || !_root.activeInHierarchy))
			{
				Close(returnToStore: false);
				return;
			}
			if (_outsideStarted && (Object)(object)_root != (Object)null && !_root.activeInHierarchy)
			{
				Scene activeScene = SceneManager.GetActiveScene();
				if (((Scene)(ref activeScene)).name == "InventoryScene")
				{
					Core.Log?.Warning("Auction UI was hidden while outside; returning to the store.");
					Close();
					return;
				}
			}
			try
			{
				if ((Object)(object)_root != (Object)null && _stageVisible && _selected != null && _selected.PendingOffer >= 0 && Time.unscaledTime >= _nextAnnouncementAt)
				{
					_nextAnnouncementAt = Time.unscaledTime + AnnouncementInterval;
					AdvanceAuction();
				}
			}
			catch (Exception ex)
			{
				Core.Log?.Warning("Auction announcement failed: " + ex.Message);
			}
			if (_tick-- > 0)
			{
				return;
			}
			_tick = 15;
			try
			{
				if ((Object)(object)_root != (Object)null)
				{
					PositionLayout();
					if ((Object)(object)_cash != (Object)null)
					{
						((TMP_Text)_cash).text = CashCaption();
					}
					if (_page == Page.Collection && _collectionRevision != CollectionRevision())
					{
						RenderCollection(_bookCategory);
					}
				}
			}
			catch (Exception ex2)
			{
				Core.Log?.Warning("Auction UI refresh failed: " + ex2.Message);
			}
		}

		internal static void Dispose(bool returnToStore = true)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			int returnToStore2;
			if (returnToStore && _outsideStarted)
			{
				Scene activeScene = SceneManager.GetActiveScene();
				returnToStore2 = ((((Scene)(ref activeScene)).name == "InventoryScene") ? 1 : 0);
			}
			else
			{
				returnToStore2 = 0;
			}
			Close((byte)returnToStore2 != 0);
			_lots.Clear();
		}

		internal static void TryDeliverPendingFromHook()
		{
			TryDeliverPending();
		}

		internal static void EnsureRuntime()
		{
			TryDeliverPending();
		}

		internal static string BuildStatus()
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			List<string> list = new List<string>
			{
				"NIGHT_AUCTION",
				"visible=" + ((Object)(object)_root != (Object)null && _root.activeInHierarchy),
				"lots=" + _lots.Count
			};
			Scene activeScene = SceneManager.GetActiveScene();
			if (((Scene)(ref activeScene)).name != "InventoryScene")
			{
				return string.Join("\n", list);
			}
			AuctionVenue venue;
			string[] obj = new string[4]
			{
				"resolvedToday=" + ResolvedAuctions,
				"venue=" + (AuctionState.TryGetVenue(out venue) ? venue.ToString() : "none"),
				"selected=" + (_selected?.Slot.ToString() ?? "none"),
				null
			};
			PlayerStore instance = PlayerStore.Instance;
			obj[3] = "cash=" + ((instance != null) ? instance.GetCash() : (-1));
			list.AddRange(obj);
			foreach (AuctionLot lot in _lots)
			{
				list.Add("lot" + lot.Slot + "=" + lot.Category.ToString() + ",items=" + lot.ItemCount + ",bid=" + lot.CurrentBid + ",auction=" + ((lot.Finished || lot.Lost) ? "resolved" : (lot.Started ? "in-progress" : "pending")) + ",round=" + lot.Round + ",pending=" + lot.PendingOffer + ",biddersShown=" + lot.BiddersShown + ",mode=three-lots-five-rounds,reference=" + lot.ReferenceValue + ",actual=" + lot.EstimatedValue + ",quality=" + lot.Quality.ToString() + ",leader=" + lot.HighestBidder + ",leading=" + lot.PlayerLeading + ",finished=" + lot.Finished + ",lost=" + lot.Lost + ",withdrawn=" + lot.Withdrawn);
			}
			return string.Join("\n", list);
		}

		internal static void Open()
		{
			if ((Object)(object)_root != (Object)null || !Core.StoreReady || (Object)(object)EmporiumEntry.Instance == (Object)null)
			{
				return;
			}
			try
			{
				BeginAfterHours();
				BuildRoot();
				_selected = null;
				_bidInput = string.Empty;
				if (AuctionState.TryGetVenue(out _venue))
				{
					_lots = AuctionState.LoadOrCreateLots(_venue);
					RenderLots(SessionComplete ? T("今晚三场竞拍已经全部结束。", "All three auctions are complete tonight.") : (T("选择集装箱进行第 ", "Choose a container for auction ") + (ResolvedAuctions + 1) + " / 3."));
				}
				else
				{
					_lots = new List<AuctionLot>();
					RenderVenues();
				}
				Core.Log?.Msg("Entered the night container auction.");
			}
			catch (Exception ex)
			{
				Core.Log?.Error("Could not enter night auction: " + ex);
				Close();
			}
		}

		internal static void Close(bool returnToStore = true)
		{
			try
			{
				CustomUIWindow officialWindow = _officialWindow;
				_officialWindow = null;
				if (officialWindow != null)
				{
					officialWindow.onClose = null;
					officialWindow.Close();
				}
				else if ((Object)(object)_root != (Object)null)
				{
					Object.Destroy((Object)(object)_root);
				}
				_root = null;
				_layout = null;
				_cash = null;
				_title = null;
				_status = null;
				_selected = null;
				_eventSummary = null;
				_sessionSummary = null;
				_manifestLot = null;
				_page = Page.None;
				_settlementLot = null;
				_broadcastScroll = null;
				_broadcastPosition = 0f;
				_nextAnnouncementAt = 0f;
				_stageVisible = false;
				_bidInput = string.Empty;
				Dynamic.Clear();
				bool outsideStarted = _outsideStarted;
				_outsideStarted = false;
				if (returnToStore && outsideStarted)
				{
					EndAfterHours();
					Core.Log?.Msg("Left the night container auction and returned to the store.");
				}
			}
			catch (Exception ex)
			{
				Core.Log?.Error("Could not leave night auction cleanly: " + ex);
			}
			finally
			{
				_outsideStarted = false;
				_phoneCollection = false;
				_phoneRelationships = false;
				_collectionRequestedAt = -1;
				_collectionStore = null;
				_collectionRun = null;
				_collectionRevision = null;
			}
		}

		private static void RenderVenues()
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			_page = Page.Venues;
			ClearDynamic();
			SetStatus(T("确认入场后锁定本晚场次，再次进入不重复收费。", "Admission locks tonight's venue; re-entry is free."));
			StageText("VenueHeading", T("选择拍卖场次", "Choose Tonight's Venue"), 16f, 24f, 133f, 900f, 28f, Color.white, bold: true);
			foreach (AuctionVenue venue in Enum.GetValues(typeof(AuctionVenue)))
			{
				int num = (int)venue;
				int num2 = 24 + num * 312;
				Color color = (new Color[3]
				{
					new Color(0.6f, 0.76f, 0.69f),
					AuctionUiStyle.Gold,
					new Color(0.75f, 0.54f, 0.57f)
				})[num];
				Dynamic.Add(CreatePanel("VenuePanel" + num, FromTop(num2, 174f, 288f, 300f), Surface));
				Dynamic.Add(CreateAccent("VenueAccent" + num, FromTop(num2, 174f, 288f, 2f), color));
				StageText("VenueEyebrow" + num, (new string[3] { "STARTER", "INTERMEDIATE", "ADVANCED" })[num], 9f, num2 + 20, 193f, 248f, 19f, color);
				StageText("VenueTitle" + num, AuctionRules.VenueName(venue), 23f, num2 + 20, 219f, 248f, 36f, Color.white, bold: true);
				(int, int) tuple = AuctionRules.BaselineRange(venue);
				(int, int) tuple2 = AuctionRules.ItemCountRange(venue);
				string[] array = new string[4]
				{
					T("入场费", "Admission"),
					T("推荐现金", "Suggested cash"),
					T("预计成交", "Expected bid"),
					T("箱内数量", "Cargo count")
				};
				string[] array2 = new string[4]
				{
					AuctionNightMarket.EntryText(venue),
					Money(AuctionRules.RecommendedCash(venue)),
					tuple.Item1.ToString("N0") + "–" + Money(tuple.Item2),
					tuple2.Item1 + "–" + tuple2.Item2 + T(" 件", " items")
				};
				for (int i = 0; i < 4; i++)
				{
					StageText("VenueLabel" + num + "_" + i, array[i], 10f, num2 + 20, 280 + i * 28, 100f, 20f, AuctionUiStyle.Muted);
					((TMP_Text)StageText("VenueValue" + num + "_" + i, array2[i], 11f, num2 + 124, 280 + i * 28, 144f, 20f, Color.white)).alignment = (TextAlignmentOptions)260;
				}
				StageText("VenueNote" + num, T("每晚三场竞拍 · 自主选择集装箱", "Three auctions per night · Choose your lots"), 9f, num2 + 20, 406f, 248f, 24f, AuctionUiStyle.Muted);
				Dynamic.Add(CreateButton("ChooseVenue" + num, T("查看并进入", "View & Enter"), FromTop(num2 + 20, 436f, 248f, 28f), delegate
				{
					RenderVenueConfirm(venue);
				}, enabled: true));
			}
			AddLobbyServices();
		}

		private static void RenderVenueConfirm(AuctionVenue venue)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			_page = Page.VenueConfirm;
			_confirmVenue = venue;
			ClearDynamic();
			int num = AuctionNightMarket.EntryFee(venue);
			Dynamic.Add(CreatePanel("VenueConfirm", new Rect(214f, 152f, 532f, 240f), (venue == AuctionVenue.Advanced) ? new Color(0.6f, 0.29f, 0.31f) : new Color(0.38f, 0.54f, 0.5f)));
			AddStageLabel("VenueConfirmTitle", T("进入“", "Enter ") + AuctionRules.VenueName(venue) + T("”", ""), 25f, 252f, -168f, 456f, 42f, (FontStyles)1);
			AddStageLabel("VenueConfirmText", T("本次入场费：", "Entry fee: ") + AuctionNightMarket.EntryText(venue) + "\n" + T("进入后今晚不能更换场次。\n离开后重新进入同一场次不会重复收费。", "You cannot change venues tonight.\nRe-entry to this venue is free."), 17f, 252f, -230f, 456f, 100f, (FontStyles)0);
			Dynamic.Add(CreateButton("ConfirmVenue", (num == 0) ? T("免费进入", "Enter Free") : (T("支付 ", "Pay ") + num + T("¢ 并进入", "¢ / Enter")), new Rect(277f, 174f, 220f, 42f), delegate
			{
				EnterVenue(venue);
			}, PlayerStore.Instance.GetCash() >= num));
			Dynamic.Add(CreateButton("CancelVenue", T("返回选择", "Back"), new Rect(515f, 174f, 168f, 42f), RenderVenues, enabled: true));
			SetStatus(T("入场费即使当晚没有成交也不会退还。", "Entry fees are nonrefundable even without a sale."));
		}

		private static void EnterVenue(AuctionVenue venue)
		{
			if (!AuctionState.EnterVenue(venue, out var error))
			{
				RenderVenueConfirm(venue);
				SetStatus(error);
				return;
			}
			_venue = venue;
			_lots = AuctionState.LoadOrCreateLots(venue);
			RenderLots(T("已进入“", "Entered ") + AuctionRules.VenueName(venue) + T("”。选择一个集装箱开始竞拍。", ". Choose a container to bid on."));
			Core.Log?.Msg("Auction venue entered: " + venue.ToString() + ", fee=" + AuctionState.VenueFee(venue) + ".");
		}

		private static void BeginAfterHours()
		{
			_outsideStarted = true;
		}

		private static void EndAfterHours()
		{
			if (Core.StoreReady && ModHelper.GetCurrentAfterhourLocationId() == "xiaowo_night_auction")
			{
				ModHelper.ReturnFromAfterhour(false);
			}
		}

		private static void BuildRoot()
		{
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_0542: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			CustomUIManager instance = CustomUIManager.Instance;
			if ((Object)(object)instance == (Object)null)
			{
				throw new InvalidOperationException("Official UI manager is unavailable.");
			}
			_officialWindow = instance.CreateWindow("xiaowo_night_auction:auction", "", "overlay", (GameObject)null, (Style)0).SetDraggable(_phoneCollection).SetSize(960f, 540f)
				.Center()
				.SetCloseOnEscape(true)
				.OnClose((Action)delegate
				{
					Close(!_phoneCollection);
				})
				.Handle;
			_root = _officialWindow.root;
			((Object)_root).name = "XIAOWONightAuctionRoot";
			_root.transform.SetAsLastSibling();
			if (!_phoneCollection)
			{
				Stretch(_officialWindow.Rect);
				((Transform)_officialWindow.Rect).localScale = Vector3.one;
				Image obj = _root.GetComponent<Image>() ?? _root.AddComponent<Image>();
				((Graphic)obj).color = Frame;
				((Graphic)obj).raycastTarget = true;
				if ((Object)(object)_officialWindow.frame != (Object)null && (Object)(object)_officialWindow.frame.closeButton != (Object)null)
				{
					((Component)_officialWindow.frame.closeButton).gameObject.SetActive(false);
				}
			}
			if (_phoneCollection)
			{
				Image obj2 = _root.GetComponent<Image>() ?? _root.AddComponent<Image>();
				((Graphic)obj2).color = new Color(0f, 0f, 0f, 0.62f);
				((Graphic)obj2).raycastTarget = true;
			}
			_layout = NewObject("Layout", _root.transform, typeof(LayoutElement)).GetComponent<RectTransform>();
			((Component)_layout).GetComponent<LayoutElement>().ignoreLayout = true;
			RectTransform layout = _layout;
			RectTransform layout2 = _layout;
			Vector2 val = default(Vector2);
			((Vector2)(ref val))..ctor(0.5f, 0.5f);
			layout2.anchorMax = val;
			layout.anchorMin = val;
			_layout.pivot = new Vector2(0.5f, 0.5f);
			_layout.sizeDelta = new Vector2(960f, 540f);
			_layout.anchoredPosition = Vector2.zero;
			GameObject val2 = NewObject("Backdrop", (Transform)(_phoneCollection ? ((object)_layout) : ((object)_root.transform)), typeof(Image), typeof(LayoutElement));
			val2.GetComponent<LayoutElement>().ignoreLayout = true;
			if (!_phoneCollection)
			{
				val2.transform.SetAsFirstSibling();
			}
			Stretch(val2.GetComponent<RectTransform>());
			Image component = val2.GetComponent<Image>();
			((Graphic)component).color = new Color(0.025f, 0.028f, 0.035f, 0.992f);
			if (!_phoneCollection)
			{
				component.sprite = OfficialAssets.Get("auction_background");
				((Graphic)component).color = Color.white;
			}
			if (_phoneCollection)
			{
				component.sprite = null;
				((Graphic)component).color = Frame;
				GameObject obj3 = NewObject("BookSurface", (Transform)(object)_layout, typeof(Image));
				SetRect(obj3.GetComponent<RectTransform>(), new Rect(4f, 4f, 952f, 532f));
				((Graphic)obj3.GetComponent<Image>()).color = Surface;
				((Graphic)obj3.GetComponent<Image>()).raycastTarget = false;
			}
			((Graphic)component).raycastTarget = false;
			if (_phoneCollection)
			{
				GameObject obj4 = NewObject("Header", (Transform)(object)_layout, typeof(Image));
				SetRect(obj4.GetComponent<RectTransform>(), new Rect(24f, 458f, 912f, 58f));
				((Graphic)obj4.GetComponent<Image>()).color = SurfaceRaised;
				((Graphic)obj4.GetComponent<Image>()).raycastTarget = false;
				GameObject obj5 = NewObject("HeaderAccent", (Transform)(object)_layout, typeof(Image));
				SetRect(obj5.GetComponent<RectTransform>(), new Rect(24f, 458f, 6f, 58f));
				((Graphic)obj5.GetComponent<Image>()).color = new Color(0.69f, 0.38f, 0.25f, 1f);
				((Graphic)obj5.GetComponent<Image>()).raycastTarget = false;
				GameObject obj6 = NewObject("StatusBar", (Transform)(object)_layout, typeof(Image));
				SetRect(obj6.GetComponent<RectTransform>(), new Rect(72f, 430f, 816f, 24f));
				((Graphic)obj6.GetComponent<Image>()).color = new Color(0.055f, 0.06f, 0.07f, 0.98f);
				((Graphic)obj6.GetComponent<Image>()).raycastTarget = false;
				_title = CreateLabel("Title", _phoneCollection ? PhonePanelTitle() : AuctionText.Get("title", "夜间集装箱拍卖"), 26f, new Vector2(44f, -29f), new Vector2(500f, 30f), (FontStyles)1);
				_eventSummary = (_phoneCollection ? null : CreateLabel("NightMarketSummary", "", 11f, new Vector2(44f, -62f), new Vector2(668f, 18f), (FontStyles)0));
				if ((Object)(object)_eventSummary != (Object)null)
				{
					((TMP_Text)_eventSummary).enableWordWrapping = false;
					((TMP_Text)_eventSummary).overflowMode = (TextOverflowModes)1;
				}
				_cash = CreateLabel("Cash", AuctionText.Get("cash", "现金  {0}", PlayerStore.Instance.GetCash()), 18f, new Vector2((float)(_phoneCollection ? 670 : 720), -34f), new Vector2(196f, 34f), (FontStyles)1);
				((TMP_Text)_cash).alignment = (TextAlignmentOptions)260;
				_status = CreateLabel("Status", "", 13f, new Vector2(82f, -87f), new Vector2(796f, 22f), (FontStyles)0);
				((TMP_Text)_status).alignment = (TextAlignmentOptions)514;
			}
			else
			{
				BuildAuctionHeader();
			}
			if (_phoneCollection)
			{
				CreateButton("CollectionClose", "X", new Rect(892f, 478f, 28f, 28f), delegate
				{
					Close(returnToStore: false);
				}, enabled: true);
			}
			_officialWindow.Show();
			PositionLayout();
		}

		private static string MainMix(AuctionLot lot)
		{
			int num = AuctionNightMarket.MainPercent(lot.Category);
			return T("主类 ", "Main ") + num + T("% / 混合 ", "% / mixed ") + (100 - num) + "%";
		}

		private static Sprite PreviewSprite(GameItem item)
		{
			if (!item.useSpriteFromMod)
			{
				return RenderHandler.LoadFromAtlas(item.spriteAtlasPath, item.spritePath);
			}
			return CustomUIManager.Instance.GetModSprite(item.spriteAtlasPath, item.spritePath, true);
		}

		private static void RenderManifest(AuctionLot lot)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			_manifestLot = lot;
			_page = Page.Manifest;
			ClearDynamic();
			string[] array = AuctionNightMarket.LeakItems(lot);
			Dynamic.Add(CreatePanel("ManifestPanel", new Rect(174f, 108f, 612f, 306f), AuctionCatalog.CategoryColor(lot.Category)));
			AddStageLabel("ManifestTitle", T("泄露货单 · 两件真实货物", "Leaked Manifest · Two Actual Items"), 21f, 202f, -139f, 556f, 32f, (FontStyles)1);
			for (int i = 0; i < array.Length; i++)
			{
				GameItem val = DirectoryMaster.Item(array[i], false);
				int num = 238 + i * 250;
				GameObject val2 = NewObject("ManifestSprite" + i, (Transform)(object)_layout, typeof(Image));
				SetRect(val2.GetComponent<RectTransform>(), new Rect((float)(num + 45), 236f, 100f, 100f));
				Image component = val2.GetComponent<Image>();
				component.sprite = PreviewSprite(val);
				component.preserveAspect = true;
				((Graphic)component).raycastTarget = false;
				Dynamic.Add(val2);
				AddStageLabel("ManifestName" + i, val.name, 15f, num, -315f, 200f, 44f, (FontStyles)1);
				AddStageLabel("ManifestValue" + i, T("单件参考价值 ", "Unit value ") + AuctionCatalog.ItemValue(array[i]) + "¢", 13f, num, -365f, 200f, 28f, (FontStyles)0);
			}
			Dynamic.Add(CreateButton("ManifestBack", T("返回四箱", "Four Lots"), new Rect(390f, 55f, 180f, 36f), delegate
			{
				RenderLots(T("货单仅公开部分货物，不代表箱内总价值。", "The manifest reveals only part of the cargo, not total value."));
			}, enabled: true));
			SetStatus(T("免费线索，与付费四件抽检独立；每次查看内容相同。", "Free clues, separate from the paid four-item sample; repeated views are identical."));
		}

		private static void ReturnLobby()
		{
			if (AuctionState.TryGetVenue(out var _))
			{
				RenderLots(T("选择集装箱。", "Choose a container."));
			}
			else
			{
				RenderVenues();
			}
		}

		private static void RenderOrders()
		{
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			_page = Page.Orders;
			ClearDynamic();
			AuctionOrder active = AuctionCommissions.Active;
			bool flag = active.Id.Length > 0 && !active.Claimed && (AuctionOrderRules.Complete(active) || !AuctionOrderRules.Expired(active, AuctionCatalog.Day));
			SetStatus(T("采购委托", "Procurement Orders") + "  ·  " + T("第 ", "Week ") + ((AuctionCatalog.Day - 1) / 7 + 1) + T(" 周", ""));
			if (flag)
			{
				Dynamic.Add(CreatePanel("ActiveOrder", new Rect(160f, 148f, 640f, 254f), AuctionCatalog.CategoryColor(active.Category)));
				AddStageLabel("OrderHeading", T("当前委托", "Active Order"), 20f, 184f, -156f, 560f, 34f, (FontStyles)1);
				AddStageLabel("OrderDetails", AuctionCommissions.Description(active, active: true), 17f, 184f, -204f, 570f, 134f, (FontStyles)0);
				bool complete = AuctionOrderRules.Complete(active);
				AddStageLabel("OrderActionStatus", complete ? (T("可领取 ", "Reward available: ") + AuctionOrderRules.Reward(active) + "¢") : (_cancelOrderPending ? T("确认取消？本周这一单无法重新接取。", "Cancel this order? It cannot be accepted again this week.") : T("进行中", "In progress")), 14f, 184f, -350f, 570f, 34f, (FontStyles)0);
				Dynamic.Add(CreateButton("OrderAction", complete ? T("领取奖励", "Claim Reward") : (_cancelOrderPending ? T("确认取消", "Confirm Cancel") : T("取消委托", "Cancel Order")), new Rect(250f, 92f, 210f, 38f), delegate
				{
					if (complete)
					{
						AuctionCommissions.Claim();
					}
					else if (_cancelOrderPending)
					{
						AuctionCommissions.Cancel();
						_cancelOrderPending = false;
					}
					else
					{
						_cancelOrderPending = true;
					}
					RenderOrders();
				}, enabled: true));
			}
			else
			{
				foreach (AuctionOrder order in AuctionCommissions.Offers())
				{
					int category = (int)order.Category;
					int num = 54 + category % 2 * 438;
					int num2 = ((category < 2) ? 257 : 88);
					Dynamic.Add(CreatePanel("Order" + category, new Rect((float)num, (float)num2, 414f, 157f), AuctionCatalog.CategoryColor(order.Category)));
					AddStageLabel("OrderText" + category, AuctionCommissions.Description(order, active: false), 13f, num + 14, -(540 - num2 - 143), 386f, 104f, (FontStyles)0);
					bool flag2 = AuctionCommissions.Available(order);
					Dynamic.Add(CreateButton("AcceptOrder" + category, flag2 ? T("接取委托", "Accept Order") : T("本周已接取", "Already Accepted"), new Rect((float)(num + 228), (float)(num2 + 9), 170f, 29f), delegate
					{
						AuctionCommissions.Accept(order.Category);
						RenderOrders();
					}, flag2));
				}
			}
			Dynamic.Add(CreateButton("OrdersBack", T("返回拍卖大厅", "Auction Lobby"), new Rect(500f, (float)(flag ? 92 : 32), 210f, 38f), delegate
			{
				_cancelOrderPending = false;
				ReturnLobby();
			}, enabled: true));
		}

		private static string PhonePanelTitle()
		{
			if (!_phoneRelationships)
			{
				return T("藏品收集册", "Collection Book");
			}
			return T("合作商人档案", "Buyer Relationships");
		}

		private static void RenderRelationships()
		{
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			_page = Page.Relationships;
			ClearDynamic();
			SetStatus(T("四类关系独立培养 · 只接受电话预约 · 没有每日衰减", "Four independent relationships · Visits by phone booking only · No daily decay"));
			foreach (ContainerCategory value in Enum.GetValues(typeof(ContainerCategory)))
			{
				int num = (int)value;
				int num2 = 54 + num % 2 * 438;
				int num3 = ((num < 2) ? 250 : 76);
				long num4 = AuctionMerchantRelations.Gifts(value);
				int num5 = AuctionMerchantRules.Tier(num4);
				int num6 = ((num5 < 3) ? AuctionMerchantRules.Threshold(num5 + 1) : AuctionMerchantRules.Threshold(3));
				Dynamic.Add(CreatePanel("Relationship" + num, new Rect((float)num2, (float)num3, 414f, 160f), AuctionCatalog.CategoryColor(value)));
				string text = AuctionMerchants.DisplayName(value) + " · " + AuctionMerchantRelations.TierName(value) + T("\n累计赠礼：", "\nTotal gifts: ") + num4.ToString("N0") + ((num5 < 3) ? (" / " + num6.ToString("N0")) : "") + "¢" + T("\n收购加成 +", "\nBuying premium +") + AuctionMerchantRules.Premium(num4) + T("% · 到访预算 ", "% · Visit budget ") + AuctionMerchantRules.Budget(num4).ToString("N0") + "¢" + T("\n预约间隔：", "\nBooking interval: ") + AuctionMerchantRules.CallInterval(num4) + T("天（所有类别共用冷却）", " days (shared by all buyers)") + ((num5 < 3) ? (T("\n距离下一档还需赠礼 ", "\nNext tier needs ") + (num6 - num4).ToString("N0") + "¢") : T("\n已满级，无需继续赠礼。", "\nMaximum tier. No further gifts needed."));
				AddStageLabel("RelationshipText" + num, text, 14f, num2 + 12, -(540 - num3 - 148), 390f, 127f, (FontStyles)0);
				Dynamic.Add(CreateAccent("RelationshipTrack" + num, new Rect((float)(num2 + 12), (float)(num3 + 10), 390f, 8f), Color.gray));
				Dynamic.Add(CreateAccent("RelationshipProgress" + num, new Rect((float)(num2 + 12), (float)(num3 + 10), 390f * Mathf.Clamp01((float)num4 / (float)num6), 8f), AuctionCatalog.CategoryColor(value)));
			}
			Dynamic.Add(CreateButton("RelationshipClose", T("关闭档案", "Close Records"), new Rect(708f, 28f, 200f, 32f), delegate
			{
				Close(returnToStore: false);
			}, enabled: true));
		}

		private static void RenderCollection(ContainerCategory category)
		{
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0354: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			_page = Page.Collection;
			if (_bookCategory != category)
			{
				_bookPage = 0;
			}
			_bookCategory = category;
			ClearDynamic();
			AuctionCollection.ScanOwned();
			_collectionRevision = CollectionRevision();
			HashSet<string> found = AuctionCollection.Found;
			SetStatus(T("藏品收集册  ·  ", "Collection Book  ·  ") + found.Count(AuctionExclusiveCatalog.IsExclusive) + "/" + AuctionExclusiveCatalog.Items.Count);
			for (int i = 0; i < 4; i++)
			{
				ContainerCategory tab = (ContainerCategory)i;
				Dynamic.Add(CreateButton("CollectionTab" + i, AuctionPhone.CategoryCaption(tab) + " " + AuctionCollection.Count(tab) + "/" + AuctionExclusiveCatalog.CategoryCount(tab), new Rect((float)(54 + i * 218), 390f, 200f, 32f), delegate
				{
					RenderCollection(tab);
				}, tab != category));
			}
			AuctionExclusiveSpec[] array = AuctionExclusiveCatalog.Items.Values.Where((AuctionExclusiveSpec spec) => spec.Category == category).ToArray();
			int num = Math.Max(1, (array.Length + 8 - 1) / 8);
			_bookPage = Math.Clamp(_bookPage, 0, num - 1);
			int num2 = _bookPage * 8;
			for (int num3 = num2; num3 < Math.Min(array.Length, num2 + 8); num3++)
			{
				AuctionExclusiveSpec auctionExclusiveSpec = array[num3];
				bool flag = found.Contains(auctionExclusiveSpec.Id);
				int num4 = num3 - num2;
				int num5 = 54 + num4 % 4 * 218;
				int num6 = ((num4 < 4) ? 242 : 110);
				Dynamic.Add(CreatePanel("CollectionItem" + num3, new Rect((float)num5, (float)num6, 200f, 124f), AuctionCatalog.CategoryColor(category)));
				GameObject val = NewObject("CollectionSprite" + num3, (Transform)(object)_layout, typeof(Image));
				SetRect(val.GetComponent<RectTransform>(), new Rect((float)(num5 + 10), (float)(num6 + 38), 72f, 72f));
				Image component = val.GetComponent<Image>();
				component.sprite = OfficialAssets.Get(auctionExclusiveSpec.Id);
				component.preserveAspect = true;
				((Graphic)component).raycastTarget = false;
				((Graphic)component).color = (Color)(flag ? Color.white : new Color(0.025f, 0.025f, 0.025f, 1f));
				Dynamic.Add(val);
				TextMeshProUGUI val2 = CreateLabel("CollectionName" + num3, flag ? AuctionCollection.Name(auctionExclusiveSpec) : "???", 13f, new Vector2((float)(num5 + 88), (float)(-(540 - num6 - 116))), new Vector2(102f, 78f), (FontStyles)1);
				((TMP_Text)val2).enableAutoSizing = true;
				((TMP_Text)val2).fontSizeMin = 9f;
				((TMP_Text)val2).fontSizeMax = 13f;
				((TMP_Text)val2).ForceMeshUpdate(false, false);
				Dynamic.Add(FindLayoutChild(((TMP_Text)val2).transform));
				AddStageLabel("CollectionValue" + num3, flag ? (T("基础价值 ", "Base value ") + auctionExclusiveSpec.Value + "¢") : T("未发现", "Undiscovered"), 12f, num5 + 12, -(540 - num6 - 29), 180f, 22f, (FontStyles)0);
			}
			Dynamic.Add(CreateButton("CollectionPrevious", T("上一页", "Previous"), new Rect(330f, 74f, 110f, 24f), delegate
			{
				_bookPage--;
				RenderCollection(category);
			}, _bookPage > 0));
			AddStageLabel("CollectionPage", T("第 ", "Page ") + (_bookPage + 1) + "/" + num, 12f, 448f, -444f, 128f, 22f, (FontStyles)0);
			Dynamic.Add(CreateButton("CollectionNext", T("下一页", "Next"), new Rect(586f, 74f, 110f, 24f), delegate
			{
				_bookPage++;
				RenderCollection(category);
			}, _bookPage + 1 < num));
			Dynamic.Add(CreateButton("CollectionReward", AuctionCollection.IsClaimed((int)category) ? T("分类奖励已领取", "Category Reward Claimed") : T("分类奖励 2000¢", "Category Reward 2000¢"), new Rect(54f, 28f, 218f, 32f), delegate
			{
				AuctionCollection.Claim((int)category);
				RenderCollection(category);
			}, AuctionCollection.CanClaim((int)category)));
			Dynamic.Add(CreateButton("CollectionFullReward", AuctionCollection.IsClaimed(4) ? T("全册奖励已领取", "Full Book Reward Claimed") : T("全册奖励 5000¢", "Full Book Reward 5000¢"), new Rect(294f, 28f, 218f, 32f), delegate
			{
				AuctionCollection.Claim(4);
				RenderCollection(category);
			}, AuctionCollection.CanClaim(4)));
			Dynamic.Add(CreateButton("CollectionBack", _phoneCollection ? T("关闭收集册", "Close Book") : T("返回拍卖大厅", "Auction Lobby"), new Rect(708f, 28f, 200f, 32f), delegate
			{
				if (_phoneCollection)
				{
					Close(returnToStore: false);
				}
				else
				{
					ReturnLobby();
				}
			}, enabled: true));
		}

		private static void SelectLot(AuctionLot lot)
		{
			if (!SessionComplete && !lot.Finished && !lot.Lost && !_lots.Any((AuctionLot other) => other != lot && other.Started && !other.Finished && !other.Lost))
			{
				_selected = lot;
				_broadcastPosition = 0f;
				AuctionRivals.Prepare(lot);
				AuctionState.StartLot(lot, _lots);
				AuctionState.SaveLots(_lots);
				_bidInput = string.Empty;
				RenderStage();
			}
		}

		private static void RenderStage(string message = null)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_063e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0677: Unknown result type (might be due to invalid IL or missing references)
			//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0745: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0774: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0801: Unknown result type (might be due to invalid IL or missing references)
			//IL_0847: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_090c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0911: Unknown result type (might be due to invalid IL or missing references)
			//IL_094d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0977: Unknown result type (might be due to invalid IL or missing references)
			//IL_097c: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0caa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0db7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0eb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fc9: Unknown result type (might be due to invalid IL or missing references)
			AuctionLot selected = _selected;
			if (selected == null)
			{
				return;
			}
			_page = Page.Stage;
			ClearDynamic();
			_stageVisible = true;
			if (selected.PendingOffer >= 0 && _nextAnnouncementAt <= 0f)
			{
				_nextAnnouncementAt = Time.unscaledTime + AnnouncementInterval;
			}
			Dynamic.Add(CreatePanel("Seats", FromTop(24f, 138f, 164f, 347f), Surface));
			StageText("SeatTitle", T("竞拍席位", "Bidders"), 11f, 36f, 149f, 140f, 20f, AuctionUiStyle.Gold, bold: true);
			StageText("PlayerSeat", T("你    你的席位", "You · Your seat"), 11f, 36f, 181f, 140f, 20f, Color.white, bold: true);
			Dynamic.Add(CreatePanel("PlayerBidBubble", FromTop(36f, 207f, 140f, 27f), SurfaceRaised));
			StageText("PlayerBidText", (selected.PendingOffer >= 0 && selected.PendingOffer > 0) ? (T("已报 ", "Bid ") + Money(selected.PendingOffer)) : ((selected.PlayerOffer > 0) ? (T("上轮 ", "Last ") + Money(selected.PlayerOffer)) : T("本轮尚未报价", "No bid this round")), 10f, 42f, 212f, 128f, 18f, AuctionUiStyle.Muted);
			StageText("PlayerHint", T("可自由输入报价金额", "Enter any amount"), 8f, 36f, 238f, 140f, 14f, AuctionUiStyle.Muted);
			for (int i = 0; i < 3; i++)
			{
				int num = 263 + i * 72;
				Dynamic.Add(CreateAccent("SeatDivider" + i, FromTop(36f, num - 8, 140f, 1f), AuctionUiStyle.Border));
				int num2 = ((selected.PendingOffer >= 0 && selected.BiddersShown > i && !selected.Sealed) ? AuctionEngine.PreviewOffers(selected, selected.Round + 1)[i] : 0);
				int num3 = AuctionEngine.ExitReason(selected, i);
				string text = ((num3 != 0) ? AuctionEngine.ExitText(num3) : ((selected.PendingOffer >= 0 && selected.BiddersShown > i) ? (selected.Sealed ? T("暗标已提交", "Sealed bid") : ((num2 > 0) ? (T("报价 ", "Bid ") + Money(num2)) : T("本轮未报价", "No bid"))) : ((selected.Round > 0 && selected.LastOffers[i] > 0) ? (T("报价 ", "Bid ") + Money(selected.LastOffers[i])) : T("等待发言", "Waiting"))));
				GameObject val = NewObject("BidderPortrait" + i, (Transform)(object)_layout, typeof(Image));
				SetRect(val.GetComponent<RectTransform>(), FromTop(36f, num, 22f, 26f));
				Image component = val.GetComponent<Image>();
				component.preserveAspect = true;
				((Graphic)component).raycastTarget = false;
				try
				{
					SpriteDict instance = SpriteDict.Instance;
					component.sprite = ((instance != null) ? instance.GetSprite((new string[3] { "joe", "merchant", "landlord" })[i]) : null);
				}
				catch
				{
				}
				if ((Object)(object)component.sprite == (Object)null)
				{
					((Graphic)component).color = SurfaceRaised;
				}
				Dynamic.Add(val);
				StageText("Seat" + i, AuctionText.BidderName(i), 10f, 64f, num + 4, 112f, 18f, Color.white, bold: true);
				bool flag = selected.PendingOffer >= 0 && selected.BiddersShown == i + 1;
				bool flag2 = !selected.Sealed && selected.HighestBidder == i;
				Dynamic.Add(CreatePanel("BidderSpeech" + i, FromTop(46f, num + 29, 130f, 24f), (flag2 || flag) ? AuctionUiStyle.Gold : AuctionUiStyle.Border));
				StageText("BidderSpeechText" + i, text, 10f, 52f, num + 33, 118f, 17f, (flag2 || flag) ? AuctionUiStyle.Gold : AuctionUiStyle.Muted);
				StageText("BidderMood" + i, AuctionRivals.Mood(selected, i), 8f, 36f, num + 56, 140f, 13f, AuctionUiStyle.Muted);
			}
			Dynamic.Add(CreatePanel("AuctionStage", FromTop(200f, 138f, 490f, 347f), Surface));
			StageText("LotHeading", AuctionCatalog.CategoryName(selected.Category), 13f, 212f, 149f, 266f, 23f, Color.white, bold: true);
			((TMP_Text)StageText("RoundHeading", (selected.Sealed ? T("暗标竞拍", "Sealed") : T("公开竞拍", "Open")) + " · " + T("第 ", "Round ") + (selected.Round + 1) + " / 5", 10f, 482f, 151f, 196f, 20f, AuctionUiStyle.Gold, bold: true)).alignment = (TextAlignmentOptions)260;
			Dynamic.Add(CreateContainerImage("SealedContainer", selected.Category, FromTop(212f, 207f, 112f, 100f)));
			StageText("ExteriorClue", AuctionEngine.SurfaceClue(selected), 12f, 336f, 211f, 342f, 31f, Color.white, bold: true);
			string text2 = (((selected.IntelMask & 2) != 0) ? AuctionText.DisplayLog(AuctionEngine.Intel(selected, ((selected.IntelMask & 0x10) == 0) ? 1 : 4)) : T("估值线索：尚未购买", "Value clue: not purchased"));
			StageText("Clues", text2 + "\n" + MainMix(selected) + T(" · 件数未知\n", " · Count unknown\n") + AuctionEngine.LeadRule(selected), 10f, 336f, 249f, 342f, 60f, AuctionUiStyle.Muted);
			Dynamic.Add(CreateAccent("DialogueRule", FromTop(212f, 321f, 466f, 1f), AuctionUiStyle.Border));
			StageText("DialogueTitle", T("竞拍播报", "Auction Feed"), 11f, 212f, 332f, 250f, 20f, Color.white, bold: true);
			Dynamic.Add(CreateButton("AnnouncementSpeed", _fastAnnouncements ? T("报价速度 ×4", "Speed ×4") : T("报价速度 ×1", "Speed ×1"), FromTop(588f, 330f, 90f, 20f), ToggleAnnouncementSpeed, enabled: true));
			CreateBroadcast(selected);
			AddIntelButton(1, FromTop(212f, 453f, 114f, 26f), T("估值区间", "Value Range"));
			AddIntelButton(2, FromTop(336f, 453f, 114f, 26f), T("四件抽检", "Sample Four"));
			Dynamic.Add(CreateButton("MoreIntel", T("更多情报", "More Intel"), FromTop(460f, 453f, 114f, 26f), RenderIntelMenu, selected.PendingOffer < 0));
			Dynamic.Add(CreatePanel("BidTools", FromTop(702f, 138f, 234f, 347f), Surface));
			StageText("BidTitle", T("本轮自由报价", "Your Bid This Round"), 12f, 714f, 149f, 210f, 22f, AuctionUiStyle.Gold, bold: true);
			Dynamic.Add(CreatePanel("BidDisplay", FromTop(714f, 179f, 210f, 42f), Frame));
			((TMP_Text)StageText("BidValue", (selected.PendingOffer < 0) ? ((_bidInput.Length == 0) ? "0" : _bidInput) : ((selected.PendingOffer > 0) ? selected.PendingOffer.ToString("N0") : "—"), 23f, 722f, 183f, 194f, 38f, Color.white)).alignment = (TextAlignmentOptions)260;
			StageText("MultiplierBase", T("倍率基准 ", "Base ") + Money(AuctionEngine.MultiplierBase(selected)) + T(" · 现金上限 ", " · Cash ") + Money(PlayerStore.Instance.GetCash()), 8f, 714f, 228f, 210f, 23f, AuctionUiStyle.Muted);
			bool flag3 = selected.PendingOffer < 0;
			int[] array = new int[4] { 11, 13, 16, 20 };
			for (int j = 0; j < array.Length; j++)
			{
				int m = array[j];
				Dynamic.Add(CreateButton("BidMultiplier" + m, (m == 20) ? "×2" : ("×1." + (m - 10)), FromTop(714 + j * 54, 259f, 48f, 23f), delegate
				{
					SetBidMultiplier(m);
				}, flag3));
			}
			for (int num4 = 1; num4 <= 9; num4++)
			{
				int d = num4;
				Dynamic.Add(CreateButton("Digit" + d, d.ToString(), FromTop(714 + (d - 1) % 3 * 72, 290 + (d - 1) / 3 * 31, 66f, 27f), delegate
				{
					AddDigit(d);
				}, flag3));
			}
			Dynamic.Add(CreateButton("Clear", T("清空", "Clear"), FromTop(714f, 383f, 66f, 27f), delegate
			{
				_bidInput = string.Empty;
				RenderStage();
			}, flag3));
			Dynamic.Add(CreateButton("Digit0", "0", FromTop(786f, 383f, 66f, 27f), delegate
			{
				AddDigit(0);
			}, flag3));
			Dynamic.Add(CreateButton("Backspace", T("退格", "Del"), FromTop(858f, 383f, 66f, 27f), Backspace, flag3));
			Dynamic.Add(CreateButton("Bid", flag3 ? T("提交本轮报价", "Submit Bid") : T("等待对手报价…", "Rivals are bidding…"), FromTop(714f, 419f, 210f, 27f), Bid, flag3));
			Dynamic.Add(CreateButton("Withdraw", T("放弃本箱竞拍", "Withdraw from This Lot"), FromTop(714f, 453f, 210f, 26f), RenderWithdrawConfirm, flag3));
			Dynamic.Add(CreateAccent("FooterRule", FromTop(24f, 499f, 912f, 1f), AuctionUiStyle.Border));
			Dynamic.Add(CreateButton("OrdersLocked", T("采购委托", "Orders"), FromTop(24f, 511f, 100f, 28f), delegate
			{
			}, enabled: false));
			Dynamic.Add(CreateButton("CollectionLocked", T("藏品收集册", "Collection"), FromTop(136f, 511f, 100f, 28f), delegate
			{
			}, enabled: false));
			Dynamic.Add(CreateButton("HistoryLocked", T("历史账本", "Ledger"), FromTop(248f, 511f, 100f, 28f), delegate
			{
			}, enabled: false));
			Dynamic.Add(CreateButton("Abandon", (selected.PlayerOffer > 0 && selected.PlayerOffer <= PlayerStore.Instance.GetCash()) ? T("保持上轮报价", "Keep Last Bid") : T("本轮不出价", "Pass This Round"), FromTop(366f, 511f, 114f, 28f), Abandon, flag3));
			StageText("FooterHint", T("播报可滚动查看 · 不打断自动报价", "Scroll the feed · Rival bids continue automatically"), 8f, 492f, 519f, 320f, 18f, AuctionUiStyle.Muted);
			Dynamic.Add(CreateButton("Back", T("返回四箱", "Four Lots"), FromTop(824f, 511f, 112f, 28f), delegate
			{
				RenderLots(T("选择一个集装箱。", "Choose a container."));
			}, enabled: true));
			SetStatus((message == null) ? T("报价与成交操作分开，允许自由输入，不提高最低报价。", "Enter any amount, then submit. No rising minimum bid.") : AuctionText.DisplayLog(message));
		}

		private static TextMeshProUGUI StageText(string name, string text, float size, float x, float top, float width, float height, Color color, bool bold = false)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			TextMeshProUGUI val = CreateLabel(name, text, size, new Vector2(x, 0f - top), new Vector2(width, height), (FontStyles)(bold ? 1 : 0));
			((Graphic)val).color = color;
			Dynamic.Add(FindLayoutChild(((TMP_Text)val).transform));
			return val;
		}

		private static void AddIntelButton(int kind, Rect bounds, string caption)
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			AuctionLot selected = _selected;
			bool flag = (selected.IntelMask & (1 << kind)) != 0;
			string text = ((!flag) ? (caption + " " + AuctionNightMarket.IntelText(selected, kind)) : ((kind == 2) ? T("查看抽检", "View Sample") : ((kind == 1) ? T("估值已查询", "Value Purchased") : T("已查询", "Purchased"))));
			Dynamic.Add(CreateButton("Intel" + kind, text, bounds, delegate
			{
				BuyIntel(kind);
			}, selected.PendingOffer < 0 && (!flag || kind == 2) && (kind != 4 || (selected.IntelMask & 2) != 0)));
		}

		private static void RenderIntelMenu()
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			if (_selected != null && _selected.PendingOffer < 0)
			{
				_page = Page.Intel;
				ClearDynamic();
				Dynamic.Add(CreatePanel("IntelMenu", FromTop(220f, 157f, 520f, 300f), Surface));
				StageText("IntelMenuTitle", T("更多情报", "More Intel"), 20f, 244f, 177f, 472f, 32f, Color.white, bold: true);
				string[] array = new string[3]
				{
					T("探查对手心理价", "Estimate Rival Limit"),
					T("识别搅局抬价", "Assess Bluffing"),
					T("重新鉴定估值", "Reappraise Value")
				};
				int[] array2 = new int[3] { 0, 3, 4 };
				for (int i = 0; i < array2.Length; i++)
				{
					AddIntelButton(array2[i], FromTop(244f, 233 + i * 48, 472f, 34f), array[i]);
				}
				StageText("IntelMenuHint", T("情报结果保存在竞拍播报中；复鉴需先查询估值。", "Intel is recorded in the feed. Reappraisal requires Value Range."), 10f, 244f, 382f, 472f, 30f, AuctionUiStyle.Muted);
				Dynamic.Add(CreateButton("IntelMenuBack", T("返回竞拍", "Back to Bidding"), FromTop(244f, 417f, 180f, 28f), delegate
				{
					RenderStage();
				}, enabled: true));
				SetStatus(T("情报费用及使用规则保持不变。", "Existing intel costs and rules apply."));
			}
		}

		private static void Bid()
		{
			AuctionLot selected = _selected;
			if (selected == null || selected.Finished || selected.Lost || SessionComplete)
			{
				return;
			}
			if (!int.TryParse(_bidInput, out var result))
			{
				RenderStage(T("请用数字键盘输入完整报价。", "Enter a complete bid using the keypad."));
				return;
			}
			string text = AuctionEngine.StageBid(selected, result, PlayerStore.Instance.GetCash());
			if (text == "已提交")
			{
				AuctionState.SaveLots(_lots);
				_nextAnnouncementAt = Time.unscaledTime + AnnouncementInterval;
				RenderStage(T("报价已提交，三位竞拍者将依次报价。", "Bid submitted. Rivals will bid in turn."));
			}
			else
			{
				RenderStage(text);
			}
		}

		private static void AdvanceAuction()
		{
			AuctionLot selected = _selected;
			if (selected == null || selected.PendingOffer < 0)
			{
				return;
			}
			if (selected.BiddersShown < 3)
			{
				string message = AuctionEngine.RevealBidder(selected);
				AuctionState.SaveLots(_lots);
				RenderStage(message);
				return;
			}
			string text = AuctionEngine.FinishStagedRound(selected);
			_nextAnnouncementAt = 0f;
			switch (text)
			{
			case "成交":
				CompletePurchase(selected);
				break;
			case "落标":
				AuctionState.SaveLots(_lots);
				RenderSettlement(selected, won: false);
				break;
			case "继续":
				AuctionState.SaveLots(_lots);
				_bidInput = string.Empty;
				RenderStage(T("本轮已揭价；未达到提前成交条件，继续下一轮。", "Bids revealed. No early winner; next round."));
				break;
			default:
				RenderStage(text);
				break;
			}
		}

		private static void CompletePurchase(AuctionLot lot)
		{
			if (lot == null || !lot.PlayerLeading || lot.Lost)
			{
				return;
			}
			try
			{
				if (PlayerStore.Instance.GetCash() < lot.CurrentBid)
				{
					throw new InvalidOperationException("现金不足");
				}
				List<string> list = ((lot.Contents.Count > 0) ? new List<string>(lot.Contents) : AuctionCatalog.GenerateContents(lot));
				if (list.Count == 0)
				{
					throw new InvalidOperationException("该类别没有可用物品池");
				}
				lot.EstimatedValue = AuctionCatalog.ContentsValue(list);
				AuctionState.CommitPurchase(lot, _lots, list);
				Core.Log?.Msg("Auction won: category=" + lot.Category.ToString() + ", price=" + lot.CurrentBid + ", items=" + list.Count + ".");
			}
			catch (Exception ex)
			{
				lot.Finished = false;
				RenderStage(T("无法完成成交：", "Purchase failed: ") + AuctionText.DisplayLog(ex.Message));
				Core.Log?.Warning("Auction purchase failed: " + ex.Message);
				return;
			}
			RenderSettlement(lot, won: true);
		}

		private static void Abandon()
		{
			if (_selected != null && _selected.PendingOffer < 0)
			{
				string text = AuctionEngine.StagePass(_selected, PlayerStore.Instance.GetCash());
				if (text == "已提交")
				{
					AuctionState.SaveLots(_lots);
				}
				if (text == "已提交")
				{
					_nextAnnouncementAt = Time.unscaledTime + AnnouncementInterval;
				}
				RenderStage((text == "已提交") ? T("已保持原价或跳过本轮，等待对手依次报价。", "Bid kept or passed. Waiting for rivals.") : text);
			}
		}

		private static void RenderWithdrawConfirm()
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			if (_selected != null && _selected.PendingOffer < 0 && !_selected.Finished && !_selected.Lost)
			{
				_page = Page.Withdraw;
				ClearDynamic();
				_stageVisible = false;
				Dynamic.Add(CreatePanel("WithdrawPanel", new Rect(174f, 150f, 612f, 245f), AuctionCatalog.CategoryColor(_selected.Category)));
				AddStageLabel("WithdrawTitle", T("放弃本箱竞拍？", "Withdraw from this lot?"), 23f, 214f, -166f, 530f, 35f, (FontStyles)1);
				AddStageLabel("WithdrawDetails", T("确认后不可继续竞拍这只集装箱，也不会扣除箱款。\nAI会继续竞价，并直接显示最终结果。\n已支付的入场费和情报费不退；本箱计入今晚的三场竞拍。", "You cannot rejoin this lot. No container payment is taken.\nAI bidding continues; the final result appears immediately.\nEntry and intel fees are not refunded; this counts as one auction."), 15f, 214f, -220f, 530f, 88f, (FontStyles)0);
				Dynamic.Add(CreateButton("ConfirmWithdraw", T("确认放弃", "Confirm Withdraw"), new Rect(250f, 181f, 190f, 39f), Withdraw, enabled: true));
				Dynamic.Add(CreateButton("CancelWithdraw", T("继续竞拍", "Continue"), new Rect(520f, 181f, 190f, 39f), delegate
				{
					RenderStage();
				}, enabled: true));
				SetStatus(T("放弃本箱竞拍需要确认；尚未提交新一轮报价。", "Confirm withdrawal before submitting the next bid."));
			}
		}

		private static void Withdraw()
		{
			AuctionLot selected = _selected;
			if (!AuctionEngine.Withdraw(selected))
			{
				RenderStage(T("当前不能退出竞拍，请等待本轮揭价。", "Wait for this round's bids before withdrawing."));
				return;
			}
			_nextAnnouncementAt = 0f;
			AuctionState.SaveLots(_lots);
			RenderSettlement(selected, won: false);
		}

		private static void AddDigit(int digit)
		{
			string text = _bidInput + digit;
			if (text.Length <= 9 && int.TryParse(text, out var _))
			{
				_bidInput = text.TrimStart('0');
				if (_bidInput.Length == 0)
				{
					_bidInput = "0";
				}
				RenderStage();
			}
		}

		private static void Backspace()
		{
			if (_bidInput.Length > 0)
			{
				_bidInput = _bidInput.Substring(0, _bidInput.Length - 1);
			}
			RenderStage();
		}

		private static void SetBidMultiplier(int tenths)
		{
			if (_selected != null && _selected.PendingOffer < 0 && !_selected.Finished && !_selected.Lost)
			{
				_bidInput = AuctionEngine.MultipliedBid(_selected, tenths).ToString();
				RenderStage(T("已填入倍率报价，请确认金额后提交。", "Multiplier bid filled. Check the amount, then submit."));
			}
		}

		private static void ToggleAnnouncementSpeed()
		{
			_fastAnnouncements = !_fastAnnouncements;
			AuctionLot selected = _selected;
			if (selected != null && selected.PendingOffer >= 0)
			{
				_nextAnnouncementAt = Time.unscaledTime + AnnouncementInterval;
			}
			RenderStage();
		}

		private static void BuyIntel(int kind)
		{
			AuctionLot selected = _selected;
			if (selected == null || selected.PendingOffer >= 0)
			{
				return;
			}
			if ((selected.IntelMask & (1 << kind)) != 0)
			{
				if (kind == 2)
				{
					RenderInspection();
				}
			}
			else
			{
				if (kind == 4 && (selected.IntelMask & 2) == 0)
				{
					return;
				}
				int num = AuctionEngine.IntelCost(selected, kind);
				if (PlayerStore.Instance.GetCash() < num)
				{
					RenderStage(T("情报购买失败：现金不足。", "Not enough cash to buy intel."));
					return;
				}
				string[] array = ((kind == 2) ? AuctionEngine.SampleContents(selected) : Array.Empty<string>());
				if (kind == 2 && array.Length == 0)
				{
					RenderStage(T("暂无可抽检货物，未扣费。", "No cargo available to sample. No fee charged."));
					return;
				}
				PlayerStore.Instance.ModCash(-num, true);
				if (kind == 2)
				{
					selected.InspectionItems.AddRange(array);
				}
				selected.IntelMask |= 1 << kind;
				selected.IntelSpent += num;
				string text = AuctionEngine.Intel(selected, kind);
				selected.BidLog.Add("情报 · " + text);
				AuctionState.SaveLots(_lots);
				if (kind == 2)
				{
					RenderInspection();
				}
				else
				{
					RenderStage(text);
				}
			}
		}

		private static void RenderInspection()
		{
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			if (_selected == null || _selected.InspectionItems.Count == 0)
			{
				RenderStage();
				return;
			}
			if (_selected.InspectionItems.Count < Math.Min(4, _selected.Contents.Count))
			{
				string[] collection = AuctionEngine.SampleContents(_selected);
				_selected.InspectionItems.Clear();
				_selected.InspectionItems.AddRange(collection);
				AuctionState.SaveLots(_lots);
			}
			_page = Page.Inspection;
			ClearDynamic();
			Color color = AuctionCatalog.CategoryColor(_selected.Category);
			Dynamic.Add(CreatePanel("InspectionPanel", new Rect(154f, 100f, 652f, 326f), color));
			Dynamic.Add(CreateAccent("InspectionAccent", new Rect(157f, 389f, 646f, 34f), color));
			AddStageLabel("InspectionTitle", T("抽检货物  ·  真实样本", "Cargo Sample  ·  Actual Contents"), 19f, 174f, -123f, 608f, 30f, (FontStyles)1);
			for (int i = 0; i < _selected.InspectionItems.Count; i++)
			{
				int num = 174 + i * 155;
				Dynamic.Add(CreatePanel("InspectionItem" + i, new Rect((float)num, 161f, 143f, 204f), color));
				GameItem val = DirectoryMaster.Item(_selected.InspectionItems[i], false);
				GameObject val2 = NewObject("InspectionSprite" + i, (Transform)(object)_layout, typeof(Image));
				SetRect(val2.GetComponent<RectTransform>(), new Rect((float)(num + 29), 236f, 86f, 86f));
				Image component = val2.GetComponent<Image>();
				component.sprite = PreviewSprite(val);
				component.preserveAspect = true;
				((Graphic)component).raycastTarget = false;
				Dynamic.Add(val2);
				TextMeshProUGUI val3 = CreateLabel("InspectionName" + i, val.name, 13f, new Vector2((float)(num + 8), -183f), new Vector2(127f, 32f), (FontStyles)1);
				Dynamic.Add(((Component)val3).gameObject);
				((TMP_Text)val3).enableAutoSizing = true;
				((TMP_Text)val3).fontSizeMin = 9f;
				((TMP_Text)val3).fontSizeMax = 13f;
				AddStageLabel("InspectionValue" + i, T("参考价值  ", "Value  ") + AuctionCatalog.ItemValue(_selected.InspectionItems[i]) + "¢", 12f, num + 8, -310f, 127f, 24f, (FontStyles)1);
				List<string> list = new List<string>();
				foreach (string itemType in val.itemTypes)
				{
					list.Add(TypeHelper.GetTypeDisplayName(itemType));
				}
				if (val.IsTag("CONTRABAND"))
				{
					TagState tagReadonly = val.GetTagReadonly("CONTRABAND_LEVEL");
					string text = ((tagReadonly != null) ? tagReadonly.GetString() : null);
					list.Add((text == "CONTRABAND_LEVEL_HIGH") ? T("高级违禁品", "High Contraband") : ((text == "CONTRABAND_LEVEL_MID") ? T("中级违禁品", "Mid Contraband") : T("违禁品", "Contraband")));
				}
				TextMeshProUGUI val4 = CreateLabel("InspectionTypes" + i, T("标签  ", "Tags  ") + ((list.Count > 0) ? string.Join(" / ", list.Distinct()) : T("普通物品", "General Item")), 9f, new Vector2((float)(num + 8), -334f), new Vector2(127f, 26f), (FontStyles)0);
				((TMP_Text)val4).enableAutoSizing = true;
				((TMP_Text)val4).fontSizeMin = 7f;
				((TMP_Text)val4).fontSizeMax = 9f;
				Dynamic.Add(((Component)val4).gameObject);
				AddStageLabel("InspectionShape" + i, T("占格  ", "Footprint  ") + val.shape.width + "×" + val.shape.height, 10f, num + 8, -361f, 127f, 18f, (FontStyles)0);
			}
			AddStageLabel("InspectionHint", T("仅查看样本，不会取走物品；其余货物继续封存。", "Preview only. Nothing is taken; the remaining cargo stays sealed."), 10f, 174f, -383f, 608f, 18f, (FontStyles)0);
			Dynamic.Add(CreateButton("InspectionBack", T("返回竞拍", "Back to Bidding"), new Rect(384f, 108f, 192f, 28f), delegate
			{
				RenderStage();
			}, enabled: true));
			SetStatus(T("抽检结果已保存，再次查看不收费。", "Sample saved. Viewing it again is free."));
		}

		private static void RenderSettlement(AuctionLot lot, bool won)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_0515: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			_page = Page.Settlement;
			_settlementLot = lot;
			_settlementWon = won;
			ClearDynamic();
			_stageVisible = false;
			_nextAnnouncementAt = 0f;
			Color color = (won ? new Color(0.33f, 0.69f, 0.49f) : new Color(0.66f, 0.37f, 0.4f));
			Dynamic.Add(CreatePanel("SettlementPanel", new Rect(174f, 112f, 612f, 310f), won ? new Color(0.33f, 0.69f, 0.49f) : new Color(0.66f, 0.37f, 0.4f)));
			Dynamic.Add(CreateAccent("SettlementAccent", new Rect(177f, 389f, 606f, 30f), color));
			string text = (lot.Withdrawn ? T("已放弃竞拍", "Withdrawn") : ((won && lot.Quality == AuctionQuality.Hidden) ? T("异常货柜成交", "Exceptional Lot Won") : (won ? T("竞拍成交", "Auction Won") : T("竞拍落标", "Outbid"))));
			AddStageLabel("SettlementTitle", text, 23f, 214f, -132f, 532f, 36f, (FontStyles)1);
			Dynamic.Add(CreateContainerImage("SettlementCrate", lot.Category, new Rect(210f, 266f, 190f, 110f)));
			AddStageLabel("SettlementStamp", lot.Withdrawn ? T("已退出", "OUT") : (won ? T("成  交", "SOLD") : T("落  标", "LOST")), 16f, 268f, -251f, 90f, 26f, (FontStyles)1);
			AddStageLabel("SettlementRivalMoods", string.Join("\n", from i in Enumerable.Range(0, 3)
				select AuctionText.BidderName(i) + " · " + AuctionRivals.Mood(lot, i) + " · " + AuctionState.RivalCash(lot, i) + "¢"), 9f, 214f, -283f, 192f, 52f, (FontStyles)0);
			int num = AuctionState.VenueFee(lot.Venue);
			string text2 = ((lot.HighestBidder < 0) ? T("无人出价，本箱流拍", "No bids. Lot unsold") : (T("得标者  ", "Winner  ") + AuctionText.BidderName(lot.HighestBidder) + T("\n最终成交价  ", "\nWinning bid  ") + lot.CurrentBid + " ¢"));
			string text3 = (won ? (T("获得  ", "Received  ") + AuctionCatalog.CategoryName(lot.Category) + " ×1\n" + T("内部物品  ", "Items inside  ") + lot.Contents.Count + T(" 件\n", "\n") + T("第 ", "Round ") + lot.Round + T(" 轮成交  ", " price  ") + lot.CurrentBid + " ¢\n" + T("当晚入场 / 本箱情报  ", "Entry / intel  ") + num + " / " + lot.IntelSpent + " ¢\n" + T("总货值与盈亏  开箱后揭晓", "Value and profit revealed after opening")) : (lot.Withdrawn ? (T("AI已完成剩余竞价\n最终轮次  ", "AI bidding completed\nFinal round  ") + lot.Round + "\n" + text2 + T("\n箱款未扣除\n入场费与情报费不退", "\nNo container payment\nEntry and intel fees are not refunded")) : (T("最终轮次  ", "Final round  ") + lot.Round + "\n" + text2 + T("\n未扣除箱款", "\nNo container payment"))));
			AddStageLabel("SettlementSummary", text3, 14f, 430f, -174f, 318f, 160f, (FontStyles)1);
			Dynamic.Add(CreatePanel("SettlementDialogue", new Rect(210f, 132f, 540f, 68f), color));
			AddStageLabel("SettlementLog", T("竞拍播报\n", "Auction Feed\n") + RecentLog(lot), 11f, 222f, -346f, 516f, 51f, (FontStyles)0);
			Dynamic.Add(CreateButton("Continue", SessionComplete ? T("查看本晚结果", "Tonight's Results") : T("继续下一场", "Next Auction"), new Rect(390f, 66f, 180f, 38f), delegate
			{
				RenderLots(SessionComplete ? T("今晚三场竞拍已经结束。", "All three auctions are complete.") : T("选择下一只集装箱。", "Choose the next container."));
			}, enabled: true));
			SetStatus((lot.Withdrawn ? T("你已退出；AI竞价结束，结果已公布。", "You withdrew. AI bidding is complete; the result is shown.") : (won ? T("本箱已登记；回店取货，全部开出后揭晓盈亏。", "Lot registered. Collect it at the shop; profit appears after opening.") : T("本箱由其他竞拍者获得。", "Another bidder won this lot."))) + T("  本晚进度 ", "  Tonight ") + ResolvedAuctions + " / 3.");
		}

		private static void RenderHistory()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			_page = Page.History;
			ClearDynamic();
			Dynamic.Add(CreatePanel("HistoryPanel", FromTop(24f, 138f, 912f, 347f), Surface));
			AddStageLabel("HistoryTitle", T("历史账本", "Auction Ledger"), 22f, 42f, -149f, 800f, 35f, (FontStyles)1);
			int[] array = new int[6] { 42, 178, 370, 480, 590, 721 };
			int[] array2 = new int[6] { 128, 184, 102, 102, 123, 193 };
			string[] array3 = ((!AuctionText.IsChinese(AuctionText.CurrentLocaleCode)) ? new string[6] { "Day / Venue", "Container", "Price", "Cargo Value", "Net Profit", "Status" } : new string[6] { "日期 / 场次", "集装箱", "成交价", "实际货值", "净收益", "状态" });
			Dynamic.Add(CreateAccent("HistoryRule", FromTop(42f, 210f, 876f, 1f), AuctionUiStyle.Border));
			for (int i = 0; i < 6; i++)
			{
				AddStageLabel("HistoryColumn" + i, array3[i], 12f, array[i], -187f, array2[i], 20f, (FontStyles)1);
			}
			List<(string[], long, bool)> list = new List<(string[], long, bool)>();
			foreach (string item in AuctionState.Read("history").Split(';').AsEnumerable()
				.Reverse())
			{
				string[] array4 = item.Split(',');
				int result3;
				int result4;
				if (array4.Length == 6 && int.TryParse(array4[1], out var result) && result >= 0 && result <= 3)
				{
					long.TryParse(array4[5], out var result2);
					list.Add((new string[6]
					{
						T("第 ", "Day ") + array4[0],
						AuctionCatalog.CategoryName((ContainerCategory)result),
						array4[3],
						array4[4],
						array4[5],
						T("已结算", "Settled")
					}, result2, true));
				}
				else if ((array4.Length == 9 || array4.Length == 10) && int.TryParse(array4[1], out result3) && result3 >= 0 && result3 <= 2 && int.TryParse(array4[2], out result4) && result4 >= 0 && result4 <= 3)
				{
					bool flag = array4.Length == 9 || array4[9] == "1";
					long.TryParse(array4[6], out var result5);
					list.Add((new string[6]
					{
						array4[0] + " · " + AuctionRules.VenueName((AuctionVenue)result3),
						AuctionCatalog.CategoryName((ContainerCategory)result4),
						array4[4],
						flag ? array4[5] : T("待揭晓", "Pending"),
						flag ? array4[6] : "—",
						flag ? T("已结算", "Settled") : T("尚未开完", "Unopened cargo")
					}, result5, flag));
				}
				if (list.Count == 8)
				{
					break;
				}
			}
			if (list.Count == 0)
			{
				AddStageLabel("HistoryEmpty", T("暂无成交记录。", "No auction records yet."), 15f, 42f, -234f, 876f, 30f, (FontStyles)0);
			}
			for (int j = 0; j < list.Count; j++)
			{
				int num = 222 + j * 28;
				for (int k = 0; k < 6; k++)
				{
					TextMeshProUGUI val = CreateLabel("HistoryCell" + j + "_" + k, list[j].Item1[k], 12f, new Vector2((float)array[k], (float)(-num)), new Vector2((float)array2[k], 23f), (FontStyles)0);
					((TMP_Text)val).enableAutoSizing = true;
					((TMP_Text)val).fontSizeMin = 8f;
					((TMP_Text)val).fontSizeMax = 12f;
					((TMP_Text)val).enableWordWrapping = false;
					((TMP_Text)val).overflowMode = (TextOverflowModes)1;
					if ((k == 3 || k == 5) && !list[j].Item3)
					{
						((Graphic)val).color = AuctionUiStyle.Gold;
					}
					if (k == 4 && list[j].Item3)
					{
						((Graphic)val).color = ((list[j].Item2 < 0) ? new Color(0.82f, 0.58f, 0.55f) : new Color(0.61f, 0.78f, 0.69f));
					}
					Dynamic.Add(FindLayoutChild(((TMP_Text)val).transform));
				}
			}
			Dynamic.Add(CreateButton("HistoryBack", AuctionState.TryGetVenue(out var _) ? T("返回四箱", "Four Lots") : T("返回场次选择", "Choose Venue"), FromTop(716f, 511f, 220f, 28f), ReturnLobby, enabled: true));
			SetStatus(T("最近八次成交；货值与净收益在全部开箱后揭晓，费用口径保持原规则。", "Last eight purchases. Value and net profit reveal after opening; existing fee rules apply."));
		}

		private static void AddStageLabel(string name, string text, float size, float x, float y, float width, float height, FontStyles style = (FontStyles)0)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			TextMeshProUGUI val = CreateLabel(name, text, size, new Vector2(x, y), new Vector2(width, height), style);
			Dynamic.Add(FindLayoutChild(((TMP_Text)val).transform));
		}

		private static string RecentLog(AuctionLot lot)
		{
			int count = lot.BidLog.Count;
			if (count != 0)
			{
				return string.Join("\n", lot.BidLog.GetRange(Math.Max(0, count - 2), Math.Min(2, count)).Select(AuctionText.DisplayLog));
			}
			return T("等待报价", "Waiting for bids");
		}

		private static string BidDialogue(AuctionLot lot)
		{
			if (lot.BidLog.Count != 0)
			{
				return string.Join("\n", lot.BidLog.Select(AuctionText.DisplayLog));
			}
			return AuctionText.DisplayLog("拍卖师：最多五轮，每轮揭价；领先足够可提前成交。");
		}

		private static GameObject CreateNativeSpeechBubble(string name, Rect bounds, bool speaking)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = NewObject(name, (Transform)(object)_layout, typeof(Image));
			SetRect(val.GetComponent<RectTransform>(), bounds);
			Image component = val.GetComponent<Image>();
			try
			{
				Image val2 = DialogUIManager.Instance?.clientBubble?.backgroundImage;
				if ((Object)(object)val2 != (Object)null && (Object)(object)val2.sprite != (Object)null)
				{
					component.sprite = val2.sprite;
					component.type = val2.type;
					((Graphic)component).color = (Color)(speaking ? Color.white : new Color(0.78f, 0.78f, 0.78f, 1f));
				}
				else
				{
					((Graphic)component).color = (speaking ? SurfaceRaised : Frame);
				}
			}
			catch
			{
				((Graphic)component).color = (speaking ? SurfaceRaised : Frame);
			}
			((Graphic)component).raycastTarget = false;
			return val;
		}

		private static void CreateBroadcast(AuctionLot lot)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = NewObject("BroadcastScroll", (Transform)(object)_layout, typeof(ScrollRect));
			SetRect(val.GetComponent<RectTransform>(), FromTop(212f, 358f, 466f, 85f));
			Dynamic.Add(val);
			GameObject val2 = NewObject("BroadcastViewport", val.transform, typeof(Image), typeof(RectMask2D));
			RectTransform component = val2.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.offsetMin = Vector2.zero;
			component.offsetMax = new Vector2(-14f, 0f);
			Image component2 = val2.GetComponent<Image>();
			((Graphic)component2).color = new Color(0f, 0f, 0f, 0f);
			((Graphic)component2).raycastTarget = true;
			RectTransform component3 = NewObject("BroadcastContent", val2.transform).GetComponent<RectTransform>();
			Vector2 val3 = default(Vector2);
			((Vector2)(ref val3))..ctor(0f, 1f);
			component3.anchorMax = val3;
			component3.anchorMin = val3;
			component3.pivot = new Vector2(0f, 1f);
			component3.anchoredPosition = Vector2.zero;
			component3.sizeDelta = new Vector2(448f, 85f);
			TextMeshProUGUI val4 = CreateLabel("DialogueText", BidDialogue(lot), 9f, Vector2.zero, new Vector2(444f, 85f), (FontStyles)0);
			GameObject obj = FindLayoutChild(((TMP_Text)val4).transform);
			obj.transform.SetParent((Transform)(object)component3, false);
			float num = Mathf.Max(85f, ((TMP_Text)val4).preferredHeight + 8f);
			component3.sizeDelta = new Vector2(448f, num);
			obj.GetComponent<RectTransform>().sizeDelta = new Vector2(444f, num);
			GameObject val5 = NewObject("BroadcastScrollbar", val.transform, typeof(Image), typeof(Scrollbar));
			RectTransform component4 = val5.GetComponent<RectTransform>();
			component4.anchorMin = new Vector2(1f, 0f);
			component4.anchorMax = new Vector2(1f, 1f);
			component4.pivot = new Vector2(1f, 0.5f);
			component4.anchoredPosition = Vector2.zero;
			component4.sizeDelta = new Vector2(10f, 0f);
			((Graphic)val5.GetComponent<Image>()).color = new Color(0.08f, 0.09f, 0.1f, 1f);
			GameObject val6 = NewObject("BroadcastHandle", val5.transform, typeof(Image));
			Stretch(val6.GetComponent<RectTransform>());
			Image component5 = val6.GetComponent<Image>();
			((Graphic)component5).color = new Color(0.65f, 0.63f, 0.52f, 1f);
			Scrollbar component6 = val5.GetComponent<Scrollbar>();
			component6.handleRect = val6.GetComponent<RectTransform>();
			((Selectable)component6).targetGraphic = (Graphic)(object)component5;
			component6.direction = (Direction)2;
			_broadcastScroll = val.GetComponent<ScrollRect>();
			_broadcastScroll.viewport = component;
			_broadcastScroll.content = component3;
			_broadcastScroll.horizontal = false;
			_broadcastScroll.vertical = true;
			_broadcastScroll.movementType = (MovementType)2;
			_broadcastScroll.verticalScrollbar = component6;
			Canvas.ForceUpdateCanvases();
			_broadcastScroll.verticalNormalizedPosition = _broadcastPosition;
		}

		private static string LeaderName(AuctionLot lot)
		{
			if (lot.HighestBidder != 3)
			{
				if (lot.HighestBidder < 0 || lot.HighestBidder >= AuctionEngine.Names.Length)
				{
					return T("暂无", "None");
				}
				return AuctionText.BidderName(lot.HighestBidder);
			}
			return T("你", "You");
		}

		private static string ClueRange(AuctionLot lot)
		{
			(int, int, bool) tuple = AuctionEngine.ValueRange(lot, 0);
			return tuple.Item1 + " - " + tuple.Item2;
		}

		private static string Risk(AuctionLot lot)
		{
			if (lot.Category != ContainerCategory.Contraband)
			{
				if (lot.Category != ContainerCategory.Medical)
				{
					return T("中等", "Medium");
				}
				return T("较低", "Low");
			}
			return T("较高", "High");
		}

		private static void TryDeliverPending()
		{
			if (!Core.StoreReady || (Object)(object)EmporiumEntry.Instance == (Object)null)
			{
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				if (!AuctionState.TryGetPendingDelivery(out var category, out var seed, out var contents))
				{
					break;
				}
				if (PlayerStore.Instance.FindAllItem(true).Any(delegate(GameItem val2)
				{
					if (((val2 != null) ? val2.identifier : null) == "xiaowo_auction_container")
					{
						TagState tagReadonly = val2.GetTagReadonly("XIAOWO_AUCTION_CONTAINER_DATA");
						return ((tagReadonly == null) ? null : tagReadonly.GetString()?.Split('~').ElementAtOrDefault(1)) == seed.ToString();
					}
					return false;
				}))
				{
					AuctionState.AcknowledgeDelivery(seed);
					continue;
				}
				GameItem val = null;
				try
				{
					val = AuctionContainer.Create(owned: true, category, seed, contents);
					EmporiumEntry.Instance.TryAddToPlayerInv(val);
					if (val.parentInventory == null && (val.parents == null || val.parents.Count == 0))
					{
						GeneralHelper.SetItemOwned(val, false);
						Core.Log?.Warning("Auction delivery is waiting for 2x2 inventory space.");
						break;
					}
					AuctionState.AcknowledgeDelivery(seed);
					Core.Log?.Msg("Auction container delivered at the store: category=" + category.ToString() + ", items=" + contents.Count + ".");
				}
				catch (Exception ex)
				{
					if (val != null)
					{
						GeneralHelper.SetItemOwned(val, false);
					}
					Core.Log?.Warning("Auction delivery deferred: " + ex.Message);
					break;
				}
			}
		}

		private static string Grade(int value)
		{
			if (value < 8000)
			{
				if (value < 3500)
				{
					if (value < 1500)
					{
						return "C";
					}
					return "B";
				}
				return "A";
			}
			return "S";
		}

		private static void PositionLayout()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)_layout == (Object)null) && !((Object)(object)_root == (Object)null))
			{
				Rect rect = _root.GetComponent<RectTransform>().rect;
				float num = Mathf.Min(((Rect)(ref rect)).width / 960f, ((Rect)(ref rect)).height / 540f);
				if (_phoneCollection)
				{
					num *= 0.86f;
				}
				((Transform)_layout).localScale = new Vector3(num, num, 1f);
			}
		}

		private static GameObject CreatePanel(string name, Rect bounds, Color color)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = NewObject(name, (Transform)(object)_layout, typeof(Image));
			SetRect(val.GetComponent<RectTransform>(), bounds);
			Image component = val.GetComponent<Image>();
			((Graphic)component).color = (name.StartsWith("BidderSpeech") ? color : AuctionUiStyle.Border);
			((Graphic)component).raycastTarget = false;
			GameObject obj = NewObject(name + "Inner", val.transform, typeof(Image));
			RectTransform component2 = obj.GetComponent<RectTransform>();
			component2.anchorMin = Vector2.zero;
			component2.anchorMax = Vector2.one;
			component2.offsetMin = new Vector2(2f, 2f);
			component2.offsetMax = new Vector2(-2f, -2f);
			Image component3 = obj.GetComponent<Image>();
			((Graphic)component3).color = new Color(Surface.r, Surface.g, Surface.b, 0.985f);
			((Graphic)component3).raycastTarget = false;
			return val;
		}

		private static GameObject CreateAccent(string name, Rect bounds, Color color)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			if (((Rect)(ref bounds)).height > 10f && ((Rect)(ref bounds)).width > ((Rect)(ref bounds)).height)
			{
				((Rect)(ref bounds))..ctor(((Rect)(ref bounds)).x, ((Rect)(ref bounds)).yMax - 2f, ((Rect)(ref bounds)).width, 2f);
			}
			GameObject obj = NewObject(name, (Transform)(object)_layout, typeof(Image));
			SetRect(obj.GetComponent<RectTransform>(), bounds);
			Image component = obj.GetComponent<Image>();
			((Graphic)component).color = new Color(color.r * 0.72f, color.g * 0.72f, color.b * 0.72f, 0.98f);
			((Graphic)component).raycastTarget = false;
			return obj;
		}

		private static GameObject CreateContainerImage(string name, ContainerCategory category, Rect bounds)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = NewObject(name, (Transform)(object)_layout, typeof(Image));
			SetRect(val.GetComponent<RectTransform>(), bounds);
			Image component = val.GetComponent<Image>();
			((Graphic)component).color = Color.white;
			component.preserveAspect = true;
			((Graphic)component).raycastTarget = false;
			try
			{
				component.sprite = AuctionContainerSprites.Get(category);
			}
			catch
			{
			}
			if ((Object)(object)component.sprite == (Object)null)
			{
				((Graphic)component).color = SurfaceRaised;
			}
			return val;
		}

		private static TextMeshProUGUI CreateLabel(string name, string text, float size, Vector2 topLeft, Vector2 dimensions, FontStyles style)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = Object.Instantiate<GameObject>(CustomUIManager.Instance?.labelPrefab ?? throw new InvalidOperationException("Native label prefab unavailable."), (Transform)(object)_layout, false);
			((Object)val).name = name;
			RectTransform component = val.GetComponent<RectTransform>();
			Vector2 val2 = default(Vector2);
			((Vector2)(ref val2))..ctor(0f, 1f);
			component.anchorMax = val2;
			component.anchorMin = val2;
			component.pivot = new Vector2(0f, 1f);
			component.anchoredPosition = topLeft;
			component.sizeDelta = dimensions;
			TextMeshProUGUI componentInChildren = val.GetComponentInChildren<TextMeshProUGUI>(true);
			((TMP_Text)componentInChildren).text = text;
			((TMP_Text)componentInChildren).fontSize = size;
			((TMP_Text)componentInChildren).fontStyle = style;
			((Graphic)componentInChildren).color = Color.white;
			((TMP_Text)componentInChildren).alignment = (TextAlignmentOptions)257;
			((TMP_Text)componentInChildren).enableWordWrapping = true;
			((Graphic)componentInChildren).raycastTarget = false;
			val.SetActive(true);
			return componentInChildren;
		}

		private static TextMeshProUGUI CreateChildLabel(Transform parent, string text, float size, FontStyles style)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = Object.Instantiate<GameObject>(CustomUIManager.Instance?.labelPrefab ?? throw new InvalidOperationException("Native label prefab unavailable."), parent, false);
			Stretch(val.GetComponent<RectTransform>());
			TextMeshProUGUI componentInChildren = val.GetComponentInChildren<TextMeshProUGUI>(true);
			((TMP_Text)componentInChildren).text = text;
			((TMP_Text)componentInChildren).fontSize = size;
			((TMP_Text)componentInChildren).fontStyle = style;
			((Graphic)componentInChildren).color = Color.white;
			((TMP_Text)componentInChildren).enableAutoSizing = true;
			((TMP_Text)componentInChildren).fontSizeMin = 7f;
			((TMP_Text)componentInChildren).fontSizeMax = size;
			((Graphic)componentInChildren).raycastTarget = false;
			val.SetActive(true);
			return componentInChildren;
		}

		private static void SetStatus(string text)
		{
			if ((Object)(object)_status != (Object)null)
			{
				((TMP_Text)_status).text = text;
			}
			UpdateAuctionHeader();
			if ((Object)(object)_eventSummary != (Object)null)
			{
				((TMP_Text)_eventSummary).text = MarketCaption();
			}
		}

		private static void ClearDynamic()
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			_stageVisible = false;
			if ((Object)(object)_broadcastScroll != (Object)null)
			{
				float verticalNormalizedPosition = _broadcastScroll.verticalNormalizedPosition;
				Rect rect = _broadcastScroll.content.rect;
				float height = ((Rect)(ref rect)).height;
				rect = _broadcastScroll.viewport.rect;
				_broadcastPosition = ((height <= ((Rect)(ref rect)).height + 1f || verticalNormalizedPosition < 0.04f) ? 0f : verticalNormalizedPosition);
				_broadcastScroll = null;
			}
			foreach (GameObject item in Dynamic)
			{
				if ((Object)(object)item != (Object)null)
				{
					Object.Destroy((Object)(object)item);
				}
			}
			Dynamic.Clear();
		}

		private static GameObject FindLayoutChild(Transform transform)
		{
			if ((Object)(object)transform == (Object)null)
			{
				return null;
			}
			while ((Object)(object)transform.parent != (Object)null && (Object)(object)transform.parent != (Object)(object)_layout)
			{
				transform = transform.parent;
			}
			return ((Component)transform).gameObject;
		}

		internal static void TestEnterVenue(AuctionVenue venue)
		{
			EnterVenue(venue);
		}

		internal static void TestSelect(int index)
		{
			SelectLot(_lots[index]);
		}

		internal static void TestBid()
		{
			_bidInput = AuctionEngine.SuggestedBid(_selected).ToString();
			Bid();
		}

		internal static void TestBidAmount(int amount)
		{
			_bidInput = amount.ToString();
			Bid();
		}

		internal static void TestPass()
		{
			Abandon();
		}

		internal static void TestAdvance()
		{
			AdvanceAuction();
		}

		internal static void TestTickAnnouncement()
		{
			_nextAnnouncementAt = 0f;
			_tick = 0;
			Tick();
		}

		internal static void TestBuyIntel(int kind)
		{
			BuyIntel(kind);
		}

		private static void SetButton(GameObject obj, string text, Action action)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected O, but got Unknown
			Button val = obj.GetComponent<Button>() ?? obj.GetComponentInChildren<Button>(true);
			SetButtonText(obj, text);
			if (!((Object)(object)val == (Object)null))
			{
				val.onClick = new ButtonClickedEvent();
				((UnityEvent)val.onClick).AddListener(new UnityAction(action.Invoke));
			}
		}

		private static void SetButtonText(GameObject obj, string text)
		{
			TextMeshProUGUI[] componentsInChildren = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				((TMP_Text)componentsInChildren[i]).text = text;
			}
		}

		private static GameObject NewObject(string name, Transform parent, params Type[] components)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			Type[] array = new Type[components.Length + 1];
			array[0] = typeof(RectTransform);
			for (int i = 0; i < components.Length; i++)
			{
				array[i + 1] = components[i];
			}
			GameObject val = new GameObject(name, array);
			val.transform.SetParent(parent, false);
			return val;
		}

		private static void SetRect(RectTransform rect, Rect bounds)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			Vector2 anchorMin = (rect.anchorMax = Vector2.zero);
			rect.anchorMin = anchorMin;
			rect.pivot = Vector2.zero;
			rect.anchoredPosition = ((Rect)(ref bounds)).position;
			rect.sizeDelta = ((Rect)(ref bounds)).size;
			((Transform)rect).localScale = Vector3.one;
		}

		private static void Stretch(RectTransform rect)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			rect.anchorMin = Vector2.zero;
			rect.anchorMax = Vector2.one;
			rect.offsetMin = Vector2.zero;
			rect.offsetMax = Vector2.zero;
		}

		private static string MarketCaption()
		{
			AuctionVenue venue;
			string text = AuctionNightMarket.Summary(AuctionState.TryGetVenue(out venue) ? new AuctionVenue?(venue) : ((AuctionVenue?)null));
			int num = text.IndexOf(" · ", StringComparison.Ordinal);
			string text2 = ((num >= 0) ? text.Substring(0, num) : text);
			string text3 = ((num >= 0) ? text.Substring(num + 3) : T("外观与对手行为不代表箱内真实价值。", "Exterior clues and rival behaviour do not guarantee cargo value."));
			return "<color=#D8B778>" + T("今晚行情 · ", "Tonight · ") + text2 + "</color>    <color=#B0B1B7>" + text3 + "</color>";
		}

		private static string Money(int value)
		{
			return value.ToString("N0") + " ¢";
		}

		private static string CashCaption()
		{
			if (!_phoneCollection)
			{
				return Money(PlayerStore.Instance.GetCash());
			}
			return AuctionText.Get("cash", "现金  {0}", PlayerStore.Instance.GetCash());
		}

		private static Rect FromTop(float x, float top, float width, float height)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return new Rect(x, 540f - top - height, width, height);
		}

		private static void BuildAuctionHeader()
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			GameObject obj = NewObject("BackgroundShade", _root.transform, typeof(Image), typeof(LayoutElement));
			obj.GetComponent<LayoutElement>().ignoreLayout = true;
			Stretch(obj.GetComponent<RectTransform>());
			((Graphic)obj.GetComponent<Image>()).color = new Color(0.015f, 0.02f, 0.035f, 0.7f);
			((Graphic)obj.GetComponent<Image>()).raycastTarget = false;
			obj.transform.SetSiblingIndex(1);
			HeaderLabel("Brand", "NIGHT CONTAINER AUCTION", 9f, 24f, 14f, 450f, 17f, AuctionUiStyle.Gold, (FontStyles)1);
			_title = HeaderLabel("Title", T("小窝夜间集装箱拍卖", "Night Container Auction"), 20f, 24f, 32f, 510f, 31f, Color.white, (FontStyles)1);
			_sessionSummary = HeaderLabel("SessionSummary", "", 10f, 555f, 24f, 175f, 39f, AuctionUiStyle.Muted, (FontStyles)0);
			HeaderLabel("CashCaption", T("可用现金", "Available cash"), 9f, 744f, 21f, 190f, 17f, AuctionUiStyle.Muted, (FontStyles)0);
			_cash = HeaderLabel("Cash", CashCaption(), 19f, 744f, 38f, 190f, 28f, Color.white, (FontStyles)1);
			GameObject obj2 = NewObject("HeaderRule", (Transform)(object)_layout, typeof(Image));
			SetRect(obj2.GetComponent<RectTransform>(), FromTop(24f, 70f, 912f, 1f));
			((Graphic)obj2.GetComponent<Image>()).color = AuctionUiStyle.Gold;
			((Graphic)obj2.GetComponent<Image>()).raycastTarget = false;
			CreatePanel("MarketStrip", FromTop(24f, 82f, 912f, 27f), SurfaceRaised);
			_eventSummary = HeaderLabel("NightMarketSummary", "", 10f, 36f, 87f, 888f, 18f, AuctionUiStyle.Gold, (FontStyles)0);
			((TMP_Text)_eventSummary).enableWordWrapping = false;
			((TMP_Text)_eventSummary).overflowMode = (TextOverflowModes)1;
			_status = HeaderLabel("Status", "", 9f, 24f, 114f, 912f, 14f, AuctionUiStyle.Muted, (FontStyles)0);
			((TMP_Text)_status).enableWordWrapping = false;
			((TMP_Text)_status).overflowMode = (TextOverflowModes)1;
			UpdateAuctionHeader();
		}

		private static TextMeshProUGUI HeaderLabel(string name, string text, float size, float x, float y, float width, float height, Color color, FontStyles style = (FontStyles)0)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			TextMeshProUGUI obj = CreateLabel(name, text, size, new Vector2(x, 0f - y), new Vector2(width, height), style);
			((Graphic)obj).color = color;
			return obj;
		}

		private static void UpdateAuctionHeader()
		{
			if (!((Object)(object)_sessionSummary == (Object)null))
			{
				((TMP_Text)_sessionSummary).text = T("第 ", "Day ") + AuctionCatalog.Day + T(" 天 · 夜间\n", " · Night\n") + (AuctionState.TryGetVenue(out var venue) ? (AuctionRules.VenueName(venue) + " · " + T("本晚 ", "Tonight ") + Math.Min(ResolvedAuctions + 1, 3) + " / " + 3 + T(" 场", " lots")) : T("选择今晚场次", "Choose tonight's venue"));
			}
		}

		private static void RenderLots(string message)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0546: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			if (!AuctionState.TryGetVenue(out var _))
			{
				RenderVenues();
				return;
			}
			_page = Page.Lots;
			ClearDynamic();
			_stageVisible = false;
			_nextAnnouncementAt = 0f;
			SetStatus(message);
			AddStageLabel("LotsHeading", T("本晚集装箱", "Tonight's Containers"), 15f, 24f, -131f, 480f, 27f, (FontStyles)1);
			TextMeshProUGUI val = CreateLabel("LotsHint", T("四箱中选择 · 最多完成三场 · 仅公开外观线索", "Choose from four · Up to three auctions · Exterior clues only"), 9f, new Vector2(520f, -139f), new Vector2(416f, 18f), (FontStyles)0);
			((TMP_Text)val).alignment = (TextAlignmentOptions)260;
			((Graphic)val).color = AuctionUiStyle.Muted;
			Dynamic.Add(FindLayoutChild(((TMP_Text)val).transform));
			for (int i = 0; i < _lots.Count; i++)
			{
				AuctionLot lot = _lots[i];
				int num = 24 + i % 2 * 462;
				int num2 = 163 + i / 2 * 158;
				Dynamic.Add(CreatePanel("LotPanel" + i, FromTop(num, num2, 450f, 145f), Surface));
				Dynamic.Add(CreateAccent("LotEdge" + i, FromTop(num + 1, num2 + 10, 2f, 125f), AuctionCatalog.CategoryColor(lot.Category)));
				Dynamic.Add(CreateContainerImage("LotCrate" + i, lot.Category, FromTop(num + 14, num2 + 25, 110f, 99f)));
				AddStageLabel("LotTitle" + i, AuctionCatalog.CategoryName(lot.Category), 16f, num + 142, -(num2 + 12), 175f, 25f, (FontStyles)1);
				string text = ((lot.PlayerLeading && lot.Finished) ? T("你已得标", "You won") : (lot.Withdrawn ? T("已放弃", "Withdrawn") : (lot.Lost ? (T("得标者 ", "Winner ") + LeaderName(lot)) : ((lot.PendingOffer >= 0) ? T("本轮播报中", "Announcing bids") : (lot.Started ? (T("第 ", "Round ") + (lot.Round + 1) + "/5") : T("等待竞拍", "Awaiting auction"))))));
				TextMeshProUGUI val2 = CreateLabel("LotState" + i, text, 8f, new Vector2((float)(num + 320), (float)(-(num2 + 19))), new Vector2(112f, 16f), (FontStyles)0);
				((TMP_Text)val2).alignment = (TextAlignmentOptions)260;
				((Graphic)val2).color = AuctionUiStyle.Gold;
				((TMP_Text)val2).enableAutoSizing = true;
				((TMP_Text)val2).fontSizeMin = 7f;
				((TMP_Text)val2).fontSizeMax = 8f;
				Dynamic.Add(FindLayoutChild(((TMP_Text)val2).transform));
				TextMeshProUGUI val3 = CreateLabel("LotText" + i, AuctionEngine.SurfaceClue(lot) + "\n" + T("风险 ", "Risk ") + Risk(lot) + " · " + MainMix(lot) + "\n" + T("货物与价值保持隐藏", "Cargo and value remain hidden"), 9.5f, new Vector2((float)(num + 142), (float)(-(num2 + 49))), new Vector2(288f, 60f), (FontStyles)0);
				((Graphic)val3).color = AuctionUiStyle.Muted;
				Dynamic.Add(FindLayoutChild(((TMP_Text)val3).transform));
				AuctionLot auctionLot = _lots.FirstOrDefault((AuctionLot other) => other.Started && !other.Finished && !other.Lost);
				bool flag = !SessionComplete && !lot.Finished && !lot.Lost && (auctionLot == null || auctionLot == lot);
				Dynamic.Add(CreateButton("Select" + i, (!flag) ? AuctionText.Get("cannot_bid", "不可竞拍") : (lot.Started ? T("继续竞拍", "Resume") : T("查看并竞拍", "View & Bid")), FromTop(num + 142, num2 + 112, 132f, 27f), delegate
				{
					SelectLot(lot);
				}, flag));
				if (AuctionNightMarket.LeakItems(lot).Length != 0)
				{
					Dynamic.Add(CreateButton("Manifest" + i, T("泄露货单", "Leaked Manifest"), FromTop(num + 286, num2 + 112, 146f, 27f), delegate
					{
						RenderManifest(lot);
					}, enabled: true));
				}
			}
			AddLobbyServices();
		}

		private static void AddLobbyServices()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			Dynamic.Add(CreateAccent("FooterRule", FromTop(24f, 499f, 912f, 1f), AuctionUiStyle.Border));
			Dynamic.Add(CreateButton("Orders", T("采购委托", "Orders"), FromTop(24f, 511f, 100f, 28f), delegate
			{
				_cancelOrderPending = false;
				RenderOrders();
			}, enabled: true));
			Dynamic.Add(CreateButton("Collection", T("藏品收集册", "Collection Book"), FromTop(136f, 511f, 100f, 28f), delegate
			{
				RenderCollection(_bookCategory);
			}, enabled: true));
			Dynamic.Add(CreateButton("History", T("历史账本", "Auction Ledger"), FromTop(248f, 511f, 100f, 28f), RenderHistory, enabled: true));
			Dynamic.Add(CreateButton("Leave", AuctionText.Get("return_store", "返回店铺"), FromTop(824f, 511f, 112f, 28f), delegate
			{
				Close();
			}, enabled: true));
			AddStageLabel("FooterHint", T("场次、现金和竞拍进度持续保留", "Venue, cash and auction progress are preserved"), 8.5f, 366f, -519f, 440f, 17f, (FontStyles)0);
		}

		private static GameObject CreateButton(string name, string text, Rect bounds, Action action, bool enabled)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Expected O, but got Unknown
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = NewObject(name, (Transform)(object)_layout, typeof(Image), typeof(Button));
			SetRect(val.GetComponent<RectTransform>(), bounds);
			bool flag = name.StartsWith("Select") || name.StartsWith("ChooseVenue") || name == "ConfirmVenue" || name == "Bid" || name == "Continue";
			((Graphic)AuctionUiStyle.RoundedImage(val, flag ? AuctionUiStyle.Gold : AuctionUiStyle.Border)).raycastTarget = true;
			GameObject obj = NewObject("ButtonOutline", val.transform, typeof(Image));
			Stretch(obj.GetComponent<RectTransform>());
			obj.GetComponent<RectTransform>().offsetMin = Vector2.one;
			obj.GetComponent<RectTransform>().offsetMax = -Vector2.one;
			AuctionUiStyle.RoundedImage(obj, Frame);
			GameObject obj2 = NewObject("ButtonFace", val.transform, typeof(Image));
			Stretch(obj2.GetComponent<RectTransform>());
			obj2.GetComponent<RectTransform>().offsetMin = Vector2.one * 2f;
			obj2.GetComponent<RectTransform>().offsetMax = Vector2.one * -2f;
			Image targetGraphic = AuctionUiStyle.RoundedImage(obj2, flag ? AuctionUiStyle.Gold : SurfaceRaised);
			Button component = val.GetComponent<Button>();
			((Selectable)component).targetGraphic = (Graphic)(object)targetGraphic;
			ColorBlock colors = default(ColorBlock);
			((ColorBlock)(ref colors)).normalColor = Color.white;
			((ColorBlock)(ref colors)).highlightedColor = new Color(1.12f, 1.12f, 1.12f);
			((ColorBlock)(ref colors)).selectedColor = Color.white;
			((ColorBlock)(ref colors)).pressedColor = new Color(0.75f, 0.75f, 0.75f);
			((ColorBlock)(ref colors)).disabledColor = new Color(0.4f, 0.4f, 0.43f);
			((ColorBlock)(ref colors)).colorMultiplier = 1f;
			((ColorBlock)(ref colors)).fadeDuration = 0.08f;
			((Selectable)component).colors = colors;
			((Selectable)component).interactable = enabled;
			((UnityEvent)component.onClick).AddListener((UnityAction)delegate
			{
				Core.Log?.Msg("Auction action: " + name);
				action();
			});
			TextMeshProUGUI obj3 = CreateChildLabel(val.transform, text, name.StartsWith("Digit") ? 16 : ((((Rect)(ref bounds)).height <= 30f) ? 10 : 13), (FontStyles)(flag ? 1 : 0));
			((TMP_Text)obj3).alignment = (TextAlignmentOptions)514;
			((Graphic)obj3).color = (Color)((!enabled) ? AuctionUiStyle.Muted : (flag ? Frame : new Color(0.94f, 0.92f, 0.87f)));
			return val;
		}
	}
	internal sealed class AuctionExclusiveSpec
	{
		internal string Id;

		internal ContainerCategory Category;

		internal int Tier;

		internal int Value;

		internal string Name;

		internal string Description;

		internal string[] Tags;

		internal string[] Types;

		internal int Width = 2;

		internal int Height = 2;

		internal string SpriteId;

		internal string NativeTemplate = "junk";
	}
	internal static class AuctionExclusiveCatalog
	{
		internal const string Atlas = "XIAOWO/NightAuction";

		internal const string ExclusiveTag = "XIAOWO_AUCTION_EXCLUSIVE";

		internal static readonly Dictionary<string, (string Name, string Description)> English;

		internal static readonly Dictionary<string, AuctionExclusiveSpec> Items;

		static AuctionExclusiveCatalog()
		{
			English = new Dictionary<string, (string, string)>(StringComparer.Ordinal)
			{
				["xiaowo_auction_hemostatic_foam"] = ("Military Hemostatic Foam", "Military-grade foam that seals wounds in seconds."),
				["xiaowo_auction_blood_filter"] = ("Portable Blood Filter", "Core filter for a disposable blood purification unit."),
				["xiaowo_auction_organ_case"] = ("Cold-Chain Organ Case", "A medical transport case still holding its low temperature."),
				["xiaowo_auction_neural_repair"] = ("Emergency Neural Repair", "Sealed medication for emergency nerve repair."),
				["xiaowo_auction_nano_suture"] = ("Field Nano-Suture Kit", "Nano-suture components sealed in a sterile military case."),
				["xiaowo_auction_regen_core"] = ("Black Clinic Regen Core", "An unregistered tissue regeneration core of unknown origin."),
				["xiaowo_auction_organ_matrix"] = ("Unregistered Organ Matrix", "Clinical printing matrix with full organ reconstruction data."),
				["xiaowo_auction_pocket_watch"] = ("Handmade Pocket Watch", "A mechanical pocket watch assembled by a master craftsman."),
				["xiaowo_auction_zero_g_perfume"] = ("Zero-G Perfume", "Limited perfume sold only at orbital stations."),
				["xiaowo_auction_crystal_wine_set"] = ("Crystal Wine Set", "An intact crystal wine set from a bygone era."),
				["xiaowo_auction_micro_engraving"] = ("Old-World Miniature", "A tiny engraving signed by an old-world artist."),
				["xiaowo_auction_century_vintage"] = ("Century Vintage", "A sealed bottle from an old cellar with its label intact."),
				["xiaowo_auction_orbital_watch"] = ("Orbital Astronomical Watch", "A limited mechanical watch made in an orbital workshop."),
				["xiaowo_auction_black_diamond"] = ("Stateless Black Diamond", "A giant black diamond with no registered origin."),
				["xiaowo_auction_forged_pass"] = ("Forged Access Chip", "Opens native keycard-locked crates. Uses remaining: {0}/10. Value scales with remaining uses; consumed after the final use."),
				["xiaowo_auction_silent_bolt"] = ("Silent Weapon Bolt", "A weapon bolt illegally modified for quieter operation."),
				["xiaowo_auction_cipher_ledger"] = ("Encrypted Black Ledger", "An encrypted ledger of underground trade routes."),
				["xiaowo_auction_black_decoder"] = ("Black-Market Decoder", "An illegal decoder that bypasses older electronic locks."),
				["xiaowo_auction_ghost_identity"] = ("Ghost Identity Board", "Single-use. Double-click in your shop to confirm use: resets investigation progress and reduces the current suspected crime amount by 50%. Does not cancel enforcement already underway. No effect means no consumption."),
				["xiaowo_auction_coilgun"] = ("Silent Coilgun Prototype", "An unproduced prototype of a low-noise coil weapon."),
				["xiaowo_auction_master_key"] = ("Military Master Key", "A core key capable of forging military access credentials."),
				["xiaowo_auction_bearing_set"] = ("High-Density Bearings", "Precision bearings for heavy industrial equipment."),
				["xiaowo_auction_calibrator"] = ("Industrial Calibrator", "A portable instrument for precision machine calibration."),
				["xiaowo_auction_supercoil"] = ("Superconducting Coil", "A fully sealed industrial superconducting coil."),
				["xiaowo_auction_control_core"] = ("Prototype Control Core", "An unreleased automation control core."),
				["xiaowo_auction_fabricator_core"] = ("Self-Calibrating Fabricator", "A fabrication core that corrects its own machining errors."),
				["xiaowo_auction_microreactor"] = ("Compact Microreactor", "An experimental compact industrial power source."),
				["xiaowo_auction_zero_point"] = ("Zero-Point Stabilizer", "A precision module for extreme energy experiments.")
			};
			Items = new Dictionary<string, AuctionExclusiveSpec>(StringComparer.Ordinal)
			{
				["xiaowo_auction_hemostatic_foam"] = Spec("xiaowo_auction_hemostatic_foam", ContainerCategory.Medical, 1, 1200, "军规止血泡沫", "能够在数秒内封闭创口的军规止血泡沫。", "MEDICAL"),
				["xiaowo_auction_blood_filter"] = Spec("xiaowo_auction_blood_filter", ContainerCategory.Medical, 1, 2400, "便携血液净化芯", "一次性便携血液净化装置的核心滤芯。", "MEDICAL"),
				["xiaowo_auction_organ_case"] = Spec("xiaowo_auction_organ_case", ContainerCategory.Medical, 1, 4200, "冷链器官保存匣", "仍保持低温的医用器官运输保存匣。", "MEDICAL"),
				["xiaowo_auction_neural_repair"] = Spec("xiaowo_auction_neural_repair", ContainerCategory.Medical, 1, 5800, "应急神经修复剂", "用于紧急修复神经损伤的封装药剂。", "MEDICAL"),
				["xiaowo_auction_nano_suture"] = Spec("xiaowo_auction_nano_suture", ContainerCategory.Medical, 1, 8000, "战地纳米缝合组", "封存在军用无菌盒中的纳米缝合组件。", "MEDICAL"),
				["xiaowo_auction_regen_core"] = Spec("xiaowo_auction_regen_core", ContainerCategory.Medical, 2, 28000, "黑诊所再生核心", "没有登记编号的组织再生核心，来源不明。", "MEDICAL"),
				["xiaowo_auction_organ_matrix"] = Spec("xiaowo_auction_organ_matrix", ContainerCategory.Medical, 3, 62000, "未登记器官打印矩阵", "保存着完整器官重构参数的临床打印矩阵。", "MEDICAL", "CONTRABAND_ITEM_TAG"),
				["xiaowo_auction_pocket_watch"] = Spec("xiaowo_auction_pocket_watch", ContainerCategory.Luxury, 1, 1500, "手工机械怀表", "由老工匠手工装配的机械怀表。", "LUXURY_ITEM"),
				["xiaowo_auction_zero_g_perfume"] = Spec("xiaowo_auction_zero_g_perfume", ContainerCategory.Luxury, 1, 2800, "零重力香水", "只在轨道站销售的限量香水。", "LUXURY_ITEM"),
				["xiaowo_auction_crystal_wine_set"] = Spec("xiaowo_auction_crystal_wine_set", ContainerCategory.Luxury, 1, 4300, "典藏晶瓷酒具", "保存完整的旧时代晶瓷酒具。", "LUXURY_ITEM"),
				["xiaowo_auction_micro_engraving"] = Spec("xiaowo_auction_micro_engraving", ContainerCategory.Luxury, 1, 6000, "旧世名家微雕", "带有旧世名家落款的微型雕刻。", "LUXURY_ITEM"),
				["xiaowo_auction_century_vintage"] = Spec("xiaowo_auction_century_vintage", ContainerCategory.Luxury, 1, 10000, "封存世纪陈酿", "旧时代酒窖留下的完整封装，瓶签仍然清晰。", "LUXURY_ITEM", "ALCOHOL", "WINE_CONTAINER_TAG"),
				["xiaowo_auction_orbital_watch"] = Spec("xiaowo_auction_orbital_watch", ContainerCategory.Luxury, 2, 32000, "轨道工坊天文表", "由轨道工坊限量制造的机械天文表。", "LUXURY_ITEM"),
				["xiaowo_auction_black_diamond"] = Spec("xiaowo_auction_black_diamond", ContainerCategory.Luxury, 3, 68000, "无主权黑钻", "没有国家印记与流通记录的巨型黑钻。", "LUXURY_ITEM"),
				["xiaowo_auction_forged_pass"] = Spec("xiaowo_auction_forged_pass", ContainerCategory.Contraband, 1, 1300, "伪造通行芯片", "可开启原版钥匙卡锁箱。剩余次数：{0}/10。价值按剩余次数比例降低，次数耗尽后销毁。", "CONTRABAND_ITEM_TAG", "ACCESS_CARD"),
				["xiaowo_auction_silent_bolt"] = Spec("xiaowo_auction_silent_bolt", ContainerCategory.Contraband, 1, 2700, "静默枪机组件", "经过非法消音改造的武器枪机组件。", "CONTRABAND_ITEM_TAG", "WEAPON"),
				["xiaowo_auction_cipher_ledger"] = Spec("xiaowo_auction_cipher_ledger", ContainerCategory.Contraband, 1, 4500, "加密违禁账本", "记录地下交易渠道的加密账本。", "CONTRABAND_ITEM_TAG"),
				["xiaowo_auction_black_decoder"] = Spec("xiaowo_auction_black_decoder", ContainerCategory.Contraband, 1, 5900, "黑市破解器", "可绕过多种旧式电子封锁的非法破解器。", "CONTRABAND_ITEM_TAG", "MODULE"),
				["xiaowo_auction_ghost_identity"] = Spec("xiaowo_auction_ghost_identity", ContainerCategory.Contraband, 1, 9000, "幽灵身份主板", "一次性道具。在店铺内双击确认使用：清空当前调查进度，涉嫌犯罪金额减少50%。不能撤销已启动的执法处置，无效果时不消耗。", "CONTRABAND_ITEM_TAG"),
				["xiaowo_auction_coilgun"] = Spec("xiaowo_auction_coilgun", ContainerCategory.Contraband, 2, 30000, "静默线圈枪原型", "没有量产记录的低噪声线圈武器原型。", "CONTRABAND_ITEM_TAG", "WEAPON", "FIREARM"),
				["xiaowo_auction_master_key"] = Spec("xiaowo_auction_master_key", ContainerCategory.Contraband, 3, 65000, "军用权限母钥", "能够伪造多级军用通行凭证的核心密钥。", "CONTRABAND_ITEM_TAG"),
				["xiaowo_auction_bearing_set"] = Spec("xiaowo_auction_bearing_set", ContainerCategory.Engineering, 1, 1100, "高密度轴承组", "适用于重型设备的高密度精密轴承。", "MATERIAL"),
				["xiaowo_auction_calibrator"] = Spec("xiaowo_auction_calibrator", ContainerCategory.Engineering, 1, 2500, "工业校准仪", "用于校准精密生产设备的便携仪器。", "TOOL"),
				["xiaowo_auction_supercoil"] = Spec("xiaowo_auction_supercoil", ContainerCategory.Engineering, 1, 4100, "超导线圈", "封装完好的工业级超导线圈。", "MATERIAL", "MODULE"),
				["xiaowo_auction_control_core"] = Spec("xiaowo_auction_control_core", ContainerCategory.Engineering, 1, 5700, "原型控制核心", "尚未量产的自动化设备控制核心。", "MODULE", "MACHINE"),
				["xiaowo_auction_fabricator_core"] = Spec("xiaowo_auction_fabricator_core", ContainerCategory.Engineering, 1, 8500, "自校准制造核心", "能够自行修正加工误差的制造设备核心。", "MODULE"),
				["xiaowo_auction_microreactor"] = Spec("xiaowo_auction_microreactor", ContainerCategory.Engineering, 2, 29000, "自修复微型反应堆", "具备有限自修复能力的密封式微型反应堆。", "MODULE", "MACHINE", "STANDARD_MACHINE_TAG"),
				["xiaowo_auction_zero_point"] = Spec("xiaowo_auction_zero_point", ContainerCategory.Engineering, 3, 64000, "零点稳定模块", "用于稳定极端能源实验的高精度工程模块。", "MODULE", "MACHINE")
			};
			AuctionNewItems.Add(Items, English);
			AuctionItemLinkRules.Configure(Items, English);
		}

		internal static AuctionExclusiveSpec Spec(string id, ContainerCategory category, int tier, int value, string name, string description, params string[] tags)
		{
			return new AuctionExclusiveSpec
			{
				Id = id,
				Category = category,
				Tier = tier,
				Value = value,
				Name = name,
				Description = description,
				Tags = tags,
				SpriteId = id,
				Types = NativeTypes(category, tags)
			};
		}

		internal static int CategoryCount(ContainerCategory category)
		{
			return Items.Values.Count((AuctionExclusiveSpec spec) => spec.Category == category);
		}

		private static string[] NativeTypes(ContainerCategory category, string[] tags)
		{
			List<string> list = new List<string>();
			switch (category)
			{
			case ContainerCategory.Medical:
				list.Add("MEDICAL");
				break;
			case ContainerCategory.Luxury:
				list.Add("LUXURY_ITEM");
				if (Array.IndexOf(tags, "ALCOHOL") >= 0)
				{
					list.Add("ALCOHOL");
				}
				break;
			case ContainerCategory.Contraband:
			{
				string[] array = new string[3] { "ACCESS_CARD", "WEAPON", "MODULE" };
				foreach (string text2 in array)
				{
					if (Array.IndexOf(tags, text2) >= 0)
					{
						list.Add(text2);
					}
				}
				break;
			}
			case ContainerCategory.Engineering:
			{
				string[] array = new string[4] { "MATERIAL", "TOOL", "MODULE", "MACHINE" };
				foreach (string text in array)
				{
					if (Array.IndexOf(tags, text) >= 0)
					{
						list.Add(text);
					}
				}
				break;
			}
			}
			return list.ToArray();
		}

		internal static bool IsExclusive(string id)
		{
			if (id != null)
			{
				return Items.ContainsKey(id);
			}
			return false;
		}

		internal static List<AuctionExclusiveSpec> Candidates(ContainerCategory category, int maxTier)
		{
			List<AuctionExclusiveSpec> list = new List<AuctionExclusiveSpec>();
			foreach (AuctionExclusiveSpec value in Items.Values)
			{
				if (value.Category == category && value.Tier <= maxTier)
				{
					list.Add(value);
				}
			}
			return list;
		}

		internal static IEnumerable<AuctionExclusiveSpec> SupplementalCandidates(ContainerCategory category)
		{
			foreach (AuctionExclusiveSpec value in Items.Values)
			{
				if (value.Category == category && value.Tier == 1 && value.Value <= 6000)
				{
					yield return value;
				}
			}
		}
	}
	internal static class AuctionExclusiveFactory
	{
		private static readonly string[] CategoryTags = new string[22]
		{
			"MEDICAL", "MEDICINE", "MEDS", "DRUG", "HEALING_ITEM_TAG", "LUXURY_ITEM", "ALCOHOL", "WINE_CONTAINER_TAG", "JEWELRY", "CONTRABAND",
			"CONTRABAND_ITEM_TAG", "ILLEGAL_ITEM_TAG", "ACCESS_CARD", "WEAPON", "FIREARM", "MACHINE", "STANDARD_MACHINE_TAG", "MODULE", "MODULE_TAG", "MATERIAL",
			"COMPONENT", "TOOL"
		};

		private static readonly string[] TemplateTags = new string[3] { "BASIC_MATERIAL", "JUNK", "SCRAP_METAL" };

		private const string ContrabandLevelTag = "CONTRABAND_LEVEL";

		private static string ExpectedContrabandLevel(AuctionExclusiveSpec spec)
		{
			if (spec.Category != ContainerCategory.Contraband && Array.IndexOf(spec.Tags, "CONTRABAND_ITEM_TAG") < 0)
			{
				return null;
			}
			if (!(spec.Id == "xiaowo_auction_forged_pass") && spec.Tier < 2)
			{
				return "CONTRABAND_LEVEL_MID";
			}
			return "CONTRABAND_LEVEL_HIGH";
		}

		internal static bool NeedsRepair(GameItem item, AuctionExclusiveSpec spec)
		{
			long num = ((spec.Id == "xiaowo_auction_forged_pass") ? AuctionForgedPass.ValueFor(item, spec.Value) : AuctionItemLinks.ValueFor(item, spec.Value));
			if (item == null || !ShapeMatches(item.shape, spec) || !ShapeMatches(item.modifiedShape, spec) || item.IsTag("XIAOWO_AUCTION_EXCLUSIVE") || (!AuctionItemLinkRules.IsModule(spec.Id) && item.unitValue != num) || AuctionItemLinks.NeedsRepair(item))
			{
				return true;
			}
			if (item.itemTypes == null || item.itemTypes.Count != spec.Types.Length)
			{
				return true;
			}
			for (int i = 0; i < spec.Types.Length; i++)
			{
				if (!string.Equals(item.itemTypes[i], spec.Types[i], StringComparison.Ordinal))
				{
					return true;
				}
			}
			string[] templateTags = TemplateTags;
			foreach (string text in templateTags)
			{
				if (item.IsTag(text))
				{
					return true;
				}
			}
			templateTags = spec.Tags;
			foreach (string text2 in templateTags)
			{
				if (!item.IsTag(text2))
				{
					return true;
				}
			}
			templateTags = spec.Types;
			foreach (string text3 in templateTags)
			{
				if (!item.IsTag(text3))
				{
					return true;
				}
			}
			string text4 = ExpectedContrabandLevel(spec);
			if (text4 != null)
			{
				if (item.IsTag("CONTRABAND") && item.IsTag("CONTRABAND_LEVEL"))
				{
					TagState tagReadonly = item.GetTagReadonly("CONTRABAND_LEVEL");
					if (!(((tagReadonly != null) ? tagReadonly.GetString() : null) != text4))
					{
						goto IL_018e;
					}
				}
				return true;
			}
			goto IL_018e;
			IL_018e:
			if (text4 == null && item.IsTag("CONTRABAND_LEVEL"))
			{
				return true;
			}
			if (spec.Id == "xiaowo_auction_forged_pass" && !item.IsTag("XIAOWO_FORGED_PASS_CHARGES"))
			{
				return true;
			}
			templateTags = CategoryTags;
			foreach (string text5 in templateTags)
			{
				if ((!(text5 == "MODULE_TAG") || !AuctionItemLinkRules.IsModule(spec.Id)) && (!(text5 == "CONTRABAND") || text4 == null) && Array.IndexOf(spec.Tags, text5) < 0 && Array.IndexOf(spec.Types, text5) < 0 && item.IsTag(text5))
				{
					return true;
				}
			}
			return false;
		}

		internal static GameItem Create(string id, bool owned)
		{
			if (!AuctionExclusiveCatalog.Items.TryGetValue(id, out var value))
			{
				return null;
			}
			GameItem obj = AuctionItemLinks.CreateTemplate(value.NativeTemplate, owned);
			GeneralHelper.SetItemOwned(obj, owned);
			Apply(obj, value, newItem: true);
			((GameItemFunc)obj).onLoaded = delegate(GameItem loaded)
			{
				Repair(loaded);
			};
			obj.ValidateShapeState();
			return obj;
		}

		internal static void Repair(GameItem item)
		{
			if (item != null && AuctionExclusiveCatalog.Items.TryGetValue(item.identifier, out var value))
			{
				bool num = !ShapeMatches(item.shape, value) || !ShapeMatches(item.modifiedShape, value);
				Apply(item, value);
				if (num)
				{
					item.ValidateShapeState();
				}
			}
		}

		private static void Apply(GameItem item, AuctionExclusiveSpec spec, bool newItem = false)
		{
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			item.identifier = spec.Id;
			ApplyLocalizedText(item, spec);
			if (!AuctionItemLinkRules.IsModule(spec.Id) || newItem)
			{
				item.SetValue((spec.Id == "xiaowo_auction_forged_pass") ? AuctionForgedPass.ValueFor(item, spec.Value) : AuctionItemLinks.ValueFor(item, spec.Value));
			}
			item.unitCount = 1;
			AuctionItemLinks.SetTag(item, "STACKABLE_ITEM_TAG", enabled: false);
			string[] templateTags = TemplateTags;
			foreach (string tag in templateTags)
			{
				AuctionItemLinks.SetTag(item, tag, enabled: false);
			}
			templateTags = CategoryTags;
			foreach (string tag2 in templateTags)
			{
				AuctionItemLinks.SetTag(item, tag2, enabled: false);
			}
			AuctionItemLinks.SetTag(item, "XIAOWO_AUCTION_EXCLUSIVE", enabled: false);
			templateTags = spec.Tags;
			foreach (string tag3 in templateTags)
			{
				AuctionItemLinks.SetTag(item, tag3, enabled: true);
			}
			templateTags = spec.Types;
			foreach (string tag4 in templateTags)
			{
				AuctionItemLinks.SetTag(item, tag4, enabled: true);
			}
			string text = ExpectedContrabandLevel(spec);
			if (text != null)
			{
				AuctionItemLinks.SetTag(item, "CONTRABAND", enabled: true);
				item.EnableTag("CONTRABAND_LEVEL", true);
				item.state.EnableTag("CONTRABAND_LEVEL");
				item.state.GetTag("CONTRABAND_LEVEL").SetString(text);
				if (item.modifiedState != null)
				{
					item.modifiedState.EnableTag("CONTRABAND_LEVEL");
					item.modifiedState.GetTag("CONTRABAND_LEVEL").SetString(text);
				}
			}
			else
			{
				item.DisableTag("CONTRABAND_LEVEL", true);
				TagSystem state = item.state;
				if (state != null)
				{
					state.DisableTag("CONTRABAND_LEVEL");
				}
				TagSystem modifiedState = item.modifiedState;
				if (modifiedState != null)
				{
					modifiedState.DisableTag("CONTRABAND_LEVEL");
				}
			}
			item.itemTypes?.Clear();
			templateTags = spec.Types;
			foreach (string item2 in templateTags)
			{
				item.itemTypes.Add(item2);
			}
			OfficialAssets.SetSprite(item, spec.Id);
			if (!ShapeMatches(item.shape, spec) || !ShapeMatches(item.modifiedShape, spec))
			{
				GridShape shape = item.shape;
				GridShapeBuilder val = new GridShapeBuilder(spec.Width, spec.Height).SetDataFill(spec.Width, spec.Height, (byte)1);
				if (shape != null)
				{
					val.SetTransform(shape);
				}
				item.shape = val.Build();
				item.modifiedShape = item.shape.Clone();
			}
			if (spec.Id == "xiaowo_auction_forged_pass")
			{
				AuctionForgedPass.Ensure(item);
				ApplyLocalizedText(item, spec);
			}
			AuctionItemLinks.Ensure(item, newItem);
			ApplyLocalizedText(item, spec);
		}

		internal static bool ShapeMatches(GridShape shape, AuctionExclusiveSpec spec)
		{
			if (shape != null && shape.width == spec.Width)
			{
				return shape.height == spec.Height;
			}
			return false;
		}

		internal static void RefreshLocalizedText(GameItem item)
		{
			if (item != null && AuctionExclusiveCatalog.Items.TryGetValue(item.identifier, out var value))
			{
				ApplyLocalizedText(item, value);
			}
		}

		internal static void RefreshOwnedText()
		{
			PlayerStore existing = NativeStore.Existing;
			List<GameItem> list = ((existing != null) ? existing.FindAllItem(true) : null);
			if (list == null)
			{
				return;
			}
			foreach (GameItem item in list)
			{
				RefreshLocalizedText(item);
			}
		}

		private static void ApplyLocalizedText(GameItem item, AuctionExclusiveSpec spec)
		{
			int num;
			string text;
			if (AuctionExclusiveCatalog.English.TryGetValue(spec.Id, out (string, string) value))
			{
				num = ((!AuctionText.IsChinese(AuctionText.CurrentLocaleCode)) ? 1 : 0);
				if (num != 0)
				{
					(text, _) = value;
					goto IL_0035;
				}
			}
			else
			{
				num = 0;
			}
			text = spec.Name;
			goto IL_0035;
			IL_0035:
			string text2 = text;
			string text3 = ((num != 0) ? value.Item2 : spec.Description);
			if (spec.Id == "xiaowo_auction_forged_pass")
			{
				text3 = string.Format(text3, AuctionForgedPass.Remaining(item));
			}
			if (spec.Id == "xiaowo_auction_calibrator")
			{
				text3 = string.Format(text3, AuctionItemLinks.Remaining(item));
			}
			item.identifierName = text2;
			item.name = text2;
			item.shortDescription = text3;
			item.longDescription = text3;
			item.flavorText = AuctionText.Select("只在夜间集装箱拍卖中流通的稀有货物。", "Rare goods traded only at the night container auction.");
		}
	}
	internal static class AuctionForgedPass
	{
		internal const string Id = "xiaowo_auction_forged_pass";

		internal const string ChargesTag = "XIAOWO_FORGED_PASS_CHARGES";

		private const int MaxCharges = 10;

		private static GameItem _nativeCard;

		internal static void ClearTemplate()
		{
			_nativeCard = null;
		}

		internal static int Remaining(GameItem item)
		{
			int? obj;
			if (item == null)
			{
				obj = null;
			}
			else
			{
				TagState tagReadonly = item.GetTagReadonly("XIAOWO_FORGED_PASS_CHARGES");
				obj = ((tagReadonly != null) ? new int?(tagReadonly.GetInt()) : ((int?)null));
			}
			return Math.Clamp(obj ?? 10, 0, 10);
		}

		internal static bool CanOpen(GameItem key, GameItem box)
		{
			if (key != null && Remaining(key) > 0 && box != null && ObjectIdentity.Key(box) != ObjectIdentity.Key(key) && LockHelper.IsLockedContainer(box) && LockHelper.IsCurrentlyLocked(box))
			{
				return !string.IsNullOrWhiteSpace(LockHelper.GetLock(box));
			}
			return false;
		}

		internal static long ValueFor(GameItem item, int fullValue)
		{
			return (long)fullValue * (long)Remaining(item) / 10;
		}

		internal static void Ensure(GameItem item)
		{
			if (item != null && !(item.identifier != "xiaowo_auction_forged_pass"))
			{
				if (!item.IsTag("XIAOWO_FORGED_PASS_CHARGES"))
				{
					SetCharges(item, 10);
				}
				if (_nativeCard == null)
				{
					_nativeCard = DirectoryMaster.Item("cmd_keycard", false) ?? throw new InvalidOperationException("Native command keycard is unavailable.");
				}
				if (((GameItemFunc)_nativeCard).mayItemTargetItemFunc == null || ((GameItemFunc)_nativeCard).canItemTargetItemFunc == null || ((GameItemFunc)_nativeCard).onItemTargetItemFunc == null)
				{
					throw new InvalidOperationException("Native command keycard interaction is unavailable.");
				}
				((GameItemFunc)item).mayItemTargetItemFunc = ((GameItemFunc)_nativeCard).mayItemTargetItemFunc;
				((GameItemFunc)item).canItemTargetItemFunc = ((GameItemFunc)_nativeCard).canItemTargetItemFunc;
				((GameItemFunc)item).onItemTargetItemFunc = ((GameItemFunc)_nativeCard).onItemTargetItemFunc;
				((GameItemFunc)item).mayThisTargetItemFunc = ((GameItemFunc)_nativeCard).mayThisTargetItemFunc;
				((GameItemFunc)item).canThisTargetItemFunc = ((GameItemFunc)_nativeCard).canThisTargetItemFunc;
				((GameItemFunc)item).onThisTargetItemFunc = ((GameItemFunc)_nativeCard).onThisTargetItemFunc;
				((GameItemFunc)item).mayItemTargetThisFunc = ((GameItemFunc)_nativeCard).mayItemTargetThisFunc;
				((GameItemFunc)item).canItemTargetThisFunc = ((GameItemFunc)_nativeCard).canItemTargetThisFunc;
				((GameItemFunc)item).onItemTargetThisFunc = ((GameItemFunc)_nativeCard).onItemTargetThisFunc;
			}
		}

		internal static void SetCharges(GameItem item, int charges)
		{
			int num = Math.Clamp(charges, 0, 10);
			item.state.EnableTag("XIAOWO_FORGED_PASS_CHARGES");
			item.state.GetTag("XIAOWO_FORGED_PASS_CHARGES").SetInt(num);
			if (item.modifiedState != null)
			{
				item.modifiedState.EnableTag("XIAOWO_FORGED_PASS_CHARGES");
				item.modifiedState.GetTag("XIAOWO_FORGED_PASS_CHARGES").SetInt(num);
			}
			item.SetValue(ValueFor(item, AuctionExclusiveCatalog.Items["xiaowo_auction_forged_pass"].Value));
			AuctionExclusiveFactory.RefreshLocalizedText(item);
		}
	}
	[HarmonyPatch(typeof(GameItem), "MayTarget")]
	internal static class AuctionForgedPassMayTargetPatch
	{
		private static bool Prefix(GameItem __instance, GameItem targetItem, ref bool __result)
		{
			if (((__instance != null) ? __instance.identifier : null) != "xiaowo_auction_forged_pass")
			{
				return true;
			}
			__result = AuctionForgedPass.CanOpen(__instance, targetItem);
			return false;
		}
	}
	[HarmonyPatch(typeof(GameItem), "CanTarget")]
	internal static class AuctionForgedPassCanTargetPatch
	{
		private static bool Prefix(GameItem __instance, GameItem targetItem, ref bool __result)
		{
			if (((__instance != null) ? __instance.identifier : null) != "xiaowo_auction_forged_pass")
			{
				return true;
			}
			__result = AuctionForgedPass.CanOpen(__instance, targetItem);
			return false;
		}
	}
	[HarmonyPatch(typeof(LockHelper), "TryOpenLock")]
	internal static class AuctionForgedPassOpenPatch
	{
		private static bool Prefix(GameItem key, GameItem box)
		{
			if (((key != null) ? key.identifier : null) != "xiaowo_auction_forged_pass")
			{
				return true;
			}
			if (!AuctionForgedPass.CanOpen(key, box))
			{
				return false;
			}
			LockHelper.UnlockContainer(box, false);
			if (!LockHelper.IsCurrentlyLocked(box))
			{
				AuctionForgedPass.SetCharges(key, AuctionForgedPass.Remaining(key) - 1);
				if (AuctionForgedPass.Remaining(key) == 0)
				{
					GameInventory parentInventory = key.parentInventory;
					if (parentInventory != null)
					{
						parentInventory.Expel(key);
					}
					key.Destroy();
				}
			}
			return false;
		}
	}
	internal static class AuctionGhostIdentity
	{
		internal const string Id = "xiaowo_auction_ghost_identity";

		private const string UsedTag = "XIAOWO_GHOST_IDENTITY_USED";

		private static GameItem _pending;

		private static ConfirmPanelUIManager _dialog;

		private static Action _confirmAction;

		private static Action _cancelAction;

		private static ConfirmPanelUIManager _labelDialog;

		private static string _originalYes;

		private static string _originalNo;

		private static readonly List<(Behaviour Component, bool Enabled)> LabelLocalizers = new List<(Behaviour, bool)>();

		internal static void OpenDialog(ConfirmPanelUIManager dialog, string body, Action yes = null, Action no = null, bool confirmation = false, string yesCaption = null)
		{
			dialog.OpenPanel(body, yes, no, false);
			_labelDialog = dialog;
			_originalYes = ((TMP_Text)dialog.yesTMP).text;
			_originalNo = ((TMP_Text)dialog.noTMP).text;
			TextMeshProUGUI[] array = (TextMeshProUGUI[])(object)new TextMeshProUGUI[2] { dialog.yesTMP, dialog.noTMP };
			for (int i = 0; i < array.Length; i++)
			{
				Behaviour[] components = ((Component)array[i]).GetComponents<Behaviour>();
				foreach (Behaviour val in components)
				{
					string fullName = ((object)val).GetType().FullName;
					if (!(fullName != "TMPLocalizer") || !(fullName != "UnityEngine.Localization.Components.LocalizeStringEvent"))
					{
						LabelLocalizers.Add((val, val.enabled));
						val.enabled = false;
					}
				}
			}
			((TMP_Text)dialog.yesTMP).text = yesCaption ?? (confirmation ? AuctionText.Select("使用", "Use") : AuctionText.Select("确定", "OK"));
			((TMP_Text)dialog.noTMP).text = (confirmation ? AuctionText.Select("取消", "Cancel") : AuctionText.Select("关闭", "Close"));
		}

		internal static void RestoreLabels(ConfirmPanelUIManager dialog)
		{
			if ((Object)(object)_labelDialog == (Object)null || (Object)(object)_labelDialog != (Object)(object)dialog)
			{
				return;
			}
			if (Object.op_Implicit((Object)(object)_labelDialog))
			{
				((TMP_Text)_labelDialog.yesTMP).text = _originalYes;
				((TMP_Text)_labelDialog.noTMP).text = _originalNo;
			}
			foreach (var labelLocalizer in LabelLocalizers)
			{
				if ((Object)(object)labelLocalizer.Component != (Object)null && Object.op_Implicit((Object)(object)labelLocalizer.Component))
				{
					labelLocalizer.Component.enabled = labelLocalizer.Enabled;
				}
			}
			LabelLocalizers.Clear();
			_labelDialog = null;
		}

		private static bool Check(GameItem item, out PlayerStore store, out int[] amounts, out string reason)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			store = null;
			amounts = null;
			reason = AuctionText.Select("请在店铺内使用。", "Use this item in your shop.");
			Scene activeScene = SceneManager.GetActiveScene();
			if (((Scene)(ref activeScene)).name != "InventoryScene" || (Object)(object)EmporiumEntry.Instance == (Object)null)
			{
				return false;
			}
			store = PlayerStore.Instance;
			reason = AuctionText.Select("只能使用你拥有且尚未消耗的主板。", "You must own an unused board.");
			if (store == null || ((item != null) ? item.identifier : null) != "xiaowo_auction_ghost_identity" || item.IsTag("XIAOWO_GHOST_IDENTITY_USED") || !GeneralHelper.IsItemOwned(item))
			{
				return false;
			}
			List<GameItem> list = store.FindAllItem(true);
			bool flag = false;
			if (list != null)
			{
				foreach (GameItem item2 in list)
				{
					if (ObjectIdentity.Key(item2) == ObjectIdentity.Key(item))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
			reason = AuctionText.Select("治安档案尚未就绪，主板未消耗。", "Security records are not ready. The board was not consumed.");
			SecData secData = store.secData;
			if (secData?.crimeAmount == null || secData.crimeTypes == null || secData.crimeAmount.Count != secData.crimeTypes.Count)
			{
				return false;
			}
			amounts = new int[secData.crimeAmount.Count];
			long num = 0L;
			for (int i = 0; i < amounts.Length; i++)
			{
				amounts[i] = secData.crimeAmount[i];
				if (amounts[i] < 0)
				{
					return false;
				}
				num += amounts[i];
			}
			if (num > int.MaxValue || secData.GetTotalCrimeValue() != num)
			{
				return false;
			}
			reason = AuctionText.Select("执法处置已启动，无法使用主板撤销。", "Enforcement has started and cannot be cancelled with this board.");
			if (secData.state != 0 || secData.evidenceLevel >= 100)
			{
				return false;
			}
			reason = AuctionText.Select("当前没有可清除的调查进度或犯罪金额，主板未消耗。", "No investigation progress or reducible crime amount. The board was not consumed.");
			return GhostIdentityRules.Available(secData.evidenceLevel, secData.state, num);
		}

		internal static bool RequestUse(GameItem item)
		{
			ConfirmPanelUIManager dialog = ((IEnumerable<ConfirmPanelUIManager>)Object.FindObjectsOfType<ConfirmPanelUIManager>(true)).FirstOrDefault((Func<ConfirmPanelUIManager, bool>)delegate(ConfirmPanelUIManager candidate)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)candidate.panel != (Object)null)
				{
					Scene scene = ((Component)candidate).gameObject.scene;
					return ((Scene)(ref scene)).name == "InventoryScene";
				}
				return false;
			});
			if ((Object)(object)dialog == (Object)null || dialog.panel.activeInHierarchy)
			{
				return false;
			}
			if (!Check(item, out var store, out var amounts, out var reason))
			{
				OpenDialog(dialog, reason);
				return false;
			}
			_pending = item;
			_dialog = dialog;
			string run = store.runID;
			int slot = store.saveSlotId;
			IntPtr pointer = ObjectIdentity.Key(store);
			int num = amounts.Sum();
			string body = AuctionText.Select("使用幽灵身份主板？\n调查进度归零，涉嫌犯罪金额减少50%。\n金额：" + num + " -> " + (num - num / 2) + "¢\n消耗一件；不能撤销已启动的执法处置。", "Use Ghost Identity Board?\nReset investigation; halve crime amount.\nAmount: " + num + " -> " + (num - num / 2) + "¢\nUses 1 board. Cannot cancel enforcement.");
			_confirmAction = MonoCallbacks.Bind<Action>((Action)delegate
			{
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				if (!(ObjectIdentity.Key(_pending) != ObjectIdentity.Key(item)))
				{
					_pending = null;
					Scene activeScene = SceneManager.GetActiveScene();
					if (!(((Scene)(ref activeScene)).name != "InventoryScene") && !(ObjectIdentity.Key(PlayerStore.Instance) != pointer) && PlayerStore.Instance.saveSlotId == slot && !(PlayerStore.Instance.runID != run))
					{
						try
						{
							TryUse(item, out var message);
							OpenDialog(dialog, message);
						}
						catch (Exception ex)
						{
							Core.Log?.Error("[幽灵身份主板] 使用失败: " + ex);
							OpenDialog(dialog, AuctionText.Select("使用失败，请查看日志。", "Use failed. Check the log."));
						}
					}
				}
			});
			_cancelAction = MonoCallbacks.Bind<Action>((Action)delegate
			{
				_pending = null;
			});
			OpenDialog(dialog, body, _confirmAction, _cancelAction, confirmation: true);
			return true;
		}

		internal static bool TryUse(GameItem item, out string message)
		{
			if (!Check(item, out var store, out var amounts, out message))
			{
				return false;
			}
			SecData secData = store.secData;
			int[] array = GhostIdentityRules.Half(amounts);
			int evidenceLevel = secData.evidenceLevel;
			GameInventory parentInventory = item.parentInventory;
			if (parentInventory == null)
			{
				message = AuctionText.Select("请先将主板放入你的库存，未消耗。", "Place the board in your inventory first. It was not consumed.");
				return false;
			}
			GridShape shape = item.modifiedShape.Clone();
			bool flag = false;
			try
			{
				flag = parentInventory.Expel(item);
				if (!flag)
				{
					message = AuctionText.Select("无法从当前容器取出主板，未消耗。", "Cannot remove the board from this container. It was not consumed.");
					return false;
				}
				for (int i = 0; i < array.Length; i++)
				{
					secData.crimeAmount[i] = array[i];
				}
				secData.evidenceLevel = 0;
				item.EnableTag("XIAOWO_GHOST_IDENTITY_USED", true);
				GeneralHelper.SetItemOwned(item, false);
				item.Destroy();
			}
			catch
			{
				for (int j = 0; j < amounts.Length; j++)
				{
					secData.crimeAmount[j] = amounts[j];
				}
				secData.evidenceLevel = evidenceLevel;
				item.DisableTag("XIAOWO_GHOST_IDENTITY_USED", true);
				if (flag)
				{
					GeneralHelper.SetItemOwned(item, true);
					if (!NativePlacement.Restore(item, parentInventory, shape))
					{
						EmporiumEntry.Instance.TryAddToPlayerInv(item);
					}
				}
				throw;
			}
			message = AuctionText.Select("身份记录已处理。调查进度归零，涉嫌犯罪金额：", "Identity records cleared. Investigation progress reset; current crime amount: ") + amounts.Sum() + " -> " + array.Sum() + "¢";
			Core.Log?.Msg("[幽灵身份主板] 已消耗一件，犯罪金额 " + amounts.Sum() + " -> " + array.Sum() + ", 证据进度 " + evidenceLevel + " -> 0。");
			return true;
		}

		internal static void Dispose()
		{
			if (_pending != null && (Object)(object)_dialog?.panel != (Object)null && _dialog.panel.activeInHierarchy)
			{
				_dialog.OnNoClick();
			}
			RestoreLabels(_labelDialog);
			_pending = null;
			_dialog = null;
			_confirmAction = (_cancelAction = null);
		}
	}
	[HarmonyPatch(typeof(ConfirmPanelUIManager), "OpenPanel")]
	internal static class AuctionGhostIdentityOpenDialogPatch
	{
		private static void Prefix(ConfirmPanelUIManager __instance)
		{
			AuctionGhostIdentity.RestoreLabels(__instance);
		}
	}
	[HarmonyPatch(typeof(ConfirmPanelUIManager), "OnYesClick")]
	internal static class AuctionGhostIdentityYesDialogPatch
	{
		private static void Prefix(ConfirmPanelUIManager __instance)
		{
			AuctionGhostIdentity.RestoreLabels(__instance);
		}
	}
	[HarmonyPatch(typeof(ConfirmPanelUIManager), "OnNoClick")]
	internal static class AuctionGhostIdentityNoDialogPatch
	{
		private static void Prefix(ConfirmPanelUIManager __instance)
		{
			AuctionGhostIdentity.RestoreLabels(__instance);
		}
	}
	[HarmonyPatch(typeof(ItemMouseDoubleClickHandler), "DoubleClickAction")]
	internal static class AuctionGhostIdentityDoubleClickPatch
	{
		private static bool Prefix(GameItem newItem)
		{
			if (((newItem != null) ? newItem.identifier : null) != "xiaowo_auction_ghost_identity")
			{
				return true;
			}
			try
			{
				AuctionGhostIdentity.RequestUse(newItem);
			}
			catch (Exception ex)
			{
				Core.Log?.Error("[幽灵身份主板] 双击失败: " + ex);
			}
			return false;
		}
	}
	internal static class AuctionItemLinks
	{
		internal const string VersionTag = "XIAOWO_AUCTION_NATIVE_LINK_V1";

		internal const string ChargesTag = "XIAOWO_CALIBRATOR_CHARGES";

		private static readonly Dictionary<string, GameItem> Templates = new Dictionary<string, GameItem>(StringComparer.Ordinal);

		private static readonly Dictionary<IntPtr, int> IndependentFeatures = new Dictionary<IntPtr, int>();

		internal static int TemplateCount => Templates.Count;

		internal static void ClearTemplates()
		{
			Templates.Clear();
			IndependentFeatures.Clear();
		}

		internal static bool IsLinked(string id)
		{
			return AuctionItemLinkRules.NativeTemplate(id) != null;
		}

		internal static bool NeedsRepair(GameItem item)
		{
			if (IsLinked(item.identifier))
			{
				return !item.IsTag("XIAOWO_AUCTION_NATIVE_LINK_V1");
			}
			return false;
		}

		internal static int Remaining(GameItem item)
		{
			int? obj;
			if (item == null)
			{
				obj = null;
			}
			else
			{
				TagState tagReadonly = item.GetTagReadonly("XIAOWO_CALIBRATOR_CHARGES");
				obj = ((tagReadonly != null) ? new int?(tagReadonly.GetInt()) : ((int?)null));
			}
			return Math.Clamp(obj ?? 3, 0, 3);
		}

		internal static long ValueFor(GameItem item, int fullValue)
		{
			if (!(((item != null) ? item.identifier : null) == "xiaowo_auction_calibrator"))
			{
				return fullValue;
			}
			return AuctionItemLinkRules.CalibratorValue(fullValue, Remaining(item));
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		internal static GameItem CreateTemplate(string nativeId, bool owned)
		{
			GameItem val = DirectoryMaster.Item((nativeId == "native:SynaprestInjector") ? "black_injector" : nativeId, owned);
			if (val == null || (nativeId != "junk" && val.unitValue <= 0))
			{
				throw new InvalidOperationException("Native auction interaction template missing: " + nativeId);
			}
			GeneralHelper.SetItemOwned(val, owned);
			return val;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		internal static void Ensure(GameItem item, bool newItem = false)
		{
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			string text = AuctionItemLinkRules.NativeTemplate(item.identifier);
			if (text == null)
			{
				return;
			}
			if (!Templates.TryGetValue(text, out var value))
			{
				value = (Templates[text] = CreateTemplate(text, owned: false));
			}
			CopyTargetCallbacks(item, value);
			((GameItemFunc)item).mayActivateSlotItemFunc = ((GameItemFunc)value).mayActivateSlotItemFunc;
			((GameItemFunc)item).canActivateSlotItemFunc = ((GameItemFunc)value).canActivateSlotItemFunc;
			((GameItemFunc)item).onActivateSlotItemFunc = ((GameItemFunc)value).onActivateSlotItemFunc;
			item.canUseOutsideCombat = value.canUseOutsideCombat;
			List<CombatAbilityEffect> list = new List<CombatAbilityEffect>();
			if (value.effects != null)
			{
				foreach (CombatAbilityEffect effect in value.effects)
				{
					list.Add((effect == null) ? null : new CombatAbilityEffect().CopyFrom(effect));
				}
			}
			item.effects = list;
			if (AuctionItemLinkRules.IsModule(item.identifier))
			{
				EnsureModule(item, value, newItem);
			}
			else if (newItem || (!item.IsTag("XIAOWO_AUCTION_NATIVE_LINK_V1") && (item.itemFeatures == null || item.itemFeatures.Count == 0)))
			{
				item.itemFeatures = new List<ItemFeature>();
				foreach (ItemFeature itemFeature in value.itemFeatures)
				{
					item.itemFeatures.Add(CloneFeature(itemFeature, item.uniqueId));
				}
			}
			EnsureIndependentFeatures(item);
			SetTag(item, "XIAOWO_AUCTION_NATIVE_LINK_V1", enabled: true);
			if (item.identifier == "xiaowo_auction_calibrator" && !item.IsTag("XIAOWO_CALIBRATOR_CHARGES"))
			{
				SetCharges(item, 3);
			}
		}

		private static void CopyTargetCallbacks(GameItem item, GameItem native)
		{
			((GameItemFunc)item).mayItemTargetItemFunc = ((GameItemFunc)native).mayItemTargetItemFunc;
			((GameItemFunc)item).canItemTargetItemFunc = ((GameItemFunc)native).canItemTargetItemFunc;
			((GameItemFunc)item).onItemTargetItemFunc = ((GameItemFunc)native).onItemTargetItemFunc;
			((GameItemFunc)item).mayThisTargetItemFunc = ((GameItemFunc)native).mayThisTargetItemFunc;
			((GameItemFunc)item).canThisTargetItemFunc = ((GameItemFunc)native).canThisTargetItemFunc;
			((GameItemFunc)item).onThisTargetItemFunc = ((GameItemFunc)native).onThisTargetItemFunc;
			((GameItemFunc)item).mayItemTargetThisFunc = ((GameItemFunc)native).mayItemTargetThisFunc;
			((GameItemFunc)item).canItemTargetThisFunc = ((GameItemFunc)native).canItemTargetThisFunc;
			((GameItemFunc)item).onItemTargetThisFunc = ((GameItemFunc)native).onItemTargetThisFunc;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void EnsureModule(GameItem item, GameItem native, bool newItem)
		{
			if (newItem || (!item.IsTag("XIAOWO_AUCTION_NATIVE_LINK_V1") && !item.IsTag("BONUS_PERCENTAGE_PERFORMANCE_INT") && !item.IsTag("BONUS_PERCENTAGE_EFFICIENCY_INT") && !item.IsTag("BONUS_PERCENTAGE_QUALITY_INT")))
			{
				foreach (KeyValuePair<string, TagState> item2 in native.state.dict)
				{
					bool flag = item2.Key.StartsWith("MODULE_", StringComparison.Ordinal) || item2.Key.StartsWith("BONUS_PERCENTAGE_", StringComparison.Ordinal) || item2.Key.StartsWith("TEMP_PERCENTAGE_", StringComparison.Ordinal);
					if (!flag)
					{
						bool flag2;
						switch (item2.Key)
						{
						case "SHIP_MODULE_TAG":
						case "EQUIPMENT_MODULE_TAG":
						case "ITEM_TYPE_STRING":
							flag2 = true;
							break;
						default:
							flag2 = false;
							break;
						}
						flag = flag2;
					}
					if (flag)
					{
						item.state.EnableTag(item2.Key).GetTag(item2.Key).Set(item2.Value);
						TagSystem modifiedState = item.modifiedState;
						if (modifiedState != null)
						{
							modifiedState.EnableTag(item2.Key).GetTag(item2.Key).Set(item2.Value);
						}
					}
				}
				WriteInt(item, "BONUS_PERCENTAGE_PERFORMANCE_INT", 0);
				WriteInt(item, "BONUS_PERCENTAGE_EFFICIENCY_INT", (item.identifier == "xiaowo_auction_quantum_controller") ? 10 : 0);
				WriteInt(item, "BONUS_PERCENTAGE_QUALITY_INT", (item.identifier == "xiaowo_auction_fabricator_core") ? 15 : 0);
				ModuleEffectHelper.ResetAllTempStat(item);
				if (newItem || item.itemFeatures == null || item.itemFeatures.Count == 0)
				{
					item.itemFeatures = new List<ItemFeature>();
					foreach (ItemFeature itemFeature in native.itemFeatures)
					{
						item.itemFeatures.Add(CloneFeature(itemFeature, item.uniqueId));
					}
				}
			}
			SetTag(item, "MODULE_TAG", enabled: true);
			((GameItemFunc)item).mayInventoryAddItemFunc = ((GameItemFunc)native).mayInventoryAddItemFunc;
			((GameItemFunc)item).mayInventoryRemoveItemFunc = ((GameItemFunc)native).mayInventoryRemoveItemFunc;
			((GameItemFunc)item).onSlotAddItemFunc = ((GameItemFunc)native).onSlotAddItemFunc;
			((GameItemFunc)item).onSlotRemoveItemFunc = ((GameItemFunc)native).onSlotRemoveItemFunc;
			((GameItemFunc)item).tooltipRichTextFunc = ((GameItemFunc)native).tooltipRichTextFunc;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static void EnsureIndependentFeatures(GameItem item)
		{
			if (IndependentFeatures.TryGetValue(ObjectIdentity.Key(item), out var value) && value == item.uniqueId)
			{
				return;
			}
			List<ItemFeature> list = new List<ItemFeature>();
			foreach (ItemFeature itemFeature in item.itemFeatures)
			{
				list.Add(CloneFeature(itemFeature, item.uniqueId));
			}
			item.itemFeatures = list;
			IndependentFeatures[ObjectIdentity.Key(item)] = item.uniqueId;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		private static ItemFeature CloneFeature(ItemFeature f, int parent)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			if (f == null)
			{
				return null;
			}
			ItemFeature val = (ItemFeature)AccessTools.Method(typeof(object), "MemberwiseClone", (Type[])null, (Type[])null).Invoke(f, null);
			val.parentItemUniqueId = parent;
			val.fakeCondition = CloneCondition(f.fakeCondition);
			val.realCondition = CloneCondition(f.realCondition);
			return val;
		}

		private static ItemCondition CloneCondition(ItemCondition c)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			if (c != null)
			{
				return new ItemCondition
				{
					display = c.display,
					identifier = c.identifier,
					modValue = c.modValue,
					modValueDisplay = c.modValueDisplay,
					isTransformative = c.isTransformative,
					category = c.category,
					hiddenAsPublic = c.hiddenAsPublic,
					customValue = c.customValue,
					onActivateActionId = c.onActivateActionId,
					compareValue = c.compareValue
				};
			}
			return null;
		}

		internal static void WriteInt(GameItem item, string tag, int value)
		{
			item.state.EnableTag(tag).GetTag(tag).SetInt(value);
			TagSystem modifiedState = item.modifiedState;
			if (modifiedState != null)
			{
				modifiedState.EnableTag(tag).GetTag(tag).SetInt(value);
			}
		}

		internal static void SetTag(GameItem item, string tag, bool enabled)
		{
			if (enabled)
			{
				item.state.EnableTag(tag);
				TagSystem modifiedState = item.modifiedState;
				if (modifiedState != null)
				{
					modifiedState.EnableTag(tag);
				}
			}
			else
			{
				item.state.DisableTag(tag);
				TagSystem modifiedState2 = item.modifiedState;
				if (modifiedState2 != null)
				{
					modifiedState2.DisableTag(tag);
				}
			}
		}

		internal static void SetCharges(GameItem item, int value)
		{
			WriteInt(item, "XIAOWO_CALIBRATOR_CHARGES", Math.Clamp(value, 0, 3));
			item.SetValue(ValueFor(item, AuctionExclusiveCatalog.Items[item.identifier].Value));
			AuctionExclusiveFactory.RefreshLocalizedText(item);
		}

		internal static bool Owned(GameItem item)
		{
			if (item != null)
			{
				return item.IsTag("IS_OWNED_TAG");
			}
			return false;
		}

		internal static bool RepairTarget(GameItem source, GameItem target)
		{
			if (Owned(source) && Owned(target) && ObjectIdentity.Key(source) != ObjectIdentity.Key(target) && (target.IsTag("MODULE_TAG") || target.identifier == "module_extractor"))
			{
				return ModuleHelper.CanElectronicPartTarget(source, target);
			}
			return false;
		}

		internal static bool CalibrationTarget(GameItem source, GameItem target)
		{
			if (Owned(source) && Owned(target) && ObjectIdentity.Key(source) != ObjectIdentity.Key(target) && Remaining(source) > 0 && target.IsTag("MODULE_TAG"))
			{
				return target.IsTag("MODULE_STUCK_TAG");
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		internal static void Calibrate(GameItem source, GameItem target)
		{
			if (!CalibrationTarget(source, target))
			{
				return;
			}
			ModuleHelper.SetModuleUnstuck(target);
			if (!target.IsTag("MODULE_STUCK_TAG"))
			{
				SetCharges(source, Remaining(source) - 1);
				if (Remaining(source) == 0)
				{
					source.Destroy();
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		internal static int ShowcaseBonus()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Scene activeScene = SceneManager.GetActiveScene();
			if (((Scene)(ref activeScene)).name != "InventoryScene")
			{
				return 0;
			}
			EmporiumEntry instance = EmporiumEntry.Instance;
			GameGridInventory val = ((instance != null) ? instance.showcaseElement : null);
			if (val == null)
			{
				return 0;
			}
			List<string> list = new List<string>();
			foreach (GameItem childItem in ((GameInventory)val).childItems)
			{
				if (ObjectIdentity.Key((childItem != null) ? childItem.parentInventory : null) == ObjectIdentity.Key(val) && Owned(childItem))
				{
					list.Add(childItem.identifier);
				}
			}
			return AuctionItemLinkRules.DisplayBonus(list);
		}
	}
	[HarmonyPatch(typeof(GameItem), "MayTarget")]
	internal static class AuctionItemLinksMayTargetPatch
	{
		private static bool Prefix(GameItem __instance, GameItem targetItem, ref bool __result)
		{
			return AuctionItemLinksTargetGate.Check(__instance, targetItem, ref __result);
		}
	}
	[HarmonyPatch(typeof(GameItem), "CanTarget")]
	internal static class AuctionItemLinksCanTargetPatch
	{
		private static bool Prefix(GameItem __instance, GameItem targetItem, ref bool __result)
		{
			return AuctionItemLinksTargetGate.Check(__instance, targetItem, ref __result);
		}
	}
	internal static class AuctionItemLinksTargetGate
	{
		internal static bool Check(GameItem source, GameItem target, ref bool result)
		{
			if (((source != null) ? source.identifier : null) == "xiaowo_auction_calibrator")
			{
				result = AuctionItemLinks.CalibrationTarget(source, target);
			}
			else
			{
				if (!AuctionItemLinkRules.IsRepairMaterial((source != null) ? source.identifier : null))
				{
					return true;
				}
				result = AuctionItemLinks.RepairTarget(source, target);
			}
			return false;
		}
	}
	[HarmonyPatch(typeof(GameItem), "Target")]
	internal static class AuctionItemLinksTargetPatch
	{
		private static bool Prefix(GameItem __instance, GameItem targetItem)
		{
			if (((__instance != null) ? __instance.identifier : null) == "xiaowo_auction_calibrator")
			{
				AuctionItemLinks.Calibrate(__instance, targetItem);
				return false;
			}
			if (AuctionItemLinkRules.IsRepairMaterial((__instance != null) ? __instance.identifier : null))
			{
				return AuctionItemLinks.RepairTarget(__instance, targetItem);
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "GetCurrentStoreAttractiveness")]
	internal static class AuctionShowcaseAttractionPatch
	{
		[HarmonyPriority(0)]
		private static void Postfix(PlayerStore __instance, ref int __result)
		{
			if (Core.StoreReady && __instance == PlayerStore.Instance)
			{
				__result += AuctionItemLinks.ShowcaseBonus();
			}
		}
	}
	internal static class AuctionMerchantRelations
	{
		private const string PriceTag = "XIAOWO_AUCTION_RELATION_PRICE_";

		private const string GivenTag = "XIAOWO_AUCTION_GIFT_GIVEN";

		private static bool _committing;

		private static ConditionalWeakTable<Dialogue, object> _pages = new ConditionalWeakTable<Dialogue, object>();

		private static RectTransform _hud;

		private static Image _fill;

		private static TextMeshProUGUI _level;

		private static Canvas _canvas;

		private static int _nextHudText;

		private static Sprite _headSprite;

		private static Vector3 _headLocal;

		internal static bool IsInteractionActive
		{
			get
			{
				PlayerStore existing = NativeStore.Existing;
				object client;
				if (existing == null)
				{
					client = null;
				}
				else
				{
					StoreClientInstance currentClientInstance = existing.currentClientInstance;
					client = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
				}
				object value;
				if (Context((StoreClient)client, out var _) && (Object)(object)DialogUIManager.Instance != (Object)null && DialogUIManager.Instance.currentDialog != null)
				{
					return _pages.TryGetValue(DialogUIManager.Instance.currentDialog, out value);
				}
				return false;
			}
		}

		private static string Unavailable => T("商人已离开或存档已切换，赠礼未执行。", "The buyer left or the save changed. No gift was given.");

		private static string Key(ContainerCategory category)
		{
			int num = (int)category;
			return "merchant_gifts_" + num;
		}

		internal static long Gifts(ContainerCategory category)
		{
			if (!long.TryParse(AuctionState.Read(Key(category)), out var result))
			{
				return 0L;
			}
			return Math.Max(0L, result);
		}

		internal static int Budget(ContainerCategory category)
		{
			return AuctionMerchantRules.Budget(Gifts(category));
		}

		internal static string TierName(ContainerCategory category)
		{
			return TierName(AuctionMerchantRules.Tier(Gifts(category)));
		}

		private static string T(string zh, string en)
		{
			return AuctionText.Select(zh, en);
		}

		internal static string TierName(int tier)
		{
			return tier switch
			{
				1 => T("熟客", "Regular"), 
				2 => T("合作伙伴", "Partner"), 
				3 => T("核心伙伴", "Trusted Partner"), 
				_ => T("尚未建立合作", "New Contact"), 
			};
		}

		internal static string Summary(ContainerCategory category)
		{
			long gifts = Gifts(category);
			return TierName(category) + T(" · 收购加成 +", " · Buying premium +") + AuctionMerchantRules.Premium(gifts) + T("% · 赠礼 ", "% · Gifts ") + gifts.ToString("N0") + "¢";
		}

		internal static void ApplyPrice(StoreClient client, ContainerCategory category)
		{
			int num = 0;
			List<string> list = new List<string>();
			if (client.tag != null)
			{
				for (int num2 = client.tag.Count - 1; num2 >= 0; num2--)
				{
					string text = client.tag[num2];
					if (text != null && text.StartsWith("XIAOWO_AUCTION_RELATION_PRICE_", StringComparison.Ordinal))
					{
						if (int.TryParse(client.tag[num2].Substring("XIAOWO_AUCTION_RELATION_PRICE_".Length), out var result))
						{
							num += result;
						}
						list.Add(client.tag[num2]);
					}
				}
			}
			int num3 = AuctionMerchantRules.Premium(Gifts(category));
			if (list.Count == 1 && list[0] == "XIAOWO_AUCTION_RELATION_PRICE_" + num3)
			{
				return;
			}
			foreach (string item in list)
			{
				client.tag.Remove(item);
			}
			client.sellPriceModifier = checked(client.sellPriceModifier - num + num3);
			client.AddTag("XIAOWO_AUCTION_RELATION_PRICE_" + num3);
		}

		private static GameItem[] CounterItems()
		{
			EmporiumEntry instance = EmporiumEntry.Instance;
			GameGridInventory counter = ((instance != null) ? instance.backInvinvElement : null);
			if (counter == null)
			{
				return Array.Empty<GameItem>();
			}
			return (from i in ((GameInventory)counter).childItems.ToArray()
				where i != null && GeneralHelper.IsItemOwned(i) && ObjectIdentity.Key(i.parentInventory) == ObjectIdentity.Key(counter)
				group i by ObjectIdentity.Key(i) into g
				select g.First()).ToArray();
		}

		internal static bool AcceptsGift(ContainerCategory category, GameItem item)
		{
			if (item != null && !item.IsTag("XIAOWO_AUCTION_GIFT_GIVEN") && GeneralHelper.IsItemOwned(item) && item.unitCount >= 1 && !(item.identifier == "xiaowo_auction_phone_card") && !(item.identifier == "xiaowo_auction_container"))
			{
				List<GraphNodeStorage> children = item.children;
				if ((children == null || children.Count <= 0) && !item.IsTag("CONTAINER_TAG") && !item.IsTag("BACKPACK_TAG"))
				{
					if (AuctionExclusiveCatalog.Items.TryGetValue(item.identifier ?? "", out var value))
					{
						return value.Category == category;
					}
					return AuctionMerchants.BuyingTags(category).Any((string tag) => item.IsTag(tag) || item.IsGameItemType(tag));
				}
			}
			return false;
		}

		internal static long GiftValue(GameItem item)
		{
			return Math.Max(0L, item.GetCurrentValue(false, false, false, false, 0L));
		}

		private static bool Context(StoreClient client, out ContainerCategory category)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			category = ContainerCategory.Medical;
			if (!Core.IsShuttingDown)
			{
				Scene activeScene = SceneManager.GetActiveScene();
				if (((Scene)(ref activeScene)).name == "InventoryScene" && NativeStore.Existing != null && NativeStore.Existing.isClientArrived && client != null)
				{
					StoreClientInstance currentClientInstance = NativeStore.Existing.currentClientInstance;
					if (ObjectIdentity.Key((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null) == ObjectIdentity.Key(client) && !client.isBusinessComplete)
					{
						return AuctionMerchants.TryCategory(client, out category);
					}
				}
			}
			return false;
		}

		internal static bool TryGive(StoreClient client, GameItem[] items, out string message)
		{
			message = T("赠礼未完成，请重新确认交易台和商人。", "Gift not given. Check the buyer and counter again.");
			if (_committing || !Context(client, out var category) || items == null || items.Length == 0 || items.Select((GameItem i) => ObjectIdentity.Key(i)).Distinct().Count() != items.Length)
			{
				return false;
			}
			long gifts = Gifts(category);
			if (AuctionMerchantRules.Tier(gifts) == 3)
			{
				message = T("已经是核心伙伴，无需继续赠礼。", "Already a Trusted Partner. No more gifts are needed.");
				return false;
			}
			GameItem[] counter = CounterItems();
			if (!items.All((GameItem i) => i != null && counter.Any((GameItem c) => ObjectIdentity.Key(c) == ObjectIdentity.Key(i)) && AcceptsGift(category, i)))
			{
				return false;
			}
			long[] source = items.Select(GiftValue).ToArray();
			if (source.Any((long v) => v <= 0) || source.Sum() <= 0)
			{
				return false;
			}
			long value = source.Sum();
			(GameItem, GameInventory, GridShape)[] array = items.Select((GameItem i) => (Item: i, Inventory: i.parentInventory, Shape: i.modifiedShape.Clone())).ToArray();
			List<GameItem> list = new List<GameItem>();
			_committing = true;
			try
			{
				foreach (GameItem val in items)
				{
					if (!val.parentInventory.Expel(val))
					{
						throw new InvalidOperationException("Gift could not leave its inventory.");
					}
					list.Add(val);
					GeneralHelper.SetItemOwned(val, false);
					val.EnableTag("XIAOWO_AUCTION_GIFT_GIVEN", true);
				}
				AuctionState.Write(Key(category), AuctionMerchantRules.AddGift(gifts, value).ToString());
			}
			catch (Exception ex)
			{
				AuctionState.Write(Key(category), gifts.ToString());
				(GameItem, GameInventory, GridShape)[] array2 = array;
				for (int num = 0; num < array2.Length; num++)
				{
					(GameItem Item, GameInventory Inventory, GridShape Shape) placement = array2[num];
					if (list.Any((GameItem i) => ObjectIdentity.Key(i) == ObjectIdentity.Key(placement.Item)))
					{
						placement.Item.DisableTag("XIAOWO_AUCTION_GIFT_GIVEN", true);
						GeneralHelper.SetItemOwned(placement.Item, true);
						if (!NativePlacement.Restore(placement.Item, placement.Inventory, placement.Shape))
						{
							EmporiumEntry.Instance.TryAddToPlayerInv(placement.Item);
						}
					}
				}
				Core.Log?.Error("Merchant gift rolled back: " + ex);
				return false;
			}
			finally
			{
				_committing = false;
			}
			foreach (GameItem item in list)
			{
				item.Destroy();
			}
			ApplyPrice(client, category);
			AuctionMerchants.RefreshDialogue(client);
			EmporiumEntry.Instance.Validate(true);
			message = T("赠礼完成。累计赠礼：", "Gift received. Total gifts: ") + gifts.ToString("N0") + " → " + Gifts(category).ToString("N0") + "¢\n" + Summary(category) + T("\n下次到访预算：", "\nNext visit budget: ") + Budget(category).ToString("N0") + "¢";
			Core.Log?.Msg("Auction merchant gift: " + category.ToString() + ", value=" + value + ", total=" + Gifts(category) + ".");
			return true;
		}

		internal static void Dispose()
		{
			DialogUIManager instance = DialogUIManager.Instance;
			if ((Object)(object)instance != (Object)null && instance.currentDialog != null && _pages.TryGetValue(instance.currentDialog, out var _))
			{
				instance.EndConversation();
			}
			DisposeHud();
			_pages = new ConditionalWeakTable<Dialogue, object>();
		}

		private static void Choice(Dialogue page, string caption, Func<Dialogue> next)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			Dialogue reply = new Dialogue().SetText("PLAYER", caption);
			_pages.Add(reply, new object());
			page.AddChoice(ItemDirectory.CreateEmptyItem((string)null), caption, "", reply, (Action)delegate
			{
				try
				{
					reply.SetNextDialogue(next());
				}
				catch (Exception ex)
				{
					Core.Log?.Error("Merchant relationship choice failed [" + caption + "]: " + ex);
					reply.SetNextDialogue(Line(T("本次操作未正常完成，请核对交易台和好感度后重试，详情已记录在日志中。", "The action did not finish normally. Check the counter and relationship before retrying. Details were logged.")));
				}
			}, (Func<bool>)null);
		}

		private static Dialogue Line(string text)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			Dialogue val = new Dialogue().SetText("CLIENT", text);
			_pages.Add(val, new object());
			return val;
		}

		internal static Dialogue ArrivalDialogue(StoreClient client, ContainerCategory category, Dialogue trade)
		{
			if (AuctionMerchantRules.Tier(Gifts(category)) < 3)
			{
				return Menu(client);
			}
			return trade;
		}

		internal static Dialogue Menu(StoreClient client)
		{
			PlayerStore store = NativeStore.Existing;
			long generation = AuctionLoadLifecycle.Generation;
			Dialogue obj = Line(T("欢迎合作。你想先赠送礼物、查看合作关系，还是直接交易？", "Good to see you. Give a gift, check our relationship, or trade?"));
			Choice(obj, T("赠送礼物", "Give a Gift"), () => (!Valid()) ? Line(Unavailable) : PrepareGift(client));
			Choice(obj, T("查看好感度", "View Relationship"), () => (!Valid()) ? Line(Unavailable) : Relationship(client));
			Choice(obj, T("开始交易", "Start Trading"), () => (!Valid()) ? Line(Unavailable) : Trade(client));
			return obj;
			bool Valid()
			{
				ContainerCategory category;
				if (generation == AuctionLoadLifecycle.Generation && store == NativeStore.Existing)
				{
					return Context(client, out category);
				}
				return false;
			}
		}

		private static Dialogue Trade(StoreClient client)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return new Dialogue().SetText("CLIENT", T("把要出售的货物放上称重交易台，我们照常交易。", "Put the goods for sale on the weighing counter. Let's trade as usual.")).SetEndAction((Action)delegate
			{
				if (Context(client, out var _) && ((IEnumerable<GameItem>)CounterItems()).Any((Func<GameItem, bool>)client.IsClientBuyingThisItem))
				{
					NegociationUIManager instance = NegociationUIManager.Instance;
					if (instance != null)
					{
						instance.OpenUI();
					}
				}
			});
		}

		private static Dialogue BackPage(StoreClient client, string text)
		{
			Dialogue val = Line(text);
			ContainerCategory category2;
			if (Context(client, out var category) && AuctionMerchantRules.Tier(Gifts(category)) < 3)
			{
				Choice(val, T("返回合作选项", "Back to Options"), () => (!Context(client, out category2)) ? Line(Unavailable) : Menu(client));
			}
			Choice(val, T("开始交易", "Start Trading"), () => (!Context(client, out category2)) ? Line(Unavailable) : Trade(client));
			return val;
		}

		private static Dialogue Relationship(StoreClient client)
		{
			if (!Context(client, out var category))
			{
				return Line(Unavailable);
			}
			long num = Gifts(category);
			int num2 = AuctionMerchantRules.Tier(num);
			string text = ((num2 < 3) ? (T("\n下一档还需赠礼：", "\nGifts needed for next level: ") + Math.Max(0L, AuctionMerchantRules.Threshold(num2 + 1) - num).ToString("N0") + "¢") : T("\n已满级，无需继续赠礼。", "\nMaximum level. No more gifts needed."));
			return BackPage(client, "LV " + num2 + " · " + Summary(category) + text + T("\n下次到访预算：", "\nNext visit budget: ") + Budget(category).ToString("N0") + "¢");
		}

		private static Dialogue PrepareGift(StoreClient client)
		{
			if (!Context(client, out var category))
			{
				return Line(Unavailable);
			}
			if (AuctionMerchantRules.Tier(Gifts(category)) >= 3)
			{
				return Relationship(client);
			}
			Dialogue obj = Line(T("请把要赠送的自有物品放到称重交易台，再选择“检查赠礼”。只接受", "Place the goods you own on the weighing counter, then choose Check Gift. I only accept ") + AuctionPhone.CategoryCaption(category) + T("。可以直接拖动物品；不会自动出售。容器、名片及其他类别不收。", ". You can drag items now; nothing is sold automatically. No containers, cards or other categories."));
			Choice(obj, T("检查赠礼", "Check Gift"), () => Confirmation(client));
			Choice(obj, T("返回合作选项", "Back to Options"), () => (!Context(client, out var _)) ? Line(Unavailable) : Menu(client));
			Choice(obj, T("开始交易", "Start Trading"), () => (!Context(client, out category2)) ? Line(Unavailable) : Trade(client));
			return obj;
		}

		private static Dialogue Confirmation(StoreClient client)
		{
			if (!Context(client, out var category))
			{
				return Line(Unavailable);
			}
			PlayerStore store = NativeStore.Existing;
			long generation = AuctionLoadLifecycle.Generation;
			GameItem[] items = CounterItems();
			ContainerCategory category2;
			if (items.Length == 0 || items.Any((GameItem i) => !AcceptsGift(category, i) || GiftValue(i) <= 0))
			{
				Dialogue obj = Line(T("交易台为空，或含有不接受的物品。请只放上对应类别的自有货物，再检查赠礼。物品没有消耗。", "The counter is empty or contains unsuitable goods. Place only matching goods you own and check again. Nothing was consumed."));
				Choice(obj, T("重新检查赠礼", "Check Again"), () => Confirmation(client));
				Choice(obj, T("返回合作选项", "Back to Options"), () => (!Context(client, out category2)) ? Line(Unavailable) : Menu(client));
				return obj;
			}
			(GameItem Item, int Count, long Value)[] snapshot = items.Select((GameItem i) => (Item: i, Count: i.unitCount, Value: GiftValue(i))).ToArray();
			long before = Gifts(category);
			long value = snapshot.Sum(((GameItem Item, int Count, long Value) i) => i.Value);
			long gifts = AuctionMerchantRules.AddGift(before, value);
			bool used = false;
			Dialogue obj2 = Line(T("确认赠送交易台上 ", "Give all ") + items.Length + T(" 件物品？\n赠礼价值：", " items on the counter?\nGift value: ") + value.ToString("N0") + "¢\n" + T("累计赠礼：", "Total gifts: ") + before.ToString("N0") + " → " + gifts.ToString("N0") + "¢\nLV " + AuctionMerchantRules.Tier(before) + " → LV " + AuctionMerchantRules.Tier(gifts) + T("\n确认后送出物品，不支付货款。", "\nConfirm to give the goods without payment."));
			Choice(obj2, T("确认赠送", "Confirm Gift"), delegate
			{
				if (used)
				{
					return Line(T("本次赠礼已处理，请勿重复确认。", "This gift has already been handled."));
				}
				used = true;
				if (generation != AuctionLoadLifecycle.Generation || store != NativeStore.Existing || !Context(client, out category2))
				{
					return Line(Unavailable);
				}
				GameItem[] current = CounterItems();
				if (Gifts(category) != before || current.Length != snapshot.Length || !snapshot.All(((GameItem Item, int Count, long Value) s) => current.Contains(s.Item) && s.Item.unitCount == s.Count && GiftValue(s.Item) == s.Value))
				{
					return BackPage(client, T("交易台发生变化，请重新检查赠礼。没有消耗物品。", "The counter changed. Check the gift again. Nothing was consumed."));
				}
				TryGive(client, items, out var message);
				return BackPage(client, message);
			});
			Choice(obj2, T("取消赠送", "Cancel Gift"), () => (!Context(client, out category2)) ? Line(Unavailable) : PrepareGift(client));
			return obj2;
		}

		internal static void Tick()
		{
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Expected O, but got Unknown
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			PlayerStore existing = NativeStore.Existing;
			object obj;
			if (existing == null)
			{
				obj = null;
			}
			else
			{
				StoreClientInstance currentClientInstance = existing.currentClientInstance;
				obj = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
			}
			StoreClient client = (StoreClient)obj;
			StoreClientMono instance = StoreClientMono.Instance;
			if (!Context(client, out var category) || (Object)(object)instance == (Object)null || (Object)(object)instance.image == (Object)null || !((Renderer)instance.image).enabled || instance.IsMoving())
			{
				if ((Object)(object)_hud != (Object)null)
				{
					((Component)_hud).gameObject.SetActive(false);
				}
				return;
			}
			if ((Object)(object)_hud == (Object)null)
			{
				CreateHud();
			}
			if ((Object)(object)_hud == (Object)null || (Object)(object)_canvas == (Object)null)
			{
				return;
			}
			((Component)_hud).gameObject.SetActive(true);
			Camera main = Camera.main;
			if ((Object)(object)main == (Object)null)
			{
				((Component)_hud).gameObject.SetActive(false);
				return;
			}
			Vector3 val = main.WorldToScreenPoint(HeadPosition(instance.image));
			if (val.z <= 0f)
			{
				((Component)_hud).gameObject.SetActive(false);
				return;
			}
			RectTransform val2 = (RectTransform)((Component)_canvas).transform;
			Camera val3 = (((int)_canvas.renderMode == 0) ? null : _canvas.worldCamera);
			Vector2 val4 = default(Vector2);
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(val2, Vector2.op_Implicit(val), val3, ref val4))
			{
				_hud.anchoredPosition = val4 + new Vector2(0f, 24f);
			}
			if (Time.frameCount >= _nextHudText)
			{
				_nextHudText = Time.frameCount + 10;
				ApplyPrice(client, category);
				long num = Gifts(category);
				int num2 = AuctionMerchantRules.Tier(num);
				int num3 = AuctionMerchantRules.Threshold(num2);
				int num4 = AuctionMerchantRules.Threshold(num2 + 1);
				_fill.fillAmount = ((num2 >= 3) ? 1f : Mathf.Clamp01((float)(num - num3) / (float)(num4 - num3)));
				((TMP_Text)_level).text = "LV " + num2 + " · " + TierName(num2) + ((num2 >= 3) ? T(" · 已满级", " · MAX") : ("  " + num.ToString("N0") + "/" + num4.ToString("N0")));
			}
		}

		private static Vector3 HeadPosition(SpriteRenderer renderer)
		{
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Expected O, but got Unknown
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			Sprite sprite = renderer.sprite;
			if ((Object)(object)sprite != (Object)(object)_headSprite)
			{
				_headSprite = sprite;
				? headLocal;
				if (!((Object)(object)sprite == (Object)null))
				{
					Bounds bounds = sprite.bounds;
					float x = ((Bounds)(ref bounds)).center.x;
					bounds = sprite.bounds;
					float y = ((Bounds)(ref bounds)).center.y;
					bounds = sprite.bounds;
					headLocal = new Vector3(x, y + ((Bounds)(ref bounds)).extents.y * 0.5f, 0f);
				}
				else
				{
					headLocal = Vector3.zero;
				}
				_headLocal = (Vector3)headLocal;
				if ((Object)(object)sprite != (Object)null)
				{
					RenderTexture val = null;
					Texture2D val2 = null;
					RenderTexture active = RenderTexture.active;
					try
					{
						Rect textureRect = sprite.textureRect;
						int num = Mathf.Max(1, Mathf.CeilToInt(96f * ((Rect)(ref textureRect)).height / ((Rect)(ref textureRect)).width));
						val = RenderTexture.GetTemporary(96, num, 0, (RenderTextureFormat)0);
						Graphics.Blit((Texture)(object)sprite.texture, val, new Vector2(((Rect)(ref textureRect)).width / (float)((Texture)sprite.texture).width, ((Rect)(ref textureRect)).height / (float)((Texture)sprite.texture).height), new Vector2(((Rect)(ref textureRect)).x / (float)((Texture)sprite.texture).width, ((Rect)(ref textureRect)).y / (float)((Texture)sprite.texture).height));
						RenderTexture.active = val;
						val2 = new Texture2D(96, num, (TextureFormat)4, false);
						val2.ReadPixels(new Rect(0f, 0f, 96f, (float)num), 0, 0);
						Color32[] pixels = val2.GetPixels32();
						int num2 = 96;
						int num3 = -1;
						int num4 = -1;
						for (int i = 0; i < num; i++)
						{
							for (int j = 0; j < 96; j++)
							{
								if (pixels[i * 96 + j].a >= 32)
								{
									num2 = Mathf.Min(num2, j);
									num3 = Mathf.Max(num3, j);
									num4 = Mathf.Max(num4, i);
								}
							}
						}
						if (num4 >= 0)
						{
							Vector2 textureRectOffset = sprite.textureRectOffset;
							_headLocal = new Vector3((textureRectOffset.x + (float)(num2 + num3 + 1) * 0.5f / 96f * ((Rect)(ref textureRect)).width - sprite.pivot.x) / sprite.pixelsPerUnit, (textureRectOffset.y + ((float)num4 + 1f) / (float)num * ((Rect)(ref textureRect)).height - sprite.pivot.y) / sprite.pixelsPerUnit, 0f);
						}
					}
					catch (Exception ex)
					{
						Core.Log?.Warning("Merchant head anchor fallback: " + ex.Message);
					}
					finally
					{
						RenderTexture.active = active;
						if ((Object)(object)val != (Object)null)
						{
							RenderTexture.ReleaseTemporary(val);
						}
						if ((Object)(object)val2 != (Object)null)
						{
							Object.Destroy((Object)(object)val2);
						}
					}
				}
			}
			Vector3 headLocal2 = _headLocal;
			if (renderer.flipX)
			{
				headLocal2.x = 0f - headLocal2.x;
			}
			if (renderer.flipY)
			{
				headLocal2.y = 0f - headLocal2.y;
			}
			return ((Component)renderer).transform.TransformPoint(headLocal2);
		}

		private static void CreateHud()
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected O, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Expected O, but got Unknown
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			_canvas = StoreUIManager.Instance?.primaryCanvas;
			if (!((Object)(object)_canvas == (Object)null))
			{
				GameObject val = new GameObject("XIAOWOAuctionMerchantRelationship", new Type[2]
				{
					typeof(RectTransform),
					typeof(CanvasGroup)
				});
				_hud = val.GetComponent<RectTransform>();
				((Transform)_hud).SetParent(((Component)_canvas).transform, false);
				RectTransform hud = _hud;
				RectTransform hud2 = _hud;
				RectTransform hud3 = _hud;
				Vector2 val2 = default(Vector2);
				((Vector2)(ref val2))..ctor(0.5f, 0.5f);
				hud3.pivot = val2;
				Vector2 anchorMin = (hud2.anchorMax = val2);
				hud.anchorMin = anchorMin;
				_hud.sizeDelta = new Vector2(176f, 32f);
				CanvasGroup component = val.GetComponent<CanvasGroup>();
				component.interactable = false;
				component.blocksRaycasts = false;
				AuctionUiStyle.RoundedImage(val, new Color(0.055f, 0.065f, 0.08f, 0.9f));
				GameObject obj = Object.Instantiate<GameObject>(CustomUIManager.Instance.labelPrefab, val.transform, false);
				((Object)obj).name = "Level";
				RectTransform component2 = obj.GetComponent<RectTransform>();
				((Transform)component2).SetParent(val.transform, false);
				component2.anchorMin = Vector2.zero;
				component2.anchorMax = Vector2.one;
				component2.offsetMin = new Vector2(6f, 10f);
				component2.offsetMax = new Vector2(-6f, -2f);
				_level = obj.GetComponentInChildren<TextMeshProUGUI>(true);
				obj.SetActive(true);
				((Graphic)_level).color = AuctionUiStyle.Gold;
				((Graphic)_level).raycastTarget = false;
				((TMP_Text)_level).enableAutoSizing = true;
				((TMP_Text)_level).fontSizeMin = 8f;
				((TMP_Text)_level).fontSizeMax = 12f;
				((TMP_Text)_level).enableWordWrapping = false;
				((TMP_Text)_level).alignment = (TextAlignmentOptions)514;
				Graphic[] componentsInChildren = obj.GetComponentsInChildren<Graphic>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].raycastTarget = false;
				}
				GameObject val4 = new GameObject("ProgressTrack", new Type[2]
				{
					typeof(RectTransform),
					typeof(Image)
				});
				RectTransform component3 = val4.GetComponent<RectTransform>();
				((Transform)component3).SetParent(val.transform, false);
				component3.anchorMin = new Vector2(0f, 0f);
				component3.anchorMax = new Vector2(1f, 0f);
				component3.offsetMin = new Vector2(6f, 4f);
				component3.offsetMax = new Vector2(-6f, 9f);
				((Graphic)val4.GetComponent<Image>()).color = new Color(0.19f, 0.21f, 0.25f, 1f);
				((Graphic)val4.GetComponent<Image>()).raycastTarget = false;
				GameObject val5 = new GameObject("Progress", new Type[2]
				{
					typeof(RectTransform),
					typeof(Image)
				});
				RectTransform component4 = val5.GetComponent<RectTransform>();
				((Transform)component4).SetParent(val4.transform, false);
				component4.anchorMin = Vector2.zero;
				component4.anchorMax = Vector2.one;
				anchorMin = (component4.offsetMax = Vector2.zero);
				component4.offsetMin = anchorMin;
				_fill = val5.GetComponent<Image>();
				Image fill = _fill;
				Sprite sprite = (val4.GetComponent<Image>().sprite = SpriteForProgress());
				fill.sprite = sprite;
				((Graphic)_fill).color = AuctionUiStyle.Gold;
				((Graphic)_fill).raycastTarget = false;
				_fill.type = (Type)3;
				_fill.fillMethod = (FillMethod)0;
				_fill.fillOrigin = 0;
				_nextHudText = 0;
			}
		}

		private static Sprite SpriteForProgress()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			return AuctionUiStyle.RoundedImage(((Component)_hud).gameObject, new Color(0.055f, 0.065f, 0.08f, 0.9f)).sprite;
		}

		private static void DisposeHud()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)_hud != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)_hud).gameObject);
			}
			_hud = null;
			_fill = null;
			_level = null;
			_canvas = null;
			_nextHudText = 0;
			_headSprite = null;
			_headLocal = Vector3.zero;
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "HandleAutoNegoUIDisplay")]
	internal static class AuctionMerchantGiftAutoNegotiationPatch
	{
		private static bool Prefix()
		{
			return !AuctionMerchantRelations.IsInteractionActive;
		}
	}
	[HarmonyPatch(typeof(StoreClientMono), "OnClick")]
	internal static class AuctionMerchantGiftNpcClickPatch
	{
		private static bool Prefix()
		{
			return !AuctionMerchantRelations.IsInteractionActive;
		}
	}
	internal static class AuctionMerchants
	{
		internal const string EventPrefix = "xiaowo_night_auction_merchant_";

		private const string ScheduleKey = "merchant_deferred_visits";

		internal static void HandleDailyVisitors(StoreClientManager manager)
		{
			PlayerStore instance = PlayerStore.Instance;
			int day = AuctionCatalog.Day;
			if (manager?.clientStack == null || instance == null || day <= 0)
			{
				return;
			}
			try
			{
				if (AuctionState.Read("merchant_deferred_visits").Length > 0)
				{
					AuctionState.Delete("merchant_deferred_visits");
				}
				for (int num = manager.clientStack.Count - 1; num >= 0; num--)
				{
					if (AuctionMerchantRules.IsLegacyVisit(manager.clientStack[num]?.eventSourceId))
					{
						manager.clientStack.RemoveAt(num);
					}
				}
			}
			catch (Exception ex)
			{
				Core.Log?.Error("Auction merchant scheduling failed: " + ex);
			}
		}

		private static void SaveSchedule(AuctionVisitSchedule schedule)
		{
			string text = schedule.Encode();
			if (AuctionState.Read("merchant_deferred_visits") != text)
			{
				AuctionState.Write("merchant_deferred_visits", text);
			}
		}

		private static bool TryVisit(StoreClient client, out int week, out int slot, out ContainerCategory category)
		{
			week = (slot = -1);
			category = ContainerCategory.Medical;
			if (client == null || client.eventSourceId?.StartsWith("xiaowo_night_auction_merchant_", StringComparison.Ordinal) != true)
			{
				return false;
			}
			string[] array = client.eventSourceId.Substring("xiaowo_night_auction_merchant_".Length).Split('_');
			if (array.Length != 2 || !int.TryParse(array[0], out week) || week < 0 || !int.TryParse(array[1], out slot) || slot < 0 || slot > 1)
			{
				return false;
			}
			foreach (ContainerCategory value in Enum.GetValues(typeof(ContainerCategory)))
			{
				if (client.identifier == ClientId(value))
				{
					category = value;
					return true;
				}
			}
			return false;
		}

		internal static void OnMerchantArrived(StoreClient client)
		{
			if (TryVisit(client, out var week, out var slot, out var category))
			{
				AuctionState.RecordMerchantVisit(week, slot, category);
				AuctionState.Write("merchant_arrived_day", AuctionCatalog.Day.ToString());
				AuctionVisitSchedule auctionVisitSchedule = AuctionVisitSchedule.Decode(AuctionState.Read("merchant_deferred_visits"));
				auctionVisitSchedule.Complete(week, slot);
				SaveSchedule(auctionVisitSchedule);
			}
		}

		internal static StoreClient CreateClient(StoreClientManager manager, ContainerCategory category, string eventSource)
		{
			StoreClient val = StoreClientList.CreateUpperLowerVisitorLuxury();
			if (val == null)
			{
				return null;
			}
			ConfigureClient(val, manager, category, eventSource);
			return val;
		}

		internal static void ConfigureClient(StoreClient client, StoreClientManager manager, ContainerCategory category, string eventSource)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (client == null)
			{
				return;
			}
			if (category == ContainerCategory.Contraband)
			{
				StoreClientFactionSetup.InitBlackMarket(client);
			}
			client.identifier = ClientId(category);
			client.eventSourceId = eventSource;
			client.displayName = DisplayName(category);
			client.clientIntent = (ClientIntent)1;
			int num = (client.clientCash = AuctionMerchantRelations.Budget(category));
			client.useClientBudget = true;
			client.OverrideBudget(num);
			client.dismissable = true;
			client.isMultiBuyDisabled = false;
			client.noAcceptingContraband = false;
			client.AcceptingContrabandOverride = category == ContainerCategory.Contraband;
			client.CanReturn = false;
			client.CanReturnAsBuyer = false;
			AuctionMerchantRelations.ApplyPrice(client, category);
			client.clientBuyingIdList?.Clear();
			client.clientBlackIdList?.Clear();
			client.clientBlackTagList?.Clear();
			client.clientBuyingTagList?.Clear();
			client.willAcceptItem = null;
			string[] array = BuyingTags(category);
			foreach (string item in array)
			{
				client.clientBuyingTagList?.Add(item);
			}
			foreach (AuctionExclusiveSpec value in AuctionExclusiveCatalog.Items.Values)
			{
				if (value.Category == category)
				{
					client.clientBuyingIdList?.Add(value.Id);
				}
			}
			string item2 = (client.spriteName = SelectUnusedSprite(manager, category));
			client.possibleSprites?.Clear();
			client.possibleSprites?.Add(item2);
			if (manager != null)
			{
				ApplyDialogue(client, category);
			}
		}

		internal static string ClientId(ContainerCategory category)
		{
			return "xiaowo_night_auction_merchant_" + category.ToString().ToLowerInvariant();
		}

		internal static bool TryCategory(StoreClient client, out ContainerCategory category)
		{
			foreach (ContainerCategory value in Enum.GetValues(typeof(ContainerCategory)))
			{
				if (client?.identifier == ClientId(value))
				{
					category = value;
					return true;
				}
			}
			category = ContainerCategory.Medical;
			return false;
		}

		internal static bool IsMatchingExclusive(StoreClient client, GameItem item)
		{
			if (client != null && item != null)
			{
				return IsMatchingExclusive(client.identifier, item.identifier);
			}
			return false;
		}

		internal static bool IsMatchingExclusive(string clientId, string itemId)
		{
			if (!AuctionExclusiveCatalog.Items.TryGetValue(itemId ?? string.Empty, out var value))
			{
				return false;
			}
			return string.Equals(clientId, ClientId(value.Category), StringComparison.Ordinal);
		}

		internal static string DisplayName(ContainerCategory category)
		{
			return category switch
			{
				ContainerCategory.Medical => AuctionText.Select("夜拍医疗采购员", "Night Auction Medic"), 
				ContainerCategory.Luxury => AuctionText.Select("夜拍藏品经纪", "Night Auction Curator"), 
				ContainerCategory.Contraband => AuctionText.Select("夜拍黑市掮客", "Night Auction Broker"), 
				_ => AuctionText.Select("夜拍工程承包商", "Night Auction Engineer"), 
			};
		}

		internal static string[] BuyingTags(ContainerCategory category)
		{
			return category switch
			{
				ContainerCategory.Medical => new string[1] { "MEDICAL" }, 
				ContainerCategory.Luxury => new string[3] { "LUXURY_ITEM", "ALCOHOL", "WINE_CONTAINER_TAG" }, 
				ContainerCategory.Contraband => new string[4] { "CONTRABAND", "CONTRABAND_ITEM_TAG", "WEAPON", "FIREARM" }, 
				_ => new string[6] { "MACHINE", "STANDARD_MACHINE_TAG", "MODULE", "MODULE_TAG", "MATERIAL", "TOOL" }, 
			};
		}

		private static string SelectUnusedSprite(StoreClientManager manager, ContainerCategory category)
		{
			List<string> list = SpriteCandidates(category);
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			string text = null;
			if (manager != null)
			{
				try
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
						obj = ((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.spriteName);
					}
					text = (string)obj;
				}
				catch
				{
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				hashSet.Add(text);
			}
			List<StoreClient> list2 = manager?.clientStack;
			if (list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					string text2 = list2[i]?.spriteName;
					if (!string.IsNullOrEmpty(text2))
					{
						hashSet.Add(text2);
					}
				}
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (!hashSet.Contains(list[num]))
				{
					return list[num];
				}
			}
			if (list.Count <= 0)
			{
				return FallbackSprite(category);
			}
			return list[list.Count - 1];
		}

		private static List<string> SpriteCandidates(ContainerCategory category)
		{
			List<string> list = new List<string>();
			if ((Object)(object)SpriteDict.Instance == (Object)null)
			{
				list.Add(FallbackSprite(category));
				return list;
			}
			switch (category)
			{
			case ContainerCategory.Medical:
				list.AddRange(SpriteDict.GetPool("maleMed", int.MaxValue));
				list.AddRange(SpriteDict.GetPool("femaleMed", int.MaxValue));
				break;
			case ContainerCategory.Luxury:
				list.AddRange(SpriteDict.GetPool("maleUpper", int.MaxValue));
				list.AddRange(SpriteDict.GetPool("femaleUpper", int.MaxValue));
				break;
			case ContainerCategory.Contraband:
				list.AddRange(SpriteDict.GetPool("maleMerc", int.MaxValue));
				list.AddRange(SpriteDict.GetPool("femaleMerc", int.MaxValue));
				break;
			default:
				list.AddRange(SpriteDict.GetPool("maleEng", int.MaxValue));
				list.AddRange(SpriteDict.GetPool("femaleEng", int.MaxValue));
				break;
			}
			if (list.Count == 0)
			{
				list.Add(FallbackSprite(category));
			}
			return list;
		}

		private static void AddEntries(List<string> target, List<SpriteEntry> entries)
		{
			if (entries == null)
			{
				return;
			}
			for (int i = 0; i < entries.Count; i++)
			{
				string text = entries[i]?.key;
				if (!string.IsNullOrWhiteSpace(text))
				{
					target.Add(text);
				}
			}
		}

		private static string FallbackSprite(ContainerCategory category)
		{
			return category switch
			{
				ContainerCategory.Medical => "femaleMed2", 
				ContainerCategory.Luxury => "femaleUpper8", 
				ContainerCategory.Contraband => "femaleMerc2", 
				_ => "femaleEng2", 
			};
		}

		internal static void RefreshDialogue(StoreClient client)
		{
			if (client == null)
			{
				return;
			}
			foreach (ContainerCategory value in Enum.GetValues(typeof(ContainerCategory)))
			{
				if (!(client.identifier != ClientId(value)))
				{
					RepairSavedBuyerFaction(client);
					AuctionMerchantRelations.ApplyPrice(client, value);
					client.displayName = DisplayName(value);
					ApplyDialogue(client, value);
					break;
				}
			}
		}

		internal static void RefreshLocalizedDialogue(StoreClientManager manager)
		{
			try
			{
				PlayerStore instance = PlayerStore.Instance;
				object client;
				if (instance == null)
				{
					client = null;
				}
				else
				{
					StoreClientInstance currentClientInstance = instance.currentClientInstance;
					client = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
				}
				RefreshDialogue((StoreClient)client);
			}
			catch (Exception ex)
			{
				Core.Log?.Warning("Auction merchant current dialogue refresh: " + ex.Message);
			}
			List<StoreClient> list = manager?.clientStack;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					RefreshDialogue(list[i]);
				}
			}
		}

		internal static void RepairSavedBuyerFactions(StoreClientManager manager)
		{
			PlayerStore instance = PlayerStore.Instance;
			object client;
			if (instance == null)
			{
				client = null;
			}
			else
			{
				StoreClientInstance currentClientInstance = instance.currentClientInstance;
				client = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
			}
			RepairSavedBuyerFaction((StoreClient)client);
			List<StoreClient> list = manager?.clientStack;
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					RepairSavedBuyerFaction(list[i]);
				}
			}
		}

		private static void RepairSavedBuyerFaction(StoreClient client)
		{
			if (client != null && client.identifier == ClientId(ContainerCategory.Contraband) && client.clientFaction != "FACTION_BLACK_MARKET")
			{
				StoreClientFactionSetup.InitBlackMarket(client);
			}
		}

		private static void ApplyDialogue(StoreClient client, ContainerCategory category)
		{
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Expected O, but got Unknown
			string text = client.GetBudget().ToString("N0") + "¢";
			string[] array = category switch
			{
				ContainerCategory.Medical => new string[3]
				{
					AuctionText.Select("拍卖场介绍我来的。我只收医疗用品和药物。", "The auction house sent me. I buy medical supplies and medicine."),
					AuctionText.Select("货要能用，来路不用解释。我的预算是 ", "If it works, I won't ask where it came from. My budget is ") + text + AuctionText.Select("。", "."),
					AuctionText.Select("把合适的东西放上交易台，我们按原来的规矩成交。", "Put suitable goods on the counter. We'll trade as usual.")
				}, 
				ContainerCategory.Luxury => new string[3]
				{
					AuctionText.Select("我替收藏圈看货，只对真正的奢侈品和好酒感兴趣。", "I buy for collectors. Only genuine luxuries and fine wine interest me."),
					AuctionText.Select("今晚带了 ", "I brought ") + text + AuctionText.Select("，成色好的东西不会压价。", ". I pay fairly for goods in good condition."),
					AuctionText.Select("把藏品放上交易台，让我看看它值不值得带走。", "Put the collectible on the counter. Let me see it.")
				}, 
				ContainerCategory.Contraband => new string[3]
				{
					AuctionText.Select("拍卖场说你手里有些不能见光的货。", "The auction house says you have goods best kept out of sight."),
					AuctionText.Select("违禁品、武器和非法药物我都收，预算 ", "I buy contraband, weapons and illegal drugs. Budget: ") + text + AuctionText.Select("。", "."),
					AuctionText.Select("别谈来源，只谈价格。把东西放上来。", "No questions about origins. Put the goods on the counter.")
				}, 
				_ => new string[3]
				{
					AuctionText.Select("我负责替工程队收设备、模组和材料。", "I buy equipment, modules and materials for an engineering crew."),
					AuctionText.Select("预算 ", "Budget: ") + text + AuctionText.Select("，只要东西还能工作就可以谈。", ". If it still works, we can talk."),
					AuctionText.Select("把工程货放上交易台，我会逐件验收。", "Put your engineering goods on the counter. I'll check each one.")
				}, 
			};
			ref string reference = ref array[2];
			reference = reference + "\n" + AuctionMerchantRelations.Summary(category);
			Dialogue val = (client.mainDialogue = new Dialogue());
			for (int i = 0; i < array.Length; i++)
			{
				val.SetText(client.displayName, array[i]);
				if (i + 1 < array.Length)
				{
					val = val.NextDialogue();
				}
			}
			client.mainDialogue = AuctionPhone.WrapIntroduction(client, AuctionMerchantRelations.ArrivalDialogue(client, category, client.mainDialogue));
		}

		private static bool ContainsEventSource(List<StoreClient> clients, string source)
		{
			if (clients == null)
			{
				return false;
			}
			for (int i = 0; i < clients.Count; i++)
			{
				if (string.Equals(clients[i]?.eventSourceId, source, StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}
	}
	[HarmonyPatch(typeof(StoreClient), "StartMainDialogue")]
	internal static class AuctionMerchantDialoguePatch
	{
		private static void Prefix(StoreClient __instance)
		{
			AuctionPhone.OnMerchantArrived(__instance);
			AuctionMerchants.OnMerchantArrived(__instance);
			AuctionMerchants.RefreshDialogue(__instance);
		}
	}
	[HarmonyPatch(typeof(StoreClient), "IsClientBuyingThisItem", new Type[]
	{
		typeof(GameItem),
		typeof(bool)
	})]
	internal static class AuctionMerchantExclusiveBuyingPatch
	{
		private static bool Prefix(StoreClient __instance, GameItem gameItem, ref bool __result)
		{
			if (!AuctionMerchants.IsMatchingExclusive(__instance, gameItem))
			{
				return true;
			}
			__result = true;
			return false;
		}
	}
	[HarmonyPatch(typeof(StoreClient), "IsClientRefusingItem", new Type[]
	{
		typeof(GameItem),
		typeof(bool)
	})]
	internal static class AuctionMerchantExclusiveRefusalPatch
	{
		private static bool Prefix(StoreClient __instance, GameItem gameItem, ref bool __result)
		{
			if (!AuctionMerchants.IsMatchingExclusive(__instance, gameItem))
			{
				return true;
			}
			__result = false;
			return false;
		}
	}
	internal static class AuctionNightMarket
	{
		private static bool _buying;

		private static string DayKey => "market_day_" + AuctionCatalog.Day;

		internal static AuctionMarketDay Current
		{
			get
			{
				if (NativeStore.Existing == null)
				{
					return null;
				}
				AuctionMarketDay auctionMarketDay = Read<AuctionMarketDay>(DayKey);
				if (!AuctionNightMarketRules.Valid(auctionMarketDay, AuctionCatalog.Day))
				{
					return null;
				}
				return auctionMarketDay;
			}
		}

		private static string T(string zh, string en)
		{
			return AuctionText.Select(zh, en);
		}

		private static string EventId(AuctionMarketEvent kind)
		{
			int num = (int)kind;
			return "xiaowo_night_auction:market_" + num;
		}

		private static string AdvisorId(AuctionVenue venue)
		{
			int num = (int)venue;
			return "xiaowo_night_auction:advisor_" + num;
		}

		private static string AdvisorKey(AuctionVenue venue)
		{
			int num = (int)venue;
			return "advisor_" + num;
		}

		private static TValue Read<TValue>(string key) where TValue : class
		{
			//IL_0024: Expected O, but got Unknown
			try
			{
				string text = AuctionState.Read(key);
				return string.IsNullOrEmpty(text) ? null : JsonConvert.DeserializeObject<TValue>(text);
			}
			catch (JsonException ex)
			{
				JsonException ex2 = ex;
				Core.Log?.Warning("[Night market] Invalid " + key + ": " + ((Exception)(object)ex2).Message);
				return null;
			}
		}

		internal static AuctionAdvisorContract Contract(AuctionVenue venue)
		{
			if (NativeStore.Existing == null)
			{
				return null;
			}
			AuctionAdvisorContract auctionAdvisorContract = Read<AuctionAdvisorContract>(AdvisorKey(venue));
			if (!AuctionNightMarketRules.Active(auctionAdvisorContract, venue, AuctionCatalog.Day))
			{
				return null;
			}
			return auctionAdvisorContract;
		}

		internal static void Register()
		{
			foreach (AuctionMarketEvent value in Enum.GetValues(typeof(AuctionMarketEvent)))
			{
				if (value != AuctionMarketEvent.None)
				{
					AuctionMarketEvent captured = value;
					ModHelper.AddAfterhourEvent("xiaowo_night_auction", EventId(value), "xiaowo_night_auction", (Func<string>)(() => Title(captured)), 1, (Func<string>)(() => Description(captured)), (IEnumerable<AfterhourEffect>)(object)new AfterhourEffect[1] { ModHelper.AfterhourEffect((Func<string>)(() => Effect(captured)), (string)null, 0f, (AfterhourEffectPolarity)((value != AuctionMarketEvent.Maintenance) ? 1 : 2)) }, 0f, (AfterhourEventTone?)null, (Func<bool>)null, (Action)null, (Action)null, (IEnumerable<AfterhourWeekday>)null, false);
				}
			}
			foreach (AuctionVenue value2 in Enum.GetValues(typeof(AuctionVenue)))
			{
				AuctionVenue captured2 = value2;
				ModHelper.AddAfterhourEvent("xiaowo_night_auction", AdvisorId(value2), "xiaowo_night_auction", (Func<string>)(() => AuctionRules.VenueName(captured2) + T("拍卖行顾问", " Advisor")), 3, (Func<string>)(() => T("已购买的情报顾问服务。", "Your purchased appraisal service.")), (IEnumerable<AfterhourEffect>)(object)new AfterhourEffect[1] { ModHelper.AfterhourEffect((Func<string>)(() => T("玩家情报费用优惠30%；与驻场优惠取较高者。", "Player intel costs 30% less; best discount applies.")), (string)null, 0f, (AfterhourEffectPolarity)1) }, 0f, (AfterhourEventTone?)null, (Func<bool>)null, (Action)null, (Action)null, (IEnumerable<AfterhourWeekday>)null, false);
			}
		}

		internal static void Unregister()
		{
			ModHelper.RemoveAfterhourEvents("xiaowo_night_auction");
			_buying = false;
		}

		internal static void EnsureToday()
		{
			if (!Core.StoreReady || NativeStore.Existing == null)
			{
				return;
			}
			AuctionMarketDay auctionMarketDay = Current;
			if (auctionMarketDay == null)
			{
				Enum.TryParse<AuctionMarketEvent>(AuctionState.Read("market_previous_random"), out var result);
				auctionMarketDay = AuctionNightMarketRules.Create(NativeStore.Existing.runID, AuctionCatalog.Day, result);
				if (!string.IsNullOrEmpty(AuctionState.Read("lots_v2_" + AuctionCatalog.Day)))
				{
					auctionMarketDay.Kind = AuctionMarketEvent.None;
				}
				AuctionState.Write(DayKey, JsonConvert.SerializeObject((object)auctionMarketDay));
				if (AuctionNightMarketRules.IsRandom(auctionMarketDay.Kind))
				{
					AuctionState.Write("market_previous_random", auctionMarketDay.Kind.ToString());
				}
			}
			foreach (AuctionMarketEvent value in Enum.GetValues(typeof(AuctionMarketEvent)))
			{
				if (value != AuctionMarketEvent.None)
				{
					if (value == auctionMarketDay.Kind)
					{
						ModHelper.StartAfterhourEvent(EventId(value), 1);
					}
					else if (ModHelper.IsAfterhourEventActive(EventId(value)))
					{
						ModHelper.EndAfterhourEvent(EventId(value));
					}
				}
			}
			foreach (AuctionVenue value2 in Enum.GetValues(typeof(AuctionVenue)))
			{
				AuctionAdvisorContract auctionAdvisorContract = Contract(value2);
				if (auctionAdvisorContract != null)
				{
					ModHelper.StartAfterhourEvent(AdvisorId(value2), auctionAdvisorContract.Ends - AuctionCatalog.Day + 1);
				}
				else if (ModHelper.IsAfterhourEventActive(AdvisorId(value2)))
				{
					ModHelper.EndAfterhourEvent(AdvisorId(value2));
				}
			}
		}

		internal static int EntryFee(AuctionVenue venue)
		{
			return AuctionNightMarketRules.EntryFee(Current, AuctionRules.EntryFee(venue));
		}

		internal static int IntelFee(AuctionLot lot, int kind, int original, bool rival)
		{
			return AuctionNightMarketRules.IntelFee(Current, original, kind, !rival && Contract(lot.Venue) != null);
		}

		internal static int MainPercent(ContainerCategory category)
		{
			return AuctionNightMarketRules.MainPercent(Current, category);
		}

		internal static int RivalLimit(AuctionLot lot, int rival, int original)
		{
			return AuctionNightMarketRules.RivalLimit(Current, rival, original);
		}

		internal static string PriceText(int original, int price)
		{
			if (original != price)
			{
				return original + " → " + price + "¢";
			}
			return price + "¢";
		}

		internal static string EntryText(AuctionVenue venue)
		{
			return PriceText(AuctionRules.EntryFee(venue), EntryFee(venue));
		}

		internal static string IntelText(AuctionLot lot, int kind)
		{
			return PriceText(AuctionEngine.BaseIntelCost(lot, kind), AuctionEngine.IntelCost(lot, kind));
		}

		internal static bool BuyAdvisor(AuctionVenue venue, out string message)
		{
			message = T("当前无法购买顾问服务。", "Advisor service is unavailable.");
			PlayerStore existing = NativeStore.Existing;
			if (!Core.StoreReady || existing == null || venue < AuctionVenue.Beginner || venue > AuctionVenue.Advanced || _buying)
			{
				return false;
			}
			if (Contract(venue) != null)
			{
				message = T("该档顾问服务仍有效，未重复扣款。", "This advisor contract is already active; no payment taken.");
				return false;
			}
			int num = AuctionNightMarketRules.AdvisorPrice(venue);
			if (existing.GetCash() < num)
			{
				message = T("现金不足，未购买顾问服务。", "Not enough cash; no advisor contract purchased.");
				return false;
			}
			_buying = true;
			string value = AuctionState.Read(AdvisorKey(venue));
			int cash = existing.GetCash();
			try
			{
				AuctionAdvisorContract auctionAdvisorContract = new AuctionAdvisorContract
				{
					Venue = (int)venue,
					Started = AuctionCatalog.Day,
					Ends = checked(AuctionCatalog.Day + 2),
					Paid = num
				};
				AuctionState.Write(AdvisorKey(venue), JsonConvert.SerializeObject((object)auctionAdvisorContract));
				if (!ModHelper.StartAfterhourEvent(AdvisorId(venue), 3))
				{
					throw new InvalidOperationException("Official advisor event could not start.");
				}
				existing.ModCash(-num, false);
				message = AuctionRules.VenueName(venue) + T("顾问已开通，情报费用优惠30%。有效至第", " advisor active: 30% off intel through day ") + auctionAdvisorContract.Ends + T("天（含当天）。", " inclusive.");
				return true;
			}
			catch
			{
				if (string.IsNullOrEmpty(value))
				{
					AuctionState.Delete(AdvisorKey(venue));
				}
				else
				{
					AuctionState.Write(AdvisorKey(venue), value);
				}
				existing.ModCash(cash - existing.GetCash(), false);
				ModHelper.EndAfterhourEvent(AdvisorId(venue));
				throw;
			}
			finally
			{
				_buying = false;
			}
		}

		internal static Dialogue BuildAdvisorDialogue(Dialogue serviceRoot)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			Dialogue val = new Dialogue().SetText(AuctionPhone.ContactName, T("拍卖行顾问：仅为所选场次提供情报优惠30%，购买当天计入3天有效期；与驻场优惠取较高者。", "Auction advisors: 30% off player intel in the selected venue for 3 days including today. Best discount applies."));
			Dialogue val2 = new Dialogue().SetText(AuctionPhone.ContactName, T("高级场顾问服务。服务不会预约商人。", "Advanced advisor services. This does not book a buyer."));
			AddAdvisorChoice(val, AuctionVenue.Beginner, val);
			AddAdvisorChoice(val, AuctionVenue.Intermediate, val);
			AuctionPhone.AddServiceChoice(val, T("高级场 / 返回", "Advanced / Back"), val2, null);
			AddAdvisorChoice(val2, AuctionVenue.Advanced, val);
			AuctionPhone.AddServiceChoice(val2, T("返回顾问列表", "Advisor List"), val, null);
			AuctionPhone.AddServiceChoice(val2, T("返回服务菜单", "Service Menu"), serviceRoot, null);
			return val;
		}

		private static void AddAdvisorChoice(Dialogue root, AuctionVenue venue, Dialogue list)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			AuctionAdvisorContract auctionAdvisorContract = Contract(venue);
			if (auctionAdvisorContract != null)
			{
				AuctionPhone.AddServiceChoice(root, AuctionRules.VenueName(venue) + T("：有效期剩余", ": active, ") + (auctionAdvisorContract.Ends - AuctionCatalog.Day + 1) + T("天", " days left"), AuctionPhone.Ending(T("服务仍然有效，不会重复购买。", "Your contract is active. No duplicate purchase.")), null);
				return;
			}
			int num = AuctionNightMarketRules.AdvisorPrice(venue);
			Dialogue val = new Dialogue().SetText(AuctionPhone.ContactName, AuctionRules.VenueName(venue) + T("顾问费用 ", " advisor: ") + num + T("¢。购买当天起3天，玩家情报费用优惠30%。仅适用于该场次；是否确认支付？", "¢ for 3 days including today. Player intel costs 30% less in this venue only. Confirm payment?"));
			Dialogue result = AuctionPhone.Ending("");
			AuctionPhone.AddServiceChoice(val, T("确认支付 ", "Pay ") + num + "¢", result, delegate
			{
				BuyAdvisor(venue, out var message);
				result.SetText(AuctionPhone.ContactName, message);
			});
			AuctionPhone.AddServiceChoice(val, T("取消", "Cancel"), list, null);
			AuctionPhone.AddServiceChoice(root, AuctionRules.VenueName(venue) + " · " + num + "¢", val, null);
		}

		internal static string Summary(AuctionVenue? venue = null)
		{
			AuctionMarketEvent auctionMarketEvent = Current?.Kind ?? AuctionMarketEvent.None;
			string text = ((auctionMarketEvent == AuctionMarketEvent.None) ? T("今晚无特殊行情", "No special market tonight") : (Title(auctionMarketEvent) + " · " + Effect(auctionMarketEvent)));
			if (venue.HasValue)
			{
				AuctionAdvisorContract auctionAdvisorContract = Contract(venue.Value);
				if (auctionAdvisorContract != null)
				{
					text = text + T(" | 顾问优惠30%，剩余", " | Advisor −30%, ") + (auctionAdvisorContract.Ends - AuctionCatalog.Day + 1) + T("天", "d left");
					goto IL_0163;
				}
			}
			if (!venue.HasValue)
			{
				foreach (AuctionVenue value in Enum.GetValues(typeof(AuctionVenue)))
				{
					AuctionAdvisorContract auctionAdvisorContract2 = Contract(value);
					if (auctionAdvisorContract2 != null)
					{
						text = text + " | " + AuctionRules.VenueName(value) + T("顾问", " advisor") + " " + (auctionAdvisorContract2.Ends - AuctionCatalog.Day + 1) + T("天", "d");
					}
				}
			}
			goto IL_0163;
			IL_0163:
			return text;
		}

		internal static string Title(AuctionMarketEvent kind)
		{
			return kind switch
			{
				AuctionMarketEvent.Appraiser => T("鉴定师驻场", "Appraiser on Site"), 
				AuctionMarketEvent.CategoryFocus => AuctionPhone.CategoryCaption((ContainerCategory)(Current?.Category ?? 0)) + T("专场", " Showcase"), 
				AuctionMarketEvent.AdmissionSale => T("入场优惠", "Admission Sale"), 
				AuctionMarketEvent.ManifestLeak => T("货单泄露", "Leaked Manifest"), 
				AuctionMarketEvent.CautiousBuyer => T("买家观望", "Cautious Buyer"), 
				AuctionMarketEvent.Maintenance => T("检测设备检修", "Scanner Maintenance"), 
				_ => T("普通拍卖夜", "Regular Auction Night"), 
			};
		}

		private static string Description(AuctionMarketEvent kind)
		{
			return kind switch
			{
				AuctionMarketEvent.CategoryFocus => T("本周分类专场；其余三类箱子照常开放。", "This week's category showcase; all four categories remain available."), 
				AuctionMarketEvent.ManifestLeak => T("一份货单提前流出，可在对应箱子查看两件真实货物。", "A leaked manifest reveals two real items in one marked container."), 
				AuctionMarketEvent.CautiousBuyer => T("一位买家收紧采购计划，但仍会独立判断和报价。", "One buyer is more cautious but still makes independent bids."), 
				AuctionMarketEvent.Maintenance => T("高级复鉴设备正在检修，其他情报服务正常开放。", "Advanced scanners need maintenance; other intel services remain available."), 
				_ => T("拍卖行今晚提供特别安排。", "Special arrangements at tonight's auction."), 
			};
		}

		private static string Effect(AuctionMarketEvent kind)
		{
			return kind switch
			{
				AuctionMarketEvent.Appraiser => T("玩家和AI情报费优惠20%", "20% off intel for you and AI"), 
				AuctionMarketEvent.CategoryFocus => T("对应箱主类别90%，混合10%；不保证盈利", "Featured box: 90% main, 10% mixed; no profit guarantee"), 
				AuctionMarketEvent.AdmissionSale => T("入场费优惠20%，新手场免费", "20% off entry; beginner remains free"), 
				AuctionMarketEvent.ManifestLeak => T("标记箱免费公开2件货物及单件价值", "Two free item reveals in the marked box"), 
				AuctionMarketEvent.CautiousBuyer => T("一位AI心理价上限降低15%", "One AI's bidding limit is 15% lower"), 
				AuctionMarketEvent.Maintenance => T("仅高级复鉴费增加20%", "Advanced reappraisal costs 20% more"), 
				_ => "", 
			};
		}

		internal static string[] LeakItems(AuctionLot lot)
		{
			AuctionMarketDay current = Current;
			if (current == null || current.Kind != AuctionMarketEvent.ManifestLeak || current.LeakSlot != lot.Slot)
			{
				return Array.Empty<string>();
			}
			string text = "market_leak_" + current.Day + "_" + lot.Seed;
			List<string> list = Read<List<string>>(text);
			if (list != null && list.Count == Math.Min(2, lot.Contents.Count))
			{
				List<string> remaining = new List<string>(lot.Contents);
				if (list.All((string id) => remaining.Remove(id)))
				{
					return list.ToArray();
				}
			}
			List<int> list2 = Enumerable.Range(0, lot.Contents.Count).ToList();
			Random random = new Random(lot.Seed ^ current.Seed ^ 0x516E7);
			List<string> list3 = new List<string>();
			while (list2.Count > 0 && list3.Count < 2)
			{
				int index = random.Next(list2.Count);
				list3.Add(lot.Contents[list2[index]]);
				list2.RemoveAt(index);
			}
			AuctionState.Write(text, JsonConvert.SerializeObject((object)list3));
			return list3.ToArray();
		}
	}
	internal enum AuctionMarketEvent
	{
		None,
		Appraiser,
		CategoryFocus,
		AdmissionSale,
		ManifestLeak,
		CautiousBuyer,
		Maintenance
	}
	internal sealed class AuctionMarketDay
	{
		public int Version = 1;

		public int Day;

		public int Seed;

		public int Category;

		public int Rival;

		public int LeakSlot;

		public AuctionMarketEvent Kind;
	}
	internal sealed class AuctionAdvisorContract
	{
		public int Version = 1;

		public int Venue;

		public int Started;

		public int Ends;

		public int Paid;
	}
	internal static class AuctionNightMarketRules
	{
		internal static int Seed(string text)
		{
			int num = 17;
			string text2 = text ?? "";
			foreach (char c in text2)
			{
				num = num * 31 + c;
			}
			return num & 0x7FFFFFFF;
		}

		internal static AuctionMarketDay Create(string run, int day, AuctionMarketEvent previousRandom)
		{
			AuctionMarketDay auctionMarketDay = new AuctionMarketDay
			{
				Day = day,
				Seed = Seed(run + ":market:" + day)
			};
			Random random = new Random(auctionMarketDay.Seed);
			auctionMarketDay.Category = Math.Max(0, (day - 1) / 7) % 4;
			auctionMarketDay.Rival = random.Next(3);
			auctionMarketDay.LeakSlot = random.Next(4);
			AuctionMarketDay auctionMarketDay2 = auctionMarketDay;
			auctionMarketDay2.Kind = (day % 7) switch
			{
				2 => AuctionMarketEvent.Appraiser, 
				4 => AuctionMarketEvent.CategoryFocus, 
				6 => AuctionMarketEvent.AdmissionSale, 
				_ => AuctionMarketEvent.None, 
			};
			if (auctionMarketDay.Kind != AuctionMarketEvent.None || random.Next(100) >= 30)
			{
				return auctionMarketDay;
			}
			List<(AuctionMarketEvent, int)> list = new List<(AuctionMarketEvent, int)>();
			if (previousRandom != AuctionMarketEvent.ManifestLeak)
			{
				list.Add((AuctionMarketEvent.ManifestLeak, 45));
			}
			if (previousRandom != AuctionMarketEvent.CautiousBuyer)
			{
				list.Add((AuctionMarketEvent.CautiousBuyer, 35));
			}
			if (day >= 8 && previousRandom != AuctionMarketEvent.Maintenance)
			{
				list.Add((AuctionMarketEvent.Maintenance, 20));
			}
			int num = 0;
			foreach (var item in list)
			{
				num += item.Item2;
			}
			if (num == 0)
			{
				return auctionMarketDay;
			}
			int num2 = random.Next(num);
			foreach (var item2 in list)
			{
				if (num2 < item2.Item2)
				{
					(auctionMarketDay.Kind, _) = item2;
					break;
				}
				num2 -= item2.Item2;
			}
			return auctionMarketDay;
		}

		internal static bool IsRandom(AuctionMarketEvent kind)
		{
			if (kind >= AuctionMarketEvent.ManifestLeak)
			{
				return kind <= AuctionMarketEvent.Maintenance;
			}
			return false;
		}

		internal static bool Valid(AuctionMarketDay value, int day)
		{
			if (value != null && value.Version == 1 && value.Day == day && value.Kind >= AuctionMarketEvent.None && value.Kind <= AuctionMarketEvent.Maintenance && value.Category >= 0 && value.Category < 4 && value.Rival >= 0 && value.Rival < 3 && value.LeakSlot >= 0)
			{
				return value.LeakSlot < 4;
			}
			return false;
		}

		internal static bool Active(AuctionAdvisorContract value, AuctionVenue venue, int day)
		{
			if (value != null && value.Version == 1 && value.Venue == (int)venue && value.Started >= 0 && value.Ends == (long)value.Started + 2L && day >= value.Started && day <= value.Ends)
			{
				return value.Paid == AdvisorPrice(venue);
			}
			return false;
		}

		internal static int AdvisorPrice(AuctionVenue venue)
		{
			return venue switch
			{
				AuctionVenue.Beginner => 80, 
				AuctionVenue.Intermediate => 250, 
				_ => 900, 
			};
		}

		internal static int MainPercent(AuctionMarketDay day, ContainerCategory category)
		{
			if (day == null || day.Kind != AuctionMarketEvent.CategoryFocus || day.Category != (int)category)
			{
				return 80;
			}
			return 90;
		}

		internal static int EntryFee(AuctionMarketDay day, int original)
		{
			return Price(original, (day != null && day.Kind == AuctionMarketEvent.AdmissionSale) ? 20 : 0, 0);
		}

		internal static int IntelFee(AuctionMarketDay day, int original, int kind, bool advisor)
		{
			return Price(original, Math.Max((day != null && day.Kind == AuctionMarketEvent.Appraiser) ? 20 : 0, advisor ? 30 : 0), (day != null && day.Kind == AuctionMarketEvent.Maintenance && kind == 4) ? 20 : 0);
		}

		private static int Price(int original, int discount, int surcharge)
		{
			if (original > 0)
			{
				return (int)Math.Min(2147483647L, Math.Max(1L, ((long)original * (long)(100 + surcharge) * (100 - discount) + 9999) / 10000));
			}
			return 0;
		}

		internal static int RivalLimit(AuctionMarketDay day, int rival, int original)
		{
			if (day == null || day.Kind != AuctionMarketEvent.CautiousBuyer || day.Rival != rival)
			{
				return original;
			}
			return (int)((long)Math.Max(0, original) * 85L / 100);
		}
	}
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
	[HarmonyPatch(typeof(StorePhoneClient), "GetCallDialog")]
	internal static class AuctionPhoneDialoguePatch
	{
		private static bool Prefix(StorePhoneClient __instance, ref Dialogue __result)
		{
			if (__instance.dialogFuncId != "xiaowo_night_auction_phone")
			{
				return true;
			}
			__result = AuctionPhone.BuildDialogue();
			return false;
		}
	}
	[HarmonyPatch(typeof(StorePhoneClient), "GetLocalizedDisplayName")]
	internal static class AuctionPhoneNamePatch
	{
		private static bool Prefix(StorePhoneClient __instance, ref string __result)
		{
			if (__instance.dialogFuncId != "xiaowo_night_auction_phone")
			{
				return true;
			}
			__result = AuctionPhone.ContactName;
			return false;
		}
	}
	[HarmonyPatch(typeof(GameItem), "ActivateSlotItem")]
	internal static class AuctionPhoneCardActivatePatch
	{
		private static bool Prefix(GameItem __instance)
		{
			if (__instance.identifier != "xiaowo_auction_phone_card")
			{
				return true;
			}
			AuctionPhone.OpenCard();
			return false;
		}
	}
	[HarmonyPatch(typeof(RenderHandler), "LoadFromAtlas", new Type[]
	{
		typeof(string),
		typeof(string)
	})]
	internal static class AuctionPhoneCardSpritePatch
	{
		private static bool Prefix(string __0, string __1, ref Sprite __result)
		{
			if (__0 != "XIAOWO/NightAuctionPhone")
			{
				return true;
			}
			__result = AuctionPhone.Sprite(__1);
			return false;
		}
	}
	[HarmonyPatch(typeof(LocHelper), "GetLocalizedClientName")]
	internal static class AuctionPhoneNativeNamePatch
	{
		private static bool Prefix(string key, ref string __result)
		{
			if (key != "xiaowo_night_auction_phone")
			{
				return true;
			}
			__result = AuctionPhone.ContactName;
			return false;
		}
	}
	internal static class AuctionCommissions
	{
		internal static AuctionOrder Active => AuctionProgressEncoding.Decode<AuctionOrder>(AuctionState.Read("order_active"));

		private static HashSet<string> Used => AuctionProgressEncoding.Decode<HashSet<string>>(AuctionState.Read("orders_used"));

		internal static bool Available(AuctionOrder order)
		{
			return !Used.Contains(order.Id);
		}

		internal static List<AuctionOrder> Offers()
		{
			int day = AuctionCatalog.Day;
			int num = Math.Max(0, (day - 1) / 7);
			List<AuctionOrder> list = new List<AuctionOrder>();
			int category;
			for (category = 0; category < 4; category++)
			{
				Random random = new Random(AuctionPhoneRules.StableSeed((PlayerStore.Instance?.runID ?? "run") + ":order:" + num + ":" + category));
				AuctionOrder auctionOrder = new AuctionOrder
				{
					Id = num + "_" + category,
					Category = (ContainerCategory)category,
					Required = random.Next(3, 6),
					RewardCap = 2000
				};
				if (num % 3 == 2)
				{
					AuctionExclusiveSpec[] array = AuctionExclusiveCatalog.Items.Values.Where((AuctionExclusiveSpec spec) => spec.Category == (ContainerCategory)category && spec.Tier == 1).ToArray();
					auctionOrder.ItemId = array[random.Next(array.Length)].Id;
					auctionOrder.Required = 1;
					auctionOrder.RewardCap = 2500;
				}
				list.Add(auctionOrder);
			}
			return list;
		}

		internal static bool Accept(ContainerCategory category)
		{
			AuctionOrder active = Active;
			if (active.Id.Length > 0 && !active.Claimed && (AuctionOrderRules.Complete(active) || !AuctionOrderRules.Expired(active, AuctionCatalog.Day)))
			{
				return false;
			}
			AuctionOrder auctionOrder = Offers().First((AuctionOrder offer) => offer.Category == category);
			HashSet<string> used = Used;
			if (!used.Add(auctionOrder.Id))
			{
				return false;
			}
			auctionOrder.AcceptedDay = AuctionCatalog.Day;
			auctionOrder.Deadline = auctionOrder.AcceptedDay + 6;
			AuctionState.Write("orders_used", AuctionProgressEncoding.Encode(used));
			Save(auctionOrder);
			return true;
		}

		internal static void Save(AuctionOrder order)
		{
			AuctionState.Write("order_active", AuctionProgressEncoding.Encode(order));
		}

		internal static bool Claim()
		{
			AuctionOrder order = Active;
			if (!AuctionOrderRules.Complete(order) || order.Claimed)
			{
				return false;
			}
			order.Claimed = true;
			Save(order);
			return AuctionRewards.Pay(AuctionOrderRules.Reward(order), delegate
			{
				order.Claimed = false;
				Save(order);
			});
		}

		internal static void Cancel()
		{
			AuctionOrder active = Active;
			if (!AuctionOrderRules.Complete(active) || active.Claimed)
			{
				AuctionState.Delete("order_active");
			}
		}

		internal static void OnSale(StoreClient client, GameItem item, int paid)
		{
			if (client == null || item == null)
			{
				return;
			}
			AuctionOrder active = Active;
			if (!(client.identifier != AuctionMerchants.ClientId(active.Category)))
			{
				AuctionExclusiveSpec value;
				bool matchesCategory = (AuctionExclusiveCatalog.Items.TryGetValue(item.identifier, out value) ? (value.Category == active.Category) : AuctionMerchants.BuyingTags(active.Category).Any((string tag) => item.IsGameItemType(tag) || item.IsTag(tag)));
				if (AuctionOrderRules.Credit(active, AuctionCatalog.Day, active.Category, item.identifier, item.uniqueId, Math.Max(1, item.unitCount), paid, matchesCategory))
				{
					Save(active);
				}
			}
		}

		internal static string Target(AuctionOrder order)
		{
			if (order.ItemId.Length <= 0 || !AuctionExclusiveCatalog.Items.TryGetValue(order.ItemId, out var value))
			{
				return AuctionPhone.CategoryCaption(order.Category);
			}
			return AuctionCollection.Name(value);
		}

		internal static string Description(AuctionOrder order, bool active)
		{
			string text = Target(order) + "  " + (active ? (order.Progress + "/") : "") + order.Required + AuctionText.Select(" 件", " items");
			text = text + AuctionText.Select("\n售给：", "\nBuyer: ") + AuctionMerchants.DisplayName(order.Category);
			text = text + AuctionText.Select("\n额外奖励：有效成交金额的 20%，上限 ", "\nBonus: 20% of eligible sales, capped at ") + order.RewardCap + "¢";
			if (active)
			{
				text = text + AuctionText.Select("\n截止：第 ", "\nDeadline: day ") + order.Deadline + AuctionText.Select("天", "") + (AuctionOrderRules.Complete(order) ? AuctionText.Select(" · 已完成", " · Completed") : (AuctionOrderRules.Expired(order, AuctionCatalog.Day) ? AuctionText.Select(" · 已过期", " · Expired") : ""));
			}
			else if (order.ItemId.Length == 0)
			{
				text += AuctionText.Select("\n每件成交单价至少 100¢", "\nAt least 100¢ per unit");
			}
			return text;
		}

		internal static Dialogue DialoguePage(ContainerCategory first = ContainerCategory.Medical)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			AuctionOrder active = Active;
			Dialogue val = new Dialogue().SetText(AuctionPhone.ContactName, AuctionText.Select("本周采购委托", "Weekly Procurement Orders"));
			if (active.Id.Length > 0 && !active.Claimed && (AuctionOrderRules.Complete(active) || !AuctionOrderRules.Expired(active, AuctionCatalog.Day)))
			{
				val.SetText(AuctionPhone.ContactName, AuctionPhone.CategoryCaption(active.Category) + " " + active.Progress + "/" + active.Required + AuctionText.Select("，截止第 ", "; due day ") + active.Deadline + AuctionText.Select(" 天。", "."));
				if (AuctionOrderRules.Complete(active))
				{
					Dialogue response = AuctionPhone.Ending("");
					AuctionPhone.AddServiceChoice(val, AuctionText.Select("领取奖励 ", "Claim reward ") + AuctionOrderRules.Reward(active) + "¢", response, delegate
					{
						response.SetText(AuctionPhone.ContactName, Claim() ? AuctionText.Select("委托已结算。", "Order settled.") : AuctionText.Select("奖励已经领取。", "Reward already claimed."));
					});
				}
				else
				{
					Dialogue val2 = new Dialogue().SetText(AuctionPhone.ContactName, AuctionText.Select("取消后，本周这一单不能重新接取。", "A cancelled order cannot be accepted again this week."));
					AuctionPhone.AddServiceChoice(val2, AuctionText.Select("确认取消", "Confirm Cancel"), AuctionPhone.Ending(AuctionText.Select("委托已取消。", "Order cancelled.")), Cancel);
					AuctionPhone.AddServiceChoice(val2, AuctionText.Select("保留委托", "Keep Order"), AuctionPhone.Ending(""), null);
					AuctionPhone.AddServiceChoice(val, AuctionText.Select("取消委托", "Cancel Order"), val2, null);
				}
				AuctionPhone.AddServiceChoice(val, AuctionText.Select("结束通话", "End Call"), AuctionPhone.Ending(""), null);
				return val;
			}
			List<AuctionOrder> list = Offers();
			val.SetText(AuctionPhone.ContactName, AuctionText.Select("一次接取一单，7 天内完成，额外奖励 20%。", "One active order, 7 days to complete, 20% bonus."));
			for (int num = (int)first; num < (int)(first + 2); num++)
			{
				AuctionOrder order = list[num];
				if (Available(order))
				{
					Dialogue val3 = new Dialogue().SetText(AuctionPhone.ContactName, Target(order) + " ×" + order.Required + AuctionText.Select("，奖励上限 ", "; bonus cap ") + order.RewardCap + "¢");
					Dialogue response2 = AuctionPhone.Ending("");
					AuctionPhone.AddServiceChoice(val3, AuctionText.Select("接取委托", "Accept Order"), response2, delegate
					{
						response2.SetText(AuctionPhone.ContactName, Accept(order.Category) ? (AuctionText.Select("已接取，截止第 ", "Accepted; due day ") + Active.Deadline + AuctionText.Select(" 天。", ".")) : AuctionText.Select("该委托已接取，或已有其他委托。", "Order unavailable, or another order is active."));
					});
					AuctionPhone.AddServiceChoice(val3, AuctionText.Select("结束通话", "End Call"), AuctionPhone.Ending(""), null);
					AuctionPhone.AddServiceChoice(val, AuctionText.Select("查看：", "View: ") + AuctionPhone.CategoryCaption(order.Category), val3, null);
				}
			}
			AuctionPhone.AddServiceChoice(val, (first == ContainerCategory.Medical) ? AuctionText.Select("其他委托", "Other Orders") : AuctionText.Select("结束通话", "End Call"), (first == ContainerCategory.Medical) ? OtherPage() : AuctionPhone.Ending(AuctionText.Select("重新拨打可查看其他委托。", "Call again to see other orders.")), null);
			return val;
		}

		private static Dialogue OtherPage()
		{
			return DialoguePage(ContainerCategory.Contraband);
		}
	}
	internal static class AuctionRewards
	{
		internal static bool Pay(int amount, Action rollback)
		{
			int cash = PlayerStore.Instance.GetCash();
			try
			{
				if (amount > 0)
				{
					PlayerStore.Instance.ModCash(amount, true);
				}
				return true;
			}
			catch (Exception ex)
			{
				if (PlayerStore.Instance.GetCash() == cash)
				{
					rollback();
				}
				Core.Log?.Error("Auction reward payment failed: " + ex);
				return false;
			}
		}
	}
	internal static class AuctionCollection
	{
		private static int _tick;

		internal static HashSet<string> Found => AuctionProgressEncoding.Decode<HashSet<string>>(AuctionState.Read("collection_found"));

		private static HashSet<string> Claimed => AuctionProgressEncoding.Decode<HashSet<string>>(AuctionState.Read("collection_claimed"));

		internal static string Name(AuctionExclusiveSpec spec)
		{
			if (AuctionText.IsChinese(AuctionText.CurrentLocaleCode) || !AuctionExclusiveCatalog.English.TryGetValue(spec.Id, out (string, string) value))
			{
				return spec.Name;
			}
			return value.Item1;
		}

		internal static void Discover(string id)
		{
			if (AuctionExclusiveCatalog.IsExclusive(id) && PlayerStore.Instance != null)
			{
				HashSet<string> found = Found;
				if (found.Add(id))
				{
					AuctionState.Write("collection_found", AuctionProgressEncoding.Encode(found));
				}
			}
		}

		internal static void ScanOwned()
		{
			if (PlayerStore.Instance == null)
			{
				return;
			}
			HashSet<string> found = Found;
			bool flag = false;
			foreach (GameItem item in PlayerStore.Instance.FindAllItem(true))
			{
				if (item != null && AuctionExclusiveCatalog.IsExclusive(item.identifier) && GeneralHelper.IsItemEffectivelyOwned(item))
				{
					flag |= found.Add(item.identifier);
				}
			}
			if (flag)
			{
				AuctionState.Write("collection_found", AuctionProgressEncoding.Encode(found));
			}
		}

		internal static void Tick()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (_tick-- > 0)
			{
				return;
			}
			_tick = 600;
			Scene activeScene = SceneManager.GetActiveScene();
			if (((Scene)(ref activeScene)).name != "InventoryScene" || (Object)(object)EmporiumEntry.Instance == (Object)null)
			{
				return;
			}
			try
			{
				ScanOwned();
			}
			catch (Exception ex)
			{
				Core.Log?.Warning("Auction collection scan deferred: " + ex.Message);
			}
		}

		internal static int Count(ContainerCategory category)
		{
			return AuctionExclusiveCatalog.Items.Values.Count((AuctionExclusiveSpec spec) => spec.Category == category && Found.Contains(spec.Id));
		}

		internal static bool IsClaimed(int category)
		{
			return Claimed.Contains(category.ToString());
		}

		internal static bool CanClaim(int category)
		{
			if (category < 0 || category > 4 || Claimed.Contains(category.ToString()))
			{
				return false;
			}
			HashSet<string> found = Found;
			return AuctionExclusiveCatalog.Items.Values.Where((AuctionExclusiveSpec spec) => category == 4 || spec.Category == (ContainerCategory)category).All((AuctionExclusiveSpec spec) => found.Contains(spec.Id));
		}

		internal static bool Claim(int category)
		{
			if (!CanClaim(category))
			{
				return false;
			}
			HashSet<string> claimed = Claimed;
			claimed.Add(category.ToString());
			AuctionState.Write("collection_claimed", AuctionProgressEncoding.Encode(claimed));
			return AuctionRewards.Pay((category == 4) ? 5000 : 2000, delegate
			{
				claimed.Remove(category.ToString());
				AuctionState.Write("collection_claimed", AuctionProgressEncoding.Encode(claimed));
			});
		}

		internal static Dialogue DialoguePage(bool second = false)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			int num = (second ? 2 : 0);
			Dialogue val = new Dialogue().SetText(AuctionPhone.ContactName, string.Join(" / ", from i in Enumerable.Range(num, 2)
				select AuctionPhone.CategoryCaption((ContainerCategory)i) + " " + Count((ContainerCategory)i) + "/" + AuctionExclusiveCatalog.CategoryCount((ContainerCategory)i)));
			for (int num2 = num; num2 < num + 2; num2++)
			{
				int category = num2;
				Dialogue response = AuctionPhone.Ending("");
				AuctionPhone.AddServiceChoice(val, AuctionPhone.CategoryCaption((ContainerCategory)num2) + AuctionText.Select("领奖", " Reward"), response, delegate
				{
					response.SetText(AuctionPhone.ContactName, Claim(category) ? AuctionText.Select("奖励已领取：2000¢。", "Reward claimed: 2000¢.") : AuctionText.Select("尚未集齐，或奖励已领取。", "Not complete, or reward already claimed."));
				});
			}
			if (!second)
			{
				AuctionPhone.AddServiceChoice(val, AuctionText.Select("其他分类", "Other Categories"), DialoguePage(second: true), null);
			}
			else
			{
				Dialogue response2 = AuctionPhone.Ending("");
				AuctionPhone.AddServiceChoice(val, AuctionText.Select("全册领奖", "Full Book Reward"), response2, delegate
				{
					response2.SetText(AuctionPhone.ContactName, Claim(4) ? AuctionText.Select("奖励已领取：5000¢。", "Reward claimed: 5000¢.") : AuctionText.Select("尚未集齐，或奖励已领取。", "Not complete, or reward already claimed."));
				});
			}
			return val;
		}
	}
	internal static class AuctionRivals
	{
		internal static AuctionRivalProfile Profile(int rival)
		{
			return AuctionProgressEncoding.Decode<AuctionRivalProfile>(AuctionState.Read("rival_" + rival));
		}

		internal static void Prepare(AuctionLot lot)
		{
			if (lot.RivalMemoryApplied)
			{
				return;
			}
			if (!lot.Started)
			{
				for (int i = 0; i < 3; i++)
				{
					lot.MemoryModifiers[i] = AuctionRivalRules.Modifier(Profile(i), lot.Category, AuctionCatalog.Day, i);
				}
			}
			lot.RivalMemoryApplied = true;
		}

		internal static void Record(AuctionLot lot)
		{
			if (!lot.Finished || lot.RivalMemoryRecorded)
			{
				return;
			}
			string item = AuctionCatalog.Day + ":" + lot.Seed + ":" + lot.Slot;
			List<string> list = AuctionProgressEncoding.Decode<List<string>>(AuctionState.Read("rival_results"));
			if (!list.Contains(item))
			{
				for (int i = 0; i < 3; i++)
				{
					AuctionRivalProfile auctionRivalProfile = Profile(i);
					AuctionRivalRules.Remember(auctionRivalProfile, AuctionCatalog.Day, i, lot.HighestBidder, lot.CurrentBid, lot.ReferenceValue, lot.RivalExitReasons[i]);
					AuctionState.Write("rival_" + i, AuctionProgressEncoding.Encode(auctionRivalProfile));
				}
				list.Add(item);
				if (list.Count > 256)
				{
					list.RemoveRange(0, list.Count - 256);
				}
				AuctionState.Write("rival_results", AuctionProgressEncoding.Encode(list));
			}
			lot.RivalMemoryRecorded = true;
		}

		internal static string Mood(AuctionLot lot, int rival)
		{
			return AuctionEngine.RivalMood(lot, rival);
		}
	}
	[HarmonyPatch(typeof(StoreReputation), "OnItemTraded")]
	internal static class AuctionCommissionSalePatch
	{
		private static void Postfix(StoreClient storeClient, bool isPlayerSold, GameItem gameItem, int value)
		{
			if (!isPlayerSold)
			{
				return;
			}
			try
			{
				AuctionCollection.Discover((gameItem != null) ? gameItem.identifier : null);
				AuctionCommissions.OnSale(storeClient, gameItem, value);
			}
			catch (Exception ex)
			{
				Core.Log?.Error("Auction order sale tracking failed: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "OnItemBought")]
	internal static class AuctionCollectionPurchasePatch
	{
		private static void Postfix(GameItem gameItem)
		{
			try
			{
				AuctionCollection.Discover((gameItem != null) ? gameItem.identifier : null);
			}
			catch (Exception ex)
			{
				Core.Log?.Warning("Auction collection purchase tracking: " + ex.Message);
			}
		}
	}
	internal sealed class AuctionOrder
	{
		public string Id { get; set; } = "";

		public ContainerCategory Category { get; set; }

		public string ItemId { get; set; } = "";

		public int Required { get; set; }

		public int AcceptedDay { get; set; }

		public int Deadline { get; set; }

		public int Progress { get; set; }

		public int Turnover { get; set; }

		public int RewardCap { get; set; }

		public bool Claimed { get; set; }

		public HashSet<int> SaleIds { get; set; } = new HashSet<int>();
	}
	internal static class AuctionOrderRules
	{
		internal static bool Expired(AuctionOrder order, int day)
		{
			if (order != null)
			{
				return day > order.Deadline;
			}
			return true;
		}

		internal static bool Complete(AuctionOrder order)
		{
			if (order != null && order.Required > 0)
			{
				return order.Progress >= order.Required;
			}
			return false;
		}

		internal static int Reward(AuctionOrder order)
		{
			if (order != null)
			{
				return Math.Min(order.RewardCap, order.Turnover / 5);
			}
			return 0;
		}

		internal static bool Credit(AuctionOrder order, int day, ContainerCategory category, string itemId, int uniqueId, int units, int paid, bool matchesCategory)
		{
			if (order == null || order.Claimed || Complete(order) || Expired(order, day) || day < order.AcceptedDay || category != order.Category || paid <= 0 || units <= 0 || uniqueId <= 0 || order.SaleIds.Contains(uniqueId))
			{
				return false;
			}
			bool num;
			if (order.ItemId.Length <= 0)
			{
				if (!matchesCategory)
				{
					goto IL_007c;
				}
				num = paid / units < 100;
			}
			else
			{
				num = order.ItemId != itemId;
			}
			if (!num)
			{
				int num2 = Math.Min(units, order.Required - order.Progress);
				order.Progress += num2;
				order.Turnover = (int)Math.Min(2147483647L, order.Turnover + (long)paid * (long)num2 / units);
				order.SaleIds.Add(uniqueId);
				return true;
			}
			goto IL_007c;
			IL_007c:
			return false;
		}
	}
	internal sealed class AuctionRivalProfile
	{
		public int Heat { get; set; }

		public int LastDay { get; set; }

		public int Wins { get; set; }

		public int Losses { get; set; }
	}
	internal static class AuctionRivalRules
	{
		internal static int Modifier(AuctionRivalProfile profile, ContainerCategory category, int day, int rival)
		{
			return Math.Clamp(DecayedHeat(profile, day) + ((category == (ContainerCategory)(rival switch
			{
				1 => 2, 
				0 => 1, 
				_ => 3, 
			})) ? 6 : 0), -12, 18);
		}

		internal static void Remember(AuctionRivalProfile profile, int day, int rival, int winner, int price, int reference, int exitReason = 0)
		{
			if (winner >= 0 && winner <= 3)
			{
				profile.Heat = DecayedHeat(profile, day);
				if (winner == rival)
				{
					profile.Wins++;
					profile.Heat -= ((price >= (long)Math.Max(100, reference) * 120L / 100) ? 5 : 2);
				}
				else
				{
					profile.Losses++;
					profile.Heat += exitReason switch
					{
						0 => (winner != 3) ? 1 : 3, 
						3 => -2, 
						_ => -1, 
					};
				}
				profile.Heat = Math.Clamp(profile.Heat, -12, 12);
				profile.LastDay = day;
			}
		}

		private static int DecayedHeat(AuctionRivalProfile profile, int day)
		{
			int num = Math.Clamp(profile.Heat, -12, 12);
			int num2 = Math.Max(0, day - profile.LastDay) / 3;
			if (num <= 0)
			{
				return Math.Min(0, num + num2);
			}
			return Math.Max(0, num - num2);
		}
	}
	internal static class AuctionProgressEncoding
	{
		internal static string Encode<T>(T value)
		{
			return JsonConvert.SerializeObject((object)value);
		}

		internal static T Decode<T>(string value) where T : new()
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return new T();
			}
			try
			{
				T val = JsonConvert.DeserializeObject<T>(value);
				return (val != null) ? val : new T();
			}
			catch (JsonException)
			{
				return new T();
			}
		}
	}
	internal static class AuctionState
	{
		private const string Prefix = "xiaowo_night_auction.";

		internal static bool PurchasedToday
		{
			get
			{
				if (int.TryParse(Read("purchased_day"), out var result))
				{
					return result == AuctionCatalog.Day;
				}
				return false;
			}
		}

		private static string RivalFundsKey => "rival_funds_" + AuctionCatalog.Day;

		internal static int PendingDeliveryCount
		{
			get
			{
				int num = Read("pending_deliveries").Split(';').Count((string value) => !string.IsNullOrWhiteSpace(value));
				if (num <= 0)
				{
					return (!string.IsNullOrWhiteSpace(Read("pending_delivery"))) ? 1 : 0;
				}
				return num;
			}
		}

		internal static string Read(string suffix)
		{
			Dictionary<string, string> dictionary = NativeStore.Existing?.modData;
			if (dictionary == null)
			{
				return string.Empty;
			}
			if (dictionary.TryGetValue("xiaowo_night_auction:" + suffix, out var value))
			{
				return value;
			}
			if (!dictionary.TryGetValue("xiaowo_night_auction." + suffix, out var value2))
			{
				return string.Empty;
			}
			return value2;
		}

		internal static void Write(string suffix, string value)
		{
			if (NativeStore.Existing == null)
			{
				throw new InvalidOperationException("Player save is unavailable.");
			}
			ModHelper.SetModData("xiaowo_night_auction", suffix, value);
		}

		internal static void Delete(string suffix)
		{
			PlayerStore existing = NativeStore.Existing;
			if (existing != null)
			{
				ModHelper.RemoveModData("xiaowo_night_auction", suffix);
				existing.modData?.Remove("xiaowo_night_auction." + suffix);
			}
		}

		internal static void MigrateKeys()
		{
			PlayerStore existing = NativeStore.Existing;
			if (existing?.modData == null)
			{
				return;
			}
			KeyValuePair<string, string>[] array = existing.modData.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<string, string> keyValuePair = array[i];
				if (keyValuePair.Key.StartsWith("xiaowo_night_auction.", StringComparison.Ordinal))
				{
					string text = keyValuePair.Key.Substring("xiaowo_night_auction.".Length);
					if (!ModHelper.HasModData("xiaowo_night_auction", text))
					{
						ModHelper.SetModData("xiaowo_night_auction", text, keyValuePair.Value);
					}
					else if (ModHelper.GetModData("xiaowo_night_auction", text, (string)null) != keyValuePair.Value)
					{
						Core.Log?.Warning("[Save migration] Existing official value retained for key " + text + "; legacy value preserved.");
					}
				}
			}
		}

		internal static bool TryGetVenue(out AuctionVenue venue)
		{
			venue = AuctionVenue.Beginner;
			string[] array = Read("venue_" + AuctionCatalog.Day).Split(',');
			if (array.Length < 1 || !int.TryParse(array[0], out var result) || result < 0 || result > 2)
			{
				return false;
			}
			venue = (AuctionVenue)result;
			return true;
		}

		internal static bool EnterVenue(AuctionVenue venue, out string error)
		{
			AuctionNightMarket.EnsureToday();
			error = string.Empty;
			if (TryGetVenue(out var venue2))
			{
				if (venue2 == venue)
				{
					return true;
				}
				error = AuctionText.Select("今晚已经进入“", "Already entered ") + AuctionRules.VenueName(venue2) + AuctionText.Select("”，不能更换场次。", "; you cannot change venues tonight.");
				return false;
			}
			int num = AuctionNightMarket.EntryFee(venue);
			int cash = PlayerStore.Instance.GetCash();
			if (cash < num)
			{
				error = AuctionText.Select("现金不足，无法支付入场费。", "Not enough cash for the entry fee.");
				return false;
			}
			if (num > 0)
			{
				PlayerStore.Instance.ModCash(-num, true);
			}
			string suffix = "venue_" + AuctionCatalog.Day;
			int num2 = (int)venue;
			Write(suffix, num2 + "," + num);
			WriteRivalFunds(AuctionRivalFunds.Create(venue, cash, RivalFundsSeed(venue)));
			return true;
		}

		private static int RivalFundsSeed(AuctionVenue venue)
		{
			string[] obj = new string[5]
			{
				PlayerStore.Instance?.runID ?? "slot",
				":",
				AuctionCatalog.Day.ToString(),
				":rival_funds:",
				null
			};
			int num = (int)venue;
			obj[4] = num.ToString();
			return StableSeed(string.Concat(obj));
		}

		private static void WriteRivalFunds(AuctionRivalFunds funds)
		{
			Write(RivalFundsKey, funds.Encode());
		}

		private static AuctionRivalFunds EnsureRivalFunds(AuctionVenue venue, List<AuctionLot> lots)
		{
			AuctionRivalFunds auctionRivalFunds = AuctionRivalFunds.Decode(Read(RivalFundsKey));
			if (auctionRivalFunds != null && auctionRivalFunds.Venue == venue)
			{
				if (auctionRivalFunds.UpgradeLowAnchor(RivalFundsSeed(venue)))
				{
					foreach (AuctionLot lot in lots)
					{
						AuctionEngine.RefreshMigratedFunds(lot, auctionRivalFunds.Remaining);
					}
					WriteRivalFunds(auctionRivalFunds);
				}
				return auctionRivalFunds;
			}
			auctionRivalFunds = AuctionRivalFunds.Create(venue, PlayerStore.Instance.GetCash(), RivalFundsSeed(venue));
			auctionRivalFunds.AdoptLegacy(lots);
			WriteRivalFunds(auctionRivalFunds);
			return auctionRivalFunds;
		}

		internal static void StartLot(AuctionLot lot, List<AuctionLot> lots)
		{
			AuctionRivalFunds auctionRivalFunds = EnsureRivalFunds(lot.Venue, lots);
			AuctionEngine.Start(lot, auctionRivalFunds.Remaining);
		}

		internal static int RivalCash(AuctionLot lot, int index)
		{
			AuctionRivalFunds auctionRivalFunds = AuctionRivalFunds.Decode(Read(RivalFundsKey));
			if (auctionRivalFunds == null || auctionRivalFunds.Venue != lot.Venue)
			{
				return lot.Budgets[index];
			}
			return auctionRivalFunds.Remaining[index];
		}

		internal static int VenueFee(AuctionVenue venue)
		{
			string[] array = Read("venue_" + AuctionCatalog.Day).Split(',');
			if (!TryGetVenue(out var venue2) || venue2 != venue || array.Length <= 1 || !int.TryParse(array[1], out var result) || result < 0)
			{
				return 0;
			}
			return result;
		}

		internal static List<AuctionLot> LoadOrCreateLots(AuctionVenue venue)
		{
			AuctionNightMarket.EnsureToday();
			int day = AuctionCatalog.Day;
			List<AuctionLot> list = Decode(Read("lots_v2_" + day));
			if (list.Count == 4 && list.All((AuctionLot lot) => lot.Venue == venue))
			{
				string text = Read(RivalFundsKey);
				EnsureRivalFunds(venue, list);
				bool flag = Read(RivalFundsKey) != text;
				foreach (AuctionLot item in list)
				{
					if ((!item.Started && !item.Finished && !AuctionRules.IsRestrictedNeuralEligible(item.Venue, item.Category) && item.Contents.Contains("system_capped_neural_core")) || item.Contents.Count <= 0 || !AuctionCatalog.AreContentsAvailable(item.Contents))
					{
						item.Contents.Clear();
						item.Contents = AuctionCatalog.GenerateContents(item);
						item.InspectionItems.Clear();
						item.IntelMask &= -5;
						item.ItemCount = item.Contents.Count;
						item.EstimatedValue = Math.Max(100, AuctionCatalog.ContentsValue(item.Contents));
						flag = true;
					}
				}
				if (flag)
				{
					SaveLots(list);
				}
				return list;
			}
			string[] obj = new string[5]
			{
				PlayerStore.Instance?.runID ?? "slot",
				":",
				day.ToString(),
				":venue:",
				null
			};
			int num = (int)venue;
			obj[4] = num.ToString();
			Random random = new Random(StableSeed(string.Concat(obj)));
			List<ContainerCategory> list2 = new List<ContainerCategory>
			{
				ContainerCategory.Medical,
				ContainerCategory.Luxury,
				ContainerCategory.Contraband,
				ContainerCategory.Engineering
			};
			for (int num2 = list2.Count - 1; num2 > 0; num2--)
			{
				int num3 = random.Next(num2 + 1);
				List<ContainerCategory> list3 = list2;
				num = num2;
				int index = num3;
				ContainerCategory value = list2[num3];
				ContainerCategory value2 = list2[num2];
				list3[num] = value;
				list2[index] = value2;
			}
			List<AuctionLot> list4 = new List<AuctionLot>();
			for (int num4 = 0; num4 < 4; num4++)
			{
				int seed = random.Next(1, int.MaxValue);
				(int, int) tuple = AuctionRules.ItemCountRange(venue);
				int itemCount = random.Next(tuple.Item1, tuple.Item2 + 1);
				AuctionQuality auctionQuality = AuctionRules.RollQuality(random);
				if (auctionQuality == AuctionQuality.Hidden)
				{
					itemCount = tuple.Item2;
				}
				(int, int) tuple2 = AuctionRules.BaselineRange(venue);
				int num5 = random.Next(tuple2.Item1, tuple2.Item2 + 1);
				int targetValue = (int)Math.Min(2147483647L, Math.Max(100L, (long)num5 * (long)AuctionRules.RollValuePercent(auctionQuality, random) / 100));
				AuctionLot auctionLot = new AuctionLot
				{
					Slot = num4,
					Category = list2[num4],
					Venue = venue,
					Quality = auctionQuality,
					Seed = seed,
					ItemCount = itemCount,
					ReferenceValue = num5,
					TargetValue = targetValue
				};
				List<string> list5 = (auctionLot.Contents = AuctionCatalog.GenerateContents(auctionLot));
				auctionLot.ItemCount = list5.Count;
				auctionLot.EstimatedValue = Math.Max(100, AuctionCatalog.ContentsValue(list5));
				(int, int) tuple3 = AuctionRules.OpeningPercent(venue);
				auctionLot.CurrentBid = Math.Max(50, num5 * random.Next(tuple3.Item1, tuple3.Item2) / 100);
				list4.Add(auctionLot);
			}
			SaveLots(list4);
			return list4;
		}

		internal static void SaveLots(List<AuctionLot> lots)
		{
			if (lots.Count > 0)
			{
				AuctionRivalFunds auctionRivalFunds = EnsureRivalFunds(lots[0].Venue, lots);
				foreach (AuctionLot lot in lots)
				{
					auctionRivalFunds.Settle(lot);
				}
				WriteRivalFunds(auctionRivalFunds);
			}
			List<string> list = new List<string>();
			foreach (AuctionLot lot2 in lots)
			{
				AuctionRivals.Record(lot2);
				list.Add(string.Join(",", lot2.Slot, (int)lot2.Category, lot2.Seed, lot2.ItemCount, lot2.EstimatedValue, lot2.CurrentBid, lot2.Round, lot2.PlayerLeading ? 1 : 0, lot2.Finished ? 1 : 0, lot2.Lost ? 1 : 0, lot2.Started ? 1 : 0, lot2.Sealed ? 1 : 0, lot2.HighestBidder, lot2.ActiveMask, lot2.IntelMask, string.Join(":", lot2.Budgets), string.Join(":", lot2.LastOffers), Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join("\n", lot2.BidLog))), (int)lot2.Venue, (int)lot2.Quality, lot2.ReferenceValue, lot2.TargetValue, lot2.IntelSpent, Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join("\n", lot2.Contents))), lot2.OpeningBid, lot2.PlayerOffer, lot2.PendingOffer, lot2.BiddersShown, lot2.Withdrawn ? 1 : 0, string.Join(":", lot2.MemoryModifiers), lot2.RivalMemoryApplied ? 1 : 0, lot2.RivalMemoryRecorded ? 1 : 0, string.Join(":", lot2.RivalExitReasons), string.Join(":", lot2.RivalIntelTiers), string.Join(":", lot2.RivalIntelValues), string.Join(":", lot2.RivalIntelSpent), string.Join(":", lot2.RivalIntelRounds), Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join("\n", lot2.InspectionItems)))));
			}
			Write("lots_v2_" + AuctionCatalog.Day, string.Join(";", list));
		}

		internal static void RecordWin(AuctionLot lot)
		{
			int num = VenueFee(lot.Venue);
			long num2 = (long)lot.EstimatedValue - (long)lot.CurrentBid - lot.IntelSpent;
			string text = string.Join(",", AuctionCatalog.Day, (int)lot.Venue, (int)lot.Category, lot.Seed, lot.CurrentBid, lot.EstimatedValue, num2, num, lot.IntelSpent, 0);
			string text2 = Read("history");
			string[] array = text2.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(',');
				if (array2.Length >= 9 && array2[0] == AuctionCatalog.Day.ToString() && array2[3] == lot.Seed.ToString())
				{
					return;
				}
			}
			Write("history", string.IsNullOrEmpty(text2) ? text : (text2 + ";" + text));
		}

		internal static void CommitPurchase(AuctionLot lot, List<AuctionLot> lots, List<string> contents)
		{
			PlayerStore store = NativeStore.Existing ?? throw new InvalidOperationException("Player save is unavailable.");
			string text = $"purchase_{AuctionCatalog.Day}_{(int)lot.Venue}_{lot.Slot}_{lot.Seed}";
			if (!string.IsNullOrEmpty(Read(text)))
			{
				lot.Finished = true;
				lot.PlayerLeading = true;
				return;
			}
			if (lot.CurrentBid < 0 || store.GetCash() < lot.CurrentBid || contents.Count == 0)
			{
				throw new InvalidOperationException("Invalid auction purchase.");
			}
			PlayerStore val = store;
			if (val.modData == null)
			{
				val.modData = new Dictionary<string, string>();
			}
			string value;
			Dictionary<string, string> dictionary = new string[4]
			{
				"pending_deliveries",
				"lots_v2_" + AuctionCatalog.Day,
				"history",
				text
			}.Select((string k) => "xiaowo_night_auction:" + k).ToArray().ToDictionary((string k) => k, (string k) => (!store.modData.TryGetValue(k, out value)) ? null : value);
			int cash = store.GetCash();
			bool finished = lot.Finished;
			bool playerLeading = lot.PlayerLeading;
			try
			{
				SetPendingDelivery(lot.Category, lot.Seed, contents);
				lot.PlayerLeading = true;
				lot.Finished = true;
				SaveLots(lots);
				RecordWin(lot);
				Write(text, lot.CurrentBid.ToString());
				store.ModCash(-lot.CurrentBid, false);
			}
			catch
			{
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					if (item.Value == null)
					{
						store.modData.Remove(item.Key);
					}
					else
					{
						store.modData[item.Key] = item.Value;
					}
				}
				store.ModCash(cash - store.GetCash(), false);
				lot.Finished = finished;
				lot.PlayerLeading = playerLeading;
				throw;
			}
		}

		internal static void RevealHistory(int seed)
		{
			List<string> list = Read("history").Split(';').ToList();
			bool flag = false;
			for (int i = 0; i < list.Count; i++)
			{
				string[] array = list[i].Split(',');
				if (array.Length == 10 && !(array[3] != seed.ToString()) && !(array[9] == "1"))
				{
					array[9] = "1";
					list[i] = string.Join(",", array);
					flag = true;
				}
			}
			if (flag)
			{
				Write("history", string.Join(";", list));
			}
		}

		internal static List<string> History()
		{
			List<string> list = new List<string>();
			string[] array = Read("history").Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(',');
				int result2;
				int result3;
				if (array2.Length == 6 && int.TryParse(array2[1], out var result) && result >= 0 && result <= 3)
				{
					list.Add(AuctionText.Select("第", "Day ") + array2[0] + AuctionText.Select("天  ", "  ") + AuctionCatalog.CategoryName((ContainerCategory)result) + AuctionText.Select("  成交 ", "  Bid ") + array2[3] + AuctionText.Select("  货值 ", "  Value ") + array2[4] + AuctionText.Select("  盈亏 ", "  Profit ") + array2[5]);
				}
				else if ((array2.Length == 9 || array2.Length == 10) && int.TryParse(array2[1], out result2) && result2 >= 0 && result2 <= 2 && int.TryParse(array2[2], out result3) && result3 >= 0 && result3 <= 3)
				{
					bool flag = array2.Length == 9 || array2[9] == "1";
					list.Add(AuctionText.Select("第", "Day ") + array2[0] + AuctionText.Select("天  ", "  ") + AuctionRules.VenueName((AuctionVenue)result2) + "  " + AuctionCatalog.CategoryName((ContainerCategory)result3) + AuctionText.Select("  成交 ", "  Bid ") + array2[4] + (flag ? (AuctionText.Select("  货值 ", "  Value ") + array2[5] + AuctionText.Select("  净收益 ", "  Net ") + array2[6]) : AuctionText.Select("  货值 待揭晓  净收益 待结算", "  Value pending  Net pending")));
				}
			}
			list.Reverse();
			return list;
		}

		internal static void MarkPurchased()
		{
			Write("purchased_day", AuctionCatalog.Day.ToString());
		}

		internal static void SetPendingDelivery(ContainerCategory category, int seed, IReadOnlyList<string> contents)
		{
			string[] array = new string[5];
			int num = (int)category;
			array[0] = num.ToString();
			array[1] = "~";
			array[2] = seed.ToString();
			array[3] = "~";
			array[4] = string.Join("|", contents);
			string text = string.Concat(array);
			string text2 = Read("pending_deliveries");
			if (!text2.Split(';').Any((string value) => value.Split('~').Length >= 2 && value.Split('~')[1] == seed.ToString()))
			{
				Write("pending_deliveries", string.IsNullOrWhiteSpace(text2) ? text : (text2 + ";" + text));
			}
		}

		internal static bool TryGetPendingDelivery(out ContainerCategory category, out int seed, out List<string> contents)
		{
			category = ContainerCategory.Engineering;
			seed = 0;
			contents = new List<string>();
			string text = Read("pending_deliveries").Split(';').FirstOrDefault((string value) => !string.IsNullOrWhiteSpace(value));
			if (string.IsNullOrWhiteSpace(text))
			{
				text = Read("pending_delivery");
			}
			string[] array = text.Split('~');
			if (array.Length != 3 || !int.TryParse(array[0], out var result) || !int.TryParse(array[1], out seed))
			{
				return false;
			}
			category = (ContainerCategory)Math.Clamp(result, 0, 3);
			string[] array2 = array[2].Split('|');
			foreach (string text2 in array2)
			{
				if (!string.IsNullOrWhiteSpace(text2))
				{
					contents.Add(text2);
				}
			}
			if (seed != 0)
			{
				return contents.Count > 0;
			}
			return false;
		}

		internal static void AcknowledgeDelivery(int seed)
		{
			List<string> list = (from value in Read("pending_deliveries").Split(';')
				where !string.IsNullOrWhiteSpace(value)
				where value.Split('~').Length < 2 || value.Split('~')[1] != seed.ToString()
				select value).ToList();
			if (list.Count > 0)
			{
				Write("pending_deliveries", string.Join(";", list));
			}
			else
			{
				Delete("pending_deliveries");
			}
			string[] array = Read("pending_delivery").Split('~');
			if (array.Length >= 2 && array[1] == seed.ToString())
			{
				Delete("pending_delivery");
			}
		}

		internal static void ClearPendingDelivery()
		{
			Delete("pending_delivery");
			Delete("pending_deliveries");
		}

		internal static bool IsMerchantVisitRecorded(int week, int slot)
		{
			return !string.IsNullOrEmpty(Read("merchant_visit_" + week + "_" + slot));
		}

		internal static void RecordMerchantVisit(int week, int slot, ContainerCategory category)
		{
			string suffix = "merchant_visit_" + week + "_" + slot;
			string text = AuctionCatalog.Day.ToString();
			int num = (int)category;
			Write(suffix, text + "," + num);
		}

		internal static int NextIncrement(AuctionLot lot)
		{
			Random random = new Random(lot.Seed ^ ((lot.Round + 1) * 7919));
			return Math.Max(10, lot.CurrentBid * random.Next(5, 13) / 100);
		}

		internal static bool NpcCounters(AuctionLot lot)
		{
			if (lot.Round >= 5)
			{
				return false;
			}
			int num = ((lot.ReferenceValue <= 0) ? 100 : (lot.CurrentBid * 100 / lot.ReferenceValue));
			int num2 = Math.Clamp(78 - lot.Round * 9 - Math.Max(0, num - 70), 12, 78);
			return new Random(lot.Seed ^ (lot.Round * 104729)).Next(100) < num2;
		}

		private static List<AuctionLot> Decode(string encoded)
		{
			List<AuctionLot> list = new List<AuctionLot>();
			if (string.IsNullOrWhiteSpace(encoded))
			{
				return list;
			}
			string[] array = encoded.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(',');
				if (array2.Length != 23 && array2.Length != 24 && array2.Length != 26 && array2.Length != 28 && array2.Length != 29 && array2.Length != 32 && array2.Length != 33 && array2.Length != 37 && array2.Length != 38)
				{
					return new List<AuctionLot>();
				}
				try
				{
					AuctionLot auctionLot = new AuctionLot
					{
						Slot = int.Parse(array2[0]),
						Category = (ContainerCategory)int.Parse(array2[1]),
						Seed = int.Parse(array2[2]),
						ItemCount = int.Parse(array2[3]),
						EstimatedValue = int.Parse(array2[4]),
						CurrentBid = int.Parse(array2[5]),
						Round = int.Parse(array2[6]),
						PlayerLeading = (array2[7] == "1"),
						Finished = (array2[8] == "1"),
						Lost = (array2[9] == "1")
					};
					auctionLot.Started = array2[10] == "1";
					auctionLot.Sealed = array2[11] == "1";
					auctionLot.HighestBidder = int.Parse(array2[12]);
					auctionLot.ActiveMask = int.Parse(array2[13]);
					auctionLot.IntelMask = int.Parse(array2[14]);
					auctionLot.Budgets = ParseTriple(array2[15]);
					auctionLot.LastOffers = ParseTriple(array2[16]);
					string text = Encoding.UTF8.GetString(Convert.FromBase64String(array2[17]));
					if (text.Length > 0)
					{
						auctionLot.BidLog.AddRange(text.Split('\n'));
					}
					auctionLot.Venue = (AuctionVenue)int.Parse(array2[18]);
					auctionLot.Quality = (AuctionQuality)int.Parse(array2[19]);
					auctionLot.ReferenceValue = int.Parse(array2[20]);
					auctionLot.TargetValue = int.Parse(array2[21]);
					auctionLot.IntelSpent = int.Parse(array2[22]);
					if (array2.Length >= 24)
					{
						string text2 = Encoding.UTF8.GetString(Convert.FromBase64String(array2[23]));
						if (text2.Length > 0)
						{
							auctionLot.Contents.AddRange(text2.Split('\n'));
						}
					}
					auctionLot.OpeningBid = ((array2.Length >= 26) ? int.Parse(array2[24]) : auctionLot.CurrentBid);
					auctionLot.PlayerOffer = ((array2.Length >= 26) ? int.Parse(array2[25]) : (auctionLot.PlayerLeading ? auctionLot.CurrentBid : 0));
					auctionLot.PendingOffer = ((array2.Length >= 28) ? int.Parse(array2[26]) : (-1));
					auctionLot.BiddersShown = ((array2.Length >= 28) ? Math.Clamp(int.Parse(array2[27]), 0, 3) : 0);
					auctionLot.Withdrawn = array2.Length >= 29 && array2[28] == "1";
					if (array2.Length >= 32)
					{
						auctionLot.MemoryModifiers = ParseTriple(array2[29]);
						auctionLot.RivalMemoryApplied = array2[30] == "1";
						auctionLot.RivalMemoryRecorded = array2[31] == "1";
					}
					if (array2.Length >= 33)
					{
						auctionLot.RivalExitReasons = ParseTriple(array2[32]);
					}
					if (array2.Length >= 37)
					{
						auctionLot.RivalIntelTiers = ParseTriple(array2[33]);
						auctionLot.RivalIntelValues = ParseTriple(array2[34]);
						auctionLot.RivalIntelSpent = ParseTriple(array2[35]);
						auctionLot.RivalIntelRounds = ParseTriple(array2[36]);
						for (int j = 0; j < 3; j++)
						{
							if (auctionLot.RivalIntelTiers[j] < 0 || auctionLot.RivalIntelTiers[j] > 2 || auctionLot.RivalIntelValues[j] < 0 || auctionLot.RivalIntelSpent[j] < 0 || auctionLot.RivalIntelSpent[j] > auctionLot.Budgets[j] || auctionLot.RivalIntelRounds[j] < 0 || auctionLot.RivalIntelRounds[j] > 5)
							{
								throw new FormatException("Invalid rival intelligence state.");
							}
						}
					}
					if (auctionLot.PendingOffer < 0)
					{
						auctionLot.BiddersShown = 0;
					}
					if (array2.Length >= 38)
					{
						string text3 = Encoding.UTF8.GetString(Convert.FromBase64String(array2[37]));
						if (text3.Length > 0)
						{
							auctionLot.InspectionItems.AddRange(text3.Split('\n'));
						}
						if (auctionLot.InspectionItems.Count > 4 || auctionLot.InspectionItems.Any(string.IsNullOrWhiteSpace) || (auctionLot.InspectionItems.Count > 0 && (auctionLot.IntelMask & 4) == 0))
						{
							throw new FormatException("Invalid inspection sample.");
						}
					}
					if (auctionLot.InspectionItems.Count == 0)
					{
						auctionLot.IntelMask &= -5;
					}
					list.Add(auctionLot);
				}
				catch
				{
					return new List<AuctionLot>();
				}
			}
			return list;
		}

		private static int[] ParseTriple(string text)
		{
			string[] array = text.Split(':');
			if (array.Length != 3)
			{
				throw new FormatException("Invalid bidder state.");
			}
			return new int[3]
			{
				int.Parse(array[0]),
				int.Parse(array[1]),
				int.Parse(array[2])
			};
		}

		private static int StableSeed(string text)
		{
			int num = 17;
			foreach (char c in text)
			{
				num = num * 31 + c;
			}
			if (num != int.MinValue)
			{
				return Math.Abs(num);
			}
			return 1;
		}
	}
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
	internal static class AuctionUiStyle
	{
		private static Texture2D _texture;

		private static Sprite _rounded;

		internal static readonly Color Gold = new Color(0.84f, 0.72f, 0.49f, 1f);

		internal static readonly Color Border = new Color(0.4f, 0.4f, 0.46f, 1f);

		internal static readonly Color Muted = new Color(0.7f, 0.71f, 0.75f, 1f);

		private static Sprite Rounded
		{
			get
			{
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Expected O, but got Unknown
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				if (Object.op_Implicit((Object)(object)_rounded))
				{
					return _rounded;
				}
				_texture = new Texture2D(20, 20, (TextureFormat)4, false)
				{
					name = "XIAOWOAuctionButtonMask",
					filterMode = (FilterMode)0,
					wrapMode = (TextureWrapMode)1,
					hideFlags = (HideFlags)61
				};
				Color[] array = (Color[])(object)new Color[400];
				for (int i = 0; i < 20; i++)
				{
					for (int j = 0; j < 20; j++)
					{
						float num = Mathf.Max(new float[3]
						{
							4f - ((float)j + 0.5f),
							(float)j + 0.5f - 16f,
							0f
						});
						float num2 = Mathf.Max(new float[3]
						{
							4f - ((float)i + 0.5f),
							(float)i + 0.5f - 16f,
							0f
						});
						array[i * 20 + j] = ((num * num + num2 * num2 <= 16f) ? Color.white : Color.clear);
					}
				}
				_texture.SetPixels(array);
				_texture.Apply(false, true);
				_rounded = Sprite.Create(_texture, new Rect(0f, 0f, 20f, 20f), new Vector2(0.5f, 0.5f), 100f, 0u, (SpriteMeshType)0, new Vector4(5f, 5f, 5f, 5f));
				((Object)_rounded).name = "XIAOWOAuctionRoundedButton";
				return _rounded;
			}
		}

		internal static Image RoundedImage(GameObject target, Color color)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			Image obj = target.GetComponent<Image>() ?? target.AddComponent<Image>();
			obj.sprite = Rounded;
			obj.type = (Type)1;
			((Graphic)obj).color = color;
			((Graphic)obj).raycastTarget = false;
			return obj;
		}

		internal static void Dispose()
		{
			if (Object.op_Implicit((Object)(object)_rounded))
			{
				Object.Destroy((Object)(object)_rounded);
			}
			if (Object.op_Implicit((Object)(object)_texture))
			{
				Object.Destroy((Object)(object)_texture);
			}
			_rounded = null;
			_texture = null;
		}
	}
	internal enum AuctionRivalExit
	{
		None,
		Budget,
		Price,
		Bluff,
		Risk
	}
	internal static class AuctionEngine
	{
		private sealed class BidPlan
		{
			internal readonly int[] Offers = new int[3];

			internal readonly int[] Exits = new int[3];

			internal readonly string[] Actions = new string[3];
		}

		internal const int AuctionsPerNight = 3;

		internal const int MaxRounds = 5;

		private static readonly int[] EarlyLeadTenths = new int[4] { 20, 16, 13, 11 };

		internal static readonly string[] Names = new string[3] { "稳健客", "赌徒", "搅局者" };

		internal const int InspectionSize = 4;

		internal static void Start(AuctionLot lot, int[] remainingFunds = null)
		{
			if (lot.Started)
			{
				return;
			}
			lot.OpeningBid = Math.Max(1, lot.CurrentBid);
			Random random = new Random(lot.Seed ^ 0x57A139);
			lot.Sealed = random.Next(2) == 0;
			int num = Math.Max(100, (lot.ReferenceValue > 0) ? lot.ReferenceValue : lot.OpeningBid);
			int num2 = (int)lot.Venue * 8;
			if (remainingFunds != null && remainingFunds.Length != 3)
			{
				throw new ArgumentException("Three rival balances are required.", "remainingFunds");
			}
			for (int i = 0; i < 3; i++)
			{
				int[] budgets = lot.Budgets;
				int num3 = i;
				int num4;
				if (remainingFunds != null)
				{
					num4 = Math.Max(0, remainingFunds[i]);
				}
				else
				{
					int val = lot.CurrentBid + 10;
					long num5 = num;
					num4 = Math.Max(val, (int)Math.Min(2147483647L, num5 * i switch
					{
						0 => random.Next(75 + num2, 131 + num2), 
						1 => random.Next(70 + num2, 171 + num2), 
						_ => random.Next(80 + num2, 151 + num2), 
					} / 100));
				}
				budgets[num3] = num4;
			}
			lot.Started = true;
			lot.Round = 0;
			lot.HighestBidder = -1;
			lot.BidLog.Add("拍卖师：" + (lot.Sealed ? "暗标" : "公开") + "竞拍，最多五轮；领先足够即可提前成交。");
		}

		internal static int Minimum(AuctionLot lot)
		{
			return 0;
		}

		internal static int SuggestedBid(AuctionLot lot)
		{
			return Math.Max(1, Math.Max(lot.OpeningBid, lot.PlayerOffer));
		}

		internal static int MultiplierBase(AuctionLot lot)
		{
			return Math.Max(1, (lot.Round > 0 && lot.CurrentBid > 0) ? lot.CurrentBid : lot.OpeningBid);
		}

		internal static int MultipliedBid(AuctionLot lot, int tenths)
		{
			return (int)Math.Min(2147483647L, ((long)MultiplierBase(lot) * (long)Math.Max(10, tenths) + 9) / 10);
		}

		internal static string[] SampleContents(AuctionLot lot)
		{
			int count = lot.Contents.Count;
			if (count == 0)
			{
				return Array.Empty<string>();
			}
			List<int> list = new List<int>();
			for (int i = 0; i < count; i++)
			{
				list.Add(i);
			}
			List<string> list2 = new List<string>();
			foreach (string id in lot.InspectionItems)
			{
				if (list2.Count == 4)
				{
					break;
				}
				int num = list.FindIndex((int position) => lot.Contents[position] == id);
				if (num >= 0)
				{
					list2.Add(id);
					list.RemoveAt(num);
				}
			}
			Random random = new Random(lot.Seed ^ 0x31D5A72);
			while (list2.Count < 4 && list.Count > 0)
			{
				int index = random.Next(list.Count);
				list2.Add(lot.Contents[list[index]]);
				list.RemoveAt(index);
			}
			return list2.ToArray();
		}

		internal static string PlaceBid(AuctionLot lot, int amount, int cash)
		{
			if (lot.Finished || lot.Lost)
			{
				return "这场竞拍已经结束。";
			}
			if (amount < 0)
			{
				return AuctionText.Select("报价不能为负数。", "A bid cannot be negative.");
			}
			if (amount > cash)
			{
				return "现金不足，无法提交报价。";
			}
			return Resolve(lot, amount);
		}

		internal static string Pass(AuctionLot lot, int cash = int.MaxValue)
		{
			if (lot.Finished || lot.Lost)
			{
				return "这场竞拍已经结束。";
			}
			return Resolve(lot, (lot.PlayerOffer <= cash) ? lot.PlayerOffer : 0);
		}

		internal static string StageBid(AuctionLot lot, int amount, int cash)
		{
			if (lot.Finished || lot.Lost)
			{
				return "这场竞拍已经结束。";
			}
			if (lot.PendingOffer >= 0)
			{
				return "本轮已经提交报价。";
			}
			if (amount < 0)
			{
				return AuctionText.Select("报价不能为负数。", "A bid cannot be negative.");
			}
			if (amount > cash)
			{
				return "现金不足，无法提交报价。";
			}
			return StageOffer(lot, amount);
		}

		internal static string StagePass(AuctionLot lot, int cash)
		{
			if (lot.Finished || lot.Lost)
			{
				return "这场竞拍已经结束。";
			}
			if (lot.PendingOffer >= 0)
			{
				return "本轮已经提交报价。";
			}
			return StageOffer(lot, (lot.PlayerOffer <= cash) ? lot.PlayerOffer : 0);
		}

		internal static bool Withdraw(AuctionLot lot)
		{
			if (lot == null || !lot.Started || lot.Finished || lot.Lost || lot.PendingOffer >= 0)
			{
				return false;
			}
			lot.Withdrawn = true;
			lot.PlayerLeading = false;
			lot.PlayerOffer = 0;
			if (lot.HighestBidder == 3)
			{
				lot.HighestBidder = -1;
			}
			lot.BidLog.Add("拍卖师：你已退出本箱竞拍，本箱计入今晚的竞拍场次。未扣除箱款。");
			do
			{
				Resolve(lot, 0, announce: false);
			}
			while (!lot.Finished && lot.Round < 5);
			lot.BidLog.Add("拍卖师：AI已完成剩余竞价，省略中间播报。");
			lot.BidLog.Add(SettlementLine(lot));
			TrimLog(lot);
			return true;
		}

		private static string StageOffer(AuctionLot lot, int amount)
		{
			PrepareRivalIntel(lot, lot.Round + 1, amount, announce: true);
			lot.PendingOffer = amount;
			lot.BiddersShown = 0;
			lot.BidLog.Add("第" + (lot.Round + 1) + "轮 · 你" + ((amount > 0) ? ("报价 " + amount + "¢") : "未报价"));
			TrimLog(lot);
			return "已提交";
		}

		internal static string RevealBidder(AuctionLot lot)
		{
			if (lot.PendingOffer < 0 || lot.BiddersShown >= 3)
			{
				return "当前没有待播报的报价。";
			}
			int num = lot.BiddersShown++;
			BidPlan bidPlan = PlanRound(lot, lot.Round + 1, lot.PendingOffer);
			int num2 = bidPlan.Offers[num];
			string text = Names[num] + "：" + ((bidPlan.Exits[num] != 0) ? ExitLogText(bidPlan.Exits[num]) : (lot.Sealed ? "暗标已提交" : ((num2 > 0) ? (bidPlan.Actions[num] + " " + num2 + "¢") : "本轮未报价")));
			lot.BidLog.Add("第" + (lot.Round + 1) + "轮 · " + text);
			TrimLog(lot);
			return text;
		}

		internal static string FinishStagedRound(AuctionLot lot)
		{
			if (lot.PendingOffer < 0 || lot.BiddersShown != 3)
			{
				return "请先等待三位竞拍者报价。";
			}
			int pendingOffer = lot.PendingOffer;
			lot.PendingOffer = -1;
			lot.BiddersShown = 0;
			return Resolve(lot, pendingOffer);
		}

		internal static int IntelCost(AuctionLot lot, int kind, bool rival = false)
		{
			int original = BaseIntelCost(lot, kind);
			return AuctionNightMarket.IntelFee(lot, kind, original, rival);
		}

		internal static int BaseIntelCost(AuctionLot lot, int kind)
		{
			return kind switch
			{
				0 => Math.Max(20, lot.ReferenceValue / 40), 
				1 => Math.Max(30, lot.ReferenceValue * 35 / 1000), 
				2 => Math.Max(25, (int)((long)lot.ReferenceValue * 30L / 1000)), 
				3 => Math.Max(40, lot.ReferenceValue / 25), 
				_ => Math.Max(50, lot.ReferenceValue / 20), 
			};
		}

		internal static string Intel(AuctionLot lot, int kind)
		{
			switch (kind)
			{
			case 0:
			{
				int num = SpendingLimit(lot, 0);
				int num2 = ((lot.Venue == AuctionVenue.Advanced) ? 500 : ((lot.Venue == AuctionVenue.Intermediate) ? 100 : 50));
				int num3 = num / num2 * num2;
				return "稳健客心理价约 " + num3 + " 至 " + Math.Min(lot.Budgets[0], num3 + num2 * 2);
			}
			case 1:
			{
				(int, int, bool) tuple2 = ValueRange(lot, 1);
				return "情报估值 " + tuple2.Item1 + " 至 " + tuple2.Item2;
			}
			case 2:
				return "抽检货物：已揭示 " + lot.InspectionItems.Count + " 件真实物品，详见抽检窗口。";
			case 4:
			{
				(int, int, bool) tuple = ValueRange(lot, 2);
				if (!tuple.Item3)
				{
					return "复鉴区间 " + tuple.Item1 + " 至 " + tuple.Item2;
				}
				return "复鉴失败：读数超出设备量程，可能存在异常高价值货物。";
			}
			default:
				if (lot.Budgets[2] - lot.CurrentBid > Math.Max(50, lot.CurrentBid / 10))
				{
					if (lot.Budgets[2] >= lot.Budgets[1])
					{
						return "搅局者仍有明显抬价空间。";
					}
					return "搅局者更像在抬价，赌徒意愿更强。";
				}
				return "搅局者已经接近退出价格。";
			}
		}

		private static string Resolve(AuctionLot lot, int playerOffer, bool announce = true)
		{
			int num = Math.Clamp(lot.Round + 1, 1, 5);
			PrepareRivalIntel(lot, num, playerOffer, announce);
			BidPlan bidPlan = PlanRound(lot, num, playerOffer);
			lot.Round = num;
			lot.PlayerOffer = playerOffer;
			int num2 = playerOffer;
			int num3 = ((playerOffer > 0) ? 3 : (-1));
			int num4 = 0;
			int[] offers = bidPlan.Offers;
			for (int i = 0; i < 3; i++)
			{
				if (bidPlan.Exits[i] != 0)
				{
					lot.ActiveMask &= ~(1 << i);
					lot.RivalExitReasons[i] = bidPlan.Exits[i];
				}
				int num5 = offers[i];
				if (num5 <= 0)
				{
					lot.LastOffers[i] = 0;
					continue;
				}
				lot.LastOffers[i] = num5;
				if (num5 > num2 || (num5 == num2 && i == lot.HighestBidder))
				{
					num4 = num2;
					num2 = num5;
					num3 = i;
				}
				else if (num5 > num4)
				{
					num4 = num5;
				}
			}
			lot.HighestBidder = num3;
			lot.CurrentBid = num2;
			lot.PlayerLeading = num3 == 3;
			if (announce)
			{
				lot.BidLog.Add("第" + num + "轮揭价 · " + BidTable(lot, playerOffer));
			}
			if ((num >= 5 || num3 < 0 || (num4 != 0 && (long)num2 * 10L < (long)num4 * (long)EarlyLeadTenths[num - 1])) && num < 5)
			{
				if (announce)
				{
					lot.BidLog.Add("拍卖师：未达到本轮领先条件，进入第" + (num + 1) + "轮。");
				}
				TrimLog(lot);
				return "继续";
			}
			lot.Finished = true;
			lot.Lost = !lot.PlayerLeading;
			if (announce)
			{
				lot.BidLog.Add(SettlementLine(lot));
			}
			TrimLog(lot);
			if (!lot.PlayerLeading)
			{
				return "落标";
			}
			return "成交";
		}

		private static string SettlementLine(AuctionLot lot)
		{
			if (lot.HighestBidder >= 0)
			{
				return "拍卖师：" + ((lot.HighestBidder == 3) ? "你" : Names[lot.HighestBidder]) + "以 " + lot.CurrentBid + " ¢ " + ((lot.Round < 5) ? "提前得标。" : "在最终轮得标。");
			}
			return "拍卖师：无人提交有效报价，本箱流拍。";
		}

		internal static int[] PreviewOffers(AuctionLot lot, int round, int playerOffer = -1)
		{
			return PlanRound(lot, round, (playerOffer >= 0) ? playerOffer : ((lot.PendingOffer >= 0) ? lot.PendingOffer : lot.PlayerOffer)).Offers;
		}

		private static int Appraisal(AuctionLot lot, int rival)
		{
			Random random = new Random(lot.Seed ^ 0x6429DAF ^ ((rival + 1) * 104729));
			int num = rival switch
			{
				0 => random.Next(82, 114), 
				1 => random.Next(76, 134), 
				_ => random.Next(76, 123), 
			};
			int num2 = ((lot.RivalIntelTiers[rival] > 0) ? Math.Max(1, lot.RivalIntelValues[rival]) : Math.Max(100, (lot.ReferenceValue > 0) ? lot.ReferenceValue : lot.OpeningBid));
			if (lot.RivalIntelTiers[rival] > 0)
			{
				num = random.Next(94, 107);
			}
			return (int)Math.Min(2147483647L, (long)num2 * (long)num / 100);
		}

		internal static int SpendingLimit(AuctionLot lot, int rival, bool bluff = false)
		{
			Random random = new Random(lot.Seed ^ 0x14EC59 ^ ((rival + 1) * 65537));
			int num = rival switch
			{
				0 => random.Next(86, 100), 
				1 => random.Next(90, 113), 
				_ => random.Next(84, 101), 
			};
			if (rival == 2 && bluff)
			{
				num = Math.Max(num, random.Next(100, 109));
			}
			int num2 = Math.Clamp(lot.MemoryModifiers[rival], -12, 18) / 3;
			int num3 = Math.Max(100, (lot.ReferenceValue > 0) ? lot.ReferenceValue : lot.OpeningBid);
			long val = (long)num3 * (long)(rival switch
			{
				1 => 145, 
				0 => 110, 
				_ => 122, 
			}) / 100;
			if (lot.RivalIntelTiers[rival] > 0)
			{
				val = Math.Min((long)num3 * 3L, Math.Max(val, (long)lot.RivalIntelValues[rival] * 110L / 100));
			}
			int original = (int)Math.Min((long)Math.Max(0, lot.Budgets[rival] - lot.RivalIntelSpent[rival]) * (long)(rival switch
			{
				1 => 95, 
				0 => 85, 
				_ => 88, 
			}) / 100, Math.Min(val, (long)Appraisal(lot, rival) * (long)(num + num2) / 100));
			return AuctionNightMarket.RivalLimit(lot, rival, original);
		}

		private static BidPlan PlanRound(AuctionLot lot, int round, int playerOffer)
		{
			BidPlan bidPlan = new BidPlan();
			round = Math.Clamp(round, 1, 5);
			int num = ((!lot.Withdrawn) ? (lot.Sealed ? lot.PlayerOffer : playerOffer) : 0);
			for (int i = 0; i < 3; i++)
			{
				if ((lot.ActiveMask & (1 << i)) == 0)
				{
					bidPlan.Exits[i] = ((lot.RivalExitReasons[i] != 0) ? lot.RivalExitReasons[i] : 2);
					continue;
				}
				int num2 = num;
				for (int j = 0; j < 3; j++)
				{
					if (j != i && (lot.ActiveMask & (1 << j)) != 0)
					{
						int val = ((!lot.Sealed && j < i) ? bidPlan.Offers[j] : lot.LastOffers[j]);
						num2 = Math.Max(num2, val);
					}
				}
				int limit = SpendingLimit(lot, i, i == 2);
				int appraisal = Appraisal(lot, i);
				int previous = lot.LastOffers[i];
				int available = Math.Max(0, lot.Budgets[i] - lot.RivalIntelSpent[i]);
				AuctionRivalDecision auctionRivalDecision = new AuctionRivalAgent(lot.Seed, i).Decide(new AuctionRivalObservation(round, previous, num2, appraisal, limit, available, lot.MemoryModifiers[i], !lot.Sealed));
				bidPlan.Offers[i] = auctionRivalDecision.Offer;
				bidPlan.Exits[i] = (int)auctionRivalDecision.Exit;
				bidPlan.Actions[i] = auctionRivalDecision.Action;
			}
			return bidPlan;
		}

		internal static void PrepareRivalIntel(AuctionLot lot, int round, int playerOffer, bool announce)
		{
			round = Math.Clamp(round, 1, 5);
			for (int i = 0; i < 3; i++)
			{
				if (lot.RivalIntelRounds[i] >= round)
				{
					continue;
				}
				lot.RivalIntelRounds[i] = round;
				if ((lot.ActiveMask & (1 << i)) == 0 || lot.RivalIntelTiers[i] >= 2)
				{
					continue;
				}
				int num = ((!lot.Withdrawn) ? (lot.Sealed ? lot.PlayerOffer : playerOffer) : 0);
				for (int j = 0; j < 3; j++)
				{
					if (j != i && (lot.ActiveMask & (1 << j)) != 0)
					{
						num = Math.Max(num, lot.LastOffers[j]);
					}
				}
				int num2 = Appraisal(lot, i);
				bool flag = num >= (long)num2 * 65L / 100;
				int num3 = ((round != 1) ? ((!flag) ? 8 : (i switch
				{
					1 => 30, 
					0 => 55, 
					_ => 45, 
				})) : (i switch
				{
					1 => 12, 
					0 => 18, 
					_ => 16, 
				}));
				if (lot.RivalIntelTiers[i] > 0 && (round < 3 || !flag))
				{
					continue;
				}
				Random random = new Random(lot.Seed ^ 0x723DA1 ^ (round * 7919) ^ ((i + 1) * 104729));
				if (random.Next(100) >= num3)
				{
					continue;
				}
				int num4 = lot.RivalIntelTiers[i] + 1;
				int num5 = IntelCost(lot, (num4 == 1) ? 1 : 4, rival: true);
				if (Math.Max(0, lot.Budgets[i] - lot.RivalIntelSpent[i]) >= (long)num5 * 4L && (long)lot.RivalIntelSpent[i] + (long)num5 <= lot.Budgets[i] / 10)
				{
					(int, int, bool) tuple = ValueRange(lot, num4);
					int num6 = Math.Max(100, (lot.ReferenceValue > 0) ? lot.ReferenceValue : lot.OpeningBid);
					long num7 = (tuple.Item3 ? ((long)num6 * 2L) : (((long)tuple.Item1 + (long)tuple.Item2) / 2));
					num7 = Math.Clamp(num7 * random.Next(91, 111) / 100, 1L, Math.Min(2147483647L, (long)num6 * 3L));
					lot.RivalIntelTiers[i] = num4;
					lot.RivalIntelValues[i] = (int)num7;
					lot.RivalIntelSpent[i] += num5;
					if (announce)
					{
						lot.BidLog.Add("第" + round + "轮 · " + Names[i] + "：购买" + ((num4 == 1) ? "估值情报" : "复鉴道具") + "，花费 " + num5 + "¢");
					}
				}
			}
			TrimLog(lot);
		}

		internal static void RefreshMigratedFunds(AuctionLot lot, int[] remainingFunds)
		{
			if (!lot.Started || lot.Finished || lot.Lost)
			{
				return;
			}
			int num = Math.Max(1, lot.OpeningBid);
			int[] array = (int[])lot.Budgets.Clone();
			int[] array2 = new int[3];
			for (int i = 0; i < 3; i++)
			{
				array2[i] = SpendingLimit(lot, i, i == 2);
			}
			lot.Budgets = (int[])remainingFunds.Clone();
			int num2 = ((!lot.Withdrawn) ? (lot.Sealed ? lot.PlayerOffer : ((lot.PendingOffer >= 0) ? lot.PendingOffer : lot.PlayerOffer)) : 0);
			for (int j = 0; j < 3; j++)
			{
				AuctionRivalExit auctionRivalExit = (AuctionRivalExit)lot.RivalExitReasons[j];
				bool flag = (auctionRivalExit == AuctionRivalExit.Budget && array[j] < num) || (auctionRivalExit == AuctionRivalExit.Price && array2[j] < num);
				int num3 = num2;
				for (int k = 0; k < 3; k++)
				{
					if (k != j && (lot.ActiveMask & (1 << k)) != 0)
					{
						num3 = Math.Max(num3, lot.LastOffers[k]);
					}
				}
				if (flag && num3 <= SpendingLimit(lot, j, j == 2) && lot.Budgets[j] > 0)
				{
					lot.RivalExitReasons[j] = 0;
					lot.ActiveMask |= 1 << j;
				}
			}
			if (lot.PendingOffer >= 0)
			{
				lot.BiddersShown = 0;
			}
		}

		internal static int ExitReason(AuctionLot lot, int rival)
		{
			if (lot.PendingOffer < 0 || lot.BiddersShown <= rival)
			{
				return lot.RivalExitReasons[rival];
			}
			return PlanRound(lot, lot.Round + 1, lot.PendingOffer).Exits[rival];
		}

		private static string ExitLogText(int reason)
		{
			return (AuctionRivalExit)reason switch
			{
				AuctionRivalExit.Budget => "退出：资金不足", 
				AuctionRivalExit.Price => "退出：超过心理价", 
				AuctionRivalExit.Bluff => "退出：抬价后收手", 
				AuctionRivalExit.Risk => "退出：不再冒险", 
				_ => "本轮未报价", 
			};
		}

		internal static string ExitText(int reason)
		{
			return (AuctionRivalExit)reason switch
			{
				AuctionRivalExit.Budget => AuctionText.Select("退出：资金不足", "Out: no funds"), 
				AuctionRivalExit.Price => AuctionText.Select("退出：超过心理价", "Out: too costly"), 
				AuctionRivalExit.Bluff => AuctionText.Select("退出：抬价后收手", "Out: bluff done"), 
				AuctionRivalExit.Risk => AuctionText.Select("退出：不再冒险", "Out: too risky"), 
				_ => AuctionText.Select("本轮未报价", "No bid this round"), 
			};
		}

		internal static string RivalMood(AuctionLot lot, int rival)
		{
			AuctionRivalExit auctionRivalExit = (AuctionRivalExit)ExitReason(lot, rival);
			if (lot.Finished && lot.HighestBidder == rival)
			{
				int num = Appraisal(lot, rival);
				if (lot.CurrentBid <= (long)num * 110L / 100)
				{
					if (lot.CurrentBid > (long)num * 75L / 100)
					{
						return AuctionText.Select("得标满意", "Pleased winner");
					}
					return AuctionText.Select("捡漏满意", "Bargain joy");
				}
				return AuctionText.Select("买贵懊恼", "Overpaid");
			}
			switch (auctionRivalExit)
			{
			default:
				return AuctionText.Select("冷静收手", "Walked away");
			case AuctionRivalExit.Budget:
				return AuctionText.Select("无奈退出", "Out of funds");
			case AuctionRivalExit.Bluff:
				return AuctionText.Select("得意收手", "Bluff complete");
			case AuctionRivalExit.None:
				if (lot.Finished)
				{
					return AuctionText.Select("落标不甘", "Disappointed");
				}
				if (lot.LastOffers[rival] > 0 && lot.LastOffers[rival] >= (long)SpendingLimit(lot, rival, rival == 2) * 90L / 100)
				{
					return AuctionText.Select("预算吃紧", "Near my limit");
				}
				if (lot.HighestBidder == rival && lot.Round > 0)
				{
					return AuctionText.Select("守价观望", "Holding the lead");
				}
				if (rival == 2 && lot.Round > 0)
				{
					return AuctionText.Select("试探抬价", "Testing a bluff");
				}
				if (rival == 1 && lot.Round > 0)
				{
					return AuctionText.Select("兴奋跟价", "Eager to bid");
				}
				if (lot.MemoryModifiers[rival] < 8)
				{
					if (lot.MemoryModifiers[rival] > -3)
					{
						return AuctionText.Select("平常心", "Steady");
					}
					return AuctionText.Select("谨慎观望", "Cautious");
				}
				return AuctionText.Select("势在必得", "Determined");
			}
		}

		internal static string BidTable(AuctionLot lot, int playerOffer = -1)
		{
			int value = ((playerOffer >= 0) ? playerOffer : lot.PlayerOffer);
			return "你 " + Offer(value) + " / " + Names[0] + " " + Offer(lot.LastOffers[0]) + " / " + Names[1] + " " + Offer(lot.LastOffers[1]) + " / " + Names[2] + " " + Offer(lot.LastOffers[2]);
		}

		private static string Offer(int value)
		{
			if (value <= 0)
			{
				return "未报价";
			}
			return value + "¢";
		}

		internal static string LeadRule(AuctionLot lot)
		{
			if (lot.Round < 4)
			{
				return AuctionText.Select("需领先第二名 ", "Lead runner-up by ") + (double)EarlyLeadTenths[lot.Round] / 10.0 + AuctionText.Select(" 倍", "x");
			}
			return AuctionText.Select("最终轮最高价得标", "Highest bid wins final round");
		}

		internal static string SurfaceClue(AuctionLot lot)
		{
			return new Random(lot.Seed ^ 0x295A1B).Next(3) switch
			{
				0 => AuctionText.Select("封条完整", "Intact seal"), 
				1 => AuctionText.Select("外壳磨损", "Worn casing"), 
				_ => AuctionText.Select("有修补痕迹", "Repaired casing"), 
			};
		}

		internal static string FundsHint(AuctionLot lot, int index)
		{
			int num = Math.Max(100, (lot.ReferenceValue > 0) ? lot.ReferenceValue : lot.OpeningBid);
			if (lot.Budgets[index] >= (long)num * 90L / 100)
			{
				if (lot.Budgets[index] < (long)num * 140L / 100)
				{
					return AuctionText.Select("资金一般", "Moderate funds");
				}
				return AuctionText.Select("资金充裕", "Well funded");
			}
			return AuctionText.Select("资金有限", "Limited funds");
		}

		internal static (int Min, int Max, bool Anomaly) ValueRange(AuctionLot lot, int tier)
		{
			tier = Math.Clamp(tier, 0, 2);
			Random random = new Random(lot.Seed ^ ((tier + 1) * 17873));
			int num = Math.Max(100, (lot.ReferenceValue > 0) ? lot.ReferenceValue : lot.OpeningBid);
			int num2 = Math.Max(100, lot.EstimatedValue);
			if (tier == 2)
			{
				int num3 = ((num2 >= (long)num * 4L) ? 70 : ((num2 >= (long)num * 2L) ? 12 : 3));
				if (random.Next(100) < num3)
				{
					return (Min: 0, Max: 0, Anomaly: true);
				}
			}
			long num4 = tier switch
			{
				0 => (long)num * (long)random.Next(75, 136) / 100, 
				1 => (long)num2 * (long)random.Next(78, 123) / 100, 
				_ => (long)num2 * (long)random.Next(91, 110) / 100, 
			};
			long num5 = num;
			long num6 = num5 * tier switch
			{
				0 => random.Next(60, 111), 
				1 => random.Next(28, 60), 
				_ => random.Next(12, 27), 
			} / 100;
			int num7 = ((lot.Venue == AuctionVenue.Advanced) ? 500 : ((lot.Venue == AuctionVenue.Intermediate) ? 100 : 50));
			long num8 = Math.Max(num7, (num4 - num6 / 2) / num7 * num7);
			long val = Math.Max(num8 + num7, (num4 + (num6 + 1) / 2 + num7 - 1) / num7 * num7);
			return (Min: (int)Math.Min(2147483647L, num8), Max: (int)Math.Min(2147483647L, val), Anomaly: false);
		}

		private static void TrimLog(AuctionLot lot)
		{
			if (lot.BidLog.Count > 100)
			{
				lot.BidLog.RemoveRange(0, lot.BidLog.Count - 100);
			}
		}
	}
	internal readonly struct AuctionRivalObservation
	{
		internal readonly int Round;

		internal readonly int Previous;

		internal readonly int Competition;

		internal readonly int Appraisal;

		internal readonly int Limit;

		internal readonly int Available;

		internal readonly int Memory;

		internal readonly bool Public;

		internal AuctionRivalObservation(int round, int previous, int competition, int appraisal, int limit, int available, int memory, bool publicAuction)
		{
			Round = round;
			Previous = previous;
			Competition = competition;
			Appraisal = appraisal;
			Limit = limit;
			Available = available;
			Memory = memory;
			Public = publicAuction;
		}
	}
	internal readonly struct AuctionRivalDecision
	{
		internal readonly int Offer;

		internal readonly AuctionRivalExit Exit;

		internal readonly string Action;

		internal AuctionRivalDecision(int offer, string action, AuctionRivalExit exit = AuctionRivalExit.None)
		{
			Offer = offer;
			Action = action;
			Exit = exit;
		}
	}
	internal sealed class AuctionRivalAgent
	{
		private readonly int _seed;

		private readonly int _seat;

		private readonly int _initiative;

		private readonly int _patience;

		private readonly int _conviction;

		private readonly int _bluff;

		internal AuctionRivalAgent(int seed, int seat)
		{
			_seed = seed;
			_seat = seat;
			Random random = new Random(seed ^ 0x2682D7 ^ ((seat + 1) * 65537));
			_initiative = random.Next(15, 46) + ((seat == 1) ? 10 : 0);
			_patience = random.Next(20, 71);
			_conviction = random.Next(-10, 11);
			_bluff = ((seat == 2) ? random.Next(25, 51) : 0);
		}

		internal AuctionRivalDecision Decide(AuctionRivalObservation seen)
		{
			Random random = new Random(_seed ^ 0x3C6EF35F ^ (seen.Round * 7919) ^ ((_seat + 1) * 104729));
			int num = ((_seat == 0) ? 60 : ((_seat == 1) ? 68 : 57)) + _conviction + Math.Clamp(seen.Memory, -12, 18) / 3 + random.Next(-12, 18);
			int num2 = ((seen.Appraisal >= 10000) ? 50 : ((seen.Appraisal >= 2000) ? 10 : ((seen.Appraisal < 200) ? 1 : 5)));
			long num3 = Math.Min(seen.Limit, (long)seen.Appraisal * (long)num / 100);
			int num4 = random.Next(100);
			int num5 = random.Next(100);
			bool flag = _bluff > 0 && new Random(_seed ^ 0x7A4B21 ^ ((_seat + 1) * 65537)).Next(100) < _bluff;
			if (seen.Available <= 0 || seen.Competition > seen.Available)
			{
				return Exit(AuctionRivalExit.Budget);
			}
			if (flag && seen.Round > 1 && seen.Previous > 0 && seen.Competition >= (long)seen.Appraisal * 85L / 100 && num5 < 70)
			{
				return Exit(AuctionRivalExit.Bluff);
			}
			if (seen.Limit <= 0 || seen.Competition >= seen.Limit)
			{
				return Exit(AuctionRivalExit.Price);
			}
			long num6 = (long)seen.Competition * 100L / seen.Limit;
			if (seen.Round > 1 && seen.Competition > seen.Previous && num6 >= ((_seat == 1) ? 88 : 78) && num5 < ((_seat == 0) ? 28 : ((_seat == 1) ? 12 : 35)))
			{
				return Exit(AuctionRivalExit.Risk);
			}
			if (seen.Previous > seen.Competition && seen.Previous <= seen.Limit && num4 < 80 && seen.Previous <= (long)seen.Appraisal * (long)(95 + _conviction) / 100)
			{
				return new AuctionRivalDecision(seen.Previous, "守住报价");
			}
			if (seen.Round > 1 && seen.Previous <= seen.Competition && num5 < _patience / 5)
			{
				return new AuctionRivalDecision(Math.Min(seen.Previous, seen.Limit), "观望报价");
			}
			string action = ((seen.Previous > 0) ? "独立估价" : "试探报价");
			long num7 = 100 - num6;
			int num8 = Math.Clamp(_initiative + (int)num7 / 6 + Math.Clamp(seen.Memory, -12, 18) / 3 - (seen.Round - 1) * 2, 8, 52);
			if (seen.Public && seen.Competition > 0 && num3 <= seen.Competition && num4 < num8)
			{
				long val = (long)seen.Appraisal * (long)random.Next(15, (_seat == 1) ? 151 : 86) / 1000;
				num3 = Math.Min(seen.Limit, seen.Competition + Math.Max(num2, val));
				action = (flag ? "试探抬价" : "争抢报价");
			}
			else if (seen.Previous > 0 && num4 < _patience / 2 && seen.Previous <= seen.Limit)
			{
				num3 = seen.Previous;
				action = "守住报价";
			}
			int num9 = (int)Math.Min(seen.Limit, Math.Max(1L, (num3 + num2 - 1) / num2 * num2));
			if (seen.Previous > 0 && num9 < seen.Previous)
			{
				action = "重新报价";
			}
			return new AuctionRivalDecision(num9, action);
		}

		private static AuctionRivalDecision Exit(AuctionRivalExit reason)
		{
			return new AuctionRivalDecision(0, null, reason);
		}
	}
	internal sealed class AuctionRivalFunds
	{
		internal AuctionVenue Venue;

		internal int AnchorCash;

		internal int[] Initial = new int[3];

		internal int[] Remaining = new int[3];

		internal bool Legacy;

		internal HashSet<string> Settled = new HashSet<string>(StringComparer.Ordinal);

		internal Dictionary<string, int[]> IntelPayments = new Dictionary<string, int[]>(StringComparer.Ordinal);

		internal static int BaseCap(AuctionVenue venue)
		{
			return venue switch
			{
				AuctionVenue.Beginner => 3000, 
				AuctionVenue.Intermediate => 15000, 
				_ => 60000, 
			};
		}

		internal static int BaseFloor(AuctionVenue venue)
		{
			return venue switch
			{
				AuctionVenue.Beginner => 1000, 
				AuctionVenue.Intermediate => 6000, 
				_ => 24000, 
			};
		}

		internal static AuctionRivalFunds Create(AuctionVenue venue, int playerCash, int seed)
		{
			AuctionRivalFunds auctionRivalFunds = new AuctionRivalFunds
			{
				Venue = venue,
				AnchorCash = Math.Clamp(playerCash, BaseFloor(venue), BaseCap(venue))
			};
			Random random = new Random(seed ^ 0x672A51B);
			int minValue = auctionRivalFunds.AnchorCash / 2;
			int num = auctionRivalFunds.AnchorCash * 3 / 2;
			for (int i = 0; i < 3; i++)
			{
				auctionRivalFunds.Initial[i] = (auctionRivalFunds.Remaining[i] = random.Next(minValue, num + 1));
			}
			return auctionRivalFunds;
		}

		internal bool UpgradeLowAnchor(int seed)
		{
			if (AnchorCash >= BaseFloor(Venue))
			{
				return false;
			}
			AuctionRivalFunds auctionRivalFunds = Create(Venue, BaseFloor(Venue), seed);
			for (int i = 0; i < 3; i++)
			{
				int num = Math.Max(0, auctionRivalFunds.Initial[i] - Initial[i]);
				Initial[i] += num;
				Remaining[i] += num;
			}
			AnchorCash = auctionRivalFunds.AnchorCash;
			return true;
		}

		internal static string LotKey(AuctionLot lot)
		{
			return lot.Seed + ":" + lot.Slot;
		}

		internal void AdoptLegacy(IEnumerable<AuctionLot> lots)
		{
			AuctionLot auctionLot = lots.FirstOrDefault((AuctionLot lot) => lot.Started && !lot.Finished && !lot.Lost);
			if (auctionLot != null)
			{
				Legacy = true;
				Initial = auctionLot.Budgets.Select((int value) => Math.Max(0, value)).ToArray();
				Remaining = (int[])Initial.Clone();
			}
			foreach (AuctionLot item in lots.Where((AuctionLot lot) => lot.Finished))
			{
				Settled.Add(LotKey(item));
			}
		}

		internal bool Settle(AuctionLot lot)
		{
			if (lot.Venue != Venue || !lot.Started)
			{
				return false;
			}
			string text = LotKey(lot);
			bool flag = false;
			if (!IntelPayments.TryGetValue(text, out var value))
			{
				value = new int[3];
			}
			for (int i = 0; i < 3; i++)
			{
				int num = lot.RivalIntelSpent[i] - value[i];
				if (num > 0)
				{
					if (num > Remaining[i])
					{
						throw new InvalidOperationException("Rival intelligence exceeds wallet.");
					}
					Remaining[i] -= num;
					value[i] += num;
					flag = true;
				}
			}
			if (flag)
			{
				IntelPayments[text] = value;
			}
			if (!lot.Finished || !Settled.Add(text))
			{
				return flag;
			}
			int highestBidder = lot.HighestBidder;
			if (highestBidder >= 0 && highestBidder < 3 && lot.CurrentBid > 0)
			{
				Remaining[lot.HighestBidder] = (int)Math.Max(0L, (long)Remaining[lot.HighestBidder] - (long)lot.CurrentBid);
			}
			return true;
		}

		internal string Encode()
		{
			return string.Join("|", "1", (int)Venue, AnchorCash, string.Join(":", Initial), string.Join(":", Remaining), Legacy ? "1" : "0", string.Join(",", Settled.OrderBy<string, string>((string key) => key, StringComparer.Ordinal)), string.Join(",", from pair in IntelPayments.OrderBy<KeyValuePair<string, int[]>, string>((KeyValuePair<string, int[]> pair) => pair.Key, StringComparer.Ordinal)
				select pair.Key + "~" + string.Join(":", pair.Value)));
		}

		internal static AuctionRivalFunds Decode(string encoded)
		{
			if (string.IsNullOrEmpty(encoded))
			{
				return null;
			}
			try
			{
				string[] array = encoded.Split('|');
				if ((array.Length != 7 && array.Length != 8) || array[0] != "1" || (array[5] != "0" && array[5] != "1"))
				{
					return null;
				}
				int num = int.Parse(array[1]);
				if (num < 0 || num > 2)
				{
					return null;
				}
				AuctionRivalFunds auctionRivalFunds = new AuctionRivalFunds
				{
					Venue = (AuctionVenue)num,
					AnchorCash = int.Parse(array[2]),
					Initial = array[3].Split(':').Select(int.Parse).ToArray(),
					Remaining = array[4].Split(':').Select(int.Parse).ToArray(),
					Legacy = (array[5] == "1")
				};
				if (auctionRivalFunds.AnchorCash < 0 || auctionRivalFunds.AnchorCash > BaseCap(auctionRivalFunds.Venue) || auctionRivalFunds.Initial.Length != 3 || auctionRivalFunds.Remaining.Length != 3)
				{
					return null;
				}
				for (int i = 0; i < 3; i++)
				{
					if (auctionRivalFunds.Initial[i] < 0 || auctionRivalFunds.Remaining[i] < 0 || auctionRivalFunds.Remaining[i] > auctionRivalFunds.Initial[i] || (!auctionRivalFunds.Legacy && auctionRivalFunds.Initial[i] > BaseCap(auctionRivalFunds.Venue) * 3 / 2))
					{
						return null;
					}
				}
				string[] array2 = array[6].Split(',');
				foreach (string text in array2)
				{
					if (!string.IsNullOrEmpty(text))
					{
						auctionRivalFunds.Settled.Add(text);
					}
				}
				if (array.Length == 8 && array[7].Length > 0)
				{
					array2 = array[7].Split(',');
					for (int j = 0; j < array2.Length; j++)
					{
						string[] array3 = array2[j].Split('~');
						if (array3.Length != 2 || string.IsNullOrEmpty(array3[0]))
						{
							return null;
						}
						int[] array4 = array3[1].Split(':').Select(int.Parse).ToArray();
						if (array4.Length != 3 || array4.Any((int value) => value < 0))
						{
							return null;
						}
						for (int num2 = 0; num2 < 3; num2++)
						{
							if (array4[num2] > auctionRivalFunds.Initial[num2])
							{
								return null;
							}
						}
						if (!auctionRivalFunds.IntelPayments.TryAdd(array3[0], array4))
						{
							return null;
						}
					}
				}
				return auctionRivalFunds;
			}
			catch (FormatException)
			{
				return null;
			}
			catch (OverflowException)
			{
				return null;
			}
		}
	}
	internal enum AuctionVenue
	{
		Beginner,
		Intermediate,
		Advanced
	}
	internal enum AuctionQuality
	{
		Loss,
		Random,
		Profit,
		Jackpot,
		Hidden
	}
	internal static class AuctionRules
	{
		internal const int MerchantBudget = 100000;

		internal const int RestrictedNeuralChance = 10;

		internal const int HardItemLimit = 32;

		internal static bool IsRestrictedNeuralEligible(AuctionVenue venue, ContainerCategory category)
		{
			if (venue == AuctionVenue.Intermediate || venue == AuctionVenue.Advanced)
			{
				if (category != ContainerCategory.Contraband)
				{
					return category == ContainerCategory.Engineering;
				}
				return true;
			}
			return false;
		}

		internal static bool ShouldIncludeRestrictedNeural(AuctionVenue venue, ContainerCategory category, int lotSeed)
		{
			if (IsRestrictedNeuralEligible(venue, category))
			{
				return new Random(lotSeed ^ 0x5A17C0DE).Next(100) < 10;
			}
			return false;
		}

		internal static string VenueName(AuctionVenue venue)
		{
			return venue switch
			{
				AuctionVenue.Beginner => AuctionText.Select("新手场", "Beginner"), 
				AuctionVenue.Intermediate => AuctionText.Select("中级场", "Intermediate"), 
				_ => AuctionText.Select("高级场", "Advanced"), 
			};
		}

		internal static int EntryFee(AuctionVenue venue)
		{
			return venue switch
			{
				AuctionVenue.Beginner => 0, 
				AuctionVenue.Intermediate => 300, 
				_ => 1500, 
			};
		}

		internal static int RecommendedCash(AuctionVenue venue)
		{
			return venue switch
			{
				AuctionVenue.Beginner => 2000, 
				AuctionVenue.Intermediate => 8000, 
				_ => 35000, 
			};
		}

		internal static (int Min, int Max) BaselineRange(AuctionVenue venue)
		{
			return venue switch
			{
				AuctionVenue.Beginner => (Min: 200, Max: 1500), 
				AuctionVenue.Intermediate => (Min: 1500, Max: 7000), 
				_ => (Min: 7000, Max: 30000), 
			};
		}

		internal static (int Min, int Max) ItemCountRange(AuctionVenue venue)
		{
			return venue switch
			{
				AuctionVenue.Beginner => (Min: 5, Max: 10), 
				AuctionVenue.Intermediate => (Min: 8, Max: 16), 
				_ => (Min: 12, Max: 24), 
			};
		}

		internal static int SealedChance(AuctionVenue venue)
		{
			return venue switch
			{
				AuctionVenue.Beginner => 20, 
				AuctionVenue.Intermediate => 45, 
				_ => 65, 
			};
		}

		internal static (int Min, int Max) OpeningPercent(AuctionVenue venue)
		{
			return venue switch
			{
				AuctionVenue.Beginner => (Min: 35, Max: 51), 
				AuctionVenue.Intermediate => (Min: 45, Max: 61), 
				_ => (Min: 55, Max: 71), 
			};
		}

		internal static AuctionQuality RollQuality(Random random)
		{
			int num = random.Next(100);
			if (num < 14)
			{
				return AuctionQuality.Loss;
			}
			if (num < 74)
			{
				return AuctionQuality.Random;
			}
			if (num < 94)
			{
				return AuctionQuality.Profit;
			}
			if (num < 99)
			{
				return AuctionQuality.Jackpot;
			}
			return AuctionQuality.Hidden;
		}

		internal static int RollValuePercent(AuctionQuality quality, Random random)
		{
			return quality switch
			{
				AuctionQuality.Loss => random.Next(55, 81), 
				AuctionQuality.Random => random.Next(75, 136), 
				AuctionQuality.Profit => random.Next(145, 191), 
				AuctionQuality.Jackpot => random.Next(240, 361), 
				_ => random.Next(800, 1201), 
			};
		}

		internal static int ExclusiveSlots(AuctionQuality quality, int itemCount, Random random)
		{
			return quality switch
			{
				AuctionQuality.Loss => 0, 
				AuctionQuality.Random => (random.Next(200) == 0) ? 1 : 0, 
				AuctionQuality.Profit => (random.Next(10) == 0) ? 1 : 0, 
				AuctionQuality.Jackpot => 1, 
				_ => Math.Clamp((int)Math.Round((double)(itemCount * random.Next(60, 81)) / 100.0), 1, itemCount), 
			};
		}

		internal static int MaxExclusiveTier(AuctionVenue venue, AuctionQuality quality)
		{
			if (quality == AuctionQuality.Jackpot)
			{
				return venue switch
				{
					AuctionVenue.Intermediate => 2, 
					AuctionVenue.Beginner => 1, 
					_ => 3, 
				};
			}
			return (int)(venue + 1);
		}

		internal static int RollExclusiveTier(AuctionVenue venue, AuctionQuality quality, int seed)
		{
			int num = MaxExclusiveTier(venue, quality);
			if (num != 1)
			{
				switch (quality)
				{
				case AuctionQuality.Hidden:
					break;
				case AuctionQuality.Loss:
					return 1;
				default:
				{
					int num2 = new Random(seed ^ 0x5A17B49D).Next(100);
					int num3 = quality switch
					{
						AuctionQuality.Random => 8, 
						AuctionQuality.Profit => 16, 
						_ => 30, 
					};
					int num4 = quality switch
					{
						AuctionQuality.Random => 35, 
						AuctionQuality.Profit => 48, 
						_ => 55, 
					};
					if (num >= 3 && num2 < num3)
					{
						return 3;
					}
					if (num2 < num4 + ((num >= 3) ? num3 : 0))
					{
						return 2;
					}
					return 1;
				}
				}
			}
			return num;
		}

		internal static (int First, int Second) MerchantWeekdays(string runId, int week)
		{
			Random random = new Random(StableSeed((runId ?? "no-run") + "|merchant-days|" + Math.Max(0, week)));
			int num = random.Next(1, 8);
			int num2 = random.Next(1, 7);
			if (num2 >= num)
			{
				num2++;
			}
			if (num >= num2)
			{
				return (First: num2, Second: num);
			}
			return (First: num, Second: num2);
		}

		internal static ContainerCategory MerchantCategory(string runId, int week, int slot)
		{
			return (ContainerCategory)new Random(StableSeed((runId ?? "no-run") + "|merchant-kind|" + Math.Max(0, week) + "|" + Math.Clamp(slot, 0, 1))).Next(0, 4);
		}

		internal static bool TryGetMerchantVisit(string runId, int day, out int slot, out ContainerCategory category)
		{
			slot = -1;
			category = ContainerCategory.Medical;
			if (day <= 0)
			{
				return false;
			}
			int week = (day - 1) / 7;
			int num = (day - 1) % 7 + 1;
			(int, int) tuple = MerchantWeekdays(runId, week);
			if (num == tuple.Item1)
			{
				slot = 0;
			}
			else if (num == tuple.Item2)
			{
				slot = 1;
			}
			if (slot < 0)
			{
				return false;
			}
			category = MerchantCategory(runId, week, slot);
			return true;
		}

		private static int StableSeed(string text)
		{
			int num = 17;
			foreach (char c in text)
			{
				num = num * 31 + c;
			}
			if (num != int.MinValue)
			{
				return Math.Abs(num);
			}
			return 1;
		}
	}
	internal static class AuctionMerchantRules
	{
		internal static int Tier(long gifts)
		{
			if (gifts < 25000)
			{
				if (gifts < 10000)
				{
					return (gifts >= 3000) ? 1 : 0;
				}
				return 2;
			}
			return 3;
		}

		internal static int Threshold(int tier)
		{
			return tier switch
			{
				1 => 3000, 
				2 => 10000, 
				3 => 25000, 
				_ => 0, 
			};
		}

		internal static int Premium(long gifts)
		{
			return Tier(gifts) switch
			{
				1 => 3, 
				2 => 6, 
				3 => 10, 
				_ => 0, 
			};
		}

		internal static int Budget(long gifts)
		{
			return Tier(gifts) switch
			{
				1 => 120000, 
				2 => 150000, 
				3 => 200000, 
				_ => 100000, 
			};
		}

		internal static int CallInterval(long gifts)
		{
			if (Tier(gifts) != 3)
			{
				return 3;
			}
			return 2;
		}

		internal static long AddGift(long gifts, long value)
		{
			if (value > 0 && Tier(gifts) != 3)
			{
				return Math.Min(long.MaxValue - value, Math.Max(0L, gifts)) + value;
			}
			return Math.Max(0L, gifts);
		}

		internal static bool IsLegacyVisit(string source)
		{
			if (source == null || !source.StartsWith("xiaowo_night_auction_merchant_", StringComparison.Ordinal))
			{
				return false;
			}
			string[] array = source.Substring("xiaowo_night_auction_merchant_".Length).Split('_');
			if (array.Length == 2 && int.TryParse(array[0], out var result) && result >= 0 && int.TryParse(array[1], out var result2) && result2 >= 0)
			{
				return result2 <= 1;
			}
			return false;
		}
	}
	internal static class AuctionPhoneRules
	{
		internal const int IntroductionDay = 8;

		internal const int CallInterval = 3;

		internal static bool Eligible(int day)
		{
			return day >= 8;
		}

		internal static int ReadyDay(int calledDay)
		{
			return checked(calledDay + 3);
		}

		internal static int ReadyDay(int calledDay, long gifts)
		{
			return checked(calledDay + AuctionMerchantRules.CallInterval(gifts));
		}

		internal static bool CanReserve(int day, int readyDay, bool pending)
		{
			if (Eligible(day) && day >= readyDay)
			{
				return !pending;
			}
			return false;
		}

		internal static bool VisitDue(int day, int dueDay)
		{
			if (dueDay > 0)
			{
				return day >= dueDay;
			}
			return false;
		}

		internal static int StableSeed(string value)
		{
			int num = 17;
			string text = value ?? "";
			foreach (char c in text)
			{
				num = num * 31 + c;
			}
			return num & 0x7FFFFFFF;
		}

		internal static ContainerCategory Presenter(string runId)
		{
			int num = 17;
			string text = runId ?? string.Empty;
			foreach (char c in text)
			{
				num = num * 31 + c;
			}
			return (ContainerCategory)((uint)num % 4u);
		}
	}
	internal sealed class AuctionVisitSchedule
	{
		internal sealed class Visit
		{
			internal int Week;

			internal int Slot;

			internal int DueDay;

			internal ContainerCategory Category;
		}

		internal readonly List<Visit> Pending = new List<Visit>();

		internal void Remember(int week, int slot, ContainerCategory category, int dueDay)
		{
			if (week >= 0 && slot >= 0 && slot <= 1 && dueDay > 0 && category >= ContainerCategory.Medical && category <= ContainerCategory.Engineering && !Pending.Any((Visit v) => v.Week == week && v.Slot == slot))
			{
				Pending.Add(new Visit
				{
					Week = week,
					Slot = slot,
					Category = category,
					DueDay = dueDay
				});
			}
		}

		internal void DelayDue(int day)
		{
			foreach (Visit item in Pending)
			{
				if (item.DueDay <= day)
				{
					item.DueDay = checked(day + 1);
				}
			}
		}

		internal Visit Next(int day)
		{
			return (from v in Pending
				where v.DueDay <= day
				orderby v.Week, v.Slot
				select v).FirstOrDefault();
		}

		internal void Complete(int week, int slot)
		{
			Pending.RemoveAll((Visit v) => v.Week == week && v.Slot == slot);
		}

		internal string Encode()
		{
			return string.Join(";", (from v in Pending
				orderby v.Week, v.Slot
				select v).Select(delegate(Visit v)
			{
				string[] obj = new string[7]
				{
					v.Week.ToString(),
					",",
					v.Slot.ToString(),
					",",
					null,
					null,
					null
				};
				int category = (int)v.Category;
				obj[4] = category.ToString();
				obj[5] = ",";
				obj[6] = v.DueDay.ToString();
				return string.Concat(obj);
			}));
		}

		internal static AuctionVisitSchedule Decode(string text)
		{
			AuctionVisitSchedule auctionVisitSchedule = new AuctionVisitSchedule();
			string[] array = (text ?? "").Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(',');
				if (array2.Length == 4 && int.TryParse(array2[0], out var result) && int.TryParse(array2[1], out var result2) && int.TryParse(array2[2], out var result3) && int.TryParse(array2[3], out var result4))
				{
					auctionVisitSchedule.Remember(result, result2, (ContainerCategory)result3, result4);
				}
			}
			return auctionVisitSchedule;
		}
	}
	internal static class AuctionItemLinkRules
	{
		internal const string Calibrator = "xiaowo_auction_calibrator";

		internal const string Fabricator = "xiaowo_auction_fabricator_core";

		internal const string Controller = "xiaowo_auction_quantum_controller";

		internal const int CalibratorUses = 3;

		internal const int QualityBonus = 15;

		internal const int EfficiencyBonus = 10;

		internal const int DisplayCap = 15;

		internal static string NativeTemplate(string id)
		{
			switch (id)
			{
			case "xiaowo_auction_sterile_gauze":
				return "bandage_item";
			case "xiaowo_auction_hemostatic_foam":
				return "hemostatic_bandage_item";
			case "xiaowo_auction_neural_repair":
				return "native:SynaprestInjector";
			case "xiaowo_auction_braided_cable":
			case "xiaowo_auction_ceramic_fuse":
			case "xiaowo_auction_bearing_set":
				return "common_electronic";
			case "xiaowo_auction_calibrator":
				return "common_electronic";
			case "xiaowo_auction_fabricator_core":
				return "system_module_quality";
			case "xiaowo_auction_quantum_controller":
				return "system_module_efficiancy";
			default:
				return null;
			}
		}

		internal static bool IsRepairMaterial(string id)
		{
			switch (id)
			{
			case "xiaowo_auction_ceramic_fuse":
			case "xiaowo_auction_braided_cable":
			case "xiaowo_auction_bearing_set":
				return true;
			default:
				return false;
			}
		}

		internal static bool IsModule(string id)
		{
			if (id == "xiaowo_auction_fabricator_core" || id == "xiaowo_auction_quantum_controller")
			{
				return true;
			}
			return false;
		}

		internal static long CalibratorValue(long fullValue, int remaining)
		{
			return fullValue * Math.Clamp(remaining, 0, 3) / 3;
		}

		internal static int DisplayBonus(string id)
		{
			return id switch
			{
				"xiaowo_auction_colony_painting" => 3, 
				"xiaowo_auction_gilt_music_box" => 5, 
				"xiaowo_auction_platinum_trophy" => 8, 
				_ => 0, 
			};
		}

		internal static int DisplayBonus(IEnumerable<string> ids)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
			int num = 0;
			foreach (string id in ids)
			{
				if (id != null && hashSet.Add(id))
				{
					num += DisplayBonus(id);
				}
			}
			return Math.Min(15, num);
		}

		internal static void Configure(Dictionary<string, AuctionExclusiveSpec> items, Dictionary<string, (string Name, string Description)> english)
		{
			foreach (AuctionExclusiveSpec value in items.Values)
			{
				string text = NativeTemplate(value.Id);
				if (text != null)
				{
					value.NativeTemplate = text;
				}
			}
			Describe("sterile_gauze", "双击使用，按原版普通绷带的条件处理伤势，使用后消耗。", "Double-click to treat wounds with the native ordinary bandage rules. Consumed on use.");
			Describe("hemostatic_foam", "双击使用，按原版止血绷带的条件稳定伤势，使用后消耗。", "Double-click to stabilize wounds with native hemostatic bandage rules. Consumed on use.");
			Describe("neural_repair", "双击使用，按原版Synaprest药剂随机解除一种成瘾；没有成瘾时不可使用，成功使用后消耗。", "Double-click to remove one random addiction using native Synaprest rules. Unusable without an addiction; consumed on use.");
			string[] array = new string[3] { "ceramic_fuse", "braided_cable", "bearing_set" };
			for (int i = 0; i < array.Length; i++)
			{
				Describe(array[i], "拖到受损模组或模组提取器上维修，沿用原版电子零件的条件、成功率和消耗。不用于其他设备升级。", "Drag onto a damaged module or module extractor to repair it with native electronic-part conditions, chances and consumption. Does not upgrade other equipment.");
			}
			Describe("calibrator", "拖到已卡住的自有模组上，解除卡住状态。剩余次数：{0}/3。每次成功消耗1次，价值按剩余次数降低，耗尽后销毁。", "Drag onto an owned stuck module to release it. Uses remaining: {0}/3. Each successful release uses one charge; value scales with remaining charges. Consumed after the final use.");
			Describe("fabricator_core", "可安装在原版兼容的模组槽中：品质加成15%。保留原版安装限制、磨损和拆卸规则。", "Install in a compatible native module slot: +15% quality. Native slot restrictions, wear and removal rules apply.");
			Describe("quantum_controller", "可安装在原版兼容的模组槽中：效率加成10%。保留原版安装限制、磨损和拆卸规则。", "Install in a compatible native module slot: +10% efficiency. Native slot restrictions, wear and removal rules apply.");
			array = new string[3] { "colony_painting", "gilt_music_box", "platinum_trophy" };
			foreach (string text2 in array)
			{
				int num = DisplayBonus("xiaowo_auction_" + text2);
				Describe(text2, $"放入橱窗时额外增加{num}点店铺吸引力，取出即失效。同款不叠加，三款合计最多15点；固定加成在特性倍率后计算。", $"Display in your showcase for +{num} store attractiveness after perk multipliers. Removed when taken out. Identical items do not stack; the three collectibles grant up to 15 in total.");
			}
			void Describe(string suffix, string zh, string en)
			{
				string key = "xiaowo_auction_" + suffix;
				items[key].Description = zh;
				english[key] = (english[key].Name, en);
			}
		}
	}
	internal static class ExpressEvidencePricing
	{
		internal static int Quote(int nativeCost, int remaining, int duration)
		{
			int num = Math.Min(Math.Max(remaining, 0), Math.Max(1, duration));
			return nativeCost + (3000 * num + Math.Max(1, duration) - 1) / Math.Max(1, duration);
		}

		internal static bool Available(int state, int remaining, bool prerequisite, bool demoLocked, bool usedToday, int cash, int price)
		{
			if (state == 1 && remaining > 0 && prerequisite && !demoLocked && !usedToday)
			{
				return cash >= price;
			}
			return false;
		}
	}
	internal static class GhostIdentityRules
	{
		internal static bool Available(int evidence, int enforcementState, long crime)
		{
			if (evidence >= 0 && evidence < 100 && enforcementState == 0 && crime >= 0)
			{
				if (evidence <= 0)
				{
					return crime > 1;
				}
				return true;
			}
			return false;
		}

		internal static int[] Half(IReadOnlyList<int> amounts)
		{
			if (amounts == null)
			{
				throw new ArgumentNullException("amounts");
			}
			int[] array = new int[amounts.Count];
			long num = 0L;
			long num2 = 0L;
			for (int i = 0; i < amounts.Count; i++)
			{
				if (amounts[i] < 0)
				{
					throw new ArgumentOutOfRangeException("amounts");
				}
				num += amounts[i];
				array[i] = amounts[i] / 2;
				num2 += array[i];
			}
			long num3 = num - num / 2 - num2;
			int num4 = 0;
			while (num3 > 0 && num4 < amounts.Count)
			{
				if ((amounts[num4] & 1) != 0)
				{
					array[num4]++;
					num3--;
				}
				num4++;
			}
			return array;
		}
	}
	internal static class AuctionNewItems
	{
		internal static void Add(Dictionary<string, AuctionExclusiveSpec> items, Dictionary<string, (string Name, string Description)> english)
		{
			Add(items, english, ContainerCategory.Medical, "sterile_gauze", 120, 1, 1, 1, "hemostatic_foam", "无菌纱布包", "独立封装的医疗纱布，适合出售给医疗物资收购商。", "Sterile Gauze Pack", "Individually sealed medical gauze for medical supply buyers.", "junk");
			Add(items, english, ContainerCategory.Medical, "burn_gel", 260, 1, 1, 2, "neural_repair", "冷敷凝胶管", "用于临床冷敷的密封凝胶管。", "Cooling Gel Tube", "A sealed tube of clinical cooling gel.", "junk");
			Add(items, english, ContainerCategory.Medical, "field_dressing", 480, 1, 2, 1, "nano_suture", "战地止血敷料", "双击使用，按原版止血绷带的条件和效果处理伤势，使用后消耗。", "Field Hemostatic Dressing", "Double-click to use. Uses the native hemostatic bandage conditions and wound treatment; consumed on use.", "hemostatic_bandage_item");
			Add(items, english, ContainerCategory.Medical, "rescue_salts", 750, 1, 1, 1, "hemostatic_foam", "急救醒神盐", "沿用原版嗅盐的恢复意识效果与使用条件；仅在原版允许的场合使用，使用后消耗。", "Rescue Smelling Salts", "Uses native smelling salt recovery effects and restrictions; usable only where the native item permits it, and consumed on use.", "smelling_salt");
			Add(items, english, ContainerCategory.Medical, "diagnostic_probe", 1800, 1, 2, 1, "neural_repair", "便携诊断探针", "用于精细检查的医疗探针，探头和接口保存完好。", "Diagnostic Probe", "A medical inspection probe with its sensor and connector intact.", "junk");
			Add(items, english, ContainerCategory.Medical, "sterile_instruments", 2600, 1, 1, 3, "nano_suture", "无菌手术器械匣", "完整封签的长型手术器械匣。", "Sterile Surgical Instrument Case", "A long case of surgical instruments with an intact sterile seal.", "junk");
			Add(items, english, ContainerCategory.Medical, "plasma_cooler", 3900, 1, 2, 2, "organ_case", "血浆冷藏箱", "保存血浆样本的医用冷藏设备，密封层未受损。", "Plasma Cooler", "A medical plasma transport cooler with undamaged seals.", "junk");
			Add(items, english, ContainerCategory.Medical, "patient_monitor", 6200, 1, 2, 2, "blood_filter", "多参数监护仪", "旧医院留下的多参数监护设备，收藏与转售价值较高。", "Patient Monitor", "A multi-parameter hospital monitor valued by collectors and medical buyers.", "junk");
			Add(items, english, ContainerCategory.Medical, "surgical_arm", 16000, 2, 2, 3, "organ_matrix", "微创手术机械臂", "用于精密微创手术的封装机械臂。", "Microsurgery Arm", "A sealed robotic arm for precision microsurgery.", "junk");
			Add(items, english, ContainerCategory.Medical, "bioprint_console", 52000, 3, 3, 2, "organ_matrix", "临床生物打印终端", "带有授权临床数据的稀有生物打印终端。", "Clinical Bioprint Console", "A rare medical bioprint console containing licensed clinical data.", "junk");
			Add(items, english, ContainerCategory.Luxury, "enamel_brooch", 180, 1, 1, 1, "micro_engraving", "珐琅纪念胸针", "带有旧殖民地徽记的小型珐琅胸针。", "Enamel Commemorative Brooch", "A small enamel brooch bearing an old colony emblem.", "junk");
			Add(items, english, ContainerCategory.Luxury, "silver_lighter", 360, 1, 1, 2, "pocket_watch", "银壳收藏打火机", "银壳上刻有工坊编号的收藏打火机。", "Silver Collector's Lighter", "A collector's lighter with a workshop number engraved on its silver case.", "junk");
			Add(items, english, ContainerCategory.Luxury, "silk_scarf", 720, 1, 2, 1, "zero_g_perfume", "轨道丝绸方巾", "轻薄的轨道织坊丝绸方巾。", "Orbital Silk Scarf", "A fine silk scarf woven in an orbital workshop.", "junk");
			Add(items, english, ContainerCategory.Luxury, "amber_reserve", 980, 1, 1, 2, "century_vintage", "琥珀珍藏啤酒", "双击饮用，沿用原版红啤酒的效果与使用条件，饮用后消耗。", "Amber Reserve Beer", "Double-click to drink with the native red beer effects and conditions; consumed on use.", "red_beer", "ALCOHOL");
			Add(items, english, ContainerCategory.Luxury, "pearl_necklace", 1900, 1, 1, 2, "zero_g_perfume", "月港珍珠项链", "由月港珠匠配对打磨的珍珠项链。", "Moonport Pearl Necklace", "A pearl necklace matched and polished by Moonport jewellers.", "junk");
			Add(items, english, ContainerCategory.Luxury, "gilt_music_box", 3400, 1, 2, 2, "orbital_watch", "鎏金机械音乐盒", "仍保有精细发条结构的鎏金音乐盒。", "Gilt Mechanical Music Box", "A gilt music box with its intricate clockwork preserved.", "junk");
			Add(items, english, ContainerCategory.Luxury, "colony_painting", 5800, 1, 3, 2, "micro_engraving", "殖民时代风景画", "装裱完整的殖民时代风景画。", "Colonial Landscape Painting", "An intact framed landscape from the colonial era.", "junk");
			Add(items, english, ContainerCategory.Luxury, "herald_signet", 12000, 2, 1, 1, "black_diamond", "纹章蓝宝石印戒", "嵌有蓝宝石的贵族纹章印戒。", "Sapphire Heraldic Signet", "An aristocratic signet ring set with a sapphire.", "junk");
			Add(items, english, ContainerCategory.Luxury, "platinum_trophy", 24000, 2, 2, 3, "micro_engraving", "星际赛事白金奖杯", "限量铸造的白金赛事奖杯，底座编号清晰。", "Interstellar Platinum Trophy", "A limited platinum tournament trophy with a legible numbered base.", "junk");
			Add(items, english, ContainerCategory.Luxury, "imperial_diadem", 56000, 3, 3, 2, "black_diamond", "旧帝国星辉冠冕", "镶嵌多颗宝石的旧帝国礼仪冠冕。", "Imperial Starlight Diadem", "An old imperial ceremonial diadem set with multiple gemstones.", "junk");
			Add(items, english, ContainerCategory.Contraband, "spoof_entry_strip", 220, 1, 2, 1, "forged_pass", "伪造门禁编码条", "已失效的伪造门禁编码条，地下收购商仍愿意回收。", "Spoofed Entry Strip", "An expired counterfeit entry strip still sought by underground buyers.", "junk");
			Add(items, english, ContainerCategory.Contraband, "cloned_service_card", 650, 1, 2, 1, "forged_pass", "克隆服务钥匙卡", "沿用原版服务钥匙卡的锁箱规则，对需要服务权限的锁箱使用后消耗一张。", "Cloned Service Keycard", "Uses native service keycard lock rules; one card is consumed when opening a crate requiring service access.", "ser_keycard", "ACCESS_CARD");
			Add(items, english, ContainerCategory.Contraband, "sealed_illegal_ammo", 1050, 1, 1, 2, "silent_bolt", "封签违禁弹药样本", "查封编号被抹去的弹药样本，作为违禁货物转售。", "Sealed Illicit Ammunition Sample", "An ammunition sample with its seizure number erased, traded as illicit goods.", "junk", "WEAPON");
			Add(items, english, ContainerCategory.Contraband, "signal_jammer", 2100, 1, 2, 2, "black_decoder", "便携信号屏蔽器", "来源不明的信号屏蔽设备，属于受管制货物。", "Portable Signal Jammer", "An unregistered signal-blocking device traded as controlled goods.", "junk", "MODULE");
			Add(items, english, ContainerCategory.Contraband, "clone_electrode", 3300, 1, 1, 2, "silent_bolt", "非法克隆电极组", "地下实验室流出的精密电极组件。", "Illicit Cloning Electrodes", "Precision electrode components leaked from an underground laboratory.", "junk");
			Add(items, english, ContainerCategory.Contraband, "outlaw_suppressor", 4800, 1, 3, 1, "silent_bolt", "无登记消音组件", "没有制造编号的受管制枪械组件。", "Unregistered Suppressor Assembly", "Controlled firearm components without a manufacturer serial number.", "junk", "WEAPON");
			Add(items, english, ContainerCategory.Contraband, "smuggling_route_drive", 6800, 1, 2, 2, "cipher_ledger", "走私航线数据盘", "存有私运航线记录的加密数据盘。", "Smuggling Route Drive", "An encrypted drive containing illicit shipping route records.", "junk");
			Add(items, english, ContainerCategory.Contraband, "record_scrambler", 14000, 2, 2, 2, "ghost_identity", "查封记录扰码器", "地下渠道交易的受管制记录设备，收藏价值取决于完整封装。", "Seizure Record Scrambler", "A controlled recording device traded underground, valued for its intact casing.", "junk", "MODULE");
			Add(items, english, ContainerCategory.Contraband, "biometric_spoofer", 27000, 2, 2, 3, "master_key", "生物签名伪造终端", "被禁止流通的生物识别研究终端。", "Biometric Spoofing Terminal", "A biometric research terminal prohibited from public circulation.", "junk", "MODULE");
			Add(items, english, ContainerCategory.Contraband, "root_access_lattice", 54000, 3, 3, 3, "master_key", "黑市根权限矩阵", "完整封装的军事权限研究矩阵，黑市藏家争相收购。", "Black-Market Root Access Lattice", "A sealed military access research lattice coveted by underground collectors.", "junk");
			Add(items, english, ContainerCategory.Engineering, "ceramic_fuse", 80, 1, 1, 1, "bearing_set", "陶瓷保险芯", "廉价但实用的工业保险芯。", "Ceramic Fuse Core", "An inexpensive industrial fuse core.", "junk", "MATERIAL");
			Add(items, english, ContainerCategory.Engineering, "braided_cable", 190, 1, 2, 1, "supercoil", "屏蔽编织线束", "接口完整的屏蔽工业线束。", "Shielded Braided Cable", "Shielded industrial wiring with intact connectors.", "junk", "MATERIAL");
			Add(items, english, ContainerCategory.Engineering, "coolant_cartridge", 420, 1, 1, 2, "microreactor", "密封冷却液筒", "仍保持密封的工业冷却液筒。", "Sealed Coolant Cartridge", "An industrial coolant cartridge with an intact seal.", "junk", "MATERIAL");
			Add(items, english, ContainerCategory.Engineering, "hydraulic_coupling", 800, 1, 1, 2, "bearing_set", "高压液压接头", "适用于高压管线的金属液压接头。", "High-Pressure Hydraulic Coupling", "A metal coupling for high-pressure hydraulic lines.", "junk", "MATERIAL");
			Add(items, english, ContainerCategory.Engineering, "precision_gears", 1600, 1, 2, 2, "bearing_set", "精密减速齿轮组", "成套出售的精密减速齿轮。", "Precision Reduction Gears", "A complete set of precision reduction gears.", "junk", "MATERIAL");
			Add(items, english, ContainerCategory.Engineering, "induction_sensor", 2800, 1, 2, 1, "calibrator", "工业感应探头", "用于自动化设备的高精度感应探头。", "Industrial Induction Sensor", "A precision induction sensor for automated equipment.", "junk", "TOOL");
			Add(items, english, ContainerCategory.Engineering, "sealed_servo_motor", 4600, 1, 2, 3, "fabricator_core", "密封伺服电机", "保留出厂封签的高扭矩伺服电机。", "Sealed Servo Motor", "A high-torque servo motor retaining its factory seal.", "junk", "MATERIAL");
			Add(items, english, ContainerCategory.Engineering, "precision_lathe_head", 11000, 2, 3, 2, "control_core", "精密加工主轴头", "旧式精密机床的完整主轴总成。", "Precision Lathe Spindle Head", "An intact spindle assembly from a precision machine tool.", "junk", "MACHINE", "STANDARD_MACHINE_TAG");
			Add(items, english, ContainerCategory.Engineering, "quantum_controller", 23000, 2, 2, 2, "control_core", "量子同步控制板", "稀有工业设备使用的同步控制板。", "Quantum Synchronization Board", "A synchronization board used in rare industrial machinery.", "junk", "MODULE");
			Add(items, english, ContainerCategory.Engineering, "grav_flywheel", 55000, 3, 3, 3, "zero_point", "重力稳定飞轮", "用于极端工况的高价值稳定装置。", "Gravitic Stabilizing Flywheel", "A valuable stabilization unit engineered for extreme operating conditions.", "junk", "MACHINE", "STANDARD_MACHINE_TAG");
		}

		private static void Add(Dictionary<string, AuctionExclusiveSpec> items, Dictionary<string, (string Name, string Description)> english, ContainerCategory category, string suffix, int value, int tier, int width, int height, string sprite, string name, string description, string nameEn, string descriptionEn, string nativeTemplate = "junk", params string[] extraTags)
		{
			string text = "xiaowo_auction_" + suffix;
			List<string> list = new List<string>();
			List<string> list2 = list;
			list2.Add(category switch
			{
				ContainerCategory.Medical => "MEDICAL", 
				ContainerCategory.Luxury => "LUXURY_ITEM", 
				ContainerCategory.Contraband => "CONTRABAND_ITEM_TAG", 
				_ => "MATERIAL", 
			});
			List<string> list3 = list;
			if (category == ContainerCategory.Engineering && extraTags.Length != 0)
			{
				list3.Clear();
			}
			list3.AddRange(extraTags);
			AuctionExclusiveSpec auctionExclusiveSpec = AuctionExclusiveCatalog.Spec(text, category, tier, value, name, description, list3.ToArray());
			auctionExclusiveSpec.Width = width;
			auctionExclusiveSpec.Height = height;
			auctionExclusiveSpec.SpriteId = text;
			auctionExclusiveSpec.NativeTemplate = nativeTemplate;
			items.Add(text, auctionExclusiveSpec);
			english.Add(text, (nameEn, descriptionEn));
		}
	}
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '9.1.0.7988')
