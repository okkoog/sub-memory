using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
class Scan
{
  static void Main(string[] args)
  {
    using var stream = File.OpenRead(args[0]);
    using var pe = new PEReader(stream);
    var md = pe.GetMetadataReader();
    var keys = new HashSet<string>(new[] {
      "ModLoader", "ModHelper", "ModHook", "LoadedMod", "RichTextBuilder",
      "PaperUI", "PaperElement", "TextBuilder", "CustomUIManager",
      "CustomUIWindow", "GameItem", "ModItemDirectory", "DirectoryMaster",
      "ModSourceHelper", "StoreClient", "Dialogue", "Dialog",
    });
    foreach(var h in md.TypeDefinitions) {
      var d = md.GetTypeDefinition(h);
      var name = md.GetString(d.Name);
      bool wanted=keys.Contains(name)||name.Contains("RichText")||name.StartsWith("Paper")||
        name.Contains("Dialog")||name.Contains("Client")&&name.Contains("Dialogue");
      if(!wanted) continue;
      Console.WriteLine("## "+name);
      foreach(var e in d.GetFields()) {var x=md.GetFieldDefinition(e);Console.WriteLine("  F "+md.GetString(x.Name)+" "+x.Attributes); }
      foreach(var e in d.GetMethods()) {var x=md.GetMethodDefinition(e);Console.WriteLine("  M "+md.GetString(x.Name)+" "+x.Attributes); }
      foreach(var e in d.GetProperties()) {var x=md.GetPropertyDefinition(e);Console.WriteLine("  P "+md.GetString(x.Name)); }
      foreach(var e in d.GetEvents()) {var x=md.GetEventDefinition(e);Console.WriteLine("  E "+md.GetString(x.Name)); }
    }
  }
}