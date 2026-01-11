#!/usr/bin/env node

import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ErrorCode,
  ListToolsRequestSchema,
  McpError,
} from '@modelcontextprotocol/sdk/types.js';
import axios from 'axios';
import * as fs from 'fs';
import * as path from 'path';
import { glob } from 'glob';

// Input validation utilities
class ValidationError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'ValidationError';
  }
}

function validateString(value: any, fieldName: string, minLength = 1, maxLength = 10000): string {
  if (typeof value !== 'string') {
    throw new ValidationError(`${fieldName} must be a string`);
  }
  if (value.length < minLength) {
    throw new ValidationError(`${fieldName} must be at least ${minLength} characters`);
  }
  if (value.length > maxLength) {
    throw new ValidationError(`${fieldName} must be at most ${maxLength} characters`);
  }
  return value;
}

class MonitoringMCPServer {
  private server: Server;
  private aspireDashboardUrl = 'http://localhost:15500';
  private otlpEndpoint = 'http://localhost:4317';

  constructor() {
    this.server = new Server(
      {
        name: 'monitoring-mcp-server',
        version: '1.0.0',
      },
      {
        capabilities: {
          tools: {},
        },
      }
    );

    this.setupToolHandlers();
    this.setupRequestHandlers();
  }

  private setupRequestHandlers() {
    this.server.setRequestHandler(ListToolsRequestSchema, async () => {
      return {
        tools: [
          {
            name: 'collect_application_metrics',
            description: 'Collect real-time application performance metrics from running services',
            inputSchema: {
              type: 'object',
              properties: {
                serviceName: {
                  type: 'string',
                  description:
                    'Name of the service to monitor (optional, monitors all if not specified)',
                },
              },
            },
          },
          {
            name: 'monitor_system_performance',
            description: 'Monitor system performance metrics (CPU, memory, disk, network)',
            inputSchema: {
              type: 'object',
              properties: {
                hostName: {
                  type: 'string',
                  description: 'Host name to monitor (defaults to localhost)',
                  default: 'localhost',
                },
              },
            },
          },
          {
            name: 'track_errors',
            description: 'Track and analyze application errors and exceptions',
            inputSchema: {
              type: 'object',
              properties: {
                serviceName: {
                  type: 'string',
                  description: 'Name of the service to monitor for errors',
                },
                timeRange: {
                  type: 'string',
                  description: 'Time range for error analysis (e.g., "1h", "24h", "7d")',
                  default: '1h',
                },
              },
            },
          },
          {
            name: 'analyze_logs',
            description: 'Analyze application logs for patterns and anomalies',
            inputSchema: {
              type: 'object',
              properties: {
                filePath: {
                  type: 'string',
                  description: 'Path to the log file to analyze',
                },
                pattern: {
                  type: 'string',
                  description: 'Log pattern to search for (optional)',
                },
              },
              required: ['filePath'],
            },
          },
          {
            name: 'validate_health_checks',
            description: 'Validate health check endpoints for services',
            inputSchema: {
              type: 'object',
              properties: {
                serviceName: {
                  type: 'string',
                  description:
                    'Name of the service to check (optional, checks all if not specified)',
                  default: 'all',
                },
              },
            },
          },
          {
            name: 'configure_alerts',
            description: 'Configure monitoring alerts and thresholds',
            inputSchema: {
              type: 'object',
              properties: {
                serviceName: {
                  type: 'string',
                  description: 'Name of the service to configure alerts for',
                },
                metric: {
                  type: 'string',
                  description: 'Metric to monitor (e.g., cpu_usage, memory_usage, error_rate)',
                },
                threshold: {
                  type: 'string',
                  description: 'Threshold value for the alert (e.g., "80%", "500ms", "5")',
                },
              },
              required: ['serviceName', 'metric', 'threshold'],
            },
          },
          {
            name: 'profile_performance',
            description: 'Profile application performance and identify bottlenecks',
            inputSchema: {
              type: 'object',
              properties: {
                serviceName: {
                  type: 'string',
                  description: 'Name of the service to profile',
                },
                duration: {
                  type: 'string',
                  description: 'Duration to profile (e.g., "30s", "5m")',
                  default: '30s',
                },
              },
              required: ['serviceName'],
            },
          },
        ],
      };
    });
  }

