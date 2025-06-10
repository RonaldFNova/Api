using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Confirmar-jwt-id")]
    public class ControllerTokenVerificarId : ControllerBase
    {


        [HttpGet]
        [Authorize(Roles ="Administrador,Medico,Paciente")]

        public Task<ActionResult> GET()
        {
            return Task.FromResult<ActionResult> (Ok(new { mensaje = "El token esta correcto" }));
        }
    }
}