-- =====================================================
-- Script de Configuración Completa para Supabase
-- Sistema de Control de Gastos
-- =====================================================
-- INSTRUCCIONES:
-- 1. Ve a Supabase → Tu proyecto → SQL Editor
-- 2. Copia y pega todo este script
-- 3. Haz clic en "Run" para ejecutarlo
-- =====================================================

-- Eliminar tablas existentes si existen (opcional, descomentar si necesitas reiniciar)
-- DROP TABLE IF EXISTS deposito CASCADE;
-- DROP TABLE IF EXISTS registrogastodetalle CASCADE;
-- DROP TABLE IF EXISTS registrogastoencabezado CASCADE;
-- DROP TABLE IF EXISTS presupuesto CASCADE;
-- DROP TABLE IF EXISTS fondomonetario CASCADE;
-- DROP TABLE IF EXISTS tipogasto CASCADE;
-- DROP TABLE IF EXISTS usuario CASCADE;
-- DROP TABLE IF EXISTS rol CASCADE;

-- =====================================================
-- Tabla: rol
-- =====================================================
CREATE TABLE IF NOT EXISTS rol (
    rolid SERIAL PRIMARY KEY,
    nombre VARCHAR(50) UNIQUE NOT NULL,
    descripcion VARCHAR(200),
    activo BOOLEAN DEFAULT TRUE,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- =====================================================
-- Tabla: usuario
-- =====================================================
CREATE TABLE IF NOT EXISTS usuario (
    usuarioid SERIAL PRIMARY KEY,
    nombreusuario VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) UNIQUE NOT NULL,
    passwordhash VARCHAR(500) NOT NULL,
    nombrecompleto VARCHAR(200) NOT NULL,
    activo BOOLEAN DEFAULT TRUE,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    ultimoacceso TIMESTAMP NULL,
    rolid INTEGER NOT NULL REFERENCES rol(rolid)
);

-- =====================================================
-- Tabla: tipogasto
-- =====================================================
CREATE TABLE IF NOT EXISTS tipogasto (
    tipogastoid SERIAL PRIMARY KEY,
    codigo VARCHAR(20) NOT NULL,
    descripcion VARCHAR(200) NOT NULL,
    activo BOOLEAN DEFAULT TRUE,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    usuarioid INTEGER NOT NULL REFERENCES usuario(usuarioid)
);

-- =====================================================
-- Tabla: fondomonetario
-- =====================================================
CREATE TABLE IF NOT EXISTS fondomonetario (
    fondomonetarioid SERIAL PRIMARY KEY,
    nombre VARCHAR(200) NOT NULL,
    tipofondo VARCHAR(50) NOT NULL,
    descripcion VARCHAR(500) NULL,
    saldoactual DECIMAL(18,2) NOT NULL DEFAULT 0,
    activo BOOLEAN DEFAULT TRUE,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    usuarioid INTEGER NOT NULL REFERENCES usuario(usuarioid)
);

-- =====================================================
-- Tabla: presupuesto
-- =====================================================
CREATE TABLE IF NOT EXISTS presupuesto (
    presupuestoid SERIAL PRIMARY KEY,
    tipogastoid INTEGER NOT NULL REFERENCES tipogasto(tipogastoid),
    mes INTEGER NOT NULL,
    anio INTEGER NOT NULL,
    montopresupuestado DECIMAL(18,2) NOT NULL,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    usuarioid INTEGER NOT NULL REFERENCES usuario(usuarioid)
);

-- =====================================================
-- Tabla: registrogastoencabezado
-- =====================================================
CREATE TABLE IF NOT EXISTS registrogastoencabezado (
    registrogastoid SERIAL PRIMARY KEY,
    fecha DATE NOT NULL,
    fondomonetarioid INTEGER NOT NULL REFERENCES fondomonetario(fondomonetarioid),
    nombrecomercio VARCHAR(200) NOT NULL,
    tipodocumento VARCHAR(50) NOT NULL,
    observaciones VARCHAR(500) NULL,
    montototal DECIMAL(18,2) NOT NULL DEFAULT 0,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    usuarioid INTEGER NOT NULL REFERENCES usuario(usuarioid)
);

