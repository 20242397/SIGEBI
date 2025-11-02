using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Entitines.Configuration.Notificaciones;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using SIGEBI.Domain.Entitines.Configuration.Security;

namespace SIGEBI.Persistence.Context
{
    public class SIGEBIContext : DbContext
    {
        public SIGEBIContext(DbContextOptions<SIGEBIContext> options) : base(options)
        {
        }
        public SIGEBIContext(DbContextOptionsBuilder<SIGEBIContext> options)
        {
        }

        #region "Entidades EF Core (Persistencia)"
        // Seguridad
        public DbSet<Usuario> Usuario { get; set; }

        // Biblioteca
        public DbSet<Libro> Libro { get; set; }
        public DbSet<Ejemplar> Ejemplar { get; set; }

        // Préstamos ✅
        public DbSet<Prestamo> Prestamo { get; set; }

        // Notificaciones
        public DbSet<Notificacion> Notificacion { get; set; }

        // Reportes
        public DbSet<Reporte> Reporte { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🔹 Aplicar configuraciones Fluent API desde el ensamblado actual
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SIGEBIContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.ClrType).Ignore("CreatedAt");
                modelBuilder.Entity(entityType.ClrType).Ignore("CreatedBy");
                modelBuilder.Entity(entityType.ClrType).Ignore("ModifiedAt");
                modelBuilder.Entity(entityType.ClrType).Ignore("ModifiedBy");
                modelBuilder.Entity(entityType.ClrType).Ignore("RowVersion");
            }

            // 🔹 Guardar el enum EstadoEjemplar como texto
            modelBuilder.Entity<Ejemplar>()
                .Property(e => e.Estado)
                .HasConversion<string>();

            // 🔹 Configuración explícita de relaciones para evitar el error con 'object'
            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Ejemplar)
                .WithMany()
                .HasForeignKey(p => p.EjemplarId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
