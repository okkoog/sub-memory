Inspected methods: 1511; matched: 5

# Eleanor v1.0.3 IL locale audit

## ProbablyEleanorFollowup.EmbeddedAssets::IsChineseLocale(06000092)
- anchor IL[18]: IL_0032: ldstr "zh"
  0001 IL_0005: stloc.0
  0002 IL_0006: ldloc.0
  0003 IL_0007: ldnull
  0004 IL_0008: call System.Boolean UnityEngine.Object::op_Inequality(UnityEngine.Object,UnityEngine.Object)
  0005 IL_000D: brfalse.s IL_0040
  0006 IL_000F: ldloc.0
  0007 IL_0010: callvirt UnityEngine.Localization.LocaleIdentifier UnityEngine.Localization.Locale::get_Identifier()
  0008 IL_0015: stloc.1
  0009 IL_0016: ldloca.s V_1
  0010 IL_0018: call System.String UnityEngine.Localization.LocaleIdentifier::get_Code()
  0011 IL_001D: call System.Boolean System.String::IsNullOrEmpty(System.String)
  0012 IL_0022: brtrue.s IL_0040
  0013 IL_0024: ldloc.0
  0014 IL_0025: callvirt UnityEngine.Localization.LocaleIdentifier UnityEngine.Localization.Locale::get_Identifier()
  0015 IL_002A: stloc.1
  0016 IL_002B: ldloca.s V_1
  0017 IL_002D: call System.String UnityEngine.Localization.LocaleIdentifier::get_Code()
  0018 IL_0032: ldstr "zh"
  0019 IL_0037: ldc.i4.5
  0020 IL_0038: callvirt System.Boolean System.String::StartsWith(System.String,System.StringComparison)
  0021 IL_003D: stloc.2
  0022 IL_003E: leave.s IL_0047
  0023 IL_0040: leave.s IL_0045
  0024 IL_0042: pop
  0025 IL_0043: leave.s IL_0045
  0026 IL_0045: ldc.i4.0
  0027 IL_0046: ret
  0028 IL_0047: ldloc.2
  0029 IL_0048: ret
## ProbablyEleanorFollowup.EmbeddedAssets::InstallLocalization(06000097)
- anchor IL[8]: IL_0016: ldstr "probably_eleanor_followup"
  0000 IL_0000: ldarg.0
  0001 IL_0001: brfalse.s IL_0022
  0002 IL_0003: ldarg.0
  0003 IL_0004: ldfld ModManifest LoadedMod::Manifest
  0004 IL_0009: brfalse.s IL_0022
  0005 IL_000B: ldarg.0
  0006 IL_000C: ldfld ModManifest LoadedMod::Manifest
  0007 IL_0011: ldfld System.String ModManifest::ID
  0008 IL_0016: ldstr "probably_eleanor_followup"
  0009 IL_001B: call System.Boolean System.String::op_Inequality(System.String,System.String)
  0010 IL_0020: brfalse.s IL_0023
  0011 IL_0022: ret
  0012 IL_0023: ldarg.0
  0013 IL_0024: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::AddLocaleAliases(LoadedMod)
  0014 IL_0029: ldstr "Localization/zh.csv"
  0015 IL_002E: call System.String ProbablyEleanorFollowup.EmbeddedAssets::ReadText(System.String)
  0016 IL_0033: call System.Collections.Generic.Dictionary`2<System.String,System.String> ProbablyEleanorFollowup.EmbeddedAssets::ParseCsv(System.String)
  0017 IL_0038: stloc.0
  0018 IL_0039: ldstr "Localization/en.csv"
  0019 IL_003E: call System.String ProbablyEleanorFollowup.EmbeddedAssets::ReadText(System.String)
  0020 IL_0043: call System.Collections.Generic.Dictionary`2<System.String,System.String> ProbablyEleanorFollowup.EmbeddedAssets::ParseCsv(System.String)
  0021 IL_0048: stloc.1
  0022 IL_0049: ldloc.0
  0023 IL_004A: brfalse.s IL_0088
  0024 IL_004C: ldloc.0
  0025 IL_004D: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0026 IL_0052: ldc.i4.0
  0027 IL_0053: ble.s IL_0088
  0028 IL_0055: ldarg.0
  0029 IL_0056: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0030 IL_005B: ldstr "zh"
  0031 IL_0060: ldloc.0
  0032 IL_0061: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0033 IL_0066: ldarg.0
