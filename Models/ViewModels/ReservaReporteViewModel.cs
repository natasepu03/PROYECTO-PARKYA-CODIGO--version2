namespace ParkYa.ViewModels
{
    public class ReservaReporteViewModel
    {
        public int IdReserva { get; set; }
        public int CodigoReserva { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan? HoraReservada { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public decimal? TarifaHora { get; set; }
        public decimal? Monto { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}