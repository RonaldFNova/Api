using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelInformacionPersonal
    {
        
        [JsonPropertyName("token")]
        [Required(ErrorMessage = "El token del usuario es obligatorio.")]
        public string Token {get; set;} = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("cell")]
        [Required(ErrorMessage = "El telefono del usuario es obligatorio.")]
        public string Cell { get; set; } = string.Empty;

        [JsonPropertyName("id")]
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