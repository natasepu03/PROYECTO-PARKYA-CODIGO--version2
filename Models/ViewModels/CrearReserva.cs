namespace ParkYa.Models.ViewModels
{
    public class ReservaViewModel
    {
        public DateTime Fecha { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public int VehiculoId { get; set; }
    }
}