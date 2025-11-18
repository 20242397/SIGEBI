using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Reporte;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.Reportes;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text;

namespace SIGEBI.Application.Services.ReportesSer
{
    public sealed class ReporteService : BaseService, IReporteService
    {
        private readonly IReporteRepository _reporteRepository;
        private readonly ILogger<ReporteService> _logger;

        public ReporteService(IReporteRepository reporteRepository, ILogger<ReporteService> logger)
        {
            _reporteRepository = reporteRepository;
            _logger = logger;
        }

    
        public Task<ServiceResult<T>> GenerarReporteAsync<T>(ReporteCreateDto dto) =>
            ExecuteAsync(async () =>
            {
                if (dto == null)
                    return Fail<T>("Los datos del reporte no pueden ser nulos.");

                var entity = new Reporte
                {
                    UsuarioId = dto.UsuarioId,
                    Tipo = dto.Tipo ?? "Desconocido",
                    Contenido = dto.Contenido ?? string.Empty,
                    FechaGeneracion = DateTime.Now
                };

                var validacion = ReporteValidator.Validar(entity);
                if (!validacion.Success)
                    return Fail<T>(validacion.Message);

                OperationResult<Reporte> result;

                switch (dto.Tipo?.ToLower())
                {
                    case "prestamos":
                        result = await _reporteRepository.GenerarReportePrestamosAsync(
                            dto.FechaInicio ?? DateTime.MinValue,
                            dto.FechaFin ?? DateTime.Now,
                            dto.UsuarioId);
                        break;

                    case "usuarios activos":
                        result = await _reporteRepository.GenerarReporteUsuariosActivosAsync(dto.UsuarioId);
                        break;

                    case "penalizaciones":
                        result = await _reporteRepository.GenerarReportePenalizacionesAsync(
                            dto.FechaInicio ?? DateTime.MinValue,
                            dto.FechaFin ?? DateTime.Now,
                            dto.UsuarioId);
                        break;

                    case "devoluciones":
                        result = await _reporteRepository.GenerarReporteDevolucionesAsync(
                            dto.FechaInicio ?? DateTime.MinValue,
                            dto.FechaFin ?? DateTime.Now,
                            dto.UsuarioId);
                        break;

                    case "libros mas prestados":
                        result = await _reporteRepository.GenerarReporteLibrosMasPrestadosAsync(
                            dto.FechaInicio ?? DateTime.MinValue,
                            dto.FechaFin ?? DateTime.Now);
                        break;

                    default:
                        _logger.LogWarning("Tipo de reporte no reconocido: {Tipo}", dto.Tipo);
                        return Fail<T>("Tipo de reporte no reconocido.");
                }

                if (!result.Success || result.Data == null)
                    return Fail<T>(result.Message);

                var dtoMapped = new ReporteGetDto
                {
                    Id = result.Data.Id,
                    UsuarioId = result.Data.UsuarioId ?? 0,
                    Tipo = result.Data.Tipo ?? string.Empty,
                    Contenido = result.Data.Contenido ?? string.Empty,
                    FechaGeneracion = result.Data.FechaGeneracion
                };

                _logger.LogInformation("Reporte generado de tipo {Tipo} por usuario {UsuarioId}", dto.Tipo, dto.UsuarioId);

                return Ok((T)(object)dtoMapped, result.Message);
            });

      
         public Task<ServiceResult<T>> ExportarReporteAsync<T>(int reporteId, string formato) =>
       ExecuteAsync(async () =>
       {
        
        if (string.IsNullOrWhiteSpace(formato))
            return Fail<T>("Debe especificar un formato de exportación.");

        formato = formato.Trim().ToLower();

        
        var reporteResult = await _reporteRepository.GetByIdAsync(reporteId);
        if (!reporteResult.Success || reporteResult.Data is null)
            return Fail<T>("Reporte no encontrado.");

        var reporte = reporteResult.Data;

       
        var carpeta = Path.Combine(Environment.CurrentDirectory, "Exportados");
        Directory.CreateDirectory(carpeta);

        var extension = formato == "pdf" ? "pdf" : "txt";  
        var rutaArchivo = Path.Combine(
            carpeta,
            $"Reporte_{reporteId}_{DateTime.Now:yyyyMMddHHmmss}.{extension}"
           );

        try
        {
            if (formato == "txt")
            {
               
                var contenido = new StringBuilder();
                contenido.AppendLine("===== SIGEBI - Reporte =====");
                contenido.AppendLine($"ID: {reporte.Id}");
                contenido.AppendLine($"Usuario ID: {reporte.UsuarioId}");
                contenido.AppendLine($"Tipo: {reporte.Tipo}");
                contenido.AppendLine($"Fecha: {reporte.FechaGeneracion:dd/MM/yyyy HH:mm}");
                contenido.AppendLine("--------------------------------------------");
                contenido.AppendLine(reporte.Contenido ?? "Sin contenido disponible.");
                await File.WriteAllTextAsync(rutaArchivo, contenido.ToString());

                return Ok((T)(object)rutaArchivo, "Reporte exportado correctamente (TXT).");
            }
            else if (formato == "pdf")
            {
              
                QuestPDF.Settings.License = LicenseType.Community;

                var documento = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.Size(PageSizes.A4);

                       
                        page.Header().Row(row =>
                        {
                            row.RelativeItem()
                               .AlignLeft()
                               .Text("📚 SIGEBI - Sistema de Gestión de Bibliotecas")
                               .FontSize(16)
                               .Bold()
                               .FontColor(Colors.Blue.Medium);
                        });

                      
                        page.Content().PaddingVertical(20).Column(col =>
                        {
                            col.Spacing(10);

                            col.Item().Text($"📅 Fecha de generación: {reporte.FechaGeneracion:dd/MM/yyyy HH:mm}")
                                .FontSize(11)
                                .FontColor(Colors.Grey.Darken2);

                            col.Item().Text($"👤 Usuario ID: {reporte.UsuarioId}")
                                .FontSize(11)
                                .FontColor(Colors.Grey.Darken2);

                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            col.Item().Text($"🧾 Tipo de Reporte: {reporte.Tipo}")
                                .FontSize(13)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2)
                                .Underline();

                          
                            col.Item().PaddingTop(10).Text(txt =>
                            {
                                txt.DefaultTextStyle(t => t
                                    .FontSize(11)
                                    .FontColor(Colors.Grey.Darken3)
                                    .LineHeight(1.4f));
                                txt.AlignLeft();
                                txt.Span(reporte.Contenido ?? "Sin contenido disponible.");
                            });
                        });

                       
                        page.Footer()
                            .AlignRight()
                            .Text($"Generado automáticamente por SIGEBI - {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);
                    });
                });

