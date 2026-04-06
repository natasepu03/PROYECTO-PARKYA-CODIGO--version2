using ParkYa.Models;

namespace ParkYa.ViewModels
{
    public class AdministradorTarifasViewModel
    {
        public Usuario Administrador { get; set; } = new Usuario();
        public List<TarifaAdminItemViewModel> Tarifas { get; set; } = new();
    }

    public class TarifaAdminItemViewModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string TipoVehiculo { get; set; } = string.Empty;
        public decimal PrecioDia { get; set; }
        public decimal PrecioHora { get; set; }
        public string Horario { get; set; } = string.Empty;
        public int TipoVehiculoId { get; set; }
    }
}
