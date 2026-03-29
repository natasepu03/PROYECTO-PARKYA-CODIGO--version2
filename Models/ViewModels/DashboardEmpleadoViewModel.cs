using ParkYa.Models;

namespace ParkYa.ViewModels
{
    public class DashboardEmpleadoViewModel
    {
        public Usuario Empleado { get; set; } = new Usuario();

        public int TotalVehiculos { get; set; }
        public int TotalReservas { get; set; }
        public int ReservasActivas { get; set; }

        public List<VehiculoActivoItemViewModel> VehiculosActivos { get; set; } = new();
    }

    public class VehiculoActivoItemViewModel
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