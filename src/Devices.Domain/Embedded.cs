using System.Text.RegularExpressions;
using Tutorial3_Task;

namespace Devices.Infrastructure;



public class Embedded : Device
{
    private string _ipAddress = string.Empty;
    private bool _isConnected = false;
    
    private string _networkName = string.Empty;
    public string NetworkName
    {
        get => _networkName;
        set
        {
            Console.WriteLine(value);
            if (!value.Contains("MD Ltd."))
            {
                throw new ConnectionException();
            }
            _networkName = value;
        }
    }

    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            Regex ipRegex = new Regex(@"^(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)\." +
                                      @"(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)\." +
                                      @"(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)\." +
                                      @"(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)$");
            if (ipRegex.IsMatch(value))
            {
                _ipAddress = value;
            }
            else
            {
                throw new ArgumentException("Wrong IP address format.");
            }
        }
    }
    

    public Embedded(string id, string name, bool isEnabled, string ipAddress, string networkName) 
    {
        if (!CheckId(id))
        {
            throw new ArgumentException("Invalid ID value. Required format: ED-1", id);
        }

        Id = id;
        Name = name;
        IsEnabled = isEnabled;
        IpAddress = ipAddress;
        NetworkName = networkName;
    }
    

    public override void TurnOn()
    {
        Connect();
        base.TurnOn();
    }

    public override void TurnOff()
    {
        _isConnected = false;
        base.TurnOff();
    }

    public override void Save()
    {
        throw new NotImplementedException();
    }


    public override string ToString()
    {
        string enabledStatus = IsEnabled ? "enabled" : "disabled";
        return $"Embedded device {Name} ({Id}) is {enabledStatus} and has IP address {IpAddress}";
    }

    private void Connect()
    {
        if (NetworkName.Contains("MD Ltd."))
        {
            _isConnected = true;
        }
        else
        {
            throw new ConnectionException();
        }
    }

    private bool CheckId(string id) => id.Contains("ED-");
}

class ConnectionException : Exception
{
    public ConnectionException() : base("Wrong network name.") { }
}
