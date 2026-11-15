using System;
using System.Collections.Generic;
// Test doubles only; do not install in the game.
public interface IMod { void Init(ModManifest m); void OnEnable(); void OnDisable(); }
public class ModManifest { public string ID; }
public class ModLog { public ModLog(ModManifest m) {} public void Log(object s) {} public void Warning(object s) {} }
public class GameItem
{
    public virtual string name { get; set; }
    public string shortDescription { get; set; }
    public string longDescription { get; set; }
    public string flavorText { get; set; }
}
public class GameItemElement : GameItem {}
public class LoadedMod { public ModManifest Manifest; public bool IsEnabled = true; }
public class ModItemDirectory
{
    public Dictionary<string, Func<GameItem>> factoryDictionary = new Dictionary<string, Func<GameItem>>();
}
public static class ModLoader
{
    public static readonly HashSet<string> Enabled = new HashSet<string>(StringComparer.Ordinal);
    public static bool IsModEnabled(string mod) => Enabled.Contains(mod);
}
public static class ModHelper
{
    public static readonly HashSet<string> Running = new HashSet<string>(StringComparer.Ordinal);
    public static string Csv;
    public static bool IsModRunning(string mod) => Running.Contains(mod);
    public static string ReadModFileText(string mod, string file) => Csv;
}
public static class ModSourceHelper
{
    public static readonly Dictionary<Delegate, LoadedMod> Owners =
        new Dictionary<Delegate, LoadedMod>();
    public static LoadedMod FindModByDelegate(Delegate d) =>
        Owners.TryGetValue(d, out var value) ? value : null;
}
public static class DirectoryMaster
{
    public static ModItemDirectory Current;
    public static Func<GameItem> GetFactory<T>(string id) =>
        Current != null && Current.factoryDictionary.TryGetValue(id, out var f) ? f : null;
}
public static class ModHook
{
    public static event Action<LoadedMod> OnModAssetsLoaded;
    public static event Action<ModItemDirectory> OnModItemDirectoryInit;
    public static event Action OnGameLoadedLate;
    public static void Assets() => OnModAssetsLoaded?.Invoke(
        new LoadedMod { Manifest = new ModManifest { ID = PostItemTranslationProbe.ModEntry.Id } });
    public static void Directory(ModItemDirectory d)
    { DirectoryMaster.Current = d; OnModItemDirectoryInit?.Invoke(d); }
    public static void Late() => OnGameLoadedLate?.Invoke();
    public static int Subscriptions =>
        (OnModAssetsLoaded?.GetInvocationList().Length ?? 0) +
        (OnModItemDirectoryInit?.GetInvocationList().Length ?? 0) +
        (OnGameLoadedLate?.GetInvocationList().Length ?? 0);
}
