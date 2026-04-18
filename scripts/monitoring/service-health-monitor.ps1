#!/usr/bin/env pwsh
<#
.SYNOPSIS
    B2X Service Health Monitor - Realtime debugging companion
    
.DESCRIPTION
    Comprehensive health monitoring for Aspire-managed services with:
    - Container health checks
    - HTTP endpoint validation
    - Log streaming capabilities
    - Startup failure detection
    
.PARAMETER Mode
    Monitor mode: 'status', 'watch', 'logs', 'diagnose'
    
.PARAMETER Service
    Specific service to monitor (optional)
    
.PARAMETER Interval
    Watch interval in seconds (default: 5)
    
.EXAMPLE
    .\service-health-monitor.ps1 -Mode status
    .\service-health-monitor.ps1 -Mode watch -Interval 3
    .\service-health-monitor.ps1 -Mode diagnose
#>

param(
    [Parameter(Position = 0)]
    [ValidateSet('status', 'watch', 'logs', 'diagnose')]
    [string]$Mode = 'status',
    
    [Parameter()]
    [string]$Service,
    
    [Parameter()]
    [int]$Interval = 5
)

# Configuration
$script:AspirePort = 15500
$script:Services = @{
    'aspire'           = @{ Port = 15500; Type = 'dashboard'; Description = 'Aspire Dashboard' }
    'elasticsearch'    = @{ Port = 9200; Type = 'http'; HealthPath = '/_cluster/health'; Description = 'Elasticsearch' }
    'rabbitmq'         = @{ Port = 15672; Type = 'http'; HealthPath = '/api/healthchecks/node'; Description = 'RabbitMQ Management' }
    'rabbitmq-amqp'    = @{ Port = 5672; Type = 'tcp'; Description = 'RabbitMQ AMQP' }
    'store-gateway'    = @{ Port = 8000; Type = 'http'; HealthPath = '/health'; Description = 'Store Gateway' }
    'admin-gateway'    = @{ Port = 8080; Type = 'http'; HealthPath = '/health'; Description = 'Admin Gateway' }
    'auth-service'     = @{ Port = 0; Type = 'aspire'; Description = 'Identity/Auth Service' }
    'tenant-service'   = @{ Port = 0; Type = 'aspire'; Description = 'Tenant Service' }
    'catalog-service'  = @{ Port = 0; Type = 'aspire'; Description = 'Catalog Service' }
}

# Aspire-managed container mappings (dynamic ports)
$script:AspireContainers = @{
    'elasticsearch' = 'elasticsearch-*'
    'rabbitmq'      = 'rabbitmq-*'
}

function Write-Status {
    param([string]$Status, [string]$Message, [string]$Details = '')
    
    $icon = switch ($Status) {
        'ok'      { '✅' }
        'warn'    { '⚠️' }
        'error'   { '❌' }
        'info'    { 'ℹ️' }
        'wait'    { '⏳' }
        default   { '•' }
    }
    
    $color = switch ($Status) {
        'ok'      { 'Green' }
        'warn'    { 'Yellow' }
        'error'   { 'Red' }
        'info'    { 'Cyan' }
        'wait'    { 'Gray' }
        default   { 'White' }
    }
    
    Write-Host "$icon " -NoNewline
    Write-Host $Message -ForegroundColor $color -NoNewline
    if ($Details) {
        Write-Host " - $Details" -ForegroundColor DarkGray
    } else {
        Write-Host
    }
}

function Get-DockerContainers {
    try {
        $containers = docker ps -a --format '{{json .}}' 2>$null | ForEach-Object {
            $_ | ConvertFrom-Json
        }
        return $containers
    }
    catch {
        return @()
    }
}

function Get-AspireManagedContainers {
    $containers = Get-DockerContainers
    $aspireContainers = $containers | Where-Object { 
        $_.Names -match 'elasticsearch-[a-z]+$|rabbitmq-[a-z]+$' -or
        $_.Labels -like '*aspire*' -or
        $_.Networks -like '*aspire*'
    }
    return $aspireContainers
}

