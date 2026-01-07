#!/bin/bash

# Token Audit Script for B2Connect
# Run monthly to identify token waste and optimization opportunities
# Usage: ./audit-tokens.sh

echo "🔍 B2Connect Token Audit Report"
echo "================================"
echo "Date: $(date)"
echo ""

# 1. Instruction File Size Analysis
echo "📄 INSTRUCTION FILE ANALYSIS"
echo "────────────────────────────────"

total_size=0
for file in .github/instructions/*.instructions.md; do
    if [ -f "$file" ]; then
        size=$(wc -c < "$file")
        size_kb=$((size / 1024))
        lines=$(wc -l < "$file")
        
        # Estimate tokens (roughly 1 token per 3-4 chars)
        tokens=$((size / 4))
        
        filename=$(basename "$file")
        echo "$filename:"
        echo "  Size: $size_kb KB | Lines: $lines | Est. Tokens: $tokens"
        
        total_size=$((total_size + size))
    fi
done

total_kb=$((total_size / 1024))
total_tokens=$((total_size / 4))
echo ""
echo "TOTAL Instruction Files: $total_kb KB (~$total_tokens tokens per load)"
echo ""

# 2. Large File Detection
echo "📊 LARGE FILE DETECTION"
echo "────────────────────────────────"
echo "Files that would benefit from fragment-based access (GL-044):"
echo ""

find . -type f \( -name "*.cs" -o -name "*.ts" -o -name "*.vue" \) ! -path "*/node_modules/*" ! -path "*/.git/*" ! -path "*/bin/*" ! -path "*/obj/*" | while read file; do
    lines=$(wc -l < "$file")
    if [ "$lines" -gt 150 ]; then
        size=$(wc -c < "$file")
        size_kb=$((size / 1024))
        tokens=$((size / 4))
        echo "  $file: $lines lines | $size_kb KB | ~$tokens tokens"
    fi
done | head -20

echo ""

# 3. KB Article References Check
echo "📚 KB ARTICLE SIZE REFERENCE"
echo "────────────────────────────────"
echo "Common KB articles (if pre-loaded as attachments):"
echo ""

# Assuming KB articles are in .ai/knowledgebase
if [ -d ".ai/knowledgebase" ]; then
    find .ai/knowledgebase -name "*.md" -type f | head -10 | while read file; do
        size=$(wc -c < "$file")
        size_kb=$((size / 1024))
        tokens=$((size / 4))
        filename=$(basename "$file")
        echo "  $filename: $size_kb KB (~$tokens tokens)"
    done
else
    echo "  .ai/knowledgebase not found"
fi

echo ""

# 4. Token Savings Calculation
echo "💰 TOKEN SAVINGS ESTIMATE (with GL-043/044/045)"
echo "────────────────────────────────"
echo ""
echo "Assuming typical backend feature work:"
echo ""
echo "BEFORE optimization:"
echo "  - Instruction files:     6,000 tokens (18 KB)"
echo "  - KB articles pre-load:  5,000 tokens (15 KB)"
echo "  - File reads (avg):      4,000 tokens"
echo "  ──────────────────────────────────────"
echo "  Overhead per task:      15,000 tokens ❌"
echo ""
echo "AFTER GL-043/044/045:"
echo "  - Instruction files:     2,000 tokens (3 KB, path-specific)"
echo "  - KB queries (on-demand): 1,500 tokens"
echo "  - Fragment-based reads:  2,000 tokens"
echo "  ──────────────────────────────────────"
echo "  Overhead per task:       5,500 tokens ✅"
echo ""
echo "SAVINGS: 9,500 tokens per task (63% reduction!)"
echo "Monthly impact (50 interactions): 475,000 tokens"
echo ""

# 5. Recommendations
echo "🎯 RECOMMENDATIONS"
echo "────────────────────────────────"
echo ""
echo "✅ IMPLEMENT IMMEDIATELY:"
echo "  1. GL-043: Smart Attachments (path-specific loading)"
echo "  2. GL-044: Fragment-Based Access (grep → read targeted)"
echo "  3. GL-045: KB-MCP Queries (on-demand, not pre-loaded)"
echo ""
echo "⚠️  REFACTOR NEEDED:"

# Check instruction file sizes
for file in .github/instructions/*.instructions.md; do
    if [ -f "$file" ]; then
        size=$(wc -c < "$file")
        size_kb=$((size / 1024))
        
        if [ "$size_kb" -gt 5 ]; then
            filename=$(basename "$file")
            echo "  - $filename ($size_kb KB) → Reduce to <3 KB"
        fi
    fi
done

echo ""
echo "📈 NEXT AUDIT: $(date -d '+30 days' 2>/dev/null || echo 'In 30 days')"
echo ""
