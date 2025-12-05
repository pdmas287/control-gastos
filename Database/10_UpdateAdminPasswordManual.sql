-- =============================================
-- Script: Actualizar Contraseña de Admin Manualmente
-- Descripción: Actualiza la contraseña usando el hash calculado por C#
-- Fecha: 2025-12-01
-- =============================================

USE ControlGastosDB;
GO

PRINT '========================================';
PRINT 'ACTUALIZANDO CONTRASEÑA DE ADMIN';
PRINT '========================================';
PRINT '';

-- Hash calculado por C# para "AdminJosbel2024!"
-- Este es el hash que apareció en los logs: e7BrSTy2DC40VBazIpm1u5DmfeHYzan5Ez1N8udhTco=
DECLARE @HashCSharp NVARCHAR(500) = 'e7BrSTy2DC40VBazIpm1u5DmfeHYzan5Ez1N8udhTco=';

-- Actualizar el usuario
UPDATE Usuario
SET PasswordHash = @HashCSharp,
    FechaModificacion = GETDATE()
WHERE Email = 'pdmas287@gmail.com';

IF @@ROWCOUNT > 0
BEGIN
    PRINT '✓ Contraseña actualizada exitosamente';
    PRINT '';
    PRINT '========================================';
    PRINT 'CREDENCIALES ACTUALIZADAS';
    PRINT '========================================';
    PRINT 'Email: pdmas287@gmail.com';
    PRINT 'Password: AdminJosbel2024!';
    PRINT 'Hash (C#): ' + @HashCSharp;
    PRINT '========================================';
    PRINT '';

    -- Mostrar el usuario actualizado
    SELECT
        u.UsuarioId,
        u.NombreUsuario,
        u.Email,
        u.NombreCompleto,
        u.PasswordHash,
        r.Nombre AS Rol,
        u.Activo,
        u.FechaModificacion
    FROM Usuario u
    INNER JOIN Rol r ON u.RolId = r.RolId
    WHERE u.Email = 'pdmas287@gmail.com';
END
ELSE
BEGIN
    PRINT '❌ ERROR: Usuario no encontrado';
END

PRINT '';
PRINT '========================================';
PRINT 'AHORA PUEDES INICIAR SESIÓN';
PRINT '========================================';
PRINT '';

GO