- anchor IL[14]: IL_0029: ldstr "Localization/zh.csv"
  0000 IL_0000: ldarg.0
  0001 IL_0001: brfalse.s IL_0022
  0002 IL_0003: ldarg.0
  0003 IL_0004: ldfld ModManifest LoadedMod::Manifest
  0004 IL_0009: brfalse.s IL_0022
  0005 IL_000B: ldarg.0
  0006 IL_000C: ldfld ModManifest LoadedMod::Manifest
  0007 IL_0011: ldfld System.String ModManifest::ID
  0008 IL_0016: ldstr "probably_eleanor_followup"
  0009 IL_001B: call System.Boolean System.String::op_Inequality(System.String,System.String)
  0010 IL_0020: brfalse.s IL_0023
  0011 IL_0022: ret
  0012 IL_0023: ldarg.0
  0013 IL_0024: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::AddLocaleAliases(LoadedMod)
  0014 IL_0029: ldstr "Localization/zh.csv"
  0015 IL_002E: call System.String ProbablyEleanorFollowup.EmbeddedAssets::ReadText(System.String)
  0016 IL_0033: call System.Collections.Generic.Dictionary`2<System.String,System.String> ProbablyEleanorFollowup.EmbeddedAssets::ParseCsv(System.String)
  0017 IL_0038: stloc.0
  0018 IL_0039: ldstr "Localization/en.csv"
  0019 IL_003E: call System.String ProbablyEleanorFollowup.EmbeddedAssets::ReadText(System.String)
  0020 IL_0043: call System.Collections.Generic.Dictionary`2<System.String,System.String> ProbablyEleanorFollowup.EmbeddedAssets::ParseCsv(System.String)
  0021 IL_0048: stloc.1
  0022 IL_0049: ldloc.0
  0023 IL_004A: brfalse.s IL_0088
  0024 IL_004C: ldloc.0
  0025 IL_004D: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0026 IL_0052: ldc.i4.0
  0027 IL_0053: ble.s IL_0088
  0028 IL_0055: ldarg.0
  0029 IL_0056: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0030 IL_005B: ldstr "zh"
  0031 IL_0060: ldloc.0
  0032 IL_0061: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0033 IL_0066: ldarg.0
  0034 IL_0067: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0035 IL_006C: ldstr "zh-Hans"
  0036 IL_0071: ldloc.0
  0037 IL_0072: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0038 IL_0077: ldarg.0
  0039 IL_0078: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
- anchor IL[18]: IL_0039: ldstr "Localization/en.csv"
  0001 IL_0001: brfalse.s IL_0022
  0002 IL_0003: ldarg.0
  0003 IL_0004: ldfld ModManifest LoadedMod::Manifest
  0004 IL_0009: brfalse.s IL_0022
  0005 IL_000B: ldarg.0
  0006 IL_000C: ldfld ModManifest LoadedMod::Manifest
  0007 IL_0011: ldfld System.String ModManifest::ID
  0008 IL_0016: ldstr "probably_eleanor_followup"
  0009 IL_001B: call System.Boolean System.String::op_Inequality(System.String,System.String)
  0010 IL_0020: brfalse.s IL_0023
  0011 IL_0022: ret
  0012 IL_0023: ldarg.0
  0013 IL_0024: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::AddLocaleAliases(LoadedMod)
  0014 IL_0029: ldstr "Localization/zh.csv"
  0015 IL_002E: call System.String ProbablyEleanorFollowup.EmbeddedAssets::ReadText(System.String)
  0016 IL_0033: call System.Collections.Generic.Dictionary`2<System.String,System.String> ProbablyEleanorFollowup.EmbeddedAssets::ParseCsv(System.String)
  0017 IL_0038: stloc.0
  0018 IL_0039: ldstr "Localization/en.csv"
  0019 IL_003E: call System.String ProbablyEleanorFollowup.EmbeddedAssets::ReadText(System.String)
  0020 IL_0043: call System.Collections.Generic.Dictionary`2<System.String,System.String> ProbablyEleanorFollowup.EmbeddedAssets::ParseCsv(System.String)
  0021 IL_0048: stloc.1
  0022 IL_0049: ldloc.0
  0023 IL_004A: brfalse.s IL_0088
  0024 IL_004C: ldloc.0
  0025 IL_004D: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0026 IL_0052: ldc.i4.0
  0027 IL_0053: ble.s IL_0088
  0028 IL_0055: ldarg.0
  0029 IL_0056: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0030 IL_005B: ldstr "zh"
  0031 IL_0060: ldloc.0
  0032 IL_0061: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0033 IL_0066: ldarg.0
  0034 IL_0067: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0035 IL_006C: ldstr "zh-Hans"
  0036 IL_0071: ldloc.0
  0037 IL_0072: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0038 IL_0077: ldarg.0
  0039 IL_0078: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0040 IL_007D: ldstr "zh-CN"
  0041 IL_0082: ldloc.0
  0042 IL_0083: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0043 IL_0088: ldloc.1
