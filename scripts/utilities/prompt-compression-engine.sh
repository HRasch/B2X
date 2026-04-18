#!/bin/bash

# GL-049: Prompt Compression Engine
# Expands shorthand notation into full prompts for token optimization

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Macro definitions (using simple variables since associative arrays may not work in all bash versions)
FE_COMP="functional components with hooks, TypeScript, single responsibility, custom hooks for reusable logic"
FE_STATE="local state for component-specific data, context/global state sparingly, avoid prop drilling >2 levels, proper loading/error states"
FE_STYLE="responsive design (mobile-first), accessibility (WCAG), consistent spacing/sizing, project styling conventions"
FE_PERF="lazy loading for routes/components, memoize expensive computations, optimize re-renders with React.memo/useMemo, image optimization"
FE_UX="immediate feedback on actions, graceful loading states, meaningful error messages, proper form validation"
FE_I18N="ZERO hardcoded strings, use \$t() in templates or useI18n() in scripts, key pattern: module.section.key, English source of truth"

BE_CODE="async/await for async operations, proper error handling with typed exceptions, input validation on public endpoints, dependency injection"
BE_API="RESTful conventions, proper HTTP status codes, consistent error response format, OpenAPI/Swagger documentation"
BE_DB="parameterized queries (prevent SQL injection), connection pooling, database migrations for schema changes, index frequently queried columns"
BE_SEC="never expose internal errors, sanitize all user inputs, rate limiting on public endpoints, environment variables for secrets"
BE_TEST="unit tests for business logic, integration tests for API endpoints, mock external dependencies, >80% code coverage"
BE_LOC="all messages translated, return translation keys not hardcoded strings, use IStringLocalizer<T>, support: en,de,fr,es,it,pt,nl,pl"

QA_UNIT="test business logic in isolation, mock dependencies, test edge cases, maintain >80% coverage"
QA_INTEG="test API endpoints, database interactions, external service calls, end-to-end workflows"
QA_E2E="test complete user journeys, cross-browser compatibility, performance under load"
QA_COV="track coverage metrics, identify uncovered code, set coverage targets by module"

SEC_VALID="input sanitization, type validation, length limits, format checking"
SEC_AUTH="proper authentication, authorization checks, session management, secure tokens"
SEC_AUDIT="log security events, monitor suspicious activity, regular security reviews"
SEC_COMPLY="GDPR compliance, data encryption, secure data handling, privacy by design"

ARCH_CQRS="separate read/write models, command/query handlers, event sourcing where appropriate"
ARCH_EVENT="domain events, event handlers, eventual consistency, message queuing"
ARCH_DOMAIN="rich domain models, business logic encapsulation, validation in domain layer"

# Action mappings
declare -A ACTIONS=(
    ["NEW"]="create new"
    ["EDIT"]="modify existing"
    ["DELETE"]="remove"
    ["TEST"]="write tests for"
    ["REVIEW"]="review and improve"
    ["OPTIMIZE"]="optimize performance of"
    ["SECURE"]="add security to"
    ["DOCUMENT"]="document"
)

# Scope mappings
declare -A SCOPES=(
    ["COMP"]="component"
    ["API"]="API endpoint"
    ["MODEL"]="data model"
    ["SERVICE"]="service class"
    ["TEST"]="test suite"
    ["CONFIG"]="configuration"
    ["DOC"]="documentation"
)

function log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

function log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

function log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

function log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

function expand_macro() {
    local shorthand="$1"
    local macro_key=""
    local scope=""

    # Parse shorthand: DOMAIN.ACTION.SCOPE
    if [[ $shorthand =~ ^([A-Z]+)\.([A-Z]+)\.(.+)$ ]]; then
        macro_key="${BASH_REMATCH[1]}_${BASH_REMATCH[2]}"
        scope="${BASH_REMATCH[3]}"
    elif [[ $shorthand =~ ^([A-Z]+)\.([A-Z]+)$ ]]; then
        macro_key="${BASH_REMATCH[1]}_${BASH_REMATCH[2]}"
    else
        log_error "Invalid shorthand format: $shorthand"
        echo "Usage: DOMAIN.ACTION[.SCOPE] [additional context]"
        exit 1
    fi

    # Use case statement to map macros
    local expansion=""
    case "$macro_key" in
        "FE_COMP") expansion="$FE_COMP" ;;
        "FE_STATE") expansion="$FE_STATE" ;;
        "FE_STYLE") expansion="$FE_STYLE" ;;
        "FE_PERF") expansion="$FE_PERF" ;;
        "FE_UX") expansion="$FE_UX" ;;
        "FE_I18N") expansion="$FE_I18N" ;;
        "BE_CODE") expansion="$BE_CODE" ;;
        "BE_API") expansion="$BE_API" ;;
        "BE_DB") expansion="$BE_DB" ;;
        "BE_SEC") expansion="$BE_SEC" ;;
        "BE_TEST") expansion="$BE_TEST" ;;
        "BE_LOC") expansion="$BE_LOC" ;;
        "QA_UNIT") expansion="$QA_UNIT" ;;
        "QA_INTEG") expansion="$QA_INTEG" ;;
        "QA_E2E") expansion="$QA_E2E" ;;
        "QA_COV") expansion="$QA_COV" ;;
        "SEC_VALID") expansion="$SEC_VALID" ;;
        "SEC_AUTH") expansion="$SEC_AUTH" ;;
        "SEC_AUDIT") expansion="$SEC_AUDIT" ;;
        "SEC_COMPLY") expansion="$SEC_COMPLY" ;;
        "ARCH_CQRS") expansion="$ARCH_CQRS" ;;
        "ARCH_EVENT") expansion="$ARCH_EVENT" ;;
        "ARCH_DOMAIN") expansion="$ARCH_DOMAIN" ;;
        *) expansion="" ;;
    esac

    # Check if macro exists
    if [[ -z "$expansion" ]]; then
        log_error "Unknown macro: $macro_key"
        echo "Available macros: FE_*, BE_*, QA_*, SEC_*, ARCH_*"
        exit 1
    fi

    # Add scope if provided
    if [[ -n "$scope" ]]; then
        expansion="$expansion (scope: $scope)"
    fi

    echo "$expansion"
}

