param(
    [Parameter(Mandatory=$true)]
    [string]$MappingFile,

    [Parameter(Mandatory=$true)]
    [string]$TargetPath,

    [switch]$DryRun
)

# Load namespace mapping
$mapping = Get-Content $MappingFile | ConvertFrom-Json

Write-Host "Loaded $($mapping.PSObject.Properties.Count) namespace mappings"

# Find all .cs files
$csFiles = Get-ChildItem -Path $TargetPath -Filter "*.cs" -Recurse

$filesModified = 0
$changesMade = 0

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    $fileModified = $false

    foreach ($prop in $mapping.PSObject.Properties) {
        $oldNamespace = $prop.Name
        $newNamespace = $prop.Value

        # Update namespace declarations
        if ($content -match "namespace $oldNamespace") {
            $newContent = $content -replace "namespace $oldNamespace", "namespace $newNamespace"
            if ($newContent -ne $content) {
                $content = $newContent
                $fileModified = $true
                $changesMade++
                Write-Host "  Updated namespace: $oldNamespace → $newNamespace"
            }
        }

        # Update using directives
        if ($content -match "using $oldNamespace") {
            $newContent = $content -replace "using $oldNamespace", "using $newNamespace"
            if ($newContent -ne $content) {
                $content = $newContent
                $fileModified = $true
                $changesMade++
                Write-Host "  Updated using directive: $oldNamespace → $newNamespace"
            }
        }

        # Update fully qualified namespace references
        if ($content -match "$oldNamespace\.") {
            $newContent = $content -replace "$oldNamespace\.", "$newNamespace."
            if ($newContent -ne $content) {
                $content = $newContent
                $fileModified = $true
                $changesMade++
                Write-Host "  Updated qualified reference: $oldNamespace. → $newNamespace."
            }
        }
    }

    if ($fileModified) {
        $filesModified++
        Write-Host "Modified: $($file.FullName)"
        if (-not $DryRun) {
            Set-Content -Path $file.FullName -Value $content -Encoding UTF8
        }
    }
}

Write-Host "`nSummary:"
Write-Host "  Files modified: $filesModified"
Write-Host "  Total changes: $changesMade"
if ($DryRun) {
    Write-Host "  DRY RUN - No files were actually modified"
} else {
    Write-Host "  All changes applied"
}