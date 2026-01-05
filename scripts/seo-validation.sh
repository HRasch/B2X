#!/bin/bash

# SEO Validation Script for B2Connect Store Frontend
# Runs Lighthouse audits on the store frontend to validate SEO improvements

set -e

echo "🚀 Starting SEO Validation for B2Connect Store Frontend"
echo "=================================================="

# Configuration
STORE_URL="http://localhost:5173"
OUTPUT_DIR="./seo-reports"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
REPORT_FILE="${OUTPUT_DIR}/lighthouse_report_${TIMESTAMP}.json"
HTML_REPORT="${OUTPUT_DIR}/lighthouse_report_${TIMESTAMP}.html"

# Create output directory
mkdir -p "$OUTPUT_DIR"

echo "📊 Running Lighthouse audit on $STORE_URL"

# Run Lighthouse with SEO-focused configuration
npx lighthouse "$STORE_URL" \
  --output json \
  --output html \
  --output-path "$REPORT_FILE" \
  --output-path "$HTML_REPORT" \
  --only-categories performance,seo,accessibility,best-practices \
  --chrome-flags="--headless --disable-gpu --no-sandbox --disable-dev-shm-usage" \
  --form-factor desktop \
  --screenEmulation.disabled \
  --throttling.cpuSlowdownMultiplier=1 \
  --throttling.requestLatencyMs=0 \
  --throttling.downloadThroughputKbps=0 \
  --throttling.uploadThroughputKbps=0

echo "✅ Lighthouse audit completed"
echo "📄 JSON Report: $REPORT_FILE"
echo "🌐 HTML Report: $HTML_REPORT"

# Extract key metrics
echo ""
echo "📈 Key SEO Metrics:"
echo "=================="

# Use jq to parse JSON if available
if command -v jq &> /dev/null; then
  echo "Performance Score: $(jq -r '.categories.performance.score * 100' "$REPORT_FILE")%"
  echo "SEO Score: $(jq -r '.categories.seo.score * 100' "$REPORT_FILE")%"
  echo "Accessibility Score: $(jq -r '.categories.accessibility.score * 100' "$REPORT_FILE")%"
  echo "Best Practices Score: $(jq -r '.categories."best-practices".score * 100' "$REPORT_FILE")%"

  echo ""
  echo "🎯 Core Web Vitals:"
  echo "=================="
  echo "LCP (Largest Contentful Paint): $(jq -r '.audits["largest-contentful-paint"].displayValue' "$REPORT_FILE")"
  echo "CLS (Cumulative Layout Shift): $(jq -r '.audits["cumulative-layout-shift"].displayValue' "$REPORT_FILE")"
  echo "FID (First Input Delay): $(jq -r '.audits["max-potential-fid"].displayValue' "$REPORT_FILE")"

  echo ""
  echo "🔍 SEO Audits:"
  echo "=============="
  echo "Document has a valid hreflang: $(jq -r '.audits["hreflang"].score' "$REPORT_FILE")"
  echo "Document has a valid rel=canonical: $(jq -r '.audits["canonical"].score' "$REPORT_FILE")"
  echo "Page has successful HTTP status code: $(jq -r '.audits["http-status-code"].score' "$REPORT_FILE")"
  echo "Document uses legible font sizes: $(jq -r '.audits["font-size"].score' "$REPORT_FILE")"
  echo "Tap targets are sized appropriately: $(jq -r '.audits["tap-targets"].score' "$REPORT_FILE")"

else
  echo "⚠️  jq not found. Install jq to see detailed metrics."
  echo "Manual review required for $HTML_REPORT"
fi

echo ""
echo "🎉 SEO Validation Complete!"
echo "=========================="
echo "Reports saved to: $OUTPUT_DIR"
echo ""
echo "Next Steps:"
echo "- Review HTML report for detailed recommendations"
echo "- Compare scores with baseline metrics"
echo "- Address any failing audits"
echo "- Monitor Core Web Vitals improvements"