IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EcommerceWebApiDb')
BEGIN
    RESTORE DATABASE EcommerceWebApiDb
    FROM DISK = '/backup/EcommerceWebApiDb.bak'
    WITH REPLACE;
END
GO
