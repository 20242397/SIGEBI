using SIGEBI.Domain.Entitines.Configuration.Biblioteca;
using SIGEBI.Domain.Entitines.Configuration.Prestamos;
using SIGEBI.Domain.Entitines.Configuration.Security;

public static class EntityToModelMapper
{
    public static Usuario ToUsuario(Dictionary<string, object> r)
    {
        return new Usuario
        {
            Id = Convert.ToInt32(r["Id"]),
            Nombre = r["Nombre"]?.ToString() ?? "",
            Apellido = r["Apellido"]?.ToString() ?? "",
            Email = r["Email"]?.ToString() ?? "",

            // 🔥 LA LÍNEA QUE FALTABA:
            Password = r.ContainsKey("Password") ? r["Password"]?.ToString() ?? "" : "",

            PhoneNumber = r.ContainsKey("PhoneNumber") ? r["PhoneNumber"]?.ToString() : null,
            Role = r["Role"]?.ToString(),
            Estado = r["Estado"]?.ToString() ?? "Activo",
            Activo = r.ContainsKey("Activo") && r["Activo"] != DBNull.Value
                     ? Convert.ToBoolean(r["Activo"])
                     : true
        };
    }




    public static Libro ToLibro(Dictionary<string, object> r)
    {
        return new Libro
        {
            Id = Convert.ToInt32(r["Id"]),
            Titulo = r["Titulo"]?.ToString() ?? "",
            Autor = r["Autor"]?.ToString() ?? "",
            ISBN = r["ISBN"]?.ToString() ?? "",
            Editorial = r["Editorial"]?.ToString() ?? "",
            AñoPublicacion = Convert.ToInt32(r["AnioPublicacion"]),
            Categoria = r["Categoria"]?.ToString() ?? "",
            Estado = r["Estado"]?.ToString() ?? "Disponible"
        };
    }


    public static Prestamo ToPrestamo(Dictionary<string, object> r)
    {
        return new Prestamo
        {
            Id = Convert.ToInt32(r["Id"]),
            UsuarioId = Convert.ToInt32(r["UsuarioId"]),
            EjemplarId = Convert.ToInt32(r["EjemplarId"]),

            LibroId = r.ContainsKey("LibroId") && r["LibroId"] != DBNull.Value
                ? Convert.ToInt32(r["LibroId"])
                : 0,

            FechaPrestamo = (DateTime)r["FechaPrestamo"],
            FechaVencimiento = (DateTime)r["FechaVencimiento"],

            FechaDevolucion = r["FechaDevolucion"] != DBNull.Value
                ? (DateTime?)r["FechaDevolucion"]
                : null,

            Penalizacion = r["Penalizacion"] != DBNull.Value
                ? (decimal?)Convert.ToDecimal(r["Penalizacion"])
                : null,

            Estado = r.ContainsKey("Estado") && r["Estado"] != DBNull.Value
                ? r["Estado"].ToString()!
                : "Activo"
        };
    }
}
