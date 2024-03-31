$BuildOutput = './obj/Debug/net48/LocomotiveRequest.dll'
$InfoJson = './info.json'
$LocalEnv = Get-Content './.env.json' | ConvertFrom-Json

$GameModPath = $LocalEnv.gamePath + 'Mods/LocomotiveRequest/'

$null = New-Item $GameModPath -ItemType Directory -Force
Copy-Item @($BuildOutput, $InfoJSON) $GameModPath
