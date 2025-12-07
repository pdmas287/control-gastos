-- =============================================
-- Procedimiento para Inicializar Datos por Usuario
-- Sistema de Control de Gastos - PostgreSQL
-- =============================================

-- Función para inicializar datos de un usuario
CREATE OR REPLACE FUNCTION inicializar_datos_usuario(p_usuario_id INTEGER)
RETURNS void AS $$
DECLARE
    v_mes_actual INTEGER;
    v_anio_actual INTEGER;
    v_tipo_gasto_id INTEGER;
BEGIN
    -- Obtener mes y año actual
    v_mes_actual := EXTRACT(MONTH FROM CURRENT_DATE);
    v_anio_actual := EXTRACT(YEAR FROM CURRENT_DATE);

    -- =============================================
    -- Insertar Tipos de Gasto por defecto
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM tipogasto WHERE usuarioid = p_usuario_id) THEN
        INSERT INTO tipogasto (codigo, descripcion, activo, fechacreacion, usuarioid)
        VALUES
            ('TG-001', 'Alimentación', true, CURRENT_TIMESTAMP, p_usuario_id),
            ('TG-002', 'Transporte', true, CURRENT_TIMESTAMP, p_usuario_id),
            ('TG-003', 'Servicios Públicos', true, CURRENT_TIMESTAMP, p_usuario_id),
            ('TG-004', 'Entretenimiento', true, CURRENT_TIMESTAMP, p_usuario_id),
            ('TG-005', 'Salud', true, CURRENT_TIMESTAMP, p_usuario_id),
            ('TG-006', 'Educación', true, CURRENT_TIMESTAMP, p_usuario_id),
            ('TG-007', 'Vivienda', true, CURRENT_TIMESTAMP, p_usuario_id),
            ('TG-008', 'Vestimenta', true, CURRENT_TIMESTAMP, p_usuario_id),
            ('TG-009', 'Tecnología', true, CURRENT_TIMESTAMP, p_usuario_id),
            ('TG-010', 'Otros', true, CURRENT_TIMESTAMP, p_usuario_id);

        RAISE NOTICE 'Tipos de Gasto insertados para usuario %', p_usuario_id;
    END IF;

    -- =============================================
    -- Insertar Fondos Monetarios por defecto
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM fondomonetario WHERE usuarioid = p_usuario_id) THEN
        INSERT INTO fondomonetario (nombre, tipofondo, descripcion, saldoactual, activo, fechacreacion, usuarioid)
        VALUES
            ('Cuenta Corriente Principal', 'Cuenta Bancaria', 'Cuenta bancaria para gastos generales', 0.00, true, CURRENT_TIMESTAMP, p_usuario_id),
            ('Cuenta de Ahorros', 'Cuenta Bancaria', 'Cuenta de ahorros', 0.00, true, CURRENT_TIMESTAMP, p_usuario_id),
            ('Caja Chica', 'Caja Menuda', 'Efectivo disponible', 0.00, true, CURRENT_TIMESTAMP, p_usuario_id),
            ('Efectivo Personal', 'Caja Menuda', 'Efectivo en billetera', 0.00, true, CURRENT_TIMESTAMP, p_usuario_id);

        RAISE NOTICE 'Fondos Monetarios insertados para usuario %', p_usuario_id;
    END IF;

    -- =============================================
    -- Insertar Presupuestos para el mes actual
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM presupuesto WHERE usuarioid = p_usuario_id AND mes = v_mes_actual AND anio = v_anio_actual) THEN
        -- Insertar presupuestos basados en los tipos de gasto del usuario
        FOR v_tipo_gasto_id IN
            SELECT tipogastoid FROM tipogasto WHERE usuarioid = p_usuario_id AND activo = true
        LOOP
            INSERT INTO presupuesto (tipogastoid, mes, anio, montopresupuestado, fechacreacion, usuarioid)
            SELECT
                v_tipo_gasto_id,
                v_mes_actual,
                v_anio_actual,
                CASE
                    WHEN codigo = 'TG-001' THEN 1200000.00  -- Alimentación
                    WHEN codigo = 'TG-002' THEN 400000.00   -- Transporte
                    WHEN codigo = 'TG-003' THEN 500000.00   -- Servicios Públicos
                    WHEN codigo = 'TG-004' THEN 300000.00   -- Entretenimiento
                    WHEN codigo = 'TG-005' THEN 250000.00   -- Salud
                    WHEN codigo = 'TG-006' THEN 200000.00   -- Educación
                    WHEN codigo = 'TG-007' THEN 800000.00   -- Vivienda
                    WHEN codigo = 'TG-008' THEN 200000.00   -- Vestimenta
                    WHEN codigo = 'TG-009' THEN 150000.00   -- Tecnología
                    WHEN codigo = 'TG-010' THEN 100000.00   -- Otros
                    ELSE 100000.00
                END,
                CURRENT_TIMESTAMP,
                p_usuario_id
            FROM tipogasto
            WHERE tipogastoid = v_tipo_gasto_id;
        END LOOP;

        RAISE NOTICE 'Presupuestos para el mes actual insertados para usuario %', p_usuario_id;
    END IF;

    RAISE NOTICE 'Datos iniciales creados exitosamente para usuario %', p_usuario_id;
END;
$$ LANGUAGE plpgsql;

-- =============================================
-- Ejecutar inicialización para usuario admin
-- =============================================
DO $$
DECLARE
    v_admin_id INTEGER;
BEGIN
    -- Obtener el ID del usuario admin
    SELECT usuarioid INTO v_admin_id
    FROM usuario
    WHERE nombreusuario = 'admin'
    LIMIT 1;

    IF v_admin_id IS NOT NULL THEN
        -- Inicializar datos para admin
        PERFORM inicializar_datos_usuario(v_admin_id);
        RAISE NOTICE '========================================';
        RAISE NOTICE 'Datos iniciales creados para usuario admin (ID: %)', v_admin_id;
        RAISE NOTICE '========================================';
    ELSE
        RAISE NOTICE 'Usuario admin no encontrado';
    END IF;
END $$;
