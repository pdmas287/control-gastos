# Script para generar clave JWT segura

Write-Host ""
Write-Host "============================================================"
Write-Host "GENERADOR DE CLAVE JWT SEGURA"
Write-Host "============================================================"
Write-Host ""

# Generar clave aleatoria de 64 caracteres
$jwtKey = -join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})

Write-Host "Clave JWT generada (64 caracteres):"
Write-Host ""
Write-Host $jwtKey -ForegroundColor Green
Write-Host ""
Write-Host "============================================================"
Write-Host ""
Write-Host "COPIA ESTA CLAVE Y USALA EN RAILWAY"
Write-Host ""
Write-Host "En Railway, crea una variable de entorno:"
Write-Host "  Nombre: Jwt__Key"
Write-Host "  Valor: $jwtKey"
Write-Host ""
Write-Host "IMPORTANTE: Usa DOBLE guion bajo: Jwt__Key"
Write-Host ""
Write-Host "============================================================"
Write-Host ""

# Guardar en archivo temporal (solo para referencia, NO commitear)
$keyFile = "jwt-key-temp.txt"
$jwtKey | Out-File -FilePath $keyFile -Encoding UTF8
Write-Host "Clave guardada temporalmente en: $keyFile"
Write-Host "ADVERTENCIA: NO subas este archivo a Git"
Write-Host ""
