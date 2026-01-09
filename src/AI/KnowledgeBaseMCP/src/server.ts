import { Server } from '@modelcontextprotocol/sdk/server/index.js';
import {
  CallToolRequestSchema,
  ErrorCode,
  ListToolsRequestSchema,
  TextContent,
  Tool,
} from '@modelcontextprotocol/sdk/types.js';
import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import Database from 'better-sqlite3';
import * as fs from 'fs';
import * as path from 'path';

const dbPath = path.join(process.cwd(), '.ai', 'kb-index.db');
let db: Database.Database;

// Initialize database connection
function initializeDatabase(): void {
  try {
    db = new Database(dbPath);
    db.pragma('journal_mode = WAL');

    // Verify tables exist
    const tables = db
      .prepare("SELECT name FROM sqlite_master WHERE type='table' AND name='documents'")
      .all();

    if (tables.length === 0) {
      throw new Error('KB index not initialized. Run: npm run index');
    }

    console.error('[KB-MCP] Database connected:', dbPath);
  } catch (error) {
    console.error('[KB-MCP] Database initialization failed:', error);
    process.exit(1);
  }
}

// Tool Definitions
const tools: Tool[] = [
  {
    name: 'search_knowledge_base',
    description:
      'Search knowledge base using semantic/full-text search. Returns most relevant articles.',
    inputSchema: {
      type: 'object' as const,
      properties: {
        query: {
          type: 'string',
          description: 'Search query (natural language or keywords)',
        },
        max_results: {
          type: 'number',
          description: 'Maximum number of results (default: 5)',
          default: 5,
        },
        category: {
          type: 'string',
          enum: ['tools', 'patterns', 'architecture', 'best-practices', 'all'],
          description: 'Filter by category (optional)',
        },
      },
      required: ['query'],
    },
  },
  {
    name: 'get_article',
    description:
      'Retrieve full article by DocID (e.g., KB-053, ADR-001). Optionally extract specific section.',
    inputSchema: {
      type: 'object' as const,
      properties: {
        docid: {
          type: 'string',
          description: 'Document ID (e.g., KB-053, ADR-045)',
        },
        section: {
          type: 'string',
          description: 'Optional section heading to extract',
        },
      },
      required: ['docid'],
    },
  },
  {
    name: 'list_by_category',
    description: 'List all articles in a specific category',
    inputSchema: {
      type: 'object' as const,
      properties: {
        category: {
          type: 'string',
          enum: ['tools', 'patterns', 'architecture', 'best-practices'],
          description: 'Knowledge base category',
        },
      },
      required: ['category'],
    },
  },
  {
    name: 'get_quick_reference',
    description: 'Get quick reference for a topic with key points and links to detailed docs',
    inputSchema: {
      type: 'object' as const,
      properties: {
        topic: {
          type: 'string',
          description: 'Topic name (e.g., mcp-tools, i18n, security, wolverine)',
        },
      },
      required: ['topic'],
    },
  },
  {
    name: 'search_lessons_learned',
    description: 'Search lessons learned by pattern, error, or technology',
    inputSchema: {
      type: 'object' as const,
      properties: {
        pattern: {
          type: 'string',
          description: 'Search pattern (error type, tech stack, issue, or lesson topic)',
        },
        limit: {
          type: 'number',
          description: 'Number of results (default: 3)',
          default: 3,
        },
      },
      required: ['pattern'],
    },
  },
];

// Tool Implementations
async function searchKnowledgeBase(
  query: string,
  maxResults: number = 5,
  category?: string
): Promise<string> {
  try {
    const categoryFilter = category && category !== 'all' ? `AND category = '${category}'` : '';

    const stmt = db.prepare(`
      SELECT docid, title, 
             snippet(kb_search, -1, '...', '...', 64) as excerpt,
             rank
      FROM kb_search
      WHERE kb_search MATCH ?
      ${categoryFilter}
      ORDER BY rank
      LIMIT ?
    `);

    const results = stmt.all(query, maxResults);

    if (results.length === 0) {
      return JSON.stringify({
        found: 0,
        message: 'No matching articles found',
      });
    }

    return JSON.stringify({
      found: results.length,
      results: results.map((r: any) => ({
        docid: r.docid,
        title: r.title,
        excerpt: r.excerpt,
        relevance: Math.round((-r.rank / 100) * 100),
      })),
    });
  } catch (error) {
    return JSON.stringify({ error: String(error) });
  }
}

