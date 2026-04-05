using System.ComponentModel.DataAnnotations;

namespace ParkYa.Models.ViewModels
{
    public class RecuperarPasswordViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
        public string Correo { get; set; } = string.Empty;

        public string PreguntaSeguridad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La respuesta es obligatoria")]
        public string RespuestaSeguridad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string NuevaPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes confirmar la contraseña")]
        [DataType(DataType.Password)]
        [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        public bool MostrarPaso2 { get; set; } = false;
    }
}