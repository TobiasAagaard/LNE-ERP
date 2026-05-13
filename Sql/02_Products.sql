CREATE TABLE Products (
    Id              INT             IDENTITY(1,1) PRIMARY KEY,
    ItemNumber      NVARCHAR(50)    NULL,
    Name            NVARCHAR(200)   NULL,
    Description     NVARCHAR(MAX)   NULL,
    Price           FLOAT           NULL,
    Cost            FLOAT           NULL,
    Location        NVARCHAR(100)   NULL,
    StockQuantity   FLOAT           NOT NULL DEFAULT 0,
    Unit            INT             NOT NULL
);