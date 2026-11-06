
USE CafeteriaDB;

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Description VARCHAR(MAX)
);

CREATE TABLE Suppliers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Email VARCHAR(250) NOT NULL,
    Phone VARCHAR(20)
);

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Quantity INT NOT NULL,
    CategoryId INT NOT NULL foreign key
    References Categories(ID),
    SupplierId INT NOT NULL foreign key
    References Suppliers (Id)
);

CREATE TABLE PurchaseOrders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATE NOT NULL,
    Status VARCHAR(20) NOT NULL,
    SupplierId INT NOT NULL foreign key
    References Suppliers (Id)
);

CREATE TABLE PurchaseOrderItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseOrderId INT NOT NULL foreign key
    References PurchaseOrders(Id),
    ProductId INT NOT NULL foreign key
    References Products(d),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL
);
