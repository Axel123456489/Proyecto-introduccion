namespace introduccion.Models;

public class Pasajero
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;

    public string NombreMostrar => $"{Nombre} ({Documento})";

    public override string ToString() => NombreMostrar;
}
