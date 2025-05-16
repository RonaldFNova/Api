using System.ComponentModel.DataAnnotations;
namespace API.Model
{
    public class ModelLogin 
    {
        public int Id { get; set; } = 0;
        
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email {get; set;} = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Pass {get; set;} = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

}