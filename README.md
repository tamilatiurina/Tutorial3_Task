1. Create appsettings.json as follows:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "myDB": "Server=YOURSERVER;Database=YOURDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

