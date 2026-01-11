#!/bin/bash

echo "=== Aspire Doppelstart Test ==="
echo "Startet Aspire Services mehrfach?"
echo ""

# 1. Cleanup
echo "1️⃣ Cleanup..."
/Users/holger/Documents/Projekte/B2X/scripts/kill-all-services.sh > /dev/null 2>&1
sleep 2

# 2. Starte Aspire im Hintergrund
echo "2️⃣ Starte Aspire..."
cd /Users/holger/Documents/Projekte/B2X/backend/AppHost
dotnet run > /tmp/aspire-output.log 2>&1 &
ASPIRE_PID=$!
echo "   Aspire gestartet (PID: $ASPIRE_PID)"

# 3. Warte 15 Sekunden auf vollständigen Start
echo "3️⃣ Warte 15 Sekunden auf Service-Start..."
for i in {1..15}; do
    echo -n "."
    sleep 1
done
echo ""

# 4. Zähle Prozesse pro Port
echo ""
echo "4️⃣ Prozesse pro Port:"
echo ""

for PORT in 5173 5174 7002 7003 7004 7005 7008 8000 8080 15500; do
    COUNT=$(lsof -i :$PORT 2>/dev/null | grep LISTEN | wc -l | tr -d ' ')
    PROCESSES=$(lsof -i :$PORT 2>/dev/null | grep LISTEN | awk '{print $1}' | sort | uniq)
    
    if [ "$COUNT" -gt 1 ]; then
        echo "❌ Port $PORT: $COUNT Prozesse (DOPPELT!)"
        lsof -i :$PORT 2>/dev/null | grep LISTEN
    elif [ "$COUNT" -eq 1 ]; then
        echo "✅ Port $PORT: 1 Prozess ($PROCESSES)"
    else
        echo "⚪ Port $PORT: Nicht belegt"
    fi
done

# 5. Detaillierte Prozess-Liste
echo ""
echo "5️⃣ Alle laufenden B2X Prozesse:"
ps aux | grep -E "B2X|dcpctrl|vite|node.*frontend" | grep -v grep | awk '{print $2, $11, $12, $13, $14}' | head -20

# 6. Cleanup
echo ""
echo "6️⃣ Stoppe Aspire..."
kill $ASPIRE_PID 2>/dev/null
sleep 2
/Users/holger/Documents/Projekte/B2X/scripts/kill-all-services.sh > /dev/null 2>&1

echo ""
echo "📝 Aspire Log: cat /tmp/aspire-output.log"
