import json
import sqlite3
import os
import sys
from pathlib import Path
from typing import Any

async def run_mcp_server():
    """Start the KB MCP server"""
    import asyncio
    from mcp.server.asyncio import AsyncioServer
    from mcp.types import Tool
    
    server = AsyncioServer("kb-mcp-server")
    db_path = Path.cwd() / ".ai" / "kb-index.db"
    
    if not db_path.exists():
        print(f"Error: KB index not found at {db_path}")
        print("Run: python scripts/build_index.py")
        sys.exit(1)
    
    conn = sqlite3.connect(str(db_path))
    conn.row_factory = sqlite3.Row
    
    @server.list_tools()
    async def list_tools() -> list[Tool]:
        return [
            Tool(
                name="search_knowledge_base",
                description="Search KB using full-text search",
                inputSchema={
                    "type": "object",
                    "properties": {
                        "query": {"type": "string", "description": "Search query"},
                        "max_results": {"type": "integer", "default": 5},
                        "category": {"type": "string", "enum": ["tools", "patterns", "architecture", "best-practices"]}
                    },
                    "required": ["query"]
                }
            ),
            Tool(
                name="get_article",
                description="Retrieve article by DocID",
                inputSchema={
                    "type": "object",
                    "properties": {
                        "docid": {"type": "string", "description": "Document ID (KB-053, ADR-001)"},
                        "section": {"type": "string", "description": "Optional section heading"}
                    },
                    "required": ["docid"]
                }
            ),
            Tool(
                name="list_by_category",
                description="List articles in category",
                inputSchema={
                    "type": "object",
                    "properties": {
                        "category": {"type": "string", "enum": ["tools", "patterns", "architecture", "best-practices"]}
                    },
                    "required": ["category"]
                }
            ),
            Tool(
                name="get_quick_reference",
                description="Get quick reference for topic",
                inputSchema={
                    "type": "object",
                    "properties": {
                        "topic": {"type": "string", "description": "Topic name"}
                    },
                    "required": ["topic"]
                }
            )
        ]
    
    @server.call_tool()
    async def call_tool(name: str, arguments: dict) -> Any:
        if name == "search_knowledge_base":
            query = arguments.get("query")
            max_results = arguments.get("max_results", 5)
            category = arguments.get("category")
            
            sql = "SELECT docid, title, summary FROM documents WHERE content LIKE ?"
            params = [f"%{query}%"]
            
            if category:
                sql += " AND category = ?"
                params.append(category)
            
            sql += " LIMIT ?"
            params.append(max_results)
            
            rows = conn.execute(sql, params).fetchall()
            return {"found": len(rows), "results": [dict(r) for r in rows]}
        
        elif name == "get_article":
            docid = arguments.get("docid")
            section = arguments.get("section")
            
            row = conn.execute(
                "SELECT docid, title, content FROM documents WHERE docid = ?",
                [docid]
            ).fetchone()
            
            if not row:
                return {"error": f"Article {docid} not found"}
            
            content = row["content"]
            if section:
                # Extract section
                import re
                match = re.search(f"^## {section}.*?(?=^## |^# |$)", content, re.MULTILINE | re.DOTALL)
                if match:
                    content = match.group(0)
            
            return {
                "docid": row["docid"],
                "title": row["title"],
                "content": content
            }
        
        elif name == "list_by_category":
            category = arguments.get("category")
            rows = conn.execute(
                "SELECT docid, title, summary FROM documents WHERE category = ? ORDER BY docid",
                [category]
            ).fetchall()
            return {"category": category, "found": len(rows), "documents": [dict(r) for r in rows]}
        
        elif name == "get_quick_reference":
            topic = arguments.get("topic")
            rows = conn.execute(
                "SELECT docid, title, summary FROM documents WHERE content LIKE ? LIMIT 3",
                [f"%{topic}%"]
            ).fetchall()
            return {"topic": topic, "results": [dict(r) for r in rows]}
        
        return {"error": f"Unknown tool: {name}"}
    
    async with server:
        print("[KB-MCP] Server started and listening", file=sys.stderr)
        await server.wait()

if __name__ == "__main__":
    import asyncio
    asyncio.run(run_mcp_server())
