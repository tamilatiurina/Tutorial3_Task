
namespace Tutorial3_Task;


public class Smartwatch : Device, IPowerNotify
{
    private int _batteryLevel;

    public int BatteryLevel
    {
        get => _batteryLevel;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentException("Invalid battery level value. Must be between 0 and 100.", nameof(value));
            }

            _batteryLevel = value;
            if (_batteryLevel < 20)
            {
                Notify();
            }
        }
    }
    

    public Smartwatch(string id, string name, bool isEnabled, int batteryLevel) 
    {
        if (!CheckId(id))
        {
            throw new ArgumentException("Invalid ID value. Required format: SW-1", id);
        }
        if (isEnabled && batteryLevel <11)
        {
            throw new EmptyBatteryException();
        }


        Id = id;
        Name = name;
        IsEnabled = isEnabled;
        BatteryLevel = batteryLevel;
    }

    public void Notify()
    {
        Console.WriteLine($"Battery level is low. Current level is: {BatteryLevel}");
    }

    public override void TurnOn()
    {
        if (BatteryLevel < 11)
        {
            throw new EmptyBatteryException();
        }

        base.TurnOn();
        BatteryLevel -= 10;

        if (BatteryLevel < 20)
        {
            Notify();
        }
    }

    public override void Save()
    {
        throw new NotImplementedException();
    }


    public override string ToString()
    {
        string enabledStatus = IsEnabled ? "enabled" : "disabled";
        return $"Smartwatch {Name} ({Id}) is {enabledStatus} and has {BatteryLevel}%";
    }

    private bool CheckId(string id) => id.Contains("SW-");
}