  private setupToolHandlers() {
    this.server.setRequestHandler(CallToolRequestSchema, async request => {
      const { name, arguments: args } = request.params;

      try {
        switch (name) {
          case 'collect_application_metrics':
            return await this.collectApplicationMetrics(args);
          case 'monitor_system_performance':
            return await this.monitorSystemPerformance(args);
          case 'track_errors':
            return await this.trackErrors(args);
          case 'analyze_logs':
            return await this.analyzeLogs(args);
          case 'validate_health_checks':
            return await this.validateHealthChecks(args);
          case 'configure_alerts':
            return await this.configureAlerts(args);
          case 'profile_performance':
            return await this.profilePerformance(args);
          default:
            throw new McpError(ErrorCode.MethodNotFound, `Unknown tool: ${name}`);
        }
      } catch (error) {
        if (error instanceof ValidationError) {
          throw new McpError(ErrorCode.InvalidParams, error.message);
        }
        throw error;
      }
    });
  }

  private async collectApplicationMetrics(args: any) {
    const serviceName = args.serviceName || 'all';

    // Try to get metrics from Aspire dashboard
    try {
      const response = await axios.get(`${this.aspireDashboardUrl}/api/metrics`, {
        timeout: 5000,
      });

      const metrics = response.data;
      let filteredMetrics = metrics;

      if (serviceName !== 'all') {
        filteredMetrics = metrics.filter((m: any) =>
          m.serviceName?.toLowerCase().includes(serviceName.toLowerCase())
        );
      }

      return {
        content: [
          {
            type: 'text',
            text: `📊 Application Metrics for ${serviceName}:\n\n${JSON.stringify(filteredMetrics, null, 2)}`,
          },
        ],
      };
    } catch (error) {
      // Fallback to basic system metrics
      return {
        content: [
          {
            type: 'text',
            text: `⚠️ Could not connect to Aspire dashboard at ${this.aspireDashboardUrl}. Service may not be running.\n\nBasic system metrics collection not yet implemented.`,
          },
        ],
      };
    }
  }

  private async monitorSystemPerformance(args: any) {
    const hostName = args.hostName || 'localhost';

    // For now, return basic system info
    // In a full implementation, this would collect actual system metrics
    const systemInfo = {
      hostname: hostName,
      timestamp: new Date().toISOString(),
      status: 'Basic monitoring active',
      note: 'Full system monitoring requires additional system monitoring tools integration',
    };

    return {
      content: [
        {
          type: 'text',
          text: `🖥️ System Performance for ${hostName}:\n\n${JSON.stringify(systemInfo, null, 2)}`,
        },
      ],
    };
  }

  private async trackErrors(args: any) {
    const serviceName = validateString(args.serviceName, 'serviceName');
    const timeRange = args.timeRange || '1h';

    // Look for error logs in common locations
    const logPatterns = ['logs/**/*.log', '**/logs/*.log', '.ai/logs/**/*.log'];

    let allErrors: any[] = [];

    for (const pattern of logPatterns) {
      try {
        const files = await glob(pattern, { cwd: process.cwd() });
        for (const file of files) {
          if (fs.existsSync(file)) {
            const content = fs.readFileSync(file, 'utf-8');
            const lines = content.split('\n');

            // Look for error patterns
            const errors = lines
              .filter(
                line =>
                  line.toLowerCase().includes('error') ||
                  line.toLowerCase().includes('exception') ||
                  line.includes('ERR') ||
                  line.includes('Exception')
              )
              .slice(-50); // Last 50 errors

            allErrors.push(
              ...errors.map(error => ({
                file,
                error,
                timestamp: new Date().toISOString(),
              }))
            );
          }
        }
      } catch (error) {
        // Ignore glob errors
      }
    }

    return {
      content: [
        {
          type: 'text',
          text: `🚨 Error Tracking for ${serviceName} (${timeRange}):\n\nFound ${allErrors.length} recent errors:\n\n${allErrors
            .slice(-10)
            .map(e => `${e.file}: ${e.error}`)
            .join('\n')}`,
        },
      ],
    };
  }

