using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using TMPro;
using UnityEngine;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyCompany("ProbablyStolenKoreanPack")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyProduct("ProbablyStolenKoreanPack")]
[assembly: AssemblyTitle("ProbablyStolenKoreanPack")]
[assembly: AssemblyVersion("1.0.0.0")]

namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
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
namespace ProbablyStolenKoreanPack
{
	public sealed class KoreanPack : IMod
	{
		private sealed class Catalog
		{
			internal string PrimaryId;

			internal readonly HashSet<string> Aliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			internal readonly Dictionary<string, string> ByKey = new Dictionary<string, string>(StringComparer.Ordinal);

			internal readonly Dictionary<string, string> ExplicitBySource = new Dictionary<string, string>(StringComparer.Ordinal);

			internal readonly HashSet<string> AmbiguousExplicitSources = new HashSet<string>(StringComparer.Ordinal);

			internal readonly Dictionary<string, string> DerivedBySource = new Dictionary<string, string>(StringComparer.Ordinal);

			internal readonly HashSet<string> AmbiguousDerivedSources = new HashSet<string>(StringComparer.Ordinal);

			internal readonly List<TemplateRule> ExplicitTemplates = new List<TemplateRule>();

			internal readonly List<TemplateRule> DerivedTemplates = new List<TemplateRule>();
		}

		private sealed class TemplateRule
		{
			internal string Source;

			internal string Korean;

			internal string[] Literals;

			internal string[] Tokens;
		}

		private sealed class SavedValue
		{
			internal string Locale;

			internal string Key;

			internal bool Existed;

			internal string Value;
		}

		private sealed class MemberMutation
		{
			internal object Target;

			internal FieldInfo Field;

			internal PropertyInfo Property;

			internal string Original;
		}

		private sealed class MutationFrame
		{
			internal readonly List<MemberMutation> Mutations = new List<MemberMutation>();
		}

		private sealed class ReferenceComparer : IEqualityComparer<object>
		{
			internal static readonly ReferenceComparer Instance = new ReferenceComparer();

			public new bool Equals(object x, object y)
			{
				return x == y;
			}

			public int GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}
		}

		private enum KnownPatchKind
		{
			StringResult,
			StringArrayResult,
			BilingualResult,
			VisibleStringArgs,
			GuiLabelTranspiler,
			LocalizerResult,
			GameItemResult,
			GameItemWindowResult,
			PixelWindowResult,
			GameItemWindowArgs,
			BuyerTypeListArgs,
			StorageBoxLabelState,
			TentacleNightReportArgs,
			StoreClientResult,
			DialogueResult,
			ObjectArgs,
			StaticDisplayState
		}

		private sealed class KnownPatch
		{
			internal readonly string MethodName;

			internal readonly KnownPatchKind Kind;

			internal KnownPatch(string methodName, KnownPatchKind kind)
			{
				MethodName = methodName;
				Kind = kind;
			}
		}

		private const string OwnModId = "probably_stolen_korean_pack";

		private const string CatalogPrefix = "pskb__";

		private const string DynamicHarmonyId = "probably_stolen_korean_pack.target_text";

		[ThreadStatic]
		private static Stack<MutationFrame> _mutationFrames;

		private static readonly object Sync = new object();

