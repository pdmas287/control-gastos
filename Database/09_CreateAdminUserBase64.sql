-- =============================================
-- Script: Crear Usuario Admin con Hash Base64
-- Descripción: Crea el usuario admin con hash compatible con C# AuthService
-- Fecha: 2025-12-01
-- =============================================

USE ControlGastosDB;
GO

PRINT '========================================';
PRINT 'CREANDO USUARIO ADMINISTRADOR';
PRINT '========================================';
PRINT '';

-- =============================================
-- PASO 1: Verificar que existe el rol Admin
-- =============================================

DECLARE @RolAdminId INT;
SELECT @RolAdminId = RolId FROM Rol WHERE Nombre = 'Admin';

IF @RolAdminId IS NULL
BEGIN
    PRINT '❌ ERROR: El rol Admin no existe.';
    PRINT '   Ejecuta primero el script 07_AddRolesSystem.sql o 07_FixRolesSystem_v3.sql';
    RETURN;
END

PRINT '✓ Rol Admin encontrado (ID: ' + CAST(@RolAdminId AS NVARCHAR) + ')';
PRINT '';

-- =============================================
-- PASO 2: Eliminar usuario existente si existe
-- =============================================

IF EXISTS (SELECT 1 FROM Usuario WHERE Email = 'pdmas287@gmail.com')
BEGIN
    PRINT '⚠ Usuario con email pdmas287@gmail.com ya existe. Eliminando...';
    DELETE FROM Usuario WHERE Email = 'pdmas287@gmail.com';
    PRINT '✓ Usuario anterior eliminado';
    PRINT '';
END

IF EXISTS (SELECT 1 FROM Usuario WHERE NombreUsuario = 'josbelmillan')
BEGIN
    PRINT '⚠ Usuario con nombre "josbelmillan" ya existe. Eliminando...';
    DELETE FROM Usuario WHERE NombreUsuario = 'josbelmillan';
    PRINT '✓ Usuario anterior eliminado';
    PRINT '';
END

-- =============================================
-- PASO 3: Crear el usuario Admin con hash Base64
-- =============================================

PRINT 'Creando usuario Administrador...';
PRINT '';

-- Datos del administrador
DECLARE @NombreUsuario NVARCHAR(50) = 'josbelmillan';
DECLARE @Email NVARCHAR(100) = 'pdmas287@gmail.com';
DECLARE @NombreCompleto NVARCHAR(200) = 'Josbel Millan';
DECLARE @Password NVARCHAR(100) = 'AdminJosbel2024!';

-- Calcular hash SHA256 en formato Base64 (compatible con C#)
DECLARE @PasswordHashBytes VARBINARY(32);
SET @PasswordHashBytes = HASHBYTES('SHA2_256', @Password);

-- Convertir a Base64 usando FOR XML PATH
DECLARE @PasswordHashBase64 NVARCHAR(500);
SET @PasswordHashBase64 = CAST('' AS XML).value('xs:base64Binary(sql:variable("@PasswordHashBytes"))', 'NVARCHAR(500)');

-- Insertar el usuario
BEGIN TRY
    INSERT INTO Usuario (
        NombreUsuario,
        Email,
        PasswordHash,
        NombreCompleto,
        RolId,
        Activo,
        FechaCreacion
    )
    VALUES (
        @NombreUsuario,
        @Email,
        @PasswordHashBase64,
        @NombreCompleto,
        @RolAdminId,
        1,
        GETDATE()
    );

    PRINT '✓ Usuario Administrador creado exitosamente!';
    PRINT '';
    PRINT '========================================';
    PRINT 'CREDENCIALES DEL ADMINISTRADOR';
    PRINT '========================================';
    PRINT 'Nombre Completo: ' + @NombreCompleto;
    PRINT 'Usuario: ' + @NombreUsuario;
    PRINT 'Email: ' + @Email;
    PRINT 'Contraseña: ' + @Password;
    PRINT 'Rol: Administrador';
    PRINT 'Hash (Base64): ' + @PasswordHashBase64;
    PRINT '========================================';
    PRINT '';

    -- Mostrar el usuario creado
    PRINT 'Información del usuario creado:';
    SELECT
        u.UsuarioId,
        u.NombreUsuario,
        u.Email,
        u.NombreCompleto,
        u.PasswordHash,
        r.Nombre AS Rol,
        r.Descripcion AS DescripcionRol,
        u.Activo,
        u.FechaCreacion
    FROM Usuario u
    INNER JOIN Rol r ON u.RolId = r.RolId
    WHERE u.Email = @Email;

END TRY
BEGIN CATCH
    PRINT '❌ ERROR al crear el usuario:';
    PRINT ERROR_MESSAGE();
    RETURN;
END CATCH

PRINT '';
PRINT '========================================';
PRINT '✓ PROCESO COMPLETADO EXITOSAMENTE';
PRINT '========================================';
PRINT '';

-- =============================================
-- PASO 4: Verificar usuarios administradores
-- =============================================

PRINT 'Total de administradores en el sistema:';
SELECT COUNT(*) AS TotalAdministradores
FROM Usuario u
INNER JOIN Rol r ON u.RolId = r.RolId
WHERE r.Nombre = 'Admin' AND u.Activo = 1;

PRINT '';
PRINT '========================================';
PRINT 'SIGUIENTE PASO: Iniciar sesión';
PRINT '========================================';
PRINT 'URL: POST http://localhost:5000/api/Auth/login';
PRINT 'Body:';
PRINT '{';
PRINT '  "nombreUsuarioOEmail": "pdmas287@gmail.com",';
PRINT '  "password": "AdminJosbel2024!"';
PRINT '}';
PRINT '';

GO
