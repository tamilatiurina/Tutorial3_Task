USE YourDb;
GO
INSERT INTO Device (Id, Name, Enabled) VALUES
                                           ('P-1', 'Personal Computer 1', 1),  -- Enabled PC
                                           ('P-2', 'Personal Computer 2', 1),  -- Enabled PC
                                           ('ED-1', 'Embedded Device 1', 1),   -- Enabled Embedded Device
                                           ('ED-2', 'Embedded Device 4', 0),   -- Disabled Embedded Devicea
                                           ('SW-1', 'Smartwatch 1', 1),        -- Enabled Smartwatch
                                           ('SW-2', 'Smartwatch 2', 0);        -- Disabled Smartwatch
                                           
                                           
                                           
                                           
                                           
INSERT INTO PersonalComputer (Id, OperationSystem, DeviceId) VALUES
                                                      (1, 'Windows', 'P-1'),
                                                      (2, 'Linux', 'P-2');





INSERT INTO Embedded (Id, IpAddress, NetworkName, DeviceId) VALUES
                                                                (1, '192.168.1.10', 'MD Ltd. A', 'ED-1'),
                                                                (2, '192.168.1.20', 'MD Ltd. B', 'ED-2');




INSERT INTO Smartwatch (Id, BatteryPercentage, DeviceId) VALUES
                                                             (1, 85, 'SW-1'),  -- Smartwatch with 85% battery
                                                             (2, 40, 'SW-2');  -- Smartwatch with 40% battery

GO