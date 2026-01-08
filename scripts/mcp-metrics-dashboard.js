#!/usr/bin/env node

/**
 * MCP Token Metrics Dashboard
 * Generates real-time dashboard of MCP token usage and efficiency metrics
 */

const fs = require('fs');
const path = require('path');
const cacheManager = require('./mcp-cache-manager');
const rateLimiter = require('./mcp-rate-limiter');

const METRICS_FILE = path.join(process.cwd(), '.ai/logs/mcp-usage/metrics.json');
const DASHBOARD_FILE = path.join(process.cwd(), '.ai/logs/mcp-usage/dashboard.html');

/**
 * Collect all metrics
 */
function collectMetrics() {
  const stats = cacheManager.getStats();
  const rateLimits = rateLimiter.loadRateLimits();
  
  const metrics = {
    timestamp: new Date().toISOString(),
    cache: {
      totalHits: 0,
      totalMisses: 0,
      hitRate: 0,
      totalTokensSaved: 0,
      servers: {}
    },
    rateLimits: {
      servers: {}
    },
    summary: {}
  };
  
  // Aggregate cache metrics
  Object.entries(stats).forEach(([server, data]) => {
    metrics.cache.totalHits += data.hits;
    metrics.cache.totalMisses += data.misses;
    metrics.cache.totalTokensSaved += data.hits * 10; // Estimated tokens per hit
    
    const hitRate = (data.hits + data.misses > 0)
      ? (data.hits / (data.hits + data.misses) * 100).toFixed(1)
      : 0;
    
    metrics.cache.servers[server] = {
      hits: data.hits,
      misses: data.misses,
      hitRate: `${hitRate}%`,
      totalTokens: data.totalTokens
    };
  });
  
  metrics.cache.hitRate = metrics.cache.totalHits + metrics.cache.totalMisses > 0
    ? (metrics.cache.totalHits / (metrics.cache.totalHits + metrics.cache.totalMisses) * 100).toFixed(1)
    : 0;
  
  // Aggregate rate limit metrics
  Object.entries(rateLimits).forEach(([server, data]) => {
    if (server === '_lastReset') return;
    
    const percentUsed = (data.tokensUsedToday / data.dailyLimit * 100).toFixed(1);
    metrics.rateLimits.servers[server] = {
      dailyLimit: data.dailyLimit,
      tokensUsedToday: data.tokensUsedToday,
      percentUsed: `${percentUsed}%`,
      remaining: data.dailyLimit - data.tokensUsedToday,
      calls: data.calls.length
    };
  });
  
  // Summary
  metrics.summary = {
    cacheEfficiency: `${metrics.cache.hitRate}% hit rate`,
    tokensSaved: `~${metrics.cache.totalTokensSaved} tokens`,
    activeServers: Object.keys(stats).length,
    rateLimitViolations: 0
  };
  
  return metrics;
}

/**
 * Save metrics to JSON file
 */
function saveMetrics(metrics) {
  try {
    fs.writeFileSync(METRICS_FILE, JSON.stringify(metrics, null, 2));
  } catch (err) {
    console.error('[Dashboard] Error saving metrics:', err.message);
  }
}

/**
 * Generate HTML dashboard
 */
