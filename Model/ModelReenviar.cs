using System.ComponentModel.DataAnnotations;
namespace API.Model
{
    public class ModelReenviar 
    {
        [Required(ErrorMessage = "El id del usuario es obligatorio.")]
        public int id {get; set;}
        
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string email {get; set;}

    }

}