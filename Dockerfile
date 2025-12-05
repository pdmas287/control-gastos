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

# Railway dynamically assigns PORT
# We'll set ASPNETCORE_URLS at runtime using a shell script
CMD ["sh", "-c", "export ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} && dotnet ControlGastos.API.dll"]
