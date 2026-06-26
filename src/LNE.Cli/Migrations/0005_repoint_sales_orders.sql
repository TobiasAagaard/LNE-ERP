IF OBJECT_ID('dbo.SalesOrderHeaders', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.SalesOrderHeaders', 'CompanyId') IS NULL
BEGIN
    ALTER TABLE dbo.SalesOrderHeaders
        ADD CompanyId INT NULL;
END

IF OBJECT_ID('dbo.SalesOrderHeaders', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.SalesOrderHeaders', 'ContactPersonId') IS NULL
BEGIN
    ALTER TABLE dbo.SalesOrderHeaders
        ADD ContactPersonId INT NULL;
END
GO

IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.SalesOrderHeaders', 'CustomerId') IS NOT NULL
BEGIN
    EXEC('
        UPDATE soh
        SET soh.CompanyId = c.CompanyId,
            soh.ContactPersonId = c.PersonId
        FROM dbo.SalesOrderHeaders soh
        JOIN dbo.Customers c ON c.Id = soh.CustomerId
        WHERE soh.CompanyId IS NULL OR soh.ContactPersonId IS NULL;
    ');
END
GO

-- Looking up system-generated name for old FK constraint, before dropping the customer relationship

DECLARE @fk SYSNAME = (
    SELECT fk.name FROM sys.foreign_keys fk
    WHERE fk.parent_object_id = OBJECT_ID('dbo.SalesOrderHeaders')
    AND fk.referenced_object_id = OBJECT_ID('dbo.Customers'));

IF @fk IS NOT NULL
BEGIN
    EXECUTE('ALTER TABLE dbo.SalesOrderHeaders DROP CONSTRAINT [' + @fk + ']')
END
GO

IF OBJECT_ID('dbo.SalesOrderHeaders', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.SalesOrderHeaders', 'CustomerId') IS NOT NULL
BEGIN
    ALTER TABLE dbo.SalesOrderHeaders DROP COLUMN CustomerId;
END
GO

IF OBJECT_ID('dbo.SalesOrderHeaders', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.SalesOrderHeaders', 'CompanyId') IS NOT NULL
   AND EXISTS (SELECT 1 FROM dbo.SalesOrderHeaders WHERE CompanyId IS NULL)
    THROW 50020, 'Orphaned SalesOrderHeaders found. Please fix the data before running this migration.', 1;
GO

IF OBJECT_ID('dbo.SalesOrderHeaders', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.SalesOrderHeaders', 'ContactPersonId') IS NOT NULL
   AND EXISTS (SELECT 1 FROM dbo.SalesOrderHeaders WHERE ContactPersonId IS NULL)
    THROW 50021, 'Orphaned SalesOrderHeaders found. Please fix the data before running this migration.', 1;
GO

IF OBJECT_ID('dbo.SalesOrderHeaders', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.SalesOrderHeaders', 'CompanyId') IS NOT NULL
   AND EXISTS (
       SELECT 1
       FROM sys.columns
       WHERE object_id = OBJECT_ID('dbo.SalesOrderHeaders')
         AND name = 'CompanyId'
         AND is_nullable = 1)
   AND NOT EXISTS (SELECT 1 FROM dbo.SalesOrderHeaders WHERE CompanyId IS NULL)
BEGIN
    ALTER TABLE dbo.SalesOrderHeaders ALTER COLUMN CompanyId INT NOT NULL;
END

IF OBJECT_ID('dbo.SalesOrderHeaders', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.SalesOrderHeaders', 'CompanyId') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SalesOrderHeaders_Companies')
BEGIN
    ALTER TABLE dbo.SalesOrderHeaders
        ADD CONSTRAINT FK_SalesOrderHeaders_Companies
            FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id);
END

IF OBJECT_ID('dbo.SalesOrderHeaders', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.SalesOrderHeaders', 'ContactPersonId') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SOH_ContactPerson')
BEGIN
    ALTER TABLE dbo.SalesOrderHeaders
        ADD CONSTRAINT FK_SOH_ContactPerson
            FOREIGN KEY (ContactPersonId) REFERENCES dbo.Persons(Id) ON DELETE SET NULL;
END
GO