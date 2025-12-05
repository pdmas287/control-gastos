-- =============================================
-- Stored Procedures y Funciones
-- Sistema de Control de Gastos
-- =============================================

USE ControlGastosDB;
GO

-- =============================================
-- SP: Obtener siguiente código de Tipo de Gasto
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ObtenerSiguienteCodigoTipoGasto')
    DROP PROCEDURE sp_ObtenerSiguienteCodigoTipoGasto;
GO

CREATE PROCEDURE sp_ObtenerSiguienteCodigoTipoGasto
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UltimoCodigo VARCHAR(20);
    DECLARE @NumeroActual INT;
    DECLARE @SiguienteCodigo VARCHAR(20);

    SELECT TOP 1 @UltimoCodigo = Codigo
    FROM TipoGasto
    ORDER BY TipoGastoId DESC;

    IF @UltimoCodigo IS NULL
    BEGIN
        SET @SiguienteCodigo = 'TG-001';
    END
    ELSE
    BEGIN
        -- Extraer el número del código (asumiendo formato TG-XXX)
        SET @NumeroActual = CAST(SUBSTRING(@UltimoCodigo, 4, LEN(@UltimoCodigo)) AS INT);
        SET @SiguienteCodigo = 'TG-' + RIGHT('000' + CAST(@NumeroActual + 1 AS VARCHAR), 3);
    END

    SELECT @SiguienteCodigo AS SiguienteCodigo;
END
GO

-- =============================================
-- SP: Validar Sobregiro de Presupuesto
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ValidarSobregiroPresupuesto')
    DROP PROCEDURE sp_ValidarSobregiroPresupuesto;
GO

CREATE PROCEDURE sp_ValidarSobregiroPresupuesto
    @Mes INT,
    @Anio INT,
    @TipoGastoId INT,
    @MontoGasto DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MontoPresupuestado DECIMAL(18,2);
    DECLARE @TotalEjecutado DECIMAL(18,2);
    DECLARE @NuevoTotal DECIMAL(18,2);
    DECLARE @Sobregiro DECIMAL(18,2);

    -- Obtener el presupuesto
    SELECT @MontoPresupuestado = MontoPresupuestado
    FROM Presupuesto
    WHERE TipoGastoId = @TipoGastoId
      AND Mes = @Mes
      AND Anio = @Anio;

    -- Si no hay presupuesto definido
    IF @MontoPresupuestado IS NULL
    BEGIN
        SELECT
            0 AS HaySobregiro,
            @TipoGastoId AS TipoGastoId,
            0 AS MontoPresupuestado,
            0 AS MontoEjecutado,
            0 AS MontoSobregiro;
        RETURN;
    END

    -- Calcular total ejecutado en el mes
    SELECT @TotalEjecutado = ISNULL(SUM(d.Monto), 0)
    FROM RegistroGastoDetalle d
    INNER JOIN RegistroGastoEncabezado e ON d.RegistroGastoId = e.RegistroGastoId
    WHERE d.TipoGastoId = @TipoGastoId
      AND MONTH(e.Fecha) = @Mes
      AND YEAR(e.Fecha) = @Anio;

    SET @NuevoTotal = @TotalEjecutado + @MontoGasto;

    -- Validar sobregiro
    IF @NuevoTotal > @MontoPresupuestado
    BEGIN
        SET @Sobregiro = @NuevoTotal - @MontoPresupuestado;
        SELECT
            1 AS HaySobregiro,
            @TipoGastoId AS TipoGastoId,
            @MontoPresupuestado AS MontoPresupuestado,
            @NuevoTotal AS MontoEjecutado,
            @Sobregiro AS MontoSobregiro;
    END
    ELSE
    BEGIN
        SELECT
            0 AS HaySobregiro,
            @TipoGastoId AS TipoGastoId,
            @MontoPresupuestado AS MontoPresupuestado,
            @NuevoTotal AS MontoEjecutado,
            0 AS MontoSobregiro;
    END
END
GO

-- =============================================
-- SP: Obtener Movimientos por Rango de Fechas
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ObtenerMovimientos')
    DROP PROCEDURE sp_ObtenerMovimientos;
GO

CREATE PROCEDURE sp_ObtenerMovimientos
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Movimientos de Gastos
    SELECT
        e.Fecha,
        'Gasto' AS TipoMovimiento,
        f.Nombre AS FondoMonetario,
        e.NombreComercio AS Descripcion,
        e.MontoTotal AS Monto,
        e.TipoDocumento,
        e.Observaciones
    FROM RegistroGastoEncabezado e
    INNER JOIN FondoMonetario f ON e.FondoMonetarioId = f.FondoMonetarioId
    WHERE e.Fecha BETWEEN @FechaInicio AND @FechaFin

    UNION ALL

    -- Movimientos de Depósitos
    SELECT
        d.Fecha,
        'Depósito' AS TipoMovimiento,
        f.Nombre AS FondoMonetario,
        ISNULL(d.Descripcion, 'Depósito') AS Descripcion,
        d.Monto,
        NULL AS TipoDocumento,
        NULL AS Observaciones
    FROM Deposito d
    INNER JOIN FondoMonetario f ON d.FondoMonetarioId = f.FondoMonetarioId
    WHERE d.Fecha BETWEEN @FechaInicio AND @FechaFin

    ORDER BY Fecha DESC;
