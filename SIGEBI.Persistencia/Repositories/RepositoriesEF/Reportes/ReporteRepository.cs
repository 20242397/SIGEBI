using Microsoft.EntityFrameworkCore;
using SIGEBI.Application.Repositories.Configuration.Reportes;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using SIGEBI.Infrastructure.Logging;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;
using System.Text;

namespace SIGEBI.Persistence.Repositories.RepositoriesEF.Reportes
{
    public sealed class ReporteRepository : BaseRepository<Reporte>, IReporteRepository
    {
        private readonly SIGEBIContext _context;
        private readonly ILoggerService<Reporte> _logger;

        public ReporteRepository(SIGEBIContext context, ILoggerService<Reporte> logger)
            : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        #region ✅ Validación con ReporteValidator

        public override async Task<OperationResult<Reporte>> AddAsync(Reporte entity)
        {
            var validacion = ReporteValidator.Validar(entity);
            if (!validacion.Success)
                return validacion;

            return await base.AddAsync(entity);
        }

        public override async Task<OperationResult<Reporte>> UpdateAsync(Reporte entity)
        {
            var validacion = ReporteValidator.Validar(entity);
            if (!validacion.Success)
                return validacion;

            return await base.UpdateAsync(entity);
        }

        #endregion

        #region 🔍 Consultas generales

        public async Task<OperationResult<IEnumerable<Reporte>>> ObtenerReportesPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var reportes = await _context.Reporte
                    .Where(r => r.FechaGeneracion >= fechaInicio && r.FechaGeneracion <= fechaFin)
                    .OrderByDescending(r => r.FechaGeneracion)
                    .ToListAsync();

                return new OperationResult<IEnumerable<Reporte>>
                {
                    Success = true,
                    Data = reportes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reportes por fecha");
                return new OperationResult<IEnumerable<Reporte>>
                {
                    Success = false,
                    Message = "Error al obtener reportes por fecha"
                };
            }
        }

