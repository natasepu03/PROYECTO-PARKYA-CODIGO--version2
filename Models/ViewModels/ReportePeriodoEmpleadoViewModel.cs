using System;
using System.Collections.Generic;

namespace ParkYa.ViewModels
{
    public class ReportePeriodoEmpleadoViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; }
        public int TotalReservas { get; set; }
        public decimal TotalMonto { get; set; }
        public List<ReservaItemViewModel> Reservas { get; set; } = new();
    }
}
