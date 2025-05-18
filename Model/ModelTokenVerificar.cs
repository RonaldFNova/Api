using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace API.Model
{
    public class ModelTokenVerificar
    {
        [JsonPropertyName("token")]
        [Required(ErrorMessage = "El token del usuario es obligatorio.")]
        public string Token {get; set;} = string.Empty;

    }
}