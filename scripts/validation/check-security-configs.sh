#!/bin/bash
# check-security-configs.sh
# Check security configurations and certificates for refactoring impact

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_ROOT"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

print_header() {
    echo -e "${BLUE}========================================${NC}"
    echo -e "${BLUE}  Security Configuration Audit${NC}"
    echo -e "${BLUE}========================================${NC}"
}

print_section() {
    echo -e "${GREEN}[SECTION]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check for certificate files and references
check_certificates() {
    print_section "Checking Certificate Files and References"

    # Look for certificate files
    local cert_files=$(find . -name "*.pfx" -o -name "*.p12" -o -name "*.cer" -o -name "*.crt" -o -name "*.key" -o -name "*.pem" 2>/dev/null | grep -v node_modules)

    if [ -n "$cert_files" ]; then
        print_warning "Found certificate files:"
        echo "$cert_files"
        echo "These files may contain sensitive information and should be handled carefully."
        echo
    fi

    # Look for certificate references in code/config
    local cert_refs=$(grep -r -i -E "(certificate|cert|ssl|tls)" --include="*.cs" --include="*.json" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null)

    if [ -n "$cert_refs" ]; then
        echo "Found certificate references in code/config:"
        echo "$cert_refs" | head -10
        echo
    fi
}

# Function to check for secrets and sensitive data
check_secrets() {
    print_section "Checking for Secrets and Sensitive Data"

    # Look for potential secrets in config files
    local secret_patterns=("password" "secret" "key" "token" "apikey" "connectionstring")

    for pattern in "${secret_patterns[@]}"; do
        local matches=$(grep -r -i "$pattern" --include="*.json" --include="*.config" --include="*.yml" --include="*.yaml" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | grep -v "package" | grep -v "Directory.Build")

        if [ -n "$matches" ]; then
            print_warning "Found potential secrets with pattern '$pattern':"
            echo "$matches" | head -3
            echo
        fi
    done

    # Check for hardcoded secrets in code
    local hardcoded_secrets=$(grep -r -E "(password|secret|key).*[\"']" --include="*.cs" --include="*.ts" --include="*.js" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null)

    if [ -n "$hardcoded_secrets" ]; then
        print_error "Found potential hardcoded secrets in code:"
        echo "$hardcoded_secrets" | head -5
        echo
    fi
}

# Function to check authentication configurations
check_authentication() {
    print_section "Checking Authentication Configurations"

    # Look for authentication middleware
    local auth_middleware=$(grep -r "UseAuthentication\|AddAuthentication" --include="*.cs" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null)

    if [ -n "$auth_middleware" ]; then
        echo "Found authentication middleware configuration:"
        echo "$auth_middleware"
        echo
    fi

    # Look for JWT configuration
    local jwt_config=$(grep -r -i "jwt\|bearer" --include="*.cs" --include="*.json" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$jwt_config" ]; then
        echo "Found JWT/Bearer token configuration:"
        echo "$jwt_config" | head -5
        echo
    fi

    # Look for OAuth configuration
    local oauth_config=$(grep -r -i "oauth" --include="*.cs" --include="*.json" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$oauth_config" ]; then
        echo "Found OAuth configuration:"
        echo "$oauth_config" | head -5
        echo
    fi
}

# Function to check authorization configurations
check_authorization() {
    print_section "Checking Authorization Configurations"

    # Look for authorization middleware
    local authz_middleware=$(grep -r "UseAuthorization\|AddAuthorization" --include="*.cs" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null)

    if [ -n "$authz_middleware" ]; then
        echo "Found authorization middleware configuration:"
        echo "$authz_middleware"
        echo
    fi

    # Look for policy-based authorization
    local policies=$(grep -r "AddPolicy\|RequireRole\|RequireClaim" --include="*.cs" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null)

    if [ -n "$policies" ]; then
        echo "Found authorization policies:"
        echo "$policies" | head -5
        echo
    fi
}

# Function to check CORS configuration
check_cors() {
    print_section "Checking CORS Configuration"

    # Look for CORS setup
    local cors_config=$(grep -r -i "cors\|origins\|allowcredentials" --include="*.cs" --include="*.json" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$cors_config" ]; then
        echo "Found CORS configuration:"
        echo "$cors_config" | head -5
        echo
    fi
}

# Function to check HTTPS configuration
check_https() {
    print_section "Checking HTTPS Configuration"

    # Look for HTTPS redirection
    local https_redirect=$(grep -r "UseHttpsRedirection\|RequireHttps" --include="*.cs" --exclude-dir=".git" --exclude-dir="node_modules" --exclude-dir="bin" --exclude-dir="obj" . 2>/dev/null)

    if [ -n "$https_redirect" ]; then
        echo "Found HTTPS redirection configuration:"
        echo "$https_redirect"
        echo
    fi

    # Look for SSL/TLS settings
    local ssl_settings=$(grep -r -i "ssl\|tls\|hsts" --include="*.cs" --include="*.json" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$ssl_settings" ]; then
        echo "Found SSL/TLS settings:"
        echo "$ssl_settings" | head -5
        echo
    fi
}

# Function to check security headers
check_security_headers() {
    print_section "Checking Security Headers"

    # Look for security middleware
    local security_headers=$(grep -r -i -E "(security.*header|helmet|csp|x-frame|x-content)" --include="*.cs" --include="*.ts" --include="*.js" --include="*.json" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$security_headers" ]; then
        echo "Found security headers configuration:"
        echo "$security_headers" | head -5
        echo
    fi
}

# Function to check for security scanning configurations
check_security_scanning() {
    print_section "Checking Security Scanning Configurations"

    # Look for security scanning tools
    local security_tools=$(grep -r -i -E "(sonar|owasp|zap|burp|nessus|qualys)" --include="*.yml" --include="*.yaml" --include="*.json" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$security_tools" ]; then
        echo "Found security scanning tool configurations:"
        echo "$security_tools" | head -5
        echo
    fi
}

# Function to check for encryption at rest
check_encryption() {
    print_section "Checking Encryption at Rest"

    # Look for encryption configurations
    local encryption=$(grep -r -i -E "(encrypt|decrypt|cipher|aes|rsa)" --include="*.cs" --include="*.json" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | grep -v "package")

    if [ -n "$encryption" ]; then
        echo "Found encryption-related code/configuration:"
        echo "$encryption" | head -5
        echo
    fi
}

# Function to generate security impact report
generate_security_report() {
    print_section "Security Impact Summary"

    echo "Key security considerations for refactoring:"
    echo
    echo "1. **Certificates**: Backup and reconfigure certificate paths"
    echo "2. **Secrets**: Ensure secrets management still works with new paths"
    echo "3. **Authentication**: Test auth flows after path changes"
    echo "4. **Authorization**: Verify authorization policies still apply"
    echo "5. **CORS**: Update CORS origins if URLs change"
    echo "6. **HTTPS**: Ensure HTTPS configuration remains intact"
    echo "7. **Security Headers**: Verify security headers are still applied"
    echo "8. **Security Scanning**: Update scan configurations for new paths"
    echo "9. **Encryption**: Test encryption/decryption with moved files"
    echo
    print_error "SECURITY CRITICAL: Test all security configurations after refactoring!"
    echo
    echo "Recommended security validation steps:"
    echo "- Run full security scan after refactoring"
    echo "- Test authentication and authorization flows"
    echo "- Verify HTTPS and certificate functionality"
    echo "- Check security headers on all endpoints"
    echo "- Validate encryption/decryption operations"
    echo "- Test CORS policies with new URLs"
}

main() {
    print_header
    echo "Auditing security configurations for refactoring impact..."
    echo

    check_certificates
    check_secrets
    check_authentication
    check_authorization
    check_cors
    check_https
    check_security_headers
    check_security_scanning
    check_encryption

    generate_security_report
}

main "$@"