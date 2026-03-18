namespace ParkYa.Models
{
    public class DetalleVenta
    {
        public int idfacturaDetalle { get; set; }
        public string cod_facDetalle { get; set; } = string.Empty;
        public TimeSpan hora_entrada { get; set; }
        public TimeSpan hora_salida { get; set; }

        public int Pago_id_Pago { get; set; }
        public Venta? Venta { get; set; }
    }
}