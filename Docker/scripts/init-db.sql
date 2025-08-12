-- DotNetSkills Database Initialization Script
-- This script is executed when the SQL Server container starts for the first time

USE master;
GO

-- Check if the database exists, create if it doesn't
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DotNetSkills_Dev')
BEGIN
    CREATE DATABASE [DotNetSkills_Dev];
    PRINT 'Database DotNetSkills_Dev created successfully.';
END
ELSE
BEGIN
    PRINT 'Database DotNetSkills_Dev already exists.';
END
GO

-- Switch to the application database
USE [DotNetSkills_Dev];
GO

-- Basic database configuration for better performance
ALTER DATABASE [DotNetSkills_Dev] SET RECOVERY SIMPLE;
ALTER DATABASE [DotNetSkills_Dev] SET AUTO_CREATE_STATISTICS ON;
ALTER DATABASE [DotNetSkills_Dev] SET AUTO_UPDATE_STATISTICS ON;
GO

PRINT 'Database initialization completed successfully.';
GO