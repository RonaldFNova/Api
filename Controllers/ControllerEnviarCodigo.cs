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

        public async Task<ActionResult> POST()
        {
            var userIdClaim = User.FindFirst("id");
            int userId = int.Parse(userIdClaim.Value);


            string codigo = await _dataRegistro.EnviarCodigo(userId);

            var tokenCodigo = _jwtService.GenerateVerificationToken(userIdClaim.Value, codigo);

            return Ok(new { mensaje = "Código enviado correctamente", tokenCodigo });
        }
    }
}