function Test-TcpPort {
    param([string]$TargetHost = 'localhost', [int]$Port, [int]$Timeout = 2)
    
    try {
        $tcpClient = New-Object System.Net.Sockets.TcpClient
        $result = $tcpClient.BeginConnect($TargetHost, $Port, $null, $null)
        $success = $result.AsyncWaitHandle.WaitOne($Timeout * 1000)
        
        if ($success) {
            $tcpClient.EndConnect($result)
            $tcpClient.Close()
            return $true
        }
        $tcpClient.Close()
        return $false
    }
    catch {
        return $false
    }
}

function Test-HttpEndpoint {
    param([string]$Url, [int]$Timeout = 5)
    
    try {
        $response = Invoke-WebRequest -Uri $Url -TimeoutSec $Timeout -UseBasicParsing -ErrorAction Stop
        return @{
            Success = $true
            StatusCode = $response.StatusCode
            Content = $response.Content
        }
    }
    catch {
        return @{
            Success = $false
            StatusCode = 0
            Error = $_.Exception.Message
        }
    }
}

function Get-ContainerHealth {
    param([string]$ContainerNamePattern)
    
    $containers = Get-DockerContainers
    $containerMatches = $containers | Where-Object { $_.Names -like $ContainerNamePattern }
    
    foreach ($container in $containerMatches) {
        $state = $container.State
        $status = $container.Status
        $ports = $container.Ports
        
        $health = @{
            Name = $container.Names
            Image = $container.Image
            State = $state
            Status = $status
            Ports = $ports
            Running = ($state -eq 'running')
            Healthy = ($status -match 'healthy' -or $state -eq 'running')
        }
        
        # Extract port mappings
        if ($ports -match '(\d+)->(\d+)/tcp') {
            $health.HostPort = [int]$Matches[1]
            $health.ContainerPort = [int]$Matches[2]
        }
        
        return $health
    }
    
    return $null
}

function Get-ContainerLogs {
    param(
        [string]$ContainerName,
        [int]$Lines = 50,
        [switch]$Follow
    )
    
    $logArgs = @('logs')
    if ($Follow) {
        $logArgs += '--follow'
    }
    $logArgs += '--tail', $Lines, $ContainerName
    
    try {
        if ($Follow) {
            & docker @logArgs
        } else {
            $logs = & docker @logArgs 2>&1
            return $logs
        }
    }
    catch {
        Write-Status 'error' "Failed to get logs for $ContainerName" $_.Exception.Message
        return $null
    }
}

