// Isolated diagnostic color labels only. No other text or functionality is modified.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

class Program {
    record Site(uint Offset, string Old, string New);
    static void Require(bool condition, string message) {
        if (!condition) throw new InvalidOperationException(message);
    }
    static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
    static string Operand(object? operand, Dictionary<Instruction,int> indices) => operand switch {
        null => "",
        Instruction target => "@" + indices[target],
        IList<Instruction> targets => string.Join(",",targets.Select(t=>"@"+indices[t])),
        _ => operand.ToString() ?? ""
    };
    static void Main(string[] args) {
        Require(args.Length==3,"Arguments: input DLL, separate output DLL, JSON report");
        string input=Path.GetFullPath(args[0]), output=Path.GetFullPath(args[1]);
        Require(input!=output && !File.Exists(output),"Output must be a fresh file");
        var inputHash=Hash(input);
        var sites=new[]{new Site(0x003D,"白","흰색"), new Site(0x0054,"紫","보라색")};
        using var before=ModuleDefMD.Load(input);
        using var edited=ModuleDefMD.Load(input);
        const uint token=0x0600004E;
        const string expected="System.Void BroodMainCycle.BodyActions/<>c::<RegisterTests>b__14_0()";
        var target=edited.GetTypes().SelectMany(t=>t.Methods).Single(m=>m.MDToken.Raw==token);
        Require(target.FullName==expected,"Method identity mismatch");
        var il=target.Body.Instructions;
        foreach(var s in sites) {
            var i=il.Single(x=>x.Offset==s.Offset);
            Require(i.OpCode==OpCodes.Ldstr && Equals(i.Operand,s.Old),"Original label mismatch");
            int n=il.IndexOf(i);
            Require(n+2<il.Count && il[n+1].OpCode==OpCodes.Ldc_R4 &&
               (il[n+2].OpCode==OpCodes.Call || il[n+2].OpCode==OpCodes.Callvirt) &&
               il[n+2].Operand is IMethod consumer &&
               consumer.FullName=="System.Void BroodMainCycle.Check::Near(System.Single,System.Single,System.String,System.Single)",
               "Diagnostic argument sink mismatch");
            i.Operand=s.New;
        }
        edited.Write(output);
        using var after=ModuleDefMD.Load(output);
        var a=before.GetTypes().SelectMany(t=>t.Methods).ToDictionary(m=>m.FullName);
        var b=after.GetTypes().SelectMany(t=>t.Methods).ToDictionary(m=>m.FullName);
        Require(a.Keys.ToHashSet().SetEquals(b.Keys),"Method identity mismatch");
        int checkedMethods=0, changes=0;
        foreach(var kv in a) {
            var x=kv.Value;var y=b[kv.Key];
            Require(x.HasBody==y.HasBody,"Body mismatch");
            if(!x.HasBody) continue;
            var xi=x.Body.Instructions;var yi=y.Body.Instructions;
            Require(xi.Count==yi.Count,"Instruction count mismatch");
            var xm=xi.Select((i,n)=>(i,n)).ToDictionary(t=>t.i,t=>t.n);
            var ym=yi.Select((i,n)=>(i,n)).ToDictionary(t=>t.i,t=>t.n);
            for(int n=0;n<xi.Count;n++) {
                Require(xi[n].OpCode==yi[n].OpCode,"Opcode mismatch");
                var site=x.FullName==expected?sites.SingleOrDefault(s=>s.Offset==xi[n].Offset):null;
                if(site!=null) {
                    Require(Equals(xi[n].Operand,site.Old)&&Equals(yi[n].Operand,site.New),"Label mismatch");
                    changes++;
                } else Require(Operand(xi[n].Operand,xm)==Operand(yi[n].Operand,ym),"Unexpected operand change");
            }
            Require(x.Body.Variables.Select(v=>v.Type.FullName).SequenceEqual(y.Body.Variables.Select(v=>v.Type.FullName)),"Local variable mismatch");
            string EH(ExceptionHandler e,Dictionary<Instruction,int> map) {
                string At(Instruction? i)=>i==null?"-":map[i].ToString();
                return string.Join("|",e.HandlerType.ToString(),At(e.TryStart),At(e.TryEnd),At(e.HandlerStart),At(e.HandlerEnd),At(e.FilterStart),e.CatchType?.FullName??"");
            }
            Require(x.Body.ExceptionHandlers.Select(e=>EH(e,xm)).SequenceEqual(y.Body.ExceptionHandlers.Select(e=>EH(e,ym))),"Exception handler mismatch");
            checkedMethods++;
        }
        Require(changes==2 && Hash(input)==inputHash,"Unexpected change count or input write");
        Require(before.Resources.Count==after.Resources.Count,"Resource count mismatch");
        var oldRes=before.Resources.OfType<EmbeddedResource>().ToDictionary(e=>e.Name.ToString(),e=>Convert.ToHexString(SHA256.HashData(e.CreateReader().ToArray())));
        var newRes=after.Resources.OfType<EmbeddedResource>().ToDictionary(e=>e.Name.ToString(),e=>Convert.ToHexString(SHA256.HashData(e.CreateReader().ToArray())));
        Require(oldRes.Count==newRes.Count&&oldRes.All(kv=>newRes.GetValueOrDefault(kv.Key)==kv.Value),"Resource mismatch");
        File.WriteAllText(args[2],JsonSerializer.Serialize(new{inputHash,temporaryOutputHash=Hash(output),checkedMethods,changes,unchangedOriginal=true,scope="Two color diagnostics only",sites,actualGameRuntimeTest=false,fullBinaryPublished=false},new JsonSerializerOptions{WriteIndented=true,Encoder=System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping})+"\n");
    }
}