async function getArticle(docid: string, section?: string): Promise<string> {
  try {
    const doc = db
      .prepare('SELECT docid, title, content, size_kb, updated_at FROM documents WHERE docid = ?')
      .get(docid) as any;

    if (!doc) {
      return JSON.stringify({
        error: `Article ${docid} not found`,
      });
    }

    let content = doc.content;

    if (section) {
      // Extract specific section (heading level 2)
      const regex = new RegExp(`^## ${section}.*?(?=^## |^# |$)`, 'ms');
      const match = content.match(regex);
      content = match ? match[0] : content;
    }

    return JSON.stringify({
      docid: doc.docid,
      title: doc.title,
      section: section || 'full',
      content_preview: content.substring(0, 500),
      full_content: content,
      size_kb: doc.size_kb,
      updated_at: doc.updated_at,
    });
  } catch (error) {
    return JSON.stringify({ error: String(error) });
  }
}

async function listByCategory(category: string): Promise<string> {
  try {
    const docs = db
      .prepare(
        `SELECT docid, title, summary, size_kb 
         FROM documents 
         WHERE category = ? 
         ORDER BY docid`
      )
      .all(category) as any[];

    if (docs.length === 0) {
      return JSON.stringify({
        category,
        found: 0,
        message: 'No articles in this category',
      });
    }

    return JSON.stringify({
      category,
      found: docs.length,
      documents: docs,
    });
  } catch (error) {
    return JSON.stringify({ error: String(error) });
  }
}

async function getQuickReference(topic: string): Promise<string> {
  try {
    // Search for articles about this topic
    const results = db
      .prepare(
        `SELECT docid, title, summary 
         FROM documents 
         WHERE content LIKE ? OR tags LIKE ?
         LIMIT 3`
      )
      .all(`%${topic}%`, `%${topic}%`) as any[];

    if (results.length === 0) {
      return JSON.stringify({
        topic,
        message: 'No quick reference found for this topic',
      });
    }

    return JSON.stringify({
      topic,
      quick_points: results.map(r => ({
        docid: r.docid,
        title: r.title,
        summary: r.summary,
      })),
      full_docs: results.map(r => r.docid),
    });
  } catch (error) {
    return JSON.stringify({ error: String(error) });
  }
}

async function searchLessonsLearned(pattern: string, limit: number = 3): Promise<string> {
  try {
    // Search in KB-LESSONS and similar documents
    const results = db
      .prepare(
        `SELECT docid, title, snippet(kb_search, -1, '...', '...', 100) as excerpt
         FROM kb_search
         WHERE (docid LIKE 'KB-LESSONS%' OR docid LIKE 'KB-050%')
         AND kb_search MATCH ?
         LIMIT ?`
      )
      .all(pattern, limit) as any[];

    if (results.length === 0) {
      return JSON.stringify({
        pattern,
        found: 0,
        message: 'No matching lessons found',
      });
    }

    return JSON.stringify({
      pattern,
      found: results.length,
      lessons: results,
    });
  } catch (error) {
    return JSON.stringify({ error: String(error) });
  }
}

// Create server
const server = new Server(
  {
    name: 'kb-mcp-server',
    version: '1.0.0',
  },
  {
    capabilities: {
      tools: {},
    },
  }
);

// Handlers
server.setRequestHandler(ListToolsRequestSchema, async () => ({
  tools,
}));

server.setRequestHandler(CallToolRequestSchema, async request => {
  const { name, arguments: args } = request.params;

  if (name === 'search_knowledge_base') {
    const result = await searchKnowledgeBase(
      args.query as string,
      (args.max_results as number) || 5,
      args.category as string | undefined
    );
    return {
      content: [{ type: 'text' as const, text: result }],
    };
  }

  if (name === 'get_article') {
    const result = await getArticle(args.docid as string, args.section as string | undefined);
    return {
      content: [{ type: 'text' as const, text: result }],
    };
  }

  if (name === 'list_by_category') {
    const result = await listByCategory(args.category as string);
    return {
      content: [{ type: 'text' as const, text: result }],
    };
  }

  if (name === 'get_quick_reference') {
    const result = await getQuickReference(args.topic as string);
    return {
      content: [{ type: 'text' as const, text: result }],
    };
  }

  if (name === 'search_lessons_learned') {
    const result = await searchLessonsLearned(args.pattern as string, (args.limit as number) || 3);
    return {
      content: [{ type: 'text' as const, text: result }],
    };
  }

  return {
    content: [{ type: 'text' as const, text: `Unknown tool: ${name}` }],
    isError: true,
  };
});

// Main
async function main() {
  initializeDatabase();

  const transport = new StdioServerTransport();
  await server.connect(transport);

  console.error('[KB-MCP] Server started and listening on stdio');
}

main().catch(console.error);
