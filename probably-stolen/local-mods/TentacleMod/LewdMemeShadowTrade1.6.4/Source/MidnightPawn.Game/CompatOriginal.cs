using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace MidnightPawn.Game;

/// <summary>
/// 依赖原版 lewd_meme；使用 1.9.2 的公开 API，夜间只添加透支门禁与结算后追加：
/// - 物品/天赋/捕获/提示全部由原版提供；精液特性交给原版或其它 mod
/// - 本模组追加夜间产物与疲劳，报告另存为手写便签；另外补齐三种乳汁、名册、疲劳、收购商、治安部线
/// - 夜间补丁在 OriginalNightlyPatch 中插入少量调用，原版每日入口与外层遍历继续运行
/// </summary>
public static class CompatOriginal
{
	public const string ORIGINAL_ID = "lewd_meme";
	private const string Owner = "lewd_meme_remake.compat";

	private static bool installed;
	private static readonly Dictionary<string, Type> originalTypes = new Dictionary<string, Type>();

	internal static Type OriginalType(string typeName)
	{
		if (originalTypes.TryGetValue(typeName, out Type type)) return type;
		// The remake shares full type names with its prerequisite. Never resolve
		// by name alone: explicitly enumerate the prerequisite's checked scope.
		foreach (Type candidate in ModReflect.Types(ORIGINAL_ID))
		{
			if (candidate.FullName != typeName) continue;
			originalTypes[typeName] = candidate;
			return candidate;
		}
		return null;
	}

	public static bool Detected { get; private set; }

	public static bool TakeoverActive { get; private set; }

	/// <summary>与便签等兼容入口分开报告，避免夜间回退原版时误报为已接管。</summary>
	public static bool NightlySupplementActive => OriginalNightlyPatch.Active;

	/// <summary>设备疲劳门禁是否安装成功；原设备活动与概率保留。</summary>
	public static bool DeviceFatigueActive => OriginalDeviceActivityPatch.Active;

	public static string OriginalVersion { get; private set; } = "";

	/// <summary>调用原版程序集里的静态方法（例如 1.3.8 新加的 CorruptTentacle.DailyRecharge）。</summary>
	public static bool InvokeOriginalStatic(string typeName, string methodName, params object[] args)
	{
		try
		{
			LoadedMod original = FindOriginal();
			Assembly assembly = ((original != null) ? OriginalAssembly(original) : null);
			if (assembly == null)
			{
				return false;
			}
			Type type = OriginalType(typeName);
			MethodBase method = ((type != null) ? ModReflect.Target(type, methodName) : null);
			if (method == null)
			{
				return false;
			}
			ModReflect.CallStatic(type, methodName, args);
			return true;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("调用原版 " + typeName + "." + methodName + " 失败: " + ex.Message);
			return false;
		}
	}

	/// <summary>调用原版静态方法并返回其返回值（用于需要读取 bool 结果的场景）。</summary>
	public static object InvokeOriginalStaticResult(string typeName, string methodName, params object[] args)
	{
		try
		{
			LoadedMod original = FindOriginal();
			Assembly assembly = ((original != null) ? OriginalAssembly(original) : null);
			if (assembly == null)
			{
				return null;
			}
			Type type = OriginalType(typeName);
			MethodBase method = ((type != null) ? ModReflect.Target(type, methodName) : null);
			if (method == null)
			{
				return null;
			}
			return ModReflect.CallStatic(type, methodName, args);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("调用原版 " + typeName + "." + methodName + " 失败: " + ex.Message);
			return null;
		}
	}

