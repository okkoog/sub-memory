using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

class P {
    static void Need(bool b,string s){if(!b)throw new Exception(s);}
    static string Hash(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant();
    static string Txt(JsonElement e,string k)=>e.GetProperty(k).GetString()!;
    static int Num(JsonElement e,string k)=>e.GetProperty(k).GetInt32();
    static string OpName(Instruction i)=>i.Operand is Instruction&&i.OpCode.Name.EndsWith(".s")?i.OpCode.Name[..^2]:i.OpCode.Name;
    static string Op(object? o,Dictionary<Instruction,int> index)=>o switch{
       null=>"",Instruction x=>"@"+index[x],IList<Instruction> a=>string.Join(",",a.Select(x=>"@"+index[x])),_=>o.ToString()??""
    };
    static Dictionary<Instruction,int> Index(IList<Instruction> il)=>il.Select((x,i)=>(x,i)).ToDictionary(x=>x.x,x=>x.i);
    static string EH(ExceptionHandler h,Dictionary<Instruction,int> ix){
       string I(Instruction? v)=>v==null?"-":ix[v].ToString();
       return string.Join("|",h.HandlerType.ToString(),I(h.TryStart),I(h.TryEnd),I(h.HandlerStart),I(h.HandlerEnd),I(h.FilterStart),h.CatchType?.FullName??"");
    }
    static IEnumerable<MethodDef> Methods(ModuleDefMD m)=>m.GetTypes().SelectMany(t=>t.Methods);
    static MethodDef Method(ModuleDefMD m,string name)=>Methods(m).Single(x=>x.FullName==name);
    static string Canon(string s)=>Regex.Replace(Regex.Replace(s,@"DisplayClass\d+_","DisplayClass#_"),@"b__\d+_","b__#_");
    static string Shape(Instruction i,bool ignoreText=false,bool compiler=false){
       string op=OpName(i),v=i.Operand switch{null=>"",Instruction=>"@",IList<Instruction>=>"@switch",_=>i.Operand.ToString()??""};
       if(ignoreText&&i.OpCode==OpCodes.Ldstr)v="<literal>";
       if(compiler)v=Canon(Regex.Replace(v,@"<>9__\d+_","<>9__#_"));
       return op+"|"+v;
    }
    static bool Text(string s)=>s.Any(c=>c>='\u3400'&&c<='\u9fff'||c>='\uac00'&&c<='\ud7a3');
    static bool Korean(string s)=>s.Any(c=>c>='\uac00'&&c<='\ud7a3');
    static string Reuse(ModuleDefMD old,ModuleDefMD donor,string method,string source){
       var found=new List<(string method,string target)>();
       var canon=Canon(method);
       foreach(var o in Methods(old).Where(m=>m.HasBody&&Canon(m.FullName)==canon)){
          var d=Methods(donor).SingleOrDefault(m=>m.FullName==o.FullName&&m.HasBody);
          if(d==null)continue;
          var a=o.Body.Instructions;var b=d.Body.Instructions;
          if(a.Count!=b.Count||!a.Select(i=>Shape(i,true)).SequenceEqual(b.Select(i=>Shape(i,true))))continue;
          for(int i=0;i<a.Count;i++)if(a[i].OpCode==OpCodes.Ldstr&&Equals(a[i].Operand,source)&&b[i].Operand is string t&&t!=source&&Korean(t))found.Add((o.FullName,t));
       }
       // An unchanged source string can move between compiler-generated closures; only
       // accept the same logical parent method, exact original source, and unique prior translation.
       if(found.Count==0){
          string name=method.Split("::",2)[1].Split('(')[0];
          string parent=name.Contains('<')&&name.Contains('>')?name[(name.IndexOf('<')+1)..name.IndexOf('>')]:name;
          foreach(var o in Methods(old).Where(m=>m.HasBody&&m.FullName.Contains(parent))){
             var d=Methods(donor).SingleOrDefault(m=>m.FullName==o.FullName&&m.HasBody);if(d==null)continue;
             var a=o.Body.Instructions;var b=d.Body.Instructions;
             if(a.Count!=b.Count||!a.Select(i=>Shape(i,true)).SequenceEqual(b.Select(i=>Shape(i,true))))continue;
             for(int i=0;i<a.Count;i++)if(a[i].OpCode==OpCodes.Ldstr&&Equals(a[i].Operand,source)&&b[i].Operand is string t&&t!=source&&Korean(t))found.Add((o.FullName,t));
          }
       }
       var options=found.Select(x=>x.target).Distinct().ToArray();
       Need(options.Length==1,"Prior exact translation absent or ambiguous in "+method+" (length "+source.Length+", matches "+options.Length+")");
       return options[0];
    }
    static int FindBoundary(IList<Instruction> old,IList<Instruction> now,int pos,string why){
       var a=old.Select(i=>Shape(i,false,true)).ToArray();var b=now.Select(i=>Shape(i,false,true)).ToArray();
       if(old.Count==now.Count&&old.Select(i=>Shape(i,true,true)).SequenceEqual(now.Select(i=>Shape(i,true,true))))return pos;
       var scored=new List<(int idx,int score,int left,int right)>();
       for(int j=0;j<=b.Length;j++){
          int l=0,r=0;
          while(l<12&&pos-l-1>=0&&j-l-1>=0&&a[pos-l-1]==b[j-l-1])l++;
          while(r<12&&pos+r<a.Length&&j+r<b.Length&&a[pos+r]==b[j+r])r++;
          if(l>=Math.Min(2,pos)&&r>=Math.Min(2,a.Length-pos)&&l+r>=5)scored.Add((j,l+r,l,r));
       }
       var max=scored.Count==0?-1:scored.Max(x=>x.score);
       var best=scored.Where(x=>x.score==max).ToArray();
       Need(best.Length==1,"Boundary requires further review "+why+" old index "+pos+" best="+best.Length+" score="+max);
       return best[0].idx;
    }
    static void Main(string[] a){
       // mod, old original, old Korean, new original, transfer json, manual json, output DLL, report
       string mod=a[0];bool mp=mod=="MidnightPawn";
       byte[] raw=File.ReadAllBytes(a[3]);
       using var old=ModuleDefMD.Load(a[1]);using var donor=ModuleDefMD.Load(a[2]);using var orig=ModuleDefMD.Load(raw);using var target=ModuleDefMD.Load(raw);
       using var plan=JsonDocument.Parse(File.ReadAllText(a[4]));using var manual=JsonDocument.Parse(File.ReadAllText(a[5]));
       Need(Hash(raw)==Txt(plan.RootElement,"baselineSHA"),"New original hash mismatch");
       var origMethods=Methods(orig).GroupBy(m=>m.FullName).ToDictionary(g=>g.Key,g=>g.OrderBy(x=>x.MDToken.Raw).ToArray());
       var patched=new Dictionary<(string,int),string>();
       var protectedManual=new List<object>();
       var reused=new List<object>();var evidence=new List<object>();
       var originalTextCount=Methods(orig).Where(m=>m.HasBody).Sum(m=>m.Body.Instructions.Count(i=>i.OpCode==OpCodes.Ldstr&&i.Operand is string s&&Text(s)));
       var sites=plan.RootElement.GetProperty("sites").EnumerateArray().ToArray();
       var manualSites=manual.RootElement.EnumerateArray().ToArray();
       Need(originalTextCount==sites.Length+manualSites.Length+Num(plan.RootElement,"protectedUnchanged")+Num(plan.RootElement,"protectedMatched"),"Inventory does not cover original text sites");
       var allSites=new HashSet<(string,int)>();
       // Resolve every site against pristine new binary before any insertion.
       void Literal(JsonElement e,bool isManual){
          string name=Txt(e,"method"),source=Txt(e,"source");int index=Num(e,"index");var m=Method(target,name);
          Need(allSites.Add((name,index)),"Duplicate "+name+" @"+index);
          Need(m.MDToken.Raw.ToString("X8")==Txt(e,"token"),"Token mismatch "+name);
          var i=m.Body.Instructions[index];Need(i.OpCode==OpCodes.Ldstr&&Equals(i.Operand,source)&&i.Offset==(uint)Num(e,"offset"),"Literal anchor "+name+" @"+index);
          string action=isManual?Txt(e,"action"):"patch";
          if(action=="protect"){protectedManual.Add(new{method=name,index,reason=Txt(e,"reason")});return;}
          string value=action=="reuse"?Reuse(old,donor,name,source):Txt(e,"target");
          // 2.2.0 corrected document item name; keep the two distinct document names.
          if(mp&&source=="触手档案"&&(name.Contains("TentacleArchive::Create")||name.Contains("Codex::RegisterOwn")||name.Contains("Perks::OnNewGameStarted")))value="촉수 기록부";
          if(mp&&source=="生成触手档案"&&name.Contains("RegisterDebugButtons"))value="촉수 기록부 생성";
          if(mp&&source=="打开触手档案失败: "&&name.Contains("ArchivePaper::Open"))value="촉수 기록부 열기 실패: ";
          Need(value!=source,"Patch with unchanged value "+name+" @"+index);
          // Validate composite format and cfg placeholders; no placeholder deletion or invention.
          string[] ph(string s)=>Regex.Matches(s,@"\{(?:\d+(?::[^{}]+)?|cfg:[^{}]+)\}").Select(x=>x.Value).OrderBy(x=>x).ToArray();
          Need(ph(source).SequenceEqual(ph(value)),"Placeholder drift "+name+" @"+index);
          patched.Add((name,index),value);
          if(action=="reuse")reused.Add(new{method=name,index,sourceLength=source.Length,targetLength=value.Length});
          evidence.Add(new{method=name,index,action});
       }
       foreach(var e in sites)Literal(e,false);
       foreach(var e in manualSites)Literal(e,true);
       // Clone only the previously verified localisation helpers from the donor into the new original.
       var sourceOwner=donor.GetTypes().Single(t=>t.FullName==(mp?"MidnightPawn.ModConst":"LewdMemeConfig.ConfigDisplayKo"));
       TypeDef owner;
       var helperSource=sourceOwner.Methods.Where(m=>mp?(m.Name=="KoreanDisplayValue"||m.Name=="LocalizedOwnTypeDisplay"):true).ToArray();
       Need(helperSource.Length==(mp?2:7)&&helperSource.All(m=>m.IsStatic&&m.HasBody&&!m.HasGenericParameters),"Helper donor shape");
       if(mp)owner=target.GetTypes().Single(t=>t.FullName==sourceOwner.FullName);
       else {Need(!target.GetTypes().Any(t=>t.FullName==sourceOwner.FullName),"Helper type already exists");owner=new TypeDefUser(sourceOwner.Namespace,sourceOwner.Name,target.CorLibTypes.Object.TypeDefOrRef){Attributes=sourceOwner.Attributes};target.Types.Add(owner);}
       var importer=new Importer(target,ImporterOptions.TryToUseTypeDefs);
       IMethod? actualTarget=null;
       if(!mp)actualTarget=(IMethod)target.GetTypes().Single(t=>t.FullName=="LewdMemeConfig.PanelRowPatch").Methods.Single(m=>m.Name=="Install").Body.Instructions.Single(i=>i.Operand is IMethod mr&&mr.FullName.Contains("ModReflect::Target")).Operand;
       var hmap=new Dictionary<MethodDef,MethodDef>();
       foreach(var d in helperSource){
          Need(!owner.Methods.Any(x=>x.Name==d.Name),"Helper already exists "+d.Name);
          var m=new MethodDefUser(d.Name,importer.Import(d.MethodSig),d.ImplAttributes,d.Attributes);
          owner.Methods.Add(m);hmap.Add(d,m);
       }
       object? ImportOperand(object? o,MethodDef dst,Dictionary<Instruction,Instruction> ins,Dictionary<Local,Local> loc)=>o switch{
          null=>null,Instruction x=>ins[x],IList<Instruction> seq=>seq.Select(x=>ins[x]).ToArray(),Local l=>loc[l],Parameter p=>dst.Parameters[p.Index],
          MethodDef d when hmap.ContainsKey(d)=>hmap[d],
          IMethod m when m.DeclaringType.FullName==sourceOwner.FullName&&hmap.Keys.Any(x=>x.FullName==m.FullName)=>hmap.First(x=>x.Key.FullName==m.FullName).Value,
          ITypeDefOrRef t when t.FullName==sourceOwner.FullName=>owner,
          IMethod m when !mp&&m.DeclaringType.FullName=="ModReflect"&&m.Name=="Target"=>actualTarget,
          IMethod m=>importer.Import(m),IField f=>importer.Import(f),ITypeDefOrRef t=>importer.Import(t),MethodSig s=>importer.Import(s),_=>o
       };
       foreach(var d in helperSource){
          var dst=hmap[d];var b=d.Body;var body=new CilBody{InitLocals=b.InitLocals,MaxStack=b.MaxStack};dst.Body=body;
          var loc=new Dictionary<Local,Local>();foreach(var l in b.Variables){var z=new Local(importer.Import(l.Type));loc.Add(l,z);body.Variables.Add(z);}
          var ins=b.Instructions.ToDictionary(i=>i,i=>new Instruction(i.OpCode));
          foreach(var i in b.Instructions){var n=ins[i];n.Operand=ImportOperand(i.Operand,dst,ins,loc);body.Instructions.Add(n);}
          Instruction? At(Instruction? i)=>i==null?null:ins[i];
          foreach(var e in b.ExceptionHandlers)body.ExceptionHandlers.Add(new ExceptionHandler(e.HandlerType){TryStart=At(e.TryStart),TryEnd=At(e.TryEnd),HandlerStart=At(e.HandlerStart),HandlerEnd=At(e.HandlerEnd),FilterStart=At(e.FilterStart),CatchType=e.CatchType==null?null:importer.Import(e.CatchType)});
       }
       var translate=owner.Methods.Single(m=>m.Name==(mp?"KoreanDisplayValue":"Translate"));
       var extra=mp?new Dictionary<string,string>{{"访客","방문객"},{"战士","병사"}}:new Dictionary<string,string>{
          {"主 mod 官方设置(拾荒)","본편 공식 설정(폐품 수집)"},{"夜间结算","야간 정산"},{"特殊顾客与名字","특수 고객과 이름"},{"侵蚀触手","침식 촉수"},{"打印机","프린터"}};
       var eq=(IMethod)translate.Body.Instructions.First(i=>i.Operand is IMethod m&&m.FullName=="System.Boolean System.String::op_Equality(System.String,System.String)").Operand;
       foreach(var kv in extra.Reverse()){
          if(translate.Body.Instructions.Any(i=>i.OpCode==OpCodes.Ldstr&&Equals(i.Operand,kv.Key)))continue;
          var first=translate.Body.Instructions[0];var seq=new[]{Instruction.Create(OpCodes.Ldarg_0),Instruction.Create(OpCodes.Ldstr,kv.Key),Instruction.Create(OpCodes.Call,eq),Instruction.Create(OpCodes.Brfalse,first),Instruction.Create(OpCodes.Ldstr,kv.Value),Instruction.Create(OpCodes.Ret)};
          for(int q=0;q<seq.Length;q++)translate.Body.Instructions.Insert(q,seq[q]);
       }
       translate.Body.SimplifyBranches();translate.Body.OptimizeBranches();
       // Derive the previous scoped helper insertions from original vs Korean IL, then locate
       // the same output boundary in the new method using matching surrounding code.
       var insertions=new Dictionary<string,SortedDictionary<int,List<Instruction>>>();
       var boundaryReport=new List<object>();
       int previousCalls=0;
       foreach(var d in Methods(donor).Where(m=>m.HasBody&&!helperSource.Contains(m))){
          var din=d.Body.Instructions;
          var calls=din.Select((i,n)=>(i,n)).Where(x=>x.i.Operand is IMethod m&&m.DeclaringType.FullName==sourceOwner.FullName&&hmap.Keys.Any(k=>k.FullName==m.FullName)).ToArray();
          if(calls.Length==0)continue;
          var om=Methods(old).SingleOrDefault(m=>m.FullName==d.FullName&&m.HasBody);var nm=Methods(target).SingleOrDefault(m=>m.FullName==d.FullName&&m.HasBody);
          Need(om!=null&&nm!=null,"Output method no longer exists "+d.FullName);
          var removed=new HashSet<int>();var spans=new List<(int start,int count,string helper)>();
          foreach(var (i,n) in calls){
             var h=(IMethod)i.Operand;
             int count=h.Name.String switch{"KoreanDisplayValue"=>1,"LocalizedOwnTypeDisplay"=>2,"Install"=>3,"Choices"=>2,_=>throw new Exception("Unexpected helper caller "+h.FullName)};
             int start=n-count+1;
             Need(start>=0,"Invalid donor helper sequence");
             for(int q=start;q<=n;q++)Need(removed.Add(q),"Overlapping helper inserts");
             spans.Add((start,count,h.Name.String));previousCalls++;
          }
          var filtered=din.Where((x,n)=>!removed.Contains(n)).ToArray();
          var oi=om!.Body.Instructions;var ni=nm!.Body.Instructions;
          Need(oi.Count==filtered.Length&&oi.Select(x=>Shape(x,true)).SequenceEqual(filtered.Select(x=>Shape(x,true))),"Donor retained non-display changes "+d.FullName);
          var byPos=new SortedDictionary<int,List<Instruction>>();
          foreach(var (start,count,helper) in spans){
             int oldpos=Enumerable.Range(0,start).Count(x=>!removed.Contains(x));
             int pos=FindBoundary(oi,ni,oldpos,d.FullName+" "+helper);
             Need(!byPos.ContainsKey(pos),"Duplicate output boundary "+d.FullName);
             var method=owner.Methods.Single(m=>m.Name==helper);
             var seq=new List<Instruction>();
             if(helper=="LocalizedOwnTypeDisplay"){
                Need(din[start].OpCode==OpCodes.Ldloc_0&&nm.Body.Variables.Count>0&&nm.Body.Variables[0].Type.FullName=="System.String","Type display argument");
                seq.Add(Instruction.Create(OpCodes.Ldloc_0));
             }else if(helper=="Install"){
                Need(din[start].OpCode==OpCodes.Ldsfld&&din[start+1].OpCode==OpCodes.Ldarg_0,"Install donor sequence");
                seq.Add(Instruction.Create(OpCodes.Ldsfld,nm.DeclaringType.Fields.Single(f=>f.Name=="harmony")));
                seq.Add(Instruction.Create(OpCodes.Ldarg_0));
             }else if(helper=="Choices"){
                Need(din[start].OpCode==OpCodes.Ldarg_1,"Choices donor sequence");seq.Add(Instruction.Create(OpCodes.Ldarg_1));
             }
             seq.Add(Instruction.Create(OpCodes.Call,method));byPos.Add(pos,seq);
             boundaryReport.Add(new{method=d.FullName,helper,oldIndex=oldpos,newIndex=pos});
          }
          insertions.Add(d.FullName,byPos);
       }
       Need(previousCalls==(mp?18:2),"Unexpected previous helper call count "+previousCalls);
       var numeric=new Dictionary<(string,int),int>();
       if(mp){
          var m=Methods(target).Single(m=>m.DeclaringType.FullName=="MidnightPawn.Game.NameGen"&&m.Name=="IsCjk");
          var i=m.Body.Instructions.Single(x=>x.OpCode==OpCodes.Ldc_I4&&Equals(x.Operand,40959));
          int index=m.Body.Instructions.IndexOf(i);
          var prior=Method(donor,m.FullName);Need(prior.Body.Instructions.Count(x=>x.OpCode==OpCodes.Ldc_I4&&Equals(x.Operand,55203))==1,"Prior Hangul name formatting evidence");
          numeric.Add((m.FullName,index),55203);
       }
       // Apply only the reviewed string positions, and only the previously scoped code insertions.
       foreach(var kv in patched)Method(target,kv.Key.Item1).Body.Instructions[kv.Key.Item2].Operand=kv.Value;
       foreach(var kv in numeric)Method(target,kv.Key.Item1).Body.Instructions[kv.Key.Item2].Operand=kv.Value;
       foreach(var pair in insertions){
          var m=Method(target,pair.Key);
          foreach(var p in pair.Value.Reverse())for(int j=0;j<p.Value.Count;j++)m.Body.Instructions.Insert(p.Key+j,p.Value[j]);
          m.Body.SimplifyBranches();m.Body.OptimizeBranches();
       }
       // Source identity / categories are never globally rewritten. Confirm the newly added
       // identity and job methods keep their exact original instructions.
       Directory.CreateDirectory(Path.GetDirectoryName(a[6])!);target.Write(a[6]);
       using var read=ModuleDefMD.Load(a[6]);
       var finalMethods=Methods(read).GroupBy(m=>m.FullName).ToDictionary(g=>g.Key,g=>g.OrderBy(x=>x.MDToken.Raw).ToArray());
       Need(finalMethods.Values.Sum(x=>x.Length)==origMethods.Values.Sum(x=>x.Length)+helperSource.Length,"Method count drift");
       Need(origMethods.All(x=>finalMethods.TryGetValue(x.Key,out var v)&&v.Length==x.Value.Length),"Original method set drift");
       int checkedMethods=0,checkedLiterals=0,checkedExtra=0,checkedNumbers=0;
       foreach(var pair in origMethods)for(int z=0;z<pair.Value.Length;z++){
          var x=pair.Value[z];var y=finalMethods[pair.Key][z];
          Need(x.HasBody==y.HasBody,"Method body availability "+x.FullName);if(!x.HasBody)continue;
          var oi=x.Body.Instructions;var ni=y.Body.Instructions;var xm=Index(oi);
          insertions.TryGetValue(x.FullName,out var adds);
          int added=adds?.Values.Sum(v=>v.Count)??0;
          Need(ni.Count==oi.Count+added,"Instruction count "+x.FullName);
          var ym=new Dictionary<Instruction,int>();var matches=new List<(Instruction x,Instruction y)>();
          int at=0;
          for(int j=0;j<=oi.Count;j++){
             if(adds!=null&&adds.TryGetValue(j,out var extraIns)){
                foreach(var planned in extraIns){
                   var actual=ni[at++];Need(OpName(planned)==OpName(actual)&&planned.Operand?.ToString()==actual.Operand?.ToString(),"Inserted code drift "+x.FullName);
                   ym.Add(actual,j);checkedExtra++;
                }
             }
             if(j<oi.Count){var actual=ni[at++];ym.Add(actual,j);matches.Add((oi[j],actual));}
          }
          Need(at==ni.Count,"Position drift "+x.FullName);
          Need(x.Body.Variables.Select(l=>l.Type.FullName).SequenceEqual(y.Body.Variables.Select(l=>l.Type.FullName)),"Local types "+x.FullName);
          Need(x.Body.ExceptionHandlers.Select(e=>EH(e,xm)).SequenceEqual(y.Body.ExceptionHandlers.Select(e=>EH(e,ym))),"Exception regions "+x.FullName);
          for(int j=0;j<matches.Count;j++){
             var (u,v)=matches[j];var key=(x.FullName,j);
             Need(OpName(u)==OpName(v),"Opcode drift "+x.FullName+" @"+j);
             string expected=patched.TryGetValue(key,out var s)?s:numeric.TryGetValue(key,out var num)?num.ToString():Op(u.Operand,xm);
             Need(expected==Op(v.Operand,ym),"Operand drift "+x.FullName+" @"+j);
             if(patched.ContainsKey(key))checkedLiterals++;
             if(numeric.ContainsKey(key))checkedNumbers++;
          }
          checkedMethods++;
       }
       Need(checkedLiterals==patched.Count&&checkedNumbers==numeric.Count&&checkedExtra==insertions.Values.Sum(v=>v.Values.Sum(q=>q.Count)),"Verification coverage");
       var rt=orig.GetTypes().Select(t=>t.FullName).OrderBy(x=>x).ToArray();
       var kt=read.GetTypes().Select(t=>t.FullName).OrderBy(x=>x).ToArray();
       Need(kt.SequenceEqual((mp?rt:rt.Append(sourceOwner.FullName)).OrderBy(x=>x)),"Type set drift");
       var res1=orig.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.ToString(),x=>Hash(x.CreateReader().ToArray()));
       var res2=read.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.ToString(),x=>Hash(x.CreateReader().ToArray()));
       Need(res1.Count==res2.Count&&res1.All(x=>res2.TryGetValue(x.Key,out var v)&&v==x.Value),"Resource drift");
       var refs=orig.GetAssemblyRefs().Select(x=>x.FullName).ToHashSet();
       var newRefs=read.GetAssemblyRefs().Select(x=>x.FullName).ToHashSet();
       Need(newRefs.SetEquals(refs),"Assembly reference drift: "+string.Join(",",newRefs.Except(refs))+" / "+string.Join(",",refs.Except(newRefs)));
       // Verify emitted helper bodies against the exact constructed bodies, including jumps.
       foreach(var h in hmap.Values){
          var r=Method(read,h.FullName);var oi=h.Body.Instructions;var ni=r.Body.Instructions;
          Need(oi.Count==ni.Count,"Helper instruction count "+h.FullName);
          var om=Index(oi);var nm=Index(ni);
          for(int i=0;i<oi.Count;i++)Need(OpName(oi[i])==OpName(ni[i])&&Op(oi[i].Operand,om)==Op(ni[i].Operand,nm),"Helper operand drift "+h.FullName);
          Need(h.Body.Variables.Select(x=>x.Type.FullName).SequenceEqual(r.Body.Variables.Select(x=>x.Type.FullName)),"Helper locals");
          Need(h.Body.ExceptionHandlers.Select(e=>EH(e,om)).SequenceEqual(r.Body.ExceptionHandlers.Select(e=>EH(e,nm))),"Helper exception regions");
       }
       var report=new{mod,originalSHA=Hash(raw),translatedSHA=Hash(File.ReadAllBytes(a[6])),originalMethods=checkedMethods,reviewedTextSites=originalTextCount,changedTextSites=checkedLiterals,protectedTextSites=originalTextCount-checkedLiterals,restoredHelperMethods=helperSource.Length,restoredHelperCalls=previousCalls,insertedInstructions=checkedExtra,changedNumericSites=checkedNumbers,resourcesVerified=res1.Count,allOriginalInstructionsVerified=true,assemblyReferencesVerified=true,remainingUnreviewed=0,actualGameRuntimeTest=false,boundaryReport,reused};
       File.WriteAllText(a[7],JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true})+"\n");
       Console.WriteLine(JsonSerializer.Serialize(report));
    }
}
