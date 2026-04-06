using ParkYa.Models;

namespace ParkYa.ViewModels
{
    public class AdministradorReportesViewModel
    {
        public Usuario Administrador { get; set; } = new Usuario();
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int TotalReservasDia { get; set; }
        public int TotalReservasSemana { get; set; }
        public int TotalReservasMes { get; set; }
        public int ReservasPendientes { get; set; }
        public int ReservasFinalizadas { get; set; }
        public int ReservasCanceladas { get; set; }
        public decimal IngresosReales { get; set; }
        public decimal MontoPromedio { get; set; }
        public string HoraPico { get; set; } = string.Empty;
        public int TotalReservasHoraPico { get; set; }
        public List<ReporteReservaAdminItemViewModel> ReservasFiltradas { get; set; } = new();
    }

    public class ReporteReservaAdminItemViewModel
    {
        public int IdReserva { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public string TipoVehiculo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }
}
