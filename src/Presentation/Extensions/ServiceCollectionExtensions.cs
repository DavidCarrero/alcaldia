using System.Reflection;

namespace Proyecto_alcaldia.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra automáticamente todos los repositorios del proyecto
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Obtener todas las interfaces de repositorios (IXxxRepository) del namespace Domain.Interfaces
        var repositoryInterfaces = assembly.GetTypes()
            .Where(t => t.IsInterface && 
                       t.Name.EndsWith("Repository") && 
                       t.Namespace != null && 
                       t.Namespace.Contains("Domain.Interfaces"));

        foreach (var interfaceType in repositoryInterfaces)
        {
            // Buscar la implementación correspondiente en Infrastructure.Repositories
            var implementationType = assembly.GetTypes()
                .FirstOrDefault(t => !t.IsInterface && 
                                    !t.IsAbstract && 
                                    t.Namespace != null &&
                                    t.Namespace.Contains("Infrastructure.Repositories") &&
                                    interfaceType.IsAssignableFrom(t));

            if (implementationType != null)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }

        return services;
    }

    /// <summary>
    /// Registra automáticamente todos los servicios del proyecto
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Obtener todas las interfaces de servicios (IXxxService) del namespace Application.Services
        var serviceInterfaces = assembly.GetTypes()
            .Where(t => t.IsInterface && 
                       t.Name.EndsWith("Service") && 
                       t.Namespace != null && 
                       t.Namespace.Contains("Application.Services"));

        foreach (var interfaceType in serviceInterfaces)
        {
            // Buscar la implementación correspondiente en Application.Services
            var implementationType = assembly.GetTypes()
                .FirstOrDefault(t => !t.IsInterface && 
                                    !t.IsAbstract && 
                                    t.Namespace != null &&
                                    t.Namespace.Contains("Application.Services") &&
                                    interfaceType.IsAssignableFrom(t));

            if (implementationType != null)
            {
                services.AddScoped(interfaceType, implementationType);
            }
        }

        return services;
    }
}
