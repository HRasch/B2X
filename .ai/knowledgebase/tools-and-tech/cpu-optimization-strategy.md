---
docid: KB-069
title: CPU Optimization Strategy
owner: @DevOps
status: Active
created: 2026-01-10
---

# CPU Optimization Strategy for Development Performance

**DocID**: `KB-069`

## Overview

This strategy enables optimal CPU utilization across different development machines by auto-detecting available cores rather than hardcoding values.

## Configuration

### Automatic Mode (Default)

All configurations use `0` or `-m` (without a number) to auto-detect available CPU cores:

| Component | Setting | Effect |
|-----------|---------|--------|
| MSBuild | `-m` | Uses all available processors |
| xUnit | `maxParallelThreads: 0` | Uses `Environment.ProcessorCount` |
| Directory.Build.props | `MaxCpuCount=0` | Auto-detect at build time |

### Manual Override

For machines where you want to reserve cores:

```powershell
# Option 1: Environment variable (persistent)
$env:B2X_MAX_CPU_COUNT = "12"

# Option 2: Per-command override
dotnet build -m:12 B2X.slnx

# Option 3: Local config file
# Copy .vscode/b2x-dev.env to .vscode/b2x-dev.local.env
# Edit B2X_MAX_CPU_COUNT value (gitignored)
```

### Helper Script

```powershell
# Get optimal CPU count (reserves 2 cores on 8+ core systems)
$cores = & scripts/get-cpu-count.ps1
dotnet build -m:$cores
```

## Files Modified

| File | Change |
|------|--------|
| `src/Directory.Build.props` | Dynamic `MaxCpuCount` with env override |
| `.vscode/tasks.json` | Tasks use `-m` (auto-detect) |
| `xunit.runner.json` | `maxParallelThreads: 0` (auto) |
| `.vscode/b2x-dev.env` | Shared dev environment template |
| `scripts/get-cpu-count.ps1` | Helper for manual control |

## VS Code Tasks

| Task | Description |
|------|-------------|
| `build-backend` | Parallel solution build |
| `build-apphost` | Parallel AppHost build |
| `test-backend` | Parallel test execution |
| `test-all-parallel` | Maximum parallel testing |
| `full-parallel-build` | Backend + Frontend parallel |

## Expected Performance Gains

| Cores | Build Improvement | Test Improvement |
|-------|-------------------|------------------|
| 4 | ~2x faster | ~2-3x faster |
| 8 | ~3-4x faster | ~4-5x faster |
| 16 | ~4-6x faster | ~6-8x faster |
| 24+ | ~5-8x faster | ~8-10x faster |

*Actual gains depend on project structure and disk I/O*

## Troubleshooting

### High Memory Usage
Reduce parallel count:
```powershell
$env:B2X_MAX_CPU_COUNT = "8"
```

### DLL Locking Issues
`UseSharedCompilation=false` is already set in `Directory.Build.props`.

### Slow on Laptop
When on battery, Windows may throttle. Check power plan or reduce:
```powershell
dotnet build -m:4 B2X.slnx
```

## Cross-Platform Notes

| OS | Auto-Detection |
|----|----------------|
| Windows | ✅ Full support |
| macOS | ✅ Full support |
| Linux | ✅ Full support |
| WSL | ⚠️ May see host cores; limit if needed |

## See Also

- [GL-006] Token Optimization Strategy
- [ADR-003] Aspire Orchestration
