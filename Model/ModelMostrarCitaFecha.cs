using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Model
{

    public class ModelMostrarCitaFecha
    {
        [Required]
        [JsonPropertyName("especialidad")]
        public string Especialidad { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; } 

    }
}