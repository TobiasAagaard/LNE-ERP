CREATE TABLE SalesOrderLines (
	Id				INT				IDENTITY(1,1) PRIMARY KEY,
	OrderNumber		INT				NOT NULL,
	ProductId		INT				NOT NULL,
	Quantity		FLOAT			NOT NULL,

	FOREIGN KEY (OrderNumber) REFERENCES SalesOrderHeaders(OrderNumber) ON DELETE CASCADE,
	FOREIGN KEY (ProductId) REFERENCES Products(Id)
);