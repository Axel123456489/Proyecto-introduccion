namespace introduccion.Models;

public class Reserva
{
    public int PasajeroId { get; set; }
    public int RutaId { get; set; }
    public int BusId { get; set; }
    public int NumeroAsiento { get; set; }
    public string Estado { get; set; } = "Creada";
}