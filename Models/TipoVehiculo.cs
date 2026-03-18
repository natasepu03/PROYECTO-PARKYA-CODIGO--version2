namespace ParkYa.Models
{
    public class TipoVehiculo
    {
        public int TipoVehiculoId { get; set; }
        public string cod_tipoVehiculo { get; set; } = string.Empty;
        public string nombre_tipo { get; set; } = string.Empty;

        public List<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
        public List<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
    }
}