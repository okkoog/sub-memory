using System;
using PostItemTranslationProbe;
class Tests
{
    private static int checks;
    static void Assert(bool ok, string description)
    {
        checks++;
        if (!ok) throw new Exception("FAILED " + description);
        Console.WriteLine("PASS " + description);
    }
    static string Header = "ModID,ItemID,Field,Original,Korean\n";
    static void Bad(string csv)
    {
        bool rejected = false;
        try { Translation.Parse(csv); } catch (FormatException) { rejected = true; }
        Assert(rejected, "Malformed CSV rejected");
    }
    static ModEntry Start(string csv, params string[] targets)
    {
        ModLoader.Enabled.Clear(); ModHelper.Running.Clear();
        ModHelper.Csv = csv;
        foreach (var t in targets) { ModLoader.Enabled.Add(t); ModHelper.Running.Add(t); }
        var m = new ModEntry();
        m.Init(new ModManifest { ID = ModEntry.Id });
        m.OnEnable();
        ModHook.Assets();
        return m;
    }
    static void Main()
    {
        var row = Header + "lewd_meme,tentacle_body,name,触手本体,촉수 본체\n";
        var entries = Translation.Parse(row);
        Assert(entries.Count == 1, "Generic CSV has one item");
        var multi = row + "other_mod,other_item,name,English,한국어\n";
        Assert(Translation.Parse(multi).Count == 2, "Two distinct mod IDs without DLL edits");
        var doc = Translation.Parse("\uFEFF" + Header +
            "another_mod,item,name,\"A, \"\"quoted\"\"\n{0}\",\"한글, \"\"인용\"\"\n{0}\"\n");
        var itemTranslation = System.Linq.Enumerable.First(doc.Items);
        var item = new GameItemElement { name = "A, \"quoted\"\n{0}" };
        itemTranslation.Apply(item);
        Assert(item.name == "한글, \"인용\"\n{0}", "BOM, commas, quotes, multiline and placeholders");
        Bad(Header + "other,item,name,{0},{1}\n");
        Bad(Header + "other,item,name,A,B\nother,item,name,A,C\n");
        Bad(Header + "other,item,unitValue,A,B\n");
        Bad(Header + "other,item,name,\"broken");
        Bad(Header + "other,item,name,\"closed\"x,hi");
        Bad(null);

        var m = Start(row);
        Assert(ModHook.Subscriptions == 0, "No running targets => zero remaining subscriptions");
        m.OnDisable();
        m = Start("malformed", "lewd_meme");
        Assert(ModHook.Subscriptions == 0, "Malformed CSV => zero remaining subscriptions");
        m.OnDisable();

        m = Start(multi, "lewd_meme", "other_mod");
        var directory = new ModItemDirectory();
        int calls1 = 0, calls2 = 0;
        Func<GameItem> a = () => { calls1++; return new GameItemElement { name = "触手本体", shortDescription = "KEEP" }; };
        Func<GameItem> b = () => { calls2++; return new GameItemElement { name = "English" }; };
        directory.factoryDictionary["tentacle_body"] = a;
        directory.factoryDictionary["other_item"] = b;
        var owner1 = new LoadedMod { Manifest = new ModManifest { ID = "lewd_meme" } };
        var owner2 = new LoadedMod { Manifest = new ModManifest { ID = "other_mod" } };
        ModSourceHelper.Owners[a] = owner1; ModSourceHelper.Owners[b] = owner2;
        ModHook.Directory(directory);
        var wa = directory.factoryDictionary["tentacle_body"];
        var wb = directory.factoryDictionary["other_item"];
        Assert(wa != a && wb != b, "Two factory delegates wrapped dynamically");
        for (int i = 0; i < 1000; i++)
        {
            var x = wa(); var y = wb();
            if (x.name != "촉수 본체" || x.shortDescription != "KEEP" || y.name != "한국어")
                throw new Exception("Incorrect translation result");
        }
        Assert(calls1 == 1000 && calls2 == 1000, "Original factory called exactly once per creation");
        ModHook.Late();
        Assert(directory.factoryDictionary["tentacle_body"] == wa, "Late check never double-wraps");
        ModHook.Directory(directory);
        Assert(directory.factoryDictionary["other_item"] == wb, "Same directory event never double-wraps");
        owner1.IsEnabled = false;
        Assert(wa().name == "触手本体", "Disabled owner bypasses existing wrapper");
        owner1.IsEnabled = true;
        m.OnDisable();
        Assert(directory.factoryDictionary["tentacle_body"] == a &&
            directory.factoryDictionary["other_item"] == b &&
            ModHook.Subscriptions == 0, "Disable restores only own wrappers");

        m = Start(row, "lewd_meme");
        ModHook.Directory(directory);
        var current = directory.factoryDictionary["tentacle_body"];
        Func<GameItem> replacement = () => new GameItemElement { name = "third-party" };
        directory.factoryDictionary["tentacle_body"] = replacement;
        ModHook.Late();
        m.OnDisable();
        Assert(directory.factoryDictionary["tentacle_body"] == replacement, "Later third-party replacement untouched");

        m = Start(row, "lewd_meme");
        directory.factoryDictionary["tentacle_body"] = () => null;
        var nullFactory = directory.factoryDictionary["tentacle_body"];
        ModSourceHelper.Owners[nullFactory] = owner1;
        ModHook.Directory(directory);
        Assert(directory.factoryDictionary["tentacle_body"]() == null, "Null factory passes through");
        m.OnDisable();

        var subclass = new Child { name = "触手本体" };
        System.Linq.Enumerable.First(entries.Items).Apply(subclass);
        Assert(subclass.name == "触手本体", "Custom child item type not mutated");
        Console.WriteLine("RESULT: " + checks + " assertions passed, test doubles only");
    }
    sealed class Child : GameItemElement {}
}
