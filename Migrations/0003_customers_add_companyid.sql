ALTER TABLE Customers
ADD CompanyId INT NULL;
GO

-- update existing records to have a default company id of 1
UPDATE Customers
SET CompanyId = 1
WHERE CompanyId IS NULL;
GO

ALTER TABLE Customers
ALTER COLUMN CompanyId INT NOT NULL;
GO

ALTER TABLE Customers
ADD CONSTRAINT FK_Customers_Companies
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id);
GO
