#!/usr/bin/env bash

# B2X Quick Start Script

set -euo pipefail

echo "🚀 B2X Projekt wird gestartet..."
echo ""

# Frontend starten
cd "$(dirname "$0")/../src/Store"

echo "📦 Frontend Dependencies werden überprüft..."
if [ ! -d "node_modules" ]; then
    echo "   npm install wird ausgeführt..."
    npm install
fi

echo ""
echo "✨ Frontend Dev Server wird gestartet..."
echo "   http://localhost:3000"
echo ""
echo "💡 Tipp: Drücke CTRL+C um den Server zu beenden"
echo ""

npm run dev
