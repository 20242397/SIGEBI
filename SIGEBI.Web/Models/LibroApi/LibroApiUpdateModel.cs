namespace SIGEBI.Web.Models.LibroApi
{
    public class LibroApiUpdateModel
    {
        public int Id { get; set; }
        public string? Titulo { get; set; }
        public string? Autor { get; set; }
        public string? ISBN { get; set; }
        public string? Editorial { get; set; }
        public int? AñoPublicacion { get; set; }
        public string? Categoria { get; set; }
        public string? Estado { get; set; }
    }
}
