using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelMostrarCitasPaciente
    {

        [Required]
        [JsonPropertyName("fecha")]
        public string Fecha { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("horaInicio")]
        public string HoraInicio { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("horaFinal")]
        public string HoraFinal { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("motivoConsulta")]
        public string MotivoConsulta { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("duracion")]
        public string Duracion { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("medicoTratante")]
        public string MedicoTratante { get; set; } = string.Empty;


        [Required]
        [JsonPropertyName("estadoCita")]
        public string EstadoCita { get; set; } = string.Empty;
    }
}