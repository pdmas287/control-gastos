# Script para limpiar el proyecto antes de hacer commit
# Ejecutar desde la raiz del proyecto

Write-Host "Limpiando archivos compilados del proyecto..." -ForegroundColor Cyan
Write-Host ""

# Limpiar Backend
Write-Host "Limpiando Backend..." -ForegroundColor Yellow
if (Test-Path "Backend/ControlGastos.API") {
    Push-Location "Backend/ControlGastos.API"

    # Ejecutar dotnet clean
    Write-Host "  - Ejecutando dotnet clean..." -ForegroundColor Gray
    dotnet clean --verbosity quiet

    # Eliminar directorios bin y obj
    if (Test-Path "bin") {
        Write-Host "  - Eliminando directorio bin..." -ForegroundColor Gray
        Remove-Item -Recurse -Force "bin"
    }
    if (Test-Path "obj") {
        Write-Host "  - Eliminando directorio obj..." -ForegroundColor Gray
        Remove-Item -Recurse -Force "obj"
    }

    Pop-Location
    Write-Host "  Backend limpio" -ForegroundColor Green
} else {
    Write-Host "  Directorio Backend no encontrado" -ForegroundColor Red
}

Write-Host ""

# Limpiar Frontend
Write-Host "Limpiando Frontend..." -ForegroundColor Yellow
if (Test-Path "Frontend/control-gastos-app") {
    Push-Location "Frontend/control-gastos-app"

    # Eliminar directorio dist
    if (Test-Path "dist") {
        Write-Host "  - Eliminando directorio dist..." -ForegroundColor Gray
        Remove-Item -Recurse -Force "dist"
    }

    # Eliminar directorio .angular
    if (Test-Path ".angular") {
        Write-Host "  - Eliminando cache de Angular..." -ForegroundColor Gray
        Remove-Item -Recurse -Force ".angular"
    }

    Pop-Location
    Write-Host "  Frontend limpio" -ForegroundColor Green
} else {
    Write-Host "  Directorio Frontend no encontrado" -ForegroundColor Red
}

Write-Host ""

# Verificar archivos sensibles
Write-Host "Verificando archivos sensibles..." -ForegroundColor Yellow

$sensibleFiles = @(
    "Backend/ControlGastos.API/appsettings.Development.json",
    "Backend/ControlGastos.API/appsettings.Production.json",
    ".env",
    ".env.local",
    "*.pfx",
    "*.key",
    "*.pem"
)

$foundSensible = $false
foreach ($pattern in $sensibleFiles) {
    $files = Get-ChildItem -Path . -Filter $pattern -Recurse -ErrorAction SilentlyContinue
    if ($files) {
        Write-Host "  ADVERTENCIA: Encontrado archivo sensible: $($files.FullName)" -ForegroundColor Red
        $foundSensible = $true
    }
}

if (-not $foundSensible) {
    Write-Host "  No se encontraron archivos sensibles" -ForegroundColor Green
}

Write-Host ""

# Mostrar tamanio del proyecto
Write-Host "Tamanio del proyecto..." -ForegroundColor Yellow
$size = (Get-ChildItem -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
Write-Host "  Tamanio total: $([math]::Round($size, 2)) MB" -ForegroundColor Gray

Write-Host ""

# Resumen
Write-Host "Limpieza completada!" -ForegroundColor Green
Write-Host "Ya puedes hacer commit de forma segura." -ForegroundColor Cyan
Write-Host ""