- anchor IL[30]: IL_005B: ldstr "zh"
  0013 IL_0024: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::AddLocaleAliases(LoadedMod)
  0014 IL_0029: ldstr "Localization/zh.csv"
  0015 IL_002E: call System.String ProbablyEleanorFollowup.EmbeddedAssets::ReadText(System.String)
  0016 IL_0033: call System.Collections.Generic.Dictionary`2<System.String,System.String> ProbablyEleanorFollowup.EmbeddedAssets::ParseCsv(System.String)
  0017 IL_0038: stloc.0
  0018 IL_0039: ldstr "Localization/en.csv"
  0019 IL_003E: call System.String ProbablyEleanorFollowup.EmbeddedAssets::ReadText(System.String)
  0020 IL_0043: call System.Collections.Generic.Dictionary`2<System.String,System.String> ProbablyEleanorFollowup.EmbeddedAssets::ParseCsv(System.String)
  0021 IL_0048: stloc.1
  0022 IL_0049: ldloc.0
  0023 IL_004A: brfalse.s IL_0088
  0024 IL_004C: ldloc.0
  0025 IL_004D: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0026 IL_0052: ldc.i4.0
  0027 IL_0053: ble.s IL_0088
  0028 IL_0055: ldarg.0
  0029 IL_0056: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0030 IL_005B: ldstr "zh"
  0031 IL_0060: ldloc.0
  0032 IL_0061: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0033 IL_0066: ldarg.0
  0034 IL_0067: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0035 IL_006C: ldstr "zh-Hans"
  0036 IL_0071: ldloc.0
  0037 IL_0072: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0038 IL_0077: ldarg.0
  0039 IL_0078: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0040 IL_007D: ldstr "zh-CN"
  0041 IL_0082: ldloc.0
  0042 IL_0083: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0043 IL_0088: ldloc.1
  0044 IL_0089: brfalse.s IL_00B6
  0045 IL_008B: ldloc.1
  0046 IL_008C: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0047 IL_0091: ldc.i4.0
  0048 IL_0092: ble.s IL_00B6
  0049 IL_0094: ldarg.0
  0050 IL_0095: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0051 IL_009A: ldstr "en"
  0052 IL_009F: ldloc.1
  0053 IL_00A0: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0054 IL_00A5: ldarg.0
  0055 IL_00A6: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
