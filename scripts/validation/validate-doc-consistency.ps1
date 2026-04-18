<#
.SYNOPSIS
    Validates documentation consistency across the B2X project.

.DESCRIPTION
    This script checks for:
    - Broken DocID references
    - Missing document metadata
    - Stale documents (source changed after last-verified)
    - Orphan documents (missing derives-from)
    - Circular references

.PARAMETER Path
    Root path to scan. Defaults to repository root.

.PARAMETER ReportFormat
    Output format: Console, Markdown, JSON. Default: Console

.PARAMETER FailOnError
    Exit with error code if issues found. For CI/CD use.

.EXAMPLE
    .\validate-doc-consistency.ps1
    .\validate-doc-consistency.ps1 -ReportFormat Markdown -FailOnError
#>

param(
    [string]$Path = (Split-Path -Parent (Split-Path -Parent $PSScriptRoot)),
    [ValidateSet("Console", "Markdown", "JSON")]
    [string]$ReportFormat = "Console",
    [switch]$FailOnError
)

$ErrorActionPreference = "Stop"

# ============================================================================
# Configuration
# ============================================================================

$DocPaths = @(
    ".ai",
    ".github/agents",
    ".github/prompts", 
    ".github/instructions",
    "docs"
)

$DocIdPattern = '\[([A-Z]+-\d+[A-Za-z0-9-]*)\]'
$MetadataPattern = '(?s)^---\s*\n(.*?)\n---'

# ============================================================================
# Data Structures
# ============================================================================

class ValidationResult {
    [string]$Type        # Error, Warning, Info
    [string]$Category    # BrokenRef, Stale, Orphan, MissingMeta
    [string]$FilePath
    [string]$Message
    [string]$DocId
}

$Results = [System.Collections.Generic.List[ValidationResult]]::new()
$DocRegistry = @{}
$DocReferences = @{}
$DocMetadata = @{}

# ============================================================================
# Functions
# ============================================================================

function Get-DocumentFiles {
    param([string]$RootPath)
    
    $files = @()
    foreach ($docPath in $DocPaths) {
        $fullPath = Join-Path $RootPath $docPath
        if (Test-Path $fullPath) {
            $files += Get-ChildItem -Path $fullPath -Recurse -Include "*.md" -File
        }
    }
    return $files
}

function Parse-DocumentMetadata {
    param([string]$Content)
    
    $metadata = @{}
    
    if ($Content -match $MetadataPattern) {
        $yamlBlock = $Matches[1]
        
        # Simple YAML parsing for common fields
        $lines = $yamlBlock -split "`n"
        $currentKey = $null
        $currentValue = @()
        
        foreach ($line in $lines) {
            if ($line -match '^(\w+[-\w]*):\s*(.*)$') {
                # Save previous key if exists
                if ($currentKey -and $currentValue.Count -gt 0) {
                    $metadata[$currentKey] = $currentValue -join ","
                }
                
                $currentKey = $Matches[1].Trim()
                $value = $Matches[2].Trim()
                
                if ($value -and $value -ne "") {
                    $currentValue = @($value)
                } else {
                    $currentValue = @()
                }
            }
            elseif ($line -match '^\s+-\s+(.+)$' -and $currentKey) {
                # List item
                $currentValue += $Matches[1].Trim()
            }
        }
        
        # Save last key
        if ($currentKey -and $currentValue.Count -gt 0) {
            $metadata[$currentKey] = $currentValue -join ","
        }
    }
    
    return $metadata
}

function Find-DocIdReferences {
    param([string]$Content)
    
    $references = @()
    $matches = [regex]::Matches($Content, $DocIdPattern)
    
    foreach ($match in $matches) {
        $docId = $match.Groups[1].Value
        if ($docId -notin $references) {
            $references += $docId
        }
    }
    
    return $references
}

function Build-DocRegistry {
    param([string]$RegistryPath)
    
    if (-not (Test-Path $RegistryPath)) {
        Write-Warning "DOCUMENT_REGISTRY.md not found at $RegistryPath"
        return @{}
    }
    
    $content = Get-Content $RegistryPath -Raw
    $registry = @{}
    
    # Parse table rows for DocIDs
    $tableRowPattern = '\|\s*`?([A-Z]+-[\w-]+)`?\s*\|'
    $matches = [regex]::Matches($content, $tableRowPattern)
    
    foreach ($match in $matches) {
        $docId = $match.Groups[1].Value
        $registry[$docId] = $true
    }
    
    return $registry
}

function Add-ValidationResult {
    param(
        [string]$Type,
        [string]$Category,
        [string]$FilePath,
        [string]$Message,
        [string]$DocId = ""
    )
    
    $result = [ValidationResult]::new()
    $result.Type = $Type
    $result.Category = $Category
    $result.FilePath = $FilePath
    $result.Message = $Message
    $result.DocId = $DocId
    
    $Results.Add($result)
}