END
GO

-- =============================================
-- SP: Obtener Comparativo Presupuesto vs Ejecución
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ComparativoPresupuestoEjecucion')
    DROP PROCEDURE sp_ComparativoPresupuestoEjecucion;
GO

CREATE PROCEDURE sp_ComparativoPresupuestoEjecucion
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MesInicio INT = MONTH(@FechaInicio);
    DECLARE @AnioInicio INT = YEAR(@FechaInicio);
    DECLARE @MesFin INT = MONTH(@FechaFin);
    DECLARE @AnioFin INT = YEAR(@FechaFin);

    SELECT
        tg.Descripcion AS TipoGasto,
        ISNULL(p.MontoPresupuestado, 0) AS MontoPresupuestado,
        ISNULL(SUM(d.Monto), 0) AS MontoEjecutado,
        ISNULL(p.MontoPresupuestado, 0) - ISNULL(SUM(d.Monto), 0) AS Diferencia
    FROM TipoGasto tg
    LEFT JOIN Presupuesto p ON tg.TipoGastoId = p.TipoGastoId
        AND ((p.Anio = @AnioInicio AND p.Mes >= @MesInicio)
             OR (p.Anio = @AnioFin AND p.Mes <= @MesFin)
             OR (p.Anio > @AnioInicio AND p.Anio < @AnioFin))
    LEFT JOIN RegistroGastoDetalle d ON tg.TipoGastoId = d.TipoGastoId
    LEFT JOIN RegistroGastoEncabezado e ON d.RegistroGastoId = e.RegistroGastoId
        AND e.Fecha BETWEEN @FechaInicio AND @FechaFin
    WHERE tg.Activo = 1
    GROUP BY tg.TipoGastoId, tg.Descripcion, p.MontoPresupuestado
    ORDER BY tg.Descripcion;
END
GO

-- =============================================
-- TRIGGER: Actualizar Saldo de Fondo Monetario
-- =============================================
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'trg_ActualizarSaldoFondo_Deposito')
    DROP TRIGGER trg_ActualizarSaldoFondo_Deposito;
GO

CREATE TRIGGER trg_ActualizarSaldoFondo_Deposito
ON Deposito
AFTER INSERT, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Incrementar saldo por depósitos insertados
    IF EXISTS (SELECT * FROM inserted)
    BEGIN
        UPDATE FondoMonetario
        SET SaldoActual = SaldoActual + i.Monto
        FROM FondoMonetario f
        INNER JOIN inserted i ON f.FondoMonetarioId = i.FondoMonetarioId;
    END

    -- Decrementar saldo por depósitos eliminados
    IF EXISTS (SELECT * FROM deleted)
    BEGIN
        UPDATE FondoMonetario
        SET SaldoActual = SaldoActual - d.Monto
        FROM FondoMonetario f
        INNER JOIN deleted d ON f.FondoMonetarioId = d.FondoMonetarioId;
    END
END
GO

-- =============================================
-- TRIGGER: Actualizar Saldo de Fondo por Gastos
-- =============================================
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'trg_ActualizarSaldoFondo_Gasto')
    DROP TRIGGER trg_ActualizarSaldoFondo_Gasto;
GO

CREATE TRIGGER trg_ActualizarSaldoFondo_Gasto
ON RegistroGastoEncabezado
AFTER INSERT, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Decrementar saldo por gastos insertados
    IF EXISTS (SELECT * FROM inserted)
    BEGIN
        UPDATE FondoMonetario
        SET SaldoActual = SaldoActual - i.MontoTotal
        FROM FondoMonetario f
        INNER JOIN inserted i ON f.FondoMonetarioId = i.FondoMonetarioId;
    END

    -- Incrementar saldo por gastos eliminados
    IF EXISTS (SELECT * FROM deleted)
    BEGIN
        UPDATE FondoMonetario
        SET SaldoActual = SaldoActual + d.MontoTotal
        FROM FondoMonetario f
        INNER JOIN deleted d ON f.FondoMonetarioId = d.FondoMonetarioId;
    END
END
GO

PRINT '========================================';
PRINT 'Stored Procedures y Triggers creados exitosamente.';
PRINT '========================================';
