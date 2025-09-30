using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Repository;
using SIGEBI.Infrastructure.Logging;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Repositories.Configuration.Biblioteca
{
    public sealed class EjemplarRepository : BaseRepository<Ejemplar>, IBaseRepository<Ejemplar>
    {
        private readonly SIGEBIContext _context;

        public EjemplarRepository(SIGEBIContext context, ILoggerService logger)
            : base(context, logger)
        {
            _context = context;
        }

        public override async Task<OperationResult<Ejemplar>> AddAsync(Ejemplar entity)
        {
            if (await _context.Ejemplares.AnyAsync(e => e.CodigoBarras == entity.CodigoBarras))
                return new OperationResult<Ejemplar> { Success = false, Message = "El código de barras ya está registrado." };

            if (!new[] { "Disponible", "Prestado", "Reservado" }.Contains(entity.Estado))
                return new OperationResult<Ejemplar> { Success = false, Message = "El estado no es válido." };

            if (!await _context.Libros.AnyAsync(l => l.Id == entity.LibroId))
                return new OperationResult<Ejemplar> { Success = false, Message = "El libro asociado no existe." };

            return await base.AddAsync(entity);
        }

        public override async Task<OperationResult<Ejemplar>> UpdateAsync(Ejemplar entity)
        {
            if (string.IsNullOrWhiteSpace(entity.CodigoBarras))
                return new OperationResult<Ejemplar> { Success = false, Message = "El código de barras es obligatorio." };

            if (!new[] { "Disponible", "Prestado", "Reservado" }.Contains(entity.Estado))
                return new OperationResult<Ejemplar> { Success = false, Message = "El estado no es válido." };

            return await base.UpdateAsync(entity);
        }
    }
}

