# Roslyn Batch Analysis Script for .NET Backend Domains
# Implements Phase 2 automated batch analysis using Roslyn analyzers
# References [GL-006] Token Optimization, [GL-043] Smart Attachments

param(
    [string]$Domain = "all",
    [switch]$Parallel,
    [switch]$Fix,
    [switch]$SLA
)

$ErrorActionPreference = "Stop"

# Configuration
$backendDomains = @("Catalog", "CMS", "Identity", "Localization", "Search")
$rootDir = Split-Path -Parent $PSScriptRoot
$backendDir = Join-Path $rootDir "backend"

function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] [$Level] $Message"
}

function Get-RoslynErrors {
    param([string]$ProjectPath)
    
    Write-Log "Analyzing $ProjectPath with Roslyn..."
    
    try {
        $output = & dotnet build $ProjectPath --verbosity minimal 2>&1
        $errors = $output | Where-Object { $_ -match "error" -or $_ -match "warning" }
        return $errors
    }
    catch {
        Write-Log "Failed to analyze ${ProjectPath}: $_" "ERROR"
        return @()
    }
}

function Analyze-Domain {
    param([string]$domainName)
    
    $domainPath = Join-Path $backendDir "Domain\$domainName"
    if (!(Test-Path $domainPath)) {
        Write-Log "Domain path not found: $domainPath" "WARN"
        return @()
    }
    
    $testPath = Join-Path $domainPath "tests"
    $apiPath = Join-Path $domainPath "API"
    
    $projects = @()
    if (Test-Path (Join-Path $domainPath "*.csproj")) {
        $projects += Get-ChildItem $domainPath -Filter "*.csproj" | Select-Object -ExpandProperty FullName
    }
    if (Test-Path $testPath) {
        $projects += Get-ChildItem $testPath -Filter "*.csproj" -Recurse | Select-Object -ExpandProperty FullName
    }
    if (Test-Path $apiPath) {
        $projects += Get-ChildItem $apiPath -Filter "*.csproj" -Recurse | Select-Object -ExpandProperty FullName
    }
    
    $allErrors = @()
    foreach ($project in $projects) {
        $errors = Get-RoslynErrors $project
        $allErrors += $errors | ForEach-Object { 
            [PSCustomObject]@{
                Project = $project
                Domain = $domainName
                Error = $_
                Timestamp = Get-Date
            }
        }
    }
    
    return $allErrors
}

function Run-ParallelAnalysis {
    param([string[]]$domains)
    
    Write-Log "Running parallel analysis for domains: $($domains -join ', ')"
    
    $jobs = @()
    foreach ($domain in $domains) {
        $job = Start-Job -ScriptBlock {
            param($domainName, $backendDir, $rootDir)
            
            function Write-Log {
                param([string]$Message, [string]$Level = "INFO")
                $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
                Write-Host "[$timestamp] [$Level] $Message"
            }
            
            function Get-RoslynErrors {
                param([string]$ProjectPath)
                
                Write-Log "Analyzing $ProjectPath with Roslyn..."
                
                try {
                    $output = & dotnet build $ProjectPath --verbosity minimal 2>&1
                    $errors = $output | Where-Object { $_ -match "error" -or $_ -match "warning" }
                    return $errors
                }
                catch {
                    Write-Log "Failed to analyze ${ProjectPath}: $_" "ERROR"
                    return @()
                }
            }
            
            $domainPath = Join-Path $backendDir "Domain\$domainName"
            if (!(Test-Path $domainPath)) {
                Write-Log "Domain path not found: $domainPath" "WARN"
                return @()
            }
            
            $testPath = Join-Path $domainPath "tests"
            $apiPath = Join-Path $domainPath "API"
            
            $projects = @()
            if (Test-Path (Join-Path $domainPath "*.csproj")) {
                $projects += Get-ChildItem $domainPath -Filter "*.csproj" | Select-Object -ExpandProperty FullName
            }
            if (Test-Path $testPath) {
                $projects += Get-ChildItem $testPath -Filter "*.csproj" -Recurse | Select-Object -ExpandProperty FullName
            }
            if (Test-Path $apiPath) {
                $projects += Get-ChildItem $apiPath -Filter "*.csproj" -Recurse | Select-Object -ExpandProperty FullName
            }
            
            $allErrors = @()
            foreach ($project in $projects) {
                $errors = Get-RoslynErrors $project
                $allErrors += $errors | ForEach-Object { 
                    [PSCustomObject]@{
                        Project = $project
                        Domain = $domainName
                        Error = $_
                        Timestamp = Get-Date
                    }
                }
            }
            
            return $allErrors
        } -ArgumentList $domain, $backendDir, $rootDir
        $jobs += $job
    }
    
    $results = @()
    foreach ($job in $jobs) {
        $result = Receive-Job -Job $job -Wait
        $results += $result
        Remove-Job -Job $job
    }
    
    return $results
}

function Apply-Fixes {
    param([array]$errors)
    
    Write-Log "Applying automated fixes..."
    
    # Group errors by project
    $errorsByProject = $errors | Group-Object -Property Project
    
    foreach ($group in $errorsByProject) {
        $project = $group.Name
        $projectErrors = $group.Group
        
        Write-Log "Fixing $project..."
        
        # Run dotnet format
        & dotnet format $project
        
        # For StyleCop warnings, attempt auto-fix
        $styleCopErrors = $projectErrors | Where-Object { $_.Error -match "SA\d+" }
        if ($styleCopErrors) {
            Write-Log "Found $($styleCopErrors.Count) StyleCop issues in $project"
            # Note: Auto-fixing StyleCop requires specific tools, placeholder for now
        }
    }
}

function Check-SLA {
    param([array]$errors)
    
    Write-Log "Checking SLA compliance..."
    
    $criticalErrors = $errors | Where-Object { $_.Error -match "error" }
    $highErrors = $errors | Where-Object { $_.Error -match "warning" -and $_.Error -match "CS\d+" }
    
    $now = Get-Date
    
    foreach ($error in $criticalErrors) {
        $age = $now - $error.Timestamp
        if ($age.TotalHours -gt 4) {
            Write-Log "SLA VIOLATION: Critical error in $($error.Project) older than 4 hours" "ERROR"
        }
    }
    
    foreach ($error in $highErrors) {
        $age = $now - $error.Timestamp
        if ($age.TotalHours -gt 24) {
            Write-Log "SLA VIOLATION: High priority error in $($error.Project) older than 24 hours" "WARN"
        }
    }
}

# Main execution
Write-Log "Starting Roslyn Batch Analysis - Phase 2"

$targetDomains = if ($Domain -eq "all") { $backendDomains } else { @($Domain) }

if ($Parallel) {
    $allErrors = Run-ParallelAnalysis $targetDomains
} else {
    $allErrors = @()
    foreach ($domain in $targetDomains) {
        $errors = Analyze-Domain $domain
        $allErrors += $errors
    }
}

Write-Log "Analysis complete. Found $($allErrors.Count) issues."

if ($Fix) {
    Apply-Fixes $allErrors
}

if ($SLA) {
    Check-SLA $allErrors
}

# Export results
$reportPath = Join-Path $rootDir ".ai\status\roslyn-batch-report-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
$allErrors | ConvertTo-Json | Out-File $reportPath
Write-Log "Report saved to $reportPath"

Write-Log "Roslyn Batch Analysis completed."