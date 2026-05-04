CREATE TABLE SalesOrderHeaders (
	OrderNumber			INT				IDENTITY(1,1) PRIMARY KEY,
	OrderCreatedAt		DATETIME2		NOT NULL,
	OrderCompletedAt	DATETIME2		NOT NULL,
	CustomerId			INT				NOT NULL,
	Status				INT				NOT NULL,
	FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);