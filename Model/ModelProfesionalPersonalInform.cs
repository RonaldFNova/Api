using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelProfesionalPersonalInform
    {

        public int Id { get; set; } = 0;
        
        [JsonPropertyName("token")]
        [Required(ErrorMessage = "El campo token es obligatorio")]
        public string Token { get; set; } = string.Empty;
        [JsonPropertyName("cell")]
        [Required(ErrorMessage = "El campo cell es obligatorio")]
        public string Cell { get; set; } = string.Empty;
        [JsonPropertyName("tipoId")]
        [Required(ErrorMessage = "El campo tipoId es obligatorio")]
        public string TipoId { get; set; } = string.Empty;

        [JsonPropertyName("personalId")]
        [Required(ErrorMessage = "El campo personalId es obligatorio")]
        public string PersonalId { get; set; } = string.Empty;
        [JsonPropertyName("especialidad")]
        [Required(ErrorMessage = "El campo especialidad es obligatorio")]
        public List<string>? Especialidad  { get; set; }
    }
}