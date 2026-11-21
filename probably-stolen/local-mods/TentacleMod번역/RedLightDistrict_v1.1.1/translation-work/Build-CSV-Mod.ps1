param([Parameter(Mandatory)][string]$OriginalFolder,[Parameter(Mandatory)][string]$GameManaged,[Parameter(Mandatory)][string]$OutputFolder)
$ErrorActionPreference='Stop'
$src=(Resolve-Path -LiteralPath $OriginalFolder).Path
$managed=(Resolve-Path -LiteralPath $GameManaged).Path
$out=[IO.Path]::GetFullPath($OutputFolder)
if(Test-Path -LiteralPath $out){throw 'Choose a new output folder.'}
$original=Join-Path $src 'RedLightDistrict.dll'
if((Get-FileHash -LiteralPath $original).Hash -ne '4F4F20A00E4754302C53879BFAEB7A7503DF10E2F05FF6AD3744BE31088159F8'){throw 'Expected RedLightDistrict v1.1.1.'}
$work=Join-Path $env:TEMP ('RedLightDistrict-build-'+[guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $work | Out-Null
$csc="$env:windir\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
Push-Location $work
try {
 Copy-Item -LiteralPath (Join-Path $managed 'Unity.Burst.Cecil.dll') -Destination .
 & $csc /nologo /target:library /optimize+ /nostdlib /out:CsvLookup.dll "/r:$managed\mscorlib.dll" "/r:$managed\Assembly-CSharp.dll" "/r:$managed\netstandard.dll" "/r:$managed\UnityEngine.CoreModule.dll" (Join-Path $PSScriptRoot 'CsvLookup.cs')
 if($LASTEXITCODE){throw 'CsvLookup compilation failed.'}
 & $csc /nologo /optimize+ /out:BridgeBuild.exe /r:Unity.Burst.Cecil.dll "/r:$managed\netstandard.dll" (Join-Path $PSScriptRoot 'BridgeBuild.cs')
 if($LASTEXITCODE){throw 'Bridge builder compilation failed.'}
 & .\BridgeBuild.exe $original $managed (Join-Path $work 'RedLightDistrict.dll')
 if($LASTEXITCODE){throw 'DLL build failed.'}
 $tmp=$out+'.'+[guid]::NewGuid().ToString('N')
 New-Item -ItemType Directory -Path $tmp|Out-Null
 Copy-Item -LiteralPath (Join-Path $src 'manifest.xml'),(Join-Path $src 'RedLightDistrictPerson'),(Join-Path $PSScriptRoot 'Localization') -Destination $tmp -Recurse
 Copy-Item -LiteralPath (Join-Path $work 'RedLightDistrict.dll') -Destination $tmp
 [IO.Directory]::Move($tmp,$out)
}finally{Pop-Location}
