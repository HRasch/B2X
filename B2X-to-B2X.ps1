# B2X-to-B2X.ps1
# Comprehensive rename script with rollback capability

param(
    [switch]$Rollback,
    [switch]$DryRun,
    [string]$LogFile = "rename-log-$(Get-Date -Format 'yyyyMMdd-HHmmss').txt"
)

# Configuration
$oldName = "B2X"
$newName = "B2X"
$oldNameLower = "B2X"
$newNameLower = "b2x"
$oldNameUpper = "B2X"
$newNameUpper = "B2X"

# File patterns to process
$includePatterns = @(
    ".cs", ".csproj", ".slnx", ".props",
    ".ts", ".vue", ".js", ".json",
    ".md", ".yml", ".yaml",
    ".sh", ".ps1", ".service", ".timer",
    ".html", ".xml", ".config"
)

# Directories to exclude
$excludeDirs = @(
    "bin", "obj", "node_modules", ".git",
    ".vs", ".vscode", "packages", "artifacts"
)

# Backup directory
$backupDir = "rename-backup-$(Get-Date -Format 'yyyyMMdd-HHmmss')"

function Write-Log {
    param([string]$Message)
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    "$timestamp - $Message" | Out-File -FilePath $LogFile -Append -Encoding UTF8
    Write-Host $Message
}

function Create-Backup {
    param([string]$FilePath)

    if (-not (Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir | Out-Null
    }

    $relativePath = $FilePath.Replace($PWD.Path + "\", "")
    $backupPath = Join-Path $backupDir $relativePath

    $backupDirPath = Split-Path $backupPath -Parent
    if (-not (Test-Path $backupDirPath)) {
        New-Item -ItemType Directory -Path $backupDirPath -Force | Out-Null
    }

    Copy-Item $FilePath $backupPath -Force
    Write-Log "Backed up: $FilePath -> $backupPath"
}

function Rename-FileContent {
    param([string]$FilePath)

    try {
        $content = Get-Content $FilePath -Raw -Encoding UTF8

        if ($Rollback) {
            $originalContent = $content
            $content = $content -replace $newName, $oldName
            $content = $content -replace $newNameLower, $oldNameLower
            $content = $content -replace $newNameUpper, $oldNameUpper
        } else {
            $originalContent = $content
            $content = $content -replace $oldName, $newName
            $content = $content -replace $oldNameLower, $newNameLower
            $content = $content -replace $oldNameUpper, $oldNameUpper
        }

        if ($content -ne $originalContent) {
            if (-not $DryRun) {
                Create-Backup $FilePath
                $content | Out-File -FilePath $FilePath -Encoding UTF8 -NoNewline
            }
            Write-Log "Updated content: $FilePath"
            return $true
        }
    } catch {
        Write-Log "Error processing $FilePath : $_"
    }

    return $false
}

function Rename-FileName {
    param([string]$FilePath)

    $fileName = [System.IO.Path]::GetFileNameWithoutExtension($FilePath)
    $extension = [System.IO.Path]::GetExtension($FilePath)
    $directory = [System.IO.Path]::GetDirectoryName($FilePath)

    $newFileName = $fileName
    if ($Rollback) {
        $newFileName = $newFileName -replace $newName, $oldName
        $newFileName = $newFileName -replace $newNameLower, $oldNameLower
        $newFileName = $newFileName -replace $newNameUpper, $oldNameUpper
    } else {
        $newFileName = $newFileName -replace $oldName, $newName
        $newFileName = $newFileName -replace $oldNameLower, $oldNameLower
        $newFileName = $newFileName -replace $oldNameUpper, $oldNameUpper
    }

    if ($newFileName -ne $fileName) {
        $newFilePath = Join-Path $directory ($newFileName + $extension)

        if (-not $DryRun) {
            Create-Backup $FilePath
            Move-Item $FilePath $newFilePath -Force
        }

        Write-Log "Renamed file: $FilePath -> $newFilePath"
        return $newFilePath
    }

    return $FilePath
}

# Main execution
Write-Log "Starting B2X rename operation"
Write-Log "Mode: $(if ($Rollback) { 'ROLLBACK' } else { 'RENAME' })"
Write-Log "Dry Run: $DryRun"
Write-Log "Log File: $LogFile"

$filesProcessed = 0
$contentChanges = 0
$fileRenames = 0

Get-ChildItem -Recurse -File | Where-Object {
    $includePatterns -contains $_.Extension -or
    ($_.Extension -eq "" -and $includePatterns -contains "(no ext)") -and
    -not ($excludeDirs | Where-Object { $_.FullName -match "\\$_\\" })
} | ForEach-Object {
    $filePath = $_.FullName

    # Rename file content
    if (Rename-FileContent $filePath) {
        $contentChanges++
    }

    # Rename file name
    $newPath = Rename-FileName $filePath
    if ($newPath -ne $filePath) {
        $fileRenames++
        $filePath = $newPath
    }

    $filesProcessed++
    if ($filesProcessed % 100 -eq 0) {
        Write-Log "Processed $filesProcessed files..."
    }
}

Write-Log "Operation complete!"
Write-Log "Files processed: $filesProcessed"
Write-Log "Content changes: $contentChanges"
Write-Log "File renames: $fileRenames"
Write-Log "Backup location: $backupDir"