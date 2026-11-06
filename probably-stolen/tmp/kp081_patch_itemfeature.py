import os, shutil
from pathlib import Path

root = Path(os.environ.get('RUNNER_TEMP', '/mnt/data/kp081test'))
src = root / 'src'
p = src / 'ProbablyStolenKoreanPack' / 'WaterNameCompatibility.cs'
s = p.read_text(encoding='utf-8')
anchor = '''\t\t\tfor (int j = 0; j < array.Length; j++)
\t\t\t{
\t\t\t\treplacements[Error(QualityKeys[j])] = array[j];
\t\t\t}
\t\t\tint num = 0;'''
repl = '''\t\t\tfor (int j = 0; j < array.Length; j++)
\t\t\t{
\t\t\t\treplacements[Error(QualityKeys[j])] = array[j];
\t\t\t}
\t\t\tint featureCategories = RepairFeatureCategoryNames();
\t\t\tint num = 0;'''
if anchor not in s:
    raise SystemExit('counter insertion anchor not found')
s = s.replace(anchor, repl, 1)
oldlog = 'Debug.Log("[KoreanPack] Water name compatibility: liquids=" + num + ", conditions=" + num2 + ", saved labels=" + num3 + "; official callbacks only.");'
newlog = 'Debug.Log("[KoreanPack] Localization cache repair: feature categories=" + featureCategories + ", liquids=" + num + ", conditions=" + num2 + ", saved labels=" + num3 + "; official callbacks only.");'
if oldlog not in s:
    raise SystemExit('log anchor not found')
s = s.replace(oldlog, newlog, 1)
method = '''
\tprivate static int RepairFeatureCategoryNames()
\t{
\t\tDictionary<string, string> categoryNames = ItemFeatureList.CategoryNames;
\t\tif (categoryNames == null || categoryNames.Count == 0)
\t\t{
\t\t\treturn 0;
\t\t}
\t\tint repaired = 0;
\t\tList<string> categories = new List<string>(categoryNames.Keys);
\t\tforeach (string category in categories)
\t\t{
\t\t\tif (!categoryNames.TryGetValue(category, out string current) || !TryGetMechanicErrorKey(current, out string key))
\t\t\t{
\t\t\t\tcontinue;
\t\t\t}
\t\t\tstring localized = LocHelper.GetLocalizedMechanic(key);
\t\t\tif (Valid(localized, key))
\t\t\t{
\t\t\t\tcategoryNames[category] = localized;
\t\t\t\trepaired++;
\t\t\t}
\t\t}
\t\treturn repaired;
\t}

\tprivate static bool TryGetMechanicErrorKey(string value, out string key)
\t{
\t\tkey = null;
\t\tif (string.IsNullOrEmpty(value))
\t\t{
\t\t\treturn false;
\t\t}
\t\tconst string prefix = "Translation Error '";
\t\tconst string suffix = "' in Mechanic";
\t\tif (!value.StartsWith(prefix, StringComparison.Ordinal) || !value.EndsWith(suffix, StringComparison.Ordinal))
\t\t{
\t\t\treturn false;
\t\t}
\t\tint length = value.Length - prefix.Length - suffix.Length;
\t\tif (length <= 0)
\t\t{
\t\t\treturn false;
\t\t}
\t\tkey = value.Substring(prefix.Length, length);
\t\treturn !string.IsNullOrEmpty(key);
\t}

'''
anchor2 = '\tprivate bool RepairCondition(ItemCondition condition)'
if anchor2 not in s:
    raise SystemExit('method insertion anchor not found')
s = s.replace(anchor2, method + anchor2, 1)
p.write_text(s, encoding='utf-8')

kp = src / 'ProbablyStolenKoreanPack' / 'KoreanPack.cs'
k = kp.read_text(encoding='utf-8')
k = k.replace('event-based water name compatibility enabled (0.8.1).', 'event-based localization cache repair enabled (0.8.1-featurefix).')
kp.write_text(k, encoding='utf-8')

build = root / 'build'
(build / 'ProbablyStolenKoreanPack').mkdir(parents=True, exist_ok=True)
shutil.copy2(kp, build / 'ProbablyStolenKoreanPack' / 'KoreanPack.cs')
shutil.copy2(p, build / 'ProbablyStolenKoreanPack' / 'WaterNameCompatibility.cs')
workspace = Path(os.environ.get('GITHUB_WORKSPACE', '/workspace'))
csproj = f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <AssemblyName>ProbablyStolenKoreanPack</AssemblyName>
    <RootNamespace>ProbablyStolenKoreanPack</RootNamespace>
    <LangVersion>latest</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <Deterministic>true</Deterministic>
    <AssemblyVersion>0.8.1.0</AssemblyVersion>
    <FileVersion>0.8.1.1</FileVersion>
    <InformationalVersion>0.8.1-itemfeaturefix</InformationalVersion>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="ProbablyStolenKoreanPack/KoreanPack.cs" />
    <Compile Include="ProbablyStolenKoreanPack/WaterNameCompatibility.cs" />
    <Reference Include="Assembly-CSharp"><HintPath>{workspace}/probably-stolen/Managed/Assembly-CSharp.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.Localization"><HintPath>{workspace}/probably-stolen/Managed/Unity.Localization.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.ResourceManager"><HintPath>{workspace}/probably-stolen/Managed/Unity.ResourceManager.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="UnityEngine.CoreModule"><HintPath>{workspace}/probably-stolen/Managed/UnityEngine.CoreModule.dll</HintPath><Private>false</Private></Reference>
  </ItemGroup>
</Project>\n'''
(build / 'ProbablyStolenKoreanPack.csproj').write_text(csproj, encoding='utf-8')
print('patched source and generated build project')
