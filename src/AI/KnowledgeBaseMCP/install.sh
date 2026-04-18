#!/bin/bash

set -e

echo "🚀 Installing Knowledge Base MCP Server..."
echo ""

cd tools/KnowledgeBaseMCP

echo "📦 Installing dependencies..."
npm install

echo ""
echo "🔨 Building TypeScript..."
npm run build

echo ""
echo "📚 Building Knowledge Base Index..."
npm run index

echo ""
echo "✅ Installation complete!"
echo ""
echo "Next steps:"
echo "1. Restart VS Code"
echo "2. Check MCP Console for 'kb-mcp' server status"
echo ""
echo "Test the server:"
echo "  cd tools/KnowledgeBaseMCP"
echo "  npm start"
echo ""
