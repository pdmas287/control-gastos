-- Script SQL para PostgreSQL (Supabase)
-- Sistema de Control de Gastos

-- =====================================================
-- Tabla: Usuario
-- =====================================================
CREATE TABLE Usuario (
    UsuarioId SERIAL PRIMARY KEY,
    NombreCompleto VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(256) NOT NULL,
    FechaCreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    Activo BOOLEAN DEFAULT TRUE
);

-- =====================================================
-- Tabla: Rol
-- =====================================================
CREATE TABLE Rol (
    RolId SERIAL PRIMARY KEY,
    Nombre VARCHAR(50) UNIQUE NOT NULL,
    Descripcion VARCHAR(200)
);

-- =====================================================
-- Tabla: UsuarioRoles (Relación Many-to-Many)
-- =====================================================
CREATE TABLE UsuarioRoles (
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId) ON DELETE CASCADE,
    RolId INTEGER REFERENCES Rol(RolId) ON DELETE CASCADE,
    PRIMARY KEY (UsuarioId, RolId)
);

-- =====================================================
-- Tabla: TipoGasto
-- =====================================================
CREATE TABLE TipoGasto (
    TipoGastoId SERIAL PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(200),
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId) ON DELETE CASCADE,
    Activo BOOLEAN DEFAULT TRUE
);

-- =====================================================
-- Tabla: FondoMonetario
-- =====================================================
CREATE TABLE FondoMonetario (
    FondoId SERIAL PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(200),
    SaldoInicial DECIMAL(18,2) DEFAULT 0,
    SaldoActual DECIMAL(18,2) DEFAULT 0,
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId) ON DELETE CASCADE,
    Activo BOOLEAN DEFAULT TRUE
);

-- =====================================================
-- Tabla: Presupuesto
-- =====================================================
CREATE TABLE Presupuesto (
    PresupuestoId SERIAL PRIMARY KEY,
    TipoGastoId INTEGER REFERENCES TipoGasto(TipoGastoId) ON DELETE CASCADE,
    Monto DECIMAL(18,2) NOT NULL,
    Periodo VARCHAR(20),
    FechaInicio DATE,
    FechaFin DATE,
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId) ON DELETE CASCADE
);

-- =====================================================
-- Tabla: RegistroGasto
-- =====================================================
CREATE TABLE RegistroGasto (
    RegistroGastoId SERIAL PRIMARY KEY,
    TipoGastoId INTEGER REFERENCES TipoGasto(TipoGastoId) ON DELETE CASCADE,
    FondoId INTEGER REFERENCES FondoMonetario(FondoId) ON DELETE CASCADE,
    Monto DECIMAL(18,2) NOT NULL,
    Descripcion VARCHAR(200),
    Fecha DATE NOT NULL,
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId) ON DELETE CASCADE
);

-- =====================================================
-- Tabla: Deposito
-- =====================================================
CREATE TABLE Deposito (
    DepositoId SERIAL PRIMARY KEY,
    FondoId INTEGER REFERENCES FondoMonetario(FondoId) ON DELETE CASCADE,
    Monto DECIMAL(18,2) NOT NULL,
    Descripcion VARCHAR(200),
    Fecha DATE NOT NULL,
    UsuarioId INTEGER REFERENCES Usuario(UsuarioId) ON DELETE CASCADE
);

-- =====================================================
-- Índices para mejorar el rendimiento
-- =====================================================

-- Índices para búsquedas por usuario
CREATE INDEX idx_tipogasto_usuario ON TipoGasto(UsuarioId);
CREATE INDEX idx_fondomonetario_usuario ON FondoMonetario(UsuarioId);
CREATE INDEX idx_presupuesto_usuario ON Presupuesto(UsuarioId);
CREATE INDEX idx_registrogasto_usuario ON RegistroGasto(UsuarioId);
CREATE INDEX idx_deposito_usuario ON Deposito(UsuarioId);

-- Índices para búsquedas por fecha
CREATE INDEX idx_registrogasto_fecha ON RegistroGasto(Fecha);
CREATE INDEX idx_deposito_fecha ON Deposito(Fecha);
CREATE INDEX idx_presupuesto_fechas ON Presupuesto(FechaInicio, FechaFin);

-- Índice para búsquedas por email
CREATE INDEX idx_usuario_email ON Usuario(Email);

-- =====================================================
-- Datos iniciales: Roles
-- =====================================================
INSERT INTO Rol (Nombre, Descripcion) VALUES
('Administrador', 'Acceso completo al sistema'),
('Usuario', 'Usuario estándar con acceso limitado');

