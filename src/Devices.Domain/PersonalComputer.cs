

namespace Tutorial3_Task;


public class PersonalComputer : Device
{
    public string? OperationSystem ;
    
    

    public PersonalComputer(string id, string name, bool isEnabled, string? operatingSystem) 
    {
        if (!CheckId(id))
        {
            throw new ArgumentException("Invalid ID value. Required format: P-1", id);
        }
        
        if (isEnabled && string.IsNullOrWhiteSpace(operatingSystem))
        {
            throw new EmptySystemException();
        }
        
        Id = id;
        Name = name;
        IsEnabled = isEnabled;
        OperationSystem = operatingSystem;
    }

    public override void TurnOn()
    {
        if (OperationSystem is null)
        {
            throw new EmptySystemException();
        }
        
        base.TurnOn();
    }

    public override void Save()
    {
        throw new NotImplementedException();
    }


    public override string ToString()
    {
        string enabledStatus = IsEnabled ? "enabled" : "disabled";
        string osStatus = OperationSystem is null ? "has not OS" : $"has {OperationSystem}";
        return $"PC {Name} ({Id}) is {enabledStatus} and {osStatus}";
    }

    private bool CheckId(string id) => id.Contains("P-");
}

public class EmptySystemException : Exception
{
    public EmptySystemException() : base("Operation system is not installed.") { }
}
