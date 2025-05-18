using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace API.Model
{
    public class ModelRegistro
    {
        public int Id { get; set; } = 0;

        [JsonPropertyName("name")]
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("pass")]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Pass { get; set; } = string.Empty;

        [JsonPropertyName("tipo")]
        [Required(ErrorMessage = "El tipo de usuario es obligatorio.")]
        public string Tipo { get; set; } = string.Empty;

    }
}