using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Devices.Infrastructure.Devices.Infrastraucture;

namespace Tutorial3_Task;

public class DeviceManager
{
    private string _inputDeviceFile;
    private const int MaxCapacity = 15;
    private List<Device> _devices = new(capacity: MaxCapacity);
    
    private readonly DeviceRepository _deviceRepository;

    public DeviceManager(string connectionString)
    {
        _deviceRepository = new DeviceRepository(connectionString);
    }


        // Get all devices (short info)
        public List<Device> GetAllDevices()
        {
            return _deviceRepository.GetAllDevices();
        }

        // Get device by ID (detailed info)
        public Device GetDeviceById(string id)
        {
            var device = _deviceRepository.GetDeviceById(id);
            if (device == null)
            {
                throw new ArgumentException($"Device with ID {id} not found.");
            }
            return device;
        }

        // Add a new device
        public void AddDevice(Device newDevice, string deviceType)
        {
            if (_deviceRepository.GetDeviceById(newDevice.Id) != null)
            {
                throw new ArgumentException($"Device with ID {newDevice.Id} already exists.");
            }

            _deviceRepository.AddDevice(newDevice, deviceType);
        }

        // Edit an existing device
        public void EditDevice(Device updatedDevice, string deviceType)
        {
            var existingDevice = _deviceRepository.GetDeviceById(updatedDevice.Id);
            if (existingDevice == null)
            {
                throw new ArgumentException($"Device with ID {updatedDevice.Id} not found.");
            }

            _deviceRepository.UpdateDevice(updatedDevice, deviceType);
        }

        // Delete device by ID
        public void RemoveDeviceById(string id)
        {
            var device = _deviceRepository.GetDeviceById(id);
            if (device == null)
            {
                throw new ArgumentException($"Device with ID {id} not found.");
            }

            _deviceRepository.DeleteDevice(id);
        }

        // Turn on device by ID
        public void TurnOnDevice(string id)
        {
            var device = _deviceRepository.GetDeviceById(id);
            if (device == null)
            {
                throw new ArgumentException($"Device with ID {id} not found.");
            }

            device.TurnOn();
            EditDevice(device, GetDeviceType(device));  // Update status in the database
        }

        // Turn off device by ID
        public void TurnOffDevice(string id)
        {
            var device = _deviceRepository.GetDeviceById(id);
            if (device == null)
            {
                throw new ArgumentException($"Device with ID {id} not found.");
            }

            device.TurnOff();
            EditDevice(device, GetDeviceType(device));  // Update status in the database
        }

        // Helper method to determine device type
        private string GetDeviceType(Device device)
        {
            if (device is PersonalComputer)
                return "pc";
            if (device is Embedded)
                return "embedded";
            if (device is Smartwatch)
                return "smartwatch";
            throw new ArgumentException("Unknown device type.");
        }
}
        
        

class EmptySystemException : Exception
{
    public EmptySystemException() : base("Operation system is not installed.") { }
}

class EmptyBatteryException : Exception
{
    public EmptyBatteryException() : base("Battery level is too low to turn it on.") { }
}

class ConnectionException : Exception
{
    public ConnectionException() : base("Wrong network name.") { }
}

interface IPowerNotify
{
    void Notify();
}