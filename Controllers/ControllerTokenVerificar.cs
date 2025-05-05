using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace API.Controllers
{
    [ApiController]
    [Route("Api/Confirmar-jwt")]
    public class ControllerTokenVerificar : ControllerBase
    {

        private readonly DataTokenVerificar _datatokenverificar;


        public ControllerTokenVerificar(DataTokenVerificar datatokenverificar)
        {
            _datatokenverificar = datatokenverificar;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Post([FromBody] ModelTokenVerificar parametros)
        {
            await _datatokenverificar.GetTokenVerificar(parametros);

            return Ok(new {mensaje = "El token esta correcto"});
        }
    }
}