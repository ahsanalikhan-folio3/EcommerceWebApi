#!/bin/bash
set -euo pipefail

LOG=/restore.log
exec > >(tee -a "$LOG") 2>&1

echo "dbrestore started at $(date)"

# Wait for a backup file to appear (30 attempts)
BACKUP=""
for i in $(seq 1 30); do
  BACKUP=$(ls /backup/*.bak 2>/dev/null | head -n1 || true)
  if [ -n "$BACKUP" ]; then break; fi
  echo "Waiting for backup... ($i/30)"; sleep 2
done
if [ -z "$BACKUP" ]; then
  echo "No backup found, exiting with error"
  exit 1
fi

echo "Found backup: $BACKUP"

# Wait for SQL Server to accept connections (60 attempts)
for i in $(seq 1 60); do
  /opt/mssql-tools/bin/sqlcmd -S sqlserver -U SA -P 'Strong!Pass123' -Q 'SELECT 1' >/dev/null 2>&1 && break
  echo "Waiting for SQL to accept connections... ($i/60)"; sleep 2
done

# Try simple restore first
echo "Attempting simple restore..."
# Use -b so sqlcmd exits non-zero on errors
if /opt/mssql-tools/bin/sqlcmd -b -S sqlserver -U SA -P 'Strong!Pass123' -Q "RESTORE DATABASE EcommerceWebApiDb FROM DISK = '$BACKUP' WITH REPLACE" 2>&1 | tee -a "$LOG"; then
  echo "Simple restore succeeded"
  exit 0
fi

# Check log for incompatibility error and give a clear message
if tail -n 20 "$LOG" | grep -i "incompatible" >/dev/null 2>&1; then
  echo "ERROR: Backup was created on a newer SQL Server version and is incompatible with this server."
  echo "Please use a SQL Server image that supports the backup (or recreate the backup on a compatible server)."
  exit 6
fi

echo "Simple restore failed, attempting restore with MOVE..."

# Get logical file names (use -b so failures are detected)
FILELIST=$(/opt/mssql-tools/bin/sqlcmd -b -S sqlserver -U SA -P 'Strong!Pass123' -Q "RESTORE FILELISTONLY FROM DISK = '$BACKUP'" -W -s"|" -h -1 2>&1) || {
  echo "RESTORE FILELISTONLY failed. Output:"
  echo "$FILELIST"
  if echo "$FILELIST" | grep -i "incompatible" >/dev/null 2>&1; then
    echo "ERROR: Backup was created on a newer SQL Server version and is incompatible with this server. Use a newer SQL Server image that supports the backup version."
    exit 5
  fi
  exit 4
}

DATA_LOGICAL=$(echo "$FILELIST" | awk -F"|" '$3=="D"{print $1; exit}')
LOG_LOGICAL=$(echo "$FILELIST" | awk -F"|" '$3=="L"{print $1; exit}')

if [ -z "$DATA_LOGICAL" ] || [ -z "$LOG_LOGICAL" ]; then
  echo "Could not detect logical file names from backup. Filelist output:"
  echo "$FILELIST"
  exit 2
fi

echo "Data logical name: $DATA_LOGICAL"
echo "Log logical name: $LOG_LOGICAL"

MDFFILE="/var/opt/mssql/data/EcommerceWebApiDb.mdf"
LDFFILE="/var/opt/mssql/data/EcommerceWebApiDb_log.ldf"

if /opt/mssql-tools/bin/sqlcmd -S sqlserver -U SA -P 'Strong!Pass123' -Q "RESTORE DATABASE EcommerceWebApiDb FROM DISK = '$BACKUP' WITH MOVE '$DATA_LOGICAL' TO '$MDFFILE', MOVE '$LOG_LOGICAL' TO '$LDFFILE', REPLACE" 2>&1 | tee -a "$LOG"; then
  echo "Restore with MOVE succeeded"
  exit 0
else
  echo "Restore with MOVE failed. See /restore.log for details"
  exit 3
fi
