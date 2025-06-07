using System.Text.Json.Serialization;

namespace API.Model
{
    public class ModelListaMostrarCitaFecha
    {
        [JsonPropertyName("id")]    
        public int Id { get; set; } = 0;

        [JsonPropertyName("nombre")]    
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("fecha")]    
        public string Fecha { get; set; } = string.Empty;

        [JsonPropertyName("fechaInicio")]    
        public string FechaInicio { get; set; } = string.Empty;

        [JsonPropertyName("fechaFinal")]    
        public string FechaFinal { get; set; } = string.Empty;
    }    
}