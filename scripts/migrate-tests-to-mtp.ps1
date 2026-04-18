# MTP v2 Migration Script for .NET 10 Test Projects
# Migrates from xunit v2 to xunit.v3.mtp-v2

$testProjects = Get-ChildItem -Path "c:\Users\Holge\repos\B2Connect\src\tests" -Filter "*.Tests.csproj" -Recurse | 
    Where-Object { $_.FullName -notlike "*AppHost.Tests*" }

Write-Host "Found $($testProjects.Count) test projects to migrate" -ForegroundColor Cyan

foreach ($project in $testProjects) {
    Write-Host "`nMigrating: $($project.Name)" -ForegroundColor Cyan
    $content = Get-Content $project.FullName -Raw
    
    # Skip if already migrated
    if ($content -match "xunit\.v3\.mtp-v2") {
        Write-Host "  Already migrated, skipping" -ForegroundColor Yellow
        continue
    }
    
    # Add MTP properties after IsTestProject
    $content = $content -replace '(<IsTestProject>true</IsTestProject>)', "`$1`r`n    <OutputType>Exe</OutputType>`r`n    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>`r`n    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>"

    # Remove Microsoft.NET.Test.Sdk
    $content = $content -replace '\s*<PackageReference Include="Microsoft\.NET\.Test\.Sdk"\s*/>', ''
    
    # Remove xunit.runner.visualstudio (with sub-elements)
    $content = $content -replace '(?s)\s*<PackageReference Include="xunit\.runner\.visualstudio"[^>]*>\s*<PrivateAssets>[^<]*</PrivateAssets>\s*<IncludeAssets>[^<]*</IncludeAssets>\s*</PackageReference>', ''
    
    # Remove xunit.runner.visualstudio (simple)
    $content = $content -replace '\s*<PackageReference Include="xunit\.runner\.visualstudio"\s*/>', ''
    
    # Remove coverlet.collector
    $content = $content -replace '\s*<PackageReference Include="coverlet\.collector"\s*/>', ''
    
    # Replace xunit with xunit.v3.mtp-v2
    $content = $content -replace '<PackageReference Include="xunit"\s*/>', '<PackageReference Include="xunit.v3.mtp-v2" />'
    
    # Clean up multiple blank lines
    $content = $content -replace "(\r?\n){3,}", "`r`n`r`n"
    
    # Save
    Set-Content $project.FullName $content -NoNewline
    Write-Host "  Done" -ForegroundColor Green
}

Write-Host "`n`nMigration complete!" -ForegroundColor Green
