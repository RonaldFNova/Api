using System.ComponentModel.DataAnnotations;
namespace API.Model
{
    public class ModelEnviarCodigo 
    {
        [Required(ErrorMessage = "El token del usuario es obligatorio.")]
        public string token {get; set;} = string.Empty;

    }

}