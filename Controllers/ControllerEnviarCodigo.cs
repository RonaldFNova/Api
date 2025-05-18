using API.Data;
using API.Model;
using API.Security; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Registro/Enviar-codigo")]
    public class ControllerEnviarCodigo : ControllerBase
    {
        private readonly DataRegistro _dataRegistro;
        private readonly JwtService _jwtService;
        public ControllerEnviarCodigo(DataRegistro dataRegistro, JwtService jwtService)
        {
            _jwtService = jwtService;
            _dataRegistro = dataRegistro;
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task<ActionResult> POST([FromBody] ModelEnviarCodigo parametros)
        {
            (string id, string codigo) = await _dataRegistro.EnviarCodigo(parametros);

            var tokenCodigo = _jwtService.GenerateVerificationToken(id, codigo);

            return Ok(new { mensaje = "Código enviado correctamente", tokenCodigo });
        }
    }
}