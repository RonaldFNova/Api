using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Confirmar-jwt-codigo")]
    public class ControllerTokenVerificarCodigo : ControllerBase
    {

        [HttpPost]
        [Authorize(Roles ="Administrador,Medico,Paciente")]

        public Task<ActionResult> POST()
        {

            return  Task.FromResult<ActionResult>(Ok(new { mensaje = "El tokenCodigo esta correcto" }));
        }
    }
}