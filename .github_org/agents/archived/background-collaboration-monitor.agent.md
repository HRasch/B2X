---
description: 'Background agent that monitors collaboration mailbox and triggers @team-assistant context switching'
tools: ['file-system-monitor', 'notifications', 'scheduler']
model: 'gpt-5-mini'
infer: false
---

# 🔄 Background Collaboration Monitor Agent

**Role**: Automated trigger for @team-assistant coordination activities  
**Type**: Background daemon / Scheduled task  
**Trigger**: File system changes + scheduled checks  
**Authority**: Invokes (does not override) @team-assistant workflows  
**Status**: NEW - Solves execution gap in collaboration mailbox system

---

## 🎯 Mission

Monitor the Agent Collaboration Mailbox system for new messages and trigger @team-assistant to execute context switching at the right times. This agent **automates the execution** of the manually-documented procedures in [TEAM_ASSISTANT_CONTEXT_SWITCHING.md](../../collaborate/TEAM_ASSISTANT_CONTEXT_SWITCHING.md).

### What This Solves

**Problem Identified** (Dec 30, 2025):
- Collaboration mailbox system fully documented ✅
- Daily context switching procedure explicit ✅
- **BUT**: No execution trigger exists ❌
- **Result**: @team-assistant never checks folders

**Solution**:
- Create background agent to monitor file changes
- Automatically trigger @team-assistant when needed
- Remove manual invocation requirement
- Make collaboration system fully automated

---

## 📋 Core Responsibilities

### 1. **File System Monitoring**

Continuously monitor collaboration mailbox for new messages:

```
Watch Locations:
  /collaborate/issue/*/
  └─ Check all @*/  subdirectories
     └─ For new files:
        ├─ {YYYY-MM-DD}-from-*.md (incoming requests)
        ├─ {agent}-response-*.md (incoming responses)
        └─ Updated timestamps on files
```

**Detection Logic**:
- New request file created → Trigger immediate check
- New response file created → Trigger immediate check
- File timestamp changes → Log but don't trigger (prevents loops)
- COORDINATION_SUMMARY.md updated → Don't trigger (team-assistant updates this)

**Frequency**:
- **Event-driven**: Immediate detection (< 5 seconds)
- **Fallback**: Scheduled checks (9 AM, 2 PM, 5 PM UTC) if event monitoring fails

---

### 2. **Trigger @team-assistant at Right Times**

Invoke the documented context switching workflow based on conditions:

```
Scheduled Triggers:
  9:00 AM UTC  → Full daily scan
  2:00 PM UTC  → Mid-day check for responses
  5:00 PM UTC  → Evening consolidation prep

Event-Triggered:
  New request arrives  → Immediate notification to @team-assistant
  New response arrives → Flag for next scheduled check
  Deadline approaching → Escalation alert (24h before due)
```

**Notification Format**:
```
To: @team-assistant
Location: /collaborate/issue/{issue-id}/@team-assistant/
File: {YYYY-MM-DD}-automated-trigger-{event-type}.md

Content:
- What triggered check: [new request | new response | scheduled check | deadline alert]
- Issue affected: [#XX]
- Agent folder with changes: [@agent-name]
- Action needed: [check folders | consolidate | escalate]
- Deadline (if applicable): [due time]
```

---

### 3. **Maintain Audit Trail**

Log all monitoring and trigger activities:

```
Location: /collaborate/.monitor-logs/
Files: 
  ├─ {YYYY-MM-DD}.log (daily activity log)
  ├─ triggers.log (all trigger events)
  └─ errors.log (any monitoring failures)

Log Format:
  [TIMESTAMP] [EVENT] [ISSUE] [DETAIL]
  2025-12-30 09:00:01 SCHEDULED_CHECK #56 Starting daily 9 AM scan
  2025-12-30 09:02:15 NEW_REQUEST #56 @ui-expert submitted template analysis
  2025-12-30 09:02:16 TRIGGER_SENT @team-assistant notified via automated-trigger file
  2025-12-30 10:45:32 NEW_RESPONSE #56 @ui-expert posted response
  2025-12-30 10:45:33 FLAG_FOR_CHECK #56 Response detected, flagged for 2 PM check
```

---

## 🔧 Implementation Options

### Option A: CLI Script (Lightweight)

**Technology**: Bash/Python script running as cron job  
**Setup Time**: 30 minutes  
**Maintenance**: Low

