#!/usr/bin/env node

/**
 * MCP Cache Manager
 * Manages caching of MCP results to prevent unnecessary re-runs
 * Tracks file changes via hash and returns cached results when files unchanged
 */

const crypto = require('crypto');
const fs = require('fs');
const path = require('path');

const CACHE_DIR = path.join(process.cwd(), '.ai/cache/mcp');
const STATS_FILE = path.join(process.cwd(), '.ai/logs/mcp-usage/mcp-stats.json');

// Ensure cache directory exists
if (!fs.existsSync(CACHE_DIR)) {
  fs.mkdirSync(CACHE_DIR, { recursive: true });
}

// Ensure stats directory exists
if (!fs.existsSync(path.dirname(STATS_FILE))) {
  fs.mkdirSync(path.dirname(STATS_FILE), { recursive: true });
}

/**
 * Calculate SHA256 hash of file content
 */
function getFileHash(filePath) {
  try {
    const content = fs.readFileSync(filePath, 'utf8');
    return crypto.createHash('sha256').update(content).digest('hex');
  } catch (err) {
    console.error(`[MCP Cache] Error reading file: ${filePath}`, err.message);
    return null;
  }
}

/**
 * Generate cache key from server name and file path
 */
function getCacheKey(serverName, filePath) {
  const hash = getFileHash(filePath);
  return `${serverName}_${hash}`;
}

/**
 * Get cached result if available and file unchanged
 */
function getCachedResult(serverName, filePath) {
  try {
    const cacheKey = getCacheKey(serverName, filePath);
    const cacheFile = path.join(CACHE_DIR, `${cacheKey}.json`);
    
    if (fs.existsSync(cacheFile)) {
      const cached = JSON.parse(fs.readFileSync(cacheFile, 'utf8'));
      logStats(serverName, 'hit', 0);
      console.log(`[MCP Cache] Cache HIT for ${serverName} on ${path.basename(filePath)}`);
      return cached.result;
    }
  } catch (err) {
    console.error(`[MCP Cache] Error reading cache:`, err.message);
  }
  return null;
}

/**
 * Store result in cache
 */
function cacheResult(serverName, filePath, result, tokensUsed = 0) {
  try {
    const cacheKey = getCacheKey(serverName, filePath);
    const cacheFile = path.join(CACHE_DIR, `${cacheKey}.json`);
    
    const cacheData = {
      timestamp: new Date().toISOString(),
      serverName,
      filePath,
      hash: getFileHash(filePath),
      tokensUsed,
      result
    };
    
    fs.writeFileSync(cacheFile, JSON.stringify(cacheData, null, 2));
    logStats(serverName, 'miss', tokensUsed);
    console.log(`[MCP Cache] Result cached for ${serverName} on ${path.basename(filePath)}`);
  } catch (err) {
    console.error(`[MCP Cache] Error writing cache:`, err.message);
  }
}

/**
 * Log cache statistics
 */
function logStats(serverName, cacheStatus, tokensUsed) {
  try {
    let stats = {};
    if (fs.existsSync(STATS_FILE)) {
      stats = JSON.parse(fs.readFileSync(STATS_FILE, 'utf8'));
    }
    
    if (!stats[serverName]) {
      stats[serverName] = {
        hits: 0,
        misses: 0,
        totalTokens: 0,
        lastUpdated: new Date().toISOString()
      };
    }
    
    if (cacheStatus === 'hit') {
      stats[serverName].hits++;
    } else {
      stats[serverName].misses++;
    }
    stats[serverName].totalTokens += tokensUsed;
    stats[serverName].lastUpdated = new Date().toISOString();
    
    fs.writeFileSync(STATS_FILE, JSON.stringify(stats, null, 2));
  } catch (err) {
    console.error(`[MCP Cache] Error logging stats:`, err.message);
  }
}

/**
 * Clear expired cache entries (older than 7 days)
 */
function clearExpiredCache() {
  try {
    const maxAge = 7 * 24 * 60 * 60 * 1000; // 7 days in ms
    const now = Date.now();
    
    const files = fs.readdirSync(CACHE_DIR);
    let cleared = 0;
    
    files.forEach(file => {
      const filePath = path.join(CACHE_DIR, file);
      const stat = fs.statSync(filePath);
      if (now - stat.mtimeMs > maxAge) {
        fs.unlinkSync(filePath);
        cleared++;
      }
    });
    
    if (cleared > 0) {
      console.log(`[MCP Cache] Cleared ${cleared} expired cache entries`);
    }
  } catch (err) {
    console.error(`[MCP Cache] Error clearing expired cache:`, err.message);
  }
}

/**
 * Get cache statistics
 */
function getStats() {
  try {
    if (fs.existsSync(STATS_FILE)) {
      return JSON.parse(fs.readFileSync(STATS_FILE, 'utf8'));
    }
  } catch (err) {
    console.error(`[MCP Cache] Error reading stats:`, err.message);
  }
  return {};
}

/**
 * Print cache statistics
 */
function printStats() {
  const stats = getStats();
  console.log('\n=== MCP Cache Statistics ===');
  Object.entries(stats).forEach(([server, data]) => {
    const hitRate = data.hits + data.misses > 0 
      ? ((data.hits / (data.hits + data.misses)) * 100).toFixed(1)
      : 0;
    console.log(`\n${server}:`);
    console.log(`  Hits: ${data.hits}`);
    console.log(`  Misses: ${data.misses}`);
    console.log(`  Hit Rate: ${hitRate}%`);
    console.log(`  Total Tokens: ${data.totalTokens}`);
    console.log(`  Last Updated: ${data.lastUpdated}`);
  });
  console.log('\n============================\n');
}

// Export functions
module.exports = {
  getCachedResult,
  cacheResult,
  clearExpiredCache,
  getStats,
  printStats
};

// CLI usage
if (require.main === module) {
  const command = process.argv[2];
  
  switch (command) {
    case 'stats':
      printStats();
      break;
    case 'clear-expired':
      clearExpiredCache();
      console.log('[MCP Cache] Expired cache entries cleared');
      break;
    case 'clear-all':
      const files = fs.readdirSync(CACHE_DIR);
      files.forEach(file => fs.unlinkSync(path.join(CACHE_DIR, file)));
      fs.writeFileSync(STATS_FILE, JSON.stringify({}, null, 2));
      console.log('[MCP Cache] All cache cleared');
      break;
    default:
      console.log('Usage: node mcp-cache-manager.js [stats|clear-expired|clear-all]');
  }
}
