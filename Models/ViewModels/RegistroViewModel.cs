using System.ComponentModel.DataAnnotations;

namespace ParkYa.Models.ViewModels
{
    public class RegistroViewModel
    {
        [Required]
        public string nombre { get; set; } = string.Empty;

        [Required]
        public string apellido { get; set; } = string.Empty;

        [Required]
        public string tipo_doc { get; set; } = string.Empty;

        [Required]
        public int documento { get; set; }

        [Required]
        [EmailAddress]
        public string correo { get; set; } = string.Empty;

        [Required]
        public string telefono { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string contraseña { get; set; } = string.Empty;

        [Required]
        [Compare("contraseña")]
        public string confirmarContraseña { get; set; } = string.Empty;
    }
}