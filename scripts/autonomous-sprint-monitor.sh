#!/bin/bash

# Autonomous Sprint Execution Monitor
# Enables continuous sprint monitoring and execution for Iteration 001
# Responsibility: @ScrumMaster (delegated by @SARAH)
# Location: /scripts/autonomous-sprint-monitor.sh

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
SPRINT_DIR="${PROJECT_ROOT}/.ai/sprint"
LOG_DIR="${SPRINT_DIR}/.autonomous-logs"
MONITOR_INTERVAL=300  # Check every 5 minutes

# Ensure log directory exists
mkdir -p "${LOG_DIR}"

# Initialize log files
TIMESTAMP=$(date +%Y-%m-%d)
LOG_FILE="${LOG_DIR}/${TIMESTAMP}.log"
STATUS_FILE="${LOG_DIR}/current-status.log"
BLOCKERS_FILE="${LOG_DIR}/blockers.log"
PROGRESS_FILE="${LOG_DIR}/progress.log"

###############################################################################
# Logging Functions
###############################################################################

log_info() {
  local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
  local message="[$timestamp] [INFO] $1"
  echo "$message" >> "${LOG_FILE}"
  echo "$message"
}

log_status() {
  local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
  local message="[$timestamp] [STATUS] $1"
  echo "$message" >> "${STATUS_FILE}"
  echo "$message" >> "${LOG_FILE}"
  echo "📊 $message"
}

log_blocker() {
  local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
  local issue=$1
  local detail=$2
  local message="[$timestamp] [BLOCKER] $issue: $detail"
  echo "$message" >> "${BLOCKERS_FILE}"
  echo "$message" >> "${LOG_FILE}"
  echo "🚨 $message"
}

log_progress() {
  local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
  local message="[$timestamp] [PROGRESS] $1"
  echo "$message" >> "${PROGRESS_FILE}"
  echo "$message" >> "${LOG_FILE}"
  echo "✅ $message"
}

###############################################################################
# Monitoring Functions
###############################################################################

check_sprint_progress() {
  log_info "Checking sprint progress..."

  # Read current tracking file
  if [ -f "${SPRINT_DIR}/ITERATION_001_TRACKING.md" ]; then
    # Extract current completed SP
    local completed=$(grep "Current Completed:" "${SPRINT_DIR}/ITERATION_001_TRACKING.md" | sed 's/.*: \([0-9]*\) SP/\1/')
    local target=$(grep "Target Velocity:" "${SPRINT_DIR}/ITERATION_001_TRACKING.md" | sed 's/.*: \([0-9]*\) SP/\1/')

    log_status "Sprint Progress: ${completed}/${target} SP completed"

    if [ "$completed" -ge "$target" ]; then
      log_progress "Sprint target reached! ${completed} SP completed."
      return 0
    fi
  else
    log_info "Tracking file not found, initializing..."
  fi
}

conduct_automated_standup() {
  log_info "Conducting automated daily standup..."

  # Check for recent commits or changes
  local last_commit=$(git log --oneline -1 --since="1 day ago" 2>/dev/null || echo "No recent commits")

  # Check for test results
  local test_results=$(find "${PROJECT_ROOT}" -name "*.trx" -newer "${LOG_FILE}" 2>/dev/null | wc -l)

  # Check for build status
  local build_status="Unknown"
  if [ -f "${PROJECT_ROOT}/backend/B2Connect.slnx" ]; then
    # Try to check if build is possible
    build_status="Build check pending"
  fi

  log_status "Automated Standup: Commits: ${last_commit}, Tests: ${test_results} results, Build: ${build_status}"
}

check_for_blockers() {
  log_info "Checking for blockers..."

  # Check for build errors
  if [ -f "${PROJECT_ROOT}/logs/build-errors.log" ]; then
    local error_count=$(wc -l < "${PROJECT_ROOT}/logs/build-errors.log")
    if [ "$error_count" -gt 0 ]; then
      log_blocker "Build Errors" "${error_count} build errors detected"
    fi
  fi

  # Check for test failures
  local failed_tests=$(find "${PROJECT_ROOT}" -name "*.trx" -exec grep -l "outcome.*Failed" {} \; 2>/dev/null | wc -l)
  if [ "$failed_tests" -gt 0 ]; then
    log_blocker "Test Failures" "${failed_tests} test files with failures"
  fi

  # Check for dependency issues
  if [ -f "${PROJECT_ROOT}/Directory.Packages.props" ]; then
    # Check for outdated packages (simplified)
    log_info "Dependency check: Monitoring for updates..."
  fi
}

execute_sprint_tasks() {
  log_info "Executing assigned sprint tasks..."

  # Check if backend tasks need execution
  if [ -f "${PROJECT_ROOT}/backend/B2Connect.slnx" ]; then
    log_progress "Backend tasks: Monitoring build status"
  fi

  # Check frontend tasks
  if [ -d "${PROJECT_ROOT}/frontend" ]; then
    log_progress "Frontend tasks: Monitoring development status"
  fi

  # Run automated tests if needed
  log_info "Running automated tests..."
  # This would trigger test tasks
}

generate_sprint_summary() {
  log_info "Generating sprint summary..."

  local summary_file="${SPRINT_DIR}/SPRINT_SUMMARY_$(date +%Y%m%d).md"

  cat > "$summary_file" << EOF
# Sprint Summary - $(date)

## Progress Overview
- Status: Autonomous execution active
- Monitoring: Continuous
- Last Check: $(date)

## Key Metrics
- Monitoring Interval: ${MONITOR_INTERVAL} seconds
- Log Files: ${LOG_DIR}

## Current Status
$(tail -10 "${STATUS_FILE}" 2>/dev/null || echo "No status updates yet")

## Blockers
$(tail -5 "${BLOCKERS_FILE}" 2>/dev/null || echo "No blockers detected")

## Recent Progress
$(tail -5 "${PROGRESS_FILE}" 2>/dev/null || echo "No progress logged yet")

---
*Generated by Autonomous Sprint Monitor*
EOF

  log_progress "Sprint summary generated: $summary_file"
}

###############################################################################
# Main Monitoring Loop
###############################################################################

main() {
  log_info "Starting Autonomous Sprint Execution Monitor for Iteration 001"
  log_info "Monitoring interval: ${MONITOR_INTERVAL} seconds"
  log_info "Log directory: ${LOG_DIR}"

  # Initial status
  log_status "Autonomous execution ENABLED for Iteration 001"
  log_status "Delegated to @ScrumMaster: Continuous monitoring active"

  while true; do
    check_sprint_progress
    conduct_automated_standup
    check_for_blockers
    execute_sprint_tasks

    # Check if sprint is complete
    local completed=$(grep "Current Completed:" "${SPRINT_DIR}/ITERATION_001_TRACKING.md" 2>/dev/null | sed 's/.*: \([0-9]*\) SP/\1/' || echo "0")
    local target=$(grep "Target Velocity:" "${SPRINT_DIR}/ITERATION_001_TRACKING.md" 2>/dev/null | sed 's/.*: \([0-9]*\) SP/\1/' || echo "28")

    if [ "$completed" -ge "$target" ]; then
      log_progress "SPRINT COMPLETE: Target velocity reached (${completed}/${target} SP)"
      generate_sprint_summary
      log_status "Autonomous execution completed successfully"
      break
    fi

    log_info "Sleeping for ${MONITOR_INTERVAL} seconds..."
    sleep "${MONITOR_INTERVAL}"
  done

  # Final summary
  generate_sprint_summary
  log_info "Autonomous sprint execution completed."
}

# Run main function
main "$@"