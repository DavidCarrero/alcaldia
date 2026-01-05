# Script de Seeding - Datos Iniciales del Sistema

## 📋 Descripción

Este script crea automáticamente los datos iniciales necesarios para el funcionamiento del sistema de gestión de alcaldías. Se ejecuta **una sola vez** al iniciar la aplicación por primera vez.

## 🔒 Condiciones de Ejecución

El script **SOLO se ejecuta** cuando se cumplen TODAS estas condiciones:

1. ✅ La tabla `usuarios` está vacía
2. ✅ La tabla `roles` está vacía  
3. ✅ La tabla `alcaldias` está vacía

Si **cualquiera** de estas tablas contiene datos, el script **NO se ejecutará** para evitar duplicados.

## 📦 Datos Creados

### 1. Departamento por Defecto
Se crea un departamento administrativo inicial:

| Campo | Valor |
|-------|-------|
| **Código** | `00` |
| **Nombre** | `Departamento Administrativo` |
| **Estado** | Activo |

### 2. Municipio por Defecto
Se crea un municipio administrativo inicial:

| Campo | Valor |
|-------|-------|
| **Código** | `00000` |
| **Nombre** | `Municipio Administrativo` |
| **Estado** | Activo |

### 3. Alcaldía por Defecto
Se crea una alcaldía administrativa con los siguientes datos:

| Campo | Valor |
|-------|-------|
| **NIT** | `000000000-0` |
| **Municipio** | `Municipio Administrativo` |
| **Departamento** | `Departamento Administrativo` |
| **Correo Institucional** | `admin@alcaldia.gov.co` |
| **Dirección** | `Dirección por defecto` |
| **Teléfono** | `0000000` |
| **Estado** | Activo |

### 4. Rol Administrador
Se crea el rol de administrador con permisos totales:

| Campo | Valor |
|-------|-------|
| **Nombre** | `Administrador` |
| **Descripción** | `Rol con acceso total al sistema` |
| **Estado** | Activo |

### 5. Usuario Administrador
Se crea el usuario administrador del sistema:

| Campo | Valor |
|-------|-------|
| **Usuario** | `admin` |
| **Contraseña** | `Admin123*` |
| **Nombre Completo** | `Administrador del Sistema` |
| **Correo** | `admin@alcaldia.gov.co` |
| **Estado** | Activo |
| **Rol Asignado** | Administrador |

### 6. Relación Usuario-Rol
Se establece automáticamente la relación entre el usuario y el rol administrador.

## 🔐 Seguridad

- La contraseña del usuario administrador se almacena utilizando **BCrypt** con hash seguro
- **Nunca** se almacena la contraseña en texto plano
- El algoritmo BCrypt incluye salt automático para mayor seguridad

## 🚀 Ejecución

El script se ejecuta automáticamente al iniciar la aplicación:

```bash
dotnet run
```

### Salida Esperada (Primera Ejecución)

```
Iniciando seeding de datos por defecto...
✓ Departamento por defecto creado con ID: 1
✓ Municipio por defecto creado con ID: 1
✓ Relación Departamento-Municipio establecida
✓ Alcaldía por defecto creada con ID: 1 (Municipio: Municipio Administrativo, Departamento: Departamento Administrativo)
✓ Rol Administrador creado con ID: 1
✓ Usuario Administrador creado con ID: 1
  Usuario: admin
  Contraseña: Admin123*
✓ Rol asignado al usuario administrador
=====================================
Seeding completado exitosamente!
=====================================
```

### Salida Esperada (Ejecuciones Posteriores)

Si el sistema ya tiene datos, **no se mostrará ningún mensaje** de seeding. La aplicación iniciará normalmente sin crear datos duplicados.

## 📝 Ubicación del Código

El script de seeding se encuentra en:

```
src/Infrastructure/Data/Seeders/DatabaseSeeder.cs
```

La configuración de ejecución se encuentra en:

```
Program.cs (líneas de seeding automático)
```

## ⚠️ Importante

### Primer Inicio del Sistema

1. Al iniciar por primera vez, el sistema creará automáticamente:
   - 1 Departamento administrativo
   - 1 Municipio administrativo
   - Relación Departamento-Municipio
   - 1 Alcaldía administrativa (asociada al departamento y municipio)
   - 1 Rol de Administrador
   - 1 Usuario Administrador

2. **Credenciales de Acceso Iniciales:**
   - **Usuario:** `admin`
   - **Contraseña:** `Admin123*`

### Recomendaciones de Seguridad

🔴 **IMPORTANTE:** Por seguridad, se recomienda **cambiar la contraseña** del usuario administrador después del primer inicio del sistema.

### ¿Cuándo NO se Ejecuta el Seeding?

El script NO se ejecutará en los siguientes casos:

- ✋ Ya existe al menos un usuario en el sistema
- ✋ Ya existe al menos un rol en el sistema
- ✋ Ya existe al menos una alcaldía en el sistema
- ✋ La base de datos ya fue inicializada previamente

## 🔄 Re-ejecutar el Seeding

Si necesitas volver a ejecutar el script de seeding, debes:

1. Eliminar todos los datos de las tablas críticas:
   ```sql
   DELETE FROM usuarios_roles;
   DELETE FROM usuarios;
   DELETE FROM roles;
   DELETE FROM alcaldias;
   DELETE FROM municipio_departamentos;
   DELETE FROM municipios;
   DELETE FROM departamentos;
   ```

2. O ejecutar un reset completo de la base de datos:
   ```bash
   dotnet ef database drop --force
   dotnet ef database update
   ```

3. Reiniciar la aplicación:
   ```bash
   dotnet run
   ```

## 📊 Verificación de Datos

Puedes verificar que los datos se crearon correctamente con estas consultas SQL:

```sql
-- Verificar departamentos y municipios
SELECT d.id, d.codigo, d.nombre as departamento, m.nombre as municipio
FROM departamentos d
LEFT JOIN municipio_departamentos md ON d.id = md.departamento_id
LEFT JOIN municipios m ON md.municipio_id = m.id;

-- Verificar alcaldía con sus relaciones
SELECT a.id, a.nit, a.direccion, a.correo_institucional,
       m.nombre as municipio, d.nombre as departamento
FROM alcaldias a
LEFT JOIN municipios m ON a.municipio_id = m.id
LEFT JOIN departamentos d ON a.departamento_id = d.id;

-- Verificar usuario administrador
SELECT u.id, u.nombre_usuario, u.nombre_completo, u.correo_electronico, r.nombre as rol 
FROM usuarios u 
INNER JOIN usuarios_roles ur ON u.id = ur.usuario_id 
INNER JOIN roles r ON ur.rol_id = r.id;

-- Verificar roles
SELECT id, nombre, descripcion 
FROM roles;
```

## 🛠️ Modificación del Script

Si necesitas modificar los datos iniciales:

1. Edita el archivo: `src/Infrastructure/Data/Seeders/DatabaseSeeder.cs`
2. Modifica los valores según tus necesidades
3. Asegúrate de eliminar los datos existentes antes de probar
4. Reinicia la aplicación

## 📚 Dependencias

El script requiere el siguiente paquete NuGet:

```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

Este paquete se instala automáticamente con el proyecto.

---

**Fecha de Creación:** 31 de Diciembre de 2025  
**Versión del Sistema:** 1.0  
**Framework:** .NET 9.0
