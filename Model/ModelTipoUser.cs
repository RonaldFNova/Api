using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelTipoUser
    {
        [JsonPropertyName("token")]
        [Required(ErrorMessage = "El token del usuario es obligatorio.")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; } = string.Empty;
    }
}