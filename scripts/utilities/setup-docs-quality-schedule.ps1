# Setup weekly documentation quality monitoring
# Run this script to create a Windows scheduled task for weekly quality reports

$taskName = "B2Connect-Docs-Quality-Monitor"
$scriptPath = "$PSScriptRoot\..\scripts\docs-quality-monitor.sh"
$repoPath = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

# Check if the script exists
if (!(Test-Path $scriptPath)) {
    Write-Host "❌ Quality monitoring script not found at: $scriptPath"
    exit 1
}

# Create the scheduled task
$action = New-ScheduledTaskAction -Execute "bash.exe" -Argument "`"$scriptPath`"" -WorkingDirectory $repoPath
$trigger = New-ScheduledTaskTrigger -Weekly -DaysOfWeek Monday -At 9am
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -StartWhenAvailable
$principal = New-ScheduledTaskPrincipal -UserId $env:USERNAME -LogonType InteractiveToken

# Remove existing task if it exists
Unregister-ScheduledTask -TaskName $taskName -Confirm:$false -ErrorAction SilentlyContinue

# Register the new task
Register-ScheduledTask -TaskName $taskName -Action $action -Trigger $trigger -Settings $settings -Principal $principal -Description "Weekly documentation quality monitoring for B2Connect"

Write-Host "✅ Weekly documentation quality monitoring task created:"
Write-Host "   Task Name: $taskName"
Write-Host "   Schedule: Every Monday at 9:00 AM"
Write-Host "   Script: $scriptPath"
Write-Host ""
Write-Host "To modify the schedule, use Task Scheduler or run this script again."