using Microsoft.EntityFrameworkCore;
using SIGEBI.Application.Repositories.Configuration.Reportes;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using SIGEBI.Persistence.Base;
using SIGEBI.Persistence.Context;
using SIGEBI.Persistence.Logging;
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

        #region  Validación con ReporteValidator

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

        #region  Consultas generales

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

        #region  Generación de reportes automáticos

        public async Task<OperationResult<Reporte>> GenerarReportePrestamosAsync(DateTime inicio, DateTime fin, int usuarioId)
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
                    UsuarioId = usuarioId,
                    Tipo = "Reporte de Préstamos",
                    Contenido = contenido.ToString(),
                    FechaGeneracion = DateTime.Now,
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



        public async Task<OperationResult<Reporte>> GenerarReporteUsuariosActivosAsync(int usuarioId)
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
                    UsuarioId = usuarioId,
                    Tipo = "Usuarios Activos",
                    Contenido = contenido.ToString(),
                    FechaGeneracion = DateTime.Now,

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


        public async Task<OperationResult<Reporte>> GenerarReportePenalizacionesAsync(DateTime fechaInicio, DateTime fechaFin, int usuarioId)
        {
            try
            {
                var total = await _context.Prestamo
                    .Where(p => p.Penalizacion > 0 && p.FechaPrestamo >= fechaInicio && p.FechaPrestamo <= fechaFin)
                    .CountAsync();

                var reporte = new Reporte
                {
                    UsuarioId = usuarioId,
                    Tipo = "Penalizaciones",
                    Contenido = $"Penalizaciones registradas entre {fechaInicio:d} y {fechaFin:d}: {total}",
                    FechaGeneracion = DateTime.Now
                };

                await _context.Reporte.AddAsync(reporte);
                await _context.SaveChangesAsync();


                return new OperationResult<Reporte>
                {
                    Success = true,
                    Message = "Reporte de penalizaciones generado correctamente.",
                    Data = reporte
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de penalizaciones");
                return new OperationResult<Reporte> { Success = false, Message = ex.Message };
            }
        }

      
        public async Task<OperationResult<Reporte>> GenerarReporteDevolucionesAsync(DateTime fechaInicio, DateTime fechaFin, int usuarioId)
        {
            try
            {
                var total = await _context.Prestamo
                    .Where(p => p.FechaDevolucion != null &&
                                p.FechaDevolucion >= fechaInicio &&
                                p.FechaDevolucion <= fechaFin)
                    .CountAsync();

                var reporte = new Reporte
                {
                    UsuarioId = usuarioId,
                    Tipo = "Devoluciones",
                    Contenido = $"Total de devoluciones entre {fechaInicio:d} y {fechaFin:d}: {total}",
                    FechaGeneracion = DateTime.Now
                };

                await _context.Reporte.AddAsync(reporte);
                await _context.SaveChangesAsync();


                return new OperationResult<Reporte>
                {
                    Success = true,
                    Message = "Reporte de devoluciones generado correctamente.",
                    Data = reporte
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de devoluciones");
                return new OperationResult<Reporte> { Success = false, Message = ex.Message };
            }
        }

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




        public async Task<OperationResult<Reporte>> GenerarReporteLibrosMasPrestadosAsync(DateTime inicio, DateTime fin)
        {
            try
            {
               
                if (inicio == default || fin == default || fin < inicio)
                {
                    return new OperationResult<Reporte>
                    {
                        Success = false,
                        Message = "El rango de fechas no es válido."
                    };
                }

                
                var prestamosAgrupados = await _context.Prestamo
                    .Include(p => p.Libro) 
                    .Where(p => p.FechaPrestamo >= inicio && p.FechaPrestamo <= fin)
                    .GroupBy(p => new { p.LibroId, p.Libro.Titulo })
                    .Select(g => new
                    {
                        g.Key.LibroId,
                        g.Key.Titulo,
                        TotalPrestamos = g.Count()
                    })
                    .OrderByDescending(x => x.TotalPrestamos)
                    .Take(10)
                    .ToListAsync();

                if (prestamosAgrupados == null || !prestamosAgrupados.Any())
                {
                    return new OperationResult<Reporte>
                    {
                        Success = false,
                        Message = "No se encontraron préstamos en el rango de fechas especificado."
                    };
                }

               
                var sb = new StringBuilder();
                sb.AppendLine(" REPORTE DE LIBROS MÁS PRESTADOS");
                sb.AppendLine($"Periodo: {inicio:dd/MM/yyyy} - {fin:dd/MM/yyyy}");
                sb.AppendLine("------------------------------------------");

                int contador = 1;
                foreach (var libro in prestamosAgrupados)
                {
                    sb.AppendLine($"{contador++}. {libro.Titulo} — {libro.TotalPrestamos} préstamos");
                }

               
                var reporte = new Reporte
                {
                    Tipo = "Libros más prestados",
                    Contenido = sb.ToString(),
                    FechaGeneracion = DateTime.Now,
                    UsuarioId = 1
                };

              
                await _context.Reporte.AddAsync(reporte);
                await _context.SaveChangesAsync();

                return new OperationResult<Reporte>
                {
                    Success = true,
                    Message = "Reporte generado correctamente.",
                    Data = reporte
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de libros más prestados.");
                return new OperationResult<Reporte>
                {
                    Success = false,
                    Message = "Error al generar el reporte. Intente nuevamente."
                };
            }
        }
        #endregion
    }
}
