#!/bin/bash

# Background Collaboration Monitor
# Monitors Agent Collaboration Mailbox for new messages
# Triggers @team-assistant context switching when needed
# Location: /scripts/monitor-collaboration.sh

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
COLLABORATE_DIR="${PROJECT_ROOT}/collaborate"
LOG_DIR="${COLLABORATE_DIR}/.monitor-logs"
WATCH_INTERVAL=5  # Check every 5 seconds

# Ensure log directory exists
mkdir -p "${LOG_DIR}"

# Initialize or rotate log files
TIMESTAMP=$(date +%Y-%m-%d)
LOG_FILE="${LOG_DIR}/${TIMESTAMP}.log"
TRIGGERS_LOG="${LOG_DIR}/triggers.log"
ERRORS_LOG="${LOG_DIR}/errors.log"

# Track files we've already processed (prevent duplicate triggers)
PROCESSED_FILE="${LOG_DIR}/.processed-files"
touch "${PROCESSED_FILE}"

###############################################################################
# Logging Functions
###############################################################################

log_info() {
  local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
  local message="[$timestamp] [INFO] $1"
  echo "$message" >> "${LOG_FILE}"
  echo "$message"
}

log_trigger() {
  local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
  local issue=$1
  local event=$2
  local detail=$3
  
  local message="[$timestamp] [TRIGGER] Issue #${issue} ${event}: ${detail}"
  echo "$message" >> "${TRIGGERS_LOG}"
  echo "$message" >> "${LOG_FILE}"
  echo "🔔 $message"
}

log_error() {
  local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
  local message="[$timestamp] [ERROR] $1"
  echo "$message" >> "${ERRORS_LOG}"
  echo "$message" >> "${LOG_FILE}"
  echo "❌ $message"
}

###############################################################################
# File Monitoring Functions
###############################################################################

get_file_hash() {
  local file=$1
  # Create a hash of file path and modification time
  echo "${file}:$(stat -f%m "$file" 2>/dev/null || stat -c%Y "$file" 2>/dev/null)"
}

is_processed() {
  local file=$1
  local file_hash=$(get_file_hash "$file")
  grep -q "^${file_hash}$" "${PROCESSED_FILE}" 2>/dev/null
}

mark_processed() {
  local file=$1
  local file_hash=$(get_file_hash "$file")
  echo "${file_hash}" >> "${PROCESSED_FILE}"
}

should_trigger() {
  local filename=$1
  
  # Skip COORDINATION_SUMMARY.md (team-assistant updates this)
  if [[ "$filename" == "COORDINATION_SUMMARY.md" ]]; then
    return 1
  fi
  
  # Skip .DS_Store and other system files
  if [[ "$filename" == .DS_Store ]] || [[ "$filename" == .* ]]; then
    return 1
  fi
  
  # Skip log files
  if [[ "$filename" == *.log ]]; then
    return 1
  fi
  
  # Only trigger on request and response files
  if [[ "$filename" =~ from-.* ]] || [[ "$filename" =~ response-.* ]]; then
    return 0
  fi
  
  return 1
}

###############################################################################
# Trigger Creation Functions
###############################################################################