		private static readonly Dictionary<string, Catalog> Catalogs = new Dictionary<string, Catalog>(StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<string> AmbiguousAliases = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static readonly Dictionary<Assembly, Catalog> AssemblyCatalogs = new Dictionary<Assembly, Catalog>();

		private static readonly Dictionary<string, Catalog> RuntimeCatalogs = new Dictionary<string, Catalog>(StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<string> FinalTooltipHitLogged = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static readonly HashSet<string> FinalTooltipMissLogged = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static FieldInfo _richTextSlicesField;

		private static readonly Dictionary<Type, FieldInfo> SliceTextFields = new Dictionary<Type, FieldInfo>();

		private static readonly Dictionary<Type, FieldInfo> SliceLengthFields = new Dictionary<Type, FieldInfo>();

		private static readonly Dictionary<LoadedMod, List<SavedValue>> SavedValues = new Dictionary<LoadedMod, List<SavedValue>>();

		private static readonly HashSet<string> PatchedMethods = new HashSet<string>(StringComparer.Ordinal);

		private static readonly List<LoadedMod> PendingMods = new List<LoadedMod>();

		private static Harmony _dynamicHarmony;

		private static bool _enabled;

		private static bool _catalogsReady;

		private static TMP_FontAsset _koreanTmpFont;

		public void Init(ModManifest manifest)
		{
		}

		public void OnEnable()
		{
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			lock (Sync)
			{
				if (!_enabled)
				{
					Catalogs.Clear();
					AmbiguousAliases.Clear();
					AssemblyCatalogs.Clear();
					RuntimeCatalogs.Clear();
					FinalTooltipHitLogged.Clear();
					FinalTooltipMissLogged.Clear();
					SliceTextFields.Clear();
					SliceLengthFields.Clear();
					_richTextSlicesField = null;
					SavedValues.Clear();
					PatchedMethods.Clear();
					PendingMods.Clear();
					_dynamicHarmony = new Harmony("probably_stolen_korean_pack.target_text");
					_koreanTmpFont = null;
					_catalogsReady = false;
					_enabled = true;
					ModHook.OnModAssetsLoaded += OnModAssetsLoaded;
					PatchFinalTooltipBridge();
					PatchSharedDisplayBridges();
					Debug.Log((object)"[KoreanPack] Enabled. Waiting for official ModHook.OnModAssetsLoaded.");
				}
			}
		}

		public void OnDisable()
		{
			lock (Sync)
			{
				if (_enabled)
				{
					_enabled = false;
					ModHook.OnModAssetsLoaded -= OnModAssetsLoaded;
					RestoreNativeEntries();
					UnpatchDynamic();
					Catalogs.Clear();
					AmbiguousAliases.Clear();
					AssemblyCatalogs.Clear();
					RuntimeCatalogs.Clear();
					FinalTooltipHitLogged.Clear();
					FinalTooltipMissLogged.Clear();
					PatchedMethods.Clear();
					PendingMods.Clear();
					_catalogsReady = false;
					_koreanTmpFont = null;
					Debug.Log((object)"[KoreanPack] Disabled.");
				}
			}
		}

		private static void OnModAssetsLoaded(LoadedMod loaded)
		{
			if (!_enabled || loaded == null || loaded.Manifest == null)
			{
				return;
			}
			lock (Sync)
			{
				try
				{
					if (string.Equals(loaded.Manifest.ID, "probably_stolen_korean_pack", StringComparison.OrdinalIgnoreCase))
					{
						BuildCatalogsFromOwnLocalization(loaded);
						_catalogsReady = true;
						if (PendingMods.Count > 0)
						{
							LoadedMod[] array = PendingMods.ToArray();
							PendingMods.Clear();
							LoadedMod[] array2 = array;
							for (int i = 0; i < array2.Length; i++)
							{
								ApplyToLoadedMod(array2[i]);
							}
						}
					}
					else if (!_catalogsReady)
					{
						if (!PendingMods.Contains(loaded))
						{
							PendingMods.Add(loaded);
						}
					}
					else
					{
						ApplyToLoadedMod(loaded);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError((object)("[KoreanPack] OnModAssetsLoaded failed for " + SafeModName(loaded) + ": " + ex));
				}
			}
		}

		private static void BuildCatalogsFromOwnLocalization(LoadedMod own)
		{
			Catalogs.Clear();
			AmbiguousAliases.Clear();
			if (own.Localization == null)
			{
				Debug.LogWarning((object)"[KoreanPack] Own Localization dictionary is null.");
				return;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (KeyValuePair<string, Dictionary<string, string>> item in own.Localization)
			{
				string text = item.Key ?? string.Empty;
				if (!text.StartsWith("pskb__", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				string text2 = text.Substring("pskb__".Length).Trim();
				if (string.IsNullOrEmpty(text2) || item.Value == null)
				{
					continue;
				}
				Catalog catalog = new Catalog
				{
					PrimaryId = text2
				};
				catalog.Aliases.Add(text2);
				foreach (KeyValuePair<string, string> item2 in item.Value)
				{
					string text3 = item2.Key ?? string.Empty;
					string text4 = item2.Value ?? string.Empty;
					if (text3.StartsWith("M|", StringComparison.Ordinal))
					{
						AddAlias(catalog, text3.Substring(2));
					}
					else if (text3.StartsWith("W|", StringComparison.Ordinal))
					{
						AddAlias(catalog, text3.Substring(2));
					}
					else
					{
						if (string.IsNullOrEmpty(text4))
						{
							continue;
						}
						if (text3.StartsWith("K|", StringComparison.Ordinal))
						{
							string text5 = text3.Substring(2);
							if (!string.IsNullOrEmpty(text5))
							{
								catalog.ByKey[text5] = text4;
							}
						}
						else if (text3.StartsWith("K64|", StringComparison.Ordinal))
						{
							string text6 = DecodeBase64Url(text3.Substring(4));
							if (!string.IsNullOrEmpty(text6))
							{
								catalog.ByKey[text6] = text4;
							}
						}
						else if (text3.StartsWith("S|", StringComparison.Ordinal))
						{
							AddExplicitSource(catalog, text3.Substring(2), text4);
						}
						else if (text3.StartsWith("S64|", StringComparison.Ordinal))
						{
							string text7 = DecodeBase64Url(text3.Substring(4));
							if (!string.IsNullOrEmpty(text7))
							{
								AddExplicitSource(catalog, text7, text4);
							}
						}
					}
				}
				if (catalog.ByKey.Count != 0 || catalog.ExplicitBySource.Count != 0)
				{
					RegisterCatalogAliases(catalog);
					num++;
					num2 += catalog.ByKey.Count;
					num3 += catalog.ExplicitBySource.Count;
				}
			}
			Debug.Log((object)("[KoreanPack] Catalogs ready: " + num + " catalog(s), " + num2 + " key translation(s), " + num3 + " source translation(s)."));
		}

		private static void AddAlias(Catalog catalog, string alias)
		{
			if (catalog != null && !string.IsNullOrWhiteSpace(alias))
			{
				catalog.Aliases.Add(alias.Trim());
			}
		}

		private static void RegisterCatalogAliases(Catalog catalog)
		{
			if (catalog == null)
			{
				return;
			}
			string[] array = catalog.Aliases.ToArray();
			Catalog catalog2 = null;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!string.IsNullOrWhiteSpace(text) && !AmbiguousAliases.Contains(text) && Catalogs.TryGetValue(text, out var value))
				{
					if (catalog2 == null)
					{
						catalog2 = value;
					}
					else if (catalog2 != value)
					{
						Catalogs.Remove(text);
						AmbiguousAliases.Add(text);
						Debug.LogWarning((object)("[KoreanPack] Ambiguous catalog alias ignored: " + text));
					}
				}
			}
			if (catalog2 != null && catalog2 != catalog)
			{
				MergeCatalog(catalog2, catalog);
				catalog = catalog2;
			}
			array2 = catalog.Aliases.ToArray();
			foreach (string text2 in array2)
			{
				if (!string.IsNullOrWhiteSpace(text2) && !AmbiguousAliases.Contains(text2))
				{
					if (!Catalogs.TryGetValue(text2, out var value2))
					{
						Catalogs[text2] = catalog;
					}
					else if (value2 != catalog)
					{
						Catalogs.Remove(text2);
						AmbiguousAliases.Add(text2);
						Debug.LogWarning((object)("[KoreanPack] Ambiguous catalog alias ignored: " + text2));
					}
				}
			}
		}

		private static void MergeCatalog(Catalog target, Catalog incoming)
		{
			if (target == null || incoming == null || target == incoming)
			{
				return;
			}
			foreach (string alias in incoming.Aliases)
			{
				target.Aliases.Add(alias);
			}
			bool flag = IsSupplementCatalog(target);
			bool flag2 = IsSupplementCatalog(incoming) && !flag;
			foreach (KeyValuePair<string, string> item in incoming.ByKey)
			{
				if (!target.ByKey.TryGetValue(item.Key, out var _) | flag2)
				{
					target.ByKey[item.Key] = item.Value;
				}
			}
			foreach (KeyValuePair<string, string> item2 in incoming.ExplicitBySource)
			{
				if (!target.ExplicitBySource.TryGetValue(item2.Key, out var _))
				{
					target.ExplicitBySource[item2.Key] = item2.Value;
				}
				else if (flag2)
				{
					target.ExplicitBySource[item2.Key] = item2.Value;
				}
			}
			target.AmbiguousExplicitSources.Clear();
			RebuildExplicitTemplates(target);
		}

		private static bool IsSupplementCatalog(Catalog catalog)
		{
			if (catalog != null && !string.IsNullOrEmpty(catalog.PrimaryId))
			{
				return catalog.PrimaryId.StartsWith("supp_", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		private static void RebuildExplicitTemplates(Catalog catalog)
		{
			if (catalog == null)
			{
				return;
			}
			catalog.ExplicitTemplates.Clear();
			foreach (KeyValuePair<string, string> item in catalog.ExplicitBySource)
			{
				AddTemplateRule(catalog.ExplicitTemplates, item.Key, item.Value);
			}
		}

		private static string DecodeBase64Url(string encoded)
		{
			if (string.IsNullOrEmpty(encoded))
			{
				return string.Empty;
			}
			try
			{
				string text = encoded.Replace('-', '+').Replace('_', '/');
				switch (text.Length % 4)
				{
				case 2:
					text += "==";
					break;
				case 3:
					text += "=";
					break;
				}
				byte[] bytes = Convert.FromBase64String(text);
				return Encoding.UTF8.GetString(bytes);
			}
			catch
			{
				return string.Empty;
			}
		}

		private static void ApplyToLoadedMod(LoadedMod loaded)
		{
			if (loaded != null && loaded.Manifest != null && !string.Equals(loaded.Manifest.ID, "probably_stolen_korean_pack", StringComparison.OrdinalIgnoreCase))
			{
				Catalog catalog = LocateCatalog(loaded);
				if (catalog != null)
				{
					RegisterRuntimeCatalogAliases(loaded, catalog);
					MergeNativeEntries(loaded, catalog);
					RebuildDerivedSourceMap(loaded, catalog);
					PatchTargetAssembly(loaded, catalog);
					Debug.Log((object)("[KoreanPack] Applied to " + SafeModName(loaded) + ": " + catalog.ByKey.Count + " key(s), " + (catalog.ExplicitBySource.Count + catalog.DerivedBySource.Count) + " source mapping(s)."));
				}
			}
		}

		private static void RegisterRuntimeCatalogAliases(LoadedMod loaded, Catalog catalog)
		{
			if (loaded == null || loaded.Manifest == null || catalog == null)
			{
				return;
			}
			Action<string> action = delegate(string alias)
			{
				if (!string.IsNullOrWhiteSpace(alias))
				{
					RuntimeCatalogs[alias.Trim()] = catalog;
				}
			};
			action(loaded.Manifest.ID);
			action(loaded.Manifest.Name);
			if (loaded.WorkshopId != 0L)
			{
				action(loaded.WorkshopId.ToString());
			}
			if (loaded.ShadowedWorkshopId != 0L)
			{
				action(loaded.ShadowedWorkshopId.ToString());
			}
		}

		private static Catalog LocateCatalogForItem(GameItem item, out string runtimeId)
		{
			runtimeId = string.Empty;
			if (item == null)
			{
				return null;
			}
			try
			{
				Catalog value;
				if (item.IsTag("MOD_ID_TAG"))
				{
					runtimeId = item.GetTagReadonly("MOD_ID_TAG").GetString() ?? string.Empty;
					if (!string.IsNullOrWhiteSpace(runtimeId))
					{
						if (RuntimeCatalogs.TryGetValue(runtimeId, out value))
						{
							return value;
						}
						if (Catalogs.TryGetValue(runtimeId, out value))
						{
							return value;
						}
					}
				}
				if (item.IsTag("MOD_NAME_TAG"))
				{
					string text = item.GetTagReadonly("MOD_NAME_TAG").GetString();
					if (!string.IsNullOrWhiteSpace(text))
					{
						if (RuntimeCatalogs.TryGetValue(text, out value))
						{
							if (string.IsNullOrEmpty(runtimeId))
							{
								runtimeId = text;
							}
							return value;
						}
						if (Catalogs.TryGetValue(text, out value))
						{
							if (string.IsNullOrEmpty(runtimeId))
							{
								runtimeId = text;
							}
							return value;
						}
					}
				}
				LoadedMod val = ModSourceHelper.FindModByDelegate((Delegate)DirectoryMaster.GetFactory<GameItem>(item.identifier));
				if (val != null && val.Manifest != null)
				{
					runtimeId = val.Manifest.ID ?? string.Empty;
					value = LocateCatalog(val);
					if (value != null)
					{
						RegisterRuntimeCatalogAliases(val, value);
						return value;
					}
					if (!string.IsNullOrWhiteSpace(runtimeId))
					{
						if (RuntimeCatalogs.TryGetValue(runtimeId, out value))
						{
							return value;
						}
						if (Catalogs.TryGetValue(runtimeId, out value))
						{
							return value;
						}
					}
					string name = val.Manifest.Name;
					if (!string.IsNullOrWhiteSpace(name))
					{
						if (RuntimeCatalogs.TryGetValue(name, out value))
						{
							return value;
						}
						if (Catalogs.TryGetValue(name, out value))
						{
							return value;
						}
					}
					if (val.WorkshopId != 0L)
					{
						string key = val.WorkshopId.ToString();
						if (RuntimeCatalogs.TryGetValue(key, out value))
						{
							return value;
						}
						if (Catalogs.TryGetValue(key, out value))
						{
							return value;
						}
					}
					if (val.ShadowedWorkshopId != 0L)
					{
						string key2 = val.ShadowedWorkshopId.ToString();
						if (RuntimeCatalogs.TryGetValue(key2, out value))
						{
							return value;
						}
						if (Catalogs.TryGetValue(key2, out value))
						{
							return value;
						}
					}
				}
			}
			catch (Exception ex)
			{
				string item2 = "owner-lookup:" + item.identifier;
				if (FinalTooltipMissLogged.Add(item2))
				{
					Debug.LogWarning((object)("[KoreanPack] Item owner lookup failed for " + item.identifier + ": " + ex.Message));
				}
			}
			return null;
		}

		private static void PatchSharedDisplayBridges()
		{
            foreach(string getter in new[]{"GetLocalizedDisplayName","GetLocalizedDescription"})
                PatchDynamic(typeof(StartingPerk).GetMethod(getter),null,"NeonStartingPerkTextPostfix","neon-starting-perk-text");
            PatchDynamic(typeof(DelegateAfterhourLocation).GetMethod("GetTitle"),null,"CardTransitTitlePostfix","card-transit-title");
            PatchDynamic(typeof(NewsUIElement).GetMethod("PopulateUI"), null, "KoreanNewsPostfix", "known-newspaper-display");
			try
			{
				MethodInfo method = typeof(GameItem).GetMethod("GetDisplayName", BindingFlags.Instance | BindingFlags.Public, null, new Type[1] { typeof(bool) }, null);
				if (method != null && PatchDynamic(method, null, "SafeGameItemDisplayNamePostfix", "shared-gameitem-display-name"))
				{
					Debug.Log((object)"[KoreanPack] GameItem display-name bridge enabled.");
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[KoreanPack] GameItem display-name bridge failed: " + ex.Message));
			}
			try
			{
				MethodInfo method2 = typeof(TypeHelper).GetMethod("GetTypeDisplayName", BindingFlags.Static | BindingFlags.Public, null, new Type[1] { typeof(string) }, null);
				if (method2 != null && PatchDynamic(method2, null, "SafeTypeDisplayNamePostfix", "shared-type-display-name"))
				{
					Debug.Log((object)"[KoreanPack] Type display-name bridge enabled.");
				}
			}
			catch (Exception ex2)
			{
				Debug.LogWarning((object)("[KoreanPack] Type display-name bridge failed: " + ex2.Message));
			}
			try
			{
				MethodInfo method3 = typeof(ClienteleUIManager).GetMethod("RefreshUI", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
				if (method3 != null && PatchDynamic(method3, null, "SafeClienteleRefreshPostfix", "shared-clientele-display"))
				{
					Debug.Log((object)"[KoreanPack] Clientele display bridge enabled.");
				}
			}
			catch (Exception ex3)
			{
				Debug.LogWarning((object)("[KoreanPack] Clientele display bridge failed: " + ex3.Message));
			}
			try
			{
				MethodInfo method4 = typeof(RecorderUIManager).GetMethod("RefreshUI", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
				if (method4 != null && PatchDynamic(method4, null, "SafeRecorderRefreshPostfix", "shared-recorder-display"))
				{
					Debug.Log((object)"[KoreanPack] Recorder display bridge enabled.");
				}
			}
			catch (Exception ex4)
			{
				Debug.LogWarning((object)("[KoreanPack] Recorder display bridge failed: " + ex4.Message));
			}
			try
			{
				MethodInfo method5 = typeof(DialogUIManager).GetMethod("DisplayClientText", BindingFlags.Instance | BindingFlags.Public, null, new Type[1] { typeof(Dialogue) }, null);
				if (method5 != null && PatchDynamic(method5, "SafeClientDialogueDisplayPrefix", null, "shared-client-dialogue-display"))
				{
					Debug.Log((object)"[KoreanPack] Client dialogue display bridge enabled.");
				}
			}
			catch (Exception ex5)
			{
				Debug.LogWarning((object)("[KoreanPack] Client dialogue display bridge failed: " + ex5.Message));
			}
			try
			{
				MethodInfo method6 = typeof(CustomUIManager).GetMethod("Window", BindingFlags.Static | BindingFlags.Public, null, new Type[3]
				{
					typeof(string),
					typeof(string),
					typeof(string)
				}, null);
				if (method6 != null)
				{
					PatchDynamic(method6, "SafeCustomUiWindowArgsPrefix", null, "tentacle-night-window-title");
				}
				Type type = typeof(CustomUIManager).Assembly.GetType("CustomUIBuilder", throwOnError: false);
				if (!(type != null))
				{
					return;
				}
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
				foreach (MethodInfo methodInfo in methods)
				{
					if (string.Equals(methodInfo.Name, "AddButton", StringComparison.Ordinal))
					{
						ParameterInfo[] parameters = methodInfo.GetParameters();
						if (parameters.Length != 0 && !(parameters[0].ParameterType != typeof(string)))
						{
							PatchDynamic(methodInfo, "SafeCustomUiButtonArgsPrefix", null, "tentacle-night-window-button");
						}
					}
				}
			}
			catch (Exception ex6)
			{
				Debug.LogWarning((object)("[KoreanPack] Custom UI display bridge failed: " + ex6.Message));
			}
		}

		private static void SafeClienteleRefreshPostfix(ClienteleUIManager __instance)
		{
			if (!_enabled || !_catalogsReady || !IsKorean() || (Object)(object)__instance == (Object)null)
			{
				return;
			}
			string text = TranslateKnownCatalogText("game_card_pk", "游戏卡");
			if (string.IsNullOrEmpty(text) || string.Equals(text, "游戏卡", StringComparison.Ordinal))
			{
				return;
			}
			TextMeshProUGUI[] array = (TextMeshProUGUI[])(object)new TextMeshProUGUI[2] { __instance.merchandiseCustomerTMP, __instance.normalCustomerTMP };
			foreach (TextMeshProUGUI val in array)
			{
				if (!((Object)(object)val == (Object)null))
				{
					string text2 = ((TMP_Text)val).text;
					if (!string.IsNullOrEmpty(text2) && text2.IndexOf("游戏卡", StringComparison.Ordinal) >= 0)
					{
						((TMP_Text)val).text = text2.Replace("游戏卡", text);
					}
				}
			}
		}

		private static void SafeClientDialogueDisplayPrefix(Dialogue dialogue)
		{
			if (!_enabled || !_catalogsReady || !IsKorean() || dialogue == null)
			{
				return;
			}
			StoreClient val = null;
			try
			{
				if (PlayerStore.Instance != null && PlayerStore.Instance.currentClientInstance != null)
				{
					val = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
				}
			}
			catch
			{
			}
			string text = null;
			if (val != null)
			{
				string a = val.sourceModId ?? string.Empty;
				string text2 = val.identifier ?? string.Empty;
				if (string.Equals(a, "game_card_pk", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("card_", StringComparison.OrdinalIgnoreCase))
				{
					text = "game_card_pk";
				}
				else if (text2.StartsWith("corp_", StringComparison.OrdinalIgnoreCase))
				{
					text = "CorpPresence";
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (val != null && !string.IsNullOrEmpty(val.displayName))
			{
				string text3 = TranslateKnownCatalogText(text, val.displayName);
				if (!string.IsNullOrEmpty(text3))
				{
					val.displayName = text3;
				}
			}
			if (!string.IsNullOrEmpty(dialogue.plainText))
			{
				string text4 = TranslateKnownCatalogText(text, dialogue.plainText);
				if (!string.IsNullOrEmpty(text4))
				{
					dialogue.plainText = text4;
				}
			}
			if (!string.IsNullOrEmpty(dialogue.title) && !string.Equals(dialogue.title, "PLAYER", StringComparison.Ordinal) && !string.Equals(dialogue.title, "SEC", StringComparison.Ordinal))
			{
				string text5 = TranslateKnownCatalogText(text, dialogue.title);
				if (!string.IsNullOrEmpty(text5))
				{
					dialogue.title = text5;
				}
			}
		}

		private static void SafeRecorderRefreshPostfix(RecorderUIManager __instance)
		{
			if (!_enabled || !_catalogsReady || !IsKorean() || (Object)(object)__instance == (Object)null || (Object)(object)__instance.ScrollViewContent == (Object)null)
			{
				return;
			}
			try
			{
				TextMeshProUGUI[] componentsInChildren = __instance.ScrollViewContent.GetComponentsInChildren<TextMeshProUGUI>(true);
				foreach (TextMeshProUGUI val in componentsInChildren)
				{
					if ((Object)(object)val == (Object)null)
					{
						continue;
					}
					string text = ((TMP_Text)val).text;
					if (!string.IsNullOrEmpty(text) && !string.Equals(text, "---", StringComparison.Ordinal))
					{
						string text2 = TranslateRecorderLine(text);
						if (!string.Equals(text, text2, StringComparison.Ordinal))
						{
							((TMP_Text)val).text = text2;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[KoreanPack] Recorder display translation skipped: " + ex.Message));
			}
		}

		private static string TranslateRecorderLine(string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				return source;
			}
			int i;
			for (i = 0; i < source.Length && char.IsWhiteSpace(source[i]); i++)
			{
			}
			string text = source.Substring(0, i);
			string text2 = source.Substring(i);
			string text3 = TranslateAcrossKnownDialogCatalogs(text2);
			if (!string.Equals(text2, text3, StringComparison.Ordinal))
			{
				return text + text3;
			}
			int num = text2.IndexOf(": ", StringComparison.Ordinal);
			if (num <= 0)
			{
				return source;
			}
			string text4 = text2.Substring(0, num);
			string text5 = text2.Substring(num + 2);
			string text6 = TranslateAcrossKnownDialogCatalogs(text4);
			string text7 = TranslateAcrossKnownDialogCatalogs(text5);
			if (string.Equals(text4, text6, StringComparison.Ordinal) && string.Equals(text5, text7, StringComparison.Ordinal))
			{
				return source;
			}
			return text + text6 + ": " + text7;
		}

		private static string TranslateAcrossKnownDialogCatalogs(string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				return source;
			}
			string[] array = new string[4] { "CorpPresence", "game_card_pk", "brewing_expansion", "lewd_meme" };
			for (int i = 0; i < array.Length; i++)
			{
				string text = TranslateKnownCatalogText(array[i], source);
				if (!string.Equals(source, text, StringComparison.Ordinal))
				{
					return text;
				}
			}
			return source;
		}

		private static void SafeCustomUiWindowArgsPrefix(object[] __args)
		{
			if (!_enabled || !_catalogsReady || !IsKorean() || __args == null || __args.Length < 2)
			{
				return;
			}
			string a = __args[0] as string;
			string text = __args[1] as string;
			if (string.Equals(a, "lewd_meme.nightreport", StringComparison.Ordinal) && !string.IsNullOrEmpty(text))
			{
				string text2 = TranslateKnownCatalogText("lewd_meme", text);
				if (!string.IsNullOrEmpty(text2))
				{
					__args[1] = text2;
				}
			}
		}

		private static void SafeCustomUiButtonArgsPrefix(object[] __args)
		{
			if (!_enabled || !_catalogsReady || !IsKorean() || __args == null || __args.Length == 0)
			{
				return;
			}
			string text = __args[0] as string;
			if (string.Equals(text, "知道了", StringComparison.Ordinal))
			{
				string text2 = TranslateKnownCatalogText("lewd_meme", text);
				if (!string.IsNullOrEmpty(text2))
				{
					__args[0] = text2;
				}
			}
		}

		private static void SafeGameItemDisplayNamePostfix(GameItem __instance, bool preferCustomName, ref string __result)
		{
			if (!_enabled || !_catalogsReady || !IsKorean() || __instance == null || string.IsNullOrEmpty(__result))
			{
				return;
			}
			try
			{
				if (preferCustomName && GeneralHelper.HasCustomName(__instance))
				{
					return;
				}
			}
			catch
			{
			}
			if (ContainsCjkText(__result) || (LocateCatalogForItem(__instance, out var neonId) is Catalog nc && (nc.Aliases.Contains("water_seller_english_collection") || nc.Aliases.Contains("CorpPresence"))))
			{
				Catalog catalog = LocateCatalogForItem(__instance, out var _);
				if (catalog != null && TryTranslateTooltipSlice(catalog, __result, out var translated) && !string.Equals(__result, translated, StringComparison.Ordinal))
				{
					__result = translated;
				}
			}
		}

		private static void SafeTypeDisplayNamePostfix(string typeConst, ref string __result)
		{
			if (_enabled && _catalogsReady && IsKorean() && !string.IsNullOrEmpty(typeConst) && string.Equals(typeConst, "游戏卡", StringComparison.Ordinal))
			{
				string text = TranslateKnownCatalogText("game_card_pk", typeConst);
				if (!string.IsNullOrEmpty(text))
				{
					__result = text;
				}
			}
		}

		private static bool ContainsCjkText(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			foreach (char c in value)
			{
				if (c >= '㐀' && c <= '鿿')
				{
					return true;
				}
			}
			return false;
		}

		private static void PatchFinalTooltipBridge()
		{
			try
			{
				MethodInfo method = typeof(ModSourceHelper).GetMethod("CreateTooltip", BindingFlags.Static | BindingFlags.Public, null, new Type[2]
				{
					typeof(RichTextBuilder),
					typeof(GameItem)
				}, null);
				if (method == null)
				{
					Debug.LogWarning((object)"[KoreanPack] Final tooltip bridge target not found.");
				}
				else if (PatchDynamic(method, null, "SafeFinalItemTooltipPostfix", "final-item-tooltip"))
				{
					Debug.Log((object)"[KoreanPack] Final item tooltip bridge enabled.");
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[KoreanPack] Final tooltip bridge failed: " + ex.Message));
			}
		}

		private static void SafeFinalItemTooltipPostfix(RichTextBuilder builder, GameItem item)
		{
			if (!_enabled || !IsKorean() || builder == null || item == null)
			{
				return;
			}
			Catalog catalog = LocateCatalogForItem(item, out var runtimeId);
			if (catalog == null)
			{
				if (!string.IsNullOrEmpty(runtimeId) && FinalTooltipMissLogged.Add(runtimeId))
				{
					Debug.LogWarning((object)("[KoreanPack] Final tooltip catalog miss: " + runtimeId));
				}
				return;
			}
			int num = TranslateFinalTooltipSlices(catalog, builder);
			if (num > 0)
			{
				string text = (string.IsNullOrEmpty(runtimeId) ? catalog.PrimaryId : runtimeId);
				if (FinalTooltipHitLogged.Add(text))
				{
					Debug.Log((object)("[KoreanPack] Final tooltip translation active: " + text + " (" + num + " slice(s) on first hit)."));
				}
			}
		}

		private static int TranslateFinalTooltipSlices(Catalog catalog, RichTextBuilder builder)
		{
			if (catalog == null || builder == null)
			{
				return 0;
			}
			try
			{
				if (_richTextSlicesField == null)
				{
					_richTextSlicesField = typeof(RichTextBuilder).GetField("slices", BindingFlags.Instance | BindingFlags.NonPublic);
				}
				if (_richTextSlicesField == null)
				{
					return 0;
				}
				if (!(_richTextSlicesField.GetValue(builder) is IEnumerable enumerable))
				{
					return 0;
				}
				int num = 0;
				foreach (object? item in enumerable)
				{
					if (item == null)
					{
						continue;
					}
					Type type = item.GetType();
					if (!SliceTextFields.TryGetValue(type, out var value))
					{
						value = type.GetField("text", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						SliceTextFields[type] = value;
					}
					if (value == null || value.FieldType != typeof(string))
					{
						continue;
					}
					string text = value.GetValue(item) as string;
					if (TryTranslateTooltipSlice(catalog, text, out var translated) && !string.Equals(text, translated, StringComparison.Ordinal))
					{
						value.SetValue(item, translated);
						if (!SliceLengthFields.TryGetValue(type, out var value2))
						{
							value2 = type.GetField("length", BindingFlags.Instance | BindingFlags.NonPublic);
							SliceLengthFields[type] = value2;
						}
						if (value2 != null && value2.FieldType == typeof(int))
						{
							value2.SetValue(item, -1);
						}
						num++;
					}
				}
				return num;
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[KoreanPack] Final tooltip translation skipped: " + ex.Message));
				return 0;
			}
		}

		private static bool TryTranslateTooltipSlice(Catalog catalog, string source, out string translated)
		{
			translated = source;
			if (catalog == null || string.IsNullOrEmpty(source))
			{
				return false;
			}
            if(catalog.Aliases.Contains("faction_reputation_expansion")) {
                translated=TranslateFactionRenderedText(catalog,source);return translated!=source;
            }
            if(catalog.Aliases.Contains("game_card_pk")) {
                translated=TranslateCardRenderedText(catalog,source);return translated!=source;
            }
			if (TryTranslateSource(catalog, source, out translated))
			{
				return true;
			}
			int i = 0;
			int num;
			for (num = source.Length; i < num && char.IsWhiteSpace(source[i]); i++)
			{
			}
			while (num > i && char.IsWhiteSpace(source[num - 1]))
			{
				num--;
			}
			if (i > 0 || num < source.Length)
			{
				string source2 = source.Substring(i, num - i);
				if (TryTranslateSource(catalog, source2, out var translated2))
				{
					translated = source.Substring(0, i) + translated2 + source.Substring(num);
					return true;
				}
			}
			bool changed = false;
			string text = Regex.Replace(source, "[\\u3400-\\u9fff]+", delegate(Match match)
			{
				if (TryTranslateSource(catalog, match.Value, out var translated3) && !string.Equals(translated3, match.Value, StringComparison.Ordinal))
				{
					changed = true;
					return translated3;
				}
				return match.Value;
			});
			if (changed)
			{
				translated = text;
				return true;
			}
			if (catalog.Aliases.Contains("water_seller_english_collection"))
            {
                translated=NeonDisplayText(source);
                return translated!=source;
            }
            return false;
		}

		private static Catalog LocateCatalog(LoadedMod loaded)
		{
			if (loaded.Manifest != null && !string.IsNullOrEmpty(loaded.Manifest.ID) && Catalogs.TryGetValue(loaded.Manifest.ID, out var value))
			{
				return value;
			}
			if (loaded.WorkshopId != 0L && Catalogs.TryGetValue(loaded.WorkshopId.ToString(), out value))
			{
				return value;
			}
			if (loaded.ShadowedWorkshopId != 0L && Catalogs.TryGetValue(loaded.ShadowedWorkshopId.ToString(), out value))
			{
				return value;
			}
			return null;
		}

		private static string SafeModName(LoadedMod loaded)
		{
			if (loaded == null || loaded.Manifest == null)
			{
				return "<unknown>";
			}
			if (!string.IsNullOrEmpty(loaded.Manifest.Name))
			{
				return loaded.Manifest.Name;
			}
			return loaded.Manifest.ID;
		}

		private static void MergeNativeEntries(LoadedMod loaded, Catalog catalog)
		{
			if (loaded.Localization == null)
			{
				loaded.Localization = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
			}
			if (!SavedValues.TryGetValue(loaded, out var value))
			{
				value = new List<SavedValue>();
				SavedValues[loaded] = value;
			}
			MergeLocale(loaded, catalog, value, "ko");
			MergeLocale(loaded, catalog, value, "ko-KR");
		}

		private static void MergeLocale(LoadedMod loaded, Catalog catalog, List<SavedValue> saved, string locale)
		{
			if (!loaded.Localization.TryGetValue(locale, out var value) || value == null)
			{
				value = new Dictionary<string, string>(StringComparer.Ordinal);
				loaded.Localization[locale] = value;
			}
			foreach (KeyValuePair<string, string> item in catalog.ByKey)
			{
				bool flag = false;
				for (int i = 0; i < saved.Count; i++)
				{
					if (string.Equals(saved[i].Locale, locale, StringComparison.OrdinalIgnoreCase) && string.Equals(saved[i].Key, item.Key, StringComparison.Ordinal))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					bool existed = value.TryGetValue(item.Key, out var value2);
					saved.Add(new SavedValue
					{
						Locale = locale,
						Key = item.Key,
						Existed = existed,
						Value = value2
					});
				}
				value[item.Key] = item.Value;
			}
		}

		private static void RebuildDerivedSourceMap(LoadedMod loaded, Catalog catalog)
		{
			catalog.DerivedBySource.Clear();
			catalog.AmbiguousDerivedSources.Clear();
			catalog.DerivedTemplates.Clear();
			if (loaded.Localization == null)
			{
				return;
			}
			foreach (KeyValuePair<string, string> item in catalog.ByKey)
			{
				foreach (KeyValuePair<string, Dictionary<string, string>> item2 in loaded.Localization)
				{
					if (!IsKoreanCode(item2.Key) && item2.Value != null && item2.Value.TryGetValue(item.Key, out var value) && !string.IsNullOrEmpty(value) && !string.Equals(value, item.Value, StringComparison.Ordinal))
					{
						AddDerivedSource(catalog, value, item.Value);
					}
				}
			}
		}

		private static void AddExplicitSource(Catalog catalog, string source, string korean)
		{
			if (catalog != null && !string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(korean) && !catalog.AmbiguousExplicitSources.Contains(source))
			{
				if (!catalog.ExplicitBySource.TryGetValue(source, out var value))
				{
					catalog.ExplicitBySource[source] = korean;
					AddTemplateRule(catalog.ExplicitTemplates, source, korean);
				}
				else if (!string.Equals(value, korean, StringComparison.Ordinal))
				{
					catalog.ExplicitBySource.Remove(source);
					catalog.AmbiguousExplicitSources.Add(source);
				}
			}
		}

		private static void AddDerivedSource(Catalog catalog, string source, string korean)
		{
			if (!catalog.ExplicitBySource.ContainsKey(source) && !catalog.AmbiguousDerivedSources.Contains(source))
			{
				if (!catalog.DerivedBySource.TryGetValue(source, out var value))
				{
					catalog.DerivedBySource[source] = korean;
					AddTemplateRule(catalog.DerivedTemplates, source, korean);
				}
				else if (!string.Equals(value, korean, StringComparison.Ordinal))
				{
					catalog.DerivedBySource.Remove(source);
					catalog.AmbiguousDerivedSources.Add(source);
				}
			}
		}

		private static bool TryTranslateSource(Catalog catalog, string source, out string translated)
		{
			translated = null;
			if (catalog == null || string.IsNullOrEmpty(source))
			{
				return false;
			}
			if (catalog.ExplicitBySource.TryGetValue(source, out translated))
			{
				return true;
			}
			if (TryTranslateTemplate(catalog.ExplicitTemplates, source, out translated))
			{
				return true;
			}
			if (catalog.DerivedBySource.TryGetValue(source, out translated))
			{
				return true;
			}
			return TryTranslateTemplate(catalog.DerivedTemplates, source, out translated);
		}

		private static void AddTemplateRule(List<TemplateRule> rules, string source, string korean)
		{
			if (rules == null || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(korean))
			{
				return;
			}
			MatchCollection matchCollection = Regex.Matches(source, "\\{\\{[^{}]+\\}\\}|\\{(?:\\d+|[A-Za-z_][A-Za-z0-9_]*)\\}");
			if (matchCollection.Count == 0)
			{
				return;
			}
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			int num = 0;
			foreach (Match item in matchCollection)
			{
				list.Add(source.Substring(num, item.Index - num));
				list2.Add(item.Value);
				num = item.Index + item.Length;
			}
			list.Add(source.Substring(num));
			for (int i = 1; i < list.Count - 1; i++)
			{
				if (list[i].Length == 0)
				{
					return;
				}
			}
			rules.Add(new TemplateRule
			{
				Source = source,
				Korean = korean,
				Literals = list.ToArray(),
				Tokens = list2.ToArray()
			});
		}

		private static bool TryTranslateTemplate(List<TemplateRule> rules, string source, out string translated)
		{
			translated = null;
			if (rules == null || string.IsNullOrEmpty(source))
			{
				return false;
			}
			foreach (TemplateRule rule in rules)
			{
				if (rule == null || rule.Literals == null || rule.Tokens == null || rule.Literals.Length != rule.Tokens.Length + 1)
				{
					continue;
				}
				string text = rule.Literals[0] ?? string.Empty;
				if (!source.StartsWith(text, StringComparison.Ordinal))
				{
					continue;
				}
				int num = text.Length;
				Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
				bool flag = true;
				for (int i = 0; i < rule.Tokens.Length; i++)
				{
					string text2 = rule.Literals[i + 1] ?? string.Empty;
					int num2;
					if (text2.Length == 0)
					{
						if (i != rule.Tokens.Length - 1)
						{
							flag = false;
							break;
						}
						num2 = source.Length;
					}
					else
					{
						num2 = source.IndexOf(text2, num, StringComparison.Ordinal);
						if (num2 < 0)
						{
							flag = false;
							break;
						}
					}
					string text3 = source.Substring(num, num2 - num);
					if (dictionary.TryGetValue(rule.Tokens[i], out var value) && !string.Equals(value, text3, StringComparison.Ordinal))
					{
						flag = false;
						break;
					}
					dictionary[rule.Tokens[i]] = text3;
					num = num2 + text2.Length;
				}
				if (!flag || num != source.Length)
				{
					continue;
				}
				string text4 = rule.Korean;
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					text4 = text4.Replace(item.Key, item.Value);
				}
				translated = text4;
				return !string.Equals(source, translated, StringComparison.Ordinal);
			}
			return false;
		}

		private static void PatchTargetAssembly(LoadedMod loaded, Catalog catalog)
		{
			if (_dynamicHarmony == null || loaded == null || loaded.Assembly == null || catalog == null)
			{
				return;
			}
			Assembly assembly = loaded.Instance != null ? loaded.Instance.GetType().Assembly : loaded.Assembly;
			if (AssemblyCatalogs.TryGetValue(assembly, out var value))
			{
				if (value != catalog)
				{
					Debug.LogWarning((object)("[KoreanPack] Assembly already mapped to another catalog: " + assembly.GetName().Name));
				}
			}
			else
			{
				AssemblyCatalogs[assembly] = catalog;
				PatchDisplayMethodsInAssembly(assembly);
                // Shop titles are presentation returns. Do not modify location IDs or saved data.
                foreach (Type t in assembly.GetTypes())
                    if (typeof(AfterhourLocation).IsAssignableFrom(t))
                        PatchKnownType(assembly,t.FullName,new KnownPatch("GetTitle",KnownPatchKind.StringResult),new KnownPatch("GetPanelTitle",KnownPatchKind.StringResult));

			}
		}

		private static void PatchDisplayMethodsInAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				return;
			}
			string text = assembly.GetName().Name ?? string.Empty;
			int num = 0;
			try
			{
				if (text.Equals("WaterSellerCollection", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Type t in assembly.GetTypes())
                    foreach (MethodInfo m in t.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                    {
                        if (m.IsAbstract || m.ContainsGenericParameters || m.GetMethodBody() == null) continue;
                        // Translate only presentation calls; never replace ldstr identifiers or gameplay comparisons.
                        if (HasNeonDisplayCall(m) && PatchDynamicTranspiler(m, "NeonDisplayTranspiler", "neon-display-calls")) num++;
                        if (m.ReturnType == typeof(GameItem))
                            num += PatchKnownType(assembly, t.FullName, new KnownPatch(m.Name, KnownPatchKind.GameItemResult), new KnownPatch(m.Name, KnownPatchKind.GameItemWindowResult));
                        if (m.ReturnType == typeof(StoreClient))
                            num += PatchKnownType(assembly, t.FullName, new KnownPatch(m.Name, KnownPatchKind.StoreClientResult));
                    }
                }
                else if (text.Equals("GameCardPK", StringComparison.OrdinalIgnoreCase))
				{
                    // Paper books resolve visible strings separately from the battle UI.
                    if(PatchDynamic(typeof(PaperUI).GetMethod("ResolveText"),null,"CardPaperTextPostfix","card-paper-visible-text"))num++;
                    Type battlePanel=assembly.GetType("GameCardPK.UI.BattlePanel",false);
                    if(battlePanel!=null) {
                        MethodInfo addLog=battlePanel.GetMethod("AddLogRow",BindingFlags.Static|BindingFlags.NonPublic);
                        if(addLog!=null && PatchDynamic(addLog,"CardLogLinePrefix","CardLogRowPostfix","card-final-log-row"))num++;
                    }
                    Type uiKit=assembly.GetType("GameCardPK.UI.UiKit",false);
                    if(uiKit!=null) {
                        MethodInfo textOutput=uiKit.GetMethod("Text",BindingFlags.Static|BindingFlags.NonPublic);
                        if(textOutput!=null && PatchDynamic(textOutput,null,"CardTextOutputPostfix","card-final-text-output")) num++;
                    }
					num += PatchKnownType(assembly, "GameCardPK.GameCardFactory", new KnownPatch("CreateCard", KnownPatchKind.GameItemResult), new KnownPatch("CreateCard", KnownPatchKind.GameItemWindowResult), new KnownPatch("CreateSkill", KnownPatchKind.GameItemResult), new KnownPatch("ApplySkillData", KnownPatchKind.ObjectArgs), new KnownPatch("AttachChessBoardWindow", KnownPatchKind.GameItemWindowArgs));
					num += PatchKnownType(assembly, "GameCardPK.SkillText", new KnownPatch("ForItem", KnownPatchKind.StringResult), new KnownPatch("ForBattle", KnownPatchKind.StringResult), new KnownPatch("PointLabel", KnownPatchKind.StringResult));
					num += PatchKnownType(assembly, "GameCardPK.CardNPC", new KnownPatch("BuildClient", KnownPatchKind.StoreClientResult));
				}
				else if (text.Equals("MidnightPawn", StringComparison.OrdinalIgnoreCase))
				{
					num += PatchKnownType(assembly, "MidnightPawn.Game.TentacleBody", new KnownPatch("Create", KnownPatchKind.GameItemResult), new KnownPatch("Create", KnownPatchKind.GameItemWindowResult), new KnownPatch("BuildWindow", KnownPatchKind.PixelWindowResult), new KnownPatch("DelayReport", KnownPatchKind.TentacleNightReportArgs));
				}
				else if (text.Equals("ItemStorage", StringComparison.OrdinalIgnoreCase))
				{
                    foreach(Type st in assembly.GetTypes())
                    foreach(MethodInfo sm in st.GetMethods(BindingFlags.DeclaredOnly|BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
                        if(!sm.IsAbstract && !sm.ContainsGenericParameters && HasStorageDisplayCall(sm))
                            if(PatchDynamicTranspiler(sm,"StorageDisplayTranspiler","storage-display-calls")) num++;

					num += PatchKnownType(assembly, "ItemStorage.BoxItem", new KnownPatch("Create", KnownPatchKind.GameItemResult), new KnownPatch("Create", KnownPatchKind.GameItemWindowResult), new KnownPatch("NotifyUser", KnownPatchKind.VisibleStringArgs));
					num += PatchKnownType(assembly, "ItemStorage.BoxWindow", new KnownPatch("UpdateTooltip", KnownPatchKind.ObjectArgs), new KnownPatch("UpdateTooltip", KnownPatchKind.GameItemWindowArgs), new KnownPatch("EnsureWindow", KnownPatchKind.StaticDisplayState), new KnownPatch("EnsureWindow", KnownPatchKind.GameItemWindowArgs), new KnownPatch("RefreshButtons", KnownPatchKind.StaticDisplayState), new KnownPatch("RefreshButtons", KnownPatchKind.GameItemWindowArgs));
					num += PatchKnownType(assembly, "ItemStorage.BoxLabel", new KnownPatch("Apply", KnownPatchKind.StorageBoxLabelState), new KnownPatch("Tick", KnownPatchKind.StorageBoxLabelState), new KnownPatch("OnItemRenamed", KnownPatchKind.StorageBoxLabelState));
				}
				else if (text.Equals("XIAOWONightAuction", StringComparison.OrdinalIgnoreCase))
				{
                    Type logType=assembly.GetType("XIAOWONightAuction.AuctionText",false);
                    if(logType!=null) {
                        MethodInfo logMethod=logType.GetMethod("DisplayLog",BindingFlags.Static|BindingFlags.NonPublic);
                        if(logMethod!=null && PatchDynamic(logMethod,null,"AuctionFinalLogPostfix","auction-final-log")) num++;
                    }

                    foreach(Type at in assembly.GetTypes())
                    foreach(MethodInfo am in at.GetMethods(BindingFlags.DeclaredOnly|BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
                        if(!am.IsAbstract && !am.ContainsGenericParameters && HasAuctionDisplayCall(am))
                            if(PatchDynamicTranspiler(am,"AuctionDisplayTranspiler","auction-display-calls")) num++;

					num += PatchKnownType(assembly, "XIAOWONightAuction.AuctionText", new KnownPatch("Select", KnownPatchKind.BilingualResult), new KnownPatch("Get", KnownPatchKind.StringResult));
					num += PatchKnownType(assembly, "XIAOWONightAuction.AuctionExclusiveFactory", new KnownPatch("ApplyLocalizedText", KnownPatchKind.ObjectArgs));
					num += PatchKnownType(assembly, "XIAOWONightAuction.AuctionPhone", new KnownPatch("CreateCard", KnownPatchKind.GameItemResult), new KnownPatch("LocalizeCard", KnownPatchKind.ObjectArgs), new KnownPatch("BuildDialogue", KnownPatchKind.DialogueResult), new KnownPatch("BuildBookingDialogue", KnownPatchKind.DialogueResult), new KnownPatch("Ending", KnownPatchKind.DialogueResult));
				}
				else if (text.Equals("BrewingExpansion", StringComparison.OrdinalIgnoreCase))
				{
					num += PatchKnownType(assembly, "BrewingExpansion.Formal.Figurines", new KnownPatch("Create", KnownPatchKind.GameItemResult));
				}
                else if (text.Equals("FactionReputationExpansion",StringComparison.OrdinalIgnoreCase))
                {
                    foreach(Type ft in assembly.GetTypes())
                    foreach(MethodInfo fm in ft.GetMethods(BindingFlags.DeclaredOnly|BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic)) {
                        if(fm.IsAbstract || fm.ContainsGenericParameters || fm.GetMethodBody()==null)continue;
                        if(HasFactionDisplayCall(fm) && PatchDynamicTranspiler(fm,"FactionDisplayTranspiler","faction-visible-output"))num++;
                        var fp=fm.GetParameters();
                        if(fm.ReturnType==typeof(string)) {
                            if(fp.Length==2 && fp.All(x=>x.ParameterType==typeof(string)) && fp[0].Name=="zh" && fp[1].Name=="en") {
                                if(PatchDynamic(fm,null,"FactionBilingualPostfix","faction-bilingual-text"))num++;
                            }
                            else if(ft.FullName=="FactionReputationExpansion.ModText" && fm.Name=="T" && fp.Length==1) {
                                if(PatchDynamic(fm,null,"FactionSingleTextPostfix","faction-locale-text"))num++;
                            }
                            else if((ft.FullName=="FactionReputationExpansion.ModText" && fm.Name=="F") || new[]{"get_Name","get_Description","get_Flavor"}.Contains(fm.Name))
                                if(PatchDynamic(fm,null,"FactionStringPostfix","faction-display-return"))num++;
                        }
                    }
                }
				else if (text.Equals("CorpPresence", StringComparison.OrdinalIgnoreCase))
				{
                    foreach(Type ct in assembly.GetTypes())
                    foreach(MethodInfo cm in ct.GetMethods(BindingFlags.DeclaredOnly|BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
                        if(!cm.IsAbstract && !cm.ContainsGenericParameters && HasCorpStoryDisplayCall(cm))
                            if(PatchDynamicTranspiler(cm,"CorpStoryDisplayTranspiler","corp-story-output"))num++;

                    // Restrict replacements to GUI.Label(Rect,string,GUIStyle) calls inside this assembly.
                    foreach(Type ct in assembly.GetTypes())
                    foreach(MethodInfo cm in ct.GetMethods(BindingFlags.DeclaredOnly|BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
                        if(!cm.IsAbstract && !cm.ContainsGenericParameters && HasCorpLabelCall(cm))
                            if(PatchDynamicTranspiler(cm,"SafeCorpGuiLabelTranspiler","safe-corp-gui-label")) num++;

					Type lang=assembly.GetType("CorpPresence.L",false);
                    if(lang!=null)foreach(var lm in lang.GetMethods(BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic))
                        if(lm.Name=="T" && lm.GetParameters().Length==2)
                            if(PatchDynamic(lm,null,lm.ReturnType==typeof(string)?"CorpStoryBilingualPostfix":"CorpStoryBilingualArrayPostfix","corp-story-bilingual"))num++;
					num += PatchKnownType(assembly, "CorpPresence.UI.MenuLabels", new KnownPatch("Get", KnownPatchKind.StringArrayResult));
					num += PatchKnownType(assembly, "CorpPresence.UI.Ui", new KnownPatch("Button", KnownPatchKind.VisibleStringArgs), new KnownPatch("Label", KnownPatchKind.VisibleStringArgs));
					num += PatchKnownType(assembly, "CorpPresence.UI.CompanyWindow", new KnownPatch("DrawPastel", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawPastelOrder", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("PastelBribeCell", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawPastelDealer", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawPastelTraffic", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawPastelFooter", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawDavre", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawDavreOrder", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawDavreLeviathan", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawDavreCanning", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawSolana", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawSolanaVip", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawSolanaLab", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawSolanaGift", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawSpacerMutual", KnownPatchKind.GuiLabelTranspiler));
					num += PatchKnownType(assembly, "CorpPresence.CompanyVisitors", new KnownPatch("Courier", KnownPatchKind.StoreClientResult));
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[KoreanPack] Safe target adapter failed for " + text + ": " + ex.Message));
			}
			if (num > 0)
			{
				Debug.Log((object)("[KoreanPack] Safe target adapter: " + text + " -> " + num + " patch(es)."));
			}
		}

		private static int PatchKnownType(Assembly assembly, string typeName, params KnownPatch[] specs)
		{
			if (assembly == null || specs == null || specs.Length == 0)
			{
				return 0;
			}
			Type type;
			try
			{
				type = assembly.GetType(typeName, throwOnError: false);
			}
			catch
			{
				return 0;
			}
			if (type == null)
			{
				return 0;
			}
			MethodInfo[] methods;
			try
			{
				methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
			catch
			{
				return 0;
			}
			int num = 0;
			foreach (KnownPatch knownPatch in specs)
			{
				if (knownPatch == null || string.IsNullOrEmpty(knownPatch.MethodName))
				{
					continue;
				}
				MethodInfo[] array = methods;
				foreach (MethodInfo methodInfo in array)
				{
					if (methodInfo == null || methodInfo.IsAbstract || methodInfo.ContainsGenericParameters || methodInfo.IsConstructor || !string.Equals(methodInfo.Name, knownPatch.MethodName, StringComparison.Ordinal))
					{
						continue;
					}
					try
					{
						switch (knownPatch.Kind)
						{
						case KnownPatchKind.StringResult:
							if (methodInfo.ReturnType == typeof(string) && PatchDynamic(methodInfo, null, "SafeStringResultPostfix", "safe-string-result"))
							{
								num++;
							}
							break;
						case KnownPatchKind.StringArrayResult:
							if (methodInfo.ReturnType == typeof(string[]) && PatchDynamic(methodInfo, null, "SafeStringArrayResultPostfix", "safe-string-array-result"))
							{
								num++;
							}
							break;
						case KnownPatchKind.BilingualResult:
							if (methodInfo.ReturnType == typeof(string))
							{
								if (PatchDynamic(methodInfo, null, "SafeBilingualStringResultPostfix", "safe-bilingual-string"))
								{
									num++;
								}
							}
							else if (methodInfo.ReturnType == typeof(string[]) && PatchDynamic(methodInfo, null, "SafeBilingualStringArrayResultPostfix", "safe-bilingual-array"))
							{
								num++;
							}
							break;
						case KnownPatchKind.VisibleStringArgs:
							if (HasVisibleStringArguments(methodInfo) && PatchDynamic(methodInfo, "SafeVisibleStringArgsPrefix", null, "safe-visible-string-args"))
							{
								num++;
							}
							break;
						case KnownPatchKind.GuiLabelTranspiler:
							if (PatchDynamicTranspiler(methodInfo, "SafeCorpGuiLabelTranspiler", "safe-corp-gui-label"))
							{
								num++;
							}
							break;
						case KnownPatchKind.LocalizerResult:
							if (methodInfo.ReturnType == typeof(string))
							{
								if (PatchDynamic(methodInfo, null, "SafeStringResultPostfix", "safe-localizer-string"))
								{
									num++;
								}
							}
							else if (methodInfo.ReturnType == typeof(string[]) && PatchDynamic(methodInfo, null, "SafeStringArrayResultPostfix", "safe-localizer-array"))
							{
								num++;
							}
							break;
						case KnownPatchKind.GameItemResult:
							if (methodInfo.ReturnType == typeof(GameItem) && PatchDynamic(methodInfo, null, "SafeGameItemResultPostfix", "safe-gameitem-result"))
							{
								num++;
							}
							break;
						case KnownPatchKind.GameItemWindowResult:
							if (methodInfo.ReturnType == typeof(GameItem) && PatchDynamic(methodInfo, null, "SafeGameItemWindowResultPostfix", "safe-gameitem-window-result"))
							{
								num++;
							}
							break;
						case KnownPatchKind.PixelWindowResult:
							if (methodInfo.ReturnType == typeof(PixelWindow) && PatchDynamic(methodInfo, null, "SafeOutputPixelWindowResultPostfix", "safe-pixelwindow-result"))
							{
								num++;
							}
							break;
						case KnownPatchKind.GameItemWindowArgs:
							if (methodInfo.GetParameters().Any((ParameterInfo p) => p.ParameterType == typeof(GameItem)) && PatchDynamic(methodInfo, null, "SafeGameItemWindowArgsPostfix", "safe-gameitem-window-args"))
							{
								num++;
							}
							break;
						case KnownPatchKind.BuyerTypeListArgs:
							if (PatchDynamic(methodInfo, null, "SafeBuyerTypeListArgsPostfix", "safe-buyer-type-list"))
							{
								num++;
							}
							break;
						case KnownPatchKind.StorageBoxLabelState:
							if (PatchDynamic(methodInfo, null, "SafeStorageBoxLabelStatePostfix", "safe-storage-box-label"))
							{
								num++;
							}
							break;
						case KnownPatchKind.TentacleNightReportArgs:
							if (methodInfo.GetParameters().Any((ParameterInfo p) => p.ParameterType == typeof(string)) && PatchDynamic(methodInfo, "SafeTentacleNightReportArgsPrefix", null, "tentacle-night-report"))
							{
								num++;
							}
							break;
						case KnownPatchKind.StoreClientResult:
							if (methodInfo.ReturnType == typeof(StoreClient) && PatchDynamic(methodInfo, null, "SafeStoreClientResultPostfix", "safe-storeclient-result"))
							{
								num++;
							}
							break;
						case KnownPatchKind.DialogueResult:
							if (methodInfo.ReturnType == typeof(Dialogue) && PatchDynamic(methodInfo, null, "SafeDialogueResultPostfix", "safe-dialogue-result"))
							{
								num++;
							}
							break;
						case KnownPatchKind.ObjectArgs:
							if (HasObjectArguments(methodInfo) && PatchDynamic(methodInfo, null, "SafeObjectArgsPostfix", "safe-object-args"))
							{
								num++;
							}
							break;
						case KnownPatchKind.StaticDisplayState:
							if (PatchDynamic(methodInfo, null, "SafeTargetStaticStatePostfix", "safe-static-display-state"))
							{
								num++;
							}
							break;
						}
					}
					catch (Exception ex)
					{
						Debug.LogWarning((object)("[KoreanPack] Safe patch skipped: " + typeName + "." + methodInfo.Name + " (" + ex.Message + ")"));
					}
				}
			}
			return num;
		}

		private static bool PatchDynamic(MethodBase method, string prefixName, string postfixName, string kind)
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			if (_dynamicHarmony == null || method == null)
			{
				return false;
			}
			string item;
			try
			{
				item = method.Module.ModuleVersionId.ToString() + ":" + method.MetadataToken + ":" + kind;
			}
			catch
			{
				return false;
			}
			if (PatchedMethods.Contains(item))
			{
				return false;
			}
			HarmonyMethod val = (string.IsNullOrEmpty(prefixName) ? ((HarmonyMethod)null) : new HarmonyMethod(typeof(KoreanPack), prefixName, (Type[])null));
			HarmonyMethod val2 = (string.IsNullOrEmpty(postfixName) ? ((HarmonyMethod)null) : new HarmonyMethod(typeof(KoreanPack), postfixName, (Type[])null));
			_dynamicHarmony.Patch(method, val, val2, (HarmonyMethod)null, (HarmonyMethod)null);
			PatchedMethods.Add(item);
			return true;
		}

		private static bool PatchDynamicTranspiler(MethodBase method, string transpilerName, string kind)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Expected O, but got Unknown
			if (_dynamicHarmony == null || method == null || string.IsNullOrEmpty(transpilerName))
			{
				return false;
			}
			string item;
			try
			{
				item = method.Module.ModuleVersionId.ToString() + ":" + method.MetadataToken + ":" + kind;
			}
			catch
			{
				return false;
			}
			if (PatchedMethods.Contains(item))
			{
				return false;
			}
			HarmonyMethod val = new HarmonyMethod(typeof(KoreanPack), transpilerName, (Type[])null);
			_dynamicHarmony.Patch(method, (HarmonyMethod)null, (HarmonyMethod)null, val, (HarmonyMethod)null);
			PatchedMethods.Add(item);
			return true;
		}

		private static IEnumerable<CodeInstruction> SafeCorpGuiLabelTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo wrapper = typeof(KoreanPack).GetMethod("CorpGuiLabel", BindingFlags.Static | BindingFlags.NonPublic);
			foreach (CodeInstruction instruction in instructions)
			{
				if (wrapper != null)
				{
					MethodInfo methodInfo = instruction.operand as MethodInfo;
					if (methodInfo != null && methodInfo.DeclaringType == typeof(GUI) && string.Equals(methodInfo.Name, "Label", StringComparison.Ordinal))
					{
						ParameterInfo[] parameters = methodInfo.GetParameters();
						if (parameters.Length == 3 && parameters[0].ParameterType == typeof(Rect) && parameters[1].ParameterType == typeof(string) && parameters[2].ParameterType == typeof(GUIStyle))
						{
							instruction.operand = wrapper;
						}
					}
				}
				yield return instruction;
			}
		}

        private static Regex NeonFragments;
        private static readonly Dictionary<string,string> NeonDisplayCache = new Dictionary<string,string>(StringComparer.Ordinal);
        private static bool HasNeonDisplayCall(MethodInfo method)
        {
            byte[] bytes = method.GetMethodBody().GetILAsByteArray();
            for (int i=0;i+4<bytes.Length;i++)
                if (bytes[i] == 0x28 || bytes[i] == 0x6f || bytes[i] == 0x73)
                    try {
                        var call=method.Module.ResolveMethod(BitConverter.ToInt32(bytes,i+1));
                        if (call != null && IsNeonDisplayCall(call)) return true;
                    } catch { }
            return false;
        }
        private static string TranslateNeonPerkText(Catalog catalog,string source,string id,string value)
        {
            bool owned=string.Equals(source,"water_seller_english_collection",StringComparison.OrdinalIgnoreCase)
                || id=="neon_undertow_bottler" || id=="neon_undertow_engineer"
                || id=="water_seller_english_collection:neon_undertow_bottler" || id=="water_seller_english_collection:neon_undertow_engineer";
            if(!owned || string.IsNullOrEmpty(value))return value;
            string translated;return TryTranslateSource(catalog,value,out translated)?translated:value;
        }
        private static void NeonStartingPerkTextPostfix(StartingPerk __instance,ref string __result)
        {
            if(!_enabled || !IsKorean() || __instance==null)return;
            Catalog catalog;if(!Catalogs.TryGetValue("water_seller_english_collection",out catalog))return;
            __result=TranslateNeonPerkText(catalog,__instance.source,__instance.id,__result);
        }
        private static MachineryPanel NeonPanel(string title, PixelElement element, string id, bool inventory) { return MachineHelper.Panel(NeonDisplayText(title), element, id, inventory); }
        private static void KoreanNewsPostfix(NewsUIElement __instance)
        {
            if (!_enabled || !IsKorean() || __instance==null) return;
            Catalog c; if(!Catalogs.TryGetValue("water_seller_english_collection",out c)) return;
            string t;
            if(__instance.title!=null && TryTranslateSource(c,__instance.title.text,out t)) __instance.title.text=t;
            if(__instance.textContent!=null) {
                string original=__instance.textContent.text;
                foreach(var entry in c.ExplicitBySource)
                    if(entry.Key.StartsWith("The upper district is celebrating",StringComparison.Ordinal) && original.Contains(entry.Key)) original=original.Replace(entry.Key,entry.Value);
                __instance.textContent.text=original;
            }
        }
        private static string NeonWrapperName(MethodBase method)
        {
            string wrapper=null;
 if (method.DeclaringType == typeof(MachineHelper) && method.Name == "Panel") return "NeonPanel";
if (method.DeclaringType == typeof(global::MachineryTabs) && method.Name == "AddTab" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::PixelElement)})) wrapper="NeonWrap0";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Action),typeof(global::System.Int32),typeof(global::System.Int32)})) wrapper="NeonWrap1";
if (method.DeclaringType == typeof(global::Dialogue) && method.Name == "SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.String)})) wrapper="NeonWrap2";
if (method.DeclaringType == typeof(global::TMPro.TMP_Text) && method.Name == "set_text" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="NeonWrap3";
if (method.DeclaringType == typeof(global::CustomUIBuilder) && method.Name == "AddHeader" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.String)})) wrapper="NeonWrap4";
if (method.DeclaringType == typeof(global::CustomUIBuilder) && method.Name == "AddLabel" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.String)})) wrapper="NeonWrap5";
if (method.DeclaringType == typeof(global::CustomUIBuilder) && method.Name == "AddButton" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Action),typeof(global::System.String)})) wrapper="NeonWrap6";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "AddLine" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="NeonWrap7";
if (method.DeclaringType == typeof(global::CustomUIBuilder) && method.Name == "BeginTab" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="NeonWrap8";
if (method.DeclaringType == typeof(global::CustomUIBuilder) && method.Name == "AddWarning" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.String)})) wrapper="NeonWrap9";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == "SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="NeonWrap10";
            return wrapper;
        }
        private static bool IsNeonDisplayCall(MethodBase method) { return NeonWrapperName(method)!=null; }
        private static string NeonDisplayText(string value)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(value)) return value;
            
            Catalog catalog;
            if (!Catalogs.TryGetValue("water_seller_english_collection", out catalog)) return value;
            string exact; if (TryTranslateSource(catalog,value,out exact)) return exact;
            string cached;
            if (NeonDisplayCache.TryGetValue(value,out cached)) return cached;
            if (NeonFragments == null)
                NeonFragments = new Regex(string.Join("|", catalog.ExplicitBySource.Keys.Where(k=>k.Length>=4 && !k.Contains("{"))
                    .OrderByDescending(k=>k.Length).Select(k => (char.IsLetter(k[0]) ? "(?<![A-Za-z])" : "") + Regex.Escape(k) + (char.IsLetter(k[k.Length-1]) ? "(?![A-Za-z])" : ""))), RegexOptions.CultureInvariant);
            string result=NeonFragments.Replace(value,m=>catalog.ExplicitBySource[m.Value]);
            if (NeonDisplayCache.Count>=4096) NeonDisplayCache.Clear();
            NeonDisplayCache[value]=result;
            return result;
        }

private static global::System.Int32 NeonWrap0(global::MachineryTabs instance,global::System.String a0,global::PixelElement a1) { return instance.AddTab(NeonDisplayText(a0),a1); }
private static global::MachineryButton NeonWrap1(global::System.String a0,global::System.Action a1,global::System.Int32 a2,global::System.Int32 a3) { return new global::MachineryButton(NeonDisplayText(a0),a1,a2,a3); }
private static global::Dialogue NeonWrap2(global::Dialogue instance,global::System.String a0,global::System.String a1) { return instance.SetText(NeonDisplayText(a0),NeonDisplayText(a1)); }
private static void NeonWrap3(global::TMPro.TMP_Text instance,global::System.String a0) { instance.text = NeonDisplayText(a0); }
private static global::CustomUIBuilder NeonWrap4(global::CustomUIBuilder instance,global::System.String a0,global::System.String a1) { return instance.AddHeader(NeonDisplayText(a0),a1); }
private static global::CustomUIBuilder NeonWrap5(global::CustomUIBuilder instance,global::System.String a0,global::System.String a1) { return instance.AddLabel(NeonDisplayText(a0),a1); }
private static global::CustomUIBuilder NeonWrap6(global::CustomUIBuilder instance,global::System.String a0,global::System.Action a1,global::System.String a2) { return instance.AddButton(NeonDisplayText(a0),a1,a2); }
private static global::RichTextBuilder NeonWrap7(global::RichTextBuilder instance,global::System.String a0,global::System.Boolean a1,global::RenderHandler.ColorPalette a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.AddLine(NeonDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::CustomUIBuilder NeonWrap8(global::CustomUIBuilder instance,global::System.String a0) { return instance.BeginTab(NeonDisplayText(a0)); }
private static global::CustomUIBuilder NeonWrap9(global::CustomUIBuilder instance,global::System.String a0,global::System.String a1) { return instance.AddWarning(NeonDisplayText(a0),a1); }
private static global::MachineryButton NeonWrap10(global::MachineryButton instance,global::System.String a0) { return instance.SetText(NeonDisplayText(a0)); }
        private static IEnumerable<CodeInstruction> NeonDisplayTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach(var instruction in instructions)
            {
                var method=instruction.operand as MethodBase;
                string wrapper=method==null?null:NeonWrapperName(method);
                if(wrapper!=null) { instruction.opcode=System.Reflection.Emit.OpCodes.Call; instruction.operand=typeof(KoreanPack).GetMethod(wrapper,BindingFlags.Static|BindingFlags.NonPublic); }
                yield return instruction;
            }
        }

        private static bool HasCorpLabelCall(MethodInfo method)
        {
            var body=method.GetMethodBody(); if(body==null)return false;
            var bytes=body.GetILAsByteArray();
            for(int i=0;i+4<bytes.Length;i++)
                if(bytes[i]==0x28 || bytes[i]==0x6f)
                    try {
                        var call=method.Module.ResolveMethod(BitConverter.ToInt32(bytes,i+1)) as MethodInfo;
                        if(call==null || call.DeclaringType!=typeof(GUI) || call.Name!="Label")continue;
                        var args=call.GetParameters();
                        if(args.Length==3 && args[0].ParameterType==typeof(Rect) && args[1].ParameterType==typeof(string) && args[2].ParameterType==typeof(GUIStyle))return true;
                    }catch { }
            return false;
        }


        private static string StorageDisplayText(string value) { return TranslateKnownCatalogText("Dream_Item_Storage",value); }
        private static string StorageWrapperName(MethodBase method)
        { string wrapper=null;
if (method.DeclaringType == typeof(global::TMPro.TMP_Text) && method.Name == "set_text" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="StorageWrap0";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "AddLine" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="StorageWrap1";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "Add" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::RenderHandler.ColorPalette),typeof(global::System.String),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="StorageWrap2";
if (method.DeclaringType == typeof(global::PixelWindow) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.Boolean),typeof(global::System.String),typeof(global::WindowStyle)})) wrapper="StorageWrap3";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Action),typeof(global::System.Int32),typeof(global::System.Int32)})) wrapper="StorageWrap4";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == "SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="StorageWrap5";
if (method.DeclaringType == typeof(global::MachineryText) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Int32),typeof(global::TMPro.HorizontalAlignmentOptions),typeof(global::System.Boolean)})) wrapper="StorageWrap6";
if (method.DeclaringType == typeof(global::MachineryRadioButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::System.Action<global::System.Boolean>)})) wrapper="StorageWrap7";
            return wrapper;
        }
        private static bool HasStorageDisplayCall(MethodInfo method)
        {
            var body=method.GetMethodBody();if(body==null)return false;
            var bytes=body.GetILAsByteArray();
            for(int i=0;i+4<bytes.Length;i++)
                if(bytes[i]==0x28||bytes[i]==0x6f||bytes[i]==0x73)
                    try { var call=method.Module.ResolveMethod(BitConverter.ToInt32(bytes,i+1)); if(call!=null && StorageWrapperName(call)!=null)return true; }catch{}
            return false;
        }
