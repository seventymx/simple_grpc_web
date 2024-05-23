param(
    [string]$projectFile
)

$codeAnalysisCommon = "Microsoft.CodeAnalysis.Common"
$codeAnalysisCSharp = "Microsoft.CodeAnalysis.CSharp"
$codeAnalysisWorkspaces = "Microsoft.CodeAnalysis.Workspaces.Common"

. "$PSScriptRoot/GetNugetVersion.ps1"
$versions = Get-NugetVersion -projectFile $projectFile -assemblyNames $codeAnalysisCommon, $codeAnalysisCSharp, $codeAnalysisWorkspaces

. "$PSScriptRoot/GetNugetResourcePath.ps1"
$codeAnalysisCommonPath = Get-NugetResourcePath -assemblyName $codeAnalysisCommon -version $versions[$codeAnalysisCommon]
$codeAnalysisCSharpPath = Get-NugetResourcePath -assemblyName $codeAnalysisCSharp -version $versions[$codeAnalysisCSharp]
$codeAnalysisWorkspacesPath = Get-NugetResourcePath -assemblyName $codeAnalysisWorkspaces -version $versions[$codeAnalysisWorkspaces]

$codeAnalysisCommonPath = $codeAnalysisCommonPath -replace ".Common.dll", ".dll"
$codeAnalysisWorkspacesPath = $codeAnalysisWorkspacesPath -replace ".Common.dll", ".dll"

Add-Type -Path $codeAnalysisCommonPath
Add-Type -Path $codeAnalysisCSharpPath
Add-Type -Path $codeAnalysisWorkspacesPath

$filePath = "./ResourceAccessor.cs"
$fileContent = Get-Content -Path $filePath -Raw

$tree = [Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree]::ParseText($fileContent)

$root = $tree.GetRoot()

$enumDeclaration = $root.DescendantNodes() | Where-Object {
    $_ -is [Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax] -and
    $_.Identifier.ValueText -eq "ResourceName"
}

$newMembersList = New-Object "System.Collections.Generic.List[Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax]"

$resourceDir = "./Resources"
$resourceDir = (Get-Item -Path $resourceDir).FullName
Get-ChildItem -Path $resourceDir -Include *.png, *.svg -Recurse | ForEach-Object {
    $name = [System.IO.Path]::GetFileNameWithoutExtension($_.FullName)

    $dir = [System.IO.Path]::GetDirectoryName($_.FullName)

    if ($dir -ne $resourceDir) {
        $subDir = [System.IO.Path]::GetFileName($dir)
        
        $name = "${subDir}_$name"
    }

    $member = [Microsoft.CodeAnalysis.CSharp.SyntaxFactory]::EnumMemberDeclaration($name)
    $newMembersList.Add($member)
}

$separatedSyntaxList = [Microsoft.CodeAnalysis.CSharp.SyntaxFactory]::SeparatedList([System.Collections.Generic.IEnumerable[Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax]]$newMembersList)

$newEnumDeclaration = $enumDeclaration.WithMembers($separatedSyntaxList)

$newRoot = [Microsoft.CodeAnalysis.SyntaxNodeExtensions]::ReplaceNode($root, $enumDeclaration, $newEnumDeclaration)

$formattedRoot = [Microsoft.CodeAnalysis.SyntaxNodeExtensions]::NormalizeWhitespace($newRoot, "    ", "`n", $true);

$newCode = $formattedRoot.ToFullString()

$newCode = [System.Text.RegularExpressions.Regex]::Replace($newCode, "^(namespace\s.*)$", "`$1`n", [System.Text.RegularExpressions.RegexOptions]::Multiline)

$newCode = [System.Text.RegularExpressions.Regex]::Replace($newCode, "\)(is\s\{)", ") `$1")

Set-Content -Path $filePath -Value $newCode -Force -NoNewline

Write-Host "ResourceName enum updated successfully in '$filePath'."