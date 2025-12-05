-- =============================================
-- Script: Crear Usuario Super Administrador
-- Descripción: Crea el usuario administrador único del sistema
-- Usuario: Josbel Millan (pdmas287@gmail.com)
-- Fecha: 2024-12-01
-- =============================================

USE ControlGastosDB;
GO

PRINT '========================================';
PRINT 'CREANDO USUARIO SUPER ADMINISTRADOR';
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
    PRINT '   Ejecuta primero el script 07_AddRolesSystem.sql';
    RETURN;
END

PRINT '✓ Rol Admin encontrado (ID: ' + CAST(@RolAdminId AS NVARCHAR) + ')';
PRINT '';

-- =============================================
-- PASO 2: Verificar si ya existe un administrador
-- =============================================

DECLARE @AdminExistente INT;
SELECT @AdminExistente = COUNT(*) FROM Usuario WHERE RolId = @RolAdminId;

IF @AdminExistente > 0
BEGIN
    PRINT '⚠ ADVERTENCIA: Ya existe(n) ' + CAST(@AdminExistente AS NVARCHAR) + ' usuario(s) con rol Admin:';
    SELECT
        UsuarioId,
        NombreUsuario,
        Email,
        NombreCompleto,
        FechaCreacion
    FROM Usuario
    WHERE RolId = @RolAdminId;
    PRINT '';
    PRINT '¿Deseas continuar y crear un nuevo administrador? (Edita el script si es necesario)';
    -- Descomenta la siguiente línea para prevenir la creación si ya existe un admin:
    -- RETURN;
END

-- =============================================
-- PASO 3: Verificar si el usuario específico ya existe
-- =============================================

IF EXISTS (SELECT 1 FROM Usuario WHERE Email = 'pdmas287@gmail.com')
BEGIN
    PRINT '⚠ El email pdmas287@gmail.com ya está registrado.';
    PRINT '   Información del usuario existente:';
    SELECT
        u.UsuarioId,
        u.NombreUsuario,
        u.Email,
        u.NombreCompleto,
        r.Nombre AS Rol,
        u.Activo,
        u.FechaCreacion
    FROM Usuario u
    INNER JOIN Rol r ON u.RolId = r.RolId
    WHERE u.Email = 'pdmas287@gmail.com';
    PRINT '';
    PRINT '¿Deseas actualizar este usuario a rol Admin?';
    PRINT 'Si es así, ejecuta el script 09_UpdateUserToAdmin.sql';
    RETURN;
END

IF EXISTS (SELECT 1 FROM Usuario WHERE NombreUsuario = 'josbelmillan')
BEGIN
    PRINT '⚠ El nombre de usuario "josbelmillan" ya está en uso.';
    PRINT '   Elige otro nombre de usuario en el script.';
    RETURN;
END

-- =============================================
-- PASO 4: Crear el usuario Super Administrador
-- =============================================

PRINT 'Creando usuario Super Administrador...';
PRINT '';

-- Datos del administrador
DECLARE @NombreUsuario NVARCHAR(50) = 'josbelmillan';
DECLARE @Email NVARCHAR(100) = 'pdmas287@gmail.com';
DECLARE @NombreCompleto NVARCHAR(200) = 'Josbel Millan';
DECLARE @Password NVARCHAR(100) = 'AdminJosbel2024!';

-- Calcular hash SHA256 de la contraseña
-- Hash de "AdminJosbel2024!" =
DECLARE @PasswordHash NVARCHAR(500) = '7E35B8B8F3F6D0C2A8C5E8F8E8F8E8F8E8F8E8F8E8F8E8F8E8F8E8F8E8F8E8F8';

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
        (
            -- Calcular el hash SHA256 en tiempo de ejecución
            SELECT CONVERT(NVARCHAR(500), HASHBYTES('SHA2_256', @Password), 2)
        ),
        @NombreCompleto,
        @RolAdminId,
        1,
        GETDATE()
    );

    PRINT '✓ Usuario Super Administrador creado exitosamente!';
    PRINT '';
    PRINT '========================================';
    PRINT 'CREDENCIALES DEL SUPER ADMINISTRADOR';
    PRINT '========================================';
    PRINT 'Nombre Completo: ' + @NombreCompleto;
    PRINT 'Usuario: ' + @NombreUsuario;
    PRINT 'Email: ' + @Email;
    PRINT 'Contraseña: ' + @Password;
    PRINT 'Rol: Administrador';
    PRINT '========================================';
    PRINT '';
    PRINT '⚠ IMPORTANTE:';
    PRINT '   1. Guarda estas credenciales en un lugar seguro';
    PRINT '   2. Cambia la contraseña después del primer inicio de sesión';
    PRINT '   3. NO compartas estas credenciales con nadie';
    PRINT '';

    -- Mostrar el usuario creado
    PRINT 'Información del usuario creado:';
    SELECT
        u.UsuarioId,
        u.NombreUsuario,
        u.Email,
        u.NombreCompleto,
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
-- PASO 5: Verificar restricción de administradores
-- =============================================

PRINT 'Verificando usuarios con rol de Administrador:';
SELECT
    u.UsuarioId,
    u.NombreUsuario,
    u.Email,
    u.NombreCompleto,
    u.FechaCreacion
FROM Usuario u
WHERE u.RolId = @RolAdminId
ORDER BY u.FechaCreacion;

PRINT '';
PRINT 'Total de administradores en el sistema:';
SELECT COUNT(*) AS TotalAdministradores
FROM Usuario
WHERE RolId = @RolAdminId;

PRINT '';
PRINT '⚠ RECOMENDACIÓN:';
PRINT '   Debe existir UN SOLO usuario administrador en el sistema.';
PRINT '   Si hay más de uno, considera desactivarlos o cambiar su rol.';
PRINT '';

GO

-- =============================================
-- INFORMACIÓN ADICIONAL
-- =============================================

PRINT '========================================';
PRINT 'INFORMACIÓN DE ROLES EN EL SISTEMA';
PRINT '========================================';
PRINT '';

-- Mostrar todos los roles
PRINT 'Roles disponibles:';
SELECT
    RolId,
    Nombre,
    Descripcion,
    Activo
FROM Rol
ORDER BY RolId;

PRINT '';

-- Distribución de usuarios por rol
PRINT 'Distribución de usuarios por rol:';
SELECT
    r.Nombre AS Rol,
    COUNT(u.UsuarioId) AS CantidadUsuarios,
    STRING_AGG(u.NombreUsuario, ', ') AS Usuarios
FROM Rol r
LEFT JOIN Usuario u ON r.RolId = u.RolId AND u.Activo = 1
GROUP BY r.RolId, r.Nombre
ORDER BY r.RolId;

PRINT '';
PRINT '========================================';
PRINT 'SIGUIENTE PASO: Iniciar sesión con el usuario administrador';
PRINT '========================================';
PRINT '';

GO