private static void StorageWrap0(global::TMPro.TMP_Text instance,global::System.String a0) { instance.text=StorageDisplayText(a0); }
private static global::RichTextBuilder StorageWrap1(global::RichTextBuilder instance,global::System.String a0,global::System.Boolean a1,global::RenderHandler.ColorPalette a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.AddLine(StorageDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::RichTextBuilder StorageWrap2(global::RichTextBuilder instance,global::System.String a0,global::RenderHandler.ColorPalette a1,global::System.String a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.Add(StorageDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::PixelWindow StorageWrap3(global::System.Boolean a0,global::System.String a1,global::WindowStyle a2) { return new global::PixelWindow(a0,StorageDisplayText(a1),a2); }
private static global::MachineryButton StorageWrap4(global::System.String a0,global::System.Action a1,global::System.Int32 a2,global::System.Int32 a3) { return new global::MachineryButton(StorageDisplayText(a0),a1,a2,a3); }
private static global::MachineryButton StorageWrap5(global::MachineryButton instance,global::System.String a0) { return instance.SetText(StorageDisplayText(a0)); }
private static global::MachineryText StorageWrap6(global::System.String a0,global::System.Int32 a1,global::TMPro.HorizontalAlignmentOptions a2,global::System.Boolean a3) { return new global::MachineryText(StorageDisplayText(a0),a1,a2,a3); }
private static global::MachineryRadioButton StorageWrap7(global::System.String a0,global::System.Boolean a1,global::System.Action<global::System.Boolean> a2) { return new global::MachineryRadioButton(StorageDisplayText(a0),a1,a2); }
        private static IEnumerable<CodeInstruction> StorageDisplayTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach(var instruction in instructions) {
                var method=instruction.operand as MethodBase;
                string wrapper=method==null?null:StorageWrapperName(method);
                if(wrapper!=null){instruction.opcode=System.Reflection.Emit.OpCodes.Call;instruction.operand=typeof(KoreanPack).GetMethod(wrapper,BindingFlags.Static|BindingFlags.NonPublic);}
                yield return instruction;
            }
        }
        private static void CardTransitTitlePostfix(DelegateAfterhourLocation __instance,ref string __result)
        {
            if(!_enabled || !IsKorean() || __instance==null || __instance.source!="game_card_pk")return;
            __result=CardDisplayText(__result);
        }
        private static int CardOutputDiagnostics;
        private static void CardTextOutputPostfix(string value,TextMeshProUGUI __result)
        {
            if(!_enabled || !IsKorean() || __result==null)return;
            string translated=CardDisplayText(value);
            if(!string.IsNullOrEmpty(value) && string.IsNullOrWhiteSpace(translated)) translated=value;
            if(!string.Equals(value,translated,StringComparison.Ordinal)) {
                __result.text=translated;
                __result.overflowMode=TextOverflowModes.Overflow;
            }
            {
                // A component may receive already-translated text. Numeric components retain their original font.
                FontLoader loader=FontLoader.Instance;
                if(ContainsHangulText(translated) && loader!=null && loader.koreanFont!=null) {
                    __result.font=loader.koreanFont;
                    __result.fontSharedMaterial=loader.koreanFont.material;
                }
            }
            if(!string.IsNullOrEmpty(value) && CardOutputDiagnostics<16) {
                CardOutputDiagnostics++;
                Debug.Log("[KoreanPack] Card text output: input="+value+"; output="+__result.text+"; font="+(__result.font==null?"null":__result.font.name)+"; visible="+__result.maxVisibleCharacters);
            }
        }
        private static void CardLogLinePrefix(ref string line)
        {
            if(!_enabled || !IsKorean() || string.IsNullOrEmpty(line))return;
            string translated=CardDisplayText(line);if(!string.IsNullOrWhiteSpace(translated))line=translated;
        }
        private static void CardLogRowPostfix(MethodBase __originalMethod,bool __result)
        {
            if(!__result || !_enabled || !IsKorean())return;
            var field=__originalMethod.DeclaringType.GetField("logRows",BindingFlags.Static|BindingFlags.NonPublic);
            var rows=field==null?null:field.GetValue(null) as IList;
            if(rows==null || rows.Count==0)return;
            TMP_Text text=rows[rows.Count-1] as TMP_Text;
            FontLoader loader=FontLoader.Instance;
            if(text!=null && ContainsHangulText(text.text) && loader!=null && loader.koreanFont!=null) {
                text.font=loader.koreanFont;text.fontSharedMaterial=loader.koreanFont.material;
            }
        }
        private static void CardPaperTextPostfix(PaperUI __instance,ref string __result)
        {
            if(__instance==null || !string.Equals(__instance.ModId,"game_card_pk",StringComparison.OrdinalIgnoreCase))return;
            __result=CardDisplayText(__result);
        }
        private static Catalog CardFragmentCatalog;
        private static Regex CardCjkFragments;
        private static readonly Dictionary<string,string> CardTranslationCache=new Dictionary<string,string>(StringComparer.Ordinal);
        private static string CardDisplayText(string value)
        {
            if(!_enabled || !IsKorean() || string.IsNullOrEmpty(value))return value;
            Catalog catalog;
            if((!RuntimeCatalogs.TryGetValue("game_card_pk",out catalog) && !Catalogs.TryGetValue("game_card_pk",out catalog)) || catalog==null)return value;
            string result;if(CardTranslationCache.TryGetValue(value,out result))return result;
            result=TranslateCardRenderedText(catalog,value);
            if(CardTranslationCache.Count>=2048)CardTranslationCache.Clear();
            CardTranslationCache[value]=result;return result;
        }
        private static string TranslateCardRenderedText(Catalog catalog,string source)
        {
            if(catalog==null || string.IsNullOrEmpty(source))return source;
            var rules=catalog.ExplicitTemplates.OrderByDescending(r=>r.Literals.Sum(x=>x.Length)).ToList();
            string result;
            if(!catalog.ExplicitBySource.TryGetValue(source,out result) && !TryTranslateTemplate(source.Contains("\n")?rules.Where(r=>r.Source.Contains("\n")).ToList():rules,source,out result)) {
                string[] lines=source.Split('\n');
                for(int i=0;i<lines.Length;i++) {
                    string translated;
                    if(catalog.ExplicitBySource.TryGetValue(lines[i],out translated) || TryTranslateTemplate(rules,lines[i],out translated) || TryTranslateSource(catalog,lines[i],out translated))lines[i]=translated;
                }
                result=string.Join("\n",lines);
            }
            // A broad trailing-value rule must not swallow unrelated tooltip lines.
            string[] outputLines=result.Split('\n');
            var singleLineRules=rules.Where(r=>!r.Source.Contains("\n")).ToList();
            for(int i=0;i<outputLines.Length;i++) {
                string translated;
                if(ContainsCjkText(outputLines[i]) && (catalog.ExplicitBySource.TryGetValue(outputLines[i],out translated) || TryTranslateTemplate(singleLineRules,outputLines[i],out translated)))outputLines[i]=translated;
            }
            result=string.Join("\n",outputLines);
            // Captured names, state labels and trigger descriptions may still be Chinese.
            if(!ReferenceEquals(CardFragmentCatalog,catalog)) {
                var chunks=catalog.ExplicitBySource.Keys.Where(k=>k.Length>=2 && Regex.IsMatch(k,"^[\\u3400-\\u9fff]+$")).OrderByDescending(k=>k.Length).Select(Regex.Escape).ToArray();
                CardCjkFragments=chunks.Length==0?null:new Regex(string.Join("|",chunks),RegexOptions.CultureInvariant);
                CardFragmentCatalog=catalog;
            }
            result=Regex.Replace(result,"[\\u3400-\\u9fff]+",m=>{
                string translated;if(TryTranslateSource(catalog,m.Value,out translated))return translated;
                return CardCjkFragments==null?m.Value:CardCjkFragments.Replace(m.Value,n=>catalog.ExplicitBySource[n.Value]);
            });
            return string.IsNullOrWhiteSpace(result)?source:result;
        }
        private static string CardWrapperName(MethodBase method)
        { string wrapper=null;
if(method.DeclaringType==typeof(Dialogue) && method.Name=="SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(string),typeof(string)})) return "CardDialogue";
if (method.DeclaringType == typeof(global::TMPro.TMP_Text) && method.Name == "set_text" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="CardWrap0";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "AddLine" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="CardWrap1";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "Add" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::RenderHandler.ColorPalette),typeof(global::System.String),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="CardWrap2";
if (method.DeclaringType == typeof(global::PixelWindow) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.Boolean),typeof(global::System.String),typeof(global::WindowStyle)})) wrapper="CardWrap3";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Action),typeof(global::System.Int32),typeof(global::System.Int32)})) wrapper="CardWrap4";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == "SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="CardWrap5";
if (method.DeclaringType == typeof(global::MachineryText) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Int32),typeof(global::TMPro.HorizontalAlignmentOptions),typeof(global::System.Boolean)})) wrapper="CardWrap6";
if (method.DeclaringType == typeof(global::MachineryRadioButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::System.Action<global::System.Boolean>)})) wrapper="CardWrap7";
            return wrapper;
        }
        private static bool HasCardDisplayCall(MethodInfo method)
        {
            var body=method.GetMethodBody();if(body==null)return false;
            var bytes=body.GetILAsByteArray();
            for(int i=0;i+4<bytes.Length;i++)
                if(bytes[i]==0x28||bytes[i]==0x6f||bytes[i]==0x73)
                    try { var call=method.Module.ResolveMethod(BitConverter.ToInt32(bytes,i+1)); if(call!=null && CardWrapperName(call)!=null)return true; }catch{}
            return false;
        }
