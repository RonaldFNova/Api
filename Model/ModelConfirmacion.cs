using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace API.Model
{
    public class ModelConfirmacion
    {

        [JsonPropertyName("codigo")]
        [Required(ErrorMessage = "El codigo de verificacion es obligatorio")]
        [Range(100000, 999999, ErrorMessage = "El código debe tener exactamente 6 dígitos.")]

        public int Codigo { get; set; } = 0;

    }
}