function compress_prompt() {
    local prompt="$1"

    # Simple compression logic - look for patterns and suggest shorthand
    local suggestions=""

    if [[ $prompt =~ (component|Component) ]]; then
        suggestions="$suggestions FE.COMP"
    fi

    if [[ $prompt =~ (API|endpoint|Endpoint) ]]; then
        suggestions="$suggestions BE.API"
    fi

    if [[ $prompt =~ (test|Test|testing) ]]; then
        suggestions="$suggestions QA.UNIT"
    fi

    if [[ $prompt =~ (security|Security|secure) ]]; then
        suggestions="$suggestions SEC.VALID"
    fi

    if [[ -n "$suggestions" ]]; then
        log_info "Suggested compressed versions:"
        echo "$suggestions" | tr ' ' '\n' | sed 's/^/  /'
    fi
}

function show_usage() {
    echo "GL-049: Prompt Compression Engine"
    echo ""
    echo "Usage:"
    echo "  $0 expand <shorthand> [context]    # Expand shorthand to full prompt"
    echo "  $0 compress <prompt>              # Suggest compression for verbose prompt"
    echo "  $0 list                            # List all available macros"
    echo "  $0 test <shorthand>                # Test macro expansion"
    echo ""
    echo "Examples:"
    echo "  $0 expand FE.COMP.NEW              # Expand frontend component creation"
    echo "  $0 compress 'Create a new Vue component with TypeScript'"
    echo "  $0 test BE.API.POST"
}

function list_macros() {
    echo "Available Macros:"
    echo "=================="
    echo ""
    echo "Frontend (FE):"
    echo "  FE.COMP  - $FE_COMP"
    echo "  FE.STATE - $FE_STATE"
    echo "  FE.STYLE - $FE_STYLE"
    echo "  FE.PERF  - $FE_PERF"
    echo "  FE.UX    - $FE_UX"
    echo "  FE.I18N  - $FE_I18N"
    echo ""
    echo "Backend (BE):"
    echo "  BE.CODE  - $BE_CODE"
    echo "  BE.API   - $BE_API"
    echo "  BE.DB    - $BE_DB"
    echo "  BE.SEC   - $BE_SEC"
    echo "  BE.TEST  - $BE_TEST"
    echo "  BE.LOC   - $BE_LOC"
    echo ""
    echo "Quality Assurance (QA):"
    echo "  QA.UNIT  - $QA_UNIT"
    echo "  QA.INTEG - $QA_INTEG"
    echo "  QA.E2E   - $QA_E2E"
    echo "  QA.COV   - $QA_COV"
    echo ""
    echo "Security (SEC):"
    echo "  SEC.VALID - $SEC_VALID"
    echo "  SEC.AUTH  - $SEC_AUTH"
    echo "  SEC.AUDIT - $SEC_AUDIT"
    echo "  SEC.COMPLY- $SEC_COMPLY"
    echo ""
    echo "Architecture (ARCH):"
    echo "  ARCH.CQRS  - $ARCH_CQRS"
    echo "  ARCH.EVENT - $ARCH_EVENT"
    echo "  ARCH.DOMAIN- $ARCH_DOMAIN"
}

function test_macro() {
    local shorthand="$1"
    log_info "Testing macro: $shorthand"

    local start_time=$(date +%s%N)
    local expansion=$(expand_macro "$shorthand")
    local end_time=$(date +%s%N)

    local duration=$(( (end_time - start_time) / 1000000 )) # milliseconds

    log_success "Expansion completed in ${duration}ms"
    echo ""
    echo "Expanded prompt:"
    echo "$expansion"
    echo ""

    # Estimate token savings
    local shorthand_tokens=$(echo "$shorthand" | wc -w)
    local expansion_tokens=$(echo "$expansion" | wc -w)
    local savings=$(( (expansion_tokens - shorthand_tokens) * 100 / expansion_tokens ))

    echo "Token Analysis:"
    echo "  Shorthand tokens: $shorthand_tokens"
    echo "  Expansion tokens: $expansion_tokens"
    echo "  Estimated savings: ${savings}%"
}

# Main command handling
case "${1:-}" in
    "expand")
        if [[ -z "$2" ]]; then
            log_error "Missing shorthand argument"
            show_usage
            exit 1
        fi
        expand_macro "$2" "$3"
        ;;
    "compress")
        if [[ -z "$2" ]]; then
            log_error "Missing prompt argument"
            show_usage
            exit 1
        fi
        compress_prompt "$2"
        ;;
    "list")
        list_macros
        ;;
    "test")
        if [[ -z "$2" ]]; then
            log_error "Missing shorthand argument"
            show_usage
            exit 1
        fi
        test_macro "$2"
        ;;
    *)
        show_usage
        exit 1
        ;;
esac</content>
<parameter name="filePath">c:\Users\Holge\repos\B2Connect\scripts\prompt-compression-engine.sh