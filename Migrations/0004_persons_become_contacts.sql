ALTER TABLE Persons
    ADD CompanyId INT NULL;
GO

ALTER TABLE Companies
    ADD LastPurchaseAt DATETIME2 NULL;
GO

UPDATE Persons
SET Persons.CompanyId = Customers.CompanyId
FROM Persons
JOIN Customers ON Customers.PersonId = Persons.Id
GO

UPDATE Companies
SET Companies.LastPurchaseAt = sub.MaxLast
FROM Companies
JOIN (
    SELECT CompanyId, MAX(LastPurchaseAt) AS MaxLast
    FROM Customers
    WHERE LastPurchaseAt IS NOT NULL
    GROUP BY CompanyId
) sub ON sub.CompanyId = Companies.Id
GO