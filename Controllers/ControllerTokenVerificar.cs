using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace API.Controllers
{
    [ApiController]
    [Route("Api/Confirmar-jwt-id")]
    public class ControllerTokenVerificarId : ControllerBase
    {
        private readonly DataTokenVerificar _datatokenverificar;

        public ControllerTokenVerificarId(DataTokenVerificar datatokenverificar)
        {
            _datatokenverificar = datatokenverificar;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Post([FromBody] ModelTokenVerificar parametros)
        {
            await _datatokenverificar.GetTokenVerificarId(parametros);

            return Ok(new { mensaje = "El token esta correcto" });
        }
    }
    
    [ApiController]
    [Route("Api/Confirmar-jwt-codigo")]
    public class ControllerTokenVerificarCodigo : ControllerBase
    {

        private readonly DataTokenVerificar _datatokenverificar;

        public ControllerTokenVerificarCodigo(DataTokenVerificar datatokenverificar)
        {
            _datatokenverificar = datatokenverificar;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Post([FromBody] ModelTokenVerificar parametros)
        {
            await _datatokenverificar.GetTokenVerificarCodigo(parametros);

            return Ok(new {mensaje = "El token esta correcto"});
        }
    }
}