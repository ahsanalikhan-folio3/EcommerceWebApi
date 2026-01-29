#!/bin/sh
# wait-for-sql.sh
set -e

host="$1"
shift

until /opt/mssql-tools/bin/sqlcmd -S "$host" -U "$MSSQL_SA_USER" -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" > /dev/null 2>&1
do
  echo "Waiting for SQL Server at $host..."
  sleep 5
done

exec "$@"
