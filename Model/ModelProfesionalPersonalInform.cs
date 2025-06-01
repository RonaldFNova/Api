using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelProfesionalPersonalInform
    {

        public int Id { get; set; } = 0;

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
        public List<string>? Especialidad { get; set; }
        
        [JsonPropertyName("direccion")]
        [Required(ErrorMessage = "La direccion del paciente es obligatoria")]
        public string Direccion { get; set; } = string.Empty;
    }
}