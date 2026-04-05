using ParkYa.Models;

namespace ParkYa.Models.ViewModels
{
    public class DashboardAdminViewModel
    {
        public Usuario Administrador { get; set; } = new Usuario();
        public Parqueadero? Parqueadero { get; set; }
    }
}