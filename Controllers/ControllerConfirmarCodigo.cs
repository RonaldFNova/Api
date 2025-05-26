using API.Data;
using API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.Error;

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
        [Authorize(Roles = "Paciente,Medico,Enfermero")] 

        public async Task<ActionResult> Post([FromBody] ModelConfirmacion parametros)
        {
            var userIdString = HttpContext.Items["CodigoToken_UserId"] as string;
            var codigoString = HttpContext.Items["CodigoToken_Codigo"] as string;
            
            int userId = int.Parse(userIdString);

            int codigo = int.Parse(codigoString);

            if (codigo != parametros.Codigo)
            {
                throw new CodigoIncorrectoException();
            }


            await _dataRegistro.ConfirmarVerificacion(userId);

            return Ok(new { mensaje = "Código confirmado correctamente" });
        }
    }
}