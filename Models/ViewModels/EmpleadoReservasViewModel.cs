using ParkYa.Models;

namespace ParkYa.ViewModels
{
    public class EmpleadoReservasViewModel
    {
        public Usuario Empleado { get; set; } = new Usuario();

        public int TotalActivasOPendientes { get; set; }
        public int TotalHoy { get; set; }
        public int TotalSemana { get; set; }
        public int TotalMes { get; set; }

        public List<ReservaEmpleadoItemViewModel> ReservasActuales { get; set; } = new();
        public List<ReservaEmpleadoItemViewModel> HistorialHoy { get; set; } = new();
        public List<ReservaEmpleadoItemViewModel> HistorialSemana { get; set; } = new();
        public List<ReservaEmpleadoItemViewModel> HistorialMes { get; set; } = new();
    }

    public class ReservaEmpleadoItemViewModel
    {
        public int IdReserva { get; set; }
        public int CodigoReserva { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}