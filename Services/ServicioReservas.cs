using System.Collections.Generic;
using introduccion.Database;
using introduccion.Models;

namespace introduccion.Services;

public class ServicioReservas
{
    private readonly ServicioBaseDatos _servicioBaseDatos;
    private readonly ServicioHistorial? _servicioHistorial;

    public ServicioReservas(ServicioBaseDatos servicioBaseDatos, ServicioHistorial? servicioHistorial = null)
    {
        _servicioBaseDatos = servicioBaseDatos;
        _servicioHistorial = servicioHistorial;
    }

    public Reserva CrearReserva(int pasajeroId, int rutaId, int busId, int numeroAsiento)
    {
        var reserva = new Reserva
        {
            PasajeroId = pasajeroId,
            RutaId = rutaId,
            BusId = busId,
            NumeroAsiento = numeroAsiento,
            Estado = "Creada"
        };

        _servicioBaseDatos.Insertar(reserva);
        _servicioBaseDatos.GuardarCambios();
        _servicioHistorial?.RegistrarAccion($"Se creo una reserva en el asiento {numeroAsiento}.");
        return reserva;
    }

    public List<Reserva> ObtenerReservas()
    {
        return _servicioBaseDatos.LeerTodos<Reserva>();
    }
}
