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

        private readonly dataTipoUser _dataTipoUser;

        public ControllerTipoUser(dataTipoUser dataTipoUser)
        {
            _dataTipoUser = dataTipoUser;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> POST([FromBody] ModelTipoUser parametros)
        {
            await _dataTipoUser.tipoUserAsyns(parametros);

            return Ok(new { mensaje = "Tipo de usuario enviado correctamente", parametros.Tipo });
        }
    }
}