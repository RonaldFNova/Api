using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelTipoUser
    {
        public int Id { get; set; } = 0;

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; } = string.Empty;
    }
}