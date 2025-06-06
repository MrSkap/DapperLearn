#!/bin/bash

# Parse arguments
while getopts ":c:U:d:o:" opt; do
  case $opt in
    c) CONTAINER="$OPTARG" ;;
    U) DB_USER="$OPTARG" ;;
    d) DB_NAME="$OPTARG" ;;
    o) DUMP_DIR="$OPTARG" ;;
    *) echo "Invalid option"; exit 1 ;;
  esac
done

# Set defaults
CONTAINER=${CONTAINER:-"postgres"}
DB_USER=${DB_USER:-"user"}
DB_NAME=${DB_NAME:-"zoo_db"}
DUMP_DIR=${DUMP_DIR:-"./backups"}

# Create backup directory
mkdir -p "$DUMP_DIR"

# Backup filename
DUMP_FILE="pg_dump_${DB_NAME}_$(date +%Y-%m-%d_%H-%M-%S).sql"

# Execute backup in container
echo "Creating backup from container $CONTAINER..."
docker exec -i "$CONTAINER" pg_dump -U "$DB_USER" -d "$DB_NAME" > "${DUMP_DIR}/${DUMP_FILE}"

if [ $? -eq 0 ]; then
    echo "Backup created: ${DUMP_DIR}/${DUMP_FILE}"
else
    echo "Backup failed"
    exit 1
fi