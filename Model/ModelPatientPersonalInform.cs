using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelPatientPersonalInform
    {
        public int Id { get; set; } = 0;
        

        [JsonPropertyName("cell")]
        [Required(ErrorMessage = "El telefono del usuario es obligatorio.")]
        public string Cell { get; set; } = string.Empty;

        [JsonPropertyName("tipoId")]
        [Required(ErrorMessage = "El tipo de id del usuario es obligatorio.")]
        public string TipoId { get; set; } = string.Empty;

        [JsonPropertyName("personalId")]
        [Required(ErrorMessage = "El id personal del usuario es obligatorio.")]
        public string PersonalId { get; set; } = string.Empty;

        [JsonPropertyName("bloodGroup")]
        [Required(ErrorMessage = "El tipo de sangre del usuario es obligatorio.")]
        public string BloodGroup { get; set; } = string.Empty;

        [JsonPropertyName("alergiasGeneral")]
        public List<string>? AlergiasGeneral { get; set; }
        
        [JsonPropertyName("alergiasMedications")]
        public List<string>? AlergiasMedications { get; set; }
        
    }
}