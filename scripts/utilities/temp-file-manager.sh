#!/bin/bash
# Temp-File Manager for Token Optimization
# Usage: ./scripts/temp-file-manager.sh <command> [args]
# Commands: create <content> <extension>, list, cleanup, cleanup-all

set -e

TEMP_DIR=".ai/temp"
mkdir -p "$TEMP_DIR"

case "$1" in
    create)
        if [ $# -lt 3 ]; then
            echo "Usage: $0 create <content> <extension>"
            exit 1
        fi
        CONTENT="$2"
        EXT="$3"
        UUID=$(uuidgen | tr '[:upper:]' '[:lower:]' | cut -d'-' -f1)
        FILENAME="task-${UUID}.${EXT}"
        FILEPATH="${TEMP_DIR}/${FILENAME}"
        echo "$CONTENT" > "$FILEPATH"
        echo "Created temp file: $FILEPATH"
        echo "$FILEPATH"
        ;;
    list)
        echo "Temp files in $TEMP_DIR:"
        ls -la "$TEMP_DIR" 2>/dev/null || echo "No temp files found."
        ;;
    cleanup)
        if [ $# -lt 2 ]; then
            echo "Usage: $0 cleanup <filename>"
            exit 1
        fi
        FILE="$2"
        if [ -f "${TEMP_DIR}/${FILE}" ]; then
            rm "${TEMP_DIR}/${FILE}"
            echo "Cleaned up: ${TEMP_DIR}/${FILE}"
        else
            echo "File not found: ${TEMP_DIR}/${FILE}"
        fi
        ;;
    cleanup-all)
        rm -rf "$TEMP_DIR"/*
        echo "Cleaned up all temp files in $TEMP_DIR"
        ;;
    *)
        echo "Usage: $0 {create|list|cleanup|cleanup-all}"
        exit 1
        ;;
esac