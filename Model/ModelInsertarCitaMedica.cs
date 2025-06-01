using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelInsertarCitaMedica
    {
        [Required(ErrorMessage = "El rango inicial de la cita es obligatorio")]
        [JsonPropertyName("fechainicial")]
        public TimeSpan FechaInicio { get; set; }

        [Required(ErrorMessage = "El rango final de la cita es obligatorio")]
        [JsonPropertyName("fechafinal")]
        public TimeSpan FechaFinal { get; set; }

        [Required(ErrorMessage = "La fecha de la cita es obligatorio")]
        [JsonPropertyName("diafecha")]
        public DateTime DiaFecha { get; set; }

        [Required(ErrorMessage = "El nombre del medico es obligatorio")]
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El motivo de consulta es obligatorio")]
        [JsonPropertyName("motivoConsulta")]                
        public string MotivoConsulta { get; set; } = string.Empty;

    }
}