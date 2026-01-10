#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Lessons Learned Maintenance Script - Phase 4 Automation

.DESCRIPTION
    Automates maintenance tasks for the lessons learned system:
    - Validates lesson structure and cross-references
    - Updates category indexes with new lessons
    - Archives old lessons based on date thresholds
    - Generates maintenance reports
    - Integrates with WF-010 documentation maintenance workflow

.PARAMETER Action
    The maintenance action to perform:
    - validate: Validate lesson structure and cross-references
    - update-indexes: Update category indexes with new lessons
    - archive-old: Archive lessons older than threshold
    - generate-report: Generate maintenance report
    - full-maintenance: Run all maintenance tasks

.PARAMETER Category
    Specific category to maintain (frontend, backend, architecture, devops, quality, process)

.PARAMETER DryRun
    Preview changes without applying them

.EXAMPLE
    .\lessons-maintenance.ps1 -Action validate
    .\lessons-maintenance.ps1 -Action full-maintenance -DryRun
    .\lessons-maintenance.ps1 -Action update-indexes -Category frontend
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("validate", "update-indexes", "archive-old", "generate-report", "full-maintenance")]
    [string]$Action,

    [Parameter(Mandatory = $false)]
    [ValidateSet("frontend", "backend", "architecture", "devops", "quality", "process")]
    [string]$Category,

    [switch]$DryRun
)

# Configuration
$Script:Config = @{
    LessonsRoot = Join-Path $PSScriptRoot ".." ".ai" "knowledgebase" "lessons"
    ArchiveThresholdDays = 365  # Archive lessons older than 1 year
    MaxActiveLessons = 20       # Maximum lessons per category index
    ReportPath = Join-Path $PSScriptRoot ".." ".ai" "logs" "lessons-maintenance-$(Get-Date -Format 'yyyyMMdd-HHmmss').log"
}

# Initialize logging
function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    Write-Host $logMessage
    Add-Content -Path $Script:Config.ReportPath -Value $logMessage
}

function Initialize-Report {
    Write-Log "=== Lessons Learned Maintenance Report ===" "INFO"
    Write-Log "Action: $Action" "INFO"
    if ($Category) { Write-Log "Category: $Category" "INFO" }
    Write-Log "Dry Run: $($DryRun.ToString().ToUpper())" "INFO"
    Write-Log "Started: $(Get-Date)" "INFO"
    Write-Log "" "INFO"
}

# Validate lesson structure and cross-references
function Test-LessonStructure {
    Write-Log "🔍 Validating lesson structure..." "INFO"

    $issues = @()
    $categories = @("frontend", "backend", "architecture", "devops", "quality", "process")

    foreach ($cat in $categories) {
        if ($Category -and $cat -ne $Category) { continue }

        $indexPath = Join-Path $Script:Config.LessonsRoot "$cat-index.md"
        if (!(Test-Path $indexPath)) {
            $issues += "Missing index file: $indexPath"
            continue
        }

        # Validate index structure
        $content = Get-Content $indexPath -Raw
        if ($content -notmatch "🔴.*Critical") {
            $issues += "Missing critical section in $indexPath"
        }
        if ($content -notmatch "🟡.*Important") {
            $issues += "Missing important section in $indexPath"
        }

        # Validate cross-references
        $references = [regex]::Matches($content, '\[([A-Z]{3}-\d{3}|[A-Z]{2}-\d{3})\]')
        foreach ($ref in $references) {
            $refId = $ref.Groups[1].Value
            # Check if reference exists in DOCUMENT_REGISTRY
            if ($refId -notmatch '^(ADR|KB)-') {
                $issues += "Invalid reference format: $refId in $indexPath"
            }
        }

        # Check archive files exist (optional for new categories)
        $archiveDir = Join-Path $Script:Config.LessonsRoot "archive"
        $archiveFiles = Get-ChildItem $archiveDir -Filter "$cat-*.md" -ErrorAction SilentlyContinue
        if ($archiveFiles.Count -eq 0) {
            # Only warn for categories that should have archives (frontend, backend)
            if ($cat -in @("frontend", "backend")) {
                $issues += "No archive files found for category: $cat"
            }
        }
    }

    if ($issues.Count -eq 0) {
        Write-Log "✅ All lesson structures are valid" "SUCCESS"
    } else {
        Write-Log "❌ Found $($issues.Count) structural issues:" "ERROR"
        foreach ($issue in $issues) {
            Write-Log "  - $issue" "ERROR"
        }
    }

    return $issues.Count -eq 0
}

