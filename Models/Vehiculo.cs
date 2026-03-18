namespace ParkYa.Models
{
    public class Vehiculo
    {
        public int id_vehiculo { get; set; }

        public required string placa { get; set; }

        public required string marca { get; set; }

        public required string color { get; set; }

        public int Usuario_id_usuario { get; set; }

        public int Parqueadero_id_Parqueadero { get; set; }

        public int Tipo_vehiculo_idTipo_vehiculo { get; set; }

        public  Usuario? Usuario { get; set; }


    }
}