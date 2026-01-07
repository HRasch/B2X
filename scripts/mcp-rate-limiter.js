#!/usr/bin/env node

/**
 * MCP Rate Limiter
 * Prevents token bleeding by enforcing rate limits per MCP server
 * Tracks usage over time and blocks excessive calls
 */

const fs = require('fs');
const path = require('path');

const RATE_LIMITS_FILE = path.join(process.cwd(), '.ai/logs/mcp-usage/rate-limits.json');
const ALERT_FILE = path.join(process.cwd(), '.ai/logs/mcp-usage/rate-limit-alerts.log');

// Default rate limits (tokens per day)
const DEFAULT_LIMITS = {
  'typescript-mcp': 500,
  'vue-mcp': 400,
  'security-mcp': 300,
  'htmlcss-mcp': 200,
  'b2connect-mcp': 300,
  'database-mcp': 250,
  'documentation-mcp': 200,
  'performance-mcp': 150,
  'git-mcp': 100,
  'docker-mcp': 150,
  'roslyn-code-navigator': 600,
  'wolverine-mcp': 500,
  'playwright-mcp': 400
};

// Ensure directories exist
if (!fs.existsSync(path.dirname(RATE_LIMITS_FILE))) {
  fs.mkdirSync(path.dirname(RATE_LIMITS_FILE), { recursive: true });
}

/**
 * Load or initialize rate limit tracking
 */
function loadRateLimits() {
  try {
    if (fs.existsSync(RATE_LIMITS_FILE)) {
      const data = JSON.parse(fs.readFileSync(RATE_LIMITS_FILE, 'utf8'));
      // Reset daily limits if new day
      const lastReset = new Date(data._lastReset);
      const today = new Date().toDateString();
      if (lastReset.toDateString() !== today) {
        console.log('[Rate Limiter] Daily reset triggered');
        return initializeRateLimits();
      }
      return data;
    }
  } catch (err) {
    console.error('[Rate Limiter] Error loading limits:', err.message);
  }
  return initializeRateLimits();
}

/**
 * Initialize fresh rate limits for all servers
 */
function initializeRateLimits() {
  const limits = {};
  Object.entries(DEFAULT_LIMITS).forEach(([server, dailyLimit]) => {
    limits[server] = {
      dailyLimit,
      tokensUsedToday: 0,
      tokensUsedThisHour: 0,
      calls: [],
      lastReset: new Date().toISOString()
    };
  });
  limits._lastReset = new Date().toISOString();
  saveLimits(limits);
  return limits;
}

/**
 * Save rate limits to file
 */
function saveLimits(limits) {
  try {
    fs.writeFileSync(RATE_LIMITS_FILE, JSON.stringify(limits, null, 2));
  } catch (err) {
    console.error('[Rate Limiter] Error saving limits:', err.message);
  }
}

/**
 * Check if MCP server is within rate limits
 */
function checkRateLimit(serverName, tokensToUse = 1) {
  const limits = loadRateLimits();
  
  if (!limits[serverName]) {
    console.warn(`[Rate Limiter] Unknown server: ${serverName}`);
    return { allowed: true, reason: 'Unknown server, allowing' };
  }
  
  const serverLimit = limits[serverName];
  const totalAfter = serverLimit.tokensUsedToday + tokensToUse;
  
  if (totalAfter > serverLimit.dailyLimit) {
    const remaining = serverLimit.dailyLimit - serverLimit.tokensUsedToday;
    const message = `[Rate Limiter] BLOCKED: ${serverName} exceeded daily limit (${remaining} tokens remaining)`;
    console.error(message);
    logAlert(message);
    return {
      allowed: false,
      reason: 'Daily limit exceeded',
      remaining,
      limit: serverLimit.dailyLimit,
      used: serverLimit.tokensUsedToday
    };
  }
  
  // Warn if approaching limit (80%)
  if (totalAfter > serverLimit.dailyLimit * 0.8) {
    const message = `[Rate Limiter] WARNING: ${serverName} approaching daily limit (${totalAfter}/${serverLimit.dailyLimit} tokens)`;
    console.warn(message);
  }
  
  return {
    allowed: true,
    reason: 'Within limits',
    remaining: serverLimit.dailyLimit - totalAfter,
    limit: serverLimit.dailyLimit
  };
}

