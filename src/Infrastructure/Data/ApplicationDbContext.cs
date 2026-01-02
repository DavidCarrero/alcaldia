using Microsoft.EntityFrameworkCore;
using Proyecto_alcaldia.Domain.Entities;

namespace Proyecto_alcaldia.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Seguridad
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<UsuarioRol> UsuariosRoles { get; set; }

    // Alcaldías
    public DbSet<Alcaldia> Alcaldias { get; set; }
    public DbSet<Alcalde> Alcaldes { get; set; }
    public DbSet<Vigencia> Vigencias { get; set; }
    public DbSet<AlcaldeVigencia> AlcaldesVigencias { get; set; }

    // Departamentos y Municipios
    public DbSet<Departamento> Departamentos { get; set; }
    public DbSet<Municipio> Municipios { get; set; }

    // Estructura Organizacional
    public DbSet<Secretaria> Secretarias { get; set; }
    public DbSet<Subsecretaria> Subsecretarias { get; set; }
    public DbSet<Responsable> Responsables { get; set; }

    // Configuraciones - Jerarquía Estratégica
    public DbSet<LineaEstrategica> LineasEstrategicas { get; set; }
    public DbSet<Sector> Sectores { get; set; }
    public DbSet<Programa> Programas { get; set; }
    public DbSet<Producto> Productos { get; set; }

    // Configuraciones - Planes
    public DbSet<PlanNacional> PlanesNacionales { get; set; }
    public DbSet<PlanDepartamental> PlanesDepartamentales { get; set; }
    public DbSet<PlanMunicipal> PlanesMunicipales { get; set; }

    // Configuraciones - ODS
    public DbSet<ODS> ODS { get; set; }
    public DbSet<MetaODS> MetasODS { get; set; }

    // Configuraciones - Proyectos
    public DbSet<Indicador> Indicadores { get; set; }
    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Actividad> Actividades { get; set; }
    public DbSet<Evidencia> Evidencias { get; set; }
    public DbSet<ProyectoIndicador> ProyectosIndicadores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configuración de entidades base
        ConfigurarSeguridadEntities(modelBuilder);
        ConfigurarAlcaldiaEntities(modelBuilder);
        ConfigurarDepartamentosYMunicipios(modelBuilder);
        ConfigurarEstructuraOrganizacional(modelBuilder);
        ConfigurarConfiguraciones(modelBuilder);
    }

    private void ConfigurarSeguridadEntities(ModelBuilder modelBuilder)
    {
        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.CorreoElectronico).IsUnique();
            entity.HasIndex(e => e.NombreUsuario).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        // Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        // UsuarioRol (tabla intermedia)
        modelBuilder.Entity<UsuarioRol>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(ur => ur.Usuario)
                .WithMany(u => u.UsuariosRoles)
                .HasForeignKey(ur => ur.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Rol)
                .WithMany(r => r.UsuariosRoles)
                .HasForeignKey(ur => ur.RolId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.UsuarioId, e.RolId }).IsUnique();
        });
    }

    private void ConfigurarAlcaldiaEntities(ModelBuilder modelBuilder)
    {
        // Alcaldia
        modelBuilder.Entity<Alcaldia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Nit).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        // Alcalde
        modelBuilder.Entity<Alcalde>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.NumeroDocumento).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(a => a.Alcaldia)
                .WithMany(al => al.Alcaldes)
                .HasForeignKey(a => a.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Vigencia
        modelBuilder.Entity<Vigencia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(v => v.Alcaldia)
                .WithMany(al => al.Vigencias)
                .HasForeignKey(v => v.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // AlcaldeVigencia (tabla intermedia)
        modelBuilder.Entity<AlcaldeVigencia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FechaAsignacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(av => av.Alcalde)
                .WithMany(a => a.AlcaldesVigencias)
                .HasForeignKey(av => av.AlcaldeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(av => av.Vigencia)
                .WithMany(v => v.AlcaldesVigencias)
                .HasForeignKey(av => av.VigenciaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.AlcaldeId, e.VigenciaId }).IsUnique();
        });
    }

    private void ConfigurarDepartamentosYMunicipios(ModelBuilder modelBuilder)
    {
        // Departamento
        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);
        });

        // Municipio
        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            // Relación muchos-a-muchos con Departamentos
            entity.HasMany(m => m.Departamentos)
                .WithMany(d => d.Municipios)
                .UsingEntity<Dictionary<string, object>>(
                    "municipio_departamentos",
                    j => j.HasOne<Departamento>().WithMany().HasForeignKey("departamento_id").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Municipio>().WithMany().HasForeignKey("municipio_id").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("municipio_id", "departamento_id");
                        j.ToTable("municipio_departamentos");
                    });
        });
    }

    private void ConfigurarEstructuraOrganizacional(ModelBuilder modelBuilder)
    {
        // Secretaria
        modelBuilder.Entity<Secretaria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(s => s.Alcaldia)
                .WithMany(a => a.Secretarias)
                .HasForeignKey(s => s.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Subsecretaria
        modelBuilder.Entity<Subsecretaria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(ss => ss.Alcaldia)
                .WithMany(a => a.Subsecretarias)
                .HasForeignKey(ss => ss.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ss => ss.Secretaria)
                .WithMany(s => s.Subsecretarias)
                .HasForeignKey(ss => ss.SecretariaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Responsable
        modelBuilder.Entity<Responsable>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.NumeroIdentificacion).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(r => r.Alcaldia)
                .WithMany(a => a.Responsables)
                .HasForeignKey(r => r.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.Secretaria)
                .WithMany(s => s.Responsables)
                .HasForeignKey(r => r.SecretariaId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private void ConfigurarConfiguraciones(ModelBuilder modelBuilder)
    {
        // LineaEstrategica
        modelBuilder.Entity<LineaEstrategica>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(l => l.Alcaldia)
                .WithMany()
                .HasForeignKey(l => l.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Sector
        modelBuilder.Entity<Sector>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(s => s.Alcaldia)
                .WithMany()
                .HasForeignKey(s => s.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.LineaEstrategica)
                .WithMany(l => l.Sectores)
                .HasForeignKey(s => s.LineaEstrategicaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Programa
        modelBuilder.Entity<Programa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(p => p.Alcaldia)
                .WithMany()
                .HasForeignKey(p => p.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Sector)
                .WithMany(s => s.Programas)
                .HasForeignKey(p => p.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.PlanMunicipal)
                .WithMany(pm => pm.Programas)
                .HasForeignKey(p => p.PlanMunicipalId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.ODS)
                .WithMany(o => o.Programas)
                .HasForeignKey(p => p.ODSId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Producto
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(p => p.Alcaldia)
                .WithMany()
                .HasForeignKey(p => p.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Programa)
                .WithMany(pr => pr.Productos)
                .HasForeignKey(p => p.ProgramaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PlanNacional
        modelBuilder.Entity<PlanNacional>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(pn => pn.Alcaldia)
                .WithMany()
                .HasForeignKey(pn => pn.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pn => pn.Sector)
                .WithMany(s => s.PlanesNacionales)
                .HasForeignKey(pn => pn.SectorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // PlanDepartamental
        modelBuilder.Entity<PlanDepartamental>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(pd => pd.Alcaldia)
                .WithMany()
                .HasForeignKey(pd => pd.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pd => pd.Sector)
                .WithMany(s => s.PlanesDepartamentales)
                .HasForeignKey(pd => pd.SectorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // PlanMunicipal
        modelBuilder.Entity<PlanMunicipal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(pm => pm.Alcaldia)
                .WithMany()
                .HasForeignKey(pm => pm.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pm => pm.Municipio)
                .WithMany()
                .HasForeignKey(pm => pm.MunicipioId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(pm => pm.Alcalde)
                .WithMany()
                .HasForeignKey(pm => pm.AlcaldeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(pm => pm.PlanNacional)
                .WithMany(pn => pn.PlanesMunicipales)
                .HasForeignKey(pm => pm.PlanNacionalId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(pm => pm.PlanDepartamental)
                .WithMany(pd => pd.PlanesMunicipales)
                .HasForeignKey(pm => pm.PlanDptlId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ODS
        modelBuilder.Entity<ODS>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(o => o.Alcaldia)
                .WithMany()
                .HasForeignKey(o => o.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.MetaODS)
                .WithMany(m => m.ODSList)
                .HasForeignKey(o => o.MetaODSId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // MetaODS
        modelBuilder.Entity<MetaODS>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(m => m.Alcaldia)
                .WithMany()
                .HasForeignKey(m => m.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Indicador
        modelBuilder.Entity<Indicador>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(i => i.Alcaldia)
                .WithMany()
                .HasForeignKey(i => i.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(i => i.Responsable)
                .WithMany()
                .HasForeignKey(i => i.ResponsableId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(i => i.Producto)
                .WithMany(p => p.Indicadores)
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Proyecto
        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(p => p.Alcaldia)
                .WithMany()
                .HasForeignKey(p => p.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Responsable)
                .WithMany()
                .HasForeignKey(p => p.ResponsableId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Actividad
        modelBuilder.Entity<Actividad>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(a => a.Alcaldia)
                .WithMany()
                .HasForeignKey(a => a.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Proyecto)
                .WithMany(p => p.Actividades)
                .HasForeignKey(a => a.ProyectoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Responsable)
                .WithMany()
                .HasForeignKey(a => a.ResponsableId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(a => a.Vigencia)
                .WithMany()
                .HasForeignKey(a => a.VigenciaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Evidencia
        modelBuilder.Entity<Evidencia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => new { e.AlcaldiaId, e.Codigo }).IsUnique();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(e => e.Alcaldia)
                .WithMany()
                .HasForeignKey(e => e.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Actividad)
                .WithMany(a => a.Evidencias)
                .HasForeignKey(e => e.ActividadId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.VerificadaPorUsuario)
                .WithMany()
                .HasForeignKey(e => e.VerificadaPor)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ProyectoIndicador (tabla intermedia)
        modelBuilder.Entity<ProyectoIndicador>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(pi => pi.Alcaldia)
                .WithMany()
                .HasForeignKey(pi => pi.AlcaldiaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pi => pi.Proyecto)
                .WithMany(p => p.ProyectosIndicadores)
                .HasForeignKey(pi => pi.ProyectoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pi => pi.Indicador)
                .WithMany(i => i.ProyectosIndicadores)
                .HasForeignKey(pi => pi.IndicadorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ProyectoId, e.IndicadorId }).IsUnique();
        });
    }
}
