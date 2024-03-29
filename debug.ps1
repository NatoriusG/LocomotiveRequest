$BuildOutput = '.\obj\Debug\net48\LocomotiveRequest.dll'
$GameModPath = 'M:\Games\Steam\steamapps\common\Derail Valley\Mods\LocomotiveRequest\'

Copy-Item -Path $BuildOutput -Destination $GameModPath -Force
