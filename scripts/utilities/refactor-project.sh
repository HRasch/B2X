#!/bin/bash
# refactor-project.sh
# Master script for B2X project refactoring to src/docs/tests structure
# Usage: ./refactor-project.sh [phase]
# Phases: all, structure, phase1, phase2, validate, update, test, rollback

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_ROOT"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if git is clean
check_git_clean() {
    if [ -n "$(git status --porcelain)" ]; then
        print_error "Git working directory is not clean. Please commit or stash changes first."
        exit 1
    fi
}

# Function to run a phase
run_phase() {
    local phase="$1"
    local script="$SCRIPT_DIR/${phase}.sh"

    if [ ! -f "$script" ]; then
        print_error "Script $script not found"
        exit 1
    fi

    print_status "Running phase: $phase"
    if bash "$script"; then
        print_success "Phase $phase completed successfully"
    else
        print_error "Phase $phase failed"
        exit 1
    fi
}

# Main logic
PHASE="${1:-help}"

case "$PHASE" in
    "all")
        print_status "Starting complete refactoring process..."
        check_git_clean

        run_phase "create-refactor-structure"
        git add -A && git commit -m "refactor: create new directory structure"

        run_phase "move-files-phase1"
        run_phase "validate-moves"
        git add -A && git commit -m "refactor: move data, docs, and config files"

        run_phase "move-files-phase2"
        run_phase "validate-moves"
        run_phase "update-references"
        git add -A && git commit -m "refactor: move source code and update references"

        run_phase "test-builds"
        print_success "Complete refactoring finished successfully!"
        ;;

    "structure")
        check_git_clean
        run_phase "create-refactor-structure"
        ;;

    "phase1")
        run_phase "move-files-phase1"
        run_phase "validate-moves"
        ;;

    "phase2")
        run_phase "move-files-phase2"
        run_phase "validate-moves"
        run_phase "update-references"
        ;;

    "validate")
        run_phase "validate-moves"
        ;;

    "update")
        run_phase "update-references"
        ;;

    "test")
        run_phase "test-builds"
        ;;

    "rollback")
        run_phase "rollback-refactor"
        ;;

    "help"|*)
        echo "B2X Project Refactoring Script"
        echo ""
        echo "Usage: $0 [phase]"
        echo ""
        echo "Phases:"
        echo "  all        - Run complete refactoring process"
        echo "  structure  - Create new directory structure only"
        echo "  phase1     - Move data, docs, config files"
        echo "  phase2     - Move source code and update references"
        echo "  validate   - Validate moves and structure"
        echo "  update     - Update file references only"
        echo "  test       - Test builds after refactoring"
        echo "  rollback   - Rollback all changes"
        echo "  help       - Show this help"
        echo ""
        echo "Examples:"
        echo "  $0 all                    # Complete refactoring"
        echo "  $0 structure             # Just create directories"
        echo "  $0 phase1 && $0 phase2   # Step-by-step approach"
        echo ""
        echo "Note: Make sure git working directory is clean before starting"
        ;;
esac