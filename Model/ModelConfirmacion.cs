using System.ComponentModel.DataAnnotations;
namespace API.Model
{
    public class ModelConfirmacion
    {
        [Required(ErrorMessage = "El token del usuario es obligatorio.")]
        public string token {get; set;}

        [MinLength(6)]
        [MaxLength(10)]
        [Required(ErrorMessage = "El codigo de verificacion del usuario es obligatorio.")]
        public string codigo {get; set;}

    }

}