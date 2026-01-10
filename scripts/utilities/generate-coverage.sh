#!/bin/bash

# B2X Code Coverage Script
# Generates coverage reports for local development

set -e

echo "🧪 Running tests with coverage..."

# Run tests with coverage
dotnet test B2X.slnx \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage \
  --settings coverlet.runsettings \
  -v minimal

echo "📊 Generating coverage reports..."

# Generate HTML and other reports
dotnet tool run reportgenerator \
  -reports:"coverage/**/coverage.opencover.xml" \
  -targetdir:"test-results/coverage-report" \
  -reporttypes:"Html;Cobertura;MarkdownSummary;TextSummary"

echo "✅ Coverage reports generated!"
echo "📁 Reports available in: test-results/coverage-report/"
echo "🌐 Open test-results/coverage-report/index.html in your browser"

# Display summary
if [ -f "test-results/coverage-report/Summary.txt" ]; then
  echo ""
  echo "📈 Coverage Summary:"
  cat test-results/coverage-report/Summary.txt
fi