- anchor IL[35]: IL_006C: ldstr "zh-Hans"
  0018 IL_0039: ldstr "Localization/en.csv"
  0019 IL_003E: call System.String ProbablyEleanorFollowup.EmbeddedAssets::ReadText(System.String)
  0020 IL_0043: call System.Collections.Generic.Dictionary`2<System.String,System.String> ProbablyEleanorFollowup.EmbeddedAssets::ParseCsv(System.String)
  0021 IL_0048: stloc.1
  0022 IL_0049: ldloc.0
  0023 IL_004A: brfalse.s IL_0088
  0024 IL_004C: ldloc.0
  0025 IL_004D: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0026 IL_0052: ldc.i4.0
  0027 IL_0053: ble.s IL_0088
  0028 IL_0055: ldarg.0
  0029 IL_0056: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0030 IL_005B: ldstr "zh"
  0031 IL_0060: ldloc.0
  0032 IL_0061: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0033 IL_0066: ldarg.0
  0034 IL_0067: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0035 IL_006C: ldstr "zh-Hans"
  0036 IL_0071: ldloc.0
  0037 IL_0072: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0038 IL_0077: ldarg.0
  0039 IL_0078: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0040 IL_007D: ldstr "zh-CN"
  0041 IL_0082: ldloc.0
  0042 IL_0083: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0043 IL_0088: ldloc.1
  0044 IL_0089: brfalse.s IL_00B6
  0045 IL_008B: ldloc.1
  0046 IL_008C: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0047 IL_0091: ldc.i4.0
  0048 IL_0092: ble.s IL_00B6
  0049 IL_0094: ldarg.0
  0050 IL_0095: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0051 IL_009A: ldstr "en"
  0052 IL_009F: ldloc.1
  0053 IL_00A0: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0054 IL_00A5: ldarg.0
  0055 IL_00A6: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0056 IL_00AB: ldstr "en-US"
  0057 IL_00B0: ldloc.1
  0058 IL_00B1: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0059 IL_00B6: ldsfld ModLog ProbablyEleanorFollowup.ModEntry::Log
  0060 IL_00BB: dup
- anchor IL[40]: IL_007D: ldstr "zh-CN"
  0023 IL_004A: brfalse.s IL_0088
  0024 IL_004C: ldloc.0
  0025 IL_004D: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0026 IL_0052: ldc.i4.0
  0027 IL_0053: ble.s IL_0088
  0028 IL_0055: ldarg.0
  0029 IL_0056: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0030 IL_005B: ldstr "zh"
  0031 IL_0060: ldloc.0
  0032 IL_0061: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0033 IL_0066: ldarg.0
  0034 IL_0067: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0035 IL_006C: ldstr "zh-Hans"
  0036 IL_0071: ldloc.0
  0037 IL_0072: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0038 IL_0077: ldarg.0
  0039 IL_0078: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0040 IL_007D: ldstr "zh-CN"
  0041 IL_0082: ldloc.0
  0042 IL_0083: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0043 IL_0088: ldloc.1
  0044 IL_0089: brfalse.s IL_00B6
  0045 IL_008B: ldloc.1
  0046 IL_008C: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0047 IL_0091: ldc.i4.0
  0048 IL_0092: ble.s IL_00B6
  0049 IL_0094: ldarg.0
  0050 IL_0095: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0051 IL_009A: ldstr "en"
  0052 IL_009F: ldloc.1
  0053 IL_00A0: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0054 IL_00A5: ldarg.0
  0055 IL_00A6: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0056 IL_00AB: ldstr "en-US"
  0057 IL_00B0: ldloc.1
  0058 IL_00B1: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0059 IL_00B6: ldsfld ModLog ProbablyEleanorFollowup.ModEntry::Log
  0060 IL_00BB: dup
  0061 IL_00BC: brtrue.s IL_00C0
  0062 IL_00BE: pop
  0063 IL_00BF: ret
  0064 IL_00C0: ldstr "Embedded localization: zh="
  0065 IL_00C5: ldloc.0
- anchor IL[51]: IL_009A: ldstr "en"
  0034 IL_0067: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0035 IL_006C: ldstr "zh-Hans"
  0036 IL_0071: ldloc.0
  0037 IL_0072: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0038 IL_0077: ldarg.0
  0039 IL_0078: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0040 IL_007D: ldstr "zh-CN"
  0041 IL_0082: ldloc.0
  0042 IL_0083: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0043 IL_0088: ldloc.1
  0044 IL_0089: brfalse.s IL_00B6
  0045 IL_008B: ldloc.1
  0046 IL_008C: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0047 IL_0091: ldc.i4.0
  0048 IL_0092: ble.s IL_00B6
  0049 IL_0094: ldarg.0
  0050 IL_0095: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0051 IL_009A: ldstr "en"
  0052 IL_009F: ldloc.1
  0053 IL_00A0: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0054 IL_00A5: ldarg.0
  0055 IL_00A6: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0056 IL_00AB: ldstr "en-US"
  0057 IL_00B0: ldloc.1
  0058 IL_00B1: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0059 IL_00B6: ldsfld ModLog ProbablyEleanorFollowup.ModEntry::Log
  0060 IL_00BB: dup
  0061 IL_00BC: brtrue.s IL_00C0
  0062 IL_00BE: pop
  0063 IL_00BF: ret
  0064 IL_00C0: ldstr "Embedded localization: zh="
  0065 IL_00C5: ldloc.0
  0066 IL_00C6: brtrue.s IL_00CB
  0067 IL_00C8: ldc.i4.0
  0068 IL_00C9: br.s IL_00D1
  0069 IL_00CB: ldloc.0
  0070 IL_00CC: call System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0071 IL_00D1: stloc.2
  0072 IL_00D2: ldloca.s V_2
  0073 IL_00D4: call System.String System.Int32::ToString()
  0074 IL_00D9: ldstr " en="
  0075 IL_00DE: ldloc.1
  0076 IL_00DF: brtrue.s IL_00E4
- anchor IL[56]: IL_00AB: ldstr "en-US"
  0039 IL_0078: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0040 IL_007D: ldstr "zh-CN"
  0041 IL_0082: ldloc.0
  0042 IL_0083: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0043 IL_0088: ldloc.1
  0044 IL_0089: brfalse.s IL_00B6
  0045 IL_008B: ldloc.1
  0046 IL_008C: callvirt System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0047 IL_0091: ldc.i4.0
  0048 IL_0092: ble.s IL_00B6
  0049 IL_0094: ldarg.0
  0050 IL_0095: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0051 IL_009A: ldstr "en"
  0052 IL_009F: ldloc.1
  0053 IL_00A0: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0054 IL_00A5: ldarg.0
  0055 IL_00A6: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0056 IL_00AB: ldstr "en-US"
  0057 IL_00B0: ldloc.1
  0058 IL_00B1: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0059 IL_00B6: ldsfld ModLog ProbablyEleanorFollowup.ModEntry::Log
  0060 IL_00BB: dup
  0061 IL_00BC: brtrue.s IL_00C0
  0062 IL_00BE: pop
  0063 IL_00BF: ret
  0064 IL_00C0: ldstr "Embedded localization: zh="
  0065 IL_00C5: ldloc.0
  0066 IL_00C6: brtrue.s IL_00CB
  0067 IL_00C8: ldc.i4.0
  0068 IL_00C9: br.s IL_00D1
  0069 IL_00CB: ldloc.0
  0070 IL_00CC: call System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0071 IL_00D1: stloc.2
  0072 IL_00D2: ldloca.s V_2
  0073 IL_00D4: call System.String System.Int32::ToString()
  0074 IL_00D9: ldstr " en="
  0075 IL_00DE: ldloc.1
  0076 IL_00DF: brtrue.s IL_00E4
  0077 IL_00E1: ldc.i4.0
  0078 IL_00E2: br.s IL_00EA
  0079 IL_00E4: ldloc.1
  0080 IL_00E5: call System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0081 IL_00EA: stloc.2
- anchor IL[64]: IL_00C0: ldstr "Embedded localization: zh="
  0047 IL_0091: ldc.i4.0
  0048 IL_0092: ble.s IL_00B6
  0049 IL_0094: ldarg.0
  0050 IL_0095: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0051 IL_009A: ldstr "en"
  0052 IL_009F: ldloc.1
  0053 IL_00A0: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0054 IL_00A5: ldarg.0
  0055 IL_00A6: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0056 IL_00AB: ldstr "en-US"
  0057 IL_00B0: ldloc.1
  0058 IL_00B1: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0059 IL_00B6: ldsfld ModLog ProbablyEleanorFollowup.ModEntry::Log
  0060 IL_00BB: dup
  0061 IL_00BC: brtrue.s IL_00C0
  0062 IL_00BE: pop
  0063 IL_00BF: ret
  0064 IL_00C0: ldstr "Embedded localization: zh="
  0065 IL_00C5: ldloc.0
  0066 IL_00C6: brtrue.s IL_00CB
  0067 IL_00C8: ldc.i4.0
  0068 IL_00C9: br.s IL_00D1
  0069 IL_00CB: ldloc.0
  0070 IL_00CC: call System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0071 IL_00D1: stloc.2
  0072 IL_00D2: ldloca.s V_2
  0073 IL_00D4: call System.String System.Int32::ToString()
  0074 IL_00D9: ldstr " en="
  0075 IL_00DE: ldloc.1
  0076 IL_00DF: brtrue.s IL_00E4
  0077 IL_00E1: ldc.i4.0
  0078 IL_00E2: br.s IL_00EA
  0079 IL_00E4: ldloc.1
  0080 IL_00E5: call System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0081 IL_00EA: stloc.2
  0082 IL_00EB: ldloca.s V_2
  0083 IL_00ED: call System.String System.Int32::ToString()
  0084 IL_00F2: call System.String System.String::Concat(System.String,System.String,System.String,System.String)
  0085 IL_00F7: call System.Void ModLog::Log(System.Object)
  0086 IL_00FC: ret
- anchor IL[74]: IL_00D9: ldstr " en="
  0057 IL_00B0: ldloc.1
  0058 IL_00B1: callvirt System.Void System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::set_Item(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>)
  0059 IL_00B6: ldsfld ModLog ProbablyEleanorFollowup.ModEntry::Log
  0060 IL_00BB: dup
  0061 IL_00BC: brtrue.s IL_00C0
  0062 IL_00BE: pop
  0063 IL_00BF: ret
  0064 IL_00C0: ldstr "Embedded localization: zh="
  0065 IL_00C5: ldloc.0
  0066 IL_00C6: brtrue.s IL_00CB
  0067 IL_00C8: ldc.i4.0
  0068 IL_00C9: br.s IL_00D1
  0069 IL_00CB: ldloc.0
  0070 IL_00CC: call System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0071 IL_00D1: stloc.2
  0072 IL_00D2: ldloca.s V_2
  0073 IL_00D4: call System.String System.Int32::ToString()
  0074 IL_00D9: ldstr " en="
  0075 IL_00DE: ldloc.1
  0076 IL_00DF: brtrue.s IL_00E4
  0077 IL_00E1: ldc.i4.0
  0078 IL_00E2: br.s IL_00EA
  0079 IL_00E4: ldloc.1
  0080 IL_00E5: call System.Int32 System.Collections.Generic.Dictionary`2<System.String,System.String>::get_Count()
  0081 IL_00EA: stloc.2
  0082 IL_00EB: ldloca.s V_2
  0083 IL_00ED: call System.String System.Int32::ToString()
  0084 IL_00F2: call System.String System.String::Concat(System.String,System.String,System.String,System.String)
  0085 IL_00F7: call System.Void ModLog::Log(System.Object)
  0086 IL_00FC: ret
