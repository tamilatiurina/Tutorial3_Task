using Devices.Infrastructure;

namespace Tutorial3_Task;


public abstract class Device
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }

    // Dependency injection for the repository
    public readonly DeviceRepository _deviceRepository;

    public Device(DeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public virtual void TurnOn()
    {
        IsEnabled = true;
        _deviceRepository.UpdateDevice(this);  // Update the device status in the repository
    }

    public virtual void TurnOff()
    {
        IsEnabled = false;
        _deviceRepository.UpdateDevice(this);  // Update the device status in the repository
    }

    // Override this method to handle specific logic for each device type
    public abstract void Save();
}