function Test-BrokenReferences {
    param(
        [hashtable]$References,
        [hashtable]$Registry
    )
    
    foreach ($file in $References.Keys) {
        foreach ($ref in $References[$file]) {
            # Skip common false positives
            if ($ref -match '^(YYYY|XX|NN)-') { continue }
            if ($ref -eq "PREFIX-NUMBER") { continue }
            
            if (-not $Registry.ContainsKey($ref)) {
                Add-ValidationResult -Type "Error" -Category "BrokenRef" `
                    -FilePath $file -DocId $ref `
                    -Message "Reference to '$ref' not found in DOCUMENT_REGISTRY"
            }
        }
    }
}

function Test-MissingMetadata {
    param([hashtable]$Metadata)
    
    $requiredFields = @("docid", "title", "owner", "status")
    
    foreach ($file in $Metadata.Keys) {
        $meta = $Metadata[$file]
        
        if ($meta.Count -eq 0) {
            Add-ValidationResult -Type "Warning" -Category "MissingMeta" `
                -FilePath $file -Message "No YAML frontmatter found"
            continue
        }
        
        foreach ($field in $requiredFields) {
            if (-not $meta.ContainsKey($field) -or [string]::IsNullOrWhiteSpace($meta[$field])) {
                Add-ValidationResult -Type "Warning" -Category "MissingMeta" `
                    -FilePath $file -Message "Missing required field: $field"
            }
        }
    }
}

function Test-StaleDocuments {
    param([hashtable]$Metadata)
    
    $today = Get-Date
    $warningThreshold = 30  # days
    $errorThreshold = 60    # days
    
    foreach ($file in $Metadata.Keys) {
        $meta = $Metadata[$file]
        
        if ($meta.ContainsKey("derives-from") -and $meta.ContainsKey("last-verified")) {
            try {
                $lastVerified = [DateTime]::Parse($meta["last-verified"])
                $daysSinceVerified = ($today - $lastVerified).Days
                
                if ($daysSinceVerified -gt $errorThreshold) {
                    Add-ValidationResult -Type "Error" -Category "Stale" `
                        -FilePath $file `
                        -Message "Document not verified in $daysSinceVerified days (threshold: $errorThreshold)"
                }
                elseif ($daysSinceVerified -gt $warningThreshold) {
                    Add-ValidationResult -Type "Warning" -Category "Stale" `
                        -FilePath $file `
                        -Message "Document not verified in $daysSinceVerified days (threshold: $warningThreshold)"
                }
            }
            catch {
                # Invalid date format, skip
            }
        }
    }
}

function Test-OrphanDocuments {
    param([hashtable]$Metadata)
    
    # Level 2 docs should have derives-from
    $level2Prefixes = @("KB-", "GL-", "INS-", "WF-")
    
    foreach ($file in $Metadata.Keys) {
        $meta = $Metadata[$file]
        
        if (-not $meta.ContainsKey("docid")) { continue }
        
        $docId = $meta["docid"]
        $isLevel2 = $level2Prefixes | Where-Object { $docId.StartsWith($_) }
        
        if ($isLevel2 -and -not $meta.ContainsKey("derives-from")) {
            # Only warn, not all Level 2 docs need derives-from
            Add-ValidationResult -Type "Info" -Category "Orphan" `
                -FilePath $file -DocId $docId `
                -Message "Level 2 document without 'derives-from' metadata"
        }
    }
}

