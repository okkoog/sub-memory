using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
class Provider : ISignatureTypeProvider<string, object>
{
  public string GetArrayType(string t, ArrayShape s)=>t+"[]";
  public string GetByReferenceType(string t)=>t+"&";
  public string GetFunctionPointerType(MethodSignature<string> s)=>"function";
  public string GetGenericInstantiation(string t, ImmutableArray<string> p)=>t+"<"+string.Join(",",p)+">";
  public string GetGenericMethodParameter(object c,int i)=>"!!"+i;
  public string GetGenericTypeParameter(object c,int i)=>"!"+i;
  public string GetModifiedType(string m,string t,bool r)=>t;
  public string GetPinnedType(string t)=>t;
  public string GetPointerType(string t)=>t+"*";
  public string GetPrimitiveType(PrimitiveTypeCode t)=>t.ToString();
  public string GetSZArrayType(string t)=>t+"[]";
  public string GetTypeFromDefinition(MetadataReader r, TypeDefinitionHandle h,byte k)=>r.GetString(r.GetTypeDefinition(h).Name);
  public string GetTypeFromReference(MetadataReader r, TypeReferenceHandle h,byte k)=>r.GetString(r.GetTypeReference(h).Name);
  public string GetTypeFromSpecification(MetadataReader r, object c,TypeSpecificationHandle h,byte k)=>r.GetTypeSpecification(h).DecodeSignature(this,c);
}
class Scanner
{
  static void Main(string[] a)
  {
    using var f=File.OpenRead(a[0]);using var p=new PEReader(f);var r=p.GetMetadataReader();var d=new Provider();
    foreach(var h in r.TypeDefinitions){
      var t=r.GetTypeDefinition(h);var n=r.GetString(t.Name);
      if(n.Contains("Book")||n.Contains("Paper")||n.Contains("Binding")||n.Contains("Dialog")||n.Contains("Dialogue")||n.Contains("Perk")||n.Contains("Starting"))
        Console.WriteLine("TYPE "+n);
      if(n!="PaperUI" && n!="ModLoader" && n!="LoadedMod" && n!="RichTextBuilder"&&
         n!="DialogueHandler" && !n.Contains("Paper")&&!n.Contains("Book")&&!n.Contains("Binding")&&!n.Contains("Perk")&&!n.Contains("Starting") && n!="ModHelper")continue;
      Console.WriteLine("## "+n);
      foreach(var ph in t.GetProperties()){
        var x=r.GetPropertyDefinition(ph);var name=r.GetString(x.Name); try{
          var sig=x.DecodeSignature(d,null);Console.WriteLine("  P "+sig.ReturnType+" "+name);
        }catch(Exception){Console.WriteLine("  P ?? "+name);}
      }
      foreach(var fh in t.GetFields()){
        var x=r.GetFieldDefinition(fh);var name=r.GetString(x.Name);
        if(n=="PaperUI"&&!name.Contains("Data"))continue;
        try{Console.WriteLine("  F "+x.Attributes+" "+x.DecodeSignature(d,null)+" "+name);}
        catch(Exception){Console.WriteLine("  F ?? "+name);}
      }
      foreach(var mh in t.GetMethods()){
        var x=r.GetMethodDefinition(mh);var name=r.GetString(x.Name);
        if(name.StartsWith("get_")||name.StartsWith("set_")||name.StartsWith("add_")||name.StartsWith("remove_"))continue;
        if(n=="ModLoader" && !name.Contains("Mod") && !name.Contains("Load"))continue;
        try{var sig=x.DecodeSignature(d,null);Console.WriteLine("  M "+x.Attributes+" "+sig.ReturnType+" "+name+"("+string.Join(", ",sig.ParameterTypes)+")");}
        catch(Exception){Console.WriteLine("  M ?? "+name);}
      }
    }
  }
}