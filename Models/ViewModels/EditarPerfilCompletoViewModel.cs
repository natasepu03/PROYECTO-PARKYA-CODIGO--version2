using System.ComponentModel.DataAnnotations;

namespace ParkYa.Models.ViewModels
{
    public class EditarPerfilCompletoViewModel
    {
        public int IdUsuario { get; set; }
        public int? IdVehiculo {get; set;}

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Telefono { get; set; } = string.Empty;
        [Required]
        public string Placa { get; set; } = string.Empty;

        [Required]
        public string Marca { get; set; } = string.Empty;

        [Required]
        public string Color { get; set; } = string.Empty;
    
    }
}