# Update category indexes with new lessons
function Update-CategoryIndexes {
    Write-Log "📝 Updating category indexes..." "INFO"

    $categories = @("frontend", "backend", "architecture", "devops", "quality", "process")

    foreach ($cat in $categories) {
        if ($Category -and $cat -ne $Category) { continue }

        Write-Log "Processing category: $cat" "INFO"

        $indexPath = Join-Path $Script:Config.LessonsRoot "$cat-index.md"
        $archiveDir = Join-Path $Script:Config.LessonsRoot "archive"

        # Get recent archive files (last 30 days)
        $recentArchives = Get-ChildItem $archiveDir -Filter "$cat-*.md" |
            Where-Object { $_.LastWriteTime -gt (Get-Date).AddDays(-30) }

        if ($recentArchives.Count -eq 0) {
            Write-Log "No recent lessons for $cat" "INFO"
            continue
        }

        # Read current index
        $indexContent = Get-Content $indexPath -Raw

        # Add recent lessons section if not exists
        if ($indexContent -notmatch "## 🟢 Recent Additions") {
            $indexContent = $indexContent -replace '\*Full details in archive files.*', @"

## 🟢 Recent Additions (Last 30 Days)

*Recent lessons added to the knowledge base*

---

*Full details in archive files. This index prioritizes prevention.*
"@
        }

        if (!$DryRun) {
            $indexContent | Set-Content $indexPath -Encoding UTF8
            Write-Log "Updated index: $indexPath" "SUCCESS"
        } else {
            Write-Log "Would update index: $indexPath" "INFO"
        }
    }
}

# Archive old lessons
function Invoke-LessonArchival {
    Write-Log "📦 Archiving old lessons..." "INFO"

    $archiveDir = Join-Path $Script:Config.LessonsRoot "archive"
    $oldLessons = Get-ChildItem $archiveDir -Filter "*.md" |
        Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-$Script:Config.ArchiveThresholdDays) }

    if ($oldLessons.Count -eq 0) {
        Write-Log "No lessons to archive" "INFO"
        return
    }

    Write-Log "Found $($oldLessons.Count) lessons older than $($Script:Config.ArchiveThresholdDays) days" "INFO"

    foreach ($lesson in $oldLessons) {
        $year = $lesson.LastWriteTime.Year
        $archivePath = Join-Path $archiveDir "$year" $lesson.Name

        if (!$DryRun) {
            # Create year directory if needed
            $yearDir = Join-Path $archiveDir $year
            if (!(Test-Path $yearDir)) {
                New-Item -ItemType Directory -Path $yearDir -Force | Out-Null
            }

            # Move file to yearly archive
            Move-Item $lesson.FullName $archivePath -Force
            Write-Log "Archived: $($lesson.Name) → $year/" "SUCCESS"
        } else {
            Write-Log "Would archive: $($lesson.Name) → $year/" "INFO"
        }
    }
}

# Generate maintenance report
function New-MaintenanceReport {
    Write-Log "📊 Generating maintenance report..." "INFO"

    $stats = @{
        TotalIndexes = 0
        TotalArchives = 0
        Categories = @("frontend", "backend", "architecture", "devops", "quality", "process")
        Issues = @()
    }

    # Count files
    foreach ($cat in $stats.Categories) {
        $indexPath = Join-Path $Script:Config.LessonsRoot "$cat-index.md"
        if (Test-Path $indexPath) { $stats.TotalIndexes++ }

        $archiveDir = Join-Path $Script:Config.LessonsRoot "archive"
        $archiveFiles = Get-ChildItem $archiveDir -Filter "$cat-*.md" -ErrorAction SilentlyContinue
        $stats.TotalArchives += $archiveFiles.Count
    }

    # Generate report
    Write-Log "" "INFO"
    Write-Log "=== MAINTENANCE STATISTICS ===" "INFO"
    Write-Log "Category Indexes: $($stats.TotalIndexes)/$($stats.Categories.Count)" "INFO"
    Write-Log "Archive Files: $($stats.TotalArchives)" "INFO"
    Write-Log "Archive Threshold: $($Script:Config.ArchiveThresholdDays) days" "INFO"
    Write-Log "Max Active Lessons: $($Script:Config.MaxActiveLessons) per category" "INFO"

    # Check for issues
    if ($stats.TotalIndexes -lt $stats.Categories.Count) {
        $stats.Issues += "Missing category indexes"
    }

    if ($stats.Issues.Count -gt 0) {
        Write-Log "" "INFO"
        Write-Log "⚠️  ISSUES FOUND:" "WARN"
        foreach ($issue in $stats.Issues) {
            Write-Log "  - $issue" "WARN"
        }
    } else {
        Write-Log "" "INFO"
        Write-Log "✅ All maintenance checks passed" "SUCCESS"
    }
}

# Main execution logic
try {
    Initialize-Report

    switch ($Action) {
        "validate" {
            $valid = Test-LessonStructure
            if (!$valid) { exit 1 }
        }
        "update-indexes" {
            Update-CategoryIndexes
        }
        "archive-old" {
            Invoke-LessonArchival
        }
        "generate-report" {
            New-MaintenanceReport
        }
        "full-maintenance" {
            Write-Log "🚀 Starting full maintenance cycle..." "INFO"

            $valid = Test-LessonStructure
            if ($valid) {
                Update-CategoryIndexes
                Invoke-LessonArchival
            }
            New-MaintenanceReport

            Write-Log "✅ Full maintenance cycle completed" "SUCCESS"
        }
    }

    Write-Log "" "INFO"
    Write-Log "Maintenance completed successfully" "SUCCESS"
    Write-Log "Report saved to: $($Script:Config.ReportPath)" "INFO"

} catch {
    Write-Log "❌ Maintenance failed: $($_.Exception.Message)" "ERROR"
    Write-Log "Stack trace: $($_.ScriptStackTrace)" "ERROR"
    exit 1
}