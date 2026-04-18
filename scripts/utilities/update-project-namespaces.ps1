param(
    [Parameter(Mandatory=$true)]
    [string]$MappingFile,

    [Parameter(Mandatory=$true)]
    [string]$TargetPath
)

# Load namespace mapping
$mapping = Get-Content $MappingFile | ConvertFrom-Json

Write-Host "Loaded $($mapping.PSObject.Properties.Count) namespace mappings"

# Find all .csproj files
$csprojFiles = Get-ChildItem -Path $TargetPath -Filter "*.csproj" -Recurse

$filesModified = 0
$changesMade = 0

foreach ($file in $csprojFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    $fileModified = $false

    foreach ($prop in $mapping.PSObject.Properties) {
        $oldNamespace = $prop.Name
        $newNamespace = $prop.Value

        # Update RootNamespace
        if ($content -match "<RootNamespace>$oldNamespace") {
            $newContent = $content -replace "<RootNamespace>$oldNamespace", "<RootNamespace>$newNamespace"
            if ($newContent -ne $content) {
                $content = $newContent
                $fileModified = $true
                $changesMade++
                Write-Host "  Updated RootNamespace: $oldNamespace → $newNamespace"
            }
        }

        # Update AssemblyName if it matches the old namespace
        if ($content -match "<AssemblyName>$oldNamespace") {
            $newContent = $content -replace "<AssemblyName>$oldNamespace", "<AssemblyName>$newNamespace"
            if ($newContent -ne $content) {
                $content = $newContent
                $fileModified = $true
                $changesMade++
                Write-Host "  Updated AssemblyName: $oldNamespace → $newNamespace"
            }
        }

        # Update PackageId if it matches the old namespace
        if ($content -match "<PackageId>$oldNamespace") {
            $newContent = $content -replace "<PackageId>$oldNamespace", "<PackageId>$newNamespace"
            if ($newContent -ne $content) {
                $content = $newContent
                $fileModified = $true
                $changesMade++
                Write-Host "  Updated PackageId: $oldNamespace → $newNamespace"
            }
        }
    }

    if ($fileModified) {
        $filesModified++
        Write-Host "Modified: $($file.FullName)"
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8
    }
}

Write-Host "`nSummary:"
Write-Host "  Project files modified: $filesModified"
Write-Host "  Total changes: $changesMade"