-- Schema SQL correcto para PostgreSQL (Supabase)
-- Sistema de Control de Gastos

-- =====================================================
-- Tabla: Rol
-- =====================================================
CREATE TABLE rol (
    rolid SERIAL PRIMARY KEY,
    nombre VARCHAR(50) UNIQUE NOT NULL,
    descripcion VARCHAR(200),
    activo BOOLEAN DEFAULT TRUE,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =====================================================
-- Tabla: Usuario
-- =====================================================
CREATE TABLE usuario (
    usuarioid SERIAL PRIMARY KEY,
    nombreusuario VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    passwordhash VARCHAR(500) NOT NULL,
    nombrecompleto VARCHAR(200) NOT NULL,
    activo BOOLEAN DEFAULT TRUE,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    ultimoacceso TIMESTAMP NULL,
    rolid INTEGER REFERENCES rol(rolid)
);

-- =====================================================
-- Tabla: TipoGasto
-- =====================================================
CREATE TABLE tipogasto (
    tipogastoid SERIAL PRIMARY KEY,
    codigo VARCHAR(20) NOT NULL,
    descripcion VARCHAR(200) NOT NULL,
    activo BOOLEAN DEFAULT TRUE,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    usuarioid INTEGER REFERENCES usuario(usuarioid)
);

-- =====================================================
-- Tabla: FondoMonetario
-- =====================================================
CREATE TABLE fondomonetario (
    fondomonetarioid SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    tipofondo VARCHAR(50) NOT NULL,
    descripcion VARCHAR(500) NULL,
    saldoactual DECIMAL(18,2) NOT NULL DEFAULT 0,
    activo BOOLEAN DEFAULT TRUE,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    usuarioid INTEGER REFERENCES usuario(usuarioid)
);

-- =====================================================
-- Tabla: Presupuesto
-- =====================================================
CREATE TABLE presupuesto (
    presupuestoid SERIAL PRIMARY KEY,
    tipogastoid INTEGER NOT NULL REFERENCES tipogasto(tipogastoid),
    mes INTEGER NOT NULL,
    anio INTEGER NOT NULL,
    montopresupuestado DECIMAL(18,2) NOT NULL,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    usuarioid INTEGER REFERENCES usuario(usuarioid)
);

-- =====================================================
-- Tabla: RegistroGastoEncabezado
-- =====================================================
CREATE TABLE registrogastoencabezado (
    registrogastoid SERIAL PRIMARY KEY,
    fecha DATE NOT NULL,
    fondomonetarioid INTEGER NOT NULL REFERENCES fondomonetario(fondomonetarioid),
    nombrecomercio VARCHAR(200) NOT NULL,
    tipodocumento VARCHAR(50) NOT NULL,
    observaciones VARCHAR(500) NULL,
    montototal DECIMAL(18,2) NOT NULL DEFAULT 0,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    usuarioid INTEGER REFERENCES usuario(usuarioid)
);

-- =====================================================
-- Tabla: RegistroGastoDetalle
-- =====================================================
CREATE TABLE registrogastodetalle (
    registrogastodetalleid SERIAL PRIMARY KEY,
    registrogastoid INTEGER NOT NULL REFERENCES registrogastoencabezado(registrogastoid) ON DELETE CASCADE,
    tipogastoid INTEGER NOT NULL REFERENCES tipogasto(tipogastoid),
    monto DECIMAL(18,2) NOT NULL,
    descripcion VARCHAR(300) NULL
);

-- =====================================================
-- Tabla: Deposito
-- =====================================================
CREATE TABLE deposito (
    depositoid SERIAL PRIMARY KEY,
    fecha DATE NOT NULL,
    fondomonetarioid INTEGER NOT NULL REFERENCES fondomonetario(fondomonetarioid),
    monto DECIMAL(18,2) NOT NULL,
    descripcion VARCHAR(300) NULL,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    usuarioid INTEGER REFERENCES usuario(usuarioid)
);

-- =====================================================
-- Índices para mejorar el rendimiento
-- =====================================================

-- Índices para búsquedas por usuario
CREATE INDEX idx_tipogasto_usuario ON tipogasto(usuarioid);
CREATE INDEX idx_fondomonetario_usuario ON fondomonetario(usuarioid);
CREATE INDEX idx_presupuesto_usuario ON presupuesto(usuarioid);
CREATE INDEX idx_registrogasto_usuario ON registrogastoencabezado(usuarioid);
CREATE INDEX idx_deposito_usuario ON deposito(usuarioid);

-- Índices para búsquedas por fecha
CREATE INDEX idx_registrogasto_fecha ON registrogastoencabezado(fecha);
CREATE INDEX idx_deposito_fecha ON deposito(fecha);
CREATE INDEX idx_presupuesto_mes_anio ON presupuesto(mes, anio);

-- Índices únicos
CREATE UNIQUE INDEX idx_usuario_nombreusuario ON usuario(nombreusuario);
CREATE UNIQUE INDEX idx_usuario_email ON usuario(email);
CREATE UNIQUE INDEX idx_rol_nombre ON rol(nombre);
CREATE UNIQUE INDEX idx_tipogasto_usuario_codigo ON tipogasto(usuarioid, codigo);
CREATE UNIQUE INDEX idx_presupuesto_usuario_mes_anio ON presupuesto(usuarioid, tipogastoid, mes, anio);

-- =====================================================
-- Datos iniciales: Roles
-- =====================================================
INSERT INTO rol (nombre, descripcion, activo) VALUES
('Administrador', 'Acceso completo al sistema', TRUE),
('Usuario', 'Usuario estándar con acceso limitado', TRUE);

-- =====================================================
-- Datos iniciales: Usuario Administrador
-- =====================================================
-- Email: admin@example.com
-- Password: Admin123! (hash SHA256)
-- IMPORTANTE: Cambiar este password después del primer login
INSERT INTO usuario (nombreusuario, email, passwordhash, nombrecompleto, activo, rolid)
VALUES (
    'admin',
    'admin@example.com',
    'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrUSqRg=',
    'Administrador del Sistema',
    TRUE,
    (SELECT rolid FROM rol WHERE nombre = 'Administrador')
);

-- =====================================================
-- Funciones útiles
-- =====================================================

-- Función: Actualizar saldo de fondo después de registrar gasto
CREATE OR REPLACE FUNCTION actualizar_saldo_fondo_gasto()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE fondomonetario
    SET saldoactual = saldoactual - NEW.monto
    WHERE fondomonetarioid = NEW.fondomonetarioid;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Función: Actualizar saldo de fondo después de hacer depósito
CREATE OR REPLACE FUNCTION actualizar_saldo_fondo_deposito()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE fondomonetario
    SET saldoactual = saldoactual + NEW.monto
    WHERE fondomonetarioid = NEW.fondomonetarioid;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- =====================================================
-- Triggers
-- =====================================================

-- Trigger: Actualizar saldo al registrar gasto
CREATE TRIGGER trigger_actualizar_saldo_gasto
AFTER INSERT ON registrogastodetalle
FOR EACH ROW
EXECUTE FUNCTION actualizar_saldo_fondo_gasto();

-- Trigger: Actualizar saldo al hacer depósito
CREATE TRIGGER trigger_actualizar_saldo_deposito
AFTER INSERT ON deposito
FOR EACH ROW
EXECUTE FUNCTION actualizar_saldo_fondo_deposito();

-- =====================================================
-- Comentarios en tablas (documentación)
-- =====================================================
COMMENT ON TABLE usuario IS 'Almacena información de usuarios del sistema';
COMMENT ON TABLE rol IS 'Define los roles disponibles en el sistema';
COMMENT ON TABLE tipogasto IS 'Categorías de gastos definidas por los usuarios';
COMMENT ON TABLE fondomonetario IS 'Fondos monetarios o cuentas de los usuarios';
COMMENT ON TABLE presupuesto IS 'Presupuestos asignados por tipo de gasto';
COMMENT ON TABLE registrogastoencabezado IS 'Encabezados de registros de gastos';
COMMENT ON TABLE registrogastodetalle IS 'Detalles de registros de gastos';
COMMENT ON TABLE deposito IS 'Registro histórico de depósitos a fondos';