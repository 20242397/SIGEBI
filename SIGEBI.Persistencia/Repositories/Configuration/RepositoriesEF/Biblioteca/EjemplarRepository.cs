using Microsoft.EntityFrameworkCore;
using SIGEBI.Application.Repositories.Configuration.IBiblioteca;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Infrastructure.Logging;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;
using SIGEBI.Application.Validators; 

namespace SIGEBI.Persistence.Repositories.Configuration.RepositoriesEF.Biblioteca
{
    public sealed class EjemplarRepository : BaseRepository<Ejemplar>, IEjemplarRepository
    {
        private readonly SIGEBIContext _context;
        private readonly ILoggerService _logger;

        public EjemplarRepository(SIGEBIContext context, ILoggerService logger)
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<OperationResult<Ejemplar>> AddAsync(Ejemplar entity)
        {
            // ✅ Validación con EjemplarValidator
            var validacion = EjemplarValidator.Validar(entity);
            if (!validacion.Success)
                return validacion;

            if (await _context.Ejemplares.AnyAsync(e => e.CodigoBarras == entity.CodigoBarras))
                return new OperationResult<Ejemplar> { Success = false, Message = "El código de barras ya está registrado." };

            if (!await _context.Libros.AnyAsync(l => l.Id == entity.LibroId))
                return new OperationResult<Ejemplar> { Success = false, Message = "El libro asociado no existe." };

            return await base.AddAsync(entity);
        }

        public override async Task<OperationResult<Ejemplar>> UpdateAsync(Ejemplar entity)
        {
            var validacion = EjemplarValidator.Validar(entity);
            if (!validacion.Success)
                return validacion;

            return await base.UpdateAsync(entity);
        }

        #region Métodos personalizados

        public async Task<IEnumerable<Ejemplar>> ObtenerPorLibroAsync(int libroId)
        {
            return await _context.Ejemplares
                .Where(e => e.LibroId == libroId)
                .Include(e => e.Libro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ejemplar>> ObtenerDisponiblesPorLibroAsync(int libroId)
        {
            return await _context.Ejemplares
                .Where(e => e.LibroId == libroId && e.Estado == EstadoEjemplar.Disponible)
                .Include(e => e.Libro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ejemplar>> ObtenerReservadosAsync()
        {
            return await _context.Ejemplares
                .Where(e => e.Estado == EstadoEjemplar.Reservado)
                .Include(e => e.Libro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ejemplar>> ObtenerPrestadosAsync()
        {
            return await _context.Ejemplares
                .Where(e => e.Estado == EstadoEjemplar.Prestado)
                .Include(e => e.Libro)
                .ToListAsync();
        }

        public async Task<OperationResult<bool>> MarcarComoPerdidoAsync(int ejemplarId)
        {
            try
            {
                var ejemplar = await _context.Ejemplares.FindAsync(ejemplarId);
                if (ejemplar == null)
                    return new OperationResult<bool> { Success = false, Message = "Ejemplar no encontrado.", Data = false };

                ejemplar.Estado = EstadoEjemplar.Perdido;
                await _context.SaveChangesAsync();

                return new OperationResult<bool> { Success = true, Message = "Ejemplar marcado como perdido.", Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar ejemplar como perdido");
                return new OperationResult<bool> { Success = false, Message = "Error al marcar ejemplar como perdido.", Data = false };
            }
        }

        public async Task<OperationResult<bool>> MarcarComoDaniadoAsync(int ejemplarId)
        {
            try
            {
                var ejemplar = await _context.Ejemplares.FindAsync(ejemplarId);
                if (ejemplar == null)
                    return new OperationResult<bool> { Success = false, Message = "Ejemplar no encontrado.", Data = false };

                ejemplar.Estado = EstadoEjemplar.Dañado;
                await _context.SaveChangesAsync();

                return new OperationResult<bool> { Success = true, Message = "Ejemplar marcado como dañado.", Data = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar ejemplar como dañado");
                return new OperationResult<bool> { Success = false, Message = "Error al marcar ejemplar como dañado.", Data = false };
            }
        }

        #endregion
    }
}
