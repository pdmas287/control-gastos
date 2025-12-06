# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY Backend/ControlGastos.API/*.csproj Backend/ControlGastos.API/
WORKDIR /src/Backend/ControlGastos.API
RUN dotnet restore

# Copy everything else and build
WORKDIR /src
COPY Backend/ControlGastos.API/ Backend/ControlGastos.API/
WORKDIR /src/Backend/ControlGastos.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Start the application
# Port configuration is handled in Program.cs by reading PORT environment variable
CMD ["dotnet", "ControlGastos.API.dll"]
