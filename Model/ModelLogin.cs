using System.ComponentModel.DataAnnotations;
namespace API.Model
{
    public class ModelLogin 
    {
        public int id { get; set; } = 0;
        
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string email {get; set;} = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string pass {get; set;} = string.Empty;
        public string tipo { get; set; } = string.Empty;
    }

}