#!/bin/bash

LOG_FILE="/tmp/B2X-port-test.log"
echo "=== Port Lifecycle Test $(date) ===" > "$LOG_FILE"

echo "📊 Test 1: Welche Prozesse blockieren Port 8080 JETZT?"
lsof -i :8080 2>&1 | tee -a "$LOG_FILE"

echo ""
echo "📊 Test 2: Alle DCP Prozesse JETZT:"
ps aux | grep -E "dcpctrl|dcpproc" | grep -v grep | tee -a "$LOG_FILE"

echo ""
echo "🛑 Test 3: Führe kill-all-services.sh aus..."
/Users/holger/Documents/Projekte/B2X/scripts/kill-all-services.sh 2>&1 | tee -a "$LOG_FILE"

echo ""
echo "⏱️  Test 4: Warte 3 Sekunden..."
sleep 3

echo ""
echo "📊 Test 5: Port 8080 nach Kill:"
lsof -i :8080 2>&1 | tee -a "$LOG_FILE"
if [ $? -eq 0 ]; then
    echo "❌ FEHLER: Port 8080 ist IMMER NOCH belegt!" | tee -a "$LOG_FILE"
else
    echo "✅ Port 8080 ist frei" | tee -a "$LOG_FILE"
fi

echo ""
echo "📊 Test 6: DCP Prozesse nach Kill:"
DCP_COUNT=$(ps aux | grep -E "dcpctrl|dcpproc" | grep -v grep | wc -l | tr -d ' ')
if [ "$DCP_COUNT" -gt 0 ]; then
    echo "❌ FEHLER: $DCP_COUNT DCP Prozesse laufen noch!" | tee -a "$LOG_FILE"
    ps aux | grep -E "dcpctrl|dcpproc" | grep -v grep | tee -a "$LOG_FILE"
else
    echo "✅ Keine DCP Prozesse mehr" | tee -a "$LOG_FILE"
fi

echo ""
echo "📊 Test 7: Starte Aspire manuell und beobachte..."
echo "Führe aus: cd backend/AppHost && dotnet run" | tee -a "$LOG_FILE"
echo "Dann prüfe Port 8080 während des Starts:" | tee -a "$LOG_FILE"

echo ""
echo "📝 Log gespeichert in: $LOG_FILE"
echo ""
echo "🔍 Nächste Schritte:"
echo "1. Prüfe das Log: cat $LOG_FILE"
echo "2. Starte Aspire manuell: cd backend/AppHost && dotnet run"
echo "3. Während des Starts (in neuem Terminal): watch -n 0.5 'lsof -i :8080'"
