using Microsoft.EntityFrameworkCore;
using ParkYa.Models;

namespace ParkYa.Data
{
    public class ParkYaDbContext : DbContext
    {
        public ParkYaDbContext(DbContextOptions<ParkYaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> usuario { get; set; }
        public DbSet<Rol> rol { get; set; }
        public DbSet<Vehiculo> vehiculo { get; set; }
        public DbSet<TipoVehiculo> tipo_vehiculo { get; set; }
        public DbSet<Parqueadero> parqueadero { get; set; }
        public DbSet<Reserva> reserva { get; set; }
        public DbSet<Tarifa> tarifas { get; set; }
        public DbSet<Venta> venta { get; set; }
        public DbSet<DetalleVenta> detalleventa { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.id_usuario);
                entity.ToTable("usuario");

                entity.HasOne(e => e.Rol)
                    .WithMany(r => r.Usuarios)
                    .HasForeignKey(e => e.Rol_id_rol);
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasKey(e => e.id_rol);
                entity.ToTable("rol");
            });

            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.HasKey(e => e.id_vehiculo);
                entity.ToTable("vehiculo");
            });

            modelBuilder.Entity<TipoVehiculo>(entity =>
            {
                entity.HasKey(e => e.TipoVehiculoId);
                entity.ToTable("tipo_vehiculo");
            });

            modelBuilder.Entity<Parqueadero>(entity =>
            {
                entity.HasKey(e => e.id_Parqueadero);
                entity.ToTable("parqueadero");
            });

            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasKey(e => e.id_reserva);
                entity.ToTable("reserva");
            });

            modelBuilder.Entity<Tarifa>(entity =>
            {
                entity.HasKey(e => e.id_tarifas);
                entity.ToTable("tarifas");
            });

            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(e => e.id_pago);
                entity.ToTable("venta");
            });

            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.HasKey(e => e.idfacturaDetalle);
                entity.ToTable("detalleventa");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}