using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Entitines.Configuration.Security;
using SIGEBI.Persistence.Models.Configuration;
using SIGEBI.Persistence.Models.Configuration.Libro;
using SIGEBI.Persistence.Models.Configuration.Usuario;

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
        public static UsuarioGetModel ToUsuarioGetModel(Dictionary<string, object> r) => new UsuarioGetModel
        {
            Id = (int)r["Id"],
            Nombre = r["Nombre"].ToString()!,
            Apellido = r["Apellido"].ToString()!,
            Email = r["Email"].ToString()!,
            PhoneNumber = r["PhoneNumber"]?.ToString(),
            Role = r["Role"]?.ToString(),
            Estado = r["Estado"].ToString()!
        };

        public static LibroGetModel ToLibroGetModel(Dictionary<string, object> r) => new LibroGetModel
        {
            Id = (int)r["Id"],
            Titulo = r["Titulo"].ToString()!,
            Autor = r["Autor"].ToString()!,
            ISBN = r["ISBN"].ToString()!,
            Editorial = r["Editorial"].ToString()!,
            AñoPublicacion = (int)r["AñoPublicacion"],
            Categoria = r["Categoria"]?.ToString()
        };

        public static PrestamoGetModel ToPrestamoGetModel(Dictionary<string, object> r) => new PrestamoGetModel
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
        public static UsuarioGetModel ToModel(this Usuario e) => new UsuarioGetModel
        {
            Id = e.Id,
            Nombre = e.Nombre,
            Apellido = e.Apellido,
            Email = e.Email,
            PhoneNumber = e.PhoneNumber,
            Role = e.Role,
            Estado = e.Estado
        };

        public static LibroGetModel ToModel(this Libro e) => new LibroGetModel
        {
            Id = e.Id,
            Titulo = e.Titulo,
            Autor = e.Autor,
            ISBN = e.ISBN,
            Editorial = e.Editorial,
            AñoPublicacion = e.AñoPublicacion,
            Categoria = e.Categoria
        };

        public static PrestamoGetModel ToModel(this Prestamo e) => new PrestamoGetModel
        {
            Id = e.Id,
            UsuarioId = e.UsuarioId,
            EjemplarId = e.EjemplarId,
            FechaPrestamo = e.FechaPrestamo,
            FechaVencimiento = e.FechaVencimiento,
            FechaDevolucion = e.FechaDevolucion,
            Penalizacion = e.Penalizacion
        };
    }
}
