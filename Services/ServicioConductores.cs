using System.Collections.Generic;
using System.Linq;
using introduccion.Database;
using introduccion.Models;

namespace introduccion.Services;

public class ServicioConductores
{
    private readonly ServicioBaseDatos _servicioBaseDatos;
    private readonly ServicioHistorial? _servicioHistorial;

    public ServicioConductores(ServicioBaseDatos servicioBaseDatos, ServicioHistorial? servicioHistorial = null)
    {
        _servicioBaseDatos = servicioBaseDatos;
        _servicioHistorial = servicioHistorial;
    }

    public void AgregarConductor(Conductor conductor)
    {
        conductor.Id = ObtenerSiguienteIdConductor();
        _servicioBaseDatos.Insertar(conductor);
        _servicioBaseDatos.GuardarCambios();
        _servicioHistorial?.RegistrarAccion($"Se registro el conductor {conductor.Nombre}.");
    }

    public bool EstaConductorDisponible(Conductor conductor)
    {
        return conductor.BusAsignadoId is null && conductor.HorasTrabajadas < conductor.MaxHorasPorDia;
    }

    public List<Conductor> ObtenerConductoresDisponibles()
    {
        return _servicioBaseDatos.Tabla<Conductor>().Where(EstaConductorDisponible).ToList();
    }

    public List<Conductor> ObtenerTodosLosConductores()
    {
        return _servicioBaseDatos.LeerTodos<Conductor>();
    }

    public Conductor? AsignarConductorABus(int busId)
    {
        var conductor = _servicioBaseDatos.Tabla<Conductor>().FirstOrDefault(EstaConductorDisponible);
        if (conductor is null)
            return null;

        conductor.BusAsignadoId = busId;
        _servicioBaseDatos.GuardarCambios();
        _servicioHistorial?.RegistrarAccion($"Se asigno el conductor {conductor.Nombre} al bus {busId}.");
        return conductor;
    }

    public bool LiberarConductor(int conductorId)
    {
        var conductor = _servicioBaseDatos.Tabla<Conductor>().FirstOrDefault(c => c.Id == conductorId);
        if (conductor is null || conductor.BusAsignadoId is null)
            return false;

        var busId = conductor.BusAsignadoId.Value;
        conductor.BusAsignadoId = null;
        _servicioBaseDatos.GuardarCambios();
        _servicioHistorial?.RegistrarAccion($"Se libero el conductor {conductor.Nombre} del bus {busId}.");
        return true;
    }

    private int ObtenerSiguienteIdConductor()
    {
        return _servicioBaseDatos.Tabla<Conductor>().Any()
            ? _servicioBaseDatos.Tabla<Conductor>().Max(conductor => conductor.Id) + 1
            : 1;
    }
}
