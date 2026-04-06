using ParkYa.Models;

namespace ParkYa.ViewModels
{
    public class AdministradorReservasViewModel
    {
        public Usuario Administrador { get; set; } = new Usuario();
        public List<ReservaAdminItemViewModel> Reservas { get; set; } = new();
        public int TotalReservas { get; set; }
        public int ReservasPendientes { get; set; }
        public int ReservasFinalizadas { get; set; }
        public int ReservasCanceladas { get; set; }
    }

    public class ReservaAdminItemViewModel
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public string TipoVehiculo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }
}