/**
 * Record token usage
 */
function recordUsage(serverName, tokensUsed, callDetails = {}) {
  const limits = loadRateLimits();
  
  if (!limits[serverName]) {
    limits[serverName] = {
      dailyLimit: DEFAULT_LIMITS[serverName] || 100,
      tokensUsedToday: 0,
      tokensUsedThisHour: 0,
      calls: []
    };
  }
  
  const now = new Date();
  const hourAgo = new Date(now.getTime() - 60 * 60 * 1000);
  
  // Record call
  limits[serverName].calls.push({
    timestamp: now.toISOString(),
    tokensUsed,
    ...callDetails
  });
  
  // Update counters
  limits[serverName].tokensUsedToday += tokensUsed;
  limits[serverName].tokensUsedThisHour = limits[serverName].calls
    .filter(c => new Date(c.timestamp) > hourAgo)
    .reduce((sum, c) => sum + c.tokensUsed, 0);
  
  // Keep only last 100 calls
  if (limits[serverName].calls.length > 100) {
    limits[serverName].calls = limits[serverName].calls.slice(-100);
  }
  
  saveLimits(limits);
  
  console.log(`[Rate Limiter] ${serverName}: +${tokensUsed} tokens (Day: ${limits[serverName].tokensUsedToday}/${limits[serverName].dailyLimit})`);
}

/**
 * Log rate limit alerts
 */
function logAlert(message) {
  try {
    const timestamp = new Date().toISOString();
    const alertEntry = `[${timestamp}] ${message}\n`;
    fs.appendFileSync(ALERT_FILE, alertEntry);
  } catch (err) {
    console.error('[Rate Limiter] Error logging alert:', err.message);
  }
}

/**
 * Get summary of all rate limits
 */
function getSummary() {
  const limits = loadRateLimits();
  const summary = {};
  
  Object.entries(limits).forEach(([server, data]) => {
    if (server === '_lastReset') return;
    
    const percentUsed = (data.tokensUsedToday / data.dailyLimit * 100).toFixed(1);
    summary[server] = {
      dailyLimit: data.dailyLimit,
      tokensUsedToday: data.tokensUsedToday,
      percentUsed: `${percentUsed}%`,
      remaining: data.dailyLimit - data.tokensUsedToday,
      callsToday: data.calls.length
    };
  });
  
  return summary;
}

/**
 * Print rate limit summary
 */
function printSummary() {
  console.log('\n=== MCP Rate Limit Summary ===');
  const summary = getSummary();
  Object.entries(summary).forEach(([server, data]) => {
    const status = data.percentUsed >= 80 ? '⚠️ ' : '✓ ';
    console.log(`${status}${server}: ${data.tokensUsedToday}/${data.dailyLimit} (${data.percentUsed})`);
  });
  console.log('==============================\n');
}

// Export functions
module.exports = {
  checkRateLimit,
  recordUsage,
  getSummary,
  printSummary,
  loadRateLimits,
  DEFAULT_LIMITS
};

// CLI usage
if (require.main === module) {
  const command = process.argv[2];
  const serverName = process.argv[3];
  const tokensUsed = parseInt(process.argv[4]) || 0;
  
  switch (command) {
    case 'check':
      if (!serverName) {
        console.error('Usage: node mcp-rate-limiter.js check <server-name> [tokens]');
        process.exit(1);
      }
      const result = checkRateLimit(serverName, tokensUsed);
      console.log(JSON.stringify(result, null, 2));
      process.exit(result.allowed ? 0 : 1);
      break;
    
    case 'record':
      if (!serverName || !tokensUsed) {
        console.error('Usage: node mcp-rate-limiter.js record <server-name> <tokens>');
        process.exit(1);
      }
      recordUsage(serverName, tokensUsed);
      break;
    
    case 'summary':
      printSummary();
      break;
    
    case 'reset':
      initializeRateLimits();
      console.log('[Rate Limiter] All limits reset');
      break;
    
    default:
      console.log('Usage:');
      console.log('  node mcp-rate-limiter.js check <server-name> [tokens]');
      console.log('  node mcp-rate-limiter.js record <server-name> <tokens>');
      console.log('  node mcp-rate-limiter.js summary');
      console.log('  node mcp-rate-limiter.js reset');
  }
}
