namespace ParkYa.Models
{
    public class Reserva
    {
        public int id_reserva { get; set; }
        public int cod_reserva { get; set; }

        public DateTime fecha { get; set; }

        public TimeSpan? hora_reservada { get; set; }   // hora que pone el cliente
        public TimeSpan? hora_entrada { get; set; }    // la registra el empleado
        public TimeSpan? hora_salida { get; set; }     // la registra el empleado

        public Estado Estado { get; set; }

        public int Usuario_id_usuario { get; set; }
        public int TarifaId { get; set; }
        public int Vehiculo_id_vehiculo { get; set; }

        public Usuario? Usuario { get; set; }
        public Vehiculo? Vehiculo { get; set; }
    }

    public enum Estado
    {
        Pendiente,
        Activa,
        Finalizada,
        Cancelada
    }
}