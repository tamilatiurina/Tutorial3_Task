INSERT INTO Device (Id, Name, Enabled) VALUES
                                           ('P-1', 'Personal Computer 1', 1),  -- Enabled PC
                                           ('P-2', 'Personal Computer 2', 1),  -- Enabled PC
                                           ('ED-1', 'Embedded Device 1', 1),   -- Enabled Embedded Device
                                           ('ED-4', 'Embedded Device 4', 0),   -- Disabled Embedded Device
                                           ('SW-1', 'Smartwatch 1', 1),        -- Enabled Smartwatch
                                           ('SW-2', 'Smartwatch 2', 0);        -- Disabled Smartwatch
                                           
                                           
                                           
                                           
                                           
INSERT INTO PersonalComputer (Id, Name, DeviceId) VALUES
                                                      (1, 'Office PC', 'P-1'),
                                                      (2, 'Gaming PC', 'P-2');





INSERT INTO Embedded (Id, IpAddress, NetworkName, DeviceId) VALUES
                                                                (1, '192.168.1.10', 'Network-A', 'ED-1'),
                                                                (2, '192.168.1.20', 'Network-B', 'ED-4');




INSERT INTO Smartwatch (Id, BatteryPercentage, DeviceId) VALUES
                                                             (1, 85, 'SW-1'),  -- Smartwatch with 85% battery
                                                             (2, 40, 'SW-2');  -- Smartwatch with 40% battery

