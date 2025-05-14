# Use the official .NET SDK image to build the app (version 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the .sln solution file
COPY Tutorial3_Task.sln ./

# Copy project files into their respective folders so the solution can find them
COPY src/Devices.Domain/Devices.Domain.csproj src/Devices.Domain/
COPY src/Devices.Infrastructure/Devices.Infrastructure.csproj src/Devices.Infrastructure/
COPY src/Devices.WebAPI/Devices.WebAPI.csproj src/Devices.WebAPI/
# Repeat for other projects

# Restore dependencies for all projects
RUN dotnet restore Tutorial3_Task.sln

# Copy the full source code
COPY . ./

# Publish the projects (you usually only need to publish the main app, like Devices.WebAPI)
RUN dotnet publish src/Devices.WebAPI/Devices.WebAPI.csproj -c Release -o /app/publish /p:TreatWarningsAsErrors=false


# Use the official .NET runtime image to run the app (version 8)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port your app will run on (optional if it's already set in your code)
EXPOSE 80

# Define entrypoint for the container
ENTRYPOINT ["dotnet", "Devices.WebAPI.dll"]
