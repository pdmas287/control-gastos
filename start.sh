#!/bin/sh
set -e

echo "=== RAILWAY DEBUG ==="
echo "PORT variable: ${PORT}"
echo "All environment variables:"
env | grep -E "(PORT|ASPNETCORE)" || true

# Set the URL based on Railway's PORT variable
export ASPNETCORE_URLS="http://0.0.0.0:${PORT:-8080}"
echo "ASPNETCORE_URLS set to: ${ASPNETCORE_URLS}"
echo "Starting application..."

# Start the application
exec dotnet ControlGastos.API.dll
