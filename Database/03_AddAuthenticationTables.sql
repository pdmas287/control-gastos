-- =============================================
-- Script de Autenticación y Multi-Usuario
-- Sistema de Control de Gastos
-- =============================================

USE ControlGastosDB;
GO

-- =============================================
-- Tabla: Usuario
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuario]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Usuario](
        [UsuarioId] INT IDENTITY(1,1) NOT NULL,
        [NombreUsuario] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [PasswordHash] NVARCHAR(500) NOT NULL,
        [NombreCompleto] NVARCHAR(200) NOT NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] DATETIME NULL,
        [UltimoAcceso] DATETIME NULL,
        CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED ([UsuarioId] ASC),
        CONSTRAINT [UK_Usuario_NombreUsuario] UNIQUE ([NombreUsuario]),
        CONSTRAINT [UK_Usuario_Email] UNIQUE ([Email])
    );
    PRINT 'Tabla Usuario creada.';
END
GO

-- =============================================
-- Agregar columna UsuarioId a tablas existentes
-- =============================================

-- TipoGasto
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TipoGasto') AND name = 'UsuarioId')
BEGIN
    ALTER TABLE [dbo].[TipoGasto]
    ADD [UsuarioId] INT NULL;

    ALTER TABLE [dbo].[TipoGasto]
    ADD CONSTRAINT [FK_TipoGasto_Usuario] FOREIGN KEY([UsuarioId])
        REFERENCES [dbo].[Usuario] ([UsuarioId]);

    PRINT 'Columna UsuarioId agregada a TipoGasto.';
END
GO

-- FondoMonetario
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FondoMonetario') AND name = 'UsuarioId')
BEGIN
    ALTER TABLE [dbo].[FondoMonetario]
    ADD [UsuarioId] INT NULL;

    ALTER TABLE [dbo].[FondoMonetario]
    ADD CONSTRAINT [FK_FondoMonetario_Usuario] FOREIGN KEY([UsuarioId])
        REFERENCES [dbo].[Usuario] ([UsuarioId]);

    PRINT 'Columna UsuarioId agregada a FondoMonetario.';
END
GO

-- Presupuesto
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Presupuesto') AND name = 'UsuarioId')
BEGIN
    ALTER TABLE [dbo].[Presupuesto]
    ADD [UsuarioId] INT NULL;

    ALTER TABLE [dbo].[Presupuesto]
    ADD CONSTRAINT [FK_Presupuesto_Usuario] FOREIGN KEY([UsuarioId])
        REFERENCES [dbo].[Usuario] ([UsuarioId]);

    PRINT 'Columna UsuarioId agregada a Presupuesto.';
END
GO

-- RegistroGastoEncabezado
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('RegistroGastoEncabezado') AND name = 'UsuarioId')
BEGIN
    ALTER TABLE [dbo].[RegistroGastoEncabezado]
    ADD [UsuarioId] INT NULL;

    ALTER TABLE [dbo].[RegistroGastoEncabezado]
    ADD CONSTRAINT [FK_RegistroGastoEncabezado_Usuario] FOREIGN KEY([UsuarioId])
        REFERENCES [dbo].[Usuario] ([UsuarioId]);

    PRINT 'Columna UsuarioId agregada a RegistroGastoEncabezado.';
END
GO

-- Deposito
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Deposito') AND name = 'UsuarioId')
BEGIN
    ALTER TABLE [dbo].[Deposito]
    ADD [UsuarioId] INT NULL;

    ALTER TABLE [dbo].[Deposito]
    ADD CONSTRAINT [FK_Deposito_Usuario] FOREIGN KEY([UsuarioId])
        REFERENCES [dbo].[Usuario] ([UsuarioId]);

    PRINT 'Columna UsuarioId agregada a Deposito.';
END
GO

-- =============================================
-- Modificar constraint UNIQUE de Presupuesto para incluir UsuarioId
-- =============================================
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UK_Presupuesto_TipoMesAnio' AND object_id = OBJECT_ID('Presupuesto'))
BEGIN
    ALTER TABLE [dbo].[Presupuesto] DROP CONSTRAINT [UK_Presupuesto_TipoMesAnio];
    PRINT 'Constraint UK_Presupuesto_TipoMesAnio eliminado.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UK_Presupuesto_UsuarioTipoMesAnio' AND object_id = OBJECT_ID('Presupuesto'))
BEGIN
    ALTER TABLE [dbo].[Presupuesto]
    ADD CONSTRAINT [UK_Presupuesto_UsuarioTipoMesAnio] UNIQUE ([UsuarioId], [TipoGastoId], [Mes], [Anio]);
    PRINT 'Constraint UK_Presupuesto_UsuarioTipoMesAnio creado.';
END
GO

-- =============================================
-- Modificar constraint UNIQUE de TipoGasto para incluir UsuarioId
-- =============================================
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UK_TipoGasto_Codigo' AND object_id = OBJECT_ID('TipoGasto'))
BEGIN
    ALTER TABLE [dbo].[TipoGasto] DROP CONSTRAINT [UK_TipoGasto_Codigo];
    PRINT 'Constraint UK_TipoGasto_Codigo eliminado.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UK_TipoGasto_UsuarioCodigo' AND object_id = OBJECT_ID('TipoGasto'))
BEGIN
    ALTER TABLE [dbo].[TipoGasto]
    ADD CONSTRAINT [UK_TipoGasto_UsuarioCodigo] UNIQUE ([UsuarioId], [Codigo]);
    PRINT 'Constraint UK_TipoGasto_UsuarioCodigo creado.';
END
GO

-- =============================================
-- Índices adicionales para mejorar el rendimiento
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TipoGasto_UsuarioId' AND object_id = OBJECT_ID('TipoGasto'))
BEGIN
    CREATE INDEX IX_TipoGasto_UsuarioId ON TipoGasto(UsuarioId);
    PRINT 'Índice IX_TipoGasto_UsuarioId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FondoMonetario_UsuarioId' AND object_id = OBJECT_ID('FondoMonetario'))
BEGIN
    CREATE INDEX IX_FondoMonetario_UsuarioId ON FondoMonetario(UsuarioId);
    PRINT 'Índice IX_FondoMonetario_UsuarioId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Presupuesto_UsuarioId' AND object_id = OBJECT_ID('Presupuesto'))
BEGIN
    CREATE INDEX IX_Presupuesto_UsuarioId ON Presupuesto(UsuarioId);
    PRINT 'Índice IX_Presupuesto_UsuarioId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RegistroGastoEncabezado_UsuarioId' AND object_id = OBJECT_ID('RegistroGastoEncabezado'))
BEGIN
    CREATE INDEX IX_RegistroGastoEncabezado_UsuarioId ON RegistroGastoEncabezado(UsuarioId);
    PRINT 'Índice IX_RegistroGastoEncabezado_UsuarioId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Deposito_UsuarioId' AND object_id = OBJECT_ID('Deposito'))
BEGIN
    CREATE INDEX IX_Deposito_UsuarioId ON Deposito(UsuarioId);
    PRINT 'Índice IX_Deposito_UsuarioId creado.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuario_Email' AND object_id = OBJECT_ID('Usuario'))
BEGIN
    CREATE INDEX IX_Usuario_Email ON Usuario(Email);
    PRINT 'Índice IX_Usuario_Email creado.';
END

PRINT '========================================';
PRINT 'Tablas de autenticación creadas exitosamente.';
PRINT 'IMPORTANTE: Ejecutar este script en una base de datos vacía o de prueba primero.';
PRINT 'Para bases de datos con datos existentes, asignar usuarios manualmente.';
PRINT '========================================';
GO
