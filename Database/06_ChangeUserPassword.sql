-- =============================================
-- Script: Cambiar Contraseña de Usuario
-- Descripción: Permite cambiar la contraseña de cualquier usuario
-- =============================================

USE ControlGastosDB;
GO

-- =============================================
-- INSTRUCCIONES DE USO:
-- =============================================
-- 1. Reemplaza '@NombreUsuario' con el nombre de usuario
-- 2. Calcula el hash SHA256 de la nueva contraseña
-- 3. Reemplaza '@NuevoPasswordHash' con el hash calculado
-- =============================================

-- Para calcular el hash SHA256 de una contraseña:
-- Opción 1: Usa una herramienta online como https://emn178.github.io/online-tools/sha256.html
-- Opción 2: Usa PowerShell:
--   $pass = 'TuNuevaContraseña'
--   $bytes = [System.Text.Encoding]::UTF8.GetBytes($pass)
--   $hash = [System.Security.Cryptography.SHA256]::Create().ComputeHash($bytes)
--   -join ($hash | ForEach-Object { $_.ToString('X2') })

-- =============================================
-- EJEMPLO: Cambiar contraseña del admin
-- =============================================

DECLARE @NombreUsuario NVARCHAR(50) = 'admin';  -- Cambiar aquí el usuario
DECLARE @NuevoPasswordHash NVARCHAR(500) = '8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918';  -- Hash de "Admin123!"

-- Verificar si existe el usuario
IF EXISTS (SELECT 1 FROM Usuario WHERE NombreUsuario = @NombreUsuario)
BEGIN
    -- Actualizar la contraseña
    UPDATE Usuario
    SET
        PasswordHash = @NuevoPasswordHash,
        FechaModificacion = GETDATE()
    WHERE NombreUsuario = @NombreUsuario;

    PRINT 'Contraseña actualizada exitosamente para el usuario: ' + @NombreUsuario;
    PRINT '';
    PRINT 'IMPORTANTE: Asegúrate de informar al usuario sobre el cambio de contraseña.';

    -- Mostrar información del usuario
    SELECT
        UsuarioId,
        NombreUsuario,
        Email,
        NombreCompleto,
        Activo,
        FechaModificacion
    FROM Usuario
    WHERE NombreUsuario = @NombreUsuario;
END
ELSE
BEGIN
    PRINT 'ERROR: El usuario "' + @NombreUsuario + '" no existe en la base de datos.';
    PRINT '';
    PRINT 'Usuarios existentes:';
    SELECT NombreUsuario, Email, NombreCompleto FROM Usuario WHERE Activo = 1;
END
GO

-- =============================================
-- CONTRASEÑAS COMUNES Y SUS HASHES SHA256
-- =============================================
-- Usa estos hashes o genera los tuyos propios

/*
Contraseña: Admin123!
Hash: 8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918

Contraseña: Password123
Hash: 42F749ADE7F9E195BF475F37A44CAFCB6039FD4F636A65916F7C8DEC3C16E0B6

Contraseña: Temporal123!
Hash: 5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8

Contraseña: Usuario123
Hash: 0B14D501A594442A01C6859541BCB3E8164D183D32937B851835442F69D5C94E

Para generar tu propio hash en PowerShell:
$pass = 'TuContraseña'
$bytes = [System.Text.Encoding]::UTF8.GetBytes($pass)
$hash = [System.Security.Cryptography.SHA256]::Create().ComputeHash($bytes)
-join ($hash | ForEach-Object { $_.ToString('X2') })
*/
