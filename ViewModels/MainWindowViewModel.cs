using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using introduccion.Data;
using introduccion.Database;
using introduccion.Models;
using introduccion.Services;

namespace introduccion.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ContextoDatos _contextoDatos;
    private readonly ServicioBaseDatos _servicioBaseDatos;
    private readonly ServicioRutas _servicioRutas;
    private readonly ServicioBuses _servicioBuses;
    private readonly ServicioConductores _servicioConductores;
    private readonly ServicioAsientos _servicioAsientos;
    private readonly ServicioReservas _servicioReservas;
    private readonly ServicioHistorial _servicioHistorial;

    public ObservableCollection<string> ResumenRutas { get; } = new();
    public ObservableCollection<Ruta> RutasRegistradas { get; } = new();
    public ObservableCollection<Pasajero> PasajerosRegistrados { get; } = new();
    public ObservableCollection<Bus> BusesRegistrados { get; } = new();
    public ObservableCollection<Conductor> ConductoresRegistrados { get; } = new();
    public ObservableCollection<string> ResumenBuses { get; } = new();
    public ObservableCollection<string> ResumenConductores { get; } = new();
    public ObservableCollection<string> ResumenConductoresRegistrados { get; } = new();
    public ObservableCollection<string> ResumenReservas { get; } = new();
    public ObservableCollection<string> ResumenPasajeros { get; } = new();
    public ObservableCollection<string> AccionesRecientes { get; } = new();

    [ObservableProperty]
    private string _mensajeGeneral = "Sistema listo.";

    [ObservableProperty]
    private string _rutaSugerida = string.Empty;

    [ObservableProperty]
    private string _origenBusqueda = "Lima";

    [ObservableProperty]
    private string _destinoBusqueda = "Arequipa";

    [ObservableProperty]
    private int _demandaBusqueda = 28;

    [ObservableProperty]
    private string _mensajeDemanda = string.Empty;

    [ObservableProperty]
    private string _nuevoOrigen = string.Empty;

    [ObservableProperty]
    private string _nuevoDestino = string.Empty;

    [ObservableProperty]
    private string _placaBus = string.Empty;

    [ObservableProperty]
    private int _capacidadBus = 40;

    [ObservableProperty]
    private string _nombreConductor = string.Empty;

    [ObservableProperty]
    private string _licenciaConductor = string.Empty;

    [ObservableProperty]
    private int _horasConductor;

    [ObservableProperty]
    private string _nombrePasajero = string.Empty;

    [ObservableProperty]
    private string _documentoPasajero = string.Empty;

    [ObservableProperty]
    private int _asientoReservaId = 13;

    [ObservableProperty]
    private Pasajero? _pasajeroSeleccionadoReserva;

    [ObservableProperty]
    private Ruta? _rutaSeleccionadaReserva;

    [ObservableProperty]
    private Bus? _busSeleccionadoReserva;

    [ObservableProperty]
    private string _mensajeReserva = string.Empty;

    [ObservableProperty]
    private int _asientoLiberarId = 12;

    [ObservableProperty]
    private string _mensajeAsiento = string.Empty;

    [ObservableProperty]
    private Bus? _busSeleccionadoConductor;

    [ObservableProperty]
    private Bus? _busSeleccionadoDisponibilidad;

    [ObservableProperty]
    private Conductor? _conductorSeleccionadoGestion;

    [ObservableProperty]
    private string _mensajeConductor = string.Empty;

    [ObservableProperty]
    private int _rutaDeshabilitarId = 1;

    [ObservableProperty]
    private string _mensajeRuta = string.Empty;

    [ObservableProperty]
    private Ruta? _rutaSeleccionadaEstado;

    [ObservableProperty]
    private string _textoBotonRutaEstado = "Deshabilitar";

    [ObservableProperty]
    private int _busDisponibilidadId = 1;

    [ObservableProperty]
    private string _mensajeBus = string.Empty;

    [ObservableProperty]
    private string _mensajePasajero = string.Empty;

    public MainWindowViewModel()
    {
        _contextoDatos = new ContextoDatos();
        _servicioBaseDatos = new ServicioBaseDatos(_contextoDatos);
        _servicioHistorial = new ServicioHistorial();
        _servicioRutas = new ServicioRutas(_servicioBaseDatos, _servicioHistorial);
        _servicioBuses = new ServicioBuses(_servicioBaseDatos, _servicioHistorial);
        _servicioConductores = new ServicioConductores(_servicioBaseDatos, _servicioHistorial);
        _servicioAsientos = new ServicioAsientos(_servicioBaseDatos, _servicioHistorial);
        _servicioReservas = new ServicioReservas(_servicioBaseDatos, _servicioHistorial);

        CargarDatosIniciales();
        RefrescarVista();
    }

    private void CargarDatosIniciales()
    {
        if (!_servicioBaseDatos.Existe<Ruta>(_ => true))
        {
            _servicioRutas.AgregarRuta("Lima", "Ica");
            _servicioRutas.AgregarRuta("Ica", "Arequipa");
        }

        if (!_servicioBaseDatos.Existe<Bus>(_ => true))
        {
            _servicioBuses.AgregarBus(new Bus { Placa = "ABC-111", Capacidad = 40, EstaDisponible = true });
            _servicioBuses.AgregarBus(new Bus { Placa = "ABC-222", Capacidad = 30, EstaDisponible = true });
        }

        if (!_servicioBaseDatos.Existe<Conductor>(_ => true))
        {
            _servicioConductores.AgregarConductor(new Conductor
            {
                Nombre = "Luis Perez",
                Licencia = "A3C-1001",
                HorasTrabajadas = 4,
                MaxHorasPorDia = 8
            });

            _servicioConductores.AgregarConductor(new Conductor
            {
                Nombre = "Carlos Rojas",
                Licencia = "A3C-1002",
                HorasTrabajadas = 2,
                MaxHorasPorDia = 8
            });
        }

        if (!_servicioBaseDatos.Existe<Pasajero>(_ => true))
        {
            _servicioBaseDatos.Insertar(new Pasajero
            {
                Nombre = "Ana Torres",
                Documento = "77889900"
            });
            _servicioBaseDatos.GuardarCambios();
        }

        if (!_servicioBaseDatos.Existe<Reserva>(_ => true))
        {
            var pasajero = _servicioBaseDatos.Tabla<Pasajero>().First();
            var ruta = _servicioBaseDatos.Tabla<Ruta>().First();
            var bus = _servicioBaseDatos.Tabla<Bus>().First();

            if (_servicioAsientos.EstaAsientoDisponible(12))
            {
                _servicioAsientos.ReservarAsiento(12);
                _servicioReservas.CrearReserva(pasajero.Id, ruta.Id, bus.Id, 12);
            }
        }
    }

    private void RefrescarVista()
    {
        ResumenRutas.Clear();
        RutasRegistradas.Clear();
        PasajerosRegistrados.Clear();
        BusesRegistrados.Clear();
        foreach (var ruta in _servicioRutas.ObtenerTodasLasRutas())
        {
            ResumenRutas.Add(ruta.NombreMostrar);
            RutasRegistradas.Add(ruta);
        }

        ResumenBuses.Clear();
        foreach (var bus in _servicioBuses.ObtenerTodosLosBuses())
        {
            ResumenBuses.Add(bus.NombreMostrar);
            BusesRegistrados.Add(bus);
        }

        ResumenConductores.Clear();
        ConductoresRegistrados.Clear();
        foreach (var conductor in _servicioConductores.ObtenerConductoresDisponibles())
            ResumenConductores.Add(conductor.NombreMostrar);

        ResumenConductoresRegistrados.Clear();
        foreach (var conductor in _servicioConductores.ObtenerTodosLosConductores())
        {
            ConductoresRegistrados.Add(conductor);
            ResumenConductoresRegistrados.Add($"{conductor.NombreMostrar} - Bus: {(conductor.BusAsignadoId is null ? "Sin asignar" : conductor.BusAsignadoId.ToString())}");
        }

        if (RutaSeleccionadaReserva is null)
            RutaSeleccionadaReserva = RutasRegistradas.FirstOrDefault();

        ResumenReservas.Clear();
        foreach (var reserva in _servicioReservas.ObtenerReservas())
            ResumenReservas.Add(ConstruirResumenReserva(reserva));

        ResumenPasajeros.Clear();
        foreach (var pasajero in _servicioBaseDatos.LeerTodos<Pasajero>())
        {
            ResumenPasajeros.Add(pasajero.NombreMostrar);
            PasajerosRegistrados.Add(pasajero);
        }

        if (PasajeroSeleccionadoReserva is null)
            PasajeroSeleccionadoReserva = PasajerosRegistrados.FirstOrDefault();

        if (BusSeleccionadoReserva is null)
            BusSeleccionadoReserva = BusesRegistrados.FirstOrDefault();

        if (BusSeleccionadoConductor is null)
            BusSeleccionadoConductor = BusesRegistrados.FirstOrDefault();

        if (BusSeleccionadoDisponibilidad is null)
            BusSeleccionadoDisponibilidad = BusesRegistrados.FirstOrDefault();

        if (ConductorSeleccionadoGestion is null)
            ConductorSeleccionadoGestion = ConductoresRegistrados.FirstOrDefault();

        AccionesRecientes.Clear();
        foreach (var accion in _servicioHistorial.ObtenerAccionesRecientes())
            AccionesRecientes.Add(accion);

        var rutaBuscada = _servicioRutas.BuscarRuta("Lima", "Arequipa");
        RutaSugerida = rutaBuscada.Count == 0
            ? "No se encontró ruta sugerida."
            : string.Join(" -> ", rutaBuscada.Select(ciudad => ciudad.Nombre));

        MensajeGeneral = $"Rutas: {ResumenRutas.Count} | Buses: {ResumenBuses.Count} | Conductores: {ResumenConductores.Count} | Reservas: {ResumenReservas.Count}";
    }

    [RelayCommand]
    private void BuscarRuta()
    {
        var rutaBuscada = _servicioRutas.BuscarRuta(OrigenBusqueda, DestinoBusqueda);
        RutaSugerida = rutaBuscada.Count == 0
            ? "No se encontró ruta sugerida."
            : string.Join(" -> ", rutaBuscada.Select(ciudad => ciudad.Nombre));

        RegistrarAccion($"Se busco ruta de {OrigenBusqueda} a {DestinoBusqueda}.");
    }

    [RelayCommand]
    private void CrearRuta()
    {
        if (string.IsNullOrWhiteSpace(NuevoOrigen) || string.IsNullOrWhiteSpace(NuevoDestino))
        {
            MensajeRuta = "Completa origen y destino.";
            return;
        }

        _servicioRutas.AgregarRuta(NuevoOrigen, NuevoDestino);
        MensajeRuta = "Ruta registrada.";
        RefrescarVista();
    }

    [RelayCommand]
    private void LiberarConductor()
    {
        if (ConductorSeleccionadoGestion is null)
        {
            MensajeConductor = "Selecciona un conductor.";
            return;
        }

        MensajeConductor = _servicioConductores.LiberarConductor(ConductorSeleccionadoGestion.Id)
            ? "Conductor liberado."
            : "No se pudo liberar el conductor.";
        RefrescarVista();
    }

    [RelayCommand]
    private void AgregarBus()
    {
        if (string.IsNullOrWhiteSpace(PlacaBus) || CapacidadBus <= 0)
        {
            MensajeBus = "Revisa la placa y la capacidad.";
            return;
        }

        _servicioBuses.AgregarBus(new Bus
        {
            Placa = PlacaBus,
            Capacidad = CapacidadBus,
            EstaDisponible = true
        });

        MensajeBus = "Bus registrado.";
        RefrescarVista();
    }

    [RelayCommand]
    private void AgregarConductor()
    {
        if (string.IsNullOrWhiteSpace(NombreConductor) || string.IsNullOrWhiteSpace(LicenciaConductor) || HorasConductor < 0)
        {
            MensajeConductor = "Revisa los datos del conductor.";
            return;
        }

        _servicioConductores.AgregarConductor(new Conductor
        {
            Nombre = NombreConductor,
            Licencia = LicenciaConductor,
            HorasTrabajadas = HorasConductor,
            MaxHorasPorDia = 8
        });

        MensajeConductor = "Conductor registrado.";
        RefrescarVista();
    }

    [RelayCommand]
    private void AgregarPasajero()
    {
        if (string.IsNullOrWhiteSpace(NombrePasajero) || string.IsNullOrWhiteSpace(DocumentoPasajero))
        {
            MensajePasajero = "Revisa el nombre y documento.";
            return;
        }

        _servicioBaseDatos.Insertar(new Pasajero
        {
            Nombre = NombrePasajero,
            Documento = DocumentoPasajero
        });
        _servicioBaseDatos.GuardarCambios();

        MensajePasajero = "Pasajero registrado.";
        RefrescarVista();
    }

    [RelayCommand]
    private void EvaluarDemanda()
    {
        MensajeDemanda = _servicioBuses.ObtenerMensajeDemanda(DemandaBusqueda);
        RegistrarAccion($"Se evaluo demanda de {DemandaBusqueda}.");
    }

    [RelayCommand]
    private void CrearReservaManual()
    {
        if (PasajeroSeleccionadoReserva is null || RutaSeleccionadaReserva is null || BusSeleccionadoReserva is null)
        {
            MensajeReserva = "Selecciona pasajero, ruta y bus.";
            return;
        }

        if (!_servicioAsientos.EstaAsientoDisponible(AsientoReservaId))
        {
            MensajeReserva = "Ese asiento ya está ocupado.";
            return;
        }

        try
        {
            _servicioAsientos.ReservarAsiento(AsientoReservaId);
            _servicioReservas.CrearReserva(PasajeroSeleccionadoReserva.Id, RutaSeleccionadaReserva.Id, BusSeleccionadoReserva.Id, AsientoReservaId);
            MensajeReserva = "Reserva creada.";
            RefrescarVista();
        }
        catch
        {
            MensajeReserva = "No se pudo crear la reserva.";
        }
    }

    [RelayCommand]
    private void LiberarAsientoManual()
    {
        MensajeAsiento = _servicioAsientos.LiberarAsiento(AsientoLiberarId)
            ? "Asiento liberado."
            : "No se pudo liberar el asiento.";
        RefrescarVista();
    }

    [RelayCommand]
    private void AsignarConductor()
    {
        if (BusSeleccionadoConductor is null)
        {
            MensajeConductor = "Selecciona un bus.";
            return;
        }

        var conductor = _servicioConductores.AsignarConductorABus(BusSeleccionadoConductor.Id);
        MensajeConductor = conductor is null
            ? "No hay conductor disponible."
            : $"Conductor asignado: {conductor.NombreMostrar}";
        RefrescarVista();
    }

    [RelayCommand]
    private void DeshabilitarRuta()
    {
        if (RutaSeleccionadaEstado is null)
        {
            MensajeRuta = "Selecciona una ruta.";
            return;
        }

        if (RutaSeleccionadaEstado.EstaHabilitada)
        {
            MensajeRuta = _servicioRutas.DeshabilitarRuta(RutaSeleccionadaEstado.Id)
                ? "Ruta deshabilitada."
                : "No se pudo deshabilitar la ruta.";
        }
        else
        {
            RutaSeleccionadaEstado.EstaHabilitada = true;
            _servicioBaseDatos.GuardarCambios();
            MensajeRuta = "Ruta habilitada.";
        }

        TextoBotonRutaEstado = RutaSeleccionadaEstado.EstaHabilitada ? "Deshabilitar" : "Habilitar";
        RefrescarVista();
    }

    partial void OnRutaSeleccionadaEstadoChanged(Ruta? value)
    {
        TextoBotonRutaEstado = value is null
            ? "Deshabilitar"
            : value.EstaHabilitada ? "Deshabilitar" : "Habilitar";
    }

    [RelayCommand]
    private void CambiarDisponibilidadBus()
    {
        if (BusSeleccionadoDisponibilidad is null)
        {
            MensajeBus = "Selecciona un bus.";
            return;
        }

        MensajeBus = _servicioBuses.CambiarDisponibilidadBus(BusSeleccionadoDisponibilidad.Id, out var estaDisponibleAhora)
            ? (estaDisponibleAhora ? "Bus habilitado." : "Bus deshabilitado.")
            : "No se pudo cambiar el bus.";
        RefrescarVista();
    }

    private void RegistrarAccion(string accion)
    {
        _servicioHistorial.RegistrarAccion(accion);
    }

    private string ConstruirResumenReserva(Reserva reserva)
    {
        var pasajero = _servicioBaseDatos.Tabla<Pasajero>().FirstOrDefault(p => p.Id == reserva.PasajeroId);
        var ruta = _servicioBaseDatos.Tabla<Ruta>().FirstOrDefault(r => r.Id == reserva.RutaId);
        var bus = _servicioBaseDatos.Tabla<Bus>().FirstOrDefault(b => b.Id == reserva.BusId);

        return $"Pasajero {pasajero?.NombreMostrar ?? reserva.PasajeroId.ToString()}, " +
               $"Ruta {ruta?.NombreMostrar ?? reserva.RutaId.ToString()}, " +
               $"Bus {bus?.NombreMostrar ?? reserva.BusId.ToString()}, " +
               $"Asiento {reserva.NumeroAsiento}";
    }
}
