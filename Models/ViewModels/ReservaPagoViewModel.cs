using System.ComponentModel.DataAnnotations;

namespace ParkYa.Models.ViewModels
{
    public class ReservaPagoViewModel
    {
        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public TimeSpan HoraReservada { get; set; }

        [Required]
        public int VehiculoId { get; set; }

        public int TipoVehiculoId { get; set; }

        public string MetodoPago { get; set; } = string.Empty;

        public decimal Monto { get; set; }
    }
}