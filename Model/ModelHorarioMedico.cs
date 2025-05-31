using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelHorarioMedico
    {
        [JsonPropertyName("dia")]
        public string Dia { get; set; } = string.Empty;

        [JsonPropertyName("horaInicio")]
        public TimeSpan? Inicio { get; set; }

        [JsonPropertyName("horaFin")]
        public TimeSpan? Final { get; set; }
    }
}