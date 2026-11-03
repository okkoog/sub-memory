using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue)]
[assembly: AssemblyVersion("0.0.0.0")]
internal static class NativeSettings
{
	internal static ConfigEntry<int> Number(ModConfig config, string key, int value, string section, Func<string> label, int min, int max, Func<string> tooltip = null, bool requiresRestart = false)
	{
		bool flag = requiresRestart;
		ConfigEntry<int> entry = config.Define(key, value, section, label, (int?)null, (int?)null, (Func<string>)(() => Hint(tooltip, min.ToString(), max.ToString())), flag);
		entry.OnChanged += delegate(int candidate)
		{
			int num = Clamp(candidate);
			if (num != candidate)
			{
				entry.Set(num);
			}
		};
		entry.Set(Clamp(entry.Value));
		return entry;
		int Clamp(int candidate)
		{
			return Math.Max(min, Math.Min(max, candidate));
		}
	}

	internal static ConfigEntry<float> Number(ModConfig config, string key, float value, string section, Func<string> label, float min, float max, Func<string> tooltip = null, bool requiresRestart = false)
	{
		float num = value;
		bool flag = requiresRestart;
		ConfigEntry<float> entry = config.Define(key, num, section, label, (float?)null, (float?)null, (Func<string>)(() => Hint(tooltip, min.ToString(CultureInfo.InvariantCulture), max.ToString(CultureInfo.InvariantCulture))), flag);
		entry.OnChanged += delegate(float candidate)
		{
			float num2 = Clamp(candidate);
			if (!num2.Equals(candidate))
			{
				entry.Set(num2);
			}
		};
		entry.Set(Clamp(entry.Value));
		return entry;
		float Clamp(float candidate)
		{
			return (float.IsNaN(candidate) || float.IsInfinity(candidate)) ? value : Math.Max(min, Math.Min(max, candidate));
		}
	}

	private static string Hint(Func<string> tooltip, string min, string max)
	{
		return ((tooltip == null) ? "" : (tooltip() + "\n")) + NativeText.Get("common.range", min, max);
	}
}
internal static class NativeText
{
	private static ModManifest manifest;

	private static string prefix;

	private static bool listening;

	internal static void Bind(ModManifest value, string keyPrefix)
	{
		manifest = value;
		prefix = keyPrefix;
		ModHook.OnModAssetsLoaded += AssetsLoaded;
	}

	internal static string Get(string key, params object[] args)
	{
		return (manifest == null) ? key : ModHelper.GetLocalized(manifest.ID, key, args);
	}

	private static void AssetsLoaded(LoadedMod mod)
	{
		if (!(mod.Manifest.ID != manifest.ID))
		{
			UpdateMetadata();
			if (!listening)
			{
				LocalizationSettings.SelectedLocaleChanged += LocaleChanged;
				listening = true;
			}
		}
	}

	private static void LocaleChanged(Locale locale)
	{
		UpdateMetadata();
	}

	private static void UpdateMetadata()
	{
		manifest.Name = Get(prefix + ".name");
		manifest.Description = Get(prefix + ".summary");
	}

	internal static void Unbind()
	{
		ModHook.OnModAssetsLoaded -= AssetsLoaded;
		if (listening)
		{
			LocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
		}
		listening = false;
	}
}
namespace HydroponicAutoReplant;

public sealed class Core : IMod
{
	private sealed class Binding
	{
		private readonly WeakReference<GameItem> machine;

		private readonly WeakReference<PixelWindow> window;

		private readonly WeakReference<MachineryPanelRow> row;

		private readonly WeakReference<MachineryPanel> panel;

		private readonly WeakReference<MachineryRadioButton> toggle;

		internal Action OpenSync;

		internal bool IsUsable
		{
			get
			{
				if (!TryGetMachine(out var _))
				{
					return false;
				}
				if (!TryGetWindow(out var value2) || (Object)(object)value2.handler == (Object)null)
				{
					return false;
				}
				if (!TryGetRow(out var value3) || (Object)(object)value3.handler == (Object)null)
				{
					return false;
				}
				if (!TryGetPanel(out var value4) || (Object)(object)value4.handler == (Object)null)
				{
					return false;
				}
				if (!TryGetToggle(out var value5) || (Object)(object)((SelectableElement)value5).handler == (Object)null)
				{
					return false;
				}
				return true;
			}
		}

