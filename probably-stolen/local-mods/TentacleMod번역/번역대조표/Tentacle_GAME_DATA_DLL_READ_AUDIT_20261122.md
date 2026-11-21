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
## Complete IL for item CSV loaders: MidnightPawn.dll
### System.Void MidnightPawn.Game.ItemCsv::Load()
```text
0: ldsfld System.Boolean MidnightPawn.Game.ItemCsv::loaded
1: brfalse.s IL_0008: ldc.i4.1
2: ret 
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
18: callvirt System.Int32 System.String::get_Length()
19: ldc.i4.0 
20: ble.s IL_005E: ldc.i4.0
21: ldloc.0 
22: ldc.i4.0 
23: callvirt System.Char System.String::get_Chars(System.Int32)
24: ldc.i4 65279
25: bne.un.s IL_005E: ldc.i4.0
26: ldloc.0 
27: ldc.i4.1 
28: callvirt System.String System.String::Substring(System.Int32)
29: stloc.0 
30: ldc.i4.0 
31: stloc.1 
32: ldloc.0 
33: ldc.i4.s 10
34: ldc.i4.0 
35: callvirt System.String[] System.String::Split(System.Char,System.StringSplitOptions)
36: stloc.2 
37: ldc.i4.0 
38: stloc.3 
39: br IL_01CD: ldloc.3
40: ldloc.2 
41: ldloc.3 
42: ldelem.ref 
43: ldc.i4.s 13
44: callvirt System.String System.String::TrimEnd(System.Char)
45: stloc.s V_4
46: ldloc.s V_4
47: callvirt System.Int32 System.String::get_Length()
48: brfalse IL_01C9: ldloc.3
49: ldloc.s V_4
50: callvirt System.String System.String::TrimStart()
51: ldstr #
52: callvirt System.Boolean System.String::StartsWith(System.String)
53: brtrue IL_01C9: ldloc.3
54: ldloc.s V_4
55: ldc.i4.s 44
56: ldc.i4.0 
57: callvirt System.String[] System.String::Split(System.Char,System.StringSplitOptions)
58: stloc.s V_5
59: ldloc.s V_5
60: ldlen 
61: conv.i4 
62: ldc.i4.3 
63: blt IL_01C9: ldloc.3
64: ldloc.s V_5
65: ldc.i4.0 
66: ldelem.ref 
67: callvirt System.String System.String::Trim()
68: stloc.s V_6
69: ldloc.s V_6
70: callvirt System.Int32 System.String::get_Length()
71: brfalse IL_01C9: ldloc.3
72: ldloc.s V_6
73: ldstr id
74: call System.Boolean System.String::op_Equality(System.String,System.String)
75: brtrue IL_01C9: ldloc.3
76: newobj System.Void MidnightPawn.Game.ItemDef::.ctor()
77: dup 
78: ldloc.s V_6
79: stfld System.String MidnightPawn.Game.ItemDef::Id
80: dup 
81: ldloc.s V_5
82: ldlen 
83: conv.i4 
84: ldc.i4.1 
85: bgt.s IL_00F6: ldloc.s V_5
86: ldloc.s V_6
87: br.s IL_00FF: stfld System.String MidnightPawn.Game.ItemDef::Name
88: ldloc.s V_5
89: ldc.i4.1 
90: ldelem.ref 
91: callvirt System.String System.String::Trim()
92: stfld System.String MidnightPawn.Game.ItemDef::Name
93: dup 
94: ldloc.s V_5
95: ldlen 
96: conv.i4 
97: ldc.i4.2 
98: bgt.s IL_0113: ldloc.s V_5
99: ldstr 0
100: br.s IL_0117: ldc.i4.0
101: ldloc.s V_5
102: ldc.i4.2 
103: ldelem.ref 
104: ldc.i4.0 
105: call System.Int32 MidnightPawn.Game.ItemCsv::ParseInt(System.String,System.Int32)
106: stfld System.Int32 MidnightPawn.Game.ItemDef::Price
107: dup 
108: ldloc.s V_5
109: ldlen 
110: conv.i4 
111: ldc.i4.3 
112: bgt.s IL_0131: ldloc.s V_5
113: ldstr 1
114: br.s IL_0135: ldc.i4.1
115: ldloc.s V_5
116: ldc.i4.3 
117: ldelem.ref 
118: ldc.i4.1 
119: call System.Int32 MidnightPawn.Game.ItemCsv::ParseInt(System.String,System.Int32)
120: stfld System.Int32 MidnightPawn.Game.ItemDef::Width
121: dup 
122: ldloc.s V_5
123: ldlen 
124: conv.i4 
125: ldc.i4.4 
126: bgt.s IL_014F: ldloc.s V_5
127: ldstr 1
128: br.s IL_0153: ldc.i4.1
129: ldloc.s V_5
130: ldc.i4.4 
131: ldelem.ref 
132: ldc.i4.1 
133: call System.Int32 MidnightPawn.Game.ItemCsv::ParseInt(System.String,System.Int32)
134: stfld System.Int32 MidnightPawn.Game.ItemDef::Height
135: dup 
136: ldloc.s V_5
137: ldlen 
138: conv.i4 
139: ldc.i4.5 
140: bgt.s IL_016D: ldloc.s V_5
141: ldstr 
142: br.s IL_0176: stfld System.String MidnightPawn.Game.ItemDef::Icon
143: ldloc.s V_5
144: ldc.i4.5 
145: ldelem.ref 
146: callvirt System.String System.String::Trim()
147: stfld System.String MidnightPawn.Game.ItemDef::Icon
148: dup 
149: ldloc.s V_5
150: ldlen 
151: conv.i4 
152: ldc.i4.6 
153: bgt.s IL_018A: ldloc.s V_5
154: ldstr 
155: br.s IL_018E: call System.String[] MidnightPawn.Game.ItemCsv::SplitList(System.String)
156: ldloc.s V_5
157: ldc.i4.6 
158: ldelem.ref 
159: call System.String[] MidnightPawn.Game.ItemCsv::SplitList(System.String)
160: stfld System.String[] MidnightPawn.Game.ItemDef::Types
161: dup 
162: ldloc.s V_5
163: ldlen 
164: conv.i4 
165: ldc.i4.7 
166: bgt.s IL_01A7: ldloc.s V_5
167: ldstr 
168: br.s IL_01AB: call System.String[] MidnightPawn.Game.ItemCsv::SplitList(System.String)
169: ldloc.s V_5
170: ldc.i4.7 
171: ldelem.ref 
172: call System.String[] MidnightPawn.Game.ItemCsv::SplitList(System.String)
173: stfld System.String[] MidnightPawn.Game.ItemDef::Tags
174: stloc.s V_7
175: ldsfld System.Collections.Generic.Dictionary`2<System.String,MidnightPawn.Game.ItemDef> MidnightPawn.Game.ItemCsv::table
176: ldloc.s V_6
177: ldloc.s V_7
178: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,MidnightPawn.Game.ItemDef>::set_Item(System.String,MidnightPawn.Game.ItemDef)
179: ldloc.1 
180: ldc.i4.1 
181: add 
182: stloc.1 
183: ldloc.3 
184: ldc.i4.1 
185: add 
186: stloc.3 
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
202: callvirt System.String System.Exception::get_Message()
203: call System.String System.String::Concat(System.String,System.String)
204: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
205: leave.s IL_020D: ret
206: ret 
```
### MidnightPawn.Game.ItemDef MidnightPawn.Game.ItemCsv::Get(System.String)
```text
0: call System.Void MidnightPawn.Game.ItemCsv::Load()
1: ldsfld System.Collections.Generic.Dictionary`2<System.String,MidnightPawn.Game.ItemDef> MidnightPawn.Game.ItemCsv::table
2: brtrue.s IL_000E: ldsfld System.Collections.Generic.Dictionary`2<System.String,MidnightPawn.Game.ItemDef> MidnightPawn.Game.ItemCsv::table
3: ldnull 
4: ret 
5: ldsfld System.Collections.Generic.Dictionary`2<System.String,MidnightPawn.Game.ItemDef> MidnightPawn.Game.ItemCsv::table
6: ldarg.0 
7: ldloca.s V_0
8: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,MidnightPawn.Game.ItemDef>::TryGetValue(System.String,MidnightPawn.Game.ItemDef&)
9: brtrue.s IL_001F: ldloc.0
10: ldnull 
11: ret 
12: ldloc.0 
13: ret 
```
### System.Void MidnightPawn.Game.ItemCsv::ApplyTypes(GameItem,MidnightPawn.Game.ItemDef)
```text
0: ldarg.0 
1: brfalse.s IL_000E: ret
2: ldarg.1 
3: brfalse.s IL_000E: ret
4: ldarg.1 
5: ldfld System.String[] MidnightPawn.Game.ItemDef::Types
6: brtrue.s IL_000F: ldarg.1
7: ret 
8: ldarg.1 
9: ldfld System.String[] MidnightPawn.Game.ItemDef::Types
10: stloc.0 
11: ldc.i4.0 
12: stloc.1 
13: br.s IL_0070: ldloc.1
14: ldloc.0 
15: ldloc.1 
16: ldelem.ref 
17: stloc.2 
18: ldloc.2 
19: call System.String MidnightPawn.Game.ItemDef::MapType(System.String)
20: stloc.3 
21: ldloc.3 
22: brtrue.s IL_005F: nop
23: ldc.i4.5 
24: newarr System.String
25: dup 
26: ldc.i4.0 
27: ldstr 未知分类「
28: stelem.ref 
29: dup 
30: ldc.i4.1 
31: ldloc.2 
32: stelem.ref 
33: dup 
34: ldc.i4.2 
35: ldstr 」（物品 
36: stelem.ref 
37: dup 
38: ldc.i4.3 
39: ldarg.1 
40: ldfld System.String MidnightPawn.Game.ItemDef::Id
41: stelem.ref 
42: dup 
43: ldc.i4.4 
44: ldstr ）
45: stelem.ref 
46: call System.String System.String::Concat(System.String[])
47: call System.Void MidnightPawn.ModCtx::LogWarning(System.Object)
48: br.s IL_006C: ldloc.1
49: nop 
50: ldarg.0 
51: ldloc.3 
52: callvirt System.Void GameItem::SetGameItemType(System.String)
53: leave.s IL_006C: ldloc.1
54: pop 
55: leave.s IL_006C: ldloc.1
56: ldloc.1 
57: ldc.i4.1 
58: add 
59: stloc.1 
60: ldloc.1 
61: ldloc.0 
62: ldlen 
63: conv.i4 
64: blt.s IL_001A: ldloc.0
65: ret 
```
### System.Void MidnightPawn.Game.ItemCsv::ApplyTags(GameItem,MidnightPawn.Game.ItemDef)
```text
0: ldarg.0 
1: brfalse.s IL_000E: ret
2: ldarg.1 
3: brfalse.s IL_000E: ret
4: ldarg.1 
5: ldfld System.String[] MidnightPawn.Game.ItemDef::Tags
6: brtrue.s IL_000F: ldarg.1
7: ret 
8: ldarg.1 
9: ldfld System.String[] MidnightPawn.Game.ItemDef::Tags
10: stloc.0 
11: ldc.i4.0 
12: stloc.1 
13: br.s IL_004F: ldloc.1
14: ldloc.0 
15: ldloc.1 
16: ldelem.ref 
17: stloc.2 
18: ldarg.0 
19: ldloc.2 
20: ldsfld System.Action`1<TagState> MidnightPawn.Game.ItemCsv/<>c::<>9__8_0
21: dup 
22: brtrue.s IL_003F: ldc.i4.0
23: pop 
24: ldsfld MidnightPawn.Game.ItemCsv/<>c MidnightPawn.Game.ItemCsv/<>c::<>9
25: ldftn System.Void MidnightPawn.Game.ItemCsv/<>c::<ApplyTags>b__8_0(TagState)
26: newobj System.Void System.Action`1<TagState>::.ctor(System.Object,System.IntPtr)
27: dup 
28: stsfld System.Action`1<TagState> MidnightPawn.Game.ItemCsv/<>c::<>9__8_0
29: ldc.i4.0 
30: callvirt GameItem GameItem::ModifyTag(System.String,System.Action`1<TagState>,System.Boolean)
31: pop 
32: leave.s IL_004B: ldloc.1
33: pop 
34: leave.s IL_004B: ldloc.1
35: ldloc.1 
36: ldc.i4.1 
37: add 
38: stloc.1 
39: ldloc.1 
40: ldloc.0 
41: ldlen 
42: conv.i4 
43: blt.s IL_001A: ldloc.0
44: ret 
```
### System.Void MidnightPawn.Game.ItemCsv::ApplyShapeAndPrice(GameItem,MidnightPawn.Game.ItemDef)
```text
0: ldarg.0 
1: brfalse.s IL_0006: ret
2: ldarg.1 
3: brtrue.s IL_0007: ldarg.1
4: ret 
5: ldarg.1 
6: ldfld System.Int32 MidnightPawn.Game.ItemDef::Price
7: ldc.i4.0 
8: ble.s IL_001D: ldarg.1
9: ldarg.0 
10: ldarg.1 
11: ldfld System.Int32 MidnightPawn.Game.ItemDef::Price
12: conv.i8 
13: callvirt System.Void GameItem::set_unitValue(System.Int64)
14: ldarg.1 
15: ldfld System.Int32 MidnightPawn.Game.ItemDef::Width
16: ldc.i4.0 
17: ble.s IL_0047: ret
18: ldarg.1 
19: ldfld System.Int32 MidnightPawn.Game.ItemDef::Height
20: ldc.i4.0 
21: ble.s IL_0047: ret
22: ldarg.0 
23: ldarg.1 
24: ldfld System.Int32 MidnightPawn.Game.ItemDef::Width
25: ldarg.1 
26: ldfld System.Int32 MidnightPawn.Game.ItemDef::Height
27: call GridShape MidnightPawn.Game.ItemFactory::RectShape(System.Int32,System.Int32)
28: callvirt GameItem GameItem::SetShape(GridShape)
29: pop 
30: ret 
```
## Complete IL for item CSV loaders: probably_adult_goods.dll
### System.Void ProbablyAdultGoods.ItemSheet::Apply(GameItem)
```text
0: ldarg.0 
1: brfalse.s IL_0010: ret
2: ldarg.0 
3: callvirt System.String GameItem::get_identifier()
4: call System.Boolean System.String::IsNullOrEmpty(System.String)
5: brfalse.s IL_0011: call System.Void ProbablyAdultGoods.ItemSheet::EnsureLoaded()
6: ret 
7: call System.Void ProbablyAdultGoods.ItemSheet::EnsureLoaded()
8: ldsfld System.Collections.Generic.Dictionary`2<System.String,ProbablyAdultGoods.ItemSheetRow> ProbablyAdultGoods.ItemSheet::Rows
9: ldarg.0 
10: callvirt System.String GameItem::get_identifier()
11: ldloca.s V_0
12: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,ProbablyAdultGoods.ItemSheetRow>::TryGetValue(System.String,ProbablyAdultGoods.ItemSheetRow&)
13: brtrue.s IL_002B: ldloc.0
14: ret 
15: ldloc.0 
16: ldfld System.Boolean ProbablyAdultGoods.ItemSheetRow::UnlimitedSale
17: brfalse.s IL_0054: ldarg.0
18: ldarg.0 
19: ldarg.0 
20: callvirt System.String GameItem::get_identifier()
21: call System.Int64 ProbablyAdultGoods.ItemSheet::MaxBaseValue(System.String)
22: callvirt GameItem GameItem::SetValue(System.Int64)
23: pop 
24: ldarg.0 
25: ldstr ADULT_UNLIMITED_SALE
26: ldc.i4.0 
27: callvirt System.Boolean GameItem::EnableTag(System.String,System.Boolean)
28: pop 
29: br.s IL_0061: ldloc.0
30: ldarg.0 
31: ldloc.0 
32: ldfld System.Int64 ProbablyAdultGoods.ItemSheetRow::Price
33: callvirt GameItem GameItem::SetValue(System.Int64)
34: pop 
35: ldloc.0 
36: ldfld System.Collections.Generic.List`1<System.String> ProbablyAdultGoods.ItemSheetRow::GameTypes
37: newobj System.Void System.Collections.Generic.HashSet`1<System.String>::.ctor(System.Collections.Generic.IEnumerable`1<System.String>)
38: stloc.1 
39: ldc.i4.0 
40: stloc.2 
41: br.s IL_0096: ldloc.2
42: ldsfld System.String[] ProbablyAdultGoods.ItemSheet::ManagedTypes
43: ldloc.2 
44: ldelem.ref 
45: stloc.3 
46: ldloc.1 
47: ldloc.3 
48: callvirt System.Boolean System.Collections.Generic.HashSet`1<System.String>::Contains(System.String)
49: brfalse.s IL_008B: ldarg.0
50: ldarg.0 
51: ldloc.3 
52: callvirt System.Void GameItem::SetGameItemType(System.String)
53: br.s IL_0092: ldloc.2
54: ldarg.0 
55: ldloc.3 
56: callvirt System.Void GameItem::RemoveGameItemType(System.String)
57: ldloc.2 
58: ldc.i4.1 
59: add 
60: stloc.2 
61: ldloc.2 
62: ldsfld System.String[] ProbablyAdultGoods.ItemSheet::ManagedTypes
63: ldlen 
64: conv.i4 
65: blt.s IL_0071: ldsfld System.String[] ProbablyAdultGoods.ItemSheet::ManagedTypes
66: ret 
```
### System.Void ProbablyAdultGoods.ItemSheet::EnsureLoaded()
```text
0: ldsfld System.Boolean ProbablyAdultGoods.ItemSheet::loaded
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
27: dup 
28: ldc.i4.1 
29: ldstr 

30: stelem.ref 
31: ldc.i4.0 
32: callvirt System.String[] System.String::Split(System.String[],System.StringSplitOptions)
33: stloc.1 
34: ldc.i4.0 
35: stloc.2 
36: br IL_0199: ldloc.2
37: ldloc.1 
38: ldloc.2 
39: ldelem.ref 
40: call System.Collections.Generic.List`1<System.String> ProbablyAdultGoods.ItemSheet::SplitCsv(System.String)
41: stloc.3 
42: ldloc.3 
43: callvirt System.Int32 System.Collections.Generic.List`1<System.String>::get_Count()
44: ldc.i4.6 
45: blt IL_0195: ldloc.2
46: ldloc.3 
47: ldc.i4.0 
48: callvirt System.String System.Collections.Generic.List`1<System.String>::get_Item(System.Int32)
49: ldstr id
50: call System.Boolean System.String::op_Equality(System.String,System.String)
51: brtrue IL_0195: ldloc.2
52: ldloc.3 
53: ldc.i4.0 
54: callvirt System.String System.Collections.Generic.List`1<System.String>::get_Item(System.Int32)
55: ldstr #
56: callvirt System.Boolean System.String::StartsWith(System.String)
57: brtrue IL_0195: ldloc.2
58: ldloc.3 
59: ldc.i4.0 
60: callvirt System.String System.Collections.Generic.List`1<System.String>::get_Item(System.Int32)
61: callvirt System.String System.String::Trim()
62: stloc.s V_4
63: ldloc.3 
64: ldc.i4.2 
65: callvirt System.String System.Collections.Generic.List`1<System.String>::get_Item(System.Int32)
66: callvirt System.String System.String::Trim()
67: stloc.s V_5
68: newobj System.Void ProbablyAdultGoods.ItemSheetRow::.ctor()
69: stloc.s V_6
70: ldloc.s V_5
71: ldstr max
72: ldc.i4.5 
73: callvirt System.Boolean System.String::Equals(System.String,System.StringComparison)
74: brfalse.s IL_00DD: ldloc.s V_5
75: ldloc.s V_6
76: ldc.i4.1 
77: stfld System.Boolean ProbablyAdultGoods.ItemSheetRow::UnlimitedSale
78: br.s IL_00F1: ldloc.3
79: ldloc.s V_5
80: ldloca.s V_7
81: call System.Boolean System.Int64::TryParse(System.String,System.Int64&)
82: brfalse.s IL_00F1: ldloc.3
83: ldloc.s V_6
84: ldloc.s V_7
85: stfld System.Int64 ProbablyAdultGoods.ItemSheetRow::Price
86: ldloc.3 
87: ldc.i4.5 
88: callvirt System.String System.Collections.Generic.List`1<System.String>::get_Item(System.Int32)
89: ldc.i4.s 59
90: ldc.i4.0 
91: callvirt System.String[] System.String::Split(System.Char,System.StringSplitOptions)
92: stloc.s V_8
93: ldc.i4.0 
94: stloc.s V_9
95: br.s IL_017F: ldloc.s V_9
96: ldloc.s V_8
97: ldloc.s V_9
98: ldelem.ref 
99: callvirt System.String System.String::Trim()
100: stloc.s V_10
101: ldloc.s V_10
102: callvirt System.Int32 System.String::get_Length()
103: brfalse.s IL_0179: ldloc.s V_9
104: ldsfld System.Collections.Generic.Dictionary`2<System.String,System.String> ProbablyAdultGoods.ItemSheet::TypeByLabel
105: ldloc.s V_10
106: ldloca.s V_11
107: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,System.String>::TryGetValue(System.String,System.String&)
108: brfalse.s IL_013C: ldsfld ModLog ProbablyAdultGoods.ModEntry::Log
109: ldloc.s V_6
110: ldfld System.Collections.Generic.List`1<System.String> ProbablyAdultGoods.ItemSheetRow::GameTypes
111: ldloc.s V_11
112: callvirt System.Void System.Collections.Generic.List`1<System.String>::Add(System.String)
113: br.s IL_0179: ldloc.s V_9
114: ldsfld ModLog ProbablyAdultGoods.ModEntry::Log
115: dup 
116: brtrue.s IL_0147: ldc.i4.5
117: pop 
118: br.s IL_0179: ldloc.s V_9
119: ldc.i4.5 
120: newarr System.String
121: dup 
122: ldc.i4.0 
123: ldstr Unknown game type '
124: stelem.ref 
125: dup 
126: ldc.i4.1 
127: ldloc.s V_10
128: stelem.ref 
129: dup 
130: ldc.i4.2 
131: ldstr ' on 
132: stelem.ref 
133: dup 
134: ldc.i4.3 
135: ldloc.s V_4
136: stelem.ref 
137: dup 
138: ldc.i4.4 
139: ldstr .
140: stelem.ref 
141: call System.String System.String::Concat(System.String[])
142: call System.Void ModLog::Log(System.Object)
143: ldloc.s V_9
144: ldc.i4.1 
145: add 
146: stloc.s V_9
147: ldloc.s V_9
148: ldloc.s V_8
149: ldlen 
150: conv.i4 
151: blt.s IL_0107: ldloc.s V_8
152: ldsfld System.Collections.Generic.Dictionary`2<System.String,ProbablyAdultGoods.ItemSheetRow> ProbablyAdultGoods.ItemSheet::Rows
153: ldloc.s V_4
154: ldloc.s V_6
155: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,ProbablyAdultGoods.ItemSheetRow>::set_Item(System.String,ProbablyAdultGoods.ItemSheetRow)
156: ldloc.2 
157: ldc.i4.1 
158: add 
159: stloc.2 
160: ldloc.2 
161: ldloc.1 
162: ldlen 
163: conv.i4 
164: blt IL_0060: ldloc.1
165: ldsfld ModLog ProbablyAdultGoods.ModEntry::Log
166: dup 
167: brtrue.s IL_01AC: ldstr "Item sheet loaded: {0} row(s)."
168: pop 
169: ret 
170: ldstr Item sheet loaded: {0} row(s).
171: ldsfld System.Collections.Generic.Dictionary`2<System.String,ProbablyAdultGoods.ItemSheetRow> ProbablyAdultGoods.ItemSheet::Rows
172: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,ProbablyAdultGoods.ItemSheetRow>::get_Count()
173: box System.Int32
174: call System.String System.String::Format(System.String,System.Object)
175: call System.Void ModLog::Log(System.Object)
176: ret 
```
