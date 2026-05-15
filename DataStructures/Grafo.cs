using System;
using System.Collections.Generic;
using System.Linq;

namespace introduccion.DataStructures;

public class Grafo
{
    private readonly Dictionary<string, HashSet<string>> _conexiones = new(StringComparer.OrdinalIgnoreCase);

    public void AgregarConexion(string ciudadOrigen, string ciudadDestino, bool esBidireccional = true)
    {
        if (string.IsNullOrWhiteSpace(ciudadOrigen) || string.IsNullOrWhiteSpace(ciudadDestino))
            return;

        AgregarConexionInterna(ciudadOrigen.Trim(), ciudadDestino.Trim());

        if (esBidireccional)
            AgregarConexionInterna(ciudadDestino.Trim(), ciudadOrigen.Trim());
    }

    public List<NodoCiudad> BuscarRutaPorAmplitud(string ciudadOrigen, string ciudadDestino)
    {
        if (string.IsNullOrWhiteSpace(ciudadOrigen) || string.IsNullOrWhiteSpace(ciudadDestino))
            return [];

        ciudadOrigen = ciudadOrigen.Trim();
        ciudadDestino = ciudadDestino.Trim();

        if (!_conexiones.ContainsKey(ciudadOrigen) || !_conexiones.ContainsKey(ciudadDestino))
            return [];

        var cola = new Queue<string>();
        var visitados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var anteriores = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        cola.Enqueue(ciudadOrigen);
        visitados.Add(ciudadOrigen);
        anteriores[ciudadOrigen] = null;

        while (cola.Count > 0)
        {
            var ciudadActual = cola.Dequeue();

            if (ciudadActual.Equals(ciudadDestino, System.StringComparison.OrdinalIgnoreCase))
                return ReconstruirRuta(ciudadDestino, anteriores);

            foreach (var ciudadVecina in _conexiones[ciudadActual])
            {
                if (visitados.Add(ciudadVecina))
                {
                    anteriores[ciudadVecina] = ciudadActual;
                    cola.Enqueue(ciudadVecina);
                }
            }
        }

        return [];
    }

    private void AgregarConexionInterna(string ciudadOrigen, string ciudadDestino)
    {
        if (!_conexiones.TryGetValue(ciudadOrigen, out var vecinos))
        {
            vecinos = [];
            _conexiones[ciudadOrigen] = vecinos;
        }

        vecinos.Add(ciudadDestino);

        if (!_conexiones.ContainsKey(ciudadDestino))
            _conexiones[ciudadDestino] = [];
    }

    private static List<NodoCiudad> ReconstruirRuta(string ciudadDestino, Dictionary<string, string?> anteriores)
    {
        var ruta = new List<NodoCiudad>();
        var ciudadActual = ciudadDestino;

        while (true)
        {
            ruta.Add(new NodoCiudad(ciudadActual));

            if (!anteriores.TryGetValue(ciudadActual, out var ciudadAnterior) || ciudadAnterior is null)
                break;

            ciudadActual = ciudadAnterior;
        }

        ruta.Reverse();
        return ruta;
    }
}
