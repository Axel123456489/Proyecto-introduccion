using System.Collections.Generic;
using System.Linq;
using introduccion.DataStructures;
using introduccion.Database;
using introduccion.Models;

namespace introduccion.Services;

public class ServicioRutas
{
    private readonly ServicioBaseDatos _servicioBaseDatos;
    private readonly ServicioHistorial? _servicioHistorial;

    public ServicioRutas(ServicioBaseDatos servicioBaseDatos, ServicioHistorial? servicioHistorial = null)
    {
        _servicioBaseDatos = servicioBaseDatos;
        _servicioHistorial = servicioHistorial;
    }

    public void AgregarRuta(string ciudadOrigen, string ciudadDestino, bool esBidireccional = true)
    {
        var nuevaRuta = new Ruta
        {
            Id = ObtenerSiguienteIdRuta(),
            CiudadOrigen = ciudadOrigen,
            CiudadDestino = ciudadDestino,
            EsBidireccional = esBidireccional,
            EstaHabilitada = true
        };

        _servicioBaseDatos.Insertar(nuevaRuta);
        _servicioBaseDatos.GuardarCambios();
        _servicioHistorial?.RegistrarAccion($"Se registro la ruta {ciudadOrigen} a {ciudadDestino}.");
    }

    public List<Ruta> ObtenerTodasLasRutas() => _servicioBaseDatos.LeerTodos<Ruta>();

    public List<Ruta> ObtenerRutasHabilitadas() => _servicioBaseDatos.Tabla<Ruta>().Where(ruta => ruta.EstaHabilitada).ToList();

    public bool DeshabilitarRuta(int rutaId)
    {
        var ruta = _servicioBaseDatos.Tabla<Ruta>().FirstOrDefault(r => r.Id == rutaId);
        if (ruta is null || !ruta.EstaHabilitada)
            return false;

        ruta.EstaHabilitada = false;
        _servicioBaseDatos.GuardarCambios();
        _servicioHistorial?.RegistrarAccion($"Se deshabilito la ruta {ruta.CiudadOrigen} a {ruta.CiudadDestino}.");
        return true;
    }

    public List<NodoCiudad> BuscarRuta(string ciudadOrigen, string ciudadDestino)
    {
        var grafo = new Grafo();

        foreach (var ruta in _servicioBaseDatos.Tabla<Ruta>().Where(ruta => ruta.EstaHabilitada))
            grafo.AgregarConexion(ruta.CiudadOrigen, ruta.CiudadDestino, ruta.EsBidireccional);

        return grafo.BuscarRutaPorAmplitud(ciudadOrigen, ciudadDestino);
    }

    private int ObtenerSiguienteIdRuta()
    {
        return _servicioBaseDatos.Tabla<Ruta>().Any()
            ? _servicioBaseDatos.Tabla<Ruta>().Max(ruta => ruta.Id) + 1
            : 1;
    }
}
