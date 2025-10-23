using Microsoft.Extensions.Logging;
using SIGEBI.Application.Base;
using SIGEBI.Application.Dtos.Models.Configuration.Reporte;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Repositories.Configuration.Reportes;
using SIGEBI.Application.Validators;
using SIGEBI.Domain.Base;
using SIGEBI.Domain.Entitines.Configuration.Reportes;

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

        // ✅ RF5.1–RF5.3 - Generar un nuevo reporte
       public Task<ServiceResult<T>> GenerarReporteAsync<T>(ReporteCreateDto dto) =>
      ExecuteAsync(async () =>
      {
        // 1️⃣ Crear entidad base
        var entity = new Reporte
        {
            UsuarioId = dto.UsuarioId,
            Tipo = dto.Tipo,
            Contenido = dto.Contenido ?? "",
            FechaGeneracion = DateTime.Now
        };

        // 2️⃣ Validar los campos básicos
        var validation = ReporteValidator.Validar(entity);
        if (!validation.Success)
            return new OperationResult<T> { Success = false, Message = validation.Message };

        // 3️⃣ Inicializar resultado por defecto
        OperationResult<Reporte> result = new OperationResult<Reporte>
        {
            Success = false,
            Message = "Tipo de reporte no reconocido."
        };

        // 4️⃣ Tipos de reporte disponibles
        switch (dto.Tipo.ToLower())
        {
            case "prestamos":
                result = await _reporteRepository.GenerarReportePrestamosAsync(dto.FechaInicio, dto.FechaFin, dto.UsuarioId);
                break;

            case "usuarios activos":
                result = await _reporteRepository.GenerarReporteUsuariosActivosAsync(dto.UsuarioId);
                break;

            case "penalizaciones":
                result = await _reporteRepository.GenerarReportePenalizacionesAsync(dto.FechaInicio, dto.FechaFin, dto.UsuarioId);
                break;

            case "devoluciones":
                result = await _reporteRepository.GenerarReporteDevolucionesAsync(dto.FechaInicio, dto.FechaFin, dto.UsuarioId);
                break;
        }

        // 5️⃣ Log de auditoría
        _logger.LogInformation("Reporte generado de tipo {tipo} por usuario con ID {usuarioId}", dto.Tipo, dto.UsuarioId);

        // 6️⃣ Devolver resultado genérico
        return new OperationResult<T>
        {
            Success = result.Success,
            Message = result.Message ?? "Sin mensaje.",
            Data = (T?)(object?)result.Data!
        };
      });


        // ✅ RF5.4 - Exportar reporte en PDF o Excel
        public Task<ServiceResult<T>> ExportarReporteAsync<T>(int reporteId, string formato) =>
      ExecuteAsync(async () =>
      {
          // 🧩 1️⃣ VALIDACIÓN DE NEGOCIO (ENTRADA)
          // --------------------------------------
          if (string.IsNullOrWhiteSpace(formato))
          {
              _logger.LogWarning("Intento de exportar reporte sin especificar formato. ID: {Id}", reporteId);
              return new OperationResult<T>
              {
                  Success = false,
                  Message = "Debe especificar un formato de exportación."
              };
          }

          if (formato.ToLower() != "pdf" && formato.ToLower() != "xlsx" && formato.ToLower() != "txt")
          {
              _logger.LogWarning("Formato no permitido para reporte {Id}: {Formato}", reporteId, formato);
              return new OperationResult<T>
              {
                  Success = false,
                  Message = "Formato no permitido. Solo se admiten PDF, XLSX o TXT."
              };
          }


          // 🔍 2️⃣ VALIDACIÓN DE EXISTENCIA
          // --------------------------------------
          var reporteResult = await _reporteRepository.GetByIdAsync(reporteId);
          if (!reporteResult.Success || reporteResult.Data == null)
          {
              _logger.LogWarning("Intento de exportar un reporte inexistente. ID: {Id}", reporteId);
              return new OperationResult<T> { Success = false, Message = "Reporte no encontrado" };
          }

          var reporte = reporteResult.Data;


          // 📁 3️⃣ PREPARACIÓN DE ENTORNO
          // --------------------------------------
          string exportPath = Path.Combine(Environment.CurrentDirectory, "Exportados");
          if (!Directory.Exists(exportPath))
          {
              Directory.CreateDirectory(exportPath);
              _logger.LogInformation("Carpeta 'Exportados' creada automáticamente en {Ruta}", exportPath);
          }

          string filePath = Path.Combine(exportPath, $"Reporte_{reporteId}_{DateTime.Now:yyyyMMddHHmmss}.{formato.ToLower()}");


          // 💾 4️⃣ EJECUCIÓN PRINCIPAL
          // --------------------------------------
          try
          {
              await File.WriteAllTextAsync(filePath, reporte.Contenido);


              // 🧠 5️⃣ LOG CORRECTO (OPERACIÓN EXITOSA)
              // --------------------------------------
              _logger.LogInformation(
                  "Reporte exportado correctamente. ID: {Id}, Formato: {Formato}, Ruta: {Ruta}",
                  reporteId, formato, filePath
              );


              // 📤 6️⃣ RESPUESTA FINAL
              // --------------------------------------
              return new OperationResult<T>
              {
                  Success = true,
                  Message = $"Reporte exportado correctamente en formato {formato}",
                  Data = (T)(object)filePath!
              };
          }
          catch (Exception ex)
          {
              // 🧠 LOG DE ERROR (FALLA DE NEGOCIO O IO)
              // --------------------------------------
              _logger.LogError(ex, "Error al exportar el reporte ID {Id} en formato {Formato}", reporteId, formato);

              return new OperationResult<T>
              {
                  Success = false,
                  Message = "Error al exportar el reporte. Intente nuevamente."
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
                _logger.LogInformation("Reportes obtenidos entre {Inicio} y {Fin}: {Count}", new object?[] { inicio, fin, result.Data?.Count ?? 0 });

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
                _logger.LogInformation("Reportes obtenidos por tipo {Tipo}: {Count}", new object?[] { tipo, result.Data?.Count ?? 0 });


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
                _logger.LogInformation("Consulta de todos los reportes completada: {Count}", new object?[] { result.Data?.Count ?? 0 });


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
