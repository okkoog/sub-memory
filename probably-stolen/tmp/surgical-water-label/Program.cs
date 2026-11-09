using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

class Program {
  const string WaterType = "ProbablyStolenKoreanPack.WaterNameCompatibility";
  static void Main(string[] args) {
    var original = ModuleDefMD.Load(args[0]);
    var modified = ModuleDefMD.Load(args[1]);
    var target = original.GetTypes().First(t => t.FullName == WaterType);
    var source = modified.GetTypes().First(t => t.FullName == WaterType);
    var imp = new Importer(original);

    var fields = new Dictionary<string, FieldDef>();
    foreach (var f in target.Fields) fields[f.Name.String] = f;
    foreach (var f in source.Fields) {
      if (fields.ContainsKey(f.Name.String)) continue;
      var dest = new FieldDefUser(f.Name, imp.Import(f.FieldSig), f.Attributes);
      target.Fields.Add(dest);
      fields[f.Name.String] = dest;
    }

    var methods = new Dictionary<string, MethodDef>();
    foreach (var m in target.Methods) methods[m.Name.String] = m;
    string[] names = { ".cctor", "Enable", "Disable", "Repair",
                       "BeforeWaterTooltip", "RestoreWaterFeatureAfterMods" };
    foreach (var n in names) {
      var s = source.Methods.FirstOrDefault(m => m.Name == n);
      if (s == null) throw new Exception("Patch source method not found: " + n);
      if (!methods.ContainsKey(n)) {
        var d = new MethodDefUser(s.Name, imp.Import(s.MethodSig), s.ImplAttributes, s.Attributes);
        target.Methods.Add(d);
        methods[n] = d;
      }
    }

    object MapRef(object obj) {
      if (obj is IField field) {
        if (field.DeclaringType?.FullName == WaterType) {
          if (!fields.TryGetValue(field.Name.String, out FieldDef? d))
            throw new Exception("Missing water field " + field.Name);
          return d;
        }
        return imp.Import(field);
      }
      if (obj is IMethod method) {
        if (method.DeclaringType?.FullName == WaterType) {
          if (!methods.TryGetValue(method.Name.String, out MethodDef? d))
            throw new Exception("Missing water method " + method.Name);
          return d;
        }
        return imp.Import(method);
      }
      if (obj is ITypeDefOrRef tr) {
        if (tr.FullName == WaterType) return target;
        return imp.Import(tr);
      }
      if (obj is TypeSig ts) return imp.Import(ts);
      if (obj is MethodSig ms) return imp.Import(ms);
      if (obj is FieldSig fs) return imp.Import(fs);
      if (obj is CallSite cs) return imp.Import(cs);
      return obj;
    }

    foreach (string n in names) {
      MethodDef s = source.Methods.First(m => m.Name == n);
      MethodDef d = methods[n];
      if (!s.HasBody) throw new Exception("No method body: " + n);
      var sb = s.Body!;
      var db = new CilBody { InitLocals = sb.InitLocals, MaxStack = sb.MaxStack, KeepOldMaxStack = false };
      var lm = new Dictionary<Local, Local>();
      foreach (var sl in sb.Variables) {
        var dl = new Local(imp.Import(sl.Type));
        db.Variables.Add(dl);
        lm[sl] = dl;
      }
      var im = new Dictionary<Instruction, Instruction>();
      foreach (var si in sb.Instructions) {
        var di = Instruction.Create(OpCodes.Nop);
        di.OpCode = si.OpCode;
        db.Instructions.Add(di);
        im[si] = di;
      }
      for (int i=0; i<sb.Instructions.Count; i++) {
        object? v = sb.Instructions[i].Operand;
        if (v is Instruction branch) v = im[branch];
        else if (v is IList<Instruction> branches) v = branches.Select(x => im[x]).ToArray();
        else if (v is Local loc) v = lm[loc];
        else if (v is Parameter p) {
          var index = s.Parameters.IndexOf(p);
          v = d.Parameters[index];
        }
        else if (v != null) v = MapRef(v);
        db.Instructions[i].Operand = v;
      }
      foreach (var eh in sb.ExceptionHandlers) {
        db.ExceptionHandlers.Add(new ExceptionHandler(eh.HandlerType) {
          TryStart = eh.TryStart == null ? null : im[eh.TryStart],
          TryEnd = eh.TryEnd == null ? null : im[eh.TryEnd],
          HandlerStart = eh.HandlerStart == null ? null : im[eh.HandlerStart],
          HandlerEnd = eh.HandlerEnd == null ? null : im[eh.HandlerEnd],
          FilterStart = eh.FilterStart == null ? null : im[eh.FilterStart],
          CatchType = eh.CatchType == null ? null : (ITypeDefOrRef)MapRef(eh.CatchType)
        });
      }
      db.SimplifyBranches();
      db.OptimizeBranches();
      d.Body = db;
      Console.WriteLine("Applied only WaterNameCompatibility." + n);
    }
    // Preserve the old DLL's KoreanPack and all other original types/methods untouched.
    original.Write(args[2]);
    Console.WriteLine("Wrote surgically patched DLL: " + args[2]);
  }
}
