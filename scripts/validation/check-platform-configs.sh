#!/bin/bash
# check-platform-configs.sh
# Check platform-specific configurations (Docker, Kubernetes, CI/CD) for refactoring impact

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
    echo -e "${BLUE}  Platform Configuration Audit${NC}"
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

# Function to check Docker configurations
check_docker_configs() {
    print_section "Checking Docker Configurations"

    local docker_files=$(find . -name "Dockerfile*" -o -name "docker-compose*.yml" -o -name "docker-compose*.yaml" | grep -v node_modules)

    for file in $docker_files; do
        echo "Checking $file..."

        # Check for hardcoded paths in Docker files
        local hardcoded_paths=$(grep -E "(AppHost|Backend|Frontend|\./)" "$file" | grep -v "^#" | grep -v "FROM")

        if [ -n "$hardcoded_paths" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded_paths"
            echo
        fi

        # Check for volume mounts
        local volumes=$(grep -E "^\s*volumes:" "$file" -A 5)

        if [ -n "$volumes" ]; then
            echo "Found volume mounts in $file:"
            echo "$volumes"
            echo
        fi
    done
}

# Function to check Kubernetes configurations
check_kubernetes_configs() {
    print_section "Checking Kubernetes Configurations"

    local k8s_files=$(find . -name "*.yml" -o -name "*.yaml" | xargs grep -l "apiVersion\|kind:" | grep -v node_modules | grep -v docker-compose)

    for file in $k8s_files; do
        echo "Checking $file..."

        # Check for hardcoded paths in K8s manifests
        local hardcoded_paths=$(grep -E "(AppHost|Backend|Frontend)" "$file")

        if [ -n "$hardcoded_paths" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded_paths"
            echo
        fi

        # Check for ConfigMaps with paths
        local configmaps=$(grep -A 10 "kind: ConfigMap" "$file")

        if [ -n "$configmaps" ]; then
            echo "Found ConfigMap in $file (may contain paths):"
            echo "$configmaps" | head -10
            echo
        fi

        # Check for volume mounts
        local volumes=$(grep -A 5 "volumes:" "$file")

        if [ -n "$volumes" ]; then
            echo "Found volume configuration in $file:"
            echo "$volumes"
            echo
        fi
    done
}

# Function to check CI/CD configurations
check_ci_cd_configs() {
    print_section "Checking CI/CD Configurations"

    # Check GitHub Actions
    local github_actions=$(find .github -name "*.yml" -o -name "*.yaml" 2>/dev/null)

    for file in $github_actions; do
        echo "Checking $file..."

        # Check for hardcoded paths in workflows
        local hardcoded_paths=$(grep -E "(AppHost|Backend|Frontend|\./)" "$file" | grep -v "^#" | grep -v "uses:")

        if [ -n "$hardcoded_paths" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded_paths"
            echo
        fi

        # Check for working-directory settings
        local working_dir=$(grep -i "working-directory\|working_dir" "$file")

        if [ -n "$working_dir" ]; then
            echo "Found working directory settings in $file:"
            echo "$working_dir"
            echo
        fi
    done

    # Check for other CI/CD files
    local ci_files=$(find . -name ".travis.yml" -o -name "azure-pipelines.yml" -o -name "Jenkinsfile" -o -name "build.gradle" -o -name "pom.xml" 2>/dev/null)

    for file in $ci_files; do
        echo "Checking $file..."

        local hardcoded_paths=$(grep -E "(AppHost|Backend|Frontend)" "$file")

        if [ -n "$hardcoded_paths" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded_paths"
            echo
        fi
    done
}

# Function to check environment-specific configurations
check_environment_configs() {
    print_section "Checking Environment-Specific Configurations"

    # Check for environment files
    local env_files=$(find . -name ".env*" -o -name "appsettings.*.json" | grep -v node_modules)

    for file in $env_files; do
        echo "Checking $file..."

        # Check for paths in environment configs
        local paths=$(grep -E "(AppHost|Backend|Frontend|PATH|DIR)" "$file")

        if [ -n "$paths" ]; then
            echo "Found path-related settings in $file:"
            echo "$paths"
            echo
        fi
    done
}

# Function to check for platform-specific scripts
check_platform_scripts() {
    print_section "Checking Platform-Specific Scripts"

    # Check for shell scripts with platform assumptions
    local shell_scripts=$(find . -name "*.sh" -not -path "./.git/*" -not -path "./node_modules/*")

    for file in $shell_scripts; do
        echo "Checking $file..."

        # Check for hardcoded paths
        local hardcoded=$(grep -E "(AppHost|Backend|Frontend)" "$file" | grep -v "#")

        if [ -n "$hardcoded" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded"
            echo
        fi

        # Check for platform-specific commands
        local platform_cmds=$(grep -E "(sudo|apt-get|yum|brew|choco|winget)" "$file")

        if [ -n "$platform_cmds" ]; then
            echo "Found platform-specific commands in $file:"
            echo "$platform_cmds"
            echo
        fi
    done

    # Check for PowerShell scripts
    local ps_scripts=$(find . -name "*.ps1" -not -path "./.git/*")

    for file in $ps_scripts; do
        echo "Checking $file..."

        local hardcoded=$(grep -E "(AppHost|Backend|Frontend)" "$file" | grep -v "#")

        if [ -n "$hardcoded" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded"
            echo
        fi
    done
}

# Function to check for cloud platform configurations
check_cloud_configs() {
    print_section "Checking Cloud Platform Configurations"

    # Check for AWS configurations
    local aws_configs=$(grep -r -i -E "(aws|amazon|s3|ec2|lambda)" --include="*.yml" --include="*.yaml" --include="*.json" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null)

    if [ -n "$aws_configs" ]; then
        echo "Found AWS configurations:"
        echo "$aws_configs" | head -5
        echo
    fi

    # Check for Azure configurations
    local azure_configs=$(grep -r -i -E "(azure|microsoft)" --include="*.yml" --include="*.yaml" --include="*.json" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | grep -v "package")

    if [ -n "$azure_configs" ]; then
        echo "Found Azure configurations:"
        echo "$azure_configs" | head -5
        echo
    fi

    # Check for GCP configurations
    local gcp_configs=$(grep -r -i -E "(google|gcp|cloud)" --include="*.yml" --include="*.yaml" --include="*.json" --include="*.config" --exclude-dir=".git" --exclude-dir="node_modules" . 2>/dev/null | grep -v "package")

    if [ -n "$gcp_configs" ]; then
        echo "Found GCP configurations:"
        echo "$gcp_configs" | head -5
        echo
    fi
}

# Function to check for infrastructure as code
check_infrastructure_code() {
    print_section "Checking Infrastructure as Code"

    # Check for Terraform files
    local tf_files=$(find . -name "*.tf" -o -name "*.tfvars" 2>/dev/null)

    for file in $tf_files; do
        echo "Checking $file..."

        local hardcoded=$(grep -E "(AppHost|Backend|Frontend)" "$file")

        if [ -n "$hardcoded" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded"
            echo
        fi
    done

    # Check for CloudFormation templates
    local cf_files=$(find . -name "*.template" -o -name "*.json" | xargs grep -l "AWSTemplateFormatVersion" 2>/dev/null)

    for file in $cf_files; do
        echo "Checking $file..."

        local hardcoded=$(grep -E "(AppHost|Backend|Frontend)" "$file")

        if [ -n "$hardcoded" ]; then
            print_warning "Found hardcoded paths in $file:"
            echo "$hardcoded"
            echo
        fi
    done
}

# Function to generate platform impact report
generate_platform_report() {
    print_section "Platform Impact Summary"

    echo "Key platform considerations for refactoring:"
    echo
    echo "1. **Docker**: Update Dockerfiles and docker-compose files with new paths"
    echo "2. **Kubernetes**: Update K8s manifests, ConfigMaps, and volumes"
    echo "3. **CI/CD**: Update GitHub Actions and other CI/CD pipelines"
    echo "4. **Environment**: Update environment-specific configuration files"
    echo "5. **Scripts**: Update shell and PowerShell scripts"
    echo "6. **Cloud**: Update cloud platform configurations (AWS, Azure, GCP)"
    echo "7. **Infrastructure**: Update Terraform and CloudFormation templates"
    echo
    print_warning "Test deployments on all platforms after refactoring!"
    echo
    echo "Recommended platform validation steps:"
    echo "- Test Docker builds with new structure"
    echo "- Deploy to Kubernetes with updated manifests"
    echo "- Run CI/CD pipelines with new paths"
    echo "- Test cloud deployments"
    echo "- Validate infrastructure provisioning"
}

main() {
    print_header
    echo "Auditing platform-specific configurations..."
    echo

    check_docker_configs
    check_kubernetes_configs
    check_ci_cd_configs
    check_environment_configs
    check_platform_scripts
    check_cloud_configs
    check_infrastructure_code

    generate_platform_report
}

main "$@"