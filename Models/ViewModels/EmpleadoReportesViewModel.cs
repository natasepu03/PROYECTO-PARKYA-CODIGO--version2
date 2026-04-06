using ParkYa.Models;
using System;
using System.Collections.Generic;

namespace ParkYa.ViewModels
{
   
    public class EmpleadoReportesViewModel
    {
        public Usuario Empleado { get; set; } = new Usuario();

       
        public int TotalReservasHoy { get; set; }
        public int TotalReservasSemana { get; set; }
        public int TotalReservasMes { get; set; }

     
public List<ReservaItemViewModel> HistorialHoy { get; set; } = new();
public List<ReservaItemViewModel> HistorialSemana { get; set; } = new();
public List<ReservaItemViewModel> HistorialMes { get; set; } = new();
        public int TotalReservas { get; set; }
    }

   
    public class ReservaItemViewModel
{
    public int IdReserva { get; set; }
    public int CodigoReserva { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public TimeSpan? HoraEntrada { get; set; }
    public TimeSpan? HoraSalida { get; set; }
    public string Estado { get; set; } = string.Empty;
    public TimeSpan? HoraReservada { get; set; }
    public decimal? TarifaHora { get; set; }
    public decimal? Monto { get; set; }
}
}
