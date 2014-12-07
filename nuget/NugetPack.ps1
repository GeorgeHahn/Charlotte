$proj = "Charlotte"
$file = $proj\bin\$env:CONFIGURATION\$proj.dll
$versionStr = $env:APPVEYOR_BUILD_VERSION

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\nuget\$proj.nuspec) 
$content = $content -replace '\$version\$',$versionStr
$content = $content -replace '\$file\$',$file

$content | Out-File $root\nuget\$proj.compiled.nuspec

& nuget pack $root\nuget\$proj.compiled.nuspec