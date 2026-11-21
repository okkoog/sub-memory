# Tentacle external gameplay data and IL reference audit

Static original DLL callsites and nearby IL are listed here; lack of a literal match is not proof of non-use.

## MidnightPawn.dll
Methods with IL: 732
**System.Void MidnightPawn.Game.ItemCsv::Load()**: string `Data/物品定义.csv`
```text
3: ldc.i4.1 
4: stsfld System.Boolean MidnightPawn.Game.ItemCsv::loaded
5: newobj System.Void System.Collections.Generic.Dictionary`2<System.String,MidnightPawn.Game.ItemDef>::.ctor()
6: stsfld System.Collections.Generic.Dictionary`2<System.String,MidnightPawn.Game.ItemDef> MidnightPawn.Game.ItemCsv::table
7: ldstr lewd_meme
8: ldstr Data/物品定义.csv
9: call System.String ModHelper::ReadModFileText(System.String,System.String)
10: stloc.0 
11: ldloc.0 
12: call System.Boolean System.String::IsNullOrEmpty(System.String)
13: brfalse.s IL_003F: ldloc.0
14: ldstr 物品定义表不存在（Data/物品定义.csv），全部使用代码内默认值
15: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
16: leave IL_020D: ret
17: ldloc.0 
```
**System.Void MidnightPawn.Game.ItemCsv::Load()**: string `物品定义表不存在（Data/物品定义.csv），全部使用代码内默认值`
```text
9: call System.String ModHelper::ReadModFileText(System.String,System.String)
10: stloc.0 
11: ldloc.0 
12: call System.Boolean System.String::IsNullOrEmpty(System.String)
13: brfalse.s IL_003F: ldloc.0
14: ldstr 物品定义表不存在（Data/物品定义.csv），全部使用代码内默认值
15: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
16: leave IL_020D: ret
17: ldloc.0 
18: callvirt System.Int32 System.String::get_Length()
19: ldc.i4.0 
20: ble.s IL_005E: ldc.i4.0
21: ldloc.0 
22: ldc.i4.0 
23: callvirt System.Char System.String::get_Chars(System.Int32)
```
**System.Void MidnightPawn.Game.ItemCsv::Load()**: string `物品定义表已加载：`
```text
187: ldloc.3 
188: ldloc.2 
189: ldlen 
190: conv.i4 
191: blt IL_0071: ldloc.2
192: ldstr 物品定义表已加载：
193: ldloca.s V_1
194: call System.String System.Int32::ToString()
195: ldstr  件
196: call System.String System.String::Concat(System.String,System.String,System.String)
197: call System.Void MidnightPawn.ModCtx::Log(System.Object)
198: leave.s IL_020D: ret
199: stloc.s V_8
200: ldstr 物品定义表加载失败: 
201: ldloc.s V_8
```
**System.Void MidnightPawn.Game.ItemCsv::Load()**: string `物品定义表加载失败: `
```text
195: ldstr  件
196: call System.String System.String::Concat(System.String,System.String,System.String)
197: call System.Void MidnightPawn.ModCtx::Log(System.Object)
198: leave.s IL_020D: ret
199: stloc.s V_8
200: ldstr 物品定义表加载失败: 
201: ldloc.s V_8
202: callvirt System.String System.Exception::get_Message()
203: call System.String System.String::Concat(System.String,System.String)
204: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
205: leave.s IL_020D: ret
206: ret 
```
**System.Void MidnightPawn.Game.ItemFactory::ApplyCsv(GameItem,System.String)**: string `应用物品定义失败(`
```text
24: ldarg.0 
25: ldloc.0 
26: call System.Void MidnightPawn.Game.ItemCsv::ApplyTags(GameItem,MidnightPawn.Game.ItemDef)
27: leave.s IL_005F: ret
28: stloc.1 
29: ldstr 应用物品定义失败(
30: ldarg.1 
31: ldstr ): 
32: ldloc.1 
33: callvirt System.String System.Exception::get_Message()
34: call System.String System.String::Concat(System.String,System.String,System.String,System.String)
35: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
36: leave.s IL_005F: ret
37: ret 
```
**System.Void MidnightPawn.Game.NameGen::EnsureLoaded()**: string `Content/names.txt`
```text
1: brfalse.s IL_0008: ldc.i4.1
2: ret 
3: ldc.i4.1 
4: stsfld System.Boolean MidnightPawn.Game.NameGen::loaded
5: ldstr lewd_meme
6: ldstr Content/names.txt
7: call System.String ModHelper::ReadModFileText(System.String,System.String)
8: stloc.0 
9: ldloc.0 
10: call System.Boolean System.String::IsNullOrEmpty(System.String)
11: brfalse.s IL_0035: newobj System.Void System.Collections.Generic.List`1<System.String[]>::.ctor()
12: ldstr 名字库读取失败（Content/names.txt 不存在）
13: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
14: leave IL_00C8: ret
15: newobj System.Void System.Collections.Generic.List`1<System.String[]>::.ctor()
```
**System.Void MidnightPawn.Game.NameGen::EnsureLoaded()**: string `名字库读取失败（Content/names.txt 不存在）`
```text
7: call System.String ModHelper::ReadModFileText(System.String,System.String)
8: stloc.0 
9: ldloc.0 
10: call System.Boolean System.String::IsNullOrEmpty(System.String)
11: brfalse.s IL_0035: newobj System.Void System.Collections.Generic.List`1<System.String[]>::.ctor()
12: ldstr 名字库读取失败（Content/names.txt 不存在）
13: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
14: leave IL_00C8: ret
15: newobj System.Void System.Collections.Generic.List`1<System.String[]>::.ctor()
16: stsfld System.Collections.Generic.List`1<System.String[]> MidnightPawn.Game.NameGen::regions
17: ldloc.0 
18: ldc.i4.s 10
19: ldc.i4.0 
20: callvirt System.String[] System.String::Split(System.Char,System.StringSplitOptions)
21: stloc.1 
```
**System.Void MidnightPawn.Game.NameGen::EnsureLoaded()**: string `名字库: `
```text
50: ldloc.2 
51: ldloc.1 
52: ldlen 
53: conv.i4 
54: blt.s IL_004D: ldloc.1
55: ldstr 名字库: 
56: ldsfld System.Collections.Generic.List`1<System.String[]> MidnightPawn.Game.NameGen::regions
57: callvirt System.Int32 System.Collections.Generic.List`1<System.String[]>::get_Count()
58: stloc.2 
59: ldloca.s V_2
60: call System.String System.Int32::ToString()
61: ldstr  个地区
62: call System.String System.String::Concat(System.String,System.String,System.String)
63: call System.Void MidnightPawn.ModCtx::Log(System.Object)
64: leave.s IL_00C8: ret
```
**System.Void MidnightPawn.Game.NameGen::EnsureLoaded()**: string `名字库加载失败: `
```text
61: ldstr  个地区
62: call System.String System.String::Concat(System.String,System.String,System.String)
63: call System.Void MidnightPawn.ModCtx::Log(System.Object)
64: leave.s IL_00C8: ret
65: stloc.s V_5
66: ldstr 名字库加载失败: 
67: ldloc.s V_5
68: callvirt System.String System.Exception::get_Message()
69: call System.String System.String::Concat(System.String,System.String)
70: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
71: leave.s IL_00C8: ret
72: ret 
```
Found 9 relevant literal-IL matches.
## probably_adult_goods.dll
Methods with IL: 248
**System.Void ProbablyAdultGoods.ItemCatalog::EnsureLoaded()**: string `Data/catalog.csv`
```text
5: ldsfld System.Collections.Generic.List`1<ProbablyAdultGoods.CatalogEntry> ProbablyAdultGoods.ItemCatalog::Entries
6: callvirt System.Void System.Collections.Generic.List`1<ProbablyAdultGoods.CatalogEntry>::Clear()
7: ldsfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.List`1<ProbablyAdultGoods.CatalogEntry>> ProbablyAdultGoods.ItemCatalog::ByTag
8: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.List`1<ProbablyAdultGoods.CatalogEntry>>::Clear()
9: ldstr probably_adult_goods
10: ldstr Data/catalog.csv
11: call System.String ModHelper::ReadModFileText(System.String,System.String)
12: stloc.0 
13: ldloc.0 
14: call System.Boolean System.String::IsNullOrEmpty(System.String)
15: brfalse.s IL_004F: ldloc.0
16: ldsfld ModLog ProbablyAdultGoods.ModEntry::Log
17: dup 
18: brtrue.s IL_0044: ldstr "catalog.csv missing or empty."
19: pop 
```
**System.Void ProbablyAdultGoods.ItemCatalog::EnsureLoaded()**: string `catalog.csv missing or empty.`
```text
16: ldsfld ModLog ProbablyAdultGoods.ModEntry::Log
17: dup 
18: brtrue.s IL_0044: ldstr "catalog.csv missing or empty."
19: pop 
20: ret 
21: ldstr catalog.csv missing or empty.
22: call System.Void ModLog::Log(System.Object)
23: ret 
24: ldloc.0 
25: ldc.i4.2 
26: newarr System.String
27: dup 
28: ldc.i4.0 
29: ldstr  
30: stelem.ref 
```
**System.Void ProbablyAdultGoods.ItemSheet::EnsureLoaded()**: string `Data/物品标签价格.csv`
```text
1: brfalse.s IL_0008: ldc.i4.1
2: ret 
3: ldc.i4.1 
4: stsfld System.Boolean ProbablyAdultGoods.ItemSheet::loaded
5: ldstr probably_adult_goods
6: ldstr Data/物品标签价格.csv
7: call System.String ModHelper::ReadModFileText(System.String,System.String)
8: stloc.0 
9: ldloc.0 
10: call System.Boolean System.String::IsNullOrEmpty(System.String)
11: brfalse.s IL_003B: ldloc.0
12: ldsfld ModLog ProbablyAdultGoods.ModEntry::Log
13: dup 
14: brtrue.s IL_0030: ldstr "物品标签价格.csv missing."
15: pop 
```
**System.Void ProbablyAdultGoods.ItemSheet::EnsureLoaded()**: string `物品标签价格.csv missing.`
```text
12: ldsfld ModLog ProbablyAdultGoods.ModEntry::Log
13: dup 
14: brtrue.s IL_0030: ldstr "物品标签价格.csv missing."
15: pop 
16: ret 
17: ldstr 物品标签价格.csv missing.
18: call System.Void ModLog::Log(System.Object)
19: ret 
20: ldloc.0 
21: ldc.i4.2 
22: newarr System.String
23: dup 
24: ldc.i4.0 
25: ldstr  
26: stelem.ref 
```
Found 4 relevant literal-IL matches.
## 起飞攻速装.dll
Methods with IL: 1199
Found 0 relevant literal-IL matches.
## ShadowTrade.dll
Methods with IL: 651
**System.Void MidnightPawn.Game.NameGen::EnsureLoaded()**: string `Content/names.txt`
```text
1: brfalse.s IL_0008: ldc.i4.1
2: ret 
3: ldc.i4.1 
4: stsfld System.Boolean MidnightPawn.Game.NameGen::loaded
5: ldstr lewd_meme_remake
6: ldstr Content/names.txt
7: call System.String ModHelper::ReadModFileText(System.String,System.String)
8: stloc.0 
9: ldloc.0 
10: call System.Boolean System.String::IsNullOrEmpty(System.String)
11: brfalse.s IL_0035: newobj System.Void System.Collections.Generic.List`1<System.String[]>::.ctor()
12: ldstr 名字库读取失败（Content/names.txt 不存在）
13: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
14: leave IL_00F0: ret
15: newobj System.Void System.Collections.Generic.List`1<System.String[]>::.ctor()
```
**System.Void MidnightPawn.Game.NameGen::EnsureLoaded()**: string `名字库读取失败（Content/names.txt 不存在）`
```text
7: call System.String ModHelper::ReadModFileText(System.String,System.String)
8: stloc.0 
9: ldloc.0 
10: call System.Boolean System.String::IsNullOrEmpty(System.String)
11: brfalse.s IL_0035: newobj System.Void System.Collections.Generic.List`1<System.String[]>::.ctor()
12: ldstr 名字库读取失败（Content/names.txt 不存在）
13: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
14: leave IL_00F0: ret
15: newobj System.Void System.Collections.Generic.List`1<System.String[]>::.ctor()
16: stsfld System.Collections.Generic.List`1<System.String[]> MidnightPawn.Game.NameGen::regions
17: ldloc.0 
18: ldc.i4.1 
19: newarr System.Char
20: dup 
21: ldc.i4.0 
```
**System.Void MidnightPawn.Game.NameGen::EnsureLoaded()**: string `名字库: `
```text
67: ldloc.2 
68: ldloc.1 
69: ldlen 
70: conv.i4 
71: blt.s IL_0056: ldloc.1
72: ldstr 名字库: 
73: ldsfld System.Collections.Generic.List`1<System.String[]> MidnightPawn.Game.NameGen::regions
74: callvirt System.Int32 System.Collections.Generic.List`1<System.String[]>::get_Count()
75: stloc.s V_5
76: ldloca.s V_5
77: call System.String System.Int32::ToString()
78: ldstr  个地区（仅中国/日本/韩国）
79: call System.String System.String::Concat(System.String,System.String,System.String)
80: call System.Void MidnightPawn.ModCtx::Log(System.Object)
81: leave.s IL_00F0: ret
```
**System.Void MidnightPawn.Game.NameGen::EnsureLoaded()**: string `名字库加载失败: `
```text
78: ldstr  个地区（仅中国/日本/韩国）
79: call System.String System.String::Concat(System.String,System.String,System.String)
80: call System.Void MidnightPawn.ModCtx::Log(System.Object)
81: leave.s IL_00F0: ret
82: stloc.s V_6
83: ldstr 名字库加载失败: 
84: ldloc.s V_6
85: callvirt System.String System.Exception::get_Message()
86: call System.String System.String::Concat(System.String,System.String)
87: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
88: leave.s IL_00F0: ret
89: ret 
```
Found 4 relevant literal-IL matches.
