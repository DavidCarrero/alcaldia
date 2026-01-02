# Arquitectura DDD - Proyecto Alcaldía

## Estructura de Directorios

```
Proyecto-alcaldia/
├── src/
│   ├── Domain/                      # Capa de Dominio
│   │   ├── Entities/               # Entidades del dominio
│   │   │   ├── BaseEntity.cs
│   │   │   └── Usuario.cs
│   │   └── Interfaces/             # Interfaces del dominio
│   │       ├── IRepository.cs
│   │       ├── IUsuarioRepository.cs
│   │       └── IUnitOfWork.cs
│   │
│   ├── Application/                 # Capa de Aplicación
│   │   ├── DTOs/                   # Data Transfer Objects
│   │   │   ├── UsuarioDto.cs
│   │   │   ├── CreateUsuarioDto.cs
│   │   │   └── UpdateUsuarioDto.cs
│   │   ├── Interfaces/             # Interfaces de servicios
│   │   │   └── IUsuarioService.cs
│   │   └── Services/               # Implementación de servicios
│   │       └── UsuarioService.cs
│   │
│   ├── Infrastructure/              # Capa de Infraestructura
│   │   ├── Data/                   # Contexto de base de datos
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── UnitOfWork.cs
│   │   └── Repositories/           # Implementación de repositorios
│   │       ├── Repository.cs
│   │       └── UsuarioRepository.cs
│   │
│   └── Presentation/                # Capa de Presentación
│       ├── Controllers/            # Controladores MVC
│       │   ├── HomeController.cs
│       │   └── UsuariosController.cs
│       ├── Models/                 # ViewModels
│       │   └── ErrorViewModel.cs
│       ├── Views/                  # Vistas Razor
│       │   ├── Home/
│       │   ├── Shared/
│       │   └── Usuarios/
│       └── wwwroot/                # Archivos estáticos
│           ├── css/
│           ├── js/
│           └── lib/
│
├── Migrations/                      # Migraciones de EF Core
├── Properties/                      # Propiedades del proyecto
├── .env                            # Variables de entorno
├── .gitignore                      # Archivos ignorados por Git
├── Dockerfile                      # Configuración Docker
├── Program.cs                      # Punto de entrada
└── Proyecto-alcaldia.csproj        # Archivo de proyecto
```

## Capas y Responsabilidades

### 🔵 Domain (Dominio)
- **Responsabilidad**: Lógica de negocio central
- **Contiene**: Entidades, interfaces de repositorios, reglas de negocio
- **Dependencias**: Ninguna (capa más interna)

### 🟢 Application (Aplicación)
- **Responsabilidad**: Casos de uso y orquestación
- **Contiene**: DTOs, interfaces de servicios, lógica de aplicación
- **Dependencias**: Domain

### 🟡 Infrastructure (Infraestructura)
- **Responsabilidad**: Acceso a datos y servicios externos
- **Contiene**: DbContext, repositorios, implementaciones técnicas
- **Dependencias**: Domain, Application

### 🔴 Presentation (Presentación)
- **Responsabilidad**: Interfaz de usuario
- **Contiene**: Controllers, Views, ViewModels, assets estáticos
- **Dependencias**: Application

## Flujo de Datos

```
Usuario → Controller → Service → Repository → DbContext → Base de Datos
                ↓         ↓          ↓
              DTO     Application  Infrastructure
```

## Principios Aplicados

- ✅ **Separation of Concerns**: Cada capa tiene responsabilidad única
- ✅ **Dependency Inversion**: Las capas dependen de abstracciones
- ✅ **Repository Pattern**: Abstracción del acceso a datos
- ✅ **Unit of Work**: Gestión de transacciones
- ✅ **DTO Pattern**: Transferencia de datos entre capas
- ✅ **Clean Architecture**: Dependencias apuntan hacia el dominio
