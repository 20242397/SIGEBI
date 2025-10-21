using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro;
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;
using SIGEBI.Application.Dtos.Models.Configuration.Usuario;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Entitines.Configuration.Security;

namespace SIGEBI.Persistence.Models
{
    public static class EntityToModelMapper
    {
        // ============
        // From ROW (DB) -> ENTITY
        // ============
        public static Usuario ToUsuario(Dictionary<string, object> r) => new Usuario
        {
            Id = (int)r["Id"],
            Nombre = r["Nombre"].ToString()!,
            Apellido = r["Apellido"].ToString()!,
            Email = r["Email"].ToString()!,
            PasswordHash = r.ContainsKey("PasswordHash") ? r["PasswordHash"]?.ToString() ?? "" : "",
            PhoneNumber = r["PhoneNumber"]?.ToString(),
            Role = r["Role"]?.ToString(),
            Estado = r["Estado"].ToString()!
        };

        public static Libro ToLibro(Dictionary<string, object> r) => new Libro
        {
            Id = (int)r["Id"],
            Titulo = r["Titulo"].ToString()!,
            Autor = r["Autor"].ToString()!,
            ISBN = r["ISBN"].ToString()!,
            Editorial = r["Editorial"].ToString()!,
            AñoPublicacion = (int)r["AñoPublicacion"],
            Categoria = r["Categoria"]?.ToString()
        };

        public static Prestamo ToPrestamo(Dictionary<string, object> r) => new Prestamo
        {
            Id = (int)r["Id"],
            UsuarioId = (int)r["UsuarioId"],
            EjemplarId = (int)r["EjemplarId"],
            FechaPrestamo = (DateTime)r["FechaPrestamo"],
            FechaVencimiento = (DateTime)r["FechaVencimiento"],
            FechaDevolucion = r["FechaDevolucion"] as DateTime?,
            Penalizacion = r["Penalizacion"] as decimal?
        };

        // ============
        // From ROW (DB) -> MODEL (DTO)
        // ============
        public static UsuarioGetDto ToUsuarioGetModel(Dictionary<string, object> r) => new UsuarioGetDto
        {


            Id = (int)r["Id"],
            Nombre = r["Nombre"].ToString()!,
            Apellido = r["Apellido"].ToString()!,
            Email = r["Email"].ToString()!,
            PhoneNumber = r["PhoneNumber"]?.ToString(),
            Role = r["Role"]?.ToString(),
            Estado = r["Estado"].ToString()!
        };

        public static LibroGetDto ToLibroGetModel(Dictionary<string, object> r) => new LibroGetDto
        {
            Id = (int)r["Id"],
            Titulo = r["Titulo"].ToString()!,
            Autor = r["Autor"].ToString()!,
            ISBN = r["ISBN"].ToString()!,
            Editorial = r["Editorial"].ToString()!,
            AñoPublicacion = (int)r["AñoPublicacion"],
            Categoria = r["Categoria"]?.ToString()
        };

        public static PrestamoGetDto ToPrestamoGetModel(Dictionary<string, object> r) => new PrestamoGetDto
        {
            Id = (int)r["Id"],
            UsuarioId = (int)r["UsuarioId"],
            EjemplarId = (int)r["EjemplarId"],
            FechaPrestamo = (DateTime)r["FechaPrestamo"],
            FechaVencimiento = (DateTime)r["FechaVencimiento"],
            FechaDevolucion = r["FechaDevolucion"] as DateTime?,
            Penalizacion = r["Penalizacion"] as decimal?
        };

        // ============
        // From ENTITY -> MODEL (DTO)
        // ============
        public static UsuarioGetDto ToModel(this Usuario e) => new UsuarioGetDto
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Apellido = e.Apellido,
            Email = e.Email,
            PhoneNumber = e.PhoneNumber,
            Role = e.Role,
            Estado = e.Estado
        };

        public static LibroGetDto ToModel(this Libro e) => new LibroGetDto
        {
            Id = e.Id,
            Titulo = e.Titulo,
            Autor = e.Autor,
            ISBN = e.ISBN,
            Editorial = e.Editorial,
            AñoPublicacion = (int)e.AñoPublicacion,
            Categoria = e.Categoria
        };

        public static PrestamoGetDto ToModel(this Prestamo e) => new PrestamoGetDto
        {
            Id = e.Id,
            UsuarioId = e.UsuarioId,
            EjemplarId = e.EjemplarId,
            FechaPrestamo = e.FechaPrestamo,
            FechaVencimiento = e.FechaVencimiento,
            FechaDevolucion = e.FechaDevolucion,
            Penalizacion = e.Penalizacion
        };
        public static Libro ToEntity(this LibroCreateDto dto) =>
           new Libro
           {
               Titulo = dto.Titulo,
               Autor = dto.Autor,
               ISBN = dto.ISBN,
               Editorial = dto.Editorial,
               AñoPublicacion = dto.AñoPublicacion,
               Categoria = dto.Categoria,
               Estado = "Disponible"
           };
        public static Libro ToEntity(this LibroUpdateDto dto) =>
           new Libro
           {
               Titulo = dto.Titulo,
               Autor = dto.Autor,
               ISBN = dto.ISBN,
               Editorial = dto.Editorial,
               AñoPublicacion = dto.AñoPublicacion.HasValue ? dto.AñoPublicacion.Value : 0,
               Categoria = dto.Categoria,
               Estado = "Disponible"
           };

        // ======================================
        // USUARIO: DTO → ENTIDAD
        // ======================================
        public static Usuario ToEntity(this UsuarioCreateDto dto) =>
            new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                PhoneNumber = dto.PhoneNumber,
                Role = "User",
                Estado = "Activo"
            };

        public static Usuario ToEntity(this UsuarioUpdateDto dto) =>
            new Usuario
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                Estado = dto.Estado
            };

    }
}