function Show-ServiceStatus {
    Write-Host "`n📊 B2X Service Health Status" -ForegroundColor Cyan
    Write-Host "═" * 50 -ForegroundColor DarkGray
    Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor DarkGray
    Write-Host
    
    # Check Aspire Dashboard
    Write-Host "🎛️  Aspire Orchestration" -ForegroundColor White
    Write-Host "─" * 40 -ForegroundColor DarkGray
    
    if (Test-TcpPort -Port $script:AspirePort) {
        Write-Status 'ok' 'Aspire Dashboard' "http://localhost:$($script:AspirePort)"
    } else {
        Write-Status 'error' 'Aspire Dashboard' 'Not running'
    }
    
    # Check Infrastructure Containers
    Write-Host "`n🐳 Infrastructure Containers" -ForegroundColor White
    Write-Host "─" * 40 -ForegroundColor DarkGray
    
    $containers = Get-DockerContainers
    $aspireContainers = $containers | Where-Object { 
        $_.Names -match 'elasticsearch-[a-z]+$|rabbitmq-[a-z]+$'
    }
    
    foreach ($container in $aspireContainers) {
        $name = $container.Names
        $state = $container.State
        $status = $container.Status
        
        if ($state -eq 'running') {
            $statusText = if ($status -match 'healthy') { 'healthy' } else { $status }
            Write-Status 'ok' $name $statusText
            
            # Show ports
            if ($container.Ports) {
                Write-Host "     └─ Ports: $($container.Ports)" -ForegroundColor DarkGray
            }
        } else {
            Write-Status 'error' $name $status
        }
    }
    
    # Check docker-compose services (if needed)
    $composeServices = $containers | Where-Object { 
        $_.Names -like 'B2X-*' -and $_.State -eq 'exited'
    }
    
    if ($composeServices.Count -gt 0) {
        Write-Host "`n⚠️  Stopped Docker-Compose Services" -ForegroundColor Yellow
        Write-Host "─" * 40 -ForegroundColor DarkGray
        
        foreach ($svc in $composeServices) {
            $exitInfo = if ($svc.Status -match 'Exited \((\d+)\)') { "Exit code: $($Matches[1])" } else { $svc.Status }
            Write-Status 'warn' $svc.Names $exitInfo
        }
    }
    
    # Health Endpoint Checks
    Write-Host "`n🌐 HTTP Health Endpoints" -ForegroundColor White
    Write-Host "─" * 40 -ForegroundColor DarkGray
    
    # Elasticsearch health
    $esContainer = $aspireContainers | Where-Object { $_.Names -match 'elasticsearch' } | Select-Object -First 1
    if ($esContainer -and $esContainer.Ports -match '(\d+)->9200/tcp') {
        $esPort = [int]$Matches[1]
        $esHealth = Test-HttpEndpoint -Url "http://localhost:$esPort/_cluster/health" -Timeout 3
        
        if ($esHealth.Success) {
            $healthData = $esHealth.Content | ConvertFrom-Json
            $clusterStatus = $healthData.status
            $statusIcon = switch ($clusterStatus) {
                'green'  { 'ok' }
                'yellow' { 'warn' }
                default  { 'error' }
            }
            Write-Status $statusIcon "Elasticsearch Cluster" "Status: $clusterStatus, Nodes: $($healthData.number_of_nodes)"
        } else {
            Write-Status 'error' 'Elasticsearch Cluster' $esHealth.Error
        }
    }
    
    # RabbitMQ health
    $rmqContainer = $aspireContainers | Where-Object { $_.Names -match 'rabbitmq' } | Select-Object -First 1
    if ($rmqContainer -and $rmqContainer.Ports -match '(\d+)->15672/tcp') {
        $rmqPort = [int]$Matches[1]
        $rmqHealth = Test-HttpEndpoint -Url "http://localhost:$rmqPort/api/health/checks/alarms" -Timeout 3
        
        if ($rmqHealth.Success) {
            Write-Status 'ok' 'RabbitMQ Management' "Port: $rmqPort"
        } else {
            # Try basic connection
            if (Test-TcpPort -Port $rmqPort) {
                Write-Status 'warn' 'RabbitMQ Management' 'Port open but health check failed'
            } else {
                Write-Status 'error' 'RabbitMQ Management' 'Not accessible'
            }
        }
    }
    
    Write-Host "`n" -NoNewline
}

