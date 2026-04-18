#!/bin/bash

# MCP-Powered Validation Checklist Script
# Pre/Post-Edit Validation for Multi-Language Fragment Editing Strategy
# Integrates with validate-large-file-edit.sh for comprehensive quality gates
# Usage: ./mcp-validation-checklist.sh <file-path> <phase> [language]
# Phase: pre-edit | post-edit

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

FILE_PATH="$1"
PHASE="$2"
LANGUAGE="${3:-auto}"

if [ -z "$FILE_PATH" ] || [ -z "$PHASE" ]; then
    echo "Usage: $0 <file-path> <phase> [language]"
    echo "Phases: pre-edit, post-edit"
    echo "Languages: dotnet, typescript, vue, database, infrastructure, test, auto"
    exit 1
fi

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🔍 MCP Validation Checklist${NC}"
echo "=============================="
echo "File: $FILE_PATH"
echo "Phase: $PHASE"
echo "Language: $LANGUAGE"
echo "Date: $(date)"
echo ""

# Auto-detect language if not specified
if [ "$LANGUAGE" = "auto" ]; then
    case "$FILE_PATH" in
        *.cs|*.csproj|*.slnx) LANGUAGE="dotnet" ;;
        *.ts|*.tsx) LANGUAGE="typescript" ;;
        *.vue) LANGUAGE="vue" ;;
        *.sql|*.db) LANGUAGE="database" ;;
        *Dockerfile*|*.yml|*.yaml|*.json) LANGUAGE="infrastructure" ;;
        *.test.*|*.spec.*|*Test.*|*Tests.*) LANGUAGE="test" ;;
        *.md) LANGUAGE="documentation" ;;
        *) LANGUAGE="unknown" ;;
    esac
    echo "Auto-detected language: $LANGUAGE"
fi

VALIDATION_LOG="$REPO_ROOT/.ai/logs/mcp-validation/$(basename "$FILE_PATH")-$(date +%Y%m%d-%H%M%S)-$PHASE.log"
mkdir -p "$(dirname "$VALIDATION_LOG")"

echo "Validation log: $VALIDATION_LOG"
echo ""

# Initialize validation results
PASSED=0
FAILED=0
WARNINGS=0

log_result() {
    local status="$1"
    local message="$2"
    local details="$3"

    case "$status" in
        "PASS")
            echo -e "${GREEN}✅ $message${NC}"
            ((PASSED++))
            ;;
        "FAIL")
            echo -e "${RED}❌ $message${NC}"
            ((FAILED++))
            ;;
        "WARN")
            echo -e "${YELLOW}⚠️  $message${NC}"
            ((WARNINGS++))
            ;;
    esac

    echo "$(date '+%Y-%m-%d %H:%M:%S') [$status] $message - $details" >> "$VALIDATION_LOG"
}

# Common validations for all languages
echo "📋 Common Validation Checks"
echo "==========================="

# File existence and accessibility
if [ -f "$FILE_PATH" ] && [ -r "$FILE_PATH" ]; then
    log_result "PASS" "File exists and is readable" "Path: $FILE_PATH"
else
    log_result "FAIL" "File does not exist or is not readable" "Path: $FILE_PATH"
    exit 1
fi

# File size check (GL-043: Fragment editing for large files)
FILE_SIZE=$(stat -f%z "$FILE_PATH" 2>/dev/null || stat -c%s "$FILE_PATH" 2>/dev/null || echo "0")
if [ "$FILE_SIZE" -gt 524288000 ]; then # 500MB
    log_result "FAIL" "File too large for direct editing" "Size: $FILE_SIZE bytes (>500MB)"
    exit 1
elif [ "$FILE_SIZE" -gt 1048576 ]; then # 1MB
    log_result "WARN" "Large file - consider fragment editing" "Size: $FILE_SIZE bytes (>1MB)"
else
    log_result "PASS" "File size acceptable" "Size: $FILE_SIZE bytes"
fi

# Backup creation for post-edit validation
if [ "$PHASE" = "pre-edit" ]; then
    BACKUP_FILE="$REPO_ROOT/.ai/backups/$(basename "$FILE_PATH").$(date +%Y%m%d-%H%M%S).backup"
    mkdir -p "$(dirname "$BACKUP_FILE")"
    cp "$FILE_PATH" "$BACKUP_FILE"
    echo "BACKUP_CREATED=$BACKUP_FILE" >> "$VALIDATION_LOG"
    log_result "PASS" "Pre-edit backup created" "Backup: $BACKUP_FILE"
fi

# Language-specific validations
echo ""
echo "🔧 Language-Specific Validation Checks"
echo "======================================"

