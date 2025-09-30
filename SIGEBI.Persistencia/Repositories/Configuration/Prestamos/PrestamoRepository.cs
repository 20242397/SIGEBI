using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Repository;
using SIGEBI.Infrastructure.Logging;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Repositories.Configuration.Prestamos
{
    public sealed class PrestamoRepository : BaseRepository<Prestamo>, IBaseRepository<Prestamo>
    {
        private readonly SIGEBIContext _context;

        public PrestamoRepository(SIGEBIContext context, ILoggerService logger)
            : base(context, logger)
        {
            _context = context;
        }

        public override async Task<OperationResult<Prestamo>> AddAsync(Prestamo entity)
        {
            if (!await _context.Usuarios.AnyAsync(u => u.Id == entity.UsuarioId))
                return new OperationResult<Prestamo> { Success = false, Message = "El usuario no existe." };

            if (!await _context.Ejemplares.AnyAsync(e => e.Id == entity.EjemplarId))
                return new OperationResult<Prestamo> { Success = false, Message = "El ejemplar no existe." };

            if (entity.FechaVencimiento <= entity.FechaPrestamo)
                return new OperationResult<Prestamo> { Success = false, Message = "La fecha de vencimiento debe ser posterior al préstamo." };

            return await base.AddAsync(entity);
        }

        public override async Task<OperationResult<Prestamo>> UpdateAsync(Prestamo entity)
        {
            if (entity.FechaVencimiento <= entity.FechaPrestamo)
                return new OperationResult<Prestamo> { Success = false, Message = "La fecha de vencimiento no es válida." };

            return await base.UpdateAsync(entity);
        }
    }
}
