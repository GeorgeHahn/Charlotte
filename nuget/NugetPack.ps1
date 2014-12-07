$proj = "Charlotte"
$root = $env:APPVEYOR_BUILD_FOLDER
$file = "$root\$($proj)\bin\$($env:CONFIGURATION)\$($proj).dll"
$versionStr = "$($env:APPVEYOR_BUILD_VERSION)"

Write-Host "Setting .nuspec version tag to $versionStr"

$content = (Get-Content $root\nuget\$proj.nuspec) 
$content = $content -replace '\$version\$',$versionStr
$content = $content -replace '\$file\$',$file

$content | Out-File $env:APPVEYOR_BUILD_FOLDER\nuget\$proj.compiled.nuspec

& nuget pack $root\nuget\$proj.compiled.nuspec -OutputDirectory ..\out