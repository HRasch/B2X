#!/usr/bin/env python3
"""Build Knowledge Base SQLite index from markdown files"""

import sqlite3
import os
from pathlib import Path
import re
from typing import Dict, Tuple

KB_PATH = Path(".ai/knowledgebase")
DB_PATH = Path(".ai/kb-index.db")

def parse_markdown(filepath: Path) -> Dict[str, str]:
    """Parse markdown file and extract metadata"""
    content = filepath.read_text(encoding="utf-8")
    
    # Extract front matter
    frontmatter = {}
    match = re.match(r"^---\n(.*?)\n---", content, re.DOTALL)
    if match:
        for line in match.group(1).split("\n"):
            if ":" in line:
                key, value = line.split(":", 1)
                frontmatter[key.strip()] = value.strip()
    
    # Extract title
    title_match = re.search(r"^# (.+)$", content, re.MULTILINE)
    title = title_match.group(1) if title_match else filepath.stem
    
    # Determine category from path
    category = "general"
    if "tools-and-tech" in str(filepath):
        category = "tools"
    elif "best-practices" in str(filepath):
        category = "best-practices"
    elif "architecture" in str(filepath):
        category = "architecture"
    elif "patterns" in str(filepath):
        category = "patterns"
    
    # Extract summary
    summary = re.sub(r"^---.*?---", "", content, flags=re.DOTALL)
    summary = re.sub(r"[#\*\[\]]", "", summary)[:200].strip()
    
    # DocID from frontmatter or filename
    docid = frontmatter.get("docid")
    if not docid:
        filename_match = re.match(r"([A-Z]+-\d+)", filepath.stem)
        docid = filename_match.group(1) if filename_match else f"KB-{filepath.stem}"
    
    return {
        "docid": docid,
        "title": title,
        "category": category,
        "content": content,
        "summary": summary,
        "tags": str(frontmatter.get("tags", category)).split(","),
        "file_path": str(filepath),
        "size_kb": filepath.stat().st_size / 1024
    }

def main():
    print(f"📚 Building KB Index from: {KB_PATH}")
    
    if not KB_PATH.exists():
        print(f"❌ KB path not found: {KB_PATH}")
        return
    
    # Create/Initialize database
    if DB_PATH.exists():
        DB_PATH.unlink()
    
    conn = sqlite3.connect(str(DB_PATH))
    c = conn.cursor()
    
    # Create tables
    c.execute("""
        CREATE TABLE documents (
            docid TEXT PRIMARY KEY,
            title TEXT NOT NULL,
            category TEXT,
            content TEXT,
            summary TEXT,
            tags TEXT,
            size_kb REAL,
            file_path TEXT
        )
    """)
    
    c.execute("""
        CREATE VIRTUAL TABLE kb_search USING fts5(
            docid UNINDEXED,
            title,
            content,
            tags
        )
    """)
    
    c.execute("CREATE INDEX idx_category ON documents(category)")
    
    # Index all markdown files
    files = list(KB_PATH.rglob("*.md"))
    print(f"Found {len(files)} markdown files\n")
    
    count = 0
    for filepath in sorted(files):
        try:
            doc = parse_markdown(filepath)
            
            c.execute("""
                INSERT INTO documents (docid, title, category, content, summary, tags, size_kb, file_path)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?)
            """, (
                doc["docid"],
                doc["title"],
                doc["category"],
                doc["content"],
                doc["summary"],
                ",".join(doc["tags"]),
                doc["size_kb"],
                doc["file_path"]
            ))
            
            c.execute("""
                INSERT INTO kb_search (docid, title, content, tags)
                VALUES (?, ?, ?, ?)
            """, (
                doc["docid"],
                doc["title"],
                doc["content"],
                ",".join(doc["tags"])
            ))
            
            print(f"✓ {doc['docid']}: {doc['title']}")
            count += 1
        except Exception as e:
            print(f"✗ Error indexing {filepath}: {e}")
    
    conn.commit()
    conn.close()
    
    print(f"\n✅ Indexed {count} documents")
    print(f"📁 Database: {DB_PATH}")
    print(f"✨ KB Index ready!")

if __name__ == "__main__":
    main()
