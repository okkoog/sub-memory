using System;
using System.Linq;
using PostItemTranslationProbe;
class Tests
{
    private static int checks;
    static void Check(bool c,string reason)
    {
        checks++; if(!c)throw new Exception("FAILED: "+reason);
        Console.WriteLine("PASS "+reason);
    }
    static string Header="Type,Target,Field,Original,Korean\n";
    static string Row="Item,tentacle_body,name,触手本体,촉수 본체\n";
    static LoadedMod Owner(string id,bool enabled=true)
    {
        var m=new LoadedMod { Manifest=new ModManifest { ID=id },IsEnabled=enabled};
        ModLoader.Mods.Add(m);
        if(enabled)ModLoader.Enabled.Add(id);
        return m;
    }
    static void Reset()
    {
        ModLoader.Mods.Clear(); ModLoader.Enabled.Clear(); ModHelper.Files.Clear();
        DirectoryMaster.Current=null; ModSourceHelper.Owners.Clear();
        StartingPerkList.Registered.Clear();
    }
    static ModEntry Enable()
    {
        var m=new ModEntry();m.Init(new ModManifest { ID=ModEntry.Id });m.OnEnable();return m;
    }
    static void Bad(string csv)
    {
        bool rejected=false;
        try {Translation.Parse("bad_mod",csv);}
        catch(FormatException) {rejected=true;}
        Check(rejected,"Invalid CSV safely rejected");
    }
    static void Main()
    {
        var one=Translation.Parse("lewd_meme",Header+Row);
        Check(one.ItemCount==1&&one.PaperCount==0&&one.PerkCount==0,"Mod ID comes from filename, not DLL");
        var two=Translation.Parse("another_mod",Header+
             "Item,gadget,name,Original,번역\n"+
             "Paper,guide,title,Original title,한글 제목\n");
        Check(two.ItemCount==1&&two.PaperCount==1,"One per-mod CSV can mix item and UI");
        var multiline=Translation.Parse("x","\uFEFF"+Header+
             "Item,id,name,\"A, \"\"quoted\"\"\n{0}\",\"한글, \"\"인용\"\"\n{0}\"\n");
        var item=multiline.Items.Single();
        var objectItem=new GameItemElement{name="A, \"quoted\"\n{0}"};
        item.Apply(objectItem);
        Check(objectItem.name=="한글, \"인용\"\n{0}","UTF-8 BOM/CSV quotes/comma/newlines/placeholders");
        foreach(var bad in new[]{Header+"Item,x,name,{0},{1}\n",
             Header+Row+Row,Header+"Paper,b,a,Hello,안녕\nPaper,b,a,Hello,안녕\n",
             Header+"Dialogue,x,y,A,B\n",Header+"Item,x,value,A,B\n",
             Header+"Item,x,name,\"unfinished",Header+"Item,x,name,\"ok\"x,hi",
             Header+"Perk,bad_mod.fake,invalid,Original,Korean\n",
             Header+"Perk,other_mod.fake,name,Original,Korean\n",
             Header+"Perk,bad_mod.fake,name,Original,Korean\nPerk,bad_mod.fake,name,Original,Korean\n"})Bad(bad);

        Reset();
        var disabled=Owner("lewd_meme",false);
        ModHelper.Files[ModEntry.Id+"/Translations/lewd_meme.csv"]=Header+Row;
        var probe=Enable();
        var directory=new ModItemDirectory();
        ModHook.Directory(directory);
        Check(ModHook.SubscriptionCount==1,"No active target leaves only non-frame bootstrap event");
        probe.OnDisable();
        Check(ModHook.SubscriptionCount==0,"Disabled probe unsubscribes remaining bootstrap event");

        Reset();
        var owner=Owner("lewd_meme");
        var other=Owner("other_mod");
        var skip=Owner("no_file_mod");
        ModHelper.Files[ModEntry.Id+"/Translations/lewd_meme.csv"]=Header+Row+
          "Paper,guide,title,Notice,공지\n";
        ModHelper.Files[ModEntry.Id+"/Translations/other_mod.csv"]=Header+
          "Item,other_item,name,English,한국어\n";
        var m=Enable();
        int ca=0,cb=0;
        Func<GameItem> a=()=>{ca++;return new GameItemElement{name="触手本体",shortDescription="KEEP"};};
        Func<GameItem> b=()=>{cb++;return new GameItemElement{name="English"};};
        directory=new ModItemDirectory();
        directory.factoryDictionary["tentacle_body"]=a;
        directory.factoryDictionary["other_item"]=b;
        ModSourceHelper.Owners[a]=owner;ModSourceHelper.Owners[b]=other;
        ModHook.Directory(directory);
        var aa=directory.factoryDictionary["tentacle_body"];
        var bb=directory.factoryDictionary["other_item"];
        Check(aa!=a&&bb!=b,"Two separate per-mod CSV files auto discovered");
        for(int i=0;i<1000;i++)
        {
            var x=aa();var y=bb();
            if(x.name!="촉수 본체"||x.shortDescription!="KEEP"||y.name!="한국어")
                throw new Exception("Wrong translation");
        }
        Check(ca==1000&&cb==1000,"Factory invoked exactly once per spawn");
        ModHook.Late();
        Check(directory.factoryDictionary["tentacle_body"]==aa,"No duplicate wrapping on load late");

        var ui=new PaperUI { Id="guide",ModId="lewd_meme" };
        var page=new PaperPageData();
        page.elements.Add(new PaperElementData{id="title",text="Notice"});
        ui.Data.pages.Add(page);
        ModHook.Paper("lewd_meme:guide",ui);
        Check(ui.Rendered.TryGetValue("title",out var ko)&&ko=="공지",
              "Paper output translated via official OnPaperOpened");
        ui.Rendered.Clear();
        ui.FlipTo(0);
        Check(ui.Rendered.ContainsKey("title"),"Page switch re-applies paper text");
        ui.Rendered.Clear();
        ModHook.Paper("other_mod:guide",ui);
        Check(ui.Rendered.Count==0,"Paper UI from wrong mod untouched");

        var different=new PaperUI();
        var dp=new PaperPageData();
        dp.elements.Add(new PaperElementData{id="title",text="Changed by author"});
        different.Data.pages.Add(dp);
        ModHook.Paper("lewd_meme:guide",different);
        Check(different.Rendered.Count==0,"Paper source mismatch untouched");

        var localized=new PaperUI();
        var lp=new PaperPageData();
        lp.elements.Add(new PaperElementData{id="title",text="Notice",loc="existing_key"});
        localized.Data.pages.Add(lp);
        ModHook.Paper("lewd_meme:guide",localized);
        Check(localized.Rendered.Count==0,"Paper with original localization protected");

        owner.IsEnabled=false;
        Check(aa().name=="触手本体","Disabled owner bypasses installed wrapper");
        owner.IsEnabled=true;
        m.OnDisable();
        Check(directory.factoryDictionary["tentacle_body"]==a&&
              directory.factoryDictionary["other_item"]==b&&ModHook.SubscriptionCount==0,
              "Disable restores only owned wrappers and unsubscribes hooks");

        Reset();
        Owner("lewd_meme");
        ModHelper.Files[ModEntry.Id+"/Translations/lewd_meme.csv"]="BROKEN";
        probe=Enable();
        directory=new ModItemDirectory();
        ModHook.Directory(directory);
        Check(ModHook.SubscriptionCount==1,"Corrupt CSV does not leave any translation hooks");
        probe.OnDisable();
        Check(ModHook.SubscriptionCount==0,"Corrupt CSV bootstrap unsubscribes on disable");

        Reset();
        owner=Owner("lewd_meme");
        ModHelper.Files[ModEntry.Id+"/Translations/lewd_meme.csv"]=Header+Row;
        probe=Enable();
        directory=new ModItemDirectory();
        a=()=>null;
        directory.factoryDictionary["tentacle_body"]=a;
        ModSourceHelper.Owners[a]=owner;
        ModHook.Directory(directory);
        Check(directory.factoryDictionary["tentacle_body"]()==null,
              "Null factory result preserved");
        Func<GameItem> later=()=>new GameItemElement{name="Third-party"};
        directory.factoryDictionary["tentacle_body"]=later;
        ModHook.Late();
        probe.OnDisable();
        Check(directory.factoryDictionary["tentacle_body"]==later,
              "Subsequent third-party factory replacement left alone");

        var child=new Child{name="触手本体"};
        one.Items.Single().Apply(child);
        Check(child.name=="触手本体","Custom child type never mutated");
        // Early starting perk translation: new-game UI opens before item directory.
        Reset();
        owner=Owner("lewd_meme");
        var originalName = "触手起源";
        var originalDesc = "你生来就不是人。开局获得【触手本体】。";
        Func<string> nameOrigin=()=>originalName;
        Func<string> descOrigin=()=>originalDesc;
        var perk=new StartingPerk {
            id="lewd_meme.tentacle_origin", source="lewd_meme",
            cost=0,maxSlot=3,
            rawName=originalName,rawDescription=originalDesc,
            nameProvider=nameOrigin,descriptionProvider=descOrigin
        };
        StartingPerkList.Registered.Add(perk.id,perk);
        ModHelper.Files[ModEntry.Id+"/Translations/lewd_meme.csv"]=Header+
            "Perk,lewd_meme.tentacle_origin,name,触手起源,촉수 기원\n"+
            "Perk,lewd_meme.tentacle_origin,description,你生来就不是人。开局获得【触手本体】。,태어날 때부터 인간이 아니었다. 시작 아이템 【촉수 본체】를 얻는다.\n";
        probe=Enable();
        Check(perk.GetLocalizedDisplayName()=="촉수 기원"&&
              perk.GetLocalizedDescription().StartsWith("태어날 때부터"),
              "Starting perk name and description translated BEFORE item directory initialization");
        Check(perk.id=="lewd_meme.tentacle_origin"&&perk.cost==0&&perk.maxSlot==3,
              "Perk id, cost and slot metadata unchanged");
        var installedName=perk.nameProvider;
        ModHook.Assets(owner);
        Check(perk.nameProvider==installedName,"Repeated asset callbacks do not wrap perk twice");
        originalName="English name";
        Check(perk.GetLocalizedDisplayName()=="English name",
              "Other-language value is never force translated");
        originalName="触手起源";
        probe.OnDisable();
        Check(perk.nameProvider==nameOrigin&&perk.descriptionProvider==descOrigin,
              "Disabling probe restores original registered name and description providers");

        // Late-loaded target: registration after our probe's OnEnable.
        Reset();
        ModHelper.Files[ModEntry.Id+"/Translations/lewd_meme.csv"]=Header+
            "Perk,lewd_meme.tentacle_origin,name,触手起源,촉수 기원\n";
        probe=Enable();
        owner=Owner("lewd_meme");
        perk=new StartingPerk {id="lewd_meme.tentacle_origin",
              nameProvider=()=> "触手起源"};
        StartingPerkList.Registered.Add(perk.id,perk);
        ModHook.Assets(owner);
        Check(perk.GetLocalizedDisplayName()=="촉수 기원",
              "Perk of subsequently loaded mod updated on official asset-loaded event");
        probe.OnDisable();
        Check(perk.GetLocalizedDisplayName()=="触手起源",
              "Late-loaded perk provider restored on disable");

        // Mismatch and missing mod: never overwrite other values.
        Reset();
        owner=Owner("lewd_meme");
        perk=new StartingPerk {id="lewd_meme.tentacle_origin",
              nameProvider=()=> "Updated Chinese text"};
        StartingPerkList.Registered.Add(perk.id,perk);
        ModHelper.Files[ModEntry.Id+"/Translations/lewd_meme.csv"]=Header+
            "Perk,lewd_meme.tentacle_origin,name,触手起源,촉수 기원\n";
        var unchanged=perk.nameProvider;
        probe=Enable();
        Check(perk.nameProvider==unchanged&&perk.GetLocalizedDisplayName()=="Updated Chinese text",
              "Original mismatch produces zero edits to the trait object");
        probe.OnDisable();

        Reset();
        probe=Enable();
        Check(StartingPerkList.Registered.Count==0,
              "No target mod means no perk registry mutations");
        probe.OnDisable();
        Check(ModHook.SubscriptionCount==0,"Probe cleanup removes all official event subscriptions");

        Console.WriteLine("RESULT: "+checks+" assertions passed (stubbed API)");
    }
    private sealed class Child:GameItemElement{}
}
