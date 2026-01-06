#!/usr/bin/env node

import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import {
  CallToolRequestSchema,
  ErrorCode,
  ListToolsRequestSchema,
  McpError,
} from '@modelcontextprotocol/sdk/types.js';
import * as fs from 'fs';
import * as path from 'path';
import { Client as PgClient } from 'pg';
import { Client as EsClient } from '@elastic/elasticsearch';

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

function validateWorkspacePath(workspacePath: any): string {
  const path = validateString(workspacePath, 'workspacePath', 1, 500);

  // Prevent path traversal attacks
  if (path.includes('..') || path.includes('../') || path.startsWith('/')) {
    throw new ValidationError('workspacePath must be a relative path without .. or absolute paths');
  }

  return path;
}

class DatabaseMCPServer {
  private server: Server;

  constructor() {
    this.server = new Server(
      {
        name: 'database-mcp-server',
        version: '1.0.0',
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
    this.server.setRequestHandler(ListToolsRequestSchema, async () => {
      return {
        tools: [
          {
            name: 'analyze_sql_queries',
            description:
              'Analyze SQL queries for performance issues, security vulnerabilities, and optimization opportunities',
            inputSchema: {
              type: 'object',
              properties: {
                sqlQueries: {
                  type: 'string',
                  description:
                    'SQL queries to analyze (can be multiple queries separated by semicolons)',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace path to analyze for SQL patterns',
                },
                databaseType: {
                  type: 'string',
                  enum: ['postgresql', 'mysql', 'sqlserver'],
                  description: 'Database type for analysis',
                  default: 'postgresql',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'validate_database_schema',
            description:
              'Validate database schema for consistency, naming conventions, and best practices',
            inputSchema: {
              type: 'object',
              properties: {
                schemaFile: {
                  type: 'string',
                  description: 'Path to schema file (SQL or migration file)',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace path containing schema files',
                },
                databaseType: {
                  type: 'string',
                  enum: ['postgresql', 'mysql', 'sqlserver'],
                  description: 'Database type',
                  default: 'postgresql',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'analyze_indexes',
            description: 'Analyze database indexes for performance and recommend optimizations',
            inputSchema: {
              type: 'object',
              properties: {
                schemaFile: {
                  type: 'string',
                  description: 'Path to schema file with index definitions',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace path to analyze',
                },
                databaseType: {
                  type: 'string',
                  enum: ['postgresql', 'mysql', 'sqlserver'],
                  description: 'Database type',
                  default: 'postgresql',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'validate_elasticsearch_mapping',
            description: 'Validate Elasticsearch index mappings for consistency and best practices',
            inputSchema: {
              type: 'object',
              properties: {
                mappingFile: {
                  type: 'string',
                  description: 'Path to Elasticsearch mapping file (JSON)',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace path containing mapping files',
                },
                indexName: {
                  type: 'string',
                  description: 'Elasticsearch index name',
                },
              },
              required: ['workspacePath'],
            },
          },
          {
            name: 'analyze_database_migrations',
            description:
              'Analyze database migration files for safety, performance, and rollback capabilities',
            inputSchema: {
              type: 'object',
              properties: {
                migrationPath: {
                  type: 'string',
                  description: 'Path to migration files directory',
                },
                workspacePath: {
                  type: 'string',
                  description: 'Workspace path containing migrations',
                },
                databaseType: {
                  type: 'string',
                  enum: ['postgresql', 'mysql', 'sqlserver'],
                  description: 'Database type',
                  default: 'postgresql',
                },
              },
              required: ['workspacePath'],
            },
          },
        ],
      };
    });

    this.server.setRequestHandler(CallToolRequestSchema, async request => {
      const { name, arguments: args } = request.params;

      try {
        switch (name) {
          case 'analyze_sql_queries':
            return await this.analyzeSqlQueries(args);
          case 'validate_database_schema':
            return await this.validateDatabaseSchema(args);
          case 'analyze_indexes':
            return await this.analyzeIndexes(args);
          case 'validate_elasticsearch_mapping':
            return await this.validateElasticsearchMapping(args);
          case 'analyze_database_migrations':
            return await this.analyzeDatabaseMigrations(args);
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

  private async analyzeSqlQueries(args: any) {
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const sqlQueries = args.sqlQueries
      ? validateString(args.sqlQueries, 'sqlQueries', 1, 50000)
      : '';
    const databaseType = args.databaseType || 'postgresql';

    const issues: string[] = [];
    const suggestions: string[] = [];

    // Find SQL files in workspace
    const sqlFiles = this.findSqlFiles(workspacePath);

    if (sqlQueries) {
      // Analyze provided queries
      const queryIssues = this.analyzeSqlQuery(sqlQueries, databaseType);
      issues.push(...queryIssues.issues);
      suggestions.push(...queryIssues.suggestions);
    }

    // Analyze files
    for (const file of sqlFiles.slice(0, 10)) {
      // Limit to 10 files
      try {
        const content = fs.readFileSync(file, 'utf8');
        const fileIssues = this.analyzeSqlQuery(content, databaseType);
        issues.push(...fileIssues.issues.map(issue => `${path.basename(file)}: ${issue}`));
        suggestions.push(...fileIssues.suggestions.map(sugg => `${path.basename(file)}: ${sugg}`));
      } catch (error) {
        issues.push(`${path.basename(file)}: Could not read file - ${(error as Error).message}`);
      }
    }

    return {
      content: [
        {
          type: 'text',
          text: `SQL Analysis Results:\n\nIssues Found (${issues.length}):\n${issues.map(issue => `• ${issue}`).join('\n')}\n\nSuggestions (${suggestions.length}):\n${suggestions.map(sugg => `• ${sugg}`).join('\n')}`,
        },
      ],
    };
  }

  private analyzeSqlQuery(sql: string, dbType: string) {
    const issues: string[] = [];
    const suggestions: string[] = [];

    const upperSql = sql.toUpperCase();

    // Security checks
    if (upperSql.includes('SELECT * FROM')) {
      issues.push('SELECT * used - specify columns explicitly');
    }

    if (upperSql.match(/WHERE\s+.*=\s*['"]?\s*\+/)) {
      issues.push('Potential SQL injection vulnerability - string concatenation in WHERE clause');
    }

    // Performance checks
    if (upperSql.includes('LIKE') && !upperSql.includes('LIKE %')) {
      suggestions.push('Consider using full-text search for better performance on text searches');
    }

    if (upperSql.includes('ORDER BY') && !upperSql.includes('LIMIT')) {
      suggestions.push('ORDER BY without LIMIT may cause performance issues');
    }

    // PostgreSQL specific
    if (dbType === 'postgresql') {
      if (upperSql.includes('ILIKE')) {
        suggestions.push('ILIKE is case-insensitive but may not use indexes efficiently');
      }
    }

    return { issues, suggestions };
  }

  private findSqlFiles(workspacePath: string): string[] {
    const sqlFiles: string[] = [];

    function scanDir(dir: string) {
      try {
        const files = fs.readdirSync(dir);
        for (const file of files) {
          const fullPath = path.join(dir, file);
          const stat = fs.statSync(fullPath);

          if (stat.isDirectory() && !file.startsWith('.') && file !== 'node_modules') {
            scanDir(fullPath);
          } else if (stat.isFile() && (file.endsWith('.sql') || file.includes('migration'))) {
            sqlFiles.push(fullPath);
          }
        }
      } catch (error) {
        // Skip directories we can't read
      }
    }

    scanDir(workspacePath);
    return sqlFiles;
  }

  private async validateDatabaseSchema(args: any) {
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const schemaFile = args.schemaFile
      ? validateString(args.schemaFile, 'schemaFile', 1, 500)
      : null;
    const databaseType = args.databaseType || 'postgresql';

    const issues: string[] = [];
    const suggestions: string[] = [];

    let schemaContent = '';

    if (schemaFile) {
      const fullPath = path.join(workspacePath, schemaFile);
      if (fs.existsSync(fullPath)) {
        schemaContent = fs.readFileSync(fullPath, 'utf8');
      } else {
        issues.push(`Schema file not found: ${schemaFile}`);
      }
    } else {
      // Find schema files
      const schemaFiles = this.findSchemaFiles(workspacePath);
      for (const file of schemaFiles.slice(0, 5)) {
        try {
          schemaContent += fs.readFileSync(file, 'utf8') + '\n';
        } catch (error) {
          issues.push(
            `Could not read schema file ${path.basename(file)}: ${(error as Error).message}`
          );
        }
      }
    }

    if (schemaContent) {
      const validation = this.validateSchemaContent(schemaContent, databaseType);
      issues.push(...validation.issues);
      suggestions.push(...validation.suggestions);
    }

    return {
      content: [
        {
          type: 'text',
          text: `Schema Validation Results:\n\nIssues Found (${issues.length}):\n${issues.map(issue => `• ${issue}`).join('\n')}\n\nSuggestions (${suggestions.length}):\n${suggestions.map(sugg => `• ${sugg}`).join('\n')}`,
        },
      ],
    };
  }

  private validateSchemaContent(schema: string, dbType: string) {
    const issues: string[] = [];
    const suggestions: string[] = [];

    const upperSchema = schema.toUpperCase();

    // Naming conventions
    const tableMatches = schema.match(/CREATE TABLE\s+(\w+)/gi) || [];
    for (const match of tableMatches) {
      const tableName = match.replace(/CREATE TABLE\s+/i, '');
      if (!/^[a-z][a-z0-9_]*$/.test(tableName)) {
        issues.push(`Table name '${tableName}' should use snake_case naming convention`);
      }
    }

    // PostgreSQL specific checks
    if (dbType === 'postgresql') {
      if (
        !upperSchema.includes('SERIAL PRIMARY KEY') &&
        !upperSchema.includes('BIGSERIAL PRIMARY KEY')
      ) {
        suggestions.push('Consider using SERIAL/BIGSERIAL for auto-incrementing primary keys');
      }
    }

    // Foreign key checks
    if (!upperSchema.includes('REFERENCES')) {
      suggestions.push('No foreign key constraints found - consider adding referential integrity');
    }

    return { issues, suggestions };
  }

  private findSchemaFiles(workspacePath: string): string[] {
    const schemaFiles: string[] = [];

    function scanDir(dir: string) {
      try {
        const files = fs.readdirSync(dir);
        for (const file of files) {
          const fullPath = path.join(dir, file);
          const stat = fs.statSync(fullPath);

          if (stat.isDirectory() && !file.startsWith('.') && file !== 'node_modules') {
            scanDir(fullPath);
          } else if (
            stat.isFile() &&
            (file.endsWith('.sql') ||
              file.includes('schema') ||
              file.includes('migration') ||
              file.includes('create'))
          ) {
            schemaFiles.push(fullPath);
          }
        }
      } catch (error) {
        // Skip directories we can't read
      }
    }

    scanDir(workspacePath);
    return schemaFiles;
  }

  private async analyzeIndexes(args: any) {
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const schemaFile = args.schemaFile
      ? validateString(args.schemaFile, 'schemaFile', 1, 500)
      : null;
    const databaseType = args.databaseType || 'postgresql';

    const analysis: string[] = [];

    let schemaContent = '';

    if (schemaFile) {
      const fullPath = path.join(workspacePath, schemaFile);
      if (fs.existsSync(fullPath)) {
        schemaContent = fs.readFileSync(fullPath, 'utf8');
      }
    } else {
      const schemaFiles = this.findSchemaFiles(workspacePath);
      for (const file of schemaFiles.slice(0, 5)) {
        try {
          schemaContent += fs.readFileSync(file, 'utf8') + '\n';
        } catch (error) {
          analysis.push(`Could not read schema file ${path.basename(file)}`);
        }
      }
    }

    if (schemaContent) {
      const indexAnalysis = this.analyzeIndexDefinitions(schemaContent, databaseType);
      analysis.push(...indexAnalysis);
    } else {
      analysis.push('No schema files found to analyze');
    }

    return {
      content: [
        {
          type: 'text',
          text: `Index Analysis Results:\n\n${analysis.map(item => `• ${item}`).join('\n')}`,
        },
      ],
    };
  }

  private analyzeIndexDefinitions(schema: string, dbType: string): string[] {
    const analysis: string[] = [];

    const upperSchema = schema.toUpperCase();

    // Count indexes
    const indexMatches = schema.match(/CREATE INDEX/gi) || [];
    analysis.push(`Found ${indexMatches.length} CREATE INDEX statements`);

    // Check for primary keys
    const pkMatches = schema.match(/PRIMARY KEY/gi) || [];
    analysis.push(`Found ${pkMatches.length} PRIMARY KEY constraints`);

    // PostgreSQL specific
    if (dbType === 'postgresql') {
      if (upperSchema.includes('CREATE INDEX') && !upperSchema.includes('CONCURRENTLY')) {
        analysis.push('Consider using CREATE INDEX CONCURRENTLY for production deployments');
      }

      if (!upperSchema.includes('ANALYZE')) {
        analysis.push('Consider running ANALYZE after bulk data operations to update statistics');
      }
    }

    // General recommendations
    if (indexMatches.length === 0) {
      analysis.push(
        'No explicit indexes found - consider adding indexes on frequently queried columns'
      );
    }

    return analysis;
  }

  private async validateElasticsearchMapping(args: any) {
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const mappingFile = args.mappingFile
      ? validateString(args.mappingFile, 'mappingFile', 1, 500)
      : null;
    const indexName = args.indexName ? validateString(args.indexName, 'indexName', 1, 100) : null;

    const issues: string[] = [];
    const suggestions: string[] = [];

    let mappingContent = '';

    if (mappingFile) {
      const fullPath = path.join(workspacePath, mappingFile);
      if (fs.existsSync(fullPath)) {
        mappingContent = fs.readFileSync(fullPath, 'utf8');
      } else {
        issues.push(`Mapping file not found: ${mappingFile}`);
      }
    } else {
      // Find mapping files
      const mappingFiles = this.findElasticsearchFiles(workspacePath);
      for (const file of mappingFiles.slice(0, 5)) {
        try {
          mappingContent += fs.readFileSync(file, 'utf8') + '\n';
        } catch (error) {
          issues.push(
            `Could not read mapping file ${path.basename(file)}: ${(error as Error).message}`
          );
        }
      }
    }

    if (mappingContent) {
      try {
        const mapping = JSON.parse(mappingContent);
        const validation = this.validateElasticsearchMappingContent(
          mapping,
          indexName || undefined
        );
        issues.push(...validation.issues);
        suggestions.push(...validation.suggestions);
      } catch (error) {
        issues.push(`Invalid JSON in mapping file: ${(error as Error).message}`);
      }
    }

    return {
      content: [
        {
          type: 'text',
          text: `Elasticsearch Mapping Validation:\n\nIssues Found (${issues.length}):\n${issues.map(issue => `• ${issue}`).join('\n')}\n\nSuggestions (${suggestions.length}):\n${suggestions.map(sugg => `• ${sugg}`).join('\n')}`,
        },
      ],
    };
  }

  private validateElasticsearchMappingContent(
    mapping: any,
    indexName?: string
  ): { issues: string[]; suggestions: string[] } {
    const issues: string[] = [];
    const suggestions: string[] = [];

    if (!mapping.mappings) {
      issues.push('Missing mappings section in Elasticsearch mapping');
    }

    if (mapping.mappings && mapping.mappings.properties) {
      const properties = mapping.mappings.properties;

      // Check for text fields without analyzers
      for (const [fieldName, fieldDef] of Object.entries(properties) as [string, any][]) {
        if (fieldDef.type === 'text' && !fieldDef.analyzer) {
          suggestions.push(`Field '${fieldName}' is text type but has no analyzer specified`);
        }

        if (fieldDef.type === 'keyword' && fieldDef.ignore_above === undefined) {
          suggestions.push(`Keyword field '${fieldName}' should specify ignore_above limit`);
        }
      }
    }

    if (indexName && !/^[a-z][a-z0-9_-]*$/.test(indexName)) {
      issues.push(
        `Index name '${indexName}' should follow Elasticsearch naming conventions (lowercase, no special chars except - and _)`
      );
    }

    return { issues, suggestions };
  }

  private findElasticsearchFiles(workspacePath: string): string[] {
    const esFiles: string[] = [];

    function scanDir(dir: string) {
      try {
        const files = fs.readdirSync(dir);
        for (const file of files) {
          const fullPath = path.join(dir, file);
          const stat = fs.statSync(fullPath);

          if (stat.isDirectory() && !file.startsWith('.') && file !== 'node_modules') {
            scanDir(fullPath);
          } else if (
            stat.isFile() &&
            (file.includes('elasticsearch') ||
              file.includes('elastic') ||
              file.includes('mapping') ||
              (file.endsWith('.json') &&
                (file.includes('index') || file.includes('search') || file.includes('es-'))))
          ) {
            esFiles.push(fullPath);
          }
        }
      } catch (error) {
        // Skip directories we can't read
      }
    }

    scanDir(workspacePath);
    return esFiles;
  }

  private async analyzeDatabaseMigrations(args: any) {
    const workspacePath = validateWorkspacePath(args.workspacePath);
    const migrationPath = args.migrationPath
      ? validateString(args.migrationPath, 'migrationPath', 1, 500)
      : null;
    const databaseType = args.databaseType || 'postgresql';

    const analysis: string[] = [];

    const migrationFiles = this.findMigrationFiles(workspacePath, migrationPath || undefined);

    if (migrationFiles.length === 0) {
      analysis.push('No migration files found');
    } else {
      analysis.push(`Found ${migrationFiles.length} migration files`);

      for (const file of migrationFiles.slice(0, 10)) {
        try {
          const content = fs.readFileSync(file, 'utf8');
          const fileAnalysis = this.analyzeMigrationContent(content, databaseType);
          analysis.push(`${path.basename(file)}: ${fileAnalysis.join(', ')}`);
        } catch (error) {
          analysis.push(`${path.basename(file)}: Could not analyze - ${(error as Error).message}`);
        }
      }
    }

    return {
      content: [
        {
          type: 'text',
          text: `Migration Analysis Results:\n\n${analysis.map(item => `• ${item}`).join('\n')}`,
        },
      ],
    };
  }

  private analyzeMigrationContent(content: string, dbType: string): string[] {
    const analysis: string[] = [];

    const upperContent = content.toUpperCase();

    // Check for dangerous operations
    if (upperContent.includes('DROP TABLE') || upperContent.includes('DROP COLUMN')) {
      analysis.push('⚠️  Contains destructive operations - ensure rollback plan exists');
    }

    if (upperContent.includes('DELETE FROM') && !upperContent.includes('WHERE')) {
      analysis.push('⚠️  DELETE without WHERE clause - potential data loss');
    }

    // Check for rollback capability
    if (!upperContent.includes('DOWN') && !content.includes('rollback')) {
      analysis.push('ℹ️  No explicit rollback section found');
    }

    // Performance considerations
    if (upperContent.includes('ALTER TABLE') && upperContent.includes('ADD COLUMN')) {
      analysis.push('ℹ️  Adding column - consider if it can be nullable to avoid table rewrite');
    }

    return analysis;
  }

  private findMigrationFiles(workspacePath: string, migrationPath?: string): string[] {
    const migrationFiles: string[] = [];
    const searchPaths = migrationPath ? [path.join(workspacePath, migrationPath)] : [workspacePath];

    for (const searchPath of searchPaths) {
      function scanDir(dir: string) {
        try {
          const files = fs.readdirSync(dir);
          for (const file of files) {
            const fullPath = path.join(dir, file);
            const stat = fs.statSync(fullPath);

            if (stat.isDirectory() && !file.startsWith('.') && file !== 'node_modules') {
              scanDir(fullPath);
            } else if (
              stat.isFile() &&
              (file.includes('migration') ||
                file.includes('Migration') ||
                /^\d+.*\.sql$/.test(file) ||
                (file.endsWith('.sql') &&
                  (file.includes('up') || file.includes('down') || file.includes('alter'))))
            ) {
              migrationFiles.push(fullPath);
            }
          }
        } catch (error) {
          // Skip directories we can't read
        }
      }

      if (fs.existsSync(searchPath)) {
        scanDir(searchPath);
      }
    }

    return migrationFiles;
  }

  async run() {
    const transport = new StdioServerTransport();
    await this.server.connect(transport);
    console.error('Database MCP server running...');
  }
}

// Start the server
const server = new DatabaseMCPServer();
server.run().catch(console.error);
