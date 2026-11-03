using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: InternalsVisibleTo("BEFormalRemainingAudit")]
[assembly: InternalsVisibleTo("BEFormalIdentityAudit")]
[assembly: TargetFramework(".NETStandard,Version=v2.1", FrameworkDisplayName = ".NET Standard 2.1")]
[assembly: AssemblyCompany("BrewingExpansion")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("3.6.7.6")]
[assembly: AssemblyInformationalVersion("3.6.7.6+a9c472546ed4ebbeda21355a103de4613089d57a")]
[assembly: AssemblyProduct("BrewingExpansion")]
[assembly: AssemblyTitle("BrewingExpansion")]
[assembly: AssemblyVersion("3.6.7.6")]
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
namespace BrewingExpansion.Formal
{
	[HarmonyPatch(typeof(ResponseList), "CreateAfterhoursResponse")]
	internal static class BackpackChoiceArt
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo native = AccessTools.Method(typeof(GameItem), "SetSpriteAndShape", (Type[])null, (Type[])null);
			MethodInfo set = AccessTools.Method(typeof(BackpackChoiceArt), "SetProxyArt", (Type[])null, (Type[])null);
			bool found = false;
			foreach (CodeInstruction instruction in instructions)
			{
				if (CodeInstructionExtensions.Calls(instruction, native))
				{
					instruction.opcode = OpCodes.Call;
					instruction.operand = set;
					found = true;
				}
				yield return instruction;
			}
			if (!found)
			{
				Debug.LogWarning((object)"[BrewingExpansion.Formal] Native backpack-choice sprite call changed; artwork adapter was not applied.");
			}
		}

		internal static GameItem SetProxyArt(GameItem proxy, string atlas, string key)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if (ModEntry.Enabled && atlas == "brewing_expansion")
			{
				RemainingItems.Definition[] all = RemainingItems.All;
				foreach (RemainingItems.Definition definition in all)
				{
					if (definition.Kind == "Bag" && key == definition.Sprite)
					{
						proxy.SetSpriteAndShapeFromMod("brewing_expansion", key);
						proxy.SetShape(new GridShapeBuilder(definition.Width, definition.Height).SetDataFill(definition.Width, definition.Height, (byte)1).Build());
						GameItemElement val = (GameItemElement)(object)((proxy is GameItemElement) ? proxy : null);
						object obj;
						if (val == null)
						{
							obj = null;
						}
						else
						{
							TreeNodeRender background = val.background;
							obj = ((background != null) ? background.image : null);
						}
						if ((Object)obj != (Object)null)
						{
							val.background.image.alphaHitTestMinimumThreshold = 0f;
						}
						object obj2;
						if (val == null)
						{
							obj2 = null;
						}
						else
						{
							TreeNodeRender fakeBackground = val.fakeBackground;
							obj2 = ((fakeBackground != null) ? fakeBackground.image : null);
						}
						if ((Object)obj2 != (Object)null)
						{
							val.fakeBackground.image.alphaHitTestMinimumThreshold = 0f;
						}
						return proxy;
					}
				}
			}
			return proxy.SetSpriteAndShape(atlas, key);
		}
	}
	public static class BarrelWine
	{
		public sealed class Definition
		{
			public readonly string Id;

			public readonly int Price;

			public Definition(string id, int price)
			{
				Id = id;
				Price = price;
			}
		}

		public static readonly Definition[] All = new Definition[57]
		{
			new Definition("be_chili_wine", 115),
			new Definition("be_rice_wine", 115),
			new Definition("be_wheat_wine", 115),
			new Definition("be_wormwood_wine", 115),
			new Definition("be_sichuan_pepper_wine", 115),
			new Definition("be_lemon_wine", 115),
			new Definition("be_yellow_grape_wine", 115),
			new Definition("be_sugarcane_wine", 115),
			new Definition("be_cucumber_wine", 115),
			new Definition("be_lychee_wine", 220),
			new Definition("be_jasmine_wine", 220),
			new Definition("be_green_grape_wine", 220),
			new Definition("be_osmanthus_wine", 520),
			new Definition("be_bayberry_wine", 520),
			new Definition("be_void_plum_wine", 2000),
			new Definition("be_stardust_berry_wine", 2000),
			new Definition("be_gusui_wine", 115),
			new Definition("be_chimadu_wine", 115),
			new Definition("be_suantian_banzhan_wine", 115),
			new Definition("be_bipo_liuguang_wine", 115),
			new Definition("be_banzhan_suanku_wine", 115),
			new Definition("be_daopo_shuanggan_wine", 115),
			new Definition("be_maigan_niang_wine", 115),
			new Definition("be_lychee_rice_wine", 180),
			new Definition("be_lychee_wormwood_wine", 180),
			new Definition("be_lychee_chili_wine", 180),
			new Definition("be_moxiang_miniang_wine", 180),
			new Definition("be_moning_qingxiao_wine", 180),
			new Definition("be_huazhe_liunian_wine", 180),
			new Definition("be_bimai_liujin_wine", 180),
			new Definition("be_cuiyu_qingzhu_wine", 180),
			new Definition("be_zilu_jinfeng_wine", 180),
			new Definition("be_moli_qingti_wine", 220),
			new Definition("be_guixin_chisu_wine", 350),
			new Definition("be_jianghen_chenzhe_wine", 350),
			new Definition("be_zhuolei_xiandan_wine", 350),
			new Definition("be_cuoji_shu_wine", 1314),
			new Definition("be_chenyuan_miniang_wine", 700),
			new Definition("be_konggu_kuai_wine", 700),
			new Definition("be_xingluo_maitian_wine", 700),
			new Definition("be_lengxing_ruzhan_wine", 700),
			new Definition("be_yexiang_chenji_wine", 800),
			new Definition("be_xingzhui_qingteng_wine", 800),
			new Definition("be_xingchen_wange_wine", 3500),
			new Definition("be_misc_wine", 80),
			new Definition("be_yidiao_wine", 115),
			new Definition("be_cuofu_wine", 220),
			new Definition("be_yujin_wine", 650),
			new Definition("be_midautumn_osmanthus_wine", 815),
			new Definition("be_nian_heshi_wine", 250),
			new Definition("be_qingxi_liuchun_wine", 180),
			new Definition("be_wujian_liubai_wine", 180),
			new Definition("be_jiangyuan_xiangfeng_wine", 180),
			new Definition("be_qingti_weifeng_wine", 180),
			new Definition("be_mobai_qinglu_wine", 180),
			new Definition("be_mijin_wanzhao_wine", 115),
			new Definition("be_jieban_yuxi_wine", 180)
		};

		internal const string Prefix = "BE_FORMAL_WINE_";

		private static readonly Dictionary<string, Definition> Definitions = All.ToDictionary<Definition, string>((Definition d) => d.Id, StringComparer.Ordinal);

		public static Definition Find(string id)
		{
			if (id == null || !Definitions.TryGetValue(id, out var value))
			{
				return null;
			}
			return value;
		}

		public static string Recipe(GameItem bottle)
		{
			return ItemIdentity.Recipe((bottle != null) ? bottle.state : null, (bottle != null) ? bottle.identifier : null);
		}

		public static void Mark(GameItem bottle, string id)
		{
			id = ItemIdentity.Canonical(id);
			if (bottle != null && Find(id) != null)
			{
				string[] array = bottle.state.dict.Keys.Where((string k) => k.StartsWith("BE_FORMAL_WINE_", StringComparison.Ordinal) && k != "BE_FORMAL_WINE_" + id).ToArray();
				foreach (string text in array)
				{
					bottle.DisableTag(text, false);
				}
				bottle.EnableTag("BE_FORMAL_WINE_" + id, false);
				bottle.identifier = id;
				ModHelper.SetSource(bottle, "brewing_expansion", ModText.Get("be_mod_name"));
				BarrelWineLoad.Attach(bottle);
			}
		}

		public static GameItem Create(Definition definition)
		{
			GameItem obj = WineHelper.CreateFinishedWineForDebug(2, false);
			Mark(obj, definition.Id);
			Refresh(obj);
			return obj;
		}

		public static string Resolve(string a, string b)
		{
			if (a == b)
			{
				switch (a)
				{
				case "be_chili_fruit":
					return "be_chili_wine";
				case "be_rice_fruit":
					return "be_rice_wine";
				case "be_wheat_fruit":
					return "be_wheat_wine";
				case "be_wormwood_fruit":
					return "be_wormwood_wine";
				case "be_sichuan_pepper_fruit":
					return "be_sichuan_pepper_wine";
				case "be_lemon_fruit":
					return "be_lemon_wine";
				case "be_yellow_grape_fruit":
					return "be_yellow_grape_wine";
				case "be_sugarcane_fruit":
					return "be_sugarcane_wine";
				case "be_cucumber_fruit":
					return "be_cucumber_wine";
				case "be_lychee_fruit":
					return "be_lychee_wine";
				case "be_jasmine_fruit":
					return "be_jasmine_wine";
				case "be_green_grape_fruit":
					return "be_green_grape_wine";
				case "be_osmanthus_fruit":
					return "be_osmanthus_wine";
				case "be_bayberry_fruit":
					return "be_bayberry_wine";
				case "be_void_plum_fruit":
					return "be_void_plum_wine";
				case "be_stardust_berry_fruit":
					return "be_stardust_berry_wine";
				case "be_midautumn_moon_osmanthus":
					return "be_midautumn_osmanthus_wine";
				case "be_jeanne_moreau_fruit":
					return "be_jieban_yuxi_wine";
				}
			}
			if ((a == "be_wheat_fruit" && b == "be_rice_fruit") || (a == "be_rice_fruit" && b == "be_wheat_fruit"))
			{
				return "be_gusui_wine";
			}
			if ((a == "be_chili_fruit" && b == "be_sichuan_pepper_fruit") || (a == "be_sichuan_pepper_fruit" && b == "be_chili_fruit"))
			{
				return "be_chimadu_wine";
			}
			if ((a == "be_lemon_fruit" && b == "be_sugarcane_fruit") || (a == "be_sugarcane_fruit" && b == "be_lemon_fruit"))
			{
				return "be_suantian_banzhan_wine";
			}
			if ((a == "be_yellow_grape_fruit" && b == "be_cucumber_fruit") || (a == "be_cucumber_fruit" && b == "be_yellow_grape_fruit"))
			{
				return "be_bipo_liuguang_wine";
			}
			if ((a == "be_wormwood_fruit" && b == "be_lemon_fruit") || (a == "be_lemon_fruit" && b == "be_wormwood_fruit"))
			{
				return "be_banzhan_suanku_wine";
			}
			if ((a == "be_rice_fruit" && b == "be_yellow_grape_fruit") || (a == "be_yellow_grape_fruit" && b == "be_rice_fruit"))
			{
				return "be_daopo_shuanggan_wine";
			}
			if ((a == "be_wheat_fruit" && b == "be_sugarcane_fruit") || (a == "be_sugarcane_fruit" && b == "be_wheat_fruit"))
			{
				return "be_maigan_niang_wine";
			}
			if ((a == "be_lychee_fruit" && b == "be_rice_fruit") || (a == "be_rice_fruit" && b == "be_lychee_fruit"))
			{
				return "be_lychee_rice_wine";
			}
			if ((a == "be_lychee_fruit" && b == "be_wormwood_fruit") || (a == "be_wormwood_fruit" && b == "be_lychee_fruit"))
			{
				return "be_lychee_wormwood_wine";
			}
			if ((a == "be_lychee_fruit" && b == "be_chili_fruit") || (a == "be_chili_fruit" && b == "be_lychee_fruit"))
			{
				return "be_lychee_chili_wine";
			}
			if ((a == "be_jasmine_fruit" && b == "be_rice_fruit") || (a == "be_rice_fruit" && b == "be_jasmine_fruit"))
			{
				return "be_moxiang_miniang_wine";
			}
			if ((a == "be_jasmine_fruit" && b == "be_lemon_fruit") || (a == "be_lemon_fruit" && b == "be_jasmine_fruit"))
			{
				return "be_moning_qingxiao_wine";
			}
			if ((a == "be_jasmine_fruit" && b == "be_sugarcane_fruit") || (a == "be_sugarcane_fruit" && b == "be_jasmine_fruit"))
			{
				return "be_huazhe_liunian_wine";
			}
			if ((a == "be_green_grape_fruit" && b == "be_wheat_fruit") || (a == "be_wheat_fruit" && b == "be_green_grape_fruit"))
			{
				return "be_bimai_liujin_wine";
			}
			if ((a == "be_green_grape_fruit" && b == "be_cucumber_fruit") || (a == "be_cucumber_fruit" && b == "be_green_grape_fruit"))
			{
				return "be_cuiyu_qingzhu_wine";
			}
			if ((a == "be_green_grape_fruit" && b == "be_yellow_grape_fruit") || (a == "be_yellow_grape_fruit" && b == "be_green_grape_fruit"))
			{
				return "be_zilu_jinfeng_wine";
			}
			if ((a == "be_green_grape_fruit" && b == "be_wormwood_fruit") || (a == "be_wormwood_fruit" && b == "be_green_grape_fruit"))
			{
				return "be_qingxi_liuchun_wine";
			}
			if ((a == "be_jasmine_fruit" && b == "be_wheat_fruit") || (a == "be_wheat_fruit" && b == "be_jasmine_fruit"))
			{
				return "be_wujian_liubai_wine";
			}
			if ((a == "be_lychee_fruit" && b == "be_sugarcane_fruit") || (a == "be_sugarcane_fruit" && b == "be_lychee_fruit"))
			{
				return "be_jiangyuan_xiangfeng_wine";
			}
			if ((a == "be_green_grape_fruit" && b == "be_lemon_fruit") || (a == "be_lemon_fruit" && b == "be_green_grape_fruit"))
			{
				return "be_qingti_weifeng_wine";
			}
			if ((a == "be_jasmine_fruit" && b == "be_cucumber_fruit") || (a == "be_cucumber_fruit" && b == "be_jasmine_fruit"))
			{
				return "be_mobai_qinglu_wine";
			}
			if ((a == "be_yellow_grape_fruit" && b == "be_sugarcane_fruit") || (a == "be_sugarcane_fruit" && b == "be_yellow_grape_fruit"))
			{
				return "be_mijin_wanzhao_wine";
			}
			if ((a == "be_jasmine_fruit" && b == "be_green_grape_fruit") || (a == "be_green_grape_fruit" && b == "be_jasmine_fruit"))
			{
				return "be_moli_qingti_wine";
			}
			if ((a == "be_osmanthus_fruit" && b == "be_lemon_fruit") || (a == "be_lemon_fruit" && b == "be_osmanthus_fruit"))
			{
				return "be_guixin_chisu_wine";
			}
			if ((a == "be_bayberry_fruit" && b == "be_sugarcane_fruit") || (a == "be_sugarcane_fruit" && b == "be_bayberry_fruit"))
			{
				return "be_jianghen_chenzhe_wine";
			}
			if ((a == "be_bayberry_fruit" && b == "be_chili_fruit") || (a == "be_chili_fruit" && b == "be_bayberry_fruit"))
			{
				return "be_zhuolei_xiandan_wine";
			}
			if ((a == "be_osmanthus_fruit" && b == "be_bayberry_fruit") || (a == "be_bayberry_fruit" && b == "be_osmanthus_fruit"))
			{
				return "be_cuoji_shu_wine";
			}
			if ((a == "be_void_plum_fruit" && b == "be_rice_fruit") || (a == "be_rice_fruit" && b == "be_void_plum_fruit"))
			{
				return "be_chenyuan_miniang_wine";
			}
			if ((a == "be_void_plum_fruit" && b == "be_wormwood_fruit") || (a == "be_wormwood_fruit" && b == "be_void_plum_fruit"))
			{
				return "be_konggu_kuai_wine";
			}
			if ((a == "be_stardust_berry_fruit" && b == "be_wheat_fruit") || (a == "be_wheat_fruit" && b == "be_stardust_berry_fruit"))
			{
				return "be_xingluo_maitian_wine";
			}
			if ((a == "be_stardust_berry_fruit" && b == "be_lemon_fruit") || (a == "be_lemon_fruit" && b == "be_stardust_berry_fruit"))
			{
				return "be_lengxing_ruzhan_wine";
			}
			if ((a == "be_void_plum_fruit" && b == "be_jasmine_fruit") || (a == "be_jasmine_fruit" && b == "be_void_plum_fruit"))
			{
				return "be_yexiang_chenji_wine";
			}
			if ((a == "be_stardust_berry_fruit" && b == "be_green_grape_fruit") || (a == "be_green_grape_fruit" && b == "be_stardust_berry_fruit"))
			{
				return "be_xingzhui_qingteng_wine";
			}
			if ((a == "be_void_plum_fruit" && b == "be_stardust_berry_fruit") || (a == "be_stardust_berry_fruit" && b == "be_void_plum_fruit"))
			{
				return "be_xingchen_wange_wine";
			}
			if (a == "wine_berry" && b == "wine_berry")
			{
				return null;
			}
			int num = Math.Max(BrewingFruits.Find(a)?.Price ?? 0, BrewingFruits.Find(b)?.Price ?? 0);
			if (num < 850)
			{
				if (num < 225)
				{
					if (num < 60)
					{
						return "be_misc_wine";
					}
					return "be_yidiao_wine";
				}
				return "be_cuofu_wine";
			}
			return "be_yujin_wine";
		}

		public static void Refresh(GameItem item)
		{
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			string id = Recipe(item);
			if (id == null)
			{
				return;
			}
			if (!WineHelper.IsFinishedWine(item))
			{
				ItemHitArea.Apply(item);
				WineChemistry.EnsureLoaded(item);
				return;
			}
			Definition definition = Array.Find(All, (Definition x) => x.Id == id);
			item.SetName(ModHelper.GetLocalized("brewing_expansion", id + "_name", Array.Empty<object>()));
			item.shortDescription = ModHelper.GetLocalized("brewing_expansion", id + "_description", Array.Empty<object>());
			GridShape modifiedShape = item.modifiedShape;
			GridShape val = ((modifiedShape != null) ? modifiedShape.Clone() : null);
			item.identifier = id;
			item.useSpriteFromMod = true;
			item.SetSpriteAndShapeFromMod("brewing_expansion", id);
			item.SetShape(new GridShapeBuilder(1, 4).SetDataFill(1, 4, (byte)1).Build());
			if (val != null && val.width == 1 && val.height == 4)
			{
				item.modifiedShape = val;
			}
			((GameItemFunc)item).getItemSpriteModifierForItemFunc = null;
			if (AgableHelper.GetAge(item) == 0)
			{
				item.unitBaseValue = definition.Price;
				item.unitValue = definition.Price;
			}
			item.SetGameItemType("ALCOHOL");
			item.SetGameItemType("SUBSTANCE");
			item.SetGameItemType("LUXURY_ITEM");
			ItemHitArea.Apply(item);
			WineChemistry.EnsureLoaded(item);
		}
	}
	[HarmonyPatch(typeof(WineHelper), "FinishFermentation")]
	internal static class BarrelWineFinish
	{
		private static void Postfix(GameItem __0)
		{
			try
			{
				if (BarrelWine.Recipe(__0) != null)
				{
					BarrelWine.Refresh(__0);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Wine metadata refresh deferred: " + ex.Message));
			}
		}
	}
	internal static class BarrelWineLoad
	{
		private sealed class Binding
		{
			internal Action<GameItem> Handler;
		}

		private static readonly ConditionalWeakTable<GameItem, Binding> Bindings = new ConditionalWeakTable<GameItem, Binding>();

		internal static void Attach(GameItem item)
		{
			if (Bindings.TryGetValue(item, out var value))
			{
				Action<GameItem> onLoaded = ((GameItemFunc)item).onLoaded;
				if (onLoaded != null && onLoaded.GetInvocationList().Contains(value.Handler))
				{
					return;
				}
			}
			Action<GameItem> native = ((GameItemFunc)item).onLoaded;
			Action<GameItem> handler = (((GameItemFunc)item).onLoaded = delegate(GameItem b)
			{
				if (BarrelWine.Recipe(b) == null)
				{
					native?.Invoke(b);
					WineChemistry.EnsureLoaded(b);
					return;
				}
				GridShape modifiedShape = b.modifiedShape;
				GridShape val = ((modifiedShape != null) ? modifiedShape.Clone() : null);
				b.useSpriteFromMod = false;
				try
				{
					native?.Invoke(b);
				}
				finally
				{
					try
					{
						BarrelWine.Refresh(b);
						if (val != null && val.width == 1 && val.height == 4)
						{
							b.modifiedShape = val;
						}
					}
					catch (Exception ex)
					{
						Debug.LogWarning((object)("[BrewingExpansion.Formal] Wine load artwork deferred: " + ex.Message));
					}
				}
			});
			Bindings.Remove(item);
			Bindings.Add(item, new Binding
			{
				Handler = handler
			});
		}
	}
	[HarmonyPatch(typeof(WineHelper), "ResetToEmptyBottle")]
	internal static class BarrelWineReset
	{
		internal sealed class State
		{
			internal readonly bool UseMod;

			internal readonly string Atlas;

			internal readonly string Sprite;

			internal State(GameItem item)
			{
				UseMod = item.useSpriteFromMod;
				Atlas = item.spriteAtlasPath;
				Sprite = item.spritePath;
			}
		}

		private static void Prefix(GameItem __0, out State __state)
		{
			__state = ((ModEntry.Enabled && WineChemistry.IsExpansionWine(__0) && !RemainingItems.IsCollector(__0)) ? new State(__0) : null);
			if (__state != null)
			{
				__0.useSpriteFromMod = false;
			}
		}

		private static void RestoreUnchangedSprite(GameItem item, State state)
		{
			if (state != null && item.spriteAtlasPath == state.Atlas && item.spritePath == state.Sprite)
			{
				item.useSpriteFromMod = state.UseMod;
			}
		}

		private static void Postfix(GameItem __0, State __state, bool __runOriginal)
		{
			if (__state == null)
			{
				return;
			}
			if (!__runOriginal || __0.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() != 0)
			{
				RestoreUnchangedSprite(__0, __state);
				return;
			}
			WineChemistry.Reset(__0);
			string[] array = __0.state.dict.Keys.Where((string k) => k.StartsWith("BE_FORMAL_WINE_", StringComparison.Ordinal)).ToArray();
			foreach (string text in array)
			{
				__0.DisableTag(text, false);
			}
			__0.identifier = "wine_bottle";
			__0.DisableTag("MOD_ID_TAG", false);
			__0.DisableTag("MOD_NAME_TAG", false);
			__0.useSpriteFromMod = false;
		}

		private static Exception Finalizer(GameItem __0, State __state, Exception __exception)
		{
			if (__exception != null)
			{
				RestoreUnchangedSprite(__0, __state);
			}
			return __exception;
		}
	}
	[HarmonyPatch(typeof(GameItem), "SetSprite", new Type[]
	{
		typeof(string),
		typeof(string)
	})]
	internal static class RestoredWineSpriteSource
	{
		private static void Prefix(GameItem __instance, string __0, string __1)
		{
			if (ModEntry.Enabled && BarrelWine.Recipe(__instance) != null)
			{
				if (__0 == "brewing_expansion")
				{
					__instance.useSpriteFromMod = true;
				}
				else if (__0 == "Items/items_winemaking" && (__1 == "wine_bottle" || __1 == "wine_fermenting"))
				{
					__instance.useSpriteFromMod = false;
				}
			}
		}
	}
	[HarmonyPatch]
	internal static class BatteryChargerPreview
	{
		private static readonly HashSet<string> Keys = new HashSet<string>(RemainingItems.All.Where((RemainingItems.Definition d) => d.Kind == "Battery").SelectMany((RemainingItems.Definition d) => (from n in Enumerable.Range(0, 5)
			select d.Sprite + "_" + n).Concat(new string[1] { d.Sprite })), StringComparer.Ordinal);

		private static MethodBase TargetMethod()
		{
			return AccessTools.Method("MachineRecharger+<>c__DisplayClass1_0:<Recharger>b__4", (Type[])null, (Type[])null);
		}

		private static bool Prepare()
		{
			MethodInfo methodInfo = TargetMethod() as MethodInfo;
			int num;
			if (methodInfo != null)
			{
				num = ((methodInfo.ReturnType == typeof(List<ItemSpriteModifier>)) ? 1 : 0);
				if (num != 0)
				{
					goto IL_0039;
				}
			}
			else
			{
				num = 0;
			}
			Debug.LogWarning((object)"[BrewingExpansion.Formal] Native charger preview changed; adapter was not applied.");
			goto IL_0039;
			IL_0039:
			return (byte)num != 0;
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo native = AccessTools.Method(typeof(RenderHandler), "LoadFromAtlas", (Type[])null, (Type[])null);
			MethodInfo resolve = AccessTools.Method(typeof(BatteryChargerPreview), "Resolve", (Type[])null, (Type[])null);
			foreach (CodeInstruction instruction in instructions)
			{
				if (CodeInstructionExtensions.Calls(instruction, native))
				{
					instruction.operand = resolve;
				}
				yield return instruction;
			}
		}

		internal static Sprite Resolve(string atlas, string key)
		{
			if (ModEntry.Enabled && atlas == "brewing_expansion" && key != null && Keys.Contains(key) && (Object)(object)CustomUIManager.Instance != (Object)null)
			{
				Sprite modSprite = CustomUIManager.Instance.GetModSprite("brewing_expansion", key);
				if ((Object)(object)modSprite != (Object)null)
				{
					return modSprite;
				}
			}
			return RenderHandler.LoadFromAtlas(atlas, key);
		}
	}
	public static class BrewingBarrel
	{
		private sealed class View
		{
			internal MachineryText Status;

			internal MachineryText Water;

			internal MachineryButton Print;
		}

		public const string Id = "be_large_brewing_barrel";

		private static readonly ConditionalWeakTable<GameItem, View> Views = new ConditionalWeakTable<GameItem, View>();

		private static string Text(string key, params object[] args)
		{
			return ModHelper.GetLocalized("brewing_expansion", key, args);
		}

		private static bool Live(GameItem item)
		{
			if (item != null)
			{
				GameItem obj = ((item is GameItemElement) ? item : null);
				if (obj == null)
				{
					return true;
				}
				return !((GameItemElement)obj).IsDestroyed();
			}
			return false;
		}

		private static bool Owned(GameItem item)
		{
			if (Live(item))
			{
				return GeneralHelper.IsItemOwned(item);
			}
			return false;
		}

		public static GameGridInventory Input(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 0);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Bottles(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 1);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Yeast(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 2);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Output(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 3);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		public static bool EmptyBottle(GameItem item)
		{
			if (item != null && item.identifier == "wine_bottle" && !WineHelper.IsFermentingOrFinished(item) && WaterHelper.GetTotalVolume(item) == 0)
			{
				return item.FindAllChildItems().Count == 0;
			}
			return false;
		}

		public static GameItem Create()
		{
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Expected O, but got Unknown
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected O, but got Unknown
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Expected O, but got Unknown
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Expected O, but got Unknown
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			GameItem item = DirectoryMaster.Item("water_jug", false);
			item.identifier = "be_large_brewing_barrel";
			WaterHelper.EmptyContainer(item);
			item.ModifyTag("LIQUID_CONTAINER_CAPACITY", (Action<TagState>)delegate(TagState t)
			{
				t.Enable().SetInt(3000000);
			}, false);
			item.ModifyTag("LIQUID_CONTAINER_BASE_PRICE_TAG", (Action<TagState>)delegate(TagState t)
			{
				t.Enable().SetInt(1400);
			}, false);
			item.RemoveAllGameItemType();
			item.SetGameItemType("MACHINE");
			item.SetValue(1400L);
			item.EnableTag("CONTAINER_TAG", false);
			((GameItemFunc)item).getItemSpriteModifierForItemFunc = null;
			item.SetSpriteAndShapeFromMod("brewing_expansion", "be_large_brewing_barrel_idle");
			item.SetShape(new GridShapeBuilder(3, 3).SetDataFill(3, 3, (byte)1).Build());
			GameGridInventory val = new GameGridInventory(4, 2)
			{
				identifier = "BE_BARREL_PULP"
			};
			GameGridInventory val2 = new GameGridInventory(4, 4)
			{
				identifier = "BE_BARREL_BOTTLES"
			};
			GameGridInventory val3 = new GameGridInventory(4, 2)
			{
				identifier = "BE_BARREL_YEAST"
			};
			GameGridInventory val4 = new GameGridInventory(4, 4)
			{
				identifier = "BE_BARREL_OUTPUT"
			};
			((GameInventoryFunc)val).mayInventoryAddItemFunc = (GameItem x, GameInventory _) => Owned(x) && x.identifier == "be_brewing_fruit_pulp" && FruitPulp.Remaining(x) > 0;
			((GameInventoryFunc)val2).mayInventoryAddItemFunc = (GameItem x, GameInventory _) => Owned(x) && EmptyBottle(x);
			((GameInventoryFunc)val3).mayInventoryAddItemFunc = (GameItem x, GameInventory _) => Owned(x) && x.IsTag("WINE_YEAST_TAG") && x.unitCount == 1;
			((GameInventoryFunc)val4).mayInventoryAddItemFunc = (GameItem x, GameInventory _) => Owned(x) && WineHelper.IsFinishedWine(x);
			View view = new View
			{
				Status = MachineryText.Status("status", Text("barrel_wait")),
				Water = MachineryText.Status("water", "")
			};
			view.Print = new MachineryButton(Text("wine_printer_print_linked"), (Action)delegate
			{
				PrintLinked(item);
			}, -1, -1).SetSize(190, 24);
			MachineryStack val5 = MachineryStack.Vertical((PixelElement[])(object)new PixelElement[4]
			{
				(PixelElement)view.Print,
				(PixelElement)new MachineryButton(Text("barrel_start"), (Action)delegate
				{
					Start(item);
				}, -1, -1).SetSize(180, 24),
				(PixelElement)view.Status,
				(PixelElement)view.Water
			});
			item.SetContentWindow(MachineHelper.CreateMachineryWindow(true, (MachineryPanel[])(object)new MachineryPanel[5]
			{
				MachineHelper.Panel(Text("barrel_pulp"), (PixelElement)(object)val, "input", true),
				MachineHelper.Panel(Text("barrel_bottles"), (PixelElement)(object)val2, "bottles", true),
				MachineHelper.Panel(Text("barrel_yeast"), (PixelElement)(object)val3, "yeast", true),
				MachineHelper.Panel(Text("barrel_output"), (PixelElement)(object)val4, "output", true),
				MachineHelper.Panel("", (PixelElement)(object)val5, "control", true)
			}));
			((GameInventoryFunc)val).onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(((GameInventoryFunc)val).onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
			{
				MachineConnections.Request();
			});
			((GameInventoryFunc)val2).onSlotAddItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(((GameInventoryFunc)val2).onSlotAddItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
			{
				RefreshPrintButton(item);
			});
			((GameInventoryFunc)val2).onSlotRemoveItemEndFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(((GameInventoryFunc)val2).onSlotRemoveItemEndFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
			{
				RefreshPrintButton(item);
			});
			Views.Add(item, view);
			GameItem obj = item;
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Loaded));
			Loaded(item);
			return item;
		}

		private static void Loaded(GameItem item)
		{
			item.SetName(MachineConnections.BarrelName(item, Text("barrel_name")));
			string shortDescription = (item.longDescription = Text("barrel_description"));
			item.shortDescription = shortDescription;
			item.contentWindow.SetTitle(item.name);
			Refresh(item);
			MachineConnections.Request();
		}

		public static void Refresh(GameItem item)
		{
			if (Live(item) && !(item.identifier != "be_large_brewing_barrel"))
			{
				GameGridInventory obj = Bottles(item);
				bool flag = obj != null && ((IEnumerable<GameItem>)((GameInventory)obj).childItems).Any((Func<GameItem, bool>)WineHelper.IsFermentingOrFinished);
				string text = (flag ? "be_large_brewing_barrel_active" : "be_large_brewing_barrel_idle");
				item.useSpriteFromMod = true;
				item.SetSprite("brewing_expansion", text);
				((GameItemFunc)item).getItemSpriteModifierForItemFunc = null;
				ItemHitArea.Apply(item);
				RefreshPrintButton(item);
				if (Views.TryGetValue(item, out var value))
				{
					value.Status.SetText(Text(flag ? "barrel_active" : "barrel_wait"));
					value.Water.SetText(Text("barrel_water", WaterHelper.GetCurrentCapacityML(item)));
				}
			}
		}

		public static void RefreshPrintButton(GameItem barrel)
		{
			if (Views.TryGetValue(barrel, out var value) && value.Print != null)
			{
				if (((GameInventory)Bottles(barrel)).childItems.Count == 0 && MachineConnections.PrinterFor(barrel) != null)
				{
					((SelectableElement)value.Print).Enable();
				}
				else
				{
					((SelectableElement)value.Print).Disable();
				}
			}
		}

		public static bool PrintLinked(GameItem barrel)
		{
			GameItem val = MachineConnections.PrinterFor(barrel);
			if (val != null)
			{
				return WineBottlePrinter.Print(val, Bottles(barrel));
			}
			return false;
		}

		public static void Collect(GameItem item)
		{
			if (!Owned(item) || item.identifier != "be_large_brewing_barrel")
			{
				return;
			}
			GameItem[] array = ((GameInventory)Bottles(item)).childItems.ToArray();
			foreach (GameItem val in array)
			{
				if (WineHelper.IsFinishedWine(val))
				{
					SlotMarker val2 = Output(item).TryFindOneValidInventorySlot(val, false);
					if (val2 != null && val2.IsValid())
					{
						val2.TryAcceptOnce(-1);
					}
				}
			}
			Refresh(item);
		}

		internal static void Wake()
		{
			if (!ModEntry.Enabled || PlayerStore.Instance == null)
			{
				return;
			}
			GameItem[] array = (from x in PlayerStore.Instance.FindAllItem(true)
				where x.identifier == "be_large_brewing_barrel"
				select x).ToArray();
			foreach (GameItem item in array)
			{
				try
				{
					Collect(item);
				}
				catch (Exception ex)
				{
					Debug.LogWarning((object)("[BrewingExpansion.Formal] Barrel collection deferred: " + ex.Message));
				}
			}
		}

		private static Dictionary<string, int> Parts(GameItem item)
		{
			return Liquid.Liquids.ToDictionary((Liquid l) => l.identifier, (Liquid l) => item.GetTagReadonly(l.currentPart).GetInt());
		}

		private static bool Same(GameItem item, Dictionary<string, int> parts)
		{
			return Liquid.Liquids.All((Liquid l) => item.GetTagReadonly(l.currentPart).GetInt() == parts[l.identifier]);
		}

		public static int Start(GameItem barrel)
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance == null || !ModEntry.Enabled || !Owned(barrel) || barrel.identifier != "be_large_brewing_barrel" || !PulpOperations.CanSave(instance) || !WaterBottlePrinter.CanSave(instance) || !PulpOperations.Busy.Add(barrel))
			{
				return 0;
			}
			PulpOperations.Store = instance;
			int num = 0;
			try
			{
				Collect(barrel);
				if (((IEnumerable<GameItem>)((GameInventory)Bottles(barrel)).childItems).Any((Func<GameItem, bool>)WineHelper.IsFermentingOrFinished) || ((GameInventory)Output(barrel)).childItems.Count > 0)
				{
					return 0;
				}
				GameItem[] array = ((GameInventory)Input(barrel)).childItems.Where((GameItem p) => Owned(p) && FruitPulp.Remaining(p) > 0 && FruitPulp.FruitId(p) != null).ToArray();
				if (array.Length != 2)
				{
					return 0;
				}
				int num2 = Math.Min(FruitPulp.Remaining(array[0]), FruitPulp.Remaining(array[1]));
				GameItem[] array2 = ((GameInventory)Bottles(barrel)).childItems.Where((GameItem x) => Owned(x) && EmptyBottle(x)).Take(num2).ToArray();
				GameItem[] array3 = ((GameInventory)Yeast(barrel)).childItems.Where((GameItem x) => Owned(x) && x.unitCount == 1 && x.IsTag("WINE_YEAST_TAG")).ToArray();
				GameItem val = ((IEnumerable<GameItem>)array3).FirstOrDefault((Func<GameItem, bool>)((GameItem x) => x.IsTag("WINE_YEAST_REUSABLE_TAG")));
				if (array2.Length < num2 || (val == null && array3.Length < num2) || WaterHelper.GetTotalVolume(barrel) < num2 * 750000)
				{
					return 0;
				}
				for (int num3 = 0; num3 < num2; num3++)
				{
					if (instance != PlayerStore.Instance)
					{
						break;
					}
					if (!Owned(barrel))
					{
						break;
					}
					if (!StartOne(instance, barrel, array, array2[num3], val ?? array3[num3]))
					{
						break;
					}
					num++;
				}
				GameItem[] array4 = array;
				foreach (GameItem val2 in array4)
				{
					if (FruitPulp.Remaining(val2) == 0)
					{
						val2.Destroy();
					}
				}
				return num;
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Barrel batch interrupted: " + ex));
				return num;
			}
			finally
			{
				try
				{
					if (instance == PlayerStore.Instance)
					{
						Refresh(barrel);
					}
				}
				finally
				{
					PulpOperations.Busy.Remove(barrel);
					if (PulpOperations.Busy.Count == 0)
					{
						PulpOperations.Store = null;
					}
				}
			}
		}

		private static bool StartOne(PlayerStore store, GameItem barrel, GameItem[] pulps, GameItem bottle, GameItem yeast)
		{
			Dictionary<string, int> dictionary = Parts(barrel);
			Dictionary<string, int> dictionary2 = Parts(bottle);
			Dictionary<string, int> dictionary3 = null;
			Dictionary<string, int> parts = null;
			int[] counts = pulps.Select(FruitPulp.Remaining).ToArray();
			List<GameItem> list = new List<GameItem>();
			GameInventory parentInventory = yeast.parentInventory;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(bottle, 0);
			GameGridInventory val = (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
			if (val == null || !Owned(bottle) || !Owned(yeast))
			{
				return false;
			}
			try
			{
				for (int i = 0; i < 2; i++)
				{
					string text = FruitPulp.FruitId(pulps[i]);
					int currentUniqueId = store.currentUniqueId;
					GameItem val2 = DirectoryMaster.Item(text, true);
					if (!Current())
					{
						return false;
					}
					if (!Live(val2) || val2.identifier != text || val2.parentInventory != null || val2.uniqueId <= currentUniqueId || val2.uniqueId > store.currentUniqueId)
					{
						PulpOperations.Protect(store, "unconfirmed brewing ingredient factory result");
						return false;
					}
					list.Add(val2);
					WineHelper.SetBerryCondition(val2, 0, 0);
					SlotMarker val3 = val.TryFindOneValidInventorySlot(val2, false);
					if (val3 == null || !val3.IsValid())
					{
						return false;
					}
					val3.TryAcceptOnce(-1);
					if ((object)val2.parentInventory != val)
					{
						return false;
					}
				}
				SlotMarker val4 = val.TryFindOneValidInventorySlot(yeast, false);
				if (val4 == null || !val4.IsValid())
				{
					return false;
				}
				val4.TryAcceptOnce(-1);
				flag = (object)yeast.parentInventory == val;
				if (!flag)
				{
					return false;
				}
				try
				{
					WaterHelper.TransferLiquid(barrel, bottle);
				}
				finally
				{
					if (Current())
					{
						dictionary3 = Parts(barrel);
						parts = Parts(bottle);
					}
				}
				if (!Current() || WaterHelper.GetTotalVolume(bottle) != 750000)
				{
					return false;
				}
				Dictionary<string, int> parts2 = WineChemistry.Capture(bottle);
				string id = BarrelWine.Resolve(FruitPulp.FruitId(pulps[0]), FruitPulp.FruitId(pulps[1]));
				flag3 = true;
				int j;
				for (j = 0; j < 2; j++)
				{
					pulps[j].ModifyTag("BE_PULP_REMAINING_USES_V1", (Action<TagState>)delegate(TagState t)
					{
						t.Enable().SetInt(counts[j] - 1);
					}, false);
				}
				if (pulps.Where((GameItem p, int num) => FruitPulp.Remaining(p) != counts[num] - 1).Any())
				{
					return false;
				}
				try
				{
					WineHelper.TryStartFermentation(bottle);
				}
				finally
				{
					flag2 = Current() && WineHelper.IsFermentingOrFinished(bottle);
					if (flag2)
					{
						if (list.Any(Live) || (!yeast.IsTag("WINE_YEAST_REUSABLE_TAG") && Live(yeast)))
						{
							PulpOperations.Protect(store, "native fermentation changed state without consuming its staged inputs");
						}
						bool special = BarrelWine.Recipe(bottle) == "be_nian_heshi_wine";
						if (!special)
						{
							BarrelWine.Mark(bottle, id);
						}
						bottle.ModifyTag("FERMENTATION_TIME_REMAINING_INT", (Action<TagState>)delegate(TagState t)
						{
							t.Enable().SetInt((!special) ? 1 : 4);
						}, false);
						int score = (int)Math.Round((double)pulps.Sum((GameItem p) => p.GetTagReadonly("BE_PULP_BERRY_SCORE_SUM_INT").GetInt()) / 4.0, MidpointRounding.AwayFromZero);
						bottle.ModifyTag("WINE_SCORE_INT", (Action<TagState>)delegate(TagState t)
						{
							t.Enable().SetInt(t.GetInt() + score);
						}, false);
						WineChemistry.Retain(bottle, parts2, fromBarrel: true);
						if (Owned(yeast) && yeast.IsTag("WINE_YEAST_REUSABLE_TAG"))
						{
							SlotMarker obj = Yeast(barrel).TryFindOneValidInventorySlot(yeast, false);
							if (obj != null)
							{
								obj.TryAcceptOnce(-1);
							}
						}
					}
				}
				return flag2;
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Bottle fermentation interrupted: " + ex.Message));
				return flag2;
			}
			finally
			{
				if (!flag2 && Current())
				{
					bool flag4 = true;
					foreach (GameItem item in list)
					{
						if (Live(item) && ((object)item.parentInventory == val || item.parentInventory == null) && item.FindAllChildItems().Count == 0)
						{
							item.Destroy();
						}
						flag4 &= !Live(item) && item.parentInventory == null;
					}
					if (flag && Live(yeast) && (object)yeast.parentInventory == val)
					{
						SlotMarker obj2 = parentInventory.TryFindOneValidInventorySlot(yeast);
						if (obj2 != null)
						{
							obj2.TryAcceptOnce(-1);
						}
						flag4 &= yeast.parentInventory == parentInventory;
					}
					else if (flag && !Live(yeast))
					{
						flag4 = false;
					}
					if (dictionary3 != null && Same(barrel, dictionary3) && Same(bottle, parts))
					{
						foreach (Liquid liquid in Liquid.Liquids)
						{
							ChemicalHelper.SetChemContentUL(bottle, liquid.identifier, dictionary2[liquid.identifier]);
							ChemicalHelper.SetChemContentUL(barrel, liquid.identifier, dictionary[liquid.identifier]);
						}
						flag4 &= Same(barrel, dictionary) && Same(bottle, dictionary2);
					}
					else if (dictionary3 != null)
					{
						flag4 = false;
					}
					if (flag4 && flag3)
					{
						int i2;
						for (i2 = 0; i2 < 2; i2++)
						{
							pulps[i2].ModifyTag("BE_PULP_REMAINING_USES_V1", (Action<TagState>)delegate(TagState t)
							{
								t.Enable().SetInt(counts[i2]);
							}, false);
						}
					}
					if (!flag4)
					{
						PulpOperations.Protect(store, "barrel input/water rollback could not be confirmed");
					}
				}
			}
			bool Current()
			{
				return store == PlayerStore.Instance;
			}
		}
	}
	[HarmonyPatch(typeof(SpriteHelper), "UpdateWaterContainerSprite")]
	internal static class BarrelWaterVisual
	{
		private static bool Prefix(GameItem __0)
		{
			if (!ModEntry.Enabled || ((__0 != null) ? __0.identifier : null) != "be_large_brewing_barrel")
			{
				return true;
			}
			BrewingBarrel.Refresh(__0);
			return false;
		}
	}
	internal static class BarrelWaterAdded
	{
		private static void Postfix(GameItem __0)
		{
			if (((__0 != null) ? __0.identifier : null) == "be_large_brewing_barrel")
			{
				BrewingBarrel.Refresh(__0);
			}
		}
	}
	internal static class BarrelWaterOnly
	{
		private static bool Prefix(GameItem __0, GameItem __1)
		{
			if (!(((__1 != null) ? __1.identifier : null) != "be_large_brewing_barrel"))
			{
				if (__0 != null && __0.identifier != "wine_bottle" && !__0.IsTag("ALCOHOL"))
				{
					return WaterFeatureHelper.GetWaterFeature(__0) != null;
				}
				return false;
			}
			return true;
		}
	}
	public static class BrewingFruits
	{
		public sealed class Definition
		{
			public readonly string Id;

			public readonly int Price;

			public readonly bool Concentratable;

			public Definition(string id, int price, bool concentratable)
			{
				Id = id;
				Price = price;
				Concentratable = concentratable;
			}
		}

		public static readonly Definition[] All = new Definition[18]
		{
			new Definition("be_chili_fruit", 30, concentratable: true),
			new Definition("be_rice_fruit", 30, concentratable: true),
			new Definition("be_wheat_fruit", 30, concentratable: true),
			new Definition("be_wormwood_fruit", 30, concentratable: true),
			new Definition("be_sichuan_pepper_fruit", 30, concentratable: true),
			new Definition("be_lemon_fruit", 30, concentratable: true),
			new Definition("be_yellow_grape_fruit", 30, concentratable: true),
			new Definition("be_sugarcane_fruit", 30, concentratable: true),
			new Definition("be_cucumber_fruit", 30, concentratable: true),
			new Definition("be_lychee_fruit", 60, concentratable: true),
			new Definition("be_jasmine_fruit", 60, concentratable: true),
			new Definition("be_green_grape_fruit", 60, concentratable: true),
			new Definition("be_osmanthus_fruit", 225, concentratable: true),
			new Definition("be_bayberry_fruit", 225, concentratable: true),
			new Definition("be_void_plum_fruit", 850, concentratable: true),
			new Definition("be_stardust_berry_fruit", 850, concentratable: true),
			new Definition("be_midautumn_moon_osmanthus", 0, concentratable: false),
			new Definition("be_jeanne_moreau_fruit", 60, concentratable: false)
		};

		public static Definition Find(string id)
		{
			return Array.Find(All, (Definition d) => d.Id == id);
		}

		public static bool CanConcentrate(GameItem item)
		{
			if (item != null && item.IsTag("WINE_INGREDIENT_TAG"))
			{
				if (!(item.identifier == "wine_berry"))
				{
					return Find(item.identifier)?.Concentratable ?? false;
				}
				return true;
			}
			return false;
		}

		public static GameItem Create(Definition d)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			GameItem obj = ItemDirectory.CreateEmptyItem(d.Id);
			obj.SetValue((long)d.Price);
			obj.SetGameItemType("FOOD");
			obj.SetSpriteAndShapeFromMod("brewing_expansion", d.Id);
			obj.SetShape(new GridShapeBuilder(2, 2).SetDataFill(2, 2, (byte)1).Build());
			WineHelper.InitBerry(obj);
			WineHelper.SetBerryCondition(obj, 2, 0);
			obj.AddClientItemBuyingFeature();
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Loaded));
			Loaded(obj);
			return obj;
		}

		private static void Loaded(GameItem item)
		{
			item.SetName(ModHelper.GetLocalized("brewing_expansion", item.identifier + "_name", Array.Empty<object>()));
			string shortDescription = (item.longDescription = ModHelper.GetLocalized("brewing_expansion", item.identifier + "_description", Array.Empty<object>()));
			item.shortDescription = shortDescription;
			ItemHitArea.Apply(item);
		}
	}
	public static class BrewingSeeds
	{
		public sealed class Definition
		{
			public readonly string Id;

			public readonly string Sprite;

			public readonly string Fruit;

			public readonly int Price;

			public Definition(string id, string sprite, string fruit, int price)
			{
				Id = id;
				Sprite = sprite;
				Fruit = fruit;
				Price = price;
			}
		}

		public static readonly Definition[] All = new Definition[16]
		{
			new Definition("be_chili_seed", "be_chili_seed", "be_chili_fruit", 20),
			new Definition("be_rice_seed", "be_rice_seed", "be_rice_fruit", 20),
			new Definition("be_wheat_seed", "be_wheat_seed", "be_wheat_fruit", 20),
			new Definition("be_wormwood_seed", "be_wormwood_seed", "be_wormwood_fruit", 20),
			new Definition("be_sichuan_pepper_seed", "be_sichuan_pepper_seed", "be_sichuan_pepper_fruit", 20),
			new Definition("be_lemon_seed", "be_lemon_seed", "be_lemon_fruit", 20),
			new Definition("be_yellow_grape_seed", "be_yellow_grape_seed", "be_yellow_grape_fruit", 20),
			new Definition("be_sugarcane_seed", "be_sugarcane_seed", "be_sugarcane_fruit", 20),
			new Definition("be_cucumber_seed", "be_cucumber_seed", "be_cucumber_fruit", 20),
			new Definition("be_lychee_seed", "be_lychee_seed", "be_lychee_fruit", 45),
			new Definition("be_jasmine_seed", "be_jasmine_seed", "be_jasmine_fruit", 45),
			new Definition("be_green_grape_seed", "be_green_grape_seed", "be_green_grape_fruit", 45),
			new Definition("be_osmanthus_seed", "be_osmanthus_seed", "be_osmanthus_fruit", 200),
			new Definition("be_bayberry_seed", "be_bayberry_seed", "be_bayberry_fruit", 200),
			new Definition("be_void_plum_seed", "be_void_plum_seed", "be_void_plum_fruit", 800),
			new Definition("be_stardust_berry_seed", "be_stardust_berry_seed", "be_stardust_berry_fruit", 800)
		};

		public static Definition Find(string id)
		{
			return Array.Find(All, (Definition d) => d.Id == id);
		}

		public static GameItem Create(Definition d)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			GameItem obj = ItemDirectory.CreateEmptyItem(d.Id);
			obj.SetValue((long)d.Price);
			obj.SetGameItemType("SEED");
			obj.SetSpriteAndShapeFromMod("brewing_expansion", d.Sprite);
			obj.SetShape(new GridShapeBuilder(1, 2).SetDataFill(1, 2, (byte)1).Build());
			obj.AddClientItemBuyingFeature();
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Loaded));
			Loaded(obj);
			return obj;
		}

		private static void Loaded(GameItem item)
		{
			NormalizeCategories(item);
			item.SetName(ModHelper.GetLocalized("brewing_expansion", item.identifier + "_name", Array.Empty<object>()));
			string shortDescription = (item.longDescription = ModHelper.GetLocalized("brewing_expansion", item.identifier + "_description", Array.Empty<object>()));
			item.shortDescription = shortDescription;
			ItemHitArea.Apply(item);
		}

		private static void NormalizeCategories(GameItem item)
		{
			if (Find(item.identifier) == null)
			{
				return;
			}
			string text = item.GetTagReadonly("MOD_ID_TAG").GetString();
			if (string.IsNullOrEmpty(text) || !(text != "brewing_expansion"))
			{
				if (!item.IsGameItemType("SEED") || !item.IsTag("SEED"))
				{
					item.SetGameItemType("SEED");
				}
				if (item.IsGameItemType("FOOD") || item.IsTag("FOOD"))
				{
					item.RemoveGameItemType("FOOD");
				}
				if (item.IsGameItemType("MATERIAL") || item.IsTag("MATERIAL"))
				{
					item.RemoveGameItemType("MATERIAL");
				}
			}
		}
	}
	public static class BrewingCertificates
	{
		public static readonly string[] Ids = new string[3] { "be_basic_cert", "be_mid_cert", "be_high_cert" };

		public static int Quota()
		{
			PlayerStore instance = PlayerStore.Instance;
			List<GameItem> list = ((instance != null) ? instance.FindAllItem(true) : null);
			int num = 0;
			if (list != null)
			{
				foreach (GameItem item in list)
				{
					if (MachineConnections.Owned(item))
					{
						num = Math.Max(num, Array.IndexOf(Ids, item.identifier) + 1);
					}
				}
			}
			return (new int[4] { 2, 5, 7, 18 })[num];
		}

		public static GameItem Create(string id)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			GameItem val = ItemDirectory.CreateEmptyItem(id);
			val.SetValue((long)(new int[3] { 400, 600, 900 })[Array.IndexOf(Ids, id)]);
			val.SetGameItemType("DOCUMENT");
			val.SetSpriteAndShapeFromMod("brewing_expansion", id);
			val.SetShape(new GridShapeBuilder(3, 2).SetDataFill(3, 2, (byte)1).Build());
			((GameItemFunc)val).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)val).onLoaded, new Action<GameItem>(Loaded));
			Loaded(val);
			return val;
		}

		private static void Loaded(GameItem item)
		{
			item.SetName(ModHelper.GetLocalized("brewing_expansion", item.identifier + "_name", Array.Empty<object>()));
			string shortDescription = (item.longDescription = ModHelper.GetLocalized("brewing_expansion", item.identifier + "_description", Array.Empty<object>()));
			item.shortDescription = shortDescription;
			ItemHitArea.Apply(item);
		}
	}
	internal static class CableTooltipLayout
	{
		private static TreeNodeRender measure;

		private static readonly Dictionary<string, string> cache = new Dictionary<string, string>();

		private static int fontId;

		private static int width;

		private static float fontSize;

		private static string locale;

		internal static string Wrap(string text)
		{
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)RenderHandler.current == (Object)null || (Object)(object)UISettings.current == (Object)null)
			{
				return text;
			}
			string currentLocaleCode = LocHelper.GetCurrentLocaleCode();
			if (locale != currentLocaleCode)
			{
				Reset();
				locale = currentLocaleCode;
			}
			if ((Object)(object)measure == (Object)null)
			{
				measure = TreeNodeRender.InstantiateText();
				((Component)measure).gameObject.SetActive(false);
			}
			int num = UISettings.current.fontWidth * 60;
			if (fontId != ((Object)((TMP_Text)measure.text).font).GetInstanceID() || fontSize != ((TMP_Text)measure.text).fontSize || width != num)
			{
				cache.Clear();
				fontId = ((Object)((TMP_Text)measure.text).font).GetInstanceID();
				fontSize = ((TMP_Text)measure.text).fontSize;
				width = num;
			}
			if (cache.TryGetValue(text, out var value))
			{
				return value;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = text.Split('\n');
			foreach (string text2 in array)
			{
				int[] array2 = StringInfo.ParseCombiningCharacters(text2);
				int j = 0;
				while (j < array2.Length)
				{
					int k;
					for (k = j + 1; k < array2.Length && Fits(text2.Substring(array2[j], array2[k] - array2[j])); k++)
					{
					}
					if (k == array2.Length && Fits(text2.Substring(array2[j])))
					{
						stringBuilder.Append(text2.Substring(array2[j]));
						j = k;
						break;
					}
					int num2 = Math.Max(j + 1, k - 1);
					if (num2 >= array2.Length)
					{
						stringBuilder.Append(text2.Substring(array2[j]));
						j = array2.Length;
						break;
					}
					for (int num3 = num2 - 1; num3 > j; num3--)
					{
						if (char.IsWhiteSpace(text2[array2[num3]]) && ((TMP_Text)measure.text).GetPreferredValues(text2.Substring(array2[j], array2[num3] - array2[j])).x >= (float)width * 0.85f)
						{
							num2 = num3;
							break;
						}
					}
					stringBuilder.Append(text2.Substring(array2[j], array2[num2] - array2[j])).Append('\n');
					for (j = num2; j < array2.Length && char.IsWhiteSpace(text2[array2[j]]); j++)
					{
					}
				}
				stringBuilder.Append('\n');
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Length--;
			}
			value = stringBuilder.ToString();
			if (cache.Count >= 32)
			{
				cache.Clear();
			}
			cache[text] = value;
			return value;
		}

		private static bool Fits(string text)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return ((TMP_Text)measure.text).GetPreferredValues(text).x <= (float)(width - 1);
		}

		internal static void Reset()
		{
			if ((Object)(object)measure != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)measure).gameObject);
			}
			measure = null;
			cache.Clear();
		}
	}
	public static class CatalogueDiagnostics
	{
		public static void Run()
		{
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Expected O, but got Unknown
			if (PlayerStore.Instance == null)
			{
				throw new InvalidOperationException("请先进入正式版测试新档。");
			}
			List<string> failures = new List<string>();
			int checks = 0;
			FigurineDefinition[] all = Figurines.All;
			foreach (FigurineDefinition figurineDefinition in all)
			{
				GameItem val = null;
				GameItem val2 = null;
				try
				{
					Check(DirectoryMaster.Has<GameItem>(figurineDefinition.Id), "catalogue " + figurineDefinition.Id);
					val = DirectoryMaster.Item(figurineDefinition.Id, true);
					Check(val.identifier == figurineDefinition.Id && val.uniqueId > 0, "native ID/UID " + figurineDefinition.Id);
					Check(GeneralHelper.IsItemOwned(val), "ownership " + figurineDefinition.Id);
					Check(val.name != figurineDefinition.Id && !string.IsNullOrEmpty(val.name), "localization " + figurineDefinition.Id);
					Check(val.useSpriteFromMod, "mod sprite " + figurineDefinition.Id);
					Check(val.shape.width == 2 && val.shape.height == figurineDefinition.Height, "shape " + figurineDefinition.Id);
					int uniqueId = val.uniqueId;
					List<SaveItemNode> saveItems = JsonUtility.FromJson<SaveState>(JsonUtility.ToJson((object)new SaveState
					{
						saveItems = SaveManager.EncodeNodes((GraphNodeStorage)(object)val)
					})).saveItems;
					foreach (SaveItemNode item in saveItems)
					{
						item.OnAfterDeserialize();
					}
					val.Destroy();
					val = null;
					val2 = SaveManager.DecodeNodes(saveItems);
					Check(val2.identifier == figurineDefinition.Id && val2.uniqueId == uniqueId && GeneralHelper.IsItemOwned(val2), "native graph reconstruction " + figurineDefinition.Id);
					Check(Figurines.Bonus((IEnumerable<GameItem>)(object)new GameItem[2] { val2, val2 }) == 100, "duplicate non-stacking " + figurineDefinition.Id);
				}
				catch (Exception ex)
				{
					failures.Add(figurineDefinition.Id + ": " + ex);
				}
				finally
				{
					if (val != null)
					{
						val.Destroy();
					}
					if (val2 != null)
					{
						val2.Destroy();
					}
				}
			}
			string text = "Checks=" + checks + "; Failures=" + failures.Count + "\n" + string.Join("\n", failures);
			ModHelper.WriteModDataFileText("brewing_expansion", "foundation-check.txt", text);
			Debug.Log((object)("[BrewingExpansion.Formal] " + text));
			void Check(bool ok, string name)
			{
				checks++;
				if (!ok)
				{
					failures.Add(name);
				}
			}
		}
	}
	internal sealed class FigurineDefinition
	{
		internal readonly string Id;

		internal readonly string Zh;

		internal readonly string En;

		internal readonly string ZhDescription;

		internal readonly string EnDescription;

		internal readonly int Height;

		internal FigurineDefinition(string id, string zh, string en, string zhDescription, string enDescription, int height = 3)
		{
			Id = id;
			Zh = zh;
			En = en;
			ZhDescription = zhDescription;
			EnDescription = enDescription;
			Height = height;
		}
	}
	internal static class Figurines
	{
		internal static readonly FigurineDefinition[] All = new FigurineDefinition[4]
		{
			new FigurineDefinition("be_ichigo_figurine", "黑崎一护", "Ichigo Kurosaki", "橘发少年握紧斩魄刀，像下一秒就要跃出展示柜。守护重要之人的决心，被凝在这小小的身影里。拥有它，店铺吸引力 +100；同款不叠加。", "The orange-haired swordsman grips his blade, ready to leap from the display case. His resolve to protect those dear to him lives in this tiny figure. Adds 100 shop attractiveness; duplicates do not stack."),
			new FigurineDefinition("be_rukia_figurine", "朽木露琪亚", "Rukia Kuchiki", "黑色死霸装与白色围巾在风里扬起。她把勇气藏在平静的目光中，也把一段相遇留在了这座展示柜里。拥有它，店铺吸引力 +100；同款不叠加。", "Her black robes and white scarf catch an unseen breeze. A quiet gaze holds both courage and the memory of a fateful meeting. Adds 100 shop attractiveness; duplicates do not stack."),
			new FigurineDefinition("be_uryu_figurine", "石田雨龙", "Uryu Ishida", "眼镜后的目光沉着，灵弓的弦仿佛仍在发亮。他不必高声宣告自己的信念，只需把箭稳稳射向前方。拥有它，店铺吸引力 +100；同款不叠加。", "Behind his glasses, his gaze stays steady and the bowstring seems to glow. He lets a carefully aimed arrow speak for his convictions. Adds 100 shop attractiveness; duplicates do not stack."),
			new FigurineDefinition("be_iridium_sea_cucumber_figurine", "噢我的天啊是铱星大海参", "Oh My God, It's an Iridium Sea Cucumber", "从铱星的海底捞起一只大海参，连星露谷的渔夫都要惊呼：噢我的天啊！把它摆在店里，路过的人总会停下来看第二眼。拥有它，店铺吸引力 +100；同款不叠加。", "An iridium sea cucumber, hauled up from the depths near Stardew Valley. Even a seasoned fisher would cry, 'Oh my god!' Passersby always stop for a second look. Adds 100 shop attractiveness; duplicates do not stack.", 2)
		};

		internal static GameItem Create(FigurineDefinition definition)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			GameItem obj = ItemDirectory.CreateEmptyItem(definition.Id);
			obj.SetName(ModHelper.GetLocalized("brewing_expansion", definition.Id + "_name", Array.Empty<object>()));
			obj.SetSpriteAndShapeFromMod("brewing_expansion", definition.Id);
			obj.SetShape(new GridShapeBuilder(2, definition.Height).SetDataFill(2, definition.Height, (byte)1).Build());
			obj.SetGameItemType("LUXURY_ITEM");
			obj.SetValue(0L);
			obj.shortDescription = ModHelper.GetLocalized("brewing_expansion", definition.Id + "_description", Array.Empty<object>());
			obj.EnableTag("BE_DECORATION", false);
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(ItemHitArea.Apply));
			ItemHitArea.Apply(obj);
			return obj;
		}

		internal static int Bonus(IEnumerable<GameItem> items)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (GameItem item in items)
			{
				if (item == null || !GeneralHelper.IsItemOwned(item))
				{
					continue;
				}
				FigurineDefinition[] all = All;
				foreach (FigurineDefinition figurineDefinition in all)
				{
					if (item.identifier == figurineDefinition.Id)
					{
						hashSet.Add(figurineDefinition.Id);
						break;
					}
				}
			}
			return hashSet.Count * 100;
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "GetCurrentStoreAttractiveness")]
	internal static class FigurineAttractiveness
	{
		private static bool warned;

		[HarmonyPostfix]
		[HarmonyPriority(800)]
		private static void Postfix(PlayerStore __instance, ref int __result)
		{
			if (!ModEntry.Enabled)
			{
				return;
			}
			try
			{
				int num = Figurines.Bonus(__instance.FindAllItem(true));
				__result = (int)Math.Min(2147483647L, Math.Max(0L, (long)__result + (long)num));
			}
			catch (Exception ex)
			{
				if (!warned)
				{
					Debug.LogWarning((object)("[BrewingExpansion.Formal] Figurine bonus could not be calculated: " + ex));
				}
				warned = true;
			}
		}
	}
	internal static class FormalAssassinEnding
	{
		internal static PlayerStore Pending;

		internal static void Reset()
		{
			Pending = null;
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "ExecuteGameOver")]
	internal static class FormalAssassinCause
	{
		private static void Prefix(PlayerStore __instance, string __0, bool __runOriginal)
		{
			FormalAssassinEnding.Reset();
			if (ModEntry.Enabled && __runOriginal && __0 == "hitman")
			{
				StoreClientInstance currentClientInstance = __instance.currentClientInstance;
				if (((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.identifier) == "be_outer_ring_assassin")
				{
					FormalAssassinEnding.Pending = __instance;
				}
			}
		}

		private static void Finalizer(Exception __exception)
		{
			if (__exception != null)
			{
				FormalAssassinEnding.Reset();
			}
		}

		private static void Postfix(bool __runOriginal)
		{
			if (!__runOriginal)
			{
				FormalAssassinEnding.Reset();
			}
		}
	}
	[HarmonyPatch(typeof(GameOverUIManager), "DisplayUI")]
	internal static class FormalAssassinText
	{
		private static void Prefix(ref string givenReason, ref string givenDescription)
		{
			if (ModEntry.Enabled && FormalAssassinEnding.Pending != null && FormalAssassinEnding.Pending == PlayerStore.Instance)
			{
				givenReason = ModText.Get("be_ending_assassination_title");
				givenDescription = ModText.Get("be_ending_assassination_description");
				FormalAssassinEnding.Reset();
			}
		}
	}
	internal static class FormalEvents
	{
		private static void Run(Action action, string name)
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] " + name + " could not finish: " + ex));
			}
		}

		internal static void Loaded()
		{
			FormalAssassinEnding.Reset();
			Run(FormalVisitors.Loaded, "Visitor restoration");
			Run(FormalVisitorPersistence.Restore, "Pending visitor restoration");
		}

		internal static void Generated(StoreClientManager manager)
		{
			Run(delegate
			{
				FormalVisitors.Generated(manager);
			}, "Brewing visitor queue");
		}

		internal static void QueueDailyVisitor(PlayerStore store)
		{
			Run(delegate
			{
				FormalVisitors.EnsureFortySeven(store);
			}, "Daily Forty-Seven queue");
		}

		internal static void NewGame(string id)
		{
			Run(delegate
			{
				FormalTraits.NewGame(id);
			}, "Starting rewards");
		}

		internal static void AfterNight()
		{
			Run(FormalTraits.NightLoss, "Drunk perk night effect");
		}
	}
	[HarmonyPatch(typeof(WineHelper), "StartFermentation")]
	internal static class FormalRecipeFermentation
	{
		internal sealed class State
		{
			internal PlayerStore Store;

			internal string Recipe;

			internal bool Special;

			internal bool Advanced;

			internal Dictionary<string, int> Dose;
		}

		private static void Prefix(GameItem __0, out State __state)
		{
			__state = null;
			if (ModEntry.Enabled && __0 != null && (!(__0.identifier != "wine_bottle") || BarrelWine.Find(__0.identifier) != null) && __0.GetTagReadonly("WINE_CONTAINER_STATE").GetInt() == 0)
			{
				List<GameItem> source = __0.FindAllChildItems();
				string[] array = (from i in source
					where BrewingFruits.Find(i.identifier) != null
					select i.identifier).ToArray();
				bool flag = source.Any((GameItem i) => i.IsTag("BE_HENTAI_YEAST"));
				bool flag2 = source.Any((GameItem i) => i.identifier == "be_advanced_yeast_reactor");
				if (array.Length != 0 || flag || flag2)
				{
					__state = new State
					{
						Store = PlayerStore.Instance,
						Special = flag,
						Advanced = flag2,
						Recipe = (flag ? "be_nian_heshi_wine" : ((array.Length == 2) ? BarrelWine.Resolve(array[0], array[1]) : ((array.Length != 0) ? Misc(array) : null))),
						Dose = WineChemistry.Capture(__0)
					};
				}
			}
		}

		private static string Misc(string[] fruits)
		{
			int num = fruits.Max((string f) => BrewingFruits.Find(f)?.Price ?? 0);
			if (num < 850)
			{
				if (num < 225)
				{
					if (num < 60)
					{
						return "be_misc_wine";
					}
					return "be_yidiao_wine";
				}
				return "be_cuofu_wine";
			}
			return "be_yujin_wine";
		}

		private static void Postfix(GameItem __0, State __state, bool __runOriginal)
		{
			if (__runOriginal && __state != null && __state.Store == PlayerStore.Instance && WineHelper.IsFermentingOrFinished(__0))
			{
				if (__state.Recipe != null)
				{
					BarrelWine.Mark(__0, __state.Recipe);
				}
				if (__state.Special)
				{
					RemainingItems.Tag(__0, "FERMENTATION_TIME_REMAINING_INT", 4);
				}
				if (__state.Advanced)
				{
					ContrabandHelper.InitContrabandItem(__0, 4);
				}
				WineChemistry.Retain(__0, __state.Dose);
			}
		}
	}
	[HarmonyPatch(typeof(MachineWineRack), "CanAgeWine")]
	internal static class FormalCollectorAgeEligibility
	{
		private static void Postfix(GameItem __0, ref bool __result)
		{
			if (ModEntry.Enabled && RemainingItems.IsCollector(__0) && !__0.IsTag("WINE_COUNTERFEIT_TAG"))
			{
				__result = true;
			}
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "GetCurrentStoreAttractiveness")]
	internal static class FormalDecorationAttraction
	{
		private static void Postfix(PlayerStore __instance, ref int __result)
		{
			if (ModEntry.Enabled)
			{
				int num = (from i in __instance.FindAllItem(true)
					where GeneralHelper.IsItemOwned(i) && RemainingItems.Find(i.identifier)?.Kind == "Decor"
					select i.identifier).Distinct().Count();
				__result = (int)Math.Min(2147483647L, Math.Max(0L, __result + 100L * (long)num));
			}
		}
	}
	internal static class FormalMusic
	{
		internal static readonly string[] Names = new string[5] { "酒吧特调", "霓虹街道", "微醺之夜", "弦间微风", "酿酒有嘻哈" };

		internal static readonly string[] Keys = new string[5] { "be_cassette_bar_special", "be_cassette_neon_street", "be_cassette_tipsy_night", "be_cassette_string_breeze", "be_cassette_brewing_hiphop" };

		internal static readonly string[] Ids = Names.Select((string n) => "be_music_cassette_" + Hash(n)).ToArray();

		internal static string Hash(string name)
		{
			using SHA256 sHA = SHA256.Create();
			return BitConverter.ToString(sHA.ComputeHash(Encoding.UTF8.GetBytes((name + ".mp3").ToLowerInvariant()))).Replace("-", "").Substring(0, 16)
				.ToLowerInvariant();
		}

		internal static GameItem Create(int index)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			GameItem obj = MiscItemDirectory.MusicCassette("brewing_expansion:be_music_track_" + Hash(Names[index]), Names[index], "cassette" + (index + 1), "xuxu", 0);
			obj.identifier = Ids[index];
			obj.SetValue(100L);
			obj.SetShape(new GridShapeBuilder(2, 2).SetDataFill(2, 2, (byte)1).Build());
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Loaded));
			Loaded(obj);
			return obj;
		}

		internal static void Loaded(GameItem item)
		{
			int num = Array.IndexOf(Ids, item.identifier);
			if (num >= 0)
			{
				item.SetName(ModHelper.GetLocalized("brewing_expansion", item.identifier + "_name", Array.Empty<object>()));
				string shortDescription = (item.longDescription = ModHelper.GetLocalized("brewing_expansion", item.identifier + "_description", Array.Empty<object>()));
				item.shortDescription = shortDescription;
				item.useSpriteFromMod = true;
				item.SetSprite("brewing_expansion", Keys[num]);
				ItemHitArea.Apply(item);
			}
		}
	}
	internal static class FormalReputation
	{
		internal sealed class GainScope : IDisposable
		{
			private readonly GainScope previous;

			private readonly PlayerStore store;

			internal double Remaining;

			internal GainScope()
			{
				previous = Current;
				store = PlayerStore.Instance;
				Remaining = previous?.Remaining ?? 10.0;
				Current = this;
			}

			internal double Take(double value)
			{
				if (store != PlayerStore.Instance || double.IsNaN(value) || double.IsInfinity(value))
				{
					return 0.0;
				}
				double num = Math.Min(Math.Max(0.0, value), Remaining);
				Remaining -= num;
				return num;
			}

			public void Dispose()
			{
				if (Current == this)
				{
					if (previous != null)
					{
						previous.Remaining = Remaining;
					}
					Current = previous;
				}
			}
		}

		internal sealed class ChangeScope : IDisposable
		{
			private readonly ChangeScope previous;

			internal readonly StoreReputation Faction;

			internal readonly GainScope Gain;

			internal readonly double Before;

			internal bool Applied;

			internal ChangeScope(StoreReputation faction)
			{
				previous = Change;
				Faction = faction;
				Gain = Current;
				Before = faction.GetReputationExact();
				Change = this;
			}

			public void Dispose()
			{
				if (Change == this)
				{
					Change = previous;
				}
			}
		}

		internal const string Id = "BE_FACTION_OUTER_RING";

		[ThreadStatic]
		internal static GainScope Current;

		[ThreadStatic]
		internal static ChangeScope Change;

		private static readonly int[] Thresholds = new int[15]
		{
			-100, -80, -60, -40, -20, 20, 40, 60, 80, 100,
			120, 140, 160, 180, 200
		};

		internal static int Reputation
		{
			get
			{
				StoreReputation storeReputation = StoreReputation.GetStoreReputation("BE_FACTION_OUTER_RING");
				if (storeReputation == null)
				{
					return 0;
				}
				return storeReputation.GetReputation();
			}
		}

		internal static int Tier
		{
			get
			{
				if (Reputation >= 0)
				{
					return Math.Min(10, Reputation / 20);
				}
				return -Math.Min(5, -Reputation / 20);
			}
		}

		internal static int SeedCountModifier
		{
			get
			{
				if (Tier > -3)
				{
					return Tier * 2;
				}
				return 0;
			}
		}

		internal static int NiniBudget => (int)Math.Round(12000.0 * (1.0 + (double)Math.Max(-4, Math.Min(0, Tier)) * 0.2));

		internal static void Ensure()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected O, but got Unknown
			PlayerStore instance = PlayerStore.Instance;
			if (instance?.storeReputations == null)
			{
				return;
			}
			StoreReputation val = StoreReputation.GetStoreReputation("BE_FACTION_OUTER_RING");
			if (val == null)
			{
				val = new StoreReputation
				{
					factionId = "BE_FACTION_OUTER_RING",
					factionColor1 = "#98454D",
					factionColor2 = "#98454D",
					factionColor3 = "#98454D"
				};
				instance.storeReputations.Add(val);
			}
			val.factionDisplay = ModText.Get("be_outer_ring_name");
			int[] array = new int[15]
			{
				-100, -80, -60, -40, -20, 20, 40, 60, 80, 100,
				120, 140, 160, 180, 200
			};
			for (int i = 0; i < array.Length; i++)
			{
				int num = array[i];
				string key = "BE_OUTER_" + ((num < 0) ? ("N" + -num) : ("P" + num));
				if (!val.perks.Any((StoreReputationPerk p) => p.perkId == key))
				{
					StoreReputationPerk val2 = ((num < 0) ? StoreReputationPerk.SecWatchlist() : StoreReputationPerk.SecProtection());
					val2.perkId = key;
					val2.perkThreshold = num;
					val2.perkType = ((num >= 0) ? 1 : 0);
					val2.firstTimeUnlockActionDone = true;
					val2.isUnlocked = ((num < 0) ? (val.GetReputation() <= num) : (val.GetReputation() >= num));
					val.perks.Add(val2);
				}
			}
		}

		internal static string PerkText(StoreReputationPerk perk, bool description)
		{
			if (perk == null || perk.perkId?.StartsWith("BE_OUTER_", StringComparison.Ordinal) != true)
			{
				return null;
			}
			int perkThreshold = perk.perkThreshold;
			if (Array.IndexOf(Thresholds, perkThreshold) >= 0)
			{
				return ModText.Get("be_outer_" + ((perkThreshold < 0) ? ("n" + -perkThreshold) : ("p" + perkThreshold)) + (description ? "_description" : "_name"));
			}
			return null;
		}

		internal static bool IsTradeClient(StoreClient client)
		{
			if (!(client?.identifier == "be_weekly_trader") && !(client?.identifier == "be_seed_trader_47"))
			{
				return client?.identifier == "be_brew_shop_47";
			}
			return true;
		}

		internal static GainScope Begin()
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
			if (!IsTradeClient((StoreClient)client))
			{
				return null;
			}
			return new GainScope();
		}
	}
	[HarmonyPatch(typeof(StoreReputation), "InitReputation")]
	internal static class FormalFactionInit
	{
		private static void Postfix()
		{
			if (ModEntry.Enabled)
			{
				FormalReputation.Ensure();
			}
		}
	}
	[HarmonyPatch(typeof(StoreReputationPerk), "GetLocalizedPerkName")]
	internal static class FormalFactionPerkName
	{
		private static void Postfix(StoreReputationPerk __instance, ref string __result)
		{
			if (ModEntry.Enabled)
			{
				__result = FormalReputation.PerkText(__instance, description: false) ?? __result;
			}
		}
	}
	[HarmonyPatch(typeof(StoreReputationPerk), "GetLocalizedDescription")]
	internal static class FormalFactionPerkDescription
	{
		private static void Postfix(StoreReputationPerk __instance, ref string __result)
		{
			if (ModEntry.Enabled)
			{
				__result = FormalReputation.PerkText(__instance, description: true) ?? __result;
			}
		}
	}
	[HarmonyPatch(typeof(StoreClient), "ApplyBudgetModifier")]
	internal static class FormalFactionBudget
	{
		private static void Postfix(StoreClient __instance, bool __runOriginal)
		{
			if (ModEntry.Enabled && __runOriginal && __instance.useClientBudget && PlayerStore.Instance != null)
			{
				int num = Math.Max(0, FormalReputation.Reputation) / 20;
				int budget = __instance.GetBudget();
				if (num != 0 && budget > 0)
				{
					__instance.OverrideBudget((int)Math.Min(2147483647L, budget + ((long)budget * (long)num + 2) / 5));
				}
			}
		}
	}
	[HarmonyPatch]
	internal static class FormalNiniBatchLimit
	{
		private static MethodBase[] TargetMethods()
		{
			return new MethodInfo[2]
			{
				AccessTools.Method(typeof(NegociationUIManager), "SellMultiple", new Type[2]
				{
					typeof(List<GameItem>),
					typeof(bool)
				}, (Type[])null),
				AccessTools.Method(typeof(NegociationUIManager), "BuyMultipleItem", new Type[1] { typeof(List<GameItem>) }, (Type[])null)
			};
		}

		private static void Prefix(out FormalReputation.GainScope __state)
		{
			__state = (ModEntry.Enabled ? FormalReputation.Begin() : null);
		}

		private static void Finalizer(FormalReputation.GainScope __state)
		{
			__state?.Dispose();
		}
	}
	[HarmonyPatch(typeof(StoreReputation), "OnItemTraded")]
	internal static class FormalNiniTradeFaction
	{
		private static void Prefix(ref string factionId, StoreClient storeClient, bool isPlayerSold, out FormalReputation.GainScope __state)
		{
			__state = null;
			if (ModEntry.Enabled && FormalReputation.IsTradeClient(storeClient))
			{
				FormalReputation.Ensure();
				factionId = "BE_FACTION_OUTER_RING";
				storeClient.clientFaction = factionId;
				__state = new FormalReputation.GainScope();
			}
		}

		private static void Finalizer(FormalReputation.GainScope __state)
		{
			__state?.Dispose();
		}
	}
	[HarmonyPatch]
	internal static class FormalNiniRepCap
	{
		private static MethodBase[] TargetMethods()
		{
			return new MethodInfo[2]
			{
				AccessTools.Method(typeof(StoreReputation), "ModReputation", new Type[1] { typeof(double) }, (Type[])null),
				AccessTools.Method(typeof(StoreReputation), "ModReputationRaw", new Type[1] { typeof(double) }, (Type[])null)
			};
		}

		private static void Prefix(StoreReputation __instance, out FormalReputation.ChangeScope __state)
		{
			__state = ((ModEntry.Enabled && __instance.factionId == "BE_FACTION_OUTER_RING" && FormalReputation.Current != null) ? new FormalReputation.ChangeScope(__instance) : null);
		}

		private static void Finalizer(FormalReputation.ChangeScope __state)
		{
			__state?.Dispose();
		}
	}
	[HarmonyPatch(typeof(StoreReputation), "CheckReputation")]
	internal static class FormalNiniFinalGainCap
	{
		private static void Prefix(StoreReputation __instance, ref double ___amount)
		{
			FormalReputation.ChangeScope change = FormalReputation.Change;
			if (ModEntry.Enabled && change != null && !change.Applied && change.Faction == __instance)
			{
				change.Applied = true;
				double num = ___amount - change.Before;
				if (num > 0.0)
				{
					___amount = change.Before + change.Gain.Take(num);
				}
			}
		}
	}
	internal sealed class FormalShop : AfterhourLocation
	{
		internal const string LocationId = "brewing_expansion.brew_shop";

		internal const string ClientId = "be_brew_shop_47";

		private readonly HashSet<GameItem> stock = new HashSet<GameItem>();

		private readonly HashSet<string> purchased = new HashSet<string>();

		private GameObject stage;

		internal static FormalShop Instance;

		public override string Id => "brewing_expansion.brew_shop";

		public override string Source => "brewing_expansion";

		public override string ImageKey => "brewing_expansion:be_shop_scene";

		public override bool ShowsStoreCounters => true;

		public override GameObject PanelObject => MapUIManager.Instance?.UIUpgradeMerchantPanel;

		public override GameObject StageObject
		{
			get
			{
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Expected O, but got Unknown
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_010e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0123: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0139: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0153: Unknown result type (might be due to invalid IL or missing references)
				//IL_0158: Unknown result type (might be due to invalid IL or missing references)
				//IL_0174: Unknown result type (might be due to invalid IL or missing references)
				//IL_017e: Unknown result type (might be due to invalid IL or missing references)
				//IL_018c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0191: Unknown result type (might be due to invalid IL or missing references)
				//IL_0195: Unknown result type (might be due to invalid IL or missing references)
				//IL_019a: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)stage != (Object)null)
				{
					return stage;
				}
				Sprite modSprite = CustomUIManager.Instance.GetModSprite("brewing_expansion", "be_shop_scene");
				if ((Object)(object)modSprite == (Object)null)
				{
					return null;
				}
				Rect rect = modSprite.rect;
				float num = ((Rect)(ref rect)).width / 9.6f;
				stage = new GameObject("BrewingExpansion.BrewShopStage");
				SpriteRenderer obj = stage.AddComponent<SpriteRenderer>();
				((Renderer)obj).sortingOrder = 20;
				obj.sprite = Sprite.Create(modSprite.texture, modSprite.rect, new Vector2(0.5f, 0.5f), num);
				Sprite modSprite2 = CustomUIManager.Instance.GetModSprite("brewing_expansion", "be_shop_47");
				if ((Object)(object)modSprite2 != (Object)null)
				{
					GameObject val = new GameObject("FortySevenShopPortrait");
					val.transform.SetParent(stage.transform, false);
					SpriteRenderer val2 = val.AddComponent<SpriteRenderer>();
					((Renderer)val2).sortingOrder = 21;
					val2.sprite = Sprite.Create(modSprite2.texture, modSprite2.rect, new Vector2(0.5f, 0.5f), num);
					val.transform.localScale = new Vector3(1.65f, 1.65f, 1f);
					Transform transform = val.transform;
					rect = modSprite.rect;
					float num2 = ((Rect)(ref rect)).height * 0.5f - 484f;
					rect = modSprite2.rect;
					transform.localPosition = new Vector3(0f, (num2 + ((Rect)(ref rect)).height * 0.5f * 1.65f) / num, 0f);
					BoxCollider2D obj2 = val.AddComponent<BoxCollider2D>();
					Bounds bounds = val2.sprite.bounds;
					obj2.size = Vector2.op_Implicit(((Bounds)(ref bounds)).size);
					bounds = val2.sprite.bounds;
					((Collider2D)obj2).offset = Vector2.op_Implicit(((Bounds)(ref bounds)).center);
					Interactable obj3 = val.AddComponent<Interactable>();
					obj3.isControlable = false;
					obj3.doNotOutline = true;
					val.AddComponent<StoreTablet>();
					val.AddComponent<TooltipContainer>();
				}
				stage.SetActive(false);
				return stage;
			}
		}

		public override string GetTitle()
		{
			return ModText.Get("be_shop_name");
		}

		public override bool IsDiscovered()
		{
			return true;
		}

		public override bool IsAvailable()
		{
			return FormalReputation.Tier > -4;
		}

		public override Vector2? GetFrontCounterPosition(int width, int height)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return ((AfterhourLocation)new DrJacksonLocation()).GetFrontCounterPosition(width, height);
		}

		public override Vector2? GetBackCounterPosition(int width, int height)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return ((AfterhourLocation)new DrJacksonLocation()).GetBackCounterPosition(width, height);
		}

		public override void Arrive()
		{
			purchased.Clear();
			stock.Clear();
			FormalVisitors.RegisterSprites();
			PlayerStore instance = PlayerStore.Instance;
			EmporiumEntry instance2 = EmporiumEntry.Instance;
			MapUIManager instance3 = MapUIManager.Instance;
			instance2.frontInvWindow.Show();
			instance2.backInvWindow.Show();
			instance3.TransferBackInvToCounter();
			instance2.ExpandFrontCounterGrid();
			StoreClient val = Blueprint();
			instance.currentClientInstance = StoreClientInstance.CreateClientInstance(val);
			instance.isClientArrived = true;
			StoreUIManager.Instance.HideClient();
			StoreClientMono.Instance.image.sprite = null;
			StoreUIManager.Instance.SetClientPositionToDrJackson();
			instance3.SetCreditDisplayPositionToDrJackson();
			StockAll();
		}

		internal StoreClient Blueprint()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			//IL_00a1: Expected O, but got Unknown
			StoreClient val = new StoreClient
			{
				identifier = "be_brew_shop_47",
				displayName = ((AfterhourLocation)this).GetTitle(),
				spriteName = "be_shop_47",
				clientIntent = (ClientIntent)3,
				isIntroduced = true,
				useClientBudget = false,
				AcceptingContrabandOverride = true,
				clientBuyingTagList = { "ALCOHOL" },
				clientFaction = "BE_FACTION_OUTER_RING",
				onItemAddedToWeighted = delegate(GameItem item)
				{
					if (!GeneralHelper.IsItemOwned(item))
					{
						ItemFeature val2 = ItemFeatureList.InventorMarkup();
						if (val2 != null)
						{
							item.AddItemFeature(val2);
						}
					}
				},
				onItemSold = delegate(GameItem item, int _)
				{
					RecordPurchase(item);
				}
			};
			ModHelper.SetClientSource(val, "brewing_expansion");
			return val;
		}

		private void RecordPurchase(GameItem item)
		{
			if (item != null && stock.Contains(item))
			{
				string item2 = item.identifier + (item.IsTag("BE_XUXU_BASIC_EMPTY_VARIANT") ? ".empty" : "");
				purchased.Add(item2);
				int dayCounter = StoreStation.Instance.dayCounter;
				if (item.identifier == "be_jeanne_moreau_fruit")
				{
					ModHelper.SetModDataInt("brewing_expansion", "flower_purchase_day", dayCounter);
				}
				if (item.identifier == "be_midautumn_blindbox")
				{
					ModHelper.SetModDataInt("brewing_expansion", "midautumn_purchase_day", dayCounter);
				}
			}
		}

		internal void StockAll()
		{
			if (ModHelper.GetCurrentAfterhourLocationId() != "brewing_expansion.brew_shop")
			{
				return;
			}
			GameItem[] array = stock.ToArray();
			foreach (GameItem val in array)
			{
				if (MachineConnections.Live(val) && !GeneralHelper.IsItemOwned(val))
				{
					val.Destroy();
				}
			}
			stock.Clear();
			int dayCounter = StoreStation.Instance.dayCounter;
			FormalShopLayout.ShelfSlot[] all = FormalShopLayout.All;
			for (int i = 0; i < all.Length; i++)
			{
				FormalShopLayout.ShelfSlot slot = all[i];
				string id = slot.Id;
				if (!purchased.Contains(id) && (!(id == "be_jeanne_moreau_fruit") || ModHelper.GetModDataInt("brewing_expansion", "flower_purchase_day", -1) != dayCounter) && (!(id == "be_midautumn_blindbox") || dayCounter - ModHelper.GetModDataInt("brewing_expansion", "midautumn_purchase_day", -999) >= 5) && (!new string[3] { "be_brewery_sign", "be_nini_card", "be_trade_perfume" }.Contains(id) || !PlayerStore.Instance.FindAllItem(true).Any((GameItem val3) => val3.identifier == id && GeneralHelper.IsItemOwned(val3))))
				{
					bool num = id.EndsWith(".empty", StringComparison.Ordinal);
					string text = (num ? id.Substring(0, id.Length - 6) : id);
					GameItem val2 = DirectoryMaster.Item(text, false);
					if (num)
					{
						RemainingItems.Tag(val2, "power_source_item_energy", 0);
						val2.SetValue(200L);
						val2.unitBaseValue = 0L;
						val2.lateUnitValue = 0L;
						val2.EnableTag("BE_XUXU_BASIC_EMPTY_VARIANT", false);
						RemainingItems.RefreshBattery(val2);
					}
					if (text == "be_nini_card")
					{
						val2.SetValue(500L);
					}
					PlayerStore.Instance.AddDirectSellingItemToTable(val2, false, false, false, 0);
					if (val2.parentInventory != null)
					{
						FormalShopLayout.Place(val2, slot);
						stock.Add(val2);
					}
					else if (MachineConnections.Live(val2))
					{
						val2.Destroy();
					}
				}
			}
			NegociationUIManager.Instance.UpdateUI();
		}

		public override string GetLeaveWarning()
		{
			return ModText.Get("be_shop_leave_warning");
		}

		public override void Leave()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			((AfterhourLocation)new DrJacksonLocation()).Leave();
			stock.Clear();
			purchased.Clear();
		}

		internal static void Register()
		{
			Instance = new FormalShop();
			ModHelper.AddAfterhourLocation((AfterhourLocation)(object)Instance);
		}

		internal static void Remove()
		{
			FormalShopPreview.Remove();
			ModHelper.RemoveAfterhourLocations("brewing_expansion");
			if ((Object)(object)Instance?.stage != (Object)null)
			{
				Object.Destroy((Object)(object)Instance.stage);
			}
			Instance = null;
		}
	}
	internal static class FormalShopLayout
	{
		internal readonly struct ShelfSlot
		{
			internal readonly string Id;

			internal readonly int X;

			internal readonly int Y;

			internal readonly int Orientation;

			internal ShelfSlot(string id, int x, int y, int orientation = 0)
			{
				Id = id;
				X = x;
				Y = y;
				Orientation = orientation;
			}
		}

		internal static readonly ShelfSlot[] All = new ShelfSlot[45]
		{
			new ShelfSlot("be_incubator", 0, 0),
			new ShelfSlot("be_wine_printer", 3, 0),
			new ShelfSlot("be_large_brewing_barrel", 6, 0),
			new ShelfSlot("be_fruit_pulp_concentrator", 9, 0),
			new ShelfSlot("be_xuxu_water_bottle_printer", 12, 0),
			new ShelfSlot("be_liunian_ager", 0, 3),
			new ShelfSlot("be_clear_water_tank", 3, 3),
			new ShelfSlot("be_clear_brew_water_bucket", 5, 3),
			new ShelfSlot("be_xuxu_connection_pliers", 7, 3, 1),
			new ShelfSlot("wine_yeast_infinite", 8, 3),
			new ShelfSlot("be_golden_yeast_reactor", 10, 3),
			new ShelfSlot("wine_yeast", 12, 3),
			new ShelfSlot("wine_yeast_red", 13, 3),
			new ShelfSlot("be_advanced_yeast_reactor", 8, 5),
			new ShelfSlot("be_forbidden_yeast_reactor", 10, 5),
			new ShelfSlot("wine_superyeast", 12, 5),
			new ShelfSlot("be_wine_yeast_hentai", 13, 5),
			new ShelfSlot("be_qingyun_filter", 0, 6),
			new ShelfSlot("be_ninglu_filter", 1, 6),
			new ShelfSlot("be_yingfei_filter", 2, 6),
			new ShelfSlot("be_xuxu_basic_battery", 3, 6),
			new ShelfSlot("be_xuxu_basic_battery.empty", 4, 6),
			new ShelfSlot("be_xuxu_mid_battery", 5, 6),
			new ShelfSlot("be_xuxu_high_battery", 6, 6),
			new ShelfSlot("be_xuxu_water_purifier_basic", 8, 7),
			new ShelfSlot("be_xuxu_water_purifier_middle", 10, 7),
			new ShelfSlot("be_xuxu_water_purifier_advanced", 12, 7),
			new ShelfSlot("be_low_blindbox", 0, 9),
			new ShelfSlot("be_mid_blindbox", 2, 9),
			new ShelfSlot("be_premium_blindbox", 4, 9),
			new ShelfSlot("be_afterglow_blindbox", 6, 9),
			new ShelfSlot("be_midautumn_blindbox", 8, 9),
			new ShelfSlot("be_jeanne_moreau_fruit", 10, 9),
			new ShelfSlot("be_multi_storage", 12, 9),
			new ShelfSlot("be_wine_blindbox", 0, 11),
			new ShelfSlot("be_figurine_blindbox", 2, 11),
			new ShelfSlot("be_container_blindbox", 4, 11),
			new ShelfSlot("be_xuxu_cassette_bundle", 6, 11),
			new ShelfSlot("be_xuxu_mini_storage", 8, 11),
			new ShelfSlot("be_compact_smuggler_bay", 10, 11),
			new ShelfSlot("be_contraband_perfume", 0, 13),
			new ShelfSlot("be_freedom_perfume", 2, 13),
			new ShelfSlot("be_trade_perfume", 4, 13),
			new ShelfSlot("be_nini_card", 6, 13),
			new ShelfSlot("be_brewery_sign", 8, 13)
		};

		internal static void Place(GameItem item, ShelfSlot slot)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			GameGridInventory frontInvinvElement = EmporiumEntry.Instance.frontInvinvElement;
			GridShape val = new GridShapeBuilder(item.modifiedShape).SetTransform(slot.X, slot.Y, false, slot.Orientation).Build();
			SlotMarker val2 = ((GameInventory)frontInvinvElement).TryInventorySlot(item, item.unitCount, val, item.modifiedState);
			if (val2 != null && val2.IsValid())
			{
				val2.TryAcceptOnce(-1);
			}
			if ((object)item.parentInventory != frontInvinvElement || item.modifiedShape.minX != slot.X || item.modifiedShape.minY != slot.Y || item.modifiedShape.orientation != slot.Orientation)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Shop shelf position occupied for " + slot.Id + "; kept native placement."));
			}
		}
	}
	internal static class FormalShopPreview
	{
		internal const string ActorName = "BE_Night47";

		internal const float ActorScale = 1.65f;

		internal const float ActorBottomFromTop = 484f;

		internal static void Apply(TransitCardView card)
		{
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			if (!ModEntry.Enabled)
			{
				return;
			}
			object obj;
			if (card == null)
			{
				obj = null;
			}
			else
			{
				AfterhourLocation location = card.location;
				obj = ((location != null) ? location.Id : null);
			}
			if (!((string?)obj != "brewing_expansion.brew_shop"))
			{
				Transform obj2 = ((Component)card).transform.Find("Image/Picture");
				Image val = ((obj2 != null) ? ((Component)obj2).GetComponent<Image>() : null);
				Sprite modSprite = CustomUIManager.Instance.GetModSprite("brewing_expansion", "be_shop_47");
				if (!((Object)(object)((val != null) ? val.sprite : null) == (Object)null) && !((Object)(object)modSprite == (Object)null) && !((Object)(object)((Component)val).transform.Find("BE_Night47") != (Object)null))
				{
					Rect rect = val.sprite.rect;
					Rect rect2 = modSprite.rect;
					float num = ((Rect)(ref rect2)).width * 1.65f / ((Rect)(ref rect)).width;
					rect2 = modSprite.rect;
					float num2 = ((Rect)(ref rect2)).height * 1.65f / ((Rect)(ref rect)).height;
					float num3 = 1f - 484f / ((Rect)(ref rect)).height;
					GameObject val2 = new GameObject("BE_Night47", new Type[3]
					{
						typeof(RectTransform),
						typeof(CanvasRenderer),
						typeof(Image)
					});
					RectTransform component = val2.GetComponent<RectTransform>();
					((Transform)component).SetParent(((Component)val).transform, false);
					component.anchorMin = new Vector2(0.5f - num * 0.5f, num3);
					component.anchorMax = new Vector2(0.5f + num * 0.5f, num3 + num2);
					Vector2 offsetMin = (component.offsetMax = Vector2.zero);
					component.offsetMin = offsetMin;
					Image component2 = val2.GetComponent<Image>();
					component2.sprite = modSprite;
					component2.preserveAspect = true;
					((Graphic)component2).raycastTarget = false;
					((Graphic)component2).color = ((Graphic)val).color;
				}
			}
		}

		internal static void Remove()
		{
			TransitCardView[] array = Resources.FindObjectsOfTypeAll<TransitCardView>();
			foreach (TransitCardView val in array)
			{
				AfterhourLocation location = val.location;
				if (!(((location != null) ? location.Id : null) != "brewing_expansion.brew_shop"))
				{
					Transform val2 = ((Component)val).transform.Find("Image/Picture/BE_Night47");
					if ((Object)(object)val2 != (Object)null)
					{
						Object.Destroy((Object)(object)((Component)val2).gameObject);
					}
				}
			}
		}
	}
	[HarmonyPatch(typeof(TransitUI), "BuildCard")]
	internal static class FormalShopPreviewCard
	{
		private static void Postfix(TransitCardView __result)
		{
			FormalShopPreview.Apply(__result);
		}
	}
	internal static class FormalTobaccoInspection
	{
		internal static bool Reveal(StoreClient client, GameItem item)
		{
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			if (!ModEntry.Enabled || !FormalVisitors.Open || client?.identifier != "be_weekly_trader" || item == null || !GeneralHelper.IsItemOwned(item) || (!item.IsTag("CIGARETTE_TAG") && !item.IsGameItemType("CIGARETTE")))
			{
				return false;
			}
			PlayerStore instance = PlayerStore.Instance;
			string identifier = ItemConditionList.CreateCounterfeitCigarette().identifier;
			ItemFeature val = item.FindItemFeatureByCategory("CATEGORY_GENUINE_CIGARETTE");
			ItemFeature val2 = item.FindItemFeatureByID("TE_TOBACCO_CONDITION");
			bool flag = false;
			bool flag2 = false;
			ItemFeature[] array = (ItemFeature[])(object)new ItemFeature[2] { val, val2 };
			foreach (ItemFeature val3 in array)
			{
				if (val3 == null || val3.isDisabled || val3.IsExposed())
				{
					continue;
				}
				bool flag3 = val3 == val && val3.realCondition?.identifier == identifier;
				bool flag4 = val3 == val2 && val3.realCondition?.identifier == "te_condition_defective";
				if ((flag3 || flag4) && !(val3.fakeCondition?.identifier == val3.realCondition?.identifier))
				{
					bool flag5 = flag3 || val3.fakeCondition?.identifier == "te_condition_good";
					val3.ExposeFeature(true, true);
					if (instance != PlayerStore.Instance || !MachineConnections.Live(item))
					{
						return false;
					}
					if (val3.IsExposed())
					{
						flag = true;
						flag2 = flag2 || flag5;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
			if (flag2 && !item.IsTag("BE_NINI_TOBACCO_PENALIZED"))
			{
				FormalReputation.Ensure();
				StoreReputation.ModReputationRaw("BE_FACTION_OUTER_RING", -5, true);
				item.EnableTag("BE_NINI_TOBACCO_PENALIZED", false);
				item.EnableTag("TE_FRAUD_PENALIZED", false);
			}
			if (FormalVisitors.Open)
			{
				DialogUIManager instance2 = DialogUIManager.Instance;
				if (instance2 != null)
				{
					instance2.StartDialogue(new Dialogue().SetText(ModText.Get("be_nini_name"), ModText.Get("be_nini_inspection_warning")), false);
				}
			}
			return true;
		}

		internal static bool BeforeSale(GameItem item)
		{
			if (!ModEntry.Enabled || RemainingConsumables.Client?.identifier != "be_weekly_trader")
			{
				return true;
			}
			try
			{
				if (!Reveal(RemainingConsumables.Client, item))
				{
					return true;
				}
				NegociationUIManager instance = NegociationUIManager.Instance;
				if (instance != null)
				{
					instance.UpdateUI(item);
				}
				NegociationUIManager instance2 = NegociationUIManager.Instance;
				if (instance2 != null)
				{
					instance2.OpenUI();
				}
				return false;
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Nini inspection interrupted; item retained: " + ex));
				return false;
			}
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "SellItem")]
	internal static class FormalNiniInspectSale
	{
		private static bool Prefix(GameItem item)
		{
			return FormalTobaccoInspection.BeforeSale(item);
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "SellItemFromTable")]
	internal static class FormalNiniInspectTable
	{
		private static bool Prefix(PlayerStore __instance)
		{
			return FormalTobaccoInspection.BeforeSale(__instance.currentNegociatedItem);
		}
	}
	[HarmonyPatch(typeof(StoreClient), "ClientExposeFeature")]
	internal static class FormalNiniInspectNative
	{
		private static void Prefix(StoreClient __instance, GameItem __0)
		{
			if (ModEntry.Enabled)
			{
				FormalTobaccoInspection.Reveal(__instance, __0);
			}
		}
	}
	[HarmonyPatch(typeof(StoreClient), "StartMainDialogue")]
	internal static class FormalVisitorDialogueHours
	{
		private static bool Prefix(StoreClient __instance)
		{
			if (ModEntry.Enabled && !FormalVisitors.Open)
			{
				return !FormalVisitors.IsDayClient(__instance.identifier);
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(StoreUIManager), "OnNextClientArrived")]
	internal static class FormalVisitorArrivalHours
	{
		private static bool Prefix()
		{
			if (!ModEntry.Enabled || FormalVisitors.Open)
			{
				return true;
			}
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
			if (!FormalVisitors.IsDayClient(val?.identifier))
			{
				return true;
			}
			if (!instance.storeClientManager.clientStack.Contains(val))
			{
				instance.storeClientManager.clientStack.Insert(0, val);
			}
			instance.currentClientInstance = null;
			instance.isClientArrived = false;
			return false;
		}
	}
	internal static class FormalTraitIcons
	{
		internal static readonly string[] Ids = new string[5] { "be_xuxu_music", "be_wine_devotion", "be_dead_drunk", "be_seed_aversion", "be_brewer_starter" };

		private static readonly Dictionary<string, Sprite> Registered = new Dictionary<string, Sprite>();

		internal static void Register()
		{
			if (!ModEntry.Enabled || (Object)(object)CustomUIManager.Instance == (Object)null)
			{
				return;
			}
			Dictionary<string, Sprite> perkIcons = StartingPerkIconLoader.perkIcons;
			string[] ids = Ids;
			foreach (string text in ids)
			{
				Sprite modSprite = CustomUIManager.Instance.GetModSprite("brewing_expansion", text);
				if (!((Object)(object)modSprite == (Object)null) && (!perkIcons.TryGetValue(text, out var value) || !((Object)(object)value != (Object)null) || (Registered.TryGetValue(text, out var value2) && value == value2)))
				{
					perkIcons[text] = modSprite;
					Registered[text] = modSprite;
				}
			}
			StartingPerkElement[] array = Resources.FindObjectsOfTypeAll<StartingPerkElement>();
			foreach (StartingPerkElement val in array)
			{
				if (val.perk != null && (Object)(object)val.icon != (Object)null && Registered.TryGetValue(val.perk.id, out var value3) && perkIcons.TryGetValue(val.perk.id, out var value4) && value4 == value3)
				{
					val.icon.sprite = value3;
				}
			}
		}

		internal static void Remove()
		{
			Dictionary<string, Sprite> perkIcons = StartingPerkIconLoader.perkIcons;
			foreach (KeyValuePair<string, Sprite> item in Registered)
			{
				if (perkIcons.TryGetValue(item.Key, out var value) && value == item.Value)
				{
					perkIcons.Remove(item.Key);
				}
			}
			Registered.Clear();
		}
	}
	[HarmonyPatch(typeof(StartingPerkIconLoader), "Start")]
	internal static class FormalTraitIconsAfterNativeInit
	{
		private static void Postfix()
		{
			try
			{
				FormalTraitIcons.Register();
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion] Trait icon registration deferred: " + ex.Message));
			}
		}
	}
	internal static class FormalTraits
	{
		internal const string Music = "be_xuxu_music";

		internal const string Devotion = "be_wine_devotion";

		internal const string Drunk = "be_dead_drunk";

		internal const string Aversion = "be_seed_aversion";

		internal const string Sponsor = "be_brewer_starter";

		internal const string Scenario = "be_new_brewer_start";

		private static PlayerStore countStore;

		private static int countFrame = -1;

		private static int ownedWineCount;

		private static int FinishedWineCount()
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance == countStore && countFrame == Time.frameCount)
			{
				return ownedWineCount;
			}
			countStore = instance;
			countFrame = Time.frameCount;
			ownedWineCount = 0;
			if (instance != null)
			{
				foreach (GameItem item in instance.FindAllItem(true))
				{
					if (GeneralHelper.IsItemOwned(item) && (WineHelper.IsFinishedWine(item) || RemainingItems.IsCollector(item)) && ++ownedWineCount >= 40)
					{
						break;
					}
				}
			}
			return ownedWineCount;
		}

		internal static bool Has(string id)
		{
			return ModHelper.IsStartingPerkActive(id);
		}

		internal static void Register()
		{
			Add("be_xuxu_music", 1, "be_perk_music_name", "be_perk_music_description");
			Add("be_wine_devotion", 2, "be_perk_devotion_name", "be_perk_devotion_description");
			Add("be_dead_drunk", -2, "be_perk_drunk_name", "be_perk_drunk_description");
			Add("be_seed_aversion", -2, "be_perk_aversion_name", "be_perk_aversion_description");
			Add("be_brewer_starter", 1, "be_perk_sponsor_name", "be_perk_sponsor_description");
			ModHelper.AddStartingScenario("brewing_expansion", "be_new_brewer_start", (Func<string>)(() => ModText.Get("be_brewer_start_name")), (Func<string>)(() => ModText.Get("be_brewer_start_description")), new Func<string>[1]
			{
				() => ModText.Get("be_brewer_start_supplies")
			}, (Action)StartScenario, "brewing_expansion:be_new_brewer_icon", (Func<string>)null, (TagTone)1, false, false);
		}

		private static void Add(string id, int cost, string nameKey, string descriptionKey)
		{
			ModHelper.AddStartingPerk("brewing_expansion", id, cost, (StartingPerkType)(cost <= 0), (Func<string>)(() => ModText.Get(nameKey)), (Func<string>)(() => ModText.Get(descriptionKey)), "brewing_expansion:" + id, (string[])null, 0, false);
		}

		private static void Grant(string id, int count = 1)
		{
			for (int i = 0; i < count; i++)
			{
				Grant(ModHelper.SpawnItem(id));
			}
		}

		private static void Grant(GameItem item)
		{
			ModHelper.MoveItemToInventory(item);
			if (item.parentInventory == null)
			{
				string identifier = item.identifier;
				item.Destroy();
				throw new InvalidOperationException("Starting item could not be placed: " + identifier);
			}
		}

		private static void StartScenario()
		{
			Grant("wine_yeast", 4);
			Grant("wine_bottle", 4);
			Grant("be_clear_water_tank");
			Grant("be_brewery_sign");
			Grant("sign_water");
			Grant("water_filter");
			Grant(PreBuiltItemHelper.FullWaterTestStripBox());
		}

		internal static void NewGame(string id)
		{
			if (Has("be_brewer_starter"))
			{
				Grant("aquascan");
				Grant("be_incubator");
				Grant("energy_credit");
			}
			if (Has("be_xuxu_music"))
			{
				Grant("be_xuxu_cassette_bundle");
			}
		}

		internal static void Apply(GameItem item, bool clientArea)
		{
			item.RemoveItemFeatureByID("be_wine_devotion");
			item.RemoveItemFeatureByID("be_dead_drunk");
			if (ModEntry.Enabled)
			{
				bool flag = GeneralHelper.IsItemOwned(item);
				bool flag2 = item.IsTag("SUBSTANCE");
				int val = ((Has("be_wine_devotion") && flag) ? FinishedWineCount() : 0);
				int bonus = ((Has("be_wine_devotion") && flag) ? (flag2 ? (20 + Math.Min(val, 40) * 2) : (-25)) : 0);
				AddFeature(item, "be_wine_devotion", bonus, flag, ModText.Get("be_perk_devotion_name"));
				AddFeature(item, "be_dead_drunk", (Has("be_dead_drunk") && flag2) ? 30 : 0, flag, ModText.Get("be_perk_drunk_name"));
			}
		}

		private static void AddFeature(GameItem item, string id, int bonus, bool owned, string display)
		{
			if (bonus != 0)
			{
				ItemFeature val = (owned ? ItemFeatureList.NeuroActivePerfumeSelling() : ItemFeatureList.NeuroActivePerfumeBuying());
				val.identifier = id;
				val.category = ModText.Get("be_trait_bonus_category");
				val.SetValueStage((ValueStage)2).SetValueModifier(bonus).SetPreExposedValueMod(bonus, true);
				display = ModText.Get("be_trait_bonus_display", display);
				val.SetPublicDisplay(display).SetActualDisplay(display);
				item.AddItemFeature(val);
			}
		}

		internal static void NightLoss()
		{
			if (!Has("be_dead_drunk") || PlayerStore.Instance == null)
			{
				return;
			}
			int dayCounter = StoreStation.Instance.dayCounter;
			if (ModHelper.GetModDataInt("brewing_expansion", "drunk_loss_day", -1) == dayCounter)
			{
				return;
			}
			PlayerStore instance = PlayerStore.Instance;
			ModHelper.SetModDataInt("brewing_expansion", "drunk_loss_day", dayCounter);
			List<GameItem> list = ((GameInventory)instance.GetStoreInventory()).childItems.SelectMany((GameItem i) => (IEnumerable<GameItem>)((i.FindAllChildItems().Count != 0) ? ((IEnumerable)(from c in i.FindAllChildItems()
				where c.FindAllChildItems().Count == 0
				select c)) : ((IEnumerable)new GameItem[1] { i }))).Where((Func<GameItem, bool>)GeneralHelper.IsItemOwned).Distinct()
				.ToList();
			Random random = new Random();
			int num = 0;
			while (list.Count > 0 && num < 2 && instance == PlayerStore.Instance)
			{
				int index = random.Next(list.Count);
				GameItem val = list[index];
				list.RemoveAt(index);
				if (MachineConnections.Owned(val) && val.FindAllChildItems().Count <= 0)
				{
					val.Destroy();
					if (!MachineConnections.Live(val))
					{
						num++;
					}
				}
			}
		}
	}
	internal static class FormalVisitorPersistence
	{
		[Serializable]
		private sealed class Receipt
		{
			public int day;

			public string[] queue = Array.Empty<string>();

			public string active;

			public string[] stock = Array.Empty<string>();
		}

		private const string QueueKey = "visitor_queue_v1";

		private const string StockKey = "visitor_stock_resume_v1";

		internal static void Capture(PlayerStore store)
		{
			if (!ModEntry.Enabled || store != PlayerStore.Instance || store.storeClientManager == null)
			{
				return;
			}
			StoreClientInstance currentClientInstance = store.currentClientInstance;
			string text = ((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.identifier);
			Receipt receipt = new Receipt
			{
				day = StoreStation.Instance.dayCounter,
				active = (FormalVisitors.IsDayClient(text) ? text : null),
				queue = (from c in store.storeClientManager.clientStack
					where FormalVisitors.IsDayClient(c.identifier)
					select c.identifier).ToArray()
			};
			if (text == "be_seed_trader_47")
			{
				receipt.stock = (from i in ((GameInventory)EmporiumEntry.Instance.frontInvinvElement).childItems
					where i.IsTag("BE_47_STOCK") && !GeneralHelper.IsItemOwned(i)
					select i.identifier).ToArray();
				string modData = ModHelper.GetModData("brewing_expansion", "visitor_stock_resume_v1", "");
				if (!string.IsNullOrEmpty(modData))
				{
					Receipt receipt2 = JsonUtility.FromJson<Receipt>(modData);
					if (receipt2?.day == receipt.day)
					{
						receipt.stock = receipt.stock.Concat(receipt2.stock ?? Array.Empty<string>()).ToArray();
					}
				}
			}
			ModHelper.SetModData("brewing_expansion", "visitor_queue_v1", JsonUtility.ToJson((object)receipt));
		}

		internal static void Restore()
		{
			string modData = ModHelper.GetModData("brewing_expansion", "visitor_queue_v1", "");
			if (string.IsNullOrEmpty(modData))
			{
				return;
			}
			Receipt receipt = JsonUtility.FromJson<Receipt>(modData);
			PlayerStore instance = PlayerStore.Instance;
			if (receipt == null || instance?.storeClientManager == null)
			{
				return;
			}
			ModHelper.SetModData("brewing_expansion", "visitor_queue_v1", "");
			if (receipt.day != StoreStation.Instance.dayCounter)
			{
				return;
			}
			receipt.stock = (receipt.stock ?? Array.Empty<string>()).Select(ItemIdentity.Canonical).ToArray();
			StoreClientInstance currentClientInstance = instance.currentClientInstance;
			string text = ((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.identifier);
			bool called = ModHelper.GetModDataInt("brewing_expansion", "nini_last_call", -999) == receipt.day;
			string[] array = (from text2 in ((!FormalVisitors.IsDayClient(receipt.active)) ? ((IEnumerable<string>)Array.Empty<string>()) : ((IEnumerable<string>)new string[1] { receipt.active })).Concat(receipt.queue ?? Array.Empty<string>())
				where FormalVisitors.IsDayClient(text2) && (text2 != "be_weekly_trader" || called)
				select text2).Distinct().ToArray();
			if (receipt.active == "be_seed_trader_47")
			{
				ModHelper.SetModData("brewing_expansion", "visitor_stock_resume_v1", JsonUtility.ToJson((object)receipt));
			}
			foreach (string id in array)
			{
				if (!(id == text) && !instance.storeClientManager.clientStack.Any((StoreClient c) => c.identifier == id))
				{
					StoreClient item = ((id == "be_weekly_trader") ? FormalVisitors.Nini() : ((id == "be_seed_trader_47") ? FormalVisitors.FortySeven() : FormalVisitors.Assassin()));
					if (id == receipt.active || id == "be_outer_ring_assassin")
					{
						instance.storeClientManager.clientStack.Insert(0, item);
					}
					else
					{
						instance.storeClientManager.clientStack.Add(item);
					}
				}
			}
			FormalVisitors.EnsureFortySeven(instance, allowNew: false);
		}

		internal static bool ResumeStock()
		{
			string modData = ModHelper.GetModData("brewing_expansion", "visitor_stock_resume_v1", "");
			if (string.IsNullOrEmpty(modData))
			{
				return false;
			}
			Receipt receipt = JsonUtility.FromJson<Receipt>(modData);
			if (receipt == null || receipt.day != StoreStation.Instance.dayCounter)
			{
				ModHelper.SetModData("brewing_expansion", "visitor_stock_resume_v1", "");
				return false;
			}
			List<string> list = (receipt.stock ?? Array.Empty<string>()).Select(ItemIdentity.Canonical).ToList();
			string[] array = list.ToArray();
			foreach (string text in array)
			{
				string text2 = RemainingItems.Find(text)?.Kind;
				if (BrewingSeeds.Find(text) == null && text2 != "Recipe" && text2 != "Manual")
				{
					list.Remove(text);
					continue;
				}
				GameItem val = DirectoryMaster.Item(text, false);
				val.EnableTag("BE_47_STOCK", false);
				PlayerStore.Instance.AddDirectSellingItemToTable(val, false, false, false, 0);
				if (val.parentInventory == null)
				{
					if (MachineConnections.Live(val))
					{
						val.Destroy();
					}
				}
				else
				{
					list.Remove(text);
					receipt.stock = list.ToArray();
					ModHelper.SetModData("brewing_expansion", "visitor_stock_resume_v1", JsonUtility.ToJson((object)receipt));
				}
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "SaveGame")]
	internal static class FormalVisitorSave
	{
		private static void Prefix(PlayerStore __instance, bool __runOriginal)
		{
			if (!__runOriginal || !ModEntry.Enabled)
			{
				return;
			}
			try
			{
				FormalVisitorPersistence.Capture(__instance);
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Pending visitor snapshot failed: " + ex));
			}
		}
	}
	internal static class FormalVisitors
	{
		internal const string NiniId = "be_weekly_trader";

		internal const string FortySevenId = "be_seed_trader_47";

		internal const string AssassinId = "be_outer_ring_assassin";

		internal const string PhoneId = "brewing_expansion.nini";

		private const string LegacyPhoneId = "BrewingExpansion.Nini";

		private static readonly Dictionary<string, Sprite> Portraits = new Dictionary<string, Sprite>();

		internal static bool Open
		{
			get
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Invalid comparison between Unknown and I4
				PlayerStore instance = PlayerStore.Instance;
				if (instance == null)
				{
					return false;
				}
				return (int)instance.storeState == 1;
			}
		}

		internal static bool IsDayClient(string id)
		{
			if (!(id == "be_weekly_trader") && !(id == "be_seed_trader_47"))
			{
				return id == "be_outer_ring_assassin";
			}
			return true;
		}

		internal static string CallCooldown(int days)
		{
			return ModText.Get("be_nini_phone_cooldown", days);
		}

		internal static void RegisterFactories()
		{
			if (!StoreClientListDict.storeClientDict.ContainsKey("be_weekly_trader"))
			{
				StoreClientListDict.storeClientDict.Add("be_weekly_trader", Nini);
			}
			if (!StoreClientListDict.storeClientDict.ContainsKey("be_seed_trader_47"))
			{
				StoreClientListDict.storeClientDict.Add("be_seed_trader_47", FortySeven);
			}
			if (!StoreClientListDict.storeClientDict.ContainsKey("be_outer_ring_assassin"))
			{
				StoreClientListDict.storeClientDict.Add("be_outer_ring_assassin", Assassin);
			}
		}

		internal static void RegisterSprites()
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)SpriteDict.Instance == (Object)null)
			{
				return;
			}
			string[] array = new string[3] { "be_nini", "be_47", "be_shop_47" };
			foreach (string text in array)
			{
				if (!Portraits.TryGetValue(text, out var value) || (Object)(object)value == (Object)null)
				{
					Sprite modSprite = CustomUIManager.Instance.GetModSprite("brewing_expansion", text);
					if ((Object)(object)modSprite == (Object)null)
					{
						continue;
					}
					value = Sprite.Create(modSprite.texture, modSprite.rect, new Vector2(0.5f, 0.5f), 100f);
					((Object)value).name = text;
					Portraits[text] = value;
				}
				if (!SpriteDict.Instance.spriteDictionary.ContainsKey(text))
				{
					SpriteDict.Instance.spriteDictionary.Add(text, value);
				}
			}
			RegisterFactories();
		}

		internal static void Loaded()
		{
			FormalReputation.Ensure();
			RegisterSprites();
			RegisterPhone();
			NiniCertificates.Retry();
		}

		internal static void Remove()
		{
			PhoneClientList.DialogFuncs.Remove("brewing_expansion.nini");
			foreach (KeyValuePair<string, Sprite> portrait in Portraits)
			{
				if ((Object)(object)SpriteDict.Instance != (Object)null && SpriteDict.Instance.spriteDictionary.TryGetValue(portrait.Key, out var value) && (Object)(object)value == (Object)(object)portrait.Value)
				{
					SpriteDict.Instance.spriteDictionary.Remove(portrait.Key);
				}
			}
			foreach (Sprite value5 in Portraits.Values)
			{
				if ((Object)(object)value5 != (Object)null)
				{
					Object.Destroy((Object)(object)value5);
				}
			}
			Portraits.Clear();
			if (StoreClientListDict.storeClientDict.TryGetValue("be_weekly_trader", out var value2) && value2 == new Func<StoreClient>(Nini))
			{
				StoreClientListDict.storeClientDict.Remove("be_weekly_trader");
			}
			if (StoreClientListDict.storeClientDict.TryGetValue("be_seed_trader_47", out var value3) && value3 == new Func<StoreClient>(FortySeven))
			{
				StoreClientListDict.storeClientDict.Remove("be_seed_trader_47");
			}
			if (StoreClientListDict.storeClientDict.TryGetValue("be_outer_ring_assassin", out var value4) && value4 == new Func<StoreClient>(Assassin))
			{
				StoreClientListDict.storeClientDict.Remove("be_outer_ring_assassin");
			}
		}

		internal static void RegisterPhone()
		{
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Expected O, but got Unknown
			//IL_00f9: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			PlayerStore instance = PlayerStore.Instance;
			if (instance?.PhoneClientDict == null)
			{
				return;
			}
			PhoneClientList.DialogFuncs["brewing_expansion.nini"] = (StorePhoneClient _) => PhoneDialogue();
			if (instance.PhoneClientDict.TryGetValue(4747L, out var value))
			{
				if (value == null || (value.dialogFuncId != "brewing_expansion.nini" && (!(value.dialogFuncId == "BrewingExpansion.Nini") || !(value.locID == "BrewingExpansion.Nini") || (int)value.phoneClientType != 0)))
				{
					return;
				}
			}
			else
			{
				if (!instance.FindAllItem(true).Any((GameItem i) => i.identifier == "be_nini_card" && GeneralHelper.IsItemOwned(i)))
				{
					return;
				}
				Dictionary<long, StorePhoneClient> phoneClientDict = instance.PhoneClientDict;
				long key = 4747L;
				StorePhoneClient val = new StorePhoneClient
				{
					phoneClientType = (PhoneClientType)0,
					phoneState = (PhoneState)4,
					dialedBefore = true
				};
				value = val;
				phoneClientDict.Add(key, val);
			}
			value.dialogFuncId = "brewing_expansion.nini";
			value.locID = "brewing_expansion.nini";
			value.displayName = ModText.Get("be_nini_name");
			StorePhoneClient val2 = value;
			if (val2.alias == null)
			{
				val2.alias = new List<string>();
			}
			if (!value.alias.Contains("be_weekly_trader"))
			{
				value.alias.Add("be_weekly_trader");
			}
		}

		internal static Dialogue PhoneDialogue()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			PlayerStore store = PlayerStore.Instance;
			int day = StoreStation.Instance.dayCounter;
			string name = ModText.Get("be_nini_name");
			Dialogue obj = new Dialogue().SetText(name, ModText.Get("be_nini_phone_greeting"));
			Dialogue reply = new Dialogue().SetText(name, "");
			Dialogue nextDialogue = new Dialogue().SetText(name, ModText.Get("be_nini_phone_goodbye"));
			string text = ModText.Get("be_nini_phone_request");
			string text2 = ModText.Get("be_nini_phone_end");
			bool requested = false;
			obj.AddChoice(DirectoryMaster.Item("$", true), text, "", new Dialogue().SetText("PLAYER", text).SetNextDialogue(reply), (Action)delegate
			{
				if (store == PlayerStore.Instance)
				{
					int num = day - ModHelper.GetModDataInt("brewing_expansion", "nini_last_call", -999);
					if (!Open)
					{
						reply.SetText(name, ModText.Get("be_nini_phone_closed"));
					}
					else if (num < 5)
					{
						reply.SetText(name, CallCooldown(5 - num));
					}
					else if (!store.FindAllItem(true).Any((GameItem i) => i.identifier == "be_nini_card" && GeneralHelper.IsItemOwned(i)))
					{
						reply.SetText(name, ModText.Get("be_nini_phone_missing_card"));
					}
					else
					{
						requested = true;
						reply.SetText(name, ModText.Get("be_nini_phone_accepted"));
					}
				}
			}, (Func<bool>)null);
			obj.AddChoice(DirectoryMaster.Item("$", true), text2, "", new Dialogue().SetText("PLAYER", text2).SetNextDialogue(nextDialogue), (Action)null, (Func<bool>)null);
			reply.SetEndAction((Action)delegate
			{
				if (requested && store == PlayerStore.Instance && StoreStation.Instance.dayCounter == day && Open && day - ModHelper.GetModDataInt("brewing_expansion", "nini_last_call", -999) >= 5 && store.FindAllItem(true).Any((GameItem i) => i.identifier == "be_nini_card" && GeneralHelper.IsItemOwned(i)) && QueueNini(store.storeClientManager))
				{
					ModHelper.SetModDataInt("brewing_expansion", "nini_last_call", day);
				}
			});
			return obj;
		}

		internal static StoreClient Nini()
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			StoreClient client = StoreClientList.CreateUpperLowerVisitorLuxury();
			client.identifier = "be_weekly_trader";
			client.displayName = ModText.Get("be_nini_name");
			client.spriteName = "be_nini";
			client.possibleSprites = new List<string> { "be_nini" };
			client.bubbleColor = "blue";
			client.clientIntent = (ClientIntent)1;
			client.clientBuyingTagList.Clear();
			client.clientBuyingTagList.Add("SUBSTANCE");
			client.clientItemFeatureBuying.Clear();
			client.clientNegociationCheck.Clear();
			client.clientBuyingIdList.Clear();
			client.clientBlackTagList.Clear();
			client.clientBlackIdList.Clear();
			client.mainDialogue = new Dialogue().SetText(client.displayName, ModText.Get("be_nini_trade_greeting"));
			client.acceptDealDialogue = new Dialogue().SetText(client.displayName, ModText.Get("be_nini_trade_accepted"));
			client.allDoneDialogue = new Dialogue().SetText(client.displayName, ModText.Get("be_nini_trade_done"));
			client.clientFaction = "BE_FACTION_OUTER_RING";
			client.SetBudget(FormalReputation.NiniBudget);
			client.clientCash = FormalReputation.NiniBudget;
			client.useClientBudget = true;
			client.isIntroduced = true;
			client.onItemBought = delegate(GameItem item, int _)
			{
				NiniCertificates.Sold(item, client);
			};
			client.OnArrival = delegate
			{
				RegisterSprites();
				NiniCertificates.Retry(client);
			};
			ModHelper.SetClientSource(client, "brewing_expansion");
			return client;
		}

		internal static StoreClient FortySeven()
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			StoreClient val = StoreClientList.CreateUpperLowerVisitorLuxury();
			val.identifier = "be_seed_trader_47";
			val.displayName = ModText.Get("be_forty_seven_name");
			val.spriteName = "be_47";
			val.possibleSprites = new List<string> { "be_47" };
			val.clientIntent = (ClientIntent)2;
			val.isIntroduced = true;
			val.clientFaction = "BE_FACTION_OUTER_RING";
			val.bubbleColor = "blue";
			val.mainDialogue = new Dialogue().SetText(val.displayName, ModText.Get("be_forty_seven_greeting"));
			val.acceptDealDialogue = new Dialogue().SetText(val.displayName, ModText.Get("be_forty_seven_trade_accepted"));
			val.allDoneDialogue = new Dialogue().SetText(val.displayName, ModText.Get("be_forty_seven_trade_done"));
			val.OnArrival = StockSeeds;
			val.OnDismiss = DismissSeedStock;
			ModHelper.SetClientSource(val, "brewing_expansion");
			return val;
		}

		internal static StoreClient Assassin()
		{
			StoreClient val = StoreClientListMorning.CreateULHitman();
			val.identifier = "be_outer_ring_assassin";
			val.clientFaction = "BE_FACTION_OUTER_RING";
			val.displayName = ModText.Get("be_assassin_name");
			val.mainDialogue.SetText(val.displayName, ModText.Get("be_assassin_greeting"));
			ModHelper.SetClientSource(val, "brewing_expansion");
			return val;
		}

		internal static int CertificateLevel()
		{
			PlayerStore instance = PlayerStore.Instance;
			return (from i in ((instance != null) ? instance.FindAllItem(true) : null)?.Where((Func<GameItem, bool>)GeneralHelper.IsItemOwned)
				select Array.IndexOf(BrewingCertificates.Ids, i.identifier) + 1).DefaultIfEmpty().Max() ?? 0;
		}

		internal static int SeedCount()
		{
			int num = (new int[4] { 3, 7, 9, 13 })[CertificateLevel()] + FormalReputation.SeedCountModifier;
			if (FormalTraits.Has("be_seed_aversion"))
			{
				num -= 2;
			}
			if (PlayerStore.Instance.FindAllItem(true).Any((GameItem i) => i.identifier == "be_xuxu_badge" && GeneralHelper.IsItemOwned(i)))
			{
				num++;
			}
			return Math.Max(0, num);
		}

		internal static void StockSeeds()
		{
			RegisterSprites();
			if (!ModEntry.Enabled || !Open || FormalVisitorPersistence.ResumeStock())
			{
				return;
			}
			int dayCounter = StoreStation.Instance.dayCounter;
			if (ModHelper.GetModDataInt("brewing_expansion", "47_stock_day", -1) == dayCounter)
			{
				return;
			}
			PlayerStore instance = PlayerStore.Instance;
			Random random = new Random();
			int num = SeedCount();
			int num2 = ((ModHelper.GetModDataInt("brewing_expansion", "47_stock_count_day", -1) == dayCounter) ? ModHelper.GetModDataInt("brewing_expansion", "47_stock_count", 0) : 0);
			for (int i = num2; i < num; i++)
			{
				if (instance != PlayerStore.Instance)
				{
					return;
				}
				int modDataInt = ModHelper.GetModDataInt("brewing_expansion", "longing_misses", 0);
				int modDataInt2 = ModHelper.GetModDataInt("brewing_expansion", "afterglow_misses", 0);
				int level = CertificateLevel();
				(int, int, int) tuple = SeedOdds(modDataInt, modDataInt2, level);
				int num3 = random.Next(10000);
				int num4 = ((num3 >= tuple.Item1) ? ((num3 < tuple.Item1 + 1500) ? 1 : ((num3 < tuple.Item1 + 1500 + tuple.Item2) ? 2 : 3)) : 0);
				int num5 = num4 switch
				{
					2 => 12 + random.Next(2), 
					1 => 9 + random.Next(3), 
					0 => random.Next(9), 
					_ => 14 + random.Next(2), 
				};
				GameItem val = DirectoryMaster.Item(BrewingSeeds.All[num5].Id, false);
				val.EnableTag("BE_47_STOCK", false);
				instance.AddDirectSellingItemToTable(val, false, false, false, 0);
				if (val.parentInventory == null)
				{
					if (MachineConnections.Live(val))
					{
						val.Destroy();
					}
				}
				else
				{
					num2++;
					ModHelper.SetModDataInt("brewing_expansion", "47_stock_count_day", dayCounter);
					ModHelper.SetModDataInt("brewing_expansion", "47_stock_count", num2);
					ModHelper.SetModDataInt("brewing_expansion", "longing_misses", (num4 < 2) ? Math.Min(1000, modDataInt + 1) : 0);
					ModHelper.SetModDataInt("brewing_expansion", "afterglow_misses", (num4 != 3) ? Math.Min(1000, modDataInt2 + 1) : 0);
				}
			}
			HashSet<string> owned = new HashSet<string>(from val2 in ((IEnumerable<GameItem>)instance.FindAllItem(true)).Where((Func<GameItem, bool>)GeneralHelper.IsItemOwned)
				select val2.identifier);
			RemainingItems.Definition[] array = RemainingItems.All.Where((RemainingItems.Definition d) => d.Kind == "Recipe" && !owned.Contains(d.Id)).ToArray();
			if (array.Length != 0 && ModHelper.GetModDataInt("brewing_expansion", "47_recipe_day", -1) != dayCounter)
			{
				StockDocument(array[random.Next(array.Length)].Id, "47_recipe_day", instance, dayCounter);
			}
			if (!owned.Contains("be_brewing_manual") && ModHelper.GetModDataInt("brewing_expansion", "47_manual_day", -1) != dayCounter)
			{
				StockDocument("be_brewing_manual", "47_manual_day", instance, dayCounter);
			}
			if (num2 == num)
			{
				ModHelper.SetModDataInt("brewing_expansion", "47_stock_day", dayCounter);
			}
		}

		private static void StockDocument(string id, string receipt, PlayerStore store, int day)
		{
			GameItem val = DirectoryMaster.Item(id, false);
			val.EnableTag("BE_47_STOCK", false);
			store.AddDirectSellingItemToTable(val, false, false, false, 0);
			if (val.parentInventory != null)
			{
				ModHelper.SetModDataInt("brewing_expansion", receipt, day);
			}
			else if (MachineConnections.Live(val))
			{
				val.Destroy();
			}
		}

		private static void DismissSeedStock()
		{
			if (PlayerStore.Instance == null)
			{
				return;
			}
			EmporiumEntry instance = EmporiumEntry.Instance;
			GameGridInventory val = ((instance != null) ? instance.frontInvinvElement : null);
			if (val == null)
			{
				return;
			}
			GameItem[] array = ((GameInventory)val).childItems.Where((GameItem i) => i.IsTag("BE_47_STOCK") && !GeneralHelper.IsItemOwned(i)).ToArray();
			foreach (GameItem val2 in array)
			{
				if (MachineConnections.Live(val2))
				{
					val2.Destroy();
				}
			}
			ModHelper.SetModDataInt("brewing_expansion", "47_stock_day", StoreStation.Instance.dayCounter);
		}

		internal static (int, int, int) SeedOdds(int longingMisses, int afterglowMisses, int level)
		{
			int num = ((level >= 3) ? 16 : ((level >= 2) ? 13 : 10));
			int num2 = (300 + Math.Min(Math.Max(0, longingMisses) / 10, 5) * 100) * num / 10;
			int num3 = (100 + Math.Min(Math.Max(0, afterglowMisses) / 20, 8) * 50) * num / 10;
			int num4 = 8500 - num2 - num3;
			if (num4 < 5000)
			{
				num2 = num2 * 3500 / (num2 + num3);
				num3 = 3500 - num2;
				num4 = 5000;
			}
			return (num4, num2, num3);
		}

		internal static bool QueueNini(StoreClientManager manager)
		{
			if (ModEntry.Enabled && Open && manager != null && !manager.clientStack.Any((StoreClient c) => c.identifier == "be_weekly_trader"))
			{
				StoreClientInstance currentClientInstance = PlayerStore.Instance.currentClientInstance;
				if (!(((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.identifier) == "be_weekly_trader"))
				{
					manager.AddClient(Nini());
					return true;
				}
			}
			return false;
		}

		internal static void Generated(StoreClientManager manager)
		{
			Loaded();
			if (FormalReputation.Tier <= -5)
			{
				StoreClientInstance currentClientInstance = PlayerStore.Instance.currentClientInstance;
				if (((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.identifier) != "be_outer_ring_assassin" && !manager.clientStack.Any((StoreClient c) => c.identifier == "be_outer_ring_assassin"))
				{
					StoreClient val = Assassin();
					manager.AddClient(val);
					manager.clientStack.Remove(val);
					manager.clientStack.Insert(0, val);
				}
			}
		}

		internal static void EnsureFortySeven(PlayerStore store, bool allowNew = true)
		{
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Invalid comparison between Unknown and I4
			if (!ModEntry.Enabled || store != PlayerStore.Instance || store?.storeClientManager == null)
			{
				return;
			}
			List<StoreClient> clientStack = store.storeClientManager.clientStack;
			StoreClientInstance currentClientInstance = store.currentClientInstance;
			bool flag = ((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.identifier) == "be_seed_trader_47";
			int num = 0;
			while (num < clientStack.Count)
			{
				if (clientStack[num]?.identifier != "be_seed_trader_47")
				{
					num++;
					continue;
				}
				if (flag)
				{
					clientStack.RemoveAt(num);
					continue;
				}
				flag = true;
				num++;
			}
			int dayCounter = StoreStation.Instance.dayCounter;
			if (flag)
			{
				ModHelper.SetModDataInt("brewing_expansion", "47_queue_day", dayCounter);
			}
			else if (allowNew && (Open || (int)store.storeState == 2) && ModHelper.GetModDataInt("brewing_expansion", "47_queue_day", -1) != dayCounter && ModHelper.GetModDataInt("brewing_expansion", "47_stock_day", -1) != dayCounter && FormalReputation.Tier > -3 && ShowcaseHelper.IsShowcaseContainItemById("be_brewery_sign"))
			{
				store.storeClientManager.AddClient(FortySeven());
				if (clientStack.Any((StoreClient c) => c?.identifier == "be_seed_trader_47"))
				{
					ModHelper.SetModDataInt("brewing_expansion", "47_queue_day", dayCounter);
				}
			}
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "GetNextClient")]
	internal static class FormalVisitorHours
	{
		private static bool Prefix(PlayerStore __instance)
		{
			if (!ModEntry.Enabled || FormalVisitors.Open)
			{
				return true;
			}
			return !FormalVisitors.IsDayClient((__instance.storeClientManager?.clientStack?.FirstOrDefault())?.identifier);
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "HandleFutureClientQueue")]
	internal static class FormalVisitorAfterFutureQueue
	{
		private static void Postfix(PlayerStore __instance, bool __runOriginal)
		{
			if (__runOriginal)
			{
				FormalEvents.QueueDailyVisitor(__instance);
			}
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "OnShutterOpened")]
	internal static class FormalVisitorAfterOpening
	{
		private static void Postfix(PlayerStore __instance, bool __runOriginal)
		{
			if (__runOriginal)
			{
				FormalEvents.QueueDailyVisitor(__instance);
			}
		}
	}
	[HarmonyPatch(typeof(StorePhoneClient), "GetLocalizedDisplayName")]
	internal static class FormalNiniContactName
	{
		private static bool Prefix(StorePhoneClient __instance, ref string __result)
		{
			if (!ModEntry.Enabled || __instance.dialogFuncId != "brewing_expansion.nini")
			{
				return true;
			}
			__result = ModText.Get("be_nini_name");
			return false;
		}
	}
	[HarmonyPatch]
	internal static class FormalNiniCategories
	{
		private static IEnumerable<MethodBase> TargetMethods()
		{
			MethodInfo methodInfo = AccessTools.DeclaredMethod(typeof(StoreClient), "IsClientBuyingThisItem", new Type[1] { typeof(GameItem) }, (Type[])null);
			MethodInfo detailed = AccessTools.DeclaredMethod(typeof(StoreClient), "IsClientBuyingThisItem", new Type[2]
			{
				typeof(GameItem),
				typeof(bool)
			}, (Type[])null);
			if (methodInfo == null || methodInfo.ReturnType != typeof(bool) || methodInfo.IsStatic)
			{
				throw new MissingMethodException("Unsupported StoreClient.IsClientBuyingThisItem(GameItem) interface.");
			}
			yield return methodInfo;
			if (detailed != null)
			{
				if (detailed.ReturnType != typeof(bool) || detailed.IsStatic)
				{
					throw new MissingMethodException("Unsupported StoreClient.IsClientBuyingThisItem(GameItem,bool) interface.");
				}
				yield return detailed;
			}
		}

		private static void Postfix(StoreClient __instance, GameItem __0, ref bool __result)
		{
			if ((ModEntry.Enabled & __result) && __instance.identifier == "be_weekly_trader")
			{
				__result = __0 != null && (__0.IsGameItemType("ALCOHOL") || __0.IsTag("CIGARETTE_TAG") || __0.IsGameItemType("CIGARETTE") || __0.IsGameItemType("TOBACCO"));
			}
		}
	}
	public static class IncubatorRecipeRegistry
	{
		private sealed class Registration : IDisposable
		{
			internal readonly string Owner;

			internal readonly IncubatorRecipes.Recipe Recipe;

			internal Registration(string owner, IncubatorRecipes.Recipe recipe)
			{
				Owner = owner;
				Recipe = recipe;
			}

			public void Dispose()
			{
				if (Recipes.TryGetValue(Recipe.SeedId, out var value) && value == this)
				{
					Recipes.Remove(Recipe.SeedId);
				}
			}
		}

		private static readonly Dictionary<string, Registration> Recipes = new Dictionary<string, Registration>(StringComparer.Ordinal);

		public static IDisposable Register(string ownerModId, string seedId, string productId, int quantity, Func<GameItem> create)
		{
			if (string.IsNullOrWhiteSpace(ownerModId) || string.IsNullOrWhiteSpace(seedId) || string.IsNullOrWhiteSpace(productId))
			{
				throw new ArgumentException("Recipe owner, seed and product identities are required.");
			}
			if (quantity < 1 || quantity > 1000)
			{
				throw new ArgumentOutOfRangeException("quantity");
			}
			if (create == null)
			{
				throw new ArgumentNullException("create");
			}
			if (BrewingSeeds.Find(seedId) != null || IncubatorRecipes.Native.Any((IncubatorRecipes.Recipe r) => r.SeedId == seedId) || Recipes.ContainsKey(seedId))
			{
				throw new InvalidOperationException("An incubator recipe already owns seed " + seedId);
			}
			Registration registration = new Registration(ownerModId, new IncubatorRecipes.Recipe(seedId, productId, quantity, create));
			Recipes.Add(seedId, registration);
			return registration;
		}

		internal static IncubatorRecipes.Recipe Find(string seedId)
		{
			if (seedId == null || !Recipes.TryGetValue(seedId, out var value) || !ModLoader.IsModEnabled(value.Owner))
			{
				return null;
			}
			return value.Recipe;
		}

		internal static bool AcceptsProduct(string id)
		{
			return Recipes.Values.Any((Registration r) => r.Recipe.ProductId == id && ModLoader.IsModEnabled(r.Owner));
		}
	}
	internal static class IncubatorRecipes
	{
		internal sealed class Recipe
		{
			internal readonly string SeedId;

			internal readonly string ProductId;

			internal readonly int Quantity;

			internal readonly Func<GameItem> Create;

			internal Recipe(string seed, string product, int quantity, Func<GameItem> create)
			{
				SeedId = seed;
				ProductId = product;
				Quantity = quantity;
				Create = create;
			}
		}

		internal static readonly Recipe[] Native = new Recipe[7]
		{
			new Recipe("nutrifruit_seed", "nutrifruit", 2, (Func<GameItem>)PreBuiltItemHelper.CreateNutrifruitNormal),
			new Recipe("kotton_seed", "kotton_fiber", 2, (Func<GameItem>)PreBuiltItemHelper.CreateKotton),
			new Recipe("dream_cap_seed", "dream_cap", 2, () => PreBuiltItemHelper.CreateDreamCap(false)),
			new Recipe("coffee_bean", "coffee_bean", 2, () => DirectoryMaster.Item("coffee_bean", true)),
			new Recipe("fillerweed_seed", "fillerweed", 2, (Func<GameItem>)PreBuiltItemHelper.CreateFillerweed),
			new Recipe("bloomberry_seed", "wine_berry", 2, CreateBerry),
			new Recipe("hydroreed_seed", "hydroreed_filter", 2, () => DirectoryMaster.Item("hydroreed_filter", true))
		};

		internal static Recipe Find(GameItem seed)
		{
			if (seed == null)
			{
				return null;
			}
			BrewingSeeds.Definition own = BrewingSeeds.Find(seed.identifier);
			if (own != null)
			{
				return new Recipe(own.Id, own.Fruit, 1, () => DirectoryMaster.Item(own.Fruit, true));
			}
			return (seed.IsTag("SEED_ITEM_TAG") ? Array.Find(Native, (Recipe recipe) => recipe.SeedId == seed.identifier) : null) ?? IncubatorRecipeRegistry.Find(seed.identifier);
		}

		internal static bool AcceptsProduct(GameItem item)
		{
			if (item != null)
			{
				BrewingFruits.Definition definition = BrewingFruits.Find(item.identifier);
				if ((definition == null || !definition.Concentratable) && !Native.Any((Recipe recipe) => recipe.ProductId == item.identifier))
				{
					return IncubatorRecipeRegistry.AcceptsProduct(item.identifier);
				}
				return true;
			}
			return false;
		}

		private static GameItem CreateBerry()
		{
			GameItem obj = DirectoryMaster.Item("wine_berry", true);
			WineHelper.SetBerryCondition(obj, 2, 0);
			return obj;
		}
	}
	internal static class ItemHitArea
	{
		internal static void Apply(GameItem item)
		{
			if (item == null || ((BarrelWine.Find(item.identifier) == null || BarrelWine.Recipe(item) == null) && (!item.useSpriteFromMod || item.spriteAtlasPath != "brewing_expansion")))
			{
				return;
			}
			bool flag = RemainingItems.Find(item.identifier) != null || Array.IndexOf(FormalMusic.Ids, item.identifier) >= 0 || item.identifier == "be_liunian_ager" || item.identifier == "be_incubator" || BrewingSeeds.Find(item.identifier) != null || Array.IndexOf(BrewingCertificates.Ids, item.identifier) >= 0 || item.identifier == "be_wine_printer" || item.identifier == "be_xuxu_connection_pliers" || item.identifier == "be_large_brewing_barrel" || BarrelWine.Recipe(item) != null || WaterContainers.Find(item) != null || item.identifier == "be_xuxu_water_bottle_printer" || item.identifier == "be_fruit_pulp_concentrator" || item.identifier == "be_brewing_fruit_pulp" || BrewingFruits.Find(item.identifier) != null;
			if (!flag)
			{
				FigurineDefinition[] all = Figurines.All;
				foreach (FigurineDefinition figurineDefinition in all)
				{
					if (item.identifier == figurineDefinition.Id)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				GameItemElement val = (GameItemElement)(object)((item is GameItemElement) ? item : null);
				object obj;
				if (val == null)
				{
					obj = null;
				}
				else
				{
					TreeNodeRender background = val.background;
					obj = ((background != null) ? background.image : null);
				}
				if ((Object)obj != (Object)null)
				{
					val.background.image.alphaHitTestMinimumThreshold = 0f;
				}
				object obj2;
				if (val == null)
				{
					obj2 = null;
				}
				else
				{
					TreeNodeRender fakeBackground = val.fakeBackground;
					obj2 = ((fakeBackground != null) ? fakeBackground.image : null);
				}
				if ((Object)obj2 != (Object)null)
				{
					val.fakeBackground.image.alphaHitTestMinimumThreshold = 0f;
				}
			}
		}
	}
	internal static class ItemIdentity
	{
		internal static readonly Dictionary<string, string> LegacyIds = new Dictionary<string, string>
		{
			{ "chili_fruit", "be_chili_fruit" },
			{ "rice_fruit", "be_rice_fruit" },
			{ "wheat_fruit", "be_wheat_fruit" },
			{ "wormwood_fruit", "be_wormwood_fruit" },
			{ "sichuan_pepper_fruit", "be_sichuan_pepper_fruit" },
			{ "lemon_fruit", "be_lemon_fruit" },
			{ "yellow_grape_fruit", "be_yellow_grape_fruit" },
			{ "sugarcane_fruit", "be_sugarcane_fruit" },
			{ "cucumber_fruit", "be_cucumber_fruit" },
			{ "lychee_fruit", "be_lychee_fruit" },
			{ "jasmine_fruit", "be_jasmine_fruit" },
			{ "green_grape_fruit", "be_green_grape_fruit" },
			{ "osmanthus_fruit", "be_osmanthus_fruit" },
			{ "bayberry_fruit", "be_bayberry_fruit" },
			{ "void_plum_fruit", "be_void_plum_fruit" },
			{ "stardust_berry_fruit", "be_stardust_berry_fruit" },
			{ "chili_seed", "be_chili_seed" },
			{ "rice_seed", "be_rice_seed" },
			{ "wheat_seed", "be_wheat_seed" },
			{ "wormwood_seed", "be_wormwood_seed" },
			{ "sichuan_pepper_seed", "be_sichuan_pepper_seed" },
			{ "lemon_seed", "be_lemon_seed" },
			{ "yellow_grape_seed", "be_yellow_grape_seed" },
			{ "sugarcane_seed", "be_sugarcane_seed" },
			{ "cucumber_seed", "be_cucumber_seed" },
			{ "lychee_seed", "be_lychee_seed" },
			{ "jasmine_seed", "be_jasmine_seed" },
			{ "green_grape_seed", "be_green_grape_seed" },
			{ "osmanthus_seed", "be_osmanthus_seed" },
			{ "bayberry_seed", "be_bayberry_seed" },
			{ "void_plum_seed", "be_void_plum_seed" },
			{ "stardust_berry_seed", "be_stardust_berry_seed" },
			{ "chili_wine", "be_chili_wine" },
			{ "rice_wine", "be_rice_wine" },
			{ "wheat_wine", "be_wheat_wine" },
			{ "wormwood_wine", "be_wormwood_wine" },
			{ "sichuan_pepper_wine", "be_sichuan_pepper_wine" },
			{ "lemon_wine", "be_lemon_wine" },
			{ "yellow_grape_wine", "be_yellow_grape_wine" },
			{ "sugarcane_wine", "be_sugarcane_wine" },
			{ "cucumber_wine", "be_cucumber_wine" },
			{ "lychee_wine", "be_lychee_wine" },
			{ "jasmine_wine", "be_jasmine_wine" },
			{ "green_grape_wine", "be_green_grape_wine" },
			{ "osmanthus_wine", "be_osmanthus_wine" },
			{ "bayberry_wine", "be_bayberry_wine" },
			{ "void_plum_wine", "be_void_plum_wine" },
			{ "stardust_berry_wine", "be_stardust_berry_wine" },
			{ "gusui_wine", "be_gusui_wine" },
			{ "chimadu_wine", "be_chimadu_wine" },
			{ "suantian_banzhan_wine", "be_suantian_banzhan_wine" },
			{ "bipo_liuguang_wine", "be_bipo_liuguang_wine" },
			{ "banzhan_suanku_wine", "be_banzhan_suanku_wine" },
			{ "daopo_shuanggan_wine", "be_daopo_shuanggan_wine" },
			{ "maigan_niang_wine", "be_maigan_niang_wine" },
			{ "lychee_rice_wine", "be_lychee_rice_wine" },
			{ "lychee_wormwood_wine", "be_lychee_wormwood_wine" },
			{ "lychee_chili_wine", "be_lychee_chili_wine" },
			{ "moxiang_miniang_wine", "be_moxiang_miniang_wine" },
			{ "moning_qingxiao_wine", "be_moning_qingxiao_wine" },
			{ "huazhe_liunian_wine", "be_huazhe_liunian_wine" },
			{ "bimai_liujin_wine", "be_bimai_liujin_wine" },
			{ "cuiyu_qingzhu_wine", "be_cuiyu_qingzhu_wine" },
			{ "zilu_jinfeng_wine", "be_zilu_jinfeng_wine" },
			{ "moli_qingti_wine", "be_moli_qingti_wine" },
			{ "guixin_chisu_wine", "be_guixin_chisu_wine" },
			{ "jianghen_chenzhe_wine", "be_jianghen_chenzhe_wine" },
			{ "zhuolei_xiandan_wine", "be_zhuolei_xiandan_wine" },
			{ "cuoji_shu_wine", "be_cuoji_shu_wine" },
			{ "chenyuan_miniang_wine", "be_chenyuan_miniang_wine" },
			{ "konggu_kuai_wine", "be_konggu_kuai_wine" },
			{ "xingluo_maitian_wine", "be_xingluo_maitian_wine" },
			{ "lengxing_ruzhan_wine", "be_lengxing_ruzhan_wine" },
			{ "yexiang_chenji_wine", "be_yexiang_chenji_wine" },
			{ "xingzhui_qingteng_wine", "be_xingzhui_qingteng_wine" },
			{ "xingchen_wange_wine", "be_xingchen_wange_wine" },
			{ "qingxi_liuchun_wine", "be_qingxi_liuchun_wine" },
			{ "wujian_liubai_wine", "be_wujian_liubai_wine" },
			{ "jiangyuan_xiangfeng_wine", "be_jiangyuan_xiangfeng_wine" },
			{ "qingti_weifeng_wine", "be_qingti_weifeng_wine" },
			{ "mobai_qinglu_wine", "be_mobai_qinglu_wine" },
			{ "mijin_wanzhao_wine", "be_mijin_wanzhao_wine" },
			{ "wine_yeast_hentai", "be_wine_yeast_hentai" }
		};

		internal static readonly Dictionary<string, string> LegacySprites = new Dictionary<string, string>
		{
			{ "transfer_line", "be_xuxu_transfer_line" },
			{ "chili_fruit", "be_chili_fruit" },
			{ "rice_fruit", "be_rice_fruit" },
			{ "wheat_fruit", "be_wheat_fruit" },
			{ "wormwood_fruit", "be_wormwood_fruit" },
			{ "sichuan_pepper_fruit", "be_sichuan_pepper_fruit" },
			{ "lemon_fruit", "be_lemon_fruit" },
			{ "yellow_grape_fruit", "be_yellow_grape_fruit" },
			{ "sugarcane_fruit", "be_sugarcane_fruit" },
			{ "cucumber_fruit", "be_cucumber_fruit" },
			{ "lychee_fruit", "be_lychee_fruit" },
			{ "jasmine_fruit", "be_jasmine_fruit" },
			{ "green_grape_fruit", "be_green_grape_fruit" },
			{ "osmanthus_fruit", "be_osmanthus_fruit" },
			{ "bayberry_fruit", "be_bayberry_fruit" },
			{ "void_plum_fruit", "be_void_plum_fruit" },
			{ "stardust_berry_fruit", "be_stardust_berry_fruit" },
			{ "chili_wine", "be_chili_wine" },
			{ "rice_wine", "be_rice_wine" },
			{ "wheat_wine", "be_wheat_wine" },
			{ "wormwood_wine", "be_wormwood_wine" },
			{ "sichuan_pepper_wine", "be_sichuan_pepper_wine" },
			{ "lemon_wine", "be_lemon_wine" },
			{ "yellow_grape_wine", "be_yellow_grape_wine" },
			{ "sugarcane_wine", "be_sugarcane_wine" },
			{ "cucumber_wine", "be_cucumber_wine" },
			{ "lychee_wine", "be_lychee_wine" },
			{ "jasmine_wine", "be_jasmine_wine" },
			{ "green_grape_wine", "be_green_grape_wine" },
			{ "osmanthus_wine", "be_osmanthus_wine" },
			{ "bayberry_wine", "be_bayberry_wine" },
			{ "void_plum_wine", "be_void_plum_wine" },
			{ "stardust_berry_wine", "be_stardust_berry_wine" },
			{ "gusui_wine", "be_gusui_wine" },
			{ "chimadu_wine", "be_chimadu_wine" },
			{ "suantian_banzhan_wine", "be_suantian_banzhan_wine" },
			{ "bipo_liuguang_wine", "be_bipo_liuguang_wine" },
			{ "banzhan_suanku_wine", "be_banzhan_suanku_wine" },
			{ "daopo_shuanggan_wine", "be_daopo_shuanggan_wine" },
			{ "maigan_niang_wine", "be_maigan_niang_wine" },
			{ "lychee_rice_wine", "be_lychee_rice_wine" },
			{ "lychee_wormwood_wine", "be_lychee_wormwood_wine" },
			{ "lychee_chili_wine", "be_lychee_chili_wine" },
			{ "moxiang_miniang_wine", "be_moxiang_miniang_wine" },
			{ "moning_qingxiao_wine", "be_moning_qingxiao_wine" },
			{ "huazhe_liunian_wine", "be_huazhe_liunian_wine" },
			{ "bimai_liujin_wine", "be_bimai_liujin_wine" },
			{ "cuiyu_qingzhu_wine", "be_cuiyu_qingzhu_wine" },
			{ "zilu_jinfeng_wine", "be_zilu_jinfeng_wine" },
			{ "moli_qingti_wine", "be_moli_qingti_wine" },
			{ "guixin_chisu_wine", "be_guixin_chisu_wine" },
			{ "jianghen_chenzhe_wine", "be_jianghen_chenzhe_wine" },
			{ "zhuolei_xiandan_wine", "be_zhuolei_xiandan_wine" },
			{ "cuoji_shu_wine", "be_cuoji_shu_wine" },
			{ "chenyuan_miniang_wine", "be_chenyuan_miniang_wine" },
			{ "konggu_kuai_wine", "be_konggu_kuai_wine" },
			{ "xingluo_maitian_wine", "be_xingluo_maitian_wine" },
			{ "lengxing_ruzhan_wine", "be_lengxing_ruzhan_wine" },
			{ "yexiang_chenji_wine", "be_yexiang_chenji_wine" },
			{ "xingzhui_qingteng_wine", "be_xingzhui_qingteng_wine" },
			{ "xingchen_wange_wine", "be_xingchen_wange_wine" },
			{ "qingxi_liuchun_wine", "be_qingxi_liuchun_wine" },
			{ "wujian_liubai_wine", "be_wujian_liubai_wine" },
			{ "jiangyuan_xiangfeng_wine", "be_jiangyuan_xiangfeng_wine" },
			{ "qingti_weifeng_wine", "be_qingti_weifeng_wine" },
			{ "mobai_qinglu_wine", "be_mobai_qinglu_wine" },
			{ "mijin_wanzhao_wine", "be_mijin_wanzhao_wine" },
			{ "barrel_idle", "be_large_brewing_barrel_idle" },
			{ "barrel_active", "be_large_brewing_barrel_active" },
			{ "concentrator", "be_fruit_pulp_concentrator" },
			{ "liunian", "be_liunian_ager" },
			{ "xuxu_water_bottle_printer", "be_xuxu_water_bottle_printer" },
			{ "pulp_chili_fruit", "be_pulp_chili_fruit" },
			{ "pulp_rice_fruit", "be_pulp_rice_fruit" },
			{ "pulp_wheat_fruit", "be_pulp_wheat_fruit" },
			{ "pulp_wormwood_fruit", "be_pulp_wormwood_fruit" },
			{ "pulp_sichuan_pepper_fruit", "be_pulp_sichuan_pepper_fruit" },
			{ "pulp_lemon_fruit", "be_pulp_lemon_fruit" },
			{ "pulp_yellow_grape_fruit", "be_pulp_yellow_grape_fruit" },
			{ "pulp_sugarcane_fruit", "be_pulp_sugarcane_fruit" },
			{ "pulp_cucumber_fruit", "be_pulp_cucumber_fruit" },
			{ "pulp_lychee_fruit", "be_pulp_lychee_fruit" },
			{ "pulp_jasmine_fruit", "be_pulp_jasmine_fruit" },
			{ "pulp_green_grape_fruit", "be_pulp_green_grape_fruit" },
			{ "pulp_osmanthus_fruit", "be_pulp_osmanthus_fruit" },
			{ "pulp_bayberry_fruit", "be_pulp_bayberry_fruit" },
			{ "pulp_void_plum_fruit", "be_pulp_void_plum_fruit" },
			{ "pulp_stardust_berry_fruit", "be_pulp_stardust_berry_fruit" },
			{ "pulp_wine_berry", "be_pulp_wine_berry" }
		};

		internal static string Canonical(string id)
		{
			if (id == null || !LegacyIds.TryGetValue(id, out var value))
			{
				return id;
			}
			return value;
		}

		internal static string Sprite(string key)
		{
			if (key == null || !LegacySprites.TryGetValue(key, out var value))
			{
				return key;
			}
			return value;
		}

		internal static string PulpSprite(string fruit)
		{
			return "be_pulp_" + (fruit.StartsWith("be_", StringComparison.Ordinal) ? fruit.Substring(3) : fruit);
		}

		internal static string Recipe(TagSystem state, string identifier = null)
		{
			if (state == null)
			{
				return null;
			}
			if (BarrelWine.Find(identifier) != null && state.IsTag("BE_FORMAL_WINE_" + identifier))
			{
				return identifier;
			}
			foreach (KeyValuePair<string, TagState> item in state.dict)
			{
				if (item.Key.StartsWith("BE_FORMAL_WINE_", StringComparison.Ordinal) && state.IsTag(item.Key))
				{
					string text = Canonical(item.Key.Substring("BE_FORMAL_WINE_".Length));
					if (BarrelWine.Find(text) != null)
					{
						return text;
					}
				}
			}
			return null;
		}

		private static bool Ours(SaveItemNode node)
		{
			TagSystem itemState = node.itemState;
			if (((itemState != null) ? itemState.dict : null) != null && node.itemState.dict.TryGetValue("MOD_ID_TAG", out var value) && !string.IsNullOrEmpty(value.GetString()))
			{
				return value.GetString() == "brewing_expansion";
			}
			if (node.spriteAtlasPath == "brewing_expansion")
			{
				return true;
			}
			if (node.identifier == "wine_bottle" || BarrelWine.Find(node.identifier) != null || LegacyIds.ContainsKey(node.identifier ?? ""))
			{
				return Recipe(node.itemState, node.identifier) != null;
			}
			return false;
		}

		internal static void Normalize(List<SaveItemNode> nodes)
		{
			if (!ModEntry.Enabled || nodes == null)
			{
				return;
			}
			foreach (SaveItemNode node in nodes)
			{
				if (node == null || !Ours(node))
				{
					continue;
				}
				node.identifier = Canonical(node.identifier);
				node.spritePath = Sprite(node.spritePath);
				TagSystem itemState = node.itemState;
				if (((itemState != null) ? itemState.dict : null) == null)
				{
					continue;
				}
				KeyValuePair<string, TagState>[] array = itemState.dict.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					KeyValuePair<string, TagState> keyValuePair = array[i];
					string[] array2 = new string[2] { "BE_FORMAL_WINE_", "BE_PULP_FRUIT_" };
					foreach (string text in array2)
					{
						if (keyValuePair.Key.StartsWith(text, StringComparison.Ordinal))
						{
							string text2 = keyValuePair.Key.Substring(text.Length);
							string text3 = Canonical(text2);
							if (!(text2 == text3))
							{
								itemState.dict[text + text3] = keyValuePair.Value.Clone();
								itemState.dict.Remove(keyValuePair.Key);
							}
						}
					}
				}
				string text4 = Recipe(itemState);
				if (text4 != null && itemState.dict.TryGetValue("WINE_CONTAINER_STATE", out var value) && value.GetInt() > 0)
				{
					node.identifier = text4;
				}
			}
		}

		internal static IEnumerable<string> Catalogue()
		{
			return Figurines.All.Select((FigurineDefinition d) => d.Id).Concat(WaterContainers.All.Select((WaterContainerDefinition d) => d.Id)).Concat(BrewingFruits.All.Select((BrewingFruits.Definition d) => d.Id))
				.Concat(BrewingSeeds.All.Select((BrewingSeeds.Definition d) => d.Id))
				.Concat(BrewingCertificates.Ids)
				.Concat(RemainingItems.All.Select((RemainingItems.Definition d) => d.Id))
				.Concat(FormalMusic.Ids)
				.Concat(BarrelWine.All.Select((BarrelWine.Definition d) => d.Id))
				.Concat(new string[8] { "be_xuxu_water_bottle_printer", "be_wine_printer", "be_incubator", "be_liunian_ager", "be_xuxu_connection_pliers", "be_large_brewing_barrel", "be_fruit_pulp_concentrator", "be_brewing_fruit_pulp" });
		}
	}
	[HarmonyPatch(typeof(SaveManager), "DecodeNodes")]
	internal static class OwnSavedItemIds
	{
		private static void Prefix(List<SaveItemNode> __0)
		{
			ItemIdentity.Normalize(__0);
		}
	}
	internal static class LiquidPatches
	{
		internal enum Phase
		{
			Disabled,
			WaitingForAssets,
			WaitingForLocalization,
			WaitingForPlayerLocale,
			Ready,
			Failed
		}

		private static ModEntry entry;

		private static ModLog log;

		private static LoadedMod owner;

		private static LiquidPatchGate gate;

		private static int generation;

		private static bool startupLocaleApplied;

		private static readonly List<MethodBase> installedTargets = new List<MethodBase>();

		internal static Phase Status { get; private set; }

		internal static bool Ready => Status == Phase.Ready;

		internal static int InstallCount { get; private set; }

		internal static void Enable(ModEntry instance, ModLog logger)
		{
			if (entry == null)
			{
				entry = instance;
				log = logger;
				generation++;
				Status = Phase.WaitingForAssets;
				ModHook.OnModAssetsLoaded += AssetsLoaded;
				LoadedMod val = ((IEnumerable<LoadedMod>)ModLoader.Mods).FirstOrDefault((Func<LoadedMod, bool>)((LoadedMod m) => (object)m.Instance == instance && m.IsEnabled));
				if (val != null)
				{
					AssetsLoaded(val);
				}
			}
		}

		private static void AssetsLoaded(LoadedMod loaded)
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Expected O, but got Unknown
			if (entry != null && ModEntry.Enabled && !((Object)(object)gate != (Object)null) && !Ready && Status != Phase.Failed && (object)loaded?.Instance == entry && loaded.IsEnabled)
			{
				if (loaded.HarmonyInstance == null || loaded.HarmonyInstance.Id != "mod.brewing_expansion")
				{
					Fail("Official Harmony owner is unavailable.", null);
					return;
				}
				owner = loaded;
				Status = Phase.WaitingForLocalization;
				GameObject val = new GameObject("BrewingExpansion.LiquidReadiness");
				Object.DontDestroyOnLoad((Object)val);
				gate = val.AddComponent<LiquidPatchGate>();
				gate.Begin(WaitForNativeResources(generation));
			}
		}

		private static bool Current(int token)
		{
			if (token == generation && entry != null)
			{
				return ModEntry.Enabled;
			}
			return false;
		}

		private static bool Valid<T>(AsyncOperationHandle<T> handle)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Invalid comparison between Unknown and I4
			if (handle.IsValid() && handle.IsDone)
			{
				return (int)handle.Status == 1;
			}
			return false;
		}

		private static bool PlayerLocaleApplied(Locale selected)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (startupLocaleApplied)
			{
				return true;
			}
			string text = LocHelper.ResolveStartupLocale();
			ILocalesProvider availableLocales = LocalizationSettings.AvailableLocales;
			Locale val = availableLocales.GetLocale(LocaleIdentifier.op_Implicit(text)) ?? availableLocales.GetLocale(LocaleIdentifier.op_Implicit("en"));
			if ((Object)(object)val != (Object)null)
			{
				return selected.Identifier == val.Identifier;
			}
			return false;
		}

		private static IEnumerable<Locale> RequiredLocales(Locale selected)
		{
			HashSet<Locale> seen = new HashSet<Locale>();
			Queue<Locale> pending = new Queue<Locale>();
			pending.Enqueue(selected);
			while (pending.Count != 0)
			{
				Locale locale = pending.Dequeue();
				if ((Object)(object)locale == (Object)null || !seen.Add(locale))
				{
					continue;
				}
				yield return locale;
				foreach (Locale fallback in locale.GetFallbacks())
				{
					if ((Object)(object)fallback != (Object)null)
					{
						pending.Enqueue(fallback);
					}
				}
			}
		}

		private static IEnumerator WaitForNativeResources(int token)
		{
			while (Current(token))
			{
				AsyncOperationHandle<LocalizationSettings> init = LocalizationSettings.InitializationOperation;
				if (!init.IsValid())
				{
					Fail("Native localization initialization handle is invalid.", null);
					break;
				}
				if (!init.IsDone)
				{
					yield return init;
				}
				if (!Current(token))
				{
					break;
				}
				if (!Valid<LocalizationSettings>(init))
				{
					Fail("Native localization initialization did not succeed.", init.IsValid() ? init.OperationException : null);
					break;
				}
				AsyncOperationHandle<Locale> selection = LocalizationSettings.SelectedLocaleAsync;
				if (!selection.IsValid())
				{
					Fail("Native locale selection handle is invalid.", null);
					break;
				}
				if (!selection.IsDone)
				{
					yield return selection;
				}
				if (!Current(token))
				{
					break;
				}
				if (!Valid<Locale>(selection) || (Object)(object)selection.Result == (Object)null)
				{
					Fail("Native locale selection did not succeed.", selection.IsValid() ? selection.OperationException : null);
					break;
				}
				Locale locale = selection.Result;
				if (!PlayerLocaleApplied(locale))
				{
					Status = Phase.WaitingForPlayerLocale;
					yield return null;
					continue;
				}
				Status = Phase.WaitingForLocalization;
				List<AsyncOperationHandle<StringTable>> tables = new List<AsyncOperationHandle<StringTable>>();
				bool localeChanged = false;
				foreach (Locale required in RequiredLocales(locale))
				{
					AsyncOperationHandle<StringTable> table = ((LocalizedDatabase<StringTable, StringTableEntry>)(object)LocalizationSettings.StringDatabase).GetTableAsync(TableReference.op_Implicit("Mechanic"), required);
					LocaleIdentifier identifier;
					if (!table.IsValid())
					{
						identifier = required.Identifier;
						Fail("Native Mechanic table handle is invalid for " + ((LocaleIdentifier)(ref identifier)).Code, null);
						yield break;
					}
					if (!table.IsDone)
					{
						yield return table;
					}
					if (!Current(token))
					{
						yield break;
					}
					if (LocalizationSettings.SelectedLocale != locale)
					{
						localeChanged = true;
						break;
					}
					if (!Valid<StringTable>(table) || (Object)(object)table.Result == (Object)null)
					{
						identifier = required.Identifier;
						Fail("Native Mechanic table did not load for " + ((LocaleIdentifier)(ref identifier)).Code, table.IsValid() ? table.OperationException : null);
						yield break;
					}
					tables.Add(table);
				}
				if (localeChanged)
				{
					continue;
				}
				if (!PlayerLocaleApplied(locale))
				{
					yield return null;
					continue;
				}
				if (Current(token))
				{
					if (!Valid<LocalizationSettings>(init) || tables.Count == 0 || tables.Any((AsyncOperationHandle<StringTable> t) => !Valid<StringTable>(t) || (Object)(object)t.Result == (Object)null))
					{
						Fail("Native localization resources became invalid before liquid adapter registration.", null);
						break;
					}
					startupLocaleApplied = true;
					Install(token);
				}
				break;
			}
		}

		private static void Install(int token)
		{
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Expected O, but got Unknown
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Expected O, but got Unknown
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Expected O, but got Unknown
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Expected O, but got Unknown
			//IL_01e4: Expected O, but got Unknown
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Expected O, but got Unknown
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			if (Current(token) && !Ready)
			{
				Harmony harmonyInstance = owner.HarmonyInstance;
				MethodInfo methodInfo = AccessTools.Method(typeof(WaterHelper), "AddWater", new Type[7]
				{
					typeof(GameItem),
					typeof(int),
					typeof(int),
					typeof(bool),
					typeof(int),
					typeof(int),
					typeof(bool)
				}, (Type[])null);
				MethodInfo methodInfo2 = AccessTools.Method(typeof(WaterHelper), "TransferLiquid", new Type[2]
				{
					typeof(GameItem),
					typeof(GameItem)
				}, (Type[])null);
				MethodInfo methodInfo3 = AccessTools.Method(typeof(WaterHelper), "EmptyContainer", new Type[1] { typeof(GameItem) }, (Type[])null);
				if (methodInfo == null || methodInfo2 == null || methodInfo3 == null)
				{
					throw new MissingMethodException("Native water helper signatures changed.");
				}
				installedTargets.AddRange(new MethodBase[3] { methodInfo, methodInfo2, methodInfo3 });
				harmonyInstance.Patch((MethodBase)methodInfo, (HarmonyMethod)null, new HarmonyMethod(AccessTools.Method(typeof(BarrelWaterAdded), "Postfix", (Type[])null, (Type[])null)), (HarmonyMethod)null, (HarmonyMethod)null);
				harmonyInstance.Patch((MethodBase)methodInfo, (HarmonyMethod)null, new HarmonyMethod(AccessTools.Method(typeof(ConnectedWaterAdded), "Postfix", (Type[])null, (Type[])null)), (HarmonyMethod)null, (HarmonyMethod)null);
				harmonyInstance.Patch((MethodBase)methodInfo, (HarmonyMethod)null, new HarmonyMethod(AccessTools.Method(typeof(ModFaucetSprite), "Postfix", (Type[])null, (Type[])null)), (HarmonyMethod)null, (HarmonyMethod)null);
				harmonyInstance.Patch((MethodBase)methodInfo2, new HarmonyMethod(AccessTools.Method(typeof(BarrelWaterOnly), "Prefix", (Type[])null, (Type[])null)), new HarmonyMethod(AccessTools.Method(typeof(ConnectedWaterTransfer), "Postfix", (Type[])null, (Type[])null)), (HarmonyMethod)null, (HarmonyMethod)null);
				harmonyInstance.Patch((MethodBase)methodInfo3, (HarmonyMethod)null, new HarmonyMethod(AccessTools.Method(typeof(ConnectedWaterEmptied), "Postfix", (Type[])null, (Type[])null)), (HarmonyMethod)null, (HarmonyMethod)null);
				if (!Current(token))
				{
					RemoveInstalled();
					return;
				}
				Status = Phase.Ready;
				InstallCount++;
				ModLog obj = log;
				LocaleIdentifier identifier = LocalizationSettings.SelectedLocale.Identifier;
				obj.Log((object)("Liquid adapters ready: 3 native targets, 6 callbacks; native player locale applied=" + ((LocaleIdentifier)(ref identifier)).Code + "; localization and Mechanic tables succeeded; owner=" + harmonyInstance.Id));
			}
		}

		internal static void Fail(string message, Exception error)
		{
			Status = Phase.Failed;
			try
			{
				RemoveInstalled();
			}
			catch (Exception ex)
			{
				ModLog obj = log;
				if (obj != null)
				{
					obj.Error((object)("Liquid adapter cleanup failed: " + ex));
				}
			}
			ModLog obj2 = log;
			if (obj2 != null)
			{
				obj2.Error((object)("Liquid adapters NOT ready: " + message + ((error == null) ? "" : ("\n" + error))));
			}
		}

		private static void RemoveInstalled()
		{
			Harmony val = owner?.HarmonyInstance;
			if (val != null)
			{
				foreach (MethodBase installedTarget in installedTargets)
				{
					val.Unpatch(installedTarget, (HarmonyPatchType)0, "mod.brewing_expansion");
				}
			}
			installedTargets.Clear();
		}

		internal static void Disable()
		{
			generation++;
			ModHook.OnModAssetsLoaded -= AssetsLoaded;
			if ((Object)(object)gate != (Object)null)
			{
				((MonoBehaviour)gate).StopAllCoroutines();
				Object.Destroy((Object)(object)((Component)gate).gameObject);
				gate = null;
			}
			try
			{
				RemoveInstalled();
			}
			finally
			{
				owner = null;
				entry = null;
				Status = Phase.Disabled;
			}
		}
	}
	internal sealed class LiquidPatchGate : MonoBehaviour
	{
		internal void Begin(IEnumerator steps)
		{
			((MonoBehaviour)this).StartCoroutine(Guard(steps));
		}

		private IEnumerator Guard(IEnumerator steps)
		{
			while (true)
			{
				bool flag;
				object obj;
				try
				{
					flag = steps.MoveNext();
					obj = (flag ? steps.Current : null);
				}
				catch (Exception error)
				{
					LiquidPatches.Fail("Native readiness or patch registration failed; restart the game after correcting it.", error);
					break;
				}
				if (!flag)
				{
					break;
				}
				yield return obj;
			}
		}
	}
	public static class LiunianCabinet
	{
		private sealed class View
		{
			internal MachineryText Value;

			internal Action<GameItem, GameInventory, SlotMarker> ForeignCycle;

			internal bool Drawing;
		}

		public const string Id = "be_liunian_ager";

		private static readonly ConditionalWeakTable<GameItem, View> Views = new ConditionalWeakTable<GameItem, View>();

		private static readonly HashSet<GameItem> Dirty = new HashSet<GameItem>();

		private static readonly HashSet<GameItem> Completed = new HashSet<GameItem>();

		private static bool inNight;

		private static PlayerStore queuedStore;

		private static LiunianPump pump;

		public static GameSlotInventory Battery(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 0);
			return (GameSlotInventory)(object)((bodyStorage is GameSlotInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Modules(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 1);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Storage(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 2);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		private static string Text(string key, params object[] args)
		{
			return ModHelper.GetLocalized("brewing_expansion", key, args);
		}

		public static GameItem Create()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			GameItem item = MachineWineRack.AgeWell();
			item.identifier = "be_liunian_ager";
			item.SetValue(6666L);
			item.SetSpriteAndShapeFromMod("brewing_expansion", "be_liunian_ager");
			item.SetShape(new GridShapeBuilder(3, 3).SetDataFill(3, 3, (byte)1).Build());
			((GameItemFunc)item).getItemSpriteModifierForItemFunc = null;
			GameGridInventory val = Modules(item);
			GameGridInventory val2 = Storage(item);
			Action<GameItem, GameInventory, SlotMarker> onCycleEndSlotItemFunc = ((GameItemFunc)item).onCycleEndSlotItemFunc;
			bool flag = onCycleEndSlotItemFunc != null && onCycleEndSlotItemFunc.GetInvocationList().Length == 1 && ReviewedNative() && onCycleEndSlotItemFunc.Method.DeclaringType?.Assembly == typeof(MachineWineRack).Assembly && onCycleEndSlotItemFunc.Method.Name == "<AgeWell>b__5";
			if (flag && NativeGridCallbacks(val, val2) && NativeGridRendering())
			{
				MachineryPanelRow val3 = (MachineryPanelRow)item.contentWindow.childElement;
				GameGridInventory val4 = val;
				GameGridInventory val5 = val2;
				val = (GameGridInventory)(object)new LiunianInventory(6, 20);
				val2 = (GameGridInventory)(object)new LiunianInventory(20, 20);
				((GameInventoryFunc)val2).mayInventoryAddItemFunc = ((GameInventoryFunc)val5).mayInventoryAddItemFunc;
				((MachineryIntake)val3.GetPanel("module").content).SetContent((PixelElement)(object)val);
				((MachineryIntake)val3.GetPanel("wine").content).SetContent((PixelElement)(object)val2);
				MachineHelper.SetupModuleBay((GameInventory)(object)val, item, (Action)delegate
				{
					Request(item);
				}, (string[])null);
				Object.Destroy((Object)(object)((Component)val4.handler).gameObject);
				Object.Destroy((Object)(object)((Component)val5.handler).gameObject);
			}
			val.SetShape(6, 20);
			val2.SetShape(20, 20);
			((GameInventory)val).identifier = "BE_LIUNIAN_MODULES";
			((GameInventory)val2).identifier = "BE_LIUNIAN_STORAGE";
			MachineryPanelRow val6 = (MachineryPanelRow)item.contentWindow.childElement;
			MachineryPanel panel = val6.GetPanel("module");
			panel.SetLabel(Text("liunian_modules"));
			MachineryPanel panel2 = val6.GetPanel("wine");
			panel2.SetLabel(Text("liunian_storage"));
			PixelElement content = panel2.content;
			panel2.SetContent((PixelElement)null);
			MachineryText val7 = new MachineryText("", 6, (HorizontalAlignmentOptions)2, true).SetMinHeight(32);
			panel2.SetContent((PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[2]
			{
				content,
				(PixelElement)val7
			}));
			PixelElement content2 = panel.content;
			panel.SetContent((PixelElement)null);
			panel.SetContent((PixelElement)(object)MachineryStack.Vertical((PixelElement[])(object)new PixelElement[2]
			{
				content2,
				(PixelElement)new MachineryText("", 6, (HorizontalAlignmentOptions)2, true).SetMinHeight(32)
			}));
			Views.Add(item, new View
			{
				Value = val7,
				ForeignCycle = (flag ? null : onCycleEndSlotItemFunc)
			});
			(val as LiunianInventory)?.Bind(item);
			(val2 as LiunianInventory)?.Bind(item);
			((GameInventoryFunc)val2).onSlotAddItemLateFunc = DeferNativeRedraw(((GameInventoryFunc)val2).onSlotAddItemLateFunc, "<AgeWell>b__3", item);
			((GameInventoryFunc)val2).onSlotRemoveItemLateFunc = DeferNativeRedraw(((GameInventoryFunc)val2).onSlotRemoveItemLateFunc, "<AgeWell>b__4", item);
			PixelWindow contentWindow = item.contentWindow;
			contentWindow.openCallback = (Action)Delegate.Combine(contentWindow.openCallback, (Action)delegate
			{
				RefreshValue(item);
			});
			((GameItemFunc)item).onCycleEndSlotItemFunc = delegate
			{
				Cycle(item);
			};
			GameItem obj = item;
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Loaded));
			Loaded(item);
			return item;
		}

		private static bool NativeGridCallbacks(GameGridInventory modules, GameGridInventory storage)
		{
			if (((GameInventory)modules).childItems.Count == 0 && ((GameInventory)storage).childItems.Count == 0 && Plain((GameInventoryFunc)(object)modules) && Plain((GameInventoryFunc)(object)storage) && Native(((GameInventoryFunc)modules).onSlotAddItemFunc) && Native(((GameInventoryFunc)modules).onSlotAddItemLateFunc) && Native(((GameInventoryFunc)modules).onSlotAddItemEndFunc) && Native(((GameInventoryFunc)modules).onSlotRemoveItemFunc) && Native(((GameInventoryFunc)modules).onSlotRemoveItemLateFunc) && Native(((GameInventoryFunc)modules).onSlotRemoveItemEndFunc) && Native(((GameInventoryFunc)modules).mayInventoryAddItemFunc) && Native(((GameInventoryFunc)storage).onSlotAddItemFunc) && Native(((GameInventoryFunc)storage).onSlotAddItemLateFunc) && Native(((GameInventoryFunc)storage).onSlotAddItemEndFunc) && Native(((GameInventoryFunc)storage).onSlotRemoveItemFunc) && Native(((GameInventoryFunc)storage).onSlotRemoveItemLateFunc) && Native(((GameInventoryFunc)storage).onSlotRemoveItemEndFunc))
			{
				return Native(((GameInventoryFunc)storage).mayInventoryAddItemFunc);
			}
			return false;
			static bool Native(Delegate callbacks)
			{
				return callbacks?.GetInvocationList().All((Delegate c) => c.Method.DeclaringType?.Assembly == typeof(MachineWineRack).Assembly) ?? true;
			}
			static bool Plain(GameInventoryFunc g)
			{
				return new Delegate[26]
				{
					g.mayInventoryRemoveItemFunc, g.maxSlotAddItemFunc, g.maxSlotRemoveItemFunc, g.maxSlotAmountItemFunc, g.minSlotAmountItemFunc, g.onSlotChangeAmountItemFunc, g.slotPriorityFunc, g.slotSearchPassthroughForItemFunc, g.parentItemShapeModifierForItemFunc, g.parentItemSpriteModifierForItemFunc,
					g.parentItemStateModifierForItemFunc, g.isItemStateValidFunc, g.maySelectSlotItemFunc, g.canSelectSlotItemFunc, g.maySlotItemTargetItemFunc, g.canSlotItemTargetItemFunc, g.onSlotItemTargetItemFunc, g.mayItemTargetSlotItemFunc, g.canItemTargetSlotItemFunc, g.onItemTargetSlotItemFunc,
					g.mayActivateSlotItemFunc, g.canActivateSlotItemFunc, g.onActivateSlotItemFunc, g.mayToggleSlotItemFunc, g.canToggleSlotItemFunc, g.onToggleSlotItemFunc
				}.All((Delegate c) => (object)c == null);
			}
		}

		private static bool NativeGridRendering()
		{
			MethodInfo[] array = new MethodInfo[2]
			{
				AccessTools.Method(typeof(GameGridInventory), "Validate", (Type[])null, (Type[])null),
				AccessTools.Method(typeof(MachineWineRack), "AgeWell", (Type[])null, (Type[])null)
			};
			for (int i = 0; i < array.Length; i++)
			{
				Patches patchInfo = Harmony.GetPatchInfo((MethodBase)array[i]);
				if (patchInfo != null && patchInfo.Prefixes.Concat(patchInfo.Postfixes).Concat(patchInfo.Transpilers).Concat(patchInfo.Finalizers)
					.Any((Patch p) => p.PatchMethod.DeclaringType?.Assembly != typeof(LiunianCabinet).Assembly))
				{
					return false;
				}
			}
			return true;
		}

		private static Action<GameItem, GameInventory, SlotMarker> DeferNativeRedraw(Action<GameItem, GameInventory, SlotMarker> original, string name, GameItem cabinet)
		{
			if (ReviewedNative() && original != null)
			{
				Delegate[] invocationList = original.GetInvocationList();
				foreach (Delegate obj in invocationList)
				{
					if (obj.Method.DeclaringType?.Assembly == typeof(MachineWineRack).Assembly && obj.Method.Name == name)
					{
						original = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Remove(original, (Action<GameItem, GameInventory, SlotMarker>)obj);
					}
				}
			}
			return (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(original, (Action<GameItem, GameInventory, SlotMarker>)delegate
			{
				Request(cabinet);
			});
		}

		private static void Loaded(GameItem item)
		{
			ModuleExpansion.RestoreCabinet(item);
			item.SetName(Text("liunian_name"));
			string shortDescription = (item.longDescription = Text("liunian_description"));
			item.shortDescription = shortDescription;
			item.contentWindow.SetTitle(item.name);
			((GameItemFunc)item).getItemSpriteModifierForItemFunc = null;
			ItemHitArea.Apply(item);
			Request(item);
		}

		private static GameItem[] Wines(GameItem item)
		{
			return GraphUtils.FindAllChildrenType<GameItem>((GraphNodeStorage)(object)Storage(item), (Func<GameItem, bool>)MachineWineRack.CanAgeWine, (Func<GameItem, bool>)null).ToArray();
		}

		public static long TotalValue(GameItem item)
		{
			long num = 0L;
			GameItem[] array = Wines(item);
			foreach (GameItem val in array)
			{
				long num2 = Math.Max(0L, val.GetCurrentValue(false, true, true, false, 0L));
				num = ((num2 > long.MaxValue - num) ? long.MaxValue : (num + num2));
			}
			return num;
		}

		public static void RefreshValue(GameItem item)
		{
			if (!MachineConnections.Live(item) || !Views.TryGetValue(item, out var value) || value.Drawing)
			{
				return;
			}
			value.Drawing = true;
			try
			{
				long num = TotalValue(item);
				int num2 = ((num >= 500) ? ((num < 1000) ? 1 : ((num < 2000) ? 2 : ((num < 5000) ? 3 : ((num < 10000) ? 4 : 5)))) : 0);
				string text = Text("liunian_total", num) + "\n" + Text("liunian_season_" + num2);
				value.Value.SetText(text);
				item.Validate();
				(Modules(item) as LiunianInventory)?.Flush();
				(Storage(item) as LiunianInventory)?.Flush();
				ItemHitArea.Apply(item);
			}
			finally
			{
				value.Drawing = false;
			}
		}

		internal static void Request(GameItem item)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Expected O, but got Unknown
			if (ModEntry.Enabled && PlayerStore.Instance != null && (!Views.TryGetValue(item, out var value) || !value.Drawing))
			{
				if (queuedStore != PlayerStore.Instance)
				{
					Dirty.Clear();
				}
				queuedStore = PlayerStore.Instance;
				Dirty.Add(item);
				if ((Object)(object)pump == (Object)null)
				{
					GameObject val = new GameObject("BrewingExpansion.Liunian");
					Object.DontDestroyOnLoad((Object)val);
					pump = val.AddComponent<LiunianPump>();
				}
			}
		}

		internal static void Drain()
		{
			if (Dirty.Count == 0)
			{
				return;
			}
			PlayerStore val = queuedStore;
			GameItem[] array = Dirty.ToArray();
			Dirty.Clear();
			queuedStore = null;
			if (val == null || val != PlayerStore.Instance)
			{
				return;
			}
			GameItem[] array2 = array;
			foreach (GameItem val2 in array2)
			{
				try
				{
					if (MachineConnections.Live(val2) && val2.contentWindow.IsVisible())
					{
						RefreshValue(val2);
					}
				}
				catch (Exception ex)
				{
					Debug.LogWarning((object)("[BrewingExpansion.Formal] Cabinet value refresh deferred: " + ex.Message));
				}
			}
		}

		private static bool ReviewedNative()
		{
			string text = typeof(WineHelper).Module.ModuleVersionId.ToString();
			if (!(text == "138a77df-e8c2-41f1-af03-1e0b96008204") && !(text == "4b229a91-52d5-4262-8e48-587e0c1677f2"))
			{
				return text == "49340d28-06ce-4506-acb7-12248c3abc50";
			}
			return true;
		}

		public static bool BulkAvailable()
		{
			if (!ReviewedNative())
			{
				return false;
			}
			MethodInfo[] array = new MethodInfo[2]
			{
				AccessTools.Method(typeof(WineHelper), "OnAgeWine", (Type[])null, (Type[])null),
				AccessTools.Method(typeof(AgableHelper), "OnAge", (Type[])null, (Type[])null)
			};
			for (int i = 0; i < array.Length; i++)
			{
				Patches patchInfo = Harmony.GetPatchInfo((MethodBase)array[i]);
				if (patchInfo != null && patchInfo.Prefixes.Concat(patchInfo.Postfixes).Concat(patchInfo.Transpilers).Concat(patchInfo.Finalizers)
					.Any((Patch p) => p.PatchMethod.DeclaringType?.Assembly != typeof(LiunianCabinet).Assembly))
				{
					return false;
				}
			}
			return true;
		}

		public static int Cycle(GameItem item)
		{
			PlayerStore instance = PlayerStore.Instance;
			if (!ModEntry.Enabled || instance == null || ((item != null) ? item.identifier : null) != "be_liunian_ager" || !MachineConnections.Owned(item) || !PulpOperations.CanSave(instance) || !WaterBottlePrinter.CanSave(instance) || !PulpOperations.Busy.Add(item))
			{
				return 0;
			}
			PulpOperations.Store = instance;
			int num = 0;
			int debit = 0;
			GameItem val = null;
			try
			{
				GameItem[] array = Wines(item);
				if (Views.TryGetValue(item, out var value) && value.ForeignCycle != null)
				{
					long[] beforeAges = array.Select(LiunianWineAge.Age).ToArray();
					try
					{
						value.ForeignCycle(item, item.parentInventory, null);
					}
					finally
					{
						if (instance == PlayerStore.Instance)
						{
							num = array.Where((GameItem w, int n) => MachineConnections.Live(w) && LiunianWineAge.Age(w) != beforeAges[n]).Count();
							if (num > 0)
							{
								if (inNight)
								{
									Completed.Add(item);
								}
								else
								{
									Chill(item);
								}
							}
						}
					}
					return num;
				}
				GameSlotInventory obj = Battery(item);
				val = ((obj != null) ? obj.childItem : null);
				if (array.Length == 0 || !MachineConnections.Owned(val))
				{
					return 0;
				}
				int num2 = val.GetTagReadonly("power_source_item_energy").GetInt();
				bool flag = false;
				try
				{
					flag = MachineHelper.TryDrawCyclePower(item, Battery(item));
				}
				finally
				{
					if (instance == PlayerStore.Instance && MachineConnections.Live(val))
					{
						debit = Math.Min(Math.Max(0, MachineryHelper.GetMachinePowerUsage(item)), Math.Max(0, num2 - val.GetTagReadonly("power_source_item_energy").GetInt()));
					}
				}
				if (!flag || instance != PlayerStore.Instance || !MachineConnections.Owned(item) || Battery(item).childItem != val)
				{
					return 0;
				}
				int agingDays = MachineWineRack.GetAgingDays(item);
				bool flag2 = BulkAvailable();
				GameItem[] array2 = array;
				foreach (GameItem val2 in array2)
				{
					if (instance != PlayerStore.Instance)
					{
						return num;
					}
					if (!MachineConnections.Owned(val2) || !MachineWineRack.CanAgeWine(val2) || (object)val2.parentInventory != Storage(item))
					{
						continue;
					}
					try
					{
						if (flag2)
						{
							if (LiunianWineAge.Advance(val2, agingDays))
							{
								num++;
							}
							continue;
						}
						for (int num4 = 0; num4 < agingDays; num4++)
						{
							if (!MachineConnections.Live(val2))
							{
								break;
							}
							WineHelper.OnAgeWine(val2);
						}
						num++;
					}
					catch (Exception ex)
					{
						Debug.LogWarning((object)("[BrewingExpansion.Formal] Cabinet skipped one wine: " + ex.Message));
					}
				}
				if (num == 0)
				{
					return 0;
				}
				if (inNight)
				{
					Completed.Add(item);
				}
				else
				{
					Chill(item);
				}
				MachineHelper.OnMachineActioned(item, (GameInventory)(object)Modules(item));
				return num;
			}
			catch (Exception ex2)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Cabinet cycle interrupted: " + ex2.Message));
				return num;
			}
			finally
			{
				try
				{
					if (num == 0 && debit > 0 && instance == PlayerStore.Instance && MachineConnections.Live(val))
					{
						val.ModifyTag("power_source_item_energy", (Action<TagState>)delegate(TagState t)
						{
							t.Enable().SetInt(t.GetInt() + debit);
						}, false);
						PowerHelper.UpdateEnergyCreditSprite(val);
					}
				}
				catch (Exception ex3)
				{
					PulpOperations.Protect(instance, "cabinet power refund failed: " + ex3.Message);
				}
				finally
				{
					PulpOperations.Busy.Remove(item);
					if (PulpOperations.Busy.Count == 0)
					{
						PulpOperations.Store = null;
					}
					Request(item);
				}
			}
		}

		private static void Chill(GameItem cabinet)
		{
			if (!MachineConnections.Owned(cabinet))
			{
				return;
			}
			GameItem[] array = Wines(cabinet);
			foreach (GameItem val in array)
			{
				if (!val.itemFeatures.Any((ItemFeature f) => f.identifier == "chilled" && !f.isDisabled))
				{
					MachineFridge.ChillItem(val);
				}
			}
			Request(cabinet);
		}

		public static void BeginNight()
		{
			Completed.Clear();
			inNight = true;
		}

		public static void FinishNight()
		{
			inNight = false;
			GameItem[] array = Completed.ToArray();
			Completed.Clear();
			GameItem[] array2 = array;
			foreach (GameItem cabinet in array2)
			{
				try
				{
					Chill(cabinet);
				}
				catch (Exception ex)
				{
					Debug.LogWarning((object)("[BrewingExpansion.Formal] Cabinet chilling deferred: " + ex.Message));
				}
			}
		}

		internal static void Reset()
		{
			Dirty.Clear();
			Completed.Clear();
			queuedStore = null;
			inNight = false;
			if ((Object)(object)pump != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)pump).gameObject);
			}
			pump = null;
		}
	}
	internal sealed class LiunianPump : MonoBehaviour
	{
		private void LateUpdate()
		{
			LiunianCabinet.Drain();
		}
	}
	internal sealed class LiunianInventory : GameGridInventory
	{
		private GameItem owner;

		private PlayerStore store;

		private bool dirty;

		internal LiunianInventory(int width, int height)
			: base(width, height)
		{
		}

		internal void Bind(GameItem item)
		{
			owner = item;
			store = PlayerStore.Instance;
		}

		public override void Validate()
		{
			if (owner == null || !ModEntry.Enabled || store != PlayerStore.Instance || ((GameGridInventory)this).widthPixels != ((GameGridInventory)this).inventoryShape.width * UISettings.current.gridSize || ((GameGridInventory)this).heightPixels != ((GameGridInventory)this).inventoryShape.height * UISettings.current.gridSize)
			{
				((GameGridInventory)this).Validate();
				return;
			}
			dirty = true;
			LiunianCabinet.Request(owner);
		}

		internal void Flush()
		{
			if (dirty)
			{
				dirty = false;
				((GameGridInventory)this).Validate();
			}
		}
	}
	public static class LiunianWineAge
	{
		public const string ExtendedTag = "BE_WINE_AGE64";

		public const string OwnedTag = "BE_FORMAL_LIUNIAN_AGED";

		public static long Age(GameItem wine)
		{
			if (!wine.IsTag("BE_WINE_AGE64"))
			{
				return AgableHelper.GetAge(wine);
			}
			return Math.Max(wine.GetTagReadonly("BE_WINE_AGE64").GetLong(), AgableHelper.GetAge(wine));
		}

		public static bool Advance(GameItem wine, int days)
		{
			if (days < 1 || !MachineWineRack.CanAgeWine(wine))
			{
				return false;
			}
			PlayerStore instance = PlayerStore.Instance;
			long num = Age(wine);
			long unitValue = wine.unitValue;
			if (num < 0)
			{
				return false;
			}
			int num2 = (wine.IsTag("AGE_VALUE_MOD_PER_DAY_INT") ? wine.GetTagReadonly("AGE_VALUE_MOD_PER_DAY_INT").GetInt() : 3);
			long nextAge;
			long num3;
			TagState val;
			TagState val2;
			checked
			{
				try
				{
					nextAge = num + days;
					num3 = unitValue + unchecked((long)num2) * unchecked((long)days);
				}
				catch (OverflowException)
				{
					return false;
				}
				val = (wine.state.dict.TryGetValue("AGE_INT", out var value) ? value.Clone() : null);
				val2 = (wine.state.dict.TryGetValue("BE_WINE_AGE64", out var value2) ? value2.Clone() : null);
			}
			try
			{
				wine.ModifyTag("AGE_INT", (Action<TagState>)delegate(TagState t)
				{
					t.Enable().SetInt((int)Math.Min(2147483647L, nextAge));
				}, false);
				if (instance != PlayerStore.Instance)
				{
					return false;
				}
				if (nextAge > int.MaxValue)
				{
					wine.ModifyTag("BE_WINE_AGE64", (Action<TagState>)delegate(TagState t)
					{
						t.Enable().SetLong(nextAge);
					}, false);
				}
				wine.unitValue = num3;
				wine.EnableTag("BE_FORMAL_LIUNIAN_AGED", false);
				if (Age(wine) != nextAge || wine.unitValue != num3)
				{
					throw new InvalidOperationException("Wine aging write was cancelled");
				}
				return true;
			}
			catch
			{
				if (instance != PlayerStore.Instance)
				{
					return false;
				}
				if (val == null)
				{
					wine.state.dict.Remove("AGE_INT");
				}
				else
				{
					wine.state.dict["AGE_INT"] = val;
				}
				if (val2 == null)
				{
					wine.state.dict.Remove("BE_WINE_AGE64");
				}
				else
				{
					wine.state.dict["BE_WINE_AGE64"] = val2;
				}
				wine.unitValue = unitValue;
				throw;
			}
		}
	}
	[HarmonyPatch(typeof(AgableHelper), "OnAge")]
	internal static class LiunianExtendedDailyAge
	{
		private static bool Prefix(GameItem __0, bool __runOriginal)
		{
			if (!__runOriginal || !__0.IsTag("BE_FORMAL_LIUNIAN_AGED") || !MachineWineRack.CanAgeWine(__0) || (!__0.IsTag("BE_WINE_AGE64") && AgableHelper.GetAge(__0) < int.MaxValue))
			{
				return true;
			}
			LiunianWineAge.Advance(__0, 1);
			return false;
		}
	}
	[HarmonyPatch(typeof(AgableHelper), "ResetAge")]
	internal static class LiunianResetAge
	{
		private static void Postfix(GameItem __0)
		{
			if (__0.IsTag("BE_FORMAL_LIUNIAN_AGED"))
			{
				__0.DisableTag("BE_WINE_AGE64", false);
				__0.DisableTag("BE_FORMAL_LIUNIAN_AGED", false);
			}
		}
	}
	public static class MachineConnections
	{
		private sealed class SourceName
		{
			internal string Base;

			internal string Applied;

			internal int Number;
		}

		private sealed class ConnectionGroup
		{
			internal GameItem Source;

			internal GameItem[] Barrels;
		}

		public const string LinksTag = "BE36_MACHINE_LINKS";

		private const string NumberTag = "BE36_BARREL_NUMBER";

		private static bool processing;

		private static bool pending;

		private static PlayerStore queuedStore;

		private static ConnectionPump pump;

		private static readonly Dictionary<GameItem, SourceName> sourceNames = new Dictionary<GameItem, SourceName>();

		private static Dictionary<int, GameItem> overviewItems = new Dictionary<int, GameItem>();

		private static ConnectionGroup[] overview = Array.Empty<ConnectionGroup>();

		private static PlayerStore overviewStore;

		public static bool Live(GameItem item)
		{
			if (item != null)
			{
				GameItem obj = ((item is GameItemElement) ? item : null);
				if (obj == null)
				{
					return true;
				}
				return !((GameItemElement)obj).IsDestroyed();
			}
			return false;
		}

		public static bool Owned(GameItem item)
		{
			if (Live(item))
			{
				return GeneralHelper.IsItemOwned(item);
			}
			return false;
		}

		public static bool IsWaterSource(GameItem item)
		{
			if (item != null && item.identifier != "be_large_brewing_barrel" && !item.IsTag("WINE_CONTAINER_TAG") && item.IsTag("LIQUID_CONTAINER_TAG") && item.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt() > 0 && !WaterHelper.IsFaucet(item))
			{
				return !WaterHelper.IsDrain(item);
			}
			return false;
		}

		public static bool IsSource(GameItem item)
		{
			if (!(((item != null) ? item.identifier : null) == "be_fruit_pulp_concentrator") && !(((item != null) ? item.identifier : null) == "be_wine_printer"))
			{
				return IsWaterSource(item);
			}
			return true;
		}

		private static int Kind(GameItem item)
		{
			if (!(((item != null) ? item.identifier : null) == "be_fruit_pulp_concentrator"))
			{
				if (!(((item != null) ? item.identifier : null) == "be_wine_printer"))
				{
					if (!IsWaterSource(item))
					{
						return -1;
					}
					return 2;
				}
				return 1;
			}
			return 0;
		}

		internal static GameItem[] OwnedItems()
		{
			if (PlayerStore.HasInstance && !((Object)(object)EmporiumEntry.Instance == (Object)null))
			{
				return PlayerStore.Instance.FindAllItem(true).Where(Owned).ToArray();
			}
			return Array.Empty<GameItem>();
		}

		private static GameItem[] Machines()
		{
			return (from x in OwnedItems()
				where IsSource(x) || x.identifier == "be_large_brewing_barrel"
				select x).ToArray();
		}

		public static string[] LinkedBarrelNames()
		{
			return LinkedBarrelNames(OwnedItems());
		}

		internal static string[] LinkedBarrelNames(GameItem[] items)
		{
			if (items.Length == 0)
			{
				return Array.Empty<string>();
			}
			Dictionary<int, GameItem> dictionary = (from x in items
				group x by x.uniqueId into g
				where g.Key > 0 && g.Count() == 1
				select g).ToDictionary((IGrouping<int, GameItem> g) => g.Key, (IGrouping<int, GameItem> g) => g.Single());
			HashSet<int> linked = new HashSet<int>();
			foreach (GameItem item in dictionary.Values.Where(IsSource))
			{
				linked.UnionWith(Targets(item));
			}
			return (from x in dictionary.Values
				where x.identifier == "be_large_brewing_barrel" && linked.Contains(x.uniqueId)
				orderby x.GetTagReadonly("BE36_BARREL_NUMBER").GetInt(), x.uniqueId
				select x).Select(DisplayName).ToArray();
		}

		public static string DisplayName(GameItem item)
		{
			if (!Live(item))
			{
				return "";
			}
			if (!GeneralHelper.HasCustomName(item))
			{
				return item.GetDisplayName(false);
			}
			SourceName value;
			int num = ((item.identifier == "be_large_brewing_barrel") ? item.GetTagReadonly("BE36_BARREL_NUMBER").GetInt() : (sourceNames.TryGetValue(item, out value) ? value.Number : 0));
			return item.GetDisplayName(true) + ((num > 0) ? (" (" + num + ")") : "");
		}

		private static void RefreshCableDescriptions(GameItem[] items)
		{
			overviewStore = (PlayerStore.HasInstance ? PlayerStore.Instance : null);
			Dictionary<int, GameItem> unique = (from x in items
				group x by x.uniqueId into g
				where g.Key > 0 && g.Count() == 1
				select g).ToDictionary((IGrouping<int, GameItem> g) => g.Key, (IGrouping<int, GameItem> g) => g.Single());
			overviewItems = unique;
			overview = (from source in unique.Values.Where(IsSource).OrderBy(Kind).ThenBy((GameItem x) => x.uniqueId)
				select new ConnectionGroup
				{
					Source = source,
					Barrels = (from id in Targets(source)
						where unique.TryGetValue(id, out var value) && value.identifier == "be_large_brewing_barrel"
						select unique[id] into x
						orderby x.GetTagReadonly("BE36_BARREL_NUMBER").GetInt(), x.uniqueId
						select x).ToArray()
				} into g
				where g.Barrels.Length != 0
				select g).ToArray();
			foreach (GameItem item in items.Where((GameItem x) => x.identifier == "be_xuxu_connection_pliers"))
			{
				RefreshCableDescription(item);
			}
		}

		internal static void RefreshCableDescriptions()
		{
			RefreshCableDescriptions(OwnedItems());
		}

		internal static void RefreshCableDescription(GameItem cable)
		{
			bool flag = PlayerStore.HasInstance && overviewStore == PlayerStore.Instance && (Object)(object)EmporiumEntry.Instance != (Object)null;
			ConnectionGroup[] array = (flag ? (from g in overview
				where Owned(g.Source)
				select new ConnectionGroup
				{
					Source = g.Source,
					Barrels = g.Barrels.Where(Owned).ToArray()
				} into g
				where g.Barrels.Length != 0
				select g).ToArray() : Array.Empty<ConnectionGroup>());
			List<string> list = (from g in array.Take(3)
				select ModText.Get("cable_connection", DisplayName(g.Source), string.Join(ModText.Get("cable_list_separator"), g.Barrels.Take(3).Select(DisplayName)) + ((g.Barrels.Length > 3) ? "…" : ""))).ToList();
			if (array.Length > 3)
			{
				list.Add(ModText.Get("cable_more_connections", array.Length - 3));
			}
			GameItem value = null;
			if (flag)
			{
				overviewItems.TryGetValue(cable.GetTagReadonly("BE36_SELECTED_SOURCE").GetInt(), out value);
			}
			TransferCable.RefreshDescription(cable, Owned(value) ? value : null, list.ToArray());
		}

		public static int[] Targets(GameItem source)
		{
			List<int> list = new List<int>();
			string[] array = (((source != null) ? source.GetTagReadonly("BE36_MACHINE_LINKS").GetString() : null) ?? "").Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				if (int.TryParse(array[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) && result > 0 && !list.Contains(result))
				{
					list.Add(result);
				}
			}
			return list.ToArray();
		}

		private static void Save(GameItem source, IEnumerable<int> targets)
		{
			source.ModifyTag("BE36_MACHINE_LINKS", (Action<TagState>)delegate(TagState t)
			{
				t.Enable().SetString(string.Join(",", targets));
			}, false);
		}

		public static bool Toggle(GameItem source, GameItem barrel)
		{
			PlayerStore instance = PlayerStore.Instance;
			if (!ModEntry.Enabled || instance == null || processing || !Owned(source) || !Owned(barrel) || !IsSource(source) || barrel.identifier != "be_large_brewing_barrel" || !PulpOperations.CanSave(instance) || !WaterBottlePrinter.CanSave(instance))
			{
				return false;
			}
			GameItem[] source2 = Machines();
			if (source2.Count((GameItem x) => x.uniqueId == source.uniqueId) != 1 || source2.Count((GameItem x) => x.uniqueId == barrel.uniqueId) != 1)
			{
				return false;
			}
			List<int> list = Targets(source).ToList();
			bool flag = !list.Remove(barrel.uniqueId);
			if (flag)
			{
				list.Add(barrel.uniqueId);
			}
			Dictionary<GameItem, int[]> dictionary = new Dictionary<GameItem, int[]> { 
			{
				source,
				list.ToArray()
			} };
			if (flag)
			{
				foreach (GameItem item in source2.Where((GameItem x) => x != source && Kind(x) == Kind(source)))
				{
					List<int> list2 = Targets(item).ToList();
					if (list2.Remove(barrel.uniqueId))
					{
						dictionary[item] = list2.ToArray();
					}
				}
			}
			Dictionary<GameItem, int[]> dictionary2 = dictionary.ToDictionary((KeyValuePair<GameItem, int[]> x) => x.Key, (KeyValuePair<GameItem, int[]> x) => Targets(x.Key));
			PulpOperations.Busy.Add(source);
			PulpOperations.Store = instance;
			try
			{
				foreach (KeyValuePair<GameItem, int[]> item2 in dictionary)
				{
					if (instance != PlayerStore.Instance || !Owned(item2.Key) || !Owned(barrel))
					{
						return false;
					}
					Save(item2.Key, item2.Value);
					if (!((ReadOnlySpan<int>)Targets(item2.Key)).SequenceEqual((ReadOnlySpan<int>)item2.Value))
					{
						throw new InvalidOperationException("Connection update was cancelled");
					}
				}
				GameItem[] array = OwnedItems();
				RefreshNames(array.Where((GameItem x) => IsSource(x) || x.identifier == "be_large_brewing_barrel").ToArray());
				RefreshCableDescriptions(array);
				Request();
				return flag;
			}
			catch (Exception ex)
			{
				if (instance == PlayerStore.Instance)
				{
					foreach (KeyValuePair<GameItem, int[]> item3 in dictionary2)
					{
						try
						{
							if (((ReadOnlySpan<int>)Targets(item3.Key)).SequenceEqual((ReadOnlySpan<int>)dictionary[item3.Key]))
							{
								Save(item3.Key, item3.Value);
							}
							if (!((ReadOnlySpan<int>)Targets(item3.Key)).SequenceEqual((ReadOnlySpan<int>)item3.Value))
							{
								PulpOperations.Protect(instance, "connection edit rollback was not confirmed");
							}
						}
						catch (Exception)
						{
							PulpOperations.Protect(instance, "connection edit rollback failed");
						}
					}
				}
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Connection update interrupted: " + ex.Message));
				return false;
			}
			finally
			{
				PulpOperations.Busy.Remove(source);
				if (PulpOperations.Busy.Count == 0)
				{
					PulpOperations.Store = null;
				}
			}
		}

		public static GameItem PrinterFor(GameItem barrel)
		{
			if (!Owned(barrel) || barrel.identifier != "be_large_brewing_barrel")
			{
				return null;
			}
			GameItem[] array = (from x in Machines()
				where x.identifier == "be_wine_printer" && Targets(x).Contains(barrel.uniqueId)
				select x).ToArray();
			if (array.Length != 1)
			{
				return null;
			}
			return array[0];
		}

		public static string BarrelName(GameItem barrel, string baseName)
		{
			int num = barrel.GetTagReadonly("BE36_BARREL_NUMBER").GetInt();
			if (num <= 0)
			{
				return baseName;
			}
			return baseName + " (" + num + ")";
		}

		private static void RefreshNames(GameItem[] machines)
		{
			GameItem[] source = (from x in machines
				group x by x.uniqueId into g
				where g.Key > 0 && g.Count() == 1
				select g.Single()).ToArray();
			HashSet<int> targets = new HashSet<int>(source.Where(IsSource).SelectMany(Targets));
			Dictionary<int, int> numbers = (from x in source
				where x.identifier == "be_large_brewing_barrel" && targets.Contains(x.uniqueId)
				orderby x.uniqueId
				select x).Select((GameItem item, int index) => (uniqueId: item.uniqueId, Number: index + 1)).ToDictionary(((int uniqueId, int Number) x) => x.uniqueId, ((int uniqueId, int Number) x) => x.Number);
			GameItem[] sources = source.Where((GameItem x) => IsSource(x) && Targets(x).Any(numbers.ContainsKey)).ToArray();
			GameItem[] array = sourceNames.Keys.Where((GameItem x) => !sources.Contains(x)).ToArray();
			foreach (GameItem val in array)
			{
				SourceName sourceName = sourceNames[val];
				if (Live(val) && val.name == sourceName.Applied)
				{
					val.SetName(sourceName.Base);
				}
				sourceNames.Remove(val);
				if (Live(val) && val.contentWindow != null)
				{
					val.contentWindow.SetTitle(val.GetDisplayName(true));
				}
			}
			foreach (IGrouping<int, GameItem> item in sources.GroupBy(Kind))
			{
				int num2 = 0;
				foreach (GameItem item2 in item.OrderBy((GameItem x) => x.uniqueId))
				{
					if (!sourceNames.TryGetValue(item2, out var value))
					{
						Dictionary<GameItem, SourceName> dictionary = sourceNames;
						SourceName obj = new SourceName
						{
							Base = item2.name
						};
						value = obj;
						dictionary[item2] = obj;
					}
					else if (item2.name != value.Applied)
					{
						value.Base = item2.name;
					}
					num2 = (value.Number = num2 + 1);
					value.Applied = value.Base + " (" + num2 + ")";
					if (item2.name != value.Applied)
					{
						item2.SetName(value.Applied);
					}
					if (item2.contentWindow != null)
					{
						item2.contentWindow.SetTitle(DisplayName(item2));
					}
				}
			}
			foreach (GameItem item3 in machines.Where((GameItem x) => x.identifier == "be_large_brewing_barrel"))
			{
				int value2;
				int number = (numbers.TryGetValue(item3.uniqueId, out value2) ? value2 : 0);
				if (item3.GetTagReadonly("BE36_BARREL_NUMBER").GetInt() != number)
				{
					item3.ModifyTag("BE36_BARREL_NUMBER", (Action<TagState>)delegate(TagState t)
					{
						t.Enable().SetInt(number);
					}, false);
				}
				string text = BarrelName(item3, ModHelper.GetLocalized("brewing_expansion", "barrel_name", Array.Empty<object>()));
				if (item3.name != text)
				{
					item3.SetName(text);
				}
				string text2 = DisplayName(item3);
				if (item3.contentWindow != null && item3.contentWindow.titleString != text2)
				{
					item3.contentWindow.SetTitle(text2);
				}
				BrewingBarrel.RefreshPrintButton(item3);
			}
		}

		public static void Request()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			if (ModEntry.Enabled && PlayerStore.Instance != null && !processing)
			{
				pending = true;
				queuedStore = PlayerStore.Instance;
				if ((Object)(object)pump == (Object)null)
				{
					GameObject val = new GameObject("BrewingExpansion.Connections");
					Object.DontDestroyOnLoad((Object)val);
					pump = val.AddComponent<ConnectionPump>();
				}
			}
		}

		internal static void Changed(GameItem item)
		{
			if (item != null && (item.identifier == "be_large_brewing_barrel" || item.identifier == "be_fruit_pulp_concentrator" || item.identifier == "be_wine_printer" || !string.IsNullOrEmpty(item.GetTagReadonly("BE36_MACHINE_LINKS").GetString())))
			{
				Request();
			}
		}

		internal static void Drain()
		{
			if (pending)
			{
				if (queuedStore != PlayerStore.Instance)
				{
					pending = false;
					queuedStore = null;
				}
				else if (PulpOperations.CanSave(queuedStore) && WaterBottlePrinter.CanSave(queuedStore))
				{
					pending = false;
					queuedStore = null;
					Process();
				}
			}
		}

		public static void Process()
		{
			PlayerStore instance = PlayerStore.Instance;
			if (!ModEntry.Enabled || instance == null || processing || !PulpOperations.CanSave(instance) || !WaterBottlePrinter.CanSave(instance))
			{
				return;
			}
			processing = true;
			try
			{
				GameItem[] array = Machines();
				Dictionary<int, GameItem> dictionary = (from x in array
					group x by x.uniqueId into g
					where g.Key > 0 && g.Count() == 1
					select g).ToDictionary((IGrouping<int, GameItem> g) => g.Key, (IGrouping<int, GameItem> g) => g.Single());
				RefreshNames(array);
				foreach (GameItem item in array.Where(IsSource).OrderBy(Kind).ThenBy((GameItem x) => x.uniqueId))
				{
					if (instance != PlayerStore.Instance)
					{
						return;
					}
					if (!Owned(item) || !dictionary.TryGetValue(item.uniqueId, out var value) || value != item)
					{
						continue;
					}
					int[] array2 = Targets(item);
					foreach (int key in array2)
					{
						if (instance != PlayerStore.Instance || !Owned(item))
						{
							break;
						}
						if (!dictionary.TryGetValue(key, out var value2) || value2.identifier != "be_large_brewing_barrel" || !Owned(value2))
						{
							continue;
						}
						PulpOperations.Busy.Add(item);
						PulpOperations.Store = instance;
						try
						{
							if (IsWaterSource(item))
							{
								if (WaterHelper.GetTotalVolume(item) > 0 && WaterHelper.GetFreeCapacity(value2) > 0)
								{
									WaterHelper.TransferLiquid(item, value2);
								}
							}
							else if (item.identifier == "be_fruit_pulp_concentrator")
							{
								GameItem[] array3 = ((GameInventory)PulpConcentrator.Output(item)).childItems.ToArray();
								foreach (GameItem val in array3)
								{
									if (!Owned(val) || val.identifier != "be_brewing_fruit_pulp" || FruitPulp.Remaining(val) == 0)
									{
										continue;
									}
									GameGridInventory val2 = PulpConcentrator.Output(item);
									SlotMarker val3 = BrewingBarrel.Input(value2).TryFindOneValidInventorySlot(val, false);
									if (val3 == null || !val3.IsValid())
									{
										continue;
									}
									try
									{
										val3.TryAcceptOnce(-1);
									}
									catch (Exception)
									{
										if (instance == PlayerStore.Instance && Live(val) && val.parentInventory == null)
										{
											SlotMarker obj = val2.TryFindOneValidInventorySlot(val, false);
											if (obj != null)
											{
												obj.TryAcceptOnce(-1);
											}
										}
										throw;
									}
									if (instance != PlayerStore.Instance)
									{
										return;
									}
									if (Live(val) && val.parentInventory == null)
									{
										PulpOperations.Protect(instance, "transferred pulp became detached");
									}
								}
							}
							if (instance == PlayerStore.Instance && Owned(value2))
							{
								BrewingBarrel.Refresh(value2);
							}
						}
						catch (Exception ex2)
						{
							Debug.LogWarning((object)("[BrewingExpansion.Formal] Connection transfer deferred: " + ex2.Message));
						}
						finally
						{
							PulpOperations.Busy.Remove(item);
							if (PulpOperations.Busy.Count == 0)
							{
								PulpOperations.Store = null;
							}
						}
					}
				}
				if (instance == PlayerStore.Instance)
				{
					GameItem[] array4 = OwnedItems();
					RefreshNames(array4.Where((GameItem x) => IsSource(x) || x.identifier == "be_large_brewing_barrel").ToArray());
					RefreshCableDescriptions(array4);
				}
			}
			finally
			{
				processing = false;
			}
		}

		internal static void Reset()
		{
			pending = false;
			queuedStore = null;
			if ((Object)(object)pump != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)pump).gameObject);
			}
			pump = null;
			foreach (KeyValuePair<GameItem, SourceName> sourceName in sourceNames)
			{
				if (Live(sourceName.Key) && sourceName.Key.name == sourceName.Value.Applied)
				{
					sourceName.Key.SetName(sourceName.Value.Base);
					if (sourceName.Key.contentWindow != null)
					{
						sourceName.Key.contentWindow.SetTitle(sourceName.Key.GetDisplayName(true));
					}
				}
			}
			sourceNames.Clear();
			overviewItems.Clear();
			overview = Array.Empty<ConnectionGroup>();
			overviewStore = null;
			CableTooltipLayout.Reset();
		}
	}
	public static class TransferCable
	{
		public const string Id = "be_xuxu_connection_pliers";

		public const string SelectedTag = "BE36_SELECTED_SOURCE";

		public static GameItem Create()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			GameItem obj = ItemDirectory.CreateEmptyItem("be_xuxu_connection_pliers");
			obj.SetValue(200L);
			obj.SetGameItemType("TOOL");
			obj.SetSpriteAndShapeFromMod("brewing_expansion", "be_xuxu_transfer_line");
			obj.SetShape(new GridShapeBuilder(3, 1).SetDataFill(3, 1, (byte)1).Build());
			((GameItemFunc)obj).mayThisTargetItemFunc = (GameItem tool, GameItem target) => CanUse(tool, target);
			((GameItemFunc)obj).canThisTargetItemFunc = (GameItem tool, GameItem target) => CanUse(tool, target);
			((GameItemFunc)obj).onThisTargetItemFunc = Use;
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Loaded));
			Loaded(obj);
			return obj;
		}

		private static void Loaded(GameItem item)
		{
			item.SetName(ModHelper.GetLocalized("brewing_expansion", "cable_name", Array.Empty<object>()));
			MachineConnections.RefreshCableDescriptions();
			MachineConnections.RefreshCableDescription(item);
			ItemHitArea.Apply(item);
			MachineConnections.Request();
		}

		internal static void RefreshDescription(GameItem item, GameItem selected, string[] connections)
		{
			string localized = ModHelper.GetLocalized("brewing_expansion", "cable_description", Array.Empty<object>());
			localized = localized + "\n\n" + ((selected != null) ? ModHelper.GetLocalized("brewing_expansion", "cable_selected", new object[1] { MachineConnections.DisplayName(selected) }) : ModHelper.GetLocalized("brewing_expansion", "cable_no_selection", Array.Empty<object>()));
			localized = localized + "\n" + ((connections.Length == 0) ? ModText.Get("cable_no_links") : ModText.Get("cable_linked", string.Join("\n", connections)));
			string shortDescription = (item.longDescription = localized);
			item.shortDescription = shortDescription;
		}

		internal static void Tooltip(RichTextBuilder builder, GameItem item)
		{
			if (ModEntry.Enabled && builder != null && !(((item != null) ? item.identifier : null) != "be_xuxu_connection_pliers"))
			{
				MachineConnections.RefreshCableDescription(item);
				item.shortDescription = CableTooltipLayout.Wrap(item.shortDescription);
				builder.maxCharWidth = 16384;
			}
		}

		public static bool CanUse(GameItem tool, GameItem target)
		{
			if (ModEntry.Enabled && ((tool != null) ? tool.identifier : null) == "be_xuxu_connection_pliers" && MachineConnections.Owned(tool) && MachineConnections.Owned(target))
			{
				if (!MachineConnections.IsSource(target))
				{
					return target.identifier == "be_large_brewing_barrel";
				}
				return true;
			}
			return false;
		}

		public static void Use(GameItem tool, GameItem target)
		{
			if (!CanUse(tool, target))
			{
				return;
			}
			PlayerStore instance = PlayerStore.Instance;
			int selectedId = tool.GetTagReadonly("BE36_SELECTED_SOURCE").GetInt();
			GameItem[] array = (from x in instance.FindAllItem(true)
				where x.uniqueId == selectedId && MachineConnections.Owned(x) && (MachineConnections.IsSource(x) || x.identifier == "be_large_brewing_barrel")
				select x).ToArray();
			if (array.Length != 1 || MachineConnections.IsSource(array[0]) == MachineConnections.IsSource(target))
			{
				tool.ModifyTag("BE36_SELECTED_SOURCE", (Action<TagState>)delegate(TagState t)
				{
					t.Enable().SetInt(target.uniqueId);
				}, false);
			}
			else
			{
				GameItem source = (MachineConnections.IsSource(target) ? target : array[0]);
				GameItem barrel = (MachineConnections.IsSource(target) ? array[0] : target);
				MachineConnections.Toggle(source, barrel);
			}
			if (instance != PlayerStore.Instance)
			{
				return;
			}
			Loaded(tool);
			try
			{
				AudioManager instance2 = AudioManager.Instance;
				if (instance2 != null)
				{
					instance2.Play2("turbo");
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Cable audio failed: " + ex.Message));
			}
		}
	}
	internal sealed class ConnectionPump : MonoBehaviour
	{
		private void LateUpdate()
		{
			MachineConnections.Drain();
		}
	}
	internal static class ConnectedWaterAdded
	{
		private static void Postfix(GameItem __0)
		{
			MachineConnections.Changed(__0);
		}
	}
	internal static class ConnectedWaterEmptied
	{
		private static void Postfix(GameItem __0)
		{
			MachineConnections.Changed(__0);
		}
	}
	internal static class ConnectedWaterTransfer
	{
		private static void Postfix(GameItem __0, GameItem __1)
		{
			MachineConnections.Changed(__0);
			MachineConnections.Changed(__1);
		}
	}
	[HarmonyPatch(typeof(GeneralHelper), "SetItemOwned")]
	internal static class ConnectedOwnershipChanged
	{
		private static void Postfix(GameItem __0)
		{
			MachineConnections.Changed(__0);
		}
	}
	[HarmonyPatch(typeof(GameItemElement), "Destroy")]
	internal static class ConnectedItemDestroyed
	{
		private static void Postfix(GameItemElement __instance)
		{
			MachineConnections.Changed((GameItem)(object)__instance);
		}
	}
	internal static class MachineGuides
	{
		internal const string Marker = "BE_MACHINE_GUIDE";

		internal static readonly string[] Ids = new string[6] { "be_incubator", "be_fruit_pulp_concentrator", "be_large_brewing_barrel", "be_wine_printer", "be_xuxu_water_bottle_printer", "be_liunian_ager" };

		[ThreadStatic]
		internal static int Decoding;

		internal static bool Known(GameItem item)
		{
			if (item != null)
			{
				return Array.IndexOf(Ids, item.identifier) >= 0;
			}
			return false;
		}

		internal static GameSlotInventory Slot(GameItem item)
		{
			object obj;
			if (item == null)
			{
				obj = null;
			}
			else
			{
				PixelWindow contentWindow = item.contentWindow;
				obj = ((contentWindow != null) ? contentWindow.childElement : null);
			}
			object obj2 = ((obj is MachineryPanelRow) ? obj : null);
			MachineryPanel obj3 = ((obj2 != null) ? ((MachineryPanelRow)obj2).GetPanel("note") : null);
			PixelElement obj4 = ((obj3 != null) ? obj3.content : null);
			PixelElement obj5 = ((obj4 is MachineryIntake) ? obj4 : null);
			PixelElement obj6 = ((obj5 != null) ? ((MachineryIntake)obj5).content : null);
			return (GameSlotInventory)(object)((obj6 is GameSlotInventory) ? obj6 : null);
		}

		internal static void Attach(GameItem item)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Expected O, but got Unknown
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Expected O, but got Unknown
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected O, but got Unknown
			if (!Known(item))
			{
				return;
			}
			PixelElement childElement = item.contentWindow.childElement;
			MachineryPanelRow val = (MachineryPanelRow)(object)((childElement is MachineryPanelRow) ? childElement : null);
			if (val != null)
			{
				if (!val.Panels.Any((MachineryPanel p) => p.role == "note"))
				{
					val.Add(MachineHelper.NotePanel(new GameSlotInventory(1, 1, false, false)
					{
						identifier = "BE_MACHINE_NOTE"
					}));
				}
				else if (item.identifier == "be_liunian_ager")
				{
					val.GetPanel("note").SetContent((PixelElement)null);
					val.GetPanel("note").SetContent((PixelElement)new MachineryIntake((PixelElement)new GameSlotInventory(1, 1, false, false)
					{
						identifier = "BE_MACHINE_NOTE"
					}, true, true));
				}
				GameItem obj = item;
				((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Ensure));
				PixelWindow contentWindow = item.contentWindow;
				contentWindow.openCallback = (Action)Delegate.Combine(contentWindow.openCallback, (Action)delegate
				{
					Ensure(item);
				});
			}
		}

		internal static void Ensure(GameItem item)
		{
			if (!ModEntry.Enabled || !Known(item) || Decoding > 0)
			{
				return;
			}
			try
			{
				GameSlotInventory val = Slot(item);
				if (val == null)
				{
					return;
				}
				GameItem val2 = val.childItem;
				if (val2 == null)
				{
					val2 = PreBuiltItemHelper.CreateHandnote(Body(item), Title(item), "xuxu", 0);
					val2.ModifyTag("BE_MACHINE_GUIDE", (Action<TagState>)delegate(TagState t)
					{
						t.Enable().SetString(item.identifier);
					}, false);
					val2.SetValue(0L);
					GeneralHelper.SetItemOwned(val2, GeneralHelper.IsItemOwned(item));
					SlotMarker val3 = ((GameInventory)val).TryFindOneValidInventorySlot(val2);
					if (val3 == null || !val3.IsValid())
					{
						val2.Destroy();
						return;
					}
					val3.TryAcceptOnce(-1);
				}
				if (!(((val2 != null) ? val2.GetTagReadonly("BE_MACHINE_GUIDE").GetString() : null) != item.identifier))
				{
					string title = Title(item);
					string body = Body(item);
					val2.ModifyTag("HANDNOTE_TITLE", (Action<TagState>)delegate(TagState t)
					{
						t.Enable().SetString(title);
					}, false);
					val2.ModifyTag("HANDNOTE_BODY", (Action<TagState>)delegate(TagState t)
					{
						t.Enable().SetString(body);
					}, false);
					val2.SetName(title);
					GameItem obj = val2;
					string shortDescription = (val2.longDescription = body);
					obj.shortDescription = shortDescription;
					GeneralHelper.SetItemOwned(val2, GeneralHelper.IsItemOwned(item));
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Machine guide deferred: " + ex.Message));
			}
		}

		internal static string Title(GameItem item)
		{
			return ModHelper.GetLocalized("brewing_expansion", item.identifier + "_guide_title", Array.Empty<object>());
		}

		internal static string Body(GameItem item)
		{
			string text = ModHelper.GetLocalized("brewing_expansion", item.identifier + "_guide_body", Array.Empty<object>());
			if (item.identifier == "be_liunian_ager")
			{
				text = text + "\n\n" + ModText.Get("be_liunian_current_stats", MachineryHelper.GetCurrentQualityBonus(item), MachineWineRack.GetAgingDays(item), MachineryHelper.GetMachinePowerUsage(item));
			}
			return text;
		}
	}
	[HarmonyPatch(typeof(DirectoryMaster), "Item", new Type[]
	{
		typeof(string),
		typeof(bool)
	})]
	internal static class MachineGuideCreated
	{
		private static void Postfix(GameItem __result)
		{
			if (MachineGuides.Decoding == 0)
			{
				MachineGuides.Ensure(__result);
			}
		}
	}
	[HarmonyPatch(typeof(SaveManager), "DecodeNodes")]
	internal static class MachineGuideDecode
	{
		private static void Prefix()
		{
			MachineGuides.Decoding++;
		}

		private static Exception Finalizer(Exception __exception)
		{
			MachineGuides.Decoding--;
			return __exception;
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "BuyItem")]
	internal static class MachineGuideBought
	{
		private static void Postfix(GameItem __0)
		{
			MachineGuides.Ensure(__0);
		}
	}
	public sealed class ModEntry : IMod, IServiceProvider
	{
		public const string Id = "brewing_expansion";

		private ModLog log;

		private bool debugRegistered;

		private readonly List<(ModItemDirectory Directory, string Id, Func<GameItem> Factory)> bindings = new List<(ModItemDirectory, string, Func<GameItem>)>();

		internal static bool Enabled { get; private set; }

		public void Init(ModManifest manifest)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			log = new ModLog(manifest);
		}

		public object GetService(Type serviceType)
		{
			if (!Enabled || serviceType != typeof(Func<string, string, string, int, Func<GameItem>, IDisposable>))
			{
				return null;
			}
			return new Func<string, string, string, int, Func<GameItem>, IDisposable>(IncubatorRecipeRegistry.Register);
		}

		public void OnEnable()
		{
			if (Enabled)
			{
				return;
			}
			foreach (string item in ItemIdentity.Catalogue())
			{
				if (!item.StartsWith("be_", StringComparison.Ordinal))
				{
					throw new InvalidOperationException("Custom item ID must use be_: " + item);
				}
			}
			Enabled = true;
			ModHook.OnCreateTooltipLate += WineChemistry.AppendTooltip;
			ModHook.OnCreateTooltipLate += TransferCable.Tooltip;
			ModHook.OnModItemDirectoryInit += Register;
			ModHook.OnWakingUpLate += BrewingBarrel.Wake;
			ModHook.OnGoingSleepLate += BrewingBarrel.Wake;
			ModHook.OnGoingSleepLate += SeedIncubator.CompleteNight;
			ModHook.OnGoingSleepEarly += LiunianCabinet.BeginNight;
			ModHook.OnGameLoadedLate += LiunianCabinet.Reset;
			ModHook.OnWakingUpLate += LiunianCabinet.FinishNight;
			ModHook.OnGoingSleepLate += LiunianCabinet.FinishNight;
			ModHook.OnGameLoadedLate += MachineConnections.Request;
			ModHook.OnGameLoadedEarly += FormalVisitors.RegisterFactories;
			ModHook.OnGameLoadedLate += FormalEvents.Loaded;
			ModHook.OnGenerateCustomerVeryLate += FormalEvents.Generated;
			ModHook.OnNewGameStarted += FormalEvents.NewGame;
			ModHook.OnGoingSleepLate += FormalEvents.AfterNight;
			FormalVisitors.RegisterFactories();
			FormalShop.Register();
			FormalTraits.Register();
			ModItemDirectory[] array = Resources.FindObjectsOfTypeAll<ModItemDirectory>();
			foreach (ModItemDirectory directory in array)
			{
				Register(directory);
			}
			LiquidPatches.Enable(this, log);
			log.Log((object)"Formal catalogue enabled: machines, water containers, consumables, native music cassettes, Outer Ring visitors and independent night shop.");
		}

		public void OnDisable()
		{
			if (!Enabled)
			{
				return;
			}
			Enabled = false;
			LiquidPatches.Disable();
			ModHook.OnCreateTooltipLate -= WineChemistry.AppendTooltip;
			ModHook.OnCreateTooltipLate -= TransferCable.Tooltip;
			ModHook.OnModItemDirectoryInit -= Register;
			ModHook.OnWakingUpLate -= BrewingBarrel.Wake;
			ModHook.OnGoingSleepLate -= BrewingBarrel.Wake;
			ModHook.OnGoingSleepLate -= SeedIncubator.CompleteNight;
			ModHook.OnGoingSleepEarly -= LiunianCabinet.BeginNight;
			ModHook.OnGameLoadedLate -= LiunianCabinet.Reset;
			ModHook.OnWakingUpLate -= LiunianCabinet.FinishNight;
			ModHook.OnGoingSleepLate -= LiunianCabinet.FinishNight;
			ModHook.OnGameLoadedLate -= MachineConnections.Request;
			ModHook.OnGameLoadedEarly -= FormalVisitors.RegisterFactories;
			ModHook.OnGameLoadedLate -= FormalEvents.Loaded;
			ModHook.OnGenerateCustomerVeryLate -= FormalEvents.Generated;
			ModHook.OnNewGameStarted -= FormalEvents.NewGame;
			ModHook.OnGoingSleepLate -= FormalEvents.AfterNight;
			FormalShop.Remove();
			FormalVisitors.Remove();
			FormalTraitIcons.Remove();
			ModHelper.RemoveStartingPerks("brewing_expansion");
			ModHelper.RemoveStartingScenarios("brewing_expansion");
			ModHelper.RemoveDebugFunctions("brewing_expansion");
			debugRegistered = false;
			PulpConcentrator.Reset();
			MachineConnections.Reset();
			LiunianCabinet.Reset();
			FormalAssassinEnding.Reset();
			foreach (var binding in bindings)
			{
				if (!((Object)(object)binding.Directory == (Object)null) && ((Directory<GameItem>)(object)binding.Directory).factoryDictionary.TryGetValue(binding.Id, out var value) && (object)value == binding.Factory)
				{
					((Directory<GameItem>)(object)binding.Directory).Remove(binding.Id);
				}
			}
			bindings.Clear();
			log.Log((object)"Formal foundation disabled; subscriptions and debug actions removed.");
		}

		private void RegisterDebugFunctions()
		{
			if (debugRegistered || !Enabled)
			{
				return;
			}
			debugRegistered = true;
			FigurineDefinition[] all = Figurines.All;
			foreach (FigurineDefinition figurineDefinition in all)
			{
				FigurineDefinition item = figurineDefinition;
				ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", item.Id + "_name", Array.Empty<object>()), (Action)delegate
				{
					ModHelper.MoveItemToInventory(ModHelper.SpawnItem(item.Id));
				}, false);
			}
			WaterContainerDefinition[] all2 = WaterContainers.All;
			foreach (WaterContainerDefinition waterContainerDefinition in all2)
			{
				WaterContainerDefinition item2 = waterContainerDefinition;
				ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", item2.Id + "_name", Array.Empty<object>()), (Action)delegate
				{
					ModHelper.MoveItemToInventory(ModHelper.SpawnItem(item2.Id));
				}, false);
			}
			ModHelper.AddDebugFunction("brewing_expansion", "Modding", ModHelper.GetLocalized("brewing_expansion", "debug_foundation_check", Array.Empty<object>()), (Action)CatalogueDiagnostics.Run, false);
			ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", "water_printer_name", Array.Empty<object>()), (Action)delegate
			{
				ModHelper.MoveItemToInventory(ModHelper.SpawnItem("be_xuxu_water_bottle_printer"));
			}, false);
			BrewingFruits.Definition[] all3 = BrewingFruits.All;
			foreach (BrewingFruits.Definition definition in all3)
			{
				BrewingFruits.Definition fruit = definition;
				ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", fruit.Id + "_name", Array.Empty<object>()), (Action)delegate
				{
					ModHelper.MoveItemToInventory(ModHelper.SpawnItem(fruit.Id));
				}, false);
			}
			ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", "concentrator_name", Array.Empty<object>()), (Action)delegate
			{
				ModHelper.MoveItemToInventory(ModHelper.SpawnItem("be_fruit_pulp_concentrator"));
			}, false);
			ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", "barrel_name", Array.Empty<object>()), (Action)delegate
			{
				ModHelper.MoveItemToInventory(ModHelper.SpawnItem("be_large_brewing_barrel"));
			}, false);
			ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", "cable_name", Array.Empty<object>()), (Action)delegate
			{
				ModHelper.MoveItemToInventory(ModHelper.SpawnItem("be_xuxu_connection_pliers"));
			}, false);
			ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", "wine_printer_name", Array.Empty<object>()), (Action)delegate
			{
				ModHelper.MoveItemToInventory(ModHelper.SpawnItem("be_wine_printer"));
			}, false);
			BrewingSeeds.Definition[] all4 = BrewingSeeds.All;
			foreach (BrewingSeeds.Definition definition2 in all4)
			{
				BrewingSeeds.Definition seed = definition2;
				ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", seed.Id + "_name", Array.Empty<object>()), (Action)delegate
				{
					ModHelper.MoveItemToInventory(ModHelper.SpawnItem(seed.Id));
				}, false);
			}
			string[] ids = BrewingCertificates.Ids;
			foreach (string text in ids)
			{
				string certificate = text;
				ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", text + "_name", Array.Empty<object>()), (Action)delegate
				{
					ModHelper.MoveItemToInventory(ModHelper.SpawnItem(certificate));
				}, false);
			}
			ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", "incubator_name", Array.Empty<object>()), (Action)delegate
			{
				ModHelper.MoveItemToInventory(ModHelper.SpawnItem("be_incubator"));
			}, false);
			ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", "liunian_name", Array.Empty<object>()), (Action)delegate
			{
				ModHelper.MoveItemToInventory(ModHelper.SpawnItem("be_liunian_ager"));
			}, false);
			RemainingItems.Definition[] all5 = RemainingItems.All;
			foreach (RemainingItems.Definition definition3 in all5)
			{
				RemainingItems.Definition item3 = definition3;
				ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", item3.Id + "_name", Array.Empty<object>()), (Action)delegate
				{
					ModHelper.MoveItemToInventory(ModHelper.SpawnItem(item3.Id));
				}, false);
			}
			for (int num = 0; num < FormalMusic.Ids.Length; num++)
			{
				int index = num;
				ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", FormalMusic.Ids[index] + "_name", Array.Empty<object>()), (Action)delegate
				{
					ModHelper.MoveItemToInventory(ModHelper.SpawnItem(FormalMusic.Ids[index]));
				}, false);
			}
			BarrelWine.Definition[] all6 = BarrelWine.All;
			foreach (BarrelWine.Definition definition4 in all6)
			{
				BarrelWine.Definition wine = definition4;
				ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", wine.Id + "_name", Array.Empty<object>()), (Action)delegate
				{
					ModHelper.MoveItemToInventory(ModHelper.SpawnItem(wine.Id));
				}, false);
			}
		}

		private void Bind(ModItemDirectory directory, string id, Func<GameItem> factory)
		{
			Func<GameItem> create = factory;
			factory = delegate
			{
				GameItem obj = create();
				MachineGuides.Attach(obj);
				return obj;
			};
			if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey(id) && ((Directory<GameItem>)(object)directory).Add(id, factory))
			{
				bindings.Add((directory, id, factory));
			}
		}

		private void Register(ModItemDirectory directory)
		{
			FormalTraitIcons.Register();
			FigurineDefinition[] all = Figurines.All;
			foreach (FigurineDefinition figurineDefinition in all)
			{
				if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey(figurineDefinition.Id))
				{
					FigurineDefinition item = figurineDefinition;
					Func<GameItem> func = () => Figurines.Create(item);
					if (((Directory<GameItem>)(object)directory).Add(item.Id, func))
					{
						bindings.Add((directory, item.Id, func));
					}
					else
					{
						log.Warning((object)("Item registration refused: " + item.Id));
					}
				}
			}
			WaterContainerDefinition[] all2 = WaterContainers.All;
			foreach (WaterContainerDefinition waterContainerDefinition in all2)
			{
				if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey(waterContainerDefinition.Id))
				{
					WaterContainerDefinition item2 = waterContainerDefinition;
					Func<GameItem> func2 = () => WaterContainers.Create(item2);
					if (((Directory<GameItem>)(object)directory).Add(item2.Id, func2))
					{
						bindings.Add((directory, item2.Id, func2));
					}
					else
					{
						log.Warning((object)("Item registration refused: " + item2.Id));
					}
				}
			}
			if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey("be_xuxu_water_bottle_printer"))
			{
				Func<GameItem> func3 = delegate
				{
					GameItem obj = WaterBottlePrinter.Create();
					MachineGuides.Attach(obj);
					return obj;
				};
				if (((Directory<GameItem>)(object)directory).Add("be_xuxu_water_bottle_printer", func3))
				{
					bindings.Add((directory, "be_xuxu_water_bottle_printer", func3));
				}
				else
				{
					log.Warning((object)"Item registration refused: be_xuxu_water_bottle_printer");
				}
			}
			BrewingFruits.Definition[] all3 = BrewingFruits.All;
			foreach (BrewingFruits.Definition definition in all3)
			{
				BrewingFruits.Definition fruit = definition;
				Bind(directory, fruit.Id, () => BrewingFruits.Create(fruit));
			}
			BrewingSeeds.Definition[] all4 = BrewingSeeds.All;
			foreach (BrewingSeeds.Definition definition2 in all4)
			{
				BrewingSeeds.Definition definition3 = definition2;
				Bind(directory, definition2.Id, () => BrewingSeeds.Create(definition3));
			}
			string[] ids = BrewingCertificates.Ids;
			foreach (string text in ids)
			{
				string certificate = text;
				Bind(directory, text, () => BrewingCertificates.Create(certificate));
			}
			Bind(directory, "be_incubator", SeedIncubator.Create);
			Bind(directory, "be_liunian_ager", LiunianCabinet.Create);
			Bind(directory, "be_wine_printer", WineBottlePrinter.Create);
			Bind(directory, "be_xuxu_connection_pliers", TransferCable.Create);
			Bind(directory, "be_large_brewing_barrel", BrewingBarrel.Create);
			Bind(directory, "be_brewing_fruit_pulp", FruitPulp.Create);
			Bind(directory, "be_fruit_pulp_concentrator", PulpConcentrator.Create);
			RemainingItems.Definition[] all5 = RemainingItems.All;
			foreach (RemainingItems.Definition definition4 in all5)
			{
				RemainingItems.Definition item3 = definition4;
				Bind(directory, item3.Id, () => RemainingItems.Create(item3));
			}
			for (int num = 0; num < FormalMusic.Ids.Length; num++)
			{
				int index = num;
				Bind(directory, FormalMusic.Ids[index], () => FormalMusic.Create(index));
			}
			BarrelWine.Definition[] all6 = BarrelWine.All;
			foreach (BarrelWine.Definition definition5 in all6)
			{
				BarrelWine.Definition wine = definition5;
				Bind(directory, wine.Id, () => BarrelWine.Create(wine));
			}
			RegisterDebugFunctions();
			log.Log((object)"Registered formal catalogue: figurines, water containers, fruits, pulp, concentrator, barrel, both printers and transmission cable.");
		}
	}
	internal static class ModText
	{
		internal static string Get(string key, params object[] args)
		{
			return ModHelper.GetLocalized("brewing_expansion", key, args);
		}
	}
	internal static class ModuleExpansion
	{
		private static readonly HashSet<string> Ids = new HashSet<string>(ItemIdentity.Catalogue(), StringComparer.Ordinal);

		internal static bool Rejects(GameItem item)
		{
			if (!ModEntry.Enabled || item == null || !Ids.Contains(item.identifier))
			{
				return false;
			}
			string text = item.GetTagReadonly("MOD_ID_TAG").GetString();
			if (!string.IsNullOrEmpty(text))
			{
				return text == "brewing_expansion";
			}
			return true;
		}

		internal static void Bind(GameItem kit)
		{
			if (kit == null)
			{
				return;
			}
			Func<GameItem, GameItem, bool> may = ((GameItemFunc)kit).mayItemTargetItemFunc;
			Func<GameItem, GameItem, bool> can = ((GameItemFunc)kit).canItemTargetItemFunc;
			Action<GameItem, GameItem> use = ((GameItemFunc)kit).onItemTargetItemFunc;
			((GameItemFunc)kit).mayItemTargetItemFunc = (GameItem tool, GameItem target) => !Rejects(target) && (may == null || may(tool, target));
			((GameItemFunc)kit).canItemTargetItemFunc = (GameItem tool, GameItem target) => !Rejects(target) && (can == null || can(tool, target));
			((GameItemFunc)kit).onItemTargetItemFunc = delegate(GameItem tool, GameItem target)
			{
				if (!Rejects(target))
				{
					use?.Invoke(tool, target);
				}
			};
		}

		internal static void RestoreCabinet(GameItem item)
		{
			if (!Rejects(item) || item.identifier != "be_liunian_ager" || item.GetTagReadonly("MODULE_UPGRADE_STAGE_INT").GetInt() <= 0)
			{
				return;
			}
			GameGridInventory val = LiunianCabinet.Modules(item);
			if (val != null && !((GameInventory)val).childItems.Any((GameItem i) => i.modifiedShape.minX < 0 || i.modifiedShape.minY < 0 || i.modifiedShape.maxX >= 6 || i.modifiedShape.maxY >= 20))
			{
				val.SetShape(6, 20);
				item.ModifyTag("MODULE_UPGRADE_STAGE_INT", (Action<TagState>)delegate(TagState t)
				{
					t.SetInt(0);
				}, false);
			}
		}
	}
	[HarmonyPatch(typeof(ModuleDirectory), "ModuleBayExpansionKit")]
	internal static class BrewingModuleExpansionKit
	{
		private static void Postfix(GameItem __result)
		{
			ModuleExpansion.Bind(__result);
		}
	}
	internal static class NiniCertificates
	{
		private const string CreditKey = "nini_certificate_credit";

		private static readonly long[] Costs = new long[3] { 1500L, 3000L, 8000L };

		private static bool issuing;

		internal static void Sold(GameItem item, StoreClient client)
		{
			if (!ModEntry.Enabled || item == null || client?.identifier != "be_weekly_trader")
			{
				return;
			}
			try
			{
				long num = Math.Max(0L, item.GetBaseValue());
				long num2 = ReadCredit();
				WriteCredit((num2 > long.MaxValue - num) ? long.MaxValue : (num2 + num));
				Issue(client, reconcileOwned: false);
			}
			catch (Exception error)
			{
				Warn(error);
			}
		}

		internal static void Retry(StoreClient client = null)
		{
			Issue(client, reconcileOwned: true);
		}

		private static void Issue(StoreClient client, bool reconcileOwned)
		{
			if (!ModEntry.Enabled || issuing || PlayerStore.Instance == null || (Object)(object)EmporiumEntry.Instance == (Object)null)
			{
				return;
			}
			issuing = true;
			GameItem val = null;
			try
			{
				int i;
				for (i = 0; i < Costs.Length && ModHelper.GetModDataBool("brewing_expansion", IssuedKey(i), false); i++)
				{
				}
				if (i == Costs.Length)
				{
					return;
				}
				long num = ReadCredit();
				if (!reconcileOwned && num < Costs[i])
				{
					return;
				}
				int num2 = FormalVisitors.CertificateLevel();
				for (int j = 0; j < num2; j++)
				{
					ModHelper.SetModDataBool("brewing_expansion", IssuedKey(j), true);
				}
				for (int k = 0; k < Costs.Length; k++)
				{
					if (!ModHelper.GetModDataBool("brewing_expansion", IssuedKey(k), false))
					{
						if (num < Costs[k])
						{
							break;
						}
						val = ModHelper.SpawnItem(BrewingCertificates.Ids[k]);
						if (val == null || val.identifier != BrewingCertificates.Ids[k])
						{
							throw new InvalidOperationException("Certificate factory did not create " + BrewingCertificates.Ids[k]);
						}
						GameGridInventory frontInvinvElement = EmporiumEntry.Instance.frontInvinvElement;
						SlotMarker val2 = ((frontInvinvElement != null) ? frontInvinvElement.TryFindOneValidInventorySlot(val, false) : null);
						if (val2 != null && val2.IsValid())
						{
							val2.TryAcceptOnce(-1);
						}
						if (val.parentInventory == null)
						{
							Tell(client, pending: true);
							break;
						}
						val = null;
						num = ReadCredit() - Costs[k];
						ModHelper.SetModDataBool("brewing_expansion", IssuedKey(k), true);
						WriteCredit(num);
						Tell(client, pending: false);
					}
				}
			}
			catch (Exception error)
			{
				Warn(error);
			}
			finally
			{
				try
				{
					if (val != null && val.parentInventory == null)
					{
						val.Destroy();
					}
				}
				catch (Exception error2)
				{
					Warn(error2);
				}
				finally
				{
					issuing = false;
				}
			}
		}

		private static string IssuedKey(int n)
		{
			return "certificate_issued_" + n;
		}

		private static long ReadCredit()
		{
			long.TryParse(ModHelper.GetModData("brewing_expansion", "nini_certificate_credit", "0"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result);
			return Math.Max(0L, result);
		}

		private static void WriteCredit(long credit)
		{
			ModHelper.SetModData("brewing_expansion", "nini_certificate_credit", credit.ToString(CultureInfo.InvariantCulture));
		}

		private static void Tell(StoreClient client, bool pending)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (!(client?.identifier != "be_weekly_trader"))
			{
				client.acceptDealDialogue = new Dialogue().SetText(client.displayName, pending ? ModText.Get("be_certificate_delivery_pending") : ModText.Get("be_certificate_awarded"));
				client.acceptDealDialoguePlayed = false;
			}
		}

		private static void Warn(Exception error)
		{
			Debug.LogWarning((object)("[BrewingExpansion] Certificate processing deferred; saved progress is preserved: " + error));
		}
	}
	public static class PulpOperations
	{
		internal static readonly HashSet<GameItem> Busy = new HashSet<GameItem>();

		internal static PlayerStore Store;

		private static WeakReference<PlayerStore> unsafeStore;

		public static bool CanSave(PlayerStore store)
		{
			if (Store != store || Busy.Count <= 0)
			{
				if (unsafeStore != null && unsafeStore.TryGetTarget(out var target))
				{
					return store != target;
				}
				return true;
			}
			return false;
		}

		internal static void Protect(PlayerStore store, string reason)
		{
			unsafeStore = new WeakReference<PlayerStore>(store);
			Debug.LogError((object)("[BrewingExpansion.Formal] Pulp transaction uncertain; saving blocked this session: " + reason));
		}
	}
	public static class FruitPulp
	{
		public const string Id = "be_brewing_fruit_pulp";

		public const string RemainingTag = "BE_PULP_REMAINING_USES_V1";

		private const string Prefix = "BE_PULP_FRUIT_";

		public static string FruitId(GameItem pulp)
		{
			if (pulp == null || pulp.identifier != "be_brewing_fruit_pulp")
			{
				return null;
			}
			if (pulp.IsTag("BE_PULP_FRUIT_wine_berry"))
			{
				return "wine_berry";
			}
			BrewingFruits.Definition[] all = BrewingFruits.All;
			foreach (BrewingFruits.Definition definition in all)
			{
				if (definition.Concentratable && pulp.IsTag("BE_PULP_FRUIT_" + definition.Id))
				{
					return definition.Id;
				}
			}
			return null;
		}

		public static int Remaining(GameItem pulp)
		{
			return Math.Max(0, Math.Min(4, pulp.GetTagReadonly("BE_PULP_REMAINING_USES_V1").GetInt()));
		}

		public static GameItem Create()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			GameItem obj = ItemDirectory.CreateEmptyItem("be_brewing_fruit_pulp");
			obj.SetValue(0L);
			obj.SetGameItemType("FOOD");
			obj.SetSpriteAndShapeFromMod("brewing_expansion", "be_pulp_wine_berry");
			obj.SetShape(new GridShapeBuilder(2, 2).SetDataFill(2, 2, (byte)1).Build());
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Refresh));
			Refresh(obj);
			return obj;
		}

		internal static void Configure(GameItem pulp, string id, int score, long value)
		{
			pulp.EnableTag("BE_PULP_FRUIT_" + id, false);
			pulp.ModifyTag("BE_PULP_BERRY_SCORE_SUM_INT", (Action<TagState>)delegate(TagState t)
			{
				t.Enable().SetInt(score);
			}, false);
			pulp.SetValue(value);
			Refresh(pulp);
		}

		internal static void Refresh(GameItem item)
		{
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			string text = FruitId(item);
			string text2 = ((text == "wine_berry") ? ModHelper.GetLocalized("brewing_expansion", "pulp_native_berry", Array.Empty<object>()) : ((text == null) ? null : ModHelper.GetLocalized("brewing_expansion", text + "_name", Array.Empty<object>())));
			item.SetName((text == null) ? ModHelper.GetLocalized("brewing_expansion", "pulp_name", Array.Empty<object>()) : ModHelper.GetLocalized("brewing_expansion", "pulp_named", new object[1] { text2 }));
			string shortDescription = (item.longDescription = ModHelper.GetLocalized("brewing_expansion", "pulp_description", Array.Empty<object>()));
			item.shortDescription = shortDescription;
			if (text != null)
			{
				item.SetSpriteAndShapeFromMod("brewing_expansion", ItemIdentity.PulpSprite(text));
			}
			item.SetShape(new GridShapeBuilder(2, 2).SetDataFill(2, 2, (byte)1).Build());
			ItemHitArea.Apply(item);
		}
	}
	public static class PulpConcentrator
	{
		private sealed class View
		{
			internal MachineryText Status;
		}

		public const string Id = "be_fruit_pulp_concentrator";

		private static readonly ConditionalWeakTable<GameItem, View> Views = new ConditionalWeakTable<GameItem, View>();

		private static readonly Dictionary<GameItem, PlayerStore> Pending = new Dictionary<GameItem, PlayerStore>();

		private static PulpPump pump;

		private static string Text(string key)
		{
			return ModHelper.GetLocalized("brewing_expansion", key, Array.Empty<object>());
		}

		private static bool Live(GameItem item)
		{
			if (item != null)
			{
				GameItem obj = ((item is GameItemElement) ? item : null);
				if (obj == null)
				{
					return true;
				}
				return !((GameItemElement)obj).IsDestroyed();
			}
			return false;
		}

		private static bool Owned(GameItem item)
		{
			if (Live(item))
			{
				return GeneralHelper.IsItemOwned(item);
			}
			return false;
		}

		public static GameSlotInventory Battery(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 0);
			return (GameSlotInventory)(object)((bodyStorage is GameSlotInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Input(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 1);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Output(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 2);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		public static GameItem Create()
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Expected O, but got Unknown
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Expected O, but got Unknown
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			GameItem item = ItemDirectory.CreateEmptyItem("be_fruit_pulp_concentrator");
			item.SetValue(500L);
			item.SetGameItemType("MACHINE");
			item.EnableTag("CONTAINER_TAG", false);
			item.SetSpriteAndShapeFromMod("brewing_expansion", "be_fruit_pulp_concentrator");
			item.SetShape(new GridShapeBuilder(3, 3).SetDataFill(3, 3, (byte)1).Build());
			GameSlotInventory val = new GameSlotInventory(1, 2, false, false)
			{
				identifier = "BE_CONCENTRATOR_BATTERY"
			};
			GameGridInventory val2 = new GameGridInventory(8, 2)
			{
				identifier = "BE_CONCENTRATOR_FRUITS"
			};
			GameGridInventory val3 = new GameGridInventory(2, 2)
			{
				identifier = "BE_CONCENTRATOR_PULP"
			};
			((GameInventoryFunc)val2).mayInventoryAddItemFunc = (GameItem incoming, GameInventory _) => Owned(incoming) && BrewingFruits.CanConcentrate(incoming);
			((GameInventoryFunc)val3).mayInventoryAddItemFunc = (GameItem incoming, GameInventory _) => Owned(incoming) && incoming.identifier == "be_brewing_fruit_pulp";
			View view = new View
			{
				Status = new MachineryText(Text("concentrator_wait"), 6, (HorizontalAlignmentOptions)2, true)
			};
			MachineryStack val4 = MachineryStack.Vertical((PixelElement[])(object)new PixelElement[2]
			{
				(PixelElement)new MachineryButton(Text("concentrator_button"), (Action)delegate
				{
					Concentrate(item);
				}, -1, -1).SetSize(160, 24),
				(PixelElement)view.Status
			});
			item.SetContentWindow(MachineHelper.CreateMachineryWindow(true, (MachineryPanel[])(object)new MachineryPanel[4]
			{
				MachineHelper.PowerPanel(val),
				MachineHelper.Panel(Text("concentrator_input"), (PixelElement)(object)val2, "input", true),
				MachineHelper.Panel(Text("concentrator_output"), (PixelElement)(object)val3, "output", true),
				MachineHelper.Panel("", (PixelElement)(object)val4, "control", true)
			}));
			MachineHelper.SetupBatterySlot(val, item);
			AudioHelper.InitSmallMachine(item);
			((GameInventoryFunc)val2).onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(((GameInventoryFunc)val2).onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
			{
				Queue(item);
			});
			((GameInventoryFunc)val3).onSlotRemoveItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(((GameInventoryFunc)val3).onSlotRemoveItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
			{
				Queue(item);
			});
			((GameInventoryFunc)val).onSlotAddItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(((GameInventoryFunc)val).onSlotAddItemFunc, (Action<GameItem, GameInventory, SlotMarker>)delegate
			{
				Queue(item);
			});
			Views.Add(item, view);
			GameItem obj = item;
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Loaded));
			Loaded(item);
			return item;
		}

		private static void Loaded(GameItem item)
		{
			item.SetName(Text("concentrator_name"));
			string shortDescription = (item.longDescription = Text("concentrator_description"));
			item.shortDescription = shortDescription;
			item.contentWindow.SetTitle(item.name);
			ItemHitArea.Apply(item);
			Queue(item);
		}

		private static void Status(GameItem item, string key)
		{
			if (Views.TryGetValue(item, out var value))
			{
				value.Status.SetText(Text(key));
			}
		}

		internal static void Queue(GameItem item)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Expected O, but got Unknown
			if (ModEntry.Enabled && PlayerStore.Instance != null && Owned(item))
			{
				Pending[item] = PlayerStore.Instance;
				if ((Object)(object)pump == (Object)null)
				{
					GameObject val = new GameObject("BrewingExpansion.PulpPump");
					Object.DontDestroyOnLoad((Object)val);
					pump = val.AddComponent<PulpPump>();
				}
			}
		}

		internal static void Drain()
		{
			KeyValuePair<GameItem, PlayerStore>[] array = Pending.ToArray();
			Pending.Clear();
			KeyValuePair<GameItem, PlayerStore>[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				KeyValuePair<GameItem, PlayerStore> keyValuePair = array2[i];
				if (ModEntry.Enabled && keyValuePair.Value == PlayerStore.Instance && Owned(keyValuePair.Key))
				{
					Concentrate(keyValuePair.Key, automatic: true);
				}
			}
		}

		internal static void Reset()
		{
			Pending.Clear();
			if ((Object)(object)pump != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)pump).gameObject);
			}
			pump = null;
		}

		public static bool Concentrate(GameItem machine, bool automatic = false)
		{
			PlayerStore store = PlayerStore.Instance;
			if (!ModEntry.Enabled || store == null || !Owned(machine) || machine.identifier != "be_fruit_pulp_concentrator" || !PulpOperations.CanSave(store) || !PulpOperations.Busy.Add(machine))
			{
				return false;
			}
			PulpOperations.Store = store;
			GameItem pulp = null;
			GameItem val = null;
			GameItem[] fruits = null;
			bool flag = false;
			bool flag2 = false;
			int debit = 0;
			int num = 0;
			try
			{
				GameGridInventory input = Input(machine);
				GameGridInventory output = Output(machine);
				GameSlotInventory obj = Battery(machine);
				val = ((obj != null) ? obj.childItem : null);
				fruits = ((GameInventory)input).childItems.ToArray();
				if (fruits.Length != 4 || fruits.Any((GameItem f) => !Owned(f) || f.unitCount != 1 || !BrewingFruits.CanConcentrate(f) || f.identifier != fruits[0].identifier))
				{
					Status(machine, "concentrator_wait");
					return false;
				}
				if (automatic && (!Owned(val) || !PowerHelper.CanDrawPowerSource(val, 8)))
				{
					Status(machine, "concentrator_power");
					return false;
				}
				int currentUniqueId = store.currentUniqueId;
				pulp = DirectoryMaster.Item("be_brewing_fruit_pulp", true);
				flag = Live(pulp) && pulp.identifier == "be_brewing_fruit_pulp" && pulp.uniqueId > currentUniqueId && pulp.uniqueId <= store.currentUniqueId && pulp.parentInventory == null;
				if (!flag)
				{
					PulpOperations.Protect(store, "factory returned an unconfirmed output");
					return false;
				}
				SlotMarker val2 = output.TryFindOneValidInventorySlot_TopMiddle(pulp, false);
				if (val2 == null || !val2.IsValid())
				{
					Status(machine, "concentrator_full");
					return false;
				}
				val2.TryAcceptOnce(-1);
				if (!Ready())
				{
					return false;
				}
				int score = fruits.Sum(delegate(GameItem f)
				{
					int num4 = Math.Max(-2, Math.Min(2, f.GetTagReadonly("BERRY_CONDITION_INT").GetInt()));
					return (num4 >= 0) ? num4 : (num4 + 1);
				});
				FruitPulp.Configure(pulp, fruits[0].identifier, score, fruits.Sum((GameItem f) => f.unitValue));
				if (automatic)
				{
					GameSlotInventory obj2 = Battery(machine);
					if (((obj2 != null) ? obj2.childItem : null) != val || !Ready())
					{
						return false;
					}
					int num2 = val.GetTagReadonly("power_source_item_energy").GetInt();
					bool flag3 = false;
					try
					{
						flag3 = PowerHelper.DrawPowerSource(val, 8);
					}
					finally
					{
						if (Current() && Live(val))
						{
							debit = Math.Min(8, Math.Max(0, num2 - val.GetTagReadonly("power_source_item_energy").GetInt()));
						}
					}
					if (!flag3 || !Ready())
					{
						return false;
					}
				}
				if (!Ready())
				{
					return false;
				}
				GameItem[] array = fruits;
				foreach (GameItem val3 in array)
				{
					try
					{
						val3.Destroy();
					}
					finally
					{
						if (!Live(val3) && val3.parentInventory == null)
						{
							num++;
						}
					}
					if (!Current() || Live(val3) || val3.parentInventory != null)
					{
						return false;
					}
				}
				pulp.ModifyTag("BE_PULP_REMAINING_USES_V1", (Action<TagState>)delegate(TagState t)
				{
					t.Enable().SetInt(4);
				}, false);
				flag2 = FruitPulp.Remaining(pulp) == 4;
				if (flag2)
				{
					Status(machine, "concentrator_done");
					MachineConnections.Request();
				}
				return flag2;
				bool Ready()
				{
					if (Current() && Owned(machine) && Live(pulp) && (object)pulp.parentInventory == output)
					{
						return fruits.All((GameItem f) => Owned(f) && f.unitCount == 1 && (object)f.parentInventory == input);
					}
					return false;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Concentration interrupted: " + ex));
				return false;
			}
			finally
			{
				try
				{
					if (!flag2 && Current())
					{
						if (num > 0)
						{
							PulpOperations.Protect(store, "partial fruit consumption; keeping pending pulp and surviving ingredients");
						}
						else
						{
							if (flag && Live(pulp))
							{
								if (GraphUtils.FindAllChildrenType<GameItem>((GraphNodeStorage)(object)pulp, (Func<GameItem, bool>)((GameItem _) => true), (Func<GameItem, bool>)null).Count == 0)
								{
									pulp.Destroy();
								}
								else
								{
									PulpOperations.Protect(store, "unpaid pulp received foreign contents; preserving them");
								}
							}
							if (flag && (Live(pulp) || pulp.parentInventory != null))
							{
								PulpOperations.Protect(store, "unpaid pulp could not be removed");
							}
							if (debit > 0 && Live(val))
							{
								val.ModifyTag("power_source_item_energy", (Action<TagState>)delegate(TagState t)
								{
									t.Enable().SetInt(t.GetInt() + debit);
								}, false);
								PowerHelper.UpdateEnergyCreditSprite(val);
							}
						}
					}
				}
				catch (Exception ex2)
				{
					if (Current())
					{
						PulpOperations.Protect(store, "rollback failed: " + ex2.Message);
					}
				}
				finally
				{
					PulpOperations.Busy.Remove(machine);
					if (PulpOperations.Busy.Count == 0)
					{
						PulpOperations.Store = null;
					}
				}
			}
			bool Current()
			{
				return store == PlayerStore.Instance;
			}
		}
	}
	internal sealed class PulpPump : MonoBehaviour
	{
		private void LateUpdate()
		{
			PulpConcentrator.Drain();
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "SaveGame")]
	internal static class PulpSaveBoundary
	{
		private static bool Prefix(PlayerStore __instance)
		{
			return PulpOperations.CanSave(__instance);
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "WriteSlotFile")]
	internal static class PulpSlotBoundary
	{
		private static bool Prefix(PlayerStore __instance)
		{
			return PulpOperations.CanSave(__instance);
		}
	}
	[HarmonyPatch(typeof(PowerHelper), "UpdateEnergyCreditSprite")]
	internal static class ConcentratorPowerChanged
	{
		private static void Postfix(GameItem item)
		{
			object obj;
			if (item == null)
			{
				obj = null;
			}
			else
			{
				GameInventory parentInventory = item.parentInventory;
				obj = ((parentInventory != null) ? parentInventory.GetParentItem() : null);
			}
			GameItem val = (GameItem)obj;
			if (((val != null) ? val.identifier : null) == "be_fruit_pulp_concentrator")
			{
				PulpConcentrator.Queue(val);
			}
		}
	}
	internal static class RecipeReadAccess
	{
		internal static void Bind(GameItem item)
		{
			Func<GameItem, GameInventory, SlotMarker, GridShape, TagSystem, RichTextBuilder> previous = ((GameItemFunc)item).tooltipRichTextFunc;
			((GameItemFunc)item).tooltipRichTextFunc = delegate(GameItem candidate, GameInventory inventory, SlotMarker slot, GridShape shape, TagSystem state)
			{
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a7: Expected O, but got Unknown
				if (ModEntry.Enabled && !GeneralHelper.IsItemOwned(candidate))
				{
					PlayerStore instance = PlayerStore.Instance;
					if (instance == null || !instance.FindAllItem(true).Any((GameItem i) => i.identifier == candidate.identifier && GeneralHelper.IsItemOwned(i)))
					{
						RichTextBuilder val = new RichTextBuilder(100);
						val.AddLine(candidate.GetDisplayName(false), true, (ColorPalette)(-1), false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
						val.AddLine(ModText.Get("be_recipe_purchase_required"), true, (ColorPalette)(-1), false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
						return val;
					}
				}
				return previous?.Invoke(candidate, inventory, slot, shape, state);
			};
		}
	}
	internal static class RemainingConsumables
	{
		private static readonly HashSet<GameItem> Busy = new HashSet<GameItem>();

		private static readonly Random Random = new Random();

		internal static StoreClient Client
		{
			get
			{
				PlayerStore instance = PlayerStore.Instance;
				if (instance == null)
				{
					return null;
				}
				StoreClientInstance currentClientInstance = instance.currentClientInstance;
				if (currentClientInstance == null)
				{
					return null;
				}
				return currentClientInstance.GetClientBlueprint();
			}
		}

		internal static void Bind(GameItem item)
		{
			((GameItemFunc)item).mayActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker __) => CanUse(s);
			((GameItemFunc)item).canActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker __) => CanUse(s);
			((GameItemFunc)item).onActivateSlotItemFunc = delegate(GameItem s, GameInventory _, SlotMarker __)
			{
				Use(s);
			};
		}

		internal static bool CanUse(GameItem item)
		{
			RemainingItems.Definition definition = RemainingItems.Find((item != null) ? item.identifier : null);
			if (!ModEntry.Enabled || definition == null || Busy.Contains(item) || !MachineConnections.Owned(item))
			{
				return false;
			}
			if (!(definition.Kind != "Perfume"))
			{
				if (UseCountHelper.CanUse(item, 1))
				{
					if (!(definition.Id != "be_trade_perfume"))
					{
						if (Client != null)
						{
							return Client.perfumeInfluence == 0;
						}
						return false;
					}
					return true;
				}
				return false;
			}
			return true;
		}

		internal static bool Use(GameItem item)
		{
			if (!CanUse(item) || !Busy.Add(item))
			{
				return false;
			}
			PlayerStore instance = PlayerStore.Instance;
			try
			{
				RemainingItems.Definition definition = RemainingItems.Find(item.identifier);
				if (definition.Kind == "Box")
				{
					return Open(item, instance);
				}
				if (definition.Kind == "Card")
				{
					ModHelper.SetModDataBool("brewing_expansion", "nini_contact", true);
					FormalVisitors.RegisterPhone();
					return true;
				}
				if (definition.Kind != "Perfume")
				{
					return false;
				}
				StoreClient client = Client;
				int num = item.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt();
				UseCountHelper.Use(item, 1);
				if (instance != PlayerStore.Instance || item.GetTagReadonly("CURRENT_USE_COUNT_INT").GetInt() != num - 1)
				{
					return false;
				}
				if (item.identifier == "be_trade_perfume")
				{
					if (Client != client || client == null)
					{
						return false;
					}
					client.perfumeInfluence = 3;
					instance.RefreshCounterItem();
				}
				else
				{
					ReduceCrime(instance, (item.identifier == "be_freedom_perfume") ? 2000 : 20000);
				}
				AudioManager instance2 = AudioManager.Instance;
				if (instance2 != null)
				{
					instance2.Play2("perfume_spray");
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Item use interrupted: " + ex));
				return false;
			}
			finally
			{
				Busy.Remove(item);
			}
		}

		private static void ReduceCrime(PlayerStore store, int amount)
		{
			SecData secData = store.secData;
			if (secData == null)
			{
				return;
			}
			int num = amount;
			for (int i = 0; i < secData.crimeAmount.Count; i++)
			{
				if (num <= 0)
				{
					break;
				}
				int num2 = Math.Min(num, Math.Max(0, secData.crimeAmount[i]));
				secData.crimeAmount[i] -= num2;
				num -= num2;
			}
			num = amount;
			for (int j = 0; j < secData.crimeAmountTotal.Count; j++)
			{
				if (num <= 0)
				{
					break;
				}
				int num3 = Math.Min(num, Math.Max(0, secData.crimeAmountTotal[j]));
				secData.crimeAmountTotal[j] -= num3;
				num -= num3;
			}
		}

		internal static string[] Rewards(string id)
		{
			return id switch
			{
				"be_low_blindbox" => Seed(0, 9), 
				"be_mid_blindbox" => Seed(9, 3), 
				"be_premium_blindbox" => Seed(12, 2), 
				"be_afterglow_blindbox" => Seed(14, 2), 
				"be_wine_blindbox" => new string[1] { RemainingItems.All.Where((RemainingItems.Definition x) => x.Kind == "Collector").ElementAt(Random.Next(10)).Id }, 
				"be_figurine_blindbox" => new string[1] { Figurines.All[Random.Next(Figurines.All.Length)].Id }, 
				"be_container_blindbox" => new string[1] { Pick(new string[7] { "be_laolishi_bag", "be_cyber_bag", "be_sakura_bag", "be_cat_yoyou_satchel", "be_xuxu_warm_cup", "be_xuxu_sweet_cup", "be_47_love_cup" }, new int[7] { 25, 15, 5, 5, 30, 15, 5 }) }, 
				"be_midautumn_blindbox" => new string[1] { Pick(new string[7] { "be_midautumn_moon_fan", "be_midautumn_moon_lamp", "be_midautumn_jade_rabbit", "be_midautumn_lantern", "be_midautumn_moon_osmanthus", "be_midautumn_mooncake_box", "be_midautumn_bag" }, new int[7] { 10, 10, 10, 10, 25, 30, 5 }) }, 
				"be_midautumn_mooncake_box" => new string[1] { "be_wuren_mooncake" }, 
				"be_xuxu_cassette_bundle" => FormalMusic.Ids, 
				_ => throw new InvalidOperationException("Unknown blindbox: " + id), 
			};
			static string[] Seed(int first, int count)
			{
				return (from _ in Enumerable.Range(0, 2)
					select BrewingSeeds.All[first + Random.Next(count)].Id).ToArray();
			}
		}

		private static string Pick(string[] ids, int[] weights)
		{
			int num = Random.Next(weights.Sum());
			for (int i = 0; i < ids.Length; i++)
			{
				num -= weights[i];
				if (num < 0)
				{
					return ids[i];
				}
			}
			throw new InvalidOperationException("Invalid reward weights");
		}

		internal static bool Open(GameItem box, PlayerStore store)
		{
			if (store == null || ((box != null) ? box.parentInventory : null) == null || !MachineConnections.Owned(box))
			{
				return false;
			}
			string[] array = Rewards(box.identifier);
			List<GameItem> list = new List<GameItem>();
			bool flag = false;
			try
			{
				List<GameGridInventory> list2 = new List<GameGridInventory>();
				GameInventory parentInventory = box.parentInventory;
				GameGridInventory val = (GameGridInventory)(object)((parentInventory is GameGridInventory) ? parentInventory : null);
				if (val != null)
				{
					list2.Add(val);
				}
				EmporiumEntry instance = EmporiumEntry.Instance;
				GameGridInventory val2 = ((instance != null) ? instance.invElement : null);
				if (!ModHelper.IsOutsideStore() && val2 != null && !list2.Contains(val2))
				{
					list2.Add(val2);
				}
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (store != PlayerStore.Instance)
					{
						return false;
					}
					GameItem val3 = DirectoryMaster.Item(text, true);
					if (val3 == null)
					{
						return false;
					}
					list.Add(val3);
					bool flag2 = false;
					foreach (GameGridInventory item in list2)
					{
						SlotMarker val4 = RowSlot(item, val3);
						if (val4 != null)
						{
							val4.TryAcceptOnce(-1);
							if ((object)val3.parentInventory == item)
							{
								flag2 = true;
								break;
							}
							if (!MachineConnections.Live(val3))
							{
								return false;
							}
						}
					}
					if (!flag2)
					{
						return false;
					}
				}
				if (store != PlayerStore.Instance || !MachineConnections.Owned(box) || list.Any((GameItem a) => a.parentInventory == null || !MachineConnections.Live(a)))
				{
					return false;
				}
				int unitCount = box.unitCount;
				if (unitCount > 1)
				{
					box.unitCount = unitCount - 1;
					flag = box.unitCount == unitCount - 1;
				}
				else
				{
					box.Destroy();
					flag = !MachineConnections.Live(box);
				}
				return flag;
			}
			finally
			{
				if (!flag && store == PlayerStore.Instance)
				{
					foreach (GameItem item2 in list)
					{
						if (!MachineConnections.Live(item2))
						{
							continue;
						}
						try
						{
							item2.Destroy();
							if (MachineConnections.Live(item2))
							{
								PulpOperations.Protect(store, "Blindbox award rollback was cancelled");
							}
						}
						catch (Exception ex)
						{
							PulpOperations.Protect(store, "Blindbox award rollback failed: " + ex.Message);
						}
					}
				}
			}
		}

		private static SlotMarker RowSlot(GameGridInventory grid, GameItem item)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			GridShape modifiedShape = item.modifiedShape;
			for (int i = 0; i <= grid.inventoryShape.height - modifiedShape.globalHeight; i++)
			{
				for (int j = 0; j <= grid.inventoryShape.width - modifiedShape.globalWidth; j++)
				{
					SlotMarker val = ((GameInventory)grid).TryInventorySlot(item, 1, new GridShapeBuilder(modifiedShape).SetPosition(j, i).Build(), item.modifiedState);
					if (val != null && val.IsValid())
					{
						return val;
					}
				}
			}
			return null;
		}

		internal static void TradeFeature(GameItem item, bool clientArea)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Invalid comparison between Unknown and I4
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Invalid comparison between Unknown and I4
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Invalid comparison between Unknown and I4
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Invalid comparison between Unknown and I4
			if (!ModEntry.Enabled || item == null)
			{
				return;
			}
			FormalTraits.Apply(item, clientArea);
			item.RemoveItemFeatureByID("be_trade_perfume_selling");
			item.RemoveItemFeatureByID("be_trade_perfume_buying");
			StoreClient client = Client;
			if (client != null && client.perfumeInfluence == 3)
			{
				StoreClient client2 = Client;
				bool flag = GeneralHelper.IsItemOwned(item);
				bool flag2 = !clientArea && flag && ((int)client2.clientIntent == 1 || (int)client2.clientIntent == 3);
				bool flag3 = !flag && ((int)client2.clientIntent == 2 || (int)client2.clientIntent == 3);
				if (flag2 || flag3)
				{
					string text = (flag2 ? "be_trade_perfume_selling" : "be_trade_perfume_buying");
					item.RemoveItemFeatureByID(text);
					ItemFeature val = (flag2 ? ItemFeatureList.NeuroActivePerfumeSelling() : ItemFeatureList.NeuroActivePerfumeBuying());
					val.identifier = text;
					val.SetValueModifier(flag2 ? 20 : (-20)).SetPreExposedValueMod(flag2 ? 20 : (-20), true);
					val.SetPublicDisplay(RemainingItems.Text("be_trade_perfume_name")).SetActualDisplay(RemainingItems.Text("be_trade_perfume_name"));
					item.AddItemFeature(val);
				}
			}
		}
	}
	[HarmonyPatch(typeof(GameItem), "OnAddedToWeightedArea")]
	internal static class RemainingPerfumeWeighted
	{
		private static void Postfix(GameItem __instance)
		{
			RemainingConsumables.TradeFeature(__instance, clientArea: false);
		}
	}
	[HarmonyPatch(typeof(GameItem), "OnAddedToClientArea")]
	internal static class RemainingPerfumeClientArea
	{
		private static void Postfix(GameItem __instance)
		{
			RemainingConsumables.TradeFeature(__instance, clientArea: true);
		}
	}
	internal static class RemainingItems
	{
		internal sealed class Definition
		{
			internal readonly string Id;

			internal readonly string Sprite;

			internal readonly string Kind;

			internal readonly string Template;

			internal readonly int Price;

			internal readonly int Width;

			internal readonly int Height;

			internal readonly int A;

			internal readonly int B;

			internal Definition(string id, string sprite, string kind, string template, int price, int width, int height, int a, int b)
			{
				Id = id;
				Sprite = sprite;
				Kind = kind;
				Template = template;
				Price = price;
				Width = width;
				Height = height;
				A = a;
				B = b;
			}
		}

		internal static readonly Definition[] All = new Definition[65]
		{
			new Definition("be_xuxu_basic_battery", "be_xuxu_basic_battery", "Battery", "energy_credit", 800, 1, 2, 200, 6),
			new Definition("be_xuxu_mid_battery", "be_xuxu_mid_battery", "Battery", "energy_credit", 1200, 1, 2, 350, 8),
			new Definition("be_xuxu_high_battery", "be_xuxu_high_battery", "Battery", "energy_credit", 2000, 1, 2, 500, 12),
			new Definition("be_qingyun_filter", "be_qingyun_filter", "Filter", "water_filter", 800, 1, 3, 600, 60),
			new Definition("be_ninglu_filter", "be_ninglu_filter", "Filter", "water_filter", 1200, 1, 3, 800, 80),
			new Definition("be_yingfei_filter", "be_yingfei_filter", "Filter", "water_filter", 2000, 1, 3, 1000, 100),
			new Definition("be_clear_water_tank", "be_clear_water_tank", "Water", "water_pitcher", 200, 2, 3, 30000, 0),
			new Definition("be_xuxu_warm_cup", "be_xuxu_warm_cup", "Water", "water_pitcher", 200, 2, 3, 17000, 0),
			new Definition("be_xuxu_sweet_cup", "be_xuxu_sweet_cup", "Water", "water_pitcher", 400, 2, 3, 30000, 0),
			new Definition("be_47_love_cup", "be_47_love_cup", "Water", "water_pitcher", 1400, 2, 3, 47000, 0),
			new Definition("be_compact_smuggler_bay", "be_compact_smuggler_bay", "Hidden", "smuggler_bay_mod", 1200, 2, 2, 12, 12),
			new Definition("be_multi_storage", "be_multi_storage", "Hidden", "smuggler_bay_mod", 2500, 2, 4, 15, 20),
			new Definition("be_midautumn_bag", "be_midautumn_bag", "Bag", "satchel", 0, 3, 3, 20, 15),
			new Definition("be_sakura_bag", "be_sakura_bag", "Bag", "satchel", 2525, 3, 3, 12, 10),
			new Definition("be_cyber_bag", "be_cyber_bag", "Bag", "satchel", 800, 3, 2, 8, 8),
			new Definition("be_laolishi_bag", "be_laolishi_bag", "Bag", "satchel", 500, 3, 2, 6, 6),
			new Definition("be_cat_yoyou_satchel", "be_cat_yoyou_satchel", "Bag", "satchel", 1107, 3, 3, 8, 8),
			new Definition("be_golden_yeast_reactor", "be_golden_yeast_reactor", "Yeast", "wine_yeast_infinite", 1000, 2, 2, 4, 0),
			new Definition("be_advanced_yeast_reactor", "be_advanced_yeast_reactor", "Yeast", "wine_yeast_infinite", 8000, 2, 2, 5, 0),
			new Definition("be_forbidden_yeast_reactor", "be_forbidden_yeast_reactor", "Yeast", "wine_yeast_red", 50000, 2, 2, 5, 0),
			new Definition("be_wine_yeast_hentai", "be_hentai_yeast", "SpecialYeast", "wine_yeast_red", 233, 1, 2, 0, 0),
			new Definition("be_brewery_sign", "be_brewery_sign", "Sign", "", 100, 2, 2, 0, 0),
			new Definition("be_brewing_manual", "be_brewing_manual", "Manual", "storage_bay", 10, 2, 3, 0, 0),
			new Definition("be_midautumn_moon_fan", "be_midautumn_moon_fan", "Decor", "", 0, 2, 3, 0, 0),
			new Definition("be_midautumn_moon_lamp", "be_midautumn_moon_lamp", "Decor", "", 0, 2, 2, 0, 0),
			new Definition("be_midautumn_jade_rabbit", "be_midautumn_jade_rabbit", "Decor", "", 0, 2, 2, 0, 0),
			new Definition("be_midautumn_lantern", "be_midautumn_lantern", "Decor", "", 0, 2, 2, 0, 0),
			new Definition("be_neon_dream_wine", "be_neon_dream_wine", "Collector", "", 999, 1, 4, 0, 0),
			new Definition("be_shuangqing_wine", "be_shuangqing_wine", "Collector", "", 666, 1, 4, 0, 0),
			new Definition("be_nanfeng_wine", "be_nanfeng_wine", "Collector", "", 666, 1, 4, 0, 0),
			new Definition("be_feimei_wine", "be_feimei_wine", "Collector", "", 666, 1, 4, 0, 0),
			new Definition("be_neon_illusion_wine", "be_neon_illusion_wine", "Collector", "", 666, 1, 4, 0, 0),
			new Definition("be_ideal_end_wine", "be_ideal_end_wine", "Collector", "", 666, 1, 4, 0, 0),
			new Definition("be_luofu_dream_wine", "be_luofu_dream_wine", "Collector", "", 666, 1, 4, 0, 0),
			new Definition("be_magic_pig_wine", "be_magic_pig_wine", "Collector", "", 666, 1, 4, 0, 0),
			new Definition("be_sea_blue_wine", "be_sea_blue_wine", "Collector", "", 666, 1, 4, 0, 0),
			new Definition("be_rainbow_recipe_wine", "be_rainbow_recipe_wine", "Collector", "", 666, 1, 4, 0, 0),
			new Definition("be_low_blindbox", "be_low_blindbox", "Box", "", 100, 2, 2, 0, 0),
			new Definition("be_mid_blindbox", "be_mid_blindbox", "Box", "", 300, 2, 2, 0, 0),
			new Definition("be_premium_blindbox", "be_premium_blindbox", "Box", "", 3000, 2, 2, 0, 0),
			new Definition("be_afterglow_blindbox", "be_afterglow_blindbox", "Box", "", 8000, 2, 2, 0, 0),
			new Definition("be_wine_blindbox", "be_wine_blindbox", "Box", "", 2333, 2, 2, 0, 0),
			new Definition("be_container_blindbox", "be_container_blindbox", "Box", "", 1000, 2, 2, 0, 0),
			new Definition("be_figurine_blindbox", "be_figurine_blindbox", "Box", "", 5000, 2, 2, 0, 0),
			new Definition("be_xuxu_cassette_bundle", "be_xuxu_cassette_bundle", "Box", "", 700, 2, 2, 0, 0),
			new Definition("be_midautumn_blindbox", "be_midautumn_blindbox", "Box", "", 4925, 2, 2, 0, 0),
			new Definition("be_midautumn_mooncake_box", "be_midautumn_mooncake_box", "Box", "", 0, 2, 2, 0, 0),
			new Definition("be_contraband_perfume", "be_contraband_perfume", "Perfume", "", 50000, 2, 2, 0, 0),
			new Definition("be_nini_card", "be_nini_card", "Card", "", 0, 2, 1, 0, 0),
			new Definition("be_wuren_mooncake", "be_wuren_mooncake", "Food", "", 815, 3, 3, 0, 0),
			new Definition("be_freedom_perfume", "be_freedom_perfume", "Perfume", "", 5000, 2, 2, 0, 0),
			new Definition("be_recipe_1", "be_recipe_1", "Recipe", "", 50, 2, 2, 0, 0),
			new Definition("be_recipe_2", "be_recipe_2", "Recipe", "", 50, 2, 2, 1, 0),
			new Definition("be_recipe_3", "be_recipe_3", "Recipe", "", 50, 2, 2, 2, 0),
			new Definition("be_recipe_4", "be_recipe_4", "Recipe", "", 50, 2, 2, 3, 0),
			new Definition("be_recipe_5", "be_recipe_5", "Recipe", "", 50, 2, 2, 4, 0),
			new Definition("be_recipe_6", "be_recipe_6", "Recipe", "", 50, 2, 2, 5, 0),
			new Definition("be_recipe_7", "be_recipe_7", "Recipe", "", 50, 2, 2, 6, 0),
			new Definition("be_recipe_8", "be_recipe_8", "Recipe", "", 50, 2, 2, 7, 0),
			new Definition("be_xuxu_mini_storage", "be_mini_storage_1", "Storage", "storage_bay", 600, 2, 2, 12, 20),
			new Definition("be_xuxu_badge", "be_xuxu_badge", "Badge", "", 1, 2, 2, 0, 0),
			new Definition("be_trade_perfume", "be_trade_perfume", "Perfume", "", 3000, 2, 2, 5, 0),
			new Definition("be_xuxu_water_purifier_basic", "be_xuxu_water_purifier_basic", "Purifier", "portable_water_purifier", 600, 2, 2, 500, 20),
			new Definition("be_xuxu_water_purifier_middle", "be_xuxu_water_purifier_middle", "Purifier", "portable_water_purifier", 900, 2, 2, 600, 30),
			new Definition("be_xuxu_water_purifier_advanced", "be_xuxu_water_purifier_advanced", "Purifier", "portable_water_purifier", 2000, 2, 2, 1000, 100)
		};

		internal static Definition Find(string id)
		{
			return Array.Find(All, (Definition d) => d.Id == id);
		}

		internal static string Text(string key, params object[] args)
		{
			return ModHelper.GetLocalized("brewing_expansion", key, args);
		}

		internal static void Tag(GameItem item, string key, int value)
		{
			item.ModifyTag(key, (Action<TagState>)delegate(TagState t)
			{
				t.Enable().SetInt(value);
			}, false);
		}

		internal static GameGridInventory Grid(GameItem item)
		{
			return GraphUtils.FindAllChildrenType<GameGridInventory>((GraphNodeStorage)(object)item, (Func<GameGridInventory, bool>)null, (Func<GameGridInventory, bool>)null).FirstOrDefault();
		}

		internal static bool IsCollector(GameItem item)
		{
			return Find((item != null) ? item.identifier : null)?.Kind == "Collector";
		}

		internal static GameItem Create(Definition d)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			GameItem val = (string.IsNullOrEmpty(d.Template) ? ItemDirectory.CreateEmptyItem(d.Id) : DirectoryMaster.Item(d.Template, false));
			if (val == null)
			{
				throw new InvalidOperationException("Missing native template: " + d.Template);
			}
			val.identifier = d.Id;
			val.SetValue((long)d.Price);
			val.SetShape(new GridShapeBuilder(d.Width, d.Height).SetDataFill(d.Width, d.Height, (byte)1).Build());
			switch (d.Kind)
			{
			case "Battery":
				Tag(val, "power_source_item_max_energy", d.A);
				Tag(val, "power_source_item_energy", d.A);
				Tag(val, "power_source_item_bandwidth", 20);
				val.DisableTag("POWER_SOURCE_ITEM_DISPOSABLE_TAG", false);
				PowerHelper.InitRecharge(val, d.B);
				((GameItemFunc)val).onCycleEndEarlySlotItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(((GameItemFunc)val).onCycleEndEarlySlotItemFunc, new Action<GameItem, GameInventory, SlotMarker>(RechargeBattery));
				val.SetValue((long)Math.Max(0, d.Price - d.A));
				val.unitBaseValue = 0L;
				val.lateUnitValue = 0L;
				break;
			case "Filter":
				val.SetGameItemType("LUXURY_ITEM");
				Tag(val, "DURABILITY_PRICING_MODE_TAG", 1);
				Tag(val, "WATER_FILTER", d.A);
				DurabilityHelper.SetDurability(val, d.A, d.A, d.A);
				((GameItemFunc)val).onCycleEndSlotItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(((GameItemFunc)val).onCycleEndSlotItemFunc, new Action<GameItem, GameInventory, SlotMarker>(Recover));
				break;
			case "Purifier":
				DurabilityHelper.SetDurability(val, d.A, d.A, d.A);
				((GameItemFunc)val).onCycleEndSlotItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(((GameItemFunc)val).onCycleEndSlotItemFunc, new Action<GameItem, GameInventory, SlotMarker>(Recover));
				break;
			case "Water":
				WaterHelper.EmptyContainer(val);
				Tag(val, "LIQUID_CONTAINER_CAPACITY", d.A * 1000);
				Tag(val, "LIQUID_CONTAINER_BASE_PRICE_TAG", d.Price);
				break;
			case "Storage":
			case "Hidden":
			case "Bag":
			{
				GameGridInventory val2 = Grid(val) ?? throw new InvalidOperationException("Container has no native grid: " + d.Id);
				val2.SetShape(d.A, d.B);
				if (d.Kind == "Hidden")
				{
					val.EnableTag("ITEM_HIDDEN_TAG", false);
					((GameInventory)val2).identifier = "smuggler_bay";
				}
				if (d.Id == "be_multi_storage" || d.Id == "be_xuxu_mini_storage")
				{
					string[] array2 = ((d.Id == "be_multi_storage") ? new string[1] { d.Sprite }.Concat(from n in Enumerable.Range(1, 17)
						select "be_multi_storage_" + n.ToString("00")).ToArray() : (from n in Enumerable.Range(1, 4)
						select "be_mini_storage_" + n).ToArray());
					ToggleAppearanceHelper.MakeSpriteCyclable(val, "brewing_expansion", array2);
					((GameItemFunc)val).onToggleSlotItemFunc = SwitchAppearance;
				}
				break;
			}
			case "Manual":
				foreach (GameGridInventory item in GraphUtils.FindAllChildrenType<GameGridInventory>((GraphNodeStorage)(object)val, (Func<GameGridInventory, bool>)null, (Func<GameGridInventory, bool>)null))
				{
					Func<GameItem, GameInventory, bool> native = ((GameInventoryFunc)item).mayInventoryAddItemFunc;
					((GameInventoryFunc)item).mayInventoryAddItemFunc = (GameItem candidate, GameInventory inventory) => (native == null || native(candidate, inventory)) && Find(candidate.identifier)?.Kind == "Recipe";
				}
				val.SetGameItemType("DOCUMENT");
				break;
			case "Yeast":
				if (d.Id == "be_forbidden_yeast_reactor")
				{
					WineHelper.InitInfiniteYeast(val, d.A);
				}
				else
				{
					Tag(val, "WINE_YEAST_POWER_INT", d.A);
				}
				if (d.Id == "be_advanced_yeast_reactor")
				{
					ContrabandHelper.InitContrabandItem(val, 4);
				}
				if (d.Id == "be_forbidden_yeast_reactor")
				{
					ContrabandHelper.InitContrabandItem(val, 3);
				}
				break;
			case "SpecialYeast":
				val.EnableTag("BE_HENTAI_YEAST", false);
				val.DisableTag("WINE_YEAST_REUSABLE_TAG", false);
				break;
			case "Collector":
			{
				string[] array = new string[3] { "ALCOHOL", "SUBSTANCE", "LUXURY_ITEM" };
				foreach (string gameItemType in array)
				{
					val.SetGameItemType(gameItemType);
				}
				AgableHelper.InitAgable(val, 3);
				break;
			}
			case "Perfume":
				val.SetGameItemType("LUXURY_ITEM");
				AudioHelper.InitGlass(val);
				UseCountHelper.InitUseCountItem(val, 5, true, true, true, 0, d.Price / 5);
				if (d.Id != "be_trade_perfume")
				{
					ContrabandHelper.InitContrabandItem(val, 4);
				}
				RemainingConsumables.Bind(val);
				break;
			case "Box":
				val.SetGameItemType("LUXURY_ITEM");
				RemainingConsumables.Bind(val);
				break;
			case "Card":
				val.SetGameItemType("DOCUMENT");
				RemainingConsumables.Bind(val);
				break;
			case "Decor":
				val.SetGameItemType("LUXURY_ITEM");
				break;
			case "Food":
				val.SetGameItemType("FOOD");
				break;
			case "Recipe":
				val.SetGameItemType("DOCUMENT");
				RecipeReadAccess.Bind(val);
				break;
			case "Sign":
				val.EnableTag("BE_BREWERY_SIGN", false);
				break;
			}
			((GameItemFunc)val).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)val).onLoaded, new Action<GameItem>(Loaded));
			Loaded(val);
			return val;
		}

		internal static void Loaded(GameItem item)
		{
			Definition definition = Find((item != null) ? item.identifier : null);
			if (definition == null)
			{
				return;
			}
			if (definition.Kind == "Collector")
			{
				WineChemistry.EnsureLoaded(item);
			}
			if (definition.Kind == "Manual")
			{
				item.DisableTag("TOGGLE_APPEARANCE_TAG", false);
				((GameItemFunc)item).mayToggleSlotItemFunc = WithoutTemplateSkins(((GameItemFunc)item).mayToggleSlotItemFunc);
				((GameItemFunc)item).canToggleSlotItemFunc = WithoutTemplateSkins(((GameItemFunc)item).canToggleSlotItemFunc);
				((GameItemFunc)item).onToggleSlotItemFunc = WithoutTemplateSkins(((GameItemFunc)item).onToggleSlotItemFunc);
			}
			item.SetName(Text(definition.Id + "_name"));
			string shortDescription = (item.longDescription = Text(definition.Id + "_description"));
			item.shortDescription = shortDescription;
			if (item.IsTag("BE_XUXU_BASIC_EMPTY_VARIANT"))
			{
				shortDescription = (item.longDescription = ModText.Get("be_basic_battery_empty_description"));
				item.shortDescription = shortDescription;
			}
			if (definition.Kind == "Battery")
			{
				RefreshBattery(item);
			}
			else
			{
				int num = ((definition.Id == "be_multi_storage" || definition.Id == "be_xuxu_mini_storage") ? item.GetTagReadonly("BE_APPEARANCE").GetInt() : 0);
				string text3 = ((definition.Id == "be_multi_storage" && num > 0) ? ("be_multi_storage_" + num.ToString("00")) : ((definition.Id == "be_xuxu_mini_storage") ? ("be_mini_storage_" + (num % 4 + 1)) : definition.Sprite));
				item.useSpriteFromMod = true;
				item.SetSprite("brewing_expansion", text3);
				if (definition.Kind == "Water")
				{
					((GameItemFunc)item).getItemSpriteModifierForItemFunc = null;
				}
				ItemHitArea.Apply(item);
			}
			if (item.contentWindow != null)
			{
				item.contentWindow.SetTitle(item.name);
			}
		}

		private static T WithoutTemplateSkins<T>(T handlers) where T : Delegate
		{
			if (handlers == null)
			{
				return null;
			}
			Delegate[] invocationList = handlers.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				MethodInfo method = obj.Method;
				Type declaringType = method.DeclaringType;
				if (declaringType?.Assembly == typeof(ToggleAppearanceHelper).Assembly && declaringType.FullName.StartsWith(typeof(ToggleAppearanceHelper).FullName + "+", StringComparison.Ordinal) && method.Name.StartsWith("<MakeSpriteCyclable>", StringComparison.Ordinal))
				{
					handlers = (T)Delegate.Remove(handlers, obj);
				}
			}
			return handlers;
		}

		internal static void RefreshBattery(GameItem item)
		{
			Definition definition = Find((item != null) ? item.identifier : null);
			if (definition?.Kind != "Battery")
			{
				return;
			}
			int num = item.GetTagReadonly("power_source_item_energy").GetInt();
			double num2 = Math.Max(0.0, Math.Min(1.0, (double)num / (double)Math.Max(1, item.GetTagReadonly("power_source_item_max_energy").GetInt())));
			string text = ((num2 >= 0.99) ? definition.Sprite : ((num2 >= 0.8) ? (definition.Sprite + "_4") : ((num2 >= 0.6) ? (definition.Sprite + "_3") : ((num2 >= 0.4) ? (definition.Sprite + "_2") : ((num2 >= 0.2) ? (definition.Sprite + "_1") : (definition.Sprite + "_0"))))));
			((GameItemFunc)item).getItemSpriteModifierForItemFunc = null;
			item.useSpriteFromMod = true;
			item.SetSprite("brewing_expansion", text);
			GameItemElement val = (GameItemElement)(object)((item is GameItemElement) ? item : null);
			object obj;
			if (val == null)
			{
				obj = null;
			}
			else
			{
				TreeNodeRender background = val.background;
				obj = ((background != null) ? background.image : null);
			}
			if ((Object)obj != (Object)null)
			{
				TreeNodeRender fakeBackground = val.fakeBackground;
				if ((Object)(object)((fakeBackground != null) ? fakeBackground.image : null) != (Object)null)
				{
					item.SetSpriteQuick("brewing_expansion", text);
				}
			}
			ItemHitArea.Apply(item);
		}

		private static void RechargeBattery(GameItem item, GameInventory inventory, SlotMarker slot)
		{
			if (!ModEntry.Enabled || Find((item != null) ? item.identifier : null)?.Kind != "Battery" || !MachineConnections.Owned(item))
			{
				return;
			}
			try
			{
				PowerHelper.ChargeBattery(item);
				RefreshBattery(item);
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Battery overnight recharge interrupted: " + ex.Message));
			}
		}

		private static void Recover(GameItem item, GameInventory inventory, SlotMarker slot)
		{
			Definition definition = Find((item != null) ? item.identifier : null);
			PlayerStore instance = PlayerStore.Instance;
			if (!ModEntry.Enabled || definition == null || instance == null || !MachineConnections.Owned(item))
			{
				return;
			}
			int dayCounter = StoreStation.Instance.dayCounter;
			TagState tagReadonly = item.GetTagReadonly("BE_RECOVERY_DAY");
			if (!item.IsTag("BE_RECOVERY_DAY") || tagReadonly.GetInt() != dayCounter)
			{
				Tag(item, "BE_RECOVERY_DAY", dayCounter);
				int num = DurabilityHelper.GetMaxDurability(item) - DurabilityHelper.GetRealDurability(item);
				if (num > 0)
				{
					DurabilityHelper.ChangeDurability(item, Math.Min(definition.B, num));
				}
			}
		}

		private static void SwitchAppearance(GameItem item, GameInventory inventory, SlotMarker slot)
		{
			if (ModEntry.Enabled && MachineConnections.Owned(item))
			{
				int num = ((item.identifier == "be_multi_storage") ? 18 : 4);
				Tag(item, "BE_APPEARANCE", (item.GetTagReadonly("BE_APPEARANCE").GetInt() + 1) % num);
				Loaded(item);
				item.Validate();
			}
		}
	}
	[HarmonyPatch(typeof(PowerHelper), "UpdateEnergyCreditSprite")]
	internal static class RemainingBatterySprite
	{
		private static void Postfix(GameItem __0)
		{
			if (ModEntry.Enabled)
			{
				RemainingItems.RefreshBattery(__0);
			}
		}
	}
	public static class SeedIncubator
	{
		public const string Id = "be_incubator";

		public const string LastNightTag = "BE_FORMAL_INCUBATION_DAY_V1";

		public const int EnergyPerNight = 12;

		public static GameSlotInventory Battery(GameItem machine)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(machine, 0);
			return (GameSlotInventory)(object)((bodyStorage is GameSlotInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Input(GameItem machine)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(machine, 1);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Output(GameItem machine)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(machine, 2);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		private static string Text(string key)
		{
			return ModHelper.GetLocalized("brewing_expansion", key, Array.Empty<object>());
		}

		public static GameItem Create()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Expected O, but got Unknown
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Expected O, but got Unknown
			GameItem val = ItemDirectory.CreateEmptyItem("be_incubator");
			val.SetValue(300L);
			val.SetGameItemType("MACHINE");
			val.EnableTag("CONTAINER_TAG", false);
			val.SetSpriteAndShapeFromMod("brewing_expansion", "be_incubator");
			val.SetShape(new GridShapeBuilder(3, 3).SetDataFill(3, 3, (byte)1).Build());
			GameSlotInventory val2 = new GameSlotInventory(1, 2, false, false)
			{
				identifier = "BE_INCUBATOR_BATTERY"
			};
			GameGridInventory val3 = new GameGridInventory(12, 12)
			{
				identifier = "BE_INCUBATOR_INPUT"
			};
			GameGridInventory val4 = new GameGridInventory(12, 12)
			{
				identifier = "BE_INCUBATOR_OUTPUT"
			};
			((GameInventoryFunc)val3).mayInventoryAddItemFunc = (GameItem item, GameInventory _) => MachineConnections.Owned(item) && IncubatorRecipes.Find(item) != null;
			((GameInventoryFunc)val4).mayInventoryAddItemFunc = (GameItem item, GameInventory _) => MachineConnections.Owned(item) && IncubatorRecipes.AcceptsProduct(item);
			val.SetContentWindow(MachineHelper.CreateMachineryWindow(true, (MachineryPanel[])(object)new MachineryPanel[4]
			{
				MachineHelper.PowerPanel(val2),
				MachineHelper.Panel(Text("incubator_input"), (PixelElement)(object)val3, "input", true),
				MachineHelper.Panel(Text("incubator_output"), (PixelElement)(object)val4, "output", true),
				MachineHelper.NotePanel(new GameSlotInventory(1, 1, false, false)
				{
					identifier = "BE_MACHINE_NOTE"
				})
			}));
			MachineHelper.SetupBatterySlot(val2, val);
			AudioHelper.InitSmallMachine(val);
			((GameItemFunc)val).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)val).onLoaded, new Action<GameItem>(Loaded));
			Loaded(val);
			return val;
		}

		private static void Loaded(GameItem machine)
		{
			machine.SetName(Text("incubator_name"));
			string shortDescription = (machine.longDescription = Text("incubator_description"));
			machine.shortDescription = shortDescription;
			machine.contentWindow.SetTitle(machine.name);
			ItemHitArea.Apply(machine);
		}

		public static void CompleteNight()
		{
			PlayerStore instance = PlayerStore.Instance;
			if (!ModEntry.Enabled || instance == null)
			{
				return;
			}
			int quota = BrewingCertificates.Quota();
			GameItem[] array = (from x in instance.FindAllItem(true)
				where x.identifier == "be_incubator" && MachineConnections.Owned(x)
				select x).Distinct().ToArray();
			foreach (GameItem machine in array)
			{
				if (instance != PlayerStore.Instance)
				{
					break;
				}
				try
				{
					RunNight(machine, quota);
				}
				catch (Exception ex)
				{
					Debug.LogWarning((object)("[BrewingExpansion.Formal] Incubator night deferred: " + ex.Message));
				}
			}
		}

		public static int RunNight(GameItem machine)
		{
			return RunNight(machine, BrewingCertificates.Quota());
		}

		private static int RunNight(GameItem machine, int quota)
		{
			PlayerStore store = PlayerStore.Instance;
			GameItem battery;
			if (store != null && StoreStation.Instance != null && ModEntry.Enabled)
			{
				GameItem obj = machine;
				if (!(((obj != null) ? obj.identifier : null) != "be_incubator") && MachineConnections.Owned(machine) && PulpOperations.CanSave(store) && WaterBottlePrinter.CanSave(store))
				{
					int day = StoreStation.Instance.dayCounter;
					if (machine.IsTag("BE_FORMAL_INCUBATION_DAY_V1") && machine.GetTagReadonly("BE_FORMAL_INCUBATION_DAY_V1").GetInt() == day)
					{
						return 0;
					}
					GameSlotInventory obj2 = Battery(machine);
					battery = ((obj2 != null) ? obj2.childItem : null);
					if (!MachineConnections.Owned(battery) || !PowerHelper.CanDrawPowerSource(battery, 12) || !PulpOperations.Busy.Add(machine))
					{
						return 0;
					}
					PulpOperations.Store = store;
					int num = 0;
					int debit = 0;
					bool flag = false;
					try
					{
						GameItem[] array = (from x in ((GameInventory)Input(machine)).childItems
							orderby x.modifiedShape.minY, x.modifiedShape.minX, x.uniqueId
							select x).ToArray();
						foreach (GameItem val in array)
						{
							IncubatorRecipes.Recipe recipe = IncubatorRecipes.Find(val);
							if (recipe == null)
							{
								continue;
							}
							while (num < quota && Ready() && MachineConnections.Owned(val) && (object)val.parentInventory == Input(machine) && val.unitCount > 0)
							{
								List<GameItem> list = new List<GameItem>(recipe.Quantity);
								bool flag2 = false;
								bool flag3 = false;
								try
								{
									for (int num3 = 0; num3 < recipe.Quantity; num3++)
									{
										int currentUniqueId = store.currentUniqueId;
										GameItem val2 = recipe.Create();
										if (!Current())
										{
											return num;
										}
										if (!MachineConnections.Live(val2) || val2.identifier != recipe.ProductId || val2.parentInventory != null || val2.uniqueId <= currentUniqueId || val2.uniqueId > store.currentUniqueId)
										{
											PulpOperations.Protect(store, "incubator product allocation was not confirmed");
											return num;
										}
										list.Add(val2);
										SlotMarker val3 = FindRowSlot(Output(machine), val2);
										if (val3 == null)
										{
											return num;
										}
										val3.TryAcceptOnce(-1);
										if (!Ready() || (object)val2.parentInventory != Output(machine))
										{
											return num;
										}
									}
									if (!flag)
									{
										int num4 = battery.GetTagReadonly("power_source_item_energy").GetInt();
										try
										{
											flag = PowerHelper.DrawPowerSource(battery, 12);
										}
										finally
										{
											if (Current() && MachineConnections.Live(battery))
											{
												debit = Math.Min(12, Math.Max(0, num4 - battery.GetTagReadonly("power_source_item_energy").GetInt()));
											}
										}
										if (!flag || !Ready())
										{
											return num;
										}
									}
									if (!MachineConnections.Owned(val) || (object)val.parentInventory != Input(machine))
									{
										return num;
									}
									int unitCount = val.unitCount;
									IncubatorRecipes.Recipe recipe2 = IncubatorRecipes.Find(val);
									if (unitCount <= 0 || recipe2 == null || recipe2.SeedId != recipe.SeedId || recipe2.ProductId != recipe.ProductId || recipe2.Quantity != recipe.Quantity)
									{
										return num;
									}
									try
									{
										if (unitCount > 1)
										{
											val.SetUnitCount(unitCount - 1);
										}
										else
										{
											val.Destroy();
										}
									}
									catch (Exception ex)
									{
										Debug.LogWarning((object)("[BrewingExpansion.Formal] Incubator seed consumption interrupted: " + ex.Message));
									}
									if (!Current())
									{
										return num;
									}
									flag3 = ((unitCount <= 1) ? (!MachineConnections.Live(val) && val.parentInventory == null) : (MachineConnections.Live(val) && (object)val.parentInventory == Input(machine) && val.unitCount == unitCount - 1));
									if (!flag3)
									{
										if ((unitCount > 1 && val.unitCount != unitCount) || (unitCount == 1 && (object)val.parentInventory != Input(machine)))
										{
											PulpOperations.Protect(store, "incubator seed consumption changed unexpectedly");
										}
										return num;
									}
									if (!Ready() || list.Any((GameItem product) => !MachineConnections.Owned(product) || (object)product.parentInventory != Output(machine)))
									{
										PulpOperations.Protect(store, "incubator paid seed lost its matching output");
										return num;
									}
									flag2 = true;
									num++;
								}
								finally
								{
									if (!flag2 && Current())
									{
										bool flag4 = flag3;
										foreach (GameItem item in list)
										{
											try
											{
												if (MachineConnections.Live(item) && ((object)item.parentInventory == Output(machine) || item.parentInventory == null))
												{
													item.Destroy();
												}
												flag4 |= MachineConnections.Live(item) || item.parentInventory != null;
											}
											catch (Exception ex2)
											{
												flag4 = true;
												Debug.LogWarning((object)("[BrewingExpansion.Formal] Incubator product rollback interrupted: " + ex2.Message));
											}
										}
										if (flag4)
										{
											PulpOperations.Protect(store, "incubator rollback could not restore a complete recipe");
										}
									}
								}
							}
							if (num >= quota)
							{
								break;
							}
						}
					}
					catch (Exception ex3)
					{
						Debug.LogWarning((object)("[BrewingExpansion.Formal] Incubation interrupted: " + ex3.Message));
					}
					finally
					{
						try
						{
							if (Current())
							{
								if (num > 0)
								{
									machine.ModifyTag("BE_FORMAL_INCUBATION_DAY_V1", (Action<TagState>)delegate(TagState t)
									{
										t.Enable().SetInt(day);
									}, false);
									if (machine.GetTagReadonly("BE_FORMAL_INCUBATION_DAY_V1").GetInt() != day || !machine.IsTag("BE_FORMAL_INCUBATION_DAY_V1"))
									{
										PulpOperations.Protect(store, "incubator night receipt could not be saved");
									}
								}
								else if (debit > 0 && MachineConnections.Live(battery))
								{
									battery.ModifyTag("power_source_item_energy", (Action<TagState>)delegate(TagState t)
									{
										t.Enable().SetInt(t.GetInt() + debit);
									}, false);
									PowerHelper.UpdateEnergyCreditSprite(battery);
								}
							}
						}
						catch (Exception ex4)
						{
							if (Current())
							{
								PulpOperations.Protect(store, "incubator settlement failed: " + ex4.Message);
							}
						}
						finally
						{
							PulpOperations.Busy.Remove(machine);
							if (PulpOperations.Busy.Count == 0)
							{
								PulpOperations.Store = null;
							}
						}
					}
					return num;
				}
			}
			return 0;
			bool Current()
			{
				return store == PlayerStore.Instance;
			}
			bool Ready()
			{
				if (Current() && MachineConnections.Owned(machine) && MachineConnections.Owned(battery))
				{
					GameSlotInventory obj3 = Battery(machine);
					return ((obj3 != null) ? obj3.childItem : null) == battery;
				}
				return false;
			}
		}

		private static SlotMarker FindRowSlot(GameGridInventory grid, GameItem item)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			GridShape modifiedShape = item.modifiedShape;
			for (int i = 0; i <= grid.inventoryShape.height - modifiedShape.globalHeight; i++)
			{
				for (int j = 0; j <= grid.inventoryShape.width - modifiedShape.globalWidth; j++)
				{
					GridShape val = new GridShapeBuilder(modifiedShape).SetPosition(j, i).Build();
					SlotMarker val2 = ((GameInventory)grid).TryInventorySlot(item, 1, val, item.modifiedState);
					if (val2 != null && val2.IsValid())
					{
						return val2;
					}
				}
			}
			return null;
		}
	}
	internal static class StorageExpansion
	{
		internal static bool Supports(GameItem item)
		{
			if (item != null)
			{
				if (!(item.identifier == "be_multi_storage") && !(item.identifier == "be_xuxu_mini_storage"))
				{
					return item.identifier == "be_compact_smuggler_bay";
				}
				return true;
			}
			return false;
		}

		internal static MachineryTabs Tabs(GameItem item)
		{
			object obj;
			if (item == null)
			{
				obj = null;
			}
			else
			{
				PixelWindow contentWindow = item.contentWindow;
				obj = ((contentWindow != null) ? contentWindow.childElement : null);
			}
			object obj2 = ((obj is MachineryPanelRow) ? obj : null);
			object obj3;
			if (obj2 == null)
			{
				obj3 = null;
			}
			else
			{
				MachineryPanel panel = ((MachineryPanelRow)obj2).GetPanel("storage");
				obj3 = ((panel != null) ? panel.content : null);
			}
			return (MachineryTabs)((obj3 is MachineryTabs) ? obj3 : null);
		}

		internal static void RetainHiddenPages(GameItem item)
		{
			if (!Supports(item) || RemainingItems.Find(item.identifier)?.Kind != "Hidden")
			{
				return;
			}
			MachineryTabs val = Tabs(item);
			if (val == null)
			{
				return;
			}
			for (int i = 0; i < val.count; i++)
			{
				PixelElement page = val.GetPage(i);
				PixelElement obj = ((page is MachineryIntake) ? page : null);
				PixelElement obj2 = ((obj != null) ? ((MachineryIntake)obj).content : null);
				GameGridInventory val2 = (GameGridInventory)(object)((obj2 is GameGridInventory) ? obj2 : null);
				if (val2 != null)
				{
					((GameInventory)val2).identifier = "smuggler_bay";
				}
			}
		}
	}
	[HarmonyPatch(typeof(StorageExpansionHelper), "GetKitTabs")]
	internal static class BrewingStorageKitSize
	{
		private static void Postfix(GameItem __0, ref int __result)
		{
			if (ModEntry.Enabled && StorageExpansion.Supports(__0))
			{
				__result = 1;
			}
		}
	}
	[HarmonyPatch(typeof(StorageExpansionHelper), "AddTabs")]
	internal static class BrewingStoragePageIdentity
	{
		private static void Postfix(GameItem __0)
		{
			if (ModEntry.Enabled)
			{
				StorageExpansion.RetainHiddenPages(__0);
			}
		}
	}
	public static class WaterBottlePrinter
	{
		private sealed class View
		{
			internal readonly MachineryButton[] Buttons = (MachineryButton[])(object)new MachineryButton[3];

			internal MachineryText Status;
		}

		public const string Id = "be_xuxu_water_bottle_printer";

		private const string UpgradeTag = "BE_WATER_PRINTER_COMPONENTS";

		private static readonly string[] Products = new string[3] { "be_xuxu_small_water_bottle", "be_xuxu_slim_water_bottle", "be_xuxu_super_water_bottle" };

		private static readonly int[] Costs = new int[3] { 20, 40, 60 };

		private static readonly ConditionalWeakTable<GameItem, View> Views = new ConditionalWeakTable<GameItem, View>();

		private static readonly HashSet<GameItem> Busy = new HashSet<GameItem>();

		private static PlayerStore operationStore;

		private static WeakReference<PlayerStore> unsafeStore;

		private static string Text(string key, params object[] args)
		{
			return ModHelper.GetLocalized("brewing_expansion", key, args);
		}

		private static bool Live(GameItem item)
		{
			if (item != null)
			{
				GameItem obj = ((item is GameItemElement) ? item : null);
				if (obj == null)
				{
					return true;
				}
				return !((GameItemElement)obj).IsDestroyed();
			}
			return false;
		}

		private static bool Owned(GameItem item)
		{
			if (Live(item))
			{
				return GeneralHelper.IsItemOwned(item);
			}
			return false;
		}

		public static int Components(GameItem item)
		{
			return Math.Min(12, Math.Max(0, item.GetTagReadonly("BE_WATER_PRINTER_COMPONENTS").GetInt()));
		}

		public static int Level(GameItem item)
		{
			return 1 + Components(item) / 6;
		}

		public static GameSlotInventory Battery(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 0);
			return (GameSlotInventory)(object)((bodyStorage is GameSlotInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Output(GameItem item)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(item, 1);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		internal static bool CanSave(PlayerStore store)
		{
			if (unsafeStore != null && unsafeStore.TryGetTarget(out var target) && store == target)
			{
				return false;
			}
			if (store == operationStore)
			{
				return Busy.Count == 0;
			}
			return true;
		}

		private static void Protect(PlayerStore store, string reason)
		{
			unsafeStore = new WeakReference<PlayerStore>(store);
			Debug.LogError((object)("[BrewingExpansion.Formal] Printing recovery could not be confirmed; saving is blocked for this session: " + reason));
		}

		public static GameItem Create()
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			GameItem item = ItemDirectory.CreateEmptyItem("be_xuxu_water_bottle_printer");
			item.SetGameItemType("MACHINE");
			item.SetValue(300L);
			item.SetSpriteAndShapeFromMod("brewing_expansion", "be_xuxu_water_bottle_printer");
			item.SetShape(new GridShapeBuilder(2, 3).SetDataFill(2, 3, (byte)1).Build());
			item.EnableTag("CONTAINER_TAG", false);
			GameSlotInventory val = new GameSlotInventory(1, 2, false, false)
			{
				identifier = "BE_WATER_PRINTER_BATTERY"
			};
			GameGridInventory val2 = new GameGridInventory(6, 4)
			{
				identifier = "BE_WATER_PRINTER_OUTPUT"
			};
			((GameInventoryFunc)val2).mayInventoryAddItemFunc = (GameItem incoming, GameInventory _) => Owned(incoming) && Array.IndexOf(Products, incoming.identifier) >= 0;
			View view = new View
			{
				Status = MachineryText.Status("status", "")
			};
			for (int num = 0; num < 3; num++)
			{
				int choice = num;
				view.Buttons[num] = new MachineryButton(Text("water_printer_print_" + num), (Action)delegate
				{
					Print(item, choice);
				}, -1, -1).SetSize(240, 24);
			}
			MachineryStack val3 = MachineryStack.Vertical((PixelElement[])(object)new PixelElement[4]
			{
				(PixelElement)view.Buttons[0],
				(PixelElement)view.Buttons[1],
				(PixelElement)view.Buttons[2],
				(PixelElement)view.Status
			});
			PixelWindow contentWindow = MachineHelper.CreateMachineryWindow(true, (MachineryPanel[])(object)new MachineryPanel[3]
			{
				MachineHelper.PowerPanel(val),
				MachineHelper.Panel(Text("water_printer_output"), (PixelElement)(object)val2, "output", true),
				MachineHelper.Panel(Text("water_printer_control"), (PixelElement)(object)val3, "control", true)
			});
			item.SetContentWindow(contentWindow);
			MachineHelper.SetupBatterySlot(val, item);
			AudioHelper.InitSmallMachine(item);
			Views.Add(item, view);
			GameItem obj = item;
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Loaded));
			Loaded(item);
			return item;
		}

		private static void Loaded(GameItem item)
		{
			item.SetName(Text("water_printer_name"));
			item.shortDescription = Text("water_printer_description");
			item.contentWindow.SetTitle(item.name);
			Refresh(item);
			ItemHitArea.Apply(item);
		}

		private static void Refresh(GameItem item, string message = null)
		{
			if (!Views.TryGetValue(item, out var value))
			{
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				value.Buttons[i].SetText(Text("water_printer_print_" + i));
				if (i < Level(item))
				{
					((SelectableElement)value.Buttons[i]).Enable();
				}
				else
				{
					((SelectableElement)value.Buttons[i]).Disable();
				}
			}
			value.Status.SetText(message ?? ((Components(item) == 12) ? Text("water_printer_max") : Text("water_printer_progress", Level(item), Components(item) % 6)));
		}

		public static bool CanUpgrade(GameItem printer)
		{
			if (ModEntry.Enabled && Owned(printer) && printer.identifier == "be_xuxu_water_bottle_printer")
			{
				return Components(printer) < 12;
			}
			return false;
		}

		public static bool Upgrade(GameItem printer, GameItem component)
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance == null || !CanUpgrade(printer) || !Owned(component) || component.identifier != "common_electronic" || component.parentInventory == null)
			{
				return false;
			}
			if (component.unitCount != 1)
			{
				Refresh(printer, Text("water_printer_single_component"));
				return false;
			}
			if (!CanSave(instance) || !Busy.Add(printer))
			{
				return false;
			}
			operationStore = instance;
			int count = Components(printer);
			long unitValue = component.unitValue;
			GameInventory parentInventory = component.parentInventory;
			bool flag = false;
			try
			{
				bool flag2 = StartingPerk.IsTinkerer();
				Exception ex = null;
				try
				{
					StartingPerkContent.ConsumeUpgradeMaterial(component);
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
				if (instance != PlayerStore.Instance || !Owned(printer))
				{
					return false;
				}
				bool num = !Live(component) && component.parentInventory == null;
				bool flag3 = flag2 && ex == null && Live(component) && component.parentInventory == parentInventory && component.unitCount == 1;
				if (!num && !flag3)
				{
					if (!Live(component) && component.parentInventory != null)
					{
						Protect(instance, "upgrade material remained linked after destruction");
					}
					return false;
				}
				printer.ModifyTag("BE_WATER_PRINTER_COMPONENTS", (Action<TagState>)delegate(TagState s)
				{
					s.Enable().SetInt(count + 1);
				}, false);
				printer.unitValue += unitValue;
				flag = true;
				EmporiumAudioPlayer instance2 = EmporiumAudioPlayer.Instance;
				if (instance2 != null)
				{
					AudioSource ratchet = instance2.ratchet;
					if (ratchet != null)
					{
						ratchet.Play();
					}
				}
				return true;
			}
			catch (Exception ex3)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Water printer upgrade interrupted: " + ex3));
				return false;
			}
			finally
			{
				try
				{
					if (instance == PlayerStore.Instance && Live(printer))
					{
						if (!flag && !Live(component) && component.parentInventory == null && Components(printer) == count)
						{
							Protect(instance, "consumed upgrade material did not commit its upgrade");
						}
						Refresh(printer);
					}
				}
				catch (Exception ex4)
				{
					Debug.LogWarning((object)("[BrewingExpansion.Formal] Printer upgrade display refresh failed: " + ex4));
				}
				finally
				{
					Busy.Remove(printer);
					if (Busy.Count == 0)
					{
						operationStore = null;
					}
				}
			}
		}

		public static bool Print(GameItem printer, int choice)
		{
			PlayerStore store = PlayerStore.Instance;
			if (store == null || !ModEntry.Enabled || !CanSave(store) || !Owned(printer) || printer.identifier != "be_xuxu_water_bottle_printer" || choice < 0 || choice >= 3 || choice >= Level(printer) || !Busy.Add(printer))
			{
				return false;
			}
			operationStore = store;
			GameItem product = null;
			GameItem val = null;
			GameGridInventory val2 = null;
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			int refund = 0;
			bool flag3 = false;
			try
			{
				val2 = Output(printer);
				GameSlotInventory obj = Battery(printer);
				val = ((obj != null) ? obj.childItem : null);
				if (val2 == null || !Owned(val) || !PowerHelper.CanDrawPowerSource(val, Costs[choice]))
				{
					Refresh(printer, Text("water_printer_no_power"));
					return false;
				}
				num = val.GetTagReadonly("power_source_item_energy").GetInt();
				int currentUniqueId = store.currentUniqueId;
				product = DirectoryMaster.Item(Products[choice], true);
				if (!Current())
				{
					return false;
				}
				flag3 = Live(product) && product.identifier == Products[choice] && product.uniqueId > currentUniqueId && product.uniqueId <= store.currentUniqueId && product.parentInventory == null;
				if (!flag3)
				{
					Protect(store, "factory did not return a confirmed fresh bottle");
					return false;
				}
				SlotMarker val3 = val2.TryFindOneValidInventorySlot_TopMiddle(product, false);
				if (val3 == null || !val3.IsValid())
				{
					Refresh(printer, Text("water_printer_full"));
					return false;
				}
				val3.TryAcceptOnce(-1);
				if (Current() && (object)product.parentInventory == val2 && Owned(printer))
				{
					GameSlotInventory obj2 = Battery(printer);
					if (((obj2 != null) ? obj2.childItem : null) == val && Owned(val))
					{
						try
						{
							flag = PowerHelper.DrawPowerSource(val, Costs[choice]);
						}
						finally
						{
							if (Current() && Live(val))
							{
								refund = Math.Min(Costs[choice], Math.Max(0, num - val.GetTagReadonly("power_source_item_energy").GetInt()));
							}
						}
						if (!Current() || !flag || !Live(product) || (object)product.parentInventory != val2)
						{
							return false;
						}
						flag2 = true;
						return true;
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)("[BrewingExpansion.Formal] Water bottle printing interrupted: " + ex));
				return false;
			}
			finally
			{
				try
				{
					if (!flag2 && Current())
					{
						if (flag3 && Live(product))
						{
							if (GraphUtils.FindAllChildrenType<GameItem>((GraphNodeStorage)(object)product, (Func<GameItem, bool>)((GameItem child) => child != product), (Func<GameItem, bool>)null).Count == 0)
							{
								product.Destroy();
							}
							else
							{
								Protect(store, "unpaid output acquired contents during a foreign callback; preserving them");
							}
						}
						if (flag3 && (Live(product) || product.parentInventory != null))
						{
							Protect(store, "unpaid output could not be removed");
						}
						if (refund > 0 && Live(val))
						{
							val.ModifyTag("power_source_item_energy", (Action<TagState>)delegate(TagState s)
							{
								s.Enable().SetInt(s.GetInt() + refund);
							}, false);
							PowerHelper.UpdateEnergyCreditSprite(val);
						}
					}
				}
				catch (Exception ex2)
				{
					if (Current())
					{
						Protect(store, "rollback failed: " + ex2.Message);
					}
				}
				finally
				{
					Busy.Remove(printer);
					if (Busy.Count == 0)
					{
						operationStore = null;
					}
				}
			}
			bool Current()
			{
				return store == PlayerStore.Instance;
			}
		}
	}
	[HarmonyPatch(typeof(MachineBottlePrinter), "CanUpgradePrinter")]
	internal static class WaterPrinterUpgradeEligibility
	{
		private static bool Prefix(GameItem printer, ref bool __result)
		{
			if (((printer != null) ? printer.identifier : null) != "be_xuxu_water_bottle_printer" || !ModEntry.Enabled)
			{
				return true;
			}
			__result = WaterBottlePrinter.CanUpgrade(printer);
			return false;
		}
	}
	[HarmonyPatch(typeof(MachineBottlePrinter), "UpgradePrinter")]
	internal static class WaterPrinterUpgrade
	{
		private static bool Prefix(GameItem printer, GameItem upgradeItem)
		{
			if (((printer != null) ? printer.identifier : null) != "be_xuxu_water_bottle_printer" || !ModEntry.Enabled)
			{
				return true;
			}
			WaterBottlePrinter.Upgrade(printer, upgradeItem);
			return false;
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "SaveGame")]
	internal static class PrinterSaveBoundary
	{
		private static bool Prefix(PlayerStore __instance)
		{
			return WaterBottlePrinter.CanSave(__instance);
		}
	}
	[HarmonyPatch(typeof(PlayerStore), "WriteSlotFile")]
	internal static class PrinterSlotBoundary
	{
		private static bool Prefix(PlayerStore __instance)
		{
			return WaterBottlePrinter.CanSave(__instance);
		}
	}
	internal sealed class WaterContainerDefinition
	{
		internal readonly string Id;

		internal readonly string Name;

		internal readonly string SpritePrefix;

		internal readonly int CapacityMl;

		internal readonly int Price;

		internal readonly int Width;

		internal readonly int Height;

		internal readonly bool HasFilter;

		internal WaterContainerDefinition(string id, string name, string spritePrefix, int ml, int price, int width, int height, bool filter = false)
		{
			Id = id;
			Name = name;
			SpritePrefix = spritePrefix;
			CapacityMl = ml;
			Price = price;
			Width = width;
			Height = height;
			HasFilter = filter;
		}
	}
	internal static class WaterContainers
	{
		private static bool warnedVisual;

		internal static readonly WaterContainerDefinition[] All = new WaterContainerDefinition[4]
		{
			new WaterContainerDefinition("be_xuxu_small_water_bottle", "xuxu小水瓶", "be_small_bottle_", 6000, 20, 1, 3),
			new WaterContainerDefinition("be_xuxu_slim_water_bottle", "xuxu细水瓶", "be_slim_bottle_", 12000, 40, 1, 4),
			new WaterContainerDefinition("be_xuxu_super_water_bottle", "xuxu的超级水瓶", "be_super_bottle_", 18000, 60, 2, 4),
			new WaterContainerDefinition("be_clear_brew_water_bucket", "清酿澄水桶", "be_clear_bucket_", 60000, 300, 2, 3, filter: true)
		};

		internal static WaterContainerDefinition Find(GameItem item)
		{
			WaterContainerDefinition[] all = All;
			foreach (WaterContainerDefinition waterContainerDefinition in all)
			{
				if (((item != null) ? item.identifier : null) == waterContainerDefinition.Id)
				{
					return waterContainerDefinition;
				}
			}
			RemainingItems.Definition definition = RemainingItems.Find((item != null) ? item.identifier : null);
			if (definition?.Kind == "Water")
			{
				return new WaterContainerDefinition(definition.Id, ModHelper.GetLocalized("brewing_expansion", definition.Id + "_name", Array.Empty<object>()), definition.Sprite, definition.A, definition.Price, definition.Width, definition.Height, filter: true);
			}
			return null;
		}

		internal static GameItem Create(WaterContainerDefinition definition)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			GameItem obj = DirectoryMaster.Item(definition.HasFilter ? "water_pitcher" : "water_jug", false);
			obj.identifier = definition.Id;
			WaterHelper.EmptyContainer(obj);
			obj.ModifyTag("LIQUID_CONTAINER_BASE_PRICE_TAG", (Action<TagState>)delegate(TagState s)
			{
				s.Enable().SetInt(definition.Price);
			}, false);
			obj.SetValue((long)definition.Price);
			obj.SetShape(new GridShapeBuilder(definition.Width, definition.Height).SetDataFill(definition.Width, definition.Height, (byte)1).Build());
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Loaded));
			Loaded(obj);
			return obj;
		}

		private static void Loaded(GameItem item)
		{
			WaterContainerDefinition definition = Find(item);
			if (definition != null)
			{
				item.SetName(ModHelper.GetLocalized("brewing_expansion", definition.Id + "_name", Array.Empty<object>()));
				item.shortDescription = ModHelper.GetLocalized("brewing_expansion", definition.Id + "_description", Array.Empty<object>());
				item.ModifyTag("LIQUID_CONTAINER_CAPACITY", (Action<TagState>)delegate(TagState s)
				{
					s.Enable().SetInt(definition.CapacityMl * 1000);
				}, false);
				item.ModifyTag("LIQUID_CONTAINER_BASE_PRICE_TAG", (Action<TagState>)delegate(TagState s)
				{
					s.Enable().SetInt(definition.Price);
				}, false);
				if (!definition.HasFilter)
				{
					item.DisableTag("FILTER_CONTAINER_TAG", false);
				}
				Refresh(item);
			}
		}

		internal static void Refresh(GameItem item)
		{
			WaterContainerDefinition waterContainerDefinition = Find(item);
			if (waterContainerDefinition == null)
			{
				return;
			}
			if (RemainingItems.Find(item.identifier)?.Kind == "Water")
			{
				RemainingItems.Loaded(item);
				return;
			}
			((GameItemFunc)item).getItemSpriteModifierForItemFunc = null;
			int totalVolume = WaterHelper.GetTotalVolume(item);
			int currentCapacity = WaterHelper.GetCurrentCapacity(item);
			string text = ((totalVolume >= waterContainerDefinition.CapacityMl * 1000) ? "100" : ((currentCapacity >= 90) ? "90" : ((currentCapacity > 65) ? "75" : SpriteHelper.MapCapacity(currentCapacity))));
			string text2 = waterContainerDefinition.SpritePrefix + text;
			item.useSpriteFromMod = true;
			if (item.spriteAtlasPath != "brewing_expansion" || item.spritePath != text2)
			{
				item.SetSprite("brewing_expansion", text2);
			}
			GameItemElement val = (GameItemElement)(object)((item is GameItemElement) ? item : null);
			object obj;
			if (val == null)
			{
				obj = null;
			}
			else
			{
				TreeNodeRender background = val.background;
				obj = ((background != null) ? background.image : null);
			}
			if ((Object)obj != (Object)null)
			{
				TreeNodeRender fakeBackground = val.fakeBackground;
				if ((Object)(object)((fakeBackground != null) ? fakeBackground.image : null) != (Object)null)
				{
					item.SetSpriteQuick("brewing_expansion", text2);
					val.HandleInnateModSprite();
				}
			}
			ItemHitArea.Apply(item);
		}

		internal static void RefreshAfterNativeChange(GameItem item)
		{
			if (!ModEntry.Enabled || Find(item) == null)
			{
				return;
			}
			try
			{
				Refresh(item);
			}
			catch (Exception ex)
			{
				if (!warnedVisual)
				{
					Debug.LogWarning((object)("[BrewingExpansion.Formal] Water artwork refresh failed: " + ex));
				}
				warnedVisual = true;
			}
		}

		internal static bool Collect(string quality, GameItem container, int volume)
		{
			if (Find(container) == null)
			{
				return false;
			}
			if (quality != "Basewater" && quality != "High Quality Water" && quality != "Pure Water" && quality != "Ghostwater" && quality != "Rustwater" && quality != "Gutterflow")
			{
				return false;
			}
			int num = Math.Min(Math.Max(0, volume), WaterHelper.GetFreeCapacity(container));
			if (num == 0)
			{
				return true;
			}
			PlayerStore instance = PlayerStore.Instance;
			GameItem val = ItemDirectory.CreateEmptyItem("be_collection_water_temporary");
			try
			{
				WaterHelper.InitLiquidContainerItem(val, (num + 999) / 1000, 0, false, false, false, false, 3);
				switch (quality)
				{
				case "Basewater":
					WaterHelper.AddBaseWater(val, num);
					break;
				case "High Quality Water":
					WaterHelper.AddHighQualityWater(val, num);
					break;
				case "Pure Water":
					WaterHelper.AddPureWater(val, num);
					break;
				case "Ghostwater":
					WaterHelper.AddGhostWater(val, num);
					break;
				case "Rustwater":
					WaterHelper.AddRustWater(val, num);
					break;
				case "Gutterflow":
					WaterHelper.AddGutterflow(val, num);
					break;
				}
				WaterHelper.TransferLiquid(val, container);
			}
			finally
			{
				if (instance == PlayerStore.Instance && val.parentInventory == null)
				{
					val.Destroy();
				}
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(SpriteHelper), "UpdateWaterContainerSprite")]
	internal static class ModWaterSprite
	{
		private static void Postfix(GameItem gameItem)
		{
			WaterContainers.RefreshAfterNativeChange(gameItem);
		}
	}
	internal static class ModFaucetSprite
	{
		private static void Postfix(GameItem waterContainer)
		{
			WaterContainers.RefreshAfterNativeChange(waterContainer);
		}
	}
	[HarmonyPatch(typeof(MachineMoistureFarm), "Fill")]
	internal static class ModCollectionCapacity
	{
		private static bool Prefix(string quality, GameItem container, int volume)
		{
			if (!ModEntry.Enabled || WaterContainers.Find(container) == null)
			{
				return true;
			}
			try
			{
				return !WaterContainers.Collect(quality, container, volume);
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("[BrewingExpansion.Formal] Water collection interrupted: " + ex));
				return false;
			}
		}
	}
	public static class WineBottlePrinter
	{
		public const string Id = "be_wine_printer";

		private static readonly ConditionalWeakTable<GameItem, MachineryText> Statuses = new ConditionalWeakTable<GameItem, MachineryText>();

		private static string Text(string key)
		{
			return ModHelper.GetLocalized("brewing_expansion", key, Array.Empty<object>());
		}

		public static GameSlotInventory Battery(GameItem printer)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(printer, 0);
			return (GameSlotInventory)(object)((bodyStorage is GameSlotInventory) ? bodyStorage : null);
		}

		public static GameGridInventory Output(GameItem printer)
		{
			GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(printer, 1);
			return (GameGridInventory)(object)((bodyStorage is GameGridInventory) ? bodyStorage : null);
		}

		public static GameItem Create()
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Expected O, but got Unknown
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Expected O, but got Unknown
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			GameItem item = ItemDirectory.CreateEmptyItem("be_wine_printer");
			item.SetValue(300L);
			item.SetGameItemType("MACHINE");
			item.EnableTag("CONTAINER_TAG", false);
			item.SetSpriteAndShapeFromMod("brewing_expansion", "be_wine_printer");
			item.SetShape(new GridShapeBuilder(3, 3).SetDataFill(3, 3, (byte)1).Build());
			GameSlotInventory val = new GameSlotInventory(1, 2, false, false)
			{
				identifier = "BE_WINE_PRINTER_BATTERY"
			};
			GameGridInventory val2 = new GameGridInventory(4, 4)
			{
				identifier = "BE_WINE_PRINTER_OUTPUT"
			};
			((GameInventoryFunc)val2).mayInventoryAddItemFunc = (GameItem incoming, GameInventory _) => MachineConnections.Owned(incoming) && BrewingBarrel.EmptyBottle(incoming);
			MachineryText val3 = new MachineryText(Text("wine_printer_ready"), 6, (HorizontalAlignmentOptions)2, true);
			MachineryStack val4 = MachineryStack.Vertical((PixelElement[])(object)new PixelElement[2]
			{
				(PixelElement)new MachineryButton(Text("wine_printer_print"), (Action)delegate
				{
					Print(item, Output(item));
				}, -1, -1).SetSize(190, 24),
				(PixelElement)val3
			});
			item.SetContentWindow(MachineHelper.CreateMachineryWindow(true, (MachineryPanel[])(object)new MachineryPanel[3]
			{
				MachineHelper.PowerPanel(val),
				MachineHelper.Panel(Text("water_printer_output"), (PixelElement)(object)val2, "output", true),
				MachineHelper.Panel("", (PixelElement)(object)val4, "control", true)
			}));
			MachineHelper.SetupBatterySlot(val, item);
			AudioHelper.InitSmallMachine(item);
			Statuses.Add(item, val3);
			GameItem obj = item;
			((GameItemFunc)obj).onLoaded = (Action<GameItem>)Delegate.Combine(((GameItemFunc)obj).onLoaded, new Action<GameItem>(Loaded));
			Loaded(item);
			return item;
		}

		private static void Loaded(GameItem item)
		{
			item.SetName(Text("wine_printer_name"));
			string shortDescription = (item.longDescription = Text("wine_printer_description"));
			item.shortDescription = shortDescription;
			item.contentWindow.SetTitle(item.name);
			ItemHitArea.Apply(item);
			MachineConnections.Request();
		}

		private static void Status(GameItem item, string key)
		{
			if (Statuses.TryGetValue(item, out var value))
			{
				value.SetText(Text(key));
			}
		}

		private static bool AllowedTarget(GameItem printer, GameGridInventory target)
		{
			if (target == null)
			{
				return false;
			}
			if (target == Output(printer))
			{
				return true;
			}
			GameItem parentItem = ((GameInventory)target).GetParentItem();
			if (((parentItem != null) ? parentItem.identifier : null) == "be_large_brewing_barrel" && MachineConnections.Owned(parentItem) && target == BrewingBarrel.Bottles(parentItem))
			{
				return MachineConnections.PrinterFor(parentItem) == printer;
			}
			return false;
		}

		public static bool Print(GameItem printer, GameGridInventory target)
		{
			PlayerStore store = PlayerStore.Instance;
			List<GameItem> made;
			GameItem battery;
			if (store != null && ModEntry.Enabled)
			{
				GameItem obj = printer;
				if (!(((obj != null) ? obj.identifier : null) != "be_wine_printer") && MachineConnections.Owned(printer) && AllowedTarget(printer, target) && PulpOperations.CanSave(store) && WaterBottlePrinter.CanSave(store) && PulpOperations.Busy.Add(printer))
				{
					PulpOperations.Store = store;
					made = new List<GameItem>();
					battery = null;
					int debit = 0;
					bool flag = false;
					try
					{
						GameSlotInventory obj2 = Battery(printer);
						battery = ((obj2 != null) ? obj2.childItem : null);
						if (!MachineConnections.Owned(battery) || !PowerHelper.CanDrawPowerSource(battery, 20))
						{
							Status(printer, "water_printer_no_power");
							return false;
						}
						for (int i = 0; i < 4; i++)
						{
							int currentUniqueId = store.currentUniqueId;
							GameItem val = DirectoryMaster.Item("wine_bottle", true);
							if (!Current())
							{
								return false;
							}
							if (!MachineConnections.Live(val) || val.identifier != "wine_bottle" || val.parentInventory != null || val.uniqueId <= currentUniqueId || val.uniqueId > store.currentUniqueId || !BrewingBarrel.EmptyBottle(val))
							{
								PulpOperations.Protect(store, "wine printer factory returned an unconfirmed empty bottle");
								return false;
							}
							made.Add(val);
							SlotMarker val2 = target.TryFindOneValidInventorySlot(val, false);
							if (val2 == null || !val2.IsValid())
							{
								Status(printer, "water_printer_full");
								return false;
							}
							val2.TryAcceptOnce(-1);
							if (!Current() || (object)val.parentInventory != target)
							{
								return false;
							}
						}
						if (!Ready())
						{
							return false;
						}
						int num = battery.GetTagReadonly("power_source_item_energy").GetInt();
						bool flag2 = false;
						try
						{
							flag2 = PowerHelper.DrawPowerSource(battery, 20);
						}
						finally
						{
							if (Current() && MachineConnections.Live(battery))
							{
								debit = Math.Min(20, Math.Max(0, num - battery.GetTagReadonly("power_source_item_energy").GetInt()));
							}
						}
						if (!flag2 || !Ready())
						{
							return false;
						}
						flag = true;
						Status(printer, "wine_printer_done");
						MachineConnections.Request();
						return true;
					}
					catch (Exception ex)
					{
						Debug.LogWarning((object)("[BrewingExpansion.Formal] Wine printing interrupted: " + ex.Message));
						return flag;
					}
					finally
					{
						try
						{
							if (!flag && Current())
							{
								foreach (GameItem item in made)
								{
									if (MachineConnections.Live(item) && ((object)item.parentInventory == target || item.parentInventory == null) && item.FindAllChildItems().Count == 0)
									{
										item.Destroy();
									}
									if (MachineConnections.Live(item) || item.parentInventory != null)
									{
										PulpOperations.Protect(store, "unpaid printed bottle could not be safely removed");
									}
								}
								if (debit > 0 && MachineConnections.Live(battery))
								{
									battery.ModifyTag("power_source_item_energy", (Action<TagState>)delegate(TagState t)
									{
										t.Enable().SetInt(t.GetInt() + debit);
									}, false);
									PowerHelper.UpdateEnergyCreditSprite(battery);
								}
							}
						}
						catch (Exception ex2)
						{
							if (Current())
							{
								PulpOperations.Protect(store, "wine printer rollback failed: " + ex2.Message);
							}
						}
						finally
						{
							PulpOperations.Busy.Remove(printer);
							if (PulpOperations.Busy.Count == 0)
							{
								PulpOperations.Store = null;
							}
						}
					}
				}
			}
			return false;
			bool Current()
			{
				return store == PlayerStore.Instance;
			}
			bool Ready()
			{
				if (Current() && MachineConnections.Owned(printer) && MachineConnections.Owned(battery))
				{
					GameSlotInventory obj3 = Battery(printer);
					if (((obj3 != null) ? obj3.childItem : null) == battery && AllowedTarget(printer, target) && made.Count == 4)
					{
						return made.All((GameItem x) => MachineConnections.Live(x) && (object)x.parentInventory == target && BrewingBarrel.EmptyBottle(x));
					}
				}
				return false;
			}
		}
	}
	internal static class WineChemistry
	{
		internal const string RetainedWater = "BE_WINE_WATER_CHEMISTRY";

		internal static bool IsExpansionWine(GameItem item)
		{
			if (item == null)
			{
				return false;
			}
			string text = item.GetTagReadonly("MOD_ID_TAG").GetString();
			if (!string.IsNullOrEmpty(text) && text != "brewing_expansion")
			{
				return false;
			}
			if (BarrelWine.Recipe(item) == null && !RemainingItems.IsCollector(item))
			{
				return item.IsTag("BE_WINE_WATER_CHEMISTRY");
			}
			return true;
		}

		internal static Dictionary<string, int> Capture(GameItem item)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (Liquid liquid in Liquid.Liquids)
			{
				if (!(liquid.identifier == "water"))
				{
					int num = item.GetTagReadonly(liquid.currentPart).GetInt();
					if (num > 0)
					{
						dictionary.Add(liquid.identifier, num);
					}
				}
			}
			return dictionary;
		}

		internal static void Retain(GameItem item, Dictionary<string, int> parts, bool fromBarrel = false)
		{
			if (parts == null || !WineHelper.IsFermentingOrFinished(item) || (!fromBarrel && !IsExpansionWine(item)))
			{
				return;
			}
			string text = item.GetTagReadonly("MOD_ID_TAG").GetString();
			if (!string.IsNullOrEmpty(text) && text != "brewing_expansion")
			{
				return;
			}
			foreach (KeyValuePair<string, int> part in parts)
			{
				ChemicalHelper.SetChemContentUL(item, part.Key, part.Value);
			}
			if (parts.Count > 0)
			{
				item.EnableTag("BE_WINE_WATER_CHEMISTRY", false);
			}
			EnsureLoaded(item);
		}

		internal static (double Euphoria, double Toxicity) Totals(GameItem item)
		{
			double num = 0.0;
			double num2 = 0.0;
			foreach (Liquid liquid in Liquid.Liquids)
			{
				int num3 = Math.Max(0, item.GetTagReadonly(liquid.currentPart).GetInt());
				num += (double)num3 * liquid.euphoria;
				num2 += (double)num3 * liquid.lethality;
			}
			return (Euphoria: num, Toxicity: num2);
		}

		internal static void EnsureLoaded(GameItem item)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Invalid comparison between Unknown and I4
			if (IsExpansionWine(item))
			{
				(double, double) tuple = Totals(item);
				if ((tuple.Item1 > 0.0 || tuple.Item2 > 0.0) && (int)ChemicalFeedbackHelper.GetChemicalIntent(item) != 3)
				{
					ChemicalFeedbackHelper.InitChemicalIntentItem(item, (ChemicalIntent)3);
				}
			}
		}

		internal static void Reset(GameItem item)
		{
			foreach (Liquid liquid in Liquid.Liquids)
			{
				bool num;
				if (!item.IsTag("BE_WINE_WATER_CHEMISTRY"))
				{
					if (liquid.euphoria != 0.0)
					{
						goto IL_005e;
					}
					num = liquid.lethality != 0.0;
				}
				else
				{
					num = liquid.identifier != "water";
				}
				if (!num)
				{
					continue;
				}
				goto IL_005e;
				IL_005e:
				ChemicalHelper.SetChemContentUL(item, liquid.identifier, 0);
			}
			item.DisableTag("BE_WINE_WATER_CHEMISTRY", false);
			item.DisableTag("CHEMICAL_INTENT_TAG", false);
		}

		internal static string DisplayLine(GameItem item)
		{
			(double, double) tuple = Totals(item);
			string text = ChemicalFeedbackHelper.GetEuphoriaLevel(item) switch
			{
				1 => ModText.Get("be_euphoria_mild"), 
				2 => ModText.Get("be_euphoria_noticeable"), 
				3 => ModText.Get("be_euphoria_strong"), 
				4 => ModText.Get("be_euphoria_intense"), 
				_ => (tuple.Item1 > 0.0) ? ModText.Get("be_euphoria_faint") : ModText.Get("be_chemical_none"), 
			};
			string text2 = (ChemicalFeedbackHelper.IsLethal(item) ? ModText.Get("be_toxicity_lethal") : (ChemicalFeedbackHelper.IsToxic(item) ? ModText.Get("be_toxicity_toxic") : ((tuple.Item2 > 0.0) ? ModText.Get("be_toxicity_low") : ModText.Get("be_chemical_none"))));
			return ModText.Get("be_wine_chemical_feedback", tuple.Item1.ToString("0.##", CultureInfo.InvariantCulture), text, tuple.Item2.ToString("0.##", CultureInfo.InvariantCulture), text2);
		}

		internal static void AppendTooltip(RichTextBuilder builder, GameItem item)
		{
			if (ModEntry.Enabled && builder != null && IsExpansionWine(item) && (WineHelper.IsFermentingOrFinished(item) || RemainingItems.IsCollector(item)))
			{
				string text = DisplayLine(item);
				builder.AddLine(text, true, (ColorPalette)10312861, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
				builder.maxCharWidth = Math.Max(builder.maxCharWidth, text.Length * 2);
			}
		}
	}
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '9.1.0.7988')