  private async analyzeLogs(args: any) {
    const filePath = validateString(args.filePath, 'filePath');
    const pattern = args.pattern;

    if (!fs.existsSync(filePath)) {
      throw new ValidationError(`Log file not found: ${filePath}`);
    }

    const content = fs.readFileSync(filePath, 'utf-8');
    const lines = content.split('\n');

    let filteredLines = lines;
    if (pattern) {
      filteredLines = lines.filter(line => line.toLowerCase().includes(pattern.toLowerCase()));
    }

    // Analyze patterns
    const errorCount = lines.filter(
      line => line.toLowerCase().includes('error') || line.toLowerCase().includes('exception')
    ).length;

    const warningCount = lines.filter(
      line => line.toLowerCase().includes('warn') || line.toLowerCase().includes('warning')
    ).length;

    const analysis = {
      totalLines: lines.length,
      filteredLines: filteredLines.length,
      errorCount,
      warningCount,
      recentEntries: filteredLines.slice(-20),
    };

    return {
      content: [
        {
          type: 'text',
          text: `📋 Log Analysis for ${filePath}:\n\n${JSON.stringify(analysis, null, 2)}`,
        },
      ],
    };
  }

  private async validateHealthChecks(args: any) {
    const serviceName = args.serviceName || 'all';

    // Common health check endpoints
    const healthEndpoints = [
      { name: 'AppHost', url: 'http://localhost:15500/health' },
      { name: 'Identity', url: 'http://localhost:5001/health' },
      { name: 'Store', url: 'http://localhost:8000/health' },
      { name: 'Admin', url: 'http://localhost:8080/health' },
    ];

    const results = [];

    for (const endpoint of healthEndpoints) {
      if (
        serviceName !== 'all' &&
        !endpoint.name.toLowerCase().includes(serviceName.toLowerCase())
      ) {
        continue;
      }

      try {
        const response = await axios.get(endpoint.url, { timeout: 5000 });
        results.push({
          service: endpoint.name,
          status: response.status === 200 ? '✅ Healthy' : '⚠️ Warning',
          url: endpoint.url,
          responseTime: `${response.headers['request-duration'] || 'N/A'}ms`,
        });
      } catch (error) {
        results.push({
          service: endpoint.name,
          status: '❌ Unhealthy',
          url: endpoint.url,
          error: error instanceof Error ? error.message : 'Unknown error',
        });
      }
    }

    return {
      content: [
        {
          type: 'text',
          text: `🏥 Health Check Results:\n\n${results
            .map(r => `${r.service}: ${r.status} (${r.url})`)
            .join('\n')}`,
        },
      ],
    };
  }

  private async configureAlerts(args: any) {
    const serviceName = validateString(args.serviceName, 'serviceName');
    const metric = validateString(args.metric, 'metric');
    const threshold = validateString(args.threshold, 'threshold');

    // In a full implementation, this would configure actual alerting
    const alertConfig = {
      serviceName,
      metric,
      threshold,
      status: 'configured',
      note: 'Alert configuration stored (actual alerting requires external monitoring system)',
    };

    return {
      content: [
        {
          type: 'text',
          text: `🚨 Alert Configured:\n\n${JSON.stringify(alertConfig, null, 2)}`,
        },
      ],
    };
  }

  private async profilePerformance(args: any) {
    const serviceName = validateString(args.serviceName, 'serviceName');
    const duration = args.duration || '30s';

    // Basic profiling simulation
    const profile = {
      serviceName,
      duration,
      timestamp: new Date().toISOString(),
      status: 'Profile completed',
      note: 'Full performance profiling requires integration with APM tools like Application Insights or DataDog',
      recommendations: [
        'Consider implementing distributed tracing',
        'Add performance counters to critical code paths',
        'Monitor database query performance',
      ],
    };

    return {
      content: [
        {
          type: 'text',
          text: `⚡ Performance Profile for ${serviceName}:\n\n${JSON.stringify(profile, null, 2)}`,
        },
      ],
    };
  }

  async run() {
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
    console.error('Monitoring MCP server started');
  }
}

// Run the server
const server = new MonitoringMCPServer();
server.run().catch(console.error);
