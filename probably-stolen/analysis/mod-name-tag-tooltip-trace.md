# MOD_NAME_TAG tooltip trace

## ModSourceHelper.cs

~~~csharp
0088: 		foreach (LoadedMod mod in ModLoader.Mods)
0089: 		{
0090: 			if (mod.Manifest != null && mod.Manifest.ID == modId)
0091: 			{
0092: 				return ModLoader.GetModLabel(mod.Manifest);
0093: 			}
0094: 		}
0095: 		return null;
0096: 	}
0097: 
0098: 	public static string GetItemModName(GameItem item)
0099: 	{
0100: 		if (item == null)
0101: 		{
0102: 			return null;
0103: 		}
0104: 		if (item.IsTag("MOD_ID_TAG"))
0105: 		{
0106: 			string text = item.GetTagReadonly("MOD_ID_TAG").GetString();
0107: 			if (!string.IsNullOrEmpty(text))
0108: 			{
0109: 				string text2 = (item.IsTag("MOD_NAME_TAG") ? item.GetTagReadonly("MOD_NAME_TAG").GetString() : null);
~~~

~~~csharp
0094: 		}
0095: 		return null;
0096: 	}
0097: 
0098: 	public static string GetItemModName(GameItem item)
0099: 	{
0100: 		if (item == null)
0101: 		{
0102: 			return null;
0103: 		}
0104: 		if (item.IsTag("MOD_ID_TAG"))
0105: 		{
0106: 			string text = item.GetTagReadonly("MOD_ID_TAG").GetString();
0107: 			if (!string.IsNullOrEmpty(text))
0108: 			{
0109: 				string text2 = (item.IsTag("MOD_NAME_TAG") ? item.GetTagReadonly("MOD_NAME_TAG").GetString() : null);
0110: 				string text3 = GetModDisplayName(text);
0111: 				if (text3 == null)
0112: 				{
0113: 					if (!string.IsNullOrEmpty(text2))
0114: 					{
0115: 						return text2;
~~~

~~~csharp
0096: 	}
0097: 
0098: 	public static string GetItemModName(GameItem item)
0099: 	{
0100: 		if (item == null)
0101: 		{
0102: 			return null;
0103: 		}
0104: 		if (item.IsTag("MOD_ID_TAG"))
0105: 		{
0106: 			string text = item.GetTagReadonly("MOD_ID_TAG").GetString();
0107: 			if (!string.IsNullOrEmpty(text))
0108: 			{
0109: 				string text2 = (item.IsTag("MOD_NAME_TAG") ? item.GetTagReadonly("MOD_NAME_TAG").GetString() : null);
0110: 				string text3 = GetModDisplayName(text);
0111: 				if (text3 == null)
0112: 				{
0113: 					if (!string.IsNullOrEmpty(text2))
0114: 					{
0115: 						return text2;
0116: 					}
0117: 					text3 = text;
~~~

~~~csharp
0099: 	{
0100: 		if (item == null)
0101: 		{
0102: 			return null;
0103: 		}
0104: 		if (item.IsTag("MOD_ID_TAG"))
0105: 		{
0106: 			string text = item.GetTagReadonly("MOD_ID_TAG").GetString();
0107: 			if (!string.IsNullOrEmpty(text))
0108: 			{
0109: 				string text2 = (item.IsTag("MOD_NAME_TAG") ? item.GetTagReadonly("MOD_NAME_TAG").GetString() : null);
0110: 				string text3 = GetModDisplayName(text);
0111: 				if (text3 == null)
0112: 				{
0113: 					if (!string.IsNullOrEmpty(text2))
0114: 					{
0115: 						return text2;
0116: 					}
0117: 					text3 = text;
0118: 				}
0119: 				return text3;
0120: 			}
~~~

~~~csharp
0128: 		{
0129: 			return null;
0130: 		}
0131: 		return ModLoader.GetModLabel(loadedMod.Manifest);
0132: 	}
0133: 
0134: 	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
0135: 	{
0136: 		if (builder != null && EmporiumOption.ShowModSourceInTooltip)
0137: 		{
0138: 			string itemModName = GetItemModName(item);
0139: 			itemModName = (string.IsNullOrEmpty(itemModName) ? null : Regex.Replace(itemModName, "<[^>]*>", "").Trim());
0140: 			if (!string.IsNullOrEmpty(itemModName))
0141: 			{
0142: 				builder.AddLine(itemModName, ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue, bold: false, italic: true);
0143: 			}
0144: 		}
0145: 	}
0146: 
0147: 	public static void MarkNewClients(List<StoreClient> clientStack, HashSet<StoreClient> before, Delegate handler)
0148: 	{
0149: 		if (clientStack == null || before == null)
~~~

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
0181: 		if (client == null || string.IsNullOrEmpty(client.sourceModId))
0182: 		{
0183: 			return null;
0184: 		}
0185: 		return GetModDisplayName(client.sourceModId) ?? client.sourceModId;
0186: 	}
0187: 
0188: 	public static void RecordClientSource(StoreClient client)
0189: 	{
0190: 		if (EmporiumOption.ShowModSourceInRecorder && !((Object)(object)DialogUIManager.Instance == (Object)null))
~~~

~~~csharp
0182: 		{
0183: 			return null;
0184: 		}
0185: 		return GetModDisplayName(client.sourceModId) ?? client.sourceModId;
0186: 	}
0187: 
0188: 	public static void RecordClientSource(StoreClient client)
0189: 	{
0190: 		if (EmporiumOption.ShowModSourceInRecorder && !((Object)(object)DialogUIManager.Instance == (Object)null))
0191: 		{
0192: 			string clientModName = GetClientModName(client);
0193: 			if (!string.IsNullOrEmpty(clientModName))
0194: 			{
0195: 				DialogUIManager.Instance.UpdateDialogHistory("<i>" + LocHelper.GetLocalizedUI("ui_recorder_from_mod", clientModName) + "</i>");
0196: 			}
0197: 		}
0198: 	}
0199: }
~~~

## ModHelper.cs

~~~csharp
0002: using System.Collections.Generic;
0003: using System.Globalization;
0004: using System.IO;
0005: using System.Linq;
0006: using UnityEngine;
0007: using UnityEngine.Localization;
0008: using UnityEngine.Localization.Settings;
0009: 
0010: public class ModHelper
0011: {
0012: 	public const string MOD_ID_TAG = "MOD_ID_TAG";
0013: 
0014: 	public const string MOD_NAME_TAG = "MOD_NAME_TAG";
0015: 
0016: 	private static readonly HashSet<string> warnedLocalizationKeys = new HashSet<string>();
0017: 
0018: 	public const string DEBUG_CATEGORY_STORE = "Store";
0019: 
0020: 	public const string DEBUG_CATEGORY_COMMAND = "Command";
0021: 
0022: 	public const string DEBUG_CATEGORY_SPAWN = "Spawn";
0023: 
~~~

~~~csharp
0004: using System.IO;
0005: using System.Linq;
0006: using UnityEngine;
0007: using UnityEngine.Localization;
0008: using UnityEngine.Localization.Settings;
0009: 
0010: public class ModHelper
0011: {
0012: 	public const string MOD_ID_TAG = "MOD_ID_TAG";
0013: 
0014: 	public const string MOD_NAME_TAG = "MOD_NAME_TAG";
0015: 
0016: 	private static readonly HashSet<string> warnedLocalizationKeys = new HashSet<string>();
0017: 
0018: 	public const string DEBUG_CATEGORY_STORE = "Store";
0019: 
0020: 	public const string DEBUG_CATEGORY_COMMAND = "Command";
0021: 
0022: 	public const string DEBUG_CATEGORY_SPAWN = "Spawn";
0023: 
0024: 	public const string DEBUG_CATEGORY_SPECIAL = "Special";
0025: 
~~~

~~~csharp
0160: 		return AudioManager.Instance.Play2(GetSoundIdentifier(modId, soundKey));
0161: 	}
0162: 
0163: 	public static bool PlaySound3(string modId, string soundKey)
0164: 	{
0165: 		return AudioManager.Instance.Play3(GetSoundIdentifier(modId, soundKey));
0166: 	}
0167: 
0168: 	public static void SetSource(GameItem item, string modId, string modName)
0169: 	{
0170: 		item.ModifyTag("MOD_ID_TAG", delegate(TagState state)
0171: 		{
0172: 			state.SetEnabled(enabled: true).SetString(modId);
0173: 		});
0174: 		item.ModifyTag("MOD_NAME_TAG", delegate(TagState state)
0175: 		{
0176: 			state.SetEnabled(enabled: true).SetString(modName);
0177: 		});
0178: 	}
0179: 
0180: 	public static void SetClientSource(StoreClient client, string modId)
0181: 	{
~~~

~~~csharp
0164: 	{
0165: 		return AudioManager.Instance.Play3(GetSoundIdentifier(modId, soundKey));
0166: 	}
0167: 
0168: 	public static void SetSource(GameItem item, string modId, string modName)
0169: 	{
0170: 		item.ModifyTag("MOD_ID_TAG", delegate(TagState state)
0171: 		{
0172: 			state.SetEnabled(enabled: true).SetString(modId);
0173: 		});
0174: 		item.ModifyTag("MOD_NAME_TAG", delegate(TagState state)
0175: 		{
0176: 			state.SetEnabled(enabled: true).SetString(modName);
0177: 		});
0178: 	}
0179: 
0180: 	public static void SetClientSource(StoreClient client, string modId)
0181: 	{
0182: 		if (client != null)
0183: 		{
0184: 			client.sourceModId = modId ?? "";
0185: 		}
~~~
