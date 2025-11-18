public class ReporteApiUpdateModel
{
    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public bool Resuelto { get; set; }
    public bool MarcarComoResuelto { get; set; }
}
