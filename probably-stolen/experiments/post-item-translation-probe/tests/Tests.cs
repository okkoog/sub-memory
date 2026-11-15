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
        Check(one.ItemCount==1&&one.PaperCount==0,"Mod ID comes from filename, not DLL");
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
             Header+"Item,x,name,\"unfinished",Header+"Item,x,name,\"ok\"x,hi"})Bad(bad);

        Reset();
        var disabled=Owner("lewd_meme",false);
        ModHelper.Files[ModEntry.Id+"/Translations/lewd_meme.csv"]=Header+Row;
        var probe=Enable();
        var directory=new ModItemDirectory();
        ModHook.Directory(directory);
        Check(ModHook.SubscriptionCount==0,"Disabled target unsubscribes all hooks after discovery");
        probe.OnDisable();

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
        Check(ModHook.SubscriptionCount==0,"Corrupt CSV leaves no persistent hooks");
        probe.OnDisable();

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
        Console.WriteLine("RESULT: "+checks+" assertions passed (stubbed API)");
    }
    private sealed class Child:GameItemElement{}
}
