#!/usr/bin/env node

/**
 * MCP A/B Testing Framework
 * Compares different optimization strategies for token efficiency
 */

const fs = require('fs');
const path = require('path');
const cacheManager = require('./mcp-cache-manager');
const rateLimiter = require('./mcp-rate-limiter');

const AB_TESTS_DIR = path.join(process.cwd(), '.ai/logs/mcp-usage/ab-tests');
const AB_RESULTS_FILE = path.join(process.cwd(), '.ai/logs/mcp-usage/ab-test-results.json');

// Ensure directory exists
if (!fs.existsSync(AB_TESTS_DIR)) {
  fs.mkdirSync(AB_TESTS_DIR, { recursive: true });
}

/**
 * Define A/B test configurations
 */
const TEST_CONFIGS = {
  baseline: {
    name: 'Baseline (No Optimization)',
    cachingEnabled: false,
    rateLimitingEnabled: false,
    batchingEnabled: false,
    description: 'Original MCP behavior without any optimization'
  },
  caching_only: {
    name: 'Caching Only',
    cachingEnabled: true,
    rateLimitingEnabled: false,
    batchingEnabled: false,
    description: 'Only file-hash caching enabled'
  },
  rate_limiting_only: {
    name: 'Rate Limiting Only',
    cachingEnabled: false,
    rateLimitingEnabled: true,
    batchingEnabled: false,
    description: 'Only daily rate limits enforced'
  },
  caching_and_rate_limiting: {
    name: 'Caching + Rate Limiting',
    cachingEnabled: true,
    rateLimitingEnabled: true,
    batchingEnabled: false,
    description: 'Both caching and rate limiting'
  },
  all_optimizations: {
    name: 'All Optimizations',
    cachingEnabled: true,
    rateLimitingEnabled: true,
    batchingEnabled: true,
    description: 'Full optimization suite enabled'
  }
};

/**
 * Create new A/B test
 */
function createTest(testName, duration = 86400000) { // Default 24 hours
  const test = {
    id: `${testName}_${Date.now()}`,
    name: testName,
    config: TEST_CONFIGS[testName],
    startTime: new Date().toISOString(),
    duration,
    endTime: new Date(Date.now() + duration).toISOString(),
    metrics: {
      tokensUsed: 0,
      cacheHits: 0,
      cacheMisses: 0,
      rateLimitBlocks: 0,
      averageLatency: 0,
      mobileLatency: 0,
      desktopLatency: 0,
      mobileFirstContentfulPaint: 0,
      desktopFirstContentfulPaint: 0
    },
    samples: [],
    status: 'running'
  };
  
  const filePath = path.join(AB_TESTS_DIR, `${test.id}.json`);
  fs.writeFileSync(filePath, JSON.stringify(test, null, 2));
  
  console.log(`[A/B Test] Created test: ${testName} (ID: ${test.id})`);
  return test;
}

/**
 * Record sample in test
 */
function recordSample(testId, sampleData) {
  const filePath = path.join(AB_TESTS_DIR, `${testId}.json`);
  
  try {
    const test = JSON.parse(fs.readFileSync(filePath, 'utf8'));
    
    test.samples.push({
      timestamp: new Date().toISOString(),
      ...sampleData
    });
    
    // Update aggregated metrics
    test.metrics.tokensUsed += sampleData.tokensUsed || 0;
    test.metrics.cacheHits += sampleData.cacheHit ? 1 : 0;
    test.metrics.cacheMisses += sampleData.cacheMiss ? 1 : 0;
    test.metrics.rateLimitBlocks += sampleData.rateLimitBlock ? 1 : 0;
    
    fs.writeFileSync(filePath, JSON.stringify(test, null, 2));
  } catch (err) {
    console.error(`[A/B Test] Error recording sample: ${err.message}`);
  }
}

/**
 * Complete test and calculate results
 */
function completeTest(testId) {
  const filePath = path.join(AB_TESTS_DIR, `${testId}.json`);
  
  try {
    const test = JSON.parse(fs.readFileSync(filePath, 'utf8'));
    
    // Calculate statistics
    const totalOps = test.metrics.cacheHits + test.metrics.cacheMisses;
    const hitRate = totalOps > 0 ? (test.metrics.cacheHits / totalOps * 100).toFixed(2) : 0;
    const avgLatency = test.samples.length > 0
      ? (test.samples.reduce((sum, s) => sum + (s.latency || 0), 0) / test.samples.length).toFixed(2)
      : 0;
    
    test.status = 'completed';
    test.results = {
      totalSamples: test.samples.length,
      cacheHitRate: `${hitRate}%`,
      averageLatency: `${avgLatency}ms`,
      tokensPerSample: (test.metrics.tokensUsed / test.samples.length).toFixed(2),
      rateLimitEffectiveness: `${test.metrics.rateLimitBlocks} blocks`,
      estimatedTokenSavings: Math.round(test.metrics.cacheHits * 10) // ~10 tokens per cache hit
    };
    
    fs.writeFileSync(filePath, JSON.stringify(test, null, 2));
    console.log(`[A/B Test] Completed test: ${testId}`);
    
    return test;
  } catch (err) {
    console.error(`[A/B Test] Error completing test: ${err.message}`);
  }
}

