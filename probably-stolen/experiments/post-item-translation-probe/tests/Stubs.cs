using System;
using System.Collections.Generic;
// Minimal game API substitutes for deterministic unit tests, NOT shipped in DLL.
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
public class LoadedMod
{
    public ModManifest Manifest;
    public object Instance = new object();
    public bool IsEnabled = true;
}
public class ModItemDirectory
{
    public Dictionary<string, Func<GameItem>> factoryDictionary =
        new Dictionary<string, Func<GameItem>>(StringComparer.Ordinal);
}
public static class ModLoader
{
    public static readonly HashSet<string> Enabled = new HashSet<string>(StringComparer.Ordinal);
    public static readonly List<LoadedMod> Mods = new List<LoadedMod>();
    public static bool IsModEnabled(string mod) => Enabled.Contains(mod);
    public static bool IsValidModId(string mod) => !mod.Contains("/") && !mod.Contains("..");
}
public static class ModHelper
{
    public static readonly Dictionary<string,string> Files = new Dictionary<string,string>(StringComparer.Ordinal);
    public static bool ModFileExists(string mod, string file) => Files.ContainsKey(mod + "/" + file);
    public static string ReadModFileText(string mod, string file) => Files[mod + "/" + file];
}
public static class ModSourceHelper
{
    public static readonly Dictionary<Delegate, LoadedMod> Owners = new Dictionary<Delegate, LoadedMod>();
    public static LoadedMod FindModByDelegate(Delegate d) =>
        Owners.TryGetValue(d, out var m) ? m : null;
}
public static class DirectoryMaster
{
    public static ModItemDirectory Current;
    public static Func<GameItem> GetFactory<T>(string id) =>
        Current != null && Current.factoryDictionary.TryGetValue(id, out var f) ? f : null;
}
public class PaperElementData
{
    public string id, text, loc;
    public bool IsText=true, IsButton=false;
    public Dictionary<string,string> translations;
}
public class PaperTemplateData { public List<PaperElementData> elements = new List<PaperElementData>(); }
public class PaperPageData
{
    public string template;
    public List<PaperElementData> elements = new List<PaperElementData>();
}
public class PaperBookData
{
    public List<PaperElementData> chrome = new List<PaperElementData>();
    public List<PaperPageData> pages = new List<PaperPageData>();
    public PaperPageData GetPage(int i) => i >=0 && i<pages.Count ? pages[i] : null;
    public PaperTemplateData GetTemplate(string id) => null;
}
public class PaperUI
{
    public PaperBookData Data = new PaperBookData();
    public int CurrentPage;
    public bool IsAlive=true;
    public string Id,ModId;
    public readonly Dictionary<string,string> Rendered = new Dictionary<string,string>();
    private Action<PaperUI,int> shown;
    public bool Has(string id) => Data.GetPage(CurrentPage)?.elements.Exists(x=>x.id==id) == true;
    public string ResolveText(PaperElementData e, PaperPageData p) => e.text;
    public PaperUI SetText(string id,string value) { Rendered[id]=value; return this; }
    public PaperUI OnPageShown(Action<PaperUI,int> callback) { shown+=callback; return this; }
    public void FlipTo(int i) { CurrentPage=i; shown?.Invoke(this,i); }
}
public static class ModHook
{
    public static event Action<ModItemDirectory> OnModItemDirectoryInit;
    public static event Action OnGameLoadedLate;
    public static event Action<string,PaperUI> OnPaperOpened;
    public static void Directory(ModItemDirectory d) {
        DirectoryMaster.Current=d;
        OnModItemDirectoryInit?.Invoke(d);
    }
    public static void Late()=>OnGameLoadedLate?.Invoke();
    public static void Paper(string full, PaperUI ui)=>OnPaperOpened?.Invoke(full,ui);
    public static int SubscriptionCount=>
        (OnModItemDirectoryInit?.GetInvocationList().Length ?? 0)+
        (OnGameLoadedLate?.GetInvocationList().Length ?? 0)+
        (OnPaperOpened?.GetInvocationList().Length ?? 0);
}