		internal Binding(GameItem machine, PixelWindow window, MachineryPanelRow row, MachineryPanel panel, MachineryRadioButton toggle)
		{
			this.machine = new WeakReference<GameItem>(machine);
			this.window = new WeakReference<PixelWindow>(window);
			this.row = new WeakReference<MachineryPanelRow>(row);
			this.panel = new WeakReference<MachineryPanel>(panel);
			this.toggle = new WeakReference<MachineryRadioButton>(toggle);
		}

		internal bool TryGetMachine(out GameItem value)
		{
			return machine.TryGetTarget(out value) && value != null;
		}

		internal bool TryGetWindow(out PixelWindow value)
		{
			return window.TryGetTarget(out value) && value != null;
		}

		internal bool TryGetRow(out MachineryPanelRow value)
		{
			return row.TryGetTarget(out value) && value != null;
		}

		internal bool TryGetPanel(out MachineryPanel value)
		{
			return panel.TryGetTarget(out value) && value != null;
		}

		internal bool TryGetToggle(out MachineryRadioButton value)
		{
			return toggle.TryGetTarget(out value) && value != null;
		}
	}

	internal const string AutoReplantTag = "local.hydroponic_auto_replant.enabled";

	internal const string UiRole = "hydroponic_auto_replant.settings";

	internal const string TextPrefix = "hydro_auto_replant";

	private static readonly Action<GameItem, GameInventory, SlotMarker> AfterCycleHandler = AfterCycle;

	internal static Core Current;

	private readonly List<Binding> bindings = new List<Binding>();

	private readonly List<WeakReference<GameItem>> cycleHooks = new List<WeakReference<GameItem>>();

	private string modId;

	private bool active;

	private bool replantRunning;

	public void Init(ModManifest manifest)
	{
		if (modId == null)
		{
			NativeText.Bind(manifest, "hydro_auto_replant");
			modId = manifest.ID;
		}
	}

	public void OnEnable()
	{
		if (!active)
		{
			active = true;
			Current = this;
			ModHook.OnGameLoadedLate += OnGameLoadedLate;
			ModLoader.OnTrustRefreshed += OnTrustRefreshed;
		}
	}

	public void OnDisable()
	{
		Shutdown();
	}

	private void Shutdown()
	{
		bool flag = active;
		active = false;
		if (Current == this)
		{
			Current = null;
		}
		if (flag)
		{
			ModHook.OnGameLoadedLate -= OnGameLoadedLate;
		}
		ModLoader.OnTrustRefreshed -= OnTrustRefreshed;
		NativeText.Unbind();
		RemoveAllUi();
		RemoveAllCycleHooks();
		replantRunning = false;
	}

	private void OnTrustRefreshed()
	{
		if (active)
		{
			LoadedMod val = ModLoader.Mods.Find((LoadedMod m) => m.Manifest.ID == modId);
			if (val != null && !val.IsEnabled)
			{
				Shutdown();
			}
		}
	}

	internal void Install(GameItem machine)
	{
		if (!active || machine == null)
		{
			return;
		}
		try
		{
			EnsureUi(machine);
		}
		catch (Exception e)
		{
			Warn("install ui " + machine.identifier, e);
		}
		try
		{
			EnsureCycleHook(machine);
		}
		catch (Exception e2)
		{
			Warn("install cycle " + machine.identifier, e2);
		}
	}

