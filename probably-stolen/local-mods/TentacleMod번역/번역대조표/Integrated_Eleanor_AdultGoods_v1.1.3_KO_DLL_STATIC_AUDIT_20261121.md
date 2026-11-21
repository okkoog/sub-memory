# Integrated Eleanor and Adult Goods v1.1.3 — Korean DLL static audit

- Source: probably-stolen/local-mods/TentacleMod/3843142171/fallen_heiress_eleanor.dll, original SHA256 221f28b4e9492af24d4fa13aad995bd46eb0b759bf6777ad07fa02aad2d6f548; original bytes 16721920
- Patched: probably-stolen/local-mods/TentacleMod번역/3843142171/fallen_heiress_eleanor.dll, SHA256 1e3615e895a532d34539a18157811053e773b502d3a56c096001bb905934ff5e; bytes 16927744
- Original 104 managed resources are byte-identical (including original Chinese/English for BOTH modules). Added resources exactly 3: Eleanor KO, AdultGoods KO, AdultGoods KO-KR.
- Original CLR TypeDef 321 and MethodDef 1918 preserved. Prior v1.0.3 Eleanor source zh/en resources exactly match this integrated DLL; AdultGoods v1.1.1 source zh/en resources likewise exactly match.
- Eleanor 1839 keys, 1553 individually reviewed Korean translations, 286 original inactive placeholders. Added IL: 14 to register ko and ko-KR; original English and Chinese unmodified.
- Adult Goods 181 verified Korean values, two independent resource aliases dynamically loaded by original EmbeddedAssets.Inject. Patched exactly 10 display-only DragonCase/DragonDisplayCase tooltip ldstr callsites.
- Original packaged native capability and native payload.txt were not executed, replaced, or removed. ZIP is a DLL-only overlay for existing original v1.1.3 installation; not a complete replacement mod. Do not combine with different probably_adult_goods ID copies or separate Eleanor follow-up.
- Unity/CLR runtime, native payload, game UI, save data and compatibility were NOT TESTED. Static success alone does not establish loading/security.