## ProbablyEleanorFollowup.EmbeddedAssets::AddLocaleAliases(06000098)
- anchor IL[10]: IL_0015: ldstr "zh"
  0000 IL_0000: ldarg.0
  0001 IL_0001: brtrue.s IL_0006
  0002 IL_0003: ldnull
  0003 IL_0004: br.s IL_000C
  0004 IL_0006: ldarg.0
  0005 IL_0007: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0006 IL_000C: brtrue.s IL_000F
  0007 IL_000E: ret
  0008 IL_000F: ldarg.0
  0009 IL_0010: ldstr "zh-Hans"
  0010 IL_0015: ldstr "zh"
  0011 IL_001A: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::Alias(LoadedMod,System.String,System.String)
  0012 IL_001F: ldarg.0
  0013 IL_0020: ldstr "zh-CN"
  0014 IL_0025: ldstr "zh"
  0015 IL_002A: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::Alias(LoadedMod,System.String,System.String)
  0016 IL_002F: ldarg.0
  0017 IL_0030: ldstr "en-US"
  0018 IL_0035: ldstr "en"
  0019 IL_003A: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::Alias(LoadedMod,System.String,System.String)
  0020 IL_003F: ret
- anchor IL[14]: IL_0025: ldstr "zh"
  0000 IL_0000: ldarg.0
  0001 IL_0001: brtrue.s IL_0006
  0002 IL_0003: ldnull
  0003 IL_0004: br.s IL_000C
  0004 IL_0006: ldarg.0
  0005 IL_0007: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0006 IL_000C: brtrue.s IL_000F
  0007 IL_000E: ret
  0008 IL_000F: ldarg.0
  0009 IL_0010: ldstr "zh-Hans"
  0010 IL_0015: ldstr "zh"
  0011 IL_001A: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::Alias(LoadedMod,System.String,System.String)
  0012 IL_001F: ldarg.0
  0013 IL_0020: ldstr "zh-CN"
  0014 IL_0025: ldstr "zh"
  0015 IL_002A: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::Alias(LoadedMod,System.String,System.String)
  0016 IL_002F: ldarg.0
  0017 IL_0030: ldstr "en-US"
  0018 IL_0035: ldstr "en"
  0019 IL_003A: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::Alias(LoadedMod,System.String,System.String)
  0020 IL_003F: ret
