using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelHorarioMedico
    {
        [JsonPropertyName("dia")]
        public string Dia { get; set; } = string.Empty;

        [JsonPropertyName("inicio")]
        public TimeSpan? Inicio { get; set; }

        [JsonPropertyName("final")]
        public TimeSpan? Final { get; set; }
    }
}