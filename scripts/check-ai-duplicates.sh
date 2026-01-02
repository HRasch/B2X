#!/bin/bash
# ============================================================================
# check-ai-duplicates.sh - Prevents duplicate files in .ai/ folder
# ============================================================================
# This script checks for files/folders with " 2", " 3", etc. naming patterns
# which indicate accidental duplicates (common with macOS Finder operations).
#
# Usage:
#   ./scripts/check-ai-duplicates.sh           # Check only
#   ./scripts/check-ai-duplicates.sh --fix     # Delete duplicates
#   ./scripts/check-ai-duplicates.sh --pre-commit  # For Git hooks
#
# Exit codes:
#   0 - No duplicates found
#   1 - Duplicates found (or deleted with --fix)
#
# Author: @SARAH (AI Coordinator)
# Date: 2026-01-02
# ============================================================================

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Get script directory and project root
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
AI_DIR="$PROJECT_ROOT/.ai"

# Parse arguments
FIX_MODE=false
PRE_COMMIT=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --fix)
            FIX_MODE=true
            shift
            ;;
        --pre-commit)
            PRE_COMMIT=true
            shift
            ;;
        *)
            shift
            ;;
    esac
done

echo "🔍 Checking for duplicate files in .ai/ folder..."

# Find all duplicates (files and folders with " 2", " 3", etc. pattern)
DUPLICATE_FILES=$(find "$AI_DIR" -name "* [0-9]*" -type f 2>/dev/null || true)
DUPLICATE_FOLDERS=$(find "$AI_DIR" -name "* [0-9]*" -type d 2>/dev/null || true)

FILE_COUNT=$(echo -n "$DUPLICATE_FILES" | grep -c '^' 2>/dev/null || echo 0)
FOLDER_COUNT=$(echo -n "$DUPLICATE_FOLDERS" | grep -c '^' 2>/dev/null || echo 0)

# Handle empty results
if [[ -z "$DUPLICATE_FILES" ]]; then
    FILE_COUNT=0
fi
if [[ -z "$DUPLICATE_FOLDERS" ]]; then
    FOLDER_COUNT=0
fi

TOTAL_COUNT=$((FILE_COUNT + FOLDER_COUNT))

if [[ $TOTAL_COUNT -eq 0 ]]; then
    echo -e "${GREEN}✅ No duplicate files or folders found in .ai/${NC}"
    exit 0
fi

echo -e "${RED}❌ Found $TOTAL_COUNT duplicates in .ai/:${NC}"
echo ""

if [[ $FILE_COUNT -gt 0 ]]; then
    echo -e "${YELLOW}📄 Duplicate Files ($FILE_COUNT):${NC}"
    echo "$DUPLICATE_FILES" | while read -r file; do
        if [[ -n "$file" ]]; then
            echo "   - ${file#$PROJECT_ROOT/}"
        fi
    done
    echo ""
fi

if [[ $FOLDER_COUNT -gt 0 ]]; then
    echo -e "${YELLOW}📁 Duplicate Folders ($FOLDER_COUNT):${NC}"
    echo "$DUPLICATE_FOLDERS" | while read -r folder; do
        if [[ -n "$folder" ]]; then
            echo "   - ${folder#$PROJECT_ROOT/}"
        fi
    done
    echo ""
fi

if [[ "$FIX_MODE" == true ]]; then
    echo -e "${YELLOW}🗑️  Deleting duplicates...${NC}"
    
    # Delete files first
    if [[ -n "$DUPLICATE_FILES" ]]; then
        echo "$DUPLICATE_FILES" | while read -r file; do
            if [[ -n "$file" && -f "$file" ]]; then
                rm -f "$file"
                echo "   Deleted: ${file#$PROJECT_ROOT/}"
            fi
        done
    fi
    
    # Delete folders
    if [[ -n "$DUPLICATE_FOLDERS" ]]; then
        echo "$DUPLICATE_FOLDERS" | while read -r folder; do
            if [[ -n "$folder" && -d "$folder" ]]; then
                rm -rf "$folder"
                echo "   Deleted: ${folder#$PROJECT_ROOT/}"
            fi
        done
    fi
    
    echo -e "${GREEN}✅ Cleanup complete${NC}"
    exit 1  # Exit with 1 to indicate changes were made
fi

if [[ "$PRE_COMMIT" == true ]]; then
    echo -e "${RED}🚫 Commit blocked: Duplicate files detected in .ai/${NC}"
    echo ""
    echo "To fix, run:"
    echo "  ./scripts/check-ai-duplicates.sh --fix"
    echo ""
    echo "Or manually resolve the duplicates by:"
    echo "  1. Comparing the duplicate with the original"
    echo "  2. Merging any unique content into the original"
    echo "  3. Deleting the duplicate (file with ' 2', ' 3', etc. suffix)"
    exit 1
fi

echo -e "${YELLOW}💡 To fix, run: ./scripts/check-ai-duplicates.sh --fix${NC}"
exit 1
