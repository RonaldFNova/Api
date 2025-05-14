using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace API.Model
{
    public class ModelConfirmacion
    {
        [JsonPropertyName("tokenCodigo")]
        [Required(ErrorMessage = "El token del usuario es obligatorio.")]
        public string tokenCodigo { get; set; } = string.Empty;

        [JsonPropertyName("codigo")]
        [Range(100000, 999999, ErrorMessage = "El código debe tener exactamente 6 dígitos.")]

        public int codigo { get; set; }
    }
}