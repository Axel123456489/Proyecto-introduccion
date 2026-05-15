using System.Collections.Generic;
using System.Linq;

namespace introduccion.Services;

public class ServicioHistorial
{
    private readonly Stack<string> _pilaHistorial = [];

    public void RegistrarAccion(string accion)
    {
        if (string.IsNullOrWhiteSpace(accion))
            return;

        _pilaHistorial.Push(accion);
    }

    public List<string> ObtenerAccionesRecientes()
    {
        return _pilaHistorial.ToList();
    }
}
