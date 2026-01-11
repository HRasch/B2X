<#
.SYNOPSIS
    Returns optimal CPU count for parallel builds based on system resources.
    
.DESCRIPTION
    Detects available CPU cores and returns an optimal count for parallel operations.
    Reserves 2 cores for system responsiveness on systems with 8+ cores.
    
.PARAMETER Reserve
    Number of cores to reserve for system (default: 2 for 8+ cores, 0 otherwise)
    
.EXAMPLE
    $cores = & scripts/get-cpu-count.ps1
    dotnet build -m:$cores
#>
param(
    [int]$Reserve = -1  # -1 means auto-detect
)

$totalCores = [Environment]::ProcessorCount

# Auto-detect reserve: 2 cores for systems with 8+, otherwise 0
if ($Reserve -eq -1) {
    $Reserve = if ($totalCores -ge 8) { 2 } else { 0 }
}

$optimalCores = [Math]::Max(1, $totalCores - $Reserve)

# Output just the number for easy consumption
Write-Output $optimalCores
