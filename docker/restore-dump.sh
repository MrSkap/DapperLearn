#!/bin/bash

# Parse arguments
while getopts ":c:U:d:f:" opt; do
  case $opt in
    c) CONTAINER="$OPTARG" ;;
    U) DB_USER="$OPTARG" ;;
    d) DB_NAME="$OPTARG" ;;
    f) DUMP_FILE="$OPTARG" ;;
    *) echo "Invalid option"; exit 1 ;;
  esac
done

# Set defaults
CONTAINER=${CONTAINER:-"postgres"}
DB_USER=${DB_USER:-"user"}
DB_NAME=${DB_NAME:-"zoo_db"}

# Check required parameters
if [ -z "$DUMP_FILE" ]; then
    echo "Dump file not specified"
    exit 1
fi

# Execute restoration in container
echo "Restoring to container $CONTAINER..."
docker exec -i "$CONTAINER" psql -U "$DB_USER" -d "$DB_NAME" < "$DUMP_FILE"

if [ $? -eq 0 ]; then
    echo "Database restored successfully"
else
    echo "Restore failed"
    exit 1
fi