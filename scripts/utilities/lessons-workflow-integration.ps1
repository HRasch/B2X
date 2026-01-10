#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Lessons Workflow Integration - WF-010 Integration

.DESCRIPTION
    Integrates lessons learned maintenance with WF-010 documentation maintenance workflow.
    This script is called by the documentation maintenance workflow to ensure lessons
    are properly maintained as part of the overall documentation quality process.

.PARAMETER Trigger
    The workflow trigger that initiated this integration

.PARAMETER Frequency
    How often this integration runs (daily, weekly, monthly)

.EXAMPLE
    .\lessons-workflow-integration.ps1 -Trigger "weekly-maintenance" -Frequency weekly
#>

param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("daily-maintenance", "weekly-maintenance", "monthly-maintenance", "manual-trigger")]
    [string]$Trigger,

    [Parameter(Mandatory = $true)]
    [ValidateSet("daily", "weekly", "monthly")]
    [string]$Frequency
)

# Configuration
$Script:Config = @{
    LessonsRoot = Join-Path $PSScriptRoot ".." ".ai" "knowledgebase" "lessons"
    WorkflowLog = Join-Path $PSScriptRoot ".." ".ai" "logs" "wf-010-lessons-integration-$(Get-Date -Format 'yyyyMMdd').log"
    MaintenanceScript = Join-Path $PSScriptRoot "lessons-maintenance.ps1"
}

function Write-WorkflowLog {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] [WF-010-LESSONS] $Message"
    Write-Host $logMessage
    Add-Content -Path $Script:Config.WorkflowLog -Value $logMessage
}

function Initialize-WorkflowIntegration {
    Write-WorkflowLog "=== WF-010 Lessons Integration Started ===" "INFO"
    Write-WorkflowLog "Trigger: $Trigger" "INFO"
    Write-WorkflowLog "Frequency: $Frequency" "INFO"
    Write-WorkflowLog "Timestamp: $(Get-Date)" "INFO"
    Write-WorkflowLog "" "INFO"
}

function Invoke-LessonsMaintenanceWorkflow {
    Write-WorkflowLog "🔄 Executing lessons maintenance workflow..." "INFO"

    # Determine maintenance actions based on frequency
    $actions = switch ($Frequency) {
        "daily" { @("validate") }
        "weekly" { @("validate", "update-indexes", "generate-report") }
        "monthly" { @("full-maintenance") }
    }

    foreach ($action in $actions) {
        Write-WorkflowLog "Running maintenance action: $action" "INFO"

        try {
            $process = Start-Process -FilePath "pwsh" -ArgumentList @(
                "-File", $Script:Config.MaintenanceScript,
                "-Action", $action
            ) -Wait -PassThru -NoNewWindow

            if ($process.ExitCode -eq 0) {
                Write-WorkflowLog "✅ Maintenance action '$action' completed successfully" "SUCCESS"
            } else {
                Write-WorkflowLog "❌ Maintenance action '$action' failed with exit code $($process.ExitCode)" "ERROR"
                return $false
            }
        } catch {
            Write-WorkflowLog "❌ Failed to execute maintenance action '$action': $($_.Exception.Message)" "ERROR"
            return $false
        }
    }

    return $true
}

function Test-LessonsHealthCheck {
    Write-WorkflowLog "🏥 Performing lessons health check..." "INFO"

    $healthIssues = @()
    $categories = @("frontend", "backend", "architecture", "devops", "quality", "process")

    # Check category indexes exist
    foreach ($cat in $categories) {
        $indexPath = Join-Path $Script:Config.LessonsRoot "$cat-index.md"
        if (!(Test-Path $indexPath)) {
            $healthIssues += "Missing category index: $cat"
        }
    }

    # Check archive directory exists
    $archiveDir = Join-Path $Script:Config.LessonsRoot "archive"
    if (!(Test-Path $archiveDir)) {
        $healthIssues += "Missing archive directory"
    }

    # Check maintenance script exists
    if (!(Test-Path $Script:Config.MaintenanceScript)) {
        $healthIssues += "Missing maintenance script"
    }

    if ($healthIssues.Count -eq 0) {
        Write-WorkflowLog "✅ Lessons health check passed" "SUCCESS"
        return $true
    } else {
        Write-WorkflowLog "❌ Lessons health check failed:" "ERROR"
        foreach ($issue in $healthIssues) {
            Write-WorkflowLog "  - $issue" "ERROR"
        }
        return $false
    }
}

