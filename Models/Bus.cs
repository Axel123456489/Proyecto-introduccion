namespace introduccion.Models;

public class Bus
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public int Capacidad { get; set; }
    public bool EstaDisponible { get; set; } = true;

    public string NombreMostrar => $"{Placa} (Cap: {Capacidad}) - {(EstaDisponible ? "Habilitado" : "Deshabilitado")}";
}