using ParkYa.Models;

namespace ParkYa.ViewModels
{
    public class EmpleadoVehiculosViewModel
    {
        public Usuario Empleado { get; set; } = new Usuario();

        public int TotalDentro { get; set; }
        public int TotalSalidosHoy { get; set; }

        public List<MovimientoVehiculoItemViewModel> VehiculosDentro { get; set; } = new();
        public List<MovimientoVehiculoItemViewModel> HistorialSalidasHoy { get; set; } = new();
    }

    public class MovimientoVehiculoItemViewModel
    {
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Propietario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}