-- =====================================================
-- Tabla: registrogastodetalle
-- =====================================================
CREATE TABLE IF NOT EXISTS registrogastodetalle (
    registrogastodetalleid SERIAL PRIMARY KEY,
    registrogastoid INTEGER NOT NULL REFERENCES registrogastoencabezado(registrogastoid) ON DELETE CASCADE,
    tipogastoid INTEGER NOT NULL REFERENCES tipogasto(tipogastoid),
    monto DECIMAL(18,2) NOT NULL,
    descripcion VARCHAR(300) NULL
);

-- =====================================================
-- Tabla: deposito
-- =====================================================
CREATE TABLE IF NOT EXISTS deposito (
    depositoid SERIAL PRIMARY KEY,
    fecha DATE NOT NULL,
    fondomonetarioid INTEGER NOT NULL REFERENCES fondomonetario(fondomonetarioid),
    monto DECIMAL(18,2) NOT NULL,
    descripcion VARCHAR(300) NULL,
    fechacreacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fechamodificacion TIMESTAMP NULL,
    usuarioid INTEGER NOT NULL REFERENCES usuario(usuarioid)
);

-- =====================================================
-- Índices para mejorar el rendimiento
-- =====================================================

-- Índices para búsquedas por usuario
CREATE INDEX IF NOT EXISTS idx_tipogasto_usuario ON tipogasto(usuarioid);
CREATE INDEX IF NOT EXISTS idx_fondomonetario_usuario ON fondomonetario(usuarioid);
CREATE INDEX IF NOT EXISTS idx_presupuesto_usuario ON presupuesto(usuarioid);
CREATE INDEX IF NOT EXISTS idx_registrogasto_usuario ON registrogastoencabezado(usuarioid);
CREATE INDEX IF NOT EXISTS idx_deposito_usuario ON deposito(usuarioid);

-- Índices para búsquedas por fecha
CREATE INDEX IF NOT EXISTS idx_registrogasto_fecha ON registrogastoencabezado(fecha);
CREATE INDEX IF NOT EXISTS idx_deposito_fecha ON deposito(fecha);
CREATE INDEX IF NOT EXISTS idx_presupuesto_mes_anio ON presupuesto(mes, anio);

-- Índices únicos compuestos
CREATE UNIQUE INDEX IF NOT EXISTS idx_tipogasto_usuario_codigo ON tipogasto(usuarioid, codigo);
CREATE UNIQUE INDEX IF NOT EXISTS idx_presupuesto_unique ON presupuesto(usuarioid, tipogastoid, mes, anio);

-- =====================================================
-- Datos iniciales: Roles
-- =====================================================
INSERT INTO rol (nombre, descripcion, activo)
VALUES
('Administrador', 'Acceso completo al sistema', TRUE),
('Usuario', 'Usuario estándar con acceso limitado', TRUE)
ON CONFLICT (nombre) DO NOTHING;

-- =====================================================
-- Datos iniciales: Usuario Administrador
-- =====================================================
-- Credenciales:
--   Usuario: admin
--   Email: admin@example.com
--   Password: Admin123!
-- IMPORTANTE: Cambiar este password después del primer login
INSERT INTO usuario (nombreusuario, email, passwordhash, nombrecompleto, activo, rolid)
VALUES (
    'admin',
    'admin@example.com',
    'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrUSqRg=',
    'Administrador del Sistema',
    TRUE,
    (SELECT rolid FROM rol WHERE nombre = 'Administrador')
)
ON CONFLICT (nombreusuario) DO NOTHING;

-- =====================================================
-- Verificación: Mostrar datos creados
-- =====================================================
SELECT 'Roles creados:' as info;
SELECT * FROM rol;

SELECT 'Usuario admin creado:' as info;
SELECT usuarioid, nombreusuario, email, nombrecompleto, activo
FROM usuario
WHERE nombreusuario = 'admin';

-- =====================================================
-- Script completado exitosamente
-- =====================================================
SELECT '✅ Base de datos configurada correctamente' as resultado;
SELECT '📝 Credenciales de acceso:' as info;
SELECT 'Usuario: admin' as credencial_1;
SELECT 'Password: Admin123!' as credencial_2;
SELECT 'Email: admin@example.com' as credencial_3;
