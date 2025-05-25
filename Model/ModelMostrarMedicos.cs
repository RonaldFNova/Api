
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelMostrarMedicos
    {
        [JsonPropertyName("name")]   
        public string Name { get; set; } = string.Empty;
    }

}