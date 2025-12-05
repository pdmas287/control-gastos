-- =============================================
-- Script: Crear Usuario Administrador
-- Descripción: Crea un usuario admin por defecto
-- =============================================

USE ControlGastosDB;
GO

-- Verificar si ya existe un usuario admin
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE NombreUsuario = 'admin')
BEGIN
    -- Insertar usuario administrador
    -- Contraseña: Admin123! (hasheada con SHA256)
    INSERT INTO Usuario (
        NombreUsuario,
        Email,
        PasswordHash,
        NombreCompleto,
        Activo,
        FechaCreacion
    )
    VALUES (
        'admin',
        'admin@controlgastos.com',
        '8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918', -- Hash de "Admin123!"
        'Administrador del Sistema',
        1,
        GETDATE()
    );

    PRINT 'Usuario administrador creado exitosamente.';
    PRINT 'Credenciales:';
    PRINT '  Usuario: admin';
    PRINT '  Email: admin@controlgastos.com';
    PRINT '  Contraseña: Admin123!';
    PRINT '';
    PRINT 'IMPORTANTE: Cambia esta contraseña después del primer inicio de sesión.';
END
ELSE
BEGIN
    PRINT 'El usuario "admin" ya existe en la base de datos.';
    PRINT 'Si olvidaste la contraseña, ejecuta el script 05_ResetAdminPassword.sql';
END
GO

-- Verificar la creación
SELECT
    UsuarioId,
    NombreUsuario,
    Email,
    NombreCompleto,
    Activo,
    FechaCreacion
FROM Usuario
WHERE NombreUsuario = 'admin';
GO
