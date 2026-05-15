using Microsoft.EntityFrameworkCore;
using introduccion.Models;

namespace introduccion.Data;

public class ContextoDatos : DbContext
{
    public DbSet<Pasajero> Pasajeros { get; set; }
    public DbSet<Ruta> Rutas { get; set; }
    public DbSet<Bus> Buses { get; set; }
    public DbSet<Conductor> Conductores { get; set; }
    public DbSet<Reserva> Reservas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=transporte.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pasajero>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombre).IsRequired();
            entity.Property(x => x.Documento).IsRequired();
        });

        modelBuilder.Entity<Ruta>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CiudadOrigen).IsRequired();
            entity.Property(x => x.CiudadDestino).IsRequired();
            entity.Property(x => x.EsBidireccional).IsRequired();
            entity.Property(x => x.EstaHabilitada).IsRequired();
        });

        modelBuilder.Entity<Bus>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Placa).IsRequired();
            entity.Property(x => x.Capacidad).IsRequired();
            entity.Property(x => x.EstaDisponible).IsRequired();
        });

        modelBuilder.Entity<Conductor>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nombre).IsRequired();
            entity.Property(x => x.Licencia).IsRequired();
            entity.Property(x => x.HorasTrabajadas).IsRequired();
            entity.Property(x => x.MaxHorasPorDia).IsRequired();
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(x => new { x.PasajeroId, x.RutaId, x.BusId, x.NumeroAsiento });
            entity.Property(x => x.Estado).IsRequired();

            entity.HasOne<Pasajero>()
                .WithMany()
                .HasForeignKey(x => x.PasajeroId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Ruta>()
                .WithMany()
                .HasForeignKey(x => x.RutaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Bus>()
                .WithMany()
                .HasForeignKey(x => x.BusId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}