# Brood five stage strings: real IL callsites (2026-11-22)

DLL: `probably-stolen/local-mods/TentacleMod번역/lewdMemeBrood v0331/LewdMemeBrood.dll`

## BroodMainCycle.TentacleChild.TentacleUse/<>c :: System.Void BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_0()
- target: `萝莉 → 少女`
- method instructions: 266
```cil
       01F4 newobj System.Void System.Func`2<System.Single,System.Boolean>::.ctor(System.Object,System.IntPtr)
       01F9 dup 
       01FA stsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_5
       01FF call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
       0204 brfalse.s IL_021D: ldc.i4.0
       0206 ldloc.1 
       0207 ldstr MAIDEN_BODY
       020C call System.String BroodMainCycle.Tags::GetStr(GameItem,System.String)
       0211 ldstr 少女
       0216 call System.Boolean System.String::op_Equality(System.String,System.String)
       021B br.s IL_021E: ldstr "萝莉 → 少女"
       021D ldc.i4.0 
TARGET 021E ldstr 萝莉 → 少女
       0223 call System.Void BroodMainCycle.Check::That(System.Boolean,System.String)
       0228 ldloc.1 
       0229 ldsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_6
       022E dup 
       022F brtrue.s IL_0248: call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
       0231 pop 
       0232 ldsfld BroodMainCycle.TentacleChild.TentacleUse/<>c BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9
       0237 ldftn System.Boolean BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_6(System.Single)
       023D newobj System.Void System.Func`2<System.Single,System.Boolean>::.ctor(System.Object,System.IntPtr)
       0242 dup 
       0243 stsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_6
       0248 call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
```

## BroodMainCycle.TentacleChild.TentacleUse/<>c :: System.Void BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_0()
- target: `少女 → 御姐`
- method instructions: 266
```cil
       023D newobj System.Void System.Func`2<System.Single,System.Boolean>::.ctor(System.Object,System.IntPtr)
       0242 dup 
       0243 stsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_6
       0248 call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
       024D brfalse.s IL_0266: ldc.i4.0
       024F ldloc.1 
       0250 ldstr MAIDEN_BODY
       0255 call System.String BroodMainCycle.Tags::GetStr(GameItem,System.String)
       025A ldstr 御姐
       025F call System.Boolean System.String::op_Equality(System.String,System.String)
       0264 br.s IL_0267: ldstr "少女 → 御姐"
       0266 ldc.i4.0 
TARGET 0267 ldstr 少女 → 御姐
       026C call System.Void BroodMainCycle.Check::That(System.Boolean,System.String)
       0271 ldloc.1 
       0272 ldsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_7
       0277 dup 
       0278 brtrue.s IL_0291: call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
       027A pop 
       027B ldsfld BroodMainCycle.TentacleChild.TentacleUse/<>c BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9
       0280 ldftn System.Boolean BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_7(System.Single)
       0286 newobj System.Void System.Func`2<System.Single,System.Boolean>::.ctor(System.Object,System.IntPtr)
       028B dup 
       028C stsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_7
       0291 call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
```

## BroodMainCycle.TentacleChild.TentacleUse/<>c :: System.Void BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_0()
- target: `御姐 → 少妇`
- method instructions: 266
```cil
       0286 newobj System.Void System.Func`2<System.Single,System.Boolean>::.ctor(System.Object,System.IntPtr)
       028B dup 
       028C stsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_7
       0291 call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
       0296 brfalse.s IL_02AF: ldc.i4.0
       0298 ldloc.1 
       0299 ldstr MAIDEN_BODY
       029E call System.String BroodMainCycle.Tags::GetStr(GameItem,System.String)
       02A3 ldstr 少妇
       02A8 call System.Boolean System.String::op_Equality(System.String,System.String)
       02AD br.s IL_02B0: ldstr "御姐 → 少妇"
       02AF ldc.i4.0 
TARGET 02B0 ldstr 御姐 → 少妇
       02B5 call System.Void BroodMainCycle.Check::That(System.Boolean,System.String)
       02BA ldloc.1 
       02BB ldsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_8
       02C0 dup 
       02C1 brtrue.s IL_02DA: call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
       02C3 pop 
       02C4 ldsfld BroodMainCycle.TentacleChild.TentacleUse/<>c BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9
       02C9 ldftn System.Boolean BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_8(System.Single)
       02CF newobj System.Void System.Func`2<System.Single,System.Boolean>::.ctor(System.Object,System.IntPtr)
       02D4 dup 
       02D5 stsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_8
       02DA call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
```

## BroodMainCycle.TentacleChild.TentacleUse/<>c :: System.Void BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_0()
- target: `少妇 → 熟女`
- method instructions: 266
```cil
       02CF newobj System.Void System.Func`2<System.Single,System.Boolean>::.ctor(System.Object,System.IntPtr)
       02D4 dup 
       02D5 stsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_8
       02DA call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
       02DF brfalse.s IL_02F8: ldc.i4.0
       02E1 ldloc.1 
       02E2 ldstr MAIDEN_BODY
       02E7 call System.String BroodMainCycle.Tags::GetStr(GameItem,System.String)
       02EC ldstr 熟女
       02F1 call System.Boolean System.String::op_Equality(System.String,System.String)
       02F6 br.s IL_02F9: ldstr "少妇 → 熟女"
       02F8 ldc.i4.0 
TARGET 02F9 ldstr 少妇 → 熟女
       02FE call System.Void BroodMainCycle.Check::That(System.Boolean,System.String)
       0303 ldloc.1 
       0304 ldsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_9
       0309 dup 
       030A brtrue.s IL_0323: call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
       030C pop 
       030D ldsfld BroodMainCycle.TentacleChild.TentacleUse/<>c BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9
       0312 ldftn System.Boolean BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_9(System.Single)
       0318 newobj System.Void System.Func`2<System.Single,System.Boolean>::.ctor(System.Object,System.IntPtr)
       031D dup 
       031E stsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_9
       0323 call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
```

## BroodMainCycle.TentacleChild.TentacleUse/<>c :: System.Void BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_0()
- target: `熟女为止`
- method instructions: 266
```cil
       0318 newobj System.Void System.Func`2<System.Single,System.Boolean>::.ctor(System.Object,System.IntPtr)
       031D dup 
       031E stsfld System.Func`2<System.Single,System.Boolean> BroodMainCycle.TentacleChild.TentacleUse/<>c::<>9__8_9
       0323 call System.String BroodMainCycle.TentacleChild.TentacleUse::TryShiftBody(GameItem,System.Func`2<System.Single,System.Boolean>)
       0328 brtrue.s IL_0341: ldc.i4.0
       032A ldloc.1 
       032B ldstr MAIDEN_BODY
       0330 call System.String BroodMainCycle.Tags::GetStr(GameItem,System.String)
       0335 ldstr 熟女
       033A call System.Boolean System.String::op_Equality(System.String,System.String)
       033F br.s IL_0342: ldstr "熟女为止"
       0341 ldc.i4.0 
TARGET 0342 ldstr 熟女为止
       0347 call System.Void BroodMainCycle.Check::That(System.Boolean,System.String)
       034C leave.s IL_0358: ret
       034E ldloc.0 
       034F brfalse.s IL_0357: endfinally
       0351 ldloc.0 
       0352 callvirt System.Void System.IDisposable::Dispose()
       0357 endfinally 
       0358 ret 
```

Found string references: 5
Caution: this document is a static audit and does not certify runtime behavior.
