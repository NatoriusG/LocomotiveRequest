$BuildOutput = './obj/Debug/net48/LocomotiveRequest.dll'
$InfoJSON = './info.json'
$LocalEnv = Get-Content -Path './.env.json' | ConvertFrom-Json

$GameModPath = $LocalEnv.gamePath + 'Mods/LocomotiveRequest/'

$null = New-Item -Path $GameModPath -ItemType Directory -Force
Copy-Item -Path @($BuildOutput, $InfoJSON) -Destination $GameModPath
