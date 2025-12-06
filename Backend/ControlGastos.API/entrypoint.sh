#!/bin/sh
# Set ASPNETCORE_URLS using Railway's PORT variable
export ASPNETCORE_URLS="http://0.0.0.0:${PORT:-8080}"
echo "Starting application on ${ASPNETCORE_URLS}"
exec dotnet ControlGastos.API.dll
