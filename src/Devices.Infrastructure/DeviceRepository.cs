using Tutorial3_Task; // Your models
using Microsoft.Data.SqlClient;

using System;
using System.Collections.Generic;
using System.Data;

namespace Devices.Infrastructure
{
    public class DeviceRepository
    {
        private readonly string _connectionString;

        public DeviceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Get all devices
        public List<Device> GetAllDevices()
        {
            var devices = new List<Device>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Fetch basic device info
                var command = new SqlCommand("SELECT Id, Name, Enabled FROM Device", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetString(0);
                        var name = reader.GetString(1);
                        var isEnabled = reader.GetBoolean(2);

                        // Instead of opening new connections, call type-check methods directly
                        var device = GetDetailedDevice(connection, id, name, isEnabled);
                        if (device != null)
                            devices.Add(device);
                    }
                }
            }

            return devices;
        }

        private Device GetDetailedDevice(SqlConnection connection, string id, string name, bool isEnabled)
        {
            // Try to fetch from each specific table
            var pc = GetPersonalComputerByDeviceId(connection, id);
            if (pc != null)
            {
                pc.Name = name;
                pc.IsEnabled = isEnabled;
                return pc;
            }

            var embedded = GetEmbeddedByDeviceId(connection, id);
            if (embedded != null)
            {
                embedded.Name = name;
                embedded.IsEnabled = isEnabled;
                return embedded;
            }

            var smartwatch = GetSmartwatchByDeviceId(connection, id);
            if (smartwatch != null)
            {
                smartwatch.Name = name;
                smartwatch.IsEnabled = isEnabled;
                return smartwatch;
            }

            // If no type matched, return a basic Device
            var device = new Device(this);
            device.Id = id;
            device.Name = name;
            device.IsEnabled = isEnabled;
            return device;
        }

        public void AddDevice(Device device, string deviceType)
        {
            device.Id = Guid.NewGuid().ToString();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand("INSERT INTO Device (Id, Name, Enabled) VALUES (@Id, @Name, @Enabled)", connection);
                command.Parameters.AddWithValue("@Id", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@Enabled", device.IsEnabled);
                command.ExecuteNonQuery();

                switch (deviceType.ToLower())
                {
                    case "pc":
                        InsertPersonalComputer((PersonalComputer)device, connection);
                        break;
                    case "embedded":
                        InsertEmbedded((Embedded)device, connection);
                        break;
                    case "smartwatch":
                        InsertSmartwatch((Smartwatch)device, connection);
                        break;
                    default:
                        throw new ArgumentException("Unknown device type");
                }
            }
        }

        private void InsertPersonalComputer(PersonalComputer pc, SqlConnection connection)
        {
            var command = new SqlCommand("INSERT INTO PersonalComputer (Id, DeviceId, OperatingSystem) VALUES (@Id, @DeviceId, @OperatingSystem)", connection);
            command.Parameters.AddWithValue("@Id", pc.Id);
            command.Parameters.AddWithValue("@DeviceId", pc.DeviceId);
            command.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem ?? (object)DBNull.Value);
            command.ExecuteNonQuery();
        }

        private void InsertEmbedded(Embedded embedded, SqlConnection connection)
        {
            var command = new SqlCommand("INSERT INTO Embedded (Id, IpAddress, NetworkName, DeviceId) VALUES (@Id, @IpAddress, @NetworkName, @DeviceId)", connection);
            command.Parameters.AddWithValue("@Id", embedded.Id);
            command.Parameters.AddWithValue("@IpAddress", embedded.IpAddress);
            command.Parameters.AddWithValue("@NetworkName", embedded.NetworkName);
            command.Parameters.AddWithValue("@DeviceId", embedded.DeviceId);
            command.ExecuteNonQuery();
        }

        private void InsertSmartwatch(Smartwatch smartwatch, SqlConnection connection)
        {
            var command = new SqlCommand("INSERT INTO Smartwatch (Id, BatteryPercentage, DeviceId) VALUES (@Id, @BatteryPercentage, @DeviceId)", connection);
            command.Parameters.AddWithValue("@Id", smartwatch.Id);
            command.Parameters.AddWithValue("@BatteryPercentage", smartwatch.BatteryLevel);
            command.Parameters.AddWithValue("@DeviceId", smartwatch.DeviceId);
            command.ExecuteNonQuery();
        }

        public void UpdateDevice(Device device)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand("UPDATE Device SET Name = @Name, Enabled = @Enabled WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", device.Id);
                command.Parameters.AddWithValue("@Name", device.Name);
                command.Parameters.AddWithValue("@Enabled", device.IsEnabled);
                command.ExecuteNonQuery();

                switch (device)
                {
                    case PersonalComputer pc:
                        UpdatePersonalComputer(pc, connection);
                        break;
                    case Embedded embedded:
                        UpdateEmbedded(embedded, connection);
                        break;
                    case Smartwatch smartwatch:
                        UpdateSmartwatch(smartwatch, connection);
                        break;
                }
            }
        }

        private void UpdatePersonalComputer(PersonalComputer pc, SqlConnection connection)
        {
            var command = new SqlCommand("UPDATE PersonalComputer SET OperatingSystem = @OperatingSystem WHERE DeviceId = @DeviceId", connection);
            command.Parameters.AddWithValue("@OperatingSystem", pc.OperatingSystem ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DeviceId", pc.DeviceId);
            command.ExecuteNonQuery();
        }

        private void UpdateEmbedded(Embedded embedded, SqlConnection connection)
        {
            var command = new SqlCommand("UPDATE Embedded SET IpAddress = @IpAddress, NetworkName = @NetworkName WHERE DeviceId = @DeviceId", connection);
            command.Parameters.AddWithValue("@IpAddress", embedded.IpAddress);
            command.Parameters.AddWithValue("@NetworkName", embedded.NetworkName);
            command.Parameters.AddWithValue("@DeviceId", embedded.DeviceId);
            command.ExecuteNonQuery();
        }

        private void UpdateSmartwatch(Smartwatch smartwatch, SqlConnection connection)
        {
            var command = new SqlCommand("UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceId = @DeviceId", connection);
            command.Parameters.AddWithValue("@BatteryPercentage", smartwatch.BatteryLevel);
            command.Parameters.AddWithValue("@DeviceId", smartwatch.DeviceId);
            command.ExecuteNonQuery();
        }

        public void DeleteDevice(string id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var deletePCCommand = new SqlCommand("DELETE FROM PersonalComputer WHERE DeviceId = @DeviceId", connection);
                deletePCCommand.Parameters.AddWithValue("@DeviceId", id);
                deletePCCommand.ExecuteNonQuery();

                var deleteEmbeddedCommand = new SqlCommand("DELETE FROM Embedded WHERE DeviceId = @DeviceId", connection);
                deleteEmbeddedCommand.Parameters.AddWithValue("@DeviceId", id);
                deleteEmbeddedCommand.ExecuteNonQuery();

                var deleteSmartwatchCommand = new SqlCommand("DELETE FROM Smartwatch WHERE DeviceId = @DeviceId", connection);
                deleteSmartwatchCommand.Parameters.AddWithValue("@DeviceId", id);
                deleteSmartwatchCommand.ExecuteNonQuery();

                var deleteDeviceCommand = new SqlCommand("DELETE FROM Device WHERE Id = @Id", connection);
                deleteDeviceCommand.Parameters.AddWithValue("@Id", id);
                deleteDeviceCommand.ExecuteNonQuery();
            }
        }

        private PersonalComputer GetPersonalComputerByDeviceId(SqlConnection connection, string deviceId)
        {
            var command = new SqlCommand("SELECT OperatingSystem FROM PersonalComputer WHERE DeviceId = @DeviceId", connection);
            command.Parameters.AddWithValue("@DeviceId", deviceId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new PersonalComputer(id: deviceId, name: "", isEnabled: false, operatingSystem: reader.IsDBNull(0) ? null : reader.GetString(0), deviceRepository: this);
                }
            }
            return null;
        }

        private Embedded GetEmbeddedByDeviceId(SqlConnection connection, string deviceId)
        {
            var command = new SqlCommand("SELECT IpAddress, NetworkName FROM Embedded WHERE DeviceId = @DeviceId", connection);
            command.Parameters.AddWithValue("@DeviceId", deviceId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Embedded(id: deviceId, name: "", isEnabled: false, ipAddress: reader.GetString(0), networkName: reader.GetString(1), deviceRepository: this);
                }
            }
            return null;
        }

        private Smartwatch GetSmartwatchByDeviceId(SqlConnection connection, string deviceId)
        {
            var command = new SqlCommand("SELECT BatteryPercentage FROM Smartwatch WHERE DeviceId = @DeviceId", connection);
            command.Parameters.AddWithValue("@DeviceId", deviceId);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Smartwatch(id: deviceId, name: "", isEnabled: false, batteryLevel: reader.GetInt32(0), deviceRepository: this);
                }
            }
            return null;
        }
    }
}
