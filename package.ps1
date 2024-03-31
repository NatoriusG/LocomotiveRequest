# Recompile for release
Invoke-Expression 'dotnet build -c Release'

# Collect files for release
$BuildOutput = './obj/Release/net48/LocomotiveRequest.dll'
$ModFiles = @($BuildOutput, './info.json', './LICENSE.md', './README.md')
$PackageDirectory = New-Item './package' -ItemType Directory -Force
Copy-Item $ModFiles $PackageDirectory

# Zip files
Invoke-Expression '7z a ./release.7z ./package/*'