case "$LANGUAGE" in
    "dotnet")
        # C#/.NET validations
        if command -v dotnet >/dev/null 2>&1; then
            log_result "PASS" "DotNet CLI available" "Version: $(dotnet --version)"

            # Syntax check
            if dotnet build "$FILE_PATH" --verbosity quiet --nologo 2>/dev/null; then
                log_result "PASS" "C# syntax validation" "File compiles successfully"
            else
                log_result "FAIL" "C# syntax errors detected" "Run 'dotnet build' for details"
            fi
        else
            log_result "WARN" "DotNet CLI not available" "Cannot perform C# validation"
        fi

        # Roslyn MCP integration check
        if [ -f ".ai/mcp/roslyn-mcp.json" ]; then
            log_result "PASS" "Roslyn MCP configured" "Available for fragment editing"
        else
            log_result "WARN" "Roslyn MCP not configured" "Limited fragment editing capabilities"
        fi
        ;;

    "typescript")
        # TypeScript validations
        if command -v npx >/dev/null 2>&1; then
            log_result "PASS" "Node.js/npm available" "Can perform TypeScript validation"

            # Type checking
            if npx tsc --noEmit "$FILE_PATH" 2>/dev/null; then
                log_result "PASS" "TypeScript compilation" "No type errors detected"
            else
                log_result "FAIL" "TypeScript errors detected" "Run 'npx tsc --noEmit' for details"
            fi
        else
            log_result "WARN" "Node.js not available" "Cannot perform TypeScript validation"
        fi

        # TypeScript MCP check
        if [ -f ".ai/mcp/typescript-mcp.json" ]; then
            log_result "PASS" "TypeScript MCP configured" "Available for fragment editing"
        else
            log_result "WARN" "TypeScript MCP not configured" "Limited fragment editing capabilities"
        fi
        ;;

    "vue")
        # Vue.js validations
        if command -v npm >/dev/null 2>&1; then
            # Check for Vue SFC structure
            if grep -q "<template>" "$FILE_PATH" && grep -q "<script" "$FILE_PATH"; then
                log_result "PASS" "Vue SFC structure valid" "Template and script sections present"
            else
                log_result "WARN" "Vue SFC structure incomplete" "Missing template or script section"
            fi
        fi

        # Vue MCP check
        if [ -f ".ai/mcp/vue-mcp.json" ]; then
            log_result "PASS" "Vue MCP configured" "Available for fragment editing"
        else
            log_result "WARN" "Vue MCP not configured" "Limited fragment editing capabilities"
        fi
        ;;

    "database")
        # Database/SQL validations
        if command -v sqlcmd >/dev/null 2>&1 || command -v mysql >/dev/null 2>&1; then
            log_result "PASS" "Database client available" "Can perform SQL validation"
        else
            log_result "WARN" "No database client found" "Cannot perform SQL validation"
        fi

        # Basic SQL syntax check
        if grep -q "SELECT\|INSERT\|UPDATE\|DELETE" "$FILE_PATH"; then
            log_result "PASS" "SQL keywords detected" "Appears to be valid SQL file"
        else
            log_result "WARN" "No SQL keywords found" "May not be a SQL file"
        fi
        ;;

    "infrastructure")
        # Infrastructure validations (Docker, YAML, etc.)
        case "$FILE_PATH" in
            *Dockerfile*)
                if command -v docker >/dev/null 2>&1; then
                    if docker build --dry-run -f "$FILE_PATH" . 2>/dev/null; then
                        log_result "PASS" "Dockerfile syntax valid" "Dry run successful"
                    else
                        log_result "FAIL" "Dockerfile syntax errors" "Check Docker syntax"
                    fi
                fi
                ;;
            *.yml|*.yaml)
                if command -v yamllint >/dev/null 2>&1; then
                    if yamllint "$FILE_PATH" 2>/dev/null; then
                        log_result "PASS" "YAML syntax valid" "yamllint passed"
                    else
                        log_result "FAIL" "YAML syntax errors" "Run yamllint for details"
                    fi
                fi
                ;;
        esac

        # Infrastructure MCP check
        if [ -f ".ai/mcp/docker-mcp.json" ]; then
            log_result "PASS" "Infrastructure MCP configured" "Available for fragment editing"
        else
            log_result "WARN" "Infrastructure MCP not configured" "Limited fragment editing capabilities"
        fi
        ;;

    "test")
        # Test file validations
        if [[ "$FILE_PATH" =~ \.test\.|\.spec\.|Test\.|Tests\. ]]; then
            log_result "PASS" "Test file naming convention" "Follows standard naming"
        else
            log_result "WARN" "Non-standard test file name" "Consider renaming to *.test.* or *.spec.*"
        fi

        # Testing MCP check
        if [ -f ".ai/mcp/testing-mcp.json" ]; then
            log_result "PASS" "Testing MCP configured" "Available for fragment editing"
        else
            log_result "WARN" "Testing MCP not configured" "Limited fragment editing capabilities"
        fi
        ;;

    "documentation")
        # Documentation validations
        if head -5 "$FILE_PATH" | grep -q "^---$"; then
            log_result "PASS" "Frontmatter present" "Markdown frontmatter detected"
        else
            log_result "WARN" "No frontmatter found" "Consider adding frontmatter for better organization"
        fi

        # Check for broken links (basic)
        BROKEN_LINKS=$(grep -o '\[.*\](\([^)]*\))' "$FILE_PATH" | grep -v "http" | wc -l)
        if [ "$BROKEN_LINKS" -eq 0 ]; then
            log_result "PASS" "No broken relative links detected" "All links appear valid"
        else
            log_result "WARN" "Potential broken links" "$BROKEN_LINKS relative links found - verify manually"
        fi
        ;;

    *)
        log_result "WARN" "Unknown language" "Limited validation capabilities for $LANGUAGE"
        ;;
