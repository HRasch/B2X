#!/bin/bash

# Lessons Learned Search Tool
# Usage: ./search-lessons.sh [search-term] [category]

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
LESSONS_DIR="$SCRIPT_DIR/../.ai/lessons"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_header() {
    echo -e "${BLUE}=== $1 ===${NC}"
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

# Check if lessons directory exists
if [ ! -d "$LESSONS_DIR" ]; then
    print_error "Lessons learned directory not found: $LESSONS_DIR"
    exit 1
fi

# Parse arguments
SEARCH_TERM="$1"
CATEGORY="$2"

# Function to search in specific category
search_category() {
    local category_dir="$1"
    local category_name="$2"

    if [ -d "$category_dir" ]; then
        print_header "Searching $category_name"

        if [ -n "$SEARCH_TERM" ]; then
            # Search for the term in the category
            local results=$(find "$category_dir" -name "*.md" -exec grep -l -i "$SEARCH_TERM" {} \; 2>/dev/null)

            if [ -n "$results" ]; then
                echo "$results" | while read -r file; do
                    local relative_path="${file#$LESSONS_DIR/}"
                    echo "📄 $relative_path"

                    # Show context around the search term
                    grep -i -A 2 -B 2 "$SEARCH_TERM" "$file" | head -10 | sed 's/^/    /'
                    echo
                done
            else
                echo "No matches found for '$SEARCH_TERM' in $category_name"
            fi
        else
            # List all files in category
            find "$category_dir" -name "*.md" | sed "s|$category_dir/|📄 |" | head -10

            local file_count=$(find "$category_dir" -name "*.md" | wc -l)
            if [ "$file_count" -gt 10 ]; then
                echo "... and $((file_count - 10)) more files"
            fi
        fi
        echo
    fi
}

# Main search logic
if [ -n "$CATEGORY" ]; then
    # Search specific category
    case "$CATEGORY" in
        "incidents"|"patterns"|"prevention"|"metrics")
            search_category "$LESSONS_DIR/$CATEGORY" "$CATEGORY"
            ;;
        *)
            print_error "Invalid category: $CATEGORY"
            echo "Valid categories: incidents, patterns, prevention, metrics"
            exit 1
            ;;
    esac
else
    # Search all categories
    print_header "Lessons Learned Search"
    echo "Search Term: ${SEARCH_TERM:-'(all files)'}"
    echo

    search_category "$LESSONS_DIR/incidents" "Incidents"
    search_category "$LESSONS_DIR/patterns" "Patterns"
    search_category "$LESSONS_DIR/prevention" "Prevention"
    search_category "$LESSONS_DIR/metrics" "Metrics"
fi

# Show usage if no arguments
if [ $# -eq 0 ]; then
    echo
    print_header "Usage Examples"
    echo "# Search for 'database' in all categories"
    echo "./search-lessons.sh database"
    echo
    echo "# Search for 'authentication' in incidents only"
    echo "./search-lessons.sh authentication incidents"
    echo
    echo "# List all incident reports"
    echo "./search-lessons.sh '' incidents"
    echo
    echo "# Show this help"
    echo "./search-lessons.sh"
fi

print_success "Search completed"