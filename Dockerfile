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
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (Railway will set the PORT environment variable)
EXPOSE 8080

# Set environment variable for ASP.NET to listen on all interfaces
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ControlGastos.API.dll"]
