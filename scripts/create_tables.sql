CREATE DATABASE YourDb;
GO
USE YourDb;
GO

CREATE TABLE Device (
                        Id VARCHAR(50) PRIMARY KEY,
                        Name NVARCHAR(100) NOT NULL,
                        Enabled BIT NOT NULL
);

CREATE TABLE PersonalComputer (
                                  Id INT PRIMARY KEY,
                                  OperationSystem VARCHAR(100) NULL,
                                  DeviceId VARCHAR(50) NOT NULL,
                                  FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

CREATE TABLE Embedded (
                                  Id INT PRIMARY KEY,
                                  IpAddress VARCHAR(100)  NOT NULL,
                                  NetworkName VARCHAR(100)  NOT NULL,
                                  DeviceId VARCHAR(50) NOT NULL,
                                  FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

CREATE TABLE Smartwatch (
                          Id INT PRIMARY KEY,
                          BatteryPercentage INT  NOT NULL,
                          DeviceId VARCHAR(50) NOT NULL,
                          FOREIGN KEY (DeviceId) REFERENCES Device(Id)
);

GO