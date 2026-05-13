IF DB_ID('ERP_CLI') IS NULL
BEGIN
    CREATE DATABASE ERP_CLI;
END
GO
USE ERP_CLI
GO

IF OBJECT_ID('Addresses', 'U') IS NULL
BEGIN
    CREATE TABLE Addresses (
        Id              INT             IDENTITY(1,1) PRIMARY KEY,
        Street          NVARCHAR(100)   NOT NULL,
        Number          NVARCHAR(50)    NOT NULL,
        PostalCode      NVARCHAR(20)    NOT NULL,
        City            NVARCHAR(50)    NOT NULL,
	Country			NVARCHAR(50)	NOT NULL
);
END

IF OBJECT_ID('Companies', 'U') IS NULL
BEGIN
    CREATE TABLE Companies (
	Id				INT				IDENTITY(1,1) PRIMARY KEY,
	AddressId		INT				NOT NULL,
	Name			NVARCHAR(200)	NOT NULL,
	Currency		NVARCHAR(10)	NOT NULL,

	FOREIGN KEY (AddressId) REFERENCES Addresses(Id)
		ON DELETE CASCADE
);
END

IF OBJECT_ID('Products', 'U') IS NULL
BEGIN
    CREATE TABLE Products (
    Id              INT             IDENTITY(1,1) PRIMARY KEY,
    ItemNumber      NVARCHAR(50)    NOT NULL,
    Name            NVARCHAR(200)   NOT NULL,
    Description     NVARCHAR(MAX)   NULL,
    Price           FLOAT           NOT NULL,
    Cost            FLOAT           NOT NULL,
    Location        NVARCHAR(100)   NOT NULL,
    StockQuantity   FLOAT           NOT NULL DEFAULT 0,
    Unit            INT             NOT NULL
);
END

IF OBJECT_ID('Persons', 'U') IS NULL
BEGIN
    CREATE TABLE Persons (
	Id				INT				IDENTITY(1,1) PRIMARY KEY,
	FirstName		NVARCHAR(100)	NOT NULL,
	LastName		NVARCHAR(100)	NOT NULL,
	Phone			NVARCHAR(30)	NOT NULL,
	Email			NVARCHAR(254)	NOT NULL,
	AddressId		INT				NOT NULL,
	FOREIGN KEY (AddressId) REFERENCES Addresses(Id)
		ON DELETE CASCADE
);
END

IF OBJECT_ID('Customers', 'U') IS NULL
BEGIN
    CREATE TABLE Customers (
	Id		        INT				PRIMARY KEY IDENTITY(1000,1),
    PersonId		INT				NOT NULL,
	LastPurchaseAt	DATETIME2		NULL,

	FOREIGN KEY (PersonId) REFERENCES Persons(Id)
		ON DELETE CASCADE
);
END

IF OBJECT_ID('SalesOrderHeaders', 'U') IS NULL
BEGIN
    CREATE TABLE SalesOrderHeaders (
	OrderNumber			INT				IDENTITY(1,1) PRIMARY KEY,
	OrderCreatedAt		DATETIME2		NOT NULL,
	OrderCompletedAt	DATETIME2		NULL,
	CustomerId			INT				NOT NULL,
	Status				INT				NOT NULL,
    
	FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);
END

IF OBJECT_ID('SalesOrderLines', 'U') IS NULL
BEGIN
    CREATE TABLE SalesOrderLines (
	Id				INT				IDENTITY(1,1) PRIMARY KEY,
	OrderNumber		INT				NOT NULL,
	ProductId		INT				NOT NULL,
	Quantity		FLOAT			NOT NULL,

	FOREIGN KEY (OrderNumber) REFERENCES SalesOrderHeaders(OrderNumber) ON DELETE CASCADE,
	FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
END
