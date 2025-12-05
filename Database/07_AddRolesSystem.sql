-- =============================================
-- Script: Sistema de Roles
-- Descripción: Agrega tabla de roles y modifica Usuario
-- Fecha: 2024-12-01
-- =============================================

USE ControlGastosDB;
GO

PRINT '========================================';
PRINT 'INICIANDO IMPLEMENTACIÓN DE ROLES';
PRINT '========================================';
PRINT '';

-- =============================================
-- PASO 1: Crear tabla Roles
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rol]') AND type in (N'U'))
BEGIN
    PRINT 'Creando tabla Rol...';

    CREATE TABLE [dbo].[Rol](
        [RolId] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(50) NOT NULL,
        [Descripcion] NVARCHAR(200) NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Rol] PRIMARY KEY CLUSTERED ([RolId] ASC),
        CONSTRAINT [UK_Rol_Nombre] UNIQUE ([Nombre])
    );

    PRINT '✓ Tabla Rol creada exitosamente.';
END
ELSE
BEGIN
    PRINT '⚠ La tabla Rol ya existe. Continuando...';
END
GO

-- =============================================
-- PASO 2: Insertar roles por defecto
-- =============================================

PRINT '';
PRINT 'Insertando roles por defecto...';

-- Verificar e insertar rol Admin
IF NOT EXISTS (SELECT 1 FROM Rol WHERE Nombre = 'Admin')
BEGIN
    INSERT INTO Rol (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES ('Admin', 'Administrador del sistema con acceso completo a todos los datos', 1, GETDATE());
    PRINT '✓ Rol Admin creado.';
END
ELSE
BEGIN
    PRINT '⚠ Rol Admin ya existe.';
END

-- Verificar e insertar rol Usuario
IF NOT EXISTS (SELECT 1 FROM Rol WHERE Nombre = 'Usuario')
BEGIN
    INSERT INTO Rol (Nombre, Descripcion, Activo, FechaCreacion)
    VALUES ('Usuario', 'Usuario normal con acceso solo a sus propios datos', 1, GETDATE());
    PRINT '✓ Rol Usuario creado.';
END
ELSE
BEGIN
    PRINT '⚠ Rol Usuario ya existe.';
END
GO

-- =============================================
-- PASO 3: Agregar columna RolId a tabla Usuario
-- =============================================

PRINT '';
PRINT 'Modificando tabla Usuario...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Usuario]') AND name = 'RolId')
BEGIN
    PRINT 'Agregando columna RolId...';

    -- Agregar columna RolId (nullable temporalmente)
    ALTER TABLE [dbo].[Usuario]
    ADD [RolId] INT NULL;

    PRINT '✓ Columna RolId agregada.';

    -- Asignar rol "Usuario" a todos los usuarios existentes
    DECLARE @RolUsuarioId INT;
    SELECT @RolUsuarioId = RolId FROM Rol WHERE Nombre = 'Usuario';

    IF @RolUsuarioId IS NOT NULL
    BEGIN
        UPDATE Usuario SET RolId = @RolUsuarioId WHERE RolId IS NULL;
        PRINT '✓ Rol Usuario asignado a usuarios existentes.';
    END

    -- Crear foreign key
    ALTER TABLE [dbo].[Usuario]
    ADD CONSTRAINT [FK_Usuario_Rol] FOREIGN KEY ([RolId])
    REFERENCES [dbo].[Rol] ([RolId]);

    PRINT '✓ Foreign key creada.';

    -- Hacer la columna NOT NULL
    ALTER TABLE [dbo].[Usuario]
    ALTER COLUMN [RolId] INT NOT NULL;

    PRINT '✓ Columna RolId configurada como NOT NULL.';
END
ELSE
BEGIN
    PRINT '⚠ La columna RolId ya existe en la tabla Usuario.';
END
GO

-- =============================================
-- PASO 4: Crear índice para optimización
-- =============================================

PRINT '';
PRINT 'Creando índices...';

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuario_RolId' AND object_id = OBJECT_ID('Usuario'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Usuario_RolId]
    ON [dbo].[Usuario] ([RolId]);
    PRINT '✓ Índice IX_Usuario_RolId creado.';
END
ELSE
BEGIN
    PRINT '⚠ Índice IX_Usuario_RolId ya existe.';
END
GO

-- =============================================
-- PASO 5: Crear vista para consultas rápidas
-- =============================================

PRINT '';
PRINT 'Creando vistas...';

IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'vw_UsuariosConRol')
BEGIN
    EXEC('
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
    INNER JOIN Rol r ON u.RolId = r.RolId
    ');
    PRINT '✓ Vista vw_UsuariosConRol creada.';
END
ELSE
BEGIN
    PRINT '⚠ Vista vw_UsuariosConRol ya existe.';
END
GO

-- =============================================
-- PASO 6: Verificación de la implementación
-- =============================================

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACIÓN DE LA IMPLEMENTACIÓN';
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
AND COLUMN_NAME IN ('UsuarioId', 'NombreUsuario', 'RolId');
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

PRINT '========================================';
PRINT '✓ SISTEMA DE ROLES IMPLEMENTADO EXITOSAMENTE';
PRINT '========================================';
PRINT '';
PRINT 'SIGUIENTE PASO: Ejecutar el script 08_CreateSuperAdmin.sql';
PRINT 'para crear el usuario administrador único.';
PRINT '';

GO
