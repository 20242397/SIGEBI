using Microsoft.EntityFrameworkCore;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Security;
using SIGEBI.Domain.Repository;
using SIGEBI.Infrastructure.Logging;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;
using System.Text.RegularExpressions;

namespace SIGEBI.Persistence.Repositories
{
    public sealed class UsuarioRepository : BaseRepository<Usuario>, IBaseRepository<Usuario>
    {
        private readonly SIGEBIContext _context;

        public UsuarioRepository(SIGEBIContext context, ILoggerService logger)
            : base(context, logger)
        {
            _context = context;
        }

        public override async Task<OperationResult<Usuario>> AddAsync(Usuario entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Nombre) || string.IsNullOrWhiteSpace(entity.Apellido))
                return new OperationResult<Usuario> { Success = false, Message = "El nombre y apellido son obligatorios." };

            if (!Regex.IsMatch(entity.Email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return new OperationResult<Usuario> { Success = false, Message = "El formato del email no es válido." };

            if (await _context.Usuarios.AnyAsync(u => u.Email == entity.Email))
                return new OperationResult<Usuario> { Success = false, Message = "El email ya está registrado." };

            if (!new[] { "Admin", "Bibliotecario", "Usuario" }.Contains(entity.Role))
                return new OperationResult<Usuario> { Success = false, Message = "El rol no es válido." };

            return await base.AddAsync(entity);
        }

        public override async Task<OperationResult<Usuario>> UpdateAsync(Usuario entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Nombre))
                return new OperationResult<Usuario> { Success = false, Message = "El nombre es obligatorio." };

            if (string.IsNullOrWhiteSpace(entity.Apellido))
                return new OperationResult<Usuario> { Success = false, Message = "El apellido es obligatorio." };

            return await base.UpdateAsync(entity);
        }
    }
}