-- =====================================================
-- Datos iniciales: Usuario Administrador
-- =====================================================
-- Email: admin@example.com
-- Password: Admin123! (hash SHA256)
-- IMPORTANTE: Cambiar este password después del primer login
INSERT INTO Usuario (NombreCompleto, Email, PasswordHash, Activo)
VALUES (
    'Administrador del Sistema',
    'admin@example.com',
    '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',
    TRUE
);

-- Asignar rol de Administrador al usuario admin
INSERT INTO UsuarioRoles (UsuarioId, RolId)
SELECT u.UsuarioId, r.RolId
FROM Usuario u, Rol r
WHERE u.Email = 'admin@example.com' AND r.Nombre = 'Administrador';

-- =====================================================
-- Vistas útiles para reportes
-- =====================================================

-- Vista: Resumen de gastos por usuario
CREATE OR REPLACE VIEW vista_resumen_gastos_usuario AS
SELECT
    u.UsuarioId,
    u.NombreCompleto,
    u.Email,
    COUNT(rg.RegistroGastoId) AS TotalGastos,
    COALESCE(SUM(rg.Monto), 0) AS MontoTotal
FROM Usuario u
LEFT JOIN RegistroGasto rg ON u.UsuarioId = rg.UsuarioId
GROUP BY u.UsuarioId, u.NombreCompleto, u.Email;

-- Vista: Resumen de fondos por usuario
CREATE OR REPLACE VIEW vista_resumen_fondos_usuario AS
SELECT
    u.UsuarioId,
    u.NombreCompleto,
    COUNT(fm.FondoId) AS TotalFondos,
    COALESCE(SUM(fm.SaldoActual), 0) AS SaldoTotalActual
FROM Usuario u
LEFT JOIN FondoMonetario fm ON u.UsuarioId = fm.UsuarioId
WHERE fm.Activo = TRUE
GROUP BY u.UsuarioId, u.NombreCompleto;

-- =====================================================
-- Funciones útiles
-- =====================================================

-- Función: Actualizar saldo de fondo después de registrar gasto
CREATE OR REPLACE FUNCTION actualizar_saldo_fondo_gasto()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE FondoMonetario
    SET SaldoActual = SaldoActual - NEW.Monto
    WHERE FondoId = NEW.FondoId;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Función: Actualizar saldo de fondo después de hacer depósito
CREATE OR REPLACE FUNCTION actualizar_saldo_fondo_deposito()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE FondoMonetario
    SET SaldoActual = SaldoActual + NEW.Monto
    WHERE FondoId = NEW.FondoId;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- Triggers
-- =====================================================

-- Trigger: Actualizar saldo al registrar gasto
CREATE TRIGGER trigger_actualizar_saldo_gasto
AFTER INSERT ON RegistroGasto
FOR EACH ROW
EXECUTE FUNCTION actualizar_saldo_fondo_gasto();

-- Trigger: Actualizar saldo al hacer depósito
CREATE TRIGGER trigger_actualizar_saldo_deposito
AFTER INSERT ON Deposito
FOR EACH ROW
EXECUTE FUNCTION actualizar_saldo_fondo_deposito();

-- =====================================================
-- Comentarios en tablas (documentación)
-- =====================================================
COMMENT ON TABLE Usuario IS 'Almacena información de usuarios del sistema';
COMMENT ON TABLE Rol IS 'Define los roles disponibles en el sistema';
COMMENT ON TABLE UsuarioRoles IS 'Relación entre usuarios y sus roles asignados';
COMMENT ON TABLE TipoGasto IS 'Categorías de gastos definidas por los usuarios';
COMMENT ON TABLE FondoMonetario IS 'Fondos monetarios o cuentas de los usuarios';
COMMENT ON TABLE Presupuesto IS 'Presupuestos asignados por tipo de gasto';
COMMENT ON TABLE RegistroGasto IS 'Registro histórico de gastos realizados';
COMMENT ON TABLE Deposito IS 'Registro histórico de depósitos a fondos';

-- =====================================================
-- Script completado
-- =====================================================
-- Verifica que todo se haya creado correctamente:
SELECT 'Tablas creadas:' AS mensaje;
SELECT tablename FROM pg_tables WHERE schemaname = 'public';

SELECT 'Roles insertados:' AS mensaje;
SELECT * FROM Rol;

SELECT 'Usuario admin creado:' AS mensaje;
SELECT UsuarioId, NombreCompleto, Email, Activo FROM Usuario;
