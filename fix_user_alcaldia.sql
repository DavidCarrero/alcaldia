-- Script para asignar alcaldía a usuario
-- Ejecutar en PostgreSQL

-- 1. Ver las alcaldías disponibles
SELECT id, nit, municipio_id, activo 
FROM alcaldias 
WHERE activo = true 
LIMIT 5;

-- 2. Asignar la primera alcaldía activa al usuario administrador
UPDATE usuarios 
SET alcaldia_id = (SELECT id FROM alcaldias WHERE activo = true LIMIT 1)
WHERE correo_electronico = 'admin@admin.com';

-- 3. Verificar que se asignó correctamente
SELECT id, nombre_completo, correo_electronico, alcaldia_id 
FROM usuarios 
WHERE correo_electronico = 'admin@admin.com';
