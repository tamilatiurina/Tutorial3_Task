namespace RESTApi;

public class DeviceCreateRequest
{
    public string DeviceType { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public string OperationSystem { get; set; }
    public string IpAddress { get; set; }
    public string NetworkName { get; set; }
    public int? BatteryLevel { get; set; }
}

public class DeviceUpdateRequest
{
    public byte[] RowVersion { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public string OperationSystem { get; set; }
    public string IpAddress { get; set; }
    public string NetworkName { get; set; }
    public int? BatteryLevel { get; set; }
}