#!/bin/bash

TEST_LOG="/tmp/aspire-startup-test.log"
echo "=== Aspire Startup Test $(date) ===" > "$TEST_LOG"

echo "📊 Test: Aspire Startup und Port 8080 Timing"
echo ""

# 1. Kill alle Services
echo "🛑 Schritt 1: Stoppe alle Services..."
/Users/holger/Documents/Projekte/B2Connect/scripts/kill-all-services.sh >> "$TEST_LOG" 2>&1
sleep 2

# 2. Verifiziere Port ist frei
echo "📊 Schritt 2: Prüfe Port 8080 ist frei..."
if lsof -i :8080 >/dev/null 2>&1; then
    echo "❌ Port 8080 ist IMMER NOCH belegt nach Kill!" | tee -a "$TEST_LOG"
    lsof -i :8080 | tee -a "$TEST_LOG"
    exit 1
else
    echo "✅ Port 8080 ist frei" | tee -a "$TEST_LOG"
fi

# 3. Starte Port Monitoring im Hintergrund
echo "📊 Schritt 3: Starte Port Monitoring..."
(
    while true; do
        if lsof -i :8080 >/dev/null 2>&1; then
            TIMESTAMP=$(date +%H:%M:%S.%3N)
            echo "[$TIMESTAMP] Port 8080 BELEGT" | tee -a "$TEST_LOG"
            lsof -i :8080 | tee -a "$TEST_LOG"
            sleep 0.5
        else
            sleep 0.1
        fi
    done
) &
MONITOR_PID=$!

# 4. Starte Aspire
echo "🚀 Schritt 4: Starte Aspire Orchestration..."
echo "[$( date +%H:%M:%S.%3N)] START: dotnet build" | tee -a "$TEST_LOG"

cd /Users/holger/Documents/Projekte/B2Connect/backend/Orchestration

dotnet build B2Connect.Orchestration.csproj >> "$TEST_LOG" 2>&1
BUILD_EXIT=$?

echo "[$( date +%H:%M:%S.%3N)] BUILD FERTIG (Exit: $BUILD_EXIT)" | tee -a "$TEST_LOG"

if [ $BUILD_EXIT -ne 0 ]; then
    echo "❌ Build fehlgeschlagen!" | tee -a "$TEST_LOG"
    kill $MONITOR_PID
    exit 1
fi

echo "[$( date +%H:%M:%S.%3N)] START: dotnet run" | tee -a "$TEST_LOG"

# Starte Aspire im Hintergrund
dotnet run --no-build >> "$TEST_LOG" 2>&1 &
ASPIRE_PID=$!

echo "🔍 Aspire gestartet (PID: $ASPIRE_PID)"
echo "   Monitoring Port 8080 für 10 Sekunden..."

# Warte 10 Sekunden und beobachte
sleep 10

# Stoppe Monitoring
kill $MONITOR_PID 2>/dev/null

# Stoppe Aspire
echo "🛑 Stoppe Aspire..."
kill $ASPIRE_PID 2>/dev/null
sleep 2

/Users/holger/Documents/Projekte/B2Connect/scripts/kill-all-services.sh >> "$TEST_LOG" 2>&1

echo ""
echo "📝 Vollständiges Log: cat $TEST_LOG"
echo ""
echo "🔍 Analyse:"
grep "Port 8080 BELEGT" "$TEST_LOG" | head -5