function generateHTMLDashboard(metrics) {
  const html = `<!DOCTYPE html>
<html lang="de">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>MCP Token Metrics Dashboard</title>
  <style>
    * { margin: 0; padding: 0; box-sizing: border-box; }
    body {
      font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
      background: linear-gradient(135deg, #1e3a8a 0%, #1e3a8a 100%);
      color: #333;
      padding: 20px;
    }
    .container {
      max-width: 1200px;
      margin: 0 auto;
    }
    h1 {
      color: white;
      margin-bottom: 30px;
      font-size: 28px;
    }
    .timestamp {
      color: rgba(255, 255, 255, 0.7);
      font-size: 12px;
      margin-top: 10px;
    }
    .grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 20px;
      margin-bottom: 30px;
    }
    .card {
      background: white;
      border-radius: 8px;
      padding: 20px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }
    .card h2 {
      font-size: 14px;
      color: #666;
      text-transform: uppercase;
      margin-bottom: 10px;
      font-weight: 600;
    }
    .metric {
      font-size: 32px;
      font-weight: 700;
      color: #1e3a8a;
      margin-bottom: 5px;
    }
    .label {
      font-size: 12px;
      color: #999;
    }
    .progress-bar {
      width: 100%;
      height: 8px;
      background: #eee;
      border-radius: 4px;
      margin: 15px 0 5px 0;
      overflow: hidden;
    }
    .progress-fill {
      height: 100%;
      background: linear-gradient(90deg, #10b981, #3b82f6);
      transition: width 0.3s ease;
    }
    .server-list {
      background: white;
      border-radius: 8px;
      padding: 20px;
      box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }
    .server-list h2 {
      font-size: 18px;
      margin-bottom: 15px;
      color: #1e3a8a;
    }
    .server-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 0;
      border-bottom: 1px solid #eee;
    }
    .server-item:last-child {
      border-bottom: none;
    }
    .server-name {
      font-weight: 600;
      color: #333;
    }
    .server-metrics {
      display: flex;
      gap: 20px;
      font-size: 12px;
      color: #666;
    }
    .status-good { color: #10b981; font-weight: 600; }
    .status-warning { color: #f59e0b; font-weight: 600; }
    .status-critical { color: #ef4444; font-weight: 600; }
    .footer {
      text-align: center;
      color: rgba(255, 255, 255, 0.5);
      margin-top: 40px;
      font-size: 12px;
    }
  </style>
</head>
<body>
  <div class="container">
    <h1>🎯 MCP Token Metrics Dashboard</h1>
    <div class="timestamp">Updated: ${metrics.timestamp}</div>
    
    <div class="grid">
      <div class="card">
        <h2>Cache Hit Rate</h2>
        <div class="metric">${metrics.cache.hitRate}%</div>
        <div class="progress-bar">
          <div class="progress-fill" style="width: ${Math.min(metrics.cache.hitRate, 100)}%"></div>
        </div>
        <div class="label">Target: 40-60%</div>
      </div>
      
      <div class="card">
        <h2>Tokens Saved</h2>
        <div class="metric">${metrics.cache.totalTokensSaved}</div>
        <div class="label">Via caching this period</div>
      </div>
      
      <div class="card">
        <h2>Active Servers</h2>
        <div class="metric">${metrics.cache.servers ? Object.keys(metrics.cache.servers).length : 0}</div>
        <div class="label">Monitored MCP servers</div>
      </div>
    </div>
    
    <div class="server-list">
      <h2>📊 Server Statistics</h2>
      ${Object.entries(metrics.cache.servers || {}).map(([server, data]) => `
        <div class="server-item">
          <div class="server-name">${server}</div>
          <div class="server-metrics">
            <span>Hits: ${data.hits}</span>
            <span>Misses: ${data.misses}</span>
            <span class="status-good">Hit Rate: ${data.hitRate}</span>
            <span>Tokens: ${data.totalTokens}</span>
          </div>
        </div>
      `).join('')}
    </div>
    
    <div class="server-list" style="margin-top: 20px;">
      <h2>⚙️ Rate Limits</h2>
      ${Object.entries(metrics.rateLimits.servers || {}).map(([server, data]) => {
        const percentUsed = parseInt(data.percentUsed);
        const statusClass = percentUsed > 80 ? 'status-critical' : percentUsed > 60 ? 'status-warning' : 'status-good';
        return `
        <div class="server-item">
          <div class="server-name">${server}</div>
          <div class="server-metrics">
            <span>Usage: <span class="${statusClass}">${data.percentUsed}</span></span>
            <span>${data.tokensUsedToday}/${data.dailyLimit} tokens</span>
            <span>${data.calls} calls</span>
          </div>
        </div>
      `}).join('')}
    </div>
    
    <div class="footer">
      <p>B2X MCP Optimization Dashboard • Phase 1 Active</p>
      <p>Next Review: 2026-01-08 09:00</p>
    </div>
  </div>
</body>
</html>`;

  try {
    fs.writeFileSync(DASHBOARD_FILE, html);
    console.log(`[Dashboard] HTML dashboard generated: ${DASHBOARD_FILE}`);
  } catch (err) {
    console.error('[Dashboard] Error generating dashboard:', err.message);
  }
}

/**
 * Print text-based dashboard
 */
function printTextDashboard(metrics) {
  console.clear();
  console.log('╔════════════════════════════════════════════════════════════════╗');
  console.log('║          MCP Token Metrics Dashboard                           ║');
  console.log('╚════════════════════════════════════════════════════════════════╝');
  console.log();
  
  console.log('📊 CACHE METRICS');
  console.log(`  Hit Rate: ${metrics.cache.hitRate}% (Target: 40-60%)`);
  console.log(`  Tokens Saved: ~${metrics.cache.totalTokensSaved}`);
  console.log(`  Total Hits: ${metrics.cache.totalHits} | Misses: ${metrics.cache.totalMisses}`);
  console.log();
  
  console.log('⚙️ RATE LIMITS');
  Object.entries(metrics.rateLimits.servers || {}).forEach(([server, data]) => {
    const status = parseInt(data.percentUsed) > 80 ? '⚠️ ' : '✓ ';
    console.log(`  ${status}${server}: ${data.tokensUsedToday}/${data.dailyLimit} (${data.percentUsed})`);
  });
  console.log();
  
  console.log('🎯 SUMMARY');
  console.log(`  Cache Efficiency: ${metrics.summary.cacheEfficiency}`);
  console.log(`  Active Servers: ${metrics.summary.activeServers}`);
  console.log(`  Violations: ${metrics.summary.rateLimitViolations}`);
  console.log();
  
  console.log(`⏰ Updated: ${metrics.timestamp}`);
}

// Export functions
module.exports = {
  collectMetrics,
  saveMetrics,
  generateHTMLDashboard,
  printTextDashboard
};

// CLI usage
if (require.main === module) {
  const command = process.argv[2];
  const metrics = collectMetrics();
  
  switch (command) {
    case 'html':
      generateHTMLDashboard(metrics);
      break;
    case 'json':
      saveMetrics(metrics);
      console.log('[Dashboard] Metrics saved to JSON');
      break;
    case 'print':
    default:
      printTextDashboard(metrics);
      saveMetrics(metrics);
      generateHTMLDashboard(metrics);
  }
}
