using System.ComponentModel.DataAnnotations;
namespace API.Model
{
    public class ModelEnviarCodigo 
    {
        [Required(ErrorMessage = "El token del usuario es obligatorio.")]
        public string Token {get; set;} = string.Empty;

    }

}