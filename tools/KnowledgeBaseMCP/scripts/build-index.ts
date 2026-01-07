import Database from 'better-sqlite3';
import * as fs from 'fs';
import * as path from 'path';
import { glob } from 'glob';

const dbPath = path.join(process.cwd(), '.ai', 'kb-index.db');
const kbPath = path.join(process.cwd(), '.ai', 'knowledgebase');

console.log(`📚 Building KB Index from: ${kbPath}`);

// Create/Initialize database
let db = new Database(dbPath);
db.pragma('journal_mode = WAL');

// Drop existing tables (rebuild)
db.exec('DROP TABLE IF EXISTS kb_search');
db.exec('DROP TABLE IF EXISTS documents');

// Create tables
db.exec(`
  CREATE TABLE documents (
    docid TEXT PRIMARY KEY,
    title TEXT NOT NULL,
    category TEXT,
    content TEXT,
    summary TEXT,
    tags TEXT,
    size_kb REAL,
    updated_at TIMESTAMP,
    file_path TEXT
  );

  CREATE VIRTUAL TABLE kb_search USING fts5(
    docid UNINDEXED,
    title,
    content,
    tags
  );

  CREATE INDEX idx_category ON documents(category);
  CREATE INDEX idx_updated ON documents(updated_at DESC);
`);

// Parse markdown file
function parseMarkdown(filePath: string): {
  docid: string;
  title: string;
  category: string;
  content: string;
  summary: string;
  tags: string;
} {
  const content = fs.readFileSync(filePath, 'utf-8');

  // Extract front matter
  const frontMatterMatch = content.match(/^---\n([\s\S]*?)\n---/);
  const frontMatter: Record<string, string> = {};

  if (frontMatterMatch) {
    const lines = frontMatterMatch[1].split('\n');
    lines.forEach(line => {
      const [key, ...valueParts] = line.split(':');
      if (key && valueParts.length > 0) {
        frontMatter[key.trim()] = valueParts.join(':').trim();
      }
    });
  }

  // Extract title from first heading
  const titleMatch = content.match(/^# (.+?)$/m);
  const title = titleMatch ? titleMatch[1] : path.basename(filePath, '.md');

  // Determine category from path
  let category = 'general';
  if (filePath.includes('tools-and-tech')) category = 'tools';
  if (filePath.includes('best-practices')) category = 'best-practices';
  if (filePath.includes('architecture')) category = 'architecture';
  if (filePath.includes('patterns')) category = 'patterns';

  const summary = content
    .replace(/^---[\s\S]*?---/, '')
    .substring(0, 200)
    .replace(/[#\*\[\]]/g, '');

  const tags = JSON.stringify([
    category,
    ...(frontMatter.tags ? frontMatter.tags.split(',').map(t => t.trim()) : []),
  ]);

  // Extract docid from front matter or filename
  let docid = frontMatter.docid;
  if (!docid) {
    const filenameMatch = path.basename(filePath).match(/^([A-Z]+-\d+)/);
    docid = filenameMatch ? filenameMatch[1] : `KB-${Date.now()}`;
  }

  return {
    docid,
    title,
    category,
    content,
    summary: summary.trim(),
    tags,
  };
}

// Index all markdown files
let count = 0;
const files = glob.sync(`${kbPath}/**/*.md`);

console.log(`Found ${files.length} markdown files\n`);

const insertStmt = db.prepare(`
  INSERT INTO documents (docid, title, category, content, summary, tags, size_kb, updated_at, file_path)
  VALUES (?, ?, ?, ?, ?, ?, ?, datetime('now'), ?)
`);

const insertFts = db.prepare(`
  INSERT INTO kb_search (docid, title, content, tags)
  VALUES (?, ?, ?, ?)
`);

db.exec('BEGIN TRANSACTION');

files.forEach(file => {
  try {
    const parsed = parseMarkdown(file);
    const stats = fs.statSync(file);

    insertStmt.run(
      parsed.docid,
      parsed.title,
      parsed.category,
      parsed.content,
      parsed.summary,
      parsed.tags,
      Math.round((stats.size / 1024) * 100) / 100,
      file
    );

    insertFts.run(parsed.docid, parsed.title, parsed.content, parsed.tags);

    console.log(`✓ ${parsed.docid}: ${parsed.title}`);
    count++;
  } catch (error) {
    console.error(`✗ Error indexing ${file}:`, error);
  }
});

db.exec('COMMIT');

console.log(`\n✅ Indexed ${count} documents`);
console.log(`📁 Database: ${dbPath}`);

// Show stats
const stats = db.prepare('SELECT COUNT(*) as total FROM documents').get() as any;
const byCat = db
  .prepare('SELECT category, COUNT(*) as count FROM documents GROUP BY category')
  .all() as any[];

console.log(`\n📊 Statistics:`);
console.log(`   Total documents: ${stats.total}`);
console.log(`   By category:`);
byCat.forEach((row: any) => {
  console.log(`   - ${row.category}: ${row.count}`);
});

db.close();
console.log('\n✨ KB Index ready!');
