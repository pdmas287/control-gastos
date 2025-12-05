# Script para verificar que el proyecto este listo para Railway
# Ejecutar desde la raiz del proyecto

Write-Host "Verificando que el proyecto este listo para Railway..." -ForegroundColor Cyan
Write-Host ""

$allGood = $true

# 1. Verificar que existe railway.toml
Write-Host "1. Verificando railway.toml..." -ForegroundColor Yellow
if (Test-Path "railway.toml") {
    Write-Host "   railway.toml existe" -ForegroundColor Green
} else {
    Write-Host "   railway.toml NO encontrado" -ForegroundColor Red
    $allGood = $false
}

# 2. Verificar estructura del proyecto
Write-Host ""
Write-Host "2. Verificando estructura del proyecto..." -ForegroundColor Yellow
$requiredDirs = @(
    "Backend/ControlGastos.API",
    "Frontend/control-gastos-app",
    "Database"
)

foreach ($dir in $requiredDirs) {
    if (Test-Path $dir) {
        Write-Host "   $dir existe" -ForegroundColor Green
    } else {
        Write-Host "   $dir NO encontrado" -ForegroundColor Red
        $allGood = $false
    }
}

# 3. Verificar que Program.cs usa PostgreSQL
Write-Host ""
Write-Host "3. Verificando configuracion de PostgreSQL..." -ForegroundColor Yellow
if (Test-Path "Backend/ControlGastos.API/Program.cs") {
    $programContent = Get-Content "Backend/ControlGastos.API/Program.cs" -Raw
    if ($programContent -match "UseNpgsql") {
        Write-Host "   Program.cs configurado para PostgreSQL (UseNpgsql)" -ForegroundColor Green
    } else {
        Write-Host "   Program.cs NO esta configurado para PostgreSQL" -ForegroundColor Red
        Write-Host "      Busca 'UseSqlServer' y cambialo a 'UseNpgsql'" -ForegroundColor Yellow
        $allGood = $false
    }
} else {
    Write-Host "   Program.cs NO encontrado" -ForegroundColor Red
    $allGood = $false
}

# 4. Verificar paquete Npgsql en .csproj
Write-Host ""
Write-Host "4. Verificando paquete Npgsql..." -ForegroundColor Yellow
if (Test-Path "Backend/ControlGastos.API/ControlGastos.API.csproj") {
    $csprojContent = Get-Content "Backend/ControlGastos.API/ControlGastos.API.csproj" -Raw
    if ($csprojContent -match "Npgsql.EntityFrameworkCore.PostgreSQL") {
        Write-Host "   Paquete Npgsql.EntityFrameworkCore.PostgreSQL instalado" -ForegroundColor Green
    } else {
        Write-Host "   Paquete Npgsql NO encontrado en .csproj" -ForegroundColor Red
        $allGood = $false
    }
} else {
    Write-Host "   ControlGastos.API.csproj NO encontrado" -ForegroundColor Red
    $allGood = $false
}

# 5. Verificar .gitignore
Write-Host ""
Write-Host "5. Verificando .gitignore..." -ForegroundColor Yellow
if (Test-Path ".gitignore") {
    Write-Host "   .gitignore existe" -ForegroundColor Green
} else {
    Write-Host "   .gitignore NO encontrado (recomendado)" -ForegroundColor Yellow
}

# 6. Verificar que el proyecto compila
Write-Host ""
Write-Host "6. Verificando que el backend compile..." -ForegroundColor Yellow
Push-Location "Backend/ControlGastos.API"
$buildResult = dotnet build --no-restore --verbosity quiet 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "   Backend compila correctamente" -ForegroundColor Green
} else {
    Write-Host "   Backend NO compila" -ForegroundColor Red
    Write-Host "   Error: $buildResult" -ForegroundColor Gray
    $allGood = $false
}
Pop-Location

# 7. Verificar archivos sensibles que NO deben estar en Git
Write-Host ""
Write-Host "7. Verificando archivos sensibles..." -ForegroundColor Yellow
$sensiblePatterns = @(
    "appsettings.Development.json",
    "appsettings.Production.json"
)

$foundSensible = $false
foreach ($pattern in $sensiblePatterns) {
    $files = Get-ChildItem -Path . -Filter $pattern -Recurse -ErrorAction SilentlyContinue
    foreach ($file in $files) {
        # Verificar si esta en Git
        $gitCheck = git ls-files $file.FullName 2>&1
        if ($gitCheck) {
            Write-Host "   ADVERTENCIA: Archivo sensible en Git: $($file.Name)" -ForegroundColor Red
            Write-Host "      Ubicacion: $($file.FullName)" -ForegroundColor Gray
            $foundSensible = $true
        }
    }
}

if (-not $foundSensible) {
    Write-Host "   No hay archivos sensibles en Git" -ForegroundColor Green
}

# 8. Verificar Git
Write-Host ""
Write-Host "8. Verificando Git..." -ForegroundColor Yellow
git rev-parse --git-dir 2>&1 | Out-Null
if ($LASTEXITCODE -eq 0) {
    Write-Host "   Git inicializado" -ForegroundColor Green

    # Verificar remote
    $remote = git remote -v 2>&1
    if ($remote -match "origin") {
        Write-Host "   Remote 'origin' configurado" -ForegroundColor Green
    } else {
        Write-Host "   Remote 'origin' NO configurado" -ForegroundColor Yellow
        Write-Host "      Necesitaras configurarlo antes de subir a GitHub" -ForegroundColor Gray
    }
} else {
    Write-Host "   Git NO inicializado" -ForegroundColor Yellow
    Write-Host "      Ejecuta: git init" -ForegroundColor Gray
}

# Resumen final
Write-Host ""
Write-Host ("="*60) -ForegroundColor Cyan
if ($allGood) {
    Write-Host "TODO LISTO PARA RAILWAY!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Proximos pasos:" -ForegroundColor Cyan
    Write-Host "1. Limpia el proyecto: .\scripts\cleanup-before-commit.ps1" -ForegroundColor White
    Write-Host "2. Commit: git add . && git commit -m 'feat: proyecto listo para Railway'" -ForegroundColor White
    Write-Host "3. Push: git push origin main" -ForegroundColor White
    Write-Host "4. Conecta Railway con tu repositorio de GitHub" -ForegroundColor White
    Write-Host "5. Configura las variables de entorno en Railway" -ForegroundColor White
    Write-Host ""
    Write-Host "Consulta RAILWAY_CHECKLIST.md para mas detalles." -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host "HAY PROBLEMAS QUE RESOLVER" -ForegroundColor Red
    Write-Host ""
    Write-Host "Por favor corrige los errores marcados arriba." -ForegroundColor Yellow
    Write-Host ""
}
Write-Host ("="*60) -ForegroundColor Cyan
Write-Host ""
