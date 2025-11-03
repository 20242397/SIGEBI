using Microsoft.EntityFrameworkCore;
using SIGEBI.Application.Repositories.Configuration.IBiblioteca;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;
using SIGEBI.Application.Validators;
using SIGEBI.Persistence.Logging;

namespace SIGEBI.Persistence.Repositories.RepositoriesEF.Biblioteca
{
    public sealed class EjemplarRepository : BaseRepository<Ejemplar>, IEjemplarRepository
    {
        private readonly SIGEBIContext _context;
        private readonly ILoggerService<Ejemplar> _logger;

        public EjemplarRepository(SIGEBIContext context, ILoggerService<Ejemplar> logger)
            : base(context ,logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<OperationResult<Ejemplar>> AddAsync(Ejemplar entity)
        {
            //  Validación con EjemplarValidator
            var validacion = EjemplarValidator.Validar(entity);
            if (!validacion.Success)
                return validacion;

            //  Validar si el código de barras ya existe
            if (await _context.Ejemplar.AnyAsync(e => e.CodigoBarras == entity.CodigoBarras))
                return new OperationResult<Ejemplar>
                {
                    Success = false,
                    Message = "El código de barras ya está registrado."
                };

            //  Validar si el libro asociado existe
            if (!await _context.Libro.AnyAsync(l => l.Id == entity.LibroId))
                return new OperationResult<Ejemplar>
                {
                    Success = false,
                    Message = "El libro asociado no existe."
                };

            try
            {
                //  Insertar registro
                await _context.Ejemplar.AddAsync(entity);
                var rows = await _context.SaveChangesAsync();

                if (rows > 0)
                {
                    return new OperationResult<Ejemplar>
                    {
                        Success = true,
                        Message = $"Ejemplar registrado correctamente para el libro ID {entity.LibroId}.",
                        Data = entity
                    };
                }

                return new OperationResult<Ejemplar>
                {
                    Success = false,
                    Message = "No se pudo registrar el ejemplar."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar ejemplar");
                return new OperationResult<Ejemplar>
                {
                    Success = false,
                    Message = $"Error al registrar el ejemplar: {ex.Message}"
                };
            }
        }


        public override async Task<OperationResult<Ejemplar>> UpdateAsync(Ejemplar entity)
        {
            //  Validar primero los datos
            var validacion = EjemplarValidator.Validar(entity);
            if (!validacion.Success)
                return validacion;

            //  Luego verificar si el estado ya es el mismo
            var original = await _context.Ejemplar.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == entity.Id);

            if (original != null && original.Estado == entity.Estado)
                return new OperationResult<Ejemplar>
                {
                    Success = false,
                    Message = "El ejemplar ya se encuentra en ese estado.",
                    Data = entity
                };

            //  Realizar actualización
            try
            {
                _context.Entry(entity).State = EntityState.Modified;

                var rows = await _context.SaveChangesAsync();

                if (rows > 0)
                {
                    return new OperationResult<Ejemplar>
                    {
                        Success = true,
                        Message = "Ejemplar actualizado correctamente.",
                        Data = entity
                    };
                }

                return new OperationResult<Ejemplar>
                {
                    Success = false,
                    Message = "No se realizaron cambios en el ejemplar."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar ejemplar");
                return new OperationResult<Ejemplar>
                {
                    Success = false,
                    Message = $"Error al actualizar ejemplar: {ex.Message}"
                };
            }
        }



        #region Métodos personalizados

        public async Task<IEnumerable<Ejemplar>> ObtenerPorLibroAsync(int libroId)
        {
            return await _context.Ejemplar
                .Where(e => e.LibroId == libroId)
                .Include(e => e.Libro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ejemplar>> ObtenerDisponiblesPorLibroAsync(int libroId)
        {
            return await _context.Ejemplar
                .Where(e => e.LibroId == libroId && e.Estado == EstadoEjemplar.Disponible)
                .Include(e => e.Libro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ejemplar>> ObtenerReservadosAsync()
        {
            return await _context.Ejemplar
                .Where(e => e.Estado == EstadoEjemplar.Reservado)
                .Include(e => e.Libro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ejemplar>> ObtenerPrestadosAsync()
        {
            return await _context.Ejemplar
                .Where(e => e.Estado == EstadoEjemplar.Prestado)
                .Include(e => e.Libro)
                .ToListAsync();
        }

        public async Task<OperationResult<bool>> MarcarComoPerdidoAsync(int ejemplarId)
        {
            try
            {
                var ejemplar = await _context.Ejemplar.FindAsync(ejemplarId);
                if (ejemplar == null)
                    return new OperationResult<bool> { Success = false, Message = "Ejemplar no encontrado.", Data = false };

                if (ejemplar.Estado == EstadoEjemplar.Perdido)
                    return new OperationResult<bool>
                    {
                        Success = false,
                        Message = "El ejemplar ya está marcado como perdido.",
                        Data = false
                    };


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


        #endregion
    }
}
