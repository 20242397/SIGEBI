

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

        #region "Entidades del módulo de Configuración"
        public DbSet<Usuario> Usuarios { get; set; }
        #endregion

        #region "Entidades del módulo de Biblioteca"
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Ejemplar> Ejemplares { get; set; }
        #endregion

        #region "Entidades del módulo de Préstamos"
        public DbSet<Prestamo> Prestamos { get; set; }
        #endregion

        #region "Entidades del módulo de Notificaciones"
        public DbSet<Notificacion> Notificaciones { get; set; }
        #endregion

        #region "Entidades del módulo de Reportes"
        public DbSet<Reporte> Reportes { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica automáticamente todas las configuraciones (Fluent API)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SIGEBIContext).Assembly);
        }
    }
}


























