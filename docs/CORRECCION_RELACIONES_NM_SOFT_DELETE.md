# Corrección: Relaciones N:M con Soft Delete

## Problema Detectado

Al editar entidades con relaciones muchos-a-muchos (N:M) que implementan soft delete, se producía el error:

```
23505: duplicate key value violates unique constraint "IX_secretarias_subsecretarias_secretaria_id_subsecretaria_id"
```

### Causa Raíz

El código original hacía:
1. **Soft delete** de relaciones existentes (IsDeleted = true)
2. **INSERT** de nuevas relaciones con las mismas IDs

El problema: Los índices únicos **no consideraban IsDeleted**, por lo que intentar insertar una relación que ya existía (aunque estuviera eliminada) violaba la restricción.

## Solución Implementada

### 1. Corrección en los Controladores

Se modificó la lógica de actualización de relaciones N:M para **reutilizar** registros existentes:

**Archivos modificados:**
- `SubsecretariasController.cs` - Método Edit
- `ResponsablesController.cs` - Método Edit

**Lógica nueva:**

```csharp
// Obtener TODAS las asociaciones (incluyendo eliminadas)
var todasAsociaciones = await _context.TablaIntermedia
    .Where(x => x.EntidadId == id)
    .ToListAsync();

var idsSeleccionados = nuevosIds ?? new List<int>();

foreach (var asociacion in todasAsociaciones)
{
    if (idsSeleccionados.Contains(asociacion.RelacionadoId))
    {
        // La relación DEBE existir
        if (asociacion.IsDeleted)
        {
            // RESTAURAR registro eliminado
            asociacion.IsDeleted = false;
            asociacion.DeletedAt = null;
            asociacion.DeletedBy = null;
            asociacion.FechaActualizacion = DateTime.UtcNow;
        }
        // Remover de la lista
        idsSeleccionados.Remove(asociacion.RelacionadoId);
    }
    else
    {
        // La relación NO debe existir
        if (!asociacion.IsDeleted)
        {
            // SOFT DELETE
            asociacion.IsDeleted = true;
            asociacion.DeletedAt = DateTime.UtcNow;
            asociacion.DeletedBy = deletedBy;
        }
    }
}

// Crear solo las asociaciones COMPLETAMENTE NUEVAS
foreach (var nuevoId in idsSeleccionados)
{
    var nuevaAsociacion = new TablaIntermedia
    {
        EntidadId = id,
        RelacionadoId = nuevoId,
        FechaCreacion = DateTime.UtcNow,
        Activo = true,
        IsDeleted = false
    };
    _context.TablaIntermedia.Add(nuevaAsociacion);
}
```

**Ventajas:**
- ✅ No crea registros duplicados
- ✅ Reutiliza relaciones previamente eliminadas
- ✅ Mantiene el historial completo
- ✅ Solo crea registros nuevos cuando realmente no existían antes

### 2. Corrección en la Base de Datos

Se crearon **índices únicos parciales** que excluyen registros eliminados:

**Migración:** `20260106170225_CorregirIndicesUnicosConSoftDelete`

**SQL aplicado:**

```sql
-- Eliminar índices únicos globales
DROP INDEX IX_secretarias_subsecretarias_secretaria_id_subsecretaria_id;
DROP INDEX IX_subsecretarias_responsables_subsecretaria_id_responsable_id;

-- Crear índices únicos parciales (solo para IsDeleted = false)
CREATE UNIQUE INDEX IX_secretarias_subsecretarias_secretaria_id_subsecretaria_id 
ON secretarias_subsecretarias (secretaria_id, subsecretaria_id) 
WHERE "IsDeleted" = false;

CREATE UNIQUE INDEX IX_subsecretarias_responsables_subsecretaria_id_responsable_id 
ON subsecretarias_responsables (subsecretaria_id, responsable_id) 
WHERE "IsDeleted" = false;
```

**Resultado:**
- ✅ Se permite tener múltiples registros con las mismas IDs si están eliminados
- ✅ Solo hay UNA relación activa por combinación de IDs
- ✅ Se mantiene la integridad referencial

## Tablas Afectadas

### Tablas con relaciones N:M corregidas:

1. **secretarias_subsecretarias**
   - Relación: Secretaria ↔ Subsecretaria
   - Índice único parcial: (secretaria_id, subsecretaria_id) WHERE IsDeleted = false

2. **subsecretarias_responsables**
   - Relación: Subsecretaria ↔ Responsable
   - Índice único parcial: (subsecretaria_id, responsable_id) WHERE IsDeleted = false

## Patrón para Aplicar en Otras Entidades

Si en el futuro se crean más relaciones N:M con soft delete, seguir este patrón:

### En el controlador (método Edit):

```csharp
// 1. Obtener TODAS las asociaciones (incluyendo eliminadas)
var todas = await _context.TablaIntermedia
    .Where(x => x.PrincipalId == id)
    .ToListAsync();

var seleccionados = nuevosIds ?? new List<int>();

// 2. Procesar existentes
foreach (var asociacion in todas)
{
    if (seleccionados.Contains(asociacion.RelacionId))
    {
        // Debe existir - restaurar si estaba eliminada
        if (asociacion.IsDeleted)
        {
            asociacion.IsDeleted = false;
            asociacion.DeletedAt = null;
            asociacion.DeletedBy = null;
        }
        seleccionados.Remove(asociacion.RelacionId);
    }
    else
    {
        // No debe existir - eliminar si estaba activa
        if (!asociacion.IsDeleted)
        {
            asociacion.IsDeleted = true;
            asociacion.DeletedAt = DateTime.UtcNow;
            asociacion.DeletedBy = deletedBy;
        }
    }
}

// 3. Crear nuevas (las que quedaron en la lista)
foreach (var nuevoId in seleccionados)
{
    _context.TablaIntermedia.Add(new TablaIntermedia
    {
        PrincipalId = id,
        RelacionId = nuevoId,
        FechaCreacion = DateTime.UtcNow,
        IsDeleted = false
    });
}
```

### En la migración:

```csharp
// Eliminar índice único global
migrationBuilder.DropIndex(
    name: "IX_tabla_columna1_columna2",
    table: "tabla");

// Crear índice único parcial
migrationBuilder.Sql(@"
    CREATE UNIQUE INDEX IX_tabla_columna1_columna2 
    ON tabla (columna1, columna2) 
    WHERE ""IsDeleted"" = false;
");
```

## Verificación

Para verificar que funciona correctamente:

1. Crear una entidad con relaciones N:M
2. Editar y cambiar las relaciones (agregar/quitar)
3. Guardar → ✅ Debe funcionar sin error de duplicado
4. Volver a editar y restablecer relaciones anteriores
5. Guardar → ✅ Debe restaurar registros eliminados en lugar de crear duplicados

## Estado Actual

- ✅ SubsecretariasController.Edit corregido
- ✅ ResponsablesController.Edit corregido
- ✅ Índices únicos parciales aplicados en BD
- ✅ Migración aplicada exitosamente
- ✅ Sistema probado y funcionando

## Notas Importantes

⚠️ **IMPORTANTE**: Este patrón es CRÍTICO para todas las relaciones N:M con soft delete. Si no se implementa correctamente:
- Se generarán errores de duplicación al editar
- Se perderá el historial de relaciones
- Se violará la integridad referencial

✅ **SIEMPRE** usar índices únicos parciales con `WHERE "IsDeleted" = false` en tablas intermedias que implementen soft delete.
