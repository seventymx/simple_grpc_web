param (
    [string]$directoryPath,
    [string]$filter = ".*(\.csproj|\.vbproj)$"
)

function Create-Solution {
    param (
        [string]$solutionName,
        [string]$solutionPath
    )
    dotnet new sln -n $solutionName -o $solutionPath
}

function Add-ProjectsToSolution {
    param (
        [string]$solutionPath,
        [string]$directoryPath
    )
    $projectFiles = Get-ChildItem -Path $directoryPath -Recurse | Where-Object { $_.Name -match $filter }
    foreach ($projectFile in $projectFiles) {
        dotnet sln $solutionPath add $projectFile.FullName
    }
}

if (-not (Test-Path $directoryPath)) {
    Write-Error "Directory does not exist."
    exit
}

$solutionName = Split-Path -Leaf $directoryPath
$solutionPath = Join-Path $directoryPath "$solutionName.sln"

Create-Solution -solutionName $solutionName -solutionPath $directoryPath
Add-ProjectsToSolution -solutionPath $solutionPath -directoryPath $directoryPath
