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

        #region "Entidades EF Core (Persistencia)"
        // Seguridad
        public DbSet<Usuario> Usuarios { get; set; }

        // Biblioteca
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Ejemplar> Ejemplares { get; set; }

        // Préstamos ✅
        public DbSet<Prestamo> Prestamos { get; set; }

        // Notificaciones
        public DbSet<Notificacion> Notificaciones { get; set; }

        // Reportes
        public DbSet<Reporte> Reportes { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar configuraciones Fluent API desde el ensamblado actual
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SIGEBIContext).Assembly);
        }
    }
}