esac

# MCP Integration checks
echo ""
echo "🔗 MCP Integration Checks"
echo "========================="

# Check for MCP server availability
if pgrep -f "mcp-server" >/dev/null 2>&1; then
    log_result "PASS" "MCP server running" "Available for fragment editing"
else
    log_result "WARN" "MCP server not running" "Start MCP server for full functionality"
fi

# Check temp file manager integration
if [ -f "$REPO_ROOT/scripts/temp-file-manager.sh" ]; then
    log_result "PASS" "Temp file manager available" "Token optimization ready"
else
    log_result "WARN" "Temp file manager missing" "Limited token optimization"
fi

# Fragment editing capability check
FRAGMENT_SUPPORT=$(find "$REPO_ROOT/.ai/mcp" -name "*-mcp.json" 2>/dev/null | wc -l)
if [ "$FRAGMENT_SUPPORT" -gt 0 ]; then
    log_result "PASS" "Fragment editing supported" "$FRAGMENT_SUPPORT MCP configurations found"
else
    log_result "FAIL" "No fragment editing support" "Configure MCP servers for language support"
fi

# Post-edit specific validations
if [ "$PHASE" = "post-edit" ]; then
    echo ""
    echo "🔄 Post-Edit Validation Checks"
    echo "=============================="

    # Check for backup comparison
    BACKUP_FILE=$(grep "BACKUP_CREATED=" "$VALIDATION_LOG" 2>/dev/null | cut -d'=' -f2)
    if [ -n "$BACKUP_FILE" ] && [ -f "$BACKUP_FILE" ]; then
        # Compare file sizes
        ORIGINAL_SIZE=$(stat -f%z "$BACKUP_FILE" 2>/dev/null || stat -c%s "$BACKUP_FILE" 2>/dev/null || echo "0")
        NEW_SIZE=$(stat -f%z "$FILE_PATH" 2>/dev/null || stat -c%s "$FILE_PATH" 2>/dev/null || echo "0")

        SIZE_DIFF=$((NEW_SIZE - ORIGINAL_SIZE))
        if [ "$SIZE_DIFF" -gt 0 ]; then
            log_result "PASS" "File size increased appropriately" "Change: +$SIZE_DIFF bytes"
        elif [ "$SIZE_DIFF" -lt 0 ]; then
            log_result "PASS" "File size reduced" "Change: $SIZE_DIFF bytes"
        else
            log_result "WARN" "File size unchanged" "Verify edits were applied"
        fi

        # Basic diff check
        DIFF_LINES=$(diff "$BACKUP_FILE" "$FILE_PATH" 2>/dev/null | wc -l)
        if [ "$DIFF_LINES" -gt 0 ]; then
            log_result "PASS" "File changes detected" "$DIFF_LINES lines changed"
        else
            log_result "WARN" "No changes detected" "Verify edits were applied correctly"
        fi
    else
        log_result "WARN" "No backup available for comparison" "Cannot verify changes"
    fi

    # Run integrated validation scripts
    echo ""
    echo "🔧 Integrated Validation Scripts"
    echo "==============================="

    # Run existing validate-large-file-edit.sh if available
    if [ -f "$REPO_ROOT/scripts/validate-large-file-edit.sh" ]; then
        if "$REPO_ROOT/scripts/validate-large-file-edit.sh" "$FILE_PATH" "$LANGUAGE" >> "$VALIDATION_LOG" 2>&1; then
            log_result "PASS" "Large file edit validation passed" "Integrated with existing script"
        else
            log_result "FAIL" "Large file edit validation failed" "Check integrated script output"
        fi
    fi

    # Run validate-metadata.sh for docs
    if [ "$LANGUAGE" = "documentation" ] && [ -f "$REPO_ROOT/scripts/validate-metadata.sh" ]; then
        if "$REPO_ROOT/scripts/validate-metadata.sh" "$FILE_PATH" >> "$VALIDATION_LOG" 2>&1; then
            log_result "PASS" "Metadata validation passed" "Documentation standards met"
        else
            log_result "FAIL" "Metadata validation failed" "Check documentation standards"
        fi
    fi
fi

# Final summary
echo ""
echo "📊 Validation Summary"
echo "===================="
echo "Passed: $PASSED"
echo "Failed: $FAILED"
echo "Warnings: $WARNINGS"
echo "Total checks: $((PASSED + FAILED + WARNINGS))"
echo ""

if [ "$FAILED" -eq 0 ]; then
    echo -e "${GREEN}✅ All critical validations passed!${NC}"
    if [ "$WARNINGS" -eq 0 ]; then
        echo -e "${GREEN}🎉 Perfect validation - ready for commit${NC}"
    else
        echo -e "${YELLOW}⚠️  Validation passed with warnings - review recommended${NC}"
    fi
    exit 0
else
    echo -e "${RED}❌ Critical validation failures detected${NC}"
    echo "Please address the failed checks before proceeding."
    exit 1
fi