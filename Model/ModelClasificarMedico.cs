
using System.Text.Json.Serialization;

namespace API.Data
{
    public class ModelClasificarMedico
    {
        [JsonPropertyName("tipo")]
        public string Tipo { get; set; } = string.Empty;
    }
}