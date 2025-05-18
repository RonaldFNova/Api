using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Registro/Confirmar-codigo")]

    public class ControllerConfirmarCodigo : ControllerBase
    {
        private readonly DataRegistro _dataRegistro;

        public ControllerConfirmarCodigo (DataRegistro dataRegistro)
        {
            _dataRegistro = dataRegistro;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> Post([FromBody] ModelConfirmacion parametros)
        {

            await _dataRegistro.ConfirmarVerificacion(parametros);

            return Ok(new { mensaje = "Código confirmado correctamente" });
        }
    }
}