namespace WebAPIGeo.Modelo;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Alumno> Alumnos { get; set; }
    public DbSet<Usuarios20453> Usuarios20453 { get; set; }
    public DbSet<Hospital20453> Hospital20453 { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar tabla Alumnos
        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.ToTable("alumnos");
        });

        // Configurar tabla Usuarios20453 (igual que Alumnos, solo diferente nombre de tabla)
        modelBuilder.Entity<Usuarios20453>(entity =>
        {
            entity.ToTable("usuarios_20453");
            entity.Property(e => e.FechaRegistro)
                .HasColumnName("fecha_registro");
        });

        // Configurar tabla Hospital20453
        modelBuilder.Entity<Hospital20453>(entity =>
        {
            entity.ToTable("hospital_20453");
            entity.Property(e => e.IdPaciente)
                .HasColumnName("id_paciente");
            entity.Property(e => e.NombrePaciente)
                .HasColumnName("nombre_paciente");
            entity.Property(e => e.Edad)
                .HasColumnName("edad");
            entity.Property(e => e.NivelGravedad)
                .HasColumnName("nivel_gravedad");
            entity.Property(e => e.Estado)
                .HasColumnName("estado");
            entity.Property(e => e.MedicoResponsable)
                .HasColumnName("medico_responsable");
            entity.Property(e => e.FechaIngreso)
                .HasColumnName("fecha_ingreso");
        });
    }
}