using System.Data;
using System.Data.SqlTypes;
using System.Security.AccessControl;
using Devices.Infrastructure;
using Microsoft.Data.SqlClient;
using Tutorial3_Task;

namespace Devices.Repository;

public class DevicesRep : IDevicesRep
{
	private readonly string _connectionString;

    public DevicesRep(string connectionString)
    {
    	_connectionString = connectionString;
    }

    public void AddDevice(Device device, string type)
    {
	    if (type.Equals("embedded"))
	    {
		    AddEmbedded(device);
	    }else if (type.Equals("smartwatch"))
	    {
		    AddSmartwatch(device);
	    }
	    else
	    { 
		    AddPC(device);   
	    }
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
	
	public void AddSmartwatch(Device device)
	{
		using var conn = new SqlConnection(_connectionString);
		using var cmd = new SqlCommand("AddSmartwatch", conn)
		{
			CommandType = CommandType.StoredProcedure
		};

		var smartwatch = device as Smartwatch;
		cmd.Parameters.AddWithValue("@Name", smartwatch.Name);
		cmd.Parameters.AddWithValue("@IsEnabled", smartwatch.IsEnabled);
		cmd.Parameters.AddWithValue("@DeviceId", smartwatch.Id);
		cmd.Parameters.AddWithValue("@BatteryPercentage", smartwatch.BatteryLevel);
	    

		conn.Open();
		cmd.ExecuteNonQuery();
	}
	
	public void AddPC(Device device)
	{
		using var conn = new SqlConnection(_connectionString);
		using var cmd = new SqlCommand("AddPersonalComputer", conn)
		{
			CommandType = CommandType.StoredProcedure
		};

		var pc = device as PersonalComputer;
		cmd.Parameters.AddWithValue("@Name", pc.Name);
		cmd.Parameters.AddWithValue("@IsEnabled", pc.IsEnabled);
		cmd.Parameters.AddWithValue("@DeviceId", pc.Id);
		cmd.Parameters.AddWithValue("@OperationSystem", pc.OperationSystem);
	    

		conn.Open();
		cmd.ExecuteNonQuery();
	}

	public bool UpdateDevice(Device device)
	{
		switch (device)
		{
			case PersonalComputer pc when pc.IsEnabled && string.IsNullOrWhiteSpace(pc.OperationSystem):
				throw new EmptySystemException();
		}

		using var connection = new SqlConnection(_connectionString);
		connection.Open();

		return device switch
		{
			PersonalComputer pc => UpdatePC(pc, connection),
			Embedded embedded => UpdateEmbedded(embedded, connection),
			Smartwatch sw => UpdateSmartwatch(sw, connection),
			_ => false
		};
	}

	public bool UpdateEmbedded(Embedded embedded, SqlConnection conn)
	{
		Console.WriteLine("entered");
		using var transaction = conn.BeginTransaction();
		try
		{
		
			var deviceCmd = new SqlCommand(@"
			UPDATE Device
			SET Name = @Name, Enabled = @IsEnabled
			WHERE Id = @DeviceId AND RowVersion = @OriginalRowVersion;", conn, transaction);

			deviceCmd.Parameters.AddWithValue("@Name", embedded.Name);
			deviceCmd.Parameters.AddWithValue("@IsEnabled", embedded.IsEnabled);
			deviceCmd.Parameters.AddWithValue("@DeviceId", embedded.Id);
			deviceCmd.Parameters.Add("@OriginalRowVersion", SqlDbType.Timestamp).Value = new SqlBinary(embedded.RowVersion);

			int deviceRows = deviceCmd.ExecuteNonQuery();
			if (deviceRows == 0)
			{
				transaction.Rollback();
				Console.WriteLine("Optimistic concurrency check failed: No rows matched RowVersion and ID.");
				Console.WriteLine($"Incoming ID: {embedded.Id}");
				Console.WriteLine($"Incoming RowVersion: {BitConverter.ToString(embedded.RowVersion)}");
				return false; 
			}

			var embeddedCmd = new SqlCommand(@"
			UPDATE Embedded
			SET IpAddress = @IpAddress, NetworkName = @NetworkName
			WHERE DeviceId = @DeviceId;", conn, transaction);

			embeddedCmd.Parameters.AddWithValue("@IpAddress", embedded.IpAddress);
			embeddedCmd.Parameters.AddWithValue("@NetworkName", embedded.NetworkName);
			embeddedCmd.Parameters.AddWithValue("@DeviceId", embedded.Id);

			embeddedCmd.ExecuteNonQuery();
			transaction.Commit();
			return true;
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			
			Console.WriteLine("Exception during update:");
			Console.WriteLine($"Message: {ex.Message}");
			Console.WriteLine($"StackTrace: {ex.StackTrace}");
			
			throw;
		}
	}
	
	public bool UpdatePC(PersonalComputer pc, SqlConnection conn)
	{
		using var transaction = conn.BeginTransaction();
		try
		{
		
			var deviceCmd = new SqlCommand(@"
			UPDATE Device
			SET Name = @Name, Enabled = @IsEnabled
			WHERE Id = @DeviceId AND RowVersion = @OriginalRowVersion;", conn, transaction);

			deviceCmd.Parameters.AddWithValue("@Name", pc.Name);
			deviceCmd.Parameters.AddWithValue("@IsEnabled", pc.IsEnabled);
			deviceCmd.Parameters.AddWithValue("@DeviceId", pc.Id);
			deviceCmd.Parameters.Add("@OriginalRowVersion", SqlDbType.Timestamp).Value = new SqlBinary(pc.RowVersion);

			int deviceRows = deviceCmd.ExecuteNonQuery();
			if (deviceRows == 0)
			{
				transaction.Rollback();
				Console.WriteLine("Optimistic concurrency check failed: No rows matched RowVersion and ID.");
				Console.WriteLine($"Incoming ID: {pc.Id}");
				Console.WriteLine($"Incoming RowVersion: {BitConverter.ToString(pc.RowVersion)}");
				return false; 
			}

			var pcCmd = new SqlCommand(@"
			UPDATE PersonalComputer
			SET OperationSystem = @OperationSystem
			WHERE DeviceId = @DeviceId;", conn, transaction);

			pcCmd.Parameters.AddWithValue("OperationSystem", pc.OperationSystem);
			pcCmd.Parameters.AddWithValue("@DeviceId", pc.Id);

			pcCmd.ExecuteNonQuery();
			transaction.Commit();
			return true;
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			
			Console.WriteLine("Exception during update:");
			Console.WriteLine($"Message: {ex.Message}");
			Console.WriteLine($"StackTrace: {ex.StackTrace}");
			
			throw;
		}
	}
	
	public bool UpdateSmartwatch(Smartwatch sm, SqlConnection conn)
	{
		using var transaction = conn.BeginTransaction();
		try
		{
		
			var deviceCmd = new SqlCommand(@"
			UPDATE Device
			SET Name = @Name, Enabled = @IsEnabled
			WHERE Id = @DeviceId AND RowVersion = @OriginalRowVersion;", conn, transaction);

			deviceCmd.Parameters.AddWithValue("@Name", sm.Name);
			deviceCmd.Parameters.AddWithValue("@IsEnabled", sm.IsEnabled);
			deviceCmd.Parameters.AddWithValue("@DeviceId", sm.Id);
			deviceCmd.Parameters.Add("@OriginalRowVersion", SqlDbType.Timestamp).Value = new SqlBinary(sm.RowVersion);

			int deviceRows = deviceCmd.ExecuteNonQuery();
			if (deviceRows == 0)
			{
				transaction.Rollback();
				Console.WriteLine("Optimistic concurrency check failed: No rows matched RowVersion and ID.");
				Console.WriteLine($"Incoming ID: {sm.Id}");
				Console.WriteLine($"Incoming RowVersion: {BitConverter.ToString(sm.RowVersion)}");
				return false; 
			}

			var smCmd = new SqlCommand(@"
			UPDATE Smartwatch
			SET BatteryPercentage = @BatteryPercentage
			WHERE DeviceId = @DeviceId;", conn, transaction);

			smCmd.Parameters.AddWithValue("@BatteryPercentage", sm.BatteryLevel);
			smCmd.Parameters.AddWithValue("@DeviceId", sm.Id);

			smCmd.ExecuteNonQuery();
			transaction.Commit();
			return true;
		}
		catch (Exception ex)
		{
			transaction.Rollback();
			
			Console.WriteLine("Exception during update:");
			Console.WriteLine($"Message: {ex.Message}");
			Console.WriteLine($"StackTrace: {ex.StackTrace}");
			
			throw;
		}
	}
	
	public void DeleteDevice(string id)
	{
		using (var connection = new SqlConnection(_connectionString))
		{
			connection.Open();
			
			using (var transaction = connection.BeginTransaction())
			{
				try
				{
					var deletePCCommand = new SqlCommand("DELETE FROM PersonalComputer WHERE DeviceId = @DeviceId", connection, transaction);
					deletePCCommand.Parameters.AddWithValue("@DeviceId", id);
					deletePCCommand.ExecuteNonQuery();

					var deleteEmbeddedCommand = new SqlCommand("DELETE FROM Embedded WHERE DeviceId = @DeviceId", connection, transaction);
					deleteEmbeddedCommand.Parameters.AddWithValue("@DeviceId", id);
					deleteEmbeddedCommand.ExecuteNonQuery();

					var deleteSmartwatchCommand = new SqlCommand("DELETE FROM Smartwatch WHERE DeviceId = @DeviceId", connection, transaction);
					deleteSmartwatchCommand.Parameters.AddWithValue("@DeviceId", id);
					deleteSmartwatchCommand.ExecuteNonQuery();

					var deleteDeviceCommand = new SqlCommand("DELETE FROM Device WHERE Id = @Id", connection, transaction);
					deleteDeviceCommand.Parameters.AddWithValue("@Id", id);
					deleteDeviceCommand.ExecuteNonQuery();
					
					transaction.Commit();
				}
				catch (Exception)
				{
					
					transaction.Rollback();
					throw; 
				}
			}
		}
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
		var deviceHeaders = new List<(string Id, string Name, bool IsEnabled)>();

		using (var connection = new SqlConnection(_connectionString))
		{
			connection.Open();

			
			var command = new SqlCommand("SELECT Id, Name, Enabled FROM Device", connection);
			using (var reader = command.ExecuteReader())
			{
				while (reader.Read())
				{
					var id = reader.GetString(0);
					var name = reader.GetString(1);
					var isEnabled = reader.GetBoolean(2);

					deviceHeaders.Add((id, name, isEnabled));
				}
			}
			
			var devices = new List<Device>();
			foreach (var (id, name, isEnabled) in deviceHeaders)
			{
				var device = GetDetailedDevice(connection, id, name, isEnabled);
				if (device != null)
					devices.Add(device);
			}

			return devices;
		}
	}
	
	
	public Device GetDetailedDevice(SqlConnection connection, string id, string name, bool isEnabled)
	{
		
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
	
	public PersonalComputer GetPersonalComputerByDeviceId(SqlConnection connection, string deviceId)
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

	public Embedded GetEmbeddedByDeviceId(SqlConnection connection, string deviceId)
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

	public Smartwatch GetSmartwatchByDeviceId(SqlConnection connection, string deviceId)
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