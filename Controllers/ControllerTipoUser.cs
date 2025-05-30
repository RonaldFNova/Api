using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Model;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Tipo-user")]
    public class ControllerTipoUser : ControllerBase
    {

        private readonly DataTipoUser _dataTipoUser;

        public ControllerTipoUser(DataTipoUser dataTipoUser)
        {
            _dataTipoUser = dataTipoUser;
        }

        [HttpPost]
        [Authorize(Roles ="Administrador,Medico,Paciente")]

        public async Task<ActionResult> POST([FromForm] ModelTipoUser parametros)
        {
            var userIdClaim = User.FindFirst("id");

            parametros.Id = int.Parse(userIdClaim.Value);

            await _dataTipoUser.tipoUserAsyns(parametros);

            return Ok(new { mensaje = "Tipo de usuario enviado correctamente", parametros.Tipo });
        }
    }
}