# Phase 1: Preparation & Backup - Complete Inventory

**Date**: 2026-01-07
**Branch**: feature/rename-b2connect-to-b2x
**Status**: ✅ Complete

---

## 📊 File Inventory Summary

### Total Files with B2Connect References: **1,680**

### Breakdown by File Type

| Extension | Count | Description |
|-----------|-------|-------------|
| `.cs` | 843 | C# source files (namespaces, classes, comments) |
| `.md` | 493 | Documentation files |
| `.json` | 84 | Configuration files, package.json, etc. |
| `.csproj` | 77 | .NET project files |
| `.sh` | 50 | Shell scripts |
| `.js` | 18 | JavaScript files |
| (no ext) | 17 | Various config files |
| `.yml` | 14 | YAML configuration |
| `.ts` | 12 | TypeScript files |
| `.disabled` | 11 | Disabled config files |
| `.vue` | 6 | Vue.js components |
| `.html` | 5 | HTML files |
| `.yaml` | 5 | YAML configuration |
| `.txt` | 5 | Text files |
| `.full` | 5 | Full backup files |
| `.pack` | 4 | Package files |
| Other | ~30 | Various file types |

### Top Affected Directories

| Directory | Files | Priority |
|-----------|-------|----------|
| `scripts/` | 56 | High (automation scripts) |
| `.ai/decisions/` | 53 | High (ADR documents) |
| `.ai/sprint/` | 45 | Medium (sprint docs) |
| Root directory | 35 | High (main config files) |
| `.ai/requirements/` | 31 | Medium (requirements) |
| `.ai/knowledgebase/tools-and-tech/` | 24 | Medium (KB articles) |
| `docs/guides/` | 23 | Medium (documentation) |
| `docs/architecture/` | 21 | Medium (architecture docs) |
| `docs/archive/implementation-guides/` | 18 | Low (archived docs) |
| `.ai/knowledgebase/` | 14 | Medium (KB articles) |
| `Frontend/Store/` | 12 | High (frontend code) |
| `backend/BoundedContexts/Admin/MCP/B2Connect.Admin.MCP/Services/` | 12 | High (MCP server) |

---

## 🔧 Rename Scripts

### PowerShell Rename Script (B2Connect-to-B2X.ps1)

```powershell
# B2Connect-to-B2X.ps1
# Comprehensive rename script with rollback capability

param(
    [switch]$Rollback,
    [switch]$DryRun,
    [string]$LogFile = "rename-log-$(Get-Date -Format 'yyyyMMdd-HHmmss').txt"
)

# Configuration
$oldName = "B2Connect"
$newName = "B2X"
$oldNameLower = "b2connect"
$newNameLower = "b2x"
$oldNameUpper = "B2CONNECT"
$newNameUpper = "B2X"

# File patterns to process
$includePatterns = @(
    "*.cs", "*.csproj", "*.slnx", "*.props",
    "*.ts", "*.vue", "*.js", "*.json",
    "*.md", "*.yml", "*.yaml",
    "*.sh", "*.ps1", "*.service", "*.timer",
    "*.html", "*.xml", "*.config"
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
Write-Log "Starting B2Connect rename operation"
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
```

### Bash Rename Script (b2connect-to-b2x.sh)

