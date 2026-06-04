ALTER TABLE SalesOrderHeaders
    ADD CompanyId INT NULL, ContactPersonId INT NULL;
GO

UPDATE SalesOrderHeaders
SET SalesOrderHeaders.CompanyId = c.CompanyId,
    SalesOrderHeaders.ContactPersonId = c.PersonId
FROM SalesOrderHeaders
JOIN Customers c ON c.Id = SalesOrderHeaders.CustomerId
GO

-- Looking up system-generated name for old FK constraint, before dropping the customer relationship

DECLARE @fk SYSNAME = (
    SELECT fk.name FROM sys.foreign_keys fk
    WHERE fk.parent_object_id = OBJECT_ID('SalesOrderHeaders')
    AND fk.referenced_object_id = OBJECT_ID('Customers'));

IF @fk IS NOT NULL
BEGIN
    EXECUTE('ALTER TABLE SalesOrderHeaders DROP CONSTRAINT [' + @fk + ']')
END
GO

ALTER TABLE SalesOrderHeaders DROP COLUMN CustomerId;
GO

IF  EXISTS (SELECT 1 FROM SalesOrderHeaders WHERE CompanyId IS NULL)
    THROW 50020, 'Orphaned SalesOrderHeaders found. Please fix the data before running this migration.', 1;
GO

IF  EXISTS (SELECT 1 FROM SalesOrderHeaders WHERE ContactPersonId IS NULL)
    THROW 50021, 'Orphaned SalesOrderHeaders found. Please fix the data before running this migration.', 1;
GO

ALTER TABLE SalesOrderHeaders ALTER COLUMN CompanyId INT NOT NULL;

ALTER TABLE SalesOrderHeaders
    ADD CONSTRAINT FK_SalesOrderHeaders_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id),
    CONSTRAINT FK_SOH_ContactPerson FOREIGN KEY (ContactPersonId) REFERENCES Persons(Id) ON DELETE SET NULL;
GO