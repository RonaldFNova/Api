using System.ComponentModel.DataAnnotations;
namespace API.Model
{
    public class ModelRegistro 
    {
        [Required(ErrorMessage = "El id del usuario es obligatorio.")]
        public int Id { get; set; } = 0;

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        public string Name {get; set;} = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email {get; set;} = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Pass {get; set;} = string.Empty;

        [Required(ErrorMessage = "El tipo de usuario es obligatorio.")]
        public string Tipo {get; set;} = string.Empty;


    }

}