import os, shutil
from pathlib import Path

root = Path(os.environ["RUNNER_TEMP"])
src = root / "src"
kp = src / "ProbablyStolenKoreanPack" / "KoreanPack.cs"
wn = src / "ProbablyStolenKoreanPack" / "WaterNameCompatibility.cs"

k = kp.read_text(encoding="utf-8")
k = k.replace("0.8.2-water-deferred", "0.8.2-water-postload", 1)
kp.write_text(k, encoding="utf-8")

s = wn.read_text(encoding="utf-8")

# add hook state
anchor = "\tprivate bool assetsReady;\n\n\tprivate int generation;"
if anchor not in s:
    raise SystemExit("field anchor not found")
s = s.replace(anchor, "\tprivate bool assetsReady;\n\n\tprivate bool localizationHooksReady;\n\n\tprivate int generation;", 1)

old_enable = """\tinternal void Enable()
\t{
\t\tif (!enabled)
\t\t{
\t\t\tenabled = true;
\t\t\tLocalizationSettings.SelectedLocaleChanged += LocaleChanged;
\t\t\tModHook.OnGameLoadedInit += Loading;
\t\t\tModHook.OnGameLoadedLate += Loaded;
\t\t\tinitialization = LocalizationSettings.InitializationOperation;
\t\t\tif (initialization.IsDone)
\t\t\t{
\t\t\t\tRequest();
\t\t\t}
\t\t\telse
\t\t\t{
\t\t\t\tinitialization.Completed += Initialized;
\t\t\t}
\t\t}
\t}"""
new_enable = """\tinternal void Enable()
\t{
\t\tif (!enabled)
\t\t{
\t\t\tenabled = true;
\t\t\tModHook.OnGameLoadedInit += Loading;
\t\t\tModHook.OnGameLoadedLate += Loaded;
\t\t}
\t}"""
if old_enable not in s:
    raise SystemExit("Enable block not found")
s = s.replace(old_enable, new_enable, 1)

old_disable = """\t\tenabled = false;
\t\tworldReady = false;
\t\tassetsReady = false;
\t\tgeneration++;
\t\tLocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
\t\tModHook.OnGameLoadedInit -= Loading;
\t\tModHook.OnGameLoadedLate -= Loaded;
\t\tif (initialization.IsValid())
\t\t{
\t\t\tinitialization.Completed -= Initialized;
\t\t}
\t\treplacements.Clear();"""
new_disable = """\t\tenabled = false;
\t\tworldReady = false;
\t\tassetsReady = false;
\t\tgeneration++;
\t\tModHook.OnGameLoadedInit -= Loading;
\t\tModHook.OnGameLoadedLate -= Loaded;
\t\tif (localizationHooksReady)
\t\t{
\t\t\tLocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
\t\t\tif (initialization.IsValid())
\t\t\t{
\t\t\t\tinitialization.Completed -= Initialized;
\t\t\t}
\t\t\tlocalizationHooksReady = false;
\t\t}
\t\treplacements.Clear();"""
if old_disable not in s:
    raise SystemExit("Disable block not found")
s = s.replace(old_disable, new_disable, 1)

# insert lazy localization hook method before Initialized
anchor2 = "\tprivate void Initialized(AsyncOperationHandle<LocalizationSettings> operation)"
if anchor2 not in s:
    raise SystemExit("Initialized anchor not found")
helper = """\tprivate void EnsureLocalizationHooks()
\t{
\t\tif (!enabled || !worldReady || localizationHooksReady)
\t\t{
\t\t\treturn;
\t\t}
\t\tlocalizationHooksReady = true;
\t\tLocalizationSettings.SelectedLocaleChanged += LocaleChanged;
\t\tinitialization = LocalizationSettings.InitializationOperation;
\t\tif (!initialization.IsDone)
\t\t{
\t\t\tinitialization.Completed += Initialized;
\t\t}
\t}

"""
s = s.replace(anchor2, helper + anchor2, 1)

old_request = """\tprivate void Request()
\t{
\t\tif (!enabled || !worldReady || !assetsReady || !initialization.IsValid() || !initialization.IsDone || initialization.Status != AsyncOperationStatus.Succeeded)
\t\t{
\t\t\treturn;
\t\t}"""
new_request = """\tprivate void Request()
\t{
\t\tif (!enabled || !worldReady)
\t\t{
\t\t\treturn;
\t\t}
\t\tEnsureLocalizationHooks();
\t\tif (!assetsReady || !localizationHooksReady || !initialization.IsValid() || !initialization.IsDone || initialization.Status != AsyncOperationStatus.Succeeded)
\t\t{
\t\t\treturn;
\t\t}"""
if old_request not in s:
    raise SystemExit("Request block not found")
s = s.replace(old_request, new_request, 1)

wn.write_text(s, encoding="utf-8")

build = root / "build"
(build / "ProbablyStolenKoreanPack").mkdir(parents=True, exist_ok=True)
shutil.copy2(kp, build / "ProbablyStolenKoreanPack" / "KoreanPack.cs")
shutil.copy2(wn, build / "ProbablyStolenKoreanPack" / "WaterNameCompatibility.cs")

workspace = Path(os.environ["GITHUB_WORKSPACE"])
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
    <AssemblyVersion>0.8.2.0</AssemblyVersion>
    <FileVersion>0.8.2.1</FileVersion>
    <InformationalVersion>0.8.2-water-postload</InformationalVersion>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="ProbablyStolenKoreanPack/KoreanPack.cs" />
    <Compile Include="ProbablyStolenKoreanPack/WaterNameCompatibility.cs" />
    <Reference Include="Assembly-CSharp"><HintPath>{workspace}/probably-stolen/Managed/Assembly-CSharp.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="0Harmony"><HintPath>{workspace}/probably-stolen/Managed/0Harmony.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.Localization"><HintPath>{workspace}/probably-stolen/Managed/Unity.Localization.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.ResourceManager"><HintPath>{workspace}/probably-stolen/Managed/Unity.ResourceManager.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="UnityEngine.CoreModule"><HintPath>{workspace}/probably-stolen/Managed/UnityEngine.CoreModule.dll</HintPath><Private>false</Private></Reference>
  </ItemGroup>
</Project>
'''
(build / "ProbablyStolenKoreanPack.csproj").write_text(csproj, encoding="utf-8")
print("patched: no LocalizationSettings access before worldReady")
