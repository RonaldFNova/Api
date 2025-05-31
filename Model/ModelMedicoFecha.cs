using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelMedicoFecha
    {   
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}