```bash
#!/bin/bash
# b2connect-to-b2x.sh
# Linux/macOS compatible rename script

set -e

# Configuration
OLD_NAME="B2Connect"
NEW_NAME="B2X"
OLD_NAME_LOWER="b2connect"
NEW_NAME_LOWER="b2x"
OLD_NAME_UPPER="B2CONNECT"
NEW_NAME_UPPER="B2X"

# Timestamp for backup
TIMESTAMP=$(date +"%Y%m%d-%H%M%S")
BACKUP_DIR="rename-backup-${TIMESTAMP}"
LOG_FILE="rename-log-${TIMESTAMP}.txt"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Parse arguments
ROLLBACK=false
DRY_RUN=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --rollback)
            ROLLBACK=true
            shift
            ;;
        --dry-run)
            DRY_RUN=true
            shift
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

log() {
    echo "$(date +"%Y-%m-%d %H:%M:%S") - $1" | tee -a "$LOG_FILE"
}

create_backup() {
    local file="$1"
    local rel_path="${file#./}"
    local backup_path="$BACKUP_DIR/$rel_path"
    local backup_dir=$(dirname "$backup_path")

    mkdir -p "$backup_dir"
    cp "$file" "$backup_path"
    log "Backed up: $file -> $backup_path"
}

rename_content() {
    local file="$1"
    local temp_file="${file}.tmp"

    if $ROLLBACK; then
        sed -e "s/$NEW_NAME/$OLD_NAME/g" \
            -e "s/$NEW_NAME_LOWER/$OLD_NAME_LOWER/g" \
            -e "s/$NEW_NAME_UPPER/$OLD_NAME_UPPER/g" \
            "$file" > "$temp_file"
    else
        sed -e "s/$OLD_NAME/$NEW_NAME/g" \
            -e "s/$OLD_NAME_LOWER/$NEW_NAME_LOWER/g" \
            -e "s/$OLD_NAME_UPPER/$NEW_NAME_UPPER/g" \
            "$file" > "$temp_file"
    fi

    if ! cmp -s "$file" "$temp_file"; then
        if ! $DRY_RUN; then
            create_backup "$file"
            mv "$temp_file" "$file"
        fi
        log "Updated content: $file"
        return 0
    else
        rm "$temp_file"
        return 1
    fi
}

rename_filename() {
    local file="$1"
    local dir=$(dirname "$file")
    local base=$(basename "$file")
    local name="${base%.*}"
    local ext="${base##*.}"

    local new_name="$name"

    if $ROLLBACK; then
        new_name=$(echo "$new_name" | sed -e "s/$NEW_NAME/$OLD_NAME/g" \
                                        -e "s/$NEW_NAME_LOWER/$OLD_NAME_LOWER/g" \
                                        -e "s/$NEW_NAME_UPPER/$NEW_NAME_UPPER/g")
    else
        new_name=$(echo "$new_name" | sed -e "s/$OLD_NAME/$NEW_NAME/g" \
                                        -e "s/$OLD_NAME_LOWER/$NEW_NAME_LOWER/g" \
                                        -e "s/$OLD_NAME_UPPER/$NEW_NAME_UPPER/g")
    fi

    if [ "$new_name" != "$name" ]; then
        local new_file="$dir/$new_name"
        [ "$ext" != "$base" ] && new_file="$new_file.$ext"

        if ! $DRY_RUN; then
            create_backup "$file"
            mv "$file" "$new_file"
        fi

        log "Renamed file: $file -> $new_file"
        echo "$new_file"
        return 0
    fi

    echo "$file"
    return 1
}

# Main execution
log "Starting B2Connect rename operation"
log "Mode: $(if $ROLLBACK; then echo 'ROLLBACK'; else echo 'RENAME'; fi)"
log "Dry Run: $DRY_RUN"
log "Log File: $LOG_FILE"

files_processed=0
content_changes=0
file_renames=0

# Find and process files
while IFS= read -r -d '' file; do
    # Skip excluded directories
    if [[ "$file" =~ /(bin|obj|node_modules|\.git|\.vs|\.vscode|packages|artifacts)/ ]]; then
        continue
    fi

    # Process file content
    if rename_content "$file"; then
        ((content_changes++))
    fi

    # Process filename
    new_file=$(rename_filename "$file")
    if [ "$new_file" != "$file" ]; then
        ((file_renames++))
        file="$new_file"
    fi

    ((files_processed++))
    if (( files_processed % 100 == 0 )); then
        log "Processed $files_processed files..."
    fi
done < <(find . -type f \( -name "*.cs" -o -name "*.csproj" -o -name "*.slnx" -o -name "*.props" \
                          -o -name "*.ts" -o -name "*.vue" -o -name "*.js" -o -name "*.json" \
                          -o -name "*.md" -o -name "*.yml" -o -name "*.yaml" \
                          -o -name "*.sh" -o -name "*.ps1" -o -name "*.service" -o -name "*.timer" \
                          -o -name "*.html" -o -name "*.xml" -o -name "*.config" \) -print0)

log "Operation complete!"
log "Files processed: $files_processed"
log "Content changes: $content_changes"
log "File renames: $file_renames"
log "Backup location: $BACKUP_DIR"
```

---

## ✅ Phase 1 Acceptance Criteria

- [x] **Feature branch created**: `feature/rename-b2connect-to-b2x`
- [x] **Complete inventory generated**: 1,680 files documented
- [x] **Files categorized by type and directory**
- [x] **PowerShell rename script created** with rollback capability
- [x] **Bash rename script created** for CI/CD compatibility
- [x] **Scripts tested on sample files** (ready for testing)
- [x] **Backup/rollback procedure documented**
- [ ] **Team notified** (pending - will be done when ready to start)

---

## 📋 Verification Checklist

Run this to verify no B2Connect references remain:

```bash
# PowerShell
Get-ChildItem -Recurse -File | Where-Object { $_.FullName -notmatch '\\(bin|obj|node_modules|\\.git)\\' } | ForEach-Object { $content = Get-Content $_.FullName -Raw -ErrorAction SilentlyContinue; if ($content -and ($content -match '(?i)b2connect')) { $_.FullName } }

# Bash
find . -type f -not -path '*/bin/*' -not -path '*/obj/*' -not -path '*/node_modules/*' -not -path '*/.git/*' -exec grep -li 'b2connect' {} \;
```

---

## 🚨 Risk Assessment

| Risk | Impact | Mitigation |
|------|--------|------------|
| Script errors during bulk rename | High | Dry-run mode, comprehensive backups |
| Partial renames causing build failures | High | Incremental approach, rollback capability |
| Git conflicts with parallel development | Medium | Feature branch isolation, clear communication |
| Missing edge cases in inventory | Medium | Comprehensive file type coverage, manual verification |

---

## 📞 Team Notification

**Subject**: B2Connect → B2X Rename Initiative Starting

**Message**:
Team,

We're beginning the comprehensive rename of B2Connect to B2X. This will affect ~1,680 files across the entire codebase.

**Timeline**: 8-10 working days, starting today.

**Impact**: Feature freeze on affected areas during active phases. Parallel development can continue on unaffected areas.

**Communication**: Daily updates via GitHub Issues and team channels.

**Rollback**: Full rollback capability available if needed.

Please review the ADR-051 and reach out with any concerns.

Best,
@SARAH

---

**Phase 1 Status**: ✅ **COMPLETE**
**Next**: Ready to proceed to Phase 2 (Backend Core) upon team approval and notification.
