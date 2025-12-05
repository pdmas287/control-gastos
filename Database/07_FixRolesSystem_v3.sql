-- =============================================
-- Script: Reparar Sistema de Roles (Versión 3)
-- Descripción: Agrega la columna RolId a Usuario si no existe
-- Fecha: 2025-12-01
-- =============================================

USE ControlGastosDB;
GO

PRINT '========================================';
PRINT 'REPARANDO SISTEMA DE ROLES';
PRINT '========================================';
PRINT '';

-- =============================================
-- PASO 1: Verificar que existe la tabla Rol
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rol]') AND type in (N'U'))
BEGIN
    PRINT '❌ ERROR: La tabla Rol no existe.';
    PRINT '   Ejecuta primero el script 07_AddRolesSystem.sql completamente.';
    RETURN;
END

PRINT '✓ Tabla Rol existe.';
GO

-- =============================================
-- PASO 2: Verificar que existen los roles
-- =============================================

IF NOT EXISTS (SELECT 1 FROM Rol WHERE Nombre = 'Admin')
   OR NOT EXISTS (SELECT 1 FROM Rol WHERE Nombre = 'Usuario')
BEGIN
    PRINT '❌ ERROR: No existen los roles Admin y/o Usuario.';
    PRINT '   Ejecuta primero el script 07_AddRolesSystem.sql completamente.';
    RETURN;
END

PRINT '✓ Roles Admin y Usuario existen.';
GO

-- =============================================
-- PASO 3: Agregar columna RolId a tabla Usuario
-- =============================================

PRINT 'Verificando columna RolId en tabla Usuario...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuario]') AND name = 'RolId')
BEGIN
    PRINT 'Agregando columna RolId a tabla Usuario...';

    -- Agregar columna RolId (nullable temporalmente)
    ALTER TABLE [dbo].[Usuario]
    ADD [RolId] INT NULL;

    PRINT '✓ Columna RolId agregada.';
END
ELSE
BEGIN
    PRINT '✓ La columna RolId ya existe en la tabla Usuario.';
END
GO

-- =============================================
-- PASO 4: Asignar rol Usuario a usuarios existentes
-- =============================================

DECLARE @RolUsuarioId INT;
SELECT @RolUsuarioId = RolId FROM Rol WHERE Nombre = 'Usuario';

IF EXISTS (SELECT 1 FROM Usuario WHERE RolId IS NULL)
BEGIN
    UPDATE Usuario SET RolId = @RolUsuarioId WHERE RolId IS NULL;
    PRINT '✓ Rol Usuario asignado a ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' usuarios.';
END
ELSE
BEGIN
    PRINT '✓ Todos los usuarios ya tienen rol asignado.';
END
GO

-- =============================================
-- PASO 5: Crear foreign key
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Usuario_Rol')
BEGIN
    ALTER TABLE [dbo].[Usuario]
    ADD CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY ([RolId])
    REFERENCES [dbo].[Rol] ([RolId]);
    PRINT '✓ Foreign key FK_Usuario_Rol creada.';
END
ELSE
BEGIN
    PRINT '✓ Foreign key FK_Usuario_Rol ya existe.';
END
GO

-- =============================================
-- PASO 6: Hacer la columna NOT NULL
-- =============================================

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS
           WHERE TABLE_NAME = 'Usuario'
           AND COLUMN_NAME = 'RolId'
           AND IS_NULLABLE = 'YES')
BEGIN
    ALTER TABLE [dbo].[Usuario]
    ALTER COLUMN [RolId] INT NOT NULL;
    PRINT '✓ Columna RolId configurada como NOT NULL.';
END
ELSE
BEGIN
    PRINT '✓ Columna RolId ya es NOT NULL.';
END
GO

-- =============================================
-- PASO 7: Crear índice para optimización
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuario_RolId' AND object_id = OBJECT_ID('Usuario'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Usuario_RolId]
    ON [dbo].[Usuario] ([RolId]);
    PRINT '✓ Índice IX_Usuario_RolId creado.';
END
ELSE
BEGIN
    PRINT '✓ Índice IX_Usuario_RolId ya existe.';
END
GO

-- =============================================
-- PASO 8: Crear o reemplazar vista
-- =============================================

PRINT '';
PRINT 'Creando vista...';

-- Eliminar vista si existe
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_UsuariosConRol')
BEGIN
    DROP VIEW [dbo].[vw_UsuariosConRol];
    PRINT 'Vista anterior eliminada.';
END
GO

-- Crear vista
CREATE VIEW [dbo].[vw_UsuariosConRol] AS
SELECT
    u.UsuarioId,
    u.NombreUsuario,
    u.Email,
    u.NombreCompleto,
    u.Activo AS UsuarioActivo,
    u.FechaCreacion,
    u.FechaModificacion,
    u.UltimoAcceso,
    r.RolId,
    r.Nombre AS RolNombre,
    r.Descripcion AS RolDescripcion,
    r.Activo AS RolActivo
FROM Usuario u
INNER JOIN Rol r ON u.RolId = r.RolId;
GO

PRINT '✓ Vista vw_UsuariosConRol creada.';
GO

-- =============================================
-- PASO 9: Verificación de la implementación
-- =============================================

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACIÓN DE LA REPARACIÓN';
PRINT '========================================';
PRINT '';

-- Mostrar roles
PRINT 'Roles disponibles:';
SELECT RolId, Nombre, Descripcion, Activo FROM Rol;
PRINT '';

-- Mostrar estructura de Usuario
PRINT 'Estructura de la tabla Usuario (columnas relacionadas con roles):';
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Usuario'
AND COLUMN_NAME IN ('UsuarioId', 'NombreUsuario', 'Email', 'RolId');
PRINT '';

-- Contar usuarios por rol
PRINT 'Distribución de usuarios por rol:';
SELECT
    r.Nombre AS Rol,
    COUNT(u.UsuarioId) AS CantidadUsuarios
FROM Rol r
LEFT JOIN Usuario u ON r.RolId = u.RolId
GROUP BY r.RolId, r.Nombre
ORDER BY r.RolId;
PRINT '';

-- Verificar vista
PRINT 'Contenido de la vista vw_UsuariosConRol:';
SELECT * FROM vw_UsuariosConRol;
PRINT '';

PRINT '========================================';
PRINT '✓ SISTEMA DE ROLES REPARADO EXITOSAMENTE';
PRINT '========================================';
PRINT '';
PRINT 'SIGUIENTE PASO: Ejecutar el script 08_CreateSuperAdmin.sql';
PRINT 'para crear el usuario administrador único.';
PRINT '';

GO
