CREATE DATABASE CatalogDb;
GO

USE CatalogDb;

CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Stock INT NOT NULL
);

INSERT INTO Products (Name, Stock) VALUES ('Product A', 100), ('Product B', 200);
