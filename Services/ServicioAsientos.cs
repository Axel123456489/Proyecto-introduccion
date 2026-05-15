using System.Collections.Generic;
using System.Linq;
using introduccion.Database;
using introduccion.Models;

namespace introduccion.Services;

public class ServicioAsientos
{
    private readonly ServicioBaseDatos _servicioBaseDatos;
    private readonly ServicioHistorial? _servicioHistorial;
    private readonly HashSet<int> _asientosOcupados = [];

    public ServicioAsientos(ServicioBaseDatos servicioBaseDatos, ServicioHistorial? servicioHistorial = null)
    {
        _servicioBaseDatos = servicioBaseDatos;
        _servicioHistorial = servicioHistorial;
        CargarAsientosOcupados();
    }

    private void CargarAsientosOcupados()
    {
        _asientosOcupados.Clear();

        foreach (var asiento in _servicioBaseDatos.Tabla<introduccion.Models.Reserva>()
                     .Where(reserva => reserva.Estado == "Creada")
                     .Select(reserva => reserva.NumeroAsiento)
                     .Distinct())
        {
            _asientosOcupados.Add(asiento);
        }
    }

    public bool EstaAsientoDisponible(int numeroAsiento)
    {
        if (numeroAsiento <= 0)
            return false;

        return !_asientosOcupados.Contains(numeroAsiento);
    }

    public bool ReservarAsiento(int numeroAsiento)
    {
        if (!EstaAsientoDisponible(numeroAsiento))
            return false;

        _asientosOcupados.Add(numeroAsiento);
        _servicioHistorial?.RegistrarAccion($"Se reservo el asiento {numeroAsiento}.");
        return true;
    }

    public bool LiberarAsiento(int numeroAsiento)
    {
        if (numeroAsiento <= 0)
            return false;

        var asientoLiberado = _asientosOcupados.Remove(numeroAsiento);
        if (!asientoLiberado)
            return false;

        var reservaActiva = _servicioBaseDatos.Tabla<Reserva>()
            .FirstOrDefault(reserva => reserva.NumeroAsiento == numeroAsiento && reserva.Estado == "Creada");

        if (reservaActiva is not null)
        {
            reservaActiva.Estado = "Liberada";
            _servicioBaseDatos.GuardarCambios();
        }

        _servicioHistorial?.RegistrarAccion($"Se libero el asiento {numeroAsiento}.");
        return true;
    }
}
