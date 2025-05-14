

using System.Text.Json.Serialization;

namespace Tutorial3_Task;


public abstract class Device
{
    [JsonPropertyName("rowVersion")] 
    public byte[] RowVersion { get; set; }
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = false;

    public virtual void TurnOn()
    {
        IsEnabled = true;
    }

    public virtual void TurnOff()
    {
        IsEnabled = false;
    }

    // Override this method to handle specific logic for each device type
    public abstract void Save();
}
