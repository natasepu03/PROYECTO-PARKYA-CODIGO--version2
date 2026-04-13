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
        public string documento { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string correo { get; set; } = string.Empty;

        [Required]
        public string telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage = "La contraseña debe tener mayúscula, minúscula, número y un carácter especial")]
        public string contraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes confirmar la contraseña")]
        [Compare("contraseña", ErrorMessage = "Las contraseñas no coinciden")]
        public string confirmarContraseña { get; set; } = string.Empty;

        [Required(ErrorMessage = "La pregunta de seguridad es obligatoria")]
        public string PreguntaSeguridad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La respuesta de seguridad es obligatoria")]
        public string RespuestaSeguridad { get; set; } = string.Empty;

    }
}
