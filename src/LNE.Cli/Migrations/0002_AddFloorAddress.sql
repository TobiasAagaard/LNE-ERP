IF OBJECT_ID('dbo.Addresses', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Addresses', 'Floor') IS NULL
BEGIN
	ALTER TABLE dbo.Addresses
	ADD Floor NVARCHAR(50) NULL;
END
GO