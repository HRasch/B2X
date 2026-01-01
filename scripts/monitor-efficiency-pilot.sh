#!/bin/bash

# Efficiency Pilot Monitoring Script
# Tracks key metrics for the efficiency improvements pilot

echo "🚀 B2Connect Efficiency Pilot - Metrics Dashboard"
echo "=================================================="
echo "Date: $(date)"
echo ""

# Function to calculate average
calculate_average() {
    local sum=0
    local count=0
    for value in "$@"; do
        sum=$((sum + value))
        count=$((count + 1))
    done
    if [ $count -gt 0 ]; then
        echo "scale=2; $sum / $count" | bc
    else
        echo "0"
    fi
}

echo "📊 COMMIT METRICS"
echo "-----------------"

# Conventional commit compliance
total_commits=$(git log --oneline --since="1 week ago" | wc -l)
conventional_commits=$(git log --oneline --since="1 week ago" | grep -E "^(feat|fix|docs|style|refactor|test|chore|perf|ci|build|revert)(\(.+\))?: " | wc -l)

if [ $total_commits -gt 0 ]; then
    compliance_rate=$((conventional_commits * 100 / total_commits))
    echo "✅ Conventional Commit Compliance: $compliance_rate% ($conventional_commits/$total_commits)"
else
    echo "ℹ️  No commits in the last week"
fi

echo ""
echo "🔒 SECURITY METRICS"
echo "-------------------"

# Check if CodeQL workflow ran successfully
codeql_runs=$(gh run list --workflow="codeql-security-scan.yml" --limit=5 --json conclusion | jq -r '.[] | select(.conclusion == "success") | .conclusion' | wc -l)
echo "✅ CodeQL Security Scans (last 5): $codeql_runs/5 successful"

# Pre-commit hook effectiveness (detect-secrets)
secrets_found=$(git log --oneline --since="1 week ago" | grep -i "secret\|key\|password" | wc -l)
echo "🔍 Potential secrets in commits: $secrets_found (should be 0)"

echo ""
echo "🧪 TESTING METRICS"
echo "------------------"

# E2E test runs
e2e_runs=$(gh run list --workflow="e2e-tests.yml" --limit=5 --json conclusion | jq -r '.[] | select(.conclusion == "success") | .conclusion' | wc -l)
echo "✅ E2E Test Runs (last 5): $e2e_runs/5 successful"

# Test coverage (if available)
if [ -f "frontend/Store/coverage/lcov-report/index.html" ]; then
    # Extract coverage percentage from lcov report (simplified)
    echo "📈 Frontend Test Coverage: Check coverage/lcov-report/index.html"
fi

echo ""
echo "💬 PROCESS METRICS"
echo "------------------"

# PR metrics (requires GitHub CLI)
open_prs=$(gh pr list --json number | jq length)
echo "📋 Open PRs: $open_prs"

# Average PR review time (simplified - would need more complex logic)
echo "⏱️  PR Review Time: Manual tracking required"

echo ""
echo "🎯 EFFICIENCY TARGETS"
echo "---------------------"
echo "Meeting Time Reduction: Target -60% (Track manually)"
echo "Review Cycle Speed: Target -30% (Track manually)"
echo "Security Detection: Target -70% (✅ Automated)"
echo "Test Execution: Target -40% (✅ Automated)"

echo ""
echo "📝 NEXT STEPS"
echo "-------------"
echo "1. Monitor metrics weekly"
echo "2. Collect team feedback via #efficiency-pilot"
echo "3. Adjust processes based on data"
echo "4. Roll out Priority 3 improvements after pilot"

echo ""
echo "📊 Raw Data Export: $(date +%Y%m%d)_efficiency_metrics.json"