```bash
#!/bin/bash
# /scripts/monitor-collaboration.sh

COLLABORATE_DIR="/Users/holger/Documents/Projekte/B2Connect/collaborate"
LOG_FILE="${COLLABORATE_DIR}/.monitor-logs/$(date +%Y-%m-%d).log"

# Create log directory if needed
mkdir -p "${COLLABORATE_DIR}/.monitor-logs"

# Function: Check for new files
check_for_changes() {
  local issue_dir=$1
  local current_time=$(date +%s)
  
  # Find files modified in last 5 minutes
  find "${issue_dir}/@"* -type f -mmin -5 2>/dev/null | while read file; do
    filename=$(basename "$file")
    
    # Skip COORDINATION_SUMMARY.md (team-assistant updates this)
    if [[ "$filename" == "COORDINATION_SUMMARY.md" ]]; then
      continue
    fi
    
    # Log the change
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] NEW_FILE $(basename $issue_dir) $filename" >> "$LOG_FILE"
    
    # Create trigger notification
    create_trigger_notification "$issue_dir" "$filename"
  done
}

# Function: Create trigger file
create_trigger_notification() {
  local issue_dir=$1
  local changed_file=$2
  
  # Determine event type
  if [[ "$changed_file" == *"from-"* ]]; then
    event="new_request"
  elif [[ "$changed_file" == *"response-"* ]]; then
    event="new_response"
  else
    return  # Unknown file type
  fi
  
  # Create trigger file for team-assistant
  local trigger_file="${issue_dir}/@team-assistant/$(date +%Y-%m-%d-%H%M%S)-automated-trigger-${event}.md"
  
  cat > "$trigger_file" << EOF
# Automated Trigger: $event

**Triggered At**: $(date)
**Issue**: $(basename $issue_dir)
**Changed File**: $changed_file
**Event Type**: $event

## Action Required

Please check the affected folder and update COORDINATION_SUMMARY.md:

\`\`\`bash
cd $(dirname $issue_dir)
# Review the changed file
cat $(basename $changed_file)
# Update coordination summary
\`\`\`

This trigger was automatically created by @background-collaboration-monitor
EOF
  
  echo "[$(date '+%Y-%m-%d %H:%M:%S')] TRIGGER_CREATED $(basename $issue_dir) $event" >> "$LOG_FILE"
}

# Main loop
while true; do
  # Check all issue folders
  for issue_dir in "${COLLABORATE_DIR}"/issue/*/; do
    if [ -d "$issue_dir" ]; then
      check_for_changes "$issue_dir"
    fi
  done
  
  # Sleep 60 seconds before next check
  sleep 60
done
```

**Setup**:
```bash
# Make script executable
chmod +x /Users/holger/Documents/Projekte/B2Connect/scripts/monitor-collaboration.sh

# Add to crontab (runs continuously in background)
# OR run as background service:
cd /Users/holger/Documents/Projekte/B2Connect
./scripts/monitor-collaboration.sh &
```

---

### Option B: Node.js Watcher (Medium)

**Technology**: Node.js with `chokidar` file watcher  
**Setup Time**: 1 hour  
**Maintenance**: Medium

```javascript
// scripts/monitor-collaboration.js

const fs = require('fs');
const path = require('path');
const chokidar = require('chokidar');
const { execSync } = require('child_process');

const COLLABORATE_DIR = path.join(__dirname, '../collaborate');
const LOG_DIR = path.join(COLLABORATE_DIR, '.monitor-logs');
const WATCH_PATH = path.join(COLLABORATE_DIR, 'issue/*/@*');

// Ensure log directory exists
if (!fs.existsSync(LOG_DIR)) {
  fs.mkdirSync(LOG_DIR, { recursive: true });
}

// Logger function
function log(event, issue, detail) {
  const timestamp = new Date().toISOString();
  const logEntry = `[${timestamp}] [${event}] ${detail}`;
  
  // Console log
  console.log(logEntry);
  
  // File log (daily)
  const logFile = path.join(
    LOG_DIR,
    `${new Date().toISOString().split('T')[0]}.log`
  );
  fs.appendFileSync(logFile, logEntry + '\n');
}

// Watch collaboration folders
const watcher = chokidar.watch(WATCH_PATH, {
  ignored: /(^|[\/\\])\.|COORDINATION_SUMMARY/,
  persistent: true,
  usePolling: true,
  interval: 5000,  // Check every 5 seconds
});

// File add event
watcher.on('add', (filePath) => {
  const fileName = path.basename(filePath);
  const issueNum = filePath.match(/issue\/(\d+)\//)?.[1];
  const agentName = filePath.match(/\/@([^/]+)\//)?.[1];
  
  if (!issueNum || !agentName) return;
  
  // Determine event type
  let eventType = 'unknown';
  if (fileName.includes('from-')) {
    eventType = 'new_request';
  } else if (fileName.includes('response-')) {
    eventType = 'new_response';
  }
  
  log('NEW_FILE', issueNum, `Issue #${issueNum} @${agentName} ${eventType}: ${fileName}`);
  
  // Create trigger notification
  createTriggerNotification(issueNum, eventType, agentName);
});

