#!/bin/bash

# Markdown Fragment Reader for Token Optimization
# Intelligently extracts important sections from large markdown files
# Usage: ./markdown-fragment-reader.sh <file-path> [max-lines]

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

FILE_PATH="$1"
MAX_LINES="${2:-100}"  # Default 100 lines if not specified

if [ -z "$FILE_PATH" ]; then
    echo "Usage: $0 <markdown-file-path> [max-lines]"
    echo "Example: $0 docs/large-guide.md 150"
    exit 1
fi

if [ ! -f "$FILE_PATH" ]; then
    echo "Error: File '$FILE_PATH' not found"
    exit 1
fi

echo "🔍 Markdown Fragment Reader"
echo "==========================="
echo "File: $FILE_PATH"
echo "Max lines: $MAX_LINES"
echo ""

# Get total lines
TOTAL_LINES=$(wc -l < "$FILE_PATH")
echo "📊 File Stats:"
echo "   Total lines: $TOTAL_LINES"
echo "   File size: $(ls -lh "$FILE_PATH" | awk '{print $5}')"
echo ""

# Calculate token estimate (rough approximation)
TOTAL_CHARS=$(wc -c < "$FILE_PATH")
ESTIMATED_TOKENS=$((TOTAL_CHARS / 4))  # ~4 chars per token
echo "💰 Token Analysis:"
echo "   Full file tokens: ~$ESTIMATED_TOKENS"
echo ""

# Extract frontmatter (between --- markers)
echo "📄 Extracted Content:"
echo "===================="

FRONTMATTER=$(sed -n '/^---$/,/^---$/p' "$FILE_PATH" | head -20)
if [ -n "$FRONTMATTER" ]; then
    echo "$FRONTMATTER"
    echo ""
fi

# Extract major headers and their first few lines
echo "## Major Sections:"
HEADERS=$(grep -n '^#' "$FILE_PATH" | head -10)
if [ -n "$HEADERS" ]; then
    echo "$HEADERS" | while IFS=: read -r line_num header; do
        echo "Line $line_num: $header"
        # Show first 2 lines after header
        sed -n "$((line_num+1)),$((line_num+2))p" "$FILE_PATH" | sed 's/^/  /'
        echo ""
    done
fi

# Calculate lines used so far
LINES_USED=$(echo "$FRONTMATTER" | wc -l)
LINES_USED=$((LINES_USED + 20))  # Rough estimate for headers section

REMAINING_LINES=$((MAX_LINES - LINES_USED))

if [ $REMAINING_LINES -gt 0 ]; then
    echo "## Content Sample:"
    # Show first part of main content (skip frontmatter and headers)
    MAIN_CONTENT_START=$(grep -n '^#' "$FILE_PATH" | tail -1 | cut -d: -f1)
    if [ -n "$MAIN_CONTENT_START" ]; then
        MAIN_CONTENT_START=$((MAIN_CONTENT_START + 5))  # Skip header lines
        sed -n "${MAIN_CONTENT_START},$((MAIN_CONTENT_START + REMAINING_LINES/2))p" "$FILE_PATH"
        echo ""
        echo "... [Content continues for $((TOTAL_LINES - MAIN_CONTENT_START - REMAINING_LINES/2)) more lines] ..."
        echo ""
    fi

    # Show end of file
    echo "## End Section:"
    tail -n $((REMAINING_LINES/2)) "$FILE_PATH"
fi

# Calculate savings
FRAGMENT_CHARS=$(echo "$FRONTMATTER$HEADERS" | wc -c)
FRAGMENT_CHARS=$((FRAGMENT_CHARS + 1000))  # Rough estimate for content samples
FRAGMENT_TOKENS=$((FRAGMENT_CHARS / 4))
SAVINGS_PERCENT=$(( (ESTIMATED_TOKENS - FRAGMENT_TOKENS) * 100 / ESTIMATED_TOKENS ))

echo ""
echo "💰 Token Savings:"
echo "   Fragment tokens: ~$FRAGMENT_TOKENS"
echo "   Savings: $SAVINGS_PERCENT%"
echo ""
echo "✅ Fragment extraction complete. Use this content for AI-assisted editing."