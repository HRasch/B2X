# Documentation Registry Audit Script
# This script audits the DOCUMENT_REGISTRY.md for accuracy and completeness

param(
    [string]$RegistryPath = ".\.ai\DOCUMENT_REGISTRY.md",
    [string]$LogDir = ".\.ai\logs\documentation\"
)

$DateStamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$LogFile = Join-Path $LogDir "documentation-audit_$DateStamp.md"

# Ensure log directory exists
if (!(Test-Path $LogDir)) {
    New-Item -ItemType Directory -Path $LogDir -Force
}

# Function to parse markdown table
function Parse-MarkdownTable {
    param([string]$Content, [string]$Section)

    $lines = $Content -split "`n"
    $inTable = $false
    $headers = @()
    $rows = @()

    foreach ($line in $lines) {
        $line = $line.Trim()
        if ($line -eq "## Registry: $Section") {
            $inTable = $true
            continue
        }
        if ($inTable -and $line.StartsWith("|")) {
            $values = $line -split '\|' | Where-Object { $_ -and $_.Trim() -ne '' } | ForEach-Object { $_.Trim() }
            if ($values.Count -ge 3) {
                if ($headers.Count -eq 0) {
                    $headers = $values
                } elseif ($values[0] -notmatch "^-+$") {  # Skip separator lines
                    $row = [PSCustomObject]@{
                        DocID = $values[0].Trim('`')
                        Title = $values[1]
                        FilePath = $values[2].Trim('`')
                        Status = if ($values.Count -gt 3) { $values[3] } else { "" }
                    }
                    $rows += $row
                }
            }
        } elseif ($inTable -and $line -eq "---") {
            # End of table (assuming --- without | is end)
            break
        }
    }
    return $rows
}

# Read registry
$registryContent = Get-Content $RegistryPath -Raw

# Define sections
$sections = @(
    "Architecture Decision Records (ADR-*)",
    "Knowledgebase (KB-*)",
    "Guidelines (GL-*)",
    "Workflows (WF-*)",
    "Compliance (CMP-*)",
    "Requirements (REQ-*)",
    "Feature Handovers (FH-*)",
    "Sprint Documents (SPR-*)",
    "Templates (TPL-*)",
    "Sales Documentation (DOCS-SALES-*)",
    "Logs (LOG-*)",
    "Prompts (PRM-*)",
    "Brainstorm & Strategy (BS-*)",
    "Status & Dashboards (STATUS-*)",
    "Collaboration (COLLAB-*)",
    "Communications (COMM-*)",
    "Reviews (REV-*)",
    "Planning (PLAN-*)",
    "Quick Start (QS-*)",
    "Indexes (INDEX-*)",
    "Agent Definitions (AGT-*)",
    "Instructions (INS-*)"
)

$allEntries = @()
foreach ($section in $sections) {
    $entries = Parse-MarkdownTable -Content $registryContent -Section $section
    $allEntries += $entries
}

# Check file existence
$missingFiles = @()
$orphanedEntries = @()
foreach ($entry in $allEntries) {
    $filePath = $entry.FilePath
    if ($filePath -and !(Test-Path $filePath)) {
        $missingFiles += $entry
    }
}

# Scan directories for documentation files
$docDirs = @(
    "docs",
    ".ai\decisions",
    ".ai\knowledgebase",
    ".ai\guidelines",
    ".ai\workflows",
    ".ai\requirements",
    ".ai\sprint",
    ".ai\templates",
    ".ai\config",
    ".ai\compliance",
    ".github\agents",
    ".github\instructions",
    ".github\prompts",
    ".ai\handovers",
    ".ai\brainstorm"
)

$allDocFiles = @()
foreach ($dir in $docDirs) {
    if (Test-Path $dir) {
        $files = Get-ChildItem -Path $dir -Recurse -File -Include "*.md" | Where-Object { $_.FullName -notmatch "\\\.ai\\logs\\" -and $_.FullName -notmatch "\\\.ai\\temp\\" }
        $allDocFiles += $files
    }
}

# Check for files without registry entries
$registeredPaths = $allEntries | Where-Object { $_.FilePath } | Select-Object -ExpandProperty FilePath
$missingEntries = @()
foreach ($file in $allDocFiles) {
    $relativePath = $file.FullName.Replace((Get-Location).Path + "\", "").Replace("\", "/")
    if ($registeredPaths -notcontains $relativePath) {
        $missingEntries += $relativePath
    }
}

# Generate report
$report = @"
# Documentation Registry Audit Report
**Date:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
**Registry File:** $RegistryPath

## Summary
- **Total Registry Entries:** $($allEntries.Count)
- **Missing Files:** $($missingFiles.Count)
- **Orphaned Entries:** $($orphanedEntries.Count) (Note: Orphaned check not implemented yet)
- **Files Missing Registry Entries:** $($missingEntries.Count)

## Missing Files (DocIDs pointing to non-existent files)
$($missingFiles | ForEach-Object { "- **$($_.DocID)**: $($_.FilePath)" } | Out-String)

## Files Missing Registry Entries
$($missingEntries | ForEach-Object { "- $_" } | Out-String)

## Orphaned DocIDs (Registry entries without files)
(No orphaned entries detected - this check needs enhancement)

## Recommendations
- Fix missing files by updating paths or removing entries
- Add registry entries for missing files
- Run this audit regularly
"@

# Write to log
$report | Out-File -FilePath $LogFile -Encoding UTF8

Write-Host "Audit complete. Report saved to $LogFile"

# Output summary
Write-Host "Summary:"
Write-Host "- Total entries: $($allEntries.Count)"
Write-Host "- Missing files: $($missingFiles.Count)"
Write-Host "- Missing entries: $($missingEntries.Count)"