create_trigger_notification() {
  local issue_dir=$1
  local changed_file=$2
  local event_type=$3
  
  local issue_num=$(basename "$issue_dir")
  local timestamp=$(date '+%Y-%m-%d-%H%M%S')
  local trigger_file="${issue_dir}/@team-assistant/${timestamp}-automated-trigger-${event_type}.md"
  
  # Ensure @team-assistant directory exists
  mkdir -p "${issue_dir}/@team-assistant"
  
  # Create trigger content
  cat > "${trigger_file}" << 'TRIGGER_EOF'
# ⚡ Automated Trigger: New Message Detected

**Triggered At**: {TRIGGERED_AT}
**Issue**: #{ISSUE}
**Event Type**: {EVENT_TYPE}
**Changed File**: {CHANGED_FILE}

## What Happened

A new message was automatically detected in your collaboration mailbox.

## Action Required

Please perform a context switch check:

### Step 1: Review the Changed Folder

The file `{CHANGED_FILE}` was added. Navigate to the affected agent folder:

```bash
cd {ISSUE_PATH}
ls -la @*/
```

### Step 2: Update Coordination Status

Open and update: `COORDINATION_SUMMARY.md`

Add the new message status to the appropriate section.

### Step 3: Delete This Trigger

When done, delete this file to mark the trigger as processed:

```bash
rm "{TRIGGER_FILE}"
```

## Documentation

For full daily workflow, see:
- [TEAM_ASSISTANT_CONTEXT_SWITCHING.md](../../collaborate/TEAM_ASSISTANT_CONTEXT_SWITCHING.md)
- [COLLABORATION_MAILBOX_SYSTEM.md](../../collaborate/COLLABORATION_MAILBOX_SYSTEM.md)

---

*This trigger was automatically created by @background-collaboration-monitor at {TRIGGERED_AT}*
*Action: Delete this file when complete*

TRIGGER_EOF

  # Replace placeholders with actual values
  local now=$(date '+%Y-%m-%d %H:%M:%S UTC')
  sed -i '' "s|{TRIGGERED_AT}|${now}|g" "${trigger_file}"
  sed -i '' "s|{ISSUE}|${issue_num}|g" "${trigger_file}"
  sed -i '' "s|{EVENT_TYPE}|${event_type}|g" "${trigger_file}"
  sed -i '' "s|{CHANGED_FILE}|${changed_file}|g" "${trigger_file}"
  sed -i '' "s|{ISSUE_PATH}|${issue_dir}|g" "${trigger_file}"
  sed -i '' "s|{TRIGGER_FILE}|${trigger_file}|g" "${trigger_file}"
  
  mark_processed "${changed_file}"
  log_trigger "${issue_num}" "${event_type}" "${changed_file}"
  
  return 0
}

###############################################################################
# Monitoring Loop
###############################################################################

check_for_changes() {
  local issue_dir=$1
  local issue_num=$(basename "$issue_dir")
  
  # Check all @agent/ folders
  for agent_dir in "${issue_dir}"/@*/; do
    if [ ! -d "$agent_dir" ]; then
      continue
    fi
    
    # Look for new files
    for file in "$agent_dir"/*; do
      if [ ! -f "$file" ]; then
        continue
      fi
      
      local filename=$(basename "$file")
      
      # Skip if already processed
      if is_processed "$file"; then
        continue
      fi
      
      # Check if should trigger
      if should_trigger "$filename"; then
        # Determine event type
        local event_type="unknown"
        if [[ "$filename" =~ from-.* ]]; then
          event_type="new_request"
        elif [[ "$filename" =~ response-.* ]]; then
          event_type="new_response"
        fi
        
        create_trigger_notification "$issue_dir" "$filename" "$event_type"
      fi
    done
  done
}

###############################################################################
# Scheduled Checks (9 AM, 2 PM, 5 PM UTC)
###############################################################################

check_scheduled_triggers() {
  local current_time=$(date '+%H:%M')
  local scheduled_times=("09:00" "14:00" "17:00")
  
  for scheduled_time in "${scheduled_times[@]}"; do
    if [[ "$current_time" == "$scheduled_time" ]]; then
      log_info "🕐 Scheduled check triggered at ${current_time} UTC"
      
      # Create scheduled check triggers for all issues
      for issue_dir in "${COLLABORATE_DIR}"/issue/*/; do
        if [ -d "$issue_dir" ]; then
          local issue_num=$(basename "$issue_dir")
          
          # Check if @team-assistant folder exists
          if [ -d "${issue_dir}/@team-assistant" ]; then
            local timestamp=$(date '+%Y-%m-%d')
            local trigger_file="${issue_dir}/@team-assistant/${timestamp}-automated-trigger-scheduled-check.md"
            
            # Don't create duplicate if already exists today
            if [ ! -f "$trigger_file" ]; then
              cat > "${trigger_file}" << 'SCHEDULED_EOF'
# ⏰ Automated Trigger: Scheduled Daily Check

**Triggered At**: {TRIGGERED_AT}
**Issue**: #{ISSUE}
**Check Time**: {CHECK_TIME} UTC
**Type**: Scheduled Daily Check

## What This Is

This is your scheduled context switching check as part of your daily workflow:

- **9:00 AM** - Full scan all @agent/ folders
- **2:00 PM** - Check for new responses
- **5:00 PM** - Evening consolidation prep

## Action Required

1. **Visit all agent folders** in `/collaborate/issue/{ISSUE}/`
2. **Check for new requests or responses**
3. **Update COORDINATION_SUMMARY.md** with current status
4. **Delete this trigger** when done

## Reference

See [TEAM_ASSISTANT_CONTEXT_SWITCHING.md](../../collaborate/TEAM_ASSISTANT_CONTEXT_SWITCHING.md) for full procedure.

---

*Automated by @background-collaboration-monitor at {TRIGGERED_AT}*
*Action: Delete this file when complete*

SCHEDULED_EOF

              # Replace placeholders
              local now=$(date '+%Y-%m-%d %H:%M:%S')
              sed -i '' "s|{TRIGGERED_AT}|${now}|g" "${trigger_file}"
              sed -i '' "s|{ISSUE}|${issue_num}|g" "${trigger_file}"
              sed -i '' "s|{CHECK_TIME}|${current_time}|g" "${trigger_file}"
              
              log_trigger "$issue_num" "scheduled_check" "Daily check at ${current_time} UTC"
            fi
          fi
        fi
      done
    fi
  done
}

