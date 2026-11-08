using System;
using System.Linq;
using CorpPresence.Malice;
using CorpPresence.Runtime;
using CorpPresence.UI;
using HarmonyLib;
using UnityEngine;

namespace CorpPresence;

public class Main : IMod
{
	private const string ReleaseVersion = "1.0.0";

	public const string Version = "1.0.0";

	public const string DisplayName = "CorpPresence";

	private Harmony _harmony;

	private GameObject _host;

	private GameObject _bootstrapObject;

	private ResourceBootstrap _bootstrap;

	private readonly RuntimeLifetime _runtime = new RuntimeLifetime(delegate(Exception error)
	{
		ModLog.Warn("卸载公司运行时资源失败：" + error.Message);
	});

	public void Init(ModManifest manifest)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		_harmony = new Harmony("mod." + manifest.ID);
		Settings.Init(manifest.ID);
	}

	public void OnEnable()
	{
		bool flag;
		try
		{
			flag = _runtime.Start(EnableCore);
		}
		catch (Exception error)
		{
			ReportStartupRollback(error);
			throw;
		}
		if (flag)
		{
			_bootstrap.Tick();
		}
	}

	private void EnableCore()
	{
		_runtime.Own(ExternalTranslations.Reload, ExternalTranslations.Clear);
		_runtime.Own(GameResources.Begin, GameResources.Reset);
		_runtime.Own(Noop, LoadedItemRepair.Reset);
		_runtime.Own(ItemRegistry.Install, ItemRegistry.Uninstall);
		_runtime.Own(delegate
		{
			Hooks.InstallCore(_harmony);
		}, delegate
		{
			Harmony harmony = _harmony;
			if (harmony != null)
			{
				harmony.UnpatchAll(_harmony.Id);
			}
		});
		_runtime.Own(CreateBootstrap, DestroyBootstrap);
	}

	private void CompleteStartup()
	{
		_runtime.Continue(EnableResources);
		DestroyBootstrap();
	}

	private void EnableResources()
	{
		_runtime.Own(SilverBlood.InstallResources, SilverBlood.UninstallResources);
		_runtime.Own(Tobacco.RegisterPlant, Tobacco.UnregisterPlant);
		_runtime.Own(Perfume.InstallResources, Perfume.UninstallResources);
		Hooks.InstallResources(_harmony);
		_runtime.Own(Hooks.InstallEvents, Hooks.UninstallEvents);
		_runtime.Own(CorpPresence.Malice.Malice.Install, CorpPresence.Malice.Malice.Uninstall);
		_runtime.Own(Noop, CatalogPricing.Reset);
		_runtime.Own(Noop, Crates.Reset);
		ItemRegistry.Activate();
		_runtime.Own(Noop, Tex.ReleaseUi);
		_runtime.Own(Noop, Theme.ResetCachedFont);
		_runtime.Own(Noop, Portraits.ResetSceneReferences);
		_runtime.Own(Noop, PhoneDialogueLayout.Reset);
		_runtime.Own(Noop, Notebook.Destroy);
		_runtime.Own(Noop, InputShield.Destroy);
		_runtime.Own(Noop, CompanyWindow.ReleaseUi);
		_runtime.Own(CreateHost, DestroyHost);
		ModLog.Info("公司增强 1.0.0 资源阶段已加载（人物：" + string.Join("、", Engine.All.Select((Character c) => c.Id)) + "）");
	}

	public void OnDisable()
	{
		_runtime.Stop();
	}

	private void ReportStartupRollback(Exception error)
	{
		_runtime.Stop();
		try
		{
			ModLog.Error("公司增强初始化失败，已执行运行时回滚：" + error);
		}
		catch
		{
		}
		RollbackNotice.Queue();
	}

	private static void Noop()
	{
	}

	private void CreateBootstrap()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		_bootstrapObject = new GameObject("CorpPresence_Bootstrap");
		Object.DontDestroyOnLoad((Object)(object)_bootstrapObject);
		((Object)_bootstrapObject).hideFlags = (HideFlags)61;
		_bootstrap = _bootstrapObject.AddComponent<ResourceBootstrap>();
		_bootstrap.Configure(CompleteStartup, ReportStartupRollback);
	}

	private void DestroyBootstrap()
	{
		_bootstrap?.Detach();
		_bootstrap = null;
		if (!((Object)(object)_bootstrapObject == (Object)null))
		{
			_bootstrapObject.SetActive(false);
			Object.Destroy((Object)(object)_bootstrapObject);
			_bootstrapObject = null;
		}
	}

	private void CreateHost()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		_host = new GameObject("CorpPresence_Host");
		Object.DontDestroyOnLoad((Object)(object)_host);
		((Object)_host).hideFlags = (HideFlags)61;
		_host.AddComponent<Host>();
	}

	private void DestroyHost()
	{
		if (!((Object)(object)_host == (Object)null))
		{
			_host.SetActive(false);
			Object.Destroy((Object)(object)_host);
			_host = null;
		}
	}
}
