using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;
using Proyecto_alcaldia.Infrastructure.Data;

namespace Proyecto_alcaldia.Infrastructure.Data.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Verificar si ya existen datos en las tablas críticas
        var existenUsuarios = await context.Usuarios.AnyAsync();
        var existenRoles = await context.Roles.AnyAsync();
        var existenAlcaldias = await context.Alcaldias.AnyAsync();

        // Solo ejecutar el seeding si no hay datos
        if (existenUsuarios || existenRoles || existenAlcaldias)
        {
            return; // Ya hay datos, no ejecutar seeding
        }

        Console.WriteLine("Iniciando seeding de datos por defecto...");

        // 1. Crear Alcaldía por defecto
        var alcaldiaAdmin = new Alcaldia
        {
            Nit = "000000000-0",
            Logo = null,
            MunicipioId = null,
            DepartamentoId = null,
            Direccion = "Dirección por defecto",
            Telefono = "0000000",
            CorreoInstitucional = "admin@alcaldia.gov.co",
            AlcaldeActualId = null,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        context.Alcaldias.Add(alcaldiaAdmin);
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Alcaldía por defecto creada con ID: {alcaldiaAdmin.Id}");

        // 2. Crear Rol Administrador
        var rolAdministrador = new Rol
        {
            Nombre = "Administrador",
            Descripcion = "Rol con acceso total al sistema",
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        context.Roles.Add(rolAdministrador);
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Rol Administrador creado con ID: {rolAdministrador.Id}");

        // 3. Crear Usuario Administrador
        // Nota: En producción deberías usar un hash real de la contraseña
        var usuarioAdmin = new Usuario
        {
            NombreCompleto = "Administrador del Sistema",
            CorreoElectronico = "admin@alcaldia.gov.co",
            ContrasenaHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"), // Contraseña: Admin123*
            NombreUsuario = "admin",
            UltimoAcceso = null,
            Activo = true,
            FechaCreacion = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        context.Usuarios.Add(usuarioAdmin);
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Usuario Administrador creado con ID: {usuarioAdmin.Id}");
        Console.WriteLine($"  Usuario: admin");
        Console.WriteLine($"  Contraseña: Admin123*");

        // 4. Asignar rol al usuario
        var usuarioRol = new UsuarioRol
        {
            UsuarioId = usuarioAdmin.Id,
            RolId = rolAdministrador.Id,
            FechaAsignacion = DateTime.UtcNow,
            Activo = true
        };

        context.UsuariosRoles.Add(usuarioRol);
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Rol asignado al usuario administrador");
        Console.WriteLine("=====================================");
        Console.WriteLine("Seeding completado exitosamente!");
        Console.WriteLine("=====================================");
    }
}