private static void CardWrap0(global::TMPro.TMP_Text instance,global::System.String a0) { instance.text=CardDisplayText(a0); }
private static global::RichTextBuilder CardWrap1(global::RichTextBuilder instance,global::System.String a0,global::System.Boolean a1,global::RenderHandler.ColorPalette a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.AddLine(CardDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::RichTextBuilder CardWrap2(global::RichTextBuilder instance,global::System.String a0,global::RenderHandler.ColorPalette a1,global::System.String a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.Add(CardDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::PixelWindow CardWrap3(global::System.Boolean a0,global::System.String a1,global::WindowStyle a2) { return new global::PixelWindow(a0,CardDisplayText(a1),a2); }
private static global::MachineryButton CardWrap4(global::System.String a0,global::System.Action a1,global::System.Int32 a2,global::System.Int32 a3) { return new global::MachineryButton(CardDisplayText(a0),a1,a2,a3); }
private static global::MachineryButton CardWrap5(global::MachineryButton instance,global::System.String a0) { return instance.SetText(CardDisplayText(a0)); }
private static global::MachineryText CardWrap6(global::System.String a0,global::System.Int32 a1,global::TMPro.HorizontalAlignmentOptions a2,global::System.Boolean a3) { return new global::MachineryText(CardDisplayText(a0),a1,a2,a3); }
private static global::MachineryRadioButton CardWrap7(global::System.String a0,global::System.Boolean a1,global::System.Action<global::System.Boolean> a2) { return new global::MachineryRadioButton(CardDisplayText(a0),a1,a2); }
        private static IEnumerable<CodeInstruction> CardDisplayTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach(var instruction in instructions) {
                var method=instruction.operand as MethodBase;
                string wrapper=method==null?null:CardWrapperName(method);
                if(wrapper!=null){instruction.opcode=System.Reflection.Emit.OpCodes.Call;instruction.operand=typeof(KoreanPack).GetMethod(wrapper,BindingFlags.Static|BindingFlags.NonPublic);}
                yield return instruction;
            }
        }

        private static Dialogue CardDialogue(Dialogue instance,string title,string text) { return instance.SetText(CardDisplayText(title),CardDisplayText(text)); }
        private static bool AuctionFinalLogLogged;
        private static void AuctionFinalLogPostfix(ref string __result)
        {
            if(!_enabled || !IsKorean() || string.IsNullOrEmpty(__result))return;
            string original=__result;
            __result=AuctionDisplayText(original);
            if(!AuctionFinalLogLogged) {
                AuctionFinalLogLogged=true;
                Debug.Log("[KoreanPack] Auction final log: " + original + " -> " + __result);
            }
        }
        private static Regex AuctionEnglishFragments;
        private static readonly Dictionary<string,string> AuctionDisplayCache=new Dictionary<string,string>(StringComparer.Ordinal);
        private static string AuctionDisplayText(string value)
        {
            if(!_enabled || !IsKorean() || string.IsNullOrEmpty(value)) return value;
            Catalog catalog; if(!Catalogs.TryGetValue("xiaowo_night_auction",out catalog))return value;
            string cached; if(AuctionDisplayCache.TryGetValue(value,out cached))return cached;
            string result=TranslateAuctionRenderedText(catalog,value);
            if(AuctionDisplayCache.Count>=2048)AuctionDisplayCache.Clear();
            AuctionDisplayCache[value]=result;return result;
        }
        private static string TranslateAuctionRenderedText(Catalog catalog,string value)
        {
            if(catalog==null || string.IsNullOrEmpty(value))return value;
            string result;
            if(!catalog.ExplicitBySource.TryGetValue(value,out result)) {
                var specific=catalog.ExplicitTemplates.Where(r=>r.Source!="Round {round}").OrderByDescending(r=>r.Literals.Sum(x=>x.Length)).ToList();
                string[] lines=value.Split('\n');
                for(int i=0;i<lines.Length;i++) {
                    string translated;
                    if(catalog.ExplicitBySource.TryGetValue(lines[i],out translated) || TryTranslateTemplate(specific,lines[i],out translated))lines[i]=translated;
                    else if(!lines[i].StartsWith("Round ",StringComparison.Ordinal) && TryTranslateTooltipSlice(catalog,lines[i],out translated))lines[i]=translated;
                }
                result=string.Join("\n",lines);
            }
            if(AuctionEnglishFragments==null) {
                var sources=catalog.ExplicitBySource.Keys.Where(k=>k.Length>=3 && Regex.IsMatch(k,"[A-Za-z]") && !k.Any(ch=>ch>=0x3400 && ch<=0x9fff) && !k.Contains("{"))
                    .OrderByDescending(k=>k.Length).Select(k=>(char.IsLetter(k[0])?"(?<![A-Za-z])":"")+Regex.Escape(k)+(char.IsLetter(k[k.Length-1])?"(?![A-Za-z])":"")).ToArray();
                if(sources.Length>0)AuctionEnglishFragments=new Regex(string.Join("|",sources),RegexOptions.CultureInvariant);
            }
            if(AuctionEnglishFragments!=null)result=AuctionEnglishFragments.Replace(result,m=>catalog.ExplicitBySource[m.Value]);
            return result;
        }
        private static string AuctionWrapperName(MethodBase method)
        { string wrapper=null;
if(method.DeclaringType==typeof(Dialogue) && method.Name=="SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(string),typeof(string)})) return "AuctionDialogue";
if (method.DeclaringType == typeof(global::TMPro.TMP_Text) && method.Name == "set_text" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="AuctionWrap0";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "AddLine" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="AuctionWrap1";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "Add" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::RenderHandler.ColorPalette),typeof(global::System.String),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="AuctionWrap2";
if (method.DeclaringType == typeof(global::PixelWindow) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.Boolean),typeof(global::System.String),typeof(global::WindowStyle)})) wrapper="AuctionWrap3";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Action),typeof(global::System.Int32),typeof(global::System.Int32)})) wrapper="AuctionWrap4";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == "SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="AuctionWrap5";
if (method.DeclaringType == typeof(global::MachineryText) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Int32),typeof(global::TMPro.HorizontalAlignmentOptions),typeof(global::System.Boolean)})) wrapper="AuctionWrap6";
if (method.DeclaringType == typeof(global::MachineryRadioButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::System.Action<global::System.Boolean>)})) wrapper="AuctionWrap7";
            return wrapper;
        }
        private static bool HasAuctionDisplayCall(MethodInfo method)
        {
            var body=method.GetMethodBody();if(body==null)return false;
            var bytes=body.GetILAsByteArray();
            for(int i=0;i+4<bytes.Length;i++)
                if(bytes[i]==0x28||bytes[i]==0x6f||bytes[i]==0x73)
                    try { var call=method.Module.ResolveMethod(BitConverter.ToInt32(bytes,i+1)); if(call!=null && AuctionWrapperName(call)!=null)return true; }catch{}
            return false;
        }