function Update-LessonsMetrics {
    Write-WorkflowLog "📊 Updating lessons metrics..." "INFO"

    $metrics = @{
        TotalCategories = 6
        ExistingIndexes = 0
        TotalArchives = 0
        LastMaintenance = Get-Date
        HealthStatus = "Unknown"
    }

    # Count existing indexes
    $categories = @("frontend", "backend", "architecture", "devops", "quality", "process")
    foreach ($cat in $categories) {
        $indexPath = Join-Path $Script:Config.LessonsRoot "$cat-index.md"
        if (Test-Path $indexPath) { $metrics.ExistingIndexes++ }
    }

    # Count archives
    $archiveDir = Join-Path $Script:Config.LessonsRoot "archive"
    if (Test-Path $archiveDir) {
        $metrics.TotalArchives = (Get-ChildItem $archiveDir -Filter "*.md" -Recurse).Count
    }

    # Determine health status
    $healthCheck = Test-LessonsHealthCheck
    $metrics.HealthStatus = if ($healthCheck) { "Healthy" } else { "Issues Found" }

    # Save metrics
    $metricsPath = Join-Path $Script:Config.LessonsRoot "metrics.json"
    $metrics | ConvertTo-Json | Set-Content $metricsPath -Encoding UTF8

    Write-WorkflowLog "Metrics updated: $($metrics.ExistingIndexes)/$($metrics.TotalCategories) indexes, $($metrics.TotalArchives) archives" "INFO"
    Write-WorkflowLog "Health Status: $($metrics.HealthStatus)" "INFO"
}

function Send-WorkflowNotification {
    param([bool]$Success, [string]$Details = "")

    $status = if ($Success) { "SUCCESS" } else { "FAILED" }
    $message = "WF-010 Lessons Integration $status"

    if ($Details) {
        $message += ": $Details"
    }

    Write-WorkflowLog $message (if ($Success) { "SUCCESS" } else { "ERROR" })

    # In a real implementation, this would send notifications to team channels
    # For now, just log the notification intent
    Write-WorkflowLog "Notification would be sent to: @DocMaintainer, @SARAH" "INFO"
}

# Main execution
try {
    Initialize-WorkflowIntegration

    # Perform health check first
    $healthOk = Test-LessonsHealthCheck
    if (!$healthOk) {
        Send-WorkflowNotification -Success $false -Details "Health check failed"
        exit 1
    }

    # Execute maintenance workflow
    $maintenanceSuccess = Invoke-LessonsMaintenanceWorkflow

    # Update metrics
    Update-LessonsMetrics

    # Send final notification
    $overallSuccess = $healthOk -and $maintenanceSuccess
    Send-WorkflowNotification -Success $overallSuccess

    Write-WorkflowLog "" "INFO"
    Write-WorkflowLog "=== WF-010 Lessons Integration Completed ===" "INFO"
    Write-WorkflowLog "Overall Status: $(if ($overallSuccess) { "SUCCESS" } else { "FAILED" })" (if ($overallSuccess) { "SUCCESS" } else { "ERROR" })

    if (!$overallSuccess) {
        exit 1
    }

} catch {
    Write-WorkflowLog "❌ WF-010 Lessons Integration failed: $($_.Exception.Message)" "ERROR"
    Send-WorkflowNotification -Success $false -Details $_.Exception.Message
    exit 1
}