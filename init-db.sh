#!/bin/bash
set -e

echo "Starting SQL Server..."
/opt/mssql/bin/sqlservr &

echo "Waiting for SQL Server to be ready..."
# Wait for the SQL Server process to write the ready message to the error log (avoids requiring sqlcmd inside this container)
until grep -q "SQL Server is now ready for client connections" /var/opt/mssql/log/errorlog > /dev/null 2>&1; do
  sleep 2
done

echo "SQL Server is ready."

echo "Backup directory contents:"
ls -la /backup || true

# Find a .bak file in /backup (pick the first if multiple)
BACKUP_FILE=$(ls /backup/*.bak 2>/dev/null | head -n1 || true)
if [ -z "$BACKUP_FILE" ]; then
  echo "No .bak file found in /backup, skipping restore."
else
  echo "Found backup: $BACKUP_FILE"
  echo "Running restore..."
  # If sqlcmd is available inside the container, run the restore here; otherwise, expect the separate dbrestore service to handle it
  if command -v /opt/mssql-tools/bin/sqlcmd > /dev/null 2>&1; then
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -Q "RESTORE DATABASE EcommerceWebApiDb FROM DISK = '$BACKUP_FILE' WITH REPLACE" 2>&1 | tee /var/opt/mssql/log/restore.log || {
      echo "Restore command failed. See /var/opt/mssql/log/restore.log for details"
    }
  else
    echo "/opt/mssql-tools/bin/sqlcmd not found inside container; skipping in-container restore. The 'dbrestore' service will attempt the restore after SQL becomes healthy."
  fi
fi

wait