/**
 * Compare all completed tests
 */
function compareTests() {
  try {
    const files = fs.readdirSync(AB_TESTS_DIR).filter(f => f.endsWith('.json'));
    const tests = files.map(f => JSON.parse(fs.readFileSync(path.join(AB_TESTS_DIR, f), 'utf8')));
    const completed = tests.filter(t => t.status === 'completed');
    
    if (completed.length === 0) {
      console.log('[A/B Test] No completed tests found');
      return null;
    }
    
    const comparison = {
      timestamp: new Date().toISOString(),
      testsCompared: completed.length,
      results: {}
    };
    
    completed.forEach(test => {
      comparison.results[test.name] = {
        config: test.config.description,
        cacheHitRate: test.results.cacheHitRate,
        averageLatency: test.results.averageLatency,
        tokensPerSample: test.results.tokensPerSample,
        estimatedSavings: test.results.estimatedTokenSavings
      };
    });
    
    // Determine winner
    const sortedByTokens = completed.sort((a, b) => 
      parseFloat(a.results.tokensPerSample) - parseFloat(b.results.tokensPerSample)
    );
    
    comparison.winner = {
      name: sortedByTokens[0].name,
      tokensPerSample: sortedByTokens[0].results.tokensPerSample,
      estimatedTokenSavings: sortedByTokens[0].results.estimatedTokenSavings
    };
    
    fs.writeFileSync(AB_RESULTS_FILE, JSON.stringify(comparison, null, 2));
    return comparison;
  } catch (err) {
    console.error(`[A/B Test] Error comparing tests: ${err.message}`);
  }
}

/**
 * Print test results
 */
function printResults() {
  try {
    if (!fs.existsSync(AB_RESULTS_FILE)) {
      console.log('[A/B Test] No test results found');
      return;
    }
    
    const results = JSON.parse(fs.readFileSync(AB_RESULTS_FILE, 'utf8'));
    
    console.log('\n╔══════════════════════════════════════════════════════════╗');
    console.log('║           A/B Test Results Comparison                     ║');
    console.log('╚══════════════════════════════════════════════════════════╝\n');
    
    Object.entries(results.results).forEach(([name, data]) => {
      console.log(`📊 ${name}`);
      console.log(`   Config: ${data.config}`);
      console.log(`   Cache Hit Rate: ${data.cacheHitRate}`);
      console.log(`   Average Latency: ${data.averageLatency}`);
      console.log(`   Tokens/Sample: ${data.tokensPerSample}`);
      console.log(`   Est. Savings: ~${data.estimatedSavings} tokens\n`);
    });
    
    console.log(`🏆 Winner: ${results.winner.name}`);
    console.log(`   Tokens/Sample: ${results.winner.tokensPerSample}`);
    console.log(`   Est. Token Savings: ~${results.winner.estimatedTokenSavings}\n`);
  } catch (err) {
    console.error(`[A/B Test] Error printing results: ${err.message}`);
  }
}

// Export functions
module.exports = {
  TEST_CONFIGS,
  createTest,
  recordSample,
  completeTest,
  compareTests,
  printResults
};

// CLI usage
if (require.main === module) {
  const command = process.argv[2];
  const testName = process.argv[3];
  const testId = process.argv[4];
  
  switch (command) {
    case 'create':
      if (!testName) {
        console.error('Usage: node mcp-ab-testing.js create <test-name>');
        console.log('\nAvailable tests:', Object.keys(TEST_CONFIGS).join(', '));
        process.exit(1);
      }
      createTest(testName);
      break;
    
    case 'complete':
      if (!testId) {
        console.error('Usage: node mcp-ab-testing.js complete <test-id>');
        process.exit(1);
      }
      completeTest(testId);
      break;
    
    case 'compare':
      const results = compareTests();
      if (results) {
        printResults();
      }
      break;
    
    case 'results':
      printResults();
      break;
    
    case 'list':
      const files = fs.readdirSync(AB_TESTS_DIR);
      console.log('Available tests:');
      files.forEach(f => console.log(`  ${f}`));
      break;
    
    default:
      console.log('Usage:');
      console.log('  node mcp-ab-testing.js create <test-name>');
      console.log('  node mcp-ab-testing.js complete <test-id>');
      console.log('  node mcp-ab-testing.js compare');
      console.log('  node mcp-ab-testing.js results');
      console.log('  node mcp-ab-testing.js list');
  }
}
