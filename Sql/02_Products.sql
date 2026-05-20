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