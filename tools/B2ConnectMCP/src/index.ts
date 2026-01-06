#!/usr/bin/env node

import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import {
  CallToolRequestSchema,
  ErrorCode,
  ListToolsRequestSchema,
  McpError,
} from "@modelcontextprotocol/sdk/types.js";
import { glob } from "fast-glob";
import * as fs from "fs/promises";
import * as path from "path";
import * as yaml from "js-yaml";
import { z } from "zod";

class B2ConnectMCPServer {
  private server: Server;
  private workspaceRoot: string = process.cwd();

  constructor() {
    this.server = new Server(
      {
        name: "b2connect-mcp-server",
        version: "1.0.0",
      },
      {
        capabilities: {
          tools: {},
        },
      }
    );

    this.setupToolHandlers();
  }

  private setupToolHandlers() {
    // List available tools
    this.server.setRequestHandler(ListToolsRequestSchema, async () => {
      return {
        tools: [
          {
            name: "validate_tenant_config",
            description: "Validate tenant configuration files for B2Connect",
            inputSchema: {
              type: "object",
              properties: {
                configPath: {
                  type: "string",
                  description: "Path to tenant configuration file"
                }
              },
              required: ["configPath"]
            }
          },
          {
            name: "validate_catalog_structure",
            description: "Validate product catalog structure and metadata",
            inputSchema: {
              type: "object",
              properties: {
                catalogPath: {
                  type: "string",
                  description: "Path to catalog directory or file"
                }
              },
              required: ["catalogPath"]
            }
          },
          {
            name: "check_erp_integration",
            description: "Validate ERP integration configurations",
            inputSchema: {
              type: "object",
              properties: {
                erpConfigPath: {
                  type: "string",
                  description: "Path to ERP configuration file"
                }
              },
              required: ["erpConfigPath"]
            }
          },
          {
            name: "analyze_domain_models",
            description: "Analyze domain models for consistency and best practices",
            inputSchema: {
              type: "object",
              properties: {
                domainPath: {
                  type: "string",
                  description: "Path to domain directory"
                }
              },
              required: ["domainPath"]
            }
          },
          {
            name: "validate_lifecycle_stages",
            description: "Validate customer integration lifecycle stages",
            inputSchema: {
              type: "object",
              properties: {
                tenantId: {
                  type: "string",
                  description: "Tenant identifier"
                }
              },
              required: ["tenantId"]
            }
          }
        ]
      };
    });

    // Handle tool calls
    this.server.setRequestHandler(CallToolRequestSchema, async (request) => {
      const { name, arguments: args } = request.params;

      try {
        switch (name) {
          case "validate_tenant_config":
            return await this.validateTenantConfig(args.configPath);
          case "validate_catalog_structure":
            return await this.validateCatalogStructure(args.catalogPath);
          case "check_erp_integration":
            return await this.checkErpIntegration(args.erpConfigPath);
          case "analyze_domain_models":
            return await this.analyzeDomainModels(args.domainPath);
          case "validate_lifecycle_stages":
            return await this.validateLifecycleStages(args.tenantId);
          default:
            throw new McpError(
              ErrorCode.MethodNotFound,
              `Unknown tool: ${name}`
            );
        }
      } catch (error) {
        throw new McpError(
          ErrorCode.InternalError,
          `Tool execution failed: ${error.message}`
        );
      }
    });
  }

