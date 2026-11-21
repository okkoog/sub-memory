using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Mono.Cecil;
using Mono.Cecil.Cil;
class BridgeBuild {
    static IEnumerable<TypeDefinition> Types(IEnumerable<TypeDefinition> types) {
        foreach (var type in types) { yield return type; foreach(var nested in Types(type.NestedTypes)) yield return nested; }
    }
    static string Field(string s) { return "\"" + s.Replace("\"","\"\"") + "\""; }
    static void Main(string[] args) {
        var resolver = new DefaultAssemblyResolver(); resolver.AddSearchDirectory(args[1]);
        var mod = ModuleDefinition.ReadModule(args[0],new ReaderParameters{AssemblyResolver=resolver});
        var template=ModuleDefinition.ReadModule("CsvLookup.dll",new ReaderParameters{AssemblyResolver=resolver});
        var orig=template.GetType("CsvLookup");
        var helper=new TypeDefinition("RedLightDistrict.Korean","CsvLookup",orig.Attributes,mod.TypeSystem.Object);
        var methods=Types(mod.Types).SelectMany(t=>t.Methods).Where(m=>m.HasBody).ToList();
        mod.Types.Add(helper);
        var owner=new FieldDefinition("owner",orig.Fields[0].Attributes,mod.ImportReference(orig.Fields[0].FieldType));helper.Fields.Add(owner);
        var src=orig.Methods.Single(m=>m.Name=="Read");
        var read=new MethodDefinition("Read",src.Attributes,mod.TypeSystem.String);helper.Methods.Add(read);
        foreach(var p in src.Parameters)read.Parameters.Add(new ParameterDefinition(p.Name,p.Attributes,mod.TypeSystem.String));
        read.Body.InitLocals=true;
        foreach(var v in src.Body.Variables)read.Body.Variables.Add(new VariableDefinition(mod.ImportReference(v.VariableType)));
        foreach(var i in src.Body.Instructions){
            var member=i.Operand as MemberReference;
            if(member!=null){
                if(member.DeclaringType==orig && member is FieldReference)i.Operand=owner;
                else if(member==orig)i.Operand=helper;
                else if(member is MethodReference)i.Operand=mod.ImportReference((MethodReference)member);
                else if(member is FieldReference)i.Operand=mod.ImportReference((FieldReference)member);
                else if(member is TypeReference)i.Operand=mod.ImportReference((TypeReference)member);
            } else if(i.Operand is VariableDefinition)i.Operand=read.Body.Variables[((VariableDefinition)i.Operand).Index];
            read.Body.Instructions.Add(i);
        }
        var keys=new HashSet<string>(); var sha=SHA256.Create();
        using(var csv=new StreamWriter("key-catalog.csv",false,new UTF8Encoding(true))){
            csv.WriteLine("key,method,IL_offset,original,next_operand");
            foreach(var m in methods){
                var occurrences=new Dictionary<string,int>();
                foreach(var i in m.Body.Instructions){
                    string s=i.Operand as string;
                    if(i.OpCode!=OpCodes.Ldstr || !s.Any(c=>c>=0x3400 && c<=0x9fff))continue;
                    int n;occurrences.TryGetValue(s,out n);occurrences[s]=n+1;
                    string key="rld_"+BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(m.FullName+"\0"+s+"\0"+n)),0,10).Replace("-","").ToLowerInvariant();
                    if(!keys.Add(key))throw new Exception("duplicate key");
                    csv.WriteLine(string.Join(",",Field(key),Field(m.FullName),i.Offset.ToString(),Field(s),Field(i.Next==null?"":Convert.ToString(i.Next.Operand))));
                    var getter=new MethodDefinition(key,MethodAttributes.Assembly|MethodAttributes.Static|MethodAttributes.HideBySig,mod.TypeSystem.String);
                    helper.Methods.Add(getter);var il=getter.Body.GetILProcessor();il.Emit(OpCodes.Ldstr,key);il.Emit(OpCodes.Ldstr,s);il.Emit(OpCodes.Call,read);il.Emit(OpCodes.Ret);
                    i.OpCode=OpCodes.Call;i.Operand=getter;
                }
            }
        }
        mod.Write(args[2]);Console.WriteLine(keys.Count);
    }
}
