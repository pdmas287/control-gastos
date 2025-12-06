-- Script para actualizar el password del usuario admin
-- Este script actualizará el hash del password en la base de datos
-- con el hash generado por el endpoint /api/auth/test-hash

-- INSTRUCCIONES:
-- 1. Primero, ve a tu API en Railway: https://[tu-dominio].up.railway.app/swagger
-- 2. Ejecuta el endpoint POST /api/auth/test-hash con el siguiente JSON:
--    {
--      "password": "Admin123!"
--    }
-- 3. Copia el hash que retorna (algo como "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrUSqRg=")
-- 4. Reemplaza 'PEGAR_AQUI_EL_HASH_GENERADO' con el hash copiado
-- 5. Ejecuta este script en Supabase SQL Editor

UPDATE usuario
SET passwordhash = 'PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=',
    fechamodificacion = CURRENT_TIMESTAMP
WHERE nombreusuario = 'admin';

-- Verificar el resultado
SELECT usuarioid, nombreusuario, email, passwordhash, activo
FROM usuario
WHERE nombreusuario = 'admin';
