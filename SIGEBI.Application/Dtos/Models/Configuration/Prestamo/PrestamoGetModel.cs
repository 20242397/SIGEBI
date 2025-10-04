namespace SIGEBI.Application.Dtos.Models.Configuration.Prestamo
{
    public class PrestamoGetModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EjemplarId { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public decimal? Penalizacion { get; set; }



        public record PrestamoCreateDto(
            int UsuarioId,
            int EjemplarId,
            DateTime FechaPrestamo,
            DateTime FechaVencimiento
        )
        {
            public string? LibroId { get; internal set; }
        }

        public record PrestamoDevolucionDto(
            int PrestamoId,
            DateTime FechaDevolucion,
            decimal? Penalizacion
        );

        public record PrestamoPenalizacionDto(
            int PrestamoId,
            decimal Monto,
            string Motivo
        );

        public record PrestamoGetDto(
            int Id,
            int UsuarioId,
            int EjemplarId,
            DateTime FechaPrestamo,
            DateTime FechaVencimiento,
            DateTime? FechaDevolucion,
            decimal? Penalizacion
        );
    }

}
