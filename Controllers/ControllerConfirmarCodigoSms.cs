using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Error;

namespace API.Controllers
{
    [ApiController]
    [Route("Api/Verificacion-sms")]

    public class ControllerConfirmarCodigoSms : ControllerBase
    {
        private readonly DataConfirmarCodigoSms _dataConfirmarCodigoSms;

        public ControllerConfirmarCodigoSms(DataConfirmarCodigoSms dataConfirmarCodigoSms)
        {
            _dataConfirmarCodigoSms = dataConfirmarCodigoSms;
        }

        [HttpPost("Recibir-codigo")]
        [Authorize(Roles = "Paciente,Enfermero,Medico")]

        public async Task<ActionResult> Post([FromBody] ModelConfirmacion parametros)
        {
            var userIdString = HttpContext.Items["CodigoToken_UserId"] as string;
            var codigoString = HttpContext.Items["CodigoToken_Codigo"] as string;
            
            int userId = int.Parse(userIdString);

            int codigo = int.Parse(codigoString);

            Console.WriteLine(codigo);

            if (codigo != parametros.Codigo)
            {
                throw new CodigoIncorrectoException();
            }


            await _dataConfirmarCodigoSms.EnviarEstadoVerificacionAsync(userId);

            return Ok(new { mensaje = "Código confirmado correctamente" });
        }
    }
}