private static void AuctionWrap0(global::TMPro.TMP_Text instance,global::System.String a0) { instance.text=AuctionDisplayText(a0); }
private static global::RichTextBuilder AuctionWrap1(global::RichTextBuilder instance,global::System.String a0,global::System.Boolean a1,global::RenderHandler.ColorPalette a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.AddLine(AuctionDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::RichTextBuilder AuctionWrap2(global::RichTextBuilder instance,global::System.String a0,global::RenderHandler.ColorPalette a1,global::System.String a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.Add(AuctionDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::PixelWindow AuctionWrap3(global::System.Boolean a0,global::System.String a1,global::WindowStyle a2) { return new global::PixelWindow(a0,AuctionDisplayText(a1),a2); }
private static global::MachineryButton AuctionWrap4(global::System.String a0,global::System.Action a1,global::System.Int32 a2,global::System.Int32 a3) { return new global::MachineryButton(AuctionDisplayText(a0),a1,a2,a3); }
private static global::MachineryButton AuctionWrap5(global::MachineryButton instance,global::System.String a0) { return instance.SetText(AuctionDisplayText(a0)); }
private static global::MachineryText AuctionWrap6(global::System.String a0,global::System.Int32 a1,global::TMPro.HorizontalAlignmentOptions a2,global::System.Boolean a3) { return new global::MachineryText(AuctionDisplayText(a0),a1,a2,a3); }
private static global::MachineryRadioButton AuctionWrap7(global::System.String a0,global::System.Boolean a1,global::System.Action<global::System.Boolean> a2) { return new global::MachineryRadioButton(AuctionDisplayText(a0),a1,a2); }
        private static IEnumerable<CodeInstruction> AuctionDisplayTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach(var instruction in instructions) {
                var method=instruction.operand as MethodBase;
                string wrapper=method==null?null:AuctionWrapperName(method);
                if(wrapper!=null){instruction.opcode=System.Reflection.Emit.OpCodes.Call;instruction.operand=typeof(KoreanPack).GetMethod(wrapper,BindingFlags.Static|BindingFlags.NonPublic);}
                yield return instruction;
            }
        }

        private static Dialogue AuctionDialogue(Dialogue instance,string title,string text) { return instance.SetText(AuctionDisplayText(title),AuctionDisplayText(text)); }
        private static void CorpStoryBilingualPostfix(string en,ref string __result)
        {
            if(_enabled && IsKorean() && !string.IsNullOrEmpty(en))__result=CorpStoryDisplayText(en);
        }
        private static void CorpStoryBilingualArrayPostfix(string[] en,ref string[] __result)
        {
            if(!_enabled || !IsKorean() || en==null)return;
            __result=en.Select(CorpStoryDisplayText).ToArray();
        }
        private static Regex CorpStoryEnglishFragments;
        private static readonly Dictionary<string,string> CorpStoryDisplayCache=new Dictionary<string,string>(StringComparer.Ordinal);
        private static string CorpStoryDisplayText(string value)
        {
            if(!_enabled || !IsKorean() || string.IsNullOrEmpty(value)) return value;
            Catalog catalog; if(!Catalogs.TryGetValue("CorpPresence",out catalog))return value;
            string cached; if(CorpStoryDisplayCache.TryGetValue(value,out cached))return cached;
            string result=TranslateCorpStoryRenderedText(catalog,value);
            if(CorpStoryDisplayCache.Count>=2048)CorpStoryDisplayCache.Clear();
            CorpStoryDisplayCache[value]=result;return result;
        }
        private static string TranslateCorpStoryRenderedText(Catalog catalog,string value)
        {
            if(catalog==null || string.IsNullOrEmpty(value))return value;
            string result;
            var specific=catalog.ExplicitTemplates.OrderByDescending(r=>r.Literals.Sum(x=>x.Length)).ToList();
            if(!catalog.ExplicitBySource.TryGetValue(value,out result) && !TryTranslateTemplate(specific,value,out result)) {
                string[] lines=value.Split('\n');
                for(int i=0;i<lines.Length;i++) {
                    string translated;
                    if(catalog.ExplicitBySource.TryGetValue(lines[i],out translated) || TryTranslateTemplate(specific,lines[i],out translated) || TryTranslateTooltipSlice(catalog,lines[i],out translated))lines[i]=translated;
                }
                result=string.Join("\n",lines);
            }
            if(CorpStoryEnglishFragments==null) {
                var sources=catalog.ExplicitBySource.Keys.Where(k=>k.Length>=3 && Regex.IsMatch(k,"[A-Za-z]") && !k.Any(ch=>ch>=0x3400 && ch<=0x9fff) && !k.Contains("{"))
                    .OrderByDescending(k=>k.Length).Select(k=>(char.IsLetter(k[0])?"(?<![A-Za-z])":"")+Regex.Escape(k)+(char.IsLetter(k[k.Length-1])?"(?![A-Za-z])":"")).ToArray();
                if(sources.Length>0)CorpStoryEnglishFragments=new Regex(string.Join("|",sources),RegexOptions.CultureInvariant);
            }
            if(CorpStoryEnglishFragments!=null)result=CorpStoryEnglishFragments.Replace(result,m=>catalog.ExplicitBySource[m.Value]);
            return result;
        }
        private static void FactionBilingualPostfix(string zh,ref string __result) { if(_enabled && IsKorean())__result=FactionDisplayText(zh); }
        private static void FactionSingleTextPostfix(string text,ref string __result) { if(_enabled && IsKorean())__result=FactionDisplayText(text); }
        private static void FactionStringPostfix(ref string __result) { __result=FactionDisplayText(__result); }
        private static Catalog FactionFragmentCatalog;
        private static Catalog FactionCachedCatalog;
        private static readonly Dictionary<string,string> FactionDisplayCache=new Dictionary<string,string>(StringComparer.Ordinal);
        private static Regex FactionEnglishFragments;
        private static string FactionDisplayText(string value)
        {
            if(!_enabled || !IsKorean() || string.IsNullOrEmpty(value))return value;
            Catalog catalog;if(!Catalogs.TryGetValue("faction_reputation_expansion",out catalog))return value;
            if(!ReferenceEquals(FactionCachedCatalog,catalog)){FactionDisplayCache.Clear();FactionCachedCatalog=catalog;}
            string result;if(FactionDisplayCache.TryGetValue(value,out result))return result;
            result=TranslateFactionRenderedText(catalog,value);
            if(FactionDisplayCache.Count>=4096)FactionDisplayCache.Clear();FactionDisplayCache[value]=result;return result;
        }
        private static string TranslateFactionRenderedText(Catalog catalog,string value)
        {
            if(catalog==null || string.IsNullOrEmpty(value))return value;
            string result=TranslateCardRenderedText(catalog,value);
            if(!ReferenceEquals(FactionFragmentCatalog,catalog)) {
                var keys=catalog.ExplicitBySource.Keys.Where(k=>k.Length>=3 && Regex.IsMatch(k,"[A-Za-z]") && !ContainsCjkText(k) && !k.Contains("{"))
                    .OrderByDescending(k=>k.Length).Select(k=>(char.IsLetter(k[0])?"(?<![A-Za-z])":"")+Regex.Escape(k)+(char.IsLetter(k[k.Length-1])?"(?![A-Za-z])":"")).ToArray();
                FactionEnglishFragments=keys.Length==0?null:new Regex(string.Join("|",keys),RegexOptions.CultureInvariant);
                FactionFragmentCatalog=catalog;
            }
            return FactionEnglishFragments==null?result:FactionEnglishFragments.Replace(result,m=>catalog.ExplicitBySource[m.Value]);
        }
        private static string FactionWrapperName(MethodBase method)
        { string wrapper=null;
if(method.DeclaringType==typeof(Dialogue) && method.Name=="SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(string),typeof(string)})) return "FactionDialogue";
if (method.DeclaringType == typeof(global::TMPro.TMP_Text) && method.Name == "set_text" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="FactionWrap0";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "AddLine" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="FactionWrap1";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "Add" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::RenderHandler.ColorPalette),typeof(global::System.String),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="FactionWrap2";
if (method.DeclaringType == typeof(global::PixelWindow) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.Boolean),typeof(global::System.String),typeof(global::WindowStyle)})) wrapper="FactionWrap3";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Action),typeof(global::System.Int32),typeof(global::System.Int32)})) wrapper="FactionWrap4";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == "SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="FactionWrap5";
if (method.DeclaringType == typeof(global::MachineryText) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Int32),typeof(global::TMPro.HorizontalAlignmentOptions),typeof(global::System.Boolean)})) wrapper="FactionWrap6";
if (method.DeclaringType == typeof(global::MachineryRadioButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::System.Action<global::System.Boolean>)})) wrapper="FactionWrap7";
            return wrapper;
        }
        private static bool HasFactionDisplayCall(MethodInfo method)
        {
            var body=method.GetMethodBody();if(body==null)return false;
            var bytes=body.GetILAsByteArray();
            for(int i=0;i+4<bytes.Length;i++)
                if(bytes[i]==0x28||bytes[i]==0x6f||bytes[i]==0x73)
                    try { var call=method.Module.ResolveMethod(BitConverter.ToInt32(bytes,i+1)); if(call!=null && FactionWrapperName(call)!=null)return true; }catch{}
            return false;
        }
