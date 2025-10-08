namespace SIGEBI.Configuracion.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1️⃣ Agregar controladores y Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // 2️⃣ Activar Swagger solo en entorno de desarrollo
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 3️⃣ Configuración de enrutamiento y autorización
            app.UseRouting();
            app.UseAuthorization();

            // 4️⃣ Mapear controladores
            app.MapControllers();

            // 5️⃣ Ejecutar la API
            app.Run();
        }
    }
}
