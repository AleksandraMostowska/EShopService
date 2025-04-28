IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MyDatabase')
BEGIN
    CREATE DATABASE MyDatabase;
END
GO

USE MyDatabase;
GO