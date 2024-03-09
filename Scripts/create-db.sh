#!/bin/bash

DATABASE_FILE="$1"

if [[ -z "$DATABASE_FILE" ]]; then
    echo "Usage: $0 DATABASE_FILE"
    exit 1
fi

if [[ -f "$DATABASE_FILE" ]]; then
    echo "$DATABASE_FILE already exists"
    exit 1
fi

sqlite3 "$DATABASE_FILE" << EOF
CREATE TABLE users(
    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    email TEXT NOT NULL,
    name TEXT NOT NULL);
EOF
