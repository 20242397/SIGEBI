using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using SIGEBI.Domain.Repository;
using SIGEBI.Infrastructure.Logging;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Repositories
{
    public sealed class ReporteRepository : BaseRepository<Reporte>, IBaseRepository<Reporte>
    {
        private readonly SIGEBIContext _context;

        public ReporteRepository(SIGEBIContext context, ILoggerService logger)
            : base(context, logger)
        {
            _context = context;
        }

        public override async Task<OperationResult<Reporte>> AddAsync(Reporte entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Tipo))
                return new OperationResult<Reporte> { Success = false, Message = "El tipo de reporte es obligatorio." };

            if (entity.FechaGeneracion == default)
                return new OperationResult<Reporte> { Success = false, Message = "La fecha de generación es obligatoria." };

            if (!await _context.Usuarios.AnyAsync(u => u.Id == entity.UsuarioId))
                return new OperationResult<Reporte> { Success = false, Message = "El usuario asociado no existe." };

            return await base.AddAsync(entity);
        }

        public override async Task<OperationResult<Reporte>> UpdateAsync(Reporte entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Tipo))
                return new OperationResult<Reporte> { Success = false, Message = "El tipo de reporte es obligatorio." };

            return await base.UpdateAsync(entity);
        }
    }
}