function Show-Diagnose {
    Write-Host "`n🔍 B2X Service Diagnostics" -ForegroundColor Cyan
    Write-Host "═" * 50 -ForegroundColor DarkGray
    Write-Host
    
    # 1. Check Docker
    Write-Host "1. Docker Status" -ForegroundColor White
    Write-Host "─" * 40 -ForegroundColor DarkGray
    
    try {
        $dockerInfo = docker info --format '{{.ServerVersion}}' 2>$null
        Write-Status 'ok' 'Docker Engine' "Version: $dockerInfo"
    }
    catch {
        Write-Status 'error' 'Docker Engine' 'Not running or not installed'
        return
    }
    
    # 2. Container Summary
    Write-Host "`n2. Container Summary" -ForegroundColor White
    Write-Host "─" * 40 -ForegroundColor DarkGray
    
    $containers = Get-DockerContainers
    $running = ($containers | Where-Object { $_.State -eq 'running' }).Count
    $exited = ($containers | Where-Object { $_.State -eq 'exited' }).Count
    $total = $containers.Count
    
    Write-Host "   Total: $total | Running: $running | Exited: $exited" -ForegroundColor Gray
    
    # 3. Failed Containers Analysis
    $failedContainers = $containers | Where-Object { 
        $_.State -eq 'exited' -and $_.Status -match 'Exited \(([1-9]\d*)\)'
    }
    
    if ($failedContainers.Count -gt 0) {
        Write-Host "`n3. Failed Containers (Non-Zero Exit)" -ForegroundColor Yellow
        Write-Host "─" * 40 -ForegroundColor DarkGray
        
        foreach ($container in $failedContainers) {
            $exitCode = if ($container.Status -match 'Exited \((\d+)\)') { $Matches[1] } else { 'Unknown' }
            Write-Status 'error' $container.Names "Exit code: $exitCode"
            
            # Get last few log lines
            Write-Host "   Last log lines:" -ForegroundColor DarkGray
            $logs = Get-ContainerLogs -ContainerName $container.Names -Lines 5
            if ($logs) {
                $logs | ForEach-Object { Write-Host "   │ $_" -ForegroundColor DarkGray }
            }
            Write-Host
        }
    }
    
    # 4. Port Conflicts
    Write-Host "`n4. Port Analysis" -ForegroundColor White
    Write-Host "─" * 40 -ForegroundColor DarkGray
    
    $commonPorts = @(5432, 6379, 9200, 5672, 15672, 8000, 8080, 15500)
    foreach ($port in $commonPorts) {
        $portName = switch ($port) {
            5432  { 'PostgreSQL' }
            6379  { 'Redis' }
            9200  { 'Elasticsearch' }
            5672  { 'RabbitMQ AMQP' }
            15672 { 'RabbitMQ Management' }
            8000  { 'Store Gateway' }
            8080  { 'Admin Gateway' }
            15500 { 'Aspire Dashboard' }
            default { "Port $port" }
        }
        
        if (Test-TcpPort -Port $port -Timeout 1) {
            Write-Status 'info' $portName "Port $port in use"
        }
    }
    
    # 5. Resource Usage
    Write-Host "`n5. Running Container Resources" -ForegroundColor White
    Write-Host "─" * 40 -ForegroundColor DarkGray
    
    $runningContainers = $containers | Where-Object { $_.State -eq 'running' }
    if ($runningContainers.Count -gt 0) {
        try {
            $stats = docker stats --no-stream --format 'table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}' 2>$null
            $stats | ForEach-Object { Write-Host "   $_" -ForegroundColor Gray }
        }
        catch {
            Write-Host "   Unable to retrieve stats" -ForegroundColor DarkGray
        }
    } else {
        Write-Host "   No running containers" -ForegroundColor DarkGray
    }
    
    Write-Host
}

function Watch-Services {
    param([int]$IntervalSec = 5)
    
    Write-Host "🔄 Watching services (Ctrl+C to stop)..." -ForegroundColor Cyan
    Write-Host
    
    while ($true) {
        Clear-Host
        Show-ServiceStatus
        
        Write-Host "─" * 50 -ForegroundColor DarkGray
        Write-Host "Next refresh in $IntervalSec seconds... (Ctrl+C to stop)" -ForegroundColor DarkGray
        
        Start-Sleep -Seconds $IntervalSec
    }
}

function Show-Logs {
    param([string]$ServiceName)
    
    $containers = Get-DockerContainers
    
    if ($ServiceName) {
        $target = $containers | Where-Object { $_.Names -like "*$ServiceName*" } | Select-Object -First 1
        if ($target) {
            Write-Host "📜 Logs for: $($target.Names)" -ForegroundColor Cyan
            Write-Host "─" * 50 -ForegroundColor DarkGray
            Get-ContainerLogs -ContainerName $target.Names -Lines 100
        } else {
            Write-Status 'error' 'Service not found' $ServiceName
        }
    } else {
        Write-Host "Available containers:" -ForegroundColor Cyan
        $containers | ForEach-Object {
            $stateColor = if ($_.State -eq 'running') { 'Green' } else { 'Yellow' }
            Write-Host "  • $($_.Names)" -ForegroundColor $stateColor -NoNewline
            Write-Host " ($($_.State))" -ForegroundColor DarkGray
        }
        Write-Host "`nUsage: .\service-health-monitor.ps1 -Mode logs -Service <name>" -ForegroundColor Gray
    }
}

# Main execution
switch ($Mode) {
    'status' {
        Show-ServiceStatus
    }
    'watch' {
        Watch-Services -IntervalSec $Interval
    }
    'diagnose' {
        Show-Diagnose
    }
    'logs' {
        Show-Logs -ServiceName $Service
    }
}
