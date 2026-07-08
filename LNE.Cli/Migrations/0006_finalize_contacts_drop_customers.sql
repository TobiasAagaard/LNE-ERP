IF OBJECT_ID('dbo.Persons', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Persons', 'CompanyId') IS NOT NULL
   AND EXISTS (SELECT 1 FROM dbo.Persons WHERE CompanyId IS NULL)
    THROW 50021, 'Orphaned Persons found. Please fix the data before running this migration.', 1;
GO

IF OBJECT_ID('dbo.Persons', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Persons', 'CompanyId') IS NOT NULL
   AND EXISTS (
       SELECT 1
       FROM sys.columns
       WHERE object_id = OBJECT_ID('dbo.Persons')
         AND name = 'CompanyId'
         AND is_nullable = 1)
   AND NOT EXISTS (SELECT 1 FROM dbo.Persons WHERE CompanyId IS NULL)
BEGIN
    ALTER TABLE dbo.Persons ALTER COLUMN CompanyId INT NOT NULL;
END
GO

IF OBJECT_ID('dbo.Persons', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Persons', 'CompanyId') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Persons_Companies')
BEGIN
    ALTER TABLE dbo.Persons
        ADD CONSTRAINT FK_Persons_Companies FOREIGN KEY (CompanyId) REFERENCES dbo.Companies(Id);
END
GO

IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Customers;
END
GO