	public static LoadedMod FindOriginal()
	{
		try
		{
			List<LoadedMod> mods = ModLoader.Mods;
			if (mods == null)
			{
				return null;
			}
			for (int i = 0; i < mods.Count; i++)
			{
				LoadedMod loadedMod = mods[i];
				if (loadedMod != null && loadedMod.Manifest != null && loadedMod.Manifest.ID == ORIGINAL_ID && loadedMod.IsEnabled)
				{
					return loadedMod;
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("检测原版 mod 失败: " + ex.Message);
		}
		return null;
	}

	public static bool Install()
	{
		if (installed)
		{
			return TakeoverActive;
		}
		// Init/OnEnable execute before the loader adds this mod to Mods. Without
		// its registered manifest, ModReflect cannot authorize our prerequisite.
		if (!ModLoader.Mods.Exists(mod => mod.IsEnabled && mod.Manifest?.ID == "lewd_meme_remake"
			&& mod.Assembly == typeof(CompatOriginal).Assembly))
		{
			return false;
		}
		try
		{
			LoadedMod original = FindOriginal();
			Detected = original != null;
			if (original == null)
			{
				// 原版是前置：缺失时提示，不再提供“无原版”分支
				ModCtx.LogError("未检测到前置模组 lewd_meme，本模组无法运行：请在模组列表启用原版「可能是触手」。");
				TakeoverActive = false;
				return false;
			}
			OriginalVersion = original.Manifest.ModVersion ?? "";
			if (!Version.TryParse(OriginalVersion, out Version version) || version < new Version(1, 9, 2))
			{
				throw new InvalidOperationException("暗流交易需要前置 lewd_meme 1.9.2 或更高版本；当前为 " + OriginalVersion);
			}
			Assembly assembly = OriginalAssembly(original);
			if (assembly == null)
			{
				ModCtx.LogWarning("找不到原版程序集，跳过接管。");
				TakeoverActive = false;
				return false;
			}
			Harmony harmony = new Harmony(Owner);
			int num = 0;
			num += OriginalNightlyPatch.Install(assembly);
			// 原版 tooltip 保持运行；本模组通过 OnCreateTooltipLate 只追加独有内容
			num += PatchNoteZone(harmony, assembly);
			num += PatchEnsureLayout(harmony, assembly);
			num += PatchMaidenFactory(harmony);
			num += OriginalDeviceActivityPatch.Install();
			num += OriginalLootPatch.Install();
			num += OriginalBuyerGuestPatch.Install(harmony);
			// 精液特性完全交给原版 / 其它 mod，本模组不接管、不清理
			installed = num > 0;
			TakeoverActive = num > 0;
			ModCtx.Log($"前置原版 {OriginalVersion}：已接入 {num} 个兼容入口（夜间追加、透支门禁、便签区及角色初始化；未透支时完整运行原版活动、孕育和产出）。");
			if (!NightlySupplementActive) ModCtx.LogWarning("夜间追加未生效，当前沿用原版夜间规则；其它兼容入口仍可工作。");
			return TakeoverActive;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("原版共存接管失败: " + ex.Message);
			TakeoverActive = false;
			return false;
		}
	}

	/// <summary>加载器只卸载其自身 owner；动态兼容补丁由本模组定向清理。</summary>
	internal static void Uninstall()
	{
		OriginalNightlyPatch.Uninstall();
		OriginalDeviceActivityPatch.Uninstall();
		OriginalLootPatch.Uninstall();
		UnpatchOwner(Owner);
		installed = false;
		Detected = false;
		TakeoverActive = false;
		OriginalVersion = "";
		originalTypes.Clear();
		NotePrepared = new ConditionalWeakTable<GameGridInventory, object>();
	}

	internal static void UnpatchOwner(string owner)
	{
		try
		{
			new Harmony(owner).UnpatchAll(owner);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("撤销补丁 " + owner + " 失败，请重启游戏: " + ex.Message);
		}
	}

	/// <summary>保留原版窗口构建，仅在返回后纵向扩展第五个便签网格。</summary>
	private static int PatchNoteZone(Harmony harmony, Assembly assembly)
	{
		try
		{
			Type type = OriginalType("MidnightPawn.Game.TentacleBody");
			MethodBase method = ((type != null) ? ModReflect.Target(type, "BuildWindow") : null);
			MethodInfo postfix = typeof(CompatOriginal).GetMethod("BuildWindowPostfix", BindingFlags.Static | BindingFlags.NonPublic);
			if (method == null || postfix == null)
			{
				ModCtx.LogWarning("原版窗口或便签扩展补丁不存在，保持原版窗口");
				return 0;
			}
			harmony.Patch(method, null, new HarmonyMethod(postfix));
			ModCtx.Log("保留原版五区窗口，仅纵向扩展便签区");
			return 1;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("便签区扩展补丁失败，保持原版窗口: " + ex.Message);
			return 0;
		}
	}

	private static void BuildWindowPostfix(ref List<GameGridInventory> __1, PixelWindow __result)
	{
		if (__1 != null && __1.Count >= 5)
		{
			PrepareNote(__1[4], false);
		}
		// 1.4.4 删除了 TrySeedZones。只在原生 Show 已恢复物品存储后补发，
		// 不在 BuildWindow 阶段向尚未挂载、可能被读档覆盖的网格塞物品。
		if (__result != null) __result.openCallback += () => PrepareOpenedNotes(__result);
	}

	private static void PrepareOpenedNotes(PixelWindow window)
	{
		try
		{
			if (window?.parentItems == null) return;
			foreach (GameItem body in window.parentItems.ToArray())
			{
				if (body == null || body.contentWindow != window
					|| (body.identifier != "tentacle_body" && !body.IsTag("TENTACLE_BODY_TAG"))) continue;
				foreach (GameGridInventory note in TentacleBody.GetNoteZones(body)) PrepareNote(note, true);
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("打开本体时补发说明失败: " + ex.Message);
		}
	}

	private static int PatchEnsureLayout(Harmony harmony, Assembly assembly)
	{
		try
		{
			Type type = OriginalType("MidnightPawn.Game.TentacleBody");
			MethodBase method = ((type != null) ? ModReflect.Target(type, "EnsureLayout") : null);
			if (method == null)
			{
				ModCtx.LogWarning("原版 TentacleBody.EnsureLayout 不存在，跳过便签区升级检查");
				return 0;
			}
			MethodInfo postfix = typeof(CompatOriginal).GetMethod("EnsureLayoutPostfix", BindingFlags.Static | BindingFlags.NonPublic);
			harmony.Patch(method, null, new HarmonyMethod(postfix));
			ModCtx.Log("已接入便签区升级检查（宽 2、高 4，原地扩展并补齐说明）");
			return 1;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("接入便签区升级检查失败: " + ex.Message);
			return 0;
		}
	}

	private static void EnsureLayoutPostfix()
	{
		try
		{
			MidnightPawn.Game.TentacleBody.UpgradeLegacyNoteZone();
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("便签区升级检查失败: " + ex.Message);
		}
	}

	/// <summary>1.6.x 陷阱直接调用新工厂，不经过 DirectoryMaster.Item。</summary>
	private static int PatchMaidenFactory(Harmony harmony)
	{
		try
		{
			Type type = OriginalType("MidnightPawn.Game.ItemFactory");
			MethodBase method = type == null ? null : ModReflect.Target(type, "CreateMaidenOfFaction", new[] { typeof(string) });
			if (method == null) return 0;
			harmony.Patch(method, postfix: new HarmonyMethod(typeof(CompatOriginal).GetMethod(nameof(MaidenFactoryPostfix), BindingFlags.Static | BindingFlags.NonPublic)));
			ModCtx.Log("已接入按势力生成角色的工厂，补齐直接生成路径的违禁属性、偏好和估值");
			return 1;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("角色工厂初始化补丁失败: " + ex.Message);
			return 0;
		}
	}

	private static void MaidenFactoryPostfix(GameItem __result)
	{
		PatchItemCreateContraband.InitializeMaiden(__result);
	}

	private static ConditionalWeakTable<GameGridInventory, object> NotePrepared = new ConditionalWeakTable<GameGridInventory, object>();

	internal static void PrepareNote(GameGridInventory note, bool seed)
	{
		if (note == null)
		{
			return;
		}
		try
		{
			// Deserialization may restore the original 2x2 after BuildWindow ran.
			// Recheck dimensions even when this inventory's filter is prepared.
			if (note.inventoryShape == null || note.inventoryShape.width != 2 || note.inventoryShape.height != 4)
			{
				note.SetShape(2, 4);
			}
			if (!NotePrepared.TryGetValue(note, out _))
			{
				GameInventoryFunc inventory = (GameInventoryFunc)(object)note;
				Func<GameItem, GameInventory, bool> previous = inventory.mayInventoryAddItemFunc;
				inventory.mayInventoryAddItemFunc = (GameItem item, GameInventory inv) => (item != null && (item.identifier == "tentacle_manual" || item.identifier == "tentacle_manual_remake")) || (previous != null && previous(item, inv));
				NotePrepared.Add(note, new object());
			}
			if (seed)
			{
				SeedNote(note, "tentacle_manual");
				SeedNote(note, "tentacle_manual_remake");
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("便签区扩展失败: " + ex.Message);
		}
	}

	private static void SeedNote(GameGridInventory note, string identifier)
	{
		if (HasNoteItem(note, identifier))
		{
			return;
		}
		GameItem item = DirectoryMaster.Item(identifier, true);
		if (item == null)
		{
			return;
		}
		try
		{
			ModHelper.SetItemOwnerShip(item, true);
			// 只处理刚生成的临时说明书；默认 parentInventory 非空会导致放置被拒绝。
			item.parentInventory = null;
			if (GraphUtils.CanAccept((GraphNodeStorage)(object)note, item))
			{
				GraphUtils.TryAcceptAll((GraphNodeStorage)(object)note, item);
			}
		}
		finally
		{
			// TryAcceptAll 返回的是剩余数量，0 也不能单独证明已经挂入网格。
			if (item.parentInventory == null) item.Destroy();
		}
	}

	private static bool HasNoteItem(GameGridInventory note, string identifier)
	{
		try
		{
			List<GameItem> items = ((GameInventory)note).childItems;
			if (items == null)
			{
				return false;
			}
			foreach (GameItem item in items)
			{
				if (item != null && item.identifier == identifier)
				{
					ItemFactory.RefreshManualDescription(item);
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static Assembly OriginalAssembly(LoadedMod original)
	{
		try
		{
			if (original.Assembly != null)
			{
				return original.Assembly;
			}
		}
		catch
		{
		}
		try
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				if (assemblies[i].GetName().Name == "MidnightPawn")
				{
					return assemblies[i];
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("枚举程序集失败: " + ex.Message);
		}
		return null;
	}

}