	private void OnGameLoadedLate()
	{
		if (!active)
		{
			return;
		}
		try
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance == null)
			{
				return;
			}
			List<GameItem> list = instance.FindAllItem(true);
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				GameItem val = list[i];
				if (IsHydroponic(val))
				{
					try
					{
						EnsureUi(val);
					}
					catch (Exception e)
					{
						Warn("game loaded ui " + val.identifier, e);
					}
					try
					{
						EnsureCycleHook(val);
					}
					catch (Exception e2)
					{
						Warn("game loaded cycle " + val.identifier, e2);
					}
				}
			}
			CleanupDeadBindings();
		}
		catch (Exception e3)
		{
			Warn("game loaded sync", e3);
		}
	}

	private static void AfterCycle(GameItem machine, GameInventory parentInventory, SlotMarker slot)
	{
		try
		{
			TryReplant(machine);
		}
		catch (Exception e)
		{
			Warn("cycle replant " + ((machine == null) ? "?" : machine.identifier), e);
		}
	}

	internal static bool TryReplant(GameItem machine)
	{
		Core current = Current;
		if (current == null || !current.active || current.replantRunning)
		{
			return false;
		}
		current.replantRunning = true;
		try
		{
			return TryReplantNow(machine);
		}
		finally
		{
			current.replantRunning = false;
		}
	}

	private static bool TryReplantNow(GameItem machine)
	{
		if (!IsHydroponic(machine))
		{
			return false;
		}
		if (!GeneralHelper.IsItemEffectivelyOwned(machine))
		{
			return false;
		}
		if (!IsEnabledFor(machine))
		{
			return false;
		}
		if (MachineProgressHelper.GetMachineState(machine) != "STATE_READY")
		{
			return false;
		}
		GameSlotInventory inputSlot = MachineHydroponic.GetInputSlot(machine);
		GameGridInventory outputSlot = MachineHydroponic.GetOutputSlot(machine);
		if (inputSlot == null || outputSlot == null)
		{
			return false;
		}
		if (inputSlot.childItem != null)
		{
			return false;
		}
		GameItem val = FindSeed(outputSlot);
		if (val == null)
		{
			return false;
		}
		if (!GeneralHelper.IsItemEffectivelyOwned(machine))
		{
			return false;
		}
		if (!IsEnabledFor(machine))
		{
			return false;
		}
		if (MachineProgressHelper.GetMachineState(machine) != "STATE_READY")
		{
			return false;
		}
		if (inputSlot.childItem != null)
		{
			return false;
		}
		if ((object)val.parentInventory != outputSlot)
		{
			return false;
		}
		if (!val.MayRemove())
		{
			return false;
		}
		SlotMarker val2 = ((GameInventory)inputSlot).TryInventorySlot(val, 1, val.modifiedShape, (TagSystem)null);
		if (val2 == null || val2.numTransfer < 1)
		{
			return false;
		}
		if (!GeneralHelper.IsItemEffectivelyOwned(machine))
		{
			return false;
		}
		if (!IsEnabledFor(machine))
		{
			return false;
		}
		if (MachineProgressHelper.GetMachineState(machine) != "STATE_READY")
		{
			return false;
		}
		if (inputSlot.childItem != null)
		{
			return false;
		}
		if ((object)val.parentInventory != outputSlot)
		{
			return false;
		}
		if (!val.MayRemove())
		{
			return false;
		}
		return val2.TryAcceptOnce(1) > 0;
	}

	private static GameItem FindSeed(GameGridInventory output)
	{
		List<GameItem> childItems = ((GameInventory)output).childItems;
		for (int i = 0; i < childItems.Count; i++)
		{
			GameItem val = childItems[i];
			if (val != null && (object)val.parentInventory == output && GeneralHelper.IsItemOwned(val) && GeneralHelper.IsItemEffectivelyOwned(val) && val.IsTag("SEED") && !val.IsTag("CONTAINER_TAG") && !GeneralHelper.IsItemLocked(val) && !ItemPinHelper.IsPinned(val) && val.MayRemove() && val.MaxNumRemove() >= 1)
			{
				return val;
			}
		}
		return null;
	}

	internal static bool IsEnabledFor(GameItem machine)
	{
		return machine != null && machine.GetTagReadonly("local.hydroponic_auto_replant.enabled").GetBool();
	}

	private static void SetEnabled(GameItem machine, bool on)
	{
		machine.ModifyTag("local.hydroponic_auto_replant.enabled", (Action<TagState>)delegate(TagState state)
		{
			state.Enable().SetBool(on);
		}, false);
	}

	private void ToggleChanged(GameItem machine, bool on)
	{
		if (!active || machine == null)
		{
			return;
		}
		try
		{
			SetEnabled(machine, on);
			if (on)
			{
				TryReplant(machine);
			}
		}
		catch (Exception e)
		{
			Warn("toggle " + machine.identifier, e);
			SyncMachine(machine);
		}
	}

	private static bool IsHydroponic(GameItem machine)
	{
		return machine != null && machine.IsTag("HYDROPONIC_MACHINE_TAG") && machine.contentWindow != null;
	}

	private void EnsureCycleHook(GameItem machine)
	{
		if (!IsHydroponic(machine) || HasCycleHookRecord(machine))
		{
			return;
		}
		Action<GameItem, GameInventory, SlotMarker> onCycleEndSlotItemFunc = ((GameItemFunc)machine).onCycleEndSlotItemFunc;
		if (onCycleEndSlotItemFunc == null)
		{
			Debug.LogWarning((object)("[HydroponicAutoReplant] " + machine.identifier + " has no vanilla onCycleEndSlotItemFunc; cycle hook skipped."));
			return;
		}
		if (!HasAfterCycle(onCycleEndSlotItemFunc))
		{
			((GameItemFunc)machine).onCycleEndSlotItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Combine(onCycleEndSlotItemFunc, AfterCycleHandler);
		}
		cycleHooks.Add(new WeakReference<GameItem>(machine));
	}

	private bool HasCycleHookRecord(GameItem machine)
	{
		for (int num = cycleHooks.Count - 1; num >= 0; num--)
		{
			if (!cycleHooks[num].TryGetTarget(out var target))
			{
				cycleHooks.RemoveAt(num);
			}
			else if (target == machine)
			{
				return true;
			}
		}
		return false;
	}

	private static bool HasAfterCycle(Action<GameItem, GameInventory, SlotMarker> callback)
	{
		if (callback == null)
		{
			return false;
		}
		Delegate[] invocationList = callback.GetInvocationList();
		for (int i = 0; i < invocationList.Length; i++)
		{
			if (object.Equals(invocationList[i], AfterCycleHandler))
			{
				return true;
			}
		}
		return false;
	}

	private void RemoveAllCycleHooks()
	{
		for (int num = cycleHooks.Count - 1; num >= 0; num--)
		{
			if (cycleHooks[num].TryGetTarget(out var target))
			{
				try
				{
					Action<GameItem, GameInventory, SlotMarker> onCycleEndSlotItemFunc = ((GameItemFunc)target).onCycleEndSlotItemFunc;
					if (HasAfterCycle(onCycleEndSlotItemFunc))
					{
						((GameItemFunc)target).onCycleEndSlotItemFunc = (Action<GameItem, GameInventory, SlotMarker>)Delegate.Remove(onCycleEndSlotItemFunc, AfterCycleHandler);
					}
				}
				catch (Exception e)
				{
					Warn("remove cycle hook " + target.identifier, e);
				}
			}
		}
		cycleHooks.Clear();
	}

	private void EnsureUi(GameItem machine)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		if (!IsHydroponic(machine))
		{
			return;
		}
		Binding binding = FindBinding(machine);
		if (binding != null)
		{
			if (binding.IsUsable && IsPanelAttached(binding))
			{
				SyncBinding(binding);
				return;
			}
			RemoveBinding(binding);
		}
		PixelWindow contentWindow = machine.contentWindow;
		MachineryPanelRow val = (MachineryPanelRow)((contentWindow != null) ? /*isinst with value type is only supported in some contexts*/: null);
		if (val == null)
		{
			return;
		}
		MachineryPanel panel = val.GetPanel("hydroponic_auto_replant.settings");
		if (panel != null)
		{
			try
			{
				val.Detach((PixelElement)(object)panel);
			}
			catch (Exception e)
			{
				if ((Object)(object)panel.handler != (Object)null)
				{
					Warn("remove stale panel", e);
				}
			}
			DestroyPanel(panel);
		}
		WeakReference<GameItem> machineRef = new WeakReference<GameItem>(machine);
		MachineryRadioButton val2 = new MachineryRadioButton(NativeText.Get("hydro_auto_replant.toggle"), IsEnabledFor(machine), (Action<bool>)delegate(bool on)
		{
			if (machineRef.TryGetTarget(out var target))
			{
				ToggleChanged(target, on);
			}
		});
		val2.SetDescription(NativeText.Get("hydro_auto_replant.toggle_hint"));
		MachineryPanel val3 = new MachineryPanel(NativeText.Get("hydro_auto_replant.settings"), (PixelElement)(object)val2, "hydroponic_auto_replant.settings");
		val.Add(val3);
		Binding binding2 = new Binding(machine, contentWindow, val, val3, val2);
		binding2.OpenSync = delegate
		{
			if (binding2.IsUsable)
			{
				SyncBinding(binding2);
			}
		};
		contentWindow.openCallback = (Action)Delegate.Combine(contentWindow.openCallback, binding2.OpenSync);
		bindings.Add(binding2);
		SyncBinding(binding2);
	}

	private Binding FindBinding(GameItem machine)
	{
		for (int num = bindings.Count - 1; num >= 0; num--)
		{
			Binding binding = bindings[num];
			if (!binding.TryGetMachine(out var value))
			{
				RemoveBinding(binding);
			}
			else if (value == machine)
			{
				return binding;
			}
		}
		return null;
	}

	private static bool IsPanelAttached(Binding binding)
	{
		if (!binding.TryGetRow(out var value))
		{
			return false;
		}
		if (!binding.TryGetPanel(out var value2))
		{
			return false;
		}
		return value.GetPanel("hydroponic_auto_replant.settings") == value2;
	}

	private void SyncMachine(GameItem machine)
	{
		Binding binding = FindBinding(machine);
		if (binding != null)
		{
			SyncBinding(binding);
		}
	}

	private void SyncBinding(Binding binding)
	{
		if (binding == null || !binding.TryGetMachine(out var value) || !binding.TryGetPanel(out var value2) || !binding.TryGetToggle(out var value3))
		{
			return;
		}
		try
		{
			value2.SetLabel(NativeText.Get("hydro_auto_replant.settings"));
			value3.SetText(NativeText.Get("hydro_auto_replant.toggle"));
			value3.SetDescription(NativeText.Get("hydro_auto_replant.toggle_hint"));
			MachineryContainerUtils.NotifySizeChanged((PixelElement)(object)value3);
			value3.SetOn(IsEnabledFor(value), false);
		}
		catch (Exception e)
		{
			Warn("sync ui", e);
		}
	}

	private void CleanupDeadBindings()
	{
		for (int num = bindings.Count - 1; num >= 0; num--)
		{
			Binding binding = bindings[num];
			if (!binding.IsUsable)
			{
				RemoveBinding(binding);
			}
		}
	}

	private void RemoveAllUi()
	{
		List<Binding> list = new List<Binding>(bindings);
		for (int i = 0; i < list.Count; i++)
		{
			RemoveBinding(list[i]);
		}
		bindings.Clear();
	}

	private void RemoveBinding(Binding binding)
	{
		if (binding == null)
		{
			return;
		}
		bindings.Remove(binding);
		try
		{
			if (binding.TryGetWindow(out var value) && binding.OpenSync != null)
			{
				PixelWindow obj = value;
				obj.openCallback = (Action)Delegate.Remove(obj.openCallback, binding.OpenSync);
			}
		}
		catch (Exception e)
		{
			Warn("remove open callback", e);
		}
		try
		{
			if (binding.TryGetPanel(out var value2))
			{
				try
				{
					GameItem value4;
					if (binding.TryGetRow(out var value3))
					{
						value3.Detach((PixelElement)(object)value2);
					}
					else if (binding.TryGetMachine(out value4))
					{
						PixelWindow contentWindow = value4.contentWindow;
						PixelElement obj2 = ((contentWindow != null) ? contentWindow.childElement : null);
						MachineryPanelRow val = (MachineryPanelRow)(object)((obj2 is MachineryPanelRow) ? obj2 : null);
						if (val != null)
						{
							val.Detach((PixelElement)(object)value2);
						}
					}
				}
				catch (Exception e2)
				{
					if ((Object)(object)value2.handler != (Object)null)
					{
						Warn("remove panel", e2);
					}
				}
				DestroyPanel(value2);
			}
		}
		catch (Exception e3)
		{
			Warn("remove panel", e3);
		}
		binding.OpenSync = null;
	}

	private static void DestroyPanel(MachineryPanel panel)
	{
		try
		{
			if (panel != null && (Object)(object)panel.handler != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)panel.handler).gameObject);
			}
		}
		catch
		{
		}
	}

	private static void Warn(string context, Exception e)
	{
		Debug.LogWarning((object)("[HydroponicAutoReplant] " + context + ": " + e));
	}
}
[HarmonyPatch(typeof(MachineHydroponic), "Hydroponic")]
internal static class HydroponicPatch
{
	private static void Postfix(GameItem __result)
	{
		Core.Current?.Install(__result);
	}
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '9.1.0.7988')