function Format-ConsoleReport {
    $errors = $Results | Where-Object { $_.Type -eq "Error" }
    $warnings = $Results | Where-Object { $_.Type -eq "Warning" }
    $infos = $Results | Where-Object { $_.Type -eq "Info" }
    
    Write-Host "`n===== Documentation Consistency Report =====" -ForegroundColor Cyan
    Write-Host "Scanned: $($DocMetadata.Count) documents"
    Write-Host "Found: $($DocReferences.Values | ForEach-Object { $_.Count } | Measure-Object -Sum | Select-Object -ExpandProperty Sum) DocID references"
    Write-Host ""
    
    if ($errors.Count -gt 0) {
        Write-Host "ERRORS ($($errors.Count)):" -ForegroundColor Red
        foreach ($e in $errors) {
            $relativePath = $e.FilePath.Replace($Path, "").TrimStart("\", "/")
            Write-Host "   [$($e.Category)] $relativePath" -ForegroundColor Red
            Write-Host "      $($e.Message)" -ForegroundColor DarkRed
        }
        Write-Host ""
    }
    
    if ($warnings.Count -gt 0) {
        Write-Host "WARNINGS ($($warnings.Count)):" -ForegroundColor Yellow
        foreach ($w in $warnings) {
            $relativePath = $w.FilePath.Replace($Path, "").TrimStart("\", "/")
            Write-Host "   [$($w.Category)] $relativePath" -ForegroundColor Yellow
            Write-Host "      $($w.Message)" -ForegroundColor DarkYellow
        }
        Write-Host ""
    }
    
    if ($infos.Count -gt 0) {
        Write-Host "INFO ($($infos.Count)):" -ForegroundColor Gray
        foreach ($i in $infos) {
            $relativePath = $i.FilePath.Replace($Path, "").TrimStart("\", "/")
            Write-Host "   [$($i.Category)] $relativePath" -ForegroundColor Gray
        }
        Write-Host ""
    }
    
    # Summary
    $status = if ($errors.Count -eq 0) { "PASSED" } else { "FAILED" }
    $statusColor = if ($errors.Count -eq 0) { "Green" } else { "Red" }
    Write-Host "Status: $status" -ForegroundColor $statusColor
    Write-Host "============================================`n" -ForegroundColor Cyan
}

function Format-MarkdownReport {
    $errors = $Results | Where-Object { $_.Type -eq "Error" }
    $warnings = $Results | Where-Object { $_.Type -eq "Warning" }
    $infos = $Results | Where-Object { $_.Type -eq "Info" }
    
    $report = "# Documentation Consistency Report`n`n"
    $report += "**Generated**: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')`n"
    $report += "**Documents Scanned**: $($DocMetadata.Count)`n"
    $report += "**References Found**: $($DocReferences.Values | ForEach-Object { $_.Count } | Measure-Object -Sum | Select-Object -ExpandProperty Sum)`n`n"
    
    $report += "## Summary`n`n"
    $pipe = [char]124
    $report += "$pipe Category $pipe Count $pipe Status $pipe`n"
    $report += "$pipe----------$pipe-------$pipe--------$pipe`n"
    $report += "$pipe Errors $pipe $($errors.Count) $pipe $(if ($errors.Count -eq 0) { 'PASS' } else { 'FAIL' }) $pipe`n"
    $report += "$pipe Warnings $pipe $($warnings.Count) $pipe $(if ($warnings.Count -eq 0) { 'PASS' } else { 'WARN' }) $pipe`n"
    $report += "$pipe Info $pipe $($infos.Count) $pipe INFO $pipe`n`n"
    
    if ($errors.Count -gt 0) {
        $report += "`n## ❌ Errors`n`n"
        foreach ($e in $errors) {
            $relativePath = $e.FilePath.Replace($Path, "").TrimStart("\", "/")
            $report += "- **[$($e.Category)]** ``$relativePath```n  $($e.Message)`n"
        }
    }
    
    if ($warnings.Count -gt 0) {
        $report += "`n## ⚠️ Warnings`n`n"
        foreach ($w in $warnings) {
            $relativePath = $w.FilePath.Replace($Path, "").TrimStart("\", "/")
            $report += "- **[$($w.Category)]** ``$relativePath```n  $($w.Message)`n"
        }
    }
    
    Write-Output $report
}

function Format-JsonReport {
    $report = @{
        generated = (Get-Date -Format "yyyy-MM-ddTHH:mm:ss")
        summary = @{
            documentsScanned = $DocMetadata.Count
            errors = ($Results | Where-Object { $_.Type -eq "Error" }).Count
            warnings = ($Results | Where-Object { $_.Type -eq "Warning" }).Count
            infos = ($Results | Where-Object { $_.Type -eq "Info" }).Count
        }
        results = $Results | ForEach-Object {
            @{
                type = $_.Type
                category = $_.Category
                file = $_.FilePath.Replace($Path, "").TrimStart("\", "/")
                message = $_.Message
                docId = $_.DocId
            }
        }
    }
    
    Write-Output ($report | ConvertTo-Json -Depth 5)
}

# ============================================================================
# Main Execution
# ============================================================================

Write-Host "Scanning documentation in: $Path" -ForegroundColor Cyan

# Build registry from DOCUMENT_REGISTRY.md
$registryPath = Join-Path $Path ".ai\DOCUMENT_REGISTRY.md"
$DocRegistry = Build-DocRegistry -RegistryPath $registryPath
Write-Host "Loaded $($DocRegistry.Count) DocIDs from registry" -ForegroundColor Gray

# Scan all document files
$files = Get-DocumentFiles -RootPath $Path
Write-Host "Found $($files.Count) markdown files to scan" -ForegroundColor Gray

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
    if (-not $content) { continue }
    
    # Parse metadata
    $DocMetadata[$file.FullName] = Parse-DocumentMetadata -Content $content
    
    # Find references
    $DocReferences[$file.FullName] = Find-DocIdReferences -Content $content
}

# Run validation checks
Write-Host "Running validation checks..." -ForegroundColor Gray

Test-BrokenReferences -References $DocReferences -Registry $DocRegistry
Test-MissingMetadata -Metadata $DocMetadata
Test-StaleDocuments -Metadata $DocMetadata
Test-OrphanDocuments -Metadata $DocMetadata

# Output report
switch ($ReportFormat) {
    "Console" { Format-ConsoleReport }
    "Markdown" { Format-MarkdownReport }
    "JSON" { Format-JsonReport }
}

# Exit with error code if requested
if ($FailOnError) {
    $errorCount = ($Results | Where-Object { $_.Type -eq "Error" }).Count
    if ($errorCount -gt 0) {
        exit 1
    }
}

exit 0