                documento.GeneratePdf(rutaArchivo);

                return Ok((T)(object)rutaArchivo, "Reporte exportado correctamente (PDF).");
            }
            else
            {
                return Fail<T>("Formato no permitido. Use 'pdf' o 'txt'.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al exportar el reporte {Id} en formato {Formato}", reporteId, formato);
            return Fail<T>("Error al exportar el reporte. Intente nuevamente.");
        }
    });


    public Task<ServiceResult<T>> ActualizarReporteAsync<T>(ReporteUpdateDto dto) =>
            ExecuteAsync(async () =>
            {
                var reporteResult = await _reporteRepository.GetByIdAsync(dto.Id);
                if (!reporteResult.Success || reporteResult.Data == null)
                    return Fail<T>("Reporte no encontrado.");

                var reporte = reporteResult.Data;
                reporte.Contenido += $"\n\n[Actualizado el {DateTime.Now:dd/MM/yyyy HH:mm}] {dto.Contenido}";

                if (dto.MarcarComoResuelto)
                    await _reporteRepository.MarcarComoResueltoAsync(dto.Id);

                var update = await _reporteRepository.UpdateAsync(reporte);

                var dtoMapped = new ReporteGetDto
                {
                    Id = reporte.Id,
                    UsuarioId = reporte.UsuarioId ?? 0,
                    Tipo = reporte.Tipo ?? string.Empty,
                    Contenido = reporte.Contenido ?? string.Empty,
                    FechaGeneracion = reporte.FechaGeneracion
                };

                return update.Success
                    ? Ok((T)(object)dtoMapped, "Reporte actualizado correctamente.")
                    : Fail<T>("No se pudo actualizar el reporte.");
            });

      
        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
            ExecuteAsync(async () =>
            {
                var result = await _reporteRepository.GetAllAsync();
                if (!result.Success)
                    return Fail<T>(result.Message);

                var lista = result.Data.Select(r => new ReporteGetDto
                {
                    Id = r.Id,
                    UsuarioId = r.UsuarioId ?? 0,
                    Tipo = r.Tipo ?? string.Empty,
                    Contenido = r.Contenido ?? string.Empty,
                    FechaGeneracion = r.FechaGeneracion
                }).ToList();

                return Ok((T)(object)lista);
            });

       
        public Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id) =>
            ExecuteAsync(async () =>
            {
                var result = await _reporteRepository.GetByIdAsync(id);
                if (!result.Success || result.Data == null)
                    return Fail<T>("Reporte no encontrado.");

                var dto = new ReporteGetDto
                {
                    Id = result.Data.Id,
                    UsuarioId = result.Data.UsuarioId ?? 0,
                    Tipo = result.Data.Tipo ?? string.Empty,
                    Contenido = result.Data.Contenido ?? string.Empty,
                    FechaGeneracion = result.Data.FechaGeneracion
                };

                return Ok((T)(object)dto);
            });

        public Task<ServiceResult<T>> ObtenerPorTipoAsync<T>(string tipo) =>
            ExecuteAsync(async () =>
            {
                var result = await _reporteRepository.ObtenerReportesPorTipoAsync(tipo);
                if (!result.Success)
                    return Fail<T>(result.Message);

                var lista = result.Data.Select(r => new ReporteGetDto
                {
                    Id = r.Id,
                    UsuarioId = r.UsuarioId ?? 0,
                    Tipo = r.Tipo ?? string.Empty,
                    Contenido = r.Contenido ?? string.Empty,
                    FechaGeneracion = r.FechaGeneracion
                }).ToList();

                return Ok((T)(object)lista);
            });

        public Task<ServiceResult<T>> ObtenerPorFechaAsync<T>(DateTime inicio, DateTime fin) =>
            ExecuteAsync(async () =>
            {
                var result = await _reporteRepository.ObtenerReportesPorFechaAsync(inicio, fin);
                if (!result.Success)
                    return Fail<T>(result.Message);

                var lista = result.Data.Select(r => new ReporteGetDto
                {
                    Id = r.Id,
                    UsuarioId = r.UsuarioId ?? 0,
                    Tipo = r.Tipo ?? string.Empty,
                    Contenido = r.Contenido ?? string.Empty,
                    FechaGeneracion = r.FechaGeneracion
                }).ToList();

                return Ok((T)(object)lista);
            });
    }
}
