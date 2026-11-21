param([Parameter(Mandatory)][string]$Original,[Parameter(Mandatory)][string]$Managed,[Parameter(Mandatory)][string]$Output)
$ErrorActionPreference='Stop'
if(Test-Path -LiteralPath $Output){throw 'Choose a new DLL output path.'}
if((Get-FileHash -LiteralPath $Original).Hash -ne '4F4F20A00E4754302C53879BFAEB7A7503DF10E2F05FF6AD3744BE31088159F8'){throw 'Unexpected input DLL'}
$a=[Reflection.Assembly]::LoadFrom((Join-Path $Managed 'Unity.Burst.Cecil.dll'))
$t=$a.GetTypes() | Where-Object Name -eq AssemblyDefinition | Select-Object -First 1
$m=$t::ReadAssembly((Resolve-Path -LiteralPath $Original).Path)
$m.MainModule.AssemblyResolver.AddSearchDirectory($Managed)
$rows=Get-Content -LiteralPath (Join-Path $PSScriptRoot 'batch01.json') -Encoding UTF8 -Raw | ConvertFrom-Json
$cls=$m.MainModule.Types | Where-Object FullName -ceq 'RedLightDistrict.BallMinigame'
foreach($row in $rows){
    $method=$cls.Methods | Where-Object FullName -ceq $row.method
    $i=$method.Body.Instructions | Where-Object Offset -eq $row.offset
    if($i.OpCode.Name -ne 'ldstr' -or $i.Operand -cne $row.source){throw "Unexpected source at $($row.method) + $($row.offset)"}
    $i.Operand=$row.korean
}
$m.Write([IO.Path]::GetFullPath($Output))
