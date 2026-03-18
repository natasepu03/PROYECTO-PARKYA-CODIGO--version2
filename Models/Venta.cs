namespace ParkYa.Models
{
    public class Venta
    {
        public int id_pago { get; set; }
        public string cod_pago { get; set; } = string.Empty;
        public decimal monto { get; set; }
        public DateTime fecha_pago { get; set; }
        public string metodo_pago { get; set; } = string.Empty;

        public int Usuario_id_usuario { get; set; }
        public Usuario? Usuario { get; set; }

        public int Reserva_id_reserva { get; set; }
        public Reserva? Reserva { get; set; }

        public List <DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
    }
}