using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Proyecto_alcaldia.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Cargar configuración desde .env
        DotNetEnv.Env.Load();

        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5433";
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "alcaldia_db";
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "admin";
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "admin123";

        var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
