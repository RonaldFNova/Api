using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelInsertarCitaMedicaId
    {
        [JsonPropertyName("id")]
        [Required]
        public int Id { get; set; } = 0;

        [JsonPropertyName("motivoConsulta")]
        [Required]
        public string MotivoConsulta { get; set; } = string.Empty;
    }
}