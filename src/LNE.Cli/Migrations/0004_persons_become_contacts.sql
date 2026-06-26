IF OBJECT_ID('dbo.Persons', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Persons', 'CompanyId') IS NULL
BEGIN
    ALTER TABLE dbo.Persons
        ADD CompanyId INT NULL;
END
GO

IF OBJECT_ID('dbo.Companies', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Companies', 'LastPurchaseAt') IS NULL
BEGIN
    ALTER TABLE dbo.Companies
        ADD LastPurchaseAt DATETIME2 NULL;
END
GO

IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Persons', 'CompanyId') IS NOT NULL
BEGIN
    UPDATE p
    SET p.CompanyId = c.CompanyId
    FROM dbo.Persons p
    JOIN dbo.Customers c ON c.PersonId = p.Id
    WHERE p.CompanyId IS NULL;
END
GO

IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Companies', 'LastPurchaseAt') IS NOT NULL
BEGIN
    UPDATE c
    SET c.LastPurchaseAt = sub.MaxLast
    FROM dbo.Companies c
    JOIN (
        SELECT CompanyId, MAX(LastPurchaseAt) AS MaxLast
        FROM dbo.Customers
        WHERE LastPurchaseAt IS NOT NULL
        GROUP BY CompanyId
    ) sub ON sub.CompanyId = c.Id
    WHERE c.LastPurchaseAt IS NULL;
END
GO