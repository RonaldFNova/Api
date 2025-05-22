
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelMostrarDoctores
    {
        [JsonPropertyName("name")]   
        public string Name { get; set; } = string.Empty;
    }

}