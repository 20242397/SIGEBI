using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Repository;
using SIGEBI.Infrastructure.Logging;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;

namespace SIGEBI.Persistence.Repositories
{
    public sealed class LibroRepository : BaseRepository<Libro>, IBaseRepository<Libro>
    {
        private readonly SIGEBIContext _context;

        public LibroRepository(SIGEBIContext context, ILoggerService logger)
            : base(context, logger)
        {
            _context = context;
        }

        public override async Task<OperationResult<Libro>> AddAsync(Libro entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Titulo) || string.IsNullOrWhiteSpace(entity.Autor))
                return new OperationResult<Libro> { Success = false, Message = "Título y autor son obligatorios." };

            if (await _context.Libros.AnyAsync(l => l.ISBN == entity.ISBN))
                return new OperationResult<Libro> { Success = false, Message = "El ISBN ya está registrado." };

            if (entity.AñoPublicacion <= 0 || entity.AñoPublicacion > DateTime.Now.Year)
                return new OperationResult<Libro> { Success = false, Message = "El año no es válido." };

            return await base.AddAsync(entity);
        }

        public override async Task<OperationResult<Libro>> UpdateAsync(Libro entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Titulo))
                return new OperationResult<Libro> { Success = false, Message = "El título es obligatorio." };

            if (string.IsNullOrWhiteSpace(entity.Autor))
                return new OperationResult<Libro> { Success = false, Message = "El autor es obligatorio." };

            return await base.UpdateAsync(entity);
        }
    }
}

