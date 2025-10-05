

using Microsoft.OpenApi.Models;
using SIGEBI.Application.Interfaces;
using SIGEBI.Application.Services;
using SIGEBI.Domain.Repository;
using SIGEBI.Persistence.Helpers;
using SIGEBI.Persistence.Repositories.Ado;

var builder = WebApplication.CreateBuilder(args);

// ======================
// 🔧 CONFIGURACIÓN GENERAL
// ======================
builder.Services.AddControllers();

// Swagger con detalles personalizados
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SIGEBI API",
        Version = "v1",
        Description = "API del Sistema de Gestión Bibliotecaria (SIGEBI)",
        Contact = new OpenApiContact
        {
            Name = "Equipo SIGEBI",
            Email = "soporte@sigebi.com"
        }
    });
});

// ======================
// 🧱 INYECCIÓN DE DEPENDENCIAS
// ======================

// Helpers
builder.Services.AddSingleton<DbHelper>();

// Repositorios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryAdo>();
builder.Services.AddScoped<ILibroRepository, LibroRepositoryAdo>();
builder.Services.AddScoped<IPrestamoRepository, PrestamoRepositoryAdo>();

// Servicios de aplicación
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SIGEBI API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz: localhost:5000
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
