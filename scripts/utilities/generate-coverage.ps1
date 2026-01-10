# B2X Code Coverage Script
# Generates coverage reports for local development

Write-Host "🧪 Running tests with coverage..." -ForegroundColor Green

# Run tests with coverage
dotnet test B2X.slnx `
  --collect:"XPlat Code Coverage" `
  --results-directory ./coverage `
  --settings coverlet.runsettings `
  -v minimal

Write-Host "📊 Generating coverage reports..." -ForegroundColor Green

# Generate HTML and other reports
dotnet tool run reportgenerator `
  -reports:"coverage/**/coverage.opencover.xml" `
  -targetdir:"test-results/coverage-report" `
  -reporttypes:"Html;Cobertura;MarkdownSummary;TextSummary"

Write-Host "✅ Coverage reports generated!" -ForegroundColor Green
Write-Host "📁 Reports available in: test-results/coverage-report/" -ForegroundColor Cyan
Write-Host "🌐 Open test-results/coverage-report/index.html in your browser" -ForegroundColor Cyan

# Display summary
$summaryPath = "test-results/coverage-report/Summary.txt"
if (Test-Path $summaryPath) {
  Write-Host ""
  Write-Host "📈 Coverage Summary:" -ForegroundColor Yellow
  Get-Content $summaryPath
}