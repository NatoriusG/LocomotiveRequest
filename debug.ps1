$BuildOutput = '.\obj\Debug\net48\LocomotiveRequest.dll'
$InfoJSON = '.\info.json'
$GameModPath = 'M:\Games\Steam\steamapps\common\Derail Valley\Mods\LocomotiveRequest\'

Copy-Item -Path @($BuildOutput, $InfoJSON) -Destination $GameModPath -Force
