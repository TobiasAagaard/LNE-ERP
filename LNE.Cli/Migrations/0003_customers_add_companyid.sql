IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Customers', 'CompanyId') IS NULL
BEGIN
    ALTER TABLE dbo.Customers
    ADD CompanyId INT NULL;
END
GO

IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Customers', 'CompanyId') IS NOT NULL
   AND EXISTS (SELECT 1 FROM dbo.Customers WHERE CompanyId IS NULL)
BEGIN
    DECLARE @BackfillCompanyId INT;

    SELECT TOP (1) @BackfillCompanyId = Id
    FROM dbo.Companies
    ORDER BY Id;

    IF @BackfillCompanyId IS NULL
        THROW 50001, 'No companies found to backfill CompanyId in Customers.', 1;

    UPDATE dbo.Customers
    SET CompanyId = @BackfillCompanyId
    WHERE CompanyId IS NULL;
END
GO

IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Customers', 'CompanyId') IS NOT NULL
   AND EXISTS (
       SELECT 1
       FROM sys.columns
       WHERE object_id = OBJECT_ID('dbo.Customers')
         AND name = 'CompanyId'
         AND is_nullable = 1)
   AND NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE CompanyId IS NULL)
BEGIN
    ALTER TABLE dbo.Customers
    ALTER COLUMN CompanyId INT NOT NULL;
END
GO

IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Customers', 'CompanyId') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Customers_Companies')
BEGIN
    ALTER TABLE dbo.Customers
    ADD CONSTRAINT FK_Customers_Companies
        FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id);
END
GO
