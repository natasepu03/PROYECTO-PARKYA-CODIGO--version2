namespace ParkYa.Models
{
    public class Tarifa
    {
        public int id_tarifas { get; set; }
        public string cod_tarifa { get; set; } = string.Empty;
        public decimal precio_dia { get; set; }
        public decimal precio_hora { get; set; }
        public string horario { get; set; } = string.Empty;

        public int Tipo_vehiculo_idTipo_vehiculo { get; set; }
        public TipoVehiculo? TipoVehiculo { get; set; }

        public List<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}