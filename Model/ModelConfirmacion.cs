using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace API.Model
{
    public class ModelConfirmacion
    {
        [JsonPropertyName("token")]
        [Required(ErrorMessage = "El token del usuario es obligatorio.")]
        public string token { get; set; } = string.Empty;

        [JsonPropertyName("codigo")]
        [MinLength(6)]
        [MaxLength(10)]
        [Required(ErrorMessage = "El codigo de verificacion del usuario es obligatorio.")]
        public int codigo { get; set; }
    }
}