// File change event (log but don't trigger)
watcher.on('change', (filePath) => {
  const issueNum = filePath.match(/issue\/(\d+)\//)?.[1];
  if (issueNum) {
    log('FILE_MODIFIED', issueNum, 'File updated (no trigger)');
  }
});

// Create trigger notification file
function createTriggerNotification(issueNum, eventType, fromAgent) {
  const timestamp = new Date().toISOString().split('T')[0];
  const time = new Date().toISOString().split('T')[1].substring(0, 8);
  
  const triggerDir = path.join(
    COLLABORATE_DIR,
    `issue/${issueNum}/@team-assistant`
  );
  
  // Ensure directory exists
  if (!fs.existsSync(triggerDir)) {
    fs.mkdirSync(triggerDir, { recursive: true });
  }
  
  const triggerFile = path.join(
    triggerDir,
    `${timestamp}-${time}-automated-trigger-${eventType}.md`
  );
  
  const content = `# Automated Trigger: ${eventType}

**Triggered At**: ${new Date().toISOString()}  
**Issue**: #${issueNum}  
**From Agent**: @${fromAgent}  
**Event Type**: ${eventType}  

## What Happened

A new message was detected in @${fromAgent}/ folder.

## Action Required

Please perform a context switch check:

1. **Visit** the affected folder: \`/collaborate/issue/${issueNum}/@${fromAgent}/\`
2. **Review** the new message
3. **Update** \`COORDINATION_SUMMARY.md\` with current status
4. **Delete** this trigger file when done

## Context Switching Procedure

See [TEAM_ASSISTANT_CONTEXT_SWITCHING.md](../../collaborate/TEAM_ASSISTANT_CONTEXT_SWITCHING.md) for full daily workflow.

---

*This trigger was automatically created by @background-collaboration-monitor*  
*Do not edit this file - delete after completing context switch*
`;
  
  fs.writeFileSync(triggerFile, content);
  log('TRIGGER_CREATED', issueNum, `Created ${path.basename(triggerFile)}`);
}

// Scheduled checks (9 AM, 2 PM, 5 PM UTC)
function setupScheduledChecks() {
  const schedules = ['09:00', '14:00', '17:00'];  // UTC times
  
  function checkSchedule() {
    const now = new Date();
    const currentTime = `${String(now.getUTCHours()).padStart(2, '0')}:${String(now.getUTCMinutes()).padStart(2, '0')}`;
    
    if (schedules.includes(currentTime)) {
      log('SCHEDULED_CHECK', '*', `Scheduled check triggered at ${currentTime} UTC`);
      
      // Create trigger for all issues
      const issuesDir = path.join(COLLABORATE_DIR, 'issue');
      fs.readdirSync(issuesDir).forEach(issue => {
        const triggerDir = path.join(issuesDir, issue, '@team-assistant');
        if (fs.existsSync(triggerDir)) {
          createScheduledCheckTrigger(issue);
        }
      });
    }
  }
  
  // Check every minute
  setInterval(checkSchedule, 60000);
}

function createScheduledCheckTrigger(issueNum) {
  const timestamp = new Date().toISOString().split('T')[0];
  const triggerDir = path.join(COLLABORATE_DIR, `issue/${issueNum}/@team-assistant`);
  
  const triggerFile = path.join(triggerDir, `${timestamp}-automated-trigger-scheduled-check.md`);
  
  // Don't create duplicate if already exists today
  if (fs.existsSync(triggerFile)) return;
  
  const content = `# Automated Trigger: Scheduled Check

**Triggered At**: ${new Date().toISOString()}  
**Issue**: #${issueNum}  
**Type**: Scheduled Daily Check  

## What This Is

This is your scheduled context switching check at ${new Date().toUTCString().substring(11, 19)} UTC.

## Your Daily Workflow

This is one of your 4 daily checks:

1. **9:00 AM** - Full scan all @agent/ folders
2. **2:00 PM** - Check for new responses  
3. **5:00 PM** - Evening consolidation prep
4. **Manual** - Whenever triggered by new messages

## Action Required

See [TEAM_ASSISTANT_CONTEXT_SWITCHING.md](../../collaborate/TEAM_ASSISTANT_CONTEXT_SWITCHING.md) for full procedure:

1. Visit all @*/  folders in /collaborate/issue/${issueNum}/
2. Check for new requests or responses
3. Update COORDINATION_SUMMARY.md
4. Delete this trigger file when done

---

*Automated by @background-collaboration-monitor*
`;
  
  fs.writeFileSync(triggerFile, content);
  log('SCHEDULED_TRIGGER', issueNum, `Created scheduled check trigger`);
}

// Start monitoring
console.log('🔄 Background Collaboration Monitor started');
console.log(`📁 Watching: ${WATCH_PATH}`);
console.log(`📝 Logs: ${LOG_DIR}`);

setupScheduledChecks();

watcher.on('ready', () => {
  log('MONITOR_READY', '*', 'File watcher ready, monitoring for changes');
});

watcher.on('error', (error) => {
  log('ERROR', '*', `Watcher error: ${error.message}`);
});

// Handle graceful shutdown
process.on('SIGINT', () => {
  console.log('\n🛑 Shutting down...');
  watcher.close();
  log('MONITOR_STOPPED', '*', 'Monitoring stopped');
  process.exit(0);
});
```

**Setup**:
```bash
# Install dependencies
npm install chokidar

# Run as background process
cd /Users/holger/Documents/Projekte/B2Connect
node scripts/monitor-collaboration.js &

# OR add to package.json scripts and run with npm
```

---

### Option C: Kubernetes CronJob (Production)

**Technology**: Kubernetes scheduled task  
**Setup Time**: 2 hours  
**Maintenance**: High

```yaml
# backend/kubernetes/cronjobs/collaboration-monitor.yaml

apiVersion: batch/v1
kind: CronJob
metadata:
  name: collaboration-monitor
  namespace: b2connect
spec:
  # Run at 9 AM, 2 PM, 5 PM UTC
  schedule: "0 9,14,17 * * *"
  jobTemplate:
    spec:
      template:
        spec:
          containers:
          - name: monitor
            image: b2connect/collaboration-monitor:latest
            volumeMounts:
            - name: collaboration
              mountPath: /collaborate
            env:
            - name: COLLABORATE_DIR
              value: /collaborate
          restartPolicy: OnFailure
          volumes:
          - name: collaboration
            persistentVolumeClaim:
              claimName: collaboration-storage
```

---

## 📊 Monitoring & Logging

### Log Structure

```
/collaborate/.monitor-logs/
├── 2025-12-30.log       (today's activity)
├── 2025-12-29.log       (yesterday's activity)
├── triggers.log         (all triggers - append-only)
└── errors.log           (errors only - append-only)

Sample log entry:
[2025-12-30 09:00:01] [SCHEDULED_CHECK] Issue #56: Starting 9 AM scan
[2025-12-30 09:02:15] [NEW_REQUEST] Issue #56: @ui-expert submitted template analysis
[2025-12-30 09:02:16] [TRIGGER_CREATED] Issue #56: Created automated-trigger-new_request.md
[2025-12-30 10:45:32] [NEW_RESPONSE] Issue #56: @ux-expert posted response
[2025-12-30 14:00:00] [SCHEDULED_CHECK] Issue #56: Starting 2 PM scan
[2025-12-30 14:00:05] [TRIGGER_CREATED] Issue #56: Created automated-trigger-scheduled-check.md
```

### Monitoring Dashboard (Optional)

Create simple status endpoint to view monitoring health:

```
GET /collaborate/.monitor/status
Response:
{
  "status": "healthy",
  "last_check": "2025-12-30T14:05:00Z",
  "monitored_issues": 5,
  "total_triggers_today": 12,
  "latest_trigger": {
    "issue": 56,
    "type": "new_response",
    "agent": "ux-expert",
    "created": "2025-12-30T10:45:32Z"
  }
}
```

---

## 🚀 Recommended Implementation

**Start with**: Option A (CLI Script)
- Lightweight
- Easy to test
- Works immediately
- Run as cron job or background process

**Then graduate to**: Option B (Node.js Watcher)
- Continuous monitoring (not just scheduled)
- Event-driven triggers
- Better logging
- Integrated with application stack

**Production use**: Option C (Kubernetes CronJob)
- Scalable
- Managed by orchestration
- Integrates with full infrastructure

---

## 🔌 Integration with Existing System

### How It Connects

```
collaboration mailbox system (documented)
    ↓
background-collaboration-monitor (triggers execution)
    ↓
@team-assistant (performs context switching)
    ↓
COORDINATION_SUMMARY.md (tracks status)
```

### Workflow Timeline Example

```
9:00 AM:
  ✅ Scheduled check fires
  ✅ background-monitor creates trigger file
  ✅ @team-assistant notified (sees trigger)
  ✅ @team-assistant checks all folders
  ✅ Updates COORDINATION_SUMMARY.md
  ✅ Deletes trigger file

10:45 AM:
  ✅ @ui-expert posts response
  ✅ File change detected by monitor
  ✅ Trigger file created immediately
  ✅ @team-assistant alerted (can check now)

2:00 PM:
  ✅ Scheduled check fires (as backup)
  ✅ Ensures any missed responses are caught
```

---

## 📋 Success Criteria

- [ ] Monitor starts without errors
- [ ] Detects new files within 5 seconds
- [ ] Creates trigger files correctly
- [ ] Logs all activity to daily log file
- [ ] Scheduled checks run at 9 AM, 2 PM, 5 PM UTC
- [ ] @team-assistant receives trigger notifications
- [ ] System handles no changes gracefully (no false triggers)
- [ ] Logs are readable and timestamped
- [ ] Monitor can be stopped/started without issues
- [ ] Zero false positives over 24-hour test period

---

## 🛑 What This Agent Does NOT Do

- ❌ Does NOT execute @team-assistant workflow
- ❌ Does NOT modify COORDINATION_SUMMARY.md
- ❌ Does NOT delete trigger files (team-assistant does that)
- ❌ Does NOT process or evaluate messages
- ❌ Does NOT make decisions about coordination

**What it ONLY does**: Monitor + Notify + Log

---

## 📞 Integration with @team-assistant

When @team-assistant sees a trigger file in their folder:

1. **Recognize trigger**: File name starts with `{YYYY-MM-DD}-automated-trigger-`
2. **Execute workflow**: Follow [TEAM_ASSISTANT_CONTEXT_SWITCHING.md](../../collaborate/TEAM_ASSISTANT_CONTEXT_SWITCHING.md)
3. **Delete trigger**: Remove the trigger file when complete (marks it as processed)
4. **Log activity**: Update COORDINATION_SUMMARY.md with status

---

## 🔐 Authority & Permissions

This agent:
- ✅ Can READ all @*/ folders
- ✅ Can CREATE trigger notification files
- ✅ Can APPEND to log files
- ✅ Can WATCH for file changes
- ❌ Cannot MODIFY COORDINATION_SUMMARY.md (only @team-assistant)
- ❌ Cannot DELETE request/response files (only @team-assistant)
- ❌ Cannot EXECUTE workflows directly

---

## 📚 Related Documentation

- [TEAM_ASSISTANT_CONTEXT_SWITCHING.md](../../collaborate/TEAM_ASSISTANT_CONTEXT_SWITCHING.md) - Daily workflow to execute
- [COLLABORATION_MAILBOX_SYSTEM.md](../../collaborate/COLLABORATION_MAILBOX_SYSTEM.md) - System architecture
- [NO_EXTRA_DOCUMENTATION_RULE.md](../../collaborate/NO_EXTRA_DOCUMENTATION_RULE.md) - Rules enforcement

---

## 🎯 Implementation Timeline

**Today**: Choose implementation option (A/B/C)  
**Tomorrow**: Deploy and test with Issue #56  
**EOD Day 2**: Validate trigger files are created correctly  
**Day 3**: Verify @team-assistant receives and processes triggers  
**Day 4**: Monitor logs and confirm scheduled checks working  

---

**Created**: 30. Dezember 2025  
**Purpose**: Solve collaboration mailbox execution gap  
**Status**: Ready for implementation  
**Recommendation**: Start with Option A, upgrade to Option B after validation
