# Mod source-label trace

## ModSourceHelper.cs:10
~~~csharp
0005: using System.Text.RegularExpressions;
0006: using UnityEngine;
0007: 
0008: public static class ModSourceHelper
0009: {
0010: 	private static readonly Dictionary<Assembly, LoadedMod> modByAssembly = new Dictionary<Assembly, LoadedMod>();
0011: 
0012: 	private static int modByAssemblyCount = -1;
0013: 
0014: 	public static LoadedMod FindModByAssembly(Assembly assembly)
0015: 	{
0016: 		if (assembly == null)
~~~

## ModSourceHelper.cs:162
~~~csharp
0157: 		}
0158: 		foreach (StoreClient item in clientStack)
0159: 		{
0160: 			if (item != null && !before.Contains(item) && item.sourceModId == null)
0161: 			{
0162: 				item.sourceModId = loadedMod.Manifest.ID;
0163: 			}
0164: 		}
0165: 	}
0166: 
0167: 	public static void MarkFromFactory(StoreClient client, Delegate factory)
0168: 	{
~~~

## ModSourceHelper.cs:169
~~~csharp
0164: 		}
0165: 	}
0166: 
0167: 	public static void MarkFromFactory(StoreClient client, Delegate factory)
0168: 	{
0169: 		if (client != null && client.sourceModId == null && ModLoader.Mods.Count != 0)
0170: 		{
0171: 			LoadedMod loadedMod = FindModByDelegate(factory);
0172: 			if (loadedMod != null && loadedMod.Manifest != null)
0173: 			{
0174: 				client.sourceModId = loadedMod.Manifest.ID;
0175: 			}
~~~

## ModSourceHelper.cs:171
~~~csharp
0166: 
0167: 	public static void MarkFromFactory(StoreClient client, Delegate factory)
0168: 	{
0169: 		if (client != null && client.sourceModId == null && ModLoader.Mods.Count != 0)
0170: 		{
0171: 			LoadedMod loadedMod = FindModByDelegate(factory);
0172: 			if (loadedMod != null && loadedMod.Manifest != null)
0173: 			{
0174: 				client.sourceModId = loadedMod.Manifest.ID;
0175: 			}
0176: 		}
0177: 	}
~~~

## ModSourceHelper.cs:174
~~~csharp
0169: 		if (client != null && client.sourceModId == null && ModLoader.Mods.Count != 0)
0170: 		{
0171: 			LoadedMod loadedMod = FindModByDelegate(factory);
0172: 			if (loadedMod != null && loadedMod.Manifest != null)
0173: 			{
0174: 				client.sourceModId = loadedMod.Manifest.ID;
0175: 			}
0176: 		}
0177: 	}
0178: 
0179: 	public static string GetClientModName(StoreClient client)
0180: 	{
~~~

## ModUIManager.cs:16
~~~csharp
0011: 
0012: public class ModUIManager : MonoBehaviour
0013: {
0014: 	private class Row
0015: 	{
0016: 		public LoadedMod mod;
0017: 
0018: 		public ModElement element;
0019: 
0020: 		public GameObject warningIcon;
0021: 
0022: 		public ModRowHoverTooltip warningTooltip;
~~~

## ModUIManager.cs:466
~~~csharp
0461: 		((HorizontalOrVerticalLayoutGroup)val3).childControlWidth = true;
0462: 		((HorizontalOrVerticalLayoutGroup)val3).childControlHeight = true;
0463: 		((HorizontalOrVerticalLayoutGroup)val3).childForceExpandWidth = false;
0464: 		((HorizontalOrVerticalLayoutGroup)val3).childForceExpandHeight = false;
0465: 		ModElement component = val.GetComponent<ModElement>();
0466: 		component.Init(mod.Manifest.ID, mod.Manifest.Name);
0467: 		((Component)CreateIcon("source_icon", val.transform, IsWorkshopMod(mod) ? instance.spriteSteam : instance.spriteFile, raycast: false)).transform.SetSiblingIndex(0);
0468: 		if ((Object)(object)component.modNameTMP != (Object)null)
0469: 		{
0470: 			LayoutElement val4 = ((Component)component.modNameTMP).GetComponent<LayoutElement>();
0471: 			if ((Object)(object)val4 == (Object)null)
0472: 			{
~~~

## ModUIManager.cs:510
~~~csharp
0505: 		obj.preferredWidth = 20f;
0506: 		obj.preferredHeight = 20f;
0507: 		return val2;
0508: 	}
0509: 
0510: 	private static bool IsWorkshopMod(LoadedMod mod)
0511: 	{
0512: 		if (mod != null)
0513: 		{
0514: 			return mod.Source == ModSource.Workshop;
0515: 		}
0516: 		return false;
~~~

## ModUIManager.cs:519
~~~csharp
0514: 			return mod.Source == ModSource.Workshop;
0515: 		}
0516: 		return false;
0517: 	}
0518: 
0519: 	private static ulong SubscriptionId(LoadedMod mod)
0520: 	{
0521: 		if (mod == null)
0522: 		{
0523: 			return 0uL;
0524: 		}
0525: 		if (!IsWorkshopMod(mod))
~~~

## PaperEditorWindow.cs:574
~~~csharp
0569: 		foreach (PaperBookRegistry.Entry entry in list)
0570: 		{
0571: 			if (entry.modId != text)
0572: 			{
0573: 				text = entry.modId;
0574: 				LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == entry.modId);
0575: 				string text2 = ((loadedMod != null) ? (ModLoader.GetModLabel(loadedMod.Manifest) + ((loadedMod.Source == ModSource.Workshop) ? "  (Workshop, read-only)" : "")) : (entry.modId ?? "code"));
0576: 				b.AddLabel(text2);
0577: 			}
0578: 			PaperBookRegistry.Entry captured = entry;
0579: 			b.BeginRow(4f, (TextAnchor)0);
0580: 			b.AddButton(entry.Title + "  (" + entry.bookId + ")", delegate
~~~

## ModLog.cs:9
~~~csharp
0004: {
0005: 	private readonly string modName;
0006: 
0007: 	public ModLog(ModManifest manifest)
0008: 	{
0009: 		modName = manifest.Name;
0010: 	}
0011: 
0012: 	public void Log(object message)
0013: 	{
0014: 		Debug.Log((object)$"[{modName}]: {message}");
0015: 	}
~~~

## PaperEditor.cs:428
~~~csharp
0423: 		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
0424: 		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
0425: 		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
0426: 		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
0427: 		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
0428: 		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
0429: 		if (loadedMod == null || loadedMod.Source != ModSource.Local)
0430: 		{
0431: 			SetStatus("Pick a local mod (Workshop folders are read-only).");
0432: 			return null;
0433: 		}
0434: 		bookId = PaperEditorFiles.SanitizeId(bookId);
~~~

## ModTrust.cs:97
~~~csharp
0092: 	}
0093: 
0094: 	public static ModTrustRecord Approve(LoadedMod mod, ModFingerprint fingerprint, IEnumerable<string> detected)
0095: 	{
0096: 		EnsureLoaded();
0097: 		ModTrustRecord modTrustRecord = Get(mod.Manifest.ID);
0098: 		string text = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
0099: 		ModTrustRecord record = new ModTrustRecord
0100: 		{
0101: 			id = mod.Manifest.ID,
0102: 			source = mod.Source.ToString(),
0103: 			workshopId = mod.WorkshopId.ToString(CultureInfo.InvariantCulture),
~~~

## ModTrust.cs:101
~~~csharp
0096: 		EnsureLoaded();
0097: 		ModTrustRecord modTrustRecord = Get(mod.Manifest.ID);
0098: 		string text = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
0099: 		ModTrustRecord record = new ModTrustRecord
0100: 		{
0101: 			id = mod.Manifest.ID,
0102: 			source = mod.Source.ToString(),
0103: 			workshopId = mod.WorkshopId.ToString(CultureInfo.InvariantCulture),
0104: 			manifestHash = fingerprint.ManifestHash,
0105: 			files = fingerprint.FileEntries(),
0106: 			combinedHash = fingerprint.Combined,
0107: 			declared = ModCapability.Order(mod.Manifest.Capabilities).ToList(),
~~~

## ModLoader.cs:46
~~~csharp
0041: 
0042: 	private static readonly byte[] IModNameBytes = new byte[5] { 73, 77, 111, 100, 0 };
0043: 
0044: 	public static ModLoader Instance { get; private set; }
0045: 
0046: 	public static List<LoadedMod> Mods { get; private set; } = new List<LoadedMod>();
0047: 
0048: 	public static HashSet<string> EnabledMods { get; private set; } = new HashSet<string>();
0049: 
0050: 	public static string ModsPath => Path.Combine(Application.dataPath, "..", "Mods");
0051: 
0052: 	public static bool RestartRequired { get; private set; }
~~~

## ModLoader.cs:161
~~~csharp
0156: 		((MonoBehaviour)this).StartCoroutine(ModTrustList.Fetch());
0157: 	}
0158: 
0159: 	private void Start()
0160: 	{
0161: 		if (!((Object)(object)Instance != (Object)(object)this) && (_rejectedLocalFolders > 0 || Mods.Any((LoadedMod m) => m.LoadError != null && m.Source == ModSource.Local)) && (Object)(object)QuantumConsole.Instance != (Object)null)
0162: 		{
0163: 			QuantumConsole.Instance.Activate();
0164: 		}
0165: 	}
0166: 
0167: 	private void Update()
~~~

## ModLoader.cs:498
~~~csharp
0493: 			mod.ListVerdict = check.Verdict;
0494: 			mod.LastScan = check.Scan;
0495: 			mod.TrustState = ModTrustState.Match;
0496: 			mod.TrustNote = null;
0497: 			SetModEnabled(mod.Manifest.ID, enabled: true);
0498: 			Debug.Log((object)$"[ModLoader] '{mod.Manifest.ID}' approved: hash {check.Fingerprint.Combined}, capabilities {ModCapability.FormatIds(check.Scan.Capabilities)}, list {check.Verdict}");
0499: 		}
0500: 	}
0501: 
0502: 	public static bool SetDeveloperMode(LoadedMod mod, bool on)
0503: 	{
0504: 		if (mod.Source != ModSource.Local)
~~~

## ModLoader.cs:502
~~~csharp
0497: 			SetModEnabled(mod.Manifest.ID, enabled: true);
0498: 			Debug.Log((object)$"[ModLoader] '{mod.Manifest.ID}' approved: hash {check.Fingerprint.Combined}, capabilities {ModCapability.FormatIds(check.Scan.Capabilities)}, list {check.Verdict}");
0499: 		}
0500: 	}
0501: 
0502: 	public static bool SetDeveloperMode(LoadedMod mod, bool on)
0503: 	{
0504: 		if (mod.Source != ModSource.Local)
0505: 		{
0506: 			return false;
0507: 		}
0508: 		bool num = ModTrust.SetDeveloperMode(mod.Manifest.ID, on);
~~~

## ModLoader.cs:508
~~~csharp
0503: 	{
0504: 		if (mod.Source != ModSource.Local)
0505: 		{
0506: 			return false;
0507: 		}
0508: 		bool num = ModTrust.SetDeveloperMode(mod.Manifest.ID, on);
0509: 		if (num)
0510: 		{
0511: 			mod.TrustRecord = ModTrust.Get(mod.Manifest.ID);
0512: 		}
0513: 		return num;
0514: 	}
~~~

## ModLoader.cs:678
~~~csharp
0673: 	}
0674: 
0675: 	private static void LogAudit()
0676: 	{
0677: 		Debug.Log((object)("[ModLoader] audit: game " + Application.version + ", trust list: " + ModTrustList.Describe()));
0678: 		foreach (LoadedMod mod in Mods)
0679: 		{
0680: 			string text = mod.Fingerprint?.Combined ?? "-";
0681: 			string text2 = ((mod.LastScan != null) ? ModCapability.FormatIds(mod.LastScan.Capabilities) : ((mod.TrustRecord != null) ? ModCapability.FormatIds(mod.TrustRecord.detected) : ("declared " + ModCapability.FormatIds(mod.Manifest.Capabilities))));
0682: 			Debug.Log((object)string.Format("[ModLoader] audit: {0} v{1} source={2} workshop={3} owner={4} running={5} trust={6}{7} caps={8} hash={9}{10}{11}", mod.Manifest.ID, mod.Manifest.ModVersion, mod.Source, mod.WorkshopId, WorkshopOwner(mod.WorkshopId), mod.IsEnabled, mod.TrustState, (mod.ListVerdict != ModListVerdict.None) ? ("/" + mod.ListVerdict) : "", text2, text, (mod.TrustNote != null) ? (" note=\"" + mod.TrustNote + "\"") : "", (mod.LoadError != null) ? (" error=\"" + mod.LoadError + "\"") : ""));
0683: 		}
0684: 	}
~~~

## ModLoader.cs:682
~~~csharp
0677: 		Debug.Log((object)("[ModLoader] audit: game " + Application.version + ", trust list: " + ModTrustList.Describe()));
0678: 		foreach (LoadedMod mod in Mods)
0679: 		{
0680: 			string text = mod.Fingerprint?.Combined ?? "-";
0681: 			string text2 = ((mod.LastScan != null) ? ModCapability.FormatIds(mod.LastScan.Capabilities) : ((mod.TrustRecord != null) ? ModCapability.FormatIds(mod.TrustRecord.detected) : ("declared " + ModCapability.FormatIds(mod.Manifest.Capabilities))));
0682: 			Debug.Log((object)string.Format("[ModLoader] audit: {0} v{1} source={2} workshop={3} owner={4} running={5} trust={6}{7} caps={8} hash={9}{10}{11}", mod.Manifest.ID, mod.Manifest.ModVersion, mod.Source, mod.WorkshopId, WorkshopOwner(mod.WorkshopId), mod.IsEnabled, mod.TrustState, (mod.ListVerdict != ModListVerdict.None) ? ("/" + mod.ListVerdict) : "", text2, text, (mod.TrustNote != null) ? (" note=\"" + mod.TrustNote + "\"") : "", (mod.LoadError != null) ? (" error=\"" + mod.LoadError + "\"") : ""));
0683: 		}
0684: 	}
0685: 
0686: 	private static ModManifest ParseManifest(string folderPath)
0687: 	{
0688: 		string problem;
~~~

## ModLoader.cs:836
~~~csharp
0831: 		string[] array = directories;
0832: 		foreach (string folderPath in array)
0833: 		{
0834: 			LoadModFromFolder(folderPath, ModSource.Local, 0uL);
0835: 		}
0836: 		Debug.Log((object)$"[ModLoader] === {Mods.Count} mod(s) registered, {Mods.Count((LoadedMod m) => m.IsEnabled)} enabled ===");
0837: 	}
0838: 
0839: 	private void ApplySavedOrder()
0840: 	{
0841: 		if (_modOrder.Count == 0)
0842: 		{
~~~

## ModLoader.cs:980
~~~csharp
0975: 		ModManifest manifest = ParseManifest(folderPath);
0976: 		if (manifest == null)
0977: 		{
0978: 			return;
0979: 		}
0980: 		LoadedMod loadedMod = Mods.FirstOrDefault((LoadedMod m) => m.Manifest.ID == manifest.ID);
0981: 		if (loadedMod != null && loadedMod.FolderPath != folderPath)
0982: 		{
0983: 			int num;
0984: 			if (loadedMod.Source == ModSource.Local)
0985: 			{
0986: 				num = ((source == ModSource.Workshop) ? 1 : 0);
~~~

## ModLoader.cs:1013
~~~csharp
1008: 			{
1009: 				Debug.LogWarning(string.Format("[{0}] {1} DLL(s) in the mod folder are not listed in <Assemblies> and are never loaded: {2}", GetModLabel(manifest), list.Count, string.Join(", ", list)));
1010: 			}
1011: 			if (!num2)
1012: 			{
1013: 				LoadedMod loadedMod2 = new LoadedMod
1014: 				{
1015: 					Manifest = manifest,
1016: 					FolderPath = folderPath,
1017: 					IsEnabled = false,
1018: 					Source = source,
1019: 					WorkshopId = workshopId,
~~~

## ModLoader.cs:1039
~~~csharp
1034: 					{
1035: 						Debug.LogWarning("[" + GetModLabel(manifest) + "] Could not hash the folder: " + ex.Message);
1036: 					}
1037: 				}
1038: 				Mods.Add(loadedMod2);
1039: 				Debug.Log((object)$"[ModLoader]   Registered (disabled, {source}): {manifest.Name} v{manifest.ModVersion}");
1040: 				return;
1041: 			}
1042: 			TrustGate trustGate;
1043: 			try
1044: 			{
1045: 				trustGate = EvaluateTrust(folderPath, manifest, source, workshopId, list);
~~~

## ModLoader.cs:1058
~~~csharp
1053: 					Note = LocHelper.GetLocalizedUIOrDefault("ui_mods_note_check_failed", "its files could not be checked")
1054: 				};
1055: 			}
1056: 			if (!trustGate.Load)
1057: 			{
1058: 				Mods.Add(new LoadedMod
1059: 				{
1060: 					Manifest = manifest,
1061: 					FolderPath = folderPath,
1062: 					IsEnabled = false,
1063: 					Source = source,
1064: 					WorkshopId = workshopId,
~~~

## ModLoader.cs:1099
~~~csharp
1094: 				loadingMod = null;
1095: 			}
1096: 			LoadedMod loadedMod3 = Mods.LastOrDefault((LoadedMod m) => m.Manifest == manifest);
1097: 			if (loadedMod3 == null)
1098: 			{
1099: 				loadedMod3 = new LoadedMod
1100: 				{
1101: 					Manifest = manifest,
1102: 					FolderPath = folderPath,
1103: 					IsEnabled = false,
1104: 					Source = source,
1105: 					WorkshopId = workshopId
~~~

## ModHook.cs:70
~~~csharp
0065: 
0066: 	public static event Action<RichTextBuilder, GameItem> OnCreateDevTooltipEarly;
0067: 
0068: 	public static event Action<RichTextBuilder, GameItem> OnCreateDevTooltipLate;
0069: 
0070: 	public static event Action<LoadedMod> OnModAssetsLoaded;
0071: 
0072: 	public static event Action<string> OnModConfigApplied;
0073: 
0074: 	public static event Action OnLootTablesLoaded;
0075: 
0076: 	public static event Action<LootRollContext> OnLootRolled;
~~~

## ModHook.cs:206
~~~csharp
0201: 
0202: 	private static void LogHandlerError(string hookName, Delegate handler, Exception e)
0203: 	{
0204: 		MethodInfo method = handler.Method;
0205: 		Assembly assembly = method.DeclaringType?.Assembly;
0206: 		LoadedMod loadedMod = ModSourceHelper.FindModByAssembly(assembly);
0207: 		string text = ((loadedMod != null) ? loadedMod.Manifest.Name : (assembly?.GetName().Name ?? "unknown assembly"));
0208: 		Debug.LogError($"[ModHook] '{hookName}' handler {method.DeclaringType?.FullName}.{method.Name} from '{text}' threw: {e}");
0209: 	}
0210: 
0211: 	internal static void FireOnModItemDirectoryInit(ModItemDirectory directory)
0212: 	{
~~~

## ModHook.cs:207
~~~csharp
0202: 	private static void LogHandlerError(string hookName, Delegate handler, Exception e)
0203: 	{
0204: 		MethodInfo method = handler.Method;
0205: 		Assembly assembly = method.DeclaringType?.Assembly;
0206: 		LoadedMod loadedMod = ModSourceHelper.FindModByAssembly(assembly);
0207: 		string text = ((loadedMod != null) ? loadedMod.Manifest.Name : (assembly?.GetName().Name ?? "unknown assembly"));
0208: 		Debug.LogError($"[ModHook] '{hookName}' handler {method.DeclaringType?.FullName}.{method.Name} from '{text}' threw: {e}");
0209: 	}
0210: 
0211: 	internal static void FireOnModItemDirectoryInit(ModItemDirectory directory)
0212: 	{
0213: 		SafeInvoke("OnModItemDirectoryInit", ModHook.OnModItemDirectoryInit, directory);
~~~

## ModHook.cs:361
~~~csharp
0356: 	internal static void FireOnCreateDevTooltipLate(RichTextBuilder builder, GameItem item)
0357: 	{
0358: 		SafeInvoke("OnCreateDevTooltipLate", ModHook.OnCreateDevTooltipLate, builder, item);
0359: 	}
0360: 
0361: 	internal static void FireOnModAssetsLoaded(LoadedMod mod)
0362: 	{
0363: 		SafeInvoke("OnModAssetsLoaded", ModHook.OnModAssetsLoaded, mod);
0364: 	}
0365: 
0366: 	internal static void FireOnModConfigApplied(string modId)
0367: 	{
~~~

## PaperBookRegistry.cs:682
~~~csharp
0677: 		{
0678: 			Open(fullId);
0679: 		};
0680: 		if (modId != null)
0681: 		{
0682: 			LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
0683: 			ModHelper.SetSource(gameItem, modId, (loadedMod != null) ? ModLoader.GetModLabel(loadedMod.Manifest) : modId);
0684: 		}
0685: 		return gameItem;
0686: 	}
0687: 
0688: 	private static void ApplyItemSprite(GameItem item, string sprite, string modId)
~~~

## PaperEditorFiles.cs:27
~~~csharp
0022: 			stringBuilder.Append((char.IsLetterOrDigit(c) || c == '_') ? c : '_');
0023: 		}
0024: 		return stringBuilder.ToString().Trim('_');
0025: 	}
0026: 
0027: 	public static List<LoadedMod> LocalMods()
0028: 	{
0029: 		return ModLoader.Mods.Where((LoadedMod m) => m.Source == ModSource.Local && m.IsEnabled).ToList();
0030: 	}
0031: 
0032: 	public static void CopyJson(PaperEditor editor)
0033: 	{
~~~

## PaperEditorFiles.cs:29
~~~csharp
0024: 		return stringBuilder.ToString().Trim('_');
0025: 	}
0026: 
0027: 	public static List<LoadedMod> LocalMods()
0028: 	{
0029: 		return ModLoader.Mods.Where((LoadedMod m) => m.Source == ModSource.Local && m.IsEnabled).ToList();
0030: 	}
0031: 
0032: 	public static void CopyJson(PaperEditor editor)
0033: 	{
0034: 		if (editor.IsEditing)
0035: 		{
~~~
