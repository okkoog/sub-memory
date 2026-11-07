import os, shutil
from pathlib import Path

root = Path(os.environ["RUNNER_TEMP"])
src = root / "src"
kp = src / "ProbablyStolenKoreanPack" / "KoreanPack.cs"
wn = src / "ProbablyStolenKoreanPack" / "WaterNameCompatibility.cs"

k = kp.read_text(encoding="utf-8")
k = k.replace(
    "event-based localization cache repair enabled (0.8.1-labelerfix).",
    "event-based localization cache repair enabled (0.8.2-water-deferred).",
    1
)
kp.write_text(k, encoding="utf-8")

s = wn.read_text(encoding="utf-8")
old = """\t\tif (!enabled || !assetsReady || !initialization.IsValid() || !initialization.IsDone || initialization.Status != AsyncOperationStatus.Succeeded)
\t\t{
\t\t\treturn;
\t\t}"""
new = """\t\tif (!enabled || !worldReady || !assetsReady || !initialization.IsValid() || !initialization.IsDone || initialization.Status != AsyncOperationStatus.Succeeded)
\t\t{
\t\t\treturn;
\t\t}"""
if old not in s:
    raise SystemExit("Request guard anchor not found")
s = s.replace(old, new, 1)
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
    <FileVersion>0.8.2.0</FileVersion>
    <InformationalVersion>0.8.2-water-deferred</InformationalVersion>
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
print("patched: water repair now requires worldReady")
