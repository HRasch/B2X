#!/bin/bash
# B2X Log Parsing and Indexing Script
# Parses application logs and indexes them into Elasticsearch

set -e

# Configuration
ELASTICSEARCH_URL="http://localhost:9200"
LOG_DIR="/Users/holger/Documents/Projekte/B2X/logs"
INDEX_NAME="B2X-logs-$(date +%Y-%m-%d)"

# Create index if it doesn't exist
create_index() {
    curl -X PUT "$ELASTICSEARCH_URL/$INDEX_NAME" \
        -H 'Content-Type: application/json' \
        -d '{
            "mappings": {
                "properties": {
                    "@timestamp": { "type": "date" },
                    "level": { "type": "keyword" },
                    "service": { "type": "keyword" },
                    "message": { "type": "text" },
                    "exception": { "type": "text" },
                    "tenant_id": { "type": "keyword" },
                    "correlation_id": { "type": "keyword" }
                }
            }
        }'
}

# Parse and index logs
parse_logs() {
    find "$LOG_DIR" -name "*.log" -type f -mtime -1 | while read -r log_file; do
        service_name=$(basename "$log_file" .log)

        while IFS= read -r line; do
            # Parse structured log line (assuming Serilog format)
            if [[ $line =~ ^([0-9]{4}-[0-9]{2}-[0-9]{2}\ [0-9]{2}:[0-9]{2}:[0-9]{2}\.[0-9]+)\ \[([A-Z]+)\]\ ([^:]+):\ (.+)$ ]]; then
                timestamp="${BASH_REMATCH[1]}"
                level="${BASH_REMATCH[2]}"
                source="${BASH_REMATCH[3]}"
                message="${BASH_REMATCH[4]}"

                # Extract additional fields
                tenant_id=$(echo "$message" | grep -oP 'tenant_id["\s]*:[\s]*"\K[^"]*' || echo "")
                correlation_id=$(echo "$message" | grep -oP 'correlation_id["\s]*:[\s]*"\K[^"]*' || echo "")
                exception=$(echo "$message" | grep -oP 'Exception: \K.+' || echo "")

                # Index document
                curl -X POST "$ELASTICSEARCH_URL/$INDEX_NAME/_doc" \
                    -H 'Content-Type: application/json' \
                    -d "{
                        \"@timestamp\": \"$timestamp\",
                        \"level\": \"$level\",
                        \"service\": \"$service_name\",
                        \"source\": \"$source\",
                        \"message\": \"$message\",
                        \"exception\": \"$exception\",
                        \"tenant_id\": \"$tenant_id\",
                        \"correlation_id\": \"$correlation_id\"
                    }"
            fi
        done < "$log_file"
    done
}

# Main execution
echo "Creating Elasticsearch index..."
create_index

echo "Parsing and indexing logs..."
parse_logs

echo "Log parsing completed successfully."