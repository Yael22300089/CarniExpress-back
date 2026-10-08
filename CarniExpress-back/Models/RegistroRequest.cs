using System.ComponentModel.DataAnnotations;

namespace CarniExpress_back.Models
{
    public class RegistroRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        public string Correo { get; set; } = "";

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^\d{10}$",
            ErrorMessage = "El teléfono debe tener 10 dígitos")]
        public string Telefono { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(8,
            ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string Contrasena { get; set; } = "";
    }
}