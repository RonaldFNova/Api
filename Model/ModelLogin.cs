using System.ComponentModel.DataAnnotations;
namespace API.Model
{
    public class ModelLogin 
    {
        public int id { get; set; }
        
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string email {get; set;}

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string pass {get; set;}
        public string tipo { get; set; } = string.Empty;
    }

}