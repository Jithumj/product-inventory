CREATE TABLE Products (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProductCode NVARCHAR(50) UNIQUE NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    ProductImage VARBINARY(MAX) NULL,
    CreatedDate DATETIMEOFFSET NOT NULL,
    UpdatedDate DATETIMEOFFSET NOT NULL,
    CreatedUser UNIQUEIDENTIFIER NOT NULL,
    IsFavourite BIT NOT NULL DEFAULT 0,
    Active BIT NOT NULL DEFAULT 1,
    HSNCode NVARCHAR(100),
    TotalStock DECIMAL(18,2) NOT NULL DEFAULT 0
);

CREATE TABLE Variants (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE VariantOptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    VariantId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    FOREIGN KEY (VariantId) REFERENCES Variants(Id)
);

CREATE TABLE ProductVariantCombinations (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ProductId UNIQUEIDENTIFIER NOT NULL,
    CombinationCode NVARCHAR(200) NOT NULL,
    Stock DECIMAL(18,2) NOT NULL DEFAULT 0,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE ProductVariantCombinationOptions (
    ProductVariantCombinationId UNIQUEIDENTIFIER NOT NULL,
    VariantId UNIQUEIDENTIFIER NOT NULL,
    VariantOptionId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY(ProductVariantCombinationId, VariantId),
    FOREIGN KEY (ProductVariantCombinationId) REFERENCES ProductVariantCombinations(Id),
    FOREIGN KEY (VariantId) REFERENCES Variants(Id),
    FOREIGN KEY (VariantOptionId) REFERENCES VariantOptions(Id)
);

CREATE INDEX IX_ProductVariantCombinations_ProductId ON ProductVariantCombinations(ProductId);
CREATE INDEX IX_ProductVariantCombinations_CombinationCode ON ProductVariantCombinations(CombinationCode);