- anchor IL[18]: IL_0035: ldstr "en"
  0001 IL_0001: brtrue.s IL_0006
  0002 IL_0003: ldnull
  0003 IL_0004: br.s IL_000C
  0004 IL_0006: ldarg.0
  0005 IL_0007: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0006 IL_000C: brtrue.s IL_000F
  0007 IL_000E: ret
  0008 IL_000F: ldarg.0
  0009 IL_0010: ldstr "zh-Hans"
  0010 IL_0015: ldstr "zh"
  0011 IL_001A: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::Alias(LoadedMod,System.String,System.String)
  0012 IL_001F: ldarg.0
  0013 IL_0020: ldstr "zh-CN"
  0014 IL_0025: ldstr "zh"
  0015 IL_002A: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::Alias(LoadedMod,System.String,System.String)
  0016 IL_002F: ldarg.0
  0017 IL_0030: ldstr "en-US"
  0018 IL_0035: ldstr "en"
  0019 IL_003A: call System.Void ProbablyEleanorFollowup.EmbeddedAssets::Alias(LoadedMod,System.String,System.String)
  0020 IL_003F: ret
## ProbablyEleanorFollowup.Loc::HasCurrent(06000393)
- anchor IL[31]: IL_004B: ldstr "en"
  0014 IL_001E: pop
  0015 IL_001F: ldsfld ProbablyEleanorFollowup.Loc/<>c ProbablyEleanorFollowup.Loc/<>c::<>9
  0016 IL_0024: ldftn System.Boolean ProbablyEleanorFollowup.Loc/<>c::<HasCurrent>b__11_0(LoadedMod)
  0017 IL_002A: newobj System.Void System.Predicate`1<LoadedMod>::.ctor(System.Object,System.IntPtr)
  0018 IL_002F: dup
  0019 IL_0030: stsfld System.Predicate`1<LoadedMod> ProbablyEleanorFollowup.Loc/<>c::<>9__11_0
  0020 IL_0035: call LoadedMod System.Collections.Generic.List`1<LoadedMod>::Find(System.Predicate`1<LoadedMod>)
  0021 IL_003A: stloc.0
  0022 IL_003B: ldloc.0
  0023 IL_003C: brtrue.s IL_0041
  0024 IL_003E: ldnull
  0025 IL_003F: br.s IL_0047
  0026 IL_0041: ldloc.0
  0027 IL_0042: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0028 IL_0047: brtrue.s IL_004B
  0029 IL_0049: ldc.i4.0
  0030 IL_004A: ret
  0031 IL_004B: ldstr "en"
  0032 IL_0050: stloc.1
  0033 IL_0051: call UnityEngine.Localization.Locale UnityEngine.Localization.Settings.LocalizationSettings::get_SelectedLocale()
  0034 IL_0056: stloc.3
  0035 IL_0057: ldloc.3
  0036 IL_0058: ldnull
  0037 IL_0059: call System.Boolean UnityEngine.Object::op_Inequality(UnityEngine.Object,UnityEngine.Object)
  0038 IL_005E: brfalse.s IL_0070
  0039 IL_0060: ldloc.3
  0040 IL_0061: callvirt UnityEngine.Localization.LocaleIdentifier UnityEngine.Localization.Locale::get_Identifier()
  0041 IL_0066: stloc.s V_4
  0042 IL_0068: ldloca.s V_4
  0043 IL_006A: call System.String UnityEngine.Localization.LocaleIdentifier::get_Code()
  0044 IL_006F: stloc.1
  0045 IL_0070: leave.s IL_0075
  0046 IL_0072: pop
  0047 IL_0073: leave.s IL_0075
  0048 IL_0075: ldloc.0
  0049 IL_0076: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0050 IL_007B: ldloc.1
  0051 IL_007C: ldloca.s V_2
  0052 IL_007E: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::TryGetValue(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>&)
  0053 IL_0083: brfalse.s IL_008D
  0054 IL_0085: ldloc.2
  0055 IL_0086: ldarg.0
  0056 IL_0087: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,System.String>::ContainsKey(System.String)
