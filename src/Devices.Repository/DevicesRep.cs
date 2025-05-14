using System.Data;
using System.Security.AccessControl;
using Devices.Infrastructure;
using Microsoft.Data.SqlClient;
using Tutorial3_Task;

namespace Devices.Repository;

public class DevicesRep
{
	private readonly string _connectionString;

    public DevicesRep(string connectionString)
    {
    	_connectionString = connectionString;
    }

	public void AddEmbedded(Device device)
	{
    	using var conn = new SqlConnection(_connectionString);
    	using var cmd = new SqlCommand("AddEmbedded", conn)
    	{
        	CommandType = CommandType.StoredProcedure
    	};

    	var embedded = device as Embedded;
    	cmd.Parameters.AddWithValue("@Name", embedded.Name);
    	cmd.Parameters.AddWithValue("@IsEnabled", embedded.IsEnabled);
    	cmd.Parameters.AddWithValue("@IpAddress", embedded.IpAddress);
    	cmd.Parameters.AddWithValue("@NetworkName", embedded.NetworkName);
	    cmd.Parameters.AddWithValue("@DeviceId", embedded.Id);
	    

    	conn.Open();
    	cmd.ExecuteNonQuery();
	}
	
	
	public string GenerateNextId(string deviceType)
	{
		string prefix = deviceType.ToLower() switch
		{
			"pc" => "P-",
			"embedded" => "ED-",
			"smartwatch" => "SW-",
			_ => throw new ArgumentException("Invalid device type")
		};

		string table = deviceType.ToLower() switch
		{
			"pc" => "PersonalComputer",
			"embedded" => "Embedded",
			"smartwatch" => "Smartwatch",
			_ => throw new ArgumentException("Invalid device type")
		};

		string query = $"SELECT TOP 1 DeviceId FROM {table} WHERE DeviceId LIKE @Prefix + '%' ORDER BY LEN(DeviceId) DESC, DeviceId DESC";

		using (var connection = new SqlConnection(_connectionString))
		{
			connection.Open();
			using (var command = new SqlCommand(query, connection))
			{
				command.Parameters.AddWithValue("@Prefix", prefix);
				var result = command.ExecuteScalar() as string;

				int lastNumber = 0;
				if (!string.IsNullOrEmpty(result))
				{
					var match = System.Text.RegularExpressions.Regex.Match(result, @"(\d+)$");
					if (match.Success)
					{
						lastNumber = int.Parse(match.Value);
					}
				}

				Console.WriteLine($"Last number: {lastNumber}");
				return $"{prefix}{lastNumber + 1}";
			}
		}
	}
	
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

		return null;
	}
	
	private PersonalComputer GetPersonalComputerByDeviceId(SqlConnection connection, string deviceId)
	{
		var command = new SqlCommand("SELECT OperationSystem FROM PersonalComputer WHERE DeviceId = @DeviceId", connection);
		command.Parameters.AddWithValue("@DeviceId", deviceId);

		using (var reader = command.ExecuteReader())
		{
			if (reader.Read())
			{
				return new PersonalComputer(id: deviceId, name: "", isEnabled: false, operatingSystem: reader.IsDBNull(0) ? null : reader.GetString(0));
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
				return new Embedded(id: deviceId, name: "", isEnabled: false, ipAddress: reader.GetString(0), networkName: reader.GetString(1));
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
				return new Smartwatch(id: deviceId, name: "", isEnabled: false, batteryLevel: reader.GetInt32(0));
			}
		}
		return null;
	}
}