-- =============================================
-- Script: Resetear Contraseña del Administrador
-- Descripción: Resetea la contraseña del usuario admin
-- =============================================

USE ControlGastosDB;
GO

-- Verificar si existe el usuario admin
IF EXISTS (SELECT 1 FROM Usuario WHERE NombreUsuario = 'admin')
BEGIN
    -- Actualizar la contraseña a "Admin123!" (hasheada con SHA256)
    UPDATE Usuario
    SET
        PasswordHash = '8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918',
        FechaModificacion = GETDATE()
    WHERE NombreUsuario = 'admin';

    PRINT 'Contraseña del administrador reseteada exitosamente.';
    PRINT 'Nuevas credenciales:';
    PRINT '  Usuario: admin';
    PRINT '  Contraseña: Admin123!';
    PRINT '';
    PRINT 'IMPORTANTE: Cambia esta contraseña después del inicio de sesión.';
END
ELSE
BEGIN
    PRINT 'ERROR: El usuario "admin" no existe en la base de datos.';
    PRINT 'Ejecuta primero el script 04_CreateAdminUser.sql';
END
GO

-- Mostrar información del usuario admin
SELECT
    UsuarioId,
    NombreUsuario,
    Email,
    NombreCompleto,
    Activo,
    FechaCreacion,
    FechaModificacion,
    UltimoAcceso
FROM Usuario
WHERE NombreUsuario = 'admin';
GO