###############################################################################
# Main Loop
###############################################################################

main() {
  log_info "🔄 Background Collaboration Monitor starting..."
  log_info "📁 Watching: ${COLLABORATE_DIR}/issue/*/"
  log_info "📝 Logs: ${LOG_DIR}"
  log_info "🔄 Check interval: ${WATCH_INTERVAL} seconds"
  
  # Handle graceful shutdown
  trap "log_info '🛑 Monitor stopped'; exit 0" SIGINT SIGTERM
  
  # Main monitoring loop
  while true; do
    # Check for file changes
    for issue_dir in "${COLLABORATE_DIR}"/issue/*/; do
      if [ -d "$issue_dir" ]; then
        check_for_changes "$issue_dir"
      fi
    done
    
    # Check for scheduled triggers
    check_scheduled_triggers
    
    # Sleep before next check
    sleep "${WATCH_INTERVAL}"
  done
}

###############################################################################
# Entry Point
###############################################################################

if [ "$1" == "--help" ] || [ "$1" == "-h" ]; then
  cat << 'HELP_EOF'
Background Collaboration Monitor
Monitors Agent Collaboration Mailbox and triggers @team-assistant context switching

Usage:
  ./monitor-collaboration.sh          # Run in foreground (Ctrl+C to stop)
  ./monitor-collaboration.sh &        # Run in background
  
Logs:
  ${COLLABORATE_DIR}/.monitor-logs/YYYY-MM-DD.log      # Daily activity log
  ${COLLABORATE_DIR}/.monitor-logs/triggers.log         # All triggers
  ${COLLABORATE_DIR}/.monitor-logs/errors.log           # Errors only

Environment Variables:
  WATCH_INTERVAL   Check interval in seconds (default: 5)
  
Examples:
  # Start monitoring (foreground)
  ./monitor-collaboration.sh
  
  # Start monitoring (background)
  ./monitor-collaboration.sh &
  
  # Start with custom interval
  WATCH_INTERVAL=10 ./monitor-collaboration.sh &
  
  # View logs
  tail -f ${COLLABORATE_DIR}/.monitor-logs/$(date +%Y-%m-%d).log
  
  # Stop background process
  pkill -f "monitor-collaboration.sh"

HELP_EOF
  exit 0
fi

# Run main loop
main
