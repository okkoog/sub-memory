using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace ProbablyStolenKoreanPack;

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
				Debug.Log("[KoreanPack] Enabled. Waiting for official ModHook.OnModAssetsLoaded.");
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
				Debug.Log("[KoreanPack] Disabled.");
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
				Debug.LogError("[KoreanPack] OnModAssetsLoaded failed for " + SafeModName(loaded) + ": " + ex);
			}
		}
	}

	private static void BuildCatalogsFromOwnLocalization(LoadedMod own)
	{
		Catalogs.Clear();
		AmbiguousAliases.Clear();
		if (own.Localization == null)
		{
			Debug.LogWarning("[KoreanPack] Own Localization dictionary is null.");
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
		Debug.Log("[KoreanPack] Catalogs ready: " + num + " catalog(s), " + num2 + " key translation(s), " + num3 + " source translation(s).");
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
					Debug.LogWarning("[KoreanPack] Ambiguous catalog alias ignored: " + text);
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
					Debug.LogWarning("[KoreanPack] Ambiguous catalog alias ignored: " + text2);
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
			if (!target.ByKey.TryGetValue(item.Key, out var _) || flag2)
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
				Debug.Log("[KoreanPack] Applied to " + SafeModName(loaded) + ": " + catalog.ByKey.Count + " key(s), " + (catalog.ExplicitBySource.Count + catalog.DerivedBySource.Count) + " source mapping(s).");
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
			LoadedMod loadedMod = ModSourceHelper.FindModByDelegate(DirectoryMaster.GetFactory<GameItem>(item.identifier));
			if (loadedMod != null && loadedMod.Manifest != null)
			{
				runtimeId = loadedMod.Manifest.ID ?? string.Empty;
				value = LocateCatalog(loadedMod);
				if (value != null)
				{
					RegisterRuntimeCatalogAliases(loadedMod, value);
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
				string name = loadedMod.Manifest.Name;
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
				if (loadedMod.WorkshopId != 0L)
				{
					string key = loadedMod.WorkshopId.ToString();
					if (RuntimeCatalogs.TryGetValue(key, out value))
					{
						return value;
					}
					if (Catalogs.TryGetValue(key, out value))
					{
						return value;
					}
				}
				if (loadedMod.ShadowedWorkshopId != 0L)
				{
					string key2 = loadedMod.ShadowedWorkshopId.ToString();
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
				Debug.LogWarning("[KoreanPack] Item owner lookup failed for " + item.identifier + ": " + ex.Message);
			}
		}
		return null;
	}

	private static void PatchSharedDisplayBridges()
	{
		try
		{
			MethodInfo method = typeof(GameItem).GetMethod("GetDisplayName", BindingFlags.Instance | BindingFlags.Public, null, new Type[1] { typeof(bool) }, null);
			if (method != null && PatchDynamic(method, null, "SafeGameItemDisplayNamePostfix", "shared-gameitem-display-name"))
			{
				Debug.Log("[KoreanPack] GameItem display-name bridge enabled.");
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] GameItem display-name bridge failed: " + ex.Message);
		}
		try
		{
			MethodInfo method2 = typeof(TypeHelper).GetMethod("GetTypeDisplayName", BindingFlags.Static | BindingFlags.Public, null, new Type[1] { typeof(string) }, null);
			if (method2 != null && PatchDynamic(method2, null, "SafeTypeDisplayNamePostfix", "shared-type-display-name"))
			{
				Debug.Log("[KoreanPack] Type display-name bridge enabled.");
			}
		}
		catch (Exception ex2)
		{
			Debug.LogWarning("[KoreanPack] Type display-name bridge failed: " + ex2.Message);
		}
		try
		{
			MethodInfo method3 = typeof(ClienteleUIManager).GetMethod("RefreshUI", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
			if (method3 != null && PatchDynamic(method3, null, "SafeClienteleRefreshPostfix", "shared-clientele-display"))
			{
				Debug.Log("[KoreanPack] Clientele display bridge enabled.");
			}
		}
		catch (Exception ex3)
		{
			Debug.LogWarning("[KoreanPack] Clientele display bridge failed: " + ex3.Message);
		}
		try
		{
			MethodInfo method4 = typeof(RecorderUIManager).GetMethod("RefreshUI", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
			if (method4 != null && PatchDynamic(method4, null, "SafeRecorderRefreshPostfix", "shared-recorder-display"))
			{
				Debug.Log("[KoreanPack] Recorder display bridge enabled.");
			}
		}
		catch (Exception ex4)
		{
			Debug.LogWarning("[KoreanPack] Recorder display bridge failed: " + ex4.Message);
		}
		try
		{
			MethodInfo method5 = typeof(DialogUIManager).GetMethod("DisplayClientText", BindingFlags.Instance | BindingFlags.Public, null, new Type[1] { typeof(Dialogue) }, null);
			if (method5 != null && PatchDynamic(method5, "SafeClientDialogueDisplayPrefix", null, "shared-client-dialogue-display"))
			{
				Debug.Log("[KoreanPack] Client dialogue display bridge enabled.");
			}
		}
		catch (Exception ex5)
		{
			Debug.LogWarning("[KoreanPack] Client dialogue display bridge failed: " + ex5.Message);
		}
		try
		{
			int num = 0;
			foreach (MethodInfo method6 in typeof(ModHelper).GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (!string.Equals(method6.Name, "AddAfterhourLocation", StringComparison.Ordinal))
				{
					continue;
				}
				ParameterInfo[] parameters = method6.GetParameters();
				if (!parameters.Any((ParameterInfo p) => p.ParameterType == typeof(Func<string>)))
				{
					continue;
				}
				if (PatchDynamic(method6, "SafeAfterhourLocationArgsPrefix", null, "game-card-afterhour-label"))
				{
					num++;
				}
			}
			if (num > 0)
			{
				Debug.Log("[KoreanPack] GameCardPK after-hours label bridge enabled.");
			}
		}
		catch (Exception ex6)
		{
			Debug.LogWarning("[KoreanPack] GameCardPK after-hours label bridge failed: " + ex6.Message);
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
		catch (Exception ex7)
		{
			Debug.LogWarning("[KoreanPack] Custom UI display bridge failed: " + ex7.Message);
		}
	}

	private static void SafeAfterhourLocationArgsPrefix(MethodBase __originalMethod, object[] __args)
	{
		if (!_enabled || __originalMethod == null || __args == null)
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
		string text = null;
		for (int i = 0; i < parameters.Length && i < __args.Length; i++)
		{
			if (parameters[i].ParameterType == typeof(string) && string.Equals(NormalizeName(parameters[i].Name), "modid", StringComparison.Ordinal))
			{
				text = __args[i] as string;
				break;
			}
		}
		if (string.IsNullOrEmpty(text) && __args.Length > 0)
		{
			text = __args[0] as string;
		}
		if (!string.Equals(text, "game_card_pk", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		for (int j = 0; j < parameters.Length && j < __args.Length; j++)
		{
			if (parameters[j].ParameterType != typeof(Func<string>) || !(__args[j] is Func<string> func))
			{
				continue;
			}
			Func<string> original = func;
			__args[j] = new Func<string>(delegate
			{
				string text2;
				try
				{
					text2 = original();
				}
				catch
				{
					return original();
				}
				return TranslateKnownCatalogText("game_card_pk", text2);
			});
		}
	}

	private static void SafeClienteleRefreshPostfix(ClienteleUIManager __instance)
	{
		if (!_enabled || !_catalogsReady || !IsKorean() || __instance == null)
		{
			return;
		}
		string text = TranslateKnownCatalogText("game_card_pk", "游戏卡");
		if (string.IsNullOrEmpty(text) || string.Equals(text, "游戏卡", StringComparison.Ordinal))
		{
			return;
		}
		TextMeshProUGUI[] array = new TextMeshProUGUI[2] { __instance.merchandiseCustomerTMP, __instance.normalCustomerTMP };
		foreach (TextMeshProUGUI textMeshProUGUI in array)
		{
			if (!((UnityEngine.Object)(object)textMeshProUGUI == null))
			{
				string text2 = textMeshProUGUI.text;
				if (!string.IsNullOrEmpty(text2) && text2.IndexOf("游戏卡", StringComparison.Ordinal) >= 0)
				{
					textMeshProUGUI.text = text2.Replace("游戏卡", text);
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
		StoreClient storeClient = null;
		try
		{
			if (PlayerStore.Instance != null && PlayerStore.Instance.currentClientInstance != null)
			{
				storeClient = PlayerStore.Instance.currentClientInstance.GetClientBlueprint();
			}
		}
		catch
		{
		}
		string text = null;
		if (storeClient != null)
		{
			string a = storeClient.sourceModId ?? string.Empty;
			string text2 = storeClient.identifier ?? string.Empty;
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
		if (storeClient != null && !string.IsNullOrEmpty(storeClient.displayName))
		{
			string text3 = TranslateKnownCatalogText(text, storeClient.displayName);
			if (!string.IsNullOrEmpty(text3))
			{
				storeClient.displayName = text3;
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
		if (!_enabled || !_catalogsReady || !IsKorean() || __instance == null || __instance.ScrollViewContent == null)
		{
			return;
		}
		try
		{
			TextMeshProUGUI[] componentsInChildren = __instance.ScrollViewContent.GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true);
			foreach (TextMeshProUGUI textMeshProUGUI in componentsInChildren)
			{
				if ((UnityEngine.Object)(object)textMeshProUGUI == null)
				{
					continue;
				}
				string text = textMeshProUGUI.text;
				if (!string.IsNullOrEmpty(text) && !string.Equals(text, "---", StringComparison.Ordinal))
				{
					string text2 = TranslateRecorderLine(text);
					if (!string.Equals(text, text2, StringComparison.Ordinal))
					{
						textMeshProUGUI.text = text2;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Recorder display translation skipped: " + ex.Message);
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
		if (ContainsCjkText(__result))
		{
			string runtimeId;
			Catalog catalog = LocateCatalogForItem(__instance, out runtimeId);
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
				Debug.LogWarning("[KoreanPack] Final tooltip bridge target not found.");
			}
			else if (PatchDynamic(method, null, "SafeFinalItemTooltipPostfix", "final-item-tooltip"))
			{
				Debug.Log("[KoreanPack] Final item tooltip bridge enabled.");
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Final tooltip bridge failed: " + ex.Message);
		}
	}

	private static void SafeFinalItemTooltipPostfix(RichTextBuilder builder, GameItem item)
	{
		if (!_enabled || !IsKorean() || builder == null || item == null)
		{
			return;
		}
		string runtimeId;
		Catalog catalog = LocateCatalogForItem(item, out runtimeId);
		if (catalog == null)
		{
			if (!string.IsNullOrEmpty(runtimeId) && FinalTooltipMissLogged.Add(runtimeId))
			{
				Debug.LogWarning("[KoreanPack] Final tooltip catalog miss: " + runtimeId);
			}
			return;
		}
		int num = TranslateFinalTooltipSlices(catalog, builder);
		if (num > 0)
		{
			string text = (string.IsNullOrEmpty(runtimeId) ? catalog.PrimaryId : runtimeId);
			if (FinalTooltipHitLogged.Add(text))
			{
				Debug.Log("[KoreanPack] Final tooltip translation active: " + text + " (" + num + " slice(s) on first hit).");
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
			foreach (object item in enumerable)
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
			Debug.LogWarning("[KoreanPack] Final tooltip translation skipped: " + ex.Message);
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
				string value2;
				bool existed = value.TryGetValue(item.Key, out value2);
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
		Assembly assembly = loaded.Assembly;
		if (AssemblyCatalogs.TryGetValue(assembly, out var value))
		{
			if (value != catalog)
			{
				Debug.LogWarning("[KoreanPack] Assembly already mapped to another catalog: " + assembly.GetName().Name);
			}
		}
		else
		{
			AssemblyCatalogs[assembly] = catalog;
			PatchDisplayMethodsInAssembly(assembly);
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
			if (text.Equals("GameCardPK", StringComparison.OrdinalIgnoreCase))
			{
				num += PatchKnownType(assembly, "GameCardPK.GameCardFactory", new KnownPatch("CreateCard", KnownPatchKind.GameItemResult), new KnownPatch("CreateCard", KnownPatchKind.GameItemWindowResult), new KnownPatch("CreateSkill", KnownPatchKind.GameItemResult), new KnownPatch("ApplySkillData", KnownPatchKind.ObjectArgs), new KnownPatch("AttachChessBoardWindow", KnownPatchKind.GameItemWindowArgs));
				num += PatchKnownType(assembly, "GameCardPK.SkillText", new KnownPatch("ForItem", KnownPatchKind.StringResult), new KnownPatch("ForBattle", KnownPatchKind.StringResult), new KnownPatch("PointLabel", KnownPatchKind.StringResult));
				num += PatchKnownType(assembly, "GameCardPK.CardNPC", new KnownPatch("BuildClient", KnownPatchKind.StoreClientResult));
				num += PatchKnownType(assembly, "GameCardPK.CardClubLocationData", new KnownPatch("get_ButtonLabel", KnownPatchKind.StringResult));
				num += PatchKnownType(assembly, "GameCardPK.UI.UiKit", new KnownPatch("Text", KnownPatchKind.VisibleStringArgs), new KnownPatch("TextMuted", KnownPatchKind.VisibleStringArgs), new KnownPatch("Button", KnownPatchKind.VisibleStringArgs), new KnownPatch("ButtonPrimary", KnownPatchKind.VisibleStringArgs));
				num += PatchKnownType(assembly, "GameCardPK.UI.PanelHost", new KnownPatch("OpenPanel", KnownPatchKind.VisibleStringArgs), new KnownPatch("set_Message", KnownPatchKind.StringArgs));
				num += PatchKnownType(assembly, "GameCardPK.UI.BattlePanel", new KnownPatch("RegisterHover", KnownPatchKind.VisibleStringArgs));
			}
			else if (text.Equals("MidnightPawn", StringComparison.OrdinalIgnoreCase))
			{
				num += PatchKnownType(assembly, "MidnightPawn.Game.TentacleBody", new KnownPatch("Create", KnownPatchKind.GameItemResult), new KnownPatch("Create", KnownPatchKind.GameItemWindowResult), new KnownPatch("BuildWindow", KnownPatchKind.PixelWindowResult), new KnownPatch("DelayReport", KnownPatchKind.TentacleNightReportArgs));
			}
			else if (text.Equals("ItemStorage", StringComparison.OrdinalIgnoreCase))
			{
				num += PatchKnownType(assembly, "ItemStorage.BoxItem", new KnownPatch("Create", KnownPatchKind.GameItemResult), new KnownPatch("Create", KnownPatchKind.GameItemWindowResult), new KnownPatch("NotifyUser", KnownPatchKind.VisibleStringArgs));
				num += PatchKnownType(assembly, "ItemStorage.BoxWindow", new KnownPatch("UpdateTooltip", KnownPatchKind.ObjectArgs), new KnownPatch("UpdateTooltip", KnownPatchKind.GameItemWindowArgs), new KnownPatch("EnsureWindow", KnownPatchKind.StaticDisplayState), new KnownPatch("EnsureWindow", KnownPatchKind.GameItemWindowArgs), new KnownPatch("RefreshButtons", KnownPatchKind.StaticDisplayState), new KnownPatch("RefreshButtons", KnownPatchKind.GameItemWindowArgs));
				num += PatchKnownType(assembly, "ItemStorage.BoxLabel", new KnownPatch("Apply", KnownPatchKind.StorageBoxLabelState), new KnownPatch("Tick", KnownPatchKind.StorageBoxLabelState), new KnownPatch("OnItemRenamed", KnownPatchKind.StorageBoxLabelState));
			}
			else if (text.Equals("XIAOWONightAuction", StringComparison.OrdinalIgnoreCase))
			{
				num += PatchKnownType(assembly, "XIAOWONightAuction.AuctionText", new KnownPatch("Select", KnownPatchKind.BilingualResult), new KnownPatch("DisplayLog", KnownPatchKind.StringResult), new KnownPatch("Get", KnownPatchKind.StringResult));
				num += PatchKnownType(assembly, "XIAOWONightAuction.AuctionExclusiveFactory", new KnownPatch("ApplyLocalizedText", KnownPatchKind.ObjectArgs));
				num += PatchKnownType(assembly, "XIAOWONightAuction.AuctionPhone", new KnownPatch("CreateCard", KnownPatchKind.GameItemResult), new KnownPatch("LocalizeCard", KnownPatchKind.ObjectArgs), new KnownPatch("BuildDialogue", KnownPatchKind.DialogueResult), new KnownPatch("BuildBookingDialogue", KnownPatchKind.DialogueResult), new KnownPatch("Ending", KnownPatchKind.DialogueResult));
			}
			else if (text.Equals("BrewingExpansion", StringComparison.OrdinalIgnoreCase))
			{
				num += PatchKnownType(assembly, "BrewingExpansion.Formal.Figurines", new KnownPatch("Create", KnownPatchKind.GameItemResult));
			}
			else if (text.Equals("CorpPresence", StringComparison.OrdinalIgnoreCase))
			{
				num += PatchKnownType(assembly, "CorpPresence.L", new KnownPatch("T", KnownPatchKind.BilingualResult));
				num += PatchKnownType(assembly, "CorpPresence.UI.MenuLabels", new KnownPatch("Get", KnownPatchKind.StringArrayResult));
				num += PatchKnownType(assembly, "CorpPresence.UI.Ui", new KnownPatch("Button", KnownPatchKind.VisibleStringArgs), new KnownPatch("Label", KnownPatchKind.VisibleStringArgs));
				num += PatchKnownType(assembly, "CorpPresence.UI.CompanyWindow", new KnownPatch("DrawPastel", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawPastelOrder", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("PastelBribeCell", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawPastelDealer", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawPastelTraffic", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawPastelFooter", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawDavre", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawDavreOrder", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawDavreLeviathan", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawDavreCanning", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawSolana", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawSolanaVip", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawSolanaLab", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawSolanaGift", KnownPatchKind.GuiLabelTranspiler), new KnownPatch("DrawSpacerMutual", KnownPatchKind.GuiLabelTranspiler));
				num += PatchKnownType(assembly, "CorpPresence.CompanyVisitors", new KnownPatch("Courier", KnownPatchKind.StoreClientResult));
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Safe target adapter failed for " + text + ": " + ex.Message);
		}
		if (num > 0)
		{
			Debug.Log("[KoreanPack] Safe target adapter: " + text + " -> " + num + " patch(es).");
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
					case KnownPatchKind.StringArgs:
						if (methodInfo.GetParameters().Any((ParameterInfo p) => p.ParameterType == typeof(string)) && PatchDynamic(methodInfo, "SafeStringArgsPrefix", null, "safe-string-args"))
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
					Debug.LogWarning("[KoreanPack] Safe patch skipped: " + typeName + "." + methodInfo.Name + " (" + ex.Message + ")");
				}
			}
		}
		return num;
	}

	private static bool PatchDynamic(MethodBase method, string prefixName, string postfixName, string kind)
	{
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
		HarmonyMethod prefix = (string.IsNullOrEmpty(prefixName) ? null : new HarmonyMethod(typeof(KoreanPack), prefixName));
		HarmonyMethod postfix = (string.IsNullOrEmpty(postfixName) ? null : new HarmonyMethod(typeof(KoreanPack), postfixName));
		_dynamicHarmony.Patch(method, prefix, postfix);
		PatchedMethods.Add(item);
		return true;
	}

	private static bool PatchDynamicTranspiler(MethodBase method, string transpilerName, string kind)
	{
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
		HarmonyMethod transpiler = new HarmonyMethod(typeof(KoreanPack), transpilerName);
		_dynamicHarmony.Patch(method, null, null, transpiler);
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

	private static void CorpGuiLabel(Rect position, string text, GUIStyle style)
	{
		GUI.Label(position, TranslateKnownCatalogText("CorpPresence", text), style);
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
		return parameters.Any((ParameterInfo p) => p.ParameterType == typeof(string)) || flag;
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
		for (int i = 0; i < __args.Length; i++)
		{
			if (__args[i] is GameItem gameItem)
			{
				TranslateGameItemDisplayFields(value, gameItem);
				TranslatePixelWindowDisplay(value, gameItem.contentWindow);
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
			Debug.LogWarning("[KoreanPack] PixelWindow output translation skipped: " + ex.Message);
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
		string translated;
		if (element is MachineryButton { text: var text } machineryButton)
		{
			if (!string.IsNullOrEmpty(text) && TryTranslateTooltipSlice(catalog, text, out translated) && !string.Equals(text, translated, StringComparison.Ordinal))
			{
				machineryButton.SetText(translated);
			}
			string description = machineryButton.description;
			if (!string.IsNullOrEmpty(description) && TryTranslateTooltipSlice(catalog, description, out translated) && !string.Equals(description, translated, StringComparison.Ordinal))
			{
				machineryButton.SetDescription(translated);
			}
		}
		if (element is MachineryLabel machineryLabel)
		{
			string text2 = machineryLabel.GetText();
			if (!string.IsNullOrEmpty(text2) && TryTranslateTooltipSlice(catalog, text2, out translated) && !string.Equals(text2, translated, StringComparison.Ordinal))
			{
				machineryLabel.SetText(translated);
			}
		}
		if (element is MachineryPanel machineryPanel)
		{
			if (machineryPanel.label != null)
			{
				string text3 = machineryPanel.label.GetText();
				if (!string.IsNullOrEmpty(text3) && TryTranslateTooltipSlice(catalog, text3, out translated) && !string.Equals(text3, translated, StringComparison.Ordinal))
				{
					machineryPanel.SetLabel(translated);
				}
			}
			TranslatePixelElementDisplay(catalog, machineryPanel.content, visited, depth + 1);
		}
		if (element is MachineryPanelRow { Panels: not null } machineryPanelRow)
		{
			for (int i = 0; i < machineryPanelRow.Panels.Count; i++)
			{
				MachineryPanel machineryPanel2 = machineryPanelRow.Panels[i];
				if (machineryPanel2 != null)
				{
					TranslatePixelElementDisplay(catalog, machineryPanel2, visited, depth + 1);
				}
			}
		}
		if (!(element is GridPixelElement gridPixelElement))
		{
			return;
		}
		int num = Math.Max(0, gridPixelElement.gridWidth);
		int num2 = Math.Max(0, gridPixelElement.gridHeight);
		for (int j = 0; j < num; j++)
		{
			for (int k = 0; k < num2; k++)
			{
				PixelElement element2;
				try
				{
					element2 = gridPixelElement.GetElement(j, k);
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
				string text5 = text4[..Math.Min(3, text4.Length)];
				if (num && text5.Length > 1)
				{
					text5 = string.Join("\n", (from c in text5.ToCharArray()
						select c.ToString()).ToArray());
				}
				TMP_Text tMP_Text = value4 as TMP_Text;
				if ((UnityEngine.Object)(object)tMP_Text != null && ContainsHangulText(text5))
				{
					TMP_FontAsset tMP_FontAsset = FindKoreanTmpFont(tMP_Text);
					if (tMP_FontAsset != null)
					{
						tMP_Text.font = tMP_FontAsset;
					}
				}
				property.SetValue(value4, text5, null);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Storage box-label output translation skipped: " + ex.Message);
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
		if (_koreanTmpFont != null)
		{
			return _koreanTmpFont;
		}
		try
		{
			TextMeshProUGUI[] array = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
			foreach (TextMeshProUGUI textMeshProUGUI in array)
			{
				if (!((UnityEngine.Object)(object)textMeshProUGUI == null) && !((UnityEngine.Object)(object)textMeshProUGUI == (UnityEngine.Object)(object)exclude) && !(textMeshProUGUI.font == null))
				{
					string text = string.Empty;
					try
					{
						text = ((Component)(object)textMeshProUGUI).gameObject.name ?? string.Empty;
					}
					catch
					{
					}
					if (text.IndexOf("Dream_BoxLabel", StringComparison.OrdinalIgnoreCase) < 0 && ContainsHangulText(textMeshProUGUI.text))
					{
						_koreanTmpFont = textMeshProUGUI.font;
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
				foreach (object item2 in enumerable)
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
				if (!(!flag || allowTargetOwnedStrings || flag2) || !IsVisibleMemberName(fieldInfo.Name, allowPlainName: true))
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
			MethodInfo getMethod = propertyInfo.GetGetMethod(nonPublic: false);
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
				Debug.LogWarning("[KoreanPack] Dynamic patch cleanup failed: " + ex.Message);
			}
			_dynamicHarmony = null;
		}
	}
}