private static void FactionWrap0(global::TMPro.TMP_Text instance,global::System.String a0) {
    string rendered=FactionDisplayText(a0);instance.text=rendered;
    FontLoader loader=FontLoader.Instance;
    if(ContainsHangulText(rendered) && loader!=null && loader.koreanFont!=null) {instance.font=loader.koreanFont;instance.fontSharedMaterial=loader.koreanFont.material;}
}
private static global::RichTextBuilder FactionWrap1(global::RichTextBuilder instance,global::System.String a0,global::System.Boolean a1,global::RenderHandler.ColorPalette a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.AddLine(FactionDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::RichTextBuilder FactionWrap2(global::RichTextBuilder instance,global::System.String a0,global::RenderHandler.ColorPalette a1,global::System.String a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.Add(FactionDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::PixelWindow FactionWrap3(global::System.Boolean a0,global::System.String a1,global::WindowStyle a2) { return new global::PixelWindow(a0,FactionDisplayText(a1),a2); }
private static global::MachineryButton FactionWrap4(global::System.String a0,global::System.Action a1,global::System.Int32 a2,global::System.Int32 a3) { return new global::MachineryButton(FactionDisplayText(a0),a1,a2,a3); }
private static global::MachineryButton FactionWrap5(global::MachineryButton instance,global::System.String a0) { return instance.SetText(FactionDisplayText(a0)); }
private static global::MachineryText FactionWrap6(global::System.String a0,global::System.Int32 a1,global::TMPro.HorizontalAlignmentOptions a2,global::System.Boolean a3) { return new global::MachineryText(FactionDisplayText(a0),a1,a2,a3); }
private static global::MachineryRadioButton FactionWrap7(global::System.String a0,global::System.Boolean a1,global::System.Action<global::System.Boolean> a2) { return new global::MachineryRadioButton(FactionDisplayText(a0),a1,a2); }
        private static IEnumerable<CodeInstruction> FactionDisplayTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach(var instruction in instructions) {
                var method=instruction.operand as MethodBase;
                string wrapper=method==null?null:FactionWrapperName(method);
                if(wrapper!=null){instruction.opcode=System.Reflection.Emit.OpCodes.Call;instruction.operand=typeof(KoreanPack).GetMethod(wrapper,BindingFlags.Static|BindingFlags.NonPublic);}
                yield return instruction;
            }
        }

        private static Dialogue FactionDialogue(Dialogue instance,string title,string text) { return instance.SetText(FactionDisplayText(title),FactionDisplayText(text)); }
        private static string CorpStoryWrapperName(MethodBase method)
        { string wrapper=null;
if(method.DeclaringType==typeof(Dialogue) && method.Name=="SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(string),typeof(string)})) return "CorpStoryDialogue";
if (method.DeclaringType == typeof(global::TMPro.TMP_Text) && method.Name == "set_text" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="CorpStoryWrap0";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "AddLine" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="CorpStoryWrap1";
if (method.DeclaringType == typeof(global::RichTextBuilder) && method.Name == "Add" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::RenderHandler.ColorPalette),typeof(global::System.String),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::System.Boolean),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette),typeof(global::RenderHandler.ColorPalette)})) wrapper="CorpStoryWrap2";
if (method.DeclaringType == typeof(global::PixelWindow) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.Boolean),typeof(global::System.String),typeof(global::WindowStyle)})) wrapper="CorpStoryWrap3";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Action),typeof(global::System.Int32),typeof(global::System.Int32)})) wrapper="CorpStoryWrap4";
if (method.DeclaringType == typeof(global::MachineryButton) && method.Name == "SetText" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String)})) wrapper="CorpStoryWrap5";
if (method.DeclaringType == typeof(global::MachineryText) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Int32),typeof(global::TMPro.HorizontalAlignmentOptions),typeof(global::System.Boolean)})) wrapper="CorpStoryWrap6";
if (method.DeclaringType == typeof(global::MachineryRadioButton) && method.Name == ".ctor" && method.GetParameters().Select(p=>p.ParameterType).SequenceEqual(new Type[]{typeof(global::System.String),typeof(global::System.Boolean),typeof(global::System.Action<global::System.Boolean>)})) wrapper="CorpStoryWrap7";
            return wrapper;
        }
        private static bool HasCorpStoryDisplayCall(MethodInfo method)
        {
            var body=method.GetMethodBody();if(body==null)return false;
            var bytes=body.GetILAsByteArray();
            for(int i=0;i+4<bytes.Length;i++)
                if(bytes[i]==0x28||bytes[i]==0x6f||bytes[i]==0x73)
                    try { var call=method.Module.ResolveMethod(BitConverter.ToInt32(bytes,i+1)); if(call!=null && CorpStoryWrapperName(call)!=null)return true; }catch{}
            return false;
        }
