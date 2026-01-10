#!/bin/bash

echo "🔍 Monitoring Port 8080 in Echtzeit..."
echo "Drücke Ctrl+C zum Beenden"
echo ""

while true; do
    PORT_STATUS=$(lsof -i :8080 2>/dev/null)
    
    if [ -n "$PORT_STATUS" ]; then
        clear
        echo "🔴 Port 8080 BELEGT - $(date +%H:%M:%S.%3N)"
        echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
        echo "$PORT_STATUS"
        echo ""
        echo "PID Details:"
        PID=$(echo "$PORT_STATUS" | tail -1 | awk '{print $2}')
        ps -p $PID -o pid,ppid,command 2>/dev/null
    else
        clear
        echo "✅ Port 8080 FREI - $(date +%H:%M:%S.%3N)"
    fi
    
    sleep 0.2
done