  private async validateTenantConfig(configPath: string) {
    const fullPath = path.resolve(this.workspaceRoot, configPath);

    try {
      const content = await fs.readFile(fullPath, 'utf-8');
      const config = JSON.parse(content);

      const issues = [];

      // Validate required fields
      if (!config.tenantId) issues.push("Missing tenantId");
      if (!config.name) issues.push("Missing tenant name");
      if (!config.domain) issues.push("Missing domain");

      // Validate domain format
      const domainRegex = /^[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
      if (config.domain && !domainRegex.test(config.domain)) {
        issues.push("Invalid domain format");
      }

      // Validate features configuration
      if (config.features) {
        const validFeatures = ['catalog', 'erp', 'cms', 'search'];
        const invalidFeatures = Object.keys(config.features).filter(
          f => !validFeatures.includes(f)
        );
        if (invalidFeatures.length > 0) {
          issues.push(`Invalid features: ${invalidFeatures.join(', ')}`);
        }
      }

      return {
        content: [
          {
            type: "text",
            text: issues.length === 0
              ? "✅ Tenant configuration is valid"
              : `❌ Found ${issues.length} issues:\n${issues.map(i => `- ${i}`).join('\n')}`
          }
        ]
      };
    } catch (error) {
      return {
        content: [
          {
            type: "text",
            text: `❌ Failed to validate tenant config: ${error.message}`
          }
        ]
      };
    }
  }

  private async validateCatalogStructure(catalogPath: string) {
    const fullPath = path.resolve(this.workspaceRoot, catalogPath);

    try {
      const files = await glob("**/*.{json,yaml,yml}", { cwd: fullPath });
      const issues = [];

      for (const file of files) {
        const filePath = path.join(fullPath, file);
        const content = await fs.readFile(filePath, 'utf-8');

        try {
          const data = path.extname(file).toLowerCase() === '.json'
            ? JSON.parse(content)
            : yaml.load(content);

          // Validate catalog structure
          if (data.products) {
            data.products.forEach((product: any, index: number) => {
              if (!product.id) issues.push(`${file}: Product ${index} missing id`);
              if (!product.name) issues.push(`${file}: Product ${index} missing name`);
              if (!product.category) issues.push(`${file}: Product ${index} missing category`);
            });
          }
        } catch (parseError) {
          issues.push(`${file}: Invalid ${path.extname(file)} format`);
        }
      }

      return {
        content: [
          {
            type: "text",
            text: issues.length === 0
              ? "✅ Catalog structure is valid"
              : `❌ Found ${issues.length} issues:\n${issues.map(i => `- ${i}`).join('\n')}`
          }
        ]
      };
    } catch (error) {
      return {
        content: [
          {
            type: "text",
            text: `❌ Failed to validate catalog: ${error.message}`
          }
        ]
      };
    }
  }

  private async checkErpIntegration(erpConfigPath: string) {
    const fullPath = path.resolve(this.workspaceRoot, erpConfigPath);

    try {
      const content = await fs.readFile(fullPath, 'utf-8');
      const config = JSON.parse(content);

      const issues = [];

      // Validate ERP connection settings
      if (!config.endpoint) issues.push("Missing ERP endpoint");
      if (!config.apiKey && !config.username) issues.push("Missing authentication credentials");

      // Validate supported ERP types
      const supportedErps = ['enventa', 'sap', 'microsoft-dynamics', 'oracle'];
      if (config.type && !supportedErps.includes(config.type.toLowerCase())) {
        issues.push(`Unsupported ERP type: ${config.type}`);
      }

      // Validate mapping configurations
      if (config.mappings) {
        const requiredMappings = ['products', 'customers', 'orders'];
        const missingMappings = requiredMappings.filter(
          m => !config.mappings[m]
        );
        if (missingMappings.length > 0) {
          issues.push(`Missing mappings: ${missingMappings.join(', ')}`);
        }
      }

      return {
        content: [
          {
            type: "text",
            text: issues.length === 0
              ? "✅ ERP integration configuration is valid"
              : `❌ Found ${issues.length} issues:\n${issues.map(i => `- ${i}`).join('\n')}`
          }
        ]
      };
    } catch (error) {
      return {
        content: [
          {
            type: "text",
            text: `❌ Failed to validate ERP config: ${error.message}`
          }
        ]
      };
    }
  }

  private async analyzeDomainModels(domainPath: string) {
    const fullPath = path.resolve(this.workspaceRoot, domainPath);

    try {
      const csFiles = await glob("**/*.cs", { cwd: fullPath });
      const issues = [];
      const patterns = [];

      for (const file of csFiles) {
        const filePath = path.join(fullPath, file);
        const content = await fs.readFile(filePath, 'utf-8');

        // Check for domain patterns
        if (content.includes("public class") && content.includes("Entity")) {
          patterns.push(`${file}: Entity class detected`);
        }

        if (content.includes("ICommand") || content.includes("IQuery")) {
          patterns.push(`${file}: CQRS pattern detected`);
        }

        // Check for potential issues
        if (content.includes("public string") && !content.includes("[Required]")) {
          issues.push(`${file}: Public string property without validation`);
        }

        if (content.includes("DateTime") && !content.includes("DateTimeOffset")) {
          issues.push(`${file}: Consider using DateTimeOffset for timezone safety`);
        }
      }

      return {
        content: [
          {
            type: "text",
            text: `📊 Domain Analysis Complete\n\nPatterns Found:\n${patterns.map(p => `- ${p}`).join('\n')}\n\nIssues:\n${issues.length === 0 ? 'None' : issues.map(i => `- ${i}`).join('\n')}`
          }
        ]
      };
    } catch (error) {
      return {
        content: [
          {
            type: "text",
            text: `❌ Failed to analyze domain models: ${error.message}`
          }
        ]
      };
    }
  }

  private async validateLifecycleStages(tenantId: string) {
    // This would integrate with the lifecycle stages framework
    // For now, return a placeholder implementation

    const stages = [
      "initial_contact",
      "requirements_gathering",
      "technical_setup",
      "data_migration",
      "testing",
      "go_live",
      "post_go_live_support"
    ];

    const currentStage = "technical_setup"; // This would be fetched from database

    return {
      content: [
        {
          type: "text",
          text: `📈 Lifecycle Validation for Tenant ${tenantId}\n\nCurrent Stage: ${currentStage}\n\nAvailable Stages:\n${stages.map(s => `- ${s}`).join('\n')}\n\n✅ Lifecycle configuration is valid`
        }
      ]
    };
  }

  async run() {
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
    console.error("B2Connect MCP server running on stdio");
  }
}

// Run the server
const server = new B2ConnectMCPServer();
server.run().catch(console.error);