        public async Task<OperationResult<IEnumerable<Reporte>>> ObtenerReportesPorTipoAsync(string tipo)
        {
            try
            {
                var reportes = await _context.Reporte
                    .Where(r => r.Tipo == tipo)
                    .OrderByDescending(r => r.FechaGeneracion)
                    .ToListAsync();

                return new OperationResult<IEnumerable<Reporte>>
                {
                    Success = true,
                    Data = reportes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reportes por tipo");
                return new OperationResult<IEnumerable<Reporte>>
                {
                    Success = false,
                    Message = "Error al obtener reportes por tipo"
                };
            }
        }

        public async Task<OperationResult<IEnumerable<Reporte>>> ObtenerReportesPendientesAsync()
        {
            try
            {
                var reportes = await _context.Reporte
                    .Where(r => string.IsNullOrWhiteSpace(r.Contenido))
                    .ToListAsync();

                return new OperationResult<IEnumerable<Reporte>>
                {
                    Success = true,
                    Data = reportes
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reportes pendientes");
                return new OperationResult<IEnumerable<Reporte>>
                {
                    Success = false,
                    Message = "Error al obtener reportes pendientes"
                };
            }
        }

        #endregion

        #region 🧾 Generación de reportes automáticos

        public async Task<OperationResult<Reporte>> GenerarReportePrestamosAsync(DateTime inicio, DateTime fin)
        {
            try
            {
                var prestamos = await _context.Prestamo
                    .Include(p => p.Usuario)
                    .Include(p => p.Ejemplar)
                    .Where(p => p.FechaPrestamo >= inicio && p.FechaPrestamo <= fin)
                    .ToListAsync();

                var contenido = new StringBuilder();
                contenido.AppendLine($"REPORTE DE PRÉSTAMOS ({inicio:dd/MM/yyyy} - {fin:dd/MM/yyyy})");
                contenido.AppendLine($"Total préstamos: {prestamos.Count}");
                contenido.AppendLine("--------------------------------------------------");

                foreach (var p in prestamos)
                {
                    contenido.AppendLine($"Usuario ID: {p.UsuarioId} | Ejemplar ID: {p.EjemplarId} | Fecha: {p.FechaPrestamo:dd/MM/yyyy}");
                }


                var reporte = new Reporte
                {
                    Tipo = "Reporte de Préstamos",
                    Contenido = contenido.ToString(),
                    FechaGeneracion = DateTime.Now,
                    UsuarioId = 1
                };

                await _context.Reporte.AddAsync(reporte);
                await _context.SaveChangesAsync();

                return new OperationResult<Reporte>
                {
                    Success = true,
                    Message = "Reporte de préstamos generado correctamente",
                    Data = reporte
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de préstamos");
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "Error al generar reporte de préstamos"
                };
            }
        }

       

        public async Task<OperationResult<Reporte>> GenerarReporteUsuariosActivosAsync()
        {
            try
            {
                var usuariosActivos = await _context.Usuario
                    .Where(u => u.Estado == "Activo")
                    .ToListAsync();

                var contenido = new StringBuilder();
                contenido.AppendLine("REPORTE DE USUARIOS ACTIVOS");
                contenido.AppendLine($"Total de usuarios activos: {usuariosActivos.Count}");
                contenido.AppendLine("--------------------------------------------------");

                foreach (var u in usuariosActivos)
                    contenido.AppendLine($"Nombre: {u.Nombre} {u.Apellido} | Email: {u.Email}");

                var reporte = new Reporte
                {
                    Tipo = "Usuarios Activos",
                    Contenido = contenido.ToString(),
                    FechaGeneracion = DateTime.Now,
                    UsuarioId = 1
                };

                await _context.Reporte.AddAsync(reporte);
                await _context.SaveChangesAsync();

                return new OperationResult<Reporte>
                {
                    Success = true,
                    Message = "Reporte de usuarios activos generado correctamente",
                    Data = reporte
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de usuarios activos");
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "Error al generar reporte de usuarios activos"
                };
            }
        }

        #endregion

        #region ⚙️ Actualización de estado

        public async Task<OperationResult<bool>> MarcarComoResueltoAsync(int reporteId)
        {
            try
            {
                var reporte = await _context.Reporte.FindAsync(reporteId);
                if (reporte == null)
                    return new OperationResult<bool>
                    {
                        Success = false,
                        Message = "Reporte no encontrado",
                        Data = false
                    };

                reporte.Contenido += "\n\n✅ Este reporte fue marcado como resuelto.";
                await _context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Success = true,
                    Message = "Reporte marcado como resuelto",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al marcar reporte como resuelto");
                return new OperationResult<bool>
                {
                    Success = false,
                    Message = "Error al marcar reporte como resuelto",
                    Data = false
                };
            }
        }

        public async Task<OperationResult<Reporte>> GenerarReportePrestamosAsync(object fechaInicio, object fechaFin)
        {
            try
            {
                // 1️⃣ Convertir los parámetros a DateTime (con validación)
                DateTime inicio = Convert.ToDateTime(fechaInicio);
                DateTime fin = Convert.ToDateTime(fechaFin);

                // 2️⃣ Consultar los préstamos dentro del rango de fechas
                var prestamos = await _context.Prestamo
                    .Where(p => p.FechaPrestamo >= inicio && p.FechaPrestamo <= fin)
                    .Select(p => new
                    {
                        p.Id,
                        p.UsuarioId,
                        p.EjemplarId,
                        p.FechaPrestamo,
                        p.FechaVencimiento,
                        p.FechaDevolucion,
                        p.Penalizacion
                    })
                    .ToListAsync();

                // 3️⃣ Validar si se encontraron datos
                if (!prestamos.Any())
                {
                    return new OperationResult<Reporte>
                    {
                        Success = false,
                        Message = "No se encontraron préstamos en el rango de fechas especificado."
                    };
                }

                // 4️⃣ Crear el contenido del reporte
                var contenido = new StringBuilder();
                contenido.AppendLine("===== REPORTE DE PRÉSTAMOS =====");
                contenido.AppendLine($"Periodo: {inicio:dd/MM/yyyy} - {fin:dd/MM/yyyy}");
                contenido.AppendLine($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}");
                contenido.AppendLine("=================================\n");

                foreach (var p in prestamos)
                {
                    contenido.AppendLine($"ID Préstamo: {p.Id}");
                    contenido.AppendLine($"Usuario ID: {p.UsuarioId}");
                    contenido.AppendLine($"Ejemplar ID: {p.EjemplarId}");
                    contenido.AppendLine($"Fecha préstamo: {p.FechaPrestamo:dd/MM/yyyy}");
                    contenido.AppendLine($"Fecha vencimiento: {p.FechaVencimiento:dd/MM/yyyy}");
                    contenido.AppendLine($"Devolución: {(p.FechaDevolucion.HasValue ? p.FechaDevolucion.Value.ToString("dd/MM/yyyy") : "Pendiente")}");
                    contenido.AppendLine($"Penalización: {p.Penalizacion:C}");
                    contenido.AppendLine("---------------------------------\n");
                }

                // 5️⃣ Crear el objeto Reporte
                var reporte = new Reporte
                {
                    Tipo = "Préstamos",
                    Contenido = contenido.ToString(),
                    FechaGeneracion = DateTime.Now
                };

                // 6️⃣ Guardar en base de datos
                _context.Reporte.Add(reporte);
                await _context.SaveChangesAsync();

                // 7️⃣ Retornar resultado exitoso
                return new OperationResult<Reporte>
                {
                    Success = true,
                    Message = "Reporte de préstamos generado correctamente.",
                    Data = reporte
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de préstamos.");
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "Error al generar el reporte de préstamos."
                };
            }
        }


        #endregion
    }
}


