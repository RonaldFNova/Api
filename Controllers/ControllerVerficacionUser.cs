using System.Security.Claims;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [ApiController]
    [Route("/Api/Estado-user-verificacion")]
    public class ControllerVerificacionUser : ControllerBase
    {
        private readonly DataVerificacionUser _dataVerificacionUser;

        public ControllerVerificacionUser(DataVerificacionUser dataVerificacionUser)
        {
            _dataVerificacionUser = dataVerificacionUser;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Medico,Paciente")]
        public async Task<IActionResult> GET()
        {
            var userIdClaim = User.FindFirst("id");
            int id = int.Parse(userIdClaim.Value);

            string EstadoUser = await _dataVerificacionUser.ObtenerVerificacionUserAsync(id);

            return Ok(new { mensaje = "El estado del usuario se ha encontrado correctamente", EstadoUser });
        }

    }
}