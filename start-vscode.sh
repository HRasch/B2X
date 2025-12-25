#!/bin/bash

# B2Connect Port Cleanup & Start Script

echo "🔧 B2Connect - Port Überprüfung & Startup"
echo "==========================================="
echo ""

# Funktion zum Killen von Prozessen auf bestimmtem Port
kill_port() {
    local port=$1
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1 ; then
        echo "⚠️  Port $port ist belegt, Prozess wird beendet..."
        lsof -ti:$port | xargs kill -9 2>/dev/null || true
        sleep 1
    fi
}

# Überprüfe kritische Ports
echo "📡 Überprüfe Ports..."
for port in 8080 9000; do
    if lsof -Pi :$port -sTCP:LISTEN -t >/dev/null 2>&1 ; then
        echo "   🔄 Port $port wird freigegeben..."
        kill_port $port
    fi
done

echo ""
echo "✅ Ports sind jetzt frei"
echo ""
echo "🚀 Starte das Projekt..."
echo "   Frontend: http://localhost:3000"
echo "   Aspire Dashboard: http://localhost:15500"
echo ""

cd "$(dirname "$0")"
exec code .
