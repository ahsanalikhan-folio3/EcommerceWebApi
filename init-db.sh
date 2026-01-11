#!/bin/bash
set -e

echo "Starting SQL Server..."
/opt/mssql/bin/sqlservr &

echo "Waiting for SQL Server to be ready..."
until /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -Q "SELECT 1" > /dev/null 2>&1
do
  sleep 2
done

echo "SQL Server is ready."

echo "Running restore (if needed)..."
/opt/mssql-tools/bin/sqlcmd \
  -S localhost \
  -U SA \
  -P "$SA_PASSWORD" \
  -i /backup/restore.sql

wait
