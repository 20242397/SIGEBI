public class ReporteApiUpdateModel
{
    public string Tipo;

    public int Id { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public bool Resuelto { get; set; }
    public bool MarcarComoResuelto { get; set; }
}
