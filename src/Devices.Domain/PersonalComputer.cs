using Devices.Infrastructure;

namespace Tutorial3_Task;


public class PersonalComputer : Device
{
    public string? OperatingSystem { get; set; }

    public PersonalComputer(DeviceRepository deviceRepository) : base(deviceRepository) { }

    public PersonalComputer(string id, string name, bool isEnabled, string? operatingSystem, DeviceRepository deviceRepository) 
        : base(deviceRepository)
    {
        if (!CheckId(id))
        {
            throw new ArgumentException("Invalid ID value. Required format: P-1", id);
        }

        Id = id;
        Name = name;
        IsEnabled = isEnabled;
        OperatingSystem = operatingSystem;
    }

    public override void TurnOn()
    {
        if (OperatingSystem is null)
        {
            throw new EmptySystemException();
        }
        
        base.TurnOn();
    }

    public override void Save()
    {
        _deviceRepository.AddDevice(this, "pc");
    }

    public override string ToString()
    {
        string enabledStatus = IsEnabled ? "enabled" : "disabled";
        string osStatus = OperatingSystem is null ? "has not OS" : $"has {OperatingSystem}";
        return $"PC {Name} ({Id}) is {enabledStatus} and {osStatus}";
    }

    private bool CheckId(string id) => id.Contains("P-");
}