## ProbablyEleanorFollowup.Loc::Has(06000394)
- anchor IL[31]: IL_004B: ldstr "en"
  0014 IL_001E: pop
  0015 IL_001F: ldsfld ProbablyEleanorFollowup.Loc/<>c ProbablyEleanorFollowup.Loc/<>c::<>9
  0016 IL_0024: ldftn System.Boolean ProbablyEleanorFollowup.Loc/<>c::<Has>b__12_0(LoadedMod)
  0017 IL_002A: newobj System.Void System.Predicate`1<LoadedMod>::.ctor(System.Object,System.IntPtr)
  0018 IL_002F: dup
  0019 IL_0030: stsfld System.Predicate`1<LoadedMod> ProbablyEleanorFollowup.Loc/<>c::<>9__12_0
  0020 IL_0035: call LoadedMod System.Collections.Generic.List`1<LoadedMod>::Find(System.Predicate`1<LoadedMod>)
  0021 IL_003A: stloc.0
  0022 IL_003B: ldloc.0
  0023 IL_003C: brtrue.s IL_0041
  0024 IL_003E: ldnull
  0025 IL_003F: br.s IL_0047
  0026 IL_0041: ldloc.0
  0027 IL_0042: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0028 IL_0047: brtrue.s IL_004B
  0029 IL_0049: ldc.i4.0
  0030 IL_004A: ret
  0031 IL_004B: ldstr "en"
  0032 IL_0050: stloc.1
  0033 IL_0051: call UnityEngine.Localization.Locale UnityEngine.Localization.Settings.LocalizationSettings::get_SelectedLocale()
  0034 IL_0056: stloc.s V_4
  0035 IL_0058: ldloc.s V_4
  0036 IL_005A: ldnull
  0037 IL_005B: call System.Boolean UnityEngine.Object::op_Inequality(UnityEngine.Object,UnityEngine.Object)
  0038 IL_0060: brfalse.s IL_0073
  0039 IL_0062: ldloc.s V_4
  0040 IL_0064: callvirt UnityEngine.Localization.LocaleIdentifier UnityEngine.Localization.Locale::get_Identifier()
  0041 IL_0069: stloc.s V_5
  0042 IL_006B: ldloca.s V_5
  0043 IL_006D: call System.String UnityEngine.Localization.LocaleIdentifier::get_Code()
  0044 IL_0072: stloc.1
  0045 IL_0073: leave.s IL_0078
  0046 IL_0075: pop
  0047 IL_0076: leave.s IL_0078
  0048 IL_0078: ldloc.0
  0049 IL_0079: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0050 IL_007E: ldloc.1
  0051 IL_007F: ldloca.s V_2
  0052 IL_0081: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::TryGetValue(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>&)
  0053 IL_0086: brfalse.s IL_0093
  0054 IL_0088: ldloc.2
  0055 IL_0089: ldarg.0
  0056 IL_008A: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,System.String>::ContainsKey(System.String)
