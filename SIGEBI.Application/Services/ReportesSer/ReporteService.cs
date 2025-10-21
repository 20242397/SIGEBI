using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Reporte;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.Reportes;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;

namespace SIGEBI.Application.Services.Reportes
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

        // ✅ RF5.1–RF5.3 - Generar un nuevo reporte
        public Task<ServiceResult<T>> GenerarReporteAsync<T>(ReporteCreateDto dto) =>
            ExecuteAsync(async () =>
            {
                var entity = new Reporte
                {
                    UsuarioId = dto.UsuarioId,
                    Tipo = dto.Tipo,
                    Contenido = dto.Contenido ?? "",
                    FechaGeneracion = DateTime.Now
                };

                var validation = ReporteValidator.Validar(entity);
                if (!validation.Success)
                    return new OperationResult<T> { Success = false, Message = validation.Message };

                OperationResult<Reporte> result;

                switch (dto.Tipo.ToLower())
                {
                    case "prestamos":
                        result = await _reporteRepository.GenerarReportePrestamosAsync(dto.FechaInicio, dto.FechaFin);
                        break;
                    case "usuarios activos":
                        result = await _reporteRepository.GenerarReporteUsuariosActivosAsync();
                        break;
                    default:
                        result = new OperationResult<Reporte>
                        {
                            Success = false,
                            Message = "Tipo de reporte no reconocido"
                        };
                        break;
                }

                _logger.LogInformation("Reporte generado: {Tipo} por usuario {UsuarioId}", dto.Tipo, dto.UsuarioId);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };
            });

        // ✅ RF5.4 - Exportar reporte en PDF o Excel
        public Task<ServiceResult<T>> ExportarReporteAsync<T>(int reporteId, string formato) =>
            ExecuteAsync(async () =>
            {
                var reporteResult = await _reporteRepository.GetByIdAsync(reporteId);
                if (!reporteResult.Success || reporteResult.Data == null)
                    return new OperationResult<T> { Success = false, Message = "Reporte no encontrado" };

                var reporte = reporteResult.Data;
                string exportPath = Path.Combine(Environment.CurrentDirectory, "Exportados");

                if (!Directory.Exists(exportPath))
                    Directory.CreateDirectory(exportPath);

                string filePath = Path.Combine(exportPath, $"Reporte_{reporteId}_{DateTime.Now:yyyyMMddHHmmss}.{formato.ToLower()}");

                try
                {
                    await File.WriteAllTextAsync(filePath, reporte.Contenido);

                    _logger.LogInformation("Reporte exportado en formato {Formato}: {Ruta}", formato, filePath);

                    return new OperationResult<T>
                    {
                        Success = true,
                        Message = $"Reporte exportado correctamente en formato {formato}",
                        Data = (T)(object)filePath!
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al exportar el reporte {Id}", reporteId);
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = "Error al exportar el reporte."
                    };
                }
            });

        // ✅ Editar reporte existente (Marcar como resuelto)
        public Task<ServiceResult<T>> ActualizarReporteAsync<T>(ReporteUpdateDto dto) =>
            ExecuteAsync(async () =>
            {
                var reporteResult = await _reporteRepository.GetByIdAsync(dto.Id);
                if (!reporteResult.Success || reporteResult.Data == null)
                    return new OperationResult<T> { Success = false, Message = "Reporte no encontrado" };

                var reporte = reporteResult.Data;
                reporte.Contenido += $"\n\n[Actualizado el {DateTime.Now:dd/MM/yyyy HH:mm}] {dto.Contenido}";
                if (dto.MarcarComoResuelto)
                    await _reporteRepository.MarcarComoResueltoAsync(dto.Id);

                var updateResult = await _reporteRepository.UpdateAsync(reporte);

                _logger.LogInformation("Reporte actualizado (ID {Id})", dto.Id);

                return new OperationResult<T>
                {
                    Success = updateResult.Success,
                    Message = updateResult.Message,
                    Data = (T)(object)updateResult.Data!
                };
            });

        // ✅ RF5.3 - Filtros: obtener por fecha
        public Task<ServiceResult<T>> ObtenerPorFechaAsync<T>(DateTime inicio, DateTime fin) =>
            ExecuteAsync(async () =>
            {
                var result = await _reporteRepository.ObtenerReportesPorFechaAsync(inicio, fin);
                _logger.LogInformation("Reportes obtenidos entre {Inicio} y {Fin}: {Count}", (inicio, fin, result.Data?.Count() ?? 0));

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };
            });

        // ✅ RF5.3 - Filtros: obtener por tipo
        public Task<ServiceResult<T>> ObtenerPorTipoAsync<T>(string tipo) =>
            ExecuteAsync(async () =>
            {
                var result = await _reporteRepository.ObtenerReportesPorTipoAsync(tipo);
                _logger.LogInformation("Reportes obtenidos por tipo {Tipo}: {Count}", (tipo, result.Data?.Count() ?? 0));

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };
            });

        // ✅ Consultar todos los reportes
        public Task<ServiceResult<T>> ObtenerTodosAsync<T>() =>
            ExecuteAsync(async () =>
            {
                var result = await _reporteRepository.GetAllAsync();
                _logger.LogInformation("Consulta de todos los reportes completada: {Count}", (result.Data?.Count() ?? 0) is int count ? count : 0);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };
            });

        // ✅ Consultar un reporte específico
        public Task<ServiceResult<T>> ObtenerPorIdAsync<T>(int id) =>
            ExecuteAsync(async () =>
            {
                var result = await _reporteRepository.GetByIdAsync(id);
                if (!result.Success)
                    _logger.LogWarning("Reporte no encontrado: {Id}", id);

                return new OperationResult<T>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = (T)(object?)result.Data!
                };
            });
    }
}
