# Pre-Build Lock Check Script
# Purpose: Detect and clean up file locks before building to prevent CS2012 errors
# Usage: Run before `dotnet build` to avoid file lock conflicts
#
# This addresses known MSBuild issue: https://github.com/dotnet/sdk/issues/9585

param(
    [switch]$AutoClean,
    [switch]$Verbose
)

$ErrorActionPreference = "Continue"
$RepoRoot = Split-Path -Parent $PSScriptRoot

Write-Host "[CHECK] Pre-Build Lock Check" -ForegroundColor Cyan
Write-Host "Checking for locked files in obj folders..." -ForegroundColor Gray

# Find all obj folders
$objFolders = Get-ChildItem -Path $RepoRoot -Recurse -Directory -Filter "obj" -ErrorAction SilentlyContinue | 
    Where-Object { $_.FullName -notlike "*node_modules*" }

$lockedFiles = @()
$checkedCount = 0

foreach ($folder in $objFolders) {
    $files = Get-ChildItem $folder.FullName -Recurse -File -Include "*.dll", "*.exe", "*.pdb" -ErrorAction SilentlyContinue
    foreach ($file in $files) {
        $checkedCount++
        try {
            $stream = [System.IO.File]::Open($file.FullName, 'Open', 'ReadWrite', 'None')
            $stream.Close()
        } catch {
            $lockedFiles += $file.FullName
            if ($Verbose) {
                Write-Host "  [WARN] Locked: $($file.FullName)" -ForegroundColor Yellow
            }
        }
    }
}

Write-Host "Checked $checkedCount files in $($objFolders.Count) obj folders" -ForegroundColor Gray

if ($lockedFiles.Count -eq 0) {
    Write-Host "[OK] No locked files detected - safe to build" -ForegroundColor Green
    exit 0
}

Write-Host ""
Write-Host "[WARN] Found $($lockedFiles.Count) locked file(s):" -ForegroundColor Yellow
$lockedFiles | ForEach-Object { Write-Host "   $_" -ForegroundColor DarkYellow }

# Check for running dotnet processes
$dotnetProcs = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
if ($dotnetProcs) {
    Write-Host ""
    Write-Host "[INFO] Running dotnet processes:" -ForegroundColor Cyan
    $dotnetProcs | ForEach-Object { 
        Write-Host "   PID: $($_.Id) | Started: $($_.StartTime) | Memory: $([math]::Round($_.WorkingSet64/1MB, 1)) MB" -ForegroundColor Gray
    }
}

if ($AutoClean -or $dotnetProcs) {
    Write-Host ""
    if ($AutoClean) {
        Write-Host "[CLEAN] Auto-clean enabled - stopping dotnet processes..." -ForegroundColor Cyan
    } else {
        $response = Read-Host "Stop all dotnet processes and clean obj folders? (Y/n)"
        if ($response -ne "" -and $response -notmatch "^[Yy]") {
            Write-Host "Skipping cleanup. Build may fail with CS2012 errors." -ForegroundColor Yellow
            exit 1
        }
    }
    
    # Stop dotnet processes
    Write-Host "Stopping dotnet processes..." -ForegroundColor Gray
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    
    # Verify processes stopped
    $remainingProcs = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
    if ($remainingProcs) {
        Write-Host "[WARN] Some dotnet processes still running - trying forceful termination..." -ForegroundColor Yellow
        $remainingProcs | Stop-Process -Force -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 2
    }
    
    # Clean obj folders with locked files
    Write-Host "Cleaning affected obj folders..." -ForegroundColor Gray
    $cleanedFolders = @()
    foreach ($lockedFile in $lockedFiles) {
        $objFolder = $lockedFile
        while ($objFolder -and (Split-Path -Leaf $objFolder) -ne "obj") {
            $objFolder = Split-Path -Parent $objFolder
        }
        if ($objFolder -and $objFolder -notin $cleanedFolders) {
            $cleanedFolders += $objFolder
            Remove-Item $objFolder -Recurse -Force -ErrorAction SilentlyContinue
            if ($Verbose) {
                Write-Host "   Cleaned: $objFolder" -ForegroundColor Gray
            }
        }
    }
    
    Write-Host "[OK] Cleanup complete - cleaned $($cleanedFolders.Count) obj folder(s)" -ForegroundColor Green
    Write-Host "   Safe to build now" -ForegroundColor Gray
    exit 0
}

Write-Host ""
Write-Host "[TIP] Run with -AutoClean to automatically fix:" -ForegroundColor Cyan
Write-Host "   .\scripts\pre-build-check.ps1 -AutoClean" -ForegroundColor Gray
exit 1