private static void CorpStoryWrap0(global::TMPro.TMP_Text instance,global::System.String a0) { instance.text=CorpStoryDisplayText(a0); }
private static global::RichTextBuilder CorpStoryWrap1(global::RichTextBuilder instance,global::System.String a0,global::System.Boolean a1,global::RenderHandler.ColorPalette a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.AddLine(CorpStoryDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::RichTextBuilder CorpStoryWrap2(global::RichTextBuilder instance,global::System.String a0,global::RenderHandler.ColorPalette a1,global::System.String a2,global::System.Boolean a3,global::System.Boolean a4,global::System.Boolean a5,global::System.Boolean a6,global::RenderHandler.ColorPalette a7,global::RenderHandler.ColorPalette a8,global::RenderHandler.ColorPalette a9) { return instance.Add(CorpStoryDisplayText(a0),a1,a2,a3,a4,a5,a6,a7,a8,a9); }
private static global::PixelWindow CorpStoryWrap3(global::System.Boolean a0,global::System.String a1,global::WindowStyle a2) { return new global::PixelWindow(a0,CorpStoryDisplayText(a1),a2); }
private static global::MachineryButton CorpStoryWrap4(global::System.String a0,global::System.Action a1,global::System.Int32 a2,global::System.Int32 a3) { return new global::MachineryButton(CorpStoryDisplayText(a0),a1,a2,a3); }
private static global::MachineryButton CorpStoryWrap5(global::MachineryButton instance,global::System.String a0) { return instance.SetText(CorpStoryDisplayText(a0)); }
private static global::MachineryText CorpStoryWrap6(global::System.String a0,global::System.Int32 a1,global::TMPro.HorizontalAlignmentOptions a2,global::System.Boolean a3) { return new global::MachineryText(CorpStoryDisplayText(a0),a1,a2,a3); }
private static global::MachineryRadioButton CorpStoryWrap7(global::System.String a0,global::System.Boolean a1,global::System.Action<global::System.Boolean> a2) { return new global::MachineryRadioButton(CorpStoryDisplayText(a0),a1,a2); }
        private static IEnumerable<CodeInstruction> CorpStoryDisplayTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach(var instruction in instructions) {
                var method=instruction.operand as MethodBase;
                string wrapper=method==null?null:CorpStoryWrapperName(method);
                if(wrapper!=null){instruction.opcode=System.Reflection.Emit.OpCodes.Call;instruction.operand=typeof(KoreanPack).GetMethod(wrapper,BindingFlags.Static|BindingFlags.NonPublic);}
                yield return instruction;
            }
        }

        private static Dialogue CorpStoryDialogue(Dialogue instance,string title,string text) { return instance.SetText(CorpStoryDisplayText(title),CorpStoryDisplayText(text)); }
		private static void CorpGuiLabel(Rect position, string text, GUIStyle style)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			GUI.Label(position, CorpStoryDisplayText(text), style);
		}

		private static string TranslateKnownCatalogText(string alias, string source)
		{
			if (!_enabled || !IsKorean() || string.IsNullOrEmpty(source))
			{
				return source;
			}
			if ((!RuntimeCatalogs.TryGetValue(alias, out var value) && !Catalogs.TryGetValue(alias, out value)) || value == null)
			{
				return source;
			}
			if (TryTranslateTooltipSlice(value, source, out var translated))
			{
				return translated;
			}
			string[] array = source.Split('\n');
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (TryTranslateTooltipSlice(value, array[i], out var translated2) && !string.Equals(array[i], translated2, StringComparison.Ordinal))
				{
					array[i] = translated2;
					flag = true;
				}
			}
			if (!flag)
			{
				return source;
			}
			return string.Join("\n", array);
		}

		private static bool HasVisibleStringArguments(MethodInfo method)
		{
			if (method == null || method.IsSpecialName)
			{
				return false;
			}
			try
			{
				if (method.GetMethodBody() == null)
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
			ParameterInfo[] parameters;
			try
			{
				parameters = method.GetParameters();
			}
			catch
			{
				return false;
			}
			return parameters.Any((ParameterInfo p) => p.ParameterType == typeof(string) && IsVisibleArgumentName(p.Name, method.Name));
		}

		private static bool LooksLikeVisibleStringProducer(MethodInfo method)
		{
			if (method == null || method.ReturnType != typeof(string))
			{
				return false;
			}
			try
			{
				if (method.GetMethodBody() == null)
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
			string text = method.Name ?? string.Empty;
			if (LooksLikeLocalizer(method))
			{
				return true;
			}
			if (method.IsSpecialName)
			{
				if (!text.StartsWith("get_", StringComparison.Ordinal))
				{
					return false;
				}
				return IsVisibleMemberName(text.Substring(4), allowPlainName: true);
			}
			ParameterInfo[] parameters;
			try
			{
				parameters = method.GetParameters();
			}
			catch
			{
				return false;
			}
			if (text.StartsWith("<", StringComparison.Ordinal) && parameters.Length == 0)
			{
				return true;
			}
			return LooksLikeDisplayMethodName(text);
		}

		private static bool LooksLikeLocalizer(MethodInfo method)
		{
			string text = (method.Name ?? string.Empty).ToLowerInvariant();
			string text2 = ((method.DeclaringType == null) ? string.Empty : (method.DeclaringType.FullName ?? method.DeclaringType.Name).ToLowerInvariant());
			int num;
			switch (text)
			{
			default:
				num = ((text == "getlocalized") ? 1 : 0);
				break;
			case "t":
			case "l":
			case "tr":
			case "text":
			case "translate":
			case "localize":
			case "select":
			case "gettext":
				num = 1;
				break;
			}
			bool flag = text2.Contains("local") || text2.Contains("text") || text2.Contains("lang") || text2.Contains("translation") || text2.Contains("i18n") || text2.Contains("l10n");
			if (num == 0 && !flag)
			{
				return false;
			}
			ParameterInfo[] parameters;
			try
			{
				parameters = method.GetParameters();
			}
			catch
			{
				return false;
			}
			return parameters.Any((ParameterInfo p) => p.ParameterType == typeof(string)) | flag;
		}

		private static bool IsVisibleArgumentName(string rawParameterName, string methodName)
		{
			if (string.IsNullOrEmpty(rawParameterName))
			{
				return false;
			}
			string text = NormalizeName(rawParameterName);
			if (IsProtectedName(text))
			{
				return false;
			}
			switch (text)
			{
			case "hint":
			case "text":
			case "summary":
			case "tooltip":
			case "caption":
			case "message":
			case "story":
			case "title":
			case "label":
			case "notes":
			case "dialogue":
			case "subtitle":
			case "displayname":
			case "displaytext":
			case "description":
			case "dialog":
			case "prompt":
			case "shortdescription":
			case "longdescription":
			case "flavortext":
				return true;
			default:
				switch (text)
				{
				case "name":
				case "value":
				case "content":
					return LooksLikeDisplayMethodName(methodName);
				default:
					return IsVisibleMemberName(rawParameterName, allowPlainName: false);
				}
			}
		}

		private static bool LooksLikeDisplayMethodName(string raw)
		{
			if (string.IsNullOrEmpty(raw))
			{
				return false;
			}
			string text = raw.ToLowerInvariant();
			if (new string[16]
			{
				"identifier", "filepath", "filename", "resource", "asset", "prefab", "texture", "sprite", "shader", "material",
				"namespace", "assembly", "guid", "hash", "token", "schema"
			}.Any(text.Contains))
			{
				return false;
			}
			return new string[19]
			{
				"text", "label", "title", "caption", "description", "describe", "display", "tooltip", "prompt", "message",
				"notify", "toast", "banner", "summary", "hint", "dialog", "story", "format", "name"
			}.Any(text.Contains);
		}

		private static bool IsVisibleMemberName(string raw, bool allowPlainName)
		{
			if (string.IsNullOrEmpty(raw))
			{
				return false;
			}
			string text = NormalizeName(raw);
			if (IsProtectedName(text))
			{
				return false;
			}
			if (text == "name")
			{
				return allowPlainName;
			}
			return new string[22]
			{
				"name", "title", "subtitle", "description", "desc", "summary", "flavor", "flavortext", "label", "caption",
				"tooltip", "hint", "message", "text", "displaytext", "dialogue", "dialog", "story", "details", "content",
				"note", "notes"
			}.Any(text.EndsWith);
		}

		private static string NormalizeName(string raw)
		{
			return (raw ?? string.Empty).TrimStart('_').ToLowerInvariant().Replace("_", string.Empty)
				.Replace("-", string.Empty);
		}

		private static bool IsProtectedName(string normalized)
		{
			if (string.IsNullOrEmpty(normalized))
			{
				return false;
			}
			switch (normalized)
			{
			case "id":
			case "key":
			case "path":
			case "identifier":
			case "type":
			case "kind":
			case "tag":
			case "code":
			case "locale":
			case "language":
				return true;
			default:
				if (!normalized.EndsWith("id", StringComparison.Ordinal) && !normalized.EndsWith("key", StringComparison.Ordinal) && !normalized.EndsWith("path", StringComparison.Ordinal) && !normalized.Contains("filename") && !normalized.Contains("filepath") && !normalized.Contains("resourcename") && !normalized.Contains("assetname") && !normalized.Contains("prefabname") && !normalized.Contains("spritename"))
				{
					return normalized.Contains("texturename");
				}
				return true;
			}
		}

		private static bool HasStringArguments(MethodInfo method)
		{
			if (method == null || method.IsSpecialName)
			{
				return false;
			}
			try
			{
				if (method.GetMethodBody() == null)
				{
					return false;
				}
				return method.GetParameters().Any((ParameterInfo p) => p.ParameterType == typeof(string));
			}
			catch
			{
				return false;
			}
		}

		private static bool HasObjectArguments(MethodInfo method)
		{
			if (method == null || method.IsSpecialName)
			{
				return false;
			}
			try
			{
				if (method.GetMethodBody() == null)
				{
					return false;
				}
				ParameterInfo[] parameters = method.GetParameters();
				for (int i = 0; i < parameters.Length; i++)
				{
					Type type = parameters[i].ParameterType;
					if (type.IsByRef)
					{
						type = type.GetElementType();
					}
					if (!(type == null) && !(type == typeof(string)) && !type.IsValueType && !(type == typeof(Type)) && !typeof(Delegate).IsAssignableFrom(type))
					{
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}

		private static bool LooksLikeDisplayCarrierMethod(MethodInfo method)
		{
			if (method == null || method.DeclaringType == null)
			{
				return false;
			}
			string text = NormalizeName(method.Name);
			string text2 = NormalizeName(method.DeclaringType.Name) + " " + text;
			return new string[23]
			{
				"window", "panel", "tooltip", "button", "label", "text", "dialog", "display", "item", "card",
				"factory", "create", "build", "register", "refresh", "update", "apply", "localize", "catalog", "phone",
				"shop", "machine", "ui"
			}.Any(text2.Contains);
		}

		private static void SafeStringArgsPrefix(MethodBase __originalMethod, object[] __args)
		{
			if (!_enabled || !IsKorean() || __originalMethod == null || __originalMethod.DeclaringType == null || __args == null || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				return;
			}
			ParameterInfo[] parameters;
			try
			{
				parameters = __originalMethod.GetParameters();
			}
			catch
			{
				return;
			}
			for (int i = 0; i < parameters.Length && i < __args.Length; i++)
			{
				if (!(parameters[i].ParameterType != typeof(string)) && !IsProtectedName(NormalizeName(parameters[i].Name)))
				{
					string text = __args[i] as string;
					if (!string.IsNullOrEmpty(text) && (IsVisibleArgumentName(parameters[i].Name, __originalMethod.Name) || LooksLikeHumanText(text)) && TryTranslateSource(value, text, out var translated))
					{
						__args[i] = translated;
					}
				}
			}
		}

		private static bool LooksLikeHumanText(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return false;
			}
			foreach (char c in value)
			{
				if ((c >= '㐀' && c <= '鿿') || char.IsWhiteSpace(c) || char.IsPunctuation(c))
				{
					return true;
				}
			}
			return false;
		}

		private static void SafeObjectArgsPrefix(MethodBase __originalMethod, object[] __args)
		{
			if (!_enabled || !IsKorean() || __originalMethod == null || __originalMethod.DeclaringType == null || __args == null || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				return;
			}
			if (_mutationFrames == null)
			{
				_mutationFrames = new Stack<MutationFrame>();
			}
			MutationFrame mutationFrame = new MutationFrame();
			_mutationFrames.Push(mutationFrame);
			Assembly assembly = __originalMethod.DeclaringType.Assembly;
			HashSet<object> visited = new HashSet<object>(ReferenceComparer.Instance);
			int budget = 160;
			for (int i = 0; i < __args.Length; i++)
			{
				if (budget <= 0)
				{
					break;
				}
				TranslateObjectGraph(value, __args[i], assembly, allowTargetOwnedStrings: true, recordMutations: true, mutationFrame, visited, 0, ref budget);
			}
		}

		private static void SafeObjectArgsPostfix(MethodBase __originalMethod, object[] __args)
		{
			MutationFrame frame = null;
			if (_mutationFrames != null && _mutationFrames.Count > 0)
			{
				frame = _mutationFrames.Pop();
			}
			RestoreMutations(frame);
			if (!_enabled || !IsKorean() || __originalMethod == null || __originalMethod.DeclaringType == null || __args == null || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				return;
			}
			Assembly assembly = __originalMethod.DeclaringType.Assembly;
			HashSet<object> visited = new HashSet<object>(ReferenceComparer.Instance);
			int budget = 180;
			for (int i = 0; i < __args.Length; i++)
			{
				if (budget <= 0)
				{
					break;
				}
				TranslateObjectGraph(value, __args[i], assembly, allowTargetOwnedStrings: false, recordMutations: false, null, visited, 0, ref budget);
			}
		}

		private static void SafePixelWindowResultPostfix(MethodBase __originalMethod, PixelWindow __result)
		{
			TranslateCarrierResult(__originalMethod, __result);
		}

		private static void SafeDialogueResultPostfix(MethodBase __originalMethod, Dialogue __result)
		{
			TranslateCarrierResult(__originalMethod, __result);
		}

		private static void SafeStoreClientResultPostfix(MethodBase __originalMethod, StoreClient __result)
		{
			TranslateCarrierResult(__originalMethod, __result);
		}

		private static void TranslateCarrierResult(MethodBase originalMethod, object result)
		{
			if (_enabled && IsKorean() && result != null && !(originalMethod == null) && !(originalMethod.DeclaringType == null) && AssemblyCatalogs.TryGetValue(originalMethod.DeclaringType.Assembly, out var value))
			{
				HashSet<object> visited = new HashSet<object>(ReferenceComparer.Instance);
				int budget = 240;
				TranslateObjectGraph(value, result, originalMethod.DeclaringType.Assembly, allowTargetOwnedStrings: false, recordMutations: false, null, visited, 0, ref budget);
			}
		}

		private static void SafeGameItemWindowResultPostfix(MethodBase __originalMethod, GameItem __result)
		{
			if (_enabled && IsKorean() && __result != null && !(__originalMethod == null) && !(__originalMethod.DeclaringType == null) && AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				TranslateGameItemDisplayFields(value, __result);
				TranslatePixelWindowDisplay(value, __result.contentWindow);
			}
		}

		private static void SafeOutputPixelWindowResultPostfix(MethodBase __originalMethod, PixelWindow __result)
		{
			if (_enabled && IsKorean() && __result != null && !(__originalMethod == null) && !(__originalMethod.DeclaringType == null) && AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				TranslatePixelWindowDisplay(value, __result);
			}
		}

		private static void SafeGameItemWindowArgsPostfix(MethodBase __originalMethod, object[] __args)
		{
			if (!_enabled || !IsKorean() || __args == null || __originalMethod == null || __originalMethod.DeclaringType == null || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				return;
			}
			foreach (object obj in __args)
			{
				GameItem val = (GameItem)((obj is GameItem) ? obj : null);
				if (val != null)
				{
					TranslateGameItemDisplayFields(value, val);
					TranslatePixelWindowDisplay(value, val.contentWindow);
					break;
				}
			}
		}

		private static void TranslateGameItemDisplayFields(Catalog catalog, GameItem item)
		{
			if (catalog != null && item != null)
			{
				if (!string.IsNullOrEmpty(item.name) && TryTranslateTooltipSlice(catalog, item.name, out var translated) && !string.Equals(item.name, translated, StringComparison.Ordinal))
				{
					item.name = translated;
				}
				if (!string.IsNullOrEmpty(item.shortDescription) && TryTranslateTooltipSlice(catalog, item.shortDescription, out translated) && !string.Equals(item.shortDescription, translated, StringComparison.Ordinal))
				{
					item.shortDescription = translated;
				}
				if (!string.IsNullOrEmpty(item.longDescription) && TryTranslateTooltipSlice(catalog, item.longDescription, out translated) && !string.Equals(item.longDescription, translated, StringComparison.Ordinal))
				{
					item.longDescription = translated;
				}
				if (!string.IsNullOrEmpty(item.flavorText) && TryTranslateTooltipSlice(catalog, item.flavorText, out translated) && !string.Equals(item.flavorText, translated, StringComparison.Ordinal))
				{
					item.flavorText = translated;
				}
			}
		}

		private static void TranslatePixelWindowDisplay(Catalog catalog, PixelWindow window)
		{
			if (catalog == null || window == null)
			{
				return;
			}
			try
			{
				string titleString = window.titleString;
				if (!string.IsNullOrEmpty(titleString) && TryTranslateTooltipSlice(catalog, titleString, out var translated) && !string.Equals(titleString, translated, StringComparison.Ordinal))
				{
					window.SetTitle(translated);
				}
				HashSet<object> visited = new HashSet<object>(ReferenceComparer.Instance);
				TranslatePixelElementDisplay(catalog, window.childElement, visited, 0);
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[KoreanPack] PixelWindow output translation skipped: " + ex.Message));
			}
		}

		private static void TranslatePixelElementDisplay(Catalog catalog, PixelElement element, HashSet<object> visited, int depth)
		{
			if (catalog == null || element == null || depth > 8)
			{
				return;
			}
			if (!visited.Add(element))
			{
				return;
			}
			MachineryButton val = (MachineryButton)(object)((element is MachineryButton) ? element : null);
			string translated;
			if (val != null)
			{
				string text = val.text;
				if (!string.IsNullOrEmpty(text) && TryTranslateTooltipSlice(catalog, text, out translated) && !string.Equals(text, translated, StringComparison.Ordinal))
				{
					val.SetText(translated);
				}
				string description = val.description;
				if (!string.IsNullOrEmpty(description) && TryTranslateTooltipSlice(catalog, description, out translated) && !string.Equals(description, translated, StringComparison.Ordinal))
				{
					val.SetDescription(translated);
				}
			}
			MachineryLabel val2 = (MachineryLabel)(object)((element is MachineryLabel) ? element : null);
			if (val2 != null)
			{
				string text2 = val2.GetText();
				if (!string.IsNullOrEmpty(text2) && TryTranslateTooltipSlice(catalog, text2, out translated) && !string.Equals(text2, translated, StringComparison.Ordinal))
				{
					val2.SetText(translated);
				}
			}
			MachineryPanel val3 = (MachineryPanel)(object)((element is MachineryPanel) ? element : null);
			if (val3 != null)
			{
				if (val3.label != null)
				{
					string text3 = val3.label.GetText();
					if (!string.IsNullOrEmpty(text3) && TryTranslateTooltipSlice(catalog, text3, out translated) && !string.Equals(text3, translated, StringComparison.Ordinal))
					{
						val3.SetLabel(translated);
					}
				}
				TranslatePixelElementDisplay(catalog, val3.content, visited, depth + 1);
			}
			MachineryPanelRow val4 = (MachineryPanelRow)(object)((element is MachineryPanelRow) ? element : null);
			if (val4 != null && val4.Panels != null)
			{
				for (int i = 0; i < val4.Panels.Count; i++)
				{
					MachineryPanel val5 = val4.Panels[i];
					if (val5 != null)
					{
						TranslatePixelElementDisplay(catalog, (PixelElement)(object)val5, visited, depth + 1);
					}
				}
			}
			GridPixelElement val6 = (GridPixelElement)(object)((element is GridPixelElement) ? element : null);
			if (val6 == null)
			{
				return;
			}
			int num = Math.Max(0, val6.gridWidth);
			int num2 = Math.Max(0, val6.gridHeight);
			for (int j = 0; j < num; j++)
			{
				for (int k = 0; k < num2; k++)
				{
					PixelElement element2;
					try
					{
						element2 = val6.GetElement(j, k);
					}
					catch
					{
						continue;
					}
					if (element2 != null)
					{
						TranslatePixelElementDisplay(catalog, element2, visited, depth + 1);
					}
				}
			}
		}

		private static void SafeBuyerTypeListArgsPostfix(MethodBase __originalMethod, object[] __args)
		{
			if (!_enabled || !IsKorean() || __args == null || __args.Length == 0 || __originalMethod == null || __originalMethod.DeclaringType == null || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value) || !(__args[0] is IList list))
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				object obj = list[i];
				if (obj == null)
				{
					continue;
				}
				Type type = obj.GetType();
				if (!type.IsValueType || !type.FullName.StartsWith("System.ValueTuple`2", StringComparison.Ordinal))
				{
					continue;
				}
				FieldInfo field = type.GetField("Item2", BindingFlags.Instance | BindingFlags.Public);
				if (!(field == null) && !(field.FieldType != typeof(string)))
				{
					string text = field.GetValue(obj) as string;
					if (!string.IsNullOrEmpty(text) && TryTranslateTooltipSlice(value, text, out var translated) && !string.Equals(text, translated, StringComparison.Ordinal))
					{
						field.SetValue(obj, translated);
						list[i] = obj;
					}
				}
			}
		}

		private static void SafeTentacleNightReportArgsPrefix(object[] __args)
		{
			if (!_enabled || !_catalogsReady || !IsKorean() || __args == null || __args.Length == 0 || (!RuntimeCatalogs.TryGetValue("lewd_meme", out var value) && !Catalogs.TryGetValue("lewd_meme", out value)) || value == null)
			{
				return;
			}
			for (int i = 0; i < __args.Length; i++)
			{
				string text = __args[i] as string;
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = BuildTentacleNightReport(value, text);
					if (!string.Equals(text, text2, StringComparison.Ordinal))
					{
						__args[i] = text2;
					}
				}
			}
		}

		private static string BuildTentacleNightReport(Catalog catalog, string source)
		{
			if (catalog == null || string.IsNullOrEmpty(source))
			{
				return source;
			}
			try
			{
				int num = source.IndexOf("在【触手本体】内被你的触手调教了一夜", StringComparison.Ordinal);
				bool flag = source.StartsWith("触手本体蠕动了整夜", StringComparison.Ordinal);
				if (num < 0 && !flag)
				{
					return source;
				}
				string text = "，产出了 ";
				string value = "，什么也没产出";
				int num2 = source.IndexOf(text, StringComparison.Ordinal);
				bool flag2 = source.EndsWith(value, StringComparison.Ordinal);
				List<string> list = new List<string>();
				if (num >= 0)
				{
					foreach (Match item in Regex.Matches(source.Substring(0, num), "(\\d+)个【([^】]+)】"))
					{
						string value2 = item.Groups[1].Value;
						string value3 = item.Groups[2].Value;
						if (!TryTranslateTooltipSlice(catalog, value3, out var translated))
						{
							translated = value3;
						}
						if (!catalog.ByKey.TryGetValue("pskb_night_report_maid", out var value4) || string.IsNullOrEmpty(value4))
						{
							value4 = "{count}명 【{type}】";
						}
						list.Add(value4.Replace("{count}", value2).Replace("{type}", translated));
					}
				}
				List<string> list2 = new List<string>();
				if (num2 >= 0)
				{
					foreach (Match item2 in Regex.Matches(source.Substring(num2 + text.Length), "【([^】]+)】\\*(\\d+)"))
					{
						string value5 = item2.Groups[1].Value;
						string value6 = item2.Groups[2].Value;
						if (!TryTranslateTooltipSlice(catalog, value5, out var translated2))
						{
							translated2 = value5;
						}
						if (!catalog.ByKey.TryGetValue("pskb_night_report_output", out var value7) || string.IsNullOrEmpty(value7))
						{
							value7 = "【{product}】 ×{count}";
						}
						list2.Add(value7.Replace("{product}", translated2).Replace("{count}", value6));
					}
				}
				string value8;
				if (flag)
				{
					if (!catalog.ByKey.TryGetValue("pskb_night_report_body_empty", out value8) || string.IsNullOrEmpty(value8))
					{
						return source;
					}
				}
				else if (!catalog.ByKey.TryGetValue("pskb_night_report_body", out value8) || string.IsNullOrEmpty(value8))
				{
					return source;
				}
				string text2 = value8.Replace("{maids}", string.Join(", ", list.ToArray()));
				string value9;
				if (flag2 || list2.Count == 0)
				{
					if (!catalog.ByKey.TryGetValue("pskb_night_report_no_output", out value9))
					{
						value9 = string.Empty;
					}
				}
				else
				{
					if (!catalog.ByKey.TryGetValue("pskb_night_report_outputs", out value9))
					{
						value9 = "{outputs}";
					}
					value9 = value9.Replace("{outputs}", string.Join(", ", list2.ToArray()));
				}
				return text2.Replace("{output_clause}", value9 ?? string.Empty);
			}
			catch
			{
				return source;
			}
		}

		private static void SafeStorageBoxLabelStatePostfix(MethodBase __originalMethod)
		{
			if (!_enabled || !IsKorean() || __originalMethod == null || __originalMethod.DeclaringType == null || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				return;
			}
			try
			{
				FieldInfo field = __originalMethod.DeclaringType.GetField("Entries", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				IDictionary dictionary = ((field == null) ? null : (field.GetValue(null) as IDictionary));
				if (dictionary == null)
				{
					return;
				}
				string value2 = null;
				value.ByKey.TryGetValue("pskb_box_label_short", out value2);
				foreach (DictionaryEntry item in dictionary)
				{
					object value3 = item.Value;
					if (value3 == null)
					{
						continue;
					}
					Type type = value3.GetType();
					FieldInfo field2 = type.GetField("BaseName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					FieldInfo field3 = type.GetField("Label", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (field2 == null || field2.FieldType != typeof(string) || field3 == null)
					{
						continue;
					}
					string text = field2.GetValue(value3) as string;
					if (string.IsNullOrEmpty(text))
					{
						continue;
					}
					string text2 = text;
					if (TryTranslateTooltipSlice(value, text, out var translated) && !string.Equals(text, translated, StringComparison.Ordinal))
					{
						text2 = translated;
						if (string.Equals(text, "收纳箱", StringComparison.Ordinal) && !string.IsNullOrEmpty(value2))
						{
							text2 = value2;
						}
						field2.SetValue(value3, text2);
					}
					else if (!ContainsHangulText(text))
					{
						continue;
					}
					object value4 = field3.GetValue(value3);
					if (value4 == null)
					{
						continue;
					}
					PropertyInfo property = value4.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
					if (property == null || !property.CanRead || !property.CanWrite || property.PropertyType != typeof(string))
					{
						continue;
					}
					string text3 = property.GetValue(value4, null) as string;
					bool num = !string.IsNullOrEmpty(text3) && text3.IndexOf('\n') >= 0;
					string text4 = (text2 ?? string.Empty).Replace(" ", "");
					string text5 = text4.Substring(0, Math.Min(3, text4.Length));
					if (num && text5.Length > 1)
					{
						text5 = string.Join("\n", (from c in text5.ToCharArray()
							select c.ToString()).ToArray());
					}
					TMP_Text val = (TMP_Text)((value4 is TMP_Text) ? value4 : null);
					if ((Object)(object)val != (Object)null && ContainsHangulText(text5))
					{
						TMP_FontAsset val2 = FindKoreanTmpFont(val);
						if ((Object)(object)val2 != (Object)null)
						{
							val.font = val2;
						}
					}
					property.SetValue(value4, text5, null);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[KoreanPack] Storage box-label output translation skipped: " + ex.Message));
			}
		}

		private static bool ContainsHangulText(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			foreach (char c in value)
			{
				if (c >= '가' && c <= '힣')
				{
					return true;
				}
			}
			return false;
		}

		private static TMP_FontAsset FindKoreanTmpFont(TMP_Text exclude)
		{
			if ((Object)(object)_koreanTmpFont != (Object)null)
			{
				return _koreanTmpFont;
			}
			try
			{
				TextMeshProUGUI[] array = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
				foreach (TextMeshProUGUI val in array)
				{
					if (!((Object)(object)val == (Object)null) && !((Object)(object)val == (Object)(object)exclude) && !((Object)(object)((TMP_Text)val).font == (Object)null))
					{
						string text = string.Empty;
						try
						{
							text = ((Object)((Component)val).gameObject).name ?? string.Empty;
						}
						catch
						{
						}
						if (text.IndexOf("Dream_BoxLabel", StringComparison.OrdinalIgnoreCase) < 0 && ContainsHangulText(((TMP_Text)val).text))
						{
							_koreanTmpFont = ((TMP_Text)val).font;
							return _koreanTmpFont;
						}
					}
				}
			}
			catch
			{
			}
			return null;
		}

		private static void SafeGameItemResultPostfix(MethodBase __originalMethod, GameItem __result)
		{
			if (_enabled && IsKorean() && __result != null && !(__originalMethod == null) && !(__originalMethod.DeclaringType == null) && AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				HashSet<object> visited = new HashSet<object>(ReferenceComparer.Instance);
				int budget = 120;
				TranslateObjectGraph(value, __result, __originalMethod.DeclaringType.Assembly, allowTargetOwnedStrings: false, recordMutations: false, null, visited, 0, ref budget);
			}
		}

		private static void SafeTargetStaticStatePostfix(MethodBase __originalMethod)
		{
			if (!_enabled || !IsKorean() || __originalMethod == null || __originalMethod.DeclaringType == null || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				return;
			}
			Type declaringType = __originalMethod.DeclaringType;
			FieldInfo[] fields;
			try
			{
				fields = declaringType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
			catch
			{
				return;
			}
			HashSet<object> visited = new HashSet<object>(ReferenceComparer.Instance);
			int budget = 220;
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (budget > 0 && !(fieldInfo == null) && !fieldInfo.IsLiteral)
				{
					object value2;
					try
					{
						value2 = fieldInfo.GetValue(null);
					}
					catch
					{
						continue;
					}
					TranslateObjectGraph(value, value2, declaringType.Assembly, allowTargetOwnedStrings: false, recordMutations: false, null, visited, 0, ref budget);
					continue;
				}
				break;
			}
		}

		private static void RestoreMutations(MutationFrame frame)
		{
			if (frame == null)
			{
				return;
			}
			for (int num = frame.Mutations.Count - 1; num >= 0; num--)
			{
				MemberMutation memberMutation = frame.Mutations[num];
				try
				{
					if (memberMutation.Field != null)
					{
						memberMutation.Field.SetValue(memberMutation.Target, memberMutation.Original);
					}
					else if (memberMutation.Property != null)
					{
						memberMutation.Property.SetValue(memberMutation.Target, memberMutation.Original, null);
					}
				}
				catch
				{
				}
			}
		}

		private static void TranslateObjectGraph(Catalog catalog, object value, Assembly targetAssembly, bool allowTargetOwnedStrings, bool recordMutations, MutationFrame frame, HashSet<object> visited, int depth, ref int budget)
		{
			if (catalog == null || value == null || budget <= 0 || depth > 3)
			{
				return;
			}
			Type type = value.GetType();
			if (type == typeof(string) || type.IsPrimitive || type.IsEnum || type == typeof(decimal) || typeof(Delegate).IsAssignableFrom(type))
			{
				return;
			}
			if (!type.IsValueType)
			{
				if (!visited.Add(value))
				{
					return;
				}
				budget--;
			}
			if (value is IDictionary dictionary)
			{
				int num = 0;
				{
					foreach (DictionaryEntry item in dictionary)
					{
						if (++num > 80 || budget <= 0)
						{
							break;
						}
						TranslateObjectGraph(catalog, item.Value, targetAssembly, allowTargetOwnedStrings, recordMutations, frame, visited, depth + 1, ref budget);
					}
					return;
				}
			}
			if (value is IEnumerable enumerable && !(value is string))
			{
				int num2 = 0;
				try
				{
					foreach (object? item2 in enumerable)
					{
						if (++num2 <= 80 && budget > 0)
						{
							TranslateObjectGraph(catalog, item2, targetAssembly, allowTargetOwnedStrings, recordMutations, frame, visited, depth + 1, ref budget);
							continue;
						}
						break;
					}
					return;
				}
				catch
				{
					return;
				}
			}
			bool flag = type.Assembly == targetAssembly;
			bool flag2 = IsDisplayCarrierType(type);
			if (!flag && !flag2)
			{
				return;
			}
			if (!flag || !allowTargetOwnedStrings)
			{
			}
			FieldInfo[] array;
			try
			{
				array = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			catch
			{
				array = Array.Empty<FieldInfo>();
			}
			FieldInfo[] array2 = array;
			foreach (FieldInfo fieldInfo in array2)
			{
				if (fieldInfo == null || fieldInfo.IsStatic || budget <= 0)
				{
					continue;
				}
				object value2;
				try
				{
					value2 = fieldInfo.GetValue(value);
				}
				catch
				{
					continue;
				}
				if (fieldInfo.FieldType == typeof(string))
				{
					if (!(!flag | allowTargetOwnedStrings | flag2) || !IsVisibleMemberName(fieldInfo.Name, allowPlainName: true))
					{
						continue;
					}
					string text = value2 as string;
					if (string.IsNullOrEmpty(text) || !TryTranslateSource(catalog, text, out var translated) || string.Equals(text, translated, StringComparison.Ordinal))
					{
						continue;
					}
					try
					{
						if (recordMutations)
						{
							frame?.Mutations.Add(new MemberMutation
							{
								Target = value,
								Field = fieldInfo,
								Original = text
							});
						}
						fieldInfo.SetValue(value, translated);
					}
					catch
					{
					}
				}
				else if (ShouldDescendInto(fieldInfo.FieldType, targetAssembly))
				{
					TranslateObjectGraph(catalog, value2, targetAssembly, allowTargetOwnedStrings, recordMutations, frame, visited, depth + 1, ref budget);
				}
			}
			PropertyInfo[] array3;
			try
			{
				array3 = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			}
			catch
			{
				array3 = Array.Empty<PropertyInfo>();
			}
			PropertyInfo[] array4 = array3;
			foreach (PropertyInfo propertyInfo in array4)
			{
				if (propertyInfo == null || budget <= 0 || propertyInfo.GetIndexParameters().Length != 0)
				{
					continue;
				}
				MethodInfo? getMethod = propertyInfo.GetGetMethod(nonPublic: false);
				MethodInfo setMethod = propertyInfo.GetSetMethod(nonPublic: false);
				if (getMethod == null)
				{
					continue;
				}
				if (propertyInfo.PropertyType == typeof(string))
				{
					if (setMethod == null || (flag && !allowTargetOwnedStrings && !flag2) || !IsVisibleMemberName(propertyInfo.Name, allowPlainName: true))
					{
						continue;
					}
					string text2;
					try
					{
						text2 = propertyInfo.GetValue(value, null) as string;
					}
					catch
					{
						continue;
					}
					if (string.IsNullOrEmpty(text2) || !TryTranslateSource(catalog, text2, out var translated2) || string.Equals(text2, translated2, StringComparison.Ordinal))
					{
						continue;
					}
					try
					{
						if (recordMutations)
						{
							frame?.Mutations.Add(new MemberMutation
							{
								Target = value,
								Property = propertyInfo,
								Original = text2
							});
						}
						propertyInfo.SetValue(value, translated2, null);
					}
					catch
					{
					}
				}
				else if (ShouldDescendInto(propertyInfo.PropertyType, targetAssembly) && flag2)
				{
					object value3;
					try
					{
						value3 = propertyInfo.GetValue(value, null);
					}
					catch
					{
						continue;
					}
					TranslateObjectGraph(catalog, value3, targetAssembly, allowTargetOwnedStrings, recordMutations, frame, visited, depth + 1, ref budget);
				}
			}
		}

		private static bool ShouldDescendInto(Type type, Assembly targetAssembly)
		{
			if (type == null || type == typeof(string) || type.IsPrimitive || type.IsEnum || type == typeof(decimal) || typeof(Delegate).IsAssignableFrom(type))
			{
				return false;
			}
			if (!(type.Assembly == targetAssembly) && !typeof(IEnumerable).IsAssignableFrom(type))
			{
				return IsDisplayCarrierType(type);
			}
			return true;
		}

		private static bool IsDisplayCarrierType(Type type)
		{
			if (type == null)
			{
				return false;
			}
			string text = ((type.FullName ?? type.Name) ?? string.Empty).ToLowerInvariant();
			if (!text.Contains("gameitem") && !text.Contains("richtext") && !text.Contains("machinery") && !text.Contains("button") && !text.Contains("label") && !text.Contains("tooltip") && !text.Contains("dialog") && !text.Contains("pixelelement") && !text.Contains("pixelwindow") && !text.Contains("storeclient") && !text.Contains("textmesh") && !text.EndsWith(".text", StringComparison.Ordinal))
			{
				return text.Contains("uielement");
			}
			return true;
		}

		private static void SafeBilingualStringResultPostfix(MethodBase __originalMethod, object[] __args, ref string __result)
		{
			if (!_enabled || !IsKorean() || __originalMethod == null || __originalMethod.DeclaringType == null || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				return;
			}
			string translated;
			if (__args != null)
			{
				for (int i = 0; i < __args.Length; i++)
				{
					string text = __args[i] as string;
					if (!string.IsNullOrEmpty(text) && TryTranslateSource(value, text, out translated))
					{
						__result = translated;
						return;
					}
				}
			}
			if (!string.IsNullOrEmpty(__result) && TryTranslateSource(value, __result, out translated))
			{
				__result = translated;
			}
		}

		private static void SafeBilingualStringArrayResultPostfix(MethodBase __originalMethod, object[] __args, ref string[] __result)
		{
			if (!_enabled || !IsKorean() || __originalMethod == null || __originalMethod.DeclaringType == null || __result == null || __result.Length == 0 || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				return;
			}
			for (int i = 0; i < __result.Length; i++)
			{
				bool flag = false;
				string translated;
				if (__args != null)
				{
					for (int j = 0; j < __args.Length; j++)
					{
						if (__args[j] is string[] array && i < array.Length)
						{
							string text = array[i];
							if (!string.IsNullOrEmpty(text) && TryTranslateSource(value, text, out translated))
							{
								__result[i] = translated;
								flag = true;
								break;
							}
						}
					}
				}
				if (!flag)
				{
					string text2 = __result[i];
					if (!string.IsNullOrEmpty(text2) && TryTranslateSource(value, text2, out translated))
					{
						__result[i] = translated;
					}
				}
			}
		}

		private static void SafeStringArrayResultPostfix(MethodBase __originalMethod, ref string[] __result)
		{
			if (!_enabled || !IsKorean() || __originalMethod == null || __originalMethod.DeclaringType == null || __result == null || __result.Length == 0 || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				return;
			}
			for (int i = 0; i < __result.Length; i++)
			{
				string text = __result[i];
				if (!string.IsNullOrEmpty(text) && TryTranslateSource(value, text, out var translated))
				{
					__result[i] = translated;
				}
			}
		}

		private static void SafeStringResultPostfix(MethodBase __originalMethod, ref string __result)
		{
			if (_enabled && IsKorean() && !(__originalMethod == null) && !(__originalMethod.DeclaringType == null) && !string.IsNullOrEmpty(__result) && AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value) && TryTranslateSource(value, __result, out var translated))
			{
				__result = translated;
			}
		}

		private static void SafeVisibleStringArgsPrefix(MethodBase __originalMethod, object[] __args)
		{
			if (!_enabled || !IsKorean() || __originalMethod == null || __originalMethod.DeclaringType == null || __args == null || !AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out var value))
			{
				return;
			}
			ParameterInfo[] parameters;
			try
			{
				parameters = __originalMethod.GetParameters();
			}
			catch
			{
				return;
			}
			for (int i = 0; i < parameters.Length && i < __args.Length; i++)
			{
				if (!(parameters[i].ParameterType != typeof(string)) && IsVisibleArgumentName(parameters[i].Name, __originalMethod.Name))
				{
					string text = __args[i] as string;
					if (!string.IsNullOrEmpty(text) && TryTranslateSource(value, text, out var translated))
					{
						__args[i] = translated;
					}
				}
			}
		}

		private static bool IsKorean()
		{
			try
			{
				return IsKoreanCode(LocHelper.GetCurrentLocaleCode());
			}
			catch
			{
				return false;
			}
		}

		private static bool IsKoreanCode(string code)
		{
			if (string.IsNullOrWhiteSpace(code))
			{
				return false;
			}
			if (!string.Equals(code, "ko", StringComparison.OrdinalIgnoreCase) && !code.StartsWith("ko-", StringComparison.OrdinalIgnoreCase))
			{
				return code.StartsWith("ko_", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}

		private static void RestoreNativeEntries()
		{
			foreach (KeyValuePair<LoadedMod, List<SavedValue>> savedValue in SavedValues)
			{
				LoadedMod key = savedValue.Key;
				if (key == null || key.Localization == null)
				{
					continue;
				}
				foreach (SavedValue item in savedValue.Value)
				{
					if (key.Localization.TryGetValue(item.Locale, out var value) && value != null)
					{
						if (item.Existed)
						{
							value[item.Key] = item.Value;
						}
						else
						{
							value.Remove(item.Key);
						}
					}
				}
			}
			SavedValues.Clear();
		}

		private static void UnpatchDynamic()
		{
			if (_dynamicHarmony != null)
			{
				try
				{
					_dynamicHarmony.UnpatchAll("probably_stolen_korean_pack.target_text");
				}
				catch (Exception ex)
				{
					Debug.LogWarning((object)("[KoreanPack] Dynamic patch cleanup failed: " + ex.Message));
				}
				_dynamicHarmony = null;
			}
		}
	}
}










