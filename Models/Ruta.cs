namespace introduccion.Models;

public class Ruta
{
    public int Id { get; set; }
    public string CiudadOrigen { get; set; } = string.Empty;
    public string CiudadDestino { get; set; } = string.Empty;
    public bool EsBidireccional { get; set; } = true;
    public bool EstaHabilitada { get; set; } = true;

    public string NombreMostrar => EstaHabilitada
        ? $"{CiudadOrigen} -> {CiudadDestino}"
        : $"{CiudadOrigen} -> {CiudadDestino} (Inactiva)";

    public override string ToString() => NombreMostrar;
}