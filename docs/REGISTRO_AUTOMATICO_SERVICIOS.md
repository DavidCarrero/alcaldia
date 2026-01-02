# Registro Automático de Servicios y Repositorios

## 📋 Descripción

Este proyecto utiliza **Assembly Scanning** para registrar automáticamente todos los repositorios y servicios en el contenedor de inyección de dependencias, eliminando la necesidad de registrar manualmente cada nuevo servicio o repositorio.

## 🎯 Ventajas

✅ **Sin configuración manual**: No necesitas modificar `Program.cs` cada vez que creas un nuevo servicio o repositorio  
✅ **Convención sobre configuración**: Sigue las convenciones de nombrado automáticamente  
✅ **Menos código**: Reduce drásticamente las líneas de configuración  
✅ **Sin dependencias externas**: Usa reflection nativa de .NET  
✅ **Escalable**: Funciona con cualquier cantidad de servicios y repositorios  

## 🔧 Cómo Funciona

### 1. Convenciones de Nombrado

El sistema escanea automáticamente los assemblies buscando:

**Repositorios:**
- Interfaz: `IXxxRepository` (en `Proyecto_alcaldia.Domain.Interfaces`)
- Implementación: `XxxRepository` (en `Proyecto_alcaldia.Infrastructure.Repositories`)

**Servicios:**
- Interfaz: `IXxxService` (en `Proyecto_alcaldia.Application.Services`)
- Implementación: `XxxService` (en `Proyecto_alcaldia.Application.Services`)

### 2. Métodos de Extensión

El archivo [`ServiceCollectionExtensions.cs`](src/Presentation/Extensions/ServiceCollectionExtensions.cs) contiene dos métodos:

```csharp
builder.Services.AddRepositories();       // Registra todos los repositorios
builder.Services.AddApplicationServices(); // Registra todos los servicios
```

### 3. Proceso de Registro

1. **Carga los assemblies** de Domain, Infrastructure y Application
2. **Busca todas las interfaces** que terminan en "Repository" o "Service"
3. **Encuentra las implementaciones** correspondientes
4. **Registra automáticamente** cada par interfaz-implementación como `Scoped`

## 📝 Uso

### Para crear un nuevo Repositorio:

1. Crea la interfaz en `src/Domain/Interfaces/`:
```csharp
public interface IProductoRepository
{
    Task<Producto> GetByIdAsync(int id);
    // ...más métodos
}
```

2. Crea la implementación en `src/Infrastructure/Repositories/`:
```csharp
public class ProductoRepository : IProductoRepository
{
    // Implementación
}
```

**¡Listo!** Se registrará automáticamente. No necesitas tocar `Program.cs`.

### Para crear un nuevo Servicio:

1. Crea la interfaz en `src/Application/Services/`:
```csharp
public interface IProductoService
{
    Task<ProductoViewModel> GetByIdAsync(int id);
    // ...más métodos
}
```

2. Crea la implementación en `src/Application/Services/`:
```csharp
public class ProductoService : IProductoService
{
    // Implementación
}
```

**¡Listo!** Se registrará automáticamente.

## ⚠️ Requisitos

Para que el registro automático funcione, debes seguir estas reglas:

1. ✅ **Nombrado consistente**: La implementación debe implementar la interfaz correspondiente
2. ✅ **Sufijo correcto**: Las interfaces deben terminar en `Repository` o `Service`
3. ✅ **Ubicación correcta**: Los archivos deben estar en los directorios apropiados
4. ✅ **Una implementación por interfaz**: Cada interfaz debe tener solo una implementación concreta

## 🔍 Ejemplo Completo

**Antes (Manual):**
```csharp
// Program.cs - Tenías que agregar manualmente cada uno
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IAlcaldiaRepository, AlcaldiaRepository>();
builder.Services.AddScoped<IAlcaldeRepository, AlcaldeRepository>();
// ... 20+ líneas más
```

**Ahora (Automático):**
```csharp
// Program.cs - Solo 2 líneas registran TODO
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
```

## 🚀 Servicios Registrados Actualmente

### Repositorios (7 automáticos)
- ✅ IUsuarioRepository → UsuarioRepository
- ✅ IRolRepository → RolRepository
- ✅ IAlcaldiaRepository → AlcaldiaRepository
- ✅ IAlcaldeRepository → AlcaldeRepository
- ✅ IDepartamentoRepository → DepartamentoRepository
- ✅ IMunicipioRepository → MunicipioRepository
- ✅ ISecretariaRepository → SecretariaRepository

### Servicios (7 automáticos)
- ✅ IUsuarioService → UsuarioService
- ✅ IRolService → RolService
- ✅ IAlcaldiaService → AlcaldiaService
- ✅ IAlcaldeService → AlcaldeService
- ✅ IDepartamentoService → DepartamentoService
- ✅ IMunicipioService → MunicipioService
- ✅ ISecretariaService → SecretariaService

## 🎯 Lifetime del Servicio

Todos los servicios y repositorios se registran con lifetime **Scoped**, lo que significa:
- Se crea una instancia por request HTTP
- Se comparte dentro del mismo scope
- Se destruye al final del request

## 🔧 Personalización

Si necesitas cambiar el lifetime o agregar más lógica, edita el archivo:
[`src/Presentation/Extensions/ServiceCollectionExtensions.cs`](src/Presentation/Extensions/ServiceCollectionExtensions.cs)

## 📚 Referencias

- [Dependency Injection en ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Service Lifetimes](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-lifetimes)
- [Assembly Scanning](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly)