- anchor IL[62]: IL_0099: ldstr "en"
  0045 IL_0073: leave.s IL_0078
  0046 IL_0075: pop
  0047 IL_0076: leave.s IL_0078
  0048 IL_0078: ldloc.0
  0049 IL_0079: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0050 IL_007E: ldloc.1
  0051 IL_007F: ldloca.s V_2
  0052 IL_0081: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::TryGetValue(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>&)
  0053 IL_0086: brfalse.s IL_0093
  0054 IL_0088: ldloc.2
  0055 IL_0089: ldarg.0
  0056 IL_008A: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,System.String>::ContainsKey(System.String)
  0057 IL_008F: brfalse.s IL_0093
  0058 IL_0091: ldc.i4.1
  0059 IL_0092: ret
  0060 IL_0093: ldloc.0
  0061 IL_0094: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0062 IL_0099: ldstr "en"
  0063 IL_009E: ldloca.s V_3
  0064 IL_00A0: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::TryGetValue(System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>&)
  0065 IL_00A5: brfalse.s IL_00B2
  0066 IL_00A7: ldloc.3
  0067 IL_00A8: ldarg.0
  0068 IL_00A9: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,System.String>::ContainsKey(System.String)
  0069 IL_00AE: brfalse.s IL_00B2
  0070 IL_00B0: ldc.i4.1
  0071 IL_00B1: ret
  0072 IL_00B2: ldloc.0
  0073 IL_00B3: ldfld System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> LoadedMod::Localization
  0074 IL_00B8: callvirt System.Collections.Generic.Dictionary`2/ValueCollection<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> System.Collections.Generic.Dictionary`2<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::get_Values()
  0075 IL_00BD: callvirt System.Collections.Generic.Dictionary`2/ValueCollection/Enumerator<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>> System.Collections.Generic.Dictionary`2/ValueCollection<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::GetEnumerator()
  0076 IL_00C2: stloc.s V_6
  0077 IL_00C4: br.s IL_00E2
  0078 IL_00C6: ldloca.s V_6
  0079 IL_00C8: call System.Collections.Generic.Dictionary`2<System.String,System.String> System.Collections.Generic.Dictionary`2/ValueCollection/Enumerator<System.String,System.Collections.Generic.Dictionary`2<System.String,System.String>>::get_Current()
  0080 IL_00CD: stloc.s V_7
  0081 IL_00CF: ldloc.s V_7
  0082 IL_00D1: brfalse.s IL_00E2
  0083 IL_00D3: ldloc.s V_7
  0084 IL_00D5: ldarg.0
  0085 IL_00D6: callvirt System.Boolean System.Collections.Generic.Dictionary`2<System.String,System.String>::ContainsKey(System.String)
  0086 IL_00DB: brfalse.s IL_00E2
  0087 IL_00DD: ldc.i4.1
