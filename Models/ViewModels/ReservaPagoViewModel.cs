using System.ComponentModel.DataAnnotations;

namespace ParkYa.Models.ViewModels
{
    public class ReservaPagoViewModel
    {
        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public TimeSpan HoraEntrada { get; set; }

        [Required]
        public TimeSpan HoraSalida { get; set; }

        [Required]
        public int VehiculoId { get; set; }

        [Required]
        public string MetodoPago { get; set; } = string.Empty;

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public int TipoVehiculoId { get; set; }
    }
}