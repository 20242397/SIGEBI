using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Ejemplar;
using SIGEBI.Application.Dtos.Models.Configuration.Biblioteca.Libro;
using SIGEBI.Application.Dtos.Models.Configuration.Prestamo;
using SIGEBI.Application.Dtos.Models.Configuration.Usuario;
using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Entitines.Configuration.Security;

namespace SIGEBI.Application.Mappers
{
    public static class EntityToDtoMapper
    {

      

        public static UsuarioGetDto ToDto(this Usuario e)
        {
            return new UsuarioGetDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                Role = e.Role,
                Estado = e.Estado,
                Activo = e.Activo
            };
        }

      
        public static UsuarioUpdateDto ToUpdateDto(this Usuario e)
        {
            return new UsuarioUpdateDto
            {
                Id = e.Id,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Email = e.Email,
                PasswordHash = e.PasswordHash,
                PhoneNumber = e.PhoneNumber,
                Role = e.Role,
                Activo = e.Activo
            };
        }

        
        public static Usuario ToEntity(this UsuarioCreateDto dto)
        {
            return new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                PhoneNumber = dto.PhoneNumber,
                Role = "Estudiante",
                Estado = "Activo",
                Activo = true
            };
        }

       
        public static Usuario ToEntity(this UsuarioUpdateDto dto)
        {
            return new Usuario
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                Estado = dto.Activo ? "Activo" : "Inactivo",
                Activo = dto.Activo
            };
        }



        

        public static LibroGetDto ToDto(this Libro e)
        {
            return new LibroGetDto
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Autor = e.Autor,
                ISBN = e.ISBN,
                Editorial = e.Editorial!,
                AñoPublicacion = e.AñoPublicacion ?? 0,
                Categoria = e.Categoria,
                Estado = e.Estado
            };
        }

       
        public static Libro ToEntity(this LibroCreateDto dto)
        {
            return new Libro
            {
                Titulo = dto.Titulo,
                Autor = dto.Autor,
                ISBN = dto.ISBN,
                Editorial = dto.Editorial,
                AñoPublicacion = dto.AñoPublicacion,
                Categoria = dto.Categoria,
                Estado = "Disponible"
            };
        }

       
        public static Libro ToEntity(this LibroUpdateDto dto)
        {
            return new Libro
            {
                Id = dto.Id,
                Titulo = dto.Titulo,
                Autor = dto.Autor,
                ISBN = dto.ISBN,
                Editorial = dto.Editorial,
                AñoPublicacion = dto.AñoPublicacion ?? 0,
                Categoria = dto.Categoria,
                Estado = dto.Estado ?? "Disponible"
            };
        }



        

        public static PrestamoGetDto ToDto(this Prestamo e)
        {
            return new PrestamoGetDto
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                EjemplarId = e.EjemplarId,
                FechaPrestamo = e.FechaPrestamo,
                FechaVencimiento = e.FechaVencimiento,
                FechaDevolucion = e.FechaDevolucion,
                Penalizacion = e.Penalizacion,
                LibroId = e.LibroId,
                Estado = e.Estado
            };
        }


       
        public static Prestamo ToEntity(this PrestamoCreateDto dto)
        {
            return new Prestamo
            {
                UsuarioId = dto.UsuarioId,
                EjemplarId = dto.EjemplarId,
                LibroId = dto.LibroId,
                FechaPrestamo = DateTime.Now,
                FechaVencimiento = dto.FechaVencimiento,
                Estado = "Activo",
                Penalizacion = 0
            };
        }

       
        public static Prestamo ToEntityUpdate(this PrestamoUpdateDto dto, Prestamo original)
        {
            original.FechaVencimiento = dto.FechaVencimiento;
            return original;
        }

      
        public static LibroUpdateDto ToUpdateDto(this Libro e)
        {
            return new LibroUpdateDto
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Autor = e.Autor,
                ISBN = e.ISBN,
                Editorial = e.Editorial,
                AñoPublicacion = e.AñoPublicacion ?? 0,
                Categoria = e.Categoria,
                Estado = e.Estado
            };
        }



        public static EjemplarGetDto ToDto(this Ejemplar e)
        {
            return new EjemplarGetDto
            {
                Id = e.Id,
                CodigoBarras = e.CodigoBarras,
                Estado = e.Estado.ToString(), 
                LibroId = e.LibroId
            };
        }

        public static Ejemplar ToEntity(this EjemplarCreateDto dto)
        {
            return new Ejemplar
            {
                CodigoBarras = dto.CodigoBarras,
                Estado = Enum.Parse<EstadoEjemplar>(dto.Estado, true),
                LibroId = dto.LibroId
            };
        }

       
        public static Ejemplar ToEntity(this EjemplarUpdateDto dto)
        {
            return new Ejemplar
            {
                Id = dto.Id,
                CodigoBarras = dto.CodigoBarras,
                Estado = Enum.Parse<EstadoEjemplar>(dto.Estado, true),
                LibroId = dto.LibroId
            };
        }


    }
}