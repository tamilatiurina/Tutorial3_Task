using Devices.Infrastructure;
using Microsoft.Data.SqlClient;
using Tutorial3_Task;

namespace Devices.Repository;

public interface IDevicesRep
{
    public void AddDevice(Device device, string type);
    public void AddEmbedded(Device device);
    public void AddSmartwatch(Device device);
    public void AddPC(Device device);
    public bool UpdateDevice(Device device);
    public bool UpdateEmbedded(Embedded embedded, SqlConnection conn);
    public bool UpdatePC(PersonalComputer pc, SqlConnection conn);
    public bool UpdateSmartwatch(Smartwatch sm, SqlConnection conn);
    public void DeleteDevice(string id);
    public string GenerateNextId(string deviceType);
    public List<Device> GetAllDevices();
    public Device GetDetailedDevice(SqlConnection connection, string id, string name, bool isEnabled, byte[] rowVersion);
    public PersonalComputer GetPersonalComputerByDeviceId(SqlConnection connection, string deviceId);
    public Embedded GetEmbeddedByDeviceId(SqlConnection connection, string deviceId);
    public Smartwatch GetSmartwatchByDeviceId(SqlConnection connection, string deviceId);
}