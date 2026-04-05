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

                entity.Property(e => e.PreguntaSeguridad) 
                    .HasColumnName("PreguntaSeguridad"); 

                entity.Property(e => e.RespuestaSeguridad) 
                    .HasColumnName("RespuestaSeguridad");     
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

                entity.Property(e => e.id_vehiculo).HasColumnName("id_vehiculo");
                entity.Property(e => e.placa).HasColumnName("placa");
                entity.Property(e => e.marca).HasColumnName("marca");
                entity.Property(e => e.color).HasColumnName("color");
                entity.Property(e => e.Usuario_id_usuario).HasColumnName("Usuario_id_usuario");
                entity.Property(e => e.Parqueadero_id_Parqueadero).HasColumnName("Parqueadero_id_Parqueadero");
                entity.Property(e => e.Tipo_vehiculo_idTipo_vehiculo).HasColumnName("Tipo_vehiculo_idTipo_vehiculo");

                entity.Ignore("TipoVehiculoId");
                entity.Ignore("TipoVehiculoId1");
                entity.Ignore("TipoVehiculo");
                entity.Ignore("Usuarioid_usuario");
                entity.Ignore("color1");


                entity.HasOne(v => v.Usuario)
                    .WithMany(u => u.Vehiculos)
                    .HasForeignKey(v => v.Usuario_id_usuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.Parqueadero)
                    .WithMany()
                    .HasForeignKey(v => v.Parqueadero_id_Parqueadero)
                    .HasPrincipalKey(p => p.id_Parqueadero)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(v => v.TipoVehiculo)
                    .WithMany()
                    .HasForeignKey(v => v.Tipo_vehiculo_idTipo_vehiculo)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<TipoVehiculo>(entity =>
            {
                entity.HasKey(e => e.TipoVehiculoId);
                entity.ToTable("tipo_vehiculo");

                entity.Property(e => e.TipoVehiculoId)
                    .HasColumnName("idTipo_vehiculo");

                entity.Property(e => e.cod_tipoVehiculo)
                    .HasColumnName("Cod_TipoVehiculo");

                entity.Property(e => e.nombre_tipo)
                    .HasColumnName("nombre_tipo");
            });

            modelBuilder.Entity<Parqueadero>(entity =>
            {
                entity.HasKey(e => e.id_Parqueadero);
                entity.ToTable("parqueadero");

                entity.Property(e => e.id_Parqueadero).HasColumnName("id_Parqueadero");
                entity.Property(e => e.cod_parqueadero).HasColumnName("cod_parqueadero");
                entity.Property(e => e.direccion).HasColumnName("direccion");
                entity.Property(e => e.nombre).HasColumnName("nombre");
                entity.Property(e => e.total_cupos).HasColumnName("total_cupos");
            });

            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasKey(e => e.id_reserva);
                entity.ToTable("reserva");

                entity.Property(e => e.id_reserva)
                    .HasColumnName("id_reserva");

                entity.Property(e => e.cod_reserva)
                    .HasColumnName("cod_reserva");

                entity.Property(e => e.fecha)
                    .HasColumnName("fecha");

                entity.Property(e=> e.hora_reservada) 
                    .HasColumnName("hora_reservada"); 

                entity.Property(e => e.hora_entrada)
                    .HasColumnName("hora_entrada");

                entity.Property(e => e.hora_salida)
                    .HasColumnName("hora_salida");

                entity.Property(e => e.Estado)
                    .HasColumnName("estado")
                    .HasConversion<string>();

                entity.Property(e => e.Usuario_id_usuario)
                    .HasColumnName("Usuario_id_usuario");

                entity.Property(e => e.TarifaId)
                    .HasColumnName("Tarifas_id_tarifas");

                entity.Property(e => e.Vehiculo_id_vehiculo)
                    .HasColumnName("Vehiculo_id_vehiculo");

                entity.Ignore("Tarifaid_tarifas");
                entity.Ignore("Tarifaid_tarifas1");

                entity.HasOne(r => r.Usuario)
                    .WithMany(u => u.Reservas)
                    .HasForeignKey(r => r.Usuario_id_usuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Tarifa>()
                    .WithMany(t => t.Reservas)
                    .HasForeignKey(r => r.TarifaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Vehiculo)
                    .WithMany()
                    .HasForeignKey(r => r.Vehiculo_id_vehiculo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Tarifa>(entity =>
            {
                entity.HasKey(e => e.id_tarifas);
                entity.ToTable("tarifas");

                entity.Property(e => e.id_tarifas)
                    .HasColumnName("id_tarifas");

                entity.Property(e => e.cod_tarifa)
                    .HasColumnName("cod_tarifa");

                entity.Property(e => e.precio_dia)
                    .HasColumnName("precio_Dia");

                entity.Property(e => e.precio_hora)
                    .HasColumnName("precio_hora");

                entity.Property(e => e.horario)
                    .HasColumnName("horario");

                entity.Property(e => e.Tipo_vehiculo_idTipo_vehiculo)
                    .HasColumnName("Tipo_vehiculo_idTipo_vehiculo");

                entity.Ignore("TipoVehiculoId");
                entity.Ignore("TipoVehiculoId1");
                entity.Ignore("TipoVehiculoId2");
                entity.Ignore("TipoVehiculo");
            });

            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(e => e.id_pago);
                entity.ToTable("venta");

                entity.Property(e => e.id_pago)
                    .HasColumnName("id_pago");

                entity.Property(e => e.cod_pago)
                    .HasColumnName("cod_pago");

                entity.Property(e => e.monto)
                    .HasColumnName("monto");

                entity.Property(e => e.fecha_pago)
                    .HasColumnName("fecha_pago");

                entity.Property(e => e.metodo_pago)
                    .HasColumnName("metodo_pago");

                entity.Property(e => e.Usuario_id_usuario)
                    .HasColumnName("Usuario_id_usuario");

                entity.Property(e => e.Reserva_id_reserva)
                    .HasColumnName("Reserva_id_reserva");

                entity.Ignore("Usuario");
                entity.Ignore("Reserva");
                entity.Ignore("Usuarioid_usuario");
                entity.Ignore("Reservaid_reserva");
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