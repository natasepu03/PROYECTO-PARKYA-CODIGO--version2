namespace ParkYa.Models
{
    public class Parqueadero
    {
        public int id_Parqueadero { get; set; }

        public int cod_parqueadero { get; set; }

        public string direccion { get; set; }  = string.Empty;

        public required string nombre { get; set; } = string.Empty;

        public int total_cupos { get; set; }

        public required string tipo_espacio { get; set; }  = string.Empty;
    }
}