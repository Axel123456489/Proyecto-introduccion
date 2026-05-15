using System.Collections.Generic;
using System.Linq;
using introduccion.Database;
using introduccion.Models;

namespace introduccion.Services;

public class ServicioBuses
{
    private readonly ServicioBaseDatos _servicioBaseDatos;
    private readonly ServicioHistorial? _servicioHistorial;

    public ServicioBuses(ServicioBaseDatos servicioBaseDatos, ServicioHistorial? servicioHistorial = null)
    {
        _servicioBaseDatos = servicioBaseDatos;
        _servicioHistorial = servicioHistorial;
    }

    public void AgregarBus(Bus bus)
    {
        bus.Id = ObtenerSiguienteIdBus();
        _servicioBaseDatos.Insertar(bus);
        _servicioBaseDatos.GuardarCambios();
        _servicioHistorial?.RegistrarAccion($"Se registro el bus {bus.Placa}.");
    }

    public List<Bus> ObtenerBusesDisponibles() => _servicioBaseDatos.Tabla<Bus>().Where(bus => bus.EstaDisponible).ToList();

    public List<Bus> ObtenerTodosLosBuses() => _servicioBaseDatos.LeerTodos<Bus>();

    public bool CambiarDisponibilidadBus(int id, out bool estaDisponibleAhora)
    {
        estaDisponibleAhora = false;

        var bus = _servicioBaseDatos.Tabla<Bus>().FirstOrDefault(b => b.Id == id);
        if (bus is null)
            return false;

        bus.EstaDisponible = !bus.EstaDisponible;
        estaDisponibleAhora = bus.EstaDisponible;
        _servicioBaseDatos.GuardarCambios();
        _servicioHistorial?.RegistrarAccion($"Se cambio la disponibilidad del bus {bus.Placa}.");
        return true;
    }

    public string ObtenerMensajeDemanda(int cantidadPasajeros)
    {
        if (cantidadPasajeros <= 0)
            return "La demanda debe ser mayor a 0.";

        var busesDisponibles = ObtenerBusesDisponibles().OrderBy(bus => bus.Capacidad).ToList();
        if (!busesDisponibles.Any())
            return "No hay buses disponibles.";

        var busRecomendado = busesDisponibles.FirstOrDefault(bus => bus.Capacidad >= cantidadPasajeros);
        return busRecomendado is null
            ? $"No hay bus apto para {cantidadPasajeros} pasajeros."
            : $"Bus recomendado: {busRecomendado.Placa} (capacidad {busRecomendado.Capacidad}).";
    }

    public Bus? AsignarBusARuta(int cantidadPasajeros)
    {
        if (cantidadPasajeros <= 0)
            return null;

        var busSeleccionado = _servicioBaseDatos.Tabla<Bus>()
            .Where(bus => bus.EstaDisponible && bus.Capacidad >= cantidadPasajeros)
            .OrderBy(bus => bus.Capacidad)
            .FirstOrDefault();

        if (busSeleccionado is null)
            return null;

        busSeleccionado.EstaDisponible = false;
        _servicioBaseDatos.GuardarCambios();
        _servicioHistorial?.RegistrarAccion($"Se asigno el bus {busSeleccionado.Placa} a una ruta para {cantidadPasajeros} pasajeros.");
        return busSeleccionado;
    }

    private int ObtenerSiguienteIdBus()
    {
        return _servicioBaseDatos.Tabla<Bus>().Any()
            ? _servicioBaseDatos.Tabla<Bus>().Max(bus => bus.Id) + 1
            : 1;
    }
}
