-- =============================================
-- Script de Creación de Base de Datos
-- Sistema de Control de Gastos
-- =============================================

USE master;
GO

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ControlGastosDB')
BEGIN
    CREATE DATABASE ControlGastosDB;
    PRINT 'Base de datos ControlGastosDB creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La base de datos ControlGastosDB ya existe.';
END
GO

USE ControlGastosDB;
GO

-- =============================================
-- Tabla: TipoGasto
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TipoGasto]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[TipoGasto](
        [TipoGastoId] INT IDENTITY(1,1) NOT NULL,
        [Codigo] VARCHAR(20) NOT NULL,
        [Descripcion] NVARCHAR(200) NOT NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] DATETIME NULL,
        CONSTRAINT [PK_TipoGasto] PRIMARY KEY CLUSTERED ([TipoGastoId] ASC),
        CONSTRAINT [UK_TipoGasto_Codigo] UNIQUE ([Codigo])
    );
    PRINT 'Tabla TipoGasto creada.';
END
GO

-- =============================================
-- Tabla: FondoMonetario
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FondoMonetario]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FondoMonetario](
        [FondoMonetarioId] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(200) NOT NULL,
        [TipoFondo] NVARCHAR(50) NOT NULL, -- 'Cuenta Bancaria', 'Caja Menuda'
        [Descripcion] NVARCHAR(500) NULL,
        [SaldoActual] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] DATETIME NULL,
        CONSTRAINT [PK_FondoMonetario] PRIMARY KEY CLUSTERED ([FondoMonetarioId] ASC)
    );
    PRINT 'Tabla FondoMonetario creada.';
END
GO

-- =============================================
-- Tabla: Presupuesto
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Presupuesto]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Presupuesto](
        [PresupuestoId] INT IDENTITY(1,1) NOT NULL,
        [TipoGastoId] INT NOT NULL,
        [Mes] INT NOT NULL, -- 1-12
        [Anio] INT NOT NULL,
        [MontoPresupuestado] DECIMAL(18,2) NOT NULL,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] DATETIME NULL,
        CONSTRAINT [PK_Presupuesto] PRIMARY KEY CLUSTERED ([PresupuestoId] ASC),
        CONSTRAINT [FK_Presupuesto_TipoGasto] FOREIGN KEY([TipoGastoId])
            REFERENCES [dbo].[TipoGasto] ([TipoGastoId]),
        CONSTRAINT [UK_Presupuesto_TipoMesAnio] UNIQUE ([TipoGastoId], [Mes], [Anio])
    );
    PRINT 'Tabla Presupuesto creada.';
END
GO

-- =============================================
-- Tabla: RegistroGastoEncabezado
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegistroGastoEncabezado]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RegistroGastoEncabezado](
        [RegistroGastoId] INT IDENTITY(1,1) NOT NULL,
        [Fecha] DATE NOT NULL,
        [FondoMonetarioId] INT NOT NULL,
        [NombreComercio] NVARCHAR(200) NOT NULL,
        [TipoDocumento] NVARCHAR(50) NOT NULL, -- 'Comprobante', 'Factura', 'Otro'
        [Observaciones] NVARCHAR(500) NULL,
        [MontoTotal] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] DATETIME NULL,
        CONSTRAINT [PK_RegistroGastoEncabezado] PRIMARY KEY CLUSTERED ([RegistroGastoId] ASC),
        CONSTRAINT [FK_RegistroGasto_FondoMonetario] FOREIGN KEY([FondoMonetarioId])
            REFERENCES [dbo].[FondoMonetario] ([FondoMonetarioId])
    );
    PRINT 'Tabla RegistroGastoEncabezado creada.';
END
GO

-- =============================================
-- Tabla: RegistroGastoDetalle
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegistroGastoDetalle]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RegistroGastoDetalle](
        [RegistroGastoDetalleId] INT IDENTITY(1,1) NOT NULL,
        [RegistroGastoId] INT NOT NULL,
        [TipoGastoId] INT NOT NULL,
        [Monto] DECIMAL(18,2) NOT NULL,
        [Descripcion] NVARCHAR(300) NULL,
        CONSTRAINT [PK_RegistroGastoDetalle] PRIMARY KEY CLUSTERED ([RegistroGastoDetalleId] ASC),
        CONSTRAINT [FK_RegistroGastoDetalle_Encabezado] FOREIGN KEY([RegistroGastoId])
            REFERENCES [dbo].[RegistroGastoEncabezado] ([RegistroGastoId]) ON DELETE CASCADE,
        CONSTRAINT [FK_RegistroGastoDetalle_TipoGasto] FOREIGN KEY([TipoGastoId])
            REFERENCES [dbo].[TipoGasto] ([TipoGastoId])
    );
    PRINT 'Tabla RegistroGastoDetalle creada.';
END
GO

-- =============================================
-- Tabla: Deposito
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Deposito]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Deposito](
        [DepositoId] INT IDENTITY(1,1) NOT NULL,
        [Fecha] DATE NOT NULL,
        [FondoMonetarioId] INT NOT NULL,
        [Monto] DECIMAL(18,2) NOT NULL,
        [Descripcion] NVARCHAR(300) NULL,
        [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
        [FechaModificacion] DATETIME NULL,
        CONSTRAINT [PK_Deposito] PRIMARY KEY CLUSTERED ([DepositoId] ASC),
        CONSTRAINT [FK_Deposito_FondoMonetario] FOREIGN KEY([FondoMonetarioId])
            REFERENCES [dbo].[FondoMonetario] ([FondoMonetarioId])
    );
    PRINT 'Tabla Deposito creada.';
END
GO

-- =============================================
-- Índices adicionales para mejorar el rendimiento
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Presupuesto_MesAnio' AND object_id = OBJECT_ID('Presupuesto'))
BEGIN
    CREATE INDEX IX_Presupuesto_MesAnio ON Presupuesto(Mes, Anio);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RegistroGasto_Fecha' AND object_id = OBJECT_ID('RegistroGastoEncabezado'))
BEGIN
    CREATE INDEX IX_RegistroGasto_Fecha ON RegistroGastoEncabezado(Fecha);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Deposito_Fecha' AND object_id = OBJECT_ID('Deposito'))
BEGIN
    CREATE INDEX IX_Deposito_Fecha ON Deposito(Fecha);
END

PRINT 'Índices creados exitosamente.';
GO

PRINT '========================================';
PRINT 'Base de datos creada exitosamente.';
PRINT '========================================';
