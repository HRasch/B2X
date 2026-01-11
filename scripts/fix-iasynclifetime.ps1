<#
.SYNOPSIS
    Fixes IAsyncLifetime implementations for xUnit v3 migration.
    
.DESCRIPTION
    xUnit v3 changed the IAsyncLifetime interface:
    - InitializeAsync() now returns ValueTask instead of Task
    - DisposeAsync() now returns ValueTask instead of Task
    
    This script finds all test files implementing IAsyncLifetime and updates
    the return types from Task to ValueTask.

.EXAMPLE
    .\fix-iasynclifetime.ps1
#>

param(
    [string]$RootPath = (Get-Location).Path,
    [switch]$WhatIf
)

$ErrorActionPreference = "Stop"

Write-Host "🔧 xUnit v3 IAsyncLifetime Migration Script" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Find all C# files that contain IAsyncLifetime
$testFiles = Get-ChildItem -Path $RootPath -Recurse -Include "*.cs" | 
    Where-Object { 
        $content = Get-Content $_.FullName -Raw
        $content -match "IAsyncLifetime" -and 
        ($content -match "public\s+async\s+Task\s+InitializeAsync" -or 
         $content -match "public\s+async\s+Task\s+DisposeAsync" -or
         $content -match "public\s+Task\s+InitializeAsync" -or
         $content -match "public\s+Task\s+DisposeAsync")
    }

if ($testFiles.Count -eq 0) {
    Write-Host "✅ No files need IAsyncLifetime migration!" -ForegroundColor Green
    exit 0
}

Write-Host "📁 Found $($testFiles.Count) files to update:" -ForegroundColor Yellow
$testFiles | ForEach-Object { Write-Host "   - $($_.FullName)" -ForegroundColor Gray }
Write-Host ""

$updatedCount = 0
$errorCount = 0

foreach ($file in $testFiles) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor White
    
    try {
        $content = Get-Content $file.FullName -Raw
        $originalContent = $content
        
        # Pattern 1: public async Task InitializeAsync() -> public async ValueTask InitializeAsync()
        $content = $content -replace 'public\s+async\s+Task\s+InitializeAsync\s*\(\s*\)', 'public async ValueTask InitializeAsync()'
        
        # Pattern 2: public Task InitializeAsync() -> public ValueTask InitializeAsync()
        $content = $content -replace 'public\s+Task\s+InitializeAsync\s*\(\s*\)', 'public ValueTask InitializeAsync()'
        
        # Pattern 3: public async Task DisposeAsync() -> public async ValueTask DisposeAsync()
        $content = $content -replace 'public\s+async\s+Task\s+DisposeAsync\s*\(\s*\)', 'public async ValueTask DisposeAsync()'
        
        # Pattern 4: public Task DisposeAsync() -> public ValueTask DisposeAsync()
        $content = $content -replace 'public\s+Task\s+DisposeAsync\s*\(\s*\)', 'public ValueTask DisposeAsync()'
        
        # Pattern 5: Handle lambda expressions like: public async Task InitializeAsync() => await ...
        $content = $content -replace 'public\s+async\s+Task\s+InitializeAsync\s*\(\s*\)\s*=>', 'public async ValueTask InitializeAsync() =>'
        $content = $content -replace 'public\s+async\s+Task\s+DisposeAsync\s*\(\s*\)\s*=>', 'public async ValueTask DisposeAsync() =>'
        
        # Pattern 6: Task.CompletedTask -> ValueTask.CompletedTask in return statements for these methods
        # This is tricky - we need context-aware replacement
        # For now, we'll handle the simple cases and warn about others
        
        if ($content -ne $originalContent) {
            if ($WhatIf) {
                Write-Host "   Would update: $($file.FullName)" -ForegroundColor Yellow
            } else {
                Set-Content -Path $file.FullName -Value $content -NoNewline
                Write-Host "   ✅ Updated" -ForegroundColor Green
            }
            $updatedCount++
        } else {
            Write-Host "   ⏭️  No changes needed (might already be migrated or has different pattern)" -ForegroundColor Gray
        }
    }
    catch {
        Write-Host "   ❌ Error: $_" -ForegroundColor Red
        $errorCount++
    }
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "  Total files processed: $($testFiles.Count)" -ForegroundColor White
Write-Host "  Files updated: $updatedCount" -ForegroundColor Green
Write-Host "  Errors: $errorCount" -ForegroundColor $(if ($errorCount -gt 0) { "Red" } else { "Green" })
Write-Host ""

if ($updatedCount -gt 0 -and -not $WhatIf) {
    Write-Host "⚠️  Note: Some files may also need:" -ForegroundColor Yellow
    Write-Host "   - 'return Task.CompletedTask' changed to 'return ValueTask.CompletedTask'" -ForegroundColor Yellow
    Write-Host "   - 'return new ValueTask(someTask)' for wrapping Task returns" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "💡 Run 'dotnet build' to check for remaining issues." -ForegroundColor Cyan
}
