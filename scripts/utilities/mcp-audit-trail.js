#!/usr/bin/env node

/**
 * MCP Audit Trail System
 * Tracks all MCP server activities for compliance and debugging
 */

const fs = require('fs');
const path = require('path');

const AUDIT_DIR = path.join(process.cwd(), '.ai/logs/mcp-usage/audit-trail');
const AUDIT_INDEX = path.join(process.cwd(), '.ai/logs/mcp-usage/audit-index.json');

// Ensure directories exist
if (!fs.existsSync(AUDIT_DIR)) {
  fs.mkdirSync(AUDIT_DIR, { recursive: true });
}

/**
 * Log audit event
 */
function logAuditEvent(event) {
  try {
    const entry = {
      id: `${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
      timestamp: new Date().toISOString(),
      ...event
    };
    
    // Write to daily audit log
    const date = new Date().toISOString().split('T')[0];
    const auditFile = path.join(AUDIT_DIR, `audit-${date}.jsonl`);
    fs.appendFileSync(auditFile, JSON.stringify(entry) + '\n');
    
    // Update index
    updateAuditIndex(entry);
    
    return entry;
  } catch (err) {
    console.error(`[Audit] Error logging event: ${err.message}`);
  }
}

/**
 * Update audit index for fast lookups
 */
function updateAuditIndex(entry) {
  try {
    let index = {};
    if (fs.existsSync(AUDIT_INDEX)) {
      index = JSON.parse(fs.readFileSync(AUDIT_INDEX, 'utf8'));
    }
    
    // Index by server
    if (!index.byServer) index.byServer = {};
    if (!index.byServer[entry.server]) {
      index.byServer[entry.server] = [];
    }
    index.byServer[entry.server].push({
      id: entry.id,
      timestamp: entry.timestamp,
      type: entry.type
    });
    
    // Index by type
    if (!index.byType) index.byType = {};
    if (!index.byType[entry.type]) {
      index.byType[entry.type] = [];
    }
    index.byType[entry.type].push({
      id: entry.id,
      server: entry.server,
      timestamp: entry.timestamp
    });
    
    // Keep only last 1000 entries per category
    Object.keys(index.byServer).forEach(server => {
      if (index.byServer[server].length > 1000) {
        index.byServer[server] = index.byServer[server].slice(-1000);
      }
    });
    Object.keys(index.byType).forEach(type => {
      if (index.byType[type].length > 1000) {
        index.byType[type] = index.byType[type].slice(-1000);
      }
    });
    
    fs.writeFileSync(AUDIT_INDEX, JSON.stringify(index, null, 2));
  } catch (err) {
    console.error(`[Audit] Error updating index: ${err.message}`);
  }
}

/**
 * Query audit trail
 */
function queryAudit(filters = {}) {
  try {
    const results = [];
    const { server, type, startDate, endDate, limit = 100 } = filters;
    
    // Get list of audit files
    const files = fs.readdirSync(AUDIT_DIR)
      .filter(f => f.startsWith('audit-') && f.endsWith('.jsonl'))
      .sort()
      .reverse();
    
    let processed = 0;
    
    for (const file of files) {
      if (processed >= limit) break;
      
      const filePath = path.join(AUDIT_DIR, file);
      const lines = fs.readFileSync(filePath, 'utf8').split('\n').filter(l => l);
      
      for (const line of lines) {
        if (processed >= limit) break;
        
        const entry = JSON.parse(line);
        
        // Apply filters
        if (server && entry.server !== server) continue;
        if (type && entry.type !== type) continue;
        
        results.push(entry);
        processed++;
      }
    }
    
    return results;
  } catch (err) {
    console.error(`[Audit] Error querying audit trail: ${err.message}`);
    return [];
  }
}

/**
 * Generate audit report
 */
function generateAuditReport(period = 'daily') {
  try {
    const now = new Date();
    let startDate;
    
    switch (period) {
      case 'hourly':
        startDate = new Date(now.getTime() - 60 * 60 * 1000);
        break;
      case 'daily':
        startDate = new Date(now.getTime() - 24 * 60 * 60 * 1000);
        break;
      case 'weekly':
        startDate = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
        break;
      default:
        startDate = new Date(now.getTime() - 24 * 60 * 60 * 1000);
    }
    
    const events = queryAudit({ startDate, limit: 10000 });
    
    const report = {
      period,
      generatedAt: now.toISOString(),
      startDate: startDate.toISOString(),
      endDate: now.toISOString(),
      summary: {
        totalEvents: events.length,
        byType: {},
        byServer: {}
      },
      events: events.slice(0, 100) // Latest 100 events
    };
    
    // Aggregate statistics
    events.forEach(event => {
      report.summary.byType[event.type] = (report.summary.byType[event.type] || 0) + 1;
      report.summary.byServer[event.server] = (report.summary.byServer[event.server] || 0) + 1;
    });
    
    return report;
  } catch (err) {
    console.error(`[Audit] Error generating report: ${err.message}`);
  }
}

/**
 * Print audit report
 */
function printAuditReport(report) {
  console.log('\n╔════════════════════════════════════════════════════════════╗');
  console.log('║            MCP Audit Trail Report                          ║');
  console.log('╚════════════════════════════════════════════════════════════╝\n');
  
  console.log(`Period: ${report.period.toUpperCase()}`);
  console.log(`Generated: ${report.generatedAt}`);
  console.log(`Range: ${report.startDate} to ${report.endDate}\n`);
  
  console.log('📊 Summary');
  console.log(`  Total Events: ${report.summary.totalEvents}\n`);
  
  console.log('📋 Events by Type');
  Object.entries(report.summary.byType).forEach(([type, count]) => {
    console.log(`  ${type}: ${count}`);
  });
  console.log();
  
  console.log('🖥️ Events by Server');
  Object.entries(report.summary.byServer).forEach(([server, count]) => {
    console.log(`  ${server}: ${count}`);
  });
  console.log();
  
  console.log('🔍 Latest Events');
  report.events.slice(0, 10).forEach(event => {
    console.log(`  [${event.timestamp}] ${event.server}: ${event.type} - ${event.message || 'N/A'}`);
  });
  console.log();
}

// Predefined event types
const EVENT_TYPES = {
  SERVER_START: 'server_start',
  SERVER_STOP: 'server_stop',
  CACHE_HIT: 'cache_hit',
  CACHE_MISS: 'cache_miss',
  RATE_LIMIT_CHECK: 'rate_limit_check',
  RATE_LIMIT_EXCEEDED: 'rate_limit_exceeded',
  TOKEN_CONSUMED: 'token_consumed',
  ERROR: 'error',
  WARNING: 'warning'
};

// Export functions
module.exports = {
  logAuditEvent,
  queryAudit,
  generateAuditReport,
  printAuditReport,
  EVENT_TYPES
};

// CLI usage
if (require.main === module) {
  const command = process.argv[2];
  
  switch (command) {
    case 'report':
      const period = process.argv[3] || 'daily';
      const report = generateAuditReport(period);
      if (report) {
        printAuditReport(report);
      }
      break;
    
    case 'query':
      const server = process.argv[3];
      if (!server) {
        console.error('Usage: node mcp-audit-trail.js query <server-name>');
        process.exit(1);
      }
      const results = queryAudit({ server, limit: 50 });
      console.log(`\n🔍 Audit Events for ${server}:\n`);
      results.forEach(e => {
        console.log(`[${e.timestamp}] ${e.type}: ${e.message || 'N/A'}`);
      });
      break;
    
    default:
      console.log('Usage:');
      console.log('  node mcp-audit-trail.js report [hourly|daily|weekly]');
      console.log('  node mcp-audit-trail.js query <server-name>');
  }
}
