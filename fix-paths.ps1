Set-Location "c:\Users\Holge\repos\B2Connect"
Get-ChildItem -Path "." -Recurse -Filter "*.csproj" | ForEach-Object {
    $file = $_.FullName
    $content = Get-Content $file -Raw
    $newContent = $content -replace '../../../../backend/', '../../../../src/backend/'
    if ($content -ne $newContent) {
        Set-Content $file $newContent
        Write-Host "Updated $file"
    }
}