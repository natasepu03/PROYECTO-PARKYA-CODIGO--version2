using ParkYa.Models;

namespace ParkYa.ViewModels
{
    public class DashboardAdminViewModel
    {
        public Usuario Administrador { get; set; } = new Usuario();
        public Parqueadero Parqueadero { get; set; }
        public int ReservasHoy { get; set; }
        public decimal IngresosHoy { get; set; }
        public int PendientesHoy { get; set; }
        public int FinalizadasHoy { get; set; }
    }
}
