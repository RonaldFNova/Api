using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace API.Model
{
    public class ModelLogin
    {

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("pass")]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Pass { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;

        public DateTime Tiempo { get; set; }
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;

    }

}