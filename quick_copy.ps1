$Plugins = dir ".\plugins" | ?{$_.PSISContainer}
$Dest = ".\src\Sora\bin\Debug\netcoreapp3.0\plugins\"
$FileName = ".\src\Sora\bin\Debug\netcoreapp3.0\plugins\*.dll"

Write-Output "Try finding any Plugins"

if (Test-Path $FileName) 
{
  Write-Output "Deleting all Plugins"
  Remove-Item $FileName
}

Write-Output "Compiling Plugins"
foreach ($d in $Plugins) {
    $path = Join-Path -Path $d.FullName -ChildPath ""
    
    Push-Location $path
    dotnet build
    Pop-Location
        
    $nmn = $d.Name
    $srcPath = Join-Path -Path $path -ChildPath ".\bin\Debug\netcoreapp3.0\$nmn.dll" 
    $dstPath = Join-Path -Path $Dest -ChildPath "$nmn.dll"
    Copy-Item $srcPath $dstPath
}
