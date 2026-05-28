ALTER TABLE Customers
ADD CompanyId INT NOT NULL;
GO

ALTER TABLE Customers
ADD CONSTRAINT FK_Customers_Companies
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id);
GO
