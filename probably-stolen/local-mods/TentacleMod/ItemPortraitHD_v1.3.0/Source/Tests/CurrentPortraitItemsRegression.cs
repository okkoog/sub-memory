using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using ItemPortraitHD;

static class CurrentPortraitItemsRegression
{
    const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    static T Blank<T>() => (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
    static void Set(object obj, Type owner, string property, object value) =>
        owner.GetProperty(property, Instance).SetValue(obj, value);
    static GameItemElement Item(bool maiden = true)
    {
        var item = Blank<GameItemElement>();
        var tags = new TagSystem();
        if (maiden) tags.dict.Add("MAIDEN_FEMALE", new TagState("MAIDEN_FEMALE", "") { valueEnabled = true });
        Set(item, typeof(GameItem), "modifiedState", tags);
        return item;
    }
    static GameGridInventory Inventory(params GameItem[] items)
    {
        var inv = Blank<GameGridInventory>();
        typeof(GameGridInventory).GetField("items", Instance).SetValue(inv, items.ToList());
        return inv;
    }
    static void Container(GameItem item, GameGridInventory inventory)
    {
        var window = Blank<PixelWindow>();
        Set(window, typeof(PixelWindow), "childElement", inventory);
        Set(item, typeof(GameItem), "contentWindow", window);
    }
    internal static void Run(Action<bool, string> check)
    {
        var type = typeof(ModEntry).Assembly.GetType("ItemPortraitHD.CurrentPortraitItems");
        var collect = type.GetMethod("Collect", BindingFlags.NonPublic | BindingFlags.Static);
        List<GameItemElement> Collect(params GraphNodeStorage[] roots) =>
            (List<GameItemElement>)collect.Invoke(null, new object[] { roots });

        var saved = Item();
        var innerBox = Item(false);
        var outerBox = Item(false);
        var inside = Inventory(saved);
        Container(innerBox, inside);
        Container(outerBox, Inventory(innerBox));
        var counter = Inventory(outerBox);
        check(!GeneralHelper.IsItemOwned(saved) && Collect(counter).SequenceEqual(new[] { saved }),
            "saved portrait item without individual ownership tag is found inside two closed containers");
        var guest = Item();
        Set(guest, typeof(GameItem), "useSpriteFromMod", true);
        Set(guest, typeof(GameItem), "spriteAtlasPath", "lewd_meme");
        Set(guest, typeof(GameItem), "spritePath", "guestp_test");
        check(Collect(Inventory(guest, saved, Item(false))).SequenceEqual(new[] { saved }),
            "recursive inventory scan excludes guest display and unrelated items");

        ((List<GameItem>)typeof(GameGridInventory).GetField("items", Instance).GetValue(inside)).Add(outerBox);
        ((List<GameItem>)typeof(GameGridInventory).GetField("items", Instance).GetValue(inside)).Add(null);
        check(Collect(null, counter, counter, inside).SequenceEqual(new[] { saved }),
            "shared inventory references, cycles and null nodes do not duplicate items or hang");

        var sold = Inventory(saved);
        typeof(GameGridInventory).GetField("items", Instance).SetValue(inside, new List<GameItem>());
        check(Collect(counter).Count == 0 && Collect(sold).Count == 1,
            "sold item is removed immediately even while its former display object remains alive");
        check(Collect().Count == 0 && Collect((GraphNodeStorage)null).Count == 0,
            "empty or not yet initialized inventory roots are safe");

        var scanCode = PatchProcessor.GetOriginalInstructions(type.GetMethod("Scan", BindingFlags.NonPublic | BindingFlags.Static), (ILGenerator)null);
        var getters = scanCode.Where(i => i.operand is MethodInfo).Select(i => ((MethodInfo)i.operand).Name).ToList();
        check(getters.Contains("get_backInvinvElementCounter") && scanCode.Any(i => i.operand is FieldInfo f && f.Name == "afterhourWindow") &&
            !getters.Contains("get_soldElement") && !getters.Contains("GetAllOwnedItems") && !getters.Contains("IsItemOwned"),
            "manager roots include back-room counter and scavenging window, exclude sold storage and per-item ownership filtering");

        var windowState = Blank<PortraitWindow>();
        void Field(string name, object value) => typeof(PortraitWindow).GetField(name, Instance).SetValue(windowState, value);
        object Read(string name) => typeof(PortraitWindow).GetField(name, Instance).GetValue(windowState);
        Field("itemSearch", "previous save search"); Field("selected", saved);
        Field("currentItems", new List<GameItemElement> { saved }); Field("items", new List<GameItemElement> { saved });
        Field("nextItemRefresh", 999f);
        typeof(PortraitWindow).GetMethod("ResetItems", Instance).Invoke(windowState, null);
        check((string)Read("itemSearch") == "" && Read("selected") == null &&
            ((List<GameItemElement>)Read("currentItems")).Count == 0 && ((List<GameItemElement>)Read("items")).Count == 0 &&
            (float)Read("nextItemRefresh") == 0,
            "loading a save clears stale search, selection and item references and schedules immediate refresh");
        var loadCode = PatchProcessor.GetOriginalInstructions(typeof(ModEntry).GetMethod("OnLoaded", Instance), (ILGenerator)null);
        check(loadCode.Any(i => i.operand is MethodInfo m && m.Name == "ResetItems"),
            "game-loaded callback resets the manager item view");
    }
}
