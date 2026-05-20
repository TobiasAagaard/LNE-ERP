CREATE TABLE Addresses (
        Id              INT             IDENTITY(1,1) PRIMARY KEY,
        Street          NVARCHAR(100)   NOT NULL,
        Number          NVARCHAR(50)    NOT NULL,
        PostalCode      NVARCHAR(20)    NOT NULL,
        City            NVARCHAR(50)    NOT NULL,
		Country			NVARCHAR(50)	NOT NULL,

		CONSTRAINT UQ_Addresses_Full
		UNIQUE (Street, [Number], PostalCode, City, Country)

);