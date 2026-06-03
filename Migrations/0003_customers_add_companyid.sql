ALTER TABLE Customers
ADD CompanyId INT NULL;
GO

DECLARE @BackfillCompanyId INT = (
    SELECT TOP (1) Id
    FROM Companies
    ORDER BY Id
)

IF @BackfillCompanyId IS NULL
    THROW 50001, 'No companies found to the backfill CompanyId in Customers.', 1;

UPDATE Customers
SET CompanyId = @BackfillCompanyId
WHERE CompanyId IS NULL;
GO

ALTER TABLE Customers
ALTER COLUMN CompanyId INT NOT NULL;
GO

ALTER TABLE Customers
ADD CONSTRAINT FK_Customers_Companies
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id);
GO
