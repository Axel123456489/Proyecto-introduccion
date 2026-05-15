namespace introduccion.Models;

public class Conductor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Licencia { get; set; } = string.Empty;
    public int HorasTrabajadas { get; set; }
    public int MaxHorasPorDia { get; set; } = 8;
    public int? BusAsignadoId { get; set; }

    public string NombreMostrar => $"{Nombre} ({Licencia})";

    public override string ToString() => NombreMostrar;
}