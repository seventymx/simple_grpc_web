param (
    [string]$ResourceFilesPath,
    [string]$OutputPath
)

$directoryPath = Resolve-Path -Path "$PSScriptRoot"

if (-not $ResourceFilesPath) {
    $ResourceFilesPath = Resolve-Path -Path (Join-Path -Path $directoryPath -ChildPath "../backend/SPTrialCommonCS/Resources/")
}

if (-not $OutputPath) {
    $OutputPath = Join-Path -Path $directoryPath -ChildPath "../frontend/src/generated"
}

$resourceFiles = Get-ChildItem -Path $ResourceFilesPath -Include *.svg -Recurse

$enumString = "const ResourceName = Object.freeze({`n"

foreach ($file in $resourceFiles) {
    $name = [IO.Path]::GetFileNameWithoutExtension($file.FullName)
    $enumString += "    ${name}: ""$name"",`n"
}

$enumString += "});`n`n"

$enumString += "export default ResourceName;`n"

$outputFileName = "resource_name.js"
$outputFilePath = Join-Path -Path $OutputPath -ChildPath $outputFileName

$enumString | Out-File -FilePath $outputFilePath