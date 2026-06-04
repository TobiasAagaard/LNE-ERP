IF EXISTS (SELECT 1 FROM Persons WHERE CompanyId IS NULL)
    THROW 50021, 'Orphaned Persons found. Please fix the data before running this migration.', 1;
GO

ALTER TABLE Persons ALTER COLUMN CompanyId INT NOT NULL;
GO

ALTER TABLE Persons
    ADD CONSTRAINT FK_Persons_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id);
GO

DROP TABLE Customers;
GO