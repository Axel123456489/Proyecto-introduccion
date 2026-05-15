namespace introduccion.DataStructures;

public class NodoCiudad
{
    public string Nombre { get; }

    public NodoCiudad(string nombre)
    {
        Nombre = nombre;
    }

    public override string